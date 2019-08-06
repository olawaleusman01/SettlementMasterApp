using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPPosMaster.Data;

namespace UPPosMaster.Dapper.Model
{
    public class VipObj 
    {
        public decimal? CUSTOMERID { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string CUSTOMEREMAIL { get; set; }
        public string PATH { get; set; }
    }
    public class MerchantDetailObj : POSMISDB_MERCHANTDETAIL
    {
        public string FullName { get; set; }
    }
    public class CurrencyObj : POSMISDB_CURRENCYTEMP
    {
        public string FULLNAME { get; set; }
        public new Nullable<long> RECORDID { get; set; }
    }
    public class DepartmentObj : POSMISDB_DEPARTMENTTEMP
    {
        public string FULLNAME { get; set; }
    }
    public class CountryObj : POSMISDB_COUNTRYTEMP
    {
        public string FULLNAME { get; set; }
    }
    public class DistinctTerminalObj
    {
        
        public string INSTITUTION_SHORTCODE { get; set; }
        public string INSTITUTION_CBNCODE { get; set; }

        public string START_ACCNO_XML { get; set; }

       
    }
    public class StateObj : POSMISDB_STATETEMP
    {
        public string FULLNAME { get; set; }

        public string Country_Name { get; set; }
    }
    public class CityObj : POSMISDB_CITYTEMP
    {
        public string FULLNAME { get; set; }
        public string STATENAME { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string Country_Name { get; set; }
    }
    public class FrequencyObj : POSMISDB_FREQUENCYTEMP
    {
        public string FULLNAME { get; set; }
    }
    public class PartyObjList : POSMISDB_PARTYTEMP
    {
        public string FULLNAME { get; set; }
        public new long ITBID { get; set; }
        public string PARTYTYPE_DESC { get; set; }
        
    }
    public class BillerObjList : POSMISDB_BILLERTEMP
    {
        public string FULLNAME { get; set; }
        public new long ITBID { get; set; }
        public string PARTYTYPE_DESC { get; set; }
        public string CHANNEL_DESC { get; set; }
        public string MERCHANTNAME { get; set; }
    }
    
    public class MerchantTerminalUpldObj : POSMISDB_UPMERTERMUPLDREC
    {
        public string FULLNAME { get; set; }      
    }
    public class MerchantUpdateUpldObj : POSMISDB_UPMERCHANTUPDATE
    {
        public string FULLNAME { get; set; }
    }
    public class valObj
    {
        public int RespCode { get; set; }
        public int ROW_COUNT { get; set; }
        
        public decimal? SETTLEMENTOPTION_ID { get; set; }
    }
}
