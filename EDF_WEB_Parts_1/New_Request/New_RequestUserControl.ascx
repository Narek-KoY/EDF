<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="New_RequestUserControl.ascx.cs"
    Inherits="EDF_WEB_Parts_1.New_Request.New_RequestUserControl" %>
<style>
    .fs
    {
        font-size: 16px;
    }
</style>
<div class="request" style="margin-bottom: 35px">
    <ul class="request_ul">
        <li class="request_li clearfix" onclick="location.href='/SitePages/VacationRequest.aspx';"
            style="cursor: pointer;">
            <div runat="server" id="img1" class="auto fleft">
            </div>
            <p class="fleft">
                <asp:Label ID="Label1" runat="server" Text="Vacation Request" CssClass="fs" />
            </p>
            <div class="step fright">
            </div>
        </li>
        <li runat="server" id="LTR" class="request_li clearfix" onclick="location.href='/SitePages/LocalTravelRequest.aspx';"
            style="cursor: pointer;">
            <div runat="server" id="img2" class="auto fleft">
            </div>
            <p class="fleft">
                <asp:Label ID="Label2" runat="server" Text="International Travel Request" CssClass="fs" />
            </p>
            <div class="step fright">
            </div>
        </li>
        <li class="request_li clearfix" onclick="location.href='/SitePages/InternationalTravelOrder.aspx';"
            style="cursor: pointer;">
            <div runat="server" id="img3" class="auto fleft">
            </div>
            <p class="fleft">
                <asp:Label ID="Label3" runat="server" Text="Leave Request Request" CssClass="fs" />
            </p>
            <div class="step fright">
            </div>
        </li>
        <li class="request_li clearfix" onclick="location.href='/SitePages/RoundSheetRequest.aspx';"
            style="cursor: pointer;">
            <div runat="server" id="img4" class="auto fleft">
            </div>
            <p class="fleft">
                <asp:Label ID="Label4" runat="server" Text="Round Sheet Request" CssClass="fs" />
            </p>
            <div class="step fright">
            </div>
        </li>
        <li class="request_li clearfix" onclick="location.href='/SitePages/DatabaseAccessRequest.aspx';"
            style="cursor: pointer;">
            <div runat="server" id="img5" class="auto fleft">
            </div>
            <p class="fleft">
                <asp:Label ID="Label5" runat="server" Text="Database Access Request" CssClass="fs" />
            </p>
            <div class="fright step">
            </div>
        </li>
        <li class="request_li clearfix" onclick="location.href='/SitePages/InternalStockOutRequest.aspx';"
            style="cursor: pointer;">
            <div runat="server" id="img6" class="auto fleft">
            </div>
            <p class="fleft">
                <asp:Label ID="Label6" runat="server" Text="Internal Stock Out Request" CssClass="fs" />
            </p>
            <div class="fright step">
            </div>
        </li>
    </ul>
</div>
