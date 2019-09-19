<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CommentControl.ascx.cs"
    Inherits="CanhCam.Web.ProductUI.CommentControl" %>

<%@ Register TagPrefix="Site" Assembly="CanhCam.Features.Product" Namespace="CanhCam.Web.ProductUI" %>
<Site:ProductDisplaySettings id="displaySettings" runat="server" />

<div id="pnlFeedback" visible="false" data-type="0" runat="server" class="commentpanel">
    <div class="journalTools">
        <div class="journalEditor">
            <div class="journalClose"></div>
            <textarea class="journalContent" placeholder="Nội dung"></textarea>
            <div class="journalInfo">
                <div id="divStar" runat="server" data-score="4" class="rating"></div>
                <input class="txtFullName" placeholder="Mời bạn nhập tên (Bắt buộc)" type="text" />
                <input class="txtEmail" placeholder="Mời bạn nhập email (Không bắt buộc)" type="text" />
                <a href="#" class="btnShare">Gửi bình luận</a>
            </div>
            <div class="journalPlaceholder">Mời bạn thảo luận hoặc đánh giá về sản phẩm này</div>
        </div>
    </div>
    <div class="clear"></div>
    <asp:Repeater ID="rptCommentTop" runat="server">
        <ItemTemplate>
            <div class="asktop">
                <div class="author">
                    <i class="iconcom"></i>
                </div>
                <div class="journalitem">
	                <div class="journalsummary">
		                <strong>
                            <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"FullName").ToString()) %>
                        </strong>
                        <span class="top-comment">Bình luận được ưa thích</span>
                        <div>
                            <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"ContentText").ToString()).Replace("\n", "<br/>").Replace("\r\n", "<br />") %>
                        </div>
                    </div>
		            <div class="journalfooter">
                        <span class="cmlike">
                            <%# CommentHelper.GetLikeText(Convert.ToInt32(Eval("CommentId"))) %>
                            <i class="iconcom-likecomm"></i>
                            <span><%#Eval("HelpfulYesTotal")%></span>
                        </span>
                        <abbr title='<%# DateTimeHelper.Format(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, "g", timeOffset) %>'>
                            <%# CommentHelper.GetTimeAgo(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, timeOffset) %>
                        </abbr>
	                </div>
	            </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <div class="journalItems">
        <asp:Repeater ID="rptComments" runat="server">
            <ItemTemplate>
                <div id='<%#"jid-" + Eval("CommentId") %>' class="journalrow">
                    <div class="author">
                        <i class="iconcom"></i>
                    </div>
                    <div class="journalitem">
                        <%# ProductPermission.CanDeleteComment ? "<div onclick='journalDelete(this);' class='minidel'></div>" : "" %>
	                    <div class="journalsummary">
		                    <strong>
                                <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"FullName").ToString()) %>
                            </strong>
                            <%# Convert.ToBoolean(Eval("IsModerator")) ? "<b class='mod'>Quản trị viên</b>" : "" %>
                            <% if (CommentType == ProductCommentType.Rating) { %>
                            <div class="ratingview"><%# FormatRating(Convert.ToInt32(Eval("Rating"))) %></div>
                            <% } %>
                            <div>
                                <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"ContentText").ToString()).Replace("\n", "<br/>").Replace("\r\n", "<br />") %>
                            </div>
                        </div>
		                <div class="journalfooter">
                            <a class="cmreply" id='<%#"cmtbtn-" + Eval("CommentId") %>' href="#">Trả lời</a>
                            <a class="cmlike" id='<%#"like-" + Eval("CommentId") %>' href="#">
                                <%# CommentHelper.GetLikeText(Convert.ToInt32(Eval("CommentId"))) %>
                                <i class="iconcom-likecomm"></i>
                                <span><%#Eval("HelpfulYesTotal")%></span>
                            </a>
                            <abbr title='<%# DateTimeHelper.Format(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, "g", timeOffset) %>'>
                                <%# CommentHelper.GetTimeAgo(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, timeOffset) %>
                            </abbr>
                            <%# Convert.ToInt32(Eval("Status"))==0 ? "<span onclick='journalReport(this);' class='cmreport' id='report-" + Eval("CommentId") + "'>Báo vi phạm</span>" : "" %>
	                    </div>
	                </div>
                    <ul id='<%#"jcmt-" + Eval("CommentId") %>' class="jcmt">
                        <asp:Repeater ID="rptChildComments" DataSource='<%# GetChildComments(Convert.ToInt32(Eval("CommentId"))) %>' runat="server">
                            <ItemTemplate>
                                <li id='<%#"cmt-" + Eval("CommentId") %>'>
                                    <%# ProductPermission.CanDeleteComment ? "<div class='miniclose'></div>" : "" %>
                                    <i class="iconcom"></i>
                                    <div class="jsummary">
                                        <strong>
                                            <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"FullName").ToString()) %>
                                        </strong>
                                        <%# Convert.ToBoolean(Eval("IsModerator")) ? "<b class='mod'>Quản trị viên</b>" : "" %>
                                        <div>
                                            <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem,"ContentText").ToString()).Replace("\n", "<br/>").Replace("\r\n", "<br />") %>
                                        </div>
                                        <a class="cmlike" id='<%#"like-" + Eval("CommentId") %>' href="#">
                                            <%# CommentHelper.GetLikeText(Convert.ToInt32(Eval("CommentId"))) %>
                                            <i class="iconcom-likecomm"></i>
                                            <span><%#Eval("HelpfulYesTotal")%></span>
                                        </a>
                                        <abbr title='<%# DateTimeHelper.Format(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, "g", timeOffset) %>'>
                                            <%# CommentHelper.GetTimeAgo(Convert.ToDateTime(Eval("CreatedUtc")), timeZone, timeOffset) %>
                                        </abbr>
                                        <%# Convert.ToInt32(Eval("Status"))==0 ? "<span onclick='journalReport(this);' class='cmreport' id='report-" + Eval("CommentId") + "'>Báo vi phạm</span>" : "" %>
                                    </div>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                        <li id='<%#"jcmt-" + Eval("CommentId") + "-txtrow" %>' class="cmteditarea">
                            <textarea class="cmteditor" id='<%#"jcmt-" + Eval("CommentId") + "-txt" %>'></textarea>
                            <div class="editorPlaceholder">Mời bạn thảo luận hoặc đánh giá về sản phẩm này</div>
                        </li>
                        <li class="cmtbtn">
                            <div class="cmtinfo">
                                <input type="text" class="cmtname" placeholder="Mời bạn nhập tên (Bắt buộc)" maxlength="100" />
                                <input type="text" class="cmtemail" placeholder="Mời bạn nhập email (Không bắt buộc)" maxlength="100" />
                                <a href="#">Gửi bình luận</a>
                            </div>
                        </li>
                    </ul>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div id="divPager" runat="server" visible="false" class="pages commentpager">
        <portal:gbCutePager ID="pgr" PageSize="10" LeaveOutSpans="true" CurrentPageCssClass="ModulePager SelectedPage" runat="server" />
    </div>
</div>