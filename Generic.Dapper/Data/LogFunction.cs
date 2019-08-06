using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public class LogFunction
    {
       
        //public void loginfo(string message, string exception, string DT)
        //{
        //    string logpath = "C:\\logFile\\" + "MEBCollog_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;

        //    //  if (!Directory.Exists(logpath))
        //    //{
        //    //    Directory.CreateDirectory(Path.GetDirectoryName(logpath));
        //    //}

        //    using (StreamWriter w = File.AppendText(logpath))
        //    {
        //        try {Log(message, exception, DT, w); }
        //        catch { }
               
        //    }

        //    ////using (StreamReader r = File.OpenText("log.txt"))
        //    ////{
        //    ////    DumpLog(r);
        //    ////}

        //}

        public void logSettinfo(string message, string exception, string DT)
        {
            string logpath = "C:\\logFile\\" + "MEBCollog_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;


            //  if (!Directory.Exists(logpath))
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(logpath));
            //}

            using (StreamWriter w = File.AppendText(logpath))
            {
                try { LogSett(message, exception, DT, w); }
                catch { }

            }

            ////using (StreamReader r = File.OpenText("log.txt"))
            ////{
            ////    DumpLog(r);
            ////}

        }
        //public void loginfo2(string message)
        //{
        //    string logpath = "C:\\logFile\\" + "MEBCollog_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;



        //    using (StreamWriter w = File.AppendText(logpath))
        //    {
        //        try { Log2(message,w); }
        //        catch { }

        //    }

        //    ////using (StreamReader r = File.OpenText("log.txt"))
        //    ////{
        //    ////    DumpLog(r);
        //    ////}

        //}

        //public void loginfo3(string message)
        //{
        //    string logpath = "C:\\logFile\\" + "MEBCollog_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;



        //    using (StreamWriter w = File.AppendText(logpath))
        //    {
        //        try { Log3(message, w); }
        //        catch { }

        //    }

        //    ////using (StreamReader r = File.OpenText("log.txt"))
        //    ////{
        //    ////    DumpLog(r);
        //    ////}

        //}
        public void loginfoMSG(string message, string exception, string DT)
        {
            string logpath = "C:\\logFile\\" + "MEBCollog_" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;

            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logpath));
            }

            using (StreamWriter w = File.AppendText(logpath))
            {
                try { LogP(message, exception, DT, w); }
                catch { }

            }

            ////using (StreamReader r = File.OpenText("log.txt"))
            ////{
            ////    DumpLog(r);
            ////}

        }
        public void logNapsinfoMSG(string message, string exception, string DT)
        {
            string logpath = "C:\\logFile\\" + "NapsNeftLog"  + ".txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;

            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logpath));
            }

            using (StreamWriter w = File.AppendText(logpath))
            {
                try { LogP(message, exception, DT, w); }
                catch { }

            }

            ////using (StreamReader r = File.OpenText("log.txt"))
            ////{
            ////    DumpLog(r);
            ////}

        }
        public static void LogP(string logMessage, string exception, string DT, TextWriter w)
        {
            if (exception == string.Empty)
            {

                exception = "none";
            }
            w.Write("\r\nLog Info - MEB Services: ");
            w.WriteLine("Messages: {0} Any Error:{1} Time:{2}", logMessage, exception, DT);

        }

        public static void Log2(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Info - MRP Services: ");
            w.WriteLine("Messages: {0}", logMessage);

        }

        public static void Log3(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Info - MEB Collection Services: ");
            w.WriteLine("Messages: {0}", logMessage);

        }

        public static void Log(string logMessage, string exception, string DT, TextWriter w)
        {
            if (exception ==string.Empty )
            {

                exception = "none";
            }
            w.Write("\r\nLog Info - MEB Upload Services: ");
            w.WriteLine("Messages: {0} Any Error:{1} Time:{2}", logMessage, exception, DT);
    
        }


        public static void LogSett(string logMessage, string exception, string DT, TextWriter w)
        {
            if (exception == string.Empty)
            {

                exception = "none";
            }
            w.Write("\r\nLog Info - Settlement Services: ");
            w.WriteLine("Messages: {0} Any Error:{1} Time:{2}", logMessage, exception, DT);

        }


        ////public static void DumpLog(StreamReader r)
        ////{
        ////    string line;
        ////    while ((line = r.ReadLine()) != null)
        ////    {
        ////        Console.WriteLine(line);
        ////    }
        ////}

    }
}
