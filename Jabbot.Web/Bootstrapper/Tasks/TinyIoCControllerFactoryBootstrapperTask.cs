using System.Web.Mvc;
using Jabbot.Web.Controllers.Factories;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class TinyIoCControllerFactoryBootstrapperTask : IBootstrapperPerApplicationTask
    {
        public void Execute()
        {
            var controllerFactory = new TinyIoCControllerFactory(Bootstrapper.IoCContainer);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}
