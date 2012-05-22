using System;
using System.Collections.Generic;
using System.Threading;
using NLog;
using SignalR.Client.Hubs;

namespace Jabbot.Core.Jabbr
{
    public class JabbrClient : IJabbrClient
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private HubConnection Connection { get; set; }
        private IHubProxy Proxy { get; set; }
        private List<string> Rooms { get; set; }
        public virtual Boolean IsConnected { get { try { return Connection.IsActive; } catch { return false; } } }
        public virtual Action OnClosed { get; set; }
        public virtual Action<Exception> OnError { get; set; }
        public virtual Action<string, string, string> OnReceivePrivateMessage { get; set; }
        public virtual Action<dynamic, string> OnReceiveRoomMessage { get; set; }

        public JabbrClient(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url");
            }

            Rooms = new List<string>();
            Connection = new HubConnection(url);
            Proxy = Connection.CreateProxy("chat");
            SubscribeToEvents();
        }

        public virtual bool JoinRoom(string room)
        {
            var success = false;

            if (string.IsNullOrWhiteSpace(room))
            {
                throw new ArgumentNullException("room");
            }

            try
            {
                Send(String.Format("/join {0}", room));
                success = true;
            }
            catch(Exception ex)
            {
                Logger.ErrorException("An error occured while joining public room.", ex.GetBaseException());
            }

            return success;
        }

        public virtual bool JoinRoom(string room, string inviteCode)
        {
            var success = false;

            if (string.IsNullOrWhiteSpace(room))
            {
                throw new ArgumentNullException("room");
            }

            if (string.IsNullOrWhiteSpace(inviteCode))
            {
                throw new ArgumentNullException("inviteCode");
            }

            try
            {
                Send(String.Format("/join {0} {1}", room, inviteCode));
                success = true;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while joining private room.", ex.GetBaseException());
            }

            return success;
        }

        public virtual bool LeaveRoom(string room)
        {
            var success = false;

            if (string.IsNullOrWhiteSpace(room))
            {
                throw new ArgumentNullException("room");
            }

            try
            {
                Send(String.Format("/leave {0}", room));
                success = true;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while leaving room.", ex.GetBaseException());
            }

            return success;
        }

        public virtual bool Login(string nick, string password)
        {
            return Login(nick, password, null);
        }

        public virtual bool Login(string nick, string password, string gravatarEmail)
        {
            var success = false;

            if (string.IsNullOrWhiteSpace(nick))
            {
                throw new ArgumentNullException("nick");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password");
            }

            try
            {
                var cancellationToken = new CancellationToken();
                var timeout = (int)new TimeSpan(0, 0, 5).TotalMilliseconds;
                Connection.Start().Wait(timeout, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                Proxy.On("userCreated", () =>
                {
                });

                Proxy.On<IEnumerable<dynamic>>("logOn", rooms =>
                {
                    foreach (var item in rooms)
                    {
                        string name = item.Name.Value;
                        long count = item.Count.Value;
                        Rooms.Add(name);
                    }
                });

                success = !Proxy.Invoke<bool>("join").Result;

                if (success)
                {
                    Send(String.Format("/nick {0} {1}", nick, password));

                    if (!string.IsNullOrWhiteSpace(gravatarEmail))
                    {
                        Send(String.Format("/gravatar {0}", gravatarEmail));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while logging in.", ex);
            }

            return success;
        }

        public virtual void Logout()
        {
            try
            {
                Rooms.ForEach(r => LeaveRoom(r));
                Proxy = null;
                Connection.Stop();
                Connection = null;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while logging out.", ex);
            }
        }

        public virtual bool PrivateReply(string who, string what)
        {
            var success = false;

            if (string.IsNullOrWhiteSpace(who))
            {
                throw new ArgumentNullException("who");
            }

            if (string.IsNullOrWhiteSpace(what))
            {
                throw new ArgumentNullException("what");
            }

            try
            {
                Send(String.Format("/msg {0} {1}", who, what));
                success = true;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while sending a private message.", ex.GetBaseException());
            }

            return success;
        }

        public virtual bool SayToRoom(string room, string what)
        {
            var success = false;

            if (string.IsNullOrWhiteSpace(room))
            {
                throw new ArgumentNullException("room");
            }

            if (string.IsNullOrWhiteSpace(what))
            {
                throw new ArgumentNullException("what");
            }

            if (what.StartsWith("/"))
            {
                throw new InvalidOperationException("Commands are not allowed");
            }

            try
            {
                Proxy.Invoke("send", what, room).Wait();
                success = true;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while sending a room message.", ex.GetBaseException());
            }

            return success;
        }

        public virtual void Send(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentNullException("command");
            }

            try
            {
                Proxy.Invoke("send", command, null).Wait();
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while sending a command.", ex.GetBaseException());
                throw;
            }
        }

        protected virtual void SubscribeToEvents()
        {
            try
            {
                Proxy.On<string, string, string>("sendPrivateMessage", (string arg1, string arg2, string arg3) =>
                {
                    var action = OnReceivePrivateMessage;

                    if (action != null)
                    {
                        action(arg1, arg2, arg3);
                    }
                });

                Proxy.On<dynamic, string>("addMessage", (dynamic arg1, string arg2) =>
                {
                    var action = OnReceiveRoomMessage;

                    if (action != null)
                    {
                        action(arg1, arg2);
                    }
                });

                Proxy.On<dynamic>("joinRoom", room =>
                {
                    string name = room.Name.Value;
                    long count = room.Count.Value;
                    if (!Rooms.Contains(name.ToLower()))
                    {
                        Rooms.Add(name.ToLower());
                    }
                });

                Proxy.On<dynamic, string>("leave", (user, room) =>
                {
                    if (Rooms.Contains(room.ToLower()))
                    {
                        Rooms.Remove(room.ToLower());
                    }
                });

                Connection.Closed += () =>
                    {
                        var action = OnClosed;

                        if (action != null)
                        {
                            action.Invoke();
                        }
                    };

                Connection.Error += (Exception ex) =>
                    {
                        var action = OnError;

                        if (action != null)
                        {
                            action.Invoke(ex);
                        }
                    };
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An error occured while subscribing to events.", ex); ;
            }
        }
    }
}
