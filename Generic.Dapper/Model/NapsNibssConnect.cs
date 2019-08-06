using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{

    public class requestPaymentObj
    {
        public string Beneficiary { get; set; }
        public string Amount { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string Narration { get; set; }
    }

    public class jsonAuthKey
    {
        public string AppUser { get; set; }
        public string Password { get; set; }
        public string FileName { get; set; }
        public string ScheduleId { get; set; }
        public string DebitSortCode { get; set; }
        public string DebitAccountNumber { get; set; }
    }

    public class JsonObjectStatus
    {
        public string AppUser { get; set; }
        public string Password { get; set; }
        public string ScheduleId { get; set; }
    }



    public class paymentRequestObj
    {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string sortCode { get; set; }
        public string narration { get; set; }
        public int count { get; set; }
        public string referenceNumber { get; set; }
        public List<Payment> payments { get; set; }
    }

    public class Payment
    {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string sortCode { get; set; }
        public decimal amount { get; set; }
        public string narration { get; set; }
        public string referenceNumber { get; set; }
    }


    public class paymentResponseObj
    {
        public string status { get; set; }
        public data data { get; set; }
    }

    public class data
    {
        public payment payment { get; set; }
    }

    public class payment
    {
        public string uniqueKey { get; set; }
        public string institution { get; set; }
        public string narration { get; set; }
        public decimal amount { get; set; }
        public string accountNumber { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }


    public class tokenResponsetObj
    {
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Token token { get; set; }
    }

    public class Token
    {
        public string token { get; set; }
        public DateTime expires { get; set; }
        public string tokenType { get; set; }
    }






}
namespace Generic.Dapper.Model2
{
    public class Data
    {
        public Payments payments { get; set; }
    }

    public class Data2
    {
        public Payments2 paymentItems { get; set; }
    }

    public class paymentEnquiryObj
    {
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class paymentEnquiryObj2
    {
        public string status { get; set; }
        public Data2 data { get; set; }
    }

    public class Payments
    {
        public int pageNumber { get; set; }
        public int size { get; set; }
        public int numberOfElements { get; set; }
        public bool last { get; set; }
        public int totalPages { get; set; }
        public List<Content> content { get; set; }
        public bool first { get; set; }
        public int totalElements { get; set; }
    }

    public class Payments2
    {
        public int pageNumber { get; set; }
        public int size { get; set; }
        public int numberOfElements { get; set; }
        public bool last { get; set; }
        public int totalPages { get; set; }
        public List<Content2> content { get; set; }
        public bool first { get; set; }
        public int totalElements { get; set; }
    }

    public class Content
    {
        public string uniqueKey { get; set; }
        public string institution { get; set; }
        public string narration { get; set; }
        public decimal amount { get; set; }
        public string accountNumber { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class Content2
    {
        public string uniqueKey { get; set; }
        public string institution { get; set; }
        public string narration { get; set; }
        public decimal amount { get; set; }
        public string accountNumber { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        public string reference { get; set; }
        public string payment { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryBankCode { get; set; }
        public string bankName { get; set; }
        public string beneficiaryAccountNumber { get; set; }

    }
}