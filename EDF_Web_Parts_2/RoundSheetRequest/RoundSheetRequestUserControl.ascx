<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoundSheetRequestUserControl.ascx.cs" Inherits="EDF_Web_Parts_2.RoundSheetRequest.RoundSheetRequestUserControl" %>

<style type="text/css">
    
table tr td
{
    vertical-align: top;
    padding: 5px;
}
.info
{
    margin-left:12px;
}    
.val
{
    font-size:16px;
    color:Red;    
}


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
        <li class="cont clearfix" style="cursor:default"><div style="background: url(/_catalogs/masterpage/images/info.png) no-repeat 28px;" class="img_logo fleft"><div class="fleft" ><img class="avatar" runat="server" id="autorImg" /></div></div><p class="text4 fleft" style="margin-top: 16px;">Round Sheet Request</p><div class="timer fright"></div></li>
        </ul>
        <ul class="fill">
        
        <li class="fill_li first"><p><b><center>Round sheet / Շրջիկ թերթիկ</center></b></p></li>
        
        <li><p><span>This is to certify that the employee doesn’t owe to the Company at the moment of leaving the Company.</span></p></li>
        <li><p><span>Սույն տեղեկանքը տրվում է առ այն, որ աշխատակիցն ընկերությունից ազատվելու պահին ընկերության նկատմամբ պարտք չունի:</span></p></li>

        <li class="fill_li clearfix">
           <div runat="server" id = "Div1">
            
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    First name, last name / Աշխատողի անուն, ազգանուն
                                </td>
                                <td>
                                     <b class="info"><label runat="server" id="lb_Name"></label></b>

                                     <br />
                                    
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Father’s name / Հայրանուն
                                </td>
                                <td>
                                     <textarea runat="server" id = "ta_father" class="edit_text " 
                                            style="min-height:0px; color:Black;">Write Father’s Name:</textarea>
                                    
                                </td>                                    
                                <td>
                                    <b class="info val"><label runat="server" id="val_father">*</label></b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Department and position / Բաժին և պաշտոն
                                </td>
                                <td>
                                     <b class="info"><label runat="server" id="lb_Department"></label></b>
                                     <b class="info"><label runat="server" id="lb_Position"></label></b>
                                     <br />
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Windows account user name/ Windows օգտագործողի անուն:
                                </td>
                                <td>
                                     <b class="info"><label runat="server" id="lb_windows_account"></label> </b>
                                     <br />
                                    
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Cboss user name (if any) / Cboss օգտատերի անուն (եթե առկա է)
                                </td>
                                <td>
                                     <textarea runat="server" id = "ta_cboss_user" class="edit_text " 
                                            style="min-height:0px; color:Black;">Write Cboss user name:</textarea> 
                                     <br />
                                        
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Mobile number / Բջջային
                                </td>
                                <td>
                                     <textarea runat="server" id = "ta_phone" class="edit_text " 
                                            style="min-height:0px; color:Black;">Write Phone Number:</textarea>
                                     <b class="info"><label runat="server" id="lb_phone"></label></b>
                                                                         
                                </td>
                                <td>                                
                                    <b class="info val"><label runat="server" id="val_Phone">*</label></b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Private mobile number / Անձնական բջջային 
(this number is required in order to contact you, in case of need/ տվյալ համարը պահանջվում է հարկ եղած դեպքում Ձեզ հետ կապ հաստատելու համար)
                                </td>
                                <td>
                                     <textarea runat="server" id = "ta_Private_phone" class="edit_text " 
                                            style="min-height:0px; color:Black;">Write Private Phone Number:</textarea> 

                                </td>
                                <td>
                                                                   <b class="info val"><label runat="server" id="val_pr_phone">*</label></b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Last working day / Վերջին աշխատանքային օրը
                                </td>
                                <td>
                                        <div class="request">
                                            <ul class="sort clearfix" >
                                                <li class="clearfix fleft">
                                                    <asp:TextBox runat="server" id="tb_last_work_day" Text="Select date" 
                                                        CssClass="datepicker" />
                                                </li>
                                            </ul>
                                        </div>
                                        <br />
                                     <b class="info val"><label runat="server" id="val_date"></label></b>
                                <asp:Label ID="datevalid" runat="server" Text=""/>

                                </td>
                            </tr>
                            <tr>
                                <td>
                                <div class="check">
                                    <asp:CheckBox ID="ChStep3" runat="server" Text="Please keep my accesses till"/>
                                </div>
                                </td>
                                <td>
                                        <div class="request">
                                            <ul class="sort clearfix" >
                                                <li class="clearfix fleft">
                                                    <asp:TextBox runat="server" id="dateIT" Text="Select date" CssClass="datepicker" />
                                                </li>
                                            </ul>
                                        </div>
                                        <br />
                                     <b class="info val"><label runat="server" id="val_dateAccTill"></label></b>
                                <asp:Label ID="Label2" runat="server" Text=""/>

                                </td>
                            </tr>                                   
                        </tbody>
                    </table>
            </div>
            </li>
        </ul>
    </div>
    <br />
            
<div class="last_form">
    <asp:Button ID="ButtonCancel" runat="server" Text="Cancel and Clear Form" 
        CssClass="button" style="margin-right:10px" onclick="ButtonCancel_Click" />
    <asp:Button ID="ButtonSubmit" runat="server" Text="Submit Form" CssClass="subbmit" 
        onclick="ButtonSubmit_Click"/>
</div>