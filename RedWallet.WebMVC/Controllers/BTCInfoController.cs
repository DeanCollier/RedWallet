using Microsoft.AspNet.Identity;
using RedWallet.Models.RequestModels;
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
        [Route("{walletId}")]
        public async Task<ActionResult> Update(int walletId)
        {
            var userId = User.Identity.GetUserId();
            try
            {
                var walletIdentity = new WalletIdentity { UserId = userId, WalletId = walletId };
                await UpdateBTCInfo(walletIdentity);
                await UpdateUsedAddresses(walletIdentity);
                //await UpdatePastTransactions(walletIdentity, info);
                return RedirectToAction("Index", "Dashboard", new { id = walletId });
            }
            catch
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        private async Task<WalletBTCInfo> UpdateBTCInfo(WalletIdentity walletIdentity)
        {
            var xpub = await _btc.GetXpub(await _wallet.GetWalletXpubAsync(walletIdentity));
            // find change and receive children
            var nextRecChild = await _btc.FindNextReceivingChildPosition(xpub);
            var nextChngChild = await _btc.FindNextChangeChildPosition(xpub);
            // find balance
            var balance = await _btc.FindBitcoinBalance(xpub, nextRecChild, nextChngChild);
            // update ballance and children in wallets
            var model = new WalletBTCInfo
            {
                WalletId = walletIdentity.WalletId,
                UserId = walletIdentity.UserId,
                LatestBalance = balance,
                NextReceiveChild = nextRecChild,
                NextChangeChild = nextChngChild
            };
            await _wallet.UpdateWalletBTCInfo(model);
            return await _wallet.GetWalletBTCInfoAsync(walletIdentity);
        }
        private async Task UpdateUsedAddresses(WalletIdentity walletIdentity)
        {
            ICollection<string> currentAddresses =
                (await _req.GetWalletRequestsAsync(walletIdentity))
                .Select(r => r.RequestAddress).ToArray();

            var nextRecChild = (await _wallet.GetWalletBTCInfoAsync(walletIdentity)).NextReceiveChild;
            var xpub = await _wallet.GetWalletXpubAsync(walletIdentity);
            for (int i = 0; i < nextRecChild; i++)
            {
                var address = (await _btc.DeriveAddress(xpub, false, i)).ToString();
                if (!(currentAddresses.Contains(address)))
                {
                    await _req.CreateRequestAsync(walletIdentity, address);
                }
            }
        }
        




    }
}