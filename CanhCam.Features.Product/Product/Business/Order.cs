// Author:					Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
// Created:					2014-07-22
// Last Modified:			2014-07-22

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{
    public enum OrderStatus
    {
        New = 0,
        Processing = 1,
        Complete = 2,
        OutOfStock = 3,
        Cancelled = 4
    }

    public enum OrderPaymentMethod
    {
        Successful = 1,
        Pending = 2,
        NotSuccessful = 3
    }

    public class Order
    {
        #region Constructors

        public Order()
        { }


        public Order(
            int orderID)
        {
            this.GetOrder(
                orderID);
        }

        #endregion

        #region Private Properties

        private int orderID = -1;
        private int siteID = -1;
        private Guid orderGuid = Guid.Empty;
        private string orderCode = string.Empty;
        private decimal orderSubtotal;
        private decimal orderShipping;
        private decimal orderDiscount;
        private decimal orderTax;
        private decimal orderTotal;
        private string currencyCode = string.Empty;
        private string couponCode = string.Empty;
        private string orderNote = string.Empty;
        private string billingFirstName = string.Empty;
        private string billingLastName = string.Empty;
        private string billingEmail = string.Empty;
        private string billingAddress = string.Empty;
        private string billingPhone = string.Empty;
        private string billingMobile = string.Empty;
        private string billingFax = string.Empty;
        private string billingStreet = string.Empty;
        private string billingWard = string.Empty;
        private Guid billingDistrictGuid = Guid.Empty;
        private Guid billingProvinceGuid = Guid.Empty;
        private Guid billingCountryGuid = Guid.Empty;
        private string shippingFirstName = string.Empty;
        private string shippingLastName = string.Empty;
        private string shippingEmail = string.Empty;
        private string shippingAddress = string.Empty;
        private string shippingPhone = string.Empty;
        private string shippingMobile = string.Empty;
        private string shippingFax = string.Empty;
        private string shippingWard = string.Empty;
        private string shippingStreet = string.Empty;
        private Guid shippingDistrictGuid = Guid.Empty;
        private Guid shippingProvinceGuid = Guid.Empty;
        private Guid shippingCountryGuid = Guid.Empty;
        private int orderStatus = 0;
        private int paymentStatus = -1;
        private int shippingStatus = -1;
        private int shippingMethod = -1;
        private int paymentMethod = -1;
        private string invoiceCompanyName = string.Empty;
        private string invoiceCompanyAddress = string.Empty;
        private string invoiceCompanyTaxCode = string.Empty;
        private string customValuesXml = string.Empty;
        private int stateID = -1;
        private Guid userGuid = Guid.Empty;
        private string createdFromIP = string.Empty;
        private string createdBy = string.Empty;
        private DateTime createdUtc = DateTime.UtcNow;
        private bool isDeleted = false;
        private int userPoint = 0;
        private decimal userPointDiscount = decimal.Zero;
        
        #endregion

        #region Public Properties

        public int OrderId
        {
            get { return orderID; }
            set { orderID = value; }
        }
        public int SiteId
        {
            get { return siteID; }
            set { siteID = value; }
        }
        public Guid OrderGuid
        {
            get { return orderGuid; }
            set { orderGuid = value; }
        }
        public string OrderCode
        {
            get { return orderCode; }
            set { orderCode = value; }
        }
        public decimal OrderSubtotal
        {
            get { return orderSubtotal; }
            set { orderSubtotal = value; }
        }
        public decimal OrderShipping
        {
            get { return orderShipping; }
            set { orderShipping = value; }
        }
        public decimal OrderDiscount
        {
            get { return orderDiscount; }
            set { orderDiscount = value; }
        }
        public decimal OrderTax
        {
            get { return orderTax; }
            set { orderTax = value; }
        }
        public decimal OrderTotal
        {
            get { return orderTotal; }
            set { orderTotal = value; }
        }
        public string CurrencyCode
        {
            get { return currencyCode; }
            set { currencyCode = value; }
        }
        public string CouponCode
        {
            get { return couponCode; }
            set { couponCode = value; }
        }
        public string OrderNote
        {
            get { return orderNote; }
            set { orderNote = value; }
        }
        public string BillingFirstName
        {
            get { return billingFirstName; }
            set { billingFirstName = value; }
        }
        public string BillingLastName
        {
            get { return billingLastName; }
            set { billingLastName = value; }
        }
        public string BillingEmail
        {
            get { return billingEmail; }
            set { billingEmail = value; }
        }
        public string BillingAddress
        {
            get { return billingAddress; }
            set { billingAddress = value; }
        }
        public string BillingPhone
        {
            get { return billingPhone; }
            set { billingPhone = value; }
        }
        public string BillingMobile
        {
            get { return billingMobile; }
            set { billingMobile = value; }
        }
        public string BillingFax
        {
            get { return billingFax; }
            set { billingFax = value; }
        }
        public string BillingStreet
        {
            get { return billingStreet; }
            set { billingStreet = value; }
        }
        public string BillingWard
        {
            get { return billingWard; }
            set { billingWard = value; }
        }
        public Guid BillingDistrictGuid
        {
            get { return billingDistrictGuid; }
            set { billingDistrictGuid = value; }
        }
        public Guid BillingProvinceGuid
        {
            get { return billingProvinceGuid; }
            set { billingProvinceGuid = value; }
        }
        public Guid BillingCountryGuid
        {
            get { return billingCountryGuid; }
            set { billingCountryGuid = value; }
        }
        public string ShippingFirstName
        {
            get { return shippingFirstName; }
            set { shippingFirstName = value; }
        }
        public string ShippingLastName
        {
            get { return shippingLastName; }
            set { shippingLastName = value; }
        }
        public string ShippingEmail
        {
            get { return shippingEmail; }
            set { shippingEmail = value; }
        }
        public string ShippingAddress
        {
            get { return shippingAddress; }
            set { shippingAddress = value; }
        }
        public string ShippingPhone
        {
            get { return shippingPhone; }
            set { shippingPhone = value; }
        }
        public string ShippingMobile
        {
            get { return shippingMobile; }
            set { shippingMobile = value; }
        }
        public string ShippingFax
        {
            get { return shippingFax; }
            set { shippingFax = value; }
        }
        public string ShippingWard
        {
            get { return shippingWard; }
            set { shippingWard = value; }
        }
        public string ShippingStreet
        {
            get { return shippingStreet; }
            set { shippingStreet = value; }
        }
        public Guid ShippingDistrictGuid
        {
            get { return shippingDistrictGuid; }
            set { shippingDistrictGuid = value; }
        }
        public Guid ShippingProvinceGuid
        {
            get { return shippingProvinceGuid; }
            set { shippingProvinceGuid = value; }
        }
        public Guid ShippingCountryGuid
        {
            get { return shippingCountryGuid; }
            set { shippingCountryGuid = value; }
        }
        public int OrderStatus
        {
            get { return orderStatus; }
            set { orderStatus = value; }
        }
        public int PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value; }
        }
        public int ShippingStatus
        {
            get { return shippingStatus; }
            set { shippingStatus = value; }
        }
        public int ShippingMethod
        {
            get { return shippingMethod; }
            set { shippingMethod = value; }
        }
        public int PaymentMethod
        {
            get { return paymentMethod; }
            set { paymentMethod = value; }
        }
        public string InvoiceCompanyName
        {
            get { return invoiceCompanyName; }
            set { invoiceCompanyName = value; }
        }
        public string InvoiceCompanyAddress
        {
            get { return invoiceCompanyAddress; }
            set { invoiceCompanyAddress = value; }
        }
        public string InvoiceCompanyTaxCode
        {
            get { return invoiceCompanyTaxCode; }
            set { invoiceCompanyTaxCode = value; }
        }
        public string CustomValuesXml
        {
            get { return customValuesXml; }
            set { customValuesXml = value; }
        }
        public int StateId
        {
            get { return stateID; }
            set { stateID = value; }
        }
        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }
        public string CreatedFromIP
        {
            get { return createdFromIP; }
            set { createdFromIP = value; }
        }
        public string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }
        public DateTime CreatedUtc
        {
            get { return createdUtc; }
            set { createdUtc = value; }
        }
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }
        public int UserPoint
        {
            get { return userPoint; }
            set { userPoint = value; }
        }
        public decimal UserPointDiscount
        {
            get { return userPointDiscount; }
            set { userPointDiscount = value; }
        }

        #endregion

        #region Private Methods

        private void GetOrder(int orderID)
        {
            using (IDataReader reader = DBOrder.GetOne(orderID))
            {
                if (reader.Read())
                {
                    GetOrderFromReader(reader, this);
                }
            }

        }

        public static void GetOrderFromReader(IDataReader reader, Order order)
        {
            order.orderID = Convert.ToInt32(reader["OrderID"]);
            order.siteID = Convert.ToInt32(reader["SiteID"]);
            order.orderGuid = new Guid(reader["OrderGuid"].ToString());
            order.orderCode = reader["OrderCode"].ToString();
            order.orderSubtotal = Convert.ToDecimal(reader["OrderSubtotal"]);
            order.orderShipping = Convert.ToDecimal(reader["OrderShipping"]);
            order.orderDiscount = Convert.ToDecimal(reader["OrderDiscount"]);
            order.orderTax = Convert.ToDecimal(reader["OrderTax"]);
            order.orderTotal = Convert.ToDecimal(reader["OrderTotal"]);
            order.currencyCode = reader["CurrencyCode"].ToString();
            order.couponCode = reader["CouponCode"].ToString();
            order.orderNote = reader["OrderNote"].ToString();
            order.billingFirstName = reader["BillingFirstName"].ToString();
            order.billingLastName = reader["BillingLastName"].ToString();
            order.billingEmail = reader["BillingEmail"].ToString();
            order.billingAddress = reader["BillingAddress"].ToString();
            order.billingPhone = reader["BillingPhone"].ToString();
            order.billingMobile = reader["BillingMobile"].ToString();
            order.billingFax = reader["BillingFax"].ToString();
            order.billingStreet = reader["BillingStreet"].ToString();
            order.billingWard = reader["BillingWard"].ToString();
            order.billingDistrictGuid = new Guid(reader["BillingDistrictGuid"].ToString());
            order.billingProvinceGuid = new Guid(reader["BillingProvinceGuid"].ToString());
            order.billingCountryGuid = new Guid(reader["BillingCountryGuid"].ToString());
            order.shippingFirstName = reader["ShippingFirstName"].ToString();
            order.shippingLastName = reader["ShippingLastName"].ToString();
            order.shippingEmail = reader["ShippingEmail"].ToString();
            order.shippingAddress = reader["ShippingAddress"].ToString();
            order.shippingPhone = reader["ShippingPhone"].ToString();
            order.shippingMobile = reader["ShippingMobile"].ToString();
            order.shippingFax = reader["ShippingFax"].ToString();
            order.shippingWard = reader["ShippingWard"].ToString();
            order.shippingStreet = reader["ShippingStreet"].ToString();
            order.shippingDistrictGuid = new Guid(reader["ShippingDistrictGuid"].ToString());
            order.shippingProvinceGuid = new Guid(reader["ShippingProvinceGuid"].ToString());
            order.shippingCountryGuid = new Guid(reader["ShippingCountryGuid"].ToString());
            order.orderStatus = Convert.ToInt32(reader["OrderStatus"]);
            order.paymentStatus = Convert.ToInt32(reader["PaymentStatus"]);
            order.shippingStatus = Convert.ToInt32(reader["ShippingStatus"]);
            order.shippingMethod = Convert.ToInt32(reader["ShippingMethod"]);
            order.paymentMethod = Convert.ToInt32(reader["PaymentMethod"]);
            order.invoiceCompanyName = reader["InvoiceCompanyName"].ToString();
            order.invoiceCompanyAddress = reader["InvoiceCompanyAddress"].ToString();
            order.invoiceCompanyTaxCode = reader["InvoiceCompanyTaxCode"].ToString();
            order.customValuesXml = reader["CustomValuesXml"].ToString();
            order.stateID = Convert.ToInt32(reader["StateID"]);
            order.userGuid = new Guid(reader["UserGuid"].ToString());
            order.createdFromIP = reader["CreatedFromIP"].ToString();
            order.createdBy = reader["CreatedBy"].ToString();
            order.createdUtc = Convert.ToDateTime(reader["CreatedUtc"]);
            order.isDeleted = Convert.ToBoolean(reader["IsDeleted"]);

            if (reader["UserPoint"] != DBNull.Value)
                order.userPoint = Convert.ToInt32(reader["UserPoint"]);
            if (reader["UserPointDiscount"] != DBNull.Value)
                order.userPointDiscount = Convert.ToDecimal(reader["UserPointDiscount"]);
        }

        /// <summary>
        /// Persists a new instance of Order. Returns true on success.
        /// </summary>
        /// <returns></returns>
        private bool Create()
        {
            int newID = 0;

            newID = DBOrder.Create(
                this.siteID,
                this.orderGuid,
                this.orderCode,
                this.orderSubtotal,
                this.orderShipping,
                this.orderDiscount,
                this.orderTax,
                this.orderTotal,
                this.currencyCode,
                this.couponCode,
                this.orderNote,
                this.billingFirstName,
                this.billingLastName,
                this.billingEmail,
                this.billingAddress,
                this.billingPhone,
                this.billingMobile,
                this.billingFax,
                this.billingStreet,
                this.billingWard,
                this.billingDistrictGuid,
                this.billingProvinceGuid,
                this.billingCountryGuid,
                this.shippingFirstName,
                this.shippingLastName,
                this.shippingEmail,
                this.shippingAddress,
                this.shippingPhone,
                this.shippingMobile,
                this.shippingFax,
                this.shippingWard,
                this.shippingStreet,
                this.shippingDistrictGuid,
                this.shippingProvinceGuid,
                this.shippingCountryGuid,
                this.orderStatus,
                this.paymentStatus,
                this.shippingStatus,
                this.shippingMethod,
                this.paymentMethod,
                this.invoiceCompanyName,
                this.invoiceCompanyAddress,
                this.invoiceCompanyTaxCode,
                this.customValuesXml,
                this.stateID,
                this.userGuid,
                this.createdFromIP,
                this.createdBy,
                this.createdUtc,
                this.isDeleted,
                this.userPoint,
                this.userPointDiscount);

            this.orderID = newID;

            return (newID > 0);
        }

        private bool Update()
        {

            return DBOrder.Update(
                this.orderID,
                this.siteID,
                this.orderGuid,
                this.orderCode,
                this.orderSubtotal,
                this.orderShipping,
                this.orderDiscount,
                this.orderTax,
                this.orderTotal,
                this.currencyCode,
                this.couponCode,
                this.orderNote,
                this.billingFirstName,
                this.billingLastName,
                this.billingEmail,
                this.billingAddress,
                this.billingPhone,
                this.billingMobile,
                this.billingFax,
                this.billingStreet,
                this.billingWard,
                this.billingDistrictGuid,
                this.billingProvinceGuid,
                this.billingCountryGuid,
                this.shippingFirstName,
                this.shippingLastName,
                this.shippingEmail,
                this.shippingAddress,
                this.shippingPhone,
                this.shippingMobile,
                this.shippingFax,
                this.shippingWard,
                this.shippingStreet,
                this.shippingDistrictGuid,
                this.shippingProvinceGuid,
                this.shippingCountryGuid,
                this.orderStatus,
                this.paymentStatus,
                this.shippingStatus,
                this.shippingMethod,
                this.paymentMethod,
                this.invoiceCompanyName,
                this.invoiceCompanyAddress,
                this.invoiceCompanyTaxCode,
                this.customValuesXml,
                this.stateID,
                this.userGuid,
                this.createdFromIP,
                this.createdBy,
                this.createdUtc,
                this.isDeleted,
                this.userPoint,
                this.userPointDiscount);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this instance of Order. Returns true on success.
        /// </summary>
        /// <returns>bool</returns>
        public bool Save()
        {
            if (this.orderID > 0)
            {
                return this.Update();
            }
            else
            {
                this.orderGuid = Guid.NewGuid();
                this.createdUtc = DateTime.UtcNow; //Prevent conflict GenerateOrderCode
                return this.Create();
            }
        }

        #endregion

        #region Static Methods

        public static bool Delete(
            int orderID)
        {
            return DBOrder.Delete(
                orderID);
        }

        public static int GetCount(
            int siteID,
            int stateID,
            int orderStatus,
            int paymentMethod,
            int shippingMethod,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? fromOrderTotal,
            decimal? toOrderTotal,
            Guid? userGuid,
            string keyword)
        {
            return DBOrder.GetCount(siteID, stateID, orderStatus, paymentMethod, shippingMethod, fromDate, toDate, fromOrderTotal, toOrderTotal, userGuid, keyword);
        }

        public static int GetCountNotAdmin(
            int siteID,
            int stateID,
            int orderStatus,
            int paymentMethod,
            int shippingMethod,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? fromOrderTotal,
            decimal? toOrderTotal,
            Guid? userGuid,
            string keyword,
            bool isAdmin)
        {
            return DBOrder.GetCountNotAdmin(siteID, stateID, orderStatus, paymentMethod, shippingMethod, fromDate, toDate, fromOrderTotal, toOrderTotal, userGuid, keyword, isAdmin);
        }

        private static List<Order> LoadListFromReader(IDataReader reader)
        {
            List<Order> orderList = new List<Order>();
            try
            {
                while (reader.Read())
                {
                    Order order = new Order();
                    order.orderID = Convert.ToInt32(reader["OrderID"]);
                    order.siteID = Convert.ToInt32(reader["SiteID"]);
                    order.orderGuid = new Guid(reader["OrderGuid"].ToString());
                    order.orderCode = reader["OrderCode"].ToString();
                    order.orderSubtotal = Convert.ToDecimal(reader["OrderSubtotal"]);
                    order.orderShipping = Convert.ToDecimal(reader["OrderShipping"]);
                    order.orderDiscount = Convert.ToDecimal(reader["OrderDiscount"]);
                    order.orderTax = Convert.ToDecimal(reader["OrderTax"]);
                    order.orderTotal = Convert.ToDecimal(reader["OrderTotal"]);
                    order.currencyCode = reader["CurrencyCode"].ToString();
                    order.couponCode = reader["CouponCode"].ToString();
                    order.orderNote = reader["OrderNote"].ToString();
                    order.billingFirstName = reader["BillingFirstName"].ToString();
                    order.billingLastName = reader["BillingLastName"].ToString();
                    order.billingEmail = reader["BillingEmail"].ToString();
                    order.billingAddress = reader["BillingAddress"].ToString();
                    order.billingPhone = reader["BillingPhone"].ToString();
                    order.billingMobile = reader["BillingMobile"].ToString();
                    order.billingFax = reader["BillingFax"].ToString();
                    order.billingStreet = reader["BillingStreet"].ToString();
                    order.billingWard = reader["BillingWard"].ToString();
                    order.billingDistrictGuid = new Guid(reader["BillingDistrictGuid"].ToString());
                    order.billingProvinceGuid = new Guid(reader["BillingProvinceGuid"].ToString());
                    order.billingCountryGuid = new Guid(reader["BillingCountryGuid"].ToString());
                    order.shippingFirstName = reader["ShippingFirstName"].ToString();
                    order.shippingLastName = reader["ShippingLastName"].ToString();
                    order.shippingEmail = reader["ShippingEmail"].ToString();
                    order.shippingAddress = reader["ShippingAddress"].ToString();
                    order.shippingPhone = reader["ShippingPhone"].ToString();
                    order.shippingMobile = reader["ShippingMobile"].ToString();
                    order.shippingFax = reader["ShippingFax"].ToString();
                    order.shippingWard = reader["ShippingWard"].ToString();
                    order.shippingStreet = reader["ShippingStreet"].ToString();
                    order.shippingDistrictGuid = new Guid(reader["ShippingDistrictGuid"].ToString());
                    order.shippingProvinceGuid = new Guid(reader["ShippingProvinceGuid"].ToString());
                    order.shippingCountryGuid = new Guid(reader["ShippingCountryGuid"].ToString());
                    order.orderStatus = Convert.ToInt32(reader["OrderStatus"]);
                    order.paymentStatus = Convert.ToInt32(reader["PaymentStatus"]);
                    order.shippingStatus = Convert.ToInt32(reader["ShippingStatus"]);
                    order.shippingMethod = Convert.ToInt32(reader["ShippingMethod"]);
                    order.paymentMethod = Convert.ToInt32(reader["PaymentMethod"]);
                    order.invoiceCompanyName = reader["InvoiceCompanyName"].ToString();
                    order.invoiceCompanyAddress = reader["InvoiceCompanyAddress"].ToString();
                    order.invoiceCompanyTaxCode = reader["InvoiceCompanyTaxCode"].ToString();
                    order.customValuesXml = reader["CustomValuesXml"].ToString();
                    order.stateID = Convert.ToInt32(reader["StateID"]);
                    order.userGuid = new Guid(reader["UserGuid"].ToString());
                    order.createdFromIP = reader["CreatedFromIP"].ToString();
                    order.createdBy = reader["CreatedBy"].ToString();
                    order.createdUtc = Convert.ToDateTime(reader["CreatedUtc"]);
                    order.isDeleted = Convert.ToBoolean(reader["IsDeleted"]);

                    if (reader["UserPoint"] != DBNull.Value)
                        order.userPoint = Convert.ToInt32(reader["UserPoint"]);
                    if (reader["UserPointDiscount"] != DBNull.Value)
                        order.userPointDiscount = Convert.ToDecimal(reader["UserPointDiscount"]);

                    orderList.Add(order);
                }
            }
            finally
            {
                reader.Close();
            }

            return orderList;
        }

        public static List<Order> GetPage(
            int siteID,
            int stateID,
            int orderStatus,
            int paymentMethod,
            int shippingMethod,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? fromOrderTotal,
            decimal? toOrderTotal,
            Guid? userGuid,
            string keyword,
            int pageNumber,
            int pageSize)
        {
            IDataReader reader = DBOrder.GetPage(siteID, stateID, orderStatus, paymentMethod, shippingMethod, fromDate, toDate, fromOrderTotal, toOrderTotal, userGuid, keyword, pageNumber, pageSize);
            return LoadListFromReader(reader);
        }

        public static List<Order> GetPageNotAdmin(
            int siteID,
            int stateID,
            int orderStatus,
            int paymentMethod,
            int shippingMethod,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? fromOrderTotal,
            decimal? toOrderTotal,
            Guid? userGuid,
            string keyword,
            int pageNumber,
            int pageSize,
            bool isAdmin)
        {
            IDataReader reader = DBOrder.GetPageNotAdmin(siteID, stateID, orderStatus, paymentMethod, shippingMethod, fromDate, toDate, fromOrderTotal, toOrderTotal, userGuid, keyword, pageNumber, pageSize, isAdmin);
            return LoadListFromReader(reader);
        }

        public static Order GetOrderByCode(string orderCode)
        {
            Order order = null;
            using (IDataReader reader = DBOrder.GetOneByCode(orderCode))
            {
                if (reader.Read())
                {
                    order = new Order();
                    GetOrderFromReader(reader, order);
                }
            }

            return order;
        }

        #endregion

    }
}