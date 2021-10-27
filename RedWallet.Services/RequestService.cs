using RedWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedWallet.Models.ReceiveModels;

namespace RedWallet.Services
{
    public class RequestService
    {
        private readonly Guid _userId;

        public RequestService(Guid userId)
        {
            _userId = userId;
        }

        // CREATE 
        public async Task<RequestDetail> CreateRequestAsync(RequestCreate model)
        {
            var entity = new Request
            {
                UserId = _userId.ToString(),
                WalletName = model.WalletName,
                EncryptedSecret = keyDetail.EncryptedSecret
            };

            using (var context = new ApplicationDbContext())
            {
                context.Wallets.Add(entity);
                await context.SaveChangesAsync();
            }

            return new string[] { keyDetail.Passphrase, keyDetail.MnemonicSeedPhrase, entity.Id.ToString() };
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
                        WalletId = w.Id,
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
                    WalletId = entity.Id,
                    WalletName = entity.WalletName,
                    PastPaymentRequests = new List<Request>(), // just empty for now
                    OutgoingPayments = new List<Send>()  // just empty for now
                };

                return model;
            }
        }

        // UPDATE
        public async Task<bool> UpdateWalletById(WalletEdit model)
        {
            if (model.NewWalletName == null)
            {
                return true; // nothing to update, confirm to controller that everything is fine
            }
            using (var context = new ApplicationDbContext())
            {
                var entity = context
                    .Wallets
                    .Single(w => w.UserId == _userId.ToString() && w.Id == model.WalletId);

                entity.WalletName = model.NewWalletName;
                return await context.SaveChangesAsync() == 1;
            }
        }

        // DELETE
        public async Task<bool> DeleteWalletAsync(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = context
                    .Wallets
                    .Single(w => w.UserId == _userId.ToString() && w.Id == id);

                context.Wallets.Remove(entity);
                return await context.SaveChangesAsync() >= 1; // other db sends and requests may also be deleted

            }
        }
    }
}
