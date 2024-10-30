using CutOutWiz.Services.Models.ClientOrders;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Core.Utilities
{
	public class CssHelper
	{
		#region Status Color Changes
		public string GivenColorForStatus(ClientOrderItemModel item)
		{
			var colorCode = "";
			if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.OrderPlaced)
			{
				colorCode = "color:Hexff0000";
			}
			else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ProductionDone)
			{
				colorCode = "color:blue";
			}
			else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.InProduction)
			{
				colorCode = "color:red";
			}
			else
			{
				colorCode = "color:white";
			}
			return colorCode;
		}
		// Set Internal Order Stutus for Admin
		public string SetInternalOrderStutusBackgroundColor(ClientOrderListModel order)
		{
			var colorCode = "";

			if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.OrderPlaced)
			{
				colorCode = "badge OrderPlaced";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.OrderPlacing)
			{
				colorCode = "badge Assigned "; //item-status-assigned
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.Assigned)
			{
				colorCode = "badge Assigned "; //item-status-assigned
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.InProgress)
			{
				colorCode = "badge InProgress";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.InProduction)
			{
				colorCode = "badge InProduction";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.ProductionDone)
			{
				colorCode = "badge ProductionDone";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.InQc)
			{
				colorCode = "badge InQc";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.ReworkQc)
			{
				colorCode = "badge ReworkQc";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.Distributed)
			{
				colorCode = "badge Distributed";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.ReadyToDeliver)
			{
				colorCode = "badge ReadyToDeliver";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.Delivered)
			{
				colorCode = "badge Delivered";
			}
			else if ((InternalOrderStatus)order.InternalOrderStatus == InternalOrderStatus.Completed)
			{
				colorCode = "badge Completed";
			}
			else
			{
				colorCode = "badge bg-primary";
			}
			return colorCode;

		}
		// Set Externale Order Status for Client
		public string SetExternalOrderStutusBackgroundColor(ClientOrderListModel order)
		{
			var colorCode = "";

			if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.OrderPlaced)
			{
				colorCode = "badge ClientOrderPlaced";
			}
			else if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.OrderPlacing)
			{
				colorCode = "badge ClientOrderPlacing "; //item-status-assigned
			}
			
			else if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.InProgress)
			{
				colorCode = "badge ClientOrderInProgress";
			}
			
			else if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.InQc)
			{
				colorCode = "badge ClientOrderInQC";
			}
			
			else if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.ReadyToDeliver)
			{
				colorCode = "badge ClientOrderReadyToDownload";
			}
            else if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.Rejected)
            {
                colorCode = "badge ClientOrderPlacing";
            }
            else if ((ExternalOrderStatus)order.ExternalOrderStatus == ExternalOrderStatus.Completed)
			{
				colorCode = "badge Completed";
			}
			else
			{
				colorCode = "badge bg-primary";
			}
			return colorCode;

		}
		public string GivenBackgroundColorForStatus(int internalOrderItemStatus)
        {
			return GivenBackgroundColorForStatus(new ClientOrderItemModel { Status = (byte)internalOrderItemStatus });
		}
		public string GivenBackgroundColorForStatus(ClientOrderItemModel item)
		{
            string colorCode;

            switch ((InternalOrderItemStatus)item.Status)
            {
                case InternalOrderItemStatus.OrderPlaced:
                    colorCode = "badge OrderPlaced";
                    break;
                case InternalOrderItemStatus.Assigned:
                    colorCode = "badge Assigned";
                    break;
                case InternalOrderItemStatus.AssignedForSupport:
                    colorCode = "badge AssignedForSupport";
                    break;
                case InternalOrderItemStatus.Distributed:
                    colorCode = "badge Distributed";
                    break;
                case InternalOrderItemStatus.InProduction:
                    colorCode = "badge InProduction";
                    break;
                case InternalOrderItemStatus.ProductionDone:
                    colorCode = "badge ProductionDone";
                    break;
                case InternalOrderItemStatus.InQc:
                    colorCode = "badge InQc";
                    break;
                case InternalOrderItemStatus.ReworkDistributed:
                    colorCode = "badge ReworkDistributed";
                    break;
                case InternalOrderItemStatus.ReworkInProduction:
                    colorCode = "badge ReworkInProduction";
                    break;
                case InternalOrderItemStatus.ReworkDone:
                    colorCode = "badge ReworkDone";
                    break;
                case InternalOrderItemStatus.ReworkQc:
                    colorCode = "badge ReworkQc";
                    break;
                case InternalOrderItemStatus.ReadyToDeliver:
                    colorCode = "badge ReadyToDeliver";
                    break;
                case InternalOrderItemStatus.Delivered:
                    colorCode = "badge Delivered";
                    break;
                case InternalOrderItemStatus.Completed:
                    colorCode = "badge Completed";
                    break;
                case InternalOrderItemStatus.Rejected:
                    colorCode = "badge Rejected";
                    break;
                default:
                    colorCode = "badge bg-primary";
                    break;
            }

            return colorCode;
        }

        public string GivenBackgroundColorForItemCategorySetStatus(ClientOrderItemModel item)
        {
            string colorCode;



            switch ((ItemCategorySetStatus)item.CategorySetStatus)
            {
                case ItemCategorySetStatus.Not_set:
                    colorCode = "badge OrderPlaced";
                    break;
                case ItemCategorySetStatus.Auto_set:
                    colorCode = "badge Assigned";
                    break;
                case ItemCategorySetStatus.Manual_set:
                    colorCode = "badge AssignedForSupport";
                    break;
                case ItemCategorySetStatus.Approved:
                    colorCode = "badge Completed";
                    break;
                default:
                    colorCode = "badge bg-primary";
                    break;
            }

            return colorCode;
        }

        public string GivenBackgroundColorForOrderCategorySetStatus(ClientOrderListModel clientOrder)
        {
            string colorCode;

            switch ((OrderCategorySetStatus)clientOrder.CategorySetStatus)
            {
                case OrderCategorySetStatus.Not_set:
                    colorCode = "badge OrderPlaced";
                    break;
                case OrderCategorySetStatus.Auto_set:
                    colorCode = "badge Assigned";
                    break;
                case OrderCategorySetStatus.Manual_set:
                    colorCode = "badge AssignedForSupport";
                    break;
                case OrderCategorySetStatus.Approved:
                    colorCode = "badge Completed";
                    break;
                default:
                    colorCode = "badge bg-primary";
                    break;
            }

            return colorCode;
        }

        public string GivenBackgroundColorForExternalStatus(int externalOrderItemStatus)
        {
            return GivenBackgroundColorForExternalStatus(new ClientOrderItemModel { ExternalStatus = (byte)externalOrderItemStatus });
        }
        public string GivenBackgroundColorForExternalStatus(ClientOrderItemModel item)
        {
            //var colorCode = "";
            //if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.OrderPlaced)
            //{
            //	colorCode = "badge OrderPlaced";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.Assigned)
            //{
            //	colorCode = "badge Assigned "; //item-status-assigned
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.AssignedForSupport)
            //{
            //	colorCode = "badge AssignedForSupport";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.Distributed)
            //{
            //	colorCode = "badge Distributed";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.InProduction)
            //{
            //	colorCode = "badge InProduction";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ProductionDone)
            //{
            //	colorCode = "badge ProductionDone";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.InQc)
            //{
            //	colorCode = "badge InQc";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ReworkDistributed)
            //{
            //	colorCode = "badge ReworkDistributed";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ReworkInProduction)
            //{
            //	colorCode = "badge ReworkInProduction";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ReworkDone)
            //{
            //	colorCode = "badge ReworkDone";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ReworkQc)
            //{
            //	colorCode = "badge ReworkQc";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.ReadyToDeliver)
            //{
            //	colorCode = "badge ReadyToDeliver";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.Delivered)
            //{
            //	colorCode = "badge Delivered";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.Completed)
            //{
            //	colorCode = "badge Completed";
            //}
            //else if ((InternalOrderItemStatus)item.Status == InternalOrderItemStatus.Rejected)
            //{
            //	colorCode = "badge Rejected";
            //}
            //else
            //{
            //	colorCode = "badge bg-primary";
            //}
            //return colorCode;
            string colorCode;

            switch ((ExternalOrderItemStatus)item.ExternalStatus)
            {
                case ExternalOrderItemStatus.OrderPlaced:
                    colorCode = "badge OrderPlaced";
                    break;
               
                case ExternalOrderItemStatus.InQc:
                    colorCode = "badge InQc";
                    break;
                
                case ExternalOrderItemStatus.ReworkInProduction:
                    colorCode = "badge ReworkInProduction";
                    break;
                case ExternalOrderItemStatus.ReworkDone:
                    colorCode = "badge ReworkDone";
                    break;
                case ExternalOrderItemStatus.ReworkQc:
                    colorCode = "badge ReworkQc";
                    break;
                
                case ExternalOrderItemStatus.Delivered:
                    colorCode = "badge Delivered";
                    break;
                case ExternalOrderItemStatus.Completed:
                    colorCode = "badge Completed";
                    break;
                case ExternalOrderItemStatus.Rejected:
                    colorCode = "badge Rejected";
                    break;
                default:
                    colorCode = "badge bg-primary";
                    break;
            }

            return colorCode;

        }
        #endregion
    }
}
