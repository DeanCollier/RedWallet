using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Send
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey(nameof(Wallet))]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        [Required, Display(Name = "Transaction Hash")]
        [MinLength(1)]
        public string BTCTransaction { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
