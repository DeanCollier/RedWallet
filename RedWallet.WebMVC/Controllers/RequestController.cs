using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Models.RequestModels;
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

        // Wallet Details => display wallet name and ask for passphrase => create new request with wallet id and passphrase


        // GET: Request
        public async Task<ActionResult> Index(int walletId)
        {
            var requestService = CreateRequestService();
            var model = await requestService.GetWalletRequestsAsync(walletId);
            return View(model);
        }

        // GET: Request Create
        // Wallet/{walletId}/Request/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var requestService = CreateWalletService();
            var detail = await requestService.GetWalletByIdAsync(walletId);
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

            var walletService = CreateWalletService();
            var btcService = CreateBitcoinService();
            var requestService = CreateRequestService();

            var walletEncryptedSecret = await walletService.GetWalletEncryptedSecretAsync(model.WalletId);
            var requestAddress = btcService.GetNewBitcoinAddress(walletEncryptedSecret, model.Passphrase);
            
            if (requestAddress != null)
            {
                var detail = await requestService.CreateRequestAsync(model.WalletId, requestAddress.ToString());
                return Redirect($"/Wallet/{detail.WalletId}/request/{detail.Id}/details");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);
        }

        // GET: Request Details
        // Wallet/{walletId}/Request/{id}/Details
        public async Task<ActionResult> Details(int id)
        {
            var requestService = CreateRequestService();
            var model = await requestService.GetWalletRequestByIdAsync(id);
            return View(model);
        }

        // UPDATE: No Update for Requets

        // GET: Delete Request (for db management)
        // Request/Delete/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var requestService = CreateRequestService();
            var model = await requestService.GetWalletRequestByIdAsync(id);
            return View(model);
        }
        // POST: Delete Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteRequest(int id)
        {
            var requestService = CreateRequestService();
            var walletId = (await requestService.GetWalletRequestByIdAsync(id)).WalletId;

            if (await requestService.DeleteRequestAsync(id))
            {
                TempData["DeleteResult"] = "Request data deleted";
                return RedirectToAction($"Index/{walletId}");
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(id);


        }
    }
}