using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DavuxLib;

namespace Deferno
{
    class Server
    {
        public delegate void ServerHandler(Server s);
        public delegate void StatusHandler(string message);
        public delegate void ChannelHandler(Channel c);


        public static event ServerHandler OnConnecting;


        public event StatusHandler OnStatusMessage;
        public event StatusHandler OnSendingMessage;
        public event ChannelHandler OnJoinedChannel;

        private IRCLib.Server s = null;
        List<Channel> Channels = new List<Channel>();

        public string HostName { get; set; }
        public int Port { get; set; }

        public Server(string Server, int port, string nick, string user)
        {
            HostName = Server;
            Port = port;
            IRCLib.Profile p = new IRCLib.Profile();
            p.Nick = nick;
            p.User = user;
            p.RealName = Settings.Get("RealName", "Deferno IRC");
            p.QuitMessage = Settings.Get("QuitMessage", "Deferno IRC: http://www.daveamenta.com");
            s = new IRCLib.Server(Server, port);
            s.Profile = p;
            if (OnConnecting != null)
            {
                OnConnecting(this);
            }

            s.OnConnected += new IRCLib.Server.ServerHandler(s_OnConnected);
            s.OnDisconnected += new IRCLib.Server.ServerHandler(s_OnDisconnected);
            s.OnError += new IRCLib.Server.ServerErrorHandler(s_OnError);
            s.OnJoined += new IRCLib.Server.ChannelHandler(s_OnJoined);
            s.OnKicked += new IRCLib.Server.ChannelHandler(s_OnKicked);
            s.OnMsg += new IRCLib.Server.MessageHandler(s_OnMsg);
            s.OnNetworkChanged += new IRCLib.Server.NetworkChangedHandler(s_OnNetworkChanged);
            s.OnNickChanged += new IRCLib.Server.NickChangedHandler(s_OnNickChanged);
            s.OnNotice += new IRCLib.Server.MessageHandler(s_OnNotice);
            s.OnParted += new IRCLib.Server.ChannelHandler(s_OnParted);
            s.OnStatusMessage += new IRCLib.Server.MessageHandler(s_OnStatusMessage);

            s.Connect();
        }

        public string MyNick
        {
            get
            {
                return s.Profile.Nick;
            }
        }

        public void Send(string msg)
        {
            if (OnSendingMessage != null)
            {
                OnSendingMessage(msg);
            }
            s.Send(msg);
        }

        void s_OnUserQuit(IRCLib.Server s, IRCLib.User u, string msg)
        {
            
        }

        void s_OnStatusMessage(IRCLib.Server s, IRCLib.Message m)
        {
            if (OnStatusMessage != null)
            {
                if (m.Command == "372")
                {
                    OnStatusMessage(m.ListString);
                }
                else
                {
                    OnStatusMessage(m.AfterCommandString);
                }
            }
        }

        void s_OnParted(IRCLib.Server s, IRCLib.Channel c)
        {
            
        }

        void s_OnNotice(IRCLib.Server s, IRCLib.Message m)
        {
            
        }

        void s_OnNickChanged(string oldNIck, string newNick)
        {
            
        }

        void s_OnNetworkChanged(string newNetworkName)
        {
            
        }

        void s_OnMsg(IRCLib.Server s, IRCLib.Message m)
        {
            
        }

        void s_OnKicked(IRCLib.Server s, IRCLib.Channel c)
        {
            
        }

        void s_OnJoined(IRCLib.Server s, IRCLib.Channel c)
        {
            Channel ch = new Channel(c);
            if (OnJoinedChannel != null)
            {
                OnJoinedChannel(ch);
            }
        }

        void s_OnError(IRCLib.Server s, Exception e)
        {
            
            
        }

        void s_OnDisconnected(IRCLib.Server s)
        {
            
        }

        void s_OnConnected(IRCLib.Server s)
        {
            
        }
    }
}
