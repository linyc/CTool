using System.Text;
using System.Runtime.InteropServices;

namespace CTool.CIO
{
    /// <summary>
    /// 操作ini文件
    /// </summary>
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern void WritePrivateProfileString(string section, string key, StringBuilder retVal, string filePath);
        /// <summary>
        /// 对ini文件进行读操作的函数
        /// </summary>
        /// <param name="Section">[xxx]</param>
        /// <param name="Key">xxx=的xxx</param>
        /// <returns></returns>
        public static string IniRead(string Section, string Key, string iniPath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
            return temp.ToString();
        }
        /// <summary>
        /// 对ini文件进行写操作的函数
        /// </summary>
        /// <param name="Section">[xxx]</param>
        /// <param name="Key">xxx=的xxx</param>
        /// <returns></returns>
        public static void IniWrite(string Section, string Key, string retVal, string iniPath)
        {
            StringBuilder temp = new StringBuilder(retVal);
            WritePrivateProfileString(Section, Key, temp, iniPath);
        }
    }
}
