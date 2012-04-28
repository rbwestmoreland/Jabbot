using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using TinyIoC;

namespace Jabbot.Web.Controllers.Factories
{
    internal class TinyIoCControllerFactory : IControllerFactory
    {
        private TinyIoCContainer Container { get; set; }

        public TinyIoCControllerFactory(TinyIoC.TinyIoCContainer container)
        {
            Container = container;
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            return Container.Resolve<IController>(controllerName);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return System.Web.SessionState.SessionStateBehavior.Default;
        }

        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}