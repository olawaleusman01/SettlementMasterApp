using Dapper;
using Generic.Dapper.Model;
using Generic.Dapper.Repository;
using Generic.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public class DapperReportSettings : RepoBase, IDapperReportSettings
    {
        public async Task<List<Rpt_Audit>> GetRptAuditAsync(int inst_id,DateTime? from = null, DateTime? to = null,int ? pagenumber = null, int? pagesize = null,string userid=null,bool IsAll = false)
        {
            var p = new DynamicParameters();
            p.Add("@P_FROMDATE", from, DbType.Date, null);
            p.Add("@P_TODATE", to, DbType.Date, null);
            p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);
            p.Add("@P_INSTID", inst_id, DbType.Int32, null);
            p.Add("@P_USERID", userid, DbType.String, null);
            p.Add("@P_ISALL", IsAll, DbType.Boolean, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<Rpt_Audit>("RPT_AUDITTRAIL", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<int> GetRptAuditTotalCountAsync(int inst_id, DateTime? from = null, DateTime? to = null,string userid = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_FROMDATE", from, DbType.DateTime, null);
            p.Add("@P_TODATE", to, DbType.DateTime, null);
            p.Add("@P_INSTID", inst_id, DbType.Int32, null);
            p.Add("@P_USERID", userid, DbType.String, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<int>("RPT_AUDITTRAIL_COUNT", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        public async Task<List<RPT_LoginAudit>> GetLoginAuditAsync(DateTime? from = null, DateTime? to = null, int? pagenumber = null, int? pagesize = null,string userId = null,bool IsAll = false)
        {
            var p = new DynamicParameters();
            p.Add("@P_FROMDATE", from, DbType.Date, null);
            p.Add("@P_TODATE", to, DbType.Date, null);
            p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);
            p.Add("@P_USERID", userId, DbType.String, null);
            p.Add("@P_ISALL", IsAll, DbType.Boolean, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RPT_LoginAudit>("RPT_LOGINAUDIT", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<int> GetLoginAuditTotalCountAsync(DateTime? from = null, DateTime? to = null, string userid = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_FROMDATE", from, DbType.DateTime, null);
            p.Add("@P_TODATE", to, DbType.DateTime, null);
            p.Add("@P_USERID", userid, DbType.String, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<int>("RPT_LOGINAUDIT_COUNT", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        public async Task<List<RPT_LoginAudit>> GetLoginAttemptAsync(DateTime? from = null, DateTime? to = null, int? pagenumber = null, int? pagesize = null,string userid = null,bool IsAll = false)
        {
            var p = new DynamicParameters();
            p.Add("@P_FROMDATE", from, DbType.Date, null);
            p.Add("@P_TODATE", to, DbType.Date, null);
            p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);
            p.Add("@P_USERID", userid, DbType.String, null);
            p.Add("@P_ISALL", IsAll, DbType.Boolean, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RPT_LoginAudit>("RPT_LOGINATTEMPT", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<Rpt_Exemption>> GetExemptionReportAsync(DateTime? reportdate = null, int? pagenumber = null, int? pagesize = null, bool IsAll = false)
        {
            var p = new DynamicParameters();
            p.Add("@DATE", reportdate, DbType.DateTime, null);
            p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);
            p.Add("@P_ISALL", IsAll, DbType.Boolean, null);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<Rpt_Exemption>("RPT_SETTLEMENTEXEMPTION", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<int> GetLoginAttemptTotalCountAsync(DateTime? from = null, DateTime? to = null, string userid = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_FROMDATE", from, DbType.DateTime, null);
            p.Add("@P_TODATE", to, DbType.DateTime, null);
            p.Add("@P_USERID", userid, DbType.String, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<int>("RPT_LOGINATTEMPT_COUNT", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }

       
        public async Task<List<Rpt_LoginUser>> GetLoginUserAsync(int? pagenumber = null, int? pagesize = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);
            //p.Add("@P_USERID", userid, DbType.String, null);
            //p.Add("@P_ISALL", IsAll, DbType.Boolean, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<Rpt_LoginUser>("RPT_LOGGEDONUSER", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<Rpt_ApprovalDetail>> GetApprovalDetailAsync(string batchno = null, int? pagenumber = null, int? pagesize = null, bool IsAll = false)
        {
            var p = new DynamicParameters();
            p.Add("@BATCHNO", batchno, DbType.String, null);
            //p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            //p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);
            //p.Add("@P_ISALL", IsAll, DbType.Boolean, null);
     

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<Rpt_ApprovalDetail>("GET_NAPSAUTHCHECKER", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SM_SETTLEMENTDETAIL> GetSettlementEnquiryObj(SettlementEnquiryObj obj)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("P_FROMDATE", obj.fromDate, DbType.DateTime);
            p.Add("P_TODATE", obj.toDate, DbType.DateTime);
            p.Add("@P_SETTDATE", obj.settDate, DbType.DateTime);
            p.Add("PAYREF", obj.payRef, DbType.String);
            p.Add("REFNO", obj.tranRef, DbType.String);
            p.Add("TRANSID", obj.tranID, DbType.Decimal);
            p.Add("MERCHANTDEPOSITBANK", obj.crBank, DbType.String);
            p.Add("DEBITBANK", obj.drBank, DbType.String);
            p.Add("MERCHANTID", obj.merchantID, DbType.String);
            p.Add("MERCHANTNAME", obj.merchantName, DbType.String);
            p.Add("CARDSCHEME", obj.cardScheme, DbType.String);
            p.Add("TRANAMOUNT", obj.tranAmt, DbType.Decimal);
            p.Add("ACQUIRERFIID", obj.acquirer, DbType.String);
            p.Add("ISSUERFIID", obj.issuer, DbType.String);
            p.Add("SETTLEMENTACCOUNT", obj.settAcct, DbType.String);
            p.Add("CHANNELID", obj.channel, DbType.Int32);
            p.Add("MASKPAN", obj.maskpan, DbType.String);
            p.Add("TERMINALID", obj.termID, DbType.String);
            p.Add("LOCATION", obj.mlocation, DbType.String);
            p.Add("INVOICENO", obj.invoiceNo, DbType.String);

            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SM_SETTLEMENTDETAIL>("PROC_SETTLEMENTENQUIRY", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }
        }



        public async Task<List<SM_SETTLEMENTDETAIL>> GetSettlementEnquiryAsync(SettlementEnquiryObj obj = null)
        {
            var p = new DynamicParameters();
            p.Add("P_FROMDATE", obj.fromDate, DbType.DateTime, null);
            p.Add("P_TODATE", obj.toDate, DbType.DateTime, null);
            p.Add("@P_SETTDATE", obj.settDate, DbType.DateTime, null);
            p.Add("PAYREF", string.IsNullOrEmpty(obj.payRef) ? null : obj.payRef  , DbType.String, null);
            p.Add("REFNO", string.IsNullOrEmpty(obj.tranRef) ? null : obj.tranRef , DbType.String, null);
            p.Add("TRANSID", obj.tranID ?? null, DbType.Decimal, null);
            p.Add("MERCHANTDEPOSITBANK", string.IsNullOrEmpty(obj.crBank) ? null : obj.crBank, DbType.String, null);
            p.Add("DEBITBANK", string.IsNullOrEmpty(obj.drBank) ? null : obj.drBank, DbType.String, null);
            p.Add("MERCHANTID", string.IsNullOrEmpty(obj.merchantID) ? null : obj.merchantID, DbType.String, null);
            p.Add("MERCHANTNAME", string.IsNullOrEmpty(obj.merchantName) ? null : obj.merchantName, DbType.String, null);
            p.Add("CARDSCHEME", string.IsNullOrEmpty(obj.cardScheme) ? null : obj.cardScheme, DbType.String, null);
            p.Add("TRANAMOUNT", obj.tranAmt ?? null, DbType.Decimal, null);
            p.Add("ACQUIRERFIID", string.IsNullOrEmpty(obj.acquirer) ? null : obj.acquirer, DbType.String, null);
            p.Add("ISSUERFIID", string.IsNullOrEmpty(obj.issuer) ? null : obj.issuer, DbType.String, null);
            p.Add("SETTLEMENTACCOUNT", string.IsNullOrEmpty(obj.settAcct) ? null : obj.settAcct, DbType.String, null);
            p.Add("CHANNELID", string.IsNullOrEmpty(obj.channel) ? null : obj.channel, DbType.Int32, null);
            p.Add("MASKPAN", string.IsNullOrEmpty(obj.maskpan) ? null : obj.maskpan, DbType.String, null);
            p.Add("TERMINALID", string.IsNullOrEmpty(obj.termID) ? null : obj.termID, DbType.String, null);
            p.Add("LOCATION", string.IsNullOrEmpty(obj.mlocation) ? null : obj.mlocation, DbType.String, null);
            p.Add("INVOICENO", string.IsNullOrEmpty(obj.invoiceNo) ? null : obj.invoiceNo, DbType.String, null);
            //p.Add("@P_PAGENUMBER", pagenumber, DbType.Int32, null);
            //p.Add("@P_PAGESIZE", pagesize, DbType.Int32, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SM_SETTLEMENTDETAIL>("PROC_SETTLEMENTENQUIRY", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
    }
}
