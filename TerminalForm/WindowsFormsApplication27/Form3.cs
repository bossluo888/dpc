using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace dpc
{
    public partial class Form3 : Form
    {
        System.Timers.Timer t = new System.Timers.Timer(100);
        public Form3()
        {           
            InitializeComponent();        
        }
    
        private void button1_Click(object sender, EventArgs e)
        {
           Form1 f1 = (Form1)this.Owner;
           f1.password = textBox1.Text;
           f1.clientSocket.Send(Encoding.ASCII.GetBytes(textBox1.Text));
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            t.Elapsed += new System.Timers.ElapsedEventHandler(refresh);
            t.Start();
        }
        public void refresh(object source, System.Timers.ElapsedEventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;
            if (f1.right)
            {
                this.Hide();
                t.Stop();
            }
        }
    }
}
