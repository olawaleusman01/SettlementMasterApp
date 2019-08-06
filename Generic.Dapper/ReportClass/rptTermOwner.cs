
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
          class rptTermOwner
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

            ws.Cells["A4"].Value = "TERMINAL OWNER REPORT FOR " + CARDSCHEMDESC;
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

                    //if (dr.HasRows)
                    //{
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

                //}


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            ws.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
            ws.Cells[2, 5, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[2, 44, row + 1, 54].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 57, row + 1, 58].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[row + 1, 11, row + 1, 11].Formula = string.Format("=SUM(K{0}:K{1})", startRow, row);
            ws.Cells[row + 1, 11, row + 1,11].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 12, row + 1, 12].Formula = string.Format("=SUM(L{0}:L{1})", startRow, row);
            ws.Cells[row + 1, 12, row + 1, 12].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 13, row + 1, 13].Formula = string.Format("=SUM(M{0}:M{1})", startRow, row);
            ws.Cells[row + 1, 13, row + 1, 13].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 14, row + 1, 14].Formula = string.Format("=SUM(N{0}:N{1})", startRow, row);
            ws.Cells[row + 1, 14, row + 1, 14].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 15, row + 1, 15].Formula = string.Format("=SUM(O{0}:O{1})", startRow, row);
            ws.Cells[row + 1, 15, row + 1, 15].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 11].Formula = "=SUM(K" + startRow + 1 + ":K" + row + ")";
            ////ws.Cells[row + 1, 11].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 12].Formula = "=SUM(L" + startRow + 1 + ":L" + row + ")";
            ////ws.Cells[row + 1, 12].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 13].Formula = "=SUM(M" + startRow + 1 + ":M" + row + ")";
            ////ws.Cells[row + 1, 13].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 14].Formula = "=SUM(N" + startRow + 1 + ":N" + row + ")";
            ////ws.Cells[row + 1, 14].Style.Numberformat.Format = "#,##0.00";

            ////ws.Cells[row + 1, 15].Formula = "=SUM(O" + startRow + 1 + ":O" + row + ")";
            ////ws.Cells[row + 1, 15].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);

            ws.Select("C8");

            return ws;


        }

        public static ExcelWorksheet generateReportSUMM(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string DOMINT, string reportClass, string sett, int startRow)
        {


            //ExcelRange cols = ws.Cells["A:XFD"];
            //cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //cols.Style.Fill.BackgroundColor.SetColor(Color.White); ;

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

            ws.Cells["A4"].Value = "TERMINAL OWNER REPORT FOR " + CARDSCHEMDESC;
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
            if (columnIDcnt > 0)
            {

                ws.Cells[2, 2, row + 1, 4].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[row + 1, 1].Value = "TOTAL";

                ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);
                ////ws.Cells[row + 1, 2, row + 1, 11].Formula = "SUM(K" + startRow+1 + ":K" + row + ")";
                ////ws.Cells[row + 1, 2, row + 1, 11].Style.Numberformat.Format = "#,##0.00";

                ////ws.Cells[row + 1, 2, row + 1, 12].Formula = "SUM(L" + startRow + 1 + ":L" + row + ")";
                ////ws.Cells[row + 1, 2, row + 1, 12].Style.Numberformat.Format = "#,##0.00";

                ////ws.Cells[row + 1, 2, row + 1, 13].Formula = "SUM(M" + startRow + 1 + ":M" + row + ")";
                ////ws.Cells[row + 1, 2, row + 1, 13].Style.Numberformat.Format = "#,##0.00";

                ////ws.Cells[row + 1, 2, row + 1, 14].Formula = "SUM(N" + startRow + 1 + ":N" + row + ")";
                ////ws.Cells[row + 1, 2, row + 1, 14].Style.Numberformat.Format = "#,##0.00";

                ////ws.Cells[row + 1, 2, row + 1, 15].Formula = "SUM(O" + startRow + 1 + ":O" + row + ")";
                ////ws.Cells[row + 1, 2, row + 1, 15].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);

                ws.Select("C8");
            }

            return ws;


        }


        public static string GenReport1(string reportFor, string ID, DateTime reportDate, string reportPATH, string TERMINALOWNERPath, string logopath, string reportFolder, string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\TERMINAL OWNER Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + "_TERMINAL OWNER.xlsx";


            string oTERMINALOWNERPath = System.IO.Path.Combine(TERMINALOWNERPath, "TERMINAL OWNER");

            if (!Directory.Exists(oTERMINALOWNERPath))
            {
                System.IO.Directory.CreateDirectory(oTERMINALOWNERPath);
            }

            string oTERMINALOWNERPath2 = System.IO.Path.Combine(oTERMINALOWNERPath, reportFolder);

            if (!Directory.Exists(oTERMINALOWNERPath2))
            {
                System.IO.Directory.CreateDirectory(oTERMINALOWNERPath2);
            }
            string reportTERMINALOWNERPath = oTERMINALOWNERPath2 + "\\" + reportname;

            try
            {
                if (System.IO.File.Exists(reportTERMINALOWNERPath))
                {

                    System.IO.File.Delete(reportTERMINALOWNERPath);

                }
            }
            catch
            {

            }



            FileInfo newFile = new FileInfo(reportTERMINALOWNERPath);
            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("VISA DR");
                    ws = generateReport(ws, logopath, reportFor, ID, "VISA", "TERW_DR", reportFolderType, "P", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {

                    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                    ws2 = generateReport(ws2, logopath, reportFor, ID, "VISA", "TERW_CR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("MASTERCARD DR");
                    ws3 = generateReport(ws3, logopath, reportFor, ID, "MAST", "TERW_DR", reportFolderType, "P", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("MASTERCARD CR");
                    ws4 = generateReport(ws4, logopath, reportFor, ID, "MAST", "TERW_CR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4A = package.Workbook.Worksheets.Add("VERVE CARD DR");
                    ws4A = generateReport(ws4A, logopath, reportFor, ID, "VERV", "TERW_DR", reportFolderType, "P", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4B = package.Workbook.Worksheets.Add("VERV CARD CR");
                    ws4B = generateReport(ws4B, logopath, reportFor, ID, "VERV", "TERW_CR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }





                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                    ws5 = generateReport(ws5, logopath, reportFor, ID, "PAYA", "TERW_DR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                    ws6 = generateReport(ws6, logopath, reportFor, ID, "PAYA", "TERW_CR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }



                try
                {
                    var ws7 = package.Workbook.Worksheets.Add("CHINA UNION PAY DR");
                    ws7 = generateReport(ws7, logopath, reportFor, ID, "CUPI", "TERW_DR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("CHINA UNION PAY CR");
                    ws8 = generateReport(ws8, logopath, reportFor, ID, "CUPI", "TERW_CR", reportFolderType, "P", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = generateReportSUMM(ws9, logopath, reportFor, ID, null, "TERW_SUMM", reportFolderType, "P", sett, 8);

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