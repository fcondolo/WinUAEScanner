namespace WinUAE_Scanner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_go = new System.Windows.Forms.Button();
            this.WinUAEProcessName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusbar1 = new System.Windows.Forms.Label();
            this.button_viewVars = new System.Windows.Forms.Button();
            this.button_hexview = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // button_go
            // 
            this.button_go.Location = new System.Drawing.Point(397, 46);
            this.button_go.Name = "button_go";
            this.button_go.Size = new System.Drawing.Size(103, 28);
            this.button_go.TabIndex = 0;
            this.button_go.Text = "Find Tags";
            this.button_go.UseVisualStyleBackColor = true;
            this.button_go.Click += new System.EventHandler(this.button_go_Click);
            // 
            // WinUAEProcessName
            // 
            this.WinUAEProcessName.Location = new System.Drawing.Point(142, 48);
            this.WinUAEProcessName.Name = "WinUAEProcessName";
            this.WinUAEProcessName.Size = new System.Drawing.Size(222, 22);
            this.WinUAEProcessName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Process name";
            // 
            // statusbar1
            // 
            this.statusbar1.AutoSize = true;
            this.statusbar1.Location = new System.Drawing.Point(75, 88);
            this.statusbar1.Name = "statusbar1";
            this.statusbar1.Size = new System.Drawing.Size(0, 17);
            this.statusbar1.TabIndex = 3;
            // 
            // button_viewVars
            // 
            this.button_viewVars.Location = new System.Drawing.Point(397, 146);
            this.button_viewVars.Name = "button_viewVars";
            this.button_viewVars.Size = new System.Drawing.Size(103, 28);
            this.button_viewVars.TabIndex = 4;
            this.button_viewVars.Text = "View Vars";
            this.button_viewVars.UseVisualStyleBackColor = true;
            this.button_viewVars.Click += new System.EventHandler(this.button_viewVars_Click);
            // 
            // button_hexview
            // 
            this.button_hexview.Location = new System.Drawing.Point(397, 112);
            this.button_hexview.Name = "button_hexview";
            this.button_hexview.Size = new System.Drawing.Size(103, 28);
            this.button_hexview.TabIndex = 5;
            this.button_hexview.Text = "View Hex ";
            this.button_hexview.UseVisualStyleBackColor = true;
            this.button_hexview.Click += new System.EventHandler(this.button_hexview_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(397, 180);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 28);
            this.button1.TabIndex = 6;
            this.button1.Text = "About";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(397, 214);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 28);
            this.button2.TabIndex = 7;
            this.button2.Text = "Help";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(537, 376);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_hexview);
            this.Controls.Add(this.button_viewVars);
            this.Controls.Add(this.statusbar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.WinUAEProcessName);
            this.Controls.Add(this.button_go);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "WinUAE Scanner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_go;
        private System.Windows.Forms.TextBox WinUAEProcessName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusbar1;
        private System.Windows.Forms.Button button_viewVars;
        private System.Windows.Forms.Button button_hexview;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

