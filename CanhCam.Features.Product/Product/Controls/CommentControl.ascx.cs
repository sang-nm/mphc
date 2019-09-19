/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-08-13
/// Last Modified:			2014-10-23

using System;
using System.Text;
using System.Web.UI;
using log4net;
using CanhCam.Business;
using CanhCam.Web.UI;
using System.Collections.Generic;
using System.Linq;
using CanhCam.Web.Framework;

namespace CanhCam.Web.ProductUI
{
    public partial class CommentControl : UserControl, IRefreshAfterPostback
    {
        #region Properties

        private static readonly ILog log = LogManager.GetLogger(typeof(CommentControl));

        protected ProductConfiguration config = new ProductConfiguration();
        private SiteUser currentUser = null;
        private Product product = null;

        private int pageNumber = 1;
        protected bool allowComments = true;
        protected Double timeOffset = 0;
        protected TimeZoneInfo timeZone = null;

        public ProductConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }

        public Product Product
        {
            get { return product; }
            set
            {
                product = value;
            }
        }

        private ProductCommentType commentType = ProductCommentType.Comment;
        public ProductCommentType CommentType
        {
            get { return commentType; }
            set
            {
                commentType = value;
            }
        }

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);

            base.OnInit(e);
        }

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            LoadSettings();

            if (!allowComments || !displaySettings.ShowComments)
            {
                this.Visible = false;
                return;
            }

            SetupScript();
            PopulateLabels();
            PopulateControls();
        }

        protected virtual void PopulateControls()
        {
            SetupCommentSystem();
        }

        #region IRefreshAfterPostback

        public void RefreshAfterPostback()
        {
            PopulateControls();
        }

        #endregion

        private void PopulateLabels()
        {
            if (commentType == (int)ProductCommentType.Comment)
            {
                divStar.Visible = false;
            }
            else
            {
                divStar.Visible = true;
            }

            pnlFeedback.Attributes.Add("class", "commentpanel commentpanel" + ((int)commentType).ToString());
        }

        private void LoadSettings()
        {
            timeOffset = SiteUtils.GetUserTimeOffset();
            timeZone = SiteUtils.GetUserTimeZone();
            currentUser = SiteUtils.GetCurrentSiteUser();
            pageNumber = WebUtils.ParseInt32FromQueryString("cmtpg", pageNumber);

            if (
                (!this.Visible)
                || (product == null)
                || (product.ProductId == -1)
                )
            {
                // query string params have been manipulated
                allowComments = false;
                return;
            }
        }

        private void SetupScript()
        {
            string skinUrl = SiteUtils.DetermineSkinBaseUrl(SiteUtils.GetSkinName(false));
            //if (!Page.ClientScript.IsStartupScriptRegistered("commentplugin"))
            //{
            //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "commentplugin", "<script type=\"text/javascript\" src=\""
            //        + skinUrl + "comment/script.js" + "\"></script>");
            //}

            StringBuilder scripts = new StringBuilder();

            scripts.Append("<script type=\"text/javascript\">"); // script

            scripts.Append("\nvar journalOptions = {};"); // journalOptions
            scripts.Append("\njournalOptions.productId=" + product.ProductId + ";");
            scripts.Append("\njournalOptions.pageSize=" + pgr.PageSize + ";");
            scripts.Append("\njournalOptions.siteRoot='" + SiteUtils.GetNavigationSiteRoot() + "';");

            if (Request.IsAuthenticated)
            {
                scripts.Append("\njournalOptions.fullName='" + currentUser.Name + "';");
                scripts.Append("\njournalOptions.email='" + currentUser.Email + "';");
            }
            else
            {
                scripts.Append("\njournalOptions.fullName='" + CommentHelper.GetNameCookieValue() + "';");
                scripts.Append("\njournalOptions.email='';");
            }

            scripts.Append("\njournalOptions.deleteConfirmText = 'Bạn có chắc chắn muốn xóa bình luận?';");
            scripts.Append("\njournalOptions.reportSuccessfullyText = 'Đã báo cáo vi phạm thành công!';");
            scripts.Append("\njournalOptions.contentRequiredText = 'Vui lòng nhập nội dung.';");
            scripts.Append("\njournalOptions.contentInvalidText = 'Nội dung không hợp lệ.';");
            scripts.Append("\njournalOptions.fullNameRequiredText = 'Vui lòng nhập tên của bạn.';");
            scripts.Append("\njournalOptions.emailInvalidText = 'Email không hợp lệ.';");
            scripts.Append("\njournalOptions.likeText = 'Thích';");
            scripts.Append("\njournalOptions.unlikeText = 'Không thích';"); // end journalOptions
            scripts.Append("\nvar commentOpts = {};");

            scripts.Append("\n\n$(document).ready(function () {"); // document ready
            
            scripts.Append("\npluginInit();");

            scripts.Append("\nif($('.rating').length){"); // init rating
            scripts.Append("\n$('.rating').raty({");
            scripts.Append("\npath:'" + skinUrl + "comment/images/',");
            scripts.Append("\nscore: function() { return $(this).attr('data-score'); }");
            scripts.Append("\n});");
            scripts.Append("\n}"); // end init rating

            scripts.Append("\nvar jopts = {};");
            scripts.Append("\njopts.maxLength=" + 2000 + ";");
            scripts.Append("\njopts.placeHolder = '#" + pnlFeedback.ClientID + " .journalPlaceholder';");
            scripts.Append("\njopts.shareButton = '#" + pnlFeedback.ClientID + " .btnShare';");
            scripts.Append("\njopts.closeButton = '#" + pnlFeedback.ClientID + " .journalClose';");
            scripts.Append("\njopts.info = '#" + pnlFeedback.ClientID + " .journalInfo';");
            scripts.Append("\njopts.fullName = '#" + pnlFeedback.ClientID + " .txtFullName';");
            scripts.Append("\njopts.email = '#" + pnlFeedback.ClientID + " .txtEmail';");
            scripts.Append("\njopts.content = '#" + pnlFeedback.ClientID + " .journalContent';");
            scripts.Append("\njopts.items = '#" + pnlFeedback.ClientID + " .journalItems';");
            scripts.Append("\njopts.pager = '#" + pnlFeedback.ClientID + " .commentpager a';");
            scripts.Append("\n$('body').journalTools(jopts);");

            scripts.Append("\n});"); // end document ready
            scripts.Append("\n</script>"); // end script
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "commentinit" + ((int)commentType).ToString(), scripts.ToString());
        }

        private void SetupCommentSystem()
        {
            if (!ShouldAllowComments())
            {
                return;
            }

            string commentSystem = DetermineCommentSystem();

            switch (commentSystem)
            {
                case "disqus":

                    break;

                case "intensedebate":

                    break;

                case "facebook":

                    break;

                case "internal":
                default:
                    if (ProductConfiguration.UseLegacyCommentSystem)
                    {
                        SetupLegacyProductComments();
                    }

                    break;
            }
        }

        private bool ShouldAllowComments()
        {
            //comments closed globally
            if (!displaySettings.ShowComments) { return false; }

            // comments not allowed on this post
            if (product.AllowCommentsForDays < 0) { return false; }

            return true;
        }

        private bool CommentsAreOpen()
        {
            //comments closed globally
            if (!displaySettings.ShowComments) { return false; }

            // comments not allowed on this product
            if (product.AllowCommentsForDays < 0) { return false; }

            //no limit on comments for this product
            if (product.AllowCommentsForDays == 0) { return true; }

            if (product.AllowCommentsForDays > 0) //limited to a specific number of days
            {
                DateTime endDate = product.StartDate.AddDays((double)product.AllowCommentsForDays);

                if (endDate > DateTime.UtcNow) { return true; }

            }

            return false;
        }

        private string DetermineCommentSystem()
        {
            // don't use new external comment system for existing posts that already have comments
            if (product.CommentCount > 0) { return "internal"; }

            return config.CommentSystem;
        }

        private void SetupLegacyProductComments()
        {
            pnlFeedback.Visible = true;
            pnlFeedback.Attributes["data-type"] = ((int)commentType).ToString();

            if (!IsPostBack)
                BindComments();
        }

        private void BindComments()
        {
            var iCount = ProductComment.GetCount(product.SiteId, product.ProductId, (int)commentType, 10, -1, -1, null, null, null);

            string pageUrl = ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId);
            pageUrl += "?cmtpg={0}";

            pgr.PageURLFormat = pageUrl;
            pgr.ItemCount = iCount;
            pgr.CurrentIndex = pageNumber;
            divPager.Visible = (iCount > pgr.PageSize);

            var lstComments = ProductComment.GetPage(product.SiteId, product.ProductId, (int)commentType, 10, -1, -1, null, null, null, 0, pageNumber, pgr.PageSize);
            if (lstComments.Count > 0)
            {
                rptComments.DataSource = lstComments;
                rptComments.DataBind();

                var lstTopComments = ProductComment.GetPage(product.SiteId, product.ProductId, (int)commentType, 10, 0, 1, null, null, null, 0, 1, 3);
                if (lstTopComments.Count > 0)
                {
                    rptCommentTop.DataSource = lstTopComments;
                    rptCommentTop.DataBind();
                }
            }
            else
            {
                rptComments.Visible = false;
            }
        }

        protected string FormatRating(int rating)
        {
            if (rating > 5)
                rating = 5;
            return string.Format("<div style=\"width:{0}%\"></div>", rating * 20);
        }

        protected List<ProductComment> GetChildComments(int parentId)
        {
            var lstComments = ProductComment.GetPage(product.SiteId, product.ProductId, (int)commentType, 10, parentId, -1, null, null, null, 1, 1, pgr.PageSize);

            return lstComments;
        }

    }
}