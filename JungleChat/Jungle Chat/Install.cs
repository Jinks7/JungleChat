using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    public partial class Install : Form
    {

        public bool finished;
        public bool cancelled;

        public Install(bool install)
        {
            InitializeComponent();

            finished = false;
            cancelled = true;
            //this.Show();
            if (install) { 
                Thread s = new Thread(installing);
                s.Start();
            }
            else
            {
                this.progress.Value = 100;
                Thread s = new Thread(uninstalling);
                s.Start();
            }
            
            
        }

        public void installing()
        {
            /*
            string workingDir = System.Environment.GetEnvironmentVariable("appdata");
            string currentDir = System.Environment.CurrentDirectory;
            string folder = workingDir +  @"\Jungle Chat\"; 
            
            // create directory
            try
            {
                Directory.CreateDirectory(folder);

                // copy exe to directory
                File.Copy(currentDir + @"\install\Jungle Chat.exe", folder + @"\Jungle Chat.exe");

                // add exe to startup programs
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                Key.SetValue("Jungle Chat", "\"" + folder + @"Jungle Chat" + "\"" + " -hidden");

                // put shortcut on dekstop
                appShortcutToDesktop("Jungle Chat");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



            /*
            // install it
            for (int i = 0; i < 100; i++)
            {
                changeValue(i);
                Thread.Sleep(50);
            }*//*
            changeValue(100);
            
            // finished installation
            Thread.Sleep(2000);
            finished = true;
            if (this.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    this.Close();
                };
                this.BeginInvoke(m);
            }
            else
            {
                this.Close();
            }
            */
            //MessageBox.Show("Method removed");

        }

        public void uninstalling()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MethodInvoker m = delegate
                    {
                        this.Text = "Uninstalling..";
                    };
                    this.BeginInvoke(m);
                }
                else
                {
                    this.Text = "Uninstalling..";
                }



                string workingDir = System.Environment.GetEnvironmentVariable("appdata");
                string currentDir = System.Environment.CurrentDirectory;
                string folder = workingDir + @"\Jungle Chat\";

                removeShortcut("Jungle Chat");

                try
                {
                    // delete settings directory
                    Directory.Delete(folder, true);
                    
                }
                catch (Exception e)
                {

                }

                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                Key.DeleteValue("Jungle Chat");
                /*
                for (int i = currentValue(); i > 0; i--)
                {
                    changeValue(i);
                    Thread.Sleep(50);
                }*/
                changeValue(0);

                finished = true;

                //ProcessStartInfo Info = new ProcessStartInfo();
                //Info.Arguments = "/C choice /C Y /N /D Y /T 30 & Del " +
                //               Application.ExecutablePath;
                //Info.WindowStyle = ProcessWindowStyle.Hidden;
                //Info.CreateNoWindow = true;
                //Info.FileName = "cmd.exe";
                //Process.Start(Info);
                
                Application.Exit();

            }
            catch (Exception e)
            {
                MessageBox.Show("Error uninstalling! Try running program as administrator and uninstalling may solve the problem.");
            }
            

        }

        private void appShortcutToDesktop(string linkName)
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            using (StreamWriter writer = new StreamWriter(deskDir + "\\" + linkName + ".url"))
            {
                string workingDir = System.Environment.GetEnvironmentVariable("appdata");
                string app = workingDir + @"\Jungle Chat\Jungle Chat.exe";
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + app);
                writer.WriteLine("IconIndex=0");
                string icon = app.Replace('\\', '/');
                writer.WriteLine("IconFile=" + icon);
                writer.Flush();
            }
        }

        private void removeShortcut(string linkName)
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            File.Delete(deskDir + @"\" + linkName + ".url");
        }

        public void changeValue(int num)
        {
            if (progress.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    progress.Value = num;
                };
                progress.BeginInvoke(m);
            }
            else
            {
                progress.Value = num;
            }
        }

        public int currentValue()
        {
            return progress.Value;
        }

        private void Install_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (finished)
            {
                cancelled = false;
            }
            else
            {


                // undo the installation
                uninstalling();

                // and tell the application to close
                cancelled = true;
            }
             */
        }


        private void Install_Load(object sender, EventArgs e)
        {

            

        }
    }
}
