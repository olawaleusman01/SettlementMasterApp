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
    
    public partial class SM_RevenuHeadPartyTemp
    {
        public long ItbId { get; set; }
        public Nullable<int> PartyId { get; set; }
        public Nullable<decimal> PartyValue { get; set; }
        public Nullable<int> PartyAccountId { get; set; }
        public Nullable<int> RvCodeItbId { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UserId { get; set; }
        public string BatchId { get; set; }
    
        public virtual SM_REVENUEHEAD SM_REVENUEHEAD { get; set; }
    }
}
