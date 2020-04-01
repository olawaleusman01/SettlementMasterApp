using Dapper;
using Generic.Dapper.Model;
using Generic.Dapper.Repository;
using Generic.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public class DapperATMSettings : RepoBase, IDapperATMSettings
    {
        public List<DropdownObj> GetATMPartyType()
        {
            DynamicParameters p = new DynamicParameters();
            string qry = "GET_ATMPartyTypeCombo";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<DropdownObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public List<ATMChargesObj> GetATMCharges_Temp(string batchId,string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);

            string qry = "Get_ATMCharges_Temp";

            var rec = Fetch(c => c.Query<ATMChargesObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<ATMChargesObj>> GetATMChargesAsync()
        {
            DynamicParameters p = new DynamicParameters();

            string qry = "Get_ATMCharges";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<ATMChargesObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<MenuPrivObj>> GetMenuPrivilegeAsync()
        {

            //var p = new DynamicParameters();

            //p.Add("@RoleId", roleId, DbType.Int32, null);

            ////  p.Add("@IsAll", IsAll, DbType.Boolean, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MenuPrivObj>("proc_GetMenuPriviledge", null, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MenuPrivObj> GetMenuPrivilegeTemp(string batchId, string userId)
        {

            var p = new DynamicParameters();
            p.Add("@P_BatchId", batchId, DbType.String, null);
            p.Add("@P_UserId", userId, DbType.String, null);
            var rec = Fetch(c => c.Query<MenuPrivObj>("Get_MenuPrivTemp", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public List<ParentMenu> GetParentMenu(int roleId, string app_key)
        {
            var p = new DynamicParameters();
            p.Add("@RoleId", roleId, DbType.Int32);
            p.Add("@AppKey", app_key, DbType.String);
            var qry = "proc_parentAppMenu";
            var rec = Fetch(c => c.Query<ParentMenu>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<ChildMenu> GetChildMenu(int roleId, string app_key)
        {

            var p = new DynamicParameters();
            p.Add("@RoleId", roleId, DbType.Int32);
            p.Add("@AppKey", app_key, DbType.String);
            var qry = "proc_childAppMenu";

            var rec = Fetch(c => c.Query<ChildMenu>(qry, p, commandType: CommandType.StoredProcedure), null);
            // var rec1 = Fetch(c => c.Query<ChildMenu>(qry, null, commandType: CommandType.Text), null);
            return rec.ToList();
            //}

            // return userInfo;


        }
    }

    
}
