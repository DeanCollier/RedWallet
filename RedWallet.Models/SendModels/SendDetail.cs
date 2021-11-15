using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.SendModels
{
    public class SendDetail
    {
        [Required]
        public int SendId { get; set; }

        [Required]
        public int WalletId { get; set; }
        
        [Required]
        [Display(Name = "Wallet Name")]
        public string WalletName { get; set; }
        
        [Required]
        public DateTimeOffset Created { get; set; }

        [Required]
        [Display(Name = "Transaction")]

        public string TransactionHash { get; set; }
    }
}
