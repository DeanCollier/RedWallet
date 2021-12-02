using Microsoft.AspNet.Identity;
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
    public class BTCInfoController : Controller
    {
        private readonly IWalletService _wallet;
        private readonly ISendService _send;
        private readonly IRequestService _req;
        private readonly IBitcoinService _btc;

        public BTCInfoController(IWalletService wallet, ISendService send, IRequestService req, IBitcoinService btc)
        {
            _wallet = wallet;
            _send = send;
            _req = req;
            _btc = btc;
        }

        // GET: Update all wallets BTC info
        [Route("{id}")]
        public async Task<ActionResult> Update(int? id)
        {
            var userId = User.Identity.GetUserId();
            try
            {
                if (id.HasValue)
                {
                    var walletIdentity = new WalletIdentity { UserId = userId, WalletId = id.GetValueOrDefault() };
                    var xpub = await _btc.GetXpub(await _wallet.GetWalletXpubAsync(walletIdentity));
                    // find change and receive children
                    var nextRecChild = _btc.FindNextReceivingChildPosition(xpub);
                    var nextChngChild = _btc.FindNextChangeChildPosition(xpub);
                    await Task.WhenAll(nextRecChild, nextRecChild);
                    // find balance
                    var balance = await _btc.FindBitcoinBalance(xpub, nextRecChild.Result, nextChngChild.Result);
                    // update ballance and children in wallets
                    var model = new WalletBTCInfo
                    {
                        WalletId = id.GetValueOrDefault(),
                        UserId = userId,
                        LatestBalance = balance,
                        NextReceiveChild = nextRecChild.Result,
                        NextChangeChild = nextChngChild.Result
                    };
                    await _wallet.UpdateWalletBTCInfo(model);
                    return RedirectToAction("Index", "Dashboard", new { id = id.GetValueOrDefault() });
                }
                else
                {
                    var userWallets = await _wallet.GetWalletsAsync(userId);
                    foreach (var wallet in userWallets)
                    {
                        var walletIdentity = new WalletIdentity { UserId = userId, WalletId = wallet.WalletId };
                        var xpub = await _btc.GetXpub(await _wallet.GetWalletXpubAsync(walletIdentity));

                        var nextRecChild = await _btc.FindNextReceivingChildPosition(xpub);
                        var nextChngChild = await _btc.FindNextChangeChildPosition(xpub);

                        var balance = await _btc.FindBitcoinBalance(xpub, nextRecChild, nextChngChild);

                        var model = new WalletBTCInfo
                        {
                            WalletId = wallet.WalletId,
                            UserId = userId,
                            LatestBalance = balance,
                            NextReceiveChild = nextRecChild,
                            NextChangeChild = nextChngChild
                        };
                        await _wallet.UpdateWalletBTCInfo(model);
                    }
                    return RedirectToAction("Index", "Dashboard");

                }
            }
            catch
            {
                return RedirectToAction("Index", "Dashboard");
            }

        }

       
    }
}