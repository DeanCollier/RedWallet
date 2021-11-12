using RedWallet.Models.SendModels;
using RedWallet.Models.WalletModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface ISendService
    {
        Task<SendDetail> CreateSendAsync(WalletIdentity model, string transactionHash);
        Task<bool> DeleteSendAsync(SendIdentity model);
        Task<SendDetail> GetWalletSendByIdAsync(SendIdentity model);
        Task<IEnumerable<SendListItem>> GetWalletSendsAsync(WalletIdentity model);
    }
}