using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required, Display(Name = "Wallet Name")]
        public string WalletName { get; set; }

        [Required]
        public decimal LatestBalance { get; set; }

        [Required]
        public int NextReceiveChild { get; set; }

        [Required]
        public int NextChangeChild { get; set; }

        [Required]
        public string EncryptedSecret { get; set; }
        [Required]
        public string Xpub { get; set; }


    }
}
