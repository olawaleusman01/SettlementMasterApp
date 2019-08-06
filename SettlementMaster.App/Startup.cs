using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SettlementMaster.App.Startup))]
namespace SettlementMaster.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
