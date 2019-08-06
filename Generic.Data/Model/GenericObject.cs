using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Data.Model
{
    //public class CurrencyObj : Currency
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}

    //public class AgentObj : Agent
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //    public string UserName { get; set; }
    //    public string FullName  { get; set; }
    //    public decimal? CreditLimit { get; set; }
    //    public decimal? DebitLimit { get; set; }
    //}

    public class RoleObj : SM_ROLES
    {
        public string CreatedBy { get; set; }
        public string AuthorizedBy { get; set; }
    }
    public class LoginAuditObj
    {
        public string FullName { get; set; }
        public DateTime? LOGINDATE { get; set; }
        public DateTime? LOGOUTDATE { get; set; }
        public DateTime? ATTEMPTDATE { get; set; }
        public string UserId { get; set; }
        public string BROWSER { get; set; }
        public string IPADDRESS { get; set; }
        public string MAC { get; set; }
        public decimal ITBID { get; set; }
        public string guid { get; set; }
    }
    //public  class CompanyProfileObj
    //{
    //    public int ITBID { get; set; }
    //    public string COMPANY_CODE { get; set; }
    //    public string COMPANY_NAME { get; set; }
    //    public string COMPANY_EMAIL { get; set; }
    //    public string COMPANY_WEBSITE { get; set; }
    //    public string COMPANY_PHONE1 { get; set; }
    //    public string COMPANY_PHONE2 { get; set; }
    //    public string COMPANY_ADDRESS { get; set; }
    //    public int PASSWORD_CHANGE_DAYS { get; set; }
    //    public int PASSWORDLENGTH { get; set; }
    //    public int SYSTEM_IDLE_TIMEOUT { get; set; }
    //    public bool ENABLE_MAKER_CHECKER { get; set; }
    //    public bool ENABLE_USER_LOCKOUT { get; set; }
    //    public int LOCKOUT_TRIAL_COUNT { get; set; }
    //    public string USERID { get; set; }
    //    public DateTime? DATECREATED { get; set; }
    //    public string LAST_MODIFIED_UID { get; set; }
    //}
    ////public class InstitutionAcctObj : POSMISDB_INSTITUTIONACCT
    ////{
    ////    public string CardSchemDesc { get; set; }
    ////    public string InstitutionTypeDesc { get; set; }
    ////    public string EVENTTYPE { get; set; }
    ////    public bool NewRecord { get; set; }
    ////    public bool Updated { get; set; }
    ////    public bool Deleted { get; set; }
    ////    public decimal? DefaultAccount { get; set; }

    ////}


  
    ////public class BillerAcctObj : POSMISDB_BILLERACCOUNT
    ////{
    ////   // public string CardSchemDesc { get; set; }
    ////    public string EVENTTYPE { get; set; }

    ////    public short DefaultAccount { get; set; }
    ////    //public string InstitutionTypeDesc { get; set; }
    ////    public bool NewRecord { get; set; }
    ////    public bool Updated { get; set; }
    ////    public bool Deleted { get; set; }

    ////}
    ////public class MerchantAcctObj : POSMISDB_MERCHANTACCT
    ////{

    ////    public string CurrencyDesc { get; set; }
    ////    public bool NewRecord { get; set; }
    ////    public bool Updated { get; set; }
    ////    public bool Deleted { get; set; }
    ////    public decimal? DefaultAccount { get; set; }
    ////    public bool VisibleFlag { get; set; }
    ////    public string EVENTTYPE { get; set; }

    ////}
    ////public class InstitutionProcessorObj : POSMISDB_INSTPROCESSOR
    ////{
    ////    public string ProcessorTypeDesc { get; set; }
    ////    public string CardSchemDesc { get; set; }
    ////    public string CODE { get; set; }
    ////    public string InstitutionName { get; set; }
    ////    public bool NewRecord { get; set; }
    ////    public bool Updated { get; set; }

    ////}
    ////public class MailGroupObj : POSMISDB_MAILGROUPTEMP
    ////{
    ////    public string FULLNAME { get; set; }


    ////}
    ////public class LastMidObj 
    ////{
    ////    public string MERCHANTID { get; set; }
    ////    public string TERMINALID { get; set; }
    ////    public string MERCHANTNAME { get; set; }
    ////    public string INSTITUTION_CBNCODE { get; set; }
    ////    public string STATECODE { get; set; }
    ////}

    ////public class RevenueHeadObj : POSMISDB_REVENUEHEADTEMP
    ////{
    ////    public string FULLNAME { get; set; }
    ////    public string BANKNAME { get; set; }

    ////}
    ////public class TerminalSingleObj : POSMISDB_TERMINALTEMP
    ////{
    ////    public string FULLNAME { get; set; }


    ////}
    //public class AuthRejectionReasonObj : AuthRejectionReason
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}
    //public class BankObj : Bank
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //    public string CountryName { get; set; }
    //}
    //public class RtgsTransactionTypeObj : RtgsTransactionType
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}
    //public class BankBranchObj : BankBranch
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //    public string Country { get; set; }
    //    public string Bank { get; set; }
    //}

    //public class CountryObj : Country
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}

    //public class CountyObj : County
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}
    //public class RtgsOffsetAccountObj : RtgsOffsetAccount
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}
    //public class AccountTypeObj : AccountType
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}

    //public class InstrumentCodeObj : InstrumentCode
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    //}

    //public class ChargeObj : Charge
    //{
    //    public string CreatedBy { get; set; }
    //    public string AuthorizedBy { get; set; }
    // }
}
