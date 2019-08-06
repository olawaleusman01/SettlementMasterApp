using Generic.Dapper.Data;
using Generic.Dapper.Model;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Generic.Data;
using SettlementMaster.App.Models;
//using System.Transactions;
using Generic.Dapper.Utility;
using Generic.Data.Utilities;
using System.Text;
using Generic.Dapper.Utilities;
using System.Text.RegularExpressions;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using Generic.Dapper.ExcelUtility;

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class MerchantUpdateController : Controller
    {
            int institutionId;
            IDapperGeneralSettings _repo = new DapperGeneralSettings();
            private readonly IUnitOfWork uow = null;
            private readonly IRepository<SM_MERCHANTUPDATEUPLD> repoUpld = null;
            private readonly IRepository<SM_MERCHANTDETAIL> repoM = null;
            private readonly IRepository<SM_MERCHANTACCT> repoMAcct = null;
            private readonly IRepository<SM_CURRENCY> repoCur = null;
            private readonly IRepository<SM_TERMINAL> repoT = null;
            private readonly IRepository<SM_MERTERUPDUPLDGLO> repoUpldGlo = null;
            private readonly IRepository<SM_UPMERUPDUPLDGLO> repoUpldGloUp = null;
            private readonly IRepository<SM_UPMERCHANTUPDATEUPLD> repoUP = null;
            private readonly IRepository<SM_AUTHLIST> repoAuth = null;
            private readonly IRepository<SM_MERCHANTCONFIG> repoMerVal = null;
            private readonly IRepository<SM_AUTHCHECKER> repoAuthChecker = null;
            private readonly IRepository<SM_INSTITUTION> repoInst = null;
            private readonly IRepository<SM_MCC> repoMCC = null;
            private readonly IRepository<SM_PARTY> repoParty = null;
            private readonly IRepository<SM_STATE> repoState = null;
            private readonly IRepository<AspNetUser> repoUsers = null;
            const string Single = "SINGLE";
            const string Batch = "BATCH";
            int roleId;
            int menuId = 25;
            string fullName;
            string deptCode;
            string userEmail;
            string active = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.ACTIVE);
            string inActive = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.INACTIVE);
            string open = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.OPEN);
            string close = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.CLOSED);
            string approve = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.APPROVED);
            string reject = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.REJECTED);
            string unapprove = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.UNAPPROVED);

            readonly string eventInsert = "New";
            readonly string eventEdit = "Modify";
            readonly string eventDelete = "Delete";
            public MerchantUpdateController()
            {
                uow = new UnitOfWork();
                repoUpld = new Repository<SM_MERCHANTUPDATEUPLD>(uow);
                repoMAcct = new Repository<SM_MERCHANTACCT>(uow);
                repoMerVal = new Repository<SM_MERCHANTCONFIG>(uow);
                repoAuth = new Repository<SM_AUTHLIST>(uow);
                repoUpldGlo = new Repository<SM_MERTERUPDUPLDGLO>(uow);
                repoUpldGloUp = new Repository<SM_UPMERUPDUPLDGLO>(uow);
                repoUP = new Repository<SM_UPMERCHANTUPDATEUPLD>(uow);
                repoAuthChecker = new Repository<SM_AUTHCHECKER>(uow);
                repoInst = new Repository<SM_INSTITUTION>(uow);
                repoT = new Repository<SM_TERMINAL>(uow);
                repoM = new Repository<SM_MERCHANTDETAIL>(uow);
                repoMCC = new Repository<SM_MCC>(uow);
                repoParty = new Repository<SM_PARTY>(uow);
                repoState = new Repository<SM_STATE>(uow);
                repoUsers = new Repository<AspNetUser>(uow);
                repoCur = new Repository<SM_CURRENCY>(uow);
                var user = new UserDataSettings().GetUserData();
                if (user != null)
                {
                    roleId = user.UserRole;
                    institutionId = user.InstitutionId;
                    deptCode = user.DeptCode;
                    fullName = user.FullName;
                }
            }
        // GET: MerchantPre
        [MyAuthorize]
        public ActionResult Index()
        {
            ViewBag.RoleId = roleId;
            ViewBag.MenuId = menuId;
            BindCombo();
            return View();
        }

            protected int ValidateUpload(int instId)
            {
                var rv = new MerchantUploadSession();

                //try
                //{
                //  int instId;
                string selectedBankCode = "";
                string countryCode = "";
                if (institutionId == 1)//if XP
                {
                    var t = repoInst.Find(instId);//(f => f.ITBID == instSelected);
                    if (t != null)
                    {
                        selectedBankCode = t.CBN_CODE;
                        countryCode = t.INSTITUTION_COUNTRY;
                    }
                    // pnlResponse.Visible = true;
                    // pnlResponseMsg.Text = "Please you must select a bank before you validate";
                    // pnlResponse.CssClass = "alert alert-warning alert-bold alert-dismissable";
                }
                else
                {
                    var t = repoInst.Find(institutionId); // repoInst.All.SingleOrDefault(f => f.ITBID == userInstitutionItbid);
                    if (t != null)
                    {
                        selectedBankCode = t.CBN_CODE;
                        countryCode = t.INSTITUTION_COUNTRY;
                    }
                    // instSelected = userInstitutionItbid;

                }
            List<MerchantUpldObj> lst = new List<MerchantUpldObj>();
            int midLength = 0;
            string midType = "";
            bool midReq = false;
            int midNameLength = 0;
            string midNameType = "";
            bool midNameReq = false;
            int acctNoLength = 0;
            string acctNoType = "";
            bool acctNoReq = false;
            int bankCodeLength = 0;
            string bankCodeType = "";
            bool bankCodeReq = false;
            int binNoLength = 0;
            string binNoType = "";
            bool binNoReq = false;
            int ptspLength = 0;
            string ptspType = "";
            bool ptspReq = false;
            int ptsaLength = 0;
            string ptsaType = "";
            bool ptsaReq = false;
            int tidLength = 0;
            string tidType = "";
            bool tidReq = false;
            int mccLength = 0;
            string mccType = "";
            bool mccReq = false;
            var fieldValList = repoMerVal.AllEager(g => g.STATUS.ToLower() == active.ToLower()).ToList();
            if (fieldValList.Count != 0)
            {
                //index 1
                if (countryCode == "NGN")
                {
                    midLength = fieldValList[0].FIELDLENGTH ?? 0;
                    midType = fieldValList[0].FIELDDATATYPE;
                    midReq = fieldValList[0].DOM_REQUIRED == "Y" ? true : false;
                    //index 2
                    midNameLength = fieldValList[1].FIELDLENGTH ?? 0;
                    midNameType = fieldValList[1].FIELDDATATYPE;
                    midNameReq = fieldValList[1].DOM_REQUIRED == "Y" ? true : false;
                    //index 3
                    bankCodeLength = fieldValList[2].FIELDLENGTH ?? 0;
                    bankCodeType = fieldValList[2].FIELDDATATYPE;
                    bankCodeReq = fieldValList[2].DOM_REQUIRED == "Y" ? true : false;
                    //index 4
                    acctNoLength = fieldValList[3].FIELDLENGTH ?? 0;
                    acctNoType = fieldValList[3].FIELDDATATYPE;
                    acctNoReq = fieldValList[3].DOM_REQUIRED == "Y" ? true : false;
                    // index 5
                    binNoLength = fieldValList[4].FIELDLENGTH ?? 0;
                    binNoType = fieldValList[4].FIELDDATATYPE;
                    binNoReq = fieldValList[4].DOM_REQUIRED == "Y" ? true : false;
                    //index 6
                    ptspLength = fieldValList[5].FIELDLENGTH ?? 0;
                    ptspType = fieldValList[5].FIELDDATATYPE;
                    ptspReq = fieldValList[5].DOM_REQUIRED == "Y" ? true : false;
                    //index 7
                    tidLength = fieldValList[6].FIELDLENGTH ?? 0;
                    tidType = fieldValList[6].FIELDDATATYPE;
                    tidReq = fieldValList[6].DOM_REQUIRED == "Y" ? true : false;
                    //index 8
                    mccLength = fieldValList[7].FIELDLENGTH ?? 0;
                    mccType = fieldValList[7].FIELDDATATYPE;
                    mccReq = fieldValList[7].DOM_REQUIRED == "Y" ? true : false;

                    ptsaLength = fieldValList[8].INT_FIELDLENGTH ?? 0;
                    ptsaType = fieldValList[8].INT_FIELDDATATYPE;
                    ptsaReq = fieldValList[8].INT_REQUIRED == "Y" ? true : false;
                }
                else
                {
                    midLength = fieldValList[0].INT_FIELDLENGTH ?? 0;
                    midType = fieldValList[0].INT_FIELDDATATYPE;
                    midReq = fieldValList[0].INT_REQUIRED == "Y" ? true : false;
                    //index 2
                    midNameLength = fieldValList[1].INT_FIELDLENGTH ?? 0;
                    midNameType = fieldValList[1].INT_FIELDDATATYPE;
                    midNameReq = fieldValList[1].INT_REQUIRED == "Y" ? true : false;
                    //index 3
                    bankCodeLength = fieldValList[2].INT_FIELDLENGTH ?? 0;
                    bankCodeType = fieldValList[2].INT_FIELDDATATYPE;
                    bankCodeReq = fieldValList[2].INT_REQUIRED == "Y" ? true : false;
                    //index 4
                    acctNoLength = fieldValList[3].INT_FIELDLENGTH ?? 0;
                    acctNoType = fieldValList[3].INT_FIELDDATATYPE;
                    acctNoReq = fieldValList[3].INT_REQUIRED == "Y" ? true : false;
                    // index 5
                    binNoLength = fieldValList[4].INT_FIELDLENGTH ?? 0;
                    binNoType = fieldValList[4].INT_FIELDDATATYPE;
                    binNoReq = fieldValList[4].INT_REQUIRED == "Y" ? true : false;
                    //index 6
                    ptspLength = fieldValList[5].INT_FIELDLENGTH ?? 0;
                    ptspType = fieldValList[5].INT_FIELDDATATYPE;
                    ptspReq = fieldValList[5].INT_REQUIRED == "Y" ? true : false;
                    //index 7
                    tidLength = fieldValList[6].INT_FIELDLENGTH ?? 0;
                    tidType = fieldValList[6].INT_FIELDDATATYPE;
                    tidReq = fieldValList[6].INT_REQUIRED == "Y" ? true : false;
                    //index 8
                    mccLength = fieldValList[7].INT_FIELDLENGTH ?? 0;
                    mccType = fieldValList[7].INT_FIELDDATATYPE;
                    mccReq = fieldValList[7].INT_REQUIRED == "Y" ? true : false;

                    ptsaLength = fieldValList[8].INT_FIELDLENGTH ?? 0;
                    ptsaType = fieldValList[8].INT_FIELDDATATYPE;
                    ptsaReq = fieldValList[8].INT_REQUIRED == "Y" ? true : false;
                }
            }
            var rec = rv.GetMerchantUpload(User.Identity.Name);
            //SM_MERTERUPLDGLO objG = null;
            //int cnt = 0;
            //int grst = 0;
            int totalErrorCount = 0;
            foreach (var t in rec)
            {
                int errorCount = 0;
                var validationErrorMessage = new List<string>();
                decimal mid;
                int specialCount = 0;
                // validating merchant id  (1)
                if (midReq)
                {
                    if (midType == "STRING")
                    {
                        // Match match = Regex.Match(t.MERCHANTID, "^a-z0-9", RegexOptions.IgnoreCase);
                        if (Regex.IsMatch(t.MERCHANTID, "[^a-z0-9]", RegexOptions.IgnoreCase))
                        {
                            specialCount++;
                            //match.NextMatch();
                        }
                        if (specialCount > 0)
                        {
                            errorCount++;
                            // totalErrorCount++;
                            validationErrorMessage.Add(string.Format("Special Character is not allowed for Merchant Id"));
                        }

                    }
                    else
                    {
                        if (!decimal.TryParse(t.MERCHANTID, out mid))
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            validationErrorMessage.Add(string.Format("Merchant ID must be a number"));
                        }
                    }
                    if (midLength != 0 && t.MERCHANTID.Length != midLength)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(string.Format("MID must be {0} Character", midLength));

                    }
                }
                // validating merchant name  (2)
                //if (midNameType == "STRING")
                //{

                //    Match match = Regex.Match(t.MERCHANTNAME, "", RegexOptions.IgnoreCase);
                //    while (match.Success)
                //    {
                //        specialCount++;
                //        match.NextMatch();
                //    }
                //    if (specialCount > 0)
                //    {
                //        t.ValidationErrorStatus = true;
                //        validationErrorMessage.Add(string.Format("Special Character is not allowed for Merchant Id"));
                //    }

                //}
                //else
                //{
                //    if (!decimal.TryParse(t.MERCHANTID, out mid))
                //    {
                //        t.ValidationErrorStatus = true;
                //        validationErrorMessage.Add(string.Format("Merchant ID must be a number"));
                //    }
                //}
                if (!string.IsNullOrEmpty(t.MERCHANTNAME))
                {
                    if (midNameReq)
                    {
                        if (t.MERCHANTNAME.Length > midNameLength)
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            validationErrorMessage.Add(string.Format("Merchant Id Lenght must not be more than {0}", midNameLength));

                        }
                    }
                }

                // VALIDATE MERCHANT NAME

                // validating bank Code  (3)
                ////if (!string.IsNullOrEmpty(t.BANKCODE))
                ////{
                ////    if (bankCodeReq)
                ////    {
                ////        if (bankCodeType == "STRING")
                ////        {
                ////            specialCount = 0;
                ////            if (Regex.IsMatch(t.BANKCODE, "[^a-z0-9]", RegexOptions.IgnoreCase))
                ////            {

                ////                errorCount++;
                ////                //  totalErrorCount++;
                ////                validationErrorMessage.Add(string.Format("Special Character is not allowed for Bank Code"));
                ////            }

                ////        }
                ////        else
                ////        {
                ////            if (!decimal.TryParse(t.BANKCODE, out mid))
                ////            {
                ////                errorCount++;
                ////                // totalErrorCount++;
                ////                validationErrorMessage.Add(string.Format(@"""Bank Code"" must be a number"));
                ////            }
                ////        }
                ////    }

                ////    if (bankCodeReq)
                ////    {

                ////        if (bankCodeLength != 0 && t.BANKCODE.Length > bankCodeLength)
                ////        {
                ////            errorCount++;
                ////            //totalErrorCount++;
                ////            validationErrorMessage.Add(string.Format(@"""Bank Code"" Lenght must not be more than {0}", bankCodeLength));

                ////        }
                ////    }
                ////}
                ////if (!string.IsNullOrEmpty(t.BANKACCNO))
                ////{
                ////    if (acctNoReq)
                ////    {
                ////        // validating Account No  (4)
                ////        if (acctNoType == "STRING")
                ////        {
                ////            specialCount = 0;
                ////            if (Regex.IsMatch(t.BANKACCNO, "[^a-z0-9]", RegexOptions.IgnoreCase))
                ////            {

                ////                errorCount++;
                ////                //  totalErrorCount++;
                ////                validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""Account No."""));
                ////            }

                ////        }
                ////        else
                ////        {
                ////            //if (countryCode == "NGN")
                ////            //{
                ////            if (!decimal.TryParse(t.BANKACCNO, out mid))
                ////            {
                ////                errorCount++;
                ////                //  totalErrorCount++;
                ////                validationErrorMessage.Add(string.Format(@"""Account No."" must be a number"));
                ////            }
                ////            // }
                ////        }
                ////        //if (countryCode == "NGN")
                ////        //{

                ////        if (acctNoLength != 0 && t.BANKACCNO.Length != acctNoLength)
                ////        {
                ////            errorCount++;
                ////            // totalErrorCount++;
                ////            validationErrorMessage.Add(string.Format(@"""Account No."" Lenght must be {0}", acctNoLength));

                ////        }
                ////        //}

                ////    }


                ////    var gh = repoMAcct.AllEager(f => f.DEPOSIT_ACCOUNTNO == t.BANKACCNO.TrimEnd() && f.DEPOSIT_BANKCODE == t.BANKCODE).ToList();
                ////    if (gh.Count <= 0)
                ////    {
                ////        t.WARNINGMESSAGE = GetString(@"""BANKACCNO"" is a new Account ");
                ////    }
                ////}
                if (!string.IsNullOrEmpty(t.BANKACCNO))
                {



                    if (acctNoReq)
                    {
                        // validating Account No  (4)
                        if (acctNoType == "STRING")
                        {
                            specialCount = 0;
                            if (Regex.IsMatch(t.BANKACCNO, "[^a-z0-9]", RegexOptions.IgnoreCase))
                            {

                                errorCount++;
                                //  totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""Account No."""));
                            }

                        }
                        else
                        {
                            //if (countryCode == "NGN")
                            //{
                            if (!decimal.TryParse(t.BANKACCNO, out mid))
                            {
                                errorCount++;
                                //  totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"""Account No."" must be a number"));
                            }
                            // }
                        }
                        //if (countryCode == "NGN")
                        //{

                        if (acctNoLength != 0 && t.BANKACCNO.Length != acctNoLength)
                        {
                            errorCount++;
                            // totalErrorCount++;
                            validationErrorMessage.Add(string.Format(@"""Account No."" Lenght must be {0}", acctNoLength));

                        }
                        //}

                    }


                    var gh = repoMAcct.AllEager(f => f.DEPOSIT_ACCOUNTNO == t.BANKACCNO.TrimEnd() && f.DEPOSIT_BANKCODE == t.BANKCODE).ToList();
                    if (gh.Count <= 0)
                    {
                        t.WARNINGMESSAGE = GetString(@"""BANKACCNO"" is a new Account ");
                    }

                    if (string.IsNullOrEmpty(t.BANKCODE))
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(string.Format(@"""Bank Code"" must not be empty."));
                    }
                    if (bankCodeReq)
                    {
                        if (bankCodeType == "STRING")
                        {
                            specialCount = 0;
                            if (Regex.IsMatch(t.BANKCODE, "[^a-z0-9]", RegexOptions.IgnoreCase))
                            {

                                errorCount++;
                                //  totalErrorCount++;
                                validationErrorMessage.Add(string.Format("Special Character is not allowed for Bank Code"));
                            }

                        }
                        else
                        {
                            if (!decimal.TryParse(t.BANKCODE, out mid))
                            {
                                errorCount++;
                                // totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"""Bank Code"" must be a number"));
                            }
                        }
                    }

                    if (bankCodeReq)
                    {

                        if (bankCodeLength != 0 && t.BANKCODE.Length > bankCodeLength)
                        {
                            errorCount++;
                            //totalErrorCount++;
                            validationErrorMessage.Add(string.Format(@"""Bank Code"" Lenght must not be more than {0}", bankCodeLength));

                        }
                    }

                    /////account name update
                    if (string.IsNullOrEmpty(t.ACCOUNTNAME))
                    {
                        errorCount++;
                        // totalErrorCount++;
                        validationErrorMessage.Add(@"""Account Name."" must not be empty");

                    }
                }



                // Validate to know if State Code already setup in the database
                if (!string.IsNullOrEmpty(t.STATECODE))
                {
                    var stateCode = repoState.AllEager(d => d.STATECODE == t.STATECODE && d.COUNTRYCODE == countryCode && d.STATUS != null && d.STATUS.ToLower() == active.ToLower()).FirstOrDefault();

                    if (stateCode == null)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""STATECODE"" does not exist");
                    }
                }
                // validating BIN  No  (5

                if (binNoReq)
                {
                    if (binNoType == "STRING")
                    {
                        if (countryCode == "NGN")
                        {
                            if (!string.IsNullOrEmpty(t.VERVEACQUIRERID))
                            {
                                specialCount = 0;
                                if (Regex.IsMatch(t.VERVEACQUIRERID, "[^a-z0-9]", RegexOptions.IgnoreCase))
                                {

                                    errorCount++;
                                    //  totalErrorCount++;
                                    validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""VERVEACQUIRERIDNUMBER"""));
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(t.VISAACQUIRERID))
                        {
                            if (Regex.IsMatch(t.VISAACQUIRERID, "[^a-z0-9]", RegexOptions.IgnoreCase))
                            {
                                errorCount++;
                                //totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""VISAACQUIRERIDNUMBER"""));
                            }
                        }

                        if (!string.IsNullOrEmpty(t.MASTERCARDACQUIRERID))
                        {
                            if (Regex.IsMatch(t.MASTERCARDACQUIRERID, "[^a-z0-9]", RegexOptions.IgnoreCase))
                            {

                                errorCount++;
                                // totalErrorCount++;

                                validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""MASTERCARDACQUIRERIDNUMBER"""));
                            }
                        }
                    }
                    else
                    {

                        if (countryCode == "NGN")
                        {
                            if (!string.IsNullOrEmpty(t.VERVEACQUIRERID))
                            {
                                if (!decimal.TryParse(t.VERVEACQUIRERID, out mid))
                                {
                                    errorCount++;
                                    //  totalErrorCount++;

                                    validationErrorMessage.Add(string.Format(@"""VERVEACQUIRERIDNUMBER"" must be a number"));
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(t.VISAACQUIRERID))
                        {
                            if (!decimal.TryParse(t.VISAACQUIRERID, out mid))
                            {
                                errorCount++;
                                //  totalErrorCount++;

                                validationErrorMessage.Add(string.Format(@"""VISAACQUIRERIDNUMBER"" must be a number"));
                            }
                        }

                        if (!string.IsNullOrEmpty(t.MASTERCARDACQUIRERID))
                        {
                            if (!decimal.TryParse(t.MASTERCARDACQUIRERID, out mid))
                            {
                                errorCount++;
                                //   totalErrorCount++;

                                validationErrorMessage.Add(string.Format(@"""MASTERCARDACQUIRERIDNUMBER"" must be a number"));
                            }
                        }
                    }
                    if (countryCode == "NGN")
                    {
                        if (!string.IsNullOrEmpty(t.VERVEACQUIRERID))
                        {
                            if (binNoLength != 0 && t.VERVEACQUIRERID.Length != binNoLength)
                            {
                                errorCount++;
                                //  totalErrorCount++;

                                validationErrorMessage.Add(string.Format(@"""VERVEACQUIRERIDNUMBER"" Lenght must not be more than {0}", binNoLength));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(t.VISAACQUIRERID))
                    {
                        if (binNoLength != 0 && t.VISAACQUIRERID.Length != binNoLength)
                        {
                            errorCount++;
                            //   totalErrorCount++;

                            validationErrorMessage.Add(string.Format(@"""VISAACQUIRERIDNUMBER"" Lenght must not be more than {0}", binNoLength));
                        }
                    }

                    if (!string.IsNullOrEmpty(t.MASTERCARDACQUIRERID))
                    {
                        if (binNoLength != 0 && t.MASTERCARDACQUIRERID.Length != binNoLength)
                        {
                            errorCount++;
                            //  totalErrorCount++;


                            validationErrorMessage.Add(string.Format(@"""MASTERCARDACQUIRERIDNUMBER"" Lenght must not be more than {0}", binNoLength));

                        }
                    }
                }
                // validating Account No  (6)
                // validating ptsp 
                if (!string.IsNullOrEmpty(t.PTSP))
                {
                    if (ptspReq)
                    {
                        if (ptspType == "STRING")
                        {


                        }
                        else
                        {
                            if (!decimal.TryParse(t.PTSP, out mid))
                            {
                                errorCount++;
                                //    totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"""PTSP"" must be a number"));
                            }
                        }
                        if (t.PTSP.Length > ptspLength)
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            validationErrorMessage.Add(string.Format(@"""PTSP"" Lenght must not be more than {0}", ptspLength));

                        }
                    }
                    // Validate to know if pARTY SHORTNAME already setup in the database
                    var ptspp = !string.IsNullOrEmpty(t.PTSP) ? t.PTSP.ToUpper() : "";
                    var ptspCount = repoParty.AllEager(d => d.PARTYTYPE_CODE == "PTSP" && d.PARTY_SHORTNAME != null && d.PARTY_SHORTNAME.ToUpper() == ptspp && d.STATUS != null && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                    if (ptspCount <= 0)
                    {
                        ptspCount = repoInst.AllEager(d => (d.PTSP == "Y" || d.PTSP == "y") && d.INSTITUTION_SHORTCODE != null && d.INSTITUTION_SHORTCODE.ToUpper() == ptspp).ToList().Count;

                    }
                    if (ptspCount <= 0)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""PTSP"" does not exist");
                    }
                }
                if (!string.IsNullOrEmpty(t.PTSA))
                {
                    // validating ptsp 
                    if (ptsaReq)
                    {
                        if (ptsaType == "STRING")
                        {


                        }
                        else
                        {
                            if (!decimal.TryParse(t.PTSA, out mid))
                            {
                                errorCount++;
                                //    totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"""PTSA"" must be a number"));
                            }
                        }
                        if (t.PTSA.Length > ptspLength)
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            validationErrorMessage.Add(string.Format(@"""PTSA"" Lenght must not be more than {0}", ptspLength));

                        }
                    }

                    var ptspa = !string.IsNullOrEmpty(t.PTSA) ? t.PTSA.ToUpper() : "";
                    var ptsaCount = repoParty.AllEager(d => d.PARTYTYPE_CODE == "PTSA" && d.PARTY_SHORTNAME != null && d.PARTY_SHORTNAME.ToUpper() == ptspa && d.STATUS != null && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                    
                    if (ptsaCount <= 0)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""PTSA"" does not exist");
                    }
                }

                if (tidReq)
                {
                    if (tidType == "STRING")
                    {
                        // Match match = Regex.Match(t.MERCHANTID, "^a-z0-9", RegexOptions.IgnoreCase);
                        if (Regex.IsMatch(t.TERMINALID, "[^a-z0-9]", RegexOptions.IgnoreCase))
                        {
                            specialCount++;
                            //match.NextMatch();
                        }
                        if (specialCount > 0)
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            validationErrorMessage.Add(string.Format("Special Character is not allowed for Terminal Id"));
                        }

                    }
                    else
                    {
                        if (!decimal.TryParse(t.TERMINALID, out mid))
                        {
                            errorCount++;
                            //totalErrorCount++;
                            validationErrorMessage.Add(string.Format("Terminal ID must be a number"));
                        }
                    }
                    if (t.TERMINALID.Length != tidLength)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(string.Format("Terminal ID must be {0} Character", tidLength));

                    }
                }
                // Validating MCC Code  (4)
                if (!string.IsNullOrEmpty(t.MERCHANTCATEGORYCODE))
                {
                    var mccCount = repoMCC.AllEager(d => d.MCC_CODE == t.MERCHANTCATEGORYCODE && d.STATUS.ToLower() == active.ToLower()).ToList();
                    if (mccCount.Count <= 0)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""MERCHANTCATEGORYCODE"" does not exist");
                    }

                    if (mccReq)
                    {
                        if (mccType == "STRING")
                        {
                            specialCount = 0;
                            if (Regex.IsMatch(t.MERCHANTCATEGORYCODE, "[^a-z0-9]", RegexOptions.IgnoreCase))
                            {
                                specialCount++;
                                //match.NextMatch();
                            }
                            if (specialCount > 0)
                            {
                                errorCount++;
                                //totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"Special Character is not allowed for ""MERCHANTCATEGORYCODE"""));
                            }

                        }
                        else
                        {
                            if (!decimal.TryParse(t.MERCHANTCATEGORYCODE, out mid))
                            {
                                errorCount++;
                                // totalErrorCount++;
                                validationErrorMessage.Add(string.Format(@"""MERCHANTCATEGORYCODE"" must be a number"));
                            }
                        }
                        if (t.MERCHANTCATEGORYCODE.Length != mccLength)
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            validationErrorMessage.Add(string.Format(@"""MERCHANTCATEGORYCODE"" Lenght must be {0}", mccLength));

                        }
                    }
                }
                // start database validation

                var tidCount = repoT.Find(t.TERMINALID);
                if (tidCount == null)
                {
                    errorCount++;
                    //totalErrorCount++;
                    validationErrorMessage.Add(@"""TERMINALID"" does not exist");
                }
                var instCbnCode = _repo.GetTerminalInstitutionCbnCode(t.TERMINALID);
                if (selectedBankCode != instCbnCode)
                {
                    errorCount++;
                    validationErrorMessage.Add(@"""TERMINALID"" does not belong to the selected institution.");
                }
                var MIDCount = repoM.Find(t.MERCHANTID);
                if (MIDCount == null)
                {
                    //var nme = MIDCount.MERCHANTNAME != null ? MIDCount.MERCHANTNAME.Trim().ToLower() : "";
                    //var cr = t.MERCHANTNAME != null ? t.MERCHANTNAME.Trim().ToLower() : "";
                    //if (nme != cr)
                    //{
                    errorCount++;
                    //totalErrorCount++;
                    validationErrorMessage.Add(@"""MerchantName"" does not exist. ");
                    // }
                }
                // Validate to know if terminal is more than one in the uploaded file
                var tidUpldCount = rec.Count(d => d.TERMINALID == t.TERMINALID);
                if (tidUpldCount > 1)
                {
                    errorCount++;
                    //totalErrorCount++;
                    validationErrorMessage.Add(@"Multiple ""TERMINALID"" exist in the uploaded file");
                }

                // Validate to know if terminal id has been uploaded before and waiting for approval
                var tidUpldRecCount = repoUpld.AllEager(f => f.TERMINALID == t.TERMINALID && f.INSTITUTION_ID == instId && f.STATUS.ToLower() == unapprove.ToLower()).ToList();
                if (tidUpldRecCount.Count > 0)
                {
                    errorCount++;
                    //  totalErrorCount++;
                    validationErrorMessage.Add(@"""TERMINALID"" exist in previous uploaded file waiting for authorization");
                }

                if (!string.IsNullOrEmpty(t.BANKCODE))
                {
                    // CBN Bank Code Validation
                    if (instId == 1)
                    {
                        //unified payment merchant is being uploaded hence only validate bank code of terminal from database
                        var dd = repoInst.AllEager(d => d.CBN_CODE == t.BANKCODE).FirstOrDefault();
                        if (dd == null)
                        {
                            errorCount++;
                            validationErrorMessage.Add(@"""BANK CODE"" does not exist");
                        }
                    }
                    else
                    {
                        if (selectedBankCode != t.BANKCODE)
                        {
                            errorCount++;
                            //  totalErrorCount++;
                            if (institutionId == 1)
                            {
                                validationErrorMessage.Add(@"""BANK CODE"" does not match Selected Bank Code");
                            }
                            else
                            {
                                validationErrorMessage.Add(@"""BANK CODE"" does not match User Bank Code");
                            }
                        }

                    }
                }

                // Validate to know if mcc code already setup in the database

                if (!string.IsNullOrEmpty(t.TRANSCURRENCY))
                {
                    var curcount = repoCur.AllEager(d => d.CURRENCY_CODE == t.TRANSCURRENCY && d.STATUS.ToLower() == active.ToLower()).ToList();
                    if (curcount.Count <= 0)
                    {
                        errorCount++;
                        //  totalErrorCount++;
                        validationErrorMessage.Add(@"""TRANSACTIONCURRENCY"" does not exist");
                    }

                }
                // Validate to know if pARTY SHORTNAME already setup in the database

                // Validate to know if terminal owner code already setup in the database
                if (!string.IsNullOrEmpty(t.TERMINALOWNERCODE))
                {
                    int termOwnCode;
                    if (int.TryParse(t.TERMINALOWNERCODE, out termOwnCode))
                    {
                        var termOwnerCodeCount = repoParty.AllEager(d => d.PARTY_CODE == t.TERMINALOWNERCODE && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                        if (termOwnerCodeCount == 0)
                        {
                            // 
                            termOwnerCodeCount = repoInst.AllEager(d => d.CBN_CODE == t.TERMINALOWNERCODE).ToList().Count;
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
                // Validate to know if business occupation class already setup in the database
                //if (!string.IsNullOrEmpty(t.TRANSCURRENCY))
                //{
                //    int bizClass;
                //    if (int.TryParse(t.BUISNESSOCCUPATIONCODE, out bizClass))
                //    {
                //        var bizCount = repoBiz.AllEager(null, d => d.CODE == bizClass && d.STATUS.ToLower() == active.ToLower()).ToList().Count;
                //        if (bizCount <= 0)
                //        {
                //            errorCount++;
                //            //  totalErrorCount++;
                //            validationErrorMessage.Add(@"""BUISNESSOCCUPATIONCODE"" does not exist");
                //        }
                //    }
                //    else
                //    {
                //        errorCount++;
                //        //totalErrorCount++;
                //        validationErrorMessage.Add(@"""BUISNESSOCCUPATIONCODE"" must be a number");
                //    }
                //}

                // Validate to know if terminal model code already setup in the database
                if (!string.IsNullOrEmpty(t.TERMINALMODELCODE))
                {
                    int termModelCode;
                    if (int.TryParse(t.TERMINALMODELCODE, out termModelCode))
                    {
                        //var termOwnerCodeCount = repoTermModel.All.Count(d => d.CODE == termModelCode && d.STATUS.ToLower() == active.ToLower());
                        //if (termOwnerCodeCount <= 0)
                        //{
                        //    errorCount++;
                        //    //totalErrorCount++;
                        //    validationErrorMessage.Add(@"""TERMINALMODELCODE"" does not exist");
                        //}
                    }
                    else
                    {
                        errorCount++;

                        validationErrorMessage.Add(@"""TERMINALMODELCODE"" must be a number");
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

                rv.PostMerchantUpload(t,2,User.Identity.Name);
            }

            //  lst.AddRange(lst);
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
                //}
                //catch (Exception ex)
                //{
                //    return -1;
                //}
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
            protected bool ForwardToXP(string batchid, string user_id)
            {
                try
                {
                    var rec = repoUpld.AllEager(g => g.BATCHID == batchid && g.USERID == user_id).ToList();
                    var rec2 = repoUpldGlo.AllEager(g => g.BATCHID == batchid && g.USERID == user_id).FirstOrDefault();
                    DateTime curDate = DateTime.Now;
                // string batchId = SmartObj.GenRefNo2();
                SM_UPMERUPDUPLDGLO objG = null;
                    int cnt = 0;
                    int grst = 0;
                    foreach (var d in rec)
                    {
                        if (cnt == 0)
                        {
                            if (rec2 != null)
                            {
                                rec2.STATUS = approve;
                            }
                            objG = new SM_UPMERUPDUPLDGLO();
                            objG.BATCHID = d.BATCHID;
                            objG.CREATEDATE = curDate;
                            objG.BATCHCOUNT = rec.Count;
                            objG.STATUS = open;
                            objG.USERID = d.USERID;
                            objG.INSTITUTION_ID = d.INSTITUTION_ID;
                            objG.MAKER_INST_ID = d.MAKER_INST_ID;
                            repoUpldGloUp.Insert(objG);
                            grst = uow.Save(User.Identity.Name);
                            //string baseUrl = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["baseUrl"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["baseUrl"]) : string.Empty;
                            //string menu = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MidMenuId"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["MidMenuId"]) : string.Empty;
                            //int menuInt = 0;
                            //int.TryParse(menu, out menuInt);
                            string url = Url.Action("DetailAuthXP", "MerchantPre"); // baseUrl + "/Admin/MerchantAuthQueue";
                            SM_AUTHLIST auth = new SM_AUTHLIST()
                            {
                                CREATEDATE = DateTime.Now,
                                EVENTTYPE = eventInsert,
                                MENUID = menuId,
                                //MENUNAME = "",
                                // RECORDID = objG.ITBID,
                                STATUS = open,
                                //TABLENAME = "SM_FREQUENCY",
                                URL = url,
                                USERID = d.USERID,
                                INSTITUTION_ITBID = institutionId,
                                BATCHID = d.BATCHID,
                                POSTTYPE = Batch,
                                TABLENAME = "Y"
                            };
                            repoAuth.Insert(auth);
                        }
                        d.STATUS = approve;
                        if (grst > 0)
                        {

                            var upObj = new SM_UPMERCHANTUPDATEUPLD()
                            {
                                ACCOUNTNAME = d.ACCOUNTNAME,
                                // AUTHORIZEID = userId,
                                BANKACCNO = d.BANKACCNO,
                                BANKCODE = d.BANKCODE,
                                BANKTYPE = d.BANKTYPE,
                                BANK_URL = d.BANK_URL,
                                BATCHID = d.BATCHID,
                                BUISNESSOCCUPATIONCODE = d.BUISNESSOCCUPATIONCODE,
                                CONTACTNAME = d.CONTACTNAME,
                                CONTACTTITLE = d.CONTACTTITLE,
                                CREATEDATE = d.CREATEDATE,
                                EMAIL = d.EMAIL,
                                EMAILALERTS = d.EMAILALERTS,
                                //LAST_MODIFIED_UID = d.LAST_MODIFIED_UID,
                                INSTITUTION_ID = d.INSTITUTION_ID,
                                LGA_LCDA = d.LGA_LCDA,
                                MASTERCARDACQUIRERID = d.MASTERCARDACQUIRERID,
                                MERCHANTCATEGORYCODE = d.MERCHANTCATEGORYCODE,
                                MERCHANTID = d.MERCHANTID,
                                MERCHANTNAME = d.MERCHANTNAME,
                                PHYSICALADDR = d.PHYSICALADDR,
                                SLIPHEADER = d.SLIPHEADER,
                                MOBILEPHONE = d.MOBILEPHONE,
                                PTSP = d.PTSP,
                                SLIPFOOTER = d.SLIPFOOTER,
                                STATECODE = d.STATECODE,
                                STATUS = open,
                                TERMINALID = d.TERMINALID,
                                TERMINALMODELCODE = d.TERMINALMODELCODE,
                                TERMINALOWNERCODE = d.TERMINALOWNERCODE,
                                VERVEACQUIRERID = d.VERVEACQUIRERID,
                                VISAACQUIRERID = d.VISAACQUIRERID,
                                USERID = d.USERID,
                                INST_AUTH_ID = User.Identity.Name,
                                GLOBAL_ITBID = objG.ITBID,
                                MAKER_INST_ID = d.MAKER_INST_ID,
                                //INTERFACE_FORMAT = d.INTERFACE_FORMAT,
                                //PROCESSOR = d.PROCESSOR,
                                PAYATTITUDE_ACCEPTANCE = d.PAYATTITUDE_ACCEPTANCE,
                                PTSA = d.PTSA,
                                TRANSCURRENCY = d.TRANSCURRENCY,
                            };

                            repoUP.Insert(upObj);
                        }
                        cnt++;
                    }
                    var f = uow.Save(User.Identity.Name);
                    if (f > 0)
                    {
                        //pnlResponse.Visible = true;
                        //pnlResponse.CssClass = "alert alert-success alert-bold alert-dismissable";
                        //pnlResponseMsg.Text = "Batch Approved Successfully and forwarded for further processing";
                        //panel_tab1.Visible = true;
                        //panel_tab2.Visible = false;
                        //BindGrid();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
        protected bool PostBulkUpload(string batchid, string user_id, out int? inst_itbid)
        {
           // string instCode = string.Empty;
            //int cnt = 0;
            var rec = repoUP.AllEager(g => g.BATCHID == batchid && g.USERID == user_id).ToList();
            List<SM_MERCHANTUPDATEUPLD> tstAcct = new List<SM_MERCHANTUPDATEUPLD>();
            foreach (var t in rec)
            {
                var trec = repoT.Find(t.TERMINALID);
                var mrec = repoM.Find(t.MERCHANTID);

                ////if (cnt == 0)
                ////{
                ////    instCode = t.BANKCODE;
                ////}
                ////cnt++;

                if (trec != null && mrec != null)
                {
                    if (!string.IsNullOrEmpty(t.PHYSICALADDR))
                    {
                        mrec.ADDRESS = t.PHYSICALADDR;
                    }
                    if (!string.IsNullOrEmpty(t.BUISNESSOCCUPATIONCODE))
                    {
                        mrec.BUSINESS_CODE = t.BUISNESSOCCUPATIONCODE;
                    }
                    if (!string.IsNullOrEmpty(t.CONTACTNAME))
                    {
                        mrec.CONTACTNAME = t.CONTACTNAME;
                    }
                    if (!string.IsNullOrEmpty(t.CONTACTTITLE))
                    {
                        mrec.CONTACTTITLE = t.CONTACTTITLE;
                    }
                    if (!string.IsNullOrEmpty(t.BANK_URL))
                    {
                        mrec.MERCHANT_URL = t.BANK_URL;
                    }
                    //if (!string.IsNullOrEmpty(t.BUISNESSOCCUPATIONCODE))
                    //{
                    //    mrec.INSTITUTION_ITBID = t.INSTITUTION_ID;
                    //}
                    if (!string.IsNullOrEmpty(t.EMAIL))
                    {
                        mrec.EMAIL = t.EMAIL;
                    }
                    if (!string.IsNullOrEmpty(t.MERCHANTCATEGORYCODE))
                    {
                        mrec.MCC_CODE = t.MERCHANTCATEGORYCODE;
                    }
                    //if (!string.IsNullOrEmpty(t.MERCHANTID))
                    //{
                    //    mrec.MERCHANTID = t.MERCHANTID;
                    //}
                    if (!string.IsNullOrEmpty(t.MERCHANTNAME))
                    {
                        mrec.MERCHANTNAME = t.MERCHANTNAME;
                    }
                    if (!string.IsNullOrEmpty(t.MOBILEPHONE))
                    {
                        mrec.PHONENO = t.MOBILEPHONE;
                    }

                    ////if (!string.IsNullOrEmpty(t.BANKCODE))
                    ////{
                    ////    mrec.INSTITUTION_CBNCODE = t.BANKCODE;
                    ////}
                    if (!string.IsNullOrEmpty(t.STATECODE))
                    {
                        mrec.STATE_CODE = t.STATECODE;
                    }
                    if (!string.IsNullOrEmpty(t.LGA_LCDA))
                    {
                        mrec.CITY_NAME = t.LGA_LCDA;
                    }

                    //if (!string.IsNullOrEmpty(t.BUISNESSOCCUPATIONCODE))
                    //{
                    //    trec.POINTED = t.PROCESSOR;
                    //}
                    if (t.TERMINALMODELCODE != null)
                    {
                        trec.TERMINALMODEL_CODE = t.TERMINALMODELCODE.ToString();
                    }
                    if (!string.IsNullOrEmpty(t.TERMINALOWNERCODE))
                    {
                        trec.TERMINALOWNER_CODE = t.TERMINALOWNERCODE;
                    }

                    //if (!string.IsNullOrEmpty(t.PAYATTITUDE_ACCEPTANCE))
                    //{
                    //    trec.PAYATTITUDE_STAMP = t.PAYATTITUDE_ACCEPTANCE;
                    //}

                    //if (!string.IsNullOrEmpty(t.BANKACCNO))
                    //{
                    //    var id = ProcessAccountNo(t.MERCHANTID, t.TERMINALID, t.BANKACCNO);
                    //    if (id != 0)
                    //    {
                    //        trec.ACCOUNT_ID = id;
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(t.BANKACCNO) && !string.IsNullOrEmpty(t.BANKCODE) && !string.IsNullOrEmpty(t.ACCOUNTNAME))
                    {
                        var ACount = repoMAcct.AllLocal.Local.FirstOrDefault(c => c.MERCHANTID == t.MERCHANTID
                    && c.DEPOSIT_ACCOUNTNO == t.BANKACCNO && c.DEPOSIT_BANKCODE == t.BANKCODE); // check if acct no already exist
                        var ACount2 = repoMAcct.AllEager(c => c.MERCHANTID == t.MERCHANTID
                        && c.DEPOSIT_ACCOUNTNO == t.BANKACCNO && c.DEPOSIT_BANKCODE == t.BANKCODE).FirstOrDefault(); // check if acct no already exist
                        //var th = repoMAcct.All.ToList();
                        //var th1 = repoMAcct.AllTEST.Local.ToList();
                        if (ACount == null && ACount2 == null)
                        {
                            var existCount = tstAcct.Count(p => p.BANKACCNO == t.BANKACCNO && p.BANKCODE == t.BANKCODE && p.MERCHANTID == t.MERCHANTID);
                            tstAcct.Add(new SM_MERCHANTUPDATEUPLD()
                            {
                                MERCHANTID = t.MERCHANTID,
                                BANKCODE = t.BANKCODE,
                                BANKACCNO = t.BANKACCNO
                            });
                            if (existCount == 0)
                            {
                                var dd = _repo.GetINSTITUTION_BY_CBNCODE(t.BANKCODE); // repoInst.AllEager(null,u => u.CBN_CODE == h.BANKCODE).FirstOrDefault();
                                var acct = new SM_MERCHANTACCT()
                                {
                                    CREATEDATE = DateTime.Now,
                                    DEPOSIT_ACCOUNTNO = t.BANKACCNO,
                                    DEPOSIT_BANKCODE = t.BANKCODE,
                                    DEPOSIT_BANKNAME = dd.INSTITUTION_NAME,
                                    DEPOSIT_ACCTNAME = t.ACCOUNTNAME,
                                    DEPOSIT_COUNTRYCODE = dd.INSTITUTION_COUNTRY,
                                    MERCHANTID = t.MERCHANTID,
                                    STATUS = active,
                                    USERID = t.USERID,
                                    SETTLEMENTCURRENCY = "566", // dd.CURRENCY_CODE
                                };
                                repoMAcct.Insert(acct);

                                // MCount.POSMISDB_MERCHANTACCT.Add(acct);

                                trec.SM_MERCHANTACCT = acct; // ADD NEW ACCOUNT TO TERMINAL
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            //  objT.ACCOUNT_ID = ACount.ITBID;
                            //  objT.POSMISDB_MERCHANTACCT = ACount ?? ACount2;// ADD AN EXISTING ACCOUNT ID TO TERMINAL
                            if (ACount != null)
                            {
                                ACount.STATUS = active;
                                trec.SM_MERCHANTACCT = ACount; // ?? ACount2;
                            }
                            else
                            {
                                ACount2.STATUS = active;
                                trec.SM_MERCHANTACCT = ACount2; // ?? ACount2;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(t.SLIPFOOTER))
                    {
                        trec.SLIP_FOOTER = t.SLIPFOOTER;
                    }
                    if (!string.IsNullOrEmpty(t.SLIPHEADER))
                    {
                        trec.SLIP_HEADER = t.SLIPHEADER;
                    }
                    if (!string.IsNullOrEmpty(t.PTSP))
                    {
                        trec.PTSP = t.PTSP;
                    }
                    if (!string.IsNullOrEmpty(t.EMAILALERTS))
                    {
                        trec.EMAIL_ALERTS = t.EMAILALERTS;
                    }
                    if (!string.IsNullOrEmpty(t.TRANSCURRENCY))
                    {
                        trec.TRANSACTION_CURRENCY = t.TRANSCURRENCY;
                    }
                    if (!string.IsNullOrEmpty(t.PTSA))
                    {
                        trec.PTSA = t.PTSA;
                    }
                    if (!string.IsNullOrEmpty(t.VERVEACQUIRERID))
                    {
                        trec.VERVACQUIRERIDNO = t.VERVEACQUIRERID;
                    }
                    if (!string.IsNullOrEmpty(t.VISAACQUIRERID))
                    {
                        trec.VISAACQUIRERIDNO = t.VISAACQUIRERID;
                    }
                    if (!string.IsNullOrEmpty(t.MASTERCARDACQUIRERID))
                    {
                        trec.MASTACQUIRERIDNO = t.MASTERCARDACQUIRERID;
                    }


                }

            }
            inst_itbid = institutionId;
            //inst_itbid = int.Parse(instCode);
            return true;
        }
        private decimal ProcessAccountNo(string mERCHANTID, string tERMINALID, string acctNo)
        {
            var existAcct = _repo.GetMerchantAcct(mERCHANTID, acctNo).FirstOrDefault(); // repoMAcct.All.Where(e => e.MERCHANTID == mERCHANTID && e.DEPOSIT_ACCOUNTNO == acctNo).FirstOrDefault();
            if (existAcct != null)
            {
                return existAcct.ITBID;
            }
            return 0;
        }
        void SendUploadErrorNotification(string message, int record, string batchid, string fullName)
            {
                List<EmailObj> lst = new List<EmailObj>();
                lst.Add(new EmailObj()
                {
                    Email = userEmail,
                    RoleId = roleId
                });
                var mail = NotificationSystem.SendEmail(new EmailMessage()
                {
                    EmailAddress = lst,

                    emailSubject = "Upload Notififcation.",

                    EmailContent = new EmailerNotification().PopulateUploadErrorMessage(message, record, batchid, fullName),
                    EntryDate = DateTime.Now,
                    HasAttachment = false
                });
            }
            void BindCombo()
            {
                var rec = _repo.GetInstitution(0, true, "Active"); // repoInst.All.Where(e => e.STATUS.ToLower() == active.ToLower()).Select(e => new { Code = e.ITBID, Description = e.INSTITUTION_NAME }).ToList();
                ViewBag.BankList = new SelectList(rec, "ITBID", "INSTITUTION_NAME");

            }
            [HttpPost]
            // [ValidateAntiForgeryToken]
            public ActionResult Validate(int INSTITUTION_ID)
            {
                try
                {

                    var rv = new MerchantUploadSession();
                    var errCount = ValidateUpload(INSTITUTION_ID);
                    var rec = rv.GetMerchantUpload(User.Identity.Name);
                    var sucCount = rec.Count - errCount;
                    var html = PartialView("_MerchantUpld", rec).RenderToString();
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
                IList<MerchantUpldObj> model = null;
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

                        var dataList = ExcelReader.GetDataToList(path, addRecord); /// ExxcellReaderClosedXml.GetDataToList(path, addRecord);
                            //int cnt = 0;
                            var rv = new MerchantUploadSession();
                            var cnt = rv.PostMerchantUploadBulk(dataList.ToList(), User.Identity.Name);

                            if (cnt > 0)
                            {
                                var rst = rv.GetMerchantUpload(User.Identity.Name);
                                var html = PartialView("_MerchantUpld", rst).RenderToString();
                                return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .xlsx file" });
                            }
                            else
                            {
                                var html = PartialView("_MerchantUpld").RenderToString();
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
            public ActionResult Process(int INSTITUTION_ID, int? VALIDATED_INST_ID)
            {
                string batchId = "";
                try
                {
                    var isUp = false;
                    var rv = new MerchantUploadSession();
                    var rec = rv.GetMerchantUpload(User.Identity.Name);

                //  isUp = userInstitutionItbid == 1 ? true : false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 30, 0)))
                //{
                    int errorcnt = 0;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat(@" <table>
                                <thead>
                                    <tr>
                                          <th> S / N </th>
                                            <th>VALIDATION STATUS <th>                                          
                                          <th> MERCHANTID </th>
                                         <th> MERCHANTNAME </th>
                                            <th> CONTACTTITLE </th>
                                            <th> CONTACTNAME </th>
                                         <th> MOBILEPHONE </th >
                                           <th> EMAIL </th>
                                          <th> EMAILALERTS </th>
                                         <th> PHYSICALADDR </th>
                                        <th> TERMINALMODELCODE </th>
                                          <th> TERMINALID </ th >
                                          <th> BANKCODE </th>
                                          <th> BANKACCNO </th>
                                         <th> BANKTYPE </th>
                                         <th> SLIPHEADER </th>
                                        <th> SLIPFOOTER </ th >
                                        <th> BUSINESSOCCUPATIONCODE </th>
                                         <th> MERCHANTCATEGORYCODE </th>
                                      <th> STATECODE </th>
                                              <th> VISAACQUIRERIDNUMBER </th>
                                             <th> VERVEACQUIRERIDNUMBER </th>
                                            <th> MASTERCARDACQUIRERIDNUMBER </th>
                                           <th> TERMINALOWNERCODE </th>
                                        <th> LGA / LDA </th>
                                          <th> URL </th>
                                           <th> ACCOUNTNAME </th>
                                        <th> PTSP </th> 
                                        <th> TRANSACTION CURRENCY </th> 
                                        <th> PTSA </th> 
                                        <th> PAYATTITUDE ACCEPTANCE  </th> 
                                        </tr>
                                </thead>
                                <tbody>
                                       
                                ");
                    DateTime curDate = DateTime.Now;
                    if (institutionId == 1)
                    {
                        batchId = "XP_" + SmartObj.GenRefNo2();
                    }
                    else
                    {
                        batchId = "BK_" + SmartObj.GenRefNo2();
                    }
                    SM_MERTERUPDUPLDGLO objG = null;
                    SM_UPMERUPDUPLDGLO objGUp = null;
                    int cnt = 0;
                    int grst = 0;
                    var instid = institutionId;
                    if (institutionId == 1)
                    {
                        isUp = true;

                        if (VALIDATED_INST_ID != INSTITUTION_ID)
                        {
                            //pnlResponse.Visible = true;
                            //pnlResponseMsg.Text = "Institution selected when validating is different from the selected institution.Please Select the institution or revaildate before saving the record.";
                            //pnlResponse.CssClass = "alert alert-warning alert-bold alert-dismissable";
                            return Json(new { RespCode = 1, RespMessage = "Institution selected when validating is different from the selected institution.Please Select the institution or revaildate before saving the record." });
                        }
                    }
                    int i = 0;
                    if (isUp)
                    {
                        int valCount = rec.Count(f => f.VALIDATIONERRORSTATUS == false);
                        foreach (var t in rec)
                        {
                            // if ValidationErrorStatus == false then the record has no error
                            //var lblItbId = (Label)Repeater2.Items[i].FindControl("lblItbId");
                            // var rdInterdaceType = (RadioButtonList)Repeater2.Items[i].FindControl("rdInterdaceType");
                            //  var chkProcessor = (DropDownList)Repeater2.Items[i].FindControl("drpProcessor");

                            //var itbid = int.Parse(lblItbId.Text);
                            //var t = rec.Find(d => d.ITBID == itbid);
                            if (i == 0)
                            {
                                objGUp = new SM_UPMERUPDUPLDGLO(); // SM_MERTERUPLDGLO();
                                objGUp.BATCHID = batchId;
                                objGUp.CREATEDATE = curDate;
                                objGUp.BATCHCOUNT = valCount;
                                objGUp.STATUS = unapprove;
                                objGUp.USERID = User.Identity.Name;
                                objGUp.INSTITUTION_ID = instid;
                                objGUp.MAKER_INST_ID = institutionId;
                                repoUpldGloUp.Insert(objGUp);
                                grst = uow.Save(User.Identity.Name);

                                string url = Url.Action("DetailAuthXP", "Merchant"); // baseUrl + "/Admin/MerchantAuthQueue";

                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = eventEdit,
                                    MENUID = menuId,
                                    //MENUNAME = "",
                                    // RECORDID = objG.ITBID,
                                    STATUS = open,
                                    //TABLENAME = "SM_FREQUENCY",
                                    URL = url,
                                    USERID = User.Identity.Name,
                                    INSTITUTION_ITBID = institutionId,
                                    BATCHID = batchId,
                                    POSTTYPE = Batch

                                };
                                repoAuth.Insert(auth);
                            }
                            i++;
                            //if (t.ValidationErrorStatus == false && string.IsNullOrEmpty(rdInterdaceType.SelectedValue))
                            //{
                            //    pnlResponse.Visible = true;
                            //    pnlResponseMsg.Text = "Interface Type is not selected for some validated records";
                            //    pnlResponse.CssClass = "alert alert-warning alert-dismissable alert-bold";
                            //    return;
                            //}
                            if (t.VALIDATIONERRORSTATUS == true)
                            {
                                errorcnt++;
                                sb.AppendFormat(@"
                                        <tr>
                                            <td>{0}</td>
                                            <td>{1}</td>
                                               <td>{2}</td>
                                              <td>{3}</td>
                                                <td>{4}</td>
                                               <td>{5}</td>
                                              <td>{6}</td>
                                              <td>{7}</td>
                                              <td>{8}</td>
                                             <td>{9}</td>
                                              <td>{10}</td>
                                              <td>{11}</td>
                                             <td>{12}</td>
                                              <td>{13}</td>
                                              <td>{14}</td>
                                              <td>{15}</td>
                                              <td>{16}</td>
                                             <td>{17}</td>
                                              <td>{18}</td>
                                              <td>{19}</td>
                                              <td>{20}</td>
                                              <td>{21}</td>
                                              <td>{22}</td>
                                              <td>{23}</td>
                                              <td>{24}</td>
                                              <td>{25}</td>
                                             <td>{26}</td>
                                              <td>{27}</td>
                                              <td>{28}</td>
                                       </tr>
                               ", errorcnt, t.VALIDATIONERRORSTATUS, t.MERCHANTID, t.MERCHANTNAME, t.CONTACTTITLE, t.CONTACTNAME, t.MOBILEPHONE,
                                   t.EMAIL, t.EMAILALERTS, t.PHYSICALADDR, t.TERMINALMODELCODE, t.TERMINALID, t.BANKCODE, t.BANKTYPE,
                                   t.SLIPHEADER, t.SLIPFOOTER, t.BUISNESSOCCUPATIONCODE, t.MERCHANTCATEGORYCODE, t.STATECODE, t.VISAACQUIRERID,
                                   t.VERVEACQUIRERID, t.MASTERCARDACQUIRERID, t.TERMINALOWNERCODE, t.LGA_LCDA,
                                   t.BANK_URL, t.ACCOUNTNAME, t.PTSP, t.TRANSCURRENCY, t.PTSA);

                                continue;
                            }

                            if (grst > 0)
                            {
                                if (t.VALIDATIONERRORSTATUS == false)
                                {
                                    var obj = new SM_UPMERCHANTUPDATEUPLD()// SM_MERCHANTTERMINALUPLD()
                                    {
                                        ACCOUNTNAME = t.ACCOUNTNAME,
                                        BANKACCNO = t.BANKACCNO,
                                        BANKCODE = t.BANKCODE,

                                        BANK_URL = t.BANK_URL,
                                        BATCHID = batchId,
                                        CREATEDATE = curDate,
                                        BUISNESSOCCUPATIONCODE = t.BUISNESSOCCUPATIONCODE,


                                        EMAIL = t.EMAIL,
                                        LGA_LCDA = t.LGA_LCDA,
                                        MASTERCARDACQUIRERID = t.MASTERCARDACQUIRERID,
                                        MERCHANTCATEGORYCODE = t.MERCHANTCATEGORYCODE,
                                        MERCHANTID = t.MERCHANTID,

                                        MOBILEPHONE = t.MOBILEPHONE,
                                        PTSP = t.PTSP,
                                        CONTACTNAME = !string.IsNullOrEmpty(t.CONTACTNAME) ? t.CONTACTNAME.Replace("'", " ") : null,
                                        BANKTYPE = !string.IsNullOrEmpty(t.BANKTYPE) ? int.Parse(t.BANKTYPE) : (int?)null,
                                        EMAILALERTS = !string.IsNullOrEmpty(t.EMAILALERTS) ? t.EMAILALERTS.ToUpper() : null,
                                        CONTACTTITLE = !string.IsNullOrEmpty(t.CONTACTTITLE) ? t.CONTACTTITLE.Replace("'", " ") : null,
                                        MERCHANTNAME = !string.IsNullOrEmpty(t.MERCHANTNAME) ? t.MERCHANTNAME.Replace("'", " ") : null,
                                        PHYSICALADDR = !string.IsNullOrEmpty(t.PHYSICALADDR) ? t.PHYSICALADDR.Replace("'", " ") : null,

                                        SLIPFOOTER = !string.IsNullOrEmpty(t.SLIPFOOTER) ? t.SLIPFOOTER.Replace("'", " ") : null,
                                        SLIPHEADER = !string.IsNullOrEmpty(t.SLIPHEADER) ? t.SLIPHEADER.Replace("'", " ") : null,
                                        TERMINALMODELCODE = !string.IsNullOrEmpty(t.TERMINALMODELCODE) ? int.Parse(t.TERMINALMODELCODE) : (int?)null,
                                        STATECODE = t.STATECODE,
                                        STATUS = unapprove,
                                        TERMINALID = t.TERMINALID,

                                        TERMINALOWNERCODE = t.TERMINALOWNERCODE,
                                        USERID = User.Identity.Name,
                                        VERVEACQUIRERID = t.VERVEACQUIRERID,
                                        VISAACQUIRERID = t.VISAACQUIRERID,
                                        INSTITUTION_ID = instid,
                                        GLOBAL_ITBID = objGUp.ITBID,
                                        MAKER_INST_ID = institutionId,
                                        // INTERFACE_FORMAT = rdInterdaceType.SelectedValue,
                                        PROCESSOR = null,// chkProcessor.SelectedValue, // ? "N" : "U",
                                        INST_AUTH_ID = User.Identity.Name,
                                        //PAYATTITUDE_ACCEPTANCE = t.PAYATTITUDE_ACCEPTANCE,
                                        PTSA = t.PTSA,
                                        TRANSCURRENCY = t.TRANSCURRENCY,

                                    };
                                    repoUP.Insert(obj);
                                    cnt++;
                                }

                            }

                        }
                    }
                    else
                    {
                        int valCount = rec.Count(f => f.VALIDATIONERRORSTATUS == false);
                        foreach (var t in rec)
                        {
                            //var t = rec.Find(d => d.ITBID == itbid);
                            if (i == 0)
                            {
                                objG = new SM_MERTERUPDUPLDGLO();
                                objG.BATCHID = batchId;
                                objG.CREATEDATE = curDate;
                                objG.BATCHCOUNT = valCount;
                                objG.STATUS = unapprove;
                                objG.USERID = User.Identity.Name;
                                objG.INSTITUTION_ID = instid;
                                objG.MAKER_INST_ID = institutionId;
                                repoUpldGlo.Insert(objG);
                                grst = uow.Save(User.Identity.Name);

                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = eventInsert,
                                    MENUID = menuId,
                                    //MENUNAME = "",
                                    //  RECORDID = objG.ITBID,
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
                            //if (t.ValidationErrorStatus == false && string.IsNullOrEmpty(rdInterdaceType.SelectedValue))
                            //{
                            //    pnlResponse.Visible = true;
                            //    pnlResponseMsg.Text = "Interface Type is not selected for some validated records";
                            //    pnlResponse.CssClass = "alert alert-warning alert-dismissable alert-bold";
                            //    return;
                            //}
                            if (t.VALIDATIONERRORSTATUS == true)
                            {
                                errorcnt++;
                                sb.AppendFormat(@"
                                        <tr>
                                            <td>{0}</td>
                                            <td>{1}</td>
                                               <td>{2}</td>
                                              <td>{3}</td>
                                                <td>{4}</td>
                                               <td>{5}</td>
                                              <td>{6}</td>
                                              <td>{7}</td>
                                              <td>{8}</td>
                                             <td>{9}</td>
                                              <td>{10}</td>
                                              <td>{11}</td>
                                             <td>{12}</td>
                                              <td>{13}</td>
                                              <td>{14}</td>
                                              <td>{15}</td>
                                              <td>{16}</td>
                                             <td>{17}</td>
                                              <td>{18}</td>
                                              <td>{19}</td>
                                              <td>{20}</td>
                                              <td>{21}</td>
                                              <td>{22}</td>
                                              <td>{23}</td>
                                              <td>{24}</td>
                                              <td>{25}</td>
                                             <td>{26}</td>
                                              <td>{27}</td>
                                              <td>{28}</td>
                                       </tr>
                               ", errorcnt, t.VALIDATIONERRORMESSAGE, t.MERCHANTID, t.MERCHANTNAME, t.CONTACTTITLE, t.CONTACTNAME, t.MOBILEPHONE,
                                   t.EMAIL, t.EMAILALERTS, t.PHYSICALADDR, t.TERMINALMODELCODE, t.TERMINALID, t.BANKCODE, t.BANKTYPE,
                                   t.SLIPHEADER, t.SLIPFOOTER, t.BUISNESSOCCUPATIONCODE, t.MERCHANTCATEGORYCODE, t.STATECODE, t.VISAACQUIRERID,
                                   t.VERVEACQUIRERID, t.MASTERCARDACQUIRERID, t.TERMINALOWNERCODE, t.LGA_LCDA,
                                   t.BANK_URL, t.ACCOUNTNAME, t.PTSP, t.TRANSCURRENCY, t.PTSA);

                                continue;
                            }

                            if (grst > 0)
                            {
                                if (t.VALIDATIONERRORSTATUS == false)
                                {
                                    var obj = new SM_MERCHANTUPDATEUPLD()
                                    {
                                        ACCOUNTNAME = t.ACCOUNTNAME,
                                        BANKACCNO = t.BANKACCNO,
                                        BANKCODE = t.BANKCODE,
                                        // BANKTYPE = int.Parse(t.BANKTYPE),
                                        BANK_URL = t.BANK_URL,
                                        BATCHID = batchId,
                                        BUISNESSOCCUPATIONCODE = t.BUISNESSOCCUPATIONCODE,
                                        CONTACTNAME = !string.IsNullOrEmpty(t.CONTACTNAME) ? t.CONTACTNAME.Replace("'", " ") : null,
                                        BANKTYPE = !string.IsNullOrEmpty(t.BANKTYPE) ? int.Parse(t.BANKTYPE) : (int?)null,
                                        EMAILALERTS = !string.IsNullOrEmpty(t.EMAILALERTS) ? t.EMAILALERTS.ToUpper() : null,
                                        CONTACTTITLE = !string.IsNullOrEmpty(t.CONTACTTITLE) ? t.CONTACTTITLE.Replace("'", " ") : null,
                                        MERCHANTNAME = !string.IsNullOrEmpty(t.MERCHANTNAME) ? t.MERCHANTNAME.Replace("'", " ") : null,
                                        PHYSICALADDR = !string.IsNullOrEmpty(t.PHYSICALADDR) ? t.PHYSICALADDR.Replace("'", " ") : null,

                                        SLIPFOOTER = !string.IsNullOrEmpty(t.SLIPFOOTER) ? t.SLIPFOOTER.Replace("'", " ") : null,
                                        SLIPHEADER = !string.IsNullOrEmpty(t.SLIPHEADER) ? t.SLIPHEADER.Replace("'", " ") : null,
                                        TERMINALMODELCODE = !string.IsNullOrEmpty(t.TERMINALMODELCODE) ? int.Parse(t.TERMINALMODELCODE) : (int?)null,
                                        //CONTACTNAME = t.CONTACTNAME.Replace("'", " "),
                                        CREATEDATE = curDate,
                                        // CONTACTTITLE = t.CONTACTTITLE.Replace("'", " "),
                                        EMAIL = t.EMAIL,
                                        // EMAILALERTS = t.EMAILALERTS.ToUpper(),
                                        LGA_LCDA = t.LGA_LCDA,
                                        MASTERCARDACQUIRERID = t.MASTERCARDACQUIRERID,
                                        MERCHANTCATEGORYCODE = t.MERCHANTCATEGORYCODE,
                                        MERCHANTID = t.MERCHANTID,
                                        // MERCHANTNAME = t.MERCHANTNAME.Replace("'", " "),
                                        MOBILEPHONE = t.MOBILEPHONE,
                                        // PHYSICALADDR = t.PHYSICALADDR.Replace("'", " "),
                                        PTSP = t.PTSP,
                                        //SLIPFOOTER = t.SLIPHEADER.Replace("'", " "),
                                        //SLIPHEADER = t.SLIPHEADER.Replace("'", " "),
                                        STATECODE = t.STATECODE,
                                        STATUS = unapprove,
                                        TERMINALID = t.TERMINALID,
                                        //TERMINALMODELCODE = int.Parse(t.TERMINALMODELCODE),
                                        TERMINALOWNERCODE = t.TERMINALOWNERCODE,
                                        USERID = User.Identity.Name,
                                        VERVEACQUIRERID = t.VERVEACQUIRERID,
                                        VISAACQUIRERID = t.VISAACQUIRERID,
                                        INSTITUTION_ID = instid,
                                        GLOBAL_ITBID = objG.ITBID,
                                        MAKER_INST_ID = institutionId,
                                        //INTERFACE_FORMAT = rdInterdaceType.SelectedValue,

                                        PTSA = t.PTSA,
                                        TRANSCURRENCY = t.TRANSCURRENCY
                                    };
                                    repoUpld.Insert(obj);
                                    cnt++;
                                }

                            }

                        }
                    }

                    sb.AppendLine(@" </tbody>
                                </table>");
                    //foreach (var t in rec)
                    //{

                    //    if (cnt == 0)
                    //    {
                    //        objG = new SM_MERTERUPLDGLO();
                    //        objG.BATCHID = batchId;
                    //        objG.CREATEDDATE = curDate;
                    //        objG.BATCHCOUNT = rec.Count;
                    //        objG.STATUS = unapprove;
                    //        objG.USERID = userId;
                    //        objG.INSTITUTION_ID = instid;
                    //        objG.MAKER_INST_ID = userInstitutionItbid;
                    //        repoUpldGlo.Insert(objG);
                    //        grst = uow.Save(userId);

                    //        SM_AUTHLIST auth = new SM_AUTHLIST()
                    //        {
                    //            CREATEDDATE = DateTime.Now,
                    //            EVENTTYPE = eventInsert,
                    //            MENUID = menuid,
                    //            //MENUNAME = "",
                    //            RECORDID = objG.ITBID,
                    //            STATUS = open,
                    //            //TABLENAME = "SM_FREQUENCY",
                    //            URL = Request.FilePath,
                    //            USERID = userId,
                    //            INSTITUTION_ITBID = userInstitutionItbid,
                    //            BATCHID = batchId,
                    //            POSTTYPE = Batch
                    //        };
                    //        repoAuth.Insert(auth);
                    //    }
                    //    if (grst > 0)
                    //    {
                    //        var obj = new SM_MERCHANTTERMINALUPLD()
                    //        {
                    //            ACCOUNTNAME = t.ACCOUNTNAME,
                    //            BANKACCNO = t.BANKACCNO,
                    //            BANKCODE = t.BANKCODE,
                    //            BANKTYPE = int.Parse(t.BANKTYPE),
                    //            BANK_URL = t.BANK_URL,
                    //            BATCHID = batchId,
                    //            BUISNESSOCCUPATIONCODE = t.BUISNESSOCCUPATIONCODE,
                    //            CONTACTNAME = t.CONTACTNAME,
                    //            CREATEDDATE = curDate,
                    //            CONTACTTITLE = t.CONTACTTITLE,
                    //            EMAIL = t.EMAIL,
                    //            EMAILALERTS = t.EMAILALERTS.ToUpper(),
                    //            LGA_LCDA = t.LGA_LCDA,
                    //            MASTERCARDACQUIRERIDNUMBER = t.MASTERCARDACQUIRERIDNUMBER,
                    //            MERCHANTCATEGORYCODE = t.MERCHANTCATEGORYCODE,
                    //            MERCHANTID = t.MERCHANTID,
                    //            MERCHANTNAME = t.MERCHANTNAME,
                    //            MOBILEPHONE = t.MOBILEPHONE,
                    //            PHYSICALADDR = t.PHYSICALADDR,
                    //            PTSP = t.PTSP,
                    //            SLIPFOOTER = t.SLIPHEADER,
                    //            SLIPHEADER = t.SLIPHEADER,
                    //            STATECODE = t.STATECODE,
                    //            STATUS = unapprove,
                    //            TERMINALID = t.TERMINALID,
                    //            TERMINALMODELCODE = int.Parse(t.TERMINALMODELCODE),
                    //            TERMINALOWNERCODE = t.TERMINALOWNERCODE,
                    //            USERID = userId,
                    //            VERVEACQUIRERIDNUMBER = t.VERVEACQUIRERIDNUMBER,
                    //            VISAACQUIRERIDNUMBER = t.VISAACQUIRERIDNUMBER,
                    //            INSTITUTION_ID = instid,
                    //            GLOBAL_ITBID = objG.ITBID,
                    //            MAKER_INST_ID = userInstitutionItbid
                    //    };
                    //        repoUpld.Insert(obj);

                    //    }
                    //    // end of for each loop
                    //    cnt++;

                    //}
                    var rst = uow.Save(User.Identity.Name);
                    if (rst > 0)
                    {

                        rv.PurgeRevenueHead(User.Identity.Name);
                        try
                        {
                            var rs = rec.Count - cnt;
                            if (rs != 0)
                            {
                                SendUploadErrorNotification(sb.ToString(), rs, batchId, fullName);
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, string.Format("Merchant Upload Record Approval Batch #{0}", batchId));
                        }
                        catch
                        {

                        }


                        //txscope.Complete();
                    }
                    else
                    {

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
            private static MerchantUpldObj addRecord(IList<string> rowData,IList<string> columnNames)
            {
                try
                {
                    var obj = new MerchantUpldObj()
                    {
                        MERCHANTID = rowData[0].Trim(),
                        MERCHANTNAME = rowData[1],
                        CONTACTTITLE = rowData[2],
                        CONTACTNAME = rowData[3].Trim(),
                        MOBILEPHONE = rowData[4].Trim(),
                        EMAIL = rowData[5].Trim(),
                        EMAILALERTS = rowData[6].Trim(),
                        PHYSICALADDR = rowData[7].Trim(),
                        TERMINALMODELCODE = rowData[8],
                        TERMINALID = rowData[9].Trim(),
                        BANKCODE = rowData[10].Trim(),
                        BANKACCNO = rowData[11].Trim(),
                        BANKTYPE = rowData[12],
                        SLIPFOOTER = rowData[13],
                        SLIPHEADER = rowData[14],
                        BUISNESSOCCUPATIONCODE = rowData[15],
                        MERCHANTCATEGORYCODE = rowData[16].Trim(),
                        STATECODE = rowData[17].Trim(),
                        VISAACQUIRERID = rowData[18],
                        VERVEACQUIRERID = rowData[19],
                        MASTERCARDACQUIRERID = rowData[20],
                        TERMINALOWNERCODE = rowData[21].Trim(),
                        LGA_LCDA = rowData[22].Trim(),
                        BANK_URL = rowData[23],
                        ACCOUNTNAME = rowData[24].Trim(),
                        PTSP = rowData[25].Trim(),
                        TRANSCURRENCY = rowData[26].Trim(),
                        PTSA = rowData[27].Trim(),
                        // PAYATTITUDE_ACCEPTANCE = rowData[28].Trim(),
                        VALIDATIONERRORSTATUS = true,
                    };
                    return obj;
                }
                catch (Exception ex)
                {
                    return new MerchantUpldObj();
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
                                case "XP":
                                    {
                                        ViewBag.HeaderTitle = "Authorize Detail for Merchant Registeration";
                                        viewtoDisplay = "DetailAuthXP";
                                        var rec = _repo.GetMerchantUpdateUploadTemp(det.BATCHID, frmType, det.USERID, null);
                                        if (rec != null && rec.Count > 0)
                                        {
                                            var model = rec.FirstOrDefault();

                                            obj.Status = det.STATUS;
                                            obj.EventType = det.EVENTTYPE;
                                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                            obj.User = model.CREATED_BY;
                                            ViewBag.Institution = model.INSTITUTION_NAME;
                                            ViewBag.Auth = obj;
                                            ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                            return View(viewtoDisplay, rec);
                                        }
                                        break;
                                    }
                                case "BK":
                                    {
                                        ViewBag.HeaderTitle = "Authorize Detail for Merchant Registeration";
                                        viewtoDisplay = "DetailAuthXP";
                                        var rec = _repo.GetMerchantUpdateUploadTemp(det.BATCHID, frmType, det.USERID, null);
                                        if (rec != null && rec.Count > 0)
                                        {
                                            var model = rec.FirstOrDefault();

                                            obj.Status = det.STATUS;
                                            obj.EventType = det.EVENTTYPE;
                                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                                            obj.User = det.CREATEDATE.GetValueOrDefault().ToString("dd-MM-yyyy");
                                            ViewBag.Auth = obj;
                                            ViewBag.DisplayAuth = det.STATUS == open && !(det.USERID == User.Identity.Name);

                                            return View(viewtoDisplay, rec);
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        viewtoDisplay = "DetailAuthXP";
                                        break;
                                    }
                            }

                            //  return Json(rec, JsonRequestBehavior.AllowGet);
                            //var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                            // return Json(obj1, JsonRequestBehavior.AllowGet);
                        }


                        return View("DetailAuthXP");
                    }
                    else
                    {
                        //bad request
                        return View("Error", "Home");
                    }

                }
                catch (Exception ex)
                {
                    return View("DetailAuthXP");
                }
            }
            int checkerNo = 1;
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult ApproveBK(decimal AuthId, int? m)
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
                                    var sp = rec2.BATCHID.Split('_');
                                    var frmType = sp[0];
                                    recordId = (int)rec2.RECORDID.GetValueOrDefault();
                                    menuId = rec2.MENUID.GetValueOrDefault();
                                    switch (rec2.EVENTTYPE)
                                    {
                                        case "New":
                                            {

                                                if (frmType == "XP")
                                                {
                                                    suc = ForwardToXP(rec2.BATCHID, rec2.USERID);
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
                                        var t = uow.Save(User.Identity.Name);
                                        if (t > 0)
                                        {
                                           // txscope.Complete();
                                            //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
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
                   // }

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

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult ApproveXP(decimal AuthId, int? m)
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
                    int recordId = 0; int? inst_id = 0;
                    string bid = "";
                    bool suc = false;
                    //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(2, 0, 0)))
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
                                    bid = rec2.BATCHID;
                                    var sp = bid.Split('_');
                                    var frmType = sp[0];
                                    recordId = (int)rec2.RECORDID.GetValueOrDefault();
                                    menuId = rec2.MENUID.GetValueOrDefault();
                                    switch (rec2.EVENTTYPE)
                                    {
                                        case "Modify":
                                            {

                                                if (frmType == "XP")
                                                {
                                                    suc = PostBulkUpload(rec2.BATCHID, rec2.USERID, out inst_id);
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
                                        var t = uow.Save(User.Identity.Name);
                                        if (t > 0)
                                        {

                                            try
                                            {
                                                EmailerNotification.SendMerchantUploadApprovalRejectionMail("", "Merchant Upload", approve, "Merchant Upload", "", bid, "", GetInstInputer_AuthorizerEmail(bid), inst_id);
                                            }
                                            catch (Exception ex)
                                            { }
                                            //txscope.Complete();
                                            //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
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

            private List<EmailObj> GetInstInputer_AuthorizerEmail(string batchId)
            {
                List<EmailObj> lst = new List<EmailObj>();
                var rec = repoUP.AllEager(g => g.BATCHID == batchId).FirstOrDefault();
                if (rec != null)
                {
                    var authUserId = rec.INST_AUTH_ID;
                    var makerUserId = rec.USERID;
                    var authRec = repoUsers.AllEager(f => f.UserName == rec.INST_AUTH_ID).SingleOrDefault();
                    var makerRec = repoUsers.AllEager(f => f.UserName == rec.USERID).SingleOrDefault();
                    if (authRec != null)
                    {
                        lst.Add(new EmailObj()
                        {
                            FULLNAME = authRec.FullName,
                            Email = authRec.Email
                        });
                    }
                    if (makerUserId != null)
                    {
                        lst.Add(new EmailObj()
                        {
                            FULLNAME = makerRec.FullName,
                            Email = makerRec.Email
                        });
                    }
                }

                return lst;
            }

            [HttpPost]
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
                                    //var recc = repoRoleTemp.Find(recordId);
                                    //if (recc != null)
                                    //{
                                    //    recc.STATUS = reject;
                                    //}

                                    rec2.STATUS = reject;
                                    var t = uow.Save(User.Identity.Name);
                                    if (t > 0)
                                    {
                                        //txscope.Complete();
                                        //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
                                        //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                                        respMsg = "Record Rejected. A mail has been sent to the user.";
                                        // TempData["msg"] = respMsg;
                                        //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                        return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });
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
                        return Json(new { RespCode = 1, RespMessage = respMsg });
                    //}

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