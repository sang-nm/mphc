<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LoginControl.ascx.cs"
    Inherits="CanhCam.Web.UI.LoginControl" %>

<div class="panel-heading hidden">
    <gb:SiteLabel ID="lblEnterUsernamePassword" runat="server" UseLabelTag="false" />
</div>
<portal:SiteLogin ID="LoginCtrl" runat="server" CssClass="loginForm">
    <LayoutTemplate>
        <asp:Panel ID="pnlLContainer" CssClass="panel-body" runat="server" DefaultButton="Login">
            <div class="form-horizontal">
                <div class="form-group row">
                    <gb:SiteLabel ID="lblEmail" runat="server" CssClass="col-sm-4 control-label" ForControl="UserName" ConfigKey="SignInEmailLabel" />
                    <gb:SiteLabel ID="lblUserID" runat="server" CssClass="col-sm-4 control-label" ForControl="UserName" ConfigKey="ManageUsersLoginNameLabel" />
                    <div class="col-sm-4">
                        <asp:TextBox id="UserName" runat="server" CssClass="form-control" maxlength="100" />
                    </div>
                </div>
                <div class="form-group row">
                    <gb:SiteLabel ID="lblPassword" runat="server" CssClass="col-sm-4 control-label" ForControl="Password" ConfigKey="SignInPasswordLabel" />
                    <div class="col-sm-4">
                        <asp:TextBox id="Password" runat="server" CssClass="form-control" textmode="password" />
                    </div>
                </div>
                <asp:Panel class="captcha" id="divCaptcha" runat="server">
                    <telerik:RadCaptcha ID="captcha" runat="server" EnableRefreshImage="true" 
                        CaptchaTextBoxCssClass="form-control" CaptchaTextBoxLabel="" ErrorMessage="<%$Resources:Resource, CaptchaFailureMessage %>" 
                        CaptchaTextBoxTitle="<%$Resources:Resource, CaptchaInstructions %>" 
                        CaptchaLinkButtonText="<%$Resources:Resource, CaptchaRefreshImage %>" />
                </asp:Panel>
                <div class="form-group row">
                    <div class="col-sm-offset-4 col-sm-4">
                        <asp:CheckBox id="RememberMe" runat="server" />
                    </div>
                </div>
                <div class="form-group row">
                    <div class="col-sm-offset-4 col-sm-4">
                        <asp:Button CssClass="btn btn-primary" ID="Login" CommandName="Login" runat="server" Text="Login" />
                    </div>
                </div>
                <div class="form-group row">
                    <div class="col-sm-offset-4 col-sm-4">
                        <asp:Hyperlink id="lnkPasswordRecovery" runat="server" />&nbsp;
                        <asp:Hyperlink id="lnkRegisterExtraLink" runat="server" />
                    </div>
                </div>
            </div>
            <portal:gbLabel ID="FailureText" runat="server" CssClass="alert alert-danger" EnableViewState="false" />
        </asp:Panel>
    </LayoutTemplate>
</portal:SiteLogin>