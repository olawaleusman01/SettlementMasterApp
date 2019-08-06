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
    public class RvHeadUpldSession
    {
        public int PostRvHeadUpload(RvHeadUpldObj obj,int postType,string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@ID", obj.PID, DbType.String);
            p.Add("@RVCODE", obj.RVCODE, DbType.String);
            p.Add("@GROUPCODE", obj.GROUPCODE, DbType.String);
            p.Add("@DESCRIPTION", obj.DESCRIPTION, DbType.String);
            p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
            p.Add("@USERID", userId, DbType.String);
            p.Add("@STATUS", obj.STATUS, DbType.String);
            p.Add("@PAYMENTITEMID", obj.PAYMENTITEMID, DbType.Int32);
            p.Add("@BANKCODE", obj.BANKCODE, DbType.String);
            p.Add("@BANKACCOUNT", obj.BANKACCOUNT, DbType.String);
            p.Add("@SETTLEMENT_FREQUENCY", obj.SETTLEMENT_FREQUENCY, DbType.Int32);
            p.Add("@POSTTYPE", 2, DbType.Int16);
            p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
            p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
            p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
            p.Add("@PostSequence", obj.POSTSEQUENCE, DbType.Int32);
            p.Add("@ACCOUNT_ID", obj.ACCOUNT_ID, DbType.Decimal);
            
            using (var con = new RepoBase().OpenConnection(null))
            {
                var rst = con.Execute("SESS_POST_RVHEADUPLD", p, commandType:CommandType.StoredProcedure);
                return rst;
            }
        }
        public int PostRvHeadUploadBulk(List<RvHeadUpldObj> objList,string userId)
        {
            var curDate = DateTime.Now;
            //OutPutObj ret = new OutPutObj();
            var cnt = 0;
            var p = new DynamicParameters();
            using (var con = new RepoBase().OpenConnection(null))
            {
                //PURGE ALL RECORD FOR CURRENT USER
                int sqn = 0;
                p.Add("@USERID", userId, DbType.String);
                var rst = con.Execute("SESS_PURGE_RVHEADUPLD", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {
                    p.Add("@ID", obj.PID, DbType.String);
                    p.Add("@RVCODE", obj.RVCODE, DbType.String);
                    p.Add("@GROUPCODE", obj.GROUPCODE, DbType.String);
                    p.Add("@DESCRIPTION", obj.DESCRIPTION, DbType.String);
                    p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
                    //p.Add("USERID", userId, DbType.String);
                    p.Add("@STATUS", obj.STATUS, DbType.String);
                    p.Add("@PAYMENTITEMID", obj.PAYMENTITEMID, DbType.Int32);
                    p.Add("@BANKCODE", obj.BANKCODE, DbType.String);
                    p.Add("@BANKACCOUNT", obj.BANKACCOUNT, DbType.String);
                    p.Add("@SETTLEMENT_FREQUENCY", obj.SETTLEMENT_FREQUENCY, DbType.Int32);
                    p.Add("@POSTTYPE", 1, DbType.Int16);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("@PostSequence", sqn++, DbType.Int32);
                    p.Add("@ACCOUNT_ID", obj.ACCOUNT_ID, DbType.Decimal);

                    cnt += con.Execute("SESS_POST_RVHEADUPLD", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public List<RvHeadUpldObj> GetRvHeadUpload(string userId)
        {
           // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<RvHeadUpldObj>("SESS_GET_RVHEADUPLD", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        //public int DeleteDataBinUpload(string Id,string userId)
        //{
        //    //OutPutObj ret = new OutPutObj();
        //    var p = new DynamicParameters();
        //    p.Add("USERID", userId, DbType.String);
        //    p.Add("ID", Id, DbType.String);
        //    using (var con = new RepoBase().OpenConnection(null))
        //    {
        //        var ret = con.Execute("DELETE_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure);
        //        return ret;
        //    }

        //}
        //public RvHeadObj FindRevenueHead(string Id, string userId)
        //{
        //    //OutPutObj ret = new OutPutObj();
        //    var p = new DynamicParameters();
        //    p.Add("USERID", userId, DbType.String);
        //    p.Add("ID", Id, DbType.String);
        //    using (var con = new RepoBase().OpenConnection(null))
        //    {
        //        var ret = con.Query<RvHeadObj>("FIND_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
        //        return ret;
        //    }

        //}
        public int PurgeRvHeadUpload(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_RVHEADUPLD", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
