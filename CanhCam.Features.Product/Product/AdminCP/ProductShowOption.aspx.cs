/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-08-25
/// Last Modified:			2014-08-25

using System;
using log4net;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using Telerik.Web.UI;
using CanhCam.Web.Framework;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace CanhCam.Web.ProductUI
{
    public partial class ProductShowOptionPage : CmsNonBasePage
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ProductShowOptionPage));
        RadGridSEOPersister gridPersister1;
        RadGridSEOPersister gridPersister2;
        
        private int option = -1;
        private bool canEditAnything = false;
        private int productType = 0;

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                SiteUtils.RedirectToLoginPage(this);
                return;
            }

            LoadSettings();
            
            if (!canEditAnything)
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            PopulateLabels();

            if (!Page.IsPostBack)
                PopulateControls();
        }

        #endregion

        #region "RadGrid Event"

        void grid1_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grid1.PagerStyle.EnableSEOPaging = false;

            bool isApplied = gridPersister1.IsAppliedSortFilterOrGroup;
            int iCount = Product.GetCountBySearch(siteSettings.SiteId, ListZoneId, -1, -1, -1, -1, null, null, -1, -1, null, null, txtTitle.Text.Trim());
            int startRowIndex = isApplied ? 1 : grid1.CurrentPageIndex + 1;
            int maximumRows = isApplied ? iCount : grid1.PageSize;

            grid1.VirtualItemCount = iCount;
            grid1.AllowCustomPaging = !isApplied;

            grid1.DataSource = Product.GetPageBySearch(startRowIndex, maximumRows, siteSettings.SiteId, ListZoneId, -1, -1, -1, -1, null, null, -1, -1, null, null, txtTitle.Text.Trim());
        }

        void grid2_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grid2.PagerStyle.EnableSEOPaging = false;

            int showOption = -1;
            int.TryParse(ddlShowOption.SelectedValue, out showOption);

            if (showOption == -1)
            {
                grid2.DataSource = new List<Product>();
                return;
            }

            bool isApplied = gridPersister2.IsAppliedSortFilterOrGroup;
            int iCount = Product.GetCountBySearch(siteSettings.SiteId, ListZoneId, -1, -1, -1, -1, null, null, -1, showOption, null, null, txtTitle.Text.Trim());
            int startRowIndex = isApplied ? 1 : grid2.CurrentPageIndex + 1;
            int maximumRows = isApplied ? iCount : grid2.PageSize;

            grid2.VirtualItemCount = iCount;
            grid2.AllowCustomPaging = !isApplied;

            grid2.DataSource = Product.GetPageBySearch(startRowIndex, maximumRows, siteSettings.SiteId, ListZoneId, -1, -1, -1, -1, null, null, -1, showOption, null, null, txtTitle.Text.Trim());
        }

        private string ListZoneId
        {
            get
            {
                if (ddZones.SelectedValue.Length > 0)
                {
                    if (ddZones.SelectedValue == "-1")
                        return "";

                    return ddZones.SelectedValue;
                }

                return "0";
            }
        }

        #endregion

        #region Event

        void btnLeft_Click(object sender, EventArgs e)
        {
            UpdateNewsPosition(false);
        }

        void btnRight_Click(object sender, EventArgs e)
        {
            UpdateNewsPosition(true);
        }

        private bool UpdateNewsPosition(bool moveRight)
        {
            if (ddlShowOption.SelectedValue.Length == 0)
                return false;

            bool isUpdated = false;
            if (moveRight)
            {
                foreach (GridDataItem data in grid1.SelectedItems)
                {
                    int productId = Convert.ToInt32(data.GetDataKeyValue("ProductId"));
                    int showOption = Convert.ToInt32(data.GetDataKeyValue("ShowOption"));

                    int showOptionNew = showOption;
                    int.TryParse(ddlShowOption.SelectedValue, out showOptionNew);

                    if ((showOption & showOptionNew) == 0)
                    {
                        Product product = new Product(SiteId, productId);

                        if (product != null && product.ProductId > 0)
                        {
                            product.ShowOption = (product.ShowOption | showOptionNew);
                            if (product.ShowOption < 0)
                                product.ShowOption = 0;

                            if (product.Save())
                            {
                                LogActivity.Write("Change product showoption", product.Title);

                                isUpdated = true;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (GridDataItem data in grid2.SelectedItems)
                {
                    int productId = Convert.ToInt32(data.GetDataKeyValue("ProductId"));
                    int showOption = Convert.ToInt32(data.GetDataKeyValue("ShowOption"));

                    int showOptionNew = showOption;
                    int.TryParse(ddlShowOption.SelectedValue, out showOptionNew);

                    if ((showOption & showOptionNew) > 0)
                    {
                        Product product = new Product(SiteId, productId);

                        if (product != null && product.ProductId > 0)
                        {
                            product.ShowOption = (product.ShowOption - showOptionNew);
                            if (product.ShowOption < 0)
                                product.ShowOption = 0;

                            if (product.Save())
                            {
                                LogActivity.Write("Change product showoption", product.Title);

                                isUpdated = true;
                            }
                        }
                    }
                }
            }

            if (isUpdated)
            {
                grid2.Rebind();

                message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "UpdateSuccessMessage");

                return true;
            }

            return false;
        }
        
        void btnSearch_Click(object sender, EventArgs e)
        {
            grid1.Rebind();
            grid2.Rebind();
        }

        //void ddZones_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    grid1.Rebind();
        //}

        void ddlPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            grid2.Rebind();
        }

        #endregion

        #region Protected methods

        #endregion

        #region Populate

        private void PopulateLabels()
        {
            heading.Text = Resources.ProductResources.ProductShowOptionTilte;
            Page.Title = SiteUtils.FormatPageTitle(siteSettings, heading.Text);

            //btnRight.ImageUrl = "~/Data/SiteImages/rt2.gif";
            //btnLeft.ImageUrl = "~/Data/SiteImages/lt2.gif";
        }

        private void PopulateControls()
        {
            List<EnumDefined> list = EnumDefined.LoadFromConfigurationXml("product", "showoption", "value", "name");
            ddlShowOption.DataSource = list;
            ddlShowOption.DataBind();

            ListItem li = ddlShowOption.Items.FindByValue(option.ToString());
            if (li != null)
            {
                ddlShowOption.ClearSelection();
                li.Selected = true;
            }

            PopulateZoneList();
        }

        #endregion

        #region Populate Zone List

        private void PopulateZoneList()
        {
            gbSiteMapProvider.PopulateListControl(ddZones, false, Product.FeatureGuid);

            if (canEditAnything)
                ddZones.Items.Insert(0, new ListItem(ResourceHelper.GetResourceString("Resource", "All"), "-1"));
        }

        #endregion

        #region LoadSettings

        private void LoadSettings()
        {
            productType = WebUtils.ParseInt32FromQueryString("type", productType);
            canEditAnything = WebUser.IsAdminOrContentAdmin && SiteUtils.UserIsSiteEditor();
            option = WebUtils.ParseInt32FromQueryString("option", option);
        }

        #endregion

        #region OnInit

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Load += new EventHandler(this.Page_Load);

            btnLeft.Click += new EventHandler(btnLeft_Click);
            btnRight.Click += new EventHandler(btnRight_Click);

            grid1.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grid1_NeedDataSource);
            grid2.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grid2_NeedDataSource);

            //ddZones.SelectedIndexChanged += new EventHandler(ddZones_SelectedIndexChanged);
            btnSearch.Click += new EventHandler(btnSearch_Click);
            ddlShowOption.SelectedIndexChanged += new EventHandler(ddlPosition_SelectedIndexChanged);

            gridPersister1 = new RadGridSEOPersister(grid1);
            gridPersister2 = new RadGridSEOPersister(grid2);
        }

        #endregion

    }
}
