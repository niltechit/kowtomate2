using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.OrderItemStatusChangeLogService;
using CutOutWiz.Services.OrderAndOrderItemStatusChangeLogServices;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.BLL.StatusChangeLog
{
	public class StatusChangeLogBLLService:IStatusChangeLogBLLService
	{
		private readonly IOrderStatusChangeLogService _orderStatusChangeLogService;
		private readonly IOrderItemStatusChangeLogService _orderItemStatusChangeLogService;
		private readonly IActivityAppLogService _activityAppLogService;
		public StatusChangeLogBLLService(IOrderStatusChangeLogService orderStatusChangeLogService,
			IOrderItemStatusChangeLogService orderItemStatusChangeLogService,
			IActivityAppLogService activityAppLogService)
		{
			_orderStatusChangeLogService = orderStatusChangeLogService;
			_orderItemStatusChangeLogService = orderItemStatusChangeLogService;
			_activityAppLogService = activityAppLogService;
		}
		public async Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus, int changeByContactId=AutomatedAppConstant.ContactId)
		{
			try
			{
				var previousLog = await _orderStatusChangeLogService.OrderStatusLastChangeLogByOrderId((int)clientOrder.Id);

				if (previousLog != null && previousLog.NewInternalStatus == (byte)internalOrderStatus)
				{
					return;
				}
				
				
					OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
					{
						OrderId = (int)clientOrder.Id,
						NewInternalStatus = (byte)internalOrderStatus,
						NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
						ChangeByContactId = changeByContactId,
						ChangeDate = DateTime.Now
					};


					if (previousLog != null)
					{
						orderStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
						orderStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
						orderStatusChangeLog.TimeDurationInMinutes = (int)orderStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes;
					}
					await _orderStatusChangeLogService.Insert(orderStatusChangeLog);

				
				
			}
			catch (Exception ex)
			{
				var loginUser = new LoginUserInfoViewModel
				{
					ContactId = AutomatedAppConstant.ContactId
				};
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					//PrimaryId = (int)order.Id,
					ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "AddOrderStatusChangeLog",
					RazorPage = "FtpOrderProcessService",
					Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
				};
				await _activityAppLogService.InsertAppErrorActivityLog(activity);
			}
		}

		public async Task AddOrderItemStatusChangeLog(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus, int changeByContactId)
		{
			try
			{
				var previousLog = await _orderItemStatusChangeLogService.OrderItemStatusLastChangeLogByOrderFileId((int)clientOrderItem.Id);

				if (previousLog != null && previousLog.NewInternalStatus == (byte)internalOrderItemStatus)
				{
					return;
				}
				OrderItemStatusChangeLogModel orderItemStatusChangeLog = new OrderItemStatusChangeLogModel
				{
					OrderFileId = (int)clientOrderItem.Id,
					NewInternalStatus = (byte)internalOrderItemStatus,
					NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
					ChangeByContactId = changeByContactId,
					ChangeDate = DateTime.Now
				};

				if (previousLog != null)
				{
					orderItemStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
					orderItemStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
					orderItemStatusChangeLog.TimeDurationInMinutes = (int)orderItemStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes;
				}
			
					
				await _orderItemStatusChangeLogService.Insert(orderItemStatusChangeLog);
				
				
			}
			catch (Exception ex)
			{
				var loginUser = new LoginUserInfoViewModel
				{
					ContactId = AutomatedAppConstant.ContactId
				};
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					//PrimaryId = (int)order.Id,
					ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "AddOrderItemStatusChangeLog",
					RazorPage = "FtpOrderProcessService",
					Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
				};
				await _activityAppLogService.InsertAppErrorActivityLog(activity);
			}

		}

		public async Task AddOrderItemListModelStatusChangeLog(ClientOrderItemListModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus, int changeByContactId)
		{
			try
			{
				var previousLog = await _orderItemStatusChangeLogService.OrderItemStatusLastChangeLogByOrderFileId((int)clientOrderItem.Id);

				if (previousLog != null && previousLog.NewInternalStatus == (byte)internalOrderItemStatus)
				{
					return;
				}
				OrderItemStatusChangeLogModel orderItemStatusChangeLog = new OrderItemStatusChangeLogModel
				{
					OrderFileId = (int)clientOrderItem.Id,
					NewInternalStatus = (byte)internalOrderItemStatus,
					NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
					ChangeByContactId = changeByContactId,
					ChangeDate = DateTime.Now
				};

				if (previousLog != null)
				{
					orderItemStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
					orderItemStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
					orderItemStatusChangeLog.TimeDurationInMinutes = (int)orderItemStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes;
				}


				await _orderItemStatusChangeLogService.Insert(orderItemStatusChangeLog);


			}
			catch (Exception ex)
			{
				var loginUser = new LoginUserInfoViewModel
				{
					ContactId = AutomatedAppConstant.ContactId
				};
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					//PrimaryId = (int)order.Id,
					ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "AddOrderItemStatusChangeLog",
					RazorPage = "FtpOrderProcessService",
					Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
				};
				await _activityAppLogService.InsertAppErrorActivityLog(activity);
			}

		}
	}
}
