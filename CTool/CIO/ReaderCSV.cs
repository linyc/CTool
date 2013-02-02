using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CTool.CIO
{
    /// <summary>
    /// 操作CSV文件
    /// </summary>
    public class ReaderCSV
    {
        /// <summary>
        /// 读取csv文件内容
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="split">csv文件采用的分隔符</param>
        /// <returns>返回csv文件每一行的集合</returns>
        public List<string[]> GetData(string filePath,char split)
        {
            List<string[]> list = new List<string[]>();           
            StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("gb2312"));
            while (sr.Peek() > -1)
            {
                list.Add(sr.ReadLine().Split(split));
            }
            sr.Close();

            return list;
        }
    }
}
