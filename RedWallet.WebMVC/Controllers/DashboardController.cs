using Microsoft.AspNet.Identity;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.DashboardModels;
using RedWallet.Models.WalletModels;
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
    public class DashboardController : Controller
    {
        private readonly IWalletService _wallet;
        private readonly ISendService _send;
        private readonly IRequestService _req;
        private readonly IBitcoinService _btc;

        public DashboardController(IWalletService wallet, ISendService send, IRequestService req, IBitcoinService btc)
        {
            _wallet = wallet;
            _send = send;
            _req = req;
            _btc = btc;
        }

        // GET: Dashboard
        [Route("{id}")]
        public async Task<ActionResult> Index(int? id)
        {
            var model = new DashboardViewModel();
            var userId = User.Identity.GetUserId();
            model.UserWallets = (await _wallet.GetWalletsAsync(userId)).ToList();
            var firstWallet = model.UserWallets.FirstOrDefault();

            if (id.HasValue)
            {
                model.SelectedWalletId = id.GetValueOrDefault();
            }
            else
            {
                if (firstWallet != null)
                {
                    model.SelectedWalletId = firstWallet.WalletId;
                }
                else
                {
                    model.SelectedWalletId = 0;
                }
            }

            var walletIdentity = new WalletIdentity { WalletId = model.SelectedWalletId, UserId = userId };
            if (model.SelectedWalletId != 0)
            {
                var walletXpub = await _btc.GetXpub(await _wallet.GetWalletXpubAsync(walletIdentity));
                model.WalletBalance = await _btc.FindBitcoinBalance(walletXpub);
            }
            model.WalletSends = (await _send.GetWalletSendsAsync(walletIdentity)).ToList();
            model.WalletAddresses = (await _req.GetWalletRequestsAsync(walletIdentity)).ToList();
            return View(model);
        }

        // GET: Dashboard wallet balance
        [Route("{id}")]
        public async Task<ActionResult> BalancePartialView(int id)
        {
            var userId = User.Identity.GetUserId();
            var walletIdentity = new WalletIdentity { WalletId = id, UserId = userId };

            var model = new BalanceDetail();

            var walletXpub = await _btc.GetXpub(await _wallet.GetWalletXpubAsync(walletIdentity));
            model.WalletBalance = await _btc.FindBitcoinBalance(walletXpub);

            return View(model);
        }
    }
}