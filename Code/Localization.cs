using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Localization
{
    public static class IniReader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileSection(
            string lpAppName,
            byte[] lpReturnedString,
            int nSize,
            string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        /// <summary>
        /// 读取无节 INI 文件，返回 Dictionary<int, string>
        /// </summary>
        /// 
        public static Dictionary<int, string> Fallback()
        {
            var dict = new Dictionary<int, string>
            {
                { 1000, "System Shutdown" },
                { 1001, "This system is shutting down. Please save all work in progress and log off .Any unsaved changes will be lost. This shutdown was initiated by %1" },
                { 1002, "Time before shutting down:  " },
                { 1003, "Message" },
                { 1004, "A system shutdown is in progress." },
                {
                    1005,
                    @"Usage: shutdown [-i | -l | -s | -r | -a] [-f] [-m \\computername] [-t xx] [-c ""comment""] [-d up:xx:yy]" +
                "\tNo args\t\t\tDisplay this message (same as -?)" +
                "\t-i\t\t\tDisplay GUI interface, must be the first option" +
                "\t-l\t\t\tLog off (cannot be used with -m option)\\t-s\t\t\tShutdown the computer" +
                "\t-r\t\t\tShutdown and restart the computer" +
                "\t-a\t\t\tAbort a system shutdown" +
                "\t-m \\\\computername\tRemote computer to shutdown/restart/abort" +
                "\t-t xx\t\t\tSet timeout for shutdown to xx seconds" +
                "\t-c \"comment\"\t\tShutdown comment (maximum of 127 characters)" +
                "\t-f\t\t\tForces running applications to close without warning" +
                "\t-d [u][p]:xx:yy\t\tThe reason code for the shutdown" +
                "\t\t\t\tu is the user code" +
                "\t\t\t\tp is a planned shutdown code" +
                "\t\t\t\txx is the major reason code (positive integer less than 256)" +
                "\t\t\t\tyy is the minor reason code (positive integer less than 65536) \r\n"
                },
                { 1006, "days" },
                { 1007, "The parameter is incorrect." }
            };
            return dict;

        }

        public static Dictionary<int, string> Read(string filePath)
        {
            var dict = new Dictionary<int, string>();
            string fullPath = null;
            try
            {
                fullPath = Path.GetFullPath(filePath);
            }
            catch (Exception)
            {
                return Fallback();
            }

            // 检查文件是否存在
            if (!File.Exists(fullPath))
            {
                string errorMsg = string.Format("Error: Cannot find localization file.\nFilePath={0}", fullPath);
                //MessageBox.Show(errorMsg, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw new FileNotFoundException(errorMsg, filePath);
                Debug.WriteLine(errorMsg);
                Debug.WriteLine("Using fallback");
                return Fallback();
            }

            // 对于无节的 INI 文件，直接读取文件内容更可靠
            try
            {
                // 尝试多种编码格式
                string content = File.ReadAllText(fullPath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = File.ReadAllText(fullPath, Encoding.Default);
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    MessageBox.Show(string.Format("Error: File is empty.\nFilePath={0}", fullPath), "Empty File",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return dict;
                }

                // 按行解析
                using (var reader = new StringReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        // 跳过空行和注释
                        if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("#"))
                            continue;

                        int eqIndex = line.IndexOf('=');
                        if (eqIndex > 0)
                        {
                            string keyStr = line.Substring(0, eqIndex).Trim();
                            string value = line.Substring(eqIndex + 1).Trim();

                            // 处理可能的值中的转义字符
                            value = value.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");

                            int id;
                            if (int.TryParse(keyStr, out id))
                            {
                                dict[id] = value;
                            }
                        }
                    }
                }
            }
            //#if DEBUG
#if false
            catch (Exception _)
            {

                MessageBox.Show($"Error reading file: {_.Message}", "Read Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
#else
            catch (Exception)
            {
                return Fallback();
#endif
            }

            return dict;
        }

        public static Dictionary<int, string> Read(string filePath, Encoding encoding)
        {
            var result = new Dictionary<int, string>();

            if (!File.Exists(filePath))
                return result;

            try
            {
                // 使用指定的编码读取所有行
                var lines = File.ReadAllLines(filePath, encoding);

                foreach (var line in lines)
                {
                    // 跳过空行和注释行（; 或 # 开头）
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith(";") || line.TrimStart().StartsWith("#"))
                        continue;

                    // 解析 key=value 格式
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        int key;
                        if (int.TryParse(parts[0].Trim(), out key))
                        {
                            result[key] = parts[1].Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("读取语言文件失败: {0}", ex.Message));
            }

            return result;
        }

        /// <summary>
        /// 使用 Windows API 读取（适用于有节的 INI 文件）
        /// </summary>
        public static Dictionary<int, string> ReadWithSection(string filePath, string sectionName)
        {
            var dict = new Dictionary<int, string>();

            string fullPath = Path.GetFullPath(filePath);

            if (!File.Exists(fullPath))
            {
                string errorMsg = string.Format("Error: Cannot find file.{0}FilePath={1}", Environment.NewLine, fullPath);
                MessageBox.Show(errorMsg, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new FileNotFoundException(errorMsg, filePath);
            }

            byte[] buffer = new byte[32768];
            int bytesRead = GetPrivateProfileSection(sectionName, buffer, buffer.Length, fullPath);

            if (bytesRead == 0)
            {
                return dict;
            }

            // 只读取实际返回的数据长度
            string content = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            foreach (string line in content.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int eqIndex = line.IndexOf('=');
                if (eqIndex > 0)
                {
                    string keyStr = line.Substring(0, eqIndex).Trim();
                    string value = line.Substring(eqIndex + 1).Trim();

                    int id;
                    if (int.TryParse(keyStr, out id))
                    {
                        dict[id] = value;
                    }
                }
            }

            return dict;
        }
    }
}