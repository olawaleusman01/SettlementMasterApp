using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.ExcelUtility
{
    public class ExxcellReaderClosedXml
    {
        public static IList<T> GetDataToList<T>(string filePath, Func<IList<string>, T> addRecord)
        {
            return GetDataToList<T>(filePath, "", addRecord);
        }

        //Read Excel data to generic list - overloaded version 2.
        public static IList<T> GetDataToList<T>(string filePath, string sheetName, Func<IList<string>, T> addRecord)
        {
            List<T> resultList = new List<T>();

            using (XLWorkbook wB = new XLWorkbook(filePath))
            {
                IXLWorksheet wS = wB.Worksheet(1);
                //DataTable dt = new DataTable();
                //bool firstRow = true;
                //Used for sheet row data to be added through delegation.                
                
                var firstRow = true;
                int lastRow = wS.LastRowUsed().RowNumber();
                var rows = wS.Rows(1, lastRow);
                foreach (IXLRow row in rows)
                {
                    if (firstRow)
                    {
                        ////foreach (IXLCell cell in row.Cells())
                        ////{
                        ////    dt.Columns.Add(cell.Value.ToString());
                        ////}
                        firstRow = false;
                        continue;
                    }
                    
                    if(row.IsEmpty())
                    {
                        continue;
                    }
                    var rowData = new List<string>();
                    //else
                    //{
                    //dt.Rows.Add();
                    //int i = 0;
                    //rowData.Clear();
                    foreach (IXLCell cell in row.Cells(wS.FirstCellUsed().Address.ColumnNumber, wS.LastCellUsed().Address.ColumnNumber))
                    {
                        rowData.Add(cell.Value.ToString());
                    }
                    //foreach (IXLCell cell in row.Cells())
                    //{
                    //    rowData.Add(cell.Value.ToString());
                    //}
                    resultList.Add(addRecord(rowData));
                    }


               // }
                return resultList;
            }
        }
    }
}
