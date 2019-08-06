//using Dapper;
using Generic.Dapper.Data;
using Generic.Dapper.Repository;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.ReportClass
{
    public class dsLoadclass
    {
        private static string sourceDb = ConfigurationManager.AppSettings["DEST_DB"].ToString();

        public DataTable generateDS(string searchID, string CardScheme, string reportType, string reportClass, string subreporttype, string sett, int Channel)
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string qry2 = string.Empty;
            //qry2 = "exec RPT_SETTLEMETDETAIL '" + searchID + "','" + CardScheme + "','" + reportType + "','" + reportClass + "','" + subreporttype + "','" + sett + "'";
            qry2 = "RPT_SETTLEMETDETAIL";
            try
            {
                if (searchID== string.Empty){
                    searchID = null;
                }
                if (CardScheme == string.Empty)
                {
                    CardScheme = null;
                }
                if (reportType == string.Empty)
                {
                    reportType = null;
                }

                if (subreporttype == string.Empty)
                {
                    subreporttype = null;
                }

                
                using (var con = new SqlConnection(sourceDb))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(qry2, con))
                    {

                        //cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        ////AddParameter(cmd, "@P_searchID", searchID, typeof(string));
                        ////AddParameter(cmd, "@P_CardScheme", CardScheme, typeof(string));
                        ////AddParameter(cmd, "@P_reporttype", reportType, typeof(string));
                        ////AddParameter(cmd, "@P_reportClass", reportClass, typeof(string));
                        ////AddParameter(cmd, "@P_subreporttype", subreporttype, typeof(string));
                        ////AddParameter(cmd, "@P_SETT", sett, typeof(DateTime));
                        ////AddParameter(cmd, "@P_Channel", Channel, typeof(string));

                        cmd.Parameters.AddWithValue ("@P_searchID", searchID);
                        cmd.Parameters.AddWithValue("@P_CardScheme", CardScheme);
                        cmd.Parameters.AddWithValue("@P_reporttype", reportType);
                        cmd.Parameters.AddWithValue("@P_reportClass", reportClass);
                        cmd.Parameters.AddWithValue("@P_subreporttype", subreporttype);
                        cmd.Parameters.AddWithValue("@P_SETT", sett);
                        cmd.Parameters.AddWithValue("@P_Channel", Channel);

                        //cmd.Parameters.AddWithValue("@P_searchID", string.IsNullOrEmpty(searchID) ? DBNull.Value : searchID).DbType = DbType.String;
                        //cmd.Parameters.AddWithValue("@P_CardScheme", CardScheme).DbType = DbType.String;
                        //cmd.Parameters.AddWithValue("@P_reporttype", reportType).DbType = DbType.String;
                        //cmd.Parameters.AddWithValue("@P_reportClass", reportClass).DbType = DbType.String;
                        //cmd.Parameters.AddWithValue("@P_subreporttype", subreporttype).DbType = DbType.String;
                        //cmd.Parameters.AddWithValue("@P_SETT", sett).DbType = DbType.DateTime;

                        cmd.CommandTimeout = 0;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            dt.Load(reader, LoadOption.OverwriteChanges);
                        }

                    }
                  



                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            return dt;


        }


        public SqlDataReader generateDR(string searchID, string CardScheme, string reportType, string reportClass, string subreporttype, string sett, int Channel)
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string qry2 = string.Empty;
            SqlDataReader reader = null;
            //qry2 = "exec RPT_SETTLEMETDETAIL '" + searchID + "','" + CardScheme + "','" + reportType + "','" + reportClass + "','" + subreporttype + "','" + sett + "'";
            qry2 = "RPT_SETTLEMETDETAIL";

            SqlConnection connection = new SqlConnection(sourceDb);
            try
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(qry2, connection))
                {

                    cmd.Parameters.AddWithValue("@P_searchID", searchID);
                    cmd.Parameters.AddWithValue("@P_CardScheme", CardScheme);
                    cmd.Parameters.AddWithValue("@P_reporttype", reportType);
                    cmd.Parameters.AddWithValue("@P_reportClass", reportClass);
                    cmd.Parameters.AddWithValue("@P_subreporttype", subreporttype);
                    cmd.Parameters.AddWithValue("@P_SETT", sett);
                    cmd.Parameters.AddWithValue("@P_Channel", Channel);

                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }


            }
            catch (Exception ex)
            {
                // Close connection before rethrowing
                connection.Close();
                throw;
            }
       


        }

        ////  public static SqlDataReader ExecuteReader(String connectionString, String commandText,
        ////CommandType commandType, params SqlParameter[] parameters)
        ////  {
        ////      SqlConnection conn = new SqlConnection(connectionString);

        ////      using (SqlCommand cmd = new SqlCommand(commandText, conn))
        ////      {
        ////          cmd.CommandType = commandType;
        ////          cmd.Parameters.AddRange(parameters);

        ////          conn.Open();
        ////          // When using CommandBehavior.CloseConnection, the connection will be closed when the 
        ////          // IDataReader is closed.
        ////          SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        ////          return reader;
        ////      }
        ////  }
        public SqlDataReader GetREcordCount(string qry2)
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
          
            SqlDataReader reader = null;
            //qry2 = "exec RPT_SETTLEMETDETAIL '" + searchID + "','" + CardScheme + "','" + reportType + "','" + reportClass + "','" + subreporttype + "','" + sett + "'";
             
            
                ////using (var con = new SqlConnection(sourceDb))
                ////{
                ////    con.Open();

                ////    SqlCommand cmd = new SqlCommand(qry2, con);


                ////       cmd.CommandTimeout = 0;

                ////       reader = cmd.ExecuteReader();


                ////}


                SqlConnection connection = new SqlConnection(sourceDb);
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(qry2, connection))
                    {
                        return command.ExecuteReader(CommandBehavior.CloseConnection);
                    }


                }
            catch (Exception ex)
            {
                    // Close connection before rethrowing
                    connection.Close();
                    throw;
                }

             
        }
             
         

    }
}
