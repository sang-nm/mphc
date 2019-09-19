/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-23
/// Last Modified:			2014-06-23

using System;
using System.Web.UI;
using log4net;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.Web.Framework;
using Resources;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CanhCam.Web.ProductUI
{
    public partial class ProductViewControl : UserControl
    {
        #region Properties

        private static readonly ILog log = LogManager.GetLogger(typeof(ProductViewControl));

        private ProductConfiguration config = new ProductConfiguration();
        private SiteUser currentUser = null;
        private string virtualRoot;
        private Product product = null;
        private Module md;

        private int zoneId = -1;
        protected int productId = -1;
        private int languageId = -1;
        private int pageNumber = 1;
        private int totalPages = 1;

        private bool parametersAreInvalid = false;
        private Double timeOffset = 0;
        private TimeZoneInfo timeZone = null;

        private bool userCanEditAsDraft = false;

        private string siteRoot = string.Empty;
        private CmsBasePage basePage;
        private bool userCanUpdate = false;

        public Module module
        {
            get { return md; }
            set { md = value; }
        }

        public ProductConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);

            basePage = Page as CmsBasePage;
            siteRoot = basePage.SiteRoot;
        }

        #endregion

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            LoadParams();

            if (parametersAreInvalid)
            {
                pnlInnerWrap.Visible = false;
                return;
            }

            LoadSettings();

            //SetupRssLink();
            PopulateLabels();

            if (!IsPostBack && productId > 0)
            { // && moduleId > 0)
                PopulateControls();
                basePage.LastPageVisited = Request.RawUrl;

                ProductHelper.AddProductToRecentlyViewedList(productId);
            }
        }

        protected void PopulateControls()
        {
            if (parametersAreInvalid)
            {
                pnlInnerWrap.Visible = false;
                return;
            }

            if (product.IsDeleted)
            {
                if (WebConfigSettings.Custom404Page.Length > 0)
                    Server.Transfer(WebConfigSettings.Custom404Page);
                else
                    Server.Transfer("~/PageNotFound.aspx");

                return;
            }

            if (product.IsPublished && product.EndDate < DateTime.UtcNow)
            {
                expired.Visible = true;
                //http://support.google.com/webmasters/bin/answer.py?hl=en&answer=40132
                // 410 means the resource is gone but once existed
                // google treats it as more permanent than a 404
                // and it should result in de-indexing the content
                Response.StatusCode = 410;
                Response.StatusDescription = "Content Expired";
                if (
                    !ProductPermission.CanUpdate
                    || !basePage.UserCanAuthorizeZone(product.ZoneId)
                )
                {
                    pnlInnerWrap.Visible = false;
                    return;
                }
            }

            // if not published only the editor can see it
            if ((!product.IsPublished) || (product.StartDate > DateTime.UtcNow))
            {
                bool stopRedirect = false;
                if (
                    (currentUser != null && currentUser.UserGuid == product.UserGuid)
                    || ((ProductPermission.CanViewList || ProductPermission.CanUpdate) && basePage.UserCanAuthorizeZone(product.ZoneId))
                )
                    stopRedirect = true;

                if (!stopRedirect)
                {
                    pnlInnerWrap.Visible = false;
                    WebUtils.SetupRedirect(this, SiteUtils.GetCurrentZoneUrl());
                    return;
                }
            }

            SetupMetaTags();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<ProductDetail></ProductDetail>");
            XmlElement root = doc.DocumentElement;

            XmlHelper.AddNode(doc, root, "ZoneTitle", basePage.CurrentZone.Name);
            XmlHelper.AddNode(doc, root, "Title", product.Title);
            XmlHelper.AddNode(doc, root, "SubTitle", product.SubTitle);
            XmlHelper.AddNode(doc, root, "ZoneUrl", SiteUtils.GetCurrentZoneUrl());

            if (module.ResourceFileDef.Length > 0 && module.ResourceKeyDef.Length > 0)
            {
                List<string> lstResourceKeys = module.ResourceKeyDef.SplitOnCharAndTrim(';');

                foreach (string item in lstResourceKeys)
                    XmlHelper.AddNode(doc, root, item, ResourceHelper.GetResourceString(module.ResourceFileDef, item));
            }

            XmlHelper.AddNode(doc, root, "EditLink", ProductHelper.BuildEditLink(product, basePage, userCanUpdate, currentUser));

            XmlHelper.AddNode(doc, root, "ProductId", product.ProductId.ToString());
            XmlHelper.AddNode(doc, root, "ShowOption", product.ShowOption.ToString());
            XmlHelper.AddNode(doc, root, "ProductType", product.ProductType.ToString());

            XmlHelper.AddNode(doc, root, "Code", product.Code);
            if (product.Price > 0)
                XmlHelper.AddNode(doc, root, "Price", ProductHelper.FormatPrice(product.Price, true));
            if (product.OldPrice > 0)
                XmlHelper.AddNode(doc, root, "OldPrice", ProductHelper.FormatPrice(product.OldPrice, true));

            XmlHelper.AddNode(doc, root, "CreatedDate", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset,
                              ProductResources.ProductDateFormat));
            XmlHelper.AddNode(doc, root, "CreatedTime", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset,
                              ProductResources.ProductTimeFormat));
            XmlHelper.AddNode(doc, root, "CreatedDD", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "dd"));
            XmlHelper.AddNode(doc, root, "CreatedYY", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "yy"));
            XmlHelper.AddNode(doc, root, "CreatedYYYY", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "yyyy"));
            XmlHelper.AddNode(doc, root, "CreatedMM", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "MM"));
            if (WorkingCulture.DefaultName.ToLower() == "vi-vn")
            {
                string monthVI = "Tháng " + ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "MM");
                XmlHelper.AddNode(doc, root, "CreatedMMM", monthVI);
                XmlHelper.AddNode(doc, root, "CreatedMMMM", monthVI);
            }
            else
            {
                XmlHelper.AddNode(doc, root, "CreatedMMM", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "MMM"));
                XmlHelper.AddNode(doc, root, "CreatedMMMM", ProductHelper.FormatDate(product.StartDate, timeZone, timeOffset, "MMMM"));
            }

            XmlHelper.AddNode(doc, root, "Code", product.Code);
            XmlHelper.AddNode(doc, root, "BriefContent", product.BriefContent);
            XmlHelper.AddNode(doc, root, "FullContent", product.FullContent);
            XmlHelper.AddNode(doc, root, "ViewCount", product.ViewCount.ToString());
            XmlHelper.AddNode(doc, root, "FileUrl", product.FileAttachment);

            if (
                displaySettings.ShowNextPreviousLink
                && !config.LoadFirstProduct
            )
                BuildNextPreviousXml(doc, root);

            string pageUrl = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId);

            XmlHelper.AddNode(doc, root, "FacebookLike", RenderFacebookLike());
            XmlHelper.AddNode(doc, root, "PlusOne", RenderPlusOne());
            XmlHelper.AddNode(doc, root, "TweetThis", RenderTweetThis());
            XmlHelper.AddNode(doc, root, "Print", RenderPrinter());
            //XmlHelper.AddNode(doc, root, "Email", RenderEmailSubject());
            XmlHelper.AddNode(doc, root, "FullUrl", pageUrl);

            BuildProductRelatedXml(doc, root, languageId);

            if (ProductConfiguration.EnableComparing)
                XmlHelper.AddNode(doc, root, "CompareListUrl", siteRoot + "/Product/Compare.aspx");

            if (displaySettings.ShowAttribute)
                BuildProductAttributesXml(doc, root, languageId);

            BuildProductPropertiesXml(doc, root, languageId);
            BuildProductMediaXml(doc, root);
            BuildProductOtherXml(doc, root, zoneId, pageNumber, config.OtherProductsPerPage, out totalPages);

            if (config.LoadFirstProduct)
                pageUrl = SiteUtils.GetCurrentZoneUrl();

            if (pageUrl.Contains("?"))
                pageUrl += "&amp;pagenumber={0}";
            else
                pageUrl += "?pagenumber={0}";

            pgr.PageURLFormat = pageUrl;
            pgr.ShowFirstLast = true;
            pgr.PageSize = config.OtherProductsPerPage;
            pgr.PageCount = totalPages;
            pgr.CurrentIndex = pageNumber;
            divPager.Visible = (totalPages > 1);

            XmlHelper.XMLTransform(xmlTransformer, SiteUtils.GetXsltBasePath("product", config.XsltFileNameDetailPage), doc);

            if (Page.Header == null) return;

            string canonicalUrl = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId);
            if (SiteUtils.IsSecureRequest() && (!basePage.CurrentPage.RequireSsl) && (!basePage.SiteInfo.UseSslOnAllPages))
            {
                if (WebConfigSettings.ForceHttpForCanonicalUrlsThatDontRequireSsl)
                    canonicalUrl = canonicalUrl.Replace("https:", "http:");
            }

            //LoadWorkflow();

            //Literal link = new Literal();
            //link.ID = "newsurl";
            //link.Text = "\n<link rel='canonical' href='" + canonicalUrl + "' />";

            //Page.Header.Controls.Add(link);

            Product.IncrementViewCount(product.ProductId);
        }

        private void SetupMetaTags()
        {
            string title = product.Title;
            if (product.MetaTitle.Length > 0)
            {
                basePage.Title = product.MetaTitle;
                title = product.MetaTitle;
            }
            else
                basePage.Title = SiteUtils.FormatPageTitle(basePage.SiteInfo, product.Title);

            if (product.MetaKeywords.Length > 0)
                basePage.MetaKeywordCsv = product.MetaKeywords;

            if (product.MetaDescription.Length > 0)
                basePage.MetaDescription = product.MetaDescription;
            else if (product.BriefContent.Length > 0)
                basePage.MetaDescription = UIHelper.CreateExcerpt(product.BriefContent, 156);
            else if (product.FullContent.Length > 0)
                basePage.MetaDescription = UIHelper.CreateExcerpt(product.FullContent, 156);

            if (title.Length > 0)
                basePage.AdditionalMetaMarkup += "\n<meta property=\"og:title\" content=\"" + title + "\" />";
            if (basePage.MetaDescription.Length > 0)
                basePage.AdditionalMetaMarkup += "\n<meta property=\"og:description\" content=\"" + basePage.MetaDescription + "\" />";

            basePage.AdditionalMetaMarkup += "\n<meta property=\"og:url\" content=\"" + ProductHelper.FormatProductUrl(product.Url,
                                             product.ProductId, product.ZoneId) + "\" />";
            basePage.AdditionalMetaMarkup += "\n<meta property=\"og:site_name\" content=\"" + basePage.SiteInfo.SiteName + "\" />";
            basePage.AdditionalMetaMarkup += "\n<meta property=\"og:type\" content=\"product\" />";

            if (title.Length > 0)
                basePage.AdditionalMetaMarkup += "\n<meta itemprop=\"name\" content=\"" + title + "\" />";
            if (basePage.MetaDescription.Length > 0)
                basePage.AdditionalMetaMarkup += "\n<meta itemprop=\"description\" content=\"" + basePage.MetaDescription + "\" />";

            ZoneSettings currentZone = CacheHelper.GetCurrentZone();
            if (basePage.SiteInfo.MetaAdditional.Length > 0)
                basePage.AdditionalMetaMarkup = basePage.SiteInfo.MetaAdditional;
            if (currentZone.AdditionalMetaTags.Length > 0)
                basePage.AdditionalMetaMarkup += currentZone.AdditionalMetaTags;
            if (product.AdditionalMetaTags.Length > 0)
                basePage.AdditionalMetaMarkup += product.AdditionalMetaTags;
        }

        #region GenXML

        private void BuildNextPreviousXml(
            XmlDocument doc,
            XmlElement root)
        {
            product.LoadNextPrevious(languageId);

            if ((product.PreviousProductId > -1) || (product.PreviousProductUrl.Length > 0))
            {
                XmlHelper.AddNode(doc, root, "PreviousLink", "<a href='" + ProductHelper.FormatProductUrl(product.PreviousProductUrl,
                                  product.PreviousProductId, product.PreviousZoneId) + "' title='" + product.PreviousProductTitle + "'>" +
                                  ProductResources.ProductPreviousLink + "</a>");
                XmlHelper.AddNode(doc, root, "PreviousUrl", ProductHelper.FormatProductUrl(product.PreviousProductUrl,
                                  product.PreviousProductId, product.PreviousZoneId));
                XmlHelper.AddNode(doc, root, "IsFirstProduct", product.IsFirstProduct.ToString().ToLower());
                XmlHelper.AddNode(doc, root, "IsLastProduct", product.IsLastProduct.ToString().ToLower());
            }
            if ((product.NextProductId > -1) || (product.NextProductUrl.Length > 0))
            {
                XmlHelper.AddNode(doc, root, "NextLink", "<a href='" + ProductHelper.FormatProductUrl(product.NextProductUrl,
                                  product.NextProductId, product.NextZoneId) + "' title='" + product.NextProductTitle + "'>" + ProductResources.ProductNextLink
                                  + "</a>");
                XmlHelper.AddNode(doc, root, "NextUrl", ProductHelper.FormatProductUrl(product.NextProductUrl, product.NextProductId,
                                  product.NextZoneId));
                XmlHelper.AddNode(doc, root, "IsFirstProduct", product.IsFirstProduct.ToString().ToLower());
                XmlHelper.AddNode(doc, root, "IsLastProduct", product.IsLastProduct.ToString().ToLower());
            }
        }

        private void BuildProductMediaXml(
            XmlDocument doc,
            XmlElement root)
        {

            string imageFolderPath = ProductHelper.MediaFolderPath(CacheHelper.GetCurrentSiteSettings().SiteId, product.ProductId);
            string thumbnailImageFolderPath = imageFolderPath + "thumbs/";
            string siteRoot = WebUtils.GetSiteRoot();

            Regex youtubeVideoRegex = new Regex("youtu(?:\\.be|be\\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
            List<ContentMedia> listMedia = ContentMedia.GetByContentDesc(product.ProductGuid);

            List<int> mediaTypes = new List<int>();
            List<CustomFieldOption> listOptions = new List<CustomFieldOption>();
            foreach (ContentMedia media in listMedia)
            {
                if (media.MediaType > 0 && !mediaTypes.Contains(media.MediaType))
                    mediaTypes.Add(media.MediaType);
            }
            if (mediaTypes.Count > 0)
                listOptions = CustomFieldOption.GetByOptionIds(product.SiteId, string.Join(";", mediaTypes.ToArray()));

            if (listOptions.Count > 0)
            {
                foreach (CustomFieldOption option in listOptions)
                {
                    XmlElement element = doc.CreateElement("ProductColors");
                    root.AppendChild(element);
                    XmlHelper.AddNode(doc, element, "Title", option.Name);
                    XmlHelper.AddNode(doc, element, "Color", option.OptionColor);
                    XmlHelper.AddNode(doc, element, "ColorId", option.CustomFieldOptionId.ToString());

                    foreach (ContentMedia media in listMedia)
                    {
                        if (
                            (option.CustomFieldOptionId == media.MediaType)
                            && (media.LanguageId == -1 || media.LanguageId == languageId)
                            && (media.MediaType != (int)ProductMediaType.Video)
                        )
                            BuildProductImagesXml(doc, element, media, imageFolderPath, thumbnailImageFolderPath);
                    }
                }
            }

            foreach (ContentMedia media in listMedia)
            {
                if (media.LanguageId == -1 || media.LanguageId == languageId)
                {
                    if (media.MediaType != (int)ProductMediaType.Video)
                    {
                        BuildProductImagesXml(doc, root, media, imageFolderPath, thumbnailImageFolderPath);

                        if (media.MediaType != (int)ProductMediaType.Image)
                        {
                            string relativePath = siteRoot + ProductHelper.GetMediaFilePath(imageFolderPath, media.MediaFile);
                            basePage.AdditionalMetaMarkup += "\n<meta property=\"og:image\" content=\"" + relativePath + "\" />";
                            basePage.AdditionalMetaMarkup += "\n<meta itemprop=\"image\" content=\"" + relativePath + "\" />";
                        }
                    }
                    else
                    {
                        XmlElement element = doc.CreateElement("ProductVideos");
                        root.AppendChild(element);

                        XmlHelper.AddNode(doc, element, "Title", media.Title);
                        XmlHelper.AddNode(doc, element, "DisplayOrder", media.DisplayOrder.ToString());
                        XmlHelper.AddNode(doc, element, "Type", media.MediaType.ToString());
                        XmlHelper.AddNode(doc, element, "VideoUrl", ProductHelper.GetMediaFilePath(imageFolderPath, media.MediaFile));

                        string thumbnailPath = ProductHelper.GetMediaFilePath(thumbnailImageFolderPath, media.ThumbnailFile);
                        if (media.ThumbnailFile.Length == 0 && media.MediaFile.ContainsCaseInsensitive("youtu"))
                        {
                            Match youtubeMatch = youtubeVideoRegex.Match(media.MediaFile);
                            string videoId = string.Empty;
                            if (youtubeMatch.Success)
                                videoId = youtubeMatch.Groups[1].Value;

                            thumbnailPath = "http://img.youtube.com/vi/" + videoId + "/0.jpg";
                        }

                        XmlHelper.AddNode(doc, element, "ThumbnailUrl", thumbnailPath);
                    }

                    if (displaySettings.ShowVideo)
                    {
                        XmlElement element = doc.CreateElement("ProductMedia");
                        root.AppendChild(element);

                        XmlHelper.AddNode(doc, element, "Title", media.Title);
                        XmlHelper.AddNode(doc, element, "DisplayOrder", media.DisplayOrder.ToString());
                        XmlHelper.AddNode(doc, element, "Type", media.MediaType.ToString());
                        XmlHelper.AddNode(doc, element, "MediaUrl", ProductHelper.GetMediaFilePath(imageFolderPath, media.MediaFile));
                        XmlHelper.AddNode(doc, element, "ThumbnailUrl", ProductHelper.GetMediaFilePath(thumbnailImageFolderPath, media.ThumbnailFile));
                    }
                }
            }
        }

        public void BuildProductImagesXml(
            XmlDocument doc,
            XmlElement root,
            ContentMedia media,
            string imageFolderPath,
            string thumbnailImageFolderPath)
        {
            string elementName = "ProductImages";
            if (media.MediaType == (int)ProductMediaType.Image1)
                elementName = "ProductImages1";
            else if (media.MediaType == (int)ProductMediaType.Image2)
                elementName = "ProductImages2";
            XmlElement element = doc.CreateElement(elementName);
            root.AppendChild(element);

            XmlHelper.AddNode(doc, element, "Title", media.Title);
            XmlHelper.AddNode(doc, element, "Type", media.MediaType.ToString());
            XmlHelper.AddNode(doc, element, "ImageUrl", ProductHelper.GetMediaFilePath(imageFolderPath, media.MediaFile));
            XmlHelper.AddNode(doc, element, "ThumbnailUrl", ProductHelper.GetMediaFilePath(thumbnailImageFolderPath,
                              media.ThumbnailFile));
        }

        public void BuildProductOtherXml(
            XmlDocument doc,
            XmlElement root,
            int zoneId,
            int pageNumber,
            int pageSize,
            out int totalPages)
        {
            XmlHelper.AddNode(doc, root, "ProductOtherText", ProductResources.OtherProductLabel);

            int siteId = CacheHelper.GetCurrentSiteSettings().SiteId;

            List<Product> lstProducts = new List<Product>();
            if (pageSize < 0)
            {
                pageSize = -pageSize;
                totalPages = 1;
                int totalRows = Product.GetCount(basePage.SiteId, zoneId, languageId, -1);

                if (pageSize > 0) totalPages = totalRows / pageSize;

                if (totalRows <= pageSize)
                    totalPages = 1;
                else if (pageSize > 0)
                {
                    int remainder;
                    Math.DivRem(totalRows, pageSize, out remainder);
                    if (remainder > 0)
                        totalPages += 1;
                }

                lstProducts = Product.GetPage(siteId, zoneId, languageId, -1, pageNumber, pageSize);
            }
            else
                lstProducts = Product.GetPageProductOther(zoneId, product.ProductId, languageId, pageNumber, pageSize, out totalPages);

            foreach (Product productOther in lstProducts)
            {
                XmlElement productXml = doc.CreateElement("ProductOther");
                root.AppendChild(productXml);

                ProductHelper.BuildProductDataXml(doc, productXml, productOther, timeZone, timeOffset,
                                                  ProductHelper.BuildEditLink(productOther, basePage, userCanUpdate, currentUser));

                if (productOther.ProductId == productId)
                    XmlHelper.AddNode(doc, productXml, "IsActive", "true");
                else
                    XmlHelper.AddNode(doc, productXml, "IsActive", "false");
            }

            if (pageNumber < totalPages)
            {
                string pageUrl = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId);

                if (config.LoadFirstProduct)
                    pageUrl = SiteUtils.GetCurrentZoneUrl();

                if (pageUrl.Contains("?"))
                    pageUrl += "&pagenumber=" + (pageNumber + 1).ToString();
                else
                    pageUrl += "?pagenumber=" + (pageNumber + 1).ToString();

                XmlHelper.AddNode(doc, root, "NextPageUrl", pageUrl);
            }
        }

        public void BuildProductRelatedXml(
            XmlDocument doc,
            XmlElement root, int languageId)
        {
            List<Product> lstProducts = Product.GetRelatedProducts(basePage.SiteId, product.ProductGuid, false,
                                        ProductConfiguration.RelatedProductsTwoWayRelationship);

            foreach (Product productRelated in lstProducts)
            {
                XmlElement productXml = doc.CreateElement("ProductRelated");
                root.AppendChild(productXml);

                ProductHelper.BuildProductDataXml(doc, productXml, productRelated, timeZone, timeOffset,
                                                  ProductHelper.BuildEditLink(productRelated, basePage, userCanUpdate, currentUser));
            }
        }

        public void BuildProductPropertiesXml(
            XmlDocument doc,
            XmlElement root,
            int languageId)
        {
            var productProperties = ProductProperty.GetPropertiesByProduct(product.ProductId);

            if (productProperties.Count > 0)
            {
                var customFields = new List<CustomField>();
                var customFieldIds = new List<int>();

                foreach (var property in productProperties)
                {
                    if (!customFieldIds.Contains(property.CustomFieldId))
                        customFieldIds.Add(property.CustomFieldId);
                }

                var tmp = CustomField.GetActiveByFields(basePage.SiteId, Product.FeatureGuid, customFieldIds, languageId);
                customFields = CustomField.GetByOption(tmp, CustomFieldOptions.ShowInProductDetailsPage);

                foreach (CustomField field in customFields)
                {
                    XmlElement groupXml = doc.CreateElement("ProductProperties");
                    root.AppendChild(groupXml);

                    XmlHelper.AddNode(doc, groupXml, "FieldId", field.CustomFieldId.ToString());
                    XmlHelper.AddNode(doc, groupXml, "FieldType", field.FieldType.ToString());
                    XmlHelper.AddNode(doc, groupXml, "DataType", field.DataType.ToString());
                    XmlHelper.AddNode(doc, groupXml, "FilterType", field.FilterType.ToString());
                    XmlHelper.AddNode(doc, groupXml, "Title", field.Name);

                    foreach (ProductProperty property in productProperties)
                    {
                        if (property.ProductId == product.ProductId && property.CustomFieldId == field.CustomFieldId)
                        {
                            XmlElement optionXml = doc.CreateElement("Options");
                            groupXml.AppendChild(optionXml);

                            XmlHelper.AddNode(doc, optionXml, "FieldId", field.CustomFieldId.ToString());
                            XmlHelper.AddNode(doc, optionXml, "OptionId", property.CustomFieldOptionId.ToString());

                            if (property.CustomFieldOptionId > 0)
                            {
                                XmlHelper.AddNode(doc, optionXml, "Title", property.OptionName);
                                XmlHelper.AddNode(doc, optionXml, "Color", property.OptionColor); //TODO: not yet implemented
                                XmlHelper.AddNode(doc, optionXml, "Price", ProductHelper.FormatPrice(product.Price + property.OverriddenPrice, true));
                                XmlHelper.AddNode(doc, optionXml, "PriceAdjustment", ProductHelper.FormatPrice(property.OverriddenPrice, true));
                            }
                            else
                                XmlHelper.AddNode(doc, optionXml, "Title", property.CustomValue);

                            string pageUrl = SiteUtils.GetCurrentZoneUrl();
                            XmlHelper.AddNode(doc, optionXml, "Url", ProductHelper.GetQueryStringFilter(pageUrl, field.FilterType, field.CustomFieldId,
                                              property.CustomFieldOptionId));
                        }
                    }
                }
            }
        }

        public void BuildProductAttributesXml(
            XmlDocument doc,
            XmlElement root, int languageId)
        {
            List<ContentAttribute> listAttributes = ContentAttribute.GetByContentAsc(product.ProductGuid, languageId);
            foreach (ContentAttribute attribute in listAttributes)
            {
                XmlElement element = doc.CreateElement("ProductAttributes");
                root.AppendChild(element);

                XmlHelper.AddNode(doc, element, "Title", attribute.Title);
                XmlHelper.AddNode(doc, element, "Content", attribute.ContentText);
            }
        }

        #endregion

        #region Render Social plugin

        private string RenderTweetThis()
        {
            string twitterWidgets = "<script type=\"text/javascript\">\n"
                                    + "!function (d, s, id) {\n"
                                    + "var js, fjs = d.getElementsByTagName(s)[0];\n"
                                    + "if (!d.getElementById(id)) {\n"
                                    + "js = d.createElement(s);\n"
                                    + "js.id = id; js.src = \"//platform.twitter.com/widgets.js\";\n"
                                    + "fjs.parentNode.insertBefore(js, fjs);\n"
                                    + "}\n"
                                    + "} (document, \"script\", \"twitter-wjs\");"
                                    + "</script>\n";

            Page.ClientScript.RegisterClientScriptBlock(typeof(Page),
                    "twitterwidgets", twitterWidgets);

            string twitterUrl = "https://twitter.com/share";

            string tweetThis = "<a class='twitter-share-button'";
            tweetThis += " title='Tweet This'";
            tweetThis += " href='" + twitterUrl + "'";
            tweetThis += " data-url='" + ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId) + "'";
            tweetThis += " data-text='" + product.Title + "'";
            tweetThis += " data-count='horizontal'";
            tweetThis += "></a>";

            return tweetThis;
        }

        private string RenderPlusOne()
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page),
                                                    "gplusone", "\n<script type=\"text/javascript\" src=\""
                                                    + "https://apis.google.com/js/plusone.js" + "\"></script>", false);

            string plusOne = "<div class='g-plusone' data-size='medium' data-count='true'";
            plusOne += " data-href='" + ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId);
            plusOne += "'></div>";

            return plusOne;
        }

        private string RenderFacebookLike()
        {
            string facebook = string.Empty;

            if (WebConfigSettings.DisableFacebookLikeButton) return facebook;

            string facebookWidgets = "<script type=\"text/javascript\">\n"
                                     + "(function (d, s, id) {\n"
                                     + "var js, fjs = d.getElementsByTagName(s)[0];\n"
                                     + "if (d.getElementById(id)) return;\n"
                                     + "js = d.createElement(s); js.id = id\n"
                                     + "js.async=true;\n"
                                     + "js.src = \"//connect.facebook.net/en_US/all.js#xfbml=1\";\n"
                                     + "fjs.parentNode.insertBefore(js, fjs);\n"
                                     + "} (document, 'script', 'facebook-jssdk'));\n"
                                     + "</script>\n";

            Page.ClientScript.RegisterClientScriptBlock(typeof(Page),
                    "facebookwidgets", facebookWidgets);

            facebook = "<a class='fb-like'";
            facebook += " data-href='" + ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId) + "'";
            facebook += " data-send='false'";
            facebook += " data-layout='button_count'";
            facebook += " data-width='100'";
            facebook += " data-show-faces='false'";
            facebook += "></a>";

            return facebook;
        }

        private string RenderPrinter()
        {
            string url = WebUtils.GetUrlWithoutQueryString(Context.Request.RawUrl)
                         + "?skin=printerfriendly"
                         + WebUtils.BuildQueryString(WebUtils.GetQueryString(Context.Request.RawUrl), "skin");

            return string.Format("<a href='{0}' target='_blank' title='{1}' rel='nofollow'><span>{2}</span></a>",
                                 Context.Server.HtmlEncode(url),
                                 ProductResources.ProductPrint,
                                 ProductResources.ProductPrint);
        }

        //private string RenderEmailSubject()
        //{
        //    SetupEmailSubjectScript();

        //    return string.Format("<a href='{0}' class='email-link' target='_blank' title='{1}' rel='nofollow'><span>{2}</span></a>",
        //                siteRoot + "/News/EmailSubjectDialog.aspx?u=" + Context.Server.HtmlEncode(ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId)),
        //                ProductResources.ProductEmailSubject,
        //                ProductResources.ProductEmailSubject);
        //}

        //private void SetupEmailSubjectScript()
        //{
        //    basePage.ScriptConfig.IncludeFancyBox = true;

        //    StringBuilder script = new StringBuilder();

        //    script.Append("<script type=\"text/javascript\">");

        //    string fancyBoxConfig = "{type:'iframe', width:500, height:400, title:{type:'outside'} }";
        //    script.Append("$('.popup-iframe a.email-link').fancybox(" + fancyBoxConfig + "); ");

        //    script.Append("</script>");

        //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "cbemailsubjectinit", script.ToString());
        //}

        #endregion

        private void PopulateLabels()
        {

        }

        private void LoadSettings()
        {
            if ((product == null || product.ProductId == -1) && productId > 0)
                product = new Product(basePage.SiteId, productId, languageId);

            currentUser = SiteUtils.GetCurrentSiteUser();
            userCanUpdate = ProductPermission.CanUpdate;
            pageNumber = WebUtils.ParseInt32FromQueryString("pagenumber", pageNumber);

            if (
                //(module.ModuleId == -1)
                //|| (product.ModuleID == -1)
                //|| (product.ModuleID != module.ModuleId)
                (basePage.SiteInfo == null)
            )
            {
                // query string params have been manipulated
                pnlInnerWrap.Visible = false;
                parametersAreInvalid = true;
                return;
            }

            userCanEditAsDraft = ProductPermission.CanCreate;

            SetupCommentSystem();
        }

        private void SetupCommentSystem()
        {
            comment.Config = config;
            comment.Product = product;
        }

        private void LoadParams()
        {
            virtualRoot = WebUtils.GetApplicationRoot();
            timeOffset = SiteUtils.GetUserTimeOffset();
            timeZone = SiteUtils.GetUserTimeZone();
            zoneId = WebUtils.ParseInt32FromQueryString("zoneId", -1);
            productId = WebUtils.ParseInt32FromQueryString("ProductID", -1);
            languageId = WorkingCulture.LanguageId;

            if (config.LoadFirstProduct)
            {
                product = Product.GetOneByZone(zoneId, languageId);
                if (product != null && product.ProductId > 0)
                    productId = Convert.ToInt32(product.ProductId);
            }

            if (zoneId == -1) parametersAreInvalid = true;
            if (productId == -1) parametersAreInvalid = true;
            //if (!basePage.UserCanViewPage(moduleId)) { parametersAreInvalid = true; }
        }

    }
}