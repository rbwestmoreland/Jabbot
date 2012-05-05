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
                "Home",
                "",
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default",
                "{*url}",
                new { controller = "Error", action = "Http404" }
            );
        }
    }
}
