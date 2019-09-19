/// Author:					Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-11-24
/// Last Modified:			2014-11-24

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{

    public class CouponAppliedToItem
    {

        #region Constructors

        public CouponAppliedToItem()
        { }

        public CouponAppliedToItem(Guid guid)
        {
            this.GetCouponAppliedToItem(guid);
        }

        #endregion

        #region Private Properties

        private Guid guid = Guid.Empty;
        private int couponID = -1;
        private int itemID = -1;
        private int appliedType = 0;
        private bool usePercentage = false;
        private decimal discount = 0;
        private int giftType = 0;
        private string giftProducts = string.Empty;
        private string giftCustomProducts = string.Empty;

        #endregion

        #region Public Properties

        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        public int CouponId
        {
            get { return couponID; }
            set { couponID = value; }
        }
        public int ItemId
        {
            get { return itemID; }
            set { itemID = value; }
        }
        public int AppliedType
        {
            get { return appliedType; }
            set { appliedType = value; }
        }
        public bool UsePercentage
        {
            get { return usePercentage; }
            set { usePercentage = value; }
        }
        public decimal Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        public int GiftType
        {
            get { return giftType; }
            set { giftType = value; }
        }
        public string GiftProducts
        {
            get { return giftProducts; }
            set { giftProducts = value; }
        }
        public string GiftCustomProducts
        {
            get { return giftCustomProducts; }
            set { giftCustomProducts = value; }
        }
        
        #endregion

        #region Private Methods

        private void GetCouponAppliedToItem(Guid guid)
        {
            using (IDataReader reader = DBCouponAppliedToItem.GetOne(guid))
            {
                if (reader.Read())
                {
                    this.guid = new Guid(reader["Guid"].ToString());
                    this.couponID = Convert.ToInt32(reader["CouponID"]);
                    this.itemID = Convert.ToInt32(reader["ItemID"]);
                    this.appliedType = Convert.ToInt32(reader["AppliedType"]);
                    this.usePercentage = Convert.ToBoolean(reader["UsePercentage"]);
                    this.discount = Convert.ToDecimal(reader["Discount"]);
                    this.giftType = Convert.ToInt32(reader["GiftType"]);
                    this.giftProducts = reader["GiftProducts"].ToString();
                    this.giftCustomProducts = reader["GiftCustomProducts"].ToString();
                }
            }
        }

        private bool Create()
        {
            this.guid = Guid.NewGuid();

            int rowsAffected = DBCouponAppliedToItem.Create(
                this.guid,
                this.couponID,
                this.itemID,
                this.appliedType,
                this.usePercentage,
                this.discount,
                this.giftType,
                this.giftProducts,
                this.giftCustomProducts);

            return (rowsAffected > 0);
        }

        private bool Update()
        {
            return DBCouponAppliedToItem.Update(
                this.guid,
                this.couponID,
                this.itemID,
                this.appliedType,
                this.usePercentage,
                this.discount,
                this.giftType,
                this.giftProducts,
                this.giftCustomProducts);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this instance of CouponAppliedToItem. Returns true on success.
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
            return DBCouponAppliedToItem.Delete(guid);
        }

        public static bool DeleteByCoupon(int couponId)
        {
            return DBCouponAppliedToItem.DeleteByCoupon(couponId);
        }

        private static List<CouponAppliedToItem> LoadListFromReader(IDataReader reader)
        {
            List<CouponAppliedToItem> couponAppliedToItemList = new List<CouponAppliedToItem>();
            try
            {
                while (reader.Read())
                {
                    CouponAppliedToItem couponAppliedToItem = new CouponAppliedToItem();
                    couponAppliedToItem.guid = new Guid(reader["Guid"].ToString());
                    couponAppliedToItem.couponID = Convert.ToInt32(reader["CouponID"]);
                    couponAppliedToItem.itemID = Convert.ToInt32(reader["ItemID"]);
                    couponAppliedToItem.appliedType = Convert.ToInt32(reader["AppliedType"]);
                    couponAppliedToItem.usePercentage = Convert.ToBoolean(reader["UsePercentage"]);
                    couponAppliedToItem.discount = Convert.ToDecimal(reader["Discount"]);
                    couponAppliedToItem.giftType = Convert.ToInt32(reader["GiftType"]);
                    couponAppliedToItem.giftProducts = reader["GiftProducts"].ToString();
                    couponAppliedToItem.giftCustomProducts = reader["GiftCustomProducts"].ToString();
                    couponAppliedToItemList.Add(couponAppliedToItem);
                }
            }
            finally
            {
                reader.Close();
            }

            return couponAppliedToItemList;
        }

        public static List<CouponAppliedToItem> GetByCoupon(int couponId, int appliedType)
        {
            IDataReader reader = DBCouponAppliedToItem.GetByCoupon(couponId, appliedType);
            return LoadListFromReader(reader);
        }

        //public static string GetItemIdsByCoupon(int couponId, int appliedType)
        //{
        //    List<CouponAppliedToItem> lstItems = CouponAppliedToItem.GetByCoupon(couponId, appliedType);
        //    string productIds = CouponAppliedToItem.GetItemIdsFromList(lstItems, appliedType);

        //    return productIds;
        //}

        public static CouponAppliedToItem FindFromList(List<CouponAppliedToItem> lstItems, int itemId, int appliedType)
        {
            foreach (CouponAppliedToItem item in lstItems)
            {
                if (item.itemID == itemId && appliedType == item.appliedType)
                    return item;
            }

            return null;
        }

        public static string GetItemIdsFromList(List<CouponAppliedToItem> lstItems, int appliedType)
        {
            string result = string.Empty;
            string sepa = string.Empty;
            foreach (CouponAppliedToItem item in lstItems)
            {
                if (item.appliedType == appliedType)
                {
                    result += sepa + item.itemID;
                    sepa = ";";
                }
            }

            return result;
        }

        #endregion

    }

}
