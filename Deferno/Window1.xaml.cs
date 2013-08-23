using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DavuxLib.Win32API;
using System.Diagnostics;
using DavuxLib;

namespace Deferno
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            if (DavuxLib.Registration.CanExecuteEx("Deferno", ""))
            {
                Deferno.App app = new Deferno.App();
                app.InitializeComponent();
                app.Run();
            }
        }


        private List<IView> Views = new List<IView>();
        private IView SelectedView = null;



        void SelectView(IView view)
        {
            if (view == null)
            {
                // switch to dashboard
                if (SelectedView == null) return;

                // hide the old view
                SelectedView.web.Visibility = Visibility.Collapsed;
                SelectedView.UsersList.Visibility = Visibility.Collapsed;

                // show the dashboard
                CenterContent.Visibility = Visibility.Collapsed;
                RightNav.Visibility = Visibility.Collapsed;
                Dashboard.Visibility = Visibility.Visible;
                RightSplit.Visibility = Visibility.Collapsed;

                SelectedView.Deselect();
                SelectedView = null;
                lblGlassName.Content = "";
                txtTopic.Text = "";
                txtModes.Text = "";
            }
            else
            {
                // switch to view
                if (view == SelectedView) return;

                // hide the old view
                if (SelectedView == null)
                {
                    // hide the dashbaord
                    CenterContent.Visibility = Visibility.Visible;
                    RightNav.Visibility = Visibility.Visible;
                    Dashboard.Visibility = Visibility.Collapsed;
                    RightSplit.Visibility = Visibility.Visible;
                }
                else
                {
                    // hide the old iview
                    SelectedView.Deselect();
                    SelectedView.web.Visibility = Visibility.Collapsed;
                    SelectedView.UsersList.Visibility = Visibility.Collapsed;
                }

                // show self
                view.web.Visibility = Visibility.Visible;
                view.UsersList.Visibility = Visibility.Visible;
                SelectedView = view;
                SelectedView.TreeItem.IsSelected = true;
                SelectedView.Select();
                lblGlassName.Content = SelectedView.GlassName;
                txtTopic.Text = SelectedView.TopicText;
                txtModes.Text = SelectedView.ModesBox;
            }
        }

        void AddView(IView view, bool root)
        {
            Views.Add(view);
            if (root)
            {
                tvServers.Items.Add(view.TreeItem);
            }
            WebContainer.Children.Add(view.web);
            listContainer.Children.Add(view.UsersList);
            
            view.web.Visibility = Visibility.Collapsed;

            if (Settings.Get("SwitchOnJoin", true))
            {
                SelectView(view);
            }
        }


        public Window1()
        {
            InitializeComponent();
            Init.InitSmart("Deferno", true);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
             GlassHelper.ExtendGlassFrame(this, new Thickness(-1));

             txtNick.Text = Settings.Get("DefaultNick", "");
             txtUser.Text = Settings.Get("DefaultUser", "deferno");
             txtServer.Text = Settings.Get("DefaultServer", "");
             txtPassword.Text = Settings.Get("DefaultPassword", "");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // web.LoadHtml("<html><script>function test() { document.body.innerHTML='<a href=# onClick=\"EXT.test()\">dave</a>'; }</script><frame name=\"test\">hello world</frame></html>");

            //web.Status += new EventHandler<Cjc.ChromiumBrowser.WebBrowser.StatusEventArgs>(web_Status);
            //web.Navigate("http://www.youtube.com");
           // web.Callback += new EventHandler<Cjc.ChromiumBrowser.WebBrowser.CallbackEventArgs>(web_Callback);
           // web.CreateObject("EXT");
           // web.SetObjectCallback("EXT", "test");



            Server.OnConnecting += new Server.ServerHandler(Server_OnConnecting);
            
        }

        void Server_OnConnecting(Server s)
        {
            AddView(new ServerView(s, new EventHandler<ServerView.ViewCreatedEventArgs>(ViewAdded), this), true);
        }

        void ViewAdded(object sender, ServerView.ViewCreatedEventArgs e)
        {
            ServerView s = sender as ServerView;
            s.TreeItem.Items.Add(e.View.TreeItem);
            s.TreeItem.IsExpanded = true;
            AddView(e.View, false);
            if (Settings.Get("SwitchOnJoin", true))
            {
                SelectView(e.View);
            }
        }

        void web_Callback(object sender, Cjc.ChromiumBrowser.WebBrowser.CallbackEventArgs e)
        {
            Trace.WriteLine("Callback: " + sender + " " + e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectView(null);
            TreeViewItem tv = tvServers.SelectedItem as TreeViewItem;
            if (tv != null)
            {
                tv.IsSelected = false;
            }
            //web.Source = "http://www.youtube.com/";
            //web.Navigate(new Uri("http://www.youtube.com"));
            //web.Source = "javascript:alert('hello world');";
            //5web.ExecuteJavascript("test();", "");
            
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Settings.Set("DefaultNick", txtNick.Text);
            Settings.Set("DefaultUser", txtUser.Text);
            Settings.Set("DefaultServer", txtServer.Text);
            Settings.Set("DefaultPassword", txtPassword.Text);

            new Server(txtServer.Text, 6667, txtNick.Text, txtUser.Text);
            
        }

        private void tvServers_Selected(object sender, RoutedEventArgs e)
        {
            SelectView((tvServers.SelectedItem as TreeViewItem).Tag as IView);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && txtInput.Text.Trim().Length > 0)
            {
                if (SelectedView != null)
                {
                    SelectedView.SendCommand(txtInput.Text);
                }
                txtInput.Text = "";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Environment.Exit(0);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (SelectedView != null)
            {
                SelectedView.Select();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (SelectedView != null)
            {
                SelectedView.Deselect();
            }
        }
    }
}
