/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-30
/// Last Modified:			2015-07-17
/// 2015-10-22: Add product with attributes to cart

using System;
using System.Web;
using CanhCam.Web.Framework;
using CanhCam.Business;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using log4net;
using System.Collections.Specialized;
using CanhCam.Business.WebHelpers;

namespace CanhCam.Web.ProductUI
{
    public static class CartHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CartHelper));

        public static List<ShoppingCartItem> GetShoppingCart(int siteId, ShoppingCartTypeEnum shoppingCartType)
        {
            var cartSessionGuid = GetCartSessionGuid(siteId);

            if (HttpContext.Current == null) return ShoppingCartItem.GetByUserGuid(siteId, shoppingCartType, cartSessionGuid);

            string contextKey = "ShoppingCart_" + siteId.ToInvariantString() + "_" + shoppingCartType;

            List<ShoppingCartItem> lstShoppingCartItems = HttpContext.Current.Items[contextKey] as List<ShoppingCartItem>;
            if (lstShoppingCartItems == null)
            {
                lstShoppingCartItems = ShoppingCartItem.GetByUserGuid(siteId, shoppingCartType, cartSessionGuid);
                HttpContext.Current.Items[contextKey] = lstShoppingCartItems;
            }

            return lstShoppingCartItems;
        }

        public static Guid GetCartSessionGuid(int siteId, bool setCookieIfNotExists = false)
        {
            if (
                (HttpContext.Current != null)
                && (HttpContext.Current.Request.IsAuthenticated)
                )
            {
                SiteUser siteUser = SiteUtils.GetCurrentSiteUser();

                if (siteUser != null && siteUser.UserGuid != Guid.Empty)
                {
                    VerifyCartUser(siteUser);
                    return siteUser.UserGuid;
                }
            }

            string cartCookie = GetCartCookie(siteId);
            if (cartCookie.Length == 36)
                return new Guid(cartCookie);

            Guid cartGuid = Guid.NewGuid();
            if (setCookieIfNotExists)
                SetCartCookie(siteId, cartGuid);

            return cartGuid;
        }

        private static string GetOrderSessionKey(int siteId)
        {
            string orderKey = "Order" + siteId.ToString();

            return orderKey;
        }

        public static Order GetOrderSession(int siteId)
        {
            string orderKey = GetOrderSessionKey(siteId);
            if (HttpContext.Current.Session[orderKey] != null)
            {
                return (Order)HttpContext.Current.Session[orderKey];
            }

            return null;
        }

        public static void SetOrderSession(int siteId, Order order)
        {
            string orderKey = GetOrderSessionKey(siteId);
            if (order == null)
                HttpContext.Current.Session.Remove(orderKey);
            else
                HttpContext.Current.Session[orderKey] = order;
        }

        private static string GetCartCookie(int siteId)
        {
            string cartKey = "cart" + siteId.ToString();

            // TODO: decrypt and verify?

            return CookieHelper.GetCookieValue(cartKey);
        }

        private static void SetCartCookie(int siteId, Guid cartGuid)
        {
            string cartKey = "cart" + siteId.ToString();

            // TODO encrypt, sign?

            CookieHelper.SetPersistentCookie(cartKey, cartGuid.ToString());
        }

        public static void ClearCartCookie(int siteId)
        {
            string cartKey = "cart" + siteId.ToString();

            CookieHelper.ExpireCookie(cartKey);
        }

        private static void VerifyCartUser(SiteUser currentUser)
        {
            string cartCookie = GetCartCookie(currentUser.SiteId);
            if (cartCookie.Length == 36)
            {
                Guid cartGuid = new Guid(cartCookie);

                ShoppingCartItem.MoveToUser(cartGuid, currentUser.UserGuid);
                ClearCartCookie(currentUser.SiteId);
            }
        }

        public static string AddProductToCart_Catalog(Product product, ShoppingCartTypeEnum cartType, int quantity, bool forceredirection = false)
        {
            ////products with "minimum order quantity" more than a specified qty
            //if (product.OrderMinimumQuantity > quantity)
            //{
            //    //we cannot add to the cart such products from category pages
            //    //it can confuse customers. That's why we redirect customers to the product details page
            //    return Json(new
            //    {
            //        redirect = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
            //    });
            //}

            //if (product.CustomerEntersPrice)
            //{
            //    //cannot be added to the cart (requires a customer to enter price)
            //    return Json(new
            //    {
            //        redirect = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
            //    });
            //}

            //if (product.IsRental)
            //{
            //    //rental products require start/end dates to be entered
            //    return Json(new
            //    {
            //        redirect = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
            //    });
            //}

            //var allowedQuantities = product.ParseAllowedQuantities();
            //if (allowedQuantities.Length > 0)
            //{
            //    //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
            //    return Json(new
            //    {
            //        redirect = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
            //    });
            //}

            //if (product.ProductAttributeMappings.Count > 0)
            //{
            //    //product has some attributes. let a customer see them
            //    return Json(new
            //    {
            //        redirect = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
            //    });
            //}

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cartSessionGuid = CartHelper.GetCartSessionGuid(product.SiteId, true);
            var cart = ShoppingCartItem.GetByUserGuid(product.SiteId, cartType, cartSessionGuid);
            var shoppingCartItem = CartHelper.FindShoppingCartItemInTheCart(cart, cartType, product);

            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = CartHelper.GetShoppingCartItemWarnings(cartSessionGuid, cartType,
                product, string.Empty, quantityToValidate, false, true, false);
            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart
                //let's display standard warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            var shoppingCartItemGuid = Guid.Empty;
            addToCartWarnings = CartHelper.AddToCart(
                product: product,
                shoppingCartType: cartType,
                shoppingCartItemGuid: out shoppingCartItemGuid,
                quantity: quantity);
            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Json(new
                {
                    redirect = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartTypeEnum.Wishlist:
                    {
                        string wishlistUrl = CartHelper.GetWishlistUrl();
                        if (ProductConfiguration.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = wishlistUrl
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(ResourceHelper.GetResourceString("ProductResources", "WishlistQuantityFormat"),
                        CartHelper.GetShoppingCart(product.SiteId, ShoppingCartTypeEnum.Wishlist)
                        .GetTotalProducts());
                        return Json(new
                        {
                            success = true,
                            message = string.Format(ResourceHelper.GetResourceString("ProductResources", "WishlistProductHasBeenAddedFormat"), wishlistUrl),
                            updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                        });
                    }
                case ShoppingCartTypeEnum.ShoppingCart:
                default:
                    {
                        string cartUrl = CartHelper.GetCartUrl();
                        if (ProductConfiguration.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = cartUrl
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(ResourceHelper.GetResourceString("ProductResources", "CartQuantityFormat"),
                        CartHelper.GetShoppingCart(product.SiteId, ShoppingCartTypeEnum.ShoppingCart)
                        .GetTotalProducts());

                        var updateflyoutcartsectionhtml = ProductConfiguration.MiniShoppingCartEnabled
                            ? PrepareMiniShoppingCart(product.SiteId, shoppingCartItemGuid) : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(ResourceHelper.GetResourceString("ProductResources", "CartProductHasBeenAddedFormat"), cartUrl),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml = updateflyoutcartsectionhtml,
                        });
                    }
            }
        }

        public static string AddProductToCart_Details(Product product, ShoppingCartTypeEnum cartType, NameValueCollection postParams, bool forceredirection = false)
        {
            ////we can add only simple products
            //if (product.ProductType != ProductType.SimpleProduct)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "Only simple products could be added to the cart"
            //    });
            //}

            #region Update existing shopping cart item?
            Guid updatecartitemguid = Guid.Empty;
            foreach (string formKey in postParams)
                if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", product.ProductId), StringComparison.InvariantCultureIgnoreCase))
                {
                    if (postParams[formKey].Length == 36)
                        updatecartitemguid = new Guid(postParams[formKey]);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (ProductConfiguration.AllowCartItemEditing && updatecartitemguid != Guid.Empty)
            {
                var cart = CartHelper.GetShoppingCart(product.SiteId, ShoppingCartTypeEnum.ShoppingCart);
                updatecartitem = cart.FirstOrDefault(x => x.Guid == updatecartitemguid);
                //not found?
                if (updatecartitem == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No shopping cart item found to update"
                    });
                }
                //is it this product?
                if (product.ProductId != updatecartitem.ProductId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This product does not match a passed shopping cart item identifier"
                    });
                }
            }
            #endregion

            #region Customer entered price
            //decimal customerEnteredPriceConverted = decimal.Zero;
            //if (product.CustomerEntersPrice)
            //{
            //    foreach (string formKey in postParams)
            //    {
            //        if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            decimal customerEnteredPrice;
            //            if (decimal.TryParse(postParams[formKey], out customerEnteredPrice))
            //                customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
            //            break;
            //        }
            //    }
            //}
            #endregion

            #region Quantity

            int quantity = 1;

            if (postParams.Get(string.Format("addtocart_{0}.EnteredQuantity", product.ProductId)) != null)
            {
                int.TryParse(postParams.Get(string.Format("addtocart_{0}.EnteredQuantity", product.ProductId)), out quantity);
            }

            #endregion

            //product and gift card attributes
            string attributes = ProductAttributeParser.ParseProductAttributes(product, postParams);

            ////rental attributes
            //DateTime? rentalStartDate = null;
            //DateTime? rentalEndDate = null;
            //if (product.IsRental)
            //{
            //    ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            //}

            //save item
            var addToCartWarnings = new List<string>();
            var shoppingCartItemGuid = Guid.Empty;
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(CartHelper.AddToCart(
                    product, cartType, out shoppingCartItemGuid,
                    attributes, quantity, true));
            }
            else
            {
                var cart = CartHelper.GetShoppingCart(product.SiteId, ShoppingCartTypeEnum.ShoppingCart);
                var otherCartItemWithSameParameters = CartHelper.FindShoppingCartItemInTheCart(
                    cart, cartType, product, attributes);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Guid == updatecartitem.Guid)
                {
                    //ensure it's other shopping cart cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(UpdateShoppingCartItem(CartHelper.GetCartSessionGuid(product.SiteId),
                    updatecartitem.Guid, attributes, quantity));
                if (otherCartItemWithSameParameters != null && addToCartWarnings.Count == 0)
                {
                    //delete the same shopping cart item (the other one)
                    ShoppingCartItem.Delete(otherCartItemWithSameParameters.Guid);
                }
            }

            #region Return result

            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartTypeEnum.Wishlist:
                    {
                        string wishlistUrl = CartHelper.GetWishlistUrl();
                        if (ProductConfiguration.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = wishlistUrl
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(ResourceHelper.GetResourceString("ProductResources", "WishlistQuantityFormat"),
                        CartHelper.GetShoppingCart(product.SiteId, ShoppingCartTypeEnum.Wishlist)
                        .GetTotalProducts());
                        return Json(new
                        {
                            success = true,
                            message = string.Format(ResourceHelper.GetResourceString("ProductResources", "WishlistProductHasBeenAddedFormat"), wishlistUrl),
                            updatetopwishlistsectionhtml = updatetopwishlistsectionhtml
                        });
                    }
                case ShoppingCartTypeEnum.ShoppingCart:
                default:
                    {
                        string cartUrl = CartHelper.GetCartUrl();
                        if (ProductConfiguration.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = cartUrl
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(ResourceHelper.GetResourceString("ProductResources", "CartQuantityFormat"),
                        CartHelper.GetShoppingCart(product.SiteId, ShoppingCartTypeEnum.ShoppingCart)
                        .GetTotalProducts());

                        var updateflyoutcartsectionhtml = ProductConfiguration.MiniShoppingCartEnabled
                            ? PrepareMiniShoppingCart(product.SiteId, shoppingCartItemGuid) : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(ResourceHelper.GetResourceString("ProductResources", "CartProductHasBeenAddedFormat"), cartUrl),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml = updateflyoutcartsectionhtml,
                            cartpageurl = cartUrl
                        });

                    }
            }

            #endregion
        }

        private static string PrepareMiniShoppingCart(int siteId, Guid cartItemGuid)
        {
            string xsltPath = SiteUtils.GetXsltBasePath("product", "ShoppingCartFlyout.xslt");
            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(xsltPath)))
                return string.Empty;

            var doc = new XmlDocument();
            BuildShoppingCartXml(siteId, cartItemGuid, out doc);

            return XmlHelper.TransformXML(xsltPath, doc);
        }

        #region XmlData

        public static XmlElement BuildShoppingCartXml(int siteId, Guid lastAddedItemGuid, out XmlDocument doc)
        {
            doc = new XmlDocument();
            doc.LoadXml("<ShoppingCart></ShoppingCart>");
            XmlElement root = doc.DocumentElement;

            var lstProductsInCart = Product.GetByShoppingCart(siteId, CartHelper.GetCartSessionGuid(siteId));
            var cart = GetShoppingCart(siteId, ShoppingCartTypeEnum.ShoppingCart);
            string cartUrl = GetCartUrl();
            string cartEmptyText = Resources.ProductResources.CartIsEmptyLabel;
            string cartItemsText = string.Format(Resources.ProductResources.CartItemsFormat, cartUrl, cart.GetTotalProducts());

            XmlHelper.AddNode(doc, root, "CartPageUrl", cartUrl);
            XmlHelper.AddNode(doc, root, "CartEmptyText", cartEmptyText);
            XmlHelper.AddNode(doc, root, "CartItemsText", cartItemsText);
            XmlHelper.AddNode(doc, root, "CartText", Resources.ProductResources.CartLabel);
            XmlHelper.AddNode(doc, root, "SubTotalText", Resources.ProductResources.CartSubTotalLabel);
            XmlHelper.AddNode(doc, root, "TotalText", Resources.ProductResources.CartTotalLabel);
            XmlHelper.AddNode(doc, root, "QuantityText", Resources.ProductResources.CartQuantityLabel);
            XmlHelper.AddNode(doc, root, "UnitPriceText", Resources.ProductResources.CartUnitPriceLabel);
            XmlHelper.AddNode(doc, root, "PriceText", Resources.ProductResources.CartPriceLabel);
            XmlHelper.AddNode(doc, root, "RemoveText", Resources.ProductResources.CartRemoveLabel);
            XmlHelper.AddNode(doc, root, "ItemTotalText", Resources.ProductResources.CartItemTotalLabel);
            XmlHelper.AddNode(doc, root, "ImageText", Resources.ProductResources.CartImageLabel);
            XmlHelper.AddNode(doc, root, "ProductText", Resources.ProductResources.CartProductLabel);
            XmlHelper.AddNode(doc, root, "UpdateCartText", Resources.ProductResources.CartUpdateLabel);
            XmlHelper.AddNode(doc, root, "ContinueShoppingText", Resources.ProductResources.CartContinueShoppingLabel);
            XmlHelper.AddNode(doc, root, "ContinueShoppingUrl", GetContinueShopping());
            XmlHelper.AddNode(doc, root, "CouponCodeText", Resources.ProductResources.CartCouponCodeLabel);
            XmlHelper.AddNode(doc, root, "CouponApplyText", Resources.ProductResources.CartCouponApplyLabel);
            XmlHelper.AddNode(doc, root, "ShippingTotalText", Resources.ProductResources.CartShippingTotalLabel);
            XmlHelper.AddNode(doc, root, "CheckoutText", Resources.ProductResources.CartCheckoutLabel);
            XmlHelper.AddNode(doc, root, "CheckoutProcessText", Resources.ProductResources.CartCheckoutProcessLabel);
            XmlHelper.AddNode(doc, root, "DiscountText", Resources.ProductResources.CartDiscountLabel);

            if (cart.GetTotalProducts() > 0)
                XmlHelper.AddNode(doc, root, "CartSummaryText", cartItemsText);
            else
                XmlHelper.AddNode(doc, root, "CartSummaryText", cartEmptyText);

            List<ProductProperty> productProperties = new List<ProductProperty>();
            List<CustomField> customFields = new List<CustomField>();
            if (ProductConfiguration.EnableShoppingCartAttributes)
            {
                var lstProductIds = cart.Select(x => x.ProductId).Distinct().ToList();
                customFields = CustomField.GetActiveForCart(siteId, Product.FeatureGuid);
                if (customFields.Count > 0 && lstProductIds.Count > 0)
                    productProperties = ProductProperty.GetPropertiesByProducts(lstProductIds);
            }

            foreach (var cartItem in cart)
            {
                XmlElement cartItemXml = doc.CreateElement("CartItem");
                root.AppendChild(cartItemXml);

                BuildCartItemDataXml(doc, cartItemXml, cartItem, lstProductsInCart, customFields, productProperties, lastAddedItemGuid);
            }

            XmlHelper.AddNode(doc, root, "TotalProducts", cart.GetTotalProducts().ToString());

            decimal subTotal = cart.GetSubTotal(lstProductsInCart);
            XmlHelper.AddNode(doc, root, "SubTotal", ProductHelper.FormatPrice(subTotal, true));

            decimal shippingTotal = decimal.Zero;

            decimal total = GetCartTotal(subTotal, shippingTotal);
            XmlHelper.AddNode(doc, root, "ShippingTotal", ProductHelper.FormatPrice(shippingTotal, true));
            XmlHelper.AddNode(doc, root, "Total", ProductHelper.FormatPrice(total, true));

            //XmlHelper.AddNode(doc, root, "SubTotalDiscount", "0");

            return root;
        }

        public static decimal GetCartTotal(decimal subTotal = decimal.Zero, decimal shippingTotal = decimal.Zero, decimal paymentTotal = decimal.Zero, decimal discountTotal = decimal.Zero, decimal orderTax = decimal.Zero, decimal pointDiscount = decimal.Zero)
        {
            return subTotal + shippingTotal + paymentTotal + orderTax - pointDiscount - discountTotal;
        }

        public static string GetContinueShopping()
        {
            string returnUrl = CartHelper.LastContinueShoppingPage;
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = SiteUtils.GetHomepageUrl();

            return returnUrl;
        }

        private static XmlDocument BuildCartItemDataXml(XmlDocument doc, XmlElement cartItemXml, ShoppingCartItem cartItem, List<Product> lstProductsInCart, List<CustomField> customFields, List<ProductProperty> productProperties, Guid cartItemGuid)
        {
            Product product = ProductHelper.GetProductFromList(lstProductsInCart, cartItem.ProductId);
            if (product != null)
            {
                ProductHelper.BuildProductDataXml(doc, cartItemXml, product);

                decimal price = ProductHelper.GetPrice(product);

                if (!string.IsNullOrEmpty(cartItem.AttributesXml))
                {
                    var attributes = ProductAttributeParser.ParseProductAttributeMappings(customFields, cartItem.AttributesXml);
                    if (attributes.Count > 0)
                    {
                        foreach (var a in attributes)
                        {
                            XmlElement attributeXml = doc.CreateElement("Attributes");
                            cartItemXml.AppendChild(attributeXml);

                            XmlHelper.AddNode(doc, attributeXml, "FieldId", a.CustomFieldId.ToString());
                            XmlHelper.AddNode(doc, attributeXml, "ItemGuid", cartItem.Guid.ToString());
                            XmlHelper.AddNode(doc, attributeXml, "Title", a.Name);

                            var values = ProductAttributeParser.ParseValues(cartItem.AttributesXml, a.CustomFieldId);
                            foreach (ProductProperty property in productProperties)
                            {
                                if (property.ProductId == product.ProductId && property.CustomFieldId == a.CustomFieldId)
                                {
                                    XmlElement optionXml = doc.CreateElement("Options");
                                    attributeXml.AppendChild(optionXml);

                                    XmlHelper.AddNode(doc, optionXml, "FieldId", a.CustomFieldId.ToString());
                                    XmlHelper.AddNode(doc, optionXml, "ItemGuid", cartItem.Guid.ToString());
                                    XmlHelper.AddNode(doc, optionXml, "OptionId", property.CustomFieldOptionId.ToString());
                                    XmlHelper.AddNode(doc, optionXml, "Title", property.OptionName);

                                    if (values.Contains(property.CustomFieldOptionId))
                                    {
                                        price += property.OverriddenPrice;

                                        XmlHelper.AddNode(doc, optionXml, "IsActive", "true");
                                    }
                                }
                            }
                        }
                    }
                }

                if (cartItemXml["Price"] != null)
                    cartItemXml["Price"].InnerText = ProductHelper.FormatPrice(price, true);
                else
                    XmlHelper.AddNode(doc, cartItemXml, "Price", ProductHelper.FormatPrice(price, true));

                XmlHelper.AddNode(doc, cartItemXml, "Quantity", cartItem.Quantity.ToString());
                XmlHelper.AddNode(doc, cartItemXml, "Discount", ProductHelper.FormatPrice(0, true));
                //XmlHelper.AddNode(doc, cartItemXml, "DiscountPercentage", "0");
                XmlHelper.AddNode(doc, cartItemXml, "ItemTotal", ProductHelper.FormatPrice(cartItem.Quantity * price, true));
                XmlHelper.AddNode(doc, cartItemXml, "ItemGuid", cartItem.Guid.ToString());
                XmlHelper.AddNode(doc, cartItemXml, "LastAddedItem", (cartItem.Guid == cartItemGuid).ToString().ToLower());

                if (!string.IsNullOrEmpty(ProductConfiguration.AllowedQuantities))
                {
                    ParseAllowedQuantities(ProductConfiguration.AllowedQuantities)
                        .ForEach(quantity =>
                        {
                            XmlElement quantityXml = doc.CreateElement("Quantities");
                            cartItemXml.AppendChild(quantityXml);
                            XmlHelper.AddNode(doc, cartItemXml, "Quantity", quantity.ToString());
                        });
                }

            }

            return doc;
        }

        private static List<int> ParseAllowedQuantities(string allowedQuantities)
        {
            var result = new List<int>();
            if (!string.IsNullOrWhiteSpace(allowedQuantities))
            {
                allowedQuantities
                    .SplitOnCharAndTrim(';')
                    .ForEach(qtyStr =>
                    {
                        if (!qtyStr.Contains("-"))
                        {
                            int qty;
                            if (int.TryParse(qtyStr.Trim(), out qty))
                                result.Add(qty);
                        }
                        else
                        {
                            var allowedRanges = allowedQuantities.SplitOnCharAndTrim('-');
                            if (allowedRanges.Count == 2)
                            {
                                int qty1;
                                int qty2;
                                if (int.TryParse(allowedRanges[0], out qty1) && int.TryParse(allowedRanges[2], out qty2))
                                    for (int i = qty1; i <= qty2; i++)
                                    {
                                        result.Add(i);
                                    }
                            }
                        }
                    });
            }

            return result;
        }

        #endregion

        // using dynamic
        public static string Json(this object result)
        {
            return StringHelper.ToJsonString(result);
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="cartItemGuid">Cart Item Guid</param>
        /// <param name="shoppingCartItemGuid">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public static IList<string> UpdateShoppingCartItem(
            Guid cartSessionGuid,
            Guid shoppingCartItemGuid, string attributesXml,
            int quantity = 1)
        {
            //    var warnings = new List<string>();

            //    var shoppingCartItem = new ShoppingCartItem(shoppingCartItemGuid);
            //    if (shoppingCartItem != null && shoppingCartItem.Guid != Guid.Empty)
            //    {
            //        if (quantity > 0)
            //        {
            //            ////check warnings
            //            //warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartItem.ShoppingCartType,
            //            //    shoppingCartItem.Product, shoppingCartItem.StoreId,
            //            //    selectedAttributes, customerEnteredPrice, quantity, false));
            //            if (warnings.Count == 0)
            //            {
            //                //if everything is OK, then update a shopping cart item
            //                shoppingCartItem.Quantity = quantity;
            //                shoppingCartItem.AttributesXml = selectedAttributes;
            //                shoppingCartItem.LastModUtc = DateTime.UtcNow;
            //                shoppingCartItem.Save();
            //            }
            //        }
            //        else
            //        {
            //            //delete a shopping cart item
            //            //DeleteShoppingCartItem(shoppingCartItem, true);
            //            ShoppingCartItem.Delete(shoppingCartItem.Guid);
            //        }
            //    }

            //    return warnings;

            if (cartSessionGuid == Guid.Empty)
                throw new ArgumentNullException("cartSessionGuid");

            var warnings = new List<string>();

            var shoppingCartItem = new ShoppingCartItem(shoppingCartItemGuid);
            if (shoppingCartItem != null && shoppingCartItem.UserGuid == cartSessionGuid)
            {
                if (quantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetShoppingCartItemWarnings(cartSessionGuid, (ShoppingCartTypeEnum)shoppingCartItem.ShoppingCartType,
                        shoppingCartItem.Product,
                        attributesXml, quantity, false));
                    if (warnings.Count == 0)
                    {
                        //if everything is OK, then update a shopping cart item
                        shoppingCartItem.Quantity = quantity;
                        shoppingCartItem.AttributesXml = attributesXml;
                        shoppingCartItem.LastModUtc = DateTime.UtcNow;

                        shoppingCartItem.Save();
                    }
                }
                else
                {
                    //delete a shopping cart item
                    ShoppingCartItem.Delete(shoppingCartItem.Guid);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Gets a number of product in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Result</returns>
        public static int GetTotalProducts(this IList<ShoppingCartItem> shoppingCart)
        {
            int result = 0;
            foreach (ShoppingCartItem sci in shoppingCart)
                result += sci.Quantity;

            return result;
        }

        public static int GetTotalProducts(this IList<OrderItem> orderItems)
        {
            int result = 0;
            foreach (OrderItem sci in orderItems)
                result += sci.Quantity;

            return result;
        }

        public static decimal GetTotalWeights(this IList<ShoppingCartItem> shoppingCart, List<Product> lstProductsInCart)
        {
            decimal result = 0;
            //foreach (ShoppingCartItem sci in shoppingCart)
            //{
            //    Product product = ProductHelper.GetProductFromList(lstProductsInCart, sci.ProductId);
            //    if (product != null)
            //        result += (sci.Quantity * product.Weight);
            //}

            return result;
        }

        public static decimal GetTotalWeights(this IList<OrderItem> orderItems, List<Product> lstProducts)
        {
            decimal result = 0;

            return result;
        }

        public static decimal GetDiscountTotal(this IList<ShoppingCartItem> shoppingCart, List<Product> lstProductsInCart)
        {
            decimal result = 0;
            //foreach (ShoppingCartItem sci in shoppingCart)
            //{
            //    Product product = ProductHelper.GetProductFromList(lstProductsInCart, sci.ProductId);
            //    if (product != null)
            //        result += (sci.Quantity * product.Weight);
            //}

            return result;
        }

        public static decimal GetSubTotal(this IList<ShoppingCartItem> shoppingCart, List<Product> lstProductsInCart)
        {
            decimal result = 0;
            foreach (ShoppingCartItem sci in shoppingCart)
            {
                Product product = ProductHelper.GetProductFromList(lstProductsInCart, sci.ProductId);
                if (product != null)
                {
                    decimal price = ProductHelper.GetPrice(product);
                    result += (sci.Quantity * price);
                }
            }

            return result;
        }

        #region Add to cart
        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public static IList<string> AddToCart(Product product,
            ShoppingCartTypeEnum shoppingCartType, out Guid shoppingCartItemGuid, string attributesXml = null,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true)
        {
            var cartSessionGuid = GetCartSessionGuid(product.SiteId, true);
            shoppingCartItemGuid = Guid.Empty;

            if (cartSessionGuid == Guid.Empty)
                throw new ArgumentNullException("cartSessionGuid");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartTypeEnum.ShoppingCart && !ProductConfiguration.EnableShoppingCart)
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }
            if (shoppingCartType == ShoppingCartTypeEnum.Wishlist && !ProductConfiguration.EnableWishlist)
            {
                warnings.Add("Wishlist is disabled");
                return warnings;
            }
            if (quantity <= 0)
            {
                warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartQuantityShouldPositive"));
                return warnings;
            }

            var cart = ShoppingCartItem.GetByUserGuid(product.SiteId, shoppingCartType, cartSessionGuid);

            var shoppingCartItem = FindShoppingCartItemInTheCart(cart,
                shoppingCartType, product, attributesXml);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(GetShoppingCartItemWarnings(cartSessionGuid, shoppingCartType, product,
                    attributesXml,
                    newQuantity, automaticallyAddRequiredProductsIfEnabled));

                if (warnings.Count == 0)
                {
                    shoppingCartItem.AttributesXml = attributesXml;
                    shoppingCartItem.Quantity = newQuantity;
                    shoppingCartItem.LastModUtc = DateTime.UtcNow;
                    shoppingCartItem.Save();
                }

                shoppingCartItemGuid = shoppingCartItem.Guid;
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(GetShoppingCartItemWarnings(cartSessionGuid, shoppingCartType, product,
                    attributesXml,
                    quantity, automaticallyAddRequiredProductsIfEnabled));
                if (warnings.Count == 0)
                {
                    //maximum items validation
                    switch (shoppingCartType)
                    {
                        case ShoppingCartTypeEnum.ShoppingCart:
                            {
                                if (cart.Count >= ProductConfiguration.MaximumShoppingCartItems)
                                {
                                    warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartMaximumShoppingCartItems"), ProductConfiguration.MaximumShoppingCartItems));
                                    return warnings;
                                }
                            }
                            break;
                        case ShoppingCartTypeEnum.Wishlist:
                            {
                                if (cart.Count >= ProductConfiguration.MaximumWishlistItems)
                                {
                                    warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartMaximumWishlistItems"), ProductConfiguration.MaximumWishlistItems));
                                    return warnings;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    DateTime now = DateTime.UtcNow;
                    shoppingCartItem = new ShoppingCartItem
                    {
                        ShoppingCartType = (int)shoppingCartType,
                        SiteId = product.SiteId,
                        ProductId = product.ProductId,
                        AttributesXml = attributesXml,
                        Quantity = quantity,
                        CreatedUtc = now,
                        LastModUtc = now,
                        CreatedFromIP = SiteUtils.GetIP4Address(),
                        UserGuid = cartSessionGuid
                    };

                    shoppingCartItem.Save();
                    shoppingCartItemGuid = shoppingCartItem.Guid;
                }
            }

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Found shopping cart item</returns>
        private static ShoppingCartItem FindShoppingCartItemInTheCart(
            IList<ShoppingCartItem> shoppingCart,
            ShoppingCartTypeEnum shoppingCartType,
            Product product,
            string attributesXml = "")
        {
            if (shoppingCart == null)
                throw new ArgumentNullException("shoppingCart");

            if (product == null)
                throw new ArgumentNullException("product");

            // Query customfields if attribute applied
            List<CustomField> customFields = new List<CustomField>();
            if (ProductConfiguration.EnableShoppingCartAttributes)
            {
                if (!string.IsNullOrEmpty(attributesXml))
                    customFields = CustomField.GetActiveForCart(product.SiteId, Product.FeatureGuid);
                else
                    foreach (var sci in shoppingCart.Where(a => a.ShoppingCartType == (int)shoppingCartType))
                    {
                        if (!string.IsNullOrEmpty(sci.AttributesXml))
                        {
                            customFields = CustomField.GetActiveForCart(product.SiteId, Product.FeatureGuid);
                            break;
                        }
                    }
            }

            foreach (var sci in shoppingCart.Where(a => a.ShoppingCartType == (int)shoppingCartType))
            {
                if (sci.ProductId == product.ProductId)
                {
                    //attributes
                    bool attributesEqual = AreProductAttributesEqual(customFields, sci.AttributesXml, attributesXml, false);

                    //gift cards
                    bool giftCardInfoSame = true;

                    //rental products
                    bool rentalInfoEqual = true;

                    //found?
                    if (attributesEqual && giftCardInfoSame && rentalInfoEqual)
                        return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Are attributes equal
        /// </summary>
        /// <param name="attributesXml1">The attributes of the first product</param>
        /// <param name="attributesXml2">The attributes of the second product</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Result</returns>
        private static bool AreProductAttributesEqual(List<CustomField> customFields, string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes)
        {
            var attributes1 = ProductAttributeParser.ParseProductAttributeMappings(customFields, attributesXml1);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(x => !x.IsNonCombinable).ToList();
            }
            var attributes2 = ProductAttributeParser.ParseProductAttributeMappings(customFields, attributesXml2);
            if (ignoreNonCombinableAttributes)
            {
                attributes2 = attributes2.Where(x => !x.IsNonCombinable).ToList();
            }
            if (attributes1.Count != attributes2.Count)
                return false;

            bool attributesEqual = true;
            foreach (var a1 in attributes1)
            {
                bool hasAttribute = false;
                foreach (var a2 in attributes2)
                {
                    if (a1.CustomFieldId == a2.CustomFieldId)
                    {
                        hasAttribute = true;
                        var values1Str = ProductAttributeParser.ParseValues(attributesXml1, a1.CustomFieldId);
                        var values2Str = ProductAttributeParser.ParseValues(attributesXml2, a2.CustomFieldId);
                        if (values1Str.Count == values2Str.Count)
                        {
                            foreach (int str1 in values1Str)
                            {
                                bool hasValue = false;
                                foreach (int str2 in values2Str)
                                {
                                    //case insensitive? 
                                    //if (str1.Trim().ToLower() == str2.Trim().ToLower())
                                    if (str1 == str2)
                                    {
                                        hasValue = true;
                                        break;
                                    }
                                }

                                if (!hasValue)
                                {
                                    attributesEqual = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            attributesEqual = false;
                            break;
                        }
                    }
                }

                if (hasAttribute == false)
                {
                    attributesEqual = false;
                    break;
                }
            }

            return attributesEqual;
        }

        #endregion

        #region Shopping cart warning

        public static IList<string> GetShoppingCartItemWarnings(Guid cartSessionGuid, ShoppingCartTypeEnum shoppingCartType,
            Product product,
            string attributesXml,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true,
            bool getStandardWarnings = true, bool getAttributesWarnings = true)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(cartSessionGuid, shoppingCartType, product, attributesXml, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetShoppingCartItemAttributeWarnings(cartSessionGuid, shoppingCartType, product, quantity, attributesXml));

            //gift cards

            //required products

            //rental products

            return warnings;
        }

        /// <summary>
        /// Validates a product for standard properties
        /// </summary>
        /// <param name="cartSessionGuid">cartSessionGuid</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public static IList<string> GetStandardWarnings(Guid cartSessionGuid, ShoppingCartTypeEnum shoppingCartType,
            Product product, string attributesXml,
            int quantity)
        {
            if (cartSessionGuid == Guid.Empty)
                throw new ArgumentNullException("cartSessionGuid");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //deleted
            if (product.IsDeleted)
            {
                warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartProductDeleted"));
                return warnings;
            }

            //published
            if (!product.IsPublished)
            {
                warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartProductUnpublished"));
            }

            ////we can add only simple products
            //if (product.ProductType != ProductType.SimpleProduct)
            //{
            //    warnings.Add("This is not simple product");
            //}

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartTypeEnum.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartBuyingDisabled"));
            }

            ////disabled "add to wishlist" button
            //if (shoppingCartType == ShoppingCartTypeEnum.Wishlist && product.DisableWishlistButton)
            //{
            //    warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartWishlistDisabled"));
            //}

            //call for price

            //customer entered price

            //quantity validation
            var hasQtyWarnings = false;
            //if (quantity < product.OrderMinimumQuantity)
            //{
            //    warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartMinimumQuantity"), product.OrderMinimumQuantity));
            //    hasQtyWarnings = true;
            //}
            if (quantity > ProductConfiguration.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartMaximumQuantity"), ProductConfiguration.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }
            //var allowedQuantities = product.ParseAllowedQuantities();
            //if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            //{
            //    warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartAllowedQuantities"), string.Join(", ", allowedQuantities)));
            //}

            //var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            //if (validateOutOfStock && !hasQtyWarnings)
            //{
            //    switch (product.ManageInventoryMethod)
            //    {
            //        case ManageInventoryMethod.DontManageStock:
            //            {
            //                //do nothing
            //            }
            //            break;
            //        case ManageInventoryMethod.ManageStock:
            //            {
            //                if (product.BackorderMode == BackorderMode.NoBackorders)
            //                {
            //                    int maximumQuantityCanBeAdded = product.GetTotalStockQuantity();
            //                    if (maximumQuantityCanBeAdded < quantity)
            //                    {
            //                        if (maximumQuantityCanBeAdded <= 0)
            //                            warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartOutOfStock"));
            //                        else
            //                            warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartQuantityExceedsStock"), maximumQuantityCanBeAdded));
            //                    }
            //                }
            //            }
            //            break;
            //        case ManageInventoryMethod.ManageStockByAttributes:
            //            {
            //                var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            //                if (combination != null)
            //                {
            //                    //combination exists
            //                    //let's check stock level
            //                    if (!combination.AllowOutOfStockOrders && combination.StockQuantity < quantity)
            //                    {
            //                        int maximumQuantityCanBeAdded = combination.StockQuantity;
            //                        if (maximumQuantityCanBeAdded <= 0)
            //                        {
            //                            warnings.Add(ResourceHelper.GetResourceString("ProductResources", "CartOutOfStock"));
            //                        }
            //                        else
            //                        {
            //                            warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartQuantityExceedsStock"), maximumQuantityCanBeAdded));
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    //combination doesn't exist
            //                    if (product.AllowAddingOnlyExistingAttributeCombinations)
            //                    {
            //                        //maybe, is it better  to display something like "No such product/combination" message?
            //                        warnings.Add(ResourceHelper.GetResourceString("ProductResources", "ShoppingCart.OutOfStock"));
            //                    }
            //                }
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //availability Out Of Stock

            //availability dates

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="cartSessionGuid">Cart Session Guid</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Warnings</returns>
        public static IList<string> GetShoppingCartItemAttributeWarnings(Guid cartSessionGuid,
            ShoppingCartTypeEnum shoppingCartType,
            Product product,
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            ////ensure it's our attributes
            //var attributes1 = ParseProductAttributeMappings(attributesXml);
            //if (ignoreNonCombinableAttributes)
            //{
            //    attributes1 = attributes1.Where(x => !x.IsNonCombinable).ToList();
            //}
            //foreach (var attribute in attributes1)
            //{
            //    ProductProperty.GetPropertiesByField(attribute.CustomFieldId);
            //    if (attribute.Product != null)
            //    {
            //        if (attribute.Product.Id != product.ProductId)
            //        {
            //            warnings.Add("Attribute error");
            //        }
            //    }
            //    else
            //    {
            //        warnings.Add("Attribute error");
            //        return warnings;
            //    }
            //}

            ////validate required product attributes (whether they're chosen/selected/entered)
            //var attributes2 = _productAttributeService.GetProductAttributeMappingsByProductId(product.ProductId);
            //if (ignoreNonCombinableAttributes)
            //{
            //    attributes2 = attributes2.Where(x => !x.IsNonCombinable).ToList();
            //}
            //foreach (var a2 in attributes2)
            //{
            //    if (a2.IsRequired)
            //    {
            //        bool found = false;
            //        //selected product attributes
            //        foreach (var a1 in attributes1)
            //        {
            //            if (a1.Id == a2.Id)
            //            {
            //                var attributeValuesStr = ParseValues(attributesXml, a1.Id);
            //                foreach (string str1 in attributeValuesStr)
            //                {
            //                    if (!String.IsNullOrEmpty(str1.Trim()))
            //                    {
            //                        found = true;
            //                        break;
            //                    }
            //                }
            //            }
            //        }

            //        //if not found
            //        if (!found)
            //        {
            //            var notFoundWarning = !string.IsNullOrEmpty(a2.TextPrompt) ?
            //                a2.TextPrompt :
            //                string.Format(ResourceHelper.GetResourceString("ProductResources", "CartSelectAttribute"), a2.ProductAttribute.GetLocalized(a => a.Name));

            //            warnings.Add(notFoundWarning);
            //        }
            //    }

            //    if (a2.AttributeControlType == AttributeControlType.ReadonlyCheckboxes)
            //    {
            //        //customers cannot edit read-only attributes
            //        var allowedReadOnlyValueIds = _productAttributeService.GetProductAttributeValues(a2.Id)
            //            .Where(x => x.IsPreSelected)
            //            .Select(x => x.Id)
            //            .ToArray();

            //        var selectedReadOnlyValueIds = _productAttributeParser.ParseProductAttributeValues(attributesXml)
            //            .Where(x => x.ProductAttributeMappingId == a2.Id)
            //            .Select(x => x.Id)
            //            .ToArray();

            //        if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
            //        {
            //            warnings.Add("You cannot change read-only values");
            //        }
            //    }
            //}

            ////validation rules
            //foreach (var pam in attributes2)
            //{
            //    if (!pam.ValidationRulesAllowed())
            //        continue;

            //    //minimum length
            //    if (pam.ValidationMinLength.HasValue)
            //    {
            //        if (pam.AttributeControlType == CustomFieldDataType.Text ||
            //            pam.AttributeControlType == AttributeControlType.MultilineTextbox)
            //        {
            //            var valuesStr = _productAttributeParser.ParseValues(attributesXml, pam.Id);
            //            var enteredText = valuesStr.FirstOrDefault();
            //            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

            //            if (pam.ValidationMinLength.Value > enteredTextLength)
            //            {
            //                warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartTextboxMinimumLength"), pam.ProductAttribute.GetLocalized(a => a.Name), pam.ValidationMinLength.Value));
            //            }
            //        }
            //    }

            //    //maximum length
            //    if (pam.ValidationMaxLength.HasValue)
            //    {
            //        if (pam.AttributeControlType == AttributeControlType.TextBox ||
            //            pam.AttributeControlType == AttributeControlType.MultilineTextbox)
            //        {
            //            var valuesStr = _productAttributeParser.ParseValues(attributesXml, pam.Id);
            //            var enteredText = valuesStr.FirstOrDefault();
            //            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

            //            if (pam.ValidationMaxLength.Value < enteredTextLength)
            //            {
            //                warnings.Add(string.Format(ResourceHelper.GetResourceString("ProductResources", "CartTextboxMaximumLength"), pam.ProductAttribute.GetLocalized(a => a.Name), pam.ValidationMaxLength.Value));
            //            }
            //        }
            //    }
            //}

            //if (warnings.Count > 0)
            //    return warnings;

            ////validate bundled products
            //var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            //foreach (var attributeValue in attributeValues)
            //{
            //    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
            //    {
            //        if (ignoreNonCombinableAttributes && attributeValue.ProductAttributeMapping.IsNonCombinable())
            //            continue;

            //        //associated product (bundle)
            //        var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
            //        if (associatedProduct != null)
            //        {
            //            var totalQty = quantity * attributeValue.Quantity;
            //            var associatedProductWarnings = GetShoppingCartItemWarnings(customer,
            //                shoppingCartType, associatedProduct, _storeContext.CurrentStore.Id,
            //                "", decimal.Zero, null, null, totalQty, false);
            //            foreach (var associatedProductWarning in associatedProductWarnings)
            //            {
            //                var attributeName = attributeValue.ProductAttributeMapping.ProductAttribute.GetLocalized(a => a.Name);
            //                var attributeValueName = attributeValue.GetLocalized(a => a.Name);
            //                warnings.Add(string.Format(
            //                    ResourceHelper.GetResourceString("ProductResources", "CartAssociatedAttributeWarning"),
            //                    attributeName, attributeValueName, associatedProductWarning));
            //            }
            //        }
            //        else
            //        {
            //            warnings.Add(string.Format("Associated product cannot be loaded - {0}", attributeValue.AssociatedProductId));
            //        }
            //    }
            //}

            return warnings;
        }

        #endregion

        public static string RemoveFromCart(Guid itemGuid)
        {
            if (!ProductConfiguration.EnableShoppingCart)
                return Json(new
                {
                    success = false,
                    redirect = SiteUtils.GetHomepageUrl()
                });

            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
            var cart = GetShoppingCart(siteSettings.SiteId, ShoppingCartTypeEnum.ShoppingCart);

            foreach (var sci in cart)
            {
                if (sci.Guid == itemGuid)
                {
                    ShoppingCartItem.Delete(sci.Guid);
                    break;
                }
            }

            return Json(new
            {
                success = true,
                redirect = GetCartUrl()
            });
        }

        public static string UpdateCart(NameValueCollection form)
        {
            if (!ProductConfiguration.EnableShoppingCart)
                return Json(new
                {
                    success = false,
                    redirect = SiteUtils.GetHomepageUrl()
                });

            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
            var cart = GetShoppingCart(siteSettings.SiteId, ShoppingCartTypeEnum.ShoppingCart);

            var allGuidsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Guid.Parse(x)).ToList() : new List<Guid>();

            var cartSessionGuid = CartHelper.GetCartSessionGuid(siteSettings.SiteId, true);
            var lstProductsInCart = Product.GetByShoppingCart(siteSettings.SiteId, cartSessionGuid);

            List<CustomField> customFields = new List<CustomField>();
            if (ProductConfiguration.EnableShoppingCartAttributes)
                customFields = CustomField.GetActiveForCart(siteSettings.SiteId, Product.FeatureGuid);

            foreach (var sci in cart)
            {
                bool remove = allGuidsToRemove.Contains(sci.Guid);
                if (remove)
                    ShoppingCartItem.Delete(sci.Guid);
                else
                {
                    int newQuantity = sci.Quantity;
                    string quantityKey = string.Format("itemquantity{0}", sci.Guid);
                    if (form[quantityKey] != null)
                    {
                        int.TryParse(form[quantityKey], out newQuantity);

                        if (newQuantity <= 0)
                        {
                            ShoppingCartItem.Delete(sci.Guid);
                            continue;
                        }
                    }

                    string attributesXml = string.Empty;
                    if (ProductConfiguration.EnableShoppingCartAttributes)
                    {
                        foreach (var attribute in customFields)
                        {
                            string controlId = string.Format("product_attribute_{0}_{1}", sci.Guid, attribute.CustomFieldId);
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = CustomFieldHelper.AddProductAttribute(attributesXml, attribute, selectedAttributeId.ToString());
                            }
                        }
                    }

                    Product product = ProductHelper.GetProductFromList(lstProductsInCart, sci.ProductId);
                    //check warnings
                    var warnings = CartHelper.GetShoppingCartItemWarnings(cartSessionGuid, ShoppingCartTypeEnum.ShoppingCart,
                        product, attributesXml, newQuantity, false, true, false);
                    if (warnings.Count == 0)
                    {
                        sci.AttributesXml = attributesXml;
                        sci.Quantity = newQuantity;
                        sci.Save();
                    }
                }
            }

            if (ProductConfiguration.EnableShoppingCartAttributes)
            {
                for (int i = cart.Count - 1; i >= 0; i--)
                {
                    var sci = cart[i];
                    for (int j = cart.Count - 1; j >= 0; j--)
                    {
                        var sci2 = cart[j];

                        if (
                            sci.Guid != sci2.Guid
                            && sci.ProductId == sci2.ProductId
                            && AreProductAttributesEqual(customFields, sci.AttributesXml, sci2.AttributesXml, false)
                            )
                        {
                            sci.Quantity += sci2.Quantity;
                            sci.Save();

                            ShoppingCartItem.Delete(sci2.Guid);
                            cart.Remove(sci2);

                            break;
                        }
                    }
                }
            }


            ////display notification message and update appropriate blocks
            //var updatetopcartsectionhtml = string.Format(ResourceHelper.GetResourceString("ProductResources", "CartQuantityFormat"),
            //        CartHelper.GetShoppingCart(siteSettings.SiteId, ShoppingCartTypeEnum.ShoppingCart).GetTotalProducts());

            //var updateflyoutcartsectionhtml = ProductConfiguration.MiniShoppingCartEnabled
            //    ? PrepareShoppingCart(siteSettings.SiteId) : "";

            return Json(new
            {
                success = true,
                redirect = GetCartUrl()
                //message = string.Format(ResourceHelper.GetResourceString("ProductResources", "CartProductHasBeenAddedFormat"), GetCartUrl()),
                //updatetopcartsectionhtml = updatetopcartsectionhtml,
                //updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
            });
        }

        public static string GetCartUrl()
        {
            return GetCartUrl(SiteUtils.GetNavigationSiteRoot());
        }

        public static string GetCartUrl(string siteRoot)
        {
            return siteRoot + ProductConfiguration.CartPageUrl;
        }

        public static void SetupRedirectToCartPage(System.Web.UI.Control control)
        {
            string cartPage = SiteUtils.GetNavigationSiteRoot() + ProductConfiguration.CartPageUrl;
            string rawUrl = HttpContext.Current.Request.RawUrl;
            if (!cartPage.ContainsCaseInsensitive(rawUrl))
                WebUtils.SetupRedirect(control, cartPage);
        }

        public static string GetZoneUrl(int zoneId)
        {
            try
            {
                return SiteUtils.GetZoneUrl(zoneId);
            }
            catch (Exception) { }

            return SiteUtils.GetHomepageUrl();
        }

        public static string GetWishlistUrl()
        {
            return GetWishlistUrl(SiteUtils.GetNavigationSiteRoot());
        }

        public static string GetWishlistUrl(string siteRoot)
        {
            return siteRoot + ProductConfiguration.WishlistPageUrl;
        }

        /// <summary>
        /// Gets the last page for "Continue shopping" button on shopping cart page
        /// </summary>
        public static string LastContinueShoppingPage
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["LastContinueShoppingPage"] != null))
                {
                    return HttpContext.Current.Session["LastContinueShoppingPage"].ToString();
                }
                return string.Empty;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["LastContinueShoppingPage"] = value;
                }
            }
        }

    }
}