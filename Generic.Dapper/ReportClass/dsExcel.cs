using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPPosMaster.Dapper.Repository;
using Dapper;
//using TechPayApp.Dapper.Model;
using System.Data.SqlClient;

using UPPosMaster.Dapper.Model;
using UPPosMaster.Data;

using System.Dynamic;

using DocumentFormat.OpenXml.Packaging;

using System.IO;

using DocumentFormat.OpenXml;

using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.Text.RegularExpressions;
using System.Reflection;


namespace UPPosMaster.Dapper.Data
{
    public class dsExcel
    {
        private static string oradb = System.Configuration.ConfigurationManager.AppSettings["TestDB"].ToString();


        public DataSet COLLECTIONSETT(string searchID, string customerID, DateTime dt, string conString = null)
        {
            // IEnumerable<Get_POSCARDSCHEME> results = null;

            string cppdate = dt.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_SETTLEMENTDETAIL", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                OracleParameter inval3 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = searchID;
                cmd.Parameters.Add(inval3);

                OracleParameter inval2 = new OracleParameter("P_CustomerID", OracleDbType.Varchar2);
                inval2.Direction = ParameterDirection.Input;
                inval2.Value = customerID;
                cmd.Parameters.Add(inval2);


                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "SETTLEMENTDETAIL");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }



        }

        public DataSet COLLECTIONNIBSS(string searchID, DateTime dt, string conString = null)
        {
            // IEnumerable<Get_POSCARDSCHEME> results = null;

            string cppdate = dt.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_NIBSS", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                OracleParameter inval3 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = searchID;
                cmd.Parameters.Add(inval3);



                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "COLLECTION_NIBSS");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }



        }


        public DataSet COLLECTIONNIBSS2(string searchID, DateTime dt, string conString = null)
        {
            // IEnumerable<Get_POSCARDSCHEME> results = null;

            string cppdate = dt.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_NIBSS2", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                OracleParameter inval3 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = searchID;
                cmd.Parameters.Add(inval3);



                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "COLLECTION_NIBSS");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }



        }

        public DataSet COLLECTIONNIBSSSUMM(string searchID, DateTime dt, string conString = null)
        {
            // IEnumerable<Get_POSCARDSCHEME> results = null;

            string cppdate = dt.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_NIBSS_SUMMARY", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                OracleParameter inval3 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = searchID;
                cmd.Parameters.Add(inval3);


                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "COLLECTION_NIBSS_SUMM");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }



        }

        public DataSet GET_NIBSS(int reporttype, DateTime dt, string conString = null)
        {
            // IEnumerable<Get_POSCARDSCHEME> results = null;

            string cppdate = dt.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("GET_NIBSS", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                OracleParameter inval3 = new OracleParameter("P_reporttype", OracleDbType.Int16);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = reporttype;
                cmd.Parameters.Add(inval3);


                OracleParameter inval5 = new OracleParameter("P_DATE", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("CURSOR1", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "NIBSS");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }



        }

        public DataSet GETNONTRANReport(DateTime cpd, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("NONTRANSACTION_MEB", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval5 = new OracleParameter("P_CPD", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);


                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "NON_FINANCIAL_TRANSACTION");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }




        }

        public DataSet GETTEXTMESSReport(DateTime cpd, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("TEXTMESS_ERROR", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval5 = new OracleParameter("P_CPD", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);


                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "TEXTMESS_ERROR");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }




        }

        public DataSet GETErorrReport(DateTime cpd, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("ERROR_MEB", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval5 = new OracleParameter("P_CPD", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);


                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "MEB_ERROR");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }



        }

        public DataSet GETUnsettledReport(string conString = null)
        {
            // IEnumerable<Get_POSCARDSCHEME> results = null;


            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("UNSETTLED_MEB", cn);
                cmd.CommandType = CommandType.StoredProcedure;


                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);


                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "UNSETTLED_MEB");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }





        }


        public DataSet GETCollectionNIBBSDetail(string searchID, string searchOption, string reporttype, DateTime cpd, string classid, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_NIBSS", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval1 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval1.Direction = ParameterDirection.Input;
                inval1.Value = searchID;
                cmd.Parameters.Add(inval1);


                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);



                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "NIBSS_DETAIL");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }






        }


        public DataSet GETCollectionNIBBSUMMARY(string searchID, string searchOption, string reporttype, DateTime cpd, string classid, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_NIBSS_SUMMARY", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval1 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval1.Direction = ParameterDirection.Input;
                inval1.Value = searchID;
                cmd.Parameters.Add(inval1);


                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);



                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "NIBSS_SUMMARY");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }






        }

        public DataSet GETCollectionSettlementDetail(string searchID, string searchOption, string reporttype, DateTime cpd, string classid, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("COLLECTION_SETTLEMENTDETAIL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval1 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval1.Direction = ParameterDirection.Input;
                inval1.Value = searchID;
                cmd.Parameters.Add(inval1);


                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("p_cursor", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);




                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "COLLECTION");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }






        }

        public DataSet GETSettlementDetail(string searchID, string searchOption, string reporttype, DateTime cpd, string classid, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("GET_SETTLEMETDETAIL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval1 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval1.Direction = ParameterDirection.Input;
                inval1.Value = searchID;
                cmd.Parameters.Add(inval1);

                OracleParameter inval2 = new OracleParameter("P_searchOption", OracleDbType.Varchar2);
                inval2.Direction = ParameterDirection.Input;
                inval2.Value = searchOption;
                cmd.Parameters.Add(inval2);

                OracleParameter inval3 = new OracleParameter("P_reporttype", OracleDbType.Varchar2);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = reporttype;
                cmd.Parameters.Add(inval3);


                OracleParameter inval4 = new OracleParameter("P_reportClass", OracleDbType.Varchar2);
                inval4.Direction = ParameterDirection.Input;
                inval4.Value = classid;
                cmd.Parameters.Add(inval4);

                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("CURSOR_", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);




                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "SETTLEMENT");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }




        }

        public DataSet rptSettlementDetail(string searchID, string searchOption, string reporttype, DateTime cpd, string classid, string conString = null)
        {
            string cppdate = cpd.ToString("dd-MMM-yy");
            cppdate = cppdate.ToUpper();
            using (OracleConnection cn = new OracleConnection(oradb))
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand("RPT_SETTLEMETDETAIL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter inval1 = new OracleParameter("P_searchID", OracleDbType.Varchar2);
                inval1.Direction = ParameterDirection.Input;
                inval1.Value = searchID;
                cmd.Parameters.Add(inval1);

                OracleParameter inval2 = new OracleParameter("P_CardScheme", OracleDbType.Varchar2);
                inval2.Direction = ParameterDirection.Input;
                inval2.Value = searchOption;
                cmd.Parameters.Add(inval2);

                OracleParameter inval3 = new OracleParameter("P_reporttype", OracleDbType.Varchar2);
                inval3.Direction = ParameterDirection.Input;
                inval3.Value = reporttype;
                cmd.Parameters.Add(inval3);


                OracleParameter inval4 = new OracleParameter("P_reportClass", OracleDbType.Varchar2);
                inval4.Direction = ParameterDirection.Input;
                inval4.Value = classid;
                cmd.Parameters.Add(inval4);

                OracleParameter inval5 = new OracleParameter("P_SETT", OracleDbType.Varchar2);
                inval5.Direction = ParameterDirection.Input;
                inval5.Value = cppdate;
                cmd.Parameters.Add(inval5);

                OracleParameter outval = new OracleParameter("CURSOR_", OracleDbType.RefCursor);
                outval.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outval);




                OracleDataAdapter da = new OracleDataAdapter(cmd);
                System.Data.DataSet ds = new System.Data.DataSet();
                da.Fill(ds, "SETTLEMENT");
                try
                {

                    return ds;

                }

                catch (Exception ex)

                {

                    return null;

                }

                finally

                {

                    cmd.Dispose();

                    cn.Close();
                    OracleConnection.ClearPool(cn);
                    cn.Dispose();
                }

            }




        }

        private static string GetFormatedValue(Cell cell, CellFormat cellformat)
        {
            string value;
            if (cellformat.NumberFormatId != 0)
            {
                string format = GetWorkbookPartFromCell(cell).WorkbookStylesPart.Stylesheet.NumberingFormats.Elements<NumberingFormat>()
                    .Where(i => i.NumberFormatId.Value == cellformat.NumberFormatId.Value)
                    .First().FormatCode;
                double number = double.Parse(cell.InnerText);
                value = number.ToString(format);
            }
            else
            {
                value = cell.InnerText;
            }
            return value;
        }

        private Stylesheet GenerateStyleSheet()
        {
            // Index 0 - The default font.
            // Index 1 - The bold font.
            // Index 2 - The Italic font.
            // Index 2 - The Times Roman font. with 16 size
            // Index 0 - The default fill.
            // Index 1 - The default fill of gray 125 (required)
            // Index 2 - The yellow fill.
            // Index 0 - The default border.
            // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
            // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
            // Index 1 - Bold 
            // Index 2 - Italic
            // Index 3 - Times Roman
            // Index 4 - Yellow Fill
            // Index 5 - Alignment
            // Index 6 - Border
            // Index 7 - Numberic #,###.## - EXCEL default style 4
            return new Stylesheet(new Fonts(new DocumentFormat.OpenXml.Spreadsheet.Font(new FontSize { Val = 11 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = new HexBinaryValue { Value = "000000" } }, new FontName { Val = "Calibri" }), new DocumentFormat.OpenXml.Spreadsheet.Font(new Bold(), new FontSize { Val = 11 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = new HexBinaryValue { Value = "000000" } }, new FontName { Val = "Calibri" }), new DocumentFormat.OpenXml.Spreadsheet.Font(new Italic(), new FontSize { Val = 11 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = new HexBinaryValue { Value = "000000" } }, new FontName { Val = "Calibri" }), new DocumentFormat.OpenXml.Spreadsheet.Font(new FontSize { Val = 16 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = new HexBinaryValue { Value = "000000" } }, new FontName { Val = "Times New Roman" })), new Fills(new Fill(new PatternFill { PatternType = PatternValues.None }), new Fill(new PatternFill { PatternType = PatternValues.Gray125 }), new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue { Value = "FFFFFF00" } }) { PatternType = PatternValues.Solid })), new Borders(new Border(new LeftBorder(), new RightBorder(), new TopBorder(), new BottomBorder(), new DiagonalBorder()), new Border(new LeftBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin }, new RightBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin }, new TopBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin }, new BottomBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin }, new DiagonalBorder())), new CellFormats(new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0
            }, new CellFormat
            {
                FontId = 1,
                FillId = 0,
                BorderId = 0,
                ApplyFont = true
            }, new CellFormat
            {
                FontId = 2,
                FillId = 0,
                BorderId = 0,
                ApplyFont = true
            }, new CellFormat
            {
                FontId = 3,
                FillId = 0,
                BorderId = 0,
                ApplyFont = true
            }, new CellFormat
            {
                FontId = 0,
                FillId = 2,
                BorderId = 0,
                ApplyFill = true
            }, new CellFormat(new Alignment
            {
                Horizontal = HorizontalAlignmentValues.Center,
                Vertical = VerticalAlignmentValues.Center
            })
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                ApplyAlignment = true
            }, new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                ApplyBorder = true
            }, new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                NumberFormatId = 4,
                ApplyNumberFormat = true
            }));
        }

        private static WorkbookPart GetWorkbookPartFromCell(Cell cell)
        {
            Worksheet workSheet = cell.Ancestors<Worksheet>().FirstOrDefault();
            SpreadsheetDocument doc = workSheet.WorksheetPart.OpenXmlPackage as SpreadsheetDocument;
            return doc.WorkbookPart;
        }
        ////public static WorkbookPart ExportExcelSAX(WorksheetPart worksheetPart,string bankname,string bankcode,string SETT,string Cardscheme,)
        ////{
        ////    using (var writer = OpenXmlWriter.Create(worksheetPart))
        ////    {

        ////        writer.WriteStartElement(new Worksheet());
        ////        writer.WriteStartElement(new SheetData());
        ////        DataSet ds = dsExcel.GenReportDS(null, null, SETT, null, "SETT", "U", "DOM");



        ////        ////writer.WriteStartElement(new Row());
        ////        List<String> columns = new List<string>();
        ////        foreach (DataTable table in ds.Tables)
        ////        {
        ////            //HEADER
        ////            writer.WriteStartElement(new Row());
        ////            foreach (DataColumn column in table.Columns)
        ////            {
        ////                columns.Add(column.ColumnName);

        ////                openXmlExportHelper.WriteCellValueSax(writer, column.ColumnName, CellValues.InlineString);

        ////            }

        ////            writer.WriteEndElement();


        ////            foreach (DataRow dsrow in table.Rows)
        ////            {

        ////                writer.WriteStartElement(new Row());
        ////                foreach (String col in columns)
        ////                {
        ////                    ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        ////                    ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        ////                    ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
        ////                    ////newRow.AppendChild(cell);

        ////                    object dataValue = CleanInvalidXmlChars(dsrow[col].ToString());



        ////                    decimal n;
        ////                    DateTime dt;
        ////                    bool isNumeric = decimal.TryParse(dataValue.ToString(), out n);




        ////                    try
        ////                    {
        ////                        if (isNumeric == true)
        ////                        {

        ////                            openXmlExportHelper.WriteCellValueSax(writer, dataValue.ToString(), CellValues.Number);


        ////                        }
        ////                        else if (DateTime.TryParse(dataValue.ToString(), out dt))
        ////                        {
        ////                            DateTime dtValue;
        ////                            string strValue = "";
        ////                            if (DateTime.TryParse(dataValue.ToString(), out dtValue))
        ////                                strValue = dtValue.ToString("dd-mm-yyyy hh:mm:ss AM/PM");
        ////                            openXmlExportHelper.WriteCellValueSax(writer, strValue, CellValues.Date);


        ////                        }
        ////                        else
        ////                        {

        ////                            openXmlExportHelper.WriteCellValueSax(writer, dataValue.ToString(), CellValues.InlineString);
        ////                        }
        ////                    }
        ////                    catch (Exception ex)
        ////                    {

        ////                    }
        ////                }

        ////                writer.WriteEndElement();
        ////            }


        ////            writer.WriteEndElement(); //end of SheetData
        ////            writer.WriteEndElement(); //end of worksheet
        ////            writer.Close();

        ////        }



        ////    }

        ////}
        public void ExportDataTOExcel(DataSet ds, string destination, string sheetNameDesr, string logopath, int allowlogo)
        {


            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = 1;

                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    //var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //sheetStyle.Stylesheet = CreateStylesheet4();
                    //sheetStyle.Stylesheet.Save();

                    //////var stylesPart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();

                    //////stylesPart.Stylesheet = new Stylesheet();

                    //////// blank font list
                    //////stylesPart.Stylesheet.Fonts = new Fonts();
                    //////stylesPart.Stylesheet.Fonts.Count = 1;
                    //////stylesPart.Stylesheet.Fonts.AppendChild(new Font());

                    //////// create fills
                    //////stylesPart.Stylesheet.Fills = new Fills();

                    //////// create a solid red fill
                    //////var solidRed = new PatternFill() { PatternType = PatternValues.Solid };
                    //////solidRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FFFF0000") }; // red fill
                    //////solidRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

                    //////stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
                    //////stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
                    //////stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = solidRed });
                    //////stylesPart.Stylesheet.Fills.Count = 3;

                    //////// blank border list
                    //////stylesPart.Stylesheet.Borders = new Borders();
                    //////stylesPart.Stylesheet.Borders.Count = 1;
                    //////stylesPart.Stylesheet.Borders.AppendChild(new Border());

                    //////// blank cell format list
                    //////stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
                    //////stylesPart.Stylesheet.CellStyleFormats.Count = 1;
                    //////stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

                    //////// cell format list
                    //////stylesPart.Stylesheet.CellFormats = new CellFormats();
                    //////// empty one for index 0, seems to be required
                    //////stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());
                    //////// cell format references style format 0, font 0, border 0, fill 2 and applies the fill
                    //////stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });
                    //////stylesPart.Stylesheet.CellFormats.Count = 2;

                    //////stylesPart.Stylesheet.Save();


                    ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
                    sheets.Append(sheet);

                    if (allowlogo == 1)
                    {
                        InsertImage(sheetPart.Worksheet, 0, 0, logopath);
                        Row row;
                        row = new Row() { RowIndex = 5 };
                        sheetData.Append(row);

                    }


                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    int index = 1;
                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();

                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    //Nullable<uint> styleIndex = null;
                    //double doubleValue;
                    //int intValue;
                    //DateTime dateValue;

                    foreach (DataRow dsrow in table.Rows)
                    {
                        index++;
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                            ////newRow.AppendChild(cell);

                            object dataValue = dsrow[col].ToString();


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
                                    strValue = dtValue.ToString("dd-mm-yyyy hh:mm:ss AM/PM");
                                newRow.AppendChild(new Cell() { CellValue = new CellValue(strValue), DataType = CellValues.Date });
                            }
                            else
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                                string textvalin = dsrow[col].ToString();
                                string textvalout = Regex.Replace(textvalin, re, "");

                                ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                                //////newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.String });
                                ////newRow.AppendChild(cell);
                                cell.DataType = CellValues.InlineString;
                                cell.InlineString = new InlineString() { Text = new Text(textvalout) };
                                newRow.AppendChild(cell);
                            }



                        }

                        sheetData.AppendChild(newRow);
                    }
                }
            }
        }



        ////public void ExportDataTOExcel(DataSet ds, string destination, string sheetNameDesr, string logopath, int allowlogo)
        ////{


        ////    using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
        ////    {
        ////        var workbookPart = workbook.AddWorkbookPart();
        ////        workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
        ////        workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

        ////        uint sheetId = 1;

        ////        foreach (DataTable table in ds.Tables)
        ////        {
        ////            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        ////            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        ////            //var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        ////            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);


        ////            ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

        ////            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        ////            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        ////            if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
        ////            {
        ////                sheetId =
        ////                    sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        ////            }

        ////            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
        ////            sheets.Append(sheet);

        ////            if (allowlogo == 1)
        ////            {
        ////                InsertImage(sheetPart.Worksheet, 0, 0, logopath);
        ////                Row row;
        ////                row = new Row() { RowIndex = 5 };
        ////                sheetData.Append(row);

        ////            }


        ////            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        ////            int index = 1;
        ////            List<String> columns = new List<string>();
        ////            foreach (DataColumn column in table.Columns)
        ////            {
        ////                columns.Add(column.ColumnName);

        ////                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();

        ////                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        ////                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
        ////                headerRow.AppendChild(cell);
        ////            }

        ////            sheetData.AppendChild(headerRow);

        ////            //Nullable<uint> styleIndex = null;
        ////            //double doubleValue;
        ////            //int intValue;
        ////            //DateTime dateValue;
        ////            double cellNumericValue = 0;
        ////            string cellValueRecord = "";
        ////            foreach (DataRow dsrow in table.Rows)
        ////            {
        ////                index++;
        ////                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        ////                foreach (String col in columns)
        ////                {
        ////                    ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        ////                    ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        ////                    ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
        ////                    ////newRow.AppendChild(cell);

        ////                    cellValueRecord = dsrow[col].ToString();
        ////                    string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        ////                    string textvalin = cellValueRecord;
        ////                    cellValueRecord = Regex.Replace(textvalin, re, "");


        ////                    decimal n;
        ////                    DateTime dt;
        ////                    bool isNumeric = decimal.TryParse(cellValueRecord, out n);

        ////                    // Create cell with data
        ////                    if (isNumeric == true)
        ////                    {
        ////                        //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
        ////                        //  If this numeric value is NULL, then don't write anything to the Excel file.
        ////                        cellNumericValue = 0;
        ////                        if (double.TryParse(cellValueRecord, out cellNumericValue))
        ////                        {
        ////                            cellValueRecord = cellNumericValue.ToString();
        ////                            newRow.AppendChild(new Cell() { CellValue = new CellValue(cellValueRecord.ToString()), DataType = CellValues.Number });

        ////                        }
        ////                    }
        ////                    else if (DateTime.TryParse(cellValueRecord, out dt))
        ////                    {
        ////                        DateTime dtValue;
        ////                        string strValue = "";
        ////                        if (DateTime.TryParse(cellValueRecord, out dtValue))
        ////                            strValue = dtValue.ToString("dd/MM/yyyy HH:mm:ss");
        ////                        newRow.AppendChild(new Cell() { CellValue = new CellValue(strValue), DataType = CellValues.Date });

        ////                    }
        ////                    else
        ////                    {
        ////                        newRow.AppendChild(new Cell() { CellValue = new CellValue(cellValueRecord.ToString()), DataType = CellValues.String });

        ////                    }

        ////                    //////if (isNumeric == true)
        ////                    //////{

        ////                    //////    newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Number });


        ////                    //////}
        ////                    //////else if (DateTime.TryParse(dataValue.ToString(), out dt))
        ////                    //////{
        ////                    //////    newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Date });


        ////                    //////}
        ////                    //////else
        ////                    //////{
        ////                    //////    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        ////                    //////    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        ////                    //////    string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        ////                    //////    string textvalin = dsrow[col].ToString();
        ////                    //////    string textvalout = Regex.Replace(textvalin, re, "");

        ////                    //////    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
        ////                    //////    //newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.String });
        ////                    //////    newRow.AppendChild(cell);
        ////                    //////}



        ////                }

        ////                sheetData.AppendChild(newRow);
        ////            }
        ////        }
        ////    }
        ////}


        //public void ExportDataTOExcel(DataSet ds, string destination, string sheetNameDesr, string logopath, int allowlogo)
        //{

        //    using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
        //    {
        //        var workbookPart = workbook.AddWorkbookPart();
        //        workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
        //        workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

        //        uint sheetId = 1;

        //        foreach (DataTable table in ds.Tables)
        //        {
        //            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        //            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        //            //var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        //            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

        //            //sheetStyle.Stylesheet = CreateStylesheet4();
        //            //sheetStyle.Stylesheet.Save();

        //            var stylesPart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        //            stylesPart.Stylesheet = new Stylesheet();


        //            // blank font list
        //            stylesPart.Stylesheet.Fonts = new Fonts();
        //            stylesPart.Stylesheet.Fonts.Count = 1;
        //            stylesPart.Stylesheet.Fonts.AppendChild(new Font());

        //            // create fills
        //            stylesPart.Stylesheet.Fills = new Fills();

        //            // create a solid red fill
        //            var solidRed = new PatternFill() { PatternType = PatternValues.Solid };
        //            solidRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FFFF0000") }; // red fill
        //            solidRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

        //            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
        //            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
        //            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = solidRed });
        //            stylesPart.Stylesheet.Fills.Count = 3;

        //            // blank border list
        //            stylesPart.Stylesheet.Borders = new Borders();
        //            stylesPart.Stylesheet.Borders.Count = 1;
        //            stylesPart.Stylesheet.Borders.AppendChild(new Border());

        //            // blank cell format list
        //            stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
        //            stylesPart.Stylesheet.CellStyleFormats.Count = 1;
        //            stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

        //            // cell format list
        //            stylesPart.Stylesheet.CellFormats = new CellFormats();
        //            // empty one for index 0, seems to be required
        //            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());
        //            // cell format references style format 0, font 0, border 0, fill 2 and applies the fill
        //            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = HorizontalAlignmentValues.Center });
        //            stylesPart.Stylesheet.CellFormats.Count = 2;

        //            stylesPart.Stylesheet.Save();


        //            ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

        //            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        //            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        //            if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
        //            {
        //                sheetId =
        //                    sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        //            }

        //            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
        //            sheets.Append(sheet);

        //            if (allowlogo == 1)
        //            {
        //                InsertImage(sheetPart.Worksheet, 0, 0, logopath);
        //                Row row;
        //                row = new Row() { RowIndex = 5 };
        //                sheetData.Append(row);

        //            }

        //            WriteDataTableToExcelWorksheet(table, sheetPart);

        //           // openXmlExportHelper.CreateShareStringPart(sheetPart);

        //            //////DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //            //////int index = 1;
        //            //////List<String> columns = new List<string>();
        //            //////foreach (DataColumn column in table.Columns)
        //            //////{
        //            //////    columns.Add(column.ColumnName);

        //            //////    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();

        //            //////    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            //////    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
        //            //////    headerRow.AppendChild(cell);
        //            //////}

        //            //////sheetData.AppendChild(headerRow);

        //            ////////Nullable<uint> styleIndex = null;
        //            ////////double doubleValue;
        //            ////////int intValue;
        //            ////////DateTime dateValue;

        //            //////foreach (DataRow dsrow in table.Rows)
        //            //////{
        //            //////    index++;
        //            //////    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //            //////    foreach (String col in columns)
        //            //////    {
        //            //////        ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            //////        ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            //////        ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
        //            //////        ////newRow.AppendChild(cell);

        //            //////        object dataValue = dsrow[col].ToString();



        //            //////        decimal n;
        //            //////        DateTime dt;
        //            //////        bool isNumeric = decimal.TryParse(dataValue.ToString(), out n);


        //            //////        ////if (dataValue != null)
        //            //////        ////{
        //            //////        ////    cell = CreateCellFromTemplate(workbook, colAlpha, rowNumber, cell, dataValue);
        //            //////        ////}

        //            //////        ////newRow.AppendChild(cell);
        //            //////        ////rowNumber++;

        //            //////        if (isNumeric == true)
        //            //////        {

        //            //////            // newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Number });

        //            //////            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            //////            cell.DataType = CellValues.Number;
        //            //////            cell.InlineString = new InlineString() { Text = new Text(n.ToString()) };
        //            //////            newRow.AppendChild(cell);

        //            //////        }
        //            //////        else if (DateTime.TryParse(dataValue.ToString(), out dt))
        //            //////        {
        //            //////            // newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Date });

        //            //////            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            //////            cell.DataType = CellValues.Date;
        //            //////            cell.InlineString = new InlineString() { Text = new Text(dt.ToString()) };
        //            //////            newRow.AppendChild(cell);

        //            //////        }
        //            //////        else
        //            //////        {
        //            //////            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            //////            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            //////            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        //            //////            string textvalin = dsrow[col].ToString();
        //            //////            string textvalout = Regex.Replace(textvalin, re, "");

        //            //////            ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
        //            //////            //////newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.String });
        //            //////            ////newRow.AppendChild(cell);
        //            //////            cell.DataType = CellValues.InlineString;
        //            //////            cell.InlineString = new InlineString() { Text = new Text(textvalout) };
        //            //////            newRow.AppendChild(cell);
        //            //////        }



        //            //////    }

        //            //sheetData.AppendChild(newRow);

        //            //}
        //        }
        //    }
        //}



        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }


        public void ExportDataTOExcelMoreSheet(DataSet ds, string destination, string sheetNameDesr, string logopath, int allowlogo, int sheetval)
        {
            FileInfo newFile = new FileInfo(destination);

            MemoryStream stream = new MemoryStream();
                       
            using (SpreadsheetDocument workbook = SpreadsheetDocument.Open(stream, true))
            {


                //////var workbookPart = workbook.AddWorkbookPart();
                //////workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                //////workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = Convert.ToUInt32(sheetval);

                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    //sheetStyle.Stylesheet = CreateStylesheet4();

                    ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);


                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
                    sheets.Append(sheet);

                    if (allowlogo == 1)
                    {
                        InsertImage(sheetPart.Worksheet, 0, 0, logopath);
                        Row row;
                        row = new Row() { RowIndex = 5 };
                        sheetData.Append(row);

                    }

                    int index = 1;

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        index++;
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                            ////newRow.AppendChild(cell);

                            object dataValue = dsrow[col].ToString();



                            decimal n;
                            DateTime dt;
                            bool isNumeric = decimal.TryParse(dataValue.ToString(), out n);


                            ////if (dataValue != null)
                            ////{
                            ////    cell = CreateCellFromTemplate(workbook, colAlpha, rowNumber, cell, dataValue);
                            ////}

                            ////newRow.AppendChild(cell);
                            ////rowNumber++;

                            if (isNumeric == true)
                            {

                                newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Number });


                            }
                            else if (DateTime.TryParse(dataValue.ToString(), out dt))
                            {
                                DateTime dtValue;
                                string strValue = "";
                                if (DateTime.TryParse(dataValue.ToString(), out dtValue))
                                    strValue = dtValue.ToString("dd-mm-yyyy hh:mm:ss AM/PM");
                                newRow.AppendChild(new Cell() { CellValue = new CellValue(strValue), DataType = CellValues.Date });

                            }
                            else
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                                string textvalin = dsrow[col].ToString();
                                string textvalout = Regex.Replace(textvalin, re, "");

                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                                //newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.String });
                                newRow.AppendChild(cell);

                            }



                        }

                        sheetData.AppendChild(newRow);
                    }
                }
            }
        }


        public static DataSet GenReportDS(string reportFor, string ID, DateTime reportDate,  string cardScheme, string reportType, string reportClass,string DOMINT)
        {
            string sett = reportDate.ToString("dd-MMM-yyyy");
            string reportfile = string.Empty;

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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

                var dr = cmd.ExecuteReader();
                dsExcel drReport = new dsExcel();
                dt.Load(dr);
                ds.Tables.Add(dt);
                cmd.Dispose();
                // drReport.ExportDataTOExcelMoreSheet(ds, ReportPath, reportFor, null, 0, cnt);



            }
            catch (Exception ex)
            {

            }

            return ds;

        }

        public void ExportDataTOExcelMoreSheetSAX(DataSet ds, string destination, string sheetNameDesr, string logopath, int allowlogo, int sheetval,Sheets sheets,WorkbookPart workbookPart)
        {
            using (var spreadSheet = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook))
            {
                var openXmlExportHelper = new OpenXmlWriterHelper();
                uint sheetId = Convert.ToUInt32(sheetval);

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = sheetNameDesr };
                sheets.Append(sheet);
                 

                //////var workbookPart = workbook.AddWorkbookPart();
                //////workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                //////workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                foreach (DataTable table in ds.Tables)
                {


                    using (var writer = OpenXmlWriter.Create(worksheetPart))
                    {
                        writer.WriteStartElement(new Worksheet());
                        writer.WriteStartElement(new SheetData());
                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        writer.WriteStartElement(new Row());
                        List<String> columns = new List<string>();
                        foreach (DataColumn column in table.Columns)
                        {
                            columns.Add(column.ColumnName);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                            headerRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(headerRow);

                        foreach (DataRow dsrow in table.Rows)
                        {
                           
                            writer.WriteStartElement(new Row());
                            foreach (String col in columns)
                            {
                                ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                                ////newRow.AppendChild(cell);

                                object dataValue = dsrow[col].ToString();
                                openXmlExportHelper.WriteCellValueSax(writer, dataValue.ToString(), CellValues.InlineString);
                            }
                            writer.WriteEndElement(); //end of Row tag
                        }
                   

                      
                        writer.WriteEndElement(); //end of SheetData
                        writer.WriteEndElement(); //end of worksheet
                        writer.Close();
                    }
                   
                }

               
            }
        }


        public void ExportDataTOExcelMoreSheetnoheader(DataSet ds, string destination, string sheetNameDesr, string logopath, int allowlogo, int sheetval)
        {
            using (SpreadsheetDocument workbook = SpreadsheetDocument.Open(destination, true))
            {


                //////var workbookPart = workbook.AddWorkbookPart();
                //////workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                //////workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = Convert.ToUInt32(sheetval);

                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    //sheetStyle.Stylesheet = CreateStylesheet4();


                    ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);


                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
                    sheets.Append(sheet);

                    if (allowlogo == 1)
                    {

                        InsertImage(sheetPart.Worksheet, 0, 0, logopath);
                        Row row;
                        row = new Row() { RowIndex = 5 };
                        sheetData.Append(row);

                    }

                    int index = 1;
                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        ////headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        index++;
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            ////DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            ////cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            ////cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                            ////newRow.AppendChild(cell);

                            object dataValue = dsrow[col].ToString();



                            decimal n;
                            DateTime dt;
                            bool isNumeric = decimal.TryParse(dataValue.ToString(), out n);


                            ////if (dataValue != null)
                            ////{
                            ////    cell = CreateCellFromTemplate(workbook, colAlpha, rowNumber, cell, dataValue);
                            ////}

                            ////newRow.AppendChild(cell);
                            ////rowNumber++;

                            if (isNumeric == true)
                            {
                                ////cell.CellValue = new CellValue(index.ToString());
                                ////cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                                //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //cell.DataType = CellValues.String;
                                //cell.CellReference = new StringValue(cellRef);
                                //cell.StyleIndex = (UInt32)styleIndex;
                                //cell.CellValue = new CellValue(dataValue.ToString());
                                newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.Number });


                            }
                            else if (DateTime.TryParse(dataValue.ToString(), out dt))
                            {
                                DateTime dtValue;
                                string strValue = "";
                                if (DateTime.TryParse(dataValue.ToString(), out dtValue))
                                    strValue = dtValue.ToString("dd-mm-yyyy hh:mm:ss AM/PM");
                                newRow.AppendChild(new Cell() { CellValue = new CellValue(strValue), DataType = CellValues.Date });

                            }
                            else
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
                                string textvalin = dsrow[col].ToString();
                                string textvalout = Regex.Replace(textvalin, re, "");

                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
                                //newRow.AppendChild(new Cell() { CellValue = new CellValue(dataValue.ToString()), DataType = CellValues.String });
                                newRow.AppendChild(cell);

                            }



                        }

                        sheetData.AppendChild(newRow);
                    }
                }
            }
        }


        //////public List<T> GETSettlementDetail(string searchID, string searchOption, string reporttype, DateTime cpd, string classid, string conString = null)
        //////{
        //////    // IEnumerable<Get_POSCARDSCHEME> results = null;

        //////    string cppdate = cpd.ToString("dd-MMM-yy");

        //////    var p = new OracleDynamicParameters();
        //////    p.Add(":P_searchID", searchID, OracleDbType.Varchar2);
        //////    p.Add(":P_searchOption", searchOption, OracleDbType.Varchar2);
        //////    p.Add(":P_reporttype", reporttype, OracleDbType.Varchar2);
        //////    p.Add(":P_reportClass", classid, OracleDbType.Varchar2);
        //////    p.Add(":P_CPD", cppdate, OracleDbType.Varchar2);
        //////    p.Add(":CURSOR_", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

        //////    var results = Fetch(c => c.Query<T>("GET_SETTLEMETDETAIL", p, commandType: CommandType.StoredProcedure), null);
        //////    // List<dynamic> dynamicDt = rec.ToList();
        //////    return results.ToList();
        //////}
        //////public void ExportDataTOExcel(List<T> Ls, string destination, string sheetNameDesr, string logopath,int allowlogo)
        //////{
        //////    ConvertListToDataset CLDS = new ConvertListToDataset();
        //////    DataSet ds = CLDS.CreateDataSet(Ls);

        //////    using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
        //////    {
        //////        var workbookPart = workbook.AddWorkbookPart();
        //////        workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
        //////        workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

        //////        uint sheetId = 1;

        //////        foreach (DataTable table in ds.Tables)
        //////        {
        //////            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        //////            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        //////            var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        //////            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

        //////            //sheetStyle.Stylesheet = CreateStylesheet();
        //////            //sheetStyle.Stylesheet.Save();

        //////            ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

        //////            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        //////            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        //////            if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
        //////            {
        //////                sheetId =
        //////                    sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        //////            }

        //////            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
        //////            sheets.Append(sheet);

        //////            if (allowlogo==1)
        //////            {
        //////                InsertImage(sheetPart.Worksheet, 0, 0, logopath);
        //////                Row row;
        //////                row = new Row() { RowIndex = 10 };
        //////                sheetData.Append(row);

        //////            }


        //////            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

        //////            List<String> columns = new List<string>();
        //////            foreach (DataColumn column in table.Columns)
        //////            {
        //////                columns.Add(column.ColumnName);

        //////                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //////                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //////                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
        //////                headerRow.AppendChild(cell);
        //////            }

        //////            sheetData.AppendChild(headerRow);

        //////            foreach (DataRow dsrow in table.Rows)
        //////            {
        //////                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //////                foreach (String col in columns)
        //////                {
        //////                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //////                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;

        //////                    string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        //////                    string textvalin = dsrow[col].ToString();
        //////                    string textvalout = Regex.Replace(textvalin, re, "");

        //////                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
        //////                    newRow.AppendChild(cell);
        //////                }

        //////                sheetData.AppendChild(newRow);
        //////            }
        //////        }
        //////    }
        //////}
        //////public void ExportDataTOExcelMoreSheet(List<T> Ls, string destination, string sheetNameDesr, string logopath, int allowlogo,int sheetval)
        //////{


        //////    using (SpreadsheetDocument workbook = SpreadsheetDocument.Open(destination, true))
        //////    {
        //////        ConvertListToDataset CLDS = new ConvertListToDataset();
        //////        DataSet ds = CLDS.CreateDataSet(Ls);

        //////        //////var workbookPart = workbook.AddWorkbookPart();
        //////        //////workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
        //////        //////workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

        //////        uint sheetId = Convert.ToUInt32(sheetval);

        //////        foreach (DataTable table in ds.Tables)
        //////        {
        //////            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        //////            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        //////            var sheetStyle = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        //////            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

        //////            //sheetStyle.Stylesheet = CreateStylesheet();
        //////            //sheetStyle.Stylesheet.Save();

        //////            ////InsertImage(sheetPart.Worksheet, 0, 0, logopath);

        //////            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        //////            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);


        //////            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetNameDesr };
        //////            sheets.Append(sheet);

        //////            if (allowlogo == 1)
        //////            {
        //////                InsertImage(sheetPart.Worksheet, 0, 0, logopath);
        //////                Row row;
        //////                row = new Row() { RowIndex = 10 };
        //////                sheetData.Append(row);

        //////            }


        //////            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

        //////            List<String> columns = new List<string>();
        //////            foreach (DataColumn column in table.Columns)
        //////            {
        //////                columns.Add(column.ColumnName);

        //////                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //////                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //////                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
        //////                headerRow.AppendChild(cell);
        //////            }

        //////            sheetData.AppendChild(headerRow);

        //////            foreach (DataRow dsrow in table.Rows)
        //////            {
        //////                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //////                foreach (String col in columns)
        //////                {
        //////                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //////                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;

        //////                    string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        //////                    string textvalin = dsrow[col].ToString();
        //////                    string textvalout = Regex.Replace(textvalin, re, "");

        //////                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(textvalout);
        //////                    newRow.AppendChild(cell);
        //////                }

        //////                sheetData.AppendChild(newRow);
        //////            }
        //////        }
        //////    }
        //////}





        //////private static Row CreateHeader(UInt32 index)
        //////{
        //////    Row r = new Row();
        //////    r.RowIndex = index;

        //////    Cell c = new Cell();
        //////    c.DataType = CellValues.String;
        //////    c.StyleIndex = 5;
        //////    c.CellReference = "A" + index.ToString();
        //////    c.CellValue = new CellValue("Congratulations! You can now create Excel Open XML styles.");
        //////    r.Append(c);

        //////    return r;
        //////}

        //////private static Row CreateColumnHeader(UInt32 index)
        //////{
        //////    Row r = new Row();
        //////    r.RowIndex = index;

        //////    Cell c;
        //////    c = new Cell();
        //////    c.DataType = CellValues.String;
        //////    c.StyleIndex = 6;
        //////    c.CellReference = "A" + index.ToString();
        //////    c.CellValue = new CellValue("Product ID");
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.DataType = CellValues.String;
        //////    c.StyleIndex = 6;
        //////    c.CellReference = "B" + index.ToString();
        //////    c.CellValue = new CellValue("Date/Time");
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.DataType = CellValues.String;
        //////    c.StyleIndex = 6;
        //////    c.CellReference = "C" + index.ToString();
        //////    c.CellValue = new CellValue("Duration");
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.DataType = CellValues.String;
        //////    c.StyleIndex = 6;
        //////    c.CellReference = "D" + index.ToString();
        //////    c.CellValue = new CellValue("Cost");
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.DataType = CellValues.String;
        //////    c.StyleIndex = 8;
        //////    c.CellReference = "E" + index.ToString();
        //////    c.CellValue = new CellValue("Revenue");
        //////    r.Append(c);

        //////    return r;
        //////}

        //////private static Row CreateContent(UInt32 index, ref Random rd)
        //////{
        //////    Row r = new Row();
        //////    r.RowIndex = index;

        //////    Cell c;
        //////    c = new Cell();
        //////    c.CellReference = "A" + index.ToString();
        //////    c.CellValue = new CellValue(rd.Next(1000000000).ToString("d9"));
        //////    r.Append(c);

        //////    DateTime dtEpoch = new DateTime(1900, 1, 1, 0, 0, 0, 0);
        //////    DateTime dt = dtEpoch.AddDays(rd.NextDouble() * 100000.0);
        //////    TimeSpan ts = dt - dtEpoch;
        //////    double fExcelDateTime;
        //////    // Excel has "bug" of treating 29 Feb 1900 as valid
        //////    // 29 Feb 1900 is 59 days after 1 Jan 1900, so just skip to 1 Mar 1900
        //////    if (ts.Days >= 59)
        //////    {
        //////        fExcelDateTime = ts.TotalDays + 2.0;
        //////    }
        //////    else
        //////    {
        //////        fExcelDateTime = ts.TotalDays + 1.0;
        //////    }
        //////    c = new Cell();
        //////    c.StyleIndex = 1;
        //////    c.CellReference = "B" + index.ToString();
        //////    c.CellValue = new CellValue(fExcelDateTime.ToString());
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.StyleIndex = 2;
        //////    c.CellReference = "C" + index.ToString();
        //////    c.CellValue = new CellValue(((double)rd.Next(10, 10000000) + rd.NextDouble()).ToString("f4"));
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.StyleIndex = 3;
        //////    c.CellReference = "D" + index.ToString();
        //////    c.CellValue = new CellValue(((double)rd.Next(10, 10000) + rd.NextDouble()).ToString("f2"));
        //////    r.Append(c);

        //////    c = new Cell();
        //////    c.StyleIndex = 7;
        //////    c.CellReference = "E" + index.ToString();
        //////    c.CellValue = new CellValue(((double)rd.Next(10, 1000) + rd.NextDouble()).ToString("f2"));
        //////    r.Append(c);

        //////    return r;
        //////}

        protected static void InsertImage(Worksheet ws, long x, long y, long? width, long? height, string sImagePath)
        {
            try
            {
                WorksheetPart wsp = ws.WorksheetPart;
                DrawingsPart dp;
                ImagePart imgp;
                WorksheetDrawing wsd;

                ImagePartType ipt;
                switch (sImagePath.Substring(sImagePath.LastIndexOf('.') + 1).ToLower())
                {
                    case "png":
                        ipt = ImagePartType.Png;
                        break;
                    case "jpg":
                    case "jpeg":
                        ipt = ImagePartType.Jpeg;
                        break;
                    case "gif":
                        ipt = ImagePartType.Gif;
                        break;
                    default:
                        return;
                }

                if (wsp.DrawingsPart == null)
                {
                    //----- no drawing part exists, add a new one

                    dp = wsp.AddNewPart<DrawingsPart>();
                    imgp = dp.AddImagePart(ipt, wsp.GetIdOfPart(dp));
                    wsd = new WorksheetDrawing();
                }
                else
                {
                    //----- use existing drawing part

                    dp = wsp.DrawingsPart;
                    imgp = dp.AddImagePart(ipt);
                    dp.CreateRelationshipToPart(imgp);
                    wsd = dp.WorksheetDrawing;
                }

                using (FileStream fs = new FileStream(sImagePath, FileMode.Open))
                {
                    imgp.FeedData(fs);
                }

                int imageNumber = dp.ImageParts.Count<ImagePart>();
                if (imageNumber == 1)
                {
                    Drawing drawing = new Drawing();
                    drawing.Id = dp.GetIdOfPart(imgp);
                    ws.Append(drawing);
                }

                NonVisualDrawingProperties nvdp = new NonVisualDrawingProperties();
                nvdp.Id = new UInt32Value((uint)(1024 + imageNumber));
                nvdp.Name = "Picture " + imageNumber.ToString();
                nvdp.Description = "";
                DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
                picLocks.NoChangeAspect = true;
                picLocks.NoChangeArrowheads = true;
                NonVisualPictureDrawingProperties nvpdp = new NonVisualPictureDrawingProperties();
                nvpdp.PictureLocks = picLocks;
                NonVisualPictureProperties nvpp = new NonVisualPictureProperties();
                nvpp.NonVisualDrawingProperties = nvdp;
                nvpp.NonVisualPictureDrawingProperties = nvpdp;

                DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
                stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

                BlipFill blipFill = new BlipFill();
                DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
                blip.Embed = dp.GetIdOfPart(imgp);
                blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
                blipFill.Blip = blip;
                blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
                blipFill.Append(stretch);

                DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
                DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
                offset.X = 0;
                offset.Y = 0;
                t2d.Offset = offset;

                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(sImagePath);

                DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();

                if (width == null)
                    extents.Cx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
                else
                    extents.Cx = width;

                if (height == null)
                    extents.Cy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
                else
                    extents.Cy = height;

                bm.Dispose();
                t2d.Extents = extents;
                ShapeProperties sp = new ShapeProperties();
                sp.BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto;
                sp.Transform2D = t2d;
                DocumentFormat.OpenXml.Drawing.PresetGeometry prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry();
                prstGeom.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle;
                prstGeom.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();
                sp.Append(prstGeom);
                sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

                DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
                picture.NonVisualPictureProperties = nvpp;
                picture.BlipFill = blipFill;
                picture.ShapeProperties = sp;

                Position pos = new Position();
                pos.X = x;
                pos.Y = y;
                Extent ext = new Extent();
                ext.Cx = extents.Cx;
                ext.Cy = extents.Cy;
                AbsoluteAnchor anchor = new AbsoluteAnchor();
                anchor.Position = pos;
                anchor.Extent = ext;
                anchor.Append(picture);
                anchor.Append(new ClientData());
                wsd.Append(anchor);
                wsd.Save(dp);
            }
            catch (Exception ex)
            {
                throw ex; // or do something more interesting if you want
            }
        }

        protected static void InsertImage(Worksheet ws, long x, long y, string sImagePath)
        {
            InsertImage(ws, x, y, null, null, sImagePath);
        }

    }
}
