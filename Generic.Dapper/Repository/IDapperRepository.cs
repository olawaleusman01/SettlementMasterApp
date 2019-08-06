using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Repository
{
    public interface IDapperRepository<T> where T : class
    {
        int Insert(T parameter, string connectionString);
        IList<T> GetAll(string connectionString);
        IEnumerable<T> GetAllWithProcedure(string spName, string connectionString, bool buffer = false);
        T Find(string spName, object Id, string KeyName, string connectionString);
        List<T> LoadViaStoreProc(string spName, DynamicParameters dp, string connectionString);
        List<T> LoadViaStoreProc2(string spName, DynamicParameters dp, bool buffer, string connectionString);

    }
}
