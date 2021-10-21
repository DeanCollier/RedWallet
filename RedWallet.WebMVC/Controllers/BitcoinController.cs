using RedWallet.WebMVC.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RedWallet.WebMVC.Controllers
{
    public class BitcoinController : Controller
    {
        // GET: Bitcoin
        public async Task<ActionResult> Index()
        {
            var _bitcoin = new Bitcoin();
            var wallet = await _bitcoin.CreateWallet();
            return View();
        }
    }
}