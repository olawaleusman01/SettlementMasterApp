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
using Generic.Dapper.Data;
namespace SettlementMaster.CollectionService
{
    partial class MEBCollection : ServiceBase
    {
        string interval = ConfigurationManager.AppSettings["IntervalMinutes"].ToString();
        LogFunction lgfn = new LogFunction();
        MEBCollectionClass TLAStarter = new MEBCollectionClass();
        //private readonly IDapperWindowServiceSettings _repo = new DapperWindowServiceSettings();
        public MEBCollection()
        {
            InitializeComponent();
        }

        private Timer Schedular;
        //protected override void OnStart(string[] args)
        //{

        //    // lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Starting MEB-POS Automation Process", null, null);
        //    System.Timers.Timer timer = new System.Timers.Timer();
        //    timer.Interval = 60000 * Convert.ToInt32(interval); // 60 seconds * 60 minute * 4 hours
        //    lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Process Interval :" + interval + " minutes", null, null);

        //    //timer.Interval = 60000 * 10 ; // 60 seconds * 60 minute * 4 hours
        //    timer.Elapsed += new System.Timers.ElapsedEventHandler(this.MEBCollectionSrv);
        //    timer.Start();
        //    //this.ScheduleService();


        //}
        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Process Interval :" + interval + " minutes", null, null);

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

                lgfn.loginfoMSG(DateTime.Now.ToString() + schedule, null, null);

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                //LogFunction2.WriteMaintenaceLogToFile("Maintenance Service Error on: {0} " + ex.Message + ex.StackTrace);
                lgfn.loginfoMSG(DateTime.Now.ToString() + "ANY ERROR:", ex.Message, null);

                //Stop the Windows Service.
                //using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
                //{
                //    serviceController.Stop();
                //}
            }
        }
        private void SchedularCallback(object e)
        {
            string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
            //If Scheduled Time is passed set Schedule for the next day.
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Timer Initializing", null, null);


            try
            {
                //TLAStarter.MEBProcess();
                var random = new Random((int)DateTime.Now.Ticks);
                var randomValue = random.Next(1000000, 9999999);
                string batchno = randomValue.ToString();
                TLAStarter.ProcessSettlement(batchno);

            }

            catch (Exception ex)
            {
                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);
            }
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Timer Closing", null, null);
            ScheduleService();
        }
        protected override void OnStop()
        {

            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Stopping MEB-POS Automation Process", null, null);
            this.Schedular.Dispose();

        }


        public void MEBCollectionSrv(object sender, System.Timers.ElapsedEventArgs args)
        {

            string dtFrom2 = DateTime.Today.ToString("dd-MMM-yyyy");
            //If Scheduled Time is passed set Schedule for the next day.
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Timer Initializing", null, null);


            try
            {
                //TLAStarter.MEBProcess();
                var random = new Random((int)DateTime.Now.Ticks);
                var randomValue = random.Next(1000000, 9999999);
                string batchno = randomValue.ToString();
                TLAStarter.ProcessSettlement(batchno);
            }

            catch (Exception ex)
            {
                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] ERROR " + ex.Message, null, null);


            }


            //////try
            //////{
            //////    var random = new Random((int)DateTime.Now.Ticks);
            //////    var randomValue = random.Next(1000000, 9999999);
            //////    string batchno = randomValue.ToString();

            //////    TLAStarter.SettlementProcess(DateTime.Today.ToString("dd-MMM-yyyy"));

            //////}

            //////catch (Exception ex)
            //////{
            //////    lgfn.loginfoMSG(ex.Message, " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

            //////}

            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO Timer Closing", null, null);


        }
    }
}
