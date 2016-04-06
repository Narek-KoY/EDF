<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchUserControl.ascx.cs" Inherits="EDF_Web_Parts_2.Search.SearchUserControl" %>


<div class="Content_inner clearfix" style="margin-bottom:50px">
    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
            </asp:ScriptManagerProxy>
        <asp:UpdatePanel UpdateMode=Conditional ID="UpdatePanel1" runat="server">
            <ContentTemplate>
    <div class="content_right fleft">
            
    <asp:HiddenField ID="start" runat="server" />
    <asp:HiddenField ID="NotCount" runat="server" />
    <asp:HiddenField ID="HFstatus" runat="server" />
    <asp:HiddenField ID="HFtype" runat="server" />
    <asp:HiddenField ID="HFdate" runat="server" />
    <asp:HiddenField ID="HFkey" runat="server" />
    <asp:HiddenField ID="hf_key" runat="server" />
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
                <asp:LinkButton ID="ButtonAll" runat="server" onclick="Status_Click" ><p class="load">Load earlier notifications</p></asp:LinkButton>        
            </div>
           
    </div>

    <div class="content_left fright">
        <div class="sale" style="padding-top:0 !important">
            <p><span>Filter</span> requests:</p>
        </div>
        <div class="request">
            <ul class="sort clearfix">
                <li class="clearfix fleft"><input runat="server" id="sdate" type="text" value="Select start date" class="datepicker"  style="width: 210px; height: 30px;" /></li>
                <li class="clearfix fright"><input runat="server" id="enddate" type="text" value="Select end date" class="datepicker"  style="width: 210px; height: 30px;" /></li>                
            </ul>
            <ul class="clearfix">
                <asp:HiddenField ID="HiddenFieldSort" runat="server" />
                <li class="sort clearfix">
                    <asp:LinkButton runat="server" onclick="Status_Click" ID="date"><p class="fleft">Sort by date</p><div runat="server" id="st_date" class="step1 fright"></div></asp:LinkButton>
                </li>
                <li class="sort clearfix">
                    <asp:LinkButton runat="server" onclick="Status_Click" ID="status"><p class="fleft">Sort by status</p><div runat="server" id="st_status" class="step1 fright"></div></asp:LinkButton>
                </li>
				<li class="sort clearfix">
                    <asp:LinkButton runat="server" onclick="Status_Click" ID="typeS"><p class="fleft">Sort by type</p><div runat="server" id="st_type" class="step1 fright"></div></asp:LinkButton>
                </li>
                <li class="sort clearfix" style="height:60px;">
                    <div class="loader" style="width: 425px;height: 38px;" >       
                        <asp:LinkButton runat="server" Width="100%" onclick="Status_Click" ID="SearchKey" ><center><img alt="" class="my_img_logo" src="/_catalogs/masterpage/images/search_icon_gray.png" /></center></asp:LinkButton>
                    </div>
                </li>
            </ul>
        </div>
    </div>
     </ContentTemplate>
        </asp:UpdatePanel>
    </div>