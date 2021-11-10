using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.SendModels;
using RedWallet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    public class SendController : Controller
    {
        private BitcoinService CreateBitcoinService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            Network network = Network.RegTest;
            var rpcHost = "127.0.0.1:18444";
            var rpcCredentials = "lightningbbobb:ViresEnNumeris";
            var service = new BitcoinService(network, rpcHost, rpcCredentials, userId);
            return service;
        }
        private WalletService CreateWalletService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new WalletService(userId);
            return service;
        }
        private SendService CreateSendService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new SendService(userId);
            return service;
        }

        // GET: Send
        // Wallet/{walletId}/Send/Index
        public async Task<ActionResult> Index(int walletId)
        {
            var sendService = CreateSendService();
            var model = await sendService.GetWalletSendsAsync(walletId);
            return View(model);
        }

        // GET: Create Send
        // Wallet/{walletId}/Send/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var walletService = CreateWalletService();
            var walletDetail = await walletService.GetWalletByIdAsync(walletId);
            var encSecret = await walletService.GetWalletEncryptedSecretAsync(walletId);

            //********* get btc balance of wallet
            var btcService = CreateBitcoinService();
            //********* var balance = btcService.GetWalletBalance( walletSecret or encSecret or something)
            double balance = 100; // filler for now so we can get app working

            var model = new TransactionCreate
            {
                WalletId = walletDetail.WalletId,
                WalletName = walletDetail.WalletName,
                Balance = balance,
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
            var btcService = CreateBitcoinService();

            if (!ModelState.IsValid ||
                (model.Balance < model.SendAmount) ||
                (!btcService.IsValidWallet(model.RecipientAddress)))
            {
                return View(model);
            }
            
            var walletService = CreateWalletService();
            var sendService = CreateSendService();

            var walletEncryptedSecret = await walletService.GetWalletEncryptedSecretAsync(model.WalletId);
            var transactionHash = btcService.BuildTransaction(walletEncryptedSecret, model.WalletPassword, model.SendAmount, model.RecipientAddress);

            if (transactionHash != null)
            {
                var detail = await sendService.CreateSendAsync(model.WalletId, transactionHash);
                return Redirect($"Wallet/{detail.WalletId}/Send/{detail.SendId}/Details");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);
        }

        // GET: Send Details
        // Wallet/{walletId}/Send/{id}/Details
        public async Task<ActionResult> Details(int id)
        {
            var sendService = CreateSendService();
            var model = await sendService.GetWalletSendByIdAsync(id);
            return View(model);
        }

        // UPDATE: No Update for Sends

        // GET: Delete Request (for db management)
        // Wallet/{walletId}/Send/{id}/Delete
        public async Task<ActionResult> Delete(int id)
        {
            var sendService = CreateSendService();
            var model = await sendService.GetWalletSendByIdAsync(id);
            return View(model);
        }
        // POST: Delete Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteSend(int id)
        {
            var sendService = CreateSendService();
            var walletId = (await sendService.GetWalletSendByIdAsync(id)).WalletId;

            if (await sendService.DeleteSendAsync(id))
            {
                TempData["DeleteDate"] = "Send data deleted";
                return RedirectToAction($"Index/{walletId}");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(id);
        }
    }
}