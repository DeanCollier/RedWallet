using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.SendModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services;
using RedWallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    [Authorize]

    public class SendController : Controller
    {
        private readonly IBitcoinService _btc;
        private readonly IWalletService _wallet;
        private readonly ISendService _send;

        public SendController(IBitcoinService btc, IWalletService wallet, ISendService send)
        {
            _btc = btc;
            _wallet = wallet;
            _send = send;
        }

        // GET: Send
        // Wallet/{walletId}/Send/Index
        public async Task<ActionResult> Index(int? walletId)
        {
            if (!walletId.HasValue)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var walletIdentity = new WalletIdentity { WalletId = walletId.GetValueOrDefault(), UserId = User.Identity.GetUserId() };
            var model = await _send.GetWalletSendsAsync(walletIdentity);
            var walletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName;
            ViewData["WalletId"] = walletId;
            ViewData["WalletName"] = walletName;
            return View(model);
        }

        // GET: Create Send
        // Wallet/{walletId}/Send/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var walletDetail = await _wallet.GetWalletByIdAsync(walletIdentity);
            var stringXpub = await _wallet.GetWalletXpubAsync(walletIdentity);
            var xpub = await _btc.GetXpub(stringXpub);

            var balance = (await _wallet.GetWalletBTCInfoAsync(walletIdentity)).LatestBalance;

            var model = new TransactionCreate
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
        // POST: Create Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TransactionCreate model)
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
            var transactionHash = _btc.BuildTransaction(walletEncryptedSecret, model.WalletPassphrase, model.SendAmount, model.RecipientAddress);

            if (transactionHash != null)
            {
                var detail = await _send.CreateSendAsync(walletIdentity, transactionHash);
                TempData["SaveResult"] = "Transaction has been sent.";
                return Redirect($"Details/{detail.SendId}");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);
        }

        // GET: Send Details
        // Wallet/{walletId}/Send/{id}/Details
        public async Task<ActionResult> Details(int id)
        {
            var sendIdentity = new SendIdentity { SendId = id, UserId = User.Identity.GetUserId() };
            var model = await _send.GetWalletSendByIdAsync(sendIdentity);
            return View(model);
        }

        // UPDATE: No Update for Sends

        // GET: Delete Request (for db management)
        // Wallet/{walletId}/Send/{id}/Delete
        public async Task<ActionResult> Delete(int id)
        {
            var sendIdentity = new SendIdentity { SendId = id, UserId = User.Identity.GetUserId() };
            var model = await _send.GetWalletSendByIdAsync(sendIdentity);
            return View(model);
        }
        // POST: Delete Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteSend(int id)
        {
            var sendIdentity = new SendIdentity { SendId = id, UserId = User.Identity.GetUserId() };
            var walletId = (await _send.GetWalletSendByIdAsync(sendIdentity)).WalletId;

            if (await _send.DeleteSendAsync(sendIdentity))
            {
                TempData["DeleteDate"] = "Send data deleted";
                return RedirectToAction("Index", new { walletId = walletId });
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(id);
        }
    }
}