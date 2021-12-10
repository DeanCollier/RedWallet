using RedWallet.Models.BitcoinModels;
using RedWallet.Models.TransactionModels;
using RedWallet.Models.WalletModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDetail> CreateTransactionAsync(WalletIdentity model, TransactionCreate transaction);
        Task<bool> DeleteTransactionAsync(TransactionIdentity model);
        Task<TransactionDetail> GetWalletTransactionByIdAsync(TransactionIdentity model);
        Task<IEnumerable<TransactionListItem>> GetWalletTransactionsAsync(WalletIdentity model);
        Task<bool> UpdateTransactionAsync(TransactionIdentity model, OperationDetail detail);
    }
}