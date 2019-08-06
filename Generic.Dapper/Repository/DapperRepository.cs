using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Repository
{
    public class DapperRepository<T> : IDapperRepository<T> where T : class
    {
        public int Insert(T parameter, string connectionString) 
        {
            int recordId = SqlHelper.InsertWithReturnId(parameter, connectionString);
            return recordId;
        }

        public IList<T> GetAll(string connectionString)  
        {
            return SqlHelper.GetAll<T>(connectionString);
        }

        public IEnumerable<T> GetAllWithProcedure(string spName, string connectionString,bool buffer = false)
        {
            var user = SqlHelper.QuerySP<T>(spName, null, null, null, buffer, 0, connectionString);
            return user;
        }

        //public T Update(T user, string connectionString)
        //{
        //    throw new NotImplementedException();
        //}

        public T Find(string spName, object Id, string KeyName, string connectionString)
        {
            var type = Id.GetType();
            var p = new DynamicParameters();
            p.Add(string.Format("@{0}",KeyName), Id, GetDbType(type), null, null);
           
            var rec = SqlHelper.QuerySP<T>(spName, p, null, null, false, 0, connectionString);
            return (T)rec.FirstOrDefault();
        }

        public List<T> LoadViaStoreProc2(string spName, DynamicParameters dp,bool buffer, string connectionString)
        {
            var rec = SqlHelper.QuerySP<T>(spName, dp, null, null, buffer, 0, connectionString);
            return rec.ToList();
        }

        DbType GetDbType(Type type)
        {
            if (type.Name == "Int32")
            {
                return DbType.Int32;
            }
            else if (type.Name == "Int64")
            {
                return DbType.Int64;
            }
            else if (type.Name == "String")
            {
                return DbType.String;
            }
            else if (type.Name == "Boolean")
            {
                return DbType.Boolean;
            }

            return DbType.Int32;
        }

        public List<T> LoadViaStoreProc(string spName, DynamicParameters dp, string connectionString)
        {
           
            var rec = SqlHelper.QuerySP<T>(spName, dp, null, null, false, 0, connectionString);
            return rec.ToList();
        }

        //public T Find(decimal Id, string connectionString)
        //{
        //    PredicateGroup pg = new PredicateGroup { Predicates = new 
        //    {pg.Predicates.Add(Predicates.Field<T>(f=> f.))
        //    }
        // //   pre df =
        //    //var d = SqlHelper.Find<T>(userId, connectionString);
        //    throw new NotImplementedException();
        //}
    }
}
