using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace XP_SystemShutdown
{
    class ConsoleCtrl
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        // 获取控制台光标位置
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD dwCursorPosition);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        private const int STD_OUTPUT_HANDLE = -11;

        [StructLayout(LayoutKind.Sequential)]
        struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            // ... 省略其他不用的字段
        }

        public static void Print(string args)
        {
            if (!AttachConsole(ATTACH_PARENT_PROCESS))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                AllocConsole();
                Console.WriteLine(Program._lang[1005]);
                return;
            }

            IntPtr consoleHwnd = GetConsoleWindow();

            if (consoleHwnd != IntPtr.Zero)
            {
                IntPtr originalForeground = GetForegroundWindow();
                SetForegroundWindow(consoleHwnd);
                Thread.Sleep(100); // 等待焦点切换完成

                string promptPrefix = Environment.CurrentDirectory + ">";

                // 直接输出，不换行（光标已经在正确位置）
                // 使用 \r 回到行首，然后输出内容覆盖当前行
                Console.Write("\r"); // 回到行首

                // 清除当前行
                Console.Write(new string(' ', Console.BufferWidth - 1));
                Console.Write("\r"); // 再次回到行首

                // 输出信息
                Console.WriteLine(args);

                // 输出最终的提示符
                Console.Write(promptPrefix);

                if (originalForeground != IntPtr.Zero && originalForeground != consoleHwnd)
                {
                    SetForegroundWindow(originalForeground);
                }
            }
        }
    }
}
