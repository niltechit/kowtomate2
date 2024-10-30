
namespace CutOutWiz.Services.Models.ClientOrders
{
	public class ClientOrderItemStatus
	{
		public byte? Status { get; set; }
		public byte? ExternalStatus { get; set; }
	}

    public class ClientOrderCategorySetStatus
    {
        public byte? CategorySetStatus { get; set; }
    }

    public class ClientOrderItemCount
	{
		public int TotalPrductionOngoingItem { get; set; }
	}

	public class ClientOrderItemArrivalTime
	{
		public DateTime? ArrivalTime { get; set; }
		
	}

    public class ClientOrderItemDeliveryDate
    {
        public DateTime? MinDeliveryDate { get; set; }
        public DateTime? MaxDeliveryDate { get; set; }

    }
}
