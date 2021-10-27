using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.WalletModels
{
    public class WalletListItem
    {
        [Required]
        public int WalletId { get; set; }

        [Required, Display(Name = "Wallet Name")]
        public string WalletName { get; set; }
    }
}
