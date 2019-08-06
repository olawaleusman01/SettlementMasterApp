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
    public class DataBinUpldSession
    {
        public int PostBinUpload(DataBinUpldObj obj,int postType,string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@PID", obj.PID, DbType.String);
            p.Add("@BANKFIID", obj.BANKFIID, DbType.String);
            p.Add("@BIN", obj.BIN, DbType.String);
            p.Add("@BUSINESSTYPE", obj.BUSINESSTYPE, DbType.String);
            p.Add("@CARDSCHEME", obj.CARDSCHEME, DbType.String);
            p.Add("@CBNCODE", obj.CBNCODE, DbType.String);
            p.Add("@COUNTRYCODE", obj.COUNTRYCODE, DbType.String);
            p.Add("@CURRENCYCODE", obj.CURRENCYCODE, DbType.String);
            p.Add("@ISSUERFIID", obj.ISSUERFIID, DbType.String);
            p.Add("@BATCHID", null, DbType.String);
            p.Add("USERID", userId, DbType.String);
            p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
            p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
            p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
            p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);
            p.Add("POSTSEQUENCE", obj.POSTSEQUENCE, DbType.Int32);

            using (var con = new RepoBase().OpenConnection(null))
            {
                var rst = con.Execute("SESS_POST_DATABINUPLD", p, commandType:CommandType.StoredProcedure);
                return rst;
            }
        }
        public int PostBinUploadBulk(List<DataBinUpldObj> objList,string userId)
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
                var rst = con.Execute("SESS_PURGE_DATABINUPLD", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {

                    p.Add("@PID", obj.PID, DbType.String);
                    p.Add("@BANKFIID", obj.BANKFIID, DbType.String);
                    p.Add("@BIN", obj.BIN, DbType.String);
                    p.Add("@BUSINESSTYPE", obj.BUSINESSTYPE, DbType.String);
                    p.Add("@CARDSCHEME", obj.CARDSCHEME, DbType.String);
                    p.Add("@CBNCODE", obj.CBNCODE, DbType.String);
                    p.Add("@COUNTRYCODE", obj.COUNTRYCODE, DbType.String);
                    p.Add("@CURRENCYCODE", obj.CURRENCYCODE, DbType.String);
                    p.Add("@ISSUERFIID", obj.ISSUERFIID, DbType.String);
                    p.Add("@BATCHID", null, DbType.String);
                    p.Add("USERID", userId, DbType.String);
                    p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.Int16);
                    p.Add("PostSequence", sqn++, DbType.Int32);
                    

                    cnt += con.Execute("SESS_POST_DATABINUPLD", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public List<DataBinUpldObj> GetDataBinUpload(string userId)
        {
           // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<DataBinUpldObj>("SESS_GET_DATABINUPLD", p, commandType: CommandType.StoredProcedure).ToList();
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
        public int PurgeDataBinUpload(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_DATABINUPLD", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
