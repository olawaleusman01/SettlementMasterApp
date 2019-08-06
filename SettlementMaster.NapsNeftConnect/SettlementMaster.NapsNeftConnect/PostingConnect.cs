using Generic.Dapper.Data;
using Generic.Dapper.PostConnect;
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

namespace SettlementMaster.NapsNeftConnect
{
    partial class PostingConnect : ServiceBase
    {
        LogFunction lgfn = new LogFunction();
        string interval = ConfigurationManager.AppSettings["IntervalMinutes"].ToString();
        NapsConnect connect = new NapsConnect();
        public PostingConnect()
        {
            InitializeComponent();
        }


        private Timer Schedular;
     
        protected override void OnStart(string[] args)
        {
            connect.PostProcessStatus("A");
            // TODO: Add code here to start your service.
            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] INFO Process Interval :" + interval + " minutes", null, null);

            this.ScheduleService();


        }
        public void ScheduleService()
        {
            try
            {
                //LogFunction2.WriteMaintenaceLogToFile("Maintenance Generation Schedular {0}");
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                // LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Mode: " + mode + " {0}");

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

                if (mode.ToUpper() == "INTERVAL")
                {
                    //Get the Interval in Minutes from AppSettings.
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);

                    //Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + schedule, null, null);

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                //LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "ANY ERROR:", ex.Message, null);

                //Stop the Windows Service.
                //using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
                //{
                //    serviceController.Stop();
                //}
            }
        }
        private void SchedularCallback(object e)
        {
            //string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
            //If Scheduled Time is passed set Schedule for the next day.
            


            try
            {
                //TLAStarter.MEBProcess();
                // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] Am Here Now", null, null);
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAPS CONNECT INFO", null, null);

                connect.PostNapsXPEApi();

            }

            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAPS CONNECTPOS PROCESS ERROR " + ex.Message, null, null);
            }

            try
            {
                //TLAStarter.MEBProcess();
                // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] Am Here Now", null, null);
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAP ENQUIRY INFO", null, null);

                connect.TranxEnquiryApi();

            }

            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAPS CONNECTPOS PROCESS ERROR " + ex.Message, null, null);
            }

            try
            {
                //TLAStarter.MEBProcess();
                // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] Am Here Now", null, null);
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NEFT CONNECT ", null, null);

                connect.PostNapsXPEApiNeft();

            }

            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NEFT CONNECTPOS PROCESS ERROR " + ex.Message, null, null);
            }

            try
            {
                //TLAStarter.MEBProcess();
                // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] Am Here Now", null, null);
                //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NEFT ENQUIRY INFO", null, null);

                ////connect.TranxEnquiryApiNeft();

            }

            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NEFT CONNECTPOS PROCESS ERROR " + ex.Message, null, null);
            }
            ////////try
            ////////{
            ////////    //TLAStarter.MEBProcess();
            ////////   // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] Am Here Now", null, null);
              
            ////////   // connect.PostNaps();

            ////////    connect.PostNapsXPEApi();

            ////////}

            ////////catch (Exception ex)
            ////////{
            ////////    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECTPOS PROCESS] ERROR " + ex.Message, null, null);
            ////////}

            ////////try
            ////////{
            ////////    //TLAStarter.MEBProcess();
            ////////    // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] Am Here Now", null, null);

            ////////    // connect.PostNaps();

            ////////    connect.PostNapsXPEApiNeft();

            ////////}

            ////////catch (Exception ex)
            ////////{
            ////////    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NEFT CONNECTPOS PROCESS] ERROR " + ex.Message, null, null);
            ////////}
            ////lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAPS CONNECT INFO Timer Closing", null, null);
            ScheduleService();
        }
        protected override void OnStop()
        {
            connect.PostProcessStatus("A");
            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] INFO Stopping MEB-POS Automation Process", null, null);
            this.Schedular.Dispose();

        }


    
    }
}
