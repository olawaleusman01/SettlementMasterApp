using Generic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class TerminalMscObj : SM_MCCMSC
    {
        public long TERMINALMSC_ITBID { get; set; }
        public string MERCHANTID { get; set; }
        public decimal DOM_MSCSUBSIDY { get; set; }
        public decimal DOM_MSCCONCESSION { get; set; }
        public decimal INTMCC_VARIANCE { get; set; }
        public decimal MCC_VARIANCE { get; set; }
        public decimal INT_MSCSUBSIDY { get; set; }
        public decimal INT_MSCCONCESSION { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public string MAKER_ID { get; set; }
        public string TERMINAL_STATUS { get; set; }
        public DateTime? DATECREATED { get; set; }
        

    }
    public class TerminalObj : SM_TERMINAL
    {
        public string ACCOUNTID { get; set; }
        public string CREATED_BY { get; set; }
        public string MERCHANTNAME { get; set; }
        public string INSTITUTION_NAME { get; set; }
        public string MCC_CODE { get; set; }
        public string MCC_DESC { get; set; }
        public string FREQUENCY { get; set; }
        public string SET_CURRENCY { get; set; }
        public string TRANS_CURRENCY { get; set; }
        public string DEPOSIT_ACCOUNTNO { get; set; }
        public string DEPOSIT_BANKNAME { get; set; }
        public string DEPOSIT_ACCTNAME { get; set; }
        public string CONTACTTITLE { get; set; }
        public string CONTACTNAME { get; set; }
        public string PHONENO { get; set; }
        public string EMAIL { get; set; }
        public string BANKCODE { get; set; }
        public string BANKACCNO { get; set; }
        public string BANKTYPE { get; set; }
        public string LGA_LCDA { get; set; }
        public string BANK_URL { get; set; }
        public string ACCOUNTNAME { get; set; }
        public string PAYATTITUDE_ACCEPTANCE { get; set; }
        public string TRANSCURRENCY { get; set; }
        public string BUSINESS_CODE { get; set; }
        public string STATE_CODE { get; set; }
        public string ADDRESS { get; set; }
        public string DEPOSIT_BANKCODE { get; set; }
        public decimal? DB_ITBID { get; set; }
        public string EVENTTYPE { get; set; }
        public string PID { get; set; }
        public bool EmailAlert { get; set; }
        public bool IS_NEWACCT { get; set; }
    }
}
