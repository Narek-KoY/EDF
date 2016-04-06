<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InternalStockOutRequestViewUserControl.ascx.cs"
    Inherits="EDF_Web_Parts_3.InternalStockOutRequestView.InternalStockOutRequestViewUserControl" %>
<script type="text/javascript">

    $(document).ready(function () {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function EndRequestHandler(sender, args) {
            $('.datepicker').datepicker({ dateFormat: 'mm/dd/yy' });
        }

    });
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<style type="text/css">
    .right_content li
    {
        cursor: default;
    }
    .GridHeader
    {
        padding-left: 10px;
    }
    .addItemButton
    {
        background-color: #fafafa;
        border: 1px solid #fa6400;
        cursor: pointer;
        font-size: 16px;
        color: #fa6400;
    }
    .formWrapper ul.fill
    {
        border: #ebebeb 1px solid;
        background-color: #fff;
        padding-left: 18px;
        padding-right: 18px;
    }
    
    .formWrapper ul.fill li
    {
        color: #000;
        font-size: 18px;
        border-bottom: #ebebeb 1px solid;
        padding: 30px 0;
    }
    
    .formWrapper ul.fill li.header
    {
        color: #000;
        font-size: 18px;
        border-bottom: #ebebeb 1px solid;
        padding-top: 16px;
    }
    
    .formWrapper ul.fill li.header .title
    {
        font-weight: bold;
        margin-top: 16px;
        margin-bottom: 35px;
    }
    .formWrapper ul.fill li.header .subtitle
    {
        font-weight: bold;
        padding-top: 25px !important;
        text-align: left;
    }
    
    .formWrapper ul.fill li .title
    {
        font-weight: bold;
        margin-bottom: 35px;
    }
    
    .formWrapper ul.fill li .fields
    {
        margin-left: 20px;
    }
    
    .fill_li p
    {
        padding-right: 46px;
    }
    
    .textBox
    {
        border: #ebebeb 1px solid;
        background-color: #fafafa;
        padding: 3px;
    }
    
    .last
    {
        border-bottom: none;
    }
</style>
<asp:ScriptManagerProxy ID="ScriptManagerProxy" runat="server">
</asp:ScriptManagerProxy>
<div class="back back2">
    <p>
        Pending <span>Request: </span>
        <asp:Label ID="LblType" runat="server" Text="Type Name" />
        by <a runat="server" id="UserProf" target="_blank">
            <asp:Label ID="LabelName" runat="server" Text="User Name" />
        </a>from
        <asp:Label ID="LabelDep" runat="server" Text="User Departament" />
    </p>
</div>
<table style="width: 95%; margin-left: auto; margin-right: auto">
    <tr>
        <td style="vertical-align: top; width: 60%">
            <div runat="server" id="ViewMode" class="right_content formWrapper">
                <ul class="fill">
                    <li class="header">
                        <div class="img_logo fleft" style="background: url(/_layouts/images/edf/stockout.png) no-repeat 28px;">
                            <div class="fleft">
                                <img class="avatar" alt="Autor" runat="server" id="autorImg" src="" />
                            </div>
                        </div>
                        <div class="title fleft">
                            Internal Stock Out Request
                        </div>
                        <div class="title fright">
                            <asp:Label runat="server" ID="titleOrderNumber" class="fleft" />
                        </div>
                        <div class="clearfix">
                        </div>
                        <div class="subtitle">
                            Internal stock out request / Պահեստից դուրս գրման պահանջագիր
                        </div>
                        <div style="text-align: right; padding-bottom: -25px">
                            <asp:Button runat="server" ID="btnPrint" Text="Print" OnClientClick="javascript:setFormSubmitToFalse()"
                                CssClass="addItemButton" OnClick="btnPrint_Click" />
                            <asp:Button runat="server" ID="btnEdit" Text="Edit Form" CssClass="addItemButton"
                                OnClick="btnEdit_Click" />
                        </div>
                    </li>
                    <li class="clearfix">
                        <div>
                            <div class="title">
                                Type of requested item/Պահանջվող ապրանքի տեսակը*
                            </div>
                            <div class="fields">
                                <div class="clearfix">
                                    <div class="fleft">
                                        <asp:RadioButton ID="rbCommercial" runat="server" Enabled="false" CssClass="radio" />
                                    </div>
                                    <div class="fleft">
                                        <label runat="server" id="lblCommercial">
                                            Commercial / Կոմերցիոն (ex.:handset, sim card, dongle etc.)</label>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <div class="fleft">
                                        <asp:RadioButton ID="rbnonCommercial" runat="server" Enabled="false" CssClass="radio" />
                                    </div>
                                    <div class="fleft">
                                        <label runat="server" id="Label1">
                                            Non-Commercial / Ոչ կոմերցիոն (ex.: table, chair, etc.)</label>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <div class="fleft">
                                        <asp:RadioButton ID="rbStationery" runat="server" Enabled="false" CssClass="radio" />
                                    </div>
                                    <div class="fleft">
                                        <label runat="server" id="Label2">
                                            Stationery / Գրենական պիտույքներ</label>
                                    </div>
                                </div>
                                <br />
                                <div class="clearfix">
                                    <div>
                                        <label runat="server" id="txtCostCenter">
                                            Cost center:</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>
                    <li class="clearfix">
                        <div>
                            <div class="title">
                                Item / Ապրանք
                            </div>
                            <asp:GridView GridLines="None" runat="server" AutoGenerateColumns="false" ID="dgItemsView">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="GridHeader"
                                        HeaderStyle-HorizontalAlign="Center" HeaderText="Item code / կոդը" HeaderStyle-Font-Size="16px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblItemCode"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="GridHeader"
                                        HeaderStyle-HorizontalAlign="Center" HeaderText="Description / նկարագիրը" HeaderStyle-Font-Size="16px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblDescription"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="GridHeader"
                                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="16px" HeaderText="Quantity / Քանակ">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblQuantity"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="GridHeader"
                                        HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="16px" HeaderText="Unit / Չափման միավոր">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblUnit"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle Font-Bold="True" ForeColor="black" />
                            </asp:GridView>
                        </div>
                    </li>
                    <li runat="server" id="liPurpose" class="clearfix">
                        <div>
                            <div class="title">
                                Purpose / Ապրանքի դուրս գրման նպատակը:*
                            </div>
                            <div class="fields">
                                <div class="clearfix">
                                    <div class="fleft">
                                        <asp:RadioButton ID="rbpermanent" runat="server" Enabled="false" CssClass="radio" />
                                    </div>
                                    <div class="fleft">
                                        <label runat="server" id="Label3">
                                            For permanent use / Մշտական օգտագործման համար</label>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <div class="fleft">
                                        <asp:RadioButton ID="rbTemporary" runat="server" Enabled="false" CssClass="radio" />
                                    </div>
                                    <div class="fleft">
                                        <label runat="server" id="lblTemporary">
                                            Temporary use / Ժամանակավոր օգտագործման համար</label>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <span style="color: Red" runat="server" id="SpanDueDate" visible="false">Due date</span>
                                </div>
                                <div class="clearfix">
                                    <div class="fleft">
                                        <asp:RadioButton ID="rbOther" runat="server" Enabled="false" CssClass="radio" />
                                    </div>
                                    <div class="fleft" style="margin-right: 10px">
                                        <label>
                                            Other purpose
                                        </label>
                                    </div>
                                    <div class="fleft">
                                        <asp:TextBox ID="txtOtherPurpose" runat="server" CssClass="textBox" Enabled="false"
                                            Visible="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>
                    <li class="clearfix" runat="server" id="liBudgeted">
                        <div class="fleft" style="margin-right: 10px">
                            Budgeted account:
                        </div>
                        <div class="fleft">
                            <asp:TextBox ID="txtBudgetedaccount" runat="server" CssClass="textBox" Enabled="false"></asp:TextBox>
                        </div>
                    </li>
                    <li class="clearfix">
                        <div class="fleft" style="margin-right: 10px">
                            Comments:
                        </div>
                        <div class="fleft">
                            <textarea runat="server" id="txtComments" class="edit_text " disabled="disabled"
                                rows="4" cols="50" maxlength="500" style="color: Black"></textarea>
                        </div>
                    </li>
                </ul>
            </div>
        </td>
        <td style="vertical-align: top; padding-left: 25px">
            <%--------------------- COMMENT -------------------------------------%>
            <div id="commentsSection" runat="server">
                <div class="sale1">
                    <p>
                        Comments:</p>
                </div>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div runat="server" id="divCom">
                        </div>
                        <div class="editor clearfix">
                            <table>
                                <tr>
                                    <td style="vertical-align: top; width: 15%">
                                        <div class="fleft">
                                            <img class="avatar" runat="server" id="comImg" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="editing fright">
                                            <textarea runat="server" id="ComText" class="edit_text " onfocus="if(this.value == 'Write your comment:') {this.value = '';}"
                                                onblur="if (this.value == '') {this.value = 'Write your comment:';}" style="color: Black">Write your comment:</textarea>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                       
                                    </td>
                                    <td>
                                    
                                     <asp:UpdateProgress ID="updateProgress" runat="server">
                                            <ProgressTemplate>
                                                <div style="position: absolute; z-index: 9999999; padding-left:150px; padding-top:20px">
                                                    <asp:Image ID="loadGif" runat="server" ImageUrl="/_layouts/images/edf/Customloading.gif" />
                                                </div>
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>

                                        <div class="fright poster">
                                            <asp:Button ID="ComSend" runat="server" class="post" Text="Post Comment" OnClick="ComSend_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        </span>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <%--------------------- APPROVE -------------------------------------%>
            <div runat="server" id="divStockkeeper" visible="false">
                <div class="clearfix">
                    <div class="fleft">
                        <asp:RadioButton ID="rbAdminStock" runat="server" CssClass="radio" GroupName="A1"
                            Checked="true" />
                    </div>
                    <div class="fleft">
                        <label>
                            Admin stock keeper</label>
                    </div>
                </div>
                <div class="clearfix">
                    <div class="fleft">
                        <asp:RadioButton ID="rbRaomarsStock" runat="server" CssClass="radio" GroupName="A1" />
                    </div>
                    <div class="fleft">
                        <label>
                            RAOMARS stock keeper</label>
                    </div>
                </div>
            </div>
            <div runat="server" id="AppRejDiv" visible="false">
                <asp:Button ID="BtnReject" runat="server" Text="Reject Request" CssClass="button"
                    Style="margin-right: 10px" OnClick="BtnReject_Click" Width="200px" Height="50px" />
                <asp:Button ID="BtnApprove" runat="server" Text="Approve Request" CssClass="subbmit"
                    OnClick="BtnApprove_Click" Width="200px" Height="50px" />
            </div>
            <div runat="server" id="ProvideDiv" visible="false">
                <asp:Button ID="btnNotProvided" runat="server" Text="Not Provided" CssClass="button"
                    Style="margin-right: 10px" OnClick="btnNotProvided_Click" Width="200px" Height="50px" />
                <asp:Button ID="btnProvided" runat="server" Text="Provided" CssClass="subbmit" OnClick="btnProvided_Click"
                    Width="200px" Height="50px" />
            </div>
            <br />
            <div class="reject">
                <asp:Label runat="server" ID="AppRejStatus" Visible="false"></asp:Label>
            </div>
            <br />
            <div runat="server" id="AppStatus">
                <ul id="ulApprovedBy" runat="server">
                    <li class="right_top">
                        <p style="font-size: 20px;">
                            Approved/Rejected by</p>
                    </li>
                </ul>
                <br />
                <ul id="ulPending" runat="server">
                    <li class="right_top">
                        <p style="font-size: 20px;">
                            Pending for approval</p>
                    </li>
                </ul>
                <ul id="ulProvide" runat="server">
                    <li class="right_top">
                        <p style="font-size: 20px;">
                            Send To Provide</p>
                    </li>
                </ul>
            </div>
        </td>
    </tr>
</table>
<div class="back back1">
    <a href="#">
        <p>
            Back to top</p>
    </a>
</div>
