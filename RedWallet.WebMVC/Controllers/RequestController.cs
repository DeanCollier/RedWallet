using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Models.ReceiveModels;
using RedWallet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    public class RequestController : Controller
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
        private RequestService CreateRequestService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new RequestService(userId);
            return service;
        }

        // GET: Request
        public async Task<ActionResult> Index(int walletId)
        {
            var requestService = CreateRequestService();
            var model = requestService.GetWalletRequestsAsync(walletId);
            return View(model);
        }

        // GET: Request Create
        // Wallet/{id}/Request/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }
        // POST: Create Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RequestCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var walletService = CreateWalletService();
            var btcService = CreateBitcoinService();
            var requestService = CreateRequestService();

            var walletEncryptedSecret = await walletService.GetWalletEncryptedSecret(model.WalletId);
            var requestAddress = btcService.GetNewBitcoinAddress(walletEncryptedSecret, model.Passphrase);
            
            if (requestAddress != null)
            {
                var detail = requestService.CreateRequestAsync(model.WalletId, requestAddress.ToString());
                return RedirectToAction($"Details/{detail.Id}");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);




        }
    }
}