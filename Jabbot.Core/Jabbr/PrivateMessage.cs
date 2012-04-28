using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbot.Core.Jabbr
{
    public class PrivateMessage : IPrivateMessage
    {
        public string From { get; private set; }
        public string Content { get; private set; }

        public PrivateMessage(string from, string content)
        {
            From = from;
            Content = content;
        }
    }
}
