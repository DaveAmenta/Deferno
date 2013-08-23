using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Deferno
{
    class Channel
    {
        public delegate void MessageHandler(User u, string message);
        public delegate void MessageHandler2(User u, string message, bool loudly);
        public delegate void KickHandler(User kicker, User kicked, string msg);

        public event MessageHandler OnMessage;
        public event MessageHandler OnTopicSet;
        public event MessageHandler OnTopicDetails;
        public event MessageHandler OnModesChanged;
        public event MessageHandler2 OnUserJoined;
        public event MessageHandler OnUserParted;
        public event KickHandler OnUserKicked;
        public event MessageHandler OnQuit;



        public List<User> Users = new List<User>();

        public string Name
        {
            get
            {
                return channel.Name;
            }
        }

        public string MyNick
        {
            get
            {
                return channel.Server.Nick;
            }
        }

        public bool IsChannel
        {
            get
            {
                return channel.IsChannel;
            }
        }

        public string Modes
        {
            get
            {
                string m = "+";
                foreach (string s in channel.Modes)
                {
                    m += s;
                }
                return m;
            }
        }

        IRCLib.Channel channel = null;
        public Channel(IRCLib.Channel c)
        {
            channel = c;

            c.OnMessage += new IRCLib.Channel.MessageHandler(c_OnMessage);
            c.TopicSet += new IRCLib.Channel.MessageHandler(c_TopicSet);
            c.OnTopicChanged += new IRCLib.Channel.TopicChanged(c_OnTopicChanged);
            c.TopicDetailsReply += new IRCLib.Channel.MessageHandler(c_TopicDetailsReply);
            c.OnModeChanged += new IRCLib.Channel.MessageHandler(c_OnModeChanged);
            c.OnUserJoined += new IRCLib.Channel.UserAction(c_OnUserJoined);
            c.OnUserKicked += new IRCLib.Channel.KickHandler(c_OnUserKicked);
            c.OnUserParted += new IRCLib.Channel.UserAction(c_OnUserParted);
            c.Server.OnUserQuit += new IRCLib.Server.UserQuitHandler(Server_OnUserQuit);
            Trace.WriteLine("Channel Handlers attached: " + c.Name);
        }

        void Server_OnUserQuit(IRCLib.Server s, IRCLib.User u, string msg)
        {
            
            if (OnQuit != null)
            {
                User ux = new User(u);
                OnQuit(ux, msg);
            }
        }

        void c_OnUserParted(IRCLib.Channel c, IRCLib.User u, string msg, bool loudly)
        {
            if (OnUserParted != null)
            {
                User ux = new User(u);
                if (loudly)
                {
                    OnUserParted(ux, msg);
                }
                else
                {
                    foreach (User ud in Users.ToArray())
                    {
                        if (ud.Nick == u.Nick)
                        {
                            Users.Remove(ud);
                            break;
                        }
                    }
                    Trace.WriteLine("Couldn't remove user: " + u);
                }
            }
        }

        void c_OnUserKicked(IRCLib.Channel c, IRCLib.User kicked, IRCLib.User kicker, string msg)
        {
            if (OnUserKicked != null)
            {
                User ukicked = new User(kicked);
                User uKicker = new User(kicker);
                OnUserKicked(uKicker, ukicked, msg);
            }
        }

        void c_OnUserJoined(IRCLib.Channel c, IRCLib.User u, string msg, bool loudly)
        {
            Trace.WriteLine("User Joined Channel: " + u);
            if (OnUserJoined != null)
            {
                User ux = new User(u);
                OnUserJoined(ux, msg, loudly);
                Users.Add(ux);

            }
            else
            {
                Trace.WriteLine("Dropped Message OnUserJOined: " + msg);
            }
        }

        void c_OnModeChanged(IRCLib.User u, string msg)
        {
            User ux = new User(u);
            if (OnModesChanged != null)
            {
                OnModesChanged(ux, msg);
            }
        }

        void c_TopicDetailsReply(IRCLib.User u, string msg)
        {
            User ux = new User(u);
            if (OnTopicDetails != null)
            {
                OnTopicDetails(ux, msg);
            }
        }

        void c_OnTopicChanged(IRCLib.Channel c, string topic)
        {
            Trace.WriteLine("Deferno/Channel/topicset2: " + topic);
            if (OnTopicSet != null)
            {
                OnTopicSet(null, topic);
            }
        }

        public void Send(string msg)
        {
            channel.SendMessage(msg);
        }

        void c_TopicSet(IRCLib.User u, string msg)
        {
            User ux = new User(u);
            if (OnTopicSet != null)
            {
                OnTopicSet(ux, msg);
            }
        }

        void c_OnMessage(IRCLib.User u, string msg)
        {
            User ux = new User(u);
            if (OnMessage != null)
            {
                OnMessage(ux, msg);
            }
        }
    }
}
