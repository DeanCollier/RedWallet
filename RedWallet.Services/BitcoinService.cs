using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.RPC;
using QBitNinja.Client;
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
        private QBitNinjaClient Client { get; set; }
        private string RPCHost { get; set; }
        private string RPCCredentials { get; set; }

        public BitcoinService()
        {
            Network = Network.Main;
            Client = new QBitNinjaClient("http://api.qbit.ninja/", Network);
            RPCHost = "127.0.0.1:18444";
            RPCCredentials = "lightningbbobb:ViresEnNumeris";
        }

        // create
        public async Task<KeyDetail> CreateNewBitcoinKey(WalletCreate model)
        {
            var entropy = RedWalletUtil.Generate256BitsOfRandomEntropy();
            RandomUtils.AddEntropy(entropy.ToSHA256()); // adding random entropy
            var seedMnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour); // random 24 work mnemonic
            var extendedKey = seedMnemonic.DeriveExtKey(); // derive extended key from mnemonic
            var bitcoinMasterSecret = extendedKey.PrivateKey.GetWif(Network); // get WIF, base58

            var encryptedSecret = bitcoinMasterSecret.Encrypt(model.Passphrase.ToSHA256()); // encrypt with passphrase hash

            return new KeyDetail
            {
                Passphrase = model.Passphrase,
                MnemonicSeedPhrase = seedMnemonic.ToString(),
                EncryptedSecret = encryptedSecret.ToString(),
                Xpub = extendedKey.Neuter().ToString(Network)
            };
        }

        // get new receive and change addresses
        public async Task<BitcoinAddress> GetNewReceivingAddress(string xpub)
        {
            var extPubKey = await GetXpub(xpub);
            var position = await FindNextReceivingChildPosition(extPubKey);
            var newAddress = extPubKey.Derive(0).Derive((uint)position).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);

            return newAddress;
        }
        public async Task<BitcoinAddress> GetNewChangeAddress(string xpub)
        {
            var extPubKey = await GetXpub(xpub);
            var position = await FindNextChangeChildPosition(extPubKey);

            var newAddress = extPubKey.Derive(1).Derive((uint)position).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);

            return newAddress;
        }

        // finders QbitNinja
        public async Task<decimal> FindBitcoinBalance(ExtPubKey xpub)
        {
            var receivingAddresses = new List<BitcoinAddress>();
            var changeAddresses = new List<BitcoinAddress>();
            int recPosition = await FindNextReceivingChildPosition(xpub);
            int chngPosition = await FindNextChangeChildPosition(xpub);

            if (recPosition + chngPosition == 0) // first addresses are empty, no UTXO's
            {
                return 0m;
            }

            for (int i = 0; i < recPosition; i++)
            {
                receivingAddresses.Add(xpub.Derive(0).Derive((uint)i).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network));
            }
            for (int i = 0; i < chngPosition; i++)
            {
                changeAddresses.Add(xpub.Derive(1).Derive((uint)i).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network));
            }

            var allAddresses = new List<BitcoinAddress>();
            allAddresses.AddRange(receivingAddresses);
            allAddresses.AddRange(changeAddresses);

            decimal totalBalance = 0;

            foreach (var address in allAddresses)
            {
                var balanceModel = await Client.GetBalance(address, true);
                if (balanceModel.Operations.Count > 0)
                {
                    var unspentCoins = new List<Coin>();
                    foreach (var operation in balanceModel.Operations)
                    {
                        unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
                    }
                    totalBalance += unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
                }
            }
            return totalBalance;
        }
        private async Task<int> FindNextReceivingChildPosition(ExtPubKey xpub)
        {
            int i = 0;
            int max = (int)(Math.Pow(2, 31) - 1);
            while (i < max)
            {
                var address = xpub.Derive(0).Derive((uint)i).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);
                var balanceModel = await Client.GetBalance(address);
                if (balanceModel.Operations.Count > 0)
                {
                    i++;
                }
                else
                {
                    return i;
                }
            }
            return 0;
        }
        private async Task<int> FindNextChangeChildPosition(ExtPubKey xpub)
        {
            int i = 0;
            int max = (int)(Math.Pow(2, 31) - 1);

            while (i < max)
            {
                var address = xpub.Derive(1).Derive((uint)i).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);
                var balanceModel = await Client.GetBalance(address);
                if (balanceModel.Operations.Count > 0)
                {
                    i++;
                }
                else
                {
                    return i;
                }
            }
            return 0;
        }

        // checkers
        public async Task<bool> IsBitcoinSecret(string encryptedSecret, string passphrase)
        {
            try
            {
                BitcoinEncryptedSecret.Create(encryptedSecret, Network).GetSecret(passphrase.ToSHA256());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> IsValidAddress(string recipientAddress)
        {
            try
            {
                BitcoinAddress.Create(recipientAddress, Network);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // get encrypted info after checkers
        public async Task<BitcoinSecret> GetBitcoinSecret(string encryptedSecret, string passphrase)
        {
            return BitcoinEncryptedSecret.Create(encryptedSecret, Network).GetSecret(passphrase.ToSHA256());
        }
        public async Task<ExtPubKey> GetXpub(string Xpub)
        {
            return ExtPubKey.Parse(Xpub, Network);
        }















        public string BuildTransaction(string encryptedSecret, string walletPassword, decimal sendAmount, string recipientAddress)
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
