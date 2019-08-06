
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
using Generic.Dapper.Data;
using OfficeOpenXml.Table;

namespace Generic.Dapper.ReportClass
{
    class rptMerchants
    {
        private static string oradb = System.Configuration.ConfigurationManager.AppSettings["TestDB"].ToString();

    
        public static ExcelWorksheet genMerchant(ExcelWorksheet ws, string img, string rName, string ID, string SettlementCur,string cardScheme, string reportType, string reportClass, string sett, int startRow,string DOMINT)
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

            //Write the headers and style them
            ws.Cells["A2"].Value = "Unified Payment Services Ltd";
            ws.Cells["A2"].Style.Font.Size = 18;
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A2:M2"].Merge = true;
            ws.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            String CARDSCHEMDESC = String.Empty;
            if (cardScheme.ToUpper() == "PAYA")
            {
                CARDSCHEMDESC = "PAYALTITUDE";
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
            ws.Cells["A4"].Value = "MERCHANT INCOME " + CARDSCHEMDESC;
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
            ws.Cells["A7"].Style.Font.Size = 14;
            ws.Cells["A7"].Style.Font.Bold = true;
          

            ws.View.FreezePanes(11, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])

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


                            ////row++;
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
            ws.Cells[2, 21, row + 1, 21].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";
          


            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";



            ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);


            return ws;


        }


        public static ExcelWorksheet genMerchant2(ExcelWorksheet ws, string img, string MERCHANTID, string MERCHANTNAME, string ADDRESS, string SettlementCur,string SETTLEMENTACCOUNT, string BANKNAME, string cardScheme, string reportType, string reportClass, string sett, int startRow, string DOMINT )
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

            //Write the headers and style them
            ws.Cells["A2"].Value = "Unified Payment Services Ltd";
            ws.Cells["A2"].Style.Font.Size = 18;
            ws.Cells["A2"].Style.Font.Bold = true;
            ws.Cells["A2:M2"].Merge = true;
            ws.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            ws.Cells["A3"].Value = "MERCHANT TRANSACTION ";
            ws.Cells["A3"].Style.Font.Size = 14;
            ws.Cells["A3"].Style.Font.Bold = true;
            ws.Cells["A3:M3"].Merge = true;
            ws.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
            ws.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
            ws.Cells["A5"].Style.Font.Size = 10;
            ws.Cells["A5:F5"].Merge = true;
            //  ws.Cells["A5"].Style.Font.Bold = true;

            ws.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
            ws.Cells["A6"].Style.Font.Size = 10;
            ws.Cells["A6:F6"].Merge = true;

            ws.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
            ws.Cells["A7"].Style.Font.Size = 10;
            ws.Cells["A7"].Style.Font.Bold = true;
           
            // ws.Cells["A6"].Style.Font.Bold = true;

            ////ws.Cells["A7"].Value = "MERCHANT BANK: " + BANKNAME;
            ////ws.Cells["A7"].Style.Font.Size = 10;
            ////ws.Cells["A7:F7"].Merge = true;
            //////ws.Cells["A7"].Style.Font.Bold = true;

            ////ws.Cells["A8"].Value = "MERCHANT ACCOUNT: " + SETTLEMENTACCOUNT;
            ////ws.Cells["A8"].Style.Font.Size = 10;
            ////ws.Cells["A8:F8"].Merge = true;
            // ws.Cells["A8"].Style.Font.Bold = true;


            ws.View.FreezePanes(11, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])
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

                cmd.Parameters.Add(new OracleParameter(":P_searchID", OracleDbType.Varchar2, ParameterDirection.Input)).Value = MERCHANTID;
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


                            ////row++;
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
            ws.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy";
           

            ws.Cells[2, 21, row + 1, 21].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[2, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";


            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[row + 1, 21, row + 1, 21].Formula = string.Format("=SUM(U{0}:U{1})", startRow, row-1);
            
            ws.Cells[row + 1, 23, row + 1, 26].Formula = string.Format("=SUM(W{0}:W{1})", startRow, row);
            ws.Cells[row + 1, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";

 

   


            ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);


            return ws;


        }

        public static ExcelWorksheet genMerchant3(ExcelWorksheet ws, string img, string rName, string ID,string SettlementCur, string cardScheme, string reportType, string reportClass, string sett, int startRow, string DOMINT)
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

            ws.Cells["A4"].Value = "MERCHANT SUMMARY ";
            ws.Cells["A4"].Style.Font.Size = 14;
            ws.Cells["A4"].Style.Font.Bold = true;
            ws.Cells["A4:M4"].Merge = true;
            ws.Cells["A4:M4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
            ws.Cells["A6"].Value = rName;
            ws.Cells["A6"].Style.Font.Size = 14;
            ws.Cells["A6"].Style.Font.Bold = true;


            ws.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
            ws.Cells["A7"].Style.Font.Size = 10;
            ws.Cells["A7"].Style.Font.Bold = true;
           


            // ws.View.FreezePanes(11, 1);

            // using (var rng = ws.Cells["A" + (startRow - 1) + ":Z" + (startRow - 1)])

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


                            ////row++;
                        }

                    }
                    cmd.Dispose();

                }


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            ////ws.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
            ////ws.Cells[2, 5, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            ////ws.Cells[2, 8, row + 1, 8].Style.Numberformat.Format = "dd-mm-yyyy";
            ////ws.Cells[2, 44, row + 1, 54].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[2, 57, row + 1, 58].Style.Numberformat.Format = "#,##0.00";
            ////ws.Cells[2, 60, row + 1, 77].Style.Numberformat.Format = "#,##0.00";


            ws.Cells[row + 1, 1].Value = "TOTAL";
            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Formula = string.Format("=SUM(B{0}:B{1})", startRow, row);

            ws.Cells[row + 1, 2, row + 1, columnIDcnt - 1].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[startRow, 1, row, 80].AutoFitColumns(20);


            return ws;


        }



        public static string GenReport3(string MERCHANTID, string MERCHANTNAME, string ADDRESS, string SettlementCur, string SETTLEMENTACCOUNT, string BANKNAME, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath, string reportFolder, string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = MERCHANTNAME + ".xlsx";



            string oISSUERPath = ISSUERPath;

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, MERCHANTNAME);

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
                DataTable dtMain = null;

                try
                {

                    var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "DETAIL_ARTE", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);

                    //ExcelRange cols = wsA.Cells["A:XFD"];
                    //wsA.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    var range = wsA.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    //wsA.Tables.Add(range, "data");
                    //Write the headers and style them
                    wsA.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA.Cells["A2"].Style.Font.Size = 18;
                    wsA.Cells["A2"].Style.Font.Bold = true;
                    wsA.Cells["A2:M2"].Merge = true;
                    wsA.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA.Cells["A3"].Style.Font.Size = 14;
                    wsA.Cells["A3"].Style.Font.Bold = true;
                    wsA.Cells["A3:M3"].Merge = true;
                    wsA.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA.Cells["A5"].Style.Font.Size = 10;
                    wsA.Cells["A5:F5"].Merge = true;
                    wsA.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA.Cells["A6"].Style.Font.Size = 10;
                    wsA.Cells["A6:F6"].Merge = true;

                    wsA.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
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
                        wsA.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                        wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy";


                        wsA.Cells[2, 21, row + 1, 21].Style.Numberformat.Format = "#,##0.00";
                        wsA.Cells[2, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";


                        wsA.Cells[row + 1, 1].Value = "TOTAL";
                        wsA.Cells[row + 1, 21, row + 1, 21].Formula = string.Format("=SUM(U{0}:U{1})", 12, row - 1);

                        wsA.Cells[row + 1, 23, row + 1, 26].Formula = string.Format("=SUM(W{0}:W{1})", 12, row - 1);
                        wsA.Cells[row + 1, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";

                    }

                    wsA.View.FreezePanes(11, 1);
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
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH_DETAIL", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();

                try
                {

                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "MERCH_EXCEP_ARTE", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);
                    var wsA2 = package.Workbook.Worksheets.Add("EXCEPTION");
                    //ExcelRange cols = wsA2.Cells["A:XFD"];
                    //wsA2.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA2.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    var range2 = wsA2.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA2.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA2.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    //wsA2.Tables.Add(range2, "data2");



                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA2.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    //Write the headers and style them
                    wsA2.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA2.Cells["A2"].Style.Font.Size = 18;
                    wsA2.Cells["A2"].Style.Font.Bold = true;
                    wsA2.Cells["A2:M2"].Merge = true;
                    wsA2.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA2.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA2.Cells["A3"].Style.Font.Size = 14;
                    wsA2.Cells["A3"].Style.Font.Bold = true;
                    wsA2.Cells["A3:M3"].Merge = true;
                    wsA2.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA2.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA2.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA2.Cells["A5"].Style.Font.Size = 10;
                    wsA2.Cells["A5:F5"].Merge = true;
                    wsA2.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA2.Cells["A6"].Style.Font.Size = 10;
                    wsA2.Cells["A6:F6"].Merge = true;

                    wsA2.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA2.Cells["A7"].Style.Font.Size = 10;
                    wsA2.Cells["A7"].Style.Font.Bold = true;

                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA2.Dimension.End.Row + 1;

                        wsA2.Cells[row + 1, 1].Value = "TOTAL";
                        wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                        wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Style.Numberformat.Format = "#,##0.00";

                    }
                    wsA2.View.FreezePanes(11, 1);
                    // var tbl2 = wsA2.Tables[0];
                    if (range2 != null)
                    {
                        range2.AutoFitColumns();
                    }
                    // tbl2.ShowFilter = false;
                    //wsA2.Cells[wsA2.Dimension.Address].AutoFilter = true;

                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH_EXCEPTION", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();
                try
                {


                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "MERCH_SUMM_ARTE", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);
                    var wsA3 = package.Workbook.Worksheets.Add("SUMMARY");

                    //ExcelRange cols = wsA3.Cells["A:XFD"];
                    //wsA3.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA3.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);
                    var range3 = wsA3.Cells["A11"].LoadFromDataTable(dtMain, true);
                    //wsA3.Tables.Add(range3, "data3");
                    wsA3.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA3.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA3.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    //Write the headers and style them
                    wsA3.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA3.Cells["A2"].Style.Font.Size = 18;
                    wsA3.Cells["A2"].Style.Font.Bold = true;
                    wsA3.Cells["A2:M2"].Merge = true;
                    wsA3.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA3.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA3.Cells["A3"].Style.Font.Size = 14;
                    wsA3.Cells["A3"].Style.Font.Bold = true;
                    wsA3.Cells["A3:M3"].Merge = true;
                    wsA3.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA3.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA3.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA3.Cells["A5"].Style.Font.Size = 10;
                    wsA3.Cells["A5:F5"].Merge = true;
                    wsA3.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA3.Cells["A6"].Style.Font.Size = 10;
                    wsA3.Cells["A6:F6"].Merge = true;

                    wsA3.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA3.Cells["A7"].Style.Font.Size = 10;
                    wsA3.Cells["A7"].Style.Font.Bold = true;
                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA3.Dimension.End.Row + 1;
                        wsA3.Cells[row + 1, 1].Value = "TOTAL";
                        wsA3.Cells[row + 1, 2, row + 1, 6 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                        wsA3.Cells[row + 1, 2, row + 1, 6 - 1].Style.Numberformat.Format = "#,##0.00";

                    }
                    wsA3.View.FreezePanes(11, 1);
                    // var tbl3 = wsA3.Tables[0];
                    if (range3 != null)
                    {
                        range3.AutoFitColumns();
                    }
                    //tbl3.ShowFilter = false;
                    //wsA3.Cells[wsA3.Dimension.Address].AutoFilter = true;

                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH-SUMMARY", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);
                }
                dtMain.Dispose();

                /*    
                try
                {
                    var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    wsA = genMerchant2(wsA, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SettlementCur, SETTLEMENTACCOUNT, BANKNAME, null, "DETAIL", "M", sett, 11, reportFolderType);


                }
                catch (Exception ex)
                {

                }
                //loop on cardscheme
                //////try
                //////{
                //////    var ws = package.Workbook.Worksheets.Add("VISA DR");
                //////    ////ws = genMerchant(ws, logopath, MERCHANTNAME, MERCHANTID, "VISA", "MERCH_DR", "M", sett, 8);
                //////    ws = genMerchant2(ws, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_DR", "M", sett, 11, reportFolderType);


                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{

                //////    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                //////    //ws2 = genMerchant(ws2, logopath, MERCHANTNAME, MERCHANTID, "VISA", "MERCH_CR", "M", sett, 8);
                //////    ws2 = genMerchant2(ws2, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws3 = package.Workbook.Worksheets.Add("MASTERCARD DR");
                //////    //ws3 = genMerchant(ws3, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", "M", sett, 8);

                //////    ws3 = genMerchant2(ws3, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "MAST", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws4 = package.Workbook.Worksheets.Add("MASTERCARD CR");
                //////    //ws4 = genMerchant(ws4, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", "M", sett, 8);
                //////    ws4 = genMerchant2(ws4, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "MAST", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}


                //////try
                //////{
                //////    var ws3 = package.Workbook.Worksheets.Add("VERVE DR");
                //////    //ws3 = genMerchant(ws3, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", "M", sett, 8);

                //////    ws3 = genMerchant2(ws3, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VERV", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws4 = package.Workbook.Worksheets.Add("VERVE CR");
                //////    //ws4 = genMerchant(ws4, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", "M", sett, 8);
                //////    ws4 = genMerchant2(ws4, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VERV", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}
                //////try
                //////{
                //////    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                //////    // ws5 = genMerchant(ws5, logopath, MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_DR", "M", sett, 8);
                //////    ws5 = genMerchant2(ws5, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "PAYA", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                //////    // ws6 = genMerchant(ws6, logopath, MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_CR", "M", sett, 8);
                //////    ws6 = genMerchant2(ws6, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "PAYA", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}





                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("EXCEPTION");
                    ws8 = genMerchant(ws8, logopath, MERCHANTNAME, MERCHANTID, SettlementCur,null, "MERCH_EXCEP", "M", sett, 11, reportFolderType);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = genMerchant3(ws9, logopath, MERCHANTNAME, MERCHANTID, SettlementCur,null, "MERCH_SUMM", "M", sett, 11, reportFolderType);
                }
                catch (Exception ex)
                {

                }


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
                */

                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }

            return newFile.FullName;



        }

        public static string GenReportvip(string MERCHANTID, string MERCHANTNAME, string ADDRESS, string SettlementCur, string SETTLEMENTACCOUNT, string BANKNAME, DateTime reportDate, string reportPATH, string ISSUERPath, string logopath, string reportFolder, string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = MERCHANTNAME + "_" + sett + ".xlsx";



            string oISSUERPath = ISSUERPath;

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, MERCHANTNAME);

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
            LogFunction2.WriteMaintenaceLogToFile("generating excel :{0}");
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                DataTable dtMain = null;

                try
                {

                    var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "DETAIL_ARTE", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);

                    //ExcelRange cols = wsA.Cells["A:XFD"];
                    //wsA.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    var range = wsA.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    var cl =  Color.Orange;
                    try {
                        if (cl != null)
                        {
                            wsA.Row(11).Style.Fill.BackgroundColor.SetColor(cl);
                        }
                    }
                    catch
                    {

                    }


                    //wsA.Tables.Add(range, "data");
                    //Write the headers and style them
                    wsA.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA.Cells["A2"].Style.Font.Size = 18;
                    wsA.Cells["A2"].Style.Font.Bold = true;
                    wsA.Cells["A2:M2"].Merge = true;
                    wsA.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA.Cells["A3"].Style.Font.Size = 14;
                    wsA.Cells["A3"].Style.Font.Bold = true;
                    wsA.Cells["A3:M3"].Merge = true;
                    wsA.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA.Cells["A5"].Style.Font.Size = 10;
                    wsA.Cells["A5:F5"].Merge = true;
                    wsA.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA.Cells["A6"].Style.Font.Size = 10;
                    wsA.Cells["A6:F6"].Merge = true;

                    wsA.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
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
                        wsA.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                        wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy";


                        wsA.Cells[2, 21, row + 1, 21].Style.Numberformat.Format = "#,##0.00";
                        wsA.Cells[2, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";


                        wsA.Cells[row + 1, 1].Value = "TOTAL";
                        wsA.Cells[row + 1, 21, row + 1, 21].Formula = string.Format("=SUM(U{0}:U{1})", 12, row - 1);

                        wsA.Cells[row + 1, 23, row + 1, 26].Formula = string.Format("=SUM(W{0}:W{1})", 12, row - 1);
                        wsA.Cells[row + 1, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";

                    }

                    wsA.View.FreezePanes(11, 1);
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
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH_DETAIL", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();

                try
                {

                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "MERCH_EXCEP_ARTE", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);
                    var wsA2 = package.Workbook.Worksheets.Add("EXCEPTION");
                    //ExcelRange cols = wsA2.Cells["A:XFD"];
                    //wsA2.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA2.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    var range2 = wsA2.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA2.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA2.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    //wsA2.Tables.Add(range2, "data2");



                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA2.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    //Write the headers and style them
                    wsA2.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA2.Cells["A2"].Style.Font.Size = 18;
                    wsA2.Cells["A2"].Style.Font.Bold = true;
                    wsA2.Cells["A2:M2"].Merge = true;
                    wsA2.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA2.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA2.Cells["A3"].Style.Font.Size = 14;
                    wsA2.Cells["A3"].Style.Font.Bold = true;
                    wsA2.Cells["A3:M3"].Merge = true;
                    wsA2.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA2.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA2.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA2.Cells["A5"].Style.Font.Size = 10;
                    wsA2.Cells["A5:F5"].Merge = true;
                    wsA2.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA2.Cells["A6"].Style.Font.Size = 10;
                    wsA2.Cells["A6:F6"].Merge = true;

                    wsA2.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA2.Cells["A7"].Style.Font.Size = 10;
                    wsA2.Cells["A7"].Style.Font.Bold = true;

                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA2.Dimension.End.Row + 1;

                        wsA2.Cells[row + 1, 1].Value = "TOTAL";
                        wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                        wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Style.Numberformat.Format = "#,##0.00";

                    }
                    wsA2.View.FreezePanes(11, 1);
                    // var tbl2 = wsA2.Tables[0];
                    if (range2 != null)
                    {
                        range2.AutoFitColumns();
                    }
                    // tbl2.ShowFilter = false;
                    //wsA2.Cells[wsA2.Dimension.Address].AutoFilter = true;

                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH_EXCEPTION", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();
                try
                {


                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "MERCH_SUMM_ARTE", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);
                    var wsA3 = package.Workbook.Worksheets.Add("SUMMARY");

                    //ExcelRange cols = wsA3.Cells["A:XFD"];
                    //wsA3.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA3.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);
                    var range3 = wsA3.Cells["A11"].LoadFromDataTable(dtMain, true);
                    //wsA3.Tables.Add(range3, "data3");
                    wsA3.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA3.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA3.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    //Write the headers and style them
                    wsA3.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA3.Cells["A2"].Style.Font.Size = 18;
                    wsA3.Cells["A2"].Style.Font.Bold = true;
                    wsA3.Cells["A2:M2"].Merge = true;
                    wsA3.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA3.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA3.Cells["A3"].Style.Font.Size = 14;
                    wsA3.Cells["A3"].Style.Font.Bold = true;
                    wsA3.Cells["A3:M3"].Merge = true;
                    wsA3.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA3.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA3.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA3.Cells["A5"].Style.Font.Size = 10;
                    wsA3.Cells["A5:F5"].Merge = true;
                    wsA3.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA3.Cells["A6"].Style.Font.Size = 10;
                    wsA3.Cells["A6:F6"].Merge = true;

                    wsA3.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA3.Cells["A7"].Style.Font.Size = 10;
                    wsA3.Cells["A7"].Style.Font.Bold = true;
                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA3.Dimension.End.Row + 1;
                        wsA3.Cells[row + 1, 1].Value = "TOTAL";
                        wsA3.Cells[row + 1, 2, row + 1, 6 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                        wsA3.Cells[row + 1, 2, row + 1, 6 - 1].Style.Numberformat.Format = "#,##0.00";

                    }
                    wsA3.View.FreezePanes(11, 1);
                    // var tbl3 = wsA3.Tables[0];
                    if (range3 != null)
                    {
                        range3.AutoFitColumns();
                    }
                    //tbl3.ShowFilter = false;
                    //wsA3.Cells[wsA3.Dimension.Address].AutoFilter = true;

                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH-SUMMARY", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);
                }
                dtMain.Dispose();

                /*    
                try
                {
                    var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    wsA = genMerchant2(wsA, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SettlementCur, SETTLEMENTACCOUNT, BANKNAME, null, "DETAIL", "M", sett, 11, reportFolderType);


                }
                catch (Exception ex)
                {

                }
                //loop on cardscheme
                //////try
                //////{
                //////    var ws = package.Workbook.Worksheets.Add("VISA DR");
                //////    ////ws = genMerchant(ws, logopath, MERCHANTNAME, MERCHANTID, "VISA", "MERCH_DR", "M", sett, 8);
                //////    ws = genMerchant2(ws, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_DR", "M", sett, 11, reportFolderType);


                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{

                //////    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                //////    //ws2 = genMerchant(ws2, logopath, MERCHANTNAME, MERCHANTID, "VISA", "MERCH_CR", "M", sett, 8);
                //////    ws2 = genMerchant2(ws2, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws3 = package.Workbook.Worksheets.Add("MASTERCARD DR");
                //////    //ws3 = genMerchant(ws3, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", "M", sett, 8);

                //////    ws3 = genMerchant2(ws3, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "MAST", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws4 = package.Workbook.Worksheets.Add("MASTERCARD CR");
                //////    //ws4 = genMerchant(ws4, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", "M", sett, 8);
                //////    ws4 = genMerchant2(ws4, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "MAST", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}


                //////try
                //////{
                //////    var ws3 = package.Workbook.Worksheets.Add("VERVE DR");
                //////    //ws3 = genMerchant(ws3, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", "M", sett, 8);

                //////    ws3 = genMerchant2(ws3, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VERV", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws4 = package.Workbook.Worksheets.Add("VERVE CR");
                //////    //ws4 = genMerchant(ws4, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", "M", sett, 8);
                //////    ws4 = genMerchant2(ws4, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VERV", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}
                //////try
                //////{
                //////    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                //////    // ws5 = genMerchant(ws5, logopath, MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_DR", "M", sett, 8);
                //////    ws5 = genMerchant2(ws5, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "PAYA", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                //////    // ws6 = genMerchant(ws6, logopath, MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_CR", "M", sett, 8);
                //////    ws6 = genMerchant2(ws6, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "PAYA", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}





                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("EXCEPTION");
                    ws8 = genMerchant(ws8, logopath, MERCHANTNAME, MERCHANTID, SettlementCur,null, "MERCH_EXCEP", "M", sett, 11, reportFolderType);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = genMerchant3(ws9, logopath, MERCHANTNAME, MERCHANTID, SettlementCur,null, "MERCH_SUMM", "M", sett, 11, reportFolderType);
                }
                catch (Exception ex)
                {

                }


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
                */

                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }

            return newFile.FullName;



        }


        public static string GenReport2(string MERCHANTID,string MERCHANTNAME, string ADDRESS,string SettlementCur, string SETTLEMENTACCOUNT, string BANKNAME, DateTime reportDate, string reportPATH, string ISSUERPath,  string logopath, string reportFolder, string reportFolderType)
        {
            //FileInfo template = new FileInfo(reportPATH + @"\TEMPLATES\Issuer Report Template.xlsx");
            string sett = reportDate.ToString("dd-MMM-yyyy");


            string reportname = MERCHANTNAME + ".xlsx";



            string oISSUERPath = System.IO.Path.Combine(ISSUERPath, reportFolder);

            if (!Directory.Exists(oISSUERPath))
            {
                System.IO.Directory.CreateDirectory(oISSUERPath);
            }

            string oISSUERPath2 = System.IO.Path.Combine(oISSUERPath, MERCHANTNAME);

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

           //ws = genMerchant2(ws, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_DR", "M", sett, 11, reportFolderType);

           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "VISA", "MERCH_DR", reportFolderType, "M", sett, reportISSUERPath, logopath,1);
           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "VISA", "MERCH_CR", reportFolderType, "M", sett, reportISSUERPath, logopath,2);


           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", reportFolderType, "M", sett, reportISSUERPath, logopath, 3);
           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", reportFolderType, "M", sett, reportISSUERPath, logopath, 4);

           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_DR", reportFolderType, "M", sett, reportISSUERPath, logopath, 5);
           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_CR", reportFolderType, "M", sett, reportISSUERPath, logopath, 6);

           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "CUPI", "MERCH_DR", reportFolderType, "M", sett, reportISSUERPath, logopath, 7);
           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "CUPI", "MERCH_CR", reportFolderType, "M", sett, reportISSUERPath, logopath, 8);

           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "VERV", "MERCH_DR", reportFolderType, "M", sett, reportISSUERPath, logopath, 9);
           //////// generateDSReport(MERCHANTNAME, MERCHANTID, "VERV", "MERCH_CR", reportFolderType, "M", sett, reportISSUERPath, logopath, 10);


           //////// generateDSReport(MERCHANTNAME, MERCHANTID,null, "MERCH_EXCEP", reportFolderType, "M", sett, reportISSUERPath, logopath, 11);
           //////// generateDSReport(MERCHANTNAME, MERCHANTID,null, "MERCH_SUMM", reportFolderType, "M", sett, reportISSUERPath, logopath, 12);



           //////// //////        ws8 = genMerchant(ws8, logopath, MERCHANTNAME, MERCHANTID, null, "MERCH_EXCEP", "M", sett, 8, reportFolderType);
           //////////// ws9 = genMerchant3(ws9, logopath, MERCHANTNAME, MERCHANTID, null, "MERCH_SUMM", "M", sett, 8, reportFolderType);
           //////// return null;

           
            FileInfo newFile = new FileInfo(reportISSUERPath);

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                DataTable dtMain = null;

                try
                {
                   
                    var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "DETAIL", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);

                    //ExcelRange cols = wsA.Cells["A:XFD"];
                    //wsA.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    var range = wsA.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    //wsA.Tables.Add(range, "data");
                    //Write the headers and style them
                    wsA.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA.Cells["A2"].Style.Font.Size = 18;
                    wsA.Cells["A2"].Style.Font.Bold = true;
                    wsA.Cells["A2:M2"].Merge = true;
                    wsA.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA.Cells["A3"].Style.Font.Size = 14;
                    wsA.Cells["A3"].Style.Font.Bold = true;
                    wsA.Cells["A3:M3"].Merge = true;
                    wsA.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA.Cells["A5"].Style.Font.Size = 10;
                    wsA.Cells["A5:F5"].Merge = true;
                     wsA.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA.Cells["A6"].Style.Font.Size = 10;
                    wsA.Cells["A6:F6"].Merge = true;

                    wsA.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
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
                        wsA.Cells[2, 3, row + 1, 3].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                        wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy";


                        wsA.Cells[2, 21, row + 1, 21].Style.Numberformat.Format = "#,##0.00";
                        wsA.Cells[2, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";


                        wsA.Cells[row + 1, 1].Value = "TOTAL";
                        wsA.Cells[row + 1, 21, row + 1, 21].Formula = string.Format("=SUM(U{0}:U{1})", 12, row - 1);

                        wsA.Cells[row + 1, 23, row + 1, 26].Formula = string.Format("=SUM(W{0}:W{1})", 12, row - 1);
                        wsA.Cells[row + 1, 23, row + 1, 26].Style.Numberformat.Format = "#,##0.00";

                    }

                    wsA.View.FreezePanes(11, 1);
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
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH_DETAIL", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();

                try
                {
             
                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "MERCH_EXCEP", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);
                    var wsA2 = package.Workbook.Worksheets.Add("EXCEPTION");
                    //ExcelRange cols = wsA2.Cells["A:XFD"];
                    //wsA2.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA2.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);

                    var range2 = wsA2.Cells["A11"].LoadFromDataTable(dtMain, true);

                    wsA2.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA2.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    //wsA2.Tables.Add(range2, "data2");



                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA2.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    //Write the headers and style them
                    wsA2.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA2.Cells["A2"].Style.Font.Size = 18;
                    wsA2.Cells["A2"].Style.Font.Bold = true;
                    wsA2.Cells["A2:M2"].Merge = true;
                    wsA2.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA2.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA2.Cells["A3"].Style.Font.Size = 14;
                    wsA2.Cells["A3"].Style.Font.Bold = true;
                    wsA2.Cells["A3:M3"].Merge = true;
                    wsA2.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA2.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA2.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA2.Cells["A5"].Style.Font.Size = 10;
                    wsA2.Cells["A5:F5"].Merge = true;
                    wsA2.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA2.Cells["A6"].Style.Font.Size = 10;
                    wsA2.Cells["A6:F6"].Merge = true;

                    wsA2.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA2.Cells["A7"].Style.Font.Size = 10;
                    wsA2.Cells["A7"].Style.Font.Bold = true;

                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA2.Dimension.End.Row + 1;

                        wsA2.Cells[row + 1, 1].Value = "TOTAL";
                        wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                        wsA2.Cells[row + 1, 2, row + 1, 4 - 1].Style.Numberformat.Format = "#,##0.00";

                    }
                    wsA2.View.FreezePanes(11, 1);
                    // var tbl2 = wsA2.Tables[0];
                    if (range2 != null)
                    {
                        range2.AutoFitColumns();
                    }
                   // tbl2.ShowFilter = false;
                    //wsA2.Cells[wsA2.Dimension.Address].AutoFilter = true;

                }
                catch(Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH_EXCEPTION", MERCHANTID, MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);

                }
                dtMain.Dispose();
                try
                {

                 
                    dtMain = generateDS(MERCHANTNAME, MERCHANTID, null, "MERCH_SUMM", reportFolderType, "M", sett, reportISSUERPath, logopath, 1);
                    var wsA3 = package.Workbook.Worksheets.Add("SUMMARY");

                    //ExcelRange cols = wsA3.Cells["A:XFD"];
                    //wsA3.Cells["A:XFD"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsA3.Cells["A:XFD"].Style.Fill.BackgroundColor.SetColor(Color.White);
                    var range3 = wsA3.Cells["A11"].LoadFromDataTable(dtMain, true );
                    //wsA3.Tables.Add(range3, "data3");
                    wsA3.Row(11).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsA3.Row(11).Style.Fill.BackgroundColor.SetColor(Color.Orange);


                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                    var pic = wsA3.Drawings.AddPicture("LOGO", myImage);
                    pic.SetPosition(0, 0, 0, 0);

                    //Write the headers and style them
                    wsA3.Cells["A2"].Value = "Unified Payment Services Ltd";
                    wsA3.Cells["A2"].Style.Font.Size = 18;
                    wsA3.Cells["A2"].Style.Font.Bold = true;
                    wsA3.Cells["A2:M2"].Merge = true;
                    wsA3.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    wsA3.Cells["A3"].Value = "MERCHANT TRANSACTION ";
                    wsA3.Cells["A3"].Style.Font.Size = 14;
                    wsA3.Cells["A3"].Style.Font.Bold = true;
                    wsA3.Cells["A3:M3"].Merge = true;
                    wsA3.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    wsA3.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                    wsA3.Cells["A5"].Value = "MERCHANT NAME: " + MERCHANTNAME;
                    wsA3.Cells["A5"].Style.Font.Size = 10;
                    wsA3.Cells["A5:F5"].Merge = true;
                    wsA3.Cells["A6"].Value = "ADDRESS: " + ADDRESS;
                    wsA3.Cells["A6"].Style.Font.Size = 10;
                    wsA3.Cells["A6:F6"].Merge = true;

                    wsA3.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                    wsA3.Cells["A7"].Style.Font.Size = 10;
                    wsA3.Cells["A7"].Style.Font.Bold = true;
                    if (dtMain != null && dtMain.Rows.Count > 1)
                    {
                        int row = wsA3.Dimension.End.Row + 1;
                        wsA3.Cells[row + 1, 1].Value = "TOTAL";
                        wsA3.Cells[row + 1, 2, row + 1, 6 - 1].Formula = string.Format("=SUM(B{0}:B{1})", 12, row - 1);

                        wsA3.Cells[row + 1, 2, row + 1, 6 - 1].Style.Numberformat.Format = "#,##0.00";

                    }
                    wsA3.View.FreezePanes(11, 1);
                    // var tbl3 = wsA3.Tables[0];
                    if (range3 != null)
                    {
                        range3.AutoFitColumns();
                    }
                    //tbl3.ShowFilter = false;
                    //wsA3.Cells[wsA3.Dimension.Address].AutoFilter = true;

                }
                catch (Exception ex)
                {
                    LogFunction2.WriteMaintenaceLogToFile(string.Format("Maintenance Service Error on: {0}-{1}-{2} --", "MERCH-SUMMARY",MERCHANTID,MERCHANTNAME) + "{0}" + ex.Message + ex.StackTrace);
                }
                dtMain.Dispose();

                /*    
                try
                {
                    var wsA = package.Workbook.Worksheets.Add("DETAILS");
                    wsA = genMerchant2(wsA, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SettlementCur, SETTLEMENTACCOUNT, BANKNAME, null, "DETAIL", "M", sett, 11, reportFolderType);


                }
                catch (Exception ex)
                {

                }
                //loop on cardscheme
                //////try
                //////{
                //////    var ws = package.Workbook.Worksheets.Add("VISA DR");
                //////    ////ws = genMerchant(ws, logopath, MERCHANTNAME, MERCHANTID, "VISA", "MERCH_DR", "M", sett, 8);
                //////    ws = genMerchant2(ws, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_DR", "M", sett, 11, reportFolderType);


                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{

                //////    var ws2 = package.Workbook.Worksheets.Add("VISA CR");
                //////    //ws2 = genMerchant(ws2, logopath, MERCHANTNAME, MERCHANTID, "VISA", "MERCH_CR", "M", sett, 8);
                //////    ws2 = genMerchant2(ws2, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VISA", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws3 = package.Workbook.Worksheets.Add("MASTERCARD DR");
                //////    //ws3 = genMerchant(ws3, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", "M", sett, 8);

                //////    ws3 = genMerchant2(ws3, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "MAST", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws4 = package.Workbook.Worksheets.Add("MASTERCARD CR");
                //////    //ws4 = genMerchant(ws4, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", "M", sett, 8);
                //////    ws4 = genMerchant2(ws4, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "MAST", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}


                //////try
                //////{
                //////    var ws3 = package.Workbook.Worksheets.Add("VERVE DR");
                //////    //ws3 = genMerchant(ws3, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_DR", "M", sett, 8);

                //////    ws3 = genMerchant2(ws3, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VERV", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws4 = package.Workbook.Worksheets.Add("VERVE CR");
                //////    //ws4 = genMerchant(ws4, logopath, MERCHANTNAME, MERCHANTID, "MAST", "MERCH_CR", "M", sett, 8);
                //////    ws4 = genMerchant2(ws4, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "VERV", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}
                //////try
                //////{
                //////    var ws5 = package.Workbook.Worksheets.Add("PAYATITUDE DR");
                //////    // ws5 = genMerchant(ws5, logopath, MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_DR", "M", sett, 8);
                //////    ws5 = genMerchant2(ws5, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "PAYA", "MERCH_DR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}

                //////try
                //////{
                //////    var ws6 = package.Workbook.Worksheets.Add("PAYATITUDE CR");
                //////    // ws6 = genMerchant(ws6, logopath, MERCHANTNAME, MERCHANTID, "PAYA", "MERCH_CR", "M", sett, 8);
                //////    ws6 = genMerchant2(ws6, logopath, MERCHANTID, MERCHANTNAME, ADDRESS, SETTLEMENTACCOUNT, BANKNAME, "PAYA", "MERCH_CR", "M", sett, 11, reportFolderType);

                //////}
                //////catch (Exception ex)
                //////{

                //////}





                try
                {
                    var ws8 = package.Workbook.Worksheets.Add("EXCEPTION");
                    ws8 = genMerchant(ws8, logopath, MERCHANTNAME, MERCHANTID, SettlementCur,null, "MERCH_EXCEP", "M", sett, 11, reportFolderType);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    var ws9 = package.Workbook.Worksheets.Add("SUMMARY");
                    ws9 = genMerchant3(ws9, logopath, MERCHANTNAME, MERCHANTID, SettlementCur,null, "MERCH_SUMM", "M", sett, 11, reportFolderType);
                }
                catch (Exception ex)
                {

                }


                package.Compression = CompressionLevel.BestSpeed;
                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
                */

                package.SaveAs(newFile);
                stream.Dispose();
                package.Dispose();
            }

            return newFile.FullName;

    

        }
         public static  DataTable generateDS(string reportFor, string ID, string cardScheme, string reportType, string DOMINT, string reportClass, string sett, string ReportPath, string logopath, int cnt)
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

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
                
                try
                {
                   
                     var dr = cmd.ExecuteReader();
                    dsExcel drReport = new dsExcel();
                    dt.Load(dr);
                    //ds.Tables.Add(dt);
                    cmd.Dispose();
                    
                }
                catch
                {


                }

                cmd.Dispose();


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            return dt;


        }

        public static string generateDSReport(string reportFor, string ID, string cardScheme, string reportType, string DOMINT, string reportClass, string sett, string ReportPath, string logopath, int cnt)
        {



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

                try
                {
                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    var dr = cmd.ExecuteReader();
                    dsExcel drReport = new dsExcel();
                    dt.Load(dr);
                    ds.Tables.Add(dt);
                    cmd.Dispose();
                    drReport.ExportDataTOExcelMoreSheet(ds, ReportPath, reportFor, null, 0,cnt);
                }
                catch
                {


                }

                cmd.Dispose();


            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message );

            }


            return null;


        }



    }


}
 