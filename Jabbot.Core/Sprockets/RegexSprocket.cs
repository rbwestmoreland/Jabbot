using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core.Jabbr;

namespace Jabbot.Core.Sprockets
{
    [PartNotDiscoverableAttribute]
    public abstract class RegexSprocket : BaseSprocket
    {
        protected virtual IEnumerable<Regex> PrivateMessagePatterns 
        {
            get { return new Regex[] { }; }
        }

        protected virtual IEnumerable<Regex> RoomMessagePatterns
        {
            get { return new Regex[] { }; }
        }

        public override bool CanHandle(IPrivateMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            return PrivateMessagePatterns.Any(p => p.Match(message.Content).Success);
        }

        public override bool CanHandle(IRoomMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            return RoomMessagePatterns.Any(p => p.Match(message.Content).Success);
        }
    }
}
