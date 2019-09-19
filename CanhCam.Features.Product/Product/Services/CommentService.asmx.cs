using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using CanhCam.Web.Framework;
using CanhCam.Business;
using System.Text;
using CanhCam.Net;
using CanhCam.Business.WebHelpers;

namespace CanhCam.Web.ProductUI
{
    /// <summary>
    /// Summary description for CommentService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CommentService : System.Web.Services.WebService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommentService));

        [WebMethod]
        public void Create(int productId, int commentType, string fullName, string email, string content)
        {
            CreateRating(productId, commentType, fullName, email, content, 0);
        }

        [WebMethod]
        //[ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public void CreateRating(int productId, int commentType, string fullName, string email, string content, int rating)
        {
            try
            {
                //Context.Response.ContentType = "application/json";
                Context.Response.ContentType = "text/html";
                Encoding encoding = new UTF8Encoding();
                Context.Response.ContentEncoding = encoding;

                if (!ShouldAllowComments(productId))
                {
                    Context.Response.Write(" ");
                    return;
                }
                if (!IsValidComment())
                {
                    Context.Response.Write(" ");
                    return;
                }

                if (commentType != (int)ProductCommentType.Comment && commentType != (int)ProductCommentType.Rating)
                {
                    Context.Response.Write(" ");
                    return;
                }

                if (commentType == (int)ProductCommentType.Rating && (rating < 1 || rating > 5))
                {
                    Context.Response.Write(" ");
                    return;
                }

                if (fullName.Length == 0 || content.Length == 0)
                {
                    Context.Response.Write(" ");
                    return;
                }

                string emailDecode = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(email));
                if (email.Length > 0 && !Email.IsValidEmailAddressSyntax(emailDecode))
                {
                    Context.Response.Write(" ");
                    return;
                }

                ProductComment comment = new ProductComment();
                comment.CommentType = commentType;
                comment.ProductId = productId;
                comment.FullName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(fullName));
                comment.Email = emailDecode;
                comment.ContentText = SecurityHelper.RemoveMarkup(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(content)));
                comment.IsApproved = true;
                comment.CreatedFromIP = SiteUtils.GetIP4Address();
                //comment.UserGuid = CommentHelper.GetCommentSessionGuid(true);

                if (commentType == (int)ProductCommentType.Rating)
                    comment.Rating = rating;

                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
                    if (siteUser != null)
                        comment.UserId = siteUser.UserId;

                    if (ProductPermission.CanApproveComment)
                    {
                        comment.Status = 1;
                        comment.IsModerator = true;
                    }
                }
                else
                    CommentHelper.SetNameCookie(comment.FullName);

                comment.Save();

                //if (config.NotifyOnComment)
                //{
                //    CommentHelper.SendCommentNotification(config.NotifyEmail);
                //}

                var results = GetResponse(comment, SiteUtils.GetUserTimeZone(), SiteUtils.GetUserTimeOffset());
                if (results.Length > 0)
                    Context.Response.Write(results);
                else
                    Context.Response.Write(" ");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Context.Response.Write(" ");
            }
        }

        private bool ShouldAllowComments(int productId)
        {
            //comments closed globally
            //if (!displaySettings.ShowComment) { return false; }

            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
            Product product = new Product(siteSettings.SiteId, productId);

            if (product == null || product.ProductId <= 0) return false;

            // comments not allowed on this post
            if (product.AllowCommentsForDays < 0) return false;

            DateTime endDate = product.StartDate.AddDays((double)product.AllowCommentsForDays);

            if ((endDate < DateTime.UtcNow) && (product.AllowCommentsForDays > 0)) { return false; }

            return true;
        }

        private bool IsValidComment()
        {
            return true;
        }

        [WebMethod]
        public void Delete(int commentId)
        {
            try
            {
                Context.Response.ContentType = "application/json";
                Encoding encoding = new UTF8Encoding();
                Context.Response.ContentEncoding = encoding;

                if (!ProductPermission.CanDeleteComment)
                {
                    Context.Response.Write("{\"Result\":\"Access Denined\"}");
                    return;
                }

                ProductComment comment = new ProductComment(commentId);

                if (comment != null && comment.CommentId > -1)
                {
                    ProductComment.Delete(comment.CommentId);
                    LogActivity.Write("Delete comment", comment.FullName);
                }

                Context.Response.Write("{\"Result\":\"Success\"}");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Context.Response.Write("{\"Result\":\"Error\"}");
            }
        }

        [WebMethod]
        public void Report(int commentId)
        {
            try
            {
                Context.Response.ContentType = "application/json";
                Encoding encoding = new UTF8Encoding();
                Context.Response.ContentEncoding = encoding;

                ProductComment comment = new ProductComment(commentId);
                if (comment != null && comment.CommentId > -1 && comment.Status == 0)
                {
                    comment.Status = -2;
                    comment.Save();
                }

                Context.Response.Write("{\"Result\":\"Success\"}");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Context.Response.Write("{\"Result\":\"Error\"}");
            }
        }

        [WebMethod(EnableSession = true)]
        public void Like(int commentId)
        {
            ProductComment comment = new ProductComment(commentId);

            try
            {
                Context.Response.ContentType = "text/html";
                Encoding encoding = new UTF8Encoding();
                Context.Response.ContentEncoding = encoding;

                if (comment != null && comment.CommentId > -1)
                {
                    var lstCommentIds = CommentHelper.UserLikedComments;

                    if (!lstCommentIds.Contains(comment.CommentId))
                    {
                        comment.HelpfulYesTotal += 1;
                        lstCommentIds.Add(comment.CommentId);
                    }
                    else
                    {
                        comment.HelpfulYesTotal -= 1;
                        if (comment.HelpfulYesTotal < 0)
                            comment.HelpfulYesTotal = 0;

                        lstCommentIds.Remove(comment.CommentId);
                    }

                    CommentHelper.UserLikedComments = lstCommentIds;
                    ProductComment.UpdateYesTotal(comment.CommentId, comment.HelpfulYesTotal);
                }

                Context.Response.Write(CommentHelper.GetLikeText(comment.CommentId) + "<i class='iconcom-likecomm'></i><span>" + comment.HelpfulYesTotal + "</span>");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Context.Response.Write(CommentHelper.GetLikeText(commentId) + "<i class='iconcom-likecomm'></i><span>" + comment.HelpfulYesTotal + "</span>");
            }
        }

        [WebMethod]
        public void CommentSave(int productId, int commentType, int commentId, string fullName, string email, string content)
        {
            try
            {
                Context.Response.ContentType = "text/html";
                Encoding encoding = new UTF8Encoding();
                Context.Response.ContentEncoding = encoding;

                if (commentType != (int)ProductCommentType.Comment && commentType != (int)ProductCommentType.Rating)
                {
                    Context.Response.Write(" ");
                    return;
                }

                if (fullName.Length == 0 || content.Length == 0)
                {
                    Context.Response.Write(" ");
                    return;
                }

                string emailDecode = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(email));
                if (email.Length > 0 && !Email.IsValidEmailAddressSyntax(emailDecode))
                {
                    Context.Response.Write(" ");
                    return;
                }

                var parent = new ProductComment(commentId);
                if (parent == null || parent.CommentId == -1 || parent.ParentId > 0)
                {
                    Context.Response.Write(" ");
                    return;
                }

                ProductComment comment = new ProductComment();
                comment.ParentId = commentId;
                comment.ProductId = productId;
                comment.CommentType = commentType;
                comment.FullName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(fullName));
                comment.Email = emailDecode;
                comment.ContentText = SecurityHelper.RemoveMarkup(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(content)));
                comment.IsApproved = true;
                comment.CreatedFromIP = SiteUtils.GetIP4Address();
                //comment.UserGuid = CommentHelper.GetCommentSessionGuid(true);

                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    SiteUser siteUser = SiteUtils.GetCurrentSiteUser();
                    if (siteUser != null)
                        comment.UserId = siteUser.UserId;

                    if (ProductPermission.CanApproveComment)
                    {
                        comment.Status = 1;
                        comment.IsModerator = true;

                        if (parent.Status != 1)
                        {
                            parent.Status = 1;
                            parent.Save();
                        }
                    }
                }
                else
                    CommentHelper.SetNameCookie(comment.FullName);

                comment.Save();

                Context.Response.Write(GetChildResponse(comment));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Context.Response.Write(" ");
            }
        }

        [WebMethod]
        public void GetList(int productId, int commentType, int parentId, int pageNumber, int pageSize)
        {
            try
            {
                Context.Response.ContentType = "text/html";
                Encoding encoding = new UTF8Encoding();
                Context.Response.ContentEncoding = encoding;

                SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();
                var lstComments = ProductComment.GetPage(siteSettings.SiteId, productId, commentType, 10, parentId, -1, null, null, null, 0, pageNumber, pageSize);
                TimeZoneInfo timeZone = SiteUtils.GetUserTimeZone();
                double timeOffset = SiteUtils.GetUserTimeOffset();

                StringBuilder results = new StringBuilder();
                foreach (ProductComment comment in lstComments)
                {
                    results.Append(GetResponse(comment, timeZone, timeOffset, false));
                    results.Append("<ul class='jcmt' id='jcmt-" + comment.CommentId + "'>");

                    var lstChildComments = ProductComment.GetPage(siteSettings.SiteId, comment.ProductId, commentType, 10, comment.CommentId, -1, null, null, null, 1, 1, pageSize);
                    if (lstChildComments.Count > 0)
                    {
                        foreach (ProductComment child in lstChildComments)
                        {
                            results.Append(GetChildResponse(child));
                        }
                    }

                    results.Append("<li class='cmteditarea' id='jcmt-" + comment.CommentId + "-txtrow'><textarea id='jcmt-" + comment.CommentId + "-txt' class='cmteditor'></textarea><div class='editorPlaceholder'>Mời bạn thảo luận hoặc đánh giá về sản phẩm này</div></li><li class='cmtbtn'><div class='cmtinfo'><input type='text' maxlength='100' placeholder='Mời bạn nhập tên (Bắt buộc)' class='cmtname'> <input type='text' maxlength='100' placeholder='Mời bạn nhập email (Không bắt buộc)' class='cmtemail'> <a href='#'>Gửi bình luận</a></div></li>");
                    results.Append("</ul></div>");//end div jid
                }

                if (results.Length > 0)
                    Context.Response.Write(results.ToString());
                else
                    Context.Response.Write(" ");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Context.Response.Write(" ");
            }
        }

        private string GetResponse(ProductComment comment, TimeZoneInfo timeZone, double timeOffset, bool closejidDiv = true)
        {
            StringBuilder results = new StringBuilder();

            string mod = string.Empty;
            if (comment.IsModerator)
                mod = "<b class='mod'>Quản trị viên</b>";

            if (comment.CommentType == (int)ProductCommentType.Rating)
            {
                mod += string.Format("<div class=\"ratingview\"><div style=\"width:{0}%\"></div></div>", comment.Rating * 20);
            }

            string report = string.Empty;
            if (comment.Status == 0)
                report = "<span id='report-" + comment.CommentId + "' class='cmreport' onclick='journalReport(this);'>Báo vi phạm</span>";

            results.Append("<div id='jid-" + comment.CommentId + "' class='journalrow'>");//div jid
            results.Append("<div class='author'><i class='iconcom'></i></div>");

            results.Append("<div class='journalitem'>");//div journalitem
            if (ProductPermission.CanDeleteComment)
                results.Append("<div class='minidel' onclick='journalDelete(this);'></div>");
            results.Append("<div class='journalsummary'><strong>" + Server.HtmlEncode(comment.FullName) + "</strong>" + mod + "<div>" + Server.HtmlEncode(comment.ContentText).Replace("\n", "<br/>").Replace("\r\n", "<br />") + "</div></div>");
            results.Append("<div class='journalfooter'><a href='#' id='cmtbtn-" + comment.CommentId + "' class='cmreply'>Trả lời</a> <a href='#' id='like-" + comment.CommentId + "' class='cmlike'>Thích <i class='iconcom-likecomm'></i> <span>" + comment.HelpfulYesTotal + "</span></a> <abbr title='" + DateTimeHelper.Format(Convert.ToDateTime(comment.CreatedUtc), timeZone, "g", timeOffset) + "'>" + CommentHelper.GetTimeAgo(comment.CreatedUtc, timeZone, timeOffset, new System.Globalization.CultureInfo("vi-VN")) + "</abbr>" + report + "</div>");
            results.Append("</div>");//end div journalitem

            if (closejidDiv)
            {
                results.Append("<ul class='jcmt' id='jcmt-" + comment.CommentId + "'><li class='cmteditarea' id='jcmt-" + comment.CommentId + "-txtrow'><textarea id='jcmt-" + comment.CommentId + "-txt' class='cmteditor'></textarea><div class='editorPlaceholder'>Mời bạn thảo luận hoặc đánh giá về sản phẩm này</div></li><li class='cmtbtn'><div class='cmtinfo'><input type='text' maxlength='100' placeholder='Mời bạn nhập tên (Bắt buộc)' class='cmtname'> <input type='text' maxlength='100' placeholder='Mời bạn nhập email (Không bắt buộc)' class='cmtemail'> <a href='#'>Gửi bình luận</a></div></li></ul>");
                results.Append("</div>");//end div jid
            }

            return results.ToString();
        }

        private string GetChildResponse(ProductComment comment)
        {
            string mod = string.Empty;
            string del = string.Empty;
            if (comment.IsModerator)
                mod = "<b class='mod'>Quản trị viên</b>";
            if (ProductPermission.CanDeleteComment)
                del = "<div class='miniclose'></div>";

            string report = string.Empty;
            if (comment.Status == 0)
                report = "<span id='report-" + comment.CommentId + "' class='cmreport' onclick='journalReport(this);'>Báo vi phạm</span>";

            return "<li id='cmt-" + comment.CommentId.ToString() + "'>" + del + "<i class='iconcom'></i><div class='jsummary'><strong>" + HttpUtility.HtmlEncode(comment.FullName) + "</strong>" + mod + "<div>" + Server.HtmlEncode(comment.ContentText).Replace("\n", "<br/>").Replace("\r\n", "<br />") + "</div><a href='#' id='like-" + comment.CommentId + "' class='cmlike'>Thích<i class='iconcom-likecomm'></i><span>" + comment.HelpfulYesTotal + "</span></a><abbr title='" + DateTimeHelper.Format(comment.CreatedUtc, SiteUtils.GetUserTimeZone(), "g", SiteUtils.GetUserTimeOffset()) + "'>" + CommentHelper.GetTimeAgo(comment.CreatedUtc, SiteUtils.GetUserTimeZone(), SiteUtils.GetUserTimeOffset(), new System.Globalization.CultureInfo("vi-VN")) + "</abbr>" + report + "</div></li>";
        }
    }
}
