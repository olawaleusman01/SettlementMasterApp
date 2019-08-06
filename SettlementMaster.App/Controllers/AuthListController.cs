using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Data.Utilities;
using SettlementMaster.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class AuthListController : Controller
    {
        string deptCode;
        int institutionId;
        int roleId;
        public AuthListController()
        {
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleId = user.UserRole;
                institutionId = user.InstitutionId;
                deptCode = user.DeptCode;
            }
        }
        IDapperGeneralSettings _repo = new DapperGeneralSettings();
        // GET: AuthList
        //[MyAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        int menuId;
        decimal authId;
        public async Task<ActionResult> Detail(string a_i,string m)
        {
            var mid = SmartObj.Decrypt(m);
            var ai = SmartObj.Decrypt(a_i);
            if (int.TryParse(mid, out menuId) && decimal.TryParse(ai, out authId))
            {
               var controller = await _repo.GetMenuById(menuId);
                ViewBag.Controller = controller;
                ViewBag.Key = authId;
               // ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            else
            {
                return View("Error", "Home");
            }
        }
    
        public async Task<ActionResult> AuthQueue()
        {
            try
            {
                //List<AuthListObj2> pre = new List<AuthListObj2>();
                var rec = await _repo.GetAuthList(deptCode,roleId,institutionId,User.Identity.Name);  //repoSession.FindAsync(id);              
                //for(int i = 0; i <= 50; i++)
                //{
                //    pre.AddRange(rec);
                //}
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<AuthListObj2>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
    }
}