using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPPosMaster.Data;

namespace UPPosMaster.Dapper.Model
{
    public class SettlementRuleObj : POSMISDB_SETTLEMENTRULE
    {
        public string PartyTypeDesc { get; set; }
        public string MCCCATEGORY_CODE { get; set; }
        public decimal? PARTYTYPE_CAP { get; set; }
        public string SETTLEMENTOPTION_DESC { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
    }
    public class InstitutionRuleObj : POSMISDB_INSTRULE_CBN
    {
        public string PartyTypeDesc { get; set; }
        public string SETTLEMENTOPTION_DESC { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
    }
    public class AcaRuleObj : POSMISDB_BL_ACCRULECBN
    {
        public string PartyTypeDesc { get; set; }
        public string TransferTypeDesc { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
        public string EVENTTYPE { get; set; }
    }
    public class AcquireSchemeObj : POSMISDB_ACQUIRERSCHEMETEMP
    {
        public string INSTITUTION_NAME { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public bool Deleted { get; set; }
    }

    public class SharingPartyObj : POSMISDB_SHAREDMSC2DETAILTEMP
    {
        public string PartyName { get; set; }

        public string MerchantName { get; set; }
      
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
    }
    public class SharingBlPartyObj : POSMISDB_BL_SHAREDMSC2TEMP
    {
        public string PartyName { get; set; }

        public string MerchantName { get; set; }

        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
    }
    public class MCCRuleObj : POSMISDB_MCCRULE
    {
        public string PartyTypeDesc { get; set; }
        public string MCC_DESC { get; set; }
        public string FullName { get; set; }
        public short MakeReadOnly { get; set; }
        public string CARDSCHEME_DESC { get; set; }
        public bool NewRecord { get; set; }
        public bool Updated { get; set; }
        public int SETTLEMENTRULE_ITBID { get; set; }
    
    }
    public class UserCount
    {
        public int ExistCount { get; set; }
       
    }
    public class DropdownObject
    {
        public string Code { get; set; }
        public string Description { get; set; }

    }
    public class DropdownObject2
    {
        public string value { get; set; }
        public string text { get; set; }

    }
}
