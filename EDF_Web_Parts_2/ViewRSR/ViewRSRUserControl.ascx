<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewRSRUserControl.ascx.cs"
    Inherits="EDF_Web_Parts_2.ViewRSR.ViewRSRUserControl" %>
<script language="javascript" type="text/javascript">

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
        window.frames["print_frame"].window.focus();
        window.frames["print_frame"].window.print();
    }

</script>
<style>
    .load_pdf
    {
        border: 1px solid;
        padding: 8px;
    }
    .info
    {
        padding: 16px;
        min-width: 240px;
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
                        <div style="background: url(/_catalogs/masterpage/images/info.png) no-repeat 28px;"
                            class="img_logo fleft">
                            <div class="fleft">
                                <img class="avatar" runat="server" id="autorImg" /></div>
                        </div>
                        <p class="text4 fleft" style="margin-top: 16px;">
                            Round sheet
                        </p>
                        <div class="timer fright">
                            <img runat="server" id="cert_icon" width="40" src="/_catalogs/masterpage/images/kindredmail-certificate-icon.png" />
                        </div>
                    </li>
                </ul>
                <ul class="fill">
                    <li class="fill_li first">
                        <p>
                            <b>
                                <center>
                                    Round sheet / Շրջիկ թերթիկ</center>
                                <span runat="server" id="OrderSpan" visible="False">
                                    <center>
                                        Order N / Հրաման</center>
                                    <center>
                                        Ա<asp:Label ID="OrderLabel" runat="server" Text="0" /></center>
                                </span></b>
                        </p>
                    </li>
                    <li>
                        <p>
                            <span>This is to certify that the employee doesn’t owe to the Company at the moment
                                of leaving the Company.</span></p>
                    </li>
                    <li>
                        <p>
                            <span>Սույն տեղեկանքը տրվում է առ այն, որ աշխատակիցն ընկերությունից ազատվելու պահին
                                ընկերության նկատմամբ պարտք չունի:</span></p>
                        <br />
                    </li>
                    <div runat="server" id="Div1">
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        First name, last name / Աշխատողի անուն, ազգանուն
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Name">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Father’s name / Հայրանուն
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_father">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Department and position / Բաժին և պաշտոն
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_Department_Position">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Windows account user name/ Windows օգտագործողի անուն:
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_windows_account">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr id="tr_cboss" runat="server">
                                    <td>
                                        Cboss user name (if any) / Cboss օգտատերի անուն (եթե առկա է)
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_cboss">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Mobile number / Բջջային
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_phone">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Private mobile number / Անձնական բջջային (this number is required in order to contact
                                        you, in case of need/ տվյալ համարը պահանջվում է հարկ եղած դեպքում Ձեզ հետ կապ հաստատելու
                                        համար)
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_pr_phone">
                                            </label>
                                        </b>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Last working day / Վերջին աշխատանքային օրը
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="lb_date">
                                            </label>
                                        </b>
                                    </td>
                                </tr>
                                <tr runat="server" id="ItDirTR">
                                    <td>
                                        Access till
                                    </td>
                                    <td class="info">
                                        <b>
                                            <label runat="server" id="AccessTill">
                                            </label>
                                        </b>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    </li>
                </ul>
            </div>
            <div runat="server" id="PrintDiv" class="loader" visible="False">
                <asp:LinkButton ID="PrintLink" runat="server" OnClick="PrintLink_Click"><p class="load">P R I N T</p></asp:LinkButton>
            </div>
            <div runat="server" id="DivPDF" visible="False">
                <div class="loader">
                    <asp:LinkButton ID="LBPDF" runat="server" OnClick="LBPDF_Click"><p class="load">Download PDF</p></asp:LinkButton>
                </div>
                <div class="loader" runat="server" id="LBDSPDFDIV">
                    <asp:LinkButton ID="LBDSPDF" runat="server" OnClick="LBDSPDF_Click"><p class="load">Download signed PDF</p></asp:LinkButton>
                </div>
                <div class="loader">
                    <asp:FileUpload ID="filePDF" runat="server" />
                    <asp:LinkButton ID="UploadPDF" runat="server" OnClick="UploadPDF_Click"><span class="load load_pdf">Upload PDF</span></asp:LinkButton>
                </div>
            </div>
        </div>
        <!------------   COMMENT          -------------------------->
        <div class="content_left fright">
            <div class="sale1">
                <p>
                    Comments:</p>
            </div>
            <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
            </asp:ScriptManagerProxy>
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
            <div runat="server" id="AppRejDiv" class="reject">
                <asp:Button ID="ButtonCancel" runat="server" Text="Reject Request" CssClass="button button1"
                    Style="margin-right: 10px" OnClick="ButtonSubmit_Click" />
                <asp:Button ID="ButtonSubmit" runat="server" Text="Approve Request" CssClass="subbmit subbmit1"
                    OnClick="ButtonSubmit_Click" />
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
