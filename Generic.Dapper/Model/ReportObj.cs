using Generic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    class ReportObj
    {
    }

    public class Get_POSCARDSCHEME
    {
        public string CARDSCHEME { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }

    public class Get_LOGDATA
    {
        public string ID { get; set; }
        public string DOCNO { get; set; }
        public string TERMINALID { get; set; }
        public string MERCHANTID { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string CPD { get; set; }

    }

    public class GET_CITY
    {
        public string CITYCODE { get; set; }
        public string CITYNAME { get; set; }
        public string STATENAME { get; set; }
        public string COUNTRYNAME { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string CREATEDATESTRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }

    public class GET_COUNTRY
    {
        public string COUNTRY_CODE { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
       // public int draw { get; set; }

    }

    public class Rpt_Audit 
    {
        public string TABLENAME { get; set; }
        public string COLUMNNAME { get; set; }
        public string ORIGINALVALUE { get; set; }
        public string NEWVALUE { get; set; }
        public string FULLNAME { get; set; }
        public string EVENTTYPE { get; set; }
        //public decimal AUDITLOGID { get; set; }
        public string EVENTDATE { get; set; }
        //public string DATESTRING { get { return EVENTDATE.GetValueOrDefault().ToString("dd-MM-yyyy hh-MM"); } }

        public string RECORDID { get; set; }
        public string IPADDRESS { get; set; }
    }

    public class Rpt_Exemption
    {
        public string MERCHANTID { get; set; }
        public string TERMINALID { get; set; }
        public string CREATEDATE { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string TRANDATETIME { get; set; }
        public string PAYMENTREFERENCE { get; set; }
        public string TRANAMOUNT { get; set; }

    }

    public class Rpt_LoginUser    {
        public string USERNAME { get; set; }
        public string FULLNAME { get; set; }
        public string EMAIL { get; set; }
        public string ROLENAME { get; set; }
        public string INSTITUTION_NAME { get; set; }
        public DateTime? CREATEDATE { get; set; }

    }

    public class Rpt_ApprovalDetail: SM_ASPNETUSERSTEMP
    {

    }
    public class GET_LOGINAUDIT
    {
        public decimal ITBID { get; set; }
        public string USERID { get; set; }
        public Nullable<System.DateTime> LOGINDATE { get; set; }
        public Nullable<System.DateTime> LOGOUTDATE { get; set; }
        public Nullable<System.DateTime> ATTEMPTDATE { get; set; }
        public string GUIDNO { get; set; }
        public string IP_ADDRESS { get; set; }
        public string MAC { get; set; }
        public string MAC_ADDRESS { get; set; }
        public string BROWSER { get; set; }
        public string FULLNAME { get; set; }

    }
    public class GET_STATE
    {
      
        public string COUNTRYCODE { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string STATENAME { get; set; }
        public string STATECODE { get; set; }
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }
    public class GET_CURRENCY
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_NAME { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string ISO_CODE { get; set; }
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }

    public class GET_FREQUENCY
    {
        public int ITBID { get; set; }
        public string FREQUENCY_DESC { get; set; }
        public int? WORKDAYS { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string CREATEDATESTRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string USERID { get; set; }
        public string STATUS { get; set; }

    }
    public class GET_DEPARTMENT
    {
        public string DEPARTMENTCODE { get; set; }
        public string DEPARTMENTNAME { get; set; }    
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }

  
       public class GET_INSTITUTION
    {
        public string CBN_CODE { get; set; }
        public string INSTITUTION_SHORTCODE { get; set; }
        public string INSTITUTION_NAME { get; set; }
        public string BANKTYPENAME { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string PHONENO { get; set; }
        public string INSTITUTION_URL { get; set; }
        public string INSTITUTION_ADDRESS { get; set; }
        public string INSTITUTION_COUNTRY { get; set; }
        public string INSTITUTION_STATE { get; set; }
        public string INSTITUTION_CITY { get; set; }
        public string DEPOSIT_BANKNAME { get; set; }
        public string DEPOSIT_ACCOUNTNO { get; set; }
        public string DEPOSIT_BANKADDRESS { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string CREATEDATESTRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }

    public class GET_MCC
    {
        public string MCC_CODE { get; set; }
        public string MCC_DESC { get; set; }
        public string SECTOR_NAME { get; set; }
       
        public DateTime? CREATEDATE { get; set; }
        public string CREATEDATESTRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }
    public class GET_MSC
    {
        public string MCC_CODE { get; set; }
        public string MCC_DESC { get; set; }
        public string SECTOR_NAME { get; set; }
        public string CARDSCHEME { get; set; }
        public string DOM_MSCVALUE { get; set; }
        public string INT_MSCVALUE { get; set; }
        public string DOM_SETTLEMENT_CURRENCY { get; set; }
        public string INT_SETTLEMENT_CURRENCY { get; set; }
        public string DOMCAP { get; set; }
        public string INTLCAP { get; set; }
        public string DOM_FREQUENCY { get; set; }
        public string INT_FREQUENCY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string CREATEDATESTRING { get { return CREATEDATE != null ? CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy") : ""; } }
    }
    public class GET_MERCHANTDETAIL
    {
    
  
        public string MERCHANTID { get; set; }
        public string MERCHANTNAME { get; set; }
        public string CONTACTTITLE { get; set; }
        public string CONTACTNAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONENO { get; set; }
        public string ADDRESS { get; set; }
        public string BUSINESS_CODE { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string STATENAME { get; set; }
        public string CITYNAME { get; set; }
  
        public string MCC_CODE { get; set; }
        public string MCC_DESC { get; set; }
        public string ACCEPTANCE_TYPE { get; set; }
        public string BANKCODE { get; set; }
        public string BANKNAME { get; set; }
        public string PAYTITUDE_STAMP { get; set; }
        public string STATUS { get; set; }
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }

    }

    public class GET_TERMINALDETAIL
    {
        public string TERMINALID { get; set; }
        public string MERCHANTID { get; set; }
        public string MERCHANTNAME { get; set; }
        public string TERMINALMODEL_CODE { get; set; }
        public string TERMINALOWNER_CODE { get; set; }
        public string SETTLEMENT_CURRENCY { get; set; }
        public string TRANSACTION_CURRENCY { get; set; }
        public string FREQUENCY { get; set; }
        public string PROCESSOR { get; set; }
        public string POINTED { get; set; }
        public string TERMINALDEPLOYEDDATE { get; set; }
        public string TERMINALACTIVATNDATE { get; set; }
        public string TERMINALCOUNTRY { get; set; }
        public string DEVICETYPE { get; set; }
        public string TERMINALSN { get; set; }
        public string TERMINALMODELCODE { get; set; }
        public string TERMINALTYPECODE { get; set; }
        public string SAMNUMBER { get; set; }
        public string DEFAULTIPADDRESS { get; set; }
        public string TERMINALOPERATNSGROUP { get; set; }
        public string DEFAULTOPERATION { get; set; }
        public string GPRSSIMCARDNO { get; set; }
        public string TERMINALSTATUS { get; set; }
        public string GPRSPROVIDERCODE { get; set; }
        public string NETCONECTNCODE { get; set; }
        public string INDICATOR_LOCATION { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string STATUS { get; set; }
        public string BATCHID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string RULE_OPTION { get; set; }
        public string PAYATTITUDE_STAMP { get; set; }
        public string PTSP { get; set; }
        public string EMAIL_ALERTS { get; set; }
        public string SLIP_HEADER { get; set; }
        public string SLIP_FOOTER { get; set; }
        public string GENERATED { get; set; }
        public string RULE_LOCATOR { get; set; }
        public string ACCOUNTNO { get; set; }
        public string INTERFACE_FORMAT { get; set; }
        public string ACCEPTANCE_TYPE { get; set; }
        public string XML_GENERATEDBY { get; set; }
        public DateTime? XML_GENERATEDDATE { get; set; }
        public string PTSA { get; set; }
        public string VERVACQUIRERIDNO { get; set; }
        public string MASTACQUIRERIDNO { get; set; }
        public string VISAACQUIRERIDNO { get; set; }
        public string TMSOWNER { get; set; }

    }
    public class GET_PARTY
    {
        public string PARTY_SHORTNAME { get; set; }
        public string PARTY_DESC { get; set; }
        public string PARTYTYPE_DESC { get; set; }
        public string CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string PARTY_REFID { get; set; }
        
    }
    public class RPT_LoginAudit
    {
        public string FULLNAME { get; set; }
        public DateTime? LOGINDATE { get; set; }
        public DateTime? LOGOUTDATE { get; set; }
        public DateTime? ATTEMPTDATE { get; set; }
        public string LOGINDATESTRING { get { return LOGINDATE.GetValueOrDefault().ToString("dd-MM-yyyy hh:MM"); } }
        public string LOGOUTDATESTRING { get { return LOGOUTDATE != null ? LOGOUTDATE.GetValueOrDefault().ToString("dd-MM-yyyy hh:MM") : ""; } }
        public string ATTEMPTDATESTRING { get { return ATTEMPTDATE.GetValueOrDefault().ToString("dd-MM-yyyy hh:MM"); } }
        public string USERID { get; set; }
        public string BROWSER { get; set; }
        public string IP_ADDRESS { get; set; }
        public string MAC { get; set; }
        public decimal ITBID { get; set; }
        public string guid { get; set; }
    }
}
