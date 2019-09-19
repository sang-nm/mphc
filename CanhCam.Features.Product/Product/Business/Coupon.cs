/// Author:					Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-10-06
/// Last Modified:			2014-10-06

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{

    public enum CouponDiscountType
    {
        PercentagePerProduct = 1,
        AmountPerProduct = 2
    }

    public enum CouponAppliedType
    {
        ToCategories = 0,
        ToProducts = 1
    }

    public class Coupon
    {

        #region Constructors

        public Coupon()
        { }

        public Coupon(int couponId)
        {
            this.GetCoupon(couponId);
        }

        #endregion

        #region Private Properties

        private int couponID = -1;
        private int siteID = -1;
        private string couponCode = string.Empty;
        private string name = string.Empty;
        private decimal discount;
        private int discountType = -1;
        private decimal orderPurchaseFrom;
        private decimal orderPurchaseTo;
        private DateTime? fromDate;
        private DateTime? expiryDate;
        private decimal minPurchase;
        private int limitationTimes = -1;
        private bool isActive = false;
        private int appliedType = 0;
        private string appliedToProducts = string.Empty;
        private string appliedToCategories = string.Empty;
        private Guid guid = Guid.Empty;
        private DateTime createdOn = DateTime.Now;
        private int numOfUses = 0;
        private int orderCount = 0;
        
        //http://www.magentocommerce.com/knowledge-base/entry/what-are-shopping-cart-price-rules-and-how-do-i-use-them
        private int discountQtyStep = 1; // Discount Qty Step
        private int maximumQtyDiscount = 0; // Maximum qty the discount
        
        #endregion

        #region Public Properties

        public int CouponId
        {
            get { return couponID; }
            set { couponID = value; }
        }
        public int SiteId
        {
            get { return siteID; }
            set { siteID = value; }
        }
        public string CouponCode
        {
            get { return couponCode; }
            set { couponCode = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public decimal Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        public int DiscountType
        {
            get { return discountType; }
            set { discountType = value; }
        }
        public decimal OrderPurchaseFrom
        {
            get { return orderPurchaseFrom; }
            set { orderPurchaseFrom = value; }
        }
        public decimal OrderPurchaseTo
        {
            get { return orderPurchaseTo; }
            set { orderPurchaseTo = value; }
        }
        public DateTime? FromDate
        {
            get { return fromDate; }
            set { fromDate = value; }
        }
        public DateTime? ExpiryDate
        {
            get { return expiryDate; }
            set { expiryDate = value; }
        }
        public decimal MinPurchase
        {
            get { return minPurchase; }
            set { minPurchase = value; }
        }
        public int LimitationTimes
        {
            get { return limitationTimes; }
            set { limitationTimes = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public int AppliedType
        {
            get { return appliedType; }
            set { appliedType = value; }
        }
        public string AppliedToProducts
        {
            get { return appliedToProducts; }
            set { appliedToProducts = value; }
        }
        public string AppliedToCategories
        {
            get { return appliedToCategories; }
            set { appliedToCategories = value; }
        }
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }
        public int NumOfUses
        {
            get { return numOfUses; }
            set { numOfUses = value; }
        }
        public int OrderCount
        {
            get { return orderCount; }
            set { orderCount = value; }
        }
        
        public int DiscountQtyStep
        {
            get { return discountQtyStep; }
            set { discountQtyStep = value; }
        }
        public int MaximumQtyDiscount
        {
            get { return maximumQtyDiscount; }
            set { maximumQtyDiscount = value; }
        }

        #endregion

        #region Private Methods

        private void GetCoupon(int couponId)
        {
            using (IDataReader reader = DBCoupon.GetOne(couponId))
            {
                if (reader.Read())
                {
                    this.couponID = Convert.ToInt32(reader["CouponID"]);
                    this.siteID = Convert.ToInt32(reader["SiteID"]);
                    this.couponCode = reader["CouponCode"].ToString();
                    this.name = reader["Name"].ToString();
                    this.discount = Convert.ToDecimal(reader["Discount"]);
                    this.discountType = Convert.ToInt32(reader["DiscountType"]);
                    this.orderPurchaseFrom = Convert.ToDecimal(reader["OrderPurchaseFrom"]);
                    this.orderPurchaseTo = Convert.ToDecimal(reader["OrderPurchaseTo"]);
                    if (reader["FromDate"] != DBNull.Value)
                        this.fromDate = Convert.ToDateTime(reader["FromDate"]);
                    if (reader["ExpiryDate"] != DBNull.Value)
                        this.expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                    this.minPurchase = Convert.ToDecimal(reader["MinPurchase"]);
                    this.limitationTimes = Convert.ToInt32(reader["LimitationTimes"]);
                    this.isActive = Convert.ToBoolean(reader["IsActive"]);
                    this.appliedType = Convert.ToInt32(reader["AppliedType"]);
                    this.appliedToProducts = reader["AppliedToProducts"].ToString();
                    this.appliedToCategories = reader["AppliedToCategories"].ToString();
                    this.guid = new Guid(reader["Guid"].ToString());
                    this.createdOn = Convert.ToDateTime(reader["CreatedOn"]);

                    if (reader["DiscountQtyStep"] != DBNull.Value)
                        this.discountQtyStep = Convert.ToInt32(reader["DiscountQtyStep"]);
                    if (reader["MaximumQtyDiscount"] != DBNull.Value)
                        this.maximumQtyDiscount = Convert.ToInt32(reader["MaximumQtyDiscount"]);
                }
            }
        }

        private bool Create()
        {
            int newID = 0;
            this.guid = Guid.NewGuid();

            newID = DBCoupon.Create(
                this.siteID,
                this.couponCode,
                this.name,
                this.discount,
                this.discountType,
                this.orderPurchaseFrom,
                this.orderPurchaseTo,
                this.fromDate,
                this.expiryDate,
                this.minPurchase,
                this.limitationTimes,
                this.isActive,
                this.appliedType,
                this.appliedToProducts,
                this.appliedToCategories,
                this.guid,
                this.createdOn,
                this.discountQtyStep,
                this.maximumQtyDiscount);

            this.couponID = newID;

            return (newID > 0);

        }

        private bool Update()
        {
            return DBCoupon.Update(
                this.couponID,
                this.siteID,
                this.couponCode,
                this.name,
                this.discount,
                this.discountType,
                this.orderPurchaseFrom,
                this.orderPurchaseTo,
                this.fromDate,
                this.expiryDate,
                this.minPurchase,
                this.limitationTimes,
                this.isActive,
                this.appliedType,
                this.appliedToProducts,
                this.appliedToCategories,
                this.guid,
                this.createdOn,
                this.discountQtyStep,
                this.maximumQtyDiscount);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this instance of Coupon. Returns true on success.
        /// </summary>
        /// <returns>bool</returns>
        public bool Save()
        {
            if (this.couponID > 0)
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

        public static bool Delete(int couponId)
        {
            return DBCoupon.Delete(couponId);
        }

        public static bool DeleteBySite(int siteId)
        {
            return DBCoupon.DeleteBySite(siteId);
        }

        public static Coupon GetOneByCode(int siteId, string couponCode)
        {
            using (IDataReader reader = DBCoupon.GetOneByCode(siteId, couponCode))
            {
                if (reader.Read())
                {
                    Coupon coupon = new Coupon();

                    coupon.couponID = Convert.ToInt32(reader["CouponID"]);
                    coupon.siteID = Convert.ToInt32(reader["SiteID"]);
                    coupon.couponCode = reader["CouponCode"].ToString();
                    coupon.name = reader["Name"].ToString();
                    coupon.discount = Convert.ToDecimal(reader["Discount"]);
                    coupon.discountType = Convert.ToInt32(reader["DiscountType"]);
                    coupon.orderPurchaseFrom = Convert.ToDecimal(reader["OrderPurchaseFrom"]);
                    coupon.orderPurchaseTo = Convert.ToDecimal(reader["OrderPurchaseTo"]);
                    if (reader["FromDate"] != DBNull.Value)
                        coupon.fromDate = Convert.ToDateTime(reader["FromDate"]);
                    if (reader["ExpiryDate"] != DBNull.Value)
                        coupon.expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                    coupon.minPurchase = Convert.ToDecimal(reader["MinPurchase"]);
                    coupon.limitationTimes = Convert.ToInt32(reader["LimitationTimes"]);
                    coupon.isActive = Convert.ToBoolean(reader["IsActive"]);
                    coupon.appliedType = Convert.ToInt32(reader["AppliedType"]);
                    coupon.appliedToProducts = reader["AppliedToProducts"].ToString();
                    coupon.appliedToCategories = reader["AppliedToCategories"].ToString();
                    coupon.guid = new Guid(reader["Guid"].ToString());
                    coupon.createdOn = Convert.ToDateTime(reader["CreatedOn"]);
                    coupon.numOfUses = Convert.ToInt32(reader["NumOfUses"]);
                    
                    if (reader["DiscountQtyStep"] != DBNull.Value)
                        coupon.discountQtyStep = Convert.ToInt32(reader["DiscountQtyStep"]);
                    if (reader["MaximumQtyDiscount"] != DBNull.Value)
                        coupon.maximumQtyDiscount = Convert.ToInt32(reader["MaximumQtyDiscount"]);

                    return coupon;
                }

                return null;
            }
        }

        public static bool ExistCode(int siteId, string couponCode)
        {
            using (IDataReader reader = DBCoupon.GetOneByCode(siteId, couponCode))
            {
                if (reader.Read())
                    return true;
            }

            return false;
        }

        public static int GetCount(int siteId)
        {
            return DBCoupon.GetCount(siteId);
        }

        private static List<Coupon> LoadListFromReader(IDataReader reader)
        {
            List<Coupon> couponList = new List<Coupon>();
            try
            {
                while (reader.Read())
                {
                    Coupon coupon = new Coupon();
                    coupon.couponID = Convert.ToInt32(reader["CouponID"]);
                    coupon.siteID = Convert.ToInt32(reader["SiteID"]);
                    coupon.couponCode = reader["CouponCode"].ToString();
                    coupon.name = reader["Name"].ToString();
                    coupon.discount = Convert.ToDecimal(reader["Discount"]);
                    coupon.discountType = Convert.ToInt32(reader["DiscountType"]);
                    coupon.orderPurchaseFrom = Convert.ToDecimal(reader["OrderPurchaseFrom"]);
                    coupon.orderPurchaseTo = Convert.ToDecimal(reader["OrderPurchaseTo"]);
                    if (reader["FromDate"] != DBNull.Value)
                        coupon.fromDate = Convert.ToDateTime(reader["FromDate"]);
                    if (reader["ExpiryDate"] != DBNull.Value)
                        coupon.expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                    coupon.minPurchase = Convert.ToDecimal(reader["MinPurchase"]);
                    coupon.limitationTimes = Convert.ToInt32(reader["LimitationTimes"]);
                    coupon.isActive = Convert.ToBoolean(reader["IsActive"]);
                    coupon.appliedType = Convert.ToInt32(reader["AppliedType"]);
                    coupon.appliedToProducts = reader["AppliedToProducts"].ToString();
                    coupon.appliedToCategories = reader["AppliedToCategories"].ToString();
                    coupon.guid = new Guid(reader["Guid"].ToString());
                    coupon.createdOn = Convert.ToDateTime(reader["CreatedOn"]);
                    coupon.orderCount = Convert.ToInt32(reader["OrderCount"]);
                    couponList.Add(coupon);

                }
            }
            finally
            {
                reader.Close();
            }

            return couponList;
        }

        public static List<Coupon> GetPage(int siteId, int pageNumber, int pageSize)
        {
            IDataReader reader = DBCoupon.GetPage(siteId, pageNumber, pageSize);
            return LoadListFromReader(reader);
        }

        public static bool IsAvailable(int siteId, string zoneIds, string productIds)
        {
            return DBCoupon.IsAvailable(siteId, zoneIds, productIds);
        }

        #endregion

    }

}
