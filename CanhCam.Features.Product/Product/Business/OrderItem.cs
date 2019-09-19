﻿// Author:					Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
// Created:					2014-7-2
// Last Modified:			2014-7-2

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{

    public class OrderItem
    {

        #region Constructors

        public OrderItem()
        { }


        public OrderItem(
            Guid guid)
        {
            this.GetOrderItem(
                guid);
        }

        #endregion

        #region Private Properties

        private Guid guid = Guid.Empty;
        private int orderID = -1;
        private int productID = -1;
        private int quantity = 0;
        private decimal unitPrice = decimal.Zero;
        private decimal price = decimal.Zero;
        private decimal discountAmount = decimal.Zero;
        private decimal originalProductCost = decimal.Zero;
        private string attributeDescription = string.Empty;
        private string attributesXml = string.Empty;

        private Guid productGuid = Guid.Empty;
        private Order order = null;
        #endregion

        #region Public Properties

        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public int OrderId
        {
            get { return orderID; }
            set { orderID = value; }
        }
        public int ProductId
        {
            get { return productID; }
            set { productID = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public decimal UnitPrice
        {
            get { return unitPrice; }
            set { unitPrice = value; }
        }
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        public decimal DiscountAmount
        {
            get { return discountAmount; }
            set { discountAmount = value; }
        }
        public decimal OriginalProductCost
        {
            get { return originalProductCost; }
            set { originalProductCost = value; }
        }
        public string AttributeDescription
        {
            get { return attributeDescription; }
            set { attributeDescription = value; }
        }
        public string AttributesXml
        {
            get { return attributesXml; }
            set { attributesXml = value; }
        }

        public Guid ProductGuid
        {
            get { return productGuid; }
            set { productGuid = value; }
        }
        public Order Order
        {
            get { return order; }
            set { order = value; }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets an instance of OrderItem.
        /// </summary>
        /// <param name="guid"> guid </param>
        private void GetOrderItem(
            Guid guid)
        {
            using (IDataReader reader = DBOrderItem.GetOne(
                guid))
            {
                if (reader.Read())
                {
                    this.guid = new Guid(reader["Guid"].ToString());
                    this.orderID = Convert.ToInt32(reader["OrderID"]);
                    this.productID = Convert.ToInt32(reader["ProductID"]);
                    this.quantity = Convert.ToInt32(reader["Quantity"]);
                    this.unitPrice = Convert.ToDecimal(reader["UnitPrice"]);
                    this.price = Convert.ToDecimal(reader["Price"]);
                    this.discountAmount = Convert.ToDecimal(reader["DiscountAmount"]);
                    this.originalProductCost = Convert.ToDecimal(reader["OriginalProductCost"]);
                    this.attributeDescription = reader["AttributeDescription"].ToString();
                    this.attributesXml = reader["AttributesXml"].ToString();
                }
            }

        }

        /// <summary>
        /// Persists a new instance of OrderItem. Returns true on success.
        /// </summary>
        /// <returns></returns>
        private bool Create()
        {
            this.guid = Guid.NewGuid();

            int rowsAffected = DBOrderItem.Create(
                this.guid,
                this.orderID,
                this.productID,
                this.quantity,
                this.unitPrice,
                this.price,
                this.discountAmount,
                this.originalProductCost,
                this.attributeDescription,
                this.attributesXml);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Updates this instance of OrderItem. Returns true on success.
        /// </summary>
        /// <returns>bool</returns>
        private bool Update()
        {

            return DBOrderItem.Update(
                this.guid,
                this.orderID,
                this.productID,
                this.quantity,
                this.unitPrice,
                this.price,
                this.discountAmount,
                this.originalProductCost,
                this.attributeDescription,
                this.attributesXml);

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this instance of OrderItem. Returns true on success.
        /// </summary>
        /// <returns>bool</returns>
        public bool Save()
        {
            if (this.guid != Guid.Empty)
            {
                return Update();
            }
            else
            {
                return Create();
            }
        }




        #endregion

        #region Static Methods

        public static bool Delete(Guid guid)
        {
            return DBOrderItem.Delete(guid);
        }

        private static List<OrderItem> LoadListFromReader(IDataReader reader, bool loadOrder = false)
        {
            List<OrderItem> orderItemList = new List<OrderItem>();
            try
            {
                while (reader.Read())
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.guid = new Guid(reader["Guid"].ToString());
                    orderItem.orderID = Convert.ToInt32(reader["OrderID"]);
                    orderItem.productID = Convert.ToInt32(reader["ProductID"]);
                    orderItem.quantity = Convert.ToInt32(reader["Quantity"]);
                    orderItem.unitPrice = Convert.ToDecimal(reader["UnitPrice"]);
                    orderItem.price = Convert.ToDecimal(reader["Price"]);
                    orderItem.discountAmount = Convert.ToDecimal(reader["DiscountAmount"]);
                    orderItem.originalProductCost = Convert.ToDecimal(reader["OriginalProductCost"]);
                    orderItem.attributeDescription = reader["AttributeDescription"].ToString();
                    orderItem.attributesXml = reader["AttributesXml"].ToString();


                    if (loadOrder)
                    {
                        orderItem.productGuid = new Guid(reader["ProductGuid"].ToString());
                        orderItem.order = new Order();
                        Order.GetOrderFromReader(reader, orderItem.order);
                    }


                    orderItemList.Add(orderItem);
                }
            }
            finally
            {
                reader.Close();
            }

            return orderItemList;

        }

        public static List<OrderItem> GetByOrder(int orderId)
        {
            IDataReader reader = DBOrderItem.GetByOrder(orderId);
            return LoadListFromReader(reader);
        }
        
        public static int GetCountBySearch(
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
            return DBOrderItem.GetCountBySearch(siteID, stateID, orderStatus, paymentMethod, shippingMethod, fromDate, toDate, fromOrderTotal, toOrderTotal, userGuid, keyword);
        }

        public static List<OrderItem> GetPageBySearch(
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
            IDataReader reader = DBOrderItem.GetPageBySearch(siteID, stateID, orderStatus, paymentMethod, shippingMethod, fromDate, toDate, fromOrderTotal, toOrderTotal, userGuid, keyword, pageNumber, pageSize);
            return LoadListFromReader(reader, true);
        }

        #endregion

    }

}
