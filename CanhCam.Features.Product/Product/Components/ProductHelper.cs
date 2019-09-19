/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-23
/// Last Modified:			2014-06-23
/// 2015-06-04: added QueryStringKeywordParam
/// 2015-10-23: Enable shopping cart with product attributes

using System;
using System.Drawing;
using System.IO;
using log4net;
using CanhCam.Business;
using CanhCam.FileSystem;
using CanhCam.Web.Framework;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using System.Web;
using System.Collections.Generic;
using CanhCam.Business.WebHelpers;
using CanhCam.Net;
using System.Text;
using System.Xml;
using System.Linq;

namespace CanhCam.Web.ProductUI {
	public static class ProductHelper {
		private static readonly ILog log = LogManager.GetLogger(typeof(ProductHelper));

		public static void ProcessImage(ContentMedia contentImage, IFileSystem fileSystem, string virtualRoot,
		                                string originalFileName, Color backgroundColor) {
			string originalPath = virtualRoot + contentImage.MediaFile;
			string thumbnailPath = virtualRoot + "thumbs/" + contentImage.ThumbnailFile;

			fileSystem.CopyFile(originalPath, thumbnailPath, true);

			CanhCam.Web.ImageHelper.ResizeImage(
			    thumbnailPath,
			    IOHelper.GetMimeType(Path.GetExtension(thumbnailPath)),
			    contentImage.ThumbNailWidth,
			    contentImage.ThumbNailHeight,
			    backgroundColor);
		}

		public static void DeleteImages(ContentMedia contentImage, IFileSystem fileSystem, string virtualRoot) {
			string imageVirtualPath = virtualRoot + contentImage.MediaFile;

			if (!contentImage.MediaFile.ContainsCaseInsensitive("/"))
				fileSystem.DeleteFile(imageVirtualPath);

			if (!contentImage.ThumbnailFile.ContainsCaseInsensitive("/")) {
				imageVirtualPath = virtualRoot + "thumbs/" + contentImage.ThumbnailFile;
				fileSystem.DeleteFile(imageVirtualPath);
			}
		}

		public static string MediaFolderPath(int siteId) {
			return "~/Data/Sites/" + siteId.ToInvariantString() + "/Product/";
		}

		public static string MediaFolderPath(int siteId, int newsId) {
			return MediaFolderPath(siteId) + newsId.ToInvariantString() + "/";
		}

		public static string GetMediaFilePath(string mediaFolderPath, string mediaFile) {
			if (mediaFile.Length == 0)
				return string.Empty;

			if (mediaFile.Contains("/"))
				return mediaFile;

			return VirtualPathUtility.ToAbsolute(mediaFolderPath) + mediaFile;
		}

		public static string GetImageFilePath(int siteId, int productId, string imageFile, string thumbnail) {
			if (imageFile.Length == 0 && thumbnail.Length == 0)
				return string.Empty;

			if (thumbnail.Contains("/"))
				return thumbnail;

			string mediaFolderPath = System.Web.VirtualPathUtility.ToAbsolute(ProductHelper.MediaFolderPath(siteId, productId));
			if (!string.IsNullOrEmpty(thumbnail))
				return mediaFolderPath + "thumbs/" + thumbnail;

			if (imageFile.Contains("/"))
				return imageFile;

			return mediaFolderPath + imageFile;
		}

		public static string AttachmentsPath(int siteId, int newsId) {
			return MediaFolderPath(siteId, newsId) + "Attachments/";
		}

		public static string FormatProductUrl(string url, int productId, int zoneId) {
			if (url.Length > 0) {
				if (url.StartsWith("http")) {
					string siteRoot = WebUtils.GetSiteRoot();
					return url.Replace("http://~", siteRoot).Replace("https://~", siteRoot.Replace("http:", "https"));
				}

				return SiteUtils.GetNavigationSiteRoot(zoneId) + url.Replace("~", string.Empty);
			}

			return SiteUtils.GetNavigationSiteRoot(zoneId) + "/Product/ProductDetail.aspx?zoneid=" + zoneId.ToInvariantString()
			       + "&ProductID=" + productId.ToInvariantString();
		}

		public static string BuildProductLink(Product product) {
			return BuildProductLink(product.Url, product.ProductId, product.ZoneId, product.Title);
		}

		public static string BuildProductLink(string url, int productId, int zoneId, string title) {
			return string.Format("<a href='{0}'>{1}</a>", ProductHelper.FormatProductUrl(url, productId, zoneId), title);
		}

		public static string BuildProductImageLink(Product product, int imageWidth = 0) {
			if (product.ImageFile.Length > 0) {
				if (imageWidth <= 0)
					return string.Format("<a href='{0}'><img src='{1}' title='{2}' /></a>", ProductHelper.FormatProductUrl(product.Url,
					                     product.ProductId, product.ZoneId), VirtualPathUtility.ToAbsolute(ProductHelper.MediaFolderPath(product.SiteId,
					                             product.ProductId) + product.ImageFile), product.Title);

				return string.Format("<a href='{0}'><img src='{1}' title='{2}' width='{3}' /></a>",
				                     ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId),
				                     VirtualPathUtility.ToAbsolute(ProductHelper.MediaFolderPath(product.SiteId, product.ProductId) + product.ImageFile),
				                     product.Title, imageWidth.ToString());
			}

			return string.Empty;
		}

		public static string BuildEditLink(Product product, CmsBasePage basePage, bool userCanUpdate, SiteUser currentUser) {
			if (PageView.UserViewMode == PageViewMode.View) return string.Empty;

			if (!userCanUpdate)
				return string.Empty;

			if (!product.IsPublished) {
				if (currentUser == null)  return string.Empty;
				if (product.UserId != currentUser.UserId)
					return string.Empty;
			}

			if (!basePage.UserCanAuthorizeZone(product.ZoneId))
				return string.Empty;

			return "<a title='" + Resources.ProductResources.ProductEditLink
			       + "' class='edit-link' href='" + SiteUtils.GetNavigationSiteRoot() + "/Product/AdminCP/ProductEdit.aspx?ProductID=" +
			       product.ProductId
			       + "'> <i class='fa fa-pencil'></i></a>";
		}

		public static string BuildEditLink(int productId) {
			return "<a href='" + SiteUtils.GetNavigationSiteRoot()
			       + "/Product/AdminCP/ProductEdit.aspx?ProductID="
			       + productId
			       + "'>" + Resources.ProductResources.OrderDetailLink + "</a>";
		}

		public static string GetWishlistUrl() {
			return SiteUtils.GetNavigationSiteRoot() + "/Product/Wishlist.aspx";
		}

		public static string GetShoppingCartUrl() {
			return SiteUtils.GetNavigationSiteRoot() + "/Product/Cart.aspx";
		}

		public static void DeleteFolder(int siteId, int newsId) {
			string virtualPath = MediaFolderPath(siteId, newsId);
			string fileSystemPath = HostingEnvironment.MapPath(virtualPath);

			try {
				DeleteDirectory(fileSystemPath);
			} catch (Exception ex) {
				try {
					System.Threading.Thread.Sleep(0);
					Directory.Delete(fileSystemPath, false);
				} catch (Exception) {

				}

				//log.Error(ex);
			}
		}

		public static void DeleteDirectory(string fileSystemPath) {
			if (Directory.Exists(fileSystemPath)) {
				string[] files = Directory.GetFiles(fileSystemPath);
				string[] dirs = Directory.GetDirectories(fileSystemPath);

				foreach (string file in files) {
					File.SetAttributes(file, FileAttributes.Normal);
					File.Delete(file);
				}

				while (Directory.GetFiles(fileSystemPath).Length > 0)
					System.Threading.Thread.Sleep(10);

				foreach (string dir in dirs)
					DeleteDirectory(dir);

				Directory.Delete(fileSystemPath, true);
			}
		}

		public static bool VerifyProductFolders(IFileSystem fileSystem, string virtualRoot) {
			bool result = false;

			string originalPath = virtualRoot;
			string thumbnailPath = virtualRoot + "thumbs/";

			try {
				if (!fileSystem.FolderExists(originalPath))
					fileSystem.CreateFolder(originalPath);

				if (!fileSystem.FolderExists(thumbnailPath))
					fileSystem.CreateFolder(thumbnailPath);

				result = true;
			} catch (UnauthorizedAccessException ex) {
				log.Error("Error creating directories in ProductHelper.VerifyProductFolders", ex);
			}

			return result;
		}

		public static string GetRangeZoneIdsToSemiColonSeparatedString(int siteId, int parentZoneId) {
			SiteMapDataSource siteMapDataSource = new SiteMapDataSource();
			siteMapDataSource.SiteMapProvider = "canhcamsite" + siteId.ToInvariantString();

			SiteMapNode rootNode = siteMapDataSource.Provider.RootNode;
			SiteMapNode startingNode = null;

			if (rootNode == null)  return null;

			string listChildZoneIds = parentZoneId + ";";

			if (parentZoneId > -1) {
				SiteMapNodeCollection allNodes = rootNode.GetAllNodes();
				foreach (SiteMapNode childNode in allNodes) {
					gbSiteMapNode gbNode = childNode as gbSiteMapNode;
					if (gbNode == null)  continue;

					if (Convert.ToInt32(gbNode.Key) == parentZoneId) {
						startingNode = gbNode;
						break;
					}
				}
			} else
				startingNode = rootNode;

			if (startingNode == null)
				return string.Empty;

			SiteMapNodeCollection childNodes = startingNode.GetAllNodes();
			foreach (gbSiteMapNode childNode in childNodes)
				listChildZoneIds += childNode.Key + ";";

			return listChildZoneIds;
		}

		public static string GetProductTarget(object openInNewWindow) {
			if (openInNewWindow != null) {
				bool isBlank = (bool)openInNewWindow;

				if (isBlank)
					return "_blank";
			}

			return "_self";
		}

		public static Color GetColor(string resizeBackgroundColor) {
			try {
				return ColorTranslator.FromHtml(resizeBackgroundColor);
			} catch (Exception) {

			}

			return Color.White;
		}

		//public static Module FindProductModule(PageSettings currentPage)
		//{
		//    foreach (Module m in currentPage.Modules)
		//    {
		//        if (m.FeatureGuid == Product.FeatureGuid) return m;

		//        if (m.ControlSource == "Product/ProductModule.ascx") return m;

		//    }

		//    return null;
		//}

		public static void SendCommentNotification(
		    SmtpSettings smtpSettings,
		    Guid siteGuid,
		    string fromAddress,
		    string fromAlias,
		    string toEmail,
		    string replyEmail,
		    string ccEmail,
		    string bccEmail,
		    string subject,
		    string messageTemplate,
		    string siteName,
		    string messageToken) {

			if (string.IsNullOrEmpty(messageTemplate))
				return;

			StringBuilder message = new StringBuilder();
			message.Append(messageTemplate);
			message.Replace("{SiteName}", siteName);
			message.Replace("{Message}", messageToken);
			subject = subject.Replace("{SiteName}", siteName);

			//try
			//{
			//    Email.SendEmail(
			//        smtpSettings,
			//        fromAddress,
			//        toEmail,
			//        replyEmail,
			//        ccEmail,
			//        bccEmail,
			//        subject,
			//        message.ToString(),
			//        true,
			//        "Normal");
			//}
			//catch (Exception ex)
			//{
			//    log.Error("Error sending email from address was " + fromAddress + " to address was " + toEmail, ex);
			//}

			EmailMessageTask messageTask = new EmailMessageTask(smtpSettings);
			messageTask.SiteGuid = siteGuid;
			messageTask.EmailFrom = fromAddress;
			messageTask.EmailFromAlias = fromAlias;
			messageTask.EmailReplyTo = replyEmail;
			messageTask.EmailTo = toEmail;
			messageTask.EmailCc = ccEmail;
			messageTask.EmailBcc = bccEmail;
			messageTask.UseHtml = true;
			messageTask.Subject = subject;
			messageTask.HtmlBody = message.ToString();
			messageTask.QueueTask();

			WebTaskManager.StartOrResumeTasks();
		}

		#region filter

		public static string BuildFilterUrlLeaveOutPageNumber(string rawUrl, bool fullUrl = false) {
			string pageUrl = SiteUtils.BuildUrlLeaveOutParam(rawUrl, ProductHelper.QueryStringPageNumberParam, false);
			return SiteUtils.BuildUrlLeaveOutParam(pageUrl, "isajax", fullUrl);
		}

		public static string BuildFilterUrlLeaveOutPageNumber(string rawUrl, string paramName, string paramValue) {
			string pageUrl = SiteUtils.BuildUrlLeaveOutParam(rawUrl, ProductHelper.QueryStringPageNumberParam, false);
			pageUrl = SiteUtils.BuildUrlLeaveOutParam(pageUrl, "isajax", false);
			pageUrl = SiteUtils.BuildUrlLeaveOutParam(pageUrl, paramName);

			if (paramValue.Length > 0) {
				if (pageUrl.Contains("?"))
					pageUrl += string.Format("&{0}={1}", paramName, paramValue);
				else
					pageUrl += string.Format("?{0}={1}", paramName, paramValue);
			}

			return pageUrl;
		}

		public static string GetPriceFromQueryString(out decimal? priceMin, out decimal? priceMax) {
			priceMin = null;
			priceMax = null;
			string priceRangeValue = WebUtils.ParseStringFromQueryString(ProductHelper.QueryStringPriceParam, string.Empty);
			if (priceRangeValue.Length > 0) {
				var priceRange = priceRangeValue.Split('-');
				if (priceRange.Length == 2) {
					decimal priceValue = -1;
					decimal.TryParse(priceRange[0], out priceValue);
					if (priceValue > 0)
						priceMin = priceValue;

					priceValue = -1;
					decimal.TryParse(priceRange[1], out priceValue);
					if (priceValue > 0)
						priceMax = priceValue;
				}
			}

			return priceRangeValue;
		}

		public const string QueryStringManufacturerParam = "m";
		public const string QueryStringViewModeParam = "view";
		public const string QueryStringSortModeParam = "sort";
		public const string QueryStringPriceParam = "price";
		public const string QueryStringFilterSingleParam = "f";
		public const string QueryStringFilterMultiParam = "mf";
		public const string QueryStringKeywordParam = "keyword";
		public const string QueryStringPageSizeParam = "pagesize";
		public const string QueryStringPageNumberParam = "pagenumber";

		public static string GetQueryStringFilter(string pageUrl, int filterType, int customFieldId, int customFieldOptionId) {
			string result = pageUrl;
			string paramName = string.Empty;
			string paramValue = customFieldOptionId.ToString();
			if (filterType == (int)CustomFieldFilterType.ByValue)
				paramName = ProductHelper.QueryStringFilterSingleParam + customFieldId.ToString();
			else {
				paramName = ProductHelper.QueryStringFilterMultiParam + customFieldId.ToString();
				paramValue = "/" + paramValue + "/";
			}

			if (pageUrl.Contains("?"))
				result += string.Format("&{0}={1}", paramName, paramValue);
			else
				result += string.Format("?{0}={1}", paramName, paramValue);

			return result;
		}

		#endregion

		#region Compare products

		private const string COMPARE_PRODUCTS_COOKIE = "CompareProducts";
		private const string COMPARE_PRODUCTS_COOKIE_VALUE = "CompareProductIds";

		/// <summary>
		/// Clears a "compare products" list
		/// </summary>
		public static void ClearCompareProducts() {
			HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE);
			if (compareCookie != null) {
				compareCookie.Values.Clear();
				compareCookie.Expires = DateTime.Now.AddYears(-1);
				HttpContext.Current.Response.Cookies.Set(compareCookie);
			}
		}

		/// <summary>
		/// Gets a "compare products" list
		/// </summary>
		/// <returns>"Compare products" list</returns>
		public static List<Product> GetCompareProducts() {
			var siteSettings = CacheHelper.GetCurrentSiteSettings();
			var productIds = GetCompareProductsIds();

			if (productIds.Count > 0) {
				var products = Product.GetPageBySearch(1, productIds.Count, siteSettings.SiteId, null, 1, WorkingCulture.LanguageId, -1, -1,
				                                       null, null, -1, -1, null, string.Join(";", productIds.ToArray()));
				return products;
			}

			return new List<Product>();
		}

		/// <summary>
		/// Gets a "compare products" identifier list
		/// </summary>
		/// <returns>"compare products" identifier list</returns>
		public static List<int> GetCompareProductsIds() {
			var productIds = new List<int>();
			HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE);
			if ((compareCookie == null) || (compareCookie.Values == null))
				return productIds;
			string[] values = compareCookie.Values.GetValues(COMPARE_PRODUCTS_COOKIE_VALUE);
			if (values == null)
				return productIds;
			foreach (string productId in values) {
				int prodId = int.Parse(productId);
				if (!productIds.Contains(prodId))
					productIds.Add(prodId);
			}

			return productIds;
		}

		/// <summary>
		/// Removes a product from a "compare products" list
		/// </summary>
		/// <param name="productId">Product identifer</param>
		public static void RemoveProductFromCompareList(int productId) {
			var oldProductIds = GetCompareProductsIds();
			var newProductIds = new List<int>();
			newProductIds.AddRange(oldProductIds);
			newProductIds.Remove(productId);

			HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE);
			if ((compareCookie == null) || (compareCookie.Values == null))
				return;
			compareCookie.Values.Clear();
			foreach (int newProductId in newProductIds)
				compareCookie.Values.Add(COMPARE_PRODUCTS_COOKIE_VALUE, newProductId.ToString());
			compareCookie.Expires = DateTime.Now.AddDays(10.0);
			HttpContext.Current.Response.Cookies.Set(compareCookie);
		}

		/// <summary>
		/// Adds a product to a "compare products" list
		/// </summary>
		/// <param name="productId">Product identifer</param>
		public static void AddProductToCompareList(int productId) {
			if (!ProductConfiguration.EnableComparing)
				return;

			var oldProductIds = GetCompareProductsIds();
			var newProductIds = new List<int>();
			newProductIds.Add(productId);
			foreach (int oldProductId in oldProductIds)
				if (oldProductId != productId)
					newProductIds.Add(oldProductId);

			HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE);
			if (compareCookie == null)
				compareCookie = new HttpCookie(COMPARE_PRODUCTS_COOKIE);
			compareCookie.Values.Clear();
			int maxProducts = ProductConfiguration.MaximumCompareItems;
			int i = 1;
			foreach (int newProductId in newProductIds) {
				compareCookie.Values.Add(COMPARE_PRODUCTS_COOKIE_VALUE, newProductId.ToString());
				if (i == maxProducts)
					break;
				i++;
			}
			compareCookie.Expires = DateTime.Now.AddDays(1.0);
			HttpContext.Current.Response.Cookies.Set(compareCookie);
		}

		#endregion

		#region recently viewed products

		/// <summary>
		/// Gets a "recently viewed products" list
		/// </summary>
		/// <param name="number">Number of products to load</param>
		/// <returns>"recently viewed products" list</returns>
		public static List<Product> GetRecentlyViewedProducts(int number) {
			var siteSettings = CacheHelper.GetCurrentSiteSettings();
			var products = new List<Product>();
			var productIds = GetRecentlyViewedProductsIds(number);
			if (productIds.Count > 0)
				products = Product.GetPageBySearch(1, productIds.Count, siteSettings.SiteId, null, 1, WorkingCulture.LanguageId, -1, -1,
				                                   null, null, -1, -1, null, string.Join(";", productIds.ToArray()));
			return products;
		}

		/// <summary>
		/// Gets a "recently viewed products" identifier list
		/// </summary>
		/// <returns>"recently viewed products" list</returns>
		public static List<int> GetRecentlyViewedProductsIds() {
			return GetRecentlyViewedProductsIds(int.MaxValue);
		}

		private static string GetRecentlyViewedProductsCookieName() {
			SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
			return "Site-" + siteSettings.SiteId.ToString() + ".RecentlyViewedProducts";
		}

		private static string GetRecentlyViewedProductIdsCookieName() {
			return "ProductId";
		}

		/// <summary>
		/// Gets a "recently viewed products" identifier list
		/// </summary>
		/// <param name="number">Number of products to load</param>
		/// <returns>"recently viewed products" list</returns>
		public static List<int> GetRecentlyViewedProductsIds(int number) {
			var productIds = new List<int>();
			HttpCookie recentlyViewedCookie = HttpContext.Current.Request.Cookies.Get(GetRecentlyViewedProductsCookieName());
			if ((recentlyViewedCookie == null) || (recentlyViewedCookie.Values == null))
				return productIds;
			string[] values = recentlyViewedCookie.Values.GetValues(GetRecentlyViewedProductIdsCookieName());
			if (values == null)
				return productIds;
			foreach (string productId in values) {
				int prodId = int.Parse(productId);
				if (!productIds.Contains(prodId)) {
					productIds.Add(prodId);
					if (productIds.Count >= number)
						break;
				}
			}

			return productIds;
		}

		/// <summary>
		/// Adds a product to a recently viewed products list
		/// </summary>
		/// <param name="productId">Product identifier</param>
		public static void AddProductToRecentlyViewedList(int productId) {
			if (!ProductConfiguration.RecentlyViewedProductsEnabled)
				return;

			var oldProductIds = GetRecentlyViewedProductsIds();
			var newProductIds = new List<int>();
			newProductIds.Add(productId);
			foreach (int oldProductId in oldProductIds)
				if (oldProductId != productId)
					newProductIds.Add(oldProductId);

			string cookieName = GetRecentlyViewedProductsCookieName();
			HttpCookie recentlyViewedCookie = HttpContext.Current.Request.Cookies.Get(cookieName);
			if (recentlyViewedCookie == null)
				recentlyViewedCookie = new HttpCookie(cookieName);
			recentlyViewedCookie.Values.Clear();
			int maxProducts = ProductConfiguration.RecentlyViewedProductCount;
			if (maxProducts <= 0)
				maxProducts = 10;

			string cookieIds = GetRecentlyViewedProductIdsCookieName();
			int i = 1;
			foreach (int newProductId in newProductIds) {
				recentlyViewedCookie.Values.Add(cookieIds, newProductId.ToString());
				if (i == maxProducts)
					break;
				i++;
			}
			recentlyViewedCookie.Expires = DateTime.Now.AddDays(10.0);
			HttpContext.Current.Response.Cookies.Set(recentlyViewedCookie);
		}

		#endregion

		public static bool IsAjaxRequest(HttpRequest request) { //public static bool IsAjaxRequest(this HttpRequest request)
			if (!WebUtils.ParseBoolFromQueryString("isajax", false))
				return false;

			if (request == null)
				throw new ArgumentNullException("request");
			var context = HttpContext.Current;
			var isCallbackRequest = false;// callback requests are ajax requests
			if (context != null && context.CurrentHandler != null && context.CurrentHandler is System.Web.UI.Page)
				isCallbackRequest = ((System.Web.UI.Page)context.CurrentHandler).IsCallback;
			return isCallbackRequest || (request["X-Requested-With"] == "XMLHttpRequest")
			       || (request.Headers["X-Requested-With"] == "XMLHttpRequest");
		}

		#region Price

		public static string FormatPrice(Product product) {
			return FormatPrice(GetPrice(product));
		}

		/// <summary>
		/// Formats the price
		/// </summary>
		/// <param name="price">Price</param>
		/// <returns>Price</returns>
		public static string FormatPrice(decimal price) {
			return FormatPrice(price, false);
		}

		public static string FormatPrice(decimal price, bool showCurrency) {
			string result = price.ToString(ProductConfiguration.CurrencyFormatting);

			if (showCurrency)
				result += (ProductConfiguration.WorkingCurrency.Length > 0 ? " " + ProductConfiguration.WorkingCurrency : string.Empty);

			return result;
		}

		public static decimal GetPrice(Product product) {
			return GetPrice(product.Price, product.SpecialPrice, product.SpecialPriceStartDate, product.SpecialPriceEndDate);
		}

		public static decimal GetPrice(decimal price, decimal specialPrice, object specialPriceStartDate,
		                               object specialPriceEndDate) {
			return GetPrice(price, specialPrice, (DateTime?)specialPriceStartDate, (DateTime?)specialPriceEndDate);
		}

		public static decimal GetPrice(decimal price, decimal specialPrice, DateTime? specialPriceStartDate,
		                               DateTime? specialPriceEndDate) {
			if (specialPrice > 0 && specialPriceStartDate.HasValue && specialPriceEndDate.HasValue) {
				if (specialPriceStartDate.Value <= DateTime.UtcNow && DateTime.UtcNow <= specialPriceEndDate)
					return specialPrice;
			}

			return price;
		}

		#endregion

		#region XmlData

		public static XmlDocument BuildProductDataXml(XmlDocument doc, XmlElement productXml, Product product,
		        TimeZoneInfo timeZone = null, double timeOffset = -1, string editLink = null, List<int> lstCompareProductIds = null) {
			XmlHelper.AddNode(doc, productXml, "Title", product.Title);
			XmlHelper.AddNode(doc, productXml, "SubTitle", product.SubTitle);
			XmlHelper.AddNode(doc, productXml, "Url", ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId));
			XmlHelper.AddNode(doc, productXml, "Target", ProductHelper.GetProductTarget(product.OpenInNewWindow));
			XmlHelper.AddNode(doc, productXml, "ProductId", product.ProductId.ToString());
			XmlHelper.AddNode(doc, productXml, "ZoneId", product.ZoneId.ToString());
			XmlHelper.AddNode(doc, productXml, "ShowOption", product.ShowOption.ToString());

			XmlHelper.AddNode(doc, productXml, "ProductType", product.ProductType.ToString());
			XmlHelper.AddNode(doc, productXml, "Code", product.Code);
			if (product.Price > 0)
				XmlHelper.AddNode(doc, productXml, "Price", ProductHelper.FormatPrice(product.Price, true));
			if (product.OldPrice > 0)
				XmlHelper.AddNode(doc, productXml, "OldPrice", ProductHelper.FormatPrice(product.OldPrice, true));

			string imageFolderPath = ProductHelper.MediaFolderPath(product.SiteId, product.ProductId);
			string thumbnailImageFolderPath = imageFolderPath + "thumbs/";
			if (product.ImageFile.Length > 0)
				XmlHelper.AddNode(doc, productXml, "ImageUrl", ProductHelper.GetMediaFilePath(imageFolderPath, product.ImageFile));
			if (product.ThumbnailFile.Length > 0)
				XmlHelper.AddNode(doc, productXml, "ThumbnailUrl", ProductHelper.GetMediaFilePath(thumbnailImageFolderPath,
				                  product.ThumbnailFile));

			XmlHelper.AddNode(doc, productXml, "BriefContent", product.BriefContent);
			XmlHelper.AddNode(doc, productXml, "FullContent", product.FullContent);
			XmlHelper.AddNode(doc, productXml, "ViewCount", product.ViewCount.ToString());
			XmlHelper.AddNode(doc, productXml, "CommentCount", product.CommentCount.ToString());

			XmlHelper.AddNode(doc, productXml, "FileUrl", product.FileAttachment);

			if (!string.IsNullOrEmpty(editLink))
				XmlHelper.AddNode(doc, productXml, "EditLink", editLink);

			if (timeZone != null && timeOffset > -1) {
				object startDate = product.StartDate;
				XmlHelper.AddNode(doc, productXml, "CreatedDate", FormatDate(startDate, timeZone, timeOffset,
				                  ResourceHelper.GetResourceString("ProductResources", "ProductDateFormat")));
				XmlHelper.AddNode(doc, productXml, "CreatedTime", FormatDate(startDate, timeZone, timeOffset,
				                  ResourceHelper.GetResourceString("ProductResources", "ProductTimeFormat")));
				XmlHelper.AddNode(doc, productXml, "CreatedDD", FormatDate(startDate, timeZone, timeOffset, "dd"));
				XmlHelper.AddNode(doc, productXml, "CreatedYY", FormatDate(startDate, timeZone, timeOffset, "yy"));
				XmlHelper.AddNode(doc, productXml, "CreatedYYYY", FormatDate(startDate, timeZone, timeOffset, "yyyy"));
				XmlHelper.AddNode(doc, productXml, "CreatedMM", FormatDate(startDate, timeZone, timeOffset, "MM"));
				if (System.Globalization.CultureInfo.CurrentCulture.Name.ToLower() == "vi-vn") {
					string monthVI = "Tháng " + FormatDate(startDate, timeZone, timeOffset, "MM");
					XmlHelper.AddNode(doc, productXml, "CreatedMMM", monthVI);
					XmlHelper.AddNode(doc, productXml, "CreatedMMMM", monthVI);
				} else {
					XmlHelper.AddNode(doc, productXml, "CreatedMMM", FormatDate(startDate, timeZone, timeOffset, "MMM"));
					XmlHelper.AddNode(doc, productXml, "CreatedMMMM", FormatDate(startDate, timeZone, timeOffset, "MMMM"));
				}
			}

			if (lstCompareProductIds != null
			    && lstCompareProductIds.Count > 0) {
				if (lstCompareProductIds.Contains(product.ProductId))
					XmlHelper.AddNode(doc, productXml, "AddedCompare", "true");
			}

			return doc;
		}

		public static string FormatDate(object startDate, TimeZoneInfo timeZone, double timeOffset, string format = "") {
			if (startDate == null)
				return string.Empty;

			if (timeZone != null)
				return TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(startDate), timeZone).ToString(format);

			return Convert.ToDateTime(startDate).AddHours(timeOffset).ToString(format);
		}

		#endregion

		public static Product GetProductFromList(List<Product> lstProducts, int productId) {
			foreach (Product pd in lstProducts) {
				if (pd.ProductId == productId)
					return pd;
			}

			return null;
		}

		#region Orders

		public static string GenerateOrderCode(int siteId) {
			var number = 0;
			try {
				DateTime fromdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
				DateTime todate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59);

				var dateFormatLength = ProductConfiguration.OrderCodeDateFormat.Length;
				var lstOrders = Order.GetPage(siteId, -1, -1, -1, -1, fromdate, todate, null, null, null, null, 1, 1);
				if (lstOrders.Count > 0 && lstOrders[0].OrderCode.Length >= (dateFormatLength + ProductConfiguration.OrderCodeMinimumLength))
					number = Convert.ToInt32(lstOrders[0].OrderCode.Remove(0, dateFormatLength));
			} catch (Exception ex) {
				log.Error(ex.Message);
			}

			return DateTime.Now.ToString(ProductConfiguration.OrderCodeDateFormat) + (number + 1).ToString().PadLeft(
			           ProductConfiguration.OrderCodeMinimumLength, '0');
		}

		public static bool SendOrderPlacedNotification(
		    SiteSettings siteSettings,
		    Order order,
		    List<Product> lstProductInOrder,
		    List<OrderItem> lstOrderItems,
		    string emailTemplate,
		    string billingProvinceName,
		    string billingDistrictName,
		    string shippingProvinceName,
		    string shippingDistrictName,
		    string toEmail = "") {
			if (order == null)
				return false;

			EmailTemplate template = EmailTemplate.Get(siteSettings.SiteId, emailTemplate, WorkingCulture.LanguageId);

			if (template == null || template.Guid == Guid.Empty)
				return false;

			string fromAlias = template.FromName;
			if (fromAlias.Length == 0)
				fromAlias = siteSettings.DefaultFromEmailAlias;

			StringBuilder message = new StringBuilder();
			message.Append(template.HtmlBody);

			message.Replace("{SiteName}", siteSettings.SiteName);

			message.Replace("{BillingFirstName}", order.BillingFirstName);
			message.Replace("{BillingLastName}", order.BillingLastName);
			message.Replace("{BillingEmail}", order.BillingEmail);
			message.Replace("{BillingMobile}", order.BillingMobile);
			message.Replace("{BillingPhone}", order.BillingPhone);
			message.Replace("{BillingAddress}", order.BillingAddress);

			message.Replace("{ShippingFirstName}", order.ShippingFirstName);
			message.Replace("{ShippingLastName}", order.ShippingLastName);
			message.Replace("{ShippingMobile}", order.ShippingMobile);
			message.Replace("{ShippingEmail}", order.ShippingEmail);
			message.Replace("{ShippingPhone}", order.ShippingPhone);
			message.Replace("{ShippingAddress}", order.ShippingAddress);

			message.Replace("{InvoiceCompanyName}", order.InvoiceCompanyName);
			message.Replace("{InvoiceCompanyAddress}", order.InvoiceCompanyAddress);
			message.Replace("{InvoiceCompanyTaxCode}", order.InvoiceCompanyTaxCode);

			message.Replace("{OrderStatus}", ProductHelper.GetOrderStatus(order.OrderStatus));

			string shippingMethod = string.Empty;
			if (order.ShippingMethod > 0) {
				ShippingMethod method = new ShippingMethod(order.ShippingMethod);
				if (method != null && method.ShippingMethodId > 0)
					shippingMethod = method.Name;
			}
			string paymentMethod = string.Empty;
			if (order.PaymentMethod > 0) {
				PaymentMethod method = new PaymentMethod(order.PaymentMethod);
				if (method != null && method.PaymentMethodId > 0)
					paymentMethod = method.Name;
			}
			message.Replace("{PaymentMethod}", paymentMethod);
			message.Replace("{ShippingMethod}", shippingMethod);

			var detail = string.Empty;
			if (lstOrderItems.Count > 0) {
				List<ProductProperty> productProperties = new List<ProductProperty>();
				List<CustomField> customFields = new List<CustomField>();
				if (ProductConfiguration.EnableShoppingCartAttributes) {
					var lstProductIds = lstOrderItems.Select(x => x.ProductId).Distinct().ToList();
					customFields = CustomField.GetActiveForCart(order.SiteId, Product.FeatureGuid);
					if (customFields.Count > 0 && lstProductIds.Count > 0)
						productProperties = ProductProperty.GetPropertiesByProducts(lstProductIds);
				}

				XmlDocument doc = new XmlDocument();

				doc.LoadXml("<OrderDetails></OrderDetails>");
				XmlElement root = doc.DocumentElement;

				XmlHelper.AddNode(doc, root, "OrderTotal", ProductHelper.FormatPrice(order.OrderTotal, true));
				XmlHelper.AddNode(doc, root, "OrderSubTotal", ProductHelper.FormatPrice(order.OrderSubtotal, true));
				XmlHelper.AddNode(doc, root, "OrderDiscount", ProductHelper.FormatPrice(order.OrderDiscount, true));
				XmlHelper.AddNode(doc, root, "ShippingFee", ProductHelper.FormatPrice(order.OrderShipping, true));

				foreach (OrderItem orderItems in lstOrderItems) {
					Product product = GetProductFromList(lstProductInOrder, orderItems.ProductId);

					if (product != null) {
						XmlElement orderItemsXml = doc.CreateElement("OrderItems");
						root.AppendChild(orderItemsXml);

						ProductHelper.BuildProductDataXml(doc, orderItemsXml, product);

						// Order detail
						if (orderItemsXml["Price"] != null)
							orderItemsXml["Price"].InnerText = ProductHelper.FormatPrice(orderItems.Price, true);
						else
							XmlHelper.AddNode(doc, orderItemsXml, "Price", ProductHelper.FormatPrice(orderItems.Price, true));

						XmlHelper.AddNode(doc, orderItemsXml, "Quantity", orderItems.Quantity.ToString());
						XmlHelper.AddNode(doc, orderItemsXml, "Discount", ProductHelper.FormatPrice(orderItems.DiscountAmount, true));
						XmlHelper.AddNode(doc, orderItemsXml, "ItemSubTotal", ProductHelper.FormatPrice(orderItems.Quantity * orderItems.Price,
						                  true));
						XmlHelper.AddNode(doc, orderItemsXml, "ItemTotal",
						                  ProductHelper.FormatPrice(orderItems.Quantity * orderItems.Price - orderItems.DiscountAmount, true));

						if (!string.IsNullOrEmpty(orderItems.AttributesXml)) {
							var attributes = ProductAttributeParser.ParseProductAttributeMappings(customFields, orderItems.AttributesXml);
							if (attributes.Count > 0) {
								foreach (var a in attributes) {
									XmlElement attributeXml = doc.CreateElement("Attributes");
									orderItemsXml.AppendChild(attributeXml);

									XmlHelper.AddNode(doc, attributeXml, "Title", a.Name);

									var values = ProductAttributeParser.ParseValues(orderItems.AttributesXml, a.CustomFieldId);
									foreach (ProductProperty property in productProperties) {
										if (property.ProductId == product.ProductId
										    && property.CustomFieldId == a.CustomFieldId
										    && values.Contains(property.CustomFieldOptionId)) {
											XmlElement optionXml = doc.CreateElement("Options");
											attributeXml.AppendChild(optionXml);

											XmlHelper.AddNode(doc, optionXml, "Title", property.OptionName);
										}
									}
								}
							}
						}
					}
				}

				detail += XmlHelper.TransformXML(SiteUtils.GetXsltBasePath("Product", "OrderDetailsTemplate.xslt"), doc);
			}

			message.Replace("{OrderDetails}", detail);
			message.Replace("{OrderUrl}", string.Empty);
			message.Replace("{OrderNumber}", order.OrderId.ToString());
			message.Replace("{OrderCode}", order.OrderCode);
			message.Replace("{OrderNote}", order.OrderNote);
			message.Replace("{CreatedOn}", DateTimeHelper.Format(Convert.ToDateTime(order.CreatedUtc), SiteUtils.GetUserTimeZone(),
			                "dd/MM/yyyy HH:mm", SiteUtils.GetUserTimeOffset()));

			message.Replace("{OrderTotal}", ProductHelper.FormatPrice(order.OrderTotal, true));
			message.Replace("{OrderSubTotal}", ProductHelper.FormatPrice(order.OrderSubtotal, true));
			message.Replace("{OrderDiscount}", ProductHelper.FormatPrice(order.OrderDiscount, true));
			message.Replace("{ShippingFee}", ProductHelper.FormatPrice(order.OrderShipping, true));
            message.Replace("{OrderTax}", ProductHelper.FormatPrice(order.OrderTax, true));

			message.Replace("{BillingProvinceName}", billingProvinceName);
			message.Replace("{BillingDistrictName}", billingDistrictName);
			message.Replace("{ShippingProvinceName}", shippingProvinceName);
			message.Replace("{ShippingDistrictName}", shippingDistrictName);

			SmtpSettings smtpSettings = SiteUtils.GetSmtpSettings();

			string subjectEmail = template.Subject.Replace("{SiteName}", siteSettings.SiteName).Replace("{OrderCode}",
			                      order.OrderCode).Replace("{OrderNumber}", order.OrderId.ToString());

			EmailMessageTask messageTask = new EmailMessageTask(smtpSettings);
			messageTask.EmailFrom = siteSettings.DefaultEmailFromAddress;
			messageTask.EmailTo = toEmail + (template.ToAddresses.Length == 0 ? string.Empty : "," + template.ToAddresses);
			messageTask.EmailCc = (template.CcAddresses.Length == 0 ? string.Empty : "," + template.CcAddresses);
			messageTask.EmailBcc = (template.BccAddresses.Length == 0 ? string.Empty : "," + template.BccAddresses);
			messageTask.EmailReplyTo = template.ReplyToAddress;
			messageTask.EmailFromAlias = template.FromName;
			messageTask.UseHtml = true;
			messageTask.SiteGuid = siteSettings.SiteGuid;
			messageTask.Subject = subjectEmail;
			messageTask.HtmlBody = message.ToString();
			messageTask.QueueTask();

			return true;
		}

		public static string GetOrderStatus(int orderStatus) {
			switch ((OrderStatus)orderStatus) {
			case OrderStatus.New:
				return Resources.ProductResources.OrderStatusNew;
			case OrderStatus.Processing:
				return Resources.ProductResources.OrderStatusProcessing;
			case OrderStatus.Complete:
				return Resources.ProductResources.OrderStatusComplete;
			case OrderStatus.OutOfStock:
				return Resources.ProductResources.OrderStatusOutOfStock;
			case OrderStatus.Cancelled:
				return Resources.ProductResources.OrderStatusCancelled;
			}

			return string.Empty;
		}

		#endregion

		#region Shipping

		public static string GetShippingGeoZoneGuidsByOrderSession(Order order) {
			string geoZoneGuids = string.Empty;
			if (order != null) {
				if (order.ShippingProvinceGuid != Guid.Empty)
					geoZoneGuids += order.ShippingProvinceGuid + ";";
				if (order.ShippingDistrictGuid != Guid.Empty)
					geoZoneGuids += order.ShippingDistrictGuid + ";";

				if (geoZoneGuids.Length == 0) {
					if (order.BillingProvinceGuid != Guid.Empty)
						geoZoneGuids += order.BillingProvinceGuid + ";";
					if (order.BillingDistrictGuid != Guid.Empty)
						geoZoneGuids += order.BillingDistrictGuid + ";";
				}
			}

			if (geoZoneGuids.Length == 0)
				return null;

			return geoZoneGuids;
		}

		public static decimal GetShippingPrice(int shippingMethodId, decimal orderSubTotal, decimal orderWeight, int productTotalQty,
		                                       string geoZoneGuids) {
			ShippingMethod method = new ShippingMethod(shippingMethodId);
			return GetShippingPrice(method, orderSubTotal, orderWeight, productTotalQty, geoZoneGuids);
		}

		public static decimal GetShippingPrice(ShippingMethod method, decimal orderSubTotal, decimal orderWeight,
		                                       int productTotalQty, string geoZoneGuids) {
			if (method != null && method.ShippingMethodId > 0 && !method.IsDeleted && method.IsActive) {
				switch ((ShippingMethodProvider)method.ShippingProvider) {
				case ShippingMethodProvider.Free:
					return decimal.Zero;
				case ShippingMethodProvider.Fixed:
					if (method.FreeShippingOverXEnabled && orderSubTotal >= method.FreeShippingOverXValue && method.FreeShippingOverXValue > 0)
						return decimal.Zero;
					return method.ShippingFee;
				case ShippingMethodProvider.FixedPerItem:
					if (method.FreeShippingOverXEnabled && orderSubTotal >= method.FreeShippingOverXValue && method.FreeShippingOverXValue > 0)
						return decimal.Zero;
					return method.ShippingFee * productTotalQty;
				case ShippingMethodProvider.ByOrderTotal:
					ShippingTableRate tableRate = ShippingTableRate.GetOneMaxValue(method.ShippingMethodId, orderSubTotal, null);
					if (tableRate != null)
						return tableRate.ShippingFee;

					return method.ShippingFee;
				case ShippingMethodProvider.ByWeight:
					if (method.FreeShippingOverXEnabled && orderSubTotal >= method.FreeShippingOverXValue && method.FreeShippingOverXValue > 0)
						return decimal.Zero;
					tableRate = ShippingTableRate.GetOneMaxValue(method.ShippingMethodId, orderWeight, null);
					if (tableRate != null) {
						if (tableRate.AdditionalFee >= 0 && tableRate.AdditionalValue > 0) {
							decimal fromValue = tableRate.FromValue;
							decimal shippingFee = tableRate.ShippingFee;
							while (orderWeight > fromValue) {
								fromValue += tableRate.AdditionalValue;
								shippingFee += tableRate.AdditionalFee;
							}

							return shippingFee;
						}

						return tableRate.ShippingFee;
					}

					return method.ShippingFee;
				case ShippingMethodProvider.ByGeoZoneAndFixed:
					tableRate = ShippingTableRate.GetOneMaxValue(method.ShippingMethodId, -1, geoZoneGuids);
					if (tableRate != null) {
						if (method.FreeShippingOverXEnabled && orderSubTotal >= tableRate.FreeShippingOverXValue
						    && tableRate.FreeShippingOverXValue > 0)
							return decimal.Zero;

						return tableRate.ShippingFee;
					}

					return method.ShippingFee;
				case ShippingMethodProvider.ByGeoZoneAndOrderTotal:
					tableRate = ShippingTableRate.GetOneMaxValue(method.ShippingMethodId, orderSubTotal, geoZoneGuids);
					if (tableRate != null)
						return tableRate.ShippingFee;

					return method.ShippingFee;
				case ShippingMethodProvider.ByGeoZoneAndWeight: // Addional value
					tableRate = ShippingTableRate.GetOneMaxValue(method.ShippingMethodId, orderWeight, geoZoneGuids);
					if (tableRate != null) {
						if (method.FreeShippingOverXEnabled && orderSubTotal >= tableRate.FreeShippingOverXValue
						    && tableRate.FreeShippingOverXValue > 0)
							return decimal.Zero;

						if (tableRate.AdditionalFee >= 0 && tableRate.AdditionalValue > 0) {
							decimal fromValue = tableRate.FromValue;
							decimal shippingFee = tableRate.ShippingFee;
							while (orderWeight > fromValue) {
								fromValue += tableRate.AdditionalValue;
								shippingFee += tableRate.AdditionalFee;
							}

							return shippingFee;
						}

						return tableRate.ShippingFee;
					}

					return method.ShippingFee;
				}
			}

			return decimal.Zero;
		}

		#endregion

        public static decimal GetDiscountByPoint(int point)
        {
            return point * 10;
        }

        public static void ProcessUserPoint(SiteSettings siteSettings, Order order)
        {
            if (order.UserGuid != Guid.Empty)
            {
                SiteUser siteUser = new SiteUser(siteSettings, order.UserGuid);
                if (siteUser != null && siteUser.UserId > 0)
                {
                    //if (order.UserPoint > 0) // Su dung point
                    //{
                    //    if (order.OrderStatus == (int)OrderStatus.Cancelled)
                    //    {
                    //        SiteUserEx.UpdateUserPoints(siteUser.UserGuid, siteUser.TotalPosts + order.UserPoint);

                    //        order.UserPointDiscount = 0;
                    //        order.StateId = order.UserPoint;
                    //        order.Save();
                    //    }
                    //    else
                    //    {
                    //        if (order.StateId > 0)
                    //        {
                    //            SiteUserEx.UpdateUserPoints(siteUser.UserGuid, siteUser.TotalPosts - order.UserPoint);

                    //            order.UserPointDiscount = ProductHelper.GetDiscountByPoint(order.UserPoint);
                    //            order.StateId = -1;
                    //            order.Save();
                    //        }
                    //    }
                    //}
                    //else // Khong su dung point, cong diem khi hoan tat
                    //{
                        if (order.OrderStatus == (int)OrderStatus.Complete)
                        {
                            order.StateId = (int)((order.OrderSubtotal - order.OrderDiscount) / 1000); // Set flag is added point to user
                            order.Save();

                            SiteUserEx.UpdateUserPoints(siteUser.UserGuid, siteUser.TotalPosts + order.StateId);
                        }
                        else
                        {
                            if (order.StateId > 0)
                            {
                                SiteUserEx.UpdateUserPoints(siteUser.UserGuid, siteUser.TotalPosts - order.StateId);

                                order.StateId = -1;
                                order.Save();
                            }
                        }
                    //}
                }
            }
        }
	}
}