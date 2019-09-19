/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2014-10-22
/// Last Modified:			2014-10-22

using System;
using log4net;
using CanhCam.Web.Framework;
using System.Web;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.Net;

namespace CanhCam.Web.ProductUI
{
    public static class CommentHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommentHelper));

        public static string GetTimeAgo(DateTime date, TimeZoneInfo timeZone, double timeOffset, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            if (timeZone != null)
            {
                return TimeAgo(TimeZoneInfo.ConvertTimeFromUtc(date, timeZone), culture);
            }

            return TimeAgo(date.AddHours(timeOffset), culture);
        }

        // http://stackoverflow.com/questions/2780523/time-like-facebook-style-min-ago
        private static string TimeAgo(DateTime date, CultureInfo culture)
        {
            TimeSpan timeSince = DateTime.Now.Subtract(date);

            if (timeSince.TotalSeconds < 1)
                return ResourceHelper.GetResourceString("ProductResources", "TimeAgoSecond", culture, false);
            if (timeSince.TotalSeconds < 60)
                return string.Format(ResourceHelper.GetResourceString("ProductResources", "TimeAgoSecondsFormat", culture, false), timeSince.Seconds);
            if (timeSince.TotalMinutes < 2)
                return ResourceHelper.GetResourceString("ProductResources", "TimeAgoMinute", culture, false);
            if (timeSince.TotalMinutes < 60)
                return string.Format(ResourceHelper.GetResourceString("ProductResources", "TimeAgoMinutesFormat", culture, false), timeSince.Minutes);
            if (timeSince.TotalMinutes < 120)
                return ResourceHelper.GetResourceString("ProductResources", "TimeAgoHour", culture, false);
            if (timeSince.TotalHours < 24)
                return string.Format(ResourceHelper.GetResourceString("ProductResources", "TimeAgoHoursFormat", culture, false), timeSince.Hours);
            if (timeSince.TotalDays < 2)
                return ResourceHelper.GetResourceString("ProductResources", "TimeAgoDay", culture, false);
            if (timeSince.TotalDays < 30)
                return string.Format(ResourceHelper.GetResourceString("ProductResources", "TimeAgoDaysFormat", culture, false), timeSince.Days);
            if (timeSince.TotalDays < 60)
                return ResourceHelper.GetResourceString("ProductResources", "TimeAgoMonth", culture, false);
            if (timeSince.TotalDays < 365)
                return string.Format(ResourceHelper.GetResourceString("ProductResources", "TimeAgoMonthsFormat", culture, false), Math.Round(timeSince.TotalDays / 30));
            if (timeSince.TotalDays < 730)
                return ResourceHelper.GetResourceString("ProductResources", "TimeAgoYear", culture, false);

            //last but not least...
            return string.Format(ResourceHelper.GetResourceString("ProductResources", "TimeAgoYearsFormat", culture, false), Math.Round(timeSince.TotalDays / 365));
        }

        public static void SetNameCookie(string cookieValue)
        {
            HttpCookie productUserCookie = new HttpCookie("CommentName", cookieValue);
            productUserCookie.Expires = DateTime.Now.AddMonths(1);
            HttpContext.Current.Response.Cookies.Add(productUserCookie);
        }

        public static string GetNameCookieValue()
        {
            return CookieHelper.GetCookieValue("CommentName");
        }

        public static List<int> UserLikedComments
        {
            get
            {
                List<int> lstComments = new List<int>();

                if (HttpContext.Current.Session == null
                    || HttpContext.Current.Session["UserLiked"] == null)
                    return lstComments;

                try
                {
                    lstComments = (List<int>)HttpContext.Current.Session["UserLiked"];
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }

                return lstComments;
            }
            set
            {
                HttpContext.Current.Session["UserLiked"] = value;
            }
        }

        public static string GetLikeText(int commentId)
        {
            if (UserLikedComments.Contains(commentId))
                return "Không thích";

            return "Thích";
        }

        public static void SendCommentNotification(string toEmail)
        {
            //added this due to product comment spam and need to be able to ban the ip of the spammer
            StringBuilder messageToken = new StringBuilder();
            //messageToken.Append(basePage.SiteRoot + product.Url.Replace("~", string.Empty));

            messageToken.Append("\n\nHTTP_USER_AGENT: " + HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] + "\n");
            messageToken.Append("HTTP_HOST: " + HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "\n");
            messageToken.Append("REMOTE_HOST: " + HttpContext.Current.Request.ServerVariables["REMOTE_HOST"] + "\n");
            messageToken.Append("REMOTE_ADDR: " + SiteUtils.GetIP4Address() + "\n");
            messageToken.Append("LOCAL_ADDR: " + HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"] + "\n");
            messageToken.Append("HTTP_REFERER: " + HttpContext.Current.Request.ServerVariables["HTTP_REFERER"] + "\n");

            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();

            if ((toEmail.Length > 0) && (Email.IsValidEmailAddressSyntax(toEmail)))
            {
                EmailTemplate template = EmailTemplate.Get(siteSettings.SiteId, "ProductCommentNotification");

                string fromAddress = siteSettings.DefaultEmailFromAddress;
                string fromAlias = template.FromName;
                if (fromAlias.Length == 0)
                    fromAlias = siteSettings.DefaultFromEmailAlias;
                if (template.ToAddresses.Length > 0)
                    toEmail += ";" + template.ToAddresses;

                SmtpSettings smtpSettings = SiteUtils.GetSmtpSettings();
                StringBuilder message = new StringBuilder();
                message.Append(template.HtmlBody);
                message.Replace("{SiteName}", siteSettings.SiteName);
                message.Replace("{Message}", messageToken.ToString());
                string subject = template.Subject.Replace("{SiteName}", siteSettings.SiteName);

                EmailMessageTask messageTask = new EmailMessageTask(smtpSettings);
                messageTask.SiteGuid = siteSettings.SiteGuid;
                messageTask.EmailFrom = fromAddress;
                messageTask.EmailFromAlias = fromAlias;
                messageTask.EmailReplyTo = template.ReplyToAddress;
                messageTask.EmailTo = toEmail;
                messageTask.EmailCc = template.CcAddresses;
                messageTask.EmailBcc = template.BccAddresses;
                messageTask.UseHtml = true;
                messageTask.Subject = subject;
                messageTask.HtmlBody = message.ToString();
                messageTask.QueueTask();

                WebTaskManager.StartOrResumeTasks();
            }

        }
    }
}