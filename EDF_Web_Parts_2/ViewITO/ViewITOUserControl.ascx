<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewITOUserControl.ascx.cs"
    Inherits="EDF_Web_Parts_2.ViewITO.ViewITOUserControl" %>
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
    table tr td:first-child
    {
        width: 350px;
        padding: 5px;
    }
    #date tr td:first-child
    {
        width: 150px;
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
                <asp:Label ID="LabelName" runat="server" Text="User Name" />
            </a>from
            <asp:Label ID="LabelDep" runat="server" Text="User Departament" />
        </p>
    </div>
    <div class="Content_inner clearfix">
        <div id="printablediv" class="right_content content_right fleft">
            <div class="right_content">
                <ul>
                    <li class="cont clearfix" style="cursor: default">
                        <div style="background: url(/_catalogs/masterpage/images/travel.png) no-repeat 28px;"
                            class="img_logo fleft">
                            <div class="fleft">
                                <img class="avatar" runat="server" id="autorImg" /></div>
                        </div>
                        <p class="text4 fleft" style="margin-top: 16px;">
                            International travel order
                        </p>
                        <div class="timer fright">
                            <img runat="server" id="cert_icon" width="40" src="/_catalogs/masterpage/images/kindredmail-certificate-icon.png" /></div>
                    </li>
                </ul>
                <ul class="fill">
                    <li class="fill_li first">
                        <p>
                            <b>
                                <center>
                                    International travel order / Միջազգային գործուղման հրաման</center>
                                <span runat="server" id="OrderSpan">
                                    <center>
                                        Order N / Հրաման</center>
                                    <center>
                                        Գ<asp:Label ID="OrderLabel" runat="server" Text="0" /></center>
                                </span></b>
                        </p>
                    </li>
                    <li class="fill_li clearfix">
                        <!----------  ROUTE  -------------->
                        <p>
                            <span>1. Route (city, region) 1 / Ուղղություն (քաղաք, երկիր):</span></p>
                        <br />
                        <ul runat="server" id="ul_Rout">
                        </ul>
                        <br />
                        <!----------  INVITING -------------->
                        <p>
                            <span>2. Inviting organization 1 (optional) / Հրավիրող կազմակերպություն 1 (ոչ պարտադրիր):</span></p>
                        <br />
                        <ul runat="server" id="ul_Inviting">
                        </ul>
                        <br />
                        <p>
                            <span>3. Dates of travel / Գործուղման ամսաթիվ:</span></p>
                        <br />
                        <div class="request">
                            <table id="date">
                                <tr>
                                    <td>
                                        From / սկսած
                                    </td>
                                    <td style="padding-left: 15px;">
                                        <label>
                                            <asp:Label ID="lb_DateFrom" runat="server" Text="dd/mm/yyyy" /></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        To / մինչև
                                    </td>
                                    <td style="padding-left: 15px;">
                                        <label>
                                            <asp:Label ID="lb_DateTo" runat="server" Text="dd/mm/yyyy" />
                                            (including / ներառյալ)</label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <br />
                        <br />
                        <p>
                            <span>4. Purpose / Գործուղման նպատակը:</span></p>
                        <br />
                        <label id="lb_Purpose" runat="server" style="margin-left: 20px">
                        </label>
                        <br />
                        <br />
                        <!--------   Budgeted      ------------------->
                        <p>
                            <span>5. Budgeted / Բյուջետավորված:</span></p>
                        <br />
                        <label id="lb_Budgeted" runat="server" style="margin-left: 20px">
                        </label>
                        <br />
                        <br />
                        <!--------  Costs    ------------------->
                        <p>
                            <span>6. Costs covered by the inviter (optional) / Ծախսերը, որոնք կատարվում են հրավիրող
                                կողմի միջոցներով (ոչ պարտադրիր):</span></p>
                        <br />
                        <label id="lb_Costs" runat="server" style="margin-left: 20px">
                        </label>
                        <br />
                        <br />
                        <!--------  Who Will Replace    ------------------->
                        <div>
                            <p style="margin-bottom: 35px">
                                <span>3. Who will replace / ո՞վ է փոխարինելու</span> (Please check mentioned name
                                with “Check name” button / խնդրում ենք ստուգել նշված անունը՝ սեղմելով «Ստուգել անունը»
                                կոճակը)
                            </p>
                            <label>
                                Name and surname / անուն, ազգանուն
                            </label>
                            <br />
                            <asp:HiddenField ID="HiddenFieldspPeoplePicker" runat="server" />
                            <SharePoint:PeopleEditor ID="spPeoplePicker" runat="server" Width="350" SelectionSet="User" />
                        </div>
                        <!--------   DAILY ------------------->
                        <br />
                        <br />
                        <p>
                            <span>7. Daily allowance / Օրապահիկ: </span>
                        </p>
                        <br />
                        <label id="lb_Daily" runat="server" style="margin-left: 20px">
                        </label>
                        <br />
                        <br />
                        <p>
                            <span>8. Hotel / Հյուրանոց: </span>
                        </p>
                        <br />
                        <label id="lb_Hotel" runat="server" style="margin-left: 20px">
                        </label>
                        <br />
                        <br />
                        <!---------    HOTEL DETAILS ----------------->
                        <div runat="server" id="HotelDiv">
                            <p>
                                <span>Hotel details / Հյուրանոցի տվյալներ: </span>
                            </p>
                            <br />
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            Name / Անուն
                                        </td>
                                        <td>
                                            <label id="lb_Hotel_Name" runat="server" style="margin-left: 20px">
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Dates of stay / Մնալու ժամանակահատված
                                        </td>
                                        <td>
                                            <label id="lb_Hotel_Dates" runat="server" style="margin-left: 20px">
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Location / Վայր
                                        </td>
                                        <td>
                                            <label id="lb_Hotel_Location" runat="server" style="margin-left: 20px">
                                            </label>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="TRphon">
                                        <td>
                                            Phone number / Հեռախոս
                                        </td>
                                        <td>
                                            <label id="lb_Hotel_Phone" runat="server" style="margin-left: 20px">
                                            </label>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="TRpayment">
                                        <td>
                                            Payment method / Վճարման ձև
                                        </td>
                                        <td>
                                            <label id="lb_Hotel_Payment" runat="server" style="margin-left: 20px">
                                            </label>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <br />
                        <!--------------    FLYING    --------------->
                        <div runat="server" id="Div1">
                            <p>
                                <span>9. Flight details / Թռիչքի տվյալներ: </span>
                            </p>
                            <br />
                            <div runat="server" id="DivFlightDetails">
                            </div>
                        </div>
                    </li>
                    <li class="fill_li clearfix" style="display: none;" runat="server" id="WorkDaysCount">
                        Working days / աշխատանքային օրեր
                        <asp:TextBox ID="TextBoxWorkDays" runat="server" Text="0" Width="60"></asp:TextBox>
                        &nbsp;
                        <asp:Button CssClass="save_btn" ID="ButtonWorkDays" runat="server" Text="S A V E"
                            OnClick="ButtonWorkDays_Click" /><br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorWorkDays" ControlToValidate="TextBoxWorkDays"
                            runat="server" ErrorMessage="Enter working days count"></asp:RequiredFieldValidator>
                    </li>
                </ul>
            </div>
            <div runat="server" id="PrintDiv" class="loader" visible="False">
                <%--<input type="button" value="Print 1st Div" onclick="javascript:printDiv('printablediv')" />--%>
                <asp:LinkButton ID="PrintLink" runat="server" OnClick="PrintLink_Click"><p class="load">P R I N T</p></asp:LinkButton>
                <%--<a href="javascript:printDiv('printablediv')"><p class="load">P R I N T</p></a>--%>
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
