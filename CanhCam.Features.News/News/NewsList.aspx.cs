/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2013-02-21
/// Last Modified:			2014-07-04

using log4net;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.SearchIndex;
using CanhCam.Web.Framework;
using Resources;
using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace CanhCam.Web.NewsUI
{
    public partial class NewsListPage : CmsNonBasePage
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(NewsListPage));
        RadGridSEOPersister gridPersister;

        private SiteSettings siteSettings;
        private Double timeOffset = 0;
        private TimeZoneInfo timeZone = null;
        private bool canEditAnything = false;
        private bool canUpdate = false;

        private SiteUser currentUser = null;
        private string startZone = string.Empty;

        private int newsType = 0;

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                SiteUtils.RedirectToLoginPage(this);
                return;
            }

            LoadParams();
            LoadSettings();

            if (!NewsPermission.CanViewList)
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            PopulateLabels();
            SetupWorkflow();

            if (!Page.IsPostBack)
                PopulateControls();
        }

        #endregion

        #region "RadGrid Event"

        void grid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grid.PagerStyle.EnableSEOPaging = false;

            DateTime? startDateFrom = null;
            DateTime? startDateTo = null;
            DateTime? endDateFrom = null;
            DateTime? endDateTo = null;
            int isPublished = -1;

            if (dpStartDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpStartDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0);

                if (timeZone != null)
                    startDateFrom = localTime.ToUtc(timeZone);
                else
                    startDateFrom = localTime.AddHours(-timeOffset);
            }
            if (dpEndDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpEndDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 23, 59, 59);

                if (timeZone != null)
                    startDateTo = localTime.ToUtc(timeZone);
                else
                    startDateTo = localTime.AddHours(-timeOffset);
            }

            bool isApplied = gridPersister.IsAppliedSortFilterOrGroup;
            int iCount = News.GetCountBySearch(siteSettings.SiteId, sZoneId, newsType, isPublished, ddStates.SelectedValue, -1, -1, -1, startDateFrom, startDateTo, endDateFrom, endDateTo, null, txtTitle.Text.Trim());
            int startRowIndex = isApplied ? 1 : grid.CurrentPageIndex + 1;
            int maximumRows = isApplied ? iCount : grid.PageSize;

            grid.VirtualItemCount = iCount;
            grid.AllowCustomPaging = !isApplied;

            grid.DataSource = News.GetPageBySearch(siteSettings.SiteId, sZoneId, newsType, isPublished, ddStates.SelectedValue, -1, -1, -1, startDateFrom, startDateTo, endDateFrom, endDateTo, null, txtTitle.Text.Trim(), startRowIndex, maximumRows);
        }

        private string sZoneId
        {
            get
            {
                if (ddZones.SelectedValue.Length > 0)
                {
                    if (ddZones.SelectedValue == "-1")
                        return string.Empty;

                    if (ddZones.SelectedValue == "0")
                    {
                        string result = "0";
                        foreach (ListItem li in ddZones.Items)
                        {
                            if (Convert.ToInt32(li.Value) > 0)
                                result += ";" + li.Value;
                        }

                        return result;
                    }

                    return ddZones.SelectedValue;
                }

                return "0";
            }
        }

        #endregion

        #region Event

        void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!NewsPermission.CanDelete)
                {
                    SiteUtils.RedirectToEditAccessDeniedPage();
                    return;
                }

                bool isDeleted = false;

                foreach (GridDataItem data in grid.SelectedItems)
                {
                    int newsId = Convert.ToInt32(data.GetDataKeyValue("NewsID"));
                    News news = new News(SiteId, newsId);

                    if (news != null && news.NewsID != -1 && !news.IsDeleted)
                    {
                        ContentDeleted.Create(siteSettings.SiteId, news.Title, "News", typeof(NewsDeleted).AssemblyQualifiedName, news.NewsID.ToString(), Page.User.Identity.Name);

                        news.IsDeleted = true;

                        news.ContentChanged += new ContentChangedEventHandler(news_ContentChanged);

                        news.SaveDeleted();
                        LogActivity.Write("Delete news", news.Title);

                        isDeleted = true;
                    }
                }

                if (isDeleted)
                {
                    SiteUtils.QueueIndexing();
                    grid.Rebind();

                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "DeleteSuccessMessage");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void news_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            IndexBuilderProvider indexBuilder = IndexBuilderManager.Providers["NewsIndexBuilderProvider"];
            if (indexBuilder != null)
            {
                indexBuilder.ContentChangedHandler(sender, e);
            }
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!NewsPermission.CanUpdate)
                {
                    SiteUtils.RedirectToEditAccessDeniedPage();
                    return;
                }

                bool isUpdated = false;
                foreach (GridDataItem data in grid.Items)
                {
                    TextBox txtDisplayOrder = (TextBox)data.FindControl("txtDisplayOrder");
                    TextBox txtViewed = (TextBox)data.FindControl("txtViewed");
                    int newsId = Convert.ToInt32(data.GetDataKeyValue("NewsID"));
                    int displayOrder = Convert.ToInt32(data.GetDataKeyValue("DisplayOrder"));
                    int viewed = Convert.ToInt32(data.GetDataKeyValue("Viewed"));

                    int displayOrderNew = displayOrder;
                    int.TryParse(txtDisplayOrder.Text, out displayOrderNew);

                    int viewedNew = viewed;
                    int.TryParse(txtViewed.Text, out viewedNew);

                    if (displayOrder != displayOrderNew || viewed != viewedNew)
                    {
                        News objNews = new News(SiteId, newsId);
                        if (objNews != null && objNews.NewsID != -1)
                        {
                            objNews.DisplayOrder = displayOrderNew;
                            objNews.Viewed = viewedNew;
                            objNews.Save();

                            LogActivity.Write("Resort news", objNews.Title);

                            isUpdated = true;
                        }
                    }
                }

                if (isUpdated)
                {
                    grid.Rebind();

                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "UpdateSuccessMessage");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            grid.Rebind();
        }

        #endregion

        #region Protected methods

        protected string FormatDate(object startDate)
        {
            if (startDate == null)
                return string.Empty;

            string timeFormat = NewsResources.DateTimeFormat;

            if (timeZone != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(startDate), timeZone).ToString(timeFormat);
            }

            return Convert.ToDateTime(startDate).AddHours(timeOffset).ToString(timeFormat);
        }

        protected bool CanEditNews(int userID, bool isPublished, object oStateId)
        {
            // Should be check permission zone???
            if (canUpdate)
                return true;

            if (WebConfigSettings.EnableContentWorkflow && siteSettings.EnableContentWorkflow && !isPublished)
            {
                if (oStateId != null)
                {
                    int stateId = Convert.ToInt32(oStateId);

                    if (stateId == firstWorkflowStateId)
                    {
                        if (currentUser == null) { return false; }
                        return (userID == currentUser.UserId);
                    }
                }
            }

            return false;
        }

        #endregion

        #region Populate

        private void PopulateLabels()
        {
            heading.Text = NewsHelper.GetNameByNewsType(newsType, NewsResources.NewsListFormat, NewsResources.NewsList);
            Page.Title = SiteUtils.FormatPageTitle(siteSettings, heading.Text);

            breadcrumb.CurrentPageTitle = heading.Text;
            breadcrumb.CurrentPageUrl = GetNewsListBreadCrumb();

            UIHelper.DisableButtonAfterClick(
                btnUpdate,
                NewsResources.ButtonDisabledPleaseWait,
                Page.ClientScript.GetPostBackEventReference(this.btnUpdate, string.Empty)
                );

            UIHelper.AddConfirmationDialog(btnDelete, NewsResources.NewsDeleteMultiWarning);
        }

        private string GetNewsListBreadCrumb()
        {
            if (newsType > 0)
                return "~/News/NewsList.aspx?type=" + newsType.ToString();

            return "~/News/NewsList.aspx";
        }

        private void PopulateControls()
        {
            PopulateZoneList();
        }

        #endregion

        #region Populate Zone List

        private void PopulateZoneList()
        {
            gbSiteMapProvider.PopulateListControl(ddZones, false, News.FeatureGuid);

            if (canEditAnything)
                ddZones.Items.Insert(0, new ListItem(ResourceHelper.GetResourceString("Resource", "All"), "-1"));
            else if (ddZones.Items.Count > 1)
                ddZones.Items.Insert(0, new ListItem(ResourceHelper.GetResourceString("Resource", "All"), "0"));

            if (startZone.Length > 0)
            {
                ListItem li = ddZones.Items.FindByValue(startZone);
                if (li != null)
                {
                    ddZones.ClearSelection();
                    li.Selected = true;
                }
            }
        }

        #endregion

        #region LoadSettings

        private void LoadSettings()
        {
            lnkInsert.Visible = NewsPermission.CanCreate;
            btnUpdate.Visible = NewsPermission.CanUpdate;
            btnDelete.Visible = NewsPermission.CanDelete;

            currentUser = SiteUtils.GetCurrentSiteUser();
            siteSettings = CacheHelper.GetCurrentSiteSettings();

            canEditAnything = WebUser.IsAdminOrContentAdmin || SiteUtils.UserIsSiteEditor();
            canUpdate = NewsPermission.CanUpdate;

            string param = "?start=";
            lnkInsert.NavigateUrl = SiteRoot + "/News/NewsEdit.aspx";
            if (newsType > 0)
            {
                lnkInsert.NavigateUrl += "?type=" + newsType.ToString();
                param = "&start=";
            }

            if (
                ddZones.SelectedValue != "-1"
                && ddZones.SelectedValue != "0"
                && ddZones.SelectedValue.Length > 0
                )
            {
                lnkInsert.NavigateUrl += param + ddZones.SelectedValue;
            }
            else
            {
                if (startZone.Length > 0)
                    lnkInsert.NavigateUrl += param + startZone.ToString();
            }
        }

        #endregion

        #region LoadParams

        private void LoadParams()
        {
            startZone = WebUtils.ParseStringFromQueryString("start", startZone);
            newsType = WebUtils.ParseInt32FromQueryString("type", newsType);

            timeOffset = SiteUtils.GetUserTimeOffset();
            timeZone = SiteUtils.GetUserTimeZone();
        }

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);

            btnSearch.Click += new EventHandler(btnSearch_Click);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnDelete.Click += new EventHandler(btnDelete_Click);

            this.grid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grid_NeedDataSource);
            this.grid.ItemDataBound += new GridItemEventHandler(grid_ItemDataBound);

            gridPersister = new RadGridSEOPersister(grid);
        }

        #endregion

        #region Workflow

        private bool workflowIsAvailable = false;
        private int workflowId = -1;
        private int firstWorkflowStateId = -1;
        private int lastWorkflowStateId = -1;

        private void SetupWorkflow()
        {
            divStates.Visible = false;
            grid.MasterTableView.GetColumn("WorkflowCreatedBy").Visible = false;
            grid.MasterTableView.GetColumn("WorkflowState").Visible = false;
            grid.MasterTableView.GetColumn("WorkflowAction").Visible = false;

            if (WebConfigSettings.EnableContentWorkflow && siteSettings.EnableContentWorkflow)
            {
                workflowId = WorkflowHelper.GetWorkflowId(News.FeatureGuid);
                workflowIsAvailable = WorkflowHelper.WorkflowIsAvailable(workflowId);
                if (workflowIsAvailable)
                {
                    firstWorkflowStateId = WorkflowHelper.GetFirstWorkflowStateId(workflowId);
                    lastWorkflowStateId = WorkflowHelper.GetLastWorkflowStateId(workflowId);

                    divStates.Visible = true;
                    grid.MasterTableView.GetColumn("WorkflowCreatedBy").Visible = true;
                    grid.MasterTableView.GetColumn("WorkflowState").Visible = true;
                    grid.MasterTableView.GetColumn("WorkflowAction").Visible = true;

                    if (!Page.IsPostBack)
                    {
                        ddStates.DataSource = WorkflowHelper.GetWorkflowStates(workflowId);
                        ddStates.DataBind();
                        ddStates.Items.Insert(0, new ListItem(ResourceHelper.GetResourceString("Resource", "All"), ""));
                    }
                }
            }
        }

        void grid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int stateId = Convert.ToInt32(item.GetDataKeyValue("StateId"));

                if (stateId == -1)
                    return;

                int newsId = Convert.ToInt32(item.GetDataKeyValue("NewsID"));
                int zoneId = Convert.ToInt32(item.GetDataKeyValue("ZoneID"));
                bool isPublished = Convert.ToBoolean(item.GetDataKeyValue("IsPublished"));

                var ibApproveContent = (ImageButton)item.FindControl("ibApproveContent");
                var lnkRejectContent = (HyperLink)item.FindControl("lnkRejectContent");
                var litWorkflowStatus = (Literal)item.FindControl("litWorkflowStatus");

                lnkRejectContent.NavigateUrl = SiteRoot + "/News/RejectContent.aspx?NewsID=" + newsId.ToInvariantString();
                lnkRejectContent.ImageUrl = Page.ResolveUrl(WebConfigSettings.RejectContentImage);
                lnkRejectContent.ToolTip = NewsResources.RejectContentToolTip;

                ibApproveContent.CommandArgument = newsId.ToInvariantString();
                ibApproveContent.ImageUrl = Page.ResolveUrl(WebConfigSettings.ApproveContentImage);
                ibApproveContent.ToolTip = NewsResources.ApproveContentToolTip;

                bool isReviewRole = false;

                WorkflowState workflowState = WorkflowHelper.GetWorkflowState(workflowId, stateId);
                if (workflowState != null && workflowState.StateId > 0)
                {
                    isReviewRole = (WorkflowHelper.UserHasStatePermission(workflowId, stateId) && UserCanAuthorizeZone(zoneId));

                    litWorkflowStatus.Text = workflowState.StateName;
                }

                if (!isPublished)
                {
                    ibApproveContent.Visible = isReviewRole;
                }

                if (stateId == firstWorkflowStateId)
                {
                    ibApproveContent.ImageUrl = Page.ResolveUrl(WebConfigSettings.RequestApprovalImage);
                    ibApproveContent.ToolTip = NewsResources.RequestApprovalToolTip;
                }
                else
                {
                    lnkRejectContent.Visible = isReviewRole;
                }
            }
        }

        protected void ibApproveContent_Command(object sender, CommandEventArgs e)
        {
            if (currentUser == null)
                return;

            News news = new News(siteSettings.SiteId, Convert.ToInt32(e.CommandArgument));
            if (news == null || news.NewsID == -1 || !news.StateId.HasValue) { return; }

            news.StateId = WorkflowHelper.GetNextWorkflowStateId(workflowId, news.StateId.Value);
            news.ApprovedUserGuid = currentUser.UserGuid;
            news.ApprovedBy = Context.User.Identity.Name.Trim();
            news.ApprovedUtc = DateTime.UtcNow;
            news.RejectedNotes = null;
            bool result = news.SaveState(lastWorkflowStateId);

            if (result)
            {
                if (!WebConfigSettings.DisableWorkflowNotification)
                {
                    NewsHelper.SendApprovalRequestNotification(
                        SiteUtils.GetSmtpSettings(),
                        siteSettings,
                        workflowId,
                        currentUser,
                        news);
                }

                message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "UpdateSuccessMessage");

                grid.Rebind();
            }
        }

        #endregion
    }
}


namespace CanhCam.Web.NewsUI
{
    public class NewsDeleted : IContentDeleted
    {
        public bool RestoreContent(string newsId)
        {
            try
            {
                SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
                News news = new News(siteSettings.SiteId, Convert.ToInt32(newsId));

                if (news != null && news.NewsID != -1)
                {
                    news.IsDeleted = false;

                    news.ContentChanged += new ContentChangedEventHandler(news_ContentChanged);

                    news.SaveDeleted();

                    SiteUtils.QueueIndexing();
                }
            }
            catch (Exception) { return false; }

            return true;
        }

        public bool DeleteContent(string newsId)
        {
            try
            {
                SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
                News news = new News(siteSettings.SiteId, Convert.ToInt32(newsId));

                if (news != null && news.NewsID != -1)
                {
                    NewsHelper.DeleteFolder(siteSettings.SiteId, news.NewsID);

                    ContentMedia.DeleteByContent(news.NewsGuid);

                    var listAtributes = ContentAttribute.GetByContentAsc(news.NewsGuid);
                    foreach (ContentAttribute item in listAtributes)
                    {
                        ContentLanguage.DeleteByContent(item.Guid);
                    }
                    ContentAttribute.DeleteByContent(news.NewsGuid);
                    ContentLanguage.DeleteByContent(news.NewsGuid);

                    news.Delete();
                    FriendlyUrl.DeleteByPageGuid(news.NewsGuid);

                    FileAttachment.DeleteByItem(news.NewsGuid);
                }
            }
            catch (Exception) { return false; }

            return true;
        }

        void news_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            IndexBuilderProvider indexBuilder = IndexBuilderManager.Providers["NewsIndexBuilderProvider"];
            if (indexBuilder != null)
            {
                indexBuilder.ContentChangedHandler(sender, e);
            }
        }

    }
}