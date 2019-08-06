
using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Data
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> All { get; }
        DbSet<T> AllLocal { get; }
        IQueryable<T> AllEagerLocal(Expression<Func<T, bool>> filter = null);
        IEnumerable<T> AllEager(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
        T Find(object id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(object id);
       // T ExecuteCustomStoredProc<T>(string commandName, SqlParameter param);
        IEnumerable<T> LoadViaStockProc(string procName, object[] param); // SqlParameter param);//; object[] param);
        //List<rolemenu_Result> rolemenu(Nullable<int> roleid, Nullable<int> coyid);
        
    
    }
}
