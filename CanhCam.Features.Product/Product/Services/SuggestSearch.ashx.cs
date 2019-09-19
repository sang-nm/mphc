/// Author:                 Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:			    2014-08-30
/// Last Modified:		    2014-09-01

using System;
using System.Text;
using System.Web;
using System.Web.Services;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.Web.Framework;
using CanhCam.SearchIndex;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace CanhCam.Web.ProductUI
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SuggestSearch : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Encoding encoding = new UTF8Encoding();
            context.Response.ContentEncoding = encoding;

            string query = WebUtils.ParseStringFromQueryString("q", string.Empty);
            if (query.Length == 0)
                return;

            //https://weblearn.ox.ac.uk/portal/help/TOCDisplay/content.hlp?docId=howdoiperformanadvancedsearch
            if (WebConfigSettings.EscapingSpecialCharactersInKeyword) //Escaping Special Characters: + - && || ! ( ) { } [ ] ^ " ~ * ? : \
                query = query.Replace("-", "\\-")
                             .Replace("+", "\\+")
                             .Replace("&&", "\\&&")
                             .Replace("||", "\\||")
                             .Replace("!", "\\!")
                             .Replace("(", "\\(")
                             .Replace(")", "\\)")
                             .Replace("{", "\\{")
                             .Replace("}", "\\}")
                             .Replace("[", "\\[")
                             .Replace("]", "\\]")
                             .Replace("^", "\\^")
                             .Replace("\"", "\\\"")
                             .Replace("~", "\\~")
                             .Replace("*", "\\*")
                             .Replace("?", "\\?")
                             .Replace(":", "\\:")
                             .Replace("\\", "\\\\")
                             ;

            query = query.Trim().Replace(" ", " + ") + "*";

            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
            int pageSize = 20;
            int totalHits = 1;
            bool queryErrorOccurred = false;
            bool isSiteEditor = WebUser.IsAdminOrContentAdmin || (SiteUtils.UserIsSiteEditor());
            string result = string.Empty;

            CanhCam.SearchIndex.IndexItemCollection searchResults = CanhCam.SearchIndex.IndexHelper.Search(
                siteSettings.SiteId,
                isSiteEditor,
                GetUserRoles(context, siteSettings.SiteId),
                Product.FeatureGuid,
                WorkingCulture.DefaultName,
                DateTime.MinValue,
                DateTime.MaxValue,
                query,
                false,
                WebConfigSettings.SearchResultsFragmentSize,
                1,
                pageSize,
                WebConfigSettings.SearchMaxClauseCount,
                out totalHits,
                out queryErrorOccurred);

            if (searchResults.Count > 0)
            {
                string productGuids = string.Empty;
                string sepa = string.Empty;
                foreach (IndexItem item in searchResults)
                {
                    if(!productGuids.ContainsCaseInsensitive(item.ItemGuid.ToString()))
                    {
                        productGuids += sepa + item.ItemGuid.ToString();
                        sepa = ";";
                    }
                }

                if (productGuids.Length > 0)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml("<ProductList></ProductList>");
                    XmlElement root = doc.DocumentElement;

                    var timeOffset = SiteUtils.GetUserTimeOffset();
                    var timeZone = SiteUtils.GetUserTimeZone();

                    List<Product> lstProducts = Product.GetByGuids(siteSettings.SiteId, productGuids, 1, WorkingCulture.LanguageId);
                    foreach (Product product in lstProducts)
                    {
                        XmlElement productXml = doc.CreateElement("Product");
                        root.AppendChild(productXml);

                        ProductHelper.BuildProductDataXml(doc, productXml, product, timeZone, timeOffset, string.Empty);
                    }

                    result = XmlHelper.TransformXML(SiteUtils.GetXsltBasePath("product", "ProductSuggestSearch.xslt"), doc);
                }
            }

            if(result.Length > 0)
                context.Response.Write(result);
            else
                context.Response.Write(" ");

            context.Response.End();
        }

        private List<string> GetUserRoles(HttpContext context, int siteId)
        {
            List<string> userRoles = new List<string>();

            userRoles.Add("All Users");
            if (context.User.Identity.IsAuthenticated)
            {
                SiteUser currentUser = SiteUtils.GetCurrentSiteUser();
                if (currentUser != null)
                {
                    using (IDataReader reader = SiteUser.GetRolesByUser(siteId, currentUser.UserId))
                    {
                        while (reader.Read())
                        {
                            userRoles.Add(reader["RoleName"].ToString());
                        }

                    }

                }
            }

            return userRoles;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
