using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.ViewITO
{
    public partial class ViewITOUserControl : UserControl
    {
        DataTable table;

        string Autor_id;
        string Request_ID;
        bool substitute, isManager1, isManager2, isManager3, isManager4;
        string User_id;
        EDF_SPUser Cur = ADSP.CurrentUser;
        EDF_SPUser autor;
        DateTime Start_date;
        DateTime End_date;

        protected void Page_Load(object sender, EventArgs e)
        {
            Request_ID = Request.QueryString["rid"];
            if (Request_ID == null)
                ER.GoToErrorPage("Request Id not found");

            AppRejDiv.Visible = false;

            User_id = Cur.Login;

            Autor_id = request.GetAutor_id(Request_ID);

            autor = AD.GetUserBySPLogin(Autor_id);
            //securoty
            if (!Approve_reject.canView(Cur, Request_ID))
            {
                Response.Redirect(SPContext.Current.Site.Url);
            }

            if (!PDF.hasSertificatedFile(Request_ID))
            {
                cert_icon.Visible = false;
                LBDSPDFDIV.Visible = false;
            }

            Draw();

            DataTable tableCom = Comments.Get(Request_ID);
            DrawComents(tableCom);


            if (Request.QueryString["nid"] != null)
            {
                string NId = Request.QueryString["nid"];
                Notificaion.Update(NId, true);
            }
            UC.SearchBox(ComText, "Write your comment:");

            HiddenFieldspPeoplePicker.Value = spPeoplePicker.CommaSeparatedAccounts;
            string str = string.Empty;
            try
            {
                string orderN = ITO.GetNumber(Request_ID);

                if (orderN == "0")

                    OrderSpan.Visible = PrintDiv.Visible = DivPDF.Visible = false;
                else
                {
                    OrderSpan.Visible = true;
                    OrderLabel.Text = orderN;
                    if (Cur.IsMemberOf("EDF_LTR_admin") || Cur.IsMemberOf("EDF_ITR-LTR_accountant"))
                        PrintDiv.Visible = true;
                    else
                        PrintDiv.Visible = false;


                    if (Cur.GetDocAccess(3))
                        DivPDF.Visible = true;
                    else DivPDF.Visible = false;
                }

                AppRejDiv.Visible = ButtonCancel.Visible = ButtonSubmit.Visible = ITO.CanAppRejITO(Request_ID, User_id);

                UserProf.HRef = @"http://intranet/SitePages/Person.aspx?accountname=FTA\" + Autor_id;

                bool t = false;

                switch (Request_Substitute.Is_ok(Request_ID, User_id))
                {
                    case -1:
                        break;
                    case 0:
                        Label2.Text = "for replacement";
                        t = true;
                        AppRejDiv.Visible = ButtonCancel.Visible = ButtonSubmit.Visible = true;
                        break;
                    case 1:
                        Label2.Text = "for replacement";
                        Label1.Text = "<b>You already approved</b>";
                        ButtonCancel.Visible = ButtonSubmit.Visible = false;
                        break;
                    case 2:
                        Label2.Text = "for replacement";
                        Label1.Text = "<b>You already rejected</b>";
                        ButtonCancel.Visible = ButtonSubmit.Visible = false;
                        break;
                }

                switch (Approve_reject.App_rej(Request_ID, User_id))
                {
                    case -1:
                        break;
                    case 0:
                        Label2.Visible = Label1.Visible =false;
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
                ////rep
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
                    }
                }
                ////rep
                bool rp = false;
                foreach (EDF_SPUser us in Cur.ParentReplacement)
                {

                    if (Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login))
                        rp = true;
                }

                string cfo = AD.CFO.Login;

                if (request.IsYour(Request_ID, User_id) && User_id != cfo)
                {
                    if (!(Approve_reject.CanAppRejLTR_Null(Request_ID, User_id) || rp))
                    {
                        Label1.Text = "<b>It's your request</b>";
                        Label1.Visible = true;

                        AppRejDiv.Visible = false;
                    }
                }
               
                str += ":1:";

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    if (Cur.IsCEO)
                    {
                        isManager3 = true;
                    }
                    else
                    {
                        if (autor.HasManager)
                        {
                            str += ":1:";
                            isManager1 = (User_id == autor.Manager.Login);
                            str += ":1:";
                        };
                        str += ":8:";

                        string sub = Request_Substitute.Get_User_id_NULL(Request_ID, Autor_id);

                        foreach (string s in sub.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (s == User_id)
                                substitute = true;
                        }
                        str += ":1:";

                        if (autor.HasDirector)
                        {
                            str += ":7:";
                            isManager2 = (User_id == autor.Director.Login);
                        }
                        else
                            isManager2 = false;
                        if (isManager2) isManager1 = false;
                        str += ":1:";
                        isManager3 = (User_id == AD.CEO.Login);
                        str += ":1:";
                        if (isManager3) isManager1 = isManager2 = false;
                        str += ":1:";

                        if ((User_id == AD.CFO.Login) && EDF.GetAproveRejectStateCFOITO(Request_ID, User_id) == 1)
                            isManager4 = true;
                        else
                            isManager4 = false;
                        str += ":1:";
                        if (isManager4) isManager1 = isManager2 = isManager3 = false;
                        str += ":1:";
                    }
                });

                if (!ITO.CanAppRejIsITO_Null(Request_ID, User_id))
                {
                    foreach (EDF_SPUser us in Cur.ParentReplacement)
                    {
                        if (ITO.CanAppRejIsITO_Null(Request_ID, us.Login))
                        {

                            SPSecurity.RunWithElevatedPrivileges(delegate()
                            {
                                if (Cur.IsCEO)
                                {
                                    isManager3 = true;
                                }
                                else
                                {
                                    if (autor.HasManager)
                                    {

                                        isManager1 = (us.Login == autor.Manager.Login);

                                    };
                                    string sub = Request_Substitute.Get_User_id_NULL(Request_ID, Autor_id);
                                    foreach (string s in sub.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (s == us.Login)
                                            substitute = true;
                                    }

                                    EDF_SPUser dep = null;
                                    if (autor.HasDirector)
                                        dep = autor.Director;

                                    if (dep != null)
                                        isManager2 = (us.Login == dep.Login);
                                    else
                                        isManager2 = false;
                                    if (isManager2) isManager1 = false;

                                    isManager3 = (us.Login == AD.CEO.Login);

                                    if (isManager3) isManager1 = isManager2 = false;


                                    if ((us.Login == AD.CFO.Login) && Autor_id != AD.CFO.Login)
                                        isManager4 = true;
                                    else if (us.Login == AD.CFO.Login && Autor_id == AD.CFO.Login && EDF.GetAproveRejectState(Request_ID, us.Login) == -1)
                                        isManager4 = true;
                                    else
                                        isManager4 = false;

                                    if (isManager4) isManager1 = isManager2 = isManager3 = false;
                                }
                            });

                            break;
                        }
                    }
                }
                if (!IsPostBack)
                {
                    spPeoplePicker.CommaSeparatedAccounts = Request_Substitute.Get_User_ids(Request_ID, Autor_id);
                    spPeoplePicker.Validate();
                }
                str += ":1:";
                if (isManager1 && t && Request_Substitute.AppRejFromAllSub(Request_ID))
                    spPeoplePicker.Enabled = spPeoplePicker.IsChanged = true;
                else
                    spPeoplePicker.Enabled = spPeoplePicker.IsChanged = false;
                str += ":1:";
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - ViewReplace pageload -  <br/> <b>" + str + "</b>" + ex.Message);
            }
            string ss = "0";
            try
            {
                ss = "1";
                ul_history.InnerHtml = string.Empty;
                string rid = Request.QueryString["rid"].ToString();
                ss = "2";
                bool IsPendTitle = true;
                bool IsPastTitle = true;
                foreach (EDF_User u in EDF.AssociationUsersITO(rid))
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
                    if (ITO.CanAppRejITO(Request_ID, p.Login))
                        hasParent = true;
            }

            if (ITO.CanAppRejITO(Request_ID, User_id) || hasParent)
            { }
            else
                return;

            string Message = string.Empty;

            Button bttn = sender as Button;

            bool b = bttn.Text == "Approve Request";

            string Autor_id = request.GetAutor_id(Request_ID);

            Int16 end = 0;

            string typeName = Request_type.GetTypeName(Request_ID);
            if (b)
                Message = string.Format("<b>{0}</b> {6}<b>{1}</b> approved <b>{2}’s</b> {3} (ID: {4} {5} )", Cur.FullName, Cur.Department, autor.FullName, typeName, Request_type.GetUppers(typeName), Request_ID, Cur.Department == null ? "" : "from ");
            else
                Message = string.Format("<b>{0}</b> {6}<b>{1}</b> has rejected <b>{2}’s</b> {3} (ID: {4} {5} )", Cur.FullName, Cur.Department, autor.FullName, typeName, Request_type.GetUppers(typeName), Request_ID, Cur.Department == null ? "" : "from ");


            if (substitute)
            {
                Request_Substitute.Update(User_id, Request_ID, b);

                if (Request_Substitute.AppRejFromAllSub(Request_ID))
                {
                    if (Autor_id == AD.CEO.Login)
                    {
                        Approve_reject.Add(AD.CFO.Login, Request_ID, "ViewITO.aspx");
                    }
                    
                    else if (autor.IsDirector)
                    {
                        Approve_reject.Add(AD.CEO.Login, Request_ID, "ViewITO.aspx");
                    }
                    else
                    {
                        string manager = autor.Manager.Login;
                        Approve_reject.Add(manager, Request_ID, "ViewITO.aspx");
                    }
                }
            }
            else if (isManager1)
            {
                if (b)
                {
                    List<string> newusers = Request_Substitute.DelateByRId(Request_ID, spPeoplePicker.CommaSeparatedAccounts);

                    if (newusers.Count > 0)
                    {
                        foreach (string s in newusers)
                        {
                            Request_Substitute.Add2(s, Request_ID, Autor_id, Start_date, End_date, true);
                            string msg = string.Format("<b>{0}</b> {2}<b>{1}</b> wants you replace {3}", Cur.FullName, Cur.Department, Cur.Department == null ? "" : "from ", autor.FullName); // YUPE IP
                            Notificaion.Add(s, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                        }

                        string manager = AD.GetUserDirector(User_id).Login;

                        Approve_reject.UpdateParent(User_id, Request_ID, b);

                        Approve_reject.Add(manager, Request_ID, "ViewITO.aspx");
                    }
                    else
                    {
                        string manager = AD.GetUserDirector(Autor_id).Login;

                        Approve_reject.UpdateParent(User_id, Request_ID, b);

                        Approve_reject.Add(manager, Request_ID, "ViewITO.aspx");
                    }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager2)
            {

                Approve_reject.UpdateParent(User_id, Request_ID, b);

                string manager = AD.CEO.Login;

                if (b) { Approve_reject.Add(manager, Request_ID, "ViewITO.aspx"); }// popoxumenq
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager3)
            {
                Approve_reject.UpdateParent(User_id, Request_ID, b);

                string manager = AD.CFO.Login;

                if (b) { Approve_reject.Add(manager, Request_ID, "ViewITO.aspx"); }// popoxumenq
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager4)
            {
                if (ITO.UpdateWorkDaysCount(Request_ID, TextBoxWorkDays.Text) > 0)
                {

                }
                Approve_reject.UpdateParent(User_id, Request_ID, b);

                request.Update(Request_ID, b);
                if (b) end = 1;
                else end = -1;
            }

            if (end == 1 || end == -1)
            {
                string TypeName = Request_type.GetTypeName(Request_ID);

                if (end == 1)
                {
                    string s = "Your";

                    string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                    Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));

                }
                else if (end == -1)
                {
                    string s = "Your";

                    string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>rejected</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                    Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
            }
            if (end == 1)
            {
                ITO.Update(Request_ID);

                string TName = Request_type.GetTypeName(Request_ID);

                string ms = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", autor.FullName + "’s", TName, Request_type.GetUppers(TName), Request_ID);

                foreach (EDF_SPUser us in AD.GetGroupUsers(AD.GetGroupNameByKey("EDF_ITR_admin")))
                {
                    Notificaion.Add(us.Login, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                    if (us.HasReplacement)
                        Notificaion.Add(us.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
                foreach (EDF_SPUser us in AD.GetGroupUsers(AD.GetGroupNameByKey("EDF_ITR-LTR_accountant")))
                {
                    Notificaion.Add(us.Login, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                    if (us.HasReplacement)
                        Notificaion.Add(us.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, ms, autor.PictureUrl, Request_type.GetId(Request_ID));
                }
            }
            if (b)
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            else
            {
                Approve_reject.UpdateParent(User_id, Request_ID, b);
                request.Update(Request_ID, b);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
        }

        void Draw()
        {
            LblType.Text = Request_type.GetTypeName(Request_ID);
            LabelName.Text = autor.FullName;
            LabelDep.Text = autor.Department;

            string ss = "0";
            try
            {
                autorImg.Src = autor.PictureUrl;
                ss = "1";

                table = EDF.GetITO(Request_ID);
                ss = "2";
                string City = table.Rows[0]["City"].ToString();
                string Organization = table.Rows[0]["Organization"].ToString();
                string StartDate = table.Rows[0]["Start_Date"].ToString();
                string EndDate = table.Rows[0]["End_Date"].ToString();
                string Purpose = table.Rows[0]["Purpose"].ToString();
                string Budgeted = table.Rows[0]["Budgeted"].ToString().ToLower() == "true" ? "Yes / Այո" : "No / Ոչ";
                string Amount = table.Rows[0]["Amount"].ToString();
                string ReplacementId = table.Rows[0]["Replacement_Id"].ToString();
                string Daily = table.Rows[0]["Daily"].ToString().ToLower() == "true" ? "Requested / Պահանջվում է" : "Not requested / Չի պահանջվում";
                string Hotel = table.Rows[0]["Hotel"].ToString().ToLower() == "true" ? "Required / Պահանջվում է" : "Not required / Չի պահանջվում";

                string HotelName = table.Rows[0]["Hotel_Name"].ToString();
                string HotelDates = table.Rows[0]["Hotel_Dates"].ToString();
                string HotelLocation = table.Rows[0]["Hotel_Location"].ToString();
                string HotelPhone = table.Rows[0]["Hotel_Phone"].ToString();
                string HotelPayment = table.Rows[0]["Hotel_Payment"].ToString();

                int i = 1;
                foreach (string c in City.Split(new string[] { "|$$|" }, StringSplitOptions.None))
                {
                    if (c != "")
                        ul_Rout.InnerHtml += string.Format("<li>   {1}.   {0}</li>", c, (i++).ToString());
                }
                ss = "5";
                i = 1;
                foreach (string org in Organization.Split(new string[] { "|$$|" }, StringSplitOptions.None))
                {
                    if (org != "")
                        ul_Inviting.InnerHtml += string.Format("<li>   {1}.   {0}</li>", org, (i++).ToString());
                }
                if (i == 1)
                    ul_Inviting.InnerHtml += "<li>   No / Ոչ </li>";
                ss = "6";

                lb_DateFrom.Text = DateTime.Parse(StartDate).ToString("dd/MM/yyyy");
                lb_DateTo.Text = DateTime.Parse(EndDate).ToString("dd/MM/yyyy");
                lb_Purpose.InnerText = Purpose;
                lb_Costs.InnerText = Amount;
                lb_Budgeted.InnerText = Budgeted;
                lb_Daily.InnerText = Daily;
                lb_Hotel.InnerText = Hotel;
                ss = "7";
                if (!Hotel.ToLower().Contains("not required"))
                {
                    HotelDiv.Visible = true;
                    lb_Hotel_Name.InnerText = HotelName;
                    lb_Hotel_Location.InnerText = HotelLocation;
                    lb_Hotel_Dates.InnerText = HotelDates;

                    if (string.IsNullOrEmpty(HotelPayment))
                        TRpayment.Visible = false;
                    else
                    {
                        TRpayment.Visible = true;
                        lb_Hotel_Payment.InnerText = HotelPayment;
                    }

                    if (string.IsNullOrEmpty(HotelPhone))
                        TRphon.Visible = false;
                    else
                    {
                        TRphon.Visible = true;
                        lb_Hotel_Phone.InnerText = HotelPhone;
                    }
                }
                else
                    HotelDiv.Visible = false;
                for (int f = 0; f < table.Rows[0]["Fly_Date"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.RemoveEmptyEntries).Length; f++)
                {

                    DivFlightDetails.InnerHtml += string.Format(@" 
            
            <div style='border:1px solid #ebebeb'>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                Date / Ամսաթիվ
                            </td>
                            <td>
                                <span>{0}</span>
                            </td>                           
                        </tr> 
                        <tr>
                            <td>
                                Airline / Ավիաընկերություն
                            </td>
                            <td>
                                <span>{1}</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Flight number / Թռիչքի համար
                            </td>
                            <td>
                                <span>{2}</span>
                            </td>
                        </tr> 
                        <tr>
                            <td>
                                Departure city / Մեկնման քաղաք
                            </td>
                            <td>
                                <span>{3}</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Destination city / Ժամանման քաղաք
                            </td>
                            <td>
                                <span>{4}</span>
                            </td>
                        </tr>                
                    </tbody>
                </table>
            </div>",
            DateTime.Parse(table.Rows[0]["Fly_Date"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f]).ToString("dd/MM/yyyy"),
            table.Rows[0]["Fly_Airline"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
            table.Rows[0]["Fly_Number"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
            table.Rows[0]["Fly_Departure_City"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f],
            table.Rows[0]["Fly_Destination_City"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.None)[f]
        );

                }
                ss = "8";
            }
            catch (Exception ex)
            {
                Response.Write(string.Format("Error {0}  ex: {1}", ss, ex.Message));
            }
        }

        protected void PrintLink_Click(object sender, EventArgs e)
        {
            string url = string.Format("{0}/SitePages/Print.aspx?rid={1}", SPContext.Current.Web.Url, Request_ID);

            Response.Redirect(url);
        }

        protected void ComSend_Click(object sender, EventArgs e)
        {
            if (ComText.Value != "" && ComText.Value != "Write your comment:")
                Comments.Add(Request_ID, ComText.Value, ADSP.CurrentUser.Login, false);


            ComText.Value = "Write your comment:";
            DataTable tableCom = Comments.Get(Request_ID);

            DrawComents(tableCom);

            string TypeName = Request_type.GetTypeName(Request_ID);

            DataTable users1 = request.GetAllUsers_ids(Request_ID);

            for (int i = 0; i < users1.Rows.Count; i++)
            {
                if (User_id == users1.Rows[i]["Autor_id"].ToString()) continue;

                string s = string.Empty;

                if (Autor_id == users1.Rows[i]["Autor_id"].ToString())
                { s = "your"; }
                else if (Autor_id == User_id)
                { s = "his/her"; }
                else { s = autor.FullName + "’s"; }

                string msg = string.Format("<b>{0}</b> {4}<b>{1}</b> commented on <b>{6}</b> {2} (ID: {5} {3})", Cur.FullName, Cur.Department, TypeName, Request_ID, (Cur.Department == null ? "" : "from "), Request_type.GetUppers(TypeName), s);

                Notificaion.Add(users1.Rows[i]["Autor_id"].ToString(), SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 1);
            }
        }

        public void DrawComents(DataTable table)
        {
            divCom.InnerHtml = string.Empty;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string t1 = table.Rows[i]["MSG"].ToString();
                string t2 = DateTime.Parse(table.Rows[i]["Add_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).ToString();
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

        protected void ButtonWorkDays_Click(object sender, EventArgs e)
        {

        }

        protected void LBDSPDF_Click(object sender, EventArgs e)
        {
            if (PDF.hasSertificatedFile(Request_ID))
            {
                Response.Redirect(PDF.GetFromSPFolder(Request_ID));
            }
        }

        protected void LBPDF_Click(object sender, EventArgs e)
        {

            String htmlText = DrawPageITO(Request_ID);

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Response.Clear();

                Response.ContentType = "application/pdf";

                Response.WriteFile(PDF_DOC.Create(htmlText, "ITO", Request_ID));

            });
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

        static string DrawPageITO(string Request_ID)
        {
            string htmlText = string.Empty;
            string Autor_id = string.Empty;
            EDF_SPUser User = null;
            string order = ITO.GetNumber(Request_ID);

            Autor_id = request.GetAutor_id(Request_ID);

            DataTable table = EDF.GetITO(Request_ID);

            string FlightDetails = string.Empty;

            for (int f = 0; f < table.Rows[0]["Fly_Date"].ToString().Split(new string[] { "|$$|" }, StringSplitOptions.RemoveEmptyEntries).Length; f++)
            {

                FlightDetails += string.Format(@" 
            
<br/>&nbsp;&nbsp;&nbsp;&nbsp;Date / Ամսաթիվ: <span Style='text-decoration: underline;'>{0}</span>
<br/>&nbsp;&nbsp;&nbsp;&nbsp;Airline / Ավիաընկերություն: <span Style='text-decoration: underline;'>{1}</span>
<br/>&nbsp;&nbsp;&nbsp;&nbsp;Flight number / Թռիչքի համար: <span Style='text-decoration: underline;'>{2}</span>
<br/>&nbsp;&nbsp;&nbsp;&nbsp;Departure city / Մեկնման քաղաք: <span Style='text-decoration: underline;'>{3}</span>
<br/>&nbsp;&nbsp;&nbsp;&nbsp;Destination city / Ժամանման քաղաք: <span Style='text-decoration: underline;'>{4}</span>
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
<br/>&nbsp;Hotel details / Հյուրանոցի տվյալներ:
<br/>&nbsp;&nbsp;&nbsp;&nbsp; Name / Անուն :  <span Style='text-decoration: underline;'>{0}</span>
<br/>&nbsp;&nbsp;&nbsp;&nbsp; Dates of stay / Մնալու ժամանակահատված :  <span Style='text-decoration: underline;'>{1}</span>
<br/>&nbsp;&nbsp;&nbsp;&nbsp; Location / Վայր :  <span Style='text-decoration: underline;'>{2}</span>
    {3}
    {4}
",
                table.Rows[0]["Hotel_Name"].ToString(),
                table.Rows[0]["Hotel_Dates"].ToString(),
                table.Rows[0]["Hotel_Location"].ToString(),
                string.IsNullOrEmpty(table.Rows[0]["Hotel_Phone"].ToString()) ? "" : "<br/>&nbsp;&nbsp;&nbsp;&nbsp; Phone number / Հեռախոս :  <span Style='text-decoration: underline;'>" + table.Rows[0]["Hotel_Phone"].ToString() + "</span>",
                string.IsNullOrEmpty(table.Rows[0]["Hotel_Payment"].ToString()) ? "" : "<br/>&nbsp;&nbsp;&nbsp;&nbsp; Payment method / Վճարման ձև : <span Style='text-decoration: underline;'>" + table.Rows[0]["Hotel_Payment"].ToString() + "</span>"
                    );

            }


            bool budgeted = (bool)table.Rows[0]["Budgeted"];

            string amount = table.Rows[0]["Amount"].ToString();
            string am = string.Empty;
            if (!string.IsNullOrEmpty(amount))
            {
                am = string.Format(@"
                                        <br/>Ծախսերը, որոնք կատարվում են հրավիրող կողմի միջոցներով: <span Style='text-decoration: underline;'>{0}</span>
                                        <br/>Costs covered by the inviter", amount);
            }

            string rr = table.Rows[0]["Replacement_Id"].ToString();
            string replacement = string.Empty;
            if (!string.IsNullOrEmpty(rr))
            {
                string rep = AD.GetUserBySPLogin(rr).FullName;

                replacement = string.Format(@"
                                        <br/>Ո՞վ է փոխարինելու: <span Style='text-decoration: underline;'>{0}</span>
                                        <br/>Who will replace", rep);
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
                apprej += string.Format("<li>{0} from {1}<br/><br/></li>", us.FullName, us.Department);
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
                <br/>Հրավիրող կազմակերպություն 1: <span Style='text-decoration: underline;'>{0}</span>
                    <br/>Inviting organization 1 (optional)
                <br/>", organization);

            if (i == 1) { organization = Org = string.Empty; }


            string substitut = Request_Substitute.Get_User_Names(Request_ID, Autor_id);

            if (substitut.Length > 0)
                substitut = "<p><b>3. Ո՞վ է փոխարինելու / Who will replace</b>  </p>" + substitut;
            try
            {
                User = AD.GetUserByLogin(Autor_id);
                string style = string.Empty;

                htmlText = string.Format(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
            <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">

                <head>
                    <style> {8} </style>
                    <title> </title>
                </head>

                <body>
<font face = ""arial unicode ms"">
                    <div class=""content"">

                        <div >
 <span Style=""color:#F60; font-size: 16pt; font-weight:700; text-align:center; white-space:normal;"">

                            <p>Միջազգային գործուղման հրաման <br/> International travel order </p>
                                            
</span>  
                        </div>
                        
                        <br/>     
                       
<br/><span Style='text-decoration: underline;'>{10} , {11}</span>
<br/>(ազգանունը, անունը, հայրանունը, պաշտոնը)
<br/>Name of employee and title
<br/>                              
<br/>Հրաման N `: <span Style='text-decoration: underline;'>Գ{0}</span>
<br/>Order
<br/>                           
<br/>Գործուղվում է`: <span Style='text-decoration: underline;'>{1}</span>
<br/>Dates of travel
<br/>
    {6}

<br/>Լրացնելու ամսաթիվ: <span Style='text-decoration: underline;'>{9}</span>
<br/>filling date  
<br/>
<br/>Գործուղման նպատակը: <span Style='text-decoration: underline;'>{5}</span>
<br/>Purpose of business trip
<br/>
<br/>Գործուղման ժամկետը:    Սկսած` <span Style='text-decoration: underline;'>{2} </span> &nbsp;  Մինչև` <span Style='text-decoration: underline;'>{3} </span>  (ներառյալ)
<br/>Duration of business trip      
<br/>
<br/>Բյուջետավորված: <span Style='text-decoration: underline;'>{13}</span>
<br/>Budgeted
<br/>
                {14}
<br/>
                {15}
<br/>
<br/>Օրապահիկ: <span Style='text-decoration: underline;'>{16}</span>
<br/>Daily allowance
<br/>
<br/>Հյուրանոց: <span Style='text-decoration: underline;'>{17}</span>
<br/>Hotel
<br/>           {18}
<br>
<br/>Թռիչքի տվյալներ / Flight details :
<br/>
                {19}
<br/>
                        <p><b>Հաստատված է/approved by</b></p>
                        {7}

                    </div>
</font>
                </body>
            </html>",
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
                                    FlightDetails
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
