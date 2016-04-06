using System;
using System.IO;
using System.Web.UI;
using EDF_CommonData;
using Microsoft.SharePoint;


namespace EDF_WEB_Parts_1.DatabaseAccessRequest
{
    public partial class DatabaseAccessRequestUserControl : UserControl
    {
        EDF_SPUser autor = ADSP.CurrentUser;
    //    string Request_ID;
        DateTime Start_date;
        DateTime End_date;
        bool d1, d2;
        string Autor_id;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!autor.IsCEO)
            {
                if (!autor.HasManager) ER.GoToErrorPage(autor.FullName + " has not manager");
                if (!autor.HasDirector) ER.GoToErrorPage(autor.FullName + " has not director");
            }
            Autor_id = autor.Login;
            UC.SearchBox(ta_description, "Write Description:");
            UC.SearchBox(ta_name, "Write name:");
            UC.SearchBox(ta_Department, "Write department:");
            UC.SearchBox(ta_Position, "Write position:");
            UC.SearchBox(ta_name2, "Write name:");
            UC.SearchBox(ta_organization, "Write organization:");
            UC.SearchBox(ta_country, "Write country:");
            UC.SearchBox(ta_assdep, "Write associated department:");
            UC.SearchBox(ta_team, "Write team:");

            autorImg.Src = autor.PictureUrl;

            if (!IsPostBack)
                Step_2_2.Visible = false;

            ScriptManager.RegisterStartupScript(
                       UpdatePanel1,
                       this.GetType(),
                       "MyAction",
                       "picker();",
                       true);
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            string ss = "0";
            try
            {
                #region UI
                bool val = true;
                if (rb_Self.Checked)
                    if (!ChPermanent.Checked)
                    {
                        val = (UC.RequiredVal(tb_start, ref val_start, "Select start date")
                            && UC.RequiredVal(tb_end, ref val_end, "Select end date"));
                    }
                    else val = true;
                else if (rb_Benef.Checked)
                {
                    if (rb_no_oam.Checked)
                    {
                        val = (UC.RequiredVal(tb_start, ref val_start, "Select start date")
                            && UC.RequiredVal(tb_end, ref val_end, "Select end date"));
                    }
                    else if (!ChPermanent.Checked)
                    {
                        val = (UC.RequiredVal(tb_start, ref val_start, "Select start date")
                               && UC.RequiredVal(tb_end, ref val_end, "Select end date"));
                    }



                    val = val && ((rb_oam.Checked
                                && UC.RequiredVal(ta_name, ref lb_val_name, "*")
                                && UC.RequiredVal(ta_Department, ref lb_val_dep, "*")
                                && UC.RequiredVal(ta_Position, ref lb_val_pos, "*"))
                                ||
                            (rb_no_oam.Checked
                                && UC.RequiredVal(ta_name2, ref lb_val_name_2, "*")
                                && UC.RequiredVal(ta_assdep, ref lb_val_ass_dep, "*")));


                }
                if (!val) return;

                if ((rb_Self.Checked && !ChPermanent.Checked) || (rb_Benef.Checked && rb_oam.Checked && !ChPermanent.Checked))
                {
                    d1 = DateTime.TryParse(tb_start.Text, out Start_date);
                    d2 = DateTime.TryParse(tb_end.Text, out End_date);

                    if (!(d1 && d2)) {return; }
                    else if (Start_date < DateTime.Now.AddDays(-1) || Start_date > End_date) {return; }
                }
                string Request_ID;
                if (!request.Add(5, Autor_id))
                    ER.GoToErrorPage("Can't add Database access request");

                Request_ID = request.GetId(Autor_id).ToString();
                string Requestor = rb_Self.Checked ? "Self" : "Beneficiary";
                string Equipment = null;
                if (cb_PC.Checked || cb_Laptop.Checked)
                    Equipment = (cb_PC.Checked ? "PC " : "") + (cb_Laptop.Checked ? " Laptop" : "");
                string Email;
                if (rb_internal_onli.Checked)
                    Email = "Internal only";
                else
                    if (rb_unrestricted.Checked)
                        Email = "unrestricted";
                    else
                        Email = "no e-mail";

                string InternetAccess = rb_full.Checked ? "Full" : (rb_no.Checked ? "No" : "Restricted (without socials websites)");


                string Description = ta_description.InnerText;
                string Attachment = null;

                ss = string.Empty;

                if (fu_atach.HasFile)              
                {
                    try
                    {
                        Attachment = Path.GetTempPath() + Guid.NewGuid().ToString() + "." + fu_atach.FileName.Split('.')[fu_atach.FileName.Split('.').Length - 1];                        
                        ss = "1";
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            fu_atach.SaveAs(Attachment);
                        });
                        ss = "2";
                        Attachment = PDF.SaveDARFileInSP(Attachment, Request_ID);
                        ss = "3";
                    }
                    catch (Exception ex) { Response.Write(string.Format("error {0}   msg: {1}", ss, ex.Message)); }
                }

                string AccessPeriodStart = null;
                string AccessPeriodEnd = null;
                string Beneficiary = null;
                if (rb_Self.Checked)
                {
                    if (!ChPermanent.Checked)
                    {
                        AccessPeriodStart = tb_start.Text;
                        AccessPeriodEnd = tb_end.Text;
                    }
                }
                else if (rb_Benef.Checked)
                {
                    if (rb_oam.Checked)
                    {
                        if (!ChPermanent.Checked)
                        {
                            AccessPeriodStart = tb_start.Text;
                            AccessPeriodEnd = tb_end.Text;
                        }
                    }
                    else
                    {
                        AccessPeriodStart = tb_start.Text;
                        AccessPeriodEnd = tb_end.Text;
                    }
                    Beneficiary = rb_oam.Checked ? "OAM user" : "Non OAM user";
                }

                OAMuser oam = new OAMuser();
                oam.Name = ta_name.InnerText;
                oam.Department = ta_Department.InnerText;
                oam.Position = ta_Position.InnerText;

                OAMuserNot noam = new OAMuserNot();
                noam.Name = ta_name2.InnerText;
                noam.Organization = ta_organization.InnerText;
                noam.Country = ta_country.InnerText;
                noam.Ass_Dep = ta_assdep.InnerText;
                noam.Team = ta_team.InnerText;
                noam.Intern = rb_intern_yes.Checked;

                #endregion
                ss = "1";
                if (!EDF.AddDAR(Request_ID, "0", autor.Department, Autor_id, Requestor, Equipment, Email, InternetAccess, "", Description, Attachment, AccessPeriodStart, AccessPeriodEnd, Beneficiary, oam, noam))
                    ER.GoToErrorPage("Can't add new Database access request");
                ss = "2";


                bool error = false;
                ss = "3";

                if (autor.IsCEO)
                {
                    string DARS = AD.DBRInformationSecurity.Login;
                    error = !Approve_reject.Add(DARS, Request_ID, "ViewDAR.aspx", "InformationSecurity");
                }
                else if (autor.IsDirector)
                {
                    ss = "31";
                    string DARS = AD.DBRInformationSecurity.Login;
                    error = !Approve_reject.Add(DARS, Request_ID, "ViewDAR.aspx", "InformationSecurity");
                }
                else if (autor.Manager.IsDirector)
                {
                    ss = "32";
                    error = !Approve_reject.Add(autor.Manager.Login, Request_ID, "ViewDAR.aspx", "director");
                }
                else
                {
                    ss = "33";
                    error = !Approve_reject.Add(autor.Manager.Login, Request_ID, "ViewDAR.aspx", "manager");
                }
                ss = "4";

                if (error)
                    Response.Write("Can't add new [Approve_reject] in Database");
                ss = "5";

                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/SuccesCreate.aspx");
                ss = "6";

            }
            catch (Exception ex) { Response.Write(string.Format("ERROR !!! -- {0} er: {1}", ss, ex.Message)); }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Dashboard.aspx");
        }

        protected void rb_Self_CheckedChanged(object sender, EventArgs e)
        {
            step_2.Visible = true;
            Step_2_2.Visible = !rb_Self.Checked;
            ChPermanent.Visible = rb_Self.Checked;
            data_div.Visible = ChPermanent.Visible = true;
        }

        protected void rb_oam_CheckedChanged(object sender, EventArgs e)
        {
            div_oam.Visible = rb_oam.Checked;
            div_oam_no.Visible = rb_no_oam.Checked;
            ChPermanent.Visible = rb_oam.Checked;
        }

        protected void ChPermanent_CheckedChanged(object sender, EventArgs e)
        {
            data_div.Visible = !ChPermanent.Checked;
        }
    }
}
