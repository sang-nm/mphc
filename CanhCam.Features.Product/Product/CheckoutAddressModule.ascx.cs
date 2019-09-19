/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-07-24
/// Last Modified:			2015-07-24

using System;
using CanhCam.Web.Framework;
using System.Xml;
using System.Collections.Generic;
using System.Collections;
using Resources;
using CanhCam.Business;

namespace CanhCam.Web.ProductUI
{
    // Feature Guid: ae721338-5b7b-4852-abbc-2bdef66c2fcc
    public partial class CheckoutAddressModule : SiteModuleControl
    {
        private CheckoutAddressConfiguration config = null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(Page_Load);
            this.EnableViewState = false;
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (CartHelper.GetShoppingCart(SiteId, ShoppingCartTypeEnum.ShoppingCart).Count == 0)
            {
                CartHelper.SetupRedirectToCartPage(this);
                return;
            }

            LoadSettings();
            PopulateControls();
        }

        private void PopulateControls()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<CheckoutAddress></CheckoutAddress>");
            XmlElement root = doc.DocumentElement;

            XmlHelper.AddNode(doc, root, "ModuleTitle", this.Title);
            XmlHelper.AddNode(doc, root, "ZoneTitle", CurrentZone.Name);
            if (ModuleConfiguration.ResourceFileDef.Length > 0 && ModuleConfiguration.ResourceKeyDef.Length > 0)
            {
                List<string> lstResourceKeys = ModuleConfiguration.ResourceKeyDef.SplitOnCharAndTrim(';');

                foreach (string item in lstResourceKeys)
                {
                    XmlHelper.AddNode(doc, root, item, ResourceHelper.GetResourceString(ModuleConfiguration.ResourceFileDef, item));
                }
            }

            XmlHelper.AddNode(doc, root, "FullNameText", ProductResources.CheckoutAddressFullName);
            XmlHelper.AddNode(doc, root, "FirstNameText", ProductResources.CheckoutAddressFirstName);
            XmlHelper.AddNode(doc, root, "LastNameText", ProductResources.CheckoutAddressLastName);
            XmlHelper.AddNode(doc, root, "EmailText", ProductResources.CheckoutAddressEmail);
            XmlHelper.AddNode(doc, root, "AddressText", ProductResources.CheckoutAddress);
            XmlHelper.AddNode(doc, root, "PhoneText", ProductResources.CheckoutAddressPhone);
            XmlHelper.AddNode(doc, root, "MobileText", ProductResources.CheckoutAddressMobile);
            XmlHelper.AddNode(doc, root, "FaxText", ProductResources.CheckoutAddressFax);
            XmlHelper.AddNode(doc, root, "StreetText", ProductResources.CheckoutAddressStreet);
            XmlHelper.AddNode(doc, root, "WardText", ProductResources.CheckoutAddressWard);
            XmlHelper.AddNode(doc, root, "DistrictText", ProductResources.CheckoutAddressDistrict);
            XmlHelper.AddNode(doc, root, "ProvinceText", ProductResources.CheckoutAddressProvince);
            XmlHelper.AddNode(doc, root, "CountryText", ProductResources.CheckoutAddressCountry);
            XmlHelper.AddNode(doc, root, "ContinueText", ProductResources.CheckoutContinue);
            XmlHelper.AddNode(doc, root, "SelectProvinceText", ProductResources.CheckoutSelectProvince);
            XmlHelper.AddNode(doc, root, "SelectDistrictText", ProductResources.CheckoutSelectDistrict);
            XmlHelper.AddNode(doc, root, "ContinueShoppingText", Resources.ProductResources.CartContinueShoppingLabel);
            XmlHelper.AddNode(doc, root, "ContinueShoppingUrl", CartHelper.GetContinueShopping());

            if (config.CheckoutNextZoneId > 0)
                XmlHelper.AddNode(doc, root, "NextPageUrl", CartHelper.GetZoneUrl(config.CheckoutNextZoneId));

            Order order = CartHelper.GetOrderSession(siteSettings.SiteId);
            Guid provinceGuid = Guid.Empty;
            Guid districtGuid = Guid.Empty;
            Guid shippingProvinceGuid = Guid.Empty;
            Guid shippingDistrictGuid = Guid.Empty;
            if (order != null)
            {
                // Billing address
                XmlHelper.AddNode(doc, root, "FirstName", order.BillingFirstName);
                XmlHelper.AddNode(doc, root, "LastName", order.BillingLastName);
                XmlHelper.AddNode(doc, root, "Email", order.BillingEmail);
                XmlHelper.AddNode(doc, root, "Address", order.BillingAddress);
                XmlHelper.AddNode(doc, root, "Phone", order.BillingPhone);
                XmlHelper.AddNode(doc, root, "Mobile", order.BillingMobile);
                XmlHelper.AddNode(doc, root, "Fax", order.BillingFax);
                XmlHelper.AddNode(doc, root, "Street", order.BillingStreet);
                XmlHelper.AddNode(doc, root, "Ward", order.BillingWard);
                XmlHelper.AddNode(doc, root, "DistrictGuid", order.BillingDistrictGuid.ToString());

                if (order.BillingProvinceGuid != Guid.Empty)
                {
                    provinceGuid = order.BillingProvinceGuid;
                    XmlHelper.AddNode(doc, root, "ProvinceGuid", order.BillingProvinceGuid.ToString());
                    GeoZone geoZone = new GeoZone(provinceGuid);
                    if (geoZone != null && geoZone.Guid != Guid.Empty)
                        XmlHelper.AddNode(doc, root, "Province", geoZone.Name);
                }
                if (order.BillingDistrictGuid != Guid.Empty)
                {
                    districtGuid = order.BillingDistrictGuid;
                    XmlHelper.AddNode(doc, root, "DistrictGuid", order.BillingDistrictGuid.ToString());
                    GeoZone geoZone = new GeoZone(districtGuid);
                    if (geoZone != null && geoZone.Guid != Guid.Empty)
                        XmlHelper.AddNode(doc, root, "District", geoZone.Name);
                }

                // Shipping address
                XmlHelper.AddNode(doc, root, "ShippingFirstName", order.ShippingFirstName);
                XmlHelper.AddNode(doc, root, "ShippingLastName", order.ShippingLastName);
                XmlHelper.AddNode(doc, root, "ShippingEmail", order.ShippingEmail);
                XmlHelper.AddNode(doc, root, "ShippingAddress", order.ShippingAddress);
                XmlHelper.AddNode(doc, root, "ShippingPhone", order.ShippingPhone);
                XmlHelper.AddNode(doc, root, "ShippingMobile", order.ShippingMobile);
                XmlHelper.AddNode(doc, root, "ShippingFax", order.ShippingFax);
                XmlHelper.AddNode(doc, root, "ShippingStreet", order.ShippingStreet);
                XmlHelper.AddNode(doc, root, "ShippingWard", order.ShippingWard);
                XmlHelper.AddNode(doc, root, "ShippingDistrictGuid", order.ShippingDistrictGuid.ToString());
                XmlHelper.AddNode(doc, root, "OrderNote", order.OrderNote);

                if (order.ShippingProvinceGuid != Guid.Empty)
                {
                    shippingProvinceGuid = order.ShippingProvinceGuid;
                    XmlHelper.AddNode(doc, root, "ShippingProvinceGuid", order.ShippingProvinceGuid.ToString());
                    GeoZone geoZone = new GeoZone(shippingProvinceGuid);
                    if (geoZone != null && geoZone.Guid != Guid.Empty)
                        XmlHelper.AddNode(doc, root, "ShippingProvince", geoZone.Name);
                }
                if (order.ShippingDistrictGuid != Guid.Empty)
                {
                    shippingDistrictGuid = order.ShippingDistrictGuid;
                    XmlHelper.AddNode(doc, root, "ShippingDistrictGuid", order.ShippingDistrictGuid.ToString());
                    GeoZone geoZone = new GeoZone(shippingDistrictGuid);
                    if (geoZone != null && geoZone.Guid != Guid.Empty)
                        XmlHelper.AddNode(doc, root, "ShippingDistrict", geoZone.Name);
                }
            }
            else if (Request.IsAuthenticated)
            {
                SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
                if (siteUser != null)
                {
                    // Billing address
                    XmlHelper.AddNode(doc, root, "FirstName", siteUser.FirstName);
                    XmlHelper.AddNode(doc, root, "LastName", siteUser.LastName);
                    XmlHelper.AddNode(doc, root, "Email", siteUser.Email);
                    XmlHelper.AddNode(doc, root, "Address", siteUser.State);
                    double phone = 0;
                    if (double.TryParse(siteUser.LoginName, out phone))
                        XmlHelper.AddNode(doc, root, "Phone", siteUser.LoginName);
                    else
                        XmlHelper.AddNode(doc, root, "Phone", string.Empty);
                    //XmlHelper.AddNode(doc, root, "Mobile", siteUser.GetCustomPropertyAsString("Mobile"));
                    //XmlHelper.AddNode(doc, root, "Fax", siteUser.GetCustomPropertyAsString("Fax"));
                    //XmlHelper.AddNode(doc, root, "Street", siteUser.GetCustomPropertyAsString("Street"));
                    //XmlHelper.AddNode(doc, root, "Ward", siteUser.GetCustomPropertyAsString("Ward"));

                    string userProvinceGuid = siteUser.ICQ;
                    string userDistrictGuid = siteUser.AIM;

                    XmlHelper.AddNode(doc, root, "DistrictGuid", userDistrictGuid);

                    if (userProvinceGuid.Length == 36)
                    {
                        provinceGuid = new Guid(userProvinceGuid);
                        XmlHelper.AddNode(doc, root, "ProvinceGuid", userProvinceGuid);
                        GeoZone geoZone = new GeoZone(provinceGuid);
                        if (geoZone != null && geoZone.Guid != Guid.Empty)
                            XmlHelper.AddNode(doc, root, "Province", geoZone.Name);
                    }
                    if (userDistrictGuid.Length == 36)
                    {
                        districtGuid = new Guid(userDistrictGuid);
                        XmlHelper.AddNode(doc, root, "DistrictGuid", districtGuid.ToString());
                        GeoZone geoZone = new GeoZone(districtGuid);
                        if (geoZone != null && geoZone.Guid != Guid.Empty)
                            XmlHelper.AddNode(doc, root, "District", geoZone.Name);
                    }

                    //// Shipping address
                    //XmlHelper.AddNode(doc, root, "ShippingFirstName", siteUser.GetCustomPropertyAsString("ShippingFirstName"));
                    //XmlHelper.AddNode(doc, root, "ShippingLastName", siteUser.GetCustomPropertyAsString("ShippingLastName"));
                    //XmlHelper.AddNode(doc, root, "ShippingEmail", siteUser.GetCustomPropertyAsString("ShippingEmail"));
                    //XmlHelper.AddNode(doc, root, "ShippingAddress", siteUser.GetCustomPropertyAsString("ShippingAddress"));
                    //XmlHelper.AddNode(doc, root, "ShippingPhone", siteUser.GetCustomPropertyAsString("ShippingPhone"));
                    //XmlHelper.AddNode(doc, root, "ShippingMobile", siteUser.GetCustomPropertyAsString("ShippingMobile"));
                    //XmlHelper.AddNode(doc, root, "ShippingFax", siteUser.GetCustomPropertyAsString("ShippingFax"));
                    //XmlHelper.AddNode(doc, root, "ShippingStreet", siteUser.GetCustomPropertyAsString("ShippingStreet"));
                    //XmlHelper.AddNode(doc, root, "ShippingWard", siteUser.GetCustomPropertyAsString("ShippingWard"));

                    //userProvinceGuid = siteUser.GetCustomPropertyAsString("ShippingProvince");
                    //userDistrictGuid = siteUser.GetCustomPropertyAsString("ShippingDistrict");

                    //XmlHelper.AddNode(doc, root, "ShippingDistrictGuid", userDistrictGuid);

                    //if (userProvinceGuid.Length == 36)
                    //{
                    //    provinceGuid = new Guid(userProvinceGuid);
                    //    XmlHelper.AddNode(doc, root, "ShippingProvinceGuid", userProvinceGuid);
                    //    GeoZone geoZone = new GeoZone(provinceGuid);
                    //    if (geoZone != null && geoZone.Guid != Guid.Empty)
                    //        XmlHelper.AddNode(doc, root, "ShippingProvince", geoZone.Name);
                    //}
                    //if (userDistrictGuid.Length == 36)
                    //{
                    //    districtGuid = new Guid(userDistrictGuid);
                    //    XmlHelper.AddNode(doc, root, "ShippingDistrictGuid", districtGuid.ToString());
                    //    GeoZone geoZone = new GeoZone(districtGuid);
                    //    if (geoZone != null && geoZone.Guid != Guid.Empty)
                    //        XmlHelper.AddNode(doc, root, "ShippingDistrict", geoZone.Name);
                    //}
                }
            }

            RenderProvinces(doc, root, provinceGuid, shippingProvinceGuid);
            if (provinceGuid != Guid.Empty)
                RenderDistricts(doc, root, provinceGuid, districtGuid, "Districts");
            if (shippingProvinceGuid != Guid.Empty)
                RenderDistricts(doc, root, shippingProvinceGuid, shippingDistrictGuid, "ShippingDistricts");

            XmlHelper.XMLTransform(xmlTransformer, SiteUtils.GetXsltBasePath("product", ModuleConfiguration.XsltFileName), doc);
        }

        private void RenderProvinces(XmlDocument doc, XmlElement root, Guid guid, Guid shippingProvinceGuid)
        {
            var lstProvinces = GeoZone.GetByCountry(siteSettings.DefaultCountryGuid, 1, WorkingCulture.LanguageId);
            foreach (GeoZone province in lstProvinces)
            {
                XmlElement provinceXml = doc.CreateElement("Provinces");
                root.AppendChild(provinceXml);

                XmlHelper.AddNode(doc, provinceXml, "Title", province.Name);
                XmlHelper.AddNode(doc, provinceXml, "Guid", province.Guid.ToString());

                if (province.Guid == guid)
                    XmlHelper.AddNode(doc, provinceXml, "IsActive", "true");
                if (province.Guid == shippingProvinceGuid)
                    XmlHelper.AddNode(doc, provinceXml, "ShippingIsActive", "true");
            }
        }

        private void RenderDistricts(XmlDocument doc, XmlElement root, Guid provinceGuid, Guid guid, string elementName)
        {
            var lstDistricts = GeoZone.GetByListParent(provinceGuid.ToString(), 1, WorkingCulture.LanguageId);
            foreach (GeoZone district in lstDistricts)
            {
                XmlElement provinceXml = doc.CreateElement(elementName);
                root.AppendChild(provinceXml);

                XmlHelper.AddNode(doc, provinceXml, "Title", district.Name);
                XmlHelper.AddNode(doc, provinceXml, "Guid", district.Guid.ToString());

                if (district.Guid == guid)
                    XmlHelper.AddNode(doc, provinceXml, "IsActive", "true");
            }
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
                config = new CheckoutAddressConfiguration(Settings);
        }
    }
}

namespace CanhCam.Web.ProductUI
{
    public class CheckoutAddressConfiguration
    {
        public CheckoutAddressConfiguration()
        { }

        public CheckoutAddressConfiguration(Hashtable settings)
        {
            LoadSettings(settings);
        }

        private void LoadSettings(Hashtable settings)
        {
            if (settings == null || settings.Count == 0) { return; throw new ArgumentException("must pass in a hashtable of settings"); }

            checkoutNextZoneId = WebUtils.ParseInt32FromHashtable(settings, "CheckoutNextPageUrl", checkoutNextZoneId);
        }

        private int checkoutNextZoneId = -1;
        public int CheckoutNextZoneId
        {
            get { return checkoutNextZoneId; }
        }

    }
}