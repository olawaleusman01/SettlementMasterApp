using System;

using System.Web.SessionState;
using Generic.Data;
using System.Web;

namespace Generic.Dapper.Utility
{
    public enum SessionKey
        {
            CART,
            MSC2BL,
        ACARule,
        MerRule,
            TEX,
            MCCMSC,
            EXCH,
        ACQScheme,
        EXCHU,
        FEE2,
        UNFEE1,
        BLMSC,
        SUBSIDYBL,
        SUBSIDYFEE1,
        BLUNMSCDOM,
        ACCOUNT,
        PROCESSOR,
        PARTYACCT,
        BILLACCT,
        SetRule,
        IntRule,
        RETURN_URL,
        MERCHANT_UPD,
        BIN,
        PARTY,
        PROD,
        MCCMSCUPLD,
        DEPT,
        //COUNTRY,
        STATE,
        ROLE,
        PARTYTYPE,
        FREQUENCY,
        CURRENCY,
        COUNTRY,
        SCHEME,
        MIDACCT,
        PRODTYPE,
        MCCRULE,
        DEL,
        MSC2DOM,
        MSC2INT,
        UNMSCDOM,
        UNMSCINT,
        SUBSIDY,
        SUBSIDYINT,
        RVHD,
        RV,
        ATMCHARGES
    }
        public static class SessionHelper
        {
            public static void Set(HttpSessionStateBase session, SessionKey key, object value)
            {
                session[Enum.GetName(typeof(SessionKey), key)] = value;
            }
            public static T Get<T>(HttpSessionStateBase session, SessionKey key)
            {
                object dataValue = session[Enum.GetName(typeof(SessionKey), key)];
                if (dataValue != null && dataValue is T)
                {
                    return (T)dataValue;
                }
                else
                {
                    return default(T);
                }
            }
            public static Merchant GetCart(HttpSessionStateBase session)
            {
            Merchant myCart = Get<Merchant>(session, SessionKey.CART);
                if (myCart == null)
                {
                    myCart = new Merchant();
                    Set(session, SessionKey.CART, myCart);
                }
                return myCart;
            }
        public static TexMess GetTexMess(HttpSessionStateBase session)
        {
            
            TexMess myCart = Get<TexMess>(session, SessionKey.TEX);
            if (myCart == null)
            {
                myCart = new TexMess();
                Set(session, SessionKey.TEX, myCart);
            }
            return myCart;
        }
        public static Institution GetInstitution(HttpSessionStateBase session)
        {
            Institution myCart = Get<Institution>(session, SessionKey.BIN);
            if (myCart == null)
            {
                myCart = new Institution();
                Set(session, SessionKey.BIN, myCart);
            }
            return myCart;
        }
        //public static PartyBin GetPartyBin(HttpSessionState session)
        //{
        //    PartyBin myCart = Get<PartyBin>(session, SessionKey.PARTY);
        //    if (myCart == null)
        //    {
        //        myCart = new PartyBin();
        //        Set(session, SessionKey.PARTY, myCart);
        //    }
        //    return myCart;
        //}
        //public static ProductUpload GetProduct(HttpSessionState session)
        //{
        //    ProductUpload myCart = Get<ProductUpload>(session, SessionKey.PROD);
        //    if (myCart == null)
        //    {
        //        myCart = new ProductUpload();
        //        Set(session, SessionKey.PROD, myCart);
        //    }
        //    return myCart;
        //}
        public static MccMsc GetMccMsc(HttpSessionStateBase session)
        {
            MccMsc myMsc = Get<MccMsc>(session, SessionKey.MCCMSC);
            if (myMsc == null)
            {
                myMsc = new MccMsc();
                Set(session, SessionKey.MCCMSC, myMsc);
            }
            return myMsc;
        }

        public static ATMCharges GetATMCharges(HttpSessionStateBase session)
        {
            ATMCharges myMsc = Get<ATMCharges>(session, SessionKey.ATMCHARGES);
            if (myMsc == null)
            {
                myMsc = new ATMCharges();
                Set(session, SessionKey.ATMCHARGES, myMsc);
            }
            return myMsc;
        }

        public static Exchange GetExchange(HttpSessionStateBase session)
        {
            Exchange myMsc = Get<Exchange>(session, SessionKey.EXCH);
            if (myMsc == null)
            {
                myMsc = new Exchange();
                Set(session, SessionKey.EXCH, myMsc);
            }
            return myMsc;
        }
        public static BankAccount GetBanks(HttpSessionStateBase session)
        {
            BankAccount myMsc = Get<BankAccount>(session, SessionKey.ACCOUNT);
            if (myMsc == null)
            {
                myMsc = new BankAccount();
                Set(session, SessionKey.ACCOUNT, myMsc);
            }
            return myMsc;
        }
        public static Processor GetProcessor(HttpSessionStateBase session)
        {
            Processor myMsc = Get<Processor>(session, SessionKey.PROCESSOR);
            if (myMsc == null)
            {
                myMsc = new Processor();
                Set(session, SessionKey.PROCESSOR, myMsc);
            }
            return myMsc;
        }
        public static PartyAccount GetPartyAcct(HttpSessionStateBase session)
        {
            PartyAccount myMsc = Get<PartyAccount>(session, SessionKey.PARTYACCT);
            if (myMsc == null)
            {
                myMsc = new PartyAccount();
                Set(session, SessionKey.PARTYACCT, myMsc);
            }
            return myMsc;
        }
        public static RvHead GetRvHead(HttpSessionStateBase session)
        {
            RvHead myMsc = Get<RvHead>(session, SessionKey.RV);
            if (myMsc == null)
            {
                myMsc = new RvHead();
                Set(session, SessionKey.RV, myMsc);
            }
            return myMsc;
        }
        public static BillerAccount GetBillerAcct(HttpSessionStateBase session)
        {
            BillerAccount myMsc = Get<BillerAccount>(session, SessionKey.BILLACCT);
            if (myMsc == null)
            {
                myMsc = new BillerAccount();
                Set(session, SessionKey.BILLACCT, myMsc);
            }
            return myMsc;
        }
        public static MerchantAccount GetMerchantAcct(HttpSessionStateBase session)
        {
            MerchantAccount myMsc = Get<MerchantAccount>(session, SessionKey.MIDACCT);
            if (myMsc == null)
            {
                myMsc = new MerchantAccount();
                Set(session, SessionKey.MIDACCT, myMsc);
            }
            return myMsc;
        }
        public static SettlementRule GetSettlementRule(HttpSessionStateBase session)
        {
            SettlementRule myMsc = Get<SettlementRule>(session, SessionKey.SetRule);
            if (myMsc == null)
            {
                myMsc = new SettlementRule();
                Set(session, SessionKey.SetRule, myMsc);
            }
            return myMsc;
        }
        public static InstitutionRule GetInstitutionRule(HttpSessionStateBase session)
        {
            InstitutionRule myMsc = Get<InstitutionRule>(session, SessionKey.IntRule);
            if (myMsc == null)
            {
                myMsc = new InstitutionRule();
                Set(session, SessionKey.IntRule, myMsc);
            }
            return myMsc;
        }
        public static AcquirerScheme GetAcquireScheme (HttpSessionStateBase session)
        {
            AcquirerScheme myMsc = Get<AcquirerScheme>(session, SessionKey.ACQScheme);
            if (myMsc == null)
            {
                myMsc = new AcquirerScheme();
                Set(session, SessionKey.ACQScheme, myMsc);
            }
            return myMsc;
        }
        public static ACARule GetACARule(HttpSessionStateBase session)
        {
            ACARule myMsc = Get<ACARule>(session, SessionKey.ACARule);
            if (myMsc == null)
            {
                myMsc = new ACARule();
                Set(session, SessionKey.ACARule, myMsc);
            }
            return myMsc;
        }
        public static MerchantRule GetMerchantRule(HttpSessionStateBase session)
        {
            MerchantRule myMsc = Get<MerchantRule>(session, SessionKey.MerRule);
            if (myMsc == null)
            {
                myMsc = new MerchantRule();
                Set(session, SessionKey.MerRule, myMsc);
            }
            return myMsc;
        }
        public static SharingMSC2PartyDom GetMerchantMSC2DomSharingParty(HttpSessionStateBase session)
        {
            SharingMSC2PartyDom myMsc = Get<SharingMSC2PartyDom>(session, SessionKey.MSC2DOM);
            if (myMsc == null)
            {
                myMsc = new SharingMSC2PartyDom();
                Set(session, SessionKey.MSC2DOM, myMsc);
            }
            return myMsc;
        }
        public static SharingBLMSC2Party GetBillerMSC2DomSharingParty(HttpSessionStateBase session)
        {
            SharingBLMSC2Party myMsc = Get<SharingBLMSC2Party>(session, SessionKey.MSC2BL);
            if (myMsc == null)
            {
                myMsc = new SharingBLMSC2Party();
                Set(session, SessionKey.MSC2BL, myMsc);
            }
            return myMsc;
        }
        public static SharingMSC2PartyInt GetMerchantMSC2IntSharingParty(HttpSessionStateBase session)
        {
            SharingMSC2PartyInt myMsc = Get<SharingMSC2PartyInt>(session, SessionKey.MSC2INT);
            if (myMsc == null)
            {
                myMsc = new SharingMSC2PartyInt();
                Set(session, SessionKey.MSC2INT, myMsc);
            }
            return myMsc;
        }
        public static SharingFEE2PartyInt GetFEE2SharingParty(HttpSessionStateBase session)
        {
            SharingFEE2PartyInt myMsc = Get<SharingFEE2PartyInt>(session, SessionKey.FEE2);
            if (myMsc == null)
            {
                myMsc = new SharingFEE2PartyInt();
                Set(session, SessionKey.FEE2, myMsc);
            }
            return myMsc;
        }
        public static SharingUnsharedMscPartyDom GetMerchantUnsharedDomSharingParty(HttpSessionStateBase session)
        {
            SharingUnsharedMscPartyDom myMsc = Get<SharingUnsharedMscPartyDom>(session, SessionKey.UNMSCDOM);
            if (myMsc == null)
            {
                myMsc = new SharingUnsharedMscPartyDom();
                Set(session, SessionKey.UNMSCDOM, myMsc);
            }
            return myMsc;
        }
        public static SharingUnsharedBLMsc1Party GetBillerUnsharedMsC1SharingParty(HttpSessionStateBase session)
        {
            SharingUnsharedBLMsc1Party myMsc = Get<SharingUnsharedBLMsc1Party>(session, SessionKey.BLUNMSCDOM);
            if (myMsc == null)
            {
                myMsc = new SharingUnsharedBLMsc1Party();
                Set(session, SessionKey.BLUNMSCDOM, myMsc);
            }
            return myMsc;
        }
        public static SharingUnsharedMscPartyInt GetMerchantUnsharedIntSharingParty(HttpSessionStateBase session)
        {
            SharingUnsharedMscPartyInt myMsc = Get<SharingUnsharedMscPartyInt>(session, SessionKey.UNMSCINT);
            if (myMsc == null)
            {
                myMsc = new SharingUnsharedMscPartyInt();
                Set(session, SessionKey.UNMSCINT, myMsc);
            }
            return myMsc;
        }
        public static SharingUnsharedFEE1Party GetBillerUnsharedFee1SharingParty(HttpSessionStateBase session)
        {
            SharingUnsharedFEE1Party myMsc = Get<SharingUnsharedFEE1Party>(session, SessionKey.UNFEE1);
            if (myMsc == null)
            {
                myMsc = new SharingUnsharedFEE1Party();
                Set(session, SessionKey.UNFEE1, myMsc);
            }
            return myMsc;
        }
        public static SharingSubsidyPartyDom GetMerchantSubsidyDomSharingParty(HttpSessionStateBase session)
        {
            SharingSubsidyPartyDom myMsc = Get<SharingSubsidyPartyDom>(session, SessionKey.SUBSIDY);
            if (myMsc == null)
            {
                myMsc = new SharingSubsidyPartyDom();
                Set(session, SessionKey.SUBSIDY, myMsc);
            }
            return myMsc;
        }
        public static SharingSubsidyPartyMsc1 GetBillerSubsidyMsc1SharingParty(HttpSessionStateBase session)
        {
            SharingSubsidyPartyMsc1 myMsc = Get<SharingSubsidyPartyMsc1>(session, SessionKey.SUBSIDYBL);
            if (myMsc == null)
            {
                myMsc = new SharingSubsidyPartyMsc1();
                Set(session, SessionKey.SUBSIDYBL, myMsc);
            }
            return myMsc;
        }
        public static BillerMerchantMsc GetBillerMerchantMscList(HttpSessionStateBase session)
        {
            BillerMerchantMsc myMsc = Get<BillerMerchantMsc>(session, SessionKey.BLMSC);
            if (myMsc == null)
            {
                myMsc = new BillerMerchantMsc();
                Set(session, SessionKey.BLMSC, myMsc);
            }
            return myMsc;
        }
        
        public static SharingSubsidyPartyInt GetMerchantSubsidyIntSharingParty(HttpSessionStateBase session)
        {
            SharingSubsidyPartyInt myMsc = Get<SharingSubsidyPartyInt>(session, SessionKey.SUBSIDYINT);
            if (myMsc == null)
            {
                myMsc = new SharingSubsidyPartyInt();
                Set(session, SessionKey.SUBSIDYINT, myMsc);
            }
            return myMsc;
        }
        public static SharingSubsidyFEE1Party GetSubsidyFEE1SharingParty(HttpSessionStateBase session)
        {
            SharingSubsidyFEE1Party myMsc = Get<SharingSubsidyFEE1Party>(session, SessionKey.SUBSIDYFEE1);
            if (myMsc == null)
            {
                myMsc = new SharingSubsidyFEE1Party();
                Set(session, SessionKey.SUBSIDYFEE1, myMsc);
            }
            return myMsc;
        }
        public static MCCRule GetMCCRule(HttpSessionStateBase session)
        {
            MCCRule myMsc = Get<MCCRule>(session, SessionKey.MCCRULE);
            if (myMsc == null)
            {
                myMsc = new MCCRule();
                Set(session, SessionKey.MCCRULE, myMsc);
            }
            return myMsc;
        }
        public static MerchantUpdate GetMerchantUpdate(HttpSessionStateBase session)
        {
            MerchantUpdate myCart = Get<MerchantUpdate>(session, SessionKey.MERCHANT_UPD);
            if (myCart == null)
            {
                myCart = new MerchantUpdate();
                Set(session, SessionKey.MERCHANT_UPD, myCart);
            }
            return myCart;
        }
        public static MCCMSCUpload GetMCCMSCUpload(HttpSessionStateBase session)
        {
            MCCMSCUpload myCart = Get<MCCMSCUpload>(session, SessionKey.MCCMSCUPLD);
            if (myCart == null)
            {
                myCart = new MCCMSCUpload();
                Set(session, SessionKey.MCCMSCUPLD, myCart);
            }
            return myCart;
        }
        public static DepartmentUpload GetDepartmentUpload(HttpSessionStateBase session)
        {
            DepartmentUpload myCart = Get<DepartmentUpload>(session, SessionKey.DEPT);
            if (myCart == null)
            {
                myCart = new DepartmentUpload();
                Set(session, SessionKey.DEPT, myCart);
            }
            return myCart;
        }

        public static RevenueHeadUpload GetRevenueHeadUpload(HttpSessionStateBase session)
        {
            RevenueHeadUpload myCart = Get<RevenueHeadUpload>(session, SessionKey.RVHD);
            if (myCart == null)
            {
                myCart = new RevenueHeadUpload();
                Set(session, SessionKey.RVHD, myCart);
            }
            return myCart;
        }
        public static CountryUpload GetCountryUpload(HttpSessionStateBase session)
        {
            CountryUpload myCart = Get<CountryUpload>(session, SessionKey.COUNTRY);
            if (myCart == null)
            {
                myCart = new CountryUpload();
                Set(session, SessionKey.COUNTRY, myCart);
            }
            return myCart;
        }
        public static StateUpload GetStateUpload(HttpSessionStateBase session)
        {
            StateUpload myCart = Get<StateUpload>(session, SessionKey.STATE);
            if (myCart == null)
            {
                myCart = new StateUpload();
                Set(session, SessionKey.STATE, myCart);
            }
            return myCart;
        }
        public static RoleUpload GetRoleUpload(HttpSessionStateBase session)
        {
            RoleUpload myCart = Get<RoleUpload>(session, SessionKey.ROLE);
            if (myCart == null)
            {
                myCart = new RoleUpload();
                Set(session, SessionKey.ROLE, myCart);
            }
            return myCart;
        }
        public static CurrencyUpload GetCurrencyUpload(HttpSessionStateBase session)
        {
            CurrencyUpload myCart = Get<CurrencyUpload>(session, SessionKey.CURRENCY);
            if (myCart == null)
            {
                myCart = new CurrencyUpload();
                Set(session, SessionKey.CURRENCY, myCart);
            }
            return myCart;
        }

        public static MidTidDeleteUpload GetMidTidDeleteUpload(HttpSessionStateBase session)
        {
            MidTidDeleteUpload myCart = Get<MidTidDeleteUpload>(session, SessionKey.DEL);
            if (myCart == null)
            {
                myCart = new MidTidDeleteUpload();
                Set(session, SessionKey.DEL, myCart);
            }
            return myCart;
        }
        public static FrequencyUpload GetFrequencyUpload(HttpSessionStateBase session)
        {
            FrequencyUpload myCart = Get<FrequencyUpload>(session, SessionKey.FREQUENCY);
            if (myCart == null)
            {
                myCart = new FrequencyUpload();
                Set(session, SessionKey.FREQUENCY, myCart);
            }
            return myCart;
        }
        public static CardSchemeUpload GetCrdSchemeUpload(HttpSessionStateBase session)
        {
            CardSchemeUpload myCart = Get<CardSchemeUpload>(session, SessionKey.SCHEME);
            if (myCart == null)
            {
                myCart = new CardSchemeUpload();
                Set(session, SessionKey.SCHEME, myCart);
            }
            return myCart;
        }
        public static Exchange GetExchangeUpload(HttpSessionStateBase session)
        {
            Exchange myCart = Get<Exchange>(session, SessionKey.EXCHU);
            if (myCart == null)
            {
                myCart = new Exchange();
                Set(session, SessionKey.EXCHU, myCart);
            }
            return myCart;
        }
        public static PartyTypeUpload GetPartyTypeUpload(HttpSessionStateBase session)
        {
            PartyTypeUpload myCart = Get<PartyTypeUpload>(session, SessionKey.PARTYTYPE);
            if (myCart == null)
            {
                myCart = new PartyTypeUpload();
                Set(session, SessionKey.PARTYTYPE, myCart);
            }
            return myCart;
        }
        public static ProductTypeUpload GetProductTypeUpload(HttpSessionStateBase session)
        {
            ProductTypeUpload myCart = Get<ProductTypeUpload>(session, SessionKey.PRODTYPE);
            if (myCart == null)
            {
                myCart = new ProductTypeUpload();
                Set(session, SessionKey.PRODTYPE, myCart);
            }
            return myCart;
        }
    }
}
