using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.RPC;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.WalletModels;
using RedWallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public async Task<KeyDetail> GetNewBitcoinKey(WalletCreate model)
        {
            RandomUtils.AddEntropy(model.EntropyInput); // adding random entropy
            var seedMnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour); // random 24 work mnemonic
            var extendedKey = seedMnemonic.DeriveExtKey(); // derive extended key from mnemonic
            var bitcoinSecret= extendedKey.PrivateKey.GetWif(Network); // get WIF, base58

            var testString = bitcoinSecret.ToString();
            
            var encryptedSecret = bitcoinSecret.Encrypt(model.Passphrase.ToSHA256()); // encrypt with passphrase hash
            //bitcoinSecret.PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);

            return new KeyDetail
            {
                Passphrase = model.Passphrase,
                MnemonicSeedPhrase = seedMnemonic.ToString(),
                EncryptedSecret = encryptedSecret.ToString()
            };
        }

        public BitcoinAddress GetNewBitcoinAddress(string encryptedSecret, string passphrase)
        {
            try
            {
                var secret = GetBitcoinSecret(encryptedSecret, passphrase);
                var address = secret.PubKey.Hash.GetAddress(Network);

                return address;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid Password");
            }

        }
        public BitcoinSecret GetBitcoinSecret(string encryptedSecret, string passphrase)
        {
            return BitcoinEncryptedSecret.Create(encryptedSecret, Network).GetSecret(passphrase.ToSHA256());
        }

        

        

        

        /*public async Task<WalletDetail> CreateAddress(WalletCreate model)
        {
            var key = new ExtKey();
            return new WalletDetail
            {
                PublicKey = key.PubKey,
                PrivateKey = key
            };


            // connect to client
            RPCClient rpcClient = new RPCClient(RPCCredentials, RPCHost, Network);
            // create wallet with passphrase
            await rpcClient.CreateWalletAsync(model.WalletName, new CreateWalletOptions { Passphrase = model.Passphrase });
            //await rpcClient.LoadWalletAsync(model.WalletName);
            // unlock for 'walletUnlockTime' seconds
            //rpcClient.GetWallet(model.WalletName);
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


            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }*/
    }
}
