using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WinUAE_Scanner
{
    public partial class Form1 : Form
    {
        public static Form1 mainForm;

        public Form1()
        {
            mainForm = this;
            InitializeComponent();
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                string pname = theprocess.ProcessName;
                pname.ToLower();
                if (pname.Contains("winuae"))
                {
                    WinUAEProcessName.Text = theprocess.ProcessName;
                    break;
                }
            }
        }

        private void button_go_Click(object sender, EventArgs e)
        {
            if (Scanner.FindTags(WinUAEProcessName.Text))
            {
                statusbar1.Text = "Tag Found!";
                button_viewVars.Enabled = true;
                button_hexview.Enabled = true;
            } else
            {
                button_viewVars.Enabled = false;
                button_hexview.Enabled = false;
                statusbar1.Text = "No tag found yet. Please retry";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button_viewVars.Enabled = false;
            button_hexview.Enabled = false;
        }

        private void button_hexview_Click(object sender, EventArgs e)
        {
            HexViewer hv = new HexViewer();
        }

        private void button_viewVars_Click(object sender, EventArgs e)
        {
            VariableView view = new VariableView();
            view.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Scanner.FindROM(WinUAEProcessName.Text))
            {
                if (Scanner.BASE_Index >= 0)
                    statusbar1.Text = "ROM Found!";
                else
                    statusbar1.Text = "ROM Found, but wrong index";
            }
            else
            {
                statusbar1.Text = "ROM not found";
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.deadliners.net/winuaescannerhelp.html");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
