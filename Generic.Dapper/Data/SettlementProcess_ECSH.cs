using System;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using Generic.Dapper.Repository;
using Dapper;
using Generic.Dapper.Model;
using System.Collections.Generic;
using System.Linq;
using Generic.Data;
//using Oracle.DataAccess.Client;

namespace Generic.Dapper.Data
{
    public class SettlementProcess_ECSH
    {
        //static string res = "ORA-00001: unique constraint";
        static string res2 = "ORA-00001: unique constraint (DPPADMIN.LOGDATA_PK) violated";
        //static DateTime? Maxdate = null;
        // private static string oradb = System.Configuration.ConfigurationManager.AppSettings["POSMISDB"].ToString();
        private static string destDb = System.Configuration.ConfigurationManager.AppSettings["DEST_DB"].ToString();
        private static string enforceFutureSetDate = System.Configuration.ConfigurationManager.AppSettings["EnforceForwardSettlementDate"].ToString();

        //DbProviderFactory factory = DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Client");
        //  private static string purgedays = System.Configuration.ConfigurationManager.AppSettings["purgedays"].ToString();
        //OracleConnection Standby_connection;
        LogFunction lgfn = new LogFunction();
        public void SettProcess(string batchNo, string opdate)
        {
            Stopwatch se = new Stopwatch();
            se.Start();
            TimeSpan ts = se.Elapsed;
            string eT = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            // lgfn.loginfoMSG("Fecthing data for settlement process Started", " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO SETTLEMENT PROCESS LOOPING", null, null);

            ////string SqlString = @"UPDATE POSMISDB_MEB SET ISSCOUNTRY = '566' WHERE SUBSTR(PAN, 1,6) IN (SELECT DISTINCT BIN FROM POSMISDB_TRAWDATABIN WHERE COUNTRYCODE = '566') AND ISSCOUNTRY <> '566' AND ACQCOUNTRY = '566'";

            DateTime tranDate = DateTime.Now;
            var ID = ""; //.GetOracleString(0);
            var DOCNO = "";
            decimal recCount = 0;
            decimal procCount = 0;
            var cnt = 0;
            string errorMsg = "";
            GetMSCObj objMsc;
            //OracleConnection sett_connection = default(OracleConnection);
            //OracleCommand oracommand = default(OracleCommand);
            //OracleCommand updatemeb = default(OracleCommand);
            using (var con = new RepoBase().OpenConnection(destDb))
            {

                DateTime SETTLEMENTDATE, CPD;
                SETTLEMENTDATE = CPD = DateTime.Today;


                lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INFO COMPLETING PRE-PROCESS", null, null);

                DateTime? Opdate2 = null;
                var sqlPostQuery = @"INSERT INTO SM_SETTLEMENTDETAIL_MEB([ID],[DOCNO],[REFNO],[CHANNELID],[CARDSCHEME]
           ,[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY],[ACQUIRERFIID],[ACQUIRERCURRENCY]
           ,[MASKEDPAN],[PAN],[EXPDATETIME],[SETTLEMENTDATETIME],[APPROVALCODE],[APPROVALMESSAGE]
           ,[TERMINALID],[MERCHANTID],[MERCHANTLOCATION],[MERCHANTDEPOSITBANK],[MERCHANTDEPOSITBANKCODE],[MERCHANTDEPOSITBANKACCTNO]
           ,[MCC],[MCC_DESCRIPTION],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT],[ORGINALCURRENCY],[TRANAMOUNT],[TRANCURRENCY],[SIGN]
           ,[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[ACQUIRER_REF],[SETTLEMENTCURRENCY]
           ,[MSC1_RATE],[MSC1_CAP],[MSC2_RATE],[MSC2_CAP],[SHARINGMSC1_RATE],[UNSHAREDMSC1_RATE],[SUBSIDY_RATE]
           ,[MSC1_AMOUNT],[MSC2_AMOUNT],[TOTALMSC_AMOUNT],[EFFECTIVEMSC],[SHARINGMSC1_AMOUNT],[UNSHAREDMSC1_AMOUNT]
           ,[MSC2_SPLITSHARING],[MSC2_SPLITSHARINGACCT],[PERC_ALLOTOTHERS],[PERC_ALLOTHOLDER],[SUBSIDY_AMOUNT],[PERC_ALLOTMERCHANT]
           ,[TOTAL_AMOUNTDUE],[DEDUCTION_AMOUNTDUE],[NET_AMOUNTDUEMERCHANT],[IRF],[SPECIALCARD],[SPECIALCARD_OWNER],[REASONFORALTERATION],[FULLREM_TRANSLATION]
           ,[TRACENO],[HOST],[ORIGID],[EXTRRN],[ORIGTYPE],[INVOICENUM],[SEQNUM],[LCYAMOUNT],[FCYAMOUNT]
           ,[EXCHANGERATE],[SETTLEMENTACCOUNT],[SETTLEMENTAMOUNT],[REVERSEREQID],[SECTORCODE],[SECTOR]
           ,[TRANXSAVED],[CARDCOUNTRY],[PROCESSINGFEE],[CUSTOMERACCOUNT],[PHONENO],[ACQR_CODE],[ACQR_NAME]
           ,[ACQR_RATE],[ACQR_VALUE],[ACQR_ACCOUNT],[ISSR_CODE],[ISSR_NAME],[ISSR_RATE],[ISSR_VALUE],[ISSR_ACCOUNT]
           ,[PTSP_CODE],[PTSP_NAME],[PTSP_RATE],[PTSP_VALUE],[PTSP_ACCOUNT],[PTSA_CODE],[PTSA_NAME],[PTSA_RATE],[PTSA_VALUE]
           ,[PTSA_ACCOUNT],[TERW_CODE],[TERW_NAME],[TERW_RATE],[TERW_VALUE],[TERW_ACCOUNT],[SWTH_CODE],[SWTH_NAME],[SWTH_RATE],[SWTH_VALUE]
           ,[SWTH_ACCOUNT],[BILR_CODE],[BILR_NAME],[BILR_RATE],[BILR_VALUE],[BILR_ACCOUNT],[TECH_CODE],[TECH_NAME],[TECH_RATE]
           ,[TECH_VALUE],[TECH_ACCOUNT],[COAQ_CODE],[COAQ_NAME],[COAQ_RATE],[COAQ_VALUE],[COAQ_ACCOUNT],[REVW_CODE]
           ,[REVW_NAME],[REVW_RATE],[REVW_VALUE],[REVW_ACCOUNT],[GOVT_CODE],[GOVT_NAME],[GOVT_RATE],[GOVT_VALUE]
           ,[GOVT_ACCOUNT],[AGNT_CODE],[AGNT_NAME],[AGNT_RATE],[AGNT_VALUE],[AGNT_ACCOUNT],[VEND_CODE],[VEND_NAME],[VEND_RATE]
           ,[VEND_VALUE],[VEND_ACCOUNT],[BLLR_CODE],[BLLR_NAME],[BLLR_RATE],[BLLR_VALUE],[BLLR_ACCOUNT],[SERVICEPROVIDER]
           ,[SERVICEPROVIDERBANKCODE],[SERVICEPROVIDERBANK],[SERVICEPROVIDERACCOUNTNO],[BENEFICIARYNAME],[BENEFICIARYBANKCODE],[BENEFICIARYBANK],[BENEFICIARYACCOUNTNO]
           ,[INTERCHANGEFEE],[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4],[SOURCEDB],[SOURCETABLE]
           ,[REMARK],[RECORDID],[BATCHNO],[PROCESS_STATUS],[CREATEDATETIME],[CUSTOMERID],[MERCHANTNAME],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID],[DR_ACCTNO]
           ,[VALUEDATE],[MSC2_MARGIN],[VALUEDATE2],[DR_BANKCODE]
           ,[REVENUECODE],[MSC2PARTY1_BANKCODE],[MSC2PARTY1_NAME],[MSC2PARTY1_RATE],[MSC2PARTY1_VALUE],[MSC2PARTY1_ACCOUNT],MSC2PARTY1_ACCOUNTNAME,[MSC2PARTY2_BANKCODE]
           ,[MSC2PARTY2_NAME],[MSC2PARTY2_RATE],[MSC2PARTY2_VALUE],[MSC2PARTY2_ACCOUNT],MSC2PARTY2_ACCOUNTNAME,[MSC2PARTY3_BANKCODE],[MSC2PARTY3_NAME],[MSC2PARTY3_RATE],[MSC2PARTY3_VALUE]
           ,[MSC2PARTY3_ACCOUNT],MSC2PARTY3_ACCOUNTNAME,[MSC2PARTY4_BANKCODE],[MSC2PARTY4_NAME],[MSC2PARTY4_RATE],[MSC2PARTY4_VALUE],[MSC2PARTY4_ACCOUNT],MSC2PARTY4_ACCOUNTNAME
           ,[MSC2PARTY5_BANKCODE],[MSC2PARTY5_NAME],[MSC2PARTY5_RATE],[MSC2PARTY5_VALUE],[MSC2PARTY5_ACCOUNT],MSC2PARTY5_ACCOUNTNAME,[MSC2PARTY6_BANKCODE],[MSC2PARTY6_NAME],[MSC2PARTY6_RATE]
           ,[MSC2PARTY6_VALUE],[MSC2PARTY6_ACCOUNT],MSC2PARTY6_ACCOUNTNAME,[MSC2PARTY7_BANKCODE],[MSC2PARTY7_NAME],[MSC2PARTY7_RATE],[MSC2PARTY7_VALUE],[MSC2PARTY7_ACCOUNT],MSC2PARTY7_ACCOUNTNAME
           ,[MSC2PARTY8_NAME],[MSC2PARTY8_RATE],[MSC2PARTY8_VALUE],[MSC2PARTY8_ACCOUNT],MSC2PARTY8_ACCOUNTNAME,[MSC2PARTY9_BANKCODE],[MSC2PARTY9_NAME],[MSC2PARTY9_RATE]
           ,[MSC2PARTY9_VALUE],[MSC2PARTY9_ACCOUNT],MSC2PARTY9_ACCOUNTNAME,[MSC2PARTY10_BANKCODE],[MSC2PARTY10_NAME],[MSC2PARTY10_RATE],[MSC2PARTY10_VALUE],[MSC2PARTY10_ACCOUNT],MSC2PARTY10_ACCOUNTNAME
           ,[BRANCHID],[SETTLEMENT_FREQUENCY],[PAYMENTITEMID])
            VALUES
            (@ID,@DOCNO,@REFNO,@CHANNELID,@CARDSCHEME,@TRANCODE,@TRANDESC,@TRANDATETIME,@ISSUERFIID,@ISSUERCURRENCY,@ACQUIRERFIID,@ACQUIRERCURRENCY
                ,@MASKEDPAN,@PAN,@EXPDATETIME,@SETTLEMENTDATETIME,@APPROVALCODE,@APPROVALMESSAGE
                ,@TERMINALID,@MERCHANTID,@MERCHANTLOCATION,@MERCHANTDEPOSITBANK,@MERCHANTDEPOSITBANKCODE,@MERCHANTDEPOSITBANKACCTNO
                ,@MCC,@MCC_DESCRIPTION,@STAN,@INVOICENO,@CUSTOMERNAME,@ORGINALAMOUNT,@ORGINALCURRENCY,@TRANAMOUNT,@TRANCURRENCY,@SIGN
                ,@ISSUERFEE,@ISSUERFEECURRENCY,@ACQUIRERFEE,@ACQUIRERFEECURRENCY,@ACQUIRER_REF,@SETTLEMENTCURRENCY
                ,@MSC1_RATE,@MSC1_CAP,@MSC2_RATE,@MSC2_CAP,@SHARINGMSC1_RATE,@UNSHAREDMSC1_RATE,@SUBSIDY_RATE
                ,@MSC1_AMOUNT,@MSC2_AMOUNT,@TOTALMSC_AMOUNT,@EFFECTIVEMSC,@SHARINGMSC1_AMOUNT,@UNSHAREDMSC1_AMOUNT
                ,@MSC2_SPLITSHARING,@MSC2_SPLITSHARINGACCT,@PERC_ALLOTOTHERS,@PERC_ALLOTHOLDER,@SUBSIDY_AMOUNT,@PERC_ALLOTMERCHANT
                ,@TOTAL_AMOUNTDUE,@DEDUCTION_AMOUNTDUE,@NET_AMOUNTDUEMERCHANT,@IRF,@SPECIALCARD,@SPECIALCARD_OWNER,@REASONFORALTERATION,@FULLREM_TRANSLATION
                ,@TRACENO,@HOST,@ORIGID,@EXTRRN,@ORIGTYPE,@INVOICENUM,@SEQNUM,@LCYAMOUNT,@FCYAMOUNT
                ,@EXCHANGERATE,@SETTLEMENTACCOUNT,@SETTLEMENTAMOUNT,@REVERSEREQID,@SECTORCODE,@SECTOR
                ,@TRANXSAVED,@CARDCOUNTRY,@PROCESSINGFEE,@CUSTOMERACCOUNT,@PHONENO,@ACQR_CODE,@ACQR_NAME
                ,@ACQR_RATE,@ACQR_VALUE,@ACQR_ACCOUNT,@ISSR_CODE,@ISSR_NAME,@ISSR_RATE,@ISSR_VALUE,@ISSR_ACCOUNT
                ,@PTSP_CODE,@PTSP_NAME,@PTSP_RATE,@PTSP_VALUE,@PTSP_ACCOUNT,@PTSA_CODE,@PTSA_NAME,@PTSA_RATE,@PTSA_VALUE
                ,@PTSA_ACCOUNT,@TERW_CODE,@TERW_NAME,@TERW_RATE,@TERW_VALUE,@TERW_ACCOUNT,@SWTH_CODE,@SWTH_NAME,@SWTH_RATE,@SWTH_VALUE
                ,@SWTH_ACCOUNT,@BILR_CODE,@BILR_NAME,@BILR_RATE,@BILR_VALUE,@BILR_ACCOUNT,@TECH_CODE,@TECH_NAME,@TECH_RATE
                ,@TECH_VALUE,@TECH_ACCOUNT,@COAQ_CODE,@COAQ_NAME,@COAQ_RATE,@COAQ_VALUE,@COAQ_ACCOUNT,@REVW_CODE
                ,@REVW_NAME,@REVW_RATE,@REVW_VALUE,@REVW_ACCOUNT,@GOVT_CODE,@GOVT_NAME,@GOVT_RATE,@GOVT_VALUE
                ,@GOVT_ACCOUNT,@AGNT_CODE,@AGNT_NAME,@AGNT_RATE,@AGNT_VALUE,@AGNT_ACCOUNT,@VEND_CODE,@VEND_NAME,@VEND_RATE
                ,@VEND_VALUE,@VEND_ACCOUNT,@BLLR_CODE,@BLLR_NAME,@BLLR_RATE,@BLLR_VALUE,@BLLR_ACCOUNT,@SERVICEPROVIDER
                ,@SERVICEPROVIDERBANKCODE,@SERVICEPROVIDERBANK,@SERVICEPROVIDERACCOUNTNO,@BENEFICIARYNAME,@BENEFICIARYBANKCODE,@BENEFICIARYBANK,@BENEFICIARYACCOUNTNO
                ,@INTERCHANGEFEE,@INTERCHANGEFEECUR,@SPECIALMESSAGE1,@SPECIALMESSAGE2,@SPECIALMESSAGE3,@SPECIALMESSAGE4,@SOURCEDB,@SOURCETABLE
                ,@REMARK,@RECORDID,@BATCHNO,@PROCESS_STATUS,@CREATEDATETIME,@CUSTOMERID,@MERCHANTNAME,@AGENT_CODE,@PAYMENTREFERENCE,@TRANSID,@DR_ACCTNO
                ,@VALUEDATE,@MSC2_MARGIN,@VALUEDATE2,@DR_BANKCODE
                ,@REVENUECODE,@MSC2PARTY1_BANKCODE,@MSC2PARTY1_NAME,@MSC2PARTY1_RATE,@MSC2PARTY1_VALUE,@MSC2PARTY1_ACCOUNT,@MSC2PARTY1_ACCOUNTNAME,@MSC2PARTY2_BANKCODE
                ,@MSC2PARTY2_NAME,@MSC2PARTY2_RATE,@MSC2PARTY2_VALUE,@MSC2PARTY2_ACCOUNT,@MSC2PARTY2_ACCOUNTNAME,@MSC2PARTY3_BANKCODE,@MSC2PARTY3_NAME,@MSC2PARTY3_RATE,@MSC2PARTY3_VALUE
                ,@MSC2PARTY3_ACCOUNT,@MSC2PARTY3_ACCOUNTNAME,@MSC2PARTY4_BANKCODE,@MSC2PARTY4_NAME,@MSC2PARTY4_RATE,@MSC2PARTY4_VALUE,@MSC2PARTY4_ACCOUNT,@MSC2PARTY4_ACCOUNTNAME
                ,@MSC2PARTY5_BANKCODE,@MSC2PARTY5_NAME,@MSC2PARTY5_RATE,@MSC2PARTY5_VALUE,@MSC2PARTY5_ACCOUNT,@MSC2PARTY5_ACCOUNTNAME,@MSC2PARTY6_BANKCODE,@MSC2PARTY6_NAME,@MSC2PARTY6_RATE
                ,@MSC2PARTY6_VALUE,@MSC2PARTY6_ACCOUNT,@MSC2PARTY6_ACCOUNTNAME,@MSC2PARTY7_BANKCODE,@MSC2PARTY7_NAME,@MSC2PARTY7_RATE,@MSC2PARTY7_VALUE,@MSC2PARTY7_ACCOUNT,@MSC2PARTY7_ACCOUNTNAME
                ,@MSC2PARTY8_NAME,@MSC2PARTY8_RATE,@MSC2PARTY8_VALUE,@MSC2PARTY8_ACCOUNT,@MSC2PARTY8_ACCOUNTNAME,@MSC2PARTY9_BANKCODE,@MSC2PARTY9_NAME,@MSC2PARTY9_RATE
                ,@MSC2PARTY9_VALUE,@MSC2PARTY9_ACCOUNT,@MSC2PARTY9_ACCOUNTNAME,@MSC2PARTY10_BANKCODE,@MSC2PARTY10_NAME,@MSC2PARTY10_RATE,@MSC2PARTY10_VALUE,@MSC2PARTY10_ACCOUNT,@MSC2PARTY10_ACCOUNTNAME
                ,@BRANCHID,@SETTLEMENT_FREQUENCY,@PAYMENTITEMID)";
                decimal tranAmount = 0;
                string mcc = "", Merchantname = "", Mccdesc = "", Sectorcode = "", Sector = "";

                decimal? custId = 0;
                var p = new DynamicParameters();
                var p1 = new DynamicParameters();
                //lgfn.loginfoMSG(DateTime.Now.ToString() +"TRACKING NEW BUILD", null, null);

                foreach (var d in processNonQuery(opdate, con))
                {
                    SETTLEMENTDATE = DateTime.Now;
                    DateTime? oldValueDate = null;
                    if (d.VALUEDATE == null)
                    {
                        continue;
                    }
                    var bl = DateTime.Compare(d.TRANDATETIME.Date, d.VALUEDATE.GetValueOrDefault().Date);
                    if (bl > 0)
                    {
                        oldValueDate = d.VALUEDATE;
                        d.VALUEDATE = d.TRANDATETIME;
                    }
                    tranDate = d.VALUEDATE.GetValueOrDefault();
                    decimal? PERC_ALLOTMERCHANT, appliedMsc, PERC_ALLOTOTHER, PERC_ALLOTUPSL, TOTAL_AMOUNTDUE, DEDUCTION_AMOUNTDUE, msc1_amount, AGENTSVALUE, AGENTRATE,
                            MSC2_AMOUNT, SUBSIDY_RATE, SUBSIDY_AMOUNT, PROCESSINGFEE, sharingmsc1_rate, SHARINGMSC1_AMOUNT, unsharedmsc1_rate, UNSHAREDMSC1_AMOUNT,
                            SHARINGMSC1_DIFFRATE, SHARINGMSC1_DIFFAMOUNT, NET_AMOUNTDUEMERCHANT, ACQR_RATE, ACQR_VALUE, COAQ_RATE, COAQ_VALUE, ISSR_RATE, ISSR_VALUE, IRF_RATE, IRF, AmountDue_Issuer,
                            PTSP_RATE, PTSP_VALUE, PTSA_RATE, PTSA_VALUE, BILLERRATE, BILLERVALUE, VASTRATE, VASTVALUE, VENDORRATE, VENDORVALUE, TERW_RATE, TERW_VALUE, SWTH_RATE, SWTH_VALUE, VEND_VALUE, AGNT_VALUE, msc2_rate, MSC1_RATE, PERC_ALLOTOTHERS,
                            TOTAL_MSC, STANDARD_MSC, total_mscamount, STANDARD_CAP, PROCSSORRATE, PROCSSOVALUE, BILLER_RECURATE, BILLER_RECUVALUE, MSC1_CAP, EFFECTIVEMSC, MSCConcession, MSCConcessionRate, EFF_SHAREDMSCCAPPED, MSCConcessionAmount
                            , msc2_margin, TECH_VALUE, TECH_RATE, REVW_VALUE, REVW_RATE, GOVT_VALUE, GOVT_RATE, COLB_VALUE, COLB_RATE;

                    EFFECTIVEMSC = MSC1_CAP = STANDARD_CAP = MSC1_RATE = total_mscamount = STANDARD_MSC = AGENTSVALUE = AGENTRATE = TOTAL_MSC = PERC_ALLOTMERCHANT = PERC_ALLOTOTHER = PERC_ALLOTUPSL = TOTAL_AMOUNTDUE = DEDUCTION_AMOUNTDUE = msc1_amount =
                    MSC2_AMOUNT = SUBSIDY_RATE = PROCSSORRATE = PROCSSOVALUE = BILLER_RECURATE = BILLER_RECUVALUE = SUBSIDY_AMOUNT = PROCESSINGFEE = sharingmsc1_rate = SHARINGMSC1_AMOUNT = MSCConcessionRate = MSCConcessionAmount = unsharedmsc1_rate = UNSHAREDMSC1_AMOUNT = PERC_ALLOTOTHERS =
                    SHARINGMSC1_DIFFRATE = SHARINGMSC1_DIFFAMOUNT = NET_AMOUNTDUEMERCHANT = MSCConcession = ACQR_RATE = ACQR_VALUE = ISSR_RATE = ISSR_VALUE = IRF_RATE = IRF = AmountDue_Issuer = EFF_SHAREDMSCCAPPED =
                    PTSP_RATE = PTSP_VALUE = BILLERRATE = BILLERVALUE = VASTRATE = VASTVALUE = VENDORRATE = VENDORVALUE = PTSA_RATE = appliedMsc = PTSA_VALUE = TERW_RATE = TERW_VALUE = SWTH_RATE = SWTH_VALUE = VEND_VALUE = AGNT_VALUE = msc2_rate
                    = TECH_VALUE = TECH_RATE = REVW_VALUE = REVW_RATE = GOVT_VALUE = GOVT_RATE = COLB_VALUE = COLB_RATE = COAQ_RATE = COAQ_VALUE = 0;
                    //decimal? whtRate, whtAmt, nibssRate, nibssAmt;
                    //whtRate = whtAmt = nibssAmt = nibssRate = 0;
                    //string whtAcct , nibssAcct;
                    //whtAcct = nibssAcct = string.Empty;
                    var md = new PMerchantObj();
                    string Issuername, Acquirername, BILLERCODE, Fullremtrans, SettlementCurrency, MDBAcct, MDBCode, MDBBank, MSC2SplitingAcct, Sign, SETTLEMENTACCOUNT,
                    MSC2Spliting, AGNT_ACCOUNT, VEND_NAME, VEND_ACCOUNT, TERW_NAME, TERW_ACCOUNT, SWTH_NAME, SWTH_ACCOUNT, PTSP_NAME, PTSP_ACCOUNT, PTSA_NAME, PTSA_ACCOUNT,
                   ISSR_NAME, ISSR_SHORTNAME, ISSR_ACCOUNT, ACQR_NAME, ACQR_ACCOUNT, mcc_meB, OrigMerchantid, MEB_BATCHNO, AGNT_NAME, AGNT_CODE, vrevenue_code, cardScheme, issr_country, instid,
                   shorttxtMess, extpaymentfields;

                    SWTH_NAME = SWTH_ACCOUNT = TERW_NAME = TERW_ACCOUNT = VEND_ACCOUNT = BILLERCODE = mcc_meB = VEND_NAME = AGNT_ACCOUNT = Issuername = Acquirername = Merchantname = Fullremtrans = SettlementCurrency = SETTLEMENTACCOUNT = MDBAcct = MDBCode = MDBBank = Sectorcode = Sector = MSC2SplitingAcct = Mccdesc = Sign =
                   ISSR_NAME = ISSR_ACCOUNT = MEB_BATCHNO = ACQR_NAME = ISSR_SHORTNAME = OrigMerchantid = ACQR_ACCOUNT = AGNT_CODE = AGNT_NAME = PTSP_NAME = shorttxtMess = vrevenue_code = instid = PTSP_ACCOUNT = PTSA_NAME = PTSA_ACCOUNT = MSC2Spliting = mcc = cardScheme = issr_country = extpaymentfields = string.Empty;
                    string acqrFiid = string.Empty;
                    string issrFiid = string.Empty;
                    string TERW_CODE = string.Empty;
                    string PTSP_CODE = string.Empty;
                    string PTSA_CODE = string.Empty;
                    string SWTH_CODE = string.Empty;

                    string MDBAcctold = string.Empty;
                    string MDBBankold = string.Empty;
                    string MDBCodeold = string.Empty;
                    string freqDesc = "";
                    Sign = d.SIGN;
                    tranAmount = d.TRANAMOUNT.GetValueOrDefault();
                    if (tranAmount == 0)
                    {

                        //string SqlStringA = @"INSERT INTO POSMISDB_NONFINANCIAL(id,docno,MERCHANTID,TERMINALID,OPDATE) Values ('" + ID.ToString() + "','" + DOCNO + "','" + Merchantid + "','" + Terminalid + "','" + Opdate.Value.ToString("dd-MMM-yyyy") + "')";

                        //try
                        //{
                        //    var cntt = con.Execute(SqlStringA,p1,commandType:CommandType.Text);
                        //}
                        //catch (Exception ex)
                        //{

                        //    lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());

                        //}
                        //string SqlStringB = @"UPDATE POSMISDB_MEB SET Process_status='F' WHERE lower(Process_status)='a' AND DOCNO='" + DOCNO + "'";

                        //try
                        //{
                        //    var cntt = con.Execute(SqlStringB, p1, commandType: CommandType.Text);

                        //}
                        //catch (Exception ex)
                        //{

                        //    lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());

                        //}
                        continue;//continue with other records in loop
                    }
                    else
                    {
                        string rvCode = string.Empty;
                        int workDays = 1;
                        if (d.CARDSCHEME == "ECSH")
                        {
                            // GET MAPPED MERCHANTID AND TERMINAL ID
                            var objM = GetMerchantTerminal(d.MERCHANTID, con);
                            if (objM != null)
                            {
                                d.MERCHANTID = objM.MerchantId;
                                d.TERMINALID = objM.TerminalId;
                            }
                        }
                        md = GetMerchantDetail(d.MERCHANTID, con);
                        if (md != null)
                        {
                            mcc = md.mcc_code;
                            Merchantname = md.merchantname;
                            Mccdesc = md.mcc_desc;
                            Sectorcode = md.sector_code;
                            Sector = md.sector;
                            custId = md.customerid;

                        }
                        else
                        {
                            try
                            {
                                errorMsg = "Invalid Merchant Detail";
                                LogErrorMessage(d.ID, d.MERCHANTID, d.TERMINALID, d.CREATEDATETIME, errorMsg, DateTime.Now, d.TRANDATETIME, d.PAYMENTREFERENCE, d.TRANAMOUNT, con);

                            }
                            catch (Exception ex)
                            {
                                lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());

                            }
                            continue;
                        }


                        int v_NODAYS = 0;

                        var td = GetTerminalDetail(d.TERMINALID, d.TRANCURRENCY, con);
                        if (td != null)
                        {
                            try
                            {
                                MDBAcct = td.DEPOSIT_ACCOUNTNO;
                                MDBBank = td.DEPOSIT_BANKNAME;
                                MDBCode = td.DEPOSIT_BANKCODE;


                                MDBAcctold = td.DEPOSIT_ACCOUNTNO;
                                MDBBankold = td.DEPOSIT_BANKNAME;
                                MDBCodeold = td.DEPOSIT_BANKCODE;

                                TERW_CODE = td.TERMINALOWNER_CODE;
                                TERW_NAME = td.Termownername;
                                PTSA_CODE = td.PTSA;
                                PTSA_NAME = td.PTSANAME;
                                PTSP_CODE = td.PTSP;
                                PTSP_NAME = td.ptspname;
                                PTSP_ACCOUNT = td.PTSPAcct;
                                SWTH_CODE = td.SWTCH;
                                SWTH_NAME = td.SWTCHNAME;
                                SettlementCurrency = td.SETTLEMENT_CURRENCY;



                                v_NODAYS = td.workdays ?? 0;
                                freqDesc = td.FREQUENCY_DESC;
                                SETTLEMENTDATE = tranDate.Date.AddDays(v_NODAYS);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            try
                            {
                                errorMsg = "Invalid Terminal Detail";
                                LogErrorMessage(d.ID, d.MERCHANTID, d.TERMINALID, d.CREATEDATETIME, errorMsg, DateTime.Now, d.TRANDATETIME, d.PAYMENTREFERENCE, d.TRANAMOUNT, con);
                            }
                            catch (Exception ex)
                            {
                                lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());
                            }
                            continue;
                        }
                        decimal signType;
                        //decimal v_processfee = 0M;
                        //decimal v_IRFRATE = 0;
                        decimal v_DEFAULTMSC = 0;
                        //decimal v_IRFAMT = 0;
                        if (Sign == "-")
                        {
                            signType = -1;

                        }
                        else
                        {
                            signType = 1;
                        }
                        if (d.CARDSCHEME == "ECSH")
                        {
                            acqrFiid = "XP";
                            issrFiid = "XP";
                            // GET AGENT NAME
                            var ag_dt = GetEcashierAgentDetail(acqrFiid, con);
                            if (ag_dt != null)
                            {
                                AGNT_NAME = ag_dt.AGENT_NAME;
                                AGNT_ACCOUNT = ag_dt.AGENT_ACCTNO;
                            }

                            // GET MAPPED MERCHANTID AND TERMINAL ID
                            //var objM = GetMerchantTerminal(d.MERCHANTID, con);
                            //if (objM != null)
                            //{
                            //    d.MERCHANTID = objM.MerchantId;
                            //    d.TERMINALID = objM.TerminalId;
                            //}
                            rvCode = GetRevenueCode(d.SPECIALMESSAGE1);
                            if (!string.IsNullOrEmpty(rvCode))
                            {
                                var rvDet = GetRevenueCodeDetail(rvCode, d.PAYMENTITEMID, con);
                                if (rvDet != null)
                                {
                                    if (rvDet.GLOBALACCOUNTFLAG == true)
                                    {
                                        MDBAcct = rvDet.RVG_DEPOSIT_ACCOUNTNO;
                                        MDBBank = rvDet.RVG_DEPOSIT_BANKNAME;
                                        MDBCode = rvDet.RVG_DEPOSIT_BANKCODE;
                                    }
                                    else
                                    {
                                        MDBAcct = rvDet.RVH_DEPOSIT_ACCOUNTNO;
                                        MDBBank = rvDet.RVH_DEPOSIT_BANKNAME;
                                        MDBCode = rvDet.RVH_DEPOSIT_BANKCODE;
                                    }
                                    workDays = rvDet.WORKDAYS.GetValueOrDefault();
                                    SETTLEMENTDATE = tranDate.AddDays(workDays);
                                    freqDesc = rvDet.FREQUENCY_DESC;
                                    // moving settlement date to future date when current date is greater than settlement dates
                                    // for regular settlement frequency
                                    var curDate = DateTime.Now;
                                    if (enforceFutureSetDate.ToLower() == "y")
                                    {
                                        if (DateTime.Compare(DateTime.Now.Date, SETTLEMENTDATE.Date) == 1)
                                        {
                                            SETTLEMENTDATE = curDate.AddDays(workDays);
                                        }
                                    }

                                    // for custom settlement frequency
                                    if (rvDet.SETTLEMENT_FREQUENCY != null)
                                    {
                                        if (rvDet.CUSTOM)
                                        {
                                            //if (rvDet.SET_DATE_TERM == 1)
                                            //{
                                            switch (rvDet.FREQ_CODE)
                                            {
                                                case "W":
                                                    {
                                                        //var currentDay = (int)tranDate.DayOfWeek;
                                                        //int daysAdded = 0;
                                                        //if (currentDay != 0)
                                                        //{
                                                        //    daysAdded = (7 - currentDay) + rvDet.SET_DAYS.GetValueOrDefault();
                                                        //}
                                                        //else
                                                        //{
                                                        //    daysAdded = rvDet.SET_DAYS.GetValueOrDefault();
                                                        //}
                                                        // var daysAdded = (7 - currentDay) + rvDet.SET_DAYS.GetValueOrDefault();
                                                        // daysAdded = 
                                                        // SETTLEMENTDATE = tranDate.AddDays(daysAdded);
                                                        //var curDay = SETTLEMENTDATE.DayOfWeek;
                                                        SETTLEMENTDATE = getCustomSettlementDate(tranDate, rvDet.SET_DAYS.GetValueOrDefault(), "W");
                                                        if (enforceFutureSetDate.ToLower() == "y")
                                                        {
                                                            if (DateTime.Compare(DateTime.Now.Date, SETTLEMENTDATE.Date) == 1)
                                                            {
                                                                SETTLEMENTDATE = getCustomSettlementDate(tranDate, rvDet.SET_DAYS.GetValueOrDefault(), "W");
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "M":
                                                    {
                                                        //var currentDay = tranDate.Day;
                                                        //var daysInMonth = DateTime.DaysInMonth(tranDate.Year, tranDate.Month);
                                                        //SETTLEMENTDATE = tranDate.AddDays((daysInMonth - currentDay));
                                                        //var daysAdded = 0;
                                                        //for (int i = 1; i <= daysInMonth; i++)
                                                        //{
                                                        //    //if(WeekendList SETTLEMENTDATE.DayOfWeek)
                                                        //    if (daysAdded == rvDet.SET_DAYS)
                                                        //    {
                                                        //        break;
                                                        //    }
                                                        //    var hj = GetWeekendList();
                                                        //    SETTLEMENTDATE = SETTLEMENTDATE.AddDays(1);
                                                        //    if (hj.Contains(SETTLEMENTDATE.DayOfWeek))
                                                        //    {
                                                        //        continue;
                                                        //    }
                                                        //    daysAdded++;
                                                        //    //sett
                                                        //}
                                                        SETTLEMENTDATE = getCustomSettlementDate(tranDate, rvDet.SET_DAYS.GetValueOrDefault(), "M");
                                                        if (enforceFutureSetDate.ToLower() == "y")
                                                        {

                                                            if (DateTime.Compare(DateTime.Now.Date, SETTLEMENTDATE.Date) == 1)
                                                            {
                                                                SETTLEMENTDATE = getCustomSettlementDate(tranDate, rvDet.SET_DAYS.GetValueOrDefault(), "M");
                                                            }
                                                        }
                                                        break;
                                                    }
                                            }
                                            //}
                                            //else if (rvDet.SET_DATE_TERM == 2)
                                            //{
                                            //    switch (rvDet.FREQ_CODE)
                                            //    {
                                            //        case "W":
                                            //            {
                                            //                var currentDay = (int)tranDate.DayOfWeek;
                                            //                int daysAdded = 0;
                                            //                if (currentDay != 0)
                                            //                {
                                            //                    daysAdded = (7 - currentDay) + rvDet.SET_DAYS.GetValueOrDefault();
                                            //                }
                                            //                else
                                            //                {
                                            //                    daysAdded = rvDet.SET_DAYS.GetValueOrDefault();
                                            //                }
                                            //                SETTLEMENTDATE = tranDate.AddDays(daysAdded);
                                            //                break;
                                            //            }
                                            //        case "M":
                                            //            {
                                            //                var currentDay = tranDate.Day;
                                            //                var daysInMonth = DateTime.DaysInMonth(tranDate.Year, tranDate.Month);
                                            //                SETTLEMENTDATE = tranDate.AddDays((daysInMonth - currentDay));
                                            //                var daysAdded = 0;
                                            //                for (int i = 1; i <= daysInMonth; i++)
                                            //                {
                                            //                    //if(WeekendList SETTLEMENTDATE.DayOfWeek)
                                            //                    if (daysAdded == rvDet.SET_DAYS)
                                            //                    {
                                            //                        break;
                                            //                    }
                                            //                    var hj = GetWeekendList();
                                            //                    SETTLEMENTDATE = SETTLEMENTDATE.AddDays(1);
                                            //                    if (hj.Contains(SETTLEMENTDATE.DayOfWeek))
                                            //                    {
                                            //                        continue;
                                            //                    }
                                            //                    daysAdded++;
                                            //                    //sett
                                            //                }
                                            //                break;
                                            //            }
                                            //    }
                                            //}
                                        }
                                    }
                                }
                                else
                                {
                                    //log the request in exception
                                    errorMsg = "Invalid Revenue Head Code- " + rvCode;
                                    LogErrorMessage(d.ID, d.MERCHANTID, d.TERMINALID, d.CREATEDATETIME, errorMsg, DateTime.Now, d.TRANDATETIME, d.PAYMENTREFERENCE, d.TRANAMOUNT, con);
                                    continue;
                                }
                            }
                            else
                            {
                                //log the request in exception
                                errorMsg = "Invalid Revenue Head Code- " + rvCode;
                                LogErrorMessage(d.ID, d.MERCHANTID, d.TERMINALID, d.CREATEDATETIME, errorMsg, DateTime.Now, d.TRANDATETIME, d.PAYMENTREFERENCE, d.TRANAMOUNT, con);
                                continue;
                            }
                        }
                        else
                        {
                            acqrFiid = d.ACQUIRERFIID;
                            issrFiid = d.ISSUERFIID;
                        }
                        //Determine settlement date
                        //if (md != null)
                        //{
                        //    if (md.SETTLEMENT_FREQUENCY != null)
                        //    {
                        //        if (md.CUSTOM)
                        //        {
                        //            if (md.SET_DATE_TERM == 1)
                        //            {
                        //                switch (md.FREQ_CODE)
                        //                {
                        //                    case "W":
                        //                        {
                        //                            var currentDay = (int)d.TRANDATETIME.DayOfWeek;
                        //                            var daysAdded = (7 - currentDay) + md.SET_DAYS.GetValueOrDefault();
                        //                            SETTLEMENTDATE = d.TRANDATETIME.AddDays(daysAdded);
                        //                            var curDay = SETTLEMENTDATE.DayOfWeek;
                        //                            break;
                        //                        }
                        //                    case "M":
                        //                        {
                        //                            var currentDay = d.TRANDATETIME.Day;
                        //                            var daysInMonth = d.TRANDATETIME.Day;
                        //                            SETTLEMENTDATE = d.TRANDATETIME.AddDays((daysInMonth - currentDay) + 1);
                        //                            //for(var i = 0;i<7;i++)
                        //                            //{
                        //                            //    if((int)SETTLEMENTDATE.DayOfWeek == md.SET_DAYS.GetValueOrDefault())
                        //                            //    {
                        //                            //    }
                        //                            //}
                        //                            break;
                        //                        }
                        //                }
                        //            }
                        //            else if (md.SET_DATE_TERM == 2)
                        //            {
                        //                switch (md.FREQ_CODE)
                        //                {
                        //                    case "W":
                        //                        {
                        //                            var currentDay = (int)d.TRANDATETIME.DayOfWeek;
                        //                            var days = (7 - currentDay) + md.SET_DAYS.GetValueOrDefault();
                        //                            SETTLEMENTDATE = d.TRANDATETIME.AddDays(days);
                        //                            break;
                        //                        }
                        //                    case "M":
                        //                        {
                        //                            var currentDay = d.TRANDATETIME.Day;
                        //                            var daysInMonth = DateTime.DaysInMonth(d.TRANDATETIME.Year,d.TRANDATETIME.Month);
                        //                            SETTLEMENTDATE = d.TRANDATETIME.AddDays((daysInMonth - currentDay));
                        //                            var daysAdded = 0;
                        //                            for (int i = 1; i <= daysInMonth; i++)
                        //                            {
                        //                                //if(WeekendList SETTLEMENTDATE.DayOfWeek)
                        //                                if(daysAdded == md.SET_DAYS)
                        //                                {
                        //                                    break;
                        //                                }
                        //                                var hj = GetWeekendList();
                        //                                SETTLEMENTDATE = SETTLEMENTDATE.AddDays(1);
                        //                                if (hj.Contains(SETTLEMENTDATE.DayOfWeek))
                        //                                {
                        //                                    continue;
                        //                                }
                        //                                daysAdded++;
                        //                                //sett
                        //                            }
                        //                            break;
                        //                        }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        string spliMSC2SHARING = "";
                        var spliMSC2SHARINGACCT = "";
                        string SPECIALMESSAGE4 = "";
                        string drAcct = "", drBankCode = "";

                        try
                        {
                            var curDate = DateTime.Now;
                            if (SETTLEMENTDATE < curDate.Date && v_NODAYS == 1)
                            {
                                SETTLEMENTDATE = curDate.Date.AddDays(1);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        objMsc = GetMSC(d.MERCHANTID, d.TERMINALID, d.TRANCURRENCY, SettlementCurrency, tranAmount, d.ORGINALAMOUNT, mcc, d.REFNO.ToString(), d.CHANNELID.GetValueOrDefault(),
                           d.CARDSCHEME, d.ACQUIRERFIID, Sign, d.SPECIALMESSAGE1, d.SPECIALMESSAGE2, d.SPECIALMESSAGE3, d.SPECIALMESSAGE4, batchNo, d.AGENT_CODE, rvCode, con);
                        decimal MSC2_CAP = 0;
                        if (objMsc != null)
                        {
                            ACQR_RATE = objMsc.ACQRRATE ?? 0;
                            ACQR_VALUE = objMsc.ACQRAMOUNT ?? 0;
                            COAQ_RATE = objMsc.COAQRATE ?? 0;
                            COAQ_VALUE = objMsc.COAQAMOUNT ?? 0;
                            ISSR_RATE = objMsc.ISSRRATE ?? 0;
                            ISSR_VALUE = objMsc.ISSRAMOUNT ?? 0;
                            PTSP_RATE = objMsc.PTSPRATE ?? 0;
                            PTSP_VALUE = objMsc.PTSPAMOUNT ?? 0;
                            PTSA_RATE = objMsc.PTSARATE ?? 0;
                            PTSA_VALUE = objMsc.PTSAAMOUNT ?? 0;
                            TERW_RATE = objMsc.TERWRATE ?? 0;
                            TERW_VALUE = objMsc.TERWAMOUNT ?? 0;
                            SWTH_RATE = objMsc.SWTHRATE ?? 0;
                            SWTH_VALUE = objMsc.SWTHAMOUNT ?? 0;
                            BILLER_RECURATE = objMsc.BILRRATE ?? 0;
                            BILLER_RECUVALUE = objMsc.BILRAMOUNT ?? 0;
                            GOVT_RATE = objMsc.GOVTRATE ?? 0;
                            GOVT_VALUE = objMsc.GOVTAMOUNT ?? 0;
                            REVW_RATE = objMsc.REVWRATE ?? 0;
                            REVW_VALUE = objMsc.REVWAMOUNT ?? 0;
                            AGENTRATE = objMsc.AGNTRATE;
                            AGNT_VALUE = objMsc.AGNTAMOUNT;
                            COLB_RATE = objMsc.COLBRATE ?? 0;
                            COLB_VALUE = objMsc.COLBAMOUNT ?? 0;
                            TECH_RATE = objMsc.TECHRATE ?? 0;
                            TECH_VALUE = objMsc.TECHAMOUNT ?? 0;
                            VENDORRATE = objMsc.VENDRATE ?? 0;
                            VENDORVALUE = objMsc.VENDAMOUNT ?? 0;
                            VASTRATE = objMsc.VASTRATE ?? 0;
                            VASTVALUE = objMsc.VASTAMOUNT ?? 0;
                            TOTAL_AMOUNTDUE = objMsc.TOTAL_AMOUNTDUE ?? 0;
                            NET_AMOUNTDUEMERCHANT = objMsc.AMT_DUEMERCHANT ?? 0;
                            STANDARD_CAP = objMsc.DEFAULTMSC_CAP ?? 0;
                            EFFECTIVEMSC = objMsc.EFFECTIVEMSC ?? 0;
                            msc1_amount = objMsc.DEFAULTMSCAMT ?? 0;
                            DEDUCTION_AMOUNTDUE = objMsc.DEDUCTION_AMOUNTDUE ?? 0;
                            v_DEFAULTMSC = objMsc.DEFAULTMSC ?? 0;
                            MSC1_CAP = objMsc.MSC1CAP ?? 0;
                            MSC2_CAP = objMsc.MSC2CAP ?? 0;
                            total_mscamount = objMsc.APPLIEDMSCVALUE ?? 0;
                            MSC2_AMOUNT = objMsc.MSC2AMOUNT ?? 0;
                            msc2_rate = objMsc.MSC2 ?? 0;
                            msc1_amount = objMsc.MSC1AMOUNT ?? 0;
                            MSC1_RATE = objMsc.MSC1 ?? 0;
                            TOTAL_MSC = objMsc.TRUEMSC_RATE ?? 0 * signType;
                            PERC_ALLOTMERCHANT = objMsc.PERC_ALLOTMERCHANT ?? 0;

                            SHARINGMSC1_DIFFRATE = objMsc.SHARINGMSCDIFFRATE ?? 0;
                            SHARINGMSC1_DIFFAMOUNT = objMsc.SHARINGMSCDIFFVALUE ?? 0;
                            sharingmsc1_rate = objMsc.SHAREDMSC ?? 0;
                            SHARINGMSC1_AMOUNT = objMsc.SHAREDMSCAMT ?? 0;
                            SUBSIDY_AMOUNT = objMsc.SUBSIDYAMOUNT ?? 0;
                            SUBSIDY_RATE = objMsc.MSCSUBSIDY ?? 0;
                            spliMSC2SHARING = objMsc.spliMSC2SHARING;
                            spliMSC2SHARINGACCT = objMsc.spliMSC2SHARINGACCT;
                            SPECIALMESSAGE4 = objMsc.SPECIALMESSAGE4;

                            STANDARD_MSC = objMsc.STANDARDRATE ?? 0 * signType;

                            unsharedmsc1_rate = objMsc.UNSHARINGMSCRATE ?? 0;
                            UNSHAREDMSC1_AMOUNT = objMsc.UNSHARINGMSCVALUE ?? 0 * signType;
                            EFF_SHAREDMSCCAPPED = objMsc.SHARINGMSCVALUECAPPED ?? 0;
                            total_mscamount = objMsc.MSCAMOUNT;
                            drAcct = objMsc.DR_ACCTNO;
                            msc2_margin = objMsc.MSC2MARGIN;
                            //whtRate = objMsc.WHTAXRATE;
                            //whtAmt = objMsc.WHTAXAMOUNT;
                            //whtAcct = objMsc.WHTACCOUNT;
                            //nibssRate = objMsc.NIBSSRATE;
                            //nibssAmt = objMsc.NIBSSAMOUNT;
                            //nibssAcct = objMsc.NIBSSACCOUNT;
                            drBankCode = objMsc.DR_BANKCODE;
                        }
                        else
                        {
                            //unable to get msc calculation
                            continue;
                        }
                        try
                        {
                            var is_dt2 = GetAcquireIssuerDetail(issrFiid, con);


                            if (is_dt2 != null)
                            {

                                ISSR_ACCOUNT = is_dt2.DEPOSIT_ACCOUNTNO;
                                ISSR_NAME = is_dt2.INSTITUTION_NAME;
                            }

                        }
                        catch { }
                        var aq_dt = GetAcquireIssuerDetail(acqrFiid, con);
                        if (aq_dt != null)

                        {
                            ACQR_NAME = aq_dt.INSTITUTION_NAME;
                            ACQR_ACCOUNT = aq_dt.DEPOSIT_ACCOUNTNO;
                        }


                        try
                        {
                            p = new DynamicParameters();
                            //post transactions into local tranx master for settlement
                            SM_SETTLEMENTDETAIL obj = new SM_SETTLEMENTDETAIL();
                            p.Add("@ACQR_ACCOUNT", ACQR_ACCOUNT, DbType.String);
                            p.Add("@ACQR_CODE", d.ACQUIRERFIID, DbType.String);
                            p.Add("@ACQR_NAME", ACQR_NAME, DbType.String);
                            p.Add("@ACQR_RATE", ACQR_RATE, DbType.Decimal);
                            p.Add("@ACQR_VALUE", ACQR_VALUE, DbType.Decimal);
                            p.Add("@ACQUIRERCURRENCY", obj.ACQUIRERCURRENCY, DbType.String);
                            p.Add("@ACQUIRERFEE", d.ACQUIRERFEE, DbType.Decimal);

                            p.Add("@ACQUIRERFEECURRENCY", d.ACQUIRERFEECURRENCY, DbType.String);
                            p.Add("@ACQUIRERFIID", d.ACQUIRERFIID, DbType.String);
                            p.Add("@ACQUIRER_REF", obj.ACQUIRER_REF, DbType.String);
                            p.Add("@AGNT_ACCOUNT", AGNT_ACCOUNT, DbType.String);
                            p.Add("@AGNT_CODE", AGNT_CODE, DbType.String);
                            p.Add("@AGNT_NAME", AGNT_NAME, DbType.String);
                            p.Add("@AGNT_RATE", AGENTRATE, DbType.Decimal);

                            p.Add("@AGNT_VALUE", AGNT_VALUE, DbType.Decimal);
                            p.Add("@APPROVALCODE", d.APPROVALCODE, DbType.String);
                            p.Add("@APPROVALMESSAGE", d.APPROVALMESSAGE, DbType.String);
                            p.Add("@BATCHNO", d.BATCHNO, DbType.Int32);
                            p.Add("@BENEFICIARYACCOUNTNO", obj.BENEFICIARYACCOUNTNO, DbType.String);
                            p.Add("@BENEFICIARYBANK", obj.BENEFICIARYBANK, DbType.String);
                            p.Add("@BENEFICIARYBANKCODE", obj.BENEFICIARYBANKCODE, DbType.String);

                            p.Add("@BENEFICIARYNAME", obj.BENEFICIARYNAME, DbType.String);
                            p.Add("@BILR_ACCOUNT", obj.BILR_ACCOUNT, DbType.String);
                            p.Add("@BILR_CODE", obj.BILR_CODE, DbType.String);
                            p.Add("@BILR_NAME", obj.BILR_NAME, DbType.String);
                            p.Add("@BILR_RATE", BILLER_RECURATE, DbType.Decimal);
                            p.Add("@BILR_VALUE", BILLER_RECUVALUE, DbType.Decimal);
                            p.Add("@BLLR_ACCOUNT", obj.BLLR_ACCOUNT, DbType.String);

                            p.Add("@BLLR_CODE", obj.BLLR_CODE, DbType.String);
                            p.Add("@BLLR_NAME", obj.BLLR_NAME, DbType.String);
                            p.Add("@BLLR_RATE", BILLERRATE, DbType.Decimal);
                            p.Add("@BLLR_VALUE", BILLERVALUE, DbType.Decimal);
                            p.Add("@CARDCOUNTRY", obj.CARDCOUNTRY, DbType.String);
                            p.Add("@CARDSCHEME", d.CARDSCHEME, DbType.String);
                            p.Add("@CHANNELID", d.CHANNELID, DbType.Int32);

                            p.Add("@COAQ_ACCOUNT", obj.COAQ_ACCOUNT, DbType.String);
                            p.Add("@COAQ_CODE", obj.COAQ_CODE, DbType.String);
                            p.Add("@COAQ_NAME", obj.COAQ_NAME, DbType.String);
                            p.Add("@COAQ_RATE", COAQ_RATE, DbType.Decimal);
                            p.Add("@COAQ_VALUE", COAQ_VALUE, DbType.Decimal);
                            p.Add("@CREATEDATETIME", d.CREATEDATETIME, DbType.DateTime);
                            p.Add("@CUSTOMERACCOUNT", obj.CUSTOMERACCOUNT, DbType.String);
                            p.Add("@CUSTOMERID", custId, DbType.Decimal);

                            p.Add("@CUSTOMERNAME", d.CUSTOMERNAME, DbType.String);
                            p.Add("@DEDUCTION_AMOUNTDUE", obj.DEDUCTION_AMOUNTDUE, DbType.Decimal);
                            p.Add("@DOCNO", obj.DOCNO, DbType.Decimal);
                            p.Add("@EFFECTIVEMSC", EFFECTIVEMSC, DbType.Decimal);
                            p.Add("@EXCHANGERATE", obj.EXCHANGERATE, DbType.Decimal);
                            p.Add("@EXPDATETIME", d.EXPDATETIME, DbType.DateTime);
                            p.Add("@EXTRRN", obj.EXTRRN, DbType.String);

                            p.Add("@FCYAMOUNT", obj.FCYAMOUNT, DbType.Decimal);
                            p.Add("@FULLREM_TRANSLATION", d.TRANDESC, DbType.String);
                            p.Add("@GOVT_ACCOUNT", obj.GOVT_ACCOUNT, DbType.String);
                            p.Add("@GOVT_CODE", obj.GOVT_CODE, DbType.String);
                            p.Add("@GOVT_NAME", obj.GOVT_NAME, DbType.String);
                            p.Add("@GOVT_RATE", GOVT_RATE, DbType.Decimal);
                            p.Add("@GOVT_VALUE", GOVT_VALUE, DbType.Decimal);

                            p.Add("@HOST", obj.HOST, DbType.Decimal);
                            p.Add("@ID", d.ID, DbType.Int64);
                            p.Add("@INTERCHANGEFEE", d.INTERCHANGEFEE, DbType.Decimal);
                            p.Add("@INTERCHANGEFEECUR", d.INTERCHANGEFEECUR, DbType.Decimal);
                            p.Add("@INVOICENO", d.INVOICENO, DbType.String);
                            p.Add("@INVOICENUM", obj.INVOICENUM, DbType.String);
                            p.Add("@IRF", obj.IRF, DbType.Decimal);

                            p.Add("@ISSR_ACCOUNT", ISSR_ACCOUNT, DbType.String);
                            p.Add("@ISSR_CODE", d.ISSUERFIID, DbType.String);
                            p.Add("@ISSR_NAME", ISSR_NAME, DbType.String);
                            p.Add("@ISSR_RATE", ISSR_RATE, DbType.Decimal);
                            p.Add("@ISSR_VALUE", ISSR_VALUE, DbType.Decimal);
                            p.Add("@ISSUERCURRENCY", d.ISSUERCURRENCY, DbType.String);
                            p.Add("@ISSUERFEE", d.ISSUERFEE, DbType.Decimal);

                            p.Add("@ISSUERFEECURRENCY", d.ISSUERFEECURRENCY, DbType.String);
                            p.Add("@ISSUERFIID", d.ISSUERFIID, DbType.String);
                            p.Add("@LCYAMOUNT", obj.LCYAMOUNT, DbType.Decimal);
                            p.Add("@MASKEDPAN", obj.MASKEDPAN, DbType.String);
                            p.Add("@MCC", mcc, DbType.String);
                            p.Add("@MCC_DESCRIPTION", Mccdesc, DbType.String);
                            p.Add("@MERCHANTID", d.MERCHANTID, DbType.String);
                            p.Add("@MERCHANTNAME", Merchantname, DbType.String);

                            p.Add("@MERCHANTDEPOSITBANK", MDBBank, DbType.String);
                            p.Add("@MERCHANTDEPOSITBANKACCTNO", MDBAcct, DbType.String);
                            p.Add("@MERCHANTDEPOSITBANKCODE", MDBCode, DbType.String);
                            p.Add("@MERCHANTLOCATION", d.MERCHANTLOCATION, DbType.String);
                            p.Add("@MSC1_AMOUNT", msc1_amount, DbType.Decimal);
                            p.Add("@MSC1_CAP", MSC1_CAP, DbType.Decimal);
                            p.Add("@MSC1_RATE", MSC1_RATE, DbType.Decimal);

                            p.Add("@MSC2_AMOUNT", MSC2_AMOUNT, DbType.Decimal);
                            p.Add("@MSC2_CAP", MSC2_CAP, DbType.Decimal);
                            p.Add("@MSC2_RATE", msc2_rate, DbType.Decimal);
                            p.Add("@MSC2_SPLITSHARING", spliMSC2SHARING, DbType.String);
                            p.Add("@MSC2_SPLITSHARINGACCT", MSC2SplitingAcct, DbType.String);
                            p.Add("@NET_AMOUNTDUEMERCHANT", NET_AMOUNTDUEMERCHANT, DbType.Decimal);
                            p.Add("@ORGINALAMOUNT", d.ORGINALAMOUNT, DbType.Decimal);

                            p.Add("@ORGINALCURRENCY", d.ORGINALCURRENCY, DbType.String);
                            p.Add("@ORIGID", obj.ORIGID, DbType.String);
                            p.Add("@ORIGTYPE", obj.ORIGTYPE, DbType.String);
                            p.Add("@PAN", d.PAN, DbType.String);
                            p.Add("@PERC_ALLOTHOLDER", obj.PERC_ALLOTHOLDER, DbType.Decimal);
                            p.Add("@PERC_ALLOTMERCHANT", obj.PERC_ALLOTMERCHANT, DbType.Decimal);
                            p.Add("@PERC_ALLOTOTHERS", obj.PERC_ALLOTOTHERS, DbType.Decimal);

                            p.Add("@PHONENO", obj.PHONENO, DbType.String);
                            p.Add("@PROCESSINGFEE", obj.PROCESSINGFEE, DbType.Decimal);
                            p.Add("@PROCESS_STATUS", "S", DbType.String);
                            p.Add("@PTSA_ACCOUNT", PTSA_ACCOUNT, DbType.String);
                            p.Add("@PTSA_CODE", PTSA_CODE, DbType.String);
                            p.Add("@PTSA_NAME", PTSA_NAME, DbType.String);
                            p.Add("@PTSA_RATE", PTSA_RATE, DbType.Decimal);

                            p.Add("@PTSA_VALUE", PTSA_VALUE, DbType.Decimal);
                            p.Add("@PTSP_ACCOUNT", PTSP_ACCOUNT, DbType.String);
                            p.Add("@PTSP_CODE", PTSP_CODE, DbType.String);
                            p.Add("@PTSP_NAME", PTSP_NAME, DbType.String);
                            p.Add("@PTSP_RATE", PTSP_RATE, DbType.Decimal);
                            p.Add("@PTSP_VALUE", PTSP_VALUE, DbType.Decimal);
                            p.Add("@REASONFORALTERATION", obj.REASONFORALTERATION, DbType.String);

                            p.Add("@RECORDID", d.RECORDID, DbType.Decimal);
                            p.Add("@REFNO", d.REFNO, DbType.Decimal);
                            p.Add("@REMARK", d.REMARK, DbType.String);
                            p.Add("@REVERSEREQID", obj.REVERSEREQID, DbType.Decimal);
                            p.Add("@REVW_ACCOUNT", obj.REVW_ACCOUNT, DbType.String);
                            p.Add("@REVW_CODE", obj.REVW_CODE, DbType.String);
                            p.Add("@REVW_NAME", obj.REVW_NAME, DbType.String);

                            p.Add("@REVW_RATE", REVW_RATE, DbType.Decimal);
                            p.Add("@REVW_VALUE", REVW_VALUE, DbType.Decimal);
                            p.Add("@SECTOR", Sector, DbType.String);
                            p.Add("@SECTORCODE", Sectorcode, DbType.String);
                            p.Add("@SEQNUM", obj.SEQNUM, DbType.String);
                            p.Add("@SERVICEPROVIDER", obj.SERVICEPROVIDER, DbType.String);
                            p.Add("@SERVICEPROVIDERACCOUNTNO", obj.SERVICEPROVIDERACCOUNTNO, DbType.String);

                            p.Add("@SERVICEPROVIDERBANK", obj.SERVICEPROVIDERBANK, DbType.String);
                            p.Add("@SERVICEPROVIDERBANKCODE", obj.SERVICEPROVIDERBANKCODE, DbType.String);
                            p.Add("@SETTLEMENTACCOUNT", MDBAcct, DbType.String);
                            p.Add("@SETTLEMENTAMOUNT", NET_AMOUNTDUEMERCHANT, DbType.Decimal);
                            p.Add("@SETTLEMENTCURRENCY", SettlementCurrency, DbType.String);
                            p.Add("@SETTLEMENTDATETIME", SETTLEMENTDATE, DbType.Date);
                            p.Add("@SHARINGMSC1_AMOUNT", SHARINGMSC1_AMOUNT, DbType.Decimal);

                            p.Add("@SHARINGMSC1_RATE", sharingmsc1_rate, DbType.Decimal);
                            p.Add("@SIGN", d.SIGN, DbType.String);
                            p.Add("@SOURCEDB", d.SOURCEDB, DbType.String);
                            p.Add("@SOURCETABLE", d.SOURCETABLE, DbType.String);
                            p.Add("@SPECIALCARD", obj.SPECIALCARD, DbType.String);
                            p.Add("@SPECIALCARD_OWNER", obj.SPECIALCARD_OWNER, DbType.String);
                            p.Add("@SPECIALMESSAGE1", d.SPECIALMESSAGE1, DbType.String);
                            p.Add("@SPECIALMESSAGE2", d.SPECIALMESSAGE2, DbType.String);
                            p.Add("@SPECIALMESSAGE3", d.SPECIALMESSAGE3, DbType.String);
                            p.Add("@SPECIALMESSAGE4", d.SPECIALMESSAGE4, DbType.String);
                            p.Add("@SPECIALMESSAGE4", SPECIALMESSAGE4, DbType.String);

                            p.Add("@STAN", d.STAN, DbType.String);
                            p.Add("@SUBSIDY_AMOUNT", SUBSIDY_AMOUNT, DbType.Decimal);
                            p.Add("@SUBSIDY_RATE", SUBSIDY_RATE, DbType.Decimal);
                            p.Add("@SWTH_ACCOUNT", SWTH_ACCOUNT, DbType.String);
                            p.Add("@SWTH_CODE", SWTH_CODE, DbType.String);
                            p.Add("@SWTH_NAME", SWTH_NAME, DbType.String);
                            p.Add("@SWTH_RATE", SWTH_RATE, DbType.Decimal);
                            p.Add("@SWTH_VALUE", SWTH_VALUE, DbType.Decimal);
                            p.Add("@TECH_ACCOUNT", obj.TECH_ACCOUNT, DbType.String);
                            p.Add("@TECH_CODE", obj.TECH_CODE, DbType.String);
                            p.Add("@TECH_NAME", obj.TECH_NAME, DbType.String);
                            p.Add("@TECH_RATE", TECH_RATE, DbType.Decimal);

                            p.Add("@TECH_VALUE", TECH_VALUE, DbType.Decimal);
                            p.Add("@TERMINALID", d.TERMINALID, DbType.String);
                            p.Add("@TERW_ACCOUNT", TERW_ACCOUNT, DbType.String);
                            p.Add("@TERW_CODE", TERW_CODE, DbType.String);
                            p.Add("@TERW_NAME", TERW_NAME, DbType.String);
                            p.Add("@TERW_RATE", TERW_RATE, DbType.Decimal);
                            p.Add("@TERW_VALUE", TERW_VALUE, DbType.Decimal);
                            p.Add("@TOTALMSC_AMOUNT", total_mscamount, DbType.Decimal);
                            p.Add("@TOTAL_AMOUNTDUE", NET_AMOUNTDUEMERCHANT, DbType.Decimal);
                            p.Add("@TRACENO", obj.TRACENO, DbType.Decimal);
                            p.Add("@TRANAMOUNT", d.TRANAMOUNT, DbType.Decimal);
                            p.Add("@TRANCODE", d.TRANCODE, DbType.String);

                            p.Add("@TRANCURRENCY", d.TRANCURRENCY, DbType.String);
                            p.Add("@TRANDATETIME", d.TRANDATETIME, DbType.DateTime);
                            p.Add("@TRANDESC", d.TRANDESC, DbType.String);
                            p.Add("@TRANXSAVED", obj.TRANXSAVED, DbType.String);
                            p.Add("@UNSHAREDMSC1_AMOUNT", UNSHAREDMSC1_AMOUNT, DbType.Decimal);
                            p.Add("@UNSHAREDMSC1_RATE", unsharedmsc1_rate, DbType.Decimal);
                            p.Add("@VEND_ACCOUNT", VEND_ACCOUNT, DbType.String);
                            p.Add("@VEND_CODE", obj.VEND_CODE, DbType.String);
                            p.Add("@VEND_NAME", VEND_NAME, DbType.String);
                            p.Add("@VEND_RATE", VENDORRATE, DbType.Decimal);
                            p.Add("@VEND_VALUE", VENDORVALUE, DbType.Decimal);
                            p.Add("@AGENT_CODE", d.AGENT_CODE, DbType.String);
                            p.Add("@PAYMENTREFERENCE", d.PAYMENTREFERENCE, DbType.String);
                            p.Add("@TRANSID", d.TRANSID, DbType.String);
                            p.Add("@DR_ACCTNO", drAcct, DbType.String);
                            p.Add("@VALUEDATE", d.VALUEDATE, DbType.DateTime);
                            p.Add("@MSC2_MARGIN", msc2_margin, DbType.Decimal);
                            p.Add("@VALUEDATE2", oldValueDate, DbType.DateTime);
                            p.Add("@DR_BANKCODE", drBankCode, DbType.String);

                            p.Add("@REVENUECODE", objMsc.REVENUECODE, DbType.String);

                            p.Add("@MSC2PARTY1_BANKCODE", objMsc.MSC2PARTY1_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY1_NAME", objMsc.MSC2PARTY1_NAME, DbType.String);
                            p.Add("@MSC2PARTY1_RATE", objMsc.MSC2PARTY1_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY1_VALUE", objMsc.MSC2PARTY1_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY1_ACCOUNT", objMsc.MSC2PARTY1_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY1_ACCOUNTNAME", objMsc.MSC2PARTY1_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY2_BANKCODE", objMsc.MSC2PARTY2_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY2_NAME", objMsc.MSC2PARTY2_NAME, DbType.String);
                            p.Add("@MSC2PARTY2_RATE", objMsc.MSC2PARTY2_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY2_VALUE", objMsc.MSC2PARTY2_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY2_ACCOUNT", objMsc.MSC2PARTY2_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY2_ACCOUNTNAME", objMsc.MSC2PARTY2_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY3_BANKCODE", objMsc.MSC2PARTY3_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY3_NAME", objMsc.MSC2PARTY3_NAME, DbType.String);
                            p.Add("@MSC2PARTY3_RATE", objMsc.MSC2PARTY3_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY3_VALUE", objMsc.MSC2PARTY3_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY3_ACCOUNT", objMsc.MSC2PARTY3_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY3_ACCOUNTNAME", objMsc.MSC2PARTY3_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY4_BANKCODE", objMsc.MSC2PARTY4_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY4_NAME", objMsc.MSC2PARTY4_NAME, DbType.String);
                            p.Add("@MSC2PARTY4_RATE", objMsc.MSC2PARTY4_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY4_VALUE", objMsc.MSC2PARTY4_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY4_ACCOUNT", objMsc.MSC2PARTY4_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY4_ACCOUNTNAME", objMsc.MSC2PARTY4_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY5_BANKCODE", objMsc.MSC2PARTY5_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY5_NAME", objMsc.MSC2PARTY5_NAME, DbType.String);
                            p.Add("@MSC2PARTY5_RATE", objMsc.MSC2PARTY5_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY5_VALUE", objMsc.MSC2PARTY5_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY5_ACCOUNT", objMsc.MSC2PARTY5_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY5_ACCOUNTNAME", objMsc.MSC2PARTY5_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY6_BANKCODE", objMsc.MSC2PARTY6_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY6_NAME", objMsc.MSC2PARTY6_NAME, DbType.String);
                            p.Add("@MSC2PARTY6_RATE", objMsc.MSC2PARTY6_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY6_VALUE", objMsc.MSC2PARTY6_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY6_ACCOUNT", objMsc.MSC2PARTY6_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY6_ACCOUNTNAME", objMsc.MSC2PARTY6_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY7_BANKCODE", objMsc.MSC2PARTY7_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY7_NAME", objMsc.MSC2PARTY7_NAME, DbType.String);
                            p.Add("@MSC2PARTY7_RATE", objMsc.MSC2PARTY7_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY7_VALUE", objMsc.MSC2PARTY7_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY7_ACCOUNT", objMsc.MSC2PARTY7_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY7_ACCOUNTNAME", objMsc.MSC2PARTY7_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY8_BANKCODE", objMsc.MSC2PARTY8_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY8_NAME", objMsc.MSC2PARTY8_NAME, DbType.String);
                            p.Add("@MSC2PARTY8_RATE", objMsc.MSC2PARTY8_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY8_VALUE", objMsc.MSC2PARTY8_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY8_ACCOUNT", objMsc.MSC2PARTY8_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY8_ACCOUNTNAME", objMsc.MSC2PARTY8_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY9_BANKCODE", objMsc.MSC2PARTY9_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY9_NAME", objMsc.MSC2PARTY9_NAME, DbType.String);
                            p.Add("@MSC2PARTY9_RATE", objMsc.MSC2PARTY9_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY9_VALUE", objMsc.MSC2PARTY9_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY9_ACCOUNT", objMsc.MSC2PARTY9_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY9_ACCOUNTNAME", objMsc.MSC2PARTY9_ACCOUNTNAME, DbType.String);

                            p.Add("@MSC2PARTY10_BANKCODE", objMsc.MSC2PARTY10_BANKCODE, DbType.String);
                            p.Add("@MSC2PARTY10_NAME", objMsc.MSC2PARTY10_NAME, DbType.String);
                            p.Add("@MSC2PARTY10_RATE", objMsc.MSC2PARTY10_RATE, DbType.Decimal);
                            p.Add("@MSC2PARTY10_VALUE", objMsc.MSC2PARTY10_VALUE, DbType.Decimal);
                            p.Add("@MSC2PARTY10_ACCOUNT", objMsc.MSC2PARTY10_ACCOUNT, DbType.String);
                            p.Add("@MSC2PARTY10_ACCOUNTNAME", objMsc.MSC2PARTY10_ACCOUNTNAME, DbType.String);
                            p.Add("@SETTLEMENT_FREQUENCY", freqDesc, DbType.String);

                            p.Add("@BRANCHID", d.BRANCHID, DbType.String);
                            p.Add("@PAYMENTITEMID", d.PAYMENTITEMID, DbType.Int64);

                            procCount += con.Execute(sqlPostQuery, p, commandType: CommandType.Text);

                        }
                        catch (Exception ex)
                        {
                            lgfn.loginfoMSG(DateTime.Now.ToString() + " [SETTLEMENT POSTING ERROR] ===", ex.Message, null);

                        }
                    }
                }

                if (procCount > 0)
                {

                    try
                    {
                        //string sql = @"UPDATE TRAXMASTER_SM SET Process_status = 'S' WHERE lower(Process_status) = 'a' AND ID IN (SELECT ID FROM SM_SETTLEMENTDETAIL_MEB)";
                        string sql = "proc_set_UpdateTraxMasterProcessStatus";
                        //updatemeb.CommandType = CommandType.Text;
                        //p1 = new DynamicParameters();
                        var RST2 = con.Execute(sql, null, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                    }
                    catch (Exception ex)
                    {

                    }


                    try
                    {
                        //  updatemeb.CommandText = @"INSERT INTO POSMISDB_SETTLEMENTDETAIL SELECT * from POSMISDB_SETTLEMENTDETAIL_MEB WHERE OPDATE>='" + opdate + "' and rowid in (SELECT rid from (SELECT rowid rid,row_number() over(partition by DOCNO order by rowid) rn from POSMISDB_SETTLEMENTDETAIL_MEB WHERE OPDATE>='" + opdate + "')  where rn <> 1) AND DOCNO NOT IN (SELECT DOCNO FROM POSMISDB_SETTLEMENTDETAIL)";

                        //                     string sql = @"INSERT INTO SM_SETTLEMENTDETAIL([ID],[DOCNO],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY],[ACQUIRERFIID]
                        //   ,[ACQUIRERCURRENCY],[MASKEDPAN],[PAN],[EXPDATETIME],[SETTLEMENTDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID],[MERCHANTLOCATION]
                        //   ,[MERCHANTDEPOSITBANK],[MERCHANTDEPOSITBANKCODE],[MERCHANTDEPOSITBANKACCTNO],[MCC],[MCC_DESCRIPTION],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT]
                        //   ,[ORGINALCURRENCY],[TRANAMOUNT],[TRANCURRENCY],[SIGN],[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[ACQUIRER_REF],[SETTLEMENTCURRENCY]
                        //   ,[MSC1_RATE],[MSC1_CAP],[MSC2_RATE],[MSC2_CAP],[SHARINGMSC1_RATE],[UNSHAREDMSC1_RATE],[SUBSIDY_RATE],[MSC1_AMOUNT],[MSC2_AMOUNT]
                        //   ,[TOTALMSC_AMOUNT],[EFFECTIVEMSC],[SHARINGMSC1_AMOUNT],[UNSHAREDMSC1_AMOUNT],[MSC2_SPLITSHARING],[MSC2_SPLITSHARINGACCT],[PERC_ALLOTOTHERS],[PERC_ALLOTHOLDER]
                        //,[SUBSIDY_AMOUNT],[PERC_ALLOTMERCHANT],[TOTAL_AMOUNTDUE],[DEDUCTION_AMOUNTDUE],[NET_AMOUNTDUEMERCHANT],[IRF],[SPECIALCARD],[SPECIALCARD_OWNER],[REASONFORALTERATION]
                        //   ,[FULLREM_TRANSLATION],[TRACENO],[HOST],[ORIGID],[EXTRRN],[ORIGTYPE],[INVOICENUM],[SEQNUM],[LCYAMOUNT],[FCYAMOUNT],[EXCHANGERATE]
                        //   ,[SETTLEMENTACCOUNT],[SETTLEMENTAMOUNT],[REVERSEREQID],[SECTORCODE],[SECTOR],[TRANXSAVED],[CARDCOUNTRY],[PROCESSINGFEE],[CUSTOMERACCOUNT]
                        //   ,[PHONENO],[ACQR_CODE],[ACQR_NAME],[ACQR_RATE],[ACQR_VALUE],[ACQR_ACCOUNT],[ISSR_CODE],[ISSR_NAME],[ISSR_RATE],[ISSR_VALUE],[ISSR_ACCOUNT],[PTSP_CODE],[PTSP_NAME]
                        //   ,[PTSP_RATE],[PTSP_VALUE],[PTSP_ACCOUNT],[PTSA_CODE],[PTSA_NAME],[PTSA_RATE],[PTSA_VALUE],[PTSA_ACCOUNT],[TERW_CODE],[TERW_NAME]
                        //   ,[TERW_RATE],[TERW_VALUE],[TERW_ACCOUNT],[SWTH_CODE],[SWTH_NAME],[SWTH_RATE],[SWTH_VALUE],[SWTH_ACCOUNT],[BILR_CODE],[BILR_NAME],[BILR_RATE]
                        //   ,[BILR_VALUE],[BILR_ACCOUNT],[TECH_CODE],[TECH_NAME],[TECH_RATE],[TECH_VALUE],[TECH_ACCOUNT],[COAQ_CODE],[COAQ_NAME],[COAQ_RATE],[COAQ_VALUE],[COAQ_ACCOUNT]
                        //   ,[REVW_CODE],[REVW_NAME],[REVW_RATE],[REVW_VALUE],[REVW_ACCOUNT],[GOVT_CODE],[GOVT_NAME],[GOVT_RATE],[GOVT_VALUE],[GOVT_ACCOUNT]
                        //   ,[AGNT_CODE],[AGNT_NAME],[AGNT_RATE] ,[AGNT_VALUE],[AGNT_ACCOUNT],[VEND_CODE],[VEND_NAME],[VEND_RATE],[VEND_VALUE],[VEND_ACCOUNT]
                        //   ,[BLLR_CODE],[BLLR_NAME],[BLLR_RATE],[BLLR_VALUE],[BLLR_ACCOUNT],[SERVICEPROVIDER],[SERVICEPROVIDERBANKCODE],[SERVICEPROVIDERBANK],[SERVICEPROVIDERACCOUNTNO]
                        //   ,[BENEFICIARYNAME],[BENEFICIARYBANKCODE],[BENEFICIARYBANK],[BENEFICIARYACCOUNTNO],[INTERCHANGEFEE],[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4]
                        //   ,[SOURCEDB],[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[PROCESS_STATUS],[CREATEDATETIME],[MERCHANTNAME],[CUSTOMERID],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID],[DR_ACCTNO],[VALUEDATE]) 
                        //                           SELECT Distinct [ID],[DOCNO],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY],[ACQUIRERFIID]
                        //   ,[ACQUIRERCURRENCY],[MASKEDPAN],[PAN],[EXPDATETIME],[SETTLEMENTDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID],[MERCHANTLOCATION]
                        //   ,[MERCHANTDEPOSITBANK],[MERCHANTDEPOSITBANKCODE],[MERCHANTDEPOSITBANKACCTNO],[MCC],[MCC_DESCRIPTION],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT]
                        //   ,[ORGINALCURRENCY],[TRANAMOUNT],[TRANCURRENCY],[SIGN],[ISSUERFEE],[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[ACQUIRER_REF],[SETTLEMENTCURRENCY]
                        //   ,[MSC1_RATE],[MSC1_CAP],[MSC2_RATE],[MSC2_CAP],[SHARINGMSC1_RATE],[UNSHAREDMSC1_RATE],[SUBSIDY_RATE],[MSC1_AMOUNT],[MSC2_AMOUNT]
                        //   ,[TOTALMSC_AMOUNT],[EFFECTIVEMSC],[SHARINGMSC1_AMOUNT],[UNSHAREDMSC1_AMOUNT],[MSC2_SPLITSHARING],[MSC2_SPLITSHARINGACCT],[PERC_ALLOTOTHERS],[PERC_ALLOTHOLDER]
                        //,[SUBSIDY_AMOUNT],[PERC_ALLOTMERCHANT],[TOTAL_AMOUNTDUE],[DEDUCTION_AMOUNTDUE],[NET_AMOUNTDUEMERCHANT],[IRF],[SPECIALCARD],[SPECIALCARD_OWNER],[REASONFORALTERATION]
                        //   ,[FULLREM_TRANSLATION],[TRACENO],[HOST],[ORIGID],[EXTRRN],[ORIGTYPE],[INVOICENUM],[SEQNUM],[LCYAMOUNT],[FCYAMOUNT],[EXCHANGERATE]
                        //   ,[SETTLEMENTACCOUNT],[SETTLEMENTAMOUNT],[REVERSEREQID],[SECTORCODE],[SECTOR],[TRANXSAVED],[CARDCOUNTRY],[PROCESSINGFEE],[CUSTOMERACCOUNT]
                        //   ,[PHONENO],[ACQR_CODE],[ACQR_NAME],[ACQR_RATE],[ACQR_VALUE],[ACQR_ACCOUNT],[ISSR_CODE],[ISSR_NAME],[ISSR_RATE],[ISSR_VALUE],[ISSR_ACCOUNT],[PTSP_CODE],[PTSP_NAME]
                        //   ,[PTSP_RATE],[PTSP_VALUE],[PTSP_ACCOUNT],[PTSA_CODE],[PTSA_NAME],[PTSA_RATE],[PTSA_VALUE],[PTSA_ACCOUNT],[TERW_CODE],[TERW_NAME]
                        //   ,[TERW_RATE],[TERW_VALUE],[TERW_ACCOUNT],[SWTH_CODE],[SWTH_NAME],[SWTH_RATE],[SWTH_VALUE],[SWTH_ACCOUNT],[BILR_CODE],[BILR_NAME],[BILR_RATE]
                        //   ,[BILR_VALUE],[BILR_ACCOUNT],[TECH_CODE],[TECH_NAME],[TECH_RATE],[TECH_VALUE],[TECH_ACCOUNT],[COAQ_CODE],[COAQ_NAME],[COAQ_RATE],[COAQ_VALUE],[COAQ_ACCOUNT]
                        //   ,[REVW_CODE],[REVW_NAME],[REVW_RATE],[REVW_VALUE],[REVW_ACCOUNT],[GOVT_CODE],[GOVT_NAME],[GOVT_RATE],[GOVT_VALUE],[GOVT_ACCOUNT]
                        //   ,[AGNT_CODE],[AGNT_NAME],[AGNT_RATE] ,[AGNT_VALUE],[AGNT_ACCOUNT],[VEND_CODE],[VEND_NAME],[VEND_RATE],[VEND_VALUE],[VEND_ACCOUNT]
                        //   ,[BLLR_CODE],[BLLR_NAME],[BLLR_RATE],[BLLR_VALUE],[BLLR_ACCOUNT],[SERVICEPROVIDER],[SERVICEPROVIDERBANKCODE],[SERVICEPROVIDERBANK],[SERVICEPROVIDERACCOUNTNO]
                        //   ,[BENEFICIARYNAME],[BENEFICIARYBANKCODE],[BENEFICIARYBANK],[BENEFICIARYACCOUNTNO],[INTERCHANGEFEE],[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2],[SPECIALMESSAGE3],[SPECIALMESSAGE4]
                        //   ,[SOURCEDB],[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[PROCESS_STATUS],[CREATEDATETIME],[MERCHANTNAME],[CUSTOMERID],[AGENT_CODE],[PAYMENTREFERENCE],[TRANSID],[DR_ACCTNO],[VALUEDATE]
                        //                            from SM_SETTLEMENTDETAIL_MEB WHERE CAST(CREATEDATETIME AS DATE) >= CAST(@P_DATE AS DATE) AND ID NOT IN (SELECT ID FROM SM_SETTLEMENTDETAIL)";
                        var sql = "proc_set_PostToSettlementDetail";
                        p1 = new DynamicParameters();
                        p1.Add("@P_DATE", opdate, DbType.Date);
                        var rec = con.Execute(sql, p1, commandTimeout: 0, commandType: CommandType.StoredProcedure);

                    }
                    catch (Exception ex)
                    {

                    }
                }

                try
                {
                    //p1 = new DynamicParameters();
                    string sql = @"UPDATE sm_company_profile Set PROCESS_FLAG = 'C' where PROCESS_FLAG = 'P'";
                    con.Execute(sql, null, commandType: CommandType.Text);
                }
                catch (Exception ex)
                {

                }
            }

            se.Stop();
            TimeSpan ts2 = se.Elapsed;
            string eT2 = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts2.Hours, ts2.Minutes, ts2.Seconds, ts2.Milliseconds);
            // lgfn.loginfoMSG("Oracle Client Insertion Completed:" + eT2, null, DateTime.Now.ToString());

            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] INSERTION COMPLETED " + eT2, null, null);
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [POS PROCESS] TOTAL RECORD PROCESSED" + procCount, null, null);
            se.Stop();
            eT = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            lgfn.loginfoMSG("Complete Processing data for settlement" + eT, " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
        }

        public DateTime getCustomSettlementDate(DateTime tranDate, int setDays, string option)
        {
            DateTime setDate;
            if (option == "W")
            {
                var currentDay = (int)tranDate.DayOfWeek;
                int daysAdded = 0;
                if (currentDay != 0)
                {
                    daysAdded = (7 - currentDay) + setDays;
                }
                else
                {
                    daysAdded = setDays;
                }

                setDate = tranDate.AddDays(daysAdded);
            }
            else
            {
                var currentDay = tranDate.Day;
                var daysInMonth = DateTime.DaysInMonth(tranDate.Year, tranDate.Month);
                setDate = tranDate.AddDays((daysInMonth - currentDay));
                var daysAdded = 0;
                for (int i = 1; i <= daysInMonth; i++)
                {
                    //if(WeekendList SETTLEMENTDATE.DayOfWeek)
                    if (daysAdded == setDays)
                    {
                        break;
                    }
                    var hj = GetWeekendList();
                    setDate = setDate.AddDays(1);
                    if (hj.Contains(setDate.DayOfWeek))
                    {
                        continue;
                    }
                    daysAdded++;
                    //sett
                }
            }

            return setDate;
        }

      
        public void LogErrorMessage(long id, string mid, string tid, DateTime? opdate, string error_msg, DateTime? createDate, DateTime? tranDate, string payRef, decimal? tranAmt, DbConnection con)
        {
            //string SqlStringB = @"insert into logdata(ID,MERCHANTID,TERMINALID,OPDATE,ERROR_MESSAGE,CREATEDATE) 
            //                                                values (@ID,@MID,@TID,@OPDATE,@ERRORMSG,@CREATEDATE)";
            string SqlStringB = "proc_set_PostLogData";
            try
            {
                var p = new DynamicParameters();
                p.Add("ID", id, DbType.Int64);
                p.Add("MID", mid, DbType.String);
                p.Add("TID", tid, DbType.String);
                p.Add("ERRORMSG", error_msg, DbType.String);
                p.Add("OPDATE", opdate, DbType.DateTime);
                p.Add("CREATEDATE", createDate, DbType.DateTime);
                p.Add("TRANDATETIME", tranDate, DbType.String);
                p.Add("PAYMENTREFERENCE", payRef, DbType.String);
                p.Add("TRANAMOUNT", tranAmt, DbType.Decimal);
                con.Execute(SqlStringB, p, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                if (!ex.Message.ToString().Contains(res2))
                {

                    lgfn.loginfoMSG(ex.Message, null, DateTime.Now.ToString());


                }


            }
        }

        private PTerminalObj GetTerminalDetail2(string merchantid, string TerminAccount, DbConnection con)
        {
            try
            {
                //var dr = default(OracleDataReader);
                PTerminalObj mdt = null;
                string SETTLEMENTCUR = string.Empty;
                string qry = string.Format(@"select DEPOSIT_COUNTRYCODE from posmisdb_merchantacct where merchantid='{0}' and DEPOSIT_ACCOUNTNO='{1}'
                              ", merchantid, TerminAccount);
                //OracleCommand cmd = new OracleCommand();
                //if (Standby_connection == null)
                //{
                //    Standby_connection = new OracleConnection(oradb);
                //}
                //if (Standby_connection.State != ConnectionState.Open)
                //{
                //    Standby_connection.Open();
                //}
                //cmd.Connection = Standby_connection;
                //cmd.CommandText = qry;

                //cmd.CommandType = CommandType.Text;
                // Standby_connection.Open();
                var p = new DynamicParameters();
                var rst = con.Query<string>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                mdt = new PTerminalObj()
                {

                    SETTLEMENT_CURRENCY = rst,// dr.GetString(12),



                };
                //using (var dr = cmd.ExecuteReader())
                //{
                //    if (dr.HasRows)
                //    {
                //        while (dr.Read())
                //        {

                //            mdt = new PTerminalObj()
                //            {

                //                SETTLEMENT_CURRENCY = dr[0] != null ? dr[0].ToString() : "",// dr.GetString(12),



                //            };
                //        }
                //    }


                //}
                SETTLEMENTCUR = mdt.SETTLEMENT_CURRENCY;

                if (SETTLEMENTCUR == "566")
                {
                    return null;
                }
                else
                {
                    string qry2 = string.Format(@"select  DEPOSIT_ACCOUNTNO from posmisdb_merchantacct where merchantid='{0}' and DEPOSIT_COUNTRYCODE'566' and rownum<2
                              ", merchantid);
                    var rst2 = con.Query<string>(qry2, p, commandType: CommandType.Text).FirstOrDefault();

                    mdt = new PTerminalObj()
                    {

                        DEPOSIT_ACCOUNTNO = rst2// dr.GetString(12),

                    };

                    //using (var dr2 = cmd.ExecuteReader())
                    //{
                    //    if (dr2.HasRows)
                    //    {
                    //        while (dr2.Read())
                    //        {

                    //            mdt = new PTerminalObj()
                    //            {

                    //                DEPOSIT_ACCOUNTNO = dr2[0] != null ? dr2[0].ToString() : "",// dr.GetString(12),

                    //            };
                    //        }
                    //    }


                    // }

                }

                //cmd.Dispose();

                return mdt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DateTime? GetDate(object v)
        {
            try
            {
                var dtString = "";
                if (v != null)
                {

                    dtString = v.ToString().Trim();
                    var d = int.Parse(dtString.Substring(0, 2));
                    var m = int.Parse(dtString.Substring(2, 2));
                    var y = int.Parse(dtString.Substring(4, 4));
                    var h = int.Parse(dtString.Substring(9, 2));
                    var mi = int.Parse(dtString.Substring(12, 2));
                    var s = int.Parse(dtString.Substring(15, 2));
                    var dr = new DateTime(y, m, d, h, mi, s);
                    return dr;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

      
        private List<TRAXMASTER> processNonQuery(string opdate, DbConnection con)
        {
            DateTime opDateCast;
            DateTime.TryParse(opdate, out opDateCast);
            //OracleDataReader dr = null;
            List<TRAXMASTER> rec = null;
            //dr = default(OracleDataReader);

            Stopwatch se = new Stopwatch();
            se.Start();

            //  lgfn.loginfoMSG("Fecthing data for settlement process Started FROM TLA", " ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            lgfn.loginfoMSG(DateTime.Now.ToString() + " [SETTLEMENT PROCESS] INFO INITIALIZING PICKING FROM TLA-MEB MAPPING", null, null);



            //string SqlString = @"UPDATE POSMISDB_MEB SET ISSCOUNTRY = '566' WHERE SUBSTR(PAN, 1,6) IN (SELECT DISTINCT BIN FROM POSMISDB_TRAWDATABIN WHERE COUNTRYCODE = '566') AND ISSCOUNTRY <> '566' AND ACQCOUNTRY = '566'";
            var p = new DynamicParameters();
            p.Add("P_DATE", opDateCast, DbType.Date);
            //         string qry = @"SELECT [ITBID],[ID],[SYSDATE],[REFNO],[CHANNELID],[CARDSCHEME],[TRANCODE],[TRANDESC],[TRANDATETIME],[ISSUERFIID],[ISSUERCURRENCY]
            //   ,[ACQUIRERFIID],[ACQUIRERCURRENCY],[PAN],[EXPDATETIME],[APPROVALCODE],[APPROVALMESSAGE],[TERMINALID],[MERCHANTID],[MERCHANTLOCATION]
            //   ,[MCC],[STAN],[INVOICENO],[CUSTOMERNAME],[ORGINALAMOUNT],[ORGINALCURRENCY],[TRANAMOUNT],[TRANCURRENCY],[SIGN],[ISSUERFEE]
            //   ,[ISSUERFEECURRENCY],[ACQUIRERFEE],[ACQUIRERFEECURRENCY],[INTERCHANGEFEE],[INTERCHANGEFEECUR],[SPECIALMESSAGE1],[SPECIALMESSAGE2]
            //   ,[SPECIALMESSAGE3],[SPECIALMESSAGE4],[SOURCEDB],[SOURCETABLE],[REMARK],[RECORDID],[BATCHNO],[CREATEDATETIME],[PROCESS_STATUS],[AGENT_CODE],[PAYMENTREFERENCE]
            //   ,[TRANSID],[VALUEDATE] FROM TRAXMASTER_SM
            //WHERE PROCESS_STATUS = 'A' 
            //AND REFNO NOT IN (SELECT REFNO FROM SM_settlementdetail_meb)
            //   AND CREATEDATETIME >= @P_DATE /* and meRchantid = '7'*/";
            string qry = "proc_set_GetTraxMasterForProcess_ECSH";
            try
            {

                rec = con.Query<TRAXMASTER>(qry, p, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception ex)
            {
                //lgfn.loginfoMSG(DateTime.Now.ToString() + " [SETTLEMENT PROCESS] INFO COMPLETE PICKING FROM TLA-MEB MAPPING", null, null);

            }
            se.Stop();
            TimeSpan ts = se.Elapsed;
            string eT = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            lgfn.loginfoMSG(DateTime.Now.ToString() + " [SETTLEMENT PROCESS] INFO COMPLETE PICKING FROM TLA-MEB MAPPING", null, null);

            return rec;
            //lgfn.loginfoMSG(DateTime.Now.ToString() + " [SETTLEMENT PROCESS] ERROR " + ex.Message, null, null);

        }
       
        private PMerchantObj GetMerchantDetail(string merchantid, DbConnection con)
        {
            // var dr = default(OracleDataReader);
            try
            {
                string qry = @"select A.merchantid,merchantname,address,country_code,A.mcc_code,mcc_desc,B.sector_code,sector_name sector, customerid, institution_cbncode,
		FREQ_CODE, FREQUENCY_DESC,CUSTOM,SETTLEMENT_FREQUENCY,SET_DATE_TERM,SET_DAYS
										from(
										select merchantid, merchantname, address, country_code, mcc_code, customerid, institution_cbncode ,SETTLEMENT_FREQUENCY
										,SET_DATE_TERM,SET_DAYS
										from sm_merchantdetail where
									  lower(status) = 'active' and merchantid = @P_MID) A
									  left outer join
									  (select mcc_code, mcc_desc, sector_code from sm_mcc) B
									  ON A.mcc_code = B.mcc_code
									  left outer join
									  (select sector_code, sector_name from sm_sector) C
									  ON B.sector_code = C.sector_code
									  left outer join
									  (select FREQ_CODE, FREQUENCY_DESC,CUSTOM,ITBID from SM_FREQUENCY) D
									  ON A.SETTLEMENT_FREQUENCY = D.ITBID 
                              ";
                var p = new DynamicParameters();
                p.Add("@P_MID", merchantid, DbType.String);
                var rec = con.Query<PMerchantObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                //using (var dr = cmd.ExecuteReader())
                //{
                //    if (dr.HasRows)
                //    {
                //        while (dr.Read())
                //        {
                //            mdt = new PMerchantObj()
                //            {
                //                merchantid = dr[0] != null ? dr[0].ToString() : "",
                //                merchantname = dr[1] != null ? dr[1].ToString() : "",
                //                address = dr[2] != null ? dr[2].ToString() : "",
                //                country_code = dr[3] != null ? dr[3].ToString() : "",
                //                mcc_code = dr[4] != null ? dr[4].ToString() : "",
                //                mcc_desc = dr[5] != null ? dr[5].ToString() : "",
                //                sector_code = dr[6] != null ? dr[6].ToString() : "",
                //                sector = dr[7] != null ? dr[7].ToString() : "",
                //                customerid = ToDecimalNullable(dr[8]),// dr[8] != null ? dr.GetDecimal(8) : (decimal?)null,
                //                institution_cbncode = dr[9] != null ? dr[9].ToString() : "",
                //            };
                //        }

                //}

                return rec;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private PMerTermObj GetMerchantTerminal(string merchantid, DbConnection con)
        {
            try
            {
                string qry = @"select A.MerchantId,TerminalId from
                               (select top(1) * from SM_MERCHANTDETAIL
                               where OLD_MID = @P_MID) A
                               left outer join
                               (select TERMINALID,MERCHANTID from SM_TERMINAL) B
                               ON A.MERCHANTID = B.MERCHANTID";
                var p = new DynamicParameters();
                p.Add("@P_MID", merchantid, DbType.String);
                var rec = con.Query<PMerTermObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                return rec;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private PTerminalObj GetTerminalDetail(string terminalid, string tranCur, DbConnection con)
        {
            try
            {
                //var dr = default(OracleDataReader);
                //PTerminalObj mdt = null;
                string qry = string.Format(@"Select DISTINCT TERMINALID,A.MERCHANTID,SETTLEMENT_CURRENCY,SETTLEMENT_FREQUENCY,ACCOUNT_ID,TERMINALOWNER_CODE,Termownername,PTSP,isnull(ptspname,PTSP) ptspname,
                        PTSA,'XP' SWTCH,
                        DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME,
						Workdays,FREQUENCY_DESC,PTSPbank,PTSPacct,'XPRESS PAYMENTS LTD' SWTCHNAME,PTSANAME,PTSABANK,PTSAACCT
                        FROM
                        (select TERMINALID,MERCHANTID,SETTLEMENT_CURRENCY,SETTLEMENT_FREQUENCY,ACCOUNT_ID,TERMINALOWNER_CODE,PTSP
						 ,PTSA from sm_terminal T where 
                        terminalid=@P_TID) A
                        left outer join
                        (select itbid,merchantid,DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME 
						from sm_merchantacct) B
                        ON A.ACCOUNT_ID=B.itbid
                        left outer join
                        (select workdays,itbid,FREQUENCY_DESC from sm_frequency) C
                        ON A.SETTLEMENT_FREQUENCY=C.ITBID
                        left outer join
                        (select PARTY_CODE code,party_Desc Termownername from sm_party) D
                        ON A.TERMINALOWNER_CODE=D.code
                        left outer join
                        (
						--select cbn_code shortcode,institution_name ptspname,NULL PTSPbank,NULL PTSPacct from sm_institution
                        --union all
                        select TOP(1) party_shortname shortcode,party_Desc ptspname,DEPOSIT_BANKNAME PTSPbank,
						DEPOSIT_ACCOUNTNO PTSPacct 
						from sm_party n,sm_partyaccount mm 
						where partytype_code='PTSP' and n.itbid=mm.PARTY_ITBID) E
                        ON A.PTSP=E.shortcode
						  left outer join
                        (
						--select cbn_code shortcode,institution_name ptspname,NULL PTSPbank,NULL PTSPacct from sm_institution
                        --union all
                        select TOP(1) party_shortname shortcode,party_Desc PTSANAME,DEPOSIT_BANKNAME PTSABANK,
						DEPOSIT_ACCOUNTNO PTSAACCT 
						from sm_party n,sm_partyaccount mm 
						where partytype_code='PTSA' and n.itbid=mm.PARTY_ITBID) F
                        ON A.PTSA=F.shortcode", terminalid, tranCur);
                var p = new DynamicParameters();
                p.Add("@P_TID", terminalid, DbType.String);
                var rec = con.Query<PTerminalObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                //using (var dr = cmd.ExecuteReader())
                //{
                //    if (dr.HasRows)
                //    {
                //        while (dr.Read())
                //        {
                //            mdt = new PTerminalObj()
                //            {
                //                TERMINALID = dr[0] != null ? dr[0].ToString() : "",  // dr.GetString(0),
                //                MERCHANTID = dr[1] != null ? dr[1].ToString() : "", // dr.GetString(1),
                //                SETTLEMENT_CURRENCY = dr[2] != null ? dr[2].ToString() : "", // dr.GetString(2),
                //                SETTLEMENT_FREQUENCY = dr[3] != null ? int.Parse(dr[3].ToString()) : (int?)null,
                //                ACCOUNT_ID = dr[4] != null ? long.Parse(dr[4].ToString()) : (long?)null,
                //                TERMINALOWNER_CODE = dr[5] != null ? dr[5].ToString() : "", // dr.GetString(4),
                //                Termownername = dr[6] != null ? dr[6].ToString() : "", // dr.GetString(6),
                //                PTSP = dr[7] != null ? dr[7].ToString() : "",// dr.GetString(7),
                //                ptspname = dr[8] != null ? dr[8].ToString() : "",// dr.GetString(8),
                //                PTSA = dr[9] != null ? dr[9].ToString() : "",// dr.GetString(9),
                //                SWTCH = dr[10] != null ? dr[10].ToString() : "",// dr.GetString(10),
                //                DEPOSIT_BANKCODE = dr[11] != null ? dr[11].ToString() : "",// dr.GetString(11),

                //                DEPOSIT_ACCOUNTNO = dr[12] != null ? dr[12].ToString() : "",// dr.GetString(12),
                //                DEPOSIT_BANKNAME = dr[13] != null ? dr[13].ToString() : "",// dr.GetString(13),
                //                DEPOSIT_ACCTNAME = dr[14] != null ? dr[14].ToString() : "",// dr.GetString(14),
                //                workdays = dr[15] != null ? int.Parse(dr[15].ToString()) : (int?)null,
                //                PTSPbank = dr[16] != null ? dr[16].ToString() : "",// dr.GetString(14),
                //                PTSPAcct = dr[17] != null ? dr[17].ToString() : "",// dr.GetString(14),
                //                SWTCHNAME = dr[18] != null ? dr[18].ToString() : "",// dr.GetString(14),


                //            };
                //        }
                //    }


                //    cmd.Dispose();

                return rec;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private PTerminalObj GetTerminalDetailFCY(string terminalid, string SettCur, DbConnection con)
        {
            try
            {
                //var dr = default(OracleDataReader);
                //PTerminalObj mdt = null;
                string qry = string.Format(@"Select TERMINALID,A.MERCHANTID,ACCOUNT_ID,DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME
                        FROM
                        (select TERMINALID,MERCHANTID,SETTLEMENT_CURRENCY,SETTLEMENT_FREQUENCY,ACCOUNT_ID,TERMINALOWNER_CODE,PTSP,FCYACCOUNTID from posmisdb_terminal T where 
                        terminalid='{0}') A
                        left outer join
                        (select itbid,merchantid,DEPOSIT_BANKCODE,DEPOSIT_ACCOUNTNO,DEPOSIT_BANKNAME,DEPOSIT_ACCTNAME from posmisdb_merchantacct) B
                        ON A.FCYACCOUNTID=B.itbid
                         ", terminalid, SettCur);
                var p = new DynamicParameters();
                var rec = con.Query<PTerminalObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                //using (var dr = cmd.ExecuteReader())
                //{
                //    if (dr.HasRows)
                //    {
                //        while (dr.Read())
                //        {
                //            //var dr3 = dr[3];
                //            //var dr4 = dr[4];
                //            //var dr15 = dr[15];
                //            mdt = new PTerminalObj()
                //            {
                //                TERMINALID = dr[0] != null ? dr[0].ToString() : "",  // dr.GetString(0),
                //                MERCHANTID = dr[1] != null ? dr[1].ToString() : "", // dr.GetString(1),
                //                  ACCOUNT_ID = dr[2] != null ? long.Parse(dr[2].ToString()) : (long?)null,
                //                  DEPOSIT_BANKCODE = dr[3] != null ? dr[3].ToString() : "",// dr.GetString(11),
                //                  DEPOSIT_ACCOUNTNO = dr[4] != null ? dr[4].ToString() : "",// dr.GetString(12),
                //                DEPOSIT_BANKNAME = dr[5] != null ? dr[5].ToString() : "",// dr.GetString(13),
                //                DEPOSIT_ACCTNAME = dr[6] != null ? dr[6].ToString() : "",// dr.GetString(14),


                //            };
                //        }
                //    }


                //    cmd.Dispose();

                return rec;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private PPartyObj GetPTSA(string v_PARTY_SHORTNAME, DbConnection con)
        {
            try
            {
                //var dr = default(OracleDataReader);
                //PPartyObj mdt = null;
                string qry = string.Format(@"SELECT party_desc v_PTSANAME,DEPOSIT_BANKCODE P_BANKCODE,DEPOSIT_ACCOUNTNO v_PTSAACCTNO,DEPOSIT_BANKNAME T_PTSABANKNAME,PARTY_SHORTNAME V_PARTY_SHORTNAME FROM POSMISDB_party a,POSMISDB_PARTYACCOUNT b WHERE partytype_code='PTSA' and a.PARTY_SHORTNAME='{0}' and A.ITBID=B.PARTY_ITBID", v_PARTY_SHORTNAME);

                var p = new DynamicParameters();
                var rec = con.Query<PPartyObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                //using (var dr = cmd.ExecuteReader())
                //{
                //    if (dr.HasRows)
                //    {
                //        while (dr.Read())
                //        {
                //            //var dr3 = dr[3];
                //            //var dr4 = dr[4];
                //            //var dr15 = dr[15];
                //            mdt = new PPartyObj()
                //            {
                //                v_PTSANAME = dr[0] != null ? dr[0].ToString() : "",  // dr.GetString(0),
                //                P_BANKCODE = dr[1] != null ? dr[1].ToString() : "", // dr.GetString(1),
                //                v_PTSAACCTNO = dr[2] != null ? dr[2].ToString() : "", // dr.GetString(2),
                //                T_PTSABANKNAME = dr[3] != null ? dr[3].ToString() : "", // dr.GetString(2),
                //                V_PARTY_SHORTNAME = dr[4] != null ? dr[4].ToString() : "", // dr.GetString(2),
                //            };
                //        }
                //    }


                //    cmd.Dispose();

                return rec;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }

              private PAcquirerObj GetAcquireIssuerDetail(string acq_iss_Id, DbConnection con)
        {
            //OracleCommand cmd = new OracleCommand();
            // var  dr = default(OracleDataReader);
            try
            {
                PAcquirerObj mdt = null;
                string qry = @"SELECT distinct ISNULL(INSTITUTION_NAME,' ') INSTITUTION_NAME,ISNULL(parent_cbncode,CBN_CODE) INSTITUTIONID,ISNULL(DEPOSIT_ACCOUNTNO,'') DEPOSIT_ACCOUNTNO,CBN_CODE
                           FROM 
                         (SELECT INSTITUTION_NAME,parent_cbncode,DEPOSIT_ACCOUNTNO,CBN_CODE 
                         FROM
                         (
						 SELECT TOP(1) COALESCE(ITBID,0) ITBID,COALESCE(INSTITUTION_SHORTCODE,' ') INSTITUTION_SHORTCODE,parent_cbncode,
                          INSTITUTION_NAME,CBN_CODE FROM SM_INSTITUTION WHERE LTRIM(RTRIM(INSTITUTION_SHORTCODE))= LTRIM(RTRIM('XP')) 
                         AND LOWER(STATUS)='active') A
                         LEFT OUTER JOIN
                         (SELECT TOP(1) COALESCE(INSTITUTION_ITBID,0)INSTITUTION_ITBID,COALESCE(DEPOSIT_ACCOUNTNO,' ') DEPOSIT_ACCOUNTNO FROM SM_INSTITUTIONACCT BB,SM_INSTITUTION M  
                         WHERE LTRIM(RTRIM(DEPOSIT_BANKCODE))=M.CBN_CODE AND LOWER(BB.STATUS)='active' AND LTRIM(RTRIM(M.INSTITUTION_SHORTCODE))=LTRIM(RTRIM('XP'))) B
                         ON A.ITBID=B.INSTITUTION_ITBID
						 ) AA
       --                  UNION ALL
       --                   SELECT INSTITUTION_NAME,null INSTITUTIONID,DEPOSIT_ACCOUNTNO,null CBN_CODE
       --                  FROM
       --                  (SELECT COALESCE(ITBID,0) ITBID,COALESCE(PARTY_SHORTNAME,' ') INSTITUTION_SHORTCODE,
       --                   PARTY_DESC INSTITUTION_NAME FROM SM_party WHERE partytype_code='PTSA' AND 
       --                  LTRIM(RTRIM(PARTY_SHORTNAME)) = LTRIM(RTRIM('XP'))  AND LOWER(STATUS)='active') C
       --                  LEFT OUTER JOIN
       --                  (SELECT TOP(1) COALESCE(PARTY_ITBID,0) INSTITUTIONITBID,COALESCE(DEPOSIT_ACCOUNTNO,' ')  DEPOSIT_ACCOUNTNO FROM SM_PARTYACCOUNT 
						 --WHERE LOWER(STATUS)='active') D
       --                   ON C.ITBID=D.INSTITUTIONITBID 
                         -- UNION ALL
                         --SELECT INSTITUTION_NAME,null INSTITUTIONID,DEPOSIT_ACCOUNTNO,null CBN_CODE
                         --FROM
                         --(SELECT TOP(1) CARDSCHEME_DESC INSTITUTION_NAME,
                         --' ' DEPOSIT_ACCOUNTNO FROM   SM_CARDSCHEME WHERE  LTRIM(RTRIM(CARDSCHEME)) = LTRIM(RTRIM('{0}'))
                         --AND LOWER(STATUS)='active'
                         --)
                              ";



                var p = new DynamicParameters();
                mdt = con.Query<PAcquirerObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();
                // Standby_connection.Open();
                //using (var dr = cmd.ExecuteReader())
                //{
                //    if (dr.HasRows)
                //    {
                //        while (dr.Read())
                //        {
                //            //var dr3 = dr[3];
                //            //var dr4 = dr[4];
                //            //var dr15 = dr[15];
                //            mdt = new PAcquirerObj()
                //            {
                //                INSTITUTION_NAME = dr[0] != null ? dr[0].ToString() : "",  // dr.GetString(0),
                //                INSTITUTIONID = dr[1] != null ? dr[1].ToString() : "", // dr.GetString(1),  
                //                DEPOSIT_ACCOUNTNO = dr[2] != null ? dr[2].ToString() : "", // dr.GetString(1),  
                //                CBN_CODE = dr[3] != null ? dr[3].ToString() : "", // dr.GetString(1),                    
                //            };
                //        }

                //    }

                return mdt;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        private PAgentObj GetEcashierAgentDetail(string cbnCode, DbConnection con)
        {
            //OracleCommand cmd = new OracleCommand();
            // var  dr = default(OracleDataReader);
            try
            {
                PAgentObj mdt = null;
                string qry = @"
select CBN_CODE, AGENT_NAME, AGENT_ACCTNO from
(select  TOP(1) CBN_CODE,INSTITUTION_NAME AGENT_NAME,ITBID from sm_institution
where CBN_CODE = @P_CBN_CODE) A
left outer join
(select INSTITUTION_ITBID,DEPOSIT_ACCOUNTNO AGENT_ACCTNO from SM_INSTITUTIONACCT) B
on A.ITBID = B.INSTITUTION_ITBID";



                var p = new DynamicParameters();
                p.Add("@P_CBN_CODE", cbnCode, DbType.String);
                mdt = con.Query<PAgentObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();


                return mdt;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        private PRevenueObj GetRevenueCodeDetail(string rvCode, long? payItemId, DbConnection con)
        {
            //OracleCommand cmd = new OracleCommand();
            // var  dr = default(OracleDataReader);
            try
            {
                // payItemId = string.IsNullOrEmpty(payItemId) ? null : payItemId;
                PRevenueObj mdt = null;
                string qry = @"	select a.code revenuecode,a.description revenuedesc,b.groupcode,b.groupname,b.merchantid,b.frequency_desc,b.workdays,
                                rvh_deposit_bankcode,rvh_deposit_accountno,rvh_deposit_bankname,
                                rvg_deposit_bankcode,rvg_deposit_accountno,rvg_deposit_bankname,globalaccountflag,CUSTOM,
                                B.settlement_frequency,SET_DAYS,SET_DATE_TERM,b.freq_code from 
                                (

                                select Code,
								       rvgroupcode,
									   description,
									   account_id,
									   b.deposit_bankcode rvh_deposit_bankcode,
									   b.deposit_accountno rvh_deposit_accountno,
									   b.deposit_bankname rvh_deposit_bankname,
									   settlement_frequency
                                 from sm_revenuehead a
                                 left outer join sm_merchantacct b
                                 on a.account_id = b.itbid
                                 where code = @P_RVCODE 
								 and   (@P_PAYMENTITEMID is null or PAYMENTITEMID = @P_PAYMENTITEMID)
                                ) A
                                left outer join
                                (
                                select a.*,
									   B.frequency_desc,
									   B.workdays,
									   CUSTOM,
									   freq_code 
							    from
                                (
								select a.GroupCode,
									   a.GroupName,
									   globalaccountflag,
									   a.MerchantId,
									   b.deposit_bankcode rvg_deposit_bankcode,
									   b.deposit_accountno rvg_deposit_accountno,
                                       b.deposit_bankname rvg_deposit_bankname,
									   a.SETTLEMENT_FREQUENCY,
									   SET_DATE_TERM,SET_DAYS 
							    from sm_revenuegroup a,sm_merchantacct b
                                where a.account_id = b.itbid
								) A
                                left outer join 
                                (select itbid,
										frequency_desc,
										workdays,
										CUSTOM,
										freq_code 
										from sm_frequency
								) B
                                on A.settlement_frequency = B.ITBID
                                ) b
                                on a.rvgroupcode = b.groupcode";



                var p = new DynamicParameters();
                p.Add("@P_RVCODE", rvCode, DbType.String);
                p.Add("@P_PAYMENTITEMID", payItemId, DbType.Int64);
                mdt = con.Query<PRevenueObj>(qry, p, commandType: CommandType.Text).FirstOrDefault();


                return mdt;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        


        public GetMSCObj GetMSC(string P_MID, string P_TID, string P_TRANCURR, string P_SETT_CUR, decimal? P_TRANAMT, decimal? P_ORIGAMT,
                            string P_MCC, string P_REFNO, int P_CHANNELID, string P_CARDSCHEME,
                           string P_ACQR, string P_SIGN, string P_SPECCIALMSG1, string P_SPECCIALMSG2, string P_SPECCIALMSG3
            , string P_SPECCIALMSG4, string P_BATCHNO, string P_AGENTCODE, string P_RVCODE, DbConnection con)
        {
            try
            {
                GetMSCObj mdt = null;
                string qry = "";
                ////if (P_CARDSCHEME == "ECSH")
                ////{
                ////    qry = "PROC_SETT_GETMSC";
                ////}
                ////else
                ////{
                ////    qry = "PROC_SETT_GETMSC_POS";
                ////}

                qry = "PROC_SETT_GETMSC";
                var p = new DynamicParameters();
                p.Add("@MID", P_MID, DbType.String);
                p.Add("@TID", P_TID, DbType.String);
                p.Add("@TRACURR", P_TRANCURR, DbType.String);
                p.Add("@SETTLEMENT_CURRENCY", P_SETT_CUR, DbType.String);
                p.Add("@TRANAMOUNT", P_TRANAMT, DbType.Decimal);
                p.Add("@ORIGAMOUNT", P_ORIGAMT, DbType.Decimal);
                p.Add("@MCC", P_MCC, DbType.String);
                p.Add("@REFNO", P_REFNO, DbType.String);
                p.Add("@CHANNELID", P_CHANNELID, DbType.Int32);
                p.Add("@CARDSCHEME", P_CARDSCHEME, DbType.String);
                p.Add("@ACQR", P_ACQR, DbType.String);
                p.Add("@SIGN", P_SIGN, DbType.String);
                p.Add("@SPECIALMESSAGE1", P_SPECCIALMSG1, DbType.String);
                p.Add("@SPECIALMESSAGE2", P_SPECCIALMSG2, DbType.String);
                p.Add("@SPECIALMESSAGE3", P_SPECCIALMSG3, DbType.String);
                p.Add("@SPECIALMESSAGE4", P_SPECCIALMSG4, DbType.String);
                p.Add("@BATCHNO", P_BATCHNO, DbType.String);
                p.Add("@AGENT_CODE", P_AGENTCODE, DbType.String);
                p.Add("@RV_CODE", P_RVCODE, DbType.String);
                mdt = con.Query<GetMSCObj>(qry, p, commandType: CommandType.StoredProcedure).FirstOrDefault();

                return mdt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        

        public decimal? ToDecimalNullable(object source)
        {
            if (source == null)
            {
                return null;
            }
            decimal outNum;
            return decimal.TryParse(source.ToString(), out outNum) ? outNum : (decimal?)null;
        }

        string GetRevenueCode(string spMsg1)
        {
            var code = string.Empty;
            if (!string.IsNullOrEmpty(spMsg1) && !string.IsNullOrWhiteSpace(spMsg1))
            {
                var spltPipe = spMsg1.Split('|');
                if (spltPipe != null && spltPipe.Length > 0)
                {
                    var spltComma = spltPipe[0].Split(',');
                    if (spltComma != null && spltComma.Length > 0)
                    {
                        code = spltComma[0];
                    }
                }
            }
            return code;

        }
        List<DayOfWeek> GetWeekendList()
        {
            var lst = new List<DayOfWeek>();
            lst.Add(DayOfWeek.Saturday);
            lst.Add(DayOfWeek.Sunday);
            return lst;
        }
    }
}

