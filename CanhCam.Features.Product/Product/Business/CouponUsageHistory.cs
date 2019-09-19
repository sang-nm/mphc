/// Author:					Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-10-06
/// Last Modified:			2014-10-06

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{

    public class CouponUsageHistory
    {

        #region Constructors

        public CouponUsageHistory()
        { }


        public CouponUsageHistory(
            int historyID)
        {
            this.GetCouponUsageHistory(
                historyID);
        }

        #endregion

        #region Private Properties

        private int historyID = -1;
        private int couponID = -1;
        private int orderID = -1;
        private DateTime createdOn = DateTime.Now;

        #endregion

        #region Public Properties

        public int HistoryId
        {
            get { return historyID; }
            set { historyID = value; }
        }
        public int CouponId
        {
            get { return couponID; }
            set { couponID = value; }
        }
        public int OrderId
        {
            get { return orderID; }
            set { orderID = value; }
        }
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        #endregion

        #region Private Methods

        private void GetCouponUsageHistory(int historyId)
        {
            using (IDataReader reader = DBCouponUsageHistory.GetOne(historyId))
            {
                if (reader.Read())
                {
                    this.historyID = Convert.ToInt32(reader["HistoryID"]);
                    this.couponID = Convert.ToInt32(reader["CouponID"]);
                    this.orderID = Convert.ToInt32(reader["OrderID"]);
                    this.createdOn = Convert.ToDateTime(reader["CreatedOn"]);
                }
            }
        }

        private bool Create()
        {
            int newID = 0;

            newID = DBCouponUsageHistory.Create(
                this.couponID,
                this.orderID,
                this.createdOn);

            this.historyID = newID;

            return (newID > 0);
        }

        private bool Update()
        {
            return DBCouponUsageHistory.Update(
                this.historyID,
                this.couponID,
                this.orderID,
                this.createdOn);
        }

        #endregion

        #region Public Methods

        public bool Save()
        {
            if (this.historyID > 0)
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

        public static int GetCountByCoupon(int couponId)
        {
            return DBCouponUsageHistory.GetCountByCoupon(couponId);
        }

        public static bool Delete(int historyId)
        {
            return DBCouponUsageHistory.Delete(historyId);
        }

        public static bool DeleteByCoupon(int couponId)
        {
            return DBCouponUsageHistory.DeleteByCoupon(couponId);
        }

        public static bool DeleteByOrder(int orderId)
        {
            return DBCouponUsageHistory.DeleteByOrder(orderId);
        }

        private static List<CouponUsageHistory> LoadListFromReader(IDataReader reader)
        {
            List<CouponUsageHistory> couponUsageHistoryList = new List<CouponUsageHistory>();
            try
            {
                while (reader.Read())
                {
                    CouponUsageHistory couponUsageHistory = new CouponUsageHistory();
                    couponUsageHistory.historyID = Convert.ToInt32(reader["HistoryID"]);
                    couponUsageHistory.couponID = Convert.ToInt32(reader["CouponID"]);
                    couponUsageHistory.orderID = Convert.ToInt32(reader["OrderID"]);
                    couponUsageHistory.createdOn = Convert.ToDateTime(reader["CreatedOn"]);
                    couponUsageHistoryList.Add(couponUsageHistory);

                }
            }
            finally
            {
                reader.Close();
            }

            return couponUsageHistoryList;

        }

        public static IDataReader GetByCoupon(int couponId)
        {
            IDataReader reader = DBCouponUsageHistory.GetByCoupon(couponId);
            return reader; //LoadListFromReader(reader);
        }

        #endregion

    }

}