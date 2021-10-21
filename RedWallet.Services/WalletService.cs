using RedWallet.Data;
using RedWallet.Models.WalletModels;
using RedWallet.Utilities;
using System;
using System.Collections.Generic;
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
            var entity = new Wallet
            {
                UserId = _userId,
                WalletName = model.WalletName,
                PassphraseHash = model.Passphrase.ToSHA256() // RedWalletUtil
            };

            using (var context = new ApplicationDbContext())
            {
                context.Wallets.Add(entity);
                return await context.SaveChangesAsync() == 1;
            }
        }
    }
}
