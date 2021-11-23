using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.WalletModels
{
    public class WalletCreate
    {
        [Required]
        [Display(Name = "Create Wallet Name")]

        [MinLength(1), MaxLength(100)]
        public string WalletName { get; set; }

        [Required]
        [Display(Name = "Create Wallet Passphrase", Description = "*This passphrase is specific to this wallet and should be different from your user password*")]
        [MinLength(8), DataType(DataType.Password)]
        public string Passphrase { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Passphrase")]
        [Compare("Passphrase", ErrorMessage = "The passphrase and confirmation passphrase do not match.")]
        public string ConfirmPassphrase { get; set; }

        public string UserId { get; set; }

    }
}
