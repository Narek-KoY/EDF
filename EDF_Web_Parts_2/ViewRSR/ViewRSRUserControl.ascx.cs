using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.ViewRSR
{
    public partial class ViewRSRUserControl : UserControl
    {
        DataTable table;

        string Autor_id;
        string Request_ID;
        bool isManager1, isCEO, isManager2,isItDir,isManager5, isHR, isAdmin, isAdminOff, isAdminCar, isFinancePayroll;
        bool isInfSecurity, isCorporateSecurity;
        bool isRSRITBilling, isRSRITReporting, isRSRitdbse, isRSRITNetwork, isRSRitse, isRSRitpcsupport, isRSRitavaya, isRSRitdomainadmins, isRSRVas;

        string curLog;
        EDF_SPUser Cur = ADSP.CurrentUser;
        EDF_SPUser autor;
     
        string curStatus = null;    

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["rid"] == null)
                ER.GoToErrorPage("Request NOT FOUND");

            Request_ID = Request.QueryString["rid"];

            table = EDF.GetRSR(Request_ID);

            Autor_id = request.GetAutor_id(Request_ID);
            autor = AD.GetUserBySPLogin(Autor_id);
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


            AppRejDiv.Visible = false;

            DataTable tableCom = Comments.Get(Request_ID);
            DrawComents(tableCom);

            if (Request.QueryString["nid"] != null)
            {
                string NId = Request.QueryString["nid"];
                Notificaion.Update(NId, true);
            }
            UC.SearchBox(ComText, "Write your comment:");

            string str = string.Empty;

            try
            {
                string orderN = RSR.GetNumber(Request_ID);

                if (orderN == "0")
                    OrderSpan.Visible = PrintDiv.Visible = DivPDF.Visible = false;
                else
                {
                    OrderLabel.Text = orderN;
                    if (Cur.IsHR)
                        PrintDiv.Visible = true;
                    else
                        PrintDiv.Visible = false;

                    if (Cur.GetDocAccess(4))
                        DivPDF.Visible = true;
                    else DivPDF.Visible = false;
                }

                AppRejDiv.Visible = ButtonCancel.Visible = ButtonSubmit.Visible = ITO.CanAppRejITO(Request_ID, Cur.Login);

                UserProf.HRef = @"http://intranet/SitePages/Person.aspx?accountname=FTA\" + autor.Login;

                #region You already approved / It's your request

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
                ////rep
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
                    if (Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login))
                        rp = true;
                }

                if (request.IsYour(Request_ID, Cur.Login))
                {
                    if (!(Approve_reject.CanAppRejLTR_Null(Request_ID, curLog) || rp))
                    {
                        Label1.Text = "<b>It's your request</b>";
                        Label1.Visible = true;

                        AppRejDiv.Visible = false;
                    }
                }

                str += ":1:";
                #endregion

                // get status

                curStatus = Approve_reject.GetStatus(curLog, Request_ID);
                //if (!string.IsNullOrEmpty(curStatus))
                   // AppRejRel = true;

                switch (curStatus)
                {
                    case "manager":
                        isManager1 = true;
                        break;
                    case "director":
                        isManager2 = true;
                        break;
                    case "CEO":
                        isCEO = true;
                        break;
                    case "Administration":
                        isAdmin = true;
                        break;
                    case "RSRAdminOffice":
                        isAdminOff = true;
                        break;
                    case "RSRAdminCar":
                        isAdminCar = true;
                        break;
                    case "Step_3":
                        //isManager3 = true;
                        break;
                    case "ITDirector":
                        isItDir = true;
                        break;
                    case "InformationSecurity":
                        isInfSecurity = true;
                        break;
                    case "CorporateSecurity":
                        isCorporateSecurity = true;
                        break;
                    case "Finance":
                        isManager5 = true;
                        break;
                    case "FinancePayroll":
                        isFinancePayroll = true;
                        break;
                    case "HR":
                        isHR = true;
                        break;
                    case "RSRITBilling":
                        isRSRITBilling = true;
                        break;
                    case "RSRITReporting":
                        isRSRITReporting = true;
                        break;
                    case "RSRitdbse":
                        isRSRitdbse = true;
                        break;
                    case "RSRITNetwork":
                        isRSRITNetwork = true;
                        break;
                    case "RSRitse":
                        isRSRitse = true;
                        break;
                    case "RSRitpcsupport":
                        isRSRitpcsupport = true;
                        break;
                    case "RSRitavaya":
                        isRSRitavaya = true;
                        break;
                    case "RSRitdomainadmins":
                        isRSRitdomainadmins = true;
                        break;
                    case "RSRVas":
                        isRSRVas = true;
                        break;
                }

                if (!Approve_reject.CanAppRejLTR_Null(Request_ID, curLog))
                {
                    foreach (EDF_SPUser us in Cur.ParentReplacement)
                    {
                        if (Approve_reject.CanAppRejLTR_Null(Request_ID, us.Login))
                        {
                            string cus = us.Login;

                            curStatus = Approve_reject.GetStatus(cus, Request_ID);
                            //if (!string.IsNullOrEmpty(curStatus))
                            //    AppRejRel = true;

                            switch (curStatus)
                            {
                                case "manager":
                                    isManager1 = true;
                                    break;
                                case "director":
                                    isManager2 = true;
                                    break;
                                case "CEO":
                                    isCEO = true;
                                    break;
                                case "Administration":
                                    isAdmin = true;
                                    break;
                                case "RSRAdminOffice":
                                    isAdminOff = true;
                                    break;
                                case "RSRAdminCar":
                                    isAdminCar = true;
                                    break;
                                case "Step_3":
                                    //isManager3 = true;
                                    break;
                                case "ITDirector":
                                    isItDir = true;
                                    break;
                                case "InformationSecurity":
                                    isInfSecurity = true;
                                    break;
                                case "CorporateSecurity":
                                    isCorporateSecurity = true;
                                    break;
                                case "Finance":
                                    isManager5 = true;
                                    break;
                                case "FinancePayroll":
                                    isFinancePayroll = true;
                                    break;
                                case "RSRITBilling":
                                    isRSRITBilling = true;
                                    break;
                                case "RSRITReporting":
                                    isRSRITReporting = true;
                                    break;
                                case "RSRitdbse":
                                    isRSRitdbse = true;
                                    break;
                                case "RSRITNetwork":
                                    isRSRITNetwork = true;
                                    break;
                                case "RSRitse":
                                    isRSRitse = true;
                                    break;
                                case "RSRitpcsupport":
                                    isRSRitpcsupport = true;
                                    break;
                                case "RSRitavaya":
                                    isRSRitavaya = true;
                                    break;
                                case "RSRitdomainadmins":
                                    isRSRitdomainadmins = true;
                                    break;
                                case "RSRVas":
                                    isRSRVas = true;
                                    break;
                            }
                        }
                    }
                }
                //  Administration
                if (isAdmin && EDF.GetRSRGroupResult(Request_ID, AD.RSRAdministration).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isAdmin && EDF.GetRSRGroupResult(Request_ID, AD.RSRAdministration).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRAdminOffice
                if (isAdminOff && EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminOffice).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isAdminOff && EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminOffice).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRAdminCar
                if (isAdminCar && EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminCar).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isAdminCar && EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminCar).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                // information Security
                if (isInfSecurity && EDF.GetRSRGroupResult(Request_ID, AD.RSRInformationSecurity).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isInfSecurity && EDF.GetRSRGroupResult(Request_ID, AD.RSRInformationSecurity).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                // Corporate Security
                if (isCorporateSecurity && EDF.GetRSRGroupResult(Request_ID, AD.RSRCorporateSecurity).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isCorporateSecurity && EDF.GetRSRGroupResult(Request_ID, AD.RSRCorporateSecurity).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  Finance
                if (isManager5 && EDF.GetRSRGroupResult(Request_ID, AD.Finance).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isManager5 && EDF.GetRSRGroupResult(Request_ID, AD.Finance).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  FinancePayroll
                if (isFinancePayroll && EDF.GetRSRGroupResult(Request_ID, AD.FinancePayroll).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isFinancePayroll && EDF.GetRSRGroupResult(Request_ID, AD.FinancePayroll).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //-------------------------------------------------------------------------------------------------------------//

                //  RSRITBilling
                if (isRSRITBilling && EDF.GetRSRGroupResult(Request_ID, AD.RSRITBilling).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRITBilling && EDF.GetRSRGroupResult(Request_ID, AD.RSRITBilling).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRITReporting
                if (isRSRITReporting && EDF.GetRSRGroupResult(Request_ID, AD.RSRITReporting).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRITReporting && EDF.GetRSRGroupResult(Request_ID, AD.RSRITReporting).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRitdbse
                if (isRSRitdbse && EDF.GetRSRGroupResult(Request_ID, AD.RSRitdbse).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRitdbse && EDF.GetRSRGroupResult(Request_ID, AD.RSRitdbse).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRITNetwork
                if (isRSRITNetwork && EDF.GetRSRGroupResult(Request_ID, AD.RSRITNetwork).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRITNetwork && EDF.GetRSRGroupResult(Request_ID, AD.RSRITNetwork).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRitse
                if (isRSRitse && EDF.GetRSRGroupResult(Request_ID, AD.RSRitse).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRitse && EDF.GetRSRGroupResult(Request_ID, AD.RSRitse).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRitpcsupport
                if (isRSRitpcsupport && EDF.GetRSRGroupResult(Request_ID, AD.RSRitpcsupport).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRitpcsupport && EDF.GetRSRGroupResult(Request_ID, AD.RSRitpcsupport).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRitavaya
                if (isRSRitavaya && EDF.GetRSRGroupResult(Request_ID, AD.RSRitavaya).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRitavaya && EDF.GetRSRGroupResult(Request_ID, AD.RSRitavaya).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRitdomainadmins
                if (isRSRitdomainadmins && EDF.GetRSRGroupResult(Request_ID, AD.RSRitdomainadmins).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRitdomainadmins && EDF.GetRSRGroupResult(Request_ID, AD.RSRitdomainadmins).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                //  RSRVas
                if (isRSRVas && EDF.GetRSRGroupResult(Request_ID, AD.RSRVas).IsOk == true)
                {
                    Label1.Text = "<b>Your group already approved</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }
                else if (isRSRVas && EDF.GetRSRGroupResult(Request_ID, AD.RSRVas).IsOk == false)
                {
                    Label1.Text = "<b>Your group already rejected</b>";
                    Label1.Visible = true;
                    ButtonCancel.Visible = ButtonSubmit.Visible = false;
                }

                Draw();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - ViewReplace pageload -  <br/> " + str + "</b>" + ex.Message);
            }

            #region //////        History        //////
            string ss = "0";
            try
            {
                ss = "1";
                ul_history.InnerHtml = string.Empty;
                string rid = Request.QueryString["rid"].ToString();
                ss = "2";
                bool IsPendTitle = true;
                bool IsPastTitle = true;
                string AppRej = string.Empty;
                string Pend = string.Empty;
                ss = "3";
                foreach (EDF_User u in EDF.AssociationUsersRSR(rid))
                {
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
                    ss = "4";
                    ss = "5" + u.FullName;
                    if (!u.Icon.Contains("timer.png"))
                        AppRej += string.Format(
    "<li style='cursor:default'><div class='fleft'><img class='avatar' src='{0}' /></div><p class='text4 width fleft'>{1}{6}{2} <span></span><br /><h9>{3}<span>{4}</span></h9></p><img class='history_icon' src='{5}'/></li>",
                            u.PictureUrl,
                            u.R + u.FullName,
                            u.Department,
                            u.Date,
                            u.Time,
                            u.Icon,
                            (u.IsGroup && u.IsOk == null) ? "" : "<span> from </span>");

                    else
                        Pend += string.Format(
    "<li style='cursor:default'><div class='fleft'><img class='avatar' src='{0}' /></div><p class='text4 width fleft'>{1}{6}{2} <span></span><br /><h9>{3}<span>{4}</span></h9></p><img class='history_icon' src='{5}'/></li>",
                            u.PictureUrl,
                            u.R + u.FullName,
                            u.Department,
                            u.Date,
                            u.Time,
                            u.Icon,
                            (u.IsGroup && u.IsOk == null) ? "" : "<span> from </span>");

                    if (u.Icon.Contains("x.png") && !u.IsSubtitute)
                        break;
                }

                ul_history.InnerHtml = AppRej + Pend;
            }
            catch (Exception ex)
            {
                Response.Write(string.Format("ERROR - History {0} EX:{1} - ERROR", ss, ex.Message));
            }
            #endregion
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (EDF.GetRSRKeepAccess(Request_ID))
                KeepAccessChecked(sender);
            else
                KeepAccessNotChecked(sender);
        }

        protected void KeepAccessNotChecked(object sender)
        {
            bool hasParent = false;
            if (Cur.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in Cur.ParentReplacement)
                    if (ITO.CanAppRejITO(Request_ID, p.Login))
                        hasParent = true;
            }

            if (ITO.CanAppRejITO(Request_ID, Cur.Login) || hasParent)
            { }
            else
                return;

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
                    string manager = autor.Director.Login;

                    Approve_reject.Add(manager, Request_ID, "ViewRSR.aspx", "director");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager2 || isCEO)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    foreach (EDF_SPUser us in AD.RSRAdministration.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "Administration");
                    foreach (EDF_SPUser us in AD.RSRAdminOffice.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRAdminOffice");
                    foreach (EDF_SPUser us in AD.RSRAdminCar.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRAdminCar");

                    foreach (EDF_SPUser us in AD.RSRITBilling.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRITBilling");
                    foreach (EDF_SPUser us in AD.RSRITReporting.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRITReporting");
                    foreach (EDF_SPUser us in AD.RSRitdbse.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRitdbse");
                    foreach (EDF_SPUser us in AD.RSRITNetwork.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRITNetwork");
                    foreach (EDF_SPUser us in AD.RSRitse.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRitse");
                    foreach (EDF_SPUser us in AD.RSRitpcsupport.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRitpcsupport");
                    foreach (EDF_SPUser us in AD.RSRitavaya.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRitavaya");
                    foreach (EDF_SPUser us in AD.RSRitdomainadmins.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRitdomainadmins");
                    foreach (EDF_SPUser us in AD.RSRVas.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRVas");

                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isItDir || isRSRITBilling || isRSRITReporting || isRSRitdbse || isRSRITNetwork || isRSRitse || isRSRitpcsupport || isRSRitavaya || isRSRitdomainadmins || isRSRVas || isAdmin || isAdminOff || isAdminCar)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (isRSRitavaya || isRSRitpcsupport || isRSRitdomainadmins)
                    {
                        List<EDF_SPUser> users = new List<EDF_SPUser> { AD.RSRitavaya.AllUsers[0], AD.RSRitpcsupport.AllUsers[0], AD.RSRitdomainadmins.AllUsers[0] };

                        if (EDF.GetRSRGroupResult(Request_ID, AD.RSRitavaya).IsOk == true && EDF.GetRSRGroupResult(Request_ID, AD.RSRitpcsupport).IsOk == true && EDF.GetRSRGroupResult(Request_ID, AD.RSRitdomainadmins).IsOk == true)
                        {
                            string TypeName = Request_type.GetTypeName(Request_ID);

                            string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from all IT System Engineering Sub Approvals</b>", autor.FullName, TypeName, Request_type.GetUppers(TypeName), Request_ID);

                            Notificaion.Add(AD.RSRitse.AllUsers[0].Login, SPContext.Current.Web.Url + "/SitePages/ViewRSR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 4);
                        }
                    }

                    if (isRSRITBilling || isRSRITReporting || isRSRitdbse || isRSRITNetwork || isRSRitse || isRSRitpcsupport || isRSRitavaya || isRSRitdomainadmins || isRSRVas)
                    {
                        if (EDF.GetRSRGroupResult(Request_ID, AD.RSRITBilling).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRITReporting).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRitdbse).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRITNetwork).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRitse).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRitpcsupport).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRitavaya).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRitdomainadmins).IsOk == true &&
                            EDF.GetRSRGroupResult(Request_ID, AD.RSRVas).IsOk == true)
                        {
                            Approve_reject.Add(AD.ITDirector.AllUsers[0].Login, Request_ID, "ViewRSR.aspx", "ITDirector");
                        }
                    }

                    if (isItDir &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRITBilling).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRITReporting).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitdbse).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRITNetwork).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitse).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitpcsupport).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitavaya).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitdomainadmins).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRVas).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRAdministration).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminOffice).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminCar).IsOk == true)
                    {
                        foreach (EDF_SPUser us in AD.RSRInformationSecurity.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "InformationSecurity");
                    }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isInfSecurity)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (EDF.GetRSRGroupResult(Request_ID, AD.RSRInformationSecurity).IsOk == true)
                    {
                        foreach (EDF_SPUser us in AD.RSRCorporateSecurity.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "CorporateSecurity");
                    }

                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isCorporateSecurity)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (EDF.GetRSRGroupResult(Request_ID, AD.RSRCorporateSecurity).IsOk == true)
                    {
                        foreach (EDF_SPUser us in AD.Finance.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "Finance");
                        foreach (EDF_SPUser us in AD.FinancePayroll.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "FinancePayroll");
                    }

                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager5 || isFinancePayroll)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (isFinancePayroll)
                    {
                        if (EDF.GetRSRGroupResult(Request_ID, AD.FinancePayroll).IsOk == true)
                        {
                            string TypeName = Request_type.GetTypeName(Request_ID);

                            string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from Finance Payroll team</b>", autor.FullName, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                            foreach (EDF_SPUser us in AD.Finance.AllUsers)
                                Notificaion.Add(us.Login, SPContext.Current.Web.Url + "/SitePages/ViewRSR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 4);
                        }
                    }
                    if (EDF.GetRSRGroupResult(Request_ID, AD.Finance).IsOk == true && EDF.GetRSRGroupResult(Request_ID, AD.FinancePayroll).IsOk == true)
                    {
                        Approve_reject.Add(AD.HR.Login, Request_ID, "ViewRSR.aspx", "HR");
                    }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isHR)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                request.Update(Request_ID, b);
                if (b) end = 1;
                else end = -1;
            }
            
            if (end == 1 || end == -1)
            {
                string TypeName = Request_type.GetTypeName(Request_ID);
            }
            if (end == 1)
            {
                RSR.Update(Request_ID);
            }
            if (b)
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            else
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);
                Approve_reject.DeleteFreeRequests(Request_ID);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
        }

        protected void KeepAccessChecked(object sender)
        {
            bool hasParent = false;
            if (Cur.ParentReplacement.Count > 0)
            {
                foreach (EDF_SPUser p in Cur.ParentReplacement)
                    if (ITO.CanAppRejITO(Request_ID, p.Login))
                        hasParent = true;
            }

            if (ITO.CanAppRejITO(Request_ID, Cur.Login) || hasParent)
            { }
            else
                return;

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
                    string manager = autor.Director.Login;

                    Approve_reject.Add(manager, Request_ID, "ViewRSR.aspx", "director");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager2 || isCEO)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    Approve_reject.Add(AD.ITDirector.AllUsers[0].Login, Request_ID, "ViewRSR.aspx", "ITDirector");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isItDir)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    foreach (EDF_SPUser us in AD.RSRAdministration.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "Administration");
                    foreach (EDF_SPUser us in AD.RSRAdminOffice.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRAdminOffice");
                    foreach (EDF_SPUser us in AD.RSRAdminCar.AllUsers)
                        Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "RSRAdminCar");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isAdmin || isAdminCar || isAdminOff)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (EDF.GetRSRGroupResult(Request_ID, AD.RSRAdministration).IsOk == true &&
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminOffice).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRAdminCar).IsOk == true)
                        foreach (EDF_SPUser us in AD.RSRInformationSecurity.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "InformationSecurity");
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isInfSecurity)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (EDF.GetRSRGroupResult(Request_ID, AD.RSRInformationSecurity).IsOk == true)
                    {
                        foreach (EDF_SPUser us in AD.RSRCorporateSecurity.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "CorporateSecurity");
                    }

                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isCorporateSecurity)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (EDF.GetRSRGroupResult(Request_ID, AD.RSRCorporateSecurity).IsOk == true)
                    {
                        foreach (EDF_SPUser us in AD.Finance.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "Finance");
                        foreach (EDF_SPUser us in AD.FinancePayroll.AllUsers)
                            Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "FinancePayroll");
                    }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isManager5 || isFinancePayroll)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (isFinancePayroll)
                    {
                        if (EDF.GetRSRGroupResult(Request_ID, AD.FinancePayroll).IsOk == true)
                        {
                            string TypeName = Request_type.GetTypeName(Request_ID);

                            string msg = string.Format("<b>{0}</b> {1} (ID: {2} {3}) <b>approved from Finance Payroll team</b>", autor.FullName, TypeName, Request_type.GetUppers(TypeName), Request_ID);
                            foreach (EDF_SPUser us in AD.Finance.AllUsers)
                                Notificaion.Add(us.Login, SPContext.Current.Web.Url + "/SitePages/ViewRSR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 4);
                        }
                    }
                    if (EDF.GetRSRGroupResult(Request_ID, AD.Finance).IsOk == true && EDF.GetRSRGroupResult(Request_ID, AD.FinancePayroll).IsOk == true)
                    {
                        Approve_reject.Add(AD.HR.Login, Request_ID, "ViewRSR.aspx", "HR");
                    }
                }
                else
                {
                    request.Update(Request_ID, b);
                    end = -1;
                }
            }
            else if (isHR)
            {
                
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                request.Update(Request_ID, b);
                if (b) end = 1;
                else end = -1;
            }
            else if (isRSRITBilling || isRSRITReporting || isRSRitdbse || isRSRITNetwork || isRSRitse || isRSRitpcsupport || isRSRitavaya || isRSRitdomainadmins || isRSRVas)
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);

                if (b)
                {
                    if (EDF.GetRSRGroupResult(Request_ID, AD.RSRITBilling).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRITReporting).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitdbse).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRITNetwork).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitse).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitpcsupport).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitavaya).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRitdomainadmins).IsOk == true && 
                        EDF.GetRSRGroupResult(Request_ID, AD.RSRVas).IsOk == true)
                    {

                        string TypeName = Request_type.GetTypeName(Request_ID);

                        string msg = string.Format("Please be informed that accesses of {0} are closed", autor.FullName);

                        Notificaion.Add(AD.RSRitse.AllUsers[0].Login, SPContext.Current.Web.Url + "/SitePages/ViewRSR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 4);


                        string comand = string.Format("UPDATE [EDF].[dbo].[RSR] SET [req_state] = 'True' WHERE Request_Id = '{0}'", Request_ID);

                        SqlConnection con = new SqlConnection(Connection.connectionString);
                        try
                        {
                            con.Open();

                            SqlCommand command = new SqlCommand(comand, con);

                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            ER.GoToErrorPage(" - UpdateReqState -  </br> " + ex.Message);
                        }
                        finally
                        {
                            con.Close();
                        }
                    }

                }
            }
          
            if (end == 1)
            {
                RSR.Update(Request_ID);

                if (!EDF.addUserToDB(autor))
                {
                    ER.GoToErrorPage("Can`t add user to Database");
                }
            }
            if (b)
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            else
            {
                Approve_reject.UpdateParent(curLog, Request_ID, b);
                Approve_reject.DeleteFreeRequests(Request_ID);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
        }

        protected void PrintLink_Click(object sender, EventArgs e)
        {
            string url = string.Format("{0}/SitePages/Print.aspx?rid={1}", SPContext.Current.Web.Url, Request_ID);

            Response.Redirect(url);
        }

        private void Draw()
        {
            try
            {
                LblType.Text = Request_type.GetTypeName(Request_ID);
                LabelName.Text = autor.FullName;
                LabelDep.Text = autor.Department;
                autorImg.Src = autor.PictureUrl;
                lb_Name.InnerText = autor.FullName;
                lb_father.InnerText = table.Rows[0]["Father"].ToString();
                lb_Department_Position.InnerText = table.Rows[0]["Department_And_Position"].ToString();
                lb_windows_account.InnerText = autor.Login;
                if (table.Rows[0]["Cboss"].ToString() == string.Empty)
                    tr_cboss.Attributes.Add("style", "visibility:collapse;");
                else
                    lb_cboss.InnerText = table.Rows[0]["Cboss"].ToString();
                lb_phone.InnerText = (string.IsNullOrEmpty(autor.Mobile) ? table.Rows[0]["Phone"].ToString() : autor.Mobile);
                lb_pr_phone.InnerText = table.Rows[0]["Private_Phone"].ToString();
                lb_date.InnerText = ((DateTime)table.Rows[0]["Last_Work_Day"]).ToString("dd/MM/yyyy");

                if (isItDir)
                    ItDirTR.Visible = true;


                if ((bool)table.Rows[0]["keep_accesses"])
                {
                    ItDirTR.Visible = true;
                    AccessTill.InnerText = ((DateTime)table.Rows[0]["keep_accesses_date"]).ToString("dd/MM/yyyy");
                }
                else
                {
                    ItDirTR.Visible = false;
                }
            }
            catch (Exception ex) { Response.Write("Can not read from database: " + ex.Message); }
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
                if (Cur.Login == users1.Rows[i]["Autor_id"].ToString()) continue;

                string s = string.Empty;

                if (autor.Login == users1.Rows[i]["Autor_id"].ToString())
                { s = "your"; }
                else if (autor.Login == Cur.Login)
                { s = "his/her"; }
                else { s = autor.FullName + "’s"; }

                string msg = string.Format("<b>{0}</b> {4}<b>{1}</b> commented on <b>{6}</b> {2} (ID: {5} {3})", Cur.FullName, Cur.Department, TypeName, Request_ID, (Cur.Department == null ? "" : "from "), Request_type.GetUppers(TypeName), s);

                Notificaion.Add(users1.Rows[i]["Autor_id"].ToString(), SPContext.Current.Web.Url + "/SitePages/ViewRSR.aspx?rid=" + Request_ID, msg, autor.PictureUrl, 1);
            }
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
            string tmp = ":1:";

            try
            {

                String htmlText = DrawPageRSR(Request_ID);

                tmp += ":1:";

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    tmp += ":1:";

                    Response.Clear();
                    tmp += ":1:";
                    Response.ContentType = "application/pdf";
                    tmp += ":1:";
                    Response.WriteFile(PDF_DOC.Create(htmlText, "RSR", Request_ID));
                    tmp += ":1:";

                });
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(ex.Message + " : " + tmp);
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
                apprej += string.Format("<li>{0} from {1}<br/><br/></li>", us.FullName, us.Department);

            apprej += "<li>HR Administration department Director Marine Aznauryan ____________<br/></li>";
            apprej += "</ul>";

            string days = string.Empty;
            string pay_terms = string.Empty;

            try
            {
                string style = "";

                htmlText = string.Format(
                    @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
            <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""hy-AM"">

                <head>
                    <style> {8} </style>
                    <title> </title>
                </head>

                <body>
<font face = string.Emptyarial unicode ms"">
                    <div class=""content"">

                        <div class=""header"">
<span Style=""color:#F60; font-size: 16pt; font-weight:700; text-align:center; white-space:normal;"">
                            <p>Շրջիկ թերթիկ / Round sheet </p>
</span>                                         
                            
                        </div>
                        
                        <br/>     
                       
<br/><span Style='text-decoration: underline;'>{10} , {1} , {11}</span>
<br/>(ազգանունը, անունը, հայրանունը, պաշտոնը)
<br/>Name of employee and title
<br/>                              
<br/>Windows օգտագործողի անուն: <span Style='text-decoration: underline;'>{2}</span>
<br/>Windows account user name
<br/>
<br/>Cboss օգտատերի անուն (եթե առկա է): <span Style='text-decoration: underline;'>{3}</span>
<br/>Cboss user name (if any)  
<br/>
<br/>Բջջային: <span Style='text-decoration: underline;'>{5}</span>
<br/>Mobile number
<br/>
<br/>Անձնական բջջային: <span Style='text-decoration: underline;'>{9}</span>
<br/>Private mobile number
<br/>
<br/>Վերջին աշխատանքային օրը: <span Style='text-decoration: underline;'>{12}</span>
<br/>Last working day
<br/>
<br/>Access disable Pending : {13}
<br/>


                        {4}
<br/>                                  
                        {6}
                       

                        <p><b>Հաստատված է/approved by</b></p>
<br/>
                        {7}

                    </div>
</font>
                </body>
            </html>",
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
                                    AP ? "Yes / Այո" : "	No / Ոչ"
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
