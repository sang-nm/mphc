<%@ Page Language="c#" ValidateRequest="false" MaintainScrollPositionOnPostback="true"
    EnableViewStateMac="false" CodeBehind="SearchResults.aspx.cs" MasterPageFile="~/App_MasterPages/layout.Master"
    AutoEventWireup="false" Inherits="CanhCam.Web.ProductUI.SearchResults" %>

<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" />
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
    <portal:SearchResultsDisplaySettings ID="displaySettings" runat="server" />
    <div class="container">
        <div class="search-page">
            <div class="search-page-top">
                <div class="left searchheading">
                    <portal:HeadingControl ID="heading" runat="server" />
                </div>
                <asp:Panel ID="pnlSearch" CssClass="form-inline searchcontrols" DefaultButton="btnDoSearch" runat="server">
                    <div class="form-group">
                        <asp:TextBox ID="txtSearchInput" CssClass="form-control" runat="server" MaxLength="255"></asp:TextBox>
                    </div>
                    <asp:Button CssClass="btn btn-default seachpage-btn" ID="btnDoSearch" runat="server" CausesValidation="false"
                        UseSubmitBehavior="true" />
                </asp:Panel>
                <asp:Label ID="lblMessage" runat="server" />
            </div>
            <asp:Panel ID="pnlInternalSearch" runat="server" DefaultButton="btnDoSearch">
                <div id="divDelete" runat="server" visible="false" class="btn-reindex">
                    <asp:Button SkinID="DefaultButton" ID="btnRebuildSearchIndex" runat="server" />
                </div>
                <div class="searchresults">
                    <portal:gbCutePager ID="pgrTop" runat="server" Visible="false" />
                    <div class="clear"></div>
                    <asp:Xml ID="xmlTransformer" runat="server"></asp:Xml>
                    <portal:gbCutePager ID="pgrBottom" runat="server" Visible="false" />
                </div>
                <span id="spnAltSearchLinks" runat="server" visible="false">
                    <asp:Literal ID="litAltSearchMessage" runat="server" />
                    <asp:HyperLink ID="lnkBingSearch" runat="server" Visible="false" CssClass="extrasearchlink" />
                    <asp:HyperLink ID="lnkGoogleSearch" runat="server" Visible="false" CssClass="extrasearchlink" />
                </span>
            </asp:Panel>
            <asp:Panel ID="pnlGoogleSearch" runat="server" Visible="false" CssClass="gcswrap">
                <portal:GoogleCustomSearchControl ID="gcs" runat="server" Visible="false" />
            </asp:Panel>
            <asp:Panel ID="pnlBingSearch" runat="server" Visible="false" CssClass="searchresults bingresults">
                <portal:BingSearchControl ID="bingSearch" runat="server" Visible="false" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightContent" ID="MPRightPane" runat="server" />
<asp:Content ContentPlaceHolderID="pageEditContent" ID="MPPageEdit" runat="server" />