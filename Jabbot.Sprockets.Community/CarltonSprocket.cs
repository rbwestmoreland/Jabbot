using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Extensions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot carlton.coffee script
    /// </summary>
    public class CarltonSprocket : RegexSprocket
    {
        private static string[] Carltons = new string[] {
            "http://media.tumblr.com/tumblr_lrzrlymUZA1qbliwr.gif",
            "http://3deadmonkeys.com/gallery3/var/albums/random_stuff/Carlton-Dance-GIF.gif"
        };

        public override string Name { get { return "Carlton Celebration Sprocket"; } }

        public override string Description { get { return "Display a dancing Carlton"; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> carlton help",
                    "carlton",
                };
            }
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex(@"(?i)^(carlton help)$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex(@"(?i)(carlton)"),
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
                jabbrClient.SayToRoom(message.Room, Carltons.RandomElement());
            }
        }
    }
}
