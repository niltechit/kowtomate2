using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.BLL.StatusChangeLog;
using CutOutWiz.Services.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.BLL.UpdateOrderItem
{
    public class UpdateOrderItemBLLService:IUpdateOrderItemBLLService
	{
		public readonly IClientOrderItemService _clientOrderItemService;
		public readonly ICompanyTeamManager _companyTeamService;
		public readonly IStatusChangeLogBLLService _statusChangeLogBLLService;
		public UpdateOrderItemBLLService(IClientOrderItemService clientOrderItemService, ICompanyTeamManager companyTeamService,
			IStatusChangeLogBLLService statusChangeLogBLLService)
		{
			_clientOrderItemService = clientOrderItemService;
			_companyTeamService = companyTeamService;
			_statusChangeLogBLLService = statusChangeLogBLLService;
		}
		public async Task UpdateOrderItemTeamId(ClientOrderModel order)
		{
			var clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
			CompanyTeamModel companyTeam = await _companyTeamService.GetTeamByCompanyId(order.CompanyId);
			
			List<string> clientOrderItemIds = new List<string>();
			foreach (var orderItem in clientOrderItems)
			{
				clientOrderItemIds.Add(orderItem.Id.ToString());
			}

			await _clientOrderItemService.UpdateClientOrderItemTeamId((int)order.Id, companyTeam.TeamId, clientOrderItemIds);

		}
	

		public async Task UpdateOrderItemsStatus(List<ClientOrderItemModel> clientOrderItems, InternalOrderItemStatus itemStatus,int contactId = AutomatedAppConstant.ContactId)
		{
			
			foreach (var orderItem in clientOrderItems)
			{
			
				orderItem.Status = (byte)itemStatus;
				orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(itemStatus));
				await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);
				await _statusChangeLogBLLService.AddOrderItemStatusChangeLog(orderItem, itemStatus, contactId);

			}
		}

		public async Task UpdateOrderItemsStatusByClientItemListModel(List<ClientOrderItemListModel> clientOrderItems, InternalOrderItemStatus itemStatus, int contactId = AutomatedAppConstant.ContactId)
		{

			foreach (var orderItem in clientOrderItems)
			{
			
				orderItem.Status = (byte)itemStatus;
				orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(itemStatus));
				await _clientOrderItemService.UpdateClientOrderItemListModelStatus(orderItem);
				await _statusChangeLogBLLService.AddOrderItemListModelStatusChangeLog(orderItem, itemStatus, contactId);
			}
		}

		public async Task UpdateOrderItemStatus(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus itemStatus, int contactId = AutomatedAppConstant.ContactId)
		{
                Console.WriteLine(clientOrderItem.Id);
                clientOrderItem.Status = (byte)itemStatus;
			    clientOrderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(itemStatus));
				await _clientOrderItemService.UpdateClientOrderItemStatus(clientOrderItem);
				await _statusChangeLogBLLService.AddOrderItemStatusChangeLog(clientOrderItem, itemStatus, contactId);
		}


        public async Task UpdateOrderItemStatusByOrderId(long orderId, InternalOrderItemStatus itemStatus, int contactId = AutomatedAppConstant.ContactId)
        {

            List<ClientOrderItemModel> clientOrderItems= await _clientOrderItemService.GetAllByOrderId(orderId);

            foreach (var orderItem in clientOrderItems)
            {

                orderItem.Status = (byte)itemStatus;
                orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(itemStatus));
                await UpdateOrderItemStatus(orderItem,itemStatus);
            }

        }

    }
}
