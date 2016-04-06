<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="adUserToDBUserControl.ascx.cs" Inherits="EDF_Web_Parts_2.adUserToDB.adUserToDBUserControl" %>

<style>
.rep
{
    float:left;
    margin:13px;
}

.but
{
    width:250px;
}
</style>




<div class="Content_inner clearfix" style="min-height:250px;">
<div class="rep">

</div>
<table>
    <tr>
        <td>
            <h3>User: </h3>
        </td>
        <td>
            <SharePoint:PeopleEditor ID="up_user" runat="server" Width="350" SelectionSet="User" />   
            <asp:Label ID="lb_date_valid" style="color:Red" runat="server" Text=""></asp:Label>     
        </td>
    </tr>
</table>

</div>
<br />
<asp:Button ID="bt_Add" runat="server" Text="Add User To Data Base" CssClass="subbmit but" OnClick="bt_add_click" />