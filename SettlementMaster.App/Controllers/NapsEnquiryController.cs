using Generic.Dapper.Data;
using Generic.Dapper.Model;
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
    public class NapsEnquiryController : Controller
    {
        IDapperGeneralSettings _repo = new DapperGeneralSettings();

        [MyAuthorize]
        // GET: NapsEnquiry
        public ActionResult Index()
        {
            return View();
        }

    
  
        public async Task<ActionResult> GetNapsEnquiry(int draw, int start, int length, string fromdate, string todate, string batchid)
        {
            //string search = Request.QueryString["search[value]"];
            //int sortColumn = -1;
            //string sortDirection = "asc";
            //if (length == -1)
            //{
            //    length = TOTAL_ROWS;
            //}

            //// note: we only sort one column at a time
            //if (Request.QueryString["order[0][column]"] != null)
            //{
            //    sortColumn = int.Parse(Request.QueryString["order[0][column]"]);
            //}
            //if (Request.QueryString["order[0][dir]"] != null)
            //{
            //    sortDirection = Request.QueryString["order[0][dir]"];
            //}
            DataTableData dataTableData = new DataTableData();

            try
            {
                start++;
                DateTime start_date;
                DateTime end_date;
                var stSuc = DateTime.TryParse(GetDate(fromdate), out start_date);
                var adSuc = DateTime.TryParse(GetDate(todate), out end_date);
                if (!stSuc || !adSuc)
                {
                    dataTableData.draw = draw;

                    dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                    // int recordsFiltered = 0;
                    dataTableData.data = new List<NapsObj>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = 0;
                }
                else
                {
                    dataTableData.draw = draw;
                    batchid = string.IsNullOrEmpty(batchid.Trim()) ? null : batchid.Trim(); 
                    var recP = await _repo.GetNapsEnquiryCountAsync(start_date, end_date, batchid);
                    var cnt = recP;
                    var data = await _repo.GetNapsEnquiryAsync(start_date, end_date, batchid); 
                    dataTableData.recordsTotal = cnt; //  TOTAL_ROWS;
                                                      // int recordsFiltered = 0;
                    dataTableData.data = data;  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                    dataTableData.recordsFiltered = data.Count; // recordsFiltered;
                }
            }
            catch (Exception ex)
            {
                dataTableData.draw = draw;

                dataTableData.recordsTotal = 0; //  TOTAL_ROWS;
                                                // int recordsFiltered = 0;
                dataTableData.data = new List<Rpt_Audit>();  //FilterData(ref recordsFiltered, start, length, search, sortColumn, sortDirection);
                dataTableData.recordsFiltered = 0;
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetNapsBatch(string id)
        {
            var rec = await _repo.GetNapsApprovedAsync(id);
               var dt = rec.Count(d => d.DisplayForReprocess == true) > 0;
                    ViewBag.DisplayButton = dt;
            var html = PartialView("_ViewNaps", rec).RenderToString();
            return Json(new {RespCode = 0,RespMessage = "",data_html = html }, JsonRequestBehavior.AllowGet);
        }

        string GetDate(string date)
        {
            try
            {
                if (string.IsNullOrEmpty(date))
                {
                    return "";
                }
                else
                {
                    if (date.Length == 10)
                    {
                        var day = date.Substring(0, 2);
                        var month = date.Substring(3, 2);
                        var year = date.Substring(6, 4);
                        return string.Concat(year, "-", month, "-", day);
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        [HttpPost]
        public async Task<ActionResult> PostReprocess(decimal[] selected_record,string batchid)
        {
            

            try
            {
                var rst = Naps.PostNapsReprocess(selected_record, User.Identity.Name);
                if(rst > 0)
                {
                    var rec = await _repo.GetNapsApprovedAsync(batchid);
                 
                    var html = PartialView("_ViewNaps", rec).RenderToString();
                    return Json(new { RespCode = 0, RespMessage = "Reprocess Initiation Completed Successfully", data_html = html }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { RespCode = 1, RespMessage = "Unable to Process Request. Try again or contact Administrator." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { RespCode = 1, RespMessage = "Problem Processing Request." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}