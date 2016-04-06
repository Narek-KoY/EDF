using System;
using System.Data.SqlClient;
using System.Web.UI;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_WEB_Parts_1.Menu
{
    public partial class MenuUserControl : UserControl
    {
        string connectionString = string.Empty;

        EDF_SPUser cur = ADSP.CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (cur.StatisticsAccess || cur.SoStatisticsAccess)
                StUser.Visible = true;
            else StUser.Visible = false;

            string User_ID = "smart";
            UC.SearchBox(TextBox1, "Search the Requests Database");
            try
            {
                connectionString = Constants.GetConnectionString();
                User_ID = cur.Login;
                int count = Get_NotificaionCount(User_ID);
                switch (Request.Path)
                {
                    case "/SitePages/Dashboard.aspx":
                        itm1.Attributes.Add("style", "background-color:#7ebc0a;");
                        itm11.Attributes.Add("class", "Tmenu2");
                        break;
                    case "/SitePages/My Requests.aspx":
                        itm2.Attributes.Add("style", "background-color:#7ebc0a;");
                        itm12.Attributes.Add("class", "Tmenu2");
                        break;
                    case "/SitePages/Received Requests.aspx":
                        itm3.Attributes.Add("style", "background-color:#7ebc0a;");
                        itm13.Attributes.Add("class", "Tmenu2");
                        break;
                    case "/SitePages/Statistics.aspx":
                        itm4.Attributes.Add("style", "background-color:#7ebc0a;");
                        itm14.Attributes.Add("class", "Tmenu2");
                        break;
                    case "/SitePages/Notifications.aspx":
                        itm5.Attributes.Add("style", "background-color:#7ebc0a;");
                        itm15.Attributes.Add("class", "Tmenu2");
                        break;
                }

                if (count > 0)
                    Label1.Text = count.ToString() + " ";
                else
                {
                    NewNot.Visible = false;
                    Label1.Text = string.Empty;
                    itm15.Attributes.Add("style", "margin-right:20px");
                }
            }

            catch (Exception ex) { Response.Write(ex.Message); return; }
        }

        public int Get_NotificaionCount(string User_ID)
        {
            int count = 0;

            string comand = "SELECT COUNT(Id) FROM Notificaion WHERE " +
                "User_id='" + User_ID.ToString() + "'and visited = 'False'";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_NotificaionCount -  </br> " + "Can not Get Notification from database: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Search.aspx?key=" + TextBox1.Text);
        }
    }
}
