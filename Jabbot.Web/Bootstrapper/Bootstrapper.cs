using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Jabbot.Web.Bootstrapper.Tasks;
using Jabbot.Web.Controllers;
using TinyIoC;

namespace Jabbot.Web.Bootstrapper
{
    internal static class Bootstrapper
    {
        public static TinyIoCContainer IoCContainer { get; set; }

        private static IEnumerable<IBootstrapperPerApplicationTask> PerApplicationTasks { get; set; }

        private static IEnumerable<IBootstrapperPerInstanceTask> PerInstanceTasks { get; set; }

        static Bootstrapper()
        {
            IoCContainer = GetIoCContainer();
            PerApplicationTasks = IoCContainer.ResolveAll<IBootstrapperPerApplicationTask>(true);
            PerInstanceTasks = IoCContainer.ResolveAll<IBootstrapperPerInstanceTask>(true);
        }

        public static void PerInstance(HttpApplication httpApplication)
        {
            foreach (var task in PerInstanceTasks)
            {
                task.Execute(httpApplication);
            }
        }

        public static void PerApplication()
        {
            foreach (var task in PerApplicationTasks)
            {
                task.Execute();
            }
        }

        private static TinyIoCContainer GetIoCContainer()
        {
            var container = new TinyIoCContainer();

            //Configuration Values
            //Bootstrapper Tasks - On Start
            container.Register<IBootstrapperPerApplicationTask, InitializeLoggerBootstrapperTask>("InitializeLoggerBootstrapperTask");
            container.Register<IBootstrapperPerApplicationTask, DisableMvcResponseHeaderBootstrapperTask>("DisableMvcResponseHeaderBootstrapperTask");
            container.Register<IBootstrapperPerApplicationTask, TinyIoCControllerFactoryBootstrapperTask>("TinyIoCControllerFactoryBootstrapperTask");
            container.Register<IBootstrapperPerApplicationTask, RegisterRouteBootstrapperTask>("RegisterRouteBootstrapperTask");
            //Bootstrapper Tasks - Per Instance
            container.Register<IBootstrapperPerInstanceTask, ResponseTimeHeaderBootstrapperTask>("ResponseTimeHeaderBootstrapperTask");
            container.Register<IBootstrapperPerInstanceTask, OnExceptionBootstrapperTask>("OnExceptionBootstrapperTask");
            //Controllers
            container.Register<IController, HomeController>("Home").AsMultiInstance();
            //Domain
            //
            //Resource

            return container;
        }
    }
}