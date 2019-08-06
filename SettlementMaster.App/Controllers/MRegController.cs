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
    public class MRegController : Controller
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
        int menuId = 35;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        string fullName;
        string deptCode;
        // GET: Roles
        public MRegController()
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
        public async Task<ActionResult> GetMerchantDetail(string id)
        {
            var rec = await _repo.GetMerchantByMidAsync(id, "", null);
            if(rec != null && rec.Count > 0)
            {
                return Json(new { data = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new {  RespCode = 1, RespMessage = "Merchant Not Found" }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult GetAcctDetail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var splt = id.Split('|');
                if (splt != null && splt.Length > 0)
                {
                    var tp = splt[1];
                    if (tp == "M")
                    {
                        var acctId = decimal.Parse(splt[0]);
                        // get account detail from merchant acct
                        var rec = repoMAcct.Find(acctId);
                        if (rec != null)
                        {
                            var obj = new { DEPOSIT_ACCOUNTNO = rec.DEPOSIT_ACCOUNTNO, DEPOSIT_ACCTNAME = rec.DEPOSIT_ACCTNAME, DEPOSIT_BANKCODE = rec.DEPOSIT_BANKCODE , DEPOSIT_BANKNAME = rec.DEPOSIT_BANKNAME};
                            return Json(new { data = obj, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    else
                    {
                        var rv = new MAcctSession();
                        var rec = rv.FindMerchantAcct(splt[0],User.Identity.Name);
                        if (rec != null)
                        {
                            var obj = new { DEPOSIT_ACCOUNTNO = rec.DEPOSIT_ACCOUNTNO, DEPOSIT_ACCTNAME = rec.DEPOSIT_ACCTNAME, DEPOSIT_BANKCODE = rec.DEPOSIT_BANKCODE, DEPOSIT_BANKNAME = rec.DEPOSIT_BANKNAME };
                            return Json(new { data = obj, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
            }
            return Json(new { RespCode = 1, RespMessage = "Account Not Found" }, JsonRequestBehavior.AllowGet);


        }
        [MyAuthorize]
        // GET: MReg
        public ActionResult Add()
        {
            try
            {
                var rv = new TidSession();
                var rvM = new MAcctSession();
                rv.PurgeTerminal(User.Identity.Name);
                rvM.PurgeMerchantAcct(User.Identity.Name);
                //SessionHelper.GetMerchantAcct(Session).Clear();
                //ViewBag.MenuId = HttpUtility.UrlDecode(m);

                BindCombo();
                BindState("NGN");
                //BindCity();
                return View("Add", new mRegObj());
            }
            catch (Exception ex)
            {
                //var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                //return Json(obj1, JsonRequestBehavior.AllowGet);
                ViewBag.Message = ex.Message;
                return View("Index", new { m = ViewBag.MenuId });
            }
        }
        void BindCombo(string acq_selected = null)
        {
            var acq = _repo.GetInstitution(0, true, "Active");
            var mcc = _repo.GetMCC(0, true, "Active").Select(d=> new {d.MCC_CODE , MCC_DESC= string.Concat(d.MCC_CODE,"-", d.MCC_DESC) });
            var country = _repo.GetCountry(0, true, "Active");

            ViewBag.AcquirerList = new SelectList(acq, "CBN_CODE", "INSTITUTION_NAME");
            ViewBag.CountryList = new SelectList(country, "COUNTRY_CODE", "COUNTRY_NAME");
            ViewBag.MCCList = new SelectList(mcc, "MCC_CODE", "MCC_DESC");

            var sta = SmartObj.GetStatus();
            ViewBag.RecordStatus = new SelectList(sta, "Code", "Description");

        }
        void BindState(string countryCode)
        {
            var rec = _repo.GetState(0, true, "Active", false, countryCode);
            ViewBag.StateList = new SelectList(rec, "STATECODE", "STATENAME");

        }
        #region Merchant Account
        void BindComboAcct()
        {
            var bankList = _repo.GetInstitution(0, true, "Active").Where(f => f.IS_BANK == "Y").ToList();
            var country = _repo.GetCurrency(0, true, "Active");

            ViewBag.BankList = new SelectList(bankList, "CBN_CODE", "INSTITUTION_NAME");
            ViewBag.CurrencyList = new SelectList(country, "CURRENCY_CODE", "CURRENCY_NAME");
          

        }
        public PartialViewResult AddAcct()
        {
            try
            {
                    BindComboAcct();
             
                    ViewBag.HeaderTitle = "Add Account";
                    ViewBag.ButtonText = "Add";
                    return PartialView("_AddAcct", new MerchantAcctObj());
                    // return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
               
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
        public ActionResult AddAcct(MerchantAcctObj model)
        {
            var rv = new MAcctSession();
            string msg = "";
            try
            {
                model.DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO != null ? model.DEPOSIT_ACCOUNTNO.Trim() : model.DEPOSIT_ACCOUNTNO;
                model.DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME != null ? model.DEPOSIT_ACCTNAME.Trim() : model.DEPOSIT_ACCTNAME;
                

                if (string.IsNullOrEmpty(model.PID) && model.DB_ITBID.GetValueOrDefault() <= 0)
                {
                    var obj = new MerchantAcctObj()
                    {
                        DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_BANKADDRESS = model.DEPOSIT_BANKADDRESS,
                        DEPOSIT_BANKNAME = model.DEPOSIT_BANKNAME,
                        DEPOSIT_COUNTRYCODE = model.DEPOSIT_COUNTRYCODE,
                        DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                        SETTLEMENTCURRENCY = model.SETTLEMENTCURRENCY,
                        CURRENCYDESC = model.CURRENCYDESC,
                        DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                        DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                        USERID = User.Identity.Name,
                        EVENTTYPE = eventInsert,
                        PID = model.PID,
                        MERCHANTID = model.MERCHANTID,
                        
                    };
                    //SessionHelper.GetRvHead(Session).AddItem(obj);
                    var rst = rv.PostMAcct(obj, 1);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    var w = rv.GetMerchantAcct(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewMerchantAcct", w).RenderToString();
                    msg = "Record Added to List";
                    return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var obj = new MerchantAcctObj()
                    {
                        DEPOSIT_ACCOUNTNO = model.DEPOSIT_ACCOUNTNO,
                        DEPOSIT_BANKADDRESS = model.DEPOSIT_BANKADDRESS,
                        DEPOSIT_BANKNAME = model.DEPOSIT_BANKNAME,
                        DEPOSIT_COUNTRYCODE = model.DEPOSIT_COUNTRYCODE,
                        DEFAULT_ACCOUNT = model.DEFAULT_ACCOUNT,
                        SETTLEMENTCURRENCY = model.SETTLEMENTCURRENCY,
                        CURRENCYDESC = model.CURRENCYDESC,
                        DEPOSIT_BANKCODE = model.DEPOSIT_BANKCODE,
                        DEPOSIT_ACCTNAME = model.DEPOSIT_ACCTNAME,
                        USERID = User.Identity.Name,
                        DB_ITBID = model.DB_ITBID,
                        PID = model.PID,
                        EVENTTYPE = model.DB_ITBID > 0 ? eventEdit : eventInsert,
                        MERCHANTID = model.MERCHANTID,
                    };
                    OutPutObj rst;

                    rst = rv.PostMAcct(obj, 2);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    }
                    var w = rv.GetMerchantAcct(User.Identity.Name); // GetRvHeadLines().ToList();
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
        public ActionResult EditAcct(string id)
        {
            try
            {

                var rv = new MAcctSession();
                var rec = rv.FindMerchantAcct(id, User.Identity.Name);
                if (rec != null)
                {

                    BindComboAcct();
                    ViewBag.ButtonText = "Update";
                    ViewBag.HeaderTitle = "Edit Account";
                    return PartialView("_AddAcct", rec);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public ActionResult DeleteAcct(string id)
        {
            var rv = new MAcctSession();
            try
            {
                rv.DeleteMerchantAcct(id, User.Identity.Name);

                var lst2 = rv.GetMerchantAcct(User.Identity.Name); // GetRvHeadLines().ToList();
                var html2 = PartialView("_ViewMerchantAcct", lst2).RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var lst3 = rv.GetMerchantAcct(User.Identity.Name); //  GetRvHeadLines().ToList();
                var html = PartialView("_ViewMerchantAcct", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion merchant account
        #region Merchant Terminal
        void BindComboTerm(string mid)
        {
            var bankList = _repo.GetMerchantAcct_Merge(mid,User.Identity.Name);
            var freq = _repo.GetFrequency(0, true, "Active");
            var currency = _repo.GetCurrency(0, true, "Active");

            ViewBag.AccountList = new SelectList(bankList, "Code", "Description");

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
                var obj = new TerminalObj() { MERCHANTID = id };
                id = null;
                return PartialView("_AddTerminal", obj);

            }
            catch
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
            var rvM = new MAcctSession();
            bool isNewAcct = false;
            string msg = "";
            string mAcctNo = "", mAcctName = "", mBankName = "";
            try
            {
                model.PTSA = model.PTSA != null ? model.PTSA.Trim() : model.PTSA;
                model.PTSP = model.PTSP != null ? model.PTSP.Trim() : model.PTSP;
                model.TERMINALOWNER_CODE = model.TERMINALOWNER_CODE != null ? model.TERMINALOWNER_CODE.Trim() : model.TERMINALOWNER_CODE;
                model.VERVACQUIRERIDNO = model.VERVACQUIRERIDNO != null ? model.VERVACQUIRERIDNO.Trim() : model.VERVACQUIRERIDNO;
                model.MASTACQUIRERIDNO = model.MASTACQUIRERIDNO != null ? model.MASTACQUIRERIDNO.Trim() : model.MASTACQUIRERIDNO;
                model.VISAACQUIRERIDNO = model.VISAACQUIRERIDNO != null ? model.VISAACQUIRERIDNO.Trim() : model.VISAACQUIRERIDNO;
                model.TERMINALMODEL_CODE = model.TERMINALMODEL_CODE != null ? model.TERMINALMODEL_CODE.Trim() : model.TERMINALMODEL_CODE;
                decimal acctId = 0;
                var pid = "";
                if (model.ACCOUNTID != null)
                {
                    var splt = model.ACCOUNTID.Split('|');
                    if (splt != null && splt.Length > 0)
                    {
                        var tp = splt[1];
                        if (tp == "M")
                        {
                            acctId = decimal.Parse(splt[0]);
                           
                            var ma = repoMAcct.Find(acctId);
                            if (ma != null)
                            {
                                mAcctName = ma.DEPOSIT_ACCTNAME;
                                mAcctNo = ma.DEPOSIT_ACCOUNTNO;
                                mBankName = ma.DEPOSIT_BANKNAME;
                            }
                        }
                        else
                        {
                            
                            pid = splt[0];
                            var ma = rvM.FindMerchantAcct(pid, User.Identity.Name); // repoMAcct.Find(acctId);
                            if (ma != null)
                            {
                                mAcctName = ma.DEPOSIT_ACCTNAME;
                                mAcctNo = ma.DEPOSIT_ACCOUNTNO;
                                mBankName = ma.DEPOSIT_BANKNAME;
                                isNewAcct = true;
                            }
                        }
                    }
                }
                if (ValidateForm(model))
                {
                    msg = GetStringFromList(validationErrorMessage);
                    return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                }
               
                if (string.IsNullOrEmpty(model.PID) && model.DB_ITBID.GetValueOrDefault() <= 0)
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
                        ACCOUNTID = model.ACCOUNTID,
                        DEPOSIT_ACCOUNTNO = mAcctNo,
                        DEPOSIT_BANKNAME = mBankName,
                        DEPOSIT_ACCTNAME = mAcctName,
                        IS_NEWACCT = isNewAcct
                        
                    };
                    //SessionHelper.GetRvHead(Session).AddItem(obj);
                    var rst = rv.PostTerminal(obj, 1);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);

                    }
                    var w = rv.GetTerminal2(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewTerminalQueue2", w).RenderToString();
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
                        ACCOUNTID = model.ACCOUNTID,
                        DEPOSIT_ACCOUNTNO = mAcctNo,
                        DEPOSIT_BANKNAME = mBankName,
                        DEPOSIT_ACCTNAME = mAcctName,
                        IS_NEWACCT = isNewAcct
                    };
                    OutPutObj rst;

                    rst = rv.PostTerminal(obj, 2);
                    if (rst != null && rst.RespCode != 0)
                    {
                        msg = rst.RespMessage; // "Revenue Code Already exist. Duplicate Record is not allowed.";
                        return Json(new { RespCode = 1, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    }
                    var w = rv.GetTerminal2(User.Identity.Name); // GetRvHeadLines().ToList();
                    var html = PartialView("_ViewTerminalQueue2", w).RenderToString();
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
        ////[ValidateAntiForgeryToken]
        //public ActionResult EditMerchantTerminal()
        //{
        //    try
        //    {
        //        var rv = new TidSession();
        //        string bid = string.Concat("MT_", SmartObj.GenRefNo2());

        //        SaveTerminalTemp(eventEdit, bid, rv);
        //        //  var rst1 = new AuthListUtil().SaveLog(auth);
        //        var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
        //        if (rst)
        //        {
        //            SM_AUTHLIST auth = new SM_AUTHLIST()
        //            {
        //                CREATEDATE = DateTime.Now,
        //                EVENTTYPE = eventEdit,
        //                MENUID = menuId,
        //                //MENUNAME = "",
        //                // RECORDID =  setId,
        //                STATUS = open,
        //                // TABLENAME = "ADMIN_DEPARTMENT",
        //                URL = Request.FilePath,
        //                USERID = User.Identity.Name,
        //                BATCHID = bid,
        //                POSTTYPE = Single,
        //                INSTITUTION_ITBID = institutionId,
        //                // RECORDID = decimal.Parse(hidItbid.Value)
        //            };

        //            repoAuth.Insert(auth);
        //            var rst1 = uow.Save(User.Identity.Name) > 0 ? true : false;
        //            if (rst1)
        //            {
        //                var msg = "Record Updated SuccessFully...Awaiting Authorization";
        //                var rec = rv.PurgeTerminal(User.Identity.Name);
        //                var rec2 = rv.GetTerminal(User.Identity.Name);
        //                var html = PartialView("_ViewTerminalQueue2", rec2).RenderToString();
        //                return Json(new { data_html = html, RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { RespCode = 2, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);

        //    }
        //    return Json(new { RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        //}
        bool SaveTerminalTemp(string bid, DateTime curDate, TidSession rvT, string cbnCode)
        {

            var col = rvT.GetTerminal2(User.Identity.Name); // GetRvHeadLines();
            foreach (var d in col)
            {
                var code = d.TERMINALID.Substring(1, 3);
                if (code != cbnCode && cbnCode != "000")
                {
                    return false;
                }
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
                    DEPOSIT_ACCTNAME = d.DEPOSIT_ACCTNAME,
                    DEPOSIT_BANKNAME = d.DEPOSIT_BANKNAME,
                    DEPOSIT_ACCOUNTNO = d.DEPOSIT_ACCOUNTNO,
                    ACCOUNTID = d.ACCOUNTID,
                    IS_NEWACCT = d.IS_NEWACCT,


                };
                repoTTemp.Insert(obj);
            }
            return true;
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

                var lst2 = rv.GetTerminal2(User.Identity.Name); // GetRvHeadLines().ToList();
                var html2 = PartialView("_ViewTerminalQueue2", lst2).RenderToString();
                return Json(new { RespCode = 0, RespMessage = "", data_html = html2 }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var lst3 = rv.GetTerminal2(User.Identity.Name); //  GetRvHeadLines().ToList();
                var html = PartialView("_ViewTerminal", lst3).RenderToString();
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request.", data_html = html }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        bool sucTrue;
        [HttpPost]
        // [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Add(mRegObj model)
        {
            DateTime curDate = DateTime.Now;
            try
            {
                TidSession rvT = new TidSession();
                MAcctSession rvM = new MAcctSession();
                string bid = string.Concat("MT_", SmartObj.GenRefNo2());
                var obj = new SM_MERCHANTDETAILTEMP()
                {
                    ADDRESS = model.mObj.ADDRESS,
                    BATCHID = bid,
                    BUSINESS_CODE = model.mObj.BUSINESS_CODE,
                    CITY_NAME = model.mObj.CITY_NAME,
                    CONTACTNAME = model.mObj.CONTACTNAME,
                    CONTACTTITLE = model.mObj.CONTACTTITLE,
                    COUNTRY_CODE = model.mObj.COUNTRY_CODE,
                    CREATEDATE = curDate,
                    EMAIL = model.mObj.EMAIL,
                    MCC_CODE = model.mObj.MCC_CODE,
                    MERCHANTID = model.mObj.MERCHANTID,
                    MERCHANTNAME = model.mObj.MERCHANTNAME,
                    OLD_MID = model.mObj.OLD_MID,
                    INSTITUTION_CBNCODE = model.mObj.INSTITUTION_CBNCODE,
                    PHONENO = model.mObj.PHONENO,
                    SETTLEMENT_FREQUENCY = model.mObj.SETTLEMENT_FREQUENCY,
                    STATE_CODE = model.mObj.STATE_CODE,
                    STATUS = open,
                    USERID = User.Identity.Name,
                };
                repoMTemp.Insert(obj);
                SaveAcctDetailTemp(model.mObj.MERCHANTID, bid,curDate);
                var suc = SaveTerminalTemp(bid,curDate,rvT,model.mObj.INSTITUTION_CBNCODE);
                if (!suc)
                {
                    return Json(new { RespCode = 2, RespMessage = "Some Terminal Id cannot be registered for the selected Acquiring Institution." }, JsonRequestBehavior.AllowGet);

                }
                //  var rst1 = new AuthListUtil().SaveLog(auth);
                var rst = uow.Save(User.Identity.Name) > 0 ? true : false;
                if (rst)
                {
                    SM_AUTHLIST auth = new SM_AUTHLIST()
                    {
                        CREATEDATE = DateTime.Now,
                        EVENTTYPE = eventInsert,
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
                        rvT.PurgeTerminal(User.Identity.Name);
                        rvM.PurgeMerchantAcct(User.Identity.Name);
                        EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, "New Merchant Record");
                        var msg = "Record Created SuccessFully...Awaiting Authorization";
                        return Json(new { RespCode = 0, RespMessage = msg }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 2, RespMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { RespCode = 0, RespMessage = "" }, JsonRequestBehavior.AllowGet);

        }

        void SaveAcctDetailTemp(string mid, string bid,DateTime curDate)
        {
            var rv = new MAcctSession();
            var col = rv.GetMerchantAcct(User.Identity.Name); //  GetMerchantAcctLines();
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
                    PID = d.PID,
                    
                    // RECORDID = d.RECORDID,
                };
                repoMAcctTemp.Insert(obj);
            }
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
                        
                        ViewBag.HeaderTitle = "Authorize Detail for Single Merchant Registeration";

                        var rec = _repo.GetMerchantDetailFromTemp(det.BATCHID, det.USERID);
                        if (rec != null)
                        {
                            BindCombo();
                            BindState(rec.COUNTRY_CODE);
                            var model = new mRegObj(); //  rec.FirstOrDefault();
                            var recMA = _repo.GetMerchantAcctTemp(det.BATCHID, det.USERID);
                          
                            var recMT = _repo.GetMerchantTerminalTemp(det.BATCHID, det.USERID,2);
                            model.mObj = rec;
                            model.mAcctObj = recMA;
                            model.mTObj = recMT;
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = rec.CREATED_BY;
                            ViewBag.Auth = obj;
                            ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                            return View("DetailAuth", model);
                        }
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
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 5, 0)))
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
                                //recordId = (int)rec2.RECORDID.GetValueOrDefault();
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                        {
                                            suc = ProcessNewMReg(rec2.BATCHID, rec2.USERID);

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
                                        sucTrue = true;
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
                            if (sucTrue)
                            {
                                EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Merchant Update Record", null, fullName);
                                respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                                TempData["msg"] = respMsg;
                                TempData["status"] = approve;
                                return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                            }
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

        private bool ProcessNewMReg(string bATCHID, string uSERID)
        {
            var isNewMid = false;
            SM_MERCHANTDETAIL mm = new SM_MERCHANTDETAIL();
            var mD = repoMTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if(mD != null)
            {
                var mObj = repoM.Find(mD.MERCHANTID);
                if(mObj == null)
                {
                    mm = new SM_MERCHANTDETAIL()
                    {
                        ADDRESS = mD.ADDRESS,
                        MERCHANTID = mD.MERCHANTID,
                        BATCHID = bATCHID,
                        BUSINESS_CODE = mD.BUSINESS_CODE,
                        CITY_NAME = mD.CITY_NAME,
                        CONTACTNAME = mD.CONTACTNAME,
                        CONTACTTITLE = mD.CONTACTTITLE,
                        COUNTRY_CODE = mD.COUNTRY_CODE,
                        CREATEDATE = DateTime.Now,
                        EMAIL = mD.EMAIL,
                        INSTITUTION_CBNCODE = mD.INSTITUTION_CBNCODE,
                        MCC_CODE = mD.MCC_CODE,
                        MERCHANTNAME = mD.MERCHANTNAME,
                        MERCHANT_URL = mD.MERCHANTNAME,
                        OLD_MID = mD.OLD_MID,
                        PHONENO = mD.PHONENO,
                        SETTLEMENT_FREQUENCY = mD.SETTLEMENT_FREQUENCY,
                        STATE_CODE = mD.STATE_CODE,
                        STATUS = active,
                        USERID = mD.USERID,
                    };
                    repoM.Insert(mm);
                    isNewMid = true;
                }
                List<SM_MERCHANTTERMINALUPLD> tstAcct = new List<SM_MERCHANTTERMINALUPLD>();

                var mtList = repoTTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
                foreach (var h in mtList)
                {
                    decimal acctId = 0;
                    var pid = "";
                    if (h.ACCOUNTID != null)
                    {
                        var splt = h.ACCOUNTID.Split('|');
                        if (splt != null && splt.Length > 0)
                        {
                            var tp = splt[1];
                            if (tp == "M")
                            {
                                acctId = decimal.Parse(splt[0]);
                            }
                            else
                            {
                                pid = splt[0];
                            }
                        }
                    }
                    var objT = new SM_TERMINAL()
                    {
                        TERMINALID = h.TERMINALID,
                        //INSTITUTION_ID = h.INSTITUTION_ID != null ? h.INSTITUTION_ID.ToString() : null,
                        MERCHANTID = h.MERCHANTID,
                        POINTED = h.PROCESSOR,
                        TERMINALMODEL_CODE = h.TERMINALMODEL_CODE,
                        TERMINALOWNER_CODE = h.TERMINALOWNER_CODE,
                        // PAYATTITUDE_STAMP = h.PAYATTITUDE_ACCEPTANCE,

                        BATCHID = h.BATCHID,
                        //RULE_OPTION = "R",
                        SLIP_FOOTER = h.SLIP_FOOTER,
                        SLIP_HEADER = h.SLIP_HEADER,
                        PTSP = h.PTSP,
                        //  INTERFACE_FORMAT = h.INTERFACE_FORMAT,
                        EMAIL_ALERTS = h.EMAIL_ALERTS,
                        TRANSACTION_CURRENCY = h.TRANSACTION_CURRENCY,
                        SETTLEMENT_CURRENCY = h.SETTLEMENT_CURRENCY,
                        //ACCEPTANCE_TYPE = "DOMESTIC",
                        PTSA = h.PTSA,
                        VERVACQUIRERIDNO = h.VERVACQUIRERIDNO,
                        MASTACQUIRERIDNO = h.MASTACQUIRERIDNO,
                        VISAACQUIRERIDNO = h.VISAACQUIRERIDNO,
                        TERMINALSTATUS = "",
                        SETTLEMENT_FREQUENCY = h.SETTLEMENT_FREQUENCY, //  curCode == "566" ? "2" :"3",
                        CREATEDATE = h.CREATEDATE,
                        STATUS = active,
                        USERID = mD.USERID,
                        //RULE_LOCATOR = "R"

                    };
                    if (isNewMid)
                    {
                        mm.SM_TERMINAL.Add(objT); // add terminal to a new merchant
                    }
                    else
                    {
                        mObj.SM_TERMINAL.Add(objT); // add terminal to an existing merchant
                    }

                    if (h.IS_NEWACCT)
                    {
                        var gg = repoMAcctTemp.AllEager(d=> d.PID == pid).FirstOrDefault();
                        if (gg != null)
                        {
                            var existCount = tstAcct.Count(p => p.BANKACCNO == gg.DEPOSIT_ACCOUNTNO && p.BANKCODE == gg.DEPOSIT_BANKCODE);
                            tstAcct.Add(new SM_MERCHANTTERMINALUPLD()
                            {
                                MERCHANTID = h.MERCHANTID,
                                BANKCODE = gg.DEPOSIT_BANKCODE,
                                BANKACCNO = gg.DEPOSIT_ACCOUNTNO
                            });
                            if (existCount == 0)
                            {
                                var dd = _repo.GetINSTITUTION_BY_CBNCODE(gg.DEPOSIT_BANKCODE); // repoInst.AllEager(null,u => u.CBN_CODE == h.BANKCODE).FirstOrDefault();
                                var acct = new SM_MERCHANTACCT()
                                {
                                    CREATEDATE = DateTime.Now,
                                    DEPOSIT_ACCOUNTNO = gg.DEPOSIT_ACCOUNTNO,
                                    DEPOSIT_BANKCODE = gg.DEPOSIT_BANKCODE,
                                    DEPOSIT_BANKNAME = dd.INSTITUTION_NAME,
                                    DEPOSIT_ACCTNAME = gg.DEPOSIT_ACCTNAME,
                                    DEPOSIT_COUNTRYCODE = dd.INSTITUTION_COUNTRY,
                                    MERCHANTID = h.MERCHANTID,
                                    STATUS = active,
                                    USERID = h.USERID,
                                    SETTLEMENTCURRENCY = "566"
                                };
                                if (isNewMid)
                                {

                                    mm.SM_MERCHANTACCT.Add(acct); // add acct to an existing merchant
                                }
                                else
                                {
                                    mObj.SM_MERCHANTACCT.Add(acct);
                                }
                                objT.SM_MERCHANTACCT = acct; // ADD NEW ACCOUNT TO TERMINAL
                            }
                        }
                    }
                    else
                    {
                        objT.ACCOUNT_ID = null;
                        var acct = repoMAcct.Find(acctId);
                        if (acct != null)
                        {
                            objT.SM_MERCHANTACCT = acct;
                        }
                    }
                }

                //var mAList = repoMAcctTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
                //foreach (var d in mAList)
                //{

                //}
                return true;
            }
            return false;
        }
        
        [HttpPost]
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
                                //recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                RejectRecord(rec2.BATCHID, rec2.USERID);

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    sucTrue = true;
                                    //txscope.Complete();
                                }
                            }
                        }
                    }
                //}
                if (sucTrue)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Single Merchant Registeration", Narration, fullName);
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
                }

                respMsg = "This request has already been processed by an authorizer.";
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

        private bool RejectRecord(string bATCHID, string uSERID)
        {
            var rec = repoMTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).FirstOrDefault();
            if(rec != null)
            {
                rec.STATUS = reject;
                var recT = repoTTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
                foreach(var f in recT)
                {
                    f.STATUS = reject;
                }
                var recM = repoMAcctTemp.AllEager(d => d.BATCHID == bATCHID && d.USERID == uSERID).ToList();
                foreach (var f in recM)
                {
                    f.STATUS = reject;
                }

                return true;
            }
            return false;
        }
    }
}