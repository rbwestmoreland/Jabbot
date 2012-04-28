using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot ascii.coffee script
    /// </summary>
    public class AsciiSprocket : RegexSprocket
    {
        private const string Url = "http://asciime.heroku.com/generate_ascii?s={0}";

        public override string Name { get { return "Ascii Sprocket"; } }

        public override string Description { get { return "ASCII art."; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> ascii help",
                    "ascii <text>",
                    "ascii me <text>",
                };
            }
        }
        
        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex(@"(?i)^(ascii help)$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                {
                    new Regex(@"(?i)^(ascii( me)?) (.+)$"),
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
                var text = Uri.EscapeUriString(match.Groups[3].Value);

                var client = new HttpClient();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
                client.GetAsync(String.Format(Url, text)).ContinueWith(task =>
                {
                    if (task.Result.IsSuccessStatusCode)
                    {
                        task.Result.Content.ReadAsStringAsync().ContinueWith(readTask =>
                        {
                            jabbrClient.SayToRoom(message.Room, readTask.Result);
                        });
                    }
                    else
                    {
                        jabbrClient.PrivateReply(message.From, "Error ascii'ing that for you.");
                    }
                });
            }
        }
    }
}
