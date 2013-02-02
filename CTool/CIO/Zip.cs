using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace CTool.CIO
{
    /// <summary>
    /// 压缩解压文件夹
    /// </summary>
    public class Zip
    {
        /// <summary>
        /// 将目录压缩到一个zip文件（支持带子文件夹一起压缩）
        /// </summary>
        /// <param name="filename">压缩后的zip文件名（带.zip后缀名）</param>
        /// <param name="directory">要进行压缩的文件夹</param>
        /// <returns></returns>
        public bool ZipFiles(string filename, string directory)
        {
            try
            {
                directory = directory.Replace("/", "\\");

                if (!directory.EndsWith("\\"))
                    directory += "\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                FastZip fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.CreateZip(filename, directory, true, "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 将zip文件解压到一个目录
        /// </summary>
        /// <param name="file">目标zip文件</param>
        /// <param name="directory">解压的目标文件夹</param>
        /// <returns></returns>
        public bool UnZipFiles(string file, string directory)
        {
            try
            {
                if (!File.Exists(file))
                    return false;

                directory = directory.Replace("/", "\\");
                if (!directory.EndsWith("\\"))
                    directory += "\\";

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                ZipInputStream s = new ZipInputStream(File.OpenRead(file));
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName != String.Empty)
                        Directory.CreateDirectory(directory + directoryName);

                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(directory + theEntry.Name);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                } s.Close(); return true;
            }
            catch (Exception) { return false; }
        }
    }
}
