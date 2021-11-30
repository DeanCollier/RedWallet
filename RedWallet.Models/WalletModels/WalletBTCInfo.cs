using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.WalletModels
{
    public class WalletBTCInfo
    {
        [Required]
        public int WalletId { get; set; }
        [Required]
        public string UserId { get; set; }

        [Required]
        public decimal LatestBalance { get; set; }

        [Required]
        public int NextReceiveChild { get; set; }

        [Required]
        public int NextChangeChild { get; set; }
    }
}
