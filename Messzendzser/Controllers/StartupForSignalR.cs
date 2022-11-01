using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Messzendzser.Controllers.Startup))]
namespace Messzendzser.Controllers
{
    public class Startup 
    {
        public void Configuration(IAppBuilder app)
        {  
            //TODO Any connection or hub wire up and configuration should go here  
            app.MapSignalR();  
        }
    }
}
