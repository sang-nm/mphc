using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.Business.WebHelpers.UserRegisteredHandlers;
using CanhCam.Web.Framework;
using CanhCam.Net;
using log4net;
using Resources;


namespace CanhCam.Web.ProductUI
{
    public class NotifyUserRegisteredHandler : UserRegisteredHandlerProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotifyUserRegisteredHandler));

        public NotifyUserRegisteredHandler()
        { }

        public override void UserRegisteredHandler(object sender, UserRegisteredEventArgs e)
        {
            if (e == null) return;
            if (e.SiteUser == null) return;

            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();

            log.Debug("NotifyUserRegisteredHandler called for new user " + e.SiteUser.Email);

            if (HttpContext.Current == null) { return; }

            if (siteSettings.UseSecureRegistration) return;

            EmailTemplate template = EmailTemplate.Get(siteSettings.SiteId, "RegisterEmailNotification");
            if (template == null || template.Guid == Guid.Empty) return;
            string fromEmailAlias = (template.FromName.Length > 0 ? template.FromName : siteSettings.DefaultFromEmailAlias);

            SmtpSettings smtpSettings = SiteUtils.GetSmtpSettings();

            string toEmail = e.SiteUser.Email;
            if (template.ToAddresses.Length > 0)
                toEmail += ";" + template.ToAddresses;

            EmailMessageTask messageTask = new EmailMessageTask(smtpSettings);
            messageTask.SiteGuid = siteSettings.SiteGuid;
            messageTask.EmailFrom = siteSettings.DefaultEmailFromAddress;
            messageTask.EmailFromAlias = (template.FromName.Length > 0 ? template.FromName : siteSettings.DefaultFromEmailAlias);
            messageTask.EmailReplyTo = template.ReplyToAddress;
            messageTask.EmailTo = toEmail;
            messageTask.EmailCc = template.CcAddresses;
            messageTask.EmailBcc = template.BccAddresses;
            messageTask.UseHtml = true;
            messageTask.Subject = template.Subject.Replace("{Email}", e.SiteUser.Email).Replace("{FirstName}", e.SiteUser.FirstName).Replace("{LastName}", e.SiteUser.LastName);
            messageTask.HtmlBody = template.HtmlBody.Replace("{SiteName}", siteSettings.SiteName).Replace("{Email}", e.SiteUser.Email).Replace("{FirstName}", e.SiteUser.FirstName).Replace("{LastName}", e.SiteUser.LastName).Replace("{Username}", e.SiteUser.Name).Replace("{Password}", e.SiteUser.Password);
            messageTask.QueueTask();

            WebTaskManager.StartOrResumeTasks();
        }
    }
}
