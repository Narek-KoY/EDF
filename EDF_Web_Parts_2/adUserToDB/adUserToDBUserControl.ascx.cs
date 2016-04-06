using System;
using System.Web.UI;
using EDF_CommonData;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_2.adUserToDB
{
    public partial class adUserToDBUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void bt_add_click(object sender, EventArgs e)
        {
            if ( string.IsNullOrEmpty(up_user.CommaSeparatedAccounts))
            {
                lb_date_valid.Text = "* This value is required";
                return;
            }
            string userName = up_user.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            EDF_SPUser user = AD.GetUserBySPLogin(userName);



            if (!EDF.addUserToDB(user))
            {
                ER.GoToErrorPage("Can`t add user to Data base");
            }
            else
            {
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            }
        }
    }
}
