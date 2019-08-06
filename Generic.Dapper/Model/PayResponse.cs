using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{

    public class PayResponse
    {
        //public Xml xml { get; set; }
        public Paymentresponse PaymentResponse { get; set; }
    }

    public class Xml
    {
        public string version { get; set; }
        public string encoding { get; set; }
    }

    public class Paymentresponse
    {
        public Header Header { get; set; }
        public string HashValue { get; set; }
    }

    public class Header
    {
        public string ScheduleId { get; set; }
        public string ClientId { get; set; }
        public string DebitSortCode { get; set; }
        public string DebitAccountNumber { get; set; }
        public string Status { get; set; }
    }

}
