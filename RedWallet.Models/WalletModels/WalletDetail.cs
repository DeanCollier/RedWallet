using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedWallet.Models.WalletModels
{
    public class WalletDetail
    {
        public PubKey PublicKey { get; set; }
        public Key PrivateKey { get; set; }
    }
}