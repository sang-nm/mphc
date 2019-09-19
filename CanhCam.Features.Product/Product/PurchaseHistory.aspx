<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/App_MasterPages/layout.Master"
    CodeBehind="PurchaseHistory.aspx.cs" Inherits="CanhCam.Web.ProductUI.PurchaseHistoryPage" %>

<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <div class="row">
        <div class="col-sm-4 col-md-3">
            <div class="account-menu">
                <ul class="noli">
                    <li><a href="/Account/UserProfile.aspx">Thông tin tài khoản</a> </li>
                    <li><a class="active" href="/Product/PurchaseHistory.aspx">Lịch sử mua hàng</a> </li>
                    <li><a href="/Account/ChangePassword.aspx">Đổi mật khẩu</a> </li>
                </ul>
            </div>
        </div>
        <div class="col-sm-8 col-md-9">
            <div class="account-wrap wrap-secure purchase-history">
                <div class="heading">
                    <portal:HeadingControl ID="heading" Text="Lịch sử mua hàng" runat="server" />
                </div>
                <div class="wrap_search_history">
                    <span>xem trong</span> 
                    <asp:TextBox ID="txtDays" placeholder="ngày" runat="server" /> 
                    <asp:Button ID="btnSearch" Text="Tìm" runat="server" />
                </div>
                <div class="clear"></div>
                <asp:Xml ID="xmlTransformer" runat="server"></asp:Xml>
                <div class="clear"></div>
            </div>
        </div>
    </div>
</asp:Content>