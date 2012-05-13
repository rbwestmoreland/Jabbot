using System;
using System.Collections.Generic;
using SignalR.Client.Hubs;

namespace Jabbot.Core.Jabbr
{
    public interface IJabbrClient
    {
        Boolean IsConnected { get; }
        Action OnClosed { get; set; }
        Action<Exception> OnError { get; set; }
        Action<string, string, string> OnReceivePrivateMessage { get; set; }
        Action<dynamic, string> OnReceiveRoomMessage { get; set; }

        bool JoinRoom(string room);

        bool JoinRoom(string room, string inviteCode);

        bool LeaveRoom(string room);

        bool Login(string nick, string password);

        bool Login(string nick, string password, string gravatarEmail);

        void Logout();

        bool PrivateReply(string who, string what);

        bool SayToRoom(string room, string what);

        void Send(string command);
    }
}
