using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
//using Sybase.Data.AseClient;

namespace Generic.Dapper.Repository
{
   public class RepoBase
    {

       public DbConnection Connection(string conString)
       {
           //get
           //{
           if (string.IsNullOrEmpty(conString))
           {
              // return new SqlConnection(conString);
               return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
           }
           else
           {
               return new SqlConnection(conString);
           }
          // }
       }
        public DbConnection ConnectionSybase(string conString)
        {
            //get
            //{
            if (string.IsNullOrEmpty(conString))
            {
                //AseConnection theCons = new AseConnection(sSQLcon)
                // return new SqlConnection(conString);
                return null;   //return new AseConnection(ConfigurationManager.ConnectionStrings["sybconnection"].ConnectionString);
            }
            else
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings["MfbConnection"].ConnectionString);
            }
            // }
        }
        public DbConnection ConnectMfbConfig
       {
           get
           {
               return new SqlConnection(ConfigurationManager.ConnectionStrings["MfbConnection"].ConnectionString);
           }
       }

        public DbConnection OpenConnection(string conString)
        {
            var connection =  Connection(conString);
            connection.Open();
            
            return connection;
        }
        public DbConnection OpenSybaseConnection(string conString)
        {
            var connection =  ConnectionSybase(conString);
            connection.Open();

            return connection;
        }
        protected T Fetch<T>(Func<DbConnection, T> func, string conString)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection(conString))
            {
                
                return func(connection);
            }
        }
        protected async Task<T> FetchAsync<T>(Func<IDbConnection, Task<T>> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            //using (var connection = OpenConnection(conString))
            //{

            //    return func(connection);
            //}

            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open a connection to the database
                    return await func(connection); // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        protected T FetchMfB<T>(Func<DbConnection, T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection(""))
            {
                return func(connection);
            }
        }

        protected int Execute(Func<DbConnection, int> func, string conString)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection(conString))
            {
                return func(connection);
            }
        }

        protected int ExecuteTransaction(Func<DbConnection, IDbTransaction, int> func, string conString, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection(conString))
            using (var transaction = connection.BeginTransaction(isolationLevel))
            {
                var value = func(connection, transaction);

                transaction.Commit();

                return value;
            }
        }
    
    }
}
