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
    public class MerchantUploadSession
    {
        public int PostMerchantUpload(MerchantUpldObj obj,int postType,string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("@PID", obj.PID, DbType.String);
            p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
            p.Add("@MERCHANTNAME", obj.MERCHANTNAME, DbType.String);
            p.Add("@CONTACTTITLE", obj.CONTACTTITLE, DbType.String);
            p.Add("@CONTACTNAME", obj.CONTACTNAME, DbType.String);
            p.Add("@MOBILEPHONE", obj.MOBILEPHONE, DbType.String);
            p.Add("@EMAIL", obj.EMAIL, DbType.String);
            p.Add("@EMAILALERTS", obj.EMAILALERTS, DbType.String);
            p.Add("@PHYSICALADDR", obj.PHYSICALADDR, DbType.String);
            p.Add("@TERMINALMODELCODE", obj.TERMINALMODELCODE, DbType.String);
            p.Add("@TERMINALID", obj.TERMINALID, DbType.String);
            p.Add("@BANKCODE", obj.BANKCODE, DbType.String);
            p.Add("@BANKACCNO", obj.BANKACCNO, DbType.String);
            p.Add("@BANKTYPE", obj.BANKTYPE, DbType.Int32);
            p.Add("@SLIPHEADER", obj.SLIPHEADER, DbType.String);
            p.Add("@SLIPFOOTER", obj.SLIPFOOTER, DbType.String);
            p.Add("@BUISNESSOCCUPATIONCODE", obj.BUISNESSOCCUPATIONCODE, DbType.String);
            p.Add("@MERCHANTCATEGORYCODE", obj.MERCHANTCATEGORYCODE, DbType.String);
            p.Add("@STATECODE", obj.STATECODE, DbType.String);
            p.Add("@VISAACQUIRERID", obj.VISAACQUIRERID, DbType.String);
            p.Add("@VERVEACQUIRERID", obj.VERVEACQUIRERID, DbType.String);
            p.Add("@MASTERCARDACQUIRERID", obj.MASTERCARDACQUIRERID, DbType.String);
            p.Add("@TERMINALOWNERCODE", obj.TERMINALOWNERCODE, DbType.String);
            p.Add("@LGA_LCDA", obj.LGA_LCDA, DbType.String);
            p.Add("@BANK_URL", obj.BANK_URL, DbType.String);
            p.Add("@ACCOUNTNAME", obj.ACCOUNTNAME, DbType.String);
            p.Add("@PTSP", obj.PTSP, DbType.String);
            p.Add("@BATCHID", null, DbType.String);
            p.Add("@USERID", userId, DbType.String);
            p.Add("@CREATEDATE", DateTime.Now, DbType.DateTime);
            p.Add("@INTERFACE_FORMAT", null, DbType.String);
            p.Add("@SERVICETYPE", null, DbType.String);
            p.Add("@GROUPLABEL", null, DbType.Int32);
            p.Add("@ROWCOLOR", null, DbType.String);
            p.Add("@PAYATTITUDE_ACCEPTANCE", null, DbType.String);
            p.Add("@TRANSCURRENCY", obj.TRANSCURRENCY, DbType.String);
            p.Add("@PTSA", obj.PTSA, DbType.String);
            p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
            p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
            p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
            p.Add("POSTTYPE", postType, DbType.Int16);


            using (var con = new RepoBase().OpenConnection(null))
            {
                var rst = con.Execute("SESS_POST_MERCHANTUPLD", p, commandType:CommandType.StoredProcedure);
                return rst;
            }
        }
        public int PostMerchantUploadBulk(List<MerchantUpldObj> objList,string userId)
        {
            var curDate = DateTime.Now;
            //OutPutObj ret = new OutPutObj();
            var cnt = 0;
            var p = new DynamicParameters();
            using (var con = new RepoBase().OpenConnection(null))
            {
                int? bankType = null;
                int? termCode = null;
                int bkType;
                //PURGE ALL RECORD FOR CURRENT USER
                int sqn = 0;
                p.Add("@USERID", userId, DbType.String);
                var rst = con.Execute("SESS_PURGE_MERCHANTUPLOAD", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {
                    if(int.TryParse(obj.BANKTYPE,out bkType)){
                        bankType = bkType;
                    }
                    if (int.TryParse(obj.TERMINALMODELCODE, out bkType))
                    {
                        termCode = bkType;
                    }
                    p.Add("@PID", obj.PID, DbType.String);
                    p.Add("@MERCHANTID", obj.MERCHANTID, DbType.String);
                    p.Add("@MERCHANTNAME", obj.MERCHANTNAME, DbType.String);
                    p.Add("@CONTACTTITLE", obj.CONTACTTITLE, DbType.String);
                    p.Add("@CONTACTNAME", obj.CONTACTNAME, DbType.String);
                    p.Add("@MOBILEPHONE", obj.MOBILEPHONE, DbType.String);
                    p.Add("@EMAIL", obj.EMAIL, DbType.String);
                    p.Add("@EMAILALERTS", obj.EMAILALERTS, DbType.String);
                    p.Add("@PHYSICALADDR", obj.PHYSICALADDR, DbType.String);
                    p.Add("@TERMINALMODELCODE", termCode, DbType.Decimal);
                    p.Add("@TERMINALID", obj.TERMINALID, DbType.String);
                    p.Add("@BANKCODE", obj.BANKCODE, DbType.String);
                    p.Add("@BANKACCNO", obj.BANKACCNO, DbType.String);
                    p.Add("@BANKTYPE", bankType, DbType.Int32);
                    p.Add("@SLIPHEADER", obj.SLIPHEADER, DbType.String);
                    p.Add("@SLIPFOOTER", obj.SLIPFOOTER, DbType.String);
                    p.Add("@BUISNESSOCCUPATIONCODE", obj.BUISNESSOCCUPATIONCODE, DbType.String);
                    p.Add("@MERCHANTCATEGORYCODE", obj.MERCHANTCATEGORYCODE, DbType.String);
                    p.Add("@STATECODE", obj.STATECODE, DbType.String);
                    p.Add("@VISAACQUIRERID", obj.VISAACQUIRERID, DbType.String);
                    p.Add("@VERVEACQUIRERID", obj.VERVEACQUIRERID, DbType.String);
                    p.Add("@MASTERCARDACQUIRERID", obj.MASTERCARDACQUIRERID, DbType.String);
                    p.Add("@TERMINALOWNERCODE", obj.TERMINALOWNERCODE, DbType.String);
                    p.Add("@LGA_LCDA", obj.LGA_LCDA, DbType.String);
                    p.Add("@BANK_URL", obj.BANK_URL, DbType.String);
                    p.Add("@ACCOUNTNAME", obj.ACCOUNTNAME, DbType.String);
                    p.Add("@PTSP", obj.PTSP, DbType.String);
                    p.Add("@BATCHID", null, DbType.String);
                    p.Add("@USERID", userId, DbType.String);
                    p.Add("@CREATEDATE", curDate, DbType.DateTime);
                    p.Add("@INTERFACE_FORMAT", null, DbType.String);
                    p.Add("@SERVICETYPE", null, DbType.String);
                    p.Add("@GROUPLABEL", obj.GROUPLABEL, DbType.Int32);
                    p.Add("@ROWCOLOR", obj.ROWCOLOR, DbType.String);
                    p.Add("@PAYATTITUDE_ACCEPTANCE", null, DbType.String);
                    p.Add("@TRANSCURRENCY", obj.TRANSCURRENCY, DbType.String);
                    p.Add("@PTSA", obj.PTSA, DbType.String);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("@POSTTYPE", 1, DbType.Int16);
                    p.Add("@PostSequence", sqn++, DbType.Int32);
                    

                    cnt += con.Execute("SESS_POST_MERCHANTUPLD", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public List<MerchantUpldObj> GetMerchantUpload(string userId)
        {
           // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<MerchantUpldObj>("SESS_GET_MERCHANTUPLOAD", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public int DeleteMerchantUpload(string Id,string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("DELETE_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
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
        public int PurgeRevenueHead(string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Execute("PURGE_REVENUEHEAD_SESSION", p, commandType: CommandType.StoredProcedure);
                return ret;
            }

        }
    }
}
