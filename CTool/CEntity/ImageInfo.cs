using System;
using System.Collections.Generic;
using System.Text;

namespace CTool.CEntity
{
    /// <summary>
    /// 一张图片的结构信息
    /// </summary>
    public struct ImageInfo
    {
        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool cImgExist { get; set; }
        /// <summary>
        /// 名称（含扩展名）
        /// </summary>
        public string cImgNameWithExtName { get; set; }
        /// <summary>
        /// 完整路径
        /// </summary>
        public string cImgFullPath { get; set; }
        /// <summary>
        /// 扩展名（不含点号）
        /// </summary>
        public string cImgExtNameWithPoint { get; set; }
        /// <summary>
        /// 扩展名（含点号）
        /// </summary>
        public string cImgExtNameWithoutPoint { get; set; }
        /// <summary>
        /// 大小（保留两位小数KB）
        /// </summary>
        public decimal cImgSize { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int cImgWidth { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int cImgHeight { get; set; }
        /// <summary>
        /// 文件流
        /// </summary>
        public byte[] cImgBytes { get; set; }
    }
}
