/// Created:			    2015-07-24
/// Last Modified:		    2015-07-24

using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using CanhCam.Business;
using CanhCam.Web.Framework;
using System.Linq;
using log4net;
using CanhCam.Business.WebHelpers;
using Resources;
using System.Collections.Generic;

namespace CanhCam.Web.ProductUI
{
    public class CheckoutService : CmsInitBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CheckoutService));

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
                    method != "SaveOrder"
                    && method != "GetDistrictsByProvinceGuid"
                    && method != "GetShippingTotal"
                    && method != "GetOrderCode"
                    && method != "ChangePoint"
                    )
                {
                    Response.Write(StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = "No method found with the " + method
                    }));
                    return;
                }

                switch (method)
                {
                    case "SaveOrder":
                        bool saveToDB = false;
                        if (postParams.Get("savetodb") != null)
                            bool.TryParse(postParams.Get("savetodb"), out saveToDB);

                        Response.Write(SaveOrder(saveToDB));
                        break;
                    case "GetDistrictsByProvinceGuid":
                        Response.Write(GetDistrictsByProvinceGuid());
                        break;
                    case "GetShippingTotal":
                        Response.Write(GetShippingTotal());
                        break;
                    case "GetOrderCode":
                        Response.Write(GetOrderCode());
                        break;
                    case "ChangePoint":
                        Response.Write(ChangePoint());
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

        private string SaveOrder(bool saveToDB)
        {
            try
            {
                //validation
                SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
                var cart = CartHelper.GetShoppingCart(siteSettings.SiteId, ShoppingCartTypeEnum.ShoppingCart);

                if (cart.Count == 0)
                {
                    return StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = ProductResources.CartIsEmptyLabel
                    });
                }

                //if (!ProductConfiguration.OnePageCheckoutEnabled)
                //{
                //    return StringHelper.ToJsonString(new
                //    {
                //        success = false,
                //        message = "One page checkout is disabled"
                //    });
                //}

                if ((!Request.IsAuthenticated && !ProductConfiguration.AnonymousCheckoutAllowed))
                {
                    return StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = ProductResources.CheckoutAnonymousNotAllowed
                    });
                }

                //string validateResult = string.Empty;
                //bool validate = CheckValidate(out validateResult);
                //if (!validate)
                //    return validateResult;

                Order order = CartHelper.GetOrderSession(siteSettings.SiteId);
                if (order == null)
                {
                    order = new Order();
                    order.SiteId = siteSettings.SiteId;
                }

                order.BillingFirstName = GetPostValue("Address_FirstName", order.BillingFirstName);
                order.BillingLastName = GetPostValue("Address_LastName", order.BillingLastName);
                order.BillingEmail = GetPostValue("Address_Email", order.BillingEmail);
                order.BillingAddress = GetPostValue("Address_Address", order.BillingAddress);
                order.BillingPhone = GetPostValue("Address_Phone", order.BillingPhone);
                order.BillingMobile = GetPostValue("Address_Mobile", order.BillingMobile);
                order.BillingFax = GetPostValue("Address_Fax", order.BillingFax);
                order.BillingStreet = GetPostValue("Address_Street", order.BillingStreet);
                order.BillingWard = GetPostValue("Address_Ward", order.BillingWard);

                string district = GetPostValue("Address_District", order.BillingDistrictGuid.ToString());
                if (district.Length == 36)
                    order.BillingDistrictGuid = new Guid(district);
                else
                    order.BillingDistrictGuid = Guid.Empty;

                string province = GetPostValue("Address_Province", order.BillingProvinceGuid.ToString());
                if (province.Length == 36)
                    order.BillingProvinceGuid = new Guid(province);
                else
                    order.BillingProvinceGuid = Guid.Empty;

                string country = GetPostValue("Address_Country", order.BillingCountryGuid.ToString());
                if (country.Length == 36)
                    order.BillingCountryGuid = new Guid(country);
                else
                    order.BillingCountryGuid = Guid.Empty;

                // Shipping method
                bool hasShipping = false;
                foreach (var key in postParams.AllKeys)
                {
                    if (key == "ShippingMethod")
                    {
                        hasShipping = true;
                        break;
                    }
                }
                if (hasShipping)
                {
                    order.ShippingMethod = -1;
                    string shippingMethod = GetPostValue("ShippingMethod");
                    var lstShippingMethods = ShippingMethod.GetByActive(siteSettings.SiteId, 1);
                    foreach (ShippingMethod shipping in lstShippingMethods)
                    {
                        if (shippingMethod == shipping.ShippingMethodId.ToString())
                        {
                            order.ShippingMethod = shipping.ShippingMethodId;
                            break;
                        }
                    }

                    if (order.ShippingMethod == -1)
                    {
                        return StringHelper.ToJsonString(new
                        {
                            success = false,
                            message = ProductResources.CheckoutShippingMethodRequired
                        });
                    }
                }

                // Payment method
                bool hasPayment = false;
                foreach (var key in postParams.AllKeys)
                {
                    if (key == "PaymentMethod")
                    {
                        hasPayment = true;
                        break;
                    }
                }
                if (hasPayment)
                {
                    order.PaymentMethod = -1;
                    string paymentMethod = GetPostValue("PaymentMethod");
                    var lstPaymentMethods = PaymentMethod.GetByActive(siteSettings.SiteId, 1);
                    foreach (PaymentMethod payment in lstPaymentMethods)
                    {
                        if (paymentMethod == payment.PaymentMethodId.ToString())
                        {
                            order.PaymentMethod = payment.PaymentMethodId;
                            break;
                        }
                    }

                    if (order.PaymentMethod == -1)
                    {
                        return StringHelper.ToJsonString(new
                        {
                            success = false,
                            message = ProductResources.CheckoutPaymentMethodRequired
                        });
                    }
                }

                // Company Info
                order.InvoiceCompanyName = GetPostValue("Invoice.CompanyName", order.InvoiceCompanyName);
                order.InvoiceCompanyAddress = GetPostValue("Invoice.CompanyAddress", order.InvoiceCompanyAddress);
                order.InvoiceCompanyTaxCode = GetPostValue("Invoice.CompanyTaxCode", order.InvoiceCompanyTaxCode);
                order.OrderNote = GetPostValue("OrderNote", order.OrderNote);

                string result = string.Empty;
                if (!IsBillingAddressValid(order, out result))
                    return result;
                if (!IsShippingAddressValid(order, out result))
                    return result;

                if (saveToDB)
                {
                    order.OrderCode = ProductHelper.GenerateOrderCode(order.SiteId);
                    order.CreatedFromIP = SiteUtils.GetIP4Address();
                    if (Request.IsAuthenticated)
                    {
                        SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
                        if (siteUser != null)
                        {
                            order.UserGuid = siteUser.UserGuid;
                            siteUser.ICQ = order.BillingProvinceGuid.ToString();
                            siteUser.AIM = order.BillingDistrictGuid.ToString();
                            siteUser.Save();
                        }
                    }

                    order.Save();
                    if (SaveOrderSummary(order, cart))
                    {
                        //CartHelper.CouponCodeInput = null;
                        CartHelper.ClearCartCookie(order.SiteId);
                        CartHelper.SetOrderSession(siteSettings.SiteId, null);
                        HttpContext.Current.Session[GetOrderIDSessionKey(order.SiteId)] = order.OrderId;

                        var onePayUrl = OnePayHelper.GetPaymentUrlIfNeeded(order);
                        if (!string.IsNullOrEmpty(onePayUrl))
                        {
                            //System.Timers.Timer timer1 = new System.Timers.Timer();
                            //timer1.Interval = 5 * 60 * 1000; //ms, 5 minutes
                            //timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);
                            //timer1.Enabled = true;
                            //GC.KeepAlive(timer1);

                            return StringHelper.ToJsonString(new
                            {
                                success = true,
                                redirect = onePayUrl
                            });
                        }
                    }
                }
                else
                    CartHelper.SetOrderSession(siteSettings.SiteId, order);

                return StringHelper.ToJsonString(new
                {
                    success = true,
                    redirect = GetPostValue("redirect", string.Empty)
                });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return StringHelper.ToJsonString(new { success = false, message = ex.Message });
            }
        }

        private string GetOrderIDSessionKey(int siteId)
        {
            return "OrderID" + siteId.ToString();
        }

        private bool SaveOrderSummary(Order order, List<ShoppingCartItem> cartList)
        {
            List<Product> lstProductsInCart = Product.GetByShoppingCart(siteSettings.SiteId, CartHelper.GetCartSessionGuid(siteSettings.SiteId));

            List<CustomField> customFields = new List<CustomField>();
            List<ProductProperty> productProperties = new List<ProductProperty>();
            if (ProductConfiguration.EnableShoppingCartAttributes && ProductConfiguration.EnableAttributesPriceAdjustment)
            {
                List<int> lstProductIds = lstProductsInCart.Select(x => x.ProductId).Distinct().ToList();
                productProperties = ProductProperty.GetPropertiesByProducts(lstProductIds);
                if (productProperties.Count > 0)
                {
                    var customFieldIds = productProperties.Select(x => x.CustomFieldId).Distinct().ToList();
                    customFields = CustomField.GetByOption(CustomField.GetActiveByFields(siteSettings.SiteId, Product.FeatureGuid, customFieldIds), CustomFieldOptions.EnableShoppingCart);
                }
            }

            //List<CouponAppliedToItem> lstAppliedItems = new List<CouponAppliedToItem>();
            //Coupon coupon = null;
            //if (CartHelper.CouponCodeInput.Length > 0)
            //    lstAppliedItems = CartHelper.GetAppliedItems(siteSettings.SiteId, cartList, lstProductsInCart, out coupon);

            decimal totalDiscount = decimal.Zero;
            List<OrderItem> lstOrderItem = new List<OrderItem>();
            foreach (ShoppingCartItem itm in cartList)
            {
                Product product = ProductHelper.GetProductFromList(lstProductsInCart, itm.ProductId);

                if (product != null)
                {
                    decimal discount = decimal.Zero;
                    decimal price = ProductHelper.GetPrice(product);

                    //if (lstAppliedItems.Count > 0)
                    //{
                    //    CouponAppliedToItem appliedToItem = CouponAppliedToItem.FindFromList(lstAppliedItems, product.ZoneId, (int)CouponAppliedType.ToCategories);
                    //    if (appliedToItem == null)
                    //        appliedToItem = CouponAppliedToItem.FindFromList(lstAppliedItems, product.ProductId, (int)CouponAppliedType.ToProducts);

                    //    if (appliedToItem != null)
                    //    {
                    //        int totalRemain = 0;
                    //        if (coupon.MaximumQtyDiscount > 0)
                    //        {
                    //            int totalQty = OrderItem.GetTotalItemsByCoupon(product.ProductId, coupon.CouponCode);
                    //            totalRemain = coupon.MaximumQtyDiscount - totalQty;
                    //            if (totalRemain <= 0)
                    //                totalRemain = -1;
                    //        }
                    //        if (totalRemain >= 0)
                    //        {
                    //            int qtyApplied = Math.Min(coupon.DiscountQtyStep, itm.Quantity);
                    //            if (totalRemain > 0)
                    //                qtyApplied = Math.Min(qtyApplied, totalRemain);
                    //            if (appliedToItem.Discount > 0)
                    //            {
                    //                if (appliedToItem.UsePercentage)
                    //                    discount = (appliedToItem.Discount * product.Price * qtyApplied) / 100;
                    //                else
                    //                    discount = appliedToItem.Discount * qtyApplied;
                    //            }
                    //            else
                    //            {
                    //                if (coupon.DiscountType == (int)CouponDiscountType.PercentagePerProduct)
                    //                    discount = (coupon.Discount * product.Price * qtyApplied) / 100;
                    //                else
                    //                    discount = coupon.Discount * qtyApplied;
                    //            }
                    //            totalDiscount += discount;
                    //        }
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(itm.AttributesXml))
                    {
                        var attributes = ProductAttributeParser.ParseProductAttributeMappings(customFields, itm.AttributesXml);
                        if (attributes.Count > 0)
                        {
                            foreach (var a in attributes)
                            {
                                var values = ProductAttributeParser.ParseValues(itm.AttributesXml, a.CustomFieldId);
                                if (values.Count > 0)
                                {
                                    productProperties.ForEach(property =>
                                    {
                                        if (property.ProductId == product.ProductId
                                            && property.CustomFieldId == a.CustomFieldId
                                            && values.Contains(property.CustomFieldOptionId))
                                            price += property.OverriddenPrice;
                                    });
                                }
                            }
                        }
                    }

                    OrderItem orderItem = new OrderItem();
                    orderItem.OrderId = order.OrderId;
                    orderItem.Price = price;
                    orderItem.DiscountAmount = discount;
                    orderItem.ProductId = itm.ProductId;
                    orderItem.Quantity = itm.Quantity;
                    orderItem.AttributesXml = itm.AttributesXml;
                    orderItem.AttributeDescription = itm.CreatedFromIP;

                    totalDiscount += discount;

                    orderItem.Save();
                    lstOrderItem.Add(orderItem);
                }

                ShoppingCartItem.Delete(itm.Guid);
            }

            decimal shippingPrice = decimal.Zero;
            if (order.ShippingMethod > 0)
            {
                decimal orderSubTotal = cartList.GetSubTotal(lstProductsInCart);
                decimal orderWeight = cartList.GetTotalWeights(lstProductsInCart);
                int productTotalQty = cartList.GetTotalProducts();
                string geoZoneGuids = ProductHelper.GetShippingGeoZoneGuidsByOrderSession(order);
                shippingPrice = ProductHelper.GetShippingPrice(order.ShippingMethod, orderSubTotal, orderWeight, productTotalQty, geoZoneGuids);
            }

            decimal subTotal = cartList.GetSubTotal(lstProductsInCart);

            //order.UserPoint = 0;
            //order.UserPointDiscount = 0;
            //int point = 0;
            //string currentPoint = GetPostValue("hdfCurrentPoint", "0");
            //int.TryParse(currentPoint, out point);
            //if (point >= 0 && Request.IsAuthenticated)
            //{
            //    SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
            //    if (siteUser != null && siteUser.UserId > 0 && point <= siteUser.TotalPosts)
            //    {
            //        order.UserPoint = point;
            //        order.UserPointDiscount = ProductHelper.GetDiscountByPoint(point);
            //        int bonusPoint = Convert.ToInt32(Math.Round(subTotal / 1000));
            //        SiteUserEx.UpdateUserPoints(siteUser.UserGuid, (siteUser.TotalPosts - point) + bonusPoint);
            //    }
            //}

            decimal total = CartHelper.GetCartTotal(subTotal: subTotal, shippingTotal: shippingPrice, pointDiscount: order.UserPointDiscount);

            order.OrderDiscount = totalDiscount;
            order.OrderSubtotal = subTotal;
            order.OrderShipping = shippingPrice;
            order.OrderTotal = total;

            //if (lstAppliedItems.Count > 0 && coupon != null)
            //{
            //    order.CouponCode = coupon.CouponCode;

            //    var history = new CouponUsageHistory();
            //    history.OrderId = order.OrderId;
            //    history.CouponId = coupon.CouponId;
            //    history.Save();
            //}

            order.Save();

            string billingProvinceName = string.Empty;
            string billingDistrictName = string.Empty;
            string shippingProvinceName = string.Empty;
            string shippingDistrictName = string.Empty;
            if (order.BillingProvinceGuid != Guid.Empty)
            {
                var province = new GeoZone(order.BillingProvinceGuid);
                if (province != null && province.Guid != Guid.Empty)
                    billingProvinceName = province.Name;
            }
            if (order.BillingDistrictGuid != Guid.Empty)
            {
                var province = new GeoZone(order.BillingDistrictGuid);
                if (province != null && province.Guid != Guid.Empty)
                    billingDistrictName = province.Name;
            }
            if (order.ShippingProvinceGuid != Guid.Empty)
            {
                var province = new GeoZone(order.ShippingProvinceGuid);
                if (province != null && province.Guid != Guid.Empty)
                    shippingProvinceName = province.Name;
            }
            if (order.ShippingDistrictGuid != Guid.Empty)
            {
                var province = new GeoZone(order.ShippingDistrictGuid);
                if (province != null && province.Guid != Guid.Empty)
                    shippingDistrictName = province.Name;
            }

            string toEmail = order.BillingEmail.Trim();
            if (
                order.ShippingEmail.Length > 0
                && !string.Equals(toEmail, order.ShippingEmail, StringComparison.CurrentCultureIgnoreCase)
                )
                toEmail += "," + order.ShippingEmail;

            //ProductHelper.SendOrderPlacedNotification(siteSettings, order, lstProductsInCart, lstOrderItem, "OrderPlacedCustomerNotification", billingProvinceName, billingDistrictName, shippingProvinceName, shippingDistrictName, toEmail);
            ProductHelper.SendOrderPlacedNotification(siteSettings, order, lstProductsInCart, lstOrderItem, "OrderPlacedStoreOwnerNotification", billingProvinceName, billingDistrictName, shippingProvinceName, shippingDistrictName);

            WebTaskManager.StartOrResumeTasks();

            return true;
        }

        #region IsBillingAddressValid

        private bool IsBillingAddressValid(Order order, out string result)
        {
            result = string.Empty;

            // Check billing address required
            string requiredFields = ProductConfiguration.OrderAddressRequiredFields;
            if (IsEmpty(requiredFields, "Address_FirstName", order.BillingFirstName))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressFirstNameRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_LastName", order.BillingLastName))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressLastNameRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Email", order.BillingEmail))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressEmailRequired
                });

                return false;
            }
            // Check email validate
            if (!string.IsNullOrWhiteSpace(order.BillingEmail))
            {
                if (!IsValidEmail(order.BillingEmail))
                {
                    result = StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = ProductResources.CheckoutAddressEmailInvalid
                    });

                    return false;
                }
            }
            if (IsEmpty(requiredFields, "Address_Address", order.BillingAddress))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Phone", order.BillingPhone))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressPhoneRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Mobile", order.BillingMobile))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressMobileRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Fax", order.BillingFax))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressFaxRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Street", order.BillingStreet))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressStreetRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Ward", order.BillingWard))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressWardRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_District", order.BillingDistrictGuid == Guid.Empty ? string.Empty : order.BillingDistrictGuid.ToString()))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressDistrictRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "Address_Province", order.BillingProvinceGuid == Guid.Empty ? string.Empty : order.BillingProvinceGuid.ToString()))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressProvinceRequired
                });

                return false;
            }

            return true;
        }

        #endregion

        #region IsShippingAddressValid

        private bool IsShippingAddressValid(Order order, out string result)
        {
            result = string.Empty;

            // Check shipping address required
            string requiredFields = ProductConfiguration.OrderAddressRequiredFields;
            if (IsEmpty(requiredFields, "ShippingAddress_FirstName", order.ShippingFirstName))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressFirstNameRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_LastName", order.ShippingLastName))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressLastNameRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Email", order.ShippingEmail))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressEmailRequired
                });

                return false;
            }
            // Check email validate
            if (!string.IsNullOrWhiteSpace(order.ShippingEmail))
            {
                if (!IsValidEmail(order.ShippingEmail))
                {
                    result = StringHelper.ToJsonString(new
                    {
                        success = false,
                        message = ProductResources.CheckoutAddressEmailInvalid
                    });

                    return false;
                }
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Address", order.ShippingAddress))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Phone", order.ShippingPhone))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressPhoneRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Mobile", order.ShippingMobile))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressMobileRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Fax", order.ShippingFax))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressFaxRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Street", order.ShippingStreet))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressStreetRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Ward", order.ShippingWard))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressWardRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_District", order.ShippingDistrictGuid == Guid.Empty ? string.Empty : order.ShippingDistrictGuid.ToString()))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressDistrictRequired
                });

                return false;
            }
            if (IsEmpty(requiredFields, "ShippingAddress_Province", order.ShippingProvinceGuid == Guid.Empty ? string.Empty : order.BillingProvinceGuid.ToString()))
            {
                result = StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CheckoutAddressProvinceRequired
                });

                return false;
            }

            return true;
        }

        #endregion

        private bool IsEmpty(string requiredFields, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value) && requiredFields.Contains(name))
            {
                return true;
            }

            return false;
        }

        private string GetPostValue(string name, string defaultValueIfEmpty = "")
        {
            if (postParams[name] != null)
                return postParams[name];

            return defaultValueIfEmpty;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string GetDistrictsByProvinceGuid()
        {
            string provinceGuid = GetPostValue("provinceGuid");

            ////this action method gets called via an ajax request
            //if (string.IsNullOrEmpty(provinceGuid) || provinceGuid.Length != 36)
            //{
            //    return StringHelper.ToJsonString(new
            //    {
            //        success = false,
            //        message = "provinceGuid"
            //    });
            //}

            //var province = new GeoZone(new Guid(provinceGuid));
            var lstDistricts = new List<GeoZone>();
            if (provinceGuid.Length == 36)
                lstDistricts = GeoZone.GetByListParent(provinceGuid, 1, WorkingCulture.LanguageId);
            var result = (from s in lstDistricts
                          select new { guid = s.Guid.ToString(), name = s.Name }).ToList();

            result.Insert(0, new { guid = "", name = ProductResources.CheckoutSelectDistrict });

            return StringHelper.ToJsonString(result);
        }

        public string GetShippingTotal()
        {
            string strShippingMethodId = GetPostValue("shippingMethodId");
            int shippingMethodId = 0;
            int.TryParse(strShippingMethodId, out shippingMethodId);
            if (shippingMethodId > 0)
            {
                List<Product> lstProductsInCart = new List<Product>();
                List<ShoppingCartItem> cartList = CartHelper.GetShoppingCart(siteSettings.SiteId, ShoppingCartTypeEnum.ShoppingCart);
                if (cartList.Count > 0) lstProductsInCart = Product.GetByShoppingCart(siteSettings.SiteId, CartHelper.GetCartSessionGuid(siteSettings.SiteId));

                Order order = CartHelper.GetOrderSession(siteSettings.SiteId);
                decimal orderSubTotal = cartList.GetSubTotal(lstProductsInCart);
                decimal orderWeight = cartList.GetTotalWeights(lstProductsInCart);
                int productTotalQty = cartList.GetTotalProducts();
                string geoZoneGuids = ProductHelper.GetShippingGeoZoneGuidsByOrderSession(order);
                decimal shippingPrice = ProductHelper.GetShippingPrice(shippingMethodId, orderSubTotal, orderWeight, productTotalQty, geoZoneGuids);
                decimal subTotal = cartList.GetSubTotal(lstProductsInCart);
                decimal total = CartHelper.GetCartTotal(subTotal, shippingPrice);

                return StringHelper.ToJsonString(new
                {
                    success = true,
                    shippingtotal = Convert.ToDouble(shippingPrice),
                    shippingtotalsectionhtml = ProductHelper.FormatPrice(shippingPrice, true),
                    totalsectionhtml = ProductHelper.FormatPrice(total, true)
                });
            }

            return StringHelper.ToJsonString(new
            {
                success = false
            });
        }

        public string GetOrderCode()
        {
            try
            {
                string orderIdKey = GetOrderIDSessionKey(siteSettings.SiteId);
                if (HttpContext.Current.Session[orderIdKey] != null)
                {
                    int orderId = Convert.ToInt32(HttpContext.Current.Session[orderIdKey]);
                    Order order = new Order(orderId);
                    if (order != null && order.OrderId > 0)
                    {
                        return StringHelper.ToJsonString(new
                        {
                            success = true,
                            ordercode = order.OrderCode,
                            homepageurl = SiteUtils.GetHomepageUrl()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return StringHelper.ToJsonString(new
            {
                success = false,
                homepageurl = SiteUtils.GetHomepageUrl()
            });
        }

        public string ChangePoint()
        {
            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
            var cart = CartHelper.GetShoppingCart(siteSettings.SiteId, ShoppingCartTypeEnum.ShoppingCart);

            if (cart.Count == 0)
            {
                return StringHelper.ToJsonString(new
                {
                    success = false,
                    message = ProductResources.CartIsEmptyLabel
                });
            }

            SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
            if (siteUser == null || siteUser.UserId <= 0)
            {
                return StringHelper.ToJsonString(new
                {
                    success = false,
                    message = "User không tồn tại."
                });
            }

            int point = 0;
            if (postParams.Get("point") != null)
                int.TryParse(postParams.Get("point"), out point);

            List<Product> lstProductsInCart = Product.GetByShoppingCart(siteSettings.SiteId, CartHelper.GetCartSessionGuid(siteSettings.SiteId));
            var subTotal = cart.GetSubTotal(lstProductsInCart);

            decimal pointDiscount = ProductHelper.GetDiscountByPoint(point);
            decimal total = subTotal - pointDiscount;

            return StringHelper.ToJsonString(new
            {
                success = true,
                pointDiscount = ProductHelper.FormatPrice(pointDiscount, true),
                total = ProductHelper.FormatPrice(total, true)
            });
        }

    }
}