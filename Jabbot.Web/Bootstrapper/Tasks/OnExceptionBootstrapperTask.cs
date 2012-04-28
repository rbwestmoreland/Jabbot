using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class OnExceptionBootstrapperTask : IBootstrapperPerInstanceTask
    {
        public void Execute(HttpApplication context)
        {
            context.Error += new EventHandler(context_Error);
        }

        void context_Error(object sender, EventArgs e)
        {
            if (sender is HttpApplication)
            {
                var context = sender as HttpApplication;
                var error = context.Server.GetLastError();
            }
        }
    }
}
