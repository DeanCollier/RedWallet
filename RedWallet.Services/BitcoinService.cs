using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.RPC;
using RedWallet.Models.BitcoinModels;
using RedWallet.Models.WalletModels;
using RedWallet.Services.Interfaces;
using RedWallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedWallet.Services
{
    public class BitcoinService : IBitcoinService
    {
        private Network Network { get; set; }
        private string RPCHost { get; set; }
        private string RPCCredentials { get; set; }

        public BitcoinService()
        {
            Network = Network.Main;
            RPCHost = "127.0.0.1:18444";
            RPCCredentials = "lightningbbobb:ViresEnNumeris";
        }

        public async Task<KeyDetail> GetNewBitcoinKey(WalletCreate model)
        {
            RandomUtils.AddEntropy(model.EntropyInput); // adding random entropy
            var seedMnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour); // random 24 work mnemonic
            var extendedKey = seedMnemonic.DeriveExtKey(); // derive extended key from mnemonic
            var bitcoinSecret = extendedKey.PrivateKey.GetWif(Network); // get WIF, base58

            var encryptedSecret = bitcoinSecret.Encrypt(model.Passphrase.ToSHA256()); // encrypt with passphrase hash
            //bitcoinSecret.PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);

            return new KeyDetail
            {
                Passphrase = model.Passphrase,
                MnemonicSeedPhrase = seedMnemonic.ToString(),
                EncryptedSecret = encryptedSecret.ToString(),
                Xpub = extendedKey.Neuter().ToString(Network),
                XpubIteration = 0
            };
        }

        // enter password and check
        public bool CheckEcryptionPassword(string encryptedSecret, string passphrase)
        {
            if (GetBitcoinSecret(encryptedSecret, passphrase).ToString() != null)
            {
                return true;
            }
            return false;
        }

        public BitcoinAddress GetNewBitcoinAddress(string encryptedSecret, string passphrase, string xpub, int xpubIteration)
        {
            if (CheckEcryptionPassword(encryptedSecret, passphrase)) // verify password
            {
                var extPubKey = ExtPubKey.Parse(xpub, Network);
                var newAddress = extPubKey.Derive(0).Derive((uint)xpubIteration).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH ,Network);

                return newAddress;
            }
            else
            {
                return null;
            }

        }

        public BitcoinSecret GetBitcoinSecret(string encryptedSecret, string passphrase)
        {
            return BitcoinEncryptedSecret.Create(encryptedSecret, Network).GetSecret(passphrase.ToSHA256());
        }

        public bool IsValidWallet(string recipientAddress)
        {
            return true;
        }

        public string GetBitcoinBalance()
        {
            double balance = 100.00000000000000000d;
            return Math.Round(balance, 8).ToString("0.00000000");
        }

        public string BuildTransaction(string encryptedSecret, string walletPassword, double sendAmount, string recipientAddress)
        {
            string transaction = "ThisIsAFakeTransactionString";
            var transactionHash = transaction.ToSHA256();
            return transactionHash;
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
