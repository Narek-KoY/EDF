<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReceivedRequestsPastUserControl.ascx.cs"
    Inherits="EDF_Web_Parts_2.ReceivedRequestsPast.ReceivedRequestsPastUserControl" %>
<style>
    .abs_class
    {
        position: absolute;
        clear: right;
        top: 200px;
        left: 800px;
    }
    .fixedsort
    {
        position: relative;
        bottom: 322px;
        left: 500px;
    }
    .load
    {
        border: 1px solid #ebebeb;
        background-color: #f2f2f2;
        margin-top: 10px;
        height: 48px;
        line-height: 48px;
        text-align: center;
        font-size: 18px;
        color: #c2bec0;
        cursor: pointer;
        width: 100%;
        height: 52px;
    }
    
    .lupa
    {
        background: url(/_catalogs/masterpage/images/search_icon_gray.png) no-repeat 4px 5px;
        height: 40px;
        width: 40px;
        border: 0px;
        background-color: #FFF;
        position: relative;
        top: 10px;
    }
    .lupa:hover
    {
        cursor: pointer;
    }
</style>
<!-----------------   END STYLE   ------------------>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="Content_inner clearfix">
            <div class="content_right fleft">
                <div class="right_content">
                    <ul id="ulPend" runat="server">
                    </ul>
                </div>
                <div class="right_top" >
                    <p>
                        My Past <span>Requests:</span></p>
                </div>
                <div class="right_content">
                    <ul id="ulAppRej" runat="server">
                    </ul>
                </div>
                <div class="loader">
                    <asp:Button ID="BtnLoad" CssClass="load" OnClick="BtnLoad_Click" Font-Size="13" runat="server"
                        Text="Load earlier requests" />
                </div>
                <asp:HiddenField ID="hf_start" Value="10" runat="server" />
            </div>
            <div class="content_left fright">
                <div class="sale" style="padding-top: 0 !important">
                    <p>
                        <span>Filter</span> requests:</p>
                </div>
                <div class="request">
                    <ul>
                        <li class="sort clearfix">
                            <asp:LinkButton runat="server" ID="date" OnClick="date_Click">
                                <p class="fleft">
                                    Sort by date</p>
                                <div id="st_date" runat="server" class="step1 fright">
                                </div>
                            </asp:LinkButton></li>
                        <li class="sort clearfix">
                            <asp:LinkButton runat="server" ID="status" OnClick="status_Click">
                                <p class="fleft">
                                    Sort by status</p>
                                <div id="st_status" runat="server" class="step1 fright">
                                </div>
                            </asp:LinkButton></li>
                        <li class="sort clearfix">
                            <asp:LinkButton runat="server" ID="type" OnClick="type_Click">
                                <p class="fleft">
                                    Sort by type</p>
                                <div id="st_type" runat="server" class="step1 fright">
                                </div>
                            </asp:LinkButton></li>
                    </ul>
                </div>
            </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:HiddenField ID="hf_asc" runat="server" />
</div>