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
    public class SettlementRuleController : Controller
    {
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
            private readonly IRepository<SM_AUTHLIST> repoAuth = null;

            //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
            private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
            private readonly IRepository<SM_SETTLEMENTOPTION> repoSetOption = null;
            private readonly IRepository<SM_SETTLEMENTRULE> repoSetRule = null;
            private readonly IRepository<SM_SETTLEMENTRULETEMP> repoSetRuleTemp = null;
            private readonly IRepository<SM_SETTLEMENTOPTIONTEMP> repoSetOptionTemp = null;
            //private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
            //private readonly IRepository<SM_CURRENCY> repoCurrency = null;
            //private readonly IRepository<SM_FREQUENCY> repoFreq = null;
            //private readonly IRepository<SM_SECTOR> repoSec = null;
            //private readonly IRepository<SM_CHANNELS> repoChannel = null;

            //private readonly IRepository<SM_PARTYCATEGORY> repoMC = null;

            private readonly IRepository<SM_PARTYTYPE> repoPartyType = null;

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
            int menuId = 26;
            int institutionId;
            int roleId;
        string fullName;string deptCode;
            int checkerNo = 1;
        // GET: Roles
        public SettlementRuleController()
        {
            uow = new UnitOfWork();
            //repoScheme = new Repository<SM_CARDSCHEME>(uow);
            //repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoSetOption = new Repository<SM_SETTLEMENTOPTION>(uow);
            repoSetRule = new Repository<SM_SETTLEMENTRULE>(uow);
            repoSetOptionTemp = new Repository<SM_SETTLEMENTOPTIONTEMP>(uow);
            repoSetRuleTemp = new Repository<SM_SETTLEMENTRULETEMP>(uow);
            //repoFreq = new Repository<SM_FREQUENCY>(uow);
            //repoSec = new Repository<SM_SECTOR>(uow);
            // repoMC = new Repository<SM_PARTYCATEGORY>(uow);
            //repoVal = new Repository<SM_MERCHANTCONFIG>(uow);
            repoPartyType = new Repository<SM_PARTYTYPE>(uow);
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
        public async Task<ActionResult> SettlementRuleList()
            {
                try
                {
                    var rec = await _repo.GetSettlementOptionAsync(0, true);  //repoSession.FindAsync(id);              
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
            public ActionResult Add(SettlementOptionObj model, string m)
            {
                try
                {
                    if (model.ITBID == 0)
                    {
                        ViewBag.ButtonText = "Save";
                        ViewBag.HeaderTitle = "Add Settlement Rule";
                    }
                    else
                    {
                        ViewBag.ButtonText = "Update";
                        ViewBag.HeaderTitle = "Edit Settlement Rule";
                    }

                    ViewBag.MenuId = m;
                    menuId = SmartUtil.GetMenuId(m);
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
                        //    ViewBag.PartyAcct = GetSettlementRuleLines();
                        //    return View("Add", model);
                        //};
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_SETTLEMENTOPTIONTEMP BType = new SM_SETTLEMENTOPTIONTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                CHANNEL = model.CHANNEL,
                                DESCRIPTION = model.DESCRIPTION,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                            };

                            repoSetOptionTemp.Insert(BType);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                SaveSettlementRuleTemp(eventInsert, BType);

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
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Settlement Rule Record");

                                    //txscope.Complete();
                                    TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", new { m = m });
                                }
                            }
                        }
                    }
                    else
                    {
                        var errMsg = "";
                        //if (validateForm(model, eventEdit, out errMsg))
                        //{
                        //    ViewBag.Message = errMsg; // "Carscheme already Exist.";
                        //    BindCombo();
                        //    ViewBag.PartyAcct = GetSettlementRuleLines();
                        //    return View("Add", model);
                        //};
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_SETTLEMENTOPTIONTEMP BType = new SM_SETTLEMENTOPTIONTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                CHANNEL = model.CHANNEL,
                                DESCRIPTION = model.DESCRIPTION,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.ITBID,
                            };
                            repoSetOptionTemp.Insert(BType);
                            //  var rst1 = new AuthListUtil().SaveLog(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                            if (rst)
                            {
                                SaveSettlementRuleTemp(eventEdit, BType);
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


                                    //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Settlement Rule Record");
                                    //txscope.Complete();
                                    TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", new { m = m });
                                }
                            }
                        }

                    //}
                        // If we got this far, something failed, redisplay form
                        //return Json(new { RespCode = 1, RespMessage = errorMsg });
                        BindCombo();
                        ViewBag.PartyAcct = GetSettlementRuleLines();
                        ViewBag.Message = errorMsg;
                        return View("Add", model);
                    //}
                }
                catch (SqlException ex)
                {
                    BindCombo();
                    ViewBag.PartyAcct = GetSettlementRuleLines();
                    ViewBag.Message = ex.Message;
                    return View("Add", model);
                    //return Json(new { RespCode = 1, RespMessage = ex.Message });
                }
                catch (Exception ex)
                {
                    BindCombo();
                    ViewBag.PartyAcct = GetSettlementRuleLines();
                    ViewBag.Message = ex.Message;
                    return View("Add", model);
                    // return Json(new { RespCode = 1, RespMessage = ex.Message });
                }
                BindCombo();
                ViewBag.PartyAcct = GetSettlementRuleLines();
                ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

            }

            //private bool validateForm(PartyObj obj, string eventType, out string errorMsg)
            //{
            //    var sb = new StringBuilder();
            //    var errCount = 0;
            //    if (eventType == eventInsert)
            //    {
            //        var existCbnCode = repoSetOption.AllEager(f => f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_CODE != null && f.PARTY_CODE == obj.PARTY_CODE).Count();
            //        if (existCbnCode > 0)
            //        {
            //            sb.AppendLine(@"""PARTY CODE"" already exist for another Party");
            //            errCount++;
            //        }
            //        var existShortName = repoSetOption.AllEager(f => f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
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
            //        var existCbnCode = repoSetOption.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_CODE != null && f.PARTY_CODE == obj.PARTY_CODE).Count();
            //        if (existCbnCode > 0)
            //        {
            //            sb.AppendLine(@"""PARTY CODE"" already exist for another Party");
            //            errCount++;
            //        }
            //        var existShortName = repoSetOption.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
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


        void SaveSettlementRuleTemp(string eventType, SM_SETTLEMENTOPTIONTEMP rec)
        {
            if (eventType == "New")
            {
                var col = GetSettlementRuleLines();
                foreach (var d in col)
                {
                    var obj = new SM_SETTLEMENTRULETEMP()
                    {
                        PARTYTYPE_CAP = d.PARTYTYPE_CAP, //drpCalcBasis.SelectedValue,
                        BATCHID = rec.BATCHID,
                        PARTYTYPE_CODE = d.PARTYTYPE_CODE,
                        PARTYTYPE_VALUE = d.PARTYTYPE_VALUE,
                        SETTLEMENTOPTION_ID = rec.ITBID,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,

                    };
                    repoSetRuleTemp.Insert(obj);
                }

            }
            else
            {
                var col = GetSettlementRuleLines();
                foreach (var d in col)
                {
                    //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                    //{
                    //    continue;
                    //}
                    if (d.NewRecord || d.Updated || d.Deleted)
                    {
                        var obj = new SM_SETTLEMENTRULETEMP()
                        {
                            PARTYTYPE_CAP = d.PARTYTYPE_CAP, //drpCalcBasis.SelectedValue,
                            BATCHID = rec.BATCHID,
                            PARTYTYPE_CODE = d.PARTYTYPE_CODE,
                            PARTYTYPE_VALUE = d.PARTYTYPE_VALUE,
                            SETTLEMENTOPTION_ID = rec.ITBID,
                            STATUS = open,
                            RECORDID = d.NewRecord ? 0 : d.ITBID,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,

                        };

                        repoSetRuleTemp.Insert(obj);
                    }
                }
            }

        }
        void BindCombo(string acq_selected = null)
        {
            var ptype = _repo.GetChannel(0, true, "Active");
            //var country = _repo.GetCountry(0, true, "Active");

            ViewBag.ChannelList = new SelectList(ptype, "CODE", "DESCRIPTION");
            //ViewBag.CountryList = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");

            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

        }
           
            public async Task<ActionResult> Add(int id = 0, string m = null)
            {
                try
                {
                    SessionHelper.GetSettlementRule(Session).Clear();
                    ViewBag.MenuId = HttpUtility.UrlDecode(m);
                    BindCombo();
                    if (id == 0)
                    {
                        ViewBag.HeaderTitle = "Add Settlement Rule";
                        ViewBag.StatusVisible = false;
                        ViewBag.ButtonText = "Save";
                    GetPriv();
                        return View("Add", new SettlementOptionObj());

                        // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //var d = _repo.GetSession(0, true);
                        ViewBag.HeaderTitle = "Edit Settlement Rule";
                        ViewBag.StatusVisible = true;
                        ViewBag.ButtonText = "Update";
                        var rec = await _repo.GetSettlementOptionAsync(id, false);
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
                var bankList = _repo.GetPartyType(0, true, "Active");
                //var country = _repo.GetCountry(0, true, "Active");

                ViewBag.PType = new SelectList(bankList, "PARTYTYPE_CODE", "PARTYTYPE_DESC");
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
                        ViewBag.HeaderTitle = "Add Settlement Rule";
                        ViewBag.ButtonText = "Add";
                        return PartialView("_AddRule", new SettlementRuleObj());
                        // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //var d = _repo.GetSession(0, true);
                        ViewBag.HeaderTitle = "Edit Settlement Rule";
                        ViewBag.StatusVisible = true;
                        ViewBag.ButtonText = "Update";
                        //var rec = await _repo.GetPartyAcctAsync(id);
                        //if (rec == null)
                        //{
                        //    return null;
                        //}
                        //var model = rec.FirstOrDefault();
                        return PartialView("_AddRule", null);

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
            //        htmlString = PartialView("_ViewRule", rec).RenderToString();
            //        return Json(new { data_msc = htmlString, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);
            //    }
            //    catch (Exception ex)
            //    {
            //        htmlString = PartialView("_ViewRule").RenderToString();
            //        return Json(new { data_msc = htmlString, RespCode = 1, RespMessage = "Problem Processing Request" }, JsonRequestBehavior.AllowGet);

            //    }
            //}

            [HttpPost]
            // [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public ActionResult AddAcct(SettlementRuleObj model, string m)
            {
                string msg = "";
                //var bankName = "";
                //var cntry_code = "";
                //var bankAddress = "";
                try
                {
                    var lst =   GetSettlementRuleLines().ToList();
                    if (model.ITBID == 0)
                    {
                        var exist = lst.Exists(r => r.PARTYTYPE_CODE == model.PARTYTYPE_CODE);
                        if (exist)
                        {
                            msg = "Party Type Code Already exist. Duplicate Record is not allowed.";
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

                        //var inst = repoPartyType.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                        //if (inst != null)
                        //{
                        //    bankName = inst.INSTITUTION_NAME;
                        //    cntry_code = inst.INSTITUTION_COUNTRY;
                        //    bankAddress = inst.INSTITUTION_ADDRESS;
                        //}

                        var obj = new SettlementRuleObj()
                        {
                            PARTYTYPE_CAP = model.PARTYTYPE_CAP,
                            PARTYTYPE_CODE = model.PARTYTYPE_CODE,
                            PARTYTYPE_VALUE = model.PARTYTYPE_VALUE,
                            USERID = User.Identity.Name,
                            ITBID = itbid,
                            NewRecord = true,
                        };


                        SessionHelper.GetSettlementRule(Session).AddItem(obj);
                        var w = GetSettlementRuleLines().ToList();
                        var html = PartialView("_ViewRule", w).RenderToString();
                        msg = "Record Added to List";
                        return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {
                        var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                        var existRec = lst.Where(r => r.PARTYTYPE_CODE == model.PARTYTYPE_CODE).FirstOrDefault();
                        if (existRec != null) // not expected to be null
                        {
                            if (oldRec.ITBID != existRec.ITBID)
                            {
                                msg = "Party Type Code Already exist. Duplicate Record is not allowed.";
                                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                            }
                        }

                        //var inst = repoPartyType.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                        //if (inst != null)
                        //{
                        //    bankName = inst.INSTITUTION_NAME;
                        //    cntry_code = inst.INSTITUTION_COUNTRY;
                        //    bankAddress = inst.INSTITUTION_ADDRESS;
                        //}

                        var obj = new SettlementRuleObj()
                        {
                            PARTYTYPE_CODE = model.PARTYTYPE_CODE,
                            PARTYTYPE_VALUE = model.PARTYTYPE_VALUE,
                            PARTYTYPE_CAP = model.PARTYTYPE_CAP,
                            USERID = User.Identity.Name,
                            ITBID = model.ITBID,
                            NewRecord = oldRec.NewRecord,
                            Updated = true,
                        };

                        SessionHelper.GetSettlementRule(Session).UpdateItem(obj);

                        var w = GetSettlementRuleLines().ToList();
                        var html = PartialView("_ViewRule", w).RenderToString();
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
            async Task<List<SettlementRuleObj>> BindAccount(int itbid)
            {
                SessionHelper.GetSettlementRule(Session).Clear();
                var rec = await _repo.GetSettlementRuleAsync(itbid);
                foreach (var d in rec)
                {
                    SessionHelper.GetSettlementRule(Session).AddItem(d);
                }
                return rec;

            }

            List<SettlementRuleObj> BindAccountTemp(int party_itbid, string batchId, string user_id)
            {
                // SessionHelper.GetMccMsc(Session).Clear();
                var rec = _repo.GetSettlementRuleTemp(party_itbid, batchId, user_id);

                return rec;

            }

            public ActionResult EditAcct(decimal id, string m)
            {
                try
                {
                    ViewBag.MenuId = m;

                    var lst = GetSettlementRuleLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        BindComboMsc();
                        ViewBag.ButtonText = "Update";
                        return PartialView("_AddRule", rec);
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

                    var lst = GetSettlementRuleLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        if (rec.NewRecord)
                        {
                            SessionHelper.GetSettlementRule(Session).RemoveLine(rec.ITBID);

                        }
                        else
                        {
                            SessionHelper.GetSettlementRule(Session).MarkForDelete(rec.ITBID);
                        }
                        var lst2 = GetSettlementRuleLines().ToList();
                        var html2 = PartialView("_ViewRule", lst2).RenderToString();
                        return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {

                }
                var lst3 = GetSettlementRuleLines().ToList();
                var html = PartialView("_ViewRule", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }
            public ActionResult UndoAcct(decimal id, string m)
            {
                try
                {
                    ViewBag.MenuId = m;

                    var lst = GetSettlementRuleLines().ToList();

                    var rec = lst.FirstOrDefault(f => f.ITBID == id);
                    if (rec != null)
                    {
                        SessionHelper.GetPartyAcct(Session).UndoDelete(rec.ITBID);

                        var lst2 = GetSettlementRuleLines().ToList();
                        var html2 = PartialView("_ViewRule", lst2).RenderToString();
                        return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {

                }
                var lst3 = GetSettlementRuleLines().ToList();
                var html3 = PartialView("_ViewRule", lst3).RenderToString();
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

                        ViewBag.HeaderTitle = "Authorize Detail for Settlement Rule";
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
                            var rec = _repo.GetSettlementOption((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
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
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Settlement Rule Approval", null, fullName);
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

        private bool CloseMainRecord(int recordId, string batchId, string user_id)
        {
            var curDate = DateTime.Now;
            var dt = repoSetOptionTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                var dm = repoSetOption.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
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
                var dt = repoSetOptionTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
                if (dt != null)
                {
                    dt.STATUS = approve;
                    var obj = new SM_SETTLEMENTOPTION()
                    {
                        DESCRIPTION = dt.DESCRIPTION,
                        CHANNEL = dt.CHANNEL,
                        STATUS = active,
                        CREATEDATE = DateTime.Now,
                        USERID = dt.USERID,
                    };

                    repoSetOption.Insert(obj);
                    var ac = repoSetRuleTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper() && e.SETTLEMENTOPTION_ID == dt.ITBID).ToList();
                    if (ac.Count > 0)
                    {
                        foreach (var d in ac)
                        {
                            d.STATUS = approve;
                            var obj2 = new SM_SETTLEMENTRULE()
                            {
                                PARTYTYPE_CAP = d.PARTYTYPE_CAP, 
                                PARTYTYPE_CODE = d.PARTYTYPE_CODE,
                                PARTYTYPE_VALUE = d.PARTYTYPE_VALUE,
                                SETTLEMENTOPTION_ID = dt.ITBID,
                                STATUS = active,
                                CREATEDATE = curDate,
                                USERID = user_id,
                            };
                            obj.SM_SETTLEMENTRULE.Add(obj2);
                        }
                    }
                    return true;
                }
                return false;
            }

            private bool ModifyMainRecord(int recordId, string batchId, string user_id)
            {
                var curDate = DateTime.Now;
                var dt = repoSetOptionTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
                if (dt != null)
                {
                    var dm = repoSetOption.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
                    dt.STATUS = approve;

                    if (dm != null)
                    {
                        dm.DESCRIPTION = dt.DESCRIPTION;
                        dm.CHANNEL = dt.CHANNEL;
                        dm.LAST_MODIFIED_UID = dt.USERID;
                        dm.LAST_MODIFIED_DATE = curDate;
                        dm.LAST_MODIFIED_AUTHID = User.Identity.Name;
                        dm.STATUS = active;
                        var ac = repoSetRuleTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID == user_id).ToList();
                        if (ac.Count > 0)
                        {
                            foreach (var d in ac)
                            {
                                d.STATUS = approve;
                                if (d.EVENTTYPE == eventInsert || d.EVENTTYPE == eventEdit)
                                {
                                    if (d.RECORDID == 0)
                                    {
                                        var dm2 = repoSetRule.AllEager(e => e.PARTYTYPE_CODE == d.PARTYTYPE_CODE && e.SETTLEMENTOPTION_ID == dt.ITBID).FirstOrDefault();
                                        if (dm2 == null)
                                        {
                                            var obj2 = new SM_SETTLEMENTRULE()
                                            {
                                                PARTYTYPE_CAP = d.PARTYTYPE_CAP,
                                                PARTYTYPE_CODE = d.PARTYTYPE_CODE, //drpCalcBasis.SelectedValue,
                                                BATCHID = batchId,
                                                PARTYTYPE_VALUE = d.PARTYTYPE_VALUE,
                                                SETTLEMENTOPTION_ID = dm.ITBID,
                                                STATUS = active,
                                                CREATEDATE = DateTime.Now,
                                                USERID = user_id,
                                            };
                                            dm.SM_SETTLEMENTRULE.Add(obj2);
                                        }
                                        else
                                        {
                                            dm2.PARTYTYPE_VALUE = d.PARTYTYPE_VALUE;
                                            dm2.PARTYTYPE_CODE = d.PARTYTYPE_CODE.ToUpper();
                                            dm2.BATCHID = d.BATCHID;
                                            dm2.PARTYTYPE_CAP = d.PARTYTYPE_CAP;
                                        }
                                    }
                                    else
                                    {
                                        var dm2 = repoSetRule.AllEager(e => e.PARTYTYPE_CODE == d.PARTYTYPE_CODE && d.SETTLEMENTOPTION_ID == dt.ITBID).FirstOrDefault();
                                        if (dm2 != null)
                                        {
                                            dm2.PARTYTYPE_CAP = d.PARTYTYPE_CAP;
                                            //dm2.PARTYTYPE_CODE = d.PARTYTYPE_CODE.ToUpper();
                                            dm2.BATCHID = d.BATCHID;
                                            dm2.PARTYTYPE_VALUE = d.PARTYTYPE_VALUE;

                                        }
                                    }
                                }
                                else
                                {
                                    var dm2 = repoSetRule.Find(d.RECORDID);
                                    if (dm2 != null)
                                    {
                                        repoSetRule.Delete(dm2.ITBID);
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
                                var recc = repoSetOptionTemp.Find(recordId);
                                if (recc != null)
                                {
                                    recc.STATUS = reject;
                                    var gh = repoSetRuleTemp.AllEager(g => g.BATCHID == rec2.BATCHID && g.USERID == rec2.USERID).ToList();
                                    foreach (var g in gh)
                                    {
                                        g.STATUS = reject;
                                    }
                                }

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
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, reject, "Settlement Rule Rejection", Narration, fullName);
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
            public IList<SettlementRuleObj> GetSettlementRuleLines()
            {
                //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
                return SessionHelper.GetSettlementRule(Session).Lines;
            }
    }
}