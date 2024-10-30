using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.BLL.UpdateOrderItem
{
	public interface IUpdateOrderItemBLLService
	{
		Task UpdateOrderItemTeamId(ClientOrderModel order);
		Task UpdateOrderItemsStatus(List<ClientOrderItemModel> clientOrderItems, InternalOrderItemStatus itemStatus, int contactId= AutomatedAppConstant.ContactId);
		Task UpdateOrderItemStatus(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus itemStatus, int contactId = AutomatedAppConstant.ContactId);
		Task UpdateOrderItemsStatusByClientItemListModel(List<ClientOrderItemListModel> clientOrderItems, InternalOrderItemStatus itemStatus, int contactId = AutomatedAppConstant.ContactId);
		Task UpdateOrderItemStatusByOrderId(long orderId, InternalOrderItemStatus itemStatus, int contactId = AutomatedAppConstant.ContactId);


    }
}
