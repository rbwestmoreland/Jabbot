using System;
using System.Collections.Generic;
using Jabbot.Web.Models.Jabbot;
using Jabbot.Web.Models.Sprockets;
using Jabbot.Web.Models.Status;

namespace Jabbot.Web.Models.Home
{
    public class HomeViewModel
    {
        public JabbotStatisticsViewModel JabbotStatistics { get; private set; }
        public IEnumerable<SprocketStatisticsViewModel> SprocketStatistics { get; private set; }
        public StatusViewModel Status { get; private set; }

        public HomeViewModel(JabbotStatisticsViewModel jabbotStatistics, IEnumerable<SprocketStatisticsViewModel> sprocketStatistics, StatusViewModel status)
        {
            JabbotStatistics = jabbotStatistics ?? JabbotStatisticsViewModel.Default;
            SprocketStatistics = sprocketStatistics ?? new List<SprocketStatisticsViewModel>();
            Status = status ?? StatusViewModel.Default;
        }

        public bool IsOnline()
        {
            if (DateTimeOffset.UtcNow.Subtract(Status.LastSeen) < new TimeSpan(0, 5, 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}