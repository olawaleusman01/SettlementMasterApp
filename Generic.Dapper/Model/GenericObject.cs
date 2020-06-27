using Generic.Data;
using Generic.Data.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Generic.Dapper.Model
{
    public class UserObj
    {
        public int ItbId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool Supervisor { get; set; }
        public string FullName
        {
            get { return string.Concat(LastName, " ", FirstName); }
        }
        public string UserName { get; set; }
        public int InstitutionId { get; set; }
        public string INSTITUTION_NAME { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string Email
        {
            get; set;
        }
        public int RoleId { get; set; }
        public bool ForcePassword { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public bool LoggedOn { get; set; }
        public string MobileNo { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Status { get; set; }
        public string RoleName { get; set; }
        public int EnforcePasswordChangeDays { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public string CreateUserId { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CreateDate != null ? CreateDate.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }

    }
    public class OutPutObj
    {
        public int RespCode { get; set; } = -1;
        public string RespCode2 { get; set; }
        public string RespMessage { get; set; }
    }
    public class MailGroupObj : SM_MAILGROUPTEMP
    {
        public string FULLNAME { get; set; }
    }
    public class MerchantObj : SM_MERCHANTDETAILTEMP
    {
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string INSTITUTION_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class MerchantAcctObj : SM_MERCHANTACCT
    {
        public string PID { get; set; }
        public decimal? DB_ITBID { get; set; }
        public string DESCRIPTION { get { return string.Concat(DEPOSIT_ACCOUNTNO, "-", DEPOSIT_ACCTNAME); } }
        public string MERCHANTNAME { get; set; }
        public string CREATED_BY { get; set; }
        public string CURRENCYDESC { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
        public bool DefaultAccount { get; set; } = false;
        public bool VisibleFlag { get; set; }
        public string EVENTTYPE { get; set; }
    }
    public class InstitutionAcctObj : SM_INSTITUTIONACCT
    {
        public string CardSchemDesc { get; set; }
        public string InstitutionTypeDesc { get; set; }
        public string EVENTTYPE { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
        public decimal? DefaultAccount { get; set; }

    }
    public class PartyAcctObj : SM_PARTYACCOUNT
    {
        public string CardSchemDesc { get; set; }
        public string EVENTTYPE { get; set; }
        public short DefaultAccount { get; set; }
        //public string InstitutionTypeDesc { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }

    }
    public class SettlementRuleObj : SM_SETTLEMENTRULE
    {
        public string CardSchemDesc { get; set; }
        public string EVENTTYPE { get; set; }
        public short DefaultAccount { get; set; }
        //public string InstitutionTypeDesc { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }

    }
    public class RvGroupObj : SM_REVENUEGROUP
    {
        public string MERCHANTNAME { get; set; }
        public string DEPOSIT_BANKCODE { get; set; }
        public string DEPOSIT_ACCOUNTNO { get; set; }
        public string DEPOSIT_BANKNAME { get; set; }
        public string DEPOSIT_COUNTRYCODE { get; set; }
        public string DEPOSIT_ACCTNAME { get; set; }
        public bool CUSTOM { get; set; }
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    
    }
    public class RvHeadObj : SM_REVENUEHEAD
    {
        public string PID { get; set; }
        public int? DB_ITBID { get; set; }
        public string MID { get; set; }
        public string RvGroupName { get; set; }
        public string FREQUENCY_DESC { get; set; }
        public string BANK_NAME { get; set; }
        public string EVENTTYPE { get; set; }
        public short DefaultAccount { get; set; }
        //public string InstitutionTypeDesc { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
        public string BATCHID { get; set; }

        public List<RevenueSharingPartyObj> RevenueSharingPartys {get; set;}

    }

    public class RevenueSharingPartyObj
    {
        public int ItbId { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public decimal PartyValue { get; set; }
        public int PartyAccountId { get; set; }
        public string PartyAccountName { get; set; }
    }


    public class ApprovalRouteObj : SM_APPROVAL_ROUTE
    {
        public string CREATED_BY { get; set; }
    }
    public class ApprovalRouteOffObj : SM_APPROVAL_ROUTE_OFFICER
    {
        public string PID { get; set; }
        public int? DB_ITBID { get; set; }
        public string EVENTTYPE { get; set; }
        public string FULLNAME { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }

    }
    public class RolePrivilegeObj
    {
        public int MenuId { get; set; }
        public int RoleAssigId { get; set; }
        public string MenuName { get; set; }
        public string RoleName { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit
        {
            get; set;
        }
        public bool CanDelete { get; set; }
        public bool CanAdd { get; set; }
        public bool CanAuthorize { get; set; }
        public int ParentId { get; set; }
        public string USERID { get; set; }
        public string CREATEDBY { get; set; }

    }
    public class UserPrivilege
    {
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit
        {
            get; set;
        }
        public bool CanDelete { get; set; }
        public bool CanAdd { get; set; }
        public bool CanAuthorize { get; set; }

    }
    [NotMapped]
    public class RolesObj : SM_ROLESTEMP
    {
        public string User_FullName { get; set; }
        public string DateString { get; set; }
    }
    [NotMapped]

    public class RolesPrivObj
    {

        public int RoleAssigId { get; set; }
        public int MenuId { get; set; }

        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanAuthorize { get; set; }

    }

    public class RolesPrivObj2
    {
        public int RoleId { get; set; }
        public List<RolesPrivObj> RolePrivList { get; set; }
    }
    public class CompanyProfileObj : SM_COMPANY_PROFILETEMP
    {
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string PROCESS_FLAG { get; set; }
        public string PROCESS_FLAG_NAP { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class UserObj2 : SM_ASPNETUSERSTEMP
    {
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CreateDate != null ? CreateDate.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class CardSchemeObj : SM_CARDSCHEMETEMP
    {

        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class CurrencyObj : SM_CURRENCYTEMP
    {
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class ChannelObj : SM_CHANNELSTEMP
    {
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }

    public class SERVICEChannelObj : SM_SERVICECHANNELSTEMP
    {
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class FrequencyObj : SM_FREQUENCYTEMP
    {

        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class BankTypeObj : SM_BANKTYPE
    {
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class CountryObj : SM_COUNTRYTEMP
    {

        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string CURRENCY_NAME { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class StateObj : SM_STATETEMP
    {
        public string COUNTRYNAME { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class CityObj : SM_STATETEMP
    {
        public string COUNTRYNAME { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class AuthViewObj
    {
        public decimal AuthId { get; set; }
        public decimal RecordId { get; set; }
        public string BatchId { get; set; }
        public string PostType { get; set; }
        public int MenuId { get; set; }
        public string Status { get; set; }
        public string EventType { get; set; }
        public string DateCreated { get; set; }
        public string User { get; set; }

    }

    public class DepartmentObj : SM_DEPARTMENTTEMP
    {
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }

    public class SectorObj : SM_SECTOR
    {
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }
    public class PartyTypeObj : SM_PARTYTYPETEMP
    {
        bool _PartyCodeReq;
        public bool PartyCodeReq
        {
            get
            {
                _PartyCodeReq = PARTYCODEREQUIRED == "Y" || PARTYCODEREQUIRED == "y";
                return _PartyCodeReq;
            }
            //set { _PartyCodeReq = value  ; }
        }
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }
    public class PartyObj : SM_PARTYTEMP
    {
        //bool _PartyCodeReq;
        public string PARTYCODEREQUIRED { get; set; }
        public bool PartyCodeReq { get { return PARTYCODEREQUIRED == "Y" || PARTYCODEREQUIRED == "y"; } /*set { _PartyCodeReq = value ; }*/ }
        public string PARTYTYPE_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }
    public class SettlementOptionObj : SM_SETTLEMENTOPTIONTEMP
    {
        //bool _PartyCodeReq;
        public string CHANNEL_DESC { get; set; }
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }
    public class InstitutionObj : SM_INSTITUTIONTEMP
    {
        // bool _chkAcquirer;
        //public bool chkAcquirer { get { return IS_BANK == "" ? true : false; } set { chkAcquirer = value; } }
        // public bool chkPtsp { get { return IS_BANK == "" ? true : false; } set { chkIsBank = value; } }
        // public bool chkIsBank { get { return IS_BANK == "" ? true : false; } set { chkIsBank = value; } }
        public bool chkPtsp { get { return PTSP == "Y" || PTSP == "y"; } /*set { _PartyCodeReq = value ; }*/ }
        public bool chkIsBank { get { return IS_BANK == "Y" || IS_BANK == "y"; } /*set { _PartyCodeReq = value ; }*/ }
        public bool chkAcquirer { get { return IS_ACQUIRER == "Y" || IS_ACQUIRER == "y"; } /*set { _PartyCodeReq = value ; }*/ }

        public string CURRENCY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }

    }

    public class MCCObj : SM_MCCTEMP
    {
        public string SECTOR_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string MODIFIED_BY { get; set; }

        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }

    public class MCCMSCObj : SM_MCCMSCTEMP
    {
        public string USER_FULLNAME { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }

    public class MccMscObj : SM_MCCMSC
    {
        public decimal RECORDID { get; set; }
        public string DisplayIntL { get { return ITBID == 0 ? "none" : "block"; } }
        public bool SET_INTL { get; set; }
        public string EVENTTYPE { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public string ChannelDesc { get; set; }
        public int ACQFLAG { get; set; }
        public string Institution_Desc { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Institution_ShortName { get; set; }
        public string IntFrequencyDesc { get; set; }
        public string DomFrequencyDesc { get; set; }
        public string IntCurrencyDesc { get; set; }
        public string DomCurrencyDesc { get; set; }
        public string INSTITUTION_NAME { get; set; }
        //  public string Institution { get { return string.Concat(currencyn}; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }

    }
    public class SharingPartyObj
    {
        public decimal ITBID { get; set; }
        public decimal CHANNEL { get; set; }
        public string MERCHANTID { get; set; }
        public Nullable<int> PARTYITBID { get; set; }
        public string PARTY_LOCATOR { get; set; }
        public Nullable<decimal> SHARINGVALUE { get; set; }
        public string TRANTYPE { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string STATUS { get; set; }
        public string LASTMODIFIED_UID { get; set; }
        public string BATCHID { get; set; }
        public Nullable<decimal> RECORDID { get; set; }
        public string CARDSCHEME { get; set; }
        public string BANKCODE { get; set; }
        public string BANKACCOUNT { get; set; }
        public Nullable<decimal> MERCHANTMSC_ITBID { get; set; }
        public Nullable<decimal> MCCMSC_ITBID { get; set; }
        public Nullable<decimal> CAP { get; set; }
        public Nullable<decimal> ACCOUNT_ID { get; set; }
        public Nullable<decimal> ACCOUNT_ID2 { get; set; }
        public string PARTYTYPE_CODE { get; set; }
        public string PARTYTYPE_DESC { get; set; }
        public string BILLER_CODE { get; set; }
        public string PARTY_ID { get; set; }
        public string PartyName { get; set; }
        public string MerchantName { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public string PID { get; set; }
        public string EVENTTYPE { get; set; }
        public decimal? DB_ITBID { get; set; }
        public decimal? BILLERMSC_ITBID { get; set; }
        public decimal? sharingRateAccount1 { get; set; }
        public decimal? sharingRateAccount2 { get; set; }
        public bool splitincome { get; set; }
    }
    public class MerchantUpldObj
    {
        public int ITBID { get; set; }
        public string PID { get; set; }
        public string TRANSCURRENCY { get; set; }
        public string INSTITUTION_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public string MERCHANTID { get; set; }
        public string MERCHANTNAME { get; set; }
        public string CONTACTTITLE { get; set; }
        public string CONTACTNAME { get; set; }
        public string MOBILEPHONE { get; set; }
        public string EMAIL { get; set; }
        public string EMAILALERTS { get; set; }
        public string PHYSICALADDR { get; set; }
        public string TERMINALMODELCODE { get; set; }
        public string TERMINALID { get; set; }
        public string BANKCODE { get; set; }
        public string BANKACCNO { get; set; }
        public string BANKTYPE { get; set; }
        public string SLIPHEADER { get; set; }
        public string SLIPFOOTER { get; set; }
        public string BUISNESSOCCUPATIONCODE { get; set; }
        public string MERCHANTCATEGORYCODE { get; set; }
        public string STATECODE { get; set; }
        public string VISAACQUIRERID { get; set; }
        public string VERVEACQUIRERID { get; set; }
        public string MASTERCARDACQUIRERID { get; set; }
        public string TERMINALOWNERCODE { get; set; }
        public string LGA_LCDA { get; set; }
        public string BANK_URL { get; set; }
        public string ACCOUNTNAME { get; set; }
        public string PTSP { get; set; }
        public string PTSA { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }
        public string ROWCOLOR { get; set; }
        public int GROUPLABEL { get; set; }
    }
    public class DataBinUpldObj
    {
        public decimal ITBID { get; set; }
        public string PID { get; set; }
        public string CBNCODE { get; set; }
        public string CARDSCHEME { get; set; }
        public string BIN { get; set; }
        public string COUNTRYCODE { get; set; }
        public string CURRENCYCODE { get; set; }
        public decimal? BANKFIID { get; set; }
        public string BUSINESSTYPE { get; set; }
        public string ISSUERFIID { get; set; }
        public string USERID { get; set; }
        public int POSTSEQUENCE { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string STATUS { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }
    }

    public class RvHeadUpldObj:SM_REVENUEHEAD
    {
        // public decimal ITBID { get; set; }
        public string PID { get; set; }
        public string ID { get; set; }
        public string GROUPCODE { get; set; }
        public string RVCODE { get; set; }
        //public string DESCRIPTION { get; set; }
        //public string USERID { get; set; }
        public int POSTSEQUENCE { get; set; }
        //public DateTime? CREATEDATE { get; set; }
        //public string STATUS { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }
    }
    public class RvDrAcctObj : SM_REVENUEBANKACCT
    {
       public string BANKNAME { get; set; }
    }
    public class RvDrAcctUpldObj:SM_REVENUEBANKACCT
    {
        //public decimal ITBID { get; set; }
        public string PID { get; set; }
        //public string RVGROUPCODE { get; set; }
       // public string AGENTCODE { get; set; }
        public string BANKCODE { get; set; }
        public string BANKACCTNO { get; set; }
        public string BANKACCTNAME { get; set; }
        //public string MERCHANTID { get; set; }
        //public string USERID { get; set; }
        public int POSTSEQUENCE { get; set; }
        //public DateTime? CREATEDATE { get; set; }
        //public string STATUS { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }
    }
    public class LastMidObj
    {
        public string MERCHANTID { get; set; }
        public string TERMINALID { get; set; }
        public string MERCHANTNAME { get; set; }
        public string INSTITUTION_CBNCODE { get; set; }
        public string STATECODE { get; set; }
    }

    public class mRegObj
    {
        public MerchantObj mObj { get; set; }
        public List<MerchantAcctObj> mAcctObj { get; set; }
        public List<TerminalObj> mTObj { get; set; }
    }
    public class BillerObj : SM_BILLER
    {
        public string CHANNEL_DESC { get; set; }
        public string MERCHANTNAME { get; set; }

        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public string AUTH_BY { get; set; }
        public string CREATEDATE_STRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string MODIFIEDDATE_STRING { get { return LAST_MODIFIED_DATE != null ? LAST_MODIFIED_DATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class BillerMscObj : SM_BILLERMSC
    {
        public string USER_FULLNAME { get; set; }
        public string AUTH_FULLNAME { get; set; }
        public string LAST_MOD_FULLNAME { get; set; }
        public string LAST_MOD_DATE { get; set; }
        public string CREATE_DATE { get; set; }
    }
    public class mBillerObj
    {
        public BillerObj mObj { get; set; }
        public BillerMscObj mBillerMscObj { get; set; }
        public List<SharingPartyObj> mFee1SharingObj { get; set; }
        public List<SharingPartyObj> mMsc1SharingObj { get; set; }
    }
    public class DataBinObj : SM_DATABIN
    {
        public string INSTITUTION_NAME { get; set; }
        public string CREATED_BY { get; set; }
        public string PID { get; set; }
        public decimal? DB_ITBID { get; set; }
        public decimal? RECORDID { get; set; }
    }

    public class ExchangeRateObj : SM_EXCHANGERATE
    {
        public string INSTITUTION_NAME { get; set; }
        public string CURRENCYNAME { get; set; }
        public string NAIRA_EQUIVALENT_F { get { return NAIRA_EQUIVALENT.GetValueOrDefault().ToString("F"); } }
        public string CREATED_BY { get; set; }
        public string PID { get; set; }
        public int? DB_ITBID { get; set; }
        public int? RECORDID { get; set; }
    }

    public class NapsObj
    {
        public string PID { get; set; }
        public decimal ITBID { get; set; }
        public string DEBITACCTNO { get; set; }
        public string DEBITBANKCODE { get; set; }
        public string BENEFICIARYACCTNO { get; set; }
        public string BENEFICIARYBANKCODE { get; set; }
        public string BENEFICIARYNAME { get; set; }
        public string BENEFICIARYNARRATION { get; set; }
        public string REASON { get; set; }
        public decimal? CREDITAMOUNT { get; set; }
        public DateTime? SETTLEMENTDATE { get; set; }
        public string SETTDATE { get { return SETTLEMENTDATE != null ? SETTLEMENTDATE.GetValueOrDefault().ToString("dd-MM-yyyy") : ""; } }
        public string REQUESTTYPE { get; set; }
        public int TRAXCOUNT { get; set; }
        public string USERID { get; set; }
        public DateTime CREATEDATE { get; set; }
        public string DATECREATED { get { return CREATEDATE.ToString("dd-MM-yyyy"); } }
        public string BATCHID { get; set; }
        public string EVENTTYPE { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }
        public string DEBITACCTNO_OLD { get; set; }
        public string DEBITBANKCODE_OLD { get; set; }
        public string BENEFICIARYACCTNO_OLD { get; set; }
        public string BENEFICIARYBANKCODE_OLD { get; set; }
        public string BENEFICIARYNAME_OLD { get; set; }
        public string BENEFICIARYNARRATION_OLD { get; set; }
        public Nullable<decimal> CREDITAMOUNT_OLD { get; set; }
        public string RESPCODE { get; set; }
        public string RESPMESSAGE { get; set; }
        public string SCHEDULEID { get; set; }
        public string POSTCODE { get; set; }
        public string MERCHANTID { get; set; }
        public string UNIQUEKEY { get; set; }
        
        public bool DisplayForReprocess
        {
            get { return  POSTCODE != null && !SmartObj.NapsCode().Contains(POSTCODE) ; }
        }
    }

    public class SetReconObj
    {
        public string PAYREFNO { get; set; }
        public decimal AMOUNT { get; set; }
        public string PAYMENTDATE { get; set; }
        public string VALUEDATE { get; set; }
        //public string RECEIPTNO { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string PAYMENTMETHOD { get; set; }
        //public string TRANSACTIONSTATUS { get; set; }
        //public string DEPOSITSLIPNO { get; set; }
        public string BANKNAME { get; set; }
        //public string BRANCHNAME { get; set; }
        //public string PAYERID { get; set; }
        //public string VALUEGRANTED { get; set; }
        // public int RECONCILE { get; set; }
        public string VALIDATIONERRORMESSAGE { get; set; }
    }
    public class SetReconUpldObj
    {
        public string PID { get; set; }
        public decimal ITBID { get; set; }
        public string PAYREFNO { get; set; }
        public decimal AMOUNT { get; set; }
        public DateTime? PAYMENTDATE { get; set; }
        public DateTime? VALUEDATE { get; set; }
        //public string RECEIPTNO { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string PAYMENTMETHOD { get; set; }
        //public string TRANSACTIONSTATUS { get; set; }
        //public string DEPOSITSLIPNO { get; set; }
        public string BANKNAME { get; set; }
        //public string BRANCHNAME { get; set; }
        //public string PAYERID { get; set; }
        //public string VALUEGRANTED { get; set; }
        // public int RECONCILE { get; set; }
        public int POSTSEQUENCE { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string STATUS { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }

    }

    public class SetReconDualObj
    {
        public string REFERENCENO { get; set; }
        public string CARDTYPE { get; set; }
        public string TRANSACTIONTYPE { get; set; }
        public string TRANSACTIONDATETIME { get; set; }
        public string SETTLEMENTDATE { get; set; }
        public string MASKEDPAN { get; set; }
        public string MERCHANTID { get; set; }
        public string MERCHANTACCOUNT { get; set; }
        public string MERCHANTNAME { get; set; }
        public string MERCHANTLOCATION { get; set; }
        public string TERMINALID { get; set; }
        public decimal TRANAMOUNT { get; set; }
        public decimal AMOUNTCHARGED { get; set; }
        public decimal SETTLEMENTAMOUNT { get; set; }
        public decimal MSCRATE { get; set; }
        public string BATCHTYPE { get; set; }
        public string VALIDATIONERRORMESSAGE { get; set; }
    }
    public class SetReconDualUpldObj
    {
        public string PID { get; set; }
        public decimal ITBID { get; set; }
        public string REFERENCENO { get; set; }
        public string CARDTYPE { get; set; }
        public string TRANSACTIONTYPE { get; set; }
        public DateTime? TRANSACTIONDATETIME { get; set; }
        public DateTime? SETTLEMENTDATE { get; set; }
        public string MASKEDPAN { get; set; }
        public string MERCHANTID { get; set; }
        public string MERCHANTACCOUNT { get; set; }
        public string MERCHANTNAME { get; set; }
        public string MERCHANTLOCATION { get; set; }
        public string TERMINALID { get; set; }
        public decimal TRANAMOUNT { get; set; }
        public decimal AMOUNTCHARGED { get; set; }
        public decimal SETTLEMENTAMOUNT { get; set; }
        public decimal MSCRATE { get; set; }
        public string BATCHTYPE { get; set; }
        public int POSTSEQUENCE { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string STATUS { get; set; }
        public bool VALIDATIONERRORSTATUS { get; set; }
        public string ValidationStatusClass
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "btn btn-warning btn-xs" : "btn btn-success btn-xs";
            }

        }
        public string ValidationStatusIcon
        {
            get
            {
                return VALIDATIONERRORSTATUS ? "fa fa-times" : "fa fa-check";
            }

        }
        public string VALIDATIONERRORMESSAGE { get; set; }
        public string WARNINGMESSAGE { get; set; }

    }


    public class Approval_Route
    {
        public string ApproverId { get; set; }
        public string FullName { get; set; }
    }
    public class Approval_RouteObj
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int NoLevel { get; set; }
        public string StaffId2 { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public List<Approval_Route_DetailObj> ApprovalRoute { get; set; }
    }
    public class Approval_Route_DetailObj
    {
        public int MenuId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
    }
    public class ApproverLineObj
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
    }
    public class MApproverObj
    {
        public int ITBID { get; set; }
        public int MENUID { get; set; }
        public string MENUNAME { get; set; }
        public int NOLEVEL { get; set; }
        public string APPROVERID { get; set; }
        public string FULLNAME { get; set; }
        public string PID { get; set; }
    }
    public class SettlementEnquiryObj
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string settDate { get; set; }
        public string payRef { get; set; }
        public string tranRef { get; set; }
        public int? tranID { get; set; }
        public string crBank { get; set; }
        public string drBank { get; set; }
        public string merchantID { get; set; }
        public string merchantName { get; set; }
        public string cardScheme { get; set; }
        public Decimal? tranAmt { get; set; }
        public string acquirer { get; set; }
        public string issuer { get; set; }
        public string settAcct { get; set; }
        public string channel { get; set; }
        public string maskpan { get; set; }
        public string termID { get; set; }
        public string mlocation { get; set; }
        public string invoiceNo { get; set; }
    }


   

}
