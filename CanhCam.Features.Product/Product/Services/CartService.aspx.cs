/// Created:			    2014-11-28
/// Last Modified:		    2015-10-23

using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using CanhCam.Business;
using CanhCam.Web.Framework;
using System.Linq;
using log4net;
using CanhCam.Business.WebHelpers;

namespace CanhCam.Web.ProductUI
{
    public class CartService : CmsInitBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CartService));

        private string method = string.Empty;
        private NameValueCollection postParams = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/json";
            Encoding encoding = new UTF8Encoding();
            Response.ContentEncoding = encoding;

            try
            {
                LoadParams();

                if (
                    method != "AddProductToCart_Catalog"
                    && method != "AddProductToCart_Details"
                    && method != "UpdateCart"
                    && method != "RemoveFromCart"
                    && method != "ChangeAttributes"
                    )
                {
                    Response.Write(StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = "No method found with the " + method
                    }));
                    return;
                }

                if (method == "UpdateCart")
                {
                    Response.Write(CartHelper.UpdateCart(postParams));
                    return;
                }
                else if (method == "RemoveFromCart")
                {
                    Guid itemGuid = Guid.Empty;
                    if (postParams.Get("itemguid") != null)
                    {
                        Guid.TryParse(postParams.Get("itemguid"), out itemGuid);
                    }

                    Response.Write(CartHelper.RemoveFromCart(itemGuid));
                    return;
                }
                
                int productId = -1;
                Product product = null;
                //productId = WebUtils.ParseInt32FromQueryString("ProductId", productId);
                if (postParams.Get("productid") != null)
                {
                    int.TryParse(postParams.Get("productid"), out productId);
                }

                SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
                if (siteSettings != null && siteSettings.SiteId > 0)
                    product = new Product(siteSettings.SiteId, productId);

                if (
                    product == null
                    || product.ProductId <= 0
                    || !product.IsPublished
                    || product.IsDeleted
                    )
                {
                    Response.Write(StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = "No product found with the specified ID"
                    }));
                    return;
                }

                switch (method)
                {
                    case "AddProductToCart_Catalog":
                        int quantity = 1;
                        if (postParams.Get("qty") != null)
                            int.TryParse(postParams.Get("qty"), out quantity);
                        Response.Write(CartHelper.AddProductToCart_Catalog(product, ShoppingCartTypeEnum.ShoppingCart, quantity));
                        break;
                    case "AddProductToCart_Details":
                        Response.Write(CartHelper.AddProductToCart_Details(product, ShoppingCartTypeEnum.ShoppingCart, postParams));
                        break;
                    case "ChangeAttributes":
                        Response.Write(ChangeAttributes(product, postParams));
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);

                Response.Write(StringHelper.ToJsonString(new
                {
                    success = false,
                    message = "Failed to process from the server. Please refresh the page and try one more time."
                }));
            }
        }

        private string ChangeAttributes(Product product, NameValueCollection form)
        {
            decimal price = ProductHelper.GetPrice(product);
            var productProperties = ProductProperty.GetPropertiesByProduct(product.ProductId);
            if (productProperties.Count > 0)
            {
                var customFieldIds = productProperties.Select(x => x.CustomFieldId).Distinct().ToList();
                var productAttributes = CustomField.GetActiveByFields(product.SiteId, Product.FeatureGuid, customFieldIds);
                foreach (var attribute in productAttributes)
                {
                    if ((attribute.Options & (int)CustomFieldOptions.EnableShoppingCart) > 0)
                    {
                        string controlId = string.Format("product_attribute_{0}_{1}", product.ProductId, attribute.CustomFieldId);
                        var ctrlAttributes = form[controlId];
                        if (!String.IsNullOrEmpty(ctrlAttributes))
                        {
                            int selectedAttributeId = int.Parse(ctrlAttributes);
                            productProperties.ForEach(x =>
                            {
                                if (x.CustomFieldOptionId == selectedAttributeId)
                                    price += x.OverriddenPrice;
                            });
                        }
                    }
                }
            }

            return StringHelper.ToJsonString(new
            {
                success = true,
                price = ProductHelper.FormatPrice(price, true)
            });
        }

        private void LoadParams()
        {
            // don't accept get requests
            if (Request.HttpMethod != "POST") { return; }

            postParams = HttpUtility.ParseQueryString(Request.GetRequestBody());

            if (postParams.Get("method") != null)
            {
                method = postParams.Get("method");
            }
        }
    }
}