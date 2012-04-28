using System;

namespace Jabbot.Web.Models.Jabbot
{
    public class JabbotViewModel
    {
        public static JabbotViewModel Default { get { return new JabbotViewModel("0.0.0.0"); } }

        public string Version { get; private set; }

        public JabbotViewModel(string version)
        {
            Version = string.IsNullOrWhiteSpace(version) ? "0.0.0.0" : version;
        }
    }
}