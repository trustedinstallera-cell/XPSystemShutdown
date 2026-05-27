
namespace XP_SystemShutdown.GUI
{
    class Utils
    {
        public static string trimConfig(string str)
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
}
