using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jabbot.Web.Models.Status
{
    public class StatusViewModel
    {
        public static StatusViewModel Default { get { return new StatusViewModel(DateTimeOffset.MinValue); } }

        public DateTimeOffset LastSeen { get; private set; }

        public StatusViewModel(DateTimeOffset lastSeen)
        {
            LastSeen = lastSeen;
        }
    }
}