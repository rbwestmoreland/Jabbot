using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot rules.coffee script
    /// </summary>
    public class RulesSprocket : RegexSprocket
    {
        public override string Name { get { return "Rules Sprocket"; } }

        public override string Description { get { return "All robawts must know the rules."; } }

        private static String[] Rules
        {
            get
            {
                return new String[]
                {
                    "1. A robot may not injure a human being or, through inaction, allow a human being to come to harm.",
                    "2. A robot must obey any orders given to it by human beings, except where such orders would conflict with the First Law.",
                    "3. A robot must protect its own existence as long as such protection does not conflict with the First or Second Law.",
                };
            }
        }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> [what are ]the three laws help",
                    "/msg <botnick> [what are ]the three laws",
                    "/msg <botnick> [what are ]the three rules",
                    "/msg <botnick> [what are ]the 3 laws",
                    "/msg <botnick> [what are ]the 3 rules",
                    "[what are ]the three laws",
                    "[what are ]the three rules",
                    "[what are ]the 3 laws",
                    "[what are ]the 3 rules",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(what are )?the (three |3 )?(rules|laws)( help)$"),
                    new Regex("(?i)^(what are )?the (three |3 )?(rules|laws)$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)^(what are )?the (three |3 )?(rules|laws)$"),
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
                    foreach (var rule in Rules)
                    {
                        jabbrClient.PrivateReply(message.From, rule);
                    }
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                foreach (var rule in Rules)
                {
                    jabbrClient.SayToRoom(message.Room, rule);
                }
            }
        }
    }
}
