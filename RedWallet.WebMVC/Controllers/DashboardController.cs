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
        private readonly ITransactionService _trans;
        private readonly IAddressService _addr;
        private readonly IBitcoinService _btc;

        public DashboardController(IWalletService wallet, ITransactionService trans, IAddressService addr, IBitcoinService btc)
        {
            _wallet = wallet;
            _trans = trans;
            _addr = addr;
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
                model.WalletBalance = (await _wallet.GetWalletBTCInfoAsync(walletIdentity)).LatestBalance;
            }
            model.WalletTransactions = (await _trans.GetWalletTransactionsAsync(walletIdentity)).ToList();
            model.WalletAddresses = (await _addr.GetWalletAddressesAsync(walletIdentity)).ToList();
            return View(model);
        }

        
    }
}