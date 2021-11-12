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
        public int RequestId { get; set; }

        [Required]
        public string WalletName { get; set; }

        [Required, Display(Name = "Address")]
        public string RequestAddress { get; set; }
        
        [Required]
        [Display(Name = "Created Date")]
        public DateTimeOffset Created { get; set; }
    }
}

