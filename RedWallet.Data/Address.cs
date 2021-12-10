using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Address")]
        [MinLength(1)]
        public string PublicAddress { get; set; }

        [Required, ForeignKey(nameof(Wallet))]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        [Required]
        public bool IsChange { get; set; }

        [Required]
        public decimal LatestBalance { get; set; }

        public DateTimeOffset Created { get; set; }
        
    }
}
