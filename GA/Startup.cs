using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GA.Startup))]
namespace GA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
