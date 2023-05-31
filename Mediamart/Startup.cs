using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mediamart.Startup))]
namespace Mediamart
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
