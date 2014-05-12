using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Jungle_Chat
{
    class FileIO
    {

        string workingDir = System.Environment.GetEnvironmentVariable("appdata");
        StreamReader reader;
        StreamWriter writer;
        string file;
        ChannelClass log;

        public FileIO(ChannelClass l)
        {
            log = l;

        }

        public bool needInstall()
        {
            // check if folder exists in workingDir called Jungle Chat
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

        public void performInstall(frmMain main)
        {
            // disable main window
            Install install = new Install(true);
            install.ShowDialog();
            
            // show sub window that will display progress
            if (install.cancelled == true)
            {
                main.Close();
            }


        }

    }
}
