
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OfficeOpenXml;
using System.IO;
using System.Data.SqlClient;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style; 
using System.Data;
using UPPosMaster.Dapper.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.Table;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Generic.Dapper.ReportClass;

namespace UPPosMaster.Dapper.ReportClass
{
    class rptNIBSS_New { 
     
        private static string oradb = System.Configuration.ConfigurationManager.AppSettings["TestDB"].ToString();
        static private DataTable ResultsData = new DataTable();
        static private int rowsPerSheet = 100;
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
        public static ExcelWorksheet generateIssuer(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel, string sett, int startRow)
        {

            
         
            ws.Cells["A1"].Value = "RECID";
            ws.Cells["B1"].Value = "VOLUME";
            ws.Cells["C1"].Value = "ACQUIRINGINSTITUTIONID";
            ws.Cells["D1"].Value = "PAYEEID";
            ws.Cells["E1"].Value = "BATCHNUM";
            ws.Cells["F1"].Value = "PAYEEBANKCODE";
            ws.Cells["G1"].Value = "PAYEENAME";
            ws.Cells["H1"].Value = "PAYERNAME";
            ws.Cells["I1"].Value = "PAYEEACCOUNT";
            ws.Cells["J1"].Value = "DR_AMOUNT";
            ws.Cells["K1"].Value = "CR_AMOUNT";
            ws.Cells["L1"].Value = "NARATION";
            ws.Cells["M1"].Value = "RECTYPE";
            ws.Cells["N1"].Value = "POSTRANSDATE";
            ws.Cells["O1"].Value = "NETFLAG";
            ws.Cells["P1"].Value = "PAYERACCOUNT";
            ws.Cells["Q1"].Value = "TOTALBATCHAMT";
            ws.Cells["R1"].Value = "PAYERBANKCODE";
            ws.Cells["S1"].Value = "PAYERID";
            ws.Cells["T1"].Value = "RECBATCHNUM";
            ws.Cells["U1"].Value = "PFLAG";
            ws.Cells["V1"].Value = "RETAILEROUTLETNAME";
            ws.Cells["W1"].Value = "MERCHANTDEPOSITBANK";
      


            int row = startRow;
            try
            {
              
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID,cardScheme , reportType, reportClass, subreporttype, sett, Channel))
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
                            ws.SetValue(row, 16, dr[15]);
                            ws.SetValue(row, 17, dr[16]);
                            ws.SetValue(row, 18, dr[17]);
                            ws.SetValue(row, 19, dr[18]);
                            ws.SetValue(row, 20, dr[19]);
                            ws.SetValue(row, 21, dr[20]);
                            ws.SetValue(row, 22, dr[21]);
                            ws.SetValue(row, 23, dr[22]);

                            


                            row++;
                        }

                    }
                   
                }

                
            }
            catch (Exception ex)
            {

            }

            ////ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ////ws.Cells[row + 1, 10, row + 1, 10].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[row + 1, 11, row + 1, 11].Style.Numberformat.Format = "#,##0.00";
             ws.Cells[startRow, 1, row, 10].AutoFitColumns(20);

               
            return ws;


        }

        public static ExcelWorksheet generateIssuerALL(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {



            ws.Cells["A1"].Value = "RECID";
            ws.Cells["B1"].Value = "VOLUME";
            ws.Cells["C1"].Value = "ACQUIRINGINSTITUTIONID";
            ws.Cells["D1"].Value = "PAYEEID";
            ws.Cells["E1"].Value = "BATCHNUM";
            ws.Cells["F1"].Value = "PAYEEBANKCODE";
            ws.Cells["G1"].Value = "PAYEENAME";
            ws.Cells["H1"].Value = "PAYERNAME";
            ws.Cells["I1"].Value = "PAYEEACCOUNT";
            ws.Cells["J1"].Value = "DR_AMOUNT";
            ws.Cells["K1"].Value = "CR_AMOUNT";
            ws.Cells["L1"].Value = "NARATION";
            ws.Cells["M1"].Value = "RECTYPE";
            ws.Cells["N1"].Value = "POSTRANSDATE";
            ws.Cells["O1"].Value = "NETFLAG";
            ws.Cells["P1"].Value = "PAYERACCOUNT";
            ws.Cells["Q1"].Value = "TOTALBATCHAMT";
            ws.Cells["R1"].Value = "PAYERBANKCODE";
            ws.Cells["S1"].Value = "PAYERID";
            ws.Cells["T1"].Value = "RECBATCHNUM";
            ws.Cells["U1"].Value = "PFLAG";
            ws.Cells["V1"].Value = "RETAILEROUTLETNAME";
            ws.Cells["W1"].Value = "MERCHANTDEPOSITBANK";



            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

                {

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ws.SetValue(row, 1, dr[1]);
                            ws.SetValue(row, 2, dr[2]);
                            ws.SetValue(row, 3, dr[3]);
                            ws.SetValue(row, 4, dr[4]);
                            ws.SetValue(row, 5, dr[5]);
                            ws.SetValue(row, 6, dr[6]);
                            ws.SetValue(row, 7, dr[7]);
                            ws.SetValue(row, 8, dr[8]);
                            ws.SetValue(row, 9, dr[9]);
                            ws.SetValue(row, 10, dr[10]);
                            ws.SetValue(row, 11, dr[11]);
                            ws.SetValue(row, 12, dr[12]);
                            ws.SetValue(row, 13, dr[13]);
                            ws.SetValue(row, 14, dr[14]);

                            ws.SetValue(row, 15, dr[15]);
                            ws.SetValue(row, 16, dr[16]);
                            ws.SetValue(row, 17, dr[17]);
                            ws.SetValue(row, 18, dr[18]);
                            ws.SetValue(row, 19, dr[19]);
                            ws.SetValue(row, 20, dr[20]);
                            ws.SetValue(row, 21, dr[21]);
                            ws.SetValue(row, 22, dr[22]);
                            ws.SetValue(row, 23, dr[23]);




                            row++;
                        }

                    }
                    

                }


            }
            catch (Exception ex)
            {

            }

            ////ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ////ws.Cells[row + 1, 10, row + 1, 10].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[row + 1, 11, row + 1, 11].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[startRow, 1, row, 10].AutoFitColumns(20);


            return ws;


        }


        public static ExcelWorksheet generateIssuer2(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {



            ws.Cells["A1"].Value = "ID";
            ws.Cells["B1"].Value = "DOCNO";
            ws.Cells["C1"].Value = "MERCHANTID";
            ws.Cells["D1"].Value = "TERMINALID";
            ws.Cells["E1"].Value = "ERROR_MESSAGE";
            



            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

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
                         



                            row++;
                        }

                    }
                     

                }


            }
            catch (Exception ex)
            {

            }

            ////ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ////ws.Cells[row + 1, 10, row + 1, 10].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[row + 1, 11, row + 1, 11].Style.Numberformat.Format = "#,##0.00";
             ws.Cells[startRow, 1, row, 10].AutoFitColumns(20);


            return ws;


        }

        public static ExcelWorksheet generateIssuer3(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {



            ws.Cells["A1"].Value = "RECID";
            ws.Cells["B1"].Value = "VOLUME";
            ws.Cells["C1"].Value = "ACQUIRINGINSTITUTIONID";
            ws.Cells["D1"].Value = "PAYEEID";
            ws.Cells["E1"].Value = "BATCHNUM";
            ws.Cells["F1"].Value = "PAYEEBANKCODE";
            ws.Cells["G1"].Value = "PAYEENAME";
            ws.Cells["H1"].Value = "PAYERNAME";
            ws.Cells["I1"].Value = "PAYEEACCOUNT";
            ws.Cells["J1"].Value = "DR_AMOUNT";
            ws.Cells["K1"].Value = "CR_AMOUNT";
            ws.Cells["L1"].Value = "NARATION";
            ws.Cells["M1"].Value = "RECTYPE";
            ws.Cells["N1"].Value = "POSTRANSDATE";
            ws.Cells["O1"].Value = "NETFLAG";
            ws.Cells["P1"].Value = "PAYERACCOUNT";
            ws.Cells["Q1"].Value = "TOTALBATCHAMT";
            ws.Cells["R1"].Value = "PAYERBANKCODE";
            ws.Cells["S1"].Value = "PAYERID";
            ws.Cells["T1"].Value = "RECBATCHNUM";
            ws.Cells["U1"].Value = "PFLAG";
            ws.Cells["V1"].Value = "RETAILEROUTLETNAME";
            ws.Cells["W1"].Value = "MERCHANTDEPOSITBANK";



            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

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
                            ws.SetValue(row, 16, dr[15]);
                            ws.SetValue(row, 17, dr[16]);
                            ws.SetValue(row, 18, dr[17]);
                            ws.SetValue(row, 19, dr[18]);
                            ws.SetValue(row, 20, dr[19]);
                            ws.SetValue(row, 21, dr[20]);
                            ws.SetValue(row, 22, dr[21]);
                            ws.SetValue(row, 23, dr[22]);




                            row++;
                        }

                    }
                   

                }


            }
            catch (Exception ex)
            {

            }
            ////ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ////ws.Cells[row + 1, 10, row + 1, 10].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[row + 1, 11, row + 1, 11].Style.Numberformat.Format = "#,##0.00";
             ws.Cells[startRow, 1, row, 10].AutoFitColumns(20);


            return ws;

        }

    


        public static ExcelWorksheet generateIssuer4(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {

            

            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

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


            ws.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
            ws.Cells[2, 5, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
            ws.Cells[2, 44, row + 1, 54].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 57, row + 1, 58].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";




            ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);


            return ws;


        }


        public static ExcelWorksheet generateIssuer5(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {
            
            ws.Cells["A1"].Value = "NAME";
            ws.Cells["B1"].Value = "COUNT";
            ws.Cells["C1"].Value = "VALUE";
           


            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

                {

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                            ws.SetValue(row, 3, dr[2]);
                          
                            row++;
                        }

                    }
                  

                }


            }
            catch (Exception ex)
            {

            }
           
            var pieChart = ws.Drawings.AddChart("crtExtensionsSize", eChartType.PieExploded3D) as ExcelPieChart;
            //Set top left corner to row 1 column 2
            pieChart.SetPosition(10, 0, 0, 0);
            pieChart.SetSize(400, 400);
            pieChart.Series.Add(ExcelRange.GetAddress(2, 3, row - 1, 3), ExcelRange.GetAddress(2, 2, row - 1, 2));

            pieChart.Title.Text = "DAILY TRANSACTION";
            //Set datalabels and remove the legend
            pieChart.DataLabel.ShowCategory = true;
            pieChart.DataLabel.ShowPercent = true;
            pieChart.DataLabel.ShowLeaderLines = true;
            pieChart.Legend.Remove();

            ws.Cells[1, 2, row + 1, 3].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[startRow, 1, row, 10].AutoFitColumns(20);


            return ws;


        }

 
        public static ExcelWorksheet generateIssuer6(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {

            //ws.Cells["A1"].Value = "TOTAL TRANSACTION AMOUNT";
            //ws.Cells["B1"].Value = "TOTAL AMOUNT DUE TO MERCHANT";
            //ws.Cells["C1"].Value = "TOTAL MSC AMOUNT";
            //ws.Cells["D1"].Value = "TOTAL SHARED MSC AMOUNT";
            //ws.Cells["E1"].Value = "TOTAL AMOUNT DUE + MSC";
            //ws.Cells["F1"].Value = "TOTAL AMOUNT DUE ALL PARTY";
            //ws.Cells["G1"].Value = "TOTAL MARGIN ON MSC1 AMOUNT AND SHARED MSC";
            //ws.Cells["H1"].Value = "TOTAL SUBSIDY";
            //ws.Cells["I1"].Value = "TOTAL CONCESSION";
            //ws.Cells["J1"].Value = "TOTAL AMOUNT DEBITED ISSUER BY UP";
            //ws.Cells["K1"].Value = "TOTAL AMOUNT DEBITED ISSUER BY MASTERCARD";
            //ws.Cells["L1"].Value = "TOTAL AMOUNT DEBITED ISSUER BY VISA";
            //ws.Cells["M1"].Value = "TOTAL AMOUNT DEBITED ISSUER BY NBSS";
            //ws.Cells["N1"].Value = "TOTAL TEST TRANSACTIONS";

            ws.Cells["A1"].Value = "DESCRIPTION";
            ws.Cells["B1"].Value = "VALUE";


            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

                {

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ws.SetValue(row, 1, dr[0]);
                            ws.SetValue(row, 2, dr[1]);
                          
                            ////ws.SetValue(row, 3, dr[2]);
                            ////ws.SetValue(row, 4, dr[3]);
                            ////ws.SetValue(row, 5, dr[4]);
                            ////ws.SetValue(row, 6, dr[5]);
                            ////ws.SetValue(row, 7, dr[6]);
                            ////ws.SetValue(row, 8, dr[7]);
                            ////ws.SetValue(row, 9, dr[8]);
                            ////ws.SetValue(row, 10, dr[9]);
                            ////ws.SetValue(row, 11, dr[10]);
                            ////ws.SetValue(row, 12, dr[11]);
                            ////ws.SetValue(row, 13, dr[12]);
                            ////ws.SetValue(row, 14, dr[13]);




                            row++;
                        }

                    }
                  

                }


            }
            catch (Exception ex)
            {

            }

            ws.Cells["E2"].Value = "ISSUER OBLIGATION BY UP/CARD SCHEME";
            ws.Cells["E2"].Style.Font.Size = 14;
            ws.Cells["E2"].Style.Font.Bold = true;
            ws.Cells["E2:I4"].Merge = true;
            ws.Cells["E2:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["E6"].Value = "DESCRIPTION";
             ws.Cells["F6"].Value = "VALUE";

            ws.Cells["H6"].Value = "DESCRIPTION";
            ws.Cells["I6"].Value = "VALUE";

            ws.Cells["E7"].Value = "TOTAL TRANSACTION AMOUNT";
            ws.Cells["F7"].Value = ws.Cells["B2"].Value;


            ws.Cells["H7"].Value = "TOTAL AMOUNT DR ISSUER BY UP";
            ws.Cells["I7"].Value = ws.Cells["B13"].Value;

            ws.Cells["H8"].Value = "TOTAL AMOUNT DR ISSUER BY MASTER CARD";
            ws.Cells["I8"].Value = ws.Cells["B14"].Value;

            ws.Cells["H9"].Value = "TOTAL AMOUNT DR ISSUER BY VISA CARD";
            ws.Cells["I9"].Value = ws.Cells["B15"].Value;

            ws.Cells["H10"].Value = "TOTAL AMOUNT DR ISSUER BY CUPI CARD";
            ws.Cells["I10"].Value = ws.Cells["B16"].Value;

            ws.Cells["H11"].Value = "TOTAL AMOUNT DR ISSUER BY NBSS";
            ws.Cells["I11"].Value = ws.Cells["B17"].Value;

            ws.Cells["H12"].Value = "TOTAL AMOUNT DR ISSUER BY OTHERS";
            ws.Cells["I12"].Value = ws.Cells["B18"].Value;

            ws.Cells["I13"].Formula = "SUM(I7:I12)";
            //ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells[1, 2, row + 1, 2].Style.Numberformat.Format = "#,##0.00";
            ws.Cells["F7"].Style.Numberformat.Format = "#,##0.00";
            ws.Cells["I7:I13"].Style.Numberformat.Format = "#,##0.00";


            ws.Cells["E15"].Value = "TRAN. AMOUNT VS AMOUNT DUE MERCHANT+MSC-SUBSIDY";
            ws.Cells["E15"].Style.Font.Size = 14;
            ws.Cells["E15"].Style.Font.Bold = true;
            ws.Cells["E15:I17"].Merge = true;
            ws.Cells["E15:I17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["E19"].Value = "DESCRIPTION";
            ws.Cells["F19"].Value = "VALUE";

            ws.Cells["H19"].Value = "DESCRIPTION";
            ws.Cells["I19"].Value = "VALUE";

            ws.Cells["E20"].Value = "TOTAL TRANSACTION AMOUNT";
            ws.Cells["F20"].Value = ws.Cells["B2"].Value;

            ws.Cells["H20"].Value = "TOTAL AMOUNT DUE TO MERCHANT";
            ws.Cells["I20"].Value = ws.Cells["B3"].Value;

            ws.Cells["H21"].Value = "TOTAL MSC AMOUNT";
            ws.Cells["I21"].Value = ws.Cells["B4"].Value;

            ws.Cells["H22"].Value = "LESS TOTAL SUBSIDY";
            ws.Cells["I22"].Value = ws.Cells["B11"].Value;

            ws.Cells["I23"].Formula = "SUM(I20:I21)-I22";
            //ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells["F20"].Style.Numberformat.Format = "#,##0.00";
            ws.Cells["I20:I23"].Style.Numberformat.Format = "#,##0.00";


            ws.Cells["E25"].Value = "TRAN. AMOUNT VS AMOUNT DUE MERCHANT+SHARING MSC+ALL PARTY-SUBSIDY";
            ws.Cells["E25"].Style.Font.Size = 14;
            ws.Cells["E25"].Style.Font.Bold = true;
            ws.Cells["E25:I27"].Merge = true;
            ws.Cells["E25:I27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["E29"].Value = "DESCRIPTION";
            ws.Cells["F29"].Value = "VALUE";

            ws.Cells["H29"].Value = "DESCRIPTION";
            ws.Cells["I29"].Value = "VALUE";

            ws.Cells["E30"].Value = "TOTAL TRANSACTION AMOUNT";
            ws.Cells["F30"].Value = ws.Cells["B2"].Value;

            ws.Cells["H30"].Value = "TOTAL AMOUNT DUE TO MERCHANT";
            ws.Cells["I30"].Value = ws.Cells["B3"].Value;

            ws.Cells["H31"].Value = "TOTAL MSC2 AMOUNT";
            ws.Cells["I31"].Value = ws.Cells["B7"].Value;

            ws.Cells["H32"].Value = "TOTAL AMOUNT DUE ALL PARTY";
            ws.Cells["I32"].Value = ws.Cells["B9"].Value;

            ws.Cells["H33"].Value = "TOTAL MARGIN ON MSC1 AMOUNT";
            ws.Cells["I33"].Value = ws.Cells["B10"].Value;

            ws.Cells["H34"].Value = "LESS TOTAL SUBSIDY";
            ws.Cells["I34"].Value = ws.Cells["B11"].Value;

            ws.Cells["I35"].Formula = "SUM(I30:I33)-I22";
            //ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ws.Cells["F30"].Style.Numberformat.Format = "#,##0.00";
            ws.Cells["I30:I35"].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[row + 1, 11, row + 1, 11].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[startRow, 1, row, 10].AutoFitColumns(20);
            
            return ws;


        }

        public static ExcelWorksheet generateIssuer7(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {
            ws.Cells["A1"].Value = "DOCNO";
            ws.Cells["B1"].Value = "FULLREM";
            ws.Cells["C1"].Value = "TRANAMOUNT";
            ws.Cells["D1"].Value = "DEBITACCT";
            ws.Cells["E1"].Value = "CREDITACCT";
            ws.Cells["F1"].Value = "TRANDATE";
            ws.Cells["G1"].Value = "ISSFIID";
            ws.Cells["H1"].Value = "PAN";
            ws.Cells["I1"].Value = "TERMINAL ID";
            ws.Cells["J1"].Value = "MERCHANTID";
            ws.Cells["K1"].Value = "ERROR MESSAGE";

            int row = startRow;
            string prevrec = string.Empty ;
            string prevrec1 = string.Empty;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

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




                            row++;
                        }

                    }
                  

                }


            }
            catch (Exception ex)
            {

            }

            ///attach summary
            ///
            ws.Cells["N2"].Value = "EXCEPTION SUMMARY" ;
            ws.Cells["N2"].Style.Font.Size = 14;
            ws.Cells["N2"].Style.Font.Bold = true;
            ws.Cells["N2:R4"].Merge = true;
            ws.Cells["N2:R4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["N6"].Value = "TITLE";
            ws.Cells["O6"].Value = "FULLREM";
            ws.Cells["P6"].Value = "FULLREM TRANSLATION";
            ws.Cells["Q6"].Value = "COUNT";
            ws.Cells["R6"].Value = "VALUE";
           

            row = 7;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

                {

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            prevrec = dr[0].ToString();

                            if (prevrec == prevrec1)
                            {
                                 ws.SetValue(row, 15, dr[1]);
                                ws.SetValue(row, 16, dr[2]);
                                ws.SetValue(row, 17, dr[3]);
                                ws.SetValue(row, 18, dr[4]);

                            }
                            else
                            {
                                ws.SetValue(row, 14, dr[0]);
                                ws.SetValue(row, 15, dr[1]);
                                ws.SetValue(row, 16, dr[2]);
                                ws.SetValue(row, 17, dr[3]);
                                ws.SetValue(row, 18, dr[4]);

                            }
                            prevrec1 = prevrec;


                            row++;
                        }

                    }
                   

                }


            }
            catch (Exception ex)
            {

            }

            ////ws.Cells[1, 1, row, 1].Style.Numberformat.Format = "#,##0";
            ////ws.Cells[row + 1, 10, row + 1, 10].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[row + 1, 11, row + 1, 11].Style.Numberformat.Format = "#,##0.00";

            ws.Cells["N30"].Value = "EXCEPTION CHART";
            ws.Cells["N30"].Style.Font.Size = 14;
            ws.Cells["N30"].Style.Font.Bold = true;
            ws.Cells["N30:R40"].Merge = true;
            ws.Cells["N30:R40"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            var pieChart = ws.Drawings.AddChart("crtExtensionsSize", eChartType.PieExploded3D) as ExcelPieChart;
            //Set top left corner to row 1 column 2
            pieChart.SetPosition(20, 0, 12, 0);
            pieChart.SetSize(400, 400);
            pieChart.Series.Add(ExcelRange.GetAddress(7, 18, row - 1,18), ExcelRange.GetAddress(7, 17, row - 1, 17));

            pieChart.Title.Text = "EXCEPTION SUMMARY";
            //Set datalabels and remove the legend
            pieChart.DataLabel.ShowCategory = true;
            pieChart.DataLabel.ShowPercent = true;
            pieChart.DataLabel.ShowLeaderLines = true;
            pieChart.Legend.Remove();


            ws.Cells[1, 3, row + 1, 3].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[1, 17, row + 1, 18].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[startRow, 1, row, 40].AutoFitColumns(20);

           

           
            return ws;


        }


        public static ExcelWorksheet generateIssuer8(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass, string subreporttype, int Channel, string sett, int startRow)
        {



            ws.Cells["A1"].Value = "RETAILERNAME";
            ws.Cells["B1"].Value = "RETAILERID";
            ws.Cells["C1"].Value = "ACCOUNTNUMBER";
            ws.Cells["D1"].Value = "BANKCODE";
            ws.Cells["E1"].Value = "BANKNAME";
            ws.Cells["F1"].Value = "MCC";
            ws.Cells["G1"].Value = "ACQUIRER";
            ws.Cells["H1"].Value = "INSTITUTION_NAME";
            ws.Cells["I1"].Value = "PTSP";
            ws.Cells["J1"].Value = "CARDSCHEME";
            ws.Cells["K1"].Value = "DOM MSC1";
            ws.Cells["L1"].Value = "DOM MSC2";
            ws.Cells["M1"].Value = "TOTAL MSC2";

            int row = startRow;
            string qry = string.Empty;
            try
            {
                dsLoadclass dv = new dsLoadclass();
              
                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

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
                            row++;
                        }

                    }
                  

                }


            }
            catch (Exception ex)
            {

            }


            ws.Cells[2, 11, row + 1, 13].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[row, 1, row, 18].AutoFitColumns(20);
            var pieChart = ws.Drawings.AddChart("crtExtensionsSize", eChartType.PieExploded3D) as ExcelPieChart;
            //Set top left corner to row 1 column 2
            pieChart.SetPosition(1, 0, 2, 0);
            pieChart.SetSize(400, 400);
            pieChart.Series.Add(ExcelRange.GetAddress(4, 2, row - 1, 2), ExcelRange.GetAddress(4, 1, row - 1, 1));

            pieChart.Title.Text = "Extension Size";
            //Set datalabels and remove the legend
            pieChart.DataLabel.ShowCategory = true;
            pieChart.DataLabel.ShowPercent = true;
            pieChart.DataLabel.ShowLeaderLines = true;
            pieChart.Legend.Remove();
            return ws;


        }



        public static ExcelWorksheet generateREPORT(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {


            int columnIDcnt = 0;
            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

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

            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[row + 1, 1].Style.Font.Bold = true;
            ws.Cells[row + 1, 2, row + 1, columnIDcnt-1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row );
            ws.Cells[1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 2, row + 1, columnIDcnt-1].Style.Numberformat.Format = "#,##0.00";


            ws.Cells[1, 1, row, 18].AutoFitColumns(20);

            return ws;


        }


        public static ExcelWorksheet generateREPORTGRAPH(ExcelWorksheet ws, string img, string rName, string ID, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  string sett, int startRow)
        {


            int columnIDcnt = 0;
            int row = startRow;
            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

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

            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

            ws.Cells[row + 1, 2, row + 1, columnIDcnt-1].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[1, 1, row, 18].AutoFitColumns(30);

            // pieChart.Series.Add(ExcelRange.GetAddress(2, 3, row - 1, 3), ExcelRange.GetAddress(2, 2, row - 1, 2));

            ws.Cells[row + 10 , 1].Value = "BAR CHART";
            ws.Cells[row + 10, 1].Style.Font.Size = 14;
            ws.Cells[row + 10, 1].Style.Font.Bold = true;
            ws.Cells[row + 10, 1, row + 5, 10].Merge = true;
            ws.Cells[row + 10, 1, row + 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

       

            ////var chart = (ExcelBarChart)ws.Drawings.AddChart("Chart", eChartType.ColumnClustered);
            ////chart.SetSize(300, 300);
            ////chart.SetPosition(10, 10);
            ////chart.Title.Text = rName;

            ////chart.Series.Add(ExcelRange.GetAddress(startRow , 2, row, 2), ExcelRange.GetAddress(startRow, 1, row, 1));


            ////var pieChart = ws.Drawings.AddChart("crtExtensionsSize", eChartType.BarStacked) as ExcelBarChart;
            //////Set top left corner to row 1 column 2
            ////pieChart.SetPosition(row + 10, 0, 0, 0);
            ////pieChart.SetSize(400, 400);
            ////pieChart.Series.Add(ExcelRange.GetAddress(2, 1, row - 1, 1), ExcelRange.GetAddress(2,2, row - 1,2));


            return ws;


        }

        #endregion


     

        public static string GenReport2b(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = "NIBSS ALL.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NIBSS ALL");

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

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("NIBSS ALL");
                    ws = generateIssuerALL(ws, logopath, null, null, null, "NIBSS_ALL", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                ////try
                ////{
                ////    var ws2 = package.Workbook.Worksheets.Add("NIBSS VISA");
                ////    ws2 = generateIssuer(ws2, logopath, null, null, null, "NIBSS_VISA", "U", sett, 1);


                ////}
                ////catch (Exception ex)
                ////{

                ////}


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }
            return newFile.FullName;

        }




        public static string GenReport2c(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {

            /*

            
                    OracleConnection Standby_connection = new OracleConnection(oradb);
                    string qry = "SELECT DISTINCT CARDSCHEME,CARDSCHEME_DESC FROM POSMISDB_CARDSCHEME WHERE UPPER(STATUS)='ACTIVE'";
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
                    int i = 0;
                    cmd.CommandType = CommandType.Text;
                    using (var dr = cmd.ExecuteReader())
                    {

                        if (dr.HasRows)
                        {
                            i += 1;
                            while (dr.Read())
                            {

                                   var ws= package.Workbook.Worksheets.Add("NIBSS" + );
                                ws = generateIssuer(ws, logopath, null, null, "VISA", "NIBSS_CARD", "U", sett, 2);

                            }
                        }

                    }

                             


    */
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = "NIBSS CARD.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NIBSS CARD");

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

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                //loop on cardscheme
                try
                {

                    var ws = package.Workbook.Worksheets.Add("NIBSS VISA");
                    ws = generateIssuerALL(ws, logopath, null, null, "VISA", "NIBSS_CARD", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws2 = package.Workbook.Worksheets.Add("NIBSS MASTERCARD");
                    ws2 = generateIssuer(ws2, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws2 = package.Workbook.Worksheets.Add("NIBSS PAY ATITUTDE");
                    ws2 = generateIssuer(ws2, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws2 = package.Workbook.Worksheets.Add("NIBSS CHINA UNION PAY");
                    ws2 = generateIssuer(ws2, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                ////try
                ////{
                ////    var ws2 = package.Workbook.Worksheets.Add("NIBSS VISA");
                ////    ws2 = generateIssuer(ws2, logopath, null, null, null, "NIBSS_VISA", "U", sett, 1);


                ////}
                ////catch (Exception ex)
                ////{

                ////}


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }
            return newFile.FullName;

        }


        //public static string GenReport2xml(DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        //{

        //    string sett = reportDate.ToString("dd-MMM-yyyy");


        //    string reportname = "NIBSS_CREDIT.xml";
        //    string reportname2 = "NIBSS_DEBIT.xml";


        //    //string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NIBSS ALL");

        //    //if (!Directory.Exists(oISSUERPath))
        //    //{
        //    //    System.IO.Directory.CreateDirectory(oISSUERPath);
        //    //}




        //    string reportISSUERPath = ISSUERPath + "\\" + reportname;

        //    try
        //    {
        //        if (System.IO.File.Exists(reportISSUERPath))
        //        {

        //            System.IO.File.Delete(reportISSUERPath);

        //        }
        //    }
        //    catch
        //    {

        //    }


        //    string reportISSUERPath2 = ISSUERPath + "\\" + reportname2;

        //    try
        //    {
        //        if (System.IO.File.Exists(reportISSUERPath2))
        //        {

        //            System.IO.File.Delete(reportISSUERPath2);

        //        }
        //    }
        //    catch
        //    {

        //    }


        //    string xml = string.Empty;

        //    dsLoadclass dv = new dsLoadclass();
        //    ////using (SqlDataReader reader = ExecuteReader(...))
        //    ////{

        //    //// }

           
        //        XmlTextWriter creditwriter = null;
        //    XmlTextWriter debitwriter = null;
        //    using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))

        //    {

        //        if (dr.HasRows)
        //                {
        //                        creditwriter = new XmlTextWriter(reportISSUERPath, Encoding.UTF8);
        //                        debitwriter = new XmlTextWriter(reportISSUERPath2, Encoding.UTF8);
        //                            while (dr.Read())
        //                              {
        //                               //var mm = dr["SN"].ToString();

        //                                if (dr["SN"].ToString().Trim() =="1")
        //                                {
        //                                    creditwriter.Formatting = Formatting.Indented;
        //                                    creditwriter.Indentation = 6;
        //                                    creditwriter.WriteStartDocument();
        //                                    creditwriter.WriteStartElement("Credit");

        //                                    creditwriter.WriteStartElement("Header");

        //                                    creditwriter.WriteStartElement("AcquiringInstitutionID");
        //                                    creditwriter.WriteString(dr["ACQUIRINGINSTITUTIONID"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("BatchNumber");
        //                                    creditwriter.WriteString(dr["BATCHNUM"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("BatchTotal");
        //                                    decimal total = Convert.ToDecimal(dr["TOTALBATCHAMT"].ToString());
        //                                    creditwriter.WriteString(total.ToString("0.00"));
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("POSTransactionDate");
        //                                    creditwriter.WriteString(dr["POSTRANSDATE"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("NumberOfRecords");
        //                                    creditwriter.WriteString(dr["VOLUME"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteEndElement();

        //                                 }

        //                                if (dr["SN"].ToString() == "3")
        //                                {
        //                                    creditwriter.WriteStartElement("Record");

        //                                    creditwriter.WriteStartElement("RecID");
        //                                    creditwriter.WriteString(dr["RECID"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("PayeeID");
        //                                    creditwriter.WriteString(dr["PAYEEID"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("PayeeAccount");
        //                                    creditwriter.WriteString(dr["PAYEEACCOUNT"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("PayeeBankCode");
        //                                    creditwriter.WriteString(dr["PAYEEBANKCODE"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("Amount");
        //                                    decimal amt = Convert.ToDecimal(dr["CR_AMOUNT"].ToString());
        //                                    creditwriter.WriteString(amt.ToString("0.00"));
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("PayeeName");
        //                                    creditwriter.WriteString(dr["PAYEENAME"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("PayerName");
        //                                    creditwriter.WriteString(dr["PAYERNAME"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteStartElement("Narration");
        //                                    creditwriter.WriteString(dr["NARATION"].ToString());
        //                                    creditwriter.WriteEndElement();

        //                                    creditwriter.WriteEndElement();

        //                           }

        //                                if (dr["SN"].ToString().Trim() == "2")
        //                                {
        //                                    debitwriter.Formatting = Formatting.Indented;
        //                                    debitwriter.Indentation = 6;
        //                                    debitwriter.WriteStartDocument();
        //                                    debitwriter.WriteStartElement("Debit");

        //                                    debitwriter.WriteStartElement("Header");

        //                                    debitwriter.WriteStartElement("AcquiringInstitutionID");
        //                                    debitwriter.WriteString(dr["ACQUIRINGINSTITUTIONID"].ToString());
        //                                    debitwriter.WriteEndElement();

        //                                    debitwriter.WriteStartElement("BatchNumber");
        //                                    debitwriter.WriteString(dr["BATCHNUM"].ToString());
        //                                    debitwriter.WriteEndElement();

        //                                    debitwriter.WriteStartElement("BatchTotal");
        //                                    decimal total = Convert.ToDecimal(dr["TOTALBATCHAMT"].ToString());
        //                                    debitwriter.WriteString(total.ToString("0.00"));
        //                                    debitwriter.WriteEndElement();

        //                                    debitwriter.WriteStartElement("POSTransactionDate");
        //                                    debitwriter.WriteString(dr["POSTRANSDATE"].ToString());
        //                                    debitwriter.WriteEndElement();

        //                                    debitwriter.WriteStartElement("NumberOfRecords");
        //                                    debitwriter.WriteString(dr["VOLUME"].ToString());
        //                                    debitwriter.WriteEndElement();

        //                                    debitwriter.WriteEndElement();

        //                                }

        //                                if (dr["SN"].ToString() == "4")
        //                                                    {
        //                                        debitwriter.WriteStartElement("Record");

        //                                        debitwriter.WriteStartElement("RecID");
        //                                        debitwriter.WriteString(dr["RECID"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("MerchantID");
        //                                        debitwriter.WriteString(dr["PAYEEID"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("PayerAccount");
        //                                        debitwriter.WriteString(dr["PAYERACCOUNT"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("PayerBankCode");
        //                                        debitwriter.WriteString(dr["PAYERBANKCODE"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("Amount");
        //                                        decimal amtdebit = Convert.ToDecimal(dr["DR_AMOUNT"].ToString());
        //                                        debitwriter.WriteString(amtdebit.ToString("0.00"));
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("MerchantName");
        //                                        debitwriter.WriteString(dr["PAYEENAME"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("PayerName");
        //                                        debitwriter.WriteString(dr["PAYERNAME"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteStartElement("Narration");
        //                                        debitwriter.WriteString(dr["NARATION"].ToString());
        //                                        debitwriter.WriteEndElement();

        //                                        debitwriter.WriteEndElement();
        //                                    }
        //                            }
        //                            creditwriter.Flush();
        //                            creditwriter.Close();
        //                            debitwriter.Flush();
        //                            debitwriter.Close();
        //                        }

        //                }
            
        //    return xml;
            

        //}


        public static string GenReport2D(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {

          
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + ".xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NIBSS BANKS");

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

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                //loop on cardscheme
                try
                {

                    var ws = package.Workbook.Worksheets.Add("NIBSS BANKS");
                    ws = generateIssuer(ws, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

              

                ////try
                ////{
                ////    var ws2 = package.Workbook.Worksheets.Add("NIBSS VISA");
                ////    ws2 = generateIssuer(ws2, logopath, null, null, null, "NIBSS_VISA", "U", sett, 1);


                ////}
                ////catch (Exception ex)
                ////{

                ////}


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }
            return newFile.FullName;

        }


        public static string GenReport3(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = "ERROR.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "ERROR");

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

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("ERROR");
                    ws = generateIssuer2(ws, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


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

        //public static string GenReport4(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        //{
        //    //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
        //    string sett = reportDate.ToString("dd-MMM-yyyy");


        //    string reportname = "Settlement.xlsx";





        //    string reportISSUERPath = ISSUERPath + "\\" + reportname;

        //    try
        //    {
        //        if (System.IO.File.Exists(reportISSUERPath))
        //        {

        //            System.IO.File.Delete(reportISSUERPath);

        //        }
        //    }
        //    catch
        //    {

        //    }



        //    //FileInfo newFile = new FileInfo(reportISSUERPath);
        //    //using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(newFile)))
        //    //using (ExcelPackage excelPackage = new ExcelPackage(ms))

        //        //loop on cardscheme
        //        try
        //        {
        //        int c = 0;
        //        bool firstTime = true;

        //        OracleConnection Standby_connection = new OracleConnection(oradb);
        //        string qry = "RPT_SETTLEMETDETAIL";
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.Connection = Standby_connection;
        //        // var dr = default(OracleDataReader);
        //        if (Standby_connection == null)
        //        {
        //            Standby_connection = new OracleConnection(oradb);
        //        }
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        cmd.Connection = Standby_connection;
        //        cmd.CommandText = qry;

        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = ID;
        //        cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = null;
        //        cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = "SETT";
        //        cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = "U";
        //        cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
        //        cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));


        //          OracleDataReader reader = cmd.ExecuteReader();

        //            DataTable dtSchema = reader.GetSchemaTable();
        //            var listCols = new List<DataColumn>();
        //            if (dtSchema != null)
        //            {
        //                foreach (DataRow drow in dtSchema.Rows)
        //                {
        //                    string columnName = Convert.ToString(drow["ColumnName"]);
        //                    var column = new DataColumn(columnName, (Type)(drow["DataType"]));
        //                    //column.Unique = (bool)drow["IsUnique"];
        //                    //column.AllowDBNull = (bool)drow["AllowDBNull"];
        //                    //column.AutoIncrement = (bool)drow["IsAutoIncrement"];
        //                    listCols.Add(column);
        //                    ResultsData.Columns.Add(column);
        //                }
        //            }

        //            // Call Read before accessing data. 
        //            while (reader.Read())
        //            {
        //                DataRow dataRow = ResultsData.NewRow();
        //                for (int i = 0; i < listCols.Count; i++)
        //                {
        //                    dataRow[(listCols[i])] = reader[i];
        //                }
        //                ResultsData.Rows.Add(dataRow);
        //                c++;
        //                if (c == rowsPerSheet)
        //                {
        //                    c = 0;
        //                    ExportToOxml(firstTime,reportISSUERPath);
        //                    ResultsData.Clear();
        //                    firstTime = false;
        //                }
        //            }
        //            if (ResultsData.Rows.Count > 0)
        //            {
        //            ExportToOxml(firstTime, reportISSUERPath);
        //            ResultsData.Clear();
        //            }
        //            // Call Close when done reading.
        //            reader.Close();

        //        }
        //        catch (Exception ex)
        //        {

        //        }


        //    return reportISSUERPath;
        //}

        public static string GenReport4(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = "Settlement.xlsx";





            string reportISSUERPath = ISSUERPath + "\\" + reportname;

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
            //using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(newFile)))
            //using (ExcelPackage excelPackage = new ExcelPackage(ms))
            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {

                    var ws = package.Workbook.Worksheets.Add("Settlement");
                    ws = generateIssuer4(ws, null, null, ID, null, null, "RECON2", null, 0, sett, 2);



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

        public static string GenReportNEWFiles(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {
            string sett = reportDate.ToString("dd-MMM-yyyy");


            FileInfo newFile = new FileInfo(reportPATH);
            //using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(newFile)))
            //using (ExcelPackage excelPackage = new ExcelPackage(ms))
            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {

                    var ws = package.Workbook.Worksheets.Add("Settlement");
                    ws = generateIssuer4(ws, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


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


        //public static string GenReportFILE2(string reportFor, string ID, DateTime reportDate, string ISSUERPath, string logopath, string reporttype)
        //{
        //    string sett = reportDate.ToString("dd-MMM-yyyy");
        //    string reportfile = string.Empty;
        //    MemoryStream stream = new MemoryStream();
        //    //create a package 
        //    using (var package = new ExcelPackage(stream)) // disposing ExcelPackage also disposes the above MemoryStream
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("worksheet");
        //        package.Save();

        //        // see the various ways to create/open a file, Create is just one of them
        //        // open the file stream
        //        using (var file = System.IO.File.Open(ISSUERPath, System.IO.FileMode.CreateNew))
        //        {
        //            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
        //            stream.Position = 0; // reset the position of the memory stream
        //            stream.CopyTo(file); // copy the memory stream to the file stream

        //            var query1 = (from cell in sheet.Cells["z:z"] select cell);
        //        }

        //        ////var query1 = (from cell in sheet.Cells["z:z"] select cell);


        //        ////int count = 0;
        //        ////foreach (var cell in query1)
        //        ////{
        //        ////    Console.WriteLine("Cell {0} has value {1:N0}", cell.Address, cell.Value);
        //        ////    count++;
        //        ////}
        //    }




        //    return reportfile;

        //}

        public static string GenReportFILE2(string reportFor, string ID, DateTime reportDate, string ISSUERPath, string logopath, string reporttype)
        {
            string sett = reportDate.ToString("dd-MMM-yyyy");
            string reportfile = string.Empty;


            ////using (ExcelPackage p = new ExcelPackage())
            ////{
            ////    using (FileStream stream = new FileStream(ISSUERPath, FileMode.Open))
            ////    {
            ////    ////p.Load(stream);
            ////    //////deleting worksheet if already present in excel file 
            ////    ////var wk = p.Workbook.Worksheets.SingleOrDefault(x => x.Name == reportFor);
            ////    ////if (wk != null) { p.Workbook.Worksheets.Delete(wk); }

            ////    ////p.Workbook.Worksheets.Add(reportFor);
            ////    ////p.Workbook.Worksheets.MoveToEnd(reportFor);
            ////    ////ExcelWorksheet ws = p.Workbook.Worksheets[p.Workbook.Worksheets.Count];
            ////    //p.Load(stream);

            ////   // ExcelWorksheet ws = p.Workbook.Worksheets.Add(reportFor);
            ////    p.Workbook.Worksheets.Add(reportFor);
            ////    p.Workbook.Worksheets.MoveToStart(reportFor);
            ////    ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ////    ws.Name = reportFor;
            ////    ws = generateIssuer4(ws, logopath, reportFor, ID, null, "SETT_M", "U", sett, 2);

            ////        //p.Save();
            ////    }

            ////var excelWorkbook = new FileInfo(ISSUERPath);
            ////using (var package = new ExcelPackage(excelWorkbook))
            ////{
            ////    ExcelWorksheet ws = package.Workbook.Worksheets.Add(reportFor);
            ////    ws = generateIssuer4(ws, logopath, reportFor, ID, null, "SETT_M", "U", sett, 2);

            ////    Byte[] bin = package.GetAsByteArray();
            ////    File.WriteAllBytes(ISSUERPath, bin);
            ////    Stream.Dispose();:package.Dispose();

            ////    // ws = package.Workbook.Worksheets.Count;

            ////}

            FileInfo newFile = new FileInfo(ISSUERPath);

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                var sheet = package.Workbook.Worksheets[1];
                var query1 = from cell in sheet.Cells["z:z"]        // a:a is the column a, Userid
                           where cell.Value.ToString().Equals(ID)  // x is the input userid
                           select sheet.Cells[cell.Start.Row, 1]; // 2 is column b, Email Address

                int count = 0;
                foreach (var cell in query1)
                {
                    Console.WriteLine("Cell {0} has value {1}", cell.Address, cell.Value);
                    count++;
                }

            }



            return reportfile;

        }


        public static string GenReportFILE(string reportFor, string ID, DateTime reportDate, string ISSUERPath, string logopath,string reporttype)
        {
            string sett = reportDate.ToString("dd-MMM-yyyy");
            string reportfile = string.Empty;


            ////using (ExcelPackage p = new ExcelPackage())
            ////{
            ////    using (FileStream stream = new FileStream(ISSUERPath, FileMode.Open))
            ////    {
            ////    ////p.Load(stream);
            ////    //////deleting worksheet if already present in excel file 
            ////    ////var wk = p.Workbook.Worksheets.SingleOrDefault(x => x.Name == reportFor);
            ////    ////if (wk != null) { p.Workbook.Worksheets.Delete(wk); }

            ////    ////p.Workbook.Worksheets.Add(reportFor);
            ////    ////p.Workbook.Worksheets.MoveToEnd(reportFor);
            ////    ////ExcelWorksheet ws = p.Workbook.Worksheets[p.Workbook.Worksheets.Count];
            ////    //p.Load(stream);

            ////   // ExcelWorksheet ws = p.Workbook.Worksheets.Add(reportFor);
            ////    p.Workbook.Worksheets.Add(reportFor);
            ////    p.Workbook.Worksheets.MoveToStart(reportFor);
            ////    ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ////    ws.Name = reportFor;
            ////    ws = generateIssuer4(ws, logopath, reportFor, ID, null, "SETT_M", "U", sett, 2);

            ////        //p.Save();
            ////    }

            ////var excelWorkbook = new FileInfo(ISSUERPath);
            ////using (var package = new ExcelPackage(excelWorkbook))
            ////{
            ////    ExcelWorksheet ws = package.Workbook.Worksheets.Add(reportFor);
            ////    ws = generateIssuer4(ws, logopath, reportFor, ID, null, "SETT_M", "U", sett, 2);

            ////    Byte[] bin = package.GetAsByteArray();
            ////    File.WriteAllBytes(ISSUERPath, bin);
            ////    Stream.Dispose();:package.Dispose();

            ////    // ws = package.Workbook.Worksheets.Count;

            ////}

            FileInfo newFile = new FileInfo(ISSUERPath);
             
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(reportFor);
                ws = generateIssuer4(ws, null, null, ID, null, null, "RECON2", null, 0, sett, 2);

                Byte[] bin = package.GetAsByteArray();
                File.WriteAllBytes(ISSUERPath, bin);
                  package.Dispose();


            }



            return reportfile;

        }


        //public static string GenReportSAXFILE(string reportFor, string ID, DateTime reportDate, string ReportPath, string logopath,string cardScheme,string reportType,string reportClass)
        //{
        //    string sett = reportDate.ToString("dd-MMM-yyyy");
        //    string reportfile = string.Empty;


        //    try
        //    {
        //        OracleConnection Standby_connection = new OracleConnection(oradb);
        //        string qry = "RPT_SETTLEMETDETAIL";
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.Connection = Standby_connection;
        //        // var dr = default(OracleDataReader);
        //        if (Standby_connection == null)
        //        {
        //            Standby_connection = new OracleConnection(oradb);
        //        }
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        cmd.Connection = Standby_connection;
        //        cmd.CommandText = qry;

        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = ID;
        //        cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = cardScheme;
        //        cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportType;
        //        cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportClass;
        //        cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
        //        cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));


        //        using (var dr = cmd.ExecuteReader())
        //        {

        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(ReportPath, true))
        //                    {
        //                        WorkbookPart workbookPart = myDoc.WorkbookPart;

        //                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        //                        string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);

        //                        WorksheetPart replacementPart = workbookPart.AddNewPart<WorksheetPart>();

        //                        string replacementPartId = workbookPart.GetIdOfPart(replacementPart);


        //                        OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
        //                        OpenXmlWriter writer = OpenXmlWriter.Create(replacementPart);

        //                        while (reader.Read())
        //                        {
        //                            if (reader.ElementType == typeof(SheetData))
        //                            {
        //                                if (reader.IsEndElement)
        //                                    continue;
        //                                writer.WriteStartElement(new SheetData());

        //                                Row rr = new Row();
        //                                writer.WriteStartElement(rr);

        //                                //Add Header          
        //                                for (int count = 0; count < dr.FieldCount; count++)
        //                                {
        //                                    String FieldName = dr.GetName(count);



        //                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dr.GetName(count));
        //                                    //headerRow.AppendChild(cell);

        //                                    writer.WriteElement(cell);
        //                                }

        //                                writer.WriteEndElement();


        //                                //writer.WriteEndElement();
        //                                //sheetData.AppendChild(headerRow);



        //                                while (dr.Read())
        //                                {
        //                                    //DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //                                    Row r = new Row();
        //                                    writer.WriteStartElement(r);

        //                                    for (int col = 0; col < dr.FieldCount; col++)
        //                                    {
        //                                        String FieldValue = dr.GetValue(col).ToString();

        //                                        //columns.Add(FieldValue);

        //                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(FieldValue);
        //                                        //newRow.AppendChild(cell);
        //                                        writer.WriteElement(cell);

        //                                    }
        //                                    //.AppendChild(newRow);
        //                                    writer.WriteEndElement();
        //                                }

        //                                writer.WriteEndElement();
        //                            }
        //                            else
        //                            {
        //                                if (reader.IsStartElement)
        //                                {
        //                                    writer.WriteStartElement(reader);
        //                                }
        //                                else if (reader.IsEndElement)
        //                                {
        //                                    writer.WriteEndElement();
        //                                }
        //                            }
        //                        }

        //                        reader.Close();
        //                        writer.Close();

        //                        Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Id.Value.Equals(origninalSheetId)).First();
        //                        sheet.Id.Value = replacementPartId;
        //                        workbookPart.DeletePart(worksheetPart);



        //                    }
        //                }

        //            }
        //          

        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return reportfile;

        //}


        //public static string GenReportDSFILE(string reportFor, string ID, DateTime reportDate, string ReportPath, string logopath, string cardScheme, string reportType, string reportClass,string subreporttype, int Channel,  int cnt, Sheets sheets, WorkbookPart workbookPart)
        //{
        //    string sett = reportDate.ToString("dd-MMM-yyyy");
        //    string reportfile = string.Empty;


        //    try
        //    {
        //        OracleConnection Standby_connection = new OracleConnection(oradb);
        //        string qry = "RPT_SETTLEMETDETAIL";
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.Connection = Standby_connection;
        //        // var dr = default(OracleDataReader);
        //        if (Standby_connection == null)
        //        {
        //            Standby_connection = new OracleConnection(oradb);
        //        }
        //        if (Standby_connection.State != ConnectionState.Open)
        //        {
        //            Standby_connection.Open();
        //        }
        //        cmd.Connection = Standby_connection;
        //        cmd.CommandText = qry;

        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = ID;
        //        cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = cardScheme;
        //        cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportType;
        //        cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = reportClass;
        //        cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
        //        cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));
        //        DataTable dt = new DataTable();
        //        DataSet ds = new DataSet();
        //        var dr = cmd.ExecuteReader();
        //        dsExcel drReport = new dsExcel();
        //        dt.Load(dr);
        //        ds.Tables.Add(dt);
        //      
        //         // drReport.ExportDataTOExcelMoreSheet(ds, ReportPath, reportFor, null, 0, cnt);
        //      drReport.ExportDataTOExcelMoreSheetSAX(ds, ReportPath, reportFor, null, 0, cnt, sheets,workbookPart);


        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    //return workbookPart;

        //}

        public static string GenReportSAXFILE(string reportFor, string ID, DateTime reportDate, string ReportPath, string logopath, string cardScheme, string reportType, string reportClass, string subreporttype, int Channel)
        {
            string sett = reportDate.ToString("dd-MMM-yyyy");
            string reportfile = string.Empty;


            try
            {
                dsLoadclass dv = new dsLoadclass();
                ////using (SqlDataReader reader = ExecuteReader(...))
                ////{

                //// }

                using (SqlDataReader dr = dv.generateDR(ID, cardScheme, reportType, reportClass, subreporttype, sett, Channel))
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(ReportPath, true))
                            {
                                WorkbookPart workbookPart = myDoc.WorkbookPart;

                                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                                string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);

                                WorksheetPart replacementPart = workbookPart.AddNewPart<WorksheetPart>();

                                string replacementPartId = workbookPart.GetIdOfPart(replacementPart);
                                SheetData sheetData1 = new SheetData();

                                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                                OpenXmlWriter writer = OpenXmlWriter.Create(replacementPart);

                                while (reader.Read())
                                {
                                    if (reader.ElementType == typeof(SheetData))
                                    {
                                        if (reader.IsEndElement)
                                            continue;
                                        writer.WriteStartElement(new SheetData());

                                        Row rr = new Row();
                                        writer.WriteStartElement(rr);

                                        //Add Header          
                                        for (int count = 0; count < dr.FieldCount; count++)
                                        {
                                            String FieldName = dr.GetName(count);

                                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dr.GetName(count));
                                            //headerRow.AppendChild(cell);

                                            writer.WriteElement(cell);
                                        }

                                        writer.WriteEndElement();


                                        //writer.WriteEndElement();
                                        //sheetData.AppendChild(headerRow);



                                        while (dr.Read())
                                        {
                                            //DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                                            Row r = new Row();
                                            writer.WriteStartElement(r);

                                            for (int col = 0; col < dr.FieldCount; col++)
                                            {
                                                String FieldValue = dr.GetValue(col).ToString();

                                                //columns.Add(FieldValue);

                                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(FieldValue);
                                                //newRow.AppendChild(cell);
                                                writer.WriteElement(cell);

                                            }
                                            //.AppendChild(newRow);
                                            writer.WriteEndElement();
                                        }

                                        writer.WriteEndElement();
                                    }
                                    else
                                    {
                                        if (reader.IsStartElement)
                                        {
                                            writer.WriteStartElement(reader);
                                        }
                                        else if (reader.IsEndElement)
                                        {
                                            writer.WriteEndElement();
                                        }
                                    }
                                }

                                reader.Close();
                                writer.Close();

                                //Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Id.Value.Equals(origninalSheetId)).First();
                                //sheet.Id.Value = replacementPartId;
                                //workbookPart.DeletePart(worksheetPart);

                                 
                                Sheets sheets = myDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                                Sheet sheet1 = new Sheet()
                                {
                                    Id = myDoc .WorkbookPart.GetIdOfPart(replacementPart),
                                    SheetId = 1,
                                    Name = reportFor
                                };
                                sheets.Append(sheet1);


                            }
                        }

                    }
                  

                }


            }
            catch (Exception ex)
            {

            }

            return reportfile;

        }


        static void WriteExcelSAX(string filename, int numRows, int numCols)
        {
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filename, true))
            {
                WorkbookPart workbookPart = myDoc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);

                WorksheetPart replacementPart =
                workbookPart.AddNewPart<WorksheetPart>();
                string replacementPartId = workbookPart.GetIdOfPart(replacementPart);

                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                OpenXmlWriter writer = OpenXmlWriter.Create(replacementPart);
                Row r = new Row();
                Cell c = new Cell();
                CellFormula f = new CellFormula();
                f.CalculateCell = true;
                f.Text = "RAND()";
                c.Append(f);
                CellValue v = new CellValue();
                c.Append(v);

                while (reader.Read())
                {
                    if (reader.ElementType == typeof(SheetData))
                    {
                        if (reader.IsEndElement)
                            continue;
                        writer.WriteStartElement(new SheetData());

                        for (int row = 0; row < numRows; row++)
                        {
                            writer.WriteStartElement(r);
                            for (int col = 0; col < numCols; col++)
                            {
                                writer.WriteElement(c);
                            }
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    else
                    {
                        if (reader.IsStartElement)
                        {
                            writer.WriteStartElement(reader);
                        }
                        else if (reader.IsEndElement)
                        {
                            writer.WriteEndElement();
                        }
                    }
                }
                reader.Close();
                writer.Close();

                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>()
                .Where(s => s.Id.Value.Equals(origninalSheetId)).First();
                sheet.Id.Value = replacementPartId;
                workbookPart.DeletePart(worksheetPart);
            }
        }

        ////public static string GenReport4(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        ////{
        ////    //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
        ////    string sett = reportDate.ToString("dd-MMM-yyyy");


        ////    string reportname = "Settlement.xlsx";





        ////    string reportISSUERPath = ISSUERPath + "\\" + reportname;

        ////    try
        ////    {
        ////        if (System.IO.File.Exists(reportISSUERPath))
        ////        {

        ////            System.IO.File.Delete(reportISSUERPath);

        ////        }
        ////    }
        ////    catch
        ////    {

        ////    }


        ////    const int max = 10;
        ////    var loop = 0;


        ////    OracleConnection Standby_connection = new OracleConnection(oradb);
        ////    string qry = "RPT_SETTLEMETDETAIL";
        ////    OracleCommand cmd = new OracleCommand();
        ////    cmd.Connection = Standby_connection;
        ////    // var dr = default(OracleDataReader);
        ////    if (Standby_connection == null)
        ////    {
        ////        Standby_connection = new OracleConnection(oradb);
        ////    }
        ////    if (Standby_connection.State != ConnectionState.Open)
        ////    {
        ////        Standby_connection.Open();
        ////    }
        ////    cmd.Connection = Standby_connection;
        ////    cmd.CommandText = qry;

        ////    cmd.CommandType = CommandType.StoredProcedure;

        ////    cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = ID;
        ////    cmd.Parameters.Add(new OracleParameter(":P_CardScheme", OracleDbType.Varchar2, ParameterDirection.Input)).Value = null;
        ////    cmd.Parameters.Add(new OracleParameter(":P_reporttype", OracleDbType.Varchar2, ParameterDirection.Input)).Value = "SETT";
        ////    cmd.Parameters.Add(new OracleParameter(":P_reportClass", OracleDbType.Varchar2, ParameterDirection.Input)).Value = "U";
        ////    cmd.Parameters.Add(new OracleParameter(":P_SETT", OracleDbType.Varchar2, ParameterDirection.Input)).Value = sett;
        ////    cmd.Parameters.Add(new OracleParameter(":CURSOR_ ", OracleDbType.RefCursor, ParameterDirection.Output));

        ////    using (var sdr = cmd.ExecuteReader())
        ////    {
        ////        var fieldcount = sdr.FieldCount;

        ////        var getfi = new Func<int, FileInfo>(i =>
        ////        {
        ////            var fi = new FileInfo(String.Format(reportISSUERPath, i));
        ////            if (fi.Exists) fi.Delete();
        ////            return fi;
        ////        });

        ////        var savefile = new Action<FileInfo, List<Object[]>>((info, rows) =>
        ////        {
        ////            using (var pck = new ExcelPackage(info))
        ////            {
        ////                var wb = pck.Workbook;
        ////                var ws = wb.Worksheets.Add("Settlement");
        ////                for (var row = 0; row < rows.Count; row++)
        ////                    for (var col = 0; col < fieldcount; col++)
        ////                        ws.SetValue(row + 1, col + 1, rows[row][col]);
        ////                pck.Save();
        ////            }
        ////        });

        ////        var rowlist = new List<Object[]>();

        ////        while (sdr.Read())
        ////        {
        ////            var rowdata = new Object[sdr.FieldCount];
        ////            sdr.GetValues(rowdata);
        ////            rowlist.Add(rowdata);

        ////            if (rowlist.Count == max)
        ////            {
        ////                savefile(getfi(++loop), rowlist);
        ////                rowlist.Clear();
        ////            }
        ////        }
        ////        if (rowlist.Count > 0)
        ////            savefile(getfi(++loop), rowlist);
        ////    }

        ////    return reportISSUERPath;

        ////    ////FileInfo newFile = new FileInfo(reportISSUERPath);

        ////    ////using (ExcelPackage package = new ExcelPackage())
        ////    ////{

        ////    ////    //loop on cardscheme
        ////    ////    try
        ////    ////    {

        ////    ////        var ws = package.Workbook.Worksheets.Add("Settlement");
        ////    ////        ws = generateIssuer4(ws, logopath, null, null, null, "SETT", "U", sett, 2);

        ////    ////        ////ExcelPackage pck = new ExcelPackage();
        ////    ////        ////DataSet ds1 = new DataSet();
        ////    ////        ////IDapperProcSettings SETTREPORT = new DapperProcSettings();
        ////    ////        ////ds1 = SETTREPORT.rptSettlementDetail(null, null, "SETT", reportDate, "U", null);
        ////    ////        ////if (ds1.Tables[0].Rows.Count > 0)
        ////    ////        ////{
        ////    ////        ////    DataTable dt = ds1.Tables[0];
        ////    ////        ////    var wsDt = pck.Workbook.Worksheets.Add("SettlementDetail");


        ////    ////        ////    wsDt.Cells["A1"].LoadFromDataTable(dt, true, TableStyles.None);

        ////    ////        ////    wsDt.Cells[wsDt.Dimension.Address].AutoFitColumns();

        ////    ////        ////}

        ////    ////        ////var fi = new FileInfo(newFile.FullName);
        ////    ////        ////if (fi.Exists)
        ////    ////        ////{
        ////    ////        ////    fi.Delete();
        ////    ////        ////}
        ////    ////        ////pck.SaveAs(fi);

        ////    ////    }
        ////    ////    catch (Exception ex)
        ////    ////    {

        ////    ////    }



        ////    ////    package.Compression = CompressionLevel.BestSpeed;
        ////    ////    package.SaveAs(newFile);
        ////    ////}
        ////    ////return newFile.FullName;


        ////}



        public static string GenReport5(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = "DailyReconcillation.xlsx";


            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "Reconcillation");

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

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {
                    var ws1 = package.Workbook.Worksheets.Add("TOTAL TRANSACTION WITH T464");
                    ws1 = generateIssuer5(ws1, null, null, ID, null, null, "RECON1", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                //loop on cardscheme
                try
                {
                    var ws2 = package.Workbook.Worksheets.Add("TOTAL TRANSACTION WITHOUT T464");
                    ws2 = generateIssuer5(ws2, null, null, ID, null, null, "RECON1B", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("T464 TOTAL TRANSACTION");
                    ws3 = generateIssuer5(ws3, null, null, ID, null, null, "RECON1C", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("EXCEPTION DETAIL");
                    ws4 = generateIssuer7(ws4, null, null, ID, null, null, "RECON3", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("STAKEHOLDERS BY DEPOSIT BANK");
                    ws5 = generateREPORT(ws5, logopath, null, null, null, "RECON5", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("MDB RECONCILLATION");
                    ws5 = generateREPORT(ws5, logopath, null, null, null, "RECON_MDB", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("ISSUER RECONCILLATION");
                    ws6= generateREPORT(ws6, logopath, null, null, null, "RECON_ISSR", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws7 = package.Workbook.Worksheets.Add("ACQUIRER RECONCILLATION");
                    ws7 = generateREPORT(ws7, logopath, null, null, null, "RECON_ACQR", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("PTSP RECONCILLATION");
                    ws8 = generateREPORTGRAPH(ws8, logopath, null, null, null, "RECON_PTSP", "U",null,0,sett, 2);

                    var chart = (ExcelBarChart)ws8.Drawings.AddChart("Chart", eChartType.ColumnClustered);
                    chart.SetSize(300, 300);
                    chart.SetPosition(50, 10);
                    chart.Title.Text = "PTSP RECONCILLATION";

                    chart.Series.Add(ExcelRange.GetAddress(2, 2, 50, 2), ExcelRange.GetAddress(2, 1, 50, 1));

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("PTSA RECONCILLATION");
                    ws9 = generateREPORTGRAPH(ws9, logopath, null, null, null, "RECON_PTSA", "U", null, 0, sett, 2);

                    //var chart = (ExcelBarChart)ws9.Drawings.AddChart("Chart", eChartType.ColumnClustered);
                    //chart.SetSize(300, 300);
                    //chart.SetPosition(50, 10);
                    //chart.Title.Text = "PTSA RECONCILLATION";

                    //chart.Series.Add(ExcelRange.GetAddress(2, 2, 50, 2), ExcelRange.GetAddress(2, 1, 50, 1));
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws10 = package.Workbook.Worksheets.Add("TERMINAL OWNER RECONCILLATION");
                    ws10 = generateREPORTGRAPH(ws10, logopath, null, null, null, "RECON_TERW", "U", null, 0, sett, 2);

                  
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws11 = package.Workbook.Worksheets.Add("SWITCH OWNER RECONCILLATION");
                    ws11= generateREPORTGRAPH(ws11, logopath, null, null, null, "RECON_SWTH", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws12 = package.Workbook.Worksheets.Add("NIBSS RECONCILLATION");
                    ws12 = generateIssuer5(ws12, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws13 = package.Workbook.Worksheets.Add("STAKEHOLDERS SUMMARY");
                    ws13 = generateREPORT(ws13, logopath, null, null, null, "RECON_SUMCON", "U", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                //////try
                //////{
                //////    var ws13 = package.Workbook.Worksheets.Add("CONSOLIDATED ACQUIRER SUMMARY");
                //////    ws13 = generateREPORT(ws13, logopath, null, null, null, "RECON_ACQRSUM", "U", sett, 2);


                //////}
                //////catch (Exception ex)
                //////{

                //////}


                ////try
                ////{
                ////    var ws14 = package.Workbook.Worksheets.Add("CONSOLIDATED ISSUER SUMMARY");
                ////    ws14 = generateREPORT(ws14, logopath, null, null, null, "RECON_ISSRSUM", "U", sett, 2);


                ////}
                ////catch (Exception ex)
                ////{

                ////}


                ////try
                ////{
                ////    var ws15 = package.Workbook.Worksheets.Add("CONSOLIDATED MDB SUMMARY");
                ////    ws15= generateREPORT(ws15, logopath, null, null, null, "RECON_MDBSUM", "U", sett, 2);


                ////}
                ////catch (Exception ex)
                ////{

                ////}

                try
                {
                    var ws20 = package.Workbook.Worksheets.Add("SETTLEMENT EVALUATION REPORT");
                    ws20 = generateIssuer6(ws20, null, null, ID, null, null, "RECON2", null, 0, sett, 2);


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
       
        public static string GenReport6(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string cardscheme)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = reportFor + " " + cardscheme;



            string oISSUERPath = ISSUERPath + "\\" + reportname + ".xlsx";

            try
            {
                if (System.IO.File.Exists(oISSUERPath))
                {

                    System.IO.File.Delete(oISSUERPath);

                }
            }
            catch
            {

            }



            FileInfo newFile = new FileInfo(oISSUERPath);

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {

                //loop on cardscheme
                try
                {
                    var ws = package.Workbook.Worksheets.Add("ZERO MSC");
                    ws = generateIssuer8(ws, null, null, null, cardscheme, ID, "", sett,0, null, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws2 = package.Workbook.Worksheets.Add("0.2");
                    ws2 = generateIssuer8(ws2, null, null, null, cardscheme, ID,null, null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws3 = package.Workbook.Worksheets.Add("0.75");
                    ws3= generateIssuer8(ws3, null, null, null, cardscheme, ID,null, null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws4 = package.Workbook.Worksheets.Add("1.25");
                    ws4 = generateIssuer8(ws4, null, null, null, cardscheme, ID, null, null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws5 = package.Workbook.Worksheets.Add("1.5");
                    ws5 = generateIssuer8(ws5, null, null, ID, cardscheme,null,null, null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws6 = package.Workbook.Worksheets.Add("2.0");
                    ws6 = generateIssuer8(ws6, null, null, ID, cardscheme, null, null, null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws7 = package.Workbook.Worksheets.Add("2.75");
                    ws7= generateIssuer8(ws7, null, null, ID, cardscheme, null, null, null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("3.0");
                    ws8= generateIssuer8(ws8, null, null, ID, cardscheme,null,"3.0",null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("3.25");
                    ws9 = generateIssuer8(ws9, null, null, ID, cardscheme, null, "3.25", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws10 = package.Workbook.Worksheets.Add("3.35");
                    ws10 = generateIssuer8(ws10, null, null, ID, cardscheme, null, "3.35", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }


                try
                {
                    var ws11 = package.Workbook.Worksheets.Add("3.5");
                    ws11 = generateIssuer8(ws11, null, null, ID, cardscheme, null, "3.5", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws12 = package.Workbook.Worksheets.Add("4.0");
                    ws12= generateIssuer8(ws12, null, null, ID, cardscheme, null, "4.0", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws13 = package.Workbook.Worksheets.Add("4.5");
                    ws13 = generateIssuer8(ws13, null, null, ID, cardscheme, null, "4.5", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws14 = package.Workbook.Worksheets.Add("5.0");
                    ws14 = generateIssuer8(ws14, null, null, ID, cardscheme, null, "5.0", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws15 = package.Workbook.Worksheets.Add("5.5");
                    ws15 = generateIssuer8(ws15, null, null, ID, cardscheme, null, "5.5", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws16 = package.Workbook.Worksheets.Add("6.0");
                    ws16 = generateIssuer8(ws16, null, null, ID, cardscheme, null, "6.0", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws17 = package.Workbook.Worksheets.Add("7.0");
                    ws17 = generateIssuer8(ws17, null, null, ID, cardscheme, null, "7.0", null, 0, sett, 2);


                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws18 = package.Workbook.Worksheets.Add("FLAT");
                    ws18 = generateIssuer8(ws18, null, null, ID, cardscheme, null, "99", null, 0, sett, 2);


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

        private static void ExportToOxml(bool firstTime, string fileName)
        {

            fileName = @"C:\MyExcel.xlsx";
            //Delete the file if it exists. 
            if (firstTime && File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            uint sheetId = 1; //Start at the first sheet in the Excel workbook.

            if (firstTime)
            {
                //This is the first time of creating the excel file and the first sheet.
                // Create a spreadsheet document by supplying the filepath.
                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                    Create(fileName, SpreadsheetDocumentType.Workbook);

                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);


                var bold1 = new Bold();
                CellFormat cf = new CellFormat();


                // Add Sheets to the Workbook.
                Sheets sheets;
                sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                        GetIdOfPart(worksheetPart),
                    SheetId = sheetId,
                    Name = "Sheet" + sheetId
                };
                sheets.Append(sheet);

                //Add Header Row.
                var headerRow = new Row();
                foreach (DataColumn column in ResultsData.Columns)
                {
                    var cell = new Cell { DataType = CellValues.String, CellValue = new CellValue(column.ColumnName) };
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                foreach (DataRow row in ResultsData.Rows)
                {
                    var newRow = new Row();
                    foreach (DataColumn col in ResultsData.Columns)
                    {
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(row[col].ToString())

                        };

                        object dataValue = row[col].ToString();


                        decimal n;
                        DateTime dt;
                        bool isNumeric = decimal.TryParse(dataValue.ToString(), out n);



                        if (isNumeric == true)
                        {

                            newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Number });



                        }
                        else if (DateTime.TryParse(dataValue.ToString(), out dt))
                        {
                            //newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Date });

                            DateTime dtValue;
                            string strValue = "";
                            if (DateTime.TryParse(dataValue.ToString(), out dtValue))
                                strValue = dtValue.ToString("dd-MMM-yyyy");
                            newRow.AppendChild(new Cell() { CellValue = new CellValue(strValue), DataType = CellValues.Date });
                        }
                        else
                        {
                            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                            string textvalin = cell.ToString();
                            string textvalout = Regex.Replace(textvalin, re, "");

                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                            newRow.AppendChild(cell);
                        }


                        ////string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                        ////string textvalin = cell.ToString();
                        ////string textvalout = Regex.Replace(textvalin, re, "");

                        ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                        ////newRow.AppendChild(cell);

                    }

                    sheetData.AppendChild(newRow);
                }
                workbookpart.Workbook.Save();

                spreadsheetDocument.Close();
            }
            else
            {
                // Open the Excel file that we created before, and start to add sheets to it.
                var spreadsheetDocument = SpreadsheetDocument.Open(fileName, true);

                var workbookpart = spreadsheetDocument.WorkbookPart;
                if (workbookpart.Workbook == null)
                    workbookpart.Workbook = new Workbook();

                var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);
                var sheets = spreadsheetDocument.WorkbookPart.Workbook.Sheets;

                if (sheets.Elements<Sheet>().Any())
                {
                    //Set the new sheet id
                    sheetId = sheets.Elements<Sheet>().Max(s => s.SheetId.Value) + 1;
                }
                else
                {
                    sheetId = 1;
                }

                // Append a new worksheet and associate it with the workbook.
                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                        GetIdOfPart(worksheetPart),
                    SheetId = sheetId,
                    Name = "Sheet" + sheetId
                };
                sheets.Append(sheet);

                //Add the header row here.
                var headerRow = new Row();

                foreach (DataColumn column in ResultsData.Columns)
                {
                    var cell = new Cell { DataType = CellValues.String, CellValue = new CellValue(column.ColumnName) };
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                foreach (DataRow row in ResultsData.Rows)
                {
                    var newRow = new Row();

                    foreach (DataColumn col in ResultsData.Columns)
                    {
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(row[col].ToString())
                        };

                        object dataValue = row[col].ToString();


                        decimal n;
                        DateTime dt;
                        bool isNumeric = decimal.TryParse(dataValue.ToString(), out n);



                        if (isNumeric == true)
                        {

                            newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Number });



                        }
                        else if (DateTime.TryParse(dataValue.ToString(), out dt))
                        {
                            //newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Date });

                            DateTime dtValue;
                            string strValue = "";
                            if (DateTime.TryParse(dataValue.ToString(), out dtValue))
                                strValue = dtValue.ToString("dd-MMM-yyyy");
                            newRow.AppendChild(new Cell() { CellValue = new CellValue(strValue), DataType = CellValues.Date });
                        }
                        else
                        {
                            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                            string textvalin = cell.ToString();
                            string textvalout = Regex.Replace(textvalin, re, "");

                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                            newRow.AppendChild(cell);
                        }

                        //string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                        //string textvalin = cell.ToString();
                        //string textvalout = Regex.Replace(textvalin, re, "");

                        //cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                        //newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbookpart.Workbook.Save();

                // Close the document.
                spreadsheetDocument.Close();
            }
        }

        //public static string GenReport1_DOLLAR(string reportFor, string ID, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath)
        //{

        //  string sett = reportDate.ToString("dd-MMM-yyyy");


        //    string reportname = "NIBSS VISA.xlsx";


        //    string oISSUERPath = System.IO.Path.Combine(ISSUERPath, "NIBSS VISA");

        //    if (!Directory.Exists(oISSUERPath))
        //    {
        //        System.IO.Directory.CreateDirectory(oISSUERPath);
        //    }




        //    string reportISSUERPath = oISSUERPath + "\\" + reportname;

        //    try
        //    {
        //        if (System.IO.File.Exists(reportISSUERPath))
        //        {

        //            System.IO.File.Delete(reportISSUERPath);

        //        }
        //    }
        //    catch
        //    {

        //    }



        //    FileInfo newFile = new FileInfo(reportISSUERPath);

        //    using (ExcelPackage package = new ExcelPackage())
        //    {

        //        //loop on cardscheme



        //        package.Compression = CompressionLevel.BestSpeed;
        //        package.SaveAs(newFile);
        //    }
        //    return newFile.FullName;

        //}


    }
}
