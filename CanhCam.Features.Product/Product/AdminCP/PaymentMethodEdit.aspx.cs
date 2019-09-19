/// Author:					Tran Quoc Vuong - itqvuong@gmail.com
/// Created:				2015-08-18
/// Last Modified:			2015-08-18

using CanhCam.Business;
using CanhCam.Web.Framework;
using Resources;
using System;
using log4net;
using System.Web.UI.WebControls;
using CanhCam.Business.WebHelpers;
using CanhCam.Web.Editor;

namespace CanhCam.Web.ProductUI
{
    public partial class PaymentMethodEditPage : CmsNonBasePage
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(PaymentMethodEditPage));

        private int paymentMethodId = -1;
        private PaymentMethod method = null;

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);

            this.btnUpdate.Click += new EventHandler(btnUpdate_Click);
            this.btnUpdateAndNew.Click += new EventHandler(btnUpdateAndNew_Click);
            this.btnUpdateAndClose.Click += new EventHandler(btnUpdateAndClose_Click);
            this.btnInsert.Click += new EventHandler(btnInsert_Click);
            this.btnInsertAndNew.Click += new EventHandler(btnInsertAndNew_Click);
            this.btnInsertAndClose.Click += new EventHandler(btnInsertAndClose_Click);
            this.btnDelete.Click += new EventHandler(btnDelete_Click);
            this.ddlPaymentProvider.SelectedIndexChanged += new EventHandler(ddlPaymentProvider_SelectedIndexChanged);
            this.chkFreeOnOrdersOverXEnabled.CheckedChanged += new EventHandler(chkFreeOnOrdersOverXEnabled_CheckedChanged);

            SiteUtils.SetupEditor(edDescription, AllowSkinOverride, Page);
        }

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                SiteUtils.RedirectToLoginPage(this);
                return;
            }

            SecurityHelper.DisableBrowserCache();

            LoadSettings();
            PopulateLabels();

            if (!WebUser.IsAdmin)
            {
                SiteUtils.RedirectToEditAccessDeniedPage();
                return;
            }

            if (!Page.IsPostBack)
                PopulateControls();
        }

        private void PopulateControls()
        {
            BindPaymentProvider();

            bool loadAllTab = false;
            if (method != null && method.PaymentMethodId > 0)
            {
                txtName.Text = method.Name;
                edDescription.Text = method.Description;
                chkIsActive.Checked = method.IsActive;
                txtAdditionalFee.Text = ProductHelper.FormatPrice(method.AdditionalFee);

                ListItem liItem = ddlUsePercentage.Items.FindByValue(method.UsePercentage.ToString().ToLower());
                if (liItem != null)
                {
                    ddlUsePercentage.ClearSelection();
                    liItem.Selected = true;
                }

                chkFreeOnOrdersOverXEnabled.Checked = method.FreeOnOrdersOverXEnabled;
                txtFreeOnOrdersOverXValue.Text = ProductHelper.FormatPrice(method.FreeOnOrdersOverXValue);

                liItem = ddlPaymentProvider.Items.FindByValue(method.PaymentProvider.ToString());
                if (liItem != null)
                {
                    ddlPaymentProvider.ClearSelection();
                    liItem.Selected = true;
                    ddlPaymentProvider.Enabled = false;
                }

                loadAllTab = true;
            }

            divFreeOnOrdersOverXValue.Visible = chkFreeOnOrdersOverXEnabled.Checked;
            LanguageHelper.PopulateTab(tabLanguage, loadAllTab);
        }

        private void BindPaymentProvider()
        {
            ddlPaymentProvider.Items.Clear();
            foreach (var item in typeof(PaymentMethodProvider).GetFields())
            {
                if (item.FieldType == typeof(PaymentMethodProvider))
                    ddlPaymentProvider.Items.Add(new ListItem { Text = ResourceHelper.GetResourceString("ProductResources", "PaymentProvider" + item.Name), Value = item.GetRawConstantValue().ToString() });
            }
        }

        void btnInsert_Click(object sender, EventArgs e)
        {
            int itemId = SaveData();
            if (itemId > 0)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethodEdit.aspx?id=" + itemId.ToString());
            }
        }

        void btnInsertAndClose_Click(object sender, EventArgs e)
        {
            int itemId = SaveData();
            if (itemId > 0)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethods.aspx");
            }
        }

        void btnInsertAndNew_Click(object sender, EventArgs e)
        {
            int itemId = SaveData();
            if (itemId > 0)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethodEdit.aspx");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int itemId = SaveData();
            if (itemId > 0)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethodEdit.aspx?id=" + itemId.ToString());
            }
        }

        void btnUpdateAndClose_Click(object sender, EventArgs e)
        {
            int itemId = SaveData();
            if (itemId > 0)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethods.aspx");
            }
        }

        void btnUpdateAndNew_Click(object sender, EventArgs e)
        {
            int itemId = SaveData();
            if (itemId > 0)
            {
                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethodEdit.aspx");
            }
        }

        private int SaveData()
        {
            if (!Page.IsValid) return -1;

            try
            {
                if (method == null || method.PaymentMethodId == -1)
                {
                    method = new PaymentMethod();
                    method.SiteId = siteSettings.SiteId;
                }

                if (!IsLanguageTab())
                {
                    method.Name = txtName.Text.Trim();
                    method.Description = edDescription.Text.Trim();
                }

                method.PaymentProvider = Convert.ToInt32(ddlPaymentProvider.SelectedValue);
                method.IsActive = chkIsActive.Checked;

                decimal additionalFee = 0;
                decimal.TryParse(txtAdditionalFee.Text, out additionalFee);
                method.AdditionalFee = additionalFee;
                method.UsePercentage = Convert.ToBoolean(ddlUsePercentage.SelectedValue);

                method.FreeOnOrdersOverXEnabled = chkFreeOnOrdersOverXEnabled.Checked;
                decimal freeOnOrdersOverXValue = 0;
                decimal.TryParse(txtFreeOnOrdersOverXValue.Text, out freeOnOrdersOverXValue);
                method.FreeOnOrdersOverXValue = freeOnOrdersOverXValue;

                if (method.Save())
                    SaveContentLanguage(method.Guid);

                if (paymentMethodId > 0)
                {
                    LogActivity.Write("Update payment method", method.Name);
                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "UpdateSuccessMessage");
                }
                else
                {
                    LogActivity.Write("Create new payment method", method.Name);
                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "InsertSuccessMessage");
                }

                return method.PaymentMethodId;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return -1;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (method != null && method.PaymentMethodId > -1)
                {
                    ContentDeleted.Create(siteSettings.SiteId, method.PaymentMethodId.ToString(), "PaymentMethod", typeof(PaymentMethodDeleted).AssemblyQualifiedName, method.PaymentMethodId.ToString(), Page.User.Identity.Name);

                    method.IsDeleted = true;
                    method.Save();

                    LogActivity.Write("Delete payment method", method.Name);

                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "DeleteSuccessMessage");
                }

                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/PaymentMethods.aspx");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void ddlPaymentProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        void chkFreeOnOrdersOverXEnabled_CheckedChanged(object sender, EventArgs e)
        {
            divFreeOnOrdersOverXValue.Visible = chkFreeOnOrdersOverXEnabled.Checked;
        }

        #region Language

        protected void tabLanguage_TabClick(object sender, Telerik.Web.UI.RadTabStripEventArgs e)
        {
            txtName.Text = string.Empty;
            edDescription.Text = string.Empty;
            btnDeleteLanguage.Visible = false;

            if (method != null && method.PaymentMethodId > 0)
            {
                if (e.Tab.Index == 0)
                {
                    txtName.Text = method.Name;
                    edDescription.Text = method.Description;
                }
                else
                {
                    ContentLanguage content = new ContentLanguage(method.Guid, Convert.ToInt32(e.Tab.Value));
                    if (content != null && content.Guid != Guid.Empty)
                    {
                        txtName.Text = content.Title;
                        edDescription.Text = content.MetaTitle;

                        btnDeleteLanguage.Visible = true;
                    }
                }
            }

            upButton.Update();
        }

        private bool IsLanguageTab()
        {
            if (tabLanguage.Visible && tabLanguage.SelectedIndex > 0)
                return true;

            return false;
        }

        private void SaveContentLanguage(Guid contentGuid)
        {
            if (contentGuid == Guid.Empty || !IsLanguageTab())
                return;

            int languageID = -1;
            if (tabLanguage.SelectedIndex > 0)
                languageID = Convert.ToInt32(tabLanguage.SelectedTab.Value);

            if (languageID == -1)
                return;

            var content = new ContentLanguage(contentGuid, languageID);

            if (txtName.Text.Length > 0)
            {
                content.LanguageId = languageID;
                content.ContentGuid = contentGuid;
                content.SiteGuid = siteSettings.SiteGuid;
                content.Title = txtName.Text.Trim();

                content.Save();
            }
        }

        protected void btnDeleteLanguage_Click(object sender, EventArgs e)
        {
            if (!IsLanguageTab())
                return;

            if (tabLanguage.SelectedIndex > 0)
            {
                int languageId = Convert.ToInt32(tabLanguage.SelectedTab.Value);

                if (languageId > 0 && method != null && method.Guid != Guid.Empty)
                {
                    ContentLanguage.Delete(method.Guid, languageId);
                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "DeleteSuccessMessage");

                    WebUtils.SetupRedirect(this, Request.RawUrl);
                }
            }
        }

        #endregion

        private void PopulateLabels()
        {
            heading.Text = ProductResources.PaymentMethodEditTitle;
            Title = SiteUtils.FormatPageTitle(siteSettings, heading.Text);

            UIHelper.AddConfirmationDialog(btnDelete, ResourceHelper.GetResourceString("Resource", "DeleteConfirmMessage"));
            UIHelper.AddConfirmationDialog(btnDeleteLanguage, ResourceHelper.GetResourceString("Resource", "DeleteConfirmMessage"));
            
            edDescription.WebEditor.ToolBar = ToolBar.FullWithTemplates;
            edDescription.WebEditor.Height = Unit.Pixel(300);
        }

        private void LoadSettings()
        {
            paymentMethodId = WebUtils.ParseInt32FromQueryString("id", paymentMethodId);

            if (paymentMethodId > 0)
            {
                method = new PaymentMethod(paymentMethodId);
                if (
                    method != null
                    && method.PaymentMethodId > 0
                    && method.SiteId == siteSettings.SiteId
                    ) ;
                else
                    method = null;
            }

            HideControls();

            AddClassToBody("paymentmethod-edit");
        }

        private void HideControls()
        {
            divAdditionalFee.Visible = false;

            btnInsert.Visible = false;
            btnInsertAndNew.Visible = false;
            btnInsertAndClose.Visible = false;
            btnUpdate.Visible = false;
            btnUpdateAndNew.Visible = false;
            btnUpdateAndClose.Visible = false;
            btnDelete.Visible = false;

            if (method == null)
            {
                btnInsert.Visible = true;
                btnInsertAndNew.Visible = true;
                btnInsertAndClose.Visible = true;
            }
            else if (method != null && method.PaymentMethodId > 0)
            {
                btnUpdate.Visible = true;
                btnUpdateAndNew.Visible = true;
                btnUpdateAndClose.Visible = true;

                btnDelete.Visible = true;
            }
        }

    }
}