using System;
using System.Collections.Generic;
using SignalR.Client.Hubs;

namespace Jabbot.Core.Jabbr
{
    public interface IJabbrClient
    {
        Action<string, string, string> OnReceivePrivateMessage { get; set; }
        Action<dynamic, string> OnReceiveRoomMessage { get; set; }

        void JoinRoom(string room);

        void JoinRoom(string room, string inviteCode);

        void LeaveRoom(string room);

        bool Login(string nick, string password);

        bool Login(string nick, string password, string gravatarEmail);

        void Logout();

        void PrivateReply(string who, string what);

        void SayToRoom(string room, string what);

        void Send(string command);
    }
}
