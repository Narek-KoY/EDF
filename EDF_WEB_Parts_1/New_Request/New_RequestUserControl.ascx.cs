using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using EDF_CommonData;


namespace EDF_WEB_Parts_1.New_Request
{
    public partial class New_RequestUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                DataTable table = GetRequestType();

                Label1.Text = table.Rows[0]["Name"].ToString();
                Label2.Text = table.Rows[1]["Name"].ToString();
                Label3.Text = table.Rows[2]["Name"].ToString();
                Label4.Text = table.Rows[3]["Name"].ToString();
                Label5.Text = table.Rows[4]["Name"].ToString();
                Label6.Text = table.Rows[5]["Name"].ToString();

                string url1 = table.Rows[0]["ImgUrlOrange"].ToString();
                string url2 = table.Rows[1]["ImgUrlOrange"].ToString();
                string url3 = table.Rows[2]["ImgUrlOrange"].ToString();
                string url4 = table.Rows[3]["ImgUrlOrange"].ToString();
                string url5 = table.Rows[4]["ImgUrlOrange"].ToString();
                string url6 = table.Rows[5]["ImgUrlOrange"].ToString();

                img1.Attributes.Add("style", "background:url(" + url1 + ")");
                img2.Attributes.Add("style", "background:url(" + url2 + ")");
                img3.Attributes.Add("style", "background:url(" + url3 + ")");
                img4.Attributes.Add("style", "background:url(" + url4 + ")");
                img5.Attributes.Add("style", "background:url(" + url5 + ")");
                img6.Attributes.Add("style", "background:url(" + url6 + ")");

                if (ADSP.CurrentUser.IsCEO)
                {
                    LTR.Attributes["onclick"] = "#";
                    LTR.Attributes["style"] = "cursor:default;";
                }
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - New Request -  </br> " + "Can not View New Request" + ex.Message);
            }
        }

        public DataTable GetRequestType()
        {
            string comand = "select * from Request_type";

            DataTable table = new DataTable("Request_type");

            try
            {
                SqlConnection con = new SqlConnection(connectionString);

                SqlDataAdapter adapter = new SqlDataAdapter(comand, con);

                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                ER.GoToErrorPage(" - GetRequestType -  </br> " + ex.Message);
            }
            return table;
        }
    }
}
