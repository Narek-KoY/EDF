<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InternalStockOutRequestUserControl.ascx.cs"
    Inherits="EDF_Web_Parts_3.InternalStockOutRequest.InternalStockOutRequestUserControl" %>
<script type="text/javascript">
    $(document).ready(function () {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function EndRequestHandler(sender, args) {
            $('.datepicker').datepicker({ dateFormat: 'mm/dd/yy' });
        }

    });

//    window.onload = function () {
//        var txts = document.getElementsByTagName('TEXTAREA')

//        for (var i = 0, l = txts.length; i < l; i++) {          
//                var func = function () {
//                    var len = 500;
//                    if (this.value.length > len) {
//                        this.value = this.value.substr(0, len);
//                        return false;
//                    }
//                }
//                txts[i].onkeyup = func;
//                txts[i].onblur = func;
//        }
//    }
</script>
<style type="text/css">
    .right_content li
    {
        cursor: default;
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
<asp:UpdatePanel ID="InsertMode" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="right_content formWrapper">
            <ul class="fill">
                <li class="header">
                    <div class="img_logo fleft" style="background: url(/_Layouts/images/edf/stockout.png) no-repeat 28px;">
                        <div class="fleft">
                            <img class="avatar" alt="Autor" runat="server" id="autorImg" src="" />
                        </div>
                    </div>
                    <div class="title fleft">
                        Internal Stock Out Request</div>
                    <div class="timer fright">
                    </div>
                    <div class="clearfix">
                    </div>
                    <div class="subtitle">
                        Internal stock out request / Պահեստից դուրս գրման պահանջագիր
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
                                    <asp:RadioButton ID="rbCommercial" runat="server" CssClass="radio" GroupName="A1"
                                        AutoPostBack="true" OnCheckedChanged="Type_OnCheckChanged" />
                                </div>
                                <div class="fleft">
                                    <label>
                                        Commercial / Կոմերցիոն (ex.:handset, sim card, dongle etc.)</label>
                                </div>
                            </div>
                            <div class="clearfix">
                                <div class="fleft">
                                    <asp:RadioButton ID="rbNonCommercial" runat="server" CssClass="radio" GroupName="A1"
                                        AutoPostBack="true" OnCheckedChanged="Type_OnCheckChanged" />
                                </div>
                                <div class="fleft">
                                    <label>
                                        Non-Commercial / Ոչ կոմերցիոն (ex.: table, chair, etc.)</label>
                                </div>
                            </div>
                            <div class="clearfix">
                                <div class="fleft">
                                    <asp:RadioButton ID="rbStationery" runat="server" CssClass="radio" GroupName="A1"
                                        AutoPostBack="true" OnCheckedChanged="Type_OnCheckChanged" />
                                </div>
                                <div class="fleft">
                                    <label>
                                        Stationery / Գրենական պիտույքներ</label>
                                </div>
                            </div>
                            <label style="color: Red" runat="server" id="ValRequestType" visible="false">
                            </label>
                            <br />
                            <div class="clearfix">
                                <div>
                                    Cost center*
                                    <asp:TextBox runat="server" ID="txtCostCenter" CssClass="textBox" />
                                    <label style="color: Red" runat="server" id="ValCostCenter" visible="false">
                                        Cost center is required.</label>
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
                        <asp:GridView GridLines="None" runat="server" AutoGenerateColumns="false" ID="dgItems"
                            OnRowDeleting="dgItems_RowDeleting">
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderText="Item code / կոդը" HeaderStyle-Font-Size="16px">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" Width="100" ID="txtItemCode" CssClass="textBox"></asp:TextBox>
                                        <asp:HiddenField runat="server" ID="hdnItemId" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderText="Description / նկարագիրը *" HeaderStyle-Font-Size="16px">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" Width="190" ID="txtItemDesc" CssClass="textBox"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="16px"
                                    HeaderText="Quantity / Քանակ *">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" Width="70" ID="txtItemQnty" CssClass="textBox"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="16px"
                                    HeaderText="Unit / Չափման միավոր  *">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" Width="70" ID="txtItemUOM" CssClass="textBox"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowDeleteButton="True" />
                            </Columns>
                            <HeaderStyle Font-Bold="True" ForeColor="black" />
                        </asp:GridView>
                        <asp:Button ID="btnAddRow" runat="server" CssClass="addItemButton" OnClick="btnAddRow_Click"
                            Text="Add item" />
                        <label id="valItems" runat="server" style="color: Red" visible="false">
                        </label>
                    </div>
                </li>
                <li class="clearfix" runat="server" id="liPurpose">
                    <div>
                        <div class="title">
                            Purpose / Ապրանքի դուրս գրման նպատակը:*
                        </div>
                        <div class="fields">
                            <div class="clearfix">
                                <div class="fleft">
                                    <asp:RadioButton ID="rbPermanent" runat="server" CssClass="radio" OnCheckedChanged="OnCheckChanged"
                                        GroupName="A2" AutoPostBack="True" />
                                </div>
                                <div class="fleft">
                                    <label>
                                        For permanent use (capex) / Մշտական օգտագործման համար
                                    </label>
                                </div>
                            </div>
                            <div class="clearfix">
                                <div class="fleft">
                                    <asp:RadioButton ID="rbTemporary" runat="server" CssClass="radio" OnCheckedChanged="OnCheckChanged"
                                        GroupName="A2" AutoPostBack="True" />
                                </div>
                                <div class="fleft">
                                    <label>
                                        Temporary use / Ժամանակավոր օգտագործման համար
                                    </label>
                                </div>
                            </div>
                            <div class="clearfix">
                                <div>
                                    <asp:TextBox runat="server" ID="txtDueDate" Text="Select due date" Visible="false"
                                        CssClass="datepicker" AutoPostBack="false" />
                                    <span runat="server" id="valDateTimeControl1" visible="false" style="color: Red">*</span>
                                    <asp:Button runat="server" ID="btnRecievedBack" Text="Returned Back" CssClass="addItemButton"
                                        Visible="false" OnClick="btnRecievedBack_Click" />
                                </div>
                            </div>
                            <div class="clearfix">
                                <div class="fleft">
                                    <asp:RadioButton ID="rbOther" runat="server" CssClass="radio" OnCheckedChanged="OnCheckChanged"
                                        GroupName="A2" AutoPostBack="True" />
                                </div>
                                <div class="fleft" style="margin-right:10px">
                                    <label >
                                        Other purpose   
                                    </label>
                                </div>
                                <div class="fleft">
                                    <asp:TextBox ID="txtOtherPurpose" runat="server" CssClass="textBox" Visible="false" ></asp:TextBox>
                                </div>
                            </div>
                            <div class="clearfix">
                                <label id="valPurpose" runat="server" style="color: Red" visible="false">
                                    Please check any radio button.</label>
                            </div>
                        </div>
                    </div>
                </li>
                <li class="clearfix" runat="server" id="liBudgeted">
                    <div class="clearfix">
                        <div class="fleft" style="margin-right:10px">
                            Budgeted account:  
                        </div>
                        <div class="fleft">
                            <asp:TextBox ID="txtBudgetedaccount" runat="server" CssClass="textBox"></asp:TextBox>
                        </div>
                    </div>
                </li>
                <li class="clearfix">
                    <div class="fleft">
                        Comments:
                    </div>
                    <div class="fleft" style="margin-right:10px">
                       <textarea runat="server" id="txtComments" class="edit_text" rows="4" cols="50" maxlength="500" style="color:Black"> </textarea>
                    </div>
                </li>
            </ul>
        </div>
        <div class="last_form" runat="server" id="InsertFormButtons">
            <asp:Button ID="ButtonCancel" runat="server" OnClick="ButtonCancel_Click" Text="Cancel and Clear Form"
                CssClass="button" Style="margin-right: 10px" />
            <asp:Button ID="ButtonSubmit" runat="server" UseSubmitBehavior="false" Text="Submit Form" OnClick="ButtonSubmit_Click"
                CssClass="subbmit" />
        </div>
        <div class="last_form" runat="server" id="EditFormButtons" visible="false">
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
                CssClass="button" Style="margin-right: 10px" />
            <asp:Button ID="btnUpdate" runat="server" Text="Edit Form" OnClick="btnUpdate_Click"
                CssClass="subbmit" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
