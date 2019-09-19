<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/App_MasterPages/layout.Master"
    CodeBehind="OrderList.aspx.cs" Inherits="CanhCam.Web.ProductUI.OrderListPage" %>

<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <portal:BreadcrumbAdmin ID="breadcrumb" runat="server"
        CurrentPageTitle="<%$Resources:ProductResources, OrderAdminTitle %>" CurrentPageUrl="~/Product/AdminCP/OrderList.aspx" />
    <div class="admin-content col-md-12">
        <portal:HeadingPanel ID="heading" runat="server">
            <asp:Button SkinID="ExportButton" ID="btnExport" OnClick="btnExport_Click" Text="Xuất excel Hóa đơn" runat="server" CausesValidation="false" />
            <asp:Button SkinID="ExportButton" ID="btnExportCustomers" OnClick="btnExportCustomers_Click" Text="Xuất excel Khách hàng" runat="server" CausesValidation="false" />
            <asp:Button SkinID="ExportButton" ID="btnExportByProduct" Visible="false" OnClick="btnExportByProduct_Click" Text="<%$Resources:ProductResources, OrderExportByProductButton %>" runat="server" CausesValidation="false" />
            <asp:Button SkinID="DeleteButton" ID="btnDelete" Text="<%$Resources:Resource, DeleteSelectedButton %>" runat="server" CausesValidation="false" />
        </portal:HeadingPanel>
        <portal:NotifyMessage ID="message" runat="server" />
        <asp:Panel ID="pnlSearch" DefaultButton="btnSearch" CssClass="headInfo form-horizontal" runat="server">
            <div class="settingrow form-group">
                <gb:SiteLabel ID="lblFromDate" runat="server" ConfigKey="OrderDateFromLabel"
                    ResourceFile="ProductResources" ForControl="dpFromDate" CssClass="settinglabel control-label col-sm-3" />
                <div class="col-sm-9">
                    <div class="form-inline">
                        <gb:DatePickerControl ID="dpFromDate" runat="server" />
                        <gb:SiteLabel ID="lblEndDate" runat="server" ConfigKey="OrderDateToLabel" ResourceFile="ProductResources"
                            ForControl="dpToDate" CssClass="settinglabel control-label" />
                        <gb:DatePickerControl ID="dpToDate" runat="server" />
                    </div>
                </div>
            </div>
            <div class="settingrow form-group">
                <gb:SiteLabel ID="lblOrderStatus" runat="server" ConfigKey="OrderStatusLabel" ResourceFile="ProductResources"
                    ForControl="ddOrderStatus" CssClass="settinglabel control-label col-sm-3" />
                <div class="col-sm-9">
                    <asp:DropDownList ID="ddOrderStatus" runat="server" />
                </div>
            </div>
            <div class="settingrow form-group">
                <gb:SiteLabel ID="lblKeyword" runat="server" ConfigKey="OrderKeywordLabel"
                    ResourceFile="ProductResources" ForControl="txtKeyword" CssClass="settinglabel control-label col-sm-3" />
                <div class="col-sm-9">
                    <asp:TextBox ID="txtKeyword" placeholder="<%$Resources:ProductResources, OrderKeywordTip %>" runat="server" MaxLength="255" />
                </div>
            </div>
            <div class="settingrow form-group">
                <div class="col-sm-offset-3 col-sm-9">
                    <asp:Button SkinID="DefaultButton" ID="btnSearch" Text="<%$Resources:ProductResources, OrderSearchButton %>"
                        runat="server" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
        <div class="workplace">
            <telerik:RadGrid ID="grid" SkinID="radGridSkin" runat="server">
                <MasterTableView DataKeyNames="OrderId,OrderStatus" AllowSorting="false">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderStyle-Width="35" HeaderText="<%$Resources:Resource, RowNumber %>">
                            <ItemTemplate>
                                <%# (grid.PageSize * grid.CurrentPageIndex) + Container.ItemIndex + 1%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="<%$Resources:ProductResources, OrderCodeLabel %>" DataField="OrderCode" />
                        <telerik:GridTemplateColumn HeaderText="<%$Resources:ProductResources, OrderCustomerLabel %>">
                            <ItemTemplate>
                                <%# GetCustomer(Eval("BillingFirstName").ToString(), Eval("BillingLastName").ToString()) %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="<%$Resources:ProductResources, OrderCreatedDateLabel %>">
                            <ItemTemplate>
                                <%# DateTimeHelper.Format(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, Resources.ProductResources.OrderCreatedDateFormat, timeOffset)%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="<%$Resources:ProductResources, OrderStatusLabel %>">
                            <ItemTemplate>
                                <asp:Label ID="lblOrderStatus" ForeColor='<%#GetForeColor(Convert.ToInt32(Eval("OrderStatus")))%>' Text='<%# CanhCam.Web.ProductUI.ProductHelper.GetOrderStatus(Convert.ToInt32(Eval("OrderStatus"))) %>' runat="server" />
                                <asp:DropDownList ID="ddOrderStatus" OnSelectedIndexChanged="ddOrderStatus_SelectedIndexChanged" AutoPostBack="false" Visible="false" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="<%$Resources:ProductResources, OrderTotalLabel %>">
                            <ItemTemplate>
                                <%# CanhCam.Web.ProductUI.ProductHelper.FormatPrice(Convert.ToDecimal(Eval("OrderTotal")), true)%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:HyperLink CssClass="cp-link" ID="lnkQuickView" NavigateUrl="#" runat="server" 
                                    Text="<%$Resources:ProductResources, OrderQuickViewLink %>">
                                </asp:HyperLink>
                                <asp:HyperLink CssClass="cp-link" ID="EditLink" runat="server" 
                                    Text="<%$Resources:ProductResources, OrderDetailLink %>" NavigateUrl='<%# SiteRoot + "/Product/AdminCP/OrderEdit.aspx?OrderID=" + Eval("OrderId") %>'>
                                </asp:HyperLink>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridClientSelectColumn HeaderStyle-Width="35" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <telerik:RadToolTipManager ID="RadToolTipManager1" runat="server" Position="BottomCenter"
                Width="960" RelativeTo="Element" OnAjaxUpdate="OnAjaxUpdate" HideEvent="ManualClose" ShowEvent="OnClick" EnableDataCaching="true">
            </telerik:RadToolTipManager>
        </div>
    </div>
    <style type="text/css">
        .RadToolTip td.rtWrapperContent{background: none repeat scroll 0 0 #dbf4cd !important;border: 1px solid #dbf4cd !important;}
        .OrderPopup {color: #4e4f4f;font-family: Arial !important;margin: 10px 0;width: 100%;}
        .OrderPopup .BillingDetails, .OrderPopup .ShippingDetails {width: 50%;}
        .OrderPopup .OrderProducts {width: 50%;}
        .OrderPopup .Key, .OrderPopup .Value, .OrderPopup .OrderProducts .Key, .OrderPopup .OrderProducts .Price, .OrderPopup .OrderProducts td {background: none repeat scroll 0 0 #dbf4cd !important;border: medium none !important;height: auto !important;padding: 4px !important;}
        .OrderPopup .BillingDetails .Key, .OrderPopup .ShippingDetails .Key {background: none repeat scroll 0 0 #dbf4cd !important;border: medium none;vertical-align: top;width: 130px;}
        .OrderPopup .BillingDetails .Value, .OrderPopup .ShippingDetails .Value {background: none repeat scroll 0 0 #dbf4cd !important;border: medium none;vertical-align: top;}
        .OrderPopup .Seperator {background: #9fce8c none repeat scroll 0 0 !important;border: medium none !important;padding: 1px;}
        .OrderPopup .QuickViewPanel {background: none repeat scroll 0 0 #dbf4cd !important;border: medium none !important;vertical-align: top;padding-left:5px;}
        .OrderPopup h5 {font-size: 13px;font-weight: bold;margin: 0 0 4px;padding-bottom: 5px;}
        .OrderPopup .OrderProducts .Key {text-align: right;}
        .OrderPopup .OrderProducts .Price {text-align: right;width: 100px;}
        .OrderPopup .OrderProducts .Seperator {margin: 10px 0;}
    </style>
</asp:Content>