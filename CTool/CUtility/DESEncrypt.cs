using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTool.CUtility
{
    public class DESEncrypt
    {
        #region ========加密========

        /// <summary>  
        /// 加密  
        /// </summary>  
        /// <param name="Text">要加密的字符串</param>  
        /// <returns></returns>  
        public static string Encrypt(string Text)
        {
            if (string.IsNullOrEmpty(Text))
                throw new ArgumentNullException("Text");

            return Encrypt(Text, "cn100com");
        }

        /// <summary>   
        /// 加密数据   
        /// </summary>   
        /// <param name="Text"></param>   
        /// <param name="sKey"></param>   
        /// <returns></returns>   
        private static string Encrypt(string Text, string sKey)
        {
            System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();

            byte[] inputByteArray;

            inputByteArray = Encoding.Default.GetBytes(Text);

            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            foreach (byte b in ms.ToArray())
            {

                ret.AppendFormat("{0:X2}", b);

            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========

        /// <summary>  
        /// 解密  
        /// </summary>  
        /// <param name="Text">要解密的字符串</param>  
        /// <returns></returns>  
        public static string Decrypt(string Text)
        {
            if (string.IsNullOrEmpty(Text))
                throw new ArgumentNullException("Text");

            return Decrypt(Text, "cn100com");
        }

        /// <summary>   
        /// 解密数据   
        /// </summary>   
        /// <param name="Text"></param>   
        /// <param name="sKey"></param>   
        /// <returns></returns>   
        private static string Decrypt(string Text, string sKey)
        {

            try
            {
                System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();

                int len;

                len = Text.Length / 2;

                byte[] inputByteArray = new byte[len];

                int x, i;

                for (x = 0; x < len; x++)
                {

                    i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);

                    inputByteArray[x] = (byte)i;

                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

                des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);

                cs.FlushFinalBlock();

                return Encoding.Default.GetString(ms.ToArray());

            }
            catch
            {
                return Text;
            }
        }
        #endregion
    }

}
