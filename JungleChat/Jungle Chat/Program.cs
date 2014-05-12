using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                frmMain m = new frmMain();

                foreach (string s in args){
                    if (s == "-hidden")
                    {
                        m.setHidden();
                    }
                }

                Application.Run(m);
            }
            catch (Exception e)
            {

            }
        }
    }
}
