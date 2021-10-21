using NBitcoin;
using RedWallet.Models.WalletModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class Bitcoin
    {
        // bitcoin core
        // network credentials
        public static Network network = Network.TestNet;
        public static readonly string rpcHost = "hntaqvaxkxcoskeqkv32rr2fwo6ok5lg4n25x3wjaweqiepwq6jlu4id.onion";
        public static readonly string rpcCredentials = "umbrel:Y0hy667C1ZhJtmZlwid4mt1Fz55LQDylrYQDAnmrVes=";
        public static readonly string walletPassPhrase = "lightningbbobb";
        public static readonly int walletUnlockTime = 20;

        public async Task<WalletDetail> CreateWallet()
        {
            try
            {
                // connect to client
                RPCClient rpcClient = new RPCClient(rpcCredentials, rpcHost, network);
                //
                await rpcClient.WalletPassphraseAsync(walletPassPhrase, walletUnlockTime);
                string walletAddress = rpcClient.GetNewAddress().ToString();
                BitcoinAddress btcWallet = BitcoinAddress.Create(walletAddress, network);
                var privateKey = rpcClient.DumpPrivKey(btcWallet);

                var newWallet = new WalletDetail()
                {
                    Address = walletAddress.ToString(),
                    PrivateKey = privateKey.ToString()
                };

                return newWallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
