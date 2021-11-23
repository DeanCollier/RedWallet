using RedWallet.Models.RequestModels;
using RedWallet.Models.SendModels;
using RedWallet.Models.WalletModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.DashboardModels
{
    public class DashboardViewModel
    {
        [Required]
        public int SelectedWalletId { get; set; }
        [Required]
        public decimal WalletBalance { get; set; }
        [Required]
        public IList<WalletListItem> UserWallets { get; set; }
        [Required]
        public IList<SendListItem> WalletSends { get; set; }
        [Required]
        public IList<RequestListItem> WalletAddresses { get; set; }

    }
}
