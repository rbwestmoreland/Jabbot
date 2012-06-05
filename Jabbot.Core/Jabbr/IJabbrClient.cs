using System;
using System.Collections.Generic;
using SignalR.Client.Hubs;

namespace Jabbot.Core.Jabbr
{
    public interface IJabbrClient : IDisposable
    {
        bool IsConnected { get; }
        Action<string, string, string> OnReceivePrivateMessage { get; set; }
        Action<dynamic, string> OnReceiveRoomMessage { get; set; }

        bool JoinRoom(string room);

        bool JoinRoom(string room, string inviteCode);

        bool LeaveRoom(string room);

        bool Login(string nick, string password);

        bool Login(string nick, string password, string gravatarEmail);

        void Logout();

        bool Connect();

        void Disconnect();

        bool PrivateReply(string who, string what);

        bool SayToRoom(string room, string what);

        void Send(string command);
    }
}
