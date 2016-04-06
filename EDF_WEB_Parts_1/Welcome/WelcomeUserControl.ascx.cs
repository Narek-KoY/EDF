using System;
using System.Data.SqlClient;
using System.Web.UI;
using EDF_CommonData;


namespace EDF_WEB_Parts_1.Welcome
{
    public partial class WelcomeUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        EDF_SPUser Cur = ADSP.CurrentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            AD.ResetDicionary();

            string User_ID = Cur.Login;
            img_avatar.Src = Cur.PictureUrl;

            Label1.Text = Get_NotificaionCount(User_ID).ToString() + " ";
            Label2.Text = Get_RequestCount(User_ID) .ToString() + " ";
            Label3.Text = (GetCountRequestsForMe(User_ID)+ GetStockOutRequestsForMe()).ToString() + " ";
            Label4.Text = Cur.FullName.Length == 0 ? User_ID : Cur.FullName;

            if (IsPostBack) return;
        }

        public int Get_NotificaionCount(string User_ID)
        {
            int count = 0;

            string comand = "SELECT COUNT(Id) FROM Notificaion WHERE " +
                "User_ID='" + User_ID.ToString() + "'and visited = 'False'";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_NotificaionCount -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        public int Get_RequestCount(string Autor_id)
        {
            int count = 0;

            string comand = "SELECT COUNT(Id) FROM [Request] WHERE " +
                "Autor_id='" + Autor_id.ToString() + "'and State is null";

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - Get_RequestCount -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        public int GetCountRequestsForMe(string User_ID)
        {
            int count = 0;

            string comand = string.Format(@"SELECT COUNT(*) as count FROM (
SELECT c.co FROM (
SELECT 
CASE 
            WHEN (SELECT COUNT(*) FROM Approve_reject WHERE Request_ID=app_rej.Request_ID AND Approve_reject.Status = app_rej.Status) > 0
               THEN 
               (SELECT COUNT(*) as count FROM Approve_reject tmp_app_rej WHERE tmp_app_rej.Request_ID = app_rej.Request_ID AND tmp_app_rej.Status = app_rej.Status AND tmp_app_rej.App_rej IS NOT NULL)
               ELSE app_rej.App_rej 
       END as co
  FROM Approve_reject app_rej WHERE  app_rej.User_ID = '{0}' AND app_rej.App_rej is null) as c WHERE c.co IS NULL OR c.co = '0') as t", User_ID);

            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetCountRequestsForMe -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        public int GetStockOutRequestsForMe()
        {
            int count = 0;
            string comand = string.Empty;
            if (Cur.IsAdminStockkeeper || Cur.IsRaomarsStockkeeper)
            {
                 comand = string.Format(@"SELECT COUNT(*) FROM SORApprove WHERE RecievedUserId = '{0}' and RequestStatus is null",
                    (Cur.IsAdminStockkeeper) ? "adminstockkeeper" : "raomarsstockkeeper");

            }
            else
            {
                 comand = string.Format(@"SELECT COUNT(*) FROM SORApprove WHERE RecievedUserId = '{0}' and RequestStatus is null",
                    Cur.Login);
            }

            
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(comand, con);
                count = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetCountRequestsForMe -  </br> " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return count;
        }
    }
}
