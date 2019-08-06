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
    public class SetReconUpldSession
    {
        public int PostSetReconUpload(SetReconUpldObj obj, int postType, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@PID", obj.PID, DbType.String);
            p.Add("@PAYREFNO", obj.PAYREFNO, DbType.String);
            p.Add("@AMOUNT", obj.AMOUNT, DbType.Decimal);
            p.Add("@PAYMENTDATE", obj.PAYMENTDATE, DbType.DateTime);
            p.Add("@VALUEDATE", obj.VALUEDATE, DbType.DateTime);
            //p.Add("@RECEIPTNO", obj.RECEIPTNO, DbType.String);
            p.Add("@CUSTOMERNAME", obj.CUSTOMERNAME, DbType.String);
            p.Add("@PAYMENTMETHOD", obj.PAYMENTMETHOD, DbType.String);
            //p.Add("@TRANSACTIONSTATUS", obj.TRANSACTIONSTATUS, DbType.String);
            //p.Add("@DEPOSITSLIPNO", obj.DEPOSITSLIPNO, DbType.String);
            p.Add("@BANKNAME", obj.BANKNAME, DbType.String);
            //p.Add("@BRANCHNAME", obj.BRANCHNAME, DbType.String);
            //p.Add("@PAYERID", obj.PAYERID, DbType.String);
            //p.Add("@VALUEGRANTED", obj.VALUEGRANTED, DbType.String);
           // p.Add("@RECONCILE", obj.RECONCILE, DbType.Int32);
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
                var rst = con.Execute("SESS_POST_SETRECON", p, commandType: CommandType.StoredProcedure);
                return rst;
            }
        }
        public int PostSetReconUploadBulk(List<SetReconUpldObj> objList, string userId)
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
                var rst = con.Execute("SESS_PURGE_SETRECON", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {

                    p.Add("@PID", obj.PID, DbType.String);
                    p.Add("@PAYREFNO", obj.PAYREFNO, DbType.String);
                    p.Add("@AMOUNT", obj.AMOUNT, DbType.Decimal);
                    p.Add("@PAYMENTDATE", obj.PAYMENTDATE, DbType.DateTime);
                    p.Add("@VALUEDATE", obj.VALUEDATE, DbType.DateTime);
                   // p.Add("@RECEIPTNO", obj.RECEIPTNO, DbType.String);
                    p.Add("@CUSTOMERNAME", obj.CUSTOMERNAME, DbType.String);
                    p.Add("@PAYMENTMETHOD", obj.PAYMENTMETHOD, DbType.String);
                    //p.Add("@TRANSACTIONSTATUS", obj.TRANSACTIONSTATUS, DbType.String);
                    //p.Add("@DEPOSITSLIPNO", obj.DEPOSITSLIPNO, DbType.String);
                    p.Add("@BANKNAME", obj.BANKNAME, DbType.String);
                    //p.Add("@BRANCHNAME", obj.BRANCHNAME, DbType.String);
                    //p.Add("@PAYERID", obj.PAYERID, DbType.String);
                    //p.Add("@VALUEGRANTED", obj.VALUEGRANTED, DbType.String);
                    //p.Add("@RECONCILE", obj.RECONCILE, DbType.Int32);
                    p.Add("@BATCHID", null, DbType.String);
                    p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.Int16);
                    p.Add("PostSequence", sqn++, DbType.Int32);


                    cnt += con.Execute("SESS_POST_SETRECON", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public List<SetReconUpldObj> GetSetReconUpload(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SetReconUpldObj>("SESS_GET_SETRECON", p, commandType:CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public List<SetReconObj> GetSetRecon(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SetReconObj>("GET_SETRECON", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public List<SetReconUpldObj> ReconcileBatch(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SetReconUpldObj>("SESS_SET_RECONCILE", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        
        public int PurgeSetReconUpload(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_SETRECON", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
