
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XP_SystemShutdown.GUI
{
    class Utils
    {
        public static int GetTitleBarHeight(Form form)
        {
            return NativeMethods.GetSystemMetrics(NativeMethods.SM_CYCAPTION);
        }
    }

    class StringUtils
    {
        public static string TrimConfig(string str)
        {
            // Outer quotes are treated as delimiters.
            // Literal quotes inside the string should be doubled.
            if (str.Length >= 2 &&
        str.StartsWith("\"") &&
        str.EndsWith("\""))
            {
                str = str.Substring(1, str.Length - 2);
                str = str.Replace("\"\"", "\"");
            }

            return str;
        }
    }

    public static class DluConverter
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct TEXTMETRIC
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC lptm);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        // 自定义类替代元组
        public class DialogBaseUnits
        {
            public float BaseX { get; set; }
            public float BaseY { get; set; }
        }

        /// <summary>
        /// 根据对话框字体计算水平和垂直 DLU 对应的像素基数。
        /// </summary>
        public static DialogBaseUnits GetDialogBaseUnits(Font font)
        {
            using (Form tmp = new Form())
            using (Graphics g = tmp.CreateGraphics())
            {
                IntPtr hdc = g.GetHdc();
                try
                {
                    IntPtr hFont = font.ToHfont();
                    IntPtr oldFont = SelectObject(hdc, hFont);

                    TEXTMETRIC tm;
                    if (!GetTextMetrics(hdc, out tm))
                        throw new System.ComponentModel.Win32Exception();

                    SelectObject(hdc, oldFont);
                    DeleteObject(hFont);

                    float baseX = tm.tmAveCharWidth / 4.0f;
                    float baseY = tm.tmHeight / 8.0f;

                    return new DialogBaseUnits { BaseX = baseX, BaseY = baseY };
                }
                finally
                {
                    g.ReleaseHdc(hdc);
                }
            }
        }


    }
}
