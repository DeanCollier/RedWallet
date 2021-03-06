using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.BitcoinModels
{
    public class TransactionSend
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        [Display(Name = "Wallet Name")]

        public string WalletName { get; set; }

        [Required]
        [Display(Name = "Current Wallet Balance")]
        public decimal WalletBalance { get; set; }

        [Required]
        [Display(Name = "Send Amount (in BTC)")]

        public decimal SendAmount { get; set; }
        
        [Required]
        [Display(Name = "Recipient Bitcoin Address")]

        public string RecipientAddress { get; set; }

        [Required]
        [Display(Name = "Wallet Passphrase")]
        [MinLength(8), DataType(DataType.Password)]
        public string WalletPassphrase { get; set; }
    }
}
