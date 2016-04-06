using System;
using System.Web.UI;

using EDF_CommonData;


namespace EDF_WEB_Parts_1.UserName
{
    public partial class UserNameUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = ADSP.CurrentUser.FullName;
        }
    }
}
