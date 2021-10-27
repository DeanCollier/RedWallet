using Microsoft.AspNet.Identity;
using NBitcoin;
using RedWallet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: Request
        public ActionResult Index()
        {
            return View();
        }
    }
}