using RedWallet.Data;
using RedWallet.Models.BitcoinModels;
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
            var created = transaction.Created == null ? DateTimeOffset.Now : transaction.Created;
            var entity = new Transaction
            {
                TransactionHash = transaction.TransactionHash,
                WalletId = model.WalletId,
                IsSend = transaction.IsSend,
                TotalAmount = transaction.TotalAmount,
                Created = created.GetValueOrDefault()
            };

            var transactionIdentity = new TransactionIdentity();

            using (var context = new ApplicationDbContext())
            {
                var clone = await context
                    .Transactions
                    .SingleOrDefaultAsync
                    (t => t.Wallet.UserId == model.UserId && t.TransactionHash == transaction.TransactionHash && t.WalletId == transaction.WalletId);

                if (clone == null)
                {
                    context.Transactions.Add(entity);
                    await context.SaveChangesAsync();

                    transactionIdentity.TransactionId = entity.Id;
                    transactionIdentity.UserId = model.UserId;
                }
                else
                {
                    transactionIdentity.TransactionId = clone.Id;
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
                        TransactionId = t.Id,
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
                    .SingleOrDefaultAsync(t => t.Wallet.UserId == model.UserId && t.Id == model.TransactionId);

                var detail = new TransactionDetail
                {
                    TransactionId = entity.Id,
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
        public async Task<bool> UpdateTransactionAsync(TransactionIdentity model, OperationDetail detail)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Transactions
                    .SingleOrDefaultAsync(t => t.Wallet.UserId == model.UserId && t.Id == model.TransactionId);

                entity.TotalAmount = detail.Amount;
                entity.Created = detail.Created;
                return await context.SaveChangesAsync() == 1;
            }
        }

        // DELETE
        public async Task<bool> DeleteTransactionAsync(TransactionIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Transactions
                    .SingleOrDefaultAsync(t => t.Wallet.UserId == model.UserId && t.Id == model.TransactionId);

                context.Transactions.Remove(entity);
                return await context.SaveChangesAsync() == 1;
            }
        }
    }
}
