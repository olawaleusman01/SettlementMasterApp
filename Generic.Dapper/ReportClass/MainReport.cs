using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using System.Data.SqlClient;
using System.Dynamic;
using System;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

using DocumentFormat.OpenXml;

using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection;
using OfficeOpenXml;
using UPPosMaster.Dapper.Data;
using System.Runtime.InteropServices;
using Generic.Dapper.Repository;
using System.Configuration;
using UPPosMaster.Dapper.ReportClass;

namespace Generic.Dapper.ReportClass
{
    public class MainReport : RepoBase, IMainReport

    {
        //string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;
        //private static string oradb = ConfigurationManager.AppSettings["TestDB"].ToString();
        private static string sourceDb = ConfigurationManager.AppSettings["DEST_DB"].ToString();

        public string RPT_GenSettlementMASTER(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        {
            LogFunction2.WriteMaintenaceLogToFile("IN........RPT_GenSettlementMASTER");
            //LogFunction2.WriteMaintenaceLogToFile("IN2........RPT_GenSettlementMASTER");

            var SETT = SETTDATE;

            string settlementpathdate = Path.Combine(reportPATH, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));

            //LogFunction2.WriteMaintenaceLogToFile("IN........RPT_GenSettlementMASTER" + settlementpathdate);

            if (!Directory.Exists(settlementpathdate))
            {
                Directory.CreateDirectory(settlementpathdate);
            }


            string output = string.Empty;
            //string bankName = string.Empty;
            //string bankshort = string.Empty;
            //string cbncode = string.Empty;
            //string cardScheme = string.Empty;
            string RECON_FILE = Path.Combine(settlementpathdate, "SETTLEMENT_MASTER");

            if (!Directory.Exists(RECON_FILE))
            {
                Directory.CreateDirectory(RECON_FILE);
            }

            string reportname = "SETTLEMENT_DETAIL.xlsx";
            string reportnameMAIN = RECON_FILE + "\\" + reportname;
            try
            {
                if (File.Exists(reportnameMAIN))
                {

                    File.Delete(reportnameMAIN);
                }
            }
            catch
            {

            }
            //LogFunction2.WriteMaintenaceLogToFile("IN........RPT_GenSettlementMASTER" + RECON_FILE);

            output = rptSett.GenSettlementAll("", "ALL", "U", SETT, reportPATH, RECON_FILE, logopath, "NAIRA SETTLEMENT", "DOM");
            return output;
        }
        

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }
        public string RPT_ACQUIREREISSMDB(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        {
            //PARTY, ISSUER, ACQUIRER AND DASHBOARD


            DateTime SETT;

            SETT = SETTDATE;

            string settlementpathdate = Path.Combine(reportPATH, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));


            if (!Directory.Exists(settlementpathdate))
            {
                Directory.CreateDirectory(settlementpathdate);
            }


            string output = string.Empty;
            string bankName = string.Empty;
            string bankshort = string.Empty;
            string cbncode = string.Empty;
            string cardScheme = string.Empty;





            string DASHBOARDPathMerchant = Path.Combine(settlementpathdate, "UP_DASHBOARD");

            if (!Directory.Exists(DASHBOARDPathMerchant))
            {
                Directory.CreateDirectory(DASHBOARDPathMerchant);
            }


            dsLoadclass dv = new dsLoadclass();



            // output = rptUPDashboard.GenReport1("UP", null, SETT, reportPATH, DASHBOARDPathMerchant, logopath);

            //BANKS ACQUIRER REPORTS
            //ACQUIRER REPORTS

            //////using (var con = new SqlConnection(sourceDb))
            //////{
            //////    con.Open();
            //////    string qryISSA = "Select * from SM_INSTITUTION where lower(status)='active' and cbn_code in (select MERCHANTDEPOSITBANKCODE from SM_SETTLEMENTDETAIL where SETTLEMENTDATETIME='" + SETT.ToString("dd-MMM-yyyy") + "') order by INSTITUTION_NAME";

            //////    using (var cmd = new SqlCommand(qryISSA, con))
            //////    {

            //////        cmd.CommandTimeout = 0;

            //////        SqlDataReader readerA = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //////          //  var recListISS = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qryISS, null, commandTimeout: 3000, commandType: CommandType.Text), conString);
            //////        using (SqlDataReader reader = readerA)
            //////        {
            //////            while (reader.Read())
            //////            {
            //////                bankName = reader["INSTITUTION_NAME"].ToString();
            //////                bankshort = reader["INSTITUTION_SHORTCODE"].ToString();
            //////                cbncode = reader["CBN_CODE"].ToString();

            //////                string FILENAME_PATH = Path.Combine(settlementpathdate, bankName);

            //////                if (!Directory.Exists(FILENAME_PATH))
            //////                {
            //////                    Directory.CreateDirectory(FILENAME_PATH);
            //////                }

            //////                try
            //////                {


            //////                    //output = rptIssuer.GenReport1(bankName, bankshort, SETT, reportPATH, FILENAME_PATH, logopath, "NAIRA SETTLEMENT", "DOM");


            //////                    //output = rptMDB.GenReport1(bankName, cbncode, SETT, reportPATH, FILENAME_PATH, logopath, "NAIRA SETTLEMENT", "DOM");




            //////                }
            //////                catch (Exception ex)
            //////                {

            //////                }

            //////            }
            //////            reader.Dispose();
            //////        }





            //////    }


            //////}


            string qry = "Select * from SM_INSTITUTION where lower(status)='active' AND CBN_CODE IN (SELECT DISTINCT CBN_CODE FROM SM_MCCMSC WHERE CBN_CODE IS NOT NULL) order by INSTITUTION_NAME";
            using (SqlDataReader reader = dv.GetREcordCount(qry))
            {
                while (reader.Read())
                {

                    //BANK LIST

                    bankName = reader["INSTITUTION_NAME"].ToString();
                    bankshort = reader["INSTITUTION_SHORTCODE"].ToString();
                    cbncode = reader["CBN_CODE"].ToString();

                    string FILENAME_PATH = Path.Combine(settlementpathdate, bankName);

                    if (!Directory.Exists(FILENAME_PATH))
                    {
                        Directory.CreateDirectory(FILENAME_PATH);
                    }

                    try
                    {



                        output = rptAcquirer.GenReport1(bankName, bankshort, SETT, reportPATH, FILENAME_PATH, logopath, "NAIRA SETTLEMENT", "DOM");
                    


                    }
                    catch (Exception ex)
                    {

                    }


                }

            }

            //ISSUER REPORTS
            //string qryISS = "Select * from POSMISDB_INSTITUTION where lower(status)='active' order by INSTITUTION_NAME";
            string qryISS = "Select * from SM_INSTITUTION where lower(status)='active' and cbn_code in (select MERCHANTDEPOSITBANKCODE from SM_SETTLEMENTDETAIL where SETTLEMENTDATETIME='" + SETT.ToString("dd-MMM-yyyy") + "') order by INSTITUTION_NAME";
            //  var recListISS = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qryISS, null, commandTimeout: 3000, commandType: CommandType.Text), conString);
            using (SqlDataReader reader = dv.GetREcordCount(qryISS))
            {
                while (reader.Read())
                {
                    bankName = reader["INSTITUTION_NAME"].ToString();
                    bankshort = reader["INSTITUTION_SHORTCODE"].ToString();
                    cbncode = reader["CBN_CODE"].ToString();

                    string FILENAME_PATH = Path.Combine(settlementpathdate, bankName);

                    if (!Directory.Exists(FILENAME_PATH))
                    {
                        Directory.CreateDirectory(FILENAME_PATH);
                    }

                    try
                    {


                        //output = rptIssuer.GenReport1(bankName, bankshort, SETT, reportPATH, FILENAME_PATH, logopath, "NAIRA SETTLEMENT", "DOM");
                   

                        //output = rptMDB.GenReport1(bankName, cbncode, SETT, reportPATH, FILENAME_PATH, logopath, "NAIRA SETTLEMENT", "DOM");
                     



                    }
                    catch (Exception ex)
                    {

                    }

                }
                reader.Dispose();
            }







            return null;

        }



        public string RPT_NIBSS(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        {
            //ALL RECONCILLATION AND NIBBS REPORT


            DateTime SETT;

            SETT = SETTDATE;

            string settlementpathdate = Path.Combine(reportPATH, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));


            if (!Directory.Exists(settlementpathdate))
            {
                Directory.CreateDirectory(settlementpathdate);
            }


            string output = string.Empty;
            string bankName = string.Empty;
            string bankshort = string.Empty;
            string cbncode = string.Empty;
            string cardScheme = string.Empty;


            string NIBSS = Path.Combine(settlementpathdate, "NIBSS");

            if (!Directory.Exists(NIBSS))
            {
                Directory.CreateDirectory(NIBSS);
            }


            string ERROR = Path.Combine(settlementpathdate, "ERROR LOG");

            if (!Directory.Exists(ERROR))
            {
                Directory.CreateDirectory(ERROR);
            }


            string RECON_FILE = Path.Combine(settlementpathdate, "RECONCILLIATION");

            if (!Directory.Exists(RECON_FILE))
            {
                Directory.CreateDirectory(RECON_FILE);
            }

         

            output = rptNIBSS_New.GenReport2b(bankName, bankshort, SETT, reportPATH, NIBSS, logopath);
            /////output = rptNIBSS_New.GenReport2c(bankName, bankshort, SETT, reportPATH, NIBSS, logopath);

          //output = rptNIBSS_New.GenReport2xml(SETT, reportPATH, NIBSS, logopath);


           output = rptNIBSS_New.GenReport3(bankName, bankshort, SETT, reportPATH, ERROR, logopath);


            output = rptNIBSS_New.GenReport5(bankName, bankshort, SETT, reportPATH, RECON_FILE, logopath);

            //IrptCollection _repoSETT = new rptCollection();
            //  output = _repoSETT.GenSettlementCollection(SETT, settlementpathdate, logopath, null);

            return null;

        }

        //public string RPT_GenMerchants(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        //{
        //    //MERCHANT REPORTS

        //    DateTime SETT;

        //    SETT = SETTDATE;

        //    string settlementpathdate = Path.Combine(reportPATH, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));


        //    if (!Directory.Exists(settlementpathdate))
        //    {
        //        Directory.CreateDirectory(settlementpathdate);
        //    }


        //    string output = string.Empty;
        //    string MERCHANTNAME = string.Empty;
        //    string ADDRESS = string.Empty;
        //    string SETTLEMENTACCOUNT = string.Empty;
        //    string BANKNAME = string.Empty;
        //    string MERCHANTID = string.Empty;
        //    string COLLECTION = string.Empty;
        //    string settlementcurrency = string.Empty;

        //    //MDB
        //    string qry = string.Format(@"select distinct a.MERCHANTID,a.MERCHANTNAME,ADDRESS,COLLECTION,SETTLEMENTACCOUNT,MERCHANTDEPOSITBANK BANKNAME,settlementcurrency
        //                    from posmisdb_merchantdetail a,posmisdb_settlementdetail b 
        //                    where a.merchantid=b.merchantid and settlementdate='{0}' and abs(b.NET_AMOUNTDUEMERCHANT)>0 
        //                    and a.merchantid not in (select merchantid from posmisdb_merchantdetail where collection in (1,2,3))
        //                    order by A.MERCHANTNAME", SETT.ToString("dd-MMM-yyyy"));
        //    //  var recListISS = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qryISS, null, commandTimeout: 3000, commandType: CommandType.Text), conString);
        //    using (OracleDataReader reader = GetREcordCount(qry))
        //    {

        //        while (reader.Read())
        //        {
        //            MERCHANTID = reader["MERCHANTID"].ToString();
        //            MERCHANTNAME = reader["MERCHANTNAME"].ToString();
        //            ADDRESS = reader["ADDRESS"].ToString();
        //            SETTLEMENTACCOUNT = reader["SETTLEMENTACCOUNT"].ToString();
        //            BANKNAME = reader["BANKNAME"].ToString();
        //            COLLECTION = reader["COLLECTION"].ToString();
        //            settlementcurrency = reader["settlementcurrency"].ToString();


        //            try
        //            {
        //                //string reportFor, DateTime reportDate, string reportPATH, DirectoryInfo outputdir


        //                string MERCHANTSPath = Path.Combine(settlementpathdate, "MERCHANTS");

        //                if (!Directory.Exists(MERCHANTSPath))
        //                {
        //                    Directory.CreateDirectory(MERCHANTSPath);
        //                }

        //                output = rptMerchants.GenReport2(MERCHANTID, MERCHANTNAME, ADDRESS, settlementcurrency, SETTLEMENTACCOUNT, BANKNAME, SETT, reportPATH, MERCHANTSPath, logopath, "NAIRA SETTLEMENT", "DOM");
        //                //output = rptMerchants.GenReport2(MERCHANTID, MERCHANTNAME, ADDRESS, settlementcurrency,SETTLEMENTACCOUNT, BANKNAME, SETT, reportPATH, MERCHANTSPath, logopath, "DOLLAR SETTLEMENT", "INT");

        //                try
        //                {
        //                    //GC.Collect();
        //                    //GC.WaitForPendingFinalizers();
        //                    //GC.Collect();
        //                    //GC.WaitForPendingFinalizers();
        //                }
        //                catch
        //                {

        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //            }


        //        }
        //        reader.Dispose();

        //    }







        //    return null;

        //}
        //public string RPT_GenARTEEMerchant(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        //{
        //    //MERCHANT REPORTS

        //    DateTime SETT;

        //    SETT = SETTDATE;

        //    string settlementpathdate = Path.Combine(reportPATH, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));


        //    if (!Directory.Exists(settlementpathdate))
        //    {
        //        Directory.CreateDirectory(settlementpathdate);
        //    }


        //    string output = string.Empty;
        //    string MERCHANTNAME = string.Empty;
        //    string ADDRESS = string.Empty;
        //    string SETTLEMENTACCOUNT = string.Empty;
        //    string BANKNAME = string.Empty;
        //    string MERCHANTID = string.Empty;
        //    string COLLECTION = string.Empty;
        //    string settlementcurrency = string.Empty;

        //    //MDB
        //    string qry = string.Format(@"select distinct a.MERCHANTID,a.MERCHANTNAME,ADDRESS,COLLECTION,SETTLEMENTACCOUNT,MERCHANTDEPOSITBANK BANKNAME,settlementcurrency
        //                    from posmisdb_merchantdetail a,posmisdb_settlementdetail b 
        //                    where a.merchantid=b.merchantid and settlementdate='{0}' and abs(b.NET_AMOUNTDUEMERCHANT)>0 and a.customerid=3166 AND ROWNUM=1
        //                    and a.merchantid not in (select merchantid from posmisdb_merchantdetail where collection in (1,2,3))
        //                    order by A.MERCHANTNAME", SETT.ToString("dd-MMM-yyyy"));
        //    //  var recListISS = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qryISS, null, commandTimeout: 3000, commandType: CommandType.Text), conString);
        //    using (OracleDataReader reader = GetREcordCount(qry))
        //    {

        //        while (reader.Read())
        //        {
        //            MERCHANTID = reader["MERCHANTID"].ToString();
        //            MERCHANTNAME = reader["MERCHANTNAME"].ToString();
        //            ADDRESS = reader["ADDRESS"].ToString();
        //            SETTLEMENTACCOUNT = reader["SETTLEMENTACCOUNT"].ToString();
        //            BANKNAME = reader["BANKNAME"].ToString();
        //            COLLECTION = reader["COLLECTION"].ToString();
        //            settlementcurrency = reader["settlementcurrency"].ToString();


        //            try
        //            {
        //                //string reportFor, DateTime reportDate, string reportPATH, DirectoryInfo outputdir


        //                string MERCHANTSPath = Path.Combine(settlementpathdate, "MERCHANTS");

        //                if (!Directory.Exists(MERCHANTSPath))
        //                {
        //                    Directory.CreateDirectory(MERCHANTSPath);
        //                }

        //                output = rptMerchants.GenReport3(MERCHANTID, "ARTEE INDUSTRIES", ADDRESS, settlementcurrency, SETTLEMENTACCOUNT, BANKNAME, SETT, reportPATH, MERCHANTSPath, logopath, "", "DOM");
        //                //output = rptMerchants.GenReport2(MERCHANTID, MERCHANTNAME, ADDRESS, settlementcurrency,SETTLEMENTACCOUNT, BANKNAME, SETT, reportPATH, MERCHANTSPath, logopath, "DOLLAR SETTLEMENT", "INT");

        //                try
        //                {
        //                    //GC.Collect();
        //                    //GC.WaitForPendingFinalizers();
        //                    //GC.Collect();
        //                    //GC.WaitForPendingFinalizers();
        //                }
        //                catch
        //                {

        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //            }


        //        }
        //        reader.Dispose();

        //    }







        //    return null;

        //}
        //public string RPT_GenVIPMerchant(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        //{
        //    //MERCHANT REPORTS

        //    DateTime SETT;

        //    SETT = SETTDATE;

        //    string settlementpathdate = Path.Combine(reportPATH, "VIP");


        //    if (!Directory.Exists(settlementpathdate))
        //    {
        //        Directory.CreateDirectory(settlementpathdate);
        //    }

        //    string qry = string.Format(@"SELECT * FROM POSMISDB_VIP_CUSTOMER");
        //    string output = string.Empty;
        //    string MERCHANTNAME = string.Empty;
        //    string ADDRESS = string.Empty;
        //    string SETTLEMENTACCOUNT = string.Empty;
        //    string BANKNAME = string.Empty;
        //    string MERCHANTID = string.Empty;
        //    string COLLECTION = string.Empty;
        //    string settlementcurrency = string.Empty;

        //    string custId = string.Empty;
        //    string custName = string.Empty;
        //    LogFunction2.WriteMaintenaceLogToFile("getting vip customer :{0}");
        //    using (OracleDataReader reader1 = GetREcordCount(qry))
        //    {
        //        LogFunction2.WriteMaintenaceLogToFile("reader to get vip customers was successful :{0}");
        //        if (reader1.HasRows)
        //        {
        //            while (reader1.Read())
        //            {
        //                LogFunction2.WriteMaintenaceLogToFile("looping in the fetch record :{0}");
        //                custId = reader1["CUSTOMERID"].ToString();
        //                custName = reader1["CUSTOMERNAME"].ToString();
        //                //ADDRESS = reader1["ADDRESS"].ToString();
        //                //SETTLEMENTACCOUNT = reader1["SETTLEMENTACCOUNT"].ToString();
        //                //BANKNAME = reader1["BANKNAME"].ToString();
        //                //COLLECTION = reader1["COLLECTION"].ToString();
        //                //settlementcurrency = reader1["settlementcurrency"].ToString();

        //                //MDB
        //                qry = string.Format(@"select distinct a.MERCHANTID,a.MERCHANTNAME,ADDRESS,COLLECTION,SETTLEMENTACCOUNT,MERCHANTDEPOSITBANK BANKNAME,settlementcurrency
        //                    from posmisdb_merchantdetail a,posmisdb_settlementdetail b 
        //                    where a.merchantid=b.merchantid and settlementdate='{0}' and abs(b.NET_AMOUNTDUEMERCHANT)>0 and a.customerid={1} AND ROWNUM=1
        //                    and a.merchantid not in (select merchantid from posmisdb_merchantdetail where collection in (1,2,3))
        //                    order by A.MERCHANTNAME", SETT.ToString("dd-MMM-yyyy"), custId);
        //                //  var recListISS = Fetch(c => c.Query<POSMISDB_INSTITUTION>(qryISS, null, commandTimeout: 3000, commandType: CommandType.Text), conString);
        //                LogFunction2.WriteMaintenaceLogToFile("getting vip settlement record :{0}");
        //                using (OracleDataReader reader = GetREcordCount(qry))
        //                {

        //                    while (reader.Read())
        //                    {
        //                        LogFunction2.WriteMaintenaceLogToFile("getting to generate report for customer :{0}");
        //                        MERCHANTID = reader["MERCHANTID"].ToString();
        //                        MERCHANTNAME = reader["MERCHANTNAME"].ToString();
        //                        ADDRESS = reader["ADDRESS"].ToString();
        //                        SETTLEMENTACCOUNT = reader["SETTLEMENTACCOUNT"].ToString();
        //                        BANKNAME = reader["BANKNAME"].ToString();
        //                        COLLECTION = reader["COLLECTION"].ToString();
        //                        settlementcurrency = reader["settlementcurrency"].ToString();


        //                        try
        //                        {
        //                            //string reportFor, DateTime reportDate, string reportPATH, DirectoryInfo outputdir


        //                            //string MERCHANTSPath = Path.Combine(settlementpathdate, "MERCHANTS");

        //                            //if (!Directory.Exists(MERCHANTSPath))
        //                            //{
        //                            //    Directory.CreateDirectory(MERCHANTSPath);
        //                            //}

        //                            output = rptMerchants.GenReportvip(custId, custName.Trim(), ADDRESS, settlementcurrency, SETTLEMENTACCOUNT, BANKNAME, SETT, reportPATH, settlementpathdate, logopath, "", "DOM");
        //                            //output = rptMerchants.GenReport2(MERCHANTID, MERCHANTNAME, ADDRESS, settlementcurrency,SETTLEMENTACCOUNT, BANKNAME, SETT, reportPATH, MERCHANTSPath, logopath, "DOLLAR SETTLEMENT", "INT");

        //                            try
        //                            {
        //                                //GC.Collect();
        //                                //GC.WaitForPendingFinalizers();
        //                                //GC.Collect();
        //                                //GC.WaitForPendingFinalizers();
        //                            }
        //                            catch
        //                            {

        //                            }

        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }


        //                    }
        //                    reader.Dispose();

        //                }



        //            }
        //        }
        //        else
        //        {
        //            LogFunction2.WriteMaintenaceLogToFile("No record found for VIP merchant :{0}");
        //        }
        //    }



        //    return null;

        //}
        //private OracleDataReader GetREcordCount(string Query)
        //{
        //    OracleDataReader dr = null;

        //    dr = default(OracleDataReader);
        //    OracleConnection Standby_connection = new OracleConnection(oradb);
        //    OracleCommand cmd = new OracleCommand();

        //    try
        //    {
        //        cmd.Connection = Standby_connection;
        //        cmd.CommandText = Query;

        //        cmd.CommandType = CommandType.Text;
        //        cmd.BindByName = true;
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        dr = cmd.ExecuteReader();

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    try
        //    {

        //        return dr;

        //    }

        //    catch (Exception ex)

        //    {

        //    }
        //    return null;
        //}
        //public string RPT_Settlement(DateTime SETTDATE, string reportPATH, string logopath, string conString)
        //{
        //    //ONLY NIBSS REPORT
        //    DateTime SETT;

        //    SETT = SETTDATE;

        //    string settlementpathdate = Path.Combine(reportPATH, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));

        //    string PTSPPath = Path.Combine(settlementpathdate, "PTSP");

        //    if (!Directory.Exists(PTSPPath))
        //    {
        //        Directory.CreateDirectory(PTSPPath);
        //    }

        //    string PTSAPath = Path.Combine(settlementpathdate, "PTSA");

        //    if (!Directory.Exists(PTSAPath))
        //    {
        //        Directory.CreateDirectory(PTSAPath);
        //    }

        //    string TERWPath = Path.Combine(settlementpathdate, "TERMINALOWNER");

        //    if (!Directory.Exists(TERWPath))
        //    {
        //        Directory.CreateDirectory(TERWPath);
        //    }

        //    string SWTCHPath = Path.Combine(settlementpathdate, "SWITCH");

        //    if (!Directory.Exists(SWTCHPath))
        //    {
        //        Directory.CreateDirectory(SWTCHPath);
        //    }


        //    ////if (!Directory.Exists(settlementpathdate))
        //    ////{
        //    ////    Directory.CreateDirectory(settlementpathdate);
        //    ////}
        //    ////string sett = Path.Combine(settlementpathdate, "SETTLEMENT");

        //    ////if (!Directory.Exists(sett))
        //    ////{
        //    ////    Directory.CreateDirectory(sett);
        //    ////}

        //    string output = string.Empty;
        //    string bankName = string.Empty;
        //    string bankshort = string.Empty;
        //    string cbncode = string.Empty;
        //    string cardScheme = string.Empty;

        //    //Settlement Detail
        //    //output = rptNIBSS_New.GenReport4(null, null, SETT, reportPATH, sett, logopath);


        //    //TERW
        //    string qry5 = "Select PARTY_DESC,nvl(PARTY_REFID,PARTY_SHORTNAME) PARTY_REFID from POSMISDB_PARTY WHERE lower(status)='active' AND PARTYTYPE_CODE='TERW' order by PARTY_DESC";
        //   // var recList5 = Fetch(c => c.Query<POSMISDB_PARTY>(qry5, null, commandType: CommandType.Text), conString);
        //    ////if (recList5.Count() > 0)
        //    ////{
        //    ////    foreach (var t in recList5)
        //    ////    {

        //            using (OracleDataReader reader = GetREcordCount(qry5))
        //            {

        //                while (reader.Read())
        //                {
        //                    bankName = reader["PARTY_DESC"].ToString();

        //                    bankshort = reader["PARTY_REFID"].ToString().Trim();

        //                    try
        //                    {

        //                        output = rptTermOwner.GenReport1(bankName, bankshort, SETT, reportPATH, TERWPath, logopath, "NAIRA SETTLEMENT", "DOM");
        //                        // output = rptTermOwner.GenReport1(bankName, bankshort, SETT, reportPATH, TERWPath, logopath, "DOLLAR SETTLEMENT", "INT");




        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                  }

        //                 }





        //    //PTSP

        //    ////string qry2 = "Select DISTINCT PARTY_DESC,PARTY_SHORTNAME from POSMISDB_PARTY A,POSMISDB_SETTLEMENTDETAIL B WHERE lower(A.status)='active' AND B.SETTLEMENTDATE='" + SETT.ToString("dd-MMM-yyyy") + "' AND PARTYTYPE_CODE='PTSP' AND A.PARTY_SHORTNAME=B.PTSP_CODE order by PARTY_DESC";
        //    string qry2 = "Select DISTINCT PARTY_DESC,PARTY_SHORTNAME from POSMISDB_PARTY WHERE PARTYTYPE_CODE='PTSP' and PARTY_SHORTNAME is not null";

        //    using (OracleDataReader reader = GetREcordCount(qry2))
        //    {

        //        while (reader.Read())
        //        {
        //            bankName = reader["PARTY_DESC"].ToString();

        //            bankshort = reader["PARTY_SHORTNAME"].ToString();

        //            try
        //            {

        //                output = rptPTSP.GenReport1(bankName, bankshort, SETT, reportPATH, PTSPPath, logopath, "NAIRA SETTLEMENT", "DOM");
        //                // output = rptTermOwner.GenReport1(bankName, bankshort, SETT, reportPATH, TERWPath, logopath, "DOLLAR SETTLEMENT", "INT");




        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }

        //    }



        //    //PTSA

        //    string qry3 = "Select * from POSMISDB_PARTY WHERE lower(status)='active' AND PARTYTYPE_CODE='PTSA' order by PARTY_DESC";

        //    using (OracleDataReader reader = GetREcordCount(qry3))
        //    {

        //        while (reader.Read())
        //        {
        //            bankName = reader["PARTY_DESC"].ToString();

        //            bankshort = reader["PARTY_SHORTNAME"].ToString();

        //            try
        //            {

        //                output = rptPTSA.GenReport1(bankName, bankshort, SETT, reportPATH, PTSAPath, logopath, "NAIRA SETTLEMENT", "DOM");
        //                // output = rptTermOwner.GenReport1(bankName, bankshort, SETT, reportPATH, TERWPath, logopath, "DOLLAR SETTLEMENT", "INT");




        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }

        //    }





        //    //SWTH

        //    string qry4 = "Select * from POSMISDB_PARTY WHERE lower(status)='active' AND PARTYTYPE_CODE='SWTH' order by PARTY_DESC";
        //    using (OracleDataReader reader = GetREcordCount(qry4))
        //    {

        //        while (reader.Read())
        //        {
        //            bankName = reader["PARTY_DESC"].ToString();

        //            bankshort = reader["PARTY_SHORTNAME"].ToString();

        //            try
        //            {

        //                output = rptSwitch.GenReport1(bankName, bankshort, SETT, reportPATH, SWTCHPath, logopath, "NAIRA SETTLEMENT", "DOM");
        //                // output = rptTermOwner.GenReport1(bankName, bankshort, SETT, reportPATH, TERWPath, logopath, "DOLLAR SETTLEMENT", "INT");




        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }

        //    }








        //    return null;



        //}



    }


 
}
