using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace CTool.CNet
{
    /// <summary>
    /// 网络操作一些方法
    /// </summary>
    public class Util
    {
        /// <summary>    
        /// asp.net生成缩略图    
        /// </summary>     
        public string UploadImage(string sSavePath, string thumbPath, int intThumbWidth, int intThumbHeight)
        {
            try
            {
                //原图加载    
                using (Image sourceImage = Image.FromFile(sSavePath))
                {
                    //原图宽度和高度    
                    int width = sourceImage.Width;
                    int height = sourceImage.Height;
                    int smallWidth;
                    int smallHeight;

                    //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽  和 原图的高/缩略图的高)    
                    if (((decimal)width) / height <= ((decimal)intThumbWidth) / intThumbHeight)
                    {
                        smallWidth = intThumbWidth;
                        smallHeight = intThumbWidth * height / width;
                    }
                    else
                    {
                        smallWidth = intThumbHeight * width / height;
                        smallHeight = intThumbHeight;
                    }

                    //缩略图保存的绝对路径    
                    string smallImagePath = thumbPath;

                    //新建一个图板,以最小等比例压缩大小绘制原图    
                    using (Image bitmap = new Bitmap(smallWidth, smallHeight))
                    {
                        //绘制中间图    
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            //高清,平滑    
                            g.InterpolationMode = InterpolationMode.High;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.Clear(Color.Black);
                            g.DrawImage(
                            sourceImage,
                            new Rectangle(0, 0, smallWidth, smallHeight),
                            new Rectangle(0, 0, width, height),
                            GraphicsUnit.Pixel
                            );
                            g.Dispose();
                            bitmap.Save(smallImagePath);
                        }
                        //新建一个图板,以缩略图大小绘制中间图    
                        //using (Image bitmap1 = new Bitmap(intThumbWidth, intThumbHeight))
                        //{
                        //    //绘制缩略图    
                        //    using (Graphics g = Graphics.FromImage(bitmap1))
                        //    {
                        //        //高清,平滑    
                        //        g.InterpolationMode = InterpolationMode.High;
                        //        g.SmoothingMode = SmoothingMode.HighQuality;
                        //        g.Clear(Color.Black);
                        //        int lwidth = (smallWidth - intThumbWidth) / 2;
                        //        int bheight = (smallHeight - intThumbHeight) / 2;
                        //        g.DrawImage(bitmap, new Rectangle(0, 0, intThumbWidth, intThumbHeight), lwidth, bheight, intThumbWidth, intThumbHeight, GraphicsUnit.Pixel);
                        //        g.Dispose();
                        //        bitmap1.Save(smallImagePath);
                        //    }
                        //}
                    }
                }
            }
            catch
            {
                //出错则删除    
                File.Delete(System.Web.HttpContext.Current.Server.MapPath(thumbPath));
                return "图片格式不正确";
            }
            //返回缩略图名称    
            return "";
        }
    }
}
