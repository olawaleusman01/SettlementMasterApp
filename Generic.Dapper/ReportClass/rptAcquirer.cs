
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
using System.Data;
using UPPosMaster.Dapper.Data;
using Generic.Dapper.ReportClass;

namespace UPPosMaster.Dapper.ReportClass
{
    class rptAcquirer
    {
    
      
        public static ExcelWorksheet generateReport(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel, string sett, int startRow)
        {
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
            else if(cardScheme.ToUpper() == "MAST")
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

            ws.Cells["A4"].Value = "ACQUIRER INCOME REPORT FOR " + CARDSCHEMDESC;
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
                dsLoadclass dv = new dsLoadclass();

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))


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


                            ////row++;
                        }

                    }
                   

                }


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            ////ws.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
            ////ws.Cells[2, 5, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            ////ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
            ////ws.Cells[2, 43, row + 1, 48].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[2, 49, row + 1, 50].Style.Numberformat.Format = "#,##0";
            ////ws.Cells[2, 51, row + 1, 57].Style.Numberformat.Format = "#,##0.00";
            //// ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";

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
            if (columnIDcnt > 0)
            {
                ws.Cells[startRow, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                ws.Cells[startRow, 5, row + 1, 7].Style.Numberformat.Format = "dd-mm-yyyy";
                // ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
                ws.Cells[startRow, 38, row + 1, 39].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 44, row + 1, 45].Style.Numberformat.Format = "#,##0";
                ws.Cells[startRow, 46, row + 1, 46].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 48, row + 1, 52].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 56, row + 1, 56].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 60, row + 1, 60].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 62, row + 1, 62].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 65, row + 1, 66].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 69, row + 1, 69].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 72, row + 1, 72].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 75, row + 1, 75].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 78, row + 1, 78].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[startRow, 81, row + 1, 82].Style.Numberformat.Format = "#,##0.00";
                // ws.Cells[2, 57, row + 1, 58].Style.Numberformat.Format = "#,##0.00";
                // ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";


                //ws.Cells[row + 1, 1].Value = "TOTAL";
                //ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

                //ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[row, 1].Value = "TOTAL";
                ws.Cells[row, 38, row, 39].Formula = string.Format("=SUM(AL{0}:AL{1})", startRow, row - 1);
                ws.Cells[row, 46, row, 46].Formula = string.Format("=SUM(AT{0}:AT{1})", startRow, row - 1);
                ws.Cells[row, 48, row, 52].Formula = string.Format("=SUM(AV{0}:AV{1})", startRow, row - 1);

                ws.Cells[row, 56, row, 56].Formula = string.Format("=SUM(BD{0}:BD{1})", startRow, row - 1);
                ws.Cells[row, 60, row, 60].Formula = string.Format("=SUM(BH{0}:BH{1})", startRow, row - 1);
                ws.Cells[row, 62, row, 62].Formula = string.Format("=SUM(BJ{0}:BJ{1})", startRow, row - 1);
                ws.Cells[row, 65, row, 66].Formula = string.Format("=SUM(BM{0}:BN{1})", startRow, row - 1);
                ws.Cells[row, 69, row, 69].Formula = string.Format("=SUM(BQ{0}:BQ{1})", startRow, row - 1);
                ws.Cells[row, 72, row, 72].Formula = string.Format("=SUM(BT{0}:BT{1})", startRow, row - 1);

                ws.Cells[row, 75, row, 75].Formula = string.Format("=SUM(BW{0}:BW{1})", startRow, row - 1);
                ws.Cells[row, 78, row, 78].Formula = string.Format("=SUM(BZ{0}:BZ{1})", startRow, row - 1);
                ws.Cells[row, 81, row, 82].Formula = string.Format("=SUM(CC{0}:CD{1})", startRow, row - 1);

                ws.Cells[startRow, 1, row, columnIDcnt].AutoFitColumns(20);

                ws.Select("C8");
            }

            return ws;


        }

        public static ExcelWorksheet generateReportSUMM(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType,  string reportClass,string subreporttype, int Channel, string sett, int startRow)
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

            ws.Cells["A4"].Value = "ACQUIRER INCOME SUMMARY " ;
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
                dsLoadclass dv = new dsLoadclass();

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))


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
                            ////ws.SetValue(row, 1, dr[0]);
                            ////ws.SetValue(row, 2, dr[1]);
                            ////ws.SetValue(row, 3, dr[2]);
                            ////ws.SetValue(row, 4, dr[3]);
                            ////ws.SetValue(row, 5, dr[4]);
                            ////ws.SetValue(row, 6, dr[5]);
                            ////ws.SetValue(row, 7, dr[6]);
                            ////ws.SetValue(row, 8, dr[7]);
                            ////if (reportType == "ACQR_EXCEP")
                            ////{
                            ////    ws.SetValue(row, 9, dr[8]);
                            ////    ws.SetValue(row, 10, dr[9]);
                            ////}

                            ////row++;
                        }

                    }
                   

                }


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }

            if (columnIDcnt > 0)
            {
                ws.Cells[2, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[row + 1, 1].Value = "TOTAL";
                ws.Cells[row + 1, 1].Style.Font.Bold = true;
                ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

                ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";


                ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);

                ws.Select("C8");
            }

            return ws;


        }


        public static string GenReport1(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath,  string logopath, string reportFolder,string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + "_ACQUIRER.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "ACQUIRER");

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
                    var ws = package.Workbook.Worksheets.Add("VISA CARD DR");
                    ws = generateReport(ws, logopath, reportFor, ID, "VISA", "ACQR_DR","B","",0, sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {

                    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                    ws2 = generateReport(ws2, logopath, reportFor, ID, "VISA", "ACQR_CR",  "B","",0, sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("MASTER CARD DR");
                    ws3 = generateReport(ws3, logopath, reportFor, ID, "MAST", "ACQR_DR",  "B","",0, sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("MASTER CARD CR");
                    ws4 = generateReport(ws4, logopath, reportFor, ID, "MAST", "ACQR_CR", "B","",0, sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3A = package.Workbook.Worksheets.Add("VERVE CARD DR");
                    ws3A = generateReport(ws3A, logopath, reportFor, ID, "VERV", "ACQR_DR",  "B","",0, sett, 8);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4A = package.Workbook.Worksheets.Add("VERVE CARD CR");
                    ws4A = generateReport(ws4A, logopath, reportFor, ID, "VERV", "ACQR_CR","B","",0, sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                    ws5 = generateReport(ws5, logopath, reportFor, ID, "PAYA", "ACQR_DR",  "B","",0,  sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                    ws6 = generateReport(ws6, logopath, reportFor, ID, "PAYA", "ACQR_CR",  "B","",0, sett, 8);

                }
                catch (Exception ex)
                {

                }



                try
                {
                    var ws7 = package.Workbook.Worksheets.Add("CHINA UNION PAY DR");
                    ws7 = generateReport(ws7, logopath, reportFor, ID, "CUPI", "ACQR_DR",  "B","",0, sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("CHINA UNION PAY CR");
                    ws8 = generateReport(ws8, logopath, reportFor, ID, "CUPI", "ACQR_CR",  "B","",0, sett, 8);

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = generateReportSUMM(ws9, logopath, reportFor, ID, null, "ACQR_SUMM","B", null,0, sett, 8);

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

        public static string GenReport2(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath, string reportFolder, string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + "_ACQUIRER_MERCHANTS.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "ACQUIRER");

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

            dsLoadclass dv = new dsLoadclass();
            dv.generateDS(reportFor, ID, null, "ACQ_MERCH","B",sett, 0);



            return null;

            ////FileInfo newFile = new FileInfo(reportISSUERPath);
            ////MemoryStream stream = new MemoryStream();

            ////using (ExcelPackage package = new ExcelPackage(stream))
            ////{


            ////    try
            ////    {
            ////        var ws = package.Workbook.Worksheets.Add("AQUIRER MERCHANTS");
            ////        ws = generateReport(ws, logopath, reportFor, ID, null, "ACQ_MERCH",  "B","",0, "B", sett, 8);


            ////    }
            ////    catch (Exception ex)
            ////    {

            ////    }



            ////    package.Compression = CompressionLevel.BestSpeed;
            ////    package.SaveAs(newFile);
            ////    Stream.Dispose();:Stream.Dispose();:package.Dispose();
            ////}

            ////return newFile.FullName;

        }



        

    }
}