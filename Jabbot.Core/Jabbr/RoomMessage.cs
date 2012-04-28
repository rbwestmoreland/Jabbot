using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbot.Core.Jabbr
{
    public class RoomMessage : IRoomMessage
    {
        public string Room { get; private set; }
        public string From { get; private set; }
        public string Content { get; private set; }

        public RoomMessage(string room, string from, string content)
        {
            Room = room;
            From = from;
            Content = content;
        }
    }
}
