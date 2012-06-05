﻿using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;
using Le;
using NLog;
using NLog.Config;
using NLog.Targets;
using ServiceStack.Redis;

namespace Jabbot.Console
{
    class Program
    {
        private static string BotName { get { return ConfigurationManager.AppSettings["Bot.Name"]; } }
        private static string BotPassword { get { return ConfigurationManager.AppSettings["Bot.Password"]; } }
        private static string BotGravatarEmail { get { return ConfigurationManager.AppSettings["Bot.GravatarEmail"]; } }
        private static string BotServer { get { return ConfigurationManager.AppSettings["Bot.Server"]; } }
        private static string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        private static Logger Logger { get { return LogManager.GetCurrentClassLogger(); } }
        private static IJabbrClient JabbRClient { get; set; }
        private static IRedisClient RedisClient { get; set; }
        private static Timer AliveTimer { get; set; }
        private static bool ShouldExit { get; set; }

        static int Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            try
            {
                System.Console.WriteLine(String.Format("Jabbot v{0}", Version));
                Initialize();
                while (!ShouldExit) { }
            }
            catch (Exception ex)
            {
                var exception = ex.GetBaseException();
                Logger.ErrorException("An error occured while starting.", exception);
            }
            finally
            {
                Shutdown();
            }

            return -1;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            if (e.IsTerminating)
            {
                Logger.FatalException("An unhandled exception is causing the worker to terminate.", exception);
                ShouldExit = true;
            }
            else
            {
                Logger.ErrorException("An unhandled exception occurred in the worker process.", exception);
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.ErrorException("An unobserved task exception occurred.", e.Exception);
            e.SetObserved();
        }

        private static void Initialize()
        {
            InitializeLogger();
            Logger.Info("Jabbot Console Starting");
            InitializeRedisClient();
            InitializeJabbRClient();
            InitializeAlivePingCronJob();
            Logger.Info("Jabbot Console Started");
        }

        private static void InitializeLogger()
        {
            string key = ConfigurationManager.AppSettings["LOGENTRIES_ACCOUNT_KEY"];
            string location = ConfigurationManager.AppSettings["LOGENTRIES_LOCATION"];

            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=u} ${logger} ${level} ${message}";
            loggingConfiguration.AddTarget("console", consoleTarget);

            LoggingRule loggingRule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            loggingConfiguration.LoggingRules.Add(loggingRule1);

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(location))
            {
                LeTarget logEntiresTarget = new LeTarget();
                logEntiresTarget.Key = key;
                logEntiresTarget.Location = location;
                logEntiresTarget.Debug = true;
                logEntiresTarget.Layout = "${date:format=u} ${logger} ${level} ${message} ${exception:format=tostring}";
                loggingConfiguration.AddTarget("logentries", logEntiresTarget);

                LoggingRule loggingRule2 = new LoggingRule("*", LogLevel.Debug, logEntiresTarget);
                loggingConfiguration.LoggingRules.Add(loggingRule2);
            }

            LogManager.Configuration = loggingConfiguration;
        }

        private static void InitializeJabbRClient()
        {
            try
            {
                Logger.Info("Initializing JabbR Client Started");
                JabbRClient = new JabbrClient(BotServer);
                JabbRClient.OnReceivePrivateMessage += ProcessPrivateMessage;
                JabbRClient.OnReceiveRoomMessage += ProcessRoomMessage;
                JabbRClient.Connect();
                JabbRClient.Login(BotName, BotPassword, BotGravatarEmail);
                Logger.Info("Initializing JabbR Client Completed");
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An exception occurred while initializing JabbR client.", ex);
            }
        }

        private static void InitializeRedisClient()
        {
            try
            {
                Logger.Info("Initializing Redis Client Started");
                var uri = new Uri(ConfigurationManager.AppSettings["REDISTOGO_URL"]);
                RedisClient = new RedisClient(uri);
                Logger.Info("Initializing Redis Client Completed");
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured initializing the RedisClient.", ex);
            }
        }

        private static void InitializeAlivePingCronJob()
        {
            Logger.Info("Initializing Alive Ping Cron Started");
            var callback = new TimerCallback((object o) =>
            {
                const string key = "Jabbot:LastSeen";
                try
                {
                    if (JabbRClient != null)
                    {
                        if (JabbRClient.IsConnected)
                        {
                            if (RedisClient != null)
                            {
                                RedisClient.Set<string>(key, DateTimeOffset.UtcNow.ToString("u"));
                            }
                        }
                        else
                        {
                            JabbRClient.Connect();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException(String.Format("There was an error setting redis key: {0}", key), ex);
                }
            });
            AliveTimer = new Timer(callback, null, new TimeSpan(0, 0, 30), new TimeSpan(0, 0, 30));
            Logger.Info("Initializing Alive Ping Cron Completed");
        }

        private static void Shutdown()
        {
            try
            {
                AliveTimer = null;
                JabbRClient.Dispose();
                RedisClient.Dispose();
                Logger.Info("Exiting");
            }
            catch { }
        }

        private static void ProcessPrivateMessage(string from, string to, string content)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (from.Equals(BotName, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    var privateMessage = new PrivateMessage(from, WebUtility.HtmlDecode(content));
                    var handled = false;

                    foreach (var sprocket in Container.Sprockets)
                    {
                        if (sprocket.CanHandle(privateMessage))
                        {
                            Logger.Info(string.Format("Message received from: {0} > {1}", from, content));
                            IncrementSprocketUsage(sprocket.Name);
                            sprocket.Handle(privateMessage, JabbRClient);
                            handled = true;
                            break;
                        }
                    }

                    if (!handled)
                    {
                        JabbRClient.PrivateReply(privateMessage.From, "I don't understand that command.");
                    }
                }
                catch (Exception ex)
                {
                    var exception = ex.GetBaseException();
                    Logger.ErrorException("An error occured while processing a private message.", ex);
                }
            });
        }

        private static void ProcessRoomMessage(dynamic message, string room)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var from = message.User.Name.Value;
                    var content = message.Content.Value;

                    if (from.Equals(BotName, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    var roomMessage = new RoomMessage(room, from, WebUtility.HtmlDecode(content));

                    foreach (var sprocket in Container.Sprockets)
                    {
                        if (sprocket.CanHandle(roomMessage))
                        {
                            Logger.Info(string.Format("Message received from: {0} > {1}", from, content));
                            IncrementSprocketUsage(sprocket.Name);
                            sprocket.Handle(roomMessage, JabbRClient);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var exception = ex.GetBaseException();
                    Logger.ErrorException("An error occured while processing a room message.", exception);
                }
            });
        }

        private static void IncrementSprocketUsage(string sprocket)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (RedisClient != null)
                        {
                            var utcNow = DateTimeOffset.UtcNow;

                            string allTimeHashId = "Jabbot:Statistics:Sprockets:Usage:AllTime";
                            RedisClient.SetEntryInHashIfNotExists(allTimeHashId, sprocket, "0");
                            RedisClient.IncrementValueInHash(allTimeHashId, sprocket, 1);

                            string yearHashId = String.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyy}", utcNow);
                            RedisClient.SetEntryInHashIfNotExists(yearHashId, sprocket, "0");
                            RedisClient.IncrementValueInHash(yearHashId, sprocket, 1);

                            string monthHashId = String.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMM}", utcNow);
                            RedisClient.SetEntryInHashIfNotExists(monthHashId, sprocket, "0");
                            RedisClient.IncrementValueInHash(monthHashId, sprocket, 1);

                            string dayHashId = String.Format("Jabbot:Statistics:Sprockets:Usage:{0:yyyyMMdd}", utcNow);
                            RedisClient.SetEntryInHashIfNotExists(dayHashId, sprocket, "0");
                            RedisClient.IncrementValueInHash(dayHashId, sprocket, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorException("An error occured incrementing Sprocket usage statistics.", ex);
                    }
                });
        }
    }
}
