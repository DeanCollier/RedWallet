using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.SendModels
{
    public class SendCreate
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        public string TransactionHash { get; set; }
    }
}
