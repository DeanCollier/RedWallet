using RedWallet.Data;
using RedWallet.Models.TransactionModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class TransactionService : ITransactionService
    {
        // CREATE
        public async Task<TransactionDetail> CreateTransactionAsync(WalletIdentity model, TransactionCreate transaction)
        {
            var entity = new Transaction
            {
                TransactionHash = transaction.TransactionHash,
                WalletId = model.WalletId,
                IsSend = transaction.IsSend,
                TotalAmount = transaction.TotalAmount,
                Created = DateTimeOffset.Now
            };

            var transactionIdentity = new TransactionIdentity();

            using (var context = new ApplicationDbContext())
            {
                var clone = await context
                    .Transactions
                    .SingleOrDefaultAsync(t => t.Wallet.UserId == model.UserId && t.TransactionHash == transaction.TransactionHash);

                if (clone == null)
                {
                    context.Transactions.Add(entity);
                    await context.SaveChangesAsync();

                    transactionIdentity.TransactionHash = entity.TransactionHash;
                    transactionIdentity.UserId = model.UserId;
                }
                else
                {
                    transactionIdentity.TransactionHash = clone.TransactionHash;
                    transactionIdentity.UserId = model.UserId;
                }
            }

            return await GetWalletTransactionByIdAsync(transactionIdentity);
        }

        // READ
        public async Task<IEnumerable<TransactionListItem>> GetWalletTransactionsAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Transactions
                    .OrderByDescending(t => t.Created)
                    .Where(t => t.Wallet.UserId == model.UserId && t.WalletId == model.WalletId)
                    .Select(t => new TransactionListItem
                    {
                        TransactionHash = t.TransactionHash,
                        WalletName = t.Wallet.WalletName,
                        WalletId = t.WalletId,
                        IsSend = t.IsSend,
                        TotalAmount = t.TotalAmount,
                        Created = t.Created
                    });

                return await query.ToArrayAsync();
            }
        }

        // READ
        public async Task<TransactionDetail> GetWalletTransactionByIdAsync(TransactionIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Transactions
                    .SingleOrDefaultAsync(t => t.Wallet.UserId == model.UserId && t.TransactionHash == model.TransactionHash);

                var detail = new TransactionDetail
                {
                    TransactionHash = entity.TransactionHash,
                    WalletId = entity.WalletId,
                    WalletName = entity.Wallet.WalletName,
                    IsSend = entity.IsSend,
                    TotalAmount = entity.TotalAmount,
                    Created = entity.Created
                };

                return detail;
            }
        }

        // UPDATE -- not needed yet?
        // possibly need in the future if transaction doesn't get confirmed

        // DELETE
        public async Task<bool> DeleteTransactionAsync(TransactionIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Transactions
                    .SingleOrDefaultAsync(t => t.Wallet.UserId == model.UserId && t.TransactionHash == model.TransactionHash);

                context.Transactions.Remove(entity);
                return await context.SaveChangesAsync() == 1;
            }
        }
    }
}
