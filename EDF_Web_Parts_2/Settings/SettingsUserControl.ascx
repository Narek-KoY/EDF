<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingsUserControl.ascx.cs" Inherits="EDF_Web_Parts_2.Settings.SettingsUserControl" %>

<div class="Content_inner clearfix" style="min-height:250px;">
<div style="float:left; margin:13px;">
<h3>Replacement: </h3>
</div>

<SharePoint:PeopleEditor ID="up" runat="server" Width="350" SelectionSet="User" />

                                            <!--   CALENDAR   --->
<div class="content_left fright" style="width:600px;">
    <div style="float:left;margin:20px;">
        <asp:Label ID="lb_date_valid" style="color:Red" runat="server" Text=""></asp:Label>
    </div>
    <div class="request" style="width:540px;">
        <ul class="sort clearfix">
            <li class="clearfix fleft"><input runat="server" id="tb_start" type="text" value="Select start date" class="datepicker" style="width:210px; height:30px;"/></li>
            <li class="clearfix fright"><input runat="server" id="tb_end" type="text" value="Select end date" class="datepicker" style="width:210px; height:30px" /></li>
        </ul>
    </div>
</div>


<div style="float:left">
<asp:Label ID="returnValue" style="color:Green"  runat="server"></asp:Label>
</div>

</div>
<br />
<asp:Button ID="bt_Remove" runat="server" Text="Remove" CssClass="button but" OnClick="bt_Remove_click" />
<asp:Button ID="bt_Save" runat="server" Text="Save" CssClass="subbmit but" OnClick="bt_Save_click" />



