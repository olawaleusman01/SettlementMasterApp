
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
//using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;






namespace Generic.Data
{
       
    public class Repository<T> : IRepository<T> where T : class
    {
       //bSet<T> dbset;
        private readonly MyContext context;
        public Repository(IUnitOfWork uow)
        {
            context = uow.Context as MyContext;
        }
        public IEnumerable<T> All
        {
            get
            {
                return context.Set<T>();
            }
        }
        public DbSet<T> AllLocal
        {
            get
            {
                return context.Set<T>();
            }
        }
        public IQueryable<T> AllEagerLocal(Expression<Func<T, bool>> filter = null)
        {
            DbSet<T> query = context.Set<T>();
            IQueryable<T> query2 = null;
            if (filter != null)
            {
                query2 = query.Where(filter);
            }
            // var dd = (DbSet<T>)query2;
            return query2 == null ? query : query2;
        }
        public IEnumerable<T> AllEager(Expression<Func<T,bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();
          
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            
            if (filter != null)
            {
               
                    query = query.Where(filter);
                
            }
            return query;
        }
        public T Find(object id)
        {
            return context.Set<T>().Find(id);
        }
        public void Insert(T item)
        {
            try
            {
                context.Entry(item).State = EntityState.Added;
            }
            catch (Exception ex)
            {

            }
           
           // dynamic obj = context.Set<T>().Add(item);
        
            
        }

       /// <summary>
       /// Update The Entity in the DbContext
       /// </summary>
       /// <param name="item"></param>
        public void Update(T item)
        {
            context.Set<T>().Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }
        public void Delete(object id)
        {
            var item = context.Set<T>().Find(id);
            context.Set<T>().Remove(item);
        }
        public IEnumerable<T> LoadViaStockProc(string procName, params object[] param) // SqlParameter param)// params object[] param)
        {
            
               // new ObjectParameter("roleid", typeof(int));
            //string val = string.Empty;
            //foreach (var h in param)
            //{
            //    val += h + ",";
            //}
          
          //  var g = val.TrimEnd(new char[]{Convert.ToChar(","),Convert.ToChar(",")}) ;
            //user.TrimEnd(New Char() {","c, " "c, ControlChars.Lf})
            IEnumerable<T> res = context.Database.SqlQuery<T>(procName);
            return res;
            // get
            //{
            //    return context.Database.SqlQuery<T>("EXEC Test");
            //}
        }

        //public virtual List<rolemenu_Result> rolemenu(Nullable<int> roleid, Nullable<int> coyid)
        //{
        //    var roleidParameter = roleid.HasValue ?
        //        new ObjectParameter("roleid", roleid) :
        //        new ObjectParameter("roleid", typeof(int));

        //    var coyidParameter = coyid.HasValue ?
        //        new ObjectParameter("coyid", coyid) :
        //        new ObjectParameter("coyid", typeof(int));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<rolemenu_Result>("rolemenu", roleidParameter, coyidParameter).ToList();
        //}
        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }
}
