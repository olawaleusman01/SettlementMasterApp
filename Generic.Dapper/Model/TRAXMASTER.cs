using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public partial class TRAXMASTER
    {
        public long ID { get; set; }
        public System.DateTime SYSDATE { get; set; }
        public string REFNO { get; set; }
        public Nullable<int> CHANNELID { get; set; }
        public string CARDSCHEME { get; set; }
        public string TRANCODE { get; set; }
        public string TRANDESC { get; set; }
        public DateTime TRANDATETIME { get; set; }
        public string ISSUERFIID { get; set; }
        public string ISSUERCURRENCY { get; set; }
        public string ACQUIRERFIID { get; set; }
        public string ACQUIRERCURRENCY { get; set; }
        public string PAN { get; set; }
        public Nullable<System.DateTime> EXPDATETIME { get; set; }
        public string APPROVALCODE { get; set; }
        public string APPROVALMESSAGE { get; set; }
        public string TERMINALID { get; set; }
        public string MERCHANTID { get; set; }
        public string MERCHANTLOCATION { get; set; }
        public string MCC { get; set; }
        public string STAN { get; set; }
        public string INVOICENO { get; set; }
        public string CUSTOMERNAME { get; set; }
        public Nullable<decimal> ORGINALAMOUNT { get; set; }
        public String ORGINALCURRENCY { get; set; }
        public Nullable<decimal> TRANAMOUNT { get; set; }
        public string TRANCURRENCY { get; set; }
        public string SIGN { get; set; }
        public Nullable<decimal> ISSUERFEE { get; set; }
        public string ISSUERFEECURRENCY { get; set; }
        public Nullable<decimal> ACQUIRERFEE { get; set; }
        public string ACQUIRERFEECURRENCY { get; set; }
        public Nullable<decimal> INTERCHANGEFEE { get; set; }
        public string INTERCHANGEFEECUR { get; set; }
        public string SPECIALMESSAGE1 { get; set; }
        public string SPECIALMESSAGE2 { get; set; }
        public string SPECIALMESSAGE3 { get; set; }
        public string SPECIALMESSAGE4 { get; set; }
        public string SOURCEDB { get; set; }
        public string SOURCETABLE { get; set; }
        public string REMARK { get; set; }
        public Nullable<decimal> RECORDID { get; set; }
        public Nullable<decimal> BATCHNO { get; set; }
        public Nullable<System.DateTime> CREATEDATETIME { get; set; }
        public string AGENT_CODE { get; set; }
        public string PAYMENTREFERENCE { get; set; }
        public string TRANSID { get; set; }
        public string ISVALUEGRANTED { get; set; }
        public DateTime? VALUEDATE { get; set; }
        public string BRANCHID { get; set; }
        public long? PAYMENTITEMID { get; set; }
    }
}
