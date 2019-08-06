using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.ReportClass
{
    public interface IMainReport
    {
        
       

        ////string RPT_GenBATCH(DateTime SETTDATE, string reportPATH, string lgopath, string conString);

        string RPT_GenSettlementMASTER(DateTime SETTDATE, string reportPATH, string lgopath, string conString);


        string RPT_NIBSS(DateTime SETTDATE, string reportPATH, string lgopath, string conString);

        string RPT_ACQUIREREISSMDB(DateTime SETTDATE, string destinationPath, string lgopath, string conString);

        //string RPT_GenSettlementFile2(DateTime SETTDATE, string destinationPath, string lgopath, string conString);

        //string RPT_GenMerchants(DateTime SETTDATE, string reportPATH, string lgopath, string conString);
        //string RPT_GenARTEEMerchant(DateTime SETTDATE, string reportPATH, string lgopath, string conString);
        //string RPT_GenVIPMerchant(DateTime SETTDATE, string reportPATH, string lgopath, string conString);

        ////string RPT_GenMDBMerchants(DateTime SETTDATE, string reportPATH, string lgopath, string conString);
        //string RPT_Settlement(DateTime SETTDATE, string reportPATH, string lgopath, string conString);


    }
}
