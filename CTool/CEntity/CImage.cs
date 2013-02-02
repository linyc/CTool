using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace CTool.CEntity
{
    /// <summary>
    /// 图片操作类
    /// </summary>
    public class CImage
    {
        /// <summary>
        /// 根据图片路径获取该图片的信息
        /// </summary>
        /// <param name="pImgFullPath">图片完整路径</param>
        /// <returns></returns>
        public static ImageInfo GetAImageInfo(string pImgFullPath)
        {
            ImageInfo imgInfo = new ImageInfo();
            if (File.Exists(pImgFullPath))
            {
                imgInfo.cImgNameWithExtName = Path.GetFileName(pImgFullPath);
                imgInfo.cImgFullPath = pImgFullPath;
                imgInfo.cImgExtNameWithPoint = Path.GetExtension(pImgFullPath);
                imgInfo.cImgExtNameWithoutPoint = Path.GetExtension(pImgFullPath).TrimStart('.');

                FileStream fs = File.OpenRead(pImgFullPath);
                Image tmpImg = Image.FromStream(fs);
                imgInfo.cImgWidth = tmpImg.Width;
                imgInfo.cImgHeight = tmpImg.Height;
                imgInfo.cImgSize = Math.Round(fs.Length * 1m / 1024, 2);
                byte[] bs = new byte[fs.Length];
                fs.Read(bs, 0, bs.Length);
                fs.Close();
                imgInfo.cImgBytes = bs;
            }
            else
            {
                imgInfo.cImgExist = false;
            }
            return imgInfo;
        }

        /// <summary>
        /// 获取一张图片的信息,返回[宽，高，保留两位小数点大小(KB)，扩展名(不含.号)]
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <returns>返回字符串数组[宽，高，保留两位小数大小(KB)，扩展名(不含.号)]</returns>
        public static string[] GetAImageInfo(string filePath,object obj=null)
        {
            Stream filestream = new FileStream(filePath, FileMode.Open);
            Image imgInfo = Image.FromStream(filestream);
            long size = filestream.Length;
            string ext = Path.GetExtension(filePath);
            string[] info = new string[] { imgInfo.Width.ToString(), imgInfo.Height.ToString(), (size / 1024.0).ToString(), ext.Substring(ext.IndexOf('.') + 1) };
            imgInfo.Dispose();
            filestream.Close();

            return info;
        }
    }
}
