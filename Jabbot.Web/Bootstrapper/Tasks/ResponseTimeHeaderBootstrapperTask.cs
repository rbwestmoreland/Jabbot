using System;
using System.Diagnostics;
using System.Web;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class ResponseTimeHeaderBootstrapperTask : IBootstrapperPerInstanceTask
    {
        private static readonly String XAppriseResponseTimeHeaderName = "X-ResponseTime";

        public void Execute(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(BeginRequest);
            context.EndRequest += new EventHandler(EndRequest);
        }

        public void BeginRequest(object sender, EventArgs e)
        {
            try
            {
                if (sender is HttpApplication)
                {
                    var httpApplication = (HttpApplication)sender;
                    var stopwatch = Stopwatch.StartNew();
                    httpApplication.Context.Items.Add(XAppriseResponseTimeHeaderName, stopwatch);
                }
            }
            catch
            {
            }
        }

        public void EndRequest(object sender, EventArgs e)
        {
            try
            {
                if (sender is HttpApplication)
                {
                    var httpApplication = (HttpApplication)sender;

                    if (httpApplication.Context.Items.Contains(XAppriseResponseTimeHeaderName))
                    {
                        var stopwatch = (Stopwatch)httpApplication.Context.Items[XAppriseResponseTimeHeaderName];
                        stopwatch.Stop();
                        var responseTime = String.Format("{0} ms", Math.Ceiling(stopwatch.Elapsed.TotalMilliseconds));
                        httpApplication.Response.AddHeader(XAppriseResponseTimeHeaderName, responseTime);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
