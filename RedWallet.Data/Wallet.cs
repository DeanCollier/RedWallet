﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedWallet.Data
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string WalletName { get; set; }
        [Required]
        public string PassphraseHash { get; set; }
        [Required]
        public string PrivateKey { get; set; }

    }
}
