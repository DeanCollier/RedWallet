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
        //private string RPCHost { get; set; }
        //private string RPCCredentials { get; set; }

        public BitcoinService()
        {
            Network = Network.Main;
            Client = new QBitNinjaClient("http://api.qbit.ninja/", Network);
            //RPCHost = "127.0.0.1:18444";
            //RPCCredentials = "lightningbbobb:ViresEnNumeris";
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
        public async Task<BitcoinAddress> DeriveAddress(string xpub, bool isChange, int position)
        {
            var extPubKey = await GetXpub(xpub);
            int change = isChange ? 1 : 0;
            return extPubKey.Derive((uint)change).Derive((uint)position).PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network);
        }

        // get new receive and change addresses
        public async Task<BitcoinAddress> GetNewReceivingAddress(string xpub)
        {
            var position = await FindNextReceivingChildPosition(xpub);
            var newAddress = await DeriveAddress(xpub, false, position);

            return newAddress;
        }
        public async Task<BitcoinAddress> GetNewChangeAddress(string xpub)
        {
            var position = await FindNextReceivingChildPosition(xpub);
            var newAddress = await DeriveAddress(xpub, true, position);

            return newAddress;
        }

        // finders QbitNinja
        public async Task<decimal> FindBitcoinBalance(string xpub, int nextRecChild, int nextChngChild)
        {
            var receivingAddresses = new List<BitcoinAddress>();
            var changeAddresses = new List<BitcoinAddress>();

            decimal totalBalance = 0m;

            if (nextRecChild + nextChngChild == 0) // first addresses are empty, no UTXO's
            {
                return totalBalance;
            }

            for (int i = 0; i < nextRecChild; i++)
            {
                receivingAddresses.Add(await DeriveAddress(xpub, false, i));
            }
            for (int i = 0; i < nextChngChild; i++)
            {
                changeAddresses.Add(await DeriveAddress(xpub, true, i));
            }

            var allAddresses = new List<BitcoinAddress>();
            allAddresses.AddRange(receivingAddresses);
            allAddresses.AddRange(changeAddresses);

            foreach (var address in allAddresses)
            {
                var balanceModel = await Client.GetBalance(address, true);
                var unspentCoins = new List<Coin>();
                foreach (var operation in balanceModel.Operations)
                {
                    unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
                }
                totalBalance += unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            }
            return totalBalance;
        }
        public async Task<IEnumerable<OperationDetail>> FindAllTransactions(string xpub, int nextRecChild, int nextChngChild)
        {
            var receivingAddresses = new List<BitcoinAddress>();
            var changeAddresses = new List<BitcoinAddress>();

            if (nextRecChild + nextChngChild == 0) // first addresses are empty, no UTXO's
            {
                return null;
            }

            for (int i = 0; i < nextRecChild; i++)
            {
                receivingAddresses.Add(await DeriveAddress(xpub, false, i));
            }
            for (int i = 0; i < nextChngChild; i++)
            {
                changeAddresses.Add(await DeriveAddress(xpub, true, i));
            }

            var allAddresses = new List<BitcoinAddress>();
            allAddresses.AddRange(receivingAddresses);
            allAddresses.AddRange(changeAddresses);

            var allTransactions = new List<OperationDetail>();

            foreach (var address in allAddresses)
            {
                var balanceModel = await Client.GetBalance(address, false);
                var unspentCoins = new List<Coin>();
                foreach (var operation in balanceModel.Operations)
                {
                    // check if transaction is already in operation detail list
                    var clone = allTransactions.FirstOrDefault(t => t.TransactionHash == operation.TransactionId.ToString());
                    if (clone != null)
                    {
                        clone.Amount += operation.Amount.ToDecimal(MoneyUnit.BTC);
                    }
                    else
                    {
                        var detail = new OperationDetail
                        {
                            TransactionHash = operation.TransactionId.ToString(),
                            Amount = operation.Amount.ToDecimal(MoneyUnit.BTC),
                            Created = operation.FirstSeen
                        };
                        allTransactions.Add(detail);
                    } 
                }
            }
            return allTransactions;
        }

        public async Task<int> FindNextReceivingChildPosition(string xpub)
        {
            bool isChange = false;
            int i = 0;
            int max = (int)(Math.Pow(2, 31) - 1);

            while (i < max)
            {
                var address = await DeriveAddress(xpub, isChange, i);
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
        public async Task<int> FindNextChangeChildPosition(string xpub)
        {
            bool isChange = true;
            int i = 0;
            int max = (int)(Math.Pow(2, 31) - 1);

            while (i < max)
            {
                var address = await DeriveAddress(xpub, isChange, i);
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
        public async Task<DateTimeOffset> FindAddressFirstSeenDate(string address)
        {
            if (await IsValidAddress(address))
            {
                var btcAddress = BitcoinAddress.Create(address, Network);
                var balanceModel = await Client.GetBalance(btcAddress, false);
                var firstSeen = balanceModel.Operations.Last().FirstSeen;
                return firstSeen;
            }
            return DateTimeOffset.Now;

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
        public async Task<bool> IsValidAddress(string address)
        {
            try
            {
                BitcoinAddress.Create(address, Network);
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
