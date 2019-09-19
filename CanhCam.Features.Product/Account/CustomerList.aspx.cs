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
    public partial class CustomerList : CmsNonBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerList));

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
            btnSearch.Click += new EventHandler(btnSearch_Click);
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

            if (!WebUser.IsAdminOrContentAdmin)
            {
                btnExport.Visible = false;
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
            try
            {
                if (!canManageUsers)
                    return;

                DateTime? fromDate = null;
                DateTime? toDate = null;
                if (dpFromDate.SelectedDate.HasValue)
                {
                    var localTime = new DateTime(dpToDate.SelectedDate.Value.Year, dpToDate.SelectedDate.Value.Month, dpToDate.SelectedDate.Value.Day, 0, 0, 0);

                    if (timeZone != null)
                        fromDate = localTime.ToUtc(timeZone);
                    else
                        fromDate = localTime.AddHours(-timeOffset);
                }
                if (dpToDate.SelectedDate.HasValue)
                {
                    var localTime = new DateTime(dpToDate.SelectedDate.Value.Year, dpToDate.SelectedDate.Value.Month, dpToDate.SelectedDate.Value.Day, 23, 59, 59);

                    if (timeZone != null)
                        toDate = localTime.ToUtc(timeZone);
                    else
                        toDate = localTime.AddHours(-timeOffset);
                }
                var role = new Role(siteSettings.SiteId, "Authenticated Users");
                List<SiteUserEx> siteUserPage = SiteUserEx.GetUsersByRole(siteSettings.SiteId, role.RoleId, fromDate, toDate, 1, 50000);
                DataTable dt = new DataTable();
                dt.Columns.Add("Email", typeof(String));
                dt.Columns.Add("Họ tên", typeof(String));
                dt.Columns.Add("Điện thoại", typeof(String));
                dt.Columns.Add("Địa chỉ", typeof(String));
                dt.Columns.Add("Sinh nhật", typeof(String));
                dt.Columns.Add("Điểm", typeof(String));
                dt.Columns.Add("Tổng tiền đã mua", typeof(String));
                dt.Columns.Add("Ngày đăng ký", typeof(String));

                foreach (SiteUserEx siteUser in siteUserPage)
                {
                    DataRow row = dt.NewRow();
                    row["Email"] = siteUser.Email;
                    row["Họ tên"] = siteUser.FirstName;
                    row["Điện thoại"] = siteUser.LoginName;
                    row["Địa chỉ"] = siteUser.State;
                    row["Sinh nhật"] = siteUser.Yahoo;
                    row["Điểm"] = siteUser.TotalPostsEx;
                    row["Tổng tiền đã mua"] = Convert.ToDouble(siteUser.TotalRevenueEx);
                    row["Ngày đăng ký"] = DateTimeHelper.Format(siteUser.DateCreatedEx, timeZone, "d", timeOffset);

                    dt.Rows.Add(row);
                }

                ExportHelper.ExportDataTableToExcel(System.Web.HttpContext.Current, dt, "memberlist-" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void grid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var role = new Role(siteSettings.SiteId, "Authenticated Users");
            string keyword = string.Empty;
            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (dpFromDate.SelectedDate.HasValue)
            {
                var localTime = new DateTime(dpFromDate.SelectedDate.Value.Year, dpFromDate.SelectedDate.Value.Month, dpFromDate.SelectedDate.Value.Day, 0, 0, 0);

                if (timeZone != null)
                    fromDate = localTime.ToUtc(timeZone);
                else
                    fromDate = localTime.AddHours(-timeOffset);
            }
            if (dpToDate.SelectedDate.HasValue)
            {
                var localTime = new DateTime(dpToDate.SelectedDate.Value.Year, dpToDate.SelectedDate.Value.Month, dpToDate.SelectedDate.Value.Day, 23, 59, 59);

                if (timeZone != null)
                    toDate = localTime.ToUtc(timeZone);
                else
                    toDate = localTime.AddHours(-timeOffset);
            }

            if (!string.IsNullOrEmpty(txtTitle.Text))
            {
                keyword = txtTitle.Text.Trim();
            }

            bool isApplied = false;
            int iCount = SiteUserEx.GetCountByRoleAddLoginName(siteSettings.SiteId, role.RoleId, fromDate, toDate, keyword);
            int startRowIndex = isApplied ? 1 : grid.CurrentPageIndex + 1;
            int maximumRows = isApplied ? iCount : grid.PageSize;

            grid.VirtualItemCount = iCount;
            grid.AllowCustomPaging = !isApplied;

            grid.DataSource = SiteUserEx.GetUsersByRoleAddLoginName(siteSettings.SiteId, role.RoleId, fromDate, toDate, startRowIndex, maximumRows, keyword);
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            grid.Rebind();
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

        protected void btnChange_Click(object sender, EventArgs e)
        {
            var role = new Role(siteSettings.SiteId, "Authenticated Users");
            List<SiteUserEx> lstUser = SiteUserEx.GetAllUserAuthenticated(siteSettings.SiteId, role.RoleId);

            foreach (SiteUserEx user in lstUser)
            {
                var test = user.TotalRevenueEx;
                List<Order> lstOrder = Order.GetPage(siteSettings.SiteId, -1, 2, -1, -1, null, null, null, null, user.UserGuidEx, null, 1, 1000);
                int point = 0;
                foreach (Order order in lstOrder)
                {
                    point += Convert.ToInt32(order.OrderTotal / 1000);
                }
                user.TotalPostsEx = point;
            }
        }
    }
}

