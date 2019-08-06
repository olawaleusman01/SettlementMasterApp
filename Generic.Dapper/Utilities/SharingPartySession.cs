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
    public class SharingPartySession
    {
        public OutPutObj PostSharingParty(SharingPartyObj obj, int postType)
        {
            OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("PID", obj.PID, DbType.String);
            p.Add("BILLER_CODE", obj.BILLER_CODE, DbType.String);
            p.Add("PARTYITBID", obj.PARTYITBID, DbType.Int32);
            p.Add("BILLERMSC_ITBID", obj.BILLERMSC_ITBID, DbType.Decimal);

            p.Add("PARTY_LOCATOR", obj.PARTY_LOCATOR, DbType.String);
            p.Add("SHARINGVALUE", obj.SHARINGVALUE, DbType.Decimal);
            p.Add("CAP", obj.CAP, DbType.Decimal);
            p.Add("ACCOUNT_ID", obj.ACCOUNT_ID, DbType.Decimal);
            p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("PARTYTYPE_CODE", obj.PARTYTYPE_CODE, DbType.String);

            p.Add("DB_ITBID", obj.DB_ITBID, DbType.Decimal);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);

            using (var con = new RepoBase().OpenConnection(null))
            {
                ret = con.Query<OutPutObj>("SESS_POST_SHARINGPARTY", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public List<SharingPartyObj> GetSharingParty(string userId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SharingPartyObj>("SESS_GET_SHARINGPARTY", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public int DeleteSharingParty(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_DELETE_SHARINGPARTY", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
        public SharingPartyObj FindSharingParty(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<SharingPartyObj>("SESS_FIND_SHARINGPARTY", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }

        }
        public int PurgeSharingParty(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("SESS_PURGE_SHARINGPARTY", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
