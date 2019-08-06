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
    public class SetReconDualUpldSession
    {
        public int PostSetReconDualUpload(SetReconDualUpldObj obj, int postType, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@PID", obj.PID, DbType.String);
            p.Add("@REFNO", obj.REFERENCENO, DbType.String);
            p.Add("@CARDTYPE", obj.CARDTYPE, DbType.String);
            p.Add("@TRANTYPE", obj.TRANSACTIONTYPE, DbType.String);
            p.Add("@TRANDATETIME", obj.TRANSACTIONDATETIME, DbType.DateTime);
            p.Add("@SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.DateTime);
            p.Add("@MASKEDPAN", obj.MASKEDPAN, DbType.String);
            p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
            p.Add("@MERCHANTACCOUNT", obj.MERCHANTACCOUNT, DbType.String);
            p.Add("@MERCHANTNAME", obj.MERCHANTNAME, DbType.String);
            p.Add("@MERCHANTLOCATION", obj.MERCHANTLOCATION, DbType.String);
            p.Add("@TERMINALID", obj.TERMINALID, DbType.String);
            p.Add("@TRANAMOUNT", obj.TRANAMOUNT, DbType.Decimal);
            p.Add("@AMOUNTCHARGED", obj.AMOUNTCHARGED, DbType.Decimal);
            p.Add("@SETTLEMENTAMOUNT", obj.SETTLEMENTAMOUNT, DbType.Decimal);
            p.Add("@MSCRATE", obj.MSCRATE, DbType.Decimal);
            p.Add("@BATCHTYPE", obj.BATCHTYPE, DbType.String);
            p.Add("@BATCHID", null, DbType.String);
            p.Add("@USERID", userId, DbType.String);
            p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
            p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
            p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
            p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);
            p.Add("POSTSEQUENCE", obj.POSTSEQUENCE, DbType.Int32);

            using (var con = new RepoBase().OpenConnection(null))
            {
                var rst = con.Execute("SESS_POST_SETRECONUPLD", p, commandType: CommandType.StoredProcedure);
                return rst;
            }
        }
        public int PostSetReconDualUploadBulkA(List<SetReconDualUpldObj> objList, string userId)
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
                var model = new SetReconDualUpldObj();

                var rst = con.Execute("SESS_PURGE_SETRECONUPLD", p, commandType: CommandType.StoredProcedure);

                foreach (var obj in objList)
                {

                    p.Add("@PID", obj.PID, DbType.String);
                    p.Add("@REFNO", obj.REFERENCENO, DbType.String);
                    p.Add("@CARDTYPE", obj.CARDTYPE, DbType.String);
                    p.Add("@TRANTYPE", obj.TRANSACTIONTYPE, DbType.String);
                    p.Add("@TRANDATETIME", obj.TRANSACTIONDATETIME, DbType.DateTime);
                    p.Add("@SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.DateTime);
                    p.Add("@MASKEDPAN", obj.MASKEDPAN, DbType.String);
                    p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
                    p.Add("@MERCHANTACCOUNT", obj.MERCHANTACCOUNT, DbType.String);
                    p.Add("@MERCHANTNAME", obj.MERCHANTNAME, DbType.String);
                    p.Add("@MERCHANTLOCATION", obj.MERCHANTLOCATION, DbType.String);
                    p.Add("@TERMINALID", obj.TERMINALID, DbType.String);
                    p.Add("@TRANAMOUNT", obj.TRANAMOUNT, DbType.Decimal);
                    p.Add("@AMOUNTCHARGED", obj.AMOUNTCHARGED, DbType.Decimal);
                    p.Add("@SETTLEMENTAMOUNT", obj.SETTLEMENTAMOUNT, DbType.Decimal);
                    p.Add("@MSCRATE", obj.MSCRATE, DbType.Decimal);
                    p.Add("@BATCHTYPE", "A", DbType.String);
                    p.Add("@BATCHID", null, DbType.String);
                    p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.Int16);
                    p.Add("PostSequence", sqn++, DbType.Int32);

                    cnt += con.Execute("SESS_POST_SETRECONUPLD", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }

        public int PostSetReconDualUploadBulkB(List<SetReconDualUpldObj> objList, string userId)
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
                //var rst = con.Execute("SESS_PURGE_SETRECONUPLD", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {

                    p.Add("@PID", obj.PID, DbType.String);
                    p.Add("@REFNO", obj.REFERENCENO, DbType.String);
                    p.Add("@CARDTYPE", obj.CARDTYPE, DbType.String);
                    p.Add("@TRANTYPE", obj.TRANSACTIONTYPE, DbType.String);
                    p.Add("@TRANDATETIME", obj.TRANSACTIONDATETIME, DbType.DateTime);
                    p.Add("@SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.DateTime);
                    p.Add("@MASKEDPAN", obj.MASKEDPAN, DbType.String);
                    p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
                    p.Add("@MERCHANTACCOUNT", obj.MERCHANTACCOUNT, DbType.String);
                    p.Add("@MERCHANTNAME", obj.MERCHANTNAME, DbType.String);
                    p.Add("@MERCHANTLOCATION", obj.MERCHANTLOCATION, DbType.String);
                    p.Add("@TERMINALID", obj.TERMINALID, DbType.String);
                    p.Add("@TRANAMOUNT", obj.TRANAMOUNT, DbType.Decimal);
                    p.Add("@AMOUNTCHARGED", obj.AMOUNTCHARGED, DbType.Decimal);
                    p.Add("@SETTLEMENTAMOUNT", obj.SETTLEMENTAMOUNT, DbType.Decimal);
                    p.Add("@MSCRATE", obj.MSCRATE, DbType.Decimal);
                    p.Add("@BATCHTYPE", "B", DbType.String);
                    p.Add("@BATCHID", null, DbType.String);
                    p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.Int16);
                    p.Add("PostSequence", sqn++, DbType.Int32);


                    cnt += con.Execute("SESS_POST_SETRECONUPLD", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public List<SetReconDualUpldObj> GetSetReconDualUpload(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SetReconDualUpldObj>("SESS_GET_SETRECONUPLD", p, commandType:CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public List<SetReconDualObj> GetSetReconDual(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SetReconDualObj>("GET_SETRECONUPLD", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public List<SetReconDualUpldObj> DualReconcileBatch(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SetReconDualUpldObj>("SESS_SET_DUAL_RECONCILE", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        
        public int PurgeSetReconDualUpload(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_SETRECONUPLD", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
