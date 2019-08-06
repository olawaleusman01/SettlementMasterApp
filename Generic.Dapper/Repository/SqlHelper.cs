using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DapperExtensions;
using Dapper;
using System.Data;

namespace Generic.Dapper.Repository
{
    public static class SqlHelper 
    {
        public static string Connection
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            }
        }
        public static  bool Insert<T>(T parameter, string connectionString) where T : class
        {
            using (var sqlConnection = new SqlConnection(connectionString == null ? Connection : connectionString))
            {
                sqlConnection.Open();
                sqlConnection.Insert(parameter);
                sqlConnection.Close();
                return true;
            }
        }

        public static int InsertWithReturnId<T>(T parameter, string connectionString) where T : class
        {
            using (var sqlConnection = new SqlConnection(connectionString == null ? Connection : connectionString))
            {
                sqlConnection.Open();
                var recordId = sqlConnection.Insert(parameter);
                sqlConnection.Close();
                return recordId;
            }
        }

        public static bool Update<T>(T parameter, string connectionString) where T : class
        {
            using (var sqlConnection = new SqlConnection(connectionString == null ? Connection : connectionString))
            {
                sqlConnection.Open();
                sqlConnection.Update(parameter);
                sqlConnection.Close();
                return true;
            }
        }

        public static IList<T> GetAll<T>(string connectionString) where T : class
        {
            using (var sqlConnection = new SqlConnection(connectionString == null ? Connection : connectionString))
            {
                sqlConnection.Open();
                var result = sqlConnection.GetList<T>();
                sqlConnection.Close();
                return result.ToList();
            }
        }

        public static T Find<T>(PredicateGroup predicate, string connectionString) where T : class
        {
            using (var sqlConnection = new SqlConnection(connectionString == null ? Connection : connectionString))
            {
                sqlConnection.Open();
                var result = sqlConnection.GetList<T>(predicate).FirstOrDefault();
                sqlConnection.Close();
                return result;
            }
        }

        public static bool Delete<T>(PredicateGroup predicate, string connectionString) where T : class
        {
            using (var sqlConnection = new SqlConnection(connectionString == null ? Connection : connectionString))
            {
                sqlConnection.Open();
                sqlConnection.Delete<T>(predicate);
                sqlConnection.Close();
                return true;
            }
        }

        public static IEnumerable<T> QuerySP<T>(string storedProcedure, dynamic param = null,
            dynamic outParam = null, SqlTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, string connectionString = null) where T : class
        {
            SqlConnection connection = new SqlConnection(connectionString == null ? Connection : connectionString);
            connection.Open();
            var output = connection.Query<T>(storedProcedure, param: (object)param, transaction: transaction, buffered: buffered, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
            return output;
        }

        //public static IEnumerable<T> LoadMultiple<T>(string storedProcedure, dynamic param = null,
        //   dynamic outParam = null, SqlTransaction transaction = null,
        //   bool buffered = true, int? commandTimeout = null, string connectionString = null) where T : class
        //{
        //    SqlConnection connection = new SqlConnection(connectionString == null ? Connection : connectionString);
        //    connection.Open();
        //    var output = connection.QueryMultiple(storedProcedure, param: (object)param, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
        //    return output;
        //}

   
        private static void CombineParameters(ref dynamic param, dynamic outParam = null)
        {
            if (outParam != null)
            {
                if (param != null)
                {
                    param = new DynamicParameters(param);
                    ((DynamicParameters)param).AddDynamicParams(outParam);
                }
                else
                {
                    param = outParam;
                }
            }
        }

        private static int ConnectionTimeout { get; set; }

        private static int GetTimeout(int? commandTimeout = null)
        {
            if (commandTimeout.HasValue)
                return commandTimeout.Value;

            return ConnectionTimeout;
        }
    }
}
