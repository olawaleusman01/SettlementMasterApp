using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SettlementMaster.App.Models;
using Generic.Dapper.Model;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using Generic.Dapper.Data;
using Generic.Data.Utilities;
using Generic.Data;
//using System.Transactions;
using Generic.Dapper.Utility;
using Generic.Data.Model;
using Generic.Dapper.Utilities;

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        string fullName, deptCode;
        private readonly IUnitOfWork uow = null;
        private readonly IRepository<AspNetUser> repoUser = null;
        private readonly IRepository<SM_ASPNETUSERSTEMP> repoUserTemp = null;
        private readonly IRepository<SM_AUTHLIST> repoAuth = null;
        private readonly IRepository<SM_AUTHCHECKER> repoChecker = null;
        private readonly IRepository<SM_INSTITUTION> repoInst = null;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        string active = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.ACTIVE);
        string inActive = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.INACTIVE);
        string open = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.OPEN);
        string close = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.CLOSED);
        string approve = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.APPROVED);
        string reject = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.REJECTED);
        readonly string eventInsert = "New";
        readonly string eventEdit = "Modify";
        readonly string eventDelete = "Delete";
        const string Single = "SINGLE";
        const string Batch = "BATCH";
        int menuId = 7;
        int institutionId;
        int roleId;
        int checkerNo = 1;
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        ILoginMultiple _repoL = new LoginMultiple();

        public AccountController()
        {
            uow = new UnitOfWork();
            repoInst = new Repository<SM_INSTITUTION>(uow);
            repoUser = new Repository<AspNetUser>(uow);
            repoUserTemp = new Repository<SM_ASPNETUSERSTEMP>(uow);
            repoAuth = new Repository<SM_AUTHLIST>(uow);
            repoChecker = new Repository<SM_AUTHCHECKER>(uow);
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleId = user.UserRole;
                institutionId = user.InstitutionId;
                fullName = user.FullName;
                deptCode = user.DeptCode;
            }
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if(!string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index","Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("Login2");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var sessId = "";
                var passChangeDays = 0;
                var bt = _repo.GetCompanyProfile(0, false);
                if (bt != null)
                {
                    passChangeDays = bt.PASSWORD_CHANGE_DAYS.GetValueOrDefault();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var retUrl = "";
                var user = await UserManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if ((user.Status??"").ToLower() != "active")
                    {
                        return Json(new { RespCode = 1, RespMessage = "Your Account is Locked Out.Contact Administrator for further advice" });
                    }
                    bool passwordExpire = CheckPasswordExpire(user.LastPasswordChangeDate, passChangeDays);
                    if (user.ForcePassword || passwordExpire)
                    {
                        retUrl = Url.Action("ResetPassword", "Account");
                        return Json(new { RespCode = 2, RespMessage = "Password Change Is Required.You will be redirected to another page to change your Password.", ReturnUrl = retUrl,UserName = model.UserName });
                    }
                    user.AppKey = model.App;
                    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
                   
                    switch (result)
                    {
                        case SignInStatus.Success:
                            {
                              
                                var guid = Guid.NewGuid();
                                Session["guidno"] = guid;
                                try
                                {
                                    user.LoggedOn = true;
                                    user.LastLoginDate = DateTime.Now;

                                    IdentityResult result2 = UserManager.Update(user);

                                    var obj = new LoginAuditObj()
                                    {
                                        LOGINDATE = DateTime.Now,
                                        UserId = model.UserName,
                                        BROWSER = SmartObj.GetBrowser(Request),
                                        IPADDRESS = getip(),
                                        MAC = "",
                                        guid = guid.ToString()
                                    };
                                    _repo.PostLoginAudit(obj, 1);
                                }
                                catch (Exception ex)
                                {

                                }
                                sessId = System.Web.HttpContext.Current.Session.SessionID;
                                _repoL.PostLogins(model.UserName, sessId);
                                Session["sessionid"] = sessId;
                                if (Url.IsLocalUrl(returnUrl))
                                {
                                    retUrl = returnUrl;
                                }
                                else
                                {
                                    retUrl = Url.Action("Index", "Home");
                                }
                                return Json(new { RespCode = 0, RespMessage = "Record Created Successfully", ReturnUrl = retUrl });
                                // RedirectToLocal(returnUrl);
                            }
                        case SignInStatus.LockedOut:
                            try
                            {
                                var obj = new LoginAuditObj()
                                {
                                    ATTEMPTDATE = DateTime.Now,
                                    UserId = model.UserName,
                                    BROWSER = SmartObj.GetBrowser(Request),
                                    IPADDRESS = getip(),
                                    MAC = "",
                                    guid = Guid.NewGuid().ToString()
                                };
                                _repo.PostLoginAttempt(obj);
                            }
                            catch (Exception ex)
                            {

                            }
                            return Json(new { RespCode = 1, RespMessage = "Account Locked Out..Contact System Administrator" });
                        // return View("Lockout");
                        case SignInStatus.RequiresVerification:
                        // return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            try
                            {
                                var obj = new LoginAuditObj()
                                {
                                    ATTEMPTDATE = DateTime.Now,
                                    UserId = model.UserName,
                                    BROWSER = SmartObj.GetBrowser(Request),
                                    IPADDRESS = getip(),
                                    MAC = "",
                                    guid = Guid.NewGuid().ToString(),
                                };
                                _repo.PostLoginAttempt(obj);
                            }
                            catch (Exception ex)
                            {

                            }
                            return Json(new { RespCode = 1, RespMessage = "Invalid login attempt." });
                            //ModelState.AddModelError("", "Invalid login attempt.");
                            //return View(model);
                    }
                }
                else
                {
                    return Json(new { RespCode = 1, RespMessage = "Invalid Username or Password" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Invalid login attempt." + ex.Message });
            }
        }

        private bool CheckPasswordExpire(DateTime? lastPasswordChangeDate, int passChangeDays)
        {
            var curDate = DateTime.Now;
            var newDate = lastPasswordChangeDate.GetValueOrDefault().AddDays(passChangeDays + 1);
            var rst = DateTime.Compare(curDate, newDate);
            return rst == 1; 
        }

        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to shouldLockout: true
        //    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return View(model);
        //    }
        //}

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        // POST: /AddErrors2/Register
       
        public ActionResult Register(RegisterViewModel model, string m)
        {
            try
            {
                menuId = 7;// SmartUtil.GetMenuId(m);
                bool suc = false;
                string title = "";
                var errorMsg = "";
                if (ModelState.IsValid == true)
                {
                    if (model.ItbId == 0)
                    {
                        var ct = repoUser.AllEager(f => f.UserName == model.UserName).Count();
                        var ct1 = repoUserTemp.AllEager(f => f.UserName == model.UserName && f.Status == open).Count();
                        if (ct > 0 || ct1 > 0)
                        {
                            return Json(new { RespCode = 1, RespMessage = "UserName already exist" });
                            
                        }
                        var ctE = repoUser.AllEager(f => f.Email == model.Email).Count();
                        var ctE1 = repoUserTemp.AllEager(f => f.Email == model.Email && f.Status == open).Count();
                        if (ctE > 0 || ctE1 > 0)
                        {
                            return Json(new { RespCode = 1, RespMessage = "E-Mail Address already exist" });

                        }
                        var mod = repoInst.AllEager(d => d.ITBID == model.InstitutionId).FirstOrDefault();
                        if (mod != null)
                        {
                            model.InstitutionName = mod.INSTITUTION_NAME;
                        }
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                            var user = new SM_ASPNETUSERSTEMP
                            {
                                UserName = model.UserName,
                                Email = model.Email,
                                LastName = model.LastName,
                                FirstName = model.FirstName,
                                RoleId = model.RoleId,
                                UserId = User.Identity.Name,
                                CreateDate = DateTime.Now,
                                EnforcePasswordChangeDays = 90,
                                FullName = model.FullName,
                                ForcePassword = true,
                                IsApproved = true,
                                MobileNo = model.MobileNo,
                                Status = open,
                                DeptCode = model.DeptCode,
                                DeptName = model.DeptName,
                                InstitutionId = model.InstitutionId.GetValueOrDefault(),
                                InstitutionName = model.InstitutionName,
                                RoleName = model.RoleName,
                                Supervisor = model.Supervisor,
                            };
                            //var user = new ApplicationUser
                            //{
                            //    UserName = "admin",
                            //    Email = "olawaleusman01@gmail.com",
                            //    LastName = "system",
                            //    FirstName = "system",
                            //    RoleId = 1,
                            //    CreateUserId = "admin",
                            //    CreateDate = DateTime.Now,
                            //    EnforcePasswordChangeDays = 90,
                            //    FullName = "system",
                            //    ForcePassword = true,
                            //    IsApproved = true,
                            //    MobileNo = null,
                            //    Status = "Active",
                            //    LocationItbId = 1,
                            //    LocationName = "Main Store",

                            //};
                            var pass = SmartObj.Encrypt(SmartObj.GenerateRandomPassword(8));
                            user.PasswordHash =  pass;// ;
                            repoUserTemp.Insert(user);
                            //var result = await UserManager.CreateAsync(user, pass);
                            if (uow.Save(User.Identity.Name, null) > 0)
                            {
                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = eventInsert,
                                    MENUID = menuId,
                                    //MENUNAME = "",
                                    RECORDID = user.ItbId,
                                    STATUS = open,
                                    INSTITUTION_ITBID = institutionId,
                                    // TABLENAME = "ADMIN_DEPARTMENT",
                                    URL = Request.FilePath,
                                    USERID = User.Identity.Name,
                                    POSTTYPE = Single
                                };
                                repoAuth.Insert(auth);
                                var rst1 = uow.Save(User.Identity.Name,null);
                                if (rst1 > 0)
                                {
                                  
                                    //txscope.Complete();
                                    suc = true;
                                    respMsg = "User Record Created....Authorization Pending...";
                                    title = "User Creation Request";
                                }
                                //return Json(new { RespCode = 0, RespMessage = "Record Created Successfully...Authorization Pending." });
                            }
                            //errorMsg = AddErrors2(result);
                        //}
                        if (suc)
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, title);
                            return Json(new { data = model, RespCode = 0, RespMessage = respMsg });

                        }
                    }
                    else
                    {
                        //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        //{
                               
                            var ctE = repoUser.AllEager(f => f.Email == model.Email && f.UserName != model.UserName).Count();
                            var ctE1 = repoUserTemp.AllEager(f => f.Email == model.Email  && f.Status == open && f.UserName != model.UserName).Count();
                            if (ctE > 0 || ctE1 > 0)
                            {
                                return Json(new { RespCode = 1, RespMessage = "E-Mail Address already exist" });
                            }
                            var mod = repoInst.AllEager(d => d.ITBID == model.InstitutionId).FirstOrDefault();
                            if (mod != null)
                            {
                                model.InstitutionName = mod.INSTITUTION_NAME;
                            }
                            var user = new SM_ASPNETUSERSTEMP
                            {
                                UserName = model.UserName,
                               // Email = model.Email,
                                LastName = model.LastName,
                                FirstName = model.FirstName,
                                RoleId = model.RoleId,
                                UserId = User.Identity.Name,
                                CreateDate = DateTime.Now,
                                EnforcePasswordChangeDays = 90,
                                FullName = model.FullName,
                                ForcePassword = true,
                                //IsApproved = true,
                                MobileNo = model.MobileNo,
                                Status = open,
                                DeptCode = model.DeptCode,
                                DeptName = model.DeptName,
                                InstitutionId = model.InstitutionId.GetValueOrDefault(),
                                InstitutionName = model.InstitutionName,
                                RoleName = model.RoleName,
                                Supervisor = model.Supervisor,
                                // RecordId = model.ItbId ,
                                Email = model.Email
                            };
                            
                            repoUserTemp.Insert(user);
                            //var result = await UserManager.CreateAsync(user, pass);
                            if (uow.Save(User.Identity.Name, null) > 0)
                            {
                                SM_AUTHLIST auth = new SM_AUTHLIST()
                                {
                                    CREATEDATE = DateTime.Now,
                                    EVENTTYPE = model.Status == active ? eventEdit : model.Status,
                                    MENUID = menuId,
                                    //MENUNAME = "",
                                    RECORDID = user.ItbId,
                                    STATUS = open,
                                    INSTITUTION_ITBID = institutionId,
                                    // TABLENAME = "ADMIN_DEPARTMENT",
                                    URL = Request.FilePath,
                                    USERID = User.Identity.Name,
                                    POSTTYPE = Single
                                };
                                repoAuth.Insert(auth);
                                var rst1 = uow.Save(User.Identity.Name, null);
                                if (rst1 > 0)
                                {

                                    //txscope.Complete();
                                    // EmailerNotification.SendForAuthorization(menuid, fullName, deptCode, institutionId, "User Record");
                                    suc = true;
                                    respMsg = "User Record Updated....Authorization Pending...";
                                    title = "User Modification Request";
                                }
                            }
                            //errorMsg = AddErrors2(result);
                        //}
                        if (suc)
                        {
                            EmailerNotification.SendForAuthorization(menuId, fullName, deptCode, institutionId, title);
                            return Json(new { data = model, RespCode = 0, RespMessage = respMsg });

                        }
                    }
                }
               
                // If we got this far, something failed, redisplay form
                return Json(new { RespCode = 1, RespMessage = errorMsg });
            }
            catch (SqlException ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = ex.Message });
            }
        }

        decimal authId;
        public async Task<ActionResult> DetailAuth(string a_i, string m)
        {
            try
            {
                int menuId;
                var mid = SmartObj.Decrypt(m);
                var ai = SmartObj.Decrypt(a_i);
                if (int.TryParse(mid, out menuId) && decimal.TryParse(ai, out authId))
                {
                    var obj = new AuthViewObj();
                    var det = repoAuth.Find(authId);
                    //var d = _repo.GetSession(0, true);

                    ViewBag.HeaderTitle = "Authorize Detail for User";
                    //ViewBag.StatusVisible = true;
                    if (det != null)
                    {
                        obj.AuthId = authId;
                        obj.BatchId = det.BATCHID;
                        obj.RecordId = det.RECORDID.GetValueOrDefault();
                        obj.PostType = det.POSTTYPE;
                        obj.MenuId = det.MENUID.GetValueOrDefault();
                        ViewBag.Message = TempData["msg"];
                        var stat = ViewBag.Message != null ? null : "open";
                        var rec = await _repo.GetUserAsync((int)det.RECORDID,false,isTemp:true, status: stat);  //repoSession.FindAsync(id);
                        if (rec != null && rec.Count > 0)
                        {
                            var model = rec.FirstOrDefault();
                            var inst2 = _repo.GetInstitution(0, true, "Active");
                            var role = await _repo.GetRolesAsync(0, true, "Active");
                            var dept = await _repo.GetDepartmentAsync(0, true, "Active");
                            var inst = await _repo.GetInstitutionAsync(0, true, "Active");
                            ViewBag.Institution = new SelectList(inst.ToList(), "ITBID", "INSTITUTION_NAME");
                            ViewBag.Department = new SelectList(dept.ToList(), "DEPARTMENTCODE", "DEPARTMENTNAME");
                            ViewBag.Role = new SelectList(role.ToList(), "ROLEID", "ROLENAME");
                            ViewBag.DisplayAuth = det.STATUS == open && !(model.CreateUserId == User.Identity.Name);
                            obj.Status = det.STATUS;
                            obj.EventType = det.EVENTTYPE;
                            obj.DateCreated = det.CREATEDATE.GetValueOrDefault().ToString("dd-MMM-yyyy");
                            obj.User = model.CREATED_BY;
                            ViewBag.Auth = obj;
                            return View("DetailAuth", model);

                        }
                    }


                    return View("DetailAuth");
                }
                else
                {
                    return View("Error", "Home");
                }

            }
            catch (Exception ex)
            {
                return View("DetailAuth");
            }

        }
        private string AddErrors2(IdentityResult result)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var error in result.Errors)
            {
                sb.AppendLine(error);
            }

            return sb.ToString();
        }

        // [AllowAnonymous]
        [MyAuthorize]
        public ActionResult Users(string m)
        {
            try
            {
                menuId = SmartUtil.GetMenuId(m);
                if (menuId == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.MenuId = menuId;
                GetPriv();
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        void GetPriv()
        {
            var rec = _repo.GetMenuPrivilege(menuId, roleId);
            if (rec != null)
            {
                ViewBag.CanAdd = rec.CanAdd;
                ViewBag.CanEdit = rec.CanEdit;
            }
        }
        // [AllowAnonymous]
        public async Task<PartialViewResult> Add(int? id, string m = null)
        {
            try
            {
                ViewBag.MenuId = m;
                List<UserObj> rec = null;
                var inst2 = _repo.GetInstitution(0, true, "Active");
                var role = await _repo.GetRolesAsync(0, true, "Active");
                var dept = await _repo.GetDepartmentAsync(0, true, "Active");
                var inst = await _repo.GetInstitutionAsync(0, true, "Active");
                ViewBag.Institution = new SelectList(inst.ToList(), "ITBID", "INSTITUTION_NAME");
                ViewBag.Department = new SelectList(dept.ToList(), "DEPARTMENTCODE", "DEPARTMENTNAME");
                ViewBag.Role = new SelectList(role.ToList(), "ROLEID", "ROLENAME");
                ViewBag.RecordStatus = new SelectList(SmartObj.GetStatus2(), "Code", "Description");
                if (id != null || id != 0)
                {
                    rec = await _repo.GetUserAsync(id.GetValueOrDefault(), false);
                    var model = rec.FirstOrDefault();
                    if (model != null)
                    {
                        ViewBag.HeaderTitle = "Edit User";
                        ViewBag.StatusVisible = true;
                        GetPriv();
                        return PartialView("_Add", model);
                    }
                    else
                    {
                        ViewBag.HeaderTitle = "Add User";
                        ViewBag.StatusVisible = false;
                        GetPriv();
                        return PartialView("_Add",new UserObj());
                    }
                }
                else
                {
                    @ViewBag.HeaderTitle = "Add User";
                    return PartialView("_Add");
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
       // [AllowAnonymous]
        public async Task<ActionResult> UserList()
        {
            try
            {
                var rec = await _repo.GetUserAsync(0, true);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<OutPutObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> GetRole(string id)
        {
            try
            {
                var rec = await _repo.GetRolesAsync(0,true,"Active");  //repoSession.FindAsync(id);              
                if(string.IsNullOrEmpty(id))
                {
                    rec = rec.Where(d => d.ROLEBASE != "1").ToList();
                }
                else
                {
                    rec = rec.Where(d => d.DEPARRTMENT_CODE == id).ToList();
                }
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<StateObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetUserName(string lastName,string firstName)
        {
            try
            {
                string userName = SmartObj.GenUserId(lastName.ToLower().Trim(), firstName.ToLower().Trim());

                if (!string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(firstName))
                {
                    var exist = _repo.GetExistedUserNameCount(userName);
                    if (exist > 0)
                    {
                        if (!string.IsNullOrEmpty(userName))
                        {
                            userName += exist + 1;
                        }
                    }
                }
                return Json(new { UserName = userName, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new {RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        string respMsg;
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(decimal AuthId, int? m)
        {
            //int menuId = 0;
            //string msg = "";
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                int recordId = 0;
                var sucMsg = "";
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                //{
                    var d = new AuthListUtil();
                    //menuId = 5;
                    var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
                    if (dd.authListObj.Count < checkerNo)
                    {

                        var chk = new SM_AUTHCHECKER()
                        {
                            AUTHLIST_ITBID = AuthId,
                            CREATEDATE = DateTime.Now,
                            NARRATION = null,
                            STATUS = approve,
                            USERID = User.Identity.Name,
                        };
                        repoChecker.Insert(chk);
                        var rst = uow.Save(User.Identity.Name);
                        if (rst > 0)
                        {
                            var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
                            noA += 1;
                            if (noA == checkerNo)
                            {
                                recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                switch (rec2.EVENTTYPE)
                                {
                                    case "New":
                                        {
                                            var frec = repoUserTemp.AllEager(f => f.ItbId == recordId).FirstOrDefault();
                                            sucMsg = CreateUserAfterApproval(frec);
                                            if (string.IsNullOrEmpty(sucMsg))
                                            {
                                                frec.Status = approve;
                                            }
                                            break;
                                        }
                                    case "Modify":
                                        {
                                            var frec = repoUserTemp.AllEager(f => f.ItbId == recordId).FirstOrDefault();
                                            sucMsg = ModifyMainRecord(frec);
                                            if (string.IsNullOrEmpty(sucMsg))
                                            {
                                                frec.Status = approve;
                                            }
                                            break;
                                        }
                                    case "LOCK":
                                        {
                                            var frec = repoUserTemp.AllEager(f => f.ItbId == recordId).FirstOrDefault();
                                            sucMsg = ModifyStatus(frec);
                                            if (string.IsNullOrEmpty(sucMsg))
                                            {
                                                frec.Status = approve;
                                            }
                                            break;
                                        }
                                    case "DELETE":
                                        {
                                            var frec = repoUserTemp.Find(recordId);
                                            sucMsg = DeleteMainRecord(frec);
                                            if(string.IsNullOrEmpty(sucMsg))
                                            {
                                                frec.Status = approve;
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                                // rec2.STATUS = close;

                                if (string.IsNullOrEmpty(sucMsg))
                                {
                                    rec2.STATUS = approve;
                                    var t = uow.Save(User.Identity.Name);
                                    if (t > 0)
                                    {
                                        //txscope.Complete();
                                        //EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "Role Record", txtReason.Text, fullName);
                                        //return Json(new { RespCode = 0, RespMessage = "Record Authorized Successfully. A mail has been sent to the user." });
                                        respMsg = "Record Authorized Successfully. A mail has been sent to the user.";
                                        // TempData["msg"] = respMsg;
                                        // return  RedirectToAction("DetailAuth", new {   a_i = SmartObj.Encrypt( rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                        return Json(new { RespCode = 0, RespMessage = respMsg, status = approve });
                                    }
                                }
                                else
                                {
                                    //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                                    respMsg = "Problem processing request. Try again or contact Administrator.";
                                    // TempData["msg"] = respMsg;
                                    // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                                    return Json(new { RespCode = 1, RespMessage = respMsg });
                                }
                            }
                        }
                    }
                    // return Json(new { RespCode = 99, RespMessage = "This request has already been processed by an authorizer." });
                    respMsg = "This request has already been processed by an authorizer.";
                    // TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                //}

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                // TempData["msg"] = respMsg;
                // return RedirectToAction("DetailAuth", new {  a_i = SmartObj.Encrypt(AuthId.ToString()) , m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                return Json(new { RespCode = 1, RespMessage = respMsg });
            }
        }

        private string DeleteMainRecord(SM_ASPNETUSERSTEMP frec)
        {
            throw new NotImplementedException();
        }

        bool sucNew;
        [HttpPost]
        // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(decimal AuthId, int? m, string Narration)
        {
            //int menuId = 0;
            //string msg = "";
            try
            {
                var rec2 = repoAuth.Find(AuthId);
                if (rec2 == null)
                {
                    respMsg = "Problem processing request. Try again or contact Administrator.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(rec2.ITBID.ToString()), m = SmartObj.Encrypt(rec2.MENUID.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });

                }
                else if (rec2.STATUS.ToLower() != "open")
                {
                    respMsg = "This request has already been processed by an authorizer.";
                    //TempData["msg"] = respMsg;
                    //return RedirectToAction("DetailAuth", new { a_i = SmartObj.Encrypt(AuthId.ToString()), m = SmartObj.Encrypt(m.GetValueOrDefault().ToString()) });
                    return Json(new { RespCode = 1, RespMessage = respMsg });
                }
                int recordId = 0;
                // bool suc = false;
                //using (var txscope = new TransactionScope(TransactionScopeOption.RequiresNew))
                //{
                    var d = new AuthListUtil();
                    //menuId = 5;
                    var dd = d.GetCheckerRecord(m.GetValueOrDefault(), AuthId, 0, institutionId);
                    if (dd.authListObj.Count < checkerNo)
                    {

                        var chk = new SM_AUTHCHECKER()
                        {
                            AUTHLIST_ITBID = AuthId,
                            CREATEDATE = DateTime.Now,
                            NARRATION = Narration,
                            STATUS = reject,
                            USERID = User.Identity.Name,
                        };
                        repoChecker.Insert(chk);
                        var rst = uow.Save(User.Identity.Name);
                        if (rst > 0)
                        {
                            var noA = dd.authListObj.Where(f => f.CHECKERSTATUS == approve).Count();
                            noA += 1;
                            if (noA == checkerNo)
                            {
                                recordId = (int)rec2.RECORDID;
                                menuId = rec2.MENUID.GetValueOrDefault();
                                var recc = repoUserTemp.Find(recordId);
                                if (recc != null)
                                {
                                    recc.Status = reject;
                                }

                                rec2.STATUS = reject;
                                var t = uow.Save(User.Identity.Name);
                                if (t > 0)
                                {
                                    sucNew = true;
                                    //txscope.Complete();
                                }

                            }
                        }
                    }
                //}
                if (sucNew)
                {
                    EmailerNotification.SendApprovalRejectionMail(rec2.USERID, rec2.EVENTTYPE, approve, "User Rejection", Narration, fullName);
                    respMsg = "Record Rejected. A mail has been sent to the user.";
                    return Json(new { RespCode = 0, RespMessage = respMsg, status = reject });

                }
                respMsg = "Problem processing request. Try again or contact Administrator.";

                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
            catch (Exception ex)
            {
                //return Json(new { RespCode = 99, RespMessage = "Problem processing request. Try again or contact Administrator." });
                respMsg = "Problem processing request. Try again or contact Administrator.";
                return Json(new { RespCode = 1, RespMessage = respMsg });

            }
        }
        public string getip()
        {
            string IP = "";
            try
            {
                IP =  Request.UserHostAddress;
            }
            catch (Exception) { }
            return IP;
        }
        string CreateUserAfterApproval(SM_ASPNETUSERSTEMP d)
        {
           // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var user1 = new ApplicationUser()
            {
                UserName = d.UserName, // new KeysGenerator().GenerateUserLoginIdNumber(6, 1000),
                Email = d.Email,
                CreateDate = d.CreateDate,
                ForcePassword = d.ForcePassword,
                LockoutEnabled = true,
                FullName = d.FullName,
                LoggedOn = false,
                IsApproved = false,
                LastLoginDate = null,
                MobileNo = d.MobileNo,
                PasswordExpiryDate = null,
                RoleId = d.RoleId,
                LastLogoutDate = null,
                RoleName = d.RoleName,
                Status = active,
                EnforcePasswordChangeDays = 90,
                LastPasswordChangeDate = null,
                DeptName = d.DeptName,
                DeptCode = d.DeptCode,
                CreateUserId = d.UserId,
                InstitutionId = d.InstitutionId,
                InstitutionName= d.InstitutionName,
                LastName = d.LastName,
                FirstName = d.FirstName,
                Supervisor = d.Supervisor
            };
            
            var pass = SmartObj.Decrypt(d.PasswordHash);
            IdentityResult result = UserManager.Create(user1, pass);
            if (result.Succeeded)
            {
                var rec = repoUser.AllEager(t => d.UserName == user1.UserName).FirstOrDefault();
                d.PasswordHash = pass;
                var rsp = SendMailToUser(d).ToString();
                AuditHelper.PostAudit(null, rec, "A", d.UserId, User.Identity.Name, user1.CreateDate.GetValueOrDefault(), rec.Id, "AspNetUsers", institutionId);
                return "";
            }
            
                return result.Errors.FirstOrDefault();
            
        }

        NotificationSystem.EmailResponse SendMailToUser(SM_ASPNETUSERSTEMP obj)
        {
            //var _repo = new Dapper.Data.DapperGeneralSettings();
            //var dg = _repo.GetEmailList(menuid);
            var dg = new List<EmailObj>();
            dg.Add(new EmailObj() { RoleId = 0, Email = obj.Email });
            var mail = NotificationSystem.SendEmail(new EmailMessage()
            {
                EmailAddress = dg,
                // FromAddress = emailaddress,
                // SenderId = rec.SenderEmail,
                emailSubject = "Account Creation.",
                //user = "",
                EmailContent = new EmailerNotification().PopulateUserBody(obj.UserName, "#", obj.PasswordHash, obj.FullName),
                EntryDate = DateTime.Now,
                HasAttachment = false
            });
            return mail;
        }
        NotificationSystem.EmailResponse SendMailToUser(String email, string userName, string Password, string FullName)
        {
            //var _repo = new Dapper.Data.DapperGeneralSettings();
            //var dg = _repo.GetEmailList(menuid);
            var dg = new List<EmailObj>();
            dg.Add(new EmailObj() { RoleId = 0, Email = email });
            var mail = NotificationSystem.SendEmail(new EmailMessage()
            {
                EmailAddress = dg,
                // FromAddress = emailaddress,
                // SenderId = rec.SenderEmail,
                emailSubject = "Password Reset.",
                //user = "",
                EmailContent = new EmailerNotification().PopulateUserBody(userName, "#", Password, FullName),
                EntryDate = DateTime.Now,
                HasAttachment = false
            });
            return mail;
        }
        private string ModifyMainRecord(SM_ASPNETUSERSTEMP obj)
        {
            //var retMsg = "";
            if (obj != null)
            {
                //repoUser.Find(obj.UserName);
                var user = repoUser.AllEager(d=> d.UserName == obj.UserName).FirstOrDefault(); //UserManager.FindByName(obj.UserName);
                var oldrec = new AspNetUser()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MobileNo = user.MobileNo,
                    AccessFailedCount = user.AccessFailedCount,
                    CreateDate = user.CreateDate,
                    CreateUserId = user.CreateUserId,
                    DeptCode = user.DeptCode,
                    DeptName = user.DeptName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    EnforcePasswordChangeDays = user.EnforcePasswordChangeDays,
                    ForcePassword = user.ForcePassword,
                    FullName = user.FullName,
                    Id = user.Id,
                    InstitutionId = user.InstitutionId,
                    InstitutionName = user.InstitutionName,
                    IsApproved = user.IsApproved,
                    ItbId = user.ItbId,
                    LastLoginDate = user.LastLoginDate,
                    LastLogoutDate = user.LastLogoutDate,
                    LastPasswordChangeDate = user.LastPasswordChangeDate,
                    Last_Auth_UID = user.Last_Auth_UID,
                    Last_Modified_UID = user.Last_Modified_UID,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEndDateUtc = user.LockoutEndDateUtc,
                    LoggedOn = user.LoggedOn,
                    PasswordExpiryDate = user.PasswordExpiryDate,
                    PasswordHash = user.PasswordHash,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    RoleId = user.RoleId,
                    RoleName = user.RoleName,
                    SecurityStamp = user.SecurityStamp,
                    Status = user.Status,
                    Supervisor = user.Supervisor,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    UserName = user.UserName,
                    
                };
               
                if (user != null)
                {
                    user.FullName = obj.LastName + " " + obj.FirstName;
                    user.MobileNo = obj.MobileNo;
                    user.RoleId = obj.RoleId;
                    user.RoleName = obj.RoleName;
                    user.DeptName = obj.DeptName;
                    user.DeptCode = obj.DeptCode;
                    user.Supervisor = obj.Supervisor;
                    user.InstitutionId = obj.InstitutionId;
                    user.LastName = obj.LastName;
                    user.FirstName = obj.FirstName;
                    user.Status = active;
                    user.Email = obj.Email;
                    var rst = uow.Save(User.Identity.Name);
                    if (rst > 0)
                    {
                        AuditHelper.PostAudit(oldrec, user, "M", obj.UserId, User.Identity.Name, obj.CreateDate.GetValueOrDefault(), oldrec.Id, "AspNetUsers", institutionId);
                    }
                }
               
                //////if(rst == 0)
                //////{
                //////    return "Problem Processing Request";
                //////}
                //IdentityResult result = UserManager.Update(user);
                //if(!result.Succeeded)
                //{
                //    return result.Errors.FirstOrDefault();
                //}

            }

            return "";
        }
        //async void BindCombo()
        //{
        //}
        private string ModifyStatus(SM_ASPNETUSERSTEMP obj)
        {
            //var retMsg = "";
            if (obj != null)
            {
                var user = UserManager.FindByName(obj.UserName);
                if (user != null)
                {
                    user.Status = "LOCK";
                }

                IdentityResult result = UserManager.Update(user);
                if (!result.Succeeded)
                {
                    return result.Errors.FirstOrDefault();
                }

            }

            return "";
        }

        public async Task<JsonResult> ViewUser(int id = 0)
        {
            if (id == 0)
            {

                return Json(new { RespCode = 99, RespMessage = "Bad Request" }, JsonRequestBehavior.AllowGet);
            }
            //var d = _repo.GetSession(0, true);
            try
            {
                var rec = await _repo.GetUserAsync(id, false);  //repoSession.FindAsync(id);
                if (rec == null)
                {
                    // return Json(null, JsonRequestBehavior.AllowGet);
                    var obj = new { RespCode = 1, RespMessage = "Record Not Found" };
                    return Json(obj, JsonRequestBehavior.AllowGet);

                }
                //  return Json(rec, JsonRequestBehavior.AllowGet);
                var obj1 = new { model = rec.FirstOrDefault(), RespCode = 0, RespMessage = "Success" };
                return Json(obj1, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var obj1 = new { RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.LoginId);
                    if (user == null)
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        //return View("ForgotPasswordConfirmation");
                        return Json(new { RespCode = 1, RespMessage = "Invalid." });

                    }
                    //var oldPass = txtOldPass.Text;
                    var passLength = 8;
                    var bt = _repo.GetCompanyProfile(0,false);
                    if(bt != null)
                    {
                        passLength = bt.PASSWORDLENGTH.GetValueOrDefault();
                    }
                    // bt.PASSWORDLENGTH;
                    if (user != null)
                    {
                        var newPass = SmartObj.GenerateRandomPassword(passLength);
                        var rem = await UserManager.RemovePasswordAsync(user.Id);
                        IdentityResult rem2 = null;
                        if (rem.Succeeded)
                        {
                            rem2 = await UserManager.AddPasswordAsync(user.Id, newPass);
                            if (rem2.Succeeded)
                            {
                                // user.LastPasswordChangeDate = DateTime.Now;
                                user.ForcePassword = true;
                                // user.IsFirstLogin = false;
                                var res = await UserManager.UpdateAsync(user);
                               // pnlResponse.Visible = true;
                                //pnlResponseMsg.Text = @"Your Password has been reset. Check your mail for a new password.";
                                //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
                                SendMailToUser(user.Email, user.UserName, newPass, user.FullName);
                                return Json(new { RespCode = 0, RespMessage = "Your Password has been reset. Check your mail for a new password." });

                            }
                            else
                            {
                                // rem = manager.AddPassword(user.Id, txtOldPass.Text.TrimEnd());
                                //pnlResponse.Visible = true;
                                //pnlResponseMsg.Text = rem2.Errors.FirstOrDefault();
                                //pnlResponse.CssClass = "alert alert-warning alert-dismissable alert-bold";
                                return Json(new { RespCode = 2, RespMessage = rem2.Errors.FirstOrDefault() });

                            }

                        }
                        else
                        {
                            //pnlResponse.Visible = true;
                            //pnlResponseMsg.Text = "UserName does not exist";
                            //pnlResponse.CssClass = "alert alert-warning alert-dismissable alert-bold";
                            //return;
                            return Json(new { RespCode = 4, RespMessage = "Login Id does not exist." });

                        }
                    }
                }
                return Json(new { RespCode = 6, RespMessage = "Login Id is required." });

            }
            catch (Exception ex)
            {
                return Json(new { RespCode = 5, RespMessage = "Problem processing request." });

            }

            //if (ModelState.IsValid)
            //{
            //    var user = await UserManager.FindByNameAsync(model.LoginId);
            //    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            //    {
            //        // Don't reveal that the user does not exist or is not confirmed
            //        return View("ForgotPasswordConfirmation");
            //    }

            //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            //    // Send an email with this link
            //    // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            //    // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
            //    // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            //    // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            // }

            // If we got this far, something failed, redisplay form
            // return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {
            //return code == null ? View("Error") : View();
          
            return View(new ResetPasswordViewModel() { LoginId = id});
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.LoginId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                //return RedirectToAction("ResetPasswordConfirmation", "Account");
                return Json(new { RespCode = 1, RespMessage = "Invalid." });

            }
            var exist = await UserManager.CheckPasswordAsync(user, model.OldPassword);
            if (!exist)
            {
                //ModelState.AddModelError("", "Password is Incorrect");
                //return View(model);
                return Json(new { RespCode = 1, RespMessage = "Old Password is Incorrect" });
            }
            
            if(model.OldPassword == model.Password)
            {
                return Json(new { RespCode = 1, RespMessage = "You cannot use your Old password." });

            }
            var rem = await UserManager.RemovePasswordAsync(user.Id);
            IdentityResult rem2 = null;
            if (rem.Succeeded)
            {
                rem2 = await UserManager.AddPasswordAsync(user.Id, model.Password);
                if (rem2.Succeeded)
                {
                    user.LastPasswordChangeDate = DateTime.Now;
                    user.ForcePassword = false;
                    var res = await UserManager.UpdateAsync(user);
                    //pnlResponse.Visible = true;
                    //pnlResponseMsg.Text = @"Password Changed Successfully...Click <a id='bt' Runat=""Server"" href='Login.aspx'>here to Login</a>";
                    //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
                    //return RedirectToAction("ResetPasswordConfirmation", "Account");
                    return Json(new { RespCode = 0, RespMessage ="" });

                }
                else
                {
                    rem = await UserManager.AddPasswordAsync(user.Id, model.OldPassword);
                    // pnlResponse.Visible = true;
                    //pnlResponseMsg.Text = rem2.Errors.FirstOrDefault();
                    // pnlResponse.CssClass = "alert alert-warning alert-dismissable alert-bold";
                    //AddErrors(rem2);
                    return Json(new { RespCode = 2, RespMessage = rem2.Errors.FirstOrDefault() });

                   // return View();
                }
                //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                //if (result.Succeeded)
                //{
                //    return RedirectToAction("ResetPasswordConfirmation", "Account");
                //}
               
            }
            else
            {
                //AddErrors(rem);
                //return View();
                return Json(new { RespCode = 3, RespMessage = rem2.Errors.FirstOrDefault() });

            }
        }

        //
        // GET: /Account/


        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                var guidNo = Session["guidno"];
                if (guidNo != null)
                {
                    var obj = new LoginAuditObj()
                    {
                        LOGOUTDATE = DateTime.Now,
                        guid = guidNo != null ? guidNo.ToString() : "",
                    };

                    _repo.PostSignOut(obj);
                }
            }
            catch(Exception ex)
            {

            }
            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Json(new { RespCode = 0, RespMessage = "" });
        }


        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}