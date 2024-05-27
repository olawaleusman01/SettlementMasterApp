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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class RevenueHeadController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;

        //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
        private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
        private readonly IRepository<SM_REVENUEGROUP> repoRvGroup = null;
        private readonly IRepository<SM_REVENUEGROUPTEMP> repoRvGroupTemp = null;
        private readonly IRepository<SM_REVENUEHEAD> repoRvHead = null;
        private readonly IRepository<SM_REVENUEHEADTEMP> repoRvHeadTemp = null;
        private readonly IRepository<SM_MERCHANTDETAIL> repoM = null;
        private readonly IRepository<SM_REVENUEBANKACCT> repoRvAcct = null;
        private readonly IRepository<SM_MERCHANTACCT> repoMAcct = null;
        private readonly IRepository<SM_REVENUEBANKACCTTEMP> repoRvAcctTemp = null;

        private readonly IRepository<SM_RevenuHeadParty> repoRvHeadParty = null;
        private readonly IRepository<SM_RevenuHeadPartyTemp> repoRvHeadPartyTemp = null;
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
        int menuId = 22;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public RevenueHeadController()
        {
            uow = new UnitOfWork();
            //repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoRvGroup = new Repository<SM_REVENUEGROUP>(uow);
            repoRvGroupTemp = new Repository<SM_REVENUEGROUPTEMP>(uow);
            repoRvHead = new Repository<SM_REVENUEHEAD>(uow);
            repoRvHeadTemp = new Repository<SM_REVENUEHEADTEMP>(uow);
            repoM = new Repository<SM_MERCHANTDETAIL>(uow);
            repoRvAcct = new Repository<SM_REVENUEBANKACCT>(uow);
            repoRvAcctTemp = new Repository<SM_REVENUEBANKACCTTEMP>(uow);
            repoMAcct = new Repository<SM_MERCHANTACCT>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoRvHeadParty = new Repository<SM_RevenuHeadParty>(uow);
            repoRvHeadPartyTemp = new Repository<SM_RevenuHeadPartyTemp>(uow);
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
                //SessionHelper.GetRvHead(Session).Clear();
                //menuId = SmartUtil.GetMenuId(m);
                //if (menuId == 0)
                //{
                //    return RedirectToAction("Index", "Home");
                //}

                //ViewBag.MenuId = HttpUtility.UrlEncode(m);
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
        public async Task<ActionResult> RvGroupList(string id)
        {
            try
            {
                var rec = await _repo.GetRvGroupAsync(0, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<RvGroupObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> RvHeadList(string id)
        {
            try
            {
                var rec = await _repo.GetRvHead_By_GroupCodeAsync(id);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<RvGroupObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> RvDrAcctList(string id)
        {
            try
            {
                var rec = await _repo.GetRvDrAcct_By_GroupCodeAsync(id);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<RvDrAcctObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> GetFrequency(int id)
        {
            try
            {
                var rec = await _repo.GetFrequencyAsync(id, false);  //repoSession.FindAsync(id);              
                return Json(new { data = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<RvGroupObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> GetMerchantAcct(string id)
        {
            try
            {
                var rec = await _repo.GetMerchantAcctAsync(id);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<MerchantAcctObj>(), RespCode = 2, RespMessage = ex.Message };
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
        public ActionResult Add(RvGroupObj model, string m)
        {
            try
            {
                if (model.ITBID == 0)
                {
                    ViewBag.ButtonText = "Save";
                    ViewBag.HeaderTitle = "Add Revenue Group";
                }
                else
                {
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Revenue Group";
                }

                //ViewBag.MenuId = m;
                //menuId = SmartUtil.GetMenuId(m);
                string bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID == 0)
                    {
                        //var errMsg = "";
                        //if (validateForm(model, eventInsert, out errMsg))
                        //{
                        //    ViewBag.Message = errMsg; // "Carscheme already Exist.";
                        //    BindCombo();
                        //    ViewBag.PartyAcct = GetRvHeadLines();
                        //    return View("Add", model);
                        //};
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_REVENUEGROUPTEMP BType = new SM_REVENUEGROUPTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                GROUPCODE = model.GROUPCODE,
                                GROUPNAME = model.GROUPNAME,
                                ACCOUNT_ID = model.ACCOUNT_ID,
                                MERCHANTID = model.MERCHANTID,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                                GLOBALACCOUNTFLAG = model.GLOBALACCOUNTFLAG,
                                SETTLEMENT_FREQUENCY = model.SETTLEMENT_FREQUENCY,
                                SET_DAYS = model.SET_DAYS
                            };

                            repoRvGroupTemp.Insert(BType);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                SaveRvHeadTemp(eventInsert, BType);

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
                                    SessionHelper.GetRvHead(Session).Clear();
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Revenue Head Record");

                                   // txscope.Complete();
                                    TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", new { m = m });
                                }
                            }
                        //}
                    }
                    else
                    {
                        //var errMsg = "";
                        //if (validateForm(model, eventEdit, out errMsg))
                        //{
                        //    ViewBag.Message = errMsg; // "Carscheme already Exist.";
                        //    BindCombo();
                        //    ViewBag.PartyAcct = GetRvHeadLines();
                        //    return View("Add", model);
                        //};
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            var BType = new SM_REVENUEGROUPTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                GROUPCODE = model.GROUPCODE,
                                GROUPNAME = model.GROUPNAME,
                                ACCOUNT_ID = model.ACCOUNT_ID,
                                MERCHANTID = model.MERCHANTID,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                                GLOBALACCOUNTFLAG = model.GLOBALACCOUNTFLAG,
                                SETTLEMENT_FREQUENCY = model.SETTLEMENT_FREQUENCY,
                                SET_DAYS = model.SET_DAYS,
                            };
                            repoRvGroupTemp.Insert(BType);
                            //  var rst1 = new AuthListUtil().SaveLog(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                            if (rst)
                            {
                                SaveRvHeadTemp(eventEdit, BType);
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
                                    SessionHelper.GetRvHead(Session).Clear();
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Revenue Head Record");

                                    //txscope.Complete();
                                    TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", new { m = m });
                                    //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                                    // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                                }
                            }
                        //}

                    }
                    // If we got this far, something failed, redisplay form
                    //return Json(new { RespCode = 1, RespMessage = errorMsg });
                    BindCombo();
                    ViewBag.PartyAcct = GetRvHeadLines();
                    ViewBag.Message = errorMsg;
                    return View("Add", model);
                }
            }
            catch (SqlException ex)
            {
                BindCombo();
                ViewBag.PartyAcct = GetRvHeadLines();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                BindCombo();
                ViewBag.PartyAcct = GetRvHeadLines();
                ViewBag.Message = ex.Message;
                return View("Add", model);
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            BindCombo();
            ViewBag.PartyAcct = GetRvHeadLines();
            ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            return View("Add", model);
            //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }

        //private bool validateForm(RvGroupObj obj, string eventType, out string errorMsg)
        //{
        //    var sb = new StringBuilder();
        //    var errCount = 0;
        //    if (eventType == eventInsert)
        //    {
        //        var existCbnCode = repoRvGroup.AllEager(f => f.GROUPCODE != null && f.GROUPCODE == obj.GROUPCODE && f.MERCHANTID != null && f.MERCHANTID == obj.MERCHANTID).Count();
        //        if (existCbnCode > 0)
        //        {
        //            sb.AppendLine(@"""GR CODE"" already exist for another Party");
        //            errCount++;
        //        }
        //        var existShortName = repoParty.AllEager(f => f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
        //        if (existShortName > 0)
        //        {
        //            sb.AppendLine(@"""PARTY SHORT NAME"" already exist for another Party");
        //            errCount++;
        //        }

        //        if (errCount > 0)
        //        {
        //            errorMsg = sb.ToString();
        //            return true;
        //        }
        //        errorMsg = sb.ToString();
        //        return false;
        //    }
        //    else
        //    {
        //        var existCbnCode = repoParty.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_CODE != null && f.PARTY_CODE == obj.PARTY_CODE).Count();
        //        if (existCbnCode > 0)
        //        {
        //            sb.AppendLine(@"""PARTY CODE"" already exist for another Party");
        //            errCount++;
        //        }
        //        var existShortName = repoParty.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
        //        if (existShortName > 0)
        //        {
        //            sb.AppendLine(@"""PARTY SHORT NAME"" already exist for another Party");
        //            errCount++;
        //        }

        //        if (errCount > 0)
        //        {
        //            errorMsg = sb.ToString();
        //            return true;
        //        }
        //        errorMsg = sb.ToString();
        //        return false;
        //    }
        //}


        void SaveRvHeadTemp(string eventType, SM_REVENUEGROUPTEMP rec)
        {
            var rv = new RvHeadSession();
            if (eventType == "New")
            {
                var col = rv.GetRevenueHead(User.Identity.Name); // GetRvHeadLines();
                foreach (var d in col)
                {
                    var obj = new SM_REVENUEHEADTEMP()
                    {
                        CODE = d.CODE, //drpCalcBasis.SelectedValue,
                        BATCHID = rec.BATCHID,
                        DESCRIPTION = d.DESCRIPTION,
                        PAYMENTITEMID = d.PAYMENTITEMID,
                        ACCOUNT_ID = d.ACCOUNT_ID,
                        RVGROUPCODE = rec.GROUPCODE,
                        MERCHANTID = rec.MERCHANTID,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                        SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY,
                    };
                    repoRvHeadTemp.Insert(obj);

                    var recP = rv.GetRevenueHeadParty(d.CODE, User.Identity.Name);
                    foreach(var pty in recP)
                    {
                        SM_RevenuHeadPartyTemp objParty = new SM_RevenuHeadPartyTemp()
                        {
                            PartyAccountId = pty.PartyAccountId,
                            PartyId = pty.PartyId,
                            PartyValue = pty.PartyValue,
                            BatchId = rec.BATCHID,
                            CreateDate = rec.CREATEDATE,
                            UserId = User.Identity.Name,
                            
                        };

                        repoRvHeadPartyTemp.Insert(objParty);
                    }
                }

            }
            else
            {
                var col = rv.GetRevenueHead(User.Identity.Name); // GetRvHeadLines();
                foreach (var d in col)
                {

                    var obj = new SM_REVENUEHEADTEMP()
                    {
                        CODE = d.CODE, //drpCalcBasis.SelectedValue,
                        RVGROUPCODE = rec.GROUPCODE,
                        DESCRIPTION = d.DESCRIPTION,
                        PAYMENTITEMID = d.PAYMENTITEMID,
                        ACCOUNT_ID = d.ACCOUNT_ID,
                        MERCHANTID = rec.MERCHANTID,
                        STATUS = open,
                        RECORDID = d.NewRecord ? 0 : d.ITBID,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = d.EVENTTYPE, // ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                        BATCHID = rec.BATCHID,
                        SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY,
                    };
                    repoRvHeadTemp.Insert(obj);

                    var recP = rv.GetRevenueHeadParty(d.CODE, User.Identity.Name);
                    foreach (var pty in recP)
                    {
                        SM_RevenuHeadPartyTemp objParty = new SM_RevenuHeadPartyTemp()
                        {
                            PartyAccountId = pty.PartyAccountId,
                            PartyId = pty.PartyId,
                            PartyValue = pty.PartyValue,
                            BatchId = rec.BATCHID,
                            CreateDate = rec.CREATEDATE,
                            UserId = User.Identity.Name,

                        };

                        repoRvHeadPartyTemp.Insert(objParty);
                    }
                }
            }
        }
        void BindCombo(string mid = null)
        {
            var freq = _repo.GetFrequency(0, true, "Active");

            var mList = _repo.GetMerchantSearchDropDown("XP");
            var bankList = _repo.GetMerchantAcct(mid);
            ViewBag.MerchantList = new SelectList(mList, "MERCHANTID", "DESCRIPTION");
            ViewBag.MAcctList = new SelectList(bankList, "ITBID", "DESCRIPTION");
            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");
            ViewBag.FrequencyList = new SelectList(freq, "ITBID", "FREQUENCY_DESC");

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
        public async Task<ActionResult> Add(int id = 0, string m = null,string mid = null)
        {
            try
            {
                var rv = new RvHeadSession();
                rv.PurgeRevenueHead(User.Identity.Name);
                //SessionHelper.GetRvHead(Session).Clear();
                ViewBag.MenuId = HttpUtility.UrlDecode(m);

                if (id == 0)
                {
                    BindCombo(mid);
                    ViewBag.HeaderTitle = "Add Revenue Group";
                    ViewBag.StatusVisible = false;
                    ViewBag.ButtonText = "Save";
                    GetPriv();
                    return View("Add", new RvGroupObj() { MERCHANTID = mid });

                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Revenue Group";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetRvGroupAsync(id, false);
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
                    BindCombo(model.MERCHANTID);
                    //var recAcct = await BindAccount(model.ITBID);
                    //ViewBag.PartyAcct = recAcct;
                    GetPriv();
                    return View("Add", model);

                }
            }
            catch (Exception ex)
            {
                BindCombo();
                ViewBag.Message = ex.Message;
                return View("Index");
            }
        }

        void BindComboMsc(string mid)
        {
            var bankList = _repo.GetMerchantAcct(mid);
            var freq = _repo.GetFrequency(0, true, "Active");
            //var country = _repo.GetCountry(0, true, "Active");

            ViewBag.MAcctList = new SelectList(bankList, "ITBID", "DESCRIPTION");
            ViewBag.FrequencyList = new SelectList(freq, "ITBID", "FREQUENCY_DESC");

        }
        public PartialViewResult AddRvHead(string id)
        {
            try
            {
                // ViewBag.MenuId = m;

                BindComboMsc(id);

                ViewBag.HeaderTitle = "Add Revenue Head";
                ViewBag.ButtonText = "Add";
                var obj = new RvHeadObj() { MID = id };
                id = null;
                return PartialView("_AddRevenue", obj);
                // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);

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
        //        htmlString = PartialView("_ViewRvHead", rec).RenderToString();
        //        return Json(new { data_msc = htmlString, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        htmlString = PartialView("_ViewRvHead").RenderToString();
        //        return Json(new { data_msc = htmlString, RespCode = 1, RespMessage = "Problem Processing Request" }, JsonRequestBehavior.AllowGet);

        //    }
        //}

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddRvHead(RvHeadObj model, string m)
        {
            var rv = new RvHeadSession();
            string msg = "";
            var bankName = "";
            var bankCode = "";
            var acctNo = "";
            var acctName = "";
            try
            {
                //var lst = GetRvHeadLines().ToList();
                if (string.IsNullOrEmpty(model.PID) && model.DB_ITBID.GetValueOrDefault() <= 0)
                {
                    //var exist = lst.Exists(r => r.CODE == model.CODE);
                    //var existDb = repoRvHead.AllEager(d => d.CODE == model.CODE && d.MERCHANTID == model.MERCHANTID).Count();
                    //if (exist || existDb > 0)
                    //{
                    //    msg = "Revenue Code Already exist. Duplicate Record is not allowed.";
                    //    return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    //}
                    //int itbid = 0;
                    //if (lst.Count == 0)
                    //{

                    //    itbid = 1;
                    //}
                    //else
                    //{
                    //    var itb = lst.Max(f => f.ITBID);
                    //    itbid = itb + 1;
                    //}

                    var inst = _repo.GetMerchantAcctByItbId(model.ACCOUNT_ID.GetValueOrDefault());
                    if (inst != null)
                    {
                        bankName = inst.DEPOSIT_BANKNAME;
                        bankCode = inst.DEPOSIT_BANKCODE;
                        acctName = inst.DEPOSIT_ACCTNAME;
                        acctNo = inst.DEPOSIT_ACCOUNTNO;
                    }
                    var freq = _repo.GetFrequency(model.SETTLEMENT_FREQUENCY.GetValueOrDefault(), false).FirstOrDefault();
                    var freqName = "";
                    if (freq != null)
                    {
                        freqName = freq.FREQUENCY_DESC;
                    }
                    var obj = new RvHeadObj()
                    {
                        ACCOUNT_ID = model.ACCOUNT_ID,
                        BANKCODE = bankCode,
                        BANK_NAME = bankName,
                        BANKACCOUNT = acctNo,
                        CODE = model.CODE,
                        ACCT_NAME = acctName,
                        DESCRIPTION = model.DESCRIPTION,
                        PAYMENTITEMID = model.PAYMENTITEMID,
                        USERID = User.Identity.Name,
                        MID = model.MID,
                        EVENTTYPE = eventInsert,
                        RVGROUPCODE = model.RVGROUPCODE,
                        //PID = model.PID,
                        SETTLEMENT_FREQUENCY = model.SETTLEMENT_FREQUENCY,
                        FREQUENCY_DESC = freqName,
                        RevenueSharingPartys = model.RevenueSharingPartys
                    };
                    //SessionHelper.GetRvHead(Session).AddItem(obj);
                    rv.PurgeRevenueHeadParty(User.Identity.Name, model.CODE);
                    var rst = rv.PostRevenueHead(obj, 1);
                   
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    var w = rv.GetRevenueHead(User.Identity.Name); // GetRvHeadLines().To===();
                    var html = PartialView("_ViewRvHead", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    //var existDb = repoRvHead.AllEager(d => d.CODE == model.CODE && d.MERCHANTID == model.MERCHANTID).FirstOrDefault();

                    //var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                    //var existRec = lst.Where(r => r.CODE == model.CODE).FirstOrDefault();
                    //if (existRec != null) // not expected to be null
                    //{
                    //    if (oldRec.ITBID != existRec.ITBID)
                    //    {
                    //        msg = "Revenue Code Already exist. Duplicate Record is not allowed.";
                    //        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //var insertRecord = false;
                    //if (oldRec == null)
                    //{
                    //    insertRecord = true;
                    //    var extDb = repoRvHead.AllEager(d => d.CODE == model.CODE && d.RVGROUPCODE == model.RVGROUPCODE && d.ITBID != model.ITBID).Count();
                    //    if (extDb > 0)
                    //    {
                    //        msg = "Revenue Code Already exist. Duplicate Record is not allowed.";
                    //        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}

                    var inst = _repo.GetMerchantAcctByItbId(model.ACCOUNT_ID.GetValueOrDefault());
                    if (inst != null)
                    {
                        bankName = inst.DEPOSIT_BANKNAME;
                        bankCode = inst.DEPOSIT_BANKCODE;
                        acctName = inst.DEPOSIT_ACCTNAME;
                        acctNo = inst.DEPOSIT_ACCOUNTNO;
                    }
                    var freq = _repo.GetFrequency(model.SETTLEMENT_FREQUENCY.GetValueOrDefault(), false).FirstOrDefault();
                    var freqName = "";
                    if (freq != null)
                    {
                        freqName = freq.FREQUENCY_DESC;
                    }
                    var obj = new RvHeadObj()
                    {
                        ACCOUNT_ID = model.ACCOUNT_ID,
                        CODE = model.CODE,
                        DESCRIPTION = model.DESCRIPTION,
                        PAYMENTITEMID = model.PAYMENTITEMID,
                        BANKCODE = bankCode,
                        BANK_NAME = bankName,
                        BANKACCOUNT = acctNo,
                        ACCT_NAME = acctName,
                        USERID = User.Identity.Name,
                        MID = model.MID,
                        DB_ITBID = model.DB_ITBID,
                        RVGROUPCODE = model.RVGROUPCODE,
                        PID = model.PID,
                        EVENTTYPE = model.DB_ITBID > 0 ? eventEdit : eventInsert,
                        SETTLEMENT_FREQUENCY = model.SETTLEMENT_FREQUENCY,
                        FREQUENCY_DESC = freqName,
                        RevenueSharingPartys = model.RevenueSharingPartys
                    };
                    OutPutObj rst;
                    //if (string.IsNullOrEmpty(model.PID))
                    //{
                    //     rst = rv.PostRevenueHead(obj, 1);
                    //}
                    //else
                    //{
                    rv.PurgeRevenueHeadParty(User.Identity.Name, model.CODE);
                    rst = rv.PostRevenueHead(obj, 2);
                    //}
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    //SessionHelper.GetRvHead(Session).UpdateItem(obj);
                    // rv.PostRevenueHead(obj, 2);
                    var w = rv.GetRevenueHead(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewRvHead", w).RenderToString();
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
        //[HttpPost]
        //// [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddRvHead(RvHeadObj model, string m)
        //{
        //    string msg = "";
        //    var bankName = "";
        //    var bankCode = "";
        //    var acctNo = "";
        //    var acctName = "";
        //try
        //    {
        //        var lst = GetRvHeadLines().ToList();
        //        if (model.ITBID == 0)
        //        {
        //            var exist = lst.Exists(r => r.CODE == model.CODE);
        //            var existDb = repoRvHead.AllEager(d => d.CODE == model.CODE && d.MERCHANTID == model.MERCHANTID).Count();
        //            if (exist || existDb > 0)
        //            {
        //                msg = "Revenue Code Already exist. Duplicate Record is not allowed.";
        //                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

        //            }
        //            int itbid = 0;
        //            if (lst.Count == 0)
        //            {

        //                itbid = 1;
        //            }
        //            else
        //            {
        //                var itb = lst.Max(f => f.ITBID);
        //                itbid = itb + 1;
        //            }

        //        var inst = _repo.GetMerchantAcctByItbId(model.ACCOUNT_ID.GetValueOrDefault());
        //        if (inst != null)
        //        {
        //            bankName = inst.DEPOSIT_BANKNAME;
        //            bankCode = inst.DEPOSIT_BANKCODE;
        //            acctName = inst.DEPOSIT_ACCTNAME;
        //            acctNo = inst.DEPOSIT_ACCOUNTNO;
        //        }

        //        var obj = new RvHeadObj()
        //        {
        //            ACCOUNT_ID = model.ACCOUNT_ID,
        //            BANKCODE = bankCode,
        //            BANK_NAME = bankName,
        //            BANKACCOUNT = acctNo,
        //            CODE = model.CODE,
        //            ACCT_NAME = acctName,
        //            DESCRIPTION = model.DESCRIPTION,
        //            USERID = User.Identity.Name,
        //            MID = model.MID,
        //            ITBID = itbid,
        //            NewRecord = true,
        //        };
        //            SessionHelper.GetRvHead(Session).AddItem(obj);
        //            var w = GetRvHeadLines().ToList();
        //            var html = PartialView("_ViewRvHead", w).RenderToString();
        //            msg = "Record Added to List";
        //            return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

        //        }
        //        else
        //        {
        //        //var existDb = repoRvHead.AllEager(d => d.CODE == model.CODE && d.MERCHANTID == model.MERCHANTID).FirstOrDefault();

        //            var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
        //            var existRec = lst.Where(r => r.CODE == model.CODE).FirstOrDefault();
        //            if (existRec != null) // not expected to be null
        //            {
        //                if (oldRec.ITBID != existRec.ITBID)
        //                {
        //                    msg = "Revenue Code Already exist. Duplicate Record is not allowed.";
        //                    return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        var insertRecord = false;
        //        if (oldRec == null)
        //        {
        //            insertRecord = true;
        //            var extDb = repoRvHead.AllEager(d => d.CODE == model.CODE && d.RVGROUPCODE == model.RVGROUPCODE && d.ITBID != model.ITBID).Count();
        //            if (extDb > 0)
        //            {
        //                msg = "Revenue Code Already exist. Duplicate Record is not allowed.";
        //                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
        //            }
        //        }

        //        var inst = _repo.GetMerchantAcctByItbId(model.ACCOUNT_ID.GetValueOrDefault());
        //        if (inst != null)
        //        {
        //            bankName = inst.DEPOSIT_BANKNAME;
        //            bankCode = inst.DEPOSIT_BANKCODE;
        //            acctName = inst.DEPOSIT_ACCTNAME;
        //            acctNo = inst.DEPOSIT_ACCOUNTNO;
        //        }
        //        var obj = new RvHeadObj()
        //        {
        //            ACCOUNT_ID = model.ACCOUNT_ID,
        //            CODE = model.CODE,
        //            DESCRIPTION = model.DESCRIPTION,
        //            BANKCODE = bankCode,
        //            BANK_NAME = bankName,
        //            BANKACCOUNT = acctNo,
        //            ACCT_NAME = acctName,
        //            USERID = User.Identity.Name,
        //            MID = model.MID,
        //            ITBID = model.ITBID,
        //            NewRecord = oldRec!= null ? oldRec.NewRecord : false,
        //            Updated = true,
        //        };
        //        if(insertRecord)
        //        {
        //            SessionHelper.GetRvHead(Session).AddItem(obj);

        //        }
        //        else
        //        {
        //            SessionHelper.GetRvHead(Session).UpdateItem(obj);

        //        }

        //        var w = GetRvHeadLines().ToList();
        //            var html = PartialView("_ViewRvHead", w).RenderToString();
        //            msg = "Record Updated to List";
        //            return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = ex.Message;
        //        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

        //    }
        //}
        //async Task<List<PartyAcctObj>> BindAccount(int itbid)
        //{
        //    SessionHelper.GetPartyAcct(Session).Clear();
        //    var rec = await _repo.GetPartyAcctAsync(itbid);
        //    foreach (var d in rec)
        //    {
        //        SessionHelper.GetPartyAcct(Session).AddItem(d);
        //    }
        //    return rec;

        //}

        List<RvHeadObj> BindRvHeadTemp(string batchId, string user_id)
        {
            // SessionHelper.GetMccMsc(Session).Clear();
            var rec = _repo.GetRvHeadTemp(batchId, user_id);

            return rec;

        } 

        public ActionResult EditRvHead(string id)
        {
            try
            {

                var rv = new RvHeadSession();
                var rec = rv.FindRevenueHead(id, User.Identity.Name);
                if (rec != null)
                {
                    rec.RevenueSharingPartys = rv.GetRevenueHeadParty(rec.CODE, User.Identity.Name);
                    BindComboMsc(rec.MID);
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Revenue Code";
                    return PartialView("_AddRevenue", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ActionResult EditRvHeadLocal(int id)
        {
            try
            {
                var rv = new RvHeadSession();
                var rec = _repo.GetRvHead(id);
                if (rec != null)
                {
                    rec.RevenueSharingPartys = _repo.GetRvHeadParty(rec.ITBID);
                    BindComboMsc(rec.MID);
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Revenue Code";
                    return PartialView("_AddRevenue", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ActionResult DeleteAcct(string id, string m)
        {
            var rv = new RvHeadSession();
            try
            {
                rv.DeleteRevenueHead(id, User.Identity.Name);

                var lst2 = rv.GetRevenueHead(User.Identity.Name); // GetRvHeadLines().ToList();
                var html2 = PartialView("_ViewRvHead", lst2).RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var lst3 = rv.GetRevenueHead(User.Identity.Name); //  GetRvHeadLines().ToList();
                var html = PartialView("_ViewRvHead", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }
           
        }
            public ActionResult UndoAcct(decimal id, string m)
            {
                try
                {
                    ViewBag.MenuId = m;

                    var lst = GetRvHeadLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        SessionHelper.GetRvHead(Session).UndoDelete(rec.ITBID);

                        var lst2 = GetRvHeadLines().ToList();
                        var html2 = PartialView("_ViewRvHead", lst2).RenderToString();
                        return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {

                }
                var lst3 = GetRvHeadLines().ToList();
                var html3 = PartialView("_ViewRvHead", lst3).RenderToString();
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

                        ViewBag.HeaderTitle = "Authorize Detail for Revenue Code";
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
                        //var rec = _repo.GetRvGroup((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        //if (rec != null && rec.Count > 0)
                        //{
                        //    var model = rec.FirstOrDefault();
                        //    ViewBag.RvHeadList = BindRvHeadTemp(det.BATCHID, model.USERID);
                        //    obj.Status = det.STATUS;
                        //    obj.EventType = det.EVENTTYPE;
                        //    obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                        //    obj.User = model.CREATED_BY;
                        //    ViewBag.Auth = obj;
                        //    ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);

                        //    BindCombo(model.MERCHANTID);
                        //    //ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
                        //    // return null;

                        //    return View("DetailAuth", model);
                        //}

                        var viewtoDisplay = "";
                        if (det.POSTTYPE == Single)
                        {


                            var rec = _repo.GetRvGroup((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                            if (rec != null && rec.Count > 0)
                            {
                                var model = rec.FirstOrDefault();
                                ViewBag.RvHeadList = BindRvHeadTemp(det.BATCHID, model.USERID);
                                obj.Status = det.STATUS;
                                obj.EventType = det.EVENTTYPE;
                                obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                obj.User = model.CREATED_BY;
                                ViewBag.Auth = obj;
                                ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);

                                BindCombo(model.MERCHANTID);
                                //ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
                                // return null;

                                return View("DetailAuth", model);
                                //break;
                            }
                        }
                        else
                        {
                            ViewBag.HeaderTitle = "Authorize Detail for Revenue Upload";
                            var splt = det.BATCHID.Split('_');
                            if (splt[0] == "RVH")
                            {
                                viewtoDisplay = "DetailAuthBulk";
                                var recg = _repo.GetRvHeadUploadTemp(det.USERID, det.BATCHID);
                                if (recg != null && recg.Count > 0)
                                {
                                    var model = recg.FirstOrDefault();
                                    obj.Status = det.STATUS;
                                    obj.EventType = det.EVENTTYPE;
                                    obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                    obj.User = det.CREATEDATE.GetValueOrDefault().ToString("dd-MM-yyyy");
                                    ViewBag.Auth = obj;
                                    ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                    return View(viewtoDisplay, recg);
                                }
                            }
                            else
                            {
                                viewtoDisplay = "DetailAuthBulkDebitAcct";
                                var recg = _repo.GetRvDrAcctTemp(det.USERID, det.BATCHID);
                                if (recg != null && recg.Count > 0)
                                {
                                    var model = recg.FirstOrDefault();
                                    obj.Status = det.STATUS;
                                    obj.EventType = det.EVENTTYPE;
                                    obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                    obj.User = det.CREATEDATE.GetValueOrDefault().ToString("dd-MM-yyyy");
                                    ViewBag.Auth = obj;
                                    ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                    return View(viewtoDisplay, recg);
                                }
                            }
                        }
                    }
                        //  return Json(rec, JsonRequestBehavior.AllowGet);
                        //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                        // return Json(obj1, JsonRequestBehavior.AllowGet);
                    


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
                                recordId = (int)rec2.RECORDID.GetValueOrDefault();
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                    {


                                        if (rec2.POSTTYPE == Batch)
                                        {
                                            var splt = rec2.BATCHID.Split('_');
                                            if (splt[0] == "RVH")
                                            {
                                                suc = PostBulkUpload(rec2.BATCHID, rec2.USERID);
                                            }
                                            else
                                            {
                                                suc = PostRvDrAcctBulkUpload(rec2.BATCHID, rec2.USERID);
                                            }

                                        }
                                        else
                                        {
                                            suc = CreateNewRecord(recordId, rec2.BATCHID, rec2.USERID);

                                        }
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
                   
                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Revenue Head Record", null, fullName);
                    respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                   // TempData["msg"] = respMsg;
                  //  TempData["status"] = approve;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                }
                respMsg = "This request has already been processed by an authorizer.";
               // TempData["msg"] = respMsg;
                //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                //TempData["msg"] = respMsg;
               // return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private bool CloseMainRecord(int recordId, string bATCHID, string uSERID)
        {
            var dt = repoRvGroupTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == uSERID.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoRvGroup.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
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
            var dt = repoRvGroupTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                dt.STATUS = approve;
                var obj = new SM_REVENUEGROUP()
                {
                    GROUPCODE = dt.GROUPCODE,
                    GROUPNAME = dt.GROUPNAME,
                    ACCOUNT_ID = dt.ACCOUNT_ID,
                    MERCHANTID = dt.MERCHANTID,
                    STATUS = active,
                    CREATEDATE = DateTime.Now,
                    USERID = dt.USERID,
                    GLOBALACCOUNTFLAG = dt.GLOBALACCOUNTFLAG,
                    SETTLEMENT_FREQUENCY = dt.SETTLEMENT_FREQUENCY,
                    SET_DAYS = dt.SET_DAYS,

                };
                var rvCode = _repo.GetNextRVCode();
                obj.GROUPCODE = rvCode;
                repoRvGroup.Insert(obj);
                var ac = repoRvHeadTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).ToList();
                if (ac.Count > 0)
                {
                    foreach (var d in ac)
                    {
                        d.STATUS = approve;
                        var obj2 = new SM_REVENUEHEAD()
                        {
                            CODE = d.CODE, //drpCalcBasis.SelectedValue,
                                           //BATCHID = batchId,
                            DESCRIPTION = d.DESCRIPTION,
                            PAYMENTITEMID = d.PAYMENTITEMID,
                            ACCOUNT_ID = d.ACCOUNT_ID,
                            RVGROUPCODE = d.RVGROUPCODE,
                            STATUS = active,
                            CREATEDATE = curDate,
                            USERID = user_id,
                            SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY
                            
                        };

                        ProcessRvHeadParty(eventInsert, batchId, user_id, obj2);


                        obj.SM_REVENUEHEAD.Add(obj2);
                    }

                }
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId, string batchId, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoRvGroupTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoRvGroup.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
                dt.STATUS = approve;

                if (dm != null)
                {
                    dm.GROUPNAME = dt.GROUPNAME;
                    dm.ACCOUNT_ID = dt.ACCOUNT_ID;
                    dm.LAST_MODIFIED_UID = dt.USERID;
                    dm.LAST_MODIFIED_DATE = curDate;
                    dm.LAST_MODIFIED_AUTHID = User.Identity.Name;
                    dm.GLOBALACCOUNTFLAG = dt.GLOBALACCOUNTFLAG;
                    dm.SETTLEMENT_FREQUENCY = dt.SETTLEMENT_FREQUENCY;
                    dm.SET_DAYS = dt.SET_DAYS;
                    dm.STATUS = active;
                    var ac = repoRvHeadTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID == user_id).ToList();
                    if (ac.Count > 0)
                    {
                        foreach (var d in ac)
                        {
                            d.STATUS = approve;
                            if (d.EVENTTYPE == eventInsert || d.EVENTTYPE == eventEdit)
                            {
                                if (d.RECORDID == 0)
                                {
                                    var dm2 = repoRvHead.AllEager(e => e.CODE == d.CODE && e.RVGROUPCODE == d.RVGROUPCODE).FirstOrDefault();
                                    if (dm2 == null)
                                    {
                                        var obj2 = new SM_REVENUEHEAD()
                                        {
                                            CODE = d.CODE,
                                            DESCRIPTION = d.DESCRIPTION, //drpCalcBasis.SelectedValue,
                                                                         //BATCHID = batchId,
                                            PAYMENTITEMID = d.PAYMENTITEMID,
                                            ACCOUNT_ID = d.ACCOUNT_ID,
                                            RVGROUPCODE = dt.GROUPCODE,
                                            STATUS = active,
                                            CREATEDATE = DateTime.Now,
                                            USERID = user_id,
                                            SETTLEMENT_FREQUENCY = dt.SETTLEMENT_FREQUENCY,
                                        };
                                        dm.SM_REVENUEHEAD.Add(obj2);
                                        //insert revenue party
                                        ProcessRvHeadParty(eventInsert, batchId, user_id, obj2);
                                    }
                                    else
                                    {
                                        dm2.DESCRIPTION = d.DESCRIPTION;
                                        dm2.PAYMENTITEMID = d.PAYMENTITEMID;
                                        dm2.ACCOUNT_ID = d.ACCOUNT_ID;
                                        dm2.RVGROUPCODE = d.RVGROUPCODE;
                                        dm2.SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY;
                                        //insert revenue party
                                        ProcessRvHeadParty(eventEdit, batchId, user_id, dm2);
                                    }
                                }
                                else
                                {
                                    var dm2 = repoRvHead.AllEager(e => e.CODE == d.CODE && d.RVGROUPCODE == d.RVGROUPCODE).FirstOrDefault();
                                    if (dm2 != null)
                                    {
                                        dm2.DESCRIPTION = d.DESCRIPTION;
                                        dm2.PAYMENTITEMID = d.PAYMENTITEMID;
                                        dm2.RVGROUPCODE = d.RVGROUPCODE;
                                        dm2.ACCOUNT_ID = d.ACCOUNT_ID;
                                        //dm2.BATCHID = d.BATCHID;
                                        ProcessRvHeadParty(eventEdit, batchId, user_id, dm2);
                                    }
                                }
                            }
                            else
                            {
                                var dm2 = repoRvHead.Find(d.RECORDID);
                                if (dm2 != null)
                                {
                                    repoRvHead.Delete(dm2.ITBID);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        void ProcessRvHeadParty(string eventType, string batchId, string user_id, SM_REVENUEHEAD objRvHead)
        {
            if (eventType == eventInsert)
            {
                var ptyList = repoRvHeadPartyTemp.AllEager(e => e.BatchId == batchId && e.UserId != null && e.UserId.ToUpper() == user_id.ToUpper()).ToList();
                foreach (var pty in ptyList)
                {
                    var ptyObj = new SM_RevenuHeadParty()
                    {
                        PartyAccountId = pty.PartyAccountId,
                        PartyId = pty.PartyId,
                        PartyValue = pty.PartyValue,
                        UserId = pty.UserId,
                        CreateDate = DateTime.Now,
                    };
                    objRvHead.SM_RevenuHeadParty.Add(ptyObj);
                }
            }
            else
            {
                var extRvHeadParty = repoRvHeadParty.AllEager(d => d.RvCodeItbId == objRvHead.ITBID).ToList();

                var ptyList = repoRvHeadPartyTemp.AllEager(e => e.BatchId == batchId && e.UserId != null && e.UserId.ToUpper() == user_id.ToUpper()).ToList();
                var extRvHeadToBeRemoved = extRvHeadParty.Where(d => !ptyList.Select(f => f.PartyId).Contains(d.PartyId));
                foreach (var pty in ptyList)
                {
                    var extparty = extRvHeadParty.Where(d => d.PartyId == pty.PartyId).FirstOrDefault();
                    if (extparty != null)
                    {
                        extparty.PartyValue = pty.PartyValue;
                        extparty.PartyAccountId = pty.PartyAccountId;
                    }
                    else
                    {
                        var ptyObj = new SM_RevenuHeadParty()
                        {
                            PartyAccountId = pty.PartyAccountId,
                            PartyId = pty.PartyId,
                            PartyValue = pty.PartyValue,
                            UserId = pty.UserId,
                            CreateDate = DateTime.Now,
                        };
                        objRvHead.SM_RevenuHeadParty.Add(ptyObj);
                    }
                }
                foreach (var pty in extRvHeadToBeRemoved)
                {
                    repoRvHeadParty.Delete(pty.ItbId);
                }
            }
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
                                RejectBatch(recordId,rec2.BATCHID,rec2.USERID);

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
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Revenue Head Rejection", Narration, fullName);
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
            var recP = repoRvGroupTemp.AllEager(d => d.ITBID == rECORDID && d.USERID == uSERID).FirstOrDefault();
            if (recP != null)
            {
                recP.STATUS = reject;
            }

            var recPP = repoRvHeadTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
            foreach (var d in recPP)
            {
                d.STATUS = reject;
            }

        }

        public IList<RvHeadObj> GetRvHeadLines()
            {
                //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
                return SessionHelper.GetRvHead(Session).Lines;
            }

        [MyAuthorize]
        public ActionResult RvHeadUpld(string mid)
        {
            // BindCombo();
            ViewBag.Mid = mid;
            return View();
        }

        [MyAuthorize]
        public ActionResult RvDebitAcctUpld(string mid)
        {
            // BindCombo();
            ViewBag.Mid = mid;
            return View();
        }
        #region Revenue Head Upload
        protected int ValidateUpload()
        {
            var rv = new RvHeadUpldSession();
            var rec = rv.GetRvHeadUpload(User.Identity.Name);

            int totalErrorCount = 0;
            foreach (var t in rec)
            {
                int errorCount = 0;
                var validationErrorMessage = new List<string>();

                var existRvGroup = repoRvGroup.AllEager(d => d.GROUPCODE == t.GROUPCODE).FirstOrDefault();
                if (existRvGroup == null)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""REVENUE GROUP CODE"" does not exist.");
                }
                else
                {
                    
                    //VALIDATE BANK ACCOUNT HERE
                   var existBank = repoMAcct.AllEager(d => d.MERCHANTID == existRvGroup.MERCHANTID && d.DEPOSIT_BANKCODE == t.BANKCODE && d.DEPOSIT_ACCOUNTNO == t.BANKACCOUNT).FirstOrDefault();
                    if (existBank == null)
                    {
                        errorCount++;
                        validationErrorMessage.Add(@"""BANK ACCOUNT"" has not been setup for the merchant attached to the revenue group.");
                    }
                    else
                    {
                        t.ACCOUNT_ID = existBank.ITBID;
                    }
                }
                //var existScheme = repoScheme.AllEager(d => d.CARDSCHEME == t.CARDSCHEME).Count();
                //if (existScheme == 0)
                //{
                //    errorCount++;
                //    validationErrorMessage.Add(@"""CARD SCHEME"" does not exist.");
                //}
                if (errorCount == 0)
                {
                    var cnt = _repo.ValidateRvHead(t.CODE, t.GROUPCODE);
                    if (cnt > 0)
                    {
                        errorCount++;
                        validationErrorMessage.Add("Revenue Head Already exist.");
                    }
                }

                if (errorCount == 0)
                {
                    t.VALIDATIONERRORSTATUS = false;
                    t.VALIDATIONERRORMESSAGE = "";
                }
                else
                {
                    totalErrorCount++;
                    t.VALIDATIONERRORSTATUS = true;
                    t.VALIDATIONERRORMESSAGE = GetStringFromList(validationErrorMessage);
                }
                var rst = rv.PostRvHeadUpload(t, 2, User.Identity.Name);
                //SessionHelper.GetCart(Session).UpdateItem(t);
            }
            if (rec.Count > 0)
            {
                if (totalErrorCount > 0)
                {

                    //pnlResponse.Visible = true;
                    //pnlResponse.CssClass = "alert alert-danger alert-dismissable alert-bold";
                    //pnlResponseMsg.Text = string.Format("{0} Record(s) Failed Validation from Batch...", totalErrorCount);
                    //if (totalErrorCount == rec.Count)
                    //{
                    //    btnProcess.Enabled = false;
                    //}
                    //else
                    //{
                    //    btnProcess.Enabled = true;
                    //}

                }
                else
                {
                    //pnlResponse.Visible = true;
                    //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
                    //pnlResponseMsg.Text = "Batch Validated Successfully...You can now save for further processing";
                    // btnProcess.Enabled = false;
                    //btnProcess.Enabled = true;
                }
            }
            return totalErrorCount;
        }

        string GetStringFromList(List<string> val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div style=""color:red;font-size:11px"">");
            foreach (var d in val)
            {
                sb.AppendLine(@"<i class=""fa-arrow-right fa""> </i> " + d + "<br/>");
            }
            sb.AppendLine("</div>");
            var l = sb.ToString();
            return l;
        }
        string GetString(string val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div style=""color:red;font-size:11px"">");

            sb.AppendLine(@"<i class=""fa-arrow-right fa""> </i> " + val + "<br/>");

            sb.AppendLine("</div>");
            var l = sb.ToString();
            string msg = string.Format(@"<small id=""disWarning"" style=""color: red"" data-toggle=""popover"" data-trigger=""hover""  
                            data-html=""true2"" data-content='{0}'><i class=""fa fa-info-circle""></i> New</small>", l);

            return msg;
        }

        protected bool PostBulkUpload(string batchid, string user_id)
        {
            try
            {
                DateTime curDate = DateTime.Now;
                var rec = repoRvHeadTemp.AllEager(d => d.BATCHID == batchid && d.USERID == user_id).ToList();
                foreach (var t in rec)
                {
                    t.STATUS = approve;
                    var cnt = _repo.ValidateRvHead(t.CODE, t.RVGROUPCODE);
                    if (cnt == 0)
                    {
                        //var rv = repoRvGroup.AllEager(f => f.GROUPCODE == t.RVGROUPCODE).FirstOrDefault();
                        //if(rv != null)
                        //{

                        //}

                        var bb = new SM_REVENUEHEAD()
                        {
                            RVGROUPCODE = t.RVGROUPCODE,
                            CODE = t.CODE,
                            DESCRIPTION = t.DESCRIPTION,
                            PAYMENTITEMID = t.PAYMENTITEMID,
                            CREATEDATE = DateTime.Now,
                            STATUS = active,
                            USERID = t.USERID,
                            MERCHANTID = t.MERCHANTID,
                            ACCOUNT_ID = t.ACCOUNT_ID,
                            SETTLEMENT_FREQUENCY = t.SETTLEMENT_FREQUENCY,
                            BANKACCOUNT = t.BANKACCOUNT,
                            BANKCODE = t.BANKCODE,
                        };
                        repoRvHead.Insert(bb);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           // return false;
        }

        //[HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Validate()
        {
            try
            {

                var rv = new RvHeadUpldSession();
                var errCount = ValidateUpload();
                var rec = rv.GetRvHeadUpload(User.Identity.Name);
                var sucCount = rec.Count - errCount;
                var html = PartialView("_RvHeadUpld", rec).RenderToString();
                return Json(new { data_html = html, RespCode = 0, RespMessage = "Record ", SucCount = sucCount, FailCount = errCount });
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Validation. " });
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult UploadFiles()
        {
            IList<RvHeadUpldObj> model = null;
            try
            {
                var rc = Request.Files;
                //  var dd = Request.Form["requestType"];
                if (rc != null)
                {
                    var file = rc[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var stream = file.InputStream;
                        var fileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        if (ext != ".xlsx")
                        {
                            return Json(new { RespCode = 1, RespMessage = "Please Upload Using .xlsx file" });
                        }

                        if (!Directory.Exists(Server.MapPath("~/UploadFiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                        }
                        var path = Path.Combine(Server.MapPath("~/UploadFiles"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        var dataList = ExcelReader.GetDataToList(path, addRecord); // ExxcellReaderClosedXml.GetDataToList(path, addRecord);
                                                                                   //int cnt = 0;
                        var rv = new RvHeadUpldSession();
                        var cnt = rv.PostRvHeadUploadBulk(dataList.ToList(), User.Identity.Name);

                        if (cnt > 0)
                        {
                            var rst = rv.GetRvHeadUpload(User.Identity.Name);
                            var html = PartialView("_RvHeadUpld", rst).RenderToString();
                            return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        }
                        else
                        {
                            var html = PartialView("_RvHeadUpld").RenderToString();
                            return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                        }
                    }
                }
                // If we got this far, something failed, redisplay form
                //return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            return Json(new { data = model, BatchId = "", RespCode = 0, RespMessage = "File Uploaded Successfully" });
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Process()
        {
            string batchId = "";
            try
            {
                var rv = new RvHeadUpldSession();
                var rec = rv.GetRvHeadUpload(User.Identity.Name);
                //  isUp = userInstitutionItbid == 1 ? true : false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 30, 0)))
                //{
                    int errorcnt = 0;

                    DateTime curDate = DateTime.Now;

                    batchId = "RVH_" + SmartObj.GenRefNo2();

                    int i = 0;
                    int valCount = rec.Count(f => f.VALIDATIONERRORSTATUS == false);
                    foreach (var d in rec)
                    {
                        if (i == 0)
                        {
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                // RECORDID = objG.ITBID,
                                STATUS = open,
                                //TABLENAME = "SM_FREQUENCY",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                INSTITUTION_ITBID = institutionId,
                                BATCHID = batchId,
                                POSTTYPE = Batch

                            };
                            repoAuth.Insert(auth);
                        }
                        i++;
                        if (d.VALIDATIONERRORSTATUS == true)
                        {
                            errorcnt++;
                            continue;
                        }

                        var obj = new SM_REVENUEHEADTEMP()// SM_MERCHANTTERMINALUPLD()
                        {
                            RVGROUPCODE = d.GROUPCODE,
                            CODE = d.RVCODE,
                            DESCRIPTION = d.DESCRIPTION,
                            PAYMENTITEMID = d.PAYMENTITEMID,
                            BATCHID = batchId,
                            CREATEDATE = curDate,
                            STATUS = open,
                            USERID = User.Identity.Name,
                            SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY,
                            BANKCODE = d.BANKCODE,
                            BANKACCOUNT = d.BANKACCOUNT,
                            ACCOUNT_ID = d.ACCOUNT_ID
                            

                        };
                        repoRvHeadTemp.Insert(obj);
                        //cnt++;
                    }


                    var rst = uow.Save(User.Identity.Name);
                    if (rst > 0)
                    {

                        //SessionHelper.GetCart(Session).Clear();
                        rv.PurgeRvHeadUpload(User.Identity.Name);

                        try
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, string.Format("Revenue Head Upload Batch #{0}", batchId));
                        }
                        catch
                        {

                        }
                        //txscope.Complete();
                    }
                    else
                    {
                        return Json(new { RespCode = 1, RespMessage = "Problem Processing Request." });
                    }
                //}
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });

            }
            var msg = string.Format("<i class='fa fa-check' ></i> Record with Batch-ID #{0} Processed SuccessFully and has been forwarded for authorization", batchId);
            return Json(new { RespCode = 0, RespMessage = msg });

        }
        private static RvHeadUpldObj addRecord(IList<string> rowData, IList<string> columnNames)
        {
            try
            {
                var obj = new RvHeadUpldObj()
                {
                    RVCODE = rowData[0].Trim(),
                    DESCRIPTION = rowData[1].Trim(),
                    GROUPCODE = rowData[2].Trim(),
                    PAYMENTITEMID = rowData[3].ToInt32(),
                    BANKCODE = rowData[4].Trim(),
                    BANKACCOUNT = rowData[5].Trim(),
                    SETTLEMENT_FREQUENCY = rowData[6].ToInt32(),
                    VALIDATIONERRORSTATUS = true,
                };
                return obj;
            }
            catch (Exception ex)
            {
                return new RvHeadUpldObj();
            }
        }

        #endregion Revenue Head Upload

        #region Revenue Debit Account Upload
        protected int ValidateRvDrAcctUpload()
        {
            var rv = new RvDrAcctUpldSession();
            var rec = rv.GetRvDrAcctUpload(User.Identity.Name);

            int totalErrorCount = 0;
            foreach (var t in rec)
            {
                int errorCount = 0;
                var validationErrorMessage = new List<string>();

                var existBank = repoRvGroup.AllEager(d => d.GROUPCODE == t.RVGROUPCODE).Count();
                if (existBank == 0)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""REVENUE GROUP CODE"" does not exist.");
                }
                var existScheme = repoM.AllEager(d => d.MERCHANTID == t.MERCHANTID).Count();
                if (existScheme == 0)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""MERCHANT ID"" does not exist.");
                }
                var existInst = repoInst.AllEager(d => d.CBN_CODE == t.BANKCODE).Count();
                if (existInst == 0)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""BANK CODE"" does not exist.");
                }
                //if (errorCount == 0)
                //{
                //    //var cnt = _repo.ValidateRvHead(t.RVCODE, t.GROUPCODE);
                //    //if (cnt > 0)
                //    //{
                //    //    errorCount++;
                //    //    validationErrorMessage.Add("Revenue Head Already exist.");
                //    //}
                //}

                if (errorCount == 0)
                {
                    t.VALIDATIONERRORSTATUS = false;
                    t.VALIDATIONERRORMESSAGE = "";
                }
                else
                {
                    totalErrorCount++;
                    t.VALIDATIONERRORSTATUS = true;
                    t.VALIDATIONERRORMESSAGE = GetStringFromList(validationErrorMessage);
                }
                var rst = rv.PostRvDrAcctUpload(t, 2, User.Identity.Name);
                //SessionHelper.GetCart(Session).UpdateItem(t);
            }
            if (rec.Count > 0)
            {
                if (totalErrorCount > 0)
                {

                    //pnlResponse.Visible = true;
                    //pnlResponse.CssClass = "alert alert-danger alert-dismissable alert-bold";
                    //pnlResponseMsg.Text = string.Format("{0} Record(s) Failed Validation from Batch...", totalErrorCount);
                    //if (totalErrorCount == rec.Count)
                    //{
                    //    btnProcess.Enabled = false;
                    //}
                    //else
                    //{
                    //    btnProcess.Enabled = true;
                    //}

                }
                else
                {
                    //pnlResponse.Visible = true;
                    //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
                    //pnlResponseMsg.Text = "Batch Validated Successfully...You can now save for further processing";
                    // btnProcess.Enabled = false;
                    //btnProcess.Enabled = true;
                }
            }
            return totalErrorCount;
        }

      

        protected bool PostRvDrAcctBulkUpload(string batchid, string user_id)
        {
            try
            {
                DateTime curDate = DateTime.Now;
                var rec = repoRvAcctTemp.AllEager(d => d.BATCHID == batchid && d.USERID == user_id).ToList();
                foreach (var t in rec)
                {
                    t.STATUS = approve;

                    var re = repoRvAcct.AllEagerLocal(d => d.RVGROUPCODE == t.RVGROUPCODE && d.MERCHANTID == t.MERCHANTID && d.AGENT_CODE == t.AGENTCODE).FirstOrDefault();
                    if (re == null)
                    {
                        var bb = new SM_REVENUEBANKACCT()
                        {
                            RVGROUPCODE = t.RVGROUPCODE,
                             AGENT_CODE= t.AGENTCODE,
                            DR_ACCOUNTNO = t.DR_ACCOUNTNO,
                            CREATEDATE = DateTime.Now,
                            STATUS = active,
                            USERID = t.USERID,
                            MERCHANTID = t.MERCHANTID,
                            DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME,
                            DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE,
                            
                        };
                        repoRvAcct.Insert(bb);
                    }
                    else
                    {
                        re.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                        re.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                        re.DR_ACCOUNTNO = t.DR_ACCOUNTNO;
                    }
                   
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            //return false;
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult ValidateRvDrAcct()
        {
            try
            {

                var rv = new RvDrAcctUpldSession();
                var errCount = ValidateRvDrAcctUpload();
                var rec = rv.GetRvDrAcctUpload(User.Identity.Name);
                var sucCount = rec.Count - errCount;
                var html = PartialView("_RvDebitAcctUpld", rec).RenderToString();
                return Json(new { data_html = html, RespCode = 0, RespMessage = "Record ", SucCount = sucCount, FailCount = errCount });
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Validation. " });
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult UploadRvDrAcctFiles()
        {
            IList<RvDrAcctUpldObj> model = null;
            try
            {
                var rc = Request.Files;
                //  var dd = Request.Form["requestType"];
                if (rc != null)
                {
                    var file = rc[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var stream = file.InputStream;
                        var fileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        if (ext != ".xlsx")
                        {
                            return Json(new { RespCode = 1, RespMessage = "Please Upload Using .xlsx file" });
                        }

                        if (!Directory.Exists(Server.MapPath("~/UploadFiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                        }
                        var path = Path.Combine(Server.MapPath("~/UploadFiles"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        var dataList = ExcelReader.GetDataToList(path, addRvDrAcctRecord); // ExxcellReaderClosedXml.GetDataToList(path, addRecord);
                                                                                   //int cnt = 0;
                        var rv = new RvDrAcctUpldSession();
                        var cnt = rv.PostRvDrAcctUploadBulk(dataList.ToList(), User.Identity.Name);

                        if (cnt > 0)
                        {
                            var rst = rv.GetRvDrAcctUpload(User.Identity.Name);
                            var html = PartialView("_RvDebitAcctUpld", rst).RenderToString();
                            return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                        }
                        else
                        {
                            var html = PartialView("_RvDebitAcctUpld").RenderToString();
                            return Json(new { RespCode = 1, RespMessage = "Problem processing file upload." });
                        }
                    }
                }
                // If we got this far, something failed, redisplay form
                //return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { data = model, RespCode = 1, RespMessage = ex.Message });
            }
            return Json(new { data = model, BatchId = "", RespCode = 0, RespMessage = "File Uploaded Successfully" });
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult ProcessRvDrAcct()
        {
            string batchId = "";
            try
            {
                var rv = new RvDrAcctUpldSession();
                var rec = rv.GetRvDrAcctUpload(User.Identity.Name);
                //  isUp = userInstitutionItbid == 1 ? true : false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 30, 0)))
                //{
                int errorcnt = 0;

                DateTime curDate = DateTime.Now;

                batchId = "RVA_" + SmartObj.GenRefNo2();

                int i = 0;
                int valCount = rec.Count(f => f.VALIDATIONERRORSTATUS == false);
                foreach (var d in rec)
                {
                    if (i == 0)
                    {
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = eventInsert,
                            MENUID = menuId,
                            //MENUNAME = "",
                            // RECORDID = objG.ITBID,
                            STATUS = open,
                            //TABLENAME = "SM_FREQUENCY",
                            URL = Request.FilePath,
                            USERID = User.Identity.Name,
                            INSTITUTION_ITBID = institutionId,
                            BATCHID = batchId,
                            POSTTYPE = Batch

                        };
                        repoAuth.Insert(auth);
                    }
                    i++;
                    if (d.VALIDATIONERRORSTATUS == true)
                    {
                        errorcnt++;
                        continue;
                    }

                    var obj = new SM_REVENUEBANKACCTTEMP()// SM_MERCHANTTERMINALUPLD()
                    {
                        RVGROUPCODE = d.RVGROUPCODE,
                        DR_ACCOUNTNO = d.BANKACCTNO,
                        DEPOSIT_ACCTNAME = d.BANKACCTNAME,
                        DEPOSIT_BANKCODE = d.BANKCODE,
                        MERCHANTID = d.MERCHANTID,
                        EVENTTYPE  = eventEdit,
                        BATCHID = batchId,
                        CREATEDATE = curDate,
                        STATUS = open,
                        USERID = User.Identity.Name,
                        AGENTCODE = d.AGENT_CODE
                    };
                    repoRvAcctTemp.Insert(obj);
                    //cnt++;
                }


                var rst = uow.Save(User.Identity.Name);
                if (rst > 0)
                {

                    //SessionHelper.GetCart(Session).Clear();
                    rv.PurgeRvDrAcctUpload(User.Identity.Name);

                    try
                    {
                        EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, string.Format("Revenue Head Upload Batch #{0}", batchId));
                    }
                    catch
                    {

                    }
                    //txscope.Complete();
                }
                else
                {
                    return Json(new { RespCode = 1, RespMessage = "Problem Processing Request." });
                }
                //}
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });

            }
            var msg = string.Format("<i class='fa fa-check' ></i> Record with Batch-ID #{0} Processed SuccessFully and has been forwarded for authorization", batchId);
            return Json(new { RespCode = 0, RespMessage = msg });

        }
        private static RvDrAcctUpldObj addRvDrAcctRecord(IList<string> rowData, IList<string> columnNames)
        {
            try
            {
                var obj = new RvDrAcctUpldObj()
                {
                    MERCHANTID = rowData[0].Trim(),
                    RVGROUPCODE = rowData[1].Trim(),
                    AGENT_CODE = rowData[2].Trim(),
                    BANKCODE = rowData[3].Trim(),
                    BANKACCTNO = rowData[4].Trim(),
                    BANKACCTNAME = rowData[5].Trim(),
                    VALIDATIONERRORSTATUS = true,
                };
                return obj;
            }
            catch (Exception ex)
            {
                return new RvDrAcctUpldObj();
            }
        }

        #endregion Revenue Debit Account Uploadj 

        #region RevenueSharingParty
        public ActionResult AddSharingPartyRowView()
        {
            try
            {
                BindComboSharingParty();
                var html = PartialView("_AddSharingPartyRow").RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request."}, JsonRequestBehavior.AllowGet);
            }

        }

        void BindComboSharingParty()
        {
            var party = _repo.GetParty(0,true,"Active");
            ViewBag.PartyList = new SelectList(party, "ITBID", "PARTY_DESC");
        }

        public ActionResult GetPartyAccount(int id)
        {
            try
            {
                var party = _repo.GetPartyAcct(id).Select(d=> new DropdownObj() {Code = d.ITBID.ToString(),Description = d.DEPOSIT_ACCOUNTNO + "-" + d.DEPOSIT_ACCTNAME });            
                return Json(new { RespCode = 0, RespMessage = "", data = party }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request." }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ViewRvHeadParty(string id)
        {
            try
            {

                var rv = new RvHeadSession();
                var rec = _repo.GetRvHeadPartyTemp(id);
                if (rec != null)
                {
                    ViewBag.HeaderTitle = "Revenue Head Party";
                    return PartialView("_ViewRVHeadParty", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        #endregion RevenueSharingParty
    }
}