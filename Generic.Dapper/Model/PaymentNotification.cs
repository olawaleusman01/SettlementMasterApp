using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Model
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class PaymentNotificationRequest
    {

        private string serviceUrlField;

        private object serviceUsernameField;

        private object servicePasswordField;

        private string ftpUrlField;

        private object ftpUsernameField;

        private object ftpPasswordField;

        private PaymentNotificationRequestPayments paymentsField;

        /// <remarks/>
        public string ServiceUrl
        {
            get
            {
                return this.serviceUrlField;
            }
            set
            {
                this.serviceUrlField = value;
            }
        }

        /// <remarks/>
        public object ServiceUsername
        {
            get
            {
                return this.serviceUsernameField;
            }
            set
            {
                this.serviceUsernameField = value;
            }
        }

        /// <remarks/>
        public object ServicePassword
        {
            get
            {
                return this.servicePasswordField;
            }
            set
            {
                this.servicePasswordField = value;
            }
        }

        /// <remarks/>
        public string FtpUrl
        {
            get
            {
                return this.ftpUrlField;
            }
            set
            {
                this.ftpUrlField = value;
            }
        }

        /// <remarks/>
        public object FtpUsername
        {
            get
            {
                return this.ftpUsernameField;
            }
            set
            {
                this.ftpUsernameField = value;
            }
        }

        /// <remarks/>
        public object FtpPassword
        {
            get
            {
                return this.ftpPasswordField;
            }
            set
            {
                this.ftpPasswordField = value;
            }
        }

        /// <remarks/>
        public PaymentNotificationRequestPayments Payments
        {
            get
            {
                return this.paymentsField;
            }
            set
            {
                this.paymentsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PaymentNotificationRequestPayments
    {

        private PaymentNotificationRequestPaymentsPayment paymentField;

        /// <remarks/>
        public PaymentNotificationRequestPaymentsPayment Payment
        {
            get
            {
                return this.paymentField;
            }
            set
            {
                this.paymentField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PaymentNotificationRequestPaymentsPayment
    {

        private string isRepeatedField;

        private string productGroupCodeField;

        private uint paymentLogIdField;

        private ulong custReferenceField;

        private string alternateCustReferenceField;

        private decimal amountField;

        private byte paymentStatusField;

        private string paymentMethodField;

        private string paymentReferenceField;

        private object terminalIdField;

        private string channelNameField;

        private string locationField;

        private string isReversalField;

        private string paymentDateField;

        private string settlementDateField;

        private string institutionIdField;

        private string institutionNameField;

        private string branchNameField;

        private string bankNameField;

        private object feeNameField;

        private object customerNameField;

        private string otherCustomerInfoField;

        private uint receiptNoField;

        private ulong collectionsAccountField;

        private object thirdPartyCodeField;

        private PaymentNotificationRequestPaymentsPaymentPaymentItems paymentItemsField;

        private string bankCodeField;

        private object customerAddressField;

        private object customerPhoneNumberField;

        private object depositorNameField;

        private uint depositSlipNumberField;

        private ushort paymentCurrencyField;

        private object originalPaymentLogIdField;

        private object originalPaymentReferenceField;

        private string tellerField;

        /// <remarks/>
        public string IsRepeated
        {
            get
            {
                return this.isRepeatedField;
            }
            set
            {
                this.isRepeatedField = value;
            }
        }

        /// <remarks/>
        public string ProductGroupCode
        {
            get
            {
                return this.productGroupCodeField;
            }
            set
            {
                this.productGroupCodeField = value;
            }
        }

        /// <remarks/>
        public uint PaymentLogId
        {
            get
            {
                return this.paymentLogIdField;
            }
            set
            {
                this.paymentLogIdField = value;
            }
        }

        /// <remarks/>
        public ulong CustReference
        {
            get
            {
                return this.custReferenceField;
            }
            set
            {
                this.custReferenceField = value;
            }
        }

        /// <remarks/>
        public string AlternateCustReference
        {
            get
            {
                return this.alternateCustReferenceField;
            }
            set
            {
                this.alternateCustReferenceField = value;
            }
        }

        /// <remarks/>
        public decimal Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }

        /// <remarks/>
        public byte PaymentStatus
        {
            get
            {
                return this.paymentStatusField;
            }
            set
            {
                this.paymentStatusField = value;
            }
        }

        /// <remarks/>
        public string PaymentMethod
        {
            get
            {
                return this.paymentMethodField;
            }
            set
            {
                this.paymentMethodField = value;
            }
        }

        /// <remarks/>
        public string PaymentReference
        {
            get
            {
                return this.paymentReferenceField;
            }
            set
            {
                this.paymentReferenceField = value;
            }
        }

        /// <remarks/>
        public object TerminalId
        {
            get
            {
                return this.terminalIdField;
            }
            set
            {
                this.terminalIdField = value;
            }
        }

        /// <remarks/>
        public string ChannelName
        {
            get
            {
                return this.channelNameField;
            }
            set
            {
                this.channelNameField = value;
            }
        }

        /// <remarks/>
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        public string IsReversal
        {
            get
            {
                return this.isReversalField;
            }
            set
            {
                this.isReversalField = value;
            }
        }

        /// <remarks/>
        public string PaymentDate
        {
            get
            {
                return this.paymentDateField;
            }
            set
            {
                this.paymentDateField = value;
            }
        }

        /// <remarks/>
        public string SettlementDate
        {
            get
            {
                return this.settlementDateField;
            }
            set
            {
                this.settlementDateField = value;
            }
        }

        /// <remarks/>
        public string InstitutionId
        {
            get
            {
                return this.institutionIdField;
            }
            set
            {
                this.institutionIdField = value;
            }
        }

        /// <remarks/>
        public string InstitutionName
        {
            get
            {
                return this.institutionNameField;
            }
            set
            {
                this.institutionNameField = value;
            }
        }

        /// <remarks/>
        public string BranchName
        {
            get
            {
                return this.branchNameField;
            }
            set
            {
                this.branchNameField = value;
            }
        }

        /// <remarks/>
        public string BankName
        {
            get
            {
                return this.bankNameField;
            }
            set
            {
                this.bankNameField = value;
            }
        }

        /// <remarks/>
        public object FeeName
        {
            get
            {
                return this.feeNameField;
            }
            set
            {
                this.feeNameField = value;
            }
        }

        /// <remarks/>
        public object CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        public string OtherCustomerInfo
        {
            get
            {
                return this.otherCustomerInfoField;
            }
            set
            {
                this.otherCustomerInfoField = value;
            }
        }

        /// <remarks/>
        public uint ReceiptNo
        {
            get
            {
                return this.receiptNoField;
            }
            set
            {
                this.receiptNoField = value;
            }
        }

        /// <remarks/>
        public ulong CollectionsAccount
        {
            get
            {
                return this.collectionsAccountField;
            }
            set
            {
                this.collectionsAccountField = value;
            }
        }

        /// <remarks/>
        public object ThirdPartyCode
        {
            get
            {
                return this.thirdPartyCodeField;
            }
            set
            {
                this.thirdPartyCodeField = value;
            }
        }

        /// <remarks/>
        public PaymentNotificationRequestPaymentsPaymentPaymentItems PaymentItems
        {
            get
            {
                return this.paymentItemsField;
            }
            set
            {
                this.paymentItemsField = value;
            }
        }

        /// <remarks/>
        public string BankCode
        {
            get
            {
                return this.bankCodeField;
            }
            set
            {
                this.bankCodeField = value;
            }
        }

        /// <remarks/>
        public object CustomerAddress
        {
            get
            {
                return this.customerAddressField;
            }
            set
            {
                this.customerAddressField = value;
            }
        }

        /// <remarks/>
        public object CustomerPhoneNumber
        {
            get
            {
                return this.customerPhoneNumberField;
            }
            set
            {
                this.customerPhoneNumberField = value;
            }
        }

        /// <remarks/>
        public object DepositorName
        {
            get
            {
                return this.depositorNameField;
            }
            set
            {
                this.depositorNameField = value;
            }
        }

        /// <remarks/>
        public uint DepositSlipNumber
        {
            get
            {
                return this.depositSlipNumberField;
            }
            set
            {
                this.depositSlipNumberField = value;
            }
        }

        /// <remarks/>
        public ushort PaymentCurrency
        {
            get
            {
                return this.paymentCurrencyField;
            }
            set
            {
                this.paymentCurrencyField = value;
            }
        }

        /// <remarks/>
        public object OriginalPaymentLogId
        {
            get
            {
                return this.originalPaymentLogIdField;
            }
            set
            {
                this.originalPaymentLogIdField = value;
            }
        }

        /// <remarks/>
        public object OriginalPaymentReference
        {
            get
            {
                return this.originalPaymentReferenceField;
            }
            set
            {
                this.originalPaymentReferenceField = value;
            }
        }

        /// <remarks/>
        public string Teller
        {
            get
            {
                return this.tellerField;
            }
            set
            {
                this.tellerField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PaymentNotificationRequestPaymentsPaymentPaymentItems
    {

        private PaymentNotificationRequestPaymentsPaymentPaymentItemsPaymentItem paymentItemField;

        /// <remarks/>
        public PaymentNotificationRequestPaymentsPaymentPaymentItemsPaymentItem PaymentItem
        {
            get
            {
                return this.paymentItemField;
            }
            set
            {
                this.paymentItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PaymentNotificationRequestPaymentsPaymentPaymentItemsPaymentItem
    {

        private string itemNameField;

        private string itemCodeField;

        private decimal itemAmountField;

        private string leadBankCodeField;

        private byte leadBankCbnCodeField;

        private string leadBankNameField;

        private object categoryCodeField;

        private object categoryNameField;

        private byte itemQuantityField;

        /// <remarks/>
        public string ItemName
        {
            get
            {
                return this.itemNameField;
            }
            set
            {
                this.itemNameField = value;
            }
        }

        /// <remarks/>
        public string ItemCode
        {
            get
            {
                return this.itemCodeField;
            }
            set
            {
                this.itemCodeField = value;
            }
        }

        /// <remarks/>
        public decimal ItemAmount
        {
            get
            {
                return this.itemAmountField;
            }
            set
            {
                this.itemAmountField = value;
            }
        }

        /// <remarks/>
        public string LeadBankCode
        {
            get
            {
                return this.leadBankCodeField;
            }
            set
            {
                this.leadBankCodeField = value;
            }
        }

        /// <remarks/>
        public byte LeadBankCbnCode
        {
            get
            {
                return this.leadBankCbnCodeField;
            }
            set
            {
                this.leadBankCbnCodeField = value;
            }
        }

        /// <remarks/>
        public string LeadBankName
        {
            get
            {
                return this.leadBankNameField;
            }
            set
            {
                this.leadBankNameField = value;
            }
        }

        /// <remarks/>
        public object CategoryCode
        {
            get
            {
                return this.categoryCodeField;
            }
            set
            {
                this.categoryCodeField = value;
            }
        }

        /// <remarks/>
        public object CategoryName
        {
            get
            {
                return this.categoryNameField;
            }
            set
            {
                this.categoryNameField = value;
            }
        }

        /// <remarks/>
        public byte ItemQuantity
        {
            get
            {
                return this.itemQuantityField;
            }
            set
            {
                this.itemQuantityField = value;
            }
        }
    }


}
