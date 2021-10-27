using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.BitcoinModels
{
    public class KeyDetail
    {
        [Required]
        public string Passphrase { get; set; }

        [Required]
        public string MnemonicSeedPhrase { get; set; }

        [Required]
        public string EncryptedSecret{ get; set; }

    }
}
