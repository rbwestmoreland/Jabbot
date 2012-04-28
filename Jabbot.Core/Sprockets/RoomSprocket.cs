using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Extensions;
using Jabbot.Core.Jabbr;

namespace Jabbot.Core.Sprockets
{
    public class RoomSprocket : RegexSprocket
    {
        private static string[] Greetings = new string[]
        {
            "Whassup!?",
            "Hey",
            "Hello",
            "I'm here",
            "Hey everyone!",
            "At your service.",
            "You rang?",
            "Bow before your robot overload!",
        };

        private static string[] Farewells = new string[]
        {
            "Bye!",
            "Goodbye!",
            "I'll be back",
            "Farewell",
        };

        public override string Name { get { return "Room Sprocket"; } }

        public override string Description { get { return "Jabbot room commands."; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> room help",
                    "/msg <botnick> join room <room> [invitecode]",
                    "/msg <botnick> leave room <room>",
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

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)^(room help)$"),
                    new Regex(@"(?i)(?<=^join room )(\S*)(\s+)?(\S+)?$"),
                    new Regex(@"(?i)^(leave room )(\S*)$"),
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
                    var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                    var room = match.Groups[1].Value;
                    var invitecode = match.Groups[3].Value;

                    if (string.IsNullOrWhiteSpace(invitecode))
                    {
                        jabbrClient.JoinRoom(room);
                    }
                    else
                    {
                        jabbrClient.JoinRoom(room, invitecode);
                    }

                    jabbrClient.SayToRoom(room, Greetings.RandomElement());
                }
                else if (PrivateMessagePatterns.ElementAt(2).Match(message.Content).Success)
                {
                    var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                    var room = match.Groups[2].Value;
                    jabbrClient.SayToRoom(room, Farewells.RandomElement());
                    jabbrClient.LeaveRoom(room);
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);
        }
    }
}
