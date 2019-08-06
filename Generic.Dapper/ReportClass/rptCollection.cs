
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
    class rptCollection : RepoBase, IrptCollection
    {
        IDapperProcSettings SETTREPORT = new DapperProcSettings();

        IDapperProc3Settings SETT3A = new DapperProc3Settings();

        public string GenSettlementCollection(DateTime SETTDATE,  string settlementpathdate, string lgopath, string conString)
        {

            DateTime SETT;

            DataSet ds1 = new DataSet(); DataSet ds2 = new DataSet(); DataSet ds3 = new DataSet(); DataSet ds4 = new DataSet();
            SETT = SETTDATE;

            ////string settlementpathdate = System.IO.Path.Combine(destinationPath, "SETTLEMENT_DETAIL_SETT_" + SETT.ToString("dd-MMM-yyyy"));


            ////if (!Directory.Exists(settlementpathdate))
            ////{
            ////    System.IO.Directory.CreateDirectory(settlementpathdate);
            ////}


            

         


            //COLLECTION MERCHANTS
            string merchantid = string.Empty;
            string Customerid = string.Empty;
            string merchantname = string.Empty;
            int recCounter;

            string COLLECTIONS = System.IO.Path.Combine(settlementpathdate, "COLLECTIONS");

            if (!Directory.Exists(COLLECTIONS))
            {
                System.IO.Directory.CreateDirectory(COLLECTIONS);
            }

            string qryM = "Select distinct Customerid from POSMISDB_MERCHANTDETAIL where lower(status)='active' and collection=1 and Customerid in (Select customerid from posmisdb_settlementdetail where settlementdate='" + SETT.ToString("dd-MMM-yyyy") + "') order by Customerid";
            var recListM = Fetch(c => c.Query<POSMISDB_MERCHANTDETAIL>(qryM, null, commandType: CommandType.Text), conString);


            if (recListM.Count() > 0)
            {
                foreach (var tM in recListM)
                {
                    //BANK LIST
                    string qryMDetail = "Select distinct MERCHANTID,MERCHANTNAME,SHORTCODE from POSMISDB_MERCHANTDETAIL where Customerid=" + tM.CUSTOMERID + " and collection=1 and rownum<2 order by MERCHANTNAME";
                    var recListMDetail = Fetch(c => c.Query<POSMISDB_MERCHANTDETAIL>(qryMDetail, null, commandType: CommandType.Text), conString).FirstOrDefault();

                    recCounter = 0;

                    string shortname = string.Empty;
                    merchantname = recListMDetail.MERCHANTNAME;
                    if (recListMDetail.MERCHANTNAME.Length > 10)
                    {
                        if (recListMDetail.SHORTCODE != string.Empty)
                        {

                            try { shortname = recListMDetail.SHORTCODE.ToUpper(); }
                            catch { shortname = recListMDetail.MERCHANTNAME.Substring(0, 10).ToUpper(); }


                        }
                        else
                        {
                            shortname = recListMDetail.MERCHANTNAME.Substring(0, 10).ToUpper();
                        }

                    }
                    else
                    {
                        shortname = recListMDetail.MERCHANTNAME;
                    }
                    merchantid = recListMDetail.MERCHANTID;
                    merchantname = merchantname.Replace("/", "");
                    Customerid = tM.CUSTOMERID.GetValueOrDefault().ToString();

                    string qryMCount = "Select nvl(count(1),0) retval from POSMISDB_REVENUEHEAD where merchantid in (Select merchantid from POSMISDB_MERCHANTDETAIL where Customerid=" + tM.CUSTOMERID + ")";
                    var recListMDetailMsg = Fetch(c => c.Query<RespMesg>(qryMCount, null, commandType: CommandType.Text), conString).FirstOrDefault();
                    recCounter = 0;
                    recCounter = recListMDetailMsg.retval;

                    string merchantnamePath = System.IO.Path.Combine(COLLECTIONS, merchantname);

                    if (!Directory.Exists(merchantnamePath))
                    {
                        System.IO.Directory.CreateDirectory(merchantnamePath);
                    }

                    string merchantnamePathDate = System.IO.Path.Combine(merchantnamePath, SETT.ToString("dd-MMM-yyyy"));


                    if (!Directory.Exists(merchantnamePathDate))
                    {
                        System.IO.Directory.CreateDirectory(merchantnamePathDate);
                    }

                    string reportnameCollection = merchantnamePathDate + "\\" + shortname.Trim() + ".xlsx";

                    try
                    {
                        if (System.IO.File.Exists(reportnameCollection))
                        {
                            foreach (string file in System.IO.Directory.GetFiles(reportnameCollection))
                            {
                                System.IO.File.Delete(file);
                            }



                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        ds1 = new DataSet(); ds2 = new DataSet(); ds3 = new DataSet(); ds4 = new DataSet();

                        ds1 = SETTREPORT.COLLECTIONSETT(merchantid, Customerid, SETT, null);


                        ////try
                        ////{
                        ////    ds2 = SETTREPORT.COLLECTIONNIBSS(merchantid, SETT, null);
                        ////}
                        ////catch (Exception ex)
                        ////{ }


                        FileInfo newFile = new FileInfo(reportnameCollection);

                        MemoryStream stream = new MemoryStream();
                         
                        using (ExcelPackage pck = new ExcelPackage(stream))
                        {
                            ExcelWorksheet ws = null;
                            ExcelWorksheet ws2 = null;
                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                ws = pck.Workbook.Worksheets.Add("Detail");
                                ws.Cells["A1"].LoadFromDataTable(ds1.Tables[0], true);
                            }

                            ////if (ds2.Tables[0].Rows.Count > 0)
                            ////{
                            ////    ws2 = pck.Workbook.Worksheets.Add("Nibss");
                            ////    ws2.Cells["A1"].LoadFromDataTable(ds2.Tables[0], true);
                            ////}

                            pck.Save();
                            stream.Dispose();
                            pck.Dispose();
                        }


                    }
                    catch (Exception ex)
                    {

                    }

                    // GC.Collect();();

                }
            }

            ds1.Dispose();
            ds1.Clear();

            ds2.Dispose();
            ds2.Clear();

            ds3.Dispose();
            ds3.Clear();

            ds4.Dispose();
            ds4.Clear();
      
            //////return true;
            //ERROR AND UNSETTLED


        
            return null;

        }


    }
}