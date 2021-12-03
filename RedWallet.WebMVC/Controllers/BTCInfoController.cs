using Microsoft.AspNet.Identity;
using RedWallet.Models.AddressModels;
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
        private readonly ITransactionService _trans;
        private readonly IAddressService _addr;
        private readonly IBitcoinService _btc;

        public BTCInfoController(IWalletService wallet, ITransactionService trans, IAddressService addr, IBitcoinService btc)
        {
            _wallet = wallet;
            _trans = trans;
            _addr = addr;
            _btc = btc;
        }

        // GET: Update all user wallets BTC info
        public async Task UpdateAll()
        {
            var userId = User.Identity.GetUserId();

            IEnumerable<WalletListItem> userWallets = await _wallet.GetWalletsAsync(userId);
            foreach (var item in userWallets)
            {
                var walletIdentity = new WalletIdentity { UserId = userId, WalletId = item.WalletId };
                await UpdateBTCInfo(walletIdentity);
                await UpdateUsedAddresses(walletIdentity);
                //await UpdatePastTransactions(walletIdentity, info);
                
            }
            return;
        }

        // GET: Update single wallet BTC info
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

        private async Task UpdateBTCInfo(WalletIdentity walletIdentity)
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
        }
        private async Task UpdateUsedAddresses(WalletIdentity walletIdentity)
        {
            ICollection<string> currentAddresses =
                (await _addr.GetWalletAddressesAsync(walletIdentity))
                .Select(a => a.PublicAddress).ToArray();

            var nextRecChild = (await _wallet.GetWalletBTCInfoAsync(walletIdentity)).NextReceiveChild;
            var xpub = await _wallet.GetWalletXpubAsync(walletIdentity);
            for (int i = 0; i < nextRecChild; i++)
            {
                var address = (await _btc.DeriveAddress(xpub, false, i)).ToString();
                if (!(currentAddresses.Contains(address)))
                {
                    var newAddress = new AddressCreate
                    {
                        PublicAddress = address,
                        IsChange = false,
                        WalletId = walletIdentity.WalletId,
                        WalletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName
                    };
                    await _addr.CreateAddressAsync(walletIdentity, newAddress);
                }
            }
        }
        




    }
}