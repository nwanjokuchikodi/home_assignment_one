using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(home_assignment_one.Startup))]
namespace home_assignment_one
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
