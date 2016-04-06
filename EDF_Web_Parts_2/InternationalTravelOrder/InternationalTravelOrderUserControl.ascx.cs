using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.InternationalTravelOrder
{
    public partial class InternationalTravelOrderUserControl : UserControl
    {
        EDF_SPUser autor = ADSP.CurrentUser;

        string requestId;
        DateTime startDate;
        DateTime endDate;
        DateTime tb_fl_date_datetime;
        string purpose = "Training / Դասընթաց";
        bool d1, d2;
        string autorId;

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                        UpdatePanel4,
                        this.GetType(),
                        "MyAction",
                        "picker();",
                        true);

            if (!autor.IsCEO)
            {
                if (!autor.HasManager) ER.GoToErrorPage(autor.FullName + " has not manager");
                if (!autor.HasDirector) ER.GoToErrorPage(autor.FullName + " has not director");
            }
            UC.SearchBox(ta_Rout, "Write your route:");
            UC.SearchBox(ta_Inviting, "Write Your Inviting Organization:");
            UC.SearchBox(tb_Purpose_other, "Write your purpose:");

            UC.SearchBox(ta_Hotel_Name, "Write Hotel Name:");
            UC.SearchBox(ta_Hotel_Dates, "Write Hotel Dates:");
            UC.SearchBox(ta_Hotel_Location, "Write Hotel Location:");
            UC.SearchBox(ta_Hotel_Phone, "Write Hotel Phone Number:");
            UC.SearchBox(ta_Hotel_Payment, "Write Hotel Payment method:");

            UC.SearchBox(ta_fl_airline, "Write airline:");
            UC.SearchBox(ta_fl_number, "Write flight number:");
            UC.SearchBox(ta_fl_dep_city, "Write departure city:");
            UC.SearchBox(ta_fl_dest_city, "Write destination city:");

            autorId = autor.Login;

            autorImg.Src = autor.PictureUrl;

            d1 = DateTime.TryParse(tb_Start_Date.Text, out startDate);
            d2 = DateTime.TryParse(tb_End_Date.Text, out endDate);
        }

        protected void AddInvit_Click(object sender, EventArgs e)
        {
            if (ta_Inviting.Text.Contains("|$$|") || ta_Inviting.Text.Contains("Write Your Inviting Organization:") || int.Parse(hf_invit_count.Value) >= 5) return;

            hf_invit.Value += ta_Inviting.Text + "|$$|";

            hf_invit_count.Value = (int.Parse(hf_invit_count.Value) + 1).ToString();

            ul_Inviting.InnerHtml += string.Format("<li><p>{0}. {1}</p></li>", hf_invit_count.Value, ta_Inviting.Text);

            ta_Inviting.Text = "Write Your Inviting Organization:";
        }

        protected void AddRoute_Click(object sender, EventArgs e)
        {
            if (ta_Rout.Text.Contains("|$$|") || ta_Rout.Text.Contains("Write your route:") || int.Parse(hf_Rout_Count.Value) >= 5) return;

            hf_Rout.Value += ta_Rout.Text + "|$$|";

            hf_Rout_Count.Value = (int.Parse(hf_Rout_Count.Value) + 1).ToString();

            ul_Rout.InnerHtml += string.Format("<li><p>{0}. {1}</p></li>", hf_Rout_Count.Value, ta_Rout.Text);

            ta_Rout.Text = "Write your route:";
        }

        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.ID)
            {
                case "rb_Hotel_Yes": HotelDetails.Visible = rb_Hotel_Yes.Checked; break;
                case "rb_Purpose_Training": div_other.Visible = rb_Purpose_Other.Checked; if (rb_Purpose_Training.Checked) purpose = "Training"; break;
                case "rb_Purpose_Conference": div_other.Visible = rb_Purpose_Other.Checked; if (rb_Purpose_Conference.Checked) purpose = "Conference"; break;
                case "rb_Purpose_Workshop": div_other.Visible = rb_Purpose_Other.Checked; if (rb_Purpose_Workshop.Checked) purpose = "Workshop"; break;
                case "rb_Purpose_Other": div_other.Visible = rb_Purpose_Other.Checked; if (rb_Purpose_Other.Checked) purpose = tb_Purpose_other.Text; break;
            }
            HotelDetails.Visible = rb_Hotel_Yes.Checked;
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Dashboard.aspx");
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {

            AddRoute_Click(AddRoute, EventArgs.Empty);
            AddInvit_Click(AddInvit, EventArgs.Empty);
            AddFlightDetails_Click(AddFlightDetails, EventArgs.Empty);

            #region UI
            if (hf_Rout_Count.Value == "0")
            {
                val1.InnerText = "* This field is required";
                return;
            }
            else
                val1.InnerText = string.Empty;

            if (!d1 || !d2 || DateTime.Now.Date > startDate.Date || startDate.Date > endDate.Date)
            {
                datevalid.Text = "<span style='color:red'>Date not valid</span>";
                return;
            }
            else
                datevalid.Text = string.Empty;

            if (rb_Purpose_Other.Checked && !UC.RequiredVal(tb_Purpose_other, ref lb_val_other_pur, "* This field is required"))
                return;


            decimal costs;
            bool b = decimal.TryParse(tb_Costs.Text, out costs);
            if (!b) { val3.InnerText = "Amount is not valid"; costs = 0; return; } else val3.InnerText = string.Empty;

            if (rb_Hotel_Yes.Checked && (ta_Hotel_Name.Text == string.Empty || ta_Hotel_Name.Text == "Write Hotel Name:" || ta_Hotel_Dates.Text == string.Empty || ta_Hotel_Dates.Text == "Write Hotel Dates:" || ta_Hotel_Location.Text == string.Empty || ta_Hotel_Location.Text == "Write Hotel Location:"/* || ta_Hotel_Phone.Text == string.Empty || ta_Hotel_Phone.Text == "Write Hotel Phone Number:" || ta_Hotel_Payment.Text == string.Empty || ta_Hotel_Payment.Text == "Write Hotel Payment method:"*/))
            { Val2.InnerText = "Please enter hotel details!"; return; }
            else Val2.InnerText = string.Empty;

            if (ta_Hotel_Phone.Text == "Write Hotel Phone Number:") ta_Hotel_Phone.Text = string.Empty;
            if (ta_Hotel_Payment.Text == "Write Hotel Payment method:") ta_Hotel_Payment.Text = string.Empty;

            if (DivFlightDetails.InnerHtml.Length < 5)
            {
                FlyVal.InnerText = "Please enter Flight details!";
                return;
            }
            else
                FlyVal.InnerText = string.Empty;

            if (rb_Purpose_Training.Checked)
                purpose = "Training / Դասընթաց";
            else
                if (rb_Purpose_Conference.Checked)
                    purpose = "Conference / Համաժողով";
                else
                    if (rb_Purpose_Workshop.Checked)
                        purpose = "Workshop / Սեմինար";
                    else
                        if (rb_Purpose_Other.Checked)
                            purpose = tb_Purpose_other.Text;

            Hotel hot = new Hotel();
            hot.Name = ta_Hotel_Name.Text;
            hot.Dates = ta_Hotel_Dates.Text;
            hot.Location = ta_Hotel_Location.Text;
            hot.Phone = ta_Hotel_Phone.Text;
            hot.Payment = ta_Hotel_Payment.Text;

            Fly Flying = new Fly();
            Flying.Date = HF_Date.Value;
            Flying.Airline = HF_Airline.Value;
            Flying.Number = HF_Number.Value;
            Flying.DepartureCity = HF_DepartureCity.Value;
            Flying.DestinationCity = HF_DestinationCity.Value;

            #endregion

            if (!request.Add(3, autorId)) ER.GoToErrorPage("Cannot create request");

            requestId = request.GetId(autorId).ToString();

            if (!ITO.Add(requestId, "0", autor.Department, hf_Rout.Value, hf_invit.Value == "Write Your Inviting Organization:" ? null : hf_invit.Value, DateTime.Parse(tb_Start_Date.Text), DateTime.Parse(tb_End_Date.Text), purpose, rb_budget_Yes.Checked, costs, spPeoplePicker.CommaSeparatedAccounts, rb_Daily_Yes.Checked, rb_Hotel_Yes.Checked ? hot : null, Flying))
                Response.Write("ERROR - Cannot create ITO - ERROR");

            if (rb_Purpose_Other.Checked) purpose = tb_Purpose_other.Text;

            d1 = DateTime.TryParse(tb_Start_Date.Text, out startDate);
            d2 = DateTime.TryParse(tb_End_Date.Text, out endDate);

            DateTime.TryParse(tb_fl_date.Text, out tb_fl_date_datetime);

            if (spPeoplePicker.CommaSeparatedAccounts.Length == 0)
            {
                if (autor.IsCEO)
                {
                    Approve_reject.Add(AD.CFO.Login, requestId, "ViewITO.aspx");
                }
                else if (autor.IsDirector)
                {
                    Approve_reject.Add(AD.CEO.Login, requestId, "ViewITO.aspx");
                }
                else
                {
                    Approve_reject.Add(autor.Manager.Login, requestId, "ViewITO.aspx");
                }
            }
            else
            {
                foreach (var loginName in spPeoplePicker.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    string User_id = AD.GetUserBySPLogin(loginName).Login;

                    Request_Substitute.Add(User_id, requestId, autorId, endDate, startDate);

                    string msg = string.Format("<b>{0}</b> {2}<b>{1}</b><br/> wants you replace him/her", autor.FullName, autor.Department, autor.Department == null ? "" : "from ");
                    string img = autor.PictureUrl;

                    Notificaion.Add(User_id, SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + requestId, msg, img, Request_type.GetId(requestId));
                }
            }
            string urll = SPContext.Current.Web.Url + "/SitePages/SuccesCreate.aspx";
            Response.Redirect(urll);
        }

        protected void AddFlightDetails_Click(object sender, EventArgs e)
        {
            DateTime k;
            if (
                !DateTime.TryParse(tb_fl_date.Text, out k)
                || ta_fl_airline.Text == "Write airline:" || ta_fl_airline.Text == " "
                || ta_fl_number.Text == "Write flight number:" || ta_fl_number.Text == " "
                || ta_fl_dep_city.Text == "Write departure city:" || ta_fl_dep_city.Text == " "
                || ta_fl_dest_city.Text == "Write destination city:" || ta_fl_dest_city.Text == " ")
            {
                FlyVal.InnerText = "Please enter Flight details!";
                return;
            }
            else
                FlyVal.InnerText = string.Empty;

            DateTime.TryParse(tb_fl_date.Text, out tb_fl_date_datetime);
            if (tb_fl_date_datetime < DateTime.Now)
            {
                FlyVal.InnerText = "DateTime not valid!";
                return;
            }
            else
                FlyVal.InnerText = string.Empty;

            HF_Date.Value += tb_fl_date.Text + "|$$|";
            HF_Airline.Value += ta_fl_airline.Text + "|$$|";
            HF_Number.Value += ta_fl_number.Text + "|$$|";
            HF_DepartureCity.Value += ta_fl_dep_city.Text + "|$$|";
            HF_DestinationCity.Value += ta_fl_dest_city.Text + "|$$|";

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
                    tb_fl_date.Text,
                    ta_fl_airline.Text,
                    ta_fl_number.Text,
                    ta_fl_dep_city.Text,
                    ta_fl_dest_city.Text
                );


            ta_fl_airline.Text = string.Empty;
            ta_fl_number.Text = string.Empty;
            ta_fl_dep_city.Text = string.Empty;
            ta_fl_dest_city.Text = string.Empty;
        }
    }
}
