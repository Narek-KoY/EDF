using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EDF_CommonData
{
    public static class StockOutRequestDAO
    {
        public static int CreateRequest(StockOutRequestModel request)
        {
            SqlParameter[] requestDetailsparam = new SqlParameter[13];
            requestDetailsparam[0] = new SqlParameter("@pOrderNumber", request.OrderNumber);
            requestDetailsparam[1] = new SqlParameter("@pFilingDate", request.FillingDate);
            requestDetailsparam[2] = new SqlParameter("@pRequestorName", string.IsNullOrEmpty(request.RequestorName) ? "" : request.RequestorName);
            requestDetailsparam[3] = new SqlParameter("@pDepartment", string.IsNullOrEmpty(request.Department) ? "" : request.Department);
            requestDetailsparam[4] = new SqlParameter("@pPosition", string.IsNullOrEmpty(request.Position) ? "" : request.Position);
            requestDetailsparam[5] = new SqlParameter("@pRequestType", request.RequestType.ToString());

            requestDetailsparam[6] = new SqlParameter("@pPurpose", (request.RequestType == RequestType.Stationery) ? string.Empty : request.Purpose.ToString());
            requestDetailsparam[7] = new SqlParameter("@pDueDate", (request.RequestType == RequestType.Stationery) ? string.Empty : request.DueDate.ToString());

            requestDetailsparam[8] = new SqlParameter("@pBudgetedAccount", (request.BudgetedAccount == null) ? string.Empty : request.BudgetedAccount);

            requestDetailsparam[9] = new SqlParameter("@pRequestUserLogin", request.RequestUser.Login);
            requestDetailsparam[10] = new SqlParameter("@pCostCenter", request.CostCenter);
            requestDetailsparam[11] = new SqlParameter("@pOtherPurpose", (request.RequestType == RequestType.Stationery) ? string.Empty: request.OtherPurpose);
            requestDetailsparam[12] = new SqlParameter("@pComments", request.Comments);


            try
            {
                int requestId = Int32.Parse(DBManager.ExecuteSprocScalar("dbo.StockOutRequest_Insert", requestDetailsparam).ToString());
                foreach (StockOutRequestItemsModel item in request.Items)
                {
                    item.RequestID = requestId;
                    CreateRequestItem(item);
                }
                return requestId;
            }
            catch
            {
                return 0;
            }
        }
       
        public static bool UpdateRequest(StockOutRequestModel request)
        {
            bool isUpdatedSuccessfully = true;

            SqlParameter[] requestDetailsparam = new SqlParameter[8];

            requestDetailsparam[0] = new SqlParameter("@pRequestId", request.RequestId);
            requestDetailsparam[1] = new SqlParameter("@pRequestType", request.RequestType.ToString());
            requestDetailsparam[2] = new SqlParameter("@pPurpose", (request.RequestType == RequestType.Stationery) ? string.Empty : request.Purpose.ToString());
            requestDetailsparam[3] = new SqlParameter("@pDueDate", (request.RequestType == RequestType.Stationery) ? string.Empty : request.DueDate.ToString());
            requestDetailsparam[4] = new SqlParameter("@pBudgetedAccount", (request.BudgetedAccount == null) ? string.Empty : request.BudgetedAccount);
            requestDetailsparam[5] = new SqlParameter("@pCostCenter", request.CostCenter);
            requestDetailsparam[6] = new SqlParameter("@pOtherPurpose", (request.RequestType == RequestType.Stationery) ? string.Empty : request.OtherPurpose);
            requestDetailsparam[7] = new SqlParameter("@pComments", request.Comments);

            SqlParameter[] requestDetailsparam1 = new SqlParameter[1];
            requestDetailsparam1[0] = new SqlParameter("@pRequestId", request.RequestId);

            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequest_Update", requestDetailsparam);
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequestItems_RemoveItemsByRequestId", requestDetailsparam1);
                foreach (StockOutRequestItemsModel item in request.Items)
                {
                    CreateRequestItem(item);
                }
                return isUpdatedSuccessfully;
            }
            catch
            {
                return false;
            }
        }

        public static void UpdateRequestkeeper(string requestId, string keeperLogin)
        {
            SqlParameter[] sparam = new SqlParameter[2];
            sparam[0] = new SqlParameter("@pRequestId", requestId);
            sparam[1] = new SqlParameter("@pkeeperLogin", keeperLogin);
            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequest_UpdateRequestkeeper", sparam);
            }
            catch
            {
            }
        }

        public static StockOutRequestModel GetRequestById(int requestId)
        {
            StockOutRequestModel curentModel = new StockOutRequestModel();
            List<StockOutRequestItemsModel> curentModelItems = new List<StockOutRequestItemsModel>();
            DataTable dtCurentModel = new DataTable();
            DataTable dtCurentModelItems = new DataTable();
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@pRequestId", requestId);

            SqlParameter[] param1 = new SqlParameter[1];
            param1[0] = new SqlParameter("@pRequestId", requestId);
            try
            {
                dtCurentModel = DBManager.ExecuteSprocDataTable("dbo.GetStockOutRequestById", param);
                dtCurentModelItems = DBManager.ExecuteSprocDataTable("dbo.GetStockOutRequestItemsById", param1);
                for (int i = 0; i < dtCurentModelItems.Rows.Count; i++)
                {
                    StockOutRequestItemsModel model = new StockOutRequestItemsModel();
                    model.ItemCode = dtCurentModelItems.Rows[i]["ItemCode"].ToString();
                    model.ItemDescription = dtCurentModelItems.Rows[i]["ItemDescription"].ToString();
                    model.ItemId = Convert.ToInt32(dtCurentModelItems.Rows[i]["ItemId"].ToString());
                    model.Quantity = Convert.ToInt32(dtCurentModelItems.Rows[i]["Quantity"].ToString());
                    model.RequestID = Convert.ToInt32(dtCurentModelItems.Rows[i]["RequestID"].ToString());
                    model.Unit = dtCurentModelItems.Rows[i]["Unit"].ToString();

                    curentModelItems.Add(model);
                }

                curentModel.RequestId = Convert.ToInt32(dtCurentModel.Rows[0]["RequestId"].ToString());

                curentModel.Department = dtCurentModel.Rows[0]["Department"].ToString();

                curentModel.FillingDate = Convert.ToDateTime(dtCurentModel.Rows[0]["FillingDate"].ToString());
                curentModel.OrderNumber = dtCurentModel.Rows[0]["OrderNumber"].ToString();
                curentModel.Position = dtCurentModel.Rows[0]["Position"].ToString();
                curentModel.RequestorName = dtCurentModel.Rows[0]["RequestorName"].ToString();
                curentModel.CostCenter = dtCurentModel.Rows[0]["CostCenter"].ToString();
                curentModel.Items = curentModelItems;
                curentModel.RequestUser = AD.GetUserByLogin(dtCurentModel.Rows[0]["RequestUserLogin"].ToString());
                curentModel.RequestStockkeeper = string.IsNullOrEmpty(dtCurentModel.Rows[0]["RequestStockkeeper"].ToString()) ? null : dtCurentModel.Rows[0]["RequestStockkeeper"].ToString();
                if (dtCurentModel.Rows[0]["Provided"] != DBNull.Value) curentModel.Provided = Convert.ToBoolean(dtCurentModel.Rows[0]["Provided"]);
                if (dtCurentModel.Rows[0]["Approved"] != DBNull.Value) curentModel.Approved = Convert.ToBoolean(dtCurentModel.Rows[0]["Approved"]);
                switch (dtCurentModel.Rows[0]["RequestType"].ToString())
                {
                    case "Commercial": curentModel.RequestType = RequestType.Commercial; break;
                    case "NonCommercial": curentModel.RequestType = RequestType.NonCommercial; break;
                    case "Stationery": curentModel.RequestType = RequestType.Stationery; break;
                }
                if (curentModel.RequestType != RequestType.Stationery)
                {
                    switch (dtCurentModel.Rows[0]["Purpose"].ToString())
                    {
                        case "Permanent": curentModel.Purpose = Purpose.Permanent; break;
                        case "Temporary": curentModel.Purpose = Purpose.Temporary;
                            if (dtCurentModel.Rows[0]["ReceivedBack"] != DBNull.Value)
                                curentModel.RecievedBack = Convert.ToBoolean(dtCurentModel.Rows[0]["ReceivedBack"]);
                                break;
                        case "Other": curentModel.Purpose = Purpose.Other; break;
                    }
                    curentModel.DueDate = Convert.ToDateTime(dtCurentModel.Rows[0]["DueDate"].ToString());
                    curentModel.OtherPurpose = dtCurentModel.Rows[0]["OtherPurpose"].ToString();
                }
                curentModel.BudgetedAccount = dtCurentModel.Rows[0]["BudgetedAccount"].ToString();
                curentModel.Comments = dtCurentModel.Rows[0]["Comments"].ToString();
            }
            catch
            {
            }
            return curentModel;
        }

        public static bool CreateRequestItem(StockOutRequestItemsModel requestItem)
        {
            bool isCreatedSuccessfully = true;
            SqlParameter[] requestItemDetailsparam = new SqlParameter[5];
            requestItemDetailsparam[0] = new SqlParameter("@pRequestID", requestItem.RequestID);
            requestItemDetailsparam[1] = new SqlParameter("@pItemCode", requestItem.ItemCode);
            requestItemDetailsparam[2] = new SqlParameter("@pItemDescription", requestItem.ItemDescription);
            requestItemDetailsparam[3] = new SqlParameter("@pQuantity", requestItem.Quantity);
            requestItemDetailsparam[4] = new SqlParameter("@pUnit", requestItem.Unit);

            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequestItems_Insert", requestItemDetailsparam);
                return isCreatedSuccessfully;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool UpdateRequestItem(StockOutRequestItemsModel requestItem)
        {
            bool isUpdatedSuccessfully = true;
            SqlParameter[] requestItemDetailsparam = new SqlParameter[5];
            requestItemDetailsparam[0] = new SqlParameter("@pItemId", requestItem.ItemId);
            requestItemDetailsparam[1] = new SqlParameter("@pItemCode", requestItem.ItemCode);
            requestItemDetailsparam[2] = new SqlParameter("@pItemDescription", requestItem.ItemDescription);
            requestItemDetailsparam[3] = new SqlParameter("@pQuantity", requestItem.Quantity);
            requestItemDetailsparam[4] = new SqlParameter("@pUnit", requestItem.Unit);

            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequestItems_Update", requestItemDetailsparam);
                return isUpdatedSuccessfully;
            }
            catch
            {
                return false;
            }
        }

        public static int GetExistOrdersCount(string requestType)
        {
            int count = -1;
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@pRequestType", requestType.ToString());

            try
            {
                count = Int32.Parse(DBManager.ExecuteSprocScalar("dbo.StockOutRequest_GetExistOrdersCount", sqlParameters).ToString());
            }
            catch
            {

            }
            return count;
        }

        public static void UpdateOrderNumber(int requestId, string orderNumber)
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@pRequestId", requestId);
            sqlParameters[1] = new SqlParameter("@pOrderNumber", orderNumber.ToString());

            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequest_UpdateOrderNumber", sqlParameters).ToString();
            }
            catch
            {

            }
        }

        public static bool RecieveRequestToUser(int requestId, string UserId, string group)
        {
            bool isCreatedSuccessfully = true;
            SqlParameter[] sparam = new SqlParameter[3];
            sparam[0] = new SqlParameter("@pRequestID", requestId);
            sparam[1] = new SqlParameter("@pRecieveUserId", UserId);
            sparam[2] = new SqlParameter("@pGroupe", group);

            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.SORApprove_Insert", sparam);
                return isCreatedSuccessfully;
            }
            catch
            {
                return false;
            }
        }

        public static bool RecieveRequestReject(int requestId)
        {
            bool isCreatedSuccessfully = true;
            SqlParameter[] sparam = new SqlParameter[1];
            sparam[0] = new SqlParameter("@pRequestID", requestId);           

            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.SORApprove_reject", sparam);
                return isCreatedSuccessfully;
            }
            catch
            {
                return false;
            }
        }

        public static bool RecieveRequestApprove(int requestId, string serId)
        {
            bool isCreatedSuccessfully = true;
            SqlParameter[] sparam = new SqlParameter[2];
            sparam[0] = new SqlParameter("@pRequestID", requestId);
            sparam[1] = new SqlParameter("@pRecieveUserId", serId);
            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.SORApprove_update", sparam);
                return isCreatedSuccessfully;
            }
            catch
            {
                return false;
            }
        }
     
        public static bool? GetRequestStatusByUser(int requestId, string login)
        {
            SqlParameter[] sparam = new SqlParameter[2];
            sparam[0] = new SqlParameter("@pRequestID", requestId);
            sparam[1] = new SqlParameter("@pRecieveUserId", login);
            try
            {
                var status = DBManager.ExecuteSprocScalar("dbo.StockOutRequest_GetRequestStatusByUser", sparam);
                if (status == DBNull.Value)
                    return null;
                else
                    return Convert.ToBoolean(status);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetRequestAppDate(int requestId, string login)
        {
            SqlParameter[] sparam = new SqlParameter[2];
            sparam[0] = new SqlParameter("@pRequestID", requestId);
            sparam[1] = new SqlParameter("@pRecieveUserId", login);
            try
            {
                DateTime appDate = Convert.ToDateTime(DBManager.ExecuteSprocScalar("dbo.StockOutRequest_GetRequestAppDate", sparam));
                return appDate;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
       
        public static DataTable GetRejectedUser(int requestId)
        {
            DataTable table = new DataTable();
            SqlParameter[] sparam = new SqlParameter[1];
            sparam[0] = new SqlParameter("@pRequestID", requestId);

            try
            {
                table = DBManager.ExecuteSprocDataTable("dbo.StockOutRequest_GetRejectedUser", sparam);
                return table;
            }
            catch
            {
                return table;
            }

        }

        public static bool AllowUserApprove(int requestId, string login)
        {
            SqlParameter[] sparam = new SqlParameter[2];
            sparam[0] = new SqlParameter("@pRequestID", requestId);
            sparam[1] = new SqlParameter("@pRecieveUserId", login);
            try
            {
                DataTable status = DBManager.ExecuteSprocDataTable("dbo.StockOutRequest_AllowUserApprove", sparam);
                if (status.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
      
        public static void RemoveRecievedsRequest(int requestId)
        {
            SqlParameter[] sparam = new SqlParameter[1];
            sparam[0] = new SqlParameter("@pRequestID", requestId);
            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.RemoveRecievedsRequest", sparam);
            }
            catch
            {
            }
        }

        public static bool UpdateRecievedBack(int requestId, bool value)
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@pRequestId", requestId);
            sqlParameters[1] = new SqlParameter("@pRecievedBack", value);
            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequest_UpdateRecievedBack", sqlParameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateRequestProvided(int requestId, bool value)
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@pRequestId", requestId);
            sqlParameters[1] = new SqlParameter("@pProvided", value);
            try
            {
                DBManager.ExecuteSprocNonQuery("dbo.StockOutRequest_UpdateProvided", sqlParameters);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
