/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-28
/// Last Modified:			2014-08-28

using System;
using CanhCam.Web.Framework;
using Resources;
using System.Xml;
using CanhCam.Business;
using System.Collections.Generic;
using CanhCam.Business.WebHelpers;
using System.Collections;

namespace CanhCam.Web.ProductUI
{
    public partial class ProductSpecialModule : SiteModuleControl
    {
        private ProductSpecialConfiguration config = null;
        private Double timeOffset = 0;
        private TimeZoneInfo timeZone = null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(Page_Load);
            this.EnableViewState = false;
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            LoadSettings();
            PopulateControls();
        }

        private void PopulateControls()
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<ProductList></ProductList>");
            XmlElement root = doc.DocumentElement;

            XmlHelper.AddNode(doc, root, "ModuleTitle", this.Title);
            XmlHelper.AddNode(doc, root, "ZoneTitle", CurrentZone.Name);
            XmlHelper.AddNode(doc, root, "ViewMore", ProductResources.ViewMoreLabel);

            if (ModuleConfiguration.ResourceFileDef.Length > 0 && ModuleConfiguration.ResourceKeyDef.Length > 0)
            {
                List<string> lstResourceKeys = ModuleConfiguration.ResourceKeyDef.SplitOnCharAndTrim(';');

                foreach (string item in lstResourceKeys)
                {
                    XmlHelper.AddNode(doc, root, item, ResourceHelper.GetResourceString(ModuleConfiguration.ResourceFileDef, item));
                }
            }

            CmsBasePage basePage = Page as CmsBasePage;
            bool userCanUpdate = ProductPermission.CanUpdate;
            SiteUser currentUser = SiteUtils.GetCurrentSiteUser();

            List<Product> lstProducts = new List<Product>();
            if (ProductConfiguration.RecentlyViewedProductsEnabled && config.Position == 0)
                lstProducts = ProductHelper.GetRecentlyViewedProducts(config.MaxProductsToGet);
            else if (config.ZoneId > -1)
            {
                int zoneId = config.ZoneId;
                if (zoneId == 0)
                    zoneId = CurrentZone.ZoneId;

                string zoneRangeIds = ProductHelper.GetRangeZoneIdsToSemiColonSeparatedString(siteSettings.SiteId, zoneId);
                lstProducts = Product.GetPageBySearch(1, config.MaxProductsToGet, siteSettings.SiteId, zoneRangeIds, 1, WorkingCulture.LanguageId, -1, -1, null, null, config.Position);
            }
            else
                lstProducts = Product.GetPage(SiteId, -1, WorkingCulture.LanguageId, config.Position, 1, config.MaxProductsToGet);

            foreach (Product product in lstProducts)
            {
                XmlElement productXml = doc.CreateElement("Product");
                root.AppendChild(productXml);

                ProductHelper.BuildProductDataXml(doc, productXml, product, timeZone, timeOffset, ProductHelper.BuildEditLink(product, basePage, userCanUpdate, currentUser));
            }

            XmlHelper.XMLTransform(xmlTransformer, SiteUtils.GetXsltBasePath("product", ModuleConfiguration.XsltFileName), doc);
        }

        protected virtual void LoadSettings()
        {
            timeOffset = SiteUtils.GetUserTimeOffset();
            timeZone = SiteUtils.GetUserTimeZone();

            EnsureConfiguration();

            if (this.ModuleConfiguration != null)
            {
                this.Title = ModuleConfiguration.ModuleTitle;
                this.Description = ModuleConfiguration.FeatureName;
            }
        }

        private void EnsureConfiguration()
        {
            if (config == null)
                config = new ProductSpecialConfiguration(Settings);
        }

        public override bool UserHasPermission()
        {
            if (!Request.IsAuthenticated)
                return false;

            bool hasPermission = false;
            EnsureConfiguration();

            if (config.Position > 0)
            {
                if (WebUser.IsAdminOrContentAdmin && SiteUtils.UserIsSiteEditor())
                {
                    this.LiteralExtraMarkup = "<dd><a class='ActionLink chooseproductlink' href='"
                            + SiteRoot
                            + "/Product/AdminCP/ProductSpecial.aspx?pos=" + config.Position.ToString() + "'><i class='fa fa-list'></i> " + ProductResources.ProductSelectLabel + "</a></dd>";

                    hasPermission = true;
                }
            }

            return hasPermission;
        }

    }
}

namespace CanhCam.Web.ProductUI
{
    public class ProductSpecialConfiguration
    {
        public ProductSpecialConfiguration()
        { }

        public ProductSpecialConfiguration(Hashtable settings)
        {
            LoadSettings(settings);
        }

        private void LoadSettings(Hashtable settings)
        {
            if (settings == null || settings.Count == 0) { return; throw new ArgumentException("must pass in a hashtable of settings"); }

            maxProductsToGet = WebUtils.ParseInt32FromHashtable(settings, "MaxProductsToGetSetting", maxProductsToGet);
            position = WebUtils.ParseInt32FromHashtable(settings, "ProductPositionSetting", position);
            zoneId = WebUtils.ParseInt32FromHashtable(settings, "ParentZoneSetting", zoneId);
        }

        private int position = -1;
        public int Position
        {
            get { return position; }
        }

        private int maxProductsToGet = 5;
        public int MaxProductsToGet
        {
            get { return maxProductsToGet; }
        }

        private int zoneId = -1;
        public int ZoneId
        {
            get { return zoneId; }
        }

    }
}