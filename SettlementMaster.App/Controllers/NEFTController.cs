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
using System.IO;
using System.Net;
using System.Configuration;
using Newtonsoft.Json;
using System.Xml;
using Generic.Dapper.Utilities;

namespace SettlementMaster.App.Controllers
{
    public class NEFTController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_NAPS_NIBSS> repoNaps = null;
        private readonly IRepository<SM_NAPS_NIBSS_TEMP> repoNapsTemp = null;
        private readonly IRepository<SM_APPROVAL_ROUTE> repoRoute = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;

        string active = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.ACTIVE);
        string inActive = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.INACTIVE);
        string open = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.OPEN);
        string close = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.CLOSED);
        string approve = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.APPROVED);
        string reject = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.REJECTED);
        readonly string eventInsert = "New";
        readonly string eventEdit = "Modify";
        readonly string eventDelete = "Delete";
        const string Single = "SINGLE";
        const string Batch = "BATCH";
        int menuId = 39;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName; string deptCode;
        // GET: Roles
        public NEFTController()
        {
            uow = new UnitOfWork();
            repoNaps = new Repository<SM_NAPS_NIBSS>(uow);
            repoNapsTemp = new Repository<SM_NAPS_NIBSS_TEMP>(uow);
            repoRoute = new Repository<SM_APPROVAL_ROUTE>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
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
        public ActionResult Index()
        {
            try
            {
                BindCombo(1);
            }
            catch
            {

            }

            return View();
        }

        [MyAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateNaps(DateTime SetDate, int Code)
        {
            var html = "";
            var bid = "";
            try
            {
                bid = SmartObj.GenRefNo();

                var rst = Naps.GenerateNaps2(SetDate, Code, bid, "A", User.Identity.Name);
                if (rst != null && rst.RespCode == 0)
                {
                    var rec = Naps.GetNaps(User.Identity.Name, bid);
                    html = PartialView("_ViewNaps", rec).RenderToString();
                    return Json(new { data_html = html, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { RespCode = 1, RespMessage = rst.RespMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Error Processing File" }, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult EditNaps(string id)
        {
            try
            {

                var rec = Naps.FindNaps(id, User.Identity.Name);
                if (rec != null)
                {
                    //BindComboMsc(rec.MID);
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Naps";
                    return PartialView("_AddNaps", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditNaps(NapsObj model)
        {

            string msg = "";

            try
            {

                var obj = new NapsObj()
                {
                    BENEFICIARYACCTNO = model.BENEFICIARYACCTNO,
                    BENEFICIARYBANKCODE = model.BENEFICIARYBANKCODE,
                    BENEFICIARYNAME = model.BENEFICIARYNAME,
                    BENEFICIARYNARRATION = model.BENEFICIARYNARRATION,
                    CREDITAMOUNT = model.CREDITAMOUNT,
                    DEBITACCTNO = model.DEBITACCTNO,
                    DEBITBANKCODE = model.DEBITBANKCODE,
                    USERID = User.Identity.Name,
                    PID = model.PID,
                    EVENTTYPE = eventEdit,
                    CREATEDATE = DateTime.Now,
                    SETTLEMENTDATE = null,
                    REASON = model.REASON
                };

                var rst = Naps.PostNaps(obj, 2);
                //}
                if (rst <= 0)
                {
                    msg = "Problem Processing Request."; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                    return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                }
                //SessionHelper.GetRvHead(Session).UpdateItem(obj);
                // rv.PostRevenueHead(obj, 2);
                var w = Naps.GetNaps(User.Identity.Name, null); // GetRvHeadLines().ToList();
                var html = PartialView("_ViewNaps", w).RenderToString();
                msg = "Record Updated to List";
                return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }
        }
        [MyAuthorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Process(string[] selected, string ReqType, string ApproverId)
        {
            try
            {
                var curDate = DateTime.Now;
                var bid = SmartObj.GenRefNo();
                var cnt = 0;
                var rec = Naps.GetNaps(User.Identity.Name, null);
                foreach (var d in rec)
                {
                    var proc = "";
                    if (selected.Contains(d.PID))
                    {
                        proc = "Y";
                    }
                    else
                    {
                        proc = "N";
                    }
                    var obj = new SM_NAPS_NIBSS_TEMP()
                    {
                        BATCHID = bid,
                        BENEFICIARYACCTNO = d.BENEFICIARYACCTNO,
                        BENEFICIARYBANKCODE = d.BENEFICIARYBANKCODE,
                        BENEFICIARYNAME = d.BENEFICIARYNAME,
                        BENEFICIARYNARRATION = d.BENEFICIARYNARRATION,
                        DEBITACCTNO = d.DEBITACCTNO,
                        DEBITBANKCODE = d.DEBITBANKCODE,
                        CREDITAMOUNT = d.CREDITAMOUNT,
                        REQUESTTYPE = ReqType,
                        SETTLEMENTDATE = d.SETTLEMENTDATE,
                        PROCESS_STATUS = proc,
                        CREATEDATE = curDate,
                        USERID = User.Identity.Name,
                        STATUS = open,
                        BENEFICIARYACCTNO_OLD = d.BENEFICIARYACCTNO_OLD,
                        BENEFICIARYBANKCODE_OLD = d.BENEFICIARYBANKCODE_OLD,
                        BENEFICIARYNAME_OLD = d.BENEFICIARYNAME_OLD,
                        BENEFICIARYNARRATION_OLD = d.BENEFICIARYNARRATION_OLD,
                        CREDITAMOUNT_OLD = d.CREDITAMOUNT_OLD,
                        DEBITACCTNO_OLD = d.DEBITACCTNO_OLD,
                        DEBITBANKCODE_OLD = d.DEBITBANKCODE_OLD,
                        EVENTTYPE = d.EVENTTYPE,
                        MERCHANTID = d.MERCHANTID,
                        REASON = d.REASON
                    };
                    repoNapsTemp.Insert(obj);
                    cnt++;
                }
                if (cnt > 0)
                {
                    SM_AUTHLIST auth = new SM_AUTHLIST()
                    {
                        CREATEDATE = DateTime.Now,
                        EVENTTYPE = eventInsert,
                        MENUID = menuId,
                        //MENUNAME = "",
                        RECORDID = null,
                        STATUS = open,
                        // TABLENAME = "ADMIN_DEPARTMENT",
                        URL = Request.FilePath,
                        USERID = User.Identity.Name,
                        INSTITUTION_ITBID = institutionId,
                        POSTTYPE = Batch,
                        BATCHID = bid,
                        ROUTEUSERID = ApproverId,
                    };
                    repoAuth.Insert(auth);
                    var suc = uow.Save(User.Identity.Name);
                    if (suc > 0)
                    {
                        EmailerNotification.SendToApprover(fullName, "Naps Nibss Posting", ApproverId);
                        return Json(new { RespCode = 0, RespMessage = "Record Processed Successfully and forwarded for approval." }, JsonRequestBehavior.AllowGet);

                    }
                }
                return Json(new { RespCode = 1, RespMessage = "No Record Processed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);
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
        decimal authId;
        void BindCombo(short action, decimal? authId = null)
        {
            var d = _repo.GetApprovalRoutePage(menuId, User.Identity.Name, action, authId);
            ViewBag.ApproverList = new SelectList(d, "ApproverId", "FullName");


            var ChannelList = _repo.GetChannel(0, true, "Active");
            ViewBag.ChannelList = new SelectList(ChannelList, "Code", "DESCRIPTION");
        }

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

                    ViewBag.HeaderTitle = "Authorize Detail for Naps";
                    //ViewBag.StatusVisible = true;
                    if (det != null)
                    {
                        obj.AuthId = authId;
                        obj.RecordId = det.RECORDID.GetValueOrDefault();
                        obj.BatchId = det.BATCHID;
                        obj.PostType = det.POSTTYPE;
                        obj.MenuId = det.MENUID.GetValueOrDefault();
                        ViewBag.Message = TempData["msg"];
                        var stat = ViewBag.Message != null ? null : "open";
                        var rec = _repo.GetNapsTemp(det.BATCHID, det.USERID, "Y");  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            ViewBag.SetDate = model.SETTLEMENTDATE.GetValueOrDefault().ToString("dd-MM-yyyy");
                            ViewBag.ReqType = model.REQUESTTYPE;
                            BindCombo(2, det.ITBID);
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.USERID;
                            ViewBag.Auth = obj;
                            var checker = repoAuthChecker.AllEager(d => d.AUTHLIST_ITBID == authId && d.USERID == User.Identity.Name).Count();
                            ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name) && checker <= 0; ;
                            var app_no = _repo.GetIfPageRequiresApproval(menuId, 2, authId);
                            if (app_no > 1) // page requires approval display approval view
                            {
                                ViewBag.MenuId = menuId;
                                ViewBag.DisplayAppoval = true;
                            }

                            return View("DetailAuth", rec);

                        }
                        //  return Json(rec, JsonRequestBehavior.AllowGet);
                        //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                        // return Json(obj1, JsonRequestBehavior.AllowGet);
                    }


                    return View("DetailAuth");
                }
                else
                {
                    return View("Error", "Home");
                }

            }
            catch (Exception ex)
            {
                return View("DetailAuth");
            }

        }
        bool sucNew = false;
        string respMsg;
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public ActionResult Approve(decimal AuthId, int? m, string ApproverId)
        {
            //int menuId = 0;
            //string msg = "";
            var route = repoRoute.AllEager(d => d.MENUID == menuId).FirstOrDefault();// _repo.GetIfPageRequiresApproval(m.GetValueOrDefault(), comp_code, 1);
            if (route != null)
            {
                checkerNo = route.NOLEVEL.GetValueOrDefault();
            }

            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    // TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                // int recordId = 0;
                bool suc = false;
                bool sucApp = false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(1, 0, 0)))
                //{
                var d = new AuthListUtil();
                //menuId = 5;
                var dd = d.GetCheckerRecord(menuId, AuthId, 0, institutionId);

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
                                        suc = ProcessBatchToNibss(rec2.BATCHID, rec2.USERID);
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
                                rec2.ROUTEUSERID = ApproverId;
                                rec2.EVENTTYPE = "NEFT";
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    //txscope.Complete();
                                    suc = true;

                                }
                            }
                            else
                            {
                                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                respMsg = "Problem processing request. Try again or contact Administrator.";
                                // TempData["msg"] = respMsg;
                                // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                return Json(new { RespCode = 1, RespMessage = respMsg });
                            }
                        }
                        else
                        {

                            rec2.ROUTEUSERID = ApproverId;
                            uow.Save(User.Identity.Name);
                            //txscope.Complete();
                            sucApp = true;

                        }
                    }
                }

                //}
                if (suc)
                {
                    // PostNaps(rec2.BATCHID, rec2.USERID);
                    EmailerNotification.SendApprovalRejectionMail2(rec2.ITBID, rec2.EVENTTYPE, approve, "Naps Record Approval", null);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    AuditHelper.PostAudit2(new SM_AUDIT() { AUTHID = "*", COLUMNNAME = "Naps Posting", EVENTDATE = DateTime.Now, EVENTTYPE = "A", INSTITUTION_ITBID = institutionId, NEWVALUE = rec2.BATCHID, RECORDID = rec2.BATCHID, USERID = rec2.USERID, TABLENAME = "NAPS" });
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                else if (sucApp)
                {
                    respMsg = "Record Approved Successfully, A Mail has been forwarded to the selected Authorizer for futher authorization.";
                    EmailerNotification.SendToApprover(fullName, "Naps Nibss Posting", ApproverId);
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = open });
                }

                // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                respMsg = "This request has already been processed by an authorizer.";
                // TempData["msg"] = respMsg;
                //return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                // TempData["msg"] = respMsg;
                // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
        }

        private bool ProcessBatchToNibss(string bATCHID, string uSERID)
        {
            var curDate = DateTime.Now;
            var cnt = 0;
            var rec = repoNapsTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID && d.PROCESS_STATUS == "Y").ToList();
            var recu = repoNapsTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID && d.PROCESS_STATUS != "Y").ToList();
            foreach (var d in rec)
            {
                var obj = new SM_NAPS_NIBSS()
                {
                    BATCHID = d.BATCHID,
                    BENEFICIARYACCTNO = d.BENEFICIARYACCTNO,
                    BENEFICIARYBANKCODE = d.BENEFICIARYBANKCODE,
                    BENEFICIARYNAME = d.BENEFICIARYNAME,
                    BENEFICIARYNARRATION = d.BENEFICIARYNARRATION,
                    DEBITACCTNO = d.DEBITACCTNO,
                    DEBITBANKCODE = d.DEBITBANKCODE,
                    CREDITAMOUNT = d.CREDITAMOUNT,
                    REQUESTTYPE = d.REQUESTTYPE,
                    SETTLEMENTDATE = d.SETTLEMENTDATE,
                    MERCHANTID = d.MERCHANTID,
                    //PROCESS_STATUS = proc,
                    CREATEDATE = curDate,
                    USERID = d.USERID,
                    STATUS = "P",
                    BENEFICIARYACCTNO_OLD = d.BENEFICIARYACCTNO_OLD,
                    BENEFICIARYBANKCODE_OLD = d.BENEFICIARYBANKCODE_OLD,
                    BENEFICIARYNAME_OLD = d.BENEFICIARYNAME_OLD,
                    BENEFICIARYNARRATION_OLD = d.BENEFICIARYNARRATION_OLD,
                    CREDITAMOUNT_OLD = d.CREDITAMOUNT_OLD,
                    DEBITACCTNO_OLD = d.DEBITACCTNO_OLD,
                    DEBITBANKCODE_OLD = d.DEBITBANKCODE_OLD,
                    EVENTTYPE = d.EVENTTYPE,
                    RESPCODE = "90",
                    RESPMESSAGE = "Approved for Posting",
                    REASON = d.REASON
                };

                repoNaps.Insert(obj);
                if (d.CREDITAMOUNT != d.CREDITAMOUNT_OLD)
                {
                    //adjustment entry will be raised
                    //call procedure to raise an entry in the settlement table
                    var rst = _repo.PostSetAdjustmentEntry(d);
                }
                cnt++;
            }

            foreach (var t in recu)
            {
                _repo.UpdateSettlementReportFlag(t);
            }

            if (cnt > 0)
            {
                return true;
            }
            return false;
        }


        //[HttpPost]
        //// [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(decimal AuthId, int? m)
        //{

        //    try
        //    {
        //        //int menuId;
        //        //var mid = SmartObj.Decrypt(m);
        //        //var ai = SmartObj.Decrypt(a_i);
        //        //if (int.TryParse(mid, out menuId) && decimal.TryParse(ai, out authId))
        //        //{
        //        var obj = new AuthViewObj();
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
        //        bool suc = false;
        //        using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //        {
        //            var d = new AuthListUtil();
        //            //menuId = 5;
        //            var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
        //            if (dd.authListObj.Count < checkerNo)
        //            {

        //                var chk = new SM_AUTHCHECKER()
        //                {
        //                    AUTHLIST_ITBID = authId,
        //                    CREATEDATE = DateTime.Now,
        //                    NARRATION = null,
        //                    STATUS = approve,
        //                    USERID = User.Identity.Name,
        //                };
        //                repoAuthChecker.Insert(chk);
        //                var rst = uow.Save(User.Identity.Name);
        //                if (rst > 0)
        //                {
        //                    var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
        //                    noA += 1;
        //                    if (noA == checkerNo)
        //                    {
        //                        recordId = (int)rec2.RECORDID;
        //                        menuId = rec2.MENUID.GetValueOrDefault();
        //                        switch (rec2.EVENTTYPE)
        //                        {
        //                            case "New":
        //                                {

        //                                    suc = CreateMainRecord(recordId);
        //                                    break;
        //                                }
        //                            case "Modify":
        //                                {

        //                                    suc = ModifyMainRecord(recordId);
        //                                    break;
        //                                }
        //                            case "CLOSE":
        //                                {

        //                                    suc = CloseMainRecord(recordId);
        //                                    break;
        //                                }
        //                            default:
        //                                {
        //                                    break;
        //                                }
        //                        }
        //                        // rec2.STATUS = close;

        //                        if (suc)
        //                        {
        //                            rec2.STATUS = approve;
        //                            var t = uow.Save(rec2.USERID, User.Identity.Name);
        //                            if (t > 0)
        //                            {
        //                                sucNew = true;
        //                                txscope.Complete();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        //                            respMsg = "Problem processing request. Try again or contact Administrator.";
        //                            // TempData["msg"] = respMsg;
        //                            // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //                            return Json(new { RespCode = 1, RespMessage = respMsg });
        //                        }
        //                    }

        //                    //if (!isApprove)
        //                    //{
        //                    //    pnlResponse.Visible = true;
        //                    //    pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold fade in";

        //                    //    pnlResponseMsg.Text = "Record Successfully Approved";
        //                    //}
        //                }
        //            }

        //        }
        //        if (sucNew)
        //        {
        //            EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Card Scheme Record", null, fullName);
        //            respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
        //            return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
        //        }
        //        respMsg = "This request has already been processed by an authorizer.";
        //        return Json(new { RespCode = 1, RespMessage = respMsg });

        //    }
        //    catch (Exception ex)
        //    {
        //        //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        //        respMsg = "Problem processing request. Try again or contact Administrator.";
        //        // TempData["msg"] = respMsg;
        //        // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        //        return Json(new { RespCode = 1, RespMessage = respMsg });
        //    }
        //}


        //[HttpPost]
        //// [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Reject(decimal AuthId, int? m, string Narration)
        //{
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
        //        using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //        {
        //            var d = new AuthListUtil();
        //            //menuId = 5;
        //            var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
        //            if (dd.authListObj.Count < checkerNo)
        //            {

        //                var chk = new SM_AUTHCHECKER()
        //                {
        //                    AUTHLIST_ITBID = AuthId,
        //                    CREATEDATE = DateTime.Now,
        //                    NARRATION = Narration,
        //                    STATUS = reject,
        //                    USERID = User.Identity.Name,
        //                };
        //                repoAuthChecker.Insert(chk);
        //                var rst = uow.Save(User.Identity.Name);
        //                if (rst > 0)
        //                {
        //                    var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
        //                    noA += 1;
        //                    if (noA == checkerNo)
        //                    {
        //                        recordId = (int)rec2.RECORDID;
        //                        menuId = rec2.MENUID.GetValueOrDefault();
        //                        var recc = repoSchemeTemp.Find(recordId);
        //                        if (recc != null)
        //                        {
        //                            recc.STATUS = reject;
        //                        }

        //                        rec2.STATUS = reject;
        //                        var t = uow.Save(User.Identity.Name);
        //                        if (t > 0)
        //                        {
        //                            sucNew = true;
        //                            txscope.Complete();
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        if (sucNew)
        //        {
        //            respMsg = "Record Rejected. A mail has been sent to the user.";
        //            return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
        //        }
        //        respMsg = "Problem processing request. Try again or contact Administrator.";

        //        return Json(new { RespCode = 1, RespMessage = respMsg });
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
        // [ValidateAntiForgeryToken]
        public ActionResult UploadFiles()
        {
            IList<NapsObj> model = null;
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
                        var cnt = Naps.PostNapsBulk(dataList.ToList(), User.Identity.Name);

                        if (cnt > 0)
                        {

                            var rst = Naps.GetNaps(User.Identity.Name, null);
                            var html = PartialView("_ViewNapsUpld", rst).RenderToString();
                            return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        }
                        else
                        {
                            var html = PartialView("_ViewNapsUpld").RenderToString();
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
        private static NapsObj addRecord(IList<string> rowData, IList<string> columnNames)
        {
            try
            {
                var obj = new NapsObj()
                {
                    DEBITACCTNO = rowData[0],
                    DEBITBANKCODE = rowData[1],
                    BENEFICIARYNAME = rowData[2],
                    BENEFICIARYACCTNO = rowData[3].Trim(),
                    //BENEFICIARYACCTNO = rowData[3] != null ? rowData[3].Trim() : rowData[3],
                    BENEFICIARYBANKCODE = rowData[4].Trim(),
                    //BENEFICIARYBANKCODE = rowData[4] != null ? rowData[4].Trim() : rowData[4],
                    CREDITAMOUNT = rowData[5].Trim().ToDecimalNullable(),
                    BENEFICIARYNARRATION = rowData[6].Trim(),
                    //BENEFICIARYNARRATION = rowData[6] != null ? rowData[6].Trim() : rowData[6],
                    VALIDATIONERRORSTATUS = true,
                    CREATEDATE = DateTime.Now,
                    EVENTTYPE = "New",
                    REQUESTTYPE = "M",

                    //USERID = User.Identity.Name
                };
                return obj;
            }
            catch (Exception ex)
            {
                return new NapsObj();
            }
        }
        protected NapsObj ValidateUpload(NapsObj t)
        {

            // List<MerchantUpldObj> lst = new List<MerchantUpldObj>();

            //var rec = Naps.GetNaps(User.Identity.Name,null);

            int totalErrorCount = 0;
            //foreach (var t in rec)
            //{
            int errorCount = 0;
            var validationErrorMessage = new List<string>();
            decimal mid;
            //int specialCount = 0;
            if (!decimal.TryParse(t.DEBITBANKCODE, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITBANKCODE must be number"));
            }
            if (t.DEBITBANKCODE.Length != 3)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITBANKCODE must be {0} Character", 3));

            }
            if (!decimal.TryParse(t.DEBITACCTNO, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITACCTNO must be number"));
            }
            if (t.DEBITACCTNO.Length != 10)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITACCTNO must be {0} Character", 10));

            }
            if (!decimal.TryParse(t.BENEFICIARYBANKCODE, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYBANKCODE must be number"));
            }
            if (t.BENEFICIARYBANKCODE.Length != 3)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYBANKCODE must be {0} Character", 3));

            }
            if (!decimal.TryParse(t.BENEFICIARYACCTNO, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYACCTNO must be number"));
            }
            if (t.BENEFICIARYACCTNO.Length != 10)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYACCTNO must be {0} Character", 10));

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
            //var rst = Naps.PostNaps(t, 2);
            //SessionHelper.GetCart(Session).UpdateItem(t);
            // }

            //  lst.AddRange(lst);
            //if (rec.Count > 0)
            //{
            //    if (totalErrorCount > 0)
            //    {

            //        //pnlResponse.Visible = true;
            //        //pnlResponse.CssClass = "alert alert-danger alert-dismissable alert-bold";
            //        //pnlResponseMsg.Text = string.Format("{0} Record(s) Failed Validation from Batch...", totalErrorCount);
            //        //if (totalErrorCount == rec.Count)
            //        //{
            //        //    btnProcess.Enabled = false;
            //        //}
            //        //else
            //        //{
            //        //    btnProcess.Enabled = true;
            //        //}

            //    }
            //    else
            //    {
            //        //pnlResponse.Visible = true;
            //        //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
            //        //pnlResponseMsg.Text = "Batch Validated Successfully...You can now save for further processing";
            //        // btnProcess.Enabled = false;
            //        //btnProcess.Enabled = true;
            //    }
            //}
            return t;
            //}
            //catch (Exception ex)
            //{
            //    return -1;
            //}
        }

        [HttpPost]
        // [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Reject(decimal AuthId, int? m, string Narration)
        {
            var suc = false;
            try
            {
                var route = repoRoute.AllEager(dr => dr.MENUID == menuId).FirstOrDefault();// _repo.GetIfPageRequiresApproval(m.GetValueOrDefault(), comp_code, 1);
                if (route != null)
                {
                    checkerNo = route.NOLEVEL.GetValueOrDefault();
                }
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
                //int recordId = 0;
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
                        //if (noA == checkerNo)
                        //{
                        //recordId = (int)rec2.RECORDID;
                        //menuId = rec2.MENUID.GetValueOrDefault();
                        var recc = repoNapsTemp.AllEager(g => g.BATCHID == rec2.BATCHID && g.USERID == rec2.USERID).ToList();
                        foreach (var g in recc)
                        {
                            g.STATUS = reject;
                        }

                        rec2.STATUS = reject;
                        var t = uow.Save(User.Identity.Name);
                        if (t > 0)
                        {
                            suc = true;
                            //txscope.Complete();
                        }
                    }
                }
                // }
                //}
                if (suc)
                {

                    EmailerNotification.SendApprovalRejectionMail2(rec2.ITBID, rec2.EVENTTYPE, reject, "Naps Record Rejection", Narration);
                    respMsg = "Record Rejected. A mail has been sent to the user.";

                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }


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
        //string destUri = "";
        [MyAuthorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ProcessUpld(string ApproverId)
        {
            try
            {
                var curDate = DateTime.Now;
                var bid = SmartObj.GenRefNo();
                var cnt = 0;
                var rec = Naps.GetNaps(User.Identity.Name, null).Where(d => d.VALIDATIONERRORSTATUS == false).ToList();
                foreach (var d in rec)
                {
                    var proc = "Y";

                    var obj = new SM_NAPS_NIBSS_TEMP()
                    {
                        BATCHID = bid,
                        BENEFICIARYACCTNO = d.BENEFICIARYACCTNO,
                        BENEFICIARYBANKCODE = d.BENEFICIARYBANKCODE,
                        BENEFICIARYNAME = d.BENEFICIARYNAME,
                        BENEFICIARYNARRATION = d.BENEFICIARYNARRATION,
                        DEBITACCTNO = d.DEBITACCTNO,
                        DEBITBANKCODE = d.DEBITBANKCODE,
                        CREDITAMOUNT = d.CREDITAMOUNT,
                        REQUESTTYPE = "M",
                        SETTLEMENTDATE = d.SETTLEMENTDATE,
                        PROCESS_STATUS = proc,
                        CREATEDATE = curDate,
                        USERID = User.Identity.Name,
                        STATUS = open,
                        MERCHANTID = d.MERCHANTID

                    };
                    repoNapsTemp.Insert(obj);
                    cnt++;
                }
                if (cnt > 0)
                {
                    SM_AUTHLIST auth = new SM_AUTHLIST()
                    {
                        CREATEDATE = DateTime.Now,
                        EVENTTYPE = eventInsert,
                        MENUID = menuId,
                        //MENUNAME = "",
                        RECORDID = null,
                        STATUS = open,
                        // TABLENAME = "ADMIN_DEPARTMENT",
                        URL = Request.FilePath,
                        USERID = User.Identity.Name,
                        INSTITUTION_ITBID = institutionId,
                        POSTTYPE = Batch,
                        BATCHID = bid,
                        ROUTEUSERID = ApproverId,
                    };
                    repoAuth.Insert(auth);
                    var suc = uow.Save(User.Identity.Name);
                    if (suc > 0)
                    {
                        EmailerNotification.SendToApprover(fullName, "Naps Nibss Posting", ApproverId);
                        return Json(new { RespCode = 0, RespMessage = "Record Processed Successfully and forwarded for approval." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { RespCode = 1, RespMessage = "No Record Processed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        void PostNaps(string _batchId, string _userId)
        {
            try
            {
                var recP = repoNaps.AllEagerLocal(d => d.BATCHID == _batchId && d.USERID == _userId).ToList();

                var destUri = ConfigurationManager.AppSettings["naps_url"].ToString();
                var appUser = ConfigurationManager.AppSettings["naps_appuser"].ToString();
                var pass = ConfigurationManager.AppSettings["naps_password"].ToString();
                foreach (var t in recP)
                {
                    try
                    {
                        var data = "";
                        var rst = "";
                        var scheduleId = SmartObj.GenRefNo();
                        var auth = new jsonAuthKey()
                        {
                            AppUser = appUser,
                            Password = pass,
                            FileName = "FilenameUploaded",
                            ScheduleId = scheduleId,
                            DebitSortCode = t.DEBITBANKCODE,
                            DebitAccountNumber = t.DEBITACCTNO
                        };
                        var payObj = new requestPaymentObj()
                        {
                            Beneficiary = t.BENEFICIARYNAME,
                            AccountNumber = t.BENEFICIARYACCTNO,
                            Amount = t.CREDITAMOUNT.GetValueOrDefault().ToString("F"),
                            Narration = t.BENEFICIARYNARRATION,
                            SortCode = t.BENEFICIARYBANKCODE
                        };
                        var dList = new List<requestPaymentObj>();
                        dList.Add(payObj);
                        //var jsonPay = JsonConvert.SerializeObject(dList);
                        var authKey = JsonConvert.SerializeObject(auth);
                        var payJson = JsonConvert.SerializeObject(dList);
                        using (WebClient postClient = new WebClient())
                        {
                            postClient.Proxy = null;
                            //postClient.Headers.Add(HttpRequestHeader.ContentType, "application/soap+xml; charset=utf-8");
                            postClient.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
                            //postClient.Headers.Add("SOAPAction", "http://tempuri.org");

                            StreamReader reader = new StreamReader(Server.MapPath("~/NapTemplate/TranxPosting.xml"));

                            data = string.Format(reader.ReadToEnd(), authKey, payJson);
                            reader.Close();

                            //  postClient.UploadStringCompleted += new UploadStringCompletedEventHandler(postClient_UploadStringCompleted);
                            try
                            {
                                rst = postClient.UploadString(new Uri(destUri), data);
                            }
                            catch (Exception ex)
                            {
                                // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
                                t.RESPCODE = "98";
                                t.RESPMESSAGE = ex.Message;
                                // t.SCHEDULEID = scheduleId;
                                uow.Save(t.USERID);
                                continue;
                            }
                            //ServicePointManager.Expect100Continue = true;
                            //.ServerCertificateValidationCallback = MyCertHandler;
                        }
                        var ret = postClient_UploadStringCompleted(rst);
                        if (ret != null)
                        {
                            t.RESPCODE = ret.RespCode2;
                            t.RESPMESSAGE = ret.RespMessage;
                            // t.SCHEDULEID = scheduleId;
                            uow.Save(t.USERID);
                        }

                    }
                    catch (Exception ex)
                    {
                        // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
                        t.RESPCODE = "99";
                        t.RESPMESSAGE = ex.Message;
                        // t.SCHEDULEID = scheduleId;
                        uow.Save(t.USERID);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        OutPutObj postClient_UploadStringCompleted(string result)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result);
            var resp = xdoc.InnerText;
            //foreach (XmlNode node in xdoc.GetElementsByTagName("UploadPayment"))
            //{
            //    foreach (XmlNode node1 in node.ChildNodes)
            //    {
            //        switch (node1.Name)
            //        {
            //            case "RespCode":
            //               // rst = node1.InnerText;
            //                break;
            //            case "RespMessage":
            //                respMsg = node1.InnerText;
            //                break;
            //        }
            //    }
            //}
            var retObj = new OutPutObj();
            var objreq = JsonConvert.DeserializeObject<PayResponse>(resp);
            if (objreq != null && objreq.PaymentResponse != null)
            {
                var rst = objreq.PaymentResponse;
                if (rst.Header != null)
                {
                    retObj.RespCode2 = rst.Header.Status;
                    retObj.RespMessage = SmartObj.GetNapsMessage(retObj.RespCode2);
                    return retObj;
                }
            }
            retObj.RespCode2 = "99";
            retObj.RespMessage = "Posting to Nibss Message cannot be interpreted.";
            return retObj;
        }

    }
}