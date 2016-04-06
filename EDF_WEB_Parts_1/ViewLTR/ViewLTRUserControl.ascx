<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewLTRUserControl.ascx.cs" Inherits="EDF_WEB_Parts_1.ViewLTR.ViewLTRUserControl" %>


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
            //window.frames["print_frame"].document.getElementById('avatar_div').innerHTML = "";
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
    <iframe id="print_frame" name="print_frame" style="display:none"></iframe>
	<div id="Content">
    
        <div class="back back2">
        	<p>Pending <span>Request:
            </span> 
                <asp:Label ID="LblType" runat="server" Text="Type Name"/> <asp:Label ID="Label2" runat="server" Text=""/> by <a runat="server" id="UserProf"  target="_blank"><asp:Label ID="LabelName" runat="server" Text="User Name"/></a> from <asp:Label ID="LabelDep" runat="server" Text="User Departament"/></p>
        </div>
    	<div class="Content_inner clearfix">



<div id="printablediv" class="right_content content_right fleft">
    <ul >
        <li class="cont clearfix" style="cursor:default"><div style="background: url(/_catalogs/masterpage/images/travel.png) no-repeat 28px;" class="img_logo fleft"><div class="fleft" ><img class="avatar" runat="server" id="autorImg" /></div></div><p class="text4 fleft" style="margin-top: 16px;">Local Travel Request</p><div class="timer fright"><img runat="server" id="cert_icon" width="40" src="/_catalogs/masterpage/images/kindredmail-certificate-icon.png" /></div></li>
        </ul>
        <ul class="fill">
        
        <li class="fill_li first"><p><b><center>Local travel order / Տեղական գործուղման հրաման</center><span runat="server" id="OrderSpan"><center>Order N / Հրաման</center><center><span>
            Գ</span><asp:Label ID="OrderLabel" runat="server" Text="0"/></center></span></b></p></li>
        
        <li class="fill_li clearfix">
            <div>
        
            <p><span>1.  Route (city, region) 1 / Ուղղություն (քաղաք, երկիր):</span></p>
            <br />
                    <ul runat = "server" id = "RoutUL"></ul>
                    
                <br/>
               

            <p><span>2.   Dates of travel / Գործուղման ամսաթիվ:</span></p>
                <br/>      
           
                <ul  >
                    <li>
                        <table>
                        <tr>
                            <td>From / սկսած</td>
                            <td style="padding-left:20px;"><label>    <asp:Label ID="dateFrom" runat="server" Text="dd/mm/yyyy"/></label>  </td>
                        </tr>
                        <tr>
                            <td>To / մինչև</td>
                            <td style="padding-left:20px;"><label>      <asp:Label ID="dateTo" runat="server" Text="dd/mm/yyyy"/>(including / ներառյալ)</label>   </td>
                        </tr>
                        </table>
                    </li> 
                </ul>
            
            <br /><br />

            <p><span>3.   Purpose / Գործուղման նպատակը:</span></p>
            <br />
            <label ID="LablePurpose" runat="server"  style="margin-left:20px" ></label>                 
            <br /><br />
            <p><span>4.   Daily allowance / Օրապահիկ: </span></p>
            <br />
            <label ID="LableDaily" runat="server" style="margin-left:20px" ></label>
                   
            <br /><br />
            <p><span>5.  Transportation / Փոխադրամիջոց: </span></p>
            <br />
            <p> &nbsp Car / Մեքենա:  </p>    
          
            <div runat="server" id = "TimeDiv">
                
               <label ID="LabelCar" runat="server" style="margin-left:20px" > </label>
            </div>
       
            <br /><br />
            <p><span>6.  Hotel / Հյուրանոց:  </span></p>
            <br />
                <label ID="LabelHotel" runat="server" style="margin-left:20px" ></label>
            </li>
        </ul>

    
    <div runat="server" id="PrintDiv" class="loader" visible="False">
  <%--<input type="button" value="Print 1st Div" onclick="javascript:printDiv('printablediv')" />--%>
       <asp:LinkButton ID="PrintLink" runat="server" onclick="PrintLink_Click"><p class="load">P R I N T</p></asp:LinkButton>
    
    <%--<a href="javascript:printDiv('printablediv')"><p class="load">P R I N T</p></a>--%>
</div>
<div runat="server" id="DivPDF" visible="False">
    <div class="loader">
        <asp:LinkButton ID="LBPDF" runat="server" onclick="LBPDF_Click"><p class="load">Download PDF</p></asp:LinkButton>
    </div>
    <div class="loader" runat="server" id="LBDSPDFDIV">
        <asp:LinkButton ID="LBDSPDF" runat="server" onclick="LBDSPDF_Click"><p class="load">Download signed PDF</p></asp:LinkButton>
    </div>
    <div class="loader">
        <asp:FileUpload ID="filePDF" runat="server" />
        <asp:LinkButton ID="UploadPDF" runat="server" onclick="UploadPDF_Click" ><span class="load load_pdf">Upload PDF</span></asp:LinkButton>
    </div>
</div>
</div>
<!------------   COMMENT          -------------------------->
            <div class="content_left fright">

                <div class="sale1">
                	<p>Comments:</p>
                </div>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
                </asp:ScriptManagerProxy>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                        <div runat="server" id = "divCom"></div>
                        <div class="editor clearfix">
                   	        <div class="fleft">
                                <img class="avatar" runat="server" id="comImg" />
                            </div>
                            <div class="editing fright">                       
                                <textarea runat="server" id = "ComText" class="edit_text " style="color:Black">Write your comment:</textarea>
                            </div>
                            <div class="fright poster">                
                                <asp:Button ID="ComSend" runat="server" class="post" Text="Post Comment" onclick="ComSend_Click" />
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
                    <asp:Button ID="ButtonCancel" runat="server" Text="Reject Request" CssClass="button button1" style="margin-right:10px" onclick="ButtonSubmit_Click" />
                    <asp:Button ID="ButtonSubmit" runat="server" Text="Approve Request" CssClass="subbmit subbmit1" onclick="ButtonSubmit_Click" />
                </div>

                <div class="reject">
                    <asp:Label ID="Label1" runat="server" Text="" />
                    
                </div>
                



    <div class="content_left fright">

    	<!--div class="right_top" style="margin:30px 0 10px 0;">
            <p>Pending for approvales</p>
        </div-->

        <div class="verj">
            <br />
                
            <ul id="ul_history" runat=server>
            </ul>

        </div>
    </div>

                      <%--  <div class="reject clearfix">
                            <asp:Button ID="ComReject" runat="server" class="button button1" style="margin-right:10px" Text="Reject Request" />
                            <asp:Button ID="ComApprove" runat="server" Text="Approve Request" class="subbmit subbmit1" />
                        </div>--%>

                    
               <%-- <div class="right_top" style="margin:30px 0 10px 0;">
                	<p>Pending <span>Requests:</span></p>
                </div>--%>

                <%--<div class="verj">
                	<ul>
                    	<li><a href="#"><div class="fleft"><img src="images/avatar8.png" /></div><p class="text4 width fleft">George Chilingaryan<span> from </span>Public Relations Department <span>wrote:</span><br /><h9>18.04.2014 | <span>13:21 PM</span></h9></p></a></li>
                        <li><a href="#"><div class="fleft"><img src="images/avatar7.png" /></div><p class="text4 width fleft">Evelina Ter-Vardanyan <span> from </span>PR Department  <span>wrote:</span><br /><h9>18.04.2014 | <span>13:21 PM</span></h9></p></a></li>
                        <li><a href="#"><div class="fleft"><img src="images/avatar5.png" style="margin-right:30px" /></div><p class="text4 fleft">Varujan Manukyan<span> from </span>IT Depart <span>wrote:</span><br /><h9>18.04.2014 | <span>13:21 PM</span></h9></p></a></li>
                        <li><a href="#"><div class="fleft"><img src="images/avatar3.png" /></div><p class="text4 width fleft">Volodya Meliksetyan<span> from </span>HR Management Department <span>wrote:</span><br /><h9>18.04.2014 | <span>13:21 PM</span></h9></p></a></li>
                        <li><a href="#"><div class="fleft"><img src="images/avatar8.png" /></div><p class="text4 width fleft">George Chilingaryan<span> from </span>Public Relations Department <span>wrote:</span><br /><h9>18.04.2014 | <span>13:21 PM</span></h9></p></a></li>
                    </ul>
                </div>--%>
                                    
            </div>            
        </div>
        <div class="back back1">
        	<a href="#"><p>Back to top</p></a>
        </div>
    </div>
