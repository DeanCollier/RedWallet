using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required, ForeignKey(nameof(Wallet))]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        [Required, Display(Name = "Send To Address")]
        [MinLength(1)]
        public string SegwitP2SHAddress { get; set; }

        public DateTimeOffset Created { get; set; }
        
    }
}
