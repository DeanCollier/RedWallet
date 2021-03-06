using NBitcoin;
using RedWallet.Data;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services.Interfaces;
using RedWallet.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class WalletService : IWalletService
    {
        // CREATE 
        public async Task<string[]> CreateWalletAsync(WalletCreate model, KeyDetail keyDetail)
        {
            var entity = new Wallet
            {
                UserId = model.UserId,
                WalletName = model.WalletName,
                LatestBalance = 0m,
                NextReceiveChild = 0,
                NextChangeChild = 0,
                EncryptedSecret = keyDetail.EncryptedSecret,
                Xpub = keyDetail.Xpub,
            };

            using (var context = new ApplicationDbContext())
            {
                context.Wallets.Add(entity);
                await context.SaveChangesAsync();
            }

            return new string[] { keyDetail.Passphrase, keyDetail.MnemonicSeedPhrase, entity.Id.ToString() };
        }

        // READ
        public async Task<IEnumerable<WalletListItem>> GetWalletsAsync(string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Wallets
                    .Where(w => w.UserId == userId)
                    .Select(w => new WalletListItem
                    {
                        WalletId = w.Id,
                        WalletName = w.WalletName
                    });

                return await query.ToArrayAsync();
            }
        }

        // READ
        public async Task<WalletDetail> GetWalletByIdAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                var detail = new WalletDetail
                {
                    WalletId = entity.Id,
                    WalletName = entity.WalletName,
                    LastestBalance = entity.LatestBalance,
                    Xpub = entity.Xpub,
                };

                return detail;
            }
        }
        public async Task<WalletBTCInfo> GetWalletBTCInfoAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                var info = new WalletBTCInfo
                {
                    UserId = model.UserId,
                    WalletId = entity.Id,
                    LatestBalance = entity.LatestBalance,
                    NextReceiveChild = entity.NextReceiveChild,
                    NextChangeChild = entity.NextChangeChild
                };

                return info;
            }
        }

        // READ
        public async Task<string> GetWalletEncryptedSecretAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                return entity.EncryptedSecret;
            }
        }
        // READ
        public async Task<string> GetWalletXpubAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                return entity.Xpub;
            }
        }

        // UPDATE
        public async Task<bool> UpdateWalletByIdAsync(WalletEdit model)
        {
            if (model.NewWalletName == null)
            {
                return true; // nothing to update, confirm to controller that everything is fine
            }
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                entity.WalletName = model.NewWalletName;
                return await context.SaveChangesAsync() == 1;
            }
        }

        // UPDATE
        public async Task UpdateWalletBTCInfo(WalletBTCInfo model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                entity.LatestBalance = model.LatestBalance;
                entity.NextReceiveChild = model.NextReceiveChild;
                entity.NextChangeChild = model.NextChangeChild;

                await context.SaveChangesAsync();
            }
        }

        // DELETE
        public async Task<bool> DeleteWalletAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Wallets
                    .SingleOrDefaultAsync(w => w.UserId == model.UserId && w.Id == model.WalletId);

                context.Wallets.Remove(entity);
                return await context.SaveChangesAsync() >= 1; // other db sends and requests may also be deleted
            }
        }
    }
}
