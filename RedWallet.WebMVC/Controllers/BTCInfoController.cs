using Microsoft.AspNet.Identity;
using RedWallet.Models.AddressModels;
using RedWallet.Models.TransactionModels;
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
                await UpdatePastTransactions(walletIdentity);
                
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
                await UpdatePastTransactions(walletIdentity);
                return RedirectToAction("Index", "Dashboard", new { id = walletId });
            }
            catch
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        private async Task UpdateBTCInfo(WalletIdentity walletIdentity)
        {
            var xpub = await _wallet.GetWalletXpubAsync(walletIdentity);
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
            // compare used addresses to addresses already in the db
            // if already in db, just updating date of first use from the network
            IEnumerable<AddressListItem> currentAddresses = (await _addr.GetWalletAddressesAsync(walletIdentity));
            IEnumerable<string> currentAddressValues = currentAddresses.Select(a => a.PublicAddress).ToArray();

            var walletBTCInfo = await _wallet.GetWalletBTCInfoAsync(walletIdentity);
            var nextRecChild = walletBTCInfo.NextReceiveChild;
            var nextChngChild = walletBTCInfo.NextChangeChild;
            var xpub = await _wallet.GetWalletXpubAsync(walletIdentity);
            
            // receive addresses
            for (int i = 0; i < nextRecChild; i++)
            {
                var address = (await _btc.DeriveAddress(xpub, false, i)).ToString();
                var created = await _btc.FindAddressFirstSeenDate(address);
                
                if (!(currentAddressValues.Contains(address)))
                {
                    var newAddress = new AddressCreate
                    {
                        WalletId = walletIdentity.WalletId,
                        WalletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName,
                        PublicAddress = address,
                        IsChange = false,
                        Created = created
                    };
                    await _addr.CreateAddressAsync(walletIdentity, newAddress);
                }
                else
                {
                    var id = currentAddresses.FirstOrDefault(a => a.PublicAddress == address).AddressId;
                    var addressIdentity = new AddressIdentity { AddressId = id, UserId = User.Identity.GetUserId() };
                    await _addr.UpdateAddressCreatedDate(addressIdentity, created);
                }
            }

            // change addresses
            for (int i = 0; i < nextChngChild; i++)
            {
                var address = (await _btc.DeriveAddress(xpub, true, i)).ToString();
                var created = await _btc.FindAddressFirstSeenDate(address);

                if (!(currentAddressValues.Contains(address)))
                {
                    var newAddress = new AddressCreate
                    {
                        WalletId = walletIdentity.WalletId,
                        WalletName = (await _wallet.GetWalletByIdAsync(walletIdentity)).WalletName,
                        PublicAddress = address,
                        IsChange = true,
                        Created = created
                    };
                    await _addr.CreateAddressAsync(walletIdentity, newAddress);
                }
                else
                {
                    var id = currentAddresses.FirstOrDefault(a => a.PublicAddress == address).AddressId;
                    var addressIdentity = new AddressIdentity { AddressId = id, UserId = User.Identity.GetUserId() };
                    await _addr.UpdateAddressCreatedDate(addressIdentity, created);
                }
            }
        }
        private async Task UpdatePastTransactions(WalletIdentity walletIdentity)
        {
            // compare transactions to ones already in the db
            // one transaction can be associated with multiple addresses, so keep transaction hash and balance, then update at the end
            // if already in db, update date of transaction from the network
            IEnumerable<TransactionListItem> currentTransactions = await _trans.GetWalletTransactionsAsync(walletIdentity);
            IEnumerable<string> currentTransactionHashs = currentTransactions.Select(t => t.TransactionHash).ToArray();

            var walletBTCInfo = await _wallet.GetWalletBTCInfoAsync(walletIdentity);
            var nextRecChild = walletBTCInfo.NextReceiveChild;
            var nextChngChild = walletBTCInfo.NextChangeChild;
            var xpub = await _wallet.GetWalletXpubAsync(walletIdentity);

            var allTransactions = await _btc.FindAllTransactions(xpub, nextRecChild, nextChngChild);
            if (allTransactions == null)
            {
                return;
            }
            foreach (var trans in allTransactions)
            {
                var clone = currentTransactions.FirstOrDefault(t => t.TransactionHash == trans.TransactionHash && t.WalletId == walletIdentity.WalletId);
                if (clone != null)
                {
                    var transactionIdentity = new TransactionIdentity
                    {
                        TransactionId = clone.TransactionId,
                        UserId = User.Identity.GetUserId()
                    };
                    await _trans.UpdateTransactionAsync(transactionIdentity, trans);
                }
                else
                {
                    var model = new TransactionCreate
                    {
                        TransactionHash = trans.TransactionHash,
                        TotalAmount = trans.Amount,
                        WalletId = walletIdentity.WalletId,
                        IsSend = trans.Amount < 0,
                        Created = trans.Created
                    };
                    await _trans.CreateTransactionAsync(walletIdentity, model);
                }
            }
        }





    }
}