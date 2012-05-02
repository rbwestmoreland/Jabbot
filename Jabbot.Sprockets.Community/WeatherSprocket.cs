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
                    "/msg <botnick> weather <city or zip code>",
                    "/msg <botnick> weather me <city or zip code>",
                    "/msg <botnick> weather in <city or zip code>",
                    "/msg <botnick> weather at <city or zip code>",
                    "/msg <botnick> weather for <city or zip code>",
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
            const string url = "http://www.google.com/ig/api?weather={0}";
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
                    var xml = XDocument.Parse(content);

                    if (xml.Root.HasElements)
                    {
                        var stringBuilder = new StringBuilder();
                        var forecastInformation = xml.Root.Descendants("forecast_information");
                        var currentConditions = xml.Root.Descendants("current_conditions");
                        var forecastConditions = xml.Root.Descendants("forecast_conditions");

                        if (forecastInformation.Any())
                        {
                            var city = forecastInformation.Descendants("city").First().Attribute("data").Value;
                            stringBuilder.AppendLine(city);
                        }

                        if (currentConditions.Any())
                        {
                            var condition = currentConditions.Descendants("condition").First().Attribute("data").Value;
                            var ferinheight = currentConditions.Descendants("temp_f").First().Attribute("data").Value;
                            var celsius = currentConditions.Descendants("temp_c").First().Attribute("data").Value;
                            var humidity = currentConditions.Descendants("humidity").First().Attribute("data").Value;
                            var windCondition = currentConditions.Descendants("wind_condition").First().Attribute("data").Value;

                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine("Currently");
                            stringBuilder.AppendLine(string.Format("{0}°F {1}°C {2}", ferinheight, celsius, condition));
                            stringBuilder.AppendLine(humidity);
                            stringBuilder.AppendLine(windCondition);
                        }

                        if (forecastConditions.Any())
                        {
                            stringBuilder.AppendLine();
                            stringBuilder.AppendLine("Forecast");
                            foreach (var forecastCondition in forecastConditions)
                            {
                                var dayOfWeek = forecastCondition.Descendants("day_of_week").First().Attribute("data").Value;
                                var high = forecastCondition.Descendants("high").First().Attribute("data").Value;
                                var low = forecastCondition.Descendants("low").First().Attribute("data").Value;
                                var condition = forecastCondition.Descendants("condition").First().Attribute("data").Value;

                                stringBuilder.AppendLine(string.Format("{0}: {1}° to {2}° {3}", dayOfWeek, high, low, condition));
                            }
                        }

                        formattedInformation = stringBuilder.ToString();
                    }

                    if (string.IsNullOrWhiteSpace(formattedInformation))
                    {
                        formattedInformation = string.Format("Sorry, but I couldn't find {0}.", location);
                    }
                }
            }
            catch
            {
                formattedInformation = "Sorry, I was unable to parse the weather data.";
            }

            return formattedInformation;
        }
    }
}
