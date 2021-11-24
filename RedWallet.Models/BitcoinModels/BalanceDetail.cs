using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.BitcoinModels
{
    public class BalanceDetail
    {
        [Required]
        public decimal WalletBalance { get; set; }
    }
}
