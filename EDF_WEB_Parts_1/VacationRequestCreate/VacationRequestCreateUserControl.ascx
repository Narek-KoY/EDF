<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VacationRequestCreateUserControl.ascx.cs" Inherits="EDF_WEB_Parts_1.VacationRequestCreate.VacationRequestCreateUserControl" %>



<style>
.my_Date{
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
    <ul >
        <li class="cont clearfix" style="cursor:default"><div style="background: url(/_catalogs/masterpage/images/leave.png) no-repeat 28px;" class="img_logo fleft"><div class="fleft" ><img class="avatar" runat="server" id="autorImg" /></div></div><p class="text4 fleft" style="margin-top: 16px;">Vacation Request</p><div class="timer fright"></div></li>
        </ul>
        <ul class="fill">
        
        <li class="fill_li first"><p><b><center>Application for vacation / Արձակուրդի դիմում</center></b></p></li>
        
        <li class="fill_li clearfix">
            <div>
            
            <p style="margin-bottom:35px"><span>1.   I would like to ask vacation for / Խնդրում եմ հատկացնել ինձ արձակուրդ</span></p>
            
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton0" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            <label>Annual / Ամենամյա </label>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan="2" style="padding-bottom:10px" >
                             <br />
                <label><b>Special purpose / Նպատակային <b/></label>
                        </td>
                        <td>
                           
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton1" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           <label>Pregnancy and maternity / Հղիություն և ծննդաբերություն </label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton2" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           <label>Non-paid / Մինչև 3 տարեկան երեխայի խնամքի համար տրամադրվող - չվճարվող </label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton3" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           <label>Exam (non-paid) / Քննություն - չվճարվող</label>
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
                        <td colspan="2" style="padding-bottom:10px" >
                             <label><b>Internal OAM / Ընկերության կողմից տրամադրվող </b></label>
                        </td>
                        <td>
                           
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton4" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           <label>Marriage (non-paid) / Ամուսնություն - չվճարվող </label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton5" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                />
                        </td>
                        <td>
                           <label>Close relative’s death (husband, wife, mother, father, child,brother, sister) (paid) / Մոտ բարեկամի մահ` ամուսին,կին,մայր,հայր,երեխա,քույր, եղբայր - վճարվող  </label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton6" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           <label>Relative’s death (non-paid) / Բարեկամի մահ`տատ,պապ - չվճարվող</label>
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
                            <asp:RadioButton ID="RadioButton7" runat="server" CssClass="radio" style="margin-left:20px" GroupName="A1" oncheckedchanged="RadioButton0_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           <label>Non-paid vacation / չվճարվող </label>
                        </td>
                    </tr>
                </tbody>
            </table>
              <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton0" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton1" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton2" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton3" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton4" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton5" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton6" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID = "RadioButton7" EventName="CheckedChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <label style="color:Red" runat="server" id="ValRadio" visible="false">Please check any radio button!</label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </li>

        <li class="fill_li clearfix">
            <div>
                <p style="margin-bottom:35px"><span>2.  Vacation duration / արձակուրդի ժամկետը </span></p>
        
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                </asp:ScriptManagerProxy>
                

                 <div class="request">
                	<ul class="sort clearfix" >
                        <li class="clearfix fleft">
                            <asp:TextBox runat="server" id="dateTimeControl1" Text="Select start date" 
                                CssClass="datepicker" AutoPostBack="True"/>
                                <span style="color:Red">*</span>
                        </li>
                        <li class="clearfix fright" >                            
                            <asp:TextBox runat="server" id="dateTimeControl2" Text="Select end date" 
                                CssClass="datepicker" AutoPostBack="True"/>
                                <span style="color:Red">*</span>
                        </li>
                    </ul>
                </div>
                <br />
                <asp:Label ID="datevalid" runat="server" Text=""></asp:Label>
                <br />
                <div runat="server" id="CheckDiv">
                    <label><p>In case of shift work, please specify your day-offs by checking boxes with days of the week / խնդրում ենք նշել Ձեր հանգստյան օրերը՝ ընտրելով շաբաթվա տվյալ օրը/օրերը </p></label>

                    <table width="100%">
                        <tbody>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="CheckBox1" runat="server" class="checkbox" /><label>Monday / Երկուշաբթի</label>
                                </td> 
                                <td>
                                    <asp:CheckBox ID="CheckBox5" runat="server" CssClass="checkbox" /><label>Friday / Ուրբաթ</label>
                                </td>
                            </tr>
                            <tr>
                                <td class="checkbox">
                                     <asp:CheckBox ID="CheckBox2" runat="server" /><label>Tuesday / Երեքշաբթի</label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="CheckBox6" runat="server" /><label>Saturday / Շաբաթ</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="CheckBox3" runat="server" /><label>Wednesday / Չորեքշաբթի</label>
                                </td> 
                                <td>
                                   <asp:CheckBox ID="CheckBox7" runat="server" /><label>Sunday / Կիրակի</label>
                                </td> 
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="CheckBox4" runat="server" /><label>Thursday / Հինգշաբթի</label>
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
            <p style="margin-bottom:35px"><span>3.   Who will replace / ո՞վ է փոխարինելու</span> (Please check mentioned name with “Check name”  button / խնդրում ենք ստուգել նշված անունը՝ սեղմելով «Ստուգել անունը» կոճակը) </p>
           
            <label>Name and surname / անուն, ազգանուն </label>      
            <br />
            <SharePoint:PeopleEditor ID="spPeoplePicker" runat="server" Width="350" SelectionSet="User" />
            
        </div>
        </li>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID = "dateTimeControl1" EventName="TextChanged" />        
        <asp:AsyncPostBackTrigger ControlID = "dateTimeControl2" EventName="TextChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton0" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton1" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton2" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton3" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton4" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton5" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton6" EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID = "RadioButton7" EventName="CheckedChanged" />
    </Triggers>
<ContentTemplate>
        <li class="fill_li clearfix" runat="server" id="paymentLi">
        <div>
            I would like to receive the payment for vacation together with my salary / Խնդրում եմ փոխանցել արձակուրդայինս աշխատավարձի հետ միասին
            <asp:CheckBox ID="CheckBox8" runat="server" Enabled="False" Checked="True" />
            <br />
            <p style="font-size:12px">To be noted, that the payment for vacation is paid with the salary, if application is submitted less than 8 calendar days before vacation period / Հարկ է նշել, որ  արձակուրդայինը փոխանցվում է աշխատավարձից առանձին, միայն եթե  դիմումը ներկայացվել է արձակուրդից առնվազն ութ օր առաջ:</p>
        </div>
        </li>
</ContentTemplate>
</asp:UpdatePanel>

    </ul>
</div>

<div class="last_form">
    <asp:Button ID="ButtonCancel" runat="server" Text="Cancel and Clear Form" 
        CssClass="button" style="margin-right:10px" onclick="ButtonCancel_Click"
       />
    <asp:Button ID="ButtonSubmit" runat="server" Text="Submit Form" CssClass="subbmit" 
        onclick="ButtonSubmit_Click"/>
</div>
