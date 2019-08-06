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
    public class RvDrAcctUpldSession
    {
        public int PostRvDrAcctUpload(RvDrAcctUpldObj obj, int postType, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@PID", obj.PID, DbType.String);
            p.Add("@GROUPCODE", obj.RVGROUPCODE, DbType.String);
            p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
            p.Add("@AGENTCODE", obj.AGENT_CODE, DbType.String);
            p.Add("@BANKACCTNAME", obj.BANKACCTNAME, DbType.String);
            p.Add("@BANKACCTNO", obj.BANKACCTNO, DbType.String);
            p.Add("@BANKCODE", obj.BANKCODE, DbType.String);
            p.Add("USERID", userId, DbType.String);
            p.Add("STATUS", obj.STATUS, DbType.String);
            p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
            p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
            p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
            p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
            p.Add("POSTTYPE", 2, DbType.Int16);
            p.Add("PostSequence", obj.POSTSEQUENCE, DbType.Int32);

            using (var con = new RepoBase().OpenConnection(null))
            {
                var rst = con.Execute("SESS_POST_RVDRACCTUPLD", p, commandType: CommandType.StoredProcedure);
                return rst;
            }
        }
        public int PostRvDrAcctUploadBulk(List<RvDrAcctUpldObj> objList, string userId)
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
                var rst = con.Execute("SESS_PURGE_RVDRACCTUPLD", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {

                    p.Add("@PID", obj.PID, DbType.String);
                    p.Add("@GROUPCODE", obj.RVGROUPCODE, DbType.String);
                    p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
                    p.Add("@AGENTCODE", obj.AGENT_CODE, DbType.String);
                    p.Add("@BANKACCTNAME", obj.BANKACCTNAME, DbType.String);
                    p.Add("@BANKACCTNO", obj.BANKACCTNO, DbType.String);
                    p.Add("@BANKCODE", obj.BANKCODE, DbType.String);
                    p.Add("USERID", userId, DbType.String);
                    p.Add("STATUS", obj.STATUS, DbType.String);
                    p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.Int16);
                    p.Add("PostSequence", sqn++, DbType.Int32);


                    cnt += con.Execute("SESS_POST_RVDRACCTUPLD", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public List<RvDrAcctUpldObj> GetRvDrAcctUpload(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<RvDrAcctUpldObj>("SESS_GET_RVDRACCTUPLD", p, commandType: CommandType.StoredProcedure).ToList();
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
        public int PurgeRvDrAcctUpload(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_RVDRACCTUPLD", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
        
    }
}
