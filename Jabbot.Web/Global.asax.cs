using System.Web;
using NLog;

namespace Jabbot.Web
{
    public class MvcApplication : HttpApplication
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            Bootstrapper.Bootstrapper.PerApplication();
            Logger.Info("Jabbot Web Started");
        }

        public override void Init()
        {
            base.Init();
            Bootstrapper.Bootstrapper.PerInstance(this);
        }
    }
}