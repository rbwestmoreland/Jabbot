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
    /// Port of the Hubot coin.coffee script
    /// </summary>
    public class CoinSprocket : RegexSprocket
    {
        public override string Name { get { return "Coin Sprocket"; } }

        public override string Description { get { return "Help decide between two things."; } }

        private IEnumerable<string> Coin { get { return new String[] { "heads", "tails" }; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> throw|flip|toss a coin help",
                    "/msg <botnick> throw|flip|toss a coin",
                    "throw|flip|toss a coin",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(throw|flip|toss)( a coin help)$"),
                    new Regex("(?i)^(throw|flip|toss)( a coin)$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)(throw|flip|toss)( a coin)"),
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
                    jabbrClient.PrivateReply(message.From, Coin.RandomElement());
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                jabbrClient.SayToRoom(message.Room, Coin.RandomElement());
            }
        }
    }
}
