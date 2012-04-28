using System;

namespace Jabbot.Web.Models.Statistics
{
    public class StatisticsViewModel
    {
        public static StatisticsViewModel Default { get { return new StatisticsViewModel(0, 0, 0); } }

        public long TotalRequestsHandledLifetime { get; private set; }
        public long TotalRequestsHandledThisDay { get; private set; }
        public long TotalRequestsHandledThisMonth { get; private set; }

        public StatisticsViewModel(
            long totalRequestsHandled, 
            long totalRequestsHandledThisDay, 
            long totalRequestsHandledThisMonth)
        {
            TotalRequestsHandledLifetime = totalRequestsHandled;
            TotalRequestsHandledThisDay = totalRequestsHandledThisDay;
            TotalRequestsHandledThisMonth = totalRequestsHandledThisMonth;
        }
    }
}