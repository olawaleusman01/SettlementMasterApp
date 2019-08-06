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
    public class SetReconciliationController : Controller
    {
        int institutionId;
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null; private readonly IRepository<SM_CURRENCY> repoCur = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_INSTITUTION> repoInst = null;
        private readonly IRepository<AspNetUser> repoUsers = null;
        private readonly IRepository<SM_SETRECONCILIATION> repoSetReconciliation = null;
        private readonly IRepository<SM_SETRECONCILIATION_TEMP> repoSetReconciliationTemp = null;
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
        public SetReconciliationController()
        {
            uow = new UnitOfWork();

            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoUsers = new Repository<AspNetUser>(uow);
            repoCur = new Repository<SM_CURRENCY>(uow);
            repoSetReconciliation = new Repository<SM_SETRECONCILIATION>(uow);
            repoSetReconciliationTemp = new Repository<SM_SETRECONCILIATION_TEMP>(uow);
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
                var rv = new SetReconUpldSession();
                var rec = rv.ReconcileBatch(User.Identity.Name);
                var html = PartialView("_SetReconUpld", rec).RenderToString();
                return Json(new { data_html = html, RespCode = 0, RespMessage = "Record ", SucCount = 0, FailCount = 0 });
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
            IList<SetReconUpldObj> model = null;
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
                        var rv = new SetReconUpldSession();
                        var cnt = rv.PostSetReconUploadBulk(dataList.ToList(), User.Identity.Name);

                        if (cnt > 0)
                        {
                            var rst = rv.GetSetReconUpload(User.Identity.Name);
                            var html = PartialView("_SetReconUpld", rst).RenderToString();
                            return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        }
                        else
                        {
                            var html = PartialView("_SetReconUpld").RenderToString();
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
                var rv = new SetReconUpldSession();
                var rec = rv.GetSetReconUpload(User.Identity.Name);
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

                    var obj = new SM_SETRECONCILIATION_TEMP()// SM_MERCHANTTERMINALUPLD()
                    {
                        PAYREFNO = d.PAYREFNO,
                        AMOUNT = d.AMOUNT,
                        PAYMENTDATE = d.PAYMENTDATE,
                        VALUEDATE = d.VALUEDATE,
                        //RECEIPTNO = d.RECEIPTNO,
                        CUSTOMERNAME = d.CUSTOMERNAME,
                        PAYMENTMETHOD = d.PAYMENTMETHOD,
                        //TRANSACTIONSTATUS = d.TRANSACTIONSTATUS,
                        //DEPOSITSLIPNO = d.DEPOSITSLIPNO,
                        BANKNAME = d.BANKNAME,
                        //BRANCHNAME = d.BRANCHNAME,
                        //PAYERID = d.PAYERID,
                        //VALUEGRANTED = d.VALUEGRANTED,
                        //RECONCILE = d.RECONCILE,
                        BATCHID = batchId,
                        CREATEDATE = curDate,
                        STATUS = open,
                        USERID = User.Identity.Name,

                    };
                    repoSetReconciliationTemp.Insert(obj);
                    //cnt++;
                }


                var rst = uow.Save(User.Identity.Name);
                if (rst > 0)
                {

                    //SessionHelper.GetCart(Session).Clear();
                    rv.PurgeSetReconUpload(User.Identity.Name);

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
        private static SetReconUpldObj addRecord(IList<string> rowData, IList<string> columnNames)
        {
            try
            {
                var obj = new SetReconUpldObj()
                {
                    PAYREFNO = rowData[0].Trim(),
                    AMOUNT = rowData[1].ToDecimal(),
                    PAYMENTDATE = rowData[2].ToDateTime(),
                    VALUEDATE = rowData[3].ToDateTime(),
                    //RECEIPTNO = rowData[4].Trim(),
                    CUSTOMERNAME = rowData[4].Trim(),
                    PAYMENTMETHOD = rowData[5].Trim(),
                    //TRANSACTIONSTATUS = rowData[7].Trim(),
                    //DEPOSITSLIPNO = rowData[8].Trim(),
                    BANKNAME = rowData[6].Trim(),
                    //BRANCHNAME = rowData[10].Trim(),
                    //PAYERID = rowData[11].Trim(),
                    //VALUEGRANTED = rowData[12].Trim(),
                    //RECONCILE = rowData[13].ToInt32(),
                    VALIDATIONERRORSTATUS = true,
                };
                return obj;
            }
            catch (Exception ex)
            {
                return new SetReconUpldObj();
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
        //public ActionResult DetailAuth(string a_i, string m)
        //{
        //    try
        //    {
        //        int menuId;
        //        var mid = SmartObj.Decrypt(m);
        //        var ai = SmartObj.Decrypt(a_i);
        //        if (int.TryParse(mid, out menuId) && decimal.TryParse(ai, out authId))
        //        {
        //            var obj = new AuthViewObj();
        //            var det = repoAuth.Find(authId);
        //            //var d = _repo.GetSession(0, true);


        //            //ViewBag.StatusVisible = true;
        //            if (det != null)
        //            {
        //                //var bid = det.BATCHID;
        //                //var splt = bid.Split('_');
        //                //var frmType = splt[0];
        //                obj.AuthId = authId;
        //                obj.RecordId = det.RECORDID.GetValueOrDefault();
        //                obj.BatchId = det.BATCHID;
        //                obj.PostType = det.POSTTYPE;
        //                obj.MenuId = det.MENUID.GetValueOrDefault();
        //                ViewBag.Message = TempData["msg"];
        //                var status = TempData["status"];
        //                var stat = status == null ? "open" : status.ToString();

        //                var viewtoDisplay = "";
        //                switch (det.POSTTYPE)
        //                {
        //                    case Single:
        //                        {
        //                            ViewBag.HeaderTitle = "Authorize Detail Settlement Reconciliation";
        //                            viewtoDisplay = "DetailAuth";
        //                            var rec = _repo.GetBinTemp(det.USERID, det.BATCHID);
        //                            if (rec != null && rec.Count > 0)
        //                            {
        //                                var model = rec.FirstOrDefault();
        //                                obj.Status = det.STATUS;
        //                                obj.EventType = det.EVENTTYPE;
        //                                obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
        //                                obj.User = model.CREATED_BY;
        //                                BindCombo2();
        //                                //ViewBag.Institution = model.INSTITUTION_NAME;
        //                                ViewBag.Auth = obj;
        //                                ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

        //                                return View(viewtoDisplay, model);
        //                            }
        //                            break;
        //                        }
        //                    case Batch:
        //                        {
        //                            ViewBag.HeaderTitle = "Authorize Detail for Settlement Reconciliation Upload";
        //                            viewtoDisplay = "DetailAuthBulk";
        //                            var rec = _repo.GetBinTemp(det.USERID, det.BATCHID);
        //                            if (rec != null && rec.Count > 0)
        //                            {
        //                                var model = rec.FirstOrDefault();
        //                                obj.Status = det.STATUS;
        //                                obj.EventType = det.EVENTTYPE;
        //                                obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
        //                                obj.User = det.CREATEDATE.GetValueOrDefault().ToString("dd-MM-yyyy");
        //                                ViewBag.Auth = obj;
        //                                ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

        //                                return View(viewtoDisplay, rec);
        //                            }
        //                            break;
        //                        }
        //                    default:
        //                        {
        //                            viewtoDisplay = "DetailAuth";
        //                            break;
        //                        }
        //                }

        //                //  return Json(rec, JsonRequestBehavior.AllowGet);
        //                //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
        //                // return Json(obj1, JsonRequestBehavior.AllowGet);
        //            }


        //            return View("DetailAuth");
        //        }
        //        else
        //        {
        //            //bad request
        //            return View("Error", "Home");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return View("DetailAuth");
        //    }
        //}
        //int checkerNo = 1;

        //[HttpPost]
        // [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(decimal AuthId, int? m)
        //{
        //    try
        //    {
        //        var rec2 = repoAuth.Find(AuthId);
        //        if (rec2 == null)
        //        {
        //            respMsg = "Problem processing request. Try again or contact Administrator.";
        //            TempData["msg"] = respMsg;
        //            return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //            //return Json(new { RespCode = 1, RespMessage = respMsg });
        //        }
        //        else if (rec2.STATUS.ToLower() != "open")
        //        {
        //            respMsg = "This request has already been processed by an authorizer.";
        //            TempData["msg"] = respMsg;
        //            return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        //            //return Json(new { RespCode = 1, RespMessage = respMsg });
        //        }
        //        //int recordId = 0; int? inst_id = 0;
        //        //string bid = "";
        //        bool suc = false;
        //        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(2, 0, 0)))
        //        //{
        //        var d = new AuthListUtil();
        //        //menuId = 5;
        //        var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
        //        if (dd.authListObj.Count < checkerNo)
        //        {

        //            var chk = new SM_AUTHCHECKER()
        //            {
        //                AUTHLIST_ITBID = AuthId,
        //                CREATEDATE = DateTime.Now,
        //                NARRATION = null,
        //                STATUS = approve,
        //                USERID = User.Identity.Name,
        //            };
        //            repoAuthChecker.Insert(chk);
        //            var rst = uow.Save(User.Identity.Name);
        //            if (rst > 0)
        //            {
        //                var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
        //                noA += 1;
        //                if (noA == checkerNo)
        //                {
        //                    menuId = rec2.MENUID.GetValueOrDefault();
        //                    switch (rec2.EVENTTYPE)
        //                    {
        //                        case "New":
        //                            {

        //                                if (rec2.POSTTYPE == Batch)
        //                                {
        //                                    suc = PostBulkUpload(rec2.BATCHID, rec2.USERID);
        //                                }
        //                                else
        //                                {
        //                                    suc = CreateMainRecord(rec2.BATCHID, rec2.USERID);

        //                                }

        //                                break;
        //                            }
        //                        case "Modify":
        //                            {
        //                                suc = ModifyMainRecord(rec2.BATCHID, rec2.USERID);
        //                                break;
        //                            }
        //                        case "CLOSE":
        //                            {
        //                                suc = CloseMainRecord(rec2.BATCHID, rec2.USERID);
        //                                break;
        //                            }
        //                        default:
        //                            {
        //                                break;
        //                            }
        //                    }
        //                    // rec2.STATUS = close;

        //                    if (suc)
        //                    {
        //                        rec2.STATUS = approve;
        //                        var t = uow.Save(rec2.USERID, User.Identity.Name);
        //                        if (t > 0)
        //                        {
        //                            sucTrue = true;
        //                            // txscope.Complete();
        //                            //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
        //                            //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });

        //                            //return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        //                        respMsg = "Problem processing request. Try again or contact Administrator.";
        //                        //TempData["msg"] = respMsg;
        //                        //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //                        return Json(new { RespCode = 1, RespMessage = respMsg });
        //                    }
        //                }
        //            }
        //        }
        //        // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
        //        respMsg = "This request has already been processed by an authorizer.";
        //        //TempData["msg"] = respMsg;
        //        //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        //        //}
        //        if (sucTrue)
        //        {
        //            EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Settlement Reconciliation Approval", null, fullName);
        //            respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
        //            return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
        //        }
        //        return Json(new { RespCode = 1, RespMessage = respMsg });

        //    }
        //    catch (Exception ex)
        //    {
        //        //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        //        respMsg = "Problem processing request. Try again or contact Administrator.";
        //        //TempData["msg"] = respMsg;
        //        //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        //        return Json(new { RespCode = 1, RespMessage = respMsg });
        //    }
        //}

        private bool CloseMainRecord(string bATCHID, string uSERID)
        {
            var rec = repoSetReconciliationTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoSetReconciliation.Find(rec.RECORDID);
                if (obj != null)
                {
                    obj.STATUS = close;
                    return true;
                }
            }
            return false;
        }

        //private bool CreateMainRecord(string bATCHID, string uSERID)
        //{
        //    var rec = repoBinTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
        //    if (rec != null)
        //    {
        //        var obj = _repo.ValidateBin(rec.CBNCODE, rec.CARDSCHEME, rec.BIN); // repoBin.AllEager(d => d.CBNCODE != null && d.CBNCODE == rec.CBNCODE && d.CARDSCHEME != null && d.CARDSCHEME == rec.CARDSCHEME && d.BIN != null && d.BIN == rec.BIN).FirstOrDefault();
        //        if (obj == 0)
        //        {
        //            var bb = new SM_DATABIN()
        //            {
        //                BANKFIID = rec.BANKFIID,
        //                BIN = rec.BIN,
        //                BUSINESSTYPE = rec.BUSINESSTYPE,
        //                CARDSCHEME = rec.CARDSCHEME,
        //                CBNCODE = rec.CBNCODE,
        //                COUNTRYCODE = rec.COUNTRYCODE,
        //                CREATEDATE = DateTime.Now,
        //                CURRENCYCODE = rec.CURRENCYCODE,
        //                ISSUERFIID = rec.ISSUERFIID,
        //                STATUS = active,
        //                USERID = rec.USERID
        //            };
        //            repoBin.Insert(bb);

        //        }
        //        return true;
        //    }
        //    return false;
        //}

        //private bool ModifyMainRecord(string bATCHID, string uSERID)
        //{
        //    var rec = repoBinTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
        //    if (rec != null)
        //    {
        //        var ext = repoBin.Find(rec.RECORDID);
        //        if (ext != null)
        //        {
        //            var obj = _repo.ValidateBin(rec.CBNCODE, rec.CARDSCHEME, rec.BIN); // repoBin.AllEager(d => d.CBNCODE != null && d.CBNCODE == rec.CBNCODE && d.CARDSCHEME != null && d.CARDSCHEME == rec.CARDSCHEME && d.BIN != null && d.BIN == rec.BIN).FirstOrDefault();
        //            if (obj == 0)
        //            {
        //                ext.BIN = rec.BIN;
        //                ext.CARDSCHEME = rec.CARDSCHEME;
        //                ext.CBNCODE = rec.CBNCODE;

        //            }
        //        }
        //        return true;
        //    }
        //    return false;
        //}


        //bool sucTrue;
        //[HttpPost]
        //// [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Reject(decimal AuthId, int? m, string Narration)
        //{
        //    //int menuId = 0;
        //    //string msg = "";
        //    try
        //    {
        //        var rec2 = repoAuth.Find(AuthId);
        //        if (rec2 == null)
        //        {
        //            respMsg = "Problem processing request. Try again or contact Administrator.";
        //            //TempData["msg"] = respMsg;
        //            //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //            return Json(new { RespCode = 1, RespMessage = respMsg });

        //        }
        //        else if (rec2.STATUS.ToLower() != "open")
        //        {
        //            respMsg = "This request has already been processed by an authorizer.";
        //            //TempData["msg"] = respMsg;
        //            //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        //            return Json(new { RespCode = 1, RespMessage = respMsg });
        //        }
        //        int recordId = 0;
        //        // bool suc = false;
        //        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //        //{
        //        var d = new AuthListUtil();
        //        //menuId = 5;
        //        var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
        //        if (dd.authListObj.Count < checkerNo)
        //        {

        //            var chk = new SM_AUTHCHECKER()
        //            {
        //                AUTHLIST_ITBID = AuthId,
        //                CREATEDATE = DateTime.Now,
        //                NARRATION = Narration,
        //                STATUS = reject,
        //                USERID = User.Identity.Name,
        //            };
        //            repoAuthChecker.Insert(chk);
        //            var rst = uow.Save(User.Identity.Name);
        //            if (rst > 0)
        //            {
        //                var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
        //                noA += 1;
        //                if (noA == checkerNo)
        //                {
        //                    // recordId = (int)rec2.RECORDID;
        //                    menuId = rec2.MENUID.GetValueOrDefault();
        //                    if (rec2.POSTTYPE == Single)
        //                    {
        //                        var recc = repoSetReconciliationTemp.AllEager(f => f.BATCHID == rec2.BATCHID && f.USERID == rec2.USERID).FirstOrDefault();
        //                        if (recc != null)
        //                        {
        //                            recc.STATUS = reject;
        //                        }
        //                    }
        //                    else if (rec2.POSTTYPE == Batch)
        //                    {
        //                        var recc = repoSetReconciliationTemp.AllEager(f => f.BATCHID == rec2.BATCHID && f.USERID == rec2.USERID).ToList();
        //                        foreach (var p in recc)
        //                        {
        //                            p.STATUS = reject;
        //                        }
        //                    }

        //                    rec2.STATUS = reject;
        //                    var t = uow.Save(User.Identity.Name);
        //                    if (t > 0)
        //                    {
        //                        //txscope.Complete();
        //                        //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
        //                        //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
        //                        respMsg = "Record Rejected. A mail has been sent to the user.";
        //                        // TempData["msg"] = respMsg;
        //                        //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //                        return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
        //                    }

        //                }
        //            }
        //        }

        //        //}
        //        if (sucTrue)
        //        {
        //            EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Issuer Bin Approval", null, fullName);

        //            respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
        //            TempData["msg"] = respMsg;
        //            TempData["status"] = approve;
        //            return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //        }
        //        return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });

        //        //return Json(new { RespCode = 1, RespMessage = respMsg });


        //    }
        //    catch (Exception ex)
        //    {
        //        //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        //        respMsg = "Problem processing request. Try again or contact Administrator.";
        //        TempData["msg"] = respMsg;
        //        return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadSetRecon(string id)
        {
            try
            {
                //Call to get Excel byte array.
                string fileName = string.Format("System Settlement Reconciliation for {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
                var rv = new SetReconUpldSession();
                var data = rv.GetSetRecon(User.Identity.Name);
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

                ws.Cells["A1"].Value = "PAYMENT REFERENCE NO.";
                ws.Cells["B1"].Value = "AMOUNT";
                ws.Cells["C1"].Value = "PAYMENT DATE";
                ws.Cells["D1"].Value = "VALUE DATE ";
                //ws.Cells["E1"].Value = "RECEIPT NO.";
                ws.Cells["E1"].Value = "CUSTOMER NAME";
                ws.Cells["F1"].Value = "PAYMENT METHOD";
                //ws.Cells["H1"].Value = "TRANSACTION STATUS ";
                //ws.Cells["I1"].Value = "DEPOSIT SLIP NO";
                ws.Cells["G1"].Value = "BANK NAME";
                //ws.Cells["K1"].Value = "BRANCH NAME";
                //ws.Cells["L1"].Value = "PAYER ID ";
                //ws.Cells["M1"].Value = "VALUE GARANTED";
                ws.Cells["H1"].Value = "RECONCILIATION STATUS";
                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A2"].LoadFromDataTable(tbl, false);

                return pck.GetAsByteArray();

            }
        }

    }
}