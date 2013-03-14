using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CTool.CUtility
{
    public class Util
    {
        /// <summary>
        /// 一个字符串是否匹配正则，不区分大小写
        /// </summary>
        /// <param name="pat">正则表达式</param>
        /// <param name="targetStr">要匹配的字符串</param>
        /// <param name="regexOptionEnum">匹配属性</param>
        /// <returns></returns>
        public static bool RegExp(string pat, string targetStr, RegexOptions regexOptionEnum = RegexOptions.IgnoreCase)
        {
            Regex reg = new Regex(pat, regexOptionEnum);
            Match match = reg.Match(targetStr);
            if (match.Success)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 传入消息进行弹窗提示，
        /// </summary>
        /// <param name="msg">要提示的消息</param>
        /// <param name="formTitle">提示框标题文字，传null默认标题为"提示"</param>
        /// <param name="confirm">如果传入true，即可获取返回的结果(Yes|No)，不传默认返回(OK)</param>
        /// <returns></returns>
        public static DialogResult ShowMsg(string msg, string formTitle, bool confirm = false)
        {
            if (confirm)
            {
                return MessageBox.Show(msg, formTitle ?? "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                return MessageBox.Show(msg, formTitle ?? "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        /// <summary>
        /// 设置鼠标状态，0：等待，1：恢复默认
        /// </summary>
        /// <param name="frm">传this</param>
        /// <param name="i"></param>
        public static void SetCursorStatue(Form frm, int i)
        {
            frm.Cursor = i == 0 ? Cursors.WaitCursor : Cursors.Default;
        }
        /// <summary>
        /// 获取config文件的属性值<AppSettings>节点
        /// </summary>
        /// <param name="key">key属性</param>
        /// <returns></returns>
        public static string GetConfig(string key)
        {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            return value;
        }

        /// <summary>        
        /// 取得HTML中所有图片的 URL。        
        /// </summary>        
        /// <param name="sHtmlText">HTML代码</param>        
        /// <returns>图片的URL列表</returns>        
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签         
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            //Regex regImg = new Regex(@"<img\b[^>]*?\bsrc[\s]*=[\s]*[""']?[\s]*(?<imgUrl>[^""'>]*)[^>]*?/?[\s]*>", RegexOptions.IgnoreCase);
            // 搜索匹配的字符串           
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];
            // 取得匹配项列表          
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;

        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="msg">消息文本</param>
        public static void WriteLog(string msg)
        {
            DateTime dt = DateTime.Now;
            File.AppendAllText(dt.ToString("yyyy-MM-dd") + ".log", dt.ToString("HH:mm:ss") + "\t" + msg + "\r\n");
        }

        #region 加密、解密字符串
        const string MKey = "CMYKEY";
        /// <summary>
        /// 生成MD5摘要
        /// </summary>
        static byte[] MakeMD5(byte[] original)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd5.ComputeHash(original);
            hashmd5 = null;
            return keyhash;
        }
        /// <summary>
        /// 使用给定密钥加密
        /// </summary>
        public static string Encrypt(string pStr)
        {
            byte[] original, key;
            original = Encoding.Default.GetBytes(pStr);
            key = Encoding.Default.GetBytes(MKey);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;

            return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length));
        }
        /// <summary>
        /// 使用给定密钥解密数据
        /// </summary>
        public static string Decrypt(string pStr)
        {
            byte[] encrypted, key;
            encrypted = Convert.FromBase64String(pStr);
            key = Encoding.Default.GetBytes(MKey);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD5(key);
            des.Mode = CipherMode.ECB;

            return Encoding.Default.GetString(des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length));
        }
        #endregion 加密、解密字符串End
    }
}
