using RedWallet.Models.WalletModels;
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
        public Guid Id { get; set; }

        [Required, ForeignKey(nameof(Wallet))]
        public Guid WalletId { get; set; }
        public virtual WalletDetail Wallet { get; set; }

        [Required]
        [MinLength(1)]
        public string BTCTransaction { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
