//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class SM_SETTLEMENTRULE
    {
        public int ITBID { get; set; }
        public Nullable<int> SETTLEMENTOPTION_ID { get; set; }
        public string PARTYTYPE_CODE { get; set; }
        public Nullable<decimal> PARTYTYPE_VALUE { get; set; }
        public Nullable<decimal> PARTYTYPE_CAP { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string STATUS { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string LAST_MODIFIED_AUTHID { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED_DATE { get; set; }
        public string BATCHID { get; set; }
    
        public virtual SM_SETTLEMENTOPTION SM_SETTLEMENTOPTION { get; set; }
    }
}
