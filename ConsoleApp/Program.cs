using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTool.CNet;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpHelper http = new HttpHelper();
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics["txt"] = "txt..test";
            dics["rd"] = "rd..test";
            //string s = http.SendHttpRequest("http://localhost:15406/webSite/JsHandler.ashx", dics, null, "post");
            //string s = http.GetResponse("http://localhost:15406/webSite/JsHandler.ashx", "post", dics);
            Dictionary<string, string> files = new Dictionary<string, string>();
            files.Add("file1", @"G:\Desktop\pro.txt");
            files.Add("file2", @"G:\Desktop\13579.ppk");
            string s = http.SendHttpRequest("http://localhost:15406/webSite/JsHandler.ashx", dics, files, null);

            Console.WriteLine(s);

            Console.Read();
        }
    }
}
