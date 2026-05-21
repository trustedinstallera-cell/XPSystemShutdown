using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace XP_SystemShutdown
{
    public static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern uint GetEnvironmentVariable(string lpName, StringBuilder lpBuffer, uint nSize);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool SetEnvironmentVariable(string lpName, string lpValue);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam,
            uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        public static string message = "";
        public static int timeout = -1;
        public static Dictionary<int, string> _lang = Localization.IniReader.Read(Path.Combine(Application.StartupPath, "lang.ini"));

        [STAThread]
        public static void Main(string[] args)
        {
            string originalArgs = string.Join(" ", args);
            string system32 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32");

            // 如果没有参数，直接调用系统shutdown
            if (args.Length == 0)
            {
                var startInfo1 = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{Path.Combine(system32, "shutdown.exe")}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process.Start(startInfo1);
                Environment.Exit(0);
            }

            // 先解析参数
            bool isAbort = false;
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i].ToLower() == "/t" || args[i].ToLower() == "-t") && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out timeout);
                }

                if ((args[i].ToLower() == "/a" || args[i].ToLower() == "-a"))
                {
                    isAbort = true;
                }

                if ((args[i].ToLower() == "/c" || args[i].ToLower() == "-c") && i + 1 < args.Length)
                {
                    message = args[i + 1];
                }
            }

            // 如果是取消关机，检查是否有其他实例
            if (isAbort)
            {
                string currentProcessName = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(currentProcessName);

                if (processes.Length > 1)
                {
                    // 终止其他实例
                    foreach (var process in processes)
                    {
                        if (process.Id != Process.GetCurrentProcess().Id)
                        {
                            try
                            {
                                process.Kill();
                                process.WaitForExit(3000);
                            }
                            catch { }
                        }
                    }
                }

                // 调用系统shutdown /a
                var abortInfo = new ProcessStartInfo(Path.Combine(system32, "shutdown.exe"), "/a")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process abortProcess = Process.Start(abortInfo))
                {
                    abortProcess.WaitForExit();
                }

                Environment.Exit(0);
            }

            // 调用系统shutdown.exe
            var startInfo = new ProcessStartInfo(Path.Combine(system32, "shutdown.exe"), originalArgs)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process current = Process.Start(startInfo))
            {
                string output = current.StandardOutput.ReadToEnd();
                string error = current.StandardError.ReadToEnd();
                Console.WriteLine(output);
                Console.WriteLine(error);
                current.WaitForExit();

                if (timeout > 0)
                {
                    switch (current.ExitCode)
                    {
                        case 0: // 成功
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            XP_SystemShutdown.Form1 Form = new XP_SystemShutdown.Form1(_lang);
                            Application.Run(Form);
                            break;

                        case 1115: // 系统关机已计划
                            MessageBox.Show(_lang.ContainsKey(1003) ? _lang[1003] : "系统关机已被计划，无法重复设置。",
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;

                        case 1190: // 已计划关机
                            if (string.IsNullOrEmpty(message))
                            {
                                message = _lang.ContainsKey(1004) ? _lang[1004] : "系统已计划关机";
                            }
                            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;

                        case 1116: // 无法关闭
                            // 不做任何事
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}