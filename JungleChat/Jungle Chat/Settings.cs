using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    public partial class Settings : Form
    {

        bool skip = false;

        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Install(false).ShowDialog();
            // close the main application
            
            skip = true;
            Application.Exit();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!skip)
            {
                // save the settings
                Properties.Settings.Default.defaultNick = txtNick.Text.ToLower().Split('+')[0];
                Properties.Settings.Default.favChannel = txtChan.Text.ToLower();
                Properties.Settings.Default.host = txtHost.Text;
                Properties.Settings.Default.Save();
            }
            
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // load the settings
            txtNick.Text = Properties.Settings.Default.defaultNick;
            txtChan.Text = Properties.Settings.Default.favChannel;
            txtHost.Text = Properties.Settings.Default.host;
        }

        private void Settings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // if enter is pressed 
            {
                Close();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtNick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // if enter is pressed 
            {
                Close();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtChan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // if enter is pressed 
            {
                Close();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtHost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // if enter is pressed 
            {
                Close();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.defaultNick = Environment.UserName.ToLower();
            Properties.Settings.Default.favChannel = "main";
            Properties.Settings.Default.host = "10.2.12.147";
            Properties.Settings.Default.Save();

            txtNick.Text = Properties.Settings.Default.defaultNick;
            txtChan.Text = Properties.Settings.Default.favChannel;
            txtHost.Text = Properties.Settings.Default.host;
        }

    }
}
