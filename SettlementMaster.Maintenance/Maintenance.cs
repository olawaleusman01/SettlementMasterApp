using Generic.Dapper.Data;
using Generic.Dapper.ReportClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SettlementMaster.Maintenance
{
    partial class Maintenance : ServiceBase
    {
        //string twoConString = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["TwoConfig"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["TwoConfig"]) : string.Empty;
        //string MEBSource = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["MEBSource"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["MEBSource"]) : string.Empty;
        //string MEBTarget = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["MEBTarget"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["MEBTarget"]) : string.Empty;
        string reportPath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]) : string.Empty;
        string logopath = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogoPath"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["LogoPath"]) : string.Empty;
        public Maintenance()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            LogFunction2.WriteMaintenaceLogToFile("Maintenance Service started {0}");

            LogFunction2.WriteMaintenaceLogToFile("");

            //LogFunction2.WriteMaintenaceLogToFile("Email Serice Processing Started");
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Interval = 60000 * 3; // 60 seconds * 60 minute * 4 hours
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(this.EmailScheduleService);
            //timer.Start();
            ////this.ScheduleService();
            LogFunction2.WriteMaintenaceLogToFile("");
            this.ScheduleService();


        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            LogFunction2.WriteMaintenaceLogToFile("Maintenance Service stopped {0}");
            this.Schedular.Dispose();
        }
        //  private Timer Schedular;



        private Timer Schedular;
        public void ScheduleService()
        {
            try
            {
                LogFunction2.WriteMaintenaceLogToFile("Maintenance Generation Schedular {0}");
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Mode: " + mode + " {0}");

                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;
                //var gen = new GenXml();
                //var schedule2 = gen.GetSchedule();

                //foreach (var d in schedule2)
                //{
                //    // int intervalMinutes;
                //    if (DateTime.TryParse(d.SCHEDULE_TIME, out scheduledTime))
                //    {

                //        if (DateTime.Now > scheduledTime)
                //        {
                //            //If Scheduled Time is passed set Schedule for the next day.
                //            scheduledTime = scheduledTime.AddDays(1);
                //            break;

                //        }
                //    }

                //}
                if (mode == "DAILY")
                {
                    //Get the Scheduled Time from AppSettings.
                    scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTime"]);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }

                //if (mode.ToUpper() == "INTERVAL")
                //{
                //    //Get the Interval in Minutes from AppSettings.
                //    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);

                //    //Set the Scheduled Time by adding the Interval to Current Time.
                //    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                //    if (DateTime.Now > scheduledTime)
                //    {
                //        //If Scheduled Time is passed set Schedule for the next Interval.
                //        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                //    }
                //}

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                LogFunction2.WriteMaintenaceLogToFile("Maintenance Service scheduled to run after: " + schedule + " {0}");

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);

                //Stop the Windows Service.
                //using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
                //{
                //    serviceController.Stop();
                //}
            }
        }

        private void SchedularCallback(object e)
        {

            try
            {
                DateTime dtFromA;
                LogFunction2.WriteMaintenaceLogToFile("Start processing..........");
                var conDate = ConfigurationManager.AppSettings["SettlementDate"].ToUpper();
                var dtFrom = "";
                if (DateTime.TryParse(conDate, out dtFromA))
                {
                     dtFrom = dtFromA.ToString("dd-MMM-yyyy");
                }
                else
                {
                     dtFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                LogFunction2.WriteMaintenaceLogToFile("enter a report processing date..........");
                // string dtFrom = Console.ReadLine();

                LogFunction2.WriteMaintenaceLogToFile(dtFrom);

                DateTime dtFromDate = Convert.ToDateTime(dtFrom);


                ////IDapperSettlementReport _repoSETT = new DapperSettlementReport();
                ////bool sett = _repoSETT.GenSettlementFileSETT_EEPLUS(dtFromDate, null, reportPath, logopath, null);

                //LogFunction2.WriteMaintenaceLogToFile("enter a report type..........");
                //LogFunction2.WriteMaintenaceLogToFile("1.  SETTLEMENT MASTER REPORTS");
                //LogFunction2.WriteMaintenaceLogToFile("2.  NIBSS SETTLEMENT REPORTS");
                //LogFunction2.WriteMaintenaceLogToFile("3.  ACQUIRERE/ISSUER/MDB REPORTS");
                //LogFunction2.WriteMaintenaceLogToFile("4.  MERCHANT REPORTS");
                //LogFunction2.WriteMaintenaceLogToFile("5.  PTSP/PTSA/TERW/SWTH REPORTS");

                //string opt = Console.ReadLine();
                IMainReport _repoSETT = new MainReport();
                //var P = new EmailProcess();
                //string emailSubject = string.Format("SETTLEMENT REPORT FOR {0} ", dtFromDate.ToString("dd-MMM-yyyy"));

                try
                {
                    LogFunction2.WriteMaintenaceLogToFile("processing........SETTLEMENT MASTER REPORT" + dtFrom);
                    //LogFunction2.WriteMaintenaceLogToFile("processing........Settlement Date--" + dtFromDate);
                   // LogFunction2.WriteMaintenaceLogToFile("processing........Settlement Report Path--" + reportPath);
                    //LogFunction2.WriteMaintenaceLogToFile("processing........Settlement Logo Path--" + logopath);

                    string sett = _repoSETT.RPT_GenSettlementMASTER(dtFromDate, reportPath, logopath, null);
                    try
                    {
                        //SEND EMAIL HERE
                      //  P.SendSettlementMasterNotification(dtFrom, string.Concat(emailSubject, ":SETTLEMENT MASTER REPORT"));
                    }
                    catch (Exception ex)
                    {
                        LogFunction2.WriteMaintenaceLogToFile("Maintenance Settlement Email Service Error on: {0} " + ex.Message + ex.StackTrace);
                    }
                    LogFunction2.WriteMaintenaceLogToFile("completed........SETTLEMENT MASTER REPORT: {0}" + dtFrom);
                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                }

                //try
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("processing..........Merchant Reports" + dtFrom);
                //    string sett = _repoSETT.RPT_GenARTEEMerchant(dtFromDate, reportPath, logopath, null);
                //    try
                //    {
                //        LogFunction2.WriteMaintenaceLogToFile("completed..........Merchant Reports" + dtFrom);
                //        P.SendMerchantarteeSettlementEmail(dtFrom, "");
                //    }
                //    catch
                //    {

                //    }
                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                //}

                //try
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("processing.........NIBSS Settlement Report" + dtFrom);
                //    string sett = _repoSETT.RPT_GenSettlementFile2(dtFromDate, reportPath, logopath, null);
                //    try
                //    {
                //        P.SendSettlementMasterNotification(dtFrom, string.Concat(emailSubject, ":NIBSS REPORT"));
                //    }
                //    catch (Exception ex)
                //    {
                //        LogFunction2.WriteMaintenaceLogToFile("Maintenance Nibss Email Service Error on: {0} " + ex.Message + ex.StackTrace);
                //    }
                //    LogFunction2.WriteMaintenaceLogToFile("completed.........NIBSS Settlement Report" + dtFrom);
                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                //}

                //try
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("processing.........ACQUIRER/ISSUER/MDB REPORT" + dtFrom);
                //    string sett = _repoSETT.RPT_GenSettlementFile(dtFromDate, reportPath, logopath, null);
                //    //SEND EMAIL HERE
                //    //P.SendSettlementInstitutionNotification("12-MAY-2017", "");
                //    LogFunction2.WriteMaintenaceLogToFile("completed.........ACQUIRER/ISSUER/MDB REPORT" + dtFrom);
                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                //}

                //try
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("processing..........Party Report (PTSP/PTSA/TERW/SWTH)" + dtFrom);

                //    string sett = _repoSETT.RPT_Settlement(dtFromDate, reportPath, logopath, null);
                //    LogFunction2.WriteMaintenaceLogToFile("completed..........Party Report (PTSP/PTSA/TERW/SWTH)" + dtFrom);
                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                //}
                //try
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("processing..........Merchant Reports" + dtFrom);
                //    string sett = _repoSETT.RPT_GenMerchants(dtFromDate, reportPath, logopath, null);
                //    LogFunction2.WriteMaintenaceLogToFile("completed..........Merchant Reports" + dtFrom);
                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                //}

                //try
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("processing..........Merchant Reports" + dtFrom);
                //    string sett = _repoSETT.RPT_GenMerchants(dtFromDate, reportPath, logopath, null);
                //    LogFunction2.WriteMaintenaceLogToFile("completed..........Merchant Reports" + dtFrom);
                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                //}


                ////if (opt == "5")
                ////{
                ////    LogFunction2.WriteMaintenaceLogToFile("processing..........Merchant Report By Merchant Deposit Bank" + dtFrom);

                ////    string sett = _repoSETT.RPT_GenMDBMerchants(DateTime.Parse(dtFrom), reportPath, logopath, null);
                ////}




                LogFunction2.WriteMaintenaceLogToFile("processing Completed");
            }
            catch (Exception ex)
            {
                LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
            }
            //try
            //{
            //   var pDays = ConfigurationManager.AppSettings["purgedays"].ToUpper();
            //    int purgeDays = 0;
            //    if (int.TryParse(pDays, out purgeDays))
            //    {
            //        LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Log: {0}");
            //        var gen = new Maintenance();
            //        gen.Purge(purgeDays);
            //        LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Completed Log: {0}");
            //    }
            //        this.ScheduleService();

            //}
            //catch (Exception ex)
            //{
            //    LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);

            //    //Stop the Windows Service.
            //    //using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
            //    //{
            //    //    serviceController.Stop();
            //    //}
            //}
        }

        
    }
}
