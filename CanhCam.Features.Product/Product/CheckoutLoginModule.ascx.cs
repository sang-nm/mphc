/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-07-28
/// Last Modified:			2015-07-28

using System;
using System.Web.UI.WebControls;
using CanhCam.Business;
using System.Text;
using System.Web.UI;
using System.Collections;
using CanhCam.Web.Framework;
using System.Web.Security;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CanhCam.Web.ProductUI
{
    // Feature Guid: 0d5c6593-44c0-41b1-8b72-8ff4c1db482c
    public partial class CheckoutLoginModule : SiteModuleControl
    {
        private CheckoutLoginConfiguration config = null;

        private TextBox txtUserName;
        private TextBox txtPassword;
        private CheckBox chkRememberMe;
        private HyperLink lnkRecovery;
        private Panel divCaptcha = null;
        private Telerik.Web.UI.RadCaptcha captcha = null;

        private Button btnLogin;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CartHelper.GetShoppingCart(SiteId, ShoppingCartTypeEnum.ShoppingCart).Count == 0)
            {
                CartHelper.SetupRedirectToCartPage(this);
                return;
            }

            LoadSettings();

            if (Request.IsAuthenticated)
            {
                //// user is logged in
                if (config.CheckoutNextZoneId > 0 && config.CheckoutNextZoneId != CurrentZone.ZoneId)
                    WebUtils.SetupRedirect(this, CartHelper.GetZoneUrl(config.CheckoutNextZoneId));
                else
                    LoginCtrl.Visible = false;

                return;
            }

            PopulateLabels();
            PopulateControls();
        }

        private void PopulateControls()
        {
            if (siteSettings == null) { return; }
            if (siteSettings.DisableDbAuth) { this.Visible = false; return; }

            LoginCtrl.SetRedirectUrl = true;

            txtUserName = (TextBox)this.LoginCtrl.FindControl("UserName");
            txtPassword = (TextBox)this.LoginCtrl.FindControl("Password");
            chkRememberMe = (CheckBox)this.LoginCtrl.FindControl("RememberMe");
            lnkRecovery = (HyperLink)this.LoginCtrl.FindControl("lnkPasswordRecovery");
            divCaptcha = (Panel)LoginCtrl.FindControl("divCaptcha");
            captcha = (Telerik.Web.UI.RadCaptcha)LoginCtrl.FindControl("captcha");
            btnLogin = (Button)this.LoginCtrl.FindControl("Login");

            if (!siteSettings.RequireCaptchaOnLogin)
            {
                if (divCaptcha != null) { divCaptcha.Visible = false; }
                if (captcha != null) { captcha.Enabled = false; }
            }

            if (lnkRecovery.Visible)
            {
                lnkRecovery.Visible = ((siteSettings.AllowPasswordRetrieval || siteSettings.AllowPasswordReset) && (!siteSettings.UseLdapAuth ||
                                                                               (siteSettings.UseLdapAuth && siteSettings.AllowDbFallbackWithLdap)));
                lnkRecovery.NavigateUrl = this.LoginCtrl.PasswordRecoveryUrl;
            }

            if (chkRememberMe.Visible)
                chkRememberMe.Visible = siteSettings.AllowPersistentLogin;
        }

        void LoginCtrl_LoggedIn(object sender, EventArgs e)
        {
            if (config.CheckoutNextZoneId > 0)
            {
                WebUtils.SetupRedirect(this, CartHelper.GetZoneUrl(config.CheckoutNextZoneId));
                Response.End();
            }
        }

        private void PopulateLabels()
        {
            PasswordRegex.ValidationExpression = Membership.Provider.PasswordStrengthRegularExpression;

            string passwordRegexWarning = MessageTemplate.GetMessage("PasswordStrengthErrorMessage");
            if (passwordRegexWarning.Length > 0)
            {
                PasswordRegex.ErrorMessage = passwordRegexWarning;
                PasswordRegex.ToolTip = passwordRegexWarning;
            }
            else
            {
                PasswordRegex.ErrorMessage = ResourceHelper.GetResourceString("Resource", "RegisterPasswordRegexWarning");
                PasswordRegex.ToolTip = ResourceHelper.GetResourceString("Resource", "RegisterPasswordRegexWarning");
            }

            if (Membership.Provider.PasswordStrengthRegularExpression.Length == 0)
                PasswordRegex.Visible = false;
        }

        private void LoadSettings()
        {
            EnsureConfiguration();

            if (this.ModuleConfiguration != null)
            {
                this.Title = ModuleConfiguration.ModuleTitle;
                this.Description = ModuleConfiguration.FeatureName;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Load += new EventHandler(Page_Load);
            LoginCtrl.LoggedIn += new EventHandler(LoginCtrl_LoggedIn);

            PasswordRulesValidator.ServerValidate += new ServerValidateEventHandler(PasswordRulesValidator_ServerValidate);
            Register.Click += new EventHandler(Register_Click);
        }

        private void EnsureConfiguration()
        {
            if (config == null)
                config = new CheckoutLoginConfiguration(Settings);
        }

        #region Register

        void Register_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var email = Email.Text;
            var password = Password.Text;
            var userName = SiteUtils.SuggestLoginNameFromEmail(siteSettings.SiteId, email);

            SiteUser existingUser = null;

            if (SiteUser.EmailExistsInDB(siteSettings.SiteId, email))
            {
                if (WebConfigSettings.AllowNewRegistrationToActivateDeletedAccountWithSameEmail)
                {
                    existingUser = SiteUser.GetByEmail(siteSettings, email);
                    if ((existingUser != null) && (!existingUser.IsDeleted))
                        existingUser = null;
                }

                if (existingUser == null)
                {
                    RegisterResults.Text = ResourceHelper.GetResourceString("Resource", "DuplicateEmailMessage");
                    return;
                }
            }

            if (SiteUser.LoginExistsInDB(siteSettings.SiteId, userName))
            {
                RegisterResults.Text = ResourceHelper.GetResourceString("Resource", "DuplicateUserNameMessage");
                return;
            }

            if (password.Length < siteSettings.MinRequiredPasswordLength)
            {
                RegisterResults.Text = ResourceHelper.GetResourceString("ProductResources", "CheckoutRegisterInvalidPassword");
                return;
            }

            int nonAlphaNumericCharactersUsedCount = 0;

            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                {
                    nonAlphaNumericCharactersUsedCount++;
                }
            }

            if (nonAlphaNumericCharactersUsedCount < siteSettings.MinRequiredNonAlphanumericCharacters)
            {
                RegisterResults.Text = ResourceHelper.GetResourceString("ProductResources", "CheckoutRegisterInvalidPassword");
                return;
            }

            if (siteSettings.PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(password, siteSettings.PasswordStrengthRegularExpression))
                {
                    RegisterResults.Text = ResourceHelper.GetResourceString("ProductResources", "CheckoutRegisterInvalidPassword");
                    return;
                }
            }

            SiteUser siteUser;
            if (existingUser != null)
                siteUser = existingUser;
            else
                siteUser = new SiteUser(siteSettings);

            siteUser.Name = userName;
            siteUser.LoginName = userName;
            siteUser.Email = email;
            siteUser.FirstName = FullName.Text.Trim();

            var PasswordFormat = (MembershipPasswordFormat)siteSettings.PasswordFormat;
            if (PasswordFormat != MembershipPasswordFormat.Clear)
            {
                siteUser.PasswordSalt = SiteUser.CreateRandomPassword(128, WebConfigSettings.PasswordGeneratorChars);
                password = (new gbMembershipProvider()).EncodePassword(siteUser.PasswordSalt + password, PasswordFormat);
            }

            siteUser.Password = password;
            siteUser.ApprovedForLogin = !siteSettings.RequireApprovalBeforeLogin;
            bool created = siteUser.Save();

            if (existingUser != null)
                SiteUser.FlagAsNotDeleted(siteUser.UserId);

            if (created)
            {
                if (siteSettings.UseEmailForLogin)
                    FormsAuthentication.SetAuthCookie(siteUser.Email, false);
                SiteUtils.CreateAndStoreSessionToken(siteUser);
                siteUser.UpdateLastLoginTime();
            }

            if (config.CheckoutNextZoneId > 0)
                WebUtils.SetupRedirect(this, CartHelper.GetZoneUrl(config.CheckoutNextZoneId));
        }

        void PasswordRulesValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator validator = source as CustomValidator;
            validator.ErrorMessage = string.Empty;

            if (args.Value.Length < Membership.MinRequiredPasswordLength)
            {
                args.IsValid = false;
                validator.ErrorMessage
                    += ResourceHelper.GetResourceString("Resource", "RegisterPasswordMinLengthWarning")
                    + Membership.MinRequiredPasswordLength.ToString(CultureInfo.InvariantCulture) + "<br />";
            }

            if (!HasEnoughNonAlphaNumericCharacters(args.Value))
            {
                args.IsValid = false;
                validator.ErrorMessage
                    += ResourceHelper.GetResourceString("Resource", "RegisterPasswordMinNonAlphaCharsWarning")
                    + Membership.MinRequiredNonAlphanumericCharacters.ToString(CultureInfo.InvariantCulture) + "<br />";
            }
        }

        private bool HasEnoughNonAlphaNumericCharacters(string newPassword)
        {
            bool result = false;
            string alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] passwordChars = newPassword.ToCharArray();
            int nonAlphaNumericCharCount = 0;
            foreach (char c in passwordChars)
            {
                if (!alphanumeric.Contains(c.ToString()))
                {
                    nonAlphaNumericCharCount += 1;
                }
            }

            if (nonAlphaNumericCharCount >= Membership.MinRequiredNonAlphanumericCharacters)
            {
                result = true;
            }

            return result;
        }

        #endregion

    }
}

namespace CanhCam.Web.ProductUI
{
    public class CheckoutLoginConfiguration
    {
        public CheckoutLoginConfiguration()
        { }

        public CheckoutLoginConfiguration(Hashtable settings)
        {
            LoadSettings(settings);
        }

        private void LoadSettings(Hashtable settings)
        {
            if (settings == null || settings.Count == 0) { return; throw new ArgumentException("must pass in a hashtable of settings"); }

            checkoutNextZoneId = WebUtils.ParseInt32FromHashtable(settings, "CheckoutNextPageUrl", checkoutNextZoneId);
        }

        private int checkoutNextZoneId = -1;
        public int CheckoutNextZoneId
        {
            get { return checkoutNextZoneId; }
        }

    }
}