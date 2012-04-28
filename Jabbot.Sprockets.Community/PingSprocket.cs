using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot ping.coffee script
    /// </summary>
    public class PingSprocket : RegexSprocket
    {
        public override string Name { get { return "Ping Sprocket"; } }

        public override string Description { get { return "Utility commands surrounding Jabbot uptime."; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> ping|echo|time|die help",
                    "/msg <botnick> ping",
                    "/msg <botnick> echo <text>",
                    "/msg <botnick> time",
                    "/msg <botnick> die",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(ping|echo|time|die)( help)$"),
                    new Regex("(?i)^(ping)$"),
                    new Regex("(?i)^(echo )(.*)"),
                    new Regex("(?i)^(time)$"),
                    new Regex("(?i)^(die)$"),
                }; 
            }
        }

        public override void Handle(IPrivateMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                if (PrivateMessagePatterns.First().Match(message.Content).Success)
                {
                    jabbrClient.PrivateReply(message.From, this.GetFormattedHelp());
                }
                else if (PrivateMessagePatterns.ElementAt(1).Match(message.Content).Success)
                {
                    jabbrClient.PrivateReply(message.From, "pong");
                }
                else if (PrivateMessagePatterns.ElementAt(2).Match(message.Content).Success)
                {
                    var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                    jabbrClient.PrivateReply(message.From, match.Groups[2].Value);
                }
                else if (PrivateMessagePatterns.ElementAt(3).Match(message.Content).Success)
                {
                    jabbrClient.PrivateReply(message.From, String.Format("{0:F} UTC", DateTimeOffset.UtcNow));
                }
                else if (PrivateMessagePatterns.ElementAt(4).Match(message.Content).Success)
                {
                    jabbrClient.PrivateReply(message.From, String.Format("I'm sorry, {0}. I'm afraid I can't do that.", message.From));
                }
            }
        }
    }
}
