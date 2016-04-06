<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuUserControl.ascx.cs"
    Inherits="EDF_WEB_Parts_1.Menu.MenuUserControl" %>
<style >
    .Tmenu
    {
        margin: 0 18px 0 18px;
        color: #fafafa;
    }
    .Tmenu2
    {
        margin: 0 18px 0 18px;
        color: #7ebc0a !important;
    }
</style>
<div class="Header_bottom">
    <div class="Header_bottom_inner clearfix">
        <div class="Header_bottom_menu fleft">
            <ul class="menu2 clearfix">
                <li class="item2 clearfix fleft" onclick="location.href='http://intranet/SitePages/index.aspx';"
                    style="cursor: pointer;"><span runat="server" id="Span1" class="Tmenu">Intranet Home
                    </span>
                    <div runat="server" id="Div1" class="active_bottom">
                    </div>
                </li>
                <li class="item2 clearfix fleft" onclick="location.href='/SitePages/Dashboard.aspx';"
                    style="cursor: pointer;"><span runat="server" id="itm11" class="Tmenu">Dashboard
                    </span>
                    <div runat="server" id="itm1" class="active_bottom">
                    </div>
                </li>
                <li class="item2 clearfix fleft" onclick="location.href='/SitePages/My%20Requests.aspx';"
                    style="cursor: pointer;"><span runat="server" id="itm12" class="Tmenu">My Requests
                    </span>
                    <div runat="server" id="itm2" class="active_bottom">
                    </div>
                </li>
                <li class="item2 fleft clearfix" onclick="location.href='/SitePages/Received%20Requests.aspx';"
                    style="cursor: pointer;"><span runat="server" id="itm13" class="Tmenu">Received Requests
                    </span>
                    <div runat="server" id="itm3" class="active_bottom">
                    </div>
                </li>
                <li runat="server" id="StUser" class="item2 fleft clearfix" onclick="location.href='/SitePages/Statistics.aspx';"
                    style="cursor: pointer;"><span runat="server" id="itm14" class="Tmenu">Statistics
                    </span>
                    <div runat="server" id="itm4" class="active_bottom">
                    </div>
                </li>
                <li class="item2 fleft clearfix notification" onclick="location.href='/SitePages/Notifications.aspx';"
                    style="cursor: pointer;"><span runat="server" id="itm15" class="Tmenu">Notifications<div
                        runat="server" id="NewNot" class="notification_ok fright">
                        <asp:Label ID="Label1" runat="server" Text="Label" />
                    </div>
                    </span>
                    <div runat="server" id="itm5" class="active_bottom">
                    </div>
                </li>
            </ul>
        </div>
        <div class="Search fright">
            <asp:Panel runat="server" ID="p_search" DefaultButton="Button1">
                <asp:TextBox ID="TextBox1" runat="server" CssClass="search_input" Text="Search the Requests Database"
                    Width="300px" BackColor="#666666" />
                <asp:Button ID="Button1" runat="server" CssClass="search_submit" OnClick="Button1_Click" />
            </asp:Panel>
        </div>
    </div>
</div>
