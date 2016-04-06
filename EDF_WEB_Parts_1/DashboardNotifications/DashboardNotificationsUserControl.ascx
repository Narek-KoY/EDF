<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardNotificationsUserControl.ascx.cs" Inherits="EDF_WEB_Parts_1.DashboardNotifications.DashboardNotificationsUserControl" %>
  
<style>
.Myclock 
    {
        font-size: 14px;
    }
</style>

<div class="content_right fright">
<div class="right_top">
    <p>
    <span>
    <asp:Label ID="Label1" runat="server" Text="Label" /> New</span> Notifications: </p>
</div>
    <div class="right_content">
        <ul runat="server" id="ul1" >
        <%--zzzzzz N E W zzzzzzz--%>
        </ul>
    </div>
    <div class="right_content" style="margin-top:0;">
        <ul runat="server" id="ul2" >
        <%--zzzzzz O L D zzzzzzz--%>
        </ul>
    </div>
    <div class="loader">
        <a href="/SitePages/Notifications.aspx"><p class="load">All notifications</p></a>
    </div>
</div>
