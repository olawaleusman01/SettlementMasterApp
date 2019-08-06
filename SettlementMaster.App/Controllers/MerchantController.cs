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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class MerchantController : Controller
    {
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
            private readonly IRepository<SM_AUTHLIST> repoAuth = null;

            //private readonly IRepository<SM_MERCHANTCONFIG> repoVal = null;
            private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
            private readonly IRepository<SM_INSTITUTION> repoInst = null;
        private readonly IRepository<SM_MERCHANTMSC> repoMerchantMsc = null;
        private readonly IRepository<SM_MERCHANTMSCTEMP> repoMerchantMscTemp = null;
        private readonly IRepository<SM_SHAREDMSC2DETAILTEMP> repoSharedMsc2Temp = null;
        private readonly IRepository<SM_SHAREDMSC2DETAIL> repoSharedMsc2 = null;

        
            private readonly IRepository<SM_MERCHANTACCT> repoMAcct = null;
            private readonly IRepository<SM_MERCHANTACCTTEMP> repoMAcctTemp = null;
            private readonly IRepository<SM_INSTITUTIONTEMP> repoInstTemp = null;
            private readonly IRepository<SM_CARDSCHEME> repoScheme = null;
            private readonly IRepository<SM_CURRENCY> repoCurrency = null;
            private readonly IRepository<SM_MERCHANTDETAIL> repoM = null;
            private readonly IRepository<SM_MERCHANTDETAILTEMP> repoMTemp = null;
            private readonly IRepository<SM_TERMINAL> repoT = null;
            private readonly IRepository<SM_TERMINALTEMP> repoTTemp = null;
            private readonly IRepository<SM_PARTY> repoParty = null;

        
            private readonly IRepository<SM_MERCHANTCONFIG> repoMerVal = null;

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
            int menuId = 17;
            int institutionId;
            int roleId;
            int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public MerchantController()
        {
            uow = new UnitOfWork();
            repoScheme = new Repository<SM_CARDSCHEME>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoMAcct = new Repository<SM_MERCHANTACCT>(uow);
            repoInstTemp = new Repository<SM_INSTITUTIONTEMP>(uow);
            repoMAcctTemp = new Repository<SM_MERCHANTACCTTEMP>(uow);
            repoMerchantMsc = new Repository<SM_MERCHANTMSC>(uow);
            repoMerchantMscTemp = new Repository<SM_MERCHANTMSCTEMP>(uow);
            repoSharedMsc2 = new Repository<SM_SHAREDMSC2DETAIL>(uow);
            repoSharedMsc2Temp = new Repository<SM_SHAREDMSC2DETAILTEMP>(uow);
            repoCurrency = new Repository<SM_CURRENCY>(uow);
            repoM = new Repository<SM_MERCHANTDETAIL>(uow);
            repoMTemp = new Repository<SM_MERCHANTDETAILTEMP>(uow);
            repoT = new Repository<SM_TERMINAL>(uow);
            repoTTemp = new Repository<SM_TERMINALTEMP>(uow);
            repoMerVal = new Repository<SM_MERCHANTCONFIG>(uow);
            repoParty = new Repository<SM_PARTY>(uow);

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
                SessionHelper.GetMerchantAcct(Session).Clear();
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

            public async Task<ActionResult> MerchantList(string q,string option,string isdefault)
            {
                try
                {
                    var rec = await _repo.GetMerchantByMidNameAsync(q, option,isdefault);  //repoSession.FindAsync(id);              
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

        #region Merchant Account
        public PartialViewResult AddAcct(decimal id = 0, string m = null)
        {
            try
            {
                BindComboAcct();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Account";
                    ViewBag.ButtonText = "Add";
                    return PartialView("_AddAcct", new MerchantAcctObj());
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
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddMerchantAcct(int pt, string MID)
        {
            try
            {
                // menuId = SmartUtil.GetMenuId(m);
                if (pt == 2)
                {
                    //its an update
                    string bid = string.Concat("MMA_", SmartObj.GenRefNo2());

                    SaveAcctDetailTemp(eventEdit, MID, bid);
                    //  var rst1 = new AuthListUtil().SaveLog(auth);
                    var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                    if (rst)
                    {
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = eventEdit,
                            MENUID = menuId,
                            //MENUNAME = "",
                            // RECORDID =  setId,
                            STATUS = open,
                            // TABLENAME = "ADMIN_DEPARTMENT",
                            URL = Request.FilePath,
                            USERID = User.Identity.Name,
                            BATCHID = bid,
                            POSTTYPE = Single,
                            INSTITUTION_ITBID = institutionId,
                            // RECORDID = decimal.Parse(hidItbid.Value)
                        };

                        repoAuth.Insert(auth);
                        var rst1 = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst1)
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Merchant Account Record");

                            //pnlResponseMsg.Text = "<i class='fa fa-info' ></i> Record Updated SuccessFully...Awaiting Authorization";
                            var msg = "Record Updated SuccessFully...Awaiting Authorization";
                            return Json(new { RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }

                    }

                }
                else
                {

                    // its an insert
                    //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    //{

                        //SaveMSC(eventInsert);
                        string bid = string.Concat("MSC", "_", SmartObj.GenRefNo2());
                        //  SaveMCCRuleTemp(eventInsert, bid);
                        SaveAcctDetailTemp(eventInsert, MID, bid);
                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst)
                        {

                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                // RECORDID = setId,
                                STATUS = open,
                                // TABLENAME = "ManageMCC",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                BATCHID = bid,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single,

                            };
                            repoAuth.Insert(auth);
                            var rst1 = uow.Save(User.Identity.Name);
                            
                            //txscope.Complete();
                            sucTrue = true;
                        }
                        else
                        {
                            //pnlForm.Visible = false;
                            //pnlDetails.Visible = true;

                        }
                        if (sucTrue)
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Merchant Account Record");

                            var msg = "Record Created SuccessFully...Awaiting Authorization";
                            return Json(new { RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }
                    }
               // }
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 2, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);

            }

            //var html = PartialView("_ViewMerchantMsc", rec).RenderToString();
            //msg = "Record Added to List";
            return Json(new { RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult EditAcct(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetMerchantAcctLines().ToList();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    BindComboAcct();
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

                var lst = GetMerchantAcctLines();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    if (rec.NewRecord)
                    {
                        SessionHelper.GetMerchantAcct(Session).RemoveLine(rec.ITBID);

                    }
                    else
                    {
                        var resp = _repo.ValidateIfCanDeleteMAcct(rec.ITBID);
                        if (resp != null && resp.RespCode == 0)
                        {
                            SessionHelper.GetMerchantAcct(Session).MarkForDelete(rec.ITBID);
                        }
                        else if (resp != null)
                        {
                            var lst4 = GetMerchantAcctLines();
                            var html3 = PartialView("_ViewMerchantAcct", lst4).RenderToString();
                            return Json(new { RespCode = 1, RespMessage = resp.RespMessage, data_html = html3 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var lst2 = GetMerchantAcctLines();
                    var html2 = PartialView("_ViewMerchantAcct", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetMerchantAcctLines().ToList();
            var html = PartialView("_ViewMerchantAcct", lst3).RenderToString();
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UndoAcct(decimal id, string m)
        {
            try
            {
                ViewBag.MenuId = m;

                var lst = GetMerchantAcctLines();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    SessionHelper.GetMerchantAcct(Session).UndoDelete(rec.ITBID);

                    var lst2 = GetMerchantAcctLines();
                    var html2 = PartialView("_ViewMerchantAcct", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetMerchantAcctLines();
            var html3 = PartialView("_ViewMerchantAcct", lst3).RenderToString();
            return Json(new { RespCode = 0, RespMessage = "", data_html = html3 }, JsonRequestBehavior.AllowGet);
        }
        public IList<MerchantAcctObj> GetMerchantAcctLines()
        {
            //HttpSessionStateBase sec = new HttpSessionStateWrapper(Page.)
            return SessionHelper.GetMerchantAcct(Session).Lines;
        }
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddMerchantDetail(MerchantObj model)
        {
            try
            {
                //if (model.ITBID == 0)
                //{
                //    ViewBag.ButtonText = "Save";
                //    ViewBag.HeaderTitle = "Add Institution";
                //}
                //else
                //{
                //    ViewBag.ButtonText = "Update";
                //    ViewBag.HeaderTitle = "Edit Institution";
                //}

                //ViewBag.MenuId = m;
                //menuId = SmartUtil.GetMenuId(m);
                string bid = "MMD_" + SmartObj.GenRefNo2();
                var errorMsg = "";
                if (ModelState.IsValid)
                {
                    if (model.ITBID == 0)
                    {
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        SM_MERCHANTDETAILTEMP BType = new SM_MERCHANTDETAILTEMP()
                        {
                            // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                            ADDRESS = model.ADDRESS,
                            CITY_NAME = model.CITY_NAME,
                            CONTACTNAME = model.CONTACTNAME,
                            EMAIL = model.EMAIL,
                            CONTACTTITLE = model.CONTACTTITLE,
                            COUNTRY_CODE = model.COUNTRY_CODE,
                            BUSINESS_CODE = model.BUSINESS_CODE,
                            OLD_MID = model.OLD_MID,
                            INSTITUTION_CBNCODE = model.INSTITUTION_CBNCODE,
                            MCC_CODE = model.MCC_CODE,
                            MERCHANTID = model.MERCHANTID,
                            MERCHANTNAME = model.MERCHANTNAME,
                            PHONENO = model.PHONENO,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            BATCHID = bid,
                            USERID = User.Identity.Name,
                            RECORDID = model.ITBID,
                            STATE_CODE = model.STATE_CODE,

                        };

                        repoMTemp.Insert(BType);
                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst)
                        {

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
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Merchant Detail Record");

                                //txscope.Complete();
                                //TempData["msg"] = "Record Created SuccessFully...Authorization Pending.";
                                return Json(new { RespCode = 0, RespMessage = "Record Created SuccessFully...Authorization Pending." });
                                //return RedirectToAction("Index", "Institution", new { m = m });
                            }
                        }
                        //}
                    }
                    else
                    {
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                        SM_MERCHANTDETAILTEMP BType = new SM_MERCHANTDETAILTEMP()
                        {
                            // BType.PARTY_CODE = int.Parse(txtpartyCode.Value);
                            ADDRESS = model.ADDRESS,
                            CITY_NAME = model.CITY_NAME,
                            CONTACTNAME = model.CONTACTNAME,
                            EMAIL = model.EMAIL,
                            CONTACTTITLE = model.CONTACTTITLE,
                            COUNTRY_CODE = model.COUNTRY_CODE,
                            BUSINESS_CODE = model.BUSINESS_CODE,
                            OLD_MID = model.OLD_MID,
                            INSTITUTION_CBNCODE = model.INSTITUTION_CBNCODE,
                            MCC_CODE = model.MCC_CODE,
                            MERCHANTID = model.MERCHANTID,
                            MERCHANTNAME = model.MERCHANTNAME,
                            PHONENO = model.PHONENO,
                            CREATEDATE = DateTime.Now,
                            BATCHID = bid,
                            USERID = User.Identity.Name,
                            RECORDID = model.ITBID,
                            STATUS = open,
                            STATE_CODE = model.STATE_CODE,

                        };
                        repoMTemp.Insert(BType);
                        //  var rst1 = new AuthListUtil().SaveLog(auth);
                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;

                        if (rst)
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
                                POSTTYPE = Single,
                                INSTITUTION_ITBID = institutionId,
                                BATCHID = bid,

                            };
                            repoAuth.Insert(auth);
                            if (uow.Save(User.Identity.Name) > 0)
                            {
                                EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "Merchant Detail Record");

                                //txscope.Complete();
                                //TempData["msg"] = "Record Updated SuccessFully...Authorization Pending.";
                                return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });
                                //return RedirectToAction("Index", "Institution", new { m = m });
                                //return Json(new { RespCode = 0, RespMessage = "Record Updated SuccessFully...Authorization Pending." });


                            }
                        }
                        //}

                    }
                    // If we got this far, something failed, redisplay form
                    //return Json(new { RespCode = 1, RespMessage = errorMsg });
                    //BindCombo();
                    // ViewBag.InstitutionAcct = GetBanksLines();
                    ViewBag.Message = errorMsg;
                    //return View("Add", model);
                    return Json(new { RespCode = 1, RespMessage = errorMsg });

                }
            }
            catch (SqlException ex)
            {
                //BindCombo();
                // ViewBag.InstitutionAcct = GetBanksLines();
                ViewBag.Message = ex.Message;
                //return View("Add", model);
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                //BindCombo();
                //ViewBag.InstitutionAcct = GetBanksLines();
                ViewBag.Message = ex.Message;
                //return View("Add", model);
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            //BindCombo();
            //ViewBag.InstitutionAcct = GetBanksLines();
            ViewBag.Message = "Problem Processing Request, Try again or Contact Administrator.";
            //return View("Add", model);
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request, Try again or Contact Administrator." });

        }

        void SaveAcctDetailTemp(string eventType, string mid,string bid)
            {
                if (eventType == "New")
                {
                    var col = GetMerchantAcctLines();
                    foreach (var d in col)
                    {
                        var obj = new SM_MERCHANTACCTTEMP()
                        {
                            DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                            BATCHID = bid,
                            DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                            DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDRESS,
                            DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                            DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                            DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                            MERCHANTID = mid,
                            SETTLEMENTCURRENCY = d.SETTLEMENTCURRENCY,
                            STATUS = open,
                            CREATEDATE = DateTime.Now,
                            USERID = User.Identity.Name,
                            EVENTTYPE = eventInsert,
                            DRACCOUNTNAME = d.DRACCOUNTNAME,
                            DRACCOUNTNO = d.DRACCOUNTNO,
                            DRBANKCODE = d.DRBANKCODE
                           // RECORDID = d.RECORDID,
                        };
                        repoMAcctTemp.Insert(obj);
                    }

                }
                else
                {
                    var col = GetMerchantAcctLines();
                    foreach (var d in col)
                    {
                        //if (drpAcquirer.SelectedValue != "501" && d.ACQFLAG != 1)
                        //{
                        //    continue;
                        //}
                        if (d.NewRecord || d.Updated || d.Deleted)
                        {
                            var obj = new SM_MERCHANTACCTTEMP()
                            {
                                DEFAULT_ACCOUNT = d.DEFAULT_ACCOUNT, //drpCalcBasis.SelectedValue,
                                BATCHID = bid,
                                DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                                DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                                DEPOSIT_BANKADDESS = d.DEPOSIT_BANKADDRESS,
                                DEPOSIT_BANKCODE = d.DEPOSIT_BANKCODE,
                                DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                                DEPOSIT_COUNTRYCODE = d.DEPOSIT_COUNTRYCODE,
                                SETTLEMENTCURRENCY = d.SETTLEMENTCURRENCY,
                                MERCHANTID = mid,
                                STATUS = open,
                                RECORDID = d.NewRecord ? 0 : d.ITBID,
                                CREATEDATE = DateTime.Now,
                                USERID = User.Identity.Name,
                                EVENTTYPE = d.NewRecord ? eventInsert : d.Deleted ? eventDelete : eventEdit,
                                DRACCOUNTNAME = d.DRACCOUNTNAME,
                                DRACCOUNTNO = d.DRACCOUNTNO,
                                DRBANKCODE = d.DRBANKCODE

                            };
                            repoMAcctTemp.Insert(obj);
                        }
                    }
                }

            }
        void BindComboAcct()
        {
            var bankList = _repo.GetInstitution(0, true, "Active").Where(f => f.IS_BANK == "Y").ToList();
            var country = _repo.GetCurrency(0, true, "Active");

            ViewBag.BankList = new SelectList(bankList, "CBN_CODE", "INSTITUTION_NAME");
            ViewBag.CurrencyList = new SelectList(country, "CURRENCY_CODE", "CURRENCY_NAME");
            //var schemeList = _repo.GetCardScheme(0, true, "Active");
            //var country = _repo.GetCountry(0, true, "Active");

            //ViewBag.CardScheme = new SelectList(schemeList, "CARDSCHEME", "CARDSCHEME_DESC");
            //var call = SmartObj.GetCalculationBasis();
            //ViewBag.CalcBasis = new SelectList(call, "Code", "Description");

        }
        #endregion
        #region Merchant Terminal
        void BindComboTerm(string mid)
        {
            var bankList = _repo.GetMerchantAcct(mid);
            var freq = _repo.GetFrequency(0, true, "Active");
            var currency = _repo.GetCurrency(0, true, "Active");

            ViewBag.AccountList = new SelectList(bankList, "ITBID", "DEPOSIT_ACCOUNTNO");

            ViewBag.CurrencyList = new SelectList(currency, "CURRENCY_CODE", "CURRENCY_NAME");
            ViewBag.FrequencyList = new SelectList(freq, "ITBID", "FREQUENCY_DESC");
            var ptsa = repoParty.AllEager(d => d.PARTYTYPE_CODE == "PTSA").ToList();
            ViewBag.PTSAList = new SelectList(ptsa, "PARTY_SHORTNAME", "PARTY_DESC");
            var ptsp = repoParty.AllEager(d => d.PARTYTYPE_CODE == "PTSP").ToList();
            ViewBag.PTSPList = new SelectList(ptsp, "PARTY_SHORTNAME", "PARTY_DESC");

        }
        public PartialViewResult AddTerminal(string id)
        {
            try
            {
                BindComboTerm(id);

                ViewBag.HeaderTitle = "Add Terminal";
                ViewBag.ButtonText = "Add";
                var obj = new TerminalObj();
                id = null;
                return PartialView("_AddTerminal", obj);

            }
            catch (Exception ex)
            {
              
                return null;
            }
        }
        List<string> validationErrorMessage = new List<string>();

        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddTerminal(TerminalObj model)
        {
            var rv = new TidSession();
            string msg = "";
            try
            {
                GetPriv();
                model.PTSA = model.PTSA != null ? model.PTSA.Trim() : model.PTSA;
                model.PTSP = model.PTSP != null ? model.PTSP.Trim() : model.PTSP;
                model.TERMINALOWNER_CODE = model.TERMINALOWNER_CODE != null ? model.TERMINALOWNER_CODE.Trim() : model.TERMINALOWNER_CODE;
                model.VERVACQUIRERIDNO = model.VERVACQUIRERIDNO != null ? model.VERVACQUIRERIDNO.Trim() : model.VERVACQUIRERIDNO;
                model.MASTACQUIRERIDNO = model.MASTACQUIRERIDNO != null ? model.MASTACQUIRERIDNO.Trim() : model.MASTACQUIRERIDNO;
                model.VISAACQUIRERIDNO = model.VISAACQUIRERIDNO != null ? model.VISAACQUIRERIDNO.Trim() : model.VISAACQUIRERIDNO;
                model.TERMINALMODEL_CODE = model.TERMINALMODEL_CODE != null ? model.TERMINALMODEL_CODE.Trim() : model.TERMINALMODEL_CODE;

                if (ValidateForm(model))
                {
                    msg = GetStringFromList(validationErrorMessage);
                    return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.PID) && model.DB_ITBID <= 0)
                {
                    var obj = new TerminalObj()
                    {
                        ACCOUNT_ID = model.ACCOUNT_ID,
                        TERMINALID = model.TERMINALID,
                        MERCHANTID = model.MERCHANTID,
                        PTSP = model.PTSP,
                        PTSA = model.PTSA,
                        SETTLEMENT_CURRENCY = model.SETTLEMENT_CURRENCY,
                        TRANSACTION_CURRENCY = model.TRANSACTION_CURRENCY,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                        PID = model.PID,
                        SETTLEMENT_FREQUENCY = model.SETTLEMENT_FREQUENCY,
                        EmailAlert = model.EmailAlert,
                        TERMINALMODEL_CODE = model.TERMINALMODEL_CODE,
                        TERMINALOWNER_CODE = model.TERMINALOWNER_CODE,
                        VERVACQUIRERIDNO = model.VERVACQUIRERIDNO,
                        MASTACQUIRERIDNO = model.MASTACQUIRERIDNO,
                        VISAACQUIRERIDNO = model.VISAACQUIRERIDNO,
                        SLIP_FOOTER = model.SLIP_FOOTER,
                        SLIP_HEADER = model.SLIP_HEADER,
                        
                    };
                    //SessionHelper.GetRvHead(Session).AddItem(obj);
                    var rst = rv.PostTerminal(obj, 1);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    var w = rv.GetTerminal(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewTerminalQueue", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var obj = new TerminalObj()
                    {
                        ACCOUNT_ID = model.ACCOUNT_ID,
                        TERMINALID = model.TERMINALID,
                        MERCHANTID = model.MERCHANTID,
                        PTSP = model.PTSP,
                        PTSA = model.PTSA,
                        SETTLEMENT_CURRENCY = model.SETTLEMENT_CURRENCY,
                        TRANSACTION_CURRENCY = model.TRANSACTION_CURRENCY,
                        USERID = User.Identity.Name,
                        DB_ITBID = model.DB_ITBID,
                        PID = model.PID,
                        EVENTTYPE = model.DB_ITBID > 0 ? eventEdit : eventInsert,
                        SETTLEMENT_FREQUENCY = model.SETTLEMENT_FREQUENCY,
                        EmailAlert = model.EmailAlert,
                        TERMINALMODEL_CODE = model.TERMINALMODEL_CODE,
                        TERMINALOWNER_CODE = model.TERMINALOWNER_CODE,
                        VERVACQUIRERIDNO = model.VERVACQUIRERIDNO,
                        MASTACQUIRERIDNO = model.MASTACQUIRERIDNO,
                        VISAACQUIRERIDNO = model.VISAACQUIRERIDNO,
                        SLIP_FOOTER = model.SLIP_FOOTER,
                        SLIP_HEADER = model.SLIP_HEADER,
                    };
                    OutPutObj rst;
                    
                    rst = rv.PostTerminal(obj, 2);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    }
                    var w = rv.GetTerminal(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewTerminalQueue", w).RenderToString();
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
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditMerchantTerminal()
        {
            try
            {
                var rv = new TidSession();
                string bid = string.Concat("MT_", SmartObj.GenRefNo2());

                SaveTerminalTemp(eventEdit, bid, rv);
                //  var rst1 = new AuthListUtil().SaveLog(auth);
                var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                if (rst)
                {
                    SM_AUTHLIST auth = new SM_AUTHLIST()
                    {
                        CREATEDATE = DateTime.Now,
                        EVENTTYPE = eventEdit,
                        MENUID = menuId,
                        //MENUNAME = "",
                        // RECORDID =  setId,
                        STATUS = open,
                        // TABLENAME = "ADMIN_DEPARTMENT",
                        URL = Request.FilePath,
                        USERID = User.Identity.Name,
                        BATCHID = bid,
                        POSTTYPE = Single,
                        INSTITUTION_ITBID = institutionId,
                        // RECORDID = decimal.Parse(hidItbid.Value)
                    };

                    repoAuth.Insert(auth);
                    var rst1 = uow.Save(User.Identity.Name) > 0 ? true : false;
                    if (rst1)
                    {
                        var msg = "Record Updated SuccessFully...Awaiting Authorization";
                        var rec = rv.PurgeTerminal(User.Identity.Name);
                        var rec2 = rv.GetTerminal(User.Identity.Name);
                        var html = PartialView("_ViewTerminalQueue", rec2).RenderToString();
                        return Json(new {data_html = html,RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 2, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        }
        void SaveTerminalTemp(string eventType, string bid,TidSession rv)
        {
            
            if (eventType == "New")
            {
                var col = rv.GetTerminal(User.Identity.Name); // GetRvHeadLines();
                foreach (var d in col)
                {
                    var obj = new SM_TERMINALTEMP()
                    {
                        EMAIL_ALERTS = d.EMAIL_ALERTS, //drpCalcBasis.SelectedValue,
                        BATCHID = bid,
                        MASTACQUIRERIDNO = d.MASTACQUIRERIDNO,
                        ACCOUNT_ID = d.ACCOUNT_ID,
                        PTSA = d.PTSA,
                        MERCHANTID = d.MERCHANTID,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                        SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY,
                        PTSP = d.PTSP,
                        SETTLEMENT_CURRENCY = d.SETTLEMENT_CURRENCY,
                        SLIP_FOOTER = d.SLIP_FOOTER,
                        SLIP_HEADER = d.SLIP_HEADER,
                        TERMINALID = d.TERMINALID,
                        TERMINALMODEL_CODE = d.TERMINALMODEL_CODE,
                        TERMINALOWNER_CODE = d.TERMINALOWNER_CODE,
                        TRANSACTION_CURRENCY = d.TRANSACTION_CURRENCY,
                        VERVACQUIRERIDNO = d.VERVACQUIRERIDNO,
                        VISAACQUIRERIDNO = d.VISAACQUIRERIDNO,
                        
                    };
                    repoTTemp.Insert(obj);
                }

            }
            else
            {
                var col = rv.GetTerminal(User.Identity.Name); // GetRvHeadLines();
                foreach (var d in col)
                {

                    var obj = new SM_TERMINALTEMP()
                    {
                        EMAIL_ALERTS = d.EMAIL_ALERTS, //drpCalcBasis.SelectedValue,
                        BATCHID = bid,
                        MASTACQUIRERIDNO = d.MASTACQUIRERIDNO,
                        ACCOUNT_ID = d.ACCOUNT_ID,
                        PTSA = d.PTSA,
                        MERCHANTID = d.MERCHANTID,
                        STATUS = open,
                        CREATEDATE = DateTime.Now,
                        USERID = User.Identity.Name,
                        SETTLEMENT_FREQUENCY = d.SETTLEMENT_FREQUENCY,
                        PTSP = d.PTSP,
                        SETTLEMENT_CURRENCY = d.SETTLEMENT_CURRENCY,
                        SLIP_FOOTER = d.SLIP_FOOTER,
                        SLIP_HEADER = d.SLIP_HEADER,
                        TERMINALID = d.TERMINALID,
                        TERMINALMODEL_CODE = d.TERMINALMODEL_CODE,
                        TERMINALOWNER_CODE = d.TERMINALOWNER_CODE,
                        TRANSACTION_CURRENCY = d.TRANSACTION_CURRENCY,
                        VERVACQUIRERIDNO = d.VERVACQUIRERIDNO,
                        VISAACQUIRERIDNO = d.VISAACQUIRERIDNO,
                        EVENTTYPE = d.EVENTTYPE, // ? eventInsert : d.Deleted ? eventDelete : eventEdit,

                    };
                    repoTTemp.Insert(obj);
                }
            }
        }
        bool ValidateForm(TerminalObj obj)
        {
            int errorCount = 0;

            string termOwner = obj.TERMINALOWNER_CODE; // txttermownerCode.Value.Trim();
            if (!string.IsNullOrEmpty(termOwner))
            {
                int termOwnCode;
                if (int.TryParse(termOwner, out termOwnCode))
                {
                    var termOwnerCodeCount = repoParty.AllEager(d => d.PARTY_CODE == termOwner && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                    if (termOwnerCodeCount == 0)
                    {
                        // 
                        termOwnerCodeCount = repoInst.AllEager(d => d.CBN_CODE == termOwner).ToList().Count;
                    }
                    if (termOwnerCodeCount <= 0)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""TERMINALOWNERCODE"" does not exist");
                    }
                }
                else
                {
                    errorCount++;
                    // totalErrorCount++;
                    validationErrorMessage.Add(@"""TERMINALOWNERCODE"" must be a number");
                }
            }
            string ptsp = obj.PTSP; // != null ? obj.PTSP.Trim(): ""; // txttermownerCode.Value.Trim();
            if (!string.IsNullOrEmpty(ptsp))
            {
                    var ptspCount = repoParty.AllEager(d => d.PARTYTYPE_CODE == "PTSP" && d.PARTY_SHORTNAME == ptsp && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                    //if (ptspCount == 0)
                    //{
                    //    termOwnerCodeCount = repoInst.AllEager(d => d.CBN_CODE == termOwner).ToList().Count;
                    //}
                    if (ptspCount <= 0)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""PTSP"" does not exist");
                    }
            }
            else
            {
                errorCount++;
                validationErrorMessage.Add(@"""PTSP"" does not exist");
            }
            string ptsa = obj.PTSA; // txttermownerCode.Value.Trim();
            if (!string.IsNullOrEmpty(ptsa))
            {
                var ptsaCount = repoParty.AllEager(d => d.PARTYTYPE_CODE == "PTSA" && d.PARTY_SHORTNAME == ptsa && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                //if (ptspCount == 0)
                //{
                //    termOwnerCodeCount = repoInst.AllEager(d => d.CBN_CODE == termOwner).ToList().Count;
                //}
                if (ptsaCount <= 0)
                {
                    errorCount++;
                    //  totalErrorCount++;
                    validationErrorMessage.Add(@"""PTSA"" does not exist");
                }
            }
            else
            {
                errorCount++;
                validationErrorMessage.Add(@"""PTSA"" does not exist");
            }
            var countryCode = "";
            int binNoLength = 0;
            string binNoType = "";
            bool binNoReq = false;

            var dd = repoM.AllEager(g => g.MERCHANTID == obj.MERCHANTID).FirstOrDefault();
            if (dd != null)
            {
                countryCode = dd.COUNTRY_CODE;
            }
            var fieldValList = repoMerVal.AllEager(g => g.STATUS != null && g.STATUS.ToLower() == active.ToLower()).ToList();
            if (fieldValList.Count != 0)
            {
                //index 1
                if (countryCode == "NGN")
                {
                    binNoLength = fieldValList[4].FIELDLENGTH ?? 0;
                    binNoType = fieldValList[4].FIELDDATATYPE;
                    binNoReq = fieldValList[4].DOM_REQUIRED == "Y" ? true : false;
                }
                else
                {
                    binNoLength = fieldValList[4].INT_FIELDLENGTH ?? 0;
                    binNoType = fieldValList[4].INT_FIELDDATATYPE;
                    binNoReq = fieldValList[4].INT_REQUIRED == "Y" ? true : false;
                }
            }
            var verv = obj.VERVACQUIRERIDNO != null ? obj.VERVACQUIRERIDNO.Trim() : "";
            var mast = obj.MASTACQUIRERIDNO != null ? obj.MASTACQUIRERIDNO.Trim() : ""; 
            var visa = obj.VISAACQUIRERIDNO != null ? obj.VISAACQUIRERIDNO.Trim() : ""; 
            decimal mid;
            if (binNoReq)
            {
                if (binNoType == "STRING")
                {
                    if (!string.IsNullOrEmpty(verv))
                    {
                        if (countryCode == "NGN")
                        {

                            if (Regex.IsMatch(verv, "[^a-z0-9]", RegexOptions.IgnoreCase))
                            {

                                errorCount++;
                                //  totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""VERVEACQUIRERIDNUMBER"""));
                            }
                        }
                    }
                    if (Regex.IsMatch(visa, "[^a-z0-9]", RegexOptions.IgnoreCase))
                    {
                        errorCount++;
                        //totalErrorCount++;
                        validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""VISAACQUIRERIDNUMBER"""));
                    }


                    if (Regex.IsMatch(mast, "[^a-z0-9]", RegexOptions.IgnoreCase))
                    {

                        errorCount++;
                        // totalErrorCount++;

                        validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""MASTERCARDACQUIRERIDNUMBER"""));
                    }
                }
                else
                {
                    if (countryCode == "NGN")
                    {
                        if (!decimal.TryParse(verv, out mid))
                        {
                            errorCount++;
                            //  totalErrorCount++;

                            validationErrorMessage.Add(string.Format(@"""VERVEACQUIRERIDNUMBER"" must be a number"));
                        }
                    }
                    if (!decimal.TryParse(visa, out mid))
                    {
                        errorCount++;
                        //  totalErrorCount++;

                        validationErrorMessage.Add(string.Format(@"""VISAACQUIRERIDNUMBER"" must be a number"));
                    }
                    if (!decimal.TryParse(mast, out mid))
                    {
                        errorCount++;
                        //   totalErrorCount++;

                        validationErrorMessage.Add(string.Format(@"""MASTERCARDACQUIRERIDNUMBER"" must be a number"));
                    }
                }
                if (countryCode == "NGN")
                {
                    if (binNoLength != 0 && verv.Length != binNoLength)
                    {
                        errorCount++;
                        //  totalErrorCount++;

                        validationErrorMessage.Add(string.Format(@"""VERVEACQUIRERIDNUMBER"" Lenght must not be more than {0}", binNoLength));
                    }
                }
                if (binNoLength != 0 && visa.Length != binNoLength)
                {
                    errorCount++;
                    //   totalErrorCount++;

                    validationErrorMessage.Add(string.Format(@"""VISAACQUIRERIDNUMBER"" Lenght must not be more than {0}", binNoLength));
                }
                if (binNoLength != 0 && mast.Length != binNoLength)
                {
                    errorCount++;
                    //  totalErrorCount++;


                    validationErrorMessage.Add(string.Format(@"""MASTERCARDACQUIRERIDNUMBER"" Lenght must not be more than {0}", binNoLength));

                }
            }
            return errorCount > 0;
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
        List<RvHeadObj> BindTerminalTemp(string batchId, string user_id)
        {
            // SessionHelper.GetMccMsc(Session).Clear();
            var rec = _repo.GetRvHeadTemp(batchId, user_id);

            return rec;

        }

        public ActionResult EditTerminal(string id)
        {
            try
            {

                var rv = new TidSession();
                var rec = rv.FindTerminal(id, User.Identity.Name);
                if (rec != null)
                {
                    BindComboTerm(rec.MERCHANTID);
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Terminal";
                    return PartialView("_AddTerminal", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public async Task<ActionResult> EditTerminalLocal(decimal id)
        {
            try
            {
                //var rv = new RvHeadSession();
                var rec = await _repo.GetTerminalByItbIdAsync(id);
                if (rec != null)
                {
                    var sig = rec.FirstOrDefault();
                    BindComboTerm(sig.MERCHANTID);
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Terminal";
                    return PartialView("_AddTerminal", sig);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ActionResult DeleteTerminal(string id)
        {
            var rv = new TidSession();
            try
            {
                rv.DeleteTerminal(id, User.Identity.Name);

                var lst2 = rv.GetTerminal(User.Identity.Name); // GetRvHeadLines().ToList();
                var html2 = PartialView("_ViewTerminalQueue", lst2).RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var lst3 = rv.GetTerminal(User.Identity.Name); //  GetRvHeadLines().ToList();
                var html = PartialView("_ViewTerminal", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }

        }
      
        #endregion
        void BindCombo(string acq_selected = null)
        {
            var acq = _repo.GetInstitution(0, true, "Active");
            var mcc = _repo.GetMCC(0, true, "Active").Select(d=> new {d.MCC_CODE, MCC_DESC = string.Concat(d.MCC_CODE,"-",d.MCC_DESC) });
            var country = _repo.GetCountry(0, true, "Active");

            ViewBag.AcquirerList = new SelectList(acq, "CBN_CODE", "INSTITUTION_NAME");
            ViewBag.CountryList = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
            ViewBag.MCCList = new SelectList(mcc, "MCC_CODE", "MCC_DESC");

            var sta = SmartObj.GetStatusMerchant();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

        }
        void BindState(string countryCode)
        {
            var rec = _repo.GetState(0, true, "Active",false, countryCode);
            ViewBag.StateList = new SelectList(rec, "STATECODE", "STATENAME");

        }
        void BindCity(string countryCode,string stateCode)
        {
            var rec = _repo.GetCityFilter(countryCode,stateCode);
            ViewBag.CityList = new SelectList(rec, "CITYCODE", "CITYNAME");

        }
        public async Task<ActionResult> Add(string id, string m = null)
        {
            try
            {

                SessionHelper.GetMerchantAcct(Session).Clear();
                ViewBag.MenuId = HttpUtility.UrlDecode(m);


                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    return RedirectToAction("Index", "Merchant", new { m = ViewBag.MenuId });
                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                //var d = _repo.GetSession(0, true);
                ViewBag.HeaderTitle = "Edit Merchant";
                ViewBag.StatusVisible = true;
                ViewBag.ButtonText = "Update";
                var rec = await _repo.GetMerchantByMidAsync(id, "", null);
                if (rec == null)
                {
                    ViewBag.Message = "Record Not Found";
                  
                    return View("Add");

                }
                var model = rec.FirstOrDefault();
                //var recAcct = await BindAccount(model.ITBID);
                BindCombo();
                BindState(model.COUNTRY_CODE);
                BindCity(model.COUNTRY_CODE, model.STATE_CODE);
                GetPriv();
                return View("Add", model);
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                ViewBag.Message = ex.Message;
                return View("Index", new { m = ViewBag.MenuId });
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
        public async Task<ActionResult> GetPartyAcct(string id)
        {
            try
            {
                
                var splt = id.Split('|');
                if (splt != null && splt.Length > 0)
                {
                    var tp = splt[1];
                    var itbid = decimal.Parse(splt[0]);
                    var rec = await _repo.GetMSC2PartyAcctAsync(itbid, tp);
                    return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {

            }
            return Json(new { RespCode = 1, RespMessage = "No record Found" }, JsonRequestBehavior.AllowGet);

        }
        void BindComboMsc()
            {
                //var bankList = _repo.GetInstitution(0, true, "Active").Where(f => f.IS_BANK == "Y").ToList();
                //var country = _repo.GetCountry(0, true, "Active");

                //ViewBag.BankList = new SelectList(bankList, "CBN_CODE", "INSTITUTION_NAME");
                //ViewBag.Country = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
                //var schemeList = _repo.GetCardScheme(0, true, "Active");
                //var country = _repo.GetCountry(0, true, "Active");

                //ViewBag.CardScheme = new SelectList(schemeList, "CARDSCHEME", "CARDSCHEME_DESC");
            var call = SmartObj.GetCalculationBasis();
            ViewBag.CalcBasis = new SelectList(call, "Code", "Description");

        }
        void BindComboParty()
        {
            var partyList = _repo.GetInstitutionParty();
            //var country = _repo.GetCountry(0, true, "Active");
            
            ViewBag.PartyList = new SelectList(partyList, "Code", "Description");
            ViewBag.AccountList = new SelectList(new List<DropdownObj>(), "CODE", "DESCRIPTION");
            //var schemeList = _repo.GetCardScheme(0, true, "Active");
            //var country = _repo.GetCountry(0, true, "Active");

            //ViewBag.CardScheme = new SelectList(schemeList, "CARDSCHEME", "CARDSCHEME_DESC");
            //var call = SmartObj.GetCalculationBasis();
            //ViewBag.CalcBasis = new SelectList(call, "Code", "Description");

        }
        void BindComboPartyAcct(decimal itbid,string option)
        {
            var rec = _repo.GetMSC2PartyAcct(itbid, option);
            ViewBag.AccountList = new SelectList(rec, "CODE", "DESCRIPTION");
        }
        public async Task<ActionResult> GetMerchantMsc(string id , string m = null)
        {
            var canAdd = false;
            var canEdit = false;
            try
            {
                ViewBag.MenuId = m;
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }

                var obj = await _repo.GetMerchantByMidAsync(id, null, null);
                var tt = obj.FirstOrDefault();
                if (tt != null)
                {
                    var rec = await _repo.GetMerchantMSCAsync(id, tt.INSTITUTION_CBNCODE, tt.MCC_CODE);
                    //var model = rec.FirstOrDefault();
                    if(rec != null && rec.Count <= 0)
                    {
                         rec = await _repo.GetMerchantMSCAsync(id, "000", tt.MCC_CODE);
                    }
                    Session["MerchantMscList"] = rec;
                    ViewBag.DisableButton = true;
                    if (rec.Where(d=> d.MERCHANTMSC_ITBID > 0).Count() > 0)
                    {
                        ViewBag.ButtonText = "Update";
                    }
                    else
                    {
                        ViewBag.ButtonText = "Save";
                    }
                    var dd = rec.Count > 0 ? rec[0] : null;
                    if (dd != null)
                    {
                        ViewBag.MCC = dd.MCC_CODE;
                        //var cbn_code = dd.CBN_CODE;
                        //var inst_cbnCode = dd.INSTITUTION_CBNCODE;
                        //ViewBag.Acquirer = dd.CBN_CODE;
                       // ViewBag.Institution = dd.INSTITUTION_CBNCODE;
                        var acquirer = repoInst.AllEager(d => d.CBN_CODE == dd.CBN_CODE).FirstOrDefault();
                        if (acquirer != null)
                        {
                            ViewBag.Acquirer = acquirer.INSTITUTION_NAME;
                        }
                        var sig = _repo.GetMerchantByMid(dd.MERCHANTID, "", null).FirstOrDefault();
                        if (sig != null)
                        {
                            ViewBag.Institution = sig.INSTITUTION_NAME;

                        }
                    }
                    // GET ALL DETAIL FOR MERCHANT IF AVAILABLE
                    var recMsc2 = _repo.GetMerchantMsc2Detail(tt.MERCHANTID,tt.MCC_CODE);
                   // var recMsc1Unshared = _repo.GetMerchantMsc1UnsharedDetail(merchantId);
                    //var recSubsidy = _repo.GetMerchantSubsidyDetail(merchantId);
                    Session["MerchantMsc2DetailList"] = recMsc2;
                    //Session["MerchantMsc1UnsharedDetailList"] = recMsc1Unshared;
                    //Session["MerchantSubsidyDetailList"] = recSubsidy;
                    var mp = await _repo.GetMenuPrivilegeAsync(menuId, roleId);
                    if(mp != null)
                    {
                        canAdd = mp.CanAdd;
                        canEdit = mp.CanEdit;
                    }
                    var html =  PartialView("_ViewMerchantMsc", rec).RenderToString();
                    var ht = new { data_html = html,data_CanAdd = canAdd, data_CanEdit = canEdit , RespCode = 0, RespMessage = ""};
                    
                    return Json(ht, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var obj2 = new { RespCode = 2, RespMessage = ex.Message };
                return Json(obj2, JsonRequestBehavior.AllowGet);
                //return null;
            }
            var obj1 = new { RespCode = 2, RespMessage = "Problem Processing Request" };
            return Json(obj1, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetMerchantAcct(string id, string m = null)
        {
            bool canAdd = false;
            bool canEdit = false;
            try
            {
                SessionHelper.GetMerchantAcct(Session).Clear();
                ViewBag.MenuId = m;
                // BindComboMsc();
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    // ViewBag.HeaderTitle = "Add Account";
                    // ViewBag.ButtonText = "Add";
                   // return PartialView("_AddAcct", new InstitutionAcctObj());
                    return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }

                var rec = await _repo.GetMerchantAcctAsync(id);
                foreach (var d in rec)
                {
                    SessionHelper.GetMerchantAcct(Session).AddItem(d);
                }
                var mp = await _repo.GetMenuPrivilegeAsync(menuId, roleId);
                if (mp != null)
                {
                    canAdd = mp.CanAdd;
                    canEdit = mp.CanEdit;
                }
                var html = PartialView("_ViewMerchantAcct", rec).RenderToString();
                var ht = new { data_html = html, data_CanAdd = canAdd, data_CanEdit = canEdit, RespCode = 0, RespMessage = "" };
                return Json(ht, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                var obj2 = new { RespCode = 2, RespMessage = ex.Message };
                return Json(obj2, JsonRequestBehavior.AllowGet);
                //return null;
            }
           
        }
        public async Task<ActionResult> GetMerchantTerminal(string id, string m = null)
        {
            try
            {
                var rv = new TidSession();
                rv.PurgeTerminal(User.Identity.Name);
                ViewBag.MenuId = m;
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }

                var rec = await _repo.GetTerminalByMidAsync(id, null);

                var html = PartialView("_ViewMerchantTerminal", rec).RenderToString();
                var ht = new { data_html = html, RespCode = 0, RespMessage = "" };
                return Json(ht, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj2 = new { RespCode = 2, RespMessage = ex.Message };
                return Json(obj2, JsonRequestBehavior.AllowGet);
                //return null;
            }
            //var obj1 = new { RespCode = 2, RespMessage = "Problem Processing Request" };
            //return Json(obj1, JsonRequestBehavior.AllowGet);
        }

       
     
        public PartialViewResult AddDomMsc2Party(decimal id = 0, string m = null)
        {
            try
            {
                BindComboParty();
                if (id == 0)
                {
                    ViewBag.HeaderTitle = "Add Domestic MSC 2 Party";
                    ViewBag.ButtonText = "Add";
                    return PartialView("_AddDomMsc2", new SharingPartyObj());
                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var d = _repo.GetSession(0, true);
                    ViewBag.HeaderTitle = "Edit Domestic MSC 2 Party";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    //var rec = await _repo.GetBanksAsync(id);
                    //if (rec == null)
                    //{
                    //    return null;
                    //}
                    //var model = rec.FirstOrDefault();
                    return PartialView("_AddDomMsc2", null);

                }
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                return null;
            }
        }
        public ActionResult EditDomMsc2Party(decimal id, string m)
        {
            try
            {
                var lst = GetMSC2DomLines();
                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {

                    BindComboParty();
                    BindComboPartyAcct(rec.PARTYITBID.GetValueOrDefault(), rec.PARTY_LOCATOR);
                    ViewBag.HeaderTitle = "Edit Domestic MSC 2 Party";
                    ViewBag.StatusVisible = true;
                    ViewBag.ButtonText = "Update";
                    return PartialView("_AddDomMsc2", rec);
                }
            }
            catch
            {

            }
            return null;
        }
        public ActionResult DeleteDomMsc2Party(decimal id, string m)
        {
            try
            {
                var lst = GetMSC2DomLines();

                var rec = lst.FirstOrDefault(f => f.ITBID == id);
                if (rec != null)
                {
                    //if (rec.NewRecord)
                    //{
                        SessionHelper.GetMerchantMSC2DomSharingParty(Session).RemoveLine(rec.ITBID);

                    //}
                    //else
                    //{
                    //    SessionHelper.GetMerchantMSC2DomSharingParty(Session).MarkForDelete(rec.ITBID);
                    //}
                    var lst2 = GetMSC2DomLines();
                    var tot = lst2.Sum(d => d.SHARINGVALUE).GetValueOrDefault().ToString("F");
                    var html2 = PartialView("_ViewDomMsc2", lst2).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "", data_html = html2,data_msc2 = tot }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var lst3 = GetMerchantAcctLines();
            var html = PartialView("_ViewMerchantAcct", lst3).RenderToString();
            return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
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
            public ActionResult AddAcct(MerchantAcctObj model, string m)
            {
                string msg = "";
                var bankName = "";
                var cntry_code = "";
                var bankAddress = "";
                var curDesc = "";
                try
                {
                    var lst = GetMerchantAcctLines().ToList();
                    if (model.ITBID == 0)
                    {
                        var exist = lst.Exists(r => r.DEPOSIT_BANKCODE == model.DEPOSIT_BANKCODE && r.DEPOSIT_ACCOUNTNO == model.DEPOSIT_ACCOUNTNO);
                        if (exist)
                        {
                            msg = "Account No Already exist. Duplicate Record is not allowed.";
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
                        var currency = repoCurrency.AllEager(d => d.CURRENCY_CODE == model.SETTLEMENTCURRENCY).FirstOrDefault();
                    if (currency != null)
                    {
                        curDesc = currency.CURRENCY_NAME;
                    }
                        var obj = new MerchantAcctObj()
                        {
                            DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_BANKADDRESS = bankAddress,
                            DEPOSIT_BANKNAME = bankName,
                            DEPOSIT_COUNTRYCODE = cntry_code,
                            DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                            SETTLEMENTCURRENCY = model.SETTLEMENTCURRENCY,
                            CURRENCYDESC = curDesc,
                            DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                            DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                            USERID = User.Identity.Name,
                            ITBID = itbid,
                            NewRecord = true,
                            DRACCOUNTNAME = model.DRACCOUNTNAME,
                            DRACCOUNTNO = model.DRACCOUNTNO,
                            DRBANKCODE = model.DRBANKCODE,
                        };


                        SessionHelper.GetMerchantAcct(Session).AddItem(obj);
                        var w = GetMerchantAcctLines();
                        var html = PartialView("_ViewMerchantAcct", w).RenderToString();
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
                                msg = "Account No already exist for selected Bank. Duplicate Record is not allowed.";
                                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                            }
                        }
                        var currency = repoCurrency.AllEager(d => d.CURRENCY_CODE == model.SETTLEMENTCURRENCY).FirstOrDefault();
                        if (currency != null)
                        {
                            curDesc = currency.CURRENCY_NAME;
                        }
                        var inst = repoInst.AllEager(d => d.CBN_CODE == model.DEPOSIT_BANKCODE).FirstOrDefault();
                        if (inst != null)
                        {
                            bankName = inst.INSTITUTION_NAME;
                            cntry_code = inst.INSTITUTION_COUNTRY;
                            bankAddress = inst.INSTITUTION_ADDRESS;
                        }

                        var obj = new MerchantAcctObj()
                        {
                            DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                            DEPOSIT_BANKADDRESS = bankAddress,
                            DEPOSIT_BANKNAME = bankName,
                            DEPOSIT_COUNTRYCODE = cntry_code,

                            // DEPOSIT_COUNTRYCODE = drpcountrycode.SelectedValue,
                            DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                            SETTLEMENTCURRENCY = model.SETTLEMENTCURRENCY,
                            CURRENCYDESC = curDesc,
                            DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                            DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                            USERID = User.Identity.Name,
                            ITBID = model.ITBID,
                            NewRecord = oldRec.NewRecord,
                            Updated = true,
                            DRACCOUNTNAME = model.DRACCOUNTNAME,
                            DRACCOUNTNO = model.DRACCOUNTNO,
                            DRBANKCODE = model.DRBANKCODE,

                        };

                        SessionHelper.GetMerchantAcct(Session).UpdateItem(obj);

                        var w = GetMerchantAcctLines();
                        var html = PartialView("_ViewMerchantAcct", w).RenderToString();
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
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddMsc(MerchantMscObj model, string m)
        {
            GetPriv();
            //decimal dom_msc1, dom_msc1Cap, dom_msc2, dom_msc2Cap, dom_sharedMsc, dom_sharedCap, dom_subsidy;

            //decimal int_msc1, int_msc1Cap, int_msc2, int_msc2Cap, int_sharedMsc, int_sharedCap, int_subsidy, amtDueOthers = 0;
            string retMsg = "";
            decimal mcc_Itbid = model.ITBID;
            var rec = GetMerchantMsc(); // Session["MerchantMscList"] as List<MerchantMscObj>;
                                        //long.TryParse(hidMccMscItbId.Value, out mcc_Itbid);
            if (rec != null)
            {
                if (!ValidateMsc1("D", out retMsg))
                {
                    // var msg = string.Format("alert('{0}');", retMsg);
                    // ScriptManager.RegisterClientScriptBlock(this, GetType(), "popup1", msg, true);
                    // return;
                }

                //if (chkSetAmtDue.Checked)
                //{
                //    if (!decimal.TryParse(txtAmtDueOthers.Text, out amtDueOthers))
                //    {
                //        var msg = "alert('Please Setup the Amount due to merchant section');";
                //        ScriptManager.RegisterClientScriptBlock(this, GetType(), "popup1", msg, true);
                //        return;
                //    }

                //}
                var obj = rec.FirstOrDefault(f => f.ITBID == mcc_Itbid);
                if (obj != null)
                {
                    obj.DOM_MSCVALUE = model.DOM_MSCVALUE; // : obj.DOM_MSCVALUE;
                    obj.DOMCAP = model.DOMCAP; // : obj.DOMCAP;

                    obj.MSC_CALCBASIS = model.MSC_CALCBASIS;
                    obj.INTMSC_CALCBASIS = model.INTMSC_CALCBASIS;

                    obj.DOM_MSC2 = model.DOM_MSC2; // decimal.TryParse(txtDomMsc2.Text, out dom_msc2) ? dom_msc2 : obj.DOM_MSC2;
                    obj.DOM_MSC2CAP = model.DOM_MSC2CAP; // decimal.TryParse(txtDomMsc2Cap.Text, out dom_msc2Cap) ? dom_msc2Cap : obj.DOM_MSC2CAP;

                    obj.DOM_MSCSUBSIDY = model.DOM_MSCSUBSIDY; // decimal.TryParse(txtDomSubsidy.Text, out dom_subsidy) ? dom_subsidy : obj.DOM_MSCSUBSIDY;


                    obj.INT_MSCVALUE = model.INT_MSCVALUE; // intMsc1Suc ? int_msc1 : obj.INT_MSCVALUE;
                    obj.INTLCAP = model.INTLCAP; // intCapSuc ? int_msc1Cap : obj.INTLCAP;

                    obj.INT_MSC2 = model.INT_MSC2; // decimal.TryParse(txtIntMsc2.Text, out int_msc2) ? int_msc2 : obj.INT_MSC2;
                    obj.INT_MSC2CAP = model.INT_MSC2CAP; // decimal.TryParse(txtIntMscCap.Text, out int_msc2Cap) ? int_msc2Cap : obj.INT_MSC2CAP;

                    obj.INT_MSCSUBSIDY = model.INT_MSCSUBSIDY; // decimal.TryParse(txtIntSubsidy.Text, out int_subsidy) ? int_subsidy : obj.INT_MSCSUBSIDY;

                    // obj.INT_UNSHAREDCAP = intSharedCapSuc && intCapSuc ? int_msc1Cap - int_sharedCap : intSharedCapSuc ? (decimal)obj.INTLCAP - int_sharedCap : obj.INT_UNSHAREDCAP;
                    if (model.INT_SHAREDCAP >= 0 && model.INTLCAP >= 0)
                    {
                        if (model.INTLCAP > model.INT_SHAREDCAP)
                        {
                            obj.INT_UNSHAREDCAP = model.INTLCAP.GetValueOrDefault() - model.INT_SHAREDCAP;
                        }
                        else
                        {
                            obj.INT_UNSHAREDCAP = 0;
                        }
                    }
                    else
                    {
                        obj.INT_UNSHAREDCAP = 0;
                    }
                    //  obj.INT_UNSHAREDMSC = intMsc1SharedSuc && intMsc1Suc ? int_msc1 - int_sharedMsc : intMsc1SharedSuc ? (decimal)obj.INT_MSCVALUE - int_sharedMsc : obj.INT_UNSHAREDMSC;
                    if (model.INT_SHAREDMSC >= 0 && model.INT_MSCVALUE >= 0/* intMsc1SharedSuc && intMsc1Suc*/)
                    {
                        if (model.INT_MSCVALUE > model.INT_SHAREDMSC)
                        {
                            obj.INT_UNSHAREDMSC = model.INT_MSCVALUE.GetValueOrDefault() - model.INT_SHAREDMSC;
                        }
                        else
                        {
                            obj.INT_UNSHAREDMSC = 0;
                        }
                    }
                    else
                    {
                        obj.INT_UNSHAREDMSC = 0;
                    }
                    // obj.MCC = hidMccCode.Value;
                    //  obj.DOM_UNSHAREDCAP = domSharedCapSuc && domCapSuc ? dom_msc1Cap - dom_sharedCap : domSharedCapSuc ? (decimal)obj.DOMCAP - dom_sharedCap : obj.DOM_UNSHAREDCAP;
                    if (model.MERCHANTDOMCAP >= 0  /*domSharedCapSuc && domCapSuc*/)
                    {
                        if (model.MERCHANTDOMCAP > model.DOM_SHAREDCAP)
                        {
                            obj.DOM_UNSHAREDCAP = model.MERCHANTDOMCAP - model.DOM_SHAREDCAP;
                        }
                        else
                        {
                            obj.DOM_UNSHAREDCAP = 0;
                        }
                    }
                    else
                    {
                        obj.DOM_UNSHAREDCAP = 0;
                    }
                    // obj.DOM_UNSHAREDMSC = domMsc1SharedSuc && domMsc1Suc ? dom_msc1 - dom_sharedMsc : domMsc1SharedSuc ? (decimal)obj.DOM_MSCVALUE - dom_sharedMsc : obj.DOM_UNSHAREDMSC;
                    if (model.DOM_SHAREDMSC >= 0 && model.DOM_MSCVALUE >= 0/*domMsc1SharedSuc && domMsc1Suc*/)
                    {
                        if (model.DOM_MSCVALUE > model.DOM_SHAREDMSC)
                        {
                            obj.DOM_UNSHAREDMSC = model.DOM_MSCVALUE.GetValueOrDefault() - model.DOM_SHAREDMSC;
                        }
                        else
                        {
                            obj.DOM_UNSHAREDMSC = 0;
                        }
                    }
                    else
                    {
                        obj.DOM_UNSHAREDMSC = 0;
                    }
                    //if (chkSetAmtDue.Checked)
                    //{
                    //    obj.AMOUNTDUEMERCH_PERC = 100 - amtDueOthers;
                    //    obj.APPLYMERCHANTSHARING = "Y";
                    //    obj.AMOUNTDUEMERCH_TYPE = rdAmtDue.SelectedValue;
                    //}
                    //else
                    //{
                    //    obj.APPLYMERCHANTSHARING = "N";
                    //    obj.AMOUNTDUEMERCH_PERC = 100;
                    //    obj.AMOUNTDUEMERCH_TYPE = null;
                    //}
                    //var obj2 = rec.FirstOrDefault(f => f.ITBID == mcc_Itbid);
                    var recMsc2List = new List<SharingPartyObj>();
                    var recMsc1UnsharedList = new List<SharingPartyObj>();
                    var recSubsidyList = new List<SharingPartyObj>();
                    var domLine = GetMSC2DomLines();
                    var intLine = GetMSC2IntLines();
                    // var domUnsharedLine = GetUnsharedDomLines();
                    //var intUnsharedLine = GetUnsharedIntLines();
                    //var domSubsidyLine = GetSubsidyDomLines();
                    //var intSubsidyLine = GetSubsidyIntLines();
                    if (domLine != null && domLine.Count != 0)
                    {
                        recMsc2List.AddRange(domLine);
                    }
                    if (intLine != null && intLine.Count != 0)
                    {
                        recMsc2List.AddRange(intLine);
                    }

                    //if (domUnsharedLine != null && domUnsharedLine.Count != 0)
                    //{
                    //    recMsc1UnsharedList.AddRange(domUnsharedLine);
                    //}
                    //if (intUnsharedLine != null && intUnsharedLine.Count != 0)
                    //{
                    //    recMsc1UnsharedList.AddRange(intUnsharedLine);
                    //}

                    //if (domSubsidyLine != null && domSubsidyLine.Count != 0)
                    //{
                    //    recSubsidyList.AddRange(domSubsidyLine);
                    //}
                    //if (intSubsidyLine != null && intSubsidyLine.Count != 0)
                    //{
                    //    recSubsidyList.AddRange(intSubsidyLine);
                    //}
                    Session["MerchantMsc2DetailList"] = recMsc2List;
                    //Session["MerchantMsc1UnsharedDetailList"] = recMsc1UnsharedList;
                    // Session["MerchantSubsidyDetailList"] = recSubsidyList;
                    //rptMerchantMsc.DataSource = rec;
                    //rptMerchantMsc.DataBind();
                }
            }
            if(rec.Count(d=> d.MERCHANTMSC_ITBID > 0) > 0)
            {
                ViewBag.ButtonText = "Update";
            }
            else
            {
                ViewBag.ButtonText = "Save";
            }
            var html = PartialView("_ViewMerchantMsc", rec).RenderToString();
            //msg = "Record Added to List";
            return Json(new { data_html = html, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        }
        ////private void ProcessMSC2Int()
        ////{
        ////    decimal msc1 = 0;
        ////    decimal.TryParse(txtDomMsc1.Text, out msc1);
        ////    pnlModal.Visible = false;
        ////    int partySelected = 0;
        ////    decimal partySHARINGvAL = 0;
        ////    string partydesc = null;
        ////    string tranType = hidMsc2TranType.Value;
        ////    string p_Locaor = "";
        ////    // decimal partyValue;
        ////    decimal total = 0;
        ////    decimal msc2_unshared_Value = 0;
        ////    // decimal.TryParse(txtPartyValue.Value, out partyValue);
        ////    decimal.TryParse(txtDomMsc2.Text, out msc2_unshared_Value);
        ////    decimal.TryParse(txtPartyMsc2.Text, out partySHARINGvAL);

        ////    if (!string.IsNullOrEmpty(drpPartyMsc2.SelectedValue))
        ////    {
        ////        var sel = drpPartyMsc2.SelectedValue;
        ////        var splt = sel.Split('|');
        ////        if (splt.Count() == 2)
        ////        {
        ////            partySelected = int.Parse(splt[0]);
        ////            p_Locaor = splt[1];
        ////            partydesc = drpPartyMsc2.SelectedItem.Text;
        ////        }
        ////        else
        ////        {
        ////            return;
        ////        }
        ////    }

        ////    var lst = GetMSC2IntLines().ToList();




        ////    pnlModal.Visible = false;


        ////    //lst = Session["ProcessorList"] as List<InstitutionProcessorObj>;
        ////    if (btnAddPartyMsc2.Text == "Save")
        ////    {
        ////        //var sumValue = lst.Sum(f => f.SHARINGVALUE);
        ////        //if (sumValue != null)
        ////        //{
        ////        //    sumValue += partySHARINGvAL;
        ////        //    if (sumValue > msc2_unshared_Value)
        ////        //    {
        ////        //        pnlModal.Visible = true;
        ////        //        pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
        ////        //        ltErrorModal.Text = string.Format("The Total Value for all parties must not be greater than {0}", msc2_unshared_Value);
        ////        //        return;
        ////        //    }
        ////        //    total = (decimal)sumValue;
        ////        //}
        ////        var exist = lst.Exists(r => r.PARTYITBID == partySelected && r.PARTY_LOCATOR == p_Locaor);
        ////        if (exist)
        ////        {
        ////            // Party Type already exist. Duplicate Record is not allowed.;
        ////            pnlModal.Visible = true;
        ////            pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
        ////            ltErrorModal.Text = "Party already exist. Duplicate Record is not allowed.";
        ////            return;
        ////        }

        ////        long itbid = 0;
        ////        if (lst.Count == 0)
        ////        {

        ////            itbid = 1;
        ////        }
        ////        else
        ////        {
        ////            var itb = lst.Max(f => f.ITBID);
        ////            itbid = itb + 1;
        ////        }


        ////        var obj = new SharingPartyObj()
        ////        {
        ////            PARTYITBID = partySelected,
        ////            //  PARTYTYPE_VALUE = partyValue,
        ////            PartyName = partydesc,
        ////            SHARINGVALUE = partySHARINGvAL,
        ////            MERCHANTID = hidMid.Value,
        ////            TRANTYPE = tranType,
        ////            PARTY_LOCATOR = p_Locaor,
        ////            ITBID = itbid,
        ////            NewRecord = true,
        ////            STATUS = inActive,
        ////            CARDSCHEME = hidCardSheme.Value

        ////        };

        ////        SessionHelper.GetMerchantMSC2IntSharingParty(Session).AddItem(obj);
        ////        var w = GetMSC2IntLines().ToList();

        ////        rptMSC2Int.DataSource = w;
        ////        rptMSC2Int.DataBind();
        ////        var tot = w.Sum(f => f.SHARINGVALUE);
        ////        lblIntMsc2TotalValue.Text = tot != null ? ((decimal)tot).ToString("F") : "";
        ////        txtIntMsc2.Text = tot != null ? ((decimal)tot).ToString("F") : "";
        ////        lblTotalDomMsc.Text = ((tot ?? 0) + msc1).ToString();
        ////        ClearMscForm();
        ////        var script = ModalPopupScript("$('#myModal').modal('hide');");

        ////        ScriptManager.RegisterClientScriptBlock(this, GetType(), "EditHideModalIntScript", script, false);

        ////    }
        ////    else
        ////    {
        ////        int curParty;
        ////        int.TryParse(hidMsc2PartySelected.Value, out curParty);

        ////        var exist = lst.Where(r => r.PARTYITBID == partySelected && r.PARTY_LOCATOR == p_Locaor).FirstOrDefault();
        ////        if (exist != null)
        ////        {
        ////            if (partySelected != curParty)
        ////            {
        ////                // Party Type already exist. Duplicate Record is not allowed.;
        ////                pnlModal.Visible = true;
        ////                pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
        ////                ltErrorModal.Text = "Party already exist. Duplicate Record is not allowed.";
        ////                return;
        ////            }

        ////            //var sumValue = lst.Sum(f => f.SHARINGVALUE) ?? 0;
        ////            //sumValue -= (exist.SHARINGVALUE ?? 0);

        ////            //    sumValue += partySHARINGvAL;
        ////            //    if (sumValue > msc2_unshared_Value)
        ////            //    {
        ////            //        pnlModal.Visible = true;
        ////            //        pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
        ////            //        ltErrorModal.Text = string.Format("The Total Value for all parties must not be greater than {0}", msc2_unshared_Value);
        ////            //        return;
        ////            //    }
        ////            //    total = sumValue;

        ////        }
        ////        else
        ////        {
        ////            // var sumValue = lst.Sum(f => f.SHARINGVALUE) ?? 0;
        ////            //// sumValue -= (exist.SHARINGVALUE ?? 0);

        ////            // sumValue += partySHARINGvAL;
        ////            // if (sumValue > msc2_unshared_Value)
        ////            // {
        ////            //     pnlModal.Visible = true;
        ////            //     pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
        ////            //     ltErrorModal.Text = string.Format("The Total Value for all parties must not be greater than {0}", msc2_unshared_Value);
        ////            //     return;
        ////            // }
        ////            // total = sumValue;
        ////        }

        ////        bool newRecord;
        ////        bool.TryParse(hidNew.Value, out newRecord);


        ////        var obj = new SharingPartyObj()
        ////        {
        ////            PARTYITBID = partySelected,
        ////            //  PARTYTYPE_VALUE = partyValue,
        ////            PartyName = partydesc,
        ////            SHARINGVALUE = partySHARINGvAL,
        ////            MERCHANTID = hidMid.Value,
        ////            TRANTYPE = tranType,
        ////            PARTY_LOCATOR = p_Locaor,


        ////            STATUS = inActive,
        ////            ITBID = int.Parse(hidMsc2PartyItbid.Value),
        ////            NewRecord = newRecord,
        ////            Updated = true,
        ////            CARDSCHEME = hidCardSheme.Value,

        ////        };
        ////        SessionHelper.GetMerchantMSC2IntSharingParty(Session).UpdateItem(obj);
        ////        var w = GetMSC2IntLines().ToList();
        ////        rptMSC2Int.DataSource = w;
        ////        rptMSC2Int.DataBind();
        ////        var tot = w.Sum(f => f.SHARINGVALUE);
        ////        lblIntMsc2TotalValue.Text = tot != null ? ((decimal)tot).ToString("F") : "";
        ////        txtIntMsc2.Text = tot != null ? ((decimal)tot).ToString("F") : "";
        ////        lblTotalDomMsc.Text = ((tot ?? 0) + msc1).ToString();
        ////        ClearMscForm();
        ////        var script = ModalPopupScript("$('#myModal').modal('hide');");

        ////        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalIntScript", script, false);
        ////    }
        ////}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDomMsc2(SharingPartyObj model)
        {
            int pt = model.ITBID == 0 ? 1 : 2;
            
            var msg = ProcessMSC2Dom(model,pt);
            if(!string.IsNullOrEmpty(msg))
            {
                return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
            }
            var rec = GetMSC2DomLines().ToList();
            var tot = rec.Sum(f => f.SHARINGVALUE);
            var tot_cap = rec.Sum(f => f.CAP);
            var html = PartialView("_ViewDomMsc2", rec).RenderToString();
            //msg = "Record Added to List";
            return Json(new { data_html = html,data_msc2 = tot.GetValueOrDefault().ToString("F"), data_cap2 = tot_cap.GetValueOrDefault().ToString("F"), RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        }
        private string ProcessMSC2Dom(SharingPartyObj model,int pt)
        {
            string msg = "";
            int partySelected = 0;
            string partydesc = null;
            string tranType = "D"; // hidMsc2TranType.Value;
            string p_Locaor = "";
            var selectedParty = model.PARTY_ID;
            if (!string.IsNullOrEmpty(selectedParty))
            {
                var sel = selectedParty;
                var splt = sel.Split('|');
                if (splt.Count() == 2)
                {
                    partySelected = int.Parse(splt[0]);
                    p_Locaor = splt[1];
                    var dd = _repo.GetInstitutionParty().Where(d => d.Code == selectedParty).FirstOrDefault();
                    partydesc = dd.Description;
                }
                else
                {
                    return "No Party Selected";
                }
            }
            if (!model.splitincome)
            {
                model.ACCOUNT_ID2 = null;
                model.sharingRateAccount1 = null;
                model.sharingRateAccount2 = null;
            }

            var lst = GetMSC2DomLines().ToList();
            if (pt == 1)
            {
                var exist = lst.Exists(r => r.PARTYITBID == partySelected && r.PARTY_LOCATOR == p_Locaor);
                if (exist)
                {
                    // Party Type already exist. Duplicate Record is not allowed.;
                    //pnlModal.Visible = true;
                    //pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
                    msg = "Party already exist. Duplicate Record is not allowed.";
                    return msg;
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
               
                var obj = new SharingPartyObj()
                {
                    PARTYITBID = partySelected,
                    PartyName = partydesc,
                    SHARINGVALUE = model.SHARINGVALUE,
                    MERCHANTID = model.MERCHANTID,
                    TRANTYPE = tranType,
                    PARTY_ID = model.PARTY_ID,
                    PARTY_LOCATOR = p_Locaor,
                    CAP = model.CAP,
                    ITBID = itbid,
                     NewRecord = true,
                    STATUS = inActive,
                    MERCHANTMSC_ITBID = model.MERCHANTMSC_ITBID,
                    MCCMSC_ITBID = model.MCCMSC_ITBID,
                    ACCOUNT_ID = model.ACCOUNT_ID,
                    ACCOUNT_ID2 = model.ACCOUNT_ID2,
                    splitincome = model.splitincome,
                    sharingRateAccount1 = model.sharingRateAccount1,
                    sharingRateAccount2 = model.sharingRateAccount2,
                    CARDSCHEME = Session["CARDSCHEME"].ToString(),
                    //CARDSCHEME =Session["CARDSCHEME"] ,
                    //BANKCODE = pBankCode,
                    //BANKACCOUNT = pAcct


                };

                SessionHelper.GetMerchantMSC2DomSharingParty(Session).AddItem(obj);
                return "";
            }
            else
            {
                //int curParty = 0;
                //var exist = lst.Where(r => r.PARTYITBID == partySelected && r.PARTY_LOCATOR == p_Locaor).FirstOrDefault();
                //if (exist != null)
                //{
                //    if (partySelected != curParty)
                //    {
                //        // Party Type already exist. Duplicate Record is not allowed.;
                //        // pnlModal.Visible = true;
                //        //pnlModal.CssClass = "alert alert-danger alert-dismissable alert-bold";
                //        msg = "Party already exist. Duplicate Record is not allowed.";
                //        return msg;
                //    }

                //}

                var oldRec = lst.Where(d => d.ITBID == model.ITBID).FirstOrDefault();
                var existRec = lst.Where(r => r.PARTY_LOCATOR == p_Locaor && r.PARTYITBID == partySelected).FirstOrDefault();
                if (existRec != null) // not expected to be null
                {
                    if (oldRec.ITBID != existRec.ITBID)
                    {
                        msg = "Party already exist. Duplicate Record is not allowed.";
                        return msg;
                    }
                }

                bool newRecord = false;
               // bool.TryParse(hidNew.Value, out newRecord);
                var obj = new SharingPartyObj()
                {
                    PARTYITBID = partySelected,
                    PARTY_ID = model.PARTY_ID,
                    PartyName = partydesc,
                    SHARINGVALUE = model.SHARINGVALUE,
                    MERCHANTID = model.MERCHANTID,
                    TRANTYPE = tranType,
                    PARTY_LOCATOR = p_Locaor,
                    STATUS = inActive,
                    CAP = model.CAP,
                    ITBID = model.ITBID, // int.Parse(hidMsc2PartyItbid.Value),
                    NewRecord = newRecord,
                    Updated = true,
                    MERCHANTMSC_ITBID = model.MERCHANTMSC_ITBID,
                    MCCMSC_ITBID = model.MCCMSC_ITBID,
                    ACCOUNT_ID = model.ACCOUNT_ID,
                    ACCOUNT_ID2 = model.ACCOUNT_ID2,
                    splitincome = model.splitincome,
                    sharingRateAccount1 = model.sharingRateAccount1,
                    sharingRateAccount2 = model.sharingRateAccount2,

                };
                SessionHelper.GetMerchantMSC2DomSharingParty(Session).UpdateItem(obj);
                return "";
            }
        }
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddMerchantMsc(int pt,string CBN_CODE,string MID, string m)
        {
            try
            {
               // menuId = SmartUtil.GetMenuId(m);
                if (pt == 2)
                {
                    //its an update
                    string bid = string.Concat("MSC_", SmartObj.GenRefNo2());
                    ////  SaveMCCRuleTemp(eventEdit, bid);
                    //SaveMCCRuleTemp2(eventEdit, bid);
                    SaveMerchantMsc(eventEdit, bid,CBN_CODE,MID);
                    //  var rst1 = new AuthListUtil().SaveLog(auth);
                    var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                    if (rst)
                    {
                        SM_AUTHLIST auth = new SM_AUTHLIST()
                        {
                            CREATEDATE = DateTime.Now,
                            EVENTTYPE = eventEdit,
                            MENUID = menuId,
                            //MENUNAME = "",
                            // RECORDID =  setId,
                            STATUS = open,
                            // TABLENAME = "ADMIN_DEPARTMENT",
                            URL = Request.FilePath,
                            USERID = User.Identity.Name,
                            BATCHID = bid,
                            POSTTYPE = Single,
                            INSTITUTION_ITBID = institutionId,
                            // RECORDID = decimal.Parse(hidItbid.Value)
                        };

                        repoAuth.Insert(auth);
                        var rst1 = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst1)
                        {

                            //pnlResponseMsg.Text = "<i class='fa fa-info' ></i> Record Updated SuccessFully...Awaiting Authorization";
                            var msg = "Record Updated SuccessFully...Awaiting Authorization";
                            return Json(new { RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }

                    }
                    //else
                    //{
                    //    pnlForm.Visible = false;
                    //    pnlGrid.Visible = true;
                    //    pnlResponse.Visible = true;
                    //    pnlResponse.CssClass = "alert alert-warning alert-dismissable alert-bold fade in";
                    //    pnlResponseMsg.Text = "<i class='fa fa-info' ></i> Record Updated SuccessFully";
                    //    clearForm();
                    //}

                }
                else
                {

                    // its an insert
                    //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    //{

                        //SaveMSC(eventInsert);
                        string bid = string.Concat("MSC", "_", SmartObj.GenRefNo2());
                        //  SaveMCCRuleTemp(eventInsert, bid);
                        SaveMerchantMsc(eventInsert, bid,CBN_CODE,MID);
                        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                        if (rst)
                        {

                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                // RECORDID = setId,
                                STATUS = open,
                                // TABLENAME = "ManageMCC",
                                URL = Request.FilePath,
                                USERID = User.Identity.Name,
                                BATCHID = bid,
                                INSTITUTION_ITBID = institutionId,
                                POSTTYPE = Single,

                            };
                            repoAuth.Insert(auth);
                            var rst1 = uow.Save(User.Identity.Name);
                           // txscope.Complete();
                            var msg = "Record Created SuccessFully...Awaiting Authorization";
                            return Json(new { RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            //pnlForm.Visible = false;
                            //pnlDetails.Visible = true;

                        }
                    }
                //}
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 2, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);

            }

            //var html = PartialView("_ViewMerchantMsc", rec).RenderToString();
            //msg = "Record Added to List";
            return Json(new {RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        }
        void SaveMerchantMsc(string eventType, string bid,string cbn_code,string mid)
        {
            var curDate = DateTime.Now;
            var recList = GetMerchantMsc(); // Session["MerchantMscList"] as List<MerchantMscObj>;
            if (eventType == "New")
            {
                if (recList != null)
                {
                    foreach (var d in recList)
                    {
                        //if (cbn_code != "100")
                        //{
                        //    continue;
                        //}
              
                        if (d.MERCHANTMSC_ITBID == 0)
                        {
                            // its new here
                            var obj = new SM_MERCHANTMSCTEMP()
                            {
                                CREATEDATE = curDate,
                                MCCMSC_ITBID = d.ITBID,
                                MERCHANTID = mid,
                                STATUS = open,
                                USERID = User.Identity.Name,
                                MERCHANTDOMCAP = d.DOMCAP,
                                MERCHANTINTLCAP = d.INTLCAP,
                                BATCHID = bid,
                                RECORDID = d.MERCHANTMSC_ITBID,
                                DOM_MSC2 = d.DOM_MSC2,
                                INT_MSC2 = d.INT_MSC2,
                                INT_UNSHAREDMSC = d.INT_UNSHAREDMSC,
                                DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC,
                                DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY,
                                INT_MSCSUBSIDY = d.INT_MSCSUBSIDY,
                                DOM_MSC1 = d.DOM_MSCVALUE,
                                DOM_MSC2CAP = d.DOM_MSC2CAP,
                                DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP,
                                INT_MSC1 = d.INT_MSCVALUE,
                                INT_MSC2CAP = d.INT_MSC2CAP,
                                INT_UNSHAREDCAP = d.INT_UNSHAREDCAP,
                                MCC = d.MCC_CODE,
                                CBN_CODE = d.CBN_CODE,
                                CARDSCHEME = d.CARDSCHEME,
                                MSC_CALCBASIS = d.MSC_CALCBASIS,
                                INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                                CHANNEL = d.CHANNEL,
                            };
                            //if (d.APPLYMERCHANTSHARING == "Y")
                            //{
                            //    obj.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                            //    obj.AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE;
                            //    obj.AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC;
                            //}
                            //else
                            //{
                            //    obj.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                            //    obj.AMOUNTDUEMERCH_TYPE = null;
                            //    obj.AMOUNTDUEMERCH_PERC = null;
                            //}
                            repoMerchantMscTemp.Insert(obj);
                        }
                    }

                    var recMsc2List = GetMSC2Party(); // Session["MerchantMsc2DetailList"] as List<SharingPartyObj>;
                   // var recMsc1UnsharedList = Session["MerchantMsc1UnsharedDetailList"] as List<SharingPartyObj>;
                   // var recSubsidyList = Session["MerchantSubsidyDetailList"] as List<SharingPartyObj>;

                    if (recMsc2List != null)
                    {
                        foreach (var d in recMsc2List)
                        {

                            var objmsc2Det = new SM_SHAREDMSC2DETAILTEMP()
                            {
                                CREATEDATE = DateTime.Now,
                                BATCHID = bid,
                                MERCHANTID = mid,
                                PARTYITBID = d.PARTYITBID,
                                PARTY_LOCATOR = d.PARTY_LOCATOR,
                                //RECORDID = d.RECORDID,
                                SHARINGVALUE = d.SHARINGVALUE,
                                STATUS = open,
                                TRANTYPE = d.TRANTYPE,
                                USERID = User.Identity.Name,
                                CARDSCHEME = d.CARDSCHEME,
                                BANKACCOUNT = d.BANKACCOUNT,
                                BANKCODE = d.BANKCODE,
                                MERCHANTMSC_ITBID = d.MERCHANTMSC_ITBID,
                                MCCMSC_ITBID = d.MCCMSC_ITBID,
                                CAP = d.CAP,
                                ACCOUNT_ID = d.ACCOUNT_ID,
                                ACCOUNT_ID2 = d.ACCOUNT_ID2,
                                sharingRateAccount1 = d.sharingRateAccount1,
                                sharingRateAccount2 = d.sharingRateAccount2,
                                splitincome = d.splitincome,
                                // RECORDID = d.NewRecord ? (long?)null : d.ITBID,
                            };

                            repoSharedMsc2Temp.Insert(objmsc2Det);

                        }
                    }
                    //if (recMsc1UnsharedList != null)
                    //{
                    //    foreach (var d in recMsc1UnsharedList)
                    //    {
                    //        var objmsc2Det = new POSMISDB_UNSHAREMSC1DETAILTEMP()
                    //        {
                    //            CREATEDATE = DateTime.Now,
                    //            BATCHID = bid,
                    //            MERCHANTID = txtMerchantId.Value,
                    //            PARTYITBID = d.PARTYITBID,
                    //            PARTY_LOCATOR = d.PARTY_LOCATOR,
                    //            RECORDID = d.NewRecord ? (long?)null : d.ITBID,
                    //            SHARINGVALUE = d.SHARINGVALUE,
                    //            STATUS = open,
                    //            TRANTYPE = d.TRANTYPE,
                    //            USERID = userId,
                    //            CARDSCHEME = d.CARDSCHEME
                    //        };
                    //        repoUnSharedMsc1Temp.Insert(objmsc2Det);
                    //    }
                    //}

                    //if (recSubsidyList != null)
                    //{
                    //    foreach (var d in recSubsidyList)
                    //    {

                    //        var objSubDet = new POSMISDB_SUBSIDYDETAILTEMP()
                    //        {
                    //            CREATEDATE = DateTime.Now,
                    //            BATCHID = bid,
                    //            MERCHANTID = txtMerchantId.Value,
                    //            PARTYITBID = d.PARTYITBID,
                    //            PARTY_LOCATOR = d.PARTY_LOCATOR,
                    //            //RECORDID = d.RECORDID,
                    //            SHARINGVALUE = d.SHARINGVALUE,
                    //            STATUS = open,
                    //            TRANTYPE = d.TRANTYPE,
                    //            USERID = userId,
                    //            CARDSCHEME = d.CARDSCHEME,
                    //            // RECORDID = d.NewRecord ? (long?)null : d.ITBID,
                    //        };

                    //        repoSubTemp.Insert(objSubDet);

                    //    }
                    //}

                }
            }
            else
            {
                //for (int i = 0; i < cnt; i++)
                foreach (var d in recList)
                {
                    //if (cbn_code != "100")
                    //{
                    //    continue;
                    //}
                    
                    var obj = new SM_MERCHANTMSCTEMP()
                    {
                        CREATEDATE = curDate,

                        MCCMSC_ITBID = d.ITBID,
                        MERCHANTID = mid,
                        STATUS = open,
                        USERID = User.Identity.Name,
                        MERCHANTDOMCAP = d.DOMCAP,
                        MERCHANTINTLCAP = d.INTLCAP,
                        BATCHID = bid,
                        RECORDID = d.MERCHANTMSC_ITBID,
                        DOM_MSC2 = d.DOM_MSC2,
                        INT_MSC2 = d.INT_MSC2,
                        INT_UNSHAREDMSC = d.INT_UNSHAREDMSC,
                        DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC,
                        DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY,
                        INT_MSCSUBSIDY = d.INT_MSCSUBSIDY,
                        DOM_MSC1 = d.DOM_MSCVALUE,
                        DOM_MSC2CAP = d.DOM_MSC2CAP,
                        DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP,
                        INT_MSC1 = d.INT_MSCVALUE,
                        INT_MSC2CAP = d.INT_MSC2CAP,
                        INT_UNSHAREDCAP = d.INT_UNSHAREDCAP,
                        MCC = d.MCC_CODE,
                        CBN_CODE = d.CBN_CODE,
                        CARDSCHEME = d.CARDSCHEME,
                        MSC_CALCBASIS = d.MSC_CALCBASIS,
                        INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                        CHANNEL = d.CHANNEL
                    };
                    //if (d.APPLYMERCHANTSHARING == "Y")
                    //{
                    //    obj.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                    //    obj.AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE;
                    //    obj.AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC;
                    //}
                    //else
                    //{
                    //    obj.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                    //    obj.AMOUNTDUEMERCH_TYPE = null;
                    //    obj.AMOUNTDUEMERCH_PERC = null;
                    //}
                    repoMerchantMscTemp.Insert(obj);

                }
                var recMsc2List = GetMSC2Party(); // Session["MerchantMsc2DetailList"] as List<SharingPartyObj>;
               // var recMsc1UnsharedList = Session["MerchantMsc1UnsharedDetailList"] as List<SharingPartyObj>;
                //var recSubsidyList = Session["MerchantSubsidyDetailList"] as List<SharingPartyObj>;
                if (recMsc2List != null)
                {
                    foreach (var d in recMsc2List)
                    {
                        var objmsc2Det = new SM_SHAREDMSC2DETAILTEMP()
                        {
                            CREATEDATE = DateTime.Now,
                            BATCHID = bid,
                            MERCHANTID = mid,
                            PARTYITBID = d.PARTYITBID,
                            PARTY_LOCATOR = d.PARTY_LOCATOR,
                            RECORDID = d.NewRecord ? (decimal?)null : d.ITBID,
                            SHARINGVALUE = d.SHARINGVALUE,
                            STATUS = open,
                            TRANTYPE = d.TRANTYPE,
                            USERID = User.Identity.Name,
                            CARDSCHEME = d.CARDSCHEME,
                            BANKACCOUNT = d.BANKACCOUNT,
                            BANKCODE = d.BANKCODE,
                            MERCHANTMSC_ITBID = d.MERCHANTMSC_ITBID,
                            MCCMSC_ITBID = d.MCCMSC_ITBID,
                            CAP = d.CAP,
                            ACCOUNT_ID = d.ACCOUNT_ID,
                            ACCOUNT_ID2 = d.ACCOUNT_ID2,
                            sharingRateAccount1 = d.sharingRateAccount1,
                            sharingRateAccount2 = d.sharingRateAccount2,
                            splitincome = d.splitincome,
                        };
                        repoSharedMsc2Temp.Insert(objmsc2Det);
                    }
                }
                //if (recMsc1UnsharedList != null)
                //{
                //    foreach (var d in recMsc1UnsharedList)
                //    {
                //        var objmsc2Det = new POSMISDB_UNSHAREMSC1DETAILTEMP()
                //        {
                //            CREATEDATE = DateTime.Now,
                //            BATCHID = bid,
                //            MERCHANTID = txtMerchantId.Value,
                //            PARTYITBID = d.PARTYITBID,
                //            PARTY_LOCATOR = d.PARTY_LOCATOR,
                //            RECORDID = d.NewRecord ? (long?)null : d.ITBID,
                //            SHARINGVALUE = d.SHARINGVALUE,
                //            STATUS = open,
                //            TRANTYPE = d.TRANTYPE,
                //            USERID = userId,
                //            CARDSCHEME = d.CARDSCHEME
                //        };
                //        repoUnSharedMsc1Temp.Insert(objmsc2Det);
                //    }
                //}
                //if (recSubsidyList != null)
                //{
                //    foreach (var d in recSubsidyList)
                //    {
                //        var objmsc2Det = new POSMISDB_SUBSIDYDETAILTEMP()
                //        {
                //            CREATEDATE = DateTime.Now,
                //            BATCHID = bid,
                //            MERCHANTID = txtMerchantId.Value,
                //            PARTYITBID = d.PARTYITBID,
                //            PARTY_LOCATOR = d.PARTY_LOCATOR,
                //            RECORDID = d.NewRecord ? (long?)null : d.ITBID,
                //            SHARINGVALUE = d.SHARINGVALUE,
                //            STATUS = open,
                //            TRANTYPE = d.TRANTYPE,
                //            USERID = userId,
                //            CARDSCHEME = d.CARDSCHEME
                //        };
                //        repoSubTemp.Insert(objmsc2Det);
                //    }
                //}
                //}
            }
        }
        public IList<SharingPartyObj> GetMSC2DomLines()
        {
            // var D = 
            return SessionHelper.GetMerchantMSC2DomSharingParty(Session).Lines;
        }
        public IList<SharingPartyObj> GetMSC2IntLines()
        {
            // var D = 
            return SessionHelper.GetMerchantMSC2IntSharingParty(Session).Lines;
        }
        //public IList<SharingPartyObj> GetUnsharedDomLines()
        //{
        //    // var D = 
        //    return SessionHelper.GetMerchantUnsharedDomSharingParty(Session).Lines;
        //}
        //public IList<SharingPartyObj> GetUnsharedIntLines()
        //{
        //    // var D = 
        //    return SessionHelper.GetMerchantUnsharedIntSharingParty(Session).Lines;
        //}

        //public IList<SharingPartyObj> GetSubsidyDomLines()
        //{
        //    // var D = 
        //    return SessionHelper.GetMerchantSubsidyDomSharingParty(Session).Lines;
        //}
        //public IList<SharingPartyObj> GetSubsidyIntLines()
        //{
        //    // var D = 
        //    return SessionHelper.GetMerchantSubsidyIntSharingParty(Session).Lines;
        //}
        void ClearAllSession()
        {
            SessionHelper.GetMerchantMSC2DomSharingParty(Session).Clear();
            SessionHelper.GetMerchantMSC2IntSharingParty(Session).Clear();
            //SessionHelper.GetMerchantUnsharedDomSharingParty(Session).Clear();
            //SessionHelper.GetMerchantUnsharedIntSharingParty(Session).Clear();
            //SessionHelper.GetMerchantSubsidyDomSharingParty(Session).Clear();
            //SessionHelper.GetMerchantSubsidyIntSharingParty(Session).Clear();
        }
        bool ValidateMsc1(string tranType, out string retMsg)
        {

            //decimal msc1_val;
            //decimal totalmc1party_val;
            //int partyCount;
            // decimal totalmc1party_val;
            if (tranType == "D")
            {
                //if (decimal.TryParse(txtDomUnSharedMsc.Text, out msc1_val))
                //{
                //    partyCount = GetUnsharedDomLines().Count;
                //    totalmc1party_val = GetUnsharedDomLines().Sum(e => e.SHARINGVALUE) ?? 0;
                //    if (partyCount > 0)  // value is set and party set
                //    {
                //        if (msc1_val != totalmc1party_val) // check if value set is equal to total party value set
                //        {
                //            retMsg = string.Format(@"Total Sharing Values among Parties must be equal to MSC 1 Unshared Value ""{0}""", msc1_val);
                //            return false; // msc1 is not equal to msc1 value 
                //        }
                //        else
                //        {
                //            retMsg = "";
                //            return true;
                //        }
                //    }
                //    else // value is set party is not set
                //    {
                //        retMsg = string.Format(@"It is mandatory to add party and sharing Value if MSC1 Unshared Value is set ""{0}""", msc1_val);
                //        return false;

                //    }

                //}
                //else // value is not set 
                //{
                //    partyCount = GetUnsharedDomLines().Count;
                //    totalmc1party_val = GetUnsharedDomLines().Sum(e => e.SHARINGVALUE) ?? 0;
                //    if (partyCount == 0)
                //    {
                //        retMsg = "";
                //        return true;
                //    }
                //    else
                //    {
                //        retMsg = string.Format(@"It is mandatory to add party and sharing Value if MSC1 Unshared Value is set ""{0}""", msc1_val);
                //        return false;
                //    }
                //}
            }
            else
            {

                //partyCount = 0;

                //     if (decimal.TryParse(txtIntMsc1.Text, out msc1_val))
                //     {
                //         partyCount = GetUnsharedIntLines().Count;
                //         totalmc1party_val = GetUnsharedIntLines().Sum(e => e.SHARINGVALUE) ?? 0;
                //         if (partyCount > 0)  // value is set and party set
                //         {
                //             if (msc1_val != totalmc1party_val) // check if value set is equal to total party value set
                //             {
                //                 retMsg = string.Format(@"Total Sharing Values among Parties must be equal to MSC 1 Unshared Value ""{0}""", msc1_val);
                //                 return false; // msc1 is not equal to msc1 value 
                //             }
                //             else
                //             {
                //                 retMsg = "";
                //                 return true;
                //             }
                //         }
                //         else // value is set party is not set
                //         {
                //             retMsg = string.Format(@"It is mandatory to add party and sharing Value if MSC1 Unshared Value is set ""{0}""", msc1_val);
                //             return false;

                //         }

                //     }
                //     else // value is not set 
                //     {
                //         partyCount = GetUnsharedIntLines().Count;
                //         totalmc1party_val = GetUnsharedIntLines().Sum(e => e.SHARINGVALUE) ?? 0;
                //         if (partyCount == 0)
                //         {
                //             retMsg = "";
                //             return true;
                //         }
                //         else
                //         {
                //             retMsg = string.Format(@"It is mandatory to add party and sharing Value if MSC1 Unshared Value is set ""{0}""", msc1_val);
                //             return false;
                //         }
                //     }
            }
            retMsg = "";
            return true;
        }
        bool ValidateMsc2(string tranType)
        {
            return false;
        }
        bool ValidateSubsidy(string tranType)
        {
            return false;
        }
        public ActionResult EditMSC(decimal id, string m)
        {
            ClearAllSession();
            var htmlString = "";
            BindComboMsc();
            var mList = Session["MerchantMscList"] as List<MerchantMscObj>;
            if(mList != null && mList.Count > 0)
            {
                var rec = mList.FirstOrDefault(f => f.ITBID == id);
                Session["CARDSCHEME"] = rec.CARDSCHEME;
                var recMsc2List = GetMSC2Party(); // Session["MerchantMsc2DetailList"] as List<SharingPartyObj>;
                //var recMsc1UnsharedList = Session["MerchantMsc1UnsharedDetailList"] as List<SharingPartyObj>;
                //var recSubsidyList = Session["MerchantSubsidyDetailList"] as List<SharingPartyObj>;
                if (recMsc2List != null && recMsc2List.Count != 0)
                {
                    var dom = recMsc2List.Where(d => d.TRANTYPE == "D" && d.MCCMSC_ITBID == rec.ITBID && d.CHANNEL == rec.CHANNEL).ToList();
                    
                    rec.DomMsc2List = dom;
                    if (dom.Count > 0)
                    {
                        SessionHelper.GetMerchantMSC2DomSharingParty(Session).AddRange(dom);
                    }
                    var intList = recMsc2List.Where(d => d.TRANTYPE == "I" && d.CARDSCHEME == rec.CARDSCHEME).ToList();
                    // bind int partial here
                   // var SUM1 = intList.Sum(F => F.SHARINGVALUE) ?? 0;
                    //lblIntMsc2TotalValue.Text = SUM1.ToString("F");
                    if (intList.Count > 0)
                    {
                        SessionHelper.GetMerchantMSC2IntSharingParty(Session).AddRange(intList);
                    }
                }
                htmlString = PartialView("_AddMSC", rec).RenderToString();

                return Json(new { data_html = htmlString, RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { data_html = htmlString, RespCode = 1, RespMessage = "Problem Processing Request." }, JsonRequestBehavior.AllowGet);

            }
            //var html = PartialView("_AddMSC").RenderToString();

            //try
            //{
            //    ViewBag.MenuId = m;

            //    var lst = GetBanksLines().ToList();

            //    var rec = lst.FirstOrDefault(f => f.ITBID == id);
            //    if (rec != null)
            //    {
            //        BindComboMsc();
            //        ViewBag.ButtonText = "Update";
            //        return PartialView("_AddAcct", rec);
            //    }
            //}
            //catch
            //{

            //}
            //return null;
        }
        List<SharingPartyObj> GetMSC2Party()
        {
            return Session["MerchantMsc2DetailList"] as List<SharingPartyObj>;
        }
        List<MerchantMscObj> GetMerchantMsc()
        {
            return Session["MerchantMscList"] as List<MerchantMscObj>;
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

     
                    //ViewBag.StatusVisible = true;
                    if (det != null)
                    {
                        var bid = det.BATCHID;
                        var splt = bid.Split('_');
                        var frmType = splt[0];
                        obj.AuthId = authId;
                        obj.RecordId = det.RECORDID.GetValueOrDefault();
                        obj.BatchId = det.BATCHID;
                        obj.PostType = det.POSTTYPE;
                        obj.MenuId = det.MENUID.GetValueOrDefault();
                        ViewBag.Message = TempData["msg"];
                        var status = TempData["status"];
                        var stat = status == null ? "open" : status.ToString();

                        var viewtoDisplay = "";
                        switch (frmType)
                        {
                            case "MSC":
                                {
                                    ViewBag.HeaderTitle = "Authorize Detail for Merchant MSC";
                                    viewtoDisplay = "DetailAuthMSC";
                                    var rec =  _repo.GetMerchantMSCTemp(null, det.BATCHID,"", det.USERID);
                                    if (rec != null && rec.Count > 0)
                                    {
                                        var model = rec.FirstOrDefault();

                                        obj.Status = det.STATUS;
                                        obj.EventType = det.EVENTTYPE;
                                        obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                        obj.User = model.CREATED_BY;
                                        ViewBag.Auth = obj;
                                        ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                        return View(viewtoDisplay, rec);
                                    }
                                    break;
                                }
                            case "MMA":
                                {
                                    ViewBag.HeaderTitle = "Authorize Detail for Merchant Account";
                                    viewtoDisplay = "DetailAuthMA";
                                    var rec = _repo.GetMerchantAcctTemp(det.BATCHID, det.USERID);
                                    if (rec != null && rec.Count > 0)
                                    {
                                        var model = rec.FirstOrDefault();
                                        obj.Status = det.STATUS;
                                        obj.EventType = det.EVENTTYPE;
                                        obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                        obj.User = model.CREATED_BY;
                                        ViewBag.Auth = obj;
                                        ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                        return View(viewtoDisplay, rec);
                                    }
                                    break;
                                }
                            case "MT":
                                {
                                    ViewBag.HeaderTitle = "Authorize Detail for Merchant Terminal";
                                    viewtoDisplay = "DetailAuthMT";
                                    var rec = _repo.GetMerchantTerminalTemp(det.BATCHID, det.USERID);
                                    if (rec != null && rec.Count > 0)
                                    {
                                        var model = rec.FirstOrDefault();
                                        obj.Status = det.STATUS;
                                        obj.EventType = det.EVENTTYPE;
                                        obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                        obj.User = model.CREATED_BY;
                                        ViewBag.Auth = obj;
                                        ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                        return View(viewtoDisplay, rec);
                                    }
                                    break;
                                }
                            case "MMD":
                                {
                                    ViewBag.HeaderTitle = "Authorize Detail for Merchant Detail";
                                    viewtoDisplay = "DetailAuth";
                                    var rec = _repo.GetMerchantDetailFromTemp(det.BATCHID, det.USERID);
                                    if (rec != null )
                                    {
                                        BindCombo();
                                        BindState(rec.COUNTRY_CODE);
                                        //BindCity(rec.COUNTRY_CODE, rec.STATE_CODE);
                                        var model = rec;
                                        obj.Status = det.STATUS;
                                        obj.EventType = det.EVENTTYPE;
                                        obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                        obj.User = model.CREATED_BY;
                                        ViewBag.Auth = obj;
                                        ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                        return View(viewtoDisplay, model);
                                    }
                                    break;
                                }
                            default:
                                {
                                    viewtoDisplay = "DetailAuth";
                                    break;
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
                        //bad request
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
                    //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew,new TimeSpan(0,5,0)))
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
                                var sp = rec2.BATCHID.Split('_');
                                var frmType = sp[0];
                                    recordId = (int)rec2.RECORDID.GetValueOrDefault();
                                    menuId = rec2.MENUID.GetValueOrDefault();
                                    switch (rec2.EVENTTYPE)
                                    {
                                        case "New":
                                            {

                                            if (frmType == "MSC")
                                            {
                                                suc = ProcessMerchantMsc(rec2.BATCHID, eventInsert, rec2.USERID);
                                            }
                                            else if (frmType == "MMA")
                                            {
                                                suc = ModifyMerchantAcct(rec2.BATCHID, rec2.USERID);
                                            }
                                            else if (frmType == "MMD")
                                            {
                                               suc = CreateNewRecord(recordId, rec2.USERID);
                                            }
                                            break;
                                            }
                                        case "Modify":
                                        {
                                            if (frmType == "MSC")
                                            {
                                                suc = ProcessMerchantMsc(rec2.BATCHID, eventInsert, rec2.USERID);
                                            }
                                            else if (frmType == "MMA")
                                            {
                                                suc = ModifyMerchantAcct(rec2.BATCHID, rec2.USERID);
                                            }
                                            else if (frmType == "MMD")
                                            {
                                                suc = ModifyMainRecord(recordId, rec2.USERID);
                                            }
                                            else if (frmType == "MT")
                                            {
                                                suc = ModifyMerchantTerminal(rec2.BATCHID, rec2.USERID);
                                            }
                                            break;

                                        }
                                    case "BLOCKED":
                                        {
                                            if (frmType == "MMD")
                                            {
                                                suc = DisableMainRecord(recordId);
                                            }
                                            else if (frmType == "MT")
                                            {
                                                //suc = ModifyMerchantTerminal(rec2.BATCHID, rec2.USERID);
                                            }
                                          
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

                                        EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Merchant Update Record", null, fullName);
                                       // txscope.Complete();
                                        //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                                        respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                                            TempData["msg"] = respMsg;
                                            TempData["status"] = approve;
                                            return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                            //return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
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
                        // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                        respMsg = "This request has already been processed by an authorizer.";
                        TempData["msg"] = respMsg;
                        return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                        //return Json(new { RespCode = 1, RespMessage = respMsg });
                    //}

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
        private bool DisableMainRecord(int recordId)
        {
            var rec = repoMTemp.Find(recordId);
            if (rec != null)
            {
                rec.STATUS = approve;
                var obj = repoM.Find(rec.MERCHANTID);
                if (obj != null)
                {
                    obj.STATUS = "BLOCKED";
                    return true;
                }
            }
            return false;
        }

        private bool ProcessMerchantMsc(string batchid, string eventType, string user_Id)
        {
            var dt = repoMerchantMscTemp.AllEager(e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
            if (dt.Count != 0)
            {
                if (eventType == eventInsert)
                {
                    foreach (var d in dt)
                    {
                        var dmsc = repoSharedMsc2Temp.AllEager(e => e.BATCHID == batchid && e.MCCMSC_ITBID == d.MCCMSC_ITBID && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();

                        var gh = repoMerchantMsc.AllEager(g => g.MCC == d.MCC && g.CARDSCHEME == d.CARDSCHEME && g.CBN_CODE == d.CBN_CODE && g.MERCHANTID == d.MERCHANTID && g.CHANNEL == d.CHANNEL).FirstOrDefault();
                        if (gh == null)
                        {
                             gh = new SM_MERCHANTMSC()
                            {
                                BATCHID = d.BATCHID,
                                DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY,
                                INT_MSCSUBSIDY = d.INT_MSCSUBSIDY,
                                MCCMSC_ITBID = d.MCCMSC_ITBID,
                                MERCHANTDOMCAP = d.MERCHANTDOMCAP,
                                MERCHANTID = d.MERCHANTID,
                                MERCHANTINTLCAP = d.MERCHANTINTLCAP,
                                DOM_MSC2 = d.DOM_MSC2,
                                INT_MSC2 = d.INT_MSC2,
                                INT_UNSHAREDMSC = d.INT_UNSHAREDMSC,
                                DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC,
                                DOM_MSC1 = d.DOM_MSC1,
                                DOM_MSC2CAP = d.DOM_MSC2CAP,
                                DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP,
                                INT_MSC1 = d.INT_MSC1,
                                INT_MSC2CAP = d.INT_MSC2CAP,
                                INT_UNSHAREDCAP = d.INT_UNSHAREDCAP,
                                STATUS = active,
                                CREATEDATE = DateTime.Now,
                                USERID = user_Id,
                                AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC,
                                AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE,
                                APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING,
                                MCC = d.MCC,
                                CBN_CODE = d.CBN_CODE,
                                CARDSCHEME = d.CARDSCHEME,
                                MSC_CALCBASIS = d.MSC_CALCBASIS,
                                INTMSC_CALCBASIS = d.INTMSC_CALCBASIS,
                                CHANNEL = d.CHANNEL,
                            };
                            d.STATUS = approve;
                            if(dmsc.Count != 0)
                            {
                                var frst = dmsc[0];
                                var midd = frst.MERCHANTID;
                                var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == midd).ToList();
                                foreach (var tt in existRec)
                                {
                                    repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                                }
                                List<SM_SHAREDMSC2DETAIL> msc2List = new List<SM_SHAREDMSC2DETAIL>();
                                foreach(var tt in dmsc)
                                {
                                    var objMsc2 = new SM_SHAREDMSC2DETAIL()
                                    {
                                        CREATEDATE = tt.CREATEDATE,
                                        MERCHANTID = tt.MERCHANTID,
                                        PARTYITBID = tt.PARTYITBID,
                                        PARTY_LOCATOR = tt.PARTY_LOCATOR,
                                        SHARINGVALUE = tt.SHARINGVALUE,
                                        STATUS = active,
                                        TRANTYPE = tt.TRANTYPE,
                                        USERID = tt.USERID,
                                        CARDSCHEME = tt.CARDSCHEME,
                                        BANKACCOUNT = tt.BANKACCOUNT,
                                        BANKCODE = tt.BANKCODE,
                                        MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID,
                                        CAP = tt.CAP,
                                        ACCOUNT_ID = tt.ACCOUNT_ID,
                                        ACCOUNT_ID2 = tt.ACCOUNT_ID2,
                                        sharingRateAccount1 = tt.sharingRateAccount1,
                                        sharingRateAccount2 = tt.sharingRateAccount2,
                                        SPLITINCOME = tt.splitincome,

                                    };
                                    msc2List.Add(objMsc2);
                                }
                                if (msc2List.Count > 0)
                                {
                                    gh.SM_SHAREDMSC2DETAIL = msc2List;
                                }
                            }
                           // 
                            repoMerchantMsc.Insert(gh);
                        }
                        else
                        {
                            gh.BATCHID = d.BATCHID;
                            gh.DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY;
                            gh.INT_MSCSUBSIDY = d.INT_MSCSUBSIDY;
                            gh.MCCMSC_ITBID = d.MCCMSC_ITBID;
                            gh.MERCHANTDOMCAP = d.MERCHANTDOMCAP;
                            gh.MERCHANTINTLCAP = d.MERCHANTINTLCAP;
                            gh.MERCHANTID = d.MERCHANTID;
                            gh.DOM_MSC2 = d.DOM_MSC2;
                            gh.INT_MSC2 = d.INT_MSC2;
                            gh.INT_UNSHAREDMSC = d.INT_UNSHAREDMSC;
                            gh.DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC;
                            gh.LAST_MODIFIED_UID = user_Id;
                            gh.DOM_MSC1 = d.DOM_MSC1;
                            gh.DOM_MSC2CAP = d.DOM_MSC2CAP;
                            gh.DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP;
                            gh.INT_MSC1 = d.INT_MSC1;
                            gh.INT_MSC2CAP = d.INT_MSC2CAP;
                            gh.INT_UNSHAREDCAP = d.INT_UNSHAREDCAP;
                            gh.AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC;
                            gh.AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE;
                            gh.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                            gh.MCC = d.MCC;
                            gh.CBN_CODE = d.CBN_CODE;
                            gh.CARDSCHEME = d.CARDSCHEME;
                            gh.MSC_CALCBASIS = d.MSC_CALCBASIS;
                            gh.INTMSC_CALCBASIS = d.INTMSC_CALCBASIS;
                            gh.CHANNEL = d.CHANNEL;
                            if (dmsc.Count != 0)
                            {
                                var frst = dmsc[0];
                                var midd = frst.MERCHANTID;
                                var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == midd).ToList();
                                foreach (var tt in existRec)
                                {
                                    repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                                }
                                List<SM_SHAREDMSC2DETAIL> msc2List = new List<SM_SHAREDMSC2DETAIL>();
                                foreach (var tt in dmsc)
                                {
                                    var objMsc2 = new SM_SHAREDMSC2DETAIL()
                                    {
                                        CREATEDATE = tt.CREATEDATE,
                                        MERCHANTID = tt.MERCHANTID,
                                        PARTYITBID = tt.PARTYITBID,
                                        PARTY_LOCATOR = tt.PARTY_LOCATOR,
                                        SHARINGVALUE = tt.SHARINGVALUE,
                                        STATUS = active,
                                        TRANTYPE = tt.TRANTYPE,
                                        USERID = tt.USERID,
                                        CARDSCHEME = tt.CARDSCHEME,
                                        BANKACCOUNT = tt.BANKACCOUNT,
                                        BANKCODE = tt.BANKCODE,
                                        MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID,
                                        CAP = tt.CAP,
                                        ACCOUNT_ID = tt.ACCOUNT_ID,
                                        ACCOUNT_ID2 = tt.ACCOUNT_ID2,
                                        sharingRateAccount1 = tt.sharingRateAccount1,
                                        sharingRateAccount2 = tt.sharingRateAccount2,
                                        SPLITINCOME = tt.splitincome,
                                    };
                                    msc2List.Add(objMsc2);
                                }
                                if (msc2List.Count > 0)
                                {
                                    gh.SM_SHAREDMSC2DETAIL = msc2List;
                                }
                            }
                        }
                    }
                    //string mid = "";
                    //var dmsc = repoSharedMsc2Temp.AllEager(e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
                    //if (dmsc.Count != 0)
                    //{
                    //    var frst = dmsc[0];
                    //    mid = frst.MERCHANTID;
                    //    var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == mid).ToList();
                    //    foreach (var tt in existRec)
                    //    {
                    //        repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    }
                    //    foreach (var tt in dmsc)
                    //    {
                    //        var objMsc2 = new SM_SHAREDMSC2DETAIL()
                    //        {
                    //            CREATEDATE = tt.CREATEDATE,
                    //            MERCHANTID = tt.MERCHANTID,
                    //            PARTYITBID = tt.PARTYITBID,
                    //            PARTY_LOCATOR = tt.PARTY_LOCATOR,
                    //            SHARINGVALUE = tt.SHARINGVALUE,
                    //            STATUS = active,
                    //            TRANTYPE = tt.TRANTYPE,
                    //            USERID = tt.USERID,
                    //            CARDSCHEME = tt.CARDSCHEME,
                    //            BANKACCOUNT = tt.BANKACCOUNT,
                    //            BANKCODE = tt.BANKCODE,
                    //            MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID,
                    //        };
                    //        repoSharedMsc2.Insert(objMsc2);
                    //    }
                    //}
                    //mid = "";
                    //var dmsc2 = repoUnSharedMsc1Temp.AllEager(null, e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
                    //if (dmsc2.Count != 0)
                    //{
                    //    var frst = dmsc2[0];
                    //    mid = frst.MERCHANTID;
                    //    var existRec = repoUnSharedMsc1.AllEager(null, f => f.MERCHANTID == mid).ToList();
                    //    foreach (var tt in existRec)
                    //    {
                    //        repoUnSharedMsc1.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    }
                    //    foreach (var tt in dmsc2)
                    //    {
                    //        var objMsc2 = new POSMISDB_UNSHAREDMSC1DETAIL()
                    //        {
                    //            CREATEDATE = tt.CREATEDATE,
                    //            MERCHANTID = tt.MERCHANTID,
                    //            PARTYITBID = tt.PARTYITBID,
                    //            PARTY_LOCATOR = tt.PARTY_LOCATOR,
                    //            SHARINGVALUE = tt.SHARINGVALUE,
                    //            STATUS = active,
                    //            TRANTYPE = tt.TRANTYPE,
                    //            USERID = tt.USERID,
                    //            CARDSCHEME = tt.CARDSCHEME

                    //        };

                    //        repoUnSharedMsc1.Insert(objMsc2);
                    //    }
                    //}

                    //mid = "";
                    //var dsub = repoSubTemp.AllEager(null, e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
                    //if (dsub.Count != 0)
                    //{
                    //    var frst = dsub[0];
                    //    mid = frst.MERCHANTID;
                    //    var existRec = repoSub.AllEager(null, f => f.MERCHANTID == mid).ToList();
                    //    foreach (var tt in existRec)
                    //    {
                    //        repoSub.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    }
                    //    foreach (var tt in dsub)
                    //    {
                    //        var objMsc2 = new POSMISDB_SUBSIDYDETAIL()
                    //        {
                    //            CREATEDATE = tt.CREATEDATE,
                    //            MERCHANTID = tt.MERCHANTID,
                    //            PARTYITBID = tt.PARTYITBID,
                    //            PARTY_LOCATOR = tt.PARTY_LOCATOR,
                    //            SHARINGVALUE = tt.SHARINGVALUE,
                    //            STATUS = active,
                    //            TRANTYPE = tt.TRANTYPE,
                    //            USERID = tt.USERID,
                    //            CARDSCHEME = tt.CARDSCHEME

                    //        };

                    //        repoSub.Insert(objMsc2);
                    //    }
                    //}
                }

                else if (eventType == eventEdit)
                {
                    foreach (var d in dt)
                    {
                        var dmsc = repoSharedMsc2Temp.AllEager(e => e.BATCHID == batchid && e.MCCMSC_ITBID == d.MCCMSC_ITBID && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();

                        d.STATUS = approve;
                        var gh = repoMerchantMsc.Find(d.RECORDID);
                        if (gh != null)
                        {
                            gh.BATCHID = d.BATCHID;
                            gh.DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY;
                            gh.INT_MSCSUBSIDY = d.INT_MSCSUBSIDY;
                            gh.MCCMSC_ITBID = d.MCCMSC_ITBID;
                            gh.MERCHANTDOMCAP = d.MERCHANTDOMCAP;
                            gh.MERCHANTINTLCAP = d.MERCHANTINTLCAP;
                            gh.MERCHANTID = d.MERCHANTID;
                            gh.DOM_MSC2 = d.DOM_MSC2;
                            gh.INT_MSC2 = d.INT_MSC2;
                            gh.INT_UNSHAREDMSC = d.INT_UNSHAREDMSC;
                            gh.DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC;
                            gh.LAST_MODIFIED_UID = user_Id;
                            gh.CHANNEL = d.CHANNEL;
                            gh.DOM_MSC1 = d.DOM_MSC1;
                            gh.DOM_MSC2CAP = d.DOM_MSC2CAP;
                            gh.DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP;
                            gh.INT_MSC1 = d.INT_MSC1;
                            gh.INT_MSC2CAP = d.INT_MSC2CAP;
                            gh.INT_UNSHAREDCAP = d.INT_UNSHAREDCAP;
                            gh.AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC;
                            gh.AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE;
                            gh.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                            gh.MCC = d.MCC;
                            gh.CBN_CODE = d.CBN_CODE;
                            gh.CARDSCHEME = d.CARDSCHEME;
                            gh.MSC_CALCBASIS = d.MSC_CALCBASIS;
                            gh.INTMSC_CALCBASIS = d.INTMSC_CALCBASIS;
                            if (dmsc.Count != 0)
                            {
                                var frst = dmsc[0];
                                var midd = frst.MERCHANTID;
                                var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == midd).ToList();
                                foreach (var tt in existRec)
                                {
                                    repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                                }
                                List<SM_SHAREDMSC2DETAIL> msc2List = new List<SM_SHAREDMSC2DETAIL>();
                                foreach (var tt in dmsc)
                                {
                                    var objMsc2 = new SM_SHAREDMSC2DETAIL()
                                    {
                                        CREATEDATE = tt.CREATEDATE,
                                        MERCHANTID = tt.MERCHANTID,
                                        PARTYITBID = tt.PARTYITBID,
                                        PARTY_LOCATOR = tt.PARTY_LOCATOR,
                                        SHARINGVALUE = tt.SHARINGVALUE,
                                        STATUS = active,
                                        TRANTYPE = tt.TRANTYPE,
                                        USERID = tt.USERID,
                                        CARDSCHEME = tt.CARDSCHEME,
                                        BANKACCOUNT = tt.BANKACCOUNT,
                                        BANKCODE = tt.BANKCODE,
                                        MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID,
                                        CAP = tt.CAP,
                                        ACCOUNT_ID = tt.ACCOUNT_ID,
                                        ACCOUNT_ID2 = tt.ACCOUNT_ID2,
                                        sharingRateAccount1 = tt.sharingRateAccount1,
                                        sharingRateAccount2 = tt.sharingRateAccount2,
                                        SPLITINCOME = tt.splitincome,
                                    };
                                    msc2List.Add(objMsc2);
                                }
                                if (msc2List.Count > 0)
                                {
                                    gh.SM_SHAREDMSC2DETAIL = msc2List;
                                }
                            }
                        }
                        else
                        {
                            var gh2 = repoMerchantMsc.AllEager(g => g.MCC == d.MCC && g.CARDSCHEME == d.CARDSCHEME && g.CBN_CODE == d.CBN_CODE && g.MERCHANTID == d.MERCHANTID).FirstOrDefault();
                            if (gh2 == null)
                            {
                                var obj = new SM_MERCHANTMSC()
                                {
                                    BATCHID = d.BATCHID,
                                    DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY,
                                    INT_MSCSUBSIDY = d.INT_MSCSUBSIDY,
                                    MCCMSC_ITBID = d.MCCMSC_ITBID,
                                    CHANNEL = d.CHANNEL,
                                    MERCHANTDOMCAP = d.MERCHANTDOMCAP,
                                    MERCHANTID = d.MERCHANTID,
                                    MERCHANTINTLCAP = d.MERCHANTINTLCAP,
                                    STATUS = active,
                                    CREATEDATE = DateTime.Now,
                                    USERID = user_Id,
                                    DOM_MSC2 = d.DOM_MSC2,
                                    INT_MSC2 = d.INT_MSC2,
                                    INT_UNSHAREDMSC = d.INT_UNSHAREDMSC,
                                    DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC,
                                    DOM_MSC1 = d.DOM_MSC1,
                                    DOM_MSC2CAP = d.DOM_MSC2CAP,
                                    DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP,
                                    INT_MSC1 = d.INT_MSC1,
                                    INT_MSC2CAP = d.INT_MSC2CAP,
                                    INT_UNSHAREDCAP = d.INT_UNSHAREDCAP,
                                    AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC,
                                    AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE,
                                    APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING,
                                    MCC = d.MCC,
                                    CBN_CODE = d.CBN_CODE,
                                    CARDSCHEME = d.CARDSCHEME,
                                    MSC_CALCBASIS = d.MSC_CALCBASIS,
                                    INTMSC_CALCBASIS = d.INTMSC_CALCBASIS
                                };
                                if (dmsc.Count != 0)
                                {
                                    var frst = dmsc[0];
                                    var midd = frst.MERCHANTID;
                                    var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == midd).ToList();
                                    foreach (var tt in existRec)
                                    {
                                        repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                                    }
                                    List<SM_SHAREDMSC2DETAIL> msc2List = new List<SM_SHAREDMSC2DETAIL>();
                                    foreach (var tt in dmsc)
                                    {
                                        var objMsc2 = new SM_SHAREDMSC2DETAIL()
                                        {
                                            CREATEDATE = tt.CREATEDATE,
                                            MERCHANTID = tt.MERCHANTID,
                                            PARTYITBID = tt.PARTYITBID,
                                            PARTY_LOCATOR = tt.PARTY_LOCATOR,
                                            SHARINGVALUE = tt.SHARINGVALUE,
                                            STATUS = active,
                                            TRANTYPE = tt.TRANTYPE,
                                            USERID = tt.USERID,
                                            CARDSCHEME = tt.CARDSCHEME,
                                            BANKACCOUNT = tt.BANKACCOUNT,
                                            BANKCODE = tt.BANKCODE,
                                            MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID,
                                            CAP = tt.CAP,
                                            ACCOUNT_ID = tt.ACCOUNT_ID,
                                            ACCOUNT_ID2 = tt.ACCOUNT_ID2,
                                            sharingRateAccount1 = tt.sharingRateAccount1,
                                            sharingRateAccount2 = tt.sharingRateAccount2,
                                            SPLITINCOME = tt.splitincome,
                                        };
                                        msc2List.Add(objMsc2);
                                    }
                                    if (msc2List.Count > 0)
                                    {
                                        gh.SM_SHAREDMSC2DETAIL = msc2List;
                                    }
                                }
                                repoMerchantMsc.Insert(obj);
                            }
                            else
                            {
                                gh.BATCHID = d.BATCHID;
                                gh.DOM_MSCSUBSIDY = d.DOM_MSCSUBSIDY;
                                gh.INT_MSCSUBSIDY = d.INT_MSCSUBSIDY;
                                gh.MCCMSC_ITBID = d.MCCMSC_ITBID;
                                gh.MERCHANTDOMCAP = d.MERCHANTDOMCAP;
                                gh.MERCHANTINTLCAP = d.MERCHANTINTLCAP;
                                gh.MERCHANTID = d.MERCHANTID;
                                gh.DOM_MSC2 = d.DOM_MSC2;
                                gh.INT_MSC2 = d.INT_MSC2;
                                gh.INT_UNSHAREDMSC = d.INT_UNSHAREDMSC;
                                gh.DOM_UNSHAREDMSC = d.DOM_UNSHAREDMSC;
                                gh.LAST_MODIFIED_UID = user_Id;
                                gh.DOM_MSC1 = d.DOM_MSC1;
                                gh.DOM_MSC2CAP = d.DOM_MSC2CAP;
                                gh.DOM_UNSHAREDCAP = d.DOM_UNSHAREDCAP;
                                gh.INT_MSC1 = d.INT_MSC1;
                                gh.INT_MSC2CAP = d.INT_MSC2CAP;
                                gh.INT_UNSHAREDCAP = d.INT_UNSHAREDCAP;
                                gh.AMOUNTDUEMERCH_PERC = d.AMOUNTDUEMERCH_PERC;
                                gh.AMOUNTDUEMERCH_TYPE = d.AMOUNTDUEMERCH_TYPE;
                                gh.APPLYMERCHANTSHARING = d.APPLYMERCHANTSHARING;
                                gh.MCC = d.MCC;
                                gh.CBN_CODE = d.CBN_CODE;
                                gh.CARDSCHEME = d.CARDSCHEME;
                                gh.MSC_CALCBASIS = d.MSC_CALCBASIS;
                                gh.INTMSC_CALCBASIS = d.INTMSC_CALCBASIS;
                                gh.CHANNEL = d.CHANNEL;
                                if (dmsc.Count != 0)
                                {
                                    var frst = dmsc[0];
                                    var midd = frst.MERCHANTID;
                                    var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == midd).ToList();
                                    foreach (var tt in existRec)
                                    {
                                        repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                                    }
                                    List<SM_SHAREDMSC2DETAIL> msc2List = new List<SM_SHAREDMSC2DETAIL>();
                                    foreach (var tt in dmsc)
                                    {
                                        var objMsc2 = new SM_SHAREDMSC2DETAIL()
                                        {
                                            CREATEDATE = tt.CREATEDATE,
                                            MERCHANTID = tt.MERCHANTID,
                                            PARTYITBID = tt.PARTYITBID,
                                            PARTY_LOCATOR = tt.PARTY_LOCATOR,
                                            SHARINGVALUE = tt.SHARINGVALUE,
                                            STATUS = active,
                                            TRANTYPE = tt.TRANTYPE,
                                            USERID = tt.USERID,
                                            CARDSCHEME = tt.CARDSCHEME,
                                            BANKACCOUNT = tt.BANKACCOUNT,
                                            BANKCODE = tt.BANKCODE,
                                            MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID,
                                            CAP = tt.CAP,
                                            ACCOUNT_ID = tt.ACCOUNT_ID,
                                            ACCOUNT_ID2 = tt.ACCOUNT_ID2,
                                            sharingRateAccount1 = tt.sharingRateAccount1,
                                            sharingRateAccount2 = tt.sharingRateAccount2,
                                            SPLITINCOME = tt.splitincome,
                                        };
                                        msc2List.Add(objMsc2);
                                    }
                                    if (msc2List.Count > 0)
                                    {
                                        gh.SM_SHAREDMSC2DETAIL = msc2List;
                                    }
                                }
                            }
                        }
                    }
                    //string mid = "";
                    //var dmsc = repoSharedMsc2Temp.AllEager(e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
                    //if (dmsc.Count != 0)
                    //{
                    //    var frst = dmsc[0];
                    //    mid = frst.MERCHANTID;
                    //    var existRec = repoSharedMsc2.AllEager(f => f.MERCHANTID == mid).ToList();

                    //    List<decimal> exitItbid = new List<decimal>();
                    //    foreach (var tt in dmsc)
                    //    {
                    //        var rec = existRec.FirstOrDefault(d => d.ITBID == tt.RECORDID && d.MERCHANTID == tt.MERCHANTID);
                    //        if (rec == null)
                    //        {
                    //            var objMsc2 = new SM_SHAREDMSC2DETAIL()
                    //            {
                    //                CREATEDATE = tt.CREATEDATE,
                    //                MERCHANTID = tt.MERCHANTID,
                    //                PARTYITBID = tt.PARTYITBID,
                    //                PARTY_LOCATOR = tt.PARTY_LOCATOR,
                    //                SHARINGVALUE = tt.SHARINGVALUE,
                    //                STATUS = active,
                    //                TRANTYPE = tt.TRANTYPE,
                    //                USERID = tt.USERID,
                    //                CARDSCHEME = tt.CARDSCHEME,
                    //                BANKACCOUNT = tt.BANKACCOUNT,
                    //                BANKCODE = tt.BANKCODE,
                    //                MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID
                    //            };
                    //            repoSharedMsc2.Insert(objMsc2);
                    //        }
                    //        else
                    //        {
                    //            exitItbid.Add(rec.ITBID);
                    //            rec.PARTYITBID = tt.PARTYITBID;
                    //            rec.PARTY_LOCATOR = tt.PARTY_LOCATOR;
                    //            rec.SHARINGVALUE = tt.SHARINGVALUE;
                    //            rec.STATUS = active;
                    //            rec.TRANTYPE = tt.TRANTYPE;
                    //            rec.LASTMODIFIED_UID = tt.USERID;
                    //            rec.CARDSCHEME = tt.CARDSCHEME;
                    //            rec.BANKCODE = tt.BANKCODE;
                    //            rec.BANKACCOUNT = tt.BANKACCOUNT;
                    //            rec.MERCHANTMSC_ITBID = tt.MERCHANTMSC_ITBID;
                    //        }
                    //    }
                    //    var gh = existRec.Where(f => !exitItbid.Contains(f.ITBID)).ToList();
                    //    foreach (var tt in gh)
                    //    {
                    //        repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    }
                    //}

                //u

                    //mid = "";
                    //var dmsc2 = repoUnSharedMsc1Temp.AllEager(null, e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
                    //if (dmsc2.Count != 0)
                    //{
                    //    var frst = dmsc2[0];
                    //    mid = frst.MERCHANTID;
                    //    var existRec = repoUnSharedMsc1.AllEager(null, f => f.MERCHANTID == mid).ToList();
                    //    //foreach (var tt in existRec)
                    //    //{
                    //    //    repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    //}
                    //    List<long> exitItbid = new List<long>();
                    //    foreach (var tt in dmsc2)
                    //    {
                    //        var rec = existRec.FirstOrDefault(d => d.ITBID == tt.RECORDID && d.MERCHANTID == tt.MERCHANTID);
                    //        if (rec == null)
                    //        {
                    //            var objMsc2 = new POSMISDB_UNSHAREDMSC1DETAIL()
                    //            {
                    //                CREATEDATE = tt.CREATEDATE,
                    //                MERCHANTID = tt.MERCHANTID,
                    //                PARTYITBID = tt.PARTYITBID,
                    //                PARTY_LOCATOR = tt.PARTY_LOCATOR,
                    //                SHARINGVALUE = tt.SHARINGVALUE,
                    //                STATUS = active,
                    //                TRANTYPE = tt.TRANTYPE,
                    //                USERID = tt.USERID,
                    //                CARDSCHEME = tt.CARDSCHEME

                    //            };

                    //            repoUnSharedMsc1.Insert(objMsc2);
                    //        }
                    //        else
                    //        {
                    //            exitItbid.Add(rec.ITBID);
                    //            rec.PARTYITBID = tt.PARTYITBID;
                    //            rec.PARTY_LOCATOR = tt.PARTY_LOCATOR;
                    //            rec.SHARINGVALUE = tt.SHARINGVALUE;
                    //            rec.STATUS = active;
                    //            rec.TRANTYPE = tt.TRANTYPE;
                    //            rec.LASTMODIFIED_UID = tt.USERID;
                    //            rec.CARDSCHEME = tt.CARDSCHEME;
                    //        }
                    //    }

                    //    var gh = existRec.Where(f => !exitItbid.Contains(f.ITBID)).ToList();
                    //    foreach (var tt in gh)
                    //    {
                    //        repoUnSharedMsc1.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    }
                    //}
                    //mid = "";
                    //var dsub = repoSubTemp.AllEager(null, e => e.BATCHID == batchid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).ToList();
                    //if (dsub.Count != 0)
                    //{
                    //    var frst = dsub[0];
                    //    mid = frst.MERCHANTID;
                    //    var existRec = repoSub.AllEager(null, f => f.MERCHANTID == mid).ToList();
                    //    //foreach (var tt in existRec)
                    //    //{
                    //    //    repoSharedMsc2.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    //}
                    //    List<long> exitItbid = new List<long>();
                    //    foreach (var tt in dsub)
                    //    {
                    //        var rec = existRec.FirstOrDefault(d => d.ITBID == tt.RECORDID && d.MERCHANTID == tt.MERCHANTID);
                    //        if (rec == null)
                    //        {
                    //            var objMsc2 = new POSMISDB_SUBSIDYDETAIL()
                    //            {
                    //                CREATEDATE = tt.CREATEDATE,
                    //                MERCHANTID = tt.MERCHANTID,
                    //                PARTYITBID = tt.PARTYITBID,
                    //                PARTY_LOCATOR = tt.PARTY_LOCATOR,
                    //                SHARINGVALUE = tt.SHARINGVALUE,
                    //                STATUS = active,
                    //                TRANTYPE = tt.TRANTYPE,
                    //                USERID = tt.USERID,
                    //                CARDSCHEME = tt.CARDSCHEME

                    //            };

                    //            repoSub.Insert(objMsc2);
                    //        }
                    //        else
                    //        {
                    //            exitItbid.Add(rec.ITBID);
                    //            rec.PARTYITBID = tt.PARTYITBID;
                    //            rec.PARTY_LOCATOR = tt.PARTY_LOCATOR;
                    //            rec.SHARINGVALUE = tt.SHARINGVALUE;
                    //            rec.STATUS = active;
                    //            rec.TRANTYPE = tt.TRANTYPE;
                    //            rec.LASTMODIFIED_UID = tt.USERID;
                    //            rec.CARDSCHEME = tt.CARDSCHEME;
                    //        }
                    //    }

                    //    var gh = existRec.Where(f => !exitItbid.Contains(f.ITBID)).ToList();
                    //    foreach (var tt in gh)
                    //    {
                    //        repoSub.Delete(tt.ITBID);  // delete existing record for merchant id...merchantid is not expected to have any record since the event is new.
                    //    }
                    //}
                }
            }

            return true;
        }

        private void CreateMerchantAcct(string batchid,string user_id)
        {
            if (!string.IsNullOrEmpty(batchid))
            {
                var lst = repoMAcctTemp.AllEager(f => f.BATCHID == batchid && f.USERID == user_id).ToList();
                if (lst.Count() != 0)
                {
                    foreach (var t in lst)
                    {
                        t.STATUS = approve;
                        var dm2 = repoMAcct.AllEager(d => d.DEPOSIT_ACCOUNTNO == t.DEPOSIT_ACCOUNTNO && d.DEPOSIT_BANKCODE == t.DEPOSIT_BANKCODE && d.MERCHANTID == t.MERCHANTID).FirstOrDefault();
                        if (dm2 == null)
                        {
                            var obj = new SM_MERCHANTACCT()
                            {
                                DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO,
                                DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS,
                                DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE,
                                DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME,
                                DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE,
                                DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT,
                                SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY,
                                STATUS = active,
                                CREATEDATE = DateTime.Now,
                                USERID = user_id,
                                DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME,
                                MERCHANTID = t.MERCHANTID,
                                BATCHID = t.BATCHID,
                            };
                            repoMAcct.Insert(obj);
                        }
                        else
                        {
                            dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                            dm2.DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS;
                            dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                            dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                            dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                            dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                            dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                            dm2.LAST_MODIFIED_UID = user_id;
                            dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                            dm2.MERCHANTID = t.MERCHANTID;
                           // dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                        }
                    }
                }
            }
        }

        private bool ModifyMerchantTerminal(string batchId, string _userId)
        {
            var ac = repoTTemp.AllEager(e => e.BATCHID == batchId && e.USERID == _userId).ToList();
            if (ac.Count > 0)
            {
                foreach (var t in ac)
                {
                    t.STATUS = approve;
                    if (t.EVENTTYPE == eventEdit)
                    {
                        var dm2 = repoT.AllEager(e => e.TERMINALID == t.TERMINALID).FirstOrDefault();
                        if (dm2 != null)
                        {
                            dm2.ACCOUNT_ID = t.ACCOUNT_ID;
                            dm2.EMAIL_ALERTS = t.EMAIL_ALERTS;
                            dm2.MASTACQUIRERIDNO = t.MASTACQUIRERIDNO;
                            dm2.PTSA = t.PTSA;
                            dm2.PTSP = t.PTSP;
                            dm2.SETTLEMENT_CURRENCY = t.SETTLEMENT_CURRENCY;
                            dm2.SETTLEMENT_FREQUENCY = t.SETTLEMENT_FREQUENCY;
                            dm2.LAST_MODIFIED_UID = _userId;
                            dm2.SLIP_FOOTER = t.SLIP_FOOTER;
                            dm2.SLIP_HEADER = t.SLIP_HEADER;
                            dm2.TERMINALMODEL_CODE = t.TERMINALMODEL_CODE;
                            dm2.TERMINALOWNER_CODE = t.TERMINALOWNER_CODE;
                            dm2.VERVACQUIRERIDNO = t.VERVACQUIRERIDNO;
                            dm2.VISAACQUIRERIDNO = t.VISAACQUIRERIDNO;
                            
                            //dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                        }
                    }
                    else if (t.EVENTTYPE == eventInsert)
                    {
                        //var rep = repoMAcct.AllEager(e => e.DEPOSIT_ACCOUNTNO == t.DEPOSIT_ACCOUNTNO && e.DEPOSIT_BANKCODE == t.DEPOSIT_BANKCODE && e.MERCHANTID == t.MERCHANTID).FirstOrDefault();
                        //if (rep == null)
                        //{
                        //    var obj2 = new SM_MERCHANTACCT()
                        //    {
                        //        DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO,
                        //        DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS,
                        //        DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE,
                        //        DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME,
                        //        DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE,
                        //        DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT,
                        //        SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY,
                        //        STATUS = active,
                        //        CREATEDATE = DateTime.Now,
                        //        USERID = _userId,
                        //        DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME,
                        //        MERCHANTID = t.MERCHANTID,
                        //        BATCHID = t.BATCHID,

                        //    };
                        //    repoMAcct.Insert(obj2);
                        //}
                        //else
                        //{
                        //    var dm2 = rep; // repoMAcct.AllEager(null,e => e.ITBID == t.RECORDID).FirstOrDefault();
                        //    if (dm2 != null)
                        //    {
                        //        dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                        //        dm2.DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS;
                        //        dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                        //        dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                        //        dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                        //        dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                        //        dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                        //        dm2.LAST_MODIFIED_UID = _userId;
                        //        dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                        //        dm2.MERCHANTID = t.MERCHANTID;
                        //        dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                        //    };
                        //}
                    }
                    else
                    {
                        //var ext = repoMAcct.Find(t.RECORDID);
                        //if (ext != null)
                        //{
                        //    repoMAcct.Delete(t.RECORDID);
                        //}
                    }

                }
                return true;
            }
            return false;
        }
        private bool ModifyMerchantAcct(string batchId, string _userId)
        {
            var ac = repoMAcctTemp.AllEager(e => e.BATCHID == batchId && e.USERID == _userId).ToList();
            if (ac.Count > 0)
            {
                foreach (var t in ac)
                {
                    t.STATUS = approve;
                    //if (t.DEFAULT_ACCOUNT)
                    //{
                    //    SetPreviousToFalse(t.MERCHANTID);
                    //}
                    if (t.EVENTTYPE == eventEdit)
                    {
                        var dm2 = repoMAcct.AllEager(e => e.ITBID == t.RECORDID).FirstOrDefault();
                        if (dm2 != null)
                        {
                            dm2.DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO;
                            dm2.DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS;
                            dm2.DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE;
                            dm2.DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME;
                            dm2.DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE;
                            dm2.DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT;
                            dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                            dm2.LAST_MODIFIED_UID = _userId;
                            dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                            dm2.MERCHANTID = t.MERCHANTID;
                            dm2.DRACCOUNTNAME = t.DRACCOUNTNAME;
                            dm2.DRACCOUNTNO = t.DRACCOUNTNO;
                            dm2.DRBANKCODE = t.DRBANKCODE;
                            //dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                        }
                    }
                    else if (t.EVENTTYPE == eventInsert)
                    {
                        var rep = repoMAcct.AllEager(e => e.DEPOSIT_ACCOUNTNO == t.DEPOSIT_ACCOUNTNO && e.DEPOSIT_BANKCODE == t.DEPOSIT_BANKCODE && e.MERCHANTID == t.MERCHANTID).FirstOrDefault();
                        if (rep == null)
                        {
                            var obj2 = new SM_MERCHANTACCT()
                            {
                                DEPOSIT_ACCOUNTNO = t.DEPOSIT_ACCOUNTNO,
                                DEPOSIT_BANKADDRESS = t.DEPOSIT_BANKADDRESS,
                                DEPOSIT_BANKCODE = t.DEPOSIT_BANKCODE,
                                DEPOSIT_BANKNAME = t.DEPOSIT_BANKNAME,
                                DEPOSIT_COUNTRYCODE = t.DEPOSIT_COUNTRYCODE,
                                DEFAULT_ACCOUNT = t.DEFAULT_ACCOUNT,
                                SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY,
                                STATUS = active,
                                CREATEDATE = DateTime.Now,
                                USERID = _userId,
                                DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME,
                                MERCHANTID = t.MERCHANTID,
                                BATCHID = t.BATCHID,
                                DRACCOUNTNAME = t.DRACCOUNTNAME,
                                DRACCOUNTNO = t.DRACCOUNTNO,
                                DRBANKCODE = t.DRBANKCODE,

                            };
                            repoMAcct.Insert(obj2);
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
                                dm2.SETTLEMENTCURRENCY = t.SETTLEMENTCURRENCY;
                                dm2.LAST_MODIFIED_UID = _userId;
                                dm2.DEPOSIT_ACCTNAME = t.DEPOSIT_ACCTNAME;
                                dm2.MERCHANTID = t.MERCHANTID;
                                dm2.STATUS = t.EVENTTYPE == "Deleted" ? eventDelete : dm2.STATUS;
                                dm2.DRACCOUNTNAME = t.DRACCOUNTNAME;
                                dm2.DRACCOUNTNO = t.DRACCOUNTNO;
                                dm2.DRBANKCODE = t.DRBANKCODE;
                            };
                        }
                    }
                    else
                    {
                        var ext = repoMAcct.Find(t.RECORDID);
                        if (ext != null)
                        {
                            repoMAcct.Delete(t.RECORDID);
                        }
                    }

                }
                return true;
            }
            return false;
        }
        private bool CreateNewRecord(decimal rid, string user_Id)
        {
            var d = repoMTemp.AllEager(e => e.ITBID == rid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).FirstOrDefault();
            if (d != null)
            {
                SM_MERCHANTDETAIL BType = new SM_MERCHANTDETAIL()
                {
                    MERCHANTID = d.MERCHANTID,
                    MERCHANTNAME = d.MERCHANTNAME,
                    CONTACTTITLE = d.CONTACTTITLE,
                    CONTACTNAME = d.CONTACTNAME,
                    EMAIL = d.EMAIL,
                    PHONENO = d.PHONENO,
                    ADDRESS = d.ADDRESS,
                    BUSINESS_CODE = d.BUSINESS_CODE,
                    OLD_MID = d.OLD_MID,
                    MCC_CODE = d.MCC_CODE,
                    COUNTRY_CODE = d.COUNTRY_CODE,
                    STATE_CODE = d.STATE_CODE,
                    CITY_NAME = d.CITY_NAME,
                   // ACCEPTANCE_TYPE = d.ACCEPTANCE_TYPE,
                    //TMS_OWNERCODE = d.TMS_OWNERCODE,
                    CREATEDATE = DateTime.Now,
                    USERID = d.USERID,
                    STATUS = active,
                    LAST_MODIFIED_AUTHID = User.Identity.Name,
                    INSTITUTION_CBNCODE = d.INSTITUTION_CBNCODE,
                    LAST_MODIFIED_DATE = DateTime.Now,
                    LAST_MODIFIED_UID = d.USERID,
                };
                repoM.Insert(BType);
                return true;
            }
            return false;
        }

        private bool ModifyMainRecord(decimal rid, string user_Id)
        {
            var rec = repoMTemp.AllEager(e => e.ITBID == rid && e.USERID != null && e.USERID.ToUpper() == user_Id.ToUpper() && e.STATUS != null && e.STATUS.ToLower() == open.ToLower()).FirstOrDefault();
            if (rec != null)
            {
                var d = repoM.AllEager(e => e.ITBID == rec.RECORDID).SingleOrDefault();
                if (d != null)
                {
                    //d.MERCHANTID = rec.MERCHANTID;
                    d.MERCHANTNAME = rec.MERCHANTNAME;
                    d.CONTACTTITLE = rec.CONTACTTITLE;
                    d.CONTACTNAME = rec.CONTACTNAME;
                    d.EMAIL = rec.EMAIL;
                    d.PHONENO = rec.PHONENO;
                    d.ADDRESS = rec.ADDRESS;
                    d.BUSINESS_CODE = rec.BUSINESS_CODE;
                    d.OLD_MID = rec.OLD_MID;
                    d.MCC_CODE = rec.MCC_CODE;
                    d.COUNTRY_CODE = rec.COUNTRY_CODE;
                    d.STATE_CODE = rec.STATE_CODE;
                    d.CITY_NAME = rec.CITY_NAME;
                    //d.ACCEPTANCE_TYPE = rec.ACCEPTANCE_TYPE;
                    //  d.TMS_OWNERCODE = rec.TMS_OWNERCODE;
                    d.CREATEDATE = DateTime.Now;
                    d.USERID = rec.USERID;
                    d.INSTITUTION_CBNCODE = rec.INSTITUTION_CBNCODE;
                    d.LAST_MODIFIED_UID = rec.USERID;
                    d.COLLECTION = rec.COLLECTION;
                    d.STATUS = active;
                    return true;
                }
                
            }
            return false;
        }

        bool sucTrue;
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
                    //int recordId = 0;
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
                                //recordId = rec2.RECORDID.GetValueOrDefault();
                                menuId = rec2.MENUID.GetValueOrDefault();
                                var sp = rec2.BATCHID.Split('_');
                                var frmType = sp[0];
                                switch (frmType)
                                {
                                    case "MMD":
                                        {
                                            var sig = repoMTemp.AllEager(h => h.BATCHID == rec2.BATCHID && h.USERID == rec2.USERID).FirstOrDefault();
                                            if(sig != null)
                                            {
                                                sig.STATUS = reject;
                                            }
                                            break;
                                        }
                                    case "MMA":
                                        {
                                            var sig = repoMAcctTemp.AllEager(h => h.BATCHID == rec2.BATCHID && h.USERID == rec2.USERID).ToList();
                                            foreach (var h in sig)
                                            {
                                                h.STATUS = reject;
                                            }
                                            break;
                                        }
                                    case "MSC":
                                        {
                                            var sig = repoMerchantMscTemp.AllEager(h => h.BATCHID == rec2.BATCHID && h.USERID == rec2.USERID).ToList();
                                            foreach (var h in sig)
                                            {
                                                h.STATUS = reject;
                                            }
                                            break;
                                        }
                                    case "MT":
                                        {
                                            var sig = repoMTemp.AllEager(h => h.BATCHID == rec2.BATCHID && h.USERID == rec2.USERID).ToList();
                                            foreach (var h in sig)
                                            {
                                                h.STATUS = reject;
                                            }
                                            break;
                                        }
                                }

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    sucTrue = true;
                                   // txscope.Complete();
                                }

                            }
                            }
                        }
                       
                    //}
                if (sucTrue)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Merchant Update Record", null, fullName);
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