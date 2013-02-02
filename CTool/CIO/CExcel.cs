using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

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
}
