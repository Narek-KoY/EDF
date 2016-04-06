using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using EDF_CommonData;


namespace EDF_WEB_Parts_1.DashboardNotifications
{
    public partial class DashboardNotificationsUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        protected void Page_Load(object sender, EventArgs e)
        {
            string User_id = ADSP.CurrentUser.Login;

            int count = Get_NotificaionCount(User_id);

            Label1.Text = count.ToString();

            DataTable table = Get_Notificaion(User_id, 0, count < 8 ? 8 : (count + 1));

            DrawList(table);

        }

        public void DrawList(DataTable table)
        {
            ul1.InnerHtml = string.Empty;
            ul2.InnerHtml = string.Empty;
            for (int i = 0; i < table.Rows.Count; i++)
            {

                string t1 = ((DateTime)table.Rows[i]["Date_Add"]).ToString("dd/MM/yyyy");
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
                        "<li class='cont clearfix' onclick='location.href=&quot;" + t3 + "&quot;;' style='cursor:pointer;'>" +
                            "<a href='" + t3 + "'>" +
                                "<div style='background:url(" + t5 + ") no-repeat 28px;' class='img_logo fleft'><img class='avatar' src='" + avatat + "' /></div>" +
                                "<p class='text3 fleft'>" + t4 + "</p>" +
                                "<div class='clock fright'><p class='text3 fleft' style='font-size: 14px;width: 90px;'><span>" + t1 + "</span></p></div>" +
                            "</a>" +
                        "</li>";
                }
                else
                {
                    ul2.InnerHtml +=
                        "<li class='cont color clearfix' onclick='location.href=&quot;" + t3 + "&quot;;' style='cursor:pointer;'>" +
                            "<a href='" + t3 + "'>" +
                                "<div style='background:url(" + t5 + ") no-repeat 28px;' class='img_logo fleft'><img class='avatar' src='" + avatat + "' /></div>" +
                                "<p class='text3 fleft'>" + t4 + "</p>" +
                                "<div class='clock fright'><p class='text3 fleft' style='font-size: 14px;width: 90px;'><span>" + t1 + "</span></p></div>" +
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
                count = int.Parse(com.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage("ERROR - Get_NotificaionCount - ERROR </br> " + "Can not Get Notificatons From Datebase: " + ex.Message);
                //Response.Write("ERROR - Get_NotificaionCount - ERROR"); 
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

               "order by [visited] , [Date_Add] DESC, [Id] DESC ",
               start.ToString(),
               (start + count).ToString(),
               User_id
               );

            DataTable table = new DataTable("Notificaion");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage("ERROR - Get_Notificaion - ERROR </br> " + "Can not Get Notificatons From Datebase: " + ex.Message);
                //Response.Write("ERROR - Get_Notificaion - ERROR"); 
            }
            return table;
        }
    }
}
