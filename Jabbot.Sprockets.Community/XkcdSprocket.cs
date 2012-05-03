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

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot xkcd.coffee script
    /// </summary>
    public class XkcdSprocket : RegexSprocket
    {
        public override string Name { get { return "XKCD Sprocket"; } }

        public override string Description { get { return "Grab XKCD comic image urls"; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> xkcd help",
                    "/msg <botnick> xkcd [number]",
                    "xkcd [number]",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(xkcd help)$"),
                    new Regex(@"(?i)^(xkcd)( \d+)?$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex(@"(?i)^(xkcd)( \d+)?$"),
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
                    var id = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content).Groups[2].Value;
                    var url = string.IsNullOrWhiteSpace(id) ? GetRandomUrl() : GetComicUrl(id);
                    jabbrClient.PrivateReply(message.From, url);
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var id = RoomMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content).Groups[2].Value;
                var url = string.IsNullOrWhiteSpace(id) ? GetRandomUrl() : GetComicUrl(id);
                jabbrClient.SayToRoom(message.Room, url);
            }
        }

        private string GetRandomUrl()
        {
            return GetComicUrl(string.Empty);
        }

        private string GetComicUrl(string id)
        {
            const string url = "http://xkcd.com/{0}/info.0.json";
            var comicUrl = string.Empty;

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
                var result = client.GetAsync(String.Format(url, id.Trim())).Result;
                var content = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    dynamic json = JsonConvert.DeserializeObject(content);
                    stringBuilder.AppendLine(json.title.Value);
                    stringBuilder.AppendLine(json.alt.Value);
                    stringBuilder.AppendLine(json.img.Value);
                    comicUrl = stringBuilder.ToString();
                }
                else
                {
                    comicUrl = "Comic not found.";
                }
            }
            catch
            {
                comicUrl = "Comic not found.";
            }

            return comicUrl;
        }
    }
}
