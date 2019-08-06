using System.Collections.Generic;
using System.Linq;
using Generic.Dapper.Model;
using Generic.Data;
using Generic.Data.Model;

namespace Generic.Dapper.Utility
{
    public class Merchant
    {
        //private List<MerchantUpldObj> lineCollection = new List<MerchantUpldObj>();
        //public void AddItem(MerchantUpldObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //        lineCollection.Add(item
        //        );
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddListItem(List<MerchantUpldObj> items)
        //{

        //    lineCollection.AddRange(items);

        //}
        ////public void RemoveLine(int itemId)
        ////{
        ////    var count = lineCollection.Where(f => f.Item.itemId == itemId).Count();
        ////    if (count > 0)
        ////    {
        ////        lineCollection.RemoveAll(l => l.Item.itemId == itemId);
        ////    }
        ////}

        //public void UpdateItem(MerchantUpldObj item)
        //{
        //    MerchantUpldObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.ACCOUNTNAME = item.ACCOUNTNAME;
        //        line.BANKACCNO = item.BANKACCNO;
        //        line.BANKCODE = item.BANKCODE;
        //        line.BANKTYPE = item.BANKTYPE;
        //        line.BANK_URL = item.BANK_URL;
        //        line.BUISNESSOCCUPATIONCODE = item.BUISNESSOCCUPATIONCODE;
        //        line.CONTACTNAME = item.CONTACTNAME;
        //        line.CONTACTTITLE = item.CONTACTTITLE;
        //        line.EMAIL = item.EMAIL;
        //        line.EMAILALERTS = item.EMAILALERTS;
        //        line.LGA_LCDA = item.LGA_LCDA;
        //        line.MASTERCARDACQUIRERIDNUMBER = item.MASTERCARDACQUIRERIDNUMBER;
        //        line.MERCHANTCATEGORYCODE = item.MERCHANTCATEGORYCODE;
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MERCHANTNAME = item.MERCHANTNAME;
        //        line.MOBILEPHONE = item.MOBILEPHONE;
        //        line.PHYSICALADDR = item.PHYSICALADDR;
        //        line.PTSP = item.PTSP;
        //        line.SLIPFOOTER = item.SLIPFOOTER;
        //        line.SLIPHEADER = item.SLIPHEADER;
        //        line.STATECODE = item.STATECODE;
        //        line.TERMINALID = item.TERMINALID;
        //        line.TERMINALMODELCODE = item.TERMINALMODELCODE;
        //        line.TERMINALOWNERCODE = item.TERMINALOWNERCODE;
        //        line.ValidationErrorMessage = item.ValidationErrorMessage;
        //        line.ValidationErrorStatus = item.ValidationErrorStatus;
        //       // line.ValidationStatusClass = item.ValidationStatusClass;
        //        //line.ValidationStatusIcon = item.ValidationStatusIcon;
        //        line.VERVEACQUIRERIDNUMBER = item.VERVEACQUIRERIDNUMBER;
        //        line.VISAACQUIRERIDNUMBER = item.VISAACQUIRERIDNUMBER;
        //        line.RowColor = item.RowColor;
        //        line.grouplabel = item.grouplabel;
        //        line.TRANSCURRENCY = item.TRANSCURRENCY;
        //        line.PTSA = item.PTSA;
        //        line.PAYATTITUDE_ACCEPTANCE = item.PAYATTITUDE_ACCEPTANCE;

        //    }

        //}

        ////}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<MerchantUpldObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class TexMess
    {
        // private List<textmess_obj> lineCollection = new List<textmess_obj>();
        // public void AddItem(textmess_obj item)
        // {
        //     //CartLine line = lineCollection
        //     //.Where(p => p.Item.itemId == item.itemId)
        //     //.FirstOrDefault();
        //     //if (line == null)
        //     //{
        //     lineCollection.Add(item
        //     );
        //     //}
        //     //else
        //     //{
        //     //    line.Quantity += quantity;
        //     //}
        // }
        // public void AddListItem(IList<textmess_obj> items)
        // {

        //     lineCollection.AddRange(items);

        // }
        // //public void RemoveLine(int itemId)
        // //{
        // //    var count = lineCollection.Where(f => f.Item.itemId == itemId).Count();
        // //    if (count > 0)
        // //    {
        // //        lineCollection.RemoveAll(l => l.Item.itemId == itemId);
        // //    }
        // //}

        // //public void UpdateItem(textmess_obj item)
        // //{
        // //    MerchantUpldObj line = lineCollection
        // //    .Where(p => p.ITBID == item.ITBID)
        // //    .FirstOrDefault();
        // //    if (line != null)
        // //    {
        // //        line.ACCOUNTNAME = item.ACCOUNTNAME;
        // //        line.BANKACCNO = item.BANKACCNO;
        // //        line.BANKCODE = item.BANKCODE;
        // //        line.BANKTYPE = item.BANKTYPE;
        // //        line.BANK_URL = item.BANK_URL;
        // //        line.BUISNESSOCCUPATIONCODE = item.BUISNESSOCCUPATIONCODE;
        // //        line.CONTACTNAME = item.CONTACTNAME;
        // //        line.CONTACTTITLE = item.CONTACTTITLE;
        // //        line.EMAIL = item.EMAIL;
        // //        line.EMAILALERTS = item.EMAILALERTS;
        // //        line.LGA_LCDA = item.LGA_LCDA;
        // //        line.MASTERCARDACQUIRERIDNUMBER = item.MASTERCARDACQUIRERIDNUMBER;
        // //        line.MERCHANTCATEGORYCODE = item.MERCHANTCATEGORYCODE;
        // //        line.MERCHANTID = item.MERCHANTID;
        // //        line.MERCHANTNAME = item.MERCHANTNAME;
        // //        line.MOBILEPHONE = item.MOBILEPHONE;
        // //        line.PHYSICALADDR = item.PHYSICALADDR;
        // //        line.PTSP = item.PTSP;
        // //        line.SLIPFOOTER = item.SLIPFOOTER;
        // //        line.SLIPHEADER = item.SLIPHEADER;
        // //        line.STATECODE = item.STATECODE;
        // //        line.TERMINALID = item.TERMINALID;
        // //        line.TERMINALMODELCODE = item.TERMINALMODELCODE;
        // //        line.TERMINALOWNERCODE = item.TERMINALOWNERCODE;
        // //        line.ValidationErrorMessage = item.ValidationErrorMessage;
        // //        line.ValidationErrorStatus = item.ValidationErrorStatus;
        // //        // line.ValidationStatusClass = item.ValidationStatusClass;
        // //        //line.ValidationStatusIcon = item.ValidationStatusIcon;
        // //        line.VERVEACQUIRERIDNUMBER = item.VERVEACQUIRERIDNUMBER;
        // //        line.VISAACQUIRERIDNUMBER = item.VISAACQUIRERIDNUMBER;
        // //        line.RowColor = item.RowColor;
        // //        line.grouplabel = item.grouplabel;

        // //    }

        //// }

        // //}
        // //public decimal ComputeTotalValue()
        // //{
        // //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        // //}
        // public void Clear()
        // {
        //     lineCollection.Clear();
        // }
        // public IEnumerable<textmess_obj> Lines
        // {
        //     get { return lineCollection; }
        // }
    }
    public class MccMsc
    {
        private List<MccMscObj> lineCollection = new List<MccMscObj>();
        public void AddItem(MccMscObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void RemoveLine(decimal itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }

        public void MarkForDelete(decimal itemId)
        {
            MccMscObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = true;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UndoDelete(decimal itemId)
        {
            MccMscObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = false;
            }
        }
        public void UpdateItem(MccMscObj item)
        {
            MccMscObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.CARDSCHEME = item.CARDSCHEME;
                line.CARDSCHEME_DESC = item.CARDSCHEME_DESC;
                line.DomCurrencyDesc = item.DomCurrencyDesc;
                line.DomFrequencyDesc = item.DomFrequencyDesc;
                line.DOM_FREQUENCY = item.DOM_FREQUENCY;
                line.DOM_MSCVALUE = item.DOM_MSCVALUE;
                line.DOM_SETTLEMENT_CURRENCY = item.DOM_SETTLEMENT_CURRENCY;
                line.IntCurrencyDesc = item.IntCurrencyDesc;
                line.IntFrequencyDesc = item.IntFrequencyDesc;
                line.INT_FREQUENCY = item.INT_FREQUENCY;
                line.INT_MSCVALUE = item.INT_MSCVALUE;
                line.INT_SETTLEMENT_CURRENCY = item.INT_SETTLEMENT_CURRENCY;
                line.STATUS = item.USERID;
                line.INTLCAP = item.INTLCAP;
                line.DOMCAP = item.DOMCAP;
                line.NewRecord = item.NewRecord;
                line.Updated = item.Updated;
                line.Institution_Desc = item.Institution_Desc;
                //line.INSTITUTION_ID = item.INSTITUTION_ID;
                line.MSC_CALCBASIS = item.MSC_CALCBASIS;
                line.INTMSC_CALCBASIS = item.INTMSC_CALCBASIS;
                line.ChannelDesc = item.ChannelDesc;
                line.CHANNEL = item.CHANNEL;
                line.ChannelDesc = item.ChannelDesc;
            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<MccMscObj> Lines
        {
            get { return lineCollection; }
        }
    }
    public class Exchange
    {
        //private List<ExchangeRateObj> lineCollection = new List<ExchangeRateObj>();
        //public void AddItem(ExchangeRateObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(ExchangeRateObj item)
        //{
        //    ExchangeRateObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.CBN_CODE = item.CBN_CODE;
        //        line.CURRENCY_CODE = item.CURRENCY_CODE;
        //        line.INSTITUTION_ITBID = item.INSTITUTION_ITBID;
        //        line.RATE = item.RATE;
        //        line.USERID = item.USERID;
        //        line.Updated = item.Updated;
        //        line.CARDSCHEME = item.CARDSCHEME;
        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<ExchangeRateObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class BankAccount
    {
        private List<InstitutionAcctObj> lineCollection = new List<InstitutionAcctObj>();
        public void AddItem(InstitutionAcctObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void RemoveLine(long itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void MarkForDelete(int itemId)
        {
            InstitutionAcctObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = true;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UndoDelete(long itemId)
        {
            InstitutionAcctObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = false;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UpdateItem(InstitutionAcctObj item)
        {
            InstitutionAcctObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.CARDSCHEME = item.CARDSCHEME;
                line.CardSchemDesc = item.CardSchemDesc;
                line.DEFAULT_ACCOUNT = item.DEFAULT_ACCOUNT;
                line.DEPOSIT_ACCOUNTNO = item.DEPOSIT_ACCOUNTNO;
                line.DEPOSIT_BANKADDRESS = item.DEPOSIT_BANKADDRESS;
                line.DEPOSIT_BANKCODE = item.DEPOSIT_BANKCODE;
                line.DEPOSIT_BANKNAME = item.DEPOSIT_BANKNAME;
                line.DEPOSIT_COUNTRYCODE = item.DEPOSIT_COUNTRYCODE;
                line.INSTITUTION_ITBID = item.INSTITUTION_ITBID;
                line.INSTITUTIONTYPE = item.INSTITUTIONTYPE;
                line.InstitutionTypeDesc = item.InstitutionTypeDesc;
                line.INSTITUTION_ID = item.INSTITUTION_ID;
                line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.USERID;
                line.Updated = item.Updated;
            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<InstitutionAcctObj> Lines
        {
            get { return lineCollection; }
        }
    }

    public class Processor
    {
        //private List<InstitutionProcessorObj> lineCollection = new List<InstitutionProcessorObj>();
        //public void AddItem(InstitutionProcessorObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(InstitutionProcessorObj item)
        //{
        //    InstitutionProcessorObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.CARDSCHEME = item.CARDSCHEME;
        //        line.CardSchemDesc = item.CardSchemDesc;

        //        line.INSTITUTIONITBID = item.INSTITUTIONITBID;
        //        line.InstitutionName = item.InstitutionName;
        //        line.ProcessorTypeDesc = item.ProcessorTypeDesc;
        //        line.PROCESSORTYPE = item.PROCESSORTYPE;
        //        line.INSTITUTION_ID = item.INSTITUTION_ID;
        //        line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.PROCTYPE = item.PROCTYPE;

        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<InstitutionProcessorObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class RvHead
    {
        private List<RvHeadObj> lineCollection = new List<RvHeadObj>();
        public void AddItem(RvHeadObj item)
        {
            lineCollection.Add(item);
        }
        public void RemoveLine(decimal itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void MarkForDelete(decimal itemId)
        {
            RvHeadObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = true;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UndoDelete(decimal itemId)
        {
            RvHeadObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = false;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UpdateItem(RvHeadObj item)
        {
            RvHeadObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.CODE = item.CODE;
                line.DESCRIPTION = item.DESCRIPTION;
                line.ACCOUNT_ID = item.ACCOUNT_ID;
                line.RVGROUPCODE = item.RVGROUPCODE;
                line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.USERID;
                //line.CARDSCHEME = item.CARDSCHEME;
                //line.CardSchemDesc = item.CardSchemDesc;
                line.Updated = item.Updated;

            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<RvHeadObj> Lines
        {
            get { return lineCollection; }
        }
    }
    public class PartyAccount
    {
        private List<PartyAcctObj> lineCollection = new List<PartyAcctObj>();
        public void AddItem(PartyAcctObj item)
        {
            lineCollection.Add(item);
        }
        public void RemoveLine(decimal itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void MarkForDelete(decimal itemId)
        {
            PartyAcctObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = true;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UndoDelete(decimal itemId)
        {
            PartyAcctObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = false;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UpdateItem(PartyAcctObj item)
        {
            PartyAcctObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.DEFAULT_ACCOUNT = item.DEFAULT_ACCOUNT;
                line.DEPOSIT_ACCOUNTNO = item.DEPOSIT_ACCOUNTNO;
                line.DEPOSIT_BANKADDESS = item.DEPOSIT_BANKADDESS;
                line.DEPOSIT_BANKCODE = item.DEPOSIT_BANKCODE;
                line.DEPOSIT_BANKNAME = item.DEPOSIT_BANKNAME;
                line.DEPOSIT_COUNTRYCODE = item.DEPOSIT_COUNTRYCODE;
                line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.USERID;
                //line.CARDSCHEME = item.CARDSCHEME;
                //line.CardSchemDesc = item.CardSchemDesc;
                line.Updated = item.Updated;
                line.AGENT_CODE = item.AGENT_CODE;

            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<PartyAcctObj> Lines
        {
            get { return lineCollection; }
        }
    }
    public class BillerAccount
    {
        //private List<BillerAcctObj> lineCollection = new List<BillerAcctObj>();
        //public void AddItem(BillerAcctObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void MarkForDelete(long itemId)
        //{
        //    BillerAcctObj line = lineCollection
        //      .Where(p => p.ITBID == itemId)
        //      .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.Deleted = true;
        //        // lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void UndoDelete(long itemId)
        //{
        //    BillerAcctObj line = lineCollection
        //      .Where(p => p.ITBID == itemId)
        //      .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.Deleted = false;
        //        // lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void UpdateItem(BillerAcctObj item)
        //{
        //    BillerAcctObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.DEFAULT_ACCOUNT = item.DEFAULT_ACCOUNT;
        //        line.DEPOSIT_ACCOUNTNO = item.DEPOSIT_ACCOUNTNO;
        //        line.DEPOSIT_BANKADDESS = item.DEPOSIT_BANKADDESS;
        //        line.DEPOSIT_BANKCODE = item.DEPOSIT_BANKCODE;
        //        line.DEPOSIT_BANKNAME = item.DEPOSIT_BANKNAME;
        //        line.DEPOSIT_COUNTRYCODE = item.DEPOSIT_COUNTRYCODE;
        //        line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
        //        line.REVENUECODE = item.REVENUECODE;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        //line.CARDSCHEME = item.CARDSCHEME;
        //        //line.CardSchemDesc = item.CardSchemDesc;
        //        line.Updated = item.Updated;

        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<BillerAcctObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class MerchantAccount
    {
        private List<MerchantAcctObj> lineCollection = new List<MerchantAcctObj>();
        public void AddItem(MerchantAcctObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void RemoveLine(decimal itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void MarkForDelete(decimal itemId)
        {
            MerchantAcctObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = true;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UndoDelete(decimal itemId)
        {
            MerchantAcctObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = false;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void UpdateItem(MerchantAcctObj item)
        {
            MerchantAcctObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.DEFAULT_ACCOUNT = item.DEFAULT_ACCOUNT;
                line.DEPOSIT_ACCOUNTNO = item.DEPOSIT_ACCOUNTNO;
                line.DEPOSIT_BANKADDRESS = item.DEPOSIT_BANKADDRESS;
                line.DEPOSIT_BANKCODE = item.DEPOSIT_BANKCODE;
                line.DEPOSIT_BANKNAME = item.DEPOSIT_BANKNAME;
                line.DEPOSIT_COUNTRYCODE = item.DEPOSIT_COUNTRYCODE;
                line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
                line.DEPOSIT_ACCTNAME = item.DEPOSIT_ACCTNAME;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.USERID;
                line.Updated = item.Updated;
                line.SETTLEMENTCURRENCY = item.SETTLEMENTCURRENCY;
                line.CURRENCYDESC = item.CURRENCYDESC;
                line.DRACCOUNTNAME = item.DRACCOUNTNAME;
                line.DRACCOUNTNO = item.DRACCOUNTNO;
                line.DRBANKCODE = item.DRBANKCODE;

            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<MerchantAcctObj> Lines
        {
            get { return lineCollection; }
        }
    }
    public class SettlementRule
    {
        private List<SettlementRuleObj> lineCollection = new List<SettlementRuleObj>();
        public void AddItem(SettlementRuleObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void MarkForDelete(long itemId)
        {
            SettlementRuleObj line = lineCollection
              .Where(p => p.ITBID == itemId)
              .FirstOrDefault();
            if (line != null)
            {
                line.Deleted = true;
                // lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }
        public void RemoveLine(long itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }

        public void UpdateItem(SettlementRuleObj item)
        {
            SettlementRuleObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                //line. = item.CARDSCHEME;
                line.PARTYTYPE_CODE = item.PARTYTYPE_CODE;
                line.PARTYTYPE_VALUE = item.PARTYTYPE_VALUE;
                line.SETTLEMENTOPTION_ID = item.SETTLEMENTOPTION_ID;
                line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
                //line.MCCCATEGORY_CODE = item.MCCCATEGORY_CODE;
                line.PARTYTYPE_CAP = item.PARTYTYPE_CAP;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.STATUS;
                line.EVENTTYPE = item.EVENTTYPE;
                line.Updated = item.Updated;

            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<SettlementRuleObj> Lines
        {
            get { return lineCollection; }
        }
    }
    public class InstitutionRule
    {
        //    private List<InstitutionRuleObj> lineCollection = new List<InstitutionRuleObj>();
        //    public void AddItem(InstitutionRuleObj item)
        //    {
        //        //CartLine line = lineCollection
        //        //.Where(p => p.Item.itemId == item.itemId)
        //        //.FirstOrDefault();
        //        //if (line == null)
        //        //{
        //        lineCollection.Add(item);
        //        //}
        //        //else
        //        //{
        //        //    line.Quantity += quantity;
        //        //}
        //    }
        //    public void RemoveLine(long itemId)
        //    {
        //        var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //        if (count > 0)
        //        {
        //            lineCollection.RemoveAll(l => l.ITBID == itemId);
        //        }
        //    }

        //    public void UpdateItem(InstitutionRuleObj item)
        //    {
        //        InstitutionRuleObj line = lineCollection
        //        .Where(p => p.ITBID == item.ITBID)
        //        .FirstOrDefault();
        //        if (line != null)
        //        {
        //            line.CBN_CODE = item.CBN_CODE;
        //            line.PARTYTYPE_CODE = item.PARTYTYPE_CODE;
        //            line.PARTYTYPE_VALUE = item.PARTYTYPE_VALUE;
        //            // line.SETTLEMENTOPTION_ID = item.SETTLEMENTOPTION_ID;
        //            line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
        //            line.MCCCATEGORY_CODE = item.MCCCATEGORY_CODE;
        //            line.PARTYTYPE_CAP = item.PARTYTYPE_CAP;
        //            line.NewRecord = item.NewRecord;
        //            line.USERID = item.USERID;
        //            line.STATUS = item.USERID;
        //            // line.CARDSCHEME = item.CARDSCHEME;


        //        }

        //}
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<InstitutionRuleObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class ACARule
    {
        //private List<AcaRuleObj> lineCollection = new List<AcaRuleObj>();
        //public void AddItem(AcaRuleObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void MarkForDelete(long itemId)
        //{
        //    AcaRuleObj line = lineCollection
        //      .Where(p => p.ITBID == itemId)
        //      .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.Deleted = true;
        //        // lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void UndoDelete(long itemId)
        //{
        //    AcaRuleObj line = lineCollection
        //      .Where(p => p.ITBID == itemId)
        //      .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.Deleted = false;
        //        // lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void UpdateItem(AcaRuleObj item)
        //{
        //    AcaRuleObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.ACC_CODE = item.ACC_CODE;
        //        line.PARTYTYPE_CODE = item.PARTYTYPE_CODE;
        //        line.PARTYTYPE_VALUE = item.PARTYTYPE_VALUE;
        //        line.TRANSFERTYPE_ITBID = item.TRANSFERTYPE_ITBID;
        //        line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
        //        line.TransferTypeDesc = item.TransferTypeDesc;
        //        line.PARTYTYPE_CAP = item.PARTYTYPE_CAP;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //       // line.CARDSCHEME = item.CARDSCHEME;


        //    }

        //}
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<AcaRuleObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class AcquirerScheme
    {
        //private List<AcquireSchemeObj> lineCollection = new List<AcquireSchemeObj>();
        //public void AddItem(AcquireSchemeObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void MarkForDelete(long itemId)
        //{
        //    AcquireSchemeObj line = lineCollection
        //      .Where(p => p.ITBID == itemId)
        //      .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.Deleted = true;
        //        // lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void UndoDelete(long itemId)
        //{
        //    AcquireSchemeObj line = lineCollection
        //      .Where(p => p.ITBID == itemId)
        //      .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.Deleted = false;
        //        // lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}
        //public void UpdateItem(AcquireSchemeObj item)
        //{
        //    AcquireSchemeObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.CARDSCHEME = item.CARDSCHEME;
        //        line.CBN_CODE = item.CBN_CODE;
        //        line.INSTITUTION_NAME = item.INSTITUTION_NAME;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.EVENTTYPE = item.EVENTTYPE;


        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<AcquireSchemeObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class MerchantRule
    {
        //private List<MerchantRuleObj2> lineCollection = new List<MerchantRuleObj2>();
        //public void AddItem(MerchantRuleObj2 item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(MerchantRuleObj2 item)
        //{
        //    MerchantRuleObj2 line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //       // line.CARDSCHEME = item.CARDSCHEME;
        //        line.PARTYTYPE_CODE = item.PARTYTYPE_CODE;
        //        line.PARTYTYPE_VALUE = item.PARTYTYPE_VALUE;
        //        line.PARTYTYPE_CAP = item.PARTYTYPE_CAP;
        //        line.PARTY_DESC = item.PARTY_DESC;
        //        line.MCCCATEGORY_CODE = item.MCCCATEGORY_CODE;
        //        line.PARTY_LOCATOR = item.PARTY_LOCATOR;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.PARTYINST_ITBID = item.PARTYINST_ITBID;
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MCCCATEGORY_CODE = item.MCCCATEGORY_CODE;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<MerchantRuleObj2> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class SharingMSC2PartyDom
    {
        private List<SharingPartyObj> lineCollection = new List<SharingPartyObj>();
        public void AddItem(SharingPartyObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void AddRange(List<SharingPartyObj> items)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.AddRange(items);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void RemoveLine(decimal itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }

        public void UpdateItem(SharingPartyObj item)
        {
            SharingPartyObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.MERCHANTID = item.MERCHANTID;
                line.MerchantName = item.MerchantName;
                line.PARTYITBID = item.PARTYITBID;
                line.PartyName = item.PartyName;
                line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.USERID;
                line.SHARINGVALUE = item.SHARINGVALUE;
                line.TRANTYPE = item.TRANTYPE;
                line.RECORDID = item.ITBID;
                line.CAP = item.CAP;
                line.ACCOUNT_ID = item.ACCOUNT_ID;
                line.ACCOUNT_ID2 = item.ACCOUNT_ID2;
                line.sharingRateAccount1 = item.sharingRateAccount1;
                line.sharingRateAccount2 = item.sharingRateAccount2;
                line.splitincome = item.splitincome;

            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<SharingPartyObj> Lines
        {
            get { return lineCollection; }
        }

    }
    public class SharingBLMSC2Party
    {
        //private List<SharingBlPartyObj> lineCollection = new List<SharingBlPartyObj>();
        //public void AddItem(SharingBlPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingBlPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingBlPartyObj item)
        //{
        //    SharingBlPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //        //line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.ITBID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingBlPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class SharingMSC2PartyInt
    {
        private List<SharingPartyObj> lineCollection = new List<SharingPartyObj>();
        public void AddItem(SharingPartyObj item)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.Add(item);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void AddRange(List<SharingPartyObj> items)
        {
            //CartLine line = lineCollection
            //.Where(p => p.Item.itemId == item.itemId)
            //.FirstOrDefault();
            //if (line == null)
            //{
            lineCollection.AddRange(items);
            //}
            //else
            //{
            //    line.Quantity += quantity;
            //}
        }
        public void RemoveLine(long itemId)
        {
            var count = lineCollection.Where(f => f.ITBID == itemId).Count();
            if (count > 0)
            {
                lineCollection.RemoveAll(l => l.ITBID == itemId);
            }
        }

        public void UpdateItem(SharingPartyObj item)
        {
            SharingPartyObj line = lineCollection
            .Where(p => p.ITBID == item.ITBID)
            .FirstOrDefault();
            if (line != null)
            {
                line.MERCHANTID = item.MERCHANTID;
                line.MerchantName = item.MerchantName;
                line.PARTYITBID = item.PARTYITBID;
                line.PartyName = item.PartyName;
                line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
                line.NewRecord = item.NewRecord;
                line.USERID = item.USERID;
                line.STATUS = item.USERID;
                line.SHARINGVALUE = item.SHARINGVALUE;
                line.TRANTYPE = item.TRANTYPE;
                line.RECORDID = item.RECORDID;



            }

        }
        //public decimal ComputeTotalValue()
        //{
        //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //}
        public void Clear()
        {
            lineCollection.Clear();
        }
        public List<SharingPartyObj> Lines
        {
            get { return lineCollection; }
        }
    }
    public class SharingFEE2PartyInt
    {
        //private List<SharingBlPartyObj> lineCollection = new List<SharingBlPartyObj>();
        //public void AddItem(SharingBlPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingBlPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingBlPartyObj item)
        //{
        //    SharingBlPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //       // line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingBlPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class SharingUnsharedMscPartyDom
    {
        //private List<SharingPartyObj> lineCollection = new List<SharingPartyObj>();
        //public void AddItem(SharingPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingPartyObj item)
        //{
        //    SharingPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //        line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class SharingUnsharedBLMsc1Party
    {
        //private List<SharingBlPartyObj> lineCollection = new List<SharingBlPartyObj>();
        //public void AddItem(SharingBlPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingBlPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingBlPartyObj item)
        //{
        //    SharingBlPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //       // line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingBlPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class SharingUnsharedMscPartyInt
    {
        //    private List<SharingPartyObj> lineCollection = new List<SharingPartyObj>();
        //    public void AddItem(SharingPartyObj item)
        //    {
        //        //CartLine line = lineCollection
        //        //.Where(p => p.Item.itemId == item.itemId)
        //        //.FirstOrDefault();
        //        //if (line == null)
        //        //{
        //        lineCollection.Add(item);
        //        //}
        //        //else
        //        //{
        //        //    line.Quantity += quantity;
        //        //}
        //    }
        //    public void AddRange(List<SharingPartyObj> items)
        //    {
        //        //CartLine line = lineCollection
        //        //.Where(p => p.Item.itemId == item.itemId)
        //        //.FirstOrDefault();
        //        //if (line == null)
        //        //{
        //        lineCollection.AddRange(items);
        //        //}
        //        //else
        //        //{
        //        //    line.Quantity += quantity;
        //        //}
        //    }
        //    public void RemoveLine(long itemId)
        //    {
        //        var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //        if (count > 0)
        //        {
        //            lineCollection.RemoveAll(l => l.ITBID == itemId);
        //        }
        //    }

        //    public void UpdateItem(SharingPartyObj item)
        //    {
        //        SharingPartyObj line = lineCollection
        //        .Where(p => p.ITBID == item.ITBID)
        //        .FirstOrDefault();
        //        if (line != null)
        //        {
        //            line.MERCHANTID = item.MERCHANTID;
        //            line.MerchantName = item.MerchantName;
        //            line.PARTYITBID = item.PARTYITBID;
        //            line.PartyName = item.PartyName;
        //            line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //            line.NewRecord = item.NewRecord;
        //            line.USERID = item.USERID;
        //            line.STATUS = item.USERID;
        //            line.SHARINGVALUE = item.SHARINGVALUE;
        //            line.TRANTYPE = item.TRANTYPE;
        //            line.RECORDID = item.RECORDID;



        //        }

        //    }
        //    //public decimal ComputeTotalValue()
        //    //{
        //    //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //    //}
        //    public void Clear()
        //    {
        //        lineCollection.Clear();
        //    }
        //    public List<SharingPartyObj> Lines
        //    {
        //        get { return lineCollection; }
        //    }
    }
    public class SharingUnsharedFEE1Party
    {
        //private List<SharingBlPartyObj> lineCollection = new List<SharingBlPartyObj>();
        //public void AddItem(SharingBlPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingBlPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingBlPartyObj item)
        //{
        //    SharingBlPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //        // line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingBlPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class SharingSubsidyPartyInt
    {
        //private List<SharingPartyObj> lineCollection = new List<SharingPartyObj>();
        //public void AddItem(SharingPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingPartyObj item)
        //{
        //    SharingPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //        line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class SharingSubsidyFEE1Party
    {
        //private List<SharingBlPartyObj> lineCollection = new List<SharingBlPartyObj>();
        //public void AddItem(SharingBlPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingBlPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingBlPartyObj item)
        //{
        //    SharingBlPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //       // line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingBlPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class SharingSubsidyPartyDom
    {
        //    private List<SharingPartyObj> lineCollection = new List<SharingPartyObj>();
        //    public void AddItem(SharingPartyObj item)
        //    {
        //        //CartLine line = lineCollection
        //        //.Where(p => p.Item.itemId == item.itemId)
        //        //.FirstOrDefault();
        //        //if (line == null)
        //        //{
        //        lineCollection.Add(item);
        //        //}
        //        //else
        //        //{
        //        //    line.Quantity += quantity;
        //        //}
        //    }
        //    public void AddRange(List<SharingPartyObj> items)
        //    {
        //        //CartLine line = lineCollection
        //        //.Where(p => p.Item.itemId == item.itemId)
        //        //.FirstOrDefault();
        //        //if (line == null)
        //        //{
        //        lineCollection.AddRange(items);
        //        //}
        //        //else
        //        //{
        //        //    line.Quantity += quantity;
        //        //}
        //    }
        //    public void RemoveLine(long itemId)
        //    {
        //        var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //        if (count > 0)
        //        {
        //            lineCollection.RemoveAll(l => l.ITBID == itemId);
        //        }
        //    }

        //    public void UpdateItem(SharingPartyObj item)
        //    {
        //        SharingPartyObj line = lineCollection
        //        .Where(p => p.ITBID == item.ITBID)
        //        .FirstOrDefault();
        //        if (line != null)
        //        {
        //            line.MERCHANTID = item.MERCHANTID;
        //            line.MerchantName = item.MerchantName;
        //            line.PARTYITBID = item.PARTYITBID;
        //            line.PartyName = item.PartyName;
        //            line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //            line.NewRecord = item.NewRecord;
        //            line.USERID = item.USERID;
        //            line.STATUS = item.USERID;
        //            line.SHARINGVALUE = item.SHARINGVALUE;
        //            line.TRANTYPE = item.TRANTYPE;
        //            line.RECORDID = item.RECORDID;



        //        }

        //    }
        //    //public decimal ComputeTotalValue()
        //    //{
        //    //    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        //    //}
        //    public void Clear()
        //    {
        //        lineCollection.Clear();
        //    }
        //    public List<SharingPartyObj> Lines
        //    {
        //        get { return lineCollection; }
        //    }
    }
    public class SharingSubsidyPartyMsc1
    {
        //private List<SharingBlPartyObj> lineCollection = new List<SharingBlPartyObj>();
        //public void AddItem(SharingBlPartyObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<SharingBlPartyObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(SharingBlPartyObj item)
        //{
        //    SharingBlPartyObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MerchantName = item.MerchantName;
        //        line.PARTYITBID = item.PARTYITBID;
        //        line.PartyName = item.PartyName;
        //        // line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.SHARINGVALUE = item.SHARINGVALUE;
        //        line.TRANTYPE = item.TRANTYPE;
        //        line.RECORDID = item.RECORDID;



        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<SharingBlPartyObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class BillerMerchantMsc
    {
        //private List<BillerMscObj> lineCollection = new List<BillerMscObj>();
        //public void AddItem(BillerMscObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void AddRange(List<BillerMscObj> items)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.AddRange(items);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(BillerMscObj item)
        //{
        //    BillerMscObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MERCHANTNAME = item.MERCHANTNAME;
        //        line.AMOUNTDUEMERCH_PERC = item.AMOUNTDUEMERCH_PERC;
        //        line.AMOUNTDUEMERCH_TYPE = item.AMOUNTDUEMERCH_TYPE;
        //        // line.LASTMODIFIED_UID = item.LASTMODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.APPLYMERCHANTSHARING = item.APPLYMERCHANTSHARING;
        //        line.RECORDID = item.RECORDID;
        //        line.BILLER_CODE = item.BILLER_CODE;
        //        line.CBN_CODE = item.CBN_CODE;
        //        line.CHANNEL = item.CHANNEL;
        //        line.CHANNEL_DESC = item.CHANNEL_DESC;
        //        line.CREATEDATE = item.CREATEDATE;
        //        line.DATECREATED = item.DATECREATED;
        //        line.DOM_CAP = item.DOM_CAP;
        //        line.DOM_MSC1 = item.DOM_MSC1;
        //        line.DOM_MSC2 = item.DOM_MSC2;
        //        line.DOM_MSCSUBSIDY = item.DOM_MSCSUBSIDY;
        //        line.DOM_MSC_CALCBASIS = item.DOM_MSC_CALCBASIS;
        //        line.DOM_UNSHAREDCAP = item.DOM_UNSHAREDCAP;
        //        line.DOM_UNSHAREDMSC = item.DOM_UNSHAREDMSC;
        //        line.FEE1 = item.FEE1;
        //        line.FEE2 = item.FEE2;
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.MERCHANTNAME = item.MERCHANTNAME;
        //        line.UNSHAREDFEE1 = item.UNSHAREDFEE1;
        //        line.Updated = item.Updated;
        //        // line.APPLYMERCHANTSHARING = item.APPLYMERCHANTSHARING;


        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<BillerMscObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class MCCRule
    {
        //private List<MCCRuleObj> lineCollection = new List<MCCRuleObj>();
        //public void AddItem(MCCRuleObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void RemoveLine(long itemId)
        //{
        //    var count = lineCollection.Where(f => f.ITBID == itemId).Count();
        //    if (count > 0)
        //    {
        //        lineCollection.RemoveAll(l => l.ITBID == itemId);
        //    }
        //}

        //public void UpdateItem(MCCRuleObj item)
        //{
        //    MCCRuleObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.CARDSCHEME = item.CARDSCHEME;
        //        line.PARTYTYPE_CODE = item.PARTYTYPE_CODE;
        //        line.PARTYTYPE_VALUE = item.PARTYTYPE_VALUE;
        //      //  line.SETTLEMENTOPTION_ID = item.SETTLEMENTOPTION_ID;
        //        line.LAST_MODIFIED_UID = item.LAST_MODIFIED_UID;
        //        line.NewRecord = item.NewRecord;
        //        line.USERID = item.USERID;
        //        line.STATUS = item.USERID;
        //        line.CARDSCHEME = item.CARDSCHEME;
        //        line.MCC_CODE = item.MCC_CODE;
        //        line.INSTITUTION_ID = item.INSTITUTION_ID;


        //    }

        //}
        ////public decimal ComputeTotalValue()
        ////{
        ////    return lineCollection.Sum(e => e.Item.Price * e.Quantity);
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public List<MCCRuleObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class MerchantLine
    {
        //public MerchantUpldObj Item { get; set; }

    }
    public class MerchantUpdate
    {
        //private List<MerchantUpdateObj> lineCollection = new List<MerchantUpdateObj>();
        //public void AddItem(MerchantUpdateObj item)
        //{
        //    //CartLine line = lineCollection
        //    //.Where(p => p.Item.itemId == item.itemId)
        //    //.FirstOrDefault();
        //    //if (line == null)
        //    //{
        //    lineCollection.Add(item);
        //    //}
        //    //else
        //    //{
        //    //    line.Quantity += quantity;
        //    //}
        //}
        //public void UpdateItem(MerchantUpdateObj item)
        //{
        //    MerchantUpdateObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.FIELD_NAME = item.FIELD_NAME;
        //        line.FIELD_VALUE = item.FIELD_VALUE;
        //        line.TERMINALID = item.TERMINALID;


        //    }

        //}
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<MerchantUpdateObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class MCCMSCUpload
    {
        //private List<MCCUpldObj> lineCollection = new List<MCCUpldObj>();
        //public void AddItem(MCCUpldObj item)
        //{

        //    lineCollection.Add(item);

        //}

        //public void UpdateItem(MCCUpldObj item)
        //{
        //    MCCUpldObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.ValidationErrorStatus = item.ValidationErrorStatus;


        //    }

        //}
        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<MCCUpldObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class RevenueHeadUpload
    {
        //private List<RevenueUpldObj> lineCollection = new List<RevenueUpldObj>();
        //public void AddItem(RevenueUpldObj item)
        //{

        //    lineCollection.Add(item);

        //}
        //public void UpdateItem(RevenueUpldObj item)
        //{
        //    RevenueUpldObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {

        //        line.ValidationErrorMessage = item.ValidationErrorMessage;
        //        line.ValidationErrorStatus = item.ValidationErrorStatus;
        //        // line.ValidationStatusClass = item.ValidationStatusClass;
        //        //line.ValidationStatusIcon = item.ValidationStatusIcon;

        //    }

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<RevenueUpldObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class DepartmentUpload
    {
        //private List<POSMISDB_DEPARTMENTTEMP> lineCollection = new List<POSMISDB_DEPARTMENTTEMP>();
        //public void AddItem(POSMISDB_DEPARTMENTTEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_DEPARTMENTTEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class RoleUpload
    {
        //private List<POSMISDB_ROLES> lineCollection = new List<POSMISDB_ROLES>();
        //public void AddItem(POSMISDB_ROLES item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_ROLES> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class CurrencyUpload
    {
        //private List<POSMISDB_CURRENCYTEMP> lineCollection = new List<POSMISDB_CURRENCYTEMP>();
        //public void AddItem(POSMISDB_CURRENCYTEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_CURRENCYTEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class MidTidDeleteUpload
    {
        //private List<MidTidDeleteObj> lineCollection = new List<MidTidDeleteObj>();
        //public void AddItem(MidTidDeleteObj item)
        //{

        //    lineCollection.Add(item);

        //}

        //public void UpdateItem(MidTidDeleteObj item)
        //{
        //    MidTidDeleteObj line = lineCollection
        //    .Where(p => p.ITBID == item.ITBID)
        //    .FirstOrDefault();
        //    if (line != null)
        //    {
        //        line.FIELD_LABEL = item.FIELD_LABEL;
        //        line.FIELD_VALUE = item.FIELD_VALUE;
        //        line.ValidationErrorMessage = item.ValidationErrorMessage;
        //        line.ValidationErrorStatus = item.ValidationErrorStatus;
        //        line.MERCHANTID = item.MERCHANTID;
        //        line.TIDCOUNT = item.TIDCOUNT;
        //        // line.ValidationStatusClass = item.ValidationStatusClass;
        //        //line.ValidationStatusIcon = item.ValidationStatusIcon;

        //    }

        //}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<MidTidDeleteObj> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class FrequencyUpload
    {
        //private List<POSMISDB_FREQUENCYTEMP> lineCollection = new List<POSMISDB_FREQUENCYTEMP>();
        //public void AddItem(POSMISDB_FREQUENCYTEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_FREQUENCYTEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class CardSchemeUpload
    {
        //private List<POSMISDB_CARDSCHEMETEMP> lineCollection = new List<POSMISDB_CARDSCHEMETEMP>();
        //public void AddItem(POSMISDB_CARDSCHEMETEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_CARDSCHEMETEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class PartyTypeUpload
    {
        //private List<POSMISDB_PARTYTYPETEMP> lineCollection = new List<POSMISDB_PARTYTYPETEMP>();
        //public void AddItem(POSMISDB_PARTYTYPETEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_PARTYTYPETEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }

    public class CountryUpload
    {
        //private List<POSMISDB_COUNTRYTEMP> lineCollection = new List<POSMISDB_COUNTRYTEMP>();
        //public void AddItem(POSMISDB_COUNTRYTEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_COUNTRYTEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class StateUpload
    {
        //private List<POSMISDB_STATETEMP> lineCollection = new List<POSMISDB_STATETEMP>();
        //public void AddItem(POSMISDB_STATETEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_STATETEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
    public class ProductTypeUpload
    {
        //private List<POSMISDB_PRODUCT_TYPETEMP> lineCollection = new List<POSMISDB_PRODUCT_TYPETEMP>();
        //public void AddItem(POSMISDB_PRODUCT_TYPETEMP item)
        //{

        //    lineCollection.Add(item);

        //}

        ////}
        //public void Clear()
        //{
        //    lineCollection.Clear();
        //}
        //public IEnumerable<POSMISDB_PRODUCT_TYPETEMP> Lines
        //{
        //    get { return lineCollection; }
        //}
    }
}


