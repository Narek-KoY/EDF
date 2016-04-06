using System;
using System.Data.SqlClient;
using System.Web.UI;
using EDF_CommonData;
using Microsoft.SharePoint;


namespace EDF_WEB_Parts_1.VacationRequestCreate
{
    public partial class VacationRequestCreateUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        EDF_SPUser autor = ADSP.CurrentUser;
        string requestId;
        string autorId;
        DateTime startDate;
        DateTime endDate;
        bool d1, d2;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!autor.IsCEO)
            {
                if (!autor.HasManager) ER.GoToErrorPage(autor.FullName + " has not manager");
                if (!autor.HasDirector) ER.GoToErrorPage(autor.FullName + " has not director");
            }

            if (IsPostBack)
            {
                if (RadioButton0.Checked || RadioButton1.Checked || RadioButton2.Checked || RadioButton3.Checked
                     || RadioButton4.Checked || RadioButton5.Checked || RadioButton6.Checked || RadioButton7.Checked)
                {
                    ValRadio.Visible = false;
                }
                else
                {
                    ValRadio.Visible = true;
                    return;
                }
            }

            string str = string.Empty;
            try
            {
                autorId = autor.Login;

                str += ":2:";
                autorImg.Src = autor.PictureUrl;
                str += ":2:";

                d1 = DateTime.TryParse(dateTimeControl1.Text, out startDate);
                d2 = DateTime.TryParse(dateTimeControl2.Text, out endDate);
                str += ":2:";
                if (d1 && startDate < DateTime.Now.AddDays(8) && RadioButton0.Checked)
                {
                    paymentLi.Visible = true;
                    CheckBox8.Checked = true;
                    CheckBox8.Enabled = false;
                }
                else
                {
                    str += ":2:";
                    if (IsPostBack)
                    {
                        str += ":2:";
                        paymentLi.Visible = false;
                        CheckBox8.Checked = false;
                    }
                }
                str += ":2:";
                if (autor.IsMemberOf(AD.CCShiftEmployees))
                    CheckDiv.Visible = true;
                else
                    CheckDiv.Visible = false;
                str += ":2:";
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - VacationRequestUserControl -  </br> " + str + "<>" + ex.Message);
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            string str = string.Empty;
            try
            {
                str += ":1:";
                d1 = DateTime.TryParse(dateTimeControl1.Text, out startDate);
                d2 = DateTime.TryParse(dateTimeControl2.Text, out endDate);
                str += ":1:";
                if (!(d1 && d2)) { datevalid.Text = "<span style='color:red'>Date is not valid</span>"; return; }
                else if (startDate.Date < DateTime.Now.Date || startDate.Date > endDate.Date) { datevalid.Text = "<span style='color:orange'>Date Not Valid</span>"; return; }
                str += ":1:";

                str += ":1:";
                AddRequest(1, autorId);
                str += ":1:";
                requestId = request.GetId(autorId).ToString();

                str += ":1:";

                if (spPeoplePicker.CommaSeparatedAccounts.Length == 0)
                {
                    if (autor.IsCEO)
                    {
                        str += ":1:";
                        Approve_reject.Add(AD.HR.Login, requestId, "ViewReplacement.aspx", "HR");
                        str += ":1:";
                    }
                    else if (autor.IsDirector)
                    {
                        str += ":1:";
                        Approve_reject.Add(AD.CEO.Login, requestId, "ViewReplacement.aspx", "CEO");
                        str += ":1:";
                    }
                    else if (autor.Manager.IsDirector)
                    {
                        str += ":1:";
                        Approve_reject.Add(autor.Manager.Login, requestId, "ViewReplacement.aspx", "director");
                        str += ":1:";
                    }
                    else
                    {
                        str += ":1:";
                        Approve_reject.Add(autor.Manager.Login, requestId, "ViewReplacement.aspx", "manager");
                        str += ":1:";
                    }
                }
                else
                {
                    foreach (var loginName in spPeoplePicker.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        str += ":1:";
                        string User_id = AD.GetUserBySPLogin(loginName).Login;
                        str += ":1:";
                        Request_Substitute.Add(User_id, requestId, autorId, endDate, startDate);
                        str += ":1:";
                        string msg = string.Format("<b>{0}</b> {2}<b>{1}</b><br/> wants you replace him/her", autor.FullName, autor.Department, autor.Department == null ? "" : "from ");
                        string img = autor.PictureUrl;
                        str += ":1:";
                        Notificaion.Add(User_id, SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + requestId, msg, img, Request_type.GetId(requestId));
                        str += ":1:";
                    }
                } str += ":1:";
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - VacationRequestUserControl -  </br> " + "Can not Create Vacation Request: " + ex.Message + "::::" + str);
            }
            string urll = SPContext.Current.Web.Url + "/SitePages/SuccesCreate.aspx";
            Response.Redirect(urll);
            str += ":1:";
        }

        public void AddRequest(int Type_id, string Autor_id)
        {
            string comand = "INSERT INTO Request (Type_id, Autor_id, Add_date)" +
                                         "VALUES ('" + Type_id.ToString() + "', " +
                                                 "'" + Autor_id.ToString() + "', " +
                                                 "'" + DateTime.Now + "')";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(comand, con);

                con.Open();
                command.ExecuteNonQuery();

                #region Add Vacation

                string Full_Name = Autor_id;

                string Vacation_type = string.Empty;

                if (RadioButton0.Checked)
                    Vacation_type = "Annual";
                if (RadioButton1.Checked)
                    Vacation_type = "Pregnancy and maternity";
                if (RadioButton2.Checked)
                    Vacation_type = "Non-paid";
                if (RadioButton3.Checked)
                    Vacation_type = "Exam (non-paid)";
                if (RadioButton4.Checked)
                    Vacation_type = "Marriage (non-paid)";
                if (RadioButton5.Checked)
                    Vacation_type = "Close relative’s death";
                if (RadioButton6.Checked)
                    Vacation_type = "Relative’s death (non-paid)";
                if (RadioButton7.Checked)
                    Vacation_type = "Non-paid vacation";

                bool Payment_terms = CheckBox8.Checked;

                string Days_offs = (CheckBox1.Checked ? "1|" : "") + (CheckBox2.Checked ? "2|" : "") + (CheckBox3.Checked ? "3|" : "") + (CheckBox4.Checked ? "4|" : "") +
                                    (CheckBox5.Checked ? "5|" : "") + (CheckBox6.Checked ? "6|" : "") + (CheckBox7.Checked ? "7|" : "");

                if (Days_offs.Length > 1) Days_offs = Days_offs.Substring(0, Days_offs.Length - 1);

                requestId = request.GetId(Autor_id).ToString();

                AddVacation(requestId, 0, Full_Name, "Department_and_position", Vacation_type, startDate, endDate, Days_offs, Payment_terms);

                #endregion

            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - EDFAddRequest -  </br> " + "Can not create Vacation Request: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void AddVacation(string REQUEST_ID, int number, string Full_Name, string Department_and_position,
                                string Vacation_type, DateTime Start_date, DateTime End_date, string Days_offs, bool Payment_terms)
        {
            string comand = string.Format("INSERT INTO [vacation] (REQUEST_ID, number, Filling_date, Full_Name, Department_and_position, Vacation_type, Start_date, End_date, Days_offs, Payment_terms) " +
                "VALUES ('{0}', '{1}', convert(datetime,'{2}',101), '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')",

                REQUEST_ID,
                number.ToString(),
                DateTime.Now,
                Full_Name,
                Department_and_position,
                Vacation_type,
                Start_date,
                End_date,
                Days_offs,
                Payment_terms.ToString()
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
                ER.GoToErrorPage(" - AddVacation -  </br> " + "Can not Create Vacation Request Document in Database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Dashboard.aspx");
        }
        protected void RadioButton0_CheckedChanged(object sender, EventArgs e)
        {
            ValRadio.Visible = false;
            if (d1 && startDate < DateTime.Now.AddDays(8) && RadioButton0.Checked)
            {
                paymentLi.Visible = true;
                CheckBox8.Checked = true;
                CheckBox8.Enabled = false;
            }
            else
            {
                paymentLi.Visible = false;
                CheckBox8.Checked = false;
            }
        }
    }
}
