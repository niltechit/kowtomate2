using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Core.Utilities
{
    public static class EnumHelper
    {
        public static ExternalOrderStatus ExternalOrderStatusChange(InternalOrderStatus internalorderStatus)
        {
            switch (internalorderStatus)
            {
                case InternalOrderStatus.OrderPlaced:
                    return ExternalOrderStatus.OrderPlaced;
                
                case InternalOrderStatus.Downloaded:
                    return ExternalOrderStatus.DownLoaded;
                
                case InternalOrderStatus.Assigned:
                case InternalOrderStatus.Distributed:
                case InternalOrderStatus.InProduction:
                case InternalOrderStatus.ProductionDone:
                    return ExternalOrderStatus.InProgress;
                case InternalOrderStatus.InQc:
                case InternalOrderStatus.ReworkInProduction:
                case InternalOrderStatus.ReworkDistributed:
                case InternalOrderStatus.ReworkDone:
                case InternalOrderStatus.ReworkQc:
                case InternalOrderStatus.ReadyToDeliver:
                    return ExternalOrderStatus.InQc;
                case InternalOrderStatus.Delivered:
                    return ExternalOrderStatus.ReadyToDownload; //Todo:Rakib if approval tool have then it will be ready to check .
                case InternalOrderStatus.Completed:
                    return ExternalOrderStatus.Completed;
                case InternalOrderStatus.OrderPlacing:
                    return ExternalOrderStatus.OrderPlacing;
                default:
                    return ExternalOrderStatus.Completed;
            }
        }

        public static ExternalOrderItemStatus ExternalOrderItemStatusChange(InternalOrderItemStatus internalOrderItemStatus)
        {
            switch (internalOrderItemStatus)
            {
                case InternalOrderItemStatus.OrderPlaced:
                    return ExternalOrderItemStatus.OrderPlaced;


                case InternalOrderItemStatus.Assigned:
                case InternalOrderItemStatus.Distributed:
                case InternalOrderItemStatus.InProduction:
                case InternalOrderItemStatus.ProductionDone:
				case InternalOrderItemStatus.InQc:
					return ExternalOrderItemStatus.InProgress;
                
                
                case InternalOrderItemStatus.ReworkDistributed:
                case InternalOrderItemStatus.ReworkInProduction:
                case InternalOrderItemStatus.ReworkDone:
                case InternalOrderItemStatus.ReworkQc:
                    return ExternalOrderItemStatus.InQc;
				case InternalOrderItemStatus.ReadyToDeliver:
                    return ExternalOrderItemStatus.ReadyToDownload;
				case InternalOrderItemStatus.Completed:
                    return ExternalOrderItemStatus.Completed;

                case InternalOrderItemStatus.Delivered:
                    return ExternalOrderItemStatus.ReadyToDownload;
				case InternalOrderItemStatus.Rejected:
					return ExternalOrderItemStatus.Rejected;
				default:
                    return ExternalOrderItemStatus.InProgress;
            }
        }

        public static OrderCategorySetStatus OrderCategorySetStatusFromItemCategorySetStatus(ItemCategorySetStatus itemCategorySetStatus)
        {
            switch (itemCategorySetStatus)
            {
                case ItemCategorySetStatus.Not_set:
                    return OrderCategorySetStatus.Not_set;
                case ItemCategorySetStatus.Auto_set:
                    return OrderCategorySetStatus.Auto_set;
                case ItemCategorySetStatus.Manual_set:
                    return OrderCategorySetStatus.Manual_set;
                case ItemCategorySetStatus.Approved:
                    return OrderCategorySetStatus.Approved;
                default:
                    return OrderCategorySetStatus.Not_set;
            }
        }
    }
}
