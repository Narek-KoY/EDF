using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;


namespace EDF_WEB_Parts_1.LocalTravelRequest1
{
    public partial class LocalTravelRequest1UserControl : UserControl
    {
      
        EDF_SPUser autor = ADSP.CurrentUser;

        string Request_ID;
        DateTime Start_date;
        DateTime End_date;
        string Purpose = "Training / Դասընթաց";
        bool d1, d2;
        string Autor_id;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!autor.HasManager) ER.GoToErrorPage(autor.FullName + " has not manager");
            if (!autor.HasDirector) ER.GoToErrorPage(autor.FullName + " has not director");


            UC.SearchBox(RouteText, "Write your route:");
            UC.SearchBox(OtherPurpose, "Write your purpose:");

            Autor_id = autor.Login;

            autorImg.Src = autor.PictureUrl;

            d1 = DateTime.TryParse(dateTimeControl1.Text, out Start_date);
            d2 = DateTime.TryParse(dateTimeControl2.Text, out End_date);
        }

        void ViewHistory()
        {
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
            }
            catch
            {
                //ER.GoToErrorPage(string.Format("History error | {0} | EX:{1} ", ss, ex.Message));
                Response.Write(string.Format("ERROR - History {0} EX:{1} - ERROR", ss, ""));
            }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Dashboard.aspx");
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            AddRoute_Click(AddRoute, EventArgs.Empty);
            UC.SearchBox(RouteText, "Write your route:");

            if (HiddenFieldRout.Value.Length > 1000)
            {
                lb_val_route.Text = "* This field is longer";
                return;
            }
            else
            {
                lb_val_route.Text = string.Empty;
            }

            if (RadioButton4.Checked && OtherPurpose.Value.Length > 500)
            {
                lb_val_other_pur.Text = "This field is longer";
                return;
            }
            else
            {
                lb_val_other_pur.Text = string.Empty;
            }


            if (HiddenFieldRoutCount.Value == "0")
            {
                lb_val_route.Text = "* This field is required";
                return;
            }
            else
            {
                lb_val_route.Text = string.Empty;
            }

            if (RadioButton4.Checked && !UC.RequiredVal(OtherPurpose, ref lb_val_other_pur, "* This field is required"))
                return;

            d1 = DateTime.TryParse(dateTimeControl1.Text, out Start_date);
            d2 = DateTime.TryParse(dateTimeControl2.Text, out End_date);

            if (!d1 || !d2 || Start_date.Date < DateTime.Now.Date || Start_date.Date > End_date.Date)
            {
                datevalid.Text = "<span style='color:red'>Date not valid</span>";
                return;
            }
            else
            {
                datevalid.Text = string.Empty;
            }



            if (!request.Add(2, Autor_id))
                ER.GoToErrorPage("Cannot create request");

            Request_ID = request.GetId(Autor_id).ToString();

            string manager = autor.Manager.Login;
            if (RadioButton1.Checked)
                Purpose = "Training / Դասընթաց";
            if (RadioButton2.Checked)
                Purpose = "Conference / Համաժողով";
            if (RadioButton3.Checked)
                Purpose = "Workshop / Սեմինար ";
            if (RadioButton4.Checked)
                Purpose = OtherPurpose.InnerText;

            if (autor.IsDirector)
            {
                if (Approve_reject.Add(autor.Login, Request_ID, "ViewLTR.aspx", "director"))
                    if (!LTR.Add(Request_ID, 0, "", HiddenFieldRout.Value, Start_date, End_date, Purpose, RadioButton5.Checked, RadioButton8.Checked ? (TextBoxhh.Text + ":" + TextBoxmm.Text) : "null", RadioButton10.Checked))
                        throw new Exception("Cannot add new LTR in database");
            }
            else if (autor.Manager.IsDirector)
            {
                if (Approve_reject.Add(autor.Manager.Login, Request_ID, "ViewLTR.aspx", "director"))
                    if (!LTR.Add(Request_ID, 0, "", HiddenFieldRout.Value, Start_date, End_date, Purpose, RadioButton5.Checked, RadioButton8.Checked ? (TextBoxhh.Text + ":" + TextBoxmm.Text) : "null", RadioButton10.Checked))
                        throw new Exception("Cannot add new LTR in database");
            }
            else if (Approve_reject.Add(manager, Request_ID, "ViewLTR.aspx", "manager"))
            {
                if (!LTR.Add(Request_ID, 0, "", HiddenFieldRout.Value, Start_date, End_date, Purpose, RadioButton5.Checked, RadioButton8.Checked ? (TextBoxhh.Text + ":" + TextBoxmm.Text) : "null", RadioButton10.Checked))
                    throw new Exception("Cannot add new LTR in database");
            }
            else Response.Write("ERROR - Approve_reject.Add - ERROR");
            string urll = SPContext.Current.Web.Url + "/SitePages/SuccesCreate.aspx";
            Response.Redirect(urll);

        }

        protected void AddRoute_Click(object sender, EventArgs e)
        {
            if (RouteText.Value.Contains("|$$|") || RouteText.Value.Contains("Write your route:") || int.Parse(HiddenFieldRoutCount.Value) >= 5) return;

            HiddenFieldRout.Value += RouteText.Value + "|$$|";

            HiddenFieldRoutCount.Value = (int.Parse(HiddenFieldRoutCount.Value) + 1).ToString();

            RoutUL.InnerHtml += string.Format("<li><p>{0}. {1}</p></li>", HiddenFieldRoutCount.Value, RouteText.Value);

            RouteText.Value = RouteText.InnerText = "Write your route:";
        }

        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbttn = sender as RadioButton;

            switch (rbttn.ID)
            {
                case "RadioButton1":
                    Purpose = "Training / Դասընթաց";
                    div_other.Visible = false;
                    break;
                case "RadioButton2":
                    Purpose = "Conference / Համաժողով";
                    div_other.Visible = false;
                    break;
                case "RadioButton3":
                    Purpose = "Workshop / Սեմինար";
                    div_other.Visible = false;
                    break;
                case "RadioButton4":
                    div_other.Visible = true;
                    break;
                case "RadioButton8":
                    TimeDiv.Visible = true;
                    break;
                case "RadioButton9":
                    TimeDiv.Visible = false;
                    break;
            }
        }
    }
}
