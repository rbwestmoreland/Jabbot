using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Jabbot.Web.Bootstrapper.Tasks;
using Jabbot.Web.Controllers;
using BookSleeve;
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
            container.Register<IController, ErrorController>("Error").AsMultiInstance();
            //Domain
            container.Register<RedisConnection>(RedisClientConstructor);
            //Resource
            //

            return container;
        }

        private static Func<TinyIoCContainer, NamedParameterOverloads, RedisConnection> RedisClientConstructor = (TinyIoCContainer c, NamedParameterOverloads p) =>
        {
            try
            {
                var uri = new Uri(ConfigurationManager.AppSettings["REDISTOGO_URL"]);
                var host = uri.Host;
                var port = uri.Port;
                var password = uri.UserInfo.Split(':')[1];
                var redisConnection = new RedisConnection(host, port, -1, password);
                return redisConnection;
            }
            catch
            {
                //todo: Handle this
                throw;
            }
        };
    }
}