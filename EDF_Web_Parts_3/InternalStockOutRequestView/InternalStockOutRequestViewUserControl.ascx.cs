using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_3.InternalStockOutRequestView
{
    public partial class InternalStockOutRequestViewUserControl : UserControl
    {
        EDF_SPUser curentUser = ADSP.CurrentUser;

        int requestId;
        DataTable dtItems = new DataTable();


        protected void Page_Load(object sender, EventArgs e)
        {
            requestId = Convert.ToInt32(Request.QueryString["rid"].ToString());
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["rid"]))
            {
                StockOutRequestModel request = new StockOutRequestModel();
                request = StockOutRequestDAO.GetRequestById(requestId);

                drowViewMode(request);
                DrawComents(request.RequestId);
                DrowAPPROVE(request);

                LblType.Text = Request_type.GetTypeName(requestId.ToString());
                LabelName.Text = request.RequestorName;
                LabelDep.Text = request.Department;
                UserProf.HRef = @"http://intranet/SitePages/Person.aspx?accountname=FTA\" + request.RequestUser.Login;
                if (!string.IsNullOrEmpty(request.OrderNumber))
                    titleOrderNumber.Text = request.OrderNumber;
            }
        }

        #region Events

        protected void ComSend_Click(object sender, EventArgs e)
        {
            requestId = Convert.ToInt32(Request.QueryString["rid"].ToString());
            //StockOutRequestModel curentRequest = new StockOutRequestModel();
            //curentRequest = StockOutRequestDAO.GetRequestById(requestId);

            if (ComText.Value != "" && ComText.Value != "Write your comment:")
                Comments.Add(requestId.ToString(), ComText.Value, curentUser.Login, false);


            ComText.Value = "Write your comment:";
            DataTable tableCom = Comments.Get(requestId.ToString());

            DrawComents(requestId);

        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            requestId = Convert.ToInt32(Request.QueryString["rid"].ToString());
            StockOutRequestModel request = new StockOutRequestModel();
            request = StockOutRequestDAO.GetRequestById(requestId);


            using (MemoryStream ms = new MemoryStream())
            {

                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();


                //Full path to the Unicode Arial file
                string ARIALUNI_TFF = Path.Combine(@"C:\Windows\Fonts", "UH.TTF");

                //Create a base font object making sure to specify IDENTITY-H
                BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                var OrangeColor = new BaseColor(255, 102, 0);

                Font fontNormal = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);
                Font fontOrange = new Font(bf, 14, Font.NORMAL, OrangeColor);

                Chunk orangeTextChunk = new Chunk("Internal stock out request / Պահեստից դուրս գրման պահանջագիր", fontOrange);
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(SPContext.Current.Web.Url + this.ResolveUrl("~/_layouts/Images/edf/stockout.png"));
                gif.Alignment = 2;
                doc.Add(gif);


                doc.Add(new Paragraph(orangeTextChunk));
                doc.Add(new Paragraph(Environment.NewLine));

                PdfPTable table = new PdfPTable(2);
                table.HorizontalAlignment = 0;

                table.AddCell(new Phrase("Order number /Պահանջագրի համար", fontNormal));
                table.AddCell(new Phrase(request.OrderNumber, fontNormal));

                table.AddCell(new Phrase("Date / Ամսաթիվ", fontNormal));
                table.AddCell(new Phrase(request.FillingDate.ToShortDateString(), fontNormal));


                table.AddCell(new Phrase("Requestor’s name / Դիմողի անունը", fontNormal));
                table.AddCell(new Phrase(request.RequestorName, fontNormal));

                table.AddCell(new Phrase("Department and position / Բաժին և պաշտոն", fontNormal));
                table.AddCell(new Phrase(request.Department + " " + request.Position, fontNormal));

                table.AddCell(new Phrase("Cost center / Ծախսային կենտրոն", fontNormal));
                table.AddCell(new Phrase(request.CostCenter, fontNormal));



                doc.Add(table);
                doc.Add(new Paragraph(Environment.NewLine));

                table = new PdfPTable(4);
                table.AddCell(new Phrase("Item code / Ծածկագիր", fontNormal));
                table.AddCell(new Phrase("Description / Նկարագրություն ", fontNormal));
                table.AddCell(new Phrase("Quantity / Քանակ", fontNormal));
                table.AddCell(new Phrase("Purpose / Դուրս գրման նպատակ", fontNormal));

                foreach (var item in request.Items)
                {
                    table.AddCell(new Phrase(item.ItemCode, fontNormal));
                    table.AddCell(new Phrase(item.ItemDescription, fontNormal));
                    table.AddCell(new Phrase(item.Quantity.ToString(), fontNormal));
                    table.AddCell(new Phrase(request.Purpose.ToString(), fontNormal));

                }
                table.HorizontalAlignment = 0;
                float[] columnWidths = new float[] { 5f, 10f, 5f, 10f };
                table.SetWidths(columnWidths);
                doc.Add(table);


                doc.Add(new Paragraph(Environment.NewLine));
                table = new PdfPTable(2);

                table.AddCell(new Phrase("Comment", fontNormal));
                table.AddCell(new Phrase("Budgeted account", fontNormal));
                table.AddCell(new Phrase(request.Comments, fontNormal));
                table.AddCell(new Phrase(request.BudgetedAccount, fontNormal));
                table.HorizontalAlignment = 0;

                columnWidths = new float[] { 20f, 10f };
                table.SetWidths(columnWidths);
                doc.Add(table);

                Chunk signatre1 = new Chunk(string.Format("Բաց թողեց՝       ____________________    {0}", (request.RequestType == RequestType.Stationery) ? AD.AdminStockkeeper.AllUsers[0].FullName : ""), fontNormal);
                Chunk signatre2 = new Chunk("Ստացավ՝           ____________________    " + request.RequestorName, fontNormal);
                doc.Add(new Paragraph(Environment.NewLine));
                doc.Add(new Paragraph(Environment.NewLine));
                doc.Add(new Paragraph(signatre1));
                doc.Add(new Paragraph(Environment.NewLine));
                doc.Add(new Paragraph(Environment.NewLine));
                doc.Add(new Paragraph(Environment.NewLine));
                doc.Add(new Paragraph(signatre2));

                doc.Close();
                writer.Close();
                Response.ContentType = "pdf/application";
                Response.AddHeader("content-disposition",
                "attachment;filename=Internal stock out.pdf");
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);

            }
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequest.aspx?rid=" + Request.QueryString["rid"].ToString());
        }

        protected void BtnApprove_Click(object sender, EventArgs e)
        {
            StockOutRequestModel request = new StockOutRequestModel();
            request = StockOutRequestDAO.GetRequestById(requestId);

            string msgApproval = string.Format("<b>{0}’s</b> Stock Out Request (ID: SOR {1}) is <b>submitted to you</b> for approval", request.RequestorName, request.RequestId.ToString());

            string msgProvide = string.Format("<b>{0}’s</b> Stock Out Request (ID: SOR {1}) is <b>submitted to you</b> for provided", request.RequestorName, request.RequestId.ToString());

            string msgApproved = string.Format("<b>Your</b> Stock Out Request (ID: SOR {0}) is <b>Approved</b>", request.RequestId.ToString());

            string msgProvided = string.Format("<b>Your</b> Stock Out Request (ID: SOR {0}) is <b>Provided</b>", request.RequestId.ToString());

            string url = SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + request.RequestId.ToString();

            if (request.RequestType == RequestType.Commercial)
            {
                if (curentUser.IsStockController)
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, AD.StockController.Login);
                    StockOutRequestDAO.UpdateRequestkeeper(request.RequestId.ToString(), (rbAdminStock.Checked) ? "adminstockkeeper" : "raomarsstockkeeper");

                    if (request.RequestUser.HasManager)
                    {
                        StockOutRequestDAO.RecieveRequestToUser(request.RequestId, request.RequestUser.Manager.Login, "directsupervisor");
                        Notificaion.Add(request.RequestUser.Manager.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                    }
                    else
                    {
                        StockOutRequestDAO.RecieveRequestToUser(request.RequestId, AD.SalesDirector.Login, "salesdirector");
                        Notificaion.Add(AD.SalesDirector.Manager.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                    }
                }
                else if (request.RequestUser.HasManager &&
                    (curentUser.Login == request.RequestUser.Manager.Login ||
                    (curentUser.ParentReplacement.Count > 0 && curentUser.ParentReplacement[0].Login == request.RequestUser.Manager.Login)))
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, request.RequestUser.Manager.Login);
                    StockOutRequestDAO.RecieveRequestToUser(request.RequestId, AD.SalesDirector.Login, "salesdirector");
                    Notificaion.Add(AD.SalesDirector.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                }
                else if (curentUser.IsSalesDirector)
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, AD.SalesDirector.Login);
                    SetOrderNumber(request.RequestId);
                    Notificaion.Add(request.RequestUser.Login, url, msgApproved, request.RequestUser.PictureUrl, 6);

                    if (request.RequestStockkeeper == "adminstockkeeper")
                    {
                        foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                        {
                            Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                        }
                    }
                    else
                    {
                        foreach (EDF_SPUser item in AD.RaomarsStockkeeper.AllUsers)
                        {
                            Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                        }
                    }
                }
            }
            else if (request.RequestType == RequestType.NonCommercial)
            {
                if (curentUser.IsStockController)
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, AD.StockController.Login);
                    StockOutRequestDAO.UpdateRequestkeeper(request.RequestId.ToString(), (rbAdminStock.Checked) ? "adminstockkeeper" : "raomarsstockkeeper");

                    if (request.RequestUser.HasManager)
                    {
                        StockOutRequestDAO.RecieveRequestToUser(request.RequestId, request.RequestUser.Manager.Login, "directsupervisor");
                        Notificaion.Add(request.RequestUser.Manager.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                    }
                    else
                    {
                        SetOrderNumber(request.RequestId);
                        Notificaion.Add(request.RequestUser.Login, url, msgApproved, request.RequestUser.PictureUrl, 6);
                        if (request.RequestStockkeeper == "adminstockkeeper")
                        {
                            foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                            {
                                Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                            }
                        }
                        else
                        {
                            foreach (EDF_SPUser item in AD.RaomarsStockkeeper.AllUsers)
                            {
                                Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                            }
                        }

                    }
                }
                else if (request.RequestUser.HasManager &&
                  (curentUser.Login == request.RequestUser.Manager.Login ||
                  (curentUser.ParentReplacement.Count > 0 && curentUser.ParentReplacement[0].Login == request.RequestUser.Manager.Login)))
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, request.RequestUser.Manager.Login);
                    SetOrderNumber(request.RequestId);

                    Notificaion.Add(request.RequestUser.Login, url, msgApproved, request.RequestUser.PictureUrl, 6);
                    if (request.RequestStockkeeper == "adminstockkeeper")
                    {
                        foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                        {
                            Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                        }
                    }
                    else
                    {
                        foreach (EDF_SPUser item in AD.RaomarsStockkeeper.AllUsers)
                        {
                            Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                        }
                    }
                }
            }
            else
            {
                if (curentUser.IsAdminStockkeeper)
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, "adminstockkeeper");
                    if (request.RequestUser.HasManager)
                    {
                        StockOutRequestDAO.RecieveRequestToUser(request.RequestId, request.RequestUser.Manager.Login, "directsupervisor");
                        Notificaion.Add(request.RequestUser.Manager.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                    }
                    else
                    {
                        StockOutRequestDAO.RecieveRequestToUser(request.RequestId, AD.AdministrativeSupervisor.Login, "salesdirector");
                        Notificaion.Add(AD.AdministrativeSupervisor.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                    }
                }
                else if (request.RequestUser.HasManager &&
                   (curentUser.Login == request.RequestUser.Manager.Login ||
                   (curentUser.ParentReplacement.Count > 0 && curentUser.ParentReplacement[0].Login == request.RequestUser.Manager.Login)))
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, request.RequestUser.Manager.Login);
                    StockOutRequestDAO.RecieveRequestToUser(request.RequestId, AD.AdministrativeSupervisor.Login, "administrativesupervisor");

                    Notificaion.Add(AD.AdministrativeSupervisor.Login, url, msgApproval, request.RequestUser.PictureUrl, 6);
                }
                else if (curentUser.IsAdministrativeSupervisor)
                {
                    StockOutRequestDAO.RecieveRequestApprove(request.RequestId, AD.AdministrativeSupervisor.Login);
                    SetOrderNumber(request.RequestId);
                    Notificaion.Add(request.RequestUser.Login, url, msgApproved, request.RequestUser.PictureUrl, 6);

                    foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                    {
                        Notificaion.Add(item.Login, url, msgProvide, request.RequestUser.PictureUrl, 6);
                    }
                }
            }
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
        }
        protected void BtnReject_Click(object sender, EventArgs e)
        {
            bool success = StockOutRequestDAO.RecieveRequestReject(requestId);
            StockOutRequestModel request = StockOutRequestDAO.GetRequestById(requestId);
            string msgReject = string.Format("<b>Your</b> Stock Out Request (ID: SOR {0}) is <b>Rejected</b> from <b>{1}</b>", requestId.ToString(), curentUser.FullName);
            if (success)
            {
                Notificaion.Add(request.RequestUser.Login, SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + requestId.ToString(), msgReject, request.RequestUser.PictureUrl, 6);
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
            else
            {
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/ErrorPage.aspx");
            }
        }

        protected void btnNotProvided_Click(object sender, EventArgs e)
        {
            bool success = StockOutRequestDAO.UpdateRequestProvided(requestId, false);
            if (success)
            {
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Rejected.aspx");
            }
            else
            {
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/ErrorPage.aspx");
            }
        }
        protected void btnProvided_Click(object sender, EventArgs e)
        {
            bool success = StockOutRequestDAO.UpdateRequestProvided(requestId, true);
            StockOutRequestModel request = new StockOutRequestModel();
            request = StockOutRequestDAO.GetRequestById(requestId);
            if (request.Purpose == Purpose.Temporary)
            {
                string url = SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + request.RequestId.ToString();
            }

            if (success)
            {
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Succes.aspx");
            }
            else
            {
                Response.Redirect(SPContext.Current.Web.Url + "/SitePages/ErrorPage.aspx");
            }
        }

        #endregion Events

        #region Methods

        private void DrowAPPROVE(StockOutRequestModel request)
        {
            if (curentUser.IsAdminStockkeeper && request.RequestType == RequestType.Stationery)
            {
                if (StockOutRequestDAO.GetRequestStatusByUser(requestId, "adminstockkeeper") == false)
                {
                    AppRejStatus.Visible = true;
                    AppRejStatus.Text = "<b>You already rejected.</b>";
                }
                else if (StockOutRequestDAO.GetRequestStatusByUser(requestId, "adminstockkeeper") == true)
                {
                    AppRejStatus.Visible = true;
                    AppRejStatus.Text = "<b>You already approved.</b>";

                }
                else if (StockOutRequestDAO.AllowUserApprove(requestId, "adminstockkeeper"))
                {
                    AppRejDiv.Visible = true;
                }
            }
            else
            {
                if (StockOutRequestDAO.GetRequestStatusByUser(requestId, curentUser.Login) == false)
                {
                    AppRejStatus.Visible = true;
                    AppRejStatus.Text = "<b>You already rejected.</b>";
                }
                else if (StockOutRequestDAO.GetRequestStatusByUser(requestId, curentUser.Login) == true)
                {
                    AppRejStatus.Visible = true;
                    AppRejStatus.Text = "<b>You already approved.</b>";

                }
                else if (StockOutRequestDAO.AllowUserApprove(requestId, curentUser.Login.ToString()))
                {
                    AppRejDiv.Visible = true;
                }

                #region //////////////////// replacement
                if (curentUser.ParentReplacement.Count > 0)
                {
                    if (StockOutRequestDAO.GetRequestStatusByUser(requestId, curentUser.ParentReplacement[0].Login) == false)
                    {
                        AppRejStatus.Visible = true;
                        AppRejStatus.Text = "<b>You already rejected.</b>";
                    }
                    else if (StockOutRequestDAO.GetRequestStatusByUser(requestId, curentUser.ParentReplacement[0].Login) == true)
                    {
                        AppRejStatus.Visible = true;
                        AppRejStatus.Text = "<b>You already approved.</b>";

                    }
                    else if (StockOutRequestDAO.AllowUserApprove(requestId, curentUser.ParentReplacement[0].Login.ToString()))
                    {
                        AppRejDiv.Visible = true;
                    }
                }
                #endregion
            }


            if (curentUser.Login == request.RequestUser.Login)
            {
                AppRejStatus.Visible = true;
                AppRejStatus.Text = "<b>It's your request</b>";
            }


            if (request.Approved == true && request.Provided == null)
            {
                AppRejDiv.Visible = false;
                if (request.RequestStockkeeper == "adminstockkeeper" && curentUser.IsAdminStockkeeper)
                {
                    ProvideDiv.Visible = true;
                }
                if (request.RequestStockkeeper == "raomarsstockkeeper" && curentUser.IsRaomarsStockkeeper)
                {
                    ProvideDiv.Visible = true;
                }
            }

            btnPrint.Visible = curentUser.IsStockController || curentUser.IsAdminStockkeeper || curentUser.IsRaomarsStockkeeper;
            btnEdit.Visible = curentUser.IsStockController || (request.RequestType == RequestType.Stationery && curentUser.IsAdminStockkeeper);

            if (curentUser.IsStockController && StockOutRequestDAO.AllowUserApprove(requestId, curentUser.Login.ToString()))
            {
                divStockkeeper.Visible = true;
            }
            else
            {
                divStockkeeper.Visible = false;
            }

            #region //////////////////// replacement
            if (curentUser.ParentReplacement.Count > 0)
            {
                if (curentUser.IsStockController && StockOutRequestDAO.AllowUserApprove(requestId, curentUser.ParentReplacement[0].Login.ToString()))
                {
                    divStockkeeper.Visible = true;
                }
                else
                {
                    divStockkeeper.Visible = false;
                }
            }
            #endregion


            if (request.Approved == false)
            {
                ulPending.Visible = false;
                ulProvide.Visible = false;
                DataTable rejectUserRow = StockOutRequestDAO.GetRejectedUser(request.RequestId);
                if (rejectUserRow.Rows[0]["RecievedUserId"].ToString() == "adminstockkeeper")
                {
                    ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                      "Admin Stock Keepers",
                   "",
                     StockOutRequestDAO.GetRequestAppDate(requestId, "adminstockkeeper").ToShortDateString(),
                      StockOutRequestDAO.GetRequestAppDate(requestId, "adminstockkeeper").ToShortTimeString(),
                    "/_catalogs/masterpage/images/x.png");
                }
                else
                {
                    EDF_SPUser rejectUser = AD.GetUserByLogin(rejectUserRow.Rows[0]["RecievedUserId"].ToString());
                    bool hasrep = rejectUser.HasReplacement;
                    if (hasrep)
                        rejectUser = rejectUser.Replacement;
                    ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      rejectUser.PictureUrl,
                      (hasrep)? "(R) "+rejectUser.FullName: rejectUser.FullName,
                      rejectUser.Department,
                      StockOutRequestDAO.GetRequestAppDate(requestId, rejectUser.Login).ToShortDateString(),
                       StockOutRequestDAO.GetRequestAppDate(requestId, rejectUser.Login).ToShortTimeString(),
                     "/_catalogs/masterpage/images/x.png");
                }
            }
            else
            {

                if (request.RequestType == RequestType.Commercial)
                {
                    DrowAPPROVECommercial(request);
                }
                else if (request.RequestType == RequestType.NonCommercial)
                {
                    DrowAPPROVENonCommercial(request);
                }
                else
                {
                    DrowAPPROVEStationery(request);
                }
            }
        }
        private void DrowAPPROVECommercial(StockOutRequestModel request)
        {
            bool ulApprovedByVisible = false;
            bool ulPendingVisible = false;

            #region StockController
            bool? StockControllerStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, AD.StockController.Login);

            if (StockControllerStatus == null)
            {
                ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                 AD.StockController.PictureUrl,
                 AD.StockController.FullName,
                 AD.StockController.Department,
                 "",
                 "",
                "/_catalogs/masterpage/images/timer.png");
                ulPendingVisible = true;
            }
            else
            {
                
                ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    AD.StockController.HasReplacement ? AD.StockController.Replacement.PictureUrl : AD.StockController.PictureUrl,
                   AD.StockController.HasReplacement ? "(R) "+ AD.StockController.Replacement.FullName: AD.StockController.FullName ,
                   AD.StockController.HasReplacement ? AD.StockController.Replacement.Department : AD.StockController.Department,
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.StockController.Login).ToShortDateString(),
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.StockController.Login).ToShortTimeString(),
                   (StockControllerStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                ulApprovedByVisible = true;
            }

            #endregion StockController

            #region DirectSupervisor

            if (request.RequestUser.HasManager)
            {
                bool? RequestUserManagerStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, request.RequestUser.Manager.Login);
                if (RequestUserManagerStatus == null)
                {
                    ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      request.RequestUser.Manager.PictureUrl,
                      request.RequestUser.Manager.FullName,
                      request.RequestUser.Manager.Department,
                      "",
                      "",
                     "/_catalogs/masterpage/images/timer.png");
                    ulPendingVisible = true;
                }
                else
                {
                    ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                        request.RequestUser.Manager.HasReplacement ? request.RequestUser.Manager.Replacement.PictureUrl : request.RequestUser.Manager.PictureUrl,
                        request.RequestUser.Manager.HasReplacement ? "(R) " + request.RequestUser.Manager.Replacement.FullName : request.RequestUser.Manager.FullName,
                        request.RequestUser.Manager.HasReplacement ? request.RequestUser.Manager.Replacement.Department :  request.RequestUser.Manager.Department,
                       StockOutRequestDAO.GetRequestAppDate(request.RequestId, request.RequestUser.Manager.Login).ToShortDateString(),
                       StockOutRequestDAO.GetRequestAppDate(request.RequestId, request.RequestUser.Manager.Login).ToShortTimeString(),
                     (RequestUserManagerStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                    ulApprovedByVisible = true;
                }
            }
            #endregion DirectSupervisor

            #region SalesDirector

            bool? SalesDirectorStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, AD.SalesDirector.Login);
            if (SalesDirectorStatus == null)
            {
                ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                   AD.SalesDirector.PictureUrl,
                   AD.SalesDirector.FullName,
                   AD.SalesDirector.Department,
                   "",
                   "",
                  "/_catalogs/masterpage/images/timer.png");
                ulPendingVisible = true;
            }
            else
            {
                ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    AD.SalesDirector.HasReplacement ? AD.SalesDirector.Replacement.PictureUrl :  AD.SalesDirector.PictureUrl,
                    AD.SalesDirector.HasReplacement ? "(R) " +  AD.SalesDirector.Replacement.FullName :  AD.SalesDirector.FullName,
                    AD.SalesDirector.HasReplacement ? AD.SalesDirector.Replacement.Department :  AD.SalesDirector.Department,
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.SalesDirector.Login).ToShortDateString(),
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.SalesDirector.Login).ToShortTimeString(),
                  (SalesDirectorStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                ulApprovedByVisible = true;
            }

            #endregion SalesDirector

            #region Stockkeeper

            if (!string.IsNullOrEmpty(request.RequestStockkeeper))
            {
                if (request.Provided == null)
                {
                    ulProvide.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                      (request.RequestStockkeeper == "adminstockkeeper") ? "Admin Stock Keepers" : "RAOMARS Stock Keepers",
                        "",
                       "",
                       "",
                      "/_catalogs/masterpage/images/timer.png");
                }
                else
                {
                    ulProvide.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                  SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                  (request.RequestStockkeeper == "adminstockkeeper") ? "Admin Stock Keepers" : "RAOMARS Stock Keepers",
                  "",
                 "",
                 "",
                  (request.Provided == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                }
            }
            #endregion Stockkeeper

            ulApprovedBy.Visible = ulApprovedByVisible;
            ulPending.Visible = ulPendingVisible;

        }
        private void DrowAPPROVENonCommercial(StockOutRequestModel request)
        {
            bool ulApprovedByVisible = false;
            bool ulPendingVisible = false;
            #region StockController
            bool? StockControllerStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, AD.StockController.Login);

            if (StockControllerStatus == null)
            {
                ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                 AD.StockController.PictureUrl,
                 AD.StockController.FullName,
                 AD.StockController.Department,
                 "",
                 "",
                "/_catalogs/masterpage/images/timer.png");
                ulPendingVisible = true;
            }
            else
            {
                ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    AD.StockController.HasReplacement ? AD.StockController.Replacement.PictureUrl : AD.StockController.PictureUrl,
                    AD.StockController.HasReplacement ? "(R) " + AD.StockController.Replacement.FullName : AD.StockController.FullName,
                    AD.StockController.HasReplacement ? AD.StockController.Replacement.Department : AD.StockController.Department,
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.StockController.Login).ToShortDateString(),
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.StockController.Login).ToShortTimeString(),
                   (StockControllerStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                ulApprovedByVisible = true;
            }

            #endregion StockController

            #region DirectSupervisor

            if (request.RequestUser.HasManager)
            {
                bool? RequestUserManagerStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, request.RequestUser.Manager.Login);
                if (RequestUserManagerStatus == null)
                {
                    ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      request.RequestUser.Manager.PictureUrl,
                      request.RequestUser.Manager.FullName,
                      request.RequestUser.Manager.Department,
                      "",
                      "",
                     "/_catalogs/masterpage/images/timer.png");
                    ulPendingVisible = true;
                }
                else
                {
                    ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                        request.RequestUser.Manager.HasReplacement ? request.RequestUser.Manager.Replacement.PictureUrl : request.RequestUser.Manager.PictureUrl,
                        request.RequestUser.Manager.HasReplacement ? "(R) " + request.RequestUser.Manager.Replacement.FullName : request.RequestUser.Manager.FullName,
                        request.RequestUser.Manager.HasReplacement ? request.RequestUser.Manager.Replacement.Department : request.RequestUser.Manager.Department,
                       StockOutRequestDAO.GetRequestAppDate(request.RequestId, request.RequestUser.Manager.Login).ToShortDateString(),
                       StockOutRequestDAO.GetRequestAppDate(request.RequestId, request.RequestUser.Manager.Login).ToShortTimeString(),
                     (RequestUserManagerStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                    ulApprovedByVisible = true;
                }
            }
            #endregion DirectSupervisor

            #region Stockkeeper

            if (!string.IsNullOrEmpty(request.RequestStockkeeper))
            {
                if (request.Provided == null)
                {
                    ulProvide.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span>  </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                         (request.RequestStockkeeper == "adminstockkeeper") ? "Admin Stock Keepers" : "RAOMARS Stock Keepers",
                        "",
                       "",
                       "",
                      "/_catalogs/masterpage/images/timer.png");
                }
                else
                {
                    ulProvide.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span>  </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                     (request.RequestStockkeeper == "adminstockkeeper") ? "Admin Stock Keepers" : "RAOMARS Stock Keepers",
                    "",
                   "",
                   "",
                    (request.Provided == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                }
            }
            #endregion Stockkeeper

            ulApprovedBy.Visible = ulApprovedByVisible;
            ulPending.Visible = ulPendingVisible;
        }
        private void DrowAPPROVEStationery(StockOutRequestModel request)
        {
            bool ulApprovedByVisible = false;
            bool ulPendingVisible = false;

            #region Stockkeeper
            bool? AdminStockKeeper = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, "adminstockkeeper");
            if (AdminStockKeeper == null)
            {
                ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span>  </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                 SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                "Admin Stock Keepers",
                 "",
                 "",
                 "",
                "/_catalogs/masterpage/images/timer.png");
                ulPendingVisible = true;
            }
            else
            {
                ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span>  </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                  SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                  "Admin Stock Keepers",
                  "",

                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, "adminstockkeeper").ToShortDateString(),
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, "adminstockkeeper").ToShortTimeString(),
                   (AdminStockKeeper == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                ulApprovedByVisible = true;
            }

            #endregion Stockkeeper

            #region DirectSupervisor

            if (request.RequestUser.HasManager)
            {
                bool? RequestUserManagerStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, request.RequestUser.Manager.Login);
                if (RequestUserManagerStatus == null)
                {
                    ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      request.RequestUser.Manager.PictureUrl,
                      request.RequestUser.Manager.FullName,
                      request.RequestUser.Manager.Department,
                      "",
                      "",
                     "/_catalogs/masterpage/images/timer.png");
                    ulPendingVisible = true;
                }
                else
                {
                    ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                        request.RequestUser.Manager.HasReplacement ? request.RequestUser.Manager.Replacement.PictureUrl : request.RequestUser.Manager.PictureUrl,
                        request.RequestUser.Manager.HasReplacement ? "(R) " + request.RequestUser.Manager.Replacement.FullName : request.RequestUser.Manager.FullName,
                        request.RequestUser.Manager.HasReplacement ? request.RequestUser.Manager.Replacement.Department : request.RequestUser.Manager.Department,
                       StockOutRequestDAO.GetRequestAppDate(request.RequestId, request.RequestUser.Manager.Login).ToShortDateString(),
                       StockOutRequestDAO.GetRequestAppDate(request.RequestId, request.RequestUser.Manager.Login).ToShortTimeString(),
                     (RequestUserManagerStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                    ulApprovedByVisible = true;
                }
            }
            #endregion DirectSupervisor

            #region AdministrativeSupervisor
            bool? AdministrativeSupervisorStatus = StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, AD.AdministrativeSupervisor.Login);

            if (AdministrativeSupervisorStatus == null)
            {
                ulPending.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                   AD.AdministrativeSupervisor.PictureUrl,
                   AD.AdministrativeSupervisor.FullName,
                   AD.AdministrativeSupervisor.Department,
                   "",
                   "",
                  "/_catalogs/masterpage/images/timer.png");
                ulPendingVisible = true;
            }
            else
            {
                ulApprovedBy.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> from </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    AD.AdministrativeSupervisor.HasReplacement ? AD.AdministrativeSupervisor.Replacement.PictureUrl :   AD.AdministrativeSupervisor.PictureUrl,
                    AD.AdministrativeSupervisor.HasReplacement ? "(R) " + AD.AdministrativeSupervisor.Replacement.FullName : AD.AdministrativeSupervisor.FullName,
                    AD.AdministrativeSupervisor.HasReplacement ? AD.AdministrativeSupervisor.Replacement.Department : AD.AdministrativeSupervisor.Department,
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.AdministrativeSupervisor.Login).ToShortDateString(),
                   StockOutRequestDAO.GetRequestAppDate(request.RequestId, AD.AdministrativeSupervisor.Login).ToShortTimeString(),
                   (AdministrativeSupervisorStatus == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                ulApprovedByVisible = true;
            }

            #endregion AdministrativeSupervisor

            #region Stockkeeper

            if (!string.IsNullOrEmpty(request.RequestStockkeeper))
            {
                if (request.Provided == null)
                {
                    ulProvide.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span>  </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                      SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                         (request.RequestStockkeeper == "adminstockkeeper") ? "Admin Stock Keepers" : "RAOMARS Stock Keepers",
                        "",
                       "",
                       "",
                      "/_catalogs/masterpage/images/timer.png");
                }
                else
                {
                    ulProvide.InnerHtml += string.Format("<li style='cursor:default'><table style='margin-top: 20px;margin-bottom: 20px;'><tr><td><div class='fleft'><img class='avatar' src='{0}' /></div></td><td><p class='text4 width fleft'>{1}<span> </span>{2} <span></span><br /><h9>{3}  <span>{4}</span></h9></p></td><td><img style='width: 35px;' src='{5}'/></td></tr></table></li>",
                    SPContext.Current.Web.Url + "/_catalogs/masterpage/images/default_group.jpg",
                     (request.RequestStockkeeper == "adminstockkeeper") ? "Admin Stock Keepers" : "RAOMARS Stock Keepers",
                    "",
                   "",
                   "",
                    (request.Provided == true) ? "/_catalogs/masterpage/images/ok.png" : "/_catalogs/masterpage/images/x.png");
                }
            }
            #endregion Stockkeeper

            ulApprovedBy.Visible = ulApprovedByVisible;
            ulPending.Visible = ulPendingVisible;
        }



        private void drowViewMode(StockOutRequestModel request)
        {
            autorImg.Src = request.RequestUser.PictureUrl;
            switch (request.RequestType.ToString())
            {
                case "Commercial": rbCommercial.Checked = true; break;
                case "NonCommercial": rbnonCommercial.Checked = true; break;
                case "Stationery": rbStationery.Checked = true; break;
            }

            txtCostCenter.InnerText += " " + request.CostCenter;
            if (request.RequestType != RequestType.Stationery)
            {
                switch (request.Purpose.ToString())
                {
                    case "Permanent": rbpermanent.Checked = true; break;
                    case "Temporary": rbTemporary.Checked = true;
                        SpanDueDate.Visible = true;
                        SpanDueDate.InnerText += " " + request.DueDate.ToShortDateString();
                        break;
                    case "Other": rbOther.Checked = true; txtOtherPurpose.Visible = true; txtOtherPurpose.Text = request.OtherPurpose; break;
                }
            }
            else
            {
                liPurpose.Visible = liBudgeted.Visible = false;
            }
            txtBudgetedaccount.Text = request.BudgetedAccount;
            txtComments.InnerText = request.Comments;
            #region bindItemsGrid
            dtItems.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Description", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Quantity", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Unit", typeof(string)));

            foreach (StockOutRequestItemsModel item in request.Items)
            {
                DataRow dr = null;
                dr = dtItems.NewRow();
                dr["ItemCode"] = item.ItemCode;
                dr["Description"] = item.ItemDescription;
                dr["Quantity"] = item.Quantity;
                dr["Unit"] = item.Unit;
                dtItems.Rows.Add(dr);
            }
            int existItemCount = dtItems.Rows.Count;
            if (existItemCount < 5)
            {
                for (int i = existItemCount; i < 5; i++)
                {
                    DataRow dr = null;
                    dr = dtItems.NewRow();
                    dr["ItemCode"] = string.Empty;
                    dr["Description"] = string.Empty;
                    dr["Quantity"] = string.Empty;
                    dr["Unit"] = string.Empty;
                    dtItems.Rows.Add(dr);
                }
            }
            dgItemsView.DataSource = dtItems;
            dgItemsView.DataBind();
            for (int i = 0; i < dtItems.Rows.Count; i++)
            {
                Label LabelItemCode = (Label)dgItemsView.Rows[i].Cells[0].FindControl("lblItemCode");
                Label LabelItemDesc = (Label)dgItemsView.Rows[i].Cells[1].FindControl("lblDescription");
                Label LabelItemQnty = (Label)dgItemsView.Rows[i].Cells[2].FindControl("lblQuantity");
                Label LabelItemUOM = (Label)dgItemsView.Rows[i].Cells[3].FindControl("lblUnit");

                LabelItemCode.Text = dtItems.Rows[i]["ItemCode"].ToString();
                LabelItemDesc.Text = dtItems.Rows[i]["Description"].ToString();
                LabelItemQnty.Text = dtItems.Rows[i]["Quantity"].ToString();
                LabelItemUOM.Text = dtItems.Rows[i]["Unit"].ToString();
            }
            #endregion bindItemsGrid
        }

        private void DrawComents(int request)
        {
            DataTable table = Comments.Get(request.ToString());
            divCom.InnerHtml = string.Empty;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string t1 = table.Rows[i]["MSG"].ToString();
                string t2 = DateTime.Parse(table.Rows[i]["Add_date"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).ToString().ToString();
                string t8 = ((DateTime)table.Rows[i]["Add_date"]).ToShortTimeString();
                string t3 = table.Rows[i]["User_ID"].ToString();
                string t4 = table.Rows[i]["see"].ToString();
                EDF_SPUser ComAutor = AD.GetUserBySPLogin(t3);
                string t5 = ComAutor.FullName;
                string t6 = ComAutor.PictureUrl;
                string t7 = ComAutor.Department;

                divCom.InnerHtml += string.Format(
                "<ul class='margin'>" +
                    "<li class='cont cont1 clearfix' style='cursor: default;'><div class='fleft'><img class='avatar' src='{1}' /></div><p class='text4 width fleft'>{2}<span> from </span>{3} <span>wrote:</span><br /><h9>{4} | <span>{5}</span></h9></p></li>" +
                    "<li class='cont cont1 clearfix' style='cursor: default;'><p class='text4 fleft'><span>{7}</span></p></li>" +
                "</ul>",
                "#", t6, t5, t7, t2, t8, "#", t1
                );
            }
            comImg.Src = curentUser.PictureUrl;
        }
        private void SetOrderNumber(int requestId)
        {
            StockOutRequestModel model = new StockOutRequestModel();
            model = StockOutRequestDAO.GetRequestById(requestId);

            int existOrdersCount = StockOutRequestDAO.GetExistOrdersCount(model.RequestType.ToString());
            string orderNumber = string.Empty;
            if (string.IsNullOrEmpty(model.OrderNumber) && existOrdersCount != -1)
            {
                orderNumber += (existOrdersCount + 1).ToString();
                orderNumber = orderNumber.PadLeft(5, '0');

                orderNumber = DateTime.Now.Year.ToString().Substring(2, 2) + orderNumber;
                switch (model.RequestType.ToString())
                {
                    case "Commercial": orderNumber = "CL" + orderNumber; break;
                    case "NonCommercial": orderNumber = "NC" + orderNumber; break;
                    case "Stationery": orderNumber = "ST" + orderNumber; break;
                }
                StockOutRequestDAO.UpdateOrderNumber(requestId, orderNumber);
            }
        }

        #endregion Methods
    }
}
