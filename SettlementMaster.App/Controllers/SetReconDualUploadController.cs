using Generic.Dapper.Data;
using Generic.Dapper.Model;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Data;
using SettlementMaster.App.Models;
//using System.Transactions;
using Generic.Dapper.Utility;
using Generic.Data.Utilities;
using System.Text;
using Generic.Dapper.Utilities;
using System.Text.RegularExpressions;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using Generic.Dapper.ExcelUtility;
using OfficeOpenXml;

namespace SettlementMaster.App.Controllers
{
    public class SetReconDualUploadController : Controller
    {
        int institutionId;
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null; private readonly IRepository<SM_CURRENCY> repoCur = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_INSTITUTION> repoInst = null;
        private readonly IRepository<AspNetUser> repoUsers = null;
        private readonly IRepository<SM_UPLOADSETRECON> repoSetReconDualUpload = null;
        private readonly IRepository<SM_UPLOADSETRECON_TEMP> repoSetReconDualUploadTemp = null;
        private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
        const string Single = "SINGLE";
        const string Batch = "BATCH";
        int roleId;
        int menuId = 45;
        string fullName;
        string deptCode;
        string userEmail;
        string active = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.ACTIVE);
        string inActive = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.INACTIVE);
        string open = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.OPEN);
        string close = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.CLOSED);
        string approve = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.APPROVED);
        string reject = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.REJECTED);
        string unapprove = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.UNAPPROVED);
        readonly string eventInsert = "New";
        readonly string eventEdit = "Modify";
        readonly string eventDelete = "Delete";
        public SetReconDualUploadController()
        {
            
            uow = new UnitOfWork();

            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoUsers = new Repository<AspNetUser>(uow);
            repoCur = new Repository<SM_CURRENCY>(uow);
            repoSetReconDualUpload = new Repository<SM_UPLOADSETRECON>(uow);
            repoSetReconDualUploadTemp = new Repository<SM_UPLOADSETRECON_TEMP>(uow);
            repoScheme = new Repository<SM_CARDSCHEME>(uow);
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleId = user.UserRole;
                institutionId = user.InstitutionId;
                deptCode = user.DeptCode;
                fullName = user.FullName;
            }
        }
        // GET: MerchantPre
       // [MyAuthorize]
        //public ActionResult Index()
        //{
        //    BindCombo();
        //    return View();
        //}
        [MyAuthorize]
        public ActionResult SetReconUpload()
        {
            //BindCombo();
            return View();
        }

        

        string GetStringFromList(List<string> val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div style=""color:red;font-size:11px"">");
            foreach (var d in val)
            {
                sb.AppendLine(@"<i class=""fa-arrow-right fa""> </i> " + d + "<br/>");
            }
            sb.AppendLine("</div>");
            var l = sb.ToString();
            return l;
        }
        string GetString(string val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div style=""color:red;font-size:11px"">");

            sb.AppendLine(@"<i class=""fa-arrow-right fa""> </i> " + val + "<br/>");

            sb.AppendLine("</div>");
            var l = sb.ToString();
            string msg = string.Format(@"<small id=""disWarning"" style=""color: red"" data-toggle=""popover"" data-trigger=""hover""  
                            data-html=""true2"" data-content='{0}'><i class=""fa fa-info-circle""></i> New</small>", l);

            return msg;
        }

        
        void SendUploadErrorNotification(string message, int record, string batchid, string fullName)
        {
            List<EmailObj> lst = new List<EmailObj>();
            lst.Add(new EmailObj()
            {
                Email = userEmail,
                RoleId = roleId
            });
            var mail = NotificationSystem.SendEmail(new EmailMessage()
            {
                EmailAddress = lst,

                emailSubject = "Upload Notififcation.",

                EmailContent = new EmailerNotification().PopulateUploadErrorMessage(message, record, batchid, fullName),
                EntryDate = DateTime.Now,
                HasAttachment = false
            });
        }
        void BindCombo()
        {
            var rec = _repo.GetInstitution(0, true, "Active").Where(d => d.CBN_CODE != null).ToList(); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
            ViewBag.BankList = new SelectList(rec, "CBN_CODE", "INSTITUTION_NAME");

        }
        void BindCombo2()
        {
            var rec = _repo.GetInstitution(0, true, "Active").Where(d => d.CBN_CODE != null).ToList(); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
            ViewBag.BankList = new SelectList(rec, "CBN_CODE", "INSTITUTION_NAME");
            var scheme = _repo.GetCardScheme(0, true, "Active"); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
            ViewBag.SchemeList = new SelectList(scheme, "CARDSCHEME", "CARDSCHEME_DESC");
            ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus(), "Code", "Description");
        }


        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Validate()
        {
            try
            {
                //var rv = new SetReconUpldSession();
                //var errCount = ValidateUpload();
                //var rec = rv.GetSetReconUpload(User.Identity.Name);
                //var sucCount = rec.Count - errCount;
                var rv = new SetReconDualUpldSession();
                var rec = rv.DualReconcileBatch(User.Identity.Name);
                var html = PartialView("_SetReconDualUpld", rec).RenderToString();
                return Json(new { data_html = html, RespCode = 0, RespMessage = "Record ", SucCount = 0, FailCount = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Validation. " });
            }
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult UploadFilesA()
        {
            IList<SetReconDualUpldObj> model = null;

          
            try
            {
                var rc = Request.Files;
                //  var dd = Request.Form["requestType"];
                if (rc != null)
                {
                    var file = rc[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var stream = file.InputStream;
                        var fileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        if (ext != ".xlsx")
                        {
                            return Json(new { RespCode = 1, RespMessage = "Please Upload Using .xlsx file" });
                        }

                        if (!Directory.Exists(Server.MapPath("~/UploadFiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                        }
                        var path = Path.Combine(Server.MapPath("~/UploadFiles"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        var dataList = ExcelReader.GetDataToList(path, addRecord); // ExxcellReaderClosedXml.GetDataToList(path, addRecord);
                                                                                   //int cnt = 0;
                        var rv = new SetReconDualUpldSession();
                        //var obj = new SetReconDualUpldObj();
                        //if (obj.BATCHTYPE == null)
                        //{
                            var cnt = rv.PostSetReconDualUploadBulkA(dataList.ToList(), User.Identity.Name);
                            if (cnt > 0)
                            {
                                var rst = rv.GetSetReconDualUpload(User.Identity.Name);
                                var html = PartialView("_SetReconDualUpld", rst).RenderToString();
                                return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                            }
                            else
                            {
                                var html = PartialView("_SetReconDualUpld").RenderToString();
                                return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                            }
                        //}
                        //else
                        //{
                        //    var cntb = rv.PostSetReconDualUploadBulkb(dataList.ToList(), User.Identity.Name);
                        //    if (cntb > 0)
                        //    {
                        //        var rst = rv.GetSetReconDualUpload(User.Identity.Name);
                        //        var html = PartialView("_SetReconDualUpld", rst).RenderToString();
                        //        return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        //    }
                        //    else
                        //    {
                        //        var html = PartialView("_SetReconDualUpld").RenderToString();
                        //        return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                        //    }
                        //}
                        

                        
                    }
                }
                // If we got this far, something failed, redisplay form
                //return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            return Json(new { data = model, BatchId = "", RespCode = 0, RespMessage = "File Uploaded Successfully" });
        }

        [HttpPost]
        public ActionResult UploadFilesB()
        {
            IList<SetReconDualUpldObj> model = null;


            try
            {
                var rc = Request.Files;
                //  var dd = Request.Form["requestType"];
                if (rc != null)
                {
                    var file = rc[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var stream = file.InputStream;
                        var fileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        if (ext != ".xlsx")
                        {
                            return Json(new { RespCode = 1, RespMessage = "Please Upload Using .xlsx file" });
                        }

                        if (!Directory.Exists(Server.MapPath("~/UploadFiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                        }
                        var path = Path.Combine(Server.MapPath("~/UploadFiles"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        var dataList = ExcelReader.GetDataToList(path, addRecord); // ExxcellReaderClosedXml.GetDataToList(path, addRecord);
                                                                                   //int cnt = 0;
                        var rv = new SetReconDualUpldSession();
                        var obj = new SetReconDualUpldObj();
                        //if (obj.BATCHTYPE == null)
                        //{
                        var cnt = rv.PostSetReconDualUploadBulkB(dataList.ToList(), User.Identity.Name);
                        if (cnt > 0)
                        {
                            var rst = rv.GetSetReconDualUpload(User.Identity.Name);
                            var html = PartialView("_SetReconDualUpld", rst).RenderToString();
                            return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        }
                        else
                        {
                            var html = PartialView("_SetReconDualUpld").RenderToString();
                            return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                        }
                        //}
                        //else
                        //{
                        //    var cntb = rv.PostSetReconDualUploadBulkb(dataList.ToList(), User.Identity.Name);
                        //    if (cntb > 0)
                        //    {
                        //        var rst = rv.GetSetReconDualUpload(User.Identity.Name);
                        //        var html = PartialView("_SetReconDualUpld", rst).RenderToString();
                        //        return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        //    }
                        //    else
                        //    {
                        //        var html = PartialView("_SetReconDualUpld").RenderToString();
                        //        return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                        //    }
                        //}



                    }
                }
                // If we got this far, something failed, redisplay form
                //return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            return Json(new { data = model, BatchId = "", RespCode = 0, RespMessage = "File Uploaded Successfully" });
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Process()
        {
            string batchId = "";
            try
            {
                var rv = new SetReconDualUpldSession();
                var rec = rv.GetSetReconDualUpload(User.Identity.Name);
                //  isUp = userInstitutionItbid == 1 ? true : false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 30, 0)))
                //{
                int errorcnt = 0;

                DateTime curDate = DateTime.Now;

                batchId = SmartObj.GenRefNo2();

                int i = 0;
                int valCount = rec.Count(f => f.VALIDATIONERRORSTATUS == false);
                foreach (var d in rec)
                {
                    if (i == 0)
                    {
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = eventInsert,
                            MENUID = menuId,
                            //MENUNAME = "",
                            // RECORDID = objG.ITBID,
                            STATUS = open,
                            //TABLENAME = "SM_FREQUENCY",
                            URL = Request.FilePath,
                            USERID = User.Identity.Name,
                            INSTITUTION_ITBID = institutionId,
                            BATCHID = batchId,
                            POSTTYPE = Batch

                        };
                        repoAuth.Insert(auth);
                    }
                    i++;
                    if (d.VALIDATIONERRORSTATUS == true)
                    {
                        errorcnt++;
                        continue;
                    }

                //    var obj = new SM_UPLOADSETRECON_TEMP()// SM_MERCHANTTERMINALUPLD()
                //    {
                //        REFERENCENO = d.REFERENCENO,
                //        CARDTYPE = d.CARDTYPE,
                //        TRANSACTIONTYPE = d.TRANSACTIONTYPE,
                //        TRANSACTIONDATETIME = d.TRANSACTIONDATETIME,
                //        SETTLEMENTDATE = d.SETTLEMENTDATE,
                //        MASKEDPAN = d.MASKEDPAN,
                //        MERCHANTID = d.MERCHANTID,
                //        MERCHANTACCOUNT = d.MERCHANTACCOUNT,
                //        MERCHANTNAME = d.MERCHANTNAME,
                //        MERCHANTLOCATION = d.MERCHANTLOCATION,
                //        TERMINALID = d.TERMINALID,
                //        TRANAMOUNT = d.TRANAMOUNT,
                //        AMOUNTCHARGED = d.AMOUNTCHARGED,
                //        SETTLEMENTAMOUNT = d.SETTLEMENTAMOUNT,
                //        MSCRATE = d.MSCRATE,
                //        BATCHTYPE = d.BATCHTYPE,
                //        BATCHID = batchId,
                //        CREATEDATE = curDate,
                //        STATUS = open,
                //        USERID = User.Identity.Name,

                //    };
                //    repoSetReconDualUploadTemp.Insert(obj);
                //    //cnt++;
                }


                var rst = uow.Save(User.Identity.Name);
                if (rst > 0)
                {

                    //SessionHelper.GetCart(Session).Clear();
                    rv.PurgeSetReconDualUpload(User.Identity.Name);

                    try
                    {
                        EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, string.Format("Setttlement Reconciliation Upload Batch #{0}", batchId));
                    }
                    catch
                    {

                    }
                    //txscope.Complete();
                }
                else
                {
                    return Json(new { RespCode = 1, RespMessage = "Problem Processing Request." });
                }
                //}
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });

            }
            var msg = string.Format("<i class='fa fa-check' ></i> Record with Batch-ID #{0} Processed SuccessFully and has been forwarded for authorization", batchId);
            return Json(new { RespCode = 0, RespMessage = msg });

        }
        private static SetReconDualUpldObj addRecord(IList<string> rowData, IList<string> columnNames)
        {
            try
            {
                var obj = new SetReconDualUpldObj()
                {
                    REFERENCENO = rowData[0].Trim(),
                    CARDTYPE = rowData[1].Trim(),
                    TRANSACTIONTYPE = rowData[2].Trim(),
                    TRANSACTIONDATETIME = rowData[3].ToDateTime(),
                    SETTLEMENTDATE = rowData[4].ToDateTime(),
                    MASKEDPAN = rowData[5].Trim(),
                    MERCHANTID = rowData[6].Trim(),
                    MERCHANTACCOUNT = rowData[7].Trim(),
                    MERCHANTNAME = rowData[8].Trim(),
                    MERCHANTLOCATION = rowData[9].Trim(),
                    TERMINALID = rowData[10].Trim(),
                    TRANAMOUNT = rowData[11].ToDecimal(),
                    AMOUNTCHARGED = rowData[12].ToDecimal(),
                    SETTLEMENTAMOUNT = rowData[13].ToDecimal(),
                    MSCRATE = rowData[14].ToDecimal(),
                  // BATCHTYPE = rowData[15].Trim(),
                    VALIDATIONERRORSTATUS = true,
                };
                return obj;
            }
            catch (Exception ex)
            {
                return new SetReconDualUpldObj();
            }
        }



        decimal authId;
        string respMsg = null;
        

        private bool CloseMainRecord(string bATCHID, string uSERID)
        {
            var rec = repoSetReconDualUploadTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoSetReconDualUpload.Find(rec.RECORDID);
                if (obj != null)
                {
                    obj.STATUS = close;
                    return true;
                }
            }
            return false;
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadSetRecon(string id)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("Settlement Reconciliation for {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
                var rv = new SetReconDualUpldSession();
                var data = rv.GetSetReconDual(User.Identity.Name);
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
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Settlement Reconciliation");

                ws.Cells["A1"].Value = "REFERENCE NO.";
                ws.Cells["B1"].Value = "CARD TYPE";
                ws.Cells["C1"].Value = "TRANSACTION TYPE";
                ws.Cells["D1"].Value = "TRANSACTION DATETIME ";
                ws.Cells["E1"].Value = "SETTLEMENT DATETIME.";
                ws.Cells["F1"].Value = "MASKED PAN";
                ws.Cells["G1"].Value = "MERCHANT ID";
                ws.Cells["H1"].Value = "MERCHANT ACCOUNT ";
                ws.Cells["I1"].Value = "MERCHANT NAME";
                ws.Cells["J1"].Value = "MERCHANT LOCATION";
                ws.Cells["K1"].Value = "TERMINAL ID";
                ws.Cells["L1"].Value = "TRANSACTION AMOUNT ";
                ws.Cells["M1"].Value = "AMOUNT CHARGED";
                ws.Cells["N1"].Value = "SETTLEMENT AMOUNT";
                ws.Cells["O1"].Value = "MSC RATE";
                ws.Cells["P1"].Value = "BATCH TYPE";
                ws.Cells["Q1"].Value = "RECONCILIATION STATUS";
                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A2"].LoadFromDataTable(tbl, false);

                return pck.GetAsByteArray();

            }
        }

    }
}