using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace SettlementMaster.App.Models
{
    public class ClaimTypeCustomize 
    {
        public static string UserItbid 
        {get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/UserItbid";
            }
        }
     

        public static string UserRole
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/UserRole";
            }
        }
        public static string IsSupervisor
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/IsSupervisor";
            }
        }

        public static string RoleName
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/RoleName";
            }
        }
      

        public static string LastLoginDate
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/LastLoginDate";
            }
        }

        public static string DeptCode
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/DeptCode";
            }
        }
        public static string DeptName
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/DeptName";
            }
        }
      

        public static string FullName
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/FullName";
            }
        }
        public static string InstitutionId
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/InstitutionId";
            }
        }
        public static string InstitutionName
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/InstitutionName";
            }
        }
        public static string IsUpUser
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/IsUpUser";
            }
        }
        public static string MerchantId
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/MerchantId";
            }
        }
        public static string LastName
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/LastName";
            }
        }
        public static string FirstName
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/FirstName";
            }
        }
        public static string Email
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Email";
            }
        }
        public static string AppKey
        {
            get
            {
                return "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/AppKey";
            }
        }


    }

    public class UserDataObj
    {
        public int UserItbId { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public bool IsUpUser { get; set; }

        public string MerchantId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public  int UserRole { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public  bool IsSupervisor { get; set; }
        public  string RoleName { get; set; }
        public string LastLoginDateString { get { return LastLoginDate.ToString("dd-MMM-yyyy") + " at " + LastLoginDate.ToShortTimeString(); } }
        //public string FullName {
        //    get
        //    {
        //        return LastName + " " + FirstName;
        //    }
        //}

        public string Email { get; set; }
        //public int AgentParentId { get; set; }
        public  DateTime LastLoginDate { get; set; }
        //public string AgentSettlementAcct { get; set; }
        //public string AgentSettlementAcctType { get; set; }
        public string AppKey { get; set; }
    }
    public  class UserDataSettings
    {

        public  UserDataObj GetUserData()
        {
            try
            {
                int roleId, userItbid = 0;
                int instItbId = 0;
                DateTime lastLoginDate;
                var prin = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var cl = prin.Claims.ToList();
                if (cl != null)
                {                
                    var userRole = cl.Where(r => r.Type == ClaimTypeCustomize.UserRole).Select(c => c.Value).SingleOrDefault();                 
                    var rlName = cl.Where(r => r.Type == ClaimTypeCustomize.RoleName).Select(c => c.Value).SingleOrDefault();
                    var loginDate = cl.Where(r => r.Type == ClaimTypeCustomize.LastLoginDate).Select(c => c.Value).SingleOrDefault();
                   // var fullName = cl.Where(r => r.Type == ClaimTypeCustomize.FullName).Select(c => c.Value).SingleOrDefault();
                    var firstName = cl.Where(r => r.Type == ClaimTypeCustomize.FirstName).Select(c => c.Value).SingleOrDefault();
                    var lastName = cl.Where(r => r.Type == ClaimTypeCustomize.LastName).Select(c => c.Value).SingleOrDefault();                  
                    var email = cl.Where(r => r.Type == ClaimTypeCustomize.Email).Select(c => c.Value).SingleOrDefault();
                    var fullName = cl.Where(r => r.Type == ClaimTypeCustomize.FullName).Select(c => c.Value).SingleOrDefault();
                    var deptCode = cl.Where(r => r.Type == ClaimTypeCustomize.DeptCode).Select(c => c.Value).SingleOrDefault();
                    var deptName = cl.Where(r => r.Type == ClaimTypeCustomize.DeptName).Select(c => c.Value).SingleOrDefault();
                    var instId = cl.Where(r => r.Type == ClaimTypeCustomize.InstitutionId).Select(c => c.Value).SingleOrDefault();
                    var instName = cl.Where(r => r.Type == ClaimTypeCustomize.InstitutionName).Select(c => c.Value).SingleOrDefault();
                    var appKey = cl.Where(r => r.Type == ClaimTypeCustomize.AppKey).Select(c => c.Value).SingleOrDefault();

                    int.TryParse(userRole, out roleId);
                    int.TryParse(instId, out instItbId);
                    DateTime.TryParse(loginDate, out lastLoginDate);
                    UserDataObj obj = new UserDataObj()
                    {

                        RoleName = rlName,
                        UserItbId = userItbid,
                        UserRole = roleId,
                        LastLoginDate = lastLoginDate,
                        FullName = fullName,
                        // FullName = string.Concat(lastName," ",firstName),
                        InstitutionId = instItbId,
                        InstitutionName = instName,
                        DeptCode = deptCode,
                        DeptName = deptName,
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        IsUpUser = instItbId == 1 ? true : false,
                        AppKey = appKey
                    };
                    return obj;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
