using System;
using System.Collections.Generic;
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
        public static Dictionary<int, string> Read(string filePath)
        {
            var dict = new Dictionary<int, string>();

            string fullPath = Path.GetFullPath(filePath);

            // 检查文件是否存在
            if (!File.Exists(fullPath))
            {
                string errorMsg = $"Error: Cannot find localization file.\nFilePath={fullPath}";
                //MessageBox.Show(errorMsg, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new FileNotFoundException(errorMsg, filePath);
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
                    MessageBox.Show($"Error: File is empty.\nFilePath={fullPath}", "Empty File",
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

                            if (int.TryParse(keyStr, out int id))
                            {
                                dict[id] = value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Read Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
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
                        if (int.TryParse(parts[0].Trim(), out int key))
                        {
                            result[key] = parts[1].Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取语言文件失败: {ex.Message}");
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
                string errorMsg = $"Error: Cannot find file.\nFilePath={fullPath}";
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

                    if (int.TryParse(keyStr, out int id))
                        dict[id] = value;
                }
            }

            return dict;
        }
    }
}