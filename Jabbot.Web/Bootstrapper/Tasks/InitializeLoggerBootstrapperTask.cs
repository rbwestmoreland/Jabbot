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
                LoggingConfiguration config = new LoggingConfiguration();

                LeTarget logEntiresTarget = new LeTarget();
                config.AddTarget("logentries", logEntiresTarget);
                logEntiresTarget.Key = key;
                logEntiresTarget.Location = location;
                logEntiresTarget.Debug = true;
                logEntiresTarget.Layout = "${date:format=ddd MMM dd} ${time:format=HH:mm:ss} ${date:format=zzz yyyy} ${logger} : ${LEVEL}, ${message, ${exception:format=tostring}";

                LoggingRule rule = new LoggingRule("*", LogLevel.Debug, logEntiresTarget);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }
        }
    }
}
