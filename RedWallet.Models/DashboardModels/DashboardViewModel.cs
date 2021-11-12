using RedWallet.Models.RequestModels;
using RedWallet.Models.SendModels;
using RedWallet.Models.WalletModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.DashboardModels
{
    public class DashboardViewModel
    {
        public IEnumerable<WalletListItem> UserWallets { get; set; }
        public IEnumerable<SendListItem> WalletSends { get; set; }
        public IEnumerable<RequestListItem> WalletAddresses { get; set; }

    }
}
