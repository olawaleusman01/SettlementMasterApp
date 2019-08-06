using Dapper;
using Generic.Dapper.Data;
using Generic.Dapper.Model;
using Generic.Dapper.Model2;
using Generic.Dapper.Repository;
using Generic.Data.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Generic.Dapper.PostConnect
{
    public class NapsConnect
    {
        static LogFunction lgfn = new LogFunction();
        private static string sourceDb = ConfigurationManager.AppSettings["DEST_DB"].ToString().Trim();
        static string destUri = ConfigurationManager.AppSettings["naps_url"].ToString();
        static string appUser = ConfigurationManager.AppSettings["naps_appuser"].ToString();
        static string pass = ConfigurationManager.AppSettings["naps_password"].ToString();
        static string clientId = ConfigurationManager.AppSettings["naps_clientId"].ToString();
        static string clientSecret = ConfigurationManager.AppSettings["naps_clientSecret"].ToString();
        static string clientIdneft = ConfigurationManager.AppSettings["neft_clientId"].ToString();
        static string clientSecretneft = ConfigurationManager.AppSettings["neft_clientSecret"].ToString();


        //public void PostNaps()
        //{
        //    try
        //    {
        //        //var recP = GetNapsTranx(); // repoNaps.AllEagerLocal(d => d.BATCHID == _batchId && d.USERID == _userId).ToList();
        //        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " STARTING NAPS CONNECT ....>", null, null);
        //        //var destUri = ConfigurationManager.AppSettings["naps_url"].ToString();
        //        //var appUser = ConfigurationManager.AppSettings["naps_appuser"].ToString();
        //        //var pass = ConfigurationManager.AppSettings["naps_password"].ToString();
        //        //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] AFTER CONNECTION STRING", null, null);
        //        var stat = GetProcessStatus();
        //        if(stat == "P")
        //        {
        //            return;
        //        }
        //        using (var conn = new RepoBase().OpenConnection(sourceDb))
        //        {
        //            PostProcessStatus("P");
        //            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " PROCESSING NAPS", null, null);
        //            var sqlQuery = "proc_GetNapsToPost";
        //            var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        //            var recP = rec.ToList();
        //           // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT"+ recP.Count, null, null);
        //            foreach (var t in recP)
        //            {
        //                try
        //                {
        //                    var data = "";
        //                    var rst = "";
        //                    var scheduleId = SmartObj.GenRefNo();
        //                    var auth = new jsonAuthKey()
        //                    {
        //                        AppUser = appUser,
        //                        Password = pass,
        //                        FileName = "FilenameUploaded",
        //                        ScheduleId = scheduleId,
        //                        DebitSortCode = "050",// t.DEBITBANKCODE, --uncomment this for live production
        //                        DebitAccountNumber = "0123456789", // t.DEBITACCTNO --uncomment this for live production
        //                        //DebitSortCode = t.DEBITBANKCODE, // "050", --uncomment this for test production
        //                        //DebitAccountNumber = t.DEBITACCTNO, //"0123456789", --uncomment this for test production
        //                    };
        //                    var payObj = new requestPaymentObj()
        //                    {
        //                        Beneficiary = t.BENEFICIARYNAME,
        //                        AccountNumber = t.BENEFICIARYACCTNO,
        //                        Amount = t.CREDITAMOUNT.GetValueOrDefault().ToString("F"),
        //                        Narration = t.BENEFICIARYNARRATION,
        //                        SortCode = t.BENEFICIARYBANKCODE
        //                    };
        //                    var dList = new List<requestPaymentObj>();
        //                    dList.Add(payObj);
        //                    //var jsonPay = JsonConvert.SerializeObject(dList);
        //                    var authKey = JsonConvert.SerializeObject(auth);
        //                    var payJson = JsonConvert.SerializeObject(dList);
        //                    using (WebClient postClient = new WebClient())
        //                    {
        //                        postClient.Proxy = null;
        //                        //postClient.Headers.Add(HttpRequestHeader.ContentType, "application/soap+xml; charset=utf-8");
        //                        postClient.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
        //                        //postClient.Headers.Add("SOAPAction", "http://tempuri.org");

        //                        StreamReader reader = new StreamReader("C:\\NapsTemplate\\TranxPosting.xml");

        //                        data = string.Format(reader.ReadToEnd(), authKey, payJson);
        //                        reader.Close();

        //                        //  postClient.UploadStringCompleted += new UploadStringCompletedEventHandler(postClient_UploadStringCompleted);
        //                        try
        //                        {
        //                            rst = postClient.UploadString(new Uri(destUri), data);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
        //                            //t.RESPCODE = "98";
        //                            //t.RESPMESSAGE = ex.Message;
        //                            // t.SCHEDULEID = scheduleId;
        //                            PostNapsStatus(t.ITBID, "91", ex.Message,null,1, conn);
        //                            continue;
        //                        }
        //                        //ServicePointManager.Expect100Continue = true;
        //                        //.ServerCertificateValidationCallback = MyCertHandler;
        //                    }
        //                    var ret = postClient_UploadStringCompleted(rst);
        //                    if (ret != null)
        //                    {
        //                        //t.RESPCODE = ret.RespCode2;
        //                        //t.RESPMESSAGE = ret.RespMessage;
        //                        // t.SCHEDULEID = scheduleId;
        //                        //uow.Save(t.USERID);
        //                        PostNapsStatus(t.ITBID, ret.RespCode2, ret.RespMessage,scheduleId,1, conn);
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
        //                    PostNapsStatus(t.ITBID, "91", ex.Message,null,1, conn);
        //                    continue;
        //                }
        //            }

        //            PostProcessStatus("C");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] POST PROCESS STARTS NOW", ex.Message, null);
        //        PostProcessStatus("A");
        //    }
        //}

        ////////////////public string TranxEnquiryApi()
        ////////////////{
        ////////////////    try
        ////////////////    {
        ////////////////        using (var conn = new RepoBase().OpenConnection(sourceDb))
        ////////////////        {
        ////////////////            // PostProcessStatus("P");

        ////////////////            var sqlQuery = "proc_GetNapsForEnquiry";
        ////////////////            var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        ////////////////            var recP = rec.ToList();

        ////////////////            if (recP.Count() > 0)
        ////////////////            {
        ////////////////                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS GetNapsForEnquiry PROCESSING .....>", null, null);

        ////////////////                using (HttpClient client = new HttpClient())
        ////////////////                {
        ////////////////                    var token = GetAPIToken(clientId, clientSecret, destUri, client).Result;
        ////////////////                    // client.BaseAddress = new Uri(destUri);
        ////////////////                    //client.DefaultRequestHeaders.Accept.Clear();
        ////////////////                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        ////////////////                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        ////////////////                    foreach (var t in recP)
        ////////////////                    {

        ////////////////                        var data = "";
        ////////////////                        var rst = "";


        ////////////////                        try
        ////////////////                        {
        ////////////////                            //HttpResponseMessage response = await client.GetAsync("/external/v1/payments?reference=" + t.SCHEDULEID);
        ////////////////                            var result = GetTransaction(t.SCHEDULEID, clientId, clientSecret, destUri, token).Result;
        ////////////////                            ////response.EnsureSuccessStatusCode();
        ////////////////                            //if (response.IsSuccessStatusCode)
        ////////////////                            //{
        ////////////////                            var retObj = new OutPutObj();
        ////////////////                            // var result = response.Content.ReadAsStringAsync().Result;
        ////////////////                            // var result = await response.Content.ReadAsAsync<paymentEnquiryObj>();
        ////////////////                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAPS Get Status is Successful ---------->>>", null, null);

        ////////////////                            if (result.data.payments != null)
        ////////////////                            {
        ////////////////                                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NAPS Payment Records Available ---------->>>", null, null);

        ////////////////                                if (result.data.payments != null)
        ////////////////                                {
        ////////////////                                    retObj.RespCode2 = SmartObj.GetNapsMessage2(result.data.payments.content[0].status);
        ////////////////                                    retObj.RespMessage = result.data.payments.content[0].statusMessage;
        ////////////////                                    PostNapsStatus(t.ITBID, retObj.RespCode2, retObj.RespMessage, t.SCHEDULEID, 2, conn);
        ////////////////                                }
        ////////////////                            }

        ////////////////                            // }
        ////////////////                        }
        ////////////////                        catch
        ////////////////                        {
        ////////////////                            continue;
        ////////////////                        }

        ////////////////                    }

        ////////////////                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Resetting Status to CLOSE", null, null);

        ////////////////                    PostProcessStatus("C");
        ////////////////                }
        ////////////////            }
        ////////////////            // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT"+ recP.Count, null, null);

        ////////////////        }
        ////////////////    }
        ////////////////    catch (Exception ex)
        ////////////////    {
        ////////////////        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Error From Enquiry", ex.Message, null);
        ////////////////        PostProcessStatus("A");
        ////////////////    }

        ////////////////    return "";
        ////////////////}

        ////////////////public string PostNapsXPEApi()
        ////////////////{
        ////////////////    var token = "";
        ////////////////    try
        ////////////////    {
        ////////////////        using (HttpClient client = new HttpClient())
        ////////////////        {
        ////////////////            token = GetAPIToken(clientId, clientSecret, destUri, client).Result;
        ////////////////        }
        ////////////////        // client.BaseAddress = new Uri(destUri);
        ////////////////        //client.DefaultRequestHeaders.Accept.Clear();
        ////////////////        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        ////////////////        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        ////////////////        //var recP = GetNapsTranx(); // repoNaps.AllEagerLocal(d => d.BATCHID == _batchId && d.USERID == _userId).ToList();
        ////////////////        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " STARTING NAPS PROCESS ----------------->>>>", null, null);
        ////////////////        //var destUri = ConfigurationManager.AppSettings["naps_url"].ToString();
        ////////////////        //var appUser = ConfigurationManager.AppSettings["naps_appuser"].ToString();
        ////////////////        //var pass = ConfigurationManager.AppSettings["naps_password"].ToString();
        ////////////////        //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] AFTER CONNECTION STRING", null, null);
        ////////////////        var stat = GetProcessStatus();
        ////////////////        if (stat == "P")
        ////////////////        {
        ////////////////            return null;
        ////////////////        }

        ////////////////        using (var conn = new RepoBase().OpenConnection(sourceDb))
        ////////////////        {
        ////////////////            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Setting Process to POSTING .....>", null, null);


        ////////////////            var sqlQuery = "proc_GetNapsToPost";
        ////////////////            var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        ////////////////            var recGroup = rec.GroupBy(d => new { d.BATCHID }).ToList();
        ////////////////            paymentRequestObj requestObj = new paymentRequestObj();
        ////////////////            string batchId = "";
        ////////////////            string drBankCode = "";
        ////////////////            string drAcctNo = "";


        ////////////////            if (recGroup.Count() > 0)
        ////////////////            {


        ////////////////                PostProcessStatus("P");
        ////////////////                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT" + recGroup.Count, null, null);
        ////////////////                foreach (var recP in recGroup)
        ////////////////                {
        ////////////////                    var scheduleId = SmartObj.GenRefNo();
        ////////////////                    int i = 0;

        ////////////////                    foreach (var t in recP)
        ////////////////                    {
        ////////////////                        try
        ////////////////                        {
        ////////////////                            if (i == 0)
        ////////////////                            {
        ////////////////                                var amt = recP.Sum(f => f.CREDITAMOUNT);
        ////////////////                                requestObj = new paymentRequestObj()
        ////////////////                                {
        ////////////////                                    accountName = "XP NAPS",
        ////////////////                                    accountNumber = t.DEBITACCTNO,
        ////////////////                                    amount = amt.Value,
        ////////////////                                    count = recP.Count(),
        ////////////////                                    currency = "NGN",
        ////////////////                                    narration = t.BENEFICIARYNARRATION,
        ////////////////                                    sortCode = t.DEBITBANKCODE,
        ////////////////                                    referenceNumber = scheduleId,

        ////////////////                                };

        ////////////////                                batchId = t.BATCHID;
        ////////////////                                drBankCode = t.DEBITBANKCODE;
        ////////////////                                drAcctNo = t.DEBITACCTNO;

        ////////////////                                requestObj.payments = new List<Payment>();

        ////////////////                            }

        ////////////////                            var refNo = SmartObj.GenRefNo();

        ////////////////                            requestObj.payments.Add(new Payment()
        ////////////////                            {
        ////////////////                                accountName = t.BENEFICIARYNAME,
        ////////////////                                accountNumber = t.BENEFICIARYACCTNO,
        ////////////////                                amount = t.CREDITAMOUNT.GetValueOrDefault(),
        ////////////////                                narration = t.BENEFICIARYNARRATION,
        ////////////////                                referenceNumber = refNo,
        ////////////////                                sortCode = t.BENEFICIARYBANKCODE,

        ////////////////                            });


        ////////////////                            i++;
        ////////////////                        }
        ////////////////                        catch (Exception ex)
        ////////////////                        {
        ////////////////                            // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
        ////////////////                            //  PostNapsStatus(t.ITBID, "91", ex.Message, null, 1, conn);
        ////////////////                            break;
        ////////////////                        }
        ////////////////                    }

        ////////////////                    try
        ////////////////                    {
        ////////////////                        var retObj = new OutPutObj();
        ////////////////                        // var result = response.Content.ReadAsStringAsync().Result;
        ////////////////                        try
        ////////////////                        {
        ////////////////                            string json = JsonConvert.SerializeObject(requestObj);
        ////////////////                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Request json NAPS Data to NIBSS " + json.ToString(), null, null);

        ////////////////                        }
        ////////////////                        catch (Exception ex)
        ////////////////                        {

        ////////////////                        }
        ////////////////                        var result = PostTransaction(requestObj, clientId, clientSecret, destUri, token).Result;
        ////////////////                        if (result != null && result.data != null)
        ////////////////                        {
        ////////////////                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS POSTING TRANSACTION ", null, null);

        ////////////////                            retObj.RespCode2 = SmartObj.GetNapsMessage2(result.data.payment.status);
        ////////////////                            retObj.RespMessage = result.data.payment.status;
        ////////////////                            PostNapsStatus2(batchId, drBankCode, drAcctNo, null, result.data.payment.uniqueKey, retObj.RespCode2, retObj.RespMessage, scheduleId, 1, conn);
        ////////////////                        }
        ////////////////                        else
        ////////////////                        {
        ////////////////                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NO RESPONSE FROM NAPS", null, null);

        ////////////////                            retObj.RespCode2 = "92";
        ////////////////                            retObj.RespMessage = "Posting to Nibss Message cannot be interpreted.";
        ////////////////                            PostNapsStatus2(batchId, drBankCode, drAcctNo, null, null, retObj.RespCode2, retObj.RespMessage, scheduleId, 1, conn);
        ////////////////                        }
        ////////////////                    }
        ////////////////                    catch (Exception ex)
        ////////////////                    {
        ////////////////                        // PostNapsStatus(t.ITBID, "91", ex.Message, null, 1, conn);
        ////////////////                        PostNapsStatus2(batchId, drBankCode, drAcctNo, null, null, "91", ex.Message, scheduleId, 1, conn);

        ////////////////                    }

        ////////////////                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Resetting Status to CLOSE ", null, null);

        ////////////////                }


        ////////////////                PostProcessStatus("C");
        ////////////////            }

        ////////////////        }

        ////////////////    }
        ////////////////    catch (Exception ex)
        ////////////////    {
        ////////////////        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " ERROR FROM NAPS", ex.Message, null);
        ////////////////        PostProcessStatus("A");
        ////////////////    }
        ////////////////    return "";
        ////////////////}



        public string TranxEnquiryApi()
        {
            try
            {



                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    // PostProcessStatus("P");
                    var sqlQuery = "proc_GetNapsForEnquiry";
                    var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                    var recP = rec.ToList();

                    if (recP.Count() > 0)
                    {
                        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS GetNapsForEnquiry PROCESSING .....>", null, null);

                        using (HttpClient client = new HttpClient())
                        {

                            var token = GetAPIToken(clientId, clientSecret, destUri, client).Result;
                            // client.BaseAddress = new Uri(destUri);
                            //client.DefaultRequestHeaders.Accept.Clear();
                            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                            foreach (var t in recP)
                            {

                                var data = "";
                                var rst = "";


                                try
                                {
                                    //HttpResponseMessage response = await client.GetAsync("/external/v1/payments?reference=" + t.SCHEDULEID);
                                    var result = GetTransaction(t.UNIQUEKEY, clientId, clientSecret, destUri, token).Result;

                                    string json = JsonConvert.SerializeObject(result);
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Enquiry Response json  Data to from xpdirect " + json.ToString(), null, null);
                                    ////response.EnsureSuccessStatusCode();
                                    //if (response.IsSuccessStatusCode)
                                    //{
                                    var retObj = new OutPutObj();
                                    // var result = response.Content.ReadAsStringAsync().Result;
                                    // var result = await response.Content.ReadAsAsync<paymentEnquiryObj>();
                                    if (result != null && result.status == "SUCCESS")
                                    {
                                        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS  Get Status is Successful ---------->>>", null, null);

                                        //////if (result.data.payments != null)
                                        //////{

                                        //////    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS  Payment Records Available ---------->>>", null, null);

                                        //////    retObj.RespCode2 = SmartObj.GetNapsMessage2(result.data.payments.content[0].status);
                                        //////    retObj.RespMessage = result.data.payments.content[0].statusMessage;

                                        //////    PostNapsStatus(t.ITBID, retObj.RespCode2, retObj.RespMessage, t.SCHEDULEID, 2, conn);
                                        //////}
                                        ///
                                        if (result.data.paymentItems.content != null)
                                        {
                                            var nn = result.data.paymentItems.content.ToList();
                                            foreach (var m in nn)
                                            {
                                                PostNapsStatus(m.beneficiaryAccountNumber, m.amount, m.narration, m.payment, m.status, m.statusMessage, m.reference, conn);
                                            }

                                        }
                                    }

                                    // }
                                }
                                catch
                                {
                                    continue;
                                }

                            }

                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Resetting Status to CLOSE", null, null);

                            PostProcessStatus("C");
                        }
                    }
                    // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT"+ recP.Count, null, null);

                }
            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Error From  PROCESS", ex.Message, null);
                PostProcessStatus("A");
            }

            return "";
        }

        public string PostNapsXPEApi()
        {
            var token = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    token = GetAPIToken(clientId, clientSecret, destUri, client).Result;
                }
                // client.BaseAddress = new Uri(destUri);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                //var recP = GetNapsTranx(); // repoNaps.AllEagerLocal(d => d.BATCHID == _batchId && d.USERID == _userId).ToList();
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS STARTING  PROCESS ------------>>>", null, null);
                //var destUri = ConfigurationManager.AppSettings["naps_url"].ToString();
                //var appUser = ConfigurationManager.AppSettings["naps_appuser"].ToString();
                //var pass = ConfigurationManager.AppSettings["naps_password"].ToString();
                //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS [NAPS CONNECT] AFTER CONNECTION STRING", null, null);
                var stat = GetProcessStatus();
                if (stat == "P")
                {
                    return null;
                }
                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Setting Process to POSTING .....>", null, null);


                    var sqlQuery = "proc_GetNapsToPost";
                    var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                    var recGroup = rec.GroupBy(d => new { d.BATCHID, d.DEBITACCTNO }).ToList();
                    paymentRequestObj requestObj = new paymentRequestObj();


                    string batchId = "";
                    string drBankCode = "";
                    string drAcctNo = "";
                    if (recGroup.Count() > 0)
                    {

                        PostProcessStatus("P");
                        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS [ CONNECT] RECORD COUNT" + recGroup.Count, null, null);
                        foreach (var recP in recGroup)
                        {

                            int i = 0;
                            var scheduleId = SmartObj.GenRefNo();
                            foreach (var t in recP)
                            {
                                try
                                {
                                    if (i == 0)
                                    {

                                        var amt = recP.Sum(f => f.CREDITAMOUNT);
                                        requestObj = new paymentRequestObj()
                                        {
                                            accountName = "XP NAPS",
                                            accountNumber = t.DEBITACCTNO,
                                            amount = amt.Value,
                                            count = recP.Count(),
                                            currency = "NGN",
                                            narration = t.BENEFICIARYNARRATION,
                                            sortCode = t.DEBITBANKCODE,
                                            referenceNumber = scheduleId,

                                        };

                                        batchId = t.BATCHID;
                                        drBankCode = t.DEBITBANKCODE;
                                        drAcctNo = t.DEBITACCTNO;

                                        requestObj.payments = new List<Payment>();

                                    }

                                    var refNo = SmartObj.GenRefNo();

                                    requestObj.payments.Add(new Payment()
                                    {
                                        accountName = t.BENEFICIARYNAME,
                                        accountNumber = t.BENEFICIARYACCTNO,
                                        amount = t.CREDITAMOUNT.GetValueOrDefault(),
                                        narration = t.BENEFICIARYNARRATION,
                                        referenceNumber = refNo,
                                        sortCode = t.BENEFICIARYBANKCODE,

                                    });


                                    i++;
                                }
                                catch (Exception ex)
                                {
                                    // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
                                    //  PostNapsStatus(t.ITBID, "91", ex.Message, null, 1, conn);
                                    break;
                                }
                            }

                            //post group to NIBSS

                            try
                            {
                                var retObj = new OutPutObj();
                                // var result = response.Content.ReadAsStringAsync().Result;
                                try
                                {
                                    string json = JsonConvert.SerializeObject(requestObj);
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Request json  Data to NIBSS " + json.ToString(), null, null);

                                }
                                catch (Exception ex)
                                {

                                }
                                var result = PostTransaction(requestObj, clientId, clientSecret, destUri, token).Result;
                                if (result != null && result.data != null)
                                {
                                    //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "Response Result " + result.ToString(), null, null);

                                    //  retObj.RespCode2 = SmartObj.GetNapsMessage2(result.status);
                                    retObj.RespMessage = result.status;

                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Response Message " + retObj.RespMessage, null, null);

                                    string json = JsonConvert.SerializeObject(result);
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Response json  Data to from xpdirect " + json.ToString(), null, null);


                                    PostNapsStatus2(batchId, drBankCode, drAcctNo, null, result.data.payment.uniqueKey, retObj.RespCode2, retObj.RespMessage, scheduleId, 1, conn);
                                }
                                else
                                {
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS  NO DATA TO POST " + result.ToString(), null, null);

                                    retObj.RespCode2 = "92";
                                    retObj.RespMessage = "Posting to  Message cannot be interpreted.";
                                    PostNapsStatus2(batchId, drBankCode, drAcctNo, null, null, retObj.RespCode2, retObj.RespMessage, scheduleId, 1, conn);

                                    string json = JsonConvert.SerializeObject(result);
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Response json  Data to from xpdirect " + json.ToString(), null, null);

                                }
                            }
                            catch (Exception ex)
                            {
                                // PostNapsStatus(t.ITBID, "91", ex.Message, null, 1, conn);
                                PostNapsStatus2(batchId, drBankCode, drAcctNo, null, null, "91", ex.Message, scheduleId, 1, conn);

                            }

                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Resetting Status to CLOSE ", null, null);

                        }


                        PostProcessStatus("C");
                    }

                }


            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS Error From ", ex.Message, null);
                PostProcessStatus("A");
            }
            return "";
        }

        //public string TranxEnquiryApiNeft()
        //{
        //    try
        //    {
        //        using (var conn = new RepoBase().OpenConnection(sourceDb))
        //        {
        //            // PostProcessStatus("P");
        //            var sqlQuery = "proc_GetNapsForEnquiryNeft";
        //            var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        //            var recP = rec.ToList();

        //            if (recP.Count() > 0)
        //            {
        //                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NEFT GetNapsForEnquiryNeft PROCESSING .....>", null, null);

        //                using (HttpClient client = new HttpClient())
        //                {

        //                    var token = GetAPIToken(clientIdneft, clientSecretneft, destUri, client).Result;
        //                    // client.BaseAddress = new Uri(destUri);
        //                    //client.DefaultRequestHeaders.Accept.Clear();
        //                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //                    foreach (var t in recP)
        //                    {

        //                        var data = "";
        //                        var rst = "";


        //                        try
        //                        {
        //                            //HttpResponseMessage response = await client.GetAsync("/external/v1/payments?reference=" + t.SCHEDULEID);
        //                            var result = GetTransaction(t.UNIQUEKEY, clientIdneft, clientSecretneft, destUri, token).Result;
        //                            ////response.EnsureSuccessStatusCode();
        //                            //if (response.IsSuccessStatusCode)
        //                            //{
        //                            var retObj = new OutPutObj();
        //                            // var result = response.Content.ReadAsStringAsync().Result;
        //                            // var result = await response.Content.ReadAsAsync<paymentEnquiryObj>();
        //                            if (result != null && result.status == "SUCCESS")
        //                            {
        //                                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS  Get Status is Successful ---------->>>", null, null);

        //                                //////if (result.data.payments != null)
        //                                //////{

        //                                //////    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NAPS  Payment Records Available ---------->>>", null, null);

        //                                //////    retObj.RespCode2 = SmartObj.GetNapsMessage2(result.data.payments.content[0].status);
        //                                //////    retObj.RespMessage = result.data.payments.content[0].statusMessage;

        //                                //////    PostNapsStatus(t.ITBID, retObj.RespCode2, retObj.RespMessage, t.SCHEDULEID, 2, conn);
        //                                //////}
        //                                ///
        //                                if (result.data.paymentItems.content != null)
        //                                {
        //                                    var nn = result.data.paymentItems.content.ToList();
        //                                    foreach (var m in nn)
        //                                    {
        //                                            PostNapsStatus(m.beneficiaryAccountNumber, m.amount, m.narration, m.uniqueKey, m.status, m.statusMessage, t.SCHEDULEID, conn);

        //                                    }

        //                                }
        //                            }

        //                            // }
        //                        }
        //                        catch
        //                        {
        //                            continue;
        //                        }

        //                    }

        //                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NEFT Resetting Status to CLOSE", null, null);

        //                    PostProcessStatus("C");
        //                }
        //            }
        //            // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT"+ recP.Count, null, null);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Error From NEFT PROCESS", ex.Message, null);
        //        PostProcessStatus("A");
        //    }

        //    return "";
        //}

        public string PostNapsXPEApiNeft()
        {
            var token = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    token = GetAPIToken(clientIdneft, clientSecretneft, destUri, client).Result;
                }
                // client.BaseAddress = new Uri(destUri);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                //var recP = GetNapsTranx(); // repoNaps.AllEagerLocal(d => d.BATCHID == _batchId && d.USERID == _userId).ToList();
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " STARTING NEFT PROCESS ------------>>>", null, null);
                //var destUri = ConfigurationManager.AppSettings["naps_url"].ToString();
                //var appUser = ConfigurationManager.AppSettings["naps_appuser"].ToString();
                //var pass = ConfigurationManager.AppSettings["naps_password"].ToString();
                //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] AFTER CONNECTION STRING", null, null);
                var stat = GetProcessStatus();
                if (stat == "P")
                {
                    return null;
                }
                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NEFT Setting Process to POSTING .....>", null, null);


                    var sqlQuery = "proc_GetNapsToPostneft";
                    var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                    var recGroup = rec.GroupBy(d => new { d.BATCHID}).ToList();
                    paymentRequestObj requestObj = new paymentRequestObj();


                    string batchId = "";
                    string drBankCode = "";
                    string drAcctNo = "";
                    if (recGroup.Count() > 0)
                    {

                        PostProcessStatus("P");
                        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NEFT CONNECT] RECORD COUNT" + recGroup.Count, null, null);
                        foreach (var recP in recGroup)
                        {

                            int i = 0;
                            var scheduleId = SmartObj.GenRefNo();
                            foreach (var t in recP)
                            {
                                try
                                {
                                    if (i == 0)
                                    {

                                        var amt = recP.Sum(f => f.CREDITAMOUNT);
                                        requestObj = new paymentRequestObj()
                                        {
                                            accountName = "XP NEFT",
                                            accountNumber = t.DEBITACCTNO,
                                            amount = amt.Value,
                                            count = recP.Count(),
                                            currency = "NGN",
                                            narration = t.BENEFICIARYNARRATION,
                                            sortCode = t.DEBITBANKCODE,
                                            referenceNumber = scheduleId,

                                        };

                                        batchId = t.BATCHID;
                                        drBankCode = t.DEBITBANKCODE;
                                        drAcctNo = t.DEBITACCTNO;

                                        requestObj.payments = new List<Payment>();

                                    }

                                    var refNo = SmartObj.GenRefNo();

                                    requestObj.payments.Add(new Payment()
                                    {
                                        accountName = t.BENEFICIARYNAME,
                                        accountNumber = t.BENEFICIARYACCTNO,
                                        amount = t.CREDITAMOUNT.GetValueOrDefault(),
                                        narration = t.BENEFICIARYNARRATION,
                                        referenceNumber = refNo,
                                        sortCode = t.BENEFICIARYBANKCODE,

                                    });


                                    i++;
                                }
                                catch (Exception ex)
                                {
                                    // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
                                    //  PostNapsStatus(t.ITBID, "91", ex.Message, null, 1, conn);
                                    break;
                                }
                            }

                            //post group to NIBSS

                            try
                            {
                                var retObj = new OutPutObj();
                                // var result = response.Content.ReadAsStringAsync().Result;
                                try
                                {
                                    string json = JsonConvert.SerializeObject(requestObj);
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Request json NEFT Data to NIBSS " + json.ToString(), null, null);

                                }
                                catch (Exception ex)
                                {

                                }
                                var result = PostTransaction(requestObj, clientId, clientSecret, destUri, token).Result;
                                if (result != null && result.data != null)
                                {

                                    // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NEFT POSTING TRANSACTION ", null, null);

                                    // retObj.RespCode2 = SmartObj.GetNapsMessage2(result.data.payment.status);
                                    retObj.RespMessage = result.status;

                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Response Message " + retObj.RespMessage, null, null);

                                    PostNapsStatus2(batchId, drBankCode, drAcctNo, null, result.data.payment.uniqueKey, retObj.RespCode2, retObj.RespMessage, scheduleId, 1, conn);
                                }
                                else
                                {
                                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " NEFT NO DATA TO POST " + result.ToString(), null, null);

                                    retObj.RespCode2 = "92";
                                    retObj.RespMessage = "Posting to Neft Message cannot be interpreted.";
                                    PostNapsStatus2(batchId, drBankCode, drAcctNo, null, null, retObj.RespCode2, retObj.RespMessage, scheduleId, 1, conn);
                                }
                            }
                            catch (Exception ex)
                            {
                                // PostNapsStatus(t.ITBID, "91", ex.Message, null, 1, conn);
                                PostNapsStatus2(batchId, drBankCode, drAcctNo, null, null, "91", ex.Message, scheduleId, 1, conn);

                            }

                            lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "NEFT Resetting Status to CLOSE ", null, null);

                        }


                        PostProcessStatus("C");
                    }

                }


            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " Error From NEFT", ex.Message, null);
                PostProcessStatus("A");
            }
            return "";
        }

        //public void TranxEnquiry()
        //{
        //    try
        //    {
        //        //var recP = GetNapsTranx(); // repoNaps.AllEagerLocal(d => d.BATCHID == _batchId && d.USERID == _userId).ToList();
        //        //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] POST PROCESS STARTS NOW", null, null);

        //        //lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] AFTER CONNECTION STRING", null, null);
        //        //var stat = GetProcessStatus();
        //        //if (stat == "P")
        //        //{
        //        //    return;
        //        //}
        //        using (var conn = new RepoBase().OpenConnection(sourceDb))
        //        {
        //           // PostProcessStatus("P");
        //            var sqlQuery = "proc_GetNapsForEnquiry";
        //            var rec = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        //            var recP = rec.ToList();
        //            // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT"+ recP.Count, null, null);
        //            foreach (var t in recP)
        //            {
        //                try
        //                {
        //                    var data = "";
        //                    var rst = "";
        //                   // var scheduleId = SmartObj.GenRefNo();
        //                    var auth = new JsonObjectStatus()
        //                    {
        //                        AppUser = appUser,
        //                        Password = pass,
        //                        ScheduleId = t.SCHEDULEID,
        //                    };
        //                    var authKey = JsonConvert.SerializeObject(auth);
        //                    using (WebClient postClient = new WebClient())
        //                    {
        //                        postClient.Proxy = null;
        //                        //postClient.Headers.Add(HttpRequestHeader.ContentType, "application/soap+xml; charset=utf-8");
        //                        postClient.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
        //                        //postClient.Headers.Add("SOAPAction", "http://tempuri.org");

        //                        StreamReader reader = new StreamReader("C:\\NapsTemplate\\TranxEnquiry.xml");

        //                        data = string.Format(reader.ReadToEnd(), authKey);
        //                        reader.Close();

        //                        //  postClient.UploadStringCompleted += new UploadStringCompletedEventHandler(postClient_UploadStringCompleted);
        //                        try
        //                        {
        //                            rst = postClient.UploadString(new Uri(destUri), data);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
        //                            //t.RESPCODE = "98";
        //                            //t.RESPMESSAGE = ex.Message;
        //                            // t.SCHEDULEID = scheduleId;
        //                           // PostNapsStatus(t.ITBID, "91", ex.Message, null, conn);
        //                            continue;
        //                        }
        //                        //ServicePointManager.Expect100Continue = true;
        //                        //.ServerCertificateValidationCallback = MyCertHandler;
        //                    }
        //                    var ret = postClient_UploadStringCompleted2(rst);
        //                    if (ret != null)
        //                    {
        //                        //t.RESPCODE = ret.RespCode2;
        //                        //t.RESPMESSAGE = ret.RespMessage;
        //                        // t.SCHEDULEID = scheduleId;
        //                        //uow.Save(t.USERID);
        //                        PostNapsStatus(t.ITBID, ret.RespCode2, ret.RespMessage, t.SCHEDULEID,2, conn);
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    // return new OutPutObj() { RespCode2 = "98", RespMessage = ex.Message };
        //                    PostNapsStatus(t.ITBID, "91", ex.Message, null,2, conn);
        //                    continue;
        //                }
        //            }

        //            PostProcessStatus("C");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] POST PROCESS STARTS NOW", ex.Message, null);
        //        PostProcessStatus("A");
        //    }
        //}
        //public string TranxEnquiryApi2()
        //{
        //    try
        //    {
        //        using (var conn = new RepoBase().OpenConnection(sourceDb))
        //        {
        //            // PostProcessStatus("P");
        //            var sqlQuery = "proc_GetNapsForEnquiry2";
        //            var recP = conn.Query<NapsObj>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
        //           // var recP = rec.ToList();
        //            // lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] RECORD COUNT"+ recP.Count, null, null);
        //            using (HttpClient client = new HttpClient())
        //            {
        //                var token = GetAPIToken(clientId, clientSecret, destUri, client).Result;
        //                // client.BaseAddress = new Uri(destUri);
        //                //client.DefaultRequestHeaders.Accept.Clear();
        //                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        //                foreach (var t in recP)
        //                {

        //                    var data = "";
        //                    var rst = "";


        //                    try
        //                    {
        //                        //HttpResponseMessage response = await client.GetAsync("/external/v1/payments?reference=" + t.SCHEDULEID);
        //                        var result = GetTransaction(t.UNIQUEKEY, clientId, clientSecret, destUri, token).Result;
        //                        ////response.EnsureSuccessStatusCode();
        //                        //if (response.IsSuccessStatusCode)
        //                        //{
        //                        var retObj = new OutPutObj();
        //                        // var result = response.Content.ReadAsStringAsync().Result;
        //                        // var result = await response.Content.ReadAsAsync<paymentEnquiryObj>();
        //                        if (result != null && result.status == "SUCCESS")
        //                        {
        //                            if (result.data.payments != null)
        //                            {
        //                                for (var p = 0; p < result.data.payments.content.Count; p++)
        //                                {
        //                                    retObj.RespCode2 = SmartObj.GetNapsMessage2(result.data.payments.content[p].status);
        //                                    retObj.RespMessage = result.data.payments.content[p].statusMessage;
        //                                    PostNapsStatus2(t.BATCHID,null, t.DEBITACCTNO, result.data.payments.content[p].accountNumber, t.UNIQUEKEY, retObj.RespCode2, retObj.RespMessage, null, 3, conn);
        //                                }
        //                            }
        //                        }

        //                        // }
        //                    }
        //                    catch
        //                    {
        //                        continue;
        //                    }

        //                }
        //                PostProcessStatus("C");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [NAPS CONNECT] POST PROCESS STARTS NOW", ex.Message, null);
        //        PostProcessStatus("A");
        //    }

        //    return "";
        //}
        OutPutObj postClient_UploadStringCompleted(string result)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result);
            var resp = xdoc.InnerText;
            //foreach (XmlNode node in xdoc.GetElementsByTagName("UploadPayment"))
            //{
            //    foreach (XmlNode node1 in node.ChildNodes)
            //    {
            //        switch (node1.Name)
            //        {
            //            case "RespCode":
            //               // rst = node1.InnerText;
            //                break;
            //            case "RespMessage":
            //                respMsg = node1.InnerText;
            //                break;
            //        }
            //    }
            //}
            var retObj = new OutPutObj();
            var objreq = JsonConvert.DeserializeObject<PayResponse>(resp);
            if (objreq != null && objreq.PaymentResponse != null)
            {
                var rst = objreq.PaymentResponse;
                if (rst.Header != null)
                {
                    retObj.RespCode2 = rst.Header.Status;
                    retObj.RespMessage = SmartObj.GetNapsMessage(retObj.RespCode2);
                    return retObj;
                }
            }
            retObj.RespCode2 = "92";
            retObj.RespMessage = "Posting to Nibss Message cannot be interpreted.";
            return retObj;
        }
        OutPutObj postClient_UploadStringCompleted2(string result)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result);
            var resp = xdoc.InnerText;
            //foreach (XmlNode node in xdoc.GetElementsByTagName("CheckPaymentStatus"))
            //{
            //    foreach (XmlNode node1 in node.ChildNodes)
            //    {
            //        switch (node1.Name)
            //        {
            //            case "RespCode":
            //                // rst = node1.InnerText;
            //                break;
            //            case "RespMessage":
            //                //respMsg = node1.InnerText;
            //                break;
            //        }
            //    }
            //}
            var retObj = new OutPutObj();
            var objreq = JsonConvert.DeserializeObject<PayResponse>(resp);
            //PayResponse
            if (objreq != null && objreq.PaymentResponse != null)
            {
                var rst = objreq.PaymentResponse;
                if (rst.Header != null)
                {
                    retObj.RespCode2 = rst.Header.Status;
                    retObj.RespMessage = SmartObj.GetNapsMessage(retObj.RespCode2);
                    return retObj;
                }
            }
            retObj.RespCode2 = "92";
            retObj.RespMessage = "Posting to Nibss Message cannot be interpreted.";
            return retObj;
        }
        public int PostNapsStatus(string accountNumber, decimal amount, string narration, string uniquekey, string respCode, string respMsg, string SCHEDULEID, DbConnection conn)
        {
            try
            {

                var p = new DynamicParameters();
                p.Add("@accountNumber", accountNumber, DbType.String);
                p.Add("@amount", amount, DbType.Decimal);
                p.Add("@narration", narration, DbType.String);
                p.Add("@RespCode", respCode, DbType.String);
                p.Add("@RespMessage", respMsg, DbType.String);
                p.Add("@uniquekey", uniquekey, DbType.String);
                p.Add("@ScheduleId", SCHEDULEID, DbType.String);
                var sqlQuery = "proc_UpdateNapSingle";
                var rec = conn.Execute(sqlQuery, p, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                return rec;

            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [POSTING TO NAPS] ===", ex.Message, null);

            }
            return 0;
        }
        public int PostNapsStatus2(string batchId, string drBankCode, string drBankAcct, string CrBankAcct, string uniqueKey, string respCode, string respMsg, string scheduleId, int reqType, DbConnection conn)
        {
            try
            {

                var p = new DynamicParameters();
                p.Add("@BatchId", batchId, DbType.String);
                p.Add("@DrBankCode", drBankCode, DbType.String);
                p.Add("@DrAcctNo", drBankAcct, DbType.String);
                p.Add("@CrAcctNo", CrBankAcct, DbType.String);
                p.Add("@UniqueKey", uniqueKey, DbType.String);
                p.Add("@RespCode", respCode, DbType.String);
                p.Add("@RespMessage", respMsg, DbType.String);
                p.Add("@ScheduleId", scheduleId, DbType.String);
                p.Add("@ReqType", reqType, DbType.Int32);
                var sqlQuery = "proc_UpdateNapsBatch";
                var rec = conn.Execute(sqlQuery, p, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                return rec;

            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [POSTING UPDATE STATUS] ===", ex.Message, null);

            }
            return 0;
        }
        public int PostProcessStatus(string status)
        {
            try
            {
                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    var p = new DynamicParameters();
                    p.Add("@Status", status, DbType.String);
                    var sqlQuery = "UPDATE SM_COMPANY_PROFILE SET PROCESS_FLAG_NAPS = @Status";
                    var rec = conn.Execute(sqlQuery, p, commandTimeout: 0, commandType: CommandType.Text);
                    return rec;
                }

            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [POSTING UPDATE STATUS] ===", ex.Message, null);

            }
            return 0;
        }
        public string GetProcessStatus()
        {
            try
            {
                using (var conn = new RepoBase().OpenConnection(sourceDb))
                {
                    var sqlQuery = "SELECT PROCESS_FLAG_NAPS FROM SM_COMPANY_PROFILE";
                    var rec = conn.Query<string>(sqlQuery, null, commandTimeout: 0, commandType: CommandType.Text);
                    return rec.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + " [POSTING UPDATE STATUS] ===", ex.Message, null);

            }
            return "";
        }
        private async Task<string> GetAPIToken(string userName, string password, string apiBaseUri, HttpClient client)
        {
            //using (var client = new HttpClient())
            //{
            //setup client
            try
            {
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup login data
                //var formContent = new FormUrlEncodedContent(new[]
                //{
                //    // new KeyValuePair<string, string>("grant_type", "password"),
                //     new KeyValuePair<string, string>("clientId", userName),
                //     new KeyValuePair<string, string>("clientSecret", password),
                //     });

                //send request
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("/external/v1/authentication", new { clientId = userName, clientSecret = password });

                //get access token from response body
                responseMessage.EnsureSuccessStatusCode();
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseJson = await responseMessage.Content.ReadAsAsync<tokenResponsetObj>();
                    //var jObject = JObject.(responseJson);
                    if (responseJson.status == "SUCCESS")
                    {
                        return responseJson.data.token.token; // jObject.GetValue("access_token").ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "Error From Gettting API Token", ex.Message, null);

            }

            // 
            return null;
            // }
        }
        private async Task<paymentResponseObj> PostTransaction(paymentRequestObj request, string userName, string password, string apiBaseUri, string token)
        {
            using (var client = new HttpClient())
            {
                //setup client
                try
                {
                    client.BaseAddress = new Uri(apiBaseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);



                    //send request
                    //  HttpResponseMessage responseMessage = await client.PostAsJsonAsync("/external/v1/authentication", request);

                    //get access token from response body
                    HttpResponseMessage response = await client.PostAsJsonAsync("/external/v1/payments", request);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var retObj = new OutPutObj();
                        // var result = response.Content.ReadAsStringAsync().Result;
                        var result = await response.Content.ReadAsAsync<paymentResponseObj>();

                        return result;

                    }
                    return null;
                }
                catch (Exception ex)
                {
                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "Error From Posting Transaction", ex.Message, null);

                }
                return null;
            }
        }
        private async Task<paymentEnquiryObj2> GetTransaction(string unique, string userName, string password, string apiBaseUri, string token)
        {
            using (var client = new HttpClient())
            {
                //setup client
                try
                {
                    client.BaseAddress = new Uri(apiBaseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);



                    //send request
                    //  HttpResponseMessage responseMessage = await client.PostAsJsonAsync("/external/v1/authentication", request);

                    //get access token from response body
                    HttpResponseMessage response = await client.GetAsync("/external/v1/payments/items?pageSize=200&payment=" + unique);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var retObj = new OutPutObj();
                        var result1 = response.Content.ReadAsStringAsync().Result;
                        var result = await response.Content.ReadAsAsync<paymentEnquiryObj2>();

                        return result;

                    }
                    return null;
                }
                catch (Exception ex)
                {
                    lgfn.logNapsinfoMSG(DateTime.Now.ToString() + "Error From Getting Transaction", ex.Message, null);

                }
                return null;
            }
        }
    }
}
