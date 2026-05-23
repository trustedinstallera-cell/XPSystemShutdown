public partial class Form1 : Form
{
    private System.Windows.Forms.Timer timer;
    private int remainingSeconds = 60;  // 倒计时秒数

    public Form1()
    {
        InitializeComponent();
        
        timer = new System.Windows.Forms.Timer();
        timer.Interval = 1000;  // 1秒触发一次
        timer.Tick += Timer_Tick;
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
            Environment.Exit();
            return;
        }
        
        remainingSeconds--;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        // 格式化为 hh:mm:ss 
        labelTimer.Text = TimeSpan.FromSeconds(remainingSeconds).ToString("hh\\:mm\\:ss");
        // 或者只显示秒: labelTimer.Text = remainingSeconds.ToString();
    }
}