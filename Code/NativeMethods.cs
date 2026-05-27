using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XP_SystemShutdown
{
    internal static class NativeMethods
    {
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
    }
}