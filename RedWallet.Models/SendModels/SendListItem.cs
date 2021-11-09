using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.SendModels
{
    public class SendListItem
    {
        [Required]
        public int SendId { get; set; }

        [Required]
        public string WalletName { get; set; }

        [Required]
        public string TransactionHash { get; set; }
    }
}
