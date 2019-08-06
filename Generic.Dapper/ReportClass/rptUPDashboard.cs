
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
    class rptUPDashboard
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

        public static ExcelWorksheet generateIssuer(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string sett, int startRow)
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
            ws.Cells["A1"].Value = "UNIFIED PAYMENTS SETTLEMENT DASHBOARD";
            ws.Cells["A1"].Style.Font.Size = 18;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1:Z1"].Merge = true;
            ws.Cells["A1:Z1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["A3"].Value = "SETTLEMENT DATE: " + sett;

            ws.Cells["A5"].Value = "SN";
            ws.Cells["A5:A6"].Merge = true;
            ws.Cells["A5:A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["B5"].Value = "BANKS";
            ws.Cells["B5:B6"].Merge = true;
            ws.Cells["B5:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["B5:B6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["B5:B6"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

            ws.Cells["C5"].Value = "ALL MERCHANTs";
            ws.Cells["C5:E5"].Merge = true;
            ws.Cells["C5:E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["C5:E5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["C5:E5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            ws.Cells["C6"].Value = "TOTAL DEBIT";
            ws.Cells["C6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["D6"].Value = "TOTAL CREDIT";
            ws.Cells["E6"].Value = "NET";

            ws.Cells["F5"].Value = "PTSPs";
            ws.Cells["F5:H5"].Merge = true;
            ws.Cells["F5:H5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["F5:H5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["F5:H5"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
            ws.Cells["F6"].Value = "TOTAL DEBIT";
            ws.Cells["F6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["G6"].Value = "TOTAL CREDIT";
            ws.Cells["H6"].Value = "NET";


            ws.Cells["I5"].Value = "TERMINAL OWNERs";
            ws.Cells["I5:K5"].Merge = true;
            ws.Cells["I5:K5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["I5:K5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["I5:K5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            ws.Cells["I6"].Value = "TOTAL DEBIT";
            ws.Cells["I6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["J6"].Value = "TOTAL CREDIT";
            ws.Cells["K6"].Value = "NET";
            ////Console.WriteLine("loading excel cell");


            ws.Cells["L5"].Value = "PTSAs";
            ws.Cells["L5:N5"].Merge = true;
            ws.Cells["L5:N5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["L5:N5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["L5:N5"].Style.Fill.BackgroundColor.SetColor(Color.Orange );
            ws.Cells["L6"].Value = "TOTAL DEBIT";
            ws.Cells["L6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["M6"].Value = "TOTAL CREDIT";
            ws.Cells["N6"].Value = "NET";

            ws.Cells["O5"].Value = "ACQUIRING OBLIGATION";
            ws.Cells["O5:Q5"].Merge = true;
            ws.Cells["O5:Q5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["O5:Q5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["O5:Q5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            ws.Cells["O6"].Value = "TOTAL DEBIT";
            ws.Cells["O6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["P6"].Value = "TOTAL CREDIT";
            ws.Cells["Q6"].Value = "NET";

            ws.Cells["R5"].Value = "ISSUEING OBLIGATION";
            ws.Cells["R5:T5"].Merge = true;
            ws.Cells["R5:T5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["R5:T5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["R5:T5"].Style.Fill.BackgroundColor.SetColor(Color.Orange );
            ws.Cells["R6"].Value = "TOTAL DEBIT";
            ws.Cells["R6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["S6"].Value = "TOTAL CREDIT";
            ws.Cells["T6"].Value = "NET";

            ws.Cells["U5"].Value = "OTHERS";
            ws.Cells["U5:W5"].Merge = true;
            ws.Cells["U5:W5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["U5:W5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["U5:W5"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            ws.Cells["U6"].Value = "TOTAL DEBIT";
            ws.Cells["U6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["V6"].Value = "TOTAL CREDIT";
            ws.Cells["W6"].Value = "NET";

            ws.Cells["X5"].Value = "TOTAL";
            ws.Cells["X5:Z5"].Merge = true;
            ws.Cells["X5:Z5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["X5:Z5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["X5:Z5"].Style.Fill.BackgroundColor.SetColor(Color.Orange );
            ws.Cells["X6"].Value = "TOTAL DEBIT";
            ws.Cells["X6"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["Y6"].Value = "TOTAL CREDIT";
            ws.Cells["Z6"].Value = "NET";

            ws.View.FreezePanes(1, 1);

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

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            //////Console.WriteLine("reading excel cell");

                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                            ws.SetValue(row, 3, dr[2]);
                            ws.SetValue(row, 4, dr[3]);
                            ws.SetValue(row, 6, dr[4]);
                            ws.SetValue(row, 7, dr[5]);
                            ws.SetValue(row, 9, dr[6]);
                            ws.SetValue(row, 10, dr[7]);
                            ws.SetValue(row, 12, dr[8]);
                            ws.SetValue(row, 13, dr[9]);
                            ws.SetValue(row, 15, dr[10]);
                            ws.SetValue(row, 16, dr[11]);
                            ws.SetValue(row, 18, dr[12]);
                            ws.SetValue(row, 19, dr[13]);
                            
                          
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

            ws.Cells[startRow, 5, row , 5].FormulaR1C1 = "RC[-2]-RC[-1]";
            ws.Cells[startRow, 8, row , 8].FormulaR1C1 = "RC[-2]-RC[-1]";
            ws.Cells[startRow, 11, row , 11].FormulaR1C1 = "RC[-2]-RC[-1]";
            ws.Cells[startRow, 14, row , 14].FormulaR1C1 = "RC[-2]-RC[-1]";
            ws.Cells[startRow, 17, row , 17].FormulaR1C1 = "RC[-2]-RC[-1]";
            ws.Cells[startRow, 20, row, 20].FormulaR1C1 = "RC[-2]-RC[-1]";

            ws.Cells[startRow, 24, row , 24].FormulaR1C1 = "RC[-21]+RC[-18]+RC[-15]+RC[-12]+RC[-9]+RC[-6]+RC[-3]";
             ws.Cells[startRow, 25, row , 25].FormulaR1C1 = "RC[-21]+RC[-18]+RC[-15]+RC[-12]+RC[-9]+RC[-6]+RC[-3]";
            ws.Cells[startRow, 26, row, 26].FormulaR1C1 = "RC[-21]+RC[-18]+RC[-15]+RC[-12]+RC[-9]+RC[-6]+RC[-3]";

            //Add a sum at the end



            using (var rng = ws.Cells["A5:Z" + row  ])
            {
               // rng.Style.Font.Bold = true;
                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //rng.Style.Border = "2pt";
            }


            ws.Cells[row + 1, 1].Value = "GRAND TOTAL";
            ws.Cells[row + 1, 5].Formula = string.Format("Sum({0})", new ExcelAddress(1, 5, row, 5).Address);
            ws.Cells[row + 1, 8].Formula = string.Format("Sum({0})", new ExcelAddress(1, 8, row, 8).Address);
            ws.Cells[row + 1, 11].Formula = string.Format("Sum({0})", new ExcelAddress(1, 11, row, 11).Address);
            ws.Cells[row + 1, 14].Formula = string.Format("Sum({0})", new ExcelAddress(1, 14, row, 14).Address);
            ws.Cells[row + 1, 17].Formula = string.Format("Sum({0})", new ExcelAddress(1, 17, row, 17).Address);
            ws.Cells[row + 1, 20].Formula = string.Format("Sum({0})", new ExcelAddress(1, 20, row, 20).Address);

            ws.Cells[startRow, 3, row, 26].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 3, row + 1, 26].Style.Font.Bold = true;
            //////Format the date and numeric columns
            ws.Cells[row, 1, row, 26].AutoFitColumns(20);

 

            ws.Select("A1");

            return ws;


        }

        public static string GenReport1(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + ".xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NAIRA SETTLEMENT");

            

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }
            string reportISSUERPath = oISSUERPath + "\\" + reportname;

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
                    var ws = package.Workbook.Worksheets.Add("UP Settlement Dashboard");
                    ws = generateIssuer(ws, logopath, reportFor, null, null, "DASH_NGN", "U", sett, 7);


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