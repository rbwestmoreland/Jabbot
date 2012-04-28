using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbot.Core.Jabbr
{
    public interface IPrivateMessage
    {
        string From { get; }
        string Content { get; }
    }
}
