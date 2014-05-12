using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    class ChannelHandler
    {

        TabControl tab;
        List<ChannelClass> tabs;
        int MAX = 5;

        public ChannelHandler(TabControl t)
        {
            tab = t;
            tabs = new List<ChannelClass>();
        }

        

        public void createChannel(string title)
        {
            foreach (ChannelClass s in tabs)
            {
                if (s.getName() == title)
                {
                    return;
                }
            }

            if (tabs.Count < MAX)
            {
                if (tab.InvokeRequired)
                {
                    MethodInvoker m = delegate
                    {
                        tabs.Add(new ChannelClass(title));
                        getChannel(title).getChannel().resize(tab.Width - 10, tab.Height - 24);
                        tab.TabPages.Add(getChannel(title).getTabPage());
                        tab.SelectedTab = getChannel(title).getTabPage();
                    };
                    tab.BeginInvoke(m);
                }
                else
                {
                    tabs.Add(new ChannelClass(title));
                    getChannel(title).getChannel().resize(tab.Width - 10, tab.Height - 24);
                    tab.TabPages.Add(getChannel(title).getTabPage());
                    tab.SelectedTab = getChannel(title).getTabPage();
                }
                
            }
        }

        public ChannelClass getChannel(string name)
        {

                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabs.ElementAt(i).getName() == name)
                    {
                        return tabs.ElementAt(i);
                    }
                }
            
            return null;
        }

        public List<ChannelClass> getAllChannels()
        {
            return tabs;
        }

        public void removeChannel(string name)
        {
            // stop the user from removing the main tab
            if (name == "main")
            {
                MessageBox.Show("Could not remove the main tab.");
                return;
            }

            

            for (int i = 0; i < tabs.Count; i++)
            {
                
                if (tabs.ElementAt(i).getName() == name)
                {
                    try
                    {

                        if (tab.InvokeRequired)
                        {
                            MethodInvoker m = delegate
                            {
                                //tab.SelectedTab = tabs.ElementAt(i).getTabPage();
                                //getChannel(name).remove();
                                tab.TabPages.Remove(tabs.ElementAt(i).getTabPage());
                                tabs.RemoveAt(i);
                            };
                            tab.BeginInvoke(m);
                        }
                        else
                        {
                            //tab.SelectedTab = tabs.ElementAt(i).getTabPage();
                            //getChannel(name).remove();
                            tab.TabPages.Remove(tabs.ElementAt(i).getTabPage());
                            tabs.RemoveAt(i);
                        }


                    }
                    catch (Exception e) { MessageBox.Show(i.ToString()); }
                }
            }

            
        }

    }
}
