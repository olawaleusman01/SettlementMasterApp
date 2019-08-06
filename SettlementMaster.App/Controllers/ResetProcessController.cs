using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Data;
using SettlementMaster.App.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class ResetProcessController : Controller
     {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_COMPANY_PROFILE> repoComp = null;

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
        public ResetProcessController()
        {
            uow = new UnitOfWork();
            //repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoComp = new Repository<SM_COMPANY_PROFILE>(uow);
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
        public async Task<ActionResult> Index(string m)
        {
            // GetMenuId();
            //SessionHelper.GetRvHead(Session).Clear();
            //menuId = SmartUtil.GetMenuId(m);
            //if (menuId == 0)
            //{
            //    return RedirectToAction("Error", "Home");
            //}

            ViewBag.MenuId = menuId; // HttpUtility.UrlEncode(m);
            var rec = await _repo.GetCompanyProfileAsync(0, false);
            ViewBag.ProcessFlag = (rec.PROCESS_FLAG == "P") ? true: false ;
            ViewBag.ProcessFlag_NAP = (rec.PROCESS_FLAG_NAP == "P") ? true : false;
            // return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            if (TempData["msg"]!=null)
            {
                ViewBag.Success = TempData["msg"];
            }
            
            return View();
        }
        //public async Task<ActionResult> ProcessList()
        //{
        //    try
        //    {


        //    }
        //    catch (Exception ex)
        //    {
        //        var obj1 = new { data = new List<UserObj>(), RespCode = 2, RespMessage = ex.Message };
        //        return Json(obj1, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
        public ActionResult updProcess(int? id, string status)
        {

            try
            {
                //ViewBag.MenuId = menuId; //HttpUtility.UrlDecode(m);
                
                            var errorMsg = "";



                                _repo.PostProcessReset();

                             
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Settlement Processing Lock Reset");

                                  TempData["msg"] = "Record Updated Successfully";
                                  return View("Index");



            }
            catch (SqlException ex)
            {
                TempData["msg"] = "Problem Updating Record.";
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Problem Updating Record.";
                return View("Index");

            }
        }

        [HttpPost]
        public ActionResult updProcess2(int? id, string status)
        {

            try
            {
                //ViewBag.MenuId = menuId; //HttpUtility.UrlDecode(m);

               


                _repo.PostProcessReset2();


                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Settlement NAP Processing Lock Reset");

                TempData["msg"] = "Record Updated Successfully";
                return View("Index");





            }
            catch (SqlException ex)
            {
                TempData["msg"] = "Problem Updating Record.";
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Problem Updating Record.";
                return View("Index");

            }
        }
        [HttpPost]
        public ActionResult updProcess3(int? id, string status)
        {

            try
            {
                //ViewBag.MenuId = menuId; //HttpUtility.UrlDecode(m);




                _repo.PostProcessReset3();


                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Settlement NAP Processing Lock Reset");

                TempData["msg"] = "Record Updated Successfully";
                return View("Index");





            }
            catch (SqlException ex)
            {
                  TempData["msg"] = "Problem Updating Record.";
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Problem Updating Record.";
                return View("Index");

            }
        }


        bool sucNew;
        //[HttpPost]
        //// [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(decimal AuthId, int? m)
        //{
        //    try
        //    {
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
        //        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //        //{
        //        var d = new AuthListUtil();
        //        //menuId = 5;
        //        var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
        //        if (dd.authListObj.Count < checkerNo)
        //        {

        //            var chk = new SM_AUTHCHECKER()
        //            {
        //                AUTHLIST_ITBID = authId,
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
        //                    // recordId = (int)rec2.RECORDID;
        //                    menuId = rec2.MENUID.GetValueOrDefault();
        //                    switch (rec2.EVENTTYPE)
        //                    {
        //                        case "New":
        //                            {

        //                                suc = ResetUser(rec2.BATCHID, rec2.USERID);
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
        //                            sucNew = true;
        //                            //txscope.Complete();

        //                        }
        //                    }
        //                    else
        //                    {
        //                        //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        //                        respMsg = "Problem processing request. Try again or contact Administrator.";
        //                        // TempData["msg"] = respMsg;
        //                        // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        //                        return Json(new { RespCode = 1, RespMessage = respMsg });
        //                    }
        //                }


        //            }
        //        }
        //        // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });

        //        // }
        //        if (sucNew)
        //        {
        //            EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Reset User Record", null, fullName);
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

        
        ////[HttpPost]
        ////// [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult Reject(decimal AuthId, int? m, string Narration)
        ////{
        ////    //int menuId = 0;
        ////    //string msg = "";
        ////    try
        ////    {
        ////        var rec2 = repoAuth.Find(AuthId);
        ////        if (rec2 == null)
        ////        {
        ////            respMsg = "Problem processing request. Try again or contact Administrator.";
        ////            //TempData["msg"] = respMsg;
        ////            //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
        ////            return Json(new { RespCode = 1, RespMessage = respMsg });

        ////        }
        ////        else if (rec2.STATUS.ToLower() != "open")
        ////        {
        ////            respMsg = "This request has already been processed by an authorizer.";
        ////            //TempData["msg"] = respMsg;
        ////            //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
        ////            return Json(new { RespCode = 1, RespMessage = respMsg });
        ////        }
        ////        int recordId = 0;
        ////        // bool suc = false;
        ////        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        ////        //{
        ////        var d = new AuthListUtil();
        ////        //menuId = 5;
        ////        var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
        ////        if (dd.authListObj.Count < checkerNo)
        ////        {

        ////            var chk = new SM_AUTHCHECKER()
        ////            {
        ////                AUTHLIST_ITBID = AuthId,
        ////                CREATEDATE = DateTime.Now,
        ////                NARRATION = Narration,
        ////                STATUS = reject,
        ////                USERID = User.Identity.Name,
        ////            };
        ////            repoAuthChecker.Insert(chk);
        ////            var rst = uow.Save(User.Identity.Name);
        ////            if (rst > 0)
        ////            {
        ////                var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
        ////                noA += 1;
        ////                if (noA == checkerNo)
        ////                {
        ////                    //recordId = (int)rec2.RECORDID;
        ////                    menuId = rec2.MENUID.GetValueOrDefault();

        ////                    RejectBatch(rec2.BATCHID, rec2.USERID);

        ////                    rec2.STATUS = reject;
        ////                    var t = uow.Save(User.Identity.Name);
        ////                    if (t > 0)
        ////                    {
        ////                        sucNew = true;
        ////                        // txscope.Complete();
        ////                    }

        ////                }
        ////            }
        ////        }

        ////        //}
        ////        if (sucNew)
        ////        {
        ////            EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Reset Lockout Rejection", Narration, fullName);
        ////            respMsg = "Record Rejected. A mail has been sent to the user.";
        ////            return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
        ////        }
        ////        respMsg = "Problem processing request. Try again or contact Administrator.";

        ////        return Json(new { RespCode = 1, RespMessage = respMsg });

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
        ////        respMsg = "Problem processing request. Try again or contact Administrator.";
        ////        //TempData["msg"] = respMsg;
        ////        return Json(new { RespCode = 1, RespMessage = respMsg });

        ////        //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });

        ////    }
        ////}
        ////private bool RejectBatch(string bATCHID, string uSERID)
        ////{
        ////    var rec = repoReset.AllEager(d => d.BatchId == bATCHID && d.UserId == uSERID).ToList();
        ////    foreach (var t in rec)
        ////    {
        ////        t.Status = reject;
        ////    }
        ////    return true;
        ////}

    }
}