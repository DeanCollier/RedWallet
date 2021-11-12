using RedWallet.Data;
using RedWallet.Models.SendModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class SendService : ISendService
    {
        // CREATE
        public async Task<SendDetail> CreateSendAsync(WalletIdentity model, string transactionHash)
        {
            var entity = new Send
            {
                TransactionHash = transactionHash,
                WalletId = model.WalletId,
                Created = DateTimeOffset.Now
            };

            using (var context = new ApplicationDbContext())
            {
                context.Sends.Add(entity);
                await context.SaveChangesAsync();
            }

            var sendIdentity = new SendIdentity { SendId = entity.Id, UserId = model.UserId };
            return await GetWalletSendByIdAsync(sendIdentity);
        }

        // READ
        public async Task<IEnumerable<SendListItem>> GetWalletSendsAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Sends
                    .OrderByDescending(s => s.Created)
                    .Where(s => s.Wallet.UserId == model.UserId && s.WalletId == model.WalletId)
                    .Select(s => new SendListItem
                    {
                        SendId = s.Id,
                        WalletName = s.Wallet.WalletName,
                        TransactionHash = s.TransactionHash,
                        Created = s.Created
                        
                    });

                return await query.ToArrayAsync();
            }
        }

        // READ
        public async Task<SendDetail> GetWalletSendByIdAsync(SendIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Sends
                    .SingleAsync(s => s.Wallet.UserId == model.UserId && s.Id == model.SendId);

                var detail = new SendDetail
                {
                    SendId = entity.Id,
                    WalletId = entity.WalletId,
                    WalletName = entity.Wallet.WalletName,
                    TransactionHash = entity.TransactionHash,
                    Created = entity.Created
                };

                return detail;
            }
        }

        // UPDATE -- not needed, there are no properties to change - NICK

        // DELETE -- only for DB purposes, don't need
        public async Task<bool> DeleteSendAsync(SendIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Sends
                    .SingleAsync(s => s.Wallet.UserId == model.UserId && s.Id == model.SendId);

                context.Sends.Remove(entity);
                return await context.SaveChangesAsync() == 1;
            }
        }
    }
}
