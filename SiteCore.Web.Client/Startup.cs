using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SiteCore.Web.Client.Startup))]
namespace SiteCore.Web.Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
