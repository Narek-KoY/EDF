using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web;


namespace EDF_CommonData
{
    public class Constants
    {
        private const string CONNECTIONSTRING_TEST = "Data Source=TEST-DC;Initial Catalog=EDF_DB;User Id=kbt_sql; Password=?3kSm!9Q;";
        private const string CONNECTIONSTRING = "Data Source=ORAM-SPSDB;Initial Catalog=EDF_DB;User Id=kbt_sql; Password=?3kSm!9Q;";

        public const string EmailHost = "smtp.ad.orangearmenia.am";
        public const string EmailFrom = "edf@ucom.am";
        public const string EmailCarpool = "edf-carpool@ucom.am";


        public static String GetConnectionString()
        {
            if (AD.Domain.Contains("pele"))
            {
                return CONNECTIONSTRING_TEST;
            }
            else
            {
                return CONNECTIONSTRING;
            }
        }
    }

    public class ADSP
    {
        public static EDF_SPUser CurrentUser
        {
            get
            {
                string name = SPContext.Current.Web.CurrentUser.LoginName;
                if (name.Contains("\\"))
                {
                    name = name.Split('\\')[1];
                }
                return AD.GetUserByLogin(name);
            }
        }

    }

    public class EDF_Request
    {
        public string Id { get; set; }

        public EDF_SPUser Autor { get; set; }

        public bool? State { get; set; }
    }

    public class ER
    {
        public static void GoToErrorPage(string ErrorMessage)
        {
            string Url = SPContext.Current.Web.Url + "/SitePages/ErrorPage.aspx";
            var cur = HttpContext.Current;
            string value = cur.Server.UrlEncode(ErrorMessage);
            HttpContext.Current.Response.Redirect(string.Format("{0}?msg={1}", Url, value));
        }

        public static string GetErrorMessage
        {
            get
            {
                string value;
                var cur = HttpContext.Current;
                if (HttpContext.Current.Request.QueryString["msg"] != null && HttpContext.Current.Request.QueryString["msg"].ToString() != "")
                {
                    value = cur.Server.UrlDecode(HttpContext.Current.Request.QueryString["msg"].ToString());
                }
                else
                {
                    value = "not Page";
                }
                return value;
            }
        }
    }
}
