using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    class ChannelClass
    {
        string name;
        string topic;
        public string command = "msg "; // if for private messaging, needs to be pmsg

        Channel channel;
        TabPage page;
        Logger log;

        Boolean admin = false;
        bool connected;

        public ChannelClass(string na)
        {
            name = na;
            page = new TabPage(name);
            channel = new Channel();
            log = new Logger(channel.getBox());
            page.Controls.Add(channel);
            connected = true;
        }

        public void setConnected(bool con)
        {
            connected = con;
        }

        public bool getConnected()
        {
            return connected;
        }

        public void setCommand(bool priv)
        {
            if (priv)
            {
                command = "pmsg ";
            }
            else
            {
                command = "msg ";
            }
        }

        public string getCommand()
        {
            return command;
        }

        public void setTopic(string s)
        {
            topic = s;
        }

        public string getTopic()
        {
            return topic;
        }

        public void setAdmin(Boolean ad)
        {
            admin = ad;
        }

        public bool getAdmin()
        {
            return admin;
        }

        public void remove()
        {
            page.Controls.Remove(channel);

        }

        public string getName()
        {
            return name;
        }

        public TabPage getTabPage()
        {
            return page;
        }

        public Channel getChannel()
        {
            return channel;
        }

        public void write(string mesg)
        {
            log.write(mesg);
        }

        public void writeln(string mesg)
        {
            log.writeln(mesg);
        }

        public void write(string mesg, Color c)
        {
            log.write(mesg, c);
        }

        public void writeln(string mesg, Color c)
        {

            log.writeln(mesg, c);
        }

        public void setColor(Color c)
        {
            log.setColor(c);
        }

    }
}