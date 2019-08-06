using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Data
{
    public interface IUnitOfWork : IDisposable
    {
        int Save(string userid,string authId = null);
        IMyContext Context { get; }
    }
}
