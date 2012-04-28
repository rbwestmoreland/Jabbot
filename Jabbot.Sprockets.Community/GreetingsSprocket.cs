using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    public class GreetingsSprocket : RegexSprocket
    {
        public override string Name { get { return "Greetings Sprocket"; } }

        public override string Description { get { return "Replies to greetings."; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> hi|hey|hello|morning|evening|afternoon|evening|bye|goodnite|goodnight|night help",
                    "/msg <botnick> hi",
                    "/msg <botnick> hey",
                    "/msg <botnick> hello",
                    "/msg <botnick> morning",
                    "/msg <botnick> afternoon",
                    "/msg <botnick> evening",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(hi|hey|hello|morning|afternoon|evening|bye|goodnite|goodnight|night)( help)$"),
                    new Regex("(?i)(^hi)(.*)"),
                    new Regex("(?i)(^hey)(.*)"),
                    new Regex("(?i)(^hello)(.*)"),
                    new Regex("(?i)(^morning)(.*)"),
                    new Regex("(?i)(^afternoon)(.*)"),
                    new Regex("(?i)(^evening)(.*)"),
                    new Regex("(?i)(^bye)(.*)"),
                    new Regex("(?i)(^goodnite)(.*)"),
                    new Regex("(?i)(^goodnight)(.*)"),
                    new Regex("(?i)(^night)(.*)"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)(^hi)(.*)"),
                    new Regex("(?i)(^hey)(.*)"),
                    new Regex("(?i)(^hello)(.*)"),
                    new Regex("(?i)(^morning)(.*)"),
                    new Regex("(?i)(^afternoon)(.*)"),
                    new Regex("(?i)(^evening)(.*)"),
                    new Regex("(?i)(^bye)(.*)"),
                    new Regex("(?i)(^goodnite)(.*)"),
                    new Regex("(?i)(^goodnight)(.*)"),
                    new Regex("(?i)(^night)(.*)"),
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
                    jabbrClient.PrivateReply(message.From, String.Format("{0}", match.Groups[1].Value));
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                jabbrClient.SayToRoom(message.Room, String.Format("{0}", match.Groups[1].Value));
            }
        }
    }
}
