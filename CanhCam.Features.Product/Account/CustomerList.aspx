<%@ Page Language="c#" CodeBehind="CustomerList.aspx.cs" MasterPageFile="~/App_MasterPages/layout.Master"
    AutoEventWireup="false" Inherits="CanhCam.Web.AccountUI.CustomerList" %>

<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <portal:BreadcrumbAdmin ID="breadcrumb" runat="server" CurrentPageTitle="Danh sách khách hàng"
        CurrentPageUrl="~/Account/CustomerList.aspx" />
    <div class="admin-content col-md-12">
        <portal:HeadingPanel ID="heading" runat="server">
            <asp:Button SkinID="ExportButton" ID="btnExport" OnClick="btnExport_Click" Visible='<%# canManageUsers %>'
                Text="<%$Resources:Resource, ExportDataAsExcel %>" runat="server" CausesValidation="false" />
            <asp:Button SkinID="DefaultButton" ID="btnChange" OnClick="btnChange_Click" Visible='<%# canManageUsers %>'
                Text="Change" runat="server" CausesValidation="false" />
        </portal:HeadingPanel>
        <portal:NotifyMessage ID="message" runat="server" />
        <div class="workplace">
            <div class="form-horizontal">
                <div class="settingrow form-group">
                    <gb:SiteLabel ID="lblFromDate" runat="server" Text="Ngày đăng ký từ" ForControl="dpFromDate"
                        CssClass="control-label col-sm-3" />
                    <div class="col-sm-9">
                        <telerik:RadDatePicker ID="dpFromDate" runat="server" />
                    </div>
                </div>
                <div class="settingrow form-group">
                    <gb:SiteLabel ID="lblToDate" runat="server" Text="Đến ngày" ForControl="dpToDate"
                        CssClass="control-label col-sm-3" />
                    <div class="col-sm-9">
                        <telerik:RadDatePicker ID="dpToDate" runat="server" />
                    </div>
                </div>
                <div class="settingrow form-group">
                    <gb:SiteLabel ID="lblTitle" runat="server" Text="Thông tin khách hàng"
                        ForControl="txtTitle" CssClass="settinglabel control-label col-sm-3" />
                    <div class="col-sm-9">
                        <div class="input-group">
                            <asp:TextBox ID="txtTitle" runat="server" MaxLength="255" />
                            <div class="input-group-btn">
                                <asp:Button SkinID="DefaultButton" ID="btnSearch" Text="Tìm kiếm" runat="server"
                                    CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <telerik:RadGrid ID="grid" SkinID="radGridSkin" runat="server">
                <MasterTableView DataKeyNames="UserID">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderStyle-Width="35" HeaderText="<%$Resources:Resource, RowNumber %>"
                            AllowFiltering="false">
                            <ItemTemplate>
                                <%# (grid.PageSize * grid.CurrentPageIndex) + Container.ItemIndex + 1%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Email">
                            <ItemTemplate>
                                <a href='<%# "mailto:" + Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Email").ToString())%>'>
                                    <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Email").ToString())%></a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Họ tên">
                            <ItemTemplate>
                                <%#Eval("FirstName")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Điện thoại">
                            <ItemTemplate>
                                <%#Eval("LoginName")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Địa chỉ">
                            <ItemTemplate>
                                <%#Eval("State")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Sinh nhật">
                            <ItemTemplate>
                                <%#Eval("Yahoo")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Điểm" SortExpression="TotalPostsEx">
                            <ItemTemplate>
                                <%#Eval("TotalPostsEx")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Tổng tiền đã mua" SortExpression="TotalRevenueEx">
                            <ItemTemplate>
                                <%#CanhCam.Web.ProductUI.ProductHelper.FormatPrice(Convert.ToDecimal(Eval("TotalRevenueEx")))%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderStyle-Width="150" HeaderText="<%$Resources:Resource, MemberListDateCreatedLabel %>"
                            DataField="DateCreated">
                            <ItemTemplate>
                                <%# DateTimeHelper.Format(Convert.ToDateTime(Eval("DateCreatedEx")), timeZone, "d", timeOffset)%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:HyperLink CssClass="cp-link" Text='<%# Resources.Resource.ManageUserLink %>'
                                    ID="Hyperlink2" NavigateUrl='<%# SiteRoot + "/AdminCP/ManageUsers.aspx?userid=" + DataBinder.Eval(Container.DataItem,"UserIdEx")   %>'
                                    Visible="<%# canManageUsers %>" runat="server" EnableViewState="false" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </div>
</asp:Content>
