using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Dapper.Utilities;
using Generic.Dapper.Utility;
using Generic.Data;
using Generic.Data.Model;
using Generic.Data.Utilities;
using SettlementMaster.App.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class ResetLockoutController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;

        //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_RESETLOCKOUT_TEMP> repoReset = null;
        private readonly IRepository<AspNetUser> repoUser = null;
       
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
        int menuId = 32;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public ResetLockoutController()
        {
            uow = new UnitOfWork();
            //repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoReset = new Repository<SM_RESETLOCKOUT_TEMP>(uow);
            repoUser = new Repository<AspNetUser>(uow);
            
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleId = user.UserRole;
                institutionId = user.InstitutionId;
                fullName = user.FullName;
                deptCode = user.DeptCode;
            }

        }
        //[MyAuthorize]
        public ActionResult Index(string m)
        {
            // GetMenuId();
            //SessionHelper.GetRvHead(Session).Clear();
            //menuId = SmartUtil.GetMenuId(m);
            //if (menuId == 0)
            //{
            //    return RedirectToAction("Error", "Home");
            //}

            ViewBag.MenuId = menuId; // HttpUtility.UrlEncode(m);
            return View();
        }
        public async Task<ActionResult> LockOutList()
        {
            try
            {
                var rec = await _repo.GetUserLockOutAsync();  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<UserObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult ResetLockout(string[] selected_value)
        {

            try
            {
                //ViewBag.MenuId = menuId; //HttpUtility.UrlDecode(m);
                var bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (selected_value != null)
                    {
                        foreach (var d in selected_value)
                        {
                            var BType = new SM_RESETLOCKOUT_TEMP()
                            {
                                BatchId = bid,
                                UserName = d,
                                Status = open,
                                UserId = User.Identity.Name,
                                CreateDate = DateTime.Now,
                            };
                            repoReset.Insert(BType);
                        }

                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,// model.STATUS == active ? eventEdit : model.STATUS,
                                MENUID = menuId,
                                BATCHID = bid,
                                
                                //MENUNAME = "",
                                //RECORDID = BType.ITBID,
                                STATUS = open,
                                // TABLENAME = "ADMIN_DEPARTMENT",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Lockout Reset Record");

                                return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });
                            }
                            else
                            {

                                return Json(new { RespCode = 1, RespMessage = "Problem Processing Record." });

                            }
                        }

                    }
                    else
                    {
                        return Json(new { RespCode = 1, RespMessage = "Problem Processing Record." });

                    }
                }
                // If we got this far, something failed, redisplay form
                return Json(new { RespCode = 1, RespMessage = errorMsg });

            }
            catch (SqlException ex)
            {
                var msg = "Problem Updating Record.";
                return Json(new { RespCode = 1, RespMessage = msg });
            }
            catch (Exception ex)
            {
                var msg = "Problem Updating Record.";
                return Json(new { RespCode = 1, RespMessage = msg });

            }
        }

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

                    ViewBag.HeaderTitle = "Authorize Detail for Reset Lockout";
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
                        var rec = _repo.GetUserLockOutTemp(det.BATCHID,det.USERID);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec[0];
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.CREATED_BY;
                            ViewBag.Auth = obj;
                            ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);
                            ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus(), "Code", "Description");
                            // return null;

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

        bool sucNew;
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(decimal AuthId, int? m)
        {
            try
            {
                var obj = new AuthViewObj();
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
                bool suc = false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                //{
                    var d = new AuthListUtil();
                    //menuId = 5;
                    var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
                    if (dd.authListObj.Count < checkerNo)
                    {

                        var chk = new SM_AUTHCHECKER()
                        {
                            AUTHLIST_ITBID = authId,
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
                               // recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                        {

                                            suc = ResetUser(rec2.BATCHID,rec2.USERID);
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
                                        sucNew = true;
                                        //txscope.Complete();

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


                        }
                    }
                    // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });

               // }
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Reset User Record", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                respMsg = "This request has already been processed by an authorizer.";
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                // TempData["msg"] = respMsg;
                // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool ResetUser(string bATCHID, string uSERID)
        {
            var rec = repoReset.AllEager(d => d.BatchId == bATCHID && d.UserId == uSERID).ToList();
            foreach(var t in rec)
            {
                t.Status = approve;
                var sig = repoUser.AllEager(d => d.UserName == t.UserName).FirstOrDefault();
                if(sig != null)
                {
                    sig.Status = active;
                    sig.LockoutEndDateUtc = null;
                }
                
            }
            return true;
        }

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
                                //recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();

                                RejectBatch(rec2.BATCHID, rec2.USERID);

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    sucNew = true;
                                   // txscope.Complete();
                                }

                            }
                        }
                    }

                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Reset Lockout Rejection", Narration, fullName);
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }
                respMsg = "Problem processing request. Try again or contact Administrator.";

                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                //TempData["msg"] = respMsg;
                return Json(new { RespCode = 1, RespMessage = respMsg });

                //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });

            }
        }
        private bool RejectBatch(string bATCHID, string uSERID)
        {
            var rec = repoReset.AllEager(d => d.BatchId == bATCHID && d.UserId == uSERID).ToList();
            foreach (var t in rec)
            {
                t.Status = reject;
            }
            return true;
        }

    }
}