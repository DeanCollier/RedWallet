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
            var newKeyDetail = await btcService.GetNewBitcoinKey(model);

            var walletService = CreateWalletService();
            var newWallet = await walletService.CreateWalletAsync(model, newKeyDetail);

            if (!(string.IsNullOrEmpty(newWallet[0])) && !(string.IsNullOrEmpty(newWallet[1])) && !(string.IsNullOrEmpty(newWallet[2]))) // passphrase & mnemonic
            {
                TempData["SaveResult"] = "Your wallet was created.";
                TempData["Passphrase"] = newWallet[0]; 
                TempData["Mnemonic"] = newWallet[1];
                var id = int.Parse(newWallet[2]);
                return RedirectToAction($"Details/{id}"); 
            }
            ModelState.AddModelError("", "Wallet could not be created.");
            return View(model);
        }

        // GET: Wallet Detail
        // Wallet/Detail/{id}
        public async Task<ActionResult> Details(int id)
        {
            var service = CreateWalletService();
            var model = await service.GetWalletByIdAsync(id);

            return View(model);
        }

        // EDIT: Edit Wallet
        // Wallet/Edit/{id}
        public async Task<ActionResult> Edit(int id)
        {
            var service = CreateWalletService();
            var detail = await service.GetWalletByIdAsync(id);
            var model = new WalletEdit
            {
                WalletId = detail.WalletId,
                NewWalletName = detail.WalletName
            };

            return View(model);
        }
        // POST: Update Wallet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, WalletEdit model)
        {
            if (model.WalletId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var service = CreateWalletService();

                if (await service.UpdateWalletById(model))
                {
                    TempData["SaveResult"] = "Your wallet name has been updated.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Your wallet name could not be changed.");
                return View(model);
            }
            return View(model);
        }

        // GET: Delete Wallet
        // Wallet/Delete/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var service = CreateWalletService();
            var model = await service.GetWalletByIdAsync(id);

            return View(model);
        }
        // POST: Delete Wallet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteWallet(int id)
        {
            var service = CreateWalletService();

            if (await service.DeleteWalletAsync(id))
            {
                TempData["SaveResult"] = "Your wallet was deleted";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Your wallet could not be deleted.");
            return View();
        }
    }
}