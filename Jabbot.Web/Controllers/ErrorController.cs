using System.Web.Mvc;
using NLog;

namespace Jabbot.Web.Controllers
{
    public class ErrorController : Controller
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public ActionResult Http404()
        {
            return RedirectToRoute("Home");
        }

        #region IDisposable Member(s)

        private bool Disposed { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                try
                {
                    if (disposing)
                    {
                    }
                    this.Disposed = true;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        #endregion IDisposable Member(s)
    }
}
