using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SettlementMaster.App.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        public int ItbId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public bool Supervisor { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
     //   [Not Required]
        public int RoleId { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string MobileNo { get; set; }

        public string RoleName { get; set; }
        public string FullName
        {
            get { return string.Concat(LastName, " ", FirstName); }
        }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Login Id")]
        public string LoginId { get; set; }
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Login ID")]
        public string LoginId { get; set; }
    }
}
