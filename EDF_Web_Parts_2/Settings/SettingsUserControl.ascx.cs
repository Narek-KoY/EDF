using System;
using System.Web.UI;

using EDF_CommonData;

namespace EDF_Web_Parts_2.Settings
{
    public partial class SettingsUserControl : UserControl
    {
        EDF_SPUser cur = ADSP.CurrentUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ADSP.CurrentUser.HasReplacement && !IsPostBack)
            {
                try
                {
                    up.CommaSeparatedAccounts = cur.Replacement.Login;
                    up.Validate();
                    tb_start.Value = cur.ReplacementStart;
                    tb_end.Value = cur.ReplacementEnd;
                }
                catch { }
            }
        }

        protected void bt_Save_click(object sender, EventArgs e)
        {
            if (up.CommaSeparatedAccounts == "")
            {
                lb_date_valid.Text = "*";
                return;
            }
            string rep = up.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            rep = AD.GetUserBySPLogin(rep).Login;

            DateTime Start, End;
            bool IsStart = DateTime.TryParse(tb_start.Value, out Start);
            bool IsEnd = DateTime.TryParse(tb_end.Value, out End);

            if (rep != "" && IsStart && IsEnd && Start <= End)
            {
                ADSP.CurrentUser.SetReplacement(rep, Start, End);
                lb_date_valid.Text = string.Empty;
                returnValue.Text = "Record Inserted Successfully";
            }
            else
            {
                if (Start > End || !IsStart || !IsEnd)
                    lb_date_valid.Text = "*";
            }
        }

        protected void bt_Remove_click(object sender, EventArgs e)
        {
            cur.RemoveReplacement();
            try
            {
                up.CommaSeparatedAccounts = string.Empty;
                up.Validate();
                tb_start.Value = "Select start date";
                tb_end.Value = "Select end date";

                returnValue.Text = "Record Removed Successfully";
            }
            catch { }
        }
    }
}
