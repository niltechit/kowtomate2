using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.AutomationAppServices.FtpOrderProcess;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.OrderTeamServices;
using CutOutWiz.Services.BLL.AssignOrderAndItem;
using CutOutWiz.Services.Management;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.OrderWorkFlowAutomationServices
{
    public class OrderWorkFlowAutomationService : IOrderWorkFlowAutomationService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IAutoOrderDeliveryService _autoOrderDeliveryService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        private readonly IOrderTeamService _orderTeamService;
        private readonly ICompanyTeamManager _companyTeamService;
        private readonly IAssingOrderItemService _assingOrderItemService;
        private readonly ITeamMemberService _teamMemberService;
        public OrderWorkFlowAutomationService (
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            ICompanyGeneralSettingManager companyGeneralSettingService,
            IClientOrderItemService clientOrderItemService,
            IAutoOrderDeliveryService autoOrderDeliveryService,
            IUpdateOrderItemBLLService updateOrderItemBLLService,
            IOrderTeamService orderTeamService,
            ICompanyTeamManager companyTeamService,
            IAssingOrderItemService assingOrderItemService,
            ITeamMemberService teamMemberService
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
            _companyGeneralSettingService= companyGeneralSettingService;
            _clientOrderItemService = clientOrderItemService;
            _autoOrderDeliveryService = autoOrderDeliveryService;
            _updateOrderItemBLLService = updateOrderItemBLLService;
            _orderTeamService = orderTeamService;
            _companyTeamService = companyTeamService;
            _assingOrderItemService = assingOrderItemService;
            _teamMemberService = teamMemberService;
    }

        public async Task<Response<bool>> AutoAssignToTeam()
        {
            try
            {
                string query = $"SELECT cgs.* from  dbo.CompanyGeneralSettings cgs where cgs.AutoAssignOrderToTeam = 1";
                
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);
                
                if (companyGeneralSettings != null && !companyGeneralSettings.Any())
                {
                    return new Response<bool>();
                }

                foreach (var generalSetting in companyGeneralSettings)
                {
                    var orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.OrderPlaced, generalSetting.CompanyId);
                    foreach (var order in orders)
                    {
                        if (order.AssignedTeamId != null && order.AssignedTeamId > 0)
                        {
                            await AssignOrderToTeam(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {               
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    //PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    PrimaryId = 0,
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ErrorMessage = ex.Message,
                    MethodName = "AutoAssignToTeam",
                    RazorPage = "OrderWorkFlowAutomationService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return new Response<bool>();
        }

        public async Task<Response<bool>> AutoDistributeToEditor()
        {
            try
            {
                string query = $"SELECT cgs.* from  dbo.CompanyGeneralSettings cgs where cgs.AutoDistributeToEditor = 1";
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);
                if (companyGeneralSettings != null && !companyGeneralSettings.Any())
                {
                    return new Response<bool>();
                }

                foreach (var companyGeneralSetting in companyGeneralSettings)
                {

                    //var orders = await _orderService.GetOrdersByOrderItemStatus(ftp.ClientCompanyId, "4");
                    var orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.Assigned, companyGeneralSetting.CompanyId);


                    //foreach (var order in orders)
                    //{
                    if (orders.Count > 0)
                    {
                        #region no need this code
                        //var companyTeam = await _companyTeamService.GetTeamByCompanyId((int)ftp.ClientCompanyId);
                        //var teamMemberList = await _teamMemberService.GetTeamActiveMemberListWithDetailsByTeamId(companyTeam.TeamId);

                        //Ignore Supporting Editor 
                        //teamMemberList.RemoveAll(tm => tm.ContactId == AutomatedAppConstant.SupportingEditorContactId);
                        #endregion
                        
                        await DistributionOrderItemBetweenEditors(orders);
                    }
                }

                //	var tempClientOrderFtps = await _fileServerService.GetAllClientFtp();
                //string query = $"SELECT cc.Id,cc.Host,cc.ClientCompanyId,cc.Port,cc.Username,cc.Password,cc.IsEnable,cc.OutputRootFolder,cc.InputRootFolder FROM [dbo].[Client_ClientOrderFtp] as cc inner join dbo.CompanyGeneralSettings as cgs on cc.ClientCompanyId = cgs.CompanyId where cc.IsEnable = 1 and cgs.AutoDistributeToEditor = 1 and cgs.FtpOrderPlacedAppNo={consoleAppId}";

                //foreach (var ftp in tempClientOrderFtps)
                //{
                //	//var orders = await _orderService.GetOrdersByOrderItemStatus(ftp.ClientCompanyId, "4");
                //	var orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.Assigned,(int)ftp.ClientCompanyId);


                //	//foreach (var order in orders)
                //	//{
                //	if (orders.Count > 0)
                //	{
                //		#region no need this code
                //		var companyTeam = await _companyTeamService.GetTeamByCompanyId((int)ftp.ClientCompanyId);
                //		var teamMemberList = await _teamMemberService.GetTeamActiveMemberListWithDetailsByTeamId(companyTeam.TeamId);

                //		//Ignore Supporting Editor 
                //		teamMemberList.RemoveAll(tm => tm.ContactId == AutomatedAppConstant.SupportingEditorContactId);
                //		#endregion
                //		await DistributionOrderItemBetweenEditors(orders);
                //	}

                //	//}
                //}
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
                    MethodName = "AutoDistributeToEditor",
                    RazorPage = "FtpOrderProcessService - VC - Console Application",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return new Response<bool>();
        }

        public async Task<Response<bool>> AutoQcPass(int consoleAppId)
        {
            try
            {
                //If company auto qc enable
                string query = $"SELECT cc.Id,cc.Host,cc.ClientCompanyId,cc.Port,cc.Username,cc.Password,cc.IsEnable,cc.OutputRootFolder,cc.InputRootFolder FROM [dbo].[Client_ClientOrderFtp] as cc inner join dbo.CompanyGeneralSettings as cgs on cc.ClientCompanyId = cgs.CompanyId where cc.IsEnable = 1 and cgs.AutoQcPass = 1 AND cgs.FtpOrderPlacedAppNo={consoleAppId}";
                var tempClientOrderFtps = await _fileServerService.GetAllClientFtpByQuery(query);

                foreach (var ftp in tempClientOrderFtps)
                {
                    //Get Client Order
                    var orders = await _orderService.GetOrdersByOrderItemStatus((int)ftp.ClientCompanyId, "9,10,13,14");

                    foreach (var order in orders)
                    {
                        string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.ClientOrderId=${order.Id} and ci.Status  in (9,10,13,14)";
                        var orderItems = await _clientOrderItemService.GetOrderItemByStatus(queryForOrderItem);

                        await _autoOrderDeliveryService.MoveAsCompletedOrder(orderItems, order);
                        await _updateOrderItemBLLService.UpdateOrderItemsStatus(orderItems, InternalOrderItemStatus.ReadyToDeliver);
                        await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
                    }

                }
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
                    MethodName = "AutoQcPass",
                    RazorPage = "FtpOrderProcessService - VC - Console Application",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            return new Response<bool>();
        }

        public async Task<Response<bool>> AutoOperationPass(int consoleAppId)
        {
            try
            {
                //If company auto ops pass enable
                string query = $"SELECT cc.Id,cc.Host,cc.ClientCompanyId,cc.Port,cc.Username,cc.Password,cc.IsEnable,cc.OutputRootFolder,cc.InputRootFolder FROM [dbo].[Client_ClientOrderFtp] as cc inner join dbo.CompanyGeneralSettings as cgs on cc.ClientCompanyId = cgs.CompanyId where cc.IsEnable = 1 and cgs.AutoOperationPass = 1  AND cgs.FtpOrderPlacedAppNo={consoleAppId}";
                var tempClientOrderFtps = await _fileServerService.GetAllClientFtpByQuery(query);

                foreach (var ftp in tempClientOrderFtps)
                {
                    CompanyGeneralSettingModel companyGeneralSetting = await _companyGeneralSettingService.GetAllGeneralSettingsByCompanyId((int)ftp.ClientCompanyId);
                    if (companyGeneralSetting == null || !companyGeneralSetting.AutoOperationPass)
                    {
                        continue;
                    }

                    //var orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.ReadyToDeliver, ftp.ClientCompanyId);
                    var orders = await _orderService.GetOrdersByOrderItemStatus((int)ftp.ClientCompanyId, "20");
                    foreach (var order in orders)
                    {
                        string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.ClientOrderId=${order.Id} and ci.Status = 20";
                        var orderItems = await _clientOrderItemService.GetOrderItemByStatus(queryForOrderItem);
                        await _updateOrderItemBLLService.UpdateOrderItemsStatus(orderItems, InternalOrderItemStatus.Delivered);

                        await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
                    }
                }
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
                    MethodName = "AutoOperationPass",
                    RazorPage = "OrderWorkFlowAutomationService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            return new Response<bool>();
        }

        #region Private Method

        private async Task DistributionOrderItemBetweenEditors(List<ClientOrderModel> orders)
        {
            try
            {
                foreach (var order in orders)
                {
                    //if(orderItemLimit> 0)
                    //{
                    var teamMemberList = new List<TeamMemberListModel>();
                    var companyTeam = await _companyTeamService.GetTeamByCompanyId(order.CompanyId);

                    if (companyTeam != null)
                    {
                        teamMemberList = await _teamMemberService.GetTeamActiveMemberListWithDetailsByTeamId(companyTeam.TeamId);
                        if (teamMemberList.Count > 0)
                        {
                            teamMemberList.RemoveAll(tm => tm.ContactId == AutomatedAppConstant.SupportingEditorContactId);
                        }
                    }
                    else
                    {
                        return;
                    }

                    //string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.ClientOrderId=${order.Id} and ci.Status  in ({(int)InternalOrderItemStatus.Assigned})";
                    string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.ClientOrderId=${order.Id} and ci.Status = {(int)InternalOrderItemStatus.Assigned} and ci.FileGroup = {(int)OrderItemFileGroup.Work}";
                    var clientOrderItems = await _clientOrderItemService.GetOrderItemByStatus(queryForOrderItem);
                    //image distribute among editors
                    int totalOrderItems = clientOrderItems.Count;
                    int totalNumberOfEditors = teamMemberList.Count;
                    int itemsPerEditor = totalOrderItems / totalNumberOfEditors;

                    var skibOrderItem = 0;
                    for (int i = 0; i < totalNumberOfEditors; i++)
                    {
                        var teamMember = teamMemberList.ElementAt(i);
                        int EditorCapacity = await GetEditorCapacity(teamMember.ContactId); //Get Capacity
                        if (EditorCapacity > 0)
                        {
                            var editorNumberOfItem = itemsPerEditor;
                            if (EditorCapacity < itemsPerEditor)
                            {
                                editorNumberOfItem = EditorCapacity;
                            }

                            var items = clientOrderItems.Skip(skibOrderItem).Take(editorNumberOfItem).ToList();


                            await _assingOrderItemService.AssignOrderItemToEditor(items, teamMember.ContactId);
                            await _updateOrderItemBLLService.UpdateOrderItemsStatus(items, InternalOrderItemStatus.Distributed);
                            await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);

                            skibOrderItem += editorNumberOfItem;
                        }
                    }

                    //Extra fractional image distribute among editors

                    int firsltyTotalImageTakenByEditors = totalNumberOfEditors * itemsPerEditor;
                    var extraOrderItems = clientOrderItems.Skip(firsltyTotalImageTakenByEditors).ToList();
                    int totalExtraImage = extraOrderItems.Count;

                    if (totalExtraImage != 0)
                    {

                        Random random = new Random();
                        var extraTeamMeberList = teamMemberList.OrderBy(x => random.Next()).Take(totalExtraImage).ToList();
                        for (int i = 0; i < totalExtraImage; i++)
                        {

                            var teamMember = extraTeamMeberList[i];
                            var tempOrderItems = extraOrderItems[i];

                            await _assingOrderItemService.AssignOrderItemToEditor(new List<ClientOrderItemModel> { tempOrderItems }, teamMember.ContactId);
                            await _updateOrderItemBLLService.UpdateOrderItemStatus(tempOrderItems, InternalOrderItemStatus.Distributed);
                            await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);

                        }
                    }


                }

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
                    MethodName = "DistributionOrderItemBetweenEditors",
                    RazorPage = "FtpOrderProcessService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
        }

        private async Task AssignOrderToTeam(ClientOrderModel order)
        {
            try
            {
                await _orderTeamService.AssignOrderToTeam(order); //  Assign Order To Client

                await _orderService.UpdateClientOrder(order); //  Update New Order After Assign


                //Update Order Item After Assign
                await _updateOrderItemBLLService.UpdateOrderItemTeamId(order);
                string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.ClientOrderId=${order.Id} and ci.Status  in ({(int)InternalOrderItemStatus.OrderPlaced})";
                var clientOrderItems = await _clientOrderItemService.GetOrderItemByStatus(queryForOrderItem);
                //var clientOrderItems = await _clientOrderItemService.GetAllByOrderId((int)order.Id);
                await _updateOrderItemBLLService.UpdateOrderItemsStatus(clientOrderItems, InternalOrderItemStatus.Assigned);
                await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId); //Update New Order Status After Assign To Team
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
                    MethodName = "AssignOrderToTeam",
                    RazorPage = "FtpOrderProcessService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
        }

        public async Task<int> GetEditorCapacity(int contactId)
        {
            DateTime now = DateTime.Now;
            DateTime currentShiftEndTime = new DateTime();
            if (now.Hour < 15 && now.Hour >= 7)
            {
                currentShiftEndTime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0); // 3 pm
            }
            else if (now.Hour < 23 && now.Hour >= 15)
            {
                currentShiftEndTime = new DateTime(now.Year, now.Month, now.Day, 23, 0, 0); // 11 pm
            }
            else if (now.Hour < 7 || now.Hour >= 23)
            {

                currentShiftEndTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0); // 7 am

                currentShiftEndTime = currentShiftEndTime.AddDays(1);
            }

            TimeSpan remainingTime = currentShiftEndTime - now;
            int remainingMinutes = (int)remainingTime.TotalMinutes;

            if (remainingMinutes > 60)
            {
                remainingMinutes = 60;
            }
            double editorCapability = remainingMinutes / AutomatedAppConstant.minPerImage;

            ClientOrderItemCount editorClientOrderItemCount = await _clientOrderItemService.GetTotalPrductionOngoingItem(contactId);

            int editorActualCapability = (int)editorCapability - editorClientOrderItemCount.TotalPrductionOngoingItem;

            return editorActualCapability;

        }
        #endregion

    }
}
