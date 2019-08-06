using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPPosMaster.Dapper.ExcelUtility
{
    public class SLExcelData
    {
        //Payer Account   Payer Account Type Payer Branch Code   Amount Payer A/C Narration Currency Ben Account No  Beneficiary Name    Banaficiary Bank Code Beneficiary Branch Code Narration
        public SLExcelStatus Status { get; set; }
        public Columns ColumnConfigurations { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> DataRows { get; set; }
        public string SheetName { get; set; }
        public class SLExcelStatus
        {
            public string Message { get; set; }
            public bool Success
            {
                get { return string.IsNullOrWhiteSpace(Message); }
            }
        }

        public SLExcelData()
        {
            Status = new SLExcelStatus();
            Headers = new List<string>();
            DataRows = new List<List<string>>();
        }
    }
}
