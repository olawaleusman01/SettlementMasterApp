using Generic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class MerchantMscObj : SM_MCCMSC
    {
        public string CREATED_BY { get; set; }
        public string CHANNEL_DESC { get; set; }
        public decimal MERCHANTMSC_ITBID { get; set; }
        public string MERCHANTID { get; set; }
        public string INSTITUTION_CBNCODE { get; set; }
        public decimal DOM_MSCSUBSIDY { get; set; }
        public decimal DOM_MSCCONCESSION { get; set; }
        public decimal INT_MSCSUBSIDY { get; set; }
        public decimal INT_MSCCONCESSION { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public decimal MERCHANTDOMCAP { get; set; }
        public decimal MERCHANTINTLCAP { get; set; }
        public string MERCHANT_STATUS { get; set; }
        public string MERCHANTNAME { get; set; }
        public string MAKER_ID { get; set; }
        public DateTime DATECREATED { get; set; }

        public decimal DOM_MSC2 { get; set; }
        public decimal INT_MSC2 { get; set; }
        public decimal DOM_MSC2CAP { get; set; }
        public decimal INT_MSC2CAP { get; set; }
        public decimal TOTAL_DOMMSC { get
            {
                return (decimal)DOM_MSCVALUE + DOM_MSC2;
            }
        }
        public decimal TOTAL_INTMSC
        {
            get
            {
                return (decimal)INT_MSCVALUE + INT_MSC2;
            }
        }
        public decimal TOTAL_DOMCAP
        {
            get
            {
                return (decimal)DOMCAP + DOM_MSC2CAP;
            }
        }
        public decimal TOTAL_INTCAP
        {
            get
            {
                return (decimal)INTLCAP + INT_MSC2CAP;
            }
        }
        public decimal DOM_UNSHAREDMSC { get; set; }
        public decimal DOM_SHAREDMSC { get
            {
               return  (decimal)DOM_MSCVALUE - DOM_UNSHAREDMSC;
            }
        }
        public decimal INT_UNSHAREDMSC { get; set; }

        public decimal INT_SHAREDMSC
        {
            get
            {
                return (decimal)INT_MSCVALUE - INT_UNSHAREDMSC;
            }
        }

        public decimal DOM_UNSHAREDCAP { get; set; }
        public decimal DOM_SHAREDCAP
        {
            get
            {
                return (decimal) MERCHANTDOMCAP - DOM_UNSHAREDCAP;
            }
        }
        public decimal AMOUNTDUEMERCH_PERC { get; set; }
        public decimal AMOUNTDUEOTHER_PERC
        {
            get
            {
                return 100 - AMOUNTDUEMERCH_PERC;
            }
        }
        public string APPLYMERCHANTSHARING { get; set; }
        public string AMOUNTDUEMERCH_TYPE { get; set; }
        public string MCC { get; set; }
        public decimal INT_UNSHAREDCAP { get; set; }
        public decimal INT_SHAREDCAP
        {
            get
            {
                return INTLCAP ?? 0 - INT_UNSHAREDCAP;
            }
        }
        public decimal FEE1 { get; set; }
        public decimal FEE2 { get; set; }
        public string FEE1_CALCBASIS { get; set; }
        public string FEE2_CALCBASIS { get; set; }
        public int ACQFLAG { get; set; }
        public List<SharingPartyObj> DomMsc2List { get; set; }
    }
    //public class BillerMscObj : POSMISDB_BILLERMSCTEMP
    //{
    //    public int _id  {get; set;}
    //    public bool NewRecord { get; set; }
    //    public bool Updated { get; set; }
    //    public string CHANNEL_DESC { get; set; }
    //    public string MERCHANTNAME { get; set; }
    //    public string MAKER_ID { get; set; }
    //    public DateTime DATECREATED { get; set; }
    //    public decimal TOTAL_DOMMSC
    //    {
    //        get
    //        {
    //            return DOM_MSC1.GetValueOrDefault() + DOM_MSC2.GetValueOrDefault();
    //        }
    //    }
    //    public decimal TOTAL_FEE
    //    {
    //        get
    //        {
    //            return FEE1.GetValueOrDefault() + FEE2.GetValueOrDefault();
    //        }
    //    }

    //    public decimal DOM_SHAREDMSC
    //    {
    //        get
    //        {
    //            return DOM_MSC1.GetValueOrDefault() - DOM_UNSHAREDMSC.GetValueOrDefault();
    //        }
    //    }
    //    //public decimal INT_UNSHAREDMSC { get; set; }

    //    //public decimal INT_SHAREDMSC
    //    //{
    //    //    get
    //    //    {
    //    //        return (decimal)INT_MSCVALUE - INT_UNSHAREDMSC;
    //    //    }
    //    //}

      
    //    public decimal DOM_SHAREDCAP
    //    {
    //        get
    //        {
    //            return DOM_CAP.GetValueOrDefault() - DOM_UNSHAREDCAP.GetValueOrDefault();
    //        }
    //    }
    //    public decimal AMOUNTDUEMERCH_PERC { get; set; }
    //    public decimal AMOUNTDUEOTHER_PERC
    //    {
    //        get
    //        {

    //            return 100 - AMOUNTDUEMERCH_PERC;
    //        }
    //    }
    //    public string APPLYMERCHANTSHARING { get; set; }
    //    public string AMOUNTDUEMERCH_TYPE { get; set; }
    //    public decimal FEE1_SHARED
    //    {
    //        get
    //        {
    //            return FEE1.GetValueOrDefault() - UNSHAREDFEE1.GetValueOrDefault();
    //        }
    //    }
    //    //public decimal INT_UNSHAREDCAP { get; set; }
    //    //public decimal INT_SHAREDCAP
    //    //{
    //    //    get
    //    //    {

    //    //        return INTLCAP ?? 0 - INT_UNSHAREDCAP;
    //    //    }
    //    //}
    //}
}
