using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XP_SystemShutdown
{
    internal static class NativeMethods
    {
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_SYSMENU = 0x00080000;

        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int WS_MINIMIZEBOX = 0x00020000;

        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_TOPMOST = 0x00000008;

        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_FRAMECHANGED = 0x0020;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(
            IntPtr hWnd,
            int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(
            IntPtr hWnd,
            int nIndex,
            int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int STD_ERROR_HANDLE = -12;
        private const int STD_INPUT_HANDLE = -10;

        private const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

        public static void InitConsole()
        {

            // 尝试附加到父控制台（例如 cmd）

            if (!AttachConsole(ATTACH_PARENT_PROCESS))
            {
                // 没有父控制台则创建新控制台
                AllocConsole();
            }
            else
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }

            // 重定向标准输出
            var stdout = Console.OpenStandardOutput();
            var writer = new StreamWriter(stdout)
            {
                AutoFlush = true
            };
            Console.SetOut(writer);

            // 错误输出
            var stderr = Console.OpenStandardError();
            var errWriter = new StreamWriter(stderr)
            {
                AutoFlush = true
            };
            Console.SetError(errWriter);
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(
    IntPtr hWnd,
    out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        // 系统度量常量
        public const int SM_CYCAPTION = 4;     // 标题栏高度
        public const int SM_CXSIZEFRAME = 32;  // 可调整边框宽度
        public const int SM_CYSIZEFRAME = 33;  // 可调整边框高度
        public const int SM_CXPADDEDBORDER = 92; // 填充边框（Win7+）

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(
            IntPtr hWnd,
            bool bRevert);

        [DllImport("user32.dll")]
        public static extern bool RemoveMenu(
            IntPtr hMenu,
            int uPosition,
            uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool DeleteMenu(
            IntPtr hMenu,
            int uPosition,
            uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam);

    }
}