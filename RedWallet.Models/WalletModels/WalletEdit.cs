using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.WalletModels
{
    public class WalletEdit
    {
        [Required]
        public int WalletId { get; set; }

        [Required, Display(Name = "New Wallet Name")]
        public string NewWalletName { get; set; }

        public string UserId { get; set; }
    }
}