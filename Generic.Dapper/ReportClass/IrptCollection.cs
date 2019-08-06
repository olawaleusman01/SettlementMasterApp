using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPPosMaster.Dapper.ReportClass
{
    public interface IrptCollection
    {
        string GenSettlementCollection(DateTime SETTDATE, string destinationPath, string lgopath, string conString);

    }
}
