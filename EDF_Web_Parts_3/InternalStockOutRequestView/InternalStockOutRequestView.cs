using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace EDF_Web_Parts_3.InternalStockOutRequestView
{
    [ToolboxItemAttribute(false)]
    public class InternalStockOutRequestView : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/EDF_Web_Parts_3/InternalStockOutRequestView/InternalStockOutRequestViewUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
