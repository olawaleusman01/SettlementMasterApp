﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Generic.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SettlementMasterEntities : DbContext
    {
        public SettlementMasterEntities()
            : base("name=SettlementMasterEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SM_ROLES> SM_ROLES { get; set; }
        public virtual DbSet<SM_ROLESTEMP> SM_ROLESTEMP { get; set; }
        public virtual DbSet<SM_AUTHLIST> SM_AUTHLIST { get; set; }
        public virtual DbSet<SM_AUTHCHECKER> SM_AUTHCHECKER { get; set; }
        public virtual DbSet<SM_CARDSCHEME> SM_CARDSCHEME { get; set; }
        public virtual DbSet<SM_CARDSCHEMETEMP> SM_CARDSCHEMETEMP { get; set; }
        public virtual DbSet<SM_CURRENCY> SM_CURRENCY { get; set; }
        public virtual DbSet<SM_CURRENCYTEMP> SM_CURRENCYTEMP { get; set; }
        public virtual DbSet<SM_DEPARTMENT> SM_DEPARTMENT { get; set; }
        public virtual DbSet<SM_INSTITUTION> SM_INSTITUTION { get; set; }
        public virtual DbSet<SM_INSTITUTIONTEMP> SM_INSTITUTIONTEMP { get; set; }
        public virtual DbSet<SM_FREQUENCY> SM_FREQUENCY { get; set; }
        public virtual DbSet<SM_FREQUENCYTEMP> SM_FREQUENCYTEMP { get; set; }
        public virtual DbSet<SM_DEPARTMENTTEMP> SM_DEPARTMENTTEMP { get; set; }
        public virtual DbSet<SM_COMPANY_PROFILE> SM_COMPANY_PROFILE { get; set; }
        public virtual DbSet<SM_COMPANY_PROFILETEMP> SM_COMPANY_PROFILETEMP { get; set; }
        public virtual DbSet<SM_COUNTRY> SM_COUNTRY { get; set; }
        public virtual DbSet<SM_COUNTRYTEMP> SM_COUNTRYTEMP { get; set; }
        public virtual DbSet<SM_STATE> SM_STATE { get; set; }
        public virtual DbSet<SM_STATETEMP> SM_STATETEMP { get; set; }
        public virtual DbSet<SM_ASPNETUSERSTEMP> SM_ASPNETUSERSTEMP { get; set; }
        public virtual DbSet<SM_MCC> SM_MCC { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<SM_MCCTEMP> SM_MCCTEMP { get; set; }
        public virtual DbSet<SM_SECTOR> SM_SECTOR { get; set; }
        public virtual DbSet<SM_SECTORTEMP> SM_SECTORTEMP { get; set; }
        public virtual DbSet<SM_PARTYTYPE> SM_PARTYTYPE { get; set; }
        public virtual DbSet<SM_PARTYTYPETEMP> SM_PARTYTYPETEMP { get; set; }
        public virtual DbSet<SM_PARTY> SM_PARTY { get; set; }
        public virtual DbSet<SM_PARTYTEMP> SM_PARTYTEMP { get; set; }
        public virtual DbSet<SM_PARTYACCOUNT> SM_PARTYACCOUNT { get; set; }
        public virtual DbSet<SM_PARTYACCOUNTTEMP> SM_PARTYACCOUNTTEMP { get; set; }
        public virtual DbSet<SM_BANKTYPE> SM_BANKTYPE { get; set; }
        public virtual DbSet<SM_INSTITUTIONACCTTEMP> SM_INSTITUTIONACCTTEMP { get; set; }
        public virtual DbSet<SM_INSTITUTIONACCT> SM_INSTITUTIONACCT { get; set; }
        public virtual DbSet<SM_MERCHANTACCT> SM_MERCHANTACCT { get; set; }
        public virtual DbSet<SM_MERCHANTACCTTEMP> SM_MERCHANTACCTTEMP { get; set; }
        public virtual DbSet<SM_MERCHANTDETAIL> SM_MERCHANTDETAIL { get; set; }
        public virtual DbSet<SM_MERCHANTDETAILHIST> SM_MERCHANTDETAILHIST { get; set; }
        public virtual DbSet<SM_MERCHANTDETAILTEMP> SM_MERCHANTDETAILTEMP { get; set; }
        public virtual DbSet<SM_TERMINAL> SM_TERMINAL { get; set; }
        public virtual DbSet<SM_MAILGROUP> SM_MAILGROUP { get; set; }
        public virtual DbSet<SM_MAILGROUPTEMP> SM_MAILGROUPTEMP { get; set; }
        public virtual DbSet<SM_REVENUEHEADTEMP> SM_REVENUEHEADTEMP { get; set; }
        public virtual DbSet<SM_REVENUEHEAD> SM_REVENUEHEAD { get; set; }
        public virtual DbSet<SM_REVENUEGROUP> SM_REVENUEGROUP { get; set; }
        public virtual DbSet<SM_REVENUEGROUPTEMP> SM_REVENUEGROUPTEMP { get; set; }
        public virtual DbSet<SM_MERCHANTTERMINALUPLD> SM_MERCHANTTERMINALUPLD { get; set; }
        public virtual DbSet<SM_MERTERUPLDGLO> SM_MERTERUPLDGLO { get; set; }
        public virtual DbSet<SM_UPMER_UPLDGLO> SM_UPMER_UPLDGLO { get; set; }
        public virtual DbSet<SM_UPMERTERMUPLDREC> SM_UPMERTERMUPLDREC { get; set; }
        public virtual DbSet<SM_MERCHANTCONFIG> SM_MERCHANTCONFIG { get; set; }
        public virtual DbSet<SM_MERCHANTCONFIGTEMP> SM_MERCHANTCONFIGTEMP { get; set; }
        public virtual DbSet<SM_SETTLEMENTOPTION> SM_SETTLEMENTOPTION { get; set; }
        public virtual DbSet<SM_SETTLEMENTOPTIONTEMP> SM_SETTLEMENTOPTIONTEMP { get; set; }
        public virtual DbSet<SM_SETTLEMENTRULE> SM_SETTLEMENTRULE { get; set; }
        public virtual DbSet<SM_SETTLEMENTRULETEMP> SM_SETTLEMENTRULETEMP { get; set; }
        public virtual DbSet<SM_CHANNELS> SM_CHANNELS { get; set; }
        public virtual DbSet<SM_CHANNELSTEMP> SM_CHANNELSTEMP { get; set; }
        public virtual DbSet<SM_MCCMSC> SM_MCCMSC { get; set; }
        public virtual DbSet<SM_MCCMSCTEMP> SM_MCCMSCTEMP { get; set; }
        public virtual DbSet<SM_MERCHANTMSC> SM_MERCHANTMSC { get; set; }
        public virtual DbSet<SM_MERCHANTMSCTEMP> SM_MERCHANTMSCTEMP { get; set; }
        public virtual DbSet<SM_ROLEPRIV> SM_ROLEPRIV { get; set; }
        public virtual DbSet<SM_ROLEPRIVTEMP> SM_ROLEPRIVTEMP { get; set; }
        public virtual DbSet<SM_MERCHANTUPDATEUPLD> SM_MERCHANTUPDATEUPLD { get; set; }
        public virtual DbSet<SM_MERTERUPDUPLDGLO> SM_MERTERUPDUPLDGLO { get; set; }
        public virtual DbSet<SM_UPMERCHANTUPDATEUPLD> SM_UPMERCHANTUPDATEUPLD { get; set; }
        public virtual DbSet<SM_UPMERUPDUPLDGLO> SM_UPMERUPDUPLDGLO { get; set; }
        public virtual DbSet<SM_RESETLOCKOUT_TEMP> SM_RESETLOCKOUT_TEMP { get; set; }
        public virtual DbSet<SM_RESETPASSWORD_TEMP> SM_RESETPASSWORD_TEMP { get; set; }
        public virtual DbSet<SM_TERMINALTEMP> SM_TERMINALTEMP { get; set; }
        public virtual DbSet<SM_DATABIN> SM_DATABIN { get; set; }
        public virtual DbSet<SM_DATABIN_TEMP> SM_DATABIN_TEMP { get; set; }
        public virtual DbSet<SM_BILLER> SM_BILLER { get; set; }
        public virtual DbSet<SM_BILLERACCOUNT> SM_BILLERACCOUNT { get; set; }
        public virtual DbSet<SM_BILLERACCOUNTTEMP> SM_BILLERACCOUNTTEMP { get; set; }
        public virtual DbSet<SM_BILLERMSC> SM_BILLERMSC { get; set; }
        public virtual DbSet<SM_BILLERMSCTEMP> SM_BILLERMSCTEMP { get; set; }
        public virtual DbSet<SM_BILLERTEMP> SM_BILLERTEMP { get; set; }
        public virtual DbSet<SM_BL_FEE1SHARINGPARTY> SM_BL_FEE1SHARINGPARTY { get; set; }
        public virtual DbSet<SM_BL_FEE1SHARINGPARTYTEMP> SM_BL_FEE1SHARINGPARTYTEMP { get; set; }
        public virtual DbSet<SM_NAPS_NIBSS> SM_NAPS_NIBSS { get; set; }
        public virtual DbSet<SM_NAPS_NIBSS_TEMP> SM_NAPS_NIBSS_TEMP { get; set; }
        public virtual DbSet<SM_APPROVAL_ROUTE> SM_APPROVAL_ROUTE { get; set; }
        public virtual DbSet<SM_APPROVAL_ROUTE_OFFICER> SM_APPROVAL_ROUTE_OFFICER { get; set; }
        public virtual DbSet<SM_APPROVAL_ROUTE_OFFICER_TEMP> SM_APPROVAL_ROUTE_OFFICER_TEMP { get; set; }
        public virtual DbSet<SM_APPROVAL_ROUTE_TEMP> SM_APPROVAL_ROUTE_TEMP { get; set; }
        public virtual DbSet<SM_AUDIT> SM_AUDIT { get; set; }
        public virtual DbSet<SM_EXCHANGERATE> SM_EXCHANGERATE { get; set; }
        public virtual DbSet<SM_EXCHANGERATETEMP> SM_EXCHANGERATETEMP { get; set; }
        public virtual DbSet<SM_SHAREDMSC2DETAILTEMP> SM_SHAREDMSC2DETAILTEMP { get; set; }
        public virtual DbSet<SM_SHAREDMSC2DETAIL> SM_SHAREDMSC2DETAIL { get; set; }
        public virtual DbSet<SM_REVENUEBANKACCT> SM_REVENUEBANKACCT { get; set; }
        public virtual DbSet<SM_REVENUEBANKACCTTEMP> SM_REVENUEBANKACCTTEMP { get; set; }
        public virtual DbSet<SESS_SETRECONDUALUPLD> SESS_SETRECONDUALUPLD { get; set; }
        public virtual DbSet<SM_SERVICECHANNELS> SM_SERVICECHANNELS { get; set; }
        public virtual DbSet<SM_SERVICECHANNELSTEMP> SM_SERVICECHANNELSTEMP { get; set; }
    
        public virtual ObjectResult<SESS_GET_SETRECONUPLD_Result> SESS_GET_SETRECONUPLD(string uSERID)
        {
            var uSERIDParameter = uSERID != null ?
                new ObjectParameter("USERID", uSERID) :
                new ObjectParameter("USERID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SESS_GET_SETRECONUPLD_Result>("SESS_GET_SETRECONUPLD", uSERIDParameter);
        }
    
        public virtual int SESS_POST_SETRECONUPLD(string pID, string pAYREFNO, Nullable<decimal> aMOUNT, Nullable<System.DateTime> pAYMENTDATE, Nullable<System.DateTime> vALUEDATE, string rECEIPTNO, string cUSTOMERNAME, string pAYMENTMETHOD, string tRANSACTIONSTATUS, string dEPOSITSLIPNO, string bANKNAME, string bRANCHNAME, string pAYERID, string vALUEGRANTED, string bATCHID, string uSERID, Nullable<System.DateTime> cREATEDATE, Nullable<short> pOSTTYPE, Nullable<bool> vALIDATIONERRORSTATUS, string vALIDATIONERRORMESSAGE, string wARNINGMESSAGE, Nullable<int> postSequence)
        {
            var pIDParameter = pID != null ?
                new ObjectParameter("PID", pID) :
                new ObjectParameter("PID", typeof(string));
    
            var pAYREFNOParameter = pAYREFNO != null ?
                new ObjectParameter("PAYREFNO", pAYREFNO) :
                new ObjectParameter("PAYREFNO", typeof(string));
    
            var aMOUNTParameter = aMOUNT.HasValue ?
                new ObjectParameter("AMOUNT", aMOUNT) :
                new ObjectParameter("AMOUNT", typeof(decimal));
    
            var pAYMENTDATEParameter = pAYMENTDATE.HasValue ?
                new ObjectParameter("PAYMENTDATE", pAYMENTDATE) :
                new ObjectParameter("PAYMENTDATE", typeof(System.DateTime));
    
            var vALUEDATEParameter = vALUEDATE.HasValue ?
                new ObjectParameter("VALUEDATE", vALUEDATE) :
                new ObjectParameter("VALUEDATE", typeof(System.DateTime));
    
            var rECEIPTNOParameter = rECEIPTNO != null ?
                new ObjectParameter("RECEIPTNO", rECEIPTNO) :
                new ObjectParameter("RECEIPTNO", typeof(string));
    
            var cUSTOMERNAMEParameter = cUSTOMERNAME != null ?
                new ObjectParameter("CUSTOMERNAME", cUSTOMERNAME) :
                new ObjectParameter("CUSTOMERNAME", typeof(string));
    
            var pAYMENTMETHODParameter = pAYMENTMETHOD != null ?
                new ObjectParameter("PAYMENTMETHOD", pAYMENTMETHOD) :
                new ObjectParameter("PAYMENTMETHOD", typeof(string));
    
            var tRANSACTIONSTATUSParameter = tRANSACTIONSTATUS != null ?
                new ObjectParameter("TRANSACTIONSTATUS", tRANSACTIONSTATUS) :
                new ObjectParameter("TRANSACTIONSTATUS", typeof(string));
    
            var dEPOSITSLIPNOParameter = dEPOSITSLIPNO != null ?
                new ObjectParameter("DEPOSITSLIPNO", dEPOSITSLIPNO) :
                new ObjectParameter("DEPOSITSLIPNO", typeof(string));
    
            var bANKNAMEParameter = bANKNAME != null ?
                new ObjectParameter("BANKNAME", bANKNAME) :
                new ObjectParameter("BANKNAME", typeof(string));
    
            var bRANCHNAMEParameter = bRANCHNAME != null ?
                new ObjectParameter("BRANCHNAME", bRANCHNAME) :
                new ObjectParameter("BRANCHNAME", typeof(string));
    
            var pAYERIDParameter = pAYERID != null ?
                new ObjectParameter("PAYERID", pAYERID) :
                new ObjectParameter("PAYERID", typeof(string));
    
            var vALUEGRANTEDParameter = vALUEGRANTED != null ?
                new ObjectParameter("VALUEGRANTED", vALUEGRANTED) :
                new ObjectParameter("VALUEGRANTED", typeof(string));
    
            var bATCHIDParameter = bATCHID != null ?
                new ObjectParameter("BATCHID", bATCHID) :
                new ObjectParameter("BATCHID", typeof(string));
    
            var uSERIDParameter = uSERID != null ?
                new ObjectParameter("USERID", uSERID) :
                new ObjectParameter("USERID", typeof(string));
    
            var cREATEDATEParameter = cREATEDATE.HasValue ?
                new ObjectParameter("CREATEDATE", cREATEDATE) :
                new ObjectParameter("CREATEDATE", typeof(System.DateTime));
    
            var pOSTTYPEParameter = pOSTTYPE.HasValue ?
                new ObjectParameter("POSTTYPE", pOSTTYPE) :
                new ObjectParameter("POSTTYPE", typeof(short));
    
            var vALIDATIONERRORSTATUSParameter = vALIDATIONERRORSTATUS.HasValue ?
                new ObjectParameter("VALIDATIONERRORSTATUS", vALIDATIONERRORSTATUS) :
                new ObjectParameter("VALIDATIONERRORSTATUS", typeof(bool));
    
            var vALIDATIONERRORMESSAGEParameter = vALIDATIONERRORMESSAGE != null ?
                new ObjectParameter("VALIDATIONERRORMESSAGE", vALIDATIONERRORMESSAGE) :
                new ObjectParameter("VALIDATIONERRORMESSAGE", typeof(string));
    
            var wARNINGMESSAGEParameter = wARNINGMESSAGE != null ?
                new ObjectParameter("WARNINGMESSAGE", wARNINGMESSAGE) :
                new ObjectParameter("WARNINGMESSAGE", typeof(string));
    
            var postSequenceParameter = postSequence.HasValue ?
                new ObjectParameter("PostSequence", postSequence) :
                new ObjectParameter("PostSequence", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SESS_POST_SETRECONUPLD", pIDParameter, pAYREFNOParameter, aMOUNTParameter, pAYMENTDATEParameter, vALUEDATEParameter, rECEIPTNOParameter, cUSTOMERNAMEParameter, pAYMENTMETHODParameter, tRANSACTIONSTATUSParameter, dEPOSITSLIPNOParameter, bANKNAMEParameter, bRANCHNAMEParameter, pAYERIDParameter, vALUEGRANTEDParameter, bATCHIDParameter, uSERIDParameter, cREATEDATEParameter, pOSTTYPEParameter, vALIDATIONERRORSTATUSParameter, vALIDATIONERRORMESSAGEParameter, wARNINGMESSAGEParameter, postSequenceParameter);
        }
    
        public virtual int SESS_PURGE_SETRECONUPLD(string uSERID)
        {
            var uSERIDParameter = uSERID != null ?
                new ObjectParameter("USERID", uSERID) :
                new ObjectParameter("USERID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SESS_PURGE_SETRECONUPLD", uSERIDParameter);
        }
    }
}