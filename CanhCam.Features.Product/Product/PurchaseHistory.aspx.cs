/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-09-18
/// Last Modified:			2014-09-18

using System;
using Resources;
using CanhCam.Business;
using log4net;
using System.Collections.Generic;
using CanhCam.Web.Framework;
using System.Xml;
using System.Web.UI;
using CanhCam.Web.UI;

namespace CanhCam.Web.ProductUI {

	public partial class PurchaseHistoryPage : CmsBasePage {
		private static readonly ILog log = LogManager.GetLogger(typeof(PurchaseHistoryPage));
		int pageNumber = 1;
		int pageSize = 20;
		protected Double timeOffset = 0;
		protected TimeZoneInfo timeZone = null;
		SiteUser siteUser;
		List<CustomFieldOption> lstOptions = new List<CustomFieldOption>();

		protected void Page_Load(object sender, EventArgs e) {
			if (!Request.IsAuthenticated) {
				SiteUtils.RedirectToLoginPage(this);
				return;
			}

			SecurityHelper.DisableBrowserCache();

			LoadSettings();
			PopulateLabels();

			if (siteUser == null || siteUser.UserGuid == Guid.Empty) {
				SiteUtils.RedirectToAccessDeniedPage();
				return;
			}

			if (!Page.IsPostBack)
				PopulateControls();
		}

		private void PopulateLabels() {
			Title = SiteUtils.FormatPageTitle(siteSettings, heading.Text);

			Control c = Master.FindControl("Breadcrumbs");
			if (c != null) {
				BreadcrumbsControl crumbs = (BreadcrumbsControl)c;
				crumbs.ForceShowBreadcrumbs = true;
				crumbs.AddedCrumbs
				    = crumbs.ItemWrapperTop + "<a href='" + SiteRoot + "/Product/PurchaseHistory.aspx"
				      + "' class='selectedcrumb'>" + heading.Text
				      + "</a>" + crumbs.ItemWrapperBottom;
			}
		}

		private void LoadSettings() {
			pageNumber = WebUtils.ParseInt32FromQueryString("pagenumber", pageNumber);
			siteUser = SiteUtils.GetCurrentSiteUser();

			timeOffset = SiteUtils.GetUserTimeOffset();
			timeZone = SiteUtils.GetUserTimeZone();

			AddClassToBody("purchase-history-page");
		}

		void btnSearch_Click(object sender, EventArgs e) {
			PopulateControls();
		}

		private void PopulateControls() {
			DateTime? startDate = null;
			if (txtDays.Text.Length > 0) {
				int days = -1;
				int.TryParse(txtDays.Text, out days);

				if (days > 0) {
					DateTime localTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-days);

					if (timeZone != null)
						startDate = localTime.ToUtc(timeZone);
					else
						startDate = localTime.AddHours(-timeOffset);
				}
			}

			List<OrderItem> lstOrderItems = OrderItem.GetPageBySearch(siteSettings.SiteId, -1, -1, -1, -1, startDate, null, null, null,
			                                siteUser.UserGuid, null, pageNumber, pageSize);

			if (lstOrderItems.Count > 0) {
				string productGuids = string.Empty;
				string attribute = string.Empty;
				foreach (OrderItem orderItem in lstOrderItems) {
					productGuids += orderItem.ProductGuid.ToString() + ";";

					string tmp = orderItem.AttributesXml;
					if (tmp.Length > 0)
						tmp += ";";

					//if (orderItem.AttributeDescription.Length > 0)
					//    tmp += orderItem.AttributeDescription + ";";

					attribute += tmp;
				}

				if (attribute.Length > 0)
					lstOptions = CustomFieldOption.GetByOptionIds(siteSettings.SiteId, attribute);

				List<Product> lstProducts = Product.GetByGuids(siteSettings.SiteId, productGuids, -1, WorkingCulture.LanguageId);

				XmlDocument doc = new XmlDocument();

				doc.LoadXml("<ProductList></ProductList>");
				XmlElement root = doc.DocumentElement;

				lstOrderItems.ForEach(orderItem => {
					Product product = ProductHelper.GetProductFromList(lstProducts, orderItem.ProductId);

					if (product != null) {
						XmlElement productXml = doc.CreateElement("Product");
						root.AppendChild(productXml);

						ProductHelper.BuildProductDataXml(doc, productXml, product, null);

						// Order detail
						XmlHelper.AddNode(doc, productXml, "OrderCode", orderItem.Order.OrderCode);
						XmlHelper.AddNode(doc, productXml, "OrderDate", FormatDate(orderItem.Order.CreatedUtc, "dd/MM/yyyy"));
						XmlHelper.AddNode(doc, productXml, "OrderStatus", ProductHelper.GetOrderStatus(orderItem.Order.OrderStatus));
						XmlHelper.AddNode(doc, productXml, "OrderTotal",
						                  ProductHelper.FormatPrice(orderItem.Quantity * orderItem.Price - orderItem.DiscountAmount, true));
					}
				});

				XmlHelper.XMLTransform(xmlTransformer, SiteUtils.GetXsltBasePath("Product", "PurchaseHistory.xslt"), doc);
			}
		}

		private string FormatDate(object startDate, string format = "") {
			if (startDate == null)
				return string.Empty;

			if (timeZone != null)
				return TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(startDate), timeZone).ToString(format);

			return Convert.ToDateTime(startDate).AddHours(timeOffset).ToString(format);
		}

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			this.Load += new EventHandler(this.Page_Load);

			this.btnSearch.Click += new EventHandler(btnSearch_Click);
		}

	}
}
