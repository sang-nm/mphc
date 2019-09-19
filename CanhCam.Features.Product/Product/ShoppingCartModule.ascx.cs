/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-07-20
/// Last Modified:			2015-07-20

using System;
using CanhCam.Web.Framework;
using System.Xml;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Globalization;
using CanhCam.Business;

namespace CanhCam.Web.ProductUI
{
    // Feature Guid: 34fc44b5-99ac-4e22-96e9-c5f2778bfdd2
    public partial class ShoppingCartModule : SiteModuleControl
    {
        private ShoppingCartConfiguration config = null;

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
            var doc = new XmlDocument();
            XmlElement root = CartHelper.BuildShoppingCartXml(SiteId, Guid.Empty, out doc);

            XmlHelper.AddNode(doc, root, "ModuleTitle", this.Title);
            XmlHelper.AddNode(doc, root, "ZoneTitle", CurrentZone.Name);

            if (config.CheckoutZoneId > 0)
                XmlHelper.AddNode(doc, root, "CheckoutUrl", CartHelper.GetZoneUrl(config.CheckoutZoneId));

            if (ModuleConfiguration.ResourceFileDef.Length > 0 && ModuleConfiguration.ResourceKeyDef.Length > 0)
            {
                List<string> lstResourceKeys = ModuleConfiguration.ResourceKeyDef.SplitOnCharAndTrim(';');

                foreach (string item in lstResourceKeys)
                {
                    XmlHelper.AddNode(doc, root, item, ResourceHelper.GetResourceString(ModuleConfiguration.ResourceFileDef, item));
                }
            }

            if (!Request.IsAuthenticated)
            {
                string redirectUrl = string.Format(CultureInfo.InvariantCulture, "{0}" + SiteUtils.GetLoginRelativeUrl() + "?returnurl={1}",
                               SiteRoot,
                               HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));

                XmlHelper.AddNode(doc, root, "LoginUrl", redirectUrl);
            }
            else
            {
                int point = 0;
                SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
                if (siteUser != null && siteUser.UserId > 0)
                    point = siteUser.TotalPosts;

                XmlHelper.AddNode(doc, root, "UserPoints", siteUser.TotalPosts.ToString());
                XmlHelper.AddNode(doc, root, "PointDiscount", ProductHelper.FormatPrice(0, true));
            }

            XmlHelper.XMLTransform(xmlTransformer, SiteUtils.GetXsltBasePath("product", ModuleConfiguration.XsltFileName), doc);
        }

        protected virtual void LoadSettings()
        {
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
                config = new ShoppingCartConfiguration(Settings);
        }
    }
}

namespace CanhCam.Web.ProductUI
{
    public class ShoppingCartConfiguration
    {
        public ShoppingCartConfiguration()
        { }

        public ShoppingCartConfiguration(Hashtable settings)
        {
            LoadSettings(settings);
        }

        private void LoadSettings(Hashtable settings)
        {
            if (settings == null || settings.Count == 0) { return; throw new ArgumentException("must pass in a hashtable of settings"); }

            //if (settings["CheckoutPageUrl"] != null)
            //    nextPageUrl = settings["CheckoutPageUrl"].ToString();

            checkoutZoneId = WebUtils.ParseInt32FromHashtable(settings, "CheckoutPageUrl", checkoutZoneId);
        }

        private int checkoutZoneId = -1;
        public int CheckoutZoneId
        {
            get { return checkoutZoneId; }
        }

    }
}