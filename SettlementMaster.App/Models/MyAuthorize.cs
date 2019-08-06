using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
//using System.Web.Mvc;
using Microsoft.Owin.Security;
using Generic.Data.Model;

namespace SettlementMaster.App.Models
{
   
   // [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorizeAttribute : ActionFilterAttribute
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        ILoginMultiple _repoL = new LoginMultiple();

        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    if (filterContext.HttpContext.Request.IsAjaxRequest())
        //    {
        //        var urlHelper = new UrlHelper(filterContext.RequestContext);
        //        filterContext.HttpContext.Response.StatusCode = 401;
        //        filterContext.Result = new JsonResult
        //        {
        //            Data = new
        //            {
        //                Error = "NotAuthorized",
        //                LogOnUrl = urlHelper.Action("Login", "Account")
        //            },
        //            JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //        };
        //        filterContext.HttpContext.Response.End();
        //    }
        //    else
        //    {
        //        // this is a standard request, let parent filter to handle it
        //        base.HandleUnauthorizedRequest(filterContext);

        //    }
        //}
        //public int RoleId { get; set; }
        //public int MenuId { get; set; }
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    // first look at routedata then at request parameter:
        //    //var id = (httpContext.Request.RequestContext.RouteData.Values["m"] as string)
        //    //         ??
        //    //         (httpContext.Request["m"] as string);
        //    //var id  = httpContext.Request..ActionParameters.SingleOrDefault(p => p.Key == "role").Value.ToString();
        //    if (RoleId == 0)
        //    {
        //        return base.AuthorizeCore(httpContext);
        //    }
        //    if(RoleId != 1)
        //    {
        //        return false;
        //    }
        //    return false;
        //}
        int roleId;
        ActionExecutingContext fc;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleId = user.UserRole;
            }
            string actionName = (string)filterContext.RouteData.Values["action"];
            string controllerName = (string)filterContext.RouteData.Values["controller"];
            //var roleId = filterContext.Controller.ViewBag.RoleId;
            //string roleId = filterContext.Controller.ViewBag["RoleId"].ToString();
            //var menuId = filterContext.Controller.ViewBag.MenuId;
            var count = _repo.GetUserAuthorization(roleId, 0, controllerName);
            //if (filterContext.HttpContext.Request.IsLocal)
            if (count <= 0)
            {
                //filterContext.Result = new HttpNotFoundResult();
                //redirect to no authorize page
                //filterContext.HttpContext.Response.Redirect("~/NoAccess", true);
                //var url =UrlHelper.GenerateUrl(null, "", "");
                filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
                   {{"controller", "Home"}, {"action", "Index"}});
                return;

            }
            var sessId = filterContext.HttpContext.Session["sessionid"];
            if (sessId == null)
                sessId = "";

            // check to see if your ID in the Logins table has 
            // LoggedIn = true - if so, continue, otherwise, redirect to Login page.
            if (_repoL.IsYourLoginStillTrue
            (HttpContext.Current.User.Identity.Name, sessId.ToString()))
            {
                // check to see if your user ID is being used elsewhere under a different session ID
                if (_repoL.IsUserLoggedOnElsewhere
                (HttpContext.Current.User.Identity.Name, sessId.ToString()))
                {
                    // if it is being used elsewhere, update all their 
                    // Logins records to LoggedIn = false, except for your session ID
                    _repoL.LogEveryoneElseOut
                    (HttpContext.Current.User.Identity.Name, sessId.ToString());

                }
            }
            else
            {
                //FormsAuthentication.SignOut();
                fc = filterContext;
                try
                {
                    var guidNo = filterContext.HttpContext.Session["guidno"];
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
                catch (Exception ex)
                {

                }
                filterContext.HttpContext.Session.Abandon();
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
                   {{"controller", "Account"}, {"action", "Login"}});
                return;
            }

        }

        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            throw new NotImplementedException();
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return fc.HttpContext.GetOwinContext().Authentication;
            }
        }

    }
}
