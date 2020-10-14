using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NCKH.HTMLSteg.Startup))]
namespace NCKH.HTMLSteg
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
