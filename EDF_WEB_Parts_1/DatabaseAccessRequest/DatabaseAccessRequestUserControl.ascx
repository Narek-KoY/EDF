<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatabaseAccessRequestUserControl.ascx.cs"
    Inherits="EDF_WEB_Parts_1.DatabaseAccessRequest.DatabaseAccessRequestUserControl" %>
<script type="text/javascript">
    function hidefu(c, b) {
        if (b)
            document.getElementById('fu').className = "show_controle";
        else
            document.getElementById('fu').className = "hide_controle";

        hidecal(c.checked);
    }

    function hidefuu(b) {
        if (b)
            document.getElementById('fu').className = "show_controle";
        else
            document.getElementById('fu').className = "hide_controle";
        hidecal(false);
    }

    function hidecal(b) {
        if (b)
            document.getElementById('Step_1_2').className = "show_controle";
        else
            document.getElementById('Step_1_2').className = "hide_controle";
    }
</script>
<style type="text/css">
    .hide_controle
    {
        display: none;
    }
    .show_controle
    {
        display: block;
    }
    #info tr td
    {
        padding: 5px;
    }
    
    .val
    {
        color: Red;
    }
    
    .my_Date
    {
        border: 1px solid #ebebeb;
        width: 150px;
        border-radius: 10px;
        height: 30px;
        padding-left: 17px;
        line-height: 30px;
        background-color: #fafafa;
        font-size: 18px;
        font-style: italic;
        font-weight: 500;
        color: #c2bec0;
    }
</style>
<div class="right_content">
    <ul>
        <li class="cont clearfix" style="cursor: default">
            <div style="background: url(/_catalogs/masterpage/images/database.png) no-repeat 28px;"
                class="img_logo fleft">
                <div class="fleft">
                    <img class="avatar" runat="server" id="autorImg" /></div>
            </div>
            <p class="text4 fleft" style="margin-top: 16px;">
                Database Access Request</p>
            <div class="timer fright">
            </div>
        </li>
    </ul>
    <ul class="fill">
        <li class="fill_li first">
            <p>
                <b>
                    <center>
                        Database Access Request
                    </center>
                    <%--<center>Order N / Հրաման</center><center>1</center>--%></b></p>
        </li>
        <li class="fill_li clearfix">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <!----------------  STEP 1        -------------------->
                    <div id="Step_1" runat="server">
                        <p>
                            <span>Request for:</span></p>
                        <br />
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rb_Self" Text="Self" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="requestor" AutoPostBack="True" OnCheckedChanged="rb_Self_CheckedChanged"
                                            onclick="hidefu(this,true)" Checked="True" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rb_Benef" Text="Beneficiary" runat="server" CssClass="radio"
                                            Style="margin-left: 20px" GroupName="requestor" AutoPostBack="True" OnCheckedChanged="rb_Self_CheckedChanged" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                    </div>
                    <div id="step_2" runat="server">
                        <div id="Step_2_2" runat="server">
                            <p>
                                <span>Beneficiary</span></p>
                            <br />
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            <asp:RadioButton ID="rb_oam" Text="OAM user" runat="server" CssClass="radio" Style="margin-left: 20px"
                                                GroupName="beneficiary" Checked="True" OnCheckedChanged="rb_oam_CheckedChanged"
                                                AutoPostBack="True" />
                                        </td>
                                        <td>
                                            <asp:RadioButton ID="rb_no_oam" Text="Non OAM user" runat="server" CssClass="radio"
                                                Style="margin-left: 20px" GroupName="beneficiary" OnCheckedChanged="rb_oam_CheckedChanged"
                                                AutoPostBack="True" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <br />
                            <div id="div_oam" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            Name
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_name" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write name:</textarea>
                                        </td>
                                        <td>
                                            <asp:Label CssClass="val" ID="lb_val_name" runat="server" Text="*"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Department
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_Department" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write department:</textarea>
                                        </td>
                                        <td>
                                            <asp:Label CssClass="val" ID="lb_val_dep" runat="server" Text="*"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Position
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_Position" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write position:</textarea>
                                        </td>
                                        <td>
                                            <asp:Label CssClass="val" ID="lb_val_pos" runat="server" Text="*"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="div_oam_no" runat="server" visible="false">
                                <table>
                                    <tr>
                                        <td>
                                            Name
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_name2" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write name:</textarea>
                                        </td>
                                        <td>
                                            <asp:Label CssClass="val" ID="lb_val_name_2" runat="server" Text="*"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Organization
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_organization" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write organization:</textarea>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Country
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_country" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write country:</textarea>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Associated Department
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_assdep" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write associated department:</textarea>
                                        </td>
                                        <td>
                                            <asp:Label CssClass="val" ID="lb_val_ass_dep" runat="server" Text="*"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Team
                                        </td>
                                        <td>
                                            <textarea runat="server" id="ta_team" class="edit_text " style="min-height: 0px;
                                                color: Black;">Write team:</textarea>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Intern
                                        </td>
                                        <td>
                                            <asp:RadioButton ID="rb_intern_yes" Text="Yes" runat="server" CssClass="radio" Style="margin-left: 20px"
                                                GroupName="Intern" />
                                            <asp:RadioButton ID="rb_intern_no" Text="No" runat="server" CssClass="radio" Style="margin-left: 20px"
                                                GroupName="Intern" Checked="True" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <br />
                        <p>
                            <span>Equipment</span></p>
                        <br />
                        <table style="margin-left: 19px;">
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="cb_PC" Text="PC" runat="server" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="cb_Laptop" Text="Laptop" runat="server" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                        <!-------------------      E-MAIL              ---->
                        <p>
                            <span>E-mail</span></p>
                        <br />
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rb_internal_onli" Text="Internal only" runat="server" CssClass="radio"
                                            Style="margin-left: 20px" GroupName="email" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rb_unrestricted" Text="unrestricted" runat="server" CssClass="radio"
                                            Style="margin-left: 20px" GroupName="email" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rb_no_Email" Text="no e-mail" runat="server" CssClass="radio"
                                            Style="margin-left: 20px" GroupName="email" Checked="true" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                        <!----------     INTERNET ACCESS    ----->
                        <p>
                            <span>Internet Access</span></p>
                        <br />
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rb_no" Text="No" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="internetaccess" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rb_rest" Text="Restricted (without socials websites)" runat="server"
                                            CssClass="radio" Style="margin-left: 20px" GroupName="internetaccess" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rb_full" Text="Full" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="internetaccess" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <br />
                        <p>
                            <span>Description (File server, Tools, Databases, needed specific accesses): </span>
                        </p>
                        <br />
                        <textarea runat="server" id="ta_description" class="edit_text " onkeypress="return (this.value.length <500);"
                            style="min-height: 0px; color: Black;">Write Description:</textarea>
                        <br />
                        <p>
                            Max 500 character</p>
                        <br />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Maximum 500 characters allowed"
                            Display="Dynamic" ControlToValidate="ta_description" ValidationExpression="^{0,500}$"></asp:RegularExpressionValidator>
                        <br />
                        <p>
                            <span>Attachment (pdf, doc, docx, xls, xlsx, jpg, jpeg, png, gif) </span>
                        </p>
                        <br />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="fu">
                <asp:FileUpload ID="fu_atach" runat="server" />
                <br />
                <br />
                <br />
            </div>
            <div id="Step_1_2">
                <p>
                    <span>Requested access period</span></p>
                <br />
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="rb_no_oam" />
                        <asp:PostBackTrigger ControlID="rb_oam" />
                        <asp:PostBackTrigger ControlID="rb_Self" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:CheckBox ID="ChPermanent" runat="server" Text="Permanent" OnCheckedChanged="ChPermanent_CheckedChanged"
                            AutoPostBack="True" />
                        <div runat="server" id="data_div" class="request">
                            <ul class="sort clearfix">
                                <li class="clearfix fleft">
                                    <asp:TextBox runat="server" ID="tb_start" Text="Select start date" CssClass="datepicker" />
                                    <br />
                                    <label class="val" id="val_start" runat="server">
                                    </label>
                                </li>
                                <li class="clearfix fright">
                                    <asp:TextBox runat="server" ID="tb_end" Text="Select end date" CssClass="datepicker" />
                                    <br />
                                </li>
                            </ul>
                            <label class="val" id="val_end" runat="server">
                            </label>
                            <asp:Label ID="datevalid" runat="server" Text="" />
                            <br />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <asp:HiddenField ID="hf_step" runat="server" />
                    <!----------------     END       ----------------->
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
            <div class="last_form">
                <asp:Button ID="ButtonCancel" runat="server" Text="Cancel and Clear Form" Width="235"
                    CssClass="button" Style="margin-right: 10px" OnClick="ButtonCancel_Click" />
                <asp:Button ID="ButtonSubmit" runat="server" Text="Submit Form" CssClass="subbmit"
                    OnClick="ButtonSubmit_Click" />
            </div>
        </li>
    </ul>
</div>
