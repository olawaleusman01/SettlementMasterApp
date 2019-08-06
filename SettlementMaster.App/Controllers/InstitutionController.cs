using Generic.Dapper.Data;
using Generic.Dapper.Model;
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
    public class InstitutionController : Controller
    {
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
            private readonly IRepository<SM_AUTHLIST> repoAuth = null;

            //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
            private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
            private readonly IRepository<SM_INSTITUTION> repoInst = null;
            private readonly IRepository<SM_INSTITUTIONACCT> repoInstAcct = null;
            private readonly IRepository<SM_INSTITUTIONACCTTEMP> repoInstAcctTemp = null;
            private readonly IRepository<SM_INSTITUTIONTEMP> repoInstTemp = null;
            private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
            //private readonly IRepository<SM_CURRENCY> repoCurrency = null;
            //private readonly IRepository<SM_FREQUENCY> repoFreq = null;
            //private readonly IRepository<SM_SECTOR> repoSec = null;
            //private readonly IRepository<SM_CHANNELS> repoChannel = null;

            //private readonly IRepository<SM_INSTITUTIONCATEGORY> repoMC = null;

            //private readonly IRepository<SM_INSTITUTION> repoInst = null;

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
            int menuId = 21;
            int institutionId;
            int roleId;
            int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public InstitutionController()
        {
            uow = new UnitOfWork();
            repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoInstAcct = new Repository<SM_INSTITUTIONACCT>(uow);
            repoInstTemp = new Repository<SM_INSTITUTIONTEMP>(uow);
            repoInstAcctTemp = new Repository<SM_INSTITUTIONACCTTEMP>(uow);
            //repoFreq = new Repository<SM_FREQUENCY>(uow);
            //repoSec = new Repository<SM_SECTOR>(uow);
            // repoMC = new Repository<SM_INSTITUTIONCATEGORY>(uow);
            //repoVal = new Repository<SM_MERCHANTCONFIG>(uow);
            //repoInst = new Repository<SM_INSTITUTION>(uow);
            //repoChannel = new Repository<SM_CHANNELS>(uow);

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
            // GetMenuId();
            try
            {
                SessionHelper.GetBanks(Session).Clear();
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

        public async Task<ActionResult> InstitutionList()
            {
                try
                {
                    var rec = await _repo.GetInstitutionAsync(0, true);  //repoSession.FindAsync(id);              
                    return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var obj1 = new { data = new List<InstitutionObj>(), RespCode = 2, RespMessage = ex.Message };
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
        public ActionResult Add(InstitutionObj model, string m)
        {
            
            try
            {
                if (model.ITBID == 0)
                {
                    ViewBag.ButtonText = "Save";
                    ViewBag.HeaderTitle = "Add Institution";
                }
                else
                {
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Institution";
                }

                ViewBag.MenuId = m;
                menuId = SmartUtil.GetMenuId(m);
                string bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                model.CBN_CODE = model.CBN_CODE != null ? model.CBN_CODE.Trim() : "";
                model.INSTITUTION_SHORTCODE = model.INSTITUTION_SHORTCODE != null ? model.INSTITUTION_SHORTCODE.Trim() : "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID == 0)
                    {
                        var errMsg = "";

                        if (validateForm(model, eventInsert, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            BindState(model.INSTITUTION_COUNTRY);
                            BindCity(model.INSTITUTION_COUNTRY, model.INSTITUTION_STATE);
                            var recAcct = GetBanksLines().ToList();
                            ViewBag.InstitutionAcct = recAcct;
                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        SM_INSTITUTIONTEMP BType = new SM_INSTITUTIONTEMP()
                        {
                            // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                            INSTITUTION_ADDRESS = model.INSTITUTION_ADDRESS,
                            BANK_SECTOR = model.BANK_SECTOR,
                            CBN_CODE = model.CBN_CODE,
                            EMAIL = model.EMAIL,
                            INSTITUTION_CITY = model.INSTITUTION_CITY,
                            INSTITUTION_COUNTRY = model.INSTITUTION_COUNTRY,
                            INSTITUTION_NAME = model.INSTITUTION_NAME,
                            INSTITUTION_SHORTCODE = model.INSTITUTION_SHORTCODE,
                            INSTITUTION_SLOGAN = model.INSTITUTION_SLOGAN,
                            INSTITUTION_STATE = model.INSTITUTION_STATE,
                            INSTITUTION_URL = model.INSTITUTION_URL,
                            IS_ACQUIRER = model.IS_ACQUIRER, // ? "Y" : "N",
                            IS_BANK = model.IS_BANK, // ? "Y" : "N",
                            PTSP = model.PTSP, // ? "Y" : "N",
                            ISSUER_NIBSSACCOUNT = model.ISSUER_NIBSSACCOUNT,
                            PHONENO = model.PHONENO,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            BATCHID = bid,
                            USERID = User.Identity.Name,
                            RECORDID = model.ITBID,

                        };

                        repoInstTemp.Insert(BType);
                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst)
                        {
                            SaveAcctDetailTemp(eventEdit, BType);

                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
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
                                SessionHelper.GetBanks(Session).Clear();
                                //txscope.Complete();
                                TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return RedirectToAction("Index", "Institution", new { m = m });
                                //EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");
                            }
                        }

                    }
                    else
                    {
                        var errMsg = "";
                        if (validateForm(model, eventEdit, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            ViewBag.StatusVisible = true;
                            var recAcct = GetBanksLines().ToList();
                            ViewBag.InstitutionAcct = recAcct;
                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{

                        SM_INSTITUTIONTEMP BType = new SM_INSTITUTIONTEMP()
                        {
                            // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                            INSTITUTION_ADDRESS = model.INSTITUTION_ADDRESS,
                            BANK_SECTOR = model.BANK_SECTOR,
                            CBN_CODE = model.CBN_CODE,
                            EMAIL = model.EMAIL,
                            INSTITUTION_CITY = model.INSTITUTION_CITY,
                            INSTITUTION_COUNTRY = model.INSTITUTION_COUNTRY,
                            INSTITUTION_NAME = model.INSTITUTION_NAME,
                            ISSUER_NIBSSACCOUNT = model.ISSUER_NIBSSACCOUNT,
                            INSTITUTION_SHORTCODE = model.INSTITUTION_SHORTCODE,
                            INSTITUTION_SLOGAN = model.INSTITUTION_SLOGAN,
                            INSTITUTION_STATE = model.INSTITUTION_STATE,
                            INSTITUTION_URL = model.INSTITUTION_URL,
                            STATUS = open,
                            IS_ACQUIRER = model.IS_ACQUIRER, // ? "Y" : "N",
                            IS_BANK = model.IS_BANK, // ? "Y" : "N",
                            PTSP = model.PTSP, // ? "Y" : "N",
                            PHONENO = model.PHONENO,
                            CREATEDATE = DateTime.Now,
                            BATCHID = bid,
                            USERID = User.Identity.Name,
                            RECORDID = model.ITBID,
                        };
                        repoInstTemp.Insert(BType);
                        //  var rst1 = new AuthListUtil().SaveLog(auth);
                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                        if (rst)
                        {
                            SaveAcctDetailTemp(eventEdit, BType);
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
                                POSTTYPE = Single,
                                INSTITUTION_ITBID = institutionId,
                                BATCHID = bid,

                            };

                            repoAuth.Insert(auth);
                            if (uow.Save(User.Identity.Name) > 0)
                            {
                                SessionHelper.GetBanks(Session).Clear();
                                //txscope.Complete();
                                TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                return RedirectToAction("Index", "Institution", new { m = m });
                                //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                                // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                            }
                        }
                    }
                }

                   // }
                    // If we got this far, something failed, redisplay form
                    //return Json(new { RespCode = 1, RespMessage = errorMsg });
                    BindCombo();
                    BindState(model.INSTITUTION_COUNTRY);
                    BindCity(model.INSTITUTION_COUNTRY, model.INSTITUTION_STATE);
                    ViewBag.InstitutionAcct = GetBanksLines();
                    ViewBag.Message = errorMsg;
                    return View("Add", model);
               // }
            }
            catch (SqlException ex)
            {
                BindCombo();
                BindState(model.INSTITUTION_COUNTRY);
                BindCity(model.INSTITUTION_COUNTRY, model.INSTITUTION_STATE);
                ViewBag.InstitutionAcct = GetBanksLines();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                BindCombo();
                BindState(model.INSTITUTION_COUNTRY);
                BindCity(model.INSTITUTION_COUNTRY, model.INSTITUTION_STATE);
                ViewBag.InstitutionAcct = GetBanksLines();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            BindCombo();
            ViewBag.InstitutionAcct = GetBanksLines();
            ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            return View("Add", model);
            //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }
        private bool validateForm(InstitutionObj obj,string eventType, out string errorMsg)
        {
            var sb = new StringBuilder();
            var errCount = 0;
            if (eventType == eventInsert)
            {
                var existCbnCode = repoInst.AllEager(f => f.CBN_CODE != null && f.CBN_CODE == obj.CBN_CODE).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"""CBN CODE"" already exist");
                    errCount++;
                }
                var existShortName = repoInst.AllEager(f => f.INSTITUTION_SHORTCODE != null && f.INSTITUTION_SHORTCODE.Trim().ToLower() == obj.INSTITUTION_SHORTCODE).FirstOrDefault();
                if (existShortName != null)
                {
                    sb.AppendLine(@"""INSTITUTION SHORTCODE"" already exist");
                    errCount++;
                }

                if (errCount > 0)
                {
                    errorMsg = sb.ToString();
                    return true;
                }
                errorMsg = sb.ToString();
                return false;
            }
            else
            {
                var existCbnCode = repoInst.AllEager(f => f.ITBID != obj.ITBID && f.CBN_CODE != null && f.CBN_CODE == obj.CBN_CODE ).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"""CBN CODE"" already exist");
                    errCount++;
                }
                var existShortName = repoInst.AllEager(f => f.ITBID != obj.ITBID && f.INSTITUTION_SHORTCODE != null && f.INSTITUTION_SHORTCODE.Trim().ToLower() == obj.INSTITUTION_SHORTCODE).FirstOrDefault();
                if (existShortName != null)
                {
                    sb.AppendLine(@"""INSTITUTION SHORTCODE"" already exist");
                    errCount++;
                }

                if (errCount > 0)
                {
                    errorMsg = sb.ToString();
                    return true;
                }
                errorMsg = sb.ToString();
                return false;
            }
        }

        void SaveAcctDetailTemp(string eventType, SM_INSTITUTIONTEMP rec)
            {
                if (eventType == "New")
                {
                    var col = GetBanksLines();
                    foreach (var d in col)
                    {
                        var obj = new SM_INSTITUTIONACCTTEMP()
                        {
                            DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                            BATCHID = rec.BATCHID,
                            DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                            DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDRESS,
                            DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                            DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                            DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                            INSTITUTION_ITBID = 0,
                            CARDSCHEME = d.CARDSCHEME,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            EVENTTYPE = eventInsert,
                        };
                        repoInstAcctTemp.Insert(obj);
                    }

                }
                else
                {
                    var col = GetBanksLines();
                    foreach (var d in col)
                    {
                        //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                        //{
                        //    continue;
                        //}
                        if (d.NewRecord || d.Updated || d.Deleted)
                        {
                            var obj = new SM_INSTITUTIONACCTTEMP()
                            {
                                DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                                BATCHID = rec.BATCHID,
                                DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                                DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                                DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDRESS,
                                DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                                DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                                DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                                CARDSCHEME = d.CARDSCHEME,
                                INSTITUTION_ITBID = rec.ITBID,
                                STATUS = open,
                                RECORDID = d.NewRecord ? 0 : d.ITBID,
                                CREATEDATE = DateTime.Now,
                                USERID = User.Identity.Name,
                                EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,

                            };

                            repoInstAcctTemp.Insert(obj);
                        }
                    }
                }

            }
        void BindCombo(string acq_selected = null)
        {
            var btype = _repo.GetBankType(0, true, "Active");
            var country = _repo.GetCountry(0, true, "Active");

            ViewBag.BType = new SelectList(btype, "ITBID", "BANKTYPENAME");
            ViewBag.CountryList = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");

            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

        }
        void BindState(string countryCode)
        {
            try
            {
                var state = _repo.GetStateFilter(countryCode);

                ViewBag.StateList = new SelectList(state, "STATECODE", "STATENAME");
            }
            catch
            {

            }
        }
        void BindCity(string countryCode,string stateCode)
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
        public async Task<ActionResult> Add(int id = 0, string m = null)
        {
            try
            {
                SessionHelper.GetBanks(Session).Clear();
                ViewBag.MenuId = HttpUtility.UrlDecode(m);
                BindCombo();
                if (id == 0)
                {
                    BindState("NGN");
                    BindCity(null, null);
                    ViewBag.HeaderTitle = "Add Institution";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add", new InstitutionObj() { INSTITUTION_COUNTRY = "NGN" });

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Institution";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetInstitutionAsync(id, false);
                    if (rec == null)
                    {
                        //bad request
                        // return Json(null, JsonRequestBehavior.AllowGet);
                        //var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                        // return Json(obj, JsonRequestBehavior.AllowGet);
                        ViewBag.Message = "Record Not Found";
                        return View("Add");
                    }
                    var model = rec.FirstOrDefault();
                    BindState(model.INSTITUTION_COUNTRY);
                    BindCity(model.INSTITUTION_COUNTRY, model.INSTITUTION_CITY);
                    var recAcct = await BindAccount(model.ITBID);
                    ViewBag.InstitutionAcct = recAcct;
                    GetPriv();
                    return View("Add", model);

                }
            }
            catch (Exception ex)
            {
                BindState("NGN");
                BindCity(null, null);
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                ViewBag.Message = ex.Message;
                return View("Add");
            }
        }

            void BindComboMsc()
            {
                var bankList = _repo.GetInstitution(0, true, "Active").Where(f => f.IS_BANK == "Y").ToList();
                //var country = _repo.GetCountry(0, true, "Active");

                ViewBag.BankList = new SelectList(bankList, "CBN_CODE", "INSTITUTION_NAME");
            //ViewBag.Country = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
            var schemeList = _repo.GetCardScheme(0, true, "Active"); //.Select(f=> new { CARDSCHEME_CODE = f.CARDSCHEME, f.CARDSCHEME_DESC }).ToList();
            //var country = _repo.GetCountry(0, true, "Active");

            ViewBag.CardSchemeList = new SelectList(schemeList, "CARDSCHEME", "CARDSCHEME_DESC");
        }
            public PartialViewResult AddAcct(decimal id = 0, string m = null)
            {
                try
                {
                    ViewBag.MenuId = m;

                    BindComboMsc();
                    if (id == 0)
                    {
                        ViewBag.HeaderTitle = "Add Account";
                        ViewBag.ButtonText = "Add";
                        return PartialView("_AddAcct", new InstitutionAcctObj());
                        // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //var d = _repo.GetSession(0, true);
                        ViewBag.HeaderTitle = "Edit Account";
                        ViewBag.StatusVisible = true;
                        ViewBag.ButtonText = "Update";
                        //var rec = await _repo.GetBanksAsync(id);
                        //if (rec == null)
                        //{
                        //    return null;
                        //}
                        //var model = rec.FirstOrDefault();
                        return PartialView("_AddAcct", null);

                    }
                }
                catch (Exception ex)
                {
                    //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                    //return Json(obj1, JsonRequestBehavior.AllowGet);
                    return null;
                }
            }
        //public async Task<ActionResult> GetAcquirerMsc(string mcc, string cbn)
        //{
        //    var htmlString = "";
        //    try
        //    {
        //        //ViewBag.MenuId = m;
        //        var rec = await BindScheme(mcc, cbn);
        //        htmlString = PartialView("_ViewAcct", rec).RenderToString();
        //        return Json(new { data_msc = htmlString, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        htmlString = PartialView("_ViewAcct").RenderToString();
        //        return Json(new { data_msc = htmlString, RespCode = 1, RespMessage = "Problem Processing Request" }, JsonRequestBehavior.AllowGet);

        //    }
        //}

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddAcct(InstitutionAcctObj model, string m)
        {
            string msg = "";
            var bankName = "";
            var cntry_code = "";
            var bankAddress = "";
            var schemedesc = "";
            try
            {
                var lst = GetBanksLines().ToList();
                if (model.ITBID == 0)
                {
                    var exist = lst.Exists(r => r.DEPOSIT_BANKCODE == model.DEPOSIT_BANKCODE && r.DEPOSIT_ACCOUNTNO == model.DEPOSIT_ACCOUNTNO && r.CARDSCHEME == model.CARDSCHEME);
                    if (exist)
                    {
                        msg = "Account No and Selected Bank Already exist for selected Card Scheme. Duplicate Record is not allowed.";
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

                    var inst = repoInst.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                    if (inst != null)
                    {
                        bankName = inst.INSTITUTION_NAME;
                        cntry_code = inst.INSTITUTION_COUNTRY;
                        bankAddress = inst.INSTITUTION_ADDRESS;
                    }
                    var scheme = repoScheme.AllEager(d => d.CARDSCHEME == model.CARDSCHEME).FirstOrDefault();
                    if (scheme != null)
                    {
                        schemedesc = scheme.CARDSCHEME_DESC;
                    }
                    var obj = new InstitutionAcctObj()
                    {
                        DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_BANKADDRESS = bankAddress,
                        DEPOSIT_BANKNAME = bankName,
                        DEPOSIT_COUNTRYCODE = cntry_code,

                        // DEPOSIT_COUNTRYCODE = drpcountrycode.SelectedValue,
                        DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                        CARDSCHEME = model.CARDSCHEME,
                        CardSchemDesc = schemedesc,
                        DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                        DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                        USERID = User.Identity.Name,
                        ITBID = itbid,
                        NewRecord = true,
                    };


                    SessionHelper.GetBanks(Session).AddItem(obj);
                    var w = GetBanksLines().ToList();
                    var html = PartialView("_ViewAcct", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                    var existRec = lst.Where(r => r.DEPOSIT_BANKCODE == model.DEPOSIT_BANKCODE && r.DEPOSIT_ACCOUNTNO == model.DEPOSIT_ACCOUNTNO && r.CARDSCHEME == model.CARDSCHEME).FirstOrDefault();
                    if (existRec != null) // not expected to be null
                    {
                        if (oldRec.ITBID != existRec.ITBID)
                        {
                            msg = "Account No and Selected Bank Already exist for selected Card Scheme. Duplicate Record is not allowed.";
                            return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    var scheme = repoScheme.AllEager(d => d.CARDSCHEME == model.CARDSCHEME).FirstOrDefault();
                    if (scheme != null)
                    {
                        schemedesc = scheme.CARDSCHEME_DESC;
                    }
                    var inst = repoInst.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                    if (inst != null)
                    {
                        bankName = inst.INSTITUTION_NAME;
                        cntry_code = inst.INSTITUTION_COUNTRY;
                        bankAddress = inst.INSTITUTION_ADDRESS;
                    }

                    var obj = new InstitutionAcctObj()
                    {
                        DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_BANKADDRESS = bankAddress,
                        DEPOSIT_BANKNAME = bankName,
                        DEPOSIT_COUNTRYCODE = cntry_code,

                        // DEPOSIT_COUNTRYCODE = drpcountrycode.SelectedValue,
                        DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                        CARDSCHEME = model.CARDSCHEME,
                        CardSchemDesc = schemedesc,
                        DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                        DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                        USERID = User.Identity.Name,
                        ITBID = model.ITBID,
                        NewRecord = oldRec.NewRecord,
                        Updated = true,
                    };

                    SessionHelper.GetBanks(Session).UpdateItem(obj);

                    var w = GetBanksLines().ToList();
                    var html = PartialView("_ViewAcct", w).RenderToString();
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
            async Task<List<InstitutionAcctObj>> BindAccount(int itbid)
            {
                SessionHelper.GetBanks(Session).Clear();
                var rec = await _repo.GetInstitutionAcctAsync(itbid);
                foreach (var d in rec)
                {
                    SessionHelper.GetBanks(Session).AddItem(d);
                }
                return rec;

            }

            List<InstitutionAcctObj> BindAccountTemp(int inst_itbid, string batchId, string user_id)
            {
                // SessionHelper.GetMccMsc(Session).Clear();
                var rec = _repo.GetInstitutionAcctTemp(inst_itbid, batchId, user_id);

                return rec;

            }

            public ActionResult EditAcct(decimal id, string m)
            {
                try
                {
                    ViewBag.MenuId = m;

                    var lst = GetBanksLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        BindComboMsc();
                        ViewBag.ButtonText = "Update";
                        return PartialView("_AddAcct", rec);
                    }
                }
                catch
                {

                }
                return null;
            }
            public ActionResult DeleteAcct(decimal id, string m)
            {
                try
                {
                    ViewBag.MenuId = m;

                    var lst = GetBanksLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        if (rec.NewRecord)
                        {
                            SessionHelper.GetBanks(Session).RemoveLine(rec.ITBID);

                        }
                        else
                        {
                            SessionHelper.GetBanks(Session).MarkForDelete(rec.ITBID);
                        }
                        var lst2 = GetBanksLines().ToList();
                        var html2 = PartialView("_ViewAcct", lst2).RenderToString();
                        return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {

                }
                var lst3 = GetBanksLines().ToList();
                var html = PartialView("_ViewAcct", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }
            public ActionResult UndoAcct(decimal id, string m)
            {
                try
                {
                    ViewBag.MenuId = m;

                    var lst = GetBanksLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        SessionHelper.GetBanks(Session).UndoDelete(rec.ITBID);

                        var lst2 = GetBanksLines().ToList();
                        var html2 = PartialView("_ViewAcct", lst2).RenderToString();
                        return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {

                }
                var lst3 = GetBanksLines().ToList();
                var html3 = PartialView("_ViewAcct", lst3).RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html3 }, JsonRequestBehavior.AllowGet);
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

                    ViewBag.HeaderTitle = "Authorize Detail for Institution";
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
                        var rec = _repo.GetInstitution((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            BindState(model.INSTITUTION_COUNTRY);
                            BindCity(model.INSTITUTION_COUNTRY, model.INSTITUTION_CITY);
                            ViewBag.InstitutionAcct = BindAccountTemp(model.ITBID, model.BATCHID, model.USERID);
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
                        }
                    }

                    //return Json(new { RespCode = 1, RespMessage = respMsg });
                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Institution Approval", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    TempData["msg"] = respMsg;
                    TempData["status"] = approve;
                    return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    //return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                respMsg = "This request has already been processed by an authorizer.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
            }
            catch (Exception ex)
            {
                respMsg = "Problem processing request. Try again or contact Administrator.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
            }
        }
        private bool CloseMainRecord(int recordId, string bATCHID, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoInstTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoInst.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
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
            var dt = repoInstTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                dt.STATUS = approve;
                var obj = new SM_INSTITUTION()
                {
                    INSTITUTION_ADDRESS = dt.INSTITUTION_ADDRESS,
                    BANK_SECTOR = dt.BANK_SECTOR,
                    CBN_CODE = dt.CBN_CODE,
                    EMAIL = dt.EMAIL,
                    INSTITUTION_CITY = dt.INSTITUTION_CITY,
                    INSTITUTION_COUNTRY = dt.INSTITUTION_COUNTRY,
                    INSTITUTION_NAME = dt.INSTITUTION_NAME,
                    INSTITUTION_SHORTCODE = dt.INSTITUTION_SHORTCODE,
                    INSTITUTION_SLOGAN = dt.INSTITUTION_SLOGAN,
                    INSTITUTION_STATE = dt.INSTITUTION_STATE,
                    INSTITUTION_URL = dt.INSTITUTION_URL,
                    ISSUER_NIBSSACCOUNT = dt.ISSUER_NIBSSACCOUNT,
                    STATUS = active,
                    CREATEDATE = DateTime.Now,
                    USERID = dt.USERID,
                    IS_ACQUIRER = dt.IS_ACQUIRER,
                    IS_BANK = dt.IS_BANK,
                    PTSP = dt.PTSP,
                    PHONENO = dt.PHONENO,

                };

                repoInst.Insert(obj);
                var ac = repoInstAcctTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).ToList();
                if (ac.Count > 0)
                {
                    foreach (var d in ac)
                    {
                        d.STATUS = approve;
                        var obj2 = new SM_INSTITUTIONACCT()
                        {
                            DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                            BATCHID = batchId,
                            DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                            DEPOSIT_BANKADDRESS = d.DEPOSIT_BANKADDESS,
                            DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                            DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                            DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                            INSTITUTION_ITBID = dt.ITBID,
                            CARDSCHEME = d.CARDSCHEME,
                            STATUS = active,
                            CREATEDATE = curDate,
                            USERID = user_id,
                        };
                        obj.SM_INSTITUTIONACCT.Add(obj2);
                    }

                }
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId, string batchId, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoInstTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID == user_id).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoInst.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
                dt.STATUS = approve;

                if (dm != null)
                {
                    dm.BANK_SECTOR = dt.BANK_SECTOR;
                    dm.CBN_CODE = dt.CBN_CODE;
                    dm.EMAIL = dt.EMAIL;
                    dm.INSTITUTION_ADDRESS = dt.INSTITUTION_ADDRESS;
                    dm.LAST_MODIFIED_UID = dt.USERID;
                    dm.INSTITUTION_CITY = dt.INSTITUTION_COUNTRY;
                    dm.INSTITUTION_COUNTRY = dt.INSTITUTION_COUNTRY;
                    dm.INSTITUTION_NAME = dt.INSTITUTION_NAME;
                    dm.ISSUER_NIBSSACCOUNT = dt.ISSUER_NIBSSACCOUNT;
                    dm.INSTITUTION_SHORTCODE = dt.INSTITUTION_SHORTCODE;
                    dm.INSTITUTION_SLOGAN = dt.INSTITUTION_SLOGAN;
                    dm.INSTITUTION_STATE = dt.INSTITUTION_STATE;
                    dm.INSTITUTION_URL = dt.INSTITUTION_URL;
                    dm.IS_ACQUIRER = dt.IS_ACQUIRER;
                    dm.IS_BANK = dt.IS_BANK;
                    dm.PTSP = dt.PTSP;
                    dm.PHONENO = dt.PHONENO;
                    dm.LAST_MODIFIED_DATE = curDate;
                    dm.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    dm.STATUS = active;
                    var ac = repoInstAcctTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID == user_id).ToList();
                    if (ac.Count > 0)
                    {
                        foreach (var t in ac)
                        {
                            t.STATUS = approve;
                            if (t.EVENTTYPE == eventEdit)
                            {
                                var dm2 = repoInstAcct.AllEager(e => e.ITBID == t.RECORDID).FirstOrDefault();
                                if (dm2 != null)
                                {
                                    dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                                    dm2.DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS;
                                    dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                                    dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                                    dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                                    dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                                    //dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                                    dm2.LAST_MODIFIED_UID = user_id;
                                    dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                                    dm2.CARDSCHEME = t.CARDSCHEME;
                                }
                            }
                            else if (t.EVENTTYPE == eventInsert)
                            {
                                var rep = repoInstAcct.AllEager(e => e.DEPOSIT_ACCOUNTNO == t.DEPOSIT_ACCOUNTNO && e.DEPOSIT_BANKCODE == t.DEPOSIT_BANKCODE && e.INSTITUTION_ITBID == t.INSTITUTION_ITBID).FirstOrDefault();
                                if (rep == null)
                                {
                                    var obj2 = new SM_INSTITUTIONACCT()
                                    {
                                        DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO,
                                        DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS,
                                        DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE,
                                        DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME,
                                        DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE,
                                        DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT,
                                        //SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY,
                                        STATUS = active,
                                        CREATEDATE = DateTime.Now,
                                        USERID = user_id,
                                        DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME,
                                        //MERCHANTID = t.MERCHANTID,
                                        BATCHID = t.BATCHID,
                                        INSTITUTION_ITBID = dm.ITBID,
                                        CARDSCHEME = t.CARDSCHEME
                                    };
                                    repoInstAcct.Insert(obj2);
                                }
                                else
                                {
                                    var dm2 = rep; // repoMAcct.AllEager(null,e => e.ITBID == t.RECORDID).FirstOrDefault();
                                    if (dm2 != null)
                                    {
                                        dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                                        dm2.DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS;
                                        dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                                        dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                                        dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                                        dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                                        //dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                                        dm2.LAST_MODIFIED_UID = user_id;
                                        dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                                        // dm2.MERCHANTID = t.MERCHANTID;
                                        dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                                        dm2.CARDSCHEME = t.CARDSCHEME;
                                    };
                                }
                            }
                            else
                            {
                                var ext = repoInstAcct.Find(t.RECORDID);
                                if (ext != null)
                                {
                                    repoInstAcct.Delete(t.RECORDID);
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
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Institution Rejection", Narration, fullName);
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
                //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
        }
        private void RejectBatch(decimal? rECORDID, string bATCHID, string uSERID)
        {
            var recP = repoInstTemp.AllEager(d => d.ITBID == rECORDID && d.USERID == uSERID).FirstOrDefault();
            if (recP != null)
            {
                recP.STATUS = reject;
            }

            var recPP = repoInstAcctTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
            foreach (var d in recPP)
            {
                d.STATUS = reject;
            }

        }

        public IList<InstitutionAcctObj> GetBanksLines()
            {
                //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
                return SessionHelper.GetBanks(Session).Lines;
            }
    }
}