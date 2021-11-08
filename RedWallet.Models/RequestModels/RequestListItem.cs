using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.RequestModels
{
    public class RequestListItem
    {
        [Required]
        public string WalletName { get; set; }

        [Required, Display(Name = "Address")]
        public string RequestAddress { get; set; }
    }
}
