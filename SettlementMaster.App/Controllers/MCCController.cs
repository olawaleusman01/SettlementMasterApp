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
using System.Text;
using System.Threading.Tasks;
//using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class MCCController : Controller
    {
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;

        //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_MCC> repoMcc = null;
        private readonly IRepository<SM_MCCMSC> repoMccMsc = null;
        private readonly IRepository<SM_MCCTEMP> repoMccTemp = null;
        private readonly IRepository<SM_MCCMSCTEMP> repoMccMscTemp = null;
        private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
        private readonly IRepository<SM_CURRENCY> repoCurrency = null;
        private readonly IRepository<SM_FREQUENCY> repoFreq = null;
        private readonly IRepository<SM_SECTOR> repoSec = null;
        private readonly IRepository<SM_CHANNELS> repoChannel = null;
        
        //private readonly IRepository<SM_MCCCATEGORY> repoMC = null;

        private readonly IRepository<SM_INSTITUTION> repoInst = null;

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
        public MCCController()
        {
            uow = new UnitOfWork();
            repoScheme = new Repository<SM_CARDSCHEME>(uow);
            repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoMcc = new Repository<SM_MCC>(uow);
            repoMccMsc = new Repository<SM_MCCMSC>(uow);
            repoMccTemp = new Repository<SM_MCCTEMP>(uow);
            repoMccMscTemp = new Repository<SM_MCCMSCTEMP>(uow);
            repoFreq = new Repository<SM_FREQUENCY>(uow);
            repoSec = new Repository<SM_SECTOR>(uow);
            // repoMC = new Repository<SM_MCCCATEGORY>(uow);
            //repoVal = new Repository<SM_MERCHANTCONFIG>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoChannel = new Repository<SM_CHANNELS>(uow);

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
                SessionHelper.GetMccMsc(Session).Clear();
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
        public async Task<ActionResult> MCCList()
            {
                try
                {
                    var rec = await _repo.GetMCCAsync(0, true);  //repoSession.FindAsync(id);              
                    return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var obj1 = new { data = new List<RolesObj>(), RespCode = 2, RespMessage = ex.Message };
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
    

        private bool validateForm(MCCObj obj, out string errorMsg)
        {
            var sb = new StringBuilder();
            var errCount = 0;
            var exist = repoMcc.AllEager(f => f.MCC_CODE == obj.MCC_CODE).FirstOrDefault();
            if (exist != null)
            {
                sb.AppendLine("MCC CODE Already Exist");
                errCount++;
            }
            //var exist2 = repoCurr.AllEager(f => f.CURRENCY_CODE == obj.CURRENCY_CODE).FirstOrDefault();
            //if (exist2 != null)
            //{
            //    sb.AppendLine("Currency Code (Numeric) Already Exist");
            //    errCount++;
            //}
            if (errCount > 0)
            {
                errorMsg = sb.ToString();
                return true;
            }
            //errorMsg = sb.ToString();
            errorMsg = "";
            return false;
        }

        void SaveMSC(string eventType, string batchid,string mcc_code,string cbn_code)
        {
            if (!string.IsNullOrEmpty(cbn_code))
            {
                //eventType = "New";
                if (eventType == "New")
                {
                    var col = GetMccMscLines();
                    foreach (var d in col)
                    {
                        //if (cbn_code != "501" && d.ACQFLAG != 1)
                        //{
                        //    continue;
                        //}
                        var obj = new SM_MCCMSCTEMP()
                        {
                            CARDSCHEME = d.CARDSCHEME,
                            MSC_CALCBASIS = d.MSC_CALCBASIS, //drpCalcBasis.SelectedValue,
                            BATCHID = batchid,
                            DOM_FREQUENCY = d.DOM_FREQUENCY,
                            DOM_MSCVALUE = d.DOM_MSCVALUE,
                            DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY,
                            INT_FREQUENCY = d.INT_FREQUENCY,
                            INT_MSCVALUE = d.INT_MSCVALUE,
                            INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY,
                            MCC_CODE = mcc_code,
                            CHANNEL = int.Parse(d.CHANNEL.ToString()),
                            DOMCAP = d.DOMCAP,
                            INTLCAP = d.INTLCAP,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            CBN_CODE = cbn_code,
                            INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                            EVENTTYPE = eventInsert,
                        };
                        repoMccMscTemp.Insert(obj);
                    }

                }
                else
                {
                    var col = GetMccMscLines();
                    foreach (var d in col)
                    {
                        //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                        //{
                        //    continue;
                        //}
                        if (d.NewRecord || d.Updated || d.Deleted)
                        {
                            var obj = new SM_MCCMSCTEMP()
                            {
                                CARDSCHEME = d.CARDSCHEME,
                                MSC_CALCBASIS = d.MSC_CALCBASIS, //drpCalcBasis.SelectedValue,
                                BATCHID = batchid,
                                CHANNEL = int.Parse(d.CHANNEL.ToString()),
                                DOM_FREQUENCY = d.DOM_FREQUENCY,
                                DOM_MSCVALUE = d.DOM_MSCVALUE,
                                DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY,
                                INT_FREQUENCY = d.INT_FREQUENCY,
                                INT_MSCVALUE = d.INT_MSCVALUE,
                                INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY,
                                MCC_CODE = mcc_code,
                                DOMCAP = d.DOMCAP,
                                INTLCAP = d.INTLCAP,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                USERID = User.Identity.Name,
                                //  RECORDID = rid,
                                RECORDID = d.NewRecord ? 0 : d.ITBID,
                                CBN_CODE = cbn_code,
                                INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                                EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                            };
                            repoMccMscTemp.Insert(obj);
                        }
                    }
                }
            }
        }

        void SaveMSCEdit(string eventType, string batchid, string mcc_code, string cbn_code)
        {
            if (!string.IsNullOrEmpty(cbn_code))
            {
                //eventType = "New";
                if (eventType == "New")
                {
                    var col = GetMccMscLines();
                    foreach (var d in col)
                    {
                        //if (cbn_code != "501" && d.ACQFLAG != 1)
                        //{
                        //    continue;
                        //}
                        var obj = new SM_MCCMSCTEMP()
                        {
                            CARDSCHEME = d.CARDSCHEME,
                            MSC_CALCBASIS = d.MSC_CALCBASIS, //drpCalcBasis.SelectedValue,
                            BATCHID = batchid,
                            DOM_FREQUENCY = d.DOM_FREQUENCY,
                            DOM_MSCVALUE = d.DOM_MSCVALUE,
                            DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY,
                            INT_FREQUENCY = d.INT_FREQUENCY,
                            INT_MSCVALUE = d.INT_MSCVALUE,
                            INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY,
                            MCC_CODE = mcc_code,
                            CHANNEL = int.Parse(d.CHANNEL.ToString()),
                            DOMCAP = d.DOMCAP,
                            INTLCAP = d.INTLCAP,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            CBN_CODE = cbn_code,
                            INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                            EVENTTYPE = eventInsert,
                        };
                        repoMccMscTemp.Insert(obj);
                    }

                }
                else
                {
                    var col = GetMccMscLines();
                    foreach (var d in col)
                    {
                        //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                        //{
                        //    continue;
                        //}
                        
                            var obj = new SM_MCCMSCTEMP()
                            {
                                CARDSCHEME = d.CARDSCHEME,
                                MSC_CALCBASIS = d.MSC_CALCBASIS, //drpCalcBasis.SelectedValue,
                                BATCHID = batchid,
                                CHANNEL = int.Parse(d.CHANNEL.ToString()),
                                DOM_FREQUENCY = d.DOM_FREQUENCY,
                                DOM_MSCVALUE = d.DOM_MSCVALUE,
                                DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY,
                                INT_FREQUENCY = d.INT_FREQUENCY,
                                INT_MSCVALUE = d.INT_MSCVALUE,
                                INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY,
                                MCC_CODE = mcc_code,
                                DOMCAP = d.DOMCAP,
                                INTLCAP = d.INTLCAP,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                USERID = User.Identity.Name,
                                //  RECORDID = rid,
                                RECORDID = d.NewRecord ? 0 : d.ITBID,
                                CBN_CODE = cbn_code,
                                INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                                EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                            };
                            repoMccMscTemp.Insert(obj);
                        
                    }
                }
            }
        }

        void BindCombo(string acq_selected = null)
        {
            var inst =  _repo.GetInstitution(0, true, "Active");
            var sector = _repo.GetSector(0, true, "Active");
            ViewBag.Sector = new SelectList(sector, "SECTOR_CODE", "SECTOR_NAME");
            ViewBag.Acquirer = new SelectList(inst, "CBN_CODE", "INSTITUTION_NAME","000");
            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

        }
        public async Task<ActionResult> Add(int id = 0, string m = null)
        {
            try
            {
                SessionHelper.GetMccMsc(Session).Clear();
                ViewBag.MenuId = HttpUtility.UrlDecode(m);
                BindCombo();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add MCC";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add", new MCCObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit MCC";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetMCCAsync(id, false);
                    if (rec == null)
                    {
                        //bad request
                        // return Json(null, JsonRequestBehavior.AllowGet);
                        //var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                        // return Json(obj, JsonRequestBehavior.AllowGet);
                        ViewBag.Message = "Record Not Found";
                        return RedirectToAction("Index","Home");
                    }
                    var model = rec.FirstOrDefault();
                    var recMCC = await BindScheme(model.MCC_CODE, "000");
                    ViewBag.MCCMSC = recMCC;
                    GetPriv();
                    return View("Add", model);

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
        public ActionResult Add(MCCObj mod_mcc, string CBN_CODE, string m)
        {
            try
            {
                ViewBag.MenuId = m;
                menuId = SmartUtil.GetMenuId(m);
                string bid = string.Concat("MCCMSC", "_", SmartObj.GenRefNo2());
                var errorMsg = "";
                if (mod_mcc.ITBID == 0)
                {
                    ViewBag.ButtonText = "Save";
                    ViewBag.HeaderTitle = "Add MCC";
                }
                else
                {
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit MCC";
                }

                if (ModelState.IsValid)
                {
                    mod_mcc.MCC_CODE = mod_mcc.MCC_CODE.Trim();

                    if (mod_mcc.ITBID == 0)
                    {
                        var errMsg = "";
                        if (validateForm(mod_mcc, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            ViewBag.MCCMSC = GetMccMscLines();
                            return View("Add", mod_mcc);
                        };

                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        SM_MCCTEMP BType = new SM_MCCTEMP()
                        {
                            MCC_CODE = mod_mcc.MCC_CODE,
                            MCC_DESC = mod_mcc.MCC_DESC,
                            MCC_CAT = mod_mcc.MCC_CAT,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            STATUS = open,
                            SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
                            BATCHID = bid,
                        };
                        repoMccTemp.Insert(BType);

                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst)
                        {
                            SaveMSC(eventInsert, bid, BType.MCC_CODE, CBN_CODE);
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                RECORDID = BType.ITBID,
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
                                return RedirectToAction("Index", "MCC", new { m = m });
                            }
                        }
                    }
                     else
                  {
                    //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    //{

                    SM_MCCTEMP BType = new SM_MCCTEMP()
                    {
                        MCC_CODE = mod_mcc.MCC_CODE,
                        MCC_DESC = mod_mcc.MCC_DESC,
                        MCC_CAT = mod_mcc.MCC_CAT,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        STATUS = open,
                        SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
                        BATCHID = bid,
                        RECORDID = mod_mcc.ITBID
                    };
                    repoMccTemp.Insert(BType);
                    //  var rst1 = new AuthListUtil().SaveLog(auth);
                    var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                    if (rst)
                    {
                        SaveMSCEdit(eventEdit, bid, BType.MCC_CODE, CBN_CODE);
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = mod_mcc.STATUS == active ? eventEdit : mod_mcc.STATUS,
                            MENUID = menuId,
                            //MENUNAME = "",
                            RECORDID = BType.ITBID,
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
                            SessionHelper.GetMccMsc(Session).Clear();
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "MCC Record");

                            //txscope.Complete();
                            TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                            //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                            return RedirectToAction("Index", "MCC", new { m = m });
                            //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                            // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                        }
                    }
                }
                }
                else
                {
                    //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    //{

                    SM_MCCTEMP BType = new SM_MCCTEMP()
                    {
                        MCC_CODE = mod_mcc.MCC_CODE,
                        MCC_DESC = mod_mcc.MCC_DESC,
                        MCC_CAT = mod_mcc.MCC_CAT,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        STATUS = open,
                        SECTOR_CODE = !string.IsNullOrEmpty(mod_mcc.SECTOR_CODE) ? mod_mcc.SECTOR_CODE : null,
                        BATCHID = bid,
                        RECORDID = mod_mcc.ITBID
                    };
                    repoMccTemp.Insert(BType);
                    //  var rst1 = new AuthListUtil().SaveLog(auth);
                    var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                    if (rst)
                    {
                        SaveMSC(eventEdit, bid, BType.MCC_CODE, CBN_CODE);
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = mod_mcc.STATUS == active ? eventEdit : mod_mcc.STATUS,
                            MENUID = menuId,
                            //MENUNAME = "",
                            RECORDID = BType.ITBID,
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
                            SessionHelper.GetMccMsc(Session).Clear();
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "MCC Record");

                            //txscope.Complete();
                            TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                            //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                            return RedirectToAction("Index", "MCC", new { m = m });
                            //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                            // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                        }
                    }
                }

                //}
                // If we got this far, something failed, redisplay form
                //return Json(new { RespCode = 1, RespMessage = errorMsg });
                BindCombo();
                ViewBag.MCCMSC = GetMccMscLines();
                TempData["msg"] = errorMsg;
                return View("Add", mod_mcc);
                //}
            }
            catch (SqlException ex)
            {
                BindCombo();
                ViewBag.MCCMSC = GetMccMscLines();
                TempData["msg"] = ex.Message;
                return View("Add", mod_mcc);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                BindCombo();
                ViewBag.MCCMSC = GetMccMscLines();
                TempData["msg"] = ex.Message;
                return View("Add", mod_mcc);
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            ViewBag.MCCMSC = GetMccMscLines();
            TempData["msg"] = "Problem Processing Request, Try again or Contact Administrator.";
            return View("Add", mod_mcc);
            //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }


        public async Task<PartialViewResult> ViewDetail(int id = 0, string m = null)
        {
            try
            {

                ViewBag.MenuId = m;

                var inst = await _repo.GetInstitutionAsync(0, true, "Active");
                var sector = await _repo.GetSectorAsync(0, true, "Active");
                ViewBag.Sector = new SelectList(sector, "SECTOR_CODE", "SECTOR_NAME");
                ViewBag.Acquirer = new SelectList(inst, "CBN_CODE", "INSTITUTION_NAME");

                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add MCC";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    return PartialView("ViewDetail", new MCCObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit MCC";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetMCCAsync(id, false);
                    if (rec == null)
                    {

                        // return Json(null, JsonRequestBehavior.AllowGet);
                        //var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                        // return Json(obj, JsonRequestBehavior.AllowGet);
                        return null;


                    }
                    var model = rec.FirstOrDefault();
                    var recMCC = BindScheme("000", model.MCC_CODE);
                    ViewBag.MCCMSC = recMCC;
                    return PartialView("ViewDetail", model);

                }
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                return null;
            }
        }

        void BindComboMsc()
        {
            var cardscheme =  _repo.GetCardScheme(0, true, "Active");
            var ChannelList =  _repo.GetChannel(0, true, "Active");
            var currency =  _repo.GetCurrency(0, true, "Active");
            var calc_basis = SmartObj.GetCalculationBasis();
            ViewBag.CardSchemeList = new SelectList(cardscheme, "CARDSCHEME", "CARDSCHEME_DESC");
            ViewBag.Channel = new SelectList(ChannelList, "CODE", "DESCRIPTION");
            ViewBag.CurrencyList = new SelectList(currency, "CURRENCY_CODE", "CURRENCY_NAME");
            ViewBag.CalcBasis = new SelectList(calc_basis, "Code", "Description");
        }
        public async Task<PartialViewResult> AddMsc(decimal id = 0,string m = null)
        {
            try
            {
                ViewBag.MenuId = m;

                BindComboMsc();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add MSC";
                    ViewBag.ButtonText = "Add";
                    return PartialView("_AddMsc", new MccMscObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit MSC";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetMCCAsync(0, false);
                    if (rec == null)
                    {
                        return null;
                    }
                    var model = rec.FirstOrDefault();
                    return PartialView("_AddMsc", model);

                }
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                return null;
            }
        }
        public async Task<ActionResult> GetAcquirerMsc(string mcc, string cbn)
        {
            var htmlString = "";
            try
            {
                //ViewBag.MenuId = m;
                var rec = await BindScheme(mcc, cbn);
                 htmlString = PartialView("_ViewAcqMSC", rec).RenderToString();
                return Json(new {data_msc = htmlString, RespCode = 0,RespMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                htmlString = PartialView("_ViewAcqMSC").RenderToString();
                return Json(new { data_msc = htmlString, RespCode = 1, RespMessage = "Problem Processing Request" }, JsonRequestBehavior.AllowGet);

            }
        }
        
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddMSC(MccMscObj model, string CBN_CODE, string m)
        {
            string msg = "";
            try
            {
                var lst = GetMccMscLines().ToList();
                if (model.ITBID == 0)
                {
                    var exist = lst.Exists(r => r.CARDSCHEME == model.CARDSCHEME && r.CHANNEL == model.CHANNEL);
                    if (exist)
                    {
                        // Party Type already exist. Duplicate Record is not allowed.;

                        msg = "Card Scheme and Channel already exist for the selected Acquirer. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    decimal itbid = 0;
                    if (lst.Count == 0)
                    {

                        itbid = 1;
                    }
                    else
                    {
                        var itb = lst.Max(f => f.ITBID);
                        itbid = itb + 1;
                    }
                    var domCur =repoCurrency.AllEager(d => d.CURRENCY_CODE == model.DOM_SETTLEMENT_CURRENCY).FirstOrDefault();
                    if(domCur != null)
                    {
                        model.DomCurrencyDesc = domCur.CURRENCY_NAME;
                    }
                    var intCur = repoCurrency.AllEager(d => d.CURRENCY_CODE == model.INT_SETTLEMENT_CURRENCY).FirstOrDefault();
                    if (intCur != null)
                    {
                        model.IntCurrencyDesc = intCur.CURRENCY_NAME;
                    }
                    var domFreq = repoFreq.AllEager(d => d.ITBID == model.DOM_FREQUENCY).FirstOrDefault();
                    if (domFreq != null)
                    {
                        model.DomFrequencyDesc = domFreq.FREQUENCY_DESC;
                    }
                    var intFreq = repoFreq.AllEager(d => d.ITBID == model.DOM_FREQUENCY).FirstOrDefault();
                    if (intFreq != null)
                    {
                        model.IntFrequencyDesc = intFreq.FREQUENCY_DESC;
                    }
                    var channel = repoChannel.AllEager(d => d.CODE == model.CHANNEL).FirstOrDefault();
                    if (channel != null)
                    {
                        model.ChannelDesc = channel.DESCRIPTION;
                    }
                   
                    var obj = new MccMscObj()
                    {
                        CHANNEL = model.CHANNEL,
                        CBN_CODE = model.CBN_CODE,
                        ChannelDesc = model.ChannelDesc,
                        CARDSCHEME = model.CARDSCHEME,
                        CARDSCHEME_DESC = model.CARDSCHEME_DESC,
                        DomCurrencyDesc = model.DomCurrencyDesc,
                        DomFrequencyDesc = model.DomFrequencyDesc,
                        DOM_FREQUENCY = model.DOM_FREQUENCY,
                        DOM_MSCVALUE = model.DOM_MSCVALUE,
                        DOM_SETTLEMENT_CURRENCY = model.DOM_SETTLEMENT_CURRENCY,
                        IntCurrencyDesc = model.IntCurrencyDesc,
                        IntFrequencyDesc = model.IntFrequencyDesc,
                        INT_FREQUENCY = model.INT_FREQUENCY,
                        INT_MSCVALUE = model.INT_MSCVALUE,
                        INT_SETTLEMENT_CURRENCY = model.INT_SETTLEMENT_CURRENCY,
                        ITBID = itbid,
                        NewRecord = true,
                        STATUS = inActive,
                        DOMCAP = model.DOMCAP,
                        INTLCAP = model.INTLCAP,
                        INTMSC_CALCBASIS = model.INTMSC_CALCBASIS,
                        MSC_CALCBASIS = model.MSC_CALCBASIS,
                    };

                    SessionHelper.GetMccMsc(Session).AddItem(obj);
                    var w = GetMccMscLines().ToList();
                    var html = PartialView("_ViewAcqMSC",w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data = w, data_msc = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                    var existRec = lst.Where(r => r.CARDSCHEME == model.CARDSCHEME && r.CHANNEL == model.CHANNEL).FirstOrDefault();
                    if (existRec!= null) // not expected to be null
                    {
                        if (oldRec.ITBID != existRec.ITBID)
                        {
                            msg = "Card Scheme and Channel already exist for the selected Acquirer. Duplicate Record is not allowed.";
                            return Json(new {  RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    var domCur = repoCurrency.AllEager(d => d.CURRENCY_CODE == model.DOM_SETTLEMENT_CURRENCY).FirstOrDefault();
                    if (domCur != null)
                    {
                        model.DomCurrencyDesc = domCur.CURRENCY_NAME;
                    }
                    var intCur = repoCurrency.AllEager(d => d.CURRENCY_CODE == model.INT_SETTLEMENT_CURRENCY).FirstOrDefault();
                    if (intCur != null)
                    {
                        model.IntCurrencyDesc = intCur.CURRENCY_NAME;
                    }
                    var domFreq = repoFreq.AllEager(d => d.ITBID == model.DOM_FREQUENCY).FirstOrDefault();
                    if (domFreq != null)
                    {
                        model.DomFrequencyDesc = domFreq.FREQUENCY_DESC;
                    }
                    var intFreq = repoFreq.AllEager(d => d.ITBID == model.DOM_FREQUENCY).FirstOrDefault();
                    if (intFreq != null)
                    {
                        model.IntFrequencyDesc = intFreq.FREQUENCY_DESC;
                    }
                    var channel = repoChannel.AllEager(d => d.CODE == model.CHANNEL).FirstOrDefault();
                    if (channel != null)
                    {
                        model.ChannelDesc = channel.DESCRIPTION;
                    }
                    var obj = new MccMscObj()
                    {
                        CARDSCHEME = model.CARDSCHEME,
                        CARDSCHEME_DESC = model.CARDSCHEME_DESC,
                        DomCurrencyDesc = model.DomCurrencyDesc,
                        DomFrequencyDesc = model.DomFrequencyDesc,
                        DOM_FREQUENCY = model.DOM_FREQUENCY,
                        DOM_MSCVALUE = model.DOM_MSCVALUE,
                        CBN_CODE = model.CBN_CODE,
                        DOM_SETTLEMENT_CURRENCY = model.DOM_SETTLEMENT_CURRENCY,
                        IntCurrencyDesc = model.IntCurrencyDesc,
                        IntFrequencyDesc = model.IntFrequencyDesc,
                        INT_FREQUENCY = model.INT_FREQUENCY,
                        INT_MSCVALUE = model.INT_MSCVALUE,
                        INT_SETTLEMENT_CURRENCY = model.INT_SETTLEMENT_CURRENCY,
                        ITBID = model.ITBID,
                        DOMCAP = model.DOMCAP,
                        INTLCAP = model.INTLCAP,
                        NewRecord = oldRec.NewRecord,
                        Updated = true,
                        INTMSC_CALCBASIS = model.INTMSC_CALCBASIS,
                        MSC_CALCBASIS = model.MSC_CALCBASIS,
                        CHANNEL = model.CHANNEL,
                        ChannelDesc = model.ChannelDesc

                    };
                    SessionHelper.GetMccMsc(Session).UpdateItem(obj);

                    var w = GetMccMscLines().ToList();
                    var html = PartialView("_ViewAcqMSC", w).RenderToString();
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
        async Task<List<MccMscObj>> BindScheme(string mcc_code, string cbn_code)
        {
            SessionHelper.GetMccMsc(Session).Clear();
            var rec = await _repo.GetAcquirerMCCAsync(cbn_code, mcc_code);
            ViewBag.AcqId = cbn_code;
            foreach(var d in rec)
            {
                SessionHelper.GetMccMsc(Session).AddItem(d);
            }
            return rec;

        }

        List<MccMscObj> BindSchemeTemp(string mcc_code, string batchId)
        {
            // SessionHelper.GetMccMsc(Session).Clear();
            var rec =  _repo.GetAcquirerMCC_Temp(batchId, mcc_code);
            //ViewBag.AcqId = cbn_code;
            if(rec != null && rec.Count > 0)
            {
                ViewBag.AcquirerName = rec[0].INSTITUTION_NAME;
            }
            return rec;

        }

        public ActionResult EditMsc(decimal id,string m)
        {
            try
            {
                ViewBag.MenuId = m;
              
                var lst = GetMccMscLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    BindComboMsc();
                    ViewBag.ButtonText = "Update";
                    return PartialView("_AddMSC", rec);
                }
            }
            catch
            {

            }
            return null;
        }
        public ActionResult DeleteMsc(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetMccMscLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    if (rec.NewRecord)
                    {
                        SessionHelper.GetMccMsc(Session).RemoveLine(rec.ITBID);
                       
                    }
                    else
                    {
                        SessionHelper.GetMccMsc(Session).MarkForDelete(rec.ITBID);

                        //var editBtn = (LinkButton)e.Item.FindControl("btnEdit");
                        //var deleteBtn = (LinkButton)e.Item.FindControl("btnDelete");
                        //var undoBtn = (LinkButton)e.Item.FindControl("btnUndo");
                        //var rdDefault = (CheckBox)e.Item.FindControl("rdDefault");

                    }
                    var lst2 = GetMccMscLines().ToList();
                    var html2 = PartialView("_ViewAcqMsc", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_msc = html2 }, JsonRequestBehavior.AllowGet) ;
                }
            }
            catch
            {

            }
            var lst3 = GetMccMscLines().ToList();
            var html = PartialView("_ViewAcqMsc", lst3).RenderToString();
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_msc = html }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UndoMsc(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetMccMscLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    SessionHelper.GetMccMsc(Session).UndoDelete(rec.ITBID);

                    var lst2 = GetMccMscLines().ToList();
                    var html2 = PartialView("_ViewAcqMsc", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_msc = html2 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetMccMscLines().ToList();
            var html3 = PartialView("_ViewAcqMsc", lst3).RenderToString();
            return Json(new { RespCode = 0, RespMessage = "", data_msc = html3 }, JsonRequestBehavior.AllowGet);
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
                        var status = TempData["status"];
                        var stat = status == null ? "open" : status.ToString();
                        var rec = _repo.GetMCC((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            ViewBag.MCCMSC = BindSchemeTemp(model.MCC_CODE, model.BATCHID);
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
                                recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                        {

                                            suc = CreateNewRecord(recordId, rec2.BATCHID, rec2.USERID); ;
                                            break;
                                        }
                                    case "Modify":
                                        {
                                            suc = ModifyMainRecord(recordId, rec2.BATCHID, rec2.USERID);
                                            break;
                                        }
                                    case "CLOSE":
                                        {
                                            suc = CloseMainRecord(recordId, rec2.BATCHID, rec2.USERID);
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

        private bool CloseMainRecord(int recordId, string bATCHID, string uSERID)
        {
            //var curDate = DateTime.Now;
            var dt = repoMccTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == uSERID.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoMcc.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
                dt.STATUS = approve;
                if (dm != null)
                {
                    dm.STATUS = close;
                    return true;
                }

            }
            return false;
        }

        private bool CreateNewRecord(int recordId, string batchId, string user_id)
        {
            DateTime curDate = DateTime.Now;
            var dt = repoMccTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                dt.STATUS = approve;
                var obj = new SM_MCC()
                {
                    MCC_CODE = dt.MCC_CODE.ToUpper(),
                    MCC_DESC = dt.MCC_DESC,
                    MCC_CAT = dt.MCC_CAT,
                    SECTOR_CODE = dt.SECTOR_CODE,
                    STATUS = active,
                    CREATEDATE = DateTime.Now,
                    USERID = dt.USERID,
                };

                repoMcc.Insert(obj);
                var ac = repoMccMscTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper() && e.MCC_CODE == dt.MCC_CODE).ToList();
                if (ac.Count > 0)
                {
                    foreach (var d in ac)
                    {
                        d.STATUS = approve;
                        var obj2 = new SM_MCCMSC()
                        {
                            CARDSCHEME = d.CARDSCHEME,
                            MSC_CALCBASIS = d.MSC_CALCBASIS, //drpCalcBasis.SelectedValue,
                            BATCHID = batchId,
                            DOM_FREQUENCY = d.DOM_FREQUENCY,
                            DOM_MSCVALUE = d.DOM_MSCVALUE,
                            DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY,
                            INT_FREQUENCY = d.INT_FREQUENCY,
                            INT_MSCVALUE = d.INT_MSCVALUE,
                            INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY,
                            MCC_CODE = dt.MCC_CODE,
                            DOMCAP = d.DOMCAP ?? 0,
                            INTLCAP = d.INTLCAP,
                            STATUS = active,
                            CREATEDATE = curDate,
                            USERID = user_id,
                            CBN_CODE = d.CBN_CODE,
                            INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                            CHANNEL = d.CHANNEL,
                        };
                        obj.SM_MCCMSC.Add(obj2);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId, string batchId, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoMccTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoMcc.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
                dt.STATUS = approve;

                if (dm != null)
                {
                  //  dm.MCC_DESC = dt.MCC_CODE;
                    dm.MCC_DESC = dt.MCC_DESC.ToUpper();
                    dm.MCC_CAT = dt.MCC_CAT;
                    dm.SECTOR_CODE = dt.SECTOR_CODE;
                    dm.LAST_MODIFIED_UID = dt.USERID;
                    dm.STATUS = active;

                    var ac = repoMccMscTemp.AllEager(e => e.BATCHID == batchId && e.MCC_CODE == dm.MCC_CODE && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).ToList();
                    if (ac.Count > 0)
                    {
                        foreach (var d in ac)
                        {
                            d.STATUS = close;
                            if (d.EVENTTYPE == eventInsert || d.EVENTTYPE == eventEdit)
                            {
                                if (d.RECORDID == 0)
                                {
                                    var dm2 = repoMccMsc.AllEager(e => e.MCC_CODE == dm.MCC_CODE && e.CARDSCHEME == d.CARDSCHEME && e.CBN_CODE == d.CBN_CODE && e.CHANNEL == d.CHANNEL).FirstOrDefault();
                                    if (dm2 == null)
                                    {
                                        var obj2 = new SM_MCCMSC()
                                        {
                                            CARDSCHEME = d.CARDSCHEME,
                                            MSC_CALCBASIS = d.MSC_CALCBASIS, //drpCalcBasis.SelectedValue,
                                            BATCHID = batchId,
                                            DOM_FREQUENCY = d.DOM_FREQUENCY,
                                            DOM_MSCVALUE = d.DOM_MSCVALUE,
                                            DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY,
                                            INT_FREQUENCY = d.INT_FREQUENCY,
                                            INT_MSCVALUE = d.INT_MSCVALUE,
                                            INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY,
                                            MCC_CODE = dt.MCC_CODE,
                                            DOMCAP = d.DOMCAP ?? 0,
                                            INTLCAP = d.INTLCAP,
                                            STATUS = active,
                                            CREATEDATE = DateTime.Now,
                                            USERID = user_id,
                                            CBN_CODE = d.CBN_CODE,
                                            INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                                            CHANNEL = d.CHANNEL
                                        };
                                        dm.SM_MCCMSC.Add(obj2);
                                    }
                                    else
                                    {
                                        dm2.DOMCAP = d.DOMCAP ?? 0;
                                        dm2.CARDSCHEME = d.CARDSCHEME.ToUpper();
                                        dm2.BATCHID = d.BATCHID;
                                        dm2.DOM_FREQUENCY = d.DOM_FREQUENCY;
                                        dm2.DOM_MSCVALUE = d.DOM_MSCVALUE;
                                        dm2.DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY;
                                        dm2.INTLCAP = d.INTLCAP;
                                        dm2.INT_FREQUENCY = d.INT_FREQUENCY;
                                        dm2.INT_MSCVALUE = d.INT_MSCVALUE;
                                        dm2.INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY;
                                        dm2.MSC_CALCBASIS = d.MSC_CALCBASIS;
                                        dm2.LAST_MODIFIED_UID = user_id;
                                        dm2.CBN_CODE = d.CBN_CODE;
                                        dm2.INTMSC_CALCBASIS = d.INTMSC_CALCBASIS;
                                        dm2.CHANNEL = d.CHANNEL;
                                    }
                                }
                                else
                                {
                                    var dm2 = repoMccMsc.AllEager(e => e.MCC_CODE == dm.MCC_CODE && e.CARDSCHEME == d.CARDSCHEME && e.CBN_CODE == d.CBN_CODE && e.CHANNEL == d.CHANNEL).FirstOrDefault();
                                    if (dm2 != null)
                                    {
                                        dm2.DOMCAP = d.DOMCAP ?? 0;
                                        dm2.CARDSCHEME = d.CARDSCHEME.ToUpper();
                                        dm2.BATCHID = d.BATCHID;
                                        dm2.DOM_FREQUENCY = d.DOM_FREQUENCY;
                                        dm2.DOM_MSCVALUE = d.DOM_MSCVALUE;
                                        dm2.DOM_SETTLEMENT_CURRENCY = d.DOM_SETTLEMENT_CURRENCY;
                                        dm2.INTLCAP = d.INTLCAP;
                                        dm2.INT_FREQUENCY = d.INT_FREQUENCY;
                                        dm2.INT_MSCVALUE = d.INT_MSCVALUE;
                                        dm2.INT_SETTLEMENT_CURRENCY = d.INT_SETTLEMENT_CURRENCY;
                                        dm2.MSC_CALCBASIS = d.MSC_CALCBASIS;
                                        dm2.LAST_MODIFIED_UID = user_id;
                                        dm2.CBN_CODE = d.CBN_CODE;
                                        dm2.INTMSC_CALCBASIS = d.INTMSC_CALCBASIS;
                                        dm2.CHANNEL = d.CHANNEL;

                                    }
                                }
                            }
                            else
                            {
                                var dm2 = repoMccMsc.AllEager(e => e.MCC_CODE == dm.MCC_CODE && e.CARDSCHEME == d.CARDSCHEME && e.CBN_CODE == d.CBN_CODE && e.CHANNEL == d.CHANNEL).FirstOrDefault();
                                if(dm2 != null)
                                {
                                    repoMccMsc.Delete(dm2.ITBID);
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
                                    //txscope.Complete();

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
            var recP = repoMccTemp.AllEager(d => d.ITBID == rECORDID && d.USERID == uSERID).FirstOrDefault();
            if (recP != null)
            {
                recP.STATUS = reject;
            }

            var recPP = repoMccMscTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
            foreach (var d in recPP)
            {
                d.STATUS = reject;
            }

        }

        public IList<MccMscObj> GetMccMscLines()
        {
            //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
           return  SessionHelper.GetMccMsc(Session).Lines;
        }

    }
}