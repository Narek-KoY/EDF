using System;
using System.Collections.Generic;

namespace EDF_CommonData
{
    public enum RequestType
    {
        Commercial,
        NonCommercial,
        Stationery 

    }
    public enum Purpose
    {
        Permanent,
        Temporary,
        Other
    }

    public class StockOutRequestModel
    {
        public int RequestId { get; set; }
        public EDF_CommonData.EDF_SPUser RequestUser { get; set; }
        public EDF_CommonData.EDF_SPUser RequestUserManager { get { return RequestUser.Manager; }}
        public string  RequestStockkeeper { get; set; }     
        public string OrderNumber { get; set; }
        public DateTime FillingDate { get; set; }
        public string RequestorName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string CostCenter { get; set; }
        public RequestType RequestType { get; set; }
        public Purpose Purpose { get; set; }
        public DateTime DueDate { get; set; }        
        public List<StockOutRequestItemsModel> Items { get; set; }
        public bool? Provided { get; set; }
        public bool? RecievedBack { get; set; }
        public bool? Approved { get; set; }


        public string BudgetedAccount { get; set; }
        public string OtherPurpose { get; set; }
        public string Comments { get; set; }
    }
}
