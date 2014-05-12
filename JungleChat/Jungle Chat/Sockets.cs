using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    class Sockets
    {

        TcpClient client;
        Stream stream;
        StreamWriter writer;
        StreamReader reader;
        ChannelClass log;
        Thread receiver;
        Thread userRequest;
        frmMain main;
        ChannelHandler channel;

        public bool connected;
        public string nickname;
        public int level;

        public List<string> admin;

        public Sockets(ChannelClass l, frmMain m, ChannelHandler c)
        {
            log = l;
            main = m;
            channel = c;
            admin = new List<string>();
            level = 1;
        }

        public void connect()
        {

            if (connected)
            {
                log.writeln("Already connected to ther server", Color.Red);
            }
            else
            {
                log.writeln("Connecting.. ");
                
                try
                {

                    string temp = Properties.Settings.Default.host;

                    // create a tcp client
                    client = new TcpClient(temp, 2226); // "10.2.12.147", "localhost"

                    // create streams
                    stream = client.GetStream();
                    writer = new StreamWriter(stream);
                    reader = new StreamReader(stream);
                    writer.AutoFlush = true;

                    // next wait for a response from the server to see if accepted
                    string s;
                    if ((s = reader.ReadLine()) == "d") // if the server declines the request.
                    {
                        connected = false;
                        log.writeln("Could not connect. Too much traffic connected to server.", Color.Red);
                    }
                    else // will assume that the response is 'a'
                    {
                        connected = true;
                        send(getLocalInfo());
                        log.writeln("Connected!", Color.Green);
                    }

                    if (connected)
                    {
                        main.notify.ShowBalloonTip(3000, "Connected", "Successfully connected to the server.", ToolTipIcon.None);
                    }
                    else // just in case the server rejected the client
                    {
                        main.notify.ShowBalloonTip(3000, "Sorry", "Could not connect to the server.", ToolTipIcon.None);
                    }

                    // start thread for reading messages from the server
                    receiver = new Thread(readerThread);
                    receiver.Start();

                    userRequest = new Thread(requestUsers);
                    userRequest.Start();

                }
                catch (Exception e)
                {
                    log.writeln("Could not connect to the server. " + e.Message, Color.Red);
                    main.notify.ShowBalloonTip(3000, "Sorry", "Could not connect to the server.", ToolTipIcon.None);

                    // check if the form is trying to exit
                    /*Thread.Sleep(30000);
                    if (!main.exit)
                    {
                        
                        connect();
                    }*/
                    
                    
                }
            }
        }

        public string getLocalInfo()
        {

            string final = null;

            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            string nick;
            string computerName = Environment.MachineName;
            string username = Environment.UserName.ToLower();

            if (Properties.Settings.Default.defaultNick.Trim() == "") {
                nick = username; // from settings class get default nickname, if not set default to username
            } else {
                nick = Properties.Settings.Default.defaultNick; 
            }

            nickname = nick;

            // change the title to the username
            Properties.Settings.Default.defaultNick = nickname;
            changeTitle(nickname);

            final = "info " + computerName + " " + localIP + " " + username + " " + nick;
            return final;
        }

        public void disconnect()
        {
            try
            {
                
                log.writeln("Disconnecting..");
                send("disconnect");
                receiver.Abort();
                stream.Close();
                client.Close();
                connected = false;

                admin = new List<string>(); // refresh the admin list 

                for (int i = 0; i < channel.getAllChannels().Count; i++)
                {
                    if (channel.getAllChannels().ElementAt(i).getName() != "main")
                    {
                        channel.removeChannel(channel.getAllChannels().ElementAt(i).getName());
                    }
                }

                channel.getChannel("main").getChannel().clrList();

                main.notify.ShowBalloonTip(3000, "Disconnect", "Successfully disconnected to the server.", ToolTipIcon.None);
                

            }
            catch (Exception e)
            {
                log.writeln("Not connected", Color.Red);
            }

            // reset the title
            resetTitle();

        }

        public void readerThread()
        {
            try
            {

                send("checkVersion " + Assembly.GetEntryAssembly().GetName().Version);

                send("join main");

                string s;
                while ((s = readln()) != "disconnect" && connected)
                {
                    processCommands(s);
                }



                try
                {
                    connected = false;
                    log.writeln("You have been kicked from the server!", Color.Red);
                    log.writeln("Disconnecting..");
                    stream.Close();
                    client.Close();

                    //main.notify.ShowBalloonTip(3000, "Disconnect", "Successfully disconnected to the server.", ToolTipIcon.None);
                    main.notify.ShowBalloonTip(3000, "Kicked", "You have been kicked from the server.", ToolTipIcon.None);

                    for (int i = 0; i < channel.getAllChannels().Count; i++)
                    {
                        if (channel.getAllChannels().ElementAt(i).getName() != "main")
                        {
                            channel.getAllChannels().ElementAt(i).writeln("Disconnected from the server.", Color.Red);
                        }
                        Action action = () => channel.getAllChannels().ElementAt(i).getChannel().clrList();
                        channel.getAllChannels().ElementAt(i).getChannel().Invoke(action);
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
            catch (Exception e)
            {
                connected = false;

                main.notify.ShowBalloonTip(3000, "Disconnect", "You have been disconnected froms the server.", ToolTipIcon.None);
                log.writeln("Disconnected from the server.", Color.Red);

                for (int i = 0; i < channel.getAllChannels().Count; i++)
                {
                    if (channel.getAllChannels().ElementAt(i).getName() != "main")
                    {
                        channel.getAllChannels().ElementAt(i).writeln("Disconnected from the server.", Color.Red);
                    }
                    Action action = () => channel.getAllChannels().ElementAt(i).getChannel().clrList();
                    channel.getAllChannels().ElementAt(i).getChannel().Invoke(action);
                }
                
            }
        }

        public void requestUsers()
        {
            while (this.connected)
            {
                ChannelClass[] temp = channel.getAllChannels().ToArray();
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp.ElementAt(i).getConnected())
                    {
                        send("getusers " + temp[i].getName());
                    }
                }

                Thread.Sleep(3000);
            }
        }

        public string readln()
        {
            return reader.ReadLine();
        }

        public void send(string mesg)
        {
            //log.writeln("out: "+ mesg);
            try
            {
                writer.WriteLine(mesg);
            }
            catch (Exception e) { }
        }

        public void processCommands(string com)
        {
            //log.writeln("in: " + com);
            

            String[] s = com.Split(' ');
            if (s[0] == "msg")
            {
                if (s[2] != "[" + nickname + "]:")
                {
                    try
                    {
                        //log.writeln(com);
                        channel.getChannel(s[1]).writeln(com.Substring(5 + s[1].Length));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error occured! Could not find channel for 'msg' command from the server");
                    }

                    //log.writeln(com.Substring(4));
                }

            }
            else if (s[0] == "pmsg")
            {
                if (s[2] != "[" + nickname + "]:")
                {
                    try
                    {
                        channel.getChannel(s[1]).writeln(com.Substring(6 + s[1].Length));
                    }
                    catch (Exception e)
                    {
                        channel.createChannel(s[1]);
                        Thread.Sleep(50);
                        channel.getChannel(s[1]).writeln(com.Substring(6 + s[1].Length));
                    }
                }
                
            }
            else if (s[0] == "requestnick")
            {
                main.newNick();

            }
            else if (s[0] == "whois")
            {
                try
                {
                    MethodInvoker m = delegate
                    {
                        channel.getChannel(main.getCurrentChannel()).writeln(com.Substring(6));
                    };
                    
                    main.BeginInvoke(m);
                    
                }
                catch (Exception e)
                {
                    
                }
            }
            else if (s[0] == "admin")
            {
                try
                {
                    if (s[1] == "false")
                    {
                        level = 1;
                        log.writeln("You have been removed as an admin", Color.RoyalBlue);
                    }
                    else
                    {
                        log.writeln("You are now a server admin", Color.RoyalBlue);
                        level = 0;
                    }
                }
                catch (Exception e)
                {

                }
            }
            else if (s[0] == "list")
            {
                try
                {
                    channel.getChannel(s[1]).writeln(com.Substring(6 + s[1].Length), Color.OrangeRed);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            else if (s[0] == "all")
            {
                List<ChannelClass> c = channel.getAllChannels();
                for (int i=0;i<c.Count;i++){
                    c.ElementAt(i).writeln(com.Substring(s[0].Length+1), Color.Blue);
                }
            }
            else if (s[0] == "topic")
            {
                try
                {
                    channel.getChannel(s[1].Trim()).writeln(com.Substring(7 + s[1].Length), Color.Blue);
                }
                catch (Exception e)
                {}

            }
            else if (s[0] == "channeladmin")
            {
                try
                {
                    channel.getChannel(s[1].Trim()).writeln("You are now the admin of this channel.", Color.RoyalBlue);
                    admin.Add(s[1].Trim());
                }
                catch (Exception e) { }
            }
            else if (s[0] == "rmchanneladmin")
            {
                try
                {
                    channel.getChannel(s[1].Trim()).writeln("You have been removed as the admin.", Color.Red);
                    admin.Remove(s[1]);
                }
                catch (Exception e) { }
            }
            else if (s[0] == "kicked")
            {
                try
                {
                    channel.getChannel(s[1].Trim()).writeln("You have been kicked from the the channel: " +s[1], Color.Red);
                    main.notify.ShowBalloonTip(4000, s[1], "You have been kicked from the channel.", ToolTipIcon.None);

                    Action action = () => channel.getChannel(s[1].Trim().ToLower()).setConnected(false);
                    channel.getChannel(s[1].Trim().ToLower()).getChannel().Invoke(action);

                    action = () => channel.getChannel(s[1].Trim().ToLower()).getChannel().clrList();
                    channel.getChannel(s[1].Trim().ToLower()).getChannel().Invoke(action);

                }
                catch (Exception e)
                { }

            }
            else if (s[0] == "nick")
            {
                nickname = s[1];
                changeTitle(nickname);
                Properties.Settings.Default.defaultNick = nickname;
            }
            else if (s[0] == "notify")
            {
                try
                {
                    string[] temp = com.Substring(7).Split(';');
                    log.writeln(temp[1], Color.Red);
                    main.notify.ShowBalloonTip(3000, temp[0], temp[1], ToolTipIcon.None);
                }
                catch (Exception e)
                {
                    // just ignore the error
                }
            }
            else if (s[0] == "createchan")
            {
                
                channel.createChannel(s[1]);
                Thread.Sleep(50); // if it doesnt wait 50 miliseconds client will disconnect from the server
                channel.getChannel(s[1]).writeln("Connected to channel", Color.Green);
                Action action = () => main.focusInput();
                main.Invoke(action);
                
            }
            else if (s[0] == "users")
            {
                string[] temp = s[2].Split(',');

                if (channel.getChannel(s[1]).getChannel().InvokeRequired)
                {
                    Action action = () => channel.getChannel(s[1].Trim().ToLower()).getChannel().clrList();
                    channel.getChannel(s[1].Trim().ToLower()).getChannel().Invoke(action);
                }
                else
                {
                    channel.getChannel(s[1]).getChannel().clrList();
                }

                for (int i=0;i<temp.Length-1;i++){
                    if (channel.getChannel(s[1]).getChannel().InvokeRequired)
                    {   
                        Action action = () => channel.getChannel(s[1].Trim().ToLower()).getChannel().addList(temp[i]);
                        channel.getChannel(s[1].Trim().ToLower()).getChannel().Invoke(action);
                    }
                    else
                    {
                        channel.getChannel(s[1]).getChannel().addList(temp[i]);
                    }
                    
                    
                }
                
            }
            else
            {
                log.writeln(com);
            }
            
            
        }

        public Boolean isChannelAdmin(string channel)
        {
            foreach (string m in admin)
            {
                if (m == channel)
                {
                    return true;
                }
            }
            return false;
        }

        public void changeTitle(string nick)
        {
            if (main.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    main.Text = "Jungle Chat - " + nick;
                };
                main.BeginInvoke(m);
            }
            else
            {
                main.Text = "Jungle Chat - " + nick;
            }
            
        }

        public void resetTitle()
        {
            if (main.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    main.Text = "Jungle Chat";
                };
                main.BeginInvoke(m);
            }
            else
            {
                main.Text = "Jungle Chat";
            }
            
        }
    }
}
