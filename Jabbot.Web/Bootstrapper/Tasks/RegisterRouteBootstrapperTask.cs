using System.Web.Mvc;
using System.Web.Routing;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class RegisterRouteBootstrapperTask : IBootstrapperPerApplicationTask
    {
        public void Execute()
        {
            var routes = RouteTable.Routes;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }
            );
        }
    }
}
