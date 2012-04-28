using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;
using Newtonsoft.Json;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot math.coffee script
    /// </summary>
    public class MathSprocket : RegexSprocket
    {
        public override string Name { get { return "Math Sprocket"; } }

        public override string Description { get { return "Allows Jabbot to do mathematics."; } }

        public override IEnumerable<string> Usage
        {
            get
            {
                return new string[]
                {
                    "/msg <botnick> calc|calculate|math|convert help",
                    "/msg <botnick> calc <expression>",
                    "/msg <botnick> calculate <expression>",
                    "/msg <botnick> math <expression>",
                    "/msg <botnick> convert <expression>",
                    "calc <expression>",
                    "calculate <expression>",
                    "math <expression>",
                    "convert <expression>",
                };
            }
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(calc|calculate|convert|math)( help)$"),
                    new Regex("(?i)^(calc|calculate|convert|math)( me)? (.*)") 
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)^(calc|calculate|convert|math)( me)? (.*)") 
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
                    var url = string.Format("http://www.google.com/ig/calculator?hl=en&q={0}", Uri.EscapeDataString(match.Groups[3].Value));

                    var client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 0, 2);
                    client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));
                    client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
                    var request = client.GetAsync(url).ContinueWith(requestTask =>
                          {
                              if (requestTask.Result.IsSuccessStatusCode)
                              {
                                  requestTask.Result.Content.ReadAsStringAsync().ContinueWith(readTask =>
                                  {
                                      dynamic json = JsonConvert.DeserializeObject(readTask.Result);
                                      string solution = json.rhs;
                                      jabbrClient.PrivateReply(message.From, solution ?? "Does not compute.");
                                  });
                              }
                              else
                              {
                                  jabbrClient.PrivateReply(message.From, "Does not compute.");
                              }
                          });
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var match = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content);
                var url = string.Format("http://www.google.com/ig/calculator?hl=en&q={0}", Uri.EscapeDataString(match.Groups[3].Value));

                var client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 2);
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
                var request = client.GetAsync(url).ContinueWith(requestTask =>
                {
                    if (requestTask.Result.IsSuccessStatusCode)
                    {
                        requestTask.Result.Content.ReadAsStringAsync().ContinueWith(readTask =>
                        {
                            dynamic json = JsonConvert.DeserializeObject(readTask.Result);
                            string solution = json.rhs;
                            jabbrClient.SayToRoom(message.Room, solution ?? "Does not compute.");
                        });
                    }
                    else
                    {
                        jabbrClient.SayToRoom(message.Room, "Does not compute.");
                    }
                });
            }
        }
    }
}
