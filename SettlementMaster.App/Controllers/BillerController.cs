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
    [Authorize]
    public class BillerController : Controller
    {
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
            private readonly IRepository<SM_AUTHLIST> repoAuth = null;

            //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
            private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
            private readonly IRepository<SM_BILLER> repoBiller = null;
            private readonly IRepository<SM_BILLERMSC> repoBillerMsc = null;
            private readonly IRepository<SM_BILLERMSCTEMP> repoBillerMscTemp = null;
            private readonly IRepository<SM_BILLERTEMP> repoBillerTemp = null;
            private readonly IRepository<SM_BL_FEE1SHARINGPARTY> repoBlFee1Sharing = null;
            private readonly IRepository<SM_BL_FEE1SHARINGPARTYTEMP> repoBlFee1SharingTemp = null;
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
            int menuId = 36;
            int institutionId;
            int roleId;
            int checkerNo = 1;
            string fullName;
            string deptCode;
            // GET: Roles
            public BillerController()
            {
                uow = new UnitOfWork();
                //repoScheme = new Repository<SM_CARDSCHEME>(uow);
                //repoCurrency = new Repository<SM_CURRENCY>(uow);
                repoAuth = new Repository<SM_AUTHLIST>(uow);
                repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
                repoBiller = new Repository<SM_BILLER>(uow);
                repoBillerMsc = new Repository<SM_BILLERMSC>(uow);
                repoBillerTemp = new Repository<SM_BILLERTEMP>(uow);
                repoBillerMscTemp = new Repository<SM_BILLERMSCTEMP>(uow);
                repoBlFee1Sharing = new Repository<SM_BL_FEE1SHARINGPARTY>(uow);
                repoBlFee1SharingTemp = new Repository<SM_BL_FEE1SHARINGPARTYTEMP>(uow);
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
            public ActionResult Index()
            {
                try
                {
                
                    //// GetMenuId();
                    //SessionHelper.GetPartyAcct(Session).Clear();
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
            public async Task<ActionResult> BillerList()
            {
                try
                {
                    var rec = await _repo.GetBillerAsync(0, true);  //repoSession.FindAsync(id);              
                    return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var obj1 = new { data = new List<BillerObj>(), RespCode = 2, RespMessage = ex.Message };
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
        public ActionResult Add(mBillerObj model, string mscUpdated)
        {
            var rv = new SharingPartySession();
            try
            {
                if (model.mObj.ITBID == 0)
                {
                    ViewBag.ButtonText = "Save";
                    ViewBag.HeaderTitle = "Add Biller";
                }
                else
                {
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Biller";
                }

                // ViewBag.MenuId = m;
                // menuId = SmartUtil.GetMenuId(m);
                string bid = SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.mObj.ITBID == 0)
                    {
                        var errMsg = "";
                        if (validateForm(model.mObj, eventInsert, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            var rec = rv.GetSharingParty(User.Identity.Name);
                            model.mFee1SharingObj = rec.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "fee1").ToList();
                            model.mMsc1SharingObj = rec.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "msc1").ToList();

                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_BILLERTEMP BType = new SM_BILLERTEMP()
                            {
                                // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                                BILLER_CODE = model.mObj.BILLER_CODE,
                                BILLER_DESC = model.mObj.BILLER_DESC,
                                BILLER_SHORTNAME = model.mObj.BILLER_SHORTNAME,
                                COUNTRY_CODE = model.mObj.COUNTRY_CODE,
                                CHANNEL = model.mObj.CHANNEL,
                                MERCHANTID = model.mObj.MERCHANTID,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.mObj.ITBID,
                            };
                            repoBillerTemp.Insert(BType);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                            if (rst)
                            {
                                if (mscUpdated == "Y")
                                {
                                    SaveBillerMscTemp(eventInsert, bid, model.mBillerMscObj, BType);
                                }
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
                                    
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Biller Record");

                                    //txscope.Complete();
                                    TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", "Biller");
                                }
                            }
                        }
                    //}
                    else
                    {
                        var errMsg = "";
                        if (validateForm(model.mObj, eventEdit, out errMsg))
                        {
                            ViewBag.Message = errMsg; // "Carscheme already Exist.";
                            BindCombo();
                            var rec2 = rv.GetSharingParty(User.Identity.Name);
                            model.mFee1SharingObj = rec2.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "fee1").ToList();
                            model.mMsc1SharingObj = rec2.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "msc1").ToList();

                            return View("Add", model);
                        };
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            SM_BILLERTEMP BType = new SM_BILLERTEMP()
                            {
                                BILLER_CODE = model.mObj.BILLER_CODE,
                                BILLER_DESC = model.mObj.BILLER_DESC,
                                BILLER_SHORTNAME = model.mObj.BILLER_SHORTNAME,
                                COUNTRY_CODE = model.mObj.COUNTRY_CODE,
                                CHANNEL = model.mObj.CHANNEL,
                                MERCHANTID = model.mObj.MERCHANTID,
                                STATUS = open,
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                USERID = User.Identity.Name,
                                RECORDID = model.mObj.ITBID,
                            };
                            repoBillerTemp.Insert(BType);
                            //  var rst1 = new AuthListUtil().SaveLog(auth);
                            var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                            if (rst)
                            {
                                if (mscUpdated == "Y")
                                {
                                    SaveBillerMscTemp(eventEdit, bid, model.mBillerMscObj, BType);
                                }
                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = model.mObj.STATUS == active ? eventEdit : model.mObj.STATUS,
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
                                    EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Biller Record");
                                    //txscope.Complete();
                                    TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                    //return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                    return RedirectToAction("Index", "Biller");
                                    //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });

                                    // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, userInstitutionItbid, "MCC Record");

                                }
                            }
                        }

                    //}
                    // If we got this far, something failed, redisplay form
                    //return Json(new { RespCode = 1, RespMessage = errorMsg });
                    BindCombo();
                    //ViewBag.PartyAcct = GetPartyAcctLines();
                    ViewBag.Message = errorMsg;
                    var rec3 = rv.GetSharingParty(User.Identity.Name);
                    model.mFee1SharingObj = rec3.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "fee1").ToList();
                    model.mMsc1SharingObj = rec3.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "msc1").ToList();

                    return View("Add", model);
                }
            }
            catch (SqlException ex)
            {
                BindCombo();
                //ViewBag.PartyAcct = GetPartyAcctLines();
                ViewBag.Message = ex.Message;
                var rec = rv.GetSharingParty(User.Identity.Name);
                model.mFee1SharingObj = rec.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "fee1").ToList();
                model.mMsc1SharingObj = rec.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "msc1").ToList();

                return View("Add", model);
                //return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                BindCombo();
                var rec4 = rv.GetSharingParty(User.Identity.Name);
                model.mFee1SharingObj = rec4.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "fee1").ToList();
                model.mMsc1SharingObj = rec4.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "msc1").ToList();

                ViewBag.Message = ex.Message;
                return View("Add", model);
                // return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            BindCombo();
            var rec5 = rv.GetSharingParty(User.Identity.Name);
            model.mFee1SharingObj = rec5.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "fee1").ToList();
            model.mMsc1SharingObj = rec5.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToLower() == "msc1").ToList();

            ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            return View("Add", model);
            //return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }

        private void SaveBillerMscTemp(string eventType,string batchId, BillerMscObj objMsc, SM_BILLERTEMP bType)
        {
            var rv = new SharingPartySession();
            var rec = rv.GetSharingParty(User.Identity.Name);
            if(rec != null && rec.Count > 0)
            {
                var dre = new SM_BILLERMSCTEMP()
                {
                    BATCHID = batchId,
                    BILLER_CODE = bType.BILLER_CODE,
                    CHANNEL = bType.CHANNEL,
                    CREATEDATETIME = bType.CREATEDATE,
                    FEE1 = rec.Where(D=> D.PARTY_LOCATOR == "FEE1").Sum(d => d.SHARINGVALUE),
                    FEE_CALCBASIS = objMsc.FEE_CALCBASIS,
                    USERID = User.Identity.Name,
                    STATUS = open,
                    RECORDID = objMsc.ITBID,
                    DOM_MSC1 = rec.Where(D => D.PARTY_LOCATOR == "MSC1").Sum(d => d.SHARINGVALUE),
                    DOM_MSC_CALCBASIS = objMsc.DOM_MSC_CALCBASIS
                };
                repoBillerMscTemp.Insert(dre);
                foreach(var d in rec)
                {
                    var obj = new SM_BL_FEE1SHARINGPARTYTEMP()
                    {
                        BILLERMSC_ITBID = objMsc.ITBID,
                        BILLER_CODE = bType.BILLER_CODE,
                        CREATEDATE = bType.CREATEDATE,
                        USERID = User.Identity.Name,
                        RECORDID = d.DB_ITBID,
                        STATUS = open,
                        PARTY_LOCATOR = d.PARTY_LOCATOR,
                        //EVENTTYPE = eventType,
                        SHARINGVALUE = d.SHARINGVALUE,
                        BATCHID = batchId,
                        PARTYTYPE_CODE = d.PARTYTYPE_CODE,
                        
                    };
                    repoBlFee1SharingTemp.Insert(obj);
                }
            }
        }

        private bool validateForm(BillerObj obj, string eventType, out string errorMsg)
        {
            var sb = new StringBuilder();
            var errCount = 0;
            if (eventType == eventInsert)
            {
                var existCbnCode = repoBiller.AllEager(f => f.BILLER_CODE != null && f.BILLER_CODE == obj.BILLER_CODE).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"""BILLER CODE"" already exist for another Party");
                    errCount++;
                }
                //var existShortName = repoBiller.AllEager(f => f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
                //if (existShortName > 0)
                //{
                //    sb.AppendLine(@"""PARTY SHORT NAME"" already exist for another Party");
                //    errCount++;
                //}

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
                var existCbnCode = repoBiller.AllEager(f => f.ITBID != obj.ITBID && f.BILLER_CODE != null && f.BILLER_CODE == obj.BILLER_CODE).Count();
                if (existCbnCode > 0)
                {
                    sb.AppendLine(@"""BILLER CODE"" already exist for another Party");
                    errCount++;
                }
                //var existShortName = repoBiller.AllEager(f => f.ITBID != obj.ITBID && f.PARTYTYPE_CODE != null && f.PARTYTYPE_CODE == obj.PARTYTYPE_CODE && f.PARTY_SHORTNAME != null && f.PARTY_SHORTNAME == obj.PARTY_SHORTNAME).Count();
                //if (existShortName > 0)
                //{
                //    sb.AppendLine(@"""PARTY SHORT NAME"" already exist for another Party");
                //    errCount++;
                //}

                if (errCount > 0)
                {
                    errorMsg = sb.ToString();
                    return true;
                }
                errorMsg = sb.ToString();
                return false;
            }
        }


        //void SaveAcctDetailTemp(string eventType, SM_PARTYTEMP rec)
        //{
        //    if (eventType == "New")
        //    {
        //        var col = GetPartyAcctLines();
        //        foreach (var d in col)
        //        {
        //            var obj = new SM_PARTYACCOUNTTEMP()
        //            {
        //                DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
        //                BATCHID = rec.BATCHID,
        //                DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
        //                DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
        //                DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDESS,
        //                DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
        //                DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
        //                DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
        //                PARTY_ITBID = rec.ITBID,
        //                STATUS = open,
        //                CREATEDATE = DateTime.Now,
        //                USERID = User.Identity.Name,
        //                EVENTTYPE = eventInsert,

        //            };
        //            repoBillerMscTemp.Insert(obj);
        //        }

        //    }
        //    else
        //    {
        //        var col = GetPartyAcctLines();
        //        foreach (var d in col)
        //        {
        //            //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
        //            //{
        //            //    continue;
        //            //}
        //            if (d.NewRecord || d.Updated || d.Deleted)
        //            {
        //                var obj = new SM_PARTYACCOUNTTEMP()
        //                {
        //                    DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
        //                    BATCHID = rec.BATCHID,
        //                    DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
        //                    DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
        //                    DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDESS,
        //                    DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
        //                    DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
        //                    DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
        //                    PARTY_ITBID = rec.ITBID,
        //                    STATUS = open,
        //                    RECORDID = d.NewRecord ? 0 : d.ITBID,
        //                    CREATEDATE = DateTime.Now,
        //                    USERID = User.Identity.Name,
        //                    EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,

        //                };

        //                repoBillerMscTemp.Insert(obj);
        //            }
        //        }
        //    }

        //}
        void BindCombo(string acq_selected = null)
        {
            var chan = _repo.GetChannel(0, true, "Active");
            var country = _repo.GetCountry(0, true, "Active");
            var mList = _repo.GetMerchantSearchDropDown("XP");

            ViewBag.ChannelList = new SelectList(chan, "CODE", "DESCRIPTION");
            ViewBag.CountryList = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
            ViewBag.MerchantList = new SelectList(mList, "MERCHANTID", "DESCRIPTION");

            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");
            ViewBag.CalcBasis = new SelectList(SmartObj.GetCalculationBasis(), "Code", "Description");
        }
        [MyAuthorize]
        public async Task<ActionResult> Add(int id = 0)
        {
            try
            {
                GetPriv();
                var rv = new SharingPartySession();
                rv.PurgeSharingParty(User.Identity.Name);
                var mod = new mBillerObj();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Biller";
                    ViewBag.ButtonText = "Save";
                    mod.mObj = new BillerObj();
                    mod.mBillerMscObj = new BillerMscObj();
                    BindCombo();
                    return View("Add", mod);
                }
                else
                {
                    ViewBag.HeaderTitle = "Edit Biller";
                    ViewBag.ButtonText = "Update";
                    var rec = await _repo.GetBillerAsync(id, false);
                    if (rec == null)
                    {
                        TempData["msg"] = "Record Not Found";
                        return View("Index");
                    }
                    mod.mObj = rec.FirstOrDefault();
                    var modChan = await _repo.GetBillerMscAsync(mod.mObj.BILLER_CODE,mod.mObj.CHANNEL.GetValueOrDefault());
                    mod.mBillerMscObj = modChan.FirstOrDefault();
                    if (mod.mBillerMscObj != null)
                    {
                        var modChanSharing = await _repo.GetBillerFee1SharingPartyAsync(mod.mBillerMscObj.BILLER_CODE, mod.mBillerMscObj.ITBID);
                        //mod.mFee1SharingObj = modChanSharing;
                       
                        foreach (var d in modChanSharing)
                        {
                            var obj = new SharingPartyObj()
                            {
                                PARTYITBID = null,
                                PARTY_LOCATOR = d.PARTY_LOCATOR,
                                PARTYTYPE_CODE = d.PARTYTYPE_CODE,
                                SHARINGVALUE = d.SHARINGVALUE,
                                CAP = d.CAP,
                                BILLER_CODE = d.BILLER_CODE,
                                MERCHANTMSC_ITBID = d.MERCHANTMSC_ITBID,
                                //SETTLEMENTCURRENCY = model.SETTLEMENTCURRENCY,
                                ACCOUNT_ID = d.ACCOUNT_ID,
                                CREATEDATE = DateTime.Now,
                                USERID = User.Identity.Name,
                                EVENTTYPE = eventInsert,
                                DB_ITBID = d.ITBID,
                                
                            };
                            var rst = rv.PostSharingParty(obj, 1);

                        }
                        var lst = rv.GetSharingParty(User.Identity.Name);
                        mod.mFee1SharingObj = lst.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToUpper() == "FEE1").ToList();
                        mod.mMsc1SharingObj = lst.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToUpper() == "MSC1").ToList();

                    }

                    var model = mod;
                    BindCombo();
                    return View("Add", model);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Problem with Request";
                return View("Index");
            }
        }
        #region Sharing Party
        void BindComboParty()
        {
            var partyTypeList = _repo.GetPartyType(0,true,"ACTIVE");
            ViewBag.PartyList = new SelectList(partyTypeList, "PARTYTYPE_CODE", "PARTYTYPE_DESC");
            //ViewBag.AccountList = new SelectList(new List<DropdownObj>(), "CODE", "DESCRIPTION");

        }
        public PartialViewResult AddParty(string opt)
        {
            try
            {
                BindComboParty();
                var label =  opt == "FEE1" ? "Add Fee Sharing Party" : "Add MSC Sharing Party";

                ViewBag.HeaderTitle = label;
                ViewBag.ButtonText = "Add";
                return PartialView("_AddSharingParty", new SharingPartyObj() { PARTY_LOCATOR = opt });
                // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                return null;
            }
        }
        public ActionResult EditParty(string id)
        {
            try
            {
                var rv = new SharingPartySession();
                var rec = rv.FindSharingParty(id, User.Identity.Name);
                if (rec != null)
                {

                    BindComboParty();
                    //BindComboPartyAcct(rec.PARTYITBID.GetValueOrDefault());
                    ViewBag.HeaderTitle =  rec.PARTY_LOCATOR == "FEE1" ? "Edit Fee Sharin Party" : "Edit MSC Sharing Party";

                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    return PartialView("_AddSharingParty", rec);
                }
            }
            catch
            {

            }
            return null;
        }
        public ActionResult DeleteParty(string id)
        {
            string viewToDisplay = "";
            try
            {
                var rv = new SharingPartySession();
                var rec = rv.FindSharingParty(id, User.Identity.Name);
                if (rec != null)
                {
                    viewToDisplay =  rec.PARTY_LOCATOR == "FEE1" ? "_ViewFEE1SharingParty" : "_ViewMSC1SharingParty";
                    rv.DeleteSharingParty(id, User.Identity.Name);

                    var lst = rv.GetSharingParty(User.Identity.Name).Where(g => g.PARTY_LOCATOR == rec.PARTY_LOCATOR).ToList();
                    var tot = lst.Sum(d => d.SHARINGVALUE).GetValueOrDefault().ToString("F");
                    var html2 = PartialView(viewToDisplay, lst).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_html = html2, data_fee = tot }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            //var lst3 = GetMerchantAcctLines();
            //var html = PartialView("_ViewParty", lst3).RenderToString();
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParty(SharingPartyObj model)
        {
            var rv = new SharingPartySession();
            string msg = "";
            string viewToDisplay = "";
            try
            {
                if (string.IsNullOrEmpty(model.PID) && model.DB_ITBID.GetValueOrDefault() <= 0)
                {
                    var obj = new SharingPartyObj()
                    {
                        PARTYITBID = null,
                        PARTY_LOCATOR = model.PARTY_LOCATOR,
                        PARTYTYPE_CODE = model.PARTYTYPE_CODE,
                        SHARINGVALUE = model.SHARINGVALUE,
                        CAP = model.CAP,
                        BILLER_CODE = model.BILLER_CODE,
                        MERCHANTMSC_ITBID = model.MERCHANTMSC_ITBID,
                        //SETTLEMENTCURRENCY = model.SETTLEMENTCURRENCY,
                        ACCOUNT_ID = model.ACCOUNT_ID,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                        PID = model.PID,
                        //CHANNELID = model.CHANNELID,
                    };
                    //SessionHelper.GetRvHead(Session).AddItem(obj);
                    var rst = rv.PostSharingParty(obj, 1);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    var w = rv.GetSharingParty(User.Identity.Name).Where(d=> d.PARTY_LOCATOR == model.PARTY_LOCATOR).ToList(); // GetRvHeadLines().ToList();
                    //var html = PartialView("_ViewSharingParty", w).RenderToString();
                    //msg = "Record Added to List";
                    //return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    var tot = w.Sum(f => f.SHARINGVALUE);
                    var tot_cap = w.Sum(f => f.CAP);
                    viewToDisplay = model.PARTY_LOCATOR == "FEE1" ? "_ViewFEE1SharingParty" : "_ViewMSC1SharingParty";
                    var html = PartialView(viewToDisplay, w).RenderToString();
                    //msg = "Record Added to List";
                    return Json(new { data_html = html, data_fee = tot.GetValueOrDefault().ToString("F"), data_cap = tot_cap.GetValueOrDefault().ToString("F"), RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var obj = new SharingPartyObj()
                    {
                        //AGENT_ITBID = model.AGENT_ITBID,
                        PARTYITBID = null,
                        PARTY_LOCATOR = model.PARTY_LOCATOR,
                        PARTYTYPE_CODE = model.PARTYTYPE_CODE,
                        SHARINGVALUE = model.SHARINGVALUE,
                        CAP = model.CAP,
                        BILLER_CODE = model.BILLER_CODE,
                        MERCHANTMSC_ITBID = model.MERCHANTMSC_ITBID,
                        ACCOUNT_ID = model.ACCOUNT_ID,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        DB_ITBID = model.DB_ITBID,
                        PID = model.PID,
                        EVENTTYPE = model.DB_ITBID > 0 ? eventEdit : eventInsert,

                    };
                    OutPutObj rst;

                    rst = rv.PostSharingParty(obj, 2);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    }
                    var w = rv.GetSharingParty(User.Identity.Name).Where(d=> d.PARTY_LOCATOR == model.PARTY_LOCATOR).ToList(); // GetRvHeadLines().ToList();
                    //var html = PartialView("_ViewMerchantAcct", w).RenderToString();
                    //msg = "Record Updated to List";
                    //return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    //var rec = GetMSC2DomLines().ToList();
                    var tot = w.Sum(f => f.SHARINGVALUE);
                    var tot_cap = w.Sum(f => f.CAP);
                    viewToDisplay = model.PARTY_LOCATOR == "FEE1" ? "_ViewFEE1SharingParty" : "_ViewMSC1SharingParty";

                    var html = PartialView(viewToDisplay, w).RenderToString();
                    //msg = "Record Added to List";
                    return Json(new { data_html = html, data_fee = tot.GetValueOrDefault().ToString("F"), data_cap = tot_cap.GetValueOrDefault().ToString("F"), RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

            }

        }
        #endregion Sharing Party
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
                    var mObj = new mBillerObj();
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
                        var rec = _repo.GetBiller((int)det.RECORDID, false, status: stat, isTemp: true);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            mObj.mObj = model;
                            // Get Biller Msc record
                            var recMsc = _repo.GetBillerMscTemp(det.BATCHID,det.USERID).FirstOrDefault();
                            if (recMsc != null)
                            {
                                mObj.mBillerMscObj = recMsc;
                                var recParty = _repo.GetBillerFee1SharingPartyTemp(det.BATCHID, det.USERID);
                                mObj.mFee1SharingObj = recParty.Where(d=> d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToUpper() == "FEE1").ToList();
                                mObj.mMsc1SharingObj = recParty.Where(d => d.PARTY_LOCATOR != null && d.PARTY_LOCATOR.ToUpper() == "MSC1").ToList();

                            }
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.CREATED_BY;
                            ViewBag.Auth = obj;
                            ViewBag.DisplayAuth = det.STATUS == open && !(model.USERID == User.Identity.Name);

                            BindCombo();
                            //ViewBag.BaseRole = new SelectList(GetRoleBase(), "Code", "Description");
                            // return null;

                            return View("DetailAuth", mObj);
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
                                        var t = uow.Save(User.Identity.Name);
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
                var dt = repoBillerTemp.AllEager(e => e.ITBID == recordId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
                if (dt != null)
                {
                    var dm = repoBiller.AllEager(e => e.ITBID == dt.RECORDID).FirstOrDefault();
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
            var dt = repoBillerTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                dt.STATUS = approve;
                var obj = new SM_BILLER()
                {
                    BILLER_CODE = dt.BILLER_CODE,
                    BILLER_DESC = dt.BILLER_DESC,
                    BILLER_SHORTNAME = dt.BILLER_SHORTNAME,
                    MERCHANTID = dt.MERCHANTID,
                    COUNTRY_CODE = dt.COUNTRY_CODE,
                    CHANNEL = dt.CHANNEL,
                    STATUS = active,
                    CREATEDATE = DateTime.Now,
                    USERID = dt.USERID,
                };

                repoBiller.Insert(obj);
                var d = repoBillerMscTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
                if (d!= null)
                {
                        d.STATUS = approve;
                        var ext = repoBillerMsc.AllEager(f => f.BILLER_CODE == d.BILLER_CODE).FirstOrDefault();
                    if (ext == null)
                    {
                        ext = new SM_BILLERMSC()
                        {
                            CBN_CODE = d.CBN_CODE, //drpCalcBasis.SelectedValue,
                            BATCHID = batchId,
                            CHANNEL = d.CHANNEL,
                            MERCHANTID = dt.MERCHANTID,
                            BILLER_CODE = dt.BILLER_CODE,
                            FEE1 = d.FEE1,
                            STATUS = active,
                            CREATEDATE = curDate,
                            USERID = user_id,
                            FEE_CALCBASIS = d.FEE_CALCBASIS,
                            DOM_MSC1 = d.DOM_MSC1,
                            DOM_MSC_CALCBASIS = d.DOM_MSC_CALCBASIS,
                        };
                        repoBillerMsc.Insert(ext);

                    }
                    else
                    {
                        ext.FEE1 = d.FEE1;
                        ext.FEE_CALCBASIS = d.FEE_CALCBASIS;
                        ext.DOM_MSC1 = d.DOM_MSC1;
                        ext.DOM_MSC_CALCBASIS = d.DOM_MSC_CALCBASIS;
                    }
                    
                    var spList = repoBlFee1SharingTemp.AllEager(f => f.BATCHID == batchId && f.USERID == user_id).ToList();
                    foreach(var f in spList)
                    {
                        var ext1 = repoBlFee1Sharing.AllEager(c => c.BILLER_CODE == f.BILLER_CODE && c.PARTYTYPE_CODE == f.PARTYTYPE_CODE && c.PARTY_LOCATOR == f.PARTY_LOCATOR).FirstOrDefault();
                        if (ext1 != null)
                        {
                            ext1.SHARINGVALUE = f.SHARINGVALUE;
                            ext1.PARTY_LOCATOR = f.PARTY_LOCATOR;
                        }
                        else
                        {
                            var ob = new SM_BL_FEE1SHARINGPARTY()
                            {
                                BILLER_CODE = f.BILLER_CODE,
                                SHARINGVALUE = f.SHARINGVALUE,
                                CREATEDATE = f.CREATEDATE,
                                STATUS = active,
                                PARTY_LOCATOR = f.PARTY_LOCATOR,
                                PARTYTYPE_CODE = f.PARTYTYPE_CODE,
                                USERID = f.USERID,
                            };
                            //repoBlFee1Sharing.Insert(ob);
                            ext.SM_BL_FEE1SHARINGPARTY.Add(ob);
                        }
                    }
                    var mainCol = repoBlFee1Sharing.AllEager(f => f.BILLER_CODE == d.BILLER_CODE).ToList();
                    var payDelete = mainCol.Where(f => !spList.Select(g=> g.PARTYTYPE_CODE).Contains(f.PARTYTYPE_CODE));
                    foreach (var g in payDelete)
                    {
                        repoBlFee1Sharing.Delete(g.ITBID);
                    }
                }
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(int recordId, string batchId, string user_id)
        {
            DateTime curDate = DateTime.Now;
            var dt = repoBillerTemp.AllEager(e => e.ITBID == recordId && e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
            if (dt != null)
            {
                dt.STATUS = approve;
                var oldRec = repoBiller.AllEager(f => f.ITBID == dt.RECORDID).FirstOrDefault();
                if (oldRec != null)
                {
                        oldRec.BILLER_CODE = dt.BILLER_CODE;
                        oldRec.BILLER_DESC = dt.BILLER_DESC;
                        oldRec.BILLER_SHORTNAME = dt.BILLER_SHORTNAME;
                        oldRec.MERCHANTID = dt.MERCHANTID;
                        oldRec.COUNTRY_CODE = dt.COUNTRY_CODE;
                        oldRec.CHANNEL = dt.CHANNEL;
                        oldRec.LAST_MODIFIED_UID = dt.USERID;
                   
                    var d = repoBillerMscTemp.AllEager(e => e.BATCHID == batchId && e.USERID != null && e.USERID.ToUpper() == user_id.ToUpper()).FirstOrDefault();
                    if (d != null)
                    {
                        d.STATUS = approve;
                        var ext = repoBillerMsc.AllEager(f => f.BILLER_CODE == d.BILLER_CODE).FirstOrDefault();
                        if (ext == null)
                        {
                            ext = new SM_BILLERMSC()
                            {
                                CBN_CODE = d.CBN_CODE, //drpCalcBasis.SelectedValue,
                                BATCHID = batchId,
                                CHANNEL = d.CHANNEL,
                                MERCHANTID = dt.MERCHANTID,
                                BILLER_CODE = dt.BILLER_CODE,
                                FEE1 = d.FEE1,
                                STATUS = active,
                                CREATEDATE = curDate,
                                USERID = user_id,
                                FEE_CALCBASIS = d.FEE_CALCBASIS,
                                DOM_MSC1 = d.DOM_MSC1,
                                DOM_MSC_CALCBASIS = d.DOM_MSC_CALCBASIS,
                            };
                            repoBillerMsc.Insert(ext);

                        }
                        else
                        {
                            ext.FEE1 = d.FEE1;
                            ext.FEE_CALCBASIS = d.FEE_CALCBASIS;
                            ext.DOM_MSC1 = d.DOM_MSC1;
                            ext.DOM_MSC_CALCBASIS = d.DOM_MSC_CALCBASIS;
                        }

                        var spList = repoBlFee1SharingTemp.AllEager(f => f.BATCHID == batchId && f.USERID == user_id).ToList();
                        foreach (var f in spList)
                        {
                            var ext1 = repoBlFee1Sharing.AllEager(c => c.BILLER_CODE == f.BILLER_CODE && c.PARTYTYPE_CODE == f.PARTYTYPE_CODE && c.PARTY_LOCATOR == f.PARTY_LOCATOR).FirstOrDefault();
                            if (ext1 != null)
                            {
                                ext1.SHARINGVALUE = f.SHARINGVALUE;
                                ext1.PARTY_LOCATOR = f.PARTY_LOCATOR;
                            }
                            else
                            {
                                var ob = new SM_BL_FEE1SHARINGPARTY()
                                {
                                    BILLER_CODE = f.BILLER_CODE,
                                    SHARINGVALUE = f.SHARINGVALUE,
                                    CREATEDATE = f.CREATEDATE,
                                    STATUS = active,
                                    PARTY_LOCATOR = f.PARTY_LOCATOR,
                                    PARTYTYPE_CODE = f.PARTYTYPE_CODE,
                                    USERID = f.USERID,
                                };
                                //repoBlFee1Sharing.Insert(ob);
                                ext.SM_BL_FEE1SHARINGPARTY.Add(ob);
                            }
                        }
                        var mainCol = repoBlFee1Sharing.AllEager(f => f.BILLER_CODE == d.BILLER_CODE).ToList();
                        var payDelete = mainCol.Where(f => !spList.Select(g => g.PARTYTYPE_CODE).Contains(f.PARTYTYPE_CODE));
                        foreach (var g in payDelete)
                        {
                            repoBlFee1Sharing.Delete(g.ITBID);
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
                var recP = repoBillerTemp.AllEager(d => d.ITBID == rECORDID && d.USERID == uSERID).FirstOrDefault();
                if (recP != null)
                {
                    recP.STATUS = reject;
                }

                var recPP = repoBillerMscTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
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