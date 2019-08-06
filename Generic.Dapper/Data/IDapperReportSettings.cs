
using Generic.Dapper.Model;
using Generic.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public interface IDapperReportSettings 
    {
        Task<List<Rpt_Audit>> GetRptAuditAsync(int inst_id, DateTime? from = null, DateTime? to = null, int? pagenumber = null, int? pagesize = null,string userid = null,bool isAll = false);
        Task<int> GetRptAuditTotalCountAsync(int inst_id, DateTime? from = null, DateTime? to = null, string userid = null);
        Task<List<RPT_LoginAudit>> GetLoginAuditAsync(DateTime? from = null, DateTime? to = null, int? pagenumber = null, int? pagesize = null, string userId = null, bool IsAll = false);
        Task<int> GetLoginAuditTotalCountAsync(DateTime? from = null, DateTime? to = null, string userid = null);
        Task<List<RPT_LoginAudit>> GetLoginAttemptAsync(DateTime? from = null, DateTime? to = null, int? pagenumber = null, int? pagesize = null, string userid = null, bool IsAll = false);
        Task<List<Rpt_Exemption>> GetExemptionReportAsync(DateTime? reportdate = null, int? pagenumber = null, int? pagesize = null, bool IsAll = false);
        Task<int> GetLoginAttemptTotalCountAsync(DateTime? from = null, DateTime? to = null, string userid = null);
       
        Task<List<Rpt_LoginUser>> GetLoginUserAsync(int? pagenumber = null, int? pagesize = null);
        Task<List<Rpt_ApprovalDetail>> GetApprovalDetailAsync(string batchno, int? pagenumber = null, int? pagesize = null, bool IsAll = false);
        List<SM_SETTLEMENTDETAIL> GetSettlementEnquiryObj(SettlementEnquiryObj obj);

        Task<List<SM_SETTLEMENTDETAIL>> GetSettlementEnquiryAsync(SettlementEnquiryObj obj = null);
    }
}
