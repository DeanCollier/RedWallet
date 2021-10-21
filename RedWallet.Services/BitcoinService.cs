using NBitcoin;
using NBitcoin.RPC;
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
        private readonly Guid? _userId;
        public Network Network { get; set; }
        public string RPCHost { get; set; }
        public string RPCCredentials { get; set; }

        public Bitcoin(Network network, string rpcHost, string rpcCredentials, Guid? userId)
        {
            Network = network;
            RPCHost = rpcHost;
            RPCCredentials = rpcCredentials;
            _userId = userId;
        }


        // bitcoin core regtest
        // network credentials
        public static Network network = Network.RegTest;
        public static readonly string rpcHost = "127.0.0.1:18444";
        public static readonly string rpcCredentials = "user:password";
        //public static readonly string walletPassPhrase = "lightningbbobb";
        //public static readonly int walletUnlockTime = 600;

        public async Task<WalletDetail> CreateWallet()
        {
            try
            {
                // connect to client
                RPCClient rpcClient = new RPCClient(rpcCredentials, rpcHost, network);
                //
                rpcClient.CreateWallet("new_wallet", new CreateWalletOptions { Passphrase = "newnew"});
                //await rpcClient.WalletPassphraseAsync(walletPassPhrase, walletUnlockTime);
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
