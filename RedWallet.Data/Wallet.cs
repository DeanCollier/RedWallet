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
        public Guid Id { get; set; }
        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string WalletName { get; set; }
        [Required]
        public string PassphraseHash { get; set; }
        [Required]
        public string PrivateKey { get; set; }

    }
}
