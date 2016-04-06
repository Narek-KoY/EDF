using System;
using System.Web.UI;
using EDF_CommonData;

namespace EDF_WEB_Parts_1.ErrorPage
{
    public partial class ErrorPageUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ul1.InnerHtml = string.Empty;
            DrawError(ER.GetErrorMessage);            
        }

        void DrawError(string msg)
        {
            ul1.InnerHtml +=String.Format
                (
                    "<li class='cont clearfix' style='padding:10px 10px 10px 10px'>" +
                        "<a href='{0}'>" +                        
                            "<p class='text3 fleft' style='width:100%'>{1}</p>" +                          
                        "</a>" +
                    "</li>",
                    "#",
                    msg
                );
        }
    }
}
