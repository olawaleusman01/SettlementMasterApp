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
    
    public partial class SM_MCC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SM_MCC()
        {
            this.SM_MERCHANTDETAIL = new HashSet<SM_MERCHANTDETAIL>();
            this.SM_MCCMSC = new HashSet<SM_MCCMSC>();
            this.SM_MERCHANTMSC = new HashSet<SM_MERCHANTMSC>();
        }
    
        public int ITBID { get; set; }
        public string MCC_CODE { get; set; }
        public string MCC_DESC { get; set; }
        public string SECTOR_CODE { get; set; }
        public string MCC_CAT { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public string USERID { get; set; }
        public string STATUS { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string LAST_MODIFIED_AUTHID { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_MERCHANTDETAIL> SM_MERCHANTDETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_MCCMSC> SM_MCCMSC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_MERCHANTMSC> SM_MERCHANTMSC { get; set; }
    }
}
