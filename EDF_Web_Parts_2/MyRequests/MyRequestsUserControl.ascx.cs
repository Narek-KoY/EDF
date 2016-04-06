using System;
using System.Data;
using System.Web.UI;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.MyRequests
{
    public partial class MyRequestsUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DrowTable(1, 5);
            }
            else
            {
                ulPend.InnerHtml = "";
                ulAppRej.InnerHtml = "";
            }
        }
        protected void BtnLoad_Click(object sender, EventArgs e)
        {
            int count = int.Parse(hf_start.Value);

            hf_start.Value = (count + 5).ToString();
            DrowTable(1, count + 5);
        }
        private void DrowTable(int start, int end)
        {
            try
            {
                DataTable tbPending = EDF.GetRequest(ADSP.CurrentUser.Login, start, end, false);

                string id, type, Autor_id, AddDate;
                for (int i = 1; i <= tbPending.Rows.Count; i++)
                {
                    id = tbPending.Rows[i - 1]["Id"].ToString();
                    type = tbPending.Rows[i - 1]["Type_id"].ToString();
                    Autor_id = tbPending.Rows[i - 1]["Autor_id"].ToString();
                    AddDate = tbPending.Rows[i - 1]["Add_date"].ToString();

                    string avatar = ADSP.CurrentUser.PictureUrl;
                    string typestring = EDF.GetRequestType(int.Parse(type)).Rows[0]["Name"].ToString();
                    string Image = EDF.GetRequestType(int.Parse(type)).Rows[0]["ImgUrlOrange"].ToString();
                    string SID = EDF.GetUppers(typestring) + " " + id;

                    string LinkPage = "";
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

                    ulPend.InnerHtml += string.Format(" <li style='cursor:pointer;' class='cont clearfix'> <a href='{4}'><div style='background:url({3}) no-repeat 28px;' class='my_img_logo fleft'><div class='fleft'><img class='avatar' src='{5}' /></div>" +
                        "</div><p class='text4 fleft'>{0}<br /><span>ID: {1}</span><br /><h9>{2}</h9></p><div class='timer fright'><img src='/_catalogs/masterpage/images/timer.png' /></div></a></li>",
                        typestring,
                        SID,
                        DateTime.Parse(AddDate).ToString("dd/MM/yyyy HH:mm:ss"),
                        Image,
                        Link,
                        avatar
                        );
                }
            }
            catch (Exception ex)
            {
                Response.Write(string.Format("ERROR - Pend EX:{0} - ERROR", ex.Message));
            }


            try
            {
                DataTable tbAppRej = EDF.GetRequest(ADSP.CurrentUser.Login, start, end, true);

                string id, type, Autor_id, AddDate;
                for (int i = 1; i <= tbAppRej.Rows.Count; i++)
                {
                    id = tbAppRej.Rows[i - 1]["Id"].ToString();
                    type = tbAppRej.Rows[i - 1]["Type_id"].ToString();
                    Autor_id = tbAppRej.Rows[i - 1]["Autor_id"].ToString();
                    AddDate = tbAppRej.Rows[i - 1]["Add_date"].ToString();
                    string state = tbAppRej.Rows[i - 1]["State"].ToString();

                    string avatar = ADSP.CurrentUser.PictureUrl;
                    string typestring = EDF.GetRequestType(int.Parse(type)).Rows[0]["Name"].ToString();
                    string Image = EDF.GetRequestType(int.Parse(type)).Rows[0]["ImgUrlOrange"].ToString();
                    string SID = EDF.GetUppers(typestring) + " " + id;

                    string LinkPage = "";
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


                    ulAppRej.InnerHtml += string.Format("<li onclick='location.href=&quot;{5}&quot;;' style='cursor:pointer;' class='cont clearfix'><a href='{5}'><div style='background:url({3}) no-repeat 28px;' class='my_img_logo fleft'><img class='avatar' src='{4}' />" +
                        "</div><p class='text4 fleft'>{0}<br /><span>ID: {1}</span><br /><h9>{2}</h9></p><div class='timer fright'><img src=" + (state == "True" ? "'/_catalogs/masterpage/images/ok.png'" : "'/_catalogs/masterpage/images/x.png'") + "/></div></a></li>",
                        typestring,
                        SID,
                        DateTime.Parse(AddDate).ToString("dd/MM/yyyy HH:mm:ss"),
                        Image,
                        avatar,
                        Link
                        );
                }
            }
            catch (Exception ex)
            {
                Response.Write(string.Format("ERROR - Past EX:{0} - ERROR", ex.Message));
            }

        }
    }
}
