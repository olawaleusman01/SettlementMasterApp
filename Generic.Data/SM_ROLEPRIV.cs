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
    
    public partial class SM_ROLEPRIV
    {
        public int ROLEASSIGID { get; set; }
        public Nullable<int> MENUID { get; set; }
        public Nullable<int> ROLEID { get; set; }
        public Nullable<bool> CANINSERT { get; set; }
        public Nullable<bool> CANUPDATE { get; set; }
        public Nullable<bool> CANDELETE { get; set; }
        public Nullable<bool> CANAUTHORIZE { get; set; }
        public string USERID { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string STATUS { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public Nullable<int> INSTITUTION_ITBID { get; set; }
    }
}