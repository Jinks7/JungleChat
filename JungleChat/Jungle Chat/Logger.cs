using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jungle_Chat
{
    class Logger
    {
        RichTextBox box;
        static Color color;

        public Logger(RichTextBox l)
        {
            box = l;
            color = Color.Black;
            box.SelectionColor = color;
        }

        public void write(string mesg)
        {
            try
            {
                MethodInvoker m = delegate
                {
                    box.AppendText(mesg);
                    box.Update();
                    box.ScrollToCaret();
                };
                box.BeginInvoke(m);
            }
            catch (Exception e)
            {

            }
        }

        public void writeln(string mesg)
        {
            try
            {
                MethodInvoker m = delegate
                {
                    box.AppendText(mesg + "\n");
                    box.Update();
                    box.ScrollToCaret();
                };
                box.BeginInvoke(m);
            }
            catch (Exception e)
            {

            }
            
        }

        public void write(string mesg, Color c)
        {
            try
            {
                MethodInvoker m = delegate
                {
                    box.SelectionStart = box.TextLength;
                    box.SelectionColor = c;
                    box.AppendText(mesg);
                    box.SelectionColor = color;
                    box.Update();
                    box.ScrollToCaret();
                };
                box.BeginInvoke(m);
            }
            catch (Exception e)
            {

            }
        }

        public void writeln(string mesg, Color c)
        {
            try
            {
                MethodInvoker m = delegate
                {
                    box.SelectionStart = box.TextLength;
                    box.SelectionColor = c;
                    box.AppendText(mesg + "\n");
                    box.SelectionColor = color;
                    box.Update();
                    box.ScrollToCaret();
                };
                box.BeginInvoke(m);
            }
            catch (Exception e)
            {

            }
        }

        public void setColor(Color c)
        {
            color = c;
        }
    }
}
