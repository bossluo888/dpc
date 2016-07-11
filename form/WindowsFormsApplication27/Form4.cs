using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace dpc
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
                       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                MessageBox.Show("密码不能为空");
            else
            {
                Form1 f1 = (Form1)this.Owner;
                StreamWriter MyWriter = new StreamWriter(f1.defaultpath + "\\" + "password.txt", false, Encoding.UTF8);
                MyWriter.Write(textBox1.Text);
                MyWriter.Flush();
                MyWriter.Close();
                this.Hide();
            }
        }
    }
}
