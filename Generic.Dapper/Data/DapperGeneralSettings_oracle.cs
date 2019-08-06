using System.Data;
using Dapper;
using UPPosMaster.Dapper.Repository;
using System.Collections.Generic;
using UPPosMaster.Dapper.Model;
using System.Linq;
using UPPosMaster.Data;
using UPPosMaster.Dapper.Utility;
using UPPosMaster.Data.Model;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using System;

namespace UPPosMaster.Dapper.Data
{
    public class DapperGeneralSettings : RepoBase,  IDapperGeneralSettings
    {

        // object of direct call
        private readonly IDapperRepository<POSMISDB_ROLES> _repoRole = new DapperRepository<POSMISDB_ROLES>();
        private readonly IDapperRepository<POSMISDB_DEPARTMENT  > _repoDept = new DapperRepository<POSMISDB_DEPARTMENT>();
        //// object of procedure call
        private readonly IDapperRepository<object> _repoConfigProc = new DapperRepository<object>();

        #region DapperCallViaDirectTable
        public List<POSMISDB_ROLES> GetRoleList(string conString)
        {
            //p.Add("@roleId", roleId, DbType.Int32, null, 100);
            //// p.Add("@bankId", bankId, DbType.Int32, null, 100);
           var rec = _repoRole.GetAll(null).ToList();
           // var gh = DatasetHelper.ToDataSet<bankconfig>(rec);
            return rec;
        }
        public int PostLoginAudit(LoginAuditObj obj , int postType)
        {
            var inserted = 0;
           
                var p = new OracleDynamicParameters();
                p.Add(":P_LOGINDATE", obj.LOGINDATE, OracleDbType.Date);
                p.Add(":P_LOGOUTDATE", obj.LOGOUTDATE, OracleDbType.Date);
                p.Add(":P_USERID", obj.UserId, OracleDbType.Varchar2);
            p.Add(":P_BROWSER", obj.BROWSER, OracleDbType.Varchar2);
            p.Add(":P_IPADDRESS", obj.IPADDRESS, OracleDbType.Varchar2);
            p.Add(":P_MAC", obj.MAC, OracleDbType.Varchar2);
            p.Add(":P_GUID", obj.guid, OracleDbType.Varchar2);
            //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            if (postType == 1)
            {
                string sql = @"INSERT
                            INTO POSMISDB_LOGINAUDIT
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
                                :P_USERID,
                                :P_LOGINDATE,
                                :P_LOGOUTDATE,
                                :P_IPADDRESS,
                                :P_MAC,
                                :P_BROWSER,
                                :P_GUID
                              )";
                inserted = Execute(c => c.Execute(sql, p
                        ), null);

            }
            else
            {
                 p = new OracleDynamicParameters();
                p.Add(":P_LOGOUTDATE", obj.LOGOUTDATE, OracleDbType.Date);
                p.Add(":P_GUID", obj.guid, OracleDbType.Varchar2);
                string sql = @"UPDATE POSMISDB_LOGINAUDIT
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

            var p = new OracleDynamicParameters();
           // p.Add(":P_LOGINDATE", obj.LOGINDATE, OracleDbType.Date);
            p.Add(":P_ATTEMPTDATE", obj.ATTEMPTDATE, OracleDbType.Date);
            p.Add(":P_USERID", obj.UserId, OracleDbType.Varchar2);
            p.Add(":P_BROWSER", obj.BROWSER, OracleDbType.Varchar2);
            p.Add(":P_IPADDRESS", obj.IPADDRESS, OracleDbType.Varchar2);
            p.Add(":P_MAC", obj.MAC, OracleDbType.Varchar2);
            p.Add(":P_GUID", obj.guid, OracleDbType.Varchar2);
            //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
       
                string sql = @"INSERT
                                INTO POSMISDB_LOGINATTEMPT
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
                                    :P_USERID,
                                    :P_ATTEMPTDATE,
                                    :P_IPADDRESS,
                                    :P_MAC,
                                    :P_BROWSER,
                                    :P_GUID
                                  )";
                inserted += Execute(c => c.Execute(sql, p
                        ), null);

          
            return inserted;
        }
        public int UPDATE_TEXTMESS(List<textmess_obj> objList)
        {
            var inserted = 0;
            foreach (var obj in objList)
            {
                var p = new OracleDynamicParameters();
                p.Add(":v_docno", obj.DOCNO, OracleDbType.Varchar2);
                p.Add(":v_merchantid", obj.MERCHANTID, OracleDbType.Varchar2);
                p.Add(":v_textmexx", obj.NEWTEXMESS, OracleDbType.Varchar2);
                p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                string sql = @"update posmisdb_wrongtextmess set status='S',texmess= :v_textmexx
                               where DOCNO= :v_docno and merchantid= :v_merchantid";
                inserted += Execute(c => c.Execute(sql, p
                        ), null);

            }
            return inserted;
        }
        public List<textmess_obj> GetTextMessException(string set_Date)
        {
            var p = new OracleDynamicParameters();
            p.Add(":V_SETDATE", set_Date, OracleDbType.Varchar2);
            // p.Add(":P_LABEL", P_LABEL, OracleDbType.Varchar2);
            //p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"select * from posmisdb_wrongtextmess where settlementdate=:V_SETDATE
                           AND (status <> 'S' or status is null)";


            var rec = Fetch(c => c.Query<textmess_obj>(qry, p, buffered: false, commandType: CommandType.Text).ToList(), null);

            return rec;
        }
        public List<POSMISDB_DEPARTMENT> GetDeptList(string conString = null)
        {
            //p.Add("@roleId", roleId, DbType.Int32, null, 100);
            //// p.Add("@bankId", bankId, DbType.Int32, null, 100);
            var rec = _repoDept.GetAll(null).ToList();
            // var gh = DatasetHelper.ToDataSet<bankconfig>(rec);
            return rec;
        }
        public int CloseRecord(decimal itbid ,string conString = null)
        {
             string sql =string.Format( @"UPDATE POSMISDB_AUTHLIST SET STATUS = 'Close' WHERE ITBID = {0}",itbid);
            var inserted = Execute(c => c.Execute(sql, null
                     ), conString);

            return inserted;
        }

        public int PostCheckerRecord(POSMISDB_AUTHCHECKER chk, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_USERID", chk.USERID, OracleDbType.Varchar2);
            p.Add(":P_NARRATION", chk.NARRATION, OracleDbType.Varchar2);
            p.Add(":P_STATUS", chk.STATUS, OracleDbType.Varchar2);
            p.Add(":P_AUTHLIST_ITBID", chk.AUTHLIST_ITBID, OracleDbType.Decimal);
          //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string sql = string.Format( @"INSERT
            //                INTO POSMISDB_AUTHCHECKER
            //                  (
            //                    USERID,
            //                    NARRATION,
                              
            //                    STATUS,
            //                    AUTHLIST_ITBID
            //                  )
            //                  VALUES
            //                  (
            //                   '{0}',
            //                   '{1}',
            //                   '{2}',
            //                   {3}
                              
            //                  )",chk.USERID,chk.NARRATION,chk.STATUS,chk.AUTHLIST_ITBID);
            string sql = @"INSERT
                            INTO POSMISDB_AUTHCHECKER
                              (
                                USERID,
                                NARRATION,
                              
                                STATUS,
                                AUTHLIST_ITBID
                              )
                              VALUES
                              (
                               :P_USERID,
                               :P_NARRATION,
                               :P_STATUS,
                               :P_AUTHLIST_ITBID
                              
                              )";
         
            var inserted = Execute(c => c.Execute(sql, p
                     ), conString);


            return inserted;
        }
        public int PostmEBuPLOADED(string UID, string fileName ,DateTime createdate)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_USERID", UID, OracleDbType.Varchar2);
            p.Add(":P_FILENAME", fileName, OracleDbType.Varchar2);
            p.Add(":P_CREATEDDATE", createdate, OracleDbType.Date);
          //  p.Add(":P_AUTHLIST_ITBID", chk.AUTHLIST_ITBID, OracleDbType.Decimal);
            //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string sql = string.Format( @"INSERT
            //                INTO POSMISDB_AUTHCHECKER
            //                  (
            //                    USERID,
            //                    NARRATION,

            //                    STATUS,
            //                    AUTHLIST_ITBID
            //                  )
            //                  VALUES
            //                  (
            //                   '{0}',
            //                   '{1}',
            //                   '{2}',
            //                   {3}

            //                  )",chk.USERID,chk.NARRATION,chk.STATUS,chk.AUTHLIST_ITBID);
            string sql = @"INSERT
INTO POSMISDB_MEBFILELOADED
  (
    FILENAME,
    USERID,
    CREATEDDATE
  )
  VALUES
  (    
    :P_FILENAME,
    :P_USERID,
    :P_CREATEDDATE
  )";

            var inserted = Execute(c => c.Execute(sql, p
                     ), null);


            return inserted;
        }
        public AuthObj GetCheckerList(int menuId , decimal authList_ItbId, decimal recordId, int userinstitution_itbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MENUID", menuId, OracleDbType.Int32);
            p.Add(":P_RECORDID", recordId, OracleDbType.Decimal);
            p.Add(":P_INSTITUTION_ITBID", userinstitution_itbid, OracleDbType.Long);
            p.Add(":P_AUTHLIST_ITBID", authList_ItbId, OracleDbType.Decimal);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            var obj = new AuthObj();
            //string qry = @"select A.ITBID,A.MENUID,A.RECORDID,A.EVENTTYPE,A.CREATEDDATE MAKERDATE,A.STATUS RECORDSTATUS,A. USERID MAKERID,
            //                      B.ITBID CKECKERITBID,B.USERID CHECKERUSERID,B.NARRATION,B.CREATEDDATE CHECKERDATE,B.STATUS CHECKERSTATUS
            //                      from (SELECT ITBID, MENUID, RECORDID, EVENTTYPE,INSTITUTION_ITBID, CREATEDDATE, STATUS, USERID FROM POSMISDB_AUTHLIST) A
            //                    inner join 
            //                    (SELECT ITBID,  USERID,  NARRATION,  CREATEDDATE,  STATUS,  AUTHLIST_ITBID
            //                    FROM POSMISDB_AUTHCHECKER) B
            //                    on A.ITBID = B.AUTHLIST_ITBID
            //                    where A.ITBID = :AUTHLIST_ITBID AND
            //                    A.RECORDID = :RECORDID and A.MENUID = :MENUID  and A.INSTITUTION_ITBID = :INSTITUTION_ITBID and Lower(A.STATUS) = 'open'";
            //string qry2 = @"SELECT ITBID FROM POSMISDB_AUTHLIST WHERE ITBID = :AUTHLIST_ITBID AND 
            //                    RECORDID = :RECORDID and MENUID = :MENUID  and INSTITUTION_ITBID = :INSTITUTION_ITBID and STATUS = 'OPEN'";
            string qry = "GET_CHECKER_LIST";
            var rec = Fetch(c => c.Query<AuthListObj>(qry, p, commandType: CommandType.StoredProcedure), null);
           // var rec2 = Fetch(c => c.Query<decimal>(qry2, p, commandType: CommandType.Text), null).FirstOrDefault();
            obj.authListObj = rec.ToList();
            if(obj.authListObj == null)
            {
                obj.authListObj = new List<AuthListObj>();
            }
           // obj.Auth_ITBID = rec2;
            return obj;
        }
        public CompanyProfileObj GetCompanyProfile(string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT ITBID,
            //               COMPANY_CODE,
            //               COMPANY_NAME,
            //               COMPANY_EMAIL,
            //               COMPANY_WEBSITE,
            //               COMPANY_PHONE1,
            //               COMPANY_PHONE2,
            //               COMPANY_ADDRESS,
            //               PASSWORD_CHANGE_DAYS,
            //               PASSWORDLENGTH,
            //               SYSTEM_IDLE_TIMEOUT,
            //               LOCKOUT_TRIAL_COUNT,
            //               USERID,
            //               DATECREATED,
            //               LAST_MODIFIED_UID
            //               FROM POSMISDB_COMPANY_PROFILE";
            string qry = "GET_COMPANY_PROFILE";

            var rec = Fetch(c => c.Query<CompanyProfileObj>(qry,  p, buffered: false, commandType: CommandType.StoredProcedure).FirstOrDefault(), null);

            return rec;

        }
        public string GetUploadTranxCurrencyCode(string bid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", bid, OracleDbType.Varchar2);
         
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
           // var obj = new AuthObj();
         
            string qry = "GET_UPLOADCURRENCY";
            var rec = Fetch(c => c.Query<string>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).FirstOrDefault(), null);
          
            return rec;
        }

        public List<RevenueHeadObj> GetRevenueByMidList(string mid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MID", mid, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // var obj = new AuthObj();

            string qry = "GET_REVENUE_BY_MID";
            var rec = Fetch(c => c.Query<RevenueHeadObj>(qry, p, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }

        public List<MerchantObj> GetMerchantMerge(decimal? custId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_CUSTID", custId, OracleDbType.Decimal);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // var obj = new AuthObj();

            string qry = "GET_MERCHANTMERGE_BY_PARENTID";
            var rec = Fetch(c => c.Query<MerchantObj>(qry, p, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        
        public List<TerminalSingleObj> GetTerminalByItbid(long itbid, bool isTemp, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Decimal);
            p.Add(":P_ISTEMP", isTemp ? 0 : 1, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            var obj = new AuthObj();

            string qry = "GET_TERMINAL_SINGLE";
            var rec = Fetch(c => c.Query<TerminalSingleObj>(qry, p, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<TerminalSingleObj> GetTerminalTemp(string batchId,string userId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
           // p.Add(":P_TID", tid, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":P_USERID", userId , OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            var obj = new AuthObj();

            string qry = "GET_TERMINAL_TEMP";
            var rec = Fetch(c => c.Query<TerminalSingleObj>(qry, p, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<RevenueHeadObj> GetRevenueHeadByItbid(long itbid, bool isTemp, string batchid = null, bool isBatch = false, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int64);
            p.Add(":P_ISTEMP", isTemp ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_ISBATCH", isBatch ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_BATCHID", batchid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            var obj = new AuthObj();

            string qry = "GET_REVENUEHEAD_LIST";
            var rec = Fetch(c => c.Query<RevenueHeadObj>(qry, p, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<MenuConfig> MenuConfigList(string conString=null)
        {
                        string qry = @"select a.MENUID,a.MENUNAME,b.NO_OF_CHECKER,b.ITBID CITBID from MENUCONTROL a
                                        left outer join ADMIN_MENU_CHECKER_CONFIG b
                                        on a.MENUID = b.MENUID where a.PARENT is not NULL";
           
            var rec = Fetch(c => c.Query<MenuConfig>(qry, null, commandType: CommandType.Text), null);
            return rec.ToList();

        }
        public List<MebFileUploadObj> GetSettlementStatusByCPDDATE(DateTime? cpdDate,string p_Option )
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_DATE", cpdDate, OracleDbType.Date);
            p.Add(":P_OPTION", p_Option, OracleDbType.Varchar2);
            //   p.Add(":P_BATCHID", BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_SETTLEMENTSTATUS";

            var rec = Fetch(c => c.Query<MebFileUploadObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MccMscObj> GetCardSchemeMccMsc(string MCC_CODE ,string institution_id,string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", MCC_CODE, OracleDbType.Varchar2 );
            p.Add(":P_CBNCODE", institution_id, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT  A.*,CARDSCHEME_DESC,C.CURRENCY_NAME AS DomCurrencyDesc,D.CURRENCY_NAME AS IntCurrencyDesc,
            //                E.FREQUENCY_DESC AS DomFrequencyDesc,F.FREQUENCY_DESC AS IntFrequencyDesc,CAST(0 AS NUMBER(1,0)) AS NEWRECORD
            //                from
            //                (select Z.* from POSMISDB_MCCMSC Z where Z.MCC_CODE = :MCC_CODE
            //                AND INSTITUTION_ID = :INSTITUTION_ID) A
            //                inner join 
            //                (select CARDSCHEME,CARDSCHEME_DESC from POSMISDB_CARDSCHEME) B
            //                on A.CARDSCHEME = B.CARDSCHEME
            //                LEFT OUTER JOIN 
            //                (select CURRENCY_CODE,CURRENCY_NAME from POSMISDB_CURRENCY) C
            //                on A.DOM_SETTLEMENT_CURRENCY = C.CURRENCY_CODE
            //                LEFT OUTER JOIN 
            //                (select CURRENCY_CODE,CURRENCY_NAME from POSMISDB_CURRENCY) D
            //                on A.INT_SETTLEMENT_CURRENCY = D.CURRENCY_CODE
            //                LEFT OUTER JOIN 
            //                (select ITBID,FREQUENCY_DESC from POSMISDB_FREQUENCY) E
            //                on A.DOM_FREQUENCY = E.ITBID
            //                LEFT OUTER JOIN 
            //                (select ITBID,FREQUENCY_DESC from POSMISDB_FREQUENCY) F
            //                on A.INT_FREQUENCY = F.ITBID";
           // string qry = "GET_CARDSCHEME_MCCMSC"; // with no aquirer scheme
            string qry = "GET_CARDSCHEME_MCCMSC2";
            var rec = Fetch(c => c.Query<MccMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
      
        public List<MccMscObj> GetCardSchemeMccMscTemp(string MCC_CODE,string BATCHID,string userId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", MCC_CODE, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", BATCHID, OracleDbType.Varchar2);
            p.Add(":P_USERID", userId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT  A.*,CARDSCHEME_DESC,C.CURRENCY_NAME AS DomCurrencyDesc,D.CURRENCY_NAME AS IntCurrencyDesc,
            //                E.FREQUENCY_DESC AS DomFrequencyDesc,F.FREQUENCY_DESC AS IntFrequencyDesc,CAST(0 AS NUMBER(1,0)) AS NEWRECORD
            //                from
            //                (select Z.* from POSMISDB_MCCMSCTEMP Z 
            //                 where Z.MCC_CODE = :MCC_CODE AND Z.BATCHID = :BATCHID) A
            //                inner join 
            //                (select CARDSCHEME,CARDSCHEME_DESC from POSMISDB_CARDSCHEME) B
            //                on A.CARDSCHEME = B.CARDSCHEME
            //                LEFT OUTER JOIN 
            //                (select CURRENCY_CODE,CURRENCY_NAME from POSMISDB_CURRENCY) C
            //                on A.DOM_SETTLEMENT_CURRENCY = C.CURRENCY_CODE
            //                LEFT OUTER JOIN 
            //                (select CURRENCY_CODE,CURRENCY_NAME from POSMISDB_CURRENCY) D
            //                on A.INT_SETTLEMENT_CURRENCY = D.CURRENCY_CODE
            //                LEFT OUTER JOIN 
            //                (select ITBID,FREQUENCY_DESC from POSMISDB_FREQUENCY) E
            //                on A.DOM_FREQUENCY = E.ITBID
            //                LEFT OUTER JOIN 
            //                (select ITBID,FREQUENCY_DESC from POSMISDB_FREQUENCY) F
            //                on A.INT_FREQUENCY = F.ITBID";
            string qry = "GET_CARDSCHEME_MCCMSC_TEMP2";
            var rec = Fetch(c => c.Query<MccMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }

        public List<MerchantMscObj> GetMerchantMsc(string MCC_CODE,string MERCHANTID, string INSTITUTION_ID = "501", string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", MCC_CODE, OracleDbType.Varchar2);
            p.Add(":P_MERCHANTID", MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_CBNCODE", INSTITUTION_ID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select AD.*,NVL(BD.DOM_MSCSUBSIDY,0) DOM_MSCSUBSIDY,
            //                NVL(BD.DOM_MSCCONCESSION,0) DOM_MSCCONCESSION,NVL(BD.INT_MSCSUBSIDY,0) INT_MSCSUBSIDY,
            //                NVL(BD.INT_MSCCONCESSION,0) INT_MSCCONCESSION,NVL(BD.MCCMSC_ITBID,0) MCCMSC_ITBID,
            //                NVL(BD.ITBID,0) MERCHANTMSC_ITBID,CD.CARDSCHEME_DESC,NVL(BD.MERCHANTDOMCAP,0) MERCHANTDOMCAP
            //                , NVL(BD.MERCHANTINTLCAP,0) MERCHANTINTLCAP from 
            //                (select A.* from POSMISDB_MCCMSC A
            //                WHERE A.MCC_CODE = :MCC_CODE) AD
            //                INNER JOIN 
            //                (SELECT C.CARDSCHEME,C.CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME C)CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME
            //                LEFT OUTER JOIN 
            //                (SELECT B.MERCHANTID,B.DOM_MSCSUBSIDY,B.DOM_MSCCONCESSION,B.INT_MSCSUBSIDY,
            //                B.INT_MSCCONCESSION,B.MERCHANTDOMCAP,B.MERCHANTINTLCAP,B.MCCMSC_ITBID ,ITBID
            //                FROM POSMISDB_MERCHANTMSC B
            //                WHERE B.MERCHANTID = :MERCHANTID) BD
            //                ON AD.ITBID = BD.MCCMSC_ITBID";
            //string qry = @"  select  AD.ITBID, AD.MCC_CODE, AD.MSC_CALCBASIS, AD.CARDSCHEME, BATCHID,
            //                NVL(BD.DOM_MSCSUBSIDY,0) DOM_MSCSUBSIDY,NVL(BD.INT_MSCSUBSIDY,0) INT_MSCSUBSIDY,
            //                NVL(BD.MCCMSC_ITBID,0) as  MCCMSC_ITBID, NVL(decode(BD.DOM_MSCCONCESSION,null,AD.DOM_MSCVALUE,BD.DOM_MSCCONCESSION),0) as DOM_MSCVALUE,
            //               NVL(decode(BD.INT_MSCCONCESSION,null,AD.INT_MSCVALUE,BD.INT_MSCCONCESSION),0) as INT_MSCVALUE, NVL(BD.ITBID,0) MERCHANTMSC_ITBID,
            //               CD.CARDSCHEME_DESC,NVL(decode(merchantintlcap,null,intlcap,merchantintlcap),0)  INTLCAP 
            //                , NVL(decode(merchantdomcap,null,domcap,merchantdomcap),0)  DOMCAP from 
            //                (select ITBID, MCC_CODE, MSC_CALCBASIS, CARDSCHEME,  NVL(DOM_MSCVALUE,0) DOM_MSCVALUE ,NVL(INT_MSCVALUE,0) INT_MSCVALUE, 
            //                DOM_SETTLEMENT_CURRENCY, INT_SETTLEMENT_CURRENCY,
            //                   BATCHID,   DOM_FREQUENCY,  INT_FREQUENCY,NVL(DOMCAP,0) DOMCAP  ,NVL(INTLCAP,0)  INTLCAP from POSMISDB_MCCMSC A
            //                WHERE A.MCC_CODE = :MCC_CODE AND  A.ITBID NOT IN (
            //                SELECT MCCMSC_ITBID 
            //                FROM POSMISDB_MERCHANTMSC WHERE MERCHANTID = '2057ED000000878')) AD
            //                INNER JOIN 
            //                (SELECT C.CARDSCHEME,C.CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME C)CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME                             
            //                LEFT OUTER JOIN 
            //                (SELECT B.MERCHANTID,B.DOM_MSCSUBSIDY,NVL(B.DOM_MSCCONCESSION,0) AS DOM_MSCCONCESSION,B.INT_MSCSUBSIDY,
            //               NVL(B.INT_MSCCONCESSION,0) AS INT_MSCCONCESSION ,B.MERCHANTDOMCAP,B.MERCHANTINTLCAP,B.MCCMSC_ITBID ,ITBID
            //                FROM POSMISDB_MERCHANTMSC B
            //                WHERE B.MERCHANTID = '2057ED000000878') BD
            //                ON AD.ITBID = BD.MCCMSC_ITBID;";
            //string qry = @"select  AD.ITBID,AD.INSTITUTION_ID, AD.MCC_CODE, AD.MSC_CALCBASIS, AD.CARDSCHEME,BATCHID,
            //                NVL(BD.DOM_MSCSUBSIDY,0) DOM_MSCSUBSIDY,NVL(BD.INT_MSCSUBSIDY,0) INT_MSCSUBSIDY,
            //                NVL(BD.MCCMSC_ITBID,0) as  MCCMSC_ITBID, NVL(BD.DOM_MSCCONCESSION,0) as DOM_MSCCONCESSION,
            //               NVL(BD.INT_MSCCONCESSION,0) as INT_MSCCONCESSION, NVL(BD.ITBID,0) MERCHANTMSC_ITBID, NVL(DOM_MSCVALUE,0) DOM_MSCVALUE
            //               ,NVL(INT_MSCVALUE,0) INT_MSCVALUE,
            //               CD.CARDSCHEME_DESC,NVL(decode(merchantintlcap,null,intlcap,merchantintlcap),0)  INTLCAP 
            //                , NVL(decode(merchantdomcap,null,domcap,merchantdomcap),0)  DOMCAP,NVL(BD.MCC_VARIANCE,0) as MCC_VARIANCE,
            //                NVL(BD.INTMCC_VARIANCE,0) as INTMCC_VARIANCE
            //                from 
            //                (select ITBID,INSTITUTION_ID, MCC_CODE, MSC_CALCBASIS, CARDSCHEME,  NVL(DOM_MSCVALUE,0) DOM_MSCVALUE ,NVL(INT_MSCVALUE,0) INT_MSCVALUE, 
            //                DOM_SETTLEMENT_CURRENCY, INT_SETTLEMENT_CURRENCY
            //                   BATCHID,   DOM_FREQUENCY,  INT_FREQUENCY,NVL(DOMCAP,0) DOMCAP  ,NVL(INTLCAP,0)  INTLCAP from POSMISDB_MCCMSC A
            //                WHERE A.MCC_CODE = :MCC_CODE AND A.INSTITUTION_ID = :INSTITUTION_ID) AD
            //                INNER JOIN 
            //                (SELECT C.CARDSCHEME,C.CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME C)CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME                             
            //                LEFT OUTER JOIN 
            //                (SELECT B.MERCHANTID,B.DOM_MSCSUBSIDY,NVL(B.DOM_MSCCONCESSION,0) AS DOM_MSCCONCESSION,B.INT_MSCSUBSIDY,
            //               NVL(B.INT_MSCCONCESSION,0) AS INT_MSCCONCESSION ,B.MERCHANTDOMCAP,B.MERCHANTINTLCAP,B.MCCMSC_ITBID ,ITBID,MCC_VARIANCE,INTMCC_VARIANCE
            //                FROM POSMISDB_MERCHANTMSC B
            //                WHERE B.MERCHANTID = :MERCHANTID) BD
            //                ON AD.ITBID = BD.MCCMSC_ITBID";
            //string qry = "GET_MERCHANT_MSC";
            string qry = "GET_MERCHANT_MSC2";
            var rec = Fetch(c => c.Query<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }

        public List<MerchantMscObj> GetMerchantMscByItbId(long P_ITBID, string MERCHANTID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", P_ITBID, OracleDbType.Int64);
            p.Add(":P_MERCHANTID", MERCHANTID, OracleDbType.Varchar2);
            //p.Add(":P_INSTITUTION_ID", INSTITUTION_ID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
           
            string qry = "GET_MERCHANT_MSC_BY_ITBID";
            var rec = Fetch(c => c.Query<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<MerchantMscObj> GetMerchantMscFromTemp(string batchId, string MCC_CODE, string cbn_code ,string USERiD,  string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add(":MERCHANTID", MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_MCC_CODE", MCC_CODE, OracleDbType.Varchar2);
            p.Add(":P_CBNCODE", cbn_code, OracleDbType.Varchar2);
            p.Add(":P_USERID", USERiD, OracleDbType.Varchar2);
            //string qry = @"select AD.ITBID,AD.INSTITUTION_ID, AD.MCC_CODE, AD.MSC_CALCBASIS, AD.CARDSCHEME,BATCHID,
            //                NVL(BD.DOM_MSCSUBSIDY, 0) DOM_MSCSUBSIDY,NVL(BD.INT_MSCSUBSIDY, 0) INT_MSCSUBSIDY,
            //                NVL(BD.MCCMSC_ITBID, 0) as MCCMSC_ITBID, NVL(BD.DOM_MSCCONCESSION, 0) as DOM_MSCCONCESSION,
            //               NVL(BD.INT_MSCCONCESSION, 0) as INT_MSCCONCESSION, NVL(BD.ITBID, 0) MERCHANTMSC_ITBID, NVL(DOM_MSCVALUE, 0) DOM_MSCVALUE
            //               ,NVL(INT_MSCVALUE, 0) INT_MSCVALUE,
            //               CD.CARDSCHEME_DESC,NVL(decode(merchantintlcap, null, intlcap, merchantintlcap), 0)  INTLCAP
            //                , NVL(decode(merchantdomcap, null, domcap, merchantdomcap), 0)  DOMCAP,NVL(BD.MCC_VARIANCE, 0) as MCC_VARIANCE,
            //                NVL(BD.INTMCC_VARIANCE, 0) as INTMCC_VARIANCE,BD.MAKER_ID,BD.DATECREATED,BD.MERCHANT_STATUS from 
            //                (select A.* from POSMISDB_MCCMSC A
            //                 ) AD
            //                INNER JOIN 
            //                (SELECT C.CARDSCHEME,C.CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME C)CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME
            //                INNER JOIN 
            //                (SELECT B.MERCHANTID,B.DOM_MSCSUBSIDY,B.DOM_MSCCONCESSION,B.INT_MSCSUBSIDY,
            //                B.INT_MSCCONCESSION,B.MERCHANTDOMCAP,B.MERCHANTINTLCAP,B.MCCMSC_ITBID ,ITBID,MCC_VARIANCE,INTMCC_VARIANCE
            //                ,B.USERID AS MAKER_ID,B.CREATEDATE AS DATECREATED,STATUS AS MERCHANT_STATUS
            //                FROM POSMISDB_MERCHANTMSCTEMP B 
            //                WHERE B.BATCHID = :BATCHID AND (LOWER(B.STATUS) = 'open' OR LOWER(B.STATUS) = 'rejected')) BD
            //                ON AD.ITBID = BD.MCCMSC_ITBID";
            string qry = "GET_MERCHANT_MSC_TEMP3";
            var rec = Fetch(c => c.Query<MerchantMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<BillerMscObj> GetBillerMsc(string biller_code, string MERCHANTID, string INSTITUTION_ID = "501", string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BILLER_CODE", biller_code, OracleDbType.Varchar2);
            p.Add(":P_MERCHANTID", MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_CBNCODE", INSTITUTION_ID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
           
            string qry = "GET_BILLER_MSC";
            var rec = Fetch(c => c.Query<BillerMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<BillerMscObj> GetBillerMscFromTemp(string batchId, string biller_code, string cbn_code, string USERiD, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add(":MERCHANTID", MERCHANTID, OracleDbType.Varchar2);
           // p.Add(":P_BILLER_CODE", biller_code, OracleDbType.Varchar2);
           // p.Add(":P_CBNCODE", cbn_code, OracleDbType.Varchar2);
            p.Add(":P_USERID", USERiD, OracleDbType.Varchar2);
      
            string qry = "GET_BILLER_MSC_TEMP";
            var rec = Fetch(c => c.Query<BillerMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<TerminalMscObj> GetTerminalMsc(string MCC_CODE, string TERMINALID, string INSTITUTION_ID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", MCC_CODE, OracleDbType.Varchar2);
            p.Add(":P_TERMINALID", TERMINALID, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTION_ID", INSTITUTION_ID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select  AD.ITBID,AD.INSTITUTION_ID, AD.MCC_CODE, AD.MSC_CALCBASIS, AD.CARDSCHEME,BATCHID,
            //                NVL(BD.DOM_MSCSUBSIDY,0) DOM_MSCSUBSIDY,NVL(BD.INT_MSCSUBSIDY,0) INT_MSCSUBSIDY,
            //                NVL(BD.MCCMSC_ITBID,0) as  MCCMSC_ITBID, NVL(BD.DOM_MSCCONCESSION,0) as DOM_MSCCONCESSION,
            //               NVL(BD.INT_MSCCONCESSION,0) as INT_MSCCONCESSION, NVL(BD.ITBID,0) MERCHANTMSC_ITBID, NVL(DOM_MSCVALUE,0) DOM_MSCVALUE
            //               ,NVL(INT_MSCVALUE,0) INT_MSCVALUE,
            //               CD.CARDSCHEME_DESC,NVL(decode(TERMINALINTLCAP,null,intlcap,TERMINALINTLCAP),0)  INTLCAP 
            //                , NVL(decode(TERMINALDOMCAP,null,domcap,TERMINALDOMCAP),0)  DOMCAP,NVL(BD.MCC_VARIANCE,0) as MCC_VARIANCE,
            //                NVL(BD.INTMCC_VARIANCE,0) as INTMCC_VARIANCE
            //                from 
            //                (select ITBID,INSTITUTION_ID, MCC_CODE, MSC_CALCBASIS, CARDSCHEME,  NVL(DOM_MSCVALUE,0) DOM_MSCVALUE ,NVL(INT_MSCVALUE,0) INT_MSCVALUE, 
            //                DOM_SETTLEMENT_CURRENCY, INT_SETTLEMENT_CURRENCY
            //                   BATCHID,   DOM_FREQUENCY,  INT_FREQUENCY,NVL(DOMCAP,0) DOMCAP  ,NVL(INTLCAP,0)  INTLCAP from POSMISDB_MCCMSC A
            //                WHERE A.MCC_CODE = :MCC_CODE AND A.INSTITUTION_ID = :INSTITUTION_ID) AD
            //                INNER JOIN 
            //                (SELECT C.CARDSCHEME,C.CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME C)CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME                             
            //                LEFT OUTER JOIN 
            //                (SELECT B.TERMINALID,B.DOM_MSCSUBSIDY,NVL(B.DOM_MSCCONCESSION,0) AS DOM_MSCCONCESSION,B.INT_MSCSUBSIDY,
            //               NVL(B.INT_MSCCONCESSION,0) AS INT_MSCCONCESSION ,B.TERMINALDOMCAP,B.TERMINALINTLCAP,B.MCCMSC_ITBID ,ITBID,MCC_VARIANCE,INTMCC_VARIANCE
            //                FROM POSMISDB_TERMINALMSC B
            //                WHERE B.TERMINALID = :TERMINALID) BD
            //                ON AD.ITBID = BD.MCCMSC_ITBID";

            string qry = "GET_TERMINAL_MSC";
            var rec = Fetch(c => c.Query<TerminalMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<TerminalMscObj> GetGetTerminalMscFromTemp(string batchId, string TERMINALID, string MCC_CODE, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
           // p.Add("CURSOR_", batchId, OracleDbType.RefCursor,direction:ParameterDirection.Output);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add(":MERCHANTID", MERCHANTID, OracleDbType.Varchar2);
            //p.Add(":MCC_CODE", MCC_CODE, OracleDbType.Varchar2);
            //p.Add(":INSTITUTION_ID", INSTITUTION_ID, OracleDbType.Varchar2);
            //string qry = @"select AD.ITBID,AD.INSTITUTION_ID, AD.MCC_CODE, AD.MSC_CALCBASIS, AD.CARDSCHEME,BATCHID,
            //                NVL(BD.DOM_MSCSUBSIDY, 0) DOM_MSCSUBSIDY,NVL(BD.INT_MSCSUBSIDY, 0) INT_MSCSUBSIDY,
            //                NVL(BD.MCCMSC_ITBID, 0) as MCCMSC_ITBID, NVL(BD.DOM_MSCCONCESSION, 0) as DOM_MSCCONCESSION,
            //               NVL(BD.INT_MSCCONCESSION, 0) as INT_MSCCONCESSION, NVL(BD.ITBID, 0) MERCHANTMSC_ITBID, NVL(DOM_MSCVALUE, 0) DOM_MSCVALUE
            //               ,NVL(INT_MSCVALUE, 0) INT_MSCVALUE,
            //               CD.CARDSCHEME_DESC,NVL(decode(merchantintlcap, null, intlcap, merchantintlcap), 0)  INTLCAP
            //                , NVL(decode(merchantdomcap, null, domcap, merchantdomcap), 0)  DOMCAP,NVL(BD.MCC_VARIANCE, 0) as MCC_VARIANCE,
            //                NVL(BD.INTMCC_VARIANCE, 0) as INTMCC_VARIANCE,BD.MAKER_ID,BD.DATECREATED,BD.TERMINAL_STATUS from 
            //                (select A.* from POSMISDB_MCCMSC A
            //                 ) AD
            //                INNER JOIN 
            //                (SELECT C.CARDSCHEME,C.CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME C)CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME
            //                INNER JOIN 
            //                (SELECT B.TERMINALID,B.DOM_MSCSUBSIDY,B.DOM_MSCCONCESSION,B.INT_MSCSUBSIDY,
            //                B.INT_MSCCONCESSION,B.MERCHANTDOMCAP,B.MERCHANTINTLCAP,B.MCCMSC_ITBID ,ITBID,MCC_VARIANCE,INTMCC_VARIANCE
            //                ,B.USERID AS MAKER_ID,B.CREATEDATE AS DATECREATED,STATUS AS TERMINAL_STATUS
            //                FROM POSMISDB_TERMINALMSCTEMP B 
            //                WHERE B.BATCHID = BATCHID AND LOWER(B.STATUS) = 'open') BD
            //                ON AD.ITBID = BD.MCCMSC_ITBID";
         string   qry = "GET_TERMINAL_MSC_FROM_TEMP";

            var rec = Fetch(c => c.Query<TerminalMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<TerminalRuleObj> GetTerminalRuleFromTemp(string BATCHID)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            // p.Add(":CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":INSTITUTION_ID", Institution_Id, OracleDbType.Varchar2);
            // p.Add(":MERCHANTID", merchantId, OracleDbType.Varchar2);
            //string qry = @" SELECT AD.*,BD.CARDSCHEME_DESC,
            //                CD.TERMINALID,cast(NVL(CD.PARTY_ITBID,0) as number(9)) PARTY_ITBID
            //                ,NVL(CD.PARTY_CONCESSION,0) PARTY_CONCESSION,NVL(CD.PARTY_SUBSIDY,0) PARTY_SUBSIDY,CD.PARTY_DESC,
            //                FD.PARTYTYPE_DESC,NVL( TERMINALRULE_ITBID,0) MERCHANTRULE_ITBID,MERCHANT_STATUS ,CD. MAKER_ID, CD.DATECREATED  FROM
            //                (select A.* from posmisdb_MCCrule A                            
            //                ) AD
            //                INNER JOIN 
            //                (SELECT F.PARTYTYPE_CODE,F.PARTYTYPE_DESC  FROM POSMISDB_PARTYTYPE F) FD
            //                ON AD.PARTYTYPE_CODE = FD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT B.CARDSCHEME,B.CARDSCHEME_DESC  FROM POSMISDB_CARDSCHEME B) BD
            //                ON AD.CARDSCHEME = BD.CARDSCHEME
            //                inner JOIN 
            //                (SELECT C.TERMINALID,MCCRULE_ITBID,PARTY_ITBID,C.PARTY_CONCESSION,C.PARTY_SUBSIDY,PARTY_DESC, C.ITBID TERMINALRULE_ITBID
            //                ,C.STATUS AS MERCHANT_STATUS,C.USERID AS MAKER_ID,C.CREATEDATE AS DATECREATED
            //                 FROM POSMISDB_TERMINALRULETEMP C
            //                left outer JOIN POSMISDB_PARTY R 
            //                ON C.PARTY_ITBID = R.ITBID
            //                WHERE C.BATCHID = P_BATCHID AND LOWER(C.STATUS) = 'open') CD
            //                ON AD.ITBID = CD.MCCRULE_ITBID";
          string  qry = "GET_TERMINAL_RULE_FROM_TEMP";
            var rec = Fetch(c => c.Query<TerminalRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }

        public List<TerminalRuleObj> GetTerminalRule(string Mcc_Code, string CardScheme, string Institution_Id, string terminalId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_MCC_CODE", Mcc_Code, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTION_ID", Institution_Id, OracleDbType.Varchar2);
            p.Add(":P_TERMINALID", terminalId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.*,BD.CARDSCHEME_DESC,
            //                CD.TERMINALID,cast(NVL(CD.PARTY_ITBID,0) as number(9)) PARTY_ITBID,NVL(CD.PARTY_CONCESSION,0) PARTY_CONCESSION
            //                ,NVL(CD.PARTY_SUBSIDY,0) PARTY_SUBSIDY,CD.PARTY_DESC,
            //                FD.PARTYTYPE_DESC,NVL( MERCHANTRULE_ITBID,0) MERCHANTRULE_ITBID FROM
            //                (select A.* from posmisdb_MCCrule A 
            //                where A.MCC_CODE = :MCC_CODE AND A.INSTITUTION_ID = :INSTITUTION_ID AND A.CARDSCHEME = :CARDSCHEME  
            //                ) AD
            //                INNER JOIN 
            //                (SELECT F.PARTYTYPE_CODE,F.PARTYTYPE_DESC  FROM POSMISDB_PARTYTYPE F) FD
            //                ON AD.PARTYTYPE_CODE = FD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT B.CARDSCHEME,B.CARDSCHEME_DESC  FROM POSMISDB_CARDSCHEME B) BD
            //                ON AD.CARDSCHEME = BD.CARDSCHEME
            //                LEFT OUTER JOIN 
            //                (SELECT C.TERMINALID,MCCRULE_ITBID,PARTY_ITBID,C.PARTY_CONCESSION,C.PARTY_SUBSIDY,PARTY_DESC, C.ITBID MERCHANTRULE_ITBID
            //                 FROM POSMISDB_TERMINALRULE C
            //                LEFT OUTER JOIN POSMISDB_PARTY R
            //                ON C.PARTY_ITBID = R.ITBID
            //                WHERE C.TERMINALID = :TERMINALID) CD
            //                ON AD.ITBID = CD.MCCRULE_ITBID
            //            ";
            string qry = "GET_TERMINAL_RULE";
            var rec = Fetch(c => c.Query<TerminalRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }

        public List<TerminalObj> GetTerminalList(string tid,string conString = null)
        {
             OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_TID", tid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select A.*,INSTITUTION_NAME,MERCHANTNAME,MCC_CODE,MCC_DESC from 
            //                (select a.* from POSMISDB_TERMINAL a
            //                where rownum <= 300) A
            //                INNER JOIN 
            //                (select MERCHANTID,MERCHANTNAME,aa.mcc_code,bb.mcc_desc from POSMISDB_MERCHANTDETAIL aa
            //                left outer join POSMISDB_MCC bb
            //                ON aa.MCC_CODE = BB.MCC_CODE
            //                ) C
            //                ON A.MERCHANTID = C.MERCHANTID
            //                INNER JOIN 
            //                (select ITBID,INSTITUTION_NAME from POSMISDB_INSTITUTION) B
            //                ON A.INSTITUTION_ID = B.ITBID";
            string qry = "GET_TERMINAL_LIST";
            var rec = Fetch(c => c.Query<TerminalObj>(qry, p, commandType: CommandType.StoredProcedure), null);
             return rec.ToList();

        }

        public List<TerminalObj> GetTerminalByMidList(string MID,string TID = null, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MID", MID, OracleDbType.Varchar2);
            p.Add(":P_TID", TID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            string qry = "GET_MERCHANTTERMINAL2";
            var rec = Fetch(c => c.Query<TerminalObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantObj2> GetMerchantList(string mid,short? IsAll)
        {
             OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", mid, OracleDbType.Varchar2);
            p.Add(":P_ISALL", IsAll, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MERCHANTDETAIL_GRID";
            
            var rec = Fetch(c => c.Query<MerchantObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantDeleteObj> GetMidTidDeletetion(string fiel_value, string field_label)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_FIELD_VAL", fiel_value, OracleDbType.Varchar2);
            p.Add(":P_FIELD_LABEL", field_label, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MIDTID_DELETE";

            var rec = Fetch(c => c.Query<MerchantDeleteObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantDeleteObj> GetMidTidDeletetionFromTemp(string location, string batchId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_LOCATION", location, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MIDTID_DELETETEMP";
           // List<MerchantDeleteObj> up = new List<MerchantDeleteObj>();
            var rec = Fetch(c => c.Query<MerchantDeleteObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
           // return up;
        }
        public List<MerchantUpldObj2> GetMerchantUpldFromUpQueue(string batchid,string conString = null)
        {
             OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", batchid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add(":TERMINALID", TERMINALID, OracleDbType.Varchar2);
            //string qry = @"select A.*,INSTITUTION_NAME from 
            //                (select A.* from POSMISDB_UPMERTERMUPLDREC A) A
            //                INNER JOIN 
            //                (select CBN_CODE,INSTITUTION_NAME from POSMISDB_INSTITUTION) B
            //                ON A.BANKCODE = B.CBN_CODE
            //                WHERE A.BATCHID = :BATCHID";
            string qry = "GET_MERCHANT_UPLD_FROM_UP_QUE";
            var rec = Fetch(c => c.Query<MerchantUpldObj2>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<MerchantGloObj> GetInstitutionUpldList(int InstitutionItbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", InstitutionItbid, OracleDbType.Decimal);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT A.*,B.FULLNAME FROM
                            
            //                (SELECT a.* FROM posmisdb_merterupldglo a
            //                WHERE INSTITUTION_ID = :P_INSTITUTION_ID
            //                ORDER by a.CREATEDDATE DESC) A
            //                 INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" FULLNAME FROM ""AspNetUsers"") B
            //                  on Lower(A.USERID) = Lower(B.""UserName"")
                           // ";
            string qry = "GET_INSTIITUTION_UPLD_LIST";
            var rec = Fetch(c => c.Query<MerchantGloObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<MerchantGloObj> GetInstitutionUpldUpdateList(int InstitutionItbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", InstitutionItbid, OracleDbType.Decimal);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT A.*, B.FULLNAME FROM
            //              (SELECT a.* FROM posmisdb_merterupdupldglo a
            //             WHERE INSTITUTION_ID = :P_INSTITUTION_ID
            //             ORDER by a.CREATEDDATE DESC) A
            //              INNER JOIN
            //              (SELECT ""UserName"", ""FullName"" FULLNAME FROM ""AspNetUsers"") B
            //               on Lower(A.USERID) = Lower(B.""UserName"")
            //                ";
            string qry = "GET_INSTIITUTION_UPLD_UPDATE";
            var rec = Fetch(c => c.Query<MerchantGloObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
      
        public List<MerchantGloObj> GetInstitutionUpldListToProcess(int InstitutionItbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":InstitutionId", InstitutionItbid, OracleDbType.Decimal);
            //string qry = @" SELECT A.BATCHCOUNT,A.BATCHID,A.USERID,B.INST_MAKER_NAME,C.INST_CHECKER_NAME,STATUS
            //                ,INSTITUTION_ID,INSTITUTION_NAME FROM
            //                (
            //                (SELECT COUNT(ITBID) BATCHCOUNT, BATCHID, USERID,INST_AUTH_ID,STATUS,INSTITUTION_ID FROM POSMISDB_UPMERTERMUPLDREC

            //                group by BATCHID, USERID,STATUS,INSTITUTION_ID,INST_AUTH_ID) A
            //                  INNER JOIN
            //                 (SELECT ITBID , INSTITUTION_NAME FROM POSMISDB_INSTITUTION) D
            //                  on A.INSTITUTION_ID = D.ITBID
            //                 INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" AS INST_MAKER_NAME FROM ""AspNetUsers"") B
            //                  on A.USERID = B.""UserName""
            //                )
            //                INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" AS INST_CHECKER_NAME FROM ""AspNetUsers"") C
            //                  on A.INST_AUTH_ID = C.""UserName"" ";
            //string qry = @" SELECT A.*,B.INST_MAKER_NAME,C.INST_CHECKER_NAME
            //                ,INSTITUTION_NAME FROM
                            
            //                (SELECT a.* FROM POSMISDB_UPMER_UPLDGLO a
            //                order by Institution_Id, a.createddate desc) A
            //                  INNER JOIN
            //                 (SELECT ITBID , INSTITUTION_NAME FROM POSMISDB_INSTITUTION) D
            //                  on A.INSTITUTION_ID = D.ITBID
            //                 INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" AS INST_MAKER_NAME FROM ""AspNetUsers"") B
            //                  on Lower(A.USERID) = Lower(B.""UserName"")

            //                LEFT OUTER JOIN
            //                 (SELECT ""UserName"", ""FullName"" AS INST_CHECKER_NAME FROM ""AspNetUsers"") C
            //                  on LOWER(A.INST_AUTH_ID) = Lower(C.""UserName"")";
            string qry = "GET_INST_UPLD_TO_PROCESS";
            var rec = Fetch(c => c.Query<MerchantGloObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }

        public List<MerchantGloObj> GetInstitutionUpdateUpldListToProcess(int InstitutionItbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add(":InstitutionId", InstitutionItbid, OracleDbType.Decimal);
            //string qry = @"SELECT A.BATCHCOUNT,A.BATCHID,A.USERID,B.INST_MAKER_NAME,C.INST_CHECKER_NAME,STATUS
            //                ,INSTITUTION_ITBID INSTITUTION_ID,INSTITUTION_NAME FROM
            //                (
            //                (SELECT COUNT(ITBID) BATCHCOUNT, BATCHID, USERID,INST_AUTH_ID,STATUS,INSTITUTION_ITBID FROM POSMISDB_UPMERCHANTUPDATE

            //                group by BATCHID, USERID,STATUS,INSTITUTION_ITBID,INST_AUTH_ID) A
            //                  INNER JOIN
            //                 (SELECT ITBID , INSTITUTION_NAME FROM POSMISDB_INSTITUTION) D
            //                  on A.INSTITUTION_ITBID = D.ITBID
            //                 INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" AS INST_MAKER_NAME FROM ""AspNetUsers"") B
            //                  on A.USERID = B.""UserName""
            //                )
            //                INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" AS INST_CHECKER_NAME FROM ""AspNetUsers"") C
            //                  on A.INST_AUTH_ID = C.""UserName"" ";

            string qry = "GET_INST_UPDATEUPLD_TO_PROCESS";
            var rec = Fetch(c => c.Query<MerchantGloObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantGloObj> GetInstitutionUpdateList(int InstitutionItbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", InstitutionItbid, OracleDbType.Decimal);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"  SELECT A.BATCHCOUNT,A.BATCHID,A.USERID,B.""FullName"",STATUS FROM
            //                (
            //                (SELECT COUNT(ITBID) BATCHCOUNT, BATCHID, USERID,STATUS FROM POSMISDB_MERCHANTUPDATE
            //                WHERE INSTITUTION_ITBID = :InstitutionId
            //                group by BATCHID, USERID,STATUS) A
            //                 INNER JOIN
            //                 (SELECT ""UserName"", ""FullName"" FROM ""AspNetUsers"") B
            //                  on A.USERID = B.""UserName"")";
            string qry = "GET_INST_UPDATE_LIST";
            var rec = Fetch(c => c.Query<MerchantGloObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }

        public List<POSMISDB_MERCHANTTERMINALUPLD> GetBankUploadDetail(string InstitutionId,string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":InstitutionId", InstitutionId, OracleDbType.Decimal);
            p.Add(":BatchId", BatchId, OracleDbType.Varchar2);
            string qry = @"  select * from POSMISDB_MERCHANTTERMINALUPLD 
                             where BATCHID = :BatchId AND INSTITUTION_ID = :InstitutionId";

            var rec = Fetch(c => c.Query<POSMISDB_MERCHANTTERMINALUPLD>(qry, p, commandType: CommandType.Text), null);
            return rec.ToList();

        }
        public List<RuleEngineObj> GetRulePerScheme(string CardScheme,string MerchantId,string TerminalId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":CardScheme", CardScheme, OracleDbType.Varchar2);
            p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            p.Add(":TerminalId", MerchantId, OracleDbType.Varchar2);
            StringBuilder qry = new StringBuilder();
            qry.Append( @"SELECT A.*,B.CARDSCHEME_DESC,C.PARTY_DESC,C.PARTYTYPE_CODE,PARTYTYPE_DESC FROM 
                            (
                            (select ITBID,MERCHANTID,TERMINALID,CARDSCHEME,INDICATOR_ID,PARTY_CODE,PARTY_VALUE,USERID,STATUS from POSMISDB_RULEENGINE) A
                            inner join
                            (SELECT CARDSCHEME,CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME) B
                            on A.CARDSCHEME = B.CARDSCHEME
                            inner join 
                            (SELECT PARTY_CODE,PARTY_DESC,PARTYTYPE_CODE FROM POSMISDB_PARTY) C
                            on A.PARTY_CODE = C.PARTY_CODE
                            INNER JOIN 
                            (SELECT PARTYTYPE_CODE,PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE) E 
                            ON C.PARTYTYPE_CODE = E.PARTYTYPE_CODE
                            )
                            where A.CARDSCHEME = :CardScheme AND A.MERCHANTID = :MerchantId ");
            if (!string.IsNullOrEmpty(TerminalId))
            {
                qry.AppendLine("AND A.TERMINALID = :TerminalId");
            }
                            


            var rec = Fetch(c => c.Query<RuleEngineObj>(qry.ToString(), p, commandType: CommandType.Text), null);

            return rec.ToList();

        }
        public List<InstitutionAcctObj> GetInstitutionAcct(long institutionId,  string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", institutionId, OracleDbType.Int64);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
            //qry = (@"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDRESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.LAST_MODIFIED_UID,
            //              AD.ITBID,AD.INSTITUTION_ID,   CAST(AD.INSTITUTIONTYPE AS NUMBER(9,0)) INSTITUTIONTYPE, AD.CARDSCHEME,
            //              AD.INSTITUTIONITBID,BD.INSTITUTION_NAME,CD.CARDSCHEME_DESC AS CardSchemDesc,PARTYTYPE_DESC AS InstitutionTypeDesc from
            //            (
            //            (select A.* from POSMISDB_INSTITUTIONACCT A) AD
            //            inner join 
            //            (select ITBID,INSTITUTION_NAME from POSMISDB_INSTITUTION )BD
            //            ON AD.INSTITUTIONITBID = BD.ITBID
            //            LEFT OUTER JOIN 
            //            (SELECT CARDSCHEME,CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME) CD
            //            ON AD.CARDSCHEME = CD.CARDSCHEME
            //            LEFT OUTER JOIN 
            //            (SELECT ITBID,PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE) DD
            //            ON AD.INSTITUTIONTYPE = DD.ITBID
            //            )
            //            where AD.INSTITUTIONITBID = :InstitutionId");

            qry = "GET_INSTITUTION_ACCT";


            var rec = Fetch(c => c.Query<InstitutionAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<InstitutionAcctObj> GetInstitutionAcct2(long ITBID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", ITBID, OracleDbType.Int64);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
      

            qry = "GET_INSTITUTION_ACCT_2";

            
            var rec = Fetch(c => c.Query<InstitutionAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<InstitutionAcctObj> GetIntitutionAcctTemp(long institutionId, string batchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", institutionId, OracleDbType.Int64);
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry;
            //qry = (@"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.USERID,AD.RECORDID,
            //              AD.ITBID,AD.INSTITUTION_ID,   CAST(AD.INSTITUTIONTYPE AS NUMBER(9,0)) INSTITUTIONTYPE, AD.CARDSCHEME,
            //              AD.INSTITUTIONITBID,CD.CARDSCHEME_DESC AS CardSchemDesc,PARTYTYPE_DESC AS InstitutionTypeDesc from
            //            (
            //            (select A.* from POSMISDB_INSTITUTIONACCTTEMP A) AD
                       
            //            LEFT OUTER JOIN 
            //            (SELECT CARDSCHEME,CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME) CD
            //            ON AD.CARDSCHEME = CD.CARDSCHEME
            //            LEFT OUTER JOIN 
            //            (SELECT ITBID,PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE) DD
            //            ON AD.INSTITUTIONTYPE = DD.ITBID
            //            )
            //            where AD.INSTITUTIONITBID = :InstitutionId AND AD.BATCHID = :BATCHID AND lower(AD.STATUS) = 'open'");


            qry = "GET_INSTITUTION_ACCT_TEMP";

            var rec = Fetch(c => c.Query<InstitutionAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }

        public List<PartyAcctObj> GetPartyAcct(long PARTY_ITBID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_PARTY_ITBID", PARTY_ITBID, OracleDbType.Int64);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
            //qry = (@"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.LAST_MODIFIED_UID,
            //              AD.ITBID,  AD.CARDSCHEME,
            //              AD.PARTY_ITBID,CD.CARDSCHEME_DESC AS CardSchemDesc from
            //            (
            //            (select A.* from POSMISDB_PartyAccount A) AD
            //            inner join 
            //            (select ITBID,PARTY_DESC from POSMISDB_PARTY )BD
            //            ON AD.PARTY_ITBID = BD.ITBID
            //            LEFT OUTER JOIN 
            //            (SELECT CARDSCHEME,CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME) CD
            //            ON AD.CARDSCHEME = CD.CARDSCHEME

            //            )
            //            where AD.PARTY_ITBID = :PARTY_ITBID");

            //qry = @"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME,AD.DEPOSIT_ACCTNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.LAST_MODIFIED_UID,
            //              AD.ITBID,  AD.CARDSCHEME,
            //              AD.PARTY_ITBID from
            //            (select A.* from POSMISDB_PartyAccount A) AD
            //            where AD.PARTY_ITBID = :P_PARTY_ITBID";

            qry = "GET_PARTY_ACCT";
            var rec = Fetch(c => c.Query<PartyAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<BillerAcctObj> GetBillerAcct(long PARTY_ITBID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BILLER_ITBID", PARTY_ITBID, OracleDbType.Int64);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
          

            qry = "GET_BILLER_ACCT";
            var rec = Fetch(c => c.Query<BillerAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public List<PartyAcctObj> GetPartyAcctTemp(long PARTY_ITBID,string bid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_PARTY_ITBID", PARTY_ITBID, OracleDbType.Int64);
            p.Add(":P_BATCHID", bid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
            //qry = (@"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME,AD.DEPOSIT_ACCTNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,
            //              AD.ITBID,  AD.CARDSCHEME,
            //              AD.PARTY_ITBID from                       
            //            (select A.* from POSMISDB_PartyAccountTEMP A) AD
            //            where AD.PARTY_ITBID = :PARTY_ITBID AND AD.BATCHID = :BATCHID");


            qry = "GET_PARTY_ACCT_TEMP";

            var rec = Fetch(c => c.Query<PartyAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<BillerAcctObj> GetBillerAcctTemp(long PARTY_ITBID, string bid,string user_id, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BILLER_ITBID", PARTY_ITBID, OracleDbType.Int64);
            p.Add(":P_BATCHID", bid, OracleDbType.Varchar2);
            p.Add(":P_USERID", user_id, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
           
            qry = "GET_BILLER_ACCT_TEMP";

            var rec = Fetch(c => c.Query<BillerAcctObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<InstitutionProcessorObj> GetIntitutionProcessor(long institutionId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", institutionId, OracleDbType.Int64);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry;
            //qry = (@" select AD.*, BD.INSTITUTION_NAME,AD.PROCTYPE || '|' || AD.ITBID AS CODE, CD.CARDSCHEME_DESC AS CardSchemDesc, DD.INSTITUTION_NAME AS InstitutionName from
            //            (
            //            (select A.* from POSMISDB_INSTPROCESSOR A) AD
            //            inner join
            //            (select ITBID, INSTITUTION_NAME from POSMISDB_INSTITUTION )BD
            //             ON AD.INSTITUTIONITBID = BD.ITBID
            //             LEFT OUTER JOIN
            //             (SELECT CARDSCHEME, CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME) CD
            //              ON AD.CARDSCHEME = CD.CARDSCHEME
            //              LEFT OUTER JOIN
            //              (SELECT CBN_CODE, INSTITUTION_NAME,ITBID FROM POSMISDB_INSTITUTION) DD
            //               ON AD.INSTITUTION_ID = DD.ITBID                         
            //            )where AD.INSTITUTIONITBID = :P_INSTITUTION_ID");

            qry = "GET_INST_PROCESSOR";


            var rec = Fetch(c => c.Query<InstitutionProcessorObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);
            var REC2 = rec.Select(f => new InstitutionProcessorObj
            {
                CARDSCHEME = f.CARDSCHEME,
                CardSchemDesc = f.CardSchemDesc,
                CREATEDATE = f.CREATEDATE,
                INSTITUTIONITBID = f.INSTITUTIONITBID,
                InstitutionName = f.InstitutionName,
                INSTITUTION_ID = f.INSTITUTION_ID,
                ITBID = f.ITBID,
                LAST_MODIFIED_UID = f.LAST_MODIFIED_UID,
                PROCESSORTYPE = f.PROCESSORTYPE,
                ProcessorTypeDesc = f.PROCESSORTYPE== "ISSR" ? "ISSUER" :f.PROCESSORTYPE =="ACQR" ? "ACQUIRER" :"",
                STATUS = f.STATUS,
                USERID = f.USERID,
                CODE = f.CODE,
                IS_PROCESSOR = f.IS_PROCESSOR,
                PROCTYPE = f.PROCTYPE
            }
            ).ToList();
            return REC2;

        }
        public List<InstitutionProcessorObj> GetIntitutionProcessorTemp(long institutionId,string batchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTION_ID", institutionId, OracleDbType.Int64);
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry;
            //qry = (@"select AD.*, CD.CARDSCHEME_DESC AS CardSchemDesc, DD.INSTITUTION_NAME AS InstitutionName from
            //            (
            //            (select A.* from POSMISDB_INSTPROCESSORTEMP A) AD
            //             LEFT OUTER JOIN
            //             (SELECT CARDSCHEME, CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME) CD
            //              ON AD.CARDSCHEME = CD.CARDSCHEME
            //              LEFT OUTER JOIN
            //              (SELECT CBN_CODE, INSTITUTION_NAME,ITBID FROM POSMISDB_INSTITUTION) DD
            //               ON AD.INSTITUTION_ID = DD.ITBID                         
            //            ) 
            //            where AD.INSTITUTIONITBID = :InstitutionId AND AD.BATCHID = :BATCHID AND lower(AD.STATUS) = 'open'");

            qry = "GET_INST_PROCESSOR_TEMP";
            var rec = Fetch(c => c.Query<InstitutionProcessorObj>(qry.ToString(), p, commandType: CommandType.StoredProcedure), null);
            var REC2 = rec.Select(f => new InstitutionProcessorObj
            {
                CARDSCHEME = f.CARDSCHEME,
                CardSchemDesc = f.CardSchemDesc,
                CREATEDATE = f.CREATEDATE,
                INSTITUTIONITBID = f.INSTITUTIONITBID,
                InstitutionName = f.InstitutionName,
                INSTITUTION_ID = f.INSTITUTION_ID,
                ITBID = f.ITBID,
                LAST_MODIFIED_UID = f.LAST_MODIFIED_UID,
                PROCESSORTYPE = f.PROCESSORTYPE,
                ProcessorTypeDesc = f.PROCESSORTYPE == "ISSR" ? "ISSUER" : f.PROCESSORTYPE == "ACQR" ? "ACQUIRER" : "",
                STATUS = f.STATUS,
                USERID = f.USERID
            }
            ).ToList();
            return REC2;

        }
        public List<EmailObj> GetAuthorizeEmailList(int MENUID,string DeptCode,int userInstitutionItbid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MENUID", MENUID, OracleDbType.Int32);
            p.Add(":P_DEPTCODE", DeptCode, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTIONITBID", userInstitutionItbid, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.ROLEID,BD.EMAIL FROM 
            //                (SELECT A.ROLEID,A.CANAUTHORIZE FROM POSMISDB_ROLEPRIV A
            //                WHERE A.MENUID = :MENUID AND CANAUTHORIZE = 1) AD
            //                INNER JOIN 
            //                (SELECT ""RoleId"" AS ROLEID,""Email"" AS EMAIL FROM ""AspNetUsers"" )BD
            //                ON AD.ROLEID = BD.ROLEID";

            string qry = "GET_AUTHORIZER_EMAIL";
            var rec = Fetch(c => c.Query<EmailObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<EmailObj> GetMakerEmail(string makerId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MAKERID", makerId, OracleDbType.Varchar2);           
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            
            string qry = "GET_MAKER_EMAIL";

            var rec = Fetch(c => c.Query<EmailObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }

        public List<AuthListObj2> GetAuthListList(int MENUID, string deptCode,int ROLEID, int userinstitution_itbid,string userid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
           // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add(":P_ROLEID", ROLEID, OracleDbType.Int32);
            p.Add(":P_INSTITUTION_ITBID", userinstitution_itbid, OracleDbType.Int32);
            p.Add(":P_DEPTCODE", deptCode, OracleDbType.Varchar2);
            p.Add(":P_USERID", userid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.*,CD.MENUNAME,INSTITUTION_NAME FROM 
            //                (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                INNER JOIN 
            //                (SELECT B.ROLEID,B.MENUID,B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B ) BD
            //                ON AD.MENUID = BD.MENUID
            //                INNER JOIN 
            //                (SELECT C.MENUID,C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                ON AD.MENUID = CD.MENUID
            //                 INNER JOIN 
            //                (SELECT D.ITBID,D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                ON AD.INSTITUTION_ITBID = DD.ITBID
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID AND BD.CANAUTHORIZE = 1
            //                AND Lower(STATUS) = 'open' order by AD.ITBID desc";
            string qry = "";
            //if (userinstitution_itbid == 1)
            //{
            //     qry = @"select * from 
            //                (SELECT AD.ITBID,  nvl(AD.TABLENAME,' ') TABLENAME,  nvl(AD.MENUID,0) MENUID,  nvl(AD.URL,' ') URL,  nvl(AD.RECORDID,0) RECORDID,
            //                  nvl(AD.EVENTTYPE,' ') EVENTTYPE, AD.CREATEDDATE, nvl(AD.STATUS,' ') STATUS, nvl(AD.USERID,' ') USERID,  AD.INSTITUTION_ITBID,
            //                  nvl(AD.POSTTYPE,' ') POSTTYPE,CD.MENUNAME,INSTITUTION_NAME,null as DEPARTMENTCODE FROM 
            //                (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                INNER JOIN 
            //                (SELECT B.ROLEID,B.MENUID,B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B ) BD
            //                ON AD.MENUID = BD.MENUID
            //                INNER JOIN 
            //                (SELECT C.MENUID,C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                ON AD.MENUID = CD.MENUID
            //                 INNER JOIN 
            //                (SELECT D.ITBID,D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                ON AD.INSTITUTION_ITBID = DD.ITBID                     
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID and LOWER(AD.USERID) != :USERID
            //                and bd.menuid=24   AND Lower(STATUS) = 'open' and BD.CANAUTHORIZE != 1
            //                union  all
            //                SELECT AD.ITBID,  nvl(AD.TABLENAME,' ') TABLENAME,  nvl(AD.MENUID,0) MENUID,  nvl(AD.URL,' ') URL,  nvl(AD.RECORDID,0) RECORDID,
            //                  nvl(AD.EVENTTYPE,' ') EVENTTYPE, AD.CREATEDDATE, nvl(AD.STATUS,' ') STATUS, nvl(AD.USERID,' ') USERID,  AD.INSTITUTION_ITBID,
            //                  nvl(AD.POSTTYPE,' ') POSTTYPE,CD.MENUNAME,INSTITUTION_NAME,FD.DEPARTMENTCODE FROM 
            //                (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                INNER JOIN 
            //                (SELECT B.ROLEID,B.MENUID,B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B ) BD
            //                ON AD.MENUID = BD.MENUID
            //                INNER JOIN 
            //                (SELECT C.MENUID,C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                ON AD.MENUID = CD.MENUID
            //                 INNER JOIN 
            //                (SELECT D.ITBID,D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                ON AD.INSTITUTION_ITBID = DD.ITBID
            //                INNER JOIN 
            //                (SELECT F.DEPARTMENTCODE,F.""UserName"" FROM  ""AspNetUsers"" F ) FD
            //                ON LOWER(AD.USERID) = Lower(FD.""UserName"")
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID
            //                AND FD.DEPARTMENTCODE = :DEPTCODE AND (LOWER(AD.USERID) = :USERID OR BD.CANAUTHORIZE = 1)
            //                AND Lower(STATUS) = 'open' 
            //                    )
            //                order by itbid desc
            //                ";
            //}
            //else {
            //                  qry = @"SELECT AD.ITBID,  nvl(AD.TABLENAME, ' ') TABLENAME,  nvl(AD.MENUID, 0) MENUID,  nvl(AD.URL, ' ') URL,  nvl(AD.RECORDID, 0) RECORDID,
            //                 nvl(AD.EVENTTYPE, ' ') EVENTTYPE, AD.CREATEDDATE, nvl(AD.STATUS, ' ') STATUS, nvl(AD.USERID, ' ') USERID,  AD.INSTITUTION_ITBID,
            //                 nvl(AD.POSTTYPE, ' ') POSTTYPE,CD.MENUNAME,INSTITUTION_NAME FROM

            //                 (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                 INNER JOIN
            //                 (SELECT B.ROLEID, B.MENUID, B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B) BD
            //                  ON AD.MENUID = BD.MENUID
            //                INNER JOIN
            //                (SELECT C.MENUID, C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                 ON AD.MENUID = CD.MENUID
            //                 INNER JOIN
            //                (SELECT D.ITBID, D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                 ON AD.INSTITUTION_ITBID = DD.ITBID
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID
            //                AND (LOWER(AD.USERID) = :USERID OR BD.CANAUTHORIZE = 1)
            //                AND Lower(STATUS) = 'open' order by AD.ITBID desc"; 


            //}
            qry = "GET_AUTH_LIST";
            var rec = Fetch(c => c.Query<AuthListObj2>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();

        }
        public AuthListCountObj GetAuthListListCount(int MENUID, string deptCode, int ROLEID, int userinstitution_itbid, string userid, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add(":P_ROLEID", ROLEID, OracleDbType.Int32);
            p.Add(":P_INSTITUTION_ITBID", userinstitution_itbid, OracleDbType.Int32);
            p.Add(":P_DEPTCODE", deptCode, OracleDbType.Varchar2);
            p.Add(":P_USERID", userid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.*,CD.MENUNAME,INSTITUTION_NAME FROM 
            //                (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                INNER JOIN 
            //                (SELECT B.ROLEID,B.MENUID,B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B ) BD
            //                ON AD.MENUID = BD.MENUID
            //                INNER JOIN 
            //                (SELECT C.MENUID,C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                ON AD.MENUID = CD.MENUID
            //                 INNER JOIN 
            //                (SELECT D.ITBID,D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                ON AD.INSTITUTION_ITBID = DD.ITBID
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID AND BD.CANAUTHORIZE = 1
            //                AND Lower(STATUS) = 'open' order by AD.ITBID desc";
            string qry = "";
            //if (userinstitution_itbid == 1)
            //{
            //     qry = @"select * from 
            //                (SELECT AD.ITBID,  nvl(AD.TABLENAME,' ') TABLENAME,  nvl(AD.MENUID,0) MENUID,  nvl(AD.URL,' ') URL,  nvl(AD.RECORDID,0) RECORDID,
            //                  nvl(AD.EVENTTYPE,' ') EVENTTYPE, AD.CREATEDDATE, nvl(AD.STATUS,' ') STATUS, nvl(AD.USERID,' ') USERID,  AD.INSTITUTION_ITBID,
            //                  nvl(AD.POSTTYPE,' ') POSTTYPE,CD.MENUNAME,INSTITUTION_NAME,null as DEPARTMENTCODE FROM 
            //                (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                INNER JOIN 
            //                (SELECT B.ROLEID,B.MENUID,B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B ) BD
            //                ON AD.MENUID = BD.MENUID
            //                INNER JOIN 
            //                (SELECT C.MENUID,C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                ON AD.MENUID = CD.MENUID
            //                 INNER JOIN 
            //                (SELECT D.ITBID,D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                ON AD.INSTITUTION_ITBID = DD.ITBID                     
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID and LOWER(AD.USERID) != :USERID
            //                and bd.menuid=24   AND Lower(STATUS) = 'open' and BD.CANAUTHORIZE != 1
            //                union  all
            //                SELECT AD.ITBID,  nvl(AD.TABLENAME,' ') TABLENAME,  nvl(AD.MENUID,0) MENUID,  nvl(AD.URL,' ') URL,  nvl(AD.RECORDID,0) RECORDID,
            //                  nvl(AD.EVENTTYPE,' ') EVENTTYPE, AD.CREATEDDATE, nvl(AD.STATUS,' ') STATUS, nvl(AD.USERID,' ') USERID,  AD.INSTITUTION_ITBID,
            //                  nvl(AD.POSTTYPE,' ') POSTTYPE,CD.MENUNAME,INSTITUTION_NAME,FD.DEPARTMENTCODE FROM 
            //                (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                INNER JOIN 
            //                (SELECT B.ROLEID,B.MENUID,B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B ) BD
            //                ON AD.MENUID = BD.MENUID
            //                INNER JOIN 
            //                (SELECT C.MENUID,C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                ON AD.MENUID = CD.MENUID
            //                 INNER JOIN 
            //                (SELECT D.ITBID,D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                ON AD.INSTITUTION_ITBID = DD.ITBID
            //                INNER JOIN 
            //                (SELECT F.DEPARTMENTCODE,F.""UserName"" FROM  ""AspNetUsers"" F ) FD
            //                ON LOWER(AD.USERID) = Lower(FD.""UserName"")
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID
            //                AND FD.DEPARTMENTCODE = :DEPTCODE AND (LOWER(AD.USERID) = :USERID OR BD.CANAUTHORIZE = 1)
            //                AND Lower(STATUS) = 'open' 
            //                    )
            //                order by itbid desc
            //                ";
            //}
            //else {
            //                  qry = @"SELECT AD.ITBID,  nvl(AD.TABLENAME, ' ') TABLENAME,  nvl(AD.MENUID, 0) MENUID,  nvl(AD.URL, ' ') URL,  nvl(AD.RECORDID, 0) RECORDID,
            //                 nvl(AD.EVENTTYPE, ' ') EVENTTYPE, AD.CREATEDDATE, nvl(AD.STATUS, ' ') STATUS, nvl(AD.USERID, ' ') USERID,  AD.INSTITUTION_ITBID,
            //                 nvl(AD.POSTTYPE, ' ') POSTTYPE,CD.MENUNAME,INSTITUTION_NAME FROM

            //                 (SELECT A.* FROM POSMISDB_AUTHLIST A) AD
            //                 INNER JOIN
            //                 (SELECT B.ROLEID, B.MENUID, B.CANAUTHORIZE FROM  POSMISDB_ROLEPRIV B) BD
            //                  ON AD.MENUID = BD.MENUID
            //                INNER JOIN
            //                (SELECT C.MENUID, C.MENUNAME FROM  POSMISDB_MENUCONTROL C ) CD
            //                 ON AD.MENUID = CD.MENUID
            //                 INNER JOIN
            //                (SELECT D.ITBID, D.INSTITUTION_NAME FROM  POSMISDB_INSTITUTION D ) DD
            //                 ON AD.INSTITUTION_ITBID = DD.ITBID
            //                WHERE BD.ROLEID = :ROLEID AND AD.INSTITUTION_ITBID = :INSTITUTION_ITBID
            //                AND (LOWER(AD.USERID) = :USERID OR BD.CANAUTHORIZE = 1)
            //                AND Lower(STATUS) = 'open' order by AD.ITBID desc"; 


            //}
            qry = "GET_AUTH_LIST_COUNT";
            var rec = Fetch(c => c.Query<AuthListCountObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.FirstOrDefault();

        }
        public RejectedUser GetAuthRejectedUser(decimal AUTH_ITBID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            // p.Add(":MENUID", MENUID, OracleDbType.Int32);
            p.Add(":P_AUTHLIST_ITBID", AUTH_ITBID, OracleDbType.Decimal);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_AUTH_REJECTED_USER";

            var rec = Fetch(c => c.Query<RejectedUser>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.FirstOrDefault();

        }


        public List<SettlementRuleObj> GetSettlementRulePerOptionScheme(int? SettlementOptionId, string CardScheme, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.*,BD.DESCRIPTION AS SETTLEMENTDESCRIPTION,CD.CARDSCHEME_DESC ,DD.PARTYTYPE_DESC PARTYTYPEDESC
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_SETTLEMENTRULE  A
            //                WHERE A.CARDSCHEME = :CARDSCHEME AND A.SETTLEMENTOPTION_ID = :SETTLEMENTOPTION_ID) AD
            //                INNER JOIN 
            //                (SELECT B.* FROM POSMISDB_SETTLEMENTOPTION B) BD
            //                ON AD.SETTLEMENTOPTION_ID = BD.ITBID
            //                 INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT C.* FROM POSMISDB_CARDSCHEME C) CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME";
            string qry = "GET_SET_RULE_OPTION_SCHEME";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<SettlementRuleObj> GetSettlementRulePerMccCategory(string mc_code, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CAT", mc_code, OracleDbType.Varchar2);
           // p.Add(":P_SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
          
            string qry = "GET_SETRULEOPTION_MC";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<AcaRuleObj> GetACARulePerACCCategory(string acc_code, int transfer_type, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ACC_CODE", acc_code, OracleDbType.Varchar2);
            p.Add(":P_TRANTYPE_ID", transfer_type, OracleDbType.Int32);
            // p.Add(":P_SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_ACARULE";
            var rec = Fetch(c => c.Query<AcaRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<AcquireSchemeObj> GetAcquirerScheme(string cbn_code, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_CBN_CODE", cbn_code, OracleDbType.Varchar2);
           // p.Add(":P_TRANTYPE_ID", transfer_type, OracleDbType.Int32);
            // p.Add(":P_SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_ACQUIRERSCHEME";
            var rec = Fetch(c => c.Query<AcquireSchemeObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantRuleObj2> GetSettlementRulePerMccCategory2(string mc_code, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CAT", mc_code, OracleDbType.Varchar2);
            // p.Add(":P_SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_SETRULEOPTION_MC";
            var rec = Fetch(c => c.Query<MerchantRuleObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantRuleObj2> GetInstitutionRulePerMccCategory(string mc_code,string cbn_code, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CAT", mc_code, OracleDbType.Varchar2);
            p.Add(":P_CBNCODE", cbn_code, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_INSTRULE_MC";
            var rec = Fetch(c => c.Query<MerchantRuleObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        
        public List<SettlementRuleObj> GetSettlementRulePerOption(int SettlementOptionId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            //  p.Add(":CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.*,BD.DESCRIPTION AS SETTLEMENTDESCRIPTION ,DD.PARTYTYPE_DESC PARTYTYPEDESC
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_SETTLEMENTRULE  A
            //                 where A.SETTLEMENTOPTION_ID = :SETTLEMENTOPTION_ID) AD
            //                INNER JOIN 
            //                (SELECT B.* FROM POSMISDB_SETTLEMENTOPTION B) BD
            //                ON AD.SETTLEMENTOPTION_ID = BD.ITBID
            //                 INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //           ";
            string qry = "GET_SET_RULE_PER_OPTION";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<SettlementRuleObj> GetSettlementRuleFromTemp( string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            //string qry = @"SELECT AD.*,BD.DESCRIPTION AS SETTLEMENTDESCRIPTION,CD.CARDSCHEME_DESC ,DD.PARTYTYPE_DESC PARTYTYPEDESC
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_SETTLEMENTRULETEMP  A
            //                WHERE A.BATCHID = :BATCHID) AD
            //                INNER JOIN 
            //                (SELECT B.* FROM POSMISDB_SETTLEMENTOPTION B) BD
            //                ON AD.SETTLEMENTOPTION_ID = BD.ITBID
            //                 INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT C.* FROM POSMISDB_CARDSCHEME C) CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME";
            //string qry = @"SELECT AD.*,BD.DESCRIPTION AS SETTLEMENTDESCRIPTION ,DD.PARTYTYPE_DESC PARTYTYPEDESC
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_SETTLEMENTRULETEMP  A
            //                WHERE A.BATCHID = :BATCHID AND (LOWER(A.STATUS) = 'open' or LOWER(A.STATUS) = 'rejected')) AD
            //                LEFT OUTER JOIN
            //                (SELECT B.* FROM POSMISDB_SETTLEMENTOPTION B) BD
            //                ON AD.SETTLEMENTOPTION_ID = BD.ITBID
            //                 INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //               ";
            string qry = "GET_SET_RULE_FROM_TEMP";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<SettlementRuleObj> GetSettlementRulePerMccCategoryTemp(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
          
            string qry = "GET_SETRULEOPTION_MCTEMP";
            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<InstitutionRuleObj> GetInstitutionRulePerMccCategoryTemp(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_INSTRULE_MCTEMP";
            var rec = Fetch(c => c.Query<InstitutionRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<AcaRuleObj> GetACARulePerACCCategoryTemp(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_ACCRULE_TEMP";
            var rec = Fetch(c => c.Query<AcaRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<AcquireSchemeObj> GetAcquirerSchemeTemp(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            string qry = "GET_ACQUIRERSCHEME_TEMP";
            var rec = Fetch(c => c.Query<AcquireSchemeObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantRuleObj2> GetMerchantRuleCBN(string Mcc_Cat, string merchantId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
           // p.Add(":P_CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_MCC_CAT", Mcc_Cat, OracleDbType.Varchar2);
            //p.Add(":P_INSTITUTION_ID", Institution_Id, OracleDbType.Varchar2);
            p.Add(":P_MERCHANTID", merchantId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            var qry = "GET_MERCHANTRULE_CBN";
            var rec = Fetch(c => c.Query<MerchantRuleObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantRuleObj2> GetMerchantRuleCBNFromTemp(string BATCHID,string userId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":USERID", userId, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            var qry = "GET_MERCHANTRULECBN_TEMP";
            var rec = Fetch(c => c.Query<MerchantRuleObj2>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MCCRuleObj> GetMCCRuleFromTemp(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            //string qry = @"SELECT AD.*,BD.MCC_DESC,CD.CARDSCHEME_DESC ,DD.PARTYTYPE_DESC PARTYTYPEDESC
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_MCCRULETEMP  A
            //                WHERE A.BATCHID = :BATCHID) AD
            //                INNER JOIN 
            //                (SELECT B.* FROM POSMISDB_MCC B) BD
            //                ON AD.MCC_CODE = BD.MCC_CODE
            //                 INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT C.* FROM POSMISDB_CARDSCHEME C) CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME";

            string qry = "GET_MCCRULE_FROM_TEMP";
            var rec = Fetch(c => c.Query<MCCRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<MCCRuleObj> GetMCCRuleFromTempWithSetllementOption(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            //string qry = @"SELECT AD.*,BD.MCC_DESC,CD.CARDSCHEME_DESC ,DD.PARTYTYPE_DESC PARTYTYPEDESC,1 as MakeReadOnly,ED.FULLNAME
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_MCCRULETEMP  A
            //                WHERE A.BATCHID = :P_BATCHID AND (Lower(A.STATUS) = 'open' or Lower(A.STATUS) = 'rejected')) AD
            //                INNER JOIN 
            //                (SELECT B.* FROM POSMISDB_MCC B) BD
            //                ON AD.MCC_CODE = BD.MCC_CODE
            //                 INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT C.* FROM POSMISDB_CARDSCHEME C) CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME
            //                LEFT OUTER JOIN 
            //                (SELECT E.""UserName"",E.""FullName"" FULLNAME FROM ""AspNetUsers"" E) ED
            //                ON AD.USERID = ED.""UserName""";
            string qry = "GET_MCCRULE_FROMTEMP_SETOPTION";
            var rec = Fetch(c => c.Query<MCCRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);



            return rec.ToList();

        }
        public List<ProductRuleObj> GetpProductRuleFromTempWithSetllementOption(string BatchId, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", BatchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_PRODRULE_TEMP_SETOPTION";
            var rec = Fetch(c => c.Query<ProductRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);           
            return rec.ToList();

        }
        public List<MCCRuleObj> GetMCCRule(string mcc_code, string institution_id,string cardscheme, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", mcc_code, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTION_ID", institution_id, OracleDbType.Varchar2);
            p.Add(":P_CARDSCHEME", cardscheme, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            //string qry = @" SELECT AD.*,BD.MCC_DESC,CD.CARDSCHEME_DESC ,DD.PARTYTYPE_DESC PARTYTYPEDESC
            //                FROM 
            //                (SELECT A.* FROM POSMISDB_MCCRULE  A
            //                WHERE A.MCC_CODE = :MCC_CODE AND A.INSTITUTION_ID = :INSTITUTION_ID 
            //                AND A.CARDSCHEME = :CARDSCHEME ) AD
            //                INNER JOIN 
            //                (SELECT B.* FROM POSMISDB_MCC B) BD
            //                ON AD.MCC_CODE = BD.MCC_CODE
            //                INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON AD.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT C.* FROM POSMISDB_CARDSCHEME C) CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME";

            var qry = "GET_MCCRULE";
            var rec = Fetch(c => c.Query<MCCRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MCCRuleObj> GetMCCRuleWithSetllementOption(string mcc_code, string institution_id, string cardscheme,int? SETTLEMENTOPTION_ID, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", mcc_code, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTION_ID", institution_id, OracleDbType.Varchar2);
            p.Add(":P_CARDSCHEME", cardscheme, OracleDbType.Varchar2);
            p.Add(":P_SETTLEMENTOPTION_ID", SETTLEMENTOPTION_ID, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":SETTLEMENTOPTION_ID", SettlementOptionId, OracleDbType.Int32);
            //string qry = @"SELECT nvl(AD.ITBID,0) ITBID,AD.MCC_CODE,AD.CARDSCHEME,nvl(AD.PARTYTYPE_VALUE,0) PARTYTYPE_VALUE,AD.USERID,AD.STATUS,AD.INSTITUTION_ID,
            //                BD.MCC_DESC,CD.CARDSCHEME_DESC ,DD.PARTYTYPE_DESC PARTYTYPEDESC,ED.PARTYTYPE_CODE ,ED.SETTLEMENTOPTION_ID,ED.ITBID SETTLEMENTRULE_ITBID
            //               FROM 
            //                (SELECT E.SETTLEMENTOPTION_ID,E.PARTYTYPE_CODE ,E.ITBID 
            //                FROM POSMISDB_SETTLEMENTRULE E 
            //                where E.SETTLEMENTOPTION_ID =:P_SETTLEMENTOPTION_ID)ED
            //               LEFT OUTER JOIN 
            //                (SELECT A.* FROM POSMISDB_MCCRULE  A
            //                WHERE A.MCC_CODE = :P_MCC_CODE AND A.INSTITUTION_ID = :P_INSTITUTION_ID 
            //                AND A.CARDSCHEME = :P_CARDSCHEME ) AD
            //                ON ED.SETTLEMENTOPTION_ID = AD.SETTLEMENTOPTION_ID and ed.PARTYTYPE_CODE = ad.PARTYTYPE_CODE
            //                LEFT OUTER JOIN 
            //                (SELECT B.* FROM POSMISDB_MCC B) BD
            //                ON AD.MCC_CODE = BD.MCC_CODE
            //                INNER JOIN 
            //                (SELECT D.PARTYTYPE_CODE,D.PARTYTYPE_DESC FROM POSMISDB_PARTYTYPE D) DD
            //                ON ED.PARTYTYPE_CODE = DD.PARTYTYPE_CODE
            //                LEFT OUTER JOIN 
            //                (SELECT C.* FROM POSMISDB_CARDSCHEME C) CD
            //                ON AD.CARDSCHEME = CD.CARDSCHEME";
            string qry = "GET_MCCRULE_SET_OPTION";
            var rec = Fetch(c => c.Query<MCCRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<ProductRuleObj> GetProductRuleBySetllementOption(string productCode, string merchantId, int? SETTLEMENTOPTION_ID)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add("P_PRODUCT_CODE", productCode, OracleDbType.Varchar2);
            p.Add("P_MERCHANTID", merchantId, OracleDbType.Varchar2);
           
            p.Add("P_SETTLEMENTOPTION_ID", SETTLEMENTOPTION_ID, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
           
            string qry = "GET_PRODUCT_RULE_BY_OPTION";

            var rec = Fetch(c => c.Query<ProductRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }


        public List<MerchantRuleObj> GetMerchantRule(string Mcc_Code, string CardScheme,string Institution_Id,string merchantId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_MCC_CODE", Mcc_Code, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTION_ID", Institution_Id, OracleDbType.Varchar2);
            p.Add(":P_MERCHANTID", merchantId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT AD.*,BD.CARDSCHEME_DESC,
            //                CD.MERCHANTID,cast(NVL(CD.PARTY_ITBID,0) as number(9)) PARTY_ITBID,NVL(CD.PARTY_CONCESSION,0) PARTY_CONCESSION
            //                ,NVL(CD.PARTY_SUBSIDY,0) PARTY_SUBSIDY,CD.PARTY_DESC,
            //                FD.PARTYTYPE_DESC,NVL( MERCHANTRULE_ITBID,0) MERCHANTRULE_ITBID FROM
            //                (select A.* from posmisdb_MCCrule A 
            //                where A.MCC_CODE = :P_MCC_CODE AND A.INSTITUTION_ID = :P_INSTITUTION_ID AND A.CARDSCHEME = :P_CARDSCHEME  
            //                ) AD
            //                INNER JOIN 
            //                (SELECT F.PARTYTYPE_CODE,F.PARTYTYPE_DESC  FROM POSMISDB_PARTYTYPE F) FD
            //                ON AD.PARTYTYPE_CODE = FD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT B.CARDSCHEME,B.CARDSCHEME_DESC  FROM POSMISDB_CARDSCHEME B) BD
            //                ON AD.CARDSCHEME = BD.CARDSCHEME
            //                LEFT OUTER JOIN 
            //                (SELECT C.MERCHANTID,MCCRULE_ITBID,PARTY_ITBID,C.PARTY_CONCESSION,C.PARTY_SUBSIDY,PARTY_DESC, C.ITBID MERCHANTRULE_ITBID
            //                 FROM POSMISDB_MERCHANTRULE C
            //                LEFT OUTER JOIN POSMISDB_PARTY R
            //                ON C.PARTY_ITBID = R.ITBID
            //                WHERE C.MERCHANTID = :P_MERCHANTID) CD
            //                ON AD.ITBID = CD.MCCRULE_ITBID";

            var qry = "GET_MERCHANT_RULE";
            var rec = Fetch(c => c.Query<MerchantRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }
        public List<MerchantRuleObj> GetMerchantRuleFromTemp(string BATCHID)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
           // p.Add(":CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @" SELECT AD.*,BD.CARDSCHEME_DESC,
            //                CD.MERCHANTID,cast(NVL(CD.PARTY_ITBID,0) as number(9)) PARTY_ITBID
            //                ,NVL(CD.PARTY_CONCESSION,0) PARTY_CONCESSION,NVL(CD.PARTY_SUBSIDY,0) PARTY_SUBSIDY,CD.PARTY_DESC,
            //                FD.PARTYTYPE_DESC,NVL( MERCHANTRULE_ITBID,0) MERCHANTRULE_ITBID,MERCHANT_STATUS ,CD. MAKER_ID, CD.DATECREATED  FROM
            //                (select A.* from posmisdb_MCCrule A                            
            //                ) AD
            //                INNER JOIN 
            //                (SELECT F.PARTYTYPE_CODE,F.PARTYTYPE_DESC  FROM POSMISDB_PARTYTYPE F) FD
            //                ON AD.PARTYTYPE_CODE = FD.PARTYTYPE_CODE
            //                INNER JOIN 
            //                (SELECT B.CARDSCHEME,B.CARDSCHEME_DESC  FROM POSMISDB_CARDSCHEME B) BD
            //                ON AD.CARDSCHEME = BD.CARDSCHEME
            //                inner JOIN 
            //                (SELECT C.MERCHANTID,MCCRULE_ITBID,PARTY_ITBID,C.PARTY_CONCESSION,C.PARTY_SUBSIDY,PARTY_DESC, C.ITBID MERCHANTRULE_ITBID
            //                ,C.STATUS AS MERCHANT_STATUS,C.USERID AS MAKER_ID,C.CREATEDATE AS DATECREATED
            //                 FROM POSMISDB_MERCHANTRULETEMP C
            //                left outer JOIN POSMISDB_PARTY R 
            //                ON C.PARTY_ITBID = R.ITBID
            //                WHERE C.BATCHID = :P_BATCHID AND (LOWER(C.STATUS) = 'open' OR LOWER(C.STATUS) = 'rejected')) CD
            //                ON AD.ITBID = CD.MCCRULE_ITBID";

            var qry = "GET_MERCHANT_RULE_TEMP";
            var rec = Fetch(c => c.Query<MerchantRuleObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();

        }

        public POSMISDB_MCCMSC GetMSCPerSchemeMCC(string MCC_CODE, string CardScheme, string conString = null)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_CARDSCHEME", CardScheme, OracleDbType.Varchar2);
            p.Add(":P_MCC_CODE", MCC_CODE, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"SELECT MSC_CALCBASIS,DOM_MSCVALUE,INT_MSCVALUE FROM POSMISDB_MCCMSC
            //                WHERE MCC_CODE = :P_MCC_CODE AND CARDSCHEME = :P_CARDSCHEME";
            string qry = "GET_MSC_PER_SCHEME_MCC";
            var rec = Fetch(c => c.Query<POSMISDB_MCCMSC>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.FirstOrDefault();

        }

        public List<SettlementRuleObj> GetSettlementRuleGrouping(string conString = null)
        {
            string qry = @"SELECT AD.* ,BD.SETTLEMENTOPTION_DESC,CD.CARDSCHEME_DESC  FROM
                            (select CARDSCHEME,SETTLEMENTOPTION_ID from POSMISDB_SETTLEMENTRULE
                            group by CARDSCHEME,SETTLEMENTOPTION_ID) AD
                            inner join 
                            (select ITBID,DESCRIPTION SETTLEMENTOPTION_DESC from posmisdb_settlementoption)BD
                            ON AD.SETTLEMENTOPTION_ID = BD.ITBID 
                            inner join 
                            (select CARDSCHEME,CARDSCHEME_DESC from POSMISDB_CARDSCHEME)CD
                            ON AD.CARDSCHEME = CD.CARDSCHEME ";

            var rec = Fetch(c => c.Query<SettlementRuleObj>(qry, null, commandType: CommandType.Text), null);

            return rec.ToList();

        }
      
        public UserCount GetExistedUserNameCount(string userName)
        {
            var p = new OracleDynamicParameters();
            p.Add(":UserName", userName, OracleDbType.Varchar2);
          //  p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = string.Format(@"select count(*) ExistCount from ""AspNetUsers""
                           where Lower(""UserName"") like '%{0}%'",userName.ToLower()); 

            var rec = Fetch(c => c.Query<UserCount>(qry, null, commandType: CommandType.Text), null);

            return rec.FirstOrDefault();
        }
        public List<DropdownObject> GetInstitutionParty()
        {

            var p = new OracleDynamicParameters();
            //p.Add(":UserName", userName, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select ITBID ||'|I' CODE,INSTITUTION_NAME || ' (INSTITUTION)' DESCRIPTION from POSMISDB_INSTITUTION A
            //               union all
            //               select ITBID ||'|P' CODE,PARTY_DESC || ' (PARTY)' DESCRIPTION from posmisdb_party B
            //               where B.INSTITUTION_BASED<> 'Y' OR B.INSTITUTION_BASED IS NULL";
            var qry = "GET_INSTITUTION_PARTY";
            var rec = Fetch(c => c.Query<DropdownObject>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public List<mccuploadGloObj> GetMCCUploadGlo()
        {
            //var p = new OracleDynamicParameters();
            //p.Add(":UserName", userName, OracleDbType.Varchar2);
            string qry = @"select A.*,B.FULLNAME from
                            (select * from POSMISDB_MCCMSCUPLDGLO )A
                            inner join 
                            (select ""UserName"",""FullName"" FULLNAME from ""AspNetUsers"" )B
                            on A.USERID = B.""UserName""
                                        ";
          //  string qry = "GET_INSTITUTION_PARTY";
            var rec = Fetch(c => c.Query<mccuploadGloObj>(qry, null, commandType: CommandType.Text), null);

            return rec.ToList();
        }
        public List<BinUploadGloObj> GetBINUploadGlo()
        {
            //var p = new OracleDynamicParameters();
            //p.Add(":UserName", userName, OracleDbType.Varchar2);
            string qry = @"select A.*,B.FULLNAME from
                            (select * from POSMISDB_INSTITUTIONBINGLO )A
                            inner join 
                            (select ""UserName"",""FullName"" FULLNAME from ""AspNetUsers"" )B
                            on LOWER(A.USERID) = LOWER(B.""UserName"")
                                        ";

            var rec = Fetch(c => c.Query<BinUploadGloObj>(qry, null, commandType: CommandType.Text), null);

            return rec.ToList();
        }

        public List<POSMISDB_MCCMSC_UPLD> GetMCCUploadDetail(int GlobalId)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_GLOBALID", GlobalId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // string qry = @"select * from POSMISDB_MCCMSC_UPLD where GLOBAL_ITBID = :P_GLOBALID and Lower(status) = 'un-approved'";

            string qry = "GET_MCCUPLOAD_DETAIL";
            var rec = Fetch(c => c.Query<POSMISDB_MCCMSC_UPLD>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public List<POSMISDB_INSTITUTIONBINTEMP> GetBINUploadDetail(int GlobalId)
        {
            var p = new OracleDynamicParameters();
            p.Add(":GlobalId", GlobalId, OracleDbType.Int32);
            string qry = @"select * from POSMISDB_INSTITUTIONBINTEMP where GLOBAL_ITBID = :GlobalId and Lower(status) = 'un-approved'"  ;

            var rec = Fetch(c => c.Query<POSMISDB_INSTITUTIONBINTEMP>(qry, p, commandType: CommandType.Text), null);

            return rec.ToList();
        }
        public List<MerchantProductObj> GetMerchantProduct(short? IsAll, string MerchantId, decimal? itbId)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_ISALL", IsAll, OracleDbType.Int16);
            p.Add(":P_ITBID", itbId, OracleDbType.Decimal);
            p.Add(":P_MERCHANTID", MerchantId, OracleDbType.Varchar2);
          //  p.Add(":P_ISTEMP", IsTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_MERCHANT_PRODUCT";
            //if (IsAll)
            //{
            //    p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            //     qry = @"select a.*,b.MERCHANTNAME,c.PRODUCTTYPEDESC from posmisdb_merchantproduct a
            //                inner join posmisdb_merchantdetail b
            //                on a.MERCHANTID = b.MERCHANTID
            //                inner join POSMISDB_PRODUCT_TYPE c
            //                on a.PRODUCT_TYPE = c.itbid";
            //}
            //else
            //{
            //    p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            //     qry = @"select a.*,b.MERCHANTNAME,c.PRODUCTTYPEDESC from posmisdb_merchantproduct a
            //                inner join posmisdb_merchantdetail b
            //                on a.MERCHANTID = b.MERCHANTID
            //                inner join POSMISDB_PRODUCT_TYPE c
            //                on a.PRODUCT_TYPE = c.itbid
            //                where a.MERCHANTID = :MERCHANTID";
            //}


            var rec = Fetch(c => c.Query<MerchantProductObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public List<MerchantProductObj> GetMerchantProductTemp(short? IsAll, string batchId, decimal? itbId)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_ISALL", IsAll, OracleDbType.Int16);
            p.Add(":P_ITBID", itbId, OracleDbType.Decimal);
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            //  p.Add(":P_ISTEMP", IsTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_MERCHANT_PRODUCTTEMP";
       

            var rec = Fetch(c => c.Query<MerchantProductObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public List<MerchantAcctObj> GetMerchantAccount(bool IsAll, string MerchantId)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_MERCHANTID", MerchantId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_MERCHANT_ACCT";
            //if (IsAll)
            //{
            //    p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            //    qry = @"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME, AD.DEPOSIT_ACCTNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDRESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.USERID
            //              AD.ITBID,AD.MERCHANTID,b.MERCHANTNAME from posmisdb_merchantACCT AD
            //                inner join posmisdb_merchantdetail b
            //                on AD.MERCHANTID = b.MERCHANTID";
            //}
            //else
            //{
            //    p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            //    qry = @"select AD.DEPOSIT_BANKCODE,
            //              AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME, AD.DEPOSIT_ACCTNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDRESS,
            //              NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.USERID,
            //              AD.ITBID,AD.MERCHANTID,b.MERCHANTNAME from posmisdb_merchantACCT AD
            //                inner join posmisdb_merchantdetail b
            //                on AD.MERCHANTID = b.MERCHANTID 
            //                where AD.MERCHANTID = :MERCHANTID";
            //}


            var rec = Fetch(c => c.Query<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public List<MerchantAcctObj> GetMerchantAccountFromTemp( string batchId)
        {
            string qry = "";
            var p = new OracleDynamicParameters();
        
                p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //qry = @"select AD.DEPOSIT_BANKCODE,
            //          AD.DEPOSIT_ACCOUNTNO, AD.DEPOSIT_BANKNAME,AD.DEPOSIT_COUNTRYCODE, AD.DEPOSIT_BANKADDRESS,
            //          NVL(AD.DEFAULT_ACCOUNT,0) AS DefaultAccount,AD.CREATEDATE, AD.USERID,AD.STATUS,AD.USERID,AD.RECORDID,
            //          AD.ITBID,AD.MERCHANTID,b.MERCHANTNAME from posmisdb_merchantACCTTEMP AD
            //            inner join posmisdb_merchantdetail b
            //            on AD.MERCHANTID = b.MERCHANTID                      
            //            where AD.BATCHID = :BATCHID AND LOWER(AD.STATUS) = 'open'";
            qry = "GET_MERCHANT_ACCT_TEMP";

            var rec = Fetch(c => c.Query<MerchantAcctObj>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }


        public List<InstitutionBinObj> GetInstitutionBin()
        {
           // var p = new OracleDynamicParameters();
           // p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            string qry = @"select A.*,INSTITUTION_NAME,PARTYTYPE_DESC,CURRENCY_NAME from 
                            (
                            select a.* from POSMISDB_INSTITUTIONBIN a) A
                            inner join 
                            (select INSTITUTION_ID,INSTITUTION_NAME from POSMISDB_INSTITUTION) B
                            on A.INSTITUTION_ID = B.INSTITUTION_ID
                            inner join 
                            (select PARTYTYPE_CODE,PARTYTYPE_DESC from POSMISDB_PARTYTYPE) C
                            on A.BINTYPE = C.PARTYTYPE_CODE
                            inner join 
                            (select CURRENCY_CODE, CURRENCY_NAME from POSMISDB_CURRENCY) D
                            on A.CURRENCY_CODE = D.CURRENCY_CODE";

            var rec = Fetch(c => c.Query<InstitutionBinObj>(qry, null, commandType: CommandType.Text), null);

            return rec.ToList();
        }
        public List<DropdownObject> GetPartyInstitutioUnionByCbn_Code()
        {
            var p = new OracleDynamicParameters();
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            //string qry = @"select CODE,DESCRIPTION from 
            //                (select 'P' || '|'|| ItbId CODE , Party_desc DESCRIPTION from posmisdb_party where Lower(PartyType_Code) = 'ptsp'
            //                union all
            //                select 'I' || '|'|| ItbId CODE,institution_name DESCRIPTION from posmisdb_institution where cbn_code is not null ) A";

            string qry = "GET_PARTY_INST_BY_CBNCODE";
            var rec = Fetch(c => c.Query<DropdownObject>(qry, null, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DropdownObject> GetPartyInstitutioUnionByPTSP(long INSTITUTIONITBID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_INSTITUTIONITBID", INSTITUTIONITBID, OracleDbType.Int64);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select CODE,DESCRIPTION from 
            //                (select 'P' || '|'|| ItbId CODE , Party_desc DESCRIPTION from posmisdb_party 
            //                where Lower(PartyType_Code) = 'ptsp'
            //                union all
            //                select 'I' || '|'|| ItbId CODE,institution_name DESCRIPTION from posmisdb_institution 
            //                where   ITBID <> :INSTITUTIONITBID AND ptsp = 'Y')";

            string qry = "GET_PARTY_INST_BY_PTSP";
            var rec = Fetch(c => c.Query<DropdownObject>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<MCCObj> GetMCC()
        {
            var p = new OracleDynamicParameters();
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //p.Add(":INSTITUTIONITBID", INSTITUTIONITBID, OracleDbType.Int64);
            //string qry = @"select A.*,B.FULLNAME,C.SECTOR_NAME from
            //                (select a.* from posmisdb_mcc a)A
            //                inner join 
            //                (select b.""UserName"",b.""FullName"" FULLNAME from ""AspNetUsers"" b) B
            //                on A.userid = B.""UserName""
            //                left outer join
            //                (select sector_code, SECTOR_NAME from POSMISDB_SECTOR) C
            //                 on A.sector_code = C.sector_code";
            string qry = "GET_MCC";
            var rec = Fetch(c => c.Query<MCCObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public List<PayStampObj> GetPayStampList(string option,string MID_OR_NAME)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MID_OR_NAME", MID_OR_NAME, OracleDbType.Varchar2);
            p.Add(":P_OPTION", option, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "";
            //if (option == "1")
            //{
            //     qry = @"SELECT A.TERMINALID, A.MERCHANTID,B.MERCHANTNAME,A.PAYATTITUDE_STAMP
            //                        FROM
            //                        (select TERMINALID, MERCHANTID,PAYATTITUDE_STAMP from posmisdb_terminal a
            //                        WHERE A.MERCHANTID = :MERCHANTID AND LOWER(a.PAYATTITUDE_STAMP) = 'y') A
            //                        inner join
            //                        (select MERCHANTID,MERCHANTNAME from posmisdb_merchantdetail) B
            //                        on A.MERCHANTID = B.MERCHANTID";
            //}
            //else
            //{
            //     //qry = @"SELECT A.TERMINALID, A.MERCHANTID,B.MERCHANTNAME,A.PAYATTITUDE_STAMP
            //     //       FROM
            //     //       (select TERMINALID, MERCHANTID,PAYATTITUDE_STAMP from posmisdb_terminal a
            //     //       WHERE A.MERCHANTID = :MERCHANTID AND (a.PAYATTITUDE_STAMP <> 'Y' or a.PAYATTITUDE_STAMP is null)) A
            //     //       inner join
            //     //       (select MERCHANTID,MERCHANTNAME from posmisdb_merchantdetail) B
            //     //       on A.MERCHANTID = B.MERCHANTID";
            //}
            // qry = "GET_PAYARENA_MERCHANT";
            qry = "GET_PAYARENA_MERCHANT2";
            var rec = Fetch(c => c.Query<PayStampObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DropdownObject> GetMCCMscInstitution(string mcc_Code)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", mcc_Code, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select distinct A.INSTITUTION_ID AS CODE,institution_name AS DESCRIPTION from 
            //                (select INSTITUTION_ID from POSMISDB_MCCMSC
            //                where  MCC_CODE = :MCC_CODE) A
            //                inner join
            //                (select institution_id,institution_name from posmisdb_institution ) B
            //                on A.INSTITUTION_ID = B.INSTITUTION_ID
            //                ";
            string qry = "GET_MCCMSC_INSTITUTION";
            var rec = Fetch(c => c.Query<DropdownObject>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<MccMscObj> GetMCCMscInstitutionCardScheme(string mcc_Code,string institution_id)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MCC_CODE", mcc_Code, OracleDbType.Varchar2);
            p.Add(":P_INSTITUTION_ID", institution_id, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //string qry = @"select A.CARDSCHEME as Code,C.CARDSCHEMe_DESC as Description,msc_calcbasis,dom_mscvalue
            //                ,intmsc_calcbasis,int_mscvalue,domcap,intlcap from 
            //                (select INSTITUTION_ID,CARDSCHEME,msc_calcbasis,dom_mscvalue,intmsc_calcbasis,int_mscvalue,domcap,intlcap from POSMISDB_MCCMSC
            //                where  MCC_CODE = :MCC_CODE  and INSTITUTION_ID = :INSTITUTION_ID) A
            //                inner join
            //                (select * from POSMISDB_CARDSCHEME ) C
            //                on A.cardscheme = C.cardscheme";
            string qry = "GET_MCCMSC_INSTITUTION_SCHEME";
            var rec = Fetch(c => c.Query<MccMscObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DropdownObject> GetTerminalMerchant(string merchantId)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", merchantId, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //  p.Add(":INSTITUTION_ID", institution_id, OracleDbType.Varchar2);c\cx
            string qry = @"GET_TERMINAL_MERCHANT";
            var rec = Fetch(c => c.Query<DropdownObject>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public string GetTerminalInstitutionCbnCode(string TERMINALID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_TERMINALID", TERMINALID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //   p.Add(":INSTITUTION_ID", institution_id, OracleDbType.Varchar2);
            //string qry = @"SELECT CBN_CODE FROM 
            //                (SELECT TERMINALID,MERCHANTID FROM POSMISDB_TERMINAL
            //                WHERE TERMINALID = :TERMINALID) A
            //                INNER JOIN
            //                (SELECT MERCHANTID,INSTITUTION_CBNCODE FROM POSMISDB_MERCHANTDETAIL) B
            //                ON A.MERCHANTID = B.MERCHANTID
            //                INNER JOIN
            //                (SELECT CBN_CODE FROM POSMISDB_INSTITUTION) C
            //                ON B.INSTITUTION_CBNCODE = C.CBN_CODE";
            string qry = "GET_TERMINAL_INST_CBNCODE";
            var rec = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qry, p, commandType: CommandType.StoredProcedure), null).FirstOrDefault();

            return rec != null ? rec.CBN_CODE : "";
        }
        public List<UserObj> GetUserList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_USER_LIST";
            var rec = Fetch(c => c.Query<UserObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<UserObj> GetUserListBank(int itbid, int isAll, int isTemp, int instId)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":P_INSTID", instId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_USER_LIST_BANK";
            var rec = Fetch(c => c.Query<UserObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<CurrencyObj> GetCurrencyList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CURRENCY_LIST";
            var rec = Fetch(c => c.Query<CurrencyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DepartmentObj> GetDepartmentList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_DEPARTMENT_LIST";
            var rec = Fetch(c => c.Query<DepartmentObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<PartyObjList> GetPartyList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_PARTY_LIST";
            var rec = Fetch(c => c.Query<PartyObjList>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<BillerObjList> GetBillerList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_BILLER_LIST";
            var rec = Fetch(c => c.Query<BillerObjList>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<BillerObjList> GetBillerByMerchantList(string mid)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_MID", mid, OracleDbType.Varchar2);
            //p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            //p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_BILLERBYMID_LIST";
            var rec = Fetch(c => c.Query<BillerObjList>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        
        public List<CountryObj> GetCountryList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_COUNTRY_LIST";
            var rec = Fetch(c => c.Query<CountryObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<StateObj> GetStateList(int itbid, int isAll, int isTemp,string countryCode)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_STATE_LIST";
            var rec = Fetch(c => c.Query<StateObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
         public List<CityObj> GetCity(int ItbId, string P_COUNTRYCODE,string stateCode, bool IsAll, bool isTemp = false)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_COUNTRYCODE", P_COUNTRYCODE, OracleDbType.Varchar2);
            p.Add(":P_STATECODE", stateCode, OracleDbType.Varchar2);
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_ITBID", ItbId, OracleDbType.Int32);
         //   p.Add(":P_ISTEMP", isTemp ? 1 : 0, OracleDbType.Int16);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CITY_LIST";


            var rec = Fetch(c => c.Query<CityObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<CityObj> GetCity2(int ItbId, string P_COUNTRYCODE, string stateCode, bool IsAll, bool isTemp = false)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_COUNTRYCODE", P_COUNTRYCODE, OracleDbType.Varchar2);
            p.Add(":P_STATECODE", stateCode, OracleDbType.Varchar2);
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_ITBID", ItbId, OracleDbType.Int32);
            p.Add(":P_ISTEMP", isTemp ? 1 : 0, OracleDbType.Int16);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CITY_LIST2";


            var rec = Fetch(c => c.Query<CityObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }

        public List<ExchangeRateObj> GetExchangeRatetEMP(string P_BATCHID = null)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_EXCHANGERATETEMP_LIST";


            var rec = Fetch(c => c.Query<ExchangeRateObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<ExchangeRateObj> GetExchangeRate(int ItbId, bool IsAll, bool isTemp = false, long? P_INST_ITBID = null)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_INST_ITBID", P_INST_ITBID, OracleDbType.Int64);
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_ITBID", ItbId, OracleDbType.Int32);
            p.Add(":P_ISTEMP", isTemp ? 1 : 0, OracleDbType.Int16);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_EXCHANGERATE_LIST";


            var rec = Fetch(c => c.Query<ExchangeRateObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<FrequencyObj> GetFrequencyList(int itbid, int isAll, int isTemp)
        {
            OracleDynamicParameters p = new OracleDynamicParameters();
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":P_ISALL", isAll, OracleDbType.Int16);
            p.Add(":P_ISTEMP", isTemp, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_FREQUENCY_LIST";
            var rec = Fetch(c => c.Query<FrequencyObj>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }
        public List<DropdownObject2> GetMerchantAutoComplete(string mName)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MNAME", mName, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MERCHANTLIST";

            var rec = Fetch(c => c.Query<DropdownObject2>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }
        public List<POSMISDB_MERCHANTACCT> GetMerchantAcct(string mid,string acctNo)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", mid, OracleDbType.Varchar2);
            p.Add(":P_ACCTNO", acctNo, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MID_ACCT";

            var rec = Fetch(c => c.Query<POSMISDB_MERCHANTACCT>(qry, p, commandType: CommandType.StoredProcedure), null);

            return rec.ToList();
        }

        public List<MerchantTerminalUpldObj> GetMerchantUpldRec(string batchId, string status,string location)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":P_STATUS", status, OracleDbType.Varchar2);
            p.Add(":P_LOCATION", location, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MERCHANT_UPLD";

            var rec = Fetch(c => c.Query<MerchantTerminalUpldObj>(qry, p, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<MerchantUpdateUpldObj> GetMerchantUpdateUpldRec(string batchId, string status, string location)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":P_STATUS", status, OracleDbType.Varchar2);
            p.Add(":P_LOCATION", location, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MERCHANT_UPDATE_UPLD";

            var rec = Fetch(c => c.Query<MerchantUpdateUpldObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public valObj GetMerchantUpldValidation(string val1, string val2,string val3,short label)
        {
            var p = new OracleDynamicParameters();
            p.Add(":field_val", val1, OracleDbType.Varchar2);
            p.Add(":field_val2", val2, OracleDbType.Varchar2);
            p.Add(":field_val3", val3, OracleDbType.Varchar2);
            p.Add(":field_label", label, OracleDbType.Int16);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "SELECT FUNC_MID_UPLD_VAL(:field_val,:field_val2,:field_val3,:field_label) RespCode FROM DUAL";  //@"GET_MID_ACCT";

            var rec = Fetch(c => c.Query<valObj>(qry, p, commandType: CommandType.Text), null).FirstOrDefault();

            return rec;
        }

        public valObj GetProductRuleSetOptionItbid(string merchantId, string productCode)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", merchantId, OracleDbType.Varchar2);
            p.Add(":P_PRODUCT_CODE", productCode, OracleDbType.Varchar2);
            //p.Add(":field_val3", val3, OracleDbType.Varchar2);
            //p.Add(":field_label", label, OracleDbType.Int16);
            // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "SELECT FUNC_PRODUCT_RULE_SETITBID(:P_MERCHANTID,:P_PRODUCT_CODE) SETTLEMENTOPTION_ID FROM DUAL";  //@"GET_MID_ACCT";

            var rec = Fetch(c => c.Query<valObj>(qry, p, commandType: CommandType.Text).FirstOrDefault(), null);

            return rec;
        }
        public valObj GetProductValidation(string p_field_val, string p_field_val2,int p_field_label)
        {
            var p = new OracleDynamicParameters();
            p.Add(":p_field_val", p_field_val, OracleDbType.Varchar2);
            p.Add(":p_field_val2", p_field_val2, OracleDbType.Varchar2);
            p.Add(":p_field_label", p_field_label, OracleDbType.Int32);
            //p.Add(":field_label", label, OracleDbType.Int16);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry =  @"PROC_VAL_PRODUCTUPLD";

            var rec = Fetch(c => c.Query<valObj>(qry, p, commandType: CommandType.StoredProcedure).FirstOrDefault(), null);

            return rec;
        }
        public List<DropdownObject> GetPartyInstitutioUnion(string party_type)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_PARTY_TYPE", party_type, dbType: OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            // p.Add(":MerchantId", MerchantId, OracleDbType.Varchar2);
            //string qry = @"select CODE,DESCRIPTION from 
            //                (select 'P' || '|'|| ItbId CODE , Party_desc DESCRIPTION from posmisdb_party where Lower(PartyType_Code) = 'ptsp'
            //                union all
            //                select 'I' || '|'|| ItbId CODE,institution_name DESCRIPTION from posmisdb_institution where cbn_code is not null ) A";

            string qry = "GET_PARTY_INST";
            var rec = Fetch(c => c.Query<DropdownObject>(qry, p, commandType: CommandType.StoredProcedure), null);
            return rec.ToList();
        }

        public List<MailGroupObj> GetGroupEmail( int itbid,bool IsAll,bool IsTemp,string batchId = null )
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_ISTEMP", IsTemp ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_BATCHID", batchId, OracleDbType.Varchar2);
            p.Add(":P_ITBID", itbid, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_MAILGROUP";
       

            var rec = Fetch(c => c.Query<MailGroupObj>(qry, p, buffered:false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public LastMidObj GetLastMidTidGenerated(string P_PREFIX, string P_CBNCODE, string P_ID_TYPE)
        {
          
            var p = new OracleDynamicParameters();
            p.Add(":P_PREFIX", P_PREFIX, OracleDbType.Varchar2);
            p.Add(":P_CBNCODE", P_CBNCODE , OracleDbType.Varchar2);
            p.Add(":P_ID_TYPE", P_ID_TYPE, OracleDbType.Varchar2);
          
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_LAST_MIDTID_INST";


            var rec = Fetch(c => c.Query<LastMidObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec.FirstOrDefault();
        }
        public MidTidObj GetTidDetils(string P_TERMINALID)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_TERMINALID", P_TERMINALID, OracleDbType.Varchar2);
            //p.Add(":P_CBNCODE", P_CBNCODE, OracleDbType.Varchar2);
            //p.Add(":P_ID_TYPE", P_ID_TYPE, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_TIDMID_UPDATE";


            var rec = Fetch(c => c.Query<MidTidObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec.FirstOrDefault();
        }

        public List<StateObj> GetState(int ItbId,string P_COUNTRYCODE,bool IsAll,bool isTemp = false)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_COUNTRYCODE", P_COUNTRYCODE, OracleDbType.Varchar2);
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_ITBID", ItbId, OracleDbType.Int32);
            p.Add(":P_ISTEMP", isTemp ? 1 : 0, OracleDbType.Int16);
         
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_STATE_LIST";


            var rec = Fetch(c => c.Query<StateObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<POSMISDB_CITY> GetCity(int ItbId, string P_COUNTRYCODE, string P_STATECODE, bool IsAll)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_COUNTRYCODE", P_COUNTRYCODE, OracleDbType.Varchar2);
            p.Add(":P_ISALL", IsAll ? 1 : 0, OracleDbType.Int16);
            p.Add(":P_STATECODE", P_STATECODE, OracleDbType.Varchar2);
            p.Add(":P_ITBID", ItbId, OracleDbType.Int32);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_CITY_LIST";


            var rec = Fetch(c => c.Query<POSMISDB_CITY>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<MidTidObj> GetXMLRecord(string P_BATCHID, string option = null)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":P_OPTION", option, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_XML_GENERATED";


            var rec = Fetch(c => c.Query<MidTidObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<MidTidObj> GetXMLRecord2(string P_CBNCODE)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_CBNCODE", P_CBNCODE, OracleDbType.Varchar2);
            //p.Add(":P_OPTION", option, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_XML_GENERATED_BY_ACQUIRER";


            var rec = Fetch(c => c.Query<MidTidObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }

        public decimal GetNextSeqTwcAcct(string type)
        {
            try
            {
                string sql = "";
                if (type == "ACC")
                {
                    sql = @" SELECT SEQ_TWCMSSETACCT.NEXTVAL FROM DUAL";
                }
                else
                {
                    sql = @" SELECT SEQ_TWCMSID.NEXTVAL FROM DUAL";
                }


                using (var connection = OpenConnection(null))
                // using (var transaction = connection.BeginTransaction())
                {
                    var d = connection.Query<decimal>(sql, null).FirstOrDefault();
                    //var d1 = connection.Execute(sql2, p2);
                    // transaction.Commit();
                    //if (d > 0 && d1 > 0)
                    //{
                    return d;
                    // }
                }

                //return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public int UpdateInstitutionLastAcct(string cbn_code,string last_start_acct,string last_start_twc)
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add(":p_last_start_acct", last_start_acct, OracleDbType.Varchar2);
                p.Add(":p_last_start_twc", last_start_twc, OracleDbType.Varchar2);
                p.Add(":p_cbncode", cbn_code, OracleDbType.Varchar2);
                string sql = "";
             
                    sql = @"update POSMISDB_INSTITUTION 
                            set LAST_START_ACCNO_XML = :p_last_start_acct,
                                LAST_START_TWCMSID_XML = :p_last_start_twc
                            where CBN_CODE = :p_cbncode";
              


                using (var connection = OpenConnection(null))
                // using (var transaction = connection.BeginTransaction())
                {
                    var d = connection.Execute(sql, p);
                    //var d1 = connection.Execute(sql2, p2);
                    // transaction.Commit();
                    //if (d > 0 && d1 > 0)
                    //{
                    return d;
                    // }
                }

                //return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public List<DistinctTerminalObj> GetDistinctCbnCodeTerminal()
        {
            try
            {
                //var p = new OracleDynamicParameters();
                //p.Add(":p_last_start_acct", last_start_acct, OracleDbType.Varchar2);
                //p.Add(":p_last_start_twc", last_start_twc, OracleDbType.Varchar2);
                //p.Add(":p_cbncode", cbn_code, OracleDbType.Varchar2);
                string sql = "";

                sql = @"select PP.*, START_ACCNO_XML,INSTITUTION_SHORTCODE from 
                        (
                        select distinct B.INSTITUTION_CBNCODE  from 
                        (select * from posmisdb_terminal
                        where Batchid is not null and (Generated IS NULL or Generated <> 'Y')) A
                        inner join 
                        (
                        select INSTITUTION_CBNCODE,Merchantid from POSMISDB_MERCHANTDETAIL
                        ) B
                        on A.Merchantid = B.Merchantid
                        where INSTITUTION_CBNCODE is not null
                         ) PP
                        inner join 
                        (select * from posmisdb_institution)BB
                        on PP.INSTITUTION_CBNCODE = BB.CBN_CODE";



                using (var connection = OpenConnection(null))
                // using (var transaction = connection.BeginTransaction())
                {
                    var d = connection.Query<DistinctTerminalObj>(sql, null).ToList();
                    //var d1 = connection.Execute(sql2, p2);
                    // transaction.Commit();
                    //if (d > 0 && d1 > 0)
                    //{
                    return d;
                    // }
                }

                //return 0;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public List<POSMISDB_XML_GENSCHEDULE> GetXmlGenSchedule()
        {
            try
            {
                //var p = new OracleDynamicParameters();
                //p.Add(":p_last_start_acct", last_start_acct, OracleDbType.Varchar2);
                //p.Add(":p_last_start_twc", last_start_twc, OracleDbType.Varchar2);
                //p.Add(":p_cbncode", cbn_code, OracleDbType.Varchar2);
                string sql = "";

                sql = @"select * from posmisdb_xml_genschedule
                        where status = 'ACTIVE'
                        order by SCHEDULE_TIME";



                using (var connection = OpenConnection(null))
                // using (var transaction = connection.BeginTransaction())
                {
                    var d = connection.Query<POSMISDB_XML_GENSCHEDULE>(sql, null).ToList();
                    //var d1 = connection.Execute(sql2, p2);
                    // transaction.Commit();
                    //if (d > 0 && d1 > 0)
                    //{
                    return d;
                    // }
                }

                //return 0;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public int UpdateTerminalGenerated(string P_BATCHID)
        //{

        //    var p = new OracleDynamicParameters();
        //    p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);

        //   // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
        //    string sql = @" update posmisdb_terminal set generated = 'Y'
        //                    where batchid = :P_BATCHID";


        //    var inserted = Execute(c => c.Execute(sql, p
        //           ), null);
        //    return inserted;
        //}


        //public bool UpdateTerminalGenerated(string P_BATCHID,string LastacctNo,string LasttwcNo,string CBNCODE)
        //{
        //    try
        //    {


        //        var p = new OracleDynamicParameters();
        //        p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);

        //        var p2 = new OracleDynamicParameters();
        //        p2.Add(":P_ACCNO", LastacctNo, OracleDbType.Varchar2);
        //        p2.Add(":P_TWCM", LasttwcNo, OracleDbType.Varchar2);
        //        p2.Add(":P_CBN_CODE", CBNCODE, OracleDbType.Varchar2);
        //        // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
        //        string sql = @" update posmisdb_terminal set generated = 'Y'
        //                    where batchid = :P_BATCHID";
        //        string sql2 = @" update POSMISDB_INSTITUTION set LAST_START_ACCNO_XML = :P_ACCNO,LAST_START_TWCMSID_XML = :P_TWCM
        //                        where CBN_CODE = :P_CBN_CODE";

        //        using (var connection = OpenConnection(null))
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            var d = connection.Execute(sql, p);
        //            var d1 = connection.Execute(sql2, p2);
        //            transaction.Commit();
        //            if (d > 0 && d1 > 0)
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //}

        public bool UpdateTerminalGenerated(string P_BATCHID, string LastacctNo, string LasttwcNo, string CBNCODE, string userId, bool hasConfig = false, bool isReprocess = false)
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
                p.Add(":P_USERID", userId, OracleDbType.Varchar2);
                p.Add(":P_DATE", DateTime.Now, OracleDbType.Date);
                var p2 = new OracleDynamicParameters();
                p2.Add(":P_ACCNO", LastacctNo, OracleDbType.Varchar2);
                p2.Add(":P_TWCM", LasttwcNo, OracleDbType.Varchar2);
                p2.Add(":P_CBN_CODE", CBNCODE, OracleDbType.Varchar2);
                // p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                string sql = @" update posmisdb_terminal set generated = 'Y',xml_generatedby = :P_USERID,xml_generateddate = :P_DATE
                            where batchid = :P_BATCHID";
                //string sql2 = @" update POSMISDB_INSTITUTION set LAST_START_ACCNO_XML = :P_ACCNO,LAST_START_TWCMSID_XML = :P_TWCM
                //                where CBN_CODE = :P_CBN_CODE";

                using (var connection = OpenConnection(null))
                using (var transaction = connection.BeginTransaction())
                {
                    if (!isReprocess)
                    {
                        // var d1 = connection.Execute(sql2, p2);
                        var d = connection.Execute(sql, p);
                        if (d > 0)
                        {
                            transaction.Commit();
                            return true;
                        }
                    }
                    else
                    {

                        return true;

                    }
                    //else if (hasConfig && isReprocess)
                    //{
                    //    var d1 = connection.Execute(sql2, p2);
                    //    if ( d1 > 0)
                    //    {
                    //        transaction.Commit();
                    //        return true;
                    //    }
                    //    //var d = connection.Execute(sql, p);
                    //}
                    //else  
                    //{
                    //    return true;
                    //}


                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool UpdateTerminalGenerated2(List<string> batchlist)
        {
            try
            {

                using (var connection = OpenConnection(null))
                //using (var transaction = connection.BeginTransaction())
                {
                    foreach (var d in batchlist)
                    {
                        var p = new OracleDynamicParameters();
                        p.Add(":P_BATCHID", d, OracleDbType.Varchar2);
                        //  p.Add(":P_USERID", userId, OracleDbType.Varchar2);
                        p.Add(":P_DATE", DateTime.Now, OracleDbType.Date);

                        string sql = @"update posmisdb_terminal set generated = 'Y',xml_generateddate = :P_DATE
                            where batchid = :P_BATCHID";
                        //string sql2 = @" update POSMISDB_INSTITUTION set LAST_START_ACCNO_XML = :P_ACCNO,LAST_START_TWCMSID_XML = :P_TWCM
                        //                where CBN_CODE = :P_CBN_CODE";


                        var tt = connection.Execute(sql, p);
                        //if (d > 0)
                        //{
                        //    transaction.Commit();
                        //    return true;
                        //}

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public List<SharingPartyObj> GetMerchantMsc2Detail(string P_MERCHANTID)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SHAREDMSC2DET_BY_MERCHANT";


            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingBlPartyObj> GetBillerMsc2Detail(string P_MERCHANTID,string BILLER_CODE)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BILLER_CODE", BILLER_CODE, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SHAREDMSC2DET_BY_BILLER";


            var rec = Fetch(c => c.Query<SharingBlPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingPartyObj> GetMerchantMsc1UnsharedDetail(string P_MERCHANTID)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_UNSHAREDMSC1_BY_MERCHANT";


            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingBlPartyObj> GetBillerMsc1UnsharedDetail(string P_MERCHANTID,string BILLER_CODE)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BILLER_CODE", BILLER_CODE, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_UNSHAREDMSC1_BY_BILLER";
            var rec = Fetch(c => c.Query<SharingBlPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingPartyObj> GetMerchantSubsidyDetail(string P_MERCHANTID)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);

            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SUBSIDY_BY_MERCHANT";


            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }

        public List<SharingBlPartyObj> GetBillerSubsidyDetail(string P_MERCHANTID,string BILLER_CODE)
        {

            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BILLER_CODE", BILLER_CODE, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SUBSIDY_BY_BILLER";


            var rec = Fetch(c => c.Query<SharingBlPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingPartyObj> GetMerchantMsc2DetailTemp(string P_MERCHANTID,string P_BATCHID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SHAREDMSC2DET_BY_MIDTEMP";


            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingBlPartyObj> GetBillerMsc2DetailTemp(string P_MERCHANTID, string P_BATCHID,string P_USERID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":P_USERID", P_USERID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SHAREDBLMSC2_BY_MIDTEMP";


            var rec = Fetch(c => c.Query<SharingBlPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingPartyObj> GetMerchantMsc1UnsharedDetailTemp(string P_MERCHANTID,string P_BATCHID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_UNSHAREDMSC1_BY_MIDTEMP";


            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingBlPartyObj> GetBillerMsc1UnsharedDetailTemp(string P_MERCHANTID, string P_BATCHID,string p_userid)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":P_USERID", p_userid, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_UNSHAREDMSC1_BL_MIDTEMP";
            var rec = Fetch(c => c.Query<SharingBlPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingPartyObj> GetMerchantSubsidyDetailTemp(string P_MERCHANTID, string P_BATCHID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SUBSIDY_BY_MIDTEMP";


            var rec = Fetch(c => c.Query<SharingPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public List<SharingBlPartyObj> GetBillerSubsidyDetailTemp(string P_MERCHANTID, string P_BATCHID,string P_USERID)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_MERCHANTID", P_MERCHANTID, OracleDbType.Varchar2);
            p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":P_USERID", P_USERID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = "GET_SUBSIDY_Bl_MIDTEMP";


            var rec = Fetch(c => c.Query<SharingBlPartyObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }
        public POSMISDB_INSTITUTION GetInstitutionName(long P_InstItbid)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_INSTITBID", P_InstItbid, OracleDbType.Int64);
            //p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            //p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"select institution_name, itbid from posmisdb_institution
                           where itbid = :P_INSTITBID";


            var rec = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qry, p, buffered: false, commandType: CommandType.Text).FirstOrDefault(), null);

            return rec;
        }
        public List<MerchantObj2> GetMerchantRecord(string P_Q,string P_LABEL, string cbn_code = null)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_Q", P_Q, OracleDbType.Varchar2);
            p.Add(":P_LABEL", P_LABEL, OracleDbType.Varchar2);
            p.Add(":P_CBNCODE", cbn_code, OracleDbType.Varchar2);
            //p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_MERCHANTBY_NAME_MID2";
      

            var rec = Fetch(c => c.Query<MerchantObj2>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).ToList(), null);

            return rec;
        }

        public InstitutionObj GetINSTITUTION_BY_CBNCODE(string P_CBNCODE)
        {
            var p = new OracleDynamicParameters();
            p.Add(":P_CBNCODE", P_CBNCODE, OracleDbType.Varchar2);
           // p.Add(":P_LABEL", P_LABEL, OracleDbType.Varchar2);
            //p.Add(":P_BATCHID", P_BATCHID, OracleDbType.Varchar2);
            p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            string qry = @"GET_INSTITUTION_BY_CBNCODE";


            var rec = Fetch(c => c.Query<InstitutionObj>(qry, p, buffered: false, commandType: CommandType.StoredProcedure).FirstOrDefault(), null);

            return rec;
        }

        public List<TerminalObj> GetTerminalToDelete(string[]  tidList)
        {
            try
            {
                using (var connection = OpenConnection(null))
                // using (var transaction = connection.BeginTransaction())
                {
                //    var p = new OracleDynamicParameters();
                //p.Add(":p_last_start_acct", last_start_acct, OracleDbType.Varchar2);
                //p.Add(":p_last_start_twc", last_start_twc, OracleDbType.Varchar2);
                //p.Add(":p_cbncode", cbn_code, OracleDbType.Varchar2);
                string sql = "";
                    var concat = "'";
                    foreach (var t in tidList)
                    {
                        concat += t + "','";
                    }
                    concat = concat.Substring(0, concat.Length - 2);
                    sql =string.Format( @"select merchantid,terminalid from posmisdb_terminal
                        where terminalid in ({0})",concat);
                    var d = connection.Query<TerminalObj>(sql, null);
                    //var d1 = connection.Execute(sql2, p2);
                    // transaction.Commit();
                    //if (d > 0 && d1 > 0)
                    //{
                    return d.ToList();
                    // }
                }

                //return 0;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion


        #region DapperCallViaProcedure
        public object LoadProcConString(int? BankFiid, bool IsAll, bool buffered = false, string connectionString = null)
        {

            var p = new DynamicParameters();

            p.Add("@BankFiid", BankFiid, DbType.String, null);

            p.Add("@IsAll", IsAll, DbType.Boolean, null);
            var rec = _repoConfigProc.LoadViaStoreProc("", p, buffered, connectionString);
            // var gh = DatasetHelper.ToDataSet<bankconfig>(rec);
            return rec;
        }
        #endregion

      
    }
}
