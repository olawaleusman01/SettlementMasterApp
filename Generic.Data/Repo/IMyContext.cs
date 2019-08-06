using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Data
{
    public interface IMyContext : IDisposable
    {
        int SaveChanges(string userid,string authId);
    }
}
