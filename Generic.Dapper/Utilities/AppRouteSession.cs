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
    public class AppRouteSession : RepoBase
    {
        public OutPutObj PostRouteOfficer(ApprovalRouteOffObj obj,int postType)
        {
            OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("PID", obj.PID, DbType.String);
            p.Add("APPROVER_ID", obj.APPROVER_ID, DbType.String);
            p.Add("MENUID", obj.MENUID, DbType.Int32);
            p.Add("PRIORITY", obj.PRIORITY, DbType.Int32);
            p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("DB_ITBID", obj.DB_ITBID, DbType.Int32);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);
            using (var con = new RepoBase().OpenConnection(null))
            {
                ret = con.Query<OutPutObj>("SESS_POST_APPROVALOFFICER", p, commandType:CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }
        }
      
        public List<ApprovalRouteOffObj> GetRouteOfficer(string userId)
        {
           // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<ApprovalRouteOffObj>("SESS_GET_APPROVALOFFICER", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public int DeleteRouteOfficer(string Id,string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("PID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_DELETE_APPROVALOFFICER", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
        public ApprovalRouteOffObj FindRouteOfficer(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("PID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<ApprovalRouteOffObj>("SESS_FIND_APPROVALOFFICER", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }

        }
        public int PurgeRouteOfficer(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_APPROVALOFFICER", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
