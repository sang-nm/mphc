using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using log4net;
using CanhCam.Data;
using CanhCam.Business.WebHelpers;

namespace CanhCam.Business
{
    public class SiteUserEx : SiteUser
    {
        public SiteUserEx(SiteSettings settings)
            : base(settings)
        {
        }

        private int userID = -1;
        //private string fullName = string.Empty;
        //private string phone = string.Empty;
        //private string address = string.Empty;
        //private string birthday = string.Empty;
        private int totalPosts = 0;
        private decimal totalRevenue = 0;
        private DateTime DateCreated = DateTime.Now;
        private Guid UserGuid = Guid.Empty;
        public int UserIdEx
        {
            get { return userID; }
            set { userID = value; }
        }
        public int TotalPostsEx
        {
            get { return totalPosts; }
            set { totalPosts = value; }
        }
        public decimal TotalRevenueEx
        {
            get { return totalRevenue; }
            set { totalRevenue = value; }
        }
        public DateTime DateCreatedEx
        {
            get { return DateCreated; }
            set { DateCreated = value; }
        }
        public Guid UserGuidEx
        {
            get { return UserGuid; }
            set { UserGuid = value; }
        }


        public static int GetCountByRole(int siteId, int roleId, DateTime? fromDate, DateTime? toDate, string keyword = null )
        {
            return DBSiteUserEx.GetCountByRole(siteId, roleId, fromDate, toDate, keyword);
        }

        public static int GetCountByRoleAddLoginName(int siteId, int roleId, DateTime? fromDate, DateTime? toDate, string keyword = null)
        {
            return DBSiteUserEx.GetCountByRoleAddLoginName(siteId, roleId, fromDate, toDate, keyword);
        }

        private static List<SiteUserEx> PopulateData(IDataReader reader, bool loadTotal)
        {
            List<SiteUserEx> userList = new List<SiteUserEx>();

            while (reader.Read())
            {
                SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
                SiteUserEx user = new SiteUserEx(siteSettings);

                user.UserIdEx = Convert.ToInt32(reader["UserID"]);
                user.Name = reader["Name"].ToString();
                user.LoginName = reader["LoginName"].ToString();

                user.FirstName = reader["FirstName"].ToString();
                user.LastName = reader["LastName"].ToString();

                user.Email = reader["Email"].ToString();
                user.Password = reader["Pwd"].ToString();
                user.PasswordQuestion = reader["PasswordQuestion"].ToString();
                user.PasswordAnswer = reader["PasswordAnswer"].ToString();
                user.Gender = reader["Gender"].ToString();
                if (reader["ProfileApproved"] != DBNull.Value)
                    user.ProfileApproved = Convert.ToBoolean(reader["ProfileApproved"]);
                if (reader["Trusted"] != DBNull.Value)
                    user.Trusted = Convert.ToBoolean(reader["Trusted"]);
                if (reader["DisplayInMemberList"] != DBNull.Value)
                    user.DisplayInMemberList = Convert.ToBoolean(reader["DisplayInMemberList"]);
                user.WebSiteUrl = reader["WebSiteURL"].ToString();
                user.Country = reader["Country"].ToString();
                user.State = reader["State"].ToString();
                user.Occupation = reader["Occupation"].ToString();
                user.Interests = reader["Interests"].ToString();
                user.MSN = reader["MSN"].ToString();
                user.Yahoo = reader["Yahoo"].ToString();
                user.AIM = reader["AIM"].ToString();
                user.ICQ = reader["ICQ"].ToString();
                if (reader["TotalPosts"] != DBNull.Value)
                    user.TotalPosts = Convert.ToInt32(reader["TotalPosts"]);
                user.AvatarUrl = reader["AvatarUrl"].ToString();
                user.Signature = reader["Signature"].ToString();
                user.DateCreatedEx = Convert.ToDateTime(reader["DateCreated"]);
                user.UserGuidEx = (Guid)reader["UserGuid"];
                user.Skin = reader["Skin"].ToString();

                user.TotalPostsEx = Convert.ToInt32(reader["TotalPosts"]);
                if (loadTotal && reader["TotalRevenueEx"] != DBNull.Value)
                    user.TotalRevenueEx = Convert.ToDecimal(reader["TotalRevenueEx"]);

                userList.Add(user);
            }

            return userList;
        }

        public static List<SiteUserEx> GetUsersByRole(
            int siteId,
            int roleId,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize,
            string keyword = null)
        {
            List<SiteUserEx> userList = new List<SiteUserEx>();
            using (IDataReader reader = DBSiteUserEx.GetUsersByRole(siteId, roleId, fromDate, toDate, pageNumber, pageSize, keyword))
            {
                userList = PopulateData(reader, true);
            }

            return userList;
        }

        public static List<SiteUserEx> GetUsersByRoleAddLoginName(
            int siteId,
            int roleId,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize,
            string keyword = null)
        {
            List<SiteUserEx> userList = new List<SiteUserEx>();
            using (IDataReader reader = DBSiteUserEx.GetUsersByRoleAddLoginName(siteId, roleId, fromDate, toDate, pageNumber, pageSize, keyword))
            {
                userList = PopulateData(reader, true);
            }

            return userList;
        }

        public static List<SiteUserEx> GetAllUserAuthenticated(
            int siteId,
            int roleId)
        {
            List<SiteUserEx> userList = new List<SiteUserEx>();
            using (IDataReader reader = DBSiteUserEx.GetAllUserAuthenticated(siteId, roleId))
            {
                userList = PopulateData(reader, true);
            }

            return userList;
        }

        public static List<SiteUserEx> GetUpcomingBirthdays(int siteId)
        {
            List<SiteUserEx> userList = new List<SiteUserEx>();
            using (IDataReader reader = DBSiteUserEx.GetUpcomingBirthdays(siteId))
            {
                userList = PopulateData(reader, false);
            }

            return userList;
        }

        public static List<SiteUserEx> GetUpcomingBirthdays41and2000(int siteId)
        {
            List<SiteUserEx> userList = new List<SiteUserEx>();
            using (IDataReader reader = DBSiteUserEx.GetUpcomingBirthdays41and2000(siteId))
            {
                userList = PopulateData(reader, false);
            }

            return userList;
        }


        public static bool UpdateUserPoints(Guid userGuid, int point)
        {
            return DBSiteUserEx.UpdateUserPoints(userGuid, point);
        }

    }
}
