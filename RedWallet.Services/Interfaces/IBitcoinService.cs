using NBitcoin;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.WalletModels;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface IBitcoinService
    {
        /*Network Network { get; set; }
        string RPCHost { get; set; }
        string RPCCredentials { get; set; }*/

        string BuildTransaction(string encryptedSecret, string walletPassword, double sendAmount, string recipientAddress);
        BitcoinSecret GetBitcoinSecret(string encryptedSecret, string passphrase);
        BitcoinAddress GetNewBitcoinAddress(string encryptedSecret, string passphrase, string xpub, int xpubIteration);
        Task<KeyDetail> GetNewBitcoinKey(WalletCreate model);
        bool IsValidWallet(string recipientAddress);
    }
}