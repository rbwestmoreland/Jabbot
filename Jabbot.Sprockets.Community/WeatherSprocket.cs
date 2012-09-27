using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Jabbot.Core.Jabbr;
using Jabbot.Core.Sprockets;
using Newtonsoft.Json;

namespace Jabbot.Sprockets.Community
{
    /// <summary>
    /// Port of the Hubot weather.coffee script
    /// </summary>
    public class WeatherSprocket : RegexSprocket
    {
        public override string Name { get { return "Weather Sprocket"; } }

        public override string Description { get { return "Weather conditions and forecasts."; } }

        public override IEnumerable<string> Usage 
        { 
            get 
            {
                return new string[]
                {
                    "/msg <botnick> weather help",
                    "/msg <botnick> weather [me|in|at|for] <city,state>",
                    "weather [me|in|at|for] <city,state>",
                };
            } 
        }

        protected override IEnumerable<Regex> PrivateMessagePatterns
        {
            get 
            { 
                return new Regex[] 
                { 
                    new Regex("(?i)^(weather help)$"),
                    new Regex("(?i)^(weather )(me |at |for |in )?(.+)$"),
                }; 
            }
        }

        protected override IEnumerable<Regex> RoomMessagePatterns
        {
            get
            {
                return new Regex[] 
                { 
                    new Regex("(?i)^(weather )(me |at |for |in )?(.+)$"),
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
                    var location = PrivateMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content).Groups[3].Value;
                    var formattedInformation = GetWeather(location);
                    jabbrClient.PrivateReply(message.From, formattedInformation);
                }
            }
        }

        public override void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            base.Handle(message, jabbrClient);

            if (this.CanHandle(message))
            {
                var location = RoomMessagePatterns.First(p => p.Match(message.Content).Success).Match(message.Content).Groups[3].Value;
                var formattedInformation = GetWeather(location);
                jabbrClient.SayToRoom(message.Room, formattedInformation);
            }
        }

        private string GetWeather(string location)
        {
            const string url = "http://openweathermap.org/data/2.1/find/name?q={0}";
            var formattedInformation = string.Empty;

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
                var result = client.GetAsync(String.Format(url, Uri.EscapeDataString(location))).Result;
                var content = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    formattedInformation = "Sorry, weather is unavaliable at the moment.";
                }
                else
                {
                    dynamic json = JsonConvert.DeserializeObject(content);
                    var city = json.list[0];
                    var name = (string)city.name;
                    var temp_current = (double)city.main.temp;
                    var temp_min = (double)city.main.temp_min;
                    var temp_max = (double)city.main.temp_max;
                    var humidity = (double)city.main.humidity;
                    var date = (DateTime)city.date;
                    var wind_speed = (double)city.wind.speed;
                    var wind_degrees = (double)city.wind.deg;
                    var description = (string)city.weather[0].description;

                    var temp_current_C = temp_current - 273.15;
                    var temp_current_F = 9.0 / 5.0 * temp_current_C + 32;

                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(name);
                    stringBuilder.AppendLine("---");
                    stringBuilder.AppendLine(description);
                    stringBuilder.AppendLine(string.Format("{0:F0}°F {1:F0}°C", temp_current_F, temp_current_C));
                    stringBuilder.AppendLine(string.Format("{0:F0}% humidity", humidity));

                    formattedInformation = stringBuilder.ToString();
                }
            }
            catch
            {
                formattedInformation = "Sorry, weather is unavaliable at the moment.";
            }

            return formattedInformation;
        }
    }
}
