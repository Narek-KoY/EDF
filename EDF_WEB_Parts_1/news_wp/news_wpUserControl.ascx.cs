using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.UI;
using Microsoft.SharePoint;

namespace EDF_WEB_Parts_1.news_wp
{
    public partial class news_wpUserControl : UserControl
    {
        static DateTime update_time = DateTime.Now.Date;
        static bool oneUpdate = true;

        protected void Page_Load(object sender, EventArgs e)
        {
 //           SPWeb web = SPContext.Current.Web;

//            SPSecurity.RunWithElevatedPrivileges(delegate()
//            {

//                string data = string.Format(@"<table cellspacing=""0"" border=""0"" style=""border-collapse:collapse;"">");

//                foreach (SPListItem li in SPContext.Current.Web.Lists["intranet_news"].Items)
//                {
//                    data += string.Format(@"            
//                                            <tr>
//			                                    <td>
//                                                    <li><span>{0}/ </span><a href='{1}'>{2}</a></li>              
//                                                </td>
//	    	                                </tr>", li["date"], li["link"], li["Title"]);
//                }

//                data += string.Format("</table>");

//                new_block_smart_new.InnerHtml = data;
//            });


            //if (oneUpdate || update_time.AddDays(1).Date < DateTime.Now.Date)
            //{
            //    oneUpdate = false;
            //    update_time = DateTime.Now.Date;
            //    Thread th = new Thread(() => { updateNewsList(web); });
            //    th.Start();
            //}


        }
        //public string getHTML()
        //{
        //    string source = "";

        //    SPSecurity.RunWithElevatedPrivileges(delegate() {

        //        WebRequest req = HttpWebRequest.Create("http://intranet.com.intraorange/_vti_bin/Orange.IG.Core/MosaicService.svc/GetData?json=%7B%22DataRetrieverName%22%3A%22HomepageNewsDataRetriever%22%2C%22DataPresenterName%22%3A%22HomepageNewsDataPresenter%22%2C%22Sorting%22%3A%22date%22%2C%22Filter%22%3A%7B%22mediatype%22%3A%22all%22%7D%2C%22Count%22%3A120%2C%22Columns%22%3A3%2C%22CallingUrl%22%3A%22http%3A%2F%2Fintranet.com.intraorange%2Fen%2FPages%2Fnews.aspx%22%2C%22ContextualInfo%22%3A%22%22%7D");
        //        req.Method = "GET";


        //        using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
        //        {
        //            source = reader.ReadToEnd();
        //        }            
        //    });

        //    return source;
        //}

        //public void updateNewsList(SPWeb web)
        //{
        //    string data = getHTML(); // File.ReadAllText(@"C:\Users\Administrator\Desktop\aaa.htm");
        //    List<intranet_news> list = getNews(data.Replace(@"\", string.Empty));
        //    SPSecurity.RunWithElevatedPrivileges(delegate()
        //        {

        //            if (list.Count > 0)
        //            {
        //                web.AllowUnsafeUpdates = true;
        //                SPList listObj = web.Lists["intranet_news"];

        //                DeleteItemsCollection(listObj, web, listObj.Items);



        //                foreach (intranet_news newObj in list)
        //                {
        //                    SPListItem item = listObj.Items.Add();
        //                    item["Title"] = newObj.title;
        //                    item["link"] = newObj.link;
        //                    item["date"] = newObj.date;
        //                    item.Update();
        //                }

        //                web.AllowUnsafeUpdates = false;
        //            }
        //        });

        //}

        //public void DeleteItemsCollection(SPList list, SPWeb web, SPListItemCollection listitemCollection)
        //{
        //    StringBuilder deletebuilder = DeleteBatchCommand(list, listitemCollection);
        //    web.ProcessBatchData(deletebuilder.ToString());
        //}

        //public StringBuilder DeleteBatchCommand(SPList spList, SPListItemCollection listitemCollection)
        // {
        //     StringBuilder deletebuilder = new StringBuilder();
        //     deletebuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
        //     string command = "<Method><SetList Scope=\"Request\">" + spList.ID +
        //     "</SetList><SetVar Name=\"ID\">{0}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";

        //     foreach (SPListItem item in listitemCollection)
        //     {
        //         deletebuilder.Append(string.Format(command, item.ID.ToString()));
        //     }
        //     deletebuilder.Append("</Batch>");
        //     return deletebuilder;
        // }        

        //public List<intranet_news> getNews(string data)
        //{

        //    List<intranet_news> list = new List<intranet_news> { };

        //    int start_index = 0;

        //    for (int i = 0; i < 8; i++)
        //    {

        //        intranet_news news = new intranet_news();

        //        int start_a_href = data.IndexOf(@"<a href=""", start_index) + 9;

        //        int end_a_href = data.ToLower().IndexOf(@""">", start_a_href);

        //        news.link = data.Substring(start_a_href, end_a_href - start_a_href).Trim();


        //        int title_start = data.ToLower().IndexOf(@"<span class=""heading-4"">", end_a_href) + 24;

        //        int title_end = data.ToLower().IndexOf(@"</span>", title_start);

        //        news.title += data.Substring(title_start, title_end - title_start).Trim();


        //        int date_start = data.ToLower().IndexOf(@"<p class=""content-info"">", title_end) + 24;

        //        int date_end = data.ToLower().IndexOf(@"</p>", date_start);

        //        news.date += data.Substring(date_start, date_end - date_start).Trim().Replace("published on", "").Trim();

        //        start_index = date_end;

        //        list.Add(news);
        //    }


        //    return list;
        //}
    }

    //public class intranet_news
    //{
    //    public string title { get; set; }
    //    public string link { get; set; }
    //    public string date { get; set; }
    //}
}
