using System;
using System.Web.UI;
using EDF_CommonData;
using Microsoft.SharePoint;


namespace EDF_Web_Parts_2.RoundSheetRequest
{
    public partial class RoundSheetRequestUserControl : UserControl
    {
        EDF_SPUser autor = ADSP.CurrentUser;
        string Autor_id;
        string Request_ID;
        DateTime date;
        bool d;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!autor.IsCEO)
            {
                if (!autor.HasManager) ER.GoToErrorPage(autor.FullName + " has not manager");
                if (!autor.HasDirector) ER.GoToErrorPage(autor.FullName + " has not director");
            }
            Autor_id = autor.Login;

            UC.SearchBox(ta_father, "Write Father’s Name:");
            UC.SearchBox(ta_phone, "Write Phone Number:");
            UC.SearchBox(ta_Private_phone, "Write Private Phone Number:");
            UC.SearchBox(ta_cboss_user, "Write Cboss user name:");

            autorImg.Src = autor.PictureUrl;
            lb_Name.InnerText = autor.FullName;
            lb_Department.InnerText = autor.Department;
            lb_Position.InnerText = "/ " + autor.JobTitle;
            lb_windows_account.InnerText = Autor_id;
            if (string.IsNullOrEmpty(autor.Mobile))
            {
                lb_phone.Visible = false;
                lb_phone.Attributes.Add("style", "width:0px;");
                ta_phone.Visible = true;
            }
            else
            {
                lb_phone.Visible = true;
                ta_phone.Visible = false;
                ta_phone.Attributes.Add("style", "width:0px;");
                lb_phone.InnerText = autor.Mobile;
            }

            autorImg.Src = autor.PictureUrl;
            d = DateTime.TryParse(tb_last_work_day.Text, out date);
        }


        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            #region UI
            bool val = (UC.RequiredVal(ta_father, ref val_father, "Write Father Name")
                        && UC.RequiredVal(ta_phone, ref val_Phone, "Write Phone Number")
                        && UC.RequiredVal(ta_Private_phone, ref val_pr_phone, "Write Private Phone Number")
                        && UC.RequiredVal(tb_last_work_day, ref val_date, "Write Last Working Day"));

            DateTime lastWorkingDay;

            if (DateTime.TryParse(tb_last_work_day.Text, out lastWorkingDay) && lastWorkingDay.Date < DateTime.Now.Date)
            {
                val_date.InnerText = "Date not valid";
                val = false;
            }
            else
            {
                val_date.InnerText = string.Empty;
            }

            if (ChStep3.Checked)
            {
                DateTime accTillDate;

                val = val && UC.RequiredVal(dateIT, ref val_dateAccTill, "Write accesses till Day");
                if (DateTime.TryParse(dateIT.Text, out accTillDate) && accTillDate > DateTime.Now && DateTime.TryParse(tb_last_work_day.Text, out lastWorkingDay) && lastWorkingDay < accTillDate)
                {
                    if (lastWorkingDay.AddDays(7).Date > accTillDate.Date)
                    {
                        val = false;
                        val_dateAccTill.InnerText = "Prolonged access could not be requested for mentioned period. Please contact IT team";
                    }
                    else
                        val_dateAccTill.InnerText = string.Empty;
                }
                else
                {
                    val = false;
                    val_dateAccTill.InnerText = "Date not valid";
                }
            }


            if (!val) return;
            #endregion
            if (!request.Add(4, Autor_id)) ER.GoToErrorPage("Cannot create request");

            Request_ID = request.GetId(Autor_id).ToString();


            if (!EDF.AddRSR(Request_ID, "0", autor.Department, Autor_id, ta_father.InnerText, ta_cboss_user.InnerText, (lb_phone.Visible ? lb_phone.InnerText : ta_phone.InnerText), ta_Private_phone.InnerText, DateTime.Parse(tb_last_work_day.Text), ChStep3.Checked, ChStep3.Checked ? DateTime.Parse(dateIT.Text) : DateTime.Now))
                throw new Exception("Cannot add new RSR in database");

            d = DateTime.TryParse(tb_last_work_day.Text, out date);

            bool error = false;

            if (autor.IsCEO)
            {
                foreach (EDF_SPUser us in AD.Step3Users)
                {
                    Approve_reject.Add(us.Login, Request_ID, "ViewRSR.aspx", "Step_3");
                }
            }
            else if (autor.IsDirector)
            {
                error = !Approve_reject.Add(AD.CEO.Login, Request_ID, "ViewRSR.aspx", "CEO");
            }
            else if (autor.Manager.IsDirector)
            {
                error = !Approve_reject.Add(autor.Manager.Login, Request_ID, "ViewRSR.aspx", "director");
            }
            else
            {
                error = !Approve_reject.Add(autor.Manager.Login, Request_ID, "ViewRSR.aspx", "manager");
            }
            if (error) Response.Write("Can't add new [Approve_reject] in Database");

            string typeN = Request_type.GetTypeName(Request_ID);

            string urll = SPContext.Current.Web.Url + "/SitePages/SuccesCreate.aspx";

            string msg = string.Format("<b>{0}’s</b> {1} (ID: {2} {3}) is <b>submitted to you</b> for approval <br/> {4}", autor.FullName, typeN, Request_type.GetUppers(typeN), Request_ID, SPContext.Current.Web.Url + "/SitePages/" + "ViewRSR.aspx" + "?rid=" + Request_ID);

            if (ChStep3.Checked)
                Notificaion.sendMail("EDF - Round Sheet Access Till ", AD.RSRitse.AllUsers[0].E_Mail, msg);

            Response.Redirect(urll);
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Dashboard.aspx");
        }
    }
}
