using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Jabbot.Core.Sprockets;
using Jabbot.Web.Models.Home;
using Jabbot.Web.Models.Jabbot;
using Jabbot.Web.Models.Sprockets;
using Jabbot.Web.Models.Statistics;
using Jabbot.Web.Models.Status;
using NLog;
using ServiceStack.Redis;

namespace Jabbot.Web.Controllers
{
    public class HomeController : Controller
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private IRedisClient RedisClient { get; set; }

        public HomeController(IRedisClient redisClient)
        {
            if (redisClient == null)
            {
                throw new ArgumentNullException("redisClient");
            }

            RedisClient = redisClient;
        }

        public ActionResult Get()
        {
            var jabbotStatisticsViewModel = GetJabbotStatisticsViewModel();
            var sprocketStatisticsViewModel = GetSprocketStatisticsViewModel();
            var statusViewModel = GetStatusViewModel();

            var model = new HomeViewModel(jabbotStatisticsViewModel, sprocketStatisticsViewModel, statusViewModel);

            return View(model);
        }

        private JabbotStatisticsViewModel GetJabbotStatisticsViewModel()
        {
            var jabbotStatisticsViewModel = JabbotStatisticsViewModel.Default;

            try
            {
                var version = Assembly.GetAssembly(typeof(ISprocket)).GetName().Version.ToString();
                var jabbot = new JabbotViewModel(version);
                var jabbotStatistics = GetStatisticsViewModel();

                jabbotStatisticsViewModel = new JabbotStatisticsViewModel(jabbot, jabbotStatistics);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while populating the JabbotStatisticsViewModel.", ex);
            }

            return jabbotStatisticsViewModel;
        }

        private StatisticsViewModel GetStatisticsViewModel()
        {
            var viewModel = StatisticsViewModel.Default;

            try
            {
                var allTimeHashValues = RedisClient.GetHashValues("Jabbot:Statistics:Sprockets:Usage:AllTime");
                var yearlyHashValues = RedisClient.GetHashValues(string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyy}", DateTimeOffset.UtcNow));
                var monthlyHashValues = RedisClient.GetHashValues(string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMM}", DateTimeOffset.UtcNow));
                var dailyHashValues = RedisClient.GetHashValues(string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMMdd}", DateTimeOffset.UtcNow));

                var allTimeSum = (from hv in allTimeHashValues select Int64.Parse(hv)).Sum();
                var yearlySum = (from hv in yearlyHashValues select Int64.Parse(hv)).Sum();
                var monthlySum = (from hv in monthlyHashValues select Int64.Parse(hv)).Sum();
                var dailySum = (from hv in dailyHashValues select Int64.Parse(hv)).Sum();

                viewModel = new StatisticsViewModel(allTimeSum, dailySum, monthlySum);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while populating the StatisticsViewModel.", ex);
            }

            return viewModel;
        }

        private IEnumerable<SprocketStatisticsViewModel> GetSprocketStatisticsViewModel()
        {
            IEnumerable<SprocketStatisticsViewModel> sprocketStatisticsViewModel = new List<SprocketStatisticsViewModel>();

            try
            {
                var sprockets = Container.Sprockets.Select(s => new SprocketViewModel(s.Name, s.Description, s.Usage)).OrderBy(s => s.Name);
                sprocketStatisticsViewModel = sprockets.Select((SprocketViewModel s) =>
                {
                    var statistics = StatisticsViewModel.Default;
                    return new SprocketStatisticsViewModel(s, statistics);
                });
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while populating the SprocketStatisticsViewModel collection.", ex);
            }

            return sprocketStatisticsViewModel;
        }

        private StatusViewModel GetStatusViewModel()
        {
            var viewModel = StatusViewModel.Default;

            try
            {
                var dateTimeOffsetString = RedisClient.Get<string>("Jabbot:LastSeen");
                var lastSeen = DateTimeOffset.Parse(dateTimeOffsetString);
                viewModel = new StatusViewModel(lastSeen);
            }
            catch(Exception ex)
            {
                Logger.ErrorException("An error occured while populating the StatusViewModel.", ex);
            }

            return viewModel;
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
                        RedisClient.Dispose();
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
