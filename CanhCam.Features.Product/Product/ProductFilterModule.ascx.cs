/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-07-26
/// Last Modified:			2014-11-21

using System;
using CanhCam.Web.Framework;
using System.Xml;
using CanhCam.Business;
using System.Collections.Generic;
using System.Web;

namespace CanhCam.Web.ProductUI {
	/// <summary>
	/// Query string structure: ?m=&view=&sort=&price=from-to=&fxxx=&mfxxx=&keyword=&pagesize=&pagenumber
	/// </summary>
	public partial class ProductFilterModule : SiteModuleControl {
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			this.Load += new EventHandler(Page_Load);
			this.EnableViewState = false;
		}

		protected virtual void Page_Load(object sender, EventArgs e) {
			LoadSettings();
			PopulateLabels();
			PopulateControls();
		}

		private void PopulateControls() {
			XmlDocument doc = new XmlDocument();

			doc.LoadXml("<ProductFilter></ProductFilter>");
			XmlElement root = doc.DocumentElement;

			string rawUrl = Request.RawUrl;

			XmlHelper.AddNode(doc, root, "ModuleTitle", this.Title);
			XmlHelper.AddNode(doc, root, "ZoneTitle", CurrentZone.Name);
			XmlHelper.AddNode(doc, root, "ClearAllUrl", WebUtils.GetUrlWithoutQueryString(rawUrl));

			if (ModuleConfiguration.ResourceFileDef.Length > 0 && ModuleConfiguration.ResourceKeyDef.Length > 0) {
				List<string> lstResourceKeys = ModuleConfiguration.ResourceKeyDef.SplitOnCharAndTrim(';');

				foreach (string item in lstResourceKeys)
					XmlHelper.AddNode(doc, root, item, ResourceHelper.GetResourceString(ModuleConfiguration.ResourceFileDef, item));
			}

			string rawUrlLeaveOutPageNumber = SiteUtils.BuildUrlLeaveOutParam(rawUrl, ProductHelper.QueryStringPageNumberParam, false);
			rawUrlLeaveOutPageNumber = SiteUtils.BuildUrlLeaveOutParam(rawUrlLeaveOutPageNumber, "isajax", false);

			List<CustomField> lstCustomFields = CustomFieldHelper.GetCustomFieldsFromContext(siteSettings.SiteId, Product.FeatureGuid,
			                                    CurrentZone.ZoneGuid, WorkingCulture.LanguageId);
			foreach (CustomField field in lstCustomFields) {
				if (
				    (field.DataType == (int)CustomFieldDataType.CheckBox
				     || field.DataType == (int)CustomFieldDataType.SelectBox)
				    && (field.FilterType == (int)CustomFieldFilterType.ByValue
				        || field.FilterType == (int)CustomFieldFilterType.ByMultipleValues)
				) {
					XmlElement productXml = doc.CreateElement("Group");
					root.AppendChild(productXml);

					XmlHelper.AddNode(doc, productXml, "GroupId", field.CustomFieldId.ToString());
					XmlHelper.AddNode(doc, productXml, "Title", field.Name);
					XmlHelper.AddNode(doc, productXml, "FilterType", field.FilterType.ToString());

					string paramName = string.Empty;
					bool isCheckBox = true;
					if (field.FilterType == (int)CustomFieldFilterType.ByValue) {
						paramName = ProductHelper.QueryStringFilterSingleParam + field.CustomFieldId.ToString();

						XmlHelper.AddNode(doc, productXml, "ClearUrl", SiteUtils.BuildUrlLeaveOutParam(rawUrlLeaveOutPageNumber, paramName));

						isCheckBox = false;
					} else {
						paramName = ProductHelper.QueryStringFilterMultiParam + field.CustomFieldId.ToString();

						XmlHelper.AddNode(doc, productXml, "ClearUrl", SiteUtils.BuildUrlLeaveOutParam(rawUrlLeaveOutPageNumber, paramName));

						isCheckBox = true;
					}

					XmlHelper.AddNode(doc, productXml, "IsCheckBox", isCheckBox.ToString().ToLower());

					List<CustomFieldOption> lstOptions = CustomFieldOption.GetByCustomField(field.CustomFieldId);
					foreach (CustomFieldOption option in lstOptions) {
						XmlElement optionXml = doc.CreateElement("Option");
						productXml.AppendChild(optionXml);

						XmlHelper.AddNode(doc, optionXml, "GroupId", option.CustomFieldId.ToString());
						XmlHelper.AddNode(doc, optionXml, "OptionId", option.CustomFieldOptionId.ToString());
						XmlHelper.AddNode(doc, optionXml, "Title", option.Name);
						XmlHelper.AddNode(doc, optionXml, "ProductCount", "0");
						XmlHelper.AddNode(doc, optionXml, "FilterType", field.FilterType.ToString());
						XmlHelper.AddNode(doc, optionXml, "IsCheckBox", isCheckBox.ToString().ToLower());
						XmlHelper.AddNode(doc, optionXml, "IsActive", IsActive(paramName, option.CustomFieldOptionId,
						                  field.FilterType).ToString().ToLower());

						//XmlHelper.AddNode(doc, optionXml, "QueryString", "");
						string queryString = string.Empty;
						string pageUrl = BuildFilterUrl(rawUrlLeaveOutPageNumber, paramName, option.CustomFieldOptionId, field.FilterType,
						                                out queryString);
						//string pageUrl = urlWithoutQueryString + BuildSpecFilterQueryString(queryString, paramName, option.CustomFieldOptionId, field.FilterType);
						XmlHelper.AddNode(doc, optionXml, "QueryString", queryString);
						XmlHelper.AddNode(doc, optionXml, "Url", pageUrl);

						XmlHelper.AddNode(doc, optionXml, "ClearUrl", SiteUtils.BuildUrlLeaveOutParam(rawUrlLeaveOutPageNumber, paramName));
					}
				}
			}

			//XmlElement priceXml = doc.CreateElement("Price");
			//root.AppendChild(priceXml);
			//XmlHelper.AddNode(doc, priceXml, "Title", string.Empty);
			//XmlHelper.AddNode(doc, priceXml, "Url", ProductHelper.BuildFilterUrlLeaveOutPageNumber(rawUrlLeaveOutPageNumber, ProductHelper.QueryStringPriceParam, string.Empty));
			XmlHelper.AddNode(doc, root, "UrlWithPrice", ProductHelper.BuildFilterUrlLeaveOutPageNumber(rawUrlLeaveOutPageNumber, true));
			XmlHelper.AddNode(doc, root, "UrlWithoutPrice",
			                  SiteUtils.BuildUrlLeaveOutParam(ProductHelper.BuildFilterUrlLeaveOutPageNumber(rawUrlLeaveOutPageNumber),
			                          ProductHelper.QueryStringPriceParam));

			decimal? priceMin = null;
			decimal? priceMax = null;
			ProductHelper.GetPriceFromQueryString(out priceMin, out priceMax);
			if (priceMin.HasValue)
				XmlHelper.AddNode(doc, root, "PriceMin", priceMin.Value.ToString());
			if (priceMax.HasValue)
				XmlHelper.AddNode(doc, root, "PriceMax", priceMax.Value.ToString());

			if (ProductHelper.IsAjaxRequest(Request)) {
				Response.Write(XmlHelper.TransformXML(SiteUtils.GetXsltBasePath("product", ModuleConfiguration.XsltFileName), doc));

				if (HttpContext.Current.Items["IsAjaxResponse"] != null)
					Response.End();

				HttpContext.Current.Items["IsAjaxResponse"] = true;

				return;
			}

			XmlHelper.XMLTransform(xmlTransformer, SiteUtils.GetXsltBasePath("product", ModuleConfiguration.XsltFileName), doc);
		}

		//private string BuildSpecFilterQueryString(string queryString, string paramName, int optionId, int filterType)
		//{
		//    string paramValue = string.Empty;
		//    if (filterType == (int)CustomFieldFilterType.ByValue)
		//    {
		//        paramValue = optionId.ToString();
		//    }
		//    else
		//    {
		//        paramValue = WebUtils.ParseStringFromQueryString(paramName, string.Empty);

		//        if (paramValue.Contains("/" + optionId.ToString() + "/"))
		//            paramValue = paramValue.Replace(string.Format("/{0}/", optionId.ToString()), "/");
		//        else
		//        {
		//            if (!paramValue.EndsWith("/"))
		//                paramValue += "/";
		//            paramValue = paramValue + string.Format("{0}/", optionId.ToString());
		//        }
		//    }

		//    string result = WebUtils.BuildQueryString(queryString, paramName);
		//    if (result.StartsWith("&"))
		//        result = result.Remove(0, 1);
		//    if (result.Length > 0 && !result.StartsWith("?"))
		//        result = "?" + result;

		//    if (paramValue.Length > 0 && paramValue != "/")
		//    {
		//        if (result.Contains("?"))
		//            result += string.Format("&{0}={1}", paramName, paramValue);
		//        else
		//            result += string.Format("?{0}={1}", paramName, paramValue);
		//    }

		//    return result;
		//}

		private string BuildFilterUrl(string rawUrl, string paramName, int optionId, int filterType, out string queryString) {
			queryString = string.Empty;
			string paramValue = string.Empty;
			if (filterType == (int)CustomFieldFilterType.ByValue)
				paramValue = optionId.ToString();
			else {
				paramValue = WebUtils.ParseStringFromQueryString(paramName, string.Empty);

				if (paramValue.Contains("/" + optionId.ToString() + "/"))
					paramValue = paramValue.Replace(string.Format("/{0}/", optionId.ToString()), "/");
				else {
					if (!paramValue.EndsWith("/"))
						paramValue += "/";
					paramValue = paramValue + string.Format("{0}/", optionId.ToString());
				}
			}

			string pageUrl = SiteUtils.BuildUrlLeaveOutParam(rawUrl, paramName);
			if (paramName.Length > 0 && paramValue.Length > 0 && paramValue != "/") {
				if (pageUrl.Contains("?"))
					pageUrl += string.Format("&{0}={1}", paramName, paramValue);
				else
					pageUrl += string.Format("?{0}={1}", paramName, paramValue);

				queryString = string.Format("{0}={1}", paramName, paramValue);
			}

			return pageUrl;
		}

		private bool IsActive(string paramName, int optionId, int filterType) {
			if (filterType == (int)CustomFieldFilterType.ByValue) {
				int optionValue = WebUtils.ParseInt32FromQueryString(paramName, -1);
				if (optionValue == optionId)
					return true;
			}

			string optionValues = WebUtils.ParseStringFromQueryString(paramName, string.Empty);
			if (optionValues.Contains("/" + optionId.ToString() + "/"))
				return true;

			return false;
		}

		protected virtual void PopulateLabels() {

		}

		protected virtual void LoadSettings() {
			if (this.ModuleConfiguration != null) {
				this.Title = ModuleConfiguration.ModuleTitle;
				this.Description = ModuleConfiguration.FeatureName;
			}
		}

	}
}