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
        public async Task<bool> CreateWalletAsync(WalletCreate model)
        {
            var seedMnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
            var privateKey = seedMnemonic.DeriveExtKey(model.Passphrase);
            var bitcoinPrivateKey = privateKey.GetWif(Network.RegTest);
            //var bitcoinEncryptedPrivateKey = bitcoinPrivateKey.Encrypt(model.Passphrase);

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
                return await context.SaveChangesAsync() == 1;
            }
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
                        WalletName = w.WalletName
                    });

                return await query.ToArrayAsync();
            }
        }
    }
}
