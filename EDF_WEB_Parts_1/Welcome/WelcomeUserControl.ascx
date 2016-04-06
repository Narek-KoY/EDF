<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WelcomeUserControl.ascx.cs" Inherits="EDF_WEB_Parts_1.Welcome.WelcomeUserControl" %>

<style type="text/css">
.text1
{
    margin-right:35px;
    }
    
</style>

<div class="profile">
    <div class="profile_top clearfix">
        <div class="fleft">        
            <img  class="avatar" runat="server" id="img_avatar" src="" />
        </div>        

        <div class="text fleft">
            <p>Welcome,</p>
            <h9>
            <asp:Label ID="Label4" runat="server" Text="Unknown"/>
            </h9>
        </div>
    </div>
    <div class="profile_bottom">
        <ul class="clearfix">
            <li class="text1 fleft">
                <a href="/SitePages/Notifications.aspx">
                    <p class="only">Notifications:</p>
                    <p class="orange_text">
                        <asp:Label ID="Label1" runat="server" Text="Label"/> NEW
                    </p>
                </a>
            </li>
            <li class="text1 fleft">
                <a href="/SitePages/My%20Requests.aspx">
                    <p class="only">My Requests:</p>
                    <p class="grey_text">
                        <asp:Label ID="Label2" runat="server" Text="Label"/> NEW
                    </p>
                </a>
            </li>
            <li class="text1 fleft">
                <a href="/SitePages/Received%20Requests.aspx">
                    <p class="only">Received requests:</p>
                    <p class="orange_text">
                        <asp:Label ID="Label3" runat="server" Text="Label"/> NEW
                    </p>
                </a>
            </li>
        </ul>
    </div>	
</div>
