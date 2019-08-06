using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Data.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SettlementMaster.App.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class ReportController : Controller
    {
        int institutionId;
        public ReportController()
        {
            
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                //roleId = user.UserRole;
                institutionId = user.InstitutionId;
               // fullName = user.FullName;
               // deptCode = user.DeptCode;
            }

        }
        IDapperReportSettings _repo = new DapperReportSettings();
        IDapperGeneralSettings _repo2 = new DapperGeneralSettings();
        private const int TOTAL_ROWS = 995;
        private static readonly List<DataItem> _data = CreateData();
        
        // here we simulate data from a database table.
        // !!!!DO NOT DO THIS IN REAL APPLICATION !!!!
        private static List<DataItem> CreateData()
        {
            Random rnd = new Random();
            List<DataItem> list = new List<DataItem>();
            for (int i = 1; i <= TOTAL_ROWS; i++)
            {
                DataItem item = new DataItem();
                item.Name = "Name_" + i.ToString().PadLeft(5, '0');
                DateTime dob = new DateTime(1900 + rnd.Next(1, 100), rnd.Next(1, 13), rnd.Next(1, 28));
                item.Age = ((DateTime.Now - dob).Days / 365).ToString();
                item.DoB = dob.ToShortDateString();
                list.Add(item);
            }
            return list;
        }

        private int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
        }

        private int SortInteger(string s1, string s2, string sortDirection)
        {
            int i1 = int.Parse(s1);
            int i2 = int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        private int SortDateTime(string s1, string s2, string sortDirection)
        {
            DateTime d1 = DateTime.Parse(s1);
            DateTime d2 = DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

        // here we simulate SQL search, sorting and paging operations
        // !!!! DO NOT DO THIS IN REAL APPLICATION !!!!
        private List<DataItem> FilterData(ref int recordFiltered, int start, int length, string search, int sortColumn, string sortDirection)
        {
            List<DataItem> list = new List<DataItem>();
            if (search == null)
            {
                list = _data;
            }
            else
            {
                // simulate search
                foreach (DataItem dataItem in _data)
                {
                    if (dataItem.Name.ToUpper().Contains(search.ToUpper()) ||
                        dataItem.Age.ToString().Contains(search.ToUpper()) ||
                        dataItem.DoB.ToString().Contains(search.ToUpper()))
                    {
                        list.Add(dataItem);
                    }
                }
            }

            // simulate sort
            if (sortColumn == 0)
            {// sort Name
                list.Sort((x, y) => SortString(x.Name, y.Name, sortDirection));
            }
            else if (sortColumn == 1)
            {// sort Age
                list.Sort((x, y) => SortInteger(x.Age, y.Age, sortDirection));
            }
            else if (sortColumn == 2)
            {   // sort DoB
                list.Sort((x, y) => SortDateTime(x.DoB, y.DoB, sortDirection));
            }

            recordFiltered = list.Count;

            // get just one page of data
            list = list.GetRange(start, Math.Min(length, list.Count - start));

            return list;
        }
        #region Audit Report
        // GET: Report
        [MyAuthorize]
        public ActionResult Audit()
        {
            var rec = _repo2.GetUser(0, true);  //repoSession.FindAsync(id);              
            ViewBag.UserList = new SelectList(rec, "UserName", "FullName");

            return View();
        }

        //public async Task<ActionResult> GetApprovalDetail(int draw, int start, int length, string batchno)
        //{
        //    //string search = Request.QueryString["search[value]"];
        //    //int sortColumn = -1;
        //    //string sortDirection = "asc";
        //    //if (length == -1)
        //    //{
        //    //    length = TOTAL_ROWS;
        //    //}

        //    //// note: we only sort one column at a time
        //    //if (Request.QueryString["order[0][column]"] != null)
        //    //{
        //    //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
        //    //}
        //    //if (Request.QueryString["order[0][dir]"] != null)
        //    //{
        //    //    sortDirection = Request.QueryString["order[0][dir]"];
        //    //}
        //    DataTableData dataTableData = new DataTableData();

        //    try
        //    {
        //        start++;
        //        //DateTime start_date;
        //        // DateTime end_date;

        //        dataTableData.draw = draw;
        //        //var cnt = await _repo.GetRptAuditTotalCountAsync(institutionId, start_date, end_date, userid);
        //        var data = await _repo.GetApprovalDetailAsync(batchno);
        //        dataTableData.recordsTotal = data.Count; //  TOTAL_ROWS;
        //                                                 // int recordsFiltered = 0;
        //        dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
        //        dataTableData.recordsFiltered = data.Count; // recordsFiltered;

        //    }
        //    catch (Exception ex)
        //    {
        //        dataTableData.draw = draw;

        //        dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
        //                                        // int recordsFiltered = 0;
        //        dataTableData.data = new List<Rpt_LoginUser>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
        //        dataTableData.recordsFiltered = 0;
        //    }
        //    return Json(dataTableData, JsonRequestBehavior.AllowGet);
        //}
        public async Task<ActionResult> ApprovalDetail(string bid)
        {
            try
            {
                var rec = await _repo.GetApprovalDetailAsync(bid);
                return PartialView("_ApprovalDetail", rec);
                //return Json(new { data_html = html, RespCode = 0, RespMessage = "Record "},JsonRequestBehavior.AllowGet);
               // return PartialView("")
            }
            catch (Exception ex)
            {
                return PartialView("_ApprovalDetail"); // Json(new { RespCode = 1, RespMessage = "Problem Processing Validation. " });
            }
        }

        public ActionResult AjaxGetJsonData(int draw, int start, int length, string fromdate, string todate)
        {
            string search = Request.QueryString["search[value]"];
            //string from__date = Request.QueryString["data[fromdate]"];
            //string to__date = Request.QueryString["data[todate]"];
            int sortColumn = -1;
            string sortDirection = "asc";
            if (length == -1)
            {
                length = TOTAL_ROWS;
            }

            // note: we only sort one column at a time
            if (Request.QueryString["order[0][column]"] != null)
            {
                sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            }
            if (Request.QueryString["order[0][dir]"] != null)
            {
                sortDirection = Request.QueryString["order[0][dir]"];
            }

            DataTableData dataTableData = new DataTableData();
            dataTableData.draw = draw;
            dataTableData.recordsTotal = TOTAL_ROWS;
            int recordsFiltered = 0;
            dataTableData.data = FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
            dataTableData.recordsFiltered = recordsFiltered;

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetAuditTrail(int draw, int start, int length,string fromdate,string todate,string userid)
        {
            //string search = Request.QueryString["search[value]"];
            //int sortColumn = -1;
            //string sortDirection = "asc";
            //if (length == -1)
            //{
            //    length = TOTAL_ROWS;
            //}

            //// note: we only sort one column at a time
            //if (Request.QueryString["order[0][column]"] != null)
            //{
            //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            //}
            //if (Request.QueryString["order[0][dir]"] != null)
            //{
            //    sortDirection = Request.QueryString["order[0][dir]"];
            //}
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                if (!stSuc || !adSuc)
                {
                    dataTableData.draw = draw;

                    dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                    // int recordsFiltered = 0;
                    dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = 0;
                }
                else
                {
                    dataTableData.draw = draw;
                    var cnt = await _repo.GetRptAuditTotalCountAsync(institutionId, start_date, end_date,userid);
                    var data = await _repo.GetRptAuditAsync(institutionId, start_date, end_date, start, length,userid);
                    dataTableData.recordsTotal = cnt; //  TOTAL_ROWS;
                                                      // int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = data.Count; // recordsFiltered;
                }
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadAudit(string fromdate, string todate,string userid)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("Audit Trail Report {0}.xlsx",DateTime.Now.ToString("dd-MM-yyyy"));
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                var data = await _repo.GetRptAuditAsync(institutionId, start_date, end_date,userid:userid,isAll:true);
                var dt = SmartObj.ToDataTable(data);
                var excelBytes = DumpExcelAudit(dt, fileName);

                //Set file name.
               
                return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
            }
            catch
            {
                return null;
            }
        }
    

        private byte[] DumpExcelAudit(DataTable tbl, string fileName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Audit Trail");

                ws.Cells["A1"].Value = "FORM NAME";
                ws.Cells["B1"].Value = "COLUMN CHANEGED";
                ws.Cells["C1"].Value = "OLD RECORD";
                ws.Cells["D1"].Value = "NEW RECORD";
                ws.Cells["E1"].Value = "USER";
                ws.Cells["F1"].Value = "ACTION";
                ws.Cells["G1"].Value = "DATE CREATED";
                ws.Cells["H1"].Value = "RECORD KEY";



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
        #endregion Audit Report
        #region Login User Report
        // GET: Report
        [MyAuthorize]
        public ActionResult LoginUser()
        {
            return View();
        }
        
        public async Task<ActionResult> GetLoginUser(int draw, int start, int length)
        {
            //string search = Request.QueryString["search[value]"];
            //int sortColumn = -1;
            //string sortDirection = "asc";
            //if (length == -1)
            //{
            //    length = TOTAL_ROWS;
            //}

            //// note: we only sort one column at a time
            //if (Request.QueryString["order[0][column]"] != null)
            //{
            //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            //}
            //if (Request.QueryString["order[0][dir]"] != null)
            //{
            //    sortDirection = Request.QueryString["order[0][dir]"];
            //}
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
                //DateTime start_date;
               // DateTime end_date;
                
                    dataTableData.draw = draw;
                    //var cnt = await _repo.GetRptAuditTotalCountAsync(institutionId, start_date, end_date, userid);
                    var data = await _repo.GetLoginUserAsync(start, length);
                    dataTableData.recordsTotal = data.Count; //  TOTAL_ROWS;
                                                      // int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = data.Count; // recordsFiltered;
                
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<Rpt_LoginUser>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadLoginUser(string fromdate, string todate, string userid)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("Login User Report {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
               
                var data = await _repo.GetLoginUserAsync();
                var dt = SmartObj.ToDataTable(data);
                var excelBytes = DumpExcelLoginUser(dt, fileName);

                //Set file name.

                return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
            }
            catch
            {
                return null;
            }
        }


        private byte[] DumpExcelLoginUser(DataTable tbl, string fileName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Login Users");

                ws.Cells["A1"].Value = "LOGIN ID";
                ws.Cells["B1"].Value = "FULL NAME";
                ws.Cells["C1"].Value = "EMAIL";
                ws.Cells["D1"].Value = "ROLE";
                ws.Cells["E1"].Value = "INSTITUTION";
                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A2"].LoadFromDataTable(tbl, false);
                
                return pck.GetAsByteArray();

            }
        }
        #endregion Login User Report
        #region Login Audit Report
        // GET: Report
        [MyAuthorize]
        public ActionResult LoginAudit()
        {
            var rec = _repo2.GetUser(0, true);  //repoSession.FindAsync(id);              
            ViewBag.UserList = new SelectList(rec, "UserName", "FullName");

            return View();
        }
       
        public async Task<ActionResult> GetLoginAudit(int draw, int start, int length, string fromdate, string todate, string userid)
        {
            //string search = Request.QueryString["search[value]"];
            //int sortColumn = -1;
            //string sortDirection = "asc";
            //if (length == -1)
            //{
            //    length = TOTAL_ROWS;
            //}

            //// note: we only sort one column at a time
            //if (Request.QueryString["order[0][column]"] != null)
            //{
            //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            //}
            //if (Request.QueryString["order[0][dir]"] != null)
            //{
            //    sortDirection = Request.QueryString["order[0][dir]"];
            //}
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                if (!stSuc || !adSuc)
                {
                    dataTableData.draw = draw;

                    dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                    // int recordsFiltered = 0;
                    dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = 0;
                }
                else
                {
                    dataTableData.draw = draw;
                    var cnt = await _repo.GetLoginAuditTotalCountAsync(start_date, end_date, userid);
                    var data = await _repo.GetLoginAuditAsync(start_date, end_date, start, length, userid);
                    dataTableData.recordsTotal = cnt; //  TOTAL_ROWS;
                                                      // int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = data.Count; // recordsFiltered;
                }
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadLoginAudit(string fromdate, string todate, string userid)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("Login Audit Report {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                var data = await _repo.GetLoginAuditAsync(start_date, end_date, userId: userid, IsAll: true);
                var rec = data.Select(d => new { d.USERID, d.FULLNAME, d.LOGINDATESTRING, d.LOGOUTDATESTRING, d.IP_ADDRESS, d.BROWSER }).ToList();
                var dt = SmartObj.ToDataTable(rec);
                var excelBytes = DumpExcelLoginAudit(dt, fileName);

                //Set file name.

                return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
            }
            catch
            {
                return null;
            }
        }


        private byte[] DumpExcelLoginAudit(DataTable tbl, string fileName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Login Audit Report");

                ws.Cells["A1"].Value = "USER NAME";
                ws.Cells["B1"].Value = "FULL NAME";
                ws.Cells["C1"].Value = "LOGIN DATE";
                ws.Cells["D1"].Value = "LOGOUT DATE";
                ws.Cells["E1"].Value = "IP ADDRESS";
                ws.Cells["F1"].Value = "BROWSER";
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
        #endregion Login Audit Report
        #region Login Attempt Report
        // GET: Report
        [MyAuthorize]
        public ActionResult LoginAttempt()
        {
            var rec = _repo2.GetUser(0, true);  //repoSession.FindAsync(id);              
            ViewBag.UserList = new SelectList(rec, "UserName", "FullName");

            return View();
        }

        public async Task<ActionResult> GetLoginAttempt(int draw, int start, int length, string fromdate, string todate, string userid)
        {
            //string search = Request.QueryString["search[value]"];
            //int sortColumn = -1;
            //string sortDirection = "asc";
            //if (length == -1)
            //{
            //    length = TOTAL_ROWS;
            //}

            //// note: we only sort one column at a time
            //if (Request.QueryString["order[0][column]"] != null)
            //{
            //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            //}
            //if (Request.QueryString["order[0][dir]"] != null)
            //{
            //    sortDirection = Request.QueryString["order[0][dir]"];
            //}
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                if (!stSuc || !adSuc)
                {
                    dataTableData.draw = draw;

                    dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                    // int recordsFiltered = 0;
                    dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = 0;
                }
                else
                {
                    dataTableData.draw = draw;
                    var cnt = await _repo.GetLoginAttemptTotalCountAsync(start_date, end_date, userid);
                    var data = await _repo.GetLoginAttemptAsync(start_date, end_date, start, length, userid);
                    dataTableData.recordsTotal = cnt; //  TOTAL_ROWS;
                                                      // int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = data.Count; // recordsFiltered;
                }
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadLoginAttempt(string fromdate, string todate, string userid)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("Login Attempt Report {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                var data = await _repo.GetLoginAttemptAsync(start_date, end_date, userid: userid, IsAll: true);
                var rec = data.Select(d => new { d.USERID, d.FULLNAME, d.ATTEMPTDATESTRING, d.IP_ADDRESS, d.BROWSER }).ToList();
                var dt = SmartObj.ToDataTable(rec);
                var excelBytes = DumpExcelLoginAttempt(dt, fileName);

                //Set file name.

                return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
            }
            catch
            {
                return null;
            }
        }


        private byte[] DumpExcelLoginAttempt(DataTable tbl, string fileName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Login Attempt Report");

                ws.Cells["A1"].Value = "USER NAME";
                ws.Cells["B1"].Value = "FULL NAME";
                ws.Cells["C1"].Value = "ATTEMPT DATE";
                ws.Cells["D1"].Value = "IP ADDRESS";
                ws.Cells["E1"].Value = "BROWSER";
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
        #endregion Login Attempt Report
        ///////

        #region Settlement Exemption Report
        // GET: Report
        [MyAuthorize]
        public ActionResult ExemptionReport()
        {
            //var rec = _repo2.GetUser(0, true);  //repoSession.FindAsync(id);              
            //ViewBag.UserList = new SelectList(rec, "UserName", "FullName");
            return View();
        }

        public async Task<ActionResult> GetExemptionReport(int draw, int start, int length, string reportdate)
        {
            //string search = Request.QueryString["search[value]"];
            //int sortColumn = -1;
            //string sortDirection = "asc";
            //if (length == -1)
            //{
            //    length = TOTAL_ROWS;
            //}

            //// note: we only sort one column at a time
            //if (Request.QueryString["order[0][column]"] != null)
            //{
            //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            //}
            //if (Request.QueryString["order[0][dir]"] != null)
            //{
            //    sortDirection = Request.QueryString["order[0][dir]"];
            //}
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
                DateTime report_date;
                //DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(reportdate), out report_date);
                //var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                if (!stSuc)
                {
                    dataTableData.draw = draw;

                    //dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                    // int recordsFiltered = 0;
                    dataTableData.data = new List<Rpt_Exemption>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                   // dataTableData.recordsFiltered = 0;
                }
                else
                {
                    dataTableData.draw = draw;
                    //var cnt = await _repo.GetLoginAttemptTotalCountAsync(report_date);
                    var data = await _repo.GetExemptionReportAsync(report_date,start, length);
                    //dataTableData.recordsTotal = cnt; //  TOTAL_ROWS;
                                                      // int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    //dataTableData.recordsFiltered = data.Count; // recordsFiltered;
                }
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<Rpt_Exemption>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadExemptionReport(string reportdate)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("Settlement Exemption Report {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
                DateTime report_date;
                //DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(reportdate), out report_date);
                //var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                var data = await _repo.GetExemptionReportAsync(report_date,IsAll: true);
                var rec = data.Select(d => new { d.MERCHANTID, d.TERMINALID, d.CREATEDATE, d.ERROR_MESSAGE, d.TRANDATETIME, d.TRANAMOUNT, d.PAYMENTREFERENCE, }).ToList();
                var dt = SmartObj.ToDataTable(rec);
                var excelBytes = DumpExcelExemptionReport(dt, fileName);

                //Set file name.

                return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
            }
            catch
            {
                return null;
            }
        }


        private byte[] DumpExcelExemptionReport(DataTable tbl, string fileName)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Settlement Exemption Report");

                ws.Cells["A1"].Value = "MERCHANT ID";
                ws.Cells["B1"].Value = "TERMINAL ID";
                ws.Cells["C1"].Value = "CREATE DATE";
                ws.Cells["D1"].Value = "ERROR MESSAGE";
                ws.Cells["E1"].Value = "TRANSACTION DATE";
                ws.Cells["F1"].Value = "PAYMENT REFERENCE";
                ws.Cells["G1"].Value = "TRANSACTION AMOUNT";
                //ws.Cells["E1"].Value = "BROWSER";
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
        #endregion Login Attempt Report
        ////
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
    }

    

    public class DataItem
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string DoB { get; set; }
    }
    public class DataItemAudit
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string DoB { get; set; }
    }

    public class DataTableData
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object data { get; set; }
    }
}