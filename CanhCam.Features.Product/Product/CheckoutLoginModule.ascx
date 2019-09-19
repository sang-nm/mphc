<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="CheckoutLoginModule.ascx.cs"
    Inherits="CanhCam.Web.ProductUI.CheckoutLoginModule" %>
<h1>
    <gb:SiteLabel ID="lblCheckoutStepLogin" runat="server" UseLabelTag="false" ConfigKey="CheckoutStepLogin"
        ResourceFile="ProductResources" />
</h1>
<div class="row">
    <div class="col-sm-6">
        <asp:RadioButton GroupName="login" runat="server" SkinID="Checkout" ID="rdbCheckoutAsReturningCustomer"
            Text="<%$Resources:ProductResources, CheckoutAsReturningCustomer %>" />
        <portal:SiteLogin ID="LoginCtrl" runat="server">
            <LayoutTemplate>
                <asp:Panel ID="pnlLContainer" runat="server" DefaultButton="Login">
                    <portal:gbLabel ID="FailureText" runat="server" CssClass="alert alert-danger" EnableViewState="false" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="row">
                                <gb:SiteLabel ID="lblEmail" runat="server" CssClass="col-sm-3 control-label" ForControl="UserName" ResourceFile="ProductResources"
                                    ConfigKey="CheckoutAddressPhone" />
                                <div class="col-sm-9">
                                    <asp:TextBox ID="UserName" runat="server" placeholder="<%$Resources:Resource, ManageUsersLoginNameLabel %>"
                                        CssClass="form-control" MaxLength="100" />
                                    <asp:RequiredFieldValidator ControlToValidate="UserName" ID="UserNameRequired" ErrorMessage="<%$Resources:ProductResources, CheckoutAddressPhoneRequired %>"
                                        runat="server" Display="Dynamic" SetFocusOnError="true" ValidationGroup="CheckoutLogin" />
                                    <%--<asp:RegularExpressionValidator ID="EmailRegex" runat="server" ControlToValidate="UserName"
                                        ErrorMessage="<%$Resources:ProductResources, CheckoutAddressEmailInvalid %>"
                                        Display="Dynamic" SetFocusOnError="true" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@(([0-9a-zA-Z])+([-\w]*[0-9a-zA-Z])*\.)+[a-zA-Z]{2,9})$"
                                        ValidationGroup="CheckoutLogin" />--%>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <gb:SiteLabel ID="lblPassword" runat="server" CssClass="col-sm-3 control-label" ForControl="Password"
                                    ConfigKey="SignInPasswordLabel" />
                                <div class="col-sm-9">
                                    <asp:TextBox ID="Password" runat="server" placeholder="<%$Resources:Resource, SignInPasswordLabel %>"
                                        CssClass="form-control" TextMode="password" />
                                    <asp:RequiredFieldValidator ControlToValidate="Password" ID="PasswordRequired" Display="Dynamic"
                                        SetFocusOnError="true" runat="server" ValidationGroup="CheckoutLogin" />
                                </div>
                            </div>
                        </div>
                        <asp:Panel class="captcha" ID="divCaptcha" runat="server">
                            <telerik:RadCaptcha ID="captcha" runat="server" EnableRefreshImage="true" CaptchaTextBoxCssClass="form-control"
                                CaptchaTextBoxLabel="" ErrorMessage="<%$Resources:Resource, CaptchaFailureMessage %>"
                                CaptchaTextBoxTitle="<%$Resources:Resource, CaptchaInstructions %>" CaptchaLinkButtonText="<%$Resources:Resource, CaptchaRefreshImage %>" />
                        </asp:Panel>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <asp:CheckBox ID="RememberMe" Visible="false" Text="<%$Resources:Resource, SignInSendRememberMeLabel %>"
                                        runat="server" />
                                    <asp:HyperLink ID="lnkPasswordRecovery" Text="<%$Resources:Resource, ForgotPasswordLabel %>"
                                        runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <asp:Button CssClass="btn btn-primary" ID="Login" CommandName="Login" ValidationGroup="CheckoutLogin"
                                        runat="server" Text="<%$Resources:ProductResources, LoginButton %>" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </LayoutTemplate>
        </portal:SiteLogin>
        <asp:RadioButton GroupName="login" runat="server" SkinID="Checkout" Checked="true"
            ID="rdbCheckoutAsGuest" Text="<%$Resources:ProductResources, CheckoutAsGuest %>" />
        <div class="settingrow form-group">
            <div class="row">
                <gb:SiteLabel ID="lblFullName" runat="server" ForControl="FullName" CssClass="settinglabel control-label col-sm-3"
                    ResourceFile="ProductResources" ConfigKey="RegisterFullNameLabel" />
                <div class="col-sm-9">
                    <asp:TextBox ID="FullName" runat="server" TabIndex="10" MaxLength="100" CssClass="form-control" />
                    <asp:RequiredFieldValidator ControlToValidate="FullName" ID="FullNameRequired" runat="server"
                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="profile" SkinID="Registration" />
                </div>
            </div>
        </div>
        <div class="settingrow form-group">
            <div class="row">
                <gb:SiteLabel ID="lblRegisterEmail1" runat="server" ForControl="Email" CssClass="settinglabel control-label col-sm-3"
                    ConfigKey="RegisterEmailLabel" />
                <div class="col-sm-9">
                    <asp:TextBox ID="Email" runat="server" TabIndex="10" MaxLength="100" CssClass="form-control" />
                    <asp:RequiredFieldValidator ControlToValidate="Email" ID="EmailRequired" runat="server"
                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="profile" SkinID="Registration" />
                    <asp:RegularExpressionValidator ID="EmailRegex" runat="server" ControlToValidate="Email"
                        Display="Dynamic" SetFocusOnError="true" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@(([0-9a-zA-Z])+([-\w]*[0-9a-zA-Z])*\.)+[a-zA-Z]{2,9})$"
                        ValidationGroup="profile" SkinID="Registration" />
                </div>
            </div>
        </div>
        <div class="settingrow form-group">
            <div class="row">
                <gb:SiteLabel ID="lblRegisterPassword1" runat="server" ForControl="Password" CssClass="settinglabel control-label col-sm-3"
                    ConfigKey="RegisterPasswordLabel" />
                <div class="col-sm-9">
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" TabIndex="10" MaxLength="20"
                        CssClass="form-control" />
                    <asp:RequiredFieldValidator ControlToValidate="Password" ID="PasswordRequired" Display="Dynamic"
                        SetFocusOnError="true" runat="server" ValidationGroup="profile" SkinID="Registration" />
                    <asp:CustomValidator ID="PasswordRulesValidator" runat="server" ControlToValidate="Password"
                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="profile" EnableClientScript="false"
                        SkinID="Registration" />
                    <asp:RegularExpressionValidator ID="PasswordRegex" runat="server" ControlToValidate="Password"
                        Display="Dynamic" SetFocusOnError="true" ValidationGroup="profile" SkinID="Registration" />
                </div>
            </div>
        </div>
        <div class="settingrow form-group regnext">
            <div class="row">
                <div class="col-sm-offset-3 col-sm-9">
                    <asp:Button CssClass="btn btn-primary" ID="Register" CommandName="MoveNext" ValidationGroup="profile"
                        runat="server" Text="<%$Resources:ProductResources, RegisterButton %>" />
                </div>
            </div>
        </div>
        <asp:Literal EnableViewState="false" ID="RegisterResults" runat="server" />
    </div>
    <div class="col-sm-6">
        <div class="formLoginSocial">
            <h2>
                <gb:SiteLabel ID="lblCheckoutLoginWithOpenID" runat="server" UseLabelTag="false"
                    ConfigKey="CheckoutLoginWithOpenID" ResourceFile="ProductResources" />
            </h2>
            <a class="loginFb" href="javascript:void(0)" data-url="/Account/LoginFacebook.aspx">
                <gb:SiteLabel ID="lblCheckoutLoginWithFacebook" runat="server" UseLabelTag="false"
                    ConfigKey="CheckoutLoginWithFacebook" ResourceFile="ProductResources" />
            </a>
            <div id="gSignInWrapper">
                <div id="customBtn" class="customGPlusSignIn">
                    <a class="loginGg" href="javascript:void(0);" data-url="/Account/LoginGooglePlus.aspx">
                        <gb:SiteLabel ID="lblCheckoutLoginWithGoogle" runat="server" UseLabelTag="false" ConfigKey="CheckoutLoginWithGoogle" ResourceFile="ProductResources" />
                    </a>
                </div>
            </div>
            <div id="name"></div>
        </div>
    </div>
</div>
