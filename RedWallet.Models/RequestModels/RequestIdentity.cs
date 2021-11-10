using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.RequestModels
{
    public class RequestIdentity
    {
        [Required]
        public int RequestId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
