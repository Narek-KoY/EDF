using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using EDF_CommonData;
using iTextSharp.text;
using Microsoft.SharePoint;

namespace EDF_Web_Parts_3.InternalStockOutRequest
{
    public partial class InternalStockOutRequestUserControl : UserControl
    {
        int requestId;
        DateTime dateTimeDueDate;
        DataTable dtItems = new DataTable();
        EDF_SPUser curentUser = ADSP.CurrentUser;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            autorImg.Src = curentUser.PictureUrl;
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
                {

                    requestId = Convert.ToInt32(Request.QueryString["rid"].ToString());
                    drowEditMode(requestId);
                }
                else
                {
                    FirstLoadItemsGrid();
                }
            }
        }

        #region Events

        protected void dgItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            SetRowData();
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                int rowIndex = Convert.ToInt32(e.RowIndex);

                if (dt.Rows.Count > 1)
                {
                    dt.Rows.Remove(dt.Rows[rowIndex]);

                    ViewState["CurrentTable"] = dt;
                    dgItems.DataSource = dt;
                    dgItems.DataBind();

                    SetPreviousData();
                }
            }
        }

        protected void OnCheckChanged(object sender, EventArgs e)
        {
            txtDueDate.Visible = valDateTimeControl1.Visible = btnRecievedBack.Visible = rbTemporary.Checked;
            txtOtherPurpose.Visible = rbOther.Checked;

        }

        protected void Type_OnCheckChanged(object sender, EventArgs e)
        {
            liPurpose.Visible = liBudgeted.Visible = !rbStationery.Checked;
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/Dashboard.aspx");
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                ViewState["ButtonSubmit"] = false;
              
                int newRequestId = SaveRequest();
                if (newRequestId != 0)
                {
                    Response.Redirect(SPContext.Current.Web.Url + "/SitePages/SuccesCreate.aspx");
                }
                else
                {
                    Response.Redirect( SPContext.Current.Web.Url + "/SitePages/ErrorPage.aspx");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + Request.QueryString["rid"].ToString());
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                btnUpdate.Enabled = false;
                if (UpdateRequest(Convert.ToInt32(Request.QueryString["rid"].ToString())))
                {
                    Response.Redirect(SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + Request.QueryString["rid"].ToString());
                }
                else
                {
                    Response.Redirect(SPContext.Current.Web.Url + "/SitePages/ErrorPage.aspx");
                }
            }
        }
        protected void btnRecievedBack_Click(object sender, EventArgs e)
        {
          bool recievedBack =  StockOutRequestDAO.UpdateRecievedBack(Convert.ToInt32(Request.QueryString["rid"].ToString()),true);
          if (recievedBack)
              btnRecievedBack.Enabled = false;
        }

        #endregion Events

        #region Methods

        private bool UpdateRequest(int requestId)
        {
            StockOutRequestModel request = new StockOutRequestModel();
            List<StockOutRequestItemsModel> requestItems = new List<StockOutRequestItemsModel>();

            StockOutRequestModel oldRequest = new StockOutRequestModel();
            oldRequest = StockOutRequestDAO.GetRequestById(requestId);

            if (rbCommercial.Checked)
                request.RequestType = RequestType.Commercial;
            else if (rbNonCommercial.Checked)
                request.RequestType = RequestType.NonCommercial;
            else
                request.RequestType = RequestType.Stationery;
            request.CostCenter = txtCostCenter.Text.Trim();

            if (!rbStationery.Checked)
            {
                if (rbPermanent.Checked)
                {
                    request.Purpose = Purpose.Permanent;
                    request.DueDate = DateTime.Now;
                    request.OtherPurpose = string.Empty;
                }

                else if (rbOther.Checked)
                {
                    request.Purpose = Purpose.Other;
                    request.DueDate = DateTime.Now;
                    request.OtherPurpose = txtOtherPurpose.Text.Trim();
                }
                else
                {
                    request.Purpose = Purpose.Temporary;
                    DateTime.TryParseExact(txtDueDate.Text, "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeDueDate);
                    request.DueDate = dateTimeDueDate;
                    request.OtherPurpose = string.Empty;
                }
                request.BudgetedAccount = txtBudgetedaccount.Text.Trim();
            }
           
            request.Comments = txtComments.InnerText.Trim();
            SetRowData();
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];

            foreach (DataRow item in dtCurrentTable.Rows)
            {
                if (!string.IsNullOrEmpty(item["Description"].ToString()))
                {
                    StockOutRequestItemsModel curentItem = new StockOutRequestItemsModel();
                    curentItem.ItemId = string.IsNullOrEmpty(item["ItemId"].ToString()) ? 0 : Convert.ToInt32(item["ItemId"].ToString());
                    curentItem.RequestID = requestId;
                    curentItem.ItemCode = item["ItemCode"].ToString().Trim();
                    curentItem.ItemDescription = item["Description"].ToString().Trim();
                    curentItem.Quantity = Convert.ToInt32(item["Quantity"].ToString().Trim());
                    curentItem.Unit = item["Unit"].ToString().Trim();
                    requestItems.Add(curentItem);
                }
            }
            request.Items = requestItems;
            request.RequestId = requestId;
            bool successUpdate = StockOutRequestDAO.UpdateRequest(request);
            if (successUpdate)
            {
                if (oldRequest.RequestType != request.RequestType)
                {
                    StockOutRequestDAO.RemoveRecievedsRequest(requestId);
                    string msg1 = string.Format("<b>{0}’s</b> Stock Out Request (ID: SOR {1}) is <b>submitted to you</b> for approval", oldRequest.RequestorName, requestId.ToString());
                    if (request.RequestType == RequestType.Commercial || request.RequestType == RequestType.NonCommercial)
                    {
                        StockOutRequestDAO.RecieveRequestToUser(requestId, AD.StockController.Login, "stockcontroller");

                        Notificaion.Add(AD.StockController.Login, SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + requestId.ToString(), msg1, oldRequest.RequestUser.PictureUrl, 6);
                    }
                    else
                    {
                        StockOutRequestDAO.UpdateRequestkeeper(requestId.ToString(), "adminstockkeeper");
                        StockOutRequestDAO.RecieveRequestToUser(requestId, "adminstockkeeper", "adminstockkeeper");

                        foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                        {
                            Notificaion.Add(item.Login, SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + requestId.ToString(), msg1, oldRequest.RequestUser.PictureUrl, 6);
                        }
                    }

                }
                string msg = string.Format("<b>Your</b> Stock Out Request (ID: SOR {0}) has been edited from <b>{1}</b>", request.RequestId.ToString(), curentUser.FullName);
                Notificaion.Add(oldRequest.RequestUser.Login, SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + request.RequestId.ToString(), msg, oldRequest.RequestUser.PictureUrl, 6);
            }
            return successUpdate;
        }

        private void drowEditMode(int requestId)
        {
            StockOutRequestModel request = new StockOutRequestModel();
            request = StockOutRequestDAO.GetRequestById(requestId);

            if (curentUser.IsStockController || (request.RequestType == RequestType.Stationery && curentUser.IsAdminStockkeeper))
            {
                switch (request.RequestType.ToString())
                {
                    case "Commercial": rbCommercial.Checked = true; break;
                    case "NonCommercial": rbNonCommercial.Checked = true; break;
                    case "Stationery": rbStationery.Checked = true; break;
                }

                txtCostCenter.Text = request.CostCenter;
                if (request.RequestType != RequestType.Stationery)
                {
                    switch (request.Purpose.ToString())
                    {
                        case "Permanent": rbPermanent.Checked = true; break;
                        case "Temporary": rbTemporary.Checked = true;
                            txtDueDate.Visible = true;
                            txtDueDate.Text = request.DueDate.ToShortDateString();
                            valDateTimeControl1.Visible = true;
                            break;
                        case "Other": rbOther.Checked = true;
                                      txtOtherPurpose.Visible = true;
                                      txtOtherPurpose.Text = request.OtherPurpose;
                                      break;
                    }
                    txtBudgetedaccount.Text = request.BudgetedAccount;
                }
                else
                {
                   // rbPermanent.Enabled = rbTemporary.Enabled = rbOther.Enabled = txtOtherPurpose.Enabled = txtDueDate.Enabled = txtBudgetedaccount.Enabled = false;
                    liPurpose.Visible = liBudgeted.Visible = false;
                }

                if (StockOutRequestDAO.GetRequestStatusByUser(request.RequestId, AD.StockController.Login) != null)
                {
                    rbCommercial.Enabled = 
                    rbNonCommercial.Enabled = 
                    rbStationery.Enabled = 
                    rbPermanent.Enabled = 
                    rbTemporary.Enabled =
                    rbOther.Enabled = false;
                }
                if (request.Purpose == Purpose.Temporary)
                {
                    btnRecievedBack.Visible = true;
                    if (request.RecievedBack == true)
                    {
                        btnRecievedBack.Enabled = false;
                    }
                    else
                    {
                        btnRecievedBack.Enabled = true;
                    }
                }
               
                txtComments.InnerText = request.Comments;
                //if (request.RequestType == RequestType.Stationery)
                //{
                //    rbCommercial.Enabled =
                //    rbNonCommercial.Enabled =
                //    rbStationery.Enabled =
                //    rbPermanent.Enabled =
                //    rbTemporary.Enabled =
                //    txtCostCenter.Enabled =
                //    rbOther.Enabled =
                //    txtBudgetedaccount.Enabled =
                //    txtComments.Disabled =false;
                //}
                #region bindItemsGrid
                dtItems.Columns.Add(new DataColumn("ItemId", typeof(string)));
                dtItems.Columns.Add(new DataColumn("ItemCode", typeof(string)));
                dtItems.Columns.Add(new DataColumn("Description", typeof(string)));
                dtItems.Columns.Add(new DataColumn("Quantity", typeof(string)));
                dtItems.Columns.Add(new DataColumn("Unit", typeof(string)));

                foreach (StockOutRequestItemsModel item in request.Items)
                {
                    DataRow dr = null;
                    dr = dtItems.NewRow();
                    dr["ItemId"] = item.ItemId.ToString();
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
                        dr["ItemId"] = string.Empty;
                        dr["ItemCode"] = string.Empty;
                        dr["Description"] = string.Empty;
                        dr["Quantity"] = string.Empty;
                        dr["Unit"] = string.Empty;
                        dtItems.Rows.Add(dr);
                    }
                }
                dgItems.DataSource = dtItems;
                dgItems.DataBind();
                int rowIndex = 0;
                for (int i = 0; i < dtItems.Rows.Count; i++)
                {
                    HiddenField HiddenFieldItemId = (HiddenField)dgItems.Rows[rowIndex].Cells[0].FindControl("hdnItemId");
                    TextBox TextBoxItemCode = (TextBox)dgItems.Rows[rowIndex].Cells[0].FindControl("txtItemCode");
                    TextBox TextBoxItemDesc = (TextBox)dgItems.Rows[rowIndex].Cells[1].FindControl("txtItemDesc");
                    TextBox TextBoxItemQnty = (TextBox)dgItems.Rows[rowIndex].Cells[2].FindControl("txtItemQnty");
                    TextBox TextBoxItemUOM = (TextBox)dgItems.Rows[rowIndex].Cells[3].FindControl("txtItemUOM");

                    HiddenFieldItemId.Value = dtItems.Rows[i]["ItemId"].ToString();
                    TextBoxItemCode.Text = dtItems.Rows[i]["ItemCode"].ToString();
                    TextBoxItemDesc.Text = dtItems.Rows[i]["Description"].ToString();
                    TextBoxItemQnty.Text = dtItems.Rows[i]["Quantity"].ToString();
                    TextBoxItemUOM.Text = dtItems.Rows[i]["Unit"].ToString();

                    rowIndex++;
                }
                ViewState["CurrentTable"] = dtItems;
                InsertFormButtons.Visible = false;
                EditFormButtons.Visible = true;
                #endregion bindItemsGrid
            }
        }

        private int SaveRequest()
        {
            StockOutRequestModel request = new StockOutRequestModel();
            List<StockOutRequestItemsModel> requestItems = new List<StockOutRequestItemsModel>();

            request.RequestUser = AD.GetUserByLogin(curentUser.Login);
            request.OrderNumber = string.Empty;
            request.FillingDate = DateTime.Now;
            request.RequestorName = curentUser.FirtsName + " " + curentUser.LastName;
            request.Department = curentUser.Department;
            request.Position = curentUser.JobTitle;
            request.CostCenter = txtCostCenter.Text.Trim();
            if (rbCommercial.Checked)
                request.RequestType = RequestType.Commercial;
            else if (rbNonCommercial.Checked)
                request.RequestType = RequestType.NonCommercial;
            else
                request.RequestType = RequestType.Stationery;
            if (!rbStationery.Checked)
            {
                if (rbPermanent.Checked)
                {
                    request.Purpose = Purpose.Permanent;
                    request.DueDate = DateTime.Now;
                    request.OtherPurpose = string.Empty;
                }
                
                else if(rbOther.Checked)
                {
                    request.Purpose = Purpose.Other;
                    request.DueDate = DateTime.Now;
                    request.OtherPurpose = txtOtherPurpose.Text.Trim();
                }
                else
                {
                    request.Purpose = Purpose.Temporary;
                    DateTime.TryParseExact(txtDueDate.Text, "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeDueDate);
                    request.DueDate = dateTimeDueDate;
                    request.OtherPurpose = string.Empty;
                }
                request.BudgetedAccount = txtBudgetedaccount.Text.Trim();
            }

           
            request.Comments = txtComments.InnerText.Trim();

            SetRowData();
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];

            foreach (DataRow item in dtCurrentTable.Rows)
            {
                if (!string.IsNullOrEmpty(item["Description"].ToString()))
                {
                    StockOutRequestItemsModel curentItem = new StockOutRequestItemsModel();
                    curentItem.ItemCode = item["ItemCode"].ToString().Trim();
                    curentItem.ItemDescription = item["Description"].ToString().Trim();
                    curentItem.Quantity = Convert.ToInt32(item["Quantity"].ToString().Trim());
                    curentItem.Unit = item["Unit"].ToString().Trim();
                    requestItems.Add(curentItem);
                }
            }
            request.Items = requestItems;
            int newRequestId = StockOutRequestDAO.CreateRequest(request);

            string msg = string.Format("<b>{0}’s</b> Stock Out Request (ID: SOR {1}) is <b>submitted to you</b> for approval", request.RequestorName, newRequestId.ToString());
            if (newRequestId != 0)
            {
                if (request.RequestType == RequestType.Commercial || request.RequestType == RequestType.NonCommercial)
                {
                    StockOutRequestDAO.RecieveRequestToUser(newRequestId, AD.StockController.Login, "stockcontroller");

                    Notificaion.Add(AD.StockController.Login, SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + newRequestId.ToString(), msg, request.RequestUser.PictureUrl, 6);
                }
                else
                {
                    StockOutRequestDAO.UpdateRequestkeeper(newRequestId.ToString(), "adminstockkeeper");
                    StockOutRequestDAO.RecieveRequestToUser(newRequestId, "adminstockkeeper", "adminstockkeeper");

                    foreach (EDF_SPUser item in AD.AdminStockkeeper.AllUsers)
                    {
                        Notificaion.Add(item.Login, SPContext.Current.Web.Url + "/SitePages/InternalStockOutRequestView.aspx?rid=" + newRequestId.ToString(), msg, request.RequestUser.PictureUrl, 6);
                    }
                }
            }
            return newRequestId;
        }

        private bool ValidateItemsGrid()
        {
            SetRowData();
            bool anyItemIsSpecified = false;

            bool allQtyIsNumber = true;
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];

            foreach (DataRow item in dtCurrentTable.Rows)
            {
                if (!string.IsNullOrEmpty(item["ItemCode"].ToString()) || !string.IsNullOrEmpty(item["Description"].ToString()) || !string.IsNullOrEmpty(item["Quantity"].ToString()) || !string.IsNullOrEmpty(item["Unit"].ToString()))
                {
                    anyItemIsSpecified = true;
                    if (string.IsNullOrEmpty(item["Description"].ToString()) || string.IsNullOrEmpty(item["Quantity"].ToString()) || string.IsNullOrEmpty(item["Unit"].ToString()))
                    {
                        valItems.Visible = true;
                        valItems.InnerText = "Please insert all required fields or remove useless rows.";
                        return false;
                    }
                    else if (!Regex.IsMatch(item["Quantity"].ToString(), @"^\d+$"))
                    {
                        allQtyIsNumber = false;
                    }
                }
            }
            if (!anyItemIsSpecified)
            {
                valItems.Visible = true;
                valItems.InnerText = "Please insert any item.";
                return false;
            }
            else
            {
                if (!allQtyIsNumber)
                {
                    valItems.Visible = true;
                    valItems.InnerText = "Quantity must be number.";
                    return false;
                }
                else
                {
                    valItems.Visible = false;
                    valItems.InnerText = "";
                    return true;
                }
            }
        }

        private bool ValidateForm()
        {
            bool formIsValidate = true;
            if (rbCommercial.Checked || rbNonCommercial.Checked || rbStationery.Checked)
            {
                ValRequestType.Visible = false;
            }
            else
            {
                formIsValidate = false;
                ValRequestType.Visible = true;
                ValRequestType.InnerText = "Please check any radio button.";
            }

            formIsValidate = ValidateItemsGrid();
            if (string.IsNullOrEmpty(txtCostCenter.Text))
            {
                formIsValidate = false;
                ValCostCenter.Visible = true;
            }
            else
            {
                ValCostCenter.Visible = false;
            }
            if (!rbStationery.Checked)
            {
                if (rbPermanent.Checked || rbTemporary.Checked || rbOther.Checked)
                {
                    valPurpose.Visible = false;
                }
                else
                {
                    formIsValidate = false;
                    valPurpose.Visible = true;
                    valPurpose.InnerText = "Please check any radio button.";
                }
                if (rbTemporary.Checked && string.IsNullOrEmpty(txtDueDate.Text))
                {
                    formIsValidate = false;
                    valPurpose.InnerText = "Please insert due date.";
                    valPurpose.Visible = true;
                }

                if (rbTemporary.Checked && !DateTime.TryParseExact(txtDueDate.Text, "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeDueDate))
                {
                    formIsValidate = false;
                    valPurpose.InnerText = "Please insert valid date.";
                    valPurpose.Visible = true;
                    return formIsValidate;
                }
                if (rbTemporary.Checked && dateTimeDueDate < DateTime.Now)
                {
                    formIsValidate = false;
                    valPurpose.InnerText = "Due date should be greater than today.";
                    valPurpose.Visible = true;
                }
            }
            return formIsValidate;
        }

        private void AddNewItem()
        {
            int rowIndex = 0;

            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        HiddenField HiddenFieldItemId = (HiddenField)dgItems.Rows[rowIndex].Cells[0].FindControl("hdnItemId");
                        TextBox TextBoxItemCode = (TextBox)dgItems.Rows[rowIndex].Cells[0].FindControl("txtItemCode");
                        TextBox TextBoxItemDesc = (TextBox)dgItems.Rows[rowIndex].Cells[1].FindControl("txtItemDesc");
                        TextBox TextBoxItemQnty = (TextBox)dgItems.Rows[rowIndex].Cells[2].FindControl("txtItemQnty");
                        TextBox TextBoxItemUOM = (TextBox)dgItems.Rows[rowIndex].Cells[3].FindControl("txtItemUOM");


                        dtCurrentTable.Rows[i - 1]["ItemId"] = HiddenFieldItemId.Value;
                        dtCurrentTable.Rows[i - 1]["ItemCode"] = TextBoxItemCode.Text.Trim();
                        dtCurrentTable.Rows[i - 1]["Description"] = TextBoxItemDesc.Text.Trim();
                        dtCurrentTable.Rows[i - 1]["Quantity"] = TextBoxItemQnty.Text.Trim();
                        dtCurrentTable.Rows[i - 1]["Unit"] = TextBoxItemUOM.Text.Trim();
                        rowIndex++;
                    }
                    drCurrentRow = dtCurrentTable.NewRow();
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    ViewState["CurrentTable"] = dtCurrentTable;

                    dgItems.DataSource = dtCurrentTable;
                    dgItems.DataBind();
                }
            }
            else
            {
                Response.Write("ViewState is null");
            }
            SetPreviousData();
        }

        private void SetPreviousData()
        {
            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        HiddenField HiddenFieldItemId = (HiddenField)dgItems.Rows[rowIndex].FindControl("hdnItemId");
                        TextBox TextBoxItemCode = (TextBox)dgItems.Rows[rowIndex].FindControl("txtItemCode");
                        TextBox TextBoxItemDesc = (TextBox)dgItems.Rows[rowIndex].Cells[1].FindControl("txtItemDesc");
                        TextBox TextBoxItemQnty = (TextBox)dgItems.Rows[rowIndex].Cells[2].FindControl("txtItemQnty");
                        TextBox TextBoxItemUOM = (TextBox)dgItems.Rows[rowIndex].Cells[3].FindControl("txtItemUOM");

                        HiddenFieldItemId.Value = dt.Rows[i]["ItemId"].ToString();
                        TextBoxItemCode.Text = dt.Rows[i]["ItemCode"].ToString();
                        TextBoxItemDesc.Text = dt.Rows[i]["Description"].ToString();
                        TextBoxItemQnty.Text = dt.Rows[i]["Quantity"].ToString();
                        TextBoxItemUOM.Text = dt.Rows[i]["Unit"].ToString();

                        rowIndex++;
                    }
                }
            }
        }

        private void FirstLoadItemsGrid()
        {
            txtDueDate.Visible = false;
            valDateTimeControl1.Visible = false;

            dtItems.Columns.Add(new DataColumn("ItemId", typeof(string)));
            dtItems.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Description", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Quantity", typeof(string)));
            dtItems.Columns.Add(new DataColumn("Unit", typeof(string)));

            for (int i = 0; i < 5; i++)
            {
                DataRow dr = null;
                dr = dtItems.NewRow();
                dr["ItemId"] = string.Empty;
                dr["ItemCode"] = string.Empty;
                dr["Description"] = string.Empty;
                dr["Quantity"] = string.Empty;
                dr["Unit"] = string.Empty;
                dtItems.Rows.Add(dr);
            }

            ViewState["CurrentTable"] = dtItems;
            dgItems.DataSource = dtItems;
            dgItems.DataBind();
        }

        private void SetRowData()
        {
            int rowIndex = 0;

            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        HiddenField HiddenFieldItemId = (HiddenField)dgItems.Rows[rowIndex].Cells[0].FindControl("hdnItemId");
                        TextBox TextBoxItemCode = (TextBox)dgItems.Rows[rowIndex].Cells[0].FindControl("txtItemCode");
                        TextBox TextBoxItemDesc = (TextBox)dgItems.Rows[rowIndex].Cells[1].FindControl("txtItemDesc");
                        TextBox TextBoxItemQnty = (TextBox)dgItems.Rows[rowIndex].Cells[2].FindControl("txtItemQnty");
                        TextBox TextBoxItemUOM = (TextBox)dgItems.Rows[rowIndex].Cells[3].FindControl("txtItemUOM");


                        dtCurrentTable.Rows[i - 1]["ItemId"] = HiddenFieldItemId.Value;
                        dtCurrentTable.Rows[i - 1]["ItemCode"] = TextBoxItemCode.Text.Trim();
                        dtCurrentTable.Rows[i - 1]["Description"] = TextBoxItemDesc.Text.Trim();
                        dtCurrentTable.Rows[i - 1]["Quantity"] = TextBoxItemQnty.Text.Trim();
                        dtCurrentTable.Rows[i - 1]["Unit"] = TextBoxItemUOM.Text.Trim();
                        rowIndex++;
                    }

                    ViewState["CurrentTable"] = dtCurrentTable;
                }
            }
            else
            {
                Response.Write("ViewState is null");
            }
        }

        #endregion Methods
    }
}