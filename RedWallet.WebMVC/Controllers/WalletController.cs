using Microsoft.AspNet.Identity;
using NBitcoin;
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
    public class WalletController : Controller
    {
        private readonly IBitcoinService _btc;
        private readonly IWalletService _wallet;


        public WalletController(IBitcoinService btc, IWalletService wallet)
        {
            _btc = btc;
            _wallet = wallet;
        }
        
        // GET: Wallet
        public async Task<ActionResult> Index()
        {
            var model = await _wallet.GetWalletsAsync(User.Identity.GetUserId());
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

            model.UserId = User.Identity.GetUserId();
            var newKeyDetail = await _btc.GetNewBitcoinKey(model);
            var newWallet = await _wallet.CreateWalletAsync(model, newKeyDetail);

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
            var walletIdentity = new WalletIdentity { WalletId = id, UserId = User.Identity.GetUserId() };
            var detail = await _wallet.GetWalletByIdAsync(walletIdentity);

            return View(detail);
        }

        // EDIT: Edit Wallet
        // Wallet/Edit/{id}
        public async Task<ActionResult> Edit(int id)
        {
            var walletIdentity = new WalletIdentity { WalletId = id, UserId = User.Identity.GetUserId() };
            var detail = await _wallet.GetWalletByIdAsync(walletIdentity);
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
                model.UserId = User.Identity.GetUserId();
                if (await _wallet.UpdateWalletById(model))
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
            var walletIdentity = new WalletIdentity { WalletId = id, UserId = User.Identity.GetUserId() };
            var model = await _wallet.GetWalletByIdAsync(walletIdentity);
            return View(model);
        }
        // POST: Delete Wallet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteWallet(int id)
        {
            var walletIdentity = new WalletIdentity { WalletId = id, UserId = User.Identity.GetUserId() };

            if (await _wallet.DeleteWalletAsync(walletIdentity))
            {
                TempData["SaveResult"] = "Your wallet was deleted";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Your wallet could not be deleted.");
            return View(id);
        }
    }
}