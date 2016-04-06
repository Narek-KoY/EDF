<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InternationalTravelOrderUserControl.ascx.cs" Inherits="EDF_Web_Parts_2.InternationalTravelOrder.InternationalTravelOrderUserControl" %>


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
    .val
    {
        color:Red;
    }
    .tb
    {
        min-height:0px; 
        color:Black;
        padding: 0px 10px 0 10px;
        height: 40px;
        margin-top: 5px;
    }
    
</style>

<div class="right_content">
    <ul >
        <li class="cont clearfix" style="cursor:default"><div style="background: url(/_catalogs/masterpage/images/international_travel.png) no-repeat 28px;" class="img_logo fleft"><div class="fleft" ><img class="avatar" runat="server" id="autorImg" /></div></div><p class="text4 fleft" style="margin-top: 16px;">International travel order</p><div class="timer fright"></div></li>
        </ul>
        <ul class="fill">
        
        <li class="fill_li first"><p><b><center>International travel order / Միջազգային գործուղման հրաման</center><%--<center>Order N / Հրաման</center><center>1</center>--%></b></p></li>
        
        <li class="fill_li clearfix">
            <div>
        
            <p><span>1.  Route (city, region) 1 / Ուղղություն (քաղաք, երկիր):</span></p>
            <br />
           
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                </asp:ScriptManagerProxy>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode=Conditional>
                    <ContentTemplate> 
                    <ul runat = "server" id = "ul_Rout"></ul>
                        <asp:HiddenField ID="hf_Rout_Count" runat="server" Value="0" />
                        <asp:HiddenField ID="hf_Rout" runat="server" />
                        <div class="editing">                       
                            <asp:TextBox  runat="server" ID = "ta_Rout" CssClass="edit_text tb">Write your route:</asp:TextBox>
                            <label runat = "server" id = "val1" class="val">*</label>                            
                        </div>
                        <div >                
                            <asp:Button ID="AddRoute" runat="server" class="post" Text="Add Route" 
                                style="height: 26px" onclick="AddRoute_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br/>

                <!----------  INVITING -------------->

            <p><span>  Inviting organization 1 (optional) / Հրավիրող կազմակերպություն 1 (ոչ պարտադրիր):</span></p>
            <br />
           
                <asp:UpdatePanel ID="UP_Inviting" runat="server" UpdateMode=Conditional>
                    <ContentTemplate> 
                    <ul runat = "server" id = "ul_Inviting"></ul>
                        <asp:HiddenField ID="hf_invit_count" runat="server" Value="0" />
                        <asp:HiddenField ID="hf_invit" runat="server" />
                        <div class="editing">                       
                            <asp:TextBox  runat="server" ID = "ta_Inviting" CssClass="edit_text tb" >Write Your Inviting Organization:</asp:TextBox>
                        </div>
                        <div >                
                            <asp:Button ID="AddInvit" runat="server" class="post" Text="Add" 
                                style="height: 26px" onclick="AddInvit_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br/>
               

            <p><span>2.   Dates of travel / Գործուղման ամսաթիվ:</span></p>
                <br/>      
            <div class="request">
                <ul class="sort clearfix" >
                    <li class="clearfix fleft">
                        <asp:TextBox runat="server" id="tb_Start_Date" Text="Select start date" 
                            CssClass="datepicker" />
                            <span style="color:Red">*</span>
                    </li>
                    <li class="clearfix fright" >                            
                        <asp:TextBox runat="server" id="tb_End_Date" Text="Select end date" 
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
                            <asp:RadioButton ID="rb_Purpose_Training" runat="server" CssClass="radio" 
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
                            <asp:RadioButton ID="rb_Purpose_Conference" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            Conference / Համաժողով
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_Purpose_Workshop" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            Workshop / Սեմինար
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_Purpose_Other" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="A1" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                            Other / Այլ &nbsp
                            
                            <div id="div_other" runat="server" class="editing"  visible="False">                       
                                <asp:TextBox  runat="server" ID = "tb_Purpose_other" CssClass="edit_text tb" >Write your purpose:</asp:TextBox>
                                <asp:Label ID="lb_val_other_pur" CssClass=val runat="server" Text="*"></asp:Label>
                                <br />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression = "^{0,100}$" runat="server" ErrorMessage="Maximum 100 characters allowed." ControlToValidate="tb_Purpose_other"></asp:RegularExpressionValidator>
                            </div>
                        </td>
                    </tr> 
                </tbody>
            </table>
               </ContentTemplate>
                </asp:UpdatePanel>

            <br />
            <br />

            <!--------   Budgeted      ------------------->

            <p><span>4.   Budgeted / Բյուջետավորված:</span></p>
            <br />
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_budget_Yes" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="budget" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True"/>
                        </td>
                        <td>
                            Yes / Այո
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_budget_No" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="budget" 
                                oncheckedchanged="RadioButton_CheckedChanged"/>
                        </td>
                        <td>
                            No / Ոչ 
                        </td>
                    </tr>                     
                </tbody>
            </table>
                  
            <br />
            <br />

            <!--------  Costs    ------------------->
            <p><span>  Costs covered by the inviter (optional) / Ծախսերը, որոնք կատարվում են հրավիրող կողմի միջոցներով (ոչ պարտադրիր):</span></p>
            <br />

            Amount / Գումար 
            <asp:TextBox ID="tb_Costs" runat="server" width="100px">0</asp:TextBox>
        
            <label runat = "server" id = "val3" style="color:Red"></label>                            

            <br />
            <br />
            <!--------  Who Will Replace    ------------------->

        <div>
            <p style="margin-bottom:35px"><span>5.   Who will replace / ո՞վ է փոխարինելու</span> (Please check mentioned name with “Check name”  button / խնդրում ենք ստուգել նշված անունը՝ սեղմելով «Ստուգել անունը» կոճակը) </p>
           
            <label>Name and surname / անուն, ազգանուն </label>      
            <br />
            <SharePoint:PeopleEditor ID="spPeoplePicker" runat="server" Width="350" SelectionSet="User" />
            
        </div>

            <!--------   DAILY ------------------->
                
            <br />
            <br />

            <p><span>6.   Daily allowance / Օրապահիկ: </span></p>
            <br />
            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_Daily_Yes" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="Daily" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True"/>
                        </td>
                        <td>
                            Requested / Պահանջվում է
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_Daily_No" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="Daily" 
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

            <!----------   HOTEL ----------------------->

            <p><span>7.  Hotel / Հյուրանոց:  </span></p>
            <br />
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode=Conditional>
            <ContentTemplate>

            <table>
                <tbody>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_Hotel_Yes" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="hotel" 
                                oncheckedchanged="RadioButton_CheckedChanged" Checked="True" 
                                AutoPostBack="True"/>
                        </td>
                        <td>
                             Required / Պահանջվում է
                        </td>
                    </tr> 
                    <tr>
                        <td>
                            <asp:RadioButton ID="rb_Hotel_No" runat="server" CssClass="radio" 
                                style="margin-left:20px" GroupName="hotel" 
                                oncheckedchanged="RadioButton_CheckedChanged" AutoPostBack="True"/>
                        </td>
                        <td>
                             Not required / Չի պահանջվում
                        </td>
                    </tr>                     
                </tbody>
            </table>
            <br />

            <div runat="server" id = "HotelDetails">

                    <p><span>  Hotel details / Հյուրանոցի տվյալներ:  </span></p>
                    <br />
            
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    Name / Անուն
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_Hotel_Name" CssClass="edit_text tb" >Write Hotel Name:</asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lb_val_hname" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Dates of stay / Մնալու ժամանակահատված
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_Hotel_Dates" CssClass="edit_text tb" >Write Hotel Dates:</asp:TextBox> 
                                </td>
                                <td>
                                    <asp:Label ID="lb_val_hdate" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Location / Վայր
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_Hotel_Location" CssClass="edit_text tb" >Write Hotel Location:</asp:TextBox> 
                                </td>
                                <td>
                                    <asp:Label ID="lb_val_hloc" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Phone number / Հեռախոս
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_Hotel_Phone" CssClass="edit_text tb" >Write Hotel Phone Number:</asp:TextBox> 
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Payment method / Վճարման ձև
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_Hotel_Payment" CssClass="edit_text tb" >Write Hotel Payment method:</asp:TextBox> 
                                </td>
                                <td>
                                </td>
                            </tr>                
                        </tbody>
                    </table>
                <label runat = "server" id = "Val2" style="color:Red"></label>                            

            </div>

            <br />
            </ContentTemplate>
    </asp:UpdatePanel>

            <div runat="server" id = "FlightDetails">
                    <p><span>9.   Flight details / Թռիչքի տվյալներ:	</span></p>
                    <br />

        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AddFlightDetails"/>
        </Triggers>
            <ContentTemplate>

                <asp:HiddenField ID="HF_Date" runat="server" />
                <asp:HiddenField ID="HF_Airline" runat="server" />
                <asp:HiddenField ID="HF_Number" runat="server" />
                <asp:HiddenField ID="HF_DepartureCity" runat="server" />
                <asp:HiddenField ID="HF_DestinationCity" runat="server" />
                
                <div runat="server" id="DivFlightDetails"/>
             <br />
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    Date / Ամսաթիվ
                                </td>
                                <td>
                                    <div class="request">
                                        <ul class="sort clearfix" >
                                            <li class="clearfix fleft">
                                                <asp:TextBox runat="server" id="tb_fl_date" Text="Select flight date" 
                                                    CssClass="datepicker" />
                                            </li>
                                        </ul>
                                    </div>

                                </td>
                                <td>
                                    <asp:Label ID="lb_val_fdate" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Airline / Ավիաընկերություն
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_fl_airline" CssClass="edit_text tb" >Write airline:</asp:TextBox>
                                </td>                                
                                <td>
                                    <asp:Label ID="lb_val_fair" runat="server" CssClass="val" Text="*"/>                                    
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Flight number / Թռիչքի համար
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_fl_number" CssClass="edit_text tb">Write flight number:</asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lb_val_fnum" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr> 
                            <tr>
                                <td>
                                    Departure city / Մեկնման քաղաք
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_fl_dep_city" CssClass="edit_text tb" >Write departure city:</asp:TextBox>
                                             
                                </td>
                                <td>
                                    <asp:Label ID="lb_val_fdepc" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Destination city / Ժամանման քաղաք
                                </td>
                                <td>
                                     <asp:TextBox  runat="server" id = "ta_fl_dest_city" CssClass="edit_text  tb" >Write destination city:</asp:TextBox> 
                                </td>
                                <td>
                                    <asp:Label ID="lb_val_fdesc" runat="server" CssClass=val Text="*"/>                                    
                                </td>
                            </tr>                
                        </tbody>
                    </table>
                <label runat = "server" id = "FlyVal" style="color:Red"></label>      
                <asp:Button ID="AddFlightDetails" runat="server" CssClass="post" Text="Add" 
                        style="height: 26px; float:right" onclick="AddFlightDetails_Click"/>       
            </ContentTemplate>
        </asp:UpdatePanel>

            </div>
        
            <!---------    HOTEL DETAILS ----------------->
            
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