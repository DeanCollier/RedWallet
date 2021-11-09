using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.BitcoinModels
{
    public class TransactionCreate
    {
        [Required]
        public int WalletId { get; set; }
        
        [Required]
        public double SendAmount { get; set; }
        
        [Required]
        public string RecipientAddress { get; set; }
    }
}
