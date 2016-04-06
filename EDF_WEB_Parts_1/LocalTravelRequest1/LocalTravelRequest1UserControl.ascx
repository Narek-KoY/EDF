<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocalTravelRequest1UserControl.ascx.cs" Inherits="EDF_WEB_Parts_1.LocalTravelRequest1.LocalTravelRequest1UserControl" %>


<style>
.val
{
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
        <li class="cont clearfix" style="cursor:default"><div style="background: url(/_catalogs/masterpage/images/travel.png) no-repeat 28px;" class="img_logo fleft"><div class="fleft" ><img class="avatar" runat="server" id="autorImg" /></div></div><p class="text4 fleft" style="margin-top: 16px;">Local Travel Request</p><div class="timer fright"></div></li>
        </ul>
        <ul class="fill">
        
        <li class="fill_li first"><p><b><center>Local travel order / Տեղական գործուղման հրաման</center></b></p></li>
        
        <li class="fill_li clearfix">
            <div>
        
            <p><span>1.  Route (city, region) 1 / Ուղղություն (քաղաք, երկիր):</span></p>
            <br />
           
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                </asp:ScriptManagerProxy>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode=Conditional>
                    <ContentTemplate> 
                    <ul runat = "server" id = "RoutUL"></ul>
                        <asp:HiddenField ID="HiddenFieldRoutCount" runat="server" Value="0" />
                        <asp:HiddenField ID="HiddenFieldRout" runat="server" />
                        <div class="editing">                       
                            <textarea runat="server" id = "RouteText" class="edit_text " style="min-height:0px; color:Black;">Write your route:</textarea>
                            <asp:Label ID="lb_val_route" CssClass=val runat="server" Text="*"></asp:Label>
                        </div>
                        <div >                
                            <asp:Button ID="AddRoute" runat="server" class="post" Text="Add route" 
                                style="height: 26px" onclick="AddRoute_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br/>
               

            <p><span>2.   Dates of travel / Գործուղման ամսաթիվ:</span></p>
                <br/>      
            <div class="request">
                <ul class="sort clearfix" >
                    <li class="clearfix fleft">
                        <asp:TextBox runat="server" id="dateTimeControl1" Text="Select start date" 
                            CssClass="datepicker" />
                            <span style="color:Red">*</span>
                    </li>
                    <li class="clearfix fright" >                            
                        <asp:TextBox runat="server" id="dateTimeControl2" Text="Select end date" 
                            CssClass="datepicker" />
                            <span style="color:Red">*</span>
                    </li>
                </ul>
            </div>
            <asp:Label ID="datevalid" runat="server" Text=""/>
            <br />
            <br />

            <p><span>3.   Purpose / Գործուղման նպատակը:</span></p>
            <br />
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode=Conditional>
                <ContentTemplate>
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton1" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True" 
                                AutoPostBack="True"/>
                        </td>
                        <td>
                            Training / Դասընթաց
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton2" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            Conference / Համաժողով
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton3" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            Workshop / Սեմինար
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton4" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            Other / Այլ &nbsp
                            
                            <div id="div_other" runat="server" visible=false class="editing">                       
                                <textarea runat="server" id = "OtherPurpose" class="edit_text " 
                                    style="min-height:0px; color:Black;">Write your purpose:</textarea>
                                <asp:Label ID="lb_val_other_pur" runat="server" CssClass=val Text="*"></asp:Label>
                                <br />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"  ValidationExpression = "^{0,100}$" runat="server" ErrorMessage="Maximum 100 characters allowed." ControlToValidate="OtherPurpose"></asp:RegularExpressionValidator>

                            </div>
                        </td>
                    </tr> 
                </tbody>
            </table>
               </ContentTemplate>
                </asp:UpdatePanel>
                
            <br />
            <br />

            <p><span>4.   Daily allowance / Օրապահիկ: </span></p>
            <br />
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton5" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A2" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True"/>
                        </td>
                        <td>
                            Requested / Պահանջվում է
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton6" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A2" 
                                oncheckedchanged="RadioButton_CheckedChanged"/>
                        </td>
                        <td>
                            Not requested / Չի պահանջվում 
                        </td>
                    </tr>                     
                </tbody>
            </table>
                  
            <br />
            <br />

            <p><span>5.  Transportation / Փոխադրամիջոց: </span></p>
            <br />
            <p> &nbsp Car / Մեքենա:  </p>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode=Conditional>
        <ContentTemplate>
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton8" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A3" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True" 
                                AutoPostBack="True" />
                        </td>
                        <td>
                           Yes / Այո
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton9" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A3" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                           No / Ոչ 
                        </td>
                    </tr>                     
                </tbody>
            </table>

            <div runat="server" id = "TimeDiv">
                Time  / Ժամ<br />
                &nbsp;&nbsp;&nbsp;&nbsp; hh&nbsp;  
                <asp:TextBox ID="TextBoxhh" runat="server" Width="50px">00</asp:TextBox>
                &nbsp;&nbsp;&nbsp; mm
                <asp:TextBox ID="TextBoxmm" runat="server" Width="50px">00</asp:TextBox>
            </div>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="&nbsp;&nbsp;0 <= hh < 24" MinimumValue="0" MaximumValue="23" Type="Integer" ControlToValidate="TextBoxhh"></asp:RangeValidator>
            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="&nbsp;&nbsp;&nbsp;&nbsp;  0 <= mm <60" MinimumValue="0" MaximumValue="59" Type="Integer" ControlToValidate="TextBoxmm"></asp:RangeValidator>
        </ContentTemplate>
    </asp:UpdatePanel>
            <br />
            <br />

            <p><span>6.  Hotel / Հյուրանոց:  </span></p>
            <br />
            
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton10" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A4" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True"/>
                        </td>
                        <td>
                             Required / Պահանջվում է 
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="RadioButton11" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A4" 
                                oncheckedchanged="RadioButton_CheckedChanged"/>
                        </td>
                        <td>
                             Not required / Չի պահանջվում
                        </td>
                    </tr>                     
                </tbody>
            </table>

            </li>
        </ul>
    </div>
            
<div class="last_form">
    <asp:Button ID="ButtonCancel" runat="server" Text="Cancel and Clear Form" 
        CssClass="button" style="margin-right:10px" onclick="ButtonCancel_Click"
       />
    <asp:Button ID="ButtonSubmit" runat="server" Text="Submit Form" CssClass="subbmit" 
        onclick="ButtonSubmit_Click"/>
</div>

<!---------------------------        HISTORY       ----------------->

<div class="content_left fright">
        <div class="verj">
            <br />
            <ul id="ul_history" runat="server">
            </ul>
        </div>
    </div>
