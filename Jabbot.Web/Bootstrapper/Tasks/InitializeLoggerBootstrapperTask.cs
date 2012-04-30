using System.Configuration;
using Le;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Jabbot.Web.Bootstrapper.Tasks
{
    internal class InitializeLoggerBootstrapperTask : IBootstrapperPerApplicationTask
    {
        public void Execute()
        {
            string key = ConfigurationManager.AppSettings["LOGENTRIES_ACCOUNT_KEY"];
            string location = ConfigurationManager.AppSettings["LOGENTRIES_LOCATION"];

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(location))
            {
                var loggingConfiguration = new LoggingConfiguration();

                var logEntiresTarget = new LeTarget();
                logEntiresTarget.Key = key;
                logEntiresTarget.Location = location;
                logEntiresTarget.Debug = true;
                logEntiresTarget.Layout = "${date:format=u} ${level} ${message}: ${exception:format=tostring}";
                loggingConfiguration.AddTarget("logentries", logEntiresTarget);

                var loggingRule = new LoggingRule("*", LogLevel.Debug, logEntiresTarget);
                loggingConfiguration.LoggingRules.Add(loggingRule);

                LogManager.Configuration = loggingConfiguration;
            }
        }
    }
}
