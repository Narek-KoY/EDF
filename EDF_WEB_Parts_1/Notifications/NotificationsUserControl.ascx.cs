using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;

namespace EDF_WEB_Parts_1.Notifications
{
    public partial class NotificationsUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        DataTable table = new DataTable();

        string User_ID = ADSP.CurrentUser.Login;

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                       UpdatePanel1,
                       this.GetType(),
                       "MyAction",
                       "picker();",
                       true);

            if (IsPostBack) return;

            start.Value = "1";

            string NId = Request.QueryString["nid"];
            UpdateNotificaion(NId, true);

            ul1.InnerHtml = ul2.InnerHtml = string.Empty;

            NotCount.Value = Get_AllNotificaionCount(User_ID).ToString();

            int count = Get_NotificaionCount(User_ID);

            Label1.Text = count.ToString();

            table = Get_Notificaion(User_ID, int.Parse(start.Value), count < 8 ? 8 : (count + 1));

            start.Value = ((count < 8 ? 8 : (count + 1)) + 1).ToString();

            DrawList(table);

            setReadAllNotifications(User_ID);

        }

        public void setReadAllNotifications(string userId)
        {
            string cmd = string.Format("UPDATE [Notificaion] SET [visited] = 'True' WHERE User_id = '{0}'", userId);
            SqlConnection con = new SqlConnection(connectionString);
            int count = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(cmd, con);
                count = (int)com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - setReadAllNotifications -  </br> " + "Can not setReadAllNotifications From Datebase: " + ex.Message);
            }
        }

        public void DrawList(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string t1 = ((DateTime)table.Rows[i]["Date_Add"]).ToString("dd/MM/yyyy");
                //     string t11 = ((DateTime)table.Rows[i]["Date_Add"]).ToString("dd/MM/yyyy");
                string t2 = table.Rows[i]["visited"].ToString();
                string t3 = table.Rows[i]["Visit_url"].ToString() + "&nid=" + table.Rows[i]["Id"];
                string t4 = table.Rows[i]["notification"].ToString();
                string t5 = table.Rows[i]["TypeImgUrlOrange"].ToString();
                string t6 = table.Rows[i]["TypeImgUrlDark"].ToString();

                string t7 = table.Rows[i]["User_id"].ToString();

                string avatat = AD.GetUserByLogin(t7).PictureUrl;

                if (t2.ToLower() == "false")
                {
                    ul1.InnerHtml +=
                          "<li class='cont clearfix' style='cursor:pointer;'>" +
                              "<a href='" + t3 + "'>" +
                                "<div style='background:url(" + t5 + ") no-repeat 28px;' class='img_logo fleft'><img class='avatar' src='" + avatat + "' /></div>" +
                                "<p class='text3 fleft'>" + t4 + "</p>" +
                                "<div class='clock fright'><p>" + t1 + "</p></div>" +
                              "</a>" +
                           "</li>";
                }
                else
                {
                    ul2.InnerHtml +=
                         "<li class='cont clearfix' style='cursor:pointer;'>" +
                             "<a href='" + t3 + "'>" +
                                "<div style='background:url(" + t5 + ") no-repeat 28px;' class='img_logo fleft'><img  class='avatar' src='" + avatat + "' /></div>" +
                                "<p class='text3 fleft'>" + t4 + "</p>" +
                                "<div class='clock fright'><p>" + t1 + "</p></div>" +
                             "</a>" +
                          "</li>";
                }
            }
        }

        public int Get_NotificaionCount(string User_ID)
        {
            int count = 0;

            string comand = "SELECT COUNT(Id) FROM Notificaion WHERE " +
                "User_id='" + User_ID + "'and visited = 'False'";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_NotificaionCount -  </br> " + "Can not Get Notificatons From Datebase: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        public int Get_AllNotificaionCount(string User_ID)
        {
            int count = 0;

            string comand = "SELECT COUNT(Id) FROM Notificaion WHERE " +
                "User_id='" + User_ID.ToString() + "'";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_AllNotificaionCount -  </br> " + "Can not Get All Notificatons From Datebase: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        public DataTable Get_Notificaion(string User_id, int start, int count)
        {
            string comand = string.Format("select * from ( " +
               "SELECT ID, User_id, Date_Add, visited, Visit_url, notification, " +
               "ROW_NUMBER() OVER ({0}) as RowNum, " +

                "(Select Request_type.ImgUrlOrange FROM Request_type WHERE Request_type.ID = Notificaion.Type_id) as TypeImgUrlOrange,  " +
                "(Select Request_type.ImgUrlDark FROM Request_type WHERE Request_type.ID = Notificaion.Type_id) as TypeImgUrlDark " +

                "FROM Notificaion WHERE User_id = '{3}'" +
               ") AS MyDerivedTable WHERE MyDerivedTable.RowNum BETWEEN {1} AND {2}",

               "order by [Date_Add] DESC ",
               start.ToString(),
               (start + count - 1).ToString(),
               User_id
               );

            DataTable table = new DataTable("Notificaion");

            HFdate.Value = "DESC";
            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_Notificaion -  </br> " + "Can not Get Notificatons From Datebase: " + ex.Message);
            }
            return table;
        }

        protected void ButtonAll_Click1(object sender, EventArgs e)
        {
            int count = 5;

            if (int.Parse(NotCount.Value) <= int.Parse(start.Value))
                return;

            DataTable table = Get_Notificaion(User_ID, int.Parse(start.Value) + 1, count);

            DrawList(table);

            int h = int.Parse(start.Value);
            start.Value = (h + 5).ToString();
        }

        protected void Status_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;

            List<string> key = new List<string>();

            string status = string.Empty, type = string.Empty, date = "DESC";

            if (btn.ID != "SearchKey")
            {
                status = HFstatus.Value;
                type = HFtype.Value;
                date = HFdate.Value;
                key.Clear();
                foreach (var k in HFkey.Value.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                { key.Add(k); }

            }
            switch (btn.ID)
            {
                case "status":
                    st_type.Attributes["class"] = "step1 fright";
                    st_date.Attributes["class"] = "step1 fright";
                    if (st_status.Attributes["class"].Contains("step1"))
                    {
                        st_status.Attributes["class"] = "step2 fright";
                        status = "ASC";
                    }
                    else
                    {
                        st_status.Attributes["class"] = "step1 fright";
                        status = "DESC";
                    }
                    break;
                case "typeS":
                    st_date.Attributes["class"] = "step1 fright";
                    st_status.Attributes["class"] = "step1 fright";
                    if (st_type.Attributes["class"].Contains("step1"))
                    {
                        st_type.Attributes["class"] = "step2 fright";
                        type = "ASC";
                    }
                    else
                    {
                        st_type.Attributes["class"] = "step1 fright";
                        type = "DESC";
                    }
                    break;
                case "date":
                    st_status.Attributes["class"] = "step1 fright";
                    st_type.Attributes["class"] = "step1 fright";
                    if (st_date.Attributes["class"].Contains("step1"))
                    {
                        st_date.Attributes["class"] = "step2 fright";
                        date = "ASC";
                    }
                    else
                    {
                        st_date.Attributes["class"] = "step1 fright";
                        date = "DESC";
                    }
                    break;
                case "SearchKey":
                    foreach (var loginName in spPeoplePicker.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        string User_id = AD.GetUserBySPLogin(loginName).Login;
                        key.Add(User_id);
                    }
                    HFkey.Value = string.Empty;
                    break;
            }
            if (btn.ID != "ButtonAll")
            {
                HFstatus.Value = status;
                HFtype.Value = type;
                HFdate.Value = date;
                foreach (string k in key)
                    HFkey.Value += k + "|";
            }

            DateTime Start_date;
            DateTime End_date;

            bool d1 = DateTime.TryParse(sdate.Value, out Start_date);
            bool d2 = DateTime.TryParse(enddate.Value, out End_date);

            if (btn.ID == "ButtonAll")
            {
                int count = 10;

                if (int.Parse(NotCount.Value) < int.Parse(start.Value))
                    return;

                table = Get_Notifications(User_ID, int.Parse(start.Value), count, false, d1 ? Start_date : DateTime.Parse("1/1/1900"), d2 ? End_date : DateTime.Parse("1/1/9000"), status, type, date, key);

                DrawList(table);

                int h = int.Parse(start.Value);
                start.Value = (h + count).ToString();
            }
            else
            {
                start.Value = "11";
                table = Get_Notifications(User_ID, 1, 10, false, d1 ? Start_date : DateTime.Parse("1/1/1900"), d2 ? End_date : DateTime.Parse("1/1/9000"), status, type, date, key);

                ul1.InnerHtml = ul2.InnerHtml = string.Empty;

                DrawList(table);
            }
        }

        public DataTable Get_Notifications(string User_ID, int start, int count, bool allRequest, DateTime startDate, DateTime endDate, string sortByStatus, string sortByType, string sortByDate, List<string> keywords)
        {
            string keyQuery = "";
            if (keywords.Count == 1) { keyQuery = String.Format("Autor = '{0}' and ", keywords[0]); }
            else if (keywords.Count > 1)
            {
                keyQuery = "(";

                foreach (string key in keywords)
                {
                    keyQuery += String.Format("Autor = '{0}' or ", key);
                }
                keyQuery = keyQuery.Substring(0, keyQuery.Length - 3);
                keyQuery += ") and ";
            }
            string comand = string.Format("select * FROM (select *, ROW_NUMBER() OVER ({0}) as RowNum from " +
               "(SELECT ID, User_id, Date_Add, visited, Visit_url, notification, Type_id, " +
               "(Select Request_type.ImgUrlOrange FROM Request_type WHERE Request_type.ID = Notificaion.Type_id) as TypeImgUrlOrange, " +
               "(Select Request_type.ImgUrlDark FROM Request_type WHERE Request_type.ID = Notificaion.Type_id) as TypeImgUrlDark, " +
               "(Select Request.Autor_id FROM Request WHERE Request.Id = (select item from dbo.fnSplit(Notificaion.Visit_url, '?rid=') where item not like '%aspx%')) as Autor " +
               "FROM Notificaion WHERE User_id = '{3}' ) AS MyDerivedTable " +
               "WHERE {4} " +

                " ([Date_add] between convert(datetime,'{5}',101) and convert(datetime,'{6}',101))) AS new_table WHERE (new_table.RowNum BETWEEN {1} AND {2}) ",

               ("order by " +
               (sortByStatus != "" ? "[visited] " + sortByStatus + ", " : "") +
               (sortByType != "" ? "[Type_id] " + sortByType + ", " : "") +
               "[Date_Add] " + sortByDate),

              start.ToString(),
              (start + count - 1).ToString(),
              User_ID,
              keyQuery,
              startDate,
              endDate
              );

            DataTable table = new DataTable("Approve_reject");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_Approve_reject -  </br> " + "Can not Get Notificatons From Datebase: " + ex.Message);
            }
            return table;
        }

        public void UpdateNotificaion(string NId, bool Is_ok)
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
                ER.GoToErrorPage(" - UpdateNotificaion -  </br> " + "Can not Get Notificatons From Datebase: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
