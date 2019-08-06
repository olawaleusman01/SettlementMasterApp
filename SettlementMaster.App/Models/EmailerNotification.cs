using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Generic.Dapper.Model;
using Generic.Data;
using Generic.Dapper.Data;
using Generic.Dapper.Utility;
//using UPPosMaster.Dapper.Model;
//using UPPosMaster.Dapper.Utility;
//using UPPosMaster.Data.Management;
//using UPPosMaster.Data.Utility;
//using SettlementMaster.

namespace SettlementMaster.App.Models
{
    public class EmailerNotification
    {
        public void SendEmail(string body, string recipent)
        {
            this.SendHtmlFormattedEmail(recipent, "New Task Created!", body);
        }
        //    Public Sub SendEmail(ByVal body As String, ByVal recipent As String)
        //    'body = Me.PopulateBody()
        //    Me.SendHtmlFormattedEmail(recipent, "New Task Created!", body)
        //End Sub

        public string PopulateBody(string createdby,string url,string eventMsg,string eventlabel, DateTime createdDate)
        {
            string body = string.Empty;
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Template/EmailTemplate.htm"));
            body = reader.ReadToEnd();
            body = body.Replace("{CreatedBy}", createdby);
            //body = body.Replace("{url}", url);
            body = body.Replace("{EventType}", eventMsg);
          //  body = body.Replace("{EventLabel}", eventlabel);
            body = body.Replace("{CreatedDate}", createdDate.ToString("dd-MMM-yyyy"));
            //body = body.Replace("{Description}", description);

            //body = body.Replace("{createdby}", createdBy);
            return body;
        }
        public string PopulateUserBody(string UserName, string url, string Password,string FullName)
        {
            string body = string.Empty;
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Template/UserCreationTemplate.html"));
            body = reader.ReadToEnd();
            body = body.Replace("{UserName}", UserName);
            //body = body.Replace("{url}", url);
            body = body.Replace("{Password}", Password);
            body = body.Replace("{FullName}", FullName);
           
            return body;
        }
        public string PopulateMerchantReportBody(string mid, string mName, string date)
        {
            string body = string.Empty;
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Template/MerchantReportTemplate.html"));
            body = reader.ReadToEnd();
            body = body.Replace("{mid}", mid);
           // body = body.Replace("{url}", url);
            body = body.Replace("{MerchantName}", mName);
            body = body.Replace("{date}", date);

            return body;
        }
        public string PopulateUploadErrorMessage(string message,int record,string batchid,  string FullName)
        {
            string body = string.Empty;
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Template/FailedMerchantUpld.html"));
            body = reader.ReadToEnd();
            body = body.Replace("{message}", message);
            body = body.Replace("{record}", record.ToString());
            body = body.Replace("{batchid}", batchid);
          //  body = body.Replace("{FullName}", FullName);

            return body;
        }

        public string PopulateMerchantBody(string batchid, string institution_inputername, string request_type, string reason, DateTime createdDate)
        {
            string body = string.Empty;
            StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Template/UploadNotificationTemplate.html"));
            body = reader.ReadToEnd();
            body = body.Replace("{batchid}", batchid);
            body = body.Replace("{institution_inputername}", institution_inputername);
            body = body.Replace("{request_type}", request_type);
            //  body = body.Replace("{EventLabel}", eventlabel);
            body = body.Replace("{reason}", reason);
            //body = body.Replace("{Description}", description);

            //body = body.Replace("{createdby}", createdBy);
            return body;
        }
        public int SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(new MailAddress(recepientEmail));
            SmtpClient smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["Host"];
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];

            NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            smtp.Send(mailMessage);
            return 0;
        }

        public static void SendForAuthorization(int menuid,string fullName,string deptCode,int userinstitutionItbid,string eventMsg)
        {
            try
            {
                IDapperGeneralSettings _repo = new DapperGeneralSettings();
                var dg = _repo.GetAuthorizeEmailList(menuid, deptCode, userinstitutionItbid);

                if (dg.Count > 0)
                {
                    var mail = NotificationSystem.SendEmail(new EmailMessage()
                    {
                        EmailAddress = dg,
                        emailSubject = "Authorization required.",
                        EmailContent = new EmailerNotification().PopulateBody(fullName, "#", eventMsg, "", DateTime.Now),
                        EntryDate = DateTime.Now,
                        HasAttachment = false
                    });
                }
            }
            catch(Exception ex)
            {

            }
        }
        public static void SendToApprover(string fullName,string eventMsg,string approverId)
        {
            try
            {
                IDapperGeneralSettings _repo = new DapperGeneralSettings();
                var dg = _repo.GetApproverIdEmail(approverId);

                if (dg.Count > 0)
                {
                    var mail = NotificationSystem.SendEmail(new EmailMessage()
                    {
                        EmailAddress = dg,
                        emailSubject = "Authorization required.",
                        EmailContent = new EmailerNotification().PopulateBody(fullName, "#", eventMsg, "", DateTime.Now),
                        EntryDate = DateTime.Now,
                        HasAttachment = false
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        static string approve = StringEnum.GetStringvalue(GeneralSettings.StatusEnum2.APPROVED);
        public static void SendApprovalRejectionMail(string makerId, string eventType,string recordStatus,
            string formName,string rejectionReason,string authorizerName)
        {
            try
            {
                var _repo = new DapperGeneralSettings();
                var dg = _repo.GetMakerEmail(makerId);
                if (dg.Count <= 0)
                {
                    return;
                }
                //List<EmailObj> dg = new List<EmailObj>();
                //dg.Add(new EmailObj()
                //{
                //    Email = makerEmail,
                //});
                string eventMsg = recordStatus == approve ? string.Format("Record Approved Successfully") : "Record Rejected";
                var mail = NotificationSystem.SendEmail(new EmailMessage()
                {
                    EmailAddress = dg,

                    emailSubject = "Approval/Rejection Status.",

                    EmailContent = new EmailerNotification().PopulateBody(dg[0].FULLNAME, "#", eventMsg, "", DateTime.Now),
                    EntryDate = DateTime.Now,
                    HasAttachment = false
                });
            }
            catch
            {

            }
        }
        public static void SendApprovalRejectionMail2(decimal authId,string eventType, string recordStatus,
          string formName, string rejectionReason)
        {
            try
            {
                var _repo = new DapperGeneralSettings();
                var dg = _repo.GetApprovers_Mail_Email(authId);
                if (dg.Count <= 0)
                {
                    return;
                }
                //List<EmailObj> dg = new List<EmailObj>();
                //dg.Add(new EmailObj()
                //{
                //    Email = makerEmail,
                //});
                string eventMsg = recordStatus == approve ? string.Format("Record Approved Successfully") : "Record Rejected";
                var mail = NotificationSystem.SendEmail(new EmailMessage()
                {
                    EmailAddress = dg,

                    emailSubject = "Approval/Rejection Status.",

                    EmailContent = new EmailerNotification().PopulateBody(dg[0].FULLNAME, "#", eventMsg, "", DateTime.Now),
                    EntryDate = DateTime.Now,
                    HasAttachment = false
                });
            }
            catch
            {

            }
        }

        public static void SendMerchantUploadApprovalRejectionMail(string makerId, string eventType, string recordStatus,
         string formName, string rejectionReason, string batchId, string instInputerFullName, List<EmailObj> instEmail, int? InitiatorInstitutionId)
        {

            try
            {


                var _repo = new DapperGeneralSettings();
                var lst = new List<EmailObj>();
                var dg = _repo.GetGroupEmail(1, false, false).FirstOrDefault();
                if (dg != null)
                {
                    var gt = SplitGroupEmail(dg.EMAIL);
                    if (gt.Count != 0)
                    {
                        lst.AddRange(gt);
                    }

                }
                if (instEmail != null && instEmail.Count != 0)
                {
                    lst.AddRange(instEmail); //_repo.GetBusinessTeamEmail(makerId);
                }
                if (lst.Count <= 0)
                {
                    return;
                }
                // get institutionName
                var instName = "";
                if ( InitiatorInstitutionId != null &&  InitiatorInstitutionId != 0 )
                {
                   
                    var rr = _repo.GetInstitutionName(InitiatorInstitutionId.GetValueOrDefault());
                    if (rr != null)
                    {
                        instName = rr.INSTITUTION_NAME;
                    }
                }
                var inputName = instName; //= ""; // string.Concat(instName,"|", instInputerFullName ?? "");
                string eventMsg = recordStatus == approve ? string.Format("Approval of #{0} Merchant registeration ", batchId) : string.Format("Decline of #{0} Merchant registeration", batchId);
                var mail = NotificationSystem.SendEmail(new EmailMessage()
                {
                    EmailAddress = lst,

                    emailSubject = eventMsg,

                    EmailContent = new EmailerNotification().PopulateMerchantBody(batchId, inputName, eventType, rejectionReason, DateTime.Now),
                    EntryDate = DateTime.Now,
                    HasAttachment = false
                });

            }
            catch
            {

            }
        }

     private static List<EmailObj> SplitGroupEmail(string emailList)
        {
            var lst = new List<EmailObj>();
            var splt = emailList.Split(',');
            if (splt.Count() > 0)
            {
                foreach (var t in splt)
                {
                    lst.Add(new EmailObj()
                    {
                        Email = t
                    });
                }
            }
            return lst;
        }
    }
}