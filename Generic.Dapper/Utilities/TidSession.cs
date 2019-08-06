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
    public class TidSession 
    {
        public OutPutObj PostTerminal(TerminalObj obj, int postType)
        {
            OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("PID", obj.PID, DbType.String);
            p.Add("TERMINALID", obj.TERMINALID, DbType.String);
            p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);
            p.Add("ACCOUNT_ID", obj.ACCOUNT_ID, DbType.Decimal);
            p.Add("PTSA", obj.PTSA, DbType.String);
            p.Add("PTSP", obj.PTSP, DbType.String);
            p.Add("TRANSACTION_CURRENCY", obj.TRANSACTION_CURRENCY, DbType.String);
            p.Add("SETTLEMENT_CURRENCY", obj.SETTLEMENT_CURRENCY, DbType.String);
            p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("STATUS", obj.STATUS, DbType.String);
            p.Add("DB_ITBID", obj.DB_ITBID, DbType.Decimal);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);
            p.Add("SETTLEMENT_FREQUENCY", obj.SETTLEMENT_FREQUENCY, DbType.Int32);
            p.Add("TERMINALMODEL_CODE", obj.TERMINALMODEL_CODE, DbType.String);
            p.Add("TERMINALOWNER_CODE", obj.TERMINALOWNER_CODE, DbType.String);
            p.Add("EmailAlert", obj.EmailAlert, DbType.Boolean);
            p.Add("VISAACQUIRERIDNO", obj.VISAACQUIRERIDNO, DbType.String);
            p.Add("MASTACQUIRERIDNO", obj.MASTACQUIRERIDNO, DbType.String);
            p.Add("VERVACQUIRERIDNO", obj.VERVACQUIRERIDNO, DbType.String);
            p.Add("SLIP_FOOTER", obj.SLIP_FOOTER, DbType.String);
            p.Add("SLIP_HEADER", obj.SLIP_HEADER, DbType.String);
            p.Add("ACCOUNTID", obj.ACCOUNTID, DbType.String);
            p.Add("DEPOSIT_ACCOUNTNO", obj.DEPOSIT_ACCOUNTNO, DbType.String);
            p.Add("DEPOSIT_ACCTNAME", obj.DEPOSIT_ACCTNAME, DbType.String);
            p.Add("DEPOSIT_BANKNAME", obj.DEPOSIT_BANKNAME, DbType.String);
            p.Add("IS_NEWACCT", obj.IS_NEWACCT, DbType.Boolean);
            
            using (var con = new RepoBase().OpenConnection(null))
            {
                ret = con.Query<OutPutObj>("SESS_POST_TERMINAL", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public List<TerminalObj> GetTerminal(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<TerminalObj>("SESS_GET_TERMINAL", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public List<TerminalObj> GetTerminal2(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<TerminalObj>("SESS_GET_TERMINAL2", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public int DeleteTerminal(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_DELETE_TERMINAL", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
        public TerminalObj FindTerminal(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<TerminalObj>("SESS_FIND_TERMINAL", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }

        }
        public int PurgeTerminal(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_TERMINAL", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
