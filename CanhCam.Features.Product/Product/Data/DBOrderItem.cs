
// Author:					Tran Quoc Vuong - itqvuong@gmail.com
// Created:					2014-7-2
// Last Modified:			2014-7-2

using System;
using System.Data;

namespace CanhCam.Data
{

    public static class DBOrderItem
    {

        /// <summary>
        /// Inserts a row in the gb_OrderItem table. Returns rows affected count.
        /// </summary>
        /// <param name="guid"> guid </param>
        /// <param name="orderID"> orderID </param>
        /// <param name="productID"> productID </param>
        /// <param name="quantity"> quantity </param>
        /// <param name="unitPrice"> unitPrice </param>
        /// <param name="price"> price </param>
        /// <param name="discountAmount"> discountAmount </param>
        /// <param name="originalProductCost"> originalProductCost </param>
        /// <param name="attributeDescription"> attributeDescription </param>
        /// <param name="attributesXml"> attributesXml </param>
        /// <returns>int</returns>
        public static int Create(
            Guid guid,
            int orderID,
            int productID,
            int quantity,
            decimal unitPrice,
            decimal price,
            decimal discountAmount,
            decimal originalProductCost,
            string attributeDescription,
            string attributesXml)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "gb_OrderItem_Insert", 10);
            sph.DefineSqlParameter("@Guid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, guid);
            sph.DefineSqlParameter("@OrderID", SqlDbType.Int, ParameterDirection.Input, orderID);
            sph.DefineSqlParameter("@ProductID", SqlDbType.Int, ParameterDirection.Input, productID);
            sph.DefineSqlParameter("@Quantity", SqlDbType.Int, ParameterDirection.Input, quantity);
            sph.DefineSqlParameter("@UnitPrice", SqlDbType.Decimal, ParameterDirection.Input, unitPrice);
            sph.DefineSqlParameter("@Price", SqlDbType.Decimal, ParameterDirection.Input, price);
            sph.DefineSqlParameter("@DiscountAmount", SqlDbType.Decimal, ParameterDirection.Input, discountAmount);
            sph.DefineSqlParameter("@OriginalProductCost", SqlDbType.Decimal, ParameterDirection.Input, originalProductCost);
            sph.DefineSqlParameter("@AttributeDescription", SqlDbType.NVarChar, -1, ParameterDirection.Input, attributeDescription);
            sph.DefineSqlParameter("@AttributesXml", SqlDbType.NVarChar, -1, ParameterDirection.Input, attributesXml);
            int rowsAffected = sph.ExecuteNonQuery();
            return rowsAffected;

        }


        /// <summary>
        /// Updates a row in the gb_OrderItem table. Returns true if row updated.
        /// </summary>
        /// <param name="guid"> guid </param>
        /// <param name="orderID"> orderID </param>
        /// <param name="productID"> productID </param>
        /// <param name="quantity"> quantity </param>
        /// <param name="unitPrice"> unitPrice </param>
        /// <param name="price"> price </param>
        /// <param name="discountAmount"> discountAmount </param>
        /// <param name="originalProductCost"> originalProductCost </param>
        /// <param name="attributeDescription"> attributeDescription </param>
        /// <param name="attributesXml"> attributesXml </param>
        /// <returns>bool</returns>
        public static bool Update(
            Guid guid,
            int orderID,
            int productID,
            int quantity,
            decimal unitPrice,
            decimal price,
            decimal discountAmount,
            decimal originalProductCost,
            string attributeDescription,
            string attributesXml)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "gb_OrderItem_Update", 10);
            sph.DefineSqlParameter("@Guid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, guid);
            sph.DefineSqlParameter("@OrderID", SqlDbType.Int, ParameterDirection.Input, orderID);
            sph.DefineSqlParameter("@ProductID", SqlDbType.Int, ParameterDirection.Input, productID);
            sph.DefineSqlParameter("@Quantity", SqlDbType.Int, ParameterDirection.Input, quantity);
            sph.DefineSqlParameter("@UnitPrice", SqlDbType.Decimal, ParameterDirection.Input, unitPrice);
            sph.DefineSqlParameter("@Price", SqlDbType.Decimal, ParameterDirection.Input, price);
            sph.DefineSqlParameter("@DiscountAmount", SqlDbType.Decimal, ParameterDirection.Input, discountAmount);
            sph.DefineSqlParameter("@OriginalProductCost", SqlDbType.Decimal, ParameterDirection.Input, originalProductCost);
            sph.DefineSqlParameter("@AttributeDescription", SqlDbType.NVarChar, -1, ParameterDirection.Input, attributeDescription);
            sph.DefineSqlParameter("@AttributesXml", SqlDbType.NVarChar, -1, ParameterDirection.Input, attributesXml);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);

        }

        /// <summary>
        /// Deletes a row from the gb_OrderItem table. Returns true if row deleted.
        /// </summary>
        /// <param name="guid"> guid </param>
        /// <returns>bool</returns>
        public static bool Delete(
            Guid guid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "gb_OrderItem_Delete", 1);
            sph.DefineSqlParameter("@Guid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, guid);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);

        }

        /// <summary>
        /// Gets an IDataReader with one row from the gb_OrderItem table.
        /// </summary>
        /// <param name="guid"> guid </param>
        public static IDataReader GetOne(
            Guid guid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_OrderItem_SelectOne", 1);
            sph.DefineSqlParameter("@Guid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, guid);
            return sph.ExecuteReader();

        }

        //public static int GetCount()
        //{

        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(
        //        ConnectionString.GetReadConnectionString(),
        //        CommandType.StoredProcedure,
        //        "gb_OrderItem_GetCount",
        //        null));

        //}

        //public static IDataReader GetAll()
        //{

        //    return SqlHelper.ExecuteReader(
        //        ConnectionString.GetReadConnectionString(),
        //        CommandType.StoredProcedure,
        //        "gb_OrderItem_SelectAll",
        //        null);

        //}

        //public static IDataReader GetPage(
        //    int pageNumber, 
        //    int pageSize,
        //    out int totalPages)
        //{
        //    totalPages = 1;
        //    int totalRows
        //        = GetCount();

        //    if (pageSize > 0) totalPages = totalRows / pageSize;

        //    if (totalRows <= pageSize)
        //    {
        //        totalPages = 1;
        //    }
        //    else
        //    {
        //        int remainder;
        //        Math.DivRem(totalRows, pageSize, out remainder);
        //        if (remainder > 0)
        //        {
        //            totalPages += 1;
        //        }
        //    }

        //    SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_OrderItem_SelectPage", 2);
        //    sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
        //    sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
        //    return sph.ExecuteReader();

        //}

        public static IDataReader GetByOrder(int orderId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_OrderItem_SelectByOrder", 1);
            sph.DefineSqlParameter("@OrderID", SqlDbType.Int, ParameterDirection.Input, orderId);
            return sph.ExecuteReader();
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
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_OrderItem_GetCountBySearch", 11);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
            sph.DefineSqlParameter("@StateID", SqlDbType.Int, ParameterDirection.Input, stateID);
            sph.DefineSqlParameter("@OrderStatus", SqlDbType.Int, ParameterDirection.Input, orderStatus);
            sph.DefineSqlParameter("@PaymentMethod", SqlDbType.Int, ParameterDirection.Input, paymentMethod);
            sph.DefineSqlParameter("@ShippingMethod", SqlDbType.Int, ParameterDirection.Input, shippingMethod);
			sph.DefineSqlParameter("@FromDate", SqlDbType.DateTime, ParameterDirection.Input, fromDate);
            sph.DefineSqlParameter("@ToDate", SqlDbType.DateTime, ParameterDirection.Input, toDate);
            sph.DefineSqlParameter("@FromOrderTotal", SqlDbType.Decimal, ParameterDirection.Input, fromOrderTotal);
            sph.DefineSqlParameter("@ToOrderTotal", SqlDbType.Decimal, ParameterDirection.Input, toOrderTotal);
            sph.DefineSqlParameter("@UserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, userGuid);
            sph.DefineSqlParameter("@Keyword", SqlDbType.NVarChar, 50, ParameterDirection.Input, keyword);
			return Convert.ToInt32(sph.ExecuteScalar());
		}
		
		public static IDataReader GetPageBySearch(
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
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_OrderItem_SelectPageBySearch", 13);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
            sph.DefineSqlParameter("@StateID", SqlDbType.Int, ParameterDirection.Input, stateID);
            sph.DefineSqlParameter("@OrderStatus", SqlDbType.Int, ParameterDirection.Input, orderStatus);
            sph.DefineSqlParameter("@PaymentMethod", SqlDbType.Int, ParameterDirection.Input, paymentMethod);
            sph.DefineSqlParameter("@ShippingMethod", SqlDbType.Int, ParameterDirection.Input, shippingMethod);
			sph.DefineSqlParameter("@FromDate", SqlDbType.DateTime, ParameterDirection.Input, fromDate);
            sph.DefineSqlParameter("@ToDate", SqlDbType.DateTime, ParameterDirection.Input, toDate);
            sph.DefineSqlParameter("@FromOrderTotal", SqlDbType.Decimal, ParameterDirection.Input, fromOrderTotal);
            sph.DefineSqlParameter("@ToOrderTotal", SqlDbType.Decimal, ParameterDirection.Input, toOrderTotal);
            sph.DefineSqlParameter("@UserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, userGuid);
            sph.DefineSqlParameter("@Keyword", SqlDbType.NVarChar, 50, ParameterDirection.Input, keyword);
			sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
			sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
			return sph.ExecuteReader();
		}
    }

}
