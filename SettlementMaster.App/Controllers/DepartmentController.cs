﻿using Generic.Dapper.Data;
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

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_DEPARTMENT> repoDept = null;
        private readonly IRepository<SM_DEPARTMENTTEMP> repoDeptTemp = null;
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
        int menuId = 8;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;string deptCode;
        // GET: Roles
        public DepartmentController()
        {
            uow = new UnitOfWork();
            repoDept = new Repository<SM_DEPARTMENT>(uow);
            repoDeptTemp = new Repository<SM_DEPARTMENTTEMP>(uow);
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
            //var m = Request.QueryString["m"];
            //if (int.TryParse(m, out menuId))
            //{
            //     RedirectToAction("Index","Home");
            //}
            // var allUrlKeyValues = ControllerContext.Controller..Request.GetQueryNameValuePairs();
            // var dd = HttpContext.Request.QueryString["m"];
            //string search = allUrlKeyValues.SingleOrDefault(x => x.Key == "search.value").Value;
            //string order = allUrlKeyValues.SingleOrDefault(x => x.Key == "order[0][column]").Value;
            //string sortDir = allUrlKeyValues.SingleOrDefault(x => x.Key == "order[0][dir]").Value;
        }
        [MyAuthorize]
        public ActionResult Index(string m)
        {
            try
            {
                // GetMenuId();
                menuId = SmartUtil.GetMenuId(m);
                if (menuId == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.MenuId = HttpUtility.UrlEncode(m);
                GetPriv();
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        void GetPriv()
        {
            var rec =  _repo.GetMenuPrivilege(menuId, roleId);
            if (rec != null)
            {
                ViewBag.CanAdd = rec.CanAdd;
                ViewBag.CanEdit = rec.CanEdit;
            }
        }

        public async Task<ActionResult> DeptList()
        {
            try
            {
                var rec = await _repo.GetDepartmentAsync(0, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<DepartmentObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }

        //private void AddErrors(ModelError result)
        //{
        //    foreach (var error in result.ErrorMessage)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}
        //
        // POST: /Account/Register
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentObj model, string m)
        {
            try
            {
                menuId = SmartUtil.GetMenuId(m);
                ViewBag.MenuId = HttpUtility.UrlDecode(m);

                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID > 0)
                    {
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        var BType = new SM_DEPARTMENTTEMP()
                        {
                            DEPARTMENTCODE = model.DEPARTMENTCODE,
                            DEPARTMENTNAME = model.DEPARTMENTNAME,
                            
                            STATUS = open,
                            RECORDID = model.ITBID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now
                        };
                        repoDeptTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
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
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Department Record");
                                TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return RedirectToAction("Index", "Department", new { m = m });
                                //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });                         
                            }
                            else
                            {
                                ViewBag.Message = "Problem Updating Record.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return View("Add", new { m = m, id = model.ITBID });
                                //return Json(new { RespCode = 1, RespMessage = "Problem Updating Record." });

                            }
                        }

                        //    var ret = _repo.PostRole(model, 1);
                        //if (ret != null && ret.RespCode == 0)
                        //{
                        //    model.RoleId = int.Parse(ret.OutputKey.ToString());
                        //    return Json(new { data = model, RespCode = 0, RespMessage = "Record Created Successfully" });
                        //}

                    }
                    else
                    {
                        if (!validateForm(model))
                        {
                            ViewBag.Message = "Department Code already Exist.";
                            return View("Add", model);
                        };
                        var BType = new SM_DEPARTMENTTEMP()
                        {
                            DEPARTMENTCODE = model.DEPARTMENTCODE,
                            DEPARTMENTNAME = model.DEPARTMENTNAME,
                            STATUS = open,
                            RECORDID = model.ITBID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now
                        };
                        repoDeptTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            //var controller =  ControllerContext.Controller..ToString();
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                RECORDID = BType.ITBID,
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
                                //newR = 1;
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Department Record");
                                TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return RedirectToAction("Index", new { m = m });

                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });

                            }
                            else
                            {
                                ViewBag.Message = "Problem Updating Record.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return View("Add", new { m = m });

                                // return Json(new { RespCode = 1, RespMessage = "Problem Creating Record." });
                            }
                        }
                    }
                }
                // If we got this far, something failed, redisplay form
                return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                ViewBag.Message = "Problem Updating Record.";
                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                return View("Add", new { m = m });

                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Problem Updating Record.";
                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                return View("Add", new { m = m });
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
        }

        private bool validateForm(DepartmentObj obj)
        {
            var exist = repoDept.AllEager(f => f.DEPARTMENTCODE == obj.DEPARTMENTCODE).FirstOrDefault();
            if (exist == null)
            {
                return true;
            }
            return false;
        }

        // [AllowAnonymous]
        //public async Task<JsonResult> ViewRole(int id = 0)
        //{
        //    if (id == 0)
        //    {

        //        return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
        //    }
        //    //var d = _repo.GetSession(0, true);
        //    try
        //    {
        //        var rec = await _repo.GetRolesAsync(id, false);  //repoSession.FindAsync(id);
        //        if (rec == null)
        //        {
        //            // return Json(null, JsonRequestBehavior.AllowGet);
        //            var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
        //            return Json(obj, JsonRequestBehavior.AllowGet);

        //        }
        //        //  return Json(rec, JsonRequestBehavior.AllowGet);
        //        var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
        //        return Json(obj1, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        var obj1 = new { RespCode = 2, RespMessage = ex.Message };
        //        return Json(obj1, JsonRequestBehavior.AllowGet);
        //    }
        //}

        decimal authId;
        string respMsg = null;
        public async Task<ActionResult> Add(int id = 0, string m = null)
        {
            try
            {
                ViewBag.MenuId = HttpUtility.UrlDecode(m);

                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Department";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add", new DepartmentObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Department";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetDepartmentAsync(id, false);
                    if (rec == null)
                    {
                        //bad request
                        // return Json(null, JsonRequestBehavior.AllowGet);
                        //var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                        // return Json(obj, JsonRequestBehavior.AllowGet);
                        TempData["msg"] = "Record Not Found";
                        return View("Index");
                    }
                    var model = rec.FirstOrDefault();
                    ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus(), "Code", "Description");
                    GetPriv();
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

                    ViewBag.HeaderTitle = "Authorize Detail for Department";
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
                        var rec = _repo.GetDepartment((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.CREATED_BY;
                            ViewBag.Auth = obj;
                            ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);
                            var dept = _repo.GetDepartment(0, true, "Active");
                            ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus(), "Code", "Description");
                            // return null;

                            return View("DetailAuth", model);

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
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(decimal AuthId, int? m)
        {
            var sucNew = false;
            try
            {
                //int menuId;
                //var mid = SmartObj.Decrypt(m);
                //var ai = SmartObj.Decrypt(a_i);
                //if (int.TryParse(mid, out menuId) && decimal.TryParse(ai, out authId))
                //{
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
                                recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                        {
                                            suc = CreateMainRecord(recordId);
                                            break;
                                        }
                                    case "Modify":
                                        {
                                            suc = ModifyMainRecord(recordId);
                                            break;
                                        }
                                    case "CLOSE":
                                        {
                                            suc = CloseMainRecord(recordId);
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                                if (suc)
                                {
                                    rec2.STATUS = approve;
                                    var t = uow.Save(rec2.USERID,User.Identity.Name);
                                    if (t > 0)
                                    {
                                        sucNew = true;
                                       // txscope.Complete();
                                    }
                                }
                                else
                                {
                                    respMsg = "Problem processing request. Try again or contact Administrator.";

                                    return Json(new { RespCode = 1, RespMessage = respMsg });
                                }
                            }

                            //if (!isApprove)
                            //{
                            //    pnlResponse.Visible = true;
                            //    pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold fade in";

                            //    pnlResponseMsg.Text = "Record Successfully Approved";
                            //}
                        }
                    }
                    // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                    
                //}
                if(sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Department Approval", null, fullName);
                   
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                   
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                respMsg = "This request has already been processed by an authorizer.";

                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                respMsg = "Problem processing request. Try again or contact Administrator.";
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool CloseMainRecord(int recordId)
        {
            var rec = repoDeptTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = reject;
                return true;
            }
            return false;
        }

        private bool CreateMainRecord(int recordId)
        {
            var rec = repoDeptTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var sig = repoDept.AllEager(d => d.DEPARTMENTCODE == rec.DEPARTMENTCODE).FirstOrDefault();
                if (sig == null)
                {
                    var obj = new SM_DEPARTMENT()
                    {
                        CREATEDATE = rec.CREATEDATE,
                        DEPARTMENTCODE = rec.DEPARTMENTCODE,
                        DEPARTMENTNAME = rec.DEPARTMENTNAME,
                        STATUS = active,
                        USERID = rec.USERID,
                        //LAST_MODIFIED_AUTHID = User.Identity.Name,
                        //LAST_MODIFIED_DATE = DateTime.Now
                    };
                    repoDept.Insert(obj);
                    return true;
                }
                else
                {
                    sig.DEPARTMENTNAME = rec.DEPARTMENTNAME;
                    sig.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    sig.LAST_MODIFIED_UID = rec.USERID;
                    sig.LAST_MODIFIED_DATE = DateTime.Now;
                    return true;
                }
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId)
        {
            var rec = repoDeptTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoDept.Find((int)rec.RECORDID);
                if (obj != null)
                {
                    obj.DEPARTMENTNAME = rec.DEPARTMENTNAME;
                    obj.LAST_MODIFIED_UID = rec.USERID;
                    obj.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    obj.LAST_MODIFIED_DATE = DateTime.Now;
                    obj.STATUS = active;

                    // repoDept.Insert(obj);
                    return true;
                }
            }
            return false;
        }


        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(decimal AuthId, int? m, string Narration)
        {
            var suc = false;
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
                                recordId = (int)rec2.RECORDID;
                                //menuId = rec2.MENUID.GetValueOrDefault();
                                var recc = repoDeptTemp.Find(recordId);
                                if (recc != null)
                                {
                                    recc.STATUS = reject;
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
                    }
               // }
                if(suc)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Department Rejection", null, fullName);
                   
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }
                return Json(new { RespCode = 1, RespMessage = respMsg });

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