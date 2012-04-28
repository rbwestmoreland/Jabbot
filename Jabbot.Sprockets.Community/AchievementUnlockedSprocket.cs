using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot achievement_unlocked.coffee script
    /// </summary>
    public class AchievementUnlockedSprocket : RegexSprocket
    {
        private const string Url = "http://achievement-unlocked.heroku.com/xbox/{0}.png";

        public override string Name { get { return "Achievement Unlocked Sprocket"; } }

        public override string Description { get { return "Life goals are in reach."; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> achievement help",
                    "achievement unlock <achievement> [achiever's gravatar email]",
                    "achievement unlocked <achievement> [achiever's gravatar email]",
                    "achievement get <achievement> [achiever's gravatar email]",
                };
            }
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex(@"(?i)^(achievement help)$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                {
                    new Regex(@"(?i)^(achievement unlock )(.+?)( ?)(\S+@\S+)?$"),
                    new Regex(@"(?i)^(achievement unlocked )(.+?)( ?)(\S+@\S+)?$"),
                    new Regex(@"(?i)^(achievement get )(.+?)( ?)(\S+@\S+)?$"),
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
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var match = RoomMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                var achievement = Uri.EscapeUriString(match.Groups[2].Value);
                var email = Uri.EscapeUriString(match.Groups[5].Value);

                var url = String.Format(Url, achievement);

                if (!String.IsNullOrWhiteSpace(email))
                {
                    url += String.Format("?email={0}.png", email);
                }

                jabbrClient.SayToRoom(message.Room, url);
            }
        }
    }
}
