using Microsoft.AspNet.Identity;
using NBitcoin;
using QRCoder;
using RedWallet.Models.RequestModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services;
using RedWallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    [Authorize]

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
        public async Task<ActionResult> Index(int? walletId)
        {
            if (!walletId.HasValue)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var walletIdentity = new WalletIdentity { WalletId = walletId.GetValueOrDefault(), UserId = User.Identity.GetUserId() };
            var model = await _req.GetWalletRequestsAsync(walletIdentity);
            var walletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName;
            ViewData["WalletId"] = walletId;
            ViewData["WalletName"] = walletName;
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
            var wallet = await _wallet.GetWalletByIdAsync(walletIdentity);
            
            var xpub = wallet.Xpub;
            var walletEncryptedSecret = await _wallet.GetWalletEncryptedSecretAsync(walletIdentity);

            var newAddress = await _btc.GetNewReceivingAddress(xpub);
            
            if (newAddress != null)
            {
                var detail = await _req.CreateRequestAsync(walletIdentity, newAddress.ToString());
                TempData["SaveResult"] = "Share the public address below to receive bitcoin from others.";
                return Redirect($"Details/{detail.RequestId}");
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
            
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(model.RequestAddress, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeInfo);

                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

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
                return RedirectToAction($"Index", new { walletId = walletId });
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(id);


        }
    }
}