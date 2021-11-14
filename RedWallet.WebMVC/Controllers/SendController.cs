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
        public async Task<ActionResult> Index(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var model = await _send.GetWalletSendsAsync(walletIdentity);
            ViewData["WalletId"] = walletId;
            return View(model);
        }

        // GET: Create Send
        // Wallet/{walletId}/Send/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var walletDetail = await _wallet.GetWalletByIdAsync(walletIdentity);
            var encSecret = await _wallet.GetWalletEncryptedSecretAsync(walletIdentity);

            //********* get btc balance of wallet
            var balance = _btc.GetBitcoinBalance();

            var model = new TransactionCreate
            {
                WalletId = walletDetail.WalletId,
                WalletName = walletDetail.WalletName,
                Balance = double.Parse(balance),
                SendAmount = 0,
                RecipientAddress = "",
                WalletPassword = ""
            };

            return View(model);
        }
        // POST: Create Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TransactionCreate model)
        {
            if (!ModelState.IsValid ||
                (model.Balance < model.SendAmount) ||
                (!_btc.IsValidWallet(model.RecipientAddress))) // need to move this below to check if password works for specified wallet
            {
                return View(model);
            }

            var walletIdentity = new WalletIdentity { WalletId = model.WalletId, UserId = User.Identity.GetUserId() };

            var walletEncryptedSecret = await _wallet.GetWalletEncryptedSecretAsync(walletIdentity);
            var transactionHash = _btc.BuildTransaction(walletEncryptedSecret, model.WalletPassword, model.SendAmount, model.RecipientAddress);

            if (transactionHash != null)
            {
                var detail = await _send.CreateSendAsync(walletIdentity, transactionHash);
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