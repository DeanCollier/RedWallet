using NBitcoin;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.WalletModels;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface IBitcoinService
    {
        string BuildTransaction(string encryptedSecret, string walletPassword, decimal sendAmount, string recipientAddress);
        Task<KeyDetail> CreateNewBitcoinKey(WalletCreate model);
        Task<decimal> FindBitcoinBalance(ExtPubKey xpub, int nextRecChild, int nextChngChild);
        Task<int> FindNextReceivingChildPosition(ExtPubKey xpub);
        Task<int> FindNextChangeChildPosition(ExtPubKey xpub);
        Task<BitcoinSecret> GetBitcoinSecret(string encryptedSecret, string passphrase);
        Task<BitcoinAddress> GetNewChangeAddress(string xpub);
        Task<BitcoinAddress> GetNewReceivingAddress(string xpub);
        Task<ExtPubKey> GetXpub(string xpub);
        Task<bool> IsBitcoinSecret(string encryptedSecret, string passphrase);
        Task<bool> IsValidAddress(string recipientAddress);
    }
}