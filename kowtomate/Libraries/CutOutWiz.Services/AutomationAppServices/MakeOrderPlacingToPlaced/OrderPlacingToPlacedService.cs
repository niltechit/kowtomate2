using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using FluentFTP.Helpers;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.MakeOrderPlacingToPlaced
{
    public class OrderPlacingToPlacedService: IOrderPlacingToPlacedService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        public OrderPlacingToPlacedService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
        }
        public async Task<Response<bool>> MakeOrderStatusOrderPlacingToOrderPlaced(int consoleAppId)
        {

            var response = new Response<bool>();
            try
            {
                string query = $"SELECT cc.Id,cc.Host,cc.ClientCompanyId,cc.Port,cc.Username,cc.Password,cc.IsEnable,cc.OutputRootFolder,cc.InputRootFolder FROM [dbo].[Client_ClientOrderFtp] as cc inner join dbo.CompanyGeneralSettings as cgs on cc.ClientCompanyId = cgs.CompanyId where cc.IsEnable = 1 and cgs.EnableFtpOrderPlacement = 1 AND cgs.FtpOrderPlacedAppNo={consoleAppId}";
                var tempClientOrderFtps = await _fileServerService.GetAllClientFtpByQuery(query);
                foreach (var clientFtp in tempClientOrderFtps)
                {
                    if (AutomatedAppConstant.SixCompanyId == clientFtp.ClientCompanyId)
                    {
                        continue;
                    }

                    var orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.OrderPlacing, (int)clientFtp.ClientCompanyId);
                    foreach (var order in orders)
                    {
                        await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
                        await _orderStatusService.UpdateOrderArrivalTime(order);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.ConsoleAppId,
                    PrimaryId = consoleAppId,
                    ErrorMessage = $"ConsoleAppId: {consoleAppId}. Exception: {ex.Message}",
                    MethodName = $"MakeOrderStatusOrderPlacingToOrderPlaced",
                    RazorPage = "OrderPlacingToPlacedService",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                Console.WriteLine($"Error line no 375: {ex.Message}");
            }
            return response;

        }
    }
}
