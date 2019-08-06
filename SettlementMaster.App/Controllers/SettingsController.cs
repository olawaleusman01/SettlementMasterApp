using Generic.Dapper.Data;
using Generic.Dapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SettlementMaster.App.Controllers
{
    public class SettingsController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();

        public async Task<ActionResult> StateList(string countryCode)
        {
            try
            {
                var rec = await _repo.GetStateFilterAsync(countryCode);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<StateObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> CityList(string countryCode,string stateCode)
        {
            try
            {
                var rec = await _repo.GetCityFilterAsync(countryCode,stateCode);  //repoSession.FindAsync(id);              
                return Json(new { data = rec, RespCode = 0, RespMessage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var obj1 = new { data = new List<CityObj>(), RespCode = 2, RespMessage = ex.Message };
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }
    }
}