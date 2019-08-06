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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SettlementMaster.Cons
{
    class Program
    {
        static void Main(string[] args)
        {
            //var set = new SettlementProcess_MEB();
            //var crD = new DateTime(2018,08,05);
            //var trD = new DateTime(2018, 06, 05);
            //using (var con = new RepoBase().OpenConnection("Data Source=.;Initial Catalog=SettlementMaster;User Id=sa;password=(oneGod)"))
            //{

            // set.LogErrorMessage(12, "12345", "1234", DateTime.Now, "Test", crD, trD, "abc123",23456, con);
            //}
            //var df = DateTime.Compare(DateTime.Now.Date, new DateTime(2018,01,02));




            //connect.TranxEnquiry();
            //var obj = new requestPaymentObj()
            //{
            // Beneficiary = "Usman Yusuf Olawale",
            // AccountNumber = "0015204612",
            // Amount = "30000",
            // Narration = "OKOK",
            // SortCode = "033"
            //};
            //var obj2 = new requestPaymentObj()
            //{
            // Beneficiary = "Usman Yusuf Olawale",
            // AccountNumber = "0015204612",
            // Amount = "30000",
            // Narration = "OKOK",
            // SortCode = "033"
            //};
            //var gh = new jsonAuthKey()
            //{
            // AppUser = "Settlementapp1",
            // Password = "8093847f2ac3ecab91b881ca7fb07d0f2948713e39037fe74055d0bd66a82f36732395a4efff30481c999b064797dc943601c07337aa0db279767dc90825fe49",
            // FileName = "FilenameUploaded",
            // ScheduleId = "488449399393",
            // DebitSortCode = "050",
            // DebitAccountNumber = "0123456789"
            //};
            //var dList = new List<requestPaymentObj>();
            //dList.Add(obj);
            //dList.Add(obj2);
            //var jsonPay = JsonConvert.SerializeObject(dList);
            //var resp = @"{""?xml"":{""@version"":""1.0"",""@encoding"":""UTF - 8""},""PaymentResponse"":{""Header"":{""ScheduleId"":""48844938989879086789078798888799388"",""ClientId"":""NIBSS_V2001"",""DebitSortCode"":""050"",""DebitAccountNumber"":""0123456789"",""Status"":""16""},""HashValue"":""""}}";
            //var objreq = JsonConvert.DeserializeObject<PayResponse>(resp);

            //var jsonAuth = JsonConvert.SerializeObject(gh);
            //var objreq2 = JsonConvert.DeserializeObject<jsonAuthKey>(jsonAuth);
            //Console.ReadLine();
            //PostNaps();
            //AspNetUser rec = new AspNetUser()
            //{
            // DeptCode = "SFT",
            // UserName = "olawale.usman",
            // DeptName = "",
            // FirstName = "Yusuf",
            // LastName = "Usman",
            // CreateDate = DateTime.Now,
            // Email = "olawaleusman01@gmail.com",

            //};
            //AuditHelper.PostAudit(null, rec, "I", "test", "test2", DateTime.Now,"key", "AspNetUsers", 1);

            //
            //var d = new DateTime(2018, 01, 2);
            //var rpt = new Naps();
            //var rst = rpt.GenerateNaps(d, "1234", "A", "12345");
            //var d = new DateTime(2017,10, 4);
            //var currentDay = d.Day;
            //var daysInMonth = DateTime.DaysInMonth(d.Year, d.Month);
            //var SETTLEMENTDATE = d.AddDays((daysInMonth - currentDay));
            //var daysAdded = 0;
            //for (int i = 1; i <= daysInMonth; i++)
            //{
            // //if(WeekendList SETTLEMENTDATE.DayOfWeek)
            // if (daysAdded == 2)
            // {
            // break;
            // }
            // var hj = GetWeekendList();
            // SETTLEMENTDATE = SETTLEMENTDATE.AddDays(1);
            // if (hj.Contains(SETTLEMENTDATE.DayOfWeek))
            // {
            // continue;
            // }
            // daysAdded++;
            // //sett
            //}
            //13 - 19
            //var d = new DateTime(2017, 11, 19);
            //var currentDay = (int)d.DayOfWeek;
            //int daysAdded = 0;
            //if (currentDay != 0)
            //{
            // daysAdded = (7 - currentDay) + 1;
            //}
            //else
            //{
            // daysAdded = 1;
            //}
            //var hh = d.AddDays(daysAdded);
            //var curDay = hh.DayOfWeek;
            /*---DOWNLOAD REPORT*/
            // var dtMain = rptSett.generateDS(null,"","ALL", "U", null, "2018-01-02", null, null);
            // var excelBytes = ExcelHelper.ExportDataSet(dtMain);
            /*---REPORT GENERATION TEST*/
            //string reportPath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) : string.Empty;
            //string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;
            //IMainReport _repoSETT = new MainReport();
            //var setDate = new DateTime(2017, 11, 20);
            //string sett = _repoSETT.RPT_GenSettlementMASTER(setDate, reportPath, logopath, null);
            /*---END OF REPORT GENERATION---*/

            //////try
            //////{
            ////// var connect = new NapsConnect();
            ////// connect.PostNapsXPEApi();
            //////}
            //////catch (Exception ex)
            //////{
            ////// var test = ex.Message;
            ////// throw;
            //////}

            string destDb = ConfigurationManager.AppSettings["DEST_DB"].ToString();

            //using (var con = new RepoBase().OpenConnection(destDb))
            //{
            // var opdate = DateTime.Now;
            // var p1 = new DynamicParameters();
            // p1.Add("@P_DATE", opdate, DbType.Date);
            // var sql = "proc_set_PostToSettlementDetail";
            // var rec = con.Execute(sql, p1,commandTimeout:0, commandType: CommandType.StoredProcedure);
            //}
            LogFunction lgfn = new LogFunction();
            MEBCollectionClass TLAStarter = new MEBCollectionClass();
            SettlementProcess_MEB TLAStarter2 = new SettlementProcess_MEB();

            NapsConnect connect = new NapsConnect();
           
            // string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
            // //If Scheduled Time is passed set Schedule for the next day.
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Timer Initializing", null, null);


            try
            {
                //TLAStarter.MEBProcess();
                var random = new Random((int)DateTime.Now.Ticks);
                var randomValue = random.Next(1000000, 9999999);
                string batchno = randomValue.ToString();
                //NapsConnect connect = new NapsConnect();
                // new NapsConnect().TranxEnquiryApi();
                //////Parallel.Invoke(() =>
                //////{
                //////    new SettlementProcess_MEB().SettProcess(batchno, "2019-05-20");
                //////});

               // TLAStarter.ProcessSettlement(batchno);
                ///new SettProcess_POS().SettProcessPOS(batchno, "2019-06-10");
                //Process RuN

                //pull from DWHOUSE
                ///TLAStarter.ProcessSettlement(batchno);

                // Console.WriteLine("Enter Process Date");
                // var dt = Console.ReadLine();
                //TLAStarter2.SettProcess(batchno, dt);

                ////////connect.PostNapsXPEApiNeft();
                ////////connect.TranxEnquiryApiNeft();
                ///

                ///connect.PostNapsXPEApi();
                ///connect.TranxEnquiryApi();

                //NapsConnect connects = new NapsConnect();
                //connects.PostNapsXPEApiNeft();


                //TLAStarter.ProcessSettlement(batchno);
                // Parallel.Invoke(() =>
                // {
                //     //new SettlementProcess_MEBVALU().SETTProcess(batchno, dtFrom);
                // },
                //() =>
                //{
                //    //new SettlementProcess_MEBVALU2().SETTProcess(batchno, dtFrom);
                //});
            }

            catch (Exception ex)
            {
                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);


            }

            //try
            //{
            // SettlementProcess_MEB gg = new SettlementProcess_MEB();
            // gg.SettProcess("124", "2017-09-29");
            //}
            //catch (Exception ex)
            //{

            //}
        }

        static void PostNaps()
        {
            var destUri = "http://localhost:15443/Service1.asmx";
            var data = "";
            var rst = "";
            using (WebClient postClient = new WebClient())
            {
                postClient.Proxy = null;
                postClient.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
                // postClient.Headers.Add("SOAPAction", "http://tempuri.org");
                StreamReader reader = new StreamReader("C:\\Application\\v2018\\SettlementMaster\\SettlementMaster.App\\NapTemplate\\TranxPosting.xml");

                data = reader.ReadToEnd();
                reader.Close();

                // postClient.UploadStringCompleted += new UploadStringCompletedEventHandler(postClient_UploadStringCompleted);

                rst = postClient.UploadString(new Uri(destUri), data);

                //ServicePointManager.Expect100Continue = true;
                //.ServerCertificateValidationCallback = MyCertHandler;
            }
            //postClient_UploadStringCompleted(rst);
        }
        static List<DayOfWeek> GetWeekendList()
        {
            var lst = new List<DayOfWeek>();
            lst.Add(DayOfWeek.Saturday);
            lst.Add(DayOfWeek.Sunday);
            return lst;
        }
    }
}
