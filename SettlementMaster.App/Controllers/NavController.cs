
using Generic.Dapper.Data;
using Generic.Dapper.Model;
using SettlementMaster.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    [Authorize]
    public class NavController : Controller
    {
        //private readonly IUnitOfWork uow = null;
        //private readonly IRepository<SchoolApp.Domain.menucontrol> repoMenu = null;
        //private readonly IRepository<SchoolApp.Domain.roleAssignment> repoRoleAssig = null;
        //private readonly IRepository<SchoolApp.Domain.state> repoState = null;
        private readonly IDapperGeneralSettings _repoMenu = null;
        //private int menuid;
        int roleid;
      
        public NavController()
        {
            _repoMenu = new DapperGeneralSettings();
        }
        //
        // GET: /Nav/

        public PartialViewResult Menu()
        {
            MenuViewModel mnv = new MenuViewModel();
            var user = new UserDataSettings().GetUserData();
            if (user != null)
            {
                roleid = user.UserRole; // short.Parse(new ProfileHelper().GetProfile(User.Identity.Name, "roleid").ToString());
                var parMenu = _repoMenu.GetParentMenu(roleid);
                //  ViewBag.ParentMenu = parMenu;
                var childMenu = _repoMenu.GetChildMenu(roleid);
              
                List<ChildMenu> mn = new List<ChildMenu>();
                foreach (var d in parMenu)
                {
                    var t = childMenu.Where(e => e.ParentId == d.ParentId).ToList();
                    if (t.Count > 0)
                    {
                        mn.AddRange(t);
                    }

                    // ViewBag.ChildMenu = mn;

                }
                mnv.ParNode = parMenu;
                mnv.ChildNode = mn;
            }
            //parentRepeater.DataSource = rst;
            //parentRepeater.DataBind();

            return PartialView(mnv);
        }

        public PartialViewResult UserDetail()
        {

            var user = new UserDataSettings().GetUserData();
            if (user == null)
            {
                user = new UserDataObj();
            }
            //parentRepeater.DataSource = rst;
            //parentRepeater.DataBind();

            return PartialView(user);
        }

        //public PartialViewResult ApprovalRoute(int m, decimal? AuthId, short action, string cola = "col-md-2", string colb = "col-md-4")
        //{
        //    ViewBag.ColA = cola;
        //    ViewBag.ColB = colb;
        //    var d = _repoMenu.GetApprovalRoutePage(m, User.Identity.Name, action, AuthId);
        //    ViewBag.ApproverList = new SelectList(d, "StaffId", "StaffName");
        //    return PartialView("ApprovalRoutePartial");
        //}

        public async Task<PartialViewResult> ApprovalRouteLine(decimal id)
        {

            var model = await _repoMenu.GetApprovalListForRequestAsync(id);
            //ViewBag.ApproverList = new SelectList(d, "StaffId", "StaffName");
            return PartialView("_ApprovalLinePartial", model);
        }
    }
}