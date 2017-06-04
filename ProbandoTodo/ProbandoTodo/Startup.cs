using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProbandoTodo.Startup))]
namespace ProbandoTodo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
