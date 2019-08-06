
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OfficeOpenXml;
using System.IO;
using System.Data.SqlClient;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Drawing;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace UPPosMaster.Dapper.ReportClass
{
    class rptIssuer
    {
        private static string oradb = System.Configuration.ConfigurationManager.AppSettings["TestDB"].ToString();
        
        public static ExcelWorksheet generateReport(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string DOMINT, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White); ;

            System.Drawing.Image myImage = System.Drawing.Image.FromFile(img);

            var pic = ws.Drawings.AddPicture("LOGO", myImage);

            // Row, RowoffsetPixel, Column, ColumnOffSetPixel
            pic.SetPosition(0, 0, 0, 0);

            //Insert a row at the top. Note that the formula-addresses are shifted down
            //ws.InsertRow(startRow-1, 1);
            if (cardScheme == null)
            {
                cardScheme = String.Empty;
            }

            //Write the headers and style them
            ws.Cells["A2"].Value = "Unified Payment Services Ltd";
            ws.Cells["A2"].Style.Font.Size = 18;
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A2:M2"].Merge = true;
            ws.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            String CARDSCHEMDESC = String.Empty;
            if (cardScheme.ToUpper() == "PAYA")
            {
                CARDSCHEMDESC = "PAY ATTITUDE";
            }
            else if (cardScheme.ToUpper() == "MAST")
            {
                CARDSCHEMDESC = "MASTER CARD";
            }
            else if (cardScheme.ToUpper() == "VISA")
            {
                CARDSCHEMDESC = "VISA CARD";
            }
            else if (cardScheme.ToUpper() == "CUPI")
            {
                CARDSCHEMDESC = "CHINA UNION PAY";
            }
            else {
                CARDSCHEMDESC = cardScheme.ToUpper();
            }

            ws.Cells["A4"].Value = "ISSUER REPORT FOR " + CARDSCHEMDESC;
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A6:H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


            ws.View.FreezePanes(8, 1);

            int columnIDcnt = 0;

            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = string.Empty;
                if (DOMINT == "DOM")
                {
                    qry = "RPT_SETTLEMETDETAIL";
                }
                else
                {
                    qry = "RPT_SETTLEMETDETAIL_INT";
                }

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = Standby_connection;
                // var dr = default(OracleDataReader);
                if (Standby_connection == null)
                {
                    Standby_connection = new OracleConnection(oradb);
                }
                if (Standby_connection.State != ConnectionState.Open)
                {
                    Standby_connection.Open();
                }
                cmd.Connection = Standby_connection;
                cmd.CommandText = qry;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = ID;
                cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = cardScheme;
                cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportType;
                cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportClass;
                cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
                cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));


                using (var dr = cmd.ExecuteReader())
                {

                    if (dr.HasRows)
                    {
                        int columnID = 1;
                        var fieldcount = dr.FieldCount;
                        DataTable dtSchema = dr.GetSchemaTable();

                        foreach (DataRow drow in dtSchema.Rows)
                        {
                            string columnName = Convert.ToString(drow["ColumnName"]);

                            ws.SetValue(startRow - 1, columnID, columnName);
                            columnID += 1;
                        }
                        columnIDcnt = columnID;
                        while (dr.Read())
                        {


                            for (int i = 0; i < fieldcount; i++)
                            {
                                ws.SetValue(row, i + 1, dr[i]);

                            }

                            row++;
                           
                        }

                    }
                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }



            ws.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
            ws.Cells[2, 5, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[2, 44, row + 1, 48].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 49, row + 1, 50].Style.Numberformat.Format = "#,##0";
            ws.Cells[2, 51, row + 1, 54].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 57, row + 1, 58].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 1].Value = "TOTAL";
            ////ws.Cells[row + 1, 43, row + 1, 48].Formula = string.Format("=SUM(K{0}:K{1})", startRow, row);
            ////ws.Cells[row + 1, 43, row + 1, 48].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 51, row + 1, 57].Formula = string.Format("=SUM(AX{0}:AX{1})", startRow, row);
            ////ws.Cells[row + 1, 51, row + 1, 57].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 65, row + 1, 80].Formula = string.Format("=SUM(BL{0}:BL{1})", startRow, row);
            ////ws.Cells[row + 1, 65, row + 1, 80].Style.Numberformat.Format = "#,##0.00";


            ////ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);

            ////ws.Select("C8");

            ////return ws;

            ws.Cells[startRow, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
            ws.Cells[startRow, 5, row + 1, 7].Style.Numberformat.Format = "dd-mm-yyyy";
            // ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[startRow, 32, row + 1, 32].Style.Numberformat.Format = "#,##0.00";
              ws.Cells[startRow, 35, row + 1, 37].Style.Numberformat.Format = "#,##0.00";
              // ws.Cells[2, 57, row + 1, 58].Style.Numberformat.Format = "#,##0.00";
            // ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";


            //ws.Cells[row + 1, 1].Value = "TOTAL";
            //ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

            //ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row, 1].Value = "TOTAL";
            ws.Cells[row, 32, row, 32].Formula = string.Format("=SUM(AF{0}:AF{1})", startRow, row-1);
            ws.Cells[row, 35, row, 37].Formula = string.Format("=SUM(AI{0}:AI{1})", startRow, row-1);
            ws.Cells[startRow, 1, row, columnIDcnt].AutoFitColumns(20);

            ws.Select("C8");

            return ws;


        }

        public static ExcelWorksheet generateReportSUMM(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string DOMINT, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White); ;

            System.Drawing.Image myImage = System.Drawing.Image.FromFile(img);

            var pic = ws.Drawings.AddPicture("LOGO", myImage);

            // Row, RowoffsetPixel, Column, ColumnOffSetPixel
            pic.SetPosition(0, 0, 0, 0);

            //Insert a row at the top. Note that the formula-addresses are shifted down
            //ws.InsertRow(startRow-1, 1);
            if (cardScheme == null)
            {
                cardScheme = String.Empty;
            }

            //Write the headers and style them
            ws.Cells["A2"].Value = "Unified Payment Services Ltd";
            ws.Cells["A2"].Style.Font.Size = 18;
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A2:M2"].Merge = true;
            ws.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            String CARDSCHEMDESC = String.Empty;
            if (cardScheme.ToUpper() == "PAYA")
            {
                CARDSCHEMDESC = "PAY ATTITUDE";
            }
            else if (cardScheme.ToUpper() == "MAST")
            {
                CARDSCHEMDESC = "MASTER CARD";
            }
            else if (cardScheme.ToUpper() == "VISA")
            {
                CARDSCHEMDESC = "VISA CARD";
            }
            else if (cardScheme.ToUpper() == "CUPI")
            {
                CARDSCHEMDESC = "CHINA UNION PAY";
            }
            else {
                CARDSCHEMDESC = cardScheme.ToUpper();
            }

            ws.Cells["A4"].Value = "ISSUER REPORT SUMMARY ";  
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A6:H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;


            ws.View.FreezePanes(8, 1);

            int columnIDcnt = 0;

            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = string.Empty;
                if (DOMINT == "DOM")
                {
                    qry = "RPT_SETTLEMETDETAIL";
                }
                else
                {
                    qry = "RPT_SETTLEMETDETAIL_INT";
                }

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = Standby_connection;
                // var dr = default(OracleDataReader);
                if (Standby_connection == null)
                {
                    Standby_connection = new OracleConnection(oradb);
                }
                if (Standby_connection.State != ConnectionState.Open)
                {
                    Standby_connection.Open();
                }
                cmd.Connection = Standby_connection;
                cmd.CommandText = qry;

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = ID;
                cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = cardScheme;
                cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportType;
                cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportClass;
                cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
                cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));


                using (var dr = cmd.ExecuteReader())
                {

                    if (dr.HasRows)
                    {
                        int columnID = 1;
                        var fieldcount = dr.FieldCount;
                        DataTable dtSchema = dr.GetSchemaTable();

                        foreach (DataRow drow in dtSchema.Rows)
                        {
                            string columnName = Convert.ToString(drow["ColumnName"]);

                            ws.SetValue(startRow - 1, columnID, columnName);
                            columnID += 1;
                        }
                        columnIDcnt = columnID;
                        while (dr.Read())
                        {


                            for (int i = 0; i < fieldcount; i++)
                            {
                                ws.SetValue(row, i + 1, dr[i]);

                            }

                            row++;
                            
                        }

                    }
                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            ws.Cells[2, 2, row + 1, 4].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[row + 1, 1].Style.Font.Bold = true;
            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";




            ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);

            ws.Cells[row + 3, 1].Value = "NOTE:";

            ws.Cells[row + 3, 2].Value = "ISSUER OBLIGATION INCLUDES PROCESSING FEE WHERE APPLICABLE:";
            


            ws.Select("C8");

            return ws;


        }
        
        public static string GenReport1(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath, string reportFolder, string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + "_ISSUER.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "ISSUER");

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, reportFolder);

            if (!Directory.Exists(oISSUERPath2))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath2);
            }
            string reportISSUERPath = oISSUERPath2 + "\\" + reportname;

            try
            {
                if (System.IO.File.Exists(reportISSUERPath))
                {

                    System.IO.File.Delete(reportISSUERPath);

                }
            }
            catch
            {

            }


            FileInfo newFile = new FileInfo(reportISSUERPath);
            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("VISA DR");
                    ws = generateReport(ws, logopath, reportFor, ID, "VISA", "ISSR_DR", reportFolderType, "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {

                    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                    ws2 = generateReport(ws2, logopath, reportFor, ID, "VISA", "ISSR_CR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("MASTER CARD DR");
                    ws3 = generateReport(ws3, logopath, reportFor, ID, "MAST", "ISSR_DR", reportFolderType, "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("MASTER CARD CR");
                    ws4 = generateReport(ws4, logopath, reportFor, ID, "MAST", "ISSR_CR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws3A = package.Workbook.Worksheets.Add("VERVE CARD DR");
                    ws3A = generateReport(ws3A, logopath, reportFor, ID, "VERV", "ISSR_DR", reportFolderType, "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4A = package.Workbook.Worksheets.Add("VERVE CARD CR");
                    ws4A = generateReport(ws4A, logopath, reportFor, ID, "VERV", "ISSR_CR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                    ws5 = generateReport(ws5, logopath, reportFor, ID, "PAYA", "ISSR_DR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                    ws6 = generateReport(ws6, logopath, reportFor, ID, "PAYA", "ISSR_CR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }



                try
                {
                    var ws7 = package.Workbook.Worksheets.Add("CHINA UNION PAY DR");
                    ws7 = generateReport(ws7, logopath, reportFor, ID, "CUPI", "ISSR_DR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("CHINA UNION PAY CR");
                    ws8 = generateReport(ws8, logopath, reportFor, ID, "CUPI", "ISSR_CR", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = generateReportSUMM(ws9, logopath, reportFor, ID, null, "ISSR_SUMM", reportFolderType, "B", sett, 8);

                }
                catch (Exception ex)
                {

                }






                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }

            return newFile.FullName;

        }

    

    }
}