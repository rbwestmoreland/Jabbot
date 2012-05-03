using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Morning Brew Sprocket
    /// </summary>
    public class MorningBrewSprocket : RegexSprocket
    {
        public override string Name { get { return "Morning Brew Sprocket"; } }

        public override string Description { get { return "Get the latest from Chris Alcock's Morning Brew at http://blog.cwa.me.uk."; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> morningbrew help",
                    "/msg <botnick> morningbrew",
                    "morningbrew",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(morningbrew help)$"),
                    new Regex(@"(?i)^(morningbrew)( me)?$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex(@"(?i)^(morningbrew)( me)?$"),
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
                    var formattedInformation = GetFormattedInformation();
                    jabbrClient.PrivateReply(message.From, formattedInformation);
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var formattedInformation = GetFormattedInformation();
                jabbrClient.SayToRoom(message.Room, formattedInformation);
            }
        }

        private string GetFormattedInformation()
        {
            var formattedInformation = string.Empty;

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
                var result = client.GetAsync("http://feeds.feedburner.com/ReflectivePerspective?format=xml").Result;
                var content = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    var stringBuilder = new StringBuilder();

                    var xml = XDocument.Parse(content);
                    var item = xml.Root.Descendants("item").First();
                    var title = item.Element("title").Value;
                    var html = item.Elements().ElementAt(10).Value; //How do you do this: "item.Element("content:encoded").Value"?
                    var formattedContent = FormatHtml(html);

                    stringBuilder.AppendLine(title);
                    stringBuilder.AppendLine(formattedContent);

                    formattedInformation = stringBuilder.ToString();
                }
                else
                {
                    formattedInformation = "Sorry, I was unable to fetch the morning brew.";
                }
            }
            catch
            {
                formattedInformation = "Sorry, I encountered an error while fetching the morning brew.";
            }

            return formattedInformation;
        }

        private static string FormatHtml(string html)
        {
            var workingString = html;

            workingString = workingString.Replace("\n", string.Empty);
            workingString = workingString.Replace("<h3>", string.Format("{0}###", Environment.NewLine));
            workingString = workingString.Replace("</h3>", Environment.NewLine);
            workingString = workingString.Replace("<ul>", string.Empty);
            workingString = workingString.Replace("</ul>", string.Empty);
            workingString = workingString.Replace("<li>", string.Format("{0}* ", Environment.NewLine));
            workingString = workingString.Replace("</li>", Environment.NewLine);

            var regEx = new Regex(@"<a.+?href=\""(.+?)\"".*?>(.+?)</a>");
            var matches = regEx.Matches(workingString);
            foreach (Match match in matches)
            {
                var tag = match.Value;
                var text = match.Groups[2];
                var link = match.Groups[1];
                workingString = workingString.Replace(tag, string.Format("[{0}]({1})", text, link));
            }

            return workingString;
        }
    }
}
