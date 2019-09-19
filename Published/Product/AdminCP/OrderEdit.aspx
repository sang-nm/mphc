<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/App_MasterPages/layout.Master"
    CodeBehind="OrderEdit.aspx.cs" Inherits="CanhCam.Web.ProductUI.OrderEditPage" %>

<%@ Register TagPrefix="Site" Assembly="CanhCam.Features.Product" Namespace="CanhCam.Web.ProductUI" %>
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <Site:ProductDisplaySettings ID="displaySettings" runat="server" />
    <portal:BreadcrumbAdmin ID="breadcrumb" runat="server"
        ParentTitle="<%$Resources:ProductResources, OrderAdminTitle %>" ParentUrl="~/Product/AdminCP/OrderList.aspx"
        CurrentPageTitle="<%$Resources:ProductResources, OrderDetailTitle %>" CurrentPageUrl="~/Product/AdminCP/OrderEdit.aspx" />
    <div class="admin-content col-md-12">
        <portal:HeadingPanel ID="heading" runat="server">
            <asp:Button ID="btnUpdate" SkinID="UpdateButton" Text="<%$Resources:ProductResources, ProductUpdateButton %>" runat="server" />
            <asp:Button ID="btnDelete" SkinID="DeleteButton" Text="<%$Resources:Resource, DeleteButton %>" runat="server" CausesValidation="false" />
        </portal:HeadingPanel>
        <portal:NotifyMessage ID="message" runat="server" />
        <div class="workplace">
            <div class="row">
                <div class="col-md-6">
                    <h3>
                        <gb:SiteLabel ID="lblCustomerInfo" runat="server" ConfigKey="OrderCustomerInfo" ResourceFile="ProductResources" UseLabelTag="false" />
                    </h3>
                    <div class="form-horizontal">
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblBillingFirstName" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="RegisterFullNameLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtBillingFirstName" runat="server" />
                                <asp:Literal ID="litBillingFirstName" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div id="divBillingLastName" runat="server" visible="false" class="settingrow form-group">
                            <gb:SiteLabel ID="lblBillingLastName" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderLastNameLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtBillingLastName" runat="server" />
                                <asp:Literal ID="litBillingLastName" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblBillingEmail" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderEmailLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtBillingEmail" runat="server" />
                                <asp:Literal ID="litBillingEmail" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblBillingPhone" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderPhoneLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtBillingPhone" runat="server" />
                                <asp:Literal ID="litBillingPhone" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblBillingAddress" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderAddressLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtBillingAddress" runat="server" />
                                <asp:Literal ID="litBillingAddress" Visible="false" runat="server" />
                            </div>
                        </div>
                        <asp:UpdatePanel ID="upBilling" runat="server">
                            <ContentTemplate>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblBillingProvince" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderProvinceLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-9">
                                        <asp:DropDownList ID="ddBillingProvince" AutoPostBack="true" AppendDataBoundItems="true" DataValueField="Guid" DataTextField="Name" runat="server">
                                            <asp:ListItem Text="<%$Resources:ProductResources, OrderSelectLabel %>" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Literal ID="litBillingProvince" Visible="false" runat="server" />
                                    </div>
                                </div>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblBillingDistrict" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderDistrictLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-9">
                                        <asp:DropDownList ID="ddBillingDistrict" DataValueField="Guid" DataTextField="Name" runat="server">
                                            <asp:ListItem Text="<%$Resources:ProductResources, OrderSelectLabel %>" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Literal ID="litBillingDistrict" Visible="false" runat="server" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="divUsers" visible="false" runat="server">
                            <div class="settingrow form-group">
                                <gb:SiteLabel ID="SiteLabel1" runat="server" CssClass="settinglabel control-label col-sm-3" Text="Mã thành viên" />
                                <div class="col-sm-9">
                                    <p class="form-control-static">
                                        <asp:Literal ID="litUserCode" runat="server" />
                                    </p>
                                </div>
                            </div>
                            <div class="settingrow form-group">
                                <gb:SiteLabel ID="lblUserEmail" runat="server" CssClass="settinglabel control-label col-sm-3" Text="Email" />
                                <div class="col-sm-9">
                                    <p class="form-control-static">
                                        <asp:Literal ID="litUserEmail" runat="server" />
                                    </p>
                                </div>
                            </div>
                            <div class="settingrow form-group">
                                <gb:SiteLabel ID="lblUserPoints" runat="server" CssClass="settinglabel control-label col-sm-3" Text="Điểm thành viên" />
                                <div class="col-sm-9">
                                    <p class="form-control-static">
                                        <asp:Literal ID="litUserPoints" runat="server" />
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="divShippingAddress" runat="server" visible="false" class="col-md-4">
                    <h3>
                        <gb:SiteLabel ID="lblConsigneeInfo" runat="server" ConfigKey="OrderConsigneeInfo" ResourceFile="ProductResources" UseLabelTag="false" />
                    </h3>
                    <div class="form-horizontal">
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblShippingFirstName" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderFirstNameLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtShippingFirstName" runat="server" />
                                <asp:Literal ID="litShippingFirstName" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblShippingLastName" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderLastNameLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtShippingLastName" runat="server" />
                                <asp:Literal ID="litShippingLastName" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblShippingEmail" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderEmailLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtShippingEmail" runat="server" />
                                <asp:Literal ID="litShippingEmail" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblShippingPhone" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderPhoneLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtShippingPhone" runat="server" />
                                <asp:Literal ID="litShippingPhone" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblShippingAddress" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderAddressLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-9">
                                <asp:TextBox ID="txtShippingAddress" runat="server" />
                                <asp:Literal ID="litShippingAddress" Visible="false" runat="server" />
                            </div>
                        </div>
                        <asp:UpdatePanel ID="upShipping" runat="server">
                            <ContentTemplate>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblShippingProvince" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderProvinceLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-9">
                                        <asp:DropDownList ID="ddShippingProvince" AutoPostBack="true" AppendDataBoundItems="true" DataValueField="Guid" DataTextField="Name" runat="server">
                                            <asp:ListItem Text="<%$Resources:ProductResources, OrderSelectLabel %>" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Literal ID="litShippingProvince" Visible="false" runat="server" />
                                    </div>
                                </div>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblShippingDistrict" runat="server" CssClass="settinglabel control-label col-sm-3" ConfigKey="OrderDistrictLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-9">
                                        <asp:DropDownList ID="ddShippingDistrict" AutoPostBack="true" DataValueField="Guid" DataTextField="Name" runat="server">
                                            <asp:ListItem Text="<%$Resources:ProductResources, OrderSelectLabel %>" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Literal ID="litShippingDistrict" Visible="false" runat="server" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="col-md-6">
                    <h3>
                        <gb:SiteLabel ID="lblOrderInfo" runat="server" ConfigKey="OrderInfoLabel" ResourceFile="ProductResources" UseLabelTag="false" />
                    </h3>
                    <div class="form-horizontal">
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblOrderStatus" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderStatusLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-7">
                                <asp:DropDownList ID="ddOrderStatus" runat="server" />
                                <asp:Label ID="litOrderStatus" Visible="false" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblOrderNote" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderNoteLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-7">
                                <asp:TextBox ID="txtOrderNote" TextMode="MultiLine" Style="min-height: 50px" runat="server" />
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblCreatedOn" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderCreatedOnLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-7">
                                <p class="form-control-static">
                                    <asp:Literal ID="litCreatedOn" runat="server" />
                                </p>
                            </div>
                        </div>
                        <div class="settingrow form-group">
                            <gb:SiteLabel ID="lblOrderCode" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderCodeLabel" ResourceFile="ProductResources" />
                            <div class="col-sm-7">
                                <p class="form-control-static">
                                    <asp:Literal ID="litOrderCode" runat="server" />
                                </p>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="upMethod" runat="server">
                            <ContentTemplate>
                                <div id="divShippingMethod" runat="server" class="settingrow form-group">
                                    <gb:SiteLabel ID="lblShippingMethod" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="ShippingMethodLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-7">
                                        <asp:RadioButtonList ID="rdbListShippingMethod" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="nav" DataTextField="Name" DataValueField="ShippingMethodId" runat="server" />
                                    </div>
                                </div>
                                <div id="divPaymentMethod" runat="server" class="settingrow form-group">
                                    <gb:SiteLabel ID="lblPaymentMethod" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="PaymentMethodLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-7">
                                        <asp:RadioButtonList ID="rdbListPaymentMethod" RepeatLayout="UnorderedList" CssClass="nav" DataTextField="Name" DataValueField="PaymentMethodId" runat="server" />
                                    </div>
                                </div>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblSubTotal" runat="server" CssClass="settinglabel control-label col-sm-5" Text="Tổng tiền hàng" />
                                    <div class="col-sm-7">
                                        <p class="form-control-static">
                                            <asp:Literal ID="litSubTotal" runat="server" />
                                        </p>
                                    </div>
                                </div>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblUserPointDiscount" runat="server" CssClass="settinglabel control-label col-sm-5" Text="Điểm tích lũy" />
                                    <div class="col-sm-7">
                                        <p class="form-control-static">
                                            <asp:Literal ID="litUserPointDiscount" runat="server" />
                                        </p>
                                    </div>
                                </div>
                                <%--<div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblOrderTax" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderTaxLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-7">
                                        <asp:TextBox ID="txtOrderTax" AutoPostBack="true" SkinID="PriceTextBox" style="display:inline-block" MaxLength="50" runat="server" />
                                        <asp:TextBox ID="txtOrderTaxPercentage" AutoPostBack="true" SkinID="PriceTextBox" style="display:inline-block" placeholder="%" MaxLength="5" runat="server" />
                                    </div>
                                </div>--%>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblOrderShipping" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderShippingLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-7">
                                        <asp:TextBox ID="txtOrderShipping" AutoPostBack="true" SkinID="PriceTextBox" Style="display: inline-block" MaxLength="50" runat="server" />
                                        <asp:TextBox ID="txtOrderShippingPercentage" AutoPostBack="true" SkinID="PriceTextBox" Style="display: inline-block" placeholder="%" MaxLength="5" runat="server" />
                                    </div>
                                </div>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblOrderDiscount" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderDiscountLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-7">
                                        <asp:TextBox ID="txtOrderDiscount" AutoPostBack="true" SkinID="PriceTextBox" Style="display: inline-block" MaxLength="50" runat="server" />
                                        <asp:TextBox ID="txtOrderDiscountPercentage" AutoPostBack="true" SkinID="PriceTextBox" Style="display: inline-block" placeholder="%" MaxLength="5" runat="server" />
                                    </div>
                                </div>
                                <div class="settingrow form-group">
                                    <gb:SiteLabel ID="lblOrderTotal" runat="server" CssClass="settinglabel control-label col-sm-5" ConfigKey="OrderTotalLabel" ResourceFile="ProductResources" />
                                    <div class="col-sm-7">
                                        <p class="form-control-static">
                                            <b>
                                                <asp:Literal ID="litOrderTotal" runat="server" />
                                            </b>
                                        </p>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="settingrow form-group">
                            <div class="col-sm-7 col-sm-offset-5">
                                <asp:Button ID="btnConfirm" CssClass="btn btn-primary" runat="server" Text="<%$Resources:ProductResources, OrderConfirmButton %>" />
                                <%--<asp:Button ID="Button1" CssClass="btn btn-primary" runat="server" Text="Minh Sang test word" />--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <h3>
                <gb:SiteLabel ID="lblProductList" runat="server" ConfigKey="ProductListTitle" ResourceFile="ProductResources" UseLabelTag="false" />
            </h3>
            <telerik:RadGrid ID="grid" SkinID="radGridSkin" runat="server">
                <MasterTableView DataKeyNames="Guid,ProductId,AttributeDescription,AttributesXml,Quantity,DiscountAmount" AllowPaging="false" AllowSorting="false">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="<%$Resources:ProductResources, ProductNameLabel %>" UniqueName="ProductName">
                            <ItemTemplate>
                                <asp:Literal ID="litProductCode" runat="server" />
                                <div>
                                    <asp:Literal ID="litProductName" runat="server" />
                                </div>
                                <asp:Literal ID="litAttributes" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="<%$Resources:ProductResources, OrderQuantityLabel %>" UniqueName="Quantity">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQuantity" SkinID="NumericTextBox" Visible="false"
                                    MaxLength="4" Text='<%# Eval("Quantity") %>' runat="server" />
                                <%# Eval("Quantity") %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="<%$Resources:ProductResources, OrderPriceLabel %>" UniqueName="OrderPrice">
                            <ItemTemplate>
                                <%#CanhCam.Web.ProductUI.ProductHelper.FormatPrice(Convert.ToDecimal(Eval("Price")), true)%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Visible="false" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="Khuyến mãi" UniqueName="DiscountAmount">
                            <ItemTemplate>
                                <%#CanhCam.Web.ProductUI.ProductHelper.FormatPrice(Convert.ToDecimal(Eval("DiscountAmount")), true)%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="<%$Resources:ProductResources, OrderTotalPriceLabel %>" UniqueName="OrderTotalPrice">
                            <ItemTemplate>
                                <%#CanhCam.Web.ProductUI.ProductHelper.FormatPrice(Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Quantity")) - Convert.ToDecimal(Eval("DiscountAmount")), true) %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Visible="false" UniqueName="Delete">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDelete" CssClass="cp-link" CommandName="Delete" CommandArgument='<%#Eval("Guid").ToString()%>' Text="Hủy" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </div>
    <style type="text/css">
        .attributes {
            font-size: 12px;
            font-style: italic;
        }
    </style>
</asp:Content>
