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
    
    public partial class ATM_OPERATORTYPE
    {
        public int ITBID { get; set; }
        public string OPERATORTYPE_CODE { get; set; }
        public string OPERATORTYPE_DESC { get; set; }
        public Nullable<int> OPERATIONMODE_ID { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string STATUS { get; set; }
        public Nullable<bool> IS_PROCESSOR { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string LAST_MODIFIED_AUTHID { get; set; }
        public string BATCHID { get; set; }
    }
}
