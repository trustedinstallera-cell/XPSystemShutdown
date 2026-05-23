using System;
using System.Linq;
using System.Collections.Generic;

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
    public static int timeout = 30;
    public static Dictionary < int,
    string > lang = IniReader.Read(Path.Combine(Directory.GetCurrentDirectory(), "lang.ini"));

    public static void Main(string[] args) {

        for (int i = 0; i < args.Length; i++) {
            if ((args[i] == "/t" || args[i] == "-t") && i + 1 < args.Length)
                int.TryParse(args[i + 1], out timeout);
        }

        // Dispach message to shutdown.exe
        string originalArgs = string.Join(" ", args);
        string system32 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32");

        var startInfo = new ProcessStartInfo((Path.Combine(system32, "shutdown.exe"), originalArgs) {
            UseShellExecute = false, // 必须设为 false
            CreateNoWindow = true, // 不创建窗口
            RedirectStandardOutput = true, // 重定向输出
            RedirectStandardError = true // 重定向错误（可选）
        };

            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.WriteLine(process.StandardError.ReadToEnd());
            process.WaitForExit();

            if (timeout > 0) {
                switch (process.ExitCode) {
                    case 0:
                        Form Form1 = new Form1();
                        for (int i = 0; i < args.Length - 1; i++) {
                            if (string.Equals(args[i], "-c", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(args[i], "/c", StringComparison.OrdinalIgnoreCase
                                ) {
                                    message = args[i+1];
                                }
                            }
                            Form.Run(Form1);
                            break;
                        case 1115:
                            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) {
                                foreach (Process process in Process.GetProcessesByName(current.ProcessName)) {
                                    if (process.Id != current.Id) {
                                        process.Kill();
                                    }
                                }
                            }
                            return;
                        case 1190:
                            message = lang[1004]
                            break;
                        case 1116:
                            break;
                        default:
                            break;
                        }
                }

            }

            public static string SetText(Form form) {
                form.text = lang[1000];
                label1.text = lang[1001].replace(%1, WindowsIdentity.GetCurrent().Name);
                label2.text = lang[1002];
            }
        }