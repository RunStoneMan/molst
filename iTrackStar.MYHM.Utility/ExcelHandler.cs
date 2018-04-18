using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;
using System.Text;
using System.Data.OleDb;

namespace iTrackStar.MYHM.Utility
{
 
    public class ExcelHandler
    {

        /// <summary>
        /// 判断上传文件是否为excel 预留暂时根据后缀名来判断 如果要严格要求可根据文件流来判断 xls = 208207  xlsx ZIP = 8075
        /// </summary>
        /// <param name="FileUpload1"></param>
        /// <returns></returns>
        public static bool UpLoadFileIsOk(System.Web.UI.WebControls.FileUpload FileUpload1)
        {
            string FileName = FileUpload1.FileName;
            if (FileName == null || FileName.Trim() == string.Empty)
                return false;

            FileName = FileName.Trim().ToLower();

            if (FileName.EndsWith(".xls") || FileName.EndsWith(".xlsx"))
                return true;
            else
                return false;
        }


        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV(System.Data.DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            try
            {
                string data = "";
                //写出列名称
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        data += dt.Rows[i][j].ToString();
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
            }
            catch
            {
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static System.Data.DataTable OpenCSV(string fileName)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            try
            {
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    aryLine = strLine.Split(',');
                    if (IsFirst == true)
                    {
                        IsFirst = false;
                        columnCount = aryLine.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(aryLine[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                sr.Close();
                fs.Close();
            }
            return dt;
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <param name="flag">功能集管理</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static System.Data.DataTable OpenCSV(string fileName,string flag)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            try
            {
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (strLine == "")
                    {
                        continue;
                    }
                    aryLine = strLine.Split(',');
                    if (IsFirst == true)
                    {
                        IsFirst = false;
                        columnCount = aryLine.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(aryLine[i]);
                            dt.Columns.Add(dc);
                        }
                        if (flag == "fun")
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                DataColumn dc = new DataColumn("col" + i);
                                dt.Columns.Add(dc);
                            }
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                sr.Close();
                fs.Close();
            }
            return dt;
        }



        #region 读取Excel方法
        /// <summary>
        /// 读取Excel的方法
        /// </summary>
        /// <param name="FilePath">xls文件路径</param>
        /// <param name="SelectSQL">查询SQL语句</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectFromXLS(string FilePath, string SelectSQL)
        {
            string connStr = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = '" + FilePath + "';Extended Properties=\"Excel 8.0; HDR=YES; IMEX=1;\"";
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr);
            System.Data.OleDb.OleDbDataAdapter da = null;
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                da = new System.Data.OleDb.OleDbDataAdapter(SelectSQL, conn);
                da.Fill(ds, "SelectResult");
            }
            catch (Exception e)
            {
                conn.Close();
                Logger.LogHelper.LogServiceWebError("iTrackStar.MYHM.Utility.ExcelHandler.SelectFromXLS()"+e.ToString());
                //throw e;
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 读取Excel某个单元格的值
        /// </summary>
        /// <param name="mysheet">Excel sheet</param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <returns>单元格的值</returns>
        public static string getExcelCellValue(Worksheet mysheet, int row, int col)
        {
            string ret = "";
            if (((Microsoft.Office.Interop.Excel.Range)mysheet.Cells[row, col]).Value2 != null)
            {
                ret = ((Range)mysheet.Cells[row, col]).Value2.ToString().Trim();
            }
            return ret;
        }

        /// <summary>
        /// 释放excel占有的内存，删除上传的excel
        /// </summary>
        /// <param name="app"></param>
        /// <param name="wb"></param>
        /// <param name="sheet"></param>
        /// <param name="filename">excel保存的路径</param>
        /// <returns></returns>
        public static void releaseComObject_Excel(Application app, Workbook wb, Worksheet sheet, string filename)
        {
            wb.Close(null, null, null);
            app.Workbooks.Close();

            app.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            sheet = null;
            wb = null;
            //System.IO.File.Delete(filename);
            GC.Collect();
        }

        /// <summary>
        /// 读取Excel到DataTable---把第一行作为列名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static System.Data.DataTable getTableFromExcel(string filePath)
        {
            if (!filePath.EndsWith(".xls") && !filePath.EndsWith(".xlsx")) { return new System.Data.DataTable(); }
            //改为上传的路径
            System.Data.DataTable dt = new System.Data.DataTable("tableExcel");
            DataColumn column;
            DataRow dr;
            Application app = new Application();
            app.Visible = false;
            //得到WorkBook对象, 可以用两种方式之一: 下面的是打开已有的文件
            Workbook wb = app.Workbooks._Open(filePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value
               , Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            Worksheet sheet = (Worksheet)wb.Sheets[1];
            int count = 100;//假定实际excel列的总个数
            //创建datatable列,列个数依据excel中前n个有数值的cell
            for (int i = 0; i < count; i++)
            {
                object name = ((Microsoft.Office.Interop.Excel.Range)sheet.Cells[1, i + 1]).Value2;
                if (name != null)
                {
                    column = new DataColumn(name.ToString(), typeof(string));
                    dt.Columns.Add(column);
                }
                else  //遇到空格认为excel列结束
                {
                    break;
                }
            }
            //赋值
            try
            {
                int index = 2;
                while (((Microsoft.Office.Interop.Excel.Range)sheet.Cells[index, 1]).Value2 != null)
                {
                    dr = dt.NewRow();
                    for (int j = 1; j < dt.Columns.Count + 1; j++)
                    {
                        dr[j - 1] = getExcelCellValue(sheet, index, j);//读取excel值的内容传给datarow
                    }

                    dt.Rows.Add(dr);
                    index++;
                }
            }
            finally
            {
                //释放excel占有的内存，删除上传的excel
                releaseComObject_Excel(app, wb, sheet, filePath);
            }
            return dt;
        }
        #endregion

        /// <summary>
        /// 采用oleDB方式读取EXCEL(支持多个sheet)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static System.Data.DataSet ReadExcel(string filePath)
        {

            OleDbConnection conn = null;
            OleDbDataAdapter adapter = null;
            DataSet set = new DataSet();
            try
            {
                string str = string.Format("Provider=Microsoft.Ace.OleDb.12.0;data source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'", filePath);

                conn = new OleDbConnection(str);
                conn.Open();
                System.Data.DataTable tableNameTab = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);


                System.Data.DataTable table = null;
                foreach (DataRow row in tableNameTab.Rows)
                {
                    table = new System.Data.DataTable();
                    adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", row["TABLE_NAME"]), conn);
                    adapter.Fill(table);
                    set.Tables.Add(table);
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (adapter != null)
                {
                    adapter.Dispose();
                }

                if (conn != null)
                {
                    conn.Close();
                }
            }

            return set;

        }


        public static string CreateFileName()
        {
            string sName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string sNum = GetRandomstr(2);
            sName += sNum;
            return sName;
        }

        /// <summary>
        /// 获取指定位数随机数
        /// </summary>
        /// <param name="nLength"></param>
        /// <returns></returns>
        public static string GetRandomstr(int nLength)
        {
            string Randomstr = string.Empty;
            Random rand = new Random();

            for (int i = 0; i < nLength; i++)
            {
                Randomstr += rand.Next(10).ToString();
            }
            return Randomstr;
        }
    }

}
