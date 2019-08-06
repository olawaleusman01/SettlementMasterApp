using Generic.Dapper.Data;
using Generic.Data.Utilities;
using SettlementMaster.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Dapper.ReportClass;
using System.Threading.Tasks;
using Generic.Dapper.Model;
using System.Data;
using OfficeOpenXml;
using Generic.Data;
using System.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.SqlClient;

namespace SettlementMaster.App.Controllers
{
    public class RPTDownloadController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        IDapperReportSettings _repo2 = new DapperReportSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_INSTITUTION> repoInst = null;
        private readonly IRepository<SM_MERCHANTDETAIL> repoM = null;
        private readonly IRepository<SM_COMPANY_PROFILE> repoComp = null;
        int menuId = 36;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        public RPTDownloadController()
        {
            uow = new UnitOfWork();
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoM = new Repository<SM_MERCHANTDETAIL>(uow);
            repoComp = new Repository<SM_COMPANY_PROFILE>(uow);

            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleId = user.UserRole;
                institutionId = user.InstitutionId;
                fullName = user.FullName;
                deptCode = user.DeptCode;
                
            }

        }
        [MyAuthorize]
        // GET: SetReport
        public async Task<ActionResult> Index()
        {
            
            GetPriv();
            ViewBag.InstId = institutionId;
            var ReportList = await _repo.GetReportListAsync();
            ViewBag.ReportList = new SelectList(ReportList, "CODE", "DESCRIPTION");
            var scheme = await _repo.GetCardSchemeAsync(0,true,"Active");
            ViewBag.CardSchemeList = new SelectList(scheme, "CARDSCHEME", "CARDSCHEME_DESC");
            var chan = await _repo.GetChannelAsync(0, true, "Active");
            ViewBag.ChannelList = new SelectList(chan, "CODE", "DESCRIPTION");
            var inst = await _repo.GetInstitutionAsync(0, true, "Active");
            var ghh = inst.Where(d => d.IS_BANK != null && (d.IS_BANK == "Y" || d.IS_BANK == "y")).ToList();
            ViewBag.InstitutionList = new SelectList(ghh, "CBN_CODE", "INSTITUTION_NAME");
            //ViewBag.ReportList = new SelectList(SmartObj.GetReportList(), "CODE", "DESCRIPTION");

            return View();
        }
        void GetPriv()
        {
            var rec = _repo.GetMenuPrivilege(menuId, roleId);
            if (rec != null)
            {
                ViewBag.CanAdd = rec.CanAdd;
                ViewBag.CanEdit = rec.CanEdit;
            }
        }
        //[HttpPost]
        //public ActionResult DownloadReport(DateTime? SetDate, string ReportType)
        //{
        //    try
        //    {
        //        //Data can be passed with hidden input elements.
        //        //string contextIdStr = Request.Form["BatchId"];
        //        //int contextId = 0;
        //        //  int.TryParse(contextIdStr, out contextId);

        //        //Call to get Excel byte array.
        //        // GetExcelBytesForPayroll(q, ReqType);
        //        var dtMain = rptSett.generateDS("ALL", "U", SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"), null, null, null);
        //        var excelBytes = ExcelHelper.ExportDataSet(dtMain);
        //        ////Set file name.
        //        //switch (ReqType)
        //        //{
        //        //    case "N":
        //        //        {
        //        //            // q = 
        //        //            break;
        //        //        }
        //        //    case "A":
        //        //        {
        //        //            break;
        //        //        }
        //        //    case "R":
        //        //        {
        //        //            break;
        //        //        }
        //        //}
        //        var fileName = string.Format("Settlement Report for {0}.xlsx", SetDate.GetValueOrDefault().ToString("dd-MM-yy"));

        //        //List<FileContentResult> tr = new List<FileContentResult>();

        //        //tr.Add(File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName));
        //        //tr.Add(File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + 1));

        //        //Return file with the type and name. 
        //        //ContentType "application/vnd.ms-excel" does not work well for browsers other than IE.
        //        return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        //    }
        //    catch(Exception ex)
        //    {
        //        return null;
        //    }
        //}
        #region Settlement Download
        [MyAuthorize]
        [HttpPost]
        public async Task<ActionResult> RunReport(DateTime? SetDate, string ReportType,string CardScheme,string DrBank,string MerchantId,string Channel)
        {
            try
            {
                string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;

                if (Channel == string.Empty)
                {
                    Channel = "0";
                }

                int newchaneel = 0;
                newchaneel = int.Parse(Channel.ToString());
                var fileName = "";
                var handle = "";
                string subReport = null;
                string searchId = null;
                string reportName = "";
                string address = "";
                int mtype = 0;
                var companyname = repoComp.AllEager().FirstOrDefault().COMPANY_NAME;
                var ReportList = await _repo.GetReportListAsync();
                if (institutionId == 1)
                {
                    if (!string.IsNullOrEmpty(MerchantId.Trim()))
                    {
                        //subReport = "M";
                        searchId = MerchantId.Trim();
                       var sig =  repoM.AllEager(d=> d.MERCHANTID ==searchId).FirstOrDefault();
                        if(sig != null)
                        {
                            reportName = sig.MERCHANTNAME;
                        }
                    }
                    else
                    {
                        searchId = DrBank;
                        var sig = repoInst.AllEager(d => d.CBN_CODE == searchId).FirstOrDefault();
                        if (sig != null)
                        {
                            reportName = sig.INSTITUTION_NAME;
                        }
                    }
                    //string searchID,string CardScheme,string reportType,string reportClass, string subreporttype, string sett, string ReportPath, string logopath
                    var rec = ReportList.Where(d => d.Code == ReportType).FirstOrDefault();
                    fileName = string.Format("{0}-{1} for {2}.xlsx", rec.Description,reportName, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));

                    dsLoadclass dv = new dsLoadclass();
                    var dtMain = dv.generateDS(searchId, CardScheme, rec.reporttype, rec.reportClass, rec.subreporttype, SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"),newchaneel);
                   // var excelBytes = ExcelHelper.ExportDataSet(dtMain, rec.Description);
                    var excelBytes = ExcelHelper.ExportDatasetWithLogo(10, dtMain, rec.Description, reportName, mtype, address, companyname, logopath, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    handle = Guid.NewGuid().ToString();
                    Session[handle] = excelBytes;
                  
                    //switch (ReportType)
                    //{
                    //    case "1":
                    //        {

                    //            //if (!string.IsNullOrEmpty(CardScheme) && !string.IsNullOrEmpty(DrBank))
                    //            //{
                    //            //    subReport = "CARD";
                    //            //    searchId = DrBank;
                    //            //}
                    //            //else if (!string.IsNullOrEmpty(DrBank))
                    //            //{
                    //            //    searchId = DrBank;
                    //            //}

                    //            if (!string.IsNullOrEmpty(MerchantId.Trim()))
                    //            {
                    //                //subReport = "M";
                    //                searchId = MerchantId.Trim();
                    //            }
                    //            else
                    //            {
                    //                searchId = DrBank;
                    //            }
                    //            //string searchID,string CardScheme,string reportType,string reportClass, string subreporttype, string sett, string ReportPath, string logopath
                    //            var rec = ReportList.Where(d => d.Code == ReportType).FirstOrDefault();
                    //            fileName = string.Format("Settlement Report for {0}.xlsx", SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    //            var dtMain = rptSett.generateDS(searchId, CardScheme, rec.reporttype, rec.reportClass, rec.subreporttype, SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"), null, null);
                    //            var excelBytes = ExcelHelper.ExportDataSet(dtMain, "SETTLEMENT DETAIL");
                    //            handle = Guid.NewGuid().ToString();
                    //            Session[handle] = excelBytes;
                    //            break;
                    //        }
                    //    case "2":
                    //        {
                    //            fileName = string.Format("NEFT Report for {0}.xlsx", SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    //            var dtMain = rptSett.generateDS("", "", "NIBSS_ALL", "U", "NEFT", SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"), null, null);
                    //            var excelBytes = ExcelHelper.ExportDataSet(dtMain, "NEFT REPORT");
                    //            handle = Guid.NewGuid().ToString();
                    //            Session[handle] = excelBytes;
                    //            break;
                    //        }
                    //    case "3":
                    //        {
                    //            fileName = string.Format("NAPS Report for {0}.xlsx", SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    //            var dtMain = rptSett.generateDS("", "", "NIBSS_ALL", "U", "NAPS", SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"), null, null);
                    //            var excelBytes = ExcelHelper.ExportDataSet(dtMain, "NAPS REPORT");
                    //            handle = Guid.NewGuid().ToString();
                    //            Session[handle] = excelBytes;
                    //            break;
                    //        }
                    //    case "4":
                    //        {
                    //            ////fileName = string.Format("NEFT Report for {0}.xlsx", SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    //            ////var dtMain = rptSett.generateDS("NEFT", "U", SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"), null, null, null);
                    //            ////var excelBytes = ExcelHelper.ExportDataSet(dtMain, "SETTLEMENT DETAIL");
                    //            ////handle = Guid.NewGuid().ToString();
                    //            ////Session[handle] = excelBytes;
                    //            break;
                    //        }
                    //}
                }
                else
                {
                   var sig = _repo.GetInstitution(institutionId, false).FirstOrDefault();
                    if(sig != null)
                    {
                        searchId = sig.CBN_CODE;
                        reportName = sig.INSTITUTION_NAME;
                    }

                    dsLoadclass dv = new dsLoadclass();
                    fileName = string.Format("Settlement Report-{0} for {1}.xlsx",reportName, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    var dtMain = dv.generateDS(searchId, CardScheme, "ALL", "U", subReport, SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"),   int.Parse (Channel));
                    // var excelBytes = ExcelHelper.ExportDataSet(dtMain, "SETTLEMENT DETAIL");
                    var excelBytes = ExcelHelper.ExportDatasetWithLogo(10, dtMain, "SETTLEMENT DETAIL", reportName, mtype, address, companyname, logopath, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));

                    handle = Guid.NewGuid().ToString();
                    Session[handle] = excelBytes;
                }

                //List<FileContentResult> tr = new List<FileContentResult>();

                //tr.Add(File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName));
                //tr.Add(File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + 1));

                //Return file with the type and name. 
                //ContentType "application/vnd.ms-excel" does not work well for browsers other than IE.
                //return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
                return Json(new { FileGuid = handle,FileName = fileName, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<ActionResult> RunPdf(DateTime? SetDate, string ReportType, string CardScheme, string DrBank, string MerchantId,string Channel)
        {
            try
            {
                string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;

                var fileName = "";
                var handle = "";
                string subReport = null;
                string searchId = null;
                string reportName = "";
                string address = "";
                int mtype = 0;
                var companyname = repoComp.AllEager().FirstOrDefault().COMPANY_NAME;
                var ReportList = await _repo.GetReportListAsync();
                if (institutionId == 1)
                {
                    if (!string.IsNullOrEmpty(MerchantId.Trim()))
                    {
                        //subReport = "M";
                        searchId = MerchantId.Trim();
                        var sig = repoM.AllEager(d => d.MERCHANTID == searchId).FirstOrDefault();
                        if (sig != null)
                        {
                            reportName = sig.MERCHANTNAME;
                        }
                    }
                    else
                    {
                        searchId = DrBank;
                        var sig = repoInst.AllEager(d => d.CBN_CODE == searchId).FirstOrDefault();
                        if (sig != null)
                        {
                            reportName = sig.INSTITUTION_NAME;
                        }
                    }
                    //string searchID,string CardScheme,string reportType,string reportClass, string subreporttype, string sett, string ReportPath, string logopath
                    var rec = ReportList.Where(d => d.Code == ReportType).FirstOrDefault();
                    fileName = string.Format("{0}-{1} for {2}.xlsx", rec.Description, reportName, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    dsLoadclass dv = new dsLoadclass();
                    var dtMain = dv.generateDS(searchId, CardScheme, rec.reporttype, rec.reportClass, rec.subreporttype, SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"),  int.Parse (Channel));
                    ExportDataTableToPdf(dtMain,SetDate.GetValueOrDefault(),fileName, rec.Description,logopath);
                }
                else
                {
                    var sig = _repo.GetInstitution(institutionId, false).FirstOrDefault();
                    if (sig != null)
                    {
                        searchId = sig.CBN_CODE;
                        reportName = sig.INSTITUTION_NAME;
                    }
                    fileName = string.Format("Settlement Report-{0} for {1}.xlsx", reportName, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));
                    dsLoadclass dv = new dsLoadclass();
                    var dtMain = dv.generateDS(searchId, CardScheme, "ALL", "U", subReport, SetDate.GetValueOrDefault().ToString("yyyy-MM-dd"),int.Parse (Channel));
                    // var excelBytes = ExcelHelper.ExportDataSet(dtMain, "SETTLEMENT DETAIL");
                    //var excelBytes = ExcelHelper.ExportDatasetWithLogo(10, dtMain, "SETTLEMENT DETAIL", reportName, mtype, address, companyname, logopath, SetDate.GetValueOrDefault().ToString("dd-MM-yy"));

                    //handle = Guid.NewGuid().ToString();
                    //Session[handle] = dtMain;
                    ExportDataTableToPdf(dtMain, SetDate.GetValueOrDefault(), fileName,"Merchant Report",logopath);
                }

                //List<FileContentResult> tr = new List<FileContentResult>();

                //tr.Add(File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName));
                //tr.Add(File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + 1));

                //Return file with the type and name. 
                //ContentType "application/vnd.ms-excel" does not work well for browsers other than IE.
                //return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
                return Json(new { FileGuid = handle, FileName = fileName, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Error Processing File" }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult DownloadReport(string fileGuid, string fileName)
        {
            try
            {
                if (Session[fileGuid] != null)
                {
                    var excelBytes = (byte[])Session[fileGuid];

                    Session.Remove(fileGuid);
                    return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void ExportDataTableToPdf()
        {
            // creating datatable and adding dumy data
            DataTable dtEmployee = new DataTable();
            dtEmployee.Columns.Add("EmpId", typeof(Int32));
            dtEmployee.Columns.Add("Name", typeof(string));
            dtEmployee.Columns.Add("Gender", typeof(string));
            dtEmployee.Columns.Add("Salary", typeof(Int32));
            dtEmployee.Columns.Add("Country", typeof(string));
            dtEmployee.Rows.Add(1, "Rahul", "Male", 60000, "India");
            dtEmployee.Rows.Add(2, "John", "Male", 50000, "USA");
            dtEmployee.Rows.Add(3, "Mary", "Female", 75000, "UK");
            dtEmployee.Rows.Add(4, "Mathew", "Male", 80000, "Australia");

            // creating document object
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
            doc.Open();

            //Creating paragraph for header
            BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 16, 1, iTextSharp.text.BaseColor.ORANGE);
            Paragraph prgHeading = new Paragraph();
            prgHeading.Alignment = Element.ALIGN_LEFT;
            prgHeading.Add(new Chunk("Employee Details".ToUpper(), fntHead));
            doc.Add(prgHeading);

            //Adding paragraph for report generated by
            Paragraph prgGeneratedBY = new Paragraph();
            BaseFont btnAuthor = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntAuthor = new iTextSharp.text.Font(btnAuthor, 8, 2, iTextSharp.text.BaseColor.BLUE);
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT;
            prgGeneratedBY.Add(new Chunk("Report Generated by : ASPArticles", fntAuthor));
            prgGeneratedBY.Add(new Chunk("\nGenerated Date : " + DateTime.Now.ToShortDateString(), fntAuthor));
            doc.Add(prgGeneratedBY);

            //Adding a line
            Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            doc.Add(p);

            //Adding line break
            doc.Add(new Chunk("\n", fntHead));

            //Adding  PdfPTable
            PdfPTable table = new PdfPTable(dtEmployee.Columns.Count);

            for (int i = 0; i < dtEmployee.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dtEmployee.Columns[i].ColumnName);
                PdfPCell cell = new PdfPCell();
                cell.Phrase = new Phrase(cellText, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ffffff"))));
                cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#990000"));
                //cell.Phrase = new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(grdStudent.HeaderStyle.ForeColor)));
                //cell.BackgroundColor = new BaseColor(grdStudent.HeaderStyle.BackColor);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 5;
                table.AddCell(cell);
            }

            //writing table Data
            for (int i = 0; i < dtEmployee.Rows.Count; i++)
            {
                for (int j = 0; j < dtEmployee.Columns.Count; j++)
                {
                    table.AddCell(dtEmployee.Rows[i][j].ToString());
                }
            }

            doc.Add(table);
            doc.Close();
            writer.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;" + "filename=EmployeeDetails.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(doc);
            Response.End();
            //File(doc, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        private void ExportDataTableToPdf(DataTable dt,DateTime setDate,string fileName,string reportName,string logoPath)
        {
            Rectangle rec = new Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec);
            doc.SetPageSize(PageSize.A4.Rotate());
            PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
            doc.Open();

            PdfPTable table = new PdfPTable(2);

            table.TotalWidth =  700f;
            table.WidthPercentage = 100;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 1f ,2f});
            ////table.SpacingAfter = 20f;
            ////Company Logo
            var cell2 = ImageCell(logoPath, 75f, Element.ALIGN_LEFT,true);
            table.AddCell(cell2);
            doc.Add(table);
            ////Company Name and Address

            var phrase = new Phrase();
            phrase.Add(new Chunk("Xpress Payments\n".ToUpper(), FontFactory.GetFont("Calibri", 16, Font.BOLD, BaseColor.BLACK)));
            phrase.Add(new Chunk(reportName.ToUpper(), FontFactory.GetFont("Calibri", 14, Font.BOLD, BaseColor.BLACK)));

            cell2 = PhraseCell(phrase, Element.ALIGN_CENTER);
            cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell2);
            doc.Add(table);
            //Creating paragraph for header

            BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font fntHead = new iTextSharp.text.Font(bfntHead, 16, 1, iTextSharp.text.BaseColor.ORANGE);
            //Paragraph prgHeading = new Paragraph();
            //prgHeading.Alignment = Element.ALIGN_CENTER;
            //prgHeading.Add(new Chunk(logoPath, fntHead));
            //prgHeading.Add(new Chunk("Xpress Payments".ToUpper(), fntHead));
            //doc.Add(prgHeading);

            //Creating sub title
            //iTextSharp.text.Font fntSubTitle = new iTextSharp.text.Font(bfntHead, 12, 1, iTextSharp.text.BaseColor.ORANGE);
            //Paragraph prgSubTitle = new Paragraph();
            //prgSubTitle.Alignment = Element.ALIGN_CENTER;
            //prgSubTitle.Add(new Chunk(reportName.ToUpper(), fntSubTitle));
            //doc.Add(prgSubTitle);

            //Adding paragraph for report generated by
            Paragraph prgGeneratedBY = new Paragraph();
            BaseFont btnAuthor = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fntAuthor = new iTextSharp.text.Font(btnAuthor, 8, 2, iTextSharp.text.BaseColor.BLUE);
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT;
            //prgGeneratedBY.Add(new Chunk("Report Generated by : ASPArticles", fntAuthor));
            prgGeneratedBY.Add(new Chunk("\nSettlement Date : " + setDate.ToShortDateString(), fntAuthor));
            doc.Add(prgGeneratedBY);

            //Adding a line
            Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            doc.Add(p);

            //Adding line break
            doc.Add(new Chunk("\n", fntHead));

            //Adding  PdfPTable
             table = new PdfPTable(dt.Columns.Count);
            table.WidthPercentage = 100.0f;
            // Options: Element.ALIGN_LEFT (or 0), Element.ALIGN_CENTER (1), Element.ALIGN_RIGHT (2).
            table.HorizontalAlignment = Element.ALIGN_LEFT;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                PdfPCell cell = new PdfPCell();
                cell.Phrase = new Phrase(cellText, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"))));
                cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff9800"));
                //cell.Phrase = new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(grdStudent.HeaderStyle.ForeColor)));
                //cell.BackgroundColor = new BaseColor(grdStudent.HeaderStyle.BackColor);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell.PaddingBottom = 5;
                table.AddCell(cell);
            }

            //writing table Data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = "";
                    PdfPCell cell = new PdfPCell();
                    if (dt.Rows[i][j].GetType() == typeof(decimal))
                    {
                        decimal value;
                        var suc = decimal.TryParse(dt.Rows[i][j].ToString(), out value);
                        if (suc)
                        {
                            cellText = SmartObj.FormatMoney(value.ToString("F"));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }
                    }
                    else if (dt.Rows[i][j].GetType() == typeof(DateTime))
                    {
                        DateTime date;
                        var suc = DateTime.TryParse( dt.Rows[i][j].ToString(),out date);
                        if (suc)
                        {
                            cellText = date.ToString("dd-MM-yyyy");
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        else
                        {
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                    }
                    else
                    {
                        cellText = dt.Rows[i][j].ToString();
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    }



                    // cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff9800"));
                    //cell.Phrase = new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(grdStudent.HeaderStyle.ForeColor)));
                    //cell.BackgroundColor = new BaseColor(grdStudent.HeaderStyle.BackColor);
                    cell.Phrase = new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 8, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"))));

                    table.AddCell(cell);
                }
            }

            doc.Add(table);
            doc.Close();
            
            writer.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;" + string.Format("filename={0}.pdf", fileName));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(doc);
            Response.End();
            //File(doc, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        private PdfPCell ImageCell(string path, float scale, int align,bool isLocal)
        {

            Image image = isLocal  ? Image.GetInstance(path) : Image.GetInstance(HttpContext.Server.MapPath(path));
            image.ScalePercent(scale);
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
        #endregion Settlement Download

        #region Settlement Enquiry
        [MyAuthorize]
        public async Task<ActionResult> SetEnquiry()
        {
            //var merchant = await _repo.GetMerchantDetailAsync("", "");
            //ViewBag.MerchantList = new SelectList(merchant, "ITBID", "MERCHANTNAME");
            var scheme = await _repo.GetCardSchemeAsync(0, true, "Active");
            ViewBag.CardSchemeList = new SelectList(scheme, "CARDSCHEME", "CARDSCHEME_DESC");
            var issuer = await _repo.GetInstitutionAsync(0, true, "Active");
            ViewBag.IssuerList = new SelectList(issuer, "CBN_CODE", "INSTITUTION_NAME");
            var channel = await _repo.GetChannelAsync(0, true, "Active");
            ViewBag.ChannelList = new SelectList(channel, "CODE", "DESCRIPTION");
            var institution = await _repo.GetInstitutionAsync(0, true, "Active");
            var insts = institution.Where(x => x.IS_BANK != null && x.IS_BANK.ToUpper() == "Y");
            ViewBag.InstitutionList = new SelectList(insts, "CBN_CODE", "INSTITUTION_NAME");
            var acquirer = await _repo.GetInstitutionAsync(0, true, "Active");
            ViewBag.AcquirerList = new SelectList(acquirer, "CBN_CODE", "INSTITUTION_NAME");
            //var rvhead = await _repo.GetRvHeadAsync(0);
            //ViewBag.RevenueHeadList = new SelectList(rvhead, "RVGROUPCODE", "DESCRIPTION");
            //ViewBag.ReportList = new SelectList(SmartObj.GetReportList(), "CODE", "DESCRIPTION");
            ViewBag.HeaderTitle = "Generate Settlement Enquiry Report";
            //ViewBag.HeaderTitle2 = "Date Range";

            return View();
        }

        public async Task<ActionResult> GetSettlementEnquiry(int draw, int start, int length, SettlementEnquiryObj obj)
        {
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
               // DateTime start_date;
               // DateTime end_date;
                //var stSuc = DateTime.TryParse(GetDate(obj.fromDate), out start_date);
                //var adSuc = DateTime.TryParse(GetDate(obj.toDate), out end_date);
                //if (!stSuc || !adSuc)
                //{
                //    dataTableData.draw = draw;

                //    dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                //                                    // int recordsFiltered = 0;
                //    dataTableData.data = new List<SettlementEnquiryObj>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                //    dataTableData.recordsFiltered = 0;
                //}
                //else
                //{
                    dataTableData.draw = draw;
                    var data = await _repo2.GetSettlementEnquiryAsync(obj);
                    //dataTableData.recordsTotal = cnt; //  TOTAL_ROWS;
                    //int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = data.Count; // recordsFiltered;
               // }
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<SM_SETTLEMENTDETAIL>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadSettEnquiry(SettlementEnquiryObj obj)
        {
            try
            {
                string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;

                //Call to get Excel byte array.
                string fileName = string.Format("Settlement Enquiry Report {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
                DateTime start_date;
                DateTime end_date;
                var companyname = repoComp.AllEager().FirstOrDefault().COMPANY_NAME;
                var stSuc = DateTime.TryParse(GetDate(obj.fromDate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(obj.toDate), out end_date);
                var data = await _repo2.GetSettlementEnquiryAsync(obj);
                var dt = SmartObj.ToDataTable(data);
                var excelBytes = ExcelHelper.ExportDatasetWithLogo2(10, dt, logopath, companyname, "Settlement Enquiry");

                //var excelBytes = DumpExcelSettEnquiry(dt, fileName);

                //Set file name.

                return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
            }
            catch
            {
                return null;
            }
        }
        private byte[] DumpExcelSettEnquiry(DataTable tbl, string fileName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Settlement Enquiry");
                ws.Cells["A1"].Value = "PAY REFERENCE";
                ws.Cells["B1"].Value = "TRANSACTION REFNO";
                ws.Cells["C1"].Value = "TRANSACTION ID";
                ws.Cells["D1"].Value = "MERCHANTDEPOSIT";
                ws.Cells["E1"].Value = "DEBITBANK";
                ws.Cells["F1"].Value = "MERCHANTID";
                ws.Cells["G1"].Value = "MERCHANTNAME";
                ws.Cells["H1"].Value = "CARDSCHEME";
                ws.Cells["I1"].Value = "TRANAMOUNT";
                ws.Cells["J1"].Value = "ACQUIRERFIID";
                ws.Cells["K1"].Value = "ISSUERFIID";
                ws.Cells["L1"].Value = "SETTLEMENTACCOUNT";
                ws.Cells["M1"].Value = "CHANNELID";
                ws.Cells["N1"].Value = "MASKPAN";
                ws.Cells["O1"].Value = "TERMINALID";
                ws.Cells["P1"].Value = "LOCATION";
                ws.Cells["Q1"].Value = "INVOICENO";



                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A2"].LoadFromDataTable(tbl, false);

                //Format the header for column 1-3
                //using (ExcelRange rng = ws.Cells["A1:C1"])
                //{
                //    rng.Style.Font.Bold = true;
                //    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                //    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                //    rng.Style.Font.Color.SetColor(Color.White);
                //}

                //Example how to Format Column 1 as numeric 
                //using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
                //{
                //    col.Style.Numberformat.Format = "#,##0.00";
                //    col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //}
                return pck.GetAsByteArray();

            }
        }


        string GetDate(string date)
        {
            try
            {
                if (string.IsNullOrEmpty(date))
                {
                    return "";
                }
                else
                {
                    if (date.Length == 10)
                    {
                        var day = date.Substring(0, 2);
                        var month = date.Substring(3, 2);
                        var year = date.Substring(6, 4);
                        return string.Concat(year, "-", month, "-", day);
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }

        }
        #endregion Settlement Enquiry

      

    }
}