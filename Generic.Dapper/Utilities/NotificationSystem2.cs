using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Generic.Dapper.Data;
using Generic.Dapper.Utility;

namespace Generic.Data.Utility
{
  
    public class NotificationSystemServiceEmail
    {
        static string  emailhost = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["emailhost"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["emailhost"]) : string.Empty;
        static string emailpassword = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["emailpassword"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["emailpassword"]) : string.Empty;
        static string emailport = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["emailport"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["emailport"]) : string.Empty;
        static string emailaddress = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["emailaddress"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["emailaddress"]) : string.Empty;
        static string emailUser = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UserName"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UserName"]) : string.Empty;
        static string enableSSl = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]) : string.Empty;
        static string usePort = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["UsePort"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UsePort"]) : string.Empty;

        public static EmailResponse SendEmailProcess(EmailMessage eObject)
        {
            bool _usePort;
            bool _enableSSl;
            // EmailResponse isSent = EmailResponse.EMPTY_OBJECT;
            int val;
            _enableSSl = enableSSl == "1" ? true : false;
            _usePort = usePort == "1" ? true : false;
            EmailMessage theEmailer = new EmailMessage();
            theEmailer.EmailContent = eObject.EmailContent;
          //  theEmailer.EmailAddress = eObject.EmailAddress.Trim();
           // theEmailer.emailPlatform = eObject.;
            theEmailer.EntryDate = DateTime.Now;
            //theEmailer.HasAttachment = eObject.HasAttachment;
            //theEmailer.Attachment = eObject.Attachment;
            theEmailer.FromAddress = eObject.FromAddress;
            theEmailer.emailSubject = eObject.emailSubject;
            using (MailMessage mm = new MailMessage())
            {
               // SmtpMail sm = new SmtpMail("TryIt");
                SmtpClient smtp = new SmtpClient();
                smtp.Host = emailhost;
                smtp.EnableSsl = _enableSSl;
                if (_usePort)
                {
                    smtp.Port = int.Parse(emailport);
                }
                smtp.DeliveryMethod =  SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = true;
                NetworkCredential NetworkCred = new NetworkCredential(emailUser, emailpassword);
                smtp.Credentials = NetworkCred;
                mm.IsBodyHtml = true;
                mm.Subject = theEmailer.emailSubject;
                mm.From = new MailAddress(emailaddress);
                foreach (var d in eObject.EmailAddress)
                {
                    mm.To.Add(d.Email);
                }

                // mm.Body = theEmailer.EmailContent;

                Attachment oAttachment = new Attachment("C:\\Email_Template\\AppLogodd.png");
                string contentID = "appLogo@01";
                oAttachment.ContentId = contentID;
                Attachment oAttachment2 = new Attachment("C:\\Email_Template\\PROMO-GREEN2_01_01.jpg");
                string contentID2 = "app@logo02";
                oAttachment2.ContentId = contentID2;
                Attachment oAttachment3 = new Attachment("C:\\Email_Template\\PROMO-GREEN2_01_04.jpg");
                string contentID3 = "appLogo@03";

                if (eObject.HasAttachment == true)
                {
                    Attachment oAttachment4 = new Attachment(eObject.AttachmentPath);
                    //string contentID4 = "appLogo@03";
                    mm.Attachments.Add(oAttachment4);
                }
                oAttachment3.ContentId = contentID3;
                mm.Attachments.Add(oAttachment);
                mm.Attachments.Add(oAttachment2);
                mm.Attachments.Add(oAttachment3);
                theEmailer.EmailContent =   theEmailer.EmailContent.Replace("{ImgLogo}", "cid:" + contentID);
                theEmailer.EmailContent = theEmailer.EmailContent.Replace("{ImgGreen01}", "cid:" + contentID2);
                theEmailer.EmailContent = theEmailer.EmailContent.Replace("{ImgGreen04}", "cid:" + contentID3);
               
                mm.Body = theEmailer.EmailContent;

                ////mm.IsBodyHtml = true;
                //// string logopath = System.Web.HttpContext.Current.Server.MapPath("~\\Template\\AppLogodd.png");

                ////AlternateView av1 = AlternateView.CreateAlternateViewFromString(mm, null, MediaTypeNames.Text.Html);
                ////LinkedResource logo = new LinkedResource(logopath);
                ////logo.ContentId = "imgsrc";
                ////av1.LinkedResources.Add(logo);
                ////mm.AlternateViews.Add(av1);

                try
                {
                    smtp.Send(mm);
                    val = 0;

                    //var msgDetail = repoEmail.All.FirstOrDefault(e => e.EmailAddress == theEmailer.EmailAddress && e.EmailStatus == "N");
                    //if (msgDetail != null)
                    //{
                    //    msgDetail.EmailStatus = "Y";
                    //    repoEmail.Insert(msgDetail);
                    //    uow.Save(eObject.Userid);
                    //}
                    
                  
                }
                catch (Exception ex)
                {
                    val = 1;
                 
                    loginfo2("");
                    loginfo(ex.Message, null, DateTime.Now.ToString());
                    loginfo2("");
                }

            }

            return val == 0 ? EmailResponse.SUCCESSFUL : EmailResponse.NOT_SUCCESSFUL;

        }

        public static EmailResponse SendEmail(EmailMessage eObject)
        {
           
            EmailResponse response =  SendEmailProcess(eObject);
            return response;

        }
        public static void loginfo(string message, string exception, string DT)
        {
            string logpath = "C:\\maillogFile\\log.txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;


            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logpath));
            }

            using (StreamWriter w = File.AppendText(logpath))
            {
                try { Log(message, exception, DT, w); }
                catch { }

            }

            ////using (StreamReader r = File.OpenText("log.txt"))
            ////{
            ////    DumpLog(r);
            ////}

        }

        public static void Log2(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Info - Email Services: ");
            w.WriteLine("Messages: {0}", logMessage);

        }
        public static void loginfo2(string message)
        {
            string logpath = "C:\\maillogFile\\log.txt"; // !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["logfile"]) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logfile"]) : string.Empty;
         
            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logpath));
            }

            using (StreamWriter w = File.AppendText(logpath))
            {
                try { Log2(message, w); }
                catch { }

            }

            ////using (StreamReader r = File.OpenText("log.txt"))
            ////{
            ////    DumpLog(r);
            ////}

        }
        public static void Log(string logMessage, string exception, string DT, TextWriter w)
        {
            if (exception == string.Empty)
            {

                exception = "none";
            }
            w.Write("\r\nLog Email Service: ");
            w.WriteLine("Messages: {0} Any Error:{1} Time:{2}", logMessage, exception, DT);

        }

        //////public SmsResponse SendSms(SmsObject sObject)
        //////{

        //////    int val;
        //////    SMSMessage theSMS = new SMSMessage();
        //////    theSMS.SMSContent = sObject.SmsContent;
        //////    theSMS.PhoneNo = sObject.MobileNo;
        //////    theSMS.SMSStatus = "N";
        //////    theSMS.SenderID = sObject.SenderId;
        //////    theSMS.EntryDate = DateTime.Now;
        //////    //theSMS.HasAttachment = eObject.HasAttachment;
        //////    //theSMS.Attachment = eObject.Attachment;
        //////      using (var db = new TechBridgeDBEntities())
        //////    {
        //////        db.AddToSMSMessages(theSMS);

        //////        val = db.SaveChanges();
        //////    }


        //////    return val > 0 ? SmsResponse.SUCCESSFUL : SmsResponse.NOT_SUCCESSFUL;

        //////}

        //[DataContract()]
        //public enum SmsResponse
        //{
        //    [EnumMember()]
        //    SUCCESSFUL = 0,
        //    [EnumMember()]
        //    NOT_SUCCESSFUL = 1,
        //    [EnumMember()]
        //    MESSAGE_TOO_LONG = 2,
        //    [EnumMember()]
        //    UNKNOW_ERROR = 3,
        //    [EnumMember()]
        //    EMPTY_OBJECT = 4,
        //    [EnumMember()]
        //    INVALID_MOBILENO = 5,
        //    [EnumMember()]
        //    INVALID_INDENTIFIER = 6

        //}

        [DataContract()]
        public enum EmailResponse
        {
            [EnumMember()]
            SUCCESSFUL = 0,
            [EnumMember()]
            NOT_SUCCESSFUL = 1,
            [EnumMember()]
            INVALID_MESSAGE = 2,
            [EnumMember()]
            UNKNOW_ERROR = 3,
            [EnumMember()]
            EMPTY_OBJECT = 4,
            [EnumMember()]
            INVALID_EMAILADDRESS = 5,
            [EnumMember()]
            INVALID_INDENTIFIER = 6,
            [EnumMember()]
            INVALID_ATTACHMENT = 7,
            [EnumMember()]
            INVALID_FROMADDRESS = 8

        }

    }

   
    }



    

