using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
            string originalArgs = string.Join(" ", Array.ConvertAll(args, arg =>
            {
                if (arg.Contains(" "))
                    return "\"" + arg + "\"";
                return arg;
            }));
            string system32 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32");

            if (args.Length == 0)
            {
                ConsoleCtrl.Print(_lang[1005]);
                //Thread.Sleep(300);
                Environment.Exit(0);
            }

            // 先解析参数
            bool isAbort = false;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();

                if ((arg == "/t" || arg == "-t") && i + 1 < args.Length)
                {
                    int parsedTimeout;
                    if (!int.TryParse(args[i + 1], out parsedTimeout) || parsedTimeout < 0 || parsedTimeout > 10 * 365 * 24 * 60 * 60) // 10 years in seconds
                    {
                        isAbort = true;
                        NativeMethods.InitConsole();
                        Console.WriteLine(_lang.ContainsKey(1007) ? _lang[1007] : "Invalid parameter.");
                        Environment.Exit(2);
                    }
                    timeout = parsedTimeout;
                    i++; // 跳过参数值
                }
                else if (arg == "/a" || arg == "-a")
                {
                    isAbort = true;
                }
                else if ((arg == "/c" || arg == "-c") && i + 1 < args.Length)
                {
                    message = args[i + 1];
                    i++; // 跳过参数值
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
                    CreateNoWindow = true,           // 不创建窗口
                    WindowStyle = ProcessWindowStyle.Hidden,  // 窗口样式隐藏
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process abortProcess = Process.Start(abortInfo))
                {
                    // 先读取输出和错误（避免缓冲区满导致死锁）
                    string output = abortProcess.StandardOutput.ReadToEnd();
                    string error = abortProcess.StandardError.ReadToEnd();

                    abortProcess.WaitForExit();

                    // shutdown 的错误信息通常输出到 StandardError
                    if (!string.IsNullOrEmpty(error))
                    {
                        ConsoleCtrl.Print(error);  // 输出："没有任何进行中的关机过程，所以无法中止系统关机。(1116)"
                    }
                    else if (!string.IsNullOrEmpty(output))
                    {
                        ConsoleCtrl.Print(output);
                    }

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
                Console.Write(output);
                Console.Write(error);
                current.WaitForExit();

                if (timeout > 0)
                {
                    switch (current.ExitCode)
                    {
                        case 0: // 成功
                            //Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Form1 Form = new XP_SystemShutdown.Form1(_lang);
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



