using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.Search
{
    public partial class SearchUserControl : UserControl
    {
        string connectionString = Constants.GetConnectionString();
        string key = string.Empty;
        DataTable table = new DataTable();
        string User_ID = ADSP.CurrentUser.Login;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["key"] != null) key = hf_key.Value = Request.QueryString["key"].ToString();
            else key = hf_key.Value;
            
            ScriptManager.RegisterStartupScript(
           UpdatePanel1,
           this.GetType(),
           "MyAction",
           "picker();",
           true);

            if (IsPostBack) return;

            start.Value = "1";

            ul1.InnerHtml = ul2.InnerHtml = string.Empty;

            table = Get_Notificaion(User_ID, int.Parse(start.Value), 5);

            int count = table.Rows.Count;

            NotCount.Value = count.ToString();

            start.Value = "6";

            DrawList(table);
        }

        void Add(DataTable table)
        {
            if (table.Rows.Count > 0)
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string id = table.Rows[i]["id"].ToString();
                    string autor_id = table.Rows[i]["Autor_id"].ToString();
                    var autor = AD.GetUserBySPLogin(autor_id);
                    string avatat = autor.PictureUrl;
                    string UserName = autor.FullName;
                    string Dep = autor.Department;
                    string t1 = table.Rows[i]["type_name"].ToString();
                    string t2 = table.Rows[i]["icon"].ToString();
                    string t4 = table.Rows[i]["Add_date"].ToString();
                    string t5 = EDF.GetUppers(t1) + " " + table.Rows[i]["Id"].ToString();
                    string State = table.Rows[i]["Status"].ToString();
                    string Icon = State == "True" ? "/_catalogs/masterpage/images/ok.png" : State == "False" ? "/_catalogs/masterpage/images/x.png" : "/_catalogs/masterpage/images/timer.png";

                    string link = "";
                    switch (t1)
                    {
                        case "Vacation Request":
                            link = SPContext.Current.Web.Url + "/SitePages/ViewReplacement.aspx?rid=" + id;
                            break;
                        case "Local Travel Request":
                            link = SPContext.Current.Web.Url + "/SitePages/ViewLTR.aspx?rid=" + id;
                            break;
                        case "International Travel Order":
                            link = SPContext.Current.Web.Url + "/SitePages/ViewITO.aspx?rid=" + id;
                            break;
                        case "Round Sheet Request":
                            link = SPContext.Current.Web.Url + "/SitePages/ViewRSR.aspx?rid=" + id;
                            break;
                        case "Database Access Request":
                            link = SPContext.Current.Web.Url + "/SitePages/ViewDAR.aspx?rid=" + id;
                            break;
                        case "Stock Out Request":
                            link = SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + id;
                            break;
                    }

                    ul1.InnerHtml += string.Format("<li  style='height: 100px;' class=' cont clearfix' onclick='location.href=&quot;{0}&quot;;' style='cursor:pointer;'><span><div style='background:url({1}) no-repeat 28px;' class='img_logo my_img_logo fleft'><img class='avatar' src='{2}' /></div><p class='text4 fleft'><span style='font-size:17px'><b>{3}</b> by <b>{4}</b><br/> from {5}<br />ID:  {6}<br /><h9>{7}</h9></span></p><div class='timer fright'><img src='{8}' /></div></span></li>",
                        link,
                        t2,
                        avatat,
                        t1,
                        UserName,
                        Dep,
                        t5,
                        t4,
                        Icon);
                }
        }

        public void DrawList(DataTable table)
        {
            Add(table);
            return;
        }

        public DataTable Get_Notificaion(string User_id, int start, int count)
        {
            HFdate.Value = "DESC";
            return EDF.Search(User_ID, start, count, false, DateTime.Parse("01/01/1900"), DateTime.Parse("01/01/9000"), "", "", "DESC", key);
        }
        
        protected void Status_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;

            string status = string.Empty, type = string.Empty, date = "DESC";

            if (btn.ID != "SearchKey")
            {
                status = HFstatus.Value;
                type = HFtype.Value;
                date = HFdate.Value;

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
            }
            if (btn.ID != "ButtonAll")
            {
                HFstatus.Value = status;
                HFtype.Value = type;
                HFdate.Value = date;

            }

            DateTime Start_date;
            DateTime End_date;

            bool d1 = DateTime.TryParse(sdate.Value, out Start_date);
            bool d2 = DateTime.TryParse(enddate.Value, out End_date);

            if (btn.ID == "ButtonAll")
            {
                int count = 10;
                table = Get_Notifications(User_ID, int.Parse(start.Value), count, false, d1 ? Start_date : DateTime.Parse("1/1/1900"), d2 ? End_date : DateTime.Parse("1/1/9000"), status, type, date, key);

                DrawList(table);

                int h = int.Parse(start.Value);
                start.Value = (h + 10).ToString();
            }
            else
            {
                table = Get_Notifications(User_ID, 1, 5, false, d1 ? Start_date : DateTime.Parse("1/1/1900"), d2 ? End_date : DateTime.Parse("1/1/9000"), status, type, date, key);

                ul1.InnerHtml = ul2.InnerHtml = string.Empty;

                start.Value = "6";

                DrawList(table);
            }
        }

        public DataTable Get_Notifications(string User_ID, int start, int count, bool allRequest, DateTime startDate, DateTime endDate, string sortByStatus, string sortByType, string sortByDate, string keywords)
        {
            return EDF.Search(User_ID, start, count, allRequest, startDate, endDate, sortByStatus, sortByType, sortByDate, keywords);
        }
    }
}
