<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDARUserControl.ascx.cs"
    Inherits="EDF_WEB_Parts_1.ViewDAR.ViewDARUserControl" %>
<script language="javascript" type="text/javascript">
    //        function printDiv(divID) {
    //            //Get the HTML of div
    //            var divElements = document.getElementById(divID).innerHTML;
    //            //Get the HTML of whole page
    //            var oldPage = document.body.innerHTML;

    //            //Reset the page's HTML with div's HTML only
    //            document.body.innerHTML =
    //              "<html><head><title></title></head><body>" +
    //              divElements + "</body>";

    //            //Print Page
    //            window.print();

    //            //Restore orignal HTML
    //            document.body.innerHTML = oldPage;


    //        }

    printDivCSS = new String('<link href="/_catalogs/masterpage/style.css" rel="stylesheet" type="text/css"><style>#avatar_div{visibility: hidden;} .fill_li{padding: 10px 0 !important;} .first{padding-top: 5px !important;padding-bottom: 5px !important;} p{margin-bottom:5px;}</style>')
    function printDiv(divId) {
        window.frames["print_frame"].document.body.innerHTML = printDivCSS + document.getElementById(divId).innerHTML;
        var element = window.frames["print_frame"].window.document.getElementById("avatar_div");
        element.parentNode.removeChild(element);
        $('#print_frame').contents().find('br').remove();
        $('#print_frame').contents().find('.save_btn').remove();
        $('#print_frame').contents().find('.fill_li').css({ 'display': 'block' });
        $('#print_frame').contents().find('.loader').remove();
        $('#print_frame').contents().find('p').css({ 'margin-bottom': '5px' });
        //window.frames["print_frame"].document.getElementById('avatar_div').innerHTML = "";
        window.frames["print_frame"].window.focus();
        window.frames["print_frame"].window.print();
    }

</script>
<style type="text/css">
    .info
    {
        padding: 16px;
        min-width: 100px;
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
<iframe id="print_frame" name="print_frame" style="display: none"></iframe>
<div id="Content">
    <div class="back back2">
        <p>
            Pending <span>Request: </span>
            <asp:Label ID="LblType" runat="server" Text="Type Name" />
            <asp:Label ID="Label2" runat="server" Text="" />
            by <a runat="server" id="UserProf" target="_blank">
                <asp:Label ID="LabelName" runat="server" Text="User Name" /></a> from
            <asp:Label ID="LabelDep" runat="server" Text="User Departament" /></p>
    </div>
    <div class="Content_inner clearfix">
        <div id="printablediv" class="right_content content_right fleft">
            <div class="right_content">
                <ul>
                    <li class="cont clearfix" style="cursor: default">
                        <div style="background: url(/_catalogs/masterpage/images/database.png) no-repeat 28px;"
                            class="img_logo fleft">
                            <div class="fleft">
                                <img class="avatar" runat="server" id="autorImg" /></div>
                        </div>
                        <p class="text4 fleft" style="margin-top: 16px;">
                            Database Access request</p>
                        <div class="timer fright">
                            <img runat="server" id="cert_icon" width="40" src="/_catalogs/masterpage/images/kindredmail-certificate-icon.png" /></div>
                    </li>
                </ul>
                <ul class="fill">
                    <li class="fill_li first">
                        <p>
                            <b>
                                <center>
                                    Database/Access request form description</center>
                                <span runat="server" id="OrderSpan">
                                    <center>
                                        Order N / Հրաման</center>
                                    <center>
                                        Ա<asp:Label ID="OrderLabel" runat="server" Text="0" /></center>
                                </span></b>
                        </p>
                    </li>
                    <li>
                        <div runat="server" id="Div1">
                            <table>
                                <tr>
                                    <td>
                                        Name
                                    </td>
                                    <td class="info">
                                        <b>
                                            <asp:Label ID="lb_name" runat="server"></asp:Label></b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Department / Position
                                    </td>
                                    <td class="info">
                                        <b>
                                            <asp:Label ID="lb_department" runat="server"></asp:Label>/
                                            <asp:Label ID="lb_position" runat="server"></asp:Label></b>
                                        <br />
                                    </td>
                                </tr>
                                <tr runat="server" id="phoneTr">
                                    <td>
                                        Phone
                                    </td>
                                    <td class="info">
                                        <b>
                                            <asp:Label ID="lb_phone" runat="server"></asp:Label></b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Requestor:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Requestor">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr runat="server" id="DIV_Equipment">
                                    <td>
                                        Equipment:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Equipment">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        E-mail:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_E_mail">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Internet Access:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Internet_Access">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr runat="server" id="tr_desc">
                                    <td>
                                        Description:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Description">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr id="tr_file" runat="server">
                                    <td>
                                        Attachment:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <asp:LinkButton ID="lb_Download" runat="server" OnClick="Download_Click"><p class="load">Download File</p></asp:LinkButton></b>
                                    </td>
                                </tr>
                                <tr id="tr_Start" runat="server">
                                    <td>
                                        Access Period Start Date:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Start">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr id="tr_End" runat="server">
                                    <td>
                                        Access Period End Date:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_End">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr id="tr1" runat="server" visible="false">
                                    <td>
                                        Requested access period:
                                    </td>
                                    <td class="info">
                                        <input type='checkbox' checked='checked' disabled="disabled" />
                                        Permanent
                                    </td>
                                </tr>
                                <tr id="tr_Ben" runat="server">
                                    <td>
                                        Beneficiary:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Beneficiary">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                            </table>
                            <div id="div_Beneficiary" runat="server">
                                <!--------    OAM INFO     ------------------->
                                <table id="div_oam_info" runat="server">
                                    <tr>
                                        <td>
                                            Name:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_name">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Department:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_dep">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Position:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_pos">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                                <!--------  NOT  OAM INFO     ------------------->
                                <table id="div_not_oam_info" runat="server">
                                    <tr>
                                        <td>
                                            Name:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_name2">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr id="lb_oam_org_td">
                                        <td>
                                            Organization:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_org">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr id="lb_oam_country_td">
                                        <td>
                                            Country:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_country">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Associated Department:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_assdep">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr id="lb_oam_team_td">
                                        <td>
                                            Team:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_team">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Intern:
                                        </td>
                                        <td class="info">
                                            <b>
                                                <label runat="server" id="lb_oam_intern">
                                                </label>
                                            </b>
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="groups_div" runat="server" visible="false">
                                <p>
                                    <span>Requestor MemberOf Field</span></p>
                                <br />
                                <ul id="ul_groups" runat="server">
                                </ul>
                            </div>
                            <div runat="server" id="divDARCom">
                            </div>
                            <div runat="server" id="DivAgiliti" class="reject" visible="false">
                                <asp:Button ID="ButtAgiliti" runat="server" Text="Agiliti Approve" CssClass="subbmit subbmit1"
                                    OnClick="ButtAgiliti_Click" />
                            </div>
                            <div runat="server" id="ExecDiv" visible="False">
                                <p>
                                    <span>Select a group</span></p>
                                <br />
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chb_Billing_division" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Billing division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="ta_billing" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chb_Reporting" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Reporting
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="ta_reporting" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chb_IT_Systems_Engineering" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                IT Systems Engineering
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="ta_syseng" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chb_OfficeIT" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Office/Network IT
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="ta_OfficeIT" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div runat="server" id="ITSEDiv" visible="False">
                                <p>
                                    <span>Select a Team</span></p>
                                <br />
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChITSE" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                IT Systems Engineering taem
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="Textarea1" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChOfficeITDivision" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Office/Network IT division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="Textarea5" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChBD" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Billing division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="Textarea3" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChREP" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Reporting division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="Textarea4" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div runat="server" id="OfficeITDiv" visible="False">
                                <p>
                                    <span>Select a Team</span></p>
                                <br />
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChbOfficeITTeam" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Office/Network IT Team
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="OfficeITTeamTextArea" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChbITSE" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                IT Systems Engineering division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="TextareaITSE" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChbBILLING" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Billing division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="TextareaBILLING" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChbREPORTING" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Reporting division
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="TextareaREPORTING" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div runat="server" id="BILLDiv" visible="False">
                                <p>
                                    <span>Select a Team</span></p>
                                <br />
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChBilling2" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Billing team
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="ta_billing1" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChDBSE2" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                DBSE team
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="ta_dbse" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div runat="server" id="REPDiv" visible="False">
                                <p>
                                    <span>Select a Team</span></p>
                                <br />
                                <table>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChReporting3" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                Reporting team
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="Textarea8" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="ChDBSE3" runat="server" CssClass="radio" Style="margin-left: 20px" />
                                            </td>
                                            <td>
                                                DBSE team
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea runat="server" id="Textarea9" class="edit_text " style="color: Black">Write your comment:</textarea>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <label style="color: Red" runat="server" id="Valchb" visible="false">
                                Please check any checkbox!</label>
                        </div>
                    </li>
                </ul>
            </div>
            <div runat="server" id="PrintDiv" class="loader" visible="false">
                <asp:LinkButton Visible="false" ID="PrintLink" runat="server" OnClick="PrintLink_Click"><p class="load">P R I N T</p></asp:LinkButton>
            </div>
        </div>
        <!------------   COMMENT          -------------------------->
        <p>
        <div class="content_left fright">
            <div class="sale1">
                <p>
                    Comments:</p>
            </div>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div runat="server" id="divCom">
                    </div>
                    <div class="editor clearfix">
                        <div class="fleft">
                            <img class="avatar" runat="server" id="comImg" />
                        </div>
                        <div class="editing fright">
                            <textarea runat="server" id="ComText" class="edit_text " style="color: Black">Write your comment:</textarea>
                        </div>                     
                        <div class="fright poster">
                            <asp:Button ID="ComSend" runat="server" class="post" Text="Post Comment" OnClick="ComSend_Click" />
                        </div>
                       
                        <asp:UpdateProgress ID="updateProgress" runat="server">
                            <ProgressTemplate>
                               <div style="position: absolute; height: 180px; width: 440px; text-align: center;
                                    z-index: 9999999;  padding-top:140px; padding-left:30px">
                                    <asp:Image ID="loadGif" runat="server" ImageUrl="/_layouts/images/edf/Customloading.gif" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <p>
            <!------------   APP/REJ BUTTONS          -------------------------->
            <div runat="server" id="AppRejDiv" class="reject">
                <asp:Button ID="ButtonCancel" runat="server" Text="Reject Request" CssClass="button button1"
                    Style="margin-right: 10px" OnClick="ButtonSubmit_Click" />
                <asp:Button ID="ButtonSubmit" runat="server" Text="Approve Request" CssClass="subbmit subbmit1"
                    OnClick="ButtonSubmit_Click" />
            </div>
            <div runat="server" id="ProvideDiv" class="reject" visible="false">
                <asp:CheckBox ID="Ch_ITSE" runat="server" Text="ITSE" Visible="false" />
                <asp:CheckBox ID="Ch_OfficeIT" runat="server" Text="Office IT" Visible="false" />
                <asp:CheckBox ID="Ch_BILLING" runat="server" Text="BILLING" Visible="false" />
                <asp:CheckBox ID="Ch_REPORTING" runat="server" Text="REPORTING" Visible="false" />
                <asp:Button ID="ButtonProvide" runat="server" Text="Provided" Width="100%" CssClass="subbmit subbmit1"
                    OnClick="ButtonProvide_Click" />
            </div>
            <div class="reject">
                <asp:Label ID="Label1" runat="server" Text="" />
            </div>
            <!--------------------------       HISTORY       -------------------------->
            <div class="content_left fright">
                <div class="verj">
                    <br />
                    <ul id="ul_history" runat="server">
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <div class="back back1">
        <a href="#">
            <p>
                Back to top</p>
        </a>
    </div>
</div>
