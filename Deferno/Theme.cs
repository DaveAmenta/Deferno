using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Deferno
{
    class Theme
    {
        public delegate void JavascriptHandler(string function, object[] args);
        public event JavascriptHandler OnJavaScriptCall;

        public string CurrentSection { get; set; }
        public string MainFile { get; private set; }
        private string Location = "";

        public Theme(string Location)
        {
            this.Location = Location;
            this.CurrentSection = "content";

            if (File.Exists(Location + "\\main.html"))
            {
                MainFile =  Location + "\\main.html";
            }
            else
            {
                throw new FileNotFoundException("Theme File does not exist.");
            }
        }

        private void InvokeScript(string script_name, object[] args)
        {
            if (OnJavaScriptCall != null)
            {
                OnJavaScriptCall(script_name, args);
            }
            else
            {
                Trace.WriteLine("Theme tried to call function without an attached JS handler: " + script_name);
            }
        }

        private string GetThemeHtml(string part1, string part2)
        {
            string loc = Location + "\\" + part1 + "\\" + part2 + ".html";
            try
            {
                if (File.Exists(loc))
                {
                    return File.ReadAllText(loc);
                }
                else
                {
                    Trace.WriteLine("Missing Theme Part! " + part1 + " " + part2);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error loading theme part: " + loc + " " + ex);
            }
            return "";
        }

        private string StripHTML(string s)
        {
            if (s == null) s = "";
            return s.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string ColorsToHtml(string s)
        {
            return Colorizer.ParseStyleCodes(s);
        }

        public void Outgoing_Message(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Outgoing", "Message").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Outgoing_Action(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Outgoing", "Action").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Outgoing_Notice(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Outgoing", "Notice").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Kick(string nick, string recip, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "Kick").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg).Replace("$recipient", recip);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_TopicDetailsReply(string nick, string date)
        {
            string msgObj = GetThemeHtml("Incoming", "TopicDetailsReply").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$date", date);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_TopicChange(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "TopicChange").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_ModeChage(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "ModeChange").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Message(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "Message").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_HighlightedMessage(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "HighlightedMessage").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Action(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "Action").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_HighlightedAction(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "HighlightedAction").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_NoticeAuth(string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "NoticeAuth").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Raw(string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "Raw").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Notice(string nick, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "Notice").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }


        public void Incoming_Nick(string nick, string msg)
        {
            string msgObj = GetThemeHtml("Incoming", "Nick").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Join(string nick, string host, string user)
        {

            string msgObj = GetThemeHtml("Incoming", "Join").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$username", user).Replace("$hostname", host);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Quit(string nick, string host, string user, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            string msgObj = GetThemeHtml("Incoming", "Quit").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$username", user).Replace("$hostname", host).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_Part(string nick, string host, string user, string msg)
        {
            msg = ColorsToHtml(StripHTML(msg));
            if (string.IsNullOrEmpty(msg))
            {
                msg = nick;
            }
            string msgObj = GetThemeHtml("Incoming", "Part").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$nickname", nick).Replace("$username", user).Replace("$hostname", host).Replace("$description", msg);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }

        public void Incoming_TopicReply(string topic)
        {
            topic = ColorsToHtml(StripHTML(topic));
            string msgObj = GetThemeHtml("Incoming", "TopicReply").Replace("$time", DateTime.Now.ToShortTimeString())
                .Replace("$description", topic);

            InvokeScript("appendMessage", new object[] { msgObj, CurrentSection });
        }
    }
}
