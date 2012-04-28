using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;

namespace Jabbot.Core.Sprockets
{
    public class HelpSprocket : RegexSprocket
    {
        public override string Name { get { return "Help Sprocket"; } }

        public override string Description { get { return "List all sprockets."; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> help",
                };
            }
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)^(help)$"),
                };
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] { };
            }
        }

        public override void Handle(IPrivateMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                if (PrivateMessagePatterns.First().Match(message.Content).Success)
                {
                    foreach (var sprocket in Container.Sprockets)
                    {
                        jabbrClient.PrivateReply(message.From, sprocket.GetFormattedHelp());
                    }
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);
        }
    }
}
