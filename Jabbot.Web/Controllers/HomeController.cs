using System;
using System.Configuration;
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

        public ActionResult Get()
        {
            var version = Assembly.GetAssembly(typeof(ISprocket)).GetName().Version.ToString();

            var jabbot = new JabbotViewModel(version);
            var jabbotStatistics = GetStatisticsViewModel();
            var jabbotStatisticsViewModel = new JabbotStatisticsViewModel(jabbot, jabbotStatistics);

            var sprockets = Container.Sprockets.Select(s => new SprocketViewModel(s.Name, s.Description, s.Usage)).OrderBy(s => s.Name);
            var sprocketStatisticsViewModel = sprockets.Select((SprocketViewModel s) =>
            {
                var statistics = StatisticsViewModel.Default;
                return new SprocketStatisticsViewModel(s, statistics);
            });

            var statusViewModel = GetStatusViewModel();

            var model = new HomeViewModel(jabbotStatisticsViewModel, sprocketStatisticsViewModel, statusViewModel);

            return View(model);
        }

        private StatusViewModel GetStatusViewModel()
        {
            var viewModel = StatusViewModel.Default;

            try
            {
                var uri = new Uri(ConfigurationManager.AppSettings["REDISTOGO_URL"]);
                using (var redisClient = new RedisClient(uri))
                {
                    var dateTimeOffsetString = redisClient.Get<string>("Jabbot:LastSeen");
                    var lastSeen = DateTimeOffset.Parse(dateTimeOffsetString);
                    viewModel = new StatusViewModel(lastSeen);
                }
            }
            catch(Exception ex)
            {
                Logger.ErrorException("An error occured while populating the StatusViewModel.", ex);
            }

            return viewModel;
        }

        private StatisticsViewModel GetStatisticsViewModel()
        {
            var viewModel = StatisticsViewModel.Default;

            try
            {
                var uri = new Uri(ConfigurationManager.AppSettings["REDISTOGO_URL"]);
                using (var redisClient = new RedisClient(uri))
                {
                    var allTimeHashValues = redisClient.GetHashValues("Jabbot:Statistics:Sprockets:Usage:AllTime");
                    var yearlyHashValues = redisClient.GetHashValues(string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyy}", DateTimeOffset.UtcNow));
                    var monthlyHashValues = redisClient.GetHashValues(string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMM}", DateTimeOffset.UtcNow));
                    var dailyHashValues = redisClient.GetHashValues(string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMMdd}", DateTimeOffset.UtcNow));

                    var allTimeSum = (from hv in allTimeHashValues select Int64.Parse(hv)).Sum();
                    var yearlySum = (from hv in yearlyHashValues select Int64.Parse(hv)).Sum();
                    var monthlySum = (from hv in monthlyHashValues select Int64.Parse(hv)).Sum();
                    var dailySum = (from hv in dailyHashValues select Int64.Parse(hv)).Sum();

                    viewModel = new StatisticsViewModel(allTimeSum, dailySum, monthlySum);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while populating the StatisticsViewModel.", ex);
            }

            return viewModel;
        }
    }
}
