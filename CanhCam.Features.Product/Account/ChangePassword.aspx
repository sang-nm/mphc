<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/App_MasterPages/layout.Master"
    CodeBehind="ChangePassword.aspx.cs" Inherits="CanhCam.Web.AccountUI.ChangePasswordPage" %>

<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <div class="row">
        <div class="col-sm-4 col-md-3">
            <div class="account-menu">
                <ul class="noli">
                    <li><a href="/Account/UserProfile.aspx">Thông tin tài khoản</a> </li>
                    <li><a href="/Product/PurchaseHistory.aspx">Lịch sử mua hàng</a> </li>
                    <li><a class="active" href="/Account/ChangePassword.aspx">Đổi mật khẩu</a> </li>
                </ul>
            </div>
        </div>
        <div class="col-sm-8 col-md-9">
            <asp:Panel ID="pnlPassword" CssClass="wrap-secure wrap-changepass" runat="server">
                <div class="heading">
                    <portal:HeadingControl ID="heading" Text="<%$Resources:Resource, ChangePasswordLabel %>" runat="server" />
                </div>
                <asp:ChangePassword ID="ChangePassword1" runat="server">
                    <ChangePasswordTemplate>
                        <div class="form-horizontal">
                            <asp:Panel ID="divCurrentPassword" runat="server" class="settingrow form-group">
                                <gb:SiteLabel ID="lblOldPassword" runat="server" CssClass="settinglabel control-label col-sm-3" ForControl="CurrentPassword" ConfigKey="ChangePasswordCurrentPasswordLabel" />
                                <div class="col-sm-9">
                                    <asp:TextBox ID="CurrentPassword" runat="server" CssClass="form-control" TextMode="password" />
                                    <asp:RequiredFieldValidator ControlToValidate="CurrentPassword" ID="CurrentPasswordRequired"
                                        Display="Dynamic" runat="server" ValidationGroup="ChangePassword1" />
                                </div>
                            </asp:Panel>
                            <div class="settingrow form-group">
                                <gb:SiteLabel ID="lblNewPassword" runat="server" CssClass="settinglabel control-label col-sm-3" ForControl="NewPassword" ConfigKey="ChangePasswordNewPasswordLabel" />
                                <div class="col-sm-9">
                                    <telerik:RadTextBox runat="server" ID="NewPassword" CssClass="form-control" Width="100%" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" TextMode="Password" EnableSingleInputRendering="false">
                                        <PasswordStrengthSettings ShowIndicator="false"></PasswordStrengthSettings>
                                    </telerik:RadTextBox>
                                    <asp:RequiredFieldValidator ControlToValidate="NewPassword" ID="NewPasswordRequired"
                                        runat="server" Display="Dynamic" ValidationGroup="ChangePassword1" />
                                    <asp:RegularExpressionValidator ID="NewPasswordRegex" runat="server" ControlToValidate="NewPassword"
                                        Display="Dynamic" ValidationGroup="ChangePassword1" />
                                    <asp:CustomValidator ID="NewPasswordRulesValidator" runat="server" ControlToValidate="NewPassword"
                                        Display="Dynamic" ValidationGroup="ChangePassword1" />
                                </div>
                            </div>
                            <div class="settingrow form-group">
                                <gb:SiteLabel ID="lblConfirmNewPassword" runat="server" CssClass="settinglabel control-label col-sm-3" ForControl="ConfirmNewPassword" ConfigKey="ChangePasswordConfirmNewPasswordLabel" />
                                <div class="col-sm-9">
                                    <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="form-control" TextMode="password" />
                                    <asp:RequiredFieldValidator ControlToValidate="ConfirmNewPassword" ID="ConfirmNewPasswordRequired"
                                        runat="server" Display="Dynamic" ValidationGroup="ChangePassword1" />
                                    <asp:CompareValidator ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                        ID="NewPasswordCompare" runat="server" Display="Dynamic" ValidationGroup="ChangePassword1" />
                                </div>
                            </div>
                            <div class="settingrow form-group">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <asp:Button SkinID="DefaultButton" ID="ChangePasswordPushButton" CommandName="ChangePassword"
                                        Text="Change Password" runat="server" ValidationGroup="ChangePassword1" />
                                    <asp:Button SkinID="DefaultButton" ID="CancelPushButton" CommandName="Cancel" Text="Cancel"
                                        runat="server" CausesValidation="false" />
                                </div>
                            </div>
                            <div class="settingrow form-group">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <portal:gbLabel ID="FailureText" CssClass="alert alert-danger" runat="server" EnableViewState="false" />
                                </div>
                            </div>
                        </div>
                    </ChangePasswordTemplate>
                </asp:ChangePassword>
            </asp:Panel>
        </div>
    </div>
</asp:Content>