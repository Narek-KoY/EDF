<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VacationRequestViewUserControl.ascx.cs"
    Inherits="EDF_WEB_Parts_1.VacationRequestView.VacationRequestViewUserControl" %>
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
            <ul>
                <li style="cursor: default" class="cont clearfix" id="avatar_div">
                    <div style="background: url(/_catalogs/masterpage/images/leave.png) no-repeat 28px;"
                        class="img_logo fleft">
                        <img class="avatar" runat="server" id="userImg" /></div>
                    <p class="text4 fleft" style="margin-top: 16px;">
                        <asp:Label ID="LblType1" runat="server" Text="Type Name" /></p>
                    <div class="timer fright">
                        <img runat="server" id="cert_icon" width="40" src="/_catalogs/masterpage/images/kindredmail-certificate-icon.png" /></div>
                </li>
            </ul>
            <ul class="fill">
                <li class="fill_li first">
                    <p>
                        <b>
                            <center>
                                Application for vacation / Արձակուրդի դիմում</center>
                            <span runat="server" id="OrderSpan">
                                <center>
                                    Order N / Հրաման</center>
                                <center>
                                    Ա<asp:Label ID="OrderLabel" runat="server" Text="0" /></center>
                            </span></b>
                    </p>
                </li>
                <li class="fill_li clearfix">
                    <div>
                        <p style="margin-bottom: 35px">
                            <span>1. I would like to ask vacation for / Խնդրում եմ հատկացնել ինձ արձակուրդ</span></p>
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton0" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Checked="true" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Annual / Ամենամյա
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="padding-bottom: 10px">
                                        <br />
                                        <label>
                                            <b>Special purpose / Նպատակային <b />
                                        </label>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton1" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Pregnancy and maternity / Հղիություն և ծննդաբերություն
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton2" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Non-paid / Մինչև 3 տարեկան երեխայի խնամքի համար տրամադրվող - չվճարվող
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton3" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Exam (non-paid) / Քննություն - չվճարվող</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="padding-bottom: 10px">
                                        <label>
                                            <b>Internal OAM / Ընկերության կողմից տրամադրվող </b>
                                        </label>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton4" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Marriage (non-paid) / Ամուսնություն - չվճարվող
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton5" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Close relative’s death (husband, wife, mother, father, child,brother, sister) (paid)
                                            / Մոտ բարեկամի մահ` ամուսին,կին,մայր,հայր,երեխա,քույր, եղբայր - վճարվող
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton6" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Relative’s death (non-paid) / Բարեկամի մահ`տատ,պապ - չվճարվող</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="RadioButton7" runat="server" CssClass="radio" Style="margin-left: 20px"
                                            GroupName="A1" Enabled="False" />
                                    </td>
                                    <td>
                                        <label>
                                            Non-paid vacation / չվճարվող
                                        </label>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </li>
                <li class="fill_li clearfix">
                    <div>
                        <p style="margin-bottom: 35px">
                            <span>2. Vacation duration / արձակուրդի ժամկետը </span>
                        </p>
                        <label>
                            From / սկսած
                            <asp:Label ID="dateFrom" runat="server" Text="dd/mm/yyyy" /></label>
                        <br />
                        <label>
                            To / մինչև
                            <asp:Label ID="dateTo" runat="server" Text="dd/mm/yyyy" />
                            (including / ներառյալ)</label>
                        <br />
                        <div runat="server" id="CheckDiv">
                            <label>
                                <p>
                                    In case of shift work, please specify your day-offs by checking boxes with days
                                    of the week / խնդրում ենք նշել Ձեր հանգստյան օրերը՝ ընտրելով շաբաթվա տվյալ օրը/օրերը
                                </p>
                            </label>
                            <table width="100%">
                                <tbody>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="CheckBox1" runat="server" class="checkbox" Enabled="False" /><label>Monday
                                                / Երկուշաբթի</label>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="CheckBox5" runat="server" CssClass="checkbox" Enabled="False" /><label>Friday
                                                / Ուրբաթ</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="checkbox">
                                            <asp:CheckBox ID="CheckBox2" runat="server" Enabled="False" /><label>Tuesday / Երեքշաբթի</label>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="CheckBox6" runat="server" Enabled="False" /><label>Saturday / Շաբաթ</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="CheckBox3" runat="server" Enabled="False" /><label>Wednesday / Չորեքշաբթի</label>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="CheckBox7" runat="server" Enabled="False" /><label>Sunday / Կիրակի</label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="CheckBox4" runat="server" Enabled="False" /><label>Thursday / Հինգշաբթի</label>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </li>
                <li class="fill_li clearfix">
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
                        <SharePoint:PeopleEditor ID="spPeoplePicker" runat="server" Width="350" SelectionSet="User"
                            Enabled="False" />
                    </div>
                </li>
                <li class="fill_li clearfix" runat="server" id="divPayment">
                    <div>
                        I would like to receive the payment for vacation together with my salary / Խնդրում
                        եմ փոխանցել արձակուրդայինս աշխատավարձի հետ միասին
                        <asp:CheckBox ID="CheckBox8" runat="server" Enabled="False" />
                        <br />
                        <p style="font-size: 12px">
                            To be noted, that the payment for vacation is paid with the salary, if application
                            is submitted less than 8 calendar days before vacation period / Հարկ է նշել, որ
                            արձակուրդայինը փոխանցվում է աշխատավարձից առանձին, միայն եթե դիմումը ներկայացվել
                            է արձակուրդից առնվազն ութ օր առաջ:</p>
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
                    Style="margin-right: 10px" OnClientClick="updateValidator();" OnClick="ButtonSubmit_Click" />
                <asp:Button ID="ButtonSubmit" runat="server" Text="Approve Request" CssClass="subbmit subbmit1"
                    OnClick="ButtonSubmit_Click" />
            </div>
            <div class="reject">
                <asp:Label ID="Label1" runat="server" Text="" />
            </div>
            <script language="javascript" type="text/javascript">
                function updateValidator() {
                    var rfvSubject = document.getElementById('RequiredFieldValidatorWorkDays');
                    ValidatorEnable(rfvSubject, false);
                }
            </script>
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
