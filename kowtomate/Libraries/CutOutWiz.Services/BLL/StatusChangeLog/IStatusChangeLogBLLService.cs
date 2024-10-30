
using CutOutWiz.Services.Models.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.BLL.StatusChangeLog
{
	public interface IStatusChangeLogBLLService
	{
		Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus,int changeByContactId);
		Task AddOrderItemStatusChangeLog(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus, int changeByContactId);
		Task AddOrderItemListModelStatusChangeLog(ClientOrderItemListModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus, int changeByContactId);
	}
}
