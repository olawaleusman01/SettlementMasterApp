using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Dapper.Repository;
using Generic.Dapper.Data;

namespace Generic.Dapper.Model
{
    public class LoginMultiple: RepoBase,ILoginMultiple
    {
        public bool IsYourLoginStillTrue(string userId, string sessionId)
        {
            var p = new DynamicParameters();
            p.Add("@UserId", userId, DbType.String, null);
            p.Add("@SessionId", sessionId, DbType.String, null);
            var rec = Fetch(c => c.Query<bool>("proc_LoginStillTrue", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public bool IsUserLoggedOnElsewhere(string userId, string sessionId)
        {
            var p = new DynamicParameters();
            p.Add("@UserId", userId, DbType.String, null);
            p.Add("@SessionId", sessionId, DbType.String, null);
            var rec = Fetch(c => c.Query<bool>("proc_LoginElseWhere", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public bool LogEveryoneElseOut(string userId, string sessionId)
        {
            var p = new DynamicParameters();
            p.Add("@UserId", userId, DbType.String, null);
            p.Add("@SessionId", sessionId, DbType.String, null);
            var rec = Fetch(c => c.Query<bool>("proc_LoginEveryoneElseOut", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public int PostLogins(string userId, string sessionId)
        {
            var p = new DynamicParameters();
            p.Add("@UserId", userId, DbType.String, null);
            p.Add("@SessionId", sessionId, DbType.String, null);
            var rec = Execute(c => c.Execute("proc_Login", p, commandType: CommandType.StoredProcedure), null);
            return rec;
        }
    }
}
