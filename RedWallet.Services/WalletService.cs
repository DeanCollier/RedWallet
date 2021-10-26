using NBitcoin;
using RedWallet.Data;
using RedWallet.Models.WalletModels;
using RedWallet.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class WalletService
    {
        private readonly Guid _userId;


        public WalletService(Guid userId)
        {
            _userId = userId;
        }

        // CREATE 
        public async Task<string[]> CreateWalletAsync(WalletCreate model)
        {
            RandomUtils.AddEntropy(model.EntropyInput);
            var seedMnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
            var privateKey = seedMnemonic.DeriveExtKey(model.Passphrase);
            var bitcoinPrivateKey = privateKey.GetWif(Network.RegTest);

            var entity = new Wallet
            {
                UserId = _userId.ToString(),
                WalletName = model.WalletName,
                PassphraseHash = model.Passphrase.ToSHA256(), // RedWalletUtil
                PrivateKey = bitcoinPrivateKey
            };

            using (var context = new ApplicationDbContext())
            {
                context.Wallets.Add(entity);
                await context.SaveChangesAsync();
            }

            return new string[] { model.Passphrase, seedMnemonic.ToString(), entity.Id.ToString() };
        }

        // READ
        public async Task<IEnumerable<WalletListItem>> GetWalletsAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Wallets
                    .Where(w => w.UserId == _userId.ToString())
                    .Select(w => new WalletListItem
                    {
                        Id = w.Id,
                        WalletName = w.WalletName
                    });

                return await query.ToArrayAsync();
            }
        }
        
        // READ
        public async Task<WalletDetail> GetWalletByIdAsync(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = context
                    .Wallets
                    .Single(w => w.UserId == _userId.ToString() && w.Id == id);

                var model = new WalletDetail
                {
                    WalletName = entity.WalletName,
                    PastPaymentRequests = new List<Request>(), // just empty for now
                    OutgoingPayments = new List<Send>()  // just empty for now
                };

                return model;
            }
        }

        public async Task<bool> DeleteWalletAsync(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = context
                    .Wallets
                    .Single(w => w.UserId == _userId.ToString() && w.Id == id);

                var isDeleted = context.Wallets.Remove(entity);
                return await context.SaveChangesAsync() >= 1; // other db sends and requests may also be deleted

            }
        }
    }
}
