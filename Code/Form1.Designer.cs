using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace XP_SystemShutdown
{
    partial class Form1 : Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();

            // 运行时初始化
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                SetupPictureBoxIcon();
            }
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SETCURSOR = 0x0020;
            const int WM_NCHITTEST = 0x0084;
            const int WM_SYSCOMMAND = 0x0112;

            const int SC_SIZE = 0xF000;
            const int SC_MAXIMIZE = 0xF030;

            const int HTLEFT = 10;
            //const int HTRIGHT = 11;
            //const int HTTOP = 12;
            //const int HTTOPLEFT = 13;
           // const int HTTOPRIGHT = 14;
            //const int HTBOTTOM = 15;
            //const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            const int HTCAPTION = 2;

            switch (m.Msg)
            {
                case WM_SETCURSOR:
                    base.WndProc(ref m);
                    int hitTest = (int)m.LParam & 0xFFFF;
                    if (hitTest >= HTLEFT && hitTest <= HTBOTTOMRIGHT)
                    {
                        Cursor.Current = Cursors.Arrow;
                        m.Result = (IntPtr)1;
                        return;
                    }
                    break;

                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    int ht = m.Result.ToInt32();
                    if (ht >= HTLEFT && ht <= HTBOTTOMRIGHT)
                    {
                        m.Result = (IntPtr)HTCAPTION;
                    }
                    return;

                case WM_SYSCOMMAND:
                    int cmd = m.WParam.ToInt32() & 0xFFF0;
                    if (cmd == SC_SIZE || cmd == SC_MAXIMIZE)
                    {
                        return;
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void SetupPictureBoxIcon()
        {
            bool fallbackIcon = true;
            Icon errorIcon;
            string configPath = Path.Combine(Application.StartupPath, "config.ini");

            if (File.Exists(configPath))
            {
                try
                {
                    Dictionary<int, string> config = Localization.IniReader.Read(configPath);
                    if (config.ContainsKey(2000))
                    {
                        string iconPath = config[2000].Trim('"');
                        if (!Path.IsPathRooted(iconPath))
                            iconPath = Path.Combine(Application.StartupPath, iconPath);

                        if (File.Exists(iconPath))
                        {
                            fallbackIcon = false;
                            errorIcon = new Icon(iconPath);
                            this.pictureBox1.Image = errorIcon.ToBitmap();
                        }
                    }
                }
                catch (Exception) { }
            }

            if (fallbackIcon)
            {
                errorIcon = SystemIcons.Error;
                this.pictureBox1.Image = new Icon(errorIcon, new Size(32, 32)).ToBitmap();
            }
        }

        #region Windows 窗体设计器生成的代码

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(23, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(75, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(295, 66);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox1.Location = new System.Drawing.Point(78, 158);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(292, 108);
            this.textBox1.TabIndex = 3;
            this.textBox1.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(88, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "label3";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(78, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(292, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(88, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(278, 98);
            this.label4.TabIndex = 6;
            this.label4.Text = "label4";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 278);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 320);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 320);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Form1";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
    }
}