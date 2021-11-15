using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.RequestModels
{
    public class RequestCreate
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        [Display(Name = "Wallet Name")]

        public string WalletName { get; set; }

        [Required]
        [Display(Name = "Wallet Passphrase")]
        [MinLength(8), DataType(DataType.Password)]
        public string Passphrase { get; set; }
    }
}
