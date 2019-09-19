<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="BirthdaySetting.ascx.cs" Inherits="CanhCam.Web.AccountUI.BirthdaySetting" %>

<asp:UpdatePanel ID="up" runat="server">
    <ContentTemplate>
        <div class="row">
            <div class="year col-xs-12 col-md-3 col-lg-2">
                <asp:DropDownList ID="ddYear" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="ddYear_SelectedIndexChanged" AutoPostBack="True">
                </asp:DropDownList>
            </div>
            <div class="month col-xs-12 col-md-3 col-lg-2">
                <asp:DropDownList ID="ddMonth" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="ddMonth_SelectedIndexChanged" AutoPostBack="True">
                </asp:DropDownList>
            </div>
            <div class="day col-xs-12 col-md-3 col-lg-2">
                <asp:DropDownList ID="ddDay" AppendDataBoundItems="true" runat="server">
                </asp:DropDownList>
                <%--<asp:RequiredFieldValidator ControlToValidate="ddDay" Visible="false" InitialValue="0"
                        ValidationGroup="profile" SkinID="Registration"
                        ID="reqDay" Display="Dynamic" SetFocusOnError="true" runat="server" />--%>
            </div>
        </div>
     </ContentTemplate>   
</asp:UpdatePanel>