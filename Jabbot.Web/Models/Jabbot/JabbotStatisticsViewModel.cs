using Jabbot.Web.Models.Statistics;

namespace Jabbot.Web.Models.Jabbot
{
    public class JabbotStatisticsViewModel
    {
        public static JabbotStatisticsViewModel Default 
        { 
            get 
            {
                return new JabbotStatisticsViewModel(JabbotViewModel.Default, StatisticsViewModel.Default); 
            } 
        }

        public JabbotViewModel Jabbot { get; private set; }
        public StatisticsViewModel Statistics { get; private set; }

        public JabbotStatisticsViewModel(JabbotViewModel jabbot, StatisticsViewModel statistics)
        {
            Jabbot = jabbot ?? JabbotViewModel.Default;
            Statistics = statistics ?? StatisticsViewModel.Default;
        }
    }
}