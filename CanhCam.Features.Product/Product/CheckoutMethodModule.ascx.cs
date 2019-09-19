/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-07-30
/// Last Modified:			2015-07-30

using System;
using CanhCam.Web.Framework;
using System.Xml;
using System.Collections.Generic;
using System.Collections;
using Resources;
using CanhCam.Business;

namespace CanhCam.Web.ProductUI
{
    // Feature Guid: c27d7534-f00a-43eb-b7fd-b17e4e9d6e70
    public partial class CheckoutMethodModule : SiteModuleControl
    {
        private CheckoutMethodConfiguration config = null;
        private Order order = null;

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
            doc.LoadXml("<CheckoutMethod></CheckoutMethod>");
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

            XmlHelper.AddNode(doc, root, "CompanyNameText", ProductResources.CheckoutCompanyName);
            XmlHelper.AddNode(doc, root, "CompanyTaxCodeText", ProductResources.CheckoutCompanyTaxCode);
            XmlHelper.AddNode(doc, root, "CompanyAddressText", ProductResources.CheckoutCompanyAddress);
            XmlHelper.AddNode(doc, root, "OrderNoteText", ProductResources.CheckoutOrderNote);

            if (config.CheckoutNextZoneId > 0)
                XmlHelper.AddNode(doc, root, "NextPageUrl", CartHelper.GetZoneUrl(config.CheckoutNextZoneId));

            int languageId = WorkingCulture.LanguageId;
            string shippingMethod = string.Empty;
            string paymentMethod = string.Empty;
            var lstShippingMethods = ShippingMethod.GetByActive(siteSettings.SiteId, 1, languageId);
            foreach (ShippingMethod shipping in lstShippingMethods)
            {
                XmlElement shippingItemXml = doc.CreateElement("Shipping");
                root.AppendChild(shippingItemXml);

                XmlHelper.AddNode(doc, shippingItemXml, "Title", shipping.Name);
                XmlHelper.AddNode(doc, shippingItemXml, "Description", shipping.Description);
                XmlHelper.AddNode(doc, shippingItemXml, "Id", shipping.ShippingMethodId.ToString());

                if (order != null && shipping.ShippingMethodId == order.ShippingMethod)
                {
                    XmlHelper.AddNode(doc, shippingItemXml, "IsActive", "true");
                    shippingMethod = shipping.Name;
                }
            }

            var lstPaymentMethods = PaymentMethod.GetByActive(siteSettings.SiteId, 1, languageId);
            foreach (PaymentMethod payment in lstPaymentMethods)
            {
                XmlElement paymentItemXml = doc.CreateElement("Payment");
                root.AppendChild(paymentItemXml);

                XmlHelper.AddNode(doc, paymentItemXml, "Title", payment.Name);
                XmlHelper.AddNode(doc, paymentItemXml, "Description", payment.Description);
                XmlHelper.AddNode(doc, paymentItemXml, "Id", payment.PaymentMethodId.ToString());

                if (order != null && payment.PaymentMethodId == order.PaymentMethod)
                {
                    XmlHelper.AddNode(doc, paymentItemXml, "IsActive", "true");
                    paymentMethod = payment.Name;
                }
            }

            if (order != null)
            {
                XmlHelper.AddNode(doc, root, "CompanyName", order.InvoiceCompanyName);
                XmlHelper.AddNode(doc, root, "CompanyTaxCode", order.InvoiceCompanyTaxCode);
                XmlHelper.AddNode(doc, root, "CompanyAddress", order.InvoiceCompanyAddress);
                XmlHelper.AddNode(doc, root, "OrderNote", order.OrderNote);
                XmlHelper.AddNode(doc, root, "ShippingMethodId", order.ShippingMethod.ToString());
                XmlHelper.AddNode(doc, root, "ShippingMethod", shippingMethod);
                XmlHelper.AddNode(doc, root, "PaymentMethodId", order.PaymentMethod.ToString());
                XmlHelper.AddNode(doc, root, "PaymentMethod", paymentMethod);
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

            order = CartHelper.GetOrderSession(siteSettings.SiteId);
        }

        private void EnsureConfiguration()
        {
            if (config == null)
                config = new CheckoutMethodConfiguration(Settings);
        }
    }
}

namespace CanhCam.Web.ProductUI
{
    public class CheckoutMethodConfiguration
    {
        public CheckoutMethodConfiguration()
        { }

        public CheckoutMethodConfiguration(Hashtable settings)
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