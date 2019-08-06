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
    [Authorize]
    public class PartyController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;

        //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_PARTY> repoParty = null;
        private readonly IRepository<SM_PARTYACCOUNT> repoPartyAcct = null;
        private readonly IRepository<SM_PARTYACCOUNTTEMP> repoPartyAcctTemp = null;
        private readonly IRepository<SM_PARTYTEMP> repoPartyTemp = null;
        //private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
        //private readonly IRepository<SM_CURRENCY> repoCurrency = null;
        //private readonly IRepository<SM_FREQUENCY> repoFreq = null;
        //private readonly IRepository<SM_SECTOR> repoSec = null;
        //private readonly IRepository<SM_CHANNELS> repoChannel = null;

        //private readonly IRepository<SM_PARTYCATEGORY> repoMC = null;

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
        int menuId = 20;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public PartyController()
        {
            uow = new UnitOfWork();
            //repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoParty = new Repository<SM_PARTY>(uow);
            repoPartyAcct = new Repository<SM_PARTYACCOUNT>(uow);
            repoPartyTemp = new Repository<SM_PARTYTEMP>(uow);
            repoPartyAcctTemp = new Repository<SM_PARTYACCOUNTTEMP>(uow);
            //repoFreq = new Repository<SM_FREQUENCY>(uow);
            //repoSec = new Repository<SM_SECTOR>(uow);
            // repoMC = new Repository<SM_PARTYCATEGORY>(uow);
            //repoVal = new Repository<SM_MERCHANTCONFIG>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
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
            try
            {
                // GetMenuId();
                SessionHelper.GetPartyAcct(Session).Clear();
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
        public async Task<ActionResult> PartyList()
        {
            try
            {
                var rec = await _repo.GetPartyAsync(0, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<PartyObj>(), RespCode = 2, RespMessage = ex.Message };
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
        public ActionResult Add(PartyObj model, string m)
        {
            try
            {
                if (model.ITBID == 0)
                {
                    ViewBag.ButtonText = "Save";
                    ViewBag.HeaderTitle = "Add Party";
                }
                else
                {
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Party";
                }

                ViewBag.MenuId = m;
                menuId = SmartUtil.GetMenuId(m);
                string bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID == 0)
                    {
                        var errMsg = "";
                        if (validateForm(model, eventInsert, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            ViewBag.PartyAcct = GetPartyAcctLines();
                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_PARTYTEMP BType = new SM_PARTYTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                PARTY_DESC = model.PARTY_DESC,
                                PARTYTYPE_CODE = model.PARTYTYPE_CODE,
                                PARTY_SHORTNAME = model.PARTY_SHORTNAME,
                                COUNTRY_CODE = model.COUNTRY_CODE,
                                CONTACT_EMAIL = model.CONTACT_EMAIL,
                                CONTACT_NAME = model.CONTACT_NAME,
                                CONTACT_PHONE = model.CONTACT_PHONE,
                                PARTY_CODE = model.PARTYCODEREQUIRED == "Y" ? model.PARTY_CODE : null,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                            };

                            repoPartyTemp.Insert(BType);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                SaveAcctDetailTemp(eventInsert, BType);

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
                                    SessionHelper.GetPartyAcct(Session).Clear();
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Party Record");

                                    //txscope.Complete();
                                    TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", "Party", new { m = m });
                                }
                            }
                        //}
                    }
                    else
                    {
                        var errMsg = "";
                        if (validateForm(model, eventEdit, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            ViewBag.PartyAcct = GetPartyAcctLines();
                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_PARTYTEMP BType = new SM_PARTYTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                PARTY_DESC = model.PARTY_DESC,
                                PARTYTYPE_CODE = model.PARTYTYPE_CODE,
                                PARTY_SHORTNAME = model.PARTY_SHORTNAME,
                                COUNTRY_CODE = model.COUNTRY_CODE,
                                CONTACT_EMAIL = model.CONTACT_EMAIL,
                                CONTACT_NAME = model.CONTACT_NAME,
                                CONTACT_PHONE = model.CONTACT_PHONE,
                                PARTY_CODE = model.PARTYCODEREQUIRED == "Y" ? model.PARTY_CODE : null,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                            };
                            repoPartyTemp.Insert(BType);
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
                                    SessionHelper.GetPartyAcct(Session).Clear();
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Party Record");
                                    //txscope.Complete();
                                    TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", "Party", new { m = m });
                                    //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                                    // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                                }
                            }
                        //}

                    }
                    // If we got this far, something failed, redisplay form
                    //return Json(new { RespCode = 1, RespMessage = errorMsg });
                    BindCombo();
                    ViewBag.PartyAcct = GetPartyAcctLines();
                    ViewBag.Message = errorMsg;
                    return View("Add", model);
                }
            }
            catch (SqlException ex)
            {
                BindCombo();
                ViewBag.PartyAcct = GetPartyAcctLines();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                BindCombo();
                ViewBag.PartyAcct = GetPartyAcctLines();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            BindCombo();
            ViewBag.PartyAcct = GetPartyAcctLines();
            ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            return View("Add", model);
            //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }

        private bool validateForm(PartyObj obj, string eventType, out string errorMsg)
        {
            var sb = new StringBuilder();
            var errCount = 0;
            if (eventType == eventInsert)
            {
                var existCbnCode = repoParty.AllEager(f => f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_CODE != null && f.PARTY_CODE == obj.PARTY_CODE).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"""PARTY CODE"" already exist for another Party");
                    errCount++;
                }
                var existShortName = repoParty.AllEager(f => f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
                if (existShortName > 0)
                {
                    sb.AppendLine(@"""PARTY SHORT NAME"" already exist for another Party");
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
                var existCbnCode = repoParty.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_CODE != null && f.PARTY_CODE == obj.PARTY_CODE).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"""PARTY CODE"" already exist for another Party");
                    errCount++;
                }
                var existShortName = repoParty.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
                if (existShortName > 0)
                {
                    sb.AppendLine(@"""PARTY SHORT NAME"" already exist for another Party");
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


        void SaveAcctDetailTemp(string eventType, SM_PARTYTEMP rec)
        {
            if (eventType == "New")
            {
                var col = GetPartyAcctLines();
                foreach (var d in col)
                {
                    var obj = new SM_PARTYACCOUNTTEMP()
                    {
                        DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                        BATCHID = rec.BATCHID,
                        DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                        DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDESS,
                        DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                        DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                        DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                        PARTY_ITBID = rec.ITBID,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                        AGENT_CODE = d.AGENT_CODE,
                    };
                    repoPartyAcctTemp.Insert(obj);
                }

            }
            else
            {
                var col = GetPartyAcctLines();
                foreach (var d in col)
                {
                    //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                    //{
                    //    continue;
                    //}
                    if (d.NewRecord || d.Updated || d.Deleted)
                    {
                        var obj = new SM_PARTYACCOUNTTEMP()
                        {
                            DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                            BATCHID = rec.BATCHID,
                            DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                            DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDESS,
                            DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                            DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                            DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                            PARTY_ITBID = rec.ITBID,
                            STATUS = open,
                            RECORDID = d.NewRecord ? 0 : d.ITBID,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                            AGENT_CODE = d.AGENT_CODE,
                        };

                        repoPartyAcctTemp.Insert(obj);
                    }
                }
            }

        }
        void BindCombo(string acq_selected = null)
        {
            var ptype = _repo.GetPartyType(0, true, "Active");
            var country = _repo.GetCountry(0, true, "Active");

            ViewBag.PType = new SelectList(ptype, "PARTYTYPE_CODE", "PARTYTYPE_DESC");
            ViewBag.CountryList = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");

            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

        }
        string GetHeaderTitle(int action, string msg)
        {
            switch (action)
            {
                case 1:
                    {
                        return "Add" + msg;

                    }
                case 2:
                    {
                        return "Edit" + msg;

                    }
                case 3:
                    {
                        return "Approval Detail for " + msg;
                    }
                default:
                    {
                        return "";
                    }
            }
        }
        public async Task<ActionResult> Add(int id = 0, string m = null)
        {
            try
            {
                SessionHelper.GetPartyAcct(Session).Clear();
                ViewBag.MenuId = HttpUtility.UrlDecode(m);
                BindCombo();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Party";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add", new PartyObj());

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Party";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetPartyAsync(id, false);
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
                    var recAcct = await BindAccount(model.ITBID);
                    ViewBag.PartyAcct = recAcct;
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

        void BindComboMsc()
        {
            var bankList = _repo.GetInstitution(0, true, "Active").Where(f => f.IS_BANK == "Y").ToList();
            //var country = _repo.GetCountry(0, true, "Active");

            ViewBag.BankList = new SelectList(bankList, "CBN_CODE", "INSTITUTION_NAME");
            //ViewBag.Country = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");

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
                    return PartialView("_AddAcct", new PartyAcctObj());
                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Account";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    //var rec = await _repo.GetPartyAcctAsync(id);
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
        public ActionResult AddAcct(PartyAcctObj model, string m)
        {
            string msg = "";
            var bankName = "";
            var cntry_code = "";
            var bankAddress = "";
            try
            {
                var lst = GetPartyAcctLines().ToList();
                if (model.ITBID == 0)
                {
                    var exist = lst.Exists(r => r.DEPOSIT_BANKCODE == model.DEPOSIT_BANKCODE && r.DEPOSIT_ACCOUNTNO == model.DEPOSIT_ACCOUNTNO);
                    if (exist)
                    {
                        msg = "Account No and Selected Bank Already exist. Duplicate Record is not allowed.";
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

                    var inst = repoInst.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                    if (inst != null)
                    {
                        bankName = inst.INSTITUTION_NAME;
                        cntry_code = inst.INSTITUTION_COUNTRY;
                        bankAddress = inst.INSTITUTION_ADDRESS;
                    }

                    var obj = new PartyAcctObj()
                    {
                        DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_BANKADDESS = bankAddress,
                        DEPOSIT_BANKNAME = bankName,
                        DEPOSIT_COUNTRYCODE = cntry_code,

                        // DEPOSIT_COUNTRYCODE = drpcountrycode.SelectedValue,
                        DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                        // CARDSCHEME = schemeSelected,
                        // CardSchemDesc = schemedesc,
                        DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                        DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                        USERID = User.Identity.Name,
                        ITBID = itbid,
                        NewRecord = true,
                        AGENT_CODE = model.AGENT_CODE,
                    };


                    SessionHelper.GetPartyAcct(Session).AddItem(obj);
                    var w = GetPartyAcctLines().ToList();
                    var html = PartialView("_ViewAcct", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                    var existRec = lst.Where(r => r.DEPOSIT_BANKCODE == model.DEPOSIT_BANKCODE && r.DEPOSIT_ACCOUNTNO == model.DEPOSIT_ACCOUNTNO).FirstOrDefault();
                    if (existRec != null) // not expected to be null
                    {
                        if (oldRec.ITBID != existRec.ITBID)
                        {
                            msg = "Account No and Selected Bank Already exist. Duplicate Record is not allowed.";
                            return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    var inst = repoInst.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                    if (inst != null)
                    {
                        bankName = inst.INSTITUTION_NAME;
                        cntry_code = inst.INSTITUTION_COUNTRY;
                        bankAddress = inst.INSTITUTION_ADDRESS;
                    }

                    var obj = new PartyAcctObj()
                    {
                        DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_BANKADDESS = bankAddress,
                        DEPOSIT_BANKNAME = bankName,
                        DEPOSIT_COUNTRYCODE = cntry_code,

                        // DEPOSIT_COUNTRYCODE = drpcountrycode.SelectedValue,
                        DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                        // CARDSCHEME = schemeSelected,
                        // CardSchemDesc = schemedesc,
                        DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                        DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                        USERID = User.Identity.Name,
                        ITBID = model.ITBID,
                        NewRecord = oldRec.NewRecord,
                        Updated = true,
                        AGENT_CODE = model.AGENT_CODE,
                    };

                    SessionHelper.GetPartyAcct(Session).UpdateItem(obj);

                    var w = GetPartyAcctLines().ToList();
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
        async Task<List<PartyAcctObj>> BindAccount(int itbid)
        {
            SessionHelper.GetPartyAcct(Session).Clear();
            var rec = await _repo.GetPartyAcctAsync(itbid);
            foreach (var d in rec)
            {
                SessionHelper.GetPartyAcct(Session).AddItem(d);
            }
            return rec;

        }

        List<PartyAcctObj> BindAccountTemp(int party_itbid, string batchId, string user_id)
        {
            // SessionHelper.GetMccMsc(Session).Clear();
            var rec = _repo.GetPartyAcctTemp(party_itbid, batchId, user_id);

            return rec;

        }

        public ActionResult EditAcct(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetPartyAcctLines().ToList();

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

                var lst = GetPartyAcctLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    if (rec.NewRecord)
                    {
                        SessionHelper.GetPartyAcct(Session).RemoveLine(rec.ITBID);

                    }
                    else
                    {
                        SessionHelper.GetPartyAcct(Session).MarkForDelete(rec.ITBID);
                    }
                    var lst2 = GetPartyAcctLines().ToList();
                    var html2 = PartialView("_ViewAcct", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetPartyAcctLines().ToList();
            var html = PartialView("_ViewAcct", lst3).RenderToString();
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UndoAcct(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetPartyAcctLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    SessionHelper.GetPartyAcct(Session).UndoDelete(rec.ITBID);

                    var lst2 = GetPartyAcctLines().ToList();
                    var html2 = PartialView("_ViewAcct", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetPartyAcctLines().ToList();
            var html3 = PartialView("_ViewAcct", lst3).RenderToString();
            return Json(new { RespCode = 0, RespMessage = "", data_html = html3 }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> GetIfCodeRequired(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var req = await _repo.GetPartyTypeAsync(0, true, "Active");
                    var rec = req.FirstOrDefault(d => d.PARTYTYPE_CODE == id);
                    if (rec.PARTYCODEREQUIRED == "Y" || rec.PARTYCODEREQUIRED == "y")
                    {
                        return Json(new { RespCode = 0, RespMessage = "", CodeRequired = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { RespCode = 0, RespMessage = "", CodeRequired = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", CodeRequired = false }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { RespCode = 2, RespMessage = "", CodeRequired = false }, JsonRequestBehavior.AllowGet);

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

                    ViewBag.HeaderTitle = "Authorize Detail for Party";
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
                        var rec = _repo.GetParty((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            ViewBag.PartyAcct = BindAccountTemp(model.ITBID, model.BATCHID, model.USERID);
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
                        }
                    }
                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Party Approval", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                    TempData["msg"] = respMsg;
                    TempData["status"] = approve;
                    return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    //return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                respMsg = "This request has already been processed by an authorizer.";
                TempData["msg"] = respMsg;
                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                //return Json(new { RespCode = 1, RespMessage = respMsg });

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

        private bool CloseMainRecord(int recordId, string bATCHID, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoPartyTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoParty.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
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
            var dt = repoPartyTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                dt.STATUS = approve;
                var obj = new SM_PARTY()
                {
                    PARTYTYPE_CODE = dt.PARTYTYPE_CODE,
                    CONTACT_EMAIL = dt.CONTACT_EMAIL,
                    CONTACT_NAME = dt.CONTACT_NAME,
                    CONTACT_PHONE = dt.CONTACT_PHONE,
                    COUNTRY_CODE = dt.COUNTRY_CODE,
                    PARTY_CODE = dt.PARTY_CODE,
                    PARTY_DESC = dt.PARTY_DESC,
                    PARTY_SHORTNAME = dt.PARTY_SHORTNAME,
                    STATUS = active,
                    CREATEDATE = DateTime.Now,
                    USERID = dt.USERID,
                };

                repoParty.Insert(obj);
                var ac = repoPartyAcctTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper() && e.PARTY_ITBID == dt.ITBID).ToList();
                if (ac.Count > 0)
                {
                    foreach (var d in ac)
                    {
                        d.STATUS = approve;
                        var obj2 = new SM_PARTYACCOUNT()
                        {
                            DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                            BATCHID = batchId,
                            DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                            DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDESS,
                            DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                            DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                            DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                            PARTY_ITBID = dt.ITBID,
                            STATUS = active,
                            CREATEDATE = curDate,
                            USERID = user_id,
                            AGENT_CODE = d.AGENT_CODE
                        };
                        obj.SM_PARTYACCOUNT.Add(obj2);
                    }
                }
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId, string batchId, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoPartyTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoParty.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
                dt.STATUS = approve;

                if (dm != null)
                {
                    dm.PARTYTYPE_CODE = dt.PARTYTYPE_CODE;
                    dm.PARTY_CODE = dt.PARTY_CODE;
                    dm.PARTY_DESC = dt.PARTY_DESC;
                    dm.PARTY_SHORTNAME = dt.PARTY_SHORTNAME;
                    dm.LAST_MODIFIED_UID = dt.USERID;
                    dm.CONTACT_NAME = dt.CONTACT_NAME;
                    dm.CONTACT_EMAIL = dt.CONTACT_EMAIL;
                    dm.CONTACT_PHONE = dt.CONTACT_PHONE;
                    dm.COUNTRY_CODE = dt.COUNTRY_CODE;
                    dm.LAST_MODIFIED_DATE = curDate;
                    dm.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    dm.STATUS = active;

                    var ac = repoPartyAcctTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID == user_id).ToList();
                    if (ac.Count > 0)
                    {
                        foreach (var t in ac)
                        {
                            t.STATUS = approve;
                            if (t.EVENTTYPE == eventEdit)
                            {
                                var dm2 = repoPartyAcct.AllEager(e => e.ITBID == t.RECORDID).FirstOrDefault();
                                if (dm2 != null)
                                {
                                    dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                                    //dm2.DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS;
                                    dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                                    dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                                    dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                                    dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                                    //dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                                    dm2.LAST_MODIFIED_UID = user_id;
                                    dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                                    //dm2.MERCHANTID = t.MERCHANTID;
                                    //dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                                    dm2.AGENT_CODE = t.AGENT_CODE;
                                }
                            }
                            else if (t.EVENTTYPE == eventInsert)
                            {
                                var rep = repoPartyAcct.AllEager(e => e.DEPOSIT_ACCOUNTNO == t.DEPOSIT_ACCOUNTNO && e.DEPOSIT_BANKCODE == t.DEPOSIT_BANKCODE && e.PARTY_ITBID == t.PARTY_ITBID).FirstOrDefault();
                                if (rep == null)
                                {
                                    var obj2 = new SM_PARTYACCOUNT()
                                    {
                                        DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO,
                                        //DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS,
                                        DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE,
                                        DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME,
                                        DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE,
                                        DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT,
                                        //SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY,
                                        STATUS = active,
                                        CREATEDATE = DateTime.Now,
                                        USERID = user_id,
                                        DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME,
                                        PARTY_ITBID = dm.ITBID,
                                        BATCHID = t.BATCHID,
                                        DEPOSIT_BANKADDESS = t.DEPOSIT_BANKADDESS,
                                        AGENT_CODE = t.AGENT_CODE,
                                    };
                                    repoPartyAcct.Insert(obj2);
                                }
                                else
                                {
                                    var dm2 = rep; // repoMAcct.AllEager(null,e => e.ITBID == t.RECORDID).FirstOrDefault();
                                    if (dm2 != null)
                                    {
                                        dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                                        dm2.DEPOSIT_BANKADDESS = t.DEPOSIT_BANKADDESS;
                                        dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                                        dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                                        dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                                        dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                                        //dm2.cu = t.SETTLEMENTCURRENCY;
                                        dm2.LAST_MODIFIED_UID = user_id;
                                        dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                                        //dm2.MERCHANTID = t.MERCHANTID;
                                        dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                                        dm2.AGENT_CODE = t.AGENT_CODE;
                                    };
                                }
                            }
                            else
                            {
                                var ext = repoPartyAcct.Find(t.RECORDID);
                                if (ext != null)
                                {
                                    repoPartyAcct.Delete(t.RECORDID);
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

                                RejectBatch(rec2.RECORDID, rec2.BATCHID, rec2.USERID);

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
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Party Rejection", Narration, fullName);
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

        private void RejectBatch(decimal? rECORDID, string bATCHID, string uSERID)
        {
            var recP = repoPartyTemp.AllEager(d => d.ITBID == rECORDID && d.USERID == uSERID).FirstOrDefault();
            if (recP != null)
            {
                recP.STATUS = reject;
            }

            var recPP = repoPartyAcctTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
            foreach (var d in recPP)
            {
                d.STATUS = reject;
            }

        }

        public IList<PartyAcctObj> GetPartyAcctLines()
        {
            //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
            return SessionHelper.GetPartyAcct(Session).Lines;
        }


    }
}