/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-23
/// Last Modified:			2014-08-02

using System;
using System.Web.Services;
using Telerik.Web.UI;
using System.Collections.Generic;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;

namespace CanhCam.Web.ProductUI
{
    public partial class ProductEditPage : CmsNonBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        [WebMethod]
        public static AutoCompleteBoxData GetProductTags(object context)
        {
            string searchString = ((Dictionary<string, object>)context)["Text"].ToString();
            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
            List<Tag> lstTags = Tag.GetPage(siteSettings.SiteGuid, Product.FeatureGuid, searchString, -1, 1, 10);
            List<AutoCompleteBoxItemData> result = new List<AutoCompleteBoxItemData>();
 
            foreach (Tag tag in lstTags)
            {
                AutoCompleteBoxItemData childNode = new AutoCompleteBoxItemData();
                childNode.Text = tag.TagText;
                childNode.Value = tag.TagId.ToString();
                result.Add(childNode);
            }
 
            AutoCompleteBoxData res = new AutoCompleteBoxData();
            res.Items = result.ToArray();
 
            return res;
        }

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion
    }
}