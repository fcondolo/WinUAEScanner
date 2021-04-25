using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace WinUAE_Scanner
{
    public class HexViewer : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkbox_autoRefresh;
//        private System.Windows.Forms.Button button2;
        private System.ComponentModel.Design.ByteViewer byteviewer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private bool pauseBackgroundWorker = true;

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {

            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "FormClosing Event");
        }

        private void formClosingCallback(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                pauseBackgroundWorker = true;
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            while (worker.CancellationPending != true)
            {
                if (pauseBackgroundWorker == false)
                {
                    Scanner.refreshLastZone();
                    try
                    {
                        byteviewer.Invoke((MethodInvoker)delegate {
                            byteviewer.SetBytes(Scanner.ScannerBuffer);
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("backgroundWorker1_DoWork: " + ex.Message);
                    }
                }
            }
        }

        private void disposed(object sender, System.EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            pauseBackgroundWorker = true;
        }

        public HexViewer()
        {
            // Initialize the controls other than the ByteViewer.
            InitializeForm();

            // Initialize the ByteViewer.
            byteviewer = new ByteViewer();
            byteviewer.Disposed += new System.EventHandler(this.disposed);
            byteviewer.Location = new Point(8, 46);
            byteviewer.Size = new Size(600, 338);
            byteviewer.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            byteviewer.SetBytes(Scanner.ScannerBuffer);
            this.Controls.Add(byteviewer);
            this.Show();

            this.FormClosing += new FormClosingEventHandler(formClosingCallback);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = false;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            pauseBackgroundWorker = true;
        }

        private void onRefreshBut(object sender, EventArgs e)
        {
            Scanner.refreshLastZone();
            byteviewer.SetBytes(Scanner.ScannerBuffer);
        }

        // Show a file selection dialog and cues the byte viewer to 
        // load the data in a selected file.
        private void loadBytesFromFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            byteviewer.SetFile(ofd.FileName);
        }

        private void onAutoRefresh(object sender, EventArgs e)
        {
            bool auto = checkbox_autoRefresh.Checked;
            if (auto)
            {
                if (backgroundWorker1.IsBusy != true)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                pauseBackgroundWorker = false;
            }
            else
            {
                pauseBackgroundWorker = true;
            }
        }

        // Changes the display mode of the byte viewer according to the 
        // Text property of the RadioButton sender control.
        private void changeByteMode(object sender, EventArgs e)
        {
            System.Windows.Forms.RadioButton rbutton =
                (System.Windows.Forms.RadioButton)sender;

            DisplayMode mode;
            switch (rbutton.Text)
            {
                case "ANSI":
                    mode = DisplayMode.Ansi;
                    break;
                case "Hex":
                    mode = DisplayMode.Hexdump;
                    break;
                case "Unicode":
                    mode = DisplayMode.Unicode;
                    break;
                default:
                    mode = DisplayMode.Auto;
                    break;
            }

            // Sets the display mode.
            byteviewer.SetDisplayMode(mode);
        }

        private void InitializeForm()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(680, 440);
            this.MinimumSize = new System.Drawing.Size(660, 400);
            this.Size = new System.Drawing.Size(680, 440);
            this.Name = "Byte Viewer Form";
            this.Text = "WinUAE Scanner - Hex View";
            this.Icon = Form1.mainForm.Icon;

            this.button1 = new System.Windows.Forms.Button();
            this.button1.Location = new System.Drawing.Point(8, 8);
            this.button1.Size = new System.Drawing.Size(150, 23);
            this.button1.Name = "button1";
            this.button1.Text = "Refresh Once";
            this.button1.TabIndex = 0;
            this.button1.Click += new EventHandler(this.onRefreshBut);
            this.Controls.Add(this.button1);

            this.checkbox_autoRefresh = new System.Windows.Forms.CheckBox();
            this.checkbox_autoRefresh.Checked = false;
            this.checkbox_autoRefresh.Location = new System.Drawing.Point(198, 8);
            this.checkbox_autoRefresh.Size = new System.Drawing.Size(190, 23);
            this.checkbox_autoRefresh.Name = "checkbox_autoRefresh";
            this.checkbox_autoRefresh.Text = "Auto Refresh";
            this.checkbox_autoRefresh.Click += new EventHandler(this.onAutoRefresh);
            this.checkbox_autoRefresh.TabIndex = 1;
            this.Controls.Add(this.checkbox_autoRefresh);


            /*
                        this.button2 = new System.Windows.Forms.Button();
                        this.button2.Location = new System.Drawing.Point(198, 8);
                        this.button2.Size = new System.Drawing.Size(190, 23);
                        this.button2.Name = "button2";
                        this.button2.Text = "Clear Bytes";
                        this.button2.Click += new EventHandler(this.clearBytes);
                        this.button2.TabIndex = 1;

                        this.Controls.Add(this.button2);
                        */
            System.Windows.Forms.GroupBox group = new System.Windows.Forms.GroupBox();
            group.Location = new Point(418, 3);
            group.Size = new Size(220, 36);
            group.Text = "Display Mode";
            this.Controls.Add(group);

            System.Windows.Forms.RadioButton rbutton1 = new System.Windows.Forms.RadioButton();
            rbutton1.Location = new Point(6, 15);
            rbutton1.Size = new Size(46, 16);
            rbutton1.Text = "Auto";
            rbutton1.Checked = true;
            rbutton1.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton1);

            System.Windows.Forms.RadioButton rbutton2 = new System.Windows.Forms.RadioButton();
            rbutton2.Location = new Point(54, 15);
            rbutton2.Size = new Size(50, 16);
            rbutton2.Text = "ANSI";
            rbutton2.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton2);

            System.Windows.Forms.RadioButton rbutton3 = new System.Windows.Forms.RadioButton();
            rbutton3.Location = new Point(106, 15);
            rbutton3.Size = new Size(46, 16);
            rbutton3.Text = "Hex";
            rbutton3.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton3);

            System.Windows.Forms.RadioButton rbutton4 = new System.Windows.Forms.RadioButton();
            rbutton4.Location = new Point(152, 15);
            rbutton4.Size = new Size(64, 16);
            rbutton4.Text = "Unicode";
            rbutton4.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton4);
            this.ResumeLayout(false);
        }
    }
}
