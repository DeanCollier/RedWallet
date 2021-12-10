using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.AddressModels
{
    public class AddressListItem
    {
        [Required]
        public int AddressId { get; set; }

        [Required, Display(Name = "Send To This Bitcoin Address")]
        public string PublicAddress { get; set; }

        [Required]
        [Display(Name = "Wallet Name")]

        public string WalletName { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [Display(Name = "Latest Balance")]

        public decimal LatestBalance { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }
    }
}

