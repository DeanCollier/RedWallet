using Microsoft.AspNet.Identity;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.TransactionModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    [Authorize]

    public class TransactionController : Controller
    {
        private readonly IBitcoinService _btc;
        private readonly IWalletService _wallet;
        private readonly ITransactionService _trans;

        public TransactionController(IBitcoinService btc, IWalletService wallet, ITransactionService trans)
        {
            _btc = btc;
            _wallet = wallet;
            _trans = trans;
        }

        // GET: Transaction
        // Wallet/{walletId}/Transaction/Index
        public async Task<ActionResult> Index(int? walletId)
        {
            if (!walletId.HasValue)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var walletIdentity = new WalletIdentity { WalletId = walletId.GetValueOrDefault(), UserId = User.Identity.GetUserId() };
            var model = await _trans.GetWalletTransactionsAsync(walletIdentity);
            var walletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName;
            ViewData["WalletId"] = walletId;
            ViewData["WalletName"] = walletName;
            return View(model);
        }

        // GET: Create Transaction
        // Wallet/{walletId}/Transaction/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var walletDetail = await _wallet.GetWalletByIdAsync(walletIdentity);

            var balance = (await _wallet.GetWalletBTCInfoAsync(walletIdentity)).LatestBalance;

            var model = new TransactionSend
            {
                WalletId = walletDetail.WalletId,
                WalletName = walletDetail.WalletName,
                WalletBalance = balance,
                SendAmount = 0m,
                RecipientAddress = "",
                WalletPassphrase = ""
            };

            return View(model);
        }
        // POST: Create Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TransactionSend model)
        {
            var walletIdentity = new WalletIdentity { WalletId = model.WalletId, UserId = User.Identity.GetUserId() };
            var encryptedSecret = await _wallet.GetWalletEncryptedSecretAsync(walletIdentity);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.WalletBalance <= model.SendAmount)
            {
                ModelState.AddModelError("", "Insufficient funds.");
                return View(model);
            }
            if (!(await _btc.IsValidAddress(model.RecipientAddress)))
            {
                ModelState.AddModelError("", "Recipient Bitcoin Address is not a valid Bitcoin address.");
                return View(model);
            }
            if (!(await _btc.IsBitcoinSecret(encryptedSecret, model.WalletPassphrase)))
            {
                ModelState.AddModelError("", "Incorrect wallet passphrase.");
                return View(model);
            }

            var walletEncryptedSecret = await _wallet.GetWalletEncryptedSecretAsync(walletIdentity);
            // return transaction hash here
            var transactionHash = _btc.BuildTransaction(walletEncryptedSecret, model.WalletPassphrase, model.SendAmount, model.RecipientAddress);

            var newTransaction = new TransactionCreate
            {
                TransactionHash = transactionHash,
                WalletId = model.WalletId,
                TotalAmount = model.SendAmount, // need to get this from bitcoin service to include fees, also replace if statement to check wallet amount vs total send amount
                IsSend = true
            };

            if (transactionHash != null)
            {
                var detail = await _trans.CreateTransactionAsync(walletIdentity, newTransaction);
                TempData["SaveResult"] = "Transaction has been sent.";
                return RedirectToAction("Details", new { id = detail.TransactionId });
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);
        }

        // GET: Transaction Details
        // Wallet/{walletId}/Transaction/{id}/Details
        [Route("{id}")]
        public async Task<ActionResult> Details(int id)
        {
            var transactionIdentity = new TransactionIdentity { TransactionId = id, UserId = User.Identity.GetUserId() };
            var model = await _trans.GetWalletTransactionByIdAsync(transactionIdentity);
            return View(model);
        }

        // UPDATE: No Update for Transactions

        // GET: Delete Address (for db management)
        // Wallet/{walletId}/Transaction/{id}/Delete
        public async Task<ActionResult> Delete(int id)
        {
            var transactionIdentity = new TransactionIdentity { TransactionId = id, UserId = User.Identity.GetUserId() };
            var model = await _trans.GetWalletTransactionByIdAsync(transactionIdentity);
            return View(model);
        }
        // POST: Delete Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var transactionIdentity = new TransactionIdentity { TransactionId = id, UserId = User.Identity.GetUserId() };
            var walletId = (await _trans.GetWalletTransactionByIdAsync(transactionIdentity)).WalletId;

            if (await _trans.DeleteTransactionAsync(transactionIdentity))
            {
                TempData["DeleteDate"] = "Transaction data deleted";
                return RedirectToAction("Index", new { walletId = walletId });
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(id);
        }
    }
}