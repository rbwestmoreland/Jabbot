using System.Web;

namespace Jabbot.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Bootstrapper.Bootstrapper.PerApplication();
        }

        public override void Init()
        {
            base.Init();
            Bootstrapper.Bootstrapper.PerInstance(this);
        }
    }
}