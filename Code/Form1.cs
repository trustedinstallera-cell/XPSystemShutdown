using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace XP_SystemShutdown
{
    public partial class Form1 : Form
    {
        private Dictionary<int, string> _lang;
        private static int remainingSeconds;
        private Timer timer = new System.Windows.Forms.Timer();

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);

        private void DisableCloseButton()
        {
            const int MF_BYCOMMAND = 0x00000000;
            const int MF_GRAYED = 0x00000001;
            const int SC_CLOSE = 0xF060;

            IntPtr hSysMenu = GetSystemMenu(this.Handle, false);
            EnableMenuItem(hSysMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }

        public Form1(Dictionary<int, string> lang)
        {
            InitializeComponent();
            DisableCloseButton();

            timer.Interval = 1000;  // 1秒触发一次
            //timer.Tick += Timer_Tick;

            _lang = lang;

            // 在这里设置文本
            if (_lang != null)
            {
                if (_lang.ContainsKey(1000))
                    this.Text = _lang[1000];

                if (_lang.ContainsKey(1001))
                    label1.Text = _lang[1001].Replace("%1", WindowsIdentity.GetCurrent().Name);

                if (_lang.ContainsKey(1002))
                    label2.Text = _lang[1002];

                if (_lang.ContainsKey(1003))
                    label3.Text = _lang[1003];
            }

            SetupNoSelectTextBox(textBox1); 
            this.textBox1.TabStop = false;
            this.textBox1.Cursor = Cursors.Default;
            this.textBox1.MouseDown += (s, e) =>
            {
                // 让焦点转移到其他控件（比如父窗体或随便一个不可见控件）
                this.ActiveControl = null;
            };
            this.label4.Text = Program.message;

            timer.Interval = 1000;  // 1秒触发一次
            timer.Tick += Timer_Tick;
            StartCountdown();
        }

        private void StartCountdown()
        {
            remainingSeconds = Program.timeout;
            UpdateDisplay();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (remainingSeconds <= 0)
            {
                timer.Stop();
                // 倒计时结束后的操作
                Environment.Exit(0);
                return;
            }

            remainingSeconds--;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // 格式化为 hh:mm:ss 
            label2.Text = _lang[1002] + TimeSpan.FromSeconds(remainingSeconds).ToString("hh\\:mm\\:ss");
            // 或者只显示秒: remainingSeconds.ToString();
        }

        private void SetupNoSelectTextBox(TextBox textBox)
        {
            textBox.GotFocus += (s, e) =>
            {
                textBox.SelectionStart = textBox.TextLength;
                textBox.SelectionLength = 0;
            };

            textBox.MouseDown += (s, e) =>
            {
                textBox.SelectionStart = textBox.TextLength;
                textBox.SelectionLength = 0;
            };

            textBox.KeyDown += (s, e) =>
            {
                // 禁止 Ctrl+A 全选
                if (e.Control && e.KeyCode == Keys.A)
                {
                    e.SuppressKeyPress = true;
                    return;
                }

                // 禁止 Shift+左右方向键选中
                if (e.Shift && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
                {
                    e.SuppressKeyPress = true;
                }
            };

            // 禁止双击选中
            textBox.DoubleClick += (s, e) =>
            {
                textBox.SelectionStart = textBox.TextLength;
                textBox.SelectionLength = 0;
            };
        }
    }
}
