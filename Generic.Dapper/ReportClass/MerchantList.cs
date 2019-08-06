using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPPosMaster.Dapper.ReportClass
{
       public partial class MerchantList
    {
       
       
         public string MERCHANTID { get; set; }
        public string MERCHANTNAME { get; set; }

        public string BANKNAME { get; set; }
        public string SETTLEMENTACCOUNT { get; set; }
        public string ADDRESS { get; set; }
        public string EMAIL { get; set; }
        public string PHONENO { get; set; }
        public string COLLECTION { get; set; }


    }
}
