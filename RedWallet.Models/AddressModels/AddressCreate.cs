using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.AddressModels
{
    public class AddressCreate
    {
        [Required, Display(Name = "Send To This Bitcoin Address")]
        public string PublicAddress { get; set; }

        [Required]
        public bool IsChange { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [Display(Name = "Wallet Name")]
        public string WalletName { get; set; }

        public DateTimeOffset? Created { get; set; }
    }
}
