using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    public partial class Channel : UserControl
    {

        public Channel()
        {
            InitializeComponent();
        }

        public RichTextBox getBox()
        {
            return output;
        }

        public ListBox getList()
        {
            return list;
        }

        public void addList(String user)
        {
            list.Items.Add(user);
            list.Update();
        }

        public void rmList(String user)
        {
            list.Items.Remove(user);
        }

        public void clrList()
        {
            list.Items.Clear();
        }

        private void output_Click(object sender, EventArgs e)
        {
            Control[] t = this.ParentForm.Controls.Find("input", false);
            t[0].Focus();
        }

        public void resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        private void Channel_Load(object sender, EventArgs e)
        {
            
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                frmMain m = (frmMain)this.ParentForm;

                // remove + symbols
                string temp = list.SelectedItem.ToString().Replace("+", "").Trim();

                m.createChannel(temp);
                m.setPriv(temp);
            }
            catch (Exception m)
            {
                
            }

        }

        private void output_TextChanged(object sender, EventArgs e)
        {

        }

        private void output_DoubleClick(object sender, EventArgs e)
        {
            output.Focus();
        }

        private void output_MouseDown(object sender, MouseEventArgs e)
        {
            output.Focus();
        }
    }
}
