<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyRequestsUserControl.ascx.cs"
    Inherits="EDF_Web_Parts_2.MyRequests.MyRequestsUserControl" %>
<style>
    .my_img_logo
    {
        width: 80px;
        height: 50px;
        margin-right: 20px;
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
</style>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel UpdateMode="Conditional" ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="content_right fleft">
            <div class="right_content">
                <ul id="ulPend" runat="server">
                </ul>
            </div>
            <div class="right_top" style="margin-top: 30px">
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
            <asp:HiddenField ID="hf_start" Value="5" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
