using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Localization
{
    public static class IniReader
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(
            string lpAppName,
            byte[] lpReturnedString,
            int nSize,
            string lpFileName);

        /// <summary>
        /// 读取无节 INI 文件，返回 Dictionary<int, string>
        /// </summary>
        public static Dictionary<int, string> Read(string filePath)
        {
            var dict = new Dictionary<int, string>();
            
            byte[] buffer = new byte[32768];
            GetPrivateProfileSection(null, buffer, buffer.Length, Path.GetFullPath(filePath));
            string content = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
            
            if (string.IsNullOrEmpty(content)) 
                throw FileNotFoundException("Localization file not found.")
            
            foreach (string line in content.Split('\0'))
            {
                int eqIndex = line.IndexOf('=');
                if (eqIndex > 0)
                {
                    string keyStr = line.Substring(0, eqIndex);
                    string value = line.Substring(eqIndex + 1);
                    
                    if (int.TryParse(keyStr, out int id))
                        dict[id] = value;
                }
            }
            
            return dict;
        }
    }
}