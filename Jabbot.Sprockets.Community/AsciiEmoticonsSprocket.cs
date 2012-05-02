using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Extensions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    public class AsciiEmoticonsSprocket : RegexSprocket
    {
        public override string Name { get { return "Ascii Emoticons Sprocket"; } }

        public override string Description { get { return "Because I don't have those keys on my keyboard."; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> asciiemoticons help",
                    ":fliptable:",
                    ":unfliptable:|:puttableback:|:tableback:",
                    ":awyea:",
                    ":yuno:",
                    ":dance:|:dancing:",
                    ":stare:|:staring:|:disapproval:",
                    ":shrug:|:shrugging:",
                };
            }
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex(@"(?i)^(asciiemoticons help)$"),
                };
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                {
                    new Regex(@"(?i):(fliptable):"),
                    new Regex(@"(?i):(un-?fliptable)|((put)?(tableback)):"),
                    new Regex(@"(?i):(a+?w+?yea):"),
                    new Regex(@"(?i):(yuno):"),
                    new Regex(@"(?i):(dance|dancing):"),
                    new Regex(@"(?i):(stare|staring|disapprov(e|al)):"),
                    new Regex(@"(?i):(shrug|shrugging):"),
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
                if (RoomMessagePatterns.ElementAt(0).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, new string[] { "(ノಠ益ಠ)ノ︵┻━┻", "（╯°□°）╯︵ ┻━┻" }.RandomElement());
                }
                else if (RoomMessagePatterns.ElementAt(1).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, "┬──┬◡ﾉ(° -°ﾉ)");
                }
                else if (RoomMessagePatterns.ElementAt(2).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, "(ケ¬◟¬)ケ");
                }
                else if (RoomMessagePatterns.ElementAt(3).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, "ლ(ಠ益ಠლ)");
                }
                else if (RoomMessagePatterns.ElementAt(4).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, new string[] { "♪┏(・o･)┛♪", "♪┗ ( ･o･) ┓♪", "♪┏ (･o･) ┛♪", "♪┗ (･o･ ) ┓♪", "♪┏(･o･)┛♪" }.RandomElement());
                }
                else if (RoomMessagePatterns.ElementAt(5).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, "ಠ_ಠ");
                }
                else if (RoomMessagePatterns.ElementAt(6).IsMatch(message.Content))
                {
                    jabbrClient.SayToRoom(message.Room, @"¯\_(ツ)_/¯");
                }
            }
        }
    }
}
