using RedWallet.Data;
using RedWallet.Models.SendModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class SendService
    {
        private readonly Guid _userId;

        public SendService(Guid userId)
        {
            _userId = userId;
        }

        // CREATE
        public async Task<SendDetail> CreateSendAsync(int walletId, string transactionHash)
        {
            var entity = new Send
            {
                TransactionHash = transactionHash,
                WalletId = walletId,
                Created = DateTimeOffset.Now
            };

            using (var context = new ApplicationDbContext())
            {
                context.Sends.Add(entity);
                await context.SaveChangesAsync();
            }

            return await GetWalletSendByIdAsync(entity.Id);
        }

        // READ
        public async Task<IEnumerable<SendListItem>> GetWalletSendsAsync(int walletId)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Sends
                    .Where(s => s.Wallet.UserId == _userId.ToString() && s.WalletId == walletId)
                    .Select(s => new SendListItem
                    {
                        SendId = s.Id,
                        WalletName = s.Wallet.WalletName,
                        TransactionHash = s.TransactionHash
                    });

                return await query.ToArrayAsync();
            }
        }

        // READ
        public async Task<SendDetail> GetWalletSendByIdAsync(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Sends
                    .SingleAsync(s => s.Wallet.UserId == _userId.ToString() && s.Id == id);

                var model = new SendDetail
                {
                    SendId = entity.Id,
                    WalletId = entity.WalletId,
                    WalletName = entity.Wallet.WalletName,
                    TransactionHash = entity.TransactionHash,
                    Created = entity.Created
                };

                return model;
            }
        }

        // UPDATE -- NONE

        // DELETE -- only for DB purposes, don't need
        public async Task<bool> DeleteSendAsync(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Sends
                    .SingleAsync(s => s.Wallet.UserId == _userId.ToString() && s.Id == id);

                context.Sends.Remove(entity);
                return await context.SaveChangesAsync() == 1;
            }
        }
    }
}
