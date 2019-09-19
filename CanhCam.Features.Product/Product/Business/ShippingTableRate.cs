/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-05-05
/// Last Modified:			2015-05-05

using System;
using System.Collections.Generic;
using System.Data;
using CanhCam.Data;

namespace CanhCam.Business
{
    public class ShippingTableRate
    {

        #region Constructors

        public ShippingTableRate()
        { }


        public ShippingTableRate(int shippingTableRateId)
        {
            this.GetShippingTableRate(shippingTableRateId);
        }

        #endregion

        #region Private Properties

        private int shippingTableRateID = -1;
        private int shippingMethodID = -1;
        private Guid geoZoneGuid = Guid.Empty;
        private decimal fromValue;
        private decimal shippingFee;
        private decimal additionalValue;
        private decimal additionalFee;
        private decimal freeShippingOverXValue;
        private string geoZoneName = string.Empty;
        private bool markAsDeleted = true;

        #endregion

        #region Public Properties

        public int ShippingTableRateId
        {
            get { return shippingTableRateID; }
            set { shippingTableRateID = value; }
        }
        public int ShippingMethodId
        {
            get { return shippingMethodID; }
            set { shippingMethodID = value; }
        }
        public Guid GeoZoneGuid
        {
            get { return geoZoneGuid; }
            set { geoZoneGuid = value; }
        }
        public decimal FromValue
        {
            get { return fromValue; }
            set { fromValue = value; }
        }
        public decimal ShippingFee
        {
            get { return shippingFee; }
            set { shippingFee = value; }
        }
        public decimal AdditionalValue
        {
            get { return additionalValue; }
            set { additionalValue = value; }
        }
        public decimal AdditionalFee
        {
            get { return additionalFee; }
            set { additionalFee = value; }
        }
        public decimal FreeShippingOverXValue
        {
            get { return freeShippingOverXValue; }
            set { freeShippingOverXValue = value; }
        }
        public string GeoZoneName
        {
            get { return geoZoneName; }
            set { geoZoneName = value; }
        }
        public bool MarkAsDeleted
        {
            get { return markAsDeleted; }
            set { markAsDeleted = value; }
        }
        #endregion

        #region Private Methods

        private void GetShippingTableRate(int shippingTableRateId)
        {
            using (IDataReader reader = DBShippingTableRate.GetOne(shippingTableRateId))
            {
                if (reader.Read())
                {
                    this.shippingTableRateID = Convert.ToInt32(reader["ShippingTableRateID"]);
                    this.shippingMethodID = Convert.ToInt32(reader["ShippingMethodID"]);
                    this.geoZoneGuid = new Guid(reader["GeoZoneGuid"].ToString());
                    this.fromValue = Convert.ToDecimal(reader["FromValue"]);
                    this.shippingFee = Convert.ToDecimal(reader["ShippingFee"]);
                    this.additionalValue = Convert.ToDecimal(reader["AdditionalValue"]);
                    this.additionalFee = Convert.ToDecimal(reader["AdditionalFee"]);
                    this.freeShippingOverXValue = Convert.ToDecimal(reader["FreeShippingOverXValue"]);
                }
            }

        }

        /// <summary>
        /// Persists a new instance of ShippingTableRate. Returns true on success.
        /// </summary>
        /// <returns></returns>
        private bool Create()
        {
            int newID = 0;

            newID = DBShippingTableRate.Create(
                this.shippingMethodID,
                this.geoZoneGuid,
                this.fromValue,
                this.shippingFee,
                this.additionalValue,
                this.additionalFee,
                this.freeShippingOverXValue);

            this.shippingTableRateID = newID;

            return (newID > 0);
        }

        private bool Update()
        {
            return DBShippingTableRate.Update(
                this.shippingTableRateID,
                this.shippingMethodID,
                this.geoZoneGuid,
                this.fromValue,
                this.shippingFee,
                this.additionalValue,
                this.additionalFee,
                this.freeShippingOverXValue);
        }

        #endregion

        #region Public Methods

        public bool Save()
        {
            if (this.shippingTableRateID > 0)
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

        public static bool Delete(int shippingTableRateId)
        {
            return DBShippingTableRate.Delete(shippingTableRateId);
        }

        public static bool DeleteByMethod(int shippingMethodId)
        {
            return DBShippingTableRate.DeleteByMethod(shippingMethodId);
        }

        private static List<ShippingTableRate> LoadListFromReader(IDataReader reader, bool loadZoneName = false)
        {
            List<ShippingTableRate> shippingTableRateList = new List<ShippingTableRate>();
            try
            {
                while (reader.Read())
                {
                    ShippingTableRate shippingTableRate = new ShippingTableRate();
                    shippingTableRate.shippingTableRateID = Convert.ToInt32(reader["ShippingTableRateID"]);
                    shippingTableRate.shippingMethodID = Convert.ToInt32(reader["ShippingMethodID"]);
                    shippingTableRate.geoZoneGuid = new Guid(reader["GeoZoneGuid"].ToString());
                    shippingTableRate.fromValue = Convert.ToDecimal(reader["FromValue"]);
                    shippingTableRate.shippingFee = Convert.ToDecimal(reader["ShippingFee"]);
                    shippingTableRate.additionalValue = Convert.ToDecimal(reader["AdditionalValue"]);
                    shippingTableRate.additionalFee = Convert.ToDecimal(reader["AdditionalFee"]);
                    shippingTableRate.freeShippingOverXValue = Convert.ToDecimal(reader["FreeShippingOverXValue"]);

                    if (loadZoneName)
                        if (reader["GeoZoneName"] != DBNull.Value)
                            shippingTableRate.geoZoneName = reader["GeoZoneName"].ToString();

                    shippingTableRateList.Add(shippingTableRate);
                }
            }
            finally
            {
                reader.Close();
            }

            return shippingTableRateList;
        }

        public static List<ShippingTableRate> GetByMethod(int shippingMethodId)
        {
            IDataReader reader = DBShippingTableRate.GetByMethod(shippingMethodId);
            return LoadListFromReader(reader, true);
        }

        public static ShippingTableRate GetOneMaxValue(int shippingMethodId, decimal fromValue, string geoZoneGuids)
        {
            if (string.IsNullOrEmpty(geoZoneGuids))
                geoZoneGuids = null;

            IDataReader reader = DBShippingTableRate.GetOneMaxValue(shippingMethodId, fromValue, geoZoneGuids);
            var lstTableRates = LoadListFromReader(reader);
            if (lstTableRates.Count > 0)
                return lstTableRates[0];

            return null;
        }

        #endregion

    }

}
