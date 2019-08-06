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
    public class ApprovalRouteController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;

        //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_APPROVAL_ROUTE> repoAppRoute = null;
        private readonly IRepository<SM_APPROVAL_ROUTE_TEMP> repoAppRouteTemp = null;
        private readonly IRepository<SM_APPROVAL_ROUTE_OFFICER> repoAppRouteOff = null;
        private readonly IRepository<SM_APPROVAL_ROUTE_OFFICER_TEMP> repoAppRouteOffTemp = null;
        
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
        int menuId = 41;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public ApprovalRouteController()
        {
            uow = new UnitOfWork();
            //repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoAppRoute = new Repository<SM_APPROVAL_ROUTE>(uow);
            repoAppRouteTemp = new Repository<SM_APPROVAL_ROUTE_TEMP>(uow);
            repoAppRouteOff = new Repository<SM_APPROVAL_ROUTE_OFFICER>(uow);
            repoAppRouteOffTemp = new Repository<SM_APPROVAL_ROUTE_OFFICER_TEMP>(uow);
           
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
        // GET: ApprovalRoute
        public ActionResult Index()
        {
            try
            {
                GetPriv();
                BindCombo();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<ActionResult> ApproverList(int id)
        {
            try
            {
                var rv = new AppRouteSession();
                rv.PurgeRouteOfficer(User.Identity.Name);
                var no_level = 0;
                var rec = await _repo.GetMenuApproverOfficersAsync(id);
                if (rec.Count > 0)
                {
                     no_level = rec[0].NOLEVEL;//repoSession.FindAsync(id); 
                   
                }
                var html = PartialView("_ViewApproverLocal", rec).RenderToString();         
                return Json(new { data_html = html,data_level = no_level, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new {RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }

        void BindCombo()
        {
            var mList = _repo.GetMenuApprover();
            ViewBag.MenuList = new SelectList(mList, "MENUID", "MENUNAME");
            
            //var sta = SmartObj.GetStatus();
            //ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");
           
        }
        void BindCombo2()
        {
            var mList = _repo.GetUser(0,true,false,"Active");
            ViewBag.UserList = new SelectList(mList, "UserName", "FullName");

            //var sta = SmartObj.GetStatus();
            //ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

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
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int MenuId,int NoLevel, string m)
        {
            try
            {
               
                string bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                   
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_APPROVAL_ROUTE_TEMP BType = new SM_APPROVAL_ROUTE_TEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                MENUID = MenuId,
                                NOLEVEL = NoLevel,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                //RECORDID = model.ITBID,
                                
                            };

                            repoAppRouteTemp.Insert(BType);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                SaveRouteOfficerTemp(eventEdit,bid);

                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = eventEdit,
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
                                  
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Approval Route Record");

                                    //txscope.Complete();
                                    //TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                    return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    //return RedirectToAction("Index", new { m = m });
                                }
                            }
                        //}
                    
                    
                    // If we got this far, something failed, redisplay form
                    return Json(new { RespCode = 1, RespMessage = errorMsg });
                  
                }
            }
            catch (SqlException ex)
            {
                //BindCombo();
                //ViewBag.PartyAcct = GetRvHeadLines();
                //ViewBag.Message = ex.Message;
                //return View("Add", model);
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                //BindCombo();
                //ViewBag.PartyAcct = GetRvHeadLines();
                //ViewBag.Message = ex.Message;
                //return View("Add", model);
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            //BindCombo();
            //ViewBag.PartyAcct = GetRvHeadLines();
            //ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            //return View("Add", model);
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }
        void SaveRouteOfficerTemp(string eventType,string batchId)
        {
            var rv = new AppRouteSession();
            //if (eventType == "New")
            //{
                var col = rv.GetRouteOfficer(User.Identity.Name); // GetRvHeadLines();
                foreach (var d in col)
                {
                    var obj = new SM_APPROVAL_ROUTE_OFFICER_TEMP()
                    {
                        MENUID = d.MENUID, //drpCalcBasis.SelectedValue,
                        BATCHID = batchId,
                        APPROVER_ID = d.APPROVER_ID,
                        PRIORITY = d.PRIORITY,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = d.EVENTTYPE,
                    };
                    repoAppRouteOffTemp.Insert(obj);
                }

            //}
            //else
            //{
            //    var col = rv.GetRevenueHead(User.Identity.Name); // GetRvHeadLines();
            //    foreach (var d in col)
            //    {

            //        var obj = new SM_REVENUEHEADTEMP()
            //        {
            //            CODE = d.CODE, //drpCalcBasis.SelectedValue,
            //            RVGROUPCODE = rec.GROUPCODE,
            //            DESCRIPTION = d.DESCRIPTION,
            //            ACCOUNT_ID = d.ACCOUNT_ID,
            //            MERCHANTID = rec.MERCHANTID,
            //            STATUS = open,
            //            RECORDID = d.NewRecord ? 0 : d.ITBID,
            //            CREATEDATE = DateTime.Now,
            //            USERID = User.Identity.Name,
            //            EVENTTYPE = d.EVENTTYPE, // ? eventInsert : d.Deleted ? eventDelete : eventEdit,
            //            BATCHID = rec.BATCHID,
            //            SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY,
            //        };
            //        repoRvHeadTemp.Insert(obj);
            //    }
            //}
        }
        #region Approver 
        void BindComboMsc()
        {
            var userList = _repo.GetUser(0, true, false, "Active");
            ViewBag.UserList = new SelectList(userList, "UserName", "FullName");
        }
        public PartialViewResult AddRouteOfficer(int menuid)
        {
            try
            {

                BindComboMsc();
                ViewBag.HeaderTitle = "Add Approver User";
                ViewBag.ButtonText = "Add";
                var obj = new ApprovalRouteOffObj() { MENUID = menuid };
                return PartialView("_AddRouteOfficer", obj);
                // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddRouteOfficer(ApprovalRouteOffObj model, string m)
        {
            var rv = new AppRouteSession();
            string msg = "";
            try
            {
                //var lst = GetRvHeadLines().ToList();
                if (string.IsNullOrEmpty(model.PID) &&  model.DB_ITBID.GetValueOrDefault() <= 0)
                {
                   
                    var obj = new ApprovalRouteOffObj()
                    {
                        APPROVER_ID = model.APPROVER_ID,
                        MENUID = model.MENUID,
                        PRIORITY = model.PRIORITY,
                        PID = model.PID,
                        EVENTTYPE = eventInsert,
                        USERID = User.Identity.Name
                    };
                    //SessionHelper.GetRvHead(Session).AddItem(obj);
                    var rst = rv.PostRouteOfficer(obj, 1);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    var w = rv.GetRouteOfficer(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewRouteOfficer", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                   
                    var obj = new ApprovalRouteOffObj()
                    {
                        APPROVER_ID = model.APPROVER_ID,
                        MENUID = model.MENUID,
                        PRIORITY = model.PRIORITY,
                        USERID = User.Identity.Name,
                        DB_ITBID = model.DB_ITBID,
                        PID = model.PID,
                        EVENTTYPE = model.DB_ITBID > 0 ? eventEdit : eventInsert,
                    };
                    OutPutObj rst;
                    rst = rv.PostRouteOfficer(obj, 2);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    //SessionHelper.GetRvHead(Session).UpdateItem(obj);
                    // rv.PostRevenueHead(obj, 2);
                    var w = rv.GetRouteOfficer(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewRouteOfficer", w).RenderToString();
                    msg = "Record Updated to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }
        }
       
        public ActionResult EditRouteOfficer(string id)
        {
            try
            {

                var rv = new AppRouteSession();
                var rec = rv.FindRouteOfficer(id, User.Identity.Name);
                if (rec != null)
                {
                    BindComboMsc();
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Route Officer";
                    return PartialView("_AddRouteOfficer", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ActionResult EditRouteOfficerLocal(int id)
        {
            try
            {
                var rec = _repo.GetRouteOfficer(id);
                if (rec != null)
                {
                    BindComboMsc();
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Route Officer";
                    return PartialView("_AddRouteOfficer", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ActionResult DeleteRouteOfficer(string id, string m)
        {
            var rv = new AppRouteSession();
            try
            {
                rv.DeleteRouteOfficer(id, User.Identity.Name);

                var lst2 = rv.GetRouteOfficer(User.Identity.Name); // GetRvHeadLines().ToList();
                var html2 = PartialView("_ViewRouteOfficer", lst2).RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);

            }
            catch(Exception ex)
            {
                var lst3 = rv.GetRouteOfficer(User.Identity.Name); //  GetRvHeadLines().ToList();
                var html = PartialView("_ViewRouteOfficer", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult DeleteRouteOfficerLocal(int id)
        {
            string msg = "";
            var rv = new AppRouteSession();
            var model = _repo.GetRouteOfficer(id);
            if (model == null)
            {
                msg = "Bad Request";
                return Json(new { RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }
            var obj = new ApprovalRouteOffObj()
            {
                APPROVER_ID = model.APPROVER_ID,
                MENUID = model.MENUID,
                PRIORITY = model.PRIORITY,
                USERID = User.Identity.Name,
                DB_ITBID = model.DB_ITBID,
                PID = model.PID,
                EVENTTYPE = eventDelete,
            };
            OutPutObj rst;
            rst = rv.PostRouteOfficer(obj, 2);
            if (rst != null && rst.RespCode != 0)
            {
                msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }
            //SessionHelper.GetRvHead(Session).UpdateItem(obj);
            // rv.PostRevenueHead(obj, 2);
            var w = rv.GetRouteOfficer(User.Identity.Name); // GetRvHeadLines().ToList();
            var html = PartialView("_ViewRouteOfficer", w).RenderToString();
            msg = "Record Updated to List";
            return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

        }
        #endregion Approver

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

                    ViewBag.HeaderTitle = "Authorize Detail for Approval Route";
                    //ViewBag.StatusVisible = true;
                    if (det != null)
                    {
                        obj.AuthId = authId;
                        obj.RecordId = det.RECORDID.GetValueOrDefault();
                        obj.BatchId = det.BATCHID;
                        obj.PostType = det.POSTTYPE;
                        obj.MenuId = det.MENUID.GetValueOrDefault();
                        ViewBag.Message = TempData["msg"];
                        var status = TempData["status"];
                        var stat = status == null ? "open" : status.ToString();
                        var rec = _repo.GetAppRouteTemp((int)det.RECORDID);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            ViewBag.AppRouteOfficerList = _repo.GetAppRouteOfficerTemp(det.BATCHID, model.USERID);
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.CREATED_BY;
                            ViewBag.Auth = obj;
                            ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);

                            BindCombo();
                            //ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
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
                    //bad request
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
                                    
                                    case "Modify":
                                        {
                                            suc = ModifyMainRecord(recordId, rec2.BATCHID, rec2.USERID);
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
                                        sucNew = true;
                                        //txscope.Complete();

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

                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Approval Route Record", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    //TempData["msg"] = respMsg;
                    //TempData["status"] = approve;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                respMsg = "This request has already been processed by an authorizer.";
                //TempData["msg"] = respMsg;
                //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
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
        private bool ModifyMainRecord(int recordId, string batchId, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoAppRouteTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoAppRoute.AllEager(e => e.MENUID == dt.MENUID).FirstOrDefault();
                dt.STATUS = approve;

                if (dm != null)
                {
                    dm.NOLEVEL = dt.NOLEVEL;
                    dm.STATUS = active;
                    var ac = repoAppRouteOffTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID == user_id).ToList();
                    if (ac.Count > 0)
                    {
                        foreach (var d in ac)
                        {
                            d.STATUS = approve;
                            if (d.EVENTTYPE == eventInsert)
                            {
                              
                                    var dm2 = repoAppRouteOff.AllEager(e => e.MENUID == d.MENUID && e.APPROVER_ID == d.APPROVER_ID).FirstOrDefault();
                                if (dm2 == null)
                                {
                                    var obj2 = new SM_APPROVAL_ROUTE_OFFICER()
                                    {
                                        APPROVER_ID = d.APPROVER_ID,
                                        MENUID = d.MENUID,          //BATCHID = batchId,
                                        PRIORITY = d.PRIORITY,
                                        //STATUS = active,
                                        CREATEDATE = DateTime.Now,
                                        USERID = user_id,
                                    };
                                    repoAppRouteOff.Insert(obj2);

                                }
                           
                            }
                            else
                            {
                                var dm2 = repoAppRouteOff.AllEager(g=> g.MENUID == d.MENUID && g.APPROVER_ID == d.APPROVER_ID ).FirstOrDefault();
                                if (dm2 != null)
                                {
                                    repoAppRouteOff.Delete(dm2.ITBID);
                                }
                            }
                        }
                    }
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
                                recordId = (int)rec2.RECORDID.GetValueOrDefault();
                                menuId = rec2.MENUID.GetValueOrDefault();
                                RejectBatch(recordId, rec2.BATCHID, rec2.USERID);
                                

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    sucNew = true;
                                    //txscope.Complete();

                                }

                            }
                        }
                    }
                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Approval Route Rejection", Narration, fullName);
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
                TempData["msg"] = respMsg;
                //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
        }

        private void RejectBatch(decimal? rECORDID, string bATCHID, string uSERID)
        {
            var recP = repoAppRouteTemp.AllEager(d => d.ITBID == rECORDID && d.USERID == uSERID).FirstOrDefault();
            if (recP != null)
            {
                recP.STATUS = reject;
            }

            var recPP = repoAppRouteOffTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
            foreach (var d in recPP)
            {
                d.STATUS = reject;
            }

        }

    }
}