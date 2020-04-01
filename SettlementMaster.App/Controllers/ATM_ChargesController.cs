using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Dapper.Utility;
using Generic.Data;
using Generic.Data.Utilities;
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
    public class ATM_ChargesController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        IDapperATMSettings _repo2 = new DapperATMSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;

        private readonly IRepository<ATM_REQUESTTYPE> repoReqType = null;
        private readonly IRepository<ATM_TRANSACTTYPE> repoTranType = null;
        private readonly IRepository<ATM_OPERATORTYPE> repoPartyType = null;
        private readonly IRepository<ATM_OPERATIONMODE> repoOperationMode = null;
        private readonly IRepository<ATM_CALCBASIS> repoCalcBasis = null;

        private readonly IRepository<ATM_CHARGES> repoAtmCharges = null;
        private readonly IRepository<ATM_CHARGESTEMP> repoAtmChargesTemp = null;


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
        int menuId = 16;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public ATM_ChargesController()
        {
            uow = new UnitOfWork();
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);

            repoReqType = new Repository<ATM_REQUESTTYPE>(uow);
            repoTranType = new Repository<ATM_TRANSACTTYPE>(uow);
            repoPartyType = new Repository<ATM_OPERATORTYPE>(uow);
            repoOperationMode = new Repository<ATM_OPERATIONMODE>(uow);
            repoCalcBasis = new Repository<ATM_CALCBASIS>(uow);

            repoAtmCharges = new Repository<ATM_CHARGES>(uow);
            repoAtmChargesTemp = new Repository<ATM_CHARGESTEMP>(uow);


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
        public ActionResult Index(string m)
        {
            try
            {
                // GetMenuId();
                SessionHelper.GetATMCharges(Session).Clear();
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
            var rec = _repo.GetMenuPrivilege(menuId, roleId);
            if (rec != null)
            {
                ViewBag.CanAdd = rec.CanAdd;
                ViewBag.CanEdit = rec.CanEdit;
            }
        }
        //public async Task<ActionResult> MCCList()
        //{
        //    try
        //    {
        //        var rec = await _repo.GetMCCAsync(0, true);  //repoSession.FindAsync(id);              
        //        return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        var obj1 = new { data = new List<RolesObj>(), RespCode = 2, RespMessage = ex.Message };
        //        return Json(obj1, JsonRequestBehavior.AllowGet);
        //    }

        //}

        public async Task<ActionResult> Add(int id = 0, string m = null)
        {
            try
            {
                menuId = SmartUtil.GetMenuId(m);
                if (menuId == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.MenuId = HttpUtility.UrlEncode(m);
                SessionHelper.GetATMCharges(Session).Clear();
                //ViewBag.MenuId = HttpUtility.UrlDecode(m);
                //BindCombo();

                var rec = await _repo2.GetATMChargesAsync();
                ViewBag.DisableButton = true;


                if (rec.Count() == 0)
                {
                    ViewBag.HeaderTitle = "Add ATM Charges";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add");

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit ATM Charges";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                     BindATMCharges(rec);

                    ViewBag.ATMCharges = rec;
                    GetPriv();
                    return View("Add");

                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Add");
            }
        }


        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string m)
        {
            var errorMsg = "";
            try
            {

                ViewBag.MenuId = m;
                m = HttpUtility.UrlDecode(m);
                menuId = SmartUtil.GetMenuId(m);
                string bid = string.Concat("ATMCharges", "_", SmartObj.GenRefNo2());
         
                //if (mod_mcc.ITBID == 0)
                //{
                //    ViewBag.ButtonText = "Save";
                //    ViewBag.HeaderTitle = "Add ATM Charges";
                //}
                //else
                //{
                //    ViewBag.ButtonText = "Update";
                //    ViewBag.HeaderTitle = "Edit ATM Charges";
                //}
                bool isnew = true;
                if (ModelState.IsValid)
                {
                    if (isnew)
                    {
                        //var errMsg = "";
                        //if (validateForm(mod_mcc, out errMsg))
                        //{
                        //    ViewBag.Message = errMsg; // "Carscheme already Exist.";
                        //    //BindCombo();
                        //    ViewBag.MCCMSC = GetMccMscLines();
                        //    return View("Add", mod_mcc);
                        //};

                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        //SM_MCCTEMP BType = new SM_MCCTEMP()
                        //{
                        //    MCC_CODE = mod_mcc.MCC_CODE,
                        //    MCC_DESC = mod_mcc.MCC_DESC,
                        //    MCC_CAT = mod_mcc.MCC_CAT,
                        //    CREATEDATE = DateTime.Now,
                        //    USERID = User.Identity.Name,
                        //    STATUS = open,
                        //    SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
                        //    BATCHID = bid,
                        //};
                        //repoMccTemp.Insert(BType);

                        //var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                        //if (rst)
                        //{
                        SaveATMChargs(eventInsert, bid);
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = eventInsert,
                            MENUID = menuId,
                            //MENUNAME = "",
                            //RECORDID = BType.ITBID,
                            STATUS = open,
                            // TABLENAME = "ManageMCC",
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
                            SessionHelper.GetMccMsc(Session).Clear();
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "MCC Record");

                            //txscope.Complete();
                            TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                            //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                            return RedirectToAction("Add", "ATM_Charges", new { m = m });
                        }
                    }
                    else
                    {

                        SaveATMChargs(eventEdit, bid);

                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = eventInsert, // mod_mcc.STATUS == active ? eventEdit : mod_mcc.STATUS,
                            MENUID = menuId,
                            //MENUNAME = "",
                            //RECORDID = BType.ITBID,
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
                            SessionHelper.GetATMCharges(Session).Clear();
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "ATM Charges Record");

                            //txscope.Complete();
                            TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                            //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                            return RedirectToAction("Add", "ATM_Charges", new { m = m });
                            //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                            // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                        }
                    }
                    // BindCombo();
                    ViewBag.MCCMSC = GetATMChargesLines();
                    TempData["msg"] = errorMsg;
                    return View("Add");
                }
            }
            catch (SqlException ex)
            {
                //BindCombo();
                ViewBag.MCCMSC = GetATMChargesLines();
                TempData["msg"] = ex.Message;
                return View("Add");
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                //BindCombo();
                ViewBag.MCCMSC = GetATMChargesLines();
                TempData["msg"] = ex.Message;
                return View("Add");
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            ViewBag.MCCMSC = GetATMChargesLines();
            TempData["msg"] = errorMsg;
            return View("Add");
        }

        void SaveATMChargs(string eventType, string batchid)
        {
            //eventType = "New";
            if (eventType == "New")
            {
                var col = GetATMChargesLines();
                foreach (var d in col)
                {
                    //if (cbn_code != "501" && d.ACQFLAG != 1)
                    //{
                    //    continue;
                    //}
                    var obj = new ATM_CHARGESTEMP()
                    {
                        CALCBASIS_ITBID = d.CALCBASIS_ITBID,
                        CUSTOM_VALUE = d.CUSTOM_VALUE, //drpCalcBasis.SelectedValue,
                        BATCHID = batchid,
                        IS_PROCESSOR = d.IS_PROCESSOR,
                        OPERATORTYPE_CODE = d.OPERATORTYPE_CODE,
                        REQUESTTYPE_CODE = d.REQUESTTYPE_CODE,
                        OPERATIONMODE_ID = d.OPERATIONMODE_ID,
                        TRAN_CODE = d.TRAN_CODE,
                        VALUE = d.VALUE,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,

                    };
                    repoAtmChargesTemp.Insert(obj);
                }

            }
            else
            {
                var col = GetATMChargesLines();
                foreach (var d in col)
                {
                    //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                    //{
                    //    continue;
                    //}
                    if (d.NewRecord || d.Updated || d.Deleted)
                    {
                        var obj = new ATM_CHARGESTEMP()
                        {
                            CALCBASIS_ITBID = d.CALCBASIS_ITBID,
                            CUSTOM_VALUE = d.CUSTOM_VALUE, //drpCalcBasis.SelectedValue,
                            BATCHID = batchid,
                            IS_PROCESSOR = d.IS_PROCESSOR,
                            OPERATORTYPE_CODE = d.OPERATORTYPE_CODE,
                            REQUESTTYPE_CODE = d.REQUESTTYPE_CODE,
                            TRAN_CODE = d.TRAN_CODE,
                            VALUE = d.VALUE,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            RECORDID = d.NewRecord ? 0 : d.ITBID,
                            EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                        };
                        repoAtmChargesTemp.Insert(obj);
                    }
                }
            }
        }
        void SaveATMChargesEdit(string eventType, string batchid)
        {
            if (eventType == "New")
            {
                var col = GetATMChargesLines();
                foreach (var d in col)
                {
                    var obj = new ATM_CHARGESTEMP()
                    {
                        CALCBASIS_ITBID = d.CALCBASIS_ITBID,
                        CUSTOM_VALUE = d.CUSTOM_VALUE, //drpCalcBasis.SelectedValue,
                        BATCHID = batchid,
                        IS_PROCESSOR = d.IS_PROCESSOR,
                        OPERATORTYPE_CODE = d.OPERATORTYPE_CODE,
                        REQUESTTYPE_CODE = d.REQUESTTYPE_CODE,
                        TRAN_CODE = d.TRAN_CODE,
                        VALUE = d.VALUE,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                    };
                    repoAtmChargesTemp.Insert(obj);
                }

            }
            else
            {
                var col = GetATMChargesLines();
                foreach (var d in col)
                {
                    //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                    //{
                    //    continue;
                    //}

                    var obj = new ATM_CHARGESTEMP()
                    {
                        CALCBASIS_ITBID = d.CALCBASIS_ITBID,
                        CUSTOM_VALUE = d.CUSTOM_VALUE, //drpCalcBasis.SelectedValue,
                        BATCHID = batchid,
                        IS_PROCESSOR = d.IS_PROCESSOR,
                        OPERATORTYPE_CODE = d.OPERATORTYPE_CODE,
                        REQUESTTYPE_CODE = d.REQUESTTYPE_CODE,
                        TRAN_CODE = d.TRAN_CODE,
                        VALUE = d.VALUE,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        RECORDID = d.NewRecord ? 0 : d.ITBID,
                        EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                    };
                    repoAtmChargesTemp.Insert(obj);

                }
            }
        }

        void BindATMCharges(List<ATMChargesObj> rec)
        {
            SessionHelper.GetATMCharges(Session).Clear();

            foreach (var d in rec)
            {
                if (d.OPERATIONMODE_ID == 3)
                {
                    d.PartyValue = d.CUSTOM_VALUE;
                }
                else
                {
                    d.PartyValue = d.VALUE.GetValueOrDefault().ToString("F");
                }
                SessionHelper.GetATMCharges(Session).AddItem(d);
            }
        }

        //[HttpPost]
        //// [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Add(MCCObj mod_mcc, string CBN_CODE, string m)
        //{
        //    try
        //    {
        //        ViewBag.MenuId = m;
        //        menuId = SmartUtil.GetMenuId(m);
        //        string bid = string.Concat("MCCMSC", "_", SmartObj.GenRefNo2());
        //        var errorMsg = "";
        //        if (mod_mcc.ITBID == 0)
        //        {
        //            ViewBag.ButtonText = "Save";
        //            ViewBag.HeaderTitle = "Add MCC";
        //        }
        //        else
        //        {
        //            ViewBag.ButtonText = "Update";
        //            ViewBag.HeaderTitle = "Edit MCC";
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            mod_mcc.MCC_CODE = mod_mcc.MCC_CODE.Trim();

        //            if (mod_mcc.ITBID == 0)
        //            {
        //                var errMsg = "";
        //                if (validateForm(mod_mcc, out errMsg))
        //                {
        //                    ViewBag.Message = errMsg; // "Carscheme already Exist.";
        //                    BindCombo();
        //                    ViewBag.MCCMSC = GetATMChargesLines();
        //                    return View("Add", mod_mcc);
        //                };

        //                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //                //{
        //                SM_MCCTEMP BType = new SM_MCCTEMP()
        //                {
        //                    MCC_CODE = mod_mcc.MCC_CODE,
        //                    MCC_DESC = mod_mcc.MCC_DESC,
        //                    MCC_CAT = mod_mcc.MCC_CAT,
        //                    CREATEDATE = DateTime.Now,
        //                    USERID = User.Identity.Name,
        //                    STATUS = open,
        //                    SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
        //                    BATCHID = bid,
        //                };
        //                repoMccTemp.Insert(BType);

        //                var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
        //                if (rst)
        //                {
        //                    SaveMSC(eventInsert, bid, BType.MCC_CODE, CBN_CODE);
        //                    SM_AUTHLIST auth = new SM_AUTHLIST()
        //                    {
        //                        CREATEDATE = DateTime.Now,
        //                        EVENTTYPE = eventInsert,
        //                        MENUID = menuId,
        //                        //MENUNAME = "",
        //                        RECORDID = BType.ITBID,
        //                        STATUS = open,
        //                        // TABLENAME = "ManageMCC",
        //                        URL = Request.FilePath,
        //                        USERID = User.Identity.Name,
        //                        POSTTYPE = Single,
        //                        BATCHID = bid,
        //                        INSTITUTION_ITBID = institutionId

        //                    };
        //                    repoAuth.Insert(auth);
        //                    var rst1 = uow.Save(User.Identity.Name);
        //                    if (rst1 > 0)
        //                    {
        //                        SessionHelper.GetMccMsc(Session).Clear();
        //                        EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "MCC Record");

        //                        //txscope.Complete();
        //                        TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
        //                        //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
        //                        return RedirectToAction("Index", "MCC", new { m = m });
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //                //{

        //                SM_MCCTEMP BType = new SM_MCCTEMP()
        //                {
        //                    MCC_CODE = mod_mcc.MCC_CODE,
        //                    MCC_DESC = mod_mcc.MCC_DESC,
        //                    MCC_CAT = mod_mcc.MCC_CAT,
        //                    CREATEDATE = DateTime.Now,
        //                    USERID = User.Identity.Name,
        //                    STATUS = open,
        //                    SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
        //                    BATCHID = bid,
        //                    RECORDID = mod_mcc.ITBID
        //                };
        //                repoMccTemp.Insert(BType);
        //                //  var rst1 = new AuthListUtil().SaveLog(auth);
        //                var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

        //                if (rst)
        //                {
        //                    SaveMSCEdit(eventEdit, bid, BType.MCC_CODE, CBN_CODE);
        //                    SM_AUTHLIST auth = new SM_AUTHLIST()
        //                    {
        //                        CREATEDATE = DateTime.Now,
        //                        EVENTTYPE = mod_mcc.STATUS == active ? eventEdit : mod_mcc.STATUS,
        //                        MENUID = menuId,
        //                        //MENUNAME = "",
        //                        RECORDID = BType.ITBID,
        //                        STATUS = open,
        //                        // TABLENAME = "ADMIN_DEPARTMENT",
        //                        URL = Request.FilePath,
        //                        USERID = User.Identity.Name,
        //                        POSTTYPE = Single,
        //                        INSTITUTION_ITBID = institutionId,
        //                        BATCHID = bid,

        //                    };

        //                    repoAuth.Insert(auth);
        //                    if (uow.Save(User.Identity.Name) > 0)
        //                    {
        //                        SessionHelper.GetMccMsc(Session).Clear();
        //                        EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "MCC Record");

        //                        //txscope.Complete();
        //                        TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
        //                        //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
        //                        return RedirectToAction("Index", "MCC", new { m = m });
        //                        //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

        //                        // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
        //            //{

        //            SM_MCCTEMP BType = new SM_MCCTEMP()
        //            {
        //                MCC_CODE = mod_mcc.MCC_CODE,
        //                MCC_DESC = mod_mcc.MCC_DESC,
        //                MCC_CAT = mod_mcc.MCC_CAT,
        //                CREATEDATE = DateTime.Now,
        //                USERID = User.Identity.Name,
        //                STATUS = open,
        //                SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
        //                BATCHID = bid,
        //                RECORDID = mod_mcc.ITBID
        //            };
        //            repoMccTemp.Insert(BType);
        //            //  var rst1 = new AuthListUtil().SaveLog(auth);
        //            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

        //            if (rst)
        //            {
        //                SaveMSC(eventEdit, bid, BType.MCC_CODE, CBN_CODE);
        //                SM_AUTHLIST auth = new SM_AUTHLIST()
        //                {
        //                    CREATEDATE = DateTime.Now,
        //                    EVENTTYPE = mod_mcc.STATUS == active ? eventEdit : mod_mcc.STATUS,
        //                    MENUID = menuId,
        //                    //MENUNAME = "",
        //                    RECORDID = BType.ITBID,
        //                    STATUS = open,
        //                    // TABLENAME = "ADMIN_DEPARTMENT",
        //                    URL = Request.FilePath,
        //                    USERID = User.Identity.Name,
        //                    POSTTYPE = Single,
        //                    INSTITUTION_ITBID = institutionId,
        //                    BATCHID = bid,

        //                };

        //                repoAuth.Insert(auth);
        //                if (uow.Save(User.Identity.Name) > 0)
        //                {
        //                    SessionHelper.GetMccMsc(Session).Clear();
        //                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "MCC Record");

        //                    //txscope.Complete();
        //                    TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
        //                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
        //                    return RedirectToAction("Index", "MCC", new { m = m });
        //                    //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

        //                    // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

        //                }
        //            }
        //        }

        //        //}
        //        // If we got this far, something failed, redisplay form
        //        //return Json(new { RespCode = 1, RespMessage = errorMsg });
        //        BindCombo();
        //        ViewBag.MCCMSC = GetATMChargesLines();
        //        TempData["msg"] = errorMsg;
        //        return View("Add", mod_mcc);
        //        //}
        //    }
        //    catch (SqlException ex)
        //    {
        //        BindCombo();
        //        ViewBag.MCCMSC = GetATMChargesLines();
        //        TempData["msg"] = ex.Message;
        //        return View("Add", mod_mcc);
        //        //return Json(new { RespCode = 1, RespMessage = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        BindCombo();
        //        ViewBag.MCCMSC = GetATMChargesLines();
        //        TempData["msg"] = ex.Message;
        //        return View("Add", mod_mcc);
        //        // return Json(new { RespCode = 1, RespMessage = ex.Message });
        //    }
        //    ViewBag.MCCMSC = GetATMChargesLines();
        //    TempData["msg"] = "Problem Processing Request, Try again or Contact Administrator.";
        //    return View("Add", mod_mcc);
        //    //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        //}

        void BindComboMsc()
        {
            var reqType = repoReqType.AllEager().Select(d=> new { Code = d.REQUESTTYPE_CODE, Description = d.REQUESTTYPE_DESC }).ToList();
            var tranType = repoTranType.AllEager().Select(d => new { Code = d.TRAN_CODE, Description = d.TRAN_DESC }).ToList(); 
            var partyType = _repo2.GetATMPartyType();
            var calc_basis = repoCalcBasis.AllEager().Select(d => new { Code = d.ITBID, Description = d.DESCRIPTION }).ToList();

            ViewBag.ReqType = new SelectList(reqType, "Code", "Description");
            ViewBag.TranType = new SelectList(tranType, "Code", "Description");
            ViewBag.ATMPartyType = new SelectList(partyType, "Code", "Description");
            ViewBag.CalcBasis = new SelectList(calc_basis, "Code", "Description");
        }
        public async Task<PartialViewResult> AddCharges(decimal id = 0, string m = null)
        {
            try
            {
                ViewBag.MenuId = m;

                BindComboMsc();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Charges";
                    ViewBag.ButtonText = "Add";
                    return PartialView("_AddCharges", new ATMChargesObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Charges";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetMCCAsync(0, false);
                    if (rec == null)
                    {
                        return null;
                    }
                    var model = rec.FirstOrDefault();
                    return PartialView("_AddCharges", model);

                }
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                return null;
            }
        }
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddCharges(ATMChargesObj model, string m)
        {
            string msg = "";
            try
            {
                var lst = GetATMChargesLines().ToList();
                int opId = 0;
                var splt = model.OPERATORTYPE.Split('-');
                if (splt != null)
                {
                    model.OPERATIONMODE_ID = int.TryParse(splt[0], out opId) ? opId : (int?)null;
                    model.OPERATORTYPE_CODE = splt[1];
                }
                if (model.ITBID == 0)
                {
                    var exist = lst.Exists(r => r.OPERATORTYPE_CODE == model.OPERATORTYPE_CODE && r.OPERATIONMODE_ID == model.OPERATIONMODE_ID);
                    if (exist)
                    {
                        // Party Type already exist. Duplicate Record is not allowed.;

                        msg = "ATM Party Type already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    int itbid = 0;
                    if (lst.Count == 0)
                    {

                        itbid = 1;
                    }
                    else
                    {
                        var itb = lst.Max(f => f.ITBID);
                        itbid = itb + 1;
                    }
                    var domCur = repoCalcBasis.AllEager(d => d.ITBID == model.CALCBASIS_ITBID).FirstOrDefault();
                    if (domCur != null)
                    {
                        model.CALC_BASIS = domCur.DESCRIPTION;
                    }
                    var intCur = repoReqType.AllEager(d => d.REQUESTTYPE_CODE == model.REQUESTTYPE_CODE).FirstOrDefault();
                    if (intCur != null)
                    {
                        model.REQUESTTYPE_DESC = intCur.REQUESTTYPE_DESC;
                    }
                    var domFreq = repoTranType.AllEager(d => d.TRAN_CODE == model.TRAN_CODE).FirstOrDefault();
                    if (domFreq != null)
                    {
                        model.TRAN_DESC = domFreq.TRAN_DESC;
                    }
                   // model.OPERATORTYPE_DESC = GetOperatorType(model.OPERATORTYPE_CODE,model.OPERATIONMODE_ID);
                    var obj = new ATMChargesObj()
                    {
                        CALCBASIS_ITBID = model.CALCBASIS_ITBID,
                        CALC_BASIS = model.CALC_BASIS,
                        CUSTOM_VALUE = model.OPERATIONMODE_ID == null ? model.PartyValue : null,
                        OPERATIONMODE_ID = model.OPERATIONMODE_ID,
                        OPERATORTYPE_CODE = model.OPERATORTYPE_CODE,
                        OPERATORTYPE_DESC = model.OPERATORTYPE_DESC,
                        OPERATORTYPE = model.OPERATORTYPE,
                        IS_PROCESSOR = model.OPERATIONMODE_ID == 3 ? true : false,
                        REQUESTTYPE_CODE = model.REQUESTTYPE_CODE,
                        REQUESTTYPE_DESC = model.REQUESTTYPE_DESC,
                        TRAN_CODE = model.TRAN_CODE,
                        TRAN_DESC = model.TRAN_DESC,
                        PartyValue = model.PartyValue,
                        VALUE = model.OPERATIONMODE_ID != null ? decimal.Parse(model.PartyValue) : 0,
                        ITBID = itbid,
                        NewRecord = true,
                        STATUS = inActive,
                    };

                    SessionHelper.GetATMCharges(Session).AddItem(obj);
                    var w = GetATMChargesLines().ToList();
                    var html = PartialView("_ViewATMCharges", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data = w, data_msc = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                    var existRec = lst.Where(r => r.OPERATORTYPE_CODE == model.OPERATORTYPE_CODE && r.OPERATIONMODE_ID == model.OPERATIONMODE_ID).FirstOrDefault();
                    if (existRec != null) // not expected to be null
                    {
                        if (oldRec.ITBID != existRec.ITBID)
                        {
                            msg = "ATM Party Type already exist. Duplicate Record is not allowed.";
                            return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    var domCur = repoCalcBasis.AllEager(d => d.ITBID == model.CALCBASIS_ITBID).FirstOrDefault();
                    if (domCur != null)
                    {
                        model.CALC_BASIS = domCur.DESCRIPTION;
                    }
                    var intCur = repoReqType.AllEager(d => d.REQUESTTYPE_CODE == model.REQUESTTYPE_CODE).FirstOrDefault();
                    if (intCur != null)
                    {
                        model.REQUESTTYPE_DESC = intCur.REQUESTTYPE_DESC;
                    }
                    var domFreq = repoTranType.AllEager(d => d.TRAN_CODE == model.TRAN_CODE).FirstOrDefault();
                    if (domFreq != null)
                    {
                        model.TRAN_DESC = domFreq.TRAN_DESC;
                    }
                   // model.OPERATORTYPE_DESC = GetOperatorType(model.OPERATORTYPE_CODE, model.OPERATIONMODE_ID);
                    //var intFreq = repoFreq.AllEager(d => d.ITBID == model.DOM_FREQUENCY).FirstOrDefault();
                    //if (intFreq != null)
                    //{
                    //    model.IntFrequencyDesc = intFreq.FREQUENCY_DESC;
                    //}
                    //var channel = repoChannel.AllEager(d => d.CODE == model.CHANNEL).FirstOrDefault();
                    //if (channel != null)
                    //{
                    //    model.ChannelDesc = channel.DESCRIPTION;
                    //}
                    var obj = new ATMChargesObj()
                    {
                        CALCBASIS_ITBID = model.CALCBASIS_ITBID,
                        CALC_BASIS = model.CALC_BASIS,
                        CUSTOM_VALUE = model.OPERATIONMODE_ID == null ? model.PartyValue : null,
                        OPERATIONMODE_ID = model.OPERATIONMODE_ID,
                        OPERATORTYPE_CODE = model.OPERATORTYPE_CODE,
                        OPERATORTYPE_DESC = model.OPERATORTYPE_DESC,
                        OPERATORTYPE = model.OPERATORTYPE,
                        IS_PROCESSOR = model.IS_PROCESSOR,
                        REQUESTTYPE_CODE = model.REQUESTTYPE_CODE,
                        REQUESTTYPE_DESC = model.REQUESTTYPE_DESC,
                        TRAN_CODE = model.TRAN_CODE,
                        TRAN_DESC = model.TRAN_DESC,
                        PartyValue = model.PartyValue,
                        VALUE = model.OPERATIONMODE_ID != null ? decimal.Parse(model.PartyValue) : 0,
                        ITBID = model.ITBID,
                        NewRecord = oldRec.NewRecord,
                        Updated = true,

                    };
                    SessionHelper.GetATMCharges(Session).UpdateItem(obj);

                    var w = GetATMChargesLines().ToList();
                    var html = PartialView("_ViewATMCharges", w).RenderToString();
                    msg = "Record Updated to List";
                    return Json(new { data_msc = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }
        }

        private string GetOperatorType(string opCode,int? opMode)
        {
            string opDesc1 = "";
            string opMode1 = "";
            var intFreq = repoPartyType.AllEager(d => d.OPERATORTYPE_CODE == opCode).FirstOrDefault();
            if (intFreq != null)
            {
                opDesc1 = intFreq.OPERATORTYPE_DESC;
            }
            var channel = repoOperationMode.AllEager(d => d.ITBID == opMode).FirstOrDefault();
            if (channel != null)
            {
                opMode1 = channel.DESCRIPTION;
            }

            return  opDesc1 + "-" + opMode1;
        }

        public IList<ATMChargesObj> GetATMChargesLines()
        {
            //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
            return SessionHelper.GetATMCharges(Session).Lines;
        }

        public ActionResult EditATMCharges(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetATMChargesLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    BindComboMsc();
                    ViewBag.ButtonText = "Update";
                    return PartialView("_AddCharges", rec);
                }
            }
            catch
            {

            }
            return null;
        }
        public ActionResult DeleteATMCharges(int id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetATMChargesLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    if (rec.NewRecord)
                    {
                        SessionHelper.GetATMCharges(Session).RemoveLine(rec.ITBID);

                    }
                    else
                    {
                        SessionHelper.GetATMCharges(Session).MarkForDelete(rec.ITBID);

                    }
                    var lst2 = GetATMChargesLines().ToList();
                    var html2 = PartialView("_ViewATMCharges", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_msc = html2 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetATMChargesLines().ToList();
            var html = PartialView("_ViewATMCharges", lst3).RenderToString();
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_msc = html }, JsonRequestBehavior.AllowGet);
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

                    ViewBag.HeaderTitle = "Authorize Detail for ATM Charges";
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
                        //var rec = _repo.GetMCC((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        //if (rec != null && rec.Count > 0)
                        //{
                            var model = BindATMChargesTemp(det.BATCHID,det.USERID);
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = det.USERID;
                            ViewBag.Auth = obj;
                            ViewBag.DisplayAuth = det.STATUS == open && !(obj.User == User.Identity.Name);

                           // BindCombo();
                            //ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
                            // return null;

                            return View("DetailAuth", model);

                       // }
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

        List<ATMChargesObj> BindATMChargesTemp(string batchId,string userId)
        {
            var rec = _repo2.GetATMCharges_Temp(batchId,userId);
            return rec;

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
                            //recordId = (int)rec2.RECORDID;
                            menuId = rec2.MENUID.GetValueOrDefault();
                            suc = ProcessATMCharges(rec2.BATCHID, rec2.USERID);
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
                                TempData["msg"] = respMsg;
                                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                //return Json(new { RespCode = 1, RespMessage = respMsg });
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
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "MCC Approval Record", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    TempData["msg"] = respMsg;
                    TempData["status"] = approve;
                    return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                }
                respMsg = "This request has already been processed by an authorizer.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                //return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool ProcessATMCharges(string batchid, string user_Id)
        {
            var dt = repoAtmChargesTemp.AllEager(e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
            if (dt.Count != 0)
            {
                    foreach (var d in dt)
                    {
                        var gh = repoAtmCharges.AllEager(g => g.OPERATORTYPE_CODE == d.OPERATORTYPE_CODE && g.OPERATIONMODE_ID == d.OPERATIONMODE_ID).FirstOrDefault();
                        if (gh == null)
                        {
                            gh = new ATM_CHARGES()
                            {
                                BATCHID = d.BATCHID,
                                CALCBASIS_ITBID = d.CALCBASIS_ITBID,
                                CUSTOM_VALUE = d.CUSTOM_VALUE,
                                IS_PROCESSOR = d.IS_PROCESSOR,
                                OPERATIONMODE_ID = d.OPERATIONMODE_ID,
                                OPERATORTYPE_CODE = d.OPERATORTYPE_CODE,
                                REQUESTTYPE_CODE = d.REQUESTTYPE_CODE,
                                TRAN_CODE = d.TRAN_CODE,
                                VALUE = d.VALUE,
                                STATUS = active,
                                CREATEDATE = DateTime.Now,
                                USERID = user_Id,
                            };
                            d.STATUS = approve;
                            repoAtmCharges.Insert(gh);
                        }
                        else
                        {
                            gh.BATCHID = d.BATCHID;
                            gh.VALUE = d.VALUE;
                            gh.CUSTOM_VALUE = d.CUSTOM_VALUE;
                            gh.CALCBASIS_ITBID = d.CALCBASIS_ITBID;
                            gh.IS_PROCESSOR = d.IS_PROCESSOR;
                            gh.OPERATIONMODE_ID = d.OPERATIONMODE_ID;
                            gh.OPERATORTYPE_CODE = d.OPERATORTYPE_CODE;
                            gh.REQUESTTYPE_CODE = d.REQUESTTYPE_CODE;
                            gh.TRAN_CODE = d.TRAN_CODE;
                            gh.LAST_MODIFIED_UID = user_Id;
                          
                        }
                    }
                }
            
            return true;
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
                            RejectBatch(recordId, rec2.BATCHID, rec2.USERID);

                            rec2.STATUS = reject;
                            var t = uow.Save(User.Identity.Name);
                            if (t > 0)
                            {
                                sucNew = true;
                            }
                        }
                    }
                }

                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "MCC Rejection Record", Narration, fullName);
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
            }
        }
        private void RejectBatch(decimal? rECORDID, string bATCHID, string uSERID)
        {
            var recPP = repoAtmChargesTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
            foreach (var d in recPP)
            {
                d.STATUS = reject;
            }
        }
    }
}