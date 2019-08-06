//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SettlementMaster.Console
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUser
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int RoleId { get; set; }
        public bool ForcePassword { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> LastLogoutDate { get; set; }
        public bool LoggedOn { get; set; }
        public string MobileNo { get; set; }
        public Nullable<System.DateTime> PasswordExpiryDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Last_Modified_UID { get; set; }
        public string Last_Auth_UID { get; set; }
        public string Status { get; set; }
        public string RoleName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string FullName { get; set; }
        public Nullable<int> EnforcePasswordChangeDays { get; set; }
        public Nullable<System.DateTime> LastPasswordChangeDate { get; set; }
        public string CreateUserId { get; set; }
        public int ItbId { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
    }
}
