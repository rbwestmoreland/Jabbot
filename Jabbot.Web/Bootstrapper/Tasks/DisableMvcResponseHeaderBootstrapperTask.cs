using System.Web.Mvc;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class DisableMvcResponseHeaderBootstrapperTask : IBootstrapperPerApplicationTask
    {
        public void Execute()
        {
            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}
