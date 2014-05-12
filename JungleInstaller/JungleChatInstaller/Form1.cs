using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JungleChatInstaller
{
    public partial class frmMain : Form
    {

        string host = "localhost";


        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            start();
        }

        public void start()
        {
            // once connection has been made
            // when opening initiate connection with the server using http
            try
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                // check if file exists (will throw error)
                WebRequest h = WebRequest.Create(new Uri("http://" + host + "/Jungle/Jungle%20Chat.exe"));
                WebResponse r = h.GetResponse();

                client.DownloadFileAsync(new Uri("http://" + host + "/Jungle/Jungle%20Chat.exe"), Path.GetTempPath() + "/Jungle Chat.exe");
            }
            catch (Exception l)
            {
                MessageBox.Show("Could not download Jungle Chat");
                Application.Exit();
            }
        }

        public void downInstaller()
        {
            try
            {
                Thread.Sleep(1000);
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChangedd);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompletedd);

                // check if file exists (will throw error)
                WebRequest h = WebRequest.Create(new Uri("http://" + host + "/Jungle/Installer.exe"));
                WebResponse r = h.GetResponse();

                client.DownloadFileAsync(new Uri("http://" + host + "/Jungle/Installer.exe"), Path.GetTempPath() + "/Installer.exe");

                
            }
            catch (Exception l)
            {
                MessageBox.Show("Could not download Installer" + l.ToString());
                Application.Exit();
            }
        }

        public void install()
        {
            Thread.Sleep(1000);
            setText("Removing old Client.");
            
            // remove the old version of Jungle chat, if it exists
            if (oldClient())
            {
                try
                {

                    string workingDir = System.Environment.GetEnvironmentVariable("appdata");
                    string folder = workingDir + @"\Jungle Chat\";

                    string tempDir = Path.GetTempPath();

                    setValue(60);
                    Thread.Sleep(500);

                    // delete the old client
                    System.IO.File.Delete(folder + @"Jungle Chat.exe");
                    System.IO.File.Delete(folder + @"Installer.exe");

                    setValue(70);
                    Thread.Sleep(500);

                    setText("Moving Jungle Chat.exe to install directory");

                    // and move the exe from temp to new direcotry
                    System.IO.File.Move(tempDir + "/Jungle Chat.exe", folder + @"Jungle Chat.exe");
                    System.IO.File.Move(tempDir + "/Installer.exe", folder + @"/Installer.exe");
                    setValue(80);
                    Thread.Sleep(500);

                    setText("Moving Installer to install directory.");

                    setValue(90);
                    Thread.Sleep(500);

                    setText("Removing evidence.");
                    System.IO.File.Delete(tempDir + "/Jungle Chat.exe");
                    System.IO.File.Delete(tempDir + "/Installer.exe");
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("There was an error installing the new Jungle Chat client. ERROR: " + e.ToString());
                }

            }
            else
            {
                try
                {

                    string workingDir = System.Environment.GetEnvironmentVariable("appdata");
                    string folder = workingDir + @"\Jungle Chat\";

                    string tempDir = Path.GetTempPath();

                    setValue(60);
                    Thread.Sleep(500);

                    setText("Creating directory");
                    Directory.CreateDirectory(folder);

                    setValue(70);
                    Thread.Sleep(500);

                    setText("Moving Jungle Chat.exe and Installer to install directory");

                    // and move the exe from temp to new direcotry
                    System.IO.File.Move(tempDir + "/Jungle Chat.exe", folder + @"Jungle Chat.exe");

                    try
                    {
                        System.IO.File.Move(tempDir + "/Installer.exe", folder + @"/Installer.exe");
                    }
                    catch (Exception l)
                    {
                        setText("Could not move installer to install directory");
                        Thread.Sleep(300);
                    }
                    setValue(80);
                    Thread.Sleep(500);

                    setText("Setting up..");
                    RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    Key.SetValue("Jungle Chat", "\"" + folder + @"Jungle Chat" + "\"" + " -hidden");
                    appShortcutToDesktop("Jungle Chat");

                    setValue(90);
                    Thread.Sleep(800);

                    setText("Removing evidence.");
                    System.IO.File.Delete(tempDir + "/Jungle Chat.exe");
                    System.IO.File.Delete(tempDir + "/Installer.exe");

                }
                catch (Exception e)
                {
                    MessageBox.Show("There was an error installing the new Jungle Chat client. ERROR: " + e.ToString());
                }
            }

            setValue(100);

            Thread.Sleep(500);

            setText("Finished");

            Thread.Sleep(1000);

            // run the main application in a seperate process
            try
            {
                string file = System.Environment.GetEnvironmentVariable("appdata") + @"\Jungle Chat\Jungle Chat.exe";

                var proc = new Process();
                proc.StartInfo.FileName = file;
                proc.Start();
            }
            catch (Exception e)
            {

            }

            Application.Exit();


        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // when the form is closing
            //lblStatus.Text = "Cancelling..";
            
            // check where the installer is up to

            // and undo what is done

            // eg delete downloaded file
            
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            setValue(int.Parse(Math.Truncate(percentage /4).ToString()));

            setText("Downloading Jungle Chat... " + percentage + "%");

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            /*
            Thread t = new Thread(install);
            t.Start();
             */
            new Thread(downInstaller).Start();
        }

        void client_DownloadProgressChangedd(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            setValue(int.Parse(Math.Truncate(25+ percentage / 4).ToString()));

            setText("Downloading Installer... " + Math.Truncate(percentage).ToString() + "%");

        }

        void client_DownloadFileCompletedd(object sender, AsyncCompletedEventArgs e)
        {
            Thread t = new Thread(install);
            t.Start();
        }

        public bool oldClient()
        {
            string workingDir = System.Environment.GetEnvironmentVariable("appdata");
            string folder = workingDir + @"\Jungle Chat\Jungle Chat.exe";
            // check if folder exists in workingDir called Jungle Chat
            if (System.IO.File.Exists(folder))
            {
                return true;
            }
            else
            {
                return false;
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

        public void setText(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    lblStatus.Text = text;
                };
                lblStatus.BeginInvoke(m);
            }
            else
            {
                lblStatus.Text = text;
            }
        }

        public void setValue(int value)
        {
            if (pbar.InvokeRequired)
            {
                MethodInvoker m = delegate
                {
                    pbar.Value = value;
                };
                pbar.BeginInvoke(m);
            }
            else
            {
                pbar.Value = value;
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //start();
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            
        }
    }
}
