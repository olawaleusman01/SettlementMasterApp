using SettlementMaster.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class HomeController : Controller
    {
        //[MyAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Dialog()
        {
            return View();
        }
    }
}