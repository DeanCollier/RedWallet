using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RedWallet.WebMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Create",
                url: "Wallet/{walletId}/{controller}/{action}");

            routes.MapRoute(
                name: "Details",
                url: "Wallet/{walletId}/{controller}/{id}/{action}",
                defaults: new { action = "Details" }
            );

            routes.MapRoute(
                name: "Delete",
                url: "Wallet/{walletId}/{controller}/{id}/{action}",
                defaults: new { action = "Delete" }
            );
        }
    }
}
