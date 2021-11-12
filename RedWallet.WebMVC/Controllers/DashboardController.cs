using Microsoft.AspNet.Identity;
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
    public class DashboardController : Controller
    {
        private readonly IWalletService _wallet;
        private readonly ISendService _send;
        private readonly IRequestService _req;

        public DashboardController(IWalletService wallet, ISendService send, IRequestService req)
        {
            _wallet = wallet;
            _send = send;
            _req = req;
        }

        // GET: Dashboard
        public async Task<ActionResult> Index(int walletId)
        {
            var model = new DashboardViewModel();
            var walletIdentity = new WalletIdentity { WalletId = walletId, UserId = User.Identity.GetUserId() };
            model.UserWallets = await _wallet.GetWalletsAsync(User.Identity.GetUserId());
            model.WalletSends = await _send.GetWalletSendsAsync(walletIdentity);
            model.WalletAddresses = await _req.GetWalletRequestsAsync(walletIdentity);
            return View();
        }
    }
}