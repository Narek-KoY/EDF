using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.ReceivedRequestsPast
{
    public partial class ReceivedRequestsPastUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hf_asc.Value = "desc";

                Sort("d");
            }
        }

        protected void date_Click(object sender, EventArgs e)
        {
            SetClass(sender);
            Sort("d");
        }
        protected void status_Click(object sender, EventArgs e)
        {
            SetClass(sender);
            Sort("s");
        }
        protected void type_Click(object sender, EventArgs e)
        {
            SetClass(sender);
            Sort("t");
        }

        void SetClass(object sender)
        {
            LinkButton btn = sender as LinkButton;

            switch (btn.ID)
            {
                case "status":
                    st_type.Attributes["class"] = "step1 fright";
                    st_date.Attributes["class"] = "step1 fright";
                    if (st_status.Attributes["class"].Contains("step1"))
                    {
                        st_status.Attributes["class"] = "step2 fright";
                        hf_asc.Value = "asc";
                    }
                    else
                    {
                        st_status.Attributes["class"] = "step1 fright";
                        hf_asc.Value = "desc";
                    }
                    break;
                case "type":
                    st_date.Attributes["class"] = "step1 fright";
                    st_status.Attributes["class"] = "step1 fright";
                    if (st_type.Attributes["class"].Contains("step1"))
                    {
                        st_type.Attributes["class"] = "step2 fright";
                        hf_asc.Value = "asc";
                    }
                    else
                    {
                        st_type.Attributes["class"] = "step1 fright";
                        hf_asc.Value = "desc";
                    }
                    break;
                case "date":
                    st_status.Attributes["class"] = "step1 fright";
                    st_type.Attributes["class"] = "step1 fright";
                    if (st_date.Attributes["class"].Contains("step1"))
                    {
                        st_date.Attributes["class"] = "step2 fright";
                        hf_asc.Value = "asc";
                    }
                    else
                    {
                        st_date.Attributes["class"] = "step1 fright";
                        hf_asc.Value = "desc";
                    }
                    break;
            }
        }
        void Sort(string SortBy)
        {
            int count = int.Parse(hf_start.Value);
            string orderBy = SortBy == "s" ? "[State] " + hf_asc.Value : "" +
                             SortBy == "t" ? "[Type_id] " + hf_asc.Value : "" +
                             SortBy == "d" ? "[Date_add] " + hf_asc.Value : "DESC";

            DataTable tb = EDF.Get_Approve_reject(ADSP.CurrentUser, 1, count, true, orderBy);

            ulPend.InnerHtml = string.Empty;
            if (tb.Rows.Count > 0)
                for (int i = 1; i <= tb.Rows.Count; i++)
                {
                    EDF_SPUser Autor = AD.GetUserBySPLogin(tb.Rows[i - 1]["Autor_id"].ToString());
                    string avatar = Autor.PictureUrl;
                    string typestring = tb.Rows[i - 1]["TypeName"].ToString();
                    string ImageOrange = tb.Rows[i - 1]["TypeImgUrlOrange"].ToString();
                    string AddDate = tb.Rows[i - 1]["Date_Add"].ToString();
                    string id = tb.Rows[i - 1]["Request_ID"].ToString();
                    string SID = EDF.GetUppers(typestring) + " " + id;
                    string UserName = Autor.FullName;

                    string type = tb.Rows[i - 1]["Type_id"].ToString();
                    if (type != "6")
                    {
                        if (Approve_reject.isApproved(id, tb.Rows[i - 1]["Status"].ToString()))
                            continue;
                    }
                    else
                    {
                        if (StockOutRequestDAO.GetRequestStatusByUser(Convert.ToInt32(id), ADSP.CurrentUser.Login) == true)
                            continue;
                    }

                    string LinkPage = string.Empty;
                    switch (type)
                    {
                        case "1": LinkPage = "ViewReplacement.aspx"; break;
                        case "2": LinkPage = "ViewLTR.aspx"; break;
                        case "3": LinkPage = "ViewITO.aspx"; break;
                        case "4": LinkPage = "ViewRSR.aspx"; break;
                        case "5": LinkPage = "ViewDAR.aspx"; break;
                        case "6": LinkPage = "InternalStockOutRequestView.aspx"; break;
                    }
                    string Link = string.Format("{0}/SitePages/{1}?rid={2}", SPContext.Current.Web.Url, LinkPage, id.ToString());

                    ulPend.InnerHtml += string.Format("<a href='{4}'><li style='cursor:pointer;' class='cont color clearfix'><div style='background:url({3}) no-repeat 28px;' class='img_logo fleft'><img class='avatar' src='{5}' />" +
                     "</div><p class='text4 fleft'><span><b>{0}</b> from <b>{6}</b><br />ID: {1}<br /><h9>{2}</h9></span></p><div class='timer fright'><img src='/_catalogs/masterpage/images/timer.png'/></div></li></a>",

                     typestring,
                     SID,
                     DateTime.Parse(AddDate).ToString("dd/MM/yyyy HH:mm:ss"),
                     ImageOrange,
                     Link,
                     avatar,
                     UserName);
                }

            tb = EDF.Get_Approve_reject(ADSP.CurrentUser, 1, count, false, orderBy);

            ulAppRej.InnerHtml = string.Empty;
            if (tb.Rows.Count > 0)
                for (int i = 1; i <= tb.Rows.Count; i++)
                {
                    EDF_SPUser Autor = AD.GetUserBySPLogin(tb.Rows[i - 1]["Autor_id"].ToString());
                    string avatar = Autor.PictureUrl;

                    string typestring = tb.Rows[i - 1]["TypeName"].ToString();
                    string ImageOrange = tb.Rows[i - 1]["TypeImgUrlOrange"].ToString();
                    string AddDate = tb.Rows[i - 1]["Date_Add"].ToString();
                    string id = tb.Rows[i - 1]["Request_ID"].ToString();
                    string AppRej = tb.Rows[i - 1]["App_rej"].ToString();
                    string SID = EDF.GetUppers(typestring) + " " + id;
                    string UserName = Autor.FullName;
                    string type = tb.Rows[i - 1]["Type_id"].ToString();
                    if (type != "6")
                    {
                        if (!Approve_reject.isApproved(id, tb.Rows[i - 1]["Status"].ToString()))
                        {
                            continue;
                        }
                    }
                    else
                    {

                    }


                    string LinkPage = string.Empty;
                    switch (type)
                    {
                        case "1": LinkPage = "ViewReplacement.aspx"; break;
                        case "2": LinkPage = "ViewLTR.aspx"; break;
                        case "3": LinkPage = "ViewITO.aspx"; break;
                        case "4": LinkPage = "ViewRSR.aspx"; break;
                        case "5": LinkPage = "ViewDAR.aspx"; break;
                        case "6": LinkPage = "InternalStockOutRequestView.aspx"; break;
                    }
                    string Link = string.Format("{0}/SitePages/{1}?rid={2}", SPContext.Current.Web.Url, LinkPage, id.ToString());

                    ulAppRej.InnerHtml += string.Format("<a href='{4}'><li style='cursor:pointer;' class='cont color clearfix'><div style='background:url({3}) no-repeat 28px;' class='img_logo fleft'><img class='avatar' src='{5}' />" +
                    "</div><p class='text4 fleft'><span><b>{0}</b> from <b>{6}</b><br />ID: {1}<br /><h9>{2}</h9></span></p><div class='timer fright'><img src=" + (AppRej == "True" ? "'/_catalogs/masterpage/images/ok.png'" : "'/_catalogs/masterpage/images/x.png'") + "/></div></li></a>",

                    typestring,
                    SID,
                    DateTime.Parse(AddDate).ToString("dd/MM/yyyy HH:mm:ss"),
                    ImageOrange,
                    Link,
                    avatar,
                    UserName);
                }
        }
        protected void BtnLoad_Click(object sender, EventArgs e)
        {
            int count = int.Parse(hf_start.Value);

            hf_start.Value = (count + 5).ToString();
            if (string.IsNullOrEmpty(hf_asc.Value))
                hf_asc.Value = "DESC";
            Sort("d");
        }
    }
}
