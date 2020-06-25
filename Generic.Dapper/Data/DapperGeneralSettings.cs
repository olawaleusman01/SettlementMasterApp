using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Generic.Dapper.Repository;
using Generic.Data.Utilities;
using Generic.Dapper.Model;
using Generic.Data;
using System.Transactions;
using Generic.Dapper.Utility;
using Generic.Data.Model;

namespace Generic.Dapper.Data
{
    public class DapperGeneralSettings : RepoBase, IDapperGeneralSettings
    {

        //readonly IDapperRepository<sf_Customer> _repoCustomer = new DapperRepository<sf_Customer>();

        //public List<proc_GetUsers_Result> GetUsers(string UserId, bool IsAll = false, string conString = null)
        //{
        //    var p = new DynamicParameters();
        //    p.Add("@UserId", UserId, DbType.String, null);
        //    p.Add("@IsAll", IsAll, DbType.Boolean, null);
        //    string sql = @"proc_GetUsers";

        //    var rec = Fetch(c => c.Query<proc_GetUsers_Result>(sql, p, commandType: CommandType.StoredProcedure).ToList(), conString);
        //    return rec;

        //  //  return rec.ToList();
        //}


        //public sf_CompanyProfile GetCompanyProfile(string conString = null)
        //{

        //    string sql = @"proc_GetCompanyProfile";

        //    var rec = Fetch(c => c.Query<sf_CompanyProfile>(sql, null, commandType: CommandType.StoredProcedure).SingleOrDefault(), conString);
        //    return rec;

        //}


        //public int PostBatchTranxUploadSybase(List<BatchUploadTranx_Log> objList, string userId)
        //{
        //    int cnt = 0;
        //    using (var connection = new RepoBase().OpenSybaseConnection(null))
        //   // using (var transaction = connection.BeginTransaction())
        //    {
        //        // var batchId = SmartObj.GenRefNo2();
        //        var id = Guid.NewGuid();
        //        foreach (var obj in objList)
        //        {
        //            var p = new DynamicParameters();
        //            p.Add("@batchid", obj.BatchId, DbType.Decimal);
        //            p.Add("@Settlementaccount", obj.CustomerAcctNo, DbType.String);
        //            p.Add("@amt", obj.Amount, DbType.Decimal);
        //            p.Add("@trandate", obj.TransactionDate, DbType.Date);
        //            p.Add("@narration", obj.Narration, DbType.String);

        //            //  batchid int, @Settlementaccount varchar(20),@amt numeric(22, 2),@trandate datetime, @narration nvarchar(200)

        //            //string sql = string.Format( @"declare @retVal int
        //            //        exec @retVal = proc_Bulkdepositprocess {0},'{1}',{2},'{3}','{4}'
        //            //        --print @batchid
        //            //        select @retVal RespCode",obj.BatchId,obj.CustomerAcctNo,obj.Amount,obj.TransactionDate,obj.Narration);
        //            string sql = @"proc_Bulkdepositprocess";

        //           // cnt += connection.Query<int>(sql, null, commandType: CommandType.Text).FirstOrDefault();
        //            var gb  = connection.Query<object>(sql, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
        //            if (gb != null)
        //                {
        //               // cnt += gb.RespCode ?? 0;
        //            }
        //        }
        //        // return rec;
        //        //if (cnt > 0 && cnt == objList.Count)
        //        //{
        //        //   // transaction.Commit();
        //        //}
        //    }
        //    return cnt;
        //}

        public int PostProcessReset()
        {
            
            var rec = Fetch(c => c.Query<int>("PROC_RESETPROCESS", null, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }

        public int PostProcessReset2()
        {

            var rec = Fetch(c => c.Query<int>("PROC_RESETPROCESS2", null, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }

        public int PostProcessReset3()
        {

            var rec = Fetch(c => c.Query<int>("PROC_RESETPROCESS3", null, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public int PostLoginAudit(LoginAuditObj obj, int postType)
        {
            var inserted = 0;

            var p = new DynamicParameters();
            p.Add("@P_LOGINDATE", obj.LOGINDATE, DbType.DateTime);
            p.Add("@P_LOGOUTDATE", obj.LOGOUTDATE, DbType.DateTime);
            p.Add("@P_USERID", obj.UserId, DbType.String);
            p.Add("@P_BROWSER", obj.BROWSER, DbType.String);
            p.Add("@P_IPADDRESS", obj.IPADDRESS, DbType.String);
            p.Add("@P_MAC", obj.MAC, DbType.String);
            p.Add("@P_GUID", obj.guid, DbType.String);
            //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            if (postType == 1)
            {
                string sql = @"INSERT
                            INTO SM_LOGINAUDIT
                              (
                                USERID,
                                LOGINDATE,
                                LOGOUTDATE,
                                IP_ADDRESS,
                                MAC_ADDRESS,
                                BROWSER,
                                GUIDNO
                              )
                              VALUES
                              (
                                @P_USERID,
                                @P_LOGINDATE,
                                @P_LOGOUTDATE,
                                @P_IPADDRESS,
                                @P_MAC,
                                @P_BROWSER,
                                @P_GUID
                              )";
                inserted = Execute(c => c.Execute(sql, p
                        ), null);

            }
            else
            {
                p = new DynamicParameters();
                p.Add(":P_LOGOUTDATE", obj.LOGOUTDATE, DbType.DateTime);
                p.Add(":P_GUID", obj.guid, DbType.String);
                string sql = @"UPDATE SM_LOGINAUDIT
                                SET  LOGOUTDATE = :P_LOGOUTDATE
                                WHERE GUIDNO = :P_GUID";
                inserted = Execute(c => c.Execute(sql, p
                    ), null);

            }
            return inserted;
        }
        public int PostLoginAttempt(LoginAuditObj obj)
        {
            var inserted = 0;

            var p = new DynamicParameters();
            // p.Add(":P_LOGINDATE", obj.LOGINDATE, OracleDbType.Date);
            p.Add("@P_ATTEMPTDATE", obj.ATTEMPTDATE, DbType.DateTime);
            p.Add("@P_USERID", obj.UserId, DbType.String);
            p.Add("@P_BROWSER", obj.BROWSER, DbType.String);
            p.Add("@P_IPADDRESS", obj.IPADDRESS, DbType.String);
            p.Add("@P_MAC", obj.MAC, DbType.String);
            p.Add("@P_GUID", obj.guid, DbType.String);
            //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string sql = @"INSERT
                                INTO SM_LOGINATTEMPT
                                  (
                                    USERID,
                                    ATTEMPTDATE,
                                    IP_ADDRESS,
                                    MAC_ADDRESS,
                                    BROWSER,
                                    GUIDNO
                                  )
                                   VALUES
                                  (
                                    @P_USERID,
                                    @P_ATTEMPTDATE,
                                    @P_IPADDRESS,
                                    @P_MAC,
                                    @P_BROWSER,
                                    @P_GUID
                                  )";
            inserted += Execute(c => c.Execute(sql, p
                    ), null);


            return inserted;
        }
        public int PostSignOut(LoginAuditObj obj)
        {
            var inserted = 0;


            var p = new DynamicParameters();
            p.Add("@P_LOGOUTDATE", obj.LOGOUTDATE, DbType.DateTime);
            p.Add("@P_GUID", obj.guid, DbType.String);
            string sql = @"PROC_SIGNOUTUSER";
            inserted = Fetch(c => c.Query<int>(sql, p, commandType: CommandType.StoredProcedure
                ), null).FirstOrDefault();


            return inserted;
        }

        public async Task<UserPrivilege> GetUserPrivilegeAsync(int roleId, int menuId, bool buffered = false, string connectionString = null)
        {
            var p = new DynamicParameters();
            p.Add("@RoleId", roleId, DbType.Int32, null);
            p.Add("@MenuId", menuId, DbType.Int32, null);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<UserPrivilege>("proc_GetUserPrivilege", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        public UserPrivilege GetUserPrivilege(int roleId, int menuId, bool buffered = false, string connectionString = null)
        {
            var p = new DynamicParameters();
            p.Add("@RoleId", roleId, DbType.Int32, null);
            p.Add("@MenuId", menuId, DbType.Int32, null);
            var rec = Fetch(c => c.Query<UserPrivilege>("proc_GetUserPrivilege", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        //public async Task<List<UsersObj>> GetUserAsync(int ItbId, bool IsAll, bool buffered = false, string connectionString = null)
        //{
        //    var p = new DynamicParameters();
        //    p.Add("@ItbId", ItbId, DbType.Int32, null);
        //    p.Add("@IsAll", IsAll, DbType.Boolean, null);

        //    var rec = await FetchAsync(async c =>
        //    {
        //        var gh = await c.QueryAsync<UsersObj>("proc_GetUserList", p, commandType: CommandType.StoredProcedure);
        //        return gh;
        //    });
        //    return rec.ToList();
        //}
        public async Task<List<UserObj>> GetUserAsync(int itbid, bool isAll, string status = null, bool isTemp = false, bool buffered = false, string connectionString = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<UserObj>("proc_GetUserList", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public List<UserObj> GetUser(int ItbId, bool IsAll, bool isTemp = false, string status = null, bool buffered = false, string connectionString = null)
        {

            var p = new DynamicParameters();

            p.Add("@P_ITBID", ItbId, DbType.Int32);
            p.Add("@P_ISALL", IsAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);

            var rec = Fetch(c => c.Query<UserObj>("proc_GetUserList", p, commandType: CommandType.StoredProcedure), null).ToList();


            return rec;
        }

        public List<UserObj> GetUserLockOutTemp(string batchId, string userId)
        {
            var p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            var rec = Fetch(c => c.Query<UserObj>("GET_LOCKEDUSERTEMP", p, commandType: CommandType.StoredProcedure), null).ToList();
            return rec;
        }
        public async Task<List<UserObj>> GetUserPasswordResetAsync()
        {
            //DynamicParameters p = new DynamicParameters();
            //p.Add("@P_ITBID", itbid, DbType.Int32);
            //p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            //p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            //p.Add("@P_STATUS", status, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<UserObj>("GET_RESETPASSWORD", null, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<UserObj> GetUserPasswordResetTemp(string batchId, string userId)
        {
            var p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            var rec = Fetch(c => c.Query<UserObj>("GET_RESETPASSWORDTEMP", p, commandType: CommandType.StoredProcedure), null).ToList();
            return rec;
        }
        public async Task<List<RolesObj>> GetRolesAsync(int itbid, bool isAll, string status = null, bool isTemp = false, bool buffered = false, string connectionString = null)
        {

            var p = new DynamicParameters();

            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RolesObj>("GET_ROLES", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<RolesObj> GetRoles(int itbid, bool isAll, string status = null, bool isTemp = false, bool buffered = false, string connectionString = null)
        {

            var p = new DynamicParameters();

            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);

            var rec = Fetch(c => c.Query<RolesObj>("GET_ROLES", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<RolePrivilegeObj>> GetRolePrivilegeAsync(int roleId, bool buffered = false, string connectionString = null)
        {

            var p = new DynamicParameters();

            p.Add("@RoleId", roleId, DbType.Int32, null);

            //  p.Add("@IsAll", IsAll, DbType.Boolean, null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RolePrivilegeObj>("proc_GetRolePriviledge", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<RolePrivilegeObj> GetRolePrivilegeTemp(string batchId, string userId)
        {

            var p = new DynamicParameters();
            p.Add("@P_BatchId", batchId, DbType.String, null);
            p.Add("@P_UserId", userId, DbType.String, null);
            var rec = Fetch(c => c.Query<RolePrivilegeObj>("GET_ROLEPRIVTEMP", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public int GetUserAuthorization(int roleId, int menuId, string controller)
        {
            var p = new DynamicParameters();
            p.Add("@P_RoleId", roleId, DbType.Int32, null);
            p.Add("@P_MenuId", menuId, DbType.Int32, null);
            p.Add("@P_Controller", controller, DbType.String, null);

            var rec = Fetch(c => c.Query<int>("proc_GetAuthorization", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }



        #region Menu
        public List<ParentMenu> GetParentMenu(int roleId, string conString = null)
        {
            var p = new DynamicParameters();
            p.Add("@RoleId", roleId, DbType.Int32);
            var qry = "proc_parentMenu";
            var rec = Fetch(c => c.Query<ParentMenu>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<ParentMenu> GetParentMenu2(string conString = null)
        {
            var qry = "proc_parentMenu2";
            var rec = Fetch(c => c.Query<ParentMenu>(qry, null, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<ChildMenu> GetChildMenu(int roleId, string conString = null)
        {

            var p = new DynamicParameters();
            p.Add("@RoleId", roleId, DbType.Int32);
            var qry = "proc_childMenu";

            var rec = Fetch(c => c.Query<ChildMenu>(qry, p, commandType: CommandType.StoredProcedure), null);
            // var rec1 = Fetch(c => c.Query<ChildMenu>(qry, null, commandType: CommandType.Text), null);
            return rec.ToList();
            //}

            // return userInfo;


        }
        public List<Approval_Route> GetApprovalRoutePage(int menuId, string userId, short action, decimal? authId = null)
        {
            var p = new DynamicParameters();
            p.Add("@MenuId", menuId, DbType.Int32, null);
            p.Add("@ApproverId", userId, DbType.String, null);
            p.Add("@AuthId", authId, DbType.Decimal, null);
            p.Add("@Action", action, DbType.Int16, null);
            var rec = Fetch(c => c.Query<Approval_Route>("proc_GetMenuApprovalRoutePage", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public int GetIfPageRequiresApproval(int menuid, int postType, decimal? authId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@MenuId", menuid, DbType.Int32);
            p.Add("@PostType", postType, DbType.Int32);
            p.Add("@AuthId", authId, DbType.Decimal);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var qry = "proc_GetIfPageRequiresApproval";
            // var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            var rec = Fetch(c => c.Query<int>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public async Task<List<ApproverLineObj>> GetApprovalListForRequestAsync(decimal authId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@AuthId", authId, DbType.Decimal);

            var qry = "proc_GetApproverListForRequest";
            // var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<ApproverLineObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();


        }
        public async Task<List<MApproverObj>> GetMenuApproverOfficersAsync(int menuId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@MenuId", menuId, DbType.Int32);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "proc_GetApproverRouteOfficer";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MApproverObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MApproverObj> GetMenuApprover()
        {
            //DynamicParameters p = new DynamicParameters();
            //p.Add("@MenuId", menuId, DbType.Int32);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "proc_GetApproverRouteMenu";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<MApproverObj>(qry, null, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<CompanyProfileObj> GetCompanyProfileAsync(int itbid, bool isTemp = false, string status = null, bool buffered = false, string connectionString = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<CompanyProfileObj>("GET_COMPANY_PROFILE", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        
        public CompanyProfileObj GetCompanyProfile(int itbid, bool isTemp = false, string status = null, bool buffered = false, string connectionString = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            var rec = Fetch(c => c.Query<CompanyProfileObj>("GET_COMPANY_PROFILE", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public async Task<List<DepartmentObj>> GetDepartmentAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_DEPARTMENT";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<DepartmentObj> GetDepartment(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_DEPARTMENT";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<SectorObj>> GetSectorAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SECTOR";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SectorObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SectorObj> GetSector(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SECTOR";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<SectorObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<CardSchemeObj>> GetCardSchemeAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CARDSCHEME";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<CardSchemeObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<CardSchemeObj> GetCardScheme(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CARDSCHEME";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<CardSchemeObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<FrequencyObj>> GetFrequencyAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_FREQUENCY";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<FrequencyObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<FrequencyObj> GetFrequency(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_FREQUENCY";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<FrequencyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<CurrencyObj>> GetCurrencyAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CURRENCY";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<CurrencyObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<CurrencyObj> GetCurrency(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CURRENCY";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<CurrencyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<SERVICEChannelObj>> GetServiceChannelAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SERVICECHANNEL";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SERVICEChannelObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public List<SERVICEChannelObj> GetServiceChannel(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SERVICECHANNEL";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<SERVICEChannelObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<ChannelObj>> GetChannelAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CHANNEL";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<ChannelObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<ChannelObj> GetChannel(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CHANNEL";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<ChannelObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<PartyTypeObj>> GetPartyTypeAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_PARTYTYPE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<PartyTypeObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<PartyTypeObj> GetPartyType(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_PARTYTYPE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<PartyTypeObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<SettlementOptionObj>> GetSettlementOptionAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SETTLEMENTOPTION";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SettlementOptionObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SettlementOptionObj> GetSettlementOption(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SETTLEMENTOPTION";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<SettlementOptionObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<PartyObj>> GetPartyAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_PARTY_LIST";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<PartyObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<PartyObj> GetParty(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_PARTY_LIST";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<PartyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<BillerObj>> GetBillerAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_BILLER_LIST";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<BillerObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<BillerObj> GetBiller(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_BILLER_LIST";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<BillerObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<BillerMscObj>> GetBillerMscAsync(string billerCode, int channelId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BILLERCODE", billerCode, DbType.String);
            p.Add("@P_CHANNELID", channelId, DbType.Int32);

            string qry = "GET_BILLERMSC";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<BillerMscObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<BillerMscObj> GetBillerMsc(string billerCode, int channelId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BILLERCODE", billerCode, DbType.String);
            p.Add("@P_CHANNELID", channelId, DbType.Int32);

            string qry = "GET_BILLERMSC";
            var rec = Fetch(c => c.Query<BillerMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<BillerMscObj>> GetBillerMscTempAsync(string batchId, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);

            string qry = "GET_BILLERMSCTEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<BillerMscObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<BillerMscObj> GetBillerMscTemp(string batchId, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);

            string qry = "GET_BILLERMSCTEMP";
            var rec = Fetch(c => c.Query<BillerMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<SharingPartyObj>> GetBillerFee1SharingPartyAsync(string billerCode, decimal mscItbId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BILLERCODE", billerCode, DbType.String);
            //p.Add("@P_CHANNELID", channelId, DbType.Int32);
            p.Add("@P_BILLERMSC_ITBID", mscItbId, DbType.Decimal);
            string qry = "GET_BL_FEE1SHARINGPARTY";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SharingPartyObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SharingPartyObj> GetBillerFee1SharingParty(string billerCode, decimal mscItbId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BILLERCODE", billerCode, DbType.String);
            //p.Add("@P_CHANNELID", channelId, DbType.Int32);
            p.Add("@P_BILLERMSC_ITBID", mscItbId, DbType.Decimal);
            string qry = "GET_BL_FEE1SHARINGPARTY";
            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<SharingPartyObj>> GetBillerFee1SharingPartyTempAsync(string batchId, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);

            string qry = "GET_BL_FEE1SHARINGPARTYTEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SharingPartyObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SharingPartyObj> GetBillerFee1SharingPartyTemp(string batchId, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);

            string qry = "GET_BL_FEE1SHARINGPARTYTEMP";
            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<PartyAcctObj>> GetPartyAcctAsync(int party_itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            string qry = "GET_PARTY_ACCT";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<PartyAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<PartyAcctObj> GetPartyAcct(int party_itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            string qry = "GET_PARTY_ACCT";
            var rec = Fetch(c => c.Query<PartyAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<PartyAcctObj>> GetPartyAcctTempAsync(int party_itbid, string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_PARTY_ACCT_TEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<PartyAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<PartyAcctObj> GetPartyAcctTemp(int party_itbid, string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_PARTY_ACCT_TEMP";
            var rec = Fetch(c => c.Query<PartyAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<SettlementRuleObj>> GetSettlementRuleAsync(int option_itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_OPTION_ITBID", option_itbid, DbType.Int32);
            string qry = "GET_SETTLEMENTRULE";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SettlementRuleObj> GetSettlementRule(int option_itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_OPTION_ITBID", option_itbid, DbType.Int32);
            string qry = "GET_SETTLEMENTRULE";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<SettlementRuleObj>> GetSettlementRuleTempAsync(int option_itbid, string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_OPTION_ITBID", option_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_SETTLEMENTRULE_TEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SettlementRuleObj> GetSettlementRuleTemp(int option_itbid, string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_OPTION_ITBID", option_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_SETTLEMENTRULE_TEMP";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<RvHeadObj>> GetRvHeadTempAsync(string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_RVHEAD_TEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RvHeadObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<RvHeadObj> GetRvHeadTemp(string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_RVHEAD_TEMP";
            var rec = Fetch(c => c.Query<RvHeadObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public RvHeadObj GetRvHead(int ITBID)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_ITBID", ITBID, DbType.String);
            string qry = "GET_RVHEAD";
            var rec = Fetch(c => c.Query<RvHeadObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public async Task<List<RvDrAcctUpldObj>> GetRvDrAcctTempAsync(string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_RVDRACCT_TEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RvDrAcctUpldObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public RvHeadObj GetRvDrAcct(int ITBID)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_ITBID", ITBID, DbType.String);
            string qry = "proc_GetRvDebitAcct";
            var rec = Fetch(c => c.Query<RvHeadObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public List<ApprovalRouteOffObj> GetRouteOfficerTemp(string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_ROUTEOFFICER_TEMP";
            var rec = Fetch(c => c.Query<ApprovalRouteOffObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public ApprovalRouteOffObj GetRouteOfficer(int ITBID)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_ITBID", ITBID, DbType.String);
            string qry = "GET_ROUTEOFFICER";
            var rec = Fetch(c => c.Query<ApprovalRouteOffObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public List<ApprovalRouteOffObj> GetAppRouteOfficerTemp(string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_ROUTEOFFICER_TEMP";
            var rec = Fetch(c => c.Query<ApprovalRouteOffObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<ApprovalRouteObj> GetAppRouteTemp(int recordId)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_PARTY_ITBID", party_itbid, DbType.Int32);
            p.Add("@P_ITBID", recordId, DbType.String);
            string qry = "GET_ROUTE_TEMP";
            var rec = Fetch(c => c.Query<ApprovalRouteObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<RvGroupObj>> GetRvGroupAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_RVGROUP_LIST";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RvGroupObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<RvGroupObj> GetRvGroup(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_RVGROUP_LIST";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<RvGroupObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<RvHeadObj>> GetRvHead_By_GroupCodeAsync(string group_code, int offset, int rows)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_GROUPCODE", group_code, DbType.String);
            p.Add("@P_OFFSET", offset, DbType.Int32);
            p.Add("@P_ROWS", rows, DbType.Int32);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_RVHEAD_BY_GROUPCODE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RvHeadObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<RvDrAcctObj>> GetRvDrAcct_By_GroupCodeAsync(string group_code)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@GROUPCODE", group_code, DbType.String);
            //p.Add("@P_OFFSET", offset, DbType.Int32);
            //p.Add("@P_ROWS", rows, DbType.Int32);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "proc_GetRvDebitAcct";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RvDrAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<RvGroupObj>> GetRvGroupByMidAsync(string mid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_RVGROUPBYMID";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RvGroupObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public string GetNextRVCode()
        {
            var code = "";
            using (var con = new RepoBase().OpenConnection(null))
            {
                var sql = "GET_NEXTRVCODE";
                code = con.Query<string>(sql, commandType: CommandType.Text).FirstOrDefault();
            }
            return code;
        }
        public async Task<List<InstitutionAcctObj>> GetInstitutionAcctAsync(int inst_itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", inst_itbid, DbType.Int32);
            string qry = "GET_INSTITUTION_ACCT";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<InstitutionAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<InstitutionAcctObj> GetInstitutionAcct(int inst_itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_INST_ITBID", inst_itbid, DbType.Int32);
            string qry = "GET_INSTITUTION_ACCT";
            var rec = Fetch(c => c.Query<InstitutionAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<InstitutionAcctObj>> GetInstitutionAcctTempAsync(int inst_itbid, string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_INST_ITBID", inst_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_INSTITUTION_ACCT_TEMP";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<InstitutionAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<InstitutionAcctObj> GetInstitutionAcctTemp(int inst_itbid, string bid, string user_id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_INST_ITBID", inst_itbid, DbType.Int32);
            p.Add("@P_BATCHID", bid, DbType.String);
            p.Add("@P_USERID", user_id, DbType.String);
            string qry = "GET_INSTITUTION_ACCT_TEMP";
            var rec = Fetch(c => c.Query<InstitutionAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<MCCObj>> GetMCCAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_MCC";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MCCObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MCCObj> GetMCC(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_MCC";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<MCCObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<MccMscObj>> GetAcquirerMCCAsync(string cbn_code, string mcc_code)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_CBNCODE", cbn_code, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            // p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            // p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_ACQUIRER_MCCMSC";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MccMscObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<List<MccMscObj>> GetAcquirerMCC_TempAsync(string batchId, string mcc_code)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);

            string qry = "GET_ACQUIRER_MCCMSC_TEMP";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MccMscObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MccMscObj> GetAcquirerMCC_Temp(string batchId, string mcc_code)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);

            string qry = "GET_ACQUIRER_MCCMSC_TEMP";

            var rec = Fetch(c => c.Query<MccMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<MerchantMscObj>> GetMerchantMSCAsync(string mid, string cbn_code, string mcc_code)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_CBNCODE", cbn_code, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);

            string qry = "GET_MERCHANTMSC";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<List<MerchantMscObj>> GetMerchantMSCTempAsync(string cbn_code, string batchId, string mcc_code, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_CBNCODE", cbn_code, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_MSC_TEMP";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MerchantMscObj> GetMerchantMSC(string mid, string cbn_code, string mcc_code)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_CBNCODE", cbn_code, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);

            string qry = "GET_MERCHANTMSC";

            var rec = Fetch(c => c.Query<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<MerchantMscObj> GetMerchantMSCTemp(string cbn_code, string batchId, string mcc_code, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            //p.Add("@P_CBNCODE", cbn_code, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_MSC_TEMP";
            var rec = Fetch(c => c.Query<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public async Task<List<MerchantUpldObj>> GetMerchantUploadTempAsync(string batchId, string source, string userId, string status)
        {
            DynamicParameters p = new DynamicParameters();
            //p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_UPLOAD_XP";

            if (source == "XP")
            {
                qry = "GET_MERCHANT_UPLOAD_XP";
            }
            else
            {
                qry = "GET_MERCHANT_UPLOAD_BK";
            }

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantUpldObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<List<NapsObj>> GetNapsApprovedAsync(string batchid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@BatchId", batchid, DbType.String);
            string qry = "proc_GetApprovedNapsByBatch";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<NapsObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<NapsObj> GetNapsTemp(string batchId, string userId, string proc_status)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_PROCESS_STATUS", proc_status, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_NAPS_TEMP";
            var rec = Fetch(c => c.Query<NapsObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public List<MerchantUpldObj> GetMerchantUploadTemp(string batchId, string source, string userId, string status)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_UPLOAD_XP";

            if (source == "XP")
            {
                qry = "GET_MERCHANT_UPLOAD_XP";
            }
            else
            {
                qry = "GET_MERCHANT_UPLOAD_BK";
            }

            var rec = Fetch(c => c.Query<MerchantUpldObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<MerchantUpldObj> GetMerchantUpdateUploadTemp(string batchId, string source, string userId, string status)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_UPLOAD_XP";

            if (source == "XP")
            {
                qry = "GET_MERCHANT_UPDATE_UPLOAD_XP";
            }
            else
            {
                qry = "GET_MERCHANT_UPDATE_UPLOAD_BK";
            }

            var rec = Fetch(c => c.Query<MerchantUpldObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<TerminalObj>> GetTerminalByMidAsync(string mid, string tid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_TID", tid, DbType.String);
            string qry = "GET_MERCHANTTERMINAL";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<TerminalObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<List<TerminalObj>> GetTerminalByItbIdAsync(decimal itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            string qry = "GET_TERMINALBYITBID";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<TerminalObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<TerminalObj> GetTerminalByMid(string mid, string tid)
        {
            var p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_TID", tid, DbType.String);
            string qry = "GET_MERCHANTTERMINAL";
            var rec = Fetch(c => c.Query<TerminalObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public async Task<List<MerchantAcctObj>> GetMerchantAcctAsync(string mid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_ISALL", false, DbType.String);
            string qry = "GET_MERCHANT_ACCT";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<DropdownObj> GetMerchantAcct_Merge(string mid, string userId)
        {
            var p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCCHANTACCT_MREG";
            var rec = Fetch(c => c.Query<DropdownObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantAcctObj> GetMerchantAcct(string mid)
        {
            var p = new DynamicParameters();
            p.Add("@P_MID", mid, DbType.String);
            p.Add("@P_ISALL", false, DbType.String);
            string qry = "GET_MERCHANT_ACCT";
            var rec = Fetch(c => c.Query<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public async Task<List<MerchantAcctObj>> GetMerchantAcctTempAsync(string batchid, string userId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_BATCHID", batchid, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_ACCT_TEMP";

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MerchantAcctObj> GetMerchantAcctTemp(string batchid, string userId)
        {
            var p = new DynamicParameters();
            p.Add("@P_BATCHID", batchid, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_ACCT_TEMP";
            var rec = Fetch(c => c.Query<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<TerminalObj> GetMerchantTerminalTemp(string batchid, string userId, int reqType = 1)
        {
            var p = new DynamicParameters();
            p.Add("@P_BATCHID", batchid, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            p.Add("@P_TYPE", reqType, DbType.Int32);
            //p.Add("@P_ISALL", isAll, DbType.Boolean);
            string qry = "GET_MERCHANT_TERM_TEMP";
            var rec = Fetch(c => c.Query<TerminalObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public MerchantObj GetMerchantDetailFromTemp(string batchid, string userId)
        {
            var p = new DynamicParameters();
            p.Add("@P_BATCHID", batchid, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            string qry = "GET_MERCHANT_DETAIL_TEMP";
            var rec = Fetch(c => c.Query<MerchantObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();

        }
        public MerchantAcctObj GetMerchantAcctByItbId(decimal itbid)
        {
            var p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            string qry = "GET_MERCHANTACCT_BY_ITBID";
            var rec = Fetch(c => c.Query<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();

        }
        public async Task<MerchantAcctObj> GetMerchantAcctByItbIdAsync(decimal itbid)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            //p.Add("@P_ISALL", false, DbType.String);
            string qry = "GET_MERCHANTACCT_BY_ITBID";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        public async Task<List<BankTypeObj>> GetBankTypeAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_BANKTYPE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<BankTypeObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<BankTypeObj> GetBankType(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_BANKTYPE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<BankTypeObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<CountryObj>> GetCountryAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_COUNTRY";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<CountryObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<ReportListObj>> GetReportListAsync()
        {

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<ReportListObj>("GET_REPORTLIST", null, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public async Task<List<UserObj>> GetUserLockOutAsync()
        {
            //DynamicParameters p = new DynamicParameters();
            //p.Add("@P_ITBID", itbid, DbType.Int32);
            //p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            //p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            //p.Add("@P_STATUS", status, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<UserObj>("GET_LOCKEDUSER", null, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }

        public List<CountryObj> GetCountry(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_COUNTRY";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<CountryObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<StateObj>> GetStateAsync(int itbid, bool isAll, string status = null, bool isTemp = false, string countryCode = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_COUNTRYCODE", countryCode, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_STATE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<StateObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<StateObj> GetState(int itbid, bool isAll, string status = null, bool isTemp = false, string countryCode = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_COUNTRYCODE", countryCode, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_STATE";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<StateObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<StateObj>> GetStateFilterAsync(string country_code, string status = "Active")
        {
            DynamicParameters p = new DynamicParameters();

            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_COUNTRYCODE", country_code, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_STATEFILTER";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<StateObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<StateObj> GetStateFilter(string country_code, string status = "Active")
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_COUNTRYCODE", country_code, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_STATEFILTER";

            var rec = Fetch(c => c.Query<StateObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<CityObj>> GetCityFilterAsync(string country_code, string state_code, string status = "Active")
        {
            DynamicParameters p = new DynamicParameters();

            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_COUNTRYCODE", country_code, DbType.String);
            p.Add("@P_STATECODE", state_code, DbType.String);
            string qry = "GET_CITYFILTER";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<CityObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<CityObj> GetCityFilter(string country_code, string state_code, string status = "Active")
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_STATUS", status, DbType.String);
            p.Add("@P_COUNTRYCODE", country_code, DbType.String);
            p.Add("@P_STATECODE", state_code, DbType.String);
            string qry = "GET_CITYFILTER";

            var rec = Fetch(c => c.Query<CityObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<InstitutionObj>> GetInstitutionAsync(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_INSTITUTION";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<InstitutionObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<InstitutionObj> GetInstitution(int itbid, bool isAll, string status = null, bool isTemp = false)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISALL", isAll ? 1 : 0, DbType.Int16);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_INSTITUTION";
            // var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            var rec = Fetch(c => c.Query<InstitutionObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public async Task<List<AuthListObj2>> GetAuthList(string deptCode, int roleId, int userinstitution_itbid, string userId, string conString = null)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add("@P_ROLEID", roleId, DbType.Int32);
            p.Add("@P_INSTITUTION_ITBID", userinstitution_itbid, DbType.Int32);
            p.Add("@P_DEPTCODE", deptCode, DbType.String);
            p.Add("@P_USERID", userId, DbType.String);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var qry = "GET_AUTH_LIST";
            // var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();


        }
        public async Task<string> GetMenuById(int menuId)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add("@P_MENUID", menuId, DbType.Int32);

            var qry = "GET_MENU_CONTROLLER";
            // var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<string>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();

        }
        public async Task<RolePrivilegeObj> GetMenuPrivilegeAsync(int menuId, int roleId)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add("@ROLEID", roleId, DbType.Int32);
            p.Add("@MENUID", menuId, DbType.Int32);
            var qry = "proc_GetMenuPrivlege";
            // var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<RolePrivilegeObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();

        }
        public RolePrivilegeObj GetMenuPrivilege(int menuId, int roleId)
        {
            DynamicParameters p = new DynamicParameters();
            // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add("@MENUID", menuId, DbType.Int32);
            p.Add("@ROLEID", roleId, DbType.Int32);
            var qry = "proc_GetMenuPrivlege";
            // var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            var rec = Fetch(c => c.Query<RolePrivilegeObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();

        }
        public AuthObj GetCheckerList(decimal authList_ItbId, int userinstitution_itbid, int menuId = 0, decimal recordId = 0, string conString = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add(":P_MENUID", menuId, DbType.Int32);
            p.Add(":P_RECORDID", recordId, DbType.Decimal);
            p.Add(":P_INSTITUTION_ITBID", userinstitution_itbid, DbType.Int32);
            p.Add(":P_AUTHLIST_ITBID", authList_ItbId, DbType.Decimal);
            // p.Add(":CURSOR_", dbType: DbType.RefCursor, direction: ParameterDirection.Output);
            var obj = new AuthObj();

            string qry = "GET_CHECKER_LIST";
            var rec = Fetch(c => c.Query<AuthListObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            // var rec2 = Fetch(c => c.Query<decimal>(qry2, p, commandType: CommandType.Text), null).FirstOrDefault();
            obj.authListObj = rec.ToList();
            if (obj.authListObj == null)
            {
                obj.authListObj = new List<AuthListObj>();
            }
            // obj.Auth_ITBID = rec2;
            return obj;
        }
        public List<MerchantObj> GetMerchantByMid(string mid, string status, string cbn_code)
        {
            var p = new DynamicParameters();
            p.Add(":P_MID", mid, DbType.String);
            p.Add(":P_STATUS", status, DbType.String);
            p.Add(":P_CBNCODE", cbn_code, DbType.String);

            string qry = @"GET_MERCHANT_BY_MID";
            var rec = Fetch(c => c.Query<MerchantObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public async Task<List<MerchantObj>> GetMerchantByMidAsync(string mid, string status, string cbn_code)
        {
            var p = new DynamicParameters();
            p.Add(":P_MID", mid, DbType.String);
            p.Add(":P_STATUS", status, DbType.String);
            p.Add(":P_CBNCODE", cbn_code, DbType.String);

            string qry = @"GET_MERCHANT_BY_MID";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<MerchantObj> GetMerchantByMidName(string P_Q, string P_LABEL, string cbn_code = null)
        {
            var p = new DynamicParameters();
            p.Add(":P_Q", P_Q, DbType.String);
            p.Add(":P_LABEL", P_LABEL, DbType.String);
            p.Add(":P_CBNCODE", cbn_code, DbType.String);

            string qry = @"GET_MERCHANTBY_NAME_MID";
            var rec = Fetch(c => c.Query<MerchantObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<MDropdownObj> GetMerchantSearchDropDown(string P_Q)
        {
            var p = new DynamicParameters();
            p.Add(":P_Q", P_Q, DbType.String);

            string qry = @"GET_MERCHANTBY_NAME_MID2";
            var rec = Fetch(c => c.Query<MDropdownObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public async Task<List<MDropdownObj>> GetMerchantSearchDropDownAsync(string P_Q)
        {
            var p = new DynamicParameters();
            p.Add(":P_Q", P_Q, DbType.String);
            string qry = @"GET_MERCHANTBY_NAME_MID2";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MDropdownObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<List<MerchantObj>> GetMerchantByMidNameAsync(string P_Q, string P_LABEL, string isDefault = "N", string cbn_code = null)
        {
            var p = new DynamicParameters();
            p.Add(":P_Q", P_Q, DbType.String);
            p.Add(":P_LABEL", P_LABEL, DbType.String);
            p.Add(":P_CBNCODE", cbn_code, DbType.String);
            p.Add(":P_DEFAULT", isDefault, DbType.String);

            string qry = @"GET_MERCHANTBY_NAME_MID";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<MerchantObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<List<NapsObj>> GetNapsEnquiryAsync(DateTime? fromDate = null, DateTime? toDate = null, string batchId = null, int rows = 100, int offset = 0)
        {
            var p = new DynamicParameters();
            p.Add("fromDate", fromDate, DbType.Date);
            p.Add("toDate", toDate, DbType.Date);
            p.Add("batchId", batchId, DbType.String);
            p.Add("offset", offset, DbType.Int32);
            p.Add("rows", rows, DbType.Int32);
            p.Add("option", 1, DbType.Int16);

            string qry = @"proc_GetAllNapsBatch";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<NapsObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public async Task<int> GetNapsEnquiryCountAsync(DateTime? fromDate = null, DateTime? toDate = null, string batchId = null)
        {
            var p = new DynamicParameters();
            p.Add("fromDate", fromDate, DbType.Date);
            p.Add("toDate", toDate, DbType.Date);
            p.Add("batchId", batchId, DbType.String);
            p.Add("offset", null, DbType.Int32);
            p.Add("rows", null, DbType.Int32);
            p.Add("option", 2, DbType.Int16);

            string qry = @"proc_GetAllNapsBatch";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<int>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        #endregion
        #region Reports

        #endregion
        protected static void SetIdentity<T>(IDbConnection connection, Action<T> setId)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id").Single();
            T newId = (T)identity.Id;
            setId(newId);
        }
        public List<Dictionary<string, object>> ToDataTable(IEnumerable<dynamic> items)
        {
            if (items == null) return null;
            var data = items.ToArray();
            if (data.Length == 0) return null;
            var lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dtRow = null;
            var dt = new DataTable();
            //foreach (var pair in ((IDictionary<string, object>)data[0]))
            //{
            //    dt.Columns.Add(pair.Key, (pair.Value ?? string.Empty).GetType());
            //}
            foreach (var d in data)
            {
                // dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
                dtRow = new Dictionary<string, object>();

                foreach (var t in d)
                {
                    //var key = t.Key;
                    if (t.Key == "STAFFID2")
                    {
                        continue;
                    }
                    dtRow.Add(t.Key, t.Value ?? string.Empty);
                    //dtRow.Add(t.Key.Replace("D_", ""), t.Value ?? string.Empty);
                }
                // dtRow.Add()

                lstRows.Add(dtRow);
            }
            // return dt;
            return lstRows;
        }
        public List<Dictionary<string, object>> GetTableRows(DataTable dtData)
        {
            var lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dtRow = null;

            foreach (DataRow dr in dtData.Rows)
            {

                dtRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dtRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dtRow);
            }
            return lstRows;
        }

        public async Task<UserObj2> GetUser2Async(int itbid, bool isTemp = false, string status = null, bool buffered = false, string connectionString = null)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Int32);
            p.Add("@P_ISTEMP", isTemp ? 1 : 0, DbType.Int16);
            p.Add("@P_STATUS", status, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<UserObj2>("proc_GetUserList", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.FirstOrDefault();
        }
        public List<EmailObj> GetMakerEmail(string makerId)
        {
            var p = new DynamicParameters();
            p.Add("@P_MAKERID", makerId, DbType.String);

            string qry = "GET_MAKER_EMAIL";

            var rec = Fetch(c => c.Query<EmailObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<EmailObj> GetApprovers_Mail_Email(decimal authId)
        {
            var p = new DynamicParameters();
            p.Add("@AuthId", authId, DbType.Decimal);

            string qry = "GetApproversEmail";

            var rec = Fetch(c => c.Query<EmailObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<EmailObj> GetAuthorizeEmailList(int MENUID, string DeptCode, int userInstitutionItbid)
        {
            var p = new DynamicParameters();
            p.Add(":P_MENUID", MENUID, DbType.Int32);
            p.Add(":P_DEPTCODE", DeptCode, DbType.String);
            p.Add(":P_INSTITUTIONITBID", userInstitutionItbid, DbType.Int32);

            string qry = "GET_AUTHORIZER_EMAIL";
            var rec = Fetch(c => c.Query<EmailObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<EmailObj> GetApproverIdEmail(string userName)
        {
            var p = new DynamicParameters();
            p.Add("@UserId", userName, DbType.String);
            string qry = "GetApproverIdEmail";
            var rec = Fetch(c => c.Query<EmailObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public SM_INSTITUTION GetInstitutionName(int P_InstItbid)
        {
            var p = new DynamicParameters();
            p.Add(":P_INSTITBID", P_InstItbid, DbType.Int32);
            string qry = @"select institution_name, itbid from SM_INSTITUTION
                           where itbid = :P_INSTITBID";
            var rec = Fetch(c => c.Query<SM_INSTITUTION>(qry, p, buffered: false, commandType: CommandType.Text).FirstOrDefault(), null);

            return rec;
        }
        public List<MailGroupObj> GetGroupEmail(int itbid, bool IsAll, bool IsTemp, string batchId = null)
        {

            var p = new DynamicParameters();
            p.Add(":P_ISALL", IsAll ? 1 : 0, DbType.Int16);
            p.Add(":P_ISTEMP", IsTemp ? 1 : 0, DbType.Int16);
            p.Add(":P_BATCHID", batchId, DbType.String);
            p.Add(":P_ITBID", itbid, DbType.Int32);
            string qry = "GET_MAILGROUP";
            var rec = Fetch(c => c.Query<MailGroupObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public async Task<List<SharingPartyObj>> GetMerchantMsc2DetailAsync(string P_MERCHANTID, string mcc_code)
        {

            var p = new DynamicParameters();
            p.Add("@P_MERCHANTID", P_MERCHANTID, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<SharingPartyObj>("GET_SHAREDMSC2DET_BY_MERCHANT", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<SharingPartyObj> GetMerchantMsc2Detail(string P_MERCHANTID, string mcc_code)
        {

            var p = new DynamicParameters();
            p.Add("@P_MERCHANTID", P_MERCHANTID, DbType.String);
            p.Add("@P_MCC_CODE", mcc_code, DbType.String);

            string qry = "GET_SHAREDMSC2DET_BY_MERCHANT";
            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public async Task<List<DropdownObj>> GetMSC2PartyAcctAsync(decimal itbid, string option)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            p.Add("@P_OPTION", itbid, DbType.String);
            string qry = "GETMSC2PARTYACCT";
            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<DropdownObj>(qry, p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<DropdownObj> GetMSC2PartyAcct(decimal itbid, string option)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            p.Add("@P_OPTION", option, DbType.String);
            string qry = "GETMSC2PARTYACCT";
            var rec = Fetch(c => c.Query<DropdownObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DropdownObj> GetInstitutionParty()
        {

            //var p = new DynamicParameters();

            //string qry = @"select ITBID ||'|I' CODE,INSTITUTION_NAME || ' (INSTITUTION)' DESCRIPTION from POSMISDB_INSTITUTION A
            //               union all
            //               select ITBID ||'|P' CODE,PARTY_DESC || ' (PARTY)' DESCRIPTION from posmisdb_party B
            //               where B.INSTITUTION_BASED<> 'Y' OR B.INSTITUTION_BASED IS NULL";
            var qry = "GET_INSTITUTION_PARTY";
            var rec = Fetch(c => c.Query<DropdownObj>(qry, null, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }

        public InstitutionObj GetINSTITUTION_BY_CBNCODE(string P_CBNCODE)
        {
            var p = new DynamicParameters();
            p.Add(":P_CBNCODE", P_CBNCODE, DbType.String);
            string qry = @"GET_INSTITUTION_BY_CBNCODE";
            var rec = Fetch(c => c.Query<InstitutionObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).FirstOrDefault(), null);

            return rec;
        }
        public LastMidObj GetLastMidTidGenerated(string P_PREFIX, string P_CBNCODE, string P_ID_TYPE)
        {
            var p = new DynamicParameters();
            p.Add(":P_PREFIX", P_PREFIX, DbType.String);
            p.Add(":P_CBNCODE", P_CBNCODE, DbType.String);
            p.Add(":P_ID_TYPE", P_ID_TYPE, DbType.String);
            string qry = "GET_LAST_MIDTID_INST";
            var rec = Fetch(c => c.Query<LastMidObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec.FirstOrDefault();
        }
        public int GetExistedUserNameCount(string userName)
        {
            var p = new DynamicParameters();
            p.Add("@P_UserName", userName, DbType.String);
            //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = string.Format(@"select count(*) ExistCount from ""AspNetUsers""
            //               where Lower(""UserName"") like '%{0}%'", userName.ToLower());
            string qry = "proc_UserExist";
            var rec = Fetch(c => c.Query<int>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.FirstOrDefault();
        }
        public string GetTerminalInstitutionCbnCode(string TERMINALID)
        {
            var p = new DynamicParameters();
            p.Add("@P_TERMINALID", TERMINALID, DbType.String);

            string qry = "GET_TERMINAL_INST_CBNCODE";
            var rec = Fetch(c => c.Query<SM_INSTITUTION>(qry, p, commandType: CommandType.StoredProcedure), null).FirstOrDefault();

            return rec != null ? rec.CBN_CODE : "";
        }
        public List<SM_MERCHANTACCT> GetMerchantAcct(string mid, string acctNo)
        {
            var p = new DynamicParameters();
            p.Add(":P_MERCHANTID", mid, DbType.String);
            p.Add(":P_ACCTNO", acctNo, DbType.String);
            string qry = @"GET_MID_ACCT";
            var rec = Fetch(c => c.Query<SM_MERCHANTACCT>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public async Task<List<DataBinObj>> GetBinAsync(decimal itbid, bool isAll, string cbncode = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            p.Add("@P_ISALL", isAll, DbType.Boolean);
            p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<DataBinObj>("GET_DATABIN", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<DataBinObj> GetBin(decimal itbid, bool isAll, string cbncode = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            p.Add("@P_ISALL", isAll, DbType.Boolean);
            p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = Fetch(c => c.Query<DataBinObj>("GET_DATABIN", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DataBinObj> GetBinTemp(string userId, string batchId)
        {
            var p = new DynamicParameters();
            p.Add("@P_USERID", userId, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            //p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = Fetch(c => c.Query<DataBinObj>("GET_DATABIN_TEMP", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }


        public async Task<List<ExchangeRateObj>> GetExchangeRateAsync(int itbid, bool isAll, string cbncode = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            p.Add("@P_ISALL", isAll, DbType.Boolean);
            p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = await FetchAsync(async c =>
            {
                var gh = await c.QueryAsync<ExchangeRateObj>("GetExchangeRate", p, commandType: CommandType.StoredProcedure);
                return gh;
            });
            return rec.ToList();
        }
        public List<ExchangeRateObj> GetExchangeRate(int itbid, bool isAll, string cbncode = null)
        {
            var p = new DynamicParameters();
            p.Add("@P_ITBID", itbid, DbType.Decimal);
            p.Add("@P_ISALL", isAll, DbType.Boolean);
            p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = Fetch(c => c.Query<ExchangeRateObj>("GetExchangeRate", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<ExchangeRateObj> GetExchangeRateTemp(string userId, string batchId)
        {
            var p = new DynamicParameters();
            p.Add("@P_USERID", userId, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            //p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = Fetch(c => c.Query<ExchangeRateObj>("GetExchangeRateTemp", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public int ValidateExchRate(string cbnCode, string scheme, string bin, int eventType = 1, decimal itbid = 0)
        {
            var p = new DynamicParameters();
            p.Add("@P_CBNCODE", cbnCode, DbType.String, null);
            p.Add("@P_CARDSCHEME", scheme, DbType.String, null);
            p.Add("@P_BIN", bin, DbType.String, null);
            p.Add("@P_EVENTTYPE", eventType, DbType.Int32, null);
            p.Add("@P_ITBID", itbid, DbType.Decimal, null);
            var rec = Fetch(c => c.Query<int>("ValidateExchRate", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }

        public List<RvDrAcctUpldObj> GetRvDrAcctTemp(string userId, string batchId)
        {
            var p = new DynamicParameters();
            p.Add("@P_USERID", userId, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            //p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = Fetch(c => c.Query<RvDrAcctUpldObj>("GET_RVDRACCTBULK_TEMP", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<RvHeadUpldObj> GetRvHeadUploadTemp(string userId, string batchId)
        {
            var p = new DynamicParameters();
            p.Add("@P_USERID", userId, DbType.String);
            p.Add("@P_BATCHID", batchId, DbType.String);
            //p.Add("@P_CBNCODE", cbncode, DbType.String);

            var rec = Fetch(c => c.Query<RvHeadUpldObj>("GET_RVHEADBULK_TEMP", p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public int ValidateBin(string cbnCode, string scheme, string bin, int eventType = 1, decimal itbid = 0)
        {
            var p = new DynamicParameters();
            p.Add("@P_CBNCODE", cbnCode, DbType.String, null);
            p.Add("@P_CARDSCHEME", scheme, DbType.String, null);
            p.Add("@P_BIN", bin, DbType.String, null);
            p.Add("@P_EVENTTYPE", eventType, DbType.Int32, null);
            p.Add("@P_ITBID", itbid, DbType.Decimal, null);
            var rec = Fetch(c => c.Query<int>("VALIDATE_BIN", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public int ValidateRvHead(string rvCode, string groupCode)
        {
            var p = new DynamicParameters();
            p.Add("@P_RVCODE", rvCode, DbType.String, null);
            p.Add("@P_GROUPCODE", groupCode, DbType.String, null);
            //p.Add("@P_BIN", bin, DbType.String, null);
            //p.Add("@P_EVENTTYPE", eventType, DbType.Int32, null);
            //p.Add("@P_ITBID", itbid, DbType.Decimal, null);
            var rec = Fetch(c => c.Query<int>("VALIDATE_RVHEAD", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public OutPutObj ValidateIfCanDeleteMAcct(decimal itbid)
        {
            var p = new DynamicParameters();
            p.Add("@ITBID", itbid, DbType.Decimal, null);
            var rec = Fetch(c => c.Query<OutPutObj>("PROC_VALIDATEDELETEDACCT", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }

        public int PostSetAdjustmentEntry(SM_NAPS_NIBSS_TEMP d)
        {
            var p = new DynamicParameters();
            p.Add("@DEBITBANKCODE", d.DEBITBANKCODE, DbType.String, null);
            p.Add("@DEBITACCTNO", d.DEBITACCTNO, DbType.String, null);
            p.Add("@CREDITAMOUNT", d.CREDITAMOUNT, DbType.Decimal, null);
            p.Add("@CREDITAMOUNT_OLD", d.CREDITAMOUNT_OLD, DbType.Decimal, null);
            p.Add("@SETTLEMENTDATETIME", d.SETTLEMENTDATE, DbType.DateTime, null);
            p.Add("@MERCHANTID", d.MERCHANTID, DbType.String, null);
            p.Add("@ADJUSTMENTREASON", d.REASON, DbType.String, null);
            var rec = Fetch(c => c.Query<int>("PROC_NAPS_ADJUSTMENTENTRY", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
        public int UpdateSettlementReportFlag(SM_NAPS_NIBSS_TEMP d)
        {
            var p = new DynamicParameters();
            p.Add("@SETTLEMENTDATETIME", d.SETTLEMENTDATE, DbType.String, null);
            p.Add("@DEBITBANKCODE", d.DEBITBANKCODE, DbType.String, null);
            p.Add("@DEBITACCTNO", d.DEBITACCTNO, DbType.String, null);
            p.Add("@MERCHANTID", d.MERCHANTID, DbType.String, null);
            var rec = Fetch(c => c.Query<int>("PROC_SETTLEMENT_REPORTFLAG", p, commandType: CommandType.StoredProcedure), null);
            return rec.FirstOrDefault();
        }
    }
}
