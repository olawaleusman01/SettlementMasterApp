using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Generic.Dapper.Data
{
    public static class ExcelHelper
    {
        //Generic list extension.
        public static byte[] ToExcelBytes<T>(this IList<T> list, string include = "", string exclude = "", string columnFixes = "", string sheetName = "")
        {
            byte[] excelBytes;
            string booleanToBit = ConfigurationManager.AppSettings["BooleanToBit"];

            using (var memoryStream = new MemoryStream())
            {
                //Create a spreadsheet document in memory (by default the type is xlsx).
                using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    //Add a WorkbookPart to the document.
                    var workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();
                    workbookpart.Workbook.AppendChild(new FileVersion { ApplicationName = "Microsoft Office Excel" });

                    //Add a WorksheetPart to the WorkbookPart. 
                    var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();

                    //Associate the sheet data after the columns. 
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet.AppendChild(sheetData);

                    //Get simple type name
                    var typeName = GetSimpleTypeName(list);

                    //Add Sheets to the Workbook.
                    var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                    //Append a new worksheet and associate it with the workbook. 
                    sheets.AppendChild(new Sheet
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = sheetName == "" ? typeName : sheetName
                    });

                    //Get property collection and set selected property list.
                    PropertyInfo[] props = typeof(T).GetProperties();
                    List<PropertyInfo> propList = GetSelectedProperties(props, include, exclude);

                    //Cache columnSuffix string to list.
                    List<string> colFixList = null;
                    if (!string.IsNullOrEmpty(columnFixes))
                    {
                        colFixList = columnFixes.Split(',').ToList();
                    }
                    
                    //Add property name to the header row of sheet.
                    var headerRow = sheetData.AppendChild(new Row());
                    int colIdx = 0;
                    foreach (var prop in propList)
                    {
                        //Replace possible double underscores for friendly column names.
                        var colName = prop.Name.Replace("__", " ");
                        
                        //Use column names with added prefix or suffix items. 
                        if (colFixList != null)
                        {
                            foreach (var item in colFixList)
                            {
                                if (item.Contains(colName))
                                {
                                    colName = item;
                                    break;
                                }
                            }
                        }                       
                        
                        headerRow.AppendChild(new Cell
                        {
                            CellValue = new CellValue(colName),
                            DataType = CellValues.String,
                            CellReference = GetColumnAddress(colIdx) + "1"
                        });                        
                        colIdx++;
                    }                                      
                    
                    //Iterate through data list collection for rows.
                    int rowIdx = 1;
                    foreach (var item in list)
                    {
                        var contentRow = sheetData.AppendChild(new Row());
                        colIdx = 0;
                        //Iterate through property collection for columns.
                        foreach (var prop in propList)
                        {
                            //Do property value.                
                            var value = prop.GetValue(item, null);

                            if (booleanToBit == "Y" && prop.PropertyType == typeof(System.Boolean))
                            {
                                value = true ? "1" : "0";
                            }

                            //Assign value to cell
                            contentRow.AppendChild(new Cell
                            {
                                CellValue = new CellValue(value != null ? value.ToString() : null),
                                DataType = CellValues.String,
                                CellReference = GetColumnAddress(colIdx) + (rowIdx + 1).ToString()
                            });

                            /* 
                            cell = new Cell();
                            cell.SetAttribute(new OpenXmlAttribute("", "t", "", "inlineStr"));
                            cell.InlineString = new InlineString { Text = new Text { Text = "Hi" } };
                            row.Append(cell); 
                             */

                            colIdx++;
                        }
                        rowIdx++;
                    }
                    workbookpart.Workbook.Save();
                }
                //Convert memory stream to byte array
                excelBytes = memoryStream.ToArray();
            }
            return excelBytes;
        }

        public static byte[] ExportDataSet(DataTable table, string reportname = null)
        {
            byte[] excelBytes;
            using (var memoryStream = new MemoryStream())
            {
                using (var workbook = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = workbook.AddWorkbookPart();

                    workbook.WorkbookPart.Workbook = new Workbook();

                    workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                    //foreach (System.Data.DataTable table in ds.Tables)
                    //{

                        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                        var sheetData = new SheetData();
                        sheetPart.Worksheet = new Worksheet(sheetData);

                        Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                        uint sheetId = 1;
                        if (sheets.Elements<Sheet>().Count() > 0)
                        {
                            sheetId =
                                sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = reportname };
                        sheets.Append(sheet);

                        Row headerRow = new Row();

                        List<string> columns = new List<string>();
                        foreach (DataColumn column in table.Columns)
                        {
                            columns.Add(column.ColumnName);

                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(column.ColumnName);
                            headerRow.AppendChild(cell);
                        }


                        sheetData.AppendChild(headerRow);

                        foreach (DataRow dsrow in table.Rows)
                        {
                            Row newRow = new Row();
                            foreach (string col in columns)
                            {
                            //var d = CellValues.String;
                            // var type = dsrow[col].GetType();
                            //typeof(System.Boolean)
                            if (dsrow[col].GetType().Name != "String")
                            {
                                Cell cell = new Cell();
                                cell.DataType = GetCellType(dsrow[col].GetType()); // CellValues.String;
                                cell.CellValue = new CellValue(dsrow[col].ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else
                            {
                                Cell cell = new Cell();
                               // cell.DataType = GetCellType(dsrow[col].GetType()); // CellValues.String;
                                //cell.CellValue = new CellValue(dsrow[col].ToString()); //
                                cell.DataType = CellValues.InlineString;
                                cell.InlineString = new InlineString() { Text = new Text(dsrow[col].ToString()) };
                                newRow.AppendChild(cell);
                            }
                        
                        }

                            sheetData.AppendChild(newRow);
                        }

                    //}
                }
                excelBytes = memoryStream.ToArray();
            }
            return excelBytes;
        }

        public static byte[] ExportDatasetWithLogo(int startid, DataTable table, string reportname, string merchantname, int merchanttype, string address, string companyname, string logopath, string sett)
        {
            byte[] excelBytes;
            //FileInfo newFile = new FileInfo(filename);
            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                var wsA = package.Workbook.Worksheets.Add("DETAILS");
                System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                var pic = wsA.Drawings.AddPicture("LOGO", myImage);
                pic.SetPosition(0, 0, 0, 0);
                int colNumber = 0;
                var range = wsA.Cells[startid, 1].LoadFromDataTable(table, true);
              

                wsA.Row(startid).Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsA.Row(startid).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                wsA.Cells["A2"].Value = companyname;
                wsA.Cells["A2"].Style.Font.Size = 18;
                wsA.Cells["A2"].Style.Font.Bold = true;
                wsA.Cells["A2:M2"].Merge = true;
                wsA.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                wsA.Cells["A3"].Value = reportname;
                wsA.Cells["A3"].Style.Font.Size = 14;
                wsA.Cells["A3"].Style.Font.Bold = true;
                wsA.Cells["A3:M3"].Merge = true;
                wsA.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                wsA.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                if (merchanttype == 1)
                {
                    wsA.Cells["A5"].Value = "INSTITUTION NAME: " + merchantname;
                }
                else
                {
                    wsA.Cells["A5"].Value = "MERCHANT NAME: " + merchantname;
                }

                wsA.Cells["A5"].Style.Font.Size = 10;
                wsA.Cells["A5:F5"].Merge = true;
                wsA.Cells["A6"].Value = "ADDRESS: " + address;
                wsA.Cells["A6"].Style.Font.Size = 10;
                wsA.Cells["A6:F6"].Merge = true;

                //wsA.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                //wsA.Cells["A7"].Style.Font.Size = 10;
                //wsA.Cells["A7"].Style.Font.Bold = true;

                wsA.View.ShowGridLines = true;

                //if (table != null && table.Rows.Count > 1)
                //{
                //    int row = wsA.Dimension.End.Row + 1;
                //    //wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                //    wsA.Cells[2, 5, row + 1, 5].Style.Numberformat.Format = "dd-mm-yyyy";
                //    wsA.Cells[2, 6, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
                //}

                 int row = wsA.Dimension.End.Row + 1;
                foreach (DataColumn col in table.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                      
                         wsA.Column(colNumber).Style.Numberformat.Format = "dd-mm-yyyy"; // @"dd MMM yyyy hh:mm";

                        //wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                         //wsA.Cells[2, colNumber, row + 1, colNumber].Style.Numberformat.Format = "dd-mm-yyyy";
                        // wsA.Cells[2, 6, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
                    }
                }

                wsA.View.FreezePanes(startid + 1, 1);
                //var tbl1 = wsA.Tables[0];
                if (range != null)
                {
                    range.AutoFitColumns();
                }

                //excelBytes = stream.ToArray();
                // package.SaveAs(newFile);

                excelBytes = package.GetAsByteArray();
                package.Dispose();
                table.Dispose();
                stream.Dispose();

                ////var worksheet = package.Workbook.Worksheets[1];
                ////var maxColumnNumber = worksheet.Dimension.End.Column;
                ////var currentRow = new List<string>(maxColumnNumber);
                ////var totalRowCount = worksheet.Dimension.End.Row;
                ////var currentRowNum = 1;

                ////var memory = new MemoryStream();

                ////using (var writer = new StreamWriter(memory, Encoding.ASCII))
                ////{
                ////    while (currentRowNum <= totalRowCount)
                ////    {
                ////        BuildRow(worksheet, currentRow, currentRowNum, maxColumnNumber);
                ////        WriteRecordToFile(currentRow, writer, currentRowNum, totalRowCount);
                ////        currentRow.Clear();
                ////        currentRowNum++;
                ////    }
                ////}



            }

            return excelBytes;
        }
        public static byte[] ExportDatasetWithLogo2(int startid, DataTable table, string logopath,string companyname,string reportname)
        {
            byte[] excelBytes;
            //FileInfo newFile = new FileInfo(filename);
            MemoryStream stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream))
            {
                var wsA = package.Workbook.Worksheets.Add("DETAILS");
                System.Drawing.Image myImage = System.Drawing.Image.FromFile(logopath);
                var pic = wsA.Drawings.AddPicture("LOGO", myImage);
                pic.SetPosition(0, 0, 0, 0);
                int colNumber = 0;
                var range = wsA.Cells[startid, 1].LoadFromDataTable(table, true);


                wsA.Row(startid).Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsA.Row(startid).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

                wsA.Cells["A2"].Value = companyname;
                wsA.Cells["A2"].Style.Font.Size = 18;
                wsA.Cells["A2"].Style.Font.Bold = true;
                wsA.Cells["A2:M2"].Merge = true;
                wsA.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                wsA.Cells["A3"].Value = reportname;
                wsA.Cells["A3"].Style.Font.Size = 14;
                wsA.Cells["A3"].Style.Font.Bold = true;
                wsA.Cells["A3:M3"].Merge = true;
                wsA.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                //wsA.Cells["A5"].Value = "SETTLEMENT DATE: " + sett;
                //if (merchanttype == 1)
                //{
                //    wsA.Cells["A5"].Value = "INSTITUTION NAME: " + merchantname;
                //}
                //else
                //{
                //    wsA.Cells["A5"].Value = "MERCHANT NAME: " + merchantname;
                //}

               // wsA.Cells["A5"].Style.Font.Size = 10;
               // wsA.Cells["A5:F5"].Merge = true;
               //// wsA.Cells["A6"].Value = "ADDRESS: " + address;
               // wsA.Cells["A6"].Style.Font.Size = 10;
               // wsA.Cells["A6:F6"].Merge = true;

                //wsA.Cells["A7"].Value = "SETTLEMENT CURRENCY " + SettlementCur;
                //wsA.Cells["A7"].Style.Font.Size = 10;
                //wsA.Cells["A7"].Style.Font.Bold = true;

                wsA.View.ShowGridLines = true;

                //if (table != null && table.Rows.Count > 1)
                //{
                //    int row = wsA.Dimension.End.Row + 1;
                //    //wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                //    wsA.Cells[2, 5, row + 1, 5].Style.Numberformat.Format = "dd-mm-yyyy";
                //    wsA.Cells[2, 6, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
                //}

                int row = wsA.Dimension.End.Row + 1;
                foreach (DataColumn col in table.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {

                        wsA.Column(colNumber).Style.Numberformat.Format = "dd-mm-yyyy"; // @"dd MMM yyyy hh:mm";

                        //wsA.Cells[2, 4, row + 1, 4].Style.Numberformat.Format = "dd-mm-yyyy hh:mm:ss AM/PM";
                        //wsA.Cells[2, colNumber, row + 1, colNumber].Style.Numberformat.Format = "dd-mm-yyyy";
                        // wsA.Cells[2, 6, row + 1, 6].Style.Numberformat.Format = "dd-mm-yyyy";
                    }
                }

                wsA.View.FreezePanes(startid + 1, 1);
                //var tbl1 = wsA.Tables[0];
                if (range != null)
                {
                    range.AutoFitColumns();
                }

                //excelBytes = stream.ToArray();
                // package.SaveAs(newFile);

                excelBytes = package.GetAsByteArray();
                package.Dispose();
                table.Dispose();
                stream.Dispose();

                ////var worksheet = package.Workbook.Worksheets[1];
                ////var maxColumnNumber = worksheet.Dimension.End.Column;
                ////var currentRow = new List<string>(maxColumnNumber);
                ////var totalRowCount = worksheet.Dimension.End.Row;
                ////var currentRowNum = 1;

                ////var memory = new MemoryStream();

                ////using (var writer = new StreamWriter(memory, Encoding.ASCII))
                ////{
                ////    while (currentRowNum <= totalRowCount)
                ////    {
                ////        BuildRow(worksheet, currentRow, currentRowNum, maxColumnNumber);
                ////        WriteRecordToFile(currentRow, writer, currentRowNum, totalRowCount);
                ////        currentRow.Clear();
                ////        currentRowNum++;
                ////    }
                ////}



            }

            return excelBytes;
        }

        private static string DuplicateTicksForSql(this string s)
        {
            return s.Replace("'", "''");
        }
        public static string ToDelimitedString(this List<string> list, string delimiter = ":", bool insertSpaces = false, string qualifier = "", bool duplicateTicksForSQL = false)
        {
            var result = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                string initialStr = duplicateTicksForSQL ? list[i].DuplicateTicksForSql() : list[i];
                result.Append((qualifier == string.Empty) ? initialStr : string.Format("{1}{0}{1}", initialStr, qualifier));
                if (i < list.Count - 1)
                {
                    result.Append(delimiter);
                    if (insertSpaces)
                    {
                        result.Append(' ');
                    }
                }
            }
            return result.ToString();
        }

        private static void WriteRecordToFile(List<string> record, StreamWriter sw, int rowNumber, int totalRowCount)
        {
            var commaDelimitedRecord = record.ToDelimitedString(",");

            if (rowNumber == totalRowCount)
            {
                sw.Write(commaDelimitedRecord);
            }
            else
            {
                sw.WriteLine(commaDelimitedRecord);
            }
        }

        private static void BuildRow(ExcelWorksheet worksheet, List<string> currentRow, int currentRowNum, int maxColumnNumber)
        {
            for (int i = 1; i <= maxColumnNumber; i++)
            {
                var cell = worksheet.Cells[currentRowNum, i];
                if (cell == null)
                {
                    // add a cell value for empty cells to keep data aligned.
                    AddCellValue(string.Empty, currentRow);
                }
                else
                {
                    AddCellValue(GetCellText(cell), currentRow);
                }
            }
        }


        private static string GetCellText(ExcelRangeBase cell)
        {
            return cell.Value == null ? string.Empty : cell.Value.ToString();
        }

        private static void AddCellValue(string s, List<string> record)
        {
            record.Add(string.Format("{0}{1}{0}", '"', s));
        }


        private static List<PropertyInfo> GetSelectedProperties(PropertyInfo[] props, string include, string exclude)
        {
            List<PropertyInfo> propList = new List<PropertyInfo>();
            if (include != "") //Do include first
            {
                var includeProps = include.ToLower().Split(',').ToList();
                foreach (var item in props)
                {
                    var propName = includeProps.Where(a => a == item.Name.ToLower()).FirstOrDefault();
                    if (!string.IsNullOrEmpty(propName))
                        propList.Add(item);
                }
            }
            else if (exclude != "") //Then do exclude
            {
                var excludeProps = exclude.ToLower().Split(',');
                foreach (var item in props)
                {
                    var propName = excludeProps.Where(a => a == item.Name.ToLower()).FirstOrDefault();
                    if (string.IsNullOrEmpty(propName))
                        propList.Add(item);
                }
            }
            else //Default
            {
                propList.AddRange(props.ToList());
            }
            return propList;
        }

        private static string GetSimpleTypeName<T>(IList<T> list)
        {
            string typeName = list.GetType().ToString();
            int pos = typeName.IndexOf("[") + 1;
            typeName = typeName.Substring(pos, typeName.LastIndexOf("]") - pos);
            typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);
            return typeName;
        }
        private static CellValues GetCellType(Type type)
        {
            if (type.Name == "Int32")
            {
                return CellValues.Number;
            }
            else if (type.Name == "Decimal")
            {
                return CellValues.Number;
            }
            else
            {
                return CellValues.String;
            }
        }
        //For writing data to Excel
        private static String GetColumnAddress(int columnIndex)
        {
            /* if (columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex: " + columnIndex);
            } */
            Stack<char> stack = new Stack<char>();
            while (columnIndex >= 0)
            {
                stack.Push((char)('A' + (columnIndex % 26)));
                columnIndex = (columnIndex / 26) - 1;
            }
            return new String(stack.ToArray());
        }

        #region ExcelReader
        //Read Excel data to generic list - overloaded version 1.
        public static IList<T> GetDataToList<T>(string filePath, Func<IList<string>, IList<string>, T> addProductData)
        {
            return GetDataToList<T>(filePath, "", addProductData);
        }

        //Read Excel data to generic list - overloaded version 2.
        public static IList<T> GetDataToList<T>(string filePath, string sheetName, Func<IList<string>, IList<string>, T> addProductData)
        {
            List<T> resultList = new List<T>();

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                Sheet sheet = null;
                WorksheetPart wsPart = null;

                // Find the worksheet.                
                if (sheetName == "")
                {
                    sheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                }
                else
                {
                    sheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
                }
                if (sheet != null)
                {
                    // Retrieve a reference to the worksheet part.
                    wsPart = (WorksheetPart)(wbPart.GetPartById(sheet.Id));
                }
                if (wsPart == null)
                {
                    throw new Exception("No worksheet.");
                }
                else
                {
                    //List to hold custom column names for mapping data to columns (index-free).
                    var columnNames = new List<string>();

                    //List to hold column address letters for handling empty cell issue (handle empty cell issue).
                    var columnLetters = new List<string>();

                    //Iterate cells of custom header row.
                    foreach (Cell cell in wsPart.Worksheet.Descendants<Row>().ElementAt(0))
                    {
                        //Get custom column names.
                        //Remove spaces, symbols (except underscore), and make lower cases and for all values in columnNames list.                    
                        columnNames.Add(Regex.Replace(GetCellValue(document, cell), @"[^A-Za-z0-9_]", "").ToLower());

                        //Get built-in column names by extracting letters from cell references.
                        columnLetters.Add(GetColumnAddress(cell.CellReference));
                    }

                    //Used for sheet row data to be added through delegation.                
                    var rowData = new List<string>();

                    //Do data in rows
                    string cellLetter = string.Empty;

                    foreach (var row in GetUsedRows(document, wsPart))
                    {
                        rowData.Clear();

                        //Iterate through prepared enumerable.
                        foreach (var cell in GetCellsForRow(row, columnLetters))
                        {
                            rowData.Add(GetCellValue(document, cell));
                        }

                        //Calls the delegated function to add it to the collection.
                        resultList.Add(addProductData(rowData, columnNames));
                    }
                }
            }
            return resultList;
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            if (cell == null) return null;
            string value = cell.InnerText;

            //Process values particularly for those data types.
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    //Obtain values from shared string table.
                    case CellValues.SharedString:
                        var sstPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        value = sstPart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                        break;

                    //Optional boolean conversion.
                    case CellValues.Boolean:
                        var booleanToBit = ConfigurationManager.AppSettings["BooleanToBit"];
                        if (booleanToBit != "Y")
                        {
                            value = value == "0" ? "FALSE" : "TRUE";
                        }
                        break;
                }
            }
            return value;
        }

        private static IEnumerable<Row> GetUsedRows(SpreadsheetDocument document, WorksheetPart wsPart)
        {
            bool hasValue;
            //Iterate all rows except the first one.
            foreach (var row in wsPart.Worksheet.Descendants<Row>().Skip(1))
            {
                hasValue = false;
                foreach (var cell in row.Descendants<Cell>())
                {
                    //Find at least one cell with value for a row
                    if (!string.IsNullOrEmpty(GetCellValue(document, cell)))
                    {
                        hasValue = true;
                        break;
                    }
                }
                if (hasValue)
                {
                    //Return the row and keep iteration state.
                    yield return row;
                }
            }
        }

        private static IEnumerable<Cell> GetCellsForRow(Row row, List<string> columnLetters)
        {
            int workIdx = 0;
            foreach (var cell in row.Descendants<Cell>())
            {
                //Get letter part of cell address.
                var cellLetter = GetColumnAddress(cell.CellReference);

                //Get column index of the matched cell.  
                int currentActualIdx = columnLetters.IndexOf(cellLetter);

                //Add empty cell if work index smaller than actual index.
                for (; workIdx < currentActualIdx; workIdx++)
                {
                    var emptyCell = new Cell() { DataType = null, CellValue = new CellValue(string.Empty) };
                    yield return emptyCell;
                }

                //Return cell with data from Excel row.
                yield return cell;
                workIdx++;

                //Check if it's ending cell but there still is any unmatched columnLetters item.   
                if (cell == row.LastChild)
                {
                    //Append empty cells to enumerable. 
                    for (; workIdx < columnLetters.Count(); workIdx++)
                    {
                        var emptyCell = new Cell() { DataType = null, CellValue = new CellValue(string.Empty) };
                        yield return emptyCell;
                    }
                }
            }
        }

        private static string GetColumnAddress(string cellReference)
        {
            //Create a regular expression to get column address letters.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        //List extension for case-insensitive match
        public static int IndexFor(this IList<string> list, string name)
        {
            int idx = list.IndexOf(name.ToLower());
            if (idx < 0)
            {
                throw new Exception(string.Format("Missing required column mapped to: {0}.", name));
            }
            return idx;
        }
        #endregion

        #region Auto file path
        public static string CheckPath(string filePath)
        {
            var checkedPath = filePath;

            if (!(checkedPath.StartsWith(@"\\") && checkedPath.Contains(@":\")))
            {
                //Search app or processing folder 
                checkedPath = GetSearchedPath(AppDomain.CurrentDomain.BaseDirectory + filePath);
                if (string.IsNullOrEmpty(checkedPath))
                {
                    //Search dev project bin folder 
                    checkedPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.LastIndexOf(@"\bin\") + 5) + filePath;
                    checkedPath = GetSearchedPath(checkedPath);
                    if (string.IsNullOrEmpty(checkedPath))
                    {
                        //Search dev project folder
                        checkedPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf(@"\bin\") + 1) + filePath;
                        checkedPath = GetSearchedPath(checkedPath);
                    }
                }
            }
            else
            {
                //Search absolute path
                checkedPath = GetSearchedPath(checkedPath);
            }

            if (!string.IsNullOrEmpty(checkedPath))
            {
                return checkedPath;
            }
            else
            {
                throw new FileNotFoundException("Source file not found.");
            }
        }

        private static string GetSearchedPath(string searchPath)
        {
            var pos = searchPath.LastIndexOf(@"\") + 1;
            var pod = searchPath.LastIndexOf(@".");
            var srcfolder = searchPath.Substring(0, pos);
            var filePureName = searchPath.Substring(pos, pod - pos);
            var fileDotExt = searchPath.Substring(pod);

            //Get latest updated source file with searched partial name in the folder
            DirectoryInfo dir = new DirectoryInfo(srcfolder);
            if (!dir.Exists) return null;
            var srcFirstFile = dir.GetFiles().Where(fi => fi.Name.ToLower().Contains(filePureName.ToLower())
                      && fi.Name.ToLower().Contains(fileDotExt)).OrderByDescending(fi => fi.LastWriteTime).FirstOrDefault();
            if (srcFirstFile != null && srcFirstFile.Exists)
            {
                return srcFirstFile.FullName;
            }
            return null;
        }
        #endregion

        #region String Converters
        public static int ToInt32(this string source)
        {
            int outNum;
            return int.TryParse(source, out outNum) ? outNum : 0;
        }
        public static int? ToInt32Nullable(this string source)
        {
            int outNum;
            return int.TryParse(source, out outNum) ? outNum : (int?)null;
        }
        public static decimal ToDecimal(this string source)
        {
            decimal outNum;
            return decimal.TryParse(source, out outNum) ? outNum : 0;
        }

        public static decimal? ToDecimalNullable(this string source)
        {
            decimal outNum;
            return decimal.TryParse(source, out outNum) ? outNum : (decimal?)null;
        }

        public static double ToDouble(this string source)
        {
            double outNum;
            return double.TryParse(source, out outNum) ? outNum : 0;
        }

        public static double? ToDoubleNullable(this string source)
        {
            double outNum;
            return double.TryParse(source, out outNum) ? outNum : (double?)null;
        }

        public static DateTime ToDateTime(this string source)
        {
            DateTime outDt;
            if (DateTime.TryParse(source, out outDt))
            {
                return outDt;
            }
            else
            {
                //Check OLE Automation date time
                if (IsNumeric(source))
                {
                    return DateTime.FromOADate(source.ToDouble());
                }
                return DateTime.Now;
            }
        }

        public static DateTime? ToDateTimeNullable(this string source)
        {
            DateTime outDt;
            if (DateTime.TryParse(source, out outDt))
            {
                return outDt;
            }
            else
            {
                //Check and handle OLE Automation date time
                if (IsNumeric(source))
                {
                    return DateTime.FromOADate(source.ToDouble());
                }
                return (DateTime?)null;
            }
        }

        public static bool ToBoolean(this string source)
        {
            if (!string.IsNullOrEmpty(source))
                if (source.ToLower() == "true" || source == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            else
            {
                return false;
            }
        }
        public static bool? ToBooleanNullable(this string source)
        {
            if (!string.IsNullOrEmpty(source))
                if (source.ToLower() == "true" || source == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            else
            {
                return (bool?)null;
            }
        }

        public static Guid ToGuid(this string source)
        {
            Guid outGuid;
            return Guid.TryParse(source, out outGuid) ? outGuid : Guid.Empty;
        }

        public static Guid? ToGuidNullable(this string source)
        {
            Guid outGuid;
            return Guid.TryParse(source, out outGuid) ? outGuid : (Guid?)null;
        }
        #endregion

        #region Util
        private static readonly Regex _isNumericRegex = new Regex("^(" +
            /*Hex*/ @"0x[0-9a-f]+" + "|" +
            /*Bin*/ @"0b[01]+" + "|" +
            /*Oct*/ @"0[0-7]*" + "|" +
            /*Dec*/ @"((?!0)|[-+]|(?=0+\.))(\d*\.)?\d+(e\d+)?" +
            ")$");
        static bool IsNumeric(string value)
        {
            return _isNumericRegex.IsMatch(value);
        }
        #endregion
    }
}
