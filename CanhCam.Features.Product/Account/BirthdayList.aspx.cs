using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.Web.Configuration;
using CanhCam.Web.Framework;
using Resources;
using log4net;
using System.Data;
using System.Web;

namespace CanhCam.Web.AccountUI
{
    public partial class BirthdayList : CmsNonBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BirthdayList));

        protected bool IsAdmin = false;
        protected bool canManageUsers = false;

        protected Double timeOffset = 0;
        protected TimeZoneInfo timeZone = null;

        private bool allowView = false;

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);

            grid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grid_NeedDataSource);
        }

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            LoadSettings();
            PopulateLabels();

            if (!allowView)
            {
                //WebUtils.SetupRedirect(this, SiteRoot + "/Default.aspx");
                SiteUtils.RedirectToEditAccessDeniedPage();
                return;
            }

            if (!IsPostBack)
            {
                PopulateControls();
            }
        }

        private void PopulateControls()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, ResourceHelper.GetResourceString("Resource", "MemberListLink"));
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DateTime? birthdayFrom = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            DateTime? birthdayTo = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59);

            var data = GetDataForExport(birthdayFrom, birthdayTo);

            ExportHelper.ExportDataTableToExcel(System.Web.HttpContext.Current, data, "memberlist-" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls");
        }

        private DataTable GetDataForExport(DateTime? birthdayFrom, DateTime? birthdayTo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã khách hàng", typeof(String));
            dt.Columns.Add("Email", typeof(String));
            dt.Columns.Add("Họ tên", typeof(String));
            dt.Columns.Add("Điện thoại", typeof(String));
            dt.Columns.Add("Địa chỉ", typeof(String));
            dt.Columns.Add("Sinh nhật", typeof(String));
            dt.Columns.Add("Điểm", typeof(String));

            try
            {
                List<SiteUserEx> siteUserPage = SiteUserEx.GetUpcomingBirthdays41and2000(siteSettings.SiteId);

                foreach (SiteUserEx siteUser in siteUserPage)
                {
                    DataRow row = dt.NewRow();
                    row["Mã khách hàng"] = siteUser.Name;
                    row["Email"] = siteUser.Email;
                    row["Họ tên"] = siteUser.FirstName;
                    row["Điện thoại"] = siteUser.LoginName;
                    row["Địa chỉ"] = siteUser.State;
                    row["Sinh nhật"] = siteUser.Yahoo;
                    row["Điểm"] = siteUser.TotalPostsEx;

                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return dt;
        }

        void grid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grid.DataSource = SiteUserEx.GetUpcomingBirthdays41and2000(siteSettings.SiteId);
        }

        void gridFuture_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            //var role = new Role(siteSettings.SiteId, "Authenticated Users");

            //DateTime? fromDate = null;
            //DateTime? toDate = null;
            //DateTime? birthdayFrom = new DateTime(DateTime.UtcNow.AddDays(1).Year, DateTime.UtcNow.AddDays(1).Month, DateTime.UtcNow.AddDays(1).Day, 0, 0, 0);
            //DateTime? birthdayTo = new DateTime(DateTime.UtcNow.AddDays(7).Year, DateTime.UtcNow.AddDays(7).Month, DateTime.UtcNow.AddDays(7).Day, 23, 59, 59);

            //bool isApplied = false;
            //int iCount = SiteUserEx.GetCountByRole(siteSettings.SiteId, role.RoleId, fromDate, toDate, birthdayFrom, birthdayTo);
            //int startRowIndex = isApplied ? 1 : grid.CurrentPageIndex + 1;
            //int maximumRows = isApplied ? iCount : grid.PageSize;

            //gridFuture.VirtualItemCount = iCount;
            //gridFuture.AllowCustomPaging = !isApplied;

            //gridFuture.DataSource = SiteUserEx.GetUsersByRole(siteSettings.SiteId, role.RoleId, fromDate, toDate, birthdayFrom, birthdayTo, startRowIndex, maximumRows);
        }

        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, ResourceHelper.GetResourceString("Resource", "MemberListLink"));
            heading.Text = breadcrumb.CurrentPageTitle;
        }

        private void LoadSettings()
        {
            timeOffset = SiteUtils.GetUserTimeOffset();
            timeZone = SiteUtils.GetUserTimeZone();
            IsAdmin = WebUser.IsAdmin;

            //allowView = WebUser.IsInRoles(siteSettings.RolesThatCanViewMemberList);
            allowView = WebUser.IsInRoles("Order Administrators");

            if ((IsAdmin) || (WebUser.IsInRoles(siteSettings.RolesThatCanManageUsers)))
                canManageUsers = true;
        }

    }
}

