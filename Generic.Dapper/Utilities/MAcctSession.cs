using Dapper;
using Generic.Dapper.Model;
using Generic.Dapper.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Generic.Dapper.Utilities
{
    public class MAcctSession
    {
        public OutPutObj PostMAcct(MerchantAcctObj obj, int postType)
        {
            OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("PID", obj.PID, DbType.String);
            p.Add("DEPOSIT_ACCOUNTNO", obj.DEPOSIT_ACCOUNTNO, DbType.String);
            p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);
            p.Add("DEPOSIT_ACCTNAME", obj.DEPOSIT_ACCTNAME, DbType.String);
            p.Add("DEPOSIT_BANKADDRESS", obj.DEPOSIT_BANKADDRESS, DbType.String);
            p.Add("DEPOSIT_BANKCODE", obj.DEPOSIT_BANKCODE, DbType.String);
            p.Add("DEPOSIT_BANKNAME", obj.DEPOSIT_BANKNAME, DbType.String);
            p.Add("DEPOSIT_COUNTRYCODE", obj.DEPOSIT_COUNTRYCODE, DbType.String);
            p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("SETTLEMENTCURRENCY", obj.SETTLEMENTCURRENCY, DbType.String);
            p.Add("DB_ITBID", obj.DB_ITBID, DbType.Decimal);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);
            p.Add("DEFAULT_ACCOUNT", obj.DEFAULT_ACCOUNT, DbType.Boolean);

            using (var con = new RepoBase().OpenConnection(null))
            {
                ret = con.Query<OutPutObj>("SESS_POST_MERCHANTACCT", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }
        }
        //public OutPutObj PostRevenueHeadBulk(List<RvHeadObj> objList)
        //{
        //    OutPutObj ret = new OutPutObj();
        //    var p = new DynamicParameters();
        //    using (var con = new RepoBase().OpenConnection(null))
        //    {
        //        foreach (var obj in objList)
        //        {
        //            ret = con.Query<OutPutObj>("POST_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure).FirstOrDefault();

        //        }
        //        return ret;
        //    }

        //}
        public List<MerchantAcctObj> GetMerchantAcct(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<MerchantAcctObj>("SESS_GET_MERCHANTACCT", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public int DeleteMerchantAcct(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_DELETE_MERCHANTACCT", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
        public MerchantAcctObj FindMerchantAcct(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<MerchantAcctObj>("SESS_FIND_MERCHANTACCT", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }

        }
        public int PurgeMerchantAcct(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_MERCHANTACCT", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
