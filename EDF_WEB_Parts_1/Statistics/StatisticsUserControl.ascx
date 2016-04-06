<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatisticsUserControl.ascx.cs"
    Inherits="EDF_WEB_Parts_1.Statistics.StatisticsUserControl" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<style>
    .hitem
    {
        height: 100px;
    }
    
    .input_key
    {
        float: left;
    }
    .addItemButton
    {
        background-color: #fafafa;
        border: 1px solid #fa6400;
        cursor: pointer;
        font-size: 16px;
        color: #fa6400;
        margin-left: 16px;
    }
</style>
<script>
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="Content_inner clearfix">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel UpdateMode="Conditional" ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExcel" />
        </Triggers>
        <ContentTemplate>
            <asp:HiddenField ID="hf_start" Value="5" runat="server" />
            <asp:HiddenField ID="hf_asc" Value="desc" runat="server" />
            <asp:HiddenField ID="hf_type" Value="d" runat="server" />
            <div class="content_right fleft">
                <div class="right_top" >
                    <p>
                        Pend <span>Requests:</span></p>
                </div>
                <div class="right_content">
                    <ul id="ulPend" runat="server">
                    </ul>
                </div>
                <div class="right_top" style="margin-top: 30px">
                    <p>
                        Past <span>Requests:</span></p>
                </div>
                <div class="right_content">
                    <ul id="ulAppRej" runat="server">
                    </ul>
                </div>
                <div class="loader">
                    <asp:LinkButton ID="ButtonAll" runat="server" OnClick="BtnLoad_Click"><p class="load">Load earlier notifications</p></asp:LinkButton>
                </div>
            </div>
            <div class="content_left fright" style="margin-bottom: 30px;">
                <div class="sale" style="padding-top: 0 !important">
                    <p>
                        <span>Filter</span> requests:</p>
                </div>
                <div class="request">
                    <ul class="sort clearfix" style="height: 190px; line-height: 60px">
                        <li class="clearfix fleft" style="line-height: 30px; font-size: 20px;">
                            <div class="check" style="margin-top: 0px;">
                                <asp:CheckBox ID="T1" runat="server" />
                                <label>
                                    Vacation Request</label>
                            </div>
                            <div class="check" style="margin-top: 0px;">
                                <asp:CheckBox ID="T2" runat="server" />
                                <label>
                                    Local Travel Request</label>
                            </div>
                            <div class="check" style="margin-top: 0px;">
                                <asp:CheckBox ID="T3" runat="server" />
                                <label>
                                    International Travel Order</label>
                            </div>
                            <div class="check" style="margin-top: 0px;">
                                <asp:CheckBox ID="T4" runat="server" />
                                <label>
                                    Round Sheet Request</label>
                            </div>
                            <div class="check" style="margin-top: 0px;">
                                <asp:CheckBox ID="T5" runat="server" />
                                <label>
                                    Database Access Request</label>
                            </div>
                            <div class="check" style="margin-top: 0px;">
                                <table style="margin-left: -2px">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="T6" runat="server" />
                                            <label>
                                                Stock Out Request</label>
                                        </td>
                                        <td>
                                            <asp:Button runat="server" ID="btnExcel" OnClick="btnExcel_Click" Text="SOR Excel"
                                                CssClass="addItemButton" Visible="false" OnClientClick="javascript:setFormSubmitToFalse()" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </li>
                    </ul>
                    <ul class="sort clearfix" style="height: 66px; line-height: 60px">
                        <li class="clearfix fleft">
                            <input runat="server" id="sdate" type="text" value="Select start date" class="datepicker"
                                style="width: 210px; height: 30px;" /></li>
                        <li class="clearfix fright">
                            <input runat="server" id="enddate" type="text" value="Select end date" class="datepicker"
                                style="width: 210px; height: 30px; margin-right: 15px" /></li>
                    </ul>
                    <ul class="clearfix">
                        <asp:HiddenField ID="HiddenFieldSort" runat="server" />
                        <li class="sort clearfix">
                            <asp:LinkButton runat="server" OnClick="date_Click" ID="date">
                                <p class="fleft">
                                    Sort by date</p>
                                <div runat="server" id="st_date" class="step1 fright">
                                </div>
                            </asp:LinkButton>
                        </li>
                        <li class="sort clearfix">
                            <asp:LinkButton runat="server" OnClick="status_Click" ID="status">
                                <p class="fleft">
                                    Sort by status</p>
                                <div runat="server" id="st_status" class="step1 fright">
                                </div>
                            </asp:LinkButton>
                        </li>
                        <li class="sort clearfix">
                            <asp:LinkButton runat="server" OnClick="type_Click" ID="type">
                                <p class="fleft">
                                    Sort by type</p>
                                <div runat="server" id="st_type" class="step1 fright">
                                </div>
                            </asp:LinkButton>
                        </li>
                        <li class="sort clearfix" style="height: 100%; line-height: 42px">
                            <asp:TextBox ID="KeyWords" TextMode="multiline" Columns="50" Rows="5" runat="server" />
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" Style="font-size: 13px; margin-top: -15px; font-style: italic"
                                            Text="Enter different keywords separated by semicolons" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:UpdateProgress ID="updateProgress" runat="server">
                                            <ProgressTemplate>
                                                <asp:Image ID="loadGif" Style="margin-left:50px;" runat="server" ImageUrl="/_layouts/images/edf/Customloading.gif" />
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </td>
                                </tr>
                            </table>
                        </li>
                        <li class="sort clearfix" style="height: 60px;">
                            <div class="loader" style="width: 425px; height: 38px;">
                                <asp:LinkButton runat="server" Width="100%" OnClick="BtnSearch_Click" ID="BtnSearch"><center><img alt="" class="my_img_logo" src="/_catalogs/masterpage/images/search_icon_gray.png" /></center></asp:LinkButton>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
