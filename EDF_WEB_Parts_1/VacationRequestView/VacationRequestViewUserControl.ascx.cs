using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_WEB_Parts_1.VacationRequestView
{
    public partial class VacationRequestViewUserControl : UserControl
    {
        readonly string connectionString = Constants.GetConnectionString();

        string requestId;
        bool substitute, isManager1, isManager2, isManager3, isManager4;
        string userId;
        EDF_SPUser currentUser = ADSP.CurrentUser;
        EDF_SPUser autor;
        DateTime startDate;
        DateTime endDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            UC.SearchBox(ComText, "Write your comment:");

            HiddenFieldspPeoplePicker.Value = spPeoplePicker.CommaSeparatedAccounts;
            string str = string.Empty;

            try
            {
                userId = currentUser.Login;

                requestId = Request.QueryString["rid"];
                if (requestId == null)
                    ER.GoToErrorPage("Request Id not found");


                if (!Approve_reject.canView(currentUser, requestId))
                {
                    Response.Redirect(SPContext.Current.Site.Url);
                }

                if (!PDF.hasSertificatedFile(requestId))
                {
                    cert_icon.Visible = false;
                    LBDSPDFDIV.Visible = false;
                }

                string orderN = GetVacationNumber(requestId);

                if (orderN == "0")
                    OrderSpan.Visible = PrintDiv.Visible = DivPDF.Visible = false;
                else
                {
                    OrderSpan.Visible = true;
                    OrderLabel.Text = orderN;
                    if (currentUser.IsHR)
                        PrintDiv.Visible = true;
                    else
                        PrintDiv.Visible = false;

                    if (currentUser.GetDocAccess(1))
                        DivPDF.Visible = true;
                    else DivPDF.Visible = false;
                }

                AppRejDiv.Visible = ButtonCancel.Visible = ButtonSubmit.Visible = CanAppRej(requestId, currentUser.Login);

                string NId = Request.QueryString["nid"];
                Notificaion.Update(NId, true);

                string Autor_id = GetAutor_id(requestId);

                autor = AD.GetUserBySPLogin(Autor_id);

                UserProf.HRef = @"http://intranet/SitePages/Person.aspx?accountname=FTA\" + autor.Login;

                bool t = false;

                #region You already approved / It's your request

                switch (IsActive2(requestId, userId))
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


                switch (IsActive(requestId, userId))
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
                ////rep
                if (currentUser.ParentReplacement.Count > 0 && !t)
                {
                    foreach (EDF_SPUser us in currentUser.ParentReplacement)
                    {
                        switch (IsActive(requestId, us.Login))
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
                foreach (EDF_SPUser us in currentUser.ParentReplacement)
                {
                    if (Approve_reject.CanAppRejLTR_Null(requestId, us.Login))
                        rp = true;
                }
                if (request.IsYour(requestId, currentUser.Login))
                {
                    if (!(Approve_reject.CanAppRejLTR_Null(requestId, Autor_id) || rp))
                    {
                        Label1.Text = "<b>It's your request</b>";
                        Label1.Visible = true;

                        AppRejDiv.Visible = false;
                    }
                }
                str += ":1:";

                #endregion

                #region     // get status

                string curStatus = Approve_reject.GetStatus(userId, requestId);
                if (!string.IsNullOrEmpty(curStatus))
                {
                    switch (curStatus)
                    {
                        case "manager":
                            isManager1 = true;
                            break;
                        case "substitute":
                            substitute = true;
                            break;
                        case "director":
                            isManager2 = true;
                            break;
                        case "CEO":
                            isManager3 = true;
                            break;
                        case "HR":
                            isManager4 = true;
                            break;
                    }

                    if (!Approve_reject.CanAppRejLTR_Null(requestId, Autor_id))
                    {
                        foreach (EDF_SPUser us in currentUser.ParentReplacement)
                        {
                            if (Approve_reject.CanAppRejLTR_Null(requestId, us.Login))
                            {
                                string cus = us.Login;

                                curStatus = Approve_reject.GetStatus(cus, requestId);

                                switch (curStatus)
                                {
                                    case "manager":
                                        isManager1 = true;
                                        break;
                                    case "substitute":
                                        substitute = true;
                                        break;
                                    case "director":
                                        isManager2 = true;
                                        break;
                                    case "CEO":
                                        isManager3 = true;
                                        break;
                                    case "HR":
                                        isManager4 = true;
                                        break;
                                }
                                foreach (string s in GetRequest_Substitute_User_id_NULL(requestId, Autor_id).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (s == currentUser.Login)
                                        substitute = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    #region OLD
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        if (autor.HasManager)
                        {
                            str += ":1:";

                            str += currentUser.Login;
                            str += ":2:";
                            str += autor.Manager.Login;
                            str += ":1:";

                            isManager1 = (currentUser.Login == autor.Manager.Login);
                            str += ":1:";
                        };
                        str += ":1:";
                        foreach (string s in GetRequest_Substitute_User_id_NULL(requestId, Autor_id).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (s == currentUser.Login)
                                substitute = true;
                        }
                        str += ":1:";
                        EDF_SPUser dep = null;

                        if (autor.HasDirector)
                        {
                            str += ":7:";
                            dep = autor.Director;
                            str += ":8:";
                            dep = AD.GetUserDirector(Autor_id);
                            str += ":9:";
                        }
                        else
                        {
                            dep = null;
                            str += ":911:";
                        }

                        str += ":9222:";

                        str += ":10:";
                        if (dep != null)
                        {
                            str += ":94444:";
                            isManager2 = (currentUser.Login == dep.Login);
                            str += ":9444455:";
                        }
                        else
                            isManager2 = false;
                        if (isManager2) isManager1 = false;
                        str += ":1:";
                        isManager3 = (currentUser.Login == AD.CEO.Login);
                        str += ":1:";
                        if (isManager3) isManager1 = isManager2 = false;
                        str += ":1:";

                        if ((currentUser.Login == AD.HR.Login) && autor.Login != AD.HR.Login)
                            isManager4 = true;
                        else if (currentUser.Login == AD.HR.Login && autor.Login == AD.HR.Login && EDF.GetAproveRejectState(requestId, currentUser.Login) == -1)
                            isManager4 = true;
                        else
                            isManager4 = false;
                        str += ":1:";
                        if (isManager4) isManager1 = isManager2 = isManager3 = false;
                        str += ":1:";

                    });


                    if (!CanAppRejIsNull(requestId, userId))
                    {
                        foreach (EDF_SPUser us in currentUser.ParentReplacement)
                        {
                            if (CanAppRejIsNull(requestId, us.Login))
                            {

                                SPSecurity.RunWithElevatedPrivileges(delegate()
                                {
                                    if (autor.HasManager)
                                    {

                                        isManager1 = (us.Login == autor.Manager.Login);

                                    };

                                    foreach (string s in GetRequest_Substitute_User_id_NULL(requestId, Autor_id).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (s == us.Login)
                                            substitute = true;
                                    }

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

                                    isManager3 = (us.Login == AD.CEO.Login);

                                    if (isManager3) isManager1 = isManager2 = false;


                                    if ((us.Login == AD.HR.Login) && autor.Login != AD.HR.Login)
                                        isManager4 = true;
                                    else if (us.Login == AD.HR.Login && autor.Login == AD.HR.Login && EDF.GetAproveRejectState(requestId, us.Login) == -1)
                                        isManager4 = true;
                                    else
                                        isManager4 = false;

                                    if (isManager4) isManager1 = isManager2 = isManager3 = false;

                                });

                                break;
                            }
                        }
                    }
                    #endregion
                }

                #endregion

                if (isManager4 && !IsPostBack)
                {
                    int workdayscount = getWorkDaysCount(requestId);
                    if (workdayscount == 0)
                        TextBoxWorkDays.Text = string.Empty;
                    else
                        TextBoxWorkDays.Text = workdayscount.ToString();

                    WorkDaysCount.Style.Add(HtmlTextWriterStyle.Display, "block");
                }
                else if (!IsPostBack)
                    TextBoxWorkDays.Text = getWorkDaysCount(requestId).ToString();


                str += ":1:";
                DataTable table = LableType(requestId);
                #region vacation
                str += ":1:";
                EDF_SPUser autorO = AD.GetUserBySPLogin(table.Rows[0]["Autor_id"].ToString());
                str += ":1:";
                LabelName.Text = autorO.FullName;
                LabelDep.Text = autorO.Department;
                LblType.Text = LblType1.Text = table.Rows[0]["typeName"].ToString();
                userImg.Src = autor.PictureUrl;
                str += ":1:";
                RadioButton0.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Annual");
                RadioButton1.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Pregnancy");
                RadioButton2.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Non-paid");
                RadioButton3.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Exam");
                RadioButton4.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Marriage");
                RadioButton5.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Close");
                RadioButton6.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Relative’s death");
                RadioButton7.Checked = table.Rows[0]["Vacation_type"].ToString().Contains("Non-paid vacation");
                str += ":1:";
                if (table.Rows[0]["Days_offs"].ToString().Length > 0)
                {
                    CheckDiv.Visible = true;
                    CheckBox1.Checked = table.Rows[0]["Days_offs"].ToString().Contains("1");
                    CheckBox2.Checked = table.Rows[0]["Days_offs"].ToString().Contains("2");
                    CheckBox3.Checked = table.Rows[0]["Days_offs"].ToString().Contains("3");
                    CheckBox4.Checked = table.Rows[0]["Days_offs"].ToString().Contains("4");
                    CheckBox5.Checked = table.Rows[0]["Days_offs"].ToString().Contains("5");
                    CheckBox6.Checked = table.Rows[0]["Days_offs"].ToString().Contains("6");
                    CheckBox7.Checked = table.Rows[0]["Days_offs"].ToString().Contains("7");
                }
                else
                {
                    CheckDiv.Visible = false;
                }
                str += ":1:";

                startDate = (DateTime)table.Rows[0]["Start_date"];
                str += ":8:";
                endDate = (DateTime)table.Rows[0]["End_date"];
                str += ":1:";
                dateFrom.Text = " " + startDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                dateTo.Text = " " + endDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                CheckBox8.Checked = table.Rows[0]["Payment_terms"].ToString().ToLower() == "true";
                #endregion
                if (!IsPostBack)
                {
                    spPeoplePicker.CommaSeparatedAccounts = GetRequest_Substitute_User_id(requestId, Autor_id);
                    spPeoplePicker.Validate();
                }
                str += ":1:";
                if (isManager1 && t && ApprovedRejectedFromAllSubstitute(requestId))
                    spPeoplePicker.Enabled = spPeoplePicker.IsChanged = true;
                else
                    spPeoplePicker.Enabled = spPeoplePicker.IsChanged = false;
                str += ":1:";
                DataTable tableCom = Comments.Get(requestId);
                str += ":1:";
                DrawComents(tableCom);
            }
            catch (Exception ex)
            {
                //  ER.GoToErrorPage(" - ViewReplace pageload -  <br/> " +str+"</b>"+ ex.Message);
                Response.Write(string.Format("ERROR {0} EX:{1} - ERROR", str, ex.Message));
                return;
            }
            #region  //          SAQO        //////
            string ss = "0";
            try
            {
                ss = "1";
                ul_history.InnerHtml = string.Empty;
                string rid = Request.QueryString["rid"].ToString();
                ss = "2";
                bool IsPendTitle = true;
                bool IsPastTitle = true;
                foreach (EDF_User u in EDF.AssociationUsers(rid))
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

                divPayment.Visible = CheckBox8.Checked;
            }
            catch (Exception ex)
            {
                //ER.GoToErrorPage(string.Format("History error | {0} | EX:{1} ", ss, ex.Message));
                Response.Write(string.Format("ERROR - History {0} EX:{1} - ERROR", ss, ex.Message));
            }
            #endregion
        }

        public void DrawComents(DataTable table)
        {
            divCom.InnerHtml = string.Empty;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string t1 = table.Rows[i]["MSG"].ToString();
                string t2 = DateTime.Parse(table.Rows[i]["Add_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).ToString().ToString();
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
            comImg.Src = currentUser.PictureUrl;
        }

        public DataTable LableType(string Request_ID)
        {
            string comand = string.Format(
                "select * , " +
                "(select Name from Request_type where Request.Type_id=Request_type.ID) as typeName, " +
                "(select ImgUrlOrange from Request_type where Request.Type_id=Request_type.ID) as typeImg, " +
                "(select Start_date from vacation where vacation.REQUEST_ID='{0}') as Start_date, " +
                "(select End_date from vacation where vacation.REQUEST_ID='{0}') as End_date, " +
                "(select Payment_terms from vacation where vacation.REQUEST_ID='{0}') as Payment_terms, " +
                "(select Days_offs from vacation where vacation.REQUEST_ID='{0}') as Days_offs, " +
                "(select Vacation_type from vacation where vacation.REQUEST_ID='{0}') as Vacation_type " +
                "from Request where Id='{0}'",
                Request_ID
                );

            DataTable table = new DataTable("Request");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequests -  <br/> " + ex.Message);
            }

            return table;
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            bool hasParent = false;
            if (currentUser.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in currentUser.ParentReplacement)
                    if (CanAppRej(requestId, p.Login))
                        hasParent = true;
            }

            if (CanAppRej(requestId, currentUser.Login) || hasParent)
            { }
            else
                return;

            string Message = string.Empty;

            Button bttn = sender as Button;

            bool b = bttn.Text == "Approve Request";

            string Autor_id = GetAutor_id(requestId);

            Int16 end = 0;

            if (b)
                Message = string.Format("<b>{0}</b> {6}<b>{1}</b> approved <b>{2}’s</b> {3} (ID: {4} {5} )", currentUser.FullName, currentUser.Department, autor.FullName, Request_type.GetTypeName(requestId), Request_type.GetUppers(Request_type.GetTypeName(requestId)), requestId, currentUser.Department == null ? "" : "from ");
            else
                Message = string.Format("<b>{0}</b> {6}<b>{1}</b> has rejected <b>{2}’s</b> {3} (ID: {4} {5} )", currentUser.FullName, currentUser.Department, autor.FullName, Request_type.GetTypeName(requestId), Request_type.GetUppers(Request_type.GetTypeName(requestId)), requestId, currentUser.Department == null ? "" : "from ");


            if (substitute)
            {
                Request_Substitute.Update(userId, requestId, b);

                if (ApprovedRejectedFromAllSubstitute(requestId))
                {
                    if (autor.IsCEO)
                    {
                        Approve_reject.Add(AD.HR.Login, requestId, "ViewReplacement.aspx", "HR");
                    }
                    else if (autor.IsDirector)
                    {
                        Approve_reject.Add(AD.CEO.Login, requestId, "ViewReplacement.aspx", "CEO");
                    }
                    else if (autor.Manager.IsDirector)
                    {
                        Approve_reject.Add(autor.Manager.Login, requestId, "ViewReplacement.aspx", "director");
                    }
                    else
                    {
                        string manager = autor.Manager.Login;
                        Approve_reject.Add(manager, requestId, "ViewReplacement.aspx", "manager");
                    }
                }
            }
            else if (isManager1)
            {
                if (b)
                {
                    List<string> newusers = DelFromReqSubByRId(requestId, spPeoplePicker.CommaSeparatedAccounts);

                    if (newusers.Count > 0)
                    {
                        foreach (string s in newusers)
                        {
                            Request_Substitute.Add2(s, requestId, Autor_id, endDate, startDate, true);
                            string msg = string.Format("<b>{0}</b> {2}<b>{1}</b> wants you replace {3}", currentUser.FullName, currentUser.Department, currentUser.Department == null ? "" : "from ", autor.FullName); // YUPE IP
                            Notificaion.Add(s, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + requestId, msg, autor.PictureUrl, Request_type.GetId(requestId));

                            EDF_SPUser rus = AD.GetUserBySPLogin(s);

                            if (rus.HasReplacement)
                            {
                                msg = string.Format("<b>{0}</b> {2}<b>{1}</b> wants {4} replace {3}", currentUser.FullName, currentUser.Department, currentUser.Department == null ? "" : "from ", autor.FullName, rus.FullName); // YUPE IP
                                Notificaion.Add(s, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + requestId, msg, autor.PictureUrl, Request_type.GetId(requestId));

                            }
                        }

                        Approve_reject.UpdateParent(userId, requestId, b);


                        Approve_reject.Add(autor.Director.Login, requestId, "ViewReplacement.aspx", "director");
                    }
                    else
                    {
                        Approve_reject.UpdateParent(userId, requestId, b);

                        Approve_reject.Add(autor.Director.Login, requestId, "ViewReplacement.aspx", "director");
                    }
                }
                else
                {
                    request.Update(requestId, b);
                    end = -1;
                }
            }
            else if (isManager2)
            {
                Approve_reject.UpdateParent(userId, requestId, b);

                if (b) { Approve_reject.Add(AD.HR.Login, requestId, "ViewReplacement.aspx", "HR"); }
                else
                {
                    UpdateRequest(requestId, b);
                    end = -1;
                }
            }
            else if (isManager3)
            {
                Approve_reject.UpdateParent(userId, requestId, b);

                if (b) { Approve_reject.Add(AD.HR.Login, requestId, "ViewReplacement.aspx", "HR"); }
                else
                {
                    UpdateRequest(requestId, b);
                    end = -1;
                }
            }
            else if (isManager4)
            {
                if (UpdateWorkDaysCount(requestId, TextBoxWorkDays.Text) > 0)
                {

                }
                Approve_reject.UpdateParent(userId, requestId, b);

                request.Update(requestId, b);
                if (b)
                {
                    end = 1;
                    UpdateVacation(requestId);
                }
                else end = -1;
            }

            if (end == 1 || end == -1)
            {
                string TypeName = Request_type.GetTypeName(requestId);

                if (end == 1)
                {
                    string s = "Your";

                    string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", s, TypeName, Request_type.GetUppers(TypeName), requestId);
                    Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + requestId, msg, autor.PictureUrl, Request_type.GetId(requestId));

                }
                else if (end == -1)
                {
                    string s = "Your";

                    string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>rejected</b>", s, TypeName, Request_type.GetUppers(TypeName), requestId);
                    Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + requestId, msg, autor.PictureUrl, Request_type.GetId(requestId));
                }

            }
            if (b)
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            else
            {
                Approve_reject.UpdateParent(userId, requestId, b);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
        }

        private int UpdateWorkDaysCount(string Request_ID, string p)
        {

            string comand = "Update vacation SET Work_days = " + p + " WHERE REQUEST_ID = '" + Request_ID + "'";
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                return com.ExecuteNonQuery();
            }
            catch { }
            return -1;
        }

        public int getWorkDaysCount(string Request_ID)
        {
            string comand = "SELECT * FROM vacation WHERE REQUEST_ID = '" + Request_ID + "'";
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    int count = (int)dr["Work_days"];
                    //if (count == null)
                    //    return 0;
                    //else
                    return count;
                }
            }
            catch { }
            return 0;
        }

        public string GetAutor_id(string Request_ID)
        {
            string Autor_id = "-1";

            string comand = "SELECT TOP 1 Autor_id FROM Request WHERE " +
                "Id='" + Request_ID + "'";
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Autor_id = reader["Autor_id"].ToString();
                    }
                }
                else
                {
                    Autor_id = "-1";
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetAutor_id -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return Autor_id;
        }

        public void UpdateRequest_Substitute(string User_id, string Request_Id, bool Is_ok)
        {
            string comand = string.Format("UPDATE [dbo].[Request_Substitute]  SET [dbo].[Request_Substitute].Is_ok = '{0}', [dbo].[Request_Substitute].Is_ok_date = '{3}' where User_id = '{1}' and REQUEST_ID = '{2}'", Is_ok, User_id, Request_Id, DateTime.Now);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateRequest_Substitute -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool ApprovedFromAllSubstitute(string Request_Id)
        {
            string comand = string.Format("select ID from Request_Substitute where REQUEST_ID = '{0}' and Is_ok <> '{1}'", Request_Id, true);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - ApprovedFromAllSubstitute -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return true;
        }

        public bool ApprovedRejectedFromAllSubstitute(string Request_Id)
        {
            string comand = string.Format("select ID from Request_Substitute where REQUEST_ID = '{0}' and Is_ok IS NULL", Request_Id);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - ApprovedFromAllSubstitute -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return true;
        }

        public void _AddApprove_reject(string User_ID, string Request_ID)
        {
            string comand = string.Format("INSERT INTO Approve_reject (User_ID, Request_ID, Date_add) VALUES ('{0}','{1}','{2}')", User_ID, Request_ID, DateTime.Now);
            string msg = "";
            string typeN = "";
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();

                typeN = Request_type.GetTypeName(Request_ID);

                msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to you</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID); // YUPE IP

            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddApprove_reject -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            _AddNotificaion(User_ID, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));


            EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);

            if (rus.HasReplacement)
            {
                msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to {4}</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID, rus.FullName); // YUPE IP

                _AddNotificaion(rus.Replacement.Login, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
            }
        }

        public void DelApprove_reject(string User_ID, string Request_ID)
        {
            SqlConnection con = new SqlConnection(connectionString);
            string delstr = string.Format("delete from Approve_reject where User_ID = '{0}' and Request_ID = '{1}'", User_ID, Request_ID);
            SqlCommand delcom = new SqlCommand(delstr, con);

            try
            {
                con.Open();
                delcom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - DelApprove_reject -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
        }

        public void _UpdateNotificaion(string NId, bool Is_ok)
        {
            string comand = string.Format("UPDATE [dbo].[Notificaion] SET [dbo].[Notificaion].visited = '{0}' where Id = '{1}'", Is_ok.ToString(), NId);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateNotificaion -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void UpdateApprove_reject(string User_id, string Request_Id, bool App_rej)
        {
            string comand = string.Format("UPDATE [dbo].[Approve_reject] SET [dbo].[Approve_reject].App_rej = '{0}', [dbo].[Approve_reject].App_rej_Date = '{1}'  where User_ID = '{2}' and Request_ID = '{3}'", App_rej.ToString(), DateTime.Now, User_id, Request_Id);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateApprove_reject -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void _UpdateApprove_reject(string User_id, string Request_Id, bool App_rej, string RepId)
        {
            string comand = string.Format("UPDATE [dbo].[Approve_reject] SET [dbo].[Approve_reject].App_rej = '{0}', [dbo].[Approve_reject].App_rej_Date = '{1}', [dbo].[Approve_reject].Rep_Id = '{4}'  where User_ID = '{2}' and Request_ID = '{3}'", App_rej.ToString(), DateTime.Now, User_id, Request_Id, RepId);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateApprove_reject2 -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void _UpdateApprove_reject_New(string User_id, string Request_Id, bool App_rej)
        {
            //////////////////////////////////////////////////////////////////////////////////////////// R 
            bool hasParent = false;
            string Par = string.Empty;
            if (currentUser.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in currentUser.ParentReplacement)
                    if (CanAppRej(requestId, p.Login))
                    {
                        hasParent = true;
                        Par = p.Login;
                    }
            }
            if (hasParent)
            {
                _UpdateApprove_reject(Par, requestId, App_rej, currentUser.Login);
            }
            else
            {
                UpdateApprove_reject(User_id, requestId, App_rej);
            }
            //////////////////////////////////////////////////////////////////////////////////////////// R /
        }

        public string GetRequest_Substitute_User_id_NULL(string REQUEST_ID, string For_user_id)
        {
            string Users_ids = string.Empty;

            string comand = string.Format("select User_id from Request_Substitute where " +
                "REQUEST_ID='{0}' AND For_user_id = '{1}' AND Is_ok IS NULL", REQUEST_ID, For_user_id);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {


                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Users_ids += reader["User_id"] + ",";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_Substitute_User_id -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return Users_ids;
        }

        public string GetRequest_Substitute_User_id(string REQUEST_ID, string For_user_id)
        {
            string Users_ids = string.Empty;

            string comand = string.Format("select User_id from Request_Substitute where " +
                "REQUEST_ID='{0}' AND For_user_id = '{1}'", REQUEST_ID, For_user_id);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Users_ids += reader["User_id"] + ",";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_Substitute_User_id -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return Users_ids;
        }

        public string GetRequest_Substitute_User_Names(string REQUEST_ID, string For_user_id)
        {
            string Users_ids = string.Empty;

            string comand = string.Format("select User_id from Request_Substitute where " +
                "REQUEST_ID='{0}' AND For_user_id = '{1}'", REQUEST_ID, For_user_id);
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    Users_ids += "<p>" + AD.GetUserByLogin(reader["User_id"].ToString()).FullName + "</p>";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequest_Substitute_User_id -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return Users_ids;
        }

        public void UpdateRequest(string Request_Id, bool State)
        {
            string comand = string.Format("UPDATE [dbo].[Request] SET [dbo].[Request].State = '{0}' where Id = '{1}'", State.ToString(), Request_Id);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateRequest -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        protected void ComSend_Click(object sender, EventArgs e)
        {
            if (ComText.Value != "" && ComText.Value != "Write your comment:")
                Comments.Add(requestId, ComText.Value, ADSP.CurrentUser.Login, false);


            ComText.Value = "Write your comment:";
            DataTable tableCom = Comments.Get(requestId);

            DrawComents(tableCom);

            string TypeName = _GetType_Name(requestId);

            DataTable users1 = GetAllUsers_ids(requestId);

            for (int i = 0; i < users1.Rows.Count; i++)
            {
                if (currentUser.Login == users1.Rows[i]["Autor_id"].ToString()) continue;

                string s = "";

                if (autor.Login == users1.Rows[i]["Autor_id"].ToString())
                { s = "your"; }
                else if (autor.Login == currentUser.Login)
                { s = "his/her"; }
                else { s = autor.FullName + "’s"; }

                string msg = string.Format("<b>{0}</b> {4}<b>{1}</b> commented on <b>{6}</b> {2} (ID: {5} {3})", currentUser.FullName, currentUser.Department, TypeName, requestId, (currentUser.Department == null ? "" : "from "), Request_type.GetUppers(TypeName), s);

                _AddNotificaion(users1.Rows[i]["Autor_id"].ToString(), SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + requestId, msg, autor.PictureUrl, 1);
            }
        }

        public DataTable GetAllUsers_ids(string REQUEST_ID)
        {
            string comand = string.Format("select Request.Autor_id from Request where Request.Id='{0}' " +
                                          "union " +
                                          "select Approve_reject.User_ID from Approve_reject where Request_ID='{0}' " +
                                          "union " +
                                          "select Approve_reject.Rep_Id from Approve_reject where Rep_Id is not NULL and Request_ID='{0}' " +
                                          "union " +
                                          "select Request_Substitute.User_id from Request_Substitute where REQUEST_ID='{0}'", REQUEST_ID);

            DataTable table = new DataTable("users");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetAllUsers_ids -  <br/> " + ex.Message);
            }

            return table;
        }

        public void DelFromReqSubByRId(string RequestId)
        {
            SqlConnection con = new SqlConnection(connectionString);
            string delstr = string.Format("delete from Request_Substitute where REQUEST_ID = '{0}'", RequestId);
            SqlCommand delcom = new SqlCommand(delstr, con);

            try
            {
                con.Open();
                delcom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - DelFromReqSubByRId -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
        }

        public Int16 IsActive(string RequestId, string UserId)
        {
            Int16 App_rej = -1;
            string comand = string.Format("select App_rej from Approve_reject where (Request_ID = '{0}' and User_ID = '{1}')", RequestId, UserId);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {

                    if (reader["App_rej"].ToString().ToLower() == "true")
                        App_rej = 1;
                    else
                        if (reader["App_rej"].ToString().ToLower() == "false")
                            App_rej = 2;
                        else
                        {
                            App_rej = 0;
                        }
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - IsActive -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return App_rej;
        }

        public Int16 IsActive2(string RequestId, string UserId)
        {
            Int16 Is_ok = -1;
            string comand = string.Format("select Is_ok from Request_Substitute where REQUEST_ID = '{0}' and User_id = '{1}'", RequestId, UserId);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand(comand, con);

                con.Open();

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {

                    if (reader["Is_ok"].ToString().ToLower() == "true")
                        Is_ok = 1;
                    else
                        if (reader["Is_ok"].ToString().ToLower() == "false")
                            Is_ok = 2;
                        else
                        {
                            Is_ok = 0;
                        }
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - IsActive -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return Is_ok;
        }

        public bool IsActive3(string id, string Autor_id)
        {
            string comand = "SELECT Id  FROM Request WHERE " +
                "Autor_id='" + Autor_id + "' and Id = '" + id + "'";
            SqlConnection con = new SqlConnection(connectionString);
            bool l = false;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    l = true;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - IsActive3 -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return l;
        }

        public List<string> DelFromReqSubByRId(string RequestId, string Names)
        {
            List<string> users = new List<string>();
            foreach (string s in Names.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                users.Add(s.Split('\\')[1]);
            }
            string selstr = string.Format("select * from Request_Substitute where REQUEST_ID = '{0}'", RequestId);
            string delstr;
            string countstr;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand selcom = new SqlCommand(selstr, con);
            SqlCommand countcom;

            //SqlDataAdapter da=new SqlDataAdapter(;
            //DataTable da=new DataTable();

            //da.Fill(da);

            delstr = string.Format("delete from Request_Substitute where REQUEST_ID = '{0}' ", RequestId);
            foreach (string u in users)
            {
                delstr += string.Format("and User_id <> '{0}'", u);
            }

            SqlCommand delcom = new SqlCommand(delstr, con);
            List<string> NewUsers = new List<string>();

            try
            {
                con.Open();
                delcom.ExecuteNonQuery();

                foreach (string u in users)
                {
                    countstr = string.Format("select count(*) from Request_Substitute where REQUEST_ID = '{0}' and User_id = '{1}'", RequestId, u);
                    countcom = new SqlCommand(countstr, con);

                    int k = int.Parse(countcom.ExecuteScalar().ToString());

                    if (k == 0)
                        NewUsers.Add(u);

                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - DelFromReqSubByRId -  <br/> " + ex.Message);
            }
            finally { con.Close(); }
            return NewUsers;
        }

        public void UpdateVacation(string Request_Id)
        {
            //string comand = string.Format("if( (YEAR((SELECT TOP 1  [Filling_date] FROM [vacation] WHERE number <> 0 ORDER BY id DESC))) < YEAR(GETDATE())) " +
            //                                    " BEGIN " +
            //                                    " UPDATE vacation SET number=1 WHERE REQUEST_ID = '{0}' " +
            //                                    " END " +
            //                                    " ELSE " +
            //                                    " UPDATE vacation SET number=(SELECT  TOP 1 ([number]+1) FROM [vacation]  WHERE number <> 0 ORDER BY id DESC) WHERE REQUEST_ID = '{0}' ", Request_ID);
            string comand = string.Empty;
            if (DateTime.Now.Year == 2015)
                comand = string.Format(" UPDATE vacation SET number=((SELECT MAX(number) FROM vacation where Filling_date > Cast('2015-03-24' as date)) + 1) WHERE REQUEST_ID = '{0}' ", requestId);
            else
                comand = string.Format(" UPDATE vacation SET number=((SELECT MAX(number) FROM vacation where year(Filling_date) = YEAR(getdate()) ) + 1) WHERE REQUEST_ID = '{0}' ", requestId);
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();

                SqlCommand command = new SqlCommand(comand, con);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - UpdateVacation -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public string GetVacationNumber(string REQUEST_ID)
        {
            string Number = "0";

            string comand = string.Format("select number from vacation WHERE REQUEST_ID = '{0}'", REQUEST_ID);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get Vacation Number - "); }
            finally { con.Close(); }
            return Number;
        }

        public string GetLTRNumber(string REQUEST_ID)
        {
            string Number = "0";

            string comand = string.Format("select Number from [LTR] WHERE REQUEST_ID = '{0}'", REQUEST_ID);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get LTR Number - "); }
            finally { con.Close(); }
            return Number;
        }

        public string GetITONumber(string REQUEST_ID)
        {
            string Number = "0";

            string comand = string.Format("select Number from [ITO] WHERE REQUEST_ID = '{0}'", REQUEST_ID);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Number = reader["number"].ToString();
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - Get ITO Number - "); }
            finally { con.Close(); }
            return Number;
        }

        public bool CanAppRej(string Request_ID, string User_ID)
        {

            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' union select ID from Request_Substitute where REQUEST_ID = '{0}' and User_id = '{1}' AND Is_ok IS NULL union select Id from Request where Id = '{0}'  and Autor_Id = '{1}'", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        public bool CanAppRejIsNull(string Request_ID, string User_ID)
        {

            string comand = string.Format("select ID from Approve_reject where Request_ID = '{0}' and User_ID = '{1}' AND App_rej IS NULL", Request_ID, User_ID);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }

                reader.Close();
            }
            catch { ER.GoToErrorPage(" - CanAppRej - "); }
            finally { con.Close(); }
            return false;
        }

        protected void PrintLink_Click(object sender, EventArgs e)
        {
            string url = string.Format("{0}/SitePages/Print.aspx?rid={1}", SPContext.Current.Web.Url, requestId);

            Response.Redirect(url);
        }

        protected void ButtonWorkDays_Click(object sender, EventArgs e)
        {
            UpdateWorkDaysCount(requestId, TextBoxWorkDays.Text);
        }

        public DataTable _GetComments(string Request_ID)
        {
            string comand = "select * from Comments where Request_ID='" + Request_ID + "'";

            DataTable table = new DataTable("Comments");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch { Response.Write("ERROR - GetComments - ERROR"); }

            return table;
        }

        public void _AddComments(string Request_ID, string MSG, string User_ID, bool see)
        {
            string comand = string.Format("INSERT INTO Comments (Request_ID, MSG, Add_date, User_ID, see) VALUES ('{0}', N'{1}', '{2}','{3}','{4}')", Request_ID, MSG, DateTime.Now, User_ID, see.ToString());

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddComments -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void _AddNotificaion(string User_id, string Visit_url, string notification, string avatar, int Type_id)
        {

            if (currentUser.Login == User_id) return;



            string comand = "INSERT INTO [Notificaion] (User_id, Date_Add, visited, Visit_url, notification, avatar, Type_id) " +
                                         "VALUES ('" + User_id + "', " +
                                                 "'" + DateTime.Now + "', " +
                                                 "'False', " +
                                                 "'" + Visit_url + "', " +
                                                 "'" + notification + "', " +
                                                 "'" + avatar + "', " +
                                                 "'" + Type_id.ToString() + "')";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddNotificaion -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            _sendMail("EDF - Notification", AD.GetUserByLogin(User_id).E_Mail, notification + "<p>URL: <a href='" + Visit_url + "'>" + Visit_url + "</a></p>");

        }

        public void _AddNotificaion(string User_id, string Visit_url, string notification, string avatar, int Type_id, bool type)
        {

            string comand = "INSERT INTO [Notificaion] (User_id, Date_Add, visited, Visit_url, notification, avatar, Type_id) " +
                                         "VALUES ('" + User_id + "', " +
                                                 "'" + DateTime.Now + "', " +
                                                 "'False', " +
                                                 "'" + Visit_url + "', " +
                                                 "'" + notification + "', " +
                                                 "'" + avatar + "', " +
                                                 "'" + Type_id.ToString() + "')";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddNotificaion -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            _sendMail("EDF - Notification", AD.GetUserByLogin(User_id).E_Mail, notification + "<p>URL: <a href='" + Visit_url + "'>" + Visit_url + "</a></p>");
            EDF_SPUser Ruser = AD.GetUserBySPLogin(User_id);
            if (Ruser.HasReplacement)
                _AddNotificaion(Ruser.Replacement.Login, Visit_url, notification, avatar, Type_id, type);
        }

        public void _AddNotificaion(string User_id, string Visit_url, string notification, string avatar, int Type_id, string mailMsg)
        {
            if (currentUser.Login == User_id) return;

            string comand = "INSERT INTO [Notificaion] (User_id, Date_Add, visited, Visit_url, notification, avatar, Type_id) " +
                                         "VALUES ('" + User_id + "', " +
                                                 "'" + DateTime.Now + "', " +
                                                 "'False', " +
                                                 "'" + Visit_url + "', " +
                                                 "'" + notification + "', " +
                                                 "'" + avatar + "', " +
                                                 "'" + Type_id.ToString() + "')";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - AddNotificaion -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            _sendMail("EDF - Notification", AD.GetUserByLogin(User_id).E_Mail, mailMsg + "<p>URL: <a href='" + Visit_url + "'>" + Visit_url + "</a></p>");

            EDF_SPUser Ruser = AD.GetUserBySPLogin(User_id);
            if (Ruser.HasReplacement)
                _AddNotificaion(Ruser.Replacement.Login, Visit_url, notification, avatar, Type_id, mailMsg);
        }

        public string _GetUppers(string s)
        {
            string ss = string.Empty;
            foreach (string w in s.Split(' '))
            {
                ss += w.Substring(0, 1).ToUpper();
            }
            return (ss);
        }

        public void _Request_Substitute(string User_id, string REQUEST_ID, string For_user_id, DateTime End_date, DateTime Start_date)
        {
            string comand = "INSERT INTO [Request_Substitute] (User_id, REQUEST_ID, For_user_id, End_date, Start_date) " +
                                                "VALUES ('" + User_id + "', " +
                                                        "'" + REQUEST_ID + "', " +
                                                        "'" + For_user_id + "', " +
                                                        "'" + End_date + "', " +
                                                        "'" + Start_date + "')";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Request_Substitute -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void _Request_Substitute(string User_id, string REQUEST_ID, string For_user_id, DateTime End_date, DateTime Start_date, bool Is_ok)
        {
            string comand = string.Format("INSERT INTO [Request_Substitute] (User_id, REQUEST_ID, For_user_id, End_date, Start_date, Is_ok, Is_ok_date) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                User_id,
                REQUEST_ID,
                For_user_id,
                End_date,
                Start_date,
                Is_ok.ToString(),
                DateTime.Now
                );

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Request_Substitute2 -  <br/> " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public int _GetType_id(string Request_ID)
        {
            int Type_id = -1;

            string comand = "select Type_id from Request WHERE " +
                "Id = '" + Request_ID + "'";

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);


                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Type_id = (int)reader["Type_id"];
                    }
                }
                else
                {
                    Type_id = -1;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetType_id -  <br/> " + ex.Message);
            }
            finally { con.Close(); }

            return Type_id;
        }

        public string _GetType_Name(string Request_ID)
        {
            string Name = string.Empty;

            string comand = string.Format("SELECT Type_id,(SELECT Request_type.Name FROM Request_type WHERE Request.Type_id = Request_type.ID)as Name FROM Request where Request.Id = '{0}'", Request_ID);

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);


                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Name = reader["Name"].ToString();
                    }
                }
                else
                {
                    Name = string.Empty;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetType_Name -  <br/> " + ex.Message);
            }
            finally { con.Close(); }

            return Name;
        }

        public void _sendMail(string subject, string To, string msg)
        {
            try
            {
                SPWeb oWeb = SPContext.Current.Web;
                SPListItemCollection ips = AD.GetSPListByName("SMTP");


                SmtpClient mySmtpClient = new SmtpClient(ips[0]["IP"].ToString());


                MailAddress from = new MailAddress(ips[0]["Sender email"].ToString(), ips[0]["Sender email"].ToString());
                MailAddress to = new MailAddress(To, To);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);


                myMail.Subject = string.Format(subject);
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;


                myMail.Body = msg;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);

            }
            catch
            {
                //Response.Write("- Could not send the e-mail - error: -");
                //ER.GoToErrorPage(" - Could not send the e-mail - error: -  <br/> " + ex.Message);
            }
        }

        protected void LBDSPDF_Click(object sender, EventArgs e)
        {
            if (PDF.hasSertificatedFile(requestId))
            {
                Response.Redirect(PDF.GetFromSPFolder(requestId));
            }
        }

        protected void LBPDF_Click(object sender, EventArgs e)
        {

            String htmlText = DrawPageVR(requestId);

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                Response.Clear();

                Response.ContentType = "application/pdf";

                Response.WriteFile(PDF_DOC.Create(htmlText, "VR", requestId));

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
                    url = PDF.SaveInSP(path, autor.Login, requestId);

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


            //string dateFrom = " " + DateTime.ParseExact(table.Rows[0]["Start_date"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
            //            .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            //string dateTo = " " + DateTime.Parse(table.Rows[0]["End_date"].ToString()).ToShortDateString();
            //date

            //

            string substitut = obj.GetRequest_Substitute_User_Names(Request_ID, Autor_id);

            if (substitut.Length > 0)
                substitut = "<p><b>3. Ո՞վ է փոխարինելու / Who will replace</b>  </p>" + substitut;

            //


            // approve Reject

            List<EDF_User> muser = EDF.AssociationUsers(Request_ID);

            string apprej = "<ul>";
            foreach (EDF_User us in muser)
                apprej += string.Format("<li>{0} from {1}<br/><br/></li>", us.FullName, us.Department);
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
                pay_terms = "<p>Խնդրում եմ փոխանցել արձակուրդայինս աշխատավարձի հետ միասին <br/> I would like to receive the payment for vacation together with my salary</p>";
            }


            try
            {

                User = AD.GetUserByLogin(Autor_id);
                string style = string.Empty;

                htmlText = string.Format(
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <!DOCTYPE html 
                     PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""
                    ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
                <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">
                <head>
<style>
{8}
</style>
                    <title>
                    </title>
                </head>
                    <body>
<font face = ""arial unicode ms"">

<div class=""content"">
                                <div class=""header"">
<span Style=""color:#F60; font-size: 16pt; font-weight:700; text-align:center; white-space:normal;"">            
                                    <p>Արձակուրդի դիմում / Application for vacation</p>
                                                                      
                                    <p>Հրաման / Order N Ա{0}</p>
</span>
                                </div>                         
<br/>

<br/><span Style='text-decoration: underline;'>{10} , {11}</span>
<br/>(ազգանունը, անունը, հայրանունը, պաշտոնը)
<br/>Name of employee and title
<br/>                          

<br/>Լրացնելու ամսաթիվ: <span Style='text-decoration: underline;'>{9}</span>
<br/>filling date  
<br/><br/>

                                    <p><b>1. Խնդրում եմ հատկացնել ինձ արձակուրդ / I would like to ask vacation / vacation for </b></p>
                                    
                                    {1}
                                                                                                          
                                    <br/><br/>

                                    <p><b>2. Արձակուրդի ժամկետը / Vacation duration</b></p>
                                   
                                    <t/>Սկսած / From {2}
                                                                                                            
                                    <br/> 

                                    <t/>Մինչև / To /  {3}  (ներառյալ / including)
                                                                                                            
                                    <br/> 
                                         
                                    {4}
<br/> 
                                    
                                    {5}
                                            <br/>                                   
                                    {6}
                                                                                                          
                                    <br/>

Աշխատանքային օրեր / Working days <b>{12}</b>
<br/>  
<br/> 
                                    <p><b>Հաստատված է/approved by</b></p>
                                    <br/>

                                    {7}

</div>
</font>
                    </body>
                </html>
                       ",
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
                        obj.getWorkDaysCount(Request_ID));


            }
            catch (Exception ex)
            {
                htmlText = "ERROR" + ex.Message;
            }
            return htmlText;
        }
    }
}
