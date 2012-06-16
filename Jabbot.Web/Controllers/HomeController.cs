using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using BookSleeve;
using Jabbot.Core.Sprockets;
using Jabbot.Web.Models.Home;
using Jabbot.Web.Models.Jabbot;
using Jabbot.Web.Models.Sprockets;
using Jabbot.Web.Models.Statistics;
using Jabbot.Web.Models.Status;
using NLog;

namespace Jabbot.Web.Controllers
{
    public class HomeController : Controller
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private RedisConnection RedisConnection { get; set; }

        public HomeController(RedisConnection redisConnection)
        {
            if (redisConnection == null)
            {
                throw new ArgumentNullException("redisConnection");
            }

            RedisConnection = redisConnection;
            RedisConnection.Open().Wait();
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
                var allTimeHashFields = RedisConnection.Hashes.GetKeys(0, "Jabbot:Statistics:Sprockets:Usage:AllTime").Result;
                var allTimeHashValues = RedisConnection.Hashes.GetString(0, "Jabbot:Statistics:Sprockets:Usage:AllTime", allTimeHashFields).Result;
                var yearlyHashFields = RedisConnection.Hashes.GetKeys(0, string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyy}", DateTimeOffset.UtcNow)).Result;
                var yearlyHashValues = yearlyHashFields.Length.Equals(0) ? new string[] { "0" } : RedisConnection.Hashes.GetString(0, string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyy}", DateTimeOffset.UtcNow), yearlyHashFields).Result;
                var monthlyHashFields = RedisConnection.Hashes.GetKeys(0, string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMM}", DateTimeOffset.UtcNow)).Result;
                var monthlyHashValues = monthlyHashFields.Length.Equals(0) ? new string[] { "0" } : RedisConnection.Hashes.GetString(0, string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMM}", DateTimeOffset.UtcNow), monthlyHashFields).Result;
                var dailyHashFields = RedisConnection.Hashes.GetKeys(0, string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMMdd}", DateTimeOffset.UtcNow)).Result;
                var dailyHashValues = dailyHashFields.Length.Equals(0) ? new string[] { "0" } : RedisConnection.Hashes.GetString(0, string.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMMdd}", DateTimeOffset.UtcNow), dailyHashFields).Result;

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
                var redisKey = "Jabbot:LastSeen";
                if (RedisConnection.Keys.Exists(0, redisKey).Result)
                {
                    var dateTimeOffsetString = RedisConnection.Strings.GetString(0, redisKey).Result;
                    dateTimeOffsetString = dateTimeOffsetString.Trim('\"');
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
                        RedisConnection.Dispose();
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
