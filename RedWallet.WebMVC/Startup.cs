using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RedWallet.WebMVC.Startup))]
namespace RedWallet.WebMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
