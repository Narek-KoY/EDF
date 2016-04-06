using System;
using System.Web.UI;
using EDF_CommonData;


namespace EDF_Web_Parts_2.SetReplacement
{
    public partial class SetReplacementUserControl : UserControl
    {
        EDF_SPUser cur = ADSP.CurrentUser;
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void bt_Save_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(up_user.CommaSeparatedAccounts))
            {
                lb_date_valid.Text = "*";
                return;
            }
            string UserName = up_user.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            EDF_SPUser user = AD.GetUserBySPLogin(UserName);

            if (string.IsNullOrEmpty(up_rep.CommaSeparatedAccounts))
            {
                lb_date_valid.Text = "*";
                return;
            }
            string rep = up_rep.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            rep = AD.GetUserBySPLogin(rep).Login;

            DateTime Start, End;
            bool IsStart = DateTime.TryParse(tb_start.Value, out Start);
            bool IsEnd = DateTime.TryParse(tb_end.Value, out End);

            if (rep != "" && IsStart && IsEnd && Start <= End)
            {
                user.SetReplacement(rep, Start, End);
                lb_date_valid.Text = string.Empty;
                string script = @"<script language=""javascript"" type=""text/javascript"">alert('" + "Record Inserted Successfully" + "');</script>";
                Response.Write(script);
            }
            else
            {
                if (Start > End || !IsStart || !IsEnd)
                    lb_date_valid.Text = "*";
            }
        }

        protected void bt_Remove_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(up_user.CommaSeparatedAccounts))
            {
                lb_date_valid.Text = "*";
                return;
            }
            string UserName = up_user.CommaSeparatedAccounts.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            EDF_SPUser user = AD.GetUserBySPLogin(UserName);
            user.RemoveReplacement();
            try
            {
                up_rep.CommaSeparatedAccounts = string.Empty;
                up_rep.Validate();
                tb_start.Value = "Select start date";
                tb_end.Value = "Select end date";
            }
            catch { }
        }
    }
}
