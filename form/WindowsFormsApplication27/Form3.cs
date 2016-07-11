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
    public partial class Form3 : Form
    {
        
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner; 
            string password = File.ReadAllText(f1.defaultpath + "\\" + "password.txt");
            if(password==textBox1.Text)
            {
                this.Hide();
                Form4 f4 = new Form4();
                f4.ShowDialog(f1);               
            }
            else
            {
                MessageBox.Show("密码错误");
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;
            if(File.ReadAllText(f1.defaultpath + "\\" + "password.txt")=="")
            {
                this.Close();
                Form4 f4 = new Form4();
                f4.ShowDialog(f1);
            }
        }
    }
}
