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

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_ROLES> repoRole = null;
        private readonly IRepository<SM_ROLESTEMP> repoRoleTemp = null;
        private readonly IRepository<SM_ROLEPRIV> repoRolePriv = null;
        private readonly IRepository<SM_ROLEPRIVTEMP> repoRolePrivTemp = null;
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
        int menuId = 5;
        int menuIdPriv = 5;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName = "";
        string deptCode = "";
        // GET: Roles
        public RolesController()
        {
            uow = new UnitOfWork();
            repoRole = new Repository<SM_ROLES>(uow);
            repoRoleTemp = new Repository<SM_ROLESTEMP>(uow);
            repoRolePriv = new Repository<SM_ROLEPRIV>(uow);
            repoRolePrivTemp = new Repository<SM_ROLEPRIVTEMP>(uow);
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
            // GetMenuId();
            //menuId = SmartUtil.GetMenuId(m);
            //if(menuId == 0)
            //{
            //    return RedirectToAction("Error", "Home");
            //}
            try
            {
                ViewBag.MenuId = menuId;
                GetPriv();
                return View();
            }
            catch {
                return RedirectToAction("Index", "Home");
            }
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
        public ActionResult RolePriviledge()
        {
            try
            {
                //ViewBag.MenuId = menuId;
                GetPriv();
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<ActionResult> RoleList()
        {
            try
            {
                var rec = await _repo.GetRolesAsync(0, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<RolesObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ParentMenuList()
        {
            try
            {
                var rec = _repo.GetParentMenu2(null);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<ParentMenu>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<ActionResult> GetPrivilege(int RoleId)
        {
            try
            {
                var rec = await _repo.GetRolePrivilegeAsync(RoleId, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<RolePrivilegeObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePrivilege(RolesPrivObj2 model)
        {
            try
            {
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    var respMsg = "";
                    var ret = SavePriviledgesTemp(model.RoleId, model,out respMsg);
                    //var ret = _repo.PostRolePriv(model, User.Identity.Name);
                    if (ret)
                    {
                        EmailerNotification.SendForAuthorization(menuIdPriv, fullName, deptCode, institutionId, "Role Record");

                        return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });
                    }
                    else
                    {
                        return Json(new { RespCode = 0, RespMessage = "There is a problem saving this record,Try again or Contact the Administrator" });
                    }
                }

                // If we got this far, something failed, redisplay form
                return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
        }
        private bool SavePriviledgesTemp(int roleIdd, RolesPrivObj2 model,out string respMsg)
        {

            try
            {
                
                var bid = "RV_" + SmartObj.GenRefNo2();
                int cnt = 0;
                //int roleIdd = int.Parse(drpRole.SelectedValue);
                var RoleAssig = repoRolePriv.AllEager(f => f.ROLEID == roleIdd).ToList();
                foreach (var d in model.RolePrivList)
                {
                   
                    var exist = RoleAssig.FirstOrDefault(f => f.ROLEASSIGID == d.RoleAssigId);

                    if (d.CanView || d.CanAdd || d.CanEdit)
                    {

                        string evtType = eventInsert;
                        if (exist != null)
                        {
                            if (exist.CANUPDATE != d.CanEdit || exist.CANINSERT != d.CanAdd)
                            {
                                evtType = eventEdit;
                                SM_ROLEPRIVTEMP rAssign = new SM_ROLEPRIVTEMP()
                                {
                                    CANAUTHORIZE = d.CanAuthorize,
                                    CANINSERT = d.CanAdd,
                                    CANUPDATE = d.CanEdit,
                                    CANDELETE = d.CanDelete,
                                    // CanView = CanView,
                                    ROLEASSIGID = d.RoleAssigId,
                                    CREATEDATE = DateTime.Now,
                                    MENUID = d.MenuId,
                                    ROLEID = roleId,
                                    STATUS = open,
                                    USERID = User.Identity.Name,
                                    DEPARTMENT_CODE = null,
                                    BATCHID = bid,
                                    EVENTTYPE = evtType,
                                    INSTITUTION_ITBID = institutionId
                                };
                                repoRolePrivTemp.Insert(rAssign);
                                cnt++;
                            }
                        }
                        else
                        {
                            SM_ROLEPRIVTEMP rAssign = new SM_ROLEPRIVTEMP()
                            {
                                CANAUTHORIZE = d.CanAuthorize,
                                CANINSERT = d.CanAdd,
                                CANUPDATE = d.CanEdit,
                                CANDELETE = d.CanDelete,
                                // CanView = CanView,
                                ROLEASSIGID = d.RoleAssigId,
                                CREATEDATE = DateTime.Now,
                                MENUID = d.MenuId,
                                ROLEID = roleIdd,
                                STATUS = open,
                                USERID = User.Identity.Name,
                                DEPARTMENT_CODE = null,
                                BATCHID = bid,
                                EVENTTYPE = evtType,
                                INSTITUTION_ITBID = institutionId
                            };

                            repoRolePrivTemp.Insert(rAssign);
                            cnt++;
                        }




                    }
                    else
                    {
                        if (exist != null)
                        {
                            SM_ROLEPRIVTEMP rAssign = new SM_ROLEPRIVTEMP()
                            {
                                CANAUTHORIZE = d.CanAuthorize,
                                CANINSERT = d.CanAdd,
                                CANUPDATE = d.CanEdit,
                                CANDELETE = d.CanDelete,
                                // CanView = CanView,
                                ROLEASSIGID = d.RoleAssigId,
                                CREATEDATE = DateTime.Now,
                                MENUID = d.MenuId,
                                ROLEID = roleIdd,
                                STATUS = open,
                                USERID = User.Identity.Name,
                                DEPARTMENT_CODE = null,
                                BATCHID = bid,
                                EVENTTYPE = eventDelete,
                                INSTITUTION_ITBID = institutionId
                            };

                            repoRolePrivTemp.Insert(rAssign);
                            cnt++;

                        }
                    }
                }
                if (cnt > 0)
                {
                    SM_AUTHLIST auth = new SM_AUTHLIST()
                    {
                        CREATEDATE = DateTime.Now,
                        EVENTTYPE = eventEdit,
                        MENUID = menuIdPriv,
                        //MENUNAME = "",
                        POSTTYPE = Batch,
                        STATUS = open,
                        // TABLENAME = "ADMIN_DEPARTMENT",
                        URL = Request.FilePath,
                        USERID = User.Identity.Name,
                        INSTITUTION_ITBID = institutionId,
                        BATCHID = bid,

                    };
                    repoAuth.Insert(auth);

                    var rst = uow.Save(User.Identity.Name);

                    if (rst > 0)
                    {
                        respMsg = "";
                        return true;
                        //pnlResponse.Visible = true;
                        //pnlResponse.CssClass = "alert alert-success alert-bold alert-dismissable";
                        //pnlResponseMsg.Text = "<i class='fa fa-info'></i> Record Saved Successfully...Authorization Pending";
                        //pnlResponse.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                respMsg = ex.Message;
              return false;

            }
            respMsg = "Problem Processing request.";
            return false;
        }
        void BindCombo()
        {
            ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");

            ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus(), "Code", "Description");
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
        public ActionResult Create(RolesObj model,int? m)
        {
            try
            {

               // menuId = m; // SmartUtil.GetMenuId(m);
                //GetMenuId();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ROLEID > 0)
                    {
                        if (model.ROLEBASE == "1" && string.IsNullOrEmpty(model.DEPARRTMENT_CODE))
                        {
                            return Json(new { data = model, RespCode = 1, RespMessage = "Please select a department if Role is for Unified Payment" });

                        }

                        //its an update

                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        var BType = new SM_ROLESTEMP()
                        {

                            ROLENAME = model.ROLENAME,
                            DEPARRTMENT_CODE = model.DEPARRTMENT_CODE,
                            STATUS = open,
                            ROLEBASE = model.ROLEBASE,
                            INSTITUTION_ROLE = model.INSTITUTION_ROLE,
                            PARTY_ROLE = model.PARTY_ROLE,
                            RECORDID = model.ROLEID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now
                        };
                        repoRoleTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = model.STATUS == active ? eventEdit : model.STATUS,
                                MENUID = menuId,
                                //MENUNAME = "",
                                RECORDID = BType.ROLEID,
                                STATUS = open,
                                // TABLENAME = "ADMIN_DEPARTMENT",
                                URL =  Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Role Record");
                                return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });                               
                            }
                            else
                            {
                                return Json(new { RespCode = 1, RespMessage = "Problem Updating Record." });

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
                        var BType = new SM_ROLESTEMP()
                        {

                            ROLENAME = model.ROLENAME,
                            DEPARRTMENT_CODE = model.DEPARRTMENT_CODE,
                            STATUS = open,
                            ROLEBASE = model.ROLEBASE,
                            INSTITUTION_ROLE = model.INSTITUTION_ROLE,
                            PARTY_ROLE = model.PARTY_ROLE,
                            RECORDID = model.ROLEID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now
                        };
                        repoRoleTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            //var controller =  ControllerContext.Controller..ToString();
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                RECORDID = BType.ROLEID,
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
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Role Record");
                                return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });

                            }
                            else
                            {
                                return Json(new { RespCode = 1, RespMessage = "Problem Creating Record." });

                            }
                        }
                    }
                }
                // If we got this far, something failed, redisplay form
                return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
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
        public async Task<PartialViewResult> ViewRole(int id = 0,string m = null)
        {
            try
            {
                //var roleBase = new List<SelectListItem>();
                ViewBag.MenuId = m;
                //roleBase.Add(new SelectListItem { Value = "1", Text = "Roles allocated to Xpress Payment" });
                //roleBase.Add(new SelectListItem { Value = "2", Text = "General Roles" });
                BindCombo();
                var dept = await _repo.GetDepartmentAsync(0, true, "Active");
                ViewBag.Department =  new SelectList(dept, "DEPARTMENTCODE", "DEPARTMENTNAME"); 
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Role";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return PartialView("_ViewRole",new RolesObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Role";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetRolesAsync(id, false);  //repoSession.FindAsync(id);
                    if (rec == null)
                    {
                        // return Json(null, JsonRequestBehavior.AllowGet);
                        //var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                        // return Json(obj, JsonRequestBehavior.AllowGet);
                        return null;
                       

                    }
                    //  return Json(rec, JsonRequestBehavior.AllowGet);
                    //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                    // return Json(obj1, JsonRequestBehavior.AllowGet);
                    GetPriv();
                    return PartialView("_ViewRole", rec.FirstOrDefault());

                }
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                return null;
            }
        }

       
        decimal authId;
        string respMsg = null;
        public  ActionResult DetailAuth(string a_i, string m)
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
                    
                    ViewBag.HeaderTitle = "Authorize Detail for Role";
                    //ViewBag.StatusVisible = true;
                    if (det != null)
                    {
                        var batchId = det.BATCHID;

                        if (batchId == null)
                        {
                            obj.AuthId = authId;
                            obj.RecordId = det.RECORDID.GetValueOrDefault();
                            obj.BatchId = det.BATCHID;
                            obj.PostType = det.POSTTYPE;
                            obj.MenuId = det.MENUID.GetValueOrDefault();
                            ViewBag.Message = TempData["msg"];
                            var stat = ViewBag.Message != null ? null : "open";
                            var rec = _repo.GetRoles((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                            if (rec != null && rec.Count > 0)
                            {
                                var model = rec.FirstOrDefault();
                                obj.Status = det.STATUS;
                                obj.EventType = det.EVENTTYPE;
                                obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                obj.User = model.User_FullName;
                                ViewBag.Auth = obj;
                                ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);
                                var dept = _repo.GetDepartment(0, true, "Active");
                                ViewBag.Department = new SelectList(dept, "DEPARTMENTCODE", "DEPARTMENTNAME");
                                ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
                                // return null;

                                return View("DetailAuthRl", model);

                            }
                        }
                        else
                        {
                            obj.AuthId = authId;
                            obj.RecordId = det.RECORDID.GetValueOrDefault();
                            obj.BatchId = det.BATCHID;
                            obj.PostType = det.POSTTYPE;
                            obj.MenuId = det.MENUID.GetValueOrDefault();
                            ViewBag.Message = TempData["msg"];
                            var stat = ViewBag.Message != null ? null : "open";
                            var rec = _repo.GetRolePrivilegeTemp(det.BATCHID,det.USERID);  //repoSession.FindAsync(id);
                            if (rec != null && rec.Count > 0)
                            {

                                var model = rec.FirstOrDefault();
                                ViewBag.RoleName = model.RoleName;
                                obj.Status = det.STATUS;
                                obj.EventType = det.EVENTTYPE;
                                obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                obj.User = model.CREATEDBY;
                                ViewBag.Auth = obj;
                                ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);
                               // var dept = _repo.GetDepartment(0, true, "Active");
                                //ViewBag.Department = new SelectList(dept, "DEPARTMENTCODE", "DEPARTMENTNAME");
                                //ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
                                // return null;

                                return View("DetailAuthPriv", rec);

                            }
                        }
                        //  return Json(rec, JsonRequestBehavior.AllowGet);
                        //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                        // return Json(obj1, JsonRequestBehavior.AllowGet);
                    }


                    return View("DetailAuthRl");
                }
                else
                {
                    return View("Error", "Home");
                }

            }
            catch (Exception ex)
            {
                return View("DetailAuthRl");
            }
       
        }
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(decimal AuthId,int? m)
        {
            var sucSave = false;
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
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
                                // rec2.STATUS = close;

                                if (suc)
                                {
                                    rec2.STATUS = approve;
                                    var t = uow.Save(rec2.USERID,User.Identity.Name);
                                    if (t > 0)
                                    {
                                        sucSave = true;
                                        //txscope.Complete();
                                    }
                                }
                                else
                                {
                                    //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                    respMsg = "Problem processing request. Try again or contact Administrator.";
                                    // TempData["msg"] = respMsg;
                                    // return RedirectToAction("DetailAuthRl", new {  a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
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
                  
                //}
                if(sucSave)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", null, fullName);
               
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
               // return RedirectToAction("DetailAuthRl", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool CloseMainRecord(int recordId)
        {
            var rec = repoRoleTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoRole.Find(rec.RECORDID);
                if(obj != null)
                {
                    obj.STATUS = close;
                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePriv(decimal AuthId, int? m)
        {
            var sucSave = false;
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
               // int recordId = 0;
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
                                //recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "Modify":
                                        {
                                            suc = PostBulkUpload(rec2.BATCHID,rec2.USERID);
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
                                    var t = uow.Save(User.Identity.Name);
                                    if (t > 0)
                                    {
                                        sucSave = true;
                                       
                                        //txscope.Complete();
                                       
                                    }
                                }
                                else
                                {
                                    //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                    respMsg = "Problem processing request. Try again or contact Administrator.";
                                    // TempData["msg"] = respMsg;
                                    // return RedirectToAction("DetailAuthRl", new {  a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
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
                   
                //}
                if(sucSave)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Privilege Record", null, fullName);
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
                // return RedirectToAction("DetailAuthRl", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool PostBulkUpload(string batchId,string user_id)
        {
            var RoleAssig2 = repoRolePrivTemp.AllEager(f => f.BATCHID == batchId && f.USERID == user_id
             && f.STATUS != null && f.STATUS.ToLower() == open.ToLower()).ToList();

            foreach (var d in RoleAssig2)
            {
                d.STATUS = approve;
                SM_ROLEPRIV rA = null;
                if (d.ROLEASSIGID != 0)
                {
                    rA = repoRolePriv.Find(d.ROLEASSIGID);
                }
                if (d.EVENTTYPE == eventInsert  || d.EVENTTYPE == eventEdit)
                {
                    if (rA != null)
                    {
                        // rA.ca = CanView;
                        rA.CANUPDATE = d.CANUPDATE;
                        rA.CANDELETE = d.CANDELETE;
                        rA.CANAUTHORIZE = d.CANAUTHORIZE;
                        rA.CANINSERT = d.CANINSERT;
                    }
                    else
                    {
                        SM_ROLEPRIV rAssign = new SM_ROLEPRIV()
                        {
                            CANAUTHORIZE = d.CANAUTHORIZE,
                            CANINSERT = d.CANINSERT,
                            CANUPDATE = d.CANUPDATE,
                            CANDELETE = d.CANDELETE,
                            CREATEDATE = DateTime.Now,
                            MENUID = d.MENUID,
                            ROLEID = d.ROLEID,
                            STATUS = "Active",
                            USERID = d.USERID,
                            DEPARTMENT_CODE = null,
                            INSTITUTION_ITBID = d.INSTITUTION_ITBID
                        };

                        repoRolePriv.Insert(rAssign);
                    }

                }
                else
                {
                    if (rA != null)
                    {
                        repoRolePriv.Delete(d.ROLEASSIGID);
                    }
                }
            }
            return true;
        }

        private bool CreateMainRecord (int recordId)
        {
            var rec = repoRoleTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = new SM_ROLES()
                {
                    CREATEDATE = rec.CREATEDATE,
                    DEPARRTMENT_CODE = rec.DEPARRTMENT_CODE,
                    INSTITUTION_ROLE = rec.INSTITUTION_ROLE,
                    IS_INTERNAL = rec.IS_INTERNAL,
                    ROLEBASE = rec.ROLEBASE,
                    ROLENAME = rec.ROLENAME,
                    STATUS = active,
                    USERID = rec.USERID,
                    //LAST_MODIFIED_UID = User.Identity.Name,
                    LAST_MODIFIED_AUTHID = User.Identity.Name,
                    LAST_MODIFIED_DATE = DateTime.Now
                };
                repoRole.Insert(obj);
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId)
        {
            var rec = repoRoleTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoRole.Find(rec.RECORDID);
                if (obj != null)
                {

                        obj.CREATEDATE = rec.CREATEDATE;
                        obj.DEPARRTMENT_CODE = rec.DEPARRTMENT_CODE;
                        obj.INSTITUTION_ROLE = rec.INSTITUTION_ROLE;
                        obj.IS_INTERNAL = rec.IS_INTERNAL;
                        obj.ROLEBASE = rec.ROLEBASE;
                        obj.ROLENAME = rec.ROLENAME;
                        //STATUS = active,
                       // USERID = rec.USERID,
                       obj.LAST_MODIFIED_UID = rec.USERID;
                       obj.LAST_MODIFIED_AUTHID = User.Identity.Name;
                       obj.LAST_MODIFIED_DATE = DateTime.Now;
                       obj.STATUS = active;

               // repoRole.Insert(obj);
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
            bool suc = false;
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });

                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
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
                                menuId = rec2.MENUID.GetValueOrDefault();
                                var recc = repoRoleTemp.Find(recordId);
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

                            //if (!isApprove)
                            //{
                            //    pnlResponse.Visible = true;
                            //    pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold fade in";

                            //    pnlResponseMsg.Text = "Record Successfully Approved";
                            //}
                        }
                    }
                   
               // }
                if(suc)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", Narration, fullName);
                    //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    // TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }
                return Json(new { RespCode = 1, RespMessage = respMsg });


            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });

            }
        }

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RejectPriv(decimal AuthId, int? m, string Narration)
        {
            bool suc = false;
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });

                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                //int recordId = 0;
                // bool suc = false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 5, 0)))
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
                                var recc = repoRolePrivTemp.AllEager(f => f.BATCHID == rec2.BATCHID).ToList(); //.Find(recordId);
                                foreach (var p in recc)
                                {
                                    p.STATUS = reject;
                                }

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    suc = true;
                                   // txscope.Complete();


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

                //}
                if (suc)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Privilege Record", Narration, fullName);
                    //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    // TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }
                return Json(new { RespCode = 1, RespMessage = respMsg });


            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuthRl", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });

            }
        }

        public List<RoleBaseObj> GetRoleBase()
        {
            var rleBase = new List<RoleBaseObj>();
            rleBase.Add(new RoleBaseObj()
            {
                Code = "1",
                Description = "Roles allocated to Xpress Payment",
            });
            rleBase.Add(new RoleBaseObj()
            {
                Code = "2",
                Description = "General Roles",
            });
            return rleBase;
        }
    }

    public class RoleBaseObj
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}