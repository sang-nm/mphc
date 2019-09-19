/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-23
/// Last Modified:			2014-06-23

using System;
using System.Collections;
using CanhCam.Web.Framework;
using System.Collections.Generic;

namespace CanhCam.Web.ProductUI {
	/// <summary>
	/// Encapsulates the feature instance configuration loaded from module settings into a more friendly object
	/// </summary>
	public partial class ProductConfiguration {
		public ProductConfiguration()
		{ }

		public ProductConfiguration(Hashtable settings) {
			LoadSettings(settings);
		}

		private void LoadSettings(Hashtable settings) {
			if (settings == null || settings.Count == 0) { return; throw new ArgumentException("must pass in a hashtable of settings"); }

			if (settings.Contains("ProductsPerPageSetting")) {
				string pageSizeOptionsString = settings["ProductsPerPageSetting"].ToString().Trim();
				pageSizeOptions = pageSizeOptionsString.SplitOnCharAndTrim(';');

				if (pageSizeOptions.Count > 0)
					int.TryParse(pageSizeOptions[0], out pageSize);
			}

			showLeftContent = WebUtils.ParseBoolFromHashtable(settings, "ShowPageLeftContentSetting", showLeftContent);
			showRightContent = WebUtils.ParseBoolFromHashtable(settings, "ShowPageRightContentSetting", showRightContent);
			showAllProducts = WebUtils.ParseBoolFromHashtable(settings, "ShowAllProductsFromChildZoneSetting", showAllProducts);

			relatedProductsToShow = WebUtils.ParseInt32FromHashtable(settings, "RelatedProductsToShow", relatedProductsToShow);
			otherProductsPerPage = WebUtils.ParseInt32FromHashtable(settings, "OtherProductsPerPageSetting", otherProductsPerPage);

			if (settings["XsltFileName"] != null)
				xsltFileName = settings["XsltFileName"].ToString();
			if (settings["XsltFileNameDetailPage"] != null)
				xsltFileNameDetailPage = settings["XsltFileNameDetailPage"].ToString();

			if (settings["LoadFirstProductSetting"] != null)
				loadFirstProduct = WebUtils.ParseBoolFromHashtable(settings, "LoadFirstProductSetting", loadFirstProduct);

			if (settings["ParentZonesSetting"] != null)
				zoneIds = settings["ParentZonesSetting"].ToString().Trim();

			showCustomFieldsInProductList = WebUtils.ParseBoolFromHashtable(settings, "ShowCustomFieldsInProductList",
			                                showCustomFieldsInProductList);
			showHiddenContents = WebUtils.ParseBoolFromHashtable(settings, "ShowHiddenContentsOnDetailPage", showHiddenContents);
			hideOtherContentsOnDetailPage = WebUtils.ParseBoolFromHashtable(settings, "HideOtherContentsOnDetailPage",
			                                hideOtherContentsOnDetailPage);
			enableKeywordFiltering = WebUtils.ParseBoolFromHashtable(settings, "EnableKeywordFiltering", enableKeywordFiltering);
		}

		private bool enableKeywordFiltering = false;
		public bool EnableKeywordFiltering {
			get { return enableKeywordFiltering; }
		}

		private bool showHiddenContents = false;
		public bool ShowHiddenContents {
			get { return showHiddenContents; }
		}

		private bool hideOtherContentsOnDetailPage = false;
		public bool HideOtherContentsOnDetailPage {
			get { return hideOtherContentsOnDetailPage; }
		}

		private bool showCustomFieldsInProductList = false;
		public bool ShowCustomFieldsInProductList {
			get { return showCustomFieldsInProductList; }
		}

		private string xsltFileName = "ProductList.xslt";
		public string XsltFileName {
			get { return xsltFileName; }
		}

		private string xsltFileNameDetailPage = "ProductDetail.xslt";
		public string XsltFileNameDetailPage {
			get { return xsltFileNameDetailPage; }
		}

		private bool loadFirstProduct = false;
		public bool LoadFirstProduct {
			get { return loadFirstProduct; }
		}

		private int relatedProductsToShow = 12;
		public int RelatedProductsToShow {
			get { return relatedProductsToShow; }
		}

		private int otherProductsPerPage = 9;
		public int OtherProductsPerPage {
			get { return otherProductsPerPage; }
		}

		#region Comment System

		private bool notifyOnComment = false;
		public bool NotifyOnComment {
			get { return notifyOnComment; }
		}

		//private bool allowComments = false;
		//public bool AllowComments
		//{
		//    get { return allowComments; }
		//}

		public static bool UseLegacyCommentSystem {
			get { return ConfigHelper.GetBoolProperty("Product:UseLegacyCommentSystem", true); }
		}

		private bool allowCommentTitle = true;
		public bool AllowCommentTitle {
			get { return allowCommentTitle; }
		}

		private bool useCaptcha = false;
		public bool UseCaptcha {
			get { return useCaptcha; }
		}

		private bool requireAuthenticationForComments = false;
		public bool RequireAuthenticationForComments {
			get { return requireAuthenticationForComments; }
		}

		private string commentSystem = "internal";
		public string CommentSystem {
			get { return commentSystem; }
		}

		private string zoneIds = string.Empty;
		public string ZoneIds {
			get { return zoneIds; }
		}

		#endregion

		private string notifyEmail = string.Empty;
		public string NotifyEmail {
			get { return notifyEmail; }
		}

		private bool enableContentVersioning = false;
		public bool EnableContentVersioning {
			get { return enableContentVersioning; }
		}

		private bool showAllProducts = false;
		public bool ShowAllProducts {
			get { return showAllProducts; }
		}

		private bool showLeftContent = true;
		public bool ShowLeftContent {
			get { return showLeftContent; }
		}

		private bool showRightContent = true;
		public bool ShowRightContent {
			get { return showRightContent; }
		}

		private int pageSize = 12;
		public int PageSize {
			get { return pageSize; }
		}

		private List<string> pageSizeOptions = new List<string>();
		public List<string> PageSizeOptions {
			get { return pageSizeOptions; }
		}

		public static string BingMapDistanceUnit {
			get { return ConfigHelper.GetStringProperty("Product:BingMapDistanceUnit", "VERouteDistanceUnit.Mile"); }
		}

		/// <summary>
		/// If true and the skin is using altcontent1 it will load the page content for that in the news detail view
		/// </summary>
		public static bool ShowTopContent {
			get { return ConfigHelper.GetBoolProperty("Product:ShowTopContent", true); }
		}

		/// <summary>
		/// If true and the skin is using altcontent2 it will load the page content for that in the news detail view
		/// </summary>
		public static bool ShowBottomContent {
			get { return ConfigHelper.GetBoolProperty("Product:ShowBottomContent", true); }
		}

		/// <summary>
		/// 165 is the max recommended by google
		/// </summary>
		public static int MetaDescriptionMaxLengthToGenerate {
			get { return ConfigHelper.GetIntProperty("Product:MetaDescriptionMaxLengthToGenerate", 165); }
		}

		public static bool UseNoIndexFollowMetaOnLists {
			get { return ConfigHelper.GetBoolProperty("Product:UseNoIndexFollowMetaOnLists", true); }
		}

		public static bool UseHtmlDiff {
			get { return ConfigHelper.GetBoolProperty("Product:UseHtmlDiff", true); }
		}

		public static bool UseImages {
			get { return ConfigHelper.GetBoolProperty("Product:UseImages", true); }
		}

		public static bool EnableShoppingCart {
			get { return ConfigHelper.GetBoolProperty("Product:EnableShoppingCart", true); }
		}
		public static bool EnableWishlist {
			get { return ConfigHelper.GetBoolProperty("Product:EnableWishlist", false); }
		}
		public static int MaximumShoppingCartItems {
			get { return ConfigHelper.GetIntProperty("Product:MaximumShoppingCartItems", 20); }
		}
		public static int MaximumWishlistItems {
			get { return ConfigHelper.GetIntProperty("Product:MaximumWishlistItems", 20); }
		}
		public static int OrderMaximumQuantity {
			get { return ConfigHelper.GetIntProperty("Product:OrderMaximumQuantity", 1000); }
		}
		public static string AllowedQuantities {
			get { return ConfigHelper.GetStringProperty("Product:AllowedQuantities", string.Empty); }
		}
		public static bool DisplayCartAfterAddingProduct {
			get { return ConfigHelper.GetBoolProperty("Product:DisplayCartAfterAddingProduct", false); }
		}
		public static bool DisplayWishlistAfterAddingProduct {
			get { return ConfigHelper.GetBoolProperty("Product:DisplayWishlistAfterAddingProduct", false); }
		}
		public static bool AllowCartItemEditing {
			get { return ConfigHelper.GetBoolProperty("Product:AllowCartItemEditing", true); }
		}
		public static bool MiniShoppingCartEnabled {
			get { return ConfigHelper.GetBoolProperty("Product:MiniShoppingCartEnabled", true); }
		}
		public static bool ShowProductImagesInMiniShoppingCart {
			get { return ConfigHelper.GetBoolProperty("Product:ShowProductImagesInMiniShoppingCart", true); }
		}

		public static bool FilterProductByTopLevelParentZones {
			get { return ConfigHelper.GetBoolProperty("Product:FilterProductByTopLevelParentZones", false); }
		}

		public static bool RelatedProductsTwoWayRelationship {
			get { return ConfigHelper.GetBoolProperty("Product:RelatedProductsTwoWayRelationship", false); }
		}

		public static string WorkingCurrency {
			get { return ConfigHelper.GetStringProperty("Product:WorkingCurrency", "₫"); }
		}
		public static string CurrencyFormatting {
			get { return ConfigHelper.GetStringProperty("Product:CurrencyFormatting", "#,##0"); }
		}

		public static string CartPageUrl {
			get { return ConfigHelper.GetStringProperty("Product:CartPageUrl", "/cart"); }
		}
		public static string WishlistPageUrl {
			get { return ConfigHelper.GetStringProperty("Product:WishlistPageUrl", "/wishlist"); }
		}
		//public static bool OnePageCheckoutEnabled
		//{
		//    get { return ConfigHelper.GetBoolProperty("Product:OnePageCheckoutEnabled", false); }
		//}
		public static bool AnonymousCheckoutAllowed {
			get { return ConfigHelper.GetBoolProperty("Product:AnonymousCheckoutAllowed", true); }
		}

		public static string OrderCodeDateFormat {
			get { return ConfigHelper.GetStringProperty("Product:OrderCodeDateFormat", "yyMMdd"); }
		}
		public static int OrderCodeMinimumLength {
			get { return ConfigHelper.GetIntProperty("Product:OrderCodeMinimumLength", 3); }
		}
		public static string OrderAddressRequiredFields {
			get { return ConfigHelper.GetStringProperty("Product:OrderAddressRequiredFields", "Address_FirstName|Address_Address|Address_Phone"); }
		}

		public static bool EnableComparing {
			get { return ConfigHelper.GetBoolProperty("Product:EnableComparing", false); }
		}

		public static int MaximumCompareItems {
			get { return ConfigHelper.GetIntProperty("Product:MaximumCompareItems", 3); }
		}

		public static bool EnableShoppingCartAttributes {
			get { return ConfigHelper.GetBoolProperty("Product:EnableShoppingCartAttributes", false); }
		}

		public static bool EnableAttributesPriceAdjustment {
			get { return ConfigHelper.GetBoolProperty("Product:EnableAttributesPriceAdjustment", false); }
		}

		public static bool RecentlyViewedProductsEnabled {
			get { return ConfigHelper.GetBoolProperty("Product:RecentlyViewedProductsEnabled", false); }
		}
		public static int RecentlyViewedProductCount {
			get { return ConfigHelper.GetIntProperty("Product:RecentlyViewedProductCount", 6); }
		}
        public static int OnePAYNoiDiaPaymentMethodId
        {
            get { return 4; }
        }
        public static int OnePAYQuocTePaymentMethodId
        {
            get { return 3; }
        }
	}
}