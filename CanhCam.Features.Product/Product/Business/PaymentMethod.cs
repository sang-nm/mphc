/// Author:					Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-08-18
/// Last Modified:			2015-08-18

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{
    public enum PaymentMethodProvider
    {
        ManualPayments = 0
    }

    public class PaymentMethod
    {

        #region Constructors

        public PaymentMethod()
        { }

        public PaymentMethod(int paymentMethodId)
        {
            this.GetPaymentMethod(paymentMethodId);
        }

        #endregion

        #region Private Properties

        private int paymentMethodID = -1;
        private int siteID = -1;
        private int paymentProvider = 0;
        private string name = string.Empty;
        private string description = string.Empty;
        private int displayOrder = 0;
        private bool isActive = false;
        private decimal additionalFee;
        private bool usePercentage = false;
        private bool freeOnOrdersOverXEnabled = false;
        private decimal freeOnOrdersOverXValue;
        private bool useSandbox = false;
        private string businessEmail = string.Empty;
        private string securePass = string.Empty;
        private string hashcode = string.Empty;
        private string merchantId = string.Empty;
        private string merchantSiteCode = string.Empty;
        private string accessCode = string.Empty;
        private Guid guid = Guid.Empty;
        private bool isDeleted = false;

        #endregion

        #region Public Properties

        public int PaymentMethodId
        {
            get { return paymentMethodID; }
            set { paymentMethodID = value; }
        }
        public int SiteId
        {
            get { return siteID; }
            set { siteID = value; }
        }
        public int PaymentProvider
        {
            get { return paymentProvider; }
            set { paymentProvider = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public int DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public decimal AdditionalFee
        {
            get { return additionalFee; }
            set { additionalFee = value; }
        }
        public bool UsePercentage
        {
            get { return usePercentage; }
            set { usePercentage = value; }
        }
        public bool FreeOnOrdersOverXEnabled
        {
            get { return freeOnOrdersOverXEnabled; }
            set { freeOnOrdersOverXEnabled = value; }
        }
        public decimal FreeOnOrdersOverXValue
        {
            get { return freeOnOrdersOverXValue; }
            set { freeOnOrdersOverXValue = value; }
        }
        public bool UseSandbox
        {
            get { return useSandbox; }
            set { useSandbox = value; }
        }
        public string BusinessEmail
        {
            get { return businessEmail; }
            set { businessEmail = value; }
        }
        public string SecurePass
        {
            get { return securePass; }
            set { securePass = value; }
        }
        public string Hashcode
        {
            get { return hashcode; }
            set { hashcode = value; }
        }
        public string MerchantId
        {
            get { return merchantId; }
            set { merchantId = value; }
        }
        public string MerchantSiteCode
        {
            get { return merchantSiteCode; }
            set { merchantSiteCode = value; }
        }
        public string AccessCode
        {
            get { return accessCode; }
            set { accessCode = value; }
        }
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }

        #endregion

        #region Private Methods

        private void GetPaymentMethod(int paymentMethodId)
        {
            using (IDataReader reader = DBPaymentMethod.GetOne(paymentMethodId))
            {
                if (reader.Read())
                {
                    this.paymentMethodID = Convert.ToInt32(reader["PaymentMethodID"]);
                    this.siteID = Convert.ToInt32(reader["SiteID"]);
                    this.paymentProvider = Convert.ToInt32(reader["PaymentProvider"]);
                    this.name = reader["Name"].ToString();
                    this.description = reader["Description"].ToString();
                    this.displayOrder = Convert.ToInt32(reader["DisplayOrder"]);
                    this.isActive = Convert.ToBoolean(reader["IsActive"]);
                    this.additionalFee = Convert.ToDecimal(reader["AdditionalFee"]);
                    this.usePercentage = Convert.ToBoolean(reader["UsePercentage"]);
                    this.freeOnOrdersOverXEnabled = Convert.ToBoolean(reader["FreeOnOrdersOverXEnabled"]);
                    this.freeOnOrdersOverXValue = Convert.ToDecimal(reader["FreeOnOrdersOverXValue"]);
                    this.useSandbox = Convert.ToBoolean(reader["UseSandbox"]);
                    this.businessEmail = reader["BusinessEmail"].ToString();
                    this.securePass = reader["SecurePass"].ToString();
                    this.hashcode = reader["Hashcode"].ToString();
                    this.merchantId = reader["MerchantId"].ToString();
                    this.merchantSiteCode = reader["MerchantSiteCode"].ToString();
                    this.accessCode = reader["AccessCode"].ToString();
                    this.guid = new Guid(reader["Guid"].ToString());
                    this.isDeleted = Convert.ToBoolean(reader["IsDeleted"]);
                }
            }
        }

        /// <summary>
        /// Persists a new instance of PaymentMethod. Returns true on success.
        /// </summary>
        /// <returns></returns>
        private bool Create()
        {
            int newID = 0;
            this.guid = Guid.NewGuid();

            newID = DBPaymentMethod.Create(
                this.siteID,
                this.paymentProvider,
                this.name,
                this.description,
                this.displayOrder,
                this.isActive,
                this.additionalFee,
                this.usePercentage,
                this.freeOnOrdersOverXEnabled,
                this.freeOnOrdersOverXValue,
                this.useSandbox,
                this.businessEmail,
                this.securePass,
                this.hashcode,
                this.merchantId,
                this.merchantSiteCode,
                this.accessCode,
                this.guid,
                this.isDeleted);

            this.paymentMethodID = newID;

            return (newID > 0);
        }

        /// <summary>
        /// Updates this instance of PaymentMethod. Returns true on success.
        /// </summary>
        /// <returns>bool</returns>
        private bool Update()
        {

            return DBPaymentMethod.Update(
                this.paymentMethodID,
                this.siteID,
                this.paymentProvider,
                this.name,
                this.description,
                this.displayOrder,
                this.isActive,
                this.additionalFee,
                this.usePercentage,
                this.freeOnOrdersOverXEnabled,
                this.freeOnOrdersOverXValue,
                this.useSandbox,
                this.businessEmail,
                this.securePass,
                this.hashcode,
                this.merchantId,
                this.merchantSiteCode,
                this.accessCode,
                this.guid,
                this.isDeleted);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this instance of PaymentMethod. Returns true on success.
        /// </summary>
        /// <returns>bool</returns>
        public bool Save()
        {
            if (this.paymentMethodID > 0)
            {
                return this.Update();
            }
            else
            {
                return this.Create();
            }
        }

        #endregion

        #region Static Methods

        public static bool Delete(int paymentMethodID)
        {
            return DBPaymentMethod.Delete(paymentMethodID);
        }

        private static List<PaymentMethod> LoadListFromReader(IDataReader reader)
        {
            List<PaymentMethod> paymentMethodList = new List<PaymentMethod>();
            try
            {
                while (reader.Read())
                {
                    PaymentMethod paymentMethod = new PaymentMethod();
                    paymentMethod.paymentMethodID = Convert.ToInt32(reader["PaymentMethodID"]);
                    paymentMethod.siteID = Convert.ToInt32(reader["SiteID"]);
                    paymentMethod.paymentProvider = Convert.ToInt32(reader["PaymentProvider"]);
                    paymentMethod.name = reader["Name"].ToString();
                    paymentMethod.description = reader["Description"].ToString();
                    paymentMethod.displayOrder = Convert.ToInt32(reader["DisplayOrder"]);
                    paymentMethod.isActive = Convert.ToBoolean(reader["IsActive"]);
                    paymentMethod.additionalFee = Convert.ToDecimal(reader["AdditionalFee"]);
                    paymentMethod.usePercentage = Convert.ToBoolean(reader["UsePercentage"]);
                    paymentMethod.freeOnOrdersOverXEnabled = Convert.ToBoolean(reader["FreeOnOrdersOverXEnabled"]);
                    paymentMethod.freeOnOrdersOverXValue = Convert.ToDecimal(reader["FreeOnOrdersOverXValue"]);
                    paymentMethod.useSandbox = Convert.ToBoolean(reader["UseSandbox"]);
                    paymentMethod.businessEmail = reader["BusinessEmail"].ToString();
                    paymentMethod.securePass = reader["SecurePass"].ToString();
                    paymentMethod.hashcode = reader["Hashcode"].ToString();
                    paymentMethod.merchantId = reader["MerchantId"].ToString();
                    paymentMethod.merchantSiteCode = reader["MerchantSiteCode"].ToString();
                    paymentMethod.accessCode = reader["AccessCode"].ToString();
                    paymentMethod.guid = new Guid(reader["Guid"].ToString());
                    paymentMethod.isDeleted = Convert.ToBoolean(reader["IsDeleted"]);
                    paymentMethodList.Add(paymentMethod);
                }
            }
            finally
            {
                reader.Close();
            }

            return paymentMethodList;
        }

        public static List<PaymentMethod> GetByActive(int siteId, int activeStatus)
        {
            return GetByActive(siteId, activeStatus, -1);
        }

        public static List<PaymentMethod> GetByActive(int siteId, int activeStatus, int languageId)
        {
            IDataReader reader = DBPaymentMethod.GetByActive(siteId, activeStatus, languageId);
            return LoadListFromReader(reader);
        }

        #endregion

    }

}
