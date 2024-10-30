
using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.BLL.StatusChangeLog;
using CutOutWiz.Services.ClientOrders;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus
{
	public class OrderStatusService:IOrderStatusService
	{
		public readonly IClientOrderService _clientOrderService;
		public readonly IClientOrderItemService _clientOrderItemService;
		public readonly IStatusChangeLogBLLService _statusChangeLogBLLService;
		public OrderStatusService(IClientOrderService clientOrderService, IClientOrderItemService clientOrderItemService,
			IStatusChangeLogBLLService statusChangeLogBLLService)
		{
			_clientOrderService = clientOrderService;
			_clientOrderItemService = clientOrderItemService;
			_statusChangeLogBLLService = statusChangeLogBLLService;
		}
		public async Task<bool> UpdateOrderStatus(ClientOrderModel clientOrder,int updateByContactId)
        {

            InternalOrderStatus internalOrderStatus = 0; //Todo:Rakib
            await Task.Yield();
            ClientOrderItemStatus clientOrderItemMinStatus = await _clientOrderItemService.GetOrderItemMinStatusByOrderId(clientOrder.Id);

            if (clientOrderItemMinStatus != null && clientOrderItemMinStatus.Status != null)
            {
                internalOrderStatus = (InternalOrderStatus)clientOrderItemMinStatus.Status;


                clientOrder.InternalOrderStatus = (byte)internalOrderStatus;
                clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(internalOrderStatus));
                var response = await _clientOrderService.UpdateClientOrderStatus(clientOrder);

                await UpdateOrderCategorySetStatus(clientOrder);

                if (response.IsSuccess)
                {
                    await _statusChangeLogBLLService.AddOrderStatusChangeLog(clientOrder, internalOrderStatus, updateByContactId);
                    return true;
                }
            }
            return false;

            //Need To Add OrderStatusLog

        }

        public async Task UpdateOrderCategorySetStatus(ClientOrderModel clientOrder)
        {
            ClientOrderCategorySetStatus clientOrderCategorySetStatus = await _clientOrderItemService.GetOrderItemMinCategorySetStatusByOrderId(clientOrder.Id);
            if (clientOrderCategorySetStatus != null && clientOrderCategorySetStatus.CategorySetStatus != null)
            {
                clientOrder.CategorySetStatus = (byte)(EnumHelper.OrderCategorySetStatusFromItemCategorySetStatus((ItemCategorySetStatus)clientOrderCategorySetStatus.CategorySetStatus));

                await _clientOrderService.UpdateClientOrderCatgegorySetStatus(clientOrder);
            }
        }

        public async Task<bool> UpdateOrderArrivalTime(ClientOrderModel clientOrder)
		{

			await Task.Yield();
			ClientOrderItemArrivalTime clientOrderItemMinArrivalTime = await _clientOrderItemService.GetOrderItemMinArrivalTimeByOrderId(clientOrder.Id);
			clientOrder.ArrivalTime = clientOrderItemMinArrivalTime.ArrivalTime;
		
			var response = await _clientOrderService.UpdateClientOrderArrivalTime(clientOrder);

			if (response.IsSuccess)
			{
				return true;
			}
			return false;

			//Need To Add OrderStatusLog

		}

		public async Task<bool> UpdateOrderStatusByOrderId(long orderId,int updateByContactId)
		{

			InternalOrderStatus internalOrderStatus = 0; //Todo:Rakib
			await Task.Yield();
			ClientOrderItemStatus clientOrderItemMinStatus = await _clientOrderItemService.GetOrderItemMinStatusByOrderId(orderId);

			if(clientOrderItemMinStatus.Status != null)
			{
				internalOrderStatus = (InternalOrderStatus)clientOrderItemMinStatus.Status;

				ClientOrderModel order = await _clientOrderService.GetById(orderId);

				order.InternalOrderStatus = (byte)internalOrderStatus;
				order.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(internalOrderStatus));
				var response = await _clientOrderService.UpdateClientOrderStatus(order);

				if (response.IsSuccess)
				{
					await _statusChangeLogBLLService.AddOrderStatusChangeLog(order, internalOrderStatus, updateByContactId);
					return true;
				}
			}
			
			return false;

			//Need To Add OrderStatusLog

		}
	}
}
