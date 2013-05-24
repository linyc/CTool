using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CTool.CIO
{
    public class CExcel
    {
        /// <summary>
        /// 读取Excel文件的第一张表信息
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <returns></returns>
        DataTable GetExcelTable(string excelFilePath)
        {
            try
            {
                Excel.Application app = new Excel.Application();
                Excel.Sheets sheets;
                Excel.Workbook workbook;
                System.Data.DataTable dt = new System.Data.DataTable();
                if (app == null)
                {
                    return null;
                }

                object oMissiong = System.Reflection.Missing.Value;

                workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);

                //将数据读入到DataTable中——Start   

                sheets = workbook.Worksheets;
                Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(1);
                if (worksheet == null)
                    return null;

                string cellContent;
                int iRowCount = worksheet.UsedRange.Rows.Count;
                int iColCount = worksheet.UsedRange.Columns.Count;
                Excel.Range range;
                for (int iRow = 1; iRow <= iRowCount; iRow++)
                {
                    DataRow dr = dt.NewRow();

                    for (int iCol = 1; iCol <= iColCount; iCol++)
                    {
                        range = (Excel.Range)worksheet.Cells[iRow, iCol];

                        cellContent = (range.Value2 == null) ? "" : range.Text.ToString();

                        if (iRow == 1)
                        {
                            dt.Columns.Add(cellContent);
                        }
                        else
                        {
                            dr[iCol - 1] = cellContent;
                        }
                    }

                    if (iRow != 1)
                        dt.Rows.Add(dr);
                }

                //将数据读入到DataTable中——End
                workbook.Close(false, oMissiong, oMissiong);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                workbook = null;
                app.Workbooks.Close();
                app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return dt;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 比较两个Excel表的不同（含行列数和单元格内容）
        /// </summary>
        /// <param name="firstExcel">第一个Excel文件</param>
        /// <param name="secondExcel">第二个Excel文件</param>
        /// <returns>不同的消息说明</returns>
        public string CompareTwoExcelTable(string firstExcel, string secondExcel)
        {
            string result = "";

            if (!File.Exists(firstExcel))
                return "第一个路径的文件不存在！";
            if (!File.Exists(secondExcel))
                return "第二个路径的文件不存在！";

            DataTable dtFirst = new DataTable();
            dtFirst = GetExcelTable(firstExcel);
            DataTable dtSecond = new DataTable();
            dtSecond = GetExcelTable(secondExcel);
            try
            {
                if (dtFirst != null && dtSecond != null)
                {
                    StringBuilder comp = new StringBuilder();
                    if (dtFirst.Rows.Count != dtSecond.Rows.Count)
                    {
                        comp.Append("两个文件的行数不一致！\n");
                    }
                    if (dtFirst.Columns.Count != dtSecond.Columns.Count)
                    {
                        comp.Append("两个文件的列数不一致！");
                    }
                    if (comp.ToString() != "")
                    {
                        return comp.ToString();
                    }
                    StringBuilder contentDifference = new StringBuilder();
                    for (int i = 0; i < dtFirst.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtFirst.Columns.Count; j++)
                        {
                            if (!dtFirst.Rows[i][j].ToString().Equals(dtSecond.Rows[i][j].ToString()))
                            {
                                contentDifference.Append("第[").Append(i + 1).Append("]行，").Append("第[").Append(j).Append("]列\n");
                            }
                        }
                    }
                    if (result.ToString() != "")
                    {
                        result.Insert(0, "以下数据不一致：\n\n");
                        return result.ToString();
                    }
                    else
                    {
                        return "两个文件相同！";
                    }
                }
                else
                {
                    return "error:读取文件出错！";
                }
            }
            catch (Exception ex)
            {
                return "error:"+ex.ToString();
            }
            finally
            {
                if (dtFirst != null)
                    dtFirst.Dispose();
                if (dtSecond != null)
                    dtSecond.Dispose();
            }
        }

        /// <summary>
        /// 根据DataGridView导出excel
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="myDGV"></param>
        private void ExportExcel(string fileName, DataGridView myDGV)
        {
            string saveFileName = "";
            //bool fileSaved = false;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消 
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                return;
            }
 
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 

            //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }
            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = myDGV.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            //if (Microsoft.Office.Interop.cmbxType.Text != "Notification")
            //{
            //    Excel.Range rg = worksheet.get_Range(worksheet.Cells[2, 2], worksheet.Cells[ds.Tables[0].Rows.Count + 1, 2]);
            //    rg.NumberFormat = "00000000";
            //}
 
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                    //fileSaved = true;
                }
                catch (Exception ex)
                {
                    //fileSaved = false;
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                }
 
            }
            //else
            //{
            //    fileSaved = false;
            //}
            xlApp.Quit();
            GC.Collect();//强行销毁 
            // if (fileSaved && System.IO.File.Exists(saveFileName)) System.Diagnostics.Process.Start(saveFileName); //打开EXCEL
            MessageBox.Show(fileName + "的简明资料保存成功", "提示", MessageBoxButtons.OK);
        }

    }
    /// <summary>
    /// author by :ljun
    /// Excel工具类，目前仅支持一个工作薄的操作
    /// </summary>
    public class ExcelHelper : IDisposable
    {
        #region 构造函数

        /// <summary>
        /// 构造函数，将一个已有Excel工作簿作为模板，并指定输出路径
        /// </summary>
        /// <param name="templetFilePath">Excel模板文件路径</param>
        /// <param name="outputFilePath">输出Excel文件路径</param>
        public ExcelHelper(string templetFilePath, string outputFilePath)
        {
            if (templetFilePath == null)
                throw new Exception("Excel模板文件路径不能为空！");

            if (outputFilePath == null)
                throw new Exception("输出Excel文件路径不能为空！");

            if (!File.Exists(templetFilePath))
                throw new Exception("指定路径的Excel模板文件不存在！");

            this.templetFile = templetFilePath;
            this.outputFile = outputFilePath;

            //excelApp = new Excel.ApplicationClass();
            excelApp = new Excel.Application();
            excelApp.Visible = false;

            excelApp.DisplayAlerts = false;  //是否需要显示提示
            excelApp.AlertBeforeOverwriting = false;  //是否弹出提示覆盖

            //打开模板文件，得到WorkBook对象
            workBook = excelApp.Workbooks.Open(templetFile, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, Type.Missing, Type.Missing);

            //得到WorkSheet对象
            workSheet = (Excel.Worksheet)workBook.Sheets.get_Item(1);
        }

        /// <summary>
        /// 构造函数，新建一个工作簿
        /// </summary>
        public ExcelHelper()
        {
            //excelApp = new Excel.ApplicationClass();
            excelApp = new Excel.Application();
            excelApp.Visible = false;

            //设置禁止弹出保存和覆盖的询问提示框
            excelApp.DisplayAlerts = false;
            excelApp.AlertBeforeOverwriting = false;

            //新建一个WorkBook
            workBook = excelApp.Workbooks.Add(Type.Missing);

            //得到WorkSheet对象
            workSheet = (Excel.Worksheet)workBook.Sheets.get_Item(1);
        }

        #endregion

        #region 私有变量

        private string templetFile = null;
        private string outputFile = null;
        private object missing = System.Reflection.Missing.Value;
        private Excel.Application excelApp;
        private Excel.Workbook workBook;
        private Excel.Worksheet workSheet;
        private Excel.Range range;
        private Excel.Range range1;
        private Excel.Range range2;

        #endregion

        #region 公共属性

        /// <summary>
        /// WorkSheet数量
        /// </summary>
        public int WorkSheetCount
        {
            get { return workBook.Sheets.Count; }
        }

        /// <summary>
        /// Excel模板文件路径
        /// </summary>
        public string TempletFilePath
        {
            set { this.templetFile = value; }
        }

        /// <summary>
        /// 输出Excel文件路径
        /// </summary>
        public string OutputFilePath
        {
            set { this.outputFile = value; }
        }

        #endregion

        #region 批量写入Excel内容

        /// <summary>
        /// 将二维数组数据写入Excel文件
        /// </summary>
        /// <param name="arr">二维数组</param>
        /// <param name="top">行索引</param>
        /// <param name="left">列索引</param>
        public void ArrayToExcel(object[,] arr, int top, int left)
        {
            int rowCount = arr.GetLength(0); //二维数组行数（一维长度）
            int colCount = arr.GetLength(1); //二维数据列数（二维长度）

            range = (Excel.Range)workSheet.Cells[top, left];
            range = range.get_Resize(rowCount, colCount);
            range.FormulaArray = arr;
        }

        #endregion

        #region 行操作

        /// <summary>
        /// 插行（在指定行上面插入指定数量行）
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="count"></param>
        public void InsertRows(int rowIndex, int count)
        {
            try
            {
                range = (Excel.Range)workSheet.Rows[rowIndex, this.missing];
                for (int i = 0; i < count; i++)
                {
                    range.Insert(Excel.XlDirection.xlDown, missing);
                }
            }
            catch (Exception e)
            {
                this.KillExcelProcess(false);
                throw e;
            }
        }

        /// <summary>
        /// 复制行（在指定行下面复制指定数量行）
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="count"></param>
        public void CopyRows(int rowIndex, int count)
        {
            try
            {
                range1 = (Excel.Range)workSheet.Rows[rowIndex, this.missing];
                for (int i = 1; i <= count; i++)
                {
                    range2 = (Excel.Range)workSheet.Rows[rowIndex + i, this.missing];
                    range1.Copy(range2);
                }
            }
            catch (Exception e)
            {
                this.KillExcelProcess(false);
                throw e;
            }
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="count"></param>
        public void DeleteRows(int rowIndex, int count)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    range = (Excel.Range)workSheet.Rows[rowIndex, this.missing];
                    range.Delete(Excel.XlDirection.xlDown);
                }
            }
            catch (Exception e)
            {
                this.KillExcelProcess(false);
                throw e;
            }
        }

        #endregion

        #region 列操作

        /// <summary>
        /// 插列（在指定列右边插入指定数量列）
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="count"></param>
        public void InsertColumns(int columnIndex, int count)
        {
            try
            {
                range = (Excel.Range)(workSheet.Columns[columnIndex, this.missing]);  //注意：这里和VS的智能提示不一样，第一个参数是columnindex

                for (int i = 0; i < count; i++)
                {
                    range.Insert(Excel.XlDirection.xlDown, missing);
                }
            }
            catch (Exception e)
            {
                this.KillExcelProcess(false);
                throw e;
            }
        }

        /// <summary>
        /// 删除列
        /// </summary> 
        /// <param name="columnIndex"></param>
        /// <param name="count"></param>
        public void DeleteColumns(int columnIndex, int count)
        {
            try
            {
                for (int i = columnIndex + count - 1; i >= columnIndex; i--)
                {
                    ((Excel.Range)workSheet.Cells[1, i]).EntireColumn.Delete(0);
                }
            }
            catch (Exception e)
            {
                this.KillExcelProcess(false);
                throw e;
            }
        }

        #endregion

        #region 单元格操作

        /// <summary>
        /// 合并单元格，并赋值，对指定WorkSheet操作
        /// </summary>
        /// <param name="sheetIndex">WorkSheet索引</param>
        /// <param name="beginRowIndex">开始行索引</param>
        /// <param name="beginColumnIndex">开始列索引</param>
        /// <param name="endRowIndex">结束行索引</param>
        /// <param name="endColumnIndex">结束列索引</param>
        /// <param name="text">合并后Range的值</param>
        public void MergeCells(int beginRowIndex, int beginColumnIndex, int endRowIndex, int endColumnIndex, string text)
        {
            range = workSheet.get_Range(workSheet.Cells[beginRowIndex, beginColumnIndex], workSheet.Cells[endRowIndex, endColumnIndex]);

            range.ClearContents(); //先把Range内容清除，合并才不会出错
            range.MergeCells = true;

            range.Value2 = text;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        }

        /// <summary>
        /// 向单元格写入数据，对当前WorkSheet操作
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="text">要写入的文本值</param>
        public void SetCells(int rowIndex, int columnIndex, string text)
        {
            try
            {
                workSheet.Cells[rowIndex, columnIndex] = text;
            }
            catch
            {
                this.KillExcelProcess(false);
                throw new Exception("向单元格[" + rowIndex + "," + columnIndex + "]写数据出错！");
            }
        }

        /// <summary>
        /// 向单元格写入数据，对当前WorkSheet操作
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="text">要写入的文本值</param>
        public void SetCells(int rowIndex, int columnIndex, string text, string comment)
        {
            try
            {
                workSheet.Cells[rowIndex, columnIndex] = text;
                SetCellComment(rowIndex, columnIndex, comment);
            }
            catch
            {
                this.KillExcelProcess(false);
                throw new Exception("向单元格[" + rowIndex + "," + columnIndex + "]写数据出错！");
            }
        }

        /// <summary>
        /// 向单元格写入数据，对当前WorkSheet操作
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="text">要写入的文本值</param>
        public void SetCellComment(int rowIndex, int columnIndex, string comment)
        {
            try
            {
                Excel.Range range = workSheet.Cells[rowIndex, columnIndex] as Excel.Range;
                range.AddComment(comment);
            }
            catch
            {
                this.KillExcelProcess(false);
                throw new Exception("向单元格[" + rowIndex + "," + columnIndex + "]写数据出错！");
            }
        }

        /// <summary>
        /// 单元格背景色及填充方式
        /// </summary>
        /// <param name="startRow">起始行</param>
        /// <param name="startColumn">起始列</param>
        /// <param name="endRow">结束行</param>
        /// <param name="endColumn">结束列</param>
        /// <param name="color">颜色索引</param>
        public void SetCellsBackColor(int startRow, int startColumn, int endRow, int endColumn, ColorIndex color)
        {
            Excel.Range range = excelApp.get_Range(excelApp.Cells[startRow, startColumn], excelApp.Cells[endRow, endColumn]);
            range.Interior.ColorIndex = color;
        }


        #endregion

        #region 保存文件

        /// <summary>
        /// 另存文件
        /// </summary>
        public void SaveAsFile()
        {
            if (this.outputFile == null)
                throw new Exception("没有指定输出文件路径！");

            try
            {
                workBook.SaveAs(outputFile, missing, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Quit();
            }
        }

        /// <summary>
        /// 将Excel文件另存为指定格式
        /// </summary>
        /// <param name="format">HTML，CSV，TEXT，EXCEL，XML</param>
        public void SaveAsFile(SaveAsFileFormat format)
        {
            if (this.outputFile == null)
                throw new Exception("没有指定输出文件路径！");

            try
            {
                switch (format)
                {
                    case SaveAsFileFormat.HTML:
                        {
                            workBook.SaveAs(outputFile, Excel.XlFileFormat.xlHtml, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);
                            break;
                        }
                    case SaveAsFileFormat.CSV:
                        {
                            workBook.SaveAs(outputFile, Excel.XlFileFormat.xlCSV, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);
                            break;
                        }
                    case SaveAsFileFormat.TEXT:
                        {
                            workBook.SaveAs(outputFile, Excel.XlFileFormat.xlUnicodeText, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);
                            break;
                        }
                    case SaveAsFileFormat.XML:
                        {
                            workBook.SaveAs(outputFile, Excel.XlFileFormat.xlXMLSpreadsheet, Type.Missing, Type.Missing,
                             Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
                             Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                            break;
                        }
                    default:
                        {
                            workBook.SaveAs(outputFile, missing, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Quit();
            }
        }

        #endregion

        #region 杀进程释放资源

        /// <summary>
        /// 结束Excel进程
        /// </summary>
        public void KillExcelProcess(bool bAll)
        {
            if (bAll)
            {
                KillAllExcelProcess();
            }
            else
            {
                KillSpecialExcel();
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        /// <summary>
        /// 杀特殊进程的Excel
        /// </summary>
        public void KillSpecialExcel()
        {
            try
            {
                if (excelApp != null)
                {
                    int lpdwProcessId;
                    GetWindowThreadProcessId((IntPtr)excelApp.Hwnd, out lpdwProcessId);

                    if (lpdwProcessId > 0)    //c-s方式
                    {
                        System.Diagnostics.Process.GetProcessById(lpdwProcessId).Kill();
                    }
                    else
                    {
                        Quit();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Quit()
        {
            if (workBook != null)
                workBook.Close(null, null, null);
            if (excelApp != null)
            {
                excelApp.Workbooks.Close();
                excelApp.Quit();
            }
            if (range != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                range = null;
            }
            if (range1 != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range1);
                range1 = null;
            }
            if (range2 != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range2);
                range2 = null;
            }
            if (workSheet != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);
                workSheet = null;
            }
            if (workBook != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                workBook = null;
            }
            if (excelApp != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                excelApp = null;
            }
            GC.Collect();
        }

        /// <summary>
        /// 接口方法 释放资源
        /// </summary>
        public void Dispose()
        {
            Quit();
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 杀Excel进程
        /// </summary>
        public static void KillAllExcelProcess()
        {
            Process[] myProcesses;
            myProcesses = Process.GetProcessesByName("Excel");

            //得不到Excel进程ID，暂时只能判断进程启动时间
            foreach (Process myProcess in myProcesses)
            {
                myProcess.Kill();
                myProcess.WaitForExit();
            }
        }

        /// <summary>
        /// 打开相应的excel
        /// </summary>
        /// <param name="filepath"></param>
        public static void OpenExcel(string filepath)
        {
            Excel.Application xlsApp = new Excel.Application();
            xlsApp.Workbooks.Open(filepath);
            xlsApp.Visible = true;
        }

        #endregion

    }

    /// <summary>
    /// 常用颜色定义,对就Excel中颜色名
    /// </summary>
    public enum ColorIndex
    {
        无色 = -4142,
        自动 = -4105,
        黑色 = 1,
        褐色 = 53,
        橄榄 = 52,
        深绿 = 51,
        深青 = 49,
        深蓝 = 11,
        靛蓝 = 55,
        灰色80 = 56,
        深红 = 9,
        橙色 = 46,
        深黄 = 12,
        绿色 = 10,
        青色 = 14,
        蓝色 = 5,
        蓝灰 = 47,
        灰色50 = 16,
        红色 = 3,
        浅橙色 = 45,
        酸橙色 = 43,
        海绿 = 50,
        水绿色 = 42,
        浅蓝 = 41,
        紫罗兰 = 13,
        灰色40 = 48,
        粉红 = 7,
        金色 = 44,
        黄色 = 6,
        鲜绿 = 4,
        青绿 = 8,
        天蓝 = 33,
        梅红 = 54,
        灰色25 = 15,
        玫瑰红 = 38,
        茶色 = 40,
        浅黄 = 36,
        浅绿 = 35,
        浅青绿 = 34,
        淡蓝 = 37,
        淡紫 = 39,
        白色 = 2
    }


    /// <summary>
    /// HTML，CSV，TEXT，EXCEL，XML
    /// </summary>
    public enum SaveAsFileFormat
    {
        HTML,
        CSV,
        TEXT,
        EXCEL,
        XML
    }
}
