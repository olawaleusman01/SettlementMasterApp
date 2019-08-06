using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Dapper.Data;
using Generic.Data;

namespace Generic.Dapper.Utility
{
   
    public class AuthListUtil
    {
        //private readonly IUnitOfWork uow = null;
        //private readonly IRepository<POSMISDB_AUTHLIST> repoException = null;
        private readonly IDapperGeneralSettings _repo = new DapperGeneralSettings();
        // private readonly IRepository<Test2> repository2 = null;
        //public AuthListUtil()
        //{
        //    uow = new UnitOfWork();
        //    repoException = new Repository<POSMISDB_AUTHLIST>(uow);
        //    // repository2 = new Repository<Test2>(uow);
        //}

        //public int SaveLog(POSMISDB_AUTHLIST log)
        //{
        //    repoException.Insert(log);
        //    var rst = uow.Save(log.USERID);
        //    return rst;
        //}

        public AuthObj GetCheckerRecord(int menuId,decimal auth_ItbId, decimal recordId,int institutionId)
        {
            var rec = _repo.GetCheckerList(auth_ItbId, institutionId, menuId , recordId);
            return rec;
        }
        //public int CloseQueueRecord(decimal itbid)
        //{
        //    var rec = _repo.CloseRecord(itbid);
        //    return rec;     
        //}
        //public int PostCheckerRecord(POSMISDB_AUTHCHECKER obj)
        //{
        //    var rec = _repo.PostCheckerRecord(obj);
        //    return rec;
        //}


    }

    public class AuthListObj
    {
        public decimal ITBID { get; set; }
        public int MENUID { get; set; }
        public decimal RECORDID { get; set; }
        public string EVENTTYPE { get; set; }
        public DateTime MAKERDATE { get; set; }
        public string RECORDSTATUS { get; set; }
        public string MAKERID { get; set; }
        public decimal CKECKERITBID { get; set; }
        public string CHECKERUSERID { get; set; }
        public string NARRATION { get; set; }
        public DateTime CHECKERDATE { get; set; }
        public string CHECKERSTATUS { get; set; }
    }
    public class AuthObj
    {
        public List<AuthListObj> authListObj { get; set; }
        public decimal Auth_ITBID { get; set; }
    }

}
