using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Dapper.Model;

namespace Generic.Dapper.Utility
{
    public partial class EmailMessage
    {
        public long itbid { get; set; }
        public string EmailContent { get; set; }
        public List<EmailObj> EmailAddress { get; set; }
        public string emailPlatform { get; set; }
        public string EmailStatus { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<bool> HasAttachment { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentPath { get; set; }
        public string FromAddress { get; set; }
        public string emailSubject { get; set; }
        public string bankfiid { get; set; }
        public Nullable<int> nostrial { get; set; }
    }
}
