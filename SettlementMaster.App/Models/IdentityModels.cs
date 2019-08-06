using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettlementMaster.App.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int RoleId { get; set; }
        public bool ForcePassword { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public bool LoggedOn { get; set; }
        public string MobileNo { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Last_Modified_UID { get; set; }
        public string Last_Auth_UID { get; set; }
        public string Status { get; set; }
        public string RoleName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public bool Supervisor { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string FullName { get; set; }
        public int? EnforcePasswordChangeDays { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public string CreateUserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItbId { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.UserRole, RoleId.ToString()));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.RoleName, RoleName ?? ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.LastLoginDate, LastLoginDate != null ? LastLoginDate.ToString() : ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.Email, Email != null ? Email.ToString() : ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.LastName, LastName ?? ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.FirstName, FirstName ?? ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.FullName, FullName ?? ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.InstitutionId, InstitutionId.ToString()));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.InstitutionName, InstitutionName ?? ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.DeptCode, DeptCode ?? ""));
            userIdentity.AddClaim(new Claim(ClaimTypeCustomize.DeptName, DeptName ?? ""));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}