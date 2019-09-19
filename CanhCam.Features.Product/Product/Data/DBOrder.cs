// Author:					Tran Quoc Vuong - itqvuong@gmail.com
// Created:					2014-07-02
// Last Modified:			2014-07-02

using System;
using System.Data;
	
namespace CanhCam.Data
{
	
	public static class DBOrder
    {

		public static int Create(
			int siteID, 
			Guid orderGuid, 
			string orderCode, 
			decimal orderSubtotal, 
			decimal orderShipping, 
			decimal orderDiscount, 
			decimal orderTax, 
			decimal orderTotal, 
			string currencyCode, 
			string couponCode, 
			string orderNote, 
			string billingFirstName, 
			string billingLastName, 
			string billingEmail, 
			string billingAddress, 
			string billingPhone, 
			string billingMobile, 
			string billingFax, 
			string billingStreet, 
			string billingWard, 
			Guid billingDistrictGuid, 
			Guid billingProvinceGuid, 
			Guid billingCountryGuid, 
			string shippingFirstName, 
			string shippingLastName, 
			string shippingEmail, 
			string shippingAddress, 
			string shippingPhone, 
			string shippingMobile, 
			string shippingFax, 
			string shippingWard, 
			string shippingStreet, 
			Guid shippingDistrictGuid, 
			Guid shippingProvinceGuid, 
			Guid shippingCountryGuid, 
			int orderStatus, 
			int paymentStatus, 
			int shippingStatus, 
			int shippingMethod, 
			int paymentMethod, 
			string invoiceCompanyName, 
			string invoiceCompanyAddress, 
			string invoiceCompanyTaxCode, 
			string customValuesXml, 
			int stateID, 
			Guid userGuid, 
			string createdFromIP, 
			string createdBy, 
			DateTime createdUtc, 
			bool isDeleted,
            int userPoint,
            decimal userPointDiscount) 
		{
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "gb_Order_Insert", 52);
			sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
			sph.DefineSqlParameter("@OrderGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, orderGuid);
			sph.DefineSqlParameter("@OrderCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, orderCode);
			sph.DefineSqlParameter("@OrderSubtotal", SqlDbType.Decimal, ParameterDirection.Input, orderSubtotal);
			sph.DefineSqlParameter("@OrderShipping", SqlDbType.Decimal, ParameterDirection.Input, orderShipping);
			sph.DefineSqlParameter("@OrderDiscount", SqlDbType.Decimal, ParameterDirection.Input, orderDiscount);
			sph.DefineSqlParameter("@OrderTax", SqlDbType.Decimal, ParameterDirection.Input, orderTax);
			sph.DefineSqlParameter("@OrderTotal", SqlDbType.Decimal, ParameterDirection.Input, orderTotal);
			sph.DefineSqlParameter("@CurrencyCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, currencyCode);
			sph.DefineSqlParameter("@CouponCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, couponCode);
			sph.DefineSqlParameter("@OrderNote", SqlDbType.NVarChar, -1, ParameterDirection.Input, orderNote);
			sph.DefineSqlParameter("@BillingFirstName", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingFirstName);
			sph.DefineSqlParameter("@BillingLastName", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingLastName);
			sph.DefineSqlParameter("@BillingEmail", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingEmail);
			sph.DefineSqlParameter("@BillingAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingAddress);
			sph.DefineSqlParameter("@BillingPhone", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingPhone);
			sph.DefineSqlParameter("@BillingMobile", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingMobile);
			sph.DefineSqlParameter("@BillingFax", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingFax);
			sph.DefineSqlParameter("@BillingStreet", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingStreet);
			sph.DefineSqlParameter("@BillingWard", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingWard);
			sph.DefineSqlParameter("@BillingDistrictGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, billingDistrictGuid);
			sph.DefineSqlParameter("@BillingProvinceGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, billingProvinceGuid);
			sph.DefineSqlParameter("@BillingCountryGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, billingCountryGuid);
			sph.DefineSqlParameter("@ShippingFirstName", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingFirstName);
			sph.DefineSqlParameter("@ShippingLastName", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingLastName);
			sph.DefineSqlParameter("@ShippingEmail", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingEmail);
			sph.DefineSqlParameter("@ShippingAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingAddress);
			sph.DefineSqlParameter("@ShippingPhone", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingPhone);
			sph.DefineSqlParameter("@ShippingMobile", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingMobile);
			sph.DefineSqlParameter("@ShippingFax", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingFax);
			sph.DefineSqlParameter("@ShippingWard", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingWard);
			sph.DefineSqlParameter("@ShippingStreet", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingStreet);
			sph.DefineSqlParameter("@ShippingDistrictGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, shippingDistrictGuid);
			sph.DefineSqlParameter("@ShippingProvinceGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, shippingProvinceGuid);
			sph.DefineSqlParameter("@ShippingCountryGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, shippingCountryGuid);
			sph.DefineSqlParameter("@OrderStatus", SqlDbType.Int, ParameterDirection.Input, orderStatus);
			sph.DefineSqlParameter("@PaymentStatus", SqlDbType.Int, ParameterDirection.Input, paymentStatus);
			sph.DefineSqlParameter("@ShippingStatus", SqlDbType.Int, ParameterDirection.Input, shippingStatus);
			sph.DefineSqlParameter("@ShippingMethod", SqlDbType.Int, ParameterDirection.Input, shippingMethod);
			sph.DefineSqlParameter("@PaymentMethod", SqlDbType.Int, ParameterDirection.Input, paymentMethod);
			sph.DefineSqlParameter("@InvoiceCompanyName", SqlDbType.NVarChar, 100, ParameterDirection.Input, invoiceCompanyName);
			sph.DefineSqlParameter("@InvoiceCompanyAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, invoiceCompanyAddress);
			sph.DefineSqlParameter("@InvoiceCompanyTaxCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, invoiceCompanyTaxCode);
			sph.DefineSqlParameter("@CustomValuesXml", SqlDbType.NVarChar, -1, ParameterDirection.Input, customValuesXml);
			sph.DefineSqlParameter("@StateID", SqlDbType.Int, ParameterDirection.Input, stateID);
			sph.DefineSqlParameter("@UserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, userGuid);
			sph.DefineSqlParameter("@CreatedFromIP", SqlDbType.NVarChar, 255, ParameterDirection.Input, createdFromIP);
			sph.DefineSqlParameter("@CreatedBy", SqlDbType.NVarChar, 255, ParameterDirection.Input, createdBy);
			sph.DefineSqlParameter("@CreatedUtc", SqlDbType.DateTime, ParameterDirection.Input, createdUtc);
			sph.DefineSqlParameter("@IsDeleted", SqlDbType.Bit, ParameterDirection.Input, isDeleted);
            sph.DefineSqlParameter("@UserPoint", SqlDbType.Int, ParameterDirection.Input, userPoint);
            sph.DefineSqlParameter("@UserPointDiscount", SqlDbType.Decimal, ParameterDirection.Input, userPointDiscount);
			int newID = Convert.ToInt32(sph.ExecuteScalar());
			return newID;
		}

		public static bool Update(
			int  orderID, 
			int siteID, 
			Guid orderGuid, 
			string orderCode, 
			decimal orderSubtotal, 
			decimal orderShipping, 
			decimal orderDiscount, 
			decimal orderTax, 
			decimal orderTotal, 
			string currencyCode, 
			string couponCode, 
			string orderNote, 
			string billingFirstName, 
			string billingLastName, 
			string billingEmail, 
			string billingAddress, 
			string billingPhone, 
			string billingMobile, 
			string billingFax, 
			string billingStreet, 
			string billingWard, 
			Guid billingDistrictGuid, 
			Guid billingProvinceGuid, 
			Guid billingCountryGuid, 
			string shippingFirstName, 
			string shippingLastName, 
			string shippingEmail, 
			string shippingAddress, 
			string shippingPhone, 
			string shippingMobile, 
			string shippingFax, 
			string shippingWard, 
			string shippingStreet, 
			Guid shippingDistrictGuid, 
			Guid shippingProvinceGuid, 
			Guid shippingCountryGuid, 
			int orderStatus, 
			int paymentStatus, 
			int shippingStatus, 
			int shippingMethod, 
			int paymentMethod, 
			string invoiceCompanyName, 
			string invoiceCompanyAddress, 
			string invoiceCompanyTaxCode, 
			string customValuesXml, 
			int stateID, 
			Guid userGuid, 
			string createdFromIP, 
			string createdBy, 
			DateTime createdUtc, 
			bool isDeleted,
            int userPoint,
            decimal userPointDiscount) 
		{
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "gb_Order_Update", 53);
			sph.DefineSqlParameter("@OrderID", SqlDbType.Int, ParameterDirection.Input, orderID);
			sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
			sph.DefineSqlParameter("@OrderGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, orderGuid);
			sph.DefineSqlParameter("@OrderCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, orderCode);
			sph.DefineSqlParameter("@OrderSubtotal", SqlDbType.Decimal, ParameterDirection.Input, orderSubtotal);
			sph.DefineSqlParameter("@OrderShipping", SqlDbType.Decimal, ParameterDirection.Input, orderShipping);
			sph.DefineSqlParameter("@OrderDiscount", SqlDbType.Decimal, ParameterDirection.Input, orderDiscount);
			sph.DefineSqlParameter("@OrderTax", SqlDbType.Decimal, ParameterDirection.Input, orderTax);
			sph.DefineSqlParameter("@OrderTotal", SqlDbType.Decimal, ParameterDirection.Input, orderTotal);
			sph.DefineSqlParameter("@CurrencyCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, currencyCode);
			sph.DefineSqlParameter("@CouponCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, couponCode);
			sph.DefineSqlParameter("@OrderNote", SqlDbType.NVarChar, -1, ParameterDirection.Input, orderNote);
			sph.DefineSqlParameter("@BillingFirstName", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingFirstName);
			sph.DefineSqlParameter("@BillingLastName", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingLastName);
			sph.DefineSqlParameter("@BillingEmail", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingEmail);
			sph.DefineSqlParameter("@BillingAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingAddress);
			sph.DefineSqlParameter("@BillingPhone", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingPhone);
			sph.DefineSqlParameter("@BillingMobile", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingMobile);
			sph.DefineSqlParameter("@BillingFax", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingFax);
			sph.DefineSqlParameter("@BillingStreet", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingStreet);
			sph.DefineSqlParameter("@BillingWard", SqlDbType.NVarChar, 255, ParameterDirection.Input, billingWard);
			sph.DefineSqlParameter("@BillingDistrictGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, billingDistrictGuid);
			sph.DefineSqlParameter("@BillingProvinceGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, billingProvinceGuid);
			sph.DefineSqlParameter("@BillingCountryGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, billingCountryGuid);
			sph.DefineSqlParameter("@ShippingFirstName", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingFirstName);
			sph.DefineSqlParameter("@ShippingLastName", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingLastName);
			sph.DefineSqlParameter("@ShippingEmail", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingEmail);
			sph.DefineSqlParameter("@ShippingAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingAddress);
			sph.DefineSqlParameter("@ShippingPhone", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingPhone);
			sph.DefineSqlParameter("@ShippingMobile", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingMobile);
			sph.DefineSqlParameter("@ShippingFax", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingFax);
			sph.DefineSqlParameter("@ShippingWard", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingWard);
			sph.DefineSqlParameter("@ShippingStreet", SqlDbType.NVarChar, 255, ParameterDirection.Input, shippingStreet);
			sph.DefineSqlParameter("@ShippingDistrictGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, shippingDistrictGuid);
			sph.DefineSqlParameter("@ShippingProvinceGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, shippingProvinceGuid);
			sph.DefineSqlParameter("@ShippingCountryGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, shippingCountryGuid);
			sph.DefineSqlParameter("@OrderStatus", SqlDbType.Int, ParameterDirection.Input, orderStatus);
			sph.DefineSqlParameter("@PaymentStatus", SqlDbType.Int, ParameterDirection.Input, paymentStatus);
			sph.DefineSqlParameter("@ShippingStatus", SqlDbType.Int, ParameterDirection.Input, shippingStatus);
			sph.DefineSqlParameter("@ShippingMethod", SqlDbType.Int, ParameterDirection.Input, shippingMethod);
			sph.DefineSqlParameter("@PaymentMethod", SqlDbType.Int, ParameterDirection.Input, paymentMethod);
			sph.DefineSqlParameter("@InvoiceCompanyName", SqlDbType.NVarChar, 100, ParameterDirection.Input, invoiceCompanyName);
			sph.DefineSqlParameter("@InvoiceCompanyAddress", SqlDbType.NVarChar, 255, ParameterDirection.Input, invoiceCompanyAddress);
			sph.DefineSqlParameter("@InvoiceCompanyTaxCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, invoiceCompanyTaxCode);
			sph.DefineSqlParameter("@CustomValuesXml", SqlDbType.NVarChar, -1, ParameterDirection.Input, customValuesXml);
			sph.DefineSqlParameter("@StateID", SqlDbType.Int, ParameterDirection.Input, stateID);
			sph.DefineSqlParameter("@UserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, userGuid);
			sph.DefineSqlParameter("@CreatedFromIP", SqlDbType.NVarChar, 255, ParameterDirection.Input, createdFromIP);
			sph.DefineSqlParameter("@CreatedBy", SqlDbType.NVarChar, 255, ParameterDirection.Input, createdBy);
			sph.DefineSqlParameter("@CreatedUtc", SqlDbType.DateTime, ParameterDirection.Input, createdUtc);
			sph.DefineSqlParameter("@IsDeleted", SqlDbType.Bit, ParameterDirection.Input, isDeleted);
            sph.DefineSqlParameter("@UserPoint", SqlDbType.Int, ParameterDirection.Input, userPoint);
            sph.DefineSqlParameter("@UserPointDiscount", SqlDbType.Decimal, ParameterDirection.Input, userPointDiscount);
			int rowsAffected = sph.ExecuteNonQuery();
			return (rowsAffected > 0);
			
		}
		
		public static bool Delete(int orderID) 
		{
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "gb_Order_Delete", 1);
			sph.DefineSqlParameter("@OrderID", SqlDbType.Int, ParameterDirection.Input, orderID);
			int rowsAffected = sph.ExecuteNonQuery();
			return (rowsAffected > 0);
		}
		
		public static IDataReader GetOne(int  orderID)  
		{
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Order_SelectOne", 1);
			sph.DefineSqlParameter("@OrderID", SqlDbType.Int, ParameterDirection.Input, orderID);
			return sph.ExecuteReader();
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
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Order_GetCount", 11);
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
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Order_GetCountNotAdmin", 12);
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
            sph.DefineSqlParameter("@isAdmin", SqlDbType.Bit, ParameterDirection.Input, isAdmin);
			return Convert.ToInt32(sph.ExecuteScalar());
		}
		
		public static IDataReader GetPage(
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
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Order_SelectPage", 13);
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

        public static IDataReader GetPageNotAdmin(
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
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Order_SelectPageNotAdmin", 14);
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
            sph.DefineSqlParameter("@isAdmin", SqlDbType.Bit, ParameterDirection.Input, isAdmin);
            return sph.ExecuteReader();
        }

        public static IDataReader GetOneByCode(string orderCode)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Order_SelectOneByCode", 1);
            sph.DefineSqlParameter("@OrderCode", SqlDbType.NVarChar, ParameterDirection.Input, orderCode);
            return sph.ExecuteReader();
        }

	}

}
