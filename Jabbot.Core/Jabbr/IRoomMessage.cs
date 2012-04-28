using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbot.Core.Jabbr
{
    public interface IRoomMessage
    {
        string Room { get; }
        string From { get; }
        string Content { get; }
    }
}
