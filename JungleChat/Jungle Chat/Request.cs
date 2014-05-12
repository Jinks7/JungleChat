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
    public partial class Request : Form
    {

        bool correct;

        public Request(string label)
        {
            InitializeComponent();

            lblMain.Text = label;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // send information to parent form
            correct = true;
            Close();
        }

        public bool getCorrect()
        {
            return correct;
        }

        public string getValue()
        {
            return txtMain.Text.ToLower();
        }

        private void txtMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) // if enter is pressed 
            {
                correct = true;
                Close();
            }
        }

        
    }
}
