using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.AddressModels
{
    public class AddressIdentity
    {
        [Required]
        public int AddressId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
