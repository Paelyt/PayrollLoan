using Microsoft.Owin;
using Owin;

//[assembly: OwinStartupAttribute(typeof(UvlotApplication.Startup))]
[assembly: OwinStartupAttribute("UvlotApplicationConfig", typeof(UvlotApplication.Startup))]
namespace UvlotApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
