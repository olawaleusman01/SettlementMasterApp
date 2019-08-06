using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPPosMaster.Dapper.Data;
using UPPosMaster.Data;
using UPPosMaster.Dapper.ExcelUtility;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
//using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using UPPosMaster.Dapper.Model;
using System.Threading;
using UPPosMaster.Dapper.ReportClass;
using System.IO;
using Generic.Dapper.Repository;
using Generic.Dapper.Model;
using Dapper;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Generic.Dapper.Data
{
    public class MEBCollectionClass
    {
        private static string sourceDb = ConfigurationManager.AppSettings["SOURCE_DB"].ToString().Trim();
        private static string destDb = ConfigurationManager.AppSettings["DEST_DB"].ToString().Trim();

       // private static string oradb2 = ""; // System.Configuration.ConfigurationManager.AppSettings["MEBConfig"].ToString().Trim();
        //private static string oradb = ""; // System.Configuration.ConfigurationManager.AppSettings["POSMISDB"].ToString().Trim();
        //private static string purgedays = ""; // System.Configuration.ConfigurationManager.AppSettings["purgedays"].ToString().Trim();
        private static string processdate = ""; // System.Configuration.ConfigurationManager.AppSettings["processdate"].ToString().Trim();
        private static string recordCount = ""; // System.Configuration.ConfigurationManager.AppSettings["recordCount"].ToString().Trim();
        string reportPath = ""; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) : string.Empty;
        string logopath = ""; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;
        string TWCMSPATH = ""; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TWCMSPATH"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["TWCMSPATH"]) : string.Empty;
        string sourcepath = ""; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["sourcepath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["sourcepath"]) : string.Empty;
        string targetpath = ""; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["targetpath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["targetpath"]) : string.Empty;

        //private readonly IDapperSettlementReport _repoSETT = new DapperSettlementReport();
        LogFunction lgfn = new LogFunction();
        
        static string res = "ORA-00001: unique constraint";

        public void SettlementProcessNew(string day)
        {
            using (var con = new RepoBase().OpenConnection(destDb))
            {
                var pstatus = Process_State(con);
                if (pstatus == 1)
                {
                    try
                    {
                        string SqlString3 = @"Update sm_company_profile set PROCESS_FLAG='P'";
                        con.Execute(SqlString3, commandType: CommandType.Text);
                    }
                    catch
                    {

                    }

                    lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Process Initializing Phase 1", null, null);
                    var random = new Random((int)DateTime.Now.Ticks);
                    var randomValue = random.Next(1000000, 9999999);
                    string batchno = randomValue.ToString();

                    try
                    {
                        //if (oldCode == string.Empty)
                        //{
                        //    //var td = LastRecord();
                        //    //if (td != null)
                        //    //{
                        //    //    try
                        //    //    {
                        //    //        Lastcode = td.Code ?? 0;
                        //    //        LastDocno = td.Docno ?? 0;

                        //    //        oldCode = Lastcode.ToString();
                        //    //        oldDocNo = LastDocno.ToString();
                        //    //    }
                        //    //    catch (Exception ex)
                        //    //    {

                        //    //    }
                        //    //}
                        //}

                        try
                        {

                            multitreadOpr(day,con);



                            //new SettlementProcess_MEB().SETTProcess(batchno, opdate.ToString("dd-MMM-yyyy"));
                            //var multitread = multitreadOpr(oldCode, oldDocNo);
                            ////var cts = new CancellationTokenSource();
                            ////try
                            ////{
                            ////    Parallel.Invoke(
                            ////        new ParallelOptions { CancellationToken = cts.Token },
                            ////        () => {
                            ////            lgfn.loginfoMSG("Starting 1st Process Task:" + eT2, null, DateTime.Now.ToString());
                            ////            multitreadOpr(day);
                            ////        },
                            ////        () => {
                            ////            lgfn.loginfoMSG("Starting 2nd Process Task:" + eT2, null, DateTime.Now.ToString());
                            ////            multitreadOpr(day);
                            ////        },
                            ////        () => {
                            ////            lgfn.loginfoMSG("Starting 3rd Process Task:" + eT2, null, DateTime.Now.ToString());
                            ////            multitreadOpr(day);
                            ////        }
                            ////    );
                            ////}
                            ////catch (AggregateException e)
                            ////{
                            ////    var cause = e.InnerExceptions[0];
                            ////    lgfn.loginfoMSG("Error From Parallel Task:" + cause, null, DateTime.Now.ToString());
                            ////    // Check if cause is a PersonIsNotVegetarianException.
                            ////}

                            ////cts.Dispose();

                            ////Parallel.Invoke(() =>
                            ////                            {
                            ////                                lgfn.loginfoMSG("Starting 1st Process Task:" + eT2, null, DateTime.Now.ToString());
                            ////                                multitreadOpr(day);
                            ////                            },  // close first Action

                            ////                                () =>
                            ////                                {
                            ////                                    lgfn.loginfoMSG("Starting 2nd Process Task:" + eT2, null, DateTime.Now.ToString());
                            ////                                      multitreadOpr(day);
                            ////                                }, //close second Action

                            ////                                () =>
                            ////                                {
                            ////                                    lgfn.loginfoMSG("Starting 3rd Process Task:" + eT2, null, DateTime.Now.ToString());
                            ////                                           multitreadOpr(day);
                            ////                                } //close third Action
                            ////                            ); //close parallel.invoke




                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Processing Completed", null, null);
                        }
                        catch (Exception ex)
                        {
                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);

                            string SqlString4 = @"Update sm_company_profile set PROCESS_FLAG='A'";
                            
                            con.Execute(SqlString4,commandType:CommandType.Text);
                        }
                    }

                    catch (Exception ex)
                    {
                        lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());


                    }


                    //FILE GENERATING

                    //if(hasRec==true )
                    //{

                    //////try
                    //////{
                    //////    oracommand_purge.CommandText = @"INSERT INTO POSMISDB_SETTLEMENTDETAIL Select * from posmisdb_settlementdetail@whvalu WHERE OPDATE>='" + opdate + "'";
                    //////    oracommand_purge.CommandType = CommandType.Text;

                    //////    oracommand_purge.ExecuteNonQuery();
                    //////}
                    //////catch (Exception ex)
                    //////{

                    //////}



                    //DateTime SETTDATE = DateTime.Today;
                    //DateTime NEW_SETTDATE = DateTime.Today;

                    //if (SETTDATE.DayOfWeek == DayOfWeek.Saturday)
                    //{

                    //    NEW_SETTDATE = SETTDATE.AddDays(2);
                    //}
                    //else if (SETTDATE.DayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    NEW_SETTDATE = SETTDATE.AddDays(1);
                    //}
                    //else
                    //{
                    //    NEW_SETTDATE = DateTime.Today;

                    //}

                    //bool sett;


                    //DateTime scheduledTime = DateTime.MinValue;
                    //DateTime scheduledTimeEND = DateTime.MinValue;
                    //scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                    //scheduledTimeEND = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTimeEnd"]);


                    //if (NEW_SETTDATE == SETTDATE)
                    //{

                    //    if ((DateTime.Now > scheduledTime) && (DateTime.Now < scheduledTimeEND))
                    //    {
                    //        lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Report Initializing", null, null);

                    //        lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Report Completed", null, null);

                    //    }

                    //    try
                    //    {
                    //        string SqlString3 = @"Update sm_company_profile set PROCESS_FLAG='C',OPDATE='" + NEW_SETTDATE.AddDays(-2).ToString("dd-MM-yyyy") + "',SETTDATE='" + NEW_SETTDATE.ToString("dd-MM-yyyy") + "',CPDDATE='" + NEW_SETTDATE.ToString("dd-MM-yyyy") + "'";
                    //        con.Execute(SqlString3,commandType:CommandType.Text);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        string SqlString3B = @"Update posmisdb_company_profile set PROCESS_FLAG='C'";
                    //        con.Execute(SqlString3B, commandType: CommandType.Text);
                    //        lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);

                    //    }
                    //}
                }
            }
        }
        //public void SettlementProcess(string day)
        //{

        //    string oldCode = string.Empty;
        //    string oldDocNo = string.Empty;

        //    try
        //    {
        //        oldCode = Lastcode.ToString();
        //        oldDocNo = LastDocno.ToString();
        //    }
        //    catch
        //    {

        //    }
        //    Stopwatch se = new Stopwatch();
        //    se.Start();
        //    OracleConnection Misportal_connection = default(OracleConnection);
        //    OracleCommand oracommand = default(OracleCommand);
        //    OracleCommand oracommand_purge = default(OracleCommand);
        //    // OracleCommand _with2 = default(OracleCommand);
        //    string constr = oradb;
        //    lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Process Initializing Phase 1" , null, null);
        //    Misportal_connection = new OracleConnection(constr);
        //    Misportal_connection.Open();
        //    oracommand_purge = new OracleCommand();
        //    oracommand_purge.Connection = Misportal_connection;
        //    oracommand_purge.CommandType = CommandType.Text;
        //    var pstatus = Process_State();
        //     if (pstatus == 1)
        //    {
        //        try
        //        {
        //            string SqlString3 = @"Update posmisdb_company_profile set PROCESS_FLAG='P'";
        //            oracommand_purge.CommandText = SqlString3;
        //            oracommand_purge.ExecuteNonQuery();
        //        }
        //        catch
        //        {

        //        }



        //        //FILE PROCESSING


        //        lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Process Initializing Phase 1", null, null);
        //        var random = new Random((int)DateTime.Now.Ticks);
        //        var randomValue = random.Next(1000000, 9999999);
        //        string batchno = randomValue.ToString();


        //        //DateTime opdate = DateTime.Today;

        //        try
        //        {
        //            //if (oldCode == string.Empty)
        //            //{
        //            //    //var td = LastRecord();
        //            //    //if (td != null)
        //            //    //{
        //            //    //    try
        //            //    //    {
        //            //    //        Lastcode = td.Code ?? 0;
        //            //    //        LastDocno = td.Docno ?? 0;

        //            //    //        oldCode = Lastcode.ToString();
        //            //    //        oldDocNo = LastDocno.ToString();
        //            //    //    }
        //            //    //    catch (Exception ex)
        //            //    //    {

        //            //    //    }
        //            //    //}
        //            //}

        //            try
        //            {

        //                multitreadOpr(day);

        //                //new SettlementProcess_MEB().SETTProcess(batchno, opdate.ToString("dd-MMM-yyyy"));
        //                //var multitread = multitreadOpr(oldCode, oldDocNo);
        //                ////var cts = new CancellationTokenSource();
        //                ////try
        //                ////{
        //                ////    Parallel.Invoke(
        //                ////        new ParallelOptions { CancellationToken = cts.Token },
        //                ////        () => {
        //                ////            lgfn.loginfoMSG("Starting 1st Process Task:" + eT2, null, DateTime.Now.ToString());
        //                ////            multitreadOpr(day);
        //                ////        },
        //                ////        () => {
        //                ////            lgfn.loginfoMSG("Starting 2nd Process Task:" + eT2, null, DateTime.Now.ToString());
        //                ////            multitreadOpr(day);
        //                ////        },
        //                ////        () => {
        //                ////            lgfn.loginfoMSG("Starting 3rd Process Task:" + eT2, null, DateTime.Now.ToString());
        //                ////            multitreadOpr(day);
        //                ////        }
        //                ////    );
        //                ////}
        //                ////catch (AggregateException e)
        //                ////{
        //                ////    var cause = e.InnerExceptions[0];
        //                ////    lgfn.loginfoMSG("Error From Parallel Task:" + cause, null, DateTime.Now.ToString());
        //                ////    // Check if cause is a PersonIsNotVegetarianException.
        //                ////}

        //                ////cts.Dispose();

        //                ////Parallel.Invoke(() =>
        //                ////                            {
        //                ////                                lgfn.loginfoMSG("Starting 1st Process Task:" + eT2, null, DateTime.Now.ToString());
        //                ////                                multitreadOpr(day);
        //                ////                            },  // close first Action

        //                ////                                () =>
        //                ////                                {
        //                ////                                    lgfn.loginfoMSG("Starting 2nd Process Task:" + eT2, null, DateTime.Now.ToString());
        //                ////                                      multitreadOpr(day);
        //                ////                                }, //close second Action

        //                ////                                () =>
        //                ////                                {
        //                ////                                    lgfn.loginfoMSG("Starting 3rd Process Task:" + eT2, null, DateTime.Now.ToString());
        //                ////                                           multitreadOpr(day);
        //                ////                                } //close third Action
        //                ////                            ); //close parallel.invoke




        //                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Processing Completed", null, null);
        //            }
        //            catch (Exception ex)
        //            {
        //                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);

        //                string SqlString4 = @"Update posmisdb_company_profile set PROCESS_FLAG='A'";
        //                oracommand_purge.CommandText = SqlString4;
        //                oracommand_purge.ExecuteNonQuery();
        //            }
        //        }

        //        catch (Exception ex)
        //        {
        //            lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());


        //        }


        //        //FILE GENERATING

        //        //if(hasRec==true )
        //        //{

        //        //////try
        //        //////{
        //        //////    oracommand_purge.CommandText = @"INSERT INTO POSMISDB_SETTLEMENTDETAIL Select * from posmisdb_settlementdetail@whvalu WHERE OPDATE>='" + opdate + "'";
        //        //////    oracommand_purge.CommandType = CommandType.Text;

        //        //////    oracommand_purge.ExecuteNonQuery();
        //        //////}
        //        //////catch (Exception ex)
        //        //////{

        //        //////}



        //        DateTime SETTDATE = DateTime.Today;
        //        DateTime NEW_SETTDATE = DateTime.Today;

        //        if (SETTDATE.DayOfWeek == DayOfWeek.Saturday)
        //        {

        //            NEW_SETTDATE = SETTDATE.AddDays(2);
        //        }
        //        else if (SETTDATE.DayOfWeek == DayOfWeek.Sunday)
        //        {
        //            NEW_SETTDATE = SETTDATE.AddDays(1);
        //        }
        //        else
        //        {
        //            NEW_SETTDATE = DateTime.Today;

        //        }

        //        bool sett;


        //        DateTime scheduledTime = DateTime.MinValue;
        //        DateTime scheduledTimeEND = DateTime.MinValue;
        //        scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
        //        scheduledTimeEND = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTimeEnd"]);


        //        if (NEW_SETTDATE == SETTDATE)
        //        {

        //            if ((DateTime.Now > scheduledTime) && (DateTime.Now < scheduledTimeEND))
        //            {
        //                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Report Initializing", null, null);

        //                //sett = _repoSETT.GenSettlementFileSETT_EEPLUS(SETTDATE, null, reportPath, logopath, null);
        //                // IMainReport _repoSETT = new MainReport();
        //                // string sett2 = _repoSETT.RPT_GenSettlementFile( SETTDATE, reportPath, logopath, null);

        //                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Report Completed", null, null);

        //            }

        //            try
        //            {
        //                string SqlString3 = @"Update posmisdb_company_profile set PROCESS_FLAG='C',OPDATE='" + NEW_SETTDATE.AddDays(-2).ToString("dd-MM-yyyy") + "',SETTDATE='" + NEW_SETTDATE.ToString("dd-MM-yyyy") + "',CPDDATE='" + NEW_SETTDATE.ToString("dd-MM-yyyy") + "'";
        //                oracommand_purge.CommandText = SqlString3;
        //                oracommand_purge.ExecuteNonQuery();
        //            }
        //            catch (Exception ex)
        //            {
        //                string SqlString3B = @"Update posmisdb_company_profile set PROCESS_FLAG='C'";
        //                oracommand_purge.CommandText = SqlString3B;
        //                oracommand_purge.ExecuteNonQuery();

        //                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message , null, null);


        //            }

        //            ////}
        //        }

        //    }//end of proc

        //}

        
        ////public void MEBProcess()
        ////{

        ////    try
        ////    {



        ////        string CODE = string.Empty;
        ////        string DOCNO = string.Empty;
        ////        string DEBITACCT, CREDITACCT,
        ////              SHORTREM, FULLREM, DEVICE, ISSFIID,
        ////             ISSPS, PAN, APPROVAL, ACQFIID, ACQPS, ISSFEECUR, DEBITCUR, CREDITCUR,
        ////              TERM, RETAILER, TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, INTRFEECUR,
        ////                                  IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, CRNUM, BIFEE, INTRFEE,
        ////             USDFN, USDFN2, USDFN3, STATION, CLERKCODE, NO, ENTCODE, DEBITVAL, CREDITVAL, EXTENTCODE,
        ////               ISSFICODE, ISSCOUNTRY, MBR, ACQFICODE, ACQCOUNTRY, TERMCODE, TRANAMOUNT,
        ////               TRANCUR, ORGAMOUNT, ORGCUR, ACQFEE, ACQFEECUR, ISSFEE;

        ////        DOCNO = string.Empty;
        ////        CODE = string.Empty;
        ////        Stopwatch se = new Stopwatch();
        ////        se.Start();
        ////        OracleConnection Misportal_connection = default(OracleConnection);
        ////        OracleCommand oracommand = default(OracleCommand);
        ////        OracleCommand oracommand_purge = default(OracleCommand);
        ////        // OracleCommand _with2 = default(OracleCommand);
        ////        string constr = oradb;

        ////        Misportal_connection = new OracleConnection(constr);
        ////        Misportal_connection.Open();
        ////        oracommand_purge = new OracleCommand();
        ////        oracommand_purge.Connection = Misportal_connection;
        ////        oracommand_purge.CommandType = CommandType.Text;


        ////        //////string SqlString = @"Insert into posmisdb_meb_log select * from POSMISDB_MEB where lower(process_status)='s' and trunc(sysdate-opdate)>=" + purgedays;

        ////        //////try
        ////        //////{
        ////        //////    oracommand_purge.CommandText = SqlString;
        ////        //////    oracommand_purge.ExecuteNonQuery();
        ////        //////}
        ////        //////catch { }

        ////        ////string SqlStringF = @"Delete from POSMISDB_MEB where lower(process_status)='s' and trunc(sysdate-opdate)>=" + purgedays;

        ////        ////try
        ////        ////{
        ////        ////    oracommand_purge.CommandText = SqlStringF;
        ////        ////    oracommand_purge.ExecuteNonQuery();
        ////        ////}
        ////        ////catch { }


        ////        ////string SqlStringB = @"Delete from POSMISDB_SETTLEMENTDETAIL_MEB where lower(process_status)='s' and trunc(sysdate-opdate)>=" + purgedays;

        ////        ////try
        ////        ////{
        ////        ////    oracommand_purge.CommandText = SqlStringB;
        ////        ////    oracommand_purge.ExecuteNonQuery();
        ////        ////}
        ////        ////catch { }

        ////        ////List<string> setACQFIIDList = new List<string>();
        ////        ////OracleDataReader readerAcqr = fetchAllAcquirer();
        ////        ////if (readerAcqr.HasRows)
        ////        ////{
        ////        ////    while (readerAcqr.Read())
        ////        ////    {
        ////        ////        setACQFIIDList.Add(readerAcqr["INSTITUTION_SHORTCODE"].ToString().Trim());
        ////        ////    }
        ////        ////}

        ////        ////readerAcqr.Close();
        ////        ////readerAcqr.Dispose();

        ////        decimal recCount = 0;
        ////        decimal procCount = 0;
        ////        int brCode = 40;
        ////        int i = 0;
        ////        while (i <= brCode)
        ////        {

        ////            //START WHILE
        ////            i += 1;
        ////            try
        ////            {


        ////               
        ////                lgfn.loginfoMSG("Branch Code:" + i, null, DateTime.Now.ToString());
        ////               
        ////                var td = LastRecord(i);
        ////                if (td != null)
        ////                {
        ////                    try
        ////                    {
        ////                        Lastcode = td.Code ?? 0;
        ////                        LastDocno = td.Docno ?? 0;
        ////                    }
        ////                    catch (Exception ex)
        ////                    {

        ////                    }
        ////                }
        ////                DOCNO = string.Empty;

        ////                OracleDataReader reader = processNonQuery(Lastcode, i);


        ////                bool hasRec = false;

        ////                if (reader.HasRows)
        ////                {
        ////                    hasRec = true;
        ////                   
        ////                    lgfn.loginfoMSG("MEB Collection Insertion Started: for branch ==>>" + i, null, DateTime.Now.ToString());
        ////                   

        ////                    while (reader.Read())
        ////                    {



        ////                        recCount += 1;


        ////                        oracommand = new OracleCommand();
        ////                        var _with2 = oracommand;
        ////                        _with2.Connection = Misportal_connection;
        ////                        _with2.CommandType = CommandType.Text;

        ////                        string OPDate = string.Empty;
        ////                        // lgfn.loginfoMSG("Level 1", null, reader.GetOracleDate(0).ToString());
        ////                        try
        ////                        {
        ////                            OPDate = ((DateTime)reader["OPDate"]).ToString("dd-MMM-yyyy");
        ////                        }
        ////                        catch { }


        ////                        string TRANDate;
        ////                        // lgfn.loginfoMSG("Level 2", null, reader.GetOracleDate(0).ToString());
        ////                        try
        ////                        {
        ////                            TRANDate = ((DateTime)reader["TRANDate"]).ToString("dd-MMM-yyyy");
        ////                        }
        ////                        catch { TRANDate = OPDate; }


        ////                        string EXPDate;
        ////                        // lgfn.loginfoMSG("Level 3", null, reader.GetOracleDate(0).ToString());
        ////                        try
        ////                        {
        ////                            EXPDate = ((DateTime)reader["EXPDate"]).ToString("dd-MMM-yyyy");
        ////                        }
        ////                        catch { EXPDate = OPDate; }

        ////                        DOCNO = reader.GetOracleDecimal(3).ToString();
        ////                        CODE = reader.GetOracleDecimal(61).ToString();

        ////                        ////lgfn.loginfoMSG("--With Insertion Level--", null, reader.GetOracleDate(0).ToString());


        ////                        _with2.CommandText = @"Insert into POSMISDB_MEB(OPDate,CLERKCODE,STATION,DOCNO,NO,ENTCODE,DEBITACCT,DEBITVAL,CREDITACCT,CREDITVAL,SHORTREM, 
        ////                FULLREM,DEVICE,EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////                TERMCODE,TERM,RETAILER,TLOCATION,MCC,STAN,ACQIIN,FORWARDIIN,INVOICE,USERDATA,TRANAMOUNT,TRANCUR,ORGAMOUNT,ORGCUR,ACQFEE,ACQFEECUR, 
        ////                ISSFEE,ISSFEECUR,INTRFEE,INTRFEECUR,IDENT1,INFO1,IDENT2,INFO2,IDENT3,INFO3,DEBITCUR,CREDITCUR,CRNUM,BIFEE,USDFN,USDFN2,USDFN3,BATCHNO,CODE,CREATEDATE,PROCESS_STATUS) Values (
        ////                :OPDate,:CLERKCODE,:STATION,:DOCNO,:NO,:ENTCODE,:DEBITACCT,:DEBITVAL,:CREDITACCT,:CREDITVAL,:SHORTREM,:FULLREM,:DEVICE,:EXTENTCODE,:TRANDate,
        ////                :ISSFICODE,:ISSFIID,:ISSPS,:ISSCOUNTRY,:PAN,:MBR,:EXPDate,:APPROVAL,:ACQFICODE,:ACQFIID,:ACQPS,:ACQCOUNTRY,:TERMCODE,:TERM,:RETAILER,:TLOCATION,
        ////                :MCC,:STAN,:ACQIIN,:FORWARDIIN,:INVOICE,:USERDATA,:TRANAMOUNT,:TRANCUR,:ORGAMOUNT,:ORGCUR,:ACQFEE,:ACQFEECUR,:ISSFEE,:ISSFEECUR,:INTRFEE,:INTRFEECUR,
        ////                :IDENT1,:INFO1,:IDENT2,:INFO2,:IDENT3,:INFO3,:DEBITCUR,:CREDITCUR,:CRNUM,:BIFEE,:USDFN,:USDFN2,:USDFN3,:BATCHNO,:CODE,:CREATEDATE,:PROCESS_STATUS)";


        ////                        _with2.Parameters.Add(":OPDATE", OracleDbType.Varchar2).Value = OPDate;
        ////                        _with2.Parameters.Add(":CLERKCODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(1);
        ////                        _with2.Parameters.Add(":STATION", OracleDbType.Decimal).Value = reader.GetOracleDecimal(2);
        ////                        _with2.Parameters.Add(":DOCNO", OracleDbType.Decimal).Value = reader.GetOracleDecimal(3);
        ////                        _with2.Parameters.Add(":NO", OracleDbType.Decimal).Value = reader.GetOracleDecimal(4);
        ////                        _with2.Parameters.Add(":ENTCODE", OracleDbType.Varchar2).Value = reader.GetOracleString(5).ToString();
        ////                        _with2.Parameters.Add(":DEBITACCT", OracleDbType.Varchar2).Value = reader.GetOracleString(6).ToString();
        ////                        _with2.Parameters.Add(":DEBITVAL", OracleDbType.Decimal).Value = reader.GetOracleDecimal(7);
        ////                        _with2.Parameters.Add(":CREDITACCT", OracleDbType.Varchar2).Value = reader.GetOracleString(8).ToString();
        ////                        _with2.Parameters.Add(":CREDITVAL", OracleDbType.Decimal).Value = reader.GetOracleDecimal(9);
        ////                        _with2.Parameters.Add(":SHORTREM", OracleDbType.Varchar2).Value = reader.GetOracleString(10).ToString();
        ////                        _with2.Parameters.Add(":FULLREM", OracleDbType.NVarchar2).Value = reader.GetOracleString(11).ToString();
        ////                        _with2.Parameters.Add(":DEVICE", OracleDbType.Varchar2).Value = reader.GetOracleString(12).ToString();
        ////                        _with2.Parameters.Add(":EXTENTCODE", OracleDbType.Varchar2).Value = reader.GetOracleString(13).ToString();
        ////                        _with2.Parameters.Add(":TRANDate", OracleDbType.Varchar2).Value = TRANDate;
        ////                        _with2.Parameters.Add(":ISSFICODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(15);
        ////                        _with2.Parameters.Add(":ISSFIID", OracleDbType.Varchar2).Value = reader.GetOracleString(16).ToString();
        ////                        _with2.Parameters.Add(":ISSPS", OracleDbType.Varchar2).Value = reader.GetOracleString(17).ToString().Substring(0, 4);
        ////                        _with2.Parameters.Add(":ISSCOUNTRY", OracleDbType.Decimal).Value = reader.GetOracleDecimal(18);
        ////                        _with2.Parameters.Add(":PAN", OracleDbType.Varchar2).Value = reader.GetOracleString(19).ToString();
        ////                        _with2.Parameters.Add(":MBR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(20);
        ////                        _with2.Parameters.Add(":EXPDate", OracleDbType.Varchar2).Value = EXPDate;
        ////                        _with2.Parameters.Add(":APPROVAL", OracleDbType.Varchar2).Value = reader.GetOracleString(22).ToString();
        ////                        _with2.Parameters.Add(":ACQFICODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(23);
        ////                        _with2.Parameters.Add(":ACQFIID", OracleDbType.Varchar2).Value = reader.GetOracleString(24).ToString();
        ////                        _with2.Parameters.Add(":ACQPS", OracleDbType.Varchar2).Value = reader.GetOracleString(25).ToString();
        ////                        _with2.Parameters.Add(":ACQCOUNTRY", OracleDbType.Decimal).Value = reader.GetOracleDecimal(26);
        ////                        _with2.Parameters.Add(":TERMCODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(27);
        ////                        _with2.Parameters.Add(":TERM", OracleDbType.Varchar2).Value = reader.GetOracleString(28).ToString();
        ////                        _with2.Parameters.Add(":RETAILER", OracleDbType.Varchar2).Value = reader.GetOracleString(29).ToString();
        ////                        _with2.Parameters.Add(":TLOCATION", OracleDbType.Varchar2).Value = reader.GetOracleString(30).ToString();
        ////                        _with2.Parameters.Add(":MCC", OracleDbType.Varchar2).Value = reader.GetOracleString(31).ToString();
        ////                        _with2.Parameters.Add(":STAN", OracleDbType.Decimal).Value = reader.GetOracleDecimal(32);
        ////                        _with2.Parameters.Add(":ACQIIN", OracleDbType.Varchar2).Value = reader.GetOracleString(33).ToString();
        ////                        _with2.Parameters.Add(":FORWARDIIN", OracleDbType.Varchar2).Value = reader.GetOracleString(34).ToString();
        ////                        _with2.Parameters.Add(":INVOICE", OracleDbType.Varchar2).Value = reader.GetOracleString(35).ToString();
        ////                        _with2.Parameters.Add(":USERDATA", OracleDbType.NVarchar2).Value = reader.GetOracleString(36).ToString();
        ////                        _with2.Parameters.Add(":TRANAMOUNT", OracleDbType.Decimal).Value = reader.GetOracleDecimal(37);
        ////                        _with2.Parameters.Add(":TRANCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(38);
        ////                        _with2.Parameters.Add(":ORGAMOUNT", OracleDbType.Decimal).Value = reader.GetOracleDecimal(39);
        ////                        _with2.Parameters.Add(":ORGCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(40);
        ////                        _with2.Parameters.Add(":ACQFEE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(41);
        ////                        _with2.Parameters.Add(":ACQFEECUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(42);
        ////                        _with2.Parameters.Add(":ISSFEE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(43);
        ////                        _with2.Parameters.Add(":ISSFEECUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(44);
        ////                        _with2.Parameters.Add(":INTRFEE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(45);
        ////                        _with2.Parameters.Add(":INTRFEECUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(46);
        ////                        _with2.Parameters.Add(":IDENT1", OracleDbType.NVarchar2).Value = reader.GetOracleString(47).ToString();
        ////                        _with2.Parameters.Add(":INFO1", OracleDbType.NVarchar2).Value = reader.GetOracleString(48).ToString();
        ////                        _with2.Parameters.Add(":IDENT2", OracleDbType.NVarchar2).Value = reader.GetOracleString(49).ToString();
        ////                        _with2.Parameters.Add(":INFO2", OracleDbType.NVarchar2).Value = reader.GetOracleString(50).ToString();
        ////                        _with2.Parameters.Add(":IDENT3", OracleDbType.NVarchar2).Value = reader.GetOracleString(51).ToString();
        ////                        _with2.Parameters.Add(":INFO3", OracleDbType.NVarchar2).Value = reader.GetOracleString(52).ToString();
        ////                        _with2.Parameters.Add(":DEBITCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(53);
        ////                        _with2.Parameters.Add(":CREDITCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(54);
        ////                        _with2.Parameters.Add(":CRNUM", OracleDbType.NVarchar2).Value = reader.GetOracleString(55).ToString();
        ////                        _with2.Parameters.Add(":BIFEE", OracleDbType.NVarchar2).Value = reader.GetOracleString(56).ToString();
        ////                        _with2.Parameters.Add(":USDFN", OracleDbType.NVarchar2).Value = reader.GetOracleString(57).ToString();
        ////                        _with2.Parameters.Add(":USDFN2", OracleDbType.NVarchar2).Value = reader.GetOracleString(58).ToString();
        ////                        _with2.Parameters.Add(":USDFN3", OracleDbType.NVarchar2).Value = reader.GetOracleString(59).ToString();
        ////                        _with2.Parameters.Add(":BATCHNO", OracleDbType.NVarchar2).Value = reader.GetOracleString(60).ToString();
        ////                        _with2.Parameters.Add(":CODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(61);
        ////                        _with2.Parameters.Add(":CREATEDATE", OracleDbType.Date).Value = DateTime.Now;
        ////                        _with2.Parameters.Add(":PROCESS_STATUS", OracleDbType.Varchar2).Value = "A";

        ////                        try
        ////                        {

        ////                            _with2.BindByName = true;
        ////                            _with2.ExecuteNonQuery();
        ////                            procCount += 1;

        ////                        }
        ////                        catch (Exception ex)
        ////                        {

        ////                            if (!ex.Message.ToString().Contains(res))
        ////                            {
        ////                               
        ////                                lgfn.loginfoMSG(ex.Message + " DOC NO:" + DOCNO + " CODE:" + CODE, null, DateTime.Now.ToString());
        ////                               

        ////                                if (CODE != string.Empty)
        ////                                {
        ////                                    string SqlString2C = @"Update Posmisdb_mebLastRecord Set LASTDOCNO='" + DOCNO + "',Code='" + CODE + "' where branchcode=" + i;
        ////                                    oracommand_purge.CommandText = SqlString2C;
        ////                                    oracommand_purge.ExecuteNonQuery();
        ////                                }
        ////                            }





        ////                        }




        ////                    }


        ////                }
        ////                reader.Dispose();




        ////                if (CODE != string.Empty)
        ////                {
        ////                    try
        ////                    {

        ////                        if (DOCNO != string.Empty)
        ////                        {
        ////                            string SqlString2B = @"Insert into POSMISDB_MEB_TIME(LASTDOCNO,CODE,BRANCHCODE,TIME) VALUES ('" + DOCNO + "','" + CODE + "'," + i + ",'" + DateTime.Now + "')";
        ////                            oracommand_purge.CommandText = SqlString2B;
        ////                            oracommand_purge.ExecuteNonQuery();

        ////                            string SqlString2C = @"Update Posmisdb_mebLastRecord Set LASTDOCNO='" + DOCNO + "',Code='" + CODE + "' where branchcode=" + i;
        ////                            oracommand_purge.CommandText = SqlString2C;
        ////                            oracommand_purge.ExecuteNonQuery();
        ////                        }



        ////                    }
        ////                    catch (Exception ex)
        ////                    {

        ////                       
        ////                        lgfn.loginfoMSG(ex.Message + " DOC NO:" + DOCNO + " CODE:" + CODE, null, DateTime.Now.ToString());
        ////                       

        ////                        if (CODE != string.Empty)
        ////                        {
        ////                            string SqlString2C = @"Update Posmisdb_mebLastRecord Set LASTDOCNO='" + DOCNO + "',Code='" + CODE + "' where branchcode=" + i;
        ////                            oracommand_purge.CommandText = SqlString2C;
        ////                            oracommand_purge.ExecuteNonQuery();
        ////                        }


        ////                    }

        ////                }

        ////            }
        ////            catch (Exception ex)
        ////            {

        ////            }

        ////            //END WHILE

        ////        }

        ////        if (CODE != string.Empty)
        ////        {
        ////            try
        ////            {
        ////                string SqlString2 = @"Update posmisdb_company_profile set PROCESS_FLAG='A'";
        ////                oracommand_purge.CommandText = SqlString2;
        ////                oracommand_purge.ExecuteNonQuery();



        ////            }
        ////            catch (Exception ex)
        ////            {

        ////               
        ////                lgfn.loginfoMSG(ex.Message + " DOC NO:" + DOCNO + " CODE:" + CODE, null, DateTime.Now.ToString());
        ////               

        ////            }

        ////            Misportal_connection.Close();
        ////            Misportal_connection.Dispose();

        ////            se.Stop();
        ////            TimeSpan ts2 = se.Elapsed;
        ////            string eT2 = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts2.Hours, ts2.Minutes, ts2.Seconds, ts2.Milliseconds);
        ////            lgfn.loginfoMSG("MEB Collection Insertion Completed:" + eT2, null, DateTime.Now.ToString());
        ////           
        ////            lgfn.loginfoMSG("Total Record Count:" + recCount + "| Total Record Processed:" + procCount, null, DateTime.Now.ToString());
        ////           

        ////        }

        ////    }

        ////    catch (Exception ex)
        ////    {
        ////       
        ////        lgfn.loginfoMSG("Error During Process:", ex.Message, DateTime.Now.ToString());
        ////       


        ////    }

        ////    //START SETTLEMENT PROCESS AFTER
        ////    lgfn.loginfoMSG("Settlement Processing Initialize", " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

        ////    string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
        ////    SettlementProcess(dtFrom2);

        ////}

        ////public void MEBProcess()
        ////{

        ////    try
        ////    {



        ////        string CODE = string.Empty;
        ////        string DOCNO = string.Empty;
        ////        string DEBITACCT, CREDITACCT,
        ////              SHORTREM, FULLREM, DEVICE, ISSFIID,
        ////             ISSPS, PAN, APPROVAL, ACQFIID, ACQPS, ISSFEECUR, DEBITCUR, CREDITCUR,
        ////              TERM, RETAILER, TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, INTRFEECUR,
        ////                                  IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, CRNUM, BIFEE, INTRFEE,
        ////             USDFN, USDFN2, USDFN3, STATION, CLERKCODE, NO, ENTCODE, DEBITVAL, CREDITVAL, EXTENTCODE,
        ////               ISSFICODE, ISSCOUNTRY, MBR, ACQFICODE, ACQCOUNTRY, TERMCODE, TRANAMOUNT,
        ////               TRANCUR, ORGAMOUNT, ORGCUR, ACQFEE, ACQFEECUR, ISSFEE;

        ////        DOCNO = string.Empty;
        ////        CODE = string.Empty;
        ////        Stopwatch se = new Stopwatch();
        ////        se.Start();
        ////        OracleConnection Misportal_connection = default(OracleConnection);
        ////        OracleCommand oracommand = default(OracleCommand);
        ////        OracleCommand oracommand_purge = default(OracleCommand);
        ////        // OracleCommand _with2 = default(OracleCommand);
        ////        string constr = oradb;

        ////        Misportal_connection = new OracleConnection(constr);
        ////        Misportal_connection.Open();
        ////        oracommand_purge = new OracleCommand();
        ////        oracommand_purge.Connection = Misportal_connection;
        ////        oracommand_purge.CommandType = CommandType.Text;


        ////        //////string SqlString = @"Insert into posmisdb_meb_log select * from POSMISDB_MEB where lower(process_status)='s' and trunc(sysdate-opdate)>=" + purgedays;

        ////        //////try
        ////        //////{
        ////        //////    oracommand_purge.CommandText = SqlString;
        ////        //////    oracommand_purge.ExecuteNonQuery();
        ////        //////}
        ////        //////catch { }

        ////        ////string SqlStringF = @"Delete from POSMISDB_MEB where lower(process_status)='s' and trunc(sysdate-opdate)>=" + purgedays;

        ////        ////try
        ////        ////{
        ////        ////    oracommand_purge.CommandText = SqlStringF;
        ////        ////    oracommand_purge.ExecuteNonQuery();
        ////        ////}
        ////        ////catch { }


        ////        ////string SqlStringB = @"Delete from POSMISDB_SETTLEMENTDETAIL_MEB where lower(process_status)='s' and trunc(sysdate-opdate)>=" + purgedays;

        ////        ////try
        ////        ////{
        ////        ////    oracommand_purge.CommandText = SqlStringB;
        ////        ////    oracommand_purge.ExecuteNonQuery();
        ////        ////}
        ////        ////catch { }

        ////        List<string> setACQFIIDList = new List<string>();
        ////        OracleDataReader readerAcqr = fetchAllAcquirer();
        ////        if (readerAcqr.HasRows)
        ////        {
        ////            while (readerAcqr.Read())
        ////            {
        ////                setACQFIIDList.Add(readerAcqr["ACQFIID"].ToString().Trim());
        ////            }
        ////        }

        ////        readerAcqr.Close();
        ////        readerAcqr.Dispose();

        ////        decimal recCount = 0;
        ////        decimal procCount = 0;




        ////        foreach (var ACQRFIID in setACQFIIDList)
        ////        {
           

        ////            //START WHILE
                 
        ////            try
        ////            {


        ////               
        ////                lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO  MEB Acquirer Last Code for " + ACQRFIID, null, null);
        ////               
        ////                var td = LastRecord_NEW(ACQRFIID);
        ////                if (td != null)
        ////                {
        ////                    try
        ////                    {
        ////                        Lastcode = td.Code ?? 0;
        ////                        LastDocno = td.Docno ?? 0;
        ////                    }
        ////                    catch (Exception ex)
        ////                    {

        ////                    }
        ////                }
        ////                DOCNO = string.Empty;

        ////                OracleDataReader reader = processNonQuery_NEW(Lastcode, ACQRFIID);


        ////                bool hasRec = false;

        ////                if (reader.HasRows)
        ////                {
        ////                    hasRec = true;
        ////                   
        ////                    lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO  MEB Acquirer Processing for " + ACQRFIID, null, null);
        ////                   

        ////                    while (reader.Read())
        ////                    {



        ////                        recCount += 1;


        ////                        oracommand = new OracleCommand();
        ////                        var _with2 = oracommand;
        ////                        _with2.Connection = Misportal_connection;
        ////                        _with2.CommandType = CommandType.Text;

        ////                        string OPDate = string.Empty;
        ////                        // lgfn.loginfoMSG("Level 1", null, reader.GetOracleDate(0).ToString());
        ////                        try
        ////                        {
        ////                            OPDate = ((DateTime)reader["OPDate"]).ToString("dd-MMM-yyyy");
        ////                        }
        ////                        catch { }


        ////                        string TRANDate;
        ////                        // lgfn.loginfoMSG("Level 2", null, reader.GetOracleDate(0).ToString());
        ////                        try
        ////                        {
        ////                            TRANDate = ((DateTime)reader["TRANDate"]).ToString("dd-MMM-yyyy");
        ////                        }
        ////                        catch { TRANDate = OPDate; }


        ////                        string EXPDate;
        ////                        // lgfn.loginfoMSG("Level 3", null, reader.GetOracleDate(0).ToString());
        ////                        try
        ////                        {
        ////                            EXPDate = ((DateTime)reader["EXPDate"]).ToString("dd-MMM-yyyy");
        ////                        }
        ////                        catch { EXPDate = OPDate; }

        ////                        DOCNO = reader.GetOracleDecimal(3).ToString();
        ////                        CODE = reader.GetOracleDecimal(61).ToString();

        ////                        ////lgfn.loginfoMSG("--With Insertion Level--", null, reader.GetOracleDate(0).ToString());


        ////                        _with2.CommandText = @"Insert into POSMISDB_MEB(OPDate,CLERKCODE,STATION,DOCNO,NO,ENTCODE,DEBITACCT,DEBITVAL,CREDITACCT,CREDITVAL,SHORTREM, 
        ////                FULLREM,DEVICE,EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////                TERMCODE,TERM,RETAILER,TLOCATION,MCC,STAN,ACQIIN,FORWARDIIN,INVOICE,USERDATA,TRANAMOUNT,TRANCUR,ORGAMOUNT,ORGCUR,ACQFEE,ACQFEECUR, 
        ////                ISSFEE,ISSFEECUR,INTRFEE,INTRFEECUR,IDENT1,INFO1,IDENT2,INFO2,IDENT3,INFO3,DEBITCUR,CREDITCUR,CRNUM,BIFEE,USDFN,USDFN2,USDFN3,BATCHNO,CODE,CREATEDATE,PROCESS_STATUS) Values (
        ////                :OPDate,:CLERKCODE,:STATION,:DOCNO,:NO,:ENTCODE,:DEBITACCT,:DEBITVAL,:CREDITACCT,:CREDITVAL,:SHORTREM,:FULLREM,:DEVICE,:EXTENTCODE,:TRANDate,
        ////                :ISSFICODE,:ISSFIID,:ISSPS,:ISSCOUNTRY,:PAN,:MBR,:EXPDate,:APPROVAL,:ACQFICODE,:ACQFIID,:ACQPS,:ACQCOUNTRY,:TERMCODE,:TERM,:RETAILER,:TLOCATION,
        ////                :MCC,:STAN,:ACQIIN,:FORWARDIIN,:INVOICE,:USERDATA,:TRANAMOUNT,:TRANCUR,:ORGAMOUNT,:ORGCUR,:ACQFEE,:ACQFEECUR,:ISSFEE,:ISSFEECUR,:INTRFEE,:INTRFEECUR,
        ////                :IDENT1,:INFO1,:IDENT2,:INFO2,:IDENT3,:INFO3,:DEBITCUR,:CREDITCUR,:CRNUM,:BIFEE,:USDFN,:USDFN2,:USDFN3,:BATCHNO,:CODE,:CREATEDATE,:PROCESS_STATUS)";


        ////                        _with2.Parameters.Add(":OPDATE", OracleDbType.Varchar2).Value = OPDate;
        ////                        _with2.Parameters.Add(":CLERKCODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(1);
        ////                        _with2.Parameters.Add(":STATION", OracleDbType.Decimal).Value = reader.GetOracleDecimal(2);
        ////                        _with2.Parameters.Add(":DOCNO", OracleDbType.Decimal).Value = reader.GetOracleDecimal(3);
        ////                        _with2.Parameters.Add(":NO", OracleDbType.Decimal).Value = reader.GetOracleDecimal(4);
        ////                        _with2.Parameters.Add(":ENTCODE", OracleDbType.Varchar2).Value = reader.GetOracleString(5).ToString();
        ////                        _with2.Parameters.Add(":DEBITACCT", OracleDbType.Varchar2).Value = reader.GetOracleString(6).ToString();
        ////                        _with2.Parameters.Add(":DEBITVAL", OracleDbType.Decimal).Value = reader.GetOracleDecimal(7);
        ////                        _with2.Parameters.Add(":CREDITACCT", OracleDbType.Varchar2).Value = reader.GetOracleString(8).ToString();
        ////                        _with2.Parameters.Add(":CREDITVAL", OracleDbType.Decimal).Value = reader.GetOracleDecimal(9);
        ////                        _with2.Parameters.Add(":SHORTREM", OracleDbType.Varchar2).Value = reader.GetOracleString(10).ToString();
        ////                        _with2.Parameters.Add(":FULLREM", OracleDbType.NVarchar2).Value = reader.GetOracleString(11).ToString();
        ////                        _with2.Parameters.Add(":DEVICE", OracleDbType.Varchar2).Value = reader.GetOracleString(12).ToString();
        ////                        _with2.Parameters.Add(":EXTENTCODE", OracleDbType.Varchar2).Value = reader.GetOracleString(13).ToString();
        ////                        _with2.Parameters.Add(":TRANDate", OracleDbType.Varchar2).Value = TRANDate;
        ////                        _with2.Parameters.Add(":ISSFICODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(15);
        ////                        _with2.Parameters.Add(":ISSFIID", OracleDbType.Varchar2).Value = reader.GetOracleString(16).ToString();
        ////                        _with2.Parameters.Add(":ISSPS", OracleDbType.Varchar2).Value = reader.GetOracleString(17).ToString().Substring(0, 4);
        ////                        _with2.Parameters.Add(":ISSCOUNTRY", OracleDbType.Decimal).Value = reader.GetOracleDecimal(18);
        ////                        _with2.Parameters.Add(":PAN", OracleDbType.Varchar2).Value = reader.GetOracleString(19).ToString();
        ////                        _with2.Parameters.Add(":MBR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(20);
        ////                        _with2.Parameters.Add(":EXPDate", OracleDbType.Varchar2).Value = EXPDate;
        ////                        _with2.Parameters.Add(":APPROVAL", OracleDbType.Varchar2).Value = reader.GetOracleString(22).ToString();
        ////                        _with2.Parameters.Add(":ACQFICODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(23);
        ////                        _with2.Parameters.Add(":ACQFIID", OracleDbType.Varchar2).Value = reader.GetOracleString(24).ToString();
        ////                        _with2.Parameters.Add(":ACQPS", OracleDbType.Varchar2).Value = reader.GetOracleString(25).ToString();
        ////                        _with2.Parameters.Add(":ACQCOUNTRY", OracleDbType.Decimal).Value = reader.GetOracleDecimal(26);
        ////                        _with2.Parameters.Add(":TERMCODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(27);
        ////                        _with2.Parameters.Add(":TERM", OracleDbType.Varchar2).Value = reader.GetOracleString(28).ToString();
        ////                        _with2.Parameters.Add(":RETAILER", OracleDbType.Varchar2).Value = reader.GetOracleString(29).ToString();
        ////                        _with2.Parameters.Add(":TLOCATION", OracleDbType.Varchar2).Value = reader.GetOracleString(30).ToString();
        ////                        _with2.Parameters.Add(":MCC", OracleDbType.Varchar2).Value = reader.GetOracleString(31).ToString();
        ////                        _with2.Parameters.Add(":STAN", OracleDbType.Decimal).Value = reader.GetOracleDecimal(32);
        ////                        _with2.Parameters.Add(":ACQIIN", OracleDbType.Varchar2).Value = reader.GetOracleString(33).ToString();
        ////                        _with2.Parameters.Add(":FORWARDIIN", OracleDbType.Varchar2).Value = reader.GetOracleString(34).ToString();
        ////                        _with2.Parameters.Add(":INVOICE", OracleDbType.Varchar2).Value = reader.GetOracleString(35).ToString();
        ////                        _with2.Parameters.Add(":USERDATA", OracleDbType.NVarchar2).Value = reader.GetOracleString(36).ToString();
        ////                        _with2.Parameters.Add(":TRANAMOUNT", OracleDbType.Decimal).Value = reader.GetOracleDecimal(37);
        ////                        _with2.Parameters.Add(":TRANCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(38);
        ////                        _with2.Parameters.Add(":ORGAMOUNT", OracleDbType.Decimal).Value = reader.GetOracleDecimal(39);
        ////                        _with2.Parameters.Add(":ORGCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(40);
        ////                        _with2.Parameters.Add(":ACQFEE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(41);
        ////                        _with2.Parameters.Add(":ACQFEECUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(42);
        ////                        _with2.Parameters.Add(":ISSFEE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(43);
        ////                        _with2.Parameters.Add(":ISSFEECUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(44);
        ////                        _with2.Parameters.Add(":INTRFEE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(45);
        ////                        _with2.Parameters.Add(":INTRFEECUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(46);
        ////                        _with2.Parameters.Add(":IDENT1", OracleDbType.NVarchar2).Value = reader.GetOracleString(47).ToString();
        ////                        _with2.Parameters.Add(":INFO1", OracleDbType.NVarchar2).Value = reader.GetOracleString(48).ToString();
        ////                        _with2.Parameters.Add(":IDENT2", OracleDbType.NVarchar2).Value = reader.GetOracleString(49).ToString();
        ////                        _with2.Parameters.Add(":INFO2", OracleDbType.NVarchar2).Value = reader.GetOracleString(50).ToString();
        ////                        _with2.Parameters.Add(":IDENT3", OracleDbType.NVarchar2).Value = reader.GetOracleString(51).ToString();
        ////                        _with2.Parameters.Add(":INFO3", OracleDbType.NVarchar2).Value = reader.GetOracleString(52).ToString();
        ////                        _with2.Parameters.Add(":DEBITCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(53);
        ////                        _with2.Parameters.Add(":CREDITCUR", OracleDbType.Decimal).Value = reader.GetOracleDecimal(54);
        ////                        _with2.Parameters.Add(":CRNUM", OracleDbType.NVarchar2).Value = reader.GetOracleString(55).ToString();
        ////                        _with2.Parameters.Add(":BIFEE", OracleDbType.NVarchar2).Value = reader.GetOracleString(56).ToString();
        ////                        _with2.Parameters.Add(":USDFN", OracleDbType.NVarchar2).Value = reader.GetOracleString(57).ToString();
        ////                        _with2.Parameters.Add(":USDFN2", OracleDbType.NVarchar2).Value = reader.GetOracleString(58).ToString();
        ////                        _with2.Parameters.Add(":USDFN3", OracleDbType.NVarchar2).Value = reader.GetOracleString(59).ToString();
        ////                        _with2.Parameters.Add(":BATCHNO", OracleDbType.NVarchar2).Value = reader.GetOracleString(60).ToString();
        ////                        _with2.Parameters.Add(":CODE", OracleDbType.Decimal).Value = reader.GetOracleDecimal(61);
        ////                        _with2.Parameters.Add(":CREATEDATE", OracleDbType.Date).Value = DateTime.Now;
        ////                        _with2.Parameters.Add(":PROCESS_STATUS", OracleDbType.Varchar2).Value = "A";

        ////                        try
        ////                        {

        ////                            _with2.BindByName = true;
        ////                            _with2.ExecuteNonQuery();
        ////                            procCount += 1;

        ////                        }
        ////                        catch (Exception ex)
        ////                        {

        ////                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + ex.Message + " ON CODE:" + CODE, null, null);

        ////                            ////if (!ex.Message.ToString().Contains(res))
        ////                            ////{
        ////                            ////   
        ////                            ////    lgfn.loginfoMSG(ex.Message + " DOC NO:" + DOCNO + " CODE:" + CODE, null, DateTime.Now.ToString());
        ////                            ////   

        ////                            ////    if (CODE != string.Empty)
        ////                            ////    {
        ////                            ////        string SqlString2C = @"Update Posmisdb_mebLastRecord Set LASTDOCNO='" + DOCNO + "',Code='" + CODE + "' where branchcode=" + i;
        ////                            ////        oracommand_purge.CommandText = SqlString2C;
        ////                            ////        oracommand_purge.ExecuteNonQuery();
        ////                            ////    }
        ////                            ////}

                                    

        ////                        }



        ////                    }


        ////                }
        ////                reader.Dispose();


        ////                //////if (CODE != string.Empty)
        ////                //////{
        ////                //////    try
        ////                //////    {

        ////                //////        ////if (DOCNO != string.Empty)
        ////                //////        ////{
        ////                //////        ////    string SqlString2B = @"Insert into POSMISDB_MEB_TIME(LASTDOCNO,CODE,BRANCHCODE,TIME) VALUES ('" + DOCNO + "','" + CODE + "'," + i + ",'" + DateTime.Now + "')";
        ////                //////        ////    oracommand_purge.CommandText = SqlString2B;
        ////                //////        ////    oracommand_purge.ExecuteNonQuery();

        ////                //////        ////    string SqlString2C = @"Update Posmisdb_mebLastRecord Set LASTDOCNO='" + DOCNO + "',Code='" + CODE + "' where branchcode=" + i;
        ////                //////        ////    oracommand_purge.CommandText = SqlString2C;
        ////                //////        ////    oracommand_purge.ExecuteNonQuery();
        ////                //////        ////}



        ////                //////    }
        ////                //////    catch (Exception ex)
        ////                //////    {

        ////                //////       
        ////                //////        lgfn.loginfoMSG(ex.Message + " DOC NO:" + DOCNO + " CODE:" + CODE, null, DateTime.Now.ToString());
        ////                //////       

        ////                //////        //////if (CODE != string.Empty)
        ////                //////        //////{
        ////                //////        //////    string SqlString2C = @"Update Posmisdb_mebLastRecord Set LASTDOCNO='" + DOCNO + "',Code='" + CODE + "' where branchcode=" + i;
        ////                //////        //////    oracommand_purge.CommandText = SqlString2C;
        ////                //////        //////    oracommand_purge.ExecuteNonQuery();
        ////                //////        //////}


        ////                //////    }

        ////                //////}

        ////            }
        ////            catch (Exception ex)
        ////            {

        ////            }

        ////            //END WHILE

        ////        }

        ////        if (CODE != string.Empty)
        ////        {
        ////            try
        ////            {
        ////                string SqlString2 = @"Update posmisdb_company_profile set PROCESS_FLAG='A'";
        ////                oracommand_purge.CommandText = SqlString2;
        ////                oracommand_purge.ExecuteNonQuery();



        ////            }
        ////            catch (Exception ex)
        ////            {

        ////               

        ////                lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + ex.Message + " ON CODE:" + CODE, null, null);

        ////               

        ////            }

        ////            Misportal_connection.Close();
        ////            Misportal_connection.Dispose();

        ////            se.Stop();
        ////            TimeSpan ts2 = se.Elapsed;
        ////            string eT2 = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts2.Hours, ts2.Minutes, ts2.Seconds, ts2.Milliseconds);
                   
        ////            lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO MEB Collection Insertion Completed" + eT2, null, null);

                  
        ////            lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO Total Record Count" + procCount, null, null);

        ////           

        ////        }

        ////    }

        ////    catch (Exception ex)
        ////    {
        ////        lgfn.loginfo3("-------------------------------------------------------------");
        ////        lgfn.loginfo3("-------------------------------------------------------------");
        ////        lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + ex.Message , null, null);



        ////    }

        ////    //START SETTLEMENT PROCESS AFTER
        ////     lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Settlement Process Initializing Phase 2" , null, null);

        ////    string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
        ////    SettlementProcess(dtFrom2);

        ////}

        public void multitreadOpr(string day,DbConnection con)
        {
            //FILE PROCESSING
             DateTime opdate;
            if (processdate != string.Empty)
            {
                try
                {
                    opdate = Convert.ToDateTime(processdate);
                }
                catch
                {
                    opdate = Convert.ToDateTime(day);
                }
            }
            else
            {
                opdate = Convert.ToDateTime(day);
            }



            try
            {

                //OracleConnection Standby_connection = new OracleConnection(oradb);
                //OracleCommand cmd = new OracleCommand();

                ////try
                ////{


                ////    string SqlString2A = @"Update posmisdb_meb set OPDATE2=OPDATE where OPDATE2 is null and PROCESS_STATUS='A' and OPDATE<'" + opdate + "'";
                ////    cmd.Connection = Standby_connection;
                ////    cmd.CommandText = SqlString2A;

                ////    cmd.CommandType = CommandType.Text;
                ////    cmd.ExecuteNonQuery();
                ////}
                ////catch
                ////{

                ////}
                //////try
                //////{
                //////    string SqlString2B = @"Update posmisdb_meb set OPDATE='" + opdate + "' where PROCESS_STATUS='A' and OPDATE<'" + opdate + "' and TO_CHAR(createdate,'HH24')>8 and TO_CHAR(createdate,'HH24')<24 and docno not in (select docno from posmisdb_settlementdetail_meb)";
                //////    con.Execute(SqlString2B, commandType: CommandType.Text);
                //////}
                //////catch
                //////{

                //////}

                try
                {

                    List<DateTime> setDateList = new List<DateTime>();
                    if (opdate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // opdate = opdate.AddDays(-2);
                       // setDateList.Add(opdate.AddDays(-2));
                        //setDateList.Add(opdate.AddDays(-1));
                        //setDateList.Add(opdate);

                        opdate = opdate.AddDays(-3);
                    }
                    else
                    {
                        //setDateList.Add(opdate.AddDays(-1));
                        // setDateList.Add(opdate);
                        opdate = opdate.AddDays(-1);
                    }

                    try
                    {
                        string batchno = string.Empty;
                        //foreach (var d in setDateList)
                        //{

                            var random = new Random((int)DateTime.Now.Ticks);
                            var randomValue = random.Next(1000000, 9999999);
                            batchno = randomValue.ToString();

                        //Parallel posting Invoke
                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [SETTLEMENT PROCESS] FOR OPDATE ==== " + opdate.ToString("dd-MMM-yyyy"), null, null);
                            //new SettlementProcess_MEB().SettProcess(batchno, opdate.ToString("dd-MMM-yyyy"));


                        Parallel.Invoke(() =>
                        {
                            new SettlementProcess_MEB().SettProcess(batchno, opdate.ToString("dd-MMM-yyyy"));
                        },
                        () =>
                        {
                            new SettProcess_POS().SettProcessPOS(batchno, opdate.ToString("dd-MMM-yyyy"));
                        }) ;



                        //}

                        ////var random = new Random((int)DateTime.Now.Ticks);
                        ////var randomValue = random.Next(1000000, 9999999);
                        ////string batchno = randomValue.ToString();
                        ////try
                        ////{
                        ////    lgfn.loginfoMSG("Revalidate Process ------------------>>>", null, DateTime.Now.ToString());
                        ////    var random = new Random((int)DateTime.Now.Ticks);
                        ////    var randomValue = random.Next(1000000, 9999999);
                        ////     batchno = randomValue.ToString();
                        ////    new SettlementProcess_MEB().SETTProcess(batchno, opdate.ToString("dd-MMM-yyyy"));

                        ////}
                        ////catch (Exception ex)
                        ////{
                        ////}

                    }
                    catch (Exception ex)
                    {

                        try
                        {
                            //if (Standby_connection == null)
                            //{
                            //    Standby_connection = new OracleConnection(oradb);
                            //}
                            //if (Standby_connection.State != ConnectionState.Open)
                            //{
                            //    Standby_connection.Open();
                            //}
                            string SqlString2 = @"Update sm_company_profile set PROCESS_FLAG='A'";
                            con.Query(SqlString2, commandType: CommandType.Text);
                        }
                        catch (Exception emx)
                        {
                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + emx.Message, null, null);

                        }
                    }

                   // lgfn.loginfoMSG("       File Processing Ended", null, DateTime.Now.ToString());

                    lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO  Settlement Process Finalizing", null, null);

                }
                catch (Exception ex)
                {
                    lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());


                }
            }
            catch (Exception ex)
            {
                lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());


            }

            //FILE GENERATING

        }
        
        private DirectoryInfo root_path;
        private FileInfo[] GetFiles(string MEBPath)
        {

            //Check folder path
            root_path = new DirectoryInfo(MEBPath);
            return root_path.GetFiles("*.dbf", SearchOption.AllDirectories);

            // return root_path.GetFiles("*.dbf,*.xlsx,*.xls", SearchOption.AllDirectories);
        }
        FileInfo[] files;
        public DataSet GetDbfFile(string FilePath, string batchno)
        {
            ///TEST REPORT HERE
            //////bool sett = _repoSETT.GenSettlementFile(DateTime.Parse("30-MAR-16"), null, reportPath, logopath, conString);
             lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO  MEB Process Initializing", null, null);


            files = GetFiles(FilePath);
           // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] MEB FILES FROM MEB SOURCE LOCAL==" + files.Count(), null, null);
            DataSet ds = new DataSet();
           // DataSet dsImport = new DataSet();
            DateTime dt = DateTime.Now;

            if (dt.DayOfWeek == DayOfWeek.Saturday)
                dt = dt.AddDays(2);
            else if (dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(1);

            if (files == null)
            {
                return ds;
            }

            int fileCount = Directory.GetFiles(FilePath).Length;

            if (fileCount <= 0) return ds;


            string cDate = dt.ToString("dd-MMM-yyyy");
            string foldername = "CPD_" + cDate;
            string filename = string.Empty;
            //valiDate file exist
            var objFolder = new SettlementList();




            int dscount = 0;
            int filecount = 0;
            int filecount2 = 0;


          //  lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] MEB FILES BEFORE LOOPING THROUGH FILES ARRAY", null, null);
            foreach (FileInfo file in files)
            {
                string conn_str = string.Empty;
                string query = string.Empty;
                filename = file.Name;

                //string conn_str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + file.Directory.FullName +
                //                  ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
               // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ==" + file.FullName, null, null);
                if (file.Extension == ".dbf")
                {
                   // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ITS A DBF FILE" + file.FullName, null, null);
                    if (file.Name.Contains('('))
                    {
                        filename = file.Name.Replace("(", "_").Replace(")", "_");
                        File.Move(file.Directory.FullName + "\\" + file.Name, file.Directory.FullName + "\\" + filename);
                    }

                    //lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] BEFORE PROVIDER", null, null);
                    conn_str = "Provider=VFPOLEDB.1;Data Source=" + file.Directory.FullName + ";Collating Sequence=MACHINE;Password=;";
                    query = "SELECT * FROM " + filename;

                   // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ==== " + query, null, null);
                    using (OleDbConnection conn = new OleDbConnection(conn_str))
                    {
                       // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] CONNECTION STATUS ===" + conn.State.ToString(), null, null);
                        try
                        {
                            using (OleDbCommand cmd = new OleDbCommand(query, conn))
                            {
                                conn.Open();
                               // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] CONNECTION OPENED ===" + conn.State.ToString(), null, null);
                                //////int row_count = (int)cmd.ExecuteScalar();
                                //////Console.WriteLine(row_count);

                                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                DataTable dtb = new DataTable();
                                da.Fill(ds);
                             
                                //if (dtb != null && dtb.Rows.Count > 0)
                                //{
                                //    ds.Tables.Add(dtb);
                                   
                                //}
                                dscount = ds.Tables[0].Rows.Count;
                            
                                filecount++;

                             
                            }

                        
                        }

                        //copyfile to destination folder
                          
                        catch (Exception ex)
                        {
                            filecount2 += 1;

                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERRO  " +  ex.Message , null, null);



                        }
                    }
                }

                //////if (file.Extension == ".xls")
                //////{

                //////    conn_str = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + file.Directory.FullName + "\\" + file + ";Extended Properties=Excel 8.0;";
                //////    ////ConvertListToDataset clds = new ConvertListToDataset();
                //////    ////ds = clds.CreateDataSet(dataList.ToList());
                //////    using (OleDbConnection conn = new OleDbConnection(conn_str))
                //////    {
                //////        conn.Open();
                //////        var sheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                //////        try
                //////        {

                //////            using (var cmd = conn.CreateCommand())
                //////            {
                //////                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //////                cmd.CommandText = "SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString().Trim() + "] ";
                //////                // cmd.CommandText = "SELECT * FROM [Sheet1$]";
                //////                var adapter = new OleDbDataAdapter(cmd);
                //////                ds = new DataSet();
                //////                da.Fill(ds);
                //////            }
                //////            conn.Close();
                //////        }

                //////        //copyfile to destination folder

                //////        catch (Exception ex)
                //////        {
                //////            filecount2 += 1;



                //////        }
                //////    }


                //////}

                //////if (file.Extension == ".xlsx")
                //////{
                //////    conn_str = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file + ";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=2;\"";

                //////    using (OleDbConnection conn = new OleDbConnection(conn_str))
                //////    {
                //////        conn.Open();
                //////        var sheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                //////        try
                //////        {

                //////            using (var cmd = conn.CreateCommand())
                //////            {
                //////                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //////                cmd.CommandText = "SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString().Trim() + "] ";
                //////                // cmd.CommandText = "SELECT * FROM [Sheet1$]";
                //////                var adapter = new OleDbDataAdapter(cmd);
                //////                ds = new DataSet();
                //////                da.Fill(ds);
                //////            }
                //////            conn.Close();
                //////        }

                //////        //copyfile to destination folder

                //////        catch (Exception ex)
                //////        {
                //////            filecount2 += 1;



                //////        }
                //////    }


                //////}
            }
           // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] FILE LOADED INTO ARRAY ===" + dscount, null, null);
           // lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] FILE COUNT===" + filecount, null, null);
            return ds;
        }
        
        public int ProcessSettlement(string batchNo)
        {
            var cnt = PostSMFromSource(batchNo);
           
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Settlement Process Initializing Phase 2", null, null);

            string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
            SettlementProcessNew(dtFrom2);

            return 0;
        }
        public int PostSMFromSource(string batchNo)
        {
            var cnt = 0;
            decimal maxId;
            using (var conn = new RepoBase().OpenConnection(destDb))
            {
               // var sqlMaxCount = "Select CAST(ISNULL(max(ID),0) AS NUMERIC(38,0)) MAXCOUNT from TRAXMASTER_SM where datediff(day,CREATEDATETIME,getdate())=20";
                //var sqlMaxCount = "Select CAST(ISNULL(max(ID),0) AS NUMERIC(38,0)) MAXCOUNT from TRAXMASTER_SM where datediff(day,CREATEDATETIME,getdate())=10";

                //var rst = conn.Query<decimal>(sqlMaxCount, null, commandType: CommandType.Text).FirstOrDefault();
                maxId = 0;
                 
                var sourceData = GetMainTranxSourceData(maxId);


                var sourceIncompleteData = GetMainTranxIncompleteSourceData(maxId);
                try
                {
                    var p = new DynamicParameters();
                    var sqlPostQuery = @"INSERT INTO [TRAXMASTER_SM]
                               (
                                ID
                               ,[SYSDATE]
                               ,[REFNO]
                               ,[CHANNELID]
                               ,[CARDSCHEME]
                               ,[TRANCODE]
                               ,[TRANDESC]
                               ,[TRANDATETIME]
                               ,[ISSUERFIID]
                               ,[ISSUERCURRENCY]
                               ,[ACQUIRERFIID]
                               ,[ACQUIRERCURRENCY]
                               ,[PAN]
                               ,[EXPDATETIME]
                               ,[APPROVALCODE]
                               ,[APPROVALMESSAGE]
                               ,[TERMINALID]
                               ,[MERCHANTID]
                               ,[MERCHANTLOCATION]
                               ,[MCC]
                               ,[STAN]
                               ,[INVOICENO]
                               ,[CUSTOMERNAME]
                               ,[ORGINALAMOUNT]
                               ,[ORGINALCURRENCY]
                               ,[TRANAMOUNT]
                               ,[TRANCURRENCY]
                               ,[SIGN]
                               ,[ISSUERFEE]
                               ,[ISSUERFEECURRENCY]
                               ,[ACQUIRERFEE]
                               ,[ACQUIRERFEECURRENCY]
                               ,[INTERCHANGEFEE]
                               ,[INTERCHANGEFEECUR]
                               ,[SPECIALMESSAGE1]
                               ,[SPECIALMESSAGE2]
                               ,[SPECIALMESSAGE3]
                               ,[SPECIALMESSAGE4]
                               ,[SOURCEDB]
                               ,[SOURCETABLE]
                               ,[REMARK]
                               ,[RECORDID]
                               ,[BATCHNO]
                               ,[CREATEDATETIME]
                               ,[PROCESS_STATUS]
                               ,[AGENT_CODE]
                               ,[PAYMENTREFERENCE]
                               ,[TRANSID]
                               ,[ISVALUEGRANTED]
                               ,[VALUEDATE]
                               ,[BRANCHID]
                               ,[PAYMENTITEMID])
                         VALUES
                               (
                                @ID
                               ,@SYSDATE
                               ,@REFNO
                               ,@CHANNELID
                               ,@CARDSCHEME
                               ,@TRANCODE
                               ,@TRANDESC
                               ,@TRANDATETIME
                               ,@ISSUERFIID
                               ,@ISSUERCURRENCY
                               ,@ACQUIRERFIID
                               ,@ACQUIRERCURRENCY
                               ,@PAN
                               ,@EXPDATETIME
                               ,@APPROVALCODE
                               ,@APPROVALMESSAGE
                               ,@TERMINALID
                               ,@MERCHANTID
                               ,@MERCHANTLOCATION
                               ,@MCC
                               ,@STAN
                               ,@INVOICENO
                               ,@CUSTOMERNAME
                               ,@ORGINALAMOUNT
                               ,@ORGINALCURRENCY
                               ,@TRANAMOUNT
                               ,@TRANCURRENCY
                               ,@SIGN
                               ,@ISSUERFEE
                               ,@ISSUERFEECURRENCY
                               ,@ACQUIRERFEE
                               ,@ACQUIRERFEECURRENCY
                               ,@INTERCHANGEFEE
                               ,@INTERCHANGEFEECUR
                               ,@SPECIALMESSAGE1
                               ,@SPECIALMESSAGE2
                               ,@SPECIALMESSAGE3
                               ,@SPECIALMESSAGE4
                               ,@SOURCEDB
                               ,@SOURCETABLE
                               ,@REMARK
                               ,@RECORDID
                               ,@BATCHNO
                               ,@CREATEDATETIME
                               ,@PROCESS_STATUS
                               ,@AGENT_CODE
                               ,@PAYMENTREFERENCE
                               ,@TRANSID
                               ,@ISVALUEGRANTED
                               ,@VALUEDATE
                               ,@BRANCHID
                               ,@PAYMENTITEMID)";

                    var regexItem = new Regex("!#$%&/()=?»«@£§€{}.-;'<>_,");
                    foreach (var d in sourceData)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(d.PAYMENTREFERENCE))
                            {
                                d.PAYMENTREFERENCE = d.TRANSID;
                            }
                            if(d.ISSUERFIID.Length>4)
                            {
                                d.ISSUERFIID = "000";
                            }

                            if (regexItem.IsMatch(d.TRANSID))
                            {
                                d.TRANSID = "0";
                            }

                            if (d.TRANSID.Contains("/"))
                            {
                                d.TRANSID = "0";
                            }
                            //post transactions into local tranx master for settlement
                            p.Add("@ID", d.ID, DbType.Int64);
                            p.Add("@SYSDATE", d.SYSDATE, DbType.DateTime);
                            p.Add("@REFNO", d.REFNO, DbType.String);
                            p.Add("@CHANNELID", d.CHANNELID, DbType.Int32);
                            p.Add("@CARDSCHEME", d.CARDSCHEME, DbType.String);
                            p.Add("@TRANCODE", d.TRANCODE, DbType.String);
                            p.Add("@TRANDESC", d.TRANDESC, DbType.String);
                            p.Add("@TRANDATETIME", d.TRANDATETIME, DbType.DateTime);
                            p.Add("@ISSUERFIID", d.ISSUERFIID, DbType.String);
                            p.Add("@ISSUERCURRENCY", d.ISSUERCURRENCY, DbType.String);
                            p.Add("@ACQUIRERFIID", d.ACQUIRERFIID, DbType.String);
                            p.Add("@ACQUIRERCURRENCY", d.ACQUIRERCURRENCY, DbType.String);
                            p.Add("@PAN", d.PAN, DbType.String);
                            p.Add("@EXPDATETIME", d.EXPDATETIME, DbType.DateTime);
                            p.Add("@APPROVALCODE", d.APPROVALCODE, DbType.String);
                            p.Add("@APPROVALMESSAGE", d.APPROVALMESSAGE, DbType.String);
                            p.Add("@TERMINALID", d.TERMINALID, DbType.String);
                            p.Add("@MERCHANTID", d.MERCHANTID, DbType.String);
                            p.Add("@MERCHANTLOCATION", d.MERCHANTLOCATION, DbType.String);
                            p.Add("@MCC", d.MCC, DbType.String);
                            p.Add("@STAN", d.STAN, DbType.String);
                            p.Add("@INVOICENO", d.INVOICENO, DbType.String);
                            p.Add("@CUSTOMERNAME", d.CUSTOMERNAME, DbType.String);
                            p.Add("@ORGINALAMOUNT", d.ORGINALAMOUNT, DbType.Decimal);
                            p.Add("@ORGINALCURRENCY", d.ORGINALCURRENCY, DbType.String);
                            p.Add("@TRANAMOUNT", d.TRANAMOUNT, DbType.Decimal);
                            p.Add("@TRANCURRENCY", d.TRANCURRENCY, DbType.String);
                            p.Add("@SIGN", d.SIGN, DbType.String);
                            p.Add("@ISSUERFEE", d.ISSUERFEE, DbType.Decimal);
                            p.Add("@ISSUERFEECURRENCY", d.ISSUERFEECURRENCY, DbType.String);
                            p.Add("@ACQUIRERFEE", d.ACQUIRERFEE, DbType.Decimal);
                            p.Add("@ACQUIRERFEECURRENCY", d.ACQUIRERFEECURRENCY, DbType.String);
                            p.Add("@INTERCHANGEFEE", d.INTERCHANGEFEE, DbType.Decimal);
                            p.Add("@INTERCHANGEFEECUR", d.INTERCHANGEFEECUR, DbType.String);
                            p.Add("@SPECIALMESSAGE1", d.SPECIALMESSAGE1, DbType.String);
                            p.Add("@SPECIALMESSAGE2", d.SPECIALMESSAGE2, DbType.String);
                            p.Add("@SPECIALMESSAGE3", d.SPECIALMESSAGE3, DbType.String);
                            p.Add("@SPECIALMESSAGE4", d.SPECIALMESSAGE4, DbType.String);
                            p.Add("@SOURCEDB", d.SOURCEDB, DbType.String);
                            p.Add("@SOURCETABLE", d.SOURCETABLE, DbType.String);
                            p.Add("@REMARK", d.REMARK, DbType.String);
                            p.Add("@RECORDID", d.RECORDID, DbType.Decimal);
                            p.Add("@BATCHNO", d.BATCHNO, DbType.Decimal);
                            p.Add("@CREATEDATETIME", d.CREATEDATETIME, DbType.DateTime);
                            p.Add("@PROCESS_STATUS", "A", DbType.String);
                            p.Add("@AGENT_CODE", d.AGENT_CODE, DbType.String);
                            p.Add("@PAYMENTREFERENCE", d.PAYMENTREFERENCE, DbType.String);
                            p.Add("@TRANSID", d.TRANSID, DbType.String);
                            p.Add("@ISVALUEGRANTED", d.ISVALUEGRANTED, DbType.String);
                            p.Add("@VALUEDATE", d.VALUEDATE, DbType.DateTime);
                            p.Add("@BRANCHID", d.BRANCHID, DbType.String);
                            p.Add("@PAYMENTITEMID", d.PAYMENTITEMID, DbType.Int64);

                            cnt += conn.Execute(sqlPostQuery, p, commandType: CommandType.Text);
                        }
                        catch(Exception ex)
                        {
                             lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB POSTING ERROR] ===", ex.Message + " " + d.REFNO , null);

                        }
                    }

                    var sqlIncompletePostQuery = @"INSERT INTO [TRAXMASTER_SM_INCOMPLETE]
                               (
                                ID
                               ,[SYSDATE]
                               ,[REFNO]
                               ,[CHANNELID]
                               ,[CARDSCHEME]
                               ,[TRANCODE]
                               ,[TRANDESC]
                               ,[TRANDATETIME]
                               ,[ISSUERFIID]
                               ,[ISSUERCURRENCY]
                               ,[ACQUIRERFIID]
                               ,[ACQUIRERCURRENCY]
                               ,[PAN]
                               ,[EXPDATETIME]
                               ,[APPROVALCODE]
                               ,[APPROVALMESSAGE]
                               ,[TERMINALID]
                               ,[MERCHANTID]
                               ,[MERCHANTLOCATION]
                               ,[MCC]
                               ,[STAN]
                               ,[INVOICENO]
                               ,[CUSTOMERNAME]
                               ,[ORGINALAMOUNT]
                               ,[ORGINALCURRENCY]
                               ,[TRANAMOUNT]
                               ,[TRANCURRENCY]
                               ,[SIGN]
                               ,[ISSUERFEE]
                               ,[ISSUERFEECURRENCY]
                               ,[ACQUIRERFEE]
                               ,[ACQUIRERFEECURRENCY]
                               ,[INTERCHANGEFEE]
                               ,[INTERCHANGEFEECUR]
                               ,[SPECIALMESSAGE1]
                               ,[SPECIALMESSAGE2]
                               ,[SPECIALMESSAGE3]
                               ,[SPECIALMESSAGE4]
                               ,[SOURCEDB]
                               ,[SOURCETABLE]
                               ,[REMARK]
                               ,[RECORDID]
                               ,[BATCHNO]
                               ,[CREATEDATETIME]
                               ,[PROCESS_STATUS]
                               ,[AGENT_CODE]
                               ,[PAYMENTREFERENCE]
                               ,[TRANSID]
                               ,[ISVALUEGRANTED]
                               ,[VALUEDATE]
                               ,[BRANCHID]
                               ,[PAYMENTITEMID])
                         VALUES
                               (
                                @ID
                               ,@SYSDATE
                               ,@REFNO
                               ,@CHANNELID
                               ,@CARDSCHEME
                               ,@TRANCODE
                               ,@TRANDESC
                               ,@TRANDATETIME
                               ,@ISSUERFIID
                               ,@ISSUERCURRENCY
                               ,@ACQUIRERFIID
                               ,@ACQUIRERCURRENCY
                               ,@PAN
                               ,@EXPDATETIME
                               ,@APPROVALCODE
                               ,@APPROVALMESSAGE
                               ,@TERMINALID
                               ,@MERCHANTID
                               ,@MERCHANTLOCATION
                               ,@MCC
                               ,@STAN
                               ,@INVOICENO
                               ,@CUSTOMERNAME
                               ,@ORGINALAMOUNT
                               ,@ORGINALCURRENCY
                               ,@TRANAMOUNT
                               ,@TRANCURRENCY
                               ,@SIGN
                               ,@ISSUERFEE
                               ,@ISSUERFEECURRENCY
                               ,@ACQUIRERFEE
                               ,@ACQUIRERFEECURRENCY
                               ,@INTERCHANGEFEE
                               ,@INTERCHANGEFEECUR
                               ,@SPECIALMESSAGE1
                               ,@SPECIALMESSAGE2
                               ,@SPECIALMESSAGE3
                               ,@SPECIALMESSAGE4
                               ,@SOURCEDB
                               ,@SOURCETABLE
                               ,@REMARK
                               ,@RECORDID
                               ,@BATCHNO
                               ,@CREATEDATETIME
                               ,@PROCESS_STATUS
                               ,@AGENT_CODE
                               ,@PAYMENTREFERENCE
                               ,@TRANSID
                               ,@ISVALUEGRANTED
                               ,@VALUEDATE
                               ,@BRANCHID
                               ,@PAYMENTITEMID)";
                    foreach (var d in sourceIncompleteData)
                    {
                        try
                        {
                            //post transactions into local tranx master for settlement
                            p.Add("@ID", d.ID, DbType.Int64);
                            p.Add("@SYSDATE", d.SYSDATE, DbType.DateTime);
                            p.Add("@REFNO", d.REFNO, DbType.String);
                            p.Add("@CHANNELID", d.CHANNELID, DbType.Int32);
                            p.Add("@CARDSCHEME", d.CARDSCHEME, DbType.String);
                            p.Add("@TRANCODE", d.TRANCODE, DbType.String);
                            p.Add("@TRANDESC", d.TRANDESC, DbType.String);
                            p.Add("@TRANDATETIME", d.TRANDATETIME, DbType.DateTime);
                            p.Add("@ISSUERFIID", d.ISSUERFIID, DbType.String);
                            p.Add("@ISSUERCURRENCY", d.ISSUERCURRENCY, DbType.String);
                            p.Add("@ACQUIRERFIID", d.ACQUIRERFIID, DbType.String);
                            p.Add("@ACQUIRERCURRENCY", d.ACQUIRERCURRENCY, DbType.String);
                            p.Add("@PAN", d.PAN, DbType.String);
                            p.Add("@EXPDATETIME", d.EXPDATETIME, DbType.DateTime);
                            p.Add("@APPROVALCODE", d.APPROVALCODE, DbType.String);
                            p.Add("@APPROVALMESSAGE", d.APPROVALMESSAGE, DbType.String);
                            p.Add("@TERMINALID", d.TERMINALID, DbType.String);
                            p.Add("@MERCHANTID", d.MERCHANTID, DbType.String);
                            p.Add("@MERCHANTLOCATION", d.MERCHANTLOCATION, DbType.String);
                            p.Add("@MCC", d.MCC, DbType.String);
                            p.Add("@STAN", d.STAN, DbType.String);
                            p.Add("@INVOICENO", d.INVOICENO, DbType.String);
                            p.Add("@CUSTOMERNAME", d.CUSTOMERNAME, DbType.String);
                            p.Add("@ORGINALAMOUNT", d.ORGINALAMOUNT, DbType.Decimal);
                            p.Add("@ORGINALCURRENCY", d.ORGINALCURRENCY, DbType.String);
                            p.Add("@TRANAMOUNT", d.TRANAMOUNT, DbType.Decimal);
                            p.Add("@TRANCURRENCY", d.TRANCURRENCY, DbType.String);
                            p.Add("@SIGN", d.SIGN, DbType.String);
                            p.Add("@ISSUERFEE", d.ISSUERFEE, DbType.Decimal);
                            p.Add("@ISSUERFEECURRENCY", d.ISSUERFEECURRENCY, DbType.String);
                            p.Add("@ACQUIRERFEE", d.ACQUIRERFEE, DbType.Decimal);
                            p.Add("@ACQUIRERFEECURRENCY", d.ACQUIRERFEECURRENCY, DbType.String);
                            p.Add("@INTERCHANGEFEE", d.INTERCHANGEFEE, DbType.Decimal);
                            p.Add("@INTERCHANGEFEECUR", d.INTERCHANGEFEECUR, DbType.String);
                            p.Add("@SPECIALMESSAGE1", d.SPECIALMESSAGE1, DbType.String);
                            p.Add("@SPECIALMESSAGE2", d.SPECIALMESSAGE2, DbType.String);
                            p.Add("@SPECIALMESSAGE3", d.SPECIALMESSAGE3, DbType.String);
                            p.Add("@SPECIALMESSAGE4", d.SPECIALMESSAGE4, DbType.String);
                            p.Add("@SOURCEDB", d.SOURCEDB, DbType.String);
                            p.Add("@SOURCETABLE", d.SOURCETABLE, DbType.String);
                            p.Add("@REMARK", d.REMARK, DbType.String);
                            p.Add("@RECORDID", d.RECORDID, DbType.Decimal);
                            p.Add("@BATCHNO", d.BATCHNO, DbType.Decimal);
                            p.Add("@CREATEDATETIME", d.CREATEDATETIME, DbType.DateTime);
                            p.Add("@PROCESS_STATUS", "A", DbType.String);
                            p.Add("@AGENT_CODE", d.AGENT_CODE, DbType.String);
                            p.Add("@PAYMENTREFERENCE", d.PAYMENTREFERENCE, DbType.String);
                            p.Add("@TRANSID", d.TRANSID, DbType.String);
                            p.Add("@ISVALUEGRANTED", d.ISVALUEGRANTED, DbType.String);
                            p.Add("@VALUEDATE", d.VALUEDATE, DbType.DateTime);
                            p.Add("@BRANCHID", d.BRANCHID, DbType.String);
                            p.Add("@PAYMENTITEMID", d.PAYMENTITEMID, DbType.String);

                            cnt += conn.Execute(sqlIncompletePostQuery, p, commandType: CommandType.Text);
                        }
                        catch (Exception ex)
                        {
                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB POSTING ERROR] ===", ex.Message, null);

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                return cnt;
            }
        }

        private List<TRAXMASTER> GetMainTranxSourceData(decimal maxId)
        {
            try
            {
                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    var p = new DynamicParameters();
                    p.Add("@P_MAXID", maxId, DbType.Decimal);
                    //var sqlQuery = @"select A.* FROM
                    //            (SELECT [ID],[SYSDATE],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY]
                    //                   ,[ACQUIRERFIID],[ACQUIRERCURRENCY],[PAN],[EXPDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID]
                    //                   ,[MERCHANTLOCATION],[MCC],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT],[ORGINALCURRENCY],[TRANAMOUNT]
                    //                   ,[TRANCURRENCY],[SIGN],[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[INTERCHANGEFEE]
                    //                   ,[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4],[SOURCEDB]
                    //                   ,[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[CREATEDATETIME],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID],[ISVALUEGRANTED]
                    //                   ,[VALUEDATE]
                    //            FROM   [TRAXMASTER_SM]
                    //             where convert(date,CREATEDATETIME) >= CONVERT(date,DATEADD(day,-1 ,GETDATE()))
							             //                    and ID > @P_MAXID AND CARDSCHEME = 'ECSH' AND ISVALUEGRANTED = '00'
                    //            ) A 
                    //                   LEFT OUTER JOIN SETTLEMENTMASTER..SM_SETTLEMENTDETAIL B 
                    //                     ON A.TRANSID = B.TRANSID 
                    //            WHERE  B.TRANSID IS  NULL 
                    //            UNION
                    //            SELECT [ID],[SYSDATE],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY]
                    //                   ,[ACQUIRERFIID],[ACQUIRERCURRENCY],[PAN],[EXPDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID]
                    //                   ,[MERCHANTLOCATION],[MCC],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT],[ORGINALCURRENCY],[TRANAMOUNT]
                    //                   ,[TRANCURRENCY],[SIGN],[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[INTERCHANGEFEE]
                    //                   ,[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4],[SOURCEDB]
                    //                   ,[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[CREATEDATETIME],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID]
                    //                   ,[ISVALUEGRANTED],[VALUEDATE]
                    //             FROM   [TRAXMASTER_SM]
                    //             where convert(date,CREATEDATETIME) >= CONVERT(date,DATEADD(day,-1 ,GETDATE()))
							             //                    and ID > @P_MAXID AND CARDSCHEME <> 'ECSH' AND ISVALUEGRANTED = '00'";
                    var sqlQuery  = "proc_GetTraxMaster_Complete";
                    var rec = conn.Query<TRAXMASTER>(sqlQuery, p,commandTimeout:0, commandType: CommandType.StoredProcedure);
                    return rec.ToList();
                }
            }
            catch(Exception ex)
            {
                lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB LOADING FROM SOURCE DB] ===", ex.Message, null);

            }
            return null;
        }
        private List<TRAXMASTER> GetMainTranxIncompleteSourceData(decimal maxId)
        {
            try
            {
                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    var p = new DynamicParameters();
                    p.Add("@P_MAXID", maxId, DbType.Decimal);
//                    var sqlQuery = @"select A.* FROM
//(SELECT [ID],[SYSDATE],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY]
//       ,[ACQUIRERFIID],[ACQUIRERCURRENCY],[PAN],[EXPDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID]
//       ,[MERCHANTLOCATION],[MCC],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT],[ORGINALCURRENCY],[TRANAMOUNT]
//       ,[TRANCURRENCY],[SIGN],[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[INTERCHANGEFEE]
//       ,[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4],[SOURCEDB]
//       ,[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[CREATEDATETIME],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID],[ISVALUEGRANTED]
//       ,[VALUEDATE]
//FROM   [TRAXMASTER_SM]
// where convert(date,CREATEDATETIME) >= CONVERT(date,DATEADD(day,-1 ,GETDATE()))
//							 and ID > @P_MAXID AND CARDSCHEME = 'ECSH' AND ISVALUEGRANTED <> '00'
//) A 
//       LEFT OUTER JOIN [SETTLEMENTMASTER]..SM_SETTLEMENTDETAIL B 
//         ON A.TRANSID = B.TRANSID 
//WHERE  B.TRANSID IS  NULL 
//UNION
//SELECT [ID],[SYSDATE],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY]
//       ,[ACQUIRERFIID],[ACQUIRERCURRENCY],[PAN],[EXPDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID]
//       ,[MERCHANTLOCATION],[MCC],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT],[ORGINALCURRENCY],[TRANAMOUNT]
//       ,[TRANCURRENCY],[SIGN],[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[INTERCHANGEFEE]
//       ,[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4],[SOURCEDB]
//       ,[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[CREATEDATETIME],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID]
//       ,[ISVALUEGRANTED],[VALUEDATE]
//FROM   [TRAXMASTER_SM]
// where convert(date,CREATEDATETIME) >= CONVERT(date,DATEADD(day,-1 ,GETDATE()))
//							 and ID > @P_MAXID AND CARDSCHEME <> 'ECSH' AND ISVALUEGRANTED <> '00'";
                    var sqlQuery = "proc_GetTraxMaster_InComplete";
                    var rec = conn.Query<TRAXMASTER>(sqlQuery, p,commandTimeout:0, commandType: CommandType.StoredProcedure);
                    return rec.ToList();
                }
            }
            catch (Exception ex)
            {
                lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB LOADING FROM SOURCE DB incomplete transactions] ===", ex.Message, null);

            }
            return null;
        }
        private List<TRAXMASTER> GetLocalTranxSourceData()
        {
            using (var conn = new RepoBase().OpenConnection(destDb))
            {
                var p = new DynamicParameters();
                var sqlQuery = "";
                var rec = conn.Query<TRAXMASTER>(sqlQuery, p, commandType: CommandType.Text);
                return rec.ToList();
            }
        }
        //public int PostSettlement()
        //{
        //    string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
        //    SettlementProcess(dtFrom2);
        //}
        public string MoveData(string FilePath, string targetPath)
        {

            string foldername = "CPD_" + DateTime.Now.ToString("dd-MMM-yyyy");

            files = null;
            files = GetFiles(FilePath);
            // move Folder
            string fileName=string.Empty;
            string destFile=string.Empty;
            string newPath = Path.Combine(targetPath, "CPD_" + DateTime.Now.ToString("yyyy-MM-dd"));
            // string newpathstr = targetPath  + DateTime.Now.ToString("yyyy-MM-dd");

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (Directory.Exists(FilePath))
            {
                // string[] files2 = files; // System.IO.Directory.GetFiles(FilePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (var s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    try
                    {

                        fileName = s.Name; // System.IO.Path.GetFileName(s.f);
                        destFile = Path.Combine(newPath, fileName);
                        if (!File.Exists(destFile))
                        {
                            File.Move(s.FullName, destFile);
                        }
                        else
                        {
                            Random rnd = new Random();
                            int idno = rnd.Next(1, 13);
                            var ext = Path.GetExtension(s.Name);
                            var name = Path.GetFileNameWithoutExtension(s.Name);

                            fileName = name + "_" + idno + ext;
                            destFile = Path.Combine(newPath, fileName);
                            File.Move(s.FullName, destFile);
                        }

                        
                    }

                    catch
                    {

                    }
                    //file.Delete();
                }
            }
            else
            {
                // Console.WriteLine("Source path does not exist!");
            }
            //Delete Folder
            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(FilePath);
            // Delete this dir and all subdirs.
            //try
            //{
            //    foreach (FileInfo file in di.GetFiles())
            //    {
            //        file.Delete();
            //    }
            //    foreach (DirectoryInfo dir in di.GetDirectories())
            //    {
            //        dir.Delete(true);
            //    }
            //}
            //catch (System.IO.IOException e)
            //{

            //    lgfn.loginfo("MEB INSERTION ERROR ............", e.Message, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

            //}

            //}


            return newPath;

        }

        //private ObjCodeDOC LastRecord2()
        //{
        //    decimal Lastcode = 0;
        //    decimal LastDocno = 0;
        //    OracleDataReader reader1 = null;
        //    ObjCodeDOC mdt = null;
        //    OracleConnection Standby_connection = new OracleConnection(oradb);
        //    OracleCommand cmd2 = new OracleCommand();

        //    try
        //    {
        //        string qry2 = "Select code,LASTDOCNO from Posmisdb_mebLastRecord";

        //        OracleCommand cmd = new OracleCommand();
        //        if (Standby_connection == null)
        //        {
        //            Standby_connection = new OracleConnection(oradb);
        //        }
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        cmd2.Connection = Standby_connection;
        //        cmd2.CommandText = qry2;

        //        cmd2.CommandType = CommandType.Text;
        //        // Standby_connection.Open();
        //        using (var dr = cmd2.ExecuteReader())
        //        {
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    mdt = new ObjCodeDOC()
        //                    {
        //                        Code = dr[0] != null ? decimal.Parse(dr[0].ToString().Trim()) : (decimal?)null, // dr.GetString(0),
        //                        Docno = dr[1] != null ? decimal.Parse(dr[1].ToString().Trim()) : (decimal?)null,
        //                    };
        //                }

        //            }
        //            cmd2.Dispose();
        //            //if (dr != null)
        //            //{
        //            //    dr.Close();
        //            //}
        //            return mdt;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //private ObjCodeDOC LastRecord(int branchcode)
        //{
        //    decimal Lastcode = 0;
        //    decimal LastDocno = 0;
        //    OracleDataReader reader1 = null;
        //    ObjCodeDOC mdt = null;
        //    OracleConnection Standby_connection = new OracleConnection(oradb);
        //    OracleCommand cmd2 = new OracleCommand();

           
        //    lgfn.loginfoMSG("Level: B", null, DateTime.Now.ToString());
           
        //    try
        //    {
        //        ////string qry2 = "Select code,LASTDOCNO from Posmisdb_mebLastRecord where branchcode=" + branchcode;
        //        string qry2 = "Select code,LASTDOCNO from Posmisdb_mebLastRecord where branchcode=" + branchcode;


        //        OracleCommand cmd = new OracleCommand();
        //        if (Standby_connection == null)
        //        {
        //            Standby_connection = new OracleConnection(oradb);
        //        }
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        cmd2.Connection = Standby_connection;
        //        cmd2.CommandText = qry2;

        //        cmd2.CommandType = CommandType.Text;
        //        // Standby_connection.Open();
        //        using (var dr = cmd2.ExecuteReader())
        //        {
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    mdt = new ObjCodeDOC()
        //                    {
        //                        Code = dr[0] != null ? decimal.Parse(dr[0].ToString().Trim()) : (decimal?)null, // dr.GetString(0),
        //                        Docno = dr[1] != null ? decimal.Parse(dr[1].ToString().Trim()) : (decimal?)null,
        //                    };
        //                }

        //            }
        //            cmd2.Dispose();
        //            //if (dr != null)
        //            //{
        //            //    dr.Close();
        //            //}
        //            return mdt;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //private ObjCodeDOC LastRecord_NEW(string ACQRFIID)
        //{
        //    decimal Lastcode = 0;
        //    decimal LastDocno = 0;
        //    OracleDataReader reader1 = null;
        //    ObjCodeDOC mdt = null;
        //    OracleConnection Standby_connection = new OracleConnection(oradb);
        //    OracleCommand cmd2 = new OracleCommand();

           

        //    lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO  Last Code Fetching Loop for " + ACQRFIID, null, null);

           
        //    try
        //    {
        //        ////string qry2 = "Select code,LASTDOCNO from Posmisdb_mebLastRecord where branchcode=" + branchcode;
        //        string qry2 = "Select max(code) code,max(docno) LASTDOCNO from posmisdb_meb where code is not null and ACQFIID='" + ACQRFIID + "'";


        //        OracleCommand cmd = new OracleCommand();
        //        if (Standby_connection == null)
        //        {
        //            Standby_connection = new OracleConnection(oradb);
        //        }
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        cmd2.Connection = Standby_connection;
        //        cmd2.CommandText = qry2;

        //        cmd2.CommandType = CommandType.Text;
        //        // Standby_connection.Open();
        //        using (var dr = cmd2.ExecuteReader())
        //        {
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    mdt = new ObjCodeDOC()
        //                    {
        //                        Code = dr[0] != null ? decimal.Parse(dr[0].ToString().Trim()) : (decimal?)null, // dr.GetString(0),
        //                        Docno = dr[1] != null ? decimal.Parse(dr[1].ToString().Trim()) : (decimal?)null,
        //                    };
        //                }

        //            }
        //            cmd2.Dispose();
        //            //if (dr != null)
        //            //{
        //            //    dr.Close();
        //            //}
        //            return mdt;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        private int Process_State(DbConnection con)
        {
            try
            {
                string qry2 = "select count(1) from sm_company_profile where PROCESS_FLAG <> 'P'";

                var rst = con.Query<int>(qry2, null, commandType: CommandType.Text).FirstOrDefault();
               
                return rst;
                
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        decimal Lastcode = 0;
        decimal LastDocno = 0;


        ////private OracleDataReader processNonQuery(decimal LastRec, int BranchCode)
        ////{


        ////    OracleDataReader reader = default(OracleDataReader);
        ////    try
        ////    {
               
        ////        lgfn.loginfoMSG("Last Branch Code: " + LastRec + " Branch: " + BranchCode, null, DateTime.Now.ToString());
               

        ////        Stopwatch se = new Stopwatch();
        ////        se.Start();
               
        ////        lgfn.loginfoMSG("MEB Collection From TWCMS Started", " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
               

        ////        /*  string qry = string.Format(@"select OPDate, CLERKCODE, STATION, DOCNO, NO, ENTCODE, DEBITACCOUNT DEBITACCT,VALUE DEBITVAL, CREDITACCOUNT CREDITACCT,CREDITVALUE CREDITVAL, SHORTREMARK SHORTREM, 
        ////                  FULLREMARK FULLREM, DEVICE, ENTCODE EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////                  TERMCODE,TERM,RETAILER,TERMLOCATION TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, AMOUNT TRANAMOUNT,CURRENCY TRANCUR, ORGAMOUNT, ORGCURRENCY ORGCUR,ACQFEE,ACQFEECURRENCY ACQFEECUR,
        ////                  ISSFEE, ISSFEECURRENCY ISSFEECUR,INTRFEE,INTRFEECURRENCY INTRFEECUR, IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, DEBIT_CURRENCY DEBITCUR,CREDIT_CURRENCY CREDITCUR, CUSTREVENUENUM CRNUM,BORDERINSPECTFEE BIFEE, USERDEFINED USDFN, 
        ////                  USERDEFINED2 USDFN2, USERDEFINED3 USDFN3,USERDEFINED BATCHNO,CODE
        ////                  from a4m.textranmeb where branch=" + BranchCode + " and code>" + LastRec + " and rownum<{0} order by code", recordCount); */

        ////        //string qry = @"select OPDate, CLERKCODE, STATION, DOCNO, NO, ENTCODE, DEBITACCOUNT DEBITACCT,VALUE DEBITVAL, CREDITACCOUNT CREDITACCT,CREDITVALUE CREDITVAL, SHORTREMARK SHORTREM, 
        ////        //        FULLREMARK FULLREM, DEVICE, ENTCODE EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////        //        TERMCODE,TERM,RETAILER,TERMLOCATION TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, AMOUNT TRANAMOUNT,CURRENCY TRANCUR, ORGAMOUNT, ORGCURRENCY ORGCUR,ACQFEE,ACQFEECURRENCY ACQFEECUR,
        ////        //        ISSFEE, ISSFEECURRENCY ISSFEECUR,INTRFEE,INTRFEECURRENCY INTRFEECUR, IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, DEBIT_CURRENCY DEBITCUR,CREDIT_CURRENCY CREDITCUR, CUSTREVENUENUM CRNUM,BORDERINSPECTFEE BIFEE, USERDEFINED USDFN, 
        ////        //        USERDEFINED2 USDFN2, USERDEFINED3 USDFN3,USERDEFINED BATCHNO,CODE
        ////        //        from a4m.textranmeb where branch=" + BranchCode + " and code>=" + LastRec + " and rownum <10001 order by code";

        ////        string qry = @"select OPDate, CLERKCODE, STATION, DOCNO, NO, ENTCODE, DEBITACCOUNT DEBITACCT,VALUE DEBITVAL, CREDITACCOUNT CREDITACCT,CREDITVALUE CREDITVAL, SHORTREMARK SHORTREM, 
        ////                FULLREMARK FULLREM, DEVICE, ENTCODE EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////                TERMCODE,TERM,RETAILER,TERMLOCATION TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, AMOUNT TRANAMOUNT,CURRENCY TRANCUR, ORGAMOUNT, ORGCURRENCY ORGCUR,ACQFEE,ACQFEECURRENCY ACQFEECUR,
        ////                ISSFEE, ISSFEECURRENCY ISSFEECUR,INTRFEE,INTRFEECURRENCY INTRFEECUR, IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, DEBIT_CURRENCY DEBITCUR,CREDIT_CURRENCY CREDITCUR, CUSTREVENUENUM CRNUM,BORDERINSPECTFEE BIFEE, USERDEFINED USDFN, 
        ////                USERDEFINED2 USDFN2, USERDEFINED3 USDFN3,USERDEFINED BATCHNO,CODE
        ////                from a4m.textranmeb where branch=" + BranchCode + " and code>=" + LastRec + " order by code";


        ////        OracleConnection Standby_connection = new OracleConnection(oradb2);
        ////        OracleCommand cmd = new OracleCommand();

        ////        try
        ////        {
        ////            cmd.Connection = Standby_connection;
        ////            cmd.CommandText = qry;

        ////            cmd.CommandType = CommandType.Text;
        ////            Standby_connection.Open();
        ////            reader = cmd.ExecuteReader();
        ////        }
        ////        catch (System.Data.SqlClient.SqlException ex)
        ////        {


        ////            if (!ex.Message.ToString().Contains(res))
        ////            {
                       
        ////                lgfn.loginfoMSG("Error During Data Spooling From TWCMS:", ex.Message, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                       
        ////            }

        ////        }
        ////        se.Stop();
        ////        TimeSpan ts = se.Elapsed;
        ////        string eT = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);


               
        ////        lgfn.loginfoMSG("MEB Collection From TWCMS Completed:" + eT, " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
               

        ////        return reader;

        ////    }
        ////    catch (System.Data.SqlClient.SqlException ex)
        ////    {
               
        ////        lgfn.loginfoMSG("Error During Data Spooling From TWCMS:", ex.Message, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
               



        ////        return reader;
        ////    }

        ////}

        ////private OracleDataReader processNonQuery_NEW(decimal LastRec, string ACQFIID)
        ////{


        ////    OracleDataReader reader = default(OracleDataReader);
        ////    try
        ////    {
               

        ////        Stopwatch se = new Stopwatch();
        ////        se.Start();
               
        ////        lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO  TWCMS Process Initializing for ACQFIID:" + ACQFIID +  " CODE:" + LastRec, null, null);
                

        ////        /*  string qry = string.Format(@"select OPDate, CLERKCODE, STATION, DOCNO, NO, ENTCODE, DEBITACCOUNT DEBITACCT,VALUE DEBITVAL, CREDITACCOUNT CREDITACCT,CREDITVALUE CREDITVAL, SHORTREMARK SHORTREM, 
        ////                  FULLREMARK FULLREM, DEVICE, ENTCODE EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////                  TERMCODE,TERM,RETAILER,TERMLOCATION TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, AMOUNT TRANAMOUNT,CURRENCY TRANCUR, ORGAMOUNT, ORGCURRENCY ORGCUR,ACQFEE,ACQFEECURRENCY ACQFEECUR,
        ////                  ISSFEE, ISSFEECURRENCY ISSFEECUR,INTRFEE,INTRFEECURRENCY INTRFEECUR, IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, DEBIT_CURRENCY DEBITCUR,CREDIT_CURRENCY CREDITCUR, CUSTREVENUENUM CRNUM,BORDERINSPECTFEE BIFEE, USERDEFINED USDFN, 
        ////                  USERDEFINED2 USDFN2, USERDEFINED3 USDFN3,USERDEFINED BATCHNO,CODE
        ////                  from a4m.textranmeb where branch=" + BranchCode + " and code>" + LastRec + " and rownum<{0} order by code", recordCount); */

        ////        //string qry = @"select OPDate, CLERKCODE, STATION, DOCNO, NO, ENTCODE, DEBITACCOUNT DEBITACCT,VALUE DEBITVAL, CREDITACCOUNT CREDITACCT,CREDITVALUE CREDITVAL, SHORTREMARK SHORTREM, 
        ////        //        FULLREMARK FULLREM, DEVICE, ENTCODE EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////        //        TERMCODE,TERM,RETAILER,TERMLOCATION TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, AMOUNT TRANAMOUNT,CURRENCY TRANCUR, ORGAMOUNT, ORGCURRENCY ORGCUR,ACQFEE,ACQFEECURRENCY ACQFEECUR,
        ////        //        ISSFEE, ISSFEECURRENCY ISSFEECUR,INTRFEE,INTRFEECURRENCY INTRFEECUR, IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, DEBIT_CURRENCY DEBITCUR,CREDIT_CURRENCY CREDITCUR, CUSTREVENUENUM CRNUM,BORDERINSPECTFEE BIFEE, USERDEFINED USDFN, 
        ////        //        USERDEFINED2 USDFN2, USERDEFINED3 USDFN3,USERDEFINED BATCHNO,CODE
        ////        //        from a4m.textranmeb where branch=" + BranchCode + " and code>=" + LastRec + " and rownum <10001 order by code";

        ////        string qry = @"select OPDate, CLERKCODE, STATION, DOCNO, NO, ENTCODE, DEBITACCOUNT DEBITACCT,VALUE DEBITVAL, CREDITACCOUNT CREDITACCT,CREDITVALUE CREDITVAL, SHORTREMARK SHORTREM, 
        ////                FULLREMARK FULLREM, DEVICE, ENTCODE EXTENTCODE,TRANDate,ISSFICODE,ISSFIID,ISSPS,ISSCOUNTRY,PAN,MBR,EXPDate,APPROVAL,ACQFICODE,ACQFIID,ACQPS,ACQCOUNTRY, 
        ////                TERMCODE,TERM,RETAILER,TERMLOCATION TLOCATION, MCC, STAN, ACQIIN, FORWARDIIN, INVOICE, USERDATA, AMOUNT TRANAMOUNT,CURRENCY TRANCUR, ORGAMOUNT, ORGCURRENCY ORGCUR,ACQFEE,ACQFEECURRENCY ACQFEECUR,
        ////                ISSFEE, ISSFEECURRENCY ISSFEECUR,INTRFEE,INTRFEECURRENCY INTRFEECUR, IDENT1, INFO1, IDENT2, INFO2, IDENT3, INFO3, DEBIT_CURRENCY DEBITCUR,CREDIT_CURRENCY CREDITCUR, CUSTREVENUENUM CRNUM,BORDERINSPECTFEE BIFEE, USERDEFINED USDFN, 
        ////                USERDEFINED2 USDFN2, USERDEFINED3 USDFN3,USERDEFINED BATCHNO,CODE
        ////                from a4m.textranmeb where ACQFIID='" + ACQFIID + "' and code>=" + LastRec + " order by code";


        ////        OracleConnection Standby_connection = new OracleConnection(oradb2);
        ////        OracleCommand cmd = new OracleCommand();

        ////        try
        ////        {
        ////            cmd.Connection = Standby_connection;
        ////            cmd.CommandText = qry;

        ////            cmd.CommandType = CommandType.Text;
        ////            Standby_connection.Open();
        ////            reader = cmd.ExecuteReader();
        ////        }
        ////        catch (System.Data.SqlClient.SqlException ex)
        ////        {


        ////            if (!ex.Message.ToString().Contains(res))
        ////            {
                        
        ////                lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + ex.Message, null, null);

        ////            }

        ////        }
        ////        se.Stop();
        ////        TimeSpan ts = se.Elapsed;
        ////        string eT = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);


               
        ////        lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] INFO  TWCMS Process Completed for ACQFIID:" + ACQFIID + " CODE:" + LastRec, null, null);

               

        ////        return reader;

        ////    }
        ////    catch (System.Data.SqlClient.SqlException ex)
        ////    {
               
        ////        lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + ex.Message, null, null);
    

        ////        return reader;
        ////    }

        ////}


        ////private OracleDataReader fetchAllAcquirer()
        ////{


        ////    OracleDataReader reader2 = null;

        ////    reader2 = default(OracleDataReader);

        ////    try
        ////    {

        ////       string qry = "Select ACQFIID,branchcode from POSMISB_ACQRMAPPING";
        ////        OracleConnection Standby_connection = new OracleConnection(oradb);
        ////        OracleCommand cmd = new OracleCommand();
        ////        cmd.Connection = Standby_connection;
        ////        cmd.CommandText = qry;

        ////        cmd.CommandType = CommandType.Text;
        ////        Standby_connection.Open();
        ////        reader2 = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                
                 
        ////        return reader2;

        ////    }
        ////    catch (System.Data.SqlClient.SqlException ex)
        ////    {
               
        ////        lgfn.loginfoMSG(DateTime.Now.ToString() + " [MEB PROCESS] ERROR " + ex.Message, null, null);



        ////        return null;
        ////    }

        ////}
    }
}
