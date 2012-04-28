using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal interface IBootstrapperPerInstanceTask
    {
        void Execute(HttpApplication application);
    }
}
