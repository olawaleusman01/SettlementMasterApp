
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
    class rptCoAcquirer
    {
        private static string oradb = System.Configuration.ConfigurationManager.AppSettings["TestDB"].ToString();

        private static decimal GetExcelDecimalValueForDate(DateTime date)
        {
            DateTime start = new DateTime(1900, 1, 1);
            TimeSpan diff = date - start;
            return diff.Days + 2;
        }

        public ExcelPackage getSheet(string templatePath)
        {
            FileInfo template = new FileInfo(templatePath);
            ExcelPackage p = new ExcelPackage(template, true);
            ExcelWorksheet ws = p.Workbook.Worksheets[1]; //position of the worksheet
                                                          //ws.Name = bookName;
            p.Save();
            ExcelPackage pck = new ExcelPackage(new System.IO.MemoryStream(), p.Stream);
            return pck;
        }

        #region "NAIRA SETTLEMENT"
        public static ExcelWorksheet generateIssuer(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White);;

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
            else {
                CARDSCHEMDESC = cardScheme.ToUpper();
            }

            ws.Cells["A4"].Value = "ACQUIRER INCOME REPORT " + CARDSCHEMDESC;
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "SN";
            ws.Cells["B7"].Value = "CPD";
            ws.Cells["C7"].Value = "SETTLEMENTDATE";
            ws.Cells["D7"].Value = "RETAILER";
            ws.Cells["E7"].Value = "RETAILER ACCOUNT";
            ws.Cells["F7"].Value = "RETAILERID";
            ws.Cells["G7"].Value = "AMOUNTDUE";
            ws.Cells["H7"].Value = "SECTOR";

            ////Console.WriteLine("loading excel cell");

            ws.View.FreezePanes(8, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])
            if (reportType == "ACQR_EXCEP")
            {
                ws.Cells["I7"].Value = "SETTLEMENT ACCOUNTNO";
                ws.Cells["J7"].Value = "REASON";

                using (var rng = ws.Cells["A7:J7"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }
            else
            {

                using (var rng = ws.Cells["A7:H7"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }


            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = "RPT_SETTLEMETDETAIL";
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
                        while (dr.Read())
                        {
                            //////Console.WriteLine("reading excel cell");

                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                            ws.SetValue(row, 3, dr[2]);
                            ws.SetValue(row, 4, dr[3]);
                            ws.SetValue(row, 5, dr[4]);
                            ws.SetValue(row, 6, dr[5]);
                            ws.SetValue(row, 7, dr[6]);
                            ws.SetValue(row, 8, dr[7]);
                            if (reportType == "ACQR_EXCEP")
                            {
                                ws.SetValue(row, 9, dr[8]);
                                ws.SetValue(row, 10, dr[9]);
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

            ////ws.Cells[1, 5, row, 5].FormulaR1C1 = "RC[-4]+RC[-1]";

            //Add a sum at the end
            ws.Cells[row + 1, 7].Formula = string.Format("Sum({0})", new ExcelAddress(1, 7, row, 7).Address);

            ws.Cells[row + 1, 7].Style.Font.Bold = true;
            ws.Cells[row + 1, 7].Style.Numberformat.Format = "#,##0.00";



            //Format the date and numeric columns
            ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells[1, 7, row, 7].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[1, 2, row, 2].Style.Numberformat.Format = "YYYY-MM-DD";
            ws.Cells[1, 3, row, 3].Style.Numberformat.Format = "YYYY-MM-DD";


            ws.Cells[row, 1, row, 8].AutoFitColumns(20);


            ////ws.Cells[2, 3, row + 1, 4].Style.Locked = false;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
            //ws.Cells[1, 5, row + 2, 5].Style.Hidden = true;

            ws.Select("C8");

            return ws;


        }

        public static ExcelWorksheet generateIssuer3(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White);;

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

            ws.Cells["A4"].Value = "ACQUIRER SUMMARY ";
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "DESCRIPTION";
            ws.Cells["B7"].Value = "COUNT";
            ws.Cells["C7"].Value = "TOTAL DR (N)";
            ws.Cells["D7"].Value = "TOTAL CR (N)";
            ws.Cells["E7"].Value = "NET AMOUNT (N)";
            using (var rng = ws.Cells["A7:E7"])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.WrapText = true;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //rng.Style.Border = "2pt";
            }


            ws.View.FreezePanes(8, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])



            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = "RPT_SETTLEMETDETAIL";
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
                    while (dr.Read())
                    {
                        ws.SetValue(row, 1, dr[0]);
                        ws.SetValue(row, 2, dr[1]);
                        ws.SetValue(row, 3, dr[2]);
                        ws.SetValue(row, 4, dr[3]);

                        row++;
                    }

                    //}

                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {

            }

            ws.Cells[startRow, 5, row - 1, 5].FormulaR1C1 = "RC[-2]+RC[-1]";

            //Add a sum at the end
            ws.Cells[row + 1, 2].Formula = string.Format("Sum({0})", new ExcelAddress(1, 2, row, 2).Address);
            ws.Cells[row + 1, 3].Formula = string.Format("Sum({0})", new ExcelAddress(1, 3, row, 3).Address);
            ws.Cells[row + 1, 4].Formula = string.Format("Sum({0})", new ExcelAddress(1, 4, row, 4).Address);
            ws.Cells[row + 1, 5].Formula = string.Format("Sum({0})", new ExcelAddress(1, 5, row, 5).Address);
            ws.Cells[row + 1, 2].Style.Font.Bold = true;
            ws.Cells[row + 1, 2].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 3].Style.Font.Bold = true;
            ws.Cells[row + 1, 3].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 4].Style.Font.Bold = true;
            ws.Cells[row + 1, 4].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 5].Style.Font.Bold = true;
            ws.Cells[row + 1, 5].Style.Numberformat.Format = "#,##0.00";
            //Format the date and numeric columns

            ws.Cells[1, 2, row, 4].Style.Numberformat.Format = "#,##0.00";




            ws.Cells[row, 1, row, 5].AutoFitColumns(20);


            ////ws.Cells[2, 3, row + 1, 4].Style.Locked = false;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
            //ws.Cells[1, 5, row + 2, 5].Style.Hidden = true;

            ws.Select("A8");

            return ws;


        }

        public static ExcelWorksheet generateIssuer2(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White);;

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

            ws.Cells["A3"].Value = "MERCHANT TRANSACTION " + cardScheme.ToUpper();
            ws.Cells["A3"].Style.Font.Size = 14;
            ws.Cells["A3"].Style.Font.Bold = true;
            ws.Cells["A3:M3"].Merge = true;
            ws.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;



            ws.Cells["A10"].Value = "SN";
            ws.Cells["B10"].Value = "SETTLEMENTDATE";
            ws.Cells["C10"].Value = "TERMINAL ID";
            ws.Cells["D10"].Value = "TERMINAL LOCATION";
            ws.Cells["E10"].Value = "TRANSACTION TYPE";
            ws.Cells["F10"].Value = "INVOICE NUMBER";
            ws.Cells["G10"].Value = "TRANSACTION AMOUNT";
            ws.Cells["H10"].Value = "MSC";
            ws.Cells["I10"].Value = "MSC VALUE";
            ws.Cells["J10"].Value = "AMOUNT DUE";
            ws.Cells["K10"].Value = "CARDNUMBER (MASKED)";
            ws.Cells["L10"].Value = "TRANSACTION DATE";
            ws.Cells["M10"].Value = "APPROVAL CODE";
            ws.Cells["N10"].Value = "CARD TYPE";
            ws.Cells["O10"].Value = "TRANS. ID";

            ws.View.FreezePanes(11, 1);

            if (reportType == "ACQR_EXCEP")
            {
                ws.Cells["P10"].Value = "SETTLEMENT ACCOUNTNO";
                ws.Cells["Q10"].Value = "REASON";
                using (var rng = ws.Cells["A10:Q10"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }
            else
            {

                using (var rng = ws.Cells["A10:O10"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }



            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])


            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = "RPT_SETTLEMETDETAIL";
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

                cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = rName;
                cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = cardScheme;
                cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportType;
                cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportClass;
                cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
                cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));


                using (var dr = cmd.ExecuteReader())
                {

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                            ws.SetValue(row, 3, dr[2]);
                            ws.SetValue(row, 4, dr[3]);
                            ws.SetValue(row, 5, dr[4]);
                            ws.SetValue(row, 6, dr[5]);
                            ws.SetValue(row, 7, dr[6]);
                            ws.SetValue(row, 8, dr[7]);

                            ws.SetValue(row, 9, dr[8]);
                            ws.SetValue(row, 10, dr[9]);
                            ws.SetValue(row, 11, dr[10]);
                            ws.SetValue(row, 12, dr[11]);
                            ws.SetValue(row, 13, dr[12]);
                            ws.SetValue(row, 14, dr[13]);
                            ws.SetValue(row, 15, dr[14]);
                            if (reportType == "ACQR_EXCEP")
                            {
                                ws.SetValue(row, 16, dr[15]);
                                ws.SetValue(row, 17, dr[16]);
                            }


                            row++;
                        }

                    }
                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {

            }

            ////ws.Cells[1, 5, row, 5].FormulaR1C1 = "RC[-4]+RC[-1]";

            //Add a sum at the end
            ws.Cells[row + 1, 7].Formula = string.Format("Sum({0})", new ExcelAddress(1, 7, row, 7).Address);
            ws.Cells[row + 1, 9].Formula = string.Format("Sum({0})", new ExcelAddress(1, 9, row, 9).Address);
            ws.Cells[row + 1, 10].Formula = string.Format("Sum({0})", new ExcelAddress(1, 10, row, 10).Address);

            ws.Cells[row + 1, 7].Style.Font.Bold = true;
            ws.Cells[row + 1, 7].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 9].Style.Font.Bold = true;
            ws.Cells[row + 1, 9].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 10].Style.Font.Bold = true;
            ws.Cells[row + 1, 10].Style.Numberformat.Format = "#,##0.00";


            //Format the date and numeric columns
            ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells[1, 7, row, 10].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[1, 2, row, 2].Style.Numberformat.Format = "YYYY-MM-DD";
            ws.Cells[1, 12, row, 12].Style.Numberformat.Format = "YYYY-MM-DD";


            ws.Cells[row, 1, row, 8].AutoFitColumns(20);


            ////ws.Cells[2, 3, row + 1, 4].Style.Locked = false;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
            //ws.Cells[1, 5, row + 2, 5].Style.Hidden = true;

            ws.Select("C11");

            return ws;


        }


        #endregion

        #region "INTERNATIOAL"
        public static ExcelWorksheet generateIssuer_DOLLAR(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White);;

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
            else {
                CARDSCHEMDESC = cardScheme.ToUpper();
            }

            ws.Cells["A4"].Value = "ACQUIRER INCOME REPORT " + CARDSCHEMDESC;
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "SN";
            ws.Cells["B7"].Value = "CPD";
            ws.Cells["C7"].Value = "SETTLEMENTDATE";
            ws.Cells["D7"].Value = "RETAILER";
            ws.Cells["E7"].Value = "RETAILER ACCOUNT";
            ws.Cells["F7"].Value = "RETAILERID";
            ws.Cells["G7"].Value = "AMOUNTDUE";
            ws.Cells["H7"].Value = "SECTOR";

            ////Console.WriteLine("loading excel cell");

            ws.View.FreezePanes(8, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])
            if (reportType == "ACQR_EXCEP")
            {
                ws.Cells["I7"].Value = "SETTLEMENT ACCOUNTNO";
                ws.Cells["J7"].Value = "REASON";

                using (var rng = ws.Cells["A7:J7"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }
            else
            {

                using (var rng = ws.Cells["A7:H7"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }


            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = "RPT_SETTLEMETDETAIL_INT";
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
                        while (dr.Read())
                        {
                            //////Console.WriteLine("reading excel cell");

                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                            ws.SetValue(row, 3, dr[2]);
                            ws.SetValue(row, 4, dr[3]);
                            ws.SetValue(row, 5, dr[4]);
                            ws.SetValue(row, 6, dr[5]);
                            ws.SetValue(row, 7, dr[6]);
                            ws.SetValue(row, 8, dr[7]);
                            if (reportType == "ACQR_EXCEP")
                            {
                                ws.SetValue(row, 9, dr[8]);
                                ws.SetValue(row, 10, dr[9]);
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

            ////ws.Cells[1, 5, row, 5].FormulaR1C1 = "RC[-4]+RC[-1]";

            //Add a sum at the end
            ws.Cells[row + 1, 7].Formula = string.Format("Sum({0})", new ExcelAddress(1, 7, row, 7).Address);

            ws.Cells[row + 1, 7].Style.Font.Bold = true;
            ws.Cells[row + 1, 7].Style.Numberformat.Format = "#,##0.00";



            //Format the date and numeric columns
            ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells[1, 7, row, 7].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[1, 2, row, 2].Style.Numberformat.Format = "YYYY-MM-DD";
            ws.Cells[1, 3, row, 3].Style.Numberformat.Format = "YYYY-MM-DD";


            ws.Cells[row, 1, row, 8].AutoFitColumns(20);


            ////ws.Cells[2, 3, row + 1, 4].Style.Locked = false;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
            //ws.Cells[1, 5, row + 2, 5].Style.Hidden = true;

            ws.Select("C8");

            return ws;


        }

        public static ExcelWorksheet generateIssuer3_DOLLAR(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White);;

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

            ws.Cells["A4"].Value = "ACQUIRER SUMMARY ";
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "DESCRIPTION";
            ws.Cells["B7"].Value = "COUNT";
            ws.Cells["C7"].Value = "TOTAL DR (N)";
            ws.Cells["D7"].Value = "TOTAL CR (N)";
            ws.Cells["E7"].Value = "NET AMOUNT (N)";
            using (var rng = ws.Cells["A7:E7"])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.WrapText = true;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //rng.Style.Border = "2pt";
            }


            ws.View.FreezePanes(8, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])



            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = "RPT_SETTLEMETDETAIL_INT";
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
                    while (dr.Read())
                    {
                        ws.SetValue(row, 1, dr[0]);
                        ws.SetValue(row, 2, dr[1]);
                        ws.SetValue(row, 3, dr[2]);
                        ws.SetValue(row, 4, dr[3]);

                        row++;
                    }

                    //}

                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {

            }

            ws.Cells[startRow, 5, row - 1, 5].FormulaR1C1 = "RC[-2]+RC[-1]";

            //Add a sum at the end
            ws.Cells[row + 1, 2].Formula = string.Format("Sum({0})", new ExcelAddress(1, 2, row, 2).Address);
            ws.Cells[row + 1, 3].Formula = string.Format("Sum({0})", new ExcelAddress(1, 3, row, 3).Address);
            ws.Cells[row + 1, 4].Formula = string.Format("Sum({0})", new ExcelAddress(1, 4, row, 4).Address);
            ws.Cells[row + 1, 5].Formula = string.Format("Sum({0})", new ExcelAddress(1, 5, row, 5).Address);
            ws.Cells[row + 1, 2].Style.Font.Bold = true;
            ws.Cells[row + 1, 2].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 3].Style.Font.Bold = true;
            ws.Cells[row + 1, 3].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 4].Style.Font.Bold = true;
            ws.Cells[row + 1, 4].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 5].Style.Font.Bold = true;
            ws.Cells[row + 1, 5].Style.Numberformat.Format = "#,##0.00";
            //Format the date and numeric columns

            ws.Cells[1, 2, row, 4].Style.Numberformat.Format = "#,##0.00";




            ws.Cells[row, 1, row, 5].AutoFitColumns(20);


            ////ws.Cells[2, 3, row + 1, 4].Style.Locked = false;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
            //ws.Cells[1, 5, row + 2, 5].Style.Hidden = true;

            ws.Select("A8");

            return ws;


        }

        public static ExcelWorksheet generateIssuer2_DOLLAR(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
        {


            ExcelRange cols = ws.Cells["A:XFD"];
            cols.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cols.Style.Fill.BackgroundColor.SetColor(Color.White);;

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

            ws.Cells["A3"].Value = "MERCHANT TRANSACTION " + cardScheme.ToUpper();
            ws.Cells["A3"].Style.Font.Size = 14;
            ws.Cells["A3"].Style.Font.Bold = true;
            ws.Cells["A3:M3"].Merge = true;
            ws.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;



            ws.Cells["A10"].Value = "SN";
            ws.Cells["B10"].Value = "SETTLEMENTDATE";
            ws.Cells["C10"].Value = "TERMINAL ID";
            ws.Cells["D10"].Value = "TERMINAL LOCATION";
            ws.Cells["E10"].Value = "TRANSACTION TYPE";
            ws.Cells["F10"].Value = "INVOICE NUMBER";
            ws.Cells["G10"].Value = "TRANSACTION AMOUNT";
            ws.Cells["H10"].Value = "MSC";
            ws.Cells["I10"].Value = "MSC VALUE";
            ws.Cells["J10"].Value = "AMOUNT DUE";
            ws.Cells["K10"].Value = "CARDNUMBER (MASKED)";
            ws.Cells["L10"].Value = "TRANSACTION DATE";
            ws.Cells["M10"].Value = "APPROVAL CODE";
            ws.Cells["N10"].Value = "CARD TYPE";
            ws.Cells["O10"].Value = "TRANS. ID";

            ws.View.FreezePanes(11, 1);

            if (reportType == "ACQR_EXCEP")
            {
                ws.Cells["P10"].Value = "SETTLEMENT ACCOUNTNO";
                ws.Cells["Q10"].Value = "REASON";
                using (var rng = ws.Cells["A10:Q10"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }
            else
            {

                using (var rng = ws.Cells["A10:O10"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.WrapText = true;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //rng.Style.Border = "2pt";
                }

            }



            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])


            int row = startRow;
            try
            {
                OracleConnection Standby_connection = new OracleConnection(oradb);
                string qry = "RPT_SETTLEMETDETAIL_INT";
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

                cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = rName;
                cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = cardScheme;
                cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportType;
                cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportClass;
                cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
                cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));


                using (var dr = cmd.ExecuteReader())
                {

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                            ws.SetValue(row, 3, dr[2]);
                            ws.SetValue(row, 4, dr[3]);
                            ws.SetValue(row, 5, dr[4]);
                            ws.SetValue(row, 6, dr[5]);
                            ws.SetValue(row, 7, dr[6]);
                            ws.SetValue(row, 8, dr[7]);

                            ws.SetValue(row, 9, dr[8]);
                            ws.SetValue(row, 10, dr[9]);
                            ws.SetValue(row, 11, dr[10]);
                            ws.SetValue(row, 12, dr[11]);
                            ws.SetValue(row, 13, dr[12]);
                            ws.SetValue(row, 14, dr[13]);
                            ws.SetValue(row, 15, dr[14]);
                            if (reportType == "ACQR_EXCEP")
                            {
                                ws.SetValue(row, 16, dr[15]);
                                ws.SetValue(row, 17, dr[16]);
                            }


                            row++;
                        }

                    }
                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {

            }

            ////ws.Cells[1, 5, row, 5].FormulaR1C1 = "RC[-4]+RC[-1]";

            //Add a sum at the end
            ws.Cells[row + 1, 7].Formula = string.Format("Sum({0})", new ExcelAddress(1, 7, row, 7).Address);
            ws.Cells[row + 1, 9].Formula = string.Format("Sum({0})", new ExcelAddress(1, 9, row, 9).Address);
            ws.Cells[row + 1, 10].Formula = string.Format("Sum({0})", new ExcelAddress(1, 10, row, 10).Address);

            ws.Cells[row + 1, 7].Style.Font.Bold = true;
            ws.Cells[row + 1, 7].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row + 1, 9].Style.Font.Bold = true;
            ws.Cells[row + 1, 9].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 10].Style.Font.Bold = true;
            ws.Cells[row + 1, 10].Style.Numberformat.Format = "#,##0.00";


            //Format the date and numeric columns
            ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells[1, 7, row, 10].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[1, 2, row, 2].Style.Numberformat.Format = "YYYY-MM-DD";
            ws.Cells[1, 12, row, 12].Style.Numberformat.Format = "YYYY-MM-DD";


            ws.Cells[row, 1, row, 8].AutoFitColumns(20);


            ////ws.Cells[2, 3, row + 1, 4].Style.Locked = false;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ////ws.Cells[2, 3, row + 1, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
            //ws.Cells[1, 5, row + 2, 5].Style.Hidden = true;

            ws.Select("C11");

            return ws;


        }


        #endregion
        public static string GenReport1(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string outputdir, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + ".xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NAIRA SETTLEMENT");

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, reportFor);


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

            using (ExcelPackage package = new ExcelPackage())
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("VISA DR");
                    ws = generateIssuer(ws, logopath, reportFor, ID, "VISA", "ACQR_DR", "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {

                    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                    ws2 = generateIssuer(ws2, logopath, reportFor, ID, "VISA", "ACQR_CR", "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("MCVE DR");
                    ws3 = generateIssuer(ws3, logopath, reportFor, ID, "MAST", "ACQR_DR", "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("MCVE CR");
                    ws4 = generateIssuer(ws4, logopath, reportFor, ID, "MAST", "ACQR_CR", "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                    ws5 = generateIssuer(ws5, logopath, reportFor, ID, "PAYA", "ACQR_DR", "B", sett, 8);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                    ws6 = generateIssuer(ws6, logopath, reportFor, ID, "PAYA", "ACQR_CR", "B", sett, 8);
                }
                catch (Exception ex)
                {

                }



                ////try
                ////{
                ////    var ws7 = package.Workbook.Worksheets.Add("SUBSIDY");
                ////    ws7 = generateIssuer(ws7, logopath, reportFor, ID, null, "ACQR_SUBSY", "B", sett, 8);
                ////}
                ////catch (Exception ex)
                ////{

                ////}

                //////try
                //////{
                //////    var ws8 = package.Workbook.Worksheets.Add("EXCEPTION");
                //////    ws8 = generateIssuer(ws8, logopath, reportFor, ID, null, "ACQR_EXCEP", "B", sett, 8);
                //////}
                //////catch (Exception ex)
                //////{

                //////}

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = generateIssuer3(ws9, logopath, reportFor, ID, null, "ACQR_SUMM", "B", sett, 8);
                }
                catch (Exception ex)
                {

                }




                //var ws2 = package.Workbook.Worksheets.Add("ISSUER CR");
                //ws2 = generateIssuer(ws2,ID, cardScheme, "ISSR_CR", "B", sett, 11);

                //var ws3 = package.Workbook.Worksheets.Add("ISSUER CR");
                //ws3 = generateIssuer(ws2, ID, cardScheme, "ISSR_CR", "B", sett, 11);

                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
            }
            return newFile.FullName;

        }

        public static string GenReport2(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string outputdir, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + ".xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NAIRA SETTLEMENT");

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, reportFor);


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

            using (ExcelPackage package = new ExcelPackage())
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("VISA");
                    ws = generateIssuer2(ws, logopath, reportFor, ID, "VISA", "ACQ_MERCH", "B", sett, 11);

                    var ws2 = package.Workbook.Worksheets.Add("MAST");
                    ws2 = generateIssuer2(ws2, logopath, reportFor, ID, "MAST", "ACQ_MERCH", "B", sett, 11);

                    var ws3 = package.Workbook.Worksheets.Add("PAYA");
                    ws3 = generateIssuer2(ws3, logopath, reportFor, ID, "PAYA", "ACQ_MERCH", "B", sett, 11);

                }
                catch (Exception ex)
                {

                }


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
            }
            return newFile.FullName;

        }



        public static string GenReport1_DOLLAR(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string outputdir, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + ".xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "DOLLAR SETTLEMENT");

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, reportFor);

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

            using (ExcelPackage package = new ExcelPackage())
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("VISA DR");
                    ws = generateIssuer_DOLLAR(ws, logopath, reportFor, ID, "VISA", "ACQR_DR", "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {

                    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                    ws2 = generateIssuer_DOLLAR(ws2, logopath, reportFor, ID, "VISA", "ACQR_CR", "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("MCVE DR");
                    ws3 = generateIssuer_DOLLAR(ws3, logopath, reportFor, ID, "MAST", "ACQR_DR", "B", sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("MCVE CR");
                    ws4 = generateIssuer_DOLLAR(ws4, logopath, reportFor, ID, "MAST", "ACQR_CR", "B", sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                    ws5 = generateIssuer_DOLLAR(ws5, logopath, reportFor, ID, "PAYA", "ACQR_DR", "B", sett, 8);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                    ws6 = generateIssuer_DOLLAR(ws6, logopath, reportFor, ID, "PAYA", "ACQR_CR", "B", sett, 8);
                }
                catch (Exception ex)
                {

                }



                ////try
                ////{
                ////    var ws7 = package.Workbook.Worksheets.Add("SUBSIDY");
                ////    ws7 = generateIssuer(ws7, logopath, reportFor, ID, null, "ACQR_SUBSY", "B", sett, 8);
                ////}
                ////catch (Exception ex)
                ////{

                ////}

                //////try
                //////{
                //////    var ws8 = package.Workbook.Worksheets.Add("EXCEPTION");
                //////    ws8 = generateIssuer(ws8, logopath, reportFor, ID, null, "ACQR_EXCEP", "B", sett, 8);
                //////}
                //////catch (Exception ex)
                //////{

                //////}

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = generateIssuer3_DOLLAR(ws9, logopath, reportFor, ID, null, "ACQR_SUMM", "B", sett, 8);
                }
                catch (Exception ex)
                {

                }




                //var ws2 = package.Workbook.Worksheets.Add("ISSUER CR");
                //ws2 = generateIssuer(ws2,ID, cardScheme, "ISSR_CR", "B", sett, 11);

                //var ws3 = package.Workbook.Worksheets.Add("ISSUER CR");
                //ws3 = generateIssuer(ws2, ID, cardScheme, "ISSR_CR", "B", sett, 11);

                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
            }
            return newFile.FullName;

        }

        public static string GenReport2_DOLLAR(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string outputdir, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + ".xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "DOLLAR SETTLEMENT");

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, reportFor);

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

            using (ExcelPackage package = new ExcelPackage())
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("VISA");
                    ws = generateIssuer2_DOLLAR(ws, logopath, reportFor, ID, "VISA", "ACQ_MERCH", "B", sett, 11);

                    var ws2 = package.Workbook.Worksheets.Add("MAST");
                    ws2 = generateIssuer2_DOLLAR(ws2, logopath, reportFor, ID, "MAST", "ACQ_MERCH", "B", sett, 11);

                    var ws3 = package.Workbook.Worksheets.Add("PAYA");
                    ws3 = generateIssuer2_DOLLAR(ws3, logopath, reportFor, ID, "PAYA", "ACQ_MERCH", "B", sett, 11);

                }
                catch (Exception ex)
                {

                }


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
            }
            return newFile.FullName;

        }


    }
}