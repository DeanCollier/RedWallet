using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.TransactionModels
{
    public class TransactionIdentity
    {
        [Required]
        public int TransactionId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
