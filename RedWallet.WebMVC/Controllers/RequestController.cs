using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Models.RequestModels;
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
    public class RequestController : Controller
    {
        private readonly IBitcoinService _btc;
        private readonly IWalletService _wallet;
        private readonly IRequestService _req;

        public RequestController(IBitcoinService btc, IWalletService wallet, IRequestService req)
        {
            _btc = btc;
            _wallet = wallet;
            _req = req;
        }

        // GET: Request
        public async Task<ActionResult> Index(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var model = await _req.GetWalletRequestsAsync(walletIdentity);
            return View(model);
        }

        // GET: Request Create
        // Wallet/{walletId}/Request/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var detail = await _wallet.GetWalletByIdAsync(walletIdentity);
            var model = new RequestCreate
            {
                WalletId = detail.WalletId,
                WalletName = detail.WalletName,
                Passphrase = ""
            };

            return View(model);
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

            var walletIdentity = new WalletIdentity { WalletId = model.WalletId, UserId = User.Identity.GetUserId() };
            var walletEncryptedSecret = await _wallet.GetWalletEncryptedSecretAsync(walletIdentity);
            var requestAddress = _btc.GetNewBitcoinAddress(walletEncryptedSecret, model.Passphrase);
            
            if (requestAddress != null)
            {
                var detail = await _req.CreateRequestAsync(walletIdentity, requestAddress.ToString());
                return Redirect($"/Wallet/{detail.WalletId}/request/{detail.Id}/details");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);
        }

        // GET: Request Details
        // Wallet/{walletId}/Request/{id}/Details
        public async Task<ActionResult> Details(int id)
        {
            var requestIdentity = new RequestIdentity { RequestId = id, UserId = User.Identity.GetUserId() };
            var model = await _req.GetWalletRequestByIdAsync(requestIdentity);
            return View(model);
        }

        // UPDATE: No Update for Requets

        // GET: Delete Request (for db management)
        // Request/Delete/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var requestIdentity = new RequestIdentity { RequestId = id, UserId = User.Identity.GetUserId() };
            var model = await _req.GetWalletRequestByIdAsync(requestIdentity);
            return View(model);
        }
        // POST: Delete Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteRequest(int id)
        {
            var requestIdentity = new RequestIdentity { RequestId = id, UserId = User.Identity.GetUserId() };
            var walletId = (await _req.GetWalletRequestByIdAsync(requestIdentity)).WalletId;

            if (await _req.DeleteRequestAsync(requestIdentity))
            {
                TempData["DeleteResult"] = "Request data deleted";
                return RedirectToAction($"Index/{walletId}");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(id);


        }
    }
}