using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.TransactionModels
{
    public class TransactionListItem
    {
        [Required]
        [Display(Name = "Bitcoin Transaction")]
        public string TransactionHash { get; set; }

        [Required]
        public string WalletName { get; set; }
        
        [Required]
        public int WalletId { get; set; }

        [Required]
        public bool IsSend { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Sent Date")]
        public DateTimeOffset Created { get; set; }
    }
}
