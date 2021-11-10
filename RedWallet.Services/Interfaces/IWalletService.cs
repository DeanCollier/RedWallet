using RedWallet.Models.BitcoinModels;
using RedWallet.Models.WalletModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface IWalletService
    {
        Task<string[]> CreateWalletAsync(WalletCreate model, KeyDetail keyDetail);
        Task<bool> DeleteWalletAsync(WalletIdentity model);
        Task<WalletDetail> GetWalletByIdAsync(WalletIdentity model);
        Task<string> GetWalletEncryptedSecretAsync(WalletIdentity model);
        Task<IEnumerable<WalletListItem>> GetWalletsAsync(string userId);
        Task<bool> UpdateWalletById(WalletEdit model);
    }
}