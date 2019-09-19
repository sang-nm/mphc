<%@ Page Language="c#" MaintainScrollPositionOnPostback="true" CodeBehind="UserProfile.aspx.cs"
    MasterPageFile="~/App_MasterPages/layout.Master" AutoEventWireup="false" Inherits="CanhCam.Web.AccountUI.UserProfilePage" %>

<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <div class="col-sm-4 col-md-3">
        <div class="account-menu">
            <ul class="noli">
                <li><a class="active" href="/Account/UserProfile.aspx">Thông tin tài khoản</a> </li>
                <li><a href="/Product/PurchaseHistory.aspx">Lịch sử mua hàng</a> </li>
                <li><a href="/Account/ChangePassword.aspx">Đổi mật khẩu</a> </li>
            </ul>
        </div>
    </div>
    <div class="col-sm-8 col-md-9">
        <asp:Panel ID="pnlUser" runat="server" CssClass="wrap-secure wrap-userprofile" DefaultButton="btnUpdate">
            <div class="heading">
                <portal:HeadingControl ID="heading" Text="<%$Resources:Resource, UserProfileMyProfileLabel %>" runat="server" />
            </div>
            <portal:NotifyMessage ID="message" runat="server" />
            <div id="divtabs" visible="false" runat="server" class="gb-tabs">
                <div id="divOpenID" runat="server" class="settingrow">
                    <gb:SiteLabel ID="SiteLabel4" runat="server" ForControl="OpenIdLogin1" CssClass="settinglabel"
                        ConfigKey="ManageUsersOpenIDURILabel" />
                    <div class="forminput">
                        <asp:Label ID="lblOpenID" runat="server" />
                        <asp:HyperLink ID="lnkOpenIDUpdate" runat="server" />
                        <portal:OpenIdRpxNowLink ID="rpxLink" runat="server" Embed="false" UseOverlay="true" Visible="false" />
                    </div>
                </div>
                <asp:Panel ID="pnlSecurityQuestion" runat="server">
                    <div class="settingrow">
                        <gb:SiteLabel ID="SiteLabel2" runat="server" ForControl="txtPasswordQuestion" CssClass="settinglabel"
                            ConfigKey="UsersSecurityQuestionLabel" />
                        <asp:TextBox ID="txtPasswordQuestion" runat="server" TabIndex="10" MaxLength="255"
                            CssClass="widetextbox forminput"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="txtPasswordQuestion" ID="QuestionRequired"
                            runat="server" Display="None" ValidationGroup="profile" SkinID="Profile"></asp:RequiredFieldValidator>
                    </div>
                    <div class="settingrow">
                        <gb:SiteLabel ID="SiteLabel3" runat="server" ForControl="txtPasswordAnswer" CssClass="settinglabel"
                            ConfigKey="UsersSecurityAnswerLabel" />
                        <asp:TextBox ID="txtPasswordAnswer" runat="server" TabIndex="10" MaxLength="255"
                            CssClass="widetextbox forminput"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="txtPasswordAnswer" ID="AnswerRequired"
                            runat="server" Display="None" ValidationGroup="profile" SkinID="Profile"></asp:RequiredFieldValidator>
                    </div>
                </asp:Panel>
                <div id="divEditorPreference" runat="server" visible="false" class="settingrow">
                    <gb:SiteLabel ID="SiteLabel29" runat="server" ForControl="ddEditorProviders" CssClass="settinglabel"
                        ConfigKey="SiteSettingsEditorProviderLabel" EnableViewState="false" />
                    <asp:DropDownList ID="ddEditorProviders" DataTextField="name" DataValueField="name"
                        EnableViewState="true" EnableTheming="false" TabIndex="10" runat="server" CssClass="forminput">
                    </asp:DropDownList>
                </div>
                <div class="settingrow">
                    <gb:SiteLabel ID="lblCreatedDateLabel" runat="server" CssClass="settinglabel" ConfigKey="ManageUsersCreatedDateLabel" />
                    <asp:Label ID="lblCreatedDate" runat="server" CssClass="forminput"></asp:Label>
                </div>
                <div id="divAvatar" runat="server" class="settingrow">
                    <gb:SiteLabel ID="lblAvatar" runat="server" CssClass="settinglabel" ConfigKey="UserProfileAvatarLabel" />
                    <div class="forminput">
                        <portal:Avatar ID="userAvatar" runat="server" CssClass="forminput" />
                        <asp:HyperLink ID="lnkAvatarUpld" CssClass="cp-link" runat="server" />
                        <asp:ImageButton ID="btnUpdateAvartar" runat="server" />
                    </div>
                </div>
                <div id="divLiveMessenger" runat="server" class="settingrow">
                    <gb:SiteLabel ID="SiteLabel14" runat="server" ForControl="chkEnableLiveMessengerOnProfile"
                        CssClass="settinglabel" ConfigKey="EnableLiveMessengerLabel" />
                    <div class="forminput">
                        <asp:CheckBox ID="chkEnableLiveMessengerOnProfile" runat="server" />
                        <asp:HyperLink ID="lnkAllowLiveMessenger" runat="server" Text="Enable Live Messenger" />
                    </div>
                </div>
                <div id="divTimeZone" runat="server" visible="false" class="settingrow">
                    <gb:SiteLabel ID="SiteLabel5" runat="server" CssClass="settinglabel" ConfigKey="TimeZone" />
                    <portal:TimeZoneIdSetting ID="timeZoneSetting" runat="server" />
                </div>
                <div class="settingrow">
                    <asp:HyperLink ID="lnkPubProfile" Visible="false" runat="server" CssClass="settinglabel cp-link" />
                </div>
            </div>
            <div class="form-horizontal">
                <div id="divName" runat="server" visible="false" class="settingrow form-group">
                    <gb:SiteLabel ID="lblUserName" runat="server" ForControl="txtName" CssClass="settinglabel control-label col-sm-3"
                        ConfigKey="ManageUsersUserNameLabel" />
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtName" Enabled="false" runat="server" TabIndex="10" MaxLength="100" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ValidationGroup="profile"
                            Display="none" ErrorMessage="" ControlToValidate="txtName" SkinID="Profile" />
                        <asp:RegularExpressionValidator ID="regexUserName" runat="server" ControlToValidate="txtName"
                            Display="None" ValidationExpression="" ValidationGroup="profile" Enabled="false"
                            SkinID="Profile" />
                        <asp:CustomValidator ID="FailSafeUserNameValidator" runat="server" ControlToValidate="txtName"
                            Display="None" ValidationGroup="profile" EnableClientScript="false" SkinID="Profile" />
                    </div>
                </div>
                <div id="divUserName" runat="server" visible="false" class="settingrow form-group">
                    <gb:SiteLabel ID="lblLoginName" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="ManageUsersLoginNameLabel" />
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtLoginName" Enabled="false" runat="server" TabIndex="10" MaxLength="100" CssClass="form-control" />
                    </div>
                </div>
                <div class="settingrow form-group">
                    <gb:SiteLabel ID="lblEmail" runat="server" ForControl="txtEmail" CssClass="settinglabel control-label col-sm-3"
                        ConfigKey="ManageUsersEmailLabel" />
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtEmail" Enabled="false" runat="server" TabIndex="10" CssClass="form-control" />
                        <asp:RegularExpressionValidator ID="regexEmail" runat="server" ValidationGroup="profile"
                            ErrorMessage="" ControlToValidate="txtEmail" Display="None" SkinID="Profile" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ValidationGroup="profile"
                            ErrorMessage="" ControlToValidate="txtEmail" Display="none" SkinID="Profile" />
                    </div>
                </div>
                <asp:Panel ID="pnlProfileProperties" runat="server">
                </asp:Panel>
                <div class="settingrow form-group">
                    <gb:SiteLabel ID="lblPoint" runat="server" CssClass="settinglabel control-label col-sm-3" Text="Điểm tích lũy" />
                    <div class="col-sm-9 form-control-label">
                        <asp:Literal ID="litPoint" runat="server" />
                    </div>
                </div>
                <div class="settingrow form-group">
                    <div class="col-sm-offset-3 col-sm-9">
                        <asp:Button SkinID="DefaultButton" ID="btnUpdate" runat="server" ValidationGroup="profile" />
                        <asp:HyperLink ID="lnkChangePassword" Visible="false" CssClass="changepass-link" runat="server" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
