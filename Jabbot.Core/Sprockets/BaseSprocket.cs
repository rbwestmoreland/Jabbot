using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Jabbot.Core.Jabbr;

namespace Jabbot.Core.Sprockets
{
    [PartNotDiscoverableAttribute]
    public abstract class BaseSprocket : ISprocket
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract IEnumerable<string> Usage { get; }

        public virtual bool CanHandle(IPrivateMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            return false;
        }

        public virtual bool CanHandle(IRoomMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            return false;
        }

        public virtual void Handle(IPrivateMessage message, IJabbrClient jabbrClient)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (jabbrClient == null)
            {
                throw new ArgumentNullException("jabbrClient");
            }
        }

        public virtual void Handle(IRoomMessage message, IJabbrClient jabbrClient)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (jabbrClient == null)
            {
                throw new ArgumentNullException("jabbrClient");
            }
        }
    }
}
