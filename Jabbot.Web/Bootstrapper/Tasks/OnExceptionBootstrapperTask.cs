using System;
using System.Web;
using NLog;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class OnExceptionBootstrapperTask : IBootstrapperPerInstanceTask
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public void Execute(HttpApplication context)
        {
            context.Error += new EventHandler(context_Error);
        }

        void context_Error(object sender, EventArgs e)
        {
            if (sender is HttpApplication)
            {
                var context = sender as HttpApplication;
                var exception = context.Server.GetLastError().GetBaseException();
                Logger.ErrorException("An unhandled exception has occured.", exception);
                context.Server.ClearError();
            }
        }
    }
}
