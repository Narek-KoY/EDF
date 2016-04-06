
namespace EDF_CommonData
{
    public class StockOutRequestItemsModel
    {
        public int ItemId { get; set; }
        public int RequestID { get; set; }

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
    }
}
