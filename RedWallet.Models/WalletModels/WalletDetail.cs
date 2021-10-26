using NBitcoin;
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
        public int Id { get; set; }
        [Required, Display(Name = "Wallet Name")]
        public string WalletName { get; set; }
        [Display(Name = "Past Payment Requests")]
        public IEnumerable<Request> PastPaymentRequests { get; set; }
        [Display(Name = "Outgoing Payments")]
        public IEnumerable<Send> OutgoingPayments { get; set; }
    }
}