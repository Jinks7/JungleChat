using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{

    public partial class frmMain : Form
    {

        string[] commands = { "help", "connect", "disconnect", "join", "leave", 
                                "addadmin", "rmadmin", "rmchannel", "whois", "list", 
                                "kick", "addban", "rmban", "settopic", "banned"};

        ChannelClass log;
        Sockets socket;
        ChannelHandler channel;
        Thread connect;

        static bool hidden;
        public bool exit = false;

        

        public frmMain()
        {

            InitializeComponent();

            channel = new ChannelHandler(tab);
            channel.createChannel("main");

            log = channel.getChannel("main");

            /*
            // check if install is needed
            FileIO file = new FileIO(log);
            if (file.needInstall())
            { // if it needs to install
                file.performInstall(this);
            }*/


            socket = new Sockets(log, this, channel);

            // attempt to connect

            connect = new Thread(socket.connect);
            connect.Start();

        }

        public void createChannel(string n)
        {
            channel.createChannel(n);
        }

        public void setPriv(string s)
        {
            channel.getChannel(s).setCommand(true);
        }

        public bool isHidden()
        {
            return hidden;
        }

        public void setHidden()
        {
            hidden = true;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }

        public void setShow()
        {
            hidden = false;
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;

        }

        // when the send button is clicked
        private void button1_Click(object sender, EventArgs e)
        {
            handleSend();
        }

        // stop the output from keeping focus
        private void output_Click(object sender, EventArgs e)
        {

            input.Focus();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab) return false;
            return base.ProcessDialogKey(keyData);
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // if enter is pressed 
            {
                handleSend();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode.Equals(Keys.Tab)) // if control tab is pressed
            {
                if (tab.SelectedIndex == tab.TabCount - 1)
                {
                    tab.SelectedTab = tab.TabPages[0];
                }
                else
                {
                    tab.SelectedTab = tab.TabPages[tab.SelectedIndex+1];
                }
                input.Focus();
            } 
            else if (e.KeyCode.Equals(Keys.Tab))
            {
                // auto fill the text box
                if (input.Text.Trim() == "")
                {

                }
                else if (input.Text.StartsWith("/"))
                {
                    // check against help commands
                    string temp = input.Text.Substring(1).ToLower();
                    string change = "";
                    int match = 0;
                    for (int i = 0; i < commands.Length;i++)
                    {
                        if (commands[i].StartsWith(temp))
                        {
                            change = commands[i];
                            match++;
                        }
                    }
                    if (match == 1)
                    {
                        input.Text = "/" + change + " ";
                        input.Select(input.Text.Length, 0); // move the cursor to the end
                    }
                }
                else
                {
                    // check against usernames
                    string temp = input.Text.ToLower();
                    string change = "";
                    int match = 0;
                    foreach (string s in channel.getChannel(tab.SelectedTab.Text).getChannel().getList().Items)
                    {
                        if (s.StartsWith(temp))
                        {
                            change = s;
                            match++;
                        }
                    }

                    if (match == 1)
                    {
                        change = change.Replace("+", "").Trim();
                        input.Text = change + ": ";
                        input.Select(input.Text.Length, 0); // move the cursor to the end
                    }


                }


                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyValue == 8 && e.Modifiers == Keys.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                String[] chars = new String[1] { " " };
                var temp = (from string s in input.Text.Split(chars, StringSplitOptions.None)
                            select s).ToArray();

                temp[temp.Length - 1] = "";

                input.Text = String.Join(" ", temp).ToString();
                try
                {
                    input.Text = input.Text.Remove(input.Text.Length - 1);
                }
                catch (Exception ex) { }
                SendKeys.Send("{END}");

            }
            else if (e.KeyCode == Keys.F4 && e.Alt)
            {
                handleExit(null);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        // commands function!
        public void handleSend()
        {
            /** process message **/

            if (input.Text.Trim() == "")
            {
                return;
            }

            // only if you are the admin of the server
            if (socket.level == 0)
            {
                // this will send a direct command to the server
                if (input.Text.StartsWith("//"))
                {
                    log.writeln(input.Text);
                    socket.send(input.Text);
                    input.Text = "";
                    return;
                }
            }

            // if it starts with a '/' we process it as a command 
            // these are all channel commands
            if (input.Text.StartsWith("/"))
            {
                string[] s = input.Text.Split(" ".ToCharArray(), StringSplitOptions.None);

                // get current selected channel
                ChannelClass cc = channel.getChannel(tab.SelectedTab.Text);

                // list of commands
                
                switch (Array.IndexOf(commands, s[0].Substring(1)))
                {
                    case 0: // help
                        cc.writeln("List of commands: ", Color.IndianRed);
                        for (int i = 0; i < commands.Length; i++)
                        {
                            cc.writeln(i+1 + ": " + commands[i], Color.RoyalBlue);
                        }
                        break;
                    case 1: // connect
                        connect = new Thread(socket.connect);
                        connect.Start();
                        break;
                    case 2: // disconnect
                        if (socket.connected)
                        {
                            socket.disconnect();
                        }
                        else
                        {
                            cc.writeln("You are already disconnected.", Color.Red);
                        }
                        break;
                    case 3: // join
                        try
                        {
                            if (socket.connected)
                            {
                                socket.send("join " + s[1]); // the server will determine if they are allowed to join
                            }
                            else
                            {
                                cc.writeln("You are not connected to the server.", Color.Red);
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("Join takes a second argument.", Color.Red);
                        }
                        break;
                    case 4: // leave

                        if (tab.SelectedTab.Text != "main")
                        {
                            // if it is private just remove it from tabs
                            if (channel.getChannel(tab.SelectedTab.Text).getCommand().Trim() == "pmsg")
                            {
                                channel.removeChannel(tab.SelectedTab.Text);
                            }
                            else // if not private send leave and remove it from tabs
                            {
                                socket.send("leave " + tab.SelectedTab.Text);
                                channel.removeChannel(tab.SelectedTab.Text);
                            }
                           
                        }
                        else
                        {
                            MessageBox.Show("Can not remove the main tab.");
                        }
                        break;
                    case 5: // addadmin
                        try
                        {
                            if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                            {
                                cc.writeln("This has not been implemented yet");
                            }
                            else
                            {
                                cc.writeln("You are not an admin of this channel.");
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("You need an extra argument", Color.Red);
                        }
                        break;
                    case 6: // rmadmin
                        try
                        {
                            if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                            {
                                cc.writeln("This has not been implemented yet");
                            }
                            else
                            {
                                cc.writeln("You are not an admin of this channel.");
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("You need an extra argument", Color.Red);
                        }
                        break;
                    case 7: // rmchannel
                        try
                        {
                            if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                            {
                                socket.send("rmchannel " + tab.SelectedTab.Text);
                            }
                            else
                            {
                                cc.writeln("You are not an admin of this channel.");
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("You need an extra argument", Color.Red);
                        }
                        break;
                    case 8: // whois
                        if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                        {
                            try
                            {
                                socket.send("whois " + s[1]);
                            }
                            catch (Exception e)
                            {
                                cc.writeln("You need to specify a name", Color.Red);
                            }
                        }
                        else
                        {
                            cc.writeln("You are not the admin of this channel.");
                        }
                        break;
                    case 9: // list
                        socket.send("requestlist " + tab.SelectedTab.Text);
                        cc.writeln("List of channels:");
                        break;
                    case 10: // kick
                        try
                        {
                            if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                            {
                                socket.send("kick " + tab.SelectedTab.Text + " " + s[1]);
                            }
                            else
                            {
                                cc.writeln("You are not an admin of this channel.");
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("You need an extra argument", Color.Red);
                        }
                        break;
                    case 11: // addban
                        try
                        {
                            if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                            {
                                socket.send("addban " + tab.SelectedTab.Text + " " + s[1]);
                            }
                            else
                            {
                                cc.writeln("You are not an admin of this channel.");
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("You need an extra argument", Color.Red);
                        }
                        break;
                    case 12: // rmban
                        try
                        {
                            if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                            {
                                socket.send("rmban " + tab.SelectedTab.Text + " " + s[1]);
                            }
                            else
                            {
                                cc.writeln("You are not an admin of this channel.");
                            }
                        }
                        catch (Exception e)
                        {
                            cc.writeln("You need an extra argument", Color.Red);
                        }
                        break;
                    case 13: // set topic of channel
                        if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                        {
                            cc.writeln("Updating topic..");
                            try
                            {
                                socket.send("newtopic " + tab.SelectedTab.Text + " " + input.Text.Substring(10));
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else
                        {
                            cc.writeln("You are not an admin of this channel.");
                        }
                        break;
                    case 14: // banned
                        if (socket.isChannelAdmin(tab.SelectedTab.Text) || socket.level == 0)
                        {
                            socket.send("banned " + tab.SelectedTab.Text);
                        }
                        else
                        {
                            cc.writeln("You are not an admin of this channel.");
                        }
                        break;
                    default:
                        cc.writeln("Could not find command: " + s[0].Substring(1));
                        break;
                }
                
            }
            else
            {
                
                // send to server
                try
                {

                    // send it to specific tab
                    string selectedTab = tab.SelectedTab.Text;

                    if (!socket.connected)
                    {
                        channel.getChannel(selectedTab).writeln("You are not connected to the server.", Color.Red);
                    }
                    else
                    {
                        socket.send(channel.getChannel(selectedTab).command + selectedTab + " " + input.Text.Trim()); // should have like a channel name
                        channel.getChannel(selectedTab).writeln("[Me]: " + input.Text.Trim(), Color.Navy);
                    }
                }
                catch (Exception e)
                {
                    log.writeln("Could not send \"" + input.Text + "\" to the server. Error is: " + e.Message, Color.Red);
                }
            }


            // clear input text
            input.Clear();
            input.Focus();
        }

        // exiting function
        public void handleExit(FormClosingEventArgs e)
        {
            exit = true;
            // hide the form
            this.Hide();
            try
            {
                for (int i = 0; i < channel.getAllChannels().Count; i++)
                {
                    socket.send("leave " + channel.getAllChannels().ElementAt(i).getName());
                }
                // send a request to the server to disconnect gracefully
                socket.send("disconnect");
                // wait for response from server.
                string s = socket.readln();

                //MessageBox.Show(s); // perform a check
                
                if (s == "hold")
                {
                    try
                    {
                        e.Cancel = true;
                    }
                    catch (Exception m) { }
                }
                else if (s == "disconnect")
                {
                    //MessageBox.Show("disconnect");
                    // permission to disconnect
                    
                    socket.disconnect();
                    Application.Exit();
                }
                    socket.disconnect();
                    Application.Exit();
                
            }
            catch (Exception l)
            {
                socket.disconnect();
                Application.Exit();
            }

        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(socket.connect);
            t.Start();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            socket.disconnect();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleExit(null);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //handleExit(e);
            if (!exit)
            {
                // hide the form
                setHidden();

                e.Cancel = true;
            }
            else
            {
                handleExit(null);
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            
            input.Focus();
        }

        private void tab_Resize(object sender, EventArgs e)
        {
            try
            {
                // change the size of all the channel
                List<ChannelClass> list = channel.getAllChannels();
                for (int i = 0; i < list.Count; i++)
                {
                    list.ElementAt(i).getChannel().resize(tab.Width - 10, tab.Height - 24);
                }
            }
            catch (Exception error)
            {

            }
        }

        public void changeTitle(string nick)
        {
            Text = "Jungle Chat - " + nick; 
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 m = new AboutBox1();
            m.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exit = true;
            try
            {
                new Settings().ShowDialog();
                if (Properties.Settings.Default.defaultNick.Split(' ')[0] != socket.nickname)
                {
                    socket.send("nick " + Properties.Settings.Default.defaultNick.Split(' ')[0]);
                }
            }
            catch (Exception m) { }
            exit = false;

        }

        private void connectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (socket.connected)
            {
                Thread s = new Thread(socket.disconnect);
                s.Start();
            }
            else
            {
                Thread s = new Thread(socket.connect);
                s.Start();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isHidden())
            {
                setShow();
            }
            else
            {
                setHidden();
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //this.Close();
            handleExit(null);
        }

        private void notify_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isHidden())
                {
                    setShow();
                }
                else
                {
                    setHidden();
                }
            }
        }

        private void joinChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Request l = new Request("Join: ");
            l.ShowDialog();

            if (l.getCorrect())
            {
                // notify server and create channel
                string[] s = l.getValue().Split(' ');
                //channel.createChannel(s[0]);
                try
                {
                    socket.send("join " + s[0]);
                }
                catch (Exception m) { }

            }
        }

        private void leaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // leave current tab
                if (tab.SelectedTab.Text != "main")
                {
                    socket.send("leave " + tab.SelectedTab.Text); // notify server
                }
                channel.removeChannel(tab.SelectedTab.Text);                
            }
            catch (Exception m) {
                channel.removeChannel(tab.SelectedTab.Text);  
            }
            
            

        }

        public void newNick()
        {
            Request request = new Request("Nick: ");
            //request.Parent = this;
            request.ShowDialog();
            if (request.getValue().Trim() == "")
            {
                newNick();
            }
            socket.send("nick " + request.getValue().Trim());
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            handleExit(null);
        }

        public void focusInput()
        {
            input.Focus();
        }

        public string getCurrentChannel()
        {
            /*if (tab.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    //return tab.SelectedTab.Text;
                };

                tab.BeginInvoke(m);
            }
            else
            {*/
                return tab.SelectedTab.Text;
            //}
            
        }

        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }


    



}
