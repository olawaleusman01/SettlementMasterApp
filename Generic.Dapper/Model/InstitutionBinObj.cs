using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Data;

namespace Generic.Dapper.Model
{
    public class InstitutionBinObj 
    {
        public string INSTITUTION_NAME { get; set; }
        public string CURRENCY_NAME { get; set; }
        public string PARTYTYPE_DESC { get; set; }     
    }
    public class InstitutionBinUpldObj
    {
        public string ITBID { get; set; }
        public int? INSTITUTION_ID { get; set; }
        public int? ORIGINALINSTITUTION_ID { get; set; }
        public string BINTYPE { get; set; }
        public int? BIN { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string CARDSCHEME { get; set; }
        public string CURRENCY_CODE { get; set; }
    }
}
