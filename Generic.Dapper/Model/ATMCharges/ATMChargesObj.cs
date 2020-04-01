using Generic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{
    public class ATMChargesObj : ATM_CHARGES
    {
        public decimal RECORDID { get; set; }
        public string DisplayIntL { get { return ITBID == 0 ? "none" : "block"; } }
        public string EVENTTYPE { get; set; }
        public string PartyValue { get; set; }
        public string REQUESTTYPE_DESC { get; set; }
        public string CALC_BASIS { get; set; }
        public string TRAN_DESC { get; set; }
        public string OPERATORTYPE_DESC { get; set; }
        public string OPERATORTYPE { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
    }
}
