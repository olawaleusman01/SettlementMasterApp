using Generic.Dapper.Model;
using Generic.Dapper.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Collections;

namespace Generic.Dapper.Utilities
{
    public class RvHeadSession : RepoBase
    {
        public OutPutObj PostRevenueHead(RvHeadObj obj,int postType)
        {
            OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("ID", obj.PID, DbType.String);
            p.Add("ACCOUNT_ID", obj.ACCOUNT_ID, DbType.Decimal);
            p.Add("CODE", obj.CODE, DbType.String);
            p.Add("RVGROUPCODE", obj.RVGROUPCODE, DbType.String);
            p.Add("MID", obj.MID, DbType.String);
            p.Add("PAYMENTITEMID", obj.PAYMENTITEMID, DbType.Int32);
            p.Add("DESCRIPTION", obj.DESCRIPTION, DbType.String);
            p.Add("BANKCODE", obj.BANKCODE, DbType.String);
            p.Add("BANKACCOUNT", obj.BANKACCOUNT, DbType.String);
            p.Add("BANK_NAME", obj.BANK_NAME, DbType.String);
            p.Add("ACCT_NAME", obj.ACCT_NAME, DbType.String);
            p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("STATUS", obj.STATUS, DbType.String);
            p.Add("DB_ITBID", obj.DB_ITBID, DbType.Int32);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);
            p.Add("SETTLEMENT_FREQUENCY", obj.SETTLEMENT_FREQUENCY, DbType.Int32);
            p.Add("FREQUENCY_DESC", obj.FREQUENCY_DESC, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                ret = con.Query<OutPutObj>("POST_REVENUEHEAD_SESSION", p, commandType:CommandType.StoredProcedure).FirstOrDefault();
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
        public List<RvHeadObj> GetRevenueHead(string userId)
        {
           // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<RvHeadObj>("GET_SM_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public int DeleteRevenueHead(string Id,string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("DELETE_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
        public RvHeadObj FindRevenueHead(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<RvHeadObj>("FIND_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }

        }
        public int PurgeRevenueHead(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("PURGE_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
