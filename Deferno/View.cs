using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using DavuxLib;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace Deferno
{
    interface IView
    {
        TreeViewItem TreeItem { get; }
        Cjc.ChromiumBrowser.WebBrowser web { get; }
        System.Windows.Controls.ListView UsersList { get; }
        
        void SendCommand(string command);
        void Select();
        void Deselect();
        string GlassName { get; }
        string TopicText { get; }
        string ModesBox { get; }
    }

    abstract class BaseView : IView
    {
        public TreeViewItem TreeItem { get; private set; }
        public Cjc.ChromiumBrowser.WebBrowser web { get; private set; }
        string _GlassName = "";
        public string GlassName
        {
            get
            {
                return _GlassName;
            }
            set
            {
                _GlassName = value;
                // change event
            }
        }

        public string TopicText { get; set; }
        // w/ change event also

        public string ModesBox { get; set; }

        protected Image TreeImage = null;
        protected TextBlock TreeText = null;

        protected Theme Theme = null;
        bool LoadedDefaultTheme = false;
        protected Window window = null;
        protected bool Selected = false;

        void ScrollViewToBottom()
        {
            string js = "window.scrollBy(0,500);";
            web.ExecuteJavascript(js, "");
        }


        public System.Windows.Controls.ListView UsersList
        {
            get;
            set;
        }

        public enum WindowState
        {
            Clear = 1, Unseen = 2, UnseenMessages = 3, Notify = 4,
        }

        protected void SetMaxState(WindowState s)
        {
            if (!Selected)
            {
                if ((int)s > (int)State)
                {
                    State = s;
                }
            }
        }

        WindowState _State = WindowState.Clear;
        protected WindowState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
                // set the treeitem state

                switch (_State)
                {
                    case WindowState.Clear:
                        FlashWindow.Stop(window);
                        TreeItem.Background = Brushes.White;
                        //TreeImage.Source = new BitmapImage(new Uri("images/notify.png", UriKind.RelativeOrAbsolute)); ;
                        break;
                    case WindowState.Notify:
                        if (Settings.Get("FlashOnNotify", true))
                        {
                            FlashWindow.Start(window);
                        }
                        TreeItem.Background = Brushes.Yellow;
                        //TreeImage.Source = new BitmapImage(new Uri("images/notify.png", UriKind.RelativeOrAbsolute));
                        break;
                    case WindowState.UnseenMessages:
                        if (Settings.Get("FlashOnUnseenMessages", true))
                        {
                            FlashWindow.Start(window);
                        }
                        TreeItem.Background = Brushes.LightBlue;
                        //TreeImage.Source = new BitmapImage(new Uri("images/unseenmessages.png", UriKind.RelativeOrAbsolute));
                        break;
                    case WindowState.Unseen:
                        if (Settings.Get("FlashOnUnseen", false))
                        {
                            FlashWindow.Start(window);
                        }
                        TreeItem.Background = Brushes.LightGreen;
                        //TreeImage.Source = new BitmapImage(new Uri("images/unseen.png", UriKind.RelativeOrAbsolute));
                        break;
                }
            }
        }

        public void Select()
        {
            Selected = true;
            State = WindowState.Clear;
        }

        public void Deselect()
        {
            Selected = false;
        }

        public BaseView(Window window)
        {
            this.window = window;
            TreeItem = new TreeViewItem();
            UsersList = new System.Windows.Controls.ListView();
            UsersList.BorderThickness = new Thickness(0);
            //UsersList.Background = Brushes.Blue;
            TreeItem.Tag = this;
            TreeItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
            TreeText = new TextBlock();
            TreeText.Margin = new Thickness(5);
           // TreeImage = new Image();
            //TreeImage.Height = 16;
            //TreeImage.Width = 16;
            //TreeImage.Visibility = Visibility.Visible;
            //TreeImage.Source = new BitmapImage(new Uri("images/unseen.png", UriKind.RelativeOrAbsolute));
            //sp.Children.Add(TreeImage);
            sp.Children.Add(TreeText);
            TreeItem.Header = sp;




            //TreeItem.Header

            web = new Cjc.ChromiumBrowser.WebBrowser();
            web.Focusable = true;
            web.Source = "about:blank";
            web.Status += new EventHandler<Cjc.ChromiumBrowser.WebBrowser.StatusEventArgs>(web_Status);

            // enable javascript communication External.Call('name', 'arg1', 'arg2', ...);
            web.Callback += new EventHandler<Cjc.ChromiumBrowser.WebBrowser.CallbackEventArgs>(web_Callback);

            try
            {
                Theme = new Theme(Init.DataDirectory + "\\" + Settings.Get("CurrentTheme", "Default"));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error Loading theme: " + ex);
                Theme = new Theme("Default");
            }
            Theme.OnJavaScriptCall += new Theme.JavascriptHandler(theme_OnJavaScriptCall);
        }

        void theme_OnJavaScriptCall(string function, object[] args)
        {
            // base64
            string js = function + "(";
            for (int i = 0; i < args.Length - 1; i++)
            {
                js += "'" + args[i].ToString().Replace("'", "\\'") +"', ";
            }
            js += "'" + args[args.Length - 1].ToString().Replace("'", "\\'") + "')";
            web.ExecuteJavascript(js, "");
            ScrollViewToBottom();
        }

        void web_Callback(object sender, Cjc.ChromiumBrowser.WebBrowser.CallbackEventArgs e)
        {
            if (e.CallbackName == "Link" && e.Arguments.Length > 0)
            {
                string url = e.Arguments[0] as string;
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    Trace.WriteLine("Open URL: " + url);
                    System.Diagnostics.Process.Start(url);
                    return;
                }
            }
            Trace.WriteLine("JS: " + this + ": " + e.ObjectName + " " + e.CallbackName + ": " + e.Arguments);
        }

        void web_Status(object sender, Cjc.ChromiumBrowser.WebBrowser.StatusEventArgs e)
        {
            Trace.WriteLine("View: " + this + ": " + e.Message);
            if (e.Message == "FinishLoading" && !LoadedDefaultTheme)
            {
                LoadedDefaultTheme = true;
                web.LoadFile(Theme.MainFile);
                web.CreateObject("EX");
                web.SetObjectCallback("EX", "Link");
                //web.LoadHtml("<html><script>function test() { document.body.innerHTML='<a href=# onClick=\"EXT.test()\">dave</a>'; }</script><frame name=\"test\">hello world</frame></html>");
            }
        }

        protected abstract void HandleCommand(string command);

        public void SendCommand(string command)
        {
            if (command.StartsWith(Settings.Get("PrefixChar", "/")))
            {
                // / commands

            }
            else
            {
                HandleCommand(command);
            }
        }
    }

    class ServerView : BaseView
    {
        public class ViewCreatedEventArgs : EventArgs
        {
            public IView View { get; set; }
        }
        private Server Server = null;

        EventHandler<ViewCreatedEventArgs> ChannelViewCreated = null;
        public ServerView(Server Server, EventHandler<ViewCreatedEventArgs> ChannelViewCreated, Window window) : base(window)
        {
            this.Server = Server;
            this.ChannelViewCreated = ChannelViewCreated;
            TreeText.Text = Server.HostName;
            GlassName = Server.MyNick;
            Server.OnStatusMessage += new Server.StatusHandler(Server_OnStatusMessage);
            Server.OnSendingMessage += new Server.StatusHandler(Server_OnSendingMessage);
            Server.OnJoinedChannel += new Server.ChannelHandler(Server_OnJoinedChannel);
            
        }

        void Server_OnJoinedChannel(Channel c)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                try
                {
                    ChannelView cv = new ChannelView(c, window);
                    ViewCreatedEventArgs e = new ViewCreatedEventArgs();
                    e.View = cv;
                    ChannelViewCreated((object)this, e);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("exex: " + ex);
                }
            }));

        }

        void Server_OnSendingMessage(string message)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(() =>
            {
                Theme.Incoming_NoticeAuth(message);
            }));
        }

        void Server_OnStatusMessage(string message)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Incoming_NoticeAuth(message);
            }));
        }

        protected override void HandleCommand(string command)
        {
            Server.Send(command);
        }
    }

    class ChannelView : BaseView
    {
        private Channel Channel = null;

        public ChannelView(Channel Channel, Window window) : base(window)
        {
            this.Channel = Channel;
            GlassName = Channel.MyNick;
            TreeText.Text = Channel.Name;
            Channel.OnMessage += new Channel.MessageHandler(Channel_OnMessage);
            Channel.OnTopicSet += new Channel.MessageHandler(Channel_OnTopicSet);
            Channel.OnModesChanged += new Channel.MessageHandler(Channel_OnModesChanged);
            Channel.OnQuit += new Channel.MessageHandler(Channel_OnQuit);
            Channel.OnUserJoined += new Channel.MessageHandler2(Channel_OnUserJoined);
            Channel.OnUserKicked += new Channel.KickHandler(Channel_OnUserKicked);
            Channel.OnUserParted += new Channel.MessageHandler(Channel_OnUserParted);
        }

        void Channel_OnUserParted(User u, string message)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Incoming_Part(u.Nick, u.Host, u.UserName, message);
                SetMaxState(WindowState.Unseen);
            }));
        }

        void Channel_OnUserKicked(User kicker, User kicked, string msg)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Incoming_Kick(kicker.Nick, kicked.Nick, msg);
                SetMaxState(WindowState.Unseen);
            }));
        }

        void Channel_OnUserJoined(User u, string message, bool loudly)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                u.TreeItem = new TreeViewItem();
                u.TreeItem.Header = u.Nick;
                UsersList.Items.Add(u);
                if (loudly)
                {
                    Theme.Incoming_Join(u.Nick, u.Host, u.UserName);
                    SetMaxState(WindowState.Unseen);
                }
            }));
        }

        void Channel_OnQuit(User u, string message)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Incoming_Quit(u.Nick, u.Host, u.UserName, message);
                SetMaxState(WindowState.Unseen);
            }));
        }

        void Channel_OnModesChanged(User u, string message)
        {
            ModesBox = Channel.Modes;
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Incoming_ModeChage(u.Nick, message);
                SetMaxState(WindowState.Unseen);
            }));
        }

        void Channel_OnTopicSet(User u, string message)
        {
            Trace.WriteLine("Deferno/Channel/topicset: " + message);
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Trace.WriteLine("Deferno+UI/Channel/topicset: " + message);
                    Theme.Incoming_TopicReply(message);
                    TopicText = message;
                    SetMaxState(WindowState.Unseen);
                }));
        }

        void Channel_OnMessage(User u, string message)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Incoming_Message(u.Nick, message);
                SetMaxState(WindowState.UnseenMessages);
            }));
        }

        protected override void HandleCommand(string command)
        {
            window.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
            {
                Theme.Outgoing_Message(Channel.MyNick, command);
            }));
            Channel.Send(command);
        }
    }
}

