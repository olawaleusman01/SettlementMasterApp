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
    
    public partial class SM_MERCHANTDETAIL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SM_MERCHANTDETAIL()
        {
            this.SM_MERCHANTACCT = new HashSet<SM_MERCHANTACCT>();
            this.SM_TERMINAL = new HashSet<SM_TERMINAL>();
            this.SM_REVENUEGROUP = new HashSet<SM_REVENUEGROUP>();
            this.SM_MERCHANTMSC = new HashSet<SM_MERCHANTMSC>();
        }
    
        public decimal ITBID { get; set; }
        public string MERCHANTID { get; set; }
        public string MERCHANTNAME { get; set; }
        public string CONTACTTITLE { get; set; }
        public string CONTACTNAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONENO { get; set; }
        public string ADDRESS { get; set; }
        public string BUSINESS_CODE { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string STATE_CODE { get; set; }
        public string CITY_NAME { get; set; }
        public string MCC_CODE { get; set; }
        public string GENERATED { get; set; }
        public string MERCHANT_URL { get; set; }
        public Nullable<int> INSTITUTION_ITBID { get; set; }
        public string INSTITUTION_CBNCODE { get; set; }
        public Nullable<decimal> CUSTOMERID { get; set; }
        public string SHORTCODE { get; set; }
        public Nullable<int> COLLECTION { get; set; }
        public Nullable<int> SPECIALMERCHANT { get; set; }
        public Nullable<int> TSA { get; set; }
        public string USERID { get; set; }
        public string LAST_MODIFIED_UID { get; set; }
        public string LAST_MODIFIED_AUTHID { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED_DATE { get; set; }
        public string STATUS { get; set; }
        public string BATCHID { get; set; }
        public string OLD_MID { get; set; }
        public Nullable<int> SETTLEMENT_FREQUENCY { get; set; }
        public Nullable<int> SET_DATE_TERM { get; set; }
        public Nullable<int> SET_DAYS { get; set; }
    
        public virtual SM_MCC SM_MCC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_MERCHANTACCT> SM_MERCHANTACCT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_TERMINAL> SM_TERMINAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_REVENUEGROUP> SM_REVENUEGROUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SM_MERCHANTMSC> SM_MERCHANTMSC { get; set; }
    }
}
