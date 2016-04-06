using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using EDF_WEB_Parts_1.VacationRequestView;
using iTextSharp.text;
using Microsoft.SharePoint;


namespace EDF_WEB_Parts_1.ViewLTR
{
    public partial class ViewLTRUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        EDF_SPUser Cur = ADSP.CurrentUser;
        string curLog;
        EDF_SPUser autor;
        string Autor_id;
        string Request_ID;
       
        bool isManager1, isManager2, isManager3;

        protected void Page_Load(object sender, EventArgs e)
        {
            UC.SearchBox(ComText, "Write your comment:");
            if (Request.QueryString["rid"] == null)
                ER.GoToErrorPage("Request NOT FOUND");

            AppRejDiv.Visible = false;

            Request_ID = Request.QueryString["rid"];

            if (Request.QueryString["nid"] != null)
            {
                string NId = Request.QueryString["nid"];
                Notificaion.Update(NId, true);
            }

            string orderN = LTR.GetNumber(Request_ID);

            curLog = Cur.Login;

            if (!Approve_reject.canView(Cur, Request_ID))
            {
                Response.Redirect(SPContext.Current.Site.Url);
            }

            if (!PDF.hasSertificatedFile(Request_ID))
            {
                cert_icon.Visible = false;
                LBDSPDFDIV.Visible = false;
            }


            if (orderN == "0")
                OrderSpan.Visible = PrintDiv.Visible = DivPDF.Visible = false;
            else
            {
                OrderSpan.Visible = true;
                OrderLabel.Text = orderN;

                if (Cur.IsMemberOf("EDF_LTR_admin") || Cur.IsMemberOf("EDF_ITR-LTR_accountant"))
                    PrintDiv.Visible = true;
                else PrintDiv.Visible = false;

                if (Cur.GetDocAccess(2))
                    DivPDF.Visible = true;
                else DivPDF.Visible = false;
            }

            Autor_id = request.GetAutor_id(Request_ID);

            autor = AD.GetUserBySPLogin(Autor_id);

            autorImg.Src = autor.PictureUrl;

            DataTable table = LTR.Get(Request_ID);

            drow(table);

            DataTable tableCom = Comments.Get(Request_ID);
            DrawComents(tableCom);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UserProf.HRef = @"http://intranet/SitePages/Person.aspx?accountname=FTA\" + autor.Login;

            bool t = false;
           
            switch (Approve_reject.App_rej(Request_ID, curLog))
            {
                case -1:                 
                    break;
                case 0:
                    Label2.Visible = Label1.Visible = false;
                    t = true;
                    AppRejDiv.Visible = ButtonCancel.Visible = ButtonSubmit.Visible = true;
                    break;
                case 1:                  
                    Label1.Text = "<b>You already approved</b>";
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                    break;
                case 2:                  
                    Label1.Text = "<b>You already rejected</b>";
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                    break;
            }
            ////rep                                                                                                         ??????????
            if (Cur.ParentReplacement.Count > 0 && !t)
            {
                foreach (EDF_SPUser us in Cur.ParentReplacement)
                {
                    switch (Approve_reject.App_rej(Request_ID, us.Login))
                    {
                        case -1:
                            break;
                        case 0:
                            Label2.Visible = Label1.Visible = false;
                            t = true;
                            AppRejDiv.Visible = ButtonCancel.Visible = ButtonSubmit.Visible = true;
                            break;
                        case 1:
                            Label1.Text = "<b>You already approved</b>";
                            ButtonCancel.Visible = ButtonSubmit.Visible = false;
                            break;
                        case 2:
                            Label1.Text = "<b>You already rejected</b>";
                            ButtonCancel.Visible = ButtonSubmit.Visible = false;
                            break;
                    }
                    if (t) break;
                }
            }
            ////rep

            bool rp = false;
            foreach (EDF_SPUser us in Cur.ParentReplacement)
            {
                if (Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login))
                    rp = true;
            }

            if (request.IsYour(Request_ID, curLog) && Cur.Login != AD.HR.Login)
            {
                if (!(Approve_reject.CanAppRejLTR_Null(Request_ID, curLog) || rp))
                {
                    Label1.Text = "<b>It's your request</b>";
                    Label1.Visible = true;

                    AppRejDiv.Visible = false;
                }
            }

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                if (autor.HasManager)
                {
                    isManager1 = (curLog == autor.Manager.Login);                        //   isManager1
                }

                EDF_SPUser dep = null;
                try
                {
                    dep = AD.GetUserDirector(autor.Login);
                }
                catch { }
                if (dep != null)
                    isManager2 = (curLog == dep.Login);                                  //   isManager2
                else
                    isManager2 = false;

                if (isManager2) isManager1 = false;

                isManager3 = (curLog == AD.CEO.Login);                                   //   isManager3

                if (isManager3) isManager1 = isManager2 = false;
            });


            if (!Approve_reject.CanAppRejLTR_Null(Request_ID, curLog))
            {
                foreach (EDF_SPUser us in Cur.ParentReplacement)
                {
                    if (Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login))
                    {

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            if (autor.HasManager)
                            {

                                isManager1 = (us.Login == autor.Manager.Login);
                            };

                            EDF_SPUser dep = null;
                            try
                            {
                                dep = AD.GetUserDirector(autor.Login);
                            }
                            catch { }
                            if (dep != null)
                                isManager2 = (us.Login == dep.Login);
                            else
                                isManager2 = false;
                            if (isManager2) isManager1 = false;
                        });

                        break;
                    }
                }
            }

            //          SAQO        //////
            string ss = "0";
            try
            {
                ss = "1";
                ul_history.InnerHtml = string.Empty;
                string rid = Request.QueryString["rid"].ToString();
                ss = "2";
                bool IsPendTitle = true;
                bool IsPastTitle = true;
                foreach (EDF_User u in EDF.AssociationUsersLTR(rid))
                {
                    if (!u.Icon.Contains("timer.png") && IsPastTitle)
                    {
                        ul_history.InnerHtml += "<li class='right_top'><p style='font-size: 20px;'>Approved/Rejected by</p></li>";
                        IsPastTitle = false;
                    }

                    if (u.Icon.Contains("timer.png") && IsPendTitle)
                    {
                        ul_history.InnerHtml += "<li class='right_top'><p style='font-size: 20px;'>Pending for approval</p></li>";
                        IsPendTitle = false;
                    }
                    ss = "3";
                    ul_history.InnerHtml += string.Format(
"<li style='cursor:default'><div class='fleft'><img class='avatar' src='{0}' /></div><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}<span>{4}</span></h9></p><img class='history_icon' src='{5}'/></li>",
                        u.PictureUrl,
                        u.R + u.FullName,
                        u.Department,
                        u.Date,
                        u.Time,
                        u.Icon);
                    ss = "4";

                    if (u.Icon.Contains("x.png") && !u.IsSubtitute)
                        break;
                }
            }
            catch
            {
                //ER.GoToErrorPage(string.Format("History error | {0} | EX:{1} ", ss, ex.Message));
                Response.Write(string.Format("ERROR - History {0} EX:{1} - ERROR", ss, ""));
            }

        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            bool hasParent = false;
            if (Cur.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in Cur.ParentReplacement)
                    if (LTR.CanAppRejLRT(Request_ID, p.Login))
                        hasParent = true;
            }

            if (LTR.CanAppRejLRT(Request_ID, Cur.Login) || hasParent)
            { }
            else
                return;


            //if (!Approve_reject.CanAppRejLTR(Request_ID, Cur.Login))
            //    return;

            Button bttn = sender as Button;
            bool b = bttn.Text == "Approve Request";

            string Message = string.Empty;

            string typeName = Request_type.GetTypeName(Request_ID);
            if (b)
                Message = string.Format("<b>{0}</b> {6}<b>{1}</b> approved <b>{2}’s</b> {3} (ID: {4} {5} )", Cur.FullName, Cur.Department, autor.FullName, typeName, Request_type.GetUppers(typeName), Request_ID, Cur.Department == null ? "" : "from ");
            else
                Message = string.Format("<b>{0}</b> {6}<b>{1}</b> has rejected <b>{2}’s</b> {3} (ID: {4} {5} )", Cur.FullName, Cur.Department, autor.FullName, typeName, Request_type.GetUppers(typeName), Request_ID, Cur.Department == null ? "" : "from ");


            Int16 end = 0;

            if (isManager1)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    Approve_reject.Add(autor.Director.Login, Request_ID, "ViewLTR.aspx");

                    if (autor.Manager.Login == curLog)
                    {
                        if (LTR.CarIsChecked(Request_ID))
                        {
                            string TypeName = Request_type.GetTypeName(Request_ID);

                            string carpoolemail = Constants.EmailCarpool;

                            string s = autor.FullName;
                            DataTable dt = LTR.Get(Request_ID);

                            string msg = string.Format("<b>ID: {1} {2}</b> requested car for transport at {5} (<b>{3}-{4}</b> {0})", s, Request_type.GetUppers(TypeName), Request_ID, ((DateTime)dt.Rows[0]["Start_Date"]).ToString("dd/mm/yyyy"), ((DateTime)dt.Rows[0]["End_Date"]).ToString("dd/mm/yyyy"), dt.Rows[0]["Car_Time"].ToString());

                            //string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);

                            string Visit_url = SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID;

                            if (!AD.Domain.Contains("pele"))
                                Notificaion.sendMail("EDF - Notification", carpoolemail, msg + "<br/>" + Visit_url);
                        }
                    }

                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager2 || isManager3)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                request.Update(Request_ID, b);

                if (autor.Manager.Login == curLog)
                {
                    if (LTR.CarIsChecked(Request_ID))
                    {
                        string TypeName = Request_type.GetTypeName(Request_ID);

                        string carpoolemail = Constants.EmailCarpool;

                        string s = autor.FullName;
                        DataTable dt = LTR.Get(Request_ID);

                        string msg = string.Format("<b>ID: {1} {2}</b> requested car for transport at {5} (<b>{3}-{4}</b> {0})", s, Request_type.GetUppers(TypeName), Request_ID, ((DateTime)dt.Rows[0]["Start_Date"]).ToString("dd/mm/yyyy"), ((DateTime)dt.Rows[0]["End_Date"]).ToString("dd/mm/yyyy"), dt.Rows[0]["Car_Time"].ToString());

                        //string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);

                        string Visit_url = SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID;

                        if (!AD.Domain.Contains("pele"))
                            Notificaion.sendMail("EDF - Notification", carpoolemail, msg + "<br/>" + Visit_url);
                    }
                }

                if (b) end = 1;
                else end = -1;
            }
          
            else
           
            if (end == 1 || end == -1)
            {
                string TypeName = Request_type.GetTypeName(Request_ID);
                if (end == 1)
                {
                    string s = "Your";

                    string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                    //string mailMsg = string.Format("Please be informed that <b>{0}’s</b> {1} (ID: {2} {3}) <b>approved by all responsible persons</b>", autor.FullName, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                    Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));

                }
                else if (end == -1)
                {
                    string s = "Your";

                    string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>rejected</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                    Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                }


            }
            if (end == 1)
            {
                LTR.Update(Request_ID);

                string TName = Request_type.GetTypeName(Request_ID);

                string ms = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", autor.FullName + "’s", TName, Request_type.GetUppers(TName), Request_ID);

                foreach (EDF_SPUser us in AD.GetGroupUsers(AD.GetGroupNameByKey("EDF_LTR_admin")))
                {
                    Notificaion.Add(us.Login, SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                    if (us.HasReplacement)
                        Notificaion.Add(us.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
                foreach (EDF_SPUser us in AD.GetGroupUsers(AD.GetGroupNameByKey("EDF_ITR-LTR_accountant")))
                {
                    Notificaion.Add(us.Login, SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                    if (us.HasReplacement)
                        Notificaion.Add(us.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
            }
            if (b)
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            else
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }

        }

        private void drow(DataTable table)
        {
            LblType.Text = Request_type.GetTypeName(Request_ID);
            LabelName.Text = autor.FullName;
            LabelDep.Text = autor.Department;

            string Rout = table.Rows[0]["City"].ToString();

            int i = 1;
            RoutUL.InnerHtml = string.Empty;
            foreach (string r in Rout.Split(new string[] { "|$$|" }, StringSplitOptions.None))
            {
                if (r.Length < 1) continue;
                RoutUL.InnerHtml += string.Format("<li><p>{0}. {1}</p></li>", i++, r);
            }

            dateFrom.Text = ((DateTime)table.Rows[0]["Start_Date"]).ToString("dd/MM/yyyy");
            dateTo.Text = ((DateTime)table.Rows[0]["End_Date"]).ToString("dd/MM/yyyy");
            LablePurpose.InnerText = table.Rows[0]["Purpose"].ToString();
            LableDaily.InnerText = (table.Rows[0]["Daily"].ToString().ToLower() == "true") ? "Requested / Պահանջվում է" : "Not requested / Չի պահանջվում";
            LabelCar.InnerText = table.Rows[0]["Car_Time"].ToString().Length > 0 ? "  Yes / Այո Time / Ժամ   " + table.Rows[0]["Car_Time"].ToString() : "No / Ոչ";
            LabelHotel.InnerText = (table.Rows[0]["Hotel"].ToString().ToLower() == "true") ? "Required / Պահանջվում է" : "Not required / Չի պահանջվում";
        }

        protected void ComSend_Click(object sender, EventArgs e)
        {
            if (ComText.Value != "" && ComText.Value != "Write your comment:")
                Comments.Add(Request_ID, ComText.Value, curLog, false);


            ComText.Value = "Write your comment:";
            DataTable tableCom = Comments.Get(Request_ID);

            DrawComents(tableCom);

            string TypeName = Request_type.GetTypeName(Request_ID);

            DataTable users1 = request.GetAllUsers_ids(Request_ID);

            for (int i = 0; i < users1.Rows.Count; i++)
            {
                if (Cur.Login == users1.Rows[i]["Autor_id"].ToString()) continue;

                string s = string.Empty;

                if (autor.Login == users1.Rows[i]["Autor_id"].ToString())
                { s = "your"; }
                else if (autor.Login == Cur.Login)
                { s = "his/her"; }
                else { s = autor.FullName + "’s"; }

                string msg = string.Format("<b>{0}</b> {4}<b>{1}</b> commented on <b>{6}</b> {2} (ID: {5} {3})", Cur.FullName, Cur.Department, TypeName, Request_ID, (Cur.Department == null ? "" : "from "), Request_type.GetUppers(TypeName), s);

                Notificaion.Add(users1.Rows[i]["Autor_id"].ToString(), SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 1);
            }
        }

        public void DrawComents(DataTable table)
        {
            divCom.InnerHtml = string.Empty;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string t1 = table.Rows[i]["MSG"].ToString();
                string t2 = DateTime.Parse(table.Rows[i]["Add_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).ToString().ToString();//((DateTime)table.Rows[i]["Add_date"]).ToShortDateString();
                string t8 = ((DateTime)table.Rows[i]["Add_date"]).ToShortTimeString();
                string t3 = table.Rows[i]["User_ID"].ToString();
                string t4 = table.Rows[i]["see"].ToString();
                EDF_SPUser ComAutor = AD.GetUserBySPLogin(t3);
                string t5 = ComAutor.FullName;
                string t6 = ComAutor.PictureUrl;
                string t7 = ComAutor.Department;

                divCom.InnerHtml += string.Format(
                "<ul class='margin'>" +
                    "<li class='cont cont1 clearfix' style='cursor: default;'><div class='fleft'><img class='avatar' src='{1}' /></div><p class='text4 width fleft'>{2}<span> from </span>{3} <span>wrote:</span><br /><h9>{4} | <span>{5}</span></h9></p></li>" +
                    "<li class='cont cont1 clearfix' style='cursor: default;'><p class='text4 fleft'><span>{7}</span></p></li>" +
                "</ul>",
                "#", t6, t5, t7, t2, t8, "#", t1
                );
            }
            comImg.Src = Cur.PictureUrl;
        }

        protected void PrintLink_Click(object sender, EventArgs e)
        {
            string url = string.Format("{0}/SitePages/Print.aspx?rid={1}", SPContext.Current.Web.Url, Request_ID);

            Response.Redirect(url);
        }

        protected void LBPDF_Click(object sender, EventArgs e)
        {

            String htmlText = DrawPageLRT(Request_ID);
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Response.Clear();

                Response.ContentType = "application/pdf";

                Response.WriteFile(PDF_DOC.Create(htmlText, "LTR", Request_ID));

            });
        }

        static string DrawPageLRT(string Request_ID)
        {
            string htmlTextPDF = string.Empty;

            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            VacationRequestViewUserControl obj = new VacationRequestViewUserControl();

            string order = obj.GetLTRNumber(Request_ID);
            Autor_id = obj.GetAutor_id(Request_ID);
            DataTable table = LTR.Get(Request_ID);

            string Rout = table.Rows[0]["City"].ToString();
            string place = string.Empty;
        //    int i = 1;
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


                htmlTextPDF = string.Format(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
            <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">

                <head>
                   
                    <title> </title>

                </head>

                <body>
<font face = ""arial unicode ms"">
                    <div>

                        <div>
<span Style=""color:#F60; font-size: 16pt; font-weight:700; text-align:center; white-space:normal;""><p>Գործուղման վկայական <br/> Travel certificate</p></span>                            
                                                                      
                            
                        </div>
                                                
                      
<br/>     
                        {4}
                                  
                        {6}

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
<br/>____  ______________ {12}  թ.             ____  ______________ {12}  թ.
<br/>
<br/>                       Կ.Տ.                                	              Կ.Տ.
<br/>         stamp					stamp
<br/>        ____________________                       ____________________
<br/>          (uտորագրությունը)                          (uտորագրությունը)
<br/>                   signature 	                                           signature
<br/>
<br/>մեկնել է ______________________ ժամանել է ________________________
<br/>Departure 				Arrival
<br/>     ____  ______________ {12}թ.             ____  ______________ {12} թ.
<br/>
<br/> Կ.Տ.__________________________________        Կ.Տ ---------------------------------------------------
<br/> (կամ պետական գրանցման վկայականի համարը)  (կամ պետական գրանցման վկայականի համարը)
<br/>state registration number 			      state registration number
<br/>
<br/>____________________                       ____________________
<br/>         			 (uտորագրությունը)                                  (uտորագրությունը)
<br/> 				signature 	                                          signature
                       

                    </div>
</font>
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
                                    DateTime.Now.Year   //12

                                );

            }
            catch (Exception ex)
            {
                htmlTextPDF = "PDF ERROR : " + ex.Message;
            }
            return htmlTextPDF;
        }

        protected void LBDSPDF_Click(object sender, EventArgs e)
        {
            if (PDF.hasSertificatedFile(Request_ID))
            {
                Response.Redirect(PDF.GetFromSPFolder(Request_ID));
            }
        }

        protected void UploadPDF_Click(object sender, EventArgs e)
        {
            if (!filePDF.HasFile) return;
            bool b = false;
            string url = string.Empty;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                string path = HttpContext.Current.Request.PhysicalApplicationPath + "\\All_Requests\\TEMP\\" + filePDF.FileName;


                while (File.Exists(path))
                    try { File.Delete(path); }
                    catch { }

                filePDF.SaveAs(path);


                PdfDoc doc = new PdfDoc(path);

                if (doc.HasCertificate)
                {
                    doc.close();
                    url = PDF.SaveInSP(path, Autor_id, Request_ID);

                    while (File.Exists(path))
                        try { File.Delete(path); }
                        catch { }
                }
                else
                {
                    doc.close();
                    while (File.Exists(path))
                        try { File.Delete(path); }
                        catch { }

                    b = true;
                }
            });

            if (b) ER.GoToErrorPage("Your PDF file has not certificate");

            string script = "<script language='javascript'>alert('The document was loaded successfully.');location.reload();</script>";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "Register", script);


        }
    }
}
