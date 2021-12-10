using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.BitcoinModels
{
    public class OperationDetail
    {
        [Required]
        public string TransactionHash { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }
    }
}
