using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Generic.Dapper.Repository;
using Generic.Data.Utilities;
using Generic.Dapper.Model;
using Generic.Data;
using System.Transactions;
using Generic.Dapper.Utility;

namespace Generic.Dapper.Data
{
    public class DapperSessionSettings : RepoBase, IDapperSessionSettings
    {
        public async Task<List<MccMscObj>> GETMCCMSCAsync(int itbid, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_USERID", userId, DbType.String);
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            string sql = "";
            if (itbid == 0)
            {
                sql = "select * from SM_SESS_MCCMSC where userid = @P_USERID";
            }
            else
            {
                sql = "select * from SM_SESS_MCCMSC where userid = @P_USERID and ITBID = @P_ITBID";
            }
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MccMscObj>(sql, p, commandType: CommandType.Text);
                return gh;
            });
            return rec.ToList();
        }
        public int EMPTYMCCMSC(string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_USERID", userId, DbType.String);
            string sql = "DELETE FROM SM_SESS_MCCMSC where userid = @P_USERID";
            var cnt = Execute(c => c.Execute(sql, p, commandType: CommandType.Text), "");
            return cnt;
        }

        public async Task<List<OutPutObj>> PostMCCMSC(MccMscObj obj, bool buffered = false, string connectionString = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@CARDSCHEME", obj.CARDSCHEME, DbType.String);
            p.Add("@BATCHID", obj.BATCHID, DbType.String);
            p.Add("@CBN_CODE", obj.CBN_CODE, DbType.String);
            p.Add("@CHANNEL", obj.CHANNEL, DbType.Int32);
            p.Add("@CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("@P_ISALL", obj.DOMCAP, DbType.Decimal);
            p.Add("@P_ISTEMP", obj.DOM_FREQUENCY, DbType.Int32);
            p.Add("@P_STATUS", obj.DOM_MSCVALUE, DbType.Decimal);
            p.Add("@P_ITBID", obj.DOM_SETTLEMENT_CURRENCY, DbType.String);
            p.Add("@P_ISALL", obj.INTLCAP, DbType.Decimal);
            p.Add("@P_ISTEMP", obj.INTL_ENABLED, DbType.Boolean);
            p.Add("@P_STATUS", obj.INTMSC_CALCBASIS, DbType.String);
            p.Add("@P_ITBID", obj.INT_FREQUENCY, DbType.Int32);
            p.Add("@P_ISALL", obj.INT_MSCVALUE , DbType.Decimal);
            p.Add("@P_ISTEMP", obj.INT_SETTLEMENT_CURRENCY , DbType.String);
            p.Add("@P_STATUS", obj.MCC_CODE, DbType.String);
            p.Add("@P_ITBID", obj.MSC_CALCBASIS, DbType.Int32);
            p.Add("@P_ISALL", obj.RECORDID , DbType.Decimal);
            p.Add("@P_ISTEMP", obj.USERID, DbType.String);
            p.Add("@P_STATUS", obj.EVENTTYPE, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<OutPutObj>("POST_SESSION_MCCMSC", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
    }
}
