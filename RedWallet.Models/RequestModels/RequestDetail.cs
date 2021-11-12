﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Models.RequestModels
{
    public class RequestDetail
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        public string WalletName { get; set; }

        [Required, Display(Name = "Address")]
        public string RequestAddress { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }
    }
}
