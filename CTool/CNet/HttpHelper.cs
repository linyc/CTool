using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace CTool.CNet
{
    public class HttpHelper
    {
        /// <summary>
        /// 模拟Http请求并返回取得的数据(application/x-www-form-urlencoded)
        /// </summary>
        /// <param name="uri">请求路径</param>
        /// <param name="keyValues">发送的键值对数据字典</param>
        /// <param name="encoding">发送请求的编码</param>
        /// <param name="type">POST|GET</param>
        /// <returns></returns>
        public string SendHttpRequest(string uri, Dictionary<string, string> keyValues, Encoding encoding, string type)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentNullException("uri");

            string postData = null;
            if (keyValues != null && keyValues.Count > 0)
            {
                postData = string.Join("&", (from kv in keyValues
                                             let item = kv.Key + "=" + HttpUtility.UrlEncode(kv.Value)
                                             select item).ToArray()
                             );
            }

            if (encoding == null)
                encoding = Encoding.UTF8;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = type.ToUpper();
            request.ContentType = "application/x-www-form-urlencoded; charset=" + encoding.WebName;

            if (postData != null)
            {
                byte[] bs = encoding.GetBytes(postData);
                using (Stream s = request.GetRequestStream())
                {
                    s.Write(bs, 0, bs.Length);
                }
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                return "Err:" + ex.Message;
            }
        }

        /// <summary>
        /// 模拟Http请求并返回取得的数据(multipart/form-data)
        /// </summary>
        /// <param name="uri">请求路径</param>
        /// <param name="keyValues">发送的键值对数据字典</param>
        /// <param name="encoding">发送请求的编码</param>
        /// <returns></returns>
        public string SendHttpRequest(string uri, Dictionary<string, string> keyValues, Dictionary<string, string> files, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentNullException("uri");

            if (encoding == null)
                encoding = Encoding.UTF8;

            if (files == null || files.Count == 0)
                return SendHttpRequest(uri, keyValues, encoding, "POST");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";

            //数据库的分隔标记，设置请求头
            string boundary = "---------------------------" + Guid.NewGuid().ToString("N");

            //请求体多了前面的 "--"
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            request.ContentType = "multipart/form-data; boundary=" + boundary;

            using (Stream s = request.GetRequestStream())
            {
                if (keyValues != null && keyValues.Count > 0)
                {
                    foreach (var kv in keyValues)
                    {
                        s.Write(boundaryBytes, 0, boundaryBytes.Length);

                        string str = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", kv.Key, kv.Value);
                        byte[] bsdata = encoding.GetBytes(str);
                        s.Write(bsdata, 0, bsdata.Length);
                    }
                }

                foreach (var file in files)
                {
                    s.Write(boundaryBytes, 0, boundaryBytes.Length);

                    string str = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n{2}"
                        , file.Key, Path.GetFileName(file.Value), file.Value);
                    byte[] bsfile = encoding.GetBytes(str);
                    s.Write(bsfile, 0, bsfile.Length);
                }

                boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                s.Write(boundaryBytes, 0, boundaryBytes.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="keyValues">该cookie里的键值对</param>
        /// <param name="expries">cookie过期时间,不传参数默认一天后过期</param>
        /// <param name="domain">如果要跨域访问,该参数必需传入,如传入"cn100.com"</param>
        public void SetCookies(string cookieName, Dictionary<string,string> keyValues,DateTime? expries=null,string domain=null)
        {
            if (string.IsNullOrEmpty(cookieName))
                throw new ArgumentNullException("cookieName");

            try
            {
                System.Web.HttpCookie cookie = new HttpCookie(cookieName);
                if (!string.IsNullOrEmpty(domain))
                {
                    cookie.Domain = domain;
                    cookie.Path = "/";
                }
                cookie.Expires = expries ?? DateTime.Now.AddDays(1);
                foreach (var item in keyValues)
                {
                    cookie.Values.Add(item.Key, item.Value);
                }
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 读取cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <returns>返回cookie名称/值列表,可通过索引访问,如果知道名称,可通过名称索引访问</returns>
        public NameValueCollection GetCookies(string cookieName)
        {
            System.Web.HttpCookie cookies = System.Web.HttpContext.Current.Request.Cookies.Get(cookieName);

            if (cookies != null)
                return cookies.Values;
            
            return null;
        }

    }
}
