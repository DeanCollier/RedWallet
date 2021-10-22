using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Models.WalletModels;
using RedWallet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    public class WalletController : Controller
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
        
        // GET: Wallet
        public async Task<ActionResult> Index()
        {
            var walletService = CreateWalletService();
            var model = await walletService.GetWalletsAsync();
            return View(model);
        }

        // GET: Wallet Create
        // Wallet/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }
        // POST: Create Wallet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WalletCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var btcService = CreateBitcoinService();
            var newWalletDetail = await btcService.CreateWallet(model);
            var walletService = CreateWalletService();

            if (newWalletDetail.Address != null)
            {
                if (await walletService.CreateWalletAsync(model, newWalletDetail))
                {
                    TempData["SaveResult"] = "Your wallet was created.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Note could not be created.");
                return View(model);
            }
            return View(model);
        }
    }
}