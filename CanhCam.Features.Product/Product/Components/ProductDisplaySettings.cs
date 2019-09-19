/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-06-23
/// Last Modified:			2014-06-23

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CanhCam.Web.ProductUI
{
    /// <summary>
    /// This control doesn't render anything, it is used only as a themeable collection of settings for things we would like to be able to configure from theme.skin
    /// </summary>
    public class ProductDisplaySettings : WebControl
    {
        private bool showAttribute = true;
        public bool ShowAttribute
        {
            get { return showAttribute; }
            set { showAttribute = value; }
        }

        private bool showRelatedProduct = false;
        public bool ShowRelatedProduct
        {
            get { return showRelatedProduct; }
            set { showRelatedProduct = value; }
        }

        private bool showNextPreviousLink = false;
        public bool ShowNextPreviousLink
        {
            get { return showNextPreviousLink; }
            set { showNextPreviousLink = value; }
        }

        private string resizeBackgroundColor = "#FFFFFF";
        public string ResizeBackgroundColor
        {
            get { return resizeBackgroundColor; }
            set { resizeBackgroundColor = value; }
        }

        private bool showComments = false;
        public bool ShowComments
        {
            get { return showComments; }
            set { showComments = value; }
        }

        private string commentDateFormat = "(dd/MM/yyyy)";
        public string CommentDateFormat
        {
            get { return commentDateFormat; }
            set { commentDateFormat = value; }
        }

        private string commentResourceFile = "ProductResources";
        public string CommentResourceFile
        {
            get { return commentResourceFile; }
            set { commentResourceFile = value; }
        }

        private bool commentUsingPlaceholder = false;
        public bool CommentUsingPlaceholder
        {
            get { return commentUsingPlaceholder; }
            set { commentUsingPlaceholder = value; }
        }

        private bool showTags = false;
        public bool ShowTags
        {
            get { return showTags; }
            set { showTags = value; }
        }

        private bool showProductCode = false;
        public bool ShowProductCode
        {
            get { return showProductCode; }
            set { showProductCode = value; }
        }

        private bool showPrice = true;
        public bool ShowPrice
        {
            get { return showPrice; }
            set { showPrice = value; }
        }

        private bool showOldPrice = false;
        public bool ShowOldPrice
        {
            get { return showOldPrice; }
            set { showOldPrice = value; }
        }

        private bool showSubTitle = false;
        public bool ShowSubTitle
        {
            get { return showSubTitle; }
            set { showSubTitle = value; }
        }

        private bool showVideo = false;
        public bool ShowVideo
        {
            get { return showVideo; }
            set { showVideo = value; }
        }

        private bool showAttachment = false;
        public bool ShowAttachment
        {
            get { return showAttachment; }
            set { showAttachment = value; }
        }

        private bool showStockQuantity = false;
        public bool ShowStockQuantity
        {
            get { return showStockQuantity; }
            set { showStockQuantity = value; }
        }

        private int cartPageId = -1;
        public int CartPageId
        {
            get { return cartPageId; }
            set { cartPageId = value; }
        }

        private int checkoutPageId = -1;
        public int CheckoutPageId
        {
            get { return checkoutPageId; }
            set { checkoutPageId = value; }
        }

        private int comparePageId = -1;
        public int ComparePageId
        {
            get { return comparePageId; }
            set { comparePageId = value; }
        }

        private int thumbnailWidth = 320;
        public int ThumbnailWidth
        {
            get { return thumbnailWidth; }
            set { thumbnailWidth = value; }
        }

        private int thumbnailHeight = 100000;
        public int ThumbnailHeight
        {
            get { return thumbnailHeight; }
            set { thumbnailHeight = value; }
        }

        private bool enableColorSwitcher = false;
        public bool EnableColorSwitcher
        {
            get { return enableColorSwitcher; }
            set { enableColorSwitcher = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (HttpContext.Current == null)
            {
                writer.Write("[" + this.ID + "]");
                return;
            }

            // nothing to render
        }
    }
}