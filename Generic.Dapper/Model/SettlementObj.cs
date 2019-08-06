using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class GetMSCObj
    {
        public decimal MSC1MARGIN { get; set; }
        public decimal MSC2MARGIN { get; set; }
        public decimal? SHARINGMSCVALUECAPPED { get; set; }
        public decimal? ACQRRATE { get; set; }
        public decimal? ACQRAMOUNT { get; set; }
       
        public decimal? AGNTRATE { get; set; }
        public decimal? AGNTAMOUNT { get; set; }
        public decimal? AMT_DUEMERCHANT { get; set; }
        public decimal? APPLIEDMSCVALUE { get; set; }
        public decimal? BILRRATE { get; set; }
        public decimal? BILRAMOUNT { get; set; }
        public decimal? COAQRATE { get; set; }
        public decimal? COAQAMOUNT { get; set; }
        public decimal? COLLECTIONBANKSRATE { get; set; }
        public decimal? COLLECTIONBANKSVALUE { get; set; }
        public decimal? DEDUCTION_AMOUNTDUE { get; set; }
        public decimal? DEFAULTMSC { get; set; }
        public decimal? DEFAULTMSCAMT { get; set; }
        public decimal? DEFAULTMSC_CAP { get; set; }
        public decimal? EFFECTIVEMSC { get; set; }
        public decimal? GOVTRATE { get; set; }
        public decimal? GOVTAMOUNT { get; set; }
        public decimal? ISSRRATE { get; set; }
        public decimal? ISSRAMOUNT { get; set; }
        public decimal? MSC2AMOUNT { get; set; }
        public decimal? MSC1AMOUNT { get; set; }
        public decimal? MSCAMOUNT { get; set; }
        public decimal? MSC2 { get; set; }
        public decimal? MSC1 { get; set; }
        public decimal? MSC1CAP { get; set; }
        public decimal? MSC2CAP { get; set; }
        public decimal? MSCCONCESSION { get; set; }
        public decimal? MSCDISCOUNT { get; set; }
        public decimal? MSCDISCOUNTAMT { get; set; }
        public decimal? MSCSUBSIDY { get; set; }
        public decimal? SUBSIDYAMOUNT { get; set; }
        // public decimal? PACQUIRERRATE { get; set; }
        // public decimal? PACQUIRERVALUE { get; set; }
        //public decimal? PAGENTSRATE { get; set; }
        //public decimal? PAGENTSVALUE { get; set; }
        //public decimal? PBILLERRATE { get; set; }
        //public decimal? PBILLERVALUE { get; set; }
        //public decimal? PCOACQUIRERERATE { get; set; }
        //public decimal? PCOACQUIREREVALUE { get; set; }
        public string SPECIALMESSAGE4 { get; set; }
        public decimal? COLBRATE { get; set; }
        public decimal? COLBAMOUNT { get; set; }
        public decimal? PERC_ALLOTMERCHANT { get; set; }
        public string CARDSCHEME { get; set; }
        //public decimal? PGOVERNMENTVALUE { get; set; }
        //public decimal? PISSUERRATE { get; set; }
        //public decimal? PISSUERVALUE { get; set; }
        //public decimal? PPTSARATE { get; set; }
        //public decimal? PPTSAVALUE { get; set; }
        //public decimal? PPTSPRATE { get; set; }
        //public decimal? PPTSPVALUE { get; set; }
        public decimal? REVWRATE { get; set; }
        public decimal? REVWAMOUNT { get; set; }
        //public decimal? PSWITCHTRATE { get; set; }
        //public decimal? PSWITCHTVALUE { get; set; }
        public decimal? TERWRATE { get; set; }
        public decimal? TERWMOUNT { get; set; }
        //public decimal? PTERMOWNERVALUE { get; set; }
        public decimal? PTSARATE { get; set; }
        public decimal? PTSAAMOUNT { get; set; }
        public decimal? PTSPRATE { get; set; }
        public decimal? PTSPAMOUNT { get; set; }
        public decimal? VASTRATE { get; set; }
        public decimal? VASTAMOUNT { get; set; }
        public decimal? TECHRATE { get; set; }
        public decimal? TECHAMOUNT { get; set; }
        public decimal? VENDRATE { get; set; }
        public decimal? VENDAMOUNT { get; set; }
        public decimal? TERWAMOUNT { get; set; }
        public decimal? REVENUEOWNERRATE { get; set; }
       // public decimal? REVENUEOWNERVALUE { get; set; }
        public decimal? SHARINGMSCDIFFRATE { get; set; }
        public decimal? SHARINGMSCDIFFVALUE { get; set; }
        public decimal? SHAREDMSC { get; set; }
        public decimal? SHAREDMSCAMT { get; set; }
        public string spliMSC2SHARING { get; set; }
        public string spliMSC2SHARINGACCT { get; set; }
        public decimal? STANDARDRATE { get; set; }
        public decimal? STANDARDVALUE { get; set; }
        public decimal? SWTHRATE { get; set; }
        public decimal? SWTHAMOUNT { get; set; }
        public decimal? TOTAL_AMOUNTDUE { get; set; }
        public decimal? TRUEMSC_RATE { get; set; }
        public decimal? UNSHARINGMSCRATE { get; set; }
        public decimal? UNSHARINGMSCVALUE { get; set; }
       
        public string DR_ACCTNO { get; set; }
        public string DR_BANKCODE { get; set; }
        public string REVENUECODE { get; set; }
        public string MSC2PARTY1_BANKCODE { get; set; }
        public string MSC2PARTY1_NAME { get; set; }
        public string MSC2PARTY1_RATE { get; set; }
        public decimal? MSC2PARTY1_VALUE { get; set; }
        public string MSC2PARTY1_ACCOUNT { get; set; }
        public string MSC2PARTY1_ACCOUNTNAME { get; set; }
        public string MSC2PARTY2_BANKCODE { get; set; }
        public string MSC2PARTY2_NAME { get; set; }
        public string MSC2PARTY2_RATE { get; set; }
        public decimal? MSC2PARTY2_VALUE { get; set; }
        public string MSC2PARTY2_ACCOUNT { get; set; }
        public string MSC2PARTY2_ACCOUNTNAME { get; set; }
        public string MSC2PARTY3_BANKCODE { get; set; }
        public string MSC2PARTY3_NAME { get; set; }
        public string MSC2PARTY3_RATE { get; set; }
        public decimal? MSC2PARTY3_VALUE { get; set; }
        public string MSC2PARTY3_ACCOUNT { get; set; }
        public string MSC2PARTY3_ACCOUNTNAME { get; set; }
        public string MSC2PARTY4_BANKCODE { get; set; }
        public string MSC2PARTY4_NAME { get; set; }
        public string MSC2PARTY4_RATE { get; set; }
        public decimal? MSC2PARTY4_VALUE { get; set; }
        public string MSC2PARTY4_ACCOUNT { get; set; }
        public string MSC2PARTY4_ACCOUNTNAME { get; set; }
        public string MSC2PARTY5_BANKCODE { get; set; }
        public string MSC2PARTY5_NAME { get; set; }
        public string MSC2PARTY5_RATE { get; set; }
        public decimal? MSC2PARTY5_VALUE { get; set; }
        public string MSC2PARTY5_ACCOUNT { get; set; }
        public string MSC2PARTY5_ACCOUNTNAME { get; set; }
        public string MSC2PARTY6_BANKCODE { get; set; }
        public string MSC2PARTY6_NAME { get; set; }
        public string MSC2PARTY6_RATE { get; set; }
        public decimal? MSC2PARTY6_VALUE { get; set; }
        public string MSC2PARTY6_ACCOUNT { get; set; }
        public string MSC2PARTY6_ACCOUNTNAME { get; set; }
        public string MSC2PARTY7_BANKCODE { get; set; }
        public string MSC2PARTY7_NAME { get; set; }
        public string MSC2PARTY7_RATE { get; set; }
        public decimal? MSC2PARTY7_VALUE { get; set; }
        public string MSC2PARTY7_ACCOUNT { get; set; }
        public string MSC2PARTY7_ACCOUNTNAME { get; set; }

        public string MSC2PARTY8_BANKCODE { get; set; }
        public string MSC2PARTY8_NAME { get; set; }
        public string MSC2PARTY8_RATE { get; set; }
        public decimal? MSC2PARTY8_VALUE { get; set; }
        public string MSC2PARTY8_ACCOUNT { get; set; }
        public string MSC2PARTY8_ACCOUNTNAME { get; set; }
        public string MSC2PARTY9_BANKCODE { get; set; }
        public string MSC2PARTY9_NAME { get; set; }
        public string MSC2PARTY9_RATE { get; set; }
        public decimal? MSC2PARTY9_VALUE { get; set; }
        public string MSC2PARTY9_ACCOUNT { get; set; }
        public string MSC2PARTY9_ACCOUNTNAME { get; set; }
        public string MSC2PARTY10_BANKCODE { get; set; }
        public string MSC2PARTY10_NAME { get; set; }
        public string MSC2PARTY10_RATE { get; set; }
        public decimal? MSC2PARTY10_VALUE { get; set; }
        public string MSC2PARTY10_ACCOUNT { get; set; }
        public string MSC2PARTY10_ACCOUNTNAME { get; set; }
    }
    public class PMerTermObj
    {

        public string MerchantId { get; set; }
        public string TerminalId { get; set; }
    }
        public class PMerchantObj
    {

        public string merchantid { get; set; }
        public string merchantname { get; set; }
        public string address { get; set; }
        public string country_code { get; set; }
        public string mcc_code { get; set; }
        public string mcc_desc { get; set; }

        public string sector_code { get; set; }

        public string sector { get; set; }
        public decimal? customerid { get; set; }
        public string institution_cbncode { get; set; }
        public string FREQ_CODE { get; set; }
        public string FREQUENCY_DESC { get; set; }
        public bool CUSTOM { get; set; }
        public int? SETTLEMENT_FREQUENCY { get; set; }
        public int? SET_DATE_TERM { get; set; }
        public int? SET_DAYS   { get; set; }
    }

    public class TrawdataBinObj
    {
        public string chargeissuer { get; set; }
        public string bintype { get; set; }
        public string cardscheme { get; set; }
        public string BANKFIID { get; set; }
        public string bankcode { get; set; }
        public string bankshortcode { get; set; }
        public string COUNTRYCODE { get; set; }
        public string bankname { get; set; }
        public decimal? PROCESSINGFEE { get; set; }
        public string processor { get; set; }

    }

    public class PTerminalObj
    {
        //TERMINALID,A.MERCHANTID,SETTLEMENT_CURRENCY,SETTLEMENT_FREQUENCY,ACCOUNT_ID,TERMINALOWNER_CODE,Termownername,PTSP,nvl(ptspname, PTSP) ptspname,
        //                'NIBSS' PTSA,'UP' SWTCH,
        //                DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME,workdays
        public string TERMINALID { get; set; }
        public string MERCHANTID { get; set; }
        public string SETTLEMENT_CURRENCY { get; set; }
        public int? SETTLEMENT_FREQUENCY { get; set; }
        public decimal? ACCOUNT_ID { get; set; }
        public string TERMINALOWNER_CODE { get; set; }

        public string Termownername { get; set; }
        public string PTSP { get; set; }
        public string ptspname { get; set; }

        public string PTSA { get; set; }
        public string SWTCH { get; set; }
        public string PTSANAME { get; set; }
        public string SWTCHNAME { get; set; }

        public string DEPOSIT_BANKCODE { get; set; }


        public string DEPOSIT_ACCOUNTNO { get; set; }
        public string DEPOSIT_BANKNAME { get; set; }
        public string DEPOSIT_ACCTNAME { get; set; }
        public string PTSPbank { get; set; }
        public string PTSPAcct { get; set; }

        public int? workdays { get; set; }
        public string FREQUENCY_DESC { get; set; }


    }

    public class PPartyObj
    {
        //   party_desc v_PTSANAME,'' P_BANKCODE ,'' v_PTSAACCTNO,'' T_PTSABANKNAME
        public string v_PTSANAME { get; set; }
        public string P_BANKCODE { get; set; }

        public string MCC { get; set; }
        public string v_PTSAACCTNO { get; set; }
        public string T_PTSABANKNAME { get; set; }
        public string V_PARTY_SHORTNAME { get; set; }


    }


    public class ObjCodeDOC
    {
        public decimal? Code { get; set; }
        public decimal? Docno { get; set; }

    }
    public class PTermInstObj
    {
        //TERMINALID,A.MERCHANTID,SETTLEMENT_CURRENCY,SETTLEMENT_FREQUENCY,ACCOUNT_ID,TERMINALOWNER_CODE,Termownername,PTSP,nvl(ptspname, PTSP) ptspname,
        //                'NIBSS' PTSA,'UP' SWTCH,
        //                DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME,workdays
        public string POSCBNBANKCODE { get; set; }
        public string ACQUIRERFIID { get; set; }

    }
    public class PAcquirerObj
    {
        //TERMINALID,A.MERCHANTID,SETTLEMENT_CURRENCY,SETTLEMENT_FREQUENCY,ACCOUNT_ID,TERMINALOWNER_CODE,Termownername,PTSP,nvl(ptspname, PTSP) ptspname,
        //                'NIBSS' PTSA,'UP' SWTCH,
        //                DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME,workdays
        public string INSTITUTION_NAME { get; set; }
        public string DEPOSIT_ACCOUNTNO { get; set; }
        public string INSTITUTIONID { get; set; }
        public string CBN_CODE { get; set; }
        public string DEPOSIT_BANKCODE { get; set; }
        public string DEPOSIT_BANKNAME { get; set; }

    }
    public class PRevenueObj
    {
        public string REVENUECODE { get; set; }
        public string REVENUEDESC { get; set; }
        public string GROUPCODE { get; set; }
        public string GROUPNAME { get; set; }
        public string MERCHANTID { get; set; }
        public string FREQUENCY_DESC { get; set; }
        public int? WORKDAYS { get; set; }
        public string RVH_DEPOSIT_BANKCODE { get; set; }
        public string RVH_DEPOSIT_ACCOUNTNO { get; set; }
        public string RVH_DEPOSIT_BANKNAME { get; set; }
        public string RVG_DEPOSIT_BANKCODE { get; set; }
        public string RVG_DEPOSIT_ACCOUNTNO { get; set; }
        public string RVG_DEPOSIT_BANKNAME { get; set; }
        public bool? GLOBALACCOUNTFLAG  { get; set; }
        public int? SETTLEMENT_FREQUENCY { get; set; }
        public int? SET_DAYS { get; set; }
        public int? SET_DATE_TERM { get; set; }
        public string FREQ_CODE { get; set; }
        public bool CUSTOM { get; set; }

    }
    public class PAgentObj
    {
        public string AGENT_NAME { get; set; }
        public string AGENT_ACCTNO { get; set; }
        public string CBN_CODE { get; set; }

    }

    public class PFullRemObj
    {

        public string FULLREM_DESC { get; set; }
        public string SIGN { get; set; }

    }
    public class PSplitSharingObj
    {
        public string v_VEND_NAME { get; set; }
        public decimal? v_VEND_VALUE { get; set; }
        public string v_VEND_ACCOUNT { get; set; }

    }
    public class PPercentageDueObj
    {
        // SELECT NVL(PERCMERCHANT,0),NVL(PERCMERCHANT_OTHER,0),NVL(PERCMERCHANT_UPSL,0),NVL(APPLYMERCHANTSHARING,0) INTO
        //v_PERCMERCHANT, v_PERCMERCHANT_OTHERS, v_PERCMERCHANT_UP, v_APPLYMERCHANTSHARING FROM POSMISDB_REVENUEHEAD WHERE TRIM(CODE)=TRIM(v_REVENUECODE);

        public decimal? v_PERCMERCHANT { get; set; }
        public decimal? v_PERCMERCHANT_OTHERS { get; set; }
        public decimal? v_PERCMERCHANT_UP { get; set; }
        public int? v_APPLYMERCHANTSHARING { get; set; }


    }

    public class PPayArenaObj
    {
        // SELECT NVL(PERCMERCHANT,0),NVL(PERCMERCHANT_OTHER,0),NVL(PERCMERCHANT_UPSL,0),NVL(APPLYMERCHANTSHARING,0) INTO
        //v_PERCMERCHANT, v_PERCMERCHANT_OTHERS, v_PERCMERCHANT_UP, v_APPLYMERCHANTSHARING FROM POSMISDB_REVENUEHEAD WHERE TRIM(CODE)=TRIM(v_REVENUECODE);

        public string v_MerchantId { get; set; }
        public string v_MerchantName { get; set; }
        public string MCC { get; set; }
        public string DEPOSIT_ACCOUNTNO { get; set; }
        public string DEPOSIT_BANKNAME { get; set; }
        public string DEPOSIT_BANKCODE { get; set; }


    }
}
