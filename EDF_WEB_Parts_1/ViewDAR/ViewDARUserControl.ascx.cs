using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;


namespace EDF_WEB_Parts_1.ViewDAR
{
    public partial class ViewDARUserControl : UserControl
    {
        DataTable table;

        string Autor_id;
        string Request_ID;
        bool isManager, isDirector, isITDirector, isITSE, isBILLING, isREPORTING,
             isITSETeam, isBillingTeam, isReportingTeam, isDBSETeam,
             isCEO, IsSecurity, isAgiliti;
        bool isOfficeIt, isOfficeItTeam;

        string curLog;
        EDF_SPUser Cur = ADSP.CurrentUser;
        EDF_SPUser autor;
        string curStatus = null;

        //class UC
        //{
        //    public static void SearchBox(WebControl tb, string DefaulText)
        //    {
        //        tb.Attributes.Add("value", DefaulText);
        //        tb.Attributes.Add("onFocus", @"if(this.value == '" + DefaulText + "') {this.value = '';}");
        //        tb.Attributes.Add("onBlur", @"if (this.value == '') {this.value = '" + DefaulText + "';}");
        //    }

        //    public static void SearchBox(HtmlTextArea tb, string DefaulText)
        //    {
        //        tb.Attributes.Add("value", DefaulText);
        //        tb.Attributes.Add("onFocus", @"if(this.value == '" + DefaulText + "') {this.value = '';}");
        //        tb.Attributes.Add("onBlur", @"if (this.value == '') {this.value = '" + DefaulText + "';}");
        //    }

        //    public static bool RequiredVal(HtmlTextArea TextArea)
        //    {
        //        return TextArea.InnerText != "";
        //    }

        //    public static bool RequiredVal(HtmlTextArea TextArea, ref HtmlGenericControl Label, string ErrorMessage)
        //    {
        //        if (!TextArea.Visible)
        //            return true;
        //        if (TextArea.InnerText == string.Empty || TextArea.InnerText.Contains("Write") || TextArea.InnerText.Contains("Select"))
        //        {
        //            Label.InnerText = ErrorMessage;
        //            return false;
        //        };
        //        Label.InnerText = string.Empty;
        //        return true;
        //    }

        //    public static bool RequiredVal(HtmlTextArea TextArea, ref Label Label, string ErrorMessage)
        //    {
        //        if (!TextArea.Visible)
        //            return true;
        //        if (TextArea.InnerText == string.Empty || TextArea.InnerText.Contains("Write") || TextArea.InnerText.Contains("Select"))
        //        {
        //            Label.Text = ErrorMessage;
        //            return false;
        //        };
        //        Label.Text = string.Empty;
        //        return true;
        //    }

        //    public static bool RequiredVal(TextBox TextBox, ref HtmlGenericControl Label, string ErrorMessage)
        //    {
        //        if (!TextBox.Visible)
        //            return true;
        //        if (TextBox.Text == string.Empty || TextBox.Text.Contains("Write") || TextBox.Text.Contains("Select"))
        //        {
        //            Label.InnerText = ErrorMessage;
        //            return false;
        //        };
        //        Label.InnerText = string.Empty;
        //        return true;
        //    }

        //    public static bool RequiredVal(TextBox TextBox, ref Label Label, string ErrorMessage)
        //    {
        //        if (!TextBox.Visible)
        //            return true;
        //        if (TextBox.Text == string.Empty || TextBox.Text.Contains("Write") || TextBox.Text.Contains("Select"))
        //        {
        //            Label.Text = ErrorMessage;
        //            return false;
        //        };
        //        Label.Text = string.Empty;
        //        return true;
        //    }

        //    public bool TryHide(ref HtmlGenericControl label)
        //    {
        //        return !string.IsNullOrEmpty(label.InnerText);
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["rid"] == null)
                ER.GoToErrorPage("Request NOT FOUND");

            Request_ID = Request.QueryString["rid"];

            curLog = Cur.Login;


            SearchBox(ComText, "Write your comment:");
            try
            {

                if (!Approve_reject.canView(Cur, Request_ID))
                {
                    Response.Redirect(SPContext.Current.Site.Url);
                }

                if (!PDF.hasSertificatedFile(Request_ID))
                {
                    cert_icon.Visible = false;
                }

                table = EDF.GetDAR(Request_ID);


                Autor_id = table.Rows[0]["Autor_Id"].ToString();
                autor = AD.GetUserBySPLogin(Autor_id);
                autorImg.Src = autor.PictureUrl;

                Draw();
            }
            catch
            {
            }

            AppRejDiv.Visible = false;

            if (Request.QueryString["nid"] != null)
            {
                string NId = Request.QueryString["nid"];
                Notificaion.Update(NId, true);
            }

            string orderN = LTR.GetNumber(Request_ID);

            if (orderN == "0")
                OrderSpan.Visible = PrintDiv.Visible = false;
            else
            {
                OrderSpan.Visible = PrintDiv.Visible = true;
                OrderLabel.Text = orderN;
            }

            Autor_id = request.GetAutor_id(Request_ID);

            autor = AD.GetUserBySPLogin(Autor_id);

            autorImg.Src = autor.PictureUrl;

            DataTable tableCom = Comments.Get(Request_ID);
            DrawComents(tableCom);

            DataTable tableDARCom = Comments.Get2(Request_ID);
            DrawDARComents(tableDARCom);

            UserProf.HRef = @"http://intranet/SitePages/Person.aspx?accountname=FTA\" + Autor_id;


            #region /////     You already approved / It's your request     /////
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
                bool tmp_bool = false;
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
                            tmp_bool = true;
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
                    if (tmp_bool)
                        break;
                }
            }
            ////rep

            bool rp = false;
            foreach (EDF_SPUser us in Cur.ParentReplacement)
            {
                rp = Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login);
            }

            if (request.IsYour(Request_ID, curLog))
            {
                if (!(Approve_reject.CanAppRejLTR_Null(Request_ID, curLog) || rp))
                {
                    Label1.Text = "<b>It's your request</b>";
                    Label1.Visible = true;

                    AppRejDiv.Visible = false;
                }
            }

            #endregion


            #region //////     get status     //////

            curStatus = Approve_reject.GetStatus(curLog, Request_ID);
            switch (curStatus)
            {
                case "CEO":
                    isCEO = true;
                    break;
                case "manager":
                    isManager = true;
                    break;
                case "director":
                    isDirector = true;
                    break;
                case "InformationSecurity":
                    IsSecurity = true;
                    break;
                case "ITDirector":
                    isITDirector = true;
                    ExecDiv.Visible = Comments.CanAddComment(Request_ID, "ITDirector");
                    break;
                case "ITSE":
                    isITSE = true;
                    ITSEDiv.Visible = Comments.CanAddComment(Request_ID, "ITSE");
                    DivAgiliti.Visible = Approve_reject.App_rej(Request_ID, AD.Agility.AllUsers[0].Login) == -1;
                    break;
                case "OfficeIT":
                    isOfficeIt = true;
                    OfficeITDiv.Visible = Comments.CanAddComment(Request_ID, "OfficeIT");
                    break;
                case "BILLING":
                    isBILLING = true;
                    BILLDiv.Visible = Comments.CanAddComment(Request_ID, "BILLING");
                    break;
                case "REPORTING":
                    isREPORTING = true;
                    REPDiv.Visible = Comments.CanAddComment(Request_ID, "REPORTING");
                    break;
                case "Agiliti":
                    isAgiliti = true;
                    break;
                case "ITSE Team":
                    isITSETeam = true;
                    AppRejDiv.Visible = false;
                    ProvideDiv.Visible = true;
                    break;
                case "OfficeITTeam":
                    isOfficeItTeam = true;
                    AppRejDiv.Visible = false;
                    ProvideDiv.Visible = true;
                    break;
                case "Billing Team":
                    isBillingTeam = true;
                    AppRejDiv.Visible = false;
                    ProvideDiv.Visible = true;
                    break;
                case "Reporting Team":
                    isReportingTeam = true;
                    AppRejDiv.Visible = false;
                    ProvideDiv.Visible = true;
                    break;
                case "DBSE Team":
                    isDBSETeam = true;
                    AppRejDiv.Visible = false;
                    ProvideDiv.Visible = true;
                    break;
            }

            foreach (DataRow r in TR.getParent(Request_ID, curLog).Rows)
            {
                switch (r["parent"].ToString())
                {
                    case "ITSE":
                        Ch_ITSE.Visible = true;
                        break;
                    case "OfficeIT":
                        Ch_OfficeIT.Visible = true;
                        break;
                    case "BILLING":
                        Ch_BILLING.Visible = true;
                        break;
                    case "REPORTING":
                        Ch_REPORTING.Visible = true;
                        break;
                }
            }


            #endregion


            #region /////     get status for Replacement     /////
            if (!Approve_reject.CanAppRejLTR_Null(Request_ID, curLog))
            {
                foreach (EDF_SPUser us in Cur.ParentReplacement)
                {
                    if (Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login))
                    {
                        string cus = us.Login;


                        curStatus = Approve_reject.GetStatus(cus, Request_ID);
                        switch (curStatus)
                        {
                            case "manager":
                                isManager = true;
                                break;
                            case "director":
                                isDirector = true;
                                break;
                            case "InformationSecurity":
                                IsSecurity = true;
                                break;
                            case "ITDirector":
                                isITDirector = true;
                                ExecDiv.Visible = Comments.CanAddComment(Request_ID, "ITDirector");
                                break;
                            case "ITSE":
                                isITSE = true;
                                ITSEDiv.Visible = Comments.CanAddComment(Request_ID, "ITSE");
                                DivAgiliti.Visible = Approve_reject.App_rej(Request_ID, AD.Agility.AllUsers[0].Login) == -1;
                                break;
                            case "OfficeIT":
                                isOfficeIt = true;
                                OfficeITDiv.Visible = Comments.CanAddComment(Request_ID, "OfficeIT");
                                break;
                            case "BILLING":
                                isBILLING = true;
                                BILLDiv.Visible = Comments.CanAddComment(Request_ID, "BILLING");
                                break;
                            case "REPORTING":
                                isREPORTING = true;
                                REPDiv.Visible = Comments.CanAddComment(Request_ID, "REPORTING");
                                break;
                            case "Agiliti":
                                isAgiliti = true;
                                break;
                            case "ITSE Team":
                                isITSETeam = true;
                                AppRejDiv.Visible = false;
                                ProvideDiv.Visible = true;
                                break;
                            case "OfficeITTeam":
                                isOfficeItTeam = true;
                                AppRejDiv.Visible = false;
                                ProvideDiv.Visible = true;
                                break;
                            case "Billing Team":
                                isBillingTeam = true;
                                AppRejDiv.Visible = false;
                                ProvideDiv.Visible = true;
                                break;
                            case "Reporting Team":
                                isReportingTeam = true;
                                AppRejDiv.Visible = false;
                                ProvideDiv.Visible = true;
                                break;
                            case "DBSE Team":
                                isDBSETeam = true;
                                AppRejDiv.Visible = false;
                                ProvideDiv.Visible = true;
                                break;
                        }
                    }
                }
            }

            #endregion

            if (!IsPostBack)
            {
                CheckSelectedItems();
            }
            #region //////     History     //////
            string ss = "0";
            try
            {
                ss = "1";
                ul_history.InnerHtml = string.Empty;
                string rid = Request.QueryString["rid"].ToString();
                ss = "2";
                bool IsPendTitle = true;
                bool IsPastTitle = true;
                bool IsImplTitle = true;
                List<string> Senders = new List<string>();

                string AppRej = string.Empty;
                string Pend = string.Empty;
                string Impl = string.Empty;

                DataTable rdt = EDF.GetRequestById(int.Parse(rid));
                ss = "3";
                foreach (EDF_User u in EDF.AssociationUsersDAR(rid))
                {
                    ss = "4";
                    if (!u.Icon.Contains("timer.png") && IsPastTitle)
                    {
                        AppRej += "<li class='right_top'><p style='font-size: 20px;'>Approved/Rejected by</p></li>";
                        IsPastTitle = false;
                    }

                    if (u.Icon.Contains("timer.png") && IsPendTitle)
                    {
                        Pend += "<li class='right_top'><p style='font-size: 20px;'>Pending for approval</p></li>";
                        IsPendTitle = false;
                    }

                    string li_prop = string.Empty;
                    string avat_prop = string.Empty;
                    string text_prop = string.Empty;
                    string timer_prop = "";
                    ss = "5";
                    #region Team Users
                    if (u.IsMemberOf(AD.ITSETeam)
                        || u.IsMemberOf(AD.ITBillingTeam)
                        || u.IsMemberOf(AD.OfficeITTeam)
                        || u.IsMemberOf(AD.ITDBSETeam)
                        || u.IsMemberOf(AD.ITReportingTeam))
                    {
                        if (IsImplTitle)
                        {
                            Impl += "<li class='right_top'><p style='font-size: 20px;'>Implementation</p></li>";
                            IsImplTitle = false;
                        }
                        li_prop = "height: 30px;padding-left:50px;height:";
                        avat_prop = "width: 30px; height: 30px;";
                        text_prop = "font-size: 15px;";
                        timer_prop = "width: 6%; margin-top: 18px;margin-left:412px;";


                        if (!(u.TeamAprove(rid)))
                        {
                            if (!Senders.Contains(u.Sender + u.Team) && (u.Sender != "" || u.Sender != null))
                            {
                                Impl += string.Format("<li style='min-height:25px;height:25px;' class='right_top'><p style='font-size: 18px;margin-left:15px;'><b>{0}</b></p><img style='width:6%;{2}' class='history_icon' src='{1}'/></li>",
                                        u.Team + " by " + u.Sender,
                                        u.TeamIcon(rid),
                                        u.Icon.Contains("timer.png") ? "margin-top:23px;" : "margin-top:30px;");

                                Senders.Add(u.Sender + u.Team);
                            }
                        }
                        else
                        {

                            if (!u.Icon.Contains("timer.png"))
                            {
                                Impl += string.Format("<li style='min-height:25px;height:25px;' class='right_top'><p style='font-size: 18px;margin-left:15px;'><b>{0}</b></p><img style='width:6%;{2}' class='history_icon' src='{1}'/></li>",
                                        u.Team + " by " + u.Sender,
                                        u.TeamIcon(rid),
                                        u.Icon.Contains("timer.png") ? "margin-top:23px;" : "margin-top:30px;");

                                Senders.Add(u.Sender + u.Team);

                                Impl += string.Format(
            "<li style='cursor:default;{6}'><div class='fleft'><img style='{7}' class='avatar' src='{0}' /></div><p style='{8}' class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}<span>{4}</span></h9></p><img style='{9}' class='history_icon' src='{5}'/></li>",
                                    u.PictureUrl,
                                    u.R + u.FullName,
                                    u.Department,
                                    u.Date,
                                    u.Time,
                                    u.Icon,
                                    li_prop,
                                    avat_prop,
                                    text_prop,
                                    timer_prop);
                            }
                        }

                        continue;

                    }

                    #endregion

                    ss = "6";
                    ss = "6" + u.FullName;
                    if (!u.Icon.Contains("timer.png"))
                    {
                        AppRej += string.Format(
    "<li style='cursor:default;{6}'><div class='fleft'><img style='{7}' class='avatar' src='{0}' /></div><p style='{8}' class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}<span>{4}</span></h9></p><img style='{9}' class='history_icon' src='{5}'/></li>",
                            u.PictureUrl,
                            u.R + u.FullName,
                            u.Department,
                            u.Date,
                            u.Time,
                            u.Icon,
                            li_prop,
                            avat_prop,
                            text_prop,
                            timer_prop);
                    }
                    else
                    {
                        Pend += string.Format(
    "<li style='cursor:default{6}'><div class='fleft'><img style='{7}' class='avatar' src='{0}' /></div><p style='{8}' class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}<span>{4}</span></h9></p><img style='{9}' class='history_icon' src='{5}'/></li>",
                            u.PictureUrl,
                            u.R + u.FullName,
                            u.Department,
                            u.Date,
                            u.Time,
                            u.Icon,
                            li_prop,
                            avat_prop,
                            text_prop,
                            timer_prop);
                    }
                    ss = "7";
                    if (u.Icon.Contains("x.png") && !u.IsSubtitute)
                        break;
                }

                ul_history.InnerHtml = AppRej + Pend + Impl;

            }
            catch (Exception ex)
            {
                //ER.GoToErrorPage(string.Format("History error | {0} | EX:{1} ", ss, ex.Message));
                Response.Write(string.Format("ERROR - History {0} EX:{1} - ERROR", ss, ex.Message));
            }
            #endregion

        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            #region return
            bool hasParent = false;
            if (Cur.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in Cur.ParentReplacement)
                    if (DAR.CanAppRej(Request_ID, p.Login))
                        hasParent = true;
            }

            if (DAR.CanAppRej(Request_ID, Cur.Login) || hasParent)
            { }
            else
                return;
            #endregion

            Button bttn = sender as Button;
            bool b = bttn.Text == "Approve Request";
            Int16 end = 0;

            #region check
            if (b)
            {
                if (ExecDiv.Visible)
                {
                    if (chb_Billing_division.Checked || chb_Reporting.Checked || chb_IT_Systems_Engineering.Checked || chb_OfficeIT.Checked)
                        Valchb.Visible = false;
                    else
                    {
                        Valchb.Visible = true;
                        return;
                    }
                }
                if (ITSEDiv.Visible)
                {
                    if ((ChITSE.Checked && ChITSE.Enabled == true)
                        || (ChBD.Checked && ChBD.Enabled == true)
                        || (ChREP.Checked && ChREP.Enabled == true)
                        || (ChOfficeITDivision.Checked && ChOfficeITDivision.Enabled == true))
                        Valchb.Visible = false;
                    else
                    {
                        Valchb.Visible = true;
                        return;
                    }
                }
                if (BILLDiv.Visible)
                {
                    if (ChBilling2.Checked || ChDBSE2.Checked)
                        Valchb.Visible = false;
                    else
                    {
                        Valchb.Visible = true;
                        return;
                    }
                }

                if (REPDiv.Visible)
                {
                    if (ChReporting3.Checked || ChDBSE3.Checked)
                        Valchb.Visible = false;
                    else
                    {
                        Valchb.Visible = true;
                        return;
                    }
                }
                if (OfficeITDiv.Visible)
                {
                    if ((ChbOfficeITTeam.Checked && ChbOfficeITTeam.Enabled == true)
                        || (ChbBILLING.Checked && ChbBILLING.Enabled == true)
                        || (ChbREPORTING.Checked && ChbREPORTING.Enabled == true)
                        || (ChbITSE.Checked && ChbITSE.Enabled == true))
                    {
                        Valchb.Visible = false;
                    }
                    else
                    {
                        Valchb.Visible = true;
                        return;
                    }
                }
            }
            #endregion check


            #region CEO
            if (isCEO)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    string DARS = AD.DBRInformationSecurity.Login;

                    Approve_reject.Add(DARS, Request_ID, "ViewDAR.aspx", "InformationSecurity");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            #endregion CEO
            #region Manager
            else if (isManager)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    string dir = autor.Director.Login;

                    Approve_reject.Add(dir, Request_ID, "ViewDAR.aspx", "director");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            #endregion Manager
            #region Director
            else if (isDirector)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    string DARS = AD.DBRInformationSecurity.Login;

                    Approve_reject.Add(DARS, Request_ID, "ViewDAR.aspx", "InformationSecurity");

                    ////// send Email to corporate security user //////////////////
                    if (!AD.Domain.Contains("pele"))
                    {
                        string url = SPContext.Current.Web.Url + "/SitePages/ViewDAR.aspx?rid=" + Request_ID;
                        string typeN = Request_type.GetTypeName(Request_ID);
                        string msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to {4}</b> for approval", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID, AD.DBRInformationSecurity.FullName);

                        Notificaion.sendMail("EDF - Notification", AD.RSRCorporateSecurity.AllUsers[0].E_Mail, msg + "<p>URL: <a href='" + url + "'>" + url + "</a></p>");
                    }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            #endregion Director
            #region Security
            else if (IsSecurity)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    string ITDir = AD.ITDirector.AllUsers[0].Login;

                    Approve_reject.Add(ITDir, Request_ID, "ViewDAR.aspx", "ITDirector");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            #endregion Security
            #region ITDirector
            else if (isITDirector)
            {
                if (b)
                {
                    string r = "";
                    try
                    {
                        if (chb_Billing_division.Checked)
                        {
                            r = "EDF_Billing division group not found!";
                            Approve_reject.Add(AD.BillingDivision.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "BILLING");
                            Comments.Add(Request_ID, ta_billing.InnerText, "ITDirector", "BILLING", curLog);
                        }
                        if (chb_Reporting.Checked)
                        {
                            r = "EDF_Reporting group not found!";
                            Approve_reject.Add(AD.Reporting.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "REPORTING");
                            Comments.Add(Request_ID, ta_reporting.InnerText, "ITDirector", "REPORTING", curLog);
                        }
                        if (chb_IT_Systems_Engineering.Checked)
                        {
                            r = "EDF_IT_Systems_Engineering group not found!";
                            Approve_reject.Add(AD.ITSystemsEngineering.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "ITSE");
                            Comments.Add(Request_ID, ta_syseng.InnerText, "ITDirector", "ITSE", curLog);
                        }
                        if (chb_OfficeIT.Checked)
                        {
                            r = "EDF_OfficeIT group not found!";
                            Approve_reject.Add(AD.OfficeIT.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "OfficeIT");
                            Comments.Add(Request_ID, ta_OfficeIT.InnerText, "ITDirector", "Office/Network IT", curLog);
                        }
                    }
                    catch { ER.GoToErrorPage(r); }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }

                Approve_reject.UpdateParent(curLog, Request_ID, b);
            }
            #endregion ITDirector
            #region ITSE
            else if (isITSE)     //ITSE     dar12
            {
                string r = string.Empty;
                try
                {
                    if (ChITSE.Checked && ChITSE.Enabled == true)
                    {
                        r = "EDF_dbr_itse_team group not found!";
                        foreach (EDF_SPUser us in AD.ITSETeam.AllUsers)
                        {
                            Approve_reject.AddT(us.Login, Request_ID, "ViewDAR.aspx", "ITSE Team", "ITSE");                 ////  ITSE Team
                        }
                        Comments.Add(Request_ID, Textarea1.InnerText, "ITSE", "ITSE Team", curLog);
                    }
                    if (ChOfficeITDivision.Checked && ChOfficeITDivision.Enabled == true)                                                                         ////  Office IT
                    {
                        r = "EDF_dbr_OfficeIT group not found!";

                        Approve_reject.Add(AD.OfficeIT.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "OfficeIT");
                        Comments.Add(Request_ID, Textarea5.InnerText, "ITSE", "Office/Network IT", curLog);
                    }
                    if (ChBD.Checked && ChBD.Enabled == true)                                                                                    ////  BILLING
                    {
                        r = "EDF_Billing division group not found!";
                        Approve_reject.Add(AD.BillingDivision.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "BILLING");
                        Comments.Add(Request_ID, Textarea3.InnerText, "ITSE", "BILLING", curLog);
                    }
                    if (ChREP.Checked && ChREP.Enabled == true)                                                                                   ////  REPORTING
                    {
                        r = "EDF_Reporting group not found!";
                        Approve_reject.Add(AD.Reporting.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "REPORTING");
                        Comments.Add(Request_ID, ta_reporting.InnerText, "ITSE", "REPORTING", curLog);
                    }
                }
                catch { ER.GoToErrorPage(r); }

                Approve_reject.UpdateParent(curLog, Request_ID, b);
            }
            #endregion ITSE
            #region Agiliti
            else if (isAgiliti)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                string TypeNamer = Request_type.GetTypeName(Request_ID);
                string s = AD.ITSystemsEngineering.AllUsers[0].FullName;
                string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved by (Agility)</b>", s, TypeNamer, Request_type.GetUppers(TypeNamer), Request_ID);
                string mailMsg = string.Format("Please be informed that <b>{0}’s</b> {1} (ID: {2} {3}) <b>approved by (Agility)</b>", autor.FullName, TypeNamer, Request_type.GetUppers(TypeNamer), Request_ID);
                Notificaion.Add(AD.ITSystemsEngineering.AllUsers[0].Login, SPContext.Current.Web.Url + "/SitePages/ViewDAR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
            }
            #endregion Agiliti
            #region OfficeIt
            else if (isOfficeIt)
            {
                string error = string.Empty;
                try
                {
                    if (ChbOfficeITTeam.Checked && ChbOfficeITTeam.Enabled == true)                                                                        //// Office IT Team
                    {
                        error += "OfficeITTeam";
                        foreach (EDF_SPUser us in AD.OfficeITTeam.AllUsers)
                        {
                            Approve_reject.AddT(us.Login, Request_ID, "ViewDAR.aspx", "OfficeITTeam", "OfficeIT");
                        }
                        Comments.Add(Request_ID, OfficeITTeamTextArea.InnerText, "Office/Network IT", "Office/Network IT Team", curLog);
                    }
                    if (ChbITSE.Checked && ChbITSE.Enabled == true)
                    {
                        error += "ITSE";
                        Approve_reject.Add(AD.ITSystemsEngineering.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "ITSE");
                        Comments.Add(Request_ID, TextareaITSE.InnerText, "Office/Network IT", "ITSE", curLog);
                    }
                    if (ChbBILLING.Checked && ChbBILLING.Enabled == true)
                    {
                        error += "BILLING";
                        Approve_reject.Add(AD.BillingDivision.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "BILLING");
                        Comments.Add(Request_ID, TextareaBILLING.InnerText, "Office/Network IT", "BILLING", curLog);
                    }
                    if (ChbREPORTING.Checked && ChbREPORTING.Enabled == true)
                    {
                        error += "REPORTING";
                        Approve_reject.Add(AD.Reporting.AllUsers[0].Login, Request_ID, "ViewDAR.aspx", "REPORTING");
                        Comments.Add(Request_ID, TextareaREPORTING.InnerText, "Office/Network IT", "REPORTING", curLog);
                    }

                }
                catch (Exception ex)
                {
                    ER.GoToErrorPage(error + "---" + ex.Message + "---" + ex.InnerException);
                }
                Approve_reject.UpdateParent(curLog, Request_ID, b);
            }
            #endregion OfficeIt
            #region BILLING
            else if (isBILLING)    //B     dar78
            {
                string r = string.Empty;
                try
                {
                    if (ChBilling2.Checked)
                    {
                        r = "EDF_dbr_itbilling_team group not found!";
                        foreach (EDF_SPUser us in AD.ITBillingTeam.AllUsers)
                        {
                            Approve_reject.AddT(us.Login, Request_ID, "ViewDAR.aspx", "Billing Team", "BILLING");              ////  BT
                        }
                        Comments.Add(Request_ID, ta_billing1.InnerText, "BILLING", "Billing Team", curLog);
                    }
                    if (ChDBSE2.Checked)
                    {
                        r = "EDF_dbr_itdbse_team group not found!";
                        foreach (EDF_SPUser us in AD.ITDBSETeam.AllUsers)
                        {
                            Approve_reject.AddT(us.Login, Request_ID, "ViewDAR.aspx", "DBSE Team", "BILLING");              ////  DBSE Team
                        }
                        Comments.Add(Request_ID, ta_dbse.InnerText, "BILLING", "DBSE Team", curLog);
                    }
                }
                catch { ER.GoToErrorPage(r); }

                Approve_reject.UpdateParent(curLog, Request_ID, b);
            }
            #endregion BILLING
            #region REPORTING
            else if (isREPORTING)    //R dar56
            {
                string r = string.Empty;
                try
                {

                    if (ChReporting3.Checked)
                    {
                        r = "EDF_dbr_itreporting_team group not found!";
                        foreach (EDF_SPUser us in AD.ITReportingTeam.AllUsers)
                        {
                            Approve_reject.AddT(us.Login, Request_ID, "ViewDAR.aspx", "Reporting Team", "REPORTING");              ////  RT
                        }
                        Comments.Add(Request_ID, Textarea8.InnerText, "REPORTING", "Reporting Team", curLog);
                    }
                    if (ChDBSE3.Checked)
                    {
                        r = "EDF_dbr_itdbse_team group not found!";
                        foreach (EDF_SPUser us in AD.ITDBSETeam.AllUsers)
                        {
                            Approve_reject.AddT(us.Login, Request_ID, "ViewDAR.aspx", "DBSE Team", "REPORTING");
                        }
                        Comments.Add(Request_ID, Textarea9.InnerText, "REPORTING", "DBSE Team", curLog);
                    }
                }
                catch { ER.GoToErrorPage(r); }

                Approve_reject.UpdateParent(curLog, Request_ID, b);
            }
            #endregion REPORTING

            #region footer
            string TypeName = Request_type.GetTypeName(Request_ID);
            if (end == -1)
            {
                string s = "Your";

                string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>rejected</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                Notificaion.Add(Autor_id, SPContext.Current.Web.Url + "/SitePages/ViewDAR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, Request_type.GetId(Request_ID));
            }

            if (b)
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            else
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
            #endregion footer
        }
        private void CheckSelectedItems()
        {
            if (isITSE)
            {
                if (EDF.GetAproveRejectState(Request_ID, AD.BillingDivision.AllUsers[0].Login) != -2)
                {
                    ChBD.Checked = true;
                    ChBD.Enabled = false;
                }
                if (EDF.GetAproveRejectState(Request_ID, AD.Reporting.AllUsers[0].Login) != -2)
                {
                    ChREP.Checked = true;
                    ChREP.Enabled = false;
                }
                if (EDF.GetAproveRejectState(Request_ID, AD.OfficeIT.AllUsers[0].Login) != -2)
                {
                    ChOfficeITDivision.Checked = true;
                    ChOfficeITDivision.Enabled = false;
                }
            }
            else if (isOfficeIt)
            {
                if (EDF.GetAproveRejectState(Request_ID, AD.BillingDivision.AllUsers[0].Login) != -2)
                {
                    ChbBILLING.Checked = true;
                    ChbBILLING.Enabled = false;
                }
                if (EDF.GetAproveRejectState(Request_ID, AD.Reporting.AllUsers[0].Login) != -2)
                {
                    ChbREPORTING.Checked = true;
                    ChbREPORTING.Enabled = false;
                }
                if (EDF.GetAproveRejectState(Request_ID, AD.ITSystemsEngineering.AllUsers[0].Login) != -2)
                {
                    ChbITSE.Checked = true;
                    ChbITSE.Enabled = false;
                }
            }
        }
        private void SearchBox(HtmlTextArea tb, string DefaulText)
        {
            tb.Attributes.Add("value", DefaulText);
            tb.Attributes.Add("onFocus", @"if(this.value == '" + DefaulText + "') {this.value = '';}");
            tb.Attributes.Add("onBlur", @"if (this.value == '') {this.value = '" + DefaulText + "';}");
        }
        private void Draw()
        {
            SearchBox(ta_billing, "Write your comment:");
            SearchBox(ta_reporting, "Write your comment:");
            SearchBox(ta_syseng, "Write your comment:");
            SearchBox(ta_OfficeIT, "Write your comment:");

            SearchBox(Textarea1, "Write your comment:");
            SearchBox(Textarea3, "Write your comment:");
            SearchBox(Textarea4, "Write your comment:");
            SearchBox(Textarea5, "Write your comment:");

            SearchBox(ta_billing1, "Write your comment:");
            SearchBox(ta_dbse, "Write your comment:");
            SearchBox(OfficeITTeamTextArea, "Write your comment:");
            SearchBox(TextareaBILLING, "Write your comment:");
            SearchBox(TextareaITSE, "Write your comment:");
            SearchBox(TextareaREPORTING, "Write your comment:");

            SearchBox(Textarea8, "Write your comment:");
            SearchBox(Textarea9, "Write your comment:");

            string s = string.Empty;
            try
            {
                lb_name.Text = autor.FullName;
                lb_department.Text = autor.Department;
                lb_position.Text = autor.JobTitle;

                if (!string.IsNullOrEmpty(autor.Mobile))
                {
                    phoneTr.Visible = true;
                    lb_phone.Text = autor.Mobile;
                }
                else phoneTr.Visible = false;

                bool viewgroup;
                s = "ITSE Team";
                viewgroup = Cur.IsMemberOf(AD.ITSETeam);

                s = "Security";
                viewgroup = viewgroup || Cur.Login == AD.DBRInformationSecurity.Login;

                s = "RSRitse";
                viewgroup = viewgroup || Cur.IsMemberOf(AD.RSRitse);

                viewgroup = viewgroup || Cur.IsMemberOf(AD.DBRMemberOff);

                s = "0";

                if (viewgroup)
                {
                    groups_div.Visible = true;
                    foreach (EDF_Group g in autor.Groups)
                    {
                        ul_groups.InnerHtml += string.Format("<li>{0}</li>", g.Name);
                    }
                }

                LblType.Text = Request_type.GetTypeName(Request_ID);
                LabelName.Text = autor.FullName;
                s = "1";
                LabelDep.Text = autor.Department;
                s = "3";
                autorImg.Src = ADSP.CurrentUser.PictureUrl;
                s = "4";

                lb_Requestor.InnerText = table.Rows[0]["Requestor"].ToString();

                if (lb_Requestor.InnerText == "Beneficiary")
                {
                    if (table.Rows[0]["Beneficiary"].ToString() == "Non OAM user")
                    {
                        div_not_oam_info.Visible = true;
                        div_oam_info.Visible = false;
                        lb_oam_name2.InnerText = table.Rows[0]["Name2"].ToString();
                        lb_oam_org.InnerText = table.Rows[0]["Organization"].ToString();
                        lb_oam_country.InnerText = table.Rows[0]["Country"].ToString();
                        lb_oam_assdep.InnerText = table.Rows[0]["Ass_Dep"].ToString();
                        lb_oam_team.InnerText = table.Rows[0]["Team"].ToString();
                        lb_oam_intern.InnerText = table.Rows[0]["Intern"].ToString() == "True" ? "Yes" : "No";

                        if (string.IsNullOrEmpty(table.Rows[0]["Organization"].ToString()))
                        {
                            lb_oam_org.Visible = lb_oam_org_td.Visible = false;
                        }

                        if (string.IsNullOrEmpty(table.Rows[0]["Country"].ToString()))
                        {
                            lb_oam_country.Visible = lb_oam_country_td.Visible = false;
                        }
                        if (string.IsNullOrEmpty(table.Rows[0]["Team"].ToString()))
                        {
                            lb_oam_team.Visible = lb_oam_team_td.Visible = false;
                        }
                    }
                    if (table.Rows[0]["Beneficiary"].ToString() == "OAM user")
                    {
                        div_not_oam_info.Visible = false;
                        div_oam_info.Visible = true;
                        lb_oam_name.InnerText = table.Rows[0]["Name"].ToString();
                        lb_oam_dep.InnerText = table.Rows[0]["Department"].ToString();
                        lb_oam_pos.InnerText = table.Rows[0]["Position"].ToString();
                    }
                }
                else
                {
                    div_not_oam_info.Visible = div_oam_info.Visible = false;
                }

                s = "4";

                string eq = table.Rows[0]["Equipment"].ToString();
                if (!string.IsNullOrEmpty(eq))
                {
                    lb_Equipment.InnerText = eq;
                    DIV_Equipment.Visible = true;
                }
                else
                {
                    DIV_Equipment.Visible = false;
                }

                lb_E_mail.InnerText = table.Rows[0]["Email"].ToString();
                s = "5";
                lb_Internet_Access.InnerText = table.Rows[0]["Internet_Access"].ToString();
                s = "6";

                string desc = table.Rows[0]["Description"].ToString();
                if (!string.IsNullOrEmpty(desc))
                {
                    tr_desc.Visible = true;
                    lb_Description.InnerText = desc;
                }
                else
                {
                    tr_desc.Visible = false;
                }
                if (PDF.GetDARFile(Request_ID) == string.Empty)
                    tr_file.Attributes.Add("Style", "display:none;");
                s = "7";
                if ((table.Rows[0]["Access_Period_Start"].ToString() != "") && (table.Rows[0]["Access_Period_End"].ToString() != ""))
                {
                    lb_Start.InnerText = ((DateTime)table.Rows[0]["Access_Period_Start"]).ToString("dd/MM/yyyy");
                    lb_End.InnerText = ((DateTime)table.Rows[0]["Access_Period_End"]).ToString("dd/MM/yyyy");
                }
                else
                {
                    lb_Start.InnerText = lb_End.InnerText = string.Empty;

                    tr_Start.Attributes.Add("Style", "display:none;");
                    tr_End.Attributes.Add("Style", "display:none;");
                    tr1.Visible = true;
                }
                s = "8";
                lb_Beneficiary.InnerText = table.Rows[0]["Beneficiary"].ToString();
                if (lb_Beneficiary.InnerText == string.Empty)
                    tr_Ben.Attributes.Add("Style", "display:none;");

            }
            catch (Exception ex) { Response.Write(ex.Message + "   +|  " + s); }
        }
        protected void ComSend_Click(object sender, EventArgs e)
        {
            ComSend.Enabled = false;
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

                Notificaion.Add(users1.Rows[i]["Autor_id"].ToString(), SPContext.Current.Web.Url + "/SitePages/ViewDAR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 1);
                ComSend.Enabled = true;
            }
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
            comImg.Src = Cur.PictureUrl;
        }
        private void DrawDARComents(DataTable table)
        {
            divDARCom.InnerHtml = string.Empty;
            bool first = true;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string t1 = table.Rows[i]["MSG"].ToString();

                string t3 = table.Rows[i]["AutorStatus"].ToString();
                string t4 = table.Rows[i]["type"].ToString();

                if (t3 == "ITSE" && first)
                {
                    if (Approve_reject.App_rej(Request_ID, AD.Agility.AllUsers[0].Login) == 1)
                    {
                        divDARCom.InnerHtml += string.Format(
                          "<ul class='margin'>" +
                          "<li class='cont cont1 clearfix' style='cursor: default;'><p>{0}</p></li>" +
                          "</ul>",
                          "Agility already approved"
                          );
                    }
                    else if (Approve_reject.App_rej(Request_ID, AD.Agility.AllUsers[0].Login) == 2)
                    {
                        divDARCom.InnerHtml += string.Format(
                          "<ul class='margin'>" +
                          "<li class='cont cont1 clearfix' style='cursor: default;'><p>{0}</p></li>" +
                          "</ul>",
                          "Agility already rejected"
                        );
                    }
                    first = false;
                }

                divDARCom.InnerHtml += string.Format(
                "<ul class='margin'>" +
                    "<li class='cont cont1 clearfix' style='cursor: default;' >{0} &nbsp   <input type='checkbox' checked='checked'  disabled='disabled'/>{2} </li>" +
                    "<li class='cont cont1 clearfix' style='cursor: default;'>Comment: <p>{1}</p></li>" +
                "</ul>",
                t3, t1, t4
                );
            }
            comImg.Src = Cur.PictureUrl;
        }
        protected void Download_Click(object sender, EventArgs e)
        {
            string url = PDF.GetDARFile(Request_ID);

            Response.Redirect(url);
        }
        protected void ButtonProvide_Click(object sender, EventArgs e)
        {
            List<string> P = new List<string>();

            if (Ch_ITSE.Checked)
                P.Add("ITSE");
            if (Ch_BILLING.Checked)
                P.Add("BILLING");
            if (Ch_REPORTING.Checked)
                P.Add("REPORTING");
            if (Ch_OfficeIT.Checked)
                P.Add("OfficeIT");

            string url = SPContext.Current.Web.Url + "/SitePages/ViewDAR.aspx?rid=" + Request_ID;
            int reqTypeId = Request_type.GetId(Request_ID);

            foreach (string p in P)
            {
                Approve_reject.UpdateParent(TR.getIds(Request_ID, p), curLog, Request_ID, true);
            }

            string status = Approve_reject.GetStatusColumn(curLog, Request_ID);

            if (isITSETeam)     //ITSE
            {
                string typeN = Request_type.GetTypeName(Request_ID);

                string msg = string.Format("<b>ITSE team</b> provided this accsess");

                string User_ID = DAR.GetAutorComent(Request_ID, "ITSE Team");

                if (User_ID != "")
                {
                    Notificaion.Add(User_ID, url, msg, autor.PictureUrl, reqTypeId);

                    EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);

                    if (rus.HasReplacement)
                    {
                        Notificaion.Add(rus.Replacement.Login, url, msg, autor.PictureUrl, reqTypeId);
                    }
                }
            }
            else if (isOfficeItTeam)
            {
                string typeN = Request_type.GetTypeName(Request_ID);
                string msg = string.Format("<b>Office IT team</b> provided this accsess");
                string User_ID = DAR.GetAutorComent(Request_ID, "OfficeITteam");
                if (User_ID != "")
                {
                    Notificaion.Add(User_ID, url, msg, autor.PictureUrl, reqTypeId);

                    EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);

                    if (rus.HasReplacement)
                    {
                        Notificaion.Add(rus.Replacement.Login, url, msg, autor.PictureUrl, reqTypeId);
                    }
                }

            }
            else if (isBillingTeam)     //B
            {
                string typeN = Request_type.GetTypeName(Request_ID);

                string msg = string.Format("<b>Billing division team</b> provided this accsess");

                string User_ID = DAR.GetAutorComent(Request_ID, "Billing Team");

                if (User_ID != "")
                {

                    Notificaion.Add(User_ID, url, msg, autor.PictureUrl, reqTypeId);

                    EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);

                    if (rus.HasReplacement)
                    {
                        Notificaion.Add(rus.Replacement.Login, url, msg, autor.PictureUrl, reqTypeId);
                    }
                }
            }
            else if (isReportingTeam)     //R
            {
                string typeN = Request_type.GetTypeName(Request_ID);

                string msg = string.Format("<b>Reporting team</b> provided this accsess");

                string User_ID = DAR.GetAutorComent(Request_ID, "Reporting Team");

                if (User_ID != "")
                {
                    Notificaion.Add(User_ID, url, msg, autor.PictureUrl, reqTypeId);

                    EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);

                    if (rus.HasReplacement)
                    {
                        Notificaion.Add(rus.Replacement.Login, url, msg, autor.PictureUrl, reqTypeId);
                    }
                }
            }
            else if (isDBSETeam)     //D
            {
                string typeN = Request_type.GetTypeName(Request_ID);

                string msg = string.Format("<b>DBSE team</b> provided this accsess");

                string User_ID = DAR.GetAutorComent(Request_ID, "DBSE Team");

                if (User_ID != "")
                {
                    Notificaion.Add(User_ID, url, msg, autor.PictureUrl, reqTypeId);

                    EDF_SPUser rus = AD.GetUserBySPLogin(User_ID);

                    if (rus.HasReplacement)
                    {
                        Notificaion.Add(rus.Replacement.Login, url, msg, autor.PictureUrl, reqTypeId);
                    }
                }
            }

            DataTable groups = Approve_reject.GetGroupsStatus(Request_ID);

            bool end = true;
            foreach (DataRow group in groups.Rows)
            {
                end = end && EDF.GetGroupState(Request_ID, group["Status"].ToString());
            }


            if (end)
            {
                request.Update(Request_ID, true);

                string TypeName = Request_type.GetTypeName(Request_ID);
                string s = "Your";

                string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all</b>", s, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                string mailMsg = string.Format("Please be informed that <b>{0}’s</b> {1} (ID: {2} {3}) <b>approved by all responsible persons</b>", autor.FullName, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                Notificaion.Add(Autor_id, url, msg, autor.PictureUrl, Request_type.GetId(Request_ID));

                DAR.Update(Request_ID);
            }

            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Provide_Succes.aspx");

        }
        protected void ButtAgiliti_Click(object sender, EventArgs e)
        {
            string ag = AD.Agility.AllUsers[0].Login;
            if (!Approve_reject.InAppRej(Request_ID, ag))
            {
                Approve_reject.Add(ag, Request_ID, "ViewDAR.aspx", "Agiliti");
                DivAgiliti.Visible = false;
            }
        }
        protected void PrintLink_Click(object sender, EventArgs e)
        {
            string url = string.Format("{0}/SitePages/Print.aspx?rid={1}", SPContext.Current.Web.Url, Request_ID);

            Response.Redirect(url);
        }
    }
}
