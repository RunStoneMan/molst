using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using NewExcel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace iTrackStar.MYHM.Utility
{
    /// <summary>
    /// 功能描述：导出功能类
    /// 创建标识：汪桂洋 2012-02-08
    /// 修改标识：
    /// 修改描述：
    /// </summary>
    public class Export
    {
        #region 将HTML导出到PDF
        /// <summary>
        /// HTML导出PDF
        /// </summary>
        /// <param name="myGridView">表格GridView的HTML</param>
        /// <param name="filepath">filename文件名</param>
        public static void ExportToPdf(string myGridViewHtml,string filename)
        {
            try
            {
                
                //System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                //System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=ExportPdf.pdf");
                //System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Document document = new Document(PageSize.A3, 20f, 20f, 20f, 20f);
                StyleSheet style = new StyleSheet();
                style.LoadTagStyle("body", "face", "SIMHEI");
                style.LoadTagStyle("body", "encoding", "Identity-H");
                style.LoadTagStyle("body", "leading", "12,0");
                FontFactory.RegisterDirectory("c:\\Windows\\Fonts");
                FontSelector selector = new FontSelector();
                string zhch = filename.Substring(filename.Length - 21, 4);
                BaseFont baseFont =
                BaseFont.CreateFont(
                "C:\\WINDOWS\\FONTS\\SIMSUN.TTC,1",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED);
                
                if (filename.Substring(filename.Length-21, 4) == "工况明细")
                {
                    //selector.AddFont(FontFactory.GetFont("Gulim", BaseFont.IDENTITY_H, false, 1));
                    selector.AddFont(new Font(baseFont, 2));
                }
                else if (filename.Substring(filename.Length - 21, 4) == "方量分析")
                {
                    //selector.AddFont(FontFactory.GetFont("Gulim", BaseFont.IDENTITY_H, false, 5));
                    selector.AddFont(new Font(baseFont, 5));
                }
                else
                {
                    //selector.AddFont(FontFactory.GetFont("Gulim", BaseFont.IDENTITY_H, false, 5));
                    selector.AddFont(new Font(baseFont, 5));
                }
                Paragraph para = new Paragraph(selector.Process(""));
                HTMLWorker worker = new HTMLWorker(document);
                StringReader stringReader = new StringReader(myGridViewHtml);
                
                HeaderFooter footer = new HeaderFooter(new Phrase("page "), true);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.Border = Rectangle.NO_BORDER;
                document.Footer = footer;
                PdfWriter.GetInstance(document, new FileStream(filename, FileMode.Create));
                document.Open();
                System.Collections.ArrayList p = HTMLWorker.ParseToList(stringReader, style);
                for (int k = 0; k < p.Count; k++)
                {
                    para.Add((IElement)p[k]);
                }
                document.Add(para);
                document.Close();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(filename).Trim()) + "\"");
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.WriteFile(filename);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
            catch (DocumentException de)
            {
                System.Web.HttpContext.Current.Response.Write(de.ToString());
            }
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 打印by Datatable 仲澄 2012-05-31
        /// <summary>
        /// Datatable导出PDF
        /// </summary>
        /// <param name="datatable">Datatable</param>
        /// <param name="filepath">filename文件名</param>
        public static void ExportToPdfByDataTable(System.Data.DataTable datatable,string filename,string HidHead)
        {
            try
            {

                //System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                //System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=ExportPdf.pdf");
                //System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Document document = new Document(PageSize.A3, 20f, 20f, 20f, 20f);
                StyleSheet style = new StyleSheet();
                style.LoadTagStyle("body", "face", "SIMHEI");
                style.LoadTagStyle("body", "encoding", "Identity-H");
                style.LoadTagStyle("body", "leading", "12,0");
                
                PdfPTable table = new PdfPTable(datatable.Columns.Count + 1);
                HeaderFooter footer = new HeaderFooter(new Phrase("page "), true);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.Border = Rectangle.NO_BORDER;
                document.Footer = footer;
                PdfWriter.GetInstance(document, new FileStream(filename, FileMode.Create));
                document.Open();
                BaseFont baseFont =
                BaseFont.CreateFont(
                "C:\\WINDOWS\\FONTS\\SIMSUN.TTC,1",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED);
                Font font = new Font(baseFont, 10);
                if (HidHead != "")
                {
                    //列名
                    string[] hidColoumName = HidHead.Split(',');
                    int cname = hidColoumName.Length;
                    int len = datatable.Columns.Count;
                    if (cname > 0 && len > 0)
                    {
                        //table.AddCell(new Phrase("序号", font));
                    }
                    for (int i = 0; i < cname; i++)
                    {
                        table.AddCell(new Phrase(hidColoumName[i], font));
                    }
                }
                for (int i = 0; i < datatable.Rows.Count; i++)
                {
                    for (int j = 0; j < datatable.Columns.Count; j++)
                    {
                        table.AddCell(new Phrase((i+1).ToString(), font));
                        table.AddCell(new Phrase(datatable.Rows[i][j].ToString(),font));
                    }
                }
                document.Add(table);　　//添加table
                //Document document = new Document();
                //RtfWriter2 writer = RtfWriter2.GetInstance(document, new FileStream(RtfFile, FileMode.Create));
                //document.Open();
                //BaseFont baseFont =
                //BaseFont.CreateFont(
                //FontPath,
                //BaseFont.IDENTITY_H,
                //BaseFont.NOT_EMBEDDED);
                //Font font = new Font(baseFont, FontSize);
                //Table table = new Table(Data.Columns.Count);
                //for (int i = 0; i < Data.Rows.Count; i++)
                //{
                //    for (int j = 0; j < Data.Columns.Count; j++)
                //    {
                //        table.AddCell(new Phrase(Data.Rows[i][j].ToString(), font));
                //    }
                //}
                //document.Add(table);
                //document.Close();

                
                document.Close();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(filename).Trim()) + "\"");
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.WriteFile(filename);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
            catch (DocumentException de)
            {
                System.Web.HttpContext.Current.Response.Write(de.ToString());
            }
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 将图片导出到Excel
        /// <summary>
        /// Excel程序
        /// </summary>
        private NewExcel.Application m_objExcel = null;
        /// <summary>
        /// 
        /// </summary>
        private NewExcel.Workbooks m_objBooks = null;
        /// <summary>
        /// 
        /// </summary>
        private NewExcel._Workbook m_objBook = null;
        /// <summary>
        /// 
        /// </summary>
        private NewExcel.Sheets m_objSheets = null;
        /// <summary>
        /// 
        /// </summary>
        private NewExcel._Worksheet m_objSheet = null;
        /// <summary>
        /// 
        /// </summary>
        private NewExcel.Range m_objRange = null;
        /// <summary>
        /// 
        /// </summary>
        private object m_objOpt = System.Reflection.Missing.Value;
        /// <summary>
        /// 打开没有模板的操作。
        /// </summary>
        public void Open()
        {
            this.Open(String.Empty);
        }

        /// <summary>
        /// 设置可见
        /// </summary>
        public void Visualize()
        {
            m_objExcel.Visible = true;
        }

        /// <summary>
        /// 回发
        /// </summary>
        public void ExcelOut()
        {
            //FileInfo DownloadFile = new FileInfo();
            //System.Web.HttpContext curContext = System.Web.HttpContext.Current;
            //curContext.Response.Clear();
            //curContext.Response.ClearHeaders();
            //curContext.Response.Buffer = false;
            //curContext.Response.ContentType = "application/octet-stream";
            //curContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(DownloadFile.FullName, System.Text.Encoding.ASCII));
            //curContext.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
            //curContext.Response.WriteFile(DownloadFile.FullName);
            //curContext.Response.Flush();
            //DownloadFile.Delete();
            //curContext.Response.End();
        }

        /// <summary>
        /// 打开Excel
        /// </summary>
        /// <param name="TemplateFilePath"></param>
        public void Open(string TemplateFilePath)
        {
            //打开对象
            m_objExcel = new NewExcel.Application();
            m_objExcel.Visible = false;
            m_objExcel.DisplayAlerts = false;

            //if (m_objExcel.Version != "12.0")
            //{
            //    //    MessageBox.Show("您的 Excel 版本不是 12.0 （Office 2007），操作可能会出现问题。");
            //    m_objExcel.Quit();
            //    return;
            //}

            m_objBooks = (NewExcel.Workbooks)m_objExcel.Workbooks;
            if (TemplateFilePath.Equals(String.Empty))
            {
                m_objBook = (NewExcel._Workbook)(m_objBooks.Add(m_objOpt));
            }
            else
            {
                m_objBook = m_objBooks.Open(TemplateFilePath, m_objOpt, m_objOpt, m_objOpt, m_objOpt,
                m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt);
            }
            m_objSheets = (NewExcel.Sheets)m_objBook.Worksheets;
            m_objSheet = (NewExcel._Worksheet)(m_objSheets.get_Item(1));
            m_objExcel.WorkbookBeforeClose += new NewExcel.AppEvents_WorkbookBeforeCloseEventHandler(m_objExcel_WorkbookBeforeClose);
        }
        
        /// <summary>
        /// 保存Excel
        /// </summary>
        /// <param name="m_objBooks"></param>
        /// <param name="_Cancel"></param>
        private void m_objExcel_WorkbookBeforeClose(NewExcel.Workbook m_objBooks, ref bool _Cancel)
        {
            //MessageBox.Show("保存完毕！");
        }

        /// <summary>
        /// 将DataTable插入Excel中.
        /// </summary>
        /// <param name="RangeName">插入的位置.比如:A1</param>
        /// <param name="dt">插入的表</param>
        public void DataTableToExcel(int RangeName, DataTable dt)
        {
            int rowIndex = RangeName;
            foreach (DataRow row in dt.Rows)
            {
                rowIndex++;
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    m_objExcel.Cells[rowIndex, i] = row[i - 1];
                    m_objRange = (NewExcel.Range)m_objExcel.Cells[rowIndex, i];
                    m_objRange.EntireColumn.AutoFit();
                }
            }
        }

        /// <summary>
        /// 将DataTable插入Excel中.
        /// </summary>
        /// <param name="RangeName">插入的位置.比如:A1</param>
        /// <param name="dt">插入的表</param>
        public void importTableToExcel(string RangeName, DataTable dt, int index)
        {
            //时间
            //工作小时
            m_objExcel.Cells[index, 1] = "时间";
            m_objRange = (NewExcel.Range)m_objExcel.Cells[index - 1, 1];
            m_objRange.EntireColumn.AutoFit();

            m_objExcel.Cells[index, 2] = "工作小时";
            m_objRange = (NewExcel.Range)m_objExcel.Cells[index - 1, 2];
            m_objRange.EntireColumn.AutoFit();

            foreach (DataRow row in dt.Rows)
            {
                index++;
                int count = dt.Columns.Count;//记录总列数
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    m_objExcel.Cells[index, count] = row[i - 1];
                    m_objRange = (NewExcel.Range)m_objExcel.Cells[index, i];
                    m_objRange.EntireColumn.AutoFit();
                    count--;
                }
            }
        }

        /// <summary>
        /// HTML转换为Excel
        /// </summary>
        /// <param name="IsMergeCells"></param>
        /// <param name="tableStr"></param>
        public void HtmlToExcel(bool IsMergeCells, string tableStr)
        {
            string a1 = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\n<style>\n</style>\n</head>\n<body>\n";
            string a2 = "\n</body>\n</html>";
            tableStr = a1 + tableStr + a2;

            if (IsMergeCells)
            {
                //合并单元格
                //m_objRange = m_objSheet.get_Range("A1", "K4");
                //m_objRange.Merge(System.Reflection.Missing.Value);
                m_objRange = m_objSheet.get_Range("A1", "A1");
                //m_objRange.
            }

            m_objRange.Select();
            m_objRange.Cells[1, 1] = tableStr;
            m_objRange.Activate();
            m_objRange.Copy(Missing.Value);
            //m_objSheet.get_Range("B2", "B2")
            //.PasteSpecial(Microsoft.Office.Interop.Excel.XlPasteType.xlPasteValues, Microsoft.Office.Interop.Excel.XlPasteSpecialOperation.xlPasteSpecialOperationSubtract
            //, false, false);
            m_objRange.EntireColumn.AutoFit();
        }
        
        /// <summary>
        /// 将图片插入到指定的单元格位置。
        /// 注意：图片必须是绝对物理路径
        /// </summary>
        /// <param name="RangeName">单元格名称，例如：B4</param>
        /// <param name="PicturePath">要插入图片的绝对路径。</param>
        public void InsertPicture(string RangeName, string PicturePath)
        {
            m_objRange = (NewExcel.Range)m_objSheet.get_Range(RangeName, m_objOpt);
            m_objRange.Select();
            NewExcel.Pictures pics = (NewExcel.Pictures)m_objSheet.Pictures(m_objOpt);
            pics.Insert(PicturePath, m_objOpt);

        }

        /// <summary>
        /// 将图片插入到指定的单元格位置，并设置图片的宽度和高度。
        /// 注意：图片必须是绝对物理路径
        /// </summary>
        /// <param name="RangeName">单元格名称，例如：B4</param>
        /// <param name="PicturePath">要插入图片的绝对路径。</param>
        /// <param name="PictuteWidth">插入后，图片在Excel中显示的宽度。</param>
        /// <param name="PictureHeight">插入后，图片在Excel中显示的高度。</param>
        public void InsertPicture(string RangeName, string PicturePath, float PictuteWidth, float PictureHeight)
        {
            m_objRange = m_objSheet.get_Range(RangeName, m_objOpt);
            m_objRange.Select();
            float PicLeft, PicTop;
            PicLeft = Convert.ToSingle(m_objRange.Left);
            PicTop = Convert.ToSingle(m_objRange.Top);
            //参数含义：
            //图片路径
            //是否链接到文件
            //图片插入时是否随文档一起保存
            //图片在文档中的坐标位置（单位：points）
            //图片显示的宽度和高度（单位：points）
            m_objSheet.Shapes.AddPicture(PicturePath, Microsoft.Office.Core.MsoTriState.msoFalse,
             Microsoft.Office.Core.MsoTriState.msoTrue, PicLeft, PicTop, PictuteWidth, PictureHeight);
        }

        /// <summary>
        /// 将图片填充到Excel中的某个或某些单元格中
        /// </summary>
        /// <param name="BeginRangeName">插入的开始单元格</param>
        /// <param name="EndRangeName">插入的结束单元格</param>
        /// <param name="PicturePath">插入图片的绝对物理路径</param>
        /// <param name="IsMergeCells">是否合并上面的单元格</param>
        public void InsertPicture(string BeginRangeName, string EndRangeName, string PicturePath, bool IsMergeCells)
        {
            m_objRange = m_objSheet.get_Range(BeginRangeName, EndRangeName);
            //计算单元格的宽和高
            float PictuteWidth, PictureHeight;
            PictuteWidth = Convert.ToSingle(m_objRange.Width);
            PictureHeight = Convert.ToSingle(m_objRange.Height);
            if (IsMergeCells)
            {
                //合并单元格
                m_objRange.Merge(System.Reflection.Missing.Value);
                m_objRange = m_objSheet.get_Range(BeginRangeName, BeginRangeName);
            }
            m_objRange.Select();
            float PicLeft, PicTop;
            PicLeft = Convert.ToSingle(m_objRange.Left);
            PicTop = Convert.ToSingle(m_objRange.Top);
            //参数含义：
            //图片路径
            //是否链接到文件
            //图片插入时是否随文档一起保存
            //图片在文档中的坐标位置（单位：points）
            //图片显示的宽度和高度（单位：points）
            //参数详细信息参见：http://msdn2.microsoft.com/zh-cn/library/aa221765(office.11).aspx
            m_objSheet.Shapes.AddPicture(PicturePath, Microsoft.Office.Core.MsoTriState.msoFalse,
              Microsoft.Office.Core.MsoTriState.msoTrue, PicLeft, PicTop, PictuteWidth, PictureHeight);
        }

        /// <summary>
        /// 将Excel文件保存到指定的目录，目录必须事先存在，文件名称不一定要存在。
        /// </summary>
        /// <param name="OutputFilePath">要保存成的文件的全路径。</param>
        public void SaveFile(string OutputFilePath)
        {
            m_objBook.SaveAs(OutputFilePath, m_objOpt, m_objOpt,
             m_objOpt, m_objOpt, m_objOpt, NewExcel.XlSaveAsAccessMode.xlNoChange,
             m_objOpt, m_objOpt, m_objOpt, m_objOpt, m_objOpt);
            this.Close();
        }
        
        /// <summary>
        /// 关闭应用程序
        /// </summary>
        private void Close()
        {
            m_objBook.Close(false, m_objOpt, m_objOpt);
            m_objExcel.Quit();
            this.Dispose();
        }
        
        /// <summary>
        /// 释放所引用的COM对象。
        /// </summary>
        public void Dispose()
        {
            ReleaseObj(m_objRange);
            ReleaseObj(m_objSheet);
            ReleaseObj(m_objSheets);
            ReleaseObj(m_objBook);
            ReleaseObj(m_objBooks);
            ReleaseObj(m_objExcel);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        
        /// <summary>
        /// 释放对象，内部调用
        /// </summary>
        /// <param name="o"></param>
        private void ReleaseObj(object o)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
            }
            catch { }
            finally 
            { 
                o = null; 
            }
        }
        
        /// <summary>
        /// 从页面文件下载
        /// </summary>
        /// <param name="strFile">要下载的文件的绝对路径 </param>
        public void DownloadFile(string strFile)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //HttpContext.Current.Response.Charset = System.Text.Encoding.UTF7.ToString();
            //Response.ContentEncoding = System.Test.Encoding.GetEncoding("UTF-8");
            //HttpContext.Current.Response.AddHeader("Content-Type", System.Text.Encoding.UTF8.ToString());
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(strFile).Trim()) + "\"");
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.WriteFile(strFile);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Close();
            if (File.Exists(strFile))
            {
                File.Delete(strFile);
            }
            HttpContext.Current.Response.End();
            Close();
        }
        #endregion

        #region 将图片导出到PDF
        /// <summary>
        /// 导出图片和表格数据到Pdf
        /// </summary>
        /// <param name="strhead">表头文字</param>
        /// <param name="strfoot">表</param>
        /// <param name="filepath">文件名</param>
        /// <param name="图片地址">imgurl</param>
        public static void ExportPicToPdf(string strhead, string strfoot, string filename, string imgurl)
        {
            //Document.compress = false;

            Document document = new Document(PageSize.A3, 20f, 20f, 20f, 20f);
            //string strFileName = "Export" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + ".pdf";
            //string path = HttpContext.Current.Server.MapPath(BasePage.AppUrl + "_Temp") + @"\" + strFileName;
            string path = filename;
            try
            {
                // step 2:
                // we create a writer that listens to the document
                // and directs a PDF-stream to a file
                PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
                // step 3: we open the document
                document.Open();
                StyleSheet style = new StyleSheet();
                style.LoadTagStyle("body", "face", "SIMHEI");
                style.LoadTagStyle("body", "encoding", "Identity-H");
                style.LoadTagStyle("body", "leading", "12,0");
                FontFactory.RegisterDirectory("c:\\Windows\\Fonts");
                FontSelector selector = new FontSelector();
                selector.AddFont(FontFactory.GetFont("Gulim", BaseFont.IDENTITY_H, false, 10));
                if (!string.IsNullOrEmpty(strhead))
                {
                    Paragraph para1 = new Paragraph(selector.Process(""));
                    HTMLWorker worker = new HTMLWorker(document);
                    StringReader stringReader = new StringReader(strhead);
                    System.Collections.ArrayList p = HTMLWorker.ParseToList(stringReader, style);
                    for (int k = 0; k < p.Count; k++)
                    {
                        para1.Add((IElement)p[k]);
                    }
                    document.Add(para1);
                }
                // step 4: we create a table and add it to the document
                string url = imgurl;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
                //图片自适应大小 经验证PDF宽度大致为800以上像素 小于此宽不论 大于则自动缩放图片到宽度800
                if (img.PlainWidth > 800)
                {
                    img.ScalePercent(100 * 800 / img.PlainWidth);
                }
                //img.ScalePercent(100);
                document.Add(img);
                
                if (!string.IsNullOrEmpty(strfoot))
                {
                    Paragraph para2 = new Paragraph(selector.Process(""));
                    HTMLWorker worker = new HTMLWorker(document);
                    StringReader stringReader = new StringReader(strfoot);
                    System.Collections.ArrayList p = HTMLWorker.ParseToList(stringReader, style);
                    for (int k = 0; k < p.Count; k++)
                    {
                        para2.Add((IElement)p[k]);
                    }
                    document.Add(para2);
                }
                //document.Add(AddTable(strfoot));
            }
            catch (DocumentException de)
            {
                System.Web.HttpContext.Current.Response.Write(de.Message);
            }
            catch (IOException ioe)
            {
                System.Web.HttpContext.Current.Response.Write(ioe.Message);
            }
            // step 5: we close the document
            document.Close();
            // DownloadFile(path);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(path).Trim()) + "\"");
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.WriteFile(path);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Close();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            HttpContext.Current.Response.End();
        }
        #endregion 

        #region 图片，DataTable 导出到pdf
        /// <summary>
        /// 图片，DataTable 导出到pdf
        /// </summary>
        /// <param name="datatable">数据集</param>
        /// <param name="filename">文件名称</param>
        /// <param name="imgurl">图片地址</param>
        public static void ExportPdfChart(System.Data.DataTable datatable, string filename, string imgurl)
        {
            try
            {
                Document document = new Document(PageSize.A3, 20f, 20f, 20f, 20f);
                StyleSheet style = new StyleSheet();
                style.LoadTagStyle("body", "face", "SIMHEI");
                style.LoadTagStyle("body", "encoding", "Identity-H");
                style.LoadTagStyle("body", "leading", "12,0");
                PdfPTable table = new PdfPTable(datatable.Columns.Count);
                HeaderFooter footer = new HeaderFooter(new Phrase("page "), true);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.Border = Rectangle.NO_BORDER;
                document.Footer = footer;
                PdfWriter.GetInstance(document, new FileStream(filename, FileMode.Create));
                document.Open();
                //导出图片注释
                //string url = imgurl;
                //iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(url);
                ////图片自适应大小 经验证PDF宽度大致为800以上像素 小于此宽不论 大于则自动缩放图片到宽度800
                //if (img.PlainWidth > 800)
                //{
                //    img.ScalePercent(100 * 800 / img.PlainWidth);
                //}
                ////img.ScalePercent(100);
                //document.Add(img);
                BaseFont baseFont =
                BaseFont.CreateFont(
                "C:\\WINDOWS\\FONTS\\SIMSUN.TTC,1",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED);
                Font font = new Font(baseFont, 10);
                for (int i = 0; i < datatable.Rows.Count; i++)
                {
                    for (int j = 0; j < datatable.Columns.Count; j++)
                    {
                        table.AddCell(new Phrase(datatable.Rows[i][j].ToString(), font));
                    }
                }
                document.Add(table);　　//添加table
                document.Close();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Path.GetFileName(filename).Trim()) + "\"");
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.WriteFile(filename);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
            catch (DocumentException de)
            {
                System.Web.HttpContext.Current.Response.Write(de.ToString());
            }
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            HttpContext.Current.Response.End();
        }
        #endregion
    }
}
