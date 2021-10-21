using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Send
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(1)]
        public string BtcTransaction { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
