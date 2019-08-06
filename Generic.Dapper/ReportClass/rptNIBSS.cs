
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

//using TechPayApp.Dapper.Model; 
using System.Data.SqlClient;
using UPPosMaster.Dapper.Repository;
using UPPosMaster.Dapper.Model;
using UPPosMaster.Data;
using Oracle.ManagedDataAccess.Client;
using System.Dynamic;
using System;
using DocumentFormat.OpenXml.Packaging;

using System.IO;

using DocumentFormat.OpenXml;

using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection;
using OfficeOpenXml;
using UPPosMaster.Dapper.Data;

namespace UPPosMaster.Dapper.ReportClass
{
    class rptNIBSS : RepoBase, IrptNIBSS
    {
        IDapperProcSettings SETTREPORT = new DapperProcSettings();
       
        IDapperProc3Settings SETT3A = new DapperProc3Settings();
       

        public string GenSettlementNIBBS(DateTime SETTDATE,  string destinationPath, string lgopath, string conString)
        {
          
            DateTime SETT;

            DataSet ds1 = new DataSet(); DataSet ds2 = new DataSet(); DataSet ds3 = new DataSet(); DataSet ds4 = new DataSet();
            SETT = SETTDATE;

            ////string settlementpathdate = System.IO.Path.Combine(destinationPath, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));


            ////if (!Directory.Exists(settlementpathdate))
            ////{
            ////    System.IO.Directory.CreateDirectory(settlementpathdate);
            ////}
            

        

          

          
            
           
            //NIBSS MAST

            ds1 = new DataSet(); ds2 = new DataSet(); ds3 = new DataSet(); ds4 = new DataSet();
            string NIBBS = string.Empty;
            string NIBBSPath = System.IO.Path.Combine(destinationPath, "NIBSS MAST");
            if (!Directory.Exists(NIBBSPath))
            {
                System.IO.Directory.CreateDirectory(NIBBSPath);
            }

            string ONIBBSPath = System.IO.Path.Combine(NIBBSPath, SETT.ToString("dd-MMM-yyyy"));



            if (!Directory.Exists(ONIBBSPath))
            {
                System.IO.Directory.CreateDirectory(ONIBBSPath);
            }
            string reportNIBBS = ONIBBSPath + "\\" + "NIBSSMAST.xlsx";

            try
            {
                if (System.IO.File.Exists(reportNIBBS))
                {

                    System.IO.File.Delete(reportNIBBS);




                }
            }
            catch
            {

            }

            try
            {
                ds1 = SETTREPORT.GETSettlementDetail(null, null, "NIBSSMAST", SETT, "A", null);


                FileInfo newFile = new FileInfo(reportNIBBS);
                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    ExcelWorksheet ws = null;
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        ws = pck.Workbook.Worksheets.Add("Detail");
                        ws.Cells["A1"].LoadFromDataTable(ds1.Tables[0], true);
                    }

                    pck.Save();
                }


            }
            catch
            {

            }

            ds1.Dispose();
            ds1.Clear();

            ds2.Dispose();
            ds2.Clear();

            ds3.Dispose();
            ds3.Clear();

            ds4.Dispose();
            ds4.Clear();
            //NIBSS VISA

            ds1 = new DataSet(); ds2 = new DataSet(); ds3 = new DataSet(); ds4 = new DataSet();
            string NIBBSVISA = string.Empty;
            string NIBBSPathVISA = System.IO.Path.Combine(destinationPath, "NIBSS VISA");


            if (!Directory.Exists(NIBBSPathVISA))
            {
                System.IO.Directory.CreateDirectory(NIBBSPathVISA);
            }

            string ONIBBSPathVISA = System.IO.Path.Combine(NIBBSPathVISA, SETT.ToString("dd-MMM-yyyy"));
            if (!Directory.Exists(ONIBBSPathVISA))
            {
                System.IO.Directory.CreateDirectory(ONIBBSPathVISA);
            }
            string reportNIBBSVISA = ONIBBSPathVISA + "\\" + "NIBSSVISA.xlsx";

            try
            {
                if (System.IO.File.Exists(reportNIBBSVISA))
                {

                    System.IO.File.Delete(reportNIBBSVISA);




                }
            }
            catch
            {

            }

            try
            {
                ds1 = SETTREPORT.GETSettlementDetail(null, null, "NIBSSVISA", SETT, "A", null);


                FileInfo newFile = new FileInfo(reportNIBBSVISA);
                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    ExcelWorksheet ws = null;
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        ws = pck.Workbook.Worksheets.Add("Detail");
                        ws.Cells["A1"].LoadFromDataTable(ds1.Tables[0], true);
                    }

                    pck.Save();
                }

            }
            catch
            {

            }

            ds1.Dispose();
            ds1.Clear();

            ds2.Dispose();
            ds2.Clear();

            ds3.Dispose();
            ds3.Clear();

            ds4.Dispose();
            ds4.Clear();

            ds1 = new DataSet(); ds2 = new DataSet(); ds3 = new DataSet(); ds4 = new DataSet();
            string NIBBSCOLL = string.Empty;
            string NIBBSPathCOLL = System.IO.Path.Combine(destinationPath, "NIBSS COLLECTION");

            if (!Directory.Exists(NIBBSPathCOLL))
            {
                System.IO.Directory.CreateDirectory(NIBBSPathCOLL);
            }

            string ONIBBSPathCOLL = System.IO.Path.Combine(NIBBSPathCOLL, SETT.ToString("dd-MMM-yyyy"));
            if (!Directory.Exists(ONIBBSPathCOLL))
            {
                System.IO.Directory.CreateDirectory(ONIBBSPathCOLL);
            }
            string reportNIBBSCOLL = ONIBBSPathCOLL + "\\" + "NIBSSCOLL.xlsx";

            try
            {
                if (System.IO.File.Exists(reportNIBBSCOLL))
                {

                    System.IO.File.Delete(reportNIBBSCOLL);




                }
            }
            catch
            {

            }

            try
            {
                ds1 = SETTREPORT.GETSettlementDetail(null, null, "NIBSS", SETT, "A", null);


                FileInfo newFile = new FileInfo(reportNIBBSCOLL);
                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    ExcelWorksheet ws = null;
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        ws = pck.Workbook.Worksheets.Add("Detail");
                        ws.Cells["A1"].LoadFromDataTable(ds1.Tables[0], true);
                    }

                    pck.Save();
                }

            }
            catch (Exception ex)
            {

            }

            ds1.Dispose();
            ds1.Clear();

            ds2.Dispose();
            ds2.Clear();

            ds3.Dispose();
            ds3.Clear();

            ds4.Dispose();
            ds4.Clear();
            return null;

        }

    }
}