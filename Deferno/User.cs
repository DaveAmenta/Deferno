using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deferno
{
    class User
    {
        public System.Windows.Controls.TreeViewItem TreeItem { get; set; }
        IRCLib.User u = null;
        public User(IRCLib.User user)
        {
            u = user;
            u.OnQuit += new IRCLib.User.QuitHandler(u_OnQuit);
            u.OnModeChanged += new IRCLib.User.ModeChanged(u_OnModeChanged);
            u.OnNickChanged += new IRCLib.User.NickChanged(u_OnNickChanged);
            
        }

        void u_OnNickChanged(string old, string newNick)
        {
            
        }

        void u_OnModeChanged(IRCLib.User u)
        {
            throw new NotImplementedException();
        }

        void u_OnQuit(IRCLib.User u, string msg)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return u.DisplayMode + u.Nick;
        }

        public string Nick
        {
            get
            {
                return u.Nick;
            }
        }

        public string Host
        {
            get
            {
                return u.Host;
            }
        }

        public string UserName
        {
            get
            {
                return u.UserName;
            }
        }
    }
}
