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

namespace SettlementMaster.App.Controllers
{
    public class ServiceChannelController : Controller
    {

        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_SERVICECHANNELS> repoServiceChannel = null;
        private readonly IRepository<SM_SERVICECHANNELSTEMP> repoServiceChannelTemp = null;
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
        int menuId = 14;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string deptCode; string fullName;
        // GET: Roles
        public ServiceChannelController()
        {
            uow = new UnitOfWork();
            repoServiceChannel = new Repository<SM_SERVICECHANNELS>(uow);
            repoServiceChannelTemp = new Repository<SM_SERVICECHANNELSTEMP>(uow);
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
                BindCombo();
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

        void BindCombo()
        {
            var ChannelList = _repo.GetChannel(0, true, "Active");
                  ViewBag.Channel = new SelectList(ChannelList, "CODE", "DESCRIPTION");

            var BankCodeList = _repo.GetInstitution(0, true, "Active");
            ViewBag.Bank = new SelectList(BankCodeList, "CBN_CODE", "INSTITUTION_NAME");
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
        public async Task<ActionResult> ServiceChannelList()
        {
            try
            {
                BindCombo();
                var rec = await _repo.GetServiceChannelAsync(0, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<SERVICEChannelObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> Add(int id = 0, string m = null)
        {
            try
            {
                BindCombo();
                ViewBag.MenuId = HttpUtility.UrlDecode(m);

                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add ServiceChannel";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add", new SERVICEChannelObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit ServiceChannel";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetServiceChannelAsync (id, false);
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
        public ActionResult Add(SERVICEChannelObj model, string m)
        {
            try
            {
                menuId = SmartUtil.GetMenuId(m);
                ViewBag.MenuId = HttpUtility.UrlDecode(m);

                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID == 0)
                    {
                        ViewBag.HeaderTitle = "Add ServiceChannel";
                        ViewBag.StatusVisible = false;
                        ViewBag.ButtonText = "Save";
                    }
                    else
                    {
                        //var d = _repo.GetSession(0, true);
                        ViewBag.HeaderTitle = "Edit ServiceChannel";
                        ViewBag.StatusVisible = true;
                        ViewBag.ButtonText = "Update";
                    }
                    if (model.ITBID > 0)
                    {

                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        var BType = new SM_SERVICECHANNELSTEMP()
                        {
                            CODE = model.CODE,
                            DESCRIPTION = model.DESCRIPTION,
                            ChannelID=model.ChannelID,
                            BankCode = model.BankCode,
                            BankAccount = model.BankAccount,
                            STATUS = open,
                            RECORDID = model.ITBID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now
                        };
                        repoServiceChannelTemp.Insert(BType);
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
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "ServiceChannel Record");
                                TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return RedirectToAction("Index", "ServiceChannel", new { m = m });
                                //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });                         
                            }
                            else
                            {
                                ViewBag.Message = "Problem Updating Record.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return View("Add", model);
                                //return Json(new { RespCode = 1, RespMessage = "Problem Updating Record." });

                            }
                        }
                    }
                    else
                    {
                        var BType = new SM_SERVICECHANNELSTEMP()
                        {
                            CODE = model.CODE,
                            DESCRIPTION = model.DESCRIPTION,
                            STATUS = open,
                            RECORDID = model.ITBID,
                            ChannelID = model.ChannelID,
                            BankCode = model.BankCode,
                            BankAccount = model.BankAccount,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now
                        };
                        repoServiceChannelTemp.Insert(BType);
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
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "ServiceChannel Record");
                                TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return RedirectToAction("Index", new { m = m });

                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });

                            }
                            else
                            {
                                ViewBag.Message = "Problem Updating Record.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return View("Add", model);

                                // return Json(new { RespCode = 1, RespMessage = "Problem Creating Record." });
                            }
                        }
                    }
                }
                // If we got this far, something failed, redisplay form
                //errorMsg = GetModelError();
                errorMsg = ModelStateErrorHandler.StringifyModelErrors(ModelState);
                ViewBag.Message = errorMsg;
                return View("Add", model);
            }
            catch (SqlException ex)
            {
                ViewBag.Message = "Problem Updating Record.";
                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                return View("Add", model);

                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Problem Updating Record.";
                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
        }

        string GetModelError()
        {
            var sb = new StringBuilder();
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    sb.AppendLine(error.ErrorMessage);
                }
            }
            return sb.ToString();
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
        public async Task<PartialViewResult> ViewRole(int id = 0, string m = null)
        {
            try
            {
                var roleBase = new List<SelectListItem>();
                ViewBag.MenuId = m;
                roleBase.Add(new SelectListItem { Value = "1", Text = "Roles allocated to Xpress Payment" });
                roleBase.Add(new SelectListItem { Value = "2", Text = "General Roles" });
                var dept = await _repo.GetDepartmentAsync(0, true, "Active");
                ViewBag.Department = new SelectList(dept, "DEPARTMENTCODE", "DEPARTMENTNAME");
                ViewBag.BaseRole = new SelectList(roleBase, "Value", "Text");
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Role";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    return PartialView("_ViewRole", new RolesObj());

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
        public ActionResult DetailAuth(string a_i, string m)
        {
            try
            {
                BindCombo();
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
                        obj.AuthId = authId;
                        obj.RecordId = det.RECORDID.GetValueOrDefault();
                        obj.BatchId = det.BATCHID;
                        obj.PostType = det.POSTTYPE;
                        obj.MenuId = det.MENUID.GetValueOrDefault();
                        ViewBag.Message = TempData["msg"];
                        var stat = ViewBag.Message != null ? null : "open";
                        var rec = _repo.GetServiceChannel((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
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

        bool sucnew = false;
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(decimal AuthId, int? m)
        {
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
                            // rec2.STATUS = close;

                            if (suc)
                            {
                                rec2.STATUS = approve;
                                var t = uow.Save(rec2.USERID, User.Identity.Name);
                                if (t > 0)
                                {
                                    sucnew = true;
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

                        //if (!isApprove)
                        //{
                        //    pnlResponse.Visible = true;
                        //    pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold fade in";

                        //    pnlResponseMsg.Text = "Record Successfully Approved";
                        //}
                    }
                }

                //}
                if (sucnew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "ServiceChannel Approval", null, fullName);
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

        private bool CloseMainRecord(int recordId)
        {
            var rec = repoServiceChannelTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoServiceChannel.Find((int)rec.RECORDID);
                if (obj != null)
                {
                    obj.STATUS = close;
                    return true;
                }
            }
            return false;
        }

        private bool CreateMainRecord(int recordId)
        {
            var rec = repoServiceChannelTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var sig = repoServiceChannel.AllEager(d => d.CODE == rec.CODE).FirstOrDefault();
                if (sig == null)
                {
                    var obj = new SM_SERVICECHANNELS()
                    {
                        CREATEDATE = rec.CREATEDATE,
                        CODE = rec.CODE,
                        DESCRIPTION = rec.DESCRIPTION,
                        BankCode =rec.BankCode,
                        BankAccount =rec.BankAccount ,
                        STATUS = active,
                        USERID = rec.USERID,
                        //LAST_MODIFIED_AUTHID = User.Identity.Name,
                        //LAST_MODIFIED_DATE = DateTime.Now
                    };
                    repoServiceChannel.Insert(obj);
                    return true;
                }
                else
                {
                    sig.DESCRIPTION = rec.DESCRIPTION;
                    sig.BankCode = rec.BankCode;
                    sig.BankAccount = rec.BankAccount;
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
            var rec = repoServiceChannelTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoServiceChannel.Find((int)rec.RECORDID);
                if (obj != null)
                {
                    //obj.CREATEDATE = rec.CREATEDATE;
                    obj.CODE = rec.CODE;
                    obj.DESCRIPTION = rec.DESCRIPTION;
                    obj.BankCode = rec.BankCode;
                    obj.BankAccount = rec.BankAccount;
                    obj.LAST_MODIFIED_UID = rec.USERID;
                    obj.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    obj.LAST_MODIFIED_DATE = DateTime.Now;
                    obj.STATUS = active;

                    // repoServiceChannel.Insert(obj);
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
                            menuId = rec2.MENUID.GetValueOrDefault();
                            var recc = repoServiceChannelTemp.Find(recordId);
                            if (recc != null)
                            {
                                recc.STATUS = reject;
                            }

                            rec2.STATUS = reject;
                            var t = uow.Save(User.Identity.Name);
                            if (t > 0)
                            {
                                sucnew = true;
                                //txscope.Complete();
                            }
                        }
                    }
                }
                //}
                if (sucnew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "ServiceChannel Rejection", Narration, fullName);
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }
                respMsg = "This request has already been processed by an authorizer.";

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