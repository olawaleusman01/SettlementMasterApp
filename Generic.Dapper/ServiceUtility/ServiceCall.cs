using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ;
using Generic.Dapper;

namespace TechClearingProject.Data.ServiceUtility
{
  public class ServiceCall
    {
        //public PostingResponse TransactionPosting(PostingRequest pr)
        //{
        //    PostingResponse rs = null;
        //    try
        //    {
               
        //        Generic.Dapper.localhost1.Service vv = new Generic.Dapper.localhost.Service();
        //        var dd = vv.TransactionPosting(pr.TransRefNo, pr.DrAcctNo, pr.CrAcctNo, pr.Narration, pr.TranCode, pr.ValueDate, pr.TranAmount);
        //        if (dd != null)
        //        {
        //            DataTable exDT = dd.Tables[0];
        //            rs = new PostingResponse();
        //            rs.ResponseCode = int.Parse(exDT.Rows[0][0].ToString());
        //            rs.ResponseMsg = exDT.Rows[0][1].ToString();
        //            //rs.nBalance = decimal.Parse(exDT.Rows[0][2].ToString());
        //            //rs.sName = exDT.Rows[0][3].ToString();
        //            //rs.sStatus = exDT.Rows[0][4].ToString();
        //            //rs.nBranch = exDT.Rows[0][5].ToString();
        //            //rs.sCrncyIso = exDT.Rows[0][6].ToString();
        //            //rs.sAddress = exDT.Rows[0][7].ToString(); ;
        //            //rs.sTransNature = exDT.Rows[0][8].ToString();
        //            //rs.sChequeStatus = exDT.Rows[0][9].ToString();
        //            //rs.sAccountType = exDT.Rows[0][10].ToString();
        //            //rs.sProductCode = exDT.Rows[0][11].ToString();
        //        }


        //        return rs;
        //    }
        //    catch (System.Net.WebException ex)
        //    {
        //        rs = new PostingResponse();
        //       // rs.ResponseCode = true;
        //        rs.ResponseCode = -99;
        //        rs.ResponseMsg = "Connection to Core Banking System could not be established. Please contact System Administrator";
        //        return rs;

        //    }
        //    catch(Exception ex)
        //    {
        //        rs = new PostingResponse();
        //        rs.ResponseCode = -99;
        //        rs.ResponseMsg = ex.Message; // "Connection to Core Banking System could not be established. Please contact System Administrator";
        //        return rs;
        //    }
        //}
        public UserValResponse UserValidation(string UserId)
        {
            UserValResponse rs = null;
            try
            {

                Generic.Dapper.localhost1.CoreBankProcess vv = new Generic.Dapper.localhost1.CoreBankProcess();
                var dd =  vv.UserAuthLocal(UserId);
                if (dd != null)
                {
                    DataTable exDT = dd.Tables[0];
                    rs = new UserValResponse();
                    rs.RespCode = int.Parse(exDT.Rows[0][0].ToString());
                    rs.RespMessage = exDT.Rows[0][1].ToString();
                    rs.FullName = exDT.Rows[0][2].ToString();
                    rs.DeptCode = exDT.Rows[0][3].ToString();
                    rs.UserId = exDT.Rows[0][4].ToString();
                    rs.BranchCode = exDT.Rows[0][5].ToString();
                    rs.Email = exDT.Rows[0][6].ToString();
                    rs.MobilePhone = exDT.Rows[0][7].ToString();
                    //rs.nBalance = decimal.Parse(exDT.Rows[0][2].ToString());
                    //rs.sName = exDT.Rows[0][3].ToString();
                    //rs.sStatus = exDT.Rows[0][4].ToString();
                    //rs.nBranch = exDT.Rows[0][5].ToString();
                    //rs.sCrncyIso = exDT.Rows[0][6].ToString();
                    //rs.sAddress = exDT.Rows[0][7].ToString(); ;
                    //rs.sTransNature = exDT.Rows[0][8].ToString();
                    //rs.sChequeStatus = exDT.Rows[0][9].ToString();
                    //rs.sAccountType = exDT.Rows[0][10].ToString();
                    //rs.sProductCode = exDT.Rows[0][11].ToString();
                }


                return rs;
            }
            catch (System.Net.WebException ex)
            {
                rs = new UserValResponse();
                // rs.ResponseCode = true;
                rs.RespCode = -99;
                rs.RespMessage = "Connection to Core Banking System could not be established. Please contact System Administrator";
                return rs;

            }
            catch (Exception ex)
            {
                rs = new UserValResponse();
                rs.RespCode = -99;
                rs.RespMessage = ex.Message; // "Connection to Core Banking System could not be established. Please contact System Administrator";
                return rs;
            }
        }
        public UserValResponse UserAuthentication(string UserId,string password)
        {
            UserValResponse rs = null;
            try
            {

                Generic.Dapper.localhost1.CoreBankProcess vv = new Generic.Dapper.localhost1.CoreBankProcess();
                var dd = vv.UserPasswordAuthLocal(UserId,password);
                if (dd != null)
                {
                    DataTable exDT = dd.Tables[0];
                    rs = new UserValResponse();
                    rs.RespCode = int.Parse(exDT.Rows[0][0].ToString());
                    rs.RespMessage = exDT.Rows[0][1].ToString();
                   
                    
                }


                return rs;
            }
            catch (System.Net.WebException ex)
            {
                rs = new UserValResponse();
                // rs.ResponseCode = true;
                rs.RespCode = -99;
                rs.RespMessage = "Connection to Core Banking System could not be established. Please contact System Administrator";
                return rs;

            }
            catch (Exception ex)
            {
                rs = new UserValResponse();
                rs.RespCode = -99;
                rs.RespMessage = ex.Message; // "Connection to Core Banking System could not be established. Please contact System Administrator";
                return rs;
            }
        }
        //public AcctValResponse CustomerDetail(string acctNo)
        //{
        //    AcctValResponse rs = null;
        //    try
        //    {

        //        Generic.Dapper.localhost.Service vv = new Generic.Dapper.localhost.Service();
        //        var dd = vv.CliqCustomerEnquiry(acctNo);
        //        if (dd != null)
        //        {
        //            DataTable exDT = dd.Tables[0];
        //            rs = new AcctValResponse();
        //            rs.ResponseCode = int.Parse(exDT.Rows[0][0].ToString());
        //            rs.ResponseMsg = exDT.Rows[0][1].ToString();
        //            rs.nBalance = decimal.Parse(exDT.Rows[0][2].ToString());
        //            //rs.sName = exDT.Rows[0][3].ToString();
        //            //rs.sStatus = exDT.Rows[0][4].ToString();
        //            //rs.nBranch = exDT.Rows[0][5].ToString();
        //            //rs.sCrncyIso = exDT.Rows[0][6].ToString();
        //            //rs.sAddress = exDT.Rows[0][7].ToString(); ;
        //            //rs.sTransNature = exDT.Rows[0][8].ToString();
        //            //rs.sChequeStatus = exDT.Rows[0][9].ToString();
        //            //rs.sAccountType = exDT.Rows[0][10].ToString();
        //            //rs.sProductCode = exDT.Rows[0][11].ToString();
        //        }


        //        return rs;
        //    }
        //    catch (System.Net.WebException ex)
        //    {
        //        rs = new AcctValResponse();
        //        // rs.ResponseCode = true;
        //        rs.ResponseCode = -99;
        //        rs.ResponseMsg = "Connection to Core Banking System could not be established. Please contact System Administrator";
        //        return rs;

        //    }
        //    catch (Exception ex)
        //    {
        //        rs = new AcctValResponse();
        //        rs.ResponseCode = -99;
        //        rs.ResponseMsg = ex.Message; // "Connection to Core Banking System could not be established. Please contact System Administrator";
        //        return rs;
        //    }
        //}
    }
    public class PostingRequest
    {
        public string TransRefNo { get; set; }
        public string DrAcctNo { get; set; }
        public string TranCode { get; set; }
        public string Narration { get; set; }
        public double TranAmount { get; set; }
        public string CrAcctNo { get; set; }      
        public DateTime ValueDate { get; set; }
    }

    public class UserValResponse
    {
        public int? RespCode {get; set;}
        public string RespMessage {get; set;}
        public string FullName {get; set;}
        public string Email { get; set; }
        public string DeptCode { get; set; }
        public string UserId { get; set; }
        public string BranchCode { get; set; }
        public string MobilePhone { get; set; }
    }
    public class CustomerDetailResponse
    {
        public int? ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string Ledgerbalance { get; set; }
        public string PendingCharge { get; set; }
        public string Closingbalance { get; set; }
        public string AvailableBalance { get; set; }
        public string Lienamount { get; set; }
        public string AccountID { get; set; }
        public string Currency { get; set; }
        public string PhoneNo { get; set; }
        public string Status { get; set; }
        public string customertype { get; set; }
        public string Email { get; set; }

        public string BranchCode { get; set; }
        public string ProductCode { get; set; }
        public string CustomerId { get; set; }
        public string OldAcctNo { get; set; }
        public string Address { get; set; }
        public DateTime? DOB { get; set; }
    
        public string CustomerType { get; set; }
        public string Fullname { get; set; }
        public string Phone1 { get; set; }
        public string StateName { get; set; }
        public string TownName { get; set; }
        public string RequestType { get; set; }
        public string DOBString
        {
            get
            {
                return DOB != null ? ((DateTime)DOB).ToString("dd/MM/yyyy") : "";
            }
        }
    }
    public class PostingResponse
    {
        public int? ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
   
    }

    public class ProcessingDateResponse
    {
        public int nErrorCode { get; set; }
        public string sErrorText { get; set; }
        public DateTime dDate { get; set; }
      
    }
   
    // psTransactionRef,PnDirection,psAccountType,psCurrencyISO,psUserName

    public class ReleaseFundRequest
    {
        public string psTransactionRef { get; set; }
        public int pnInstrument { get; set; }
        public int pnDirection { get; set; }
        public string psAccountNo { get; set; }
        public string psAccountType { get; set; }   
    }

}
