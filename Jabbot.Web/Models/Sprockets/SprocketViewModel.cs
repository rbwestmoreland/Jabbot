using System.Collections.Generic;

namespace Jabbot.Web.Models.Sprockets
{
    public class SprocketViewModel
    {
        public static SprocketViewModel Unknown { get { return new SprocketViewModel(string.Empty, string.Empty, new string[] { }); } }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IEnumerable<string> Usage { get; private set; }

        public SprocketViewModel(string name, string description, IEnumerable<string> usage)
        {
            Name = name;
            Description = description;
            Usage = usage ?? new List<string>();
        }
    }
}