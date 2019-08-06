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
namespace SettlementMaster.App.Controllers
{
    public class ExchangeRateController : Controller
    {
            int institutionId;
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;private readonly IRepository<SM_CURRENCY> repoCur = null;
            private readonly IRepository<SM_AUTHLIST> repoAuth = null;
            private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
            private readonly IRepository<SM_INSTITUTION> repoInst = null;
            private readonly IRepository<AspNetUser> repoUsers = null;
            private readonly IRepository<SM_EXCHANGERATE> repoRate = null;
            private readonly IRepository<SM_EXCHANGERATETEMP> repoRateTemp = null;
            private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
        const string Single = "SINGLE";
            const string Batch = "BATCH";
            int roleId;
            int menuId = 44;
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
        public ExchangeRateController()
        {
            uow = new UnitOfWork();
          
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoUsers = new Repository<AspNetUser>(uow);
            repoCur = new Repository<SM_CURRENCY>(uow);
            repoRate = new Repository<SM_EXCHANGERATE>(uow);
            repoRateTemp = new Repository<SM_EXCHANGERATETEMP>(uow);
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
        [MyAuthorize]
        public ActionResult Index()
        {
            BindCombo();
            return View();
        }
        [MyAuthorize]
        public ActionResult BinUpload()
        {
            BindCombo();
            return View();
        }
        public async Task<ActionResult> ExchangeRateList(string id)
        {
            try
            {
                if (id == "-1")
                {
                    id = null;
                }
                var rec = await _repo.GetExchangeRateAsync(0, true,id);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<ExchangeRateObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> Add(int id = 0)
        {
            try
            {
                ViewBag.MenuId = menuId; // HttpUtility.UrlDecode(m);
             
                if (id == 0)
                {
                    BindCombo2();
                    ViewBag.HeaderTitle = "Add Exchange Rate";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    return View("Add", new ExchangeRateObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Exchange Rate";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetExchangeRateAsync(id, false);
                    if (rec == null)
                    {
                     
                        //bad request
                        // return Json(null, JsonRequestBehavior.AllowGet);
                        //var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                        // return Json(obj, JsonRequestBehavior.AllowGet);
                        ViewBag.Message = "Record Not Found";
                        return View("Add");
                    }
                    var model = rec.FirstOrDefault();
                    BindCombo2(model.CURRENCY_CODE);
                    return View("Add", model);
                }
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                ViewBag.Message = ex.Message;
                return View("Add");
            }
        }

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ExchangeRateObj model, string m)
        {
            try
            {
                if (model.ITBID == 0)
                {
                    ViewBag.ButtonText = "Save";
                    ViewBag.HeaderTitle = "Add Exchange Rate";
                }
                else
                {
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Exchange Rate";
                }

                //ViewBag.MenuId =  m;
                //menuId = SmartUtil.GetMenuId(m);
                string bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID == 0)
                    {
                        var errMsg = "";
                        if (validateForm(model, eventInsert, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo2();
                            //ViewBag.PartyAcct = GetPartyAcctLines();
                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_EXCHANGERATETEMP BType = new SM_EXCHANGERATETEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                CARDSCHEME = model.CARDSCHEME,
                                CBN_CODE = "000", // model.CBN_CODE,
                                CURRENCY_CODE = model.CURRENCY_CODE,
                                NAIRA_EQUIVALENT = model.NAIRA_EQUIVALENT,
                                RATE = 1,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = 0, // model.RECORDID,
                            };

                            repoRateTemp.Insert(BType);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                               
                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = eventInsert,
                                    MENUID = menuId,
                                    RECORDID = BType.ITBID,
                                    STATUS = open,
                                    URL = Request.FilePath,
                                    USERID = User.Identity.Name,
                                    POSTTYPE = Single,
                                    BATCHID = bid,
                                    INSTITUTION_ITBID = institutionId
                                    
                                };
                                repoAuth.Insert(auth);
                                var rst1 = uow.Save(User.Identity.Name);
                                if (rst1 > 0)
                                {
                                    SessionHelper.GetPartyAcct(Session).Clear();
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Exchange Rate Record");

                                    //txscope.Complete();
                                    TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", new { m = m });
                                }
                            }
                        }
                    }
                    else
                    {
                        //var errMsg = "";
                        //if (validateForm(model,eventEdit, out errMsg))
                        //{
                        //    ViewBag.Message = errMsg; // "Carscheme already Exist.";
                        //    BindCombo2();
                        //    return View("Add", model);
                        //};
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_EXCHANGERATETEMP BType = new SM_EXCHANGERATETEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                CARDSCHEME = model.CARDSCHEME,
                                CBN_CODE = "000", // model.CBN_CODE,
                                CURRENCY_CODE = model.CURRENCY_CODE,
                                NAIRA_EQUIVALENT = model.NAIRA_EQUIVALENT,
                                RATE = 1,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                            };
                            repoRateTemp.Insert(BType);
                            //  var rst1 = new AuthListUtil().SaveLog(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                            if (rst)
                            {
                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = model.STATUS == active ? eventEdit : model.STATUS,
                                    MENUID = menuId,
                                    //MENUNAME = "",
                                    RECORDID = BType.ITBID,
                                    STATUS = open,
                                    // TABLENAME = "ADMIN_DEPARTMENT",
                                    URL = Request.FilePath,
                                    USERID = User.Identity.Name,
                                    POSTTYPE = Single,
                                    INSTITUTION_ITBID = institutionId,
                                    BATCHID = bid,

                                };

                                repoAuth.Insert(auth);
                                if (uow.Save(User.Identity.Name) > 0)
                                {
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Exchange Rate Record");
                                    //txscope.Complete();
                                    TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", new { m = m });
                                    //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                                    // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                                }
                            }
                        }

                    //}
                    // If we got this far, something failed, redisplay form
                    //return Json(new { RespCode = 1, RespMessage = errorMsg });
                    BindCombo2();
                    ViewBag.Message = errorMsg;
                    return View("Add", model);
                //}
            }
            catch (SqlException ex)
            {
                BindCombo2();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                BindCombo2();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            BindCombo();
            ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            return View("Add", model);
            //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }

        private bool validateForm(ExchangeRateObj obj,string eventType, out string errorMsg)
        {
            var sb = new StringBuilder();
            var errCount = 0;
            //if (eventType == eventInsert)
            //{
                var existCbnCode = _repo.ValidateExchRate(obj.CBN_CODE, obj.CARDSCHEME, obj.CURRENCY_CODE,2); // repoBin.AllEager(f => f.CBNCODE != null && f.CBNCODE == obj.CBNCODE && f.CARDSCHEME != null && f.CARDSCHEME == obj.CARDSCHEME && f.).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"Exchange Rate is already setup for the selected currency");
                    errCount++;
                }
                if (errCount > 0)
                {
                    errorMsg = sb.ToString();
                    return true;
                }
            //}
            //else
            //{
            //    var existCbnCode = _repo.ValidateBin(obj.CBN_CODE, obj.CARDSCHEME, obj.CURRENCY_CODE,2,obj.ITBID); // repoBin.AllEager(f => f.CBNCODE != null && f.CBNCODE == obj.CBNCODE && f.CARDSCHEME != null && f.CARDSCHEME == obj.CARDSCHEME && f.).Count();
            //    if (existCbnCode > 0)
            //    {
            //        sb.AppendLine(@"""ISSUER BIN"" already exist for selected Institution and Card Scheme");
            //        errCount++;
            //    }
            //    if (errCount > 0)
            //    {
            //        errorMsg = sb.ToString();
            //        return true;
            //    }
            //}
            errorMsg = sb.ToString();
            return false;
        }

        protected int ValidateUpload()
            {
                var rv = new DataBinUpldSession();
                var rec = rv.GetDataBinUpload(User.Identity.Name);
               
                int totalErrorCount = 0;
                foreach (var t in rec)
                {
                    int errorCount = 0;
                    var validationErrorMessage = new List<string>();
                
                var existBank = repoInst.AllEager(d => d.CBN_CODE == t.CBNCODE).Count();
                if(existBank == 0)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""CBN CODE"" does not exist.");
                }
                var existScheme = repoScheme.AllEager(d => d.CARDSCHEME == t.CARDSCHEME).Count();
                if (existScheme == 0)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""CARD SCHEME"" does not exist.");
                }
                if (errorCount == 0)
                {
                    var cnt = _repo.ValidateBin(t.CBNCODE, t.CARDSCHEME, t.BIN);
                    if (cnt > 0)
                    {
                        errorCount++;
                        validationErrorMessage.Add("Issuer Bin Already exist.");
                    }
                }
                   
                    if (errorCount == 0)
                    {
                        t.VALIDATIONERRORSTATUS = false;
                        t.VALIDATIONERRORMESSAGE = "";
                    }
                    else
                    {
                        totalErrorCount++;
                        t.VALIDATIONERRORSTATUS = true;
                        t.VALIDATIONERRORMESSAGE = GetStringFromList(validationErrorMessage);
                    }
                    var rst = rv.PostBinUpload(t, 2, User.Identity.Name);
                    //SessionHelper.GetCart(Session).UpdateItem(t);
                }
                if (rec.Count > 0)
                {
                    if (totalErrorCount > 0)
                    {

                        //pnlResponse.Visible = true;
                        //pnlResponse.CssClass = "alert alert-danger alert-dismissable alert-bold";
                        //pnlResponseMsg.Text = string.Format("{0} Record(s) Failed Validation from Batch...", totalErrorCount);
                        //if (totalErrorCount == rec.Count)
                        //{
                        //    btnProcess.Enabled = false;
                        //}
                        //else
                        //{
                        //    btnProcess.Enabled = true;
                        //}

                    }
                    else
                    {
                        //pnlResponse.Visible = true;
                        //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
                        //pnlResponseMsg.Text = "Batch Validated Successfully...You can now save for further processing";
                        // btnProcess.Enabled = false;
                        //btnProcess.Enabled = true;
                    }
                }
                return totalErrorCount;
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

        protected bool PostBulkUpload(string batchid, string user_id)
        {
            try
            {
                DateTime curDate = DateTime.Now;
                var rec = repoRateTemp.AllEager(d => d.BATCHID == batchid && d.USERID == user_id).ToList();
                foreach (var t in rec)
                {
                    t.STATUS = approve;
                    //var cnt = _repo.ValidateBin(t.CBNCODE, t.CARDSCHEME, t.BIN);
                    //if (cnt == 0)
                    //{
                    //    var bb = new SM_DATABIN()
                    //    {
                    //        BANKFIID = t.BANKFIID,
                    //        BIN = t.BIN,
                    //        BUSINESSTYPE = t.BUSINESSTYPE,
                    //        CARDSCHEME = t.CARDSCHEME,
                    //        CBNCODE = t.CBNCODE,
                    //        COUNTRYCODE = t.COUNTRYCODE,
                    //        CREATEDATE = DateTime.Now,
                    //        CURRENCYCODE = t.CURRENCYCODE,
                    //        ISSUERFIID = t.ISSUERFIID,
                    //        STATUS = active,
                    //        USERID = t.USERID
                    //    };
                    //    repoBin.Insert(bb);
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
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
                var rec = _repo.GetInstitution(0, true, "Active").Where(d=> d.CBN_CODE != null).ToList(); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
                ViewBag.BankList = new SelectList(rec, "CBN_CODE", "INSTITUTION_NAME");

            }
        void BindCombo2(string selected = null)
        {
            //var rec = _repo.GetInstitution(0, true, "Active").Where(d => d.CBN_CODE != null).ToList(); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
            //ViewBag.BankList = new SelectList(rec, "CBN_CODE", "INSTITUTION_NAME");
            //var scheme = _repo.GetCardScheme(0, true, "Active"); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
            //ViewBag.SchemeList = new SelectList(scheme, "CARDSCHEME", "CARDSCHEME_DESC");
            var scheme = _repo.GetCurrency(0, true, "Active"); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
            if (selected != null)
            {
                ViewBag.CurrencyList = new SelectList(scheme, "CURRENCY_CODE", "CURRENCY_NAME",selected);
            }
            else
            {
                ViewBag.CurrencyList = new SelectList(scheme, "CURRENCY_CODE", "CURRENCY_NAME");
            }
            ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus(), "Code", "Description");
        }
        //[HttpPost]
            // [ValidateAntiForgeryToken]
            public ActionResult Validate()
            {
                try
                {

                    var rv = new DataBinUpldSession();
                    var errCount = ValidateUpload();
                    var rec = rv.GetDataBinUpload(User.Identity.Name);
                    var sucCount = rec.Count - errCount;
                    var html = PartialView("_BinUpld", rec).RenderToString();
                    return Json(new { data_html = html, RespCode = 0, RespMessage = "Record ", SucCount = sucCount, FailCount = errCount });
                }
                catch (Exception ex)
                {
                    return Json(new { RespCode = 1, RespMessage = "Problem Processing Validation. " });
                }
            }
            [HttpPost]
            // [ValidateAntiForgeryToken]
            public ActionResult UploadFiles()
            {
                IList<DataBinUpldObj> model = null;
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
                            var rv = new DataBinUpldSession();
                            var cnt = rv.PostBinUploadBulk(dataList.ToList(), User.Identity.Name);

                            if (cnt > 0)
                            {
                                var rst = rv.GetDataBinUpload(User.Identity.Name);
                                var html = PartialView("_BinUpld", rst).RenderToString();
                                return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                            }
                            else
                            {
                                var html = PartialView("_BinUpld").RenderToString();
                                return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                            }
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
                var rv = new DataBinUpldSession();
                var rec = rv.GetDataBinUpload(User.Identity.Name);
                //  isUp = userInstitutionItbid == 1 ? true : false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 30, 0)))
                //{
                    int errorcnt = 0;

                    DateTime curDate = DateTime.Now;

                    batchId =  SmartObj.GenRefNo2();

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

                        var obj = new SM_DATABIN_TEMP()// SM_MERCHANTTERMINALUPLD()
                        {
                            CBNCODE = d.CBNCODE,
                            CARDSCHEME = d.CARDSCHEME,
                            BIN = d.BIN,
                            ISSUERFIID = d.ISSUERFIID,
                            BANKFIID = d.BANKFIID,
                            BATCHID = batchId,
                            BUSINESSTYPE = d.BUSINESSTYPE,
                            COUNTRYCODE = d.COUNTRYCODE,
                            CREATEDATE = curDate,
                            CURRENCYCODE = d.CURRENCYCODE,
                            STATUS = open,
                            USERID = User.Identity.Name,

                        };
                        //repoBinTemp.Insert(obj);
                        //cnt++;
                    }


                    var rst = uow.Save(User.Identity.Name);
                    if (rst > 0)
                    {

                        //SessionHelper.GetCart(Session).Clear();
                        rv.PurgeDataBinUpload(User.Identity.Name);

                        try
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, string.Format("Issuer Bin Upload Batch #{0}", batchId));
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
               // }
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });

            }
            var msg = string.Format("<i class='fa fa-check' ></i> Record with Batch-ID #{0} Processed SuccessFully and has been forwarded for authorization", batchId);
            return Json(new { RespCode = 0, RespMessage = msg });

        }
            private static DataBinUpldObj addRecord(IList<string> rowData, IList<string> columnNames)
            {
                try
                {
                    var obj = new DataBinUpldObj()
                    {
                        CBNCODE = rowData[0].Trim(),
                        CARDSCHEME = rowData[1].Trim(),
                        BIN = rowData[2].Trim(),
                        VALIDATIONERRORSTATUS = true,
                    };
                    return obj;
                }
                catch (Exception ex)
                {
                    return new DataBinUpldObj();
                }
            }

            //private static MerchantUpldObj addRecord(IList<string> rowData)
            //{
            //    try
            //    {
            //        var obj = new MerchantUpldObj()
            //        {
            //            MERCHANTID = rowData[0].Trim(),
            //            MERCHANTNAME = rowData[1],
            //            CONTACTTITLE = rowData[2],
            //            CONTACTNAME = rowData[3].Trim(),
            //            MOBILEPHONE = rowData[4].Trim(),
            //            EMAIL = rowData[5].Trim(),
            //            EMAILALERTS = rowData[6].Trim(),
            //            PHYSICALADDR = rowData[7].Trim(),
            //            TERMINALMODELCODE = rowData[8],
            //            TERMINALID = rowData[9].Trim(),
            //            BANKCODE = rowData[10].Trim(),
            //            BANKACCNO = rowData[11].Trim(),
            //            BANKTYPE = rowData[12],
            //            SLIPFOOTER = rowData[13],
            //            SLIPHEADER = rowData[14],
            //            BUISNESSOCCUPATIONCODE = rowData[15],
            //            MERCHANTCATEGORYCODE = rowData[16].Trim(),
            //            STATECODE = rowData[17].Trim(),
            //            VISAACQUIRERID = rowData[18],
            //            VERVEACQUIRERID = rowData[19],
            //            MASTERCARDACQUIRERID = rowData[20],
            //            TERMINALOWNERCODE = rowData[21].Trim(),
            //            LGA_LCDA = rowData[22].Trim(),
            //            BANK_URL = rowData[23],
            //            ACCOUNTNAME = rowData[24].Trim(),
            //            PTSP = rowData[25].Trim(),
            //            TRANSCURRENCY = rowData[26].Trim(),
            //            PTSA = rowData[27].Trim(),
            //           // PAYATTITUDE_ACCEPTANCE = rowData[28].Trim(),
            //            VALIDATIONERRORSTATUS = true,
            //        };
            //        return obj;
            //    }
            //    catch (Exception ex)
            //    {
            //        return new MerchantUpldObj();
            //    }
            //}

            decimal authId;
            string respMsg = null;
            public ActionResult DetailAuth(string a_i, string m)
            {
                try
                {
                    int menuId;
                    var mid = SmartObj.Decrypt(m);
                    var ai = SmartObj.Decrypt(a_i);
                    if (int.TryParse(mid, out menuId) && decimal.TryParse(ai, out authId))
                    {
                        var obj = new AuthViewObj();
                        var det = repoAuth.Find(authId);
                        //var d = _repo.GetSession(0, true);


                        //ViewBag.StatusVisible = true;
                        if (det != null)
                        {
                            //var bid = det.BATCHID;
                            //var splt = bid.Split('_');
                            //var frmType = splt[0];
                            obj.AuthId = authId;
                            obj.RecordId = det.RECORDID.GetValueOrDefault();
                            obj.BatchId = det.BATCHID;
                            obj.PostType = det.POSTTYPE;
                            obj.MenuId = det.MENUID.GetValueOrDefault();
                            ViewBag.Message = TempData["msg"];
                            var status = TempData["status"];
                            var stat = status == null ? "open" : status.ToString();

                            var viewtoDisplay = "";
                            switch (det.POSTTYPE)
                            {
                                case Single:
                                    {
                                        ViewBag.HeaderTitle = "Authorize Detail Issuer Bin";
                                        viewtoDisplay = "DetailAuth";
                                        var rec = _repo.GetExchangeRateTemp(det.USERID, det.BATCHID);
                                        if (rec != null && rec.Count > 0)
                                        {
                                            var model = rec.FirstOrDefault();
                                            obj.Status = det.STATUS;
                                            obj.EventType = det.EVENTTYPE;
                                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                            obj.User = model.CREATED_BY;
                                            BindCombo2();
                                            //ViewBag.Institution = model.INSTITUTION_NAME;
                                            ViewBag.Auth = obj;
                                            ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                            return View(viewtoDisplay, model);
                                        }
                                        break;
                                    }
                                case Batch:
                                    {
                                        ViewBag.HeaderTitle = "Authorize Detail for Issuer Bin Upload";
                                        viewtoDisplay = "DetailAuthBulk";
                                        var rec = _repo.GetExchangeRateTemp(det.USERID, det.BATCHID);
                                        if (rec != null && rec.Count > 0)
                                        {
                                            var model = rec.FirstOrDefault();
                                            obj.Status = det.STATUS;
                                            obj.EventType = det.EVENTTYPE;
                                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                            obj.User = det.CREATEDATE.GetValueOrDefault().ToString("dd-MM-yyyy");
                                            ViewBag.Auth = obj;
                                            ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                            return View(viewtoDisplay, rec);
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        viewtoDisplay = "DetailAuth";
                                        break;
                                    }
                            }

                            //  return Json(rec, JsonRequestBehavior.AllowGet);
                            //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                            // return Json(obj1, JsonRequestBehavior.AllowGet);
                        }


                        return View("DetailAuth");
                    }
                    else
                    {
                        //bad request
                        return View("Error", "Home");
                    }

                }
                catch (Exception ex)
                {
                    return View("DetailAuth");
                }
            }
            int checkerNo = 1;

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(decimal AuthId, int? m)
        {
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    TempData["msg"] = respMsg;
                    return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    //return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    TempData["msg"] = respMsg;
                    return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    //return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                //int recordId = 0; int? inst_id = 0;
                //string bid = "";
                bool suc = false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(2, 0, 0)))
                //{
                    var d = new AuthListUtil();
                    //menuId = 5;
                    var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
                    if (dd.authListObj.Count < checkerNo)
                    {

                        var chk = new SM_AUTHCHECKER()
                        {
                            AUTHLIST_ITBID = AuthId,
                            CREATEDATE = DateTime.Now,
                            NARRATION = null,
                            STATUS = approve,
                            USERID = User.Identity.Name,
                        };
                        repoAuthChecker.Insert(chk);
                        var rst = uow.Save(User.Identity.Name);
                        if (rst > 0)
                        {
                            var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
                            noA += 1;
                            if (noA == checkerNo)
                            {
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                        {

                                            if (rec2.POSTTYPE == Batch)
                                            {
                                                suc = PostBulkUpload(rec2.BATCHID, rec2.USERID);
                                            }
                                            else
                                            {
                                                suc = CreateMainRecord(rec2.BATCHID, rec2.USERID);

                                            }

                                            break;
                                        }
                                    case "Modify":
                                        {
                                            suc = ModifyMainRecord(rec2.BATCHID, rec2.USERID);
                                            break;
                                        }
                                    case "CLOSE":
                                        {
                                            suc = CloseMainRecord(rec2.BATCHID, rec2.USERID);
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                                // rec2.STATUS = close;

                                if (suc)
                                {
                                    rec2.STATUS = approve;
                                    var t = uow.Save(rec2.USERID,User.Identity.Name);
                                    if (t > 0)
                                    {
                                        sucTrue = true;
                                        //txscope.Complete();
                                        //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
                                        //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                                       
                                        //return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                                    }
                                }
                                else
                                {
                                    //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                    respMsg = "Problem processing request. Try again or contact Administrator.";
                                    //TempData["msg"] = respMsg;
                                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                    return Json(new { RespCode = 1, RespMessage = respMsg });
                                }
                            }
                        }
                    }
                    // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                    respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                //}
                if (sucTrue)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Isuer Bin Approval", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                return Json(new { RespCode = 1, RespMessage = respMsg });
                
            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                //TempData["msg"] = respMsg;
                //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool CloseMainRecord(string bATCHID, string uSERID)
        {
            var rec = repoRateTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if(rec != null)
            {
                rec.STATUS = approve;
                var obj = repoRate.Find(rec.RECORDID);
                if(obj != null)
                {
                    obj.STATUS = close;
                    return true;
                }
            }
            return false;
        }

        private bool CreateMainRecord(string bATCHID, string uSERID)
        {
            var rec = repoRateTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if(rec != null)
            {
                //var obj = _repo.ValidateBin(rec.CBN_CODE, rec.CARDSCHEME, rec.BIN); // repoBin.AllEager(d => d.CBNCODE != null && d.CBNCODE == rec.CBNCODE && d.CARDSCHEME != null && d.CARDSCHEME == rec.CARDSCHEME && d.BIN != null && d.BIN == rec.BIN).FirstOrDefault();
                //if(obj == 0)
                //{
                var rst = repoRate.AllEager(d => d.CBN_CODE == rec.CBN_CODE && d.CURRENCY_CODE == rec.CURRENCY_CODE).FirstOrDefault();
                if (rst == null)
                {
                    var bb = new SM_EXCHANGERATE()
                    {
                        CURRENCY_CODE = rec.CURRENCY_CODE,
                        CARDSCHEME = rec.CARDSCHEME,
                        CBN_CODE = rec.CBN_CODE,
                        RATE = rec.RATE,
                        NAIRA_EQUIVALENT = rec.NAIRA_EQUIVALENT,
                        CREATEDATETIME = rec.CREATEDATE,
                        STATUS = active,
                        USERID = rec.USERID
                    };
                    repoRate.Insert(bb);
                }
                else
                {
                    rst.NAIRA_EQUIVALENT = rec.NAIRA_EQUIVALENT;
                    rst.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    rst.LAST_MODIFIED_DATE = DateTime.Now;
                    rst.LAST_MODIFIED_UID = rec.USERID;
                }
                    
                //}
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(string bATCHID, string uSERID)
        {
            var rec = repoRateTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if (rec != null)
            {
                var ext = repoRate.AllEager(d=> d.CURRENCY_CODE == rec.CURRENCY_CODE ).FirstOrDefault();
                if (ext != null)
                {
                   
                        ext.NAIRA_EQUIVALENT = rec.NAIRA_EQUIVALENT;
                        ext.LAST_MODIFIED_AUTHID = User.Identity.Name;
                        ext.LAST_MODIFIED_DATE = DateTime.Now;
                        ext.LAST_MODIFIED_UID = rec.USERID;
                    
                }
                return true;
            }
            return false;
        }

      
            bool sucTrue;
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(decimal AuthId, int? m, string Narration)
        {
            //int menuId = 0;
            //string msg = "";
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });

                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                int recordId = 0;
                // bool suc = false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                //{
                    var d = new AuthListUtil();
                    //menuId = 5;
                    var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
                    if (dd.authListObj.Count < checkerNo)
                    {

                        var chk = new SM_AUTHCHECKER()
                        {
                            AUTHLIST_ITBID = AuthId,
                            CREATEDATE = DateTime.Now,
                            NARRATION = Narration,
                            STATUS = reject,
                            USERID = User.Identity.Name,
                        };
                        repoAuthChecker.Insert(chk);
                        var rst = uow.Save(User.Identity.Name);
                        if (rst > 0)
                        {
                            var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
                            noA += 1;
                            if (noA == checkerNo)
                            {
                               // recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                if (rec2.POSTTYPE == Single)
                                {
                                    var recc = repoRateTemp.AllEager(f=> f.BATCHID == rec2.BATCHID && f.USERID == rec2.USERID).FirstOrDefault();
                                    if (recc != null)
                                    {
                                        recc.STATUS = reject;
                                    }
                                }
                                else if (rec2.POSTTYPE == Batch)
                                {
                                    var recc = repoRateTemp.AllEager(f => f.BATCHID == rec2.BATCHID && f.USERID == rec2.USERID).ToList();
                                    foreach (var p in recc)
                                    {
                                        p.STATUS = reject;
                                    }
                                }

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                   // txscope.Complete();
                                    //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
                                    //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                                    respMsg = "Record Rejected. A mail has been sent to the user.";
                                    // TempData["msg"] = respMsg;
                                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                                }

                            }
                        }
                    }

                //}
                if (sucTrue)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Issuer Bin Approval", null, fullName);

                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    TempData["msg"] = respMsg;
                    TempData["status"] = approve;
                    return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                }
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });

                //return Json(new { RespCode = 1, RespMessage = respMsg });


            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
            }
        }
        
    }
}