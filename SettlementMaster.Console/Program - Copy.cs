using Dapper;
using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Dapper.PostConnect;
using Generic.Dapper.ReportClass;
using Generic.Dapper.Repository;
using Generic.Dapper.Utilities;
using Generic.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SettlementMaster.Cons
{
    class Program
    {

        static void Main(string[] args)

        {
            //           
            //string gh = "user id=DPPADMIN; password=DPPADMIN; Connection Timeout=600; Max Pool Size=150; data source= (DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 172.19.4.90)(PORT = 1700)) (CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = TWODB))))";
            //string conString = "Data Source=172.19.4.103:1700/TWODB;User Id=DPPADMIN;password=DPPADMIN;";
            //string twoConString = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TwoConfig"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["TwoConfig"]) : string.Empty;
            //string MEBSource = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MEBSource"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["MEBSource"]) : string.Empty;
            //string MEBTarget = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MEBTarget"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["MEBTarget"]) : string.Empty;
            string reportPath = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) : string.Empty;
            string logopath = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;
            IDapperGeneralSettings _repo = new DapperGeneralSettings();

            // string conString = twoConString;
            // var dd =  new SettlementProcess().GetMSC_PERC("1654487896", "1199803451", "2044QF28", "2044OG00V000183", "VISA", "566", "8220", 20916, "566", "0", "0",0,0,0,0, "0");

            // int rec1 = _repo.PostMEB(MEBSource, MEBTarget, reportPath, logopath, "9622331", null);


            // DateTime? v_SETTDATE = null;

            //new SettlementProcess().SETTProcess("9622331");

            //(SETTDATE, reportPATH, lgopath,  NULL);
            IMainReport _repoSETT = new MainReport();
            ////string sett = _repoSETT.RPT_GenSettlementFile(DateTime.Parse("02-FEB-17"), reportPath, logopath, null);

            //var g = new EmailProcess();
            // g.SendMerchantarteeSettlementEmail("26-May-2017", "");
            // IMainReport _repoSETT = new MainReport();
            // g.SendMerchantVIPSettlementEmail("29-JUN-2017", "");
            //    string sett2 = _repoSETT.RPT_GenVIPMerchant(new DateTime(2017,09,06), reportPath, logopath, null);

            //new SettlementProcess_MEB().SETTProcess("9622331", "08-FEB-17");


            //string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
            //new MEBCollectionClass().MEBProcess(dtFrom2);

            ////v_SETTDATE = DateTime.Today;
            ////v_SETTDATE.Value.Date.ToString("dd-MMM-yy");
            //IDapperSettlementReport _repoSETT = new DapperSettlementReport();
            // bool sett = _repoSETT.GenSettlementFileSETT(DateTime.Parse(v_SETTDATE.Value.Date.ToString("dd-MMM-yy")), null, reportPath, logopath, null);
            // bool sett = _repoSETT.GenSettlementFileSETT(DateTime.Parse("16-JAN-17"), null, reportPath, logopath, null);


            //   int rec2 = _repo.processSettlement(MEBTarget, reportPath, logopath, null);

            //P.SendSettlementInstitutionNotification("12-MAY-2017", "");

            //var cbn_code = "011";
            //var prefixM = string.Concat("2", cbn_code, "LA"); ;
            //var lastMidGen = "";
            //var recM = _repo.GetLastMidTidGenerated(prefixM, cbn_code, "M");
            //if(recM != null)
            //{
            //    lastMidGen = recM.MERCHANTID;
            //}
            //var mid = MidTidGenerator.GenMid(prefixM, "2011LAAAAAAAAA9");
            //Console.WriteLine("Start processing..........");
            //LogFunction lgfn = new LogFunction();
            //lgfn.loginfo("", null, DateTime.Now.ToString());
            ////var D = new EmailProcess();
            ////D.SendMerchantarteeSettlementEmail("24-MAY-2017", "");
            //var output = @"C:\POS_AUTOMATION\SETTLEMENT_DETAIL_SETT_28-Jul-2017\ACCESS BANK NIGERIA PLC\ACQUIRER\NAIRA SETTLEMENT\ACCESS BANK NIGERIA PLC_ACQUIRER.xlsx";
            //var ext = Path.GetExtension(output);
            //var fileNameNew = string.Concat(Path.GetFileNameWithoutExtension(output), "_", DateTime.Now.ToString("dd-MM-yyyy"), Path.GetExtension(output));
            //var dest = @"C:\FTP_Path\UBA";
            //if (File.Exists(output) && Directory.Exists(dest))
            //{
            //    LogFunction2.WriteMaintenaceLogToFile("Directory exist for both source and destination path");
            //    // copy file to ftp path
            //    File.Copy(output, Path.Combine(dest, fileNameNew));
            //    LogFunction2.WriteMaintenaceLogToFile("File Copied Successfully");
            //}
            Console.WriteLine("enter a report type..........");
            Console.WriteLine("1.  SETTLEMENT MASTER REPORTS");
            Console.WriteLine("2.  NIBSS SETTLEMENT REPORTS");
            Console.WriteLine("3.  ACQUIRERE/ISSUER/MDB REPORTS");
            Console.WriteLine("4.  MERCHANT REPORTS");
            Console.WriteLine("5.  PTSP/PTSA/TERW/SWTH REPORTS");
            Console.WriteLine("6.  ARTEE MERCHANT REPORTS");
            Console.WriteLine("7.  VIP MERCHANT REPORT");
            Console.WriteLine("8.  COLLECTION REPORT");
            Console.WriteLine("9.  UPHSS REPORT");
            Console.WriteLine("10. UPHSS REPORT CONSOLIDATED");
            string opt = Console.ReadLine();
            //GenHSSReportConsolidated
            Console.WriteLine("Select Date type..........");
            Console.WriteLine("1.  By Single Day");
            Console.WriteLine("2.  By Date Range");
            string optdatetype = Console.ReadLine();
            int noofdays = 0;
            string dtFrom = "";
            if (optdatetype == "2")
            {
                Console.WriteLine("enter a report Start date..........");
                string fromdt = Console.ReadLine();
                Console.WriteLine("enter a report End date..........");
                string todt = Console.ReadLine();
                dtFrom = fromdt;
                noofdays = (Convert.ToDateTime(todt) - Convert.ToDateTime(fromdt)).Days;
                Console.WriteLine("Starting From" + dtFrom);
                for (int i = 0; i < noofdays; i++)
                {


                    DateTime dtFromDate = Convert.ToDateTime(dtFrom);


                    ////IDapperSettlementReport _repoSETT = new DapperSettlementReport();
                    ////bool sett = _repoSETT.GenSettlementFileSETT_EEPLUS(dtFromDate, null, reportPath, logopath, null);
                    //IMainReport _repoSETT = new MainReport();
                    // _repoSETT.TestExport();


                    if (opt == "1")
                    {
                        Console.WriteLine("processing........SETTLEMENT MASTER REPORT" + dtFrom);
                        string sett = _repoSETT.RPT_GenSettlementMASTER(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    }
                    if (opt == "2")
                    {
                        Console.WriteLine("processing.........NIBSS Settlement Report" + dtFrom);
                        //string sett = _repoSETT.RPT_GenSettlementFile2(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    }
                    if (opt == "3")
                    {
                        Console.WriteLine("processing.........ACQUIRER/ISSUER/MDB REPORT" + dtFrom);
                        //string sett = _repoSETT.RPT_GenSettlementFile(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    }
                    if (opt == "4")
                    {
                        Console.WriteLine("processing..........Merchant Reports" + dtFrom);
                        //string sett = _repoSETT.RPT_GenMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    }

                    if (opt == "6")
                    {
                        Console.WriteLine("processing..........ARTEE Merchant Reports" + dtFrom);
                        //string sett = _repoSETT.RPT_GenARTEEMerchant(DateTime.Parse(dtFrom), reportPath, logopath, null);
                        //var P = new EmailProcess();
                        //P.SendMerchantarteeSettlementEmail(dtFrom, "");

                    }
                    if (opt == "7")
                    {
                        Console.WriteLine("processing..........VIP Merchant Reports" + dtFrom);
                       // string sett = _repoSETT.RPT_GenVIPMerchant(DateTime.Parse(dtFrom), reportPath, logopath, null);
                       // var P = new EmailProcess();
                        //P.SendMerchantVIPSettlementEmail(dtFrom, "");
                    }
                    if (opt == "8")
                    {
                        Console.WriteLine("processing..........Collection Merchant Reports" + dtFrom);
                       // string sett = _repoSETT.RPT_CollectionGenMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                        // var P = new EmailProcess();
                        //  P.SendMerchantVIPSettlementEmail(dtFrom, "");
                    }

                    ////if (opt == "5")
                    ////{
                    ////    Console.WriteLine("processing..........Merchant Report By Merchant Deposit Bank" + dtFrom);

                    ////    string sett = _repoSETT.RPT_GenMDBMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    ////}

                    if (opt == "5")
                    {
                        Console.WriteLine("processing..........Party Report (PTSP/PTSA/TERW/SWTH)" + dtFrom);

                        //string sett = _repoSETT.RPT_Settlement(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    }
                    Console.WriteLine("processing Completed For " + dtFrom);
                    dtFrom = Convert.ToDateTime(dtFrom).AddDays(1).ToString();

                }
                Console.WriteLine("processing Completed");

            }
            else
            {
                Console.WriteLine("enter a report processing date..........");
                dtFrom = Console.ReadLine();
                Console.WriteLine(dtFrom);

                //DateTime dtFromDate =DateTime.Parse(dtFrom);
                //DateTime dtFromDate = DateTime.ParseExact(dtFrom, "dd/MM/yyyy", null);

                //Console.WriteLine("enter a report type..........");
                //Console.WriteLine("1.  SETTLEMENT MASTER REPORTS");
                //Console.WriteLine("2.  NIBSS SETTLEMENT REPORTS");
                //Console.WriteLine("3.  ACQUIRERE/ISSUER/MDB REPORTS");
                //Console.WriteLine("4.  MERCHANT REPORTS");
                //Console.WriteLine("5.  PTSP/PTSA/TERW/SWTH REPORTS");
                //Console.WriteLine("6.  ARTEE MERCHANT REPORTS");
                //Console.WriteLine("7.  VIP MERCHANT REPORT");
                //Console.WriteLine("8.  COLLECTION REPORT");

                //   opt = Console.ReadLine();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB"); //dd/MM/yyyy

                 

               

                if (opt == "1")
                {
                    Console.WriteLine("processing........SETTLEMENT MASTER REPORT" + dtFrom);
                    string sett = _repoSETT.RPT_GenSettlementMASTER(DateTime.Parse(dtFrom), reportPath, logopath, null);
                }
                if (opt == "2")
                {
                    //Console.WriteLine("processing.........NIBSS Settlement Report" + dtFrom);
                    string sett = _repoSETT.RPT_NIBSS(DateTime.Parse(dtFrom), reportPath, logopath, null);
                }
                if (opt == "3")
                {
                    Console.WriteLine("processing.........ACQUIRERE/ISSUER/MDB REPORT" + dtFrom);
                   string sett = _repoSETT.RPT_ACQUIREREISSMDB(DateTime.Parse(dtFrom), reportPath, logopath, null);
                }
                if (opt == "4")
                {
                    Console.WriteLine("processing..........Merchant Reports" + dtFrom);
                   // string sett = _repoSETT.RPT_GenMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                }

                if (opt == "6")
                {
                    Console.WriteLine("processing..........ARTEE Merchant Reports" + dtFrom);
                    //string sett = _repoSETT.RPT_GenARTEEMerchant(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    //var P = new EmailProcess();
                    //P.SendMerchantarteeSettlementEmail(dtFrom, "");

                }
                if (opt == "7")
                {
                    Console.WriteLine("processing..........VIP Merchant Reports" + dtFrom);
                    //string sett = _repoSETT.RPT_GenVIPMerchant(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    //var P = new EmailProcess();
                    //P.SendMerchantVIPSettlementEmail(dtFrom, "");
                }
                if (opt == "8")
                {
                    Console.WriteLine("processing..........Collection Merchant Reports" + dtFrom);
                    //string sett = _repoSETT.RPT_CollectionGenMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                    // var P = new EmailProcess();
                    //  P.SendMerchantVIPSettlementEmail(dtFrom, "");
                }

                ////if (opt == "5")
                ////{
                ////    Console.WriteLine("processing..........Merchant Report By Merchant Deposit Bank" + dtFrom);

                ////    string sett = _repoSETT.RPT_GenMDBMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                ////}

                if (opt == "5")
                {
                    Console.WriteLine("processing..........Party Report (PTSP/PTSA/TERW/SWTH)" + dtFrom);

                    //string sett = _repoSETT.RPT_Settlement(DateTime.Parse(dtFrom), reportPath, logopath, null);
                }

                if (opt == "9")
                {
                    //string[] sessions = { "2018021916" };
                    Console.WriteLine("Enter Session ID: ");
                    string sessionID = Console.ReadLine();
                    //foreach (string sessionID in sessions)
                    //{
                    Console.WriteLine("processing..........UPHSS REPORT " + dtFrom + " SESSION: " + sessionID);

                    //string sett = _repoSETT.RPT_GenHSSReport(DateTime.Parse(dtFrom), reportPath, logopath, null, sessionID);

                    // }
                }

                if (opt == "10")
                {
                    //string[] sessions = { "2018021916" };
                    //  Console.WriteLine("Enter Session ID: ");
                    // string sessionID = Console.ReadLine();
                    //foreach (string sessionID in sessions)
                    //{
                    Console.WriteLine("processing..........UPHSS REPORT cosolidated ");

                    //string sett = _repoSETT.GenHSSReportConsolidated(DateTime.Parse(dtFrom), reportPath, logopath, null);

                    // }
                }



                Console.WriteLine("processing Completed");
            }



        }



    }
}
////static void Main(string[] args)
////{
////    //var set = new SettlementProcess_MEB();
////    //var crD = new DateTime(2018,08,05);
////    //var trD = new DateTime(2018, 06, 05);
////    //using (var con = new RepoBase().OpenConnection("Data Source=.;Initial Catalog=SettlementMaster;User Id=sa;password=(oneGod)"))
////    //{

////    //    set.LogErrorMessage(12, "12345", "1234", DateTime.Now, "Test", crD, trD, "abc123",23456, con);
////    //}
////    //var df = DateTime.Compare(DateTime.Now.Date, new DateTime(2018,01,02));




////    //connect.TranxEnquiry();
////    //var obj = new requestPaymentObj()
////    //{
////    //    Beneficiary = "Usman Yusuf Olawale",
////    //    AccountNumber = "0015204612",
////    //    Amount = "30000",
////    //    Narration = "OKOK",
////    //    SortCode = "033"
////    //};
////    //var obj2 = new requestPaymentObj()
////    //{
////    //    Beneficiary = "Usman Yusuf Olawale",
////    //    AccountNumber = "0015204612",
////    //    Amount = "30000",
////    //    Narration = "OKOK",
////    //    SortCode = "033"
////    //};
////    //var gh = new jsonAuthKey()
////    //{
////    //    AppUser = "Settlementapp1",
////    //    Password = "8093847f2ac3ecab91b881ca7fb07d0f2948713e39037fe74055d0bd66a82f36732395a4efff30481c999b064797dc943601c07337aa0db279767dc90825fe49",
////    //    FileName = "FilenameUploaded",
////    //    ScheduleId = "488449399393",
////    //    DebitSortCode = "050",
////    //    DebitAccountNumber = "0123456789"
////    //};
////    //var dList = new List<requestPaymentObj>();
////    //dList.Add(obj);
////    //dList.Add(obj2);
////    //var jsonPay = JsonConvert.SerializeObject(dList);
////    //var resp = @"{""?xml"":{""@version"":""1.0"",""@encoding"":""UTF - 8""},""PaymentResponse"":{""Header"":{""ScheduleId"":""48844938989879086789078798888799388"",""ClientId"":""NIBSS_V2001"",""DebitSortCode"":""050"",""DebitAccountNumber"":""0123456789"",""Status"":""16""},""HashValue"":""""}}";
////    //var objreq = JsonConvert.DeserializeObject<PayResponse>(resp);

////    //var jsonAuth = JsonConvert.SerializeObject(gh);
////    //var objreq2 = JsonConvert.DeserializeObject<jsonAuthKey>(jsonAuth);
////    //Console.ReadLine();
////    //PostNaps();
////    //AspNetUser rec = new AspNetUser()
////    //{
////    //    DeptCode = "SFT",
////    //    UserName = "olawale.usman",
////    //    DeptName = "",
////    //    FirstName = "Yusuf",
////    //    LastName = "Usman",
////    //    CreateDate = DateTime.Now,
////    //    Email = "olawaleusman01@gmail.com",

////    //};
////    //AuditHelper.PostAudit(null, rec, "I", "test", "test2", DateTime.Now,"key", "AspNetUsers", 1);

////    //
////    //var d = new DateTime(2018, 01, 2);
////    //var rpt = new Naps();
////    //var rst = rpt.GenerateNaps(d, "1234", "A", "12345");
////    //var d = new DateTime(2017,10, 4);
////    //var currentDay = d.Day;
////    //var daysInMonth = DateTime.DaysInMonth(d.Year, d.Month);
////    //var SETTLEMENTDATE = d.AddDays((daysInMonth - currentDay));
////    //var daysAdded = 0;
////    //for (int i = 1; i <= daysInMonth; i++)
////    //{
////    //    //if(WeekendList SETTLEMENTDATE.DayOfWeek)
////    //    if (daysAdded == 2)
////    //    {
////    //        break;
////    //    }
////    //    var hj = GetWeekendList();
////    //    SETTLEMENTDATE = SETTLEMENTDATE.AddDays(1);
////    //    if (hj.Contains(SETTLEMENTDATE.DayOfWeek))
////    //    {
////    //        continue;
////    //    }
////    //    daysAdded++;
////    //    //sett
////    //}
////    //13 - 19
////    //var d = new DateTime(2017, 11, 19);
////    //var currentDay = (int)d.DayOfWeek;
////    //int daysAdded = 0;
////    //if (currentDay != 0)
////    //{
////    //     daysAdded = (7 - currentDay) + 1;
////    //}
////    //else
////    //{
////    //     daysAdded =  1;
////    //}
////    //var hh = d.AddDays(daysAdded);
////    //var curDay = hh.DayOfWeek;
////    /*---DOWNLOAD REPORT*/
////    // var dtMain = rptSett.generateDS(null,"","ALL", "U", null, "2018-01-02", null, null);
////    // var excelBytes = ExcelHelper.ExportDataSet(dtMain);
////    /*---REPORT GENERATION TEST*/
////    //string reportPath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) : string.Empty;
////    //string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;
////    //IMainReport _repoSETT = new MainReport();
////    //var setDate = new DateTime(2017, 11, 20);
////    //string sett = _repoSETT.RPT_GenSettlementMASTER(setDate, reportPath, logopath, null);
////    /*---END OF REPORT GENERATION---*/

////    //////try
////    //////{
////    //////    var connect = new NapsConnect();
////    //////    connect.PostNapsXPEApi();
////    //////}
////    //////catch (Exception ex)
////    //////{
////    //////    var test = ex.Message;
////    //////    throw;
////    //////}

////    string destDb = ConfigurationManager.AppSettings["DEST_DB"].ToString();

////    //using (var con = new RepoBase().OpenConnection(destDb))
////    //{
////    //    var opdate = DateTime.Now;
////    //    var p1 = new DynamicParameters();
////    //    p1.Add("@P_DATE", opdate, DbType.Date);
////    //    var sql = "proc_set_PostToSettlementDetail";
////    //    var rec = con.Execute(sql, p1,commandTimeout:0, commandType: CommandType.StoredProcedure);
////    //}
////    LogFunction lgfn = new LogFunction();
////    MEBCollectionClass TLAStarter = new MEBCollectionClass();
////    SettlementProcess_MEB TLAStarter2 = new SettlementProcess_MEB();
////    //    string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
////    //    //If Scheduled Time is passed set Schedule for the next day.
////    lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Timer Initializing", null, null);


////    try
////    {
////        //TLAStarter.MEBProcess();
////        var random = new Random((int)DateTime.Now.Ticks);
////        var randomValue = random.Next(1000000, 9999999);
////        string batchno = randomValue.ToString();
////       // TLAStarter.ProcessSettlement(batchno);
////        TLAStarter2.SettProcess(batchno,"2018-09-12");
////    }

////    catch (Exception ex)
////    {
////        lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);


////    }

////    //try
////    //{
////    //    SettlementProcess_MEB gg = new SettlementProcess_MEB();
////    //    gg.SettProcess("124", "2017-09-29");
////    //}
////    //catch (Exception ex)
////    //{

////    //}
////}

////static void PostNaps()
////        {
////            var destUri = "http://localhost:15443/Service1.asmx";
////            var data = "";
////            var rst = "";
////            using (WebClient postClient = new WebClient())
////            {
////                postClient.Proxy = null;
////                postClient.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
////               // postClient.Headers.Add("SOAPAction", "http://tempuri.org");
////                StreamReader reader = new StreamReader("C:\\Application\\v2018\\SettlementMaster\\SettlementMaster.App\\NapTemplate\\TranxPosting.xml");

////                data = reader.ReadToEnd();
////                reader.Close();

////                //  postClient.UploadStringCompleted += new UploadStringCompletedEventHandler(postClient_UploadStringCompleted);

////                rst = postClient.UploadString(new Uri(destUri), data);

////                //ServicePointManager.Expect100Continue = true;
////                //.ServerCertificateValidationCallback = MyCertHandler;
////            }
////            //postClient_UploadStringCompleted(rst);
////        }
////        static List<DayOfWeek> GetWeekendList()
////        {
////            var lst = new List<DayOfWeek>();
////            lst.Add(DayOfWeek.Saturday);
////            lst.Add(DayOfWeek.Sunday);
////            return lst;
////        }
////    }
////}
