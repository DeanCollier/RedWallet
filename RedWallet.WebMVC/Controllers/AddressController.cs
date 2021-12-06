using Microsoft.AspNet.Identity;
using NBitcoin;
using QRCoder;
using RedWallet.Models.AddressModels;
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

    public class AddressController : Controller
    {
        private readonly IBitcoinService _btc;
        private readonly IWalletService _wallet;
        private readonly IAddressService _addr;

        public AddressController(IBitcoinService btc, IWalletService wallet, IAddressService addr)
        {
            _btc = btc;
            _wallet = wallet;
            _addr = addr;
        }

        // GET: Address
        public async Task<ActionResult> Index(int? walletId)
        {
            if (!walletId.HasValue)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var walletIdentity = new WalletIdentity { WalletId = walletId.GetValueOrDefault(), UserId = User.Identity.GetUserId() };
            var model = await _addr.GetWalletAddressesAsync(walletIdentity);
            var walletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName;
            ViewData["WalletId"] = walletId;
            ViewData["WalletName"] = walletName;
            return View(model);
        }

        // GET: Address Create
        // Wallet/{walletId}/Address/Create
        public async Task<ActionResult> Create(int walletId)
        {
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var model = await _wallet.GetWalletByIdAsync(walletIdentity);
           
            return View(model);
        }
        // POST: Create Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<ActionResult> CreateAddress(int walletId)
        {
            /*if (!ModelState.IsValid)
            {
                return View(model);
            }*/

            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            var wallet = await _wallet.GetWalletByIdAsync(walletIdentity);

            var xpub = wallet.Xpub;
            var newAddress = await _btc.GetNewReceivingAddress(xpub);

            var model = new AddressCreate
            {
                WalletId = walletId,
                WalletName = wallet.WalletName,
                PublicAddress = "",
                IsChange = false
            };

            if (newAddress != null)
            {
                model.PublicAddress = newAddress.ToString();
                var detail = await _addr.CreateAddressAsync(walletIdentity, model);
                TempData["SaveResult"] = "Share the public address below to receive bitcoin from others.";
                return RedirectToAction("Details", new { pa = detail.PublicAddress });
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(model);
            

        }

        // GET: Address Details
        // Wallet/{walletId}/Address/{id}/Details
        public async Task<ActionResult> Details(string pa)
        {
            var addressIdentity = new AddressIdentity { PublicAddress = pa, UserId = User.Identity.GetUserId() };
            var model = await _addr.GetWalletAddressByIdAsync(addressIdentity);
            
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(model.PublicAddress, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeInfo);

                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            return View(model);
        }

        // UPDATE: No Update Controller for Addresses

        // GET: Delete Address (for db management)
        // Address/Delete/{id}
        public async Task<ActionResult> Delete(string pa)
        {
            var addressIdentity = new AddressIdentity { PublicAddress = pa, UserId = User.Identity.GetUserId() };
            var model = await _addr.GetWalletAddressByIdAsync(addressIdentity);
            return View(model);
        }
        // POST: Delete Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAddress(string pa)
        {
            var addressIdentity = new AddressIdentity { PublicAddress = pa, UserId = User.Identity.GetUserId() };
            var walletId = (await _addr.GetWalletAddressByIdAsync(addressIdentity)).WalletId;

            if (await _addr.DeleteAddressAsync(addressIdentity))
            {
                TempData["DeleteResult"] = "Address data deleted";
                return RedirectToAction($"Index", new { walletId = walletId });
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View(pa);


        }
    }
}