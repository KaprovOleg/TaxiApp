using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaxiApp.Startup))]
namespace TaxiApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
