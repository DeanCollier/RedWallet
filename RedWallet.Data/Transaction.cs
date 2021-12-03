using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Transaction
    {
        [Key]
        [Required, Display(Name = "Transaction")]
        [MinLength(1)]
        public string TransactionHash { get; set; }

        [Required, ForeignKey(nameof(Wallet))]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        [Required]
        public bool IsSend { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
