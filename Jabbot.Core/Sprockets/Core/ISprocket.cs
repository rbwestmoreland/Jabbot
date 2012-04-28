using System.Collections.Generic;
using System.ComponentModel.Composition;
using Jabbot.Core.Jabbr;

namespace Jabbot.Core.Sprockets
{
    [InheritedExport]
    public interface ISprocket
    {
        string Name { get; }

        string Description { get; }

        IEnumerable<string> Usage { get; }

        bool CanHandle(IPrivateMessage message);

        bool CanHandle(IRoomMessage message);

        void Handle(IPrivateMessage message, IJabbrClient jabbrClient);

        void Handle(IRoomMessage message, IJabbrClient jabbrClient);
    }
}
