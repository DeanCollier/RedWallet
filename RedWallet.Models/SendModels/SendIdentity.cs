using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.SendModels
{
    public class SendIdentity
    {
        [Required]
        public int SendId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
