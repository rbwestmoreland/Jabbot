using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Extensions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot decide.coffee script
    /// </summary>
    public class DecideSprocket : RegexSprocket
    {
        public override string Name { get { return "Decide Sprocket"; } }

        public override string Description { get { return "Help you decide between multiple options."; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> decide help",
                    "/msg <botnick> decide \"option1\" \"option2\" \"optionN\"",
                    "decide \"option1\" \"option2\" \"optionN\"",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(decide help)$"),
                    new Regex("(?i)(?<=^decide )(( ?\".+?\"){1,})"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)(?<=^decide )(( ?\".+?\"){1,})"),
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
                else
                {
                    var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                    var choices = match.Groups[1].Value.Split(new string[] { "\"" }, StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s));
                    jabbrClient.PrivateReply(message.From, choices.RandomElement());
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                var choices = match.Groups[1].Value.Split(new string[] { "\"" }, StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s));
                jabbrClient.SayToRoom(message.Room, choices.RandomElement());
            }
        }
    }
}
