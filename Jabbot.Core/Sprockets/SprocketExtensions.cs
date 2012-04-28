using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbot.Core.Sprockets
{
    public static class SprocketExtensions
    {
        public static string GetFormattedHelp(this ISprocket sprocket)
        {
            var builder = new StringBuilder();
            
            builder.Append(string.Format("{0}{1}", sprocket.Name, Environment.NewLine));
            builder.Append(string.Format("{0}{1}", sprocket.Description, Environment.NewLine));
            if (sprocket.Usage != null)
            {
                foreach (var usage in sprocket.Usage)
                {
                    builder.Append(string.Format("{0}{1}", usage, Environment.NewLine));
                }
            }

            return builder.ToString();
        }
    }
}
