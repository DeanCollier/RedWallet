using RedWallet.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace RedWallet.Models.WalletModels
{
    public class WalletDetail
    {
        [Required]
        public int WalletId { get; set; }

        [Required, Display(Name = "Wallet Name")]
        public string WalletName { get; set; }

        [Required]
        [Display(Name = "Extended Public Key")]

        public string Xpub { get; set; }
        [Required]
        public int XpubIteration { get; set; }

    }
}