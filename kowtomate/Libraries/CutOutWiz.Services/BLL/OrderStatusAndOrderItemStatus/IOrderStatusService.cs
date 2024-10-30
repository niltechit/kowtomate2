using CutOutWiz.Services.Models.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus 
{ 
	public interface IOrderStatusService
	{
		Task<bool> UpdateOrderStatus(ClientOrderModel clientOrder, int contactId);
		Task<bool> UpdateOrderStatusByOrderId(long orderId, int updateByContactId);
		Task<bool> UpdateOrderArrivalTime(ClientOrderModel clientOrder);
		Task UpdateOrderCategorySetStatus(ClientOrderModel clientOrder);

    }
}
