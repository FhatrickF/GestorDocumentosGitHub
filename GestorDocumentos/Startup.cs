using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GestorDocumentos.Startup))]
namespace GestorDocumentos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
