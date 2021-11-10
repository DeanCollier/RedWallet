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
        [MinLength(1), MaxLength(100)]
        public string WalletName { get; set; }

        [Required]
        [MinLength(8), DataType(DataType.Password)]
        public string Passphrase { get; set; }

        [Required]
        [MinLength(8), Display(Name = "Entropy Input", Description = "Enter random letters for enhanced security.")]
        public string EntropyInput { get; set; }

        public string UserId { get; set; }

    }
}
