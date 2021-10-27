using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.ReceiveModels
{
    public class RequestCreate
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        public string WalletName { get; set; }

        [Required]
        [MinLength(8), DataType(DataType.Password)]
        public string Passphrase { get; set; }
    }
}
