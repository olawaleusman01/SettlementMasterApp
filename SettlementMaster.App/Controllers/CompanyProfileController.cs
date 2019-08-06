using System;
using Generic.Dapper.Data;
using Generic.Dapper.Model;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Data;
using SettlementMaster.App.Models;
using System.Transactions;
using Generic.Dapper.Utility;
using Generic.Data.Utilities;

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class CompanyProfileController : Controller
    {
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
            private readonly IRepository<SM_COMPANY_PROFILE> repoComp = null;
            private readonly IRepository<SM_COMPANY_PROFILETEMP> repoCompTemp = null;
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
            int menuId = 13;
        int menuIdPol = 27;
        int institutionId;
            int roleId;
            int checkerNo = 1;
        string fullName;string deptCode;
        // GET: Roles
        public CompanyProfileController()
        {
            uow = new UnitOfWork();
            repoComp = new Repository<SM_COMPANY_PROFILE>(uow);
            repoCompTemp = new Repository<SM_COMPANY_PROFILETEMP>(uow);
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
        public async Task<ActionResult> Index(string m)
        {
            //ViewBag.RoleId = roleId;
            //ViewBag.MenuId = menuId;
            try
            {
               
                // GetMenuId();
                menuId = SmartUtil.GetMenuId(m);
                if (menuId == 0)
                {
                    return RedirectToAction("Error", "Home");
                }
                ViewBag.MenuId = menuId;
                var rec = await _repo.GetCompanyProfileAsync(0, false);
                var country = await _repo.GetCountryAsync(0, true, "Active");
                ViewBag.Country = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
                var state = await _repo.GetStateAsync(0, true, "Active", countryCode: rec.COMPANY_COUNTRY);
                ViewBag.State = new SelectList(state, "STATECODE", "STATENAME");
                var banks = await _repo.GetInstitutionAsync(0, true, "Active");
                ViewBag.BankList = new SelectList(banks.Where(d=> d.IS_BANK != null && d.IS_BANK.ToLower() == "y").ToList() , "CBN_CODE", "INSTITUTION_NAME");
                BindCity(rec.COMPANY_COUNTRY, rec.COMPANY_STATE);
                GetPriv();
                return View(rec);
            }
            catch
            {
                return RedirectToAction("Index","Home");
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
        void BindCity(string countryCode, string stateCode)
        {
            try
            {
                var city = _repo.GetCityFilter(countryCode, stateCode);

                ViewBag.CityList = new SelectList(city, "CITYCODE", "CITYNAME");
            }
            catch
            {

            }
        }
        public async Task<ActionResult> SPolicy(string m)
        {
            //ViewBag.RoleId = roleId;
            //ViewBag.MenuId = menuId;
            try
            {

                // GetMenuId();
                menuId = SmartUtil.GetMenuId(m);
                if (menuId == 0)
                {
                    return RedirectToAction("Error", "Home");
                }
                ViewBag.MenuId = menuId;
                var rec = await _repo.GetCompanyProfileAsync(0, false);
                GetPriv();
                return View(rec);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(CompanyProfileObj model, int? m)
            {
                try
                {
                    var errorMsg = "";
                    if (ModelState.IsValid)
                    {
                    if (model.ITBID > 0)
                    {

                        var BType = new SM_COMPANY_PROFILETEMP()
                        {
                            COMPANY_ADDRESS = model.COMPANY_ADDRESS,
                            COMPANY_CITY = model.COMPANY_CITY,
                            COMPANY_CODE = model.COMPANY_CODE,
                            COMPANY_COUNTRY = model.COMPANY_COUNTRY,
                            COMPANY_EMAIL = model.COMPANY_EMAIL,
                            COMPANY_NAME = model.COMPANY_NAME,
                            COMPANY_PHONE1 = model.COMPANY_PHONE1,
                            COMPANY_PHONE2 = model.COMPANY_PHONE2,
                            COMPANY_STATE = model.COMPANY_STATE,
                            COMPANY_WEBSITE = model.COMPANY_WEBSITE,
                            STATUS = open,
                            RECORDID = model.ITBID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now,
                            LOCKOUT_TRIAL_COUNT = model.LOCKOUT_TRIAL_COUNT,
                            PASSWORDLENGTH = model.PASSWORDLENGTH,
                            PASSWORD_CHANGE_DAYS = model.PASSWORD_CHANGE_DAYS,
                            SYSTEM_IDLE_TIMEOUT = model.SYSTEM_IDLE_TIMEOUT,
                            WHTACCOUNT = model.WHTACCOUNT,
                            WHTAXRATE = model.WHTAXRATE,
                            WHTBANKCODE = model.WHTBANKCODE,
                            NIBSSACCOUNT = model.NIBSSACCOUNT,
                            NIBSSBANKCODE = model.NIBSSBANKCODE,
                            NIBSSRATE = model.NIBSSRATE,
                        };
                        //BType = model;
                        //        BType.RECORDID = model.ITBID;
                        //        BType.USERID = model.USERID;
                        //        BType.CREATEDATE = model.CREATEDATE;
                        //        BType.STATUS = open;
                        //        BType.ITBID = 0;
                        repoCompTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventEdit, // model.STATUS == active ? eventEdit : model.STATUS,
                                MENUID = m,
                                //MENUNAME = "",
                                RECORDID = BType.ITBID,
                                STATUS = open,
                                // TABLENAME = "ADMIN_DEPARTMENT",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single,
                                TABLENAME = "COMP"
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Department Record");
                                return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });
                            }
                        }
                        return Json(new { RespCode = 1, RespMessage = "Problem Updating Record." });

                    }
                    else
                    {
                        var BType = new SM_COMPANY_PROFILETEMP();

                        BType = model;
                        BType.RECORDID = model.ITBID;
                        BType.USERID = model.USERID;
                        BType.CREATEDATE = model.CREATEDATE;
                        BType.STATUS = open;
                        BType.ITBID = 0;
                        repoCompTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            //var controller =  ControllerContext.Controller..ToString();
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = m,
                                //MENUNAME = "",
                                RECORDID = BType.ITBID,
                                STATUS = open,
                                // TABLENAME = "ADMIN_DEPARTMENT",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single,
                                TABLENAME = "COMP"
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                //newR = 1;
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Department Record");
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
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddPolicy(CompanyProfileObj model, int? m)
        {
            try
            {

                //menuId = SmartUtil.GetMenuId(m);
                //GetMenuId();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID > 0)
                    {

                        var BType = new SM_COMPANY_PROFILETEMP()
                        {
                            COMPANY_ADDRESS = model.COMPANY_ADDRESS,
                            COMPANY_CITY = model.COMPANY_CITY,
                            COMPANY_CODE = model.COMPANY_CODE,
                            COMPANY_COUNTRY = model.COMPANY_COUNTRY,
                            COMPANY_EMAIL = model.COMPANY_EMAIL,
                            COMPANY_NAME = model.COMPANY_NAME,
                            COMPANY_PHONE1 = model.COMPANY_PHONE1,
                            COMPANY_PHONE2 = model.COMPANY_PHONE2,
                            COMPANY_STATE = model.COMPANY_STATE,
                            COMPANY_WEBSITE = model.COMPANY_WEBSITE,
                            STATUS = open,
                            RECORDID = model.ITBID,
                            USERID = User.Identity.Name,
                            CREATEDATE = DateTime.Now,
                            LOCKOUT_TRIAL_COUNT = model.LOCKOUT_TRIAL_COUNT,
                            PASSWORDLENGTH = model.PASSWORDLENGTH,
                            PASSWORD_CHANGE_DAYS = model.PASSWORD_CHANGE_DAYS,
                            SYSTEM_IDLE_TIMEOUT = model.SYSTEM_IDLE_TIMEOUT
                        };
                        //BType = model;
                        //        BType.RECORDID = model.ITBID;
                        //        BType.USERID = model.USERID;
                        //        BType.CREATEDATE = model.CREATEDATE;
                        //        BType.STATUS = open;
                        //        BType.ITBID = 0;
                        repoCompTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventEdit, // model.STATUS == active ? eventEdit : model.STATUS,
                                MENUID = m,
                                //MENUNAME = "",
                                RECORDID = BType.ITBID,
                                STATUS = open,
                                // TABLENAME = "ADMIN_DEPARTMENT",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single,
                                //TABLENAME = "COMP"
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Department Record");
                                return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });
                            }
                        }
                        return Json(new { RespCode = 1, RespMessage = "Problem Updating Record." });

                    }
                    else
                    {
                        var BType = new SM_COMPANY_PROFILETEMP();

                        BType = model;
                        BType.RECORDID = model.ITBID;
                        BType.USERID = model.USERID;
                        BType.CREATEDATE = model.CREATEDATE;
                        BType.STATUS = open;
                        BType.ITBID = 0;
                        repoCompTemp.Insert(BType);
                        if (uow.Save(User.Identity.Name) > 0)
                        {
                            //var controller =  ControllerContext.Controller..ToString();
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = m,
                                //MENUNAME = "",
                                RECORDID = BType.ITBID,
                                STATUS = open,
                                // TABLENAME = "ADMIN_DEPARTMENT",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single,
                                //TABLENAME = "COMP"
                            };
                            repoAuth.Insert(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                //newR = 1;
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Department Record");
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

        decimal authId;
            string respMsg = null;

        public async Task<ActionResult> DetailAuth(string a_i, string m)
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

                        ViewBag.HeaderTitle = "Authorize Detail for Company Profile";
                        //ViewBag.StatusVisible = true;
                        if (det != null)
                        {
                            //ViewBag.AuthId = authId;
                            //ViewBag.RecordId = det.RECORDID;
                            //ViewBag.BatchId = det.BATCHID;
                            //ViewBag.PostType = det.POSTTYPE;
                            //ViewBag.MenuId = det.MENUID;
                        obj.AuthId = authId;
                        obj.BatchId = det.BATCHID;
                        obj.RecordId = det.RECORDID.GetValueOrDefault();
                        obj.PostType = det.POSTTYPE;
                        obj.MenuId = det.MENUID.GetValueOrDefault();
                            ViewBag.Message = TempData["msg"];
                            var stat = ViewBag.Message != null ? null : "open";
                            var rec =await _repo.GetCompanyProfileAsync((int)det.RECORDID, true, status: stat);  //repoSession.FindAsync(id);
                        if (rec != null)
                        {
                            var model = rec;
                            var country = await _repo.GetCountryAsync(0, true, "Active");
                            ViewBag.Country = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
                            var state = await _repo.GetStateAsync(0, true, "Active", countryCode: rec.COMPANY_COUNTRY);
                            ViewBag.State = new SelectList(state, "STATECODE", "STATENAME");
                            var banks = await _repo.GetInstitutionAsync(0, true, "Active");
                            ViewBag.BankList = new SelectList(banks.Where(d => d.IS_BANK != null && d.IS_BANK.ToLower() == "y").ToList(), "CBN_CODE", "INSTITUTION_NAME");
                            BindCity(rec.COMPANY_COUNTRY, rec.COMPANY_CITY);
                            ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.CREATED_BY;
                            obj.Status = det.STATUS;
                            ViewBag.Auth = obj;
                            if (det.TABLENAME != null)
                            {
                                return View("DetailAuth", model);
                            }
                            else
                            {
                                ViewBag.HeaderTitle = "Authorize Detail for Srcurity Policy";

                                return View("DetailAuthPL", model);
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
            //int menuId = 0;
            //string msg = "";
            var sucNew = false;
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

                                            suc = ModifyMainRecord(recordId);
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
                                        //txscope.Complete();
                                    }
                                }
                                else
                                {
                                    //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                    //respMsg = "Problem processing request. Try again or contact Administrator.";
                                    //TempData["msg"] = respMsg;
                                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
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
                if(sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Company Profile Approval", null, fullName);
                    
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
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
        private bool ModifyMainRecord(int recordId)
        {
            var rec = repoCompTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = close;
                var obj = repoComp.Find(rec.RECORDID);
                if (obj != null)
                {
                    obj.COMPANY_ADDRESS = rec.COMPANY_ADDRESS;
                    obj.COMPANY_CITY = rec.COMPANY_CITY;
                    obj.COMPANY_CODE = rec.COMPANY_CODE;
                    obj.COMPANY_COUNTRY = rec.COMPANY_COUNTRY;
                    obj.COMPANY_EMAIL = rec.COMPANY_EMAIL;
                    obj.COMPANY_NAME = rec.COMPANY_NAME;
                    obj.COMPANY_PHONE1 = rec.COMPANY_PHONE1;
                    obj.COMPANY_PHONE2 = rec.COMPANY_PHONE2;
                    obj.COMPANY_STATE = rec.COMPANY_STATE;
                    obj.COMPANY_WEBSITE = rec.COMPANY_WEBSITE;
                    obj.NIBSSRATE = rec.NIBSSRATE;
                    obj.NIBSSBANKCODE = rec.NIBSSBANKCODE;
                    obj.NIBSSACCOUNT = rec.NIBSSACCOUNT;
                    obj.WHTACCOUNT = rec.WHTACCOUNT;
                    obj.WHTAXRATE = rec.WHTAXRATE;
                    obj.WHTBANKCODE = rec.WHTBANKCODE;
                    //obj.PASSWORDLENGTH = rec.PASSWORDLENGTH;
                    //obj.PASSWORD_CHANGE_DAYS = rec.PASSWORD_CHANGE_DAYS;
                    //obj.SYSTEM_IDLE_TIMEOUT = rec.SYSTEM_IDLE_TIMEOUT;

                    obj.LAST_MODIFIED_UID = rec.USERID;
                    obj.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    obj.LAST_MODIFIED_DATE = DateTime.Now;
                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(decimal AuthId, int? m,string Narration)
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
                            NARRATION = Narration,
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
                                var recc =  repoCompTemp.Find(recordId);
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
                    // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                    //respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                //}
                if(suc)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Company Profile Rejection", Narration, fullName);
                   
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
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePL(decimal AuthId, int? m)
        {
            var sucNew = false;
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

                                            suc = ModifyMainRecordPL(recordId);
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
                                        //txscope.Complete();
                                    }
                                }
                                else
                                {
                                    //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                    //respMsg = "Problem processing request. Try again or contact Administrator.";
                                    //TempData["msg"] = respMsg;
                                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
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
                    //respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                //}
                if(sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Security Policy Approval", null, fullName);

                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                   
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuthPL", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });

            }
        }
        private bool ModifyMainRecordPL(int recordId)
        {
            var rec = repoCompTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = close;
                var obj = repoComp.Find(rec.RECORDID);
                if (obj != null)
                {
                    obj.PASSWORDLENGTH = rec.PASSWORDLENGTH;
                    obj.PASSWORD_CHANGE_DAYS = rec.PASSWORD_CHANGE_DAYS;
                    obj.SYSTEM_IDLE_TIMEOUT = rec.SYSTEM_IDLE_TIMEOUT;
                    obj.LOCKOUT_TRIAL_COUNT = rec.LOCKOUT_TRIAL_COUNT;
                    try
                    {
                        System.IO.File.SetLastWriteTimeUtc(Server.MapPath("~/web.config"), DateTime.UtcNow);
                    }
                    catch
                    {

                    }
                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RejectPL(decimal AuthId, int? m, string Narration)
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
                            NARRATION = Narration,
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
                                var recc = repoCompTemp.Find(recordId);
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
                //}
                if(suc)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Security Policy Rejection", Narration, fullName);
                  
                   
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