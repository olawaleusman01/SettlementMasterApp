using Dapper;
using Generic.Dapper.Model;
using Generic.Dapper.ReportClass;
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
    public static class Naps
    {
        public static int PostNaps(NapsObj obj, int postType)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("DEBITACCTNO", obj.DEBITACCTNO, DbType.String);
            p.Add("DEBITBANKCODE", obj.DEBITBANKCODE, DbType.String);
            p.Add("BENEFICIARYACCTNO", obj.BENEFICIARYACCTNO, DbType.String);
            p.Add("BENEFICIARYBANKCODE", obj.BENEFICIARYBANKCODE, DbType.String);
            p.Add("BENEFICIARYNAME", obj.BENEFICIARYNAME, DbType.String);
            p.Add("BENEFICIARYNARRATION", obj.BENEFICIARYNARRATION, DbType.String);
            p.Add("CREDITAMOUNT", obj.CREDITAMOUNT, DbType.Decimal);
            p.Add("SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.Date);
            p.Add("REASON", obj.REASON, DbType.String);
            p.Add("REQUESTTYPE", obj.REQUESTTYPE, DbType.String);
            p.Add("USERID", obj.USERID, DbType.String);
            p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
            p.Add("BATCHID", obj.BATCHID, DbType.String);
            p.Add("PID", obj.PID, DbType.String);
            p.Add("POSTTYPE", postType, DbType.String);
            p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
            p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);
            p.Add("PostSequence", null, DbType.Int32);
            p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
            p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.Boolean);
            p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var rst = con.Execute("SESS_POST_NAPS", p, commandType: CommandType.StoredProcedure);
                return rst;
            }
        }
        public static int PostNapsBulk(List<NapsObj> objList, string userId)
        {
            var curDate = DateTime.Now;
            //OutPutObj ret = new OutPutObj();
            var cnt = 0;
            var p = new DynamicParameters();
            using (var con = new RepoBase().OpenConnection(null))
            {
                //PURGE ALL RECORD FOR CURRENT USER
                int sqn = 0;
                p.Add("USERID", userId, DbType.String);
                var t2 = con.Execute("SESS_PURGE_NAPS", p, commandType: CommandType.StoredProcedure);
                foreach (var obj in objList)
                {
                    ValidateUpload(obj);
                    p.Add("DEBITACCTNO", obj.DEBITACCTNO, DbType.String);
                    p.Add("DEBITBANKCODE", obj.DEBITBANKCODE, DbType.String);
                    p.Add("BENEFICIARYACCTNO", obj.BENEFICIARYACCTNO, DbType.String);
                    p.Add("BENEFICIARYBANKCODE", obj.BENEFICIARYBANKCODE, DbType.String);
                    p.Add("BENEFICIARYNAME", obj.BENEFICIARYNAME, DbType.String);
                    p.Add("BENEFICIARYNARRATION", obj.BENEFICIARYNARRATION, DbType.String);
                    p.Add("CREDITAMOUNT", obj.CREDITAMOUNT, DbType.Decimal);
                    p.Add("SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.Date);
                    p.Add("REASON", obj.REASON, DbType.String);
                    p.Add("REQUESTTYPE", obj.REQUESTTYPE, DbType.String);
                    p.Add("USERID", userId, DbType.String);
                    p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
                    p.Add("BATCHID", obj.BATCHID, DbType.String);
                    p.Add("PID", obj.PID, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.String);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.Boolean);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("PostSequence", sqn++, DbType.Int32);
                    p.Add("EVENTTYPE", obj.EVENTTYPE, DbType.String);
                    p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);

                    cnt += con.Execute("SESS_POST_NAPS", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
        public static OutPutObj GenerateNaps(DateTime sett_date,int channelid,int ID,string bid,string reqType,string userId)
        {
            var cnt = 0;
            var rowCnt = 0;
            var curDate = DateTime.Now;
            DynamicParameters p;
            using (var con = new RepoBase().OpenConnection(null))
            {
                p = new DynamicParameters();
                p.Add("ReqType", reqType, DbType.String);
                p.Add("SetDate", sett_date, DbType.Date); 
                var tt = con.Query<OutPutObj>("proc_ValidateNapsNibss", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (tt.RespCode == 1)
                {
                    return tt;
                }
                // var dtMain = rptSett.generateDS("", "", "NIBSS_ALL", "U", "NAPS", sett_date.ToString("yyyy-MM-dd"), null, null,null);
                dsLoadclass dv = new dsLoadclass();
               
                if (ID == 1)
                {
                    var dtMain = dv.generateDS("", "", "NIBSS_ALL", "U", "NAPS", sett_date.ToString("yyyy-MM-dd"), channelid);
                    p = new DynamicParameters();
                    p.Add("USERID", userId, DbType.String);
                    var t2 = con.Execute("SESS_PURGE_NAPS", p, commandType: CommandType.StoredProcedure);
                    int sqn = 0;
                    rowCnt = dtMain.Rows.Count;
                    foreach (DataRow dsrow in dtMain.Rows)
                    {
                        sqn++;
                        p = new DynamicParameters();
                        NapsObj obj = new NapsObj()
                        {
                            DEBITACCTNO = dsrow[0].ToString(),
                            DEBITBANKCODE = dsrow[1].ToString(),
                            BENEFICIARYNAME = dsrow[2].ToString(),
                            BENEFICIARYACCTNO = dsrow[3].ToString(),
                            BENEFICIARYBANKCODE = dsrow[4].ToString(),
                            CREDITAMOUNT = decimal.Parse(dsrow[5].ToString()),
                            BENEFICIARYNARRATION = dsrow[6].ToString(),
                            BATCHID = bid,
                            CREATEDATE = curDate,
                            REQUESTTYPE = reqType,
                            SETTLEMENTDATE = sett_date,
                            USERID = userId,
                            MERCHANTID = dsrow[7].ToString(),//TAKE NOTE
                        };
                        p.Add("DEBITACCTNO", obj.DEBITACCTNO, DbType.String);
                        p.Add("DEBITBANKCODE", obj.DEBITBANKCODE, DbType.String);
                        p.Add("BENEFICIARYACCTNO", obj.BENEFICIARYACCTNO, DbType.String);
                        p.Add("BENEFICIARYBANKCODE", obj.BENEFICIARYBANKCODE, DbType.String);
                        p.Add("BENEFICIARYNAME", obj.BENEFICIARYNAME, DbType.String);
                        p.Add("BENEFICIARYNARRATION", obj.BENEFICIARYNARRATION, DbType.String);
                        p.Add("CREDITAMOUNT", obj.CREDITAMOUNT, DbType.Decimal);
                        p.Add("SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.Date);
                        p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);
                        p.Add("REQUESTTYPE", obj.REQUESTTYPE, DbType.String);
                        p.Add("USERID", obj.USERID, DbType.String);
                        p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
                        p.Add("BATCHID", obj.BATCHID, DbType.String);
                        p.Add("REASON", obj.REASON, DbType.String);
                        p.Add("PID", obj.PID, DbType.String);
                        p.Add("POSTTYPE", 1, DbType.String);
                        p.Add("EVENTTYPE", "New", DbType.String);
                        p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                        p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                        p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                        p.Add("PostSequence", sqn++, DbType.Int32);
                        cnt += con.Execute("SESS_POST_NAPS", p, commandType: CommandType.StoredProcedure);

                    }
                }
                else
                {
                    var dtMain = dv.generateDS("", "", "NIBSS_ALL", "U", "NEFT", sett_date.ToString("yyyy-MM-dd"), channelid);

                    p = new DynamicParameters();
                    p.Add("USERID", userId, DbType.String);
                    var t2 = con.Execute("SESS_PURGE_NAPS", p, commandType: CommandType.StoredProcedure);
                    int sqn = 0;
                    rowCnt = dtMain.Rows.Count;
                    foreach (DataRow dsrow in dtMain.Rows)
                    {
                        sqn++;
                        p = new DynamicParameters();
                        NapsObj obj = new NapsObj()
                        {
                            DEBITACCTNO = dsrow[0].ToString(),
                            DEBITBANKCODE = dsrow[1].ToString(),
                            BENEFICIARYNAME = dsrow[2].ToString(),
                            BENEFICIARYACCTNO = dsrow[3].ToString(),
                            BENEFICIARYBANKCODE = dsrow[4].ToString(),
                            CREDITAMOUNT = decimal.Parse(dsrow[5].ToString()),
                            BENEFICIARYNARRATION = dsrow[6].ToString(),
                            BATCHID = bid,
                            CREATEDATE = curDate,
                            REQUESTTYPE = reqType,
                            SETTLEMENTDATE = sett_date,
                            USERID = userId,
                            MERCHANTID = dsrow[7].ToString(),//TAKE NOTE
                        };
                        p.Add("DEBITACCTNO", obj.DEBITACCTNO, DbType.String);
                        p.Add("DEBITBANKCODE", obj.DEBITBANKCODE, DbType.String);
                        p.Add("BENEFICIARYACCTNO", obj.BENEFICIARYACCTNO, DbType.String);
                        p.Add("BENEFICIARYBANKCODE", obj.BENEFICIARYBANKCODE, DbType.String);
                        p.Add("BENEFICIARYNAME", obj.BENEFICIARYNAME, DbType.String);
                        p.Add("BENEFICIARYNARRATION", obj.BENEFICIARYNARRATION, DbType.String);
                        p.Add("CREDITAMOUNT", obj.CREDITAMOUNT, DbType.Decimal);
                        p.Add("SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.Date);
                        p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);
                        p.Add("REQUESTTYPE", obj.REQUESTTYPE, DbType.String);
                        p.Add("USERID", obj.USERID, DbType.String);
                        p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
                        p.Add("BATCHID", obj.BATCHID, DbType.String);
                        p.Add("REASON", obj.REASON, DbType.String);
                        p.Add("PID", obj.PID, DbType.String);
                        p.Add("POSTTYPE", 1, DbType.String);
                        p.Add("EVENTTYPE", "New", DbType.String);
                        p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                        p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                        p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                        p.Add("PostSequence", sqn++, DbType.Int32);
                        cnt += con.Execute("SESS_POST_NAPS", p, commandType: CommandType.StoredProcedure);

                    }
                }

                OutPutObj ret = new OutPutObj();
               
//==========================================
              
                if (rowCnt == 0)
                {

                    ret = new OutPutObj()
                    {
                        RespCode = 1,
                        RespMessage = "No Record found for selected date."
                    };
                }
                else if (cnt == rowCnt)
                {

                    ret = new OutPutObj()
                    {
                        RespCode = 0,
                        RespMessage = ""
                    };
                }
                else
                {
                    ret = new OutPutObj()
                    {
                        RespCode = 1,
                        RespMessage = ""
                    };
                }
                return ret;
            }
        }

        public static OutPutObj GenerateNaps2(DateTime sett_date, int channelid, string bid, string reqType, string userId)
        {
            var cnt = 0;
            var rowCnt = 0;
            var curDate = DateTime.Now;
            DynamicParameters p;
            using (var con = new RepoBase().OpenConnection(null))
            {
                p = new DynamicParameters();
                p.Add("ReqType", reqType, DbType.String);
                p.Add("SetDate", sett_date, DbType.Date);
                var tt = con.Query<OutPutObj>("proc_ValidateNapsNibss", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (tt.RespCode == 1)
                {
                    return tt;
                }
                // var dtMain = rptSett.generateDS("", "", "NIBSS_ALL", "U", "NAPS", sett_date.ToString("yyyy-MM-dd"), null, null,null);
                dsLoadclass dv = new dsLoadclass();
                var dtMain = dv.generateDS("", "", "NIBSS_ALL", "U", "NEFT", sett_date.ToString("yyyy-MM-dd"), channelid);

                OutPutObj ret = new OutPutObj();

                //==========================================
                p = new DynamicParameters();
                p.Add("USERID", userId, DbType.String);
                var t2 = con.Execute("SESS_PURGE_NAPS", p, commandType: CommandType.StoredProcedure);
                int sqn = 0;
                rowCnt = dtMain.Rows.Count;
                foreach (DataRow dsrow in dtMain.Rows)
                {
                    sqn++;
                    p = new DynamicParameters();
                    NapsObj obj = new NapsObj()
                    {
                        DEBITACCTNO = dsrow[0].ToString(),
                        DEBITBANKCODE = dsrow[1].ToString(),
                        BENEFICIARYNAME = dsrow[2].ToString(),
                        BENEFICIARYACCTNO = dsrow[3].ToString(),
                        BENEFICIARYBANKCODE = dsrow[4].ToString(),
                        CREDITAMOUNT = decimal.Parse(dsrow[5].ToString()),
                        BENEFICIARYNARRATION = dsrow[6].ToString(),
                        BATCHID = bid,
                        CREATEDATE = curDate,
                        REQUESTTYPE = reqType,
                        SETTLEMENTDATE = sett_date,
                        USERID = userId,
                        MERCHANTID = dsrow[7].ToString(),//TAKE NOTE
                    };
                    p.Add("DEBITACCTNO", obj.DEBITACCTNO, DbType.String);
                    p.Add("DEBITBANKCODE", obj.DEBITBANKCODE, DbType.String);
                    p.Add("BENEFICIARYACCTNO", obj.BENEFICIARYACCTNO, DbType.String);
                    p.Add("BENEFICIARYBANKCODE", obj.BENEFICIARYBANKCODE, DbType.String);
                    p.Add("BENEFICIARYNAME", obj.BENEFICIARYNAME, DbType.String);
                    p.Add("BENEFICIARYNARRATION", obj.BENEFICIARYNARRATION, DbType.String);
                    p.Add("CREDITAMOUNT", obj.CREDITAMOUNT, DbType.Decimal);
                    p.Add("SETTLEMENTDATE", obj.SETTLEMENTDATE, DbType.Date);
                    p.Add("MERCHANTID", obj.MERCHANTID, DbType.String);
                    p.Add("REQUESTTYPE", obj.REQUESTTYPE, DbType.String);
                    p.Add("USERID", obj.USERID, DbType.String);
                    p.Add("CREATEDATE", obj.CREATEDATE, DbType.DateTime);
                    p.Add("BATCHID", obj.BATCHID, DbType.String);
                    p.Add("REASON", obj.REASON, DbType.String);
                    p.Add("PID", obj.PID, DbType.String);
                    p.Add("POSTTYPE", 1, DbType.String);
                    p.Add("EVENTTYPE", "New", DbType.String);
                    p.Add("@VALIDATIONERRORMESSAGE", obj.VALIDATIONERRORMESSAGE, DbType.String);
                    p.Add("@VALIDATIONERRORSTATUS", obj.VALIDATIONERRORSTATUS, DbType.String);
                    p.Add("@WARNINGMESSAGE", obj.WARNINGMESSAGE, DbType.String);
                    p.Add("PostSequence", sqn++, DbType.Int32);
                    cnt += con.Execute("SESS_POST_NAPS", p, commandType: CommandType.StoredProcedure);

                }
                if (rowCnt == 0)
                {

                    ret = new OutPutObj()
                    {
                        RespCode = 1,
                        RespMessage = "No Record found for selected date."
                    };
                }
                else if (cnt == rowCnt)
                {

                    ret = new OutPutObj()
                    {
                        RespCode = 0,
                        RespMessage = ""
                    };
                }
                else
                {
                    ret = new OutPutObj()
                    {
                        RespCode = 1,
                        RespMessage = ""
                    };
                }
                return ret;
            }
        }
        public static List<NapsObj> GetNaps(string userId,string batchId)
        {
            // OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("BATCHID", batchId, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<NapsObj>("SESS_GET_NAPS", p, commandType: CommandType.StoredProcedure).ToList();
                return ret;
            }

        }
        public static NapsObj FindNaps(string Id, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var p = new DynamicParameters();
            p.Add("USERID", userId, DbType.String);
            p.Add("ID", Id, DbType.String);
            using (var con = new RepoBase().OpenConnection(null))
            {
                var ret = con.Query<NapsObj>("SESS_FIND_NAPS", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return ret;
            }

        }
        static NapsObj ValidateUpload(NapsObj t)
        {

            // List<MerchantUpldObj> lst = new List<MerchantUpldObj>();

            //var rec = Naps.GetNaps(User.Identity.Name,null);

            int totalErrorCount = 0;
            //foreach (var t in rec)
            //{
            int errorCount = 0;
            var validationErrorMessage = new List<string>();
            decimal mid;
            //int specialCount = 0;
            if (!decimal.TryParse(t.DEBITBANKCODE, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITBANKCODE must be number"));
            }
            if (t.DEBITBANKCODE.Length != 3)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITBANKCODE must be {0} Character", 3));

            }
            if (!decimal.TryParse(t.DEBITACCTNO, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITACCTNO must be number"));
            }
            if (t.DEBITACCTNO.Length != 10)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("DEBITACCTNO must be {0} Character", 10));

            }
            if (!decimal.TryParse(t.BENEFICIARYBANKCODE, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYBANKCODE must be number"));
            }
            if (t.BENEFICIARYBANKCODE.Length != 3)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYBANKCODE must be {0} Character", 3));

            }
            if (!decimal.TryParse(t.BENEFICIARYACCTNO, out mid))
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYACCTNO must be number"));
            }
            if (t.BENEFICIARYACCTNO.Length != 10)
            {
                errorCount++;
                //  totalErrorCount++;
                validationErrorMessage.Add(string.Format("BENEFICIARYACCTNO must be {0} Character", 10));

            }

            if (errorCount == 0)
            {
                t.VALIDATIONERRORSTATUS = false;
                t.VALIDATIONERRORMESSAGE = "";
            }
            else
            {
                totalErrorCount++;
                t.VALIDATIONERRORSTATUS = true;
                t.VALIDATIONERRORMESSAGE = SmartObj.GetStringFromList(validationErrorMessage);
            }
            //var rst = Naps.PostNaps(t, 2);
            //SessionHelper.GetCart(Session).UpdateItem(t);
            // }

            //  lst.AddRange(lst);
            //if (rec.Count > 0)
            //{
            //    if (totalErrorCount > 0)
            //    {

            //        //pnlResponse.Visible = true;
            //        //pnlResponse.CssClass = "alert alert-danger alert-dismissable alert-bold";
            //        //pnlResponseMsg.Text = string.Format("{0} Record(s) Failed Validation from Batch...", totalErrorCount);
            //        //if (totalErrorCount == rec.Count)
            //        //{
            //        //    btnProcess.Enabled = false;
            //        //}
            //        //else
            //        //{
            //        //    btnProcess.Enabled = true;
            //        //}

            //    }
            //    else
            //    {
            //        //pnlResponse.Visible = true;
            //        //pnlResponse.CssClass = "alert alert-success alert-dismissable alert-bold";
            //        //pnlResponseMsg.Text = "Batch Validated Successfully...You can now save for further processing";
            //        // btnProcess.Enabled = false;
            //        //btnProcess.Enabled = true;
            //    }
            //}
            return t;
            //}
            //catch (Exception ex)
            //{
            //    return -1;
            //}
        }
        public static int PostNapsReprocess(decimal[] objList, string userId)
        {
            //OutPutObj ret = new OutPutObj();
            var cnt = 0;
            var p = new DynamicParameters();
            using (var con = new RepoBase().OpenConnection(null))
            {
                //PURGE ALL RECORD FOR CURRENT USER
              
                foreach (var obj in objList)
                {
                    p.Add("@P_ITBID", obj, DbType.Decimal);

                    cnt += con.Execute("proc_PostNapsReprocess", p, commandType: CommandType.StoredProcedure);
                }
                return cnt;
            }
        }
    }
}
