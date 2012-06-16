using System;
using System.Web;
using NLog;
using System.Threading.Tasks;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class OnExceptionBootstrapperTask : IBootstrapperPerInstanceTask
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public void Execute(HttpApplication context)
        {
            context.Error += new EventHandler(context_Error);
            TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private static void context_Error(object sender, EventArgs e)
        {
            if (sender is HttpApplication)
            {
                var context = sender as HttpApplication;
                var exception = context.Server.GetLastError().GetBaseException();
                Logger.ErrorException("An unhandled exception has occured.", exception);
                context.Server.ClearError();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            if (e.IsTerminating)
            {
                Logger.FatalException("An unhandled exception is causing the application to terminate.", exception);
            }
            else
            {
                Logger.ErrorException("An unhandled exception occurred in the application process.", exception);
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.ErrorException("An unobserved task exception occurred.", e.Exception);
            e.SetObserved();
        }
    }
}
