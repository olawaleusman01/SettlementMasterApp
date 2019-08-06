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
    public class rptSett
    {
        private static string sourceDb = ConfigurationManager.AppSettings["DEST_DB"].ToString();

        public static string GenSettlementAll(string searchId,string reportType,string reportClass, DateTime setDate, string reportPATH, string filePath, string logopath, string reportFolder, string reportFolderType)
        {
           // LogFunction2.WriteMaintenaceLogToFile("IN........GenSettlementAll");
            string sett = setDate.ToString("dd-MMM-yyyy");


            string reportname = "SETTLEMENT_ALL" + ".xlsx";



            string _filePath = Path.Combine(filePath, reportFolder);

            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

           
            string reportPath = _filePath + "\\" + reportname;


            try
            {
                if (File.Exists(reportPath))
                {
                    File.Delete(reportPath);
                }
            }
            catch
            {

            }

            FileInfo newFile = new FileInfo(reportPath);

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                DataTable dtMain = null;

                try
                {
                    dsLoadclass dv = new dsLoadclass();
                    //string searchID,string CardScheme,string reportType,string reportClass, string subreporttype, string sett, string ReportPath, string logopath)
                          var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    dtMain = dv.generateDS("","","ALL","U","",sett,0);

                    //ExcelRange cols = wsA.Cells["A:XFD"];
                    //wsA.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    var range = wsA.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA.Row(11).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);


                    //wsA.Tables.Add(range, "data");
                    //Write the headers and style them
                    wsA.Cells["A2"].Value = "Xpress Payment Services Ltd";
                    wsA.Cells["A2"].Style.Font.Size = 18;
                    wsA.Cells["A2"].Style.Font.Bold = true;
                    wsA.Cells["A2:M2"].Merge = true;
                    wsA.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA.Cells["A3"].Value = "SETTLEMENT DETAIL REPORT ";
                    wsA.Cells["A3"].Style.Font.Size = 14;
                    wsA.Cells["A3"].Style.Font.Bold = true;
                    wsA.Cells["A3:M3"].Merge = true;
                    wsA.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    //wsA.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA.Cells["A5"].Style.Font.Size = 10;
                    wsA.Cells["A5:F5"].Merge = true;
                    //wsA.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA.Cells["A6"].Style.Font.Size = 10;
                    wsA.Cells["A6:F6"].Merge = true;

                    //wsA.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA.Cells["A7"].Style.Font.Size = 10;
                    wsA.Cells["A7"].Style.Font.Bold = true;

                    wsA.View.ShowGridLines = true;
                    //wsA.Cells["A:D"].Style.Numberformat.Format = null;
                    //wsA.Cells["B:B"].Style.Numberformat.Format = "0.00";
                    //wsA.Cells[1, 1].Value = "AA";
                    //wsA.Cells[1, 2].Value = "BB";
                    //wsA.Cells[1, 3].Value = "CC";
                    //wsA.Cells[1, 4].Value = "DD";

                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA.Dimension.End.Row + 1;
                        //wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                        wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy";
                        wsA.Cells[2, 5, row + 1, 5].Style.Numberformat.Format = "dd-mm-yyyy";
                        //wsA.Cells[2, 21, row + 1, 21].Style.Numberformat.Format = "#,##0.00";
                        //wsA.Cells[2, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";


                        //wsA.Cells[row + 1, 1].Value = "TOTAL";
                        //wsA.Cells[row + 1, 21, row + 1, 21].Formula = string.Format("=SUM(U{0}:U{1})", 12, row - 1);

                        //wsA.Cells[row + 1, 23, row + 1, 26].Formula = string.Format("=SUM(W{0}:W{1})", 12, row - 1);
                        //wsA.Cells[row + 1, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";

                    }

                    wsA.View.FreezePanes(12, 1);
                    //var tbl1 = wsA.Tables[0];
                    if (range != null)
                    {
                        range.AutoFitColumns();
                    }
                    // tbl1.ShowFilter = false;
                    //wsA.Cells[wsA.Dimension.Address].AutoFilter = true;


                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1} --", "SETT_GEN", sett) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();

                //try
                //{

                //    dtMain = generateDS("GEN", "U", sett, reportPath, logopath,null);
                //    var wsA2 = package.Workbook.Worksheets.Add("SETTLEMENT REPORT");
                //    //ExcelRange cols = wsA2.Cells["A:XFD"];
                //    //wsA2.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    //wsA2.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                //    var range2 = wsA2.Cells["A11"].LoadFromDataTable(dtMain, true);

                //    wsA2.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    wsA2.Row(11).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);


                //    //wsA2.Tables.Add(range2, "data2");



                //    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    
                //    var pic = wsA2.Drawings.AddPicture("LOGO", myImage);
                //    pic.SetPosition(0, 0, 0, 0);

                //    //Write the headers and style them
                //    wsA2.Cells["A2"].Value = "Xpress Payment Services Ltd";
                //    wsA2.Cells["A2"].Style.Font.Size = 18;
                //    wsA2.Cells["A2"].Style.Font.Bold = true;
                //    wsA2.Cells["A2:M2"].Merge = true;
                //    wsA2.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                //    wsA2.Cells["A3"].Value = "SETTLEMENT DETAIL REPORT ";
                //    wsA2.Cells["A3"].Style.Font.Size = 14;
                //    wsA2.Cells["A3"].Style.Font.Bold = true;
                //    wsA2.Cells["A3:M3"].Merge = true;
                //    wsA2.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                //    wsA2.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                //    //wsA2.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                //    wsA2.Cells["A5"].Style.Font.Size = 10;
                //    wsA2.Cells["A5:F5"].Merge = true;
                //    //wsA2.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                //    wsA2.Cells["A6"].Style.Font.Size = 10;
                //    wsA2.Cells["A6:F6"].Merge = true;

                //   // wsA2.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                //    wsA2.Cells["A7"].Style.Font.Size = 10;
                //    wsA2.Cells["A7"].Style.Font.Bold = true;

                //    if (dtMain != null && dtMain.Rows.Count > 1)
                //    {
                //        //int row = wsA2.Dimension.End.Row + 1;

                //        //wsA2.Cells[row + 1, 1].Value = "TOTAL";
                //        //wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                //        //wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Style.Numberformat.Format = "#,##0.00";

                //    }
                //    wsA2.View.FreezePanes(12, 1);
                //    // var tbl2 = wsA2.Tables[0];
                //    if (range2 != null)
                //    {
                //        range2.AutoFitColumns();
                //    }
                //    // tbl2.ShowFilter = false;
                //    //wsA2.Cells[wsA2.Dimension.Address].AutoFilter = true;

                //}
                //catch (Exception ex)
                //{
                //    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1} --", "SETT_ALL", sett) + "{0}" + ex.Message + ex.StackTrace);

                //}
                //dtMain.Dispose();
                

                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }

            return newFile.FullName;
        }
        
        //////public static DataTable generateDS(string searchID,string CardScheme,string reportType,string reportClass, string subreporttype, string sett, string ReportPath, string logopath,string Channel)
        //////{

        //////    DataSet ds = new DataSet();
        //////    DataTable dt = new DataTable();
        //////    string qry2 = string.Empty;
        //////    //qry2 = "exec RPT_SETTLEMETDETAIL '" + searchID + "','" + CardScheme + "','" + reportType + "','" + reportClass + "','" + subreporttype + "','" + sett + "'";
        //////    qry2 = "RPT_SETTLEMETDETAIL";
        //////    try
        //////    {
        //////        using (var con = new SqlConnection(sourceDb))
        //////        {
        //////            con.Open();
        //////            using (var cmd = new SqlCommand(qry2, con))
        //////            {

        //////                //cmd.Connection = con;
        //////                cmd.CommandType = CommandType.StoredProcedure;
        //////                // SqlParameterCollection sp = new SqlParameterCollection("");
        //////                AddParameter(cmd, "@P_searchID", searchID,typeof(string));
        //////                AddParameter(cmd, "@P_CardScheme", CardScheme, typeof(string));
        //////                AddParameter(cmd, "@P_reporttype", reportType, typeof(string));
        //////                AddParameter(cmd, "@P_reportClass", reportClass, typeof(string));
        //////                AddParameter(cmd, "@P_subreporttype", subreporttype, typeof(string));
        //////                AddParameter(cmd, "@P_SETT", sett, typeof(DateTime));
        //////                AddParameter(cmd, "@P_Channel", Channel, typeof(string));
        //////                //cmd.Parameters.AddWithValue("@P_searchID", string.IsNullOrEmpty(searchID) ? DBNull.Value : searchID).DbType = DbType.String;
        //////                //cmd.Parameters.AddWithValue("@P_CardScheme", CardScheme).DbType = DbType.String;
        //////                //cmd.Parameters.AddWithValue("@P_reporttype", reportType).DbType = DbType.String;
        //////                //cmd.Parameters.AddWithValue("@P_reportClass", reportClass).DbType = DbType.String;
        //////                //cmd.Parameters.AddWithValue("@P_subreporttype", subreporttype).DbType = DbType.String;
        //////                //cmd.Parameters.AddWithValue("@P_SETT", sett).DbType = DbType.DateTime;

        //////                cmd.CommandTimeout = 0;

        //////                using (SqlDataReader reader = cmd.ExecuteReader())
        //////                {

        //////                    dt.Load(reader, LoadOption.OverwriteChanges);
        //////                }

        //////            }
        //////            //////con.Open();
        //////            //////string qry = "RPT_SETTLEMETDETAIL";
        //////            //////var cmd = new SqlCommand(qry, con);
        //////            //////cmd.CommandType = CommandType.StoredProcedure;

        //////            ////////cmd.Parameters.AddWithValue("@P_searchID", searchID).DbType = DbType.String;
        //////            ////////cmd.Parameters.AddWithValue("@P_CardScheme", CardScheme).DbType = DbType.String;
        //////            ////////cmd.Parameters.AddWithValue("@P_reporttype", reportType).DbType = DbType.String;
        //////            ////////cmd.Parameters.AddWithValue("@P_reportClass", reportClass).DbType = DbType.String;
        //////            ////////cmd.Parameters.AddWithValue("@P_subreporttype", subreporttype).DbType = DbType.String;
        //////            ////////cmd.Parameters.AddWithValue("@P_SETT", sett).DbType = DbType.DateTime;
        //////            //////var p1 = new SqlParameter("@P_searchID", DbType.String);
        //////            //////p1.Value = DBNull.Value;
        //////            //////cmd.Parameters.Add(p1);
        //////            //////cmd.CommandTimeout = 0;
        //////            //////var reader = cmd.ExecuteReader();
        //////            //////dt.Load(reader, LoadOption.OverwriteChanges);



        //////        }

        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        //Console.WriteLine(ex.Message );

        //////    }


        //////    return dt;


        //////}

        public static void AddParameter(SqlCommand cmd, string parameterName, string value,Type type)
        {
            //SqlParameter parameters = new SqlParameter
            if (type == typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    cmd.Parameters.AddWithValue(parameterName, DBNull.Value).DbType = DbType.String;
                }
                else
                {
                    cmd.Parameters.AddWithValue(parameterName, value).DbType = DbType.String;
                }
            }
            else if (type == typeof(DateTime))
            {
                if (value == null)
                {
                    cmd.Parameters.AddWithValue(parameterName, DBNull.Value).DbType = DbType.DateTime;
                }
                else
                {
                    cmd.Parameters.AddWithValue(parameterName, value).DbType = DbType.DateTime;
                }
            }
            else
            {
                if (value == null)
                {
                    cmd.Parameters.AddWithValue(parameterName, DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue(parameterName, value);
                }
            }

        }

    }
}
