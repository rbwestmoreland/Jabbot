using Jabbot.Web.Models.Statistics;

namespace Jabbot.Web.Models.Sprockets
{
    public class SprocketStatisticsViewModel
    {
        public SprocketViewModel Sprocket { get; private set; }
        public StatisticsViewModel Statistics { get; private set; }

        public SprocketStatisticsViewModel(SprocketViewModel sprocket, StatisticsViewModel statistics)
        {
            Sprocket = sprocket ?? SprocketViewModel.Unknown;
            Statistics = statistics ?? StatisticsViewModel.Default;
        }
    }
}