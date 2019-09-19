using System;
using System.Data;

namespace CanhCam.Data
{
    public static class DBSiteUserEx
    {

        public static int GetCountByRole(int siteId, int roleId, DateTime? fromDate, DateTime? toDate,string keyword = null)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_CountByRole", 5);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            sph.DefineSqlParameter("@RoleID", SqlDbType.Int, ParameterDirection.Input, roleId);
            sph.DefineSqlParameter("@FromDate", SqlDbType.DateTime, ParameterDirection.Input, fromDate);
            sph.DefineSqlParameter("@ToDate", SqlDbType.DateTime, ParameterDirection.Input, toDate);
            sph.DefineSqlParameter("@Keyword", SqlDbType.NVarChar, 255, ParameterDirection.Input, keyword);
            return Convert.ToInt32(sph.ExecuteScalar());
        }

        public static int GetCountByRoleAddLoginName(int siteId, int roleId, DateTime? fromDate, DateTime? toDate, string keyword = null)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_CountByRoleAddLoginName", 5);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            sph.DefineSqlParameter("@RoleID", SqlDbType.Int, ParameterDirection.Input, roleId);
            sph.DefineSqlParameter("@FromDate", SqlDbType.DateTime, ParameterDirection.Input, fromDate);
            sph.DefineSqlParameter("@ToDate", SqlDbType.DateTime, ParameterDirection.Input, toDate);
            sph.DefineSqlParameter("@Keyword", SqlDbType.NVarChar, 255, ParameterDirection.Input, keyword);
            return Convert.ToInt32(sph.ExecuteScalar());
        }

        public static IDataReader GetUsersByRole(
            int siteId,
            int roleId,
            DateTime? fromDate, 
            DateTime? toDate,
            int pageNumber,
            int pageSize,
            string keyword = null 
            )
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_SelectPageByRole", 7);

            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            sph.DefineSqlParameter("@RoleID", SqlDbType.Int, ParameterDirection.Input, roleId);
            sph.DefineSqlParameter("@FromDate", SqlDbType.DateTime, ParameterDirection.Input, fromDate);
            sph.DefineSqlParameter("@ToDate", SqlDbType.DateTime, ParameterDirection.Input, toDate);
            sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
            sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
            sph.DefineSqlParameter("@Keyword", SqlDbType.NVarChar,255, ParameterDirection.Input, keyword);
            return sph.ExecuteReader();
        }

        public static IDataReader GetUsersByRoleAddLoginName(
            int siteId,
            int roleId,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize,
            string keyword = null
            )
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_SelectPageByRoleAddLoginName", 7);

            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            sph.DefineSqlParameter("@RoleID", SqlDbType.Int, ParameterDirection.Input, roleId);
            sph.DefineSqlParameter("@FromDate", SqlDbType.DateTime, ParameterDirection.Input, fromDate);
            sph.DefineSqlParameter("@ToDate", SqlDbType.DateTime, ParameterDirection.Input, toDate);
            sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
            sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
            sph.DefineSqlParameter("@Keyword", SqlDbType.NVarChar, 255, ParameterDirection.Input, keyword);
            return sph.ExecuteReader();
        }

        public static IDataReader GetAllUserAuthenticated(
            int siteId,
            int roleId
            )
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_SelectAllUserAuthenticated", 2);

            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            sph.DefineSqlParameter("@RoleID", SqlDbType.Int, ParameterDirection.Input, roleId);
            return sph.ExecuteReader();
        }

        public static IDataReader GetUpcomingBirthdays(int siteId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_SelectUpcomingBirthdays", 1);

            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);

            return sph.ExecuteReader();
        }


        public static IDataReader GetUpcomingBirthdays41and2000(int siteId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_SelectUpcomingBirthdays41and2000", 1);

            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);

            return sph.ExecuteReader();
        }
        public static bool UpdateUserPoints(Guid userGuid, int point)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "gb_Users_UpdatePoints", 2);
            sph.DefineSqlParameter("@UserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, userGuid);
            sph.DefineSqlParameter("@Point", SqlDbType.Int, ParameterDirection.Input, point);

            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);
        }

    }
}
