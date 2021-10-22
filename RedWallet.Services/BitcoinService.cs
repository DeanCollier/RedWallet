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
    public class BitcoinService
    {
        private readonly Guid _userId;
        private readonly Network Network;
        private readonly string RPCHost;
        private readonly string RPCCredentials;

        public BitcoinService(Network network, string rpcHost, string rpcCredentials, Guid userId)
        {
            Network = network;
            RPCHost = rpcHost;
            RPCCredentials = rpcCredentials;
            _userId = userId;
        }

        public static readonly int walletUnlockTime = 20;

        public async Task<WalletDetail> CreateWallet(WalletCreate model)
        {
            try
            {
                // connect to client
                RPCClient rpcClient = new RPCClient(RPCCredentials, RPCHost, Network);
                // create wallet with passphrase
                await rpcClient.CreateWalletAsync(model.WalletName, new CreateWalletOptions { Passphrase = model.Passphrase});
                // unlock for 'walletUnlockTime' seconds
                await rpcClient.WalletPassphraseAsync(model.Passphrase, walletUnlockTime);
                // get public address
                string walletAddress = rpcClient.GetNewAddress().ToString();
                BitcoinAddress btcWallet = BitcoinAddress.Create(walletAddress, Network);
                // get private key for Wallet entity
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
