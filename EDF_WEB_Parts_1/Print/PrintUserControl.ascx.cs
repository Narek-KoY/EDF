using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using EDF_CommonData;
using EDF_WEB_Parts_1.VacationRequestView;
using iTextSharp.text;
using Microsoft.SharePoint;


namespace EDF_WEB_Parts_1.Print
{
    public partial class PrintUserControl : UserControl
    {
        public static string url1 = string.Empty, url2 = string.Empty;
        string innerHtml = string.Empty;
        public static string htmlTextPDF = string.Empty;
        string Request_ID = string.Empty;
        public static string style = @"
                            .header{
                            color: #7EBC0A;
                            font-size: 16.0pt;
                            font-weight: 700;
                            font-family: ""Orange Armenian 35"", sans-serif;
                            mso-font-charset: 0;
                            text-align: center;
                            vertical-align: 121;
                            background: white;
                            mso-pattern: black none;
                            white-space: normal;
                            }
                            .content{
                            width: 750px;
                            margin: 0 auto;
                            border: 1px solid #ebebeb;
                            padding: 10px;
                            }
                            ul li{
                            list-style-type:none;
                            }";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["rid"] == null) Response.Redirect(SPContext.Current.Web.Url);

            bool view2 = Request.QueryString["view"] == "2";
            bool view3 = Request.QueryString["view"] == "3";

            Request_ID = Request.QueryString["rid"];

            url1 = string.Format("{0}/SitePages/Print.aspx?rid={1}&view=2", SPContext.Current.Web.Url, Request_ID);
            url2 = string.Format("{0}/SitePages/Print.aspx?rid={1}&view=3", SPContext.Current.Web.Url, Request_ID);


            int typeId = Request_type.GetId(Request_ID);

            switch (typeId)
            {
                case 1:
                    innerHtml = DrawPageVR(Request_ID);
                    break;
                case 2:
                    if (view2)
                        innerHtml = DrawPageLRT1(Request_ID);
                    else if (view3)
                        innerHtml = DrawPageLTR2(Request_ID);
                    else
                        innerHtml = DrawPageLRT(Request_ID);
                    break;
                case 3:
                    innerHtml = DrawPageITO(Request_ID);
                    break;
                case 4:
                    innerHtml = DrawPageRSR(Request_ID);
                    break;
            }

            printDiv.InnerHtml = innerHtml;

            // createPDF(innerHtml);
        }
        static string DrawPageVR(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            VacationRequestViewUserControl obj = new VacationRequestViewUserControl();

            string order = obj.GetVacationNumber(Request_ID);
            Autor_id = obj.GetAutor_id(Request_ID);
            DataTable table = obj.LableType(Request_ID);

            string type = string.Empty;

            type = table.Rows[0]["Vacation_type"].ToString().Contains("Annual") ? "Ամենամյա / Annual" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Pregnancy") ? "Հղիություն և ծննդաբերություն / Pregnancy and maternity" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Non-paid") ? "Մինչև 3 տարեկան երեխայի խնամքի համար տրամադրվող - չվճարվող / Non-paid" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Exam") ? "Քննություն - չվճարվող / Exam (non-paid)" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Marriage") ? "Ամուսնություն - չվճարվող / Marriage (non-paid)" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Close") ? "Մոտ բարեկամի մահ` ամուսին,կին,մայր,հայր,երեխա,քույր, եղբայր - վճարվող / Close relative’s death (husband, wife, mother, father, child,brother, sister) (paid)" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Relative’s death") ? "Բարեկամի մահ`տատ,պապ - չվճարվող / Relative’s death (non-paid)" : type;
            type = table.Rows[0]["Vacation_type"].ToString().Contains("Non-paid vacation") ? "Չվճարվող / Non-paid vacation" : type;
            //date
            string dateFrom = " " + DateTime.Parse(table.Rows[0]["Start_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);// ToShortDateString();
            string dateTo = " " + DateTime.Parse(table.Rows[0]["End_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            //

            string substitut = obj.GetRequest_Substitute_User_Names(Request_ID, Autor_id);

            if (substitut.Length > 0)
                substitut = "<p><b>3. Ո՞վ է փոխարինելու / Who will replace</b>  </p>" + substitut;


            // approve Reject

            List<EDF_User> muser = EDF.AssociationUsers(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);
            apprej += "<li>HR Administration department Director Marine Aznauryan ____________<br/></li>";
            apprej += "</ul>";

            string days = string.Empty;
            foreach (string day in table.Rows[0]["Days_offs"].ToString().Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                switch (day)
                {
                    case "1":
                        days += "<t/>Monday / Երկուշաբթի";
                        break;
                    case "2":
                        days += "<t/>Tuesday / Երեքշաբթի";
                        break;
                    case "3":
                        days += "<t/>Wednesday / Չորեքշաբթի";
                        break;
                    case "4":
                        days += "<t/>Thursday / Հինգշաբթի";
                        break;
                    case "5":
                        days += "<t/>Friday / Ուրբաթ";
                        break;
                    case "6":
                        days += "<t/>Saturday / Շաբաթ";
                        break;
                    case "7":
                        days += "<t/>Sunday / Կիրակի";
                        break;
                }
                days += "<br/>";

            }
            if (string.IsNullOrEmpty(table.Rows[0]["Days_offs"].ToString()))
                days = string.Empty;
            else
                days = "<p>Խնդրում ենք նշել Ձեր հանգստյան օրերը՝ ընտրելով շաբաթվա տվյալ օրը/օրերը / In case of shift work, please specify your day-offs by checking boxes with days of the week</p>" + days;
            // approve Reject

            string pay_terms = string.Empty;
            if (table.Rows[0]["Payment_terms"].ToString().ToLower() == "true")
            {
                pay_terms = "<p>Խնդրում եմ փոխանցել արձակուրդայինս աշխատավարձի հետ միասին / I would like to receive the payment for vacation together with my salary</p>";
            }

            try
            {
                User = AD.GetUserByLogin(Autor_id);

                htmlText = string.Format(
                        @"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <!DOCTYPE html 
                                PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""
                            ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                        <head><style>{8}</style><title>PRINT</title></head>
                            <body>
                        <div class=""content"">
                        <div class=""header"">
                        <p>Արձակուրդի դիմում / Application for vacation</p>                                                                      
                        <p>Հրաման / Order N Ա{0} / {13}</p>
                        </div>                         
                        <span style=""width: 120px;float: right;height: 60px;margin-top: -90px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span>
                        <br/></br><span Style='text-decoration: underline;'>{10} , {11}</span></br>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                        </br>Name of employee and title</br></br>Լրացնելու ամսաթիվ: <span Style='text-decoration: underline;'>{9}</span>
                        </br>filling date  </br> <p><b>1. Խնդրում եմ հատկացնել ինձ արձակուրդ / I would like to ask vacation / vacation for </b></p>{1}<br/>
                        <p><b>2. Արձակուրդի ժամկետը / Vacation duration</b></p>
                        <t/>Սկսած / From {2}<br/><t/>Մինչև / To /  {3}  (ներառյալ / including)<br/> {4} {5} {6} <br/>
                        Աշխատանքային օրեր / Working days <b>{12}</b><br/><p><b>Հաստատված է/approved by</b></p>{7}
                        </div></body></html>",
                        order,          //0
                        type,           //1
                        dateFrom,       //2 
                        dateTo,         //3
                        days,           //4
                        substitut,      //5 
                        pay_terms,      //6
                        apprej,         //7
                        style,          //8
                        DateTime.Parse(table.Rows[0]["Add_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        User.FullName,
                        User.Department,
                        obj.getWorkDaysCount(Request_ID),
                        Approve_reject.getLastApproveDate(Request_ID).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                htmlText = "ERROR" + ex.Message;
            }
            return htmlText;
        }
        static string DrawPageLRT1(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            VacationRequestViewUserControl obj = new VacationRequestViewUserControl();

            string order = obj.GetLTRNumber(Request_ID);
            Autor_id = obj.GetAutor_id(Request_ID);
            DataTable table = LTR.Get(Request_ID);

            string Rout = table.Rows[0]["City"].ToString();
            string place = string.Empty;
            foreach (string r in Rout.Split(new string[] { "|$$|" }, StringSplitOptions.None))
            {
                if (r.Length < 1) continue;
                place += string.Format("{0}, ", r);
            }
            place.Substring(0, place.Length - 1);

            string dateFrom = " " + DateTime.Parse(table.Rows[0]["Start_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);// ToShortDateString();
            string dateTo = " " + DateTime.Parse(table.Rows[0]["End_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<EDF_User> muser = EDF.AssociationUsersLTR(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);
            apprej += string.Format("<li>Francoise Caye from Finance ____________________ </br></br></li>");
            apprej += "</ul>";

            string days = string.Empty;
            string pay_terms = string.Empty;

            try
            {
                User = AD.GetUserByLogin(Autor_id);
                htmlText = string.Format(
                        @"<?xml version=""1.0"" encoding=""UTF-8""?>
                        <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                        <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                        <head><style>{8}</style><title>PRINT</title></head>
                        <body><div class=""content""><div class=""header""><p>Գործուղման հրաման <br/> Travel order  <br/> {12}</p></div>
                        <span style=""width: 120px;float: right;height: 60px;margin-top: -90px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span><br/>     
                        </br><span Style='text-decoration: underline;'>{10} , {11}</span>
                        </br>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                        </br>Name of employee and title</br>                           
                        </br>Գործուղվում է <span Style='text-decoration: underline;'>{1}</span>
                        </br>(գործուղման վայրը, կազմակերպությունը)
                        </br>Place and name of company
                        </br>
                        </br>Հրաման N <span Style='text-decoration: underline;'>Գ{0}</span>
                        </br>Travel order No.
                        </br>
                        </br>Ամսաթիվ <span Style='text-decoration: underline;'>{9}</span>  թ.
                        </br>Date
                        </br>
                        </br>Գործուղման նպատակը <span Style='text-decoration: underline;'>{5}</span>
                        </br>Purpose of business trip
                        </br>
                        </br>Գործուղման ժամկետը 		<span Style='text-decoration: underline;'>{2}</span>  թ. 	մինչեւ <span Style='text-decoration: underline;'>{3}</span>  թ.
                        </br>Duration of business trip		     days	up to
                        </br> 
                        {4} {6} <p><b>Հաստատված է/approved by</b></p> {7}
                        </div>
                        </body>
                        </html>",
                                    order,              // 0
                                    place,              // 1
                                    dateFrom,           // 2
                                    dateTo,             // 3
                                    days,               // 4
                                    table.Rows[0]["Purpose"].ToString(),    // 5
                                    pay_terms,          // 6
                                    apprej,             // 7
                                    style,              // 8
                                    DateTime.Parse(table.Rows[0]["Filling_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),// 9
                                    User.FullName,      // 10
                                    User.Department,     // 11            
                                    Approve_reject.getLastApproveDate(Request_ID).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) //12
                                );
            }
            catch (Exception ex)
            {
                htmlText = "PRINT ERROR view2: " + ex.Message;
            }
            return htmlText;
        }
        static string DrawPageLTR2(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            VacationRequestViewUserControl obj = new VacationRequestViewUserControl();

            string order = obj.GetLTRNumber(Request_ID);
            Autor_id = obj.GetAutor_id(Request_ID);
            DataTable table = LTR.Get(Request_ID);

            string Rout = table.Rows[0]["City"].ToString();
            string place = string.Empty;
            foreach (string r in Rout.Split(new string[] { "|$$|" }, StringSplitOptions.None))
            {
                if (r.Length < 1) continue;
                place += string.Format("{0}, ", r);
            }
            place.Substring(0, place.Length - 1);

            string dateFrom = " " + DateTime.Parse(table.Rows[0]["Start_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);// ToShortDateString();
            string dateTo = " " + DateTime.Parse(table.Rows[0]["End_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


            int nights = DateTime.Parse(table.Rows[0]["End_Date"].ToString()).Subtract(DateTime.Parse(table.Rows[0]["Start_Date"].ToString())).Days;

            List<EDF_User> muser = EDF.AssociationUsersLTR(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);
            apprej += string.Format("<li>Francoise Caye from Finance ____________________ </br></br></li>");
            apprej += "</ul>";

            string days = string.Empty;
            string pay_terms = string.Empty;

            try
            {
                User = AD.GetUserByLogin(Autor_id);
                style += "  table { border-collapse:collapse; }table td, table th { border:1px solid black;padding:5px; }";
                htmlText = string.Format(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
    <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
    <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
    <head><style>{8}</style><title>PRINT</title></head>
    <body><div class=""content""><div class=""header""><p>Daily allowance request form</p> </div>
    <span style=""width: 120px;float: right;height: 60px;margin-top: -60px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span>
    <br/><table style=""width:100%""><tr><td>No.</td><td>{0}</td><td>Date</td><td>{9}</td></tr><tr><td>Requestor's name</td><td colspan=""3"">{10}
    </td></tr><tr><td>Department</td><td colspan=""3"">{11}</td></tr><tr><td>Route</td><td colspan=""3"">{1}</td></tr><tr><td>Dates of travel
    </td><td colspan=""3"">from: {2}  to: {3}</td></tr><tr><td>Number of nights</td><td colspan=""3"">{14}</td></tr><tr><td>Number of days</td>
    <td colspan=""3"">{13}</td></tr><tr><td>Purpose </td><td colspan=""3"">{15}</td></tr><tr><td>Country </td><td colspan=""3"">Armenia</td></tr>
    </table><br/><br/><table style=""width:100%""><tr><td colspan=""6"">Daily Allowance:</td></tr><tr><th>Description</th><th>Ammount - for other (AMD)
    </th><th>Number of days / nights / other expenses</th><th>Cash advance ammount (AMD)</th><th>Bank transfer ammount (AMD)</th><th>Total expenses (AMD)
    </th></tr><tr><td>Per diem country 1 (AMD)</td><td>7.000</td><td>{13}</td><td>{16}</td><td></td><td>{16}</td></tr><tr><td colspan=""3"" style=""text-align: right;"">Total amount</td>
    <td>{16}</td><td>0.00</td><td>{16}</td></tr></table><br/><table style=""width:100%""><tr><th>Validation </th><th>Name</th><th>Signature	</th>
    </tr><tr><td>Chief Accountant</td><td>{17}</td><td></td></tr><tr><td>CFO / controlling	</td><td>{18}</td><td></td></tr></table></div></body></html>",
                    order,              // 0
                    place.Trim().TrimEnd(','),              // 1
                    dateFrom,           // 2
                    dateTo,             // 3
                    days,               // 4
                    table.Rows[0]["Purpose"].ToString(),    // 5
                    pay_terms,          // 6
                    apprej,             // 7
                    style,              // 8
                    DateTime.Parse(table.Rows[0]["Filling_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),// 9
                    User.FullName,      // 10
                    User.Department,     // 11            
                    Approve_reject.getLastApproveDate(Request_ID).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), //12
                    nights + 1, //13
                    nights, //14
                    table.Rows[0]["Purpose"].ToString(),   //15
                    (nights + 1) * 7000, //16
                    AD.GetGroupUsers("EDF_ITR-LTR_Chief_accountant")[0].FullName, //17
                    AD.CFO.FullName   //18
                );
            }
            catch (Exception ex)
            {
                htmlText = "PRINT ERROR view3: " + ex.Message;
            }
            return htmlText;
        }
        static string DrawPageLRT(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            VacationRequestViewUserControl obj = new VacationRequestViewUserControl();

            string order = obj.GetLTRNumber(Request_ID);
            Autor_id = obj.GetAutor_id(Request_ID);
            DataTable table = LTR.Get(Request_ID);

            string Rout = table.Rows[0]["City"].ToString();
            string place = string.Empty;
            foreach (string r in Rout.Split(new string[] { "|$$|" }, StringSplitOptions.None))
            {
                if (r.Length < 1) continue;
                place += string.Format("{0}, ", r);
            }
            place.Substring(0, place.Length - 1);
            string dateFrom = " " + DateTime.Parse(table.Rows[0]["Start_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);// ToShortDateString();
            string dateTo = " " + DateTime.Parse(table.Rows[0]["End_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<EDF_User> muser = EDF.AssociationUsersLTR(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);
            apprej += "</ul>";

            string days = string.Empty;
            string pay_terms = string.Empty;

            try
            {
                User = AD.GetUserByLogin(Autor_id);

                htmlText = string.Format(
                      @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                    <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                    <head><style>{8}</style><title>PRINT</title>   <script>window.open('{12}')</script> <script>window.open('{13}')</script></head>
                    <body><div class=""content""><div class=""header""><p>Գործուղման վկայական <br/> Travel certificate</p></div>
                    <span style=""width: 120px;float: right;height: 60px;margin-top: -70px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span><br/>     
                    {4}  {6}
                    <br/><span Style='text-decoration: underline;'>{10} , {11}</span>
                    <br/>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                    <br/>Name of employee and title
                    <br/>                              
                    <br/>Գործուղվում է <span Style='text-decoration: underline;'>{1}</span>
                    <br/>(գործուղման վայրը, կազմակերպությունը)
                    <br/>Place and name of company
                    <br/>
                    <br/>Հրաման N <span Style='text-decoration: underline;'>Գ{0}</span>
                    <br/>Travel order No.
                    <br/>
                    <br/>Ամսաթիվ <span Style='text-decoration: underline;'>{9}</span>  թ.
                    <br/>Date
                    <br/>
                    <br/>Գործուղման նպատակը <span Style='text-decoration: underline;'>{5}</span>
                    <br/>Purpose of business trip
                    <br/>
                    <br/>Գործուղման ժամկետը 		____ oր 	մինչեւ <span Style='text-decoration: underline;'>{3}</span>  թ.
                    <br/>Duration of business trip		     days	up to
                    <br/>
                    <br/>
                    <br/>մեկնել է ______________________ ժամանել է _______________________
                    <br/>Departure				    Arrival
                    <br/>____  ______________ {14}  թ.             ____  ______________ {14}  թ.
                    <br/>
                    <br/>                       Կ.Տ.                                	              Կ.Տ.
                    <br/>         stamp					stamp
                    <br/>        ____________________                       ____________________
                    <br/>          (uտորագրությունը)                          (uտորագրությունը)
                    <br/>                   signature 	                                           signature
                    <br/>
                    <br/>մեկնել է ______________________ ժամանել է ________________________
                    <br/>Departure 				Arrival
                    <br/>     ____  ______________ {14} թ.             ____  ______________ {14} թ.
                    <br/>
                    <br/> Կ.Տ.___________________________________        Կ.Տ ----------------------------------------------------
                    <br/> (կամ պետական գրանցման վկայականի համարը)  (կամ պետական գրանցման վկայականի համարը)
                    <br/>state registration number 			      state registration number
                    <br/>
                    <br/>____________________                       ____________________
                    <br/>         			 (uտորագրությունը)                                  (uտորագրությունը)
                    <br/> 				signature 	                                          signature
                    </div></body></html>",
                                    order,              // 0
                                    place,              // 1
                                    dateFrom,           // 2
                                    dateTo,             // 3
                                    days,               // 4
                                    table.Rows[0]["Purpose"].ToString(),    // 5
                                    pay_terms,          // 6
                                    apprej,             // 7
                                    style,              // 8
                                    DateTime.Parse(table.Rows[0]["Filling_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),// 9
                                    User.FullName,      // 10
                                    User.Department,     // 11            
                                    url1,                    //12
                                    url2,                    //13
                                    DateTime.Now.Year   //14
                                );

                htmlTextPDF = string.Format(
                                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                <head><style>{8}</style><title>PRINT</title></head><body><font face = ""arial unicode ms""><div><div>
                <span Style=""color:#F60; font-size: 16pt; font-weight:700; text-align:center; white-space:normal;""><p>Գործուղման վկայական <br/> Travel certificate</p></span>                            
                </div><br/> {4}{6}
                <br/><span Style='text-decoration: underline;'>{10} , {11}</span>
                <br/>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                <br/>Name of employee and title
                <br/>                              
                <br/>Գործուղվում է <span Style='text-decoration: underline;'>{1}</span>
                <br/>(գործուղման վայրը, կազմակերպությունը)
                <br/>Place and name of company
                <br/>
                <br/>Հրաման N <span Style='text-decoration: underline;'>Գ{0}</span>
                <br/>Travel order No.
                <br/>
                <br/>Ամսաթիվ <span Style='text-decoration: underline;'>{9}</span>  թ.
                <br/>Date
                <br/>
                <br/>Գործուղման նպատակը <span Style='text-decoration: underline;'>{5}</span>
                <br/>Purpose of business trip
                <br/>
                <br/>Գործուղման ժամկետը 		____ oր 	մինչեւ <span Style='text-decoration: underline;'>{3}</span>  թ.
                <br/>Duration of business trip		     days	up to
                <br/>
                <br/>
                <br/>մեկնել է ______________________ ժամանել է _______________________
                <br/>Departure				    Arrival
                <br/>____  ______________ {13}  թ.             ____  ______________ {13}  թ.
                <br/>
                <br/>                       Կ.Տ.                                	              Կ.Տ.
                <br/>         stamp					stamp
                <br/>        ____________________                       ____________________
                <br/>          (uտորագրությունը)                          (uտորագրությունը)
                <br/>                   signature 	                                           signature
                <br/>
                <br/>մեկնել է ______________________ ժամանել է ________________________
                <br/>Departure 				Arrival
                <br/>     ____  ______________ {13} թ.             ____  ______________ {13} թ.
                <br/>
                <br/> Կ.Տ.___________________________________        Կ.Տ ----------------------------------------------------
                <br/> (կամ պետական գրանցման վկայականի համարը)  (կամ պետական գրանցման վկայականի համարը)
                <br/>state registration number 			      state registration number
                <br/>
                <br/>____________________                       ____________________
                <br/>         			 (uտորագրությունը)                                  (uտորագրությունը)
                <br/> 				signature 	                                          signature
                 </div></font></body></html>",
                                    order,              // 0
                                    place,              // 1
                                    dateFrom,           // 2
                                    dateTo,             // 3
                                    days,               // 4
                                    table.Rows[0]["Purpose"].ToString(),    // 5
                                    pay_terms,          // 6
                                    apprej,             // 7
                                    style,              // 8
                                    DateTime.Parse(table.Rows[0]["Filling_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),// 9
                                    User.FullName,      // 10
                                    User.Department,     // 11            
                                    url1,                    //12
                                    DateTime.Now.Year   //13
                                );
            }
            catch (Exception ex)
            {
                htmlText = "PRINT ERROR : " + ex.Message;
            }
            return htmlText;
        }
        static string DrawPageLRT_PDF(string Request_ID)
        {
            string htmlText = string.Empty;

            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            VacationRequestViewUserControl obj = new VacationRequestViewUserControl();

            string order = obj.GetLTRNumber(Request_ID);
            Autor_id = obj.GetAutor_id(Request_ID);
            DataTable table = LTR.Get(Request_ID);

            string Rout = table.Rows[0]["City"].ToString();
            string place = string.Empty;
            foreach (string r in Rout.Split(new string[] { "|$$|" }, StringSplitOptions.None))
            {
                if (r.Length < 1) continue;
                place += string.Format("{0}, ", r);
            }
            place.Substring(0, place.Length - 1);
            string dateFrom = " " + DateTime.Parse(table.Rows[0]["Start_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);// ToShortDateString();
            string dateTo = " " + DateTime.Parse(table.Rows[0]["End_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<EDF_User> muser = EDF.AssociationUsersLTR(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);
            apprej += "</ul>";

            string days = string.Empty;
            string pay_terms = string.Empty;
            try
            {
                User = AD.GetUserByLogin(Autor_id);
                string style = string.Empty;

                htmlText = string.Format(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                <head><style>{8}</style><title>PRINT</title></head><body>
                <div style=""width: 750px; margin: 0 auto; border: 1px solid #ebebeb; padding: 10px; "">
                <div style""color: #7EBC0A; font-size: 16.0pt; font-weight: 700; font-family: ""Orange Armenian 35"", sans-serif; mso-font-charset: 0; text-align: center; vertical-align: 121; background: white; mso-pattern: black none; white-space: normal;"" class=""header"">
                <p>Գործուղման վկայական <br/> Travel certificate</p></div>
                <span style=""width: 120px;float: right;height: 60px;margin-top: -66px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span>
                <br/> {4}{6}
                <br/><span Style='text-decoration: underline;'>{10} , {11}</span>
                <br/>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                <br/>Name of employee and title
                <br/>                              
                <br/>Գործուղվում է <span Style='text-decoration: underline;'>{1}</span>
                <br/>(գործուղման վայրը, կազմակերպությունը)
                <br/>Place and name of company
                <br/>
                <br/>Հրաման N <span Style='text-decoration: underline;'>Գ{0}</span>
                <br/>Travel order No.
                <br/>
                <br/>Ամսաթիվ <span Style='text-decoration: underline;'>{9}</span>  թ.
                <br/>Date
                <br/>
                <br/>Գործուղման նպատակը <span Style='text-decoration: underline;'>{5}</span>
                <br/>Purpose of business trip
                <br/>
                <br/>Գործուղման ժամկետը 		____ oր 	մինչեւ <span Style='text-decoration: underline;'>{3}</span>  թ.
                <br/>Duration of business trip		     days	up to
                <br/>
                <br/>
                <br/>մեկնել է ______________________ ժամանել է _______________________
                <br/>Departure				    Arrival
                <br/>____  ______________ {13}  թ.             ____  ______________ {13}  թ.
                <br/>
                <br/>                       Կ.Տ.                                	              Կ.Տ.
                <br/>         stamp					stamp
                <br/>        ____________________                       ____________________
                <br/>          (uտորագրությունը)                          (uտորագրությունը)
                <br/>                   signature 	                                           signature
                <br/>
                <br/>մեկնել է ______________________ ժամանել է ________________________
                <br/>Departure 				Arrival
                <br/>     ____  ______________ {13} թ.             ____  ______________ {13} թ.
                <br/>
                <br/> Կ.Տ.___________________________________        Կ.Տ ----------------------------------------------------
                <br/> (կամ պետական գրանցման վկայականի համարը)  (կամ պետական գրանցման վկայականի համարը)
                <br/>state registration number 			      state registration number
                <br/>
                <br/>____________________                       ____________________
                <br/>         			 (uտորագրությունը)                                  (uտորագրությունը)
                <br/> 				signature 	                                          signature                       
                </div></body></html>",
                                    order,              // 0
                                    place,              // 1
                                    dateFrom,           // 2
                                    dateTo,             // 3
                                    days,               // 4
                                    table.Rows[0]["Purpose"].ToString(),    // 5
                                    pay_terms,          // 6
                                    apprej,             // 7
                                    style,              // 8
                                    DateTime.Parse(table.Rows[0]["Filling_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),// 9
                                    User.FullName,      // 10
                                    User.Department,     // 11            
                                    url1,                    //12
                                    DateTime.Now.Year   //13
                                );
            }
            catch (Exception ex)
            {
                htmlText = "PRINT ERROR : " + ex.Message;
            }
            return htmlText;
        }
        static string DrawPageITO(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;

            string order = ITO.GetNumber(Request_ID);

            Autor_id = request.GetAutor_id(Request_ID);

            DataTable table = EDF.GetITO(Request_ID);

            string FlightDetails = "";

            for (int f = 0; f < table.Rows[0]["Fly_Date"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.RemoveEmptyEntries).Length; f++)
            {

                FlightDetails += string.Format(@" 
            
</br>&nbsp;&nbsp;&nbsp;&nbsp;Date / Ամսաթիվ: <span Style='text-decoration: underline;'>{0}</span>
</br>&nbsp;&nbsp;&nbsp;&nbsp;Airline / Ավիաընկերություն: <span Style='text-decoration: underline;'>{1}</span>
</br>&nbsp;&nbsp;&nbsp;&nbsp;Flight number / Թռիչքի համար: <span Style='text-decoration: underline;'>{2}</span>
</br>&nbsp;&nbsp;&nbsp;&nbsp;Departure city / Մեկնման քաղաք: <span Style='text-decoration: underline;'>{3}</span>
</br>&nbsp;&nbsp;&nbsp;&nbsp;Destination city / Ժամանման քաղաք: <span Style='text-decoration: underline;'>{4}</span>
<br/>
            ",
        table.Rows[0]["Fly_Date"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
        table.Rows[0]["Fly_Airline"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
        table.Rows[0]["Fly_Number"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
        table.Rows[0]["Fly_Departure_City"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
        table.Rows[0]["Fly_Destination_City"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f]
    );
            }

            bool daily = (bool)table.Rows[0]["Daily"];

            bool hotel = (bool)table.Rows[0]["Hotel"];

            string hotelDetails = string.Empty;
            if (hotel)
            {
                hotelDetails = string.Format(@"
</br>&nbsp;Hotel details / Հյուրանոցի տվյալներ:
</br>&nbsp;&nbsp;&nbsp;&nbsp; Name / Անուն :  <span Style='text-decoration: underline;'>{0}</span>
</br>&nbsp;&nbsp;&nbsp;&nbsp; Dates of stay / Մնալու ժամանակահատված :  <span Style='text-decoration: underline;'>{1}</span>
</br>&nbsp;&nbsp;&nbsp;&nbsp; Location / Վայր :  <span Style='text-decoration: underline;'>{2}</span>
    {3}
    {4}
",
                table.Rows[0]["Hotel_Name"].ToString(),
                table.Rows[0]["Hotel_Dates"].ToString(),
                table.Rows[0]["Hotel_Location"].ToString(),
                string.IsNullOrEmpty(table.Rows[0]["Hotel_Phone"].ToString()) ? "" : "</br>&nbsp;&nbsp;&nbsp;&nbsp; Phone number / Հեռախոս :  <span Style='text-decoration: underline;'>" + table.Rows[0]["Hotel_Phone"].ToString() + "</span>",
                string.IsNullOrEmpty(table.Rows[0]["Hotel_Payment"].ToString()) ? "" : "</br>&nbsp;&nbsp;&nbsp;&nbsp; Payment method / Վճարման ձև : <span Style='text-decoration: underline;'>" + table.Rows[0]["Hotel_Payment"].ToString() + "</span>"
                    );
            }
            bool budgeted = (bool)table.Rows[0]["Budgeted"];

            string amount = table.Rows[0]["Amount"].ToString();
            string am = "";
            if (!string.IsNullOrEmpty(amount))
            {
                am = string.Format(@"
                                        </br>Ծախսերը, որոնք կատարվում են հրավիրող կողմի միջոցներով: <span Style='text-decoration: underline;'>{0}</span>
                                        </br>Costs covered by the inviter", amount);
            }
            List<EDF_SPUser> repUsers = new List<EDF_SPUser> { };

            repUsers = Approve_reject.getRepPeoples(Request_ID);

            string replacement = string.Empty;

            if (repUsers.Count > 0)
            {
                string rep = string.Empty;

                foreach (EDF_SPUser us in repUsers)
                {
                    rep += us.FullName + ", ";
                }

                replacement = string.Format(@"
                                        </br>Ո՞վ է փոխարինելու: <span Style='text-decoration: underline;'>{0}</span>
                                        </br>Who will replace", rep);
            }

            string Rout = table.Rows[0]["City"].ToString();
            string place = string.Empty;
            int i = 1;
            foreach (string r in Rout.Split(new string[] { "|$$|" }, StringSplitOptions.None))
            {
                if (r.Length < 1) continue;
                place += string.Format("{0}, ", r);
            }
            place = place.Substring(0, place.Length - 2);

            string dateFrom = " " + DateTime.Parse(table.Rows[0]["Start_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);// ToShortDateString();
            string dateTo = " " + DateTime.Parse(table.Rows[0]["End_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<EDF_User> muser = EDF.AssociationUsersITO(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);
            apprej += "</ul>";

            string Organization = table.Rows[0]["Organization"].ToString();
            string organization = string.Empty;

            i = 1;
            foreach (string r in Organization.Split(new string[] { "|$$|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (r.Length < 1) continue;
                organization += r + ", ";
                i = 2;
            }
            if (organization.Length > 2) { organization = organization.Substring(0, organization.Length - 2); }

            string Org = string.Format(@"
                </br>Հրավիրող կազմակերպություն 1: <span Style='text-decoration: underline;'>{0}</span>
                    </br>Inviting organization 1 (optional)
                </br>", organization);

            if (i == 1) { organization = Org = string.Empty; }


            string substitut = Request_Substitute.Get_User_Names(Request_ID, Autor_id);

            if (substitut.Length > 0)
                substitut = "<p><b>3. Ո՞վ է փոխարինելու / Who will replace</b>  </p>" + substitut;

            try
            {
                User = AD.GetUserByLogin(Autor_id);
                htmlText = string.Format(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                <head><style>{8}</style><title>PRINT</title></head><body><div class=""content""><div class=""header""><p>Միջազգային գործուղման հրաման / International travel order </p>
                <p>Order N / Հրաման Գ{0} / {20}</p> </div><span style=""width: 100px;float: right;height: 60px;margin-top: -75px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span>
                <br/>                       
                </br><span Style='text-decoration: underline;'>{10} , {11}</span>
                </br>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                </br>Name of employee and title
                </br>                              
                </br>Գործուղվում է`: <span Style='text-decoration: underline;'>{1}</span>
                </br>Dates of travel
                </br>{6}
                </br>Լրացնելու ամսաթիվ: <span Style='text-decoration: underline;'>{9}</span>
                </br>filling date  
                </br>
                </br>Գործուղման նպատակը: <span Style='text-decoration: underline;'>{5}</span>
                </br>Purpose of business trip
                </br>
                </br>Գործուղման ժամկետը:    Սկսած` <span Style='text-decoration: underline;'>{2} </span> &nbsp;  Մինչև` <span Style='text-decoration: underline;'>{3} </span>  (ներառյալ)
                </br>Duration of business trip      
                </br>
                </br>Բյուջետավորված: <span Style='text-decoration: underline;'>{13}</span>
                </br>Budgeted</br>{14}</br>{15}</br>
                </br>Օրապահիկ: <span Style='text-decoration: underline;'>{16}</span>
                </br>Daily allowance</br>
                </br>Հյուրանոց: <span Style='text-decoration: underline;'>{17}</span>
                </br>Hotel</br>{18}<br></br>Թռիչքի տվյալներ / Flight details :</br>{19}
                </br><p><b>Հաստատված է/approved by</b></p>{7}</div></body></html>",
                                    order,              // 0
                                    place,              // 1
                                    dateFrom,           // 2
                                    dateTo,             // 3
                                    organization,       // 4
                                    table.Rows[0]["Purpose"].ToString(),    // 5
                                    Org,          // 6
                                    apprej,             // 7
                                    style,              // 8
                                    DateTime.Parse(table.Rows[0]["Filling_Date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),// 9
                                    User.FullName,      // 10
                                    User.Department,     // 11            
                                    substitut,
                                    budgeted ? "Yes / Այո" : "No / ոչ",  //13
                                    am,                  //14
                                    replacement,         //15
                                    daily ? "Requested / Պահանջվում է" : "Not requested / Չի պահանջվում",    //16
                                    hotel ? "Required / Պահանջվում է" : "Not required / Չի պահանջվում",  //17
                                    hotelDetails,        //18
                                    FlightDetails, //19
                                    Approve_reject.getLastApproveDate(Request_ID).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) //20
                                );
            }
            catch (Exception ex)
            {
                htmlText = "PRINT ERROR : " + ex.Message;
            }
            return htmlText;
        }
        static string DrawPageRSR(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;

            string order = RSR.GetNumber(Request_ID);

            Autor_id = request.GetAutor_id(Request_ID);
            DataTable table = EDF.GetRSR(Request_ID);

            User = AD.GetUserByLogin(Autor_id);

            bool AP = string.IsNullOrEmpty(table.Rows[0]["access_pending"].ToString()) ? false : (bool)table.Rows[0]["access_pending"];

            string fatherName = table.Rows[0]["Father"].ToString();
            string cboss = table.Rows[0]["Cboss"].ToString();
            string Mobile = (string.IsNullOrEmpty(User.Mobile) ? table.Rows[0]["Phone"].ToString() : User.Mobile);
            string Private_Phone = table.Rows[0]["Private_Phone"].ToString();
            string dateEnd = " " + ((DateTime)table.Rows[0]["Last_Work_Day"]).ToString("dd/MM/yyyy");


            List<EDF_User> muser = EDF.AssociationUsersRSR(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}</br></br></li>", us.FullName, us.Department);

            apprej += "<li>HR Administration department Director Marine Aznauryan ____________<br/></li>";
            apprej += "</ul>";

            string days = string.Empty;
            string pay_terms = string.Empty;

            try
            {
                htmlText = string.Format(
                   @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""  ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                <head><style>{8}</style><title>PRINT</title></head><body><div class=""content""><div class=""header"">
                <p>Շրջիկ թերթիկ <br/> Round sheet <br/> {14} </p>  </div><span style=""width: 120px;float: right;height: 60px;margin-top: -90px;""><img width=""100"" height=""50"" src=""/_layouts/images/edf/docLogo.png""></span>
                <br/></br><span Style='text-decoration: underline;'>{10} , {1} , {11}</span>
                </br>(ազգանունը, անունը, հայրանունը, պաշտոնը)
                </br>Name of employee and title                </br>                              
                </br>Windows օգտագործողի անուն: <span Style='text-decoration: underline;'>{2}</span>
                </br>Windows account user name</br>
                </br>Cboss օգտատերի անուն (եթե առկա է): <span Style='text-decoration: underline;'>{3}</span>
                </br>Cboss user name (if any) </br>
                </br>Բջջային: <span Style='text-decoration: underline;'>{5}</span>
                </br>Mobile number</br>
                </br>Անձնական բջջային: <span Style='text-decoration: underline;'>{9}</span>
                </br>Private mobile number</br>
                </br>Վերջին աշխատանքային օրը: <span Style='text-decoration: underline;'>{12}</span>
                </br>Last working day</br>
                </br>Access disable Pending : {13}</br>{4} {6}<p><b>Հաստատված է/approved by</b></p>{7}</div>
                </body></html>",
                                    order,              // 0
                                    fatherName,         // 1
                                    User.Login,         // 2
                                    string.IsNullOrEmpty(cboss) ? "առկա չէ" : cboss,             // 3
                                    days,               // 4
                                    Mobile,             // 5
                                    pay_terms,          // 6
                                    apprej,             // 7
                                    style,              // 8
                                    Private_Phone,      // 9
                                    User.FullName,      // 10
                                    User.Department,    // 11            
                                    dateEnd,             // 12
                                    AP ? "<input checked='checked' type='checkbox' disabled='disabled' />" : "<input type='checkbox' disabled='disabled' />", //13
                                    Approve_reject.getLastApproveDate(Request_ID).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) //14
                                );
            }
            catch (Exception ex)
            {
                htmlText = "PRINT ERROR : " + ex.Message;
            }
            return htmlText;
        }
    }
}
