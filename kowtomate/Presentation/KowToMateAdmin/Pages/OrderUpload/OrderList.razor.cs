using ClosedXML.Excel;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.Models.Message;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentFTP;
using KowToMateAdmin.Helper;
using KowToMateAdmin.Models;
using KowToMateAdmin.Models.FormModel;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Security;

namespace KowToMateAdmin.Pages.OrderUpload
{
    public partial class OrderList
    {

        [Parameter]
        public string paramCompanyObjectId { get; set; }

        protected ModalNotification ModalNotification { get; set; }
        RadzenDataGrid<ClientOrderListModel> grid;
        IEnumerable<ClientOrderListModel> orders;
        int totalOrderCount;
        int totalImageCount;
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50, 100, 500 };
        bool isLoading = false;

        ClientOrderFilter orderFilter = new ClientOrderFilter();
        private LoginUserInfoViewModel loginUser = null;
        private string selectedObjectId = "";
        List<CustomEnumTypes> statuses;
        List<CustomEnumTypes> categorySetStatus;

        List<CompanyModel> companies;
        List<ContactForDropdown> contacts;

        IEnumerable<int> selectedFilterCompanies;
        IEnumerable<int> selectedFilterContacts;
        IEnumerable<int> selectedFilterTeams;
        IEnumerable<byte> selectedFilterInternalStatuss;
        IEnumerable<byte> selectedFilterCategorySetStatus;

        List<CustomEnumTypes> externalStatuses;
        IEnumerable<byte> selectedFilterExternalStatuss;
        IList<ClientOrderListModel> selectedOrders = new List<ClientOrderListModel>();
        bool allowRowSelectOnRowClick = false;
        bool showCheckColumn = false;
        bool isAssignOrderSubmitting = false;
        private TeamModel team = new TeamModel();
        bool isAssignPopupVisible = false;
        List<int> selectedTeams = new List<int>();
        List<TeamModel> teams = new List<TeamModel>();
        List<InternalMessageNotification> internalMessagesNotificationsForOps = new List<InternalMessageNotification>();
        List<InternalMessageNotification> internalMessagesNotificationsForTeam = new List<InternalMessageNotification>();
        List<CompanyTeamModel> companyTeams = new List<CompanyTeamModel>();
        int teamId = 0;
        AuthenticationState authState;
        //Progress Bar
        private double maxValue;
        private double CurrentValue = 0;
        bool isSubmitting = false;
        bool spinShow = false;
        private List<OrderStatusChangeLogModel> orderStatusChangeLogs { get; set; } = new List<OrderStatusChangeLogModel>();
        bool isOrderChangeLogPopupVisible = false;
        private int? TotalMinutesForOrder = 0;
        bool isProgressBar = false;
        bool assignOrderToTeam = false;
        CssHelper cssHelper = new CssHelper();
        private ContactModel contact = new ContactModel();

        public bool isExporting = false;
        List<CustomTableColumnModel> tableColumns;

        string message = "";
        bool isShowMessage = false;

		bool isOrderDeadlinePopupVisible=false;
        bool isOrderExpectedDeliveryDatePopupVisible = false;
        public ClientOrderListModel ClientOrder = new ClientOrderListModel();
        public DateTime? OrderExpectedDeliveryDate = null;
        private List<CutOutWiz.Services.Models.ClientCategoryServices.ClientCategoryModel> commonCategories = new List<CutOutWiz.Services.Models.ClientCategoryServices.ClientCategoryModel>();
        private int commonCatogoryId = 0;
        private bool isOrderCategorySetUpPopupVisible = false;
        private OrderWiseCategoryModel orderWiseCategory = new OrderWiseCategoryModel();
        bool isOrderAssignToTeamVisible = false;
        OrderAssignToTeam orderAssignToTeam = new OrderAssignToTeam();
        #region FIlter
        RadzenDataFilter<ClientOrderListModel> dataFilter;
        private bool isShowTopFilter = true;
        #endregion


        void OnColumnResized(DataGridColumnResizedEventArgs<ClientOrderListModel> args)
        {
            Console.WriteLine($"Resized {args.Column.Title} to {args.Width} pixels");
        }
        protected override async Task OnInitializedAsync()
        {
            authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            loginUser = _workContext.LoginUserInfo;



            statuses = new List<CustomEnumTypes>();
            categorySetStatus = new List<CustomEnumTypes>();

            foreach (InternalOrderStatus item in Enum.GetValues(typeof(InternalOrderStatus)))
            {
                statuses.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }

            externalStatuses = new List<CustomEnumTypes>();
            foreach (ExternalOrderStatus item in Enum.GetValues(typeof(ExternalOrderStatus)))
            {
                externalStatuses.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }

            foreach (OrderCategorySetStatus item in Enum.GetValues(typeof(OrderCategorySetStatus)))
            {
                categorySetStatus.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }
            //
            companies = await _companyService.GetAllClientCompany();
            teams = await _teamService.GetAll();
            //companyTeams = await _companyTeamService.GetByCompanyId(loginUser.CompanyId);

            if (loginUser.CompanyType != (int)CompanyType.Client)
            {
                contacts = await _contactManager.GetContactsForDropdownByCompanyId(loginUser.CompanyId);
            }

            await ShowHideCheckColumn();

            contact = await _contactManager.GetById(loginUser.ContactId);
            tableColumns = _clientOrderService.GetAllTableColumns();

        }

        async Task ReloadGrid()
        {
            await grid.Reload();
        }


        //
        private void OnRowRender(RowRenderEventArgs<ClientOrderListModel> args)
        {
            if (args.Data.ExpectedDeliveryDate != null)
            {
                //var arrivalTimePlus1_4Hours = args.Data.ArrivalTime.Value.AddHours(AutomatedAppConstant.VcDeadLineInHour);
                var expectedDeliveryDate = args.Data.ExpectedDeliveryDate ?? DateTime.Now;
                var timeLeft = expectedDeliveryDate - DateTime.Now;
                var minLeft = (int)timeLeft.TotalMinutes;

                var dangerWarningTime = (args.Data.DeliveryDeadlineInMinute * AutomatedAppConstant.dangerWarningPercent) / 100;
                var dangerTime = (args.Data.DeliveryDeadlineInMinute * AutomatedAppConstant.dangerPercent) / 100;



                if ((args.Data.InternalOrderStatus < (int)InternalOrderStatus.Completed || args.Data.InternalOrderStatus == (int)InternalOrderStatus.OrderPlacing) && args.Data.InternalOrderStatus != 0)
                {
                    if (minLeft <= dangerWarningTime && minLeft > dangerTime)
                    {
                        args.Attributes["class"] = "row-warning";
                    }
                    else if (minLeft > 0 && minLeft <= dangerTime)
                    {
                        args.Attributes["class"] = "row-warning-danger";
                    }
                    else if (minLeft <= 0)
                    {
                        args.Attributes["class"] = "row-failed";
                    }
                }
                //StateHasChanged();
            }
        }

        async Task ResetGrid()
        {
            selectedFilterCompanies = null;
            selectedFilterInternalStatuss = null;
            selectedFilterExternalStatuss = null;
            //--Not Implemented
            selectedFilterContacts = null;
            selectedFilterTeams = null;
            StateHasChanged();
            teams = await _teamService.GetAll();

            grid.Reset(true);

            await grid.FirstPage(true);

        }

        //async Task Reset()
        //{
        //	grid.Reset(true);
        //	await grid.FirstPage(true);
        //}

        private async Task ShowHideCheckColumn()
        {
            await Task.Yield();
            if (authState.User.IsInRole(PermissionConstants.AssignNewOrderToTeam))
            {
                showCheckColumn = true;
            }
        }

        private async Task RowDoubleClick(DataGridRowMouseEventArgs<ClientOrderListModel> args)
        {
            await Task.Yield();
            NavigationManager.NavigateTo($"/order/Details/{args.Data.ObjectId}", true);
        }

        async Task OnSelectedCompanyChange(object value)
        {
            try
            {
                if (selectedFilterCompanies != null && !selectedFilterCompanies.Any())
                {
                    selectedFilterCompanies = null;
                }

                await grid.FirstPage(true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnSelectedCompanyChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task OnSelectedContactNameChange(object value)
        {
            try
            {
                if (selectedFilterContacts != null && !selectedFilterContacts.Any())
                {
                    selectedFilterContacts = null;
                }

                await grid.FirstPage(true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnSelectedContactNameChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task OnSelectedTeamNameChange(object value)
        {
            try
            {
                if (selectedFilterTeams != null && !selectedFilterTeams.Any())
                {
                    selectedFilterTeams = null;
                }

                await grid.FirstPage(true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnSelectedTeamNameChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task OnSelectedExternalOrderStatusChange(object value)
        {
            try
            {
                if (selectedFilterExternalStatuss != null && !selectedFilterExternalStatuss.Any())
                {
                    selectedFilterExternalStatuss = null;
                }

                await grid.FirstPage(true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnSelectedExternalOrderStatusChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task OnSelectedInternalOrderStatusChange(object value)
        {
            try
            {
                if (selectedFilterInternalStatuss != null && !selectedFilterInternalStatuss.Any())
                {
                    selectedFilterInternalStatuss = null;
                }

                await grid.FirstPage(true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnSelectedInternalOrderStatusChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task OnSelectedInternalOrderSetStatusChange(object value)
        {
            try
            {
                if (selectedFilterCategorySetStatus != null && !selectedFilterCategorySetStatus.Any())
                {
                    selectedFilterCategorySetStatus = null;
                }

                await grid.FirstPage(true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnSelectedInternalOrderSetStatusChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task LoadData(LoadDataArgs args)
        {
            try
            {
                isLoading = true;
                orderFilter.Top = args.Top ?? 20;
                orderFilter.Skip = args.Skip ?? 0;
                orderFilter.IsCalculateTotal = true;
                orderFilter.Where = await PrepareFilterQuery(args.Filters, dataFilter.Filters);

                //Add sorting
                PopulateSortFilterQuery(orderFilter, args.Sorts);

                orders = await _orderService.GetOrderByFilterWithPaging(orderFilter);

                if (orders != null && orders.Any())
                {
                    foreach (var order in orders)
                    {
                        ContactListModel contactVM = await _teamService.GetByContactId(loginUser.ContactId);

                        if (contactVM != null && contactVM.TeamId == AutomatedAppConstant.VcTeamId && authState.User.IsInRole(PermissionConstants.Order_ViewAllTeamOrders))
                        {
                            //TODO: return count only
                            string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.CompanyId=${order.CompanyId} and ci.Status  in (4)";
                            var orderItems = await _clientOrderItemService.GetOrderItemByStatus(queryForOrderItem);

                            if (orderItems.Count > 0)
                            {
                                message = $"Your VC Team {orderItems.Count} Items still not distributed ! May be your capacity is full now";
                                isShowMessage = true;
                            }
                        }
                    }
                }
                else
                {
                    orders = new List<ClientOrderListModel>();
                }
                //limited message

                totalOrderCount = orderFilter.TotalCount;
                totalImageCount = orderFilter.TotalImageCount;

                isLoading = false;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "LoadData",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task<string> PrepareFilterQuery(IEnumerable<FilterDescriptor> filters, IEnumerable<CompositeFilterDescriptor> topFilters)
        {
            string where = string.Empty;
            string and = string.Empty;

            //Set group filter
            var items = string.Empty;
            var query = "";

            try
            {
                //Set permisison
                if (loginUser.CompanyType == (int)CompanyType.Client)
                {
                    where = $"{where}{and} o.[CompanyId] = {loginUser.CompanyId} and o.IsDeleted = 0 and o.InternalOrderStatus != {(int)InternalOrderStatus.OrderPlacing} ";
                    and = " AND ";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(paramCompanyObjectId))
                    {
                        var companySelectedOnParam = await _companyService.GetByObjectId(paramCompanyObjectId);

                        if (companySelectedOnParam == null)
                        {
                            NavigationManager.NavigateTo("/", true);
                            return null;
                        }
                        where = $"{where}{and} o.[CompanyId] = {companySelectedOnParam.Id} ";
                        and = " AND ";
                    }

                    if (authState.User.IsInRole(PermissionConstants.Order_ViewAllClientOrders))
                    {
                        //query = $"LEFT JOIN [dbo].[Order_Assigned_Team] AT WITH(NOLOCK) on AT.OrderId=o.Id left JOIN [dbo].Security_Contact CON WITH(NOLOCK) ON CON.Id=AT.[CreatedBy] left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=AT.TeamId LEFT JOIN Management_TeamMember MT WITH(NOLOCK) ON MT.TeamId=AT.TeamId";
                        var addWhereClause = $"o.IsDeleted = 0";
                        where = $"{addWhereClause}{and}{where}";
                    }
                    else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllTeamOrders))
                    {
                        where = $@"{where}{and} o.IsDeleted = 0 and  o.Id IN (
                    SELECT a.OrderId FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
                    WHERE m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1) ";
                        and = " AND ";
                    }
                    else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllQCOrders))
                    {
                        where = $@"{where}{and} o.IsDeleted = 0 and  o.Id IN (
                    SELECT a.OrderId FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
                    Inner Join Order_ClientOrderItem ci WITH(NOLOCK) ON ci.ClientOrderId = a.OrderId
                    WHERE m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1 and ci.Status  in (9,10,13,14,20))";
                        and = " AND ";
                    }
                    else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllAssignedOrders))
                    {
                        var ExceptCompletedOrder = "o.IsDeleted = 0 and o.InternalOrderStatus<>26 and";
                        where = $@"{where} {ExceptCompletedOrder} {and}o.Id IN (
                    SELECT ed.OrderId FROM dbo.Order_AssignedImageEditor ed WITH(NOLOCK)
                    WHERE ed.AssignContactId = {loginUser.ContactId}  and IsActive = 1 )";
                        and = " AND ";
                    }
                    else  //Default team orders
                    {
                        
                        where = $@"{where}{and} o.IsDeleted = 0 and o.Id IN (
                    SELECT ed.OrderId FROM dbo.Order_AssignedImageEditor ed WITH(NOLOCK)
                    WHERE ed.AssignContactId = {loginUser.ContactId} )";
                        and = " AND ";
                    }

                    //else if (authState.User.IsInRole(PermissionContants.Order_ViewAllProductionDoneOrder))
                    //{
                    //	var teamMember =await _teamMemberService.GetByContactId(loginUser.ContactId);
                    //	if(teamMember != null)
                    //	{
                    //		query = $"inner join dbo.Order_Assigned_Team as orderassignteam on o.Id = orderassignteam.OrderId";
                    //		where = $"{where}{and} o.InternalOrderStatus = {(byte)InternalOrderStatus.ProductionDone} and orderassignteam.TeamId = {teamMember.TeamId}";
                    //		and = " AND ";
                    //	}
                    //}

                    //else
                    //{
                    //	where = "notFound";
                    //}
                }

                if (selectedFilterCompanies != null && selectedFilterCompanies.Any())
                {
                    if (selectedFilterCompanies.Count() == 1)
                    {
                        and = " AND ";
                        where = $"{where}{and} o.[CompanyId] = {selectedFilterCompanies.FirstOrDefault()} ";
                        
                    }
                    else //count > 1
                    {
                        and = " AND ";
                        items = GetCommaSeperatedIntItems(selectedFilterCompanies);
                        where = $"{where}{and} o.[CompanyId] in ({items})";
                        
                    }
                }

                //For admin Company
                if (selectedFilterInternalStatuss != null && selectedFilterInternalStatuss.Any())
                {
                    if (selectedFilterInternalStatuss.Count() == 1)
                    {
                        and = " AND ";
                        where = $"{where}{and} o.[InternalOrderStatus] = {selectedFilterInternalStatuss.FirstOrDefault()} ";

                    }
                    else //count > 1
                    {
                        and = " AND ";
                        items = GetCommaSeperatedByteItems(selectedFilterInternalStatuss);
                        where = $"{where}{and} o.[InternalOrderStatus] in ({items})";

                    }
                }

                //For Client
                if (selectedFilterExternalStatuss != null && selectedFilterExternalStatuss.Any())
                {
                    if (selectedFilterExternalStatuss.Count() == 1)
                    {
                        and = " AND ";
                        where = $"{where}{and} o.[ExternalOrderStatus] = {selectedFilterExternalStatuss.FirstOrDefault()} ";

                    }
                    else //count > 1
                    {
                        and = " AND ";
                        items = GetCommaSeperatedByteItems(selectedFilterExternalStatuss);
                        where = $"{where}{and} o.[ExternalOrderStatus] in ({items})";

                    }
                }

                //For Category Set Status
                if (selectedFilterCategorySetStatus != null && selectedFilterCategorySetStatus.Any())
                {
                    if (selectedFilterCategorySetStatus.Count() == 1)
                    {
                        and = " AND ";
                        where = $"{where}{and} o.[CategorySetStatus] = {selectedFilterCategorySetStatus.FirstOrDefault()} ";

                    }
                    else //count > 1
                    {
                        and = " AND ";
                        items = GetCommaSeperatedByteItems(selectedFilterCategorySetStatus);
                        where = $"{where}{and} o.[CategorySetStatus] in ({items})";

                    }
                }

                //TeamName
                if (selectedFilterTeams != null && selectedFilterTeams.Any())
                {
                    if (selectedFilterTeams.Count() == 1)
                    {
                        and = " AND ";
                        where = $"{where}{and} o.[AssignedTeamId] = {selectedFilterTeams.FirstOrDefault()} ";
                    }
                    else //count > 1
                    {
                        and = " AND ";
                        items = GetCommaSeperatedIntItems(selectedFilterTeams);
                        where = $"{where}{and} o.[AssignedTeamId] in ({items})";
                    }
                }

                //Assign by ContactName
                if (selectedFilterContacts != null && selectedFilterContacts.Any())
                {
                    if (selectedFilterContacts.Count() == 1)
                    {
                        and = " AND ";
                        where = $"{where}{and} o.[AssignedByOpsContactId] = {selectedFilterContacts.FirstOrDefault()} ";
                    }
                    else //count > 1
                    {
                        and = " AND ";
                        items = GetCommaSeperatedIntItems(selectedFilterContacts);
                        where = $"{where}{and} o.[AssignedByOpsContactId] in ({items})";
                    }
                }

                if (filters != null && filters.Any())
                {
                    foreach (var filterItem in filters)
                    {
                        //Not in database need to add those
                        //Check boxes
                        if (filterItem.Property == "OrderPlaceDate" || filterItem.Property == "OrderPlaceDateOnly"
                            || filterItem.Property == "ExpectedDeliveryDate"
                            || filterItem.Property == "DeliveredDate"
                            || filterItem.Property == "CreatedDate"
                            || filterItem.Property == "UpdatedDate"
                        )
                        {
                            GetDateFilterQueryForGridHeader(ref where, ref and, filterItem);
                            //PopulateDateFiltersForGridHeader(ref where, ref and, filterItem);
                        }
                        else if (filterItem.Property == "NumberOfImage"
                        )
                        {
                            PopulateNumberFiltersForGridHeader(ref where, ref and, filterItem);
                        }
                        else if (filterItem.Property == "IsDeleted"
                        )
                        {
                            and = " AND ";
                            where = $"{where}{and} [{filterItem.Property}] = ''{filterItem.FilterValue.ToString()}''";

                        }  //Date time fields
                        else  // String fields
                        {
                            and = " AND ";
                            PopulateStringFiltersForGridHeader(ref where, ref and, filterItem);
                        }
                    }
                }

                if (topFilters != null && topFilters.Any())
                {
                    var topLogicalFilterOperator = dataFilter.LogicalFilterOperator.ToString();
                    var topWhere = PopulateTopFiltersQuery(topFilters, topLogicalFilterOperator);

                    if (!string.IsNullOrWhiteSpace(topWhere))
                    {
                        and = " AND ";
                        where = $"{where}{and} ({topWhere})";
                    }
                    //string serializedString = System.Text.Json.JsonSerializer.Serialize(topFilters);
                    //var userCopy = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CompositeFilterDescriptor>>(serializedString);
                }

                if (!string.IsNullOrWhiteSpace(where))
                    where = $" WHERE {where}";
                if (!string.IsNullOrWhiteSpace(query))
                    where = $"{query} {where}";

                if (string.IsNullOrWhiteSpace(where))
                    where = $"{where}";
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "PrepareFilterQuery",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return where;
        }

        private string PopulateTopFiltersQuery(IEnumerable<CompositeFilterDescriptor> topFilters, string logicalOperator)
        {
            string where = "";
            var and = "";
            foreach (var filterItem in topFilters)
            {
                if (filterItem.Filters != null && filterItem.Filters.Any())
                {
                    var innerWhere = PopulateTopFiltersQuery(filterItem.Filters, filterItem.LogicalFilterOperator.ToString());
                    where = $"{where} {and} ({innerWhere})";
                    and = $" {logicalOperator} ";
                    continue;
                }

                if (filterItem.Property == "OrderPlaceDate" || filterItem.Property == "OrderPlaceDateOnly"
                            || filterItem.Property == "ExpectedDeliveryDate"
                            || filterItem.Property == "DeliveredDate"
                            || filterItem.Property == "CreatedDate"
                            || filterItem.Property == "UpdatedDate"
                            || filterItem.Property == "TeamAssignedDate"
                            //|| filterItem.Property == "InternalFileInputPath"
						)
                {
                    var actualFiledName = GetActualFieldName(filterItem.Property);
                    PopulateDateFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                    //PopulateDateFiltersForGridHeader(ref where, ref and, filterItem);
                }
                else if (filterItem.Property == "NumberOfImage")
                {
                    var actualFiledName = GetActualFieldName(filterItem.Property);
                    PopulateNumberFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }
                else if (filterItem.Property == "IsDeleted"
                )
                {
                    var actualFiledName = GetActualFieldName(filterItem.Property);
                    where = $"{where}{and} [{filterItem.Property}] = ''{filterItem.FilterValue.ToString()}''";
                    and = $" {logicalOperator} ";
                }  //Date time fields
                else if (filterItem.Property == "ImageName")
                {
                    var actualFiledName = "";
                    PopulateFileNameStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }  //Filter Path
                else if (filterItem.Property == "InternalFileInputPath")
                {
					var actualFiledName = GetActualFieldName(filterItem.FilterValue.ToString());
					PopulateInternalFileInputPathFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }  //Date time fields
                else  // String fields
                {
                    var actualFiledName = GetActualFieldName(filterItem.Property);
                    PopulateStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }
            }

            return where;
        }

        private void GetDateFilterQueryForGridHeader(ref string where, ref string and, FilterDescriptor filterItem)
        {
            try
            {

                var firstQuery = GetDateFilterQuery(filterItem.FilterValue, filterItem.FilterOperator, filterItem.Property);

                if (!string.IsNullOrWhiteSpace(firstQuery))
                {
                    var secondQuery = GetDateFilterQuery(filterItem.SecondFilterValue, filterItem.SecondFilterOperator, filterItem.Property);

                    if (string.IsNullOrWhiteSpace(secondQuery))
                    {
                        where = $"{where}{and} {firstQuery}";
                        //Only first filter
                    }
                    else
                    {
                        where = $"{where}{and} ({firstQuery} {filterItem.LogicalFilterOperator.ToString()} {secondQuery})";
                        //First and second filter
                    }

                    and = " AND ";
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "PopulateDateFiltersForGridHeader",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
        }


        private string GetDateFilterQuery(Object FilterValue, FilterOperator filterOperator, string property)
        {
            if (!string.IsNullOrEmpty(FilterValue?.ToString()))
            {
                if (filterOperator == FilterOperator.Equals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {GetActualFieldName(property)}), 0) = ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.NotEquals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {GetActualFieldName(property)}), 0) <> ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.LessThan)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {GetActualFieldName(property)}), 0) < ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.LessThanOrEquals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {GetActualFieldName(property)}), 0) <= ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.GreaterThan)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {GetActualFieldName(property)}), 0) > ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {GetActualFieldName(property)}), 0) >= ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
            }
            else
            {
                if (filterOperator == FilterOperator.IsNull)
                {
                    return $"{GetActualFieldName(property)} IS NULL";
                }
                else if (filterOperator == FilterOperator.IsNotNull)
                {
                    return $"{GetActualFieldName(property)} IS NOT NULL";
                }
            }

            return "";
        }

        private void PopulateNumberFiltersForGridHeader(ref string where, ref string and, FilterDescriptor filterItem)
        {
            try
            {
                if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
                {
                    if (filterItem.FilterOperator == FilterOperator.Equals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} = {filterItem.FilterValue}";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} <> {filterItem.FilterValue}";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.LessThan)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} < {filterItem.FilterValue}";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} <= {filterItem.FilterValue}";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} > {filterItem.FilterValue}";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} >= {filterItem.FilterValue}";
                        and = " AND ";
                    }
                }
                else
                {
                    if (filterItem.FilterOperator == FilterOperator.IsNull)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} IS NULL";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} IS NOT NULL";
                        and = " AND ";
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "PopulateNumberFiltersForGridHeader",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
        }

        private void PopulateStringFiltersForGridHeader(ref string where, ref string and, FilterDescriptor filterItem)
        {
            try
            {
                if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
                {
                    if (filterItem.FilterOperator == FilterOperator.Contains)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} like ''%{filterItem.FilterValue}%''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} NOT like ''%{filterItem.FilterValue}%''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.StartsWith)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} like ''{filterItem.FilterValue}%''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.EndsWith)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)}  like ''%{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.Equals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} <> ''{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.LessThan)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} < ''{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} <= ''{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} > ''{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                    {
                        where = $"{where}{and} {GetActualFieldName(filterItem.Property)} >= ''{filterItem.FilterValue}''";
                        and = " AND ";
                    }
                }
                else
                {
                    if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
                    {
                        where = $"{where}{and} ({GetActualFieldName(filterItem.Property)} = '''' OR {GetActualFieldName(filterItem.Property)} IS NULL)";
                        and = " AND ";
                    }
                    else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
                    {
                        where = $"{where}{and} ({GetActualFieldName(filterItem.Property)} <> '''' AND {GetActualFieldName(filterItem.Property)} IS NOT NULL)";
                        and = " AND ";
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "PopulateStringFiltersForGridHeader",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
        }

        void PopulateSortFilterQuery(ClientOrderFilter filter, IEnumerable<SortDescriptor> sorts)
        {
            try
            {
                //Set group filter
                if (sorts != null && sorts.Any())
                {
                    var sortColumn = sorts.FirstOrDefault();
                    filter.SortColumn = GetActualFieldName(sortColumn.Property);

                    if (sortColumn.SortOrder == SortOrder.Ascending)
                    {
                        filter.SortDirection = "ASC";
                    }
                    else
                    {
                        filter.SortDirection = "DESC";
                    }
                }
                else
                {
                    filter.SortColumn = "";
                    filter.SortDirection = "";
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "PopulateSortFilterQuery",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
        }


        #region Top Filter
        public void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) = ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <> ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) < ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) > ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) >= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = $" {logicalOperator} ";
                }
            }
        }
        public void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = $" {logicalOperator} ";
                }
            }
        }

        public void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                var filterValue = filterItem.FilterValue.ToString().Replace("'", "''+ CHAR(39)+''");

                if (filterItem.FilterOperator == FilterOperator.Contains)
                {
                    where = $"{where}{and} {actualFieldName} like ''%{filterValue}%''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
                {
                    where = $"{where}{and} {actualFieldName} NOT like ''%{filterValue}%''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.StartsWith)
                {
                    where = $"{where}{and} {actualFieldName} like ''{filterValue}%''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.EndsWith)
                {
                    where = $"{where}{and} {actualFieldName}  like ''%{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} ({actualFieldName} = '''' OR {actualFieldName} IS NULL)";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} ({actualFieldName} <> '''' AND {actualFieldName} IS NOT NULL)";
                    and = $" {logicalOperator} ";
                }
            }
        }

        public void PopulateFileNameStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            actualFieldName = "(o.[Id] IN (select i.ClientOrderId FROM [dbo].[Order_ClientOrderItem] i WITH(NOLOCK) WHERE i.FileName";

            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                var filterValue = filterItem.FilterValue.ToString().Replace("'", "''+ CHAR(39)+''");

                if (filterItem.FilterOperator == FilterOperator.Contains)
                {
                    where = $"{where}{and} {actualFieldName} like ''%{filterValue}%''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
                {
                    where = $"{where}{and} {actualFieldName} NOT like ''%{filterValue}%''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.StartsWith)
                {
                    where = $"{where}{and} {actualFieldName} like ''{filterValue}%''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.EndsWith)
                {
                    where = $"{where}{and} {actualFieldName}  like ''%{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = ''{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> ''{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < ''{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= ''{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > ''{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= ''{filterValue}''))";
                    and = $" {logicalOperator} ";
                }
            }
            //else
            //{
            //	if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
            //	{
            //		where = $"{where}{and} ({actualFieldName} = '''' OR {actualFieldName} IS NULL)";
            //		and = $" {logicalOperator} ";
            //	}
            //	else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
            //	{
            //		where = $"{where}{and} ({actualFieldName} <> '''' AND {actualFieldName} IS NOT NULL)";
            //		and = $" {logicalOperator} ";
            //	}
            //}
        }
		public void PopulateInternalFileInputPathFiltersForGridHeader(string InternalFileInputPath, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
		{
			InternalFileInputPath = "(o.[Id] IN (select i.ClientOrderId FROM [dbo].[Order_ClientOrderItem] i WITH(NOLOCK) WHERE i.InternalFileInputPath";

			if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
			{
				var filterValue = filterItem.FilterValue.ToString().Replace("'", "''+ CHAR(39)+''");

				if (filterItem.FilterOperator == FilterOperator.Contains)
				{
					where = $"{where}{and} {InternalFileInputPath} like ''%{filterValue}%''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
				{
					where = $"{where}{and} {InternalFileInputPath} NOT like ''%{filterValue}%''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.StartsWith)
				{
					where = $"{where}{and} {InternalFileInputPath} like ''{filterValue}%''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.EndsWith)
				{
					where = $"{where}{and} {InternalFileInputPath}  like ''%{filterValue}''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.Equals)
				{
					where = $"{where}{and} {InternalFileInputPath} = ''{filterValue}''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.NotEquals)
				{
					where = $"{where}{and} {InternalFileInputPath} <> ''{filterValue}''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.LessThan)
				{
					where = $"{where}{and} {InternalFileInputPath} < ''{filterValue}''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
				{
					where = $"{where}{and} {InternalFileInputPath} <= ''{filterValue}''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
				{
					where = $"{where}{and} {InternalFileInputPath} > ''{filterValue}''))";
					and = $" {logicalOperator} ";
				}
				else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
				{
					where = $"{where}{and} {InternalFileInputPath} >= ''{filterValue}''))";
					and = $" {logicalOperator} ";
				}
			}
		}

		#endregion

		string GetActualFieldName(string uiFieldName)
        {
            try
            {
                if (uiFieldName == "OrderPlaceDate" || uiFieldName == "OrderPlaceDateOnly")
                {
                    return "o.[OrderPlaceDate]";
                }
                else if (uiFieldName == "CompanyName")
                {
                    return "c.[Name]";
                }
                else if (uiFieldName == "ContactName")
                {
                    return "assignby.[FirstName]";
                }
                else if (uiFieldName == "TeamName")
                {
                    return "T.[Name]";
                }
                else if (uiFieldName == "TeamAssignedDate")
                {
                    return "o.[AssignedDateToTeam]";
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetActualFieldName",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }

            return $"o.[{uiFieldName}]";
        }

        string GetCommaSeperatedStringItems(IEnumerable<string> filterItems)
        {
            var items = "";
            try
            {
                var seperator = "";
                var planItem = "";

                foreach (var item in filterItems)
                {
                    planItem = item.Replace("'", " "); 
                    items = $"{items}{seperator}''{planItem}''";
                    seperator = ",";
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetCommaSeperatedStringItems",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
            return items;
        }

        string GetCommaSeperatedIntItems(IEnumerable<int> filterItems)
        {
            var items = "";
            try
            {
                var seperator = "";

                foreach (var item in filterItems)
                {
                    items = $"{items}{seperator}{item}";
                    seperator = ",";
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetCommaSeperatedIntItems",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
            return items;
        }

        string GetCommaSeperatedByteItems(IEnumerable<byte> filterItems)
        {
            var items = "";
            try
            {
                var seperator = "";

                foreach (var item in filterItems)
                {
                    items = $"{items}{seperator}{item}";
                    seperator = ",";
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetCommaSeperatedByteItems",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
            return items;
        }

        protected async void Delete(string objectId, string orderNumber)
        {
            await Task.Yield();
            selectedObjectId = objectId;
            var msg = $"Are you sure you want to delete the Order \"{orderNumber}\"?";
            ModalNotification.ShowConfirmation("Confirm Delete", msg);
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            try
            {
                if (deleteConfirmed)
                {
                    spinShow = true;
                    var selectedOrder = orders.FirstOrDefault(f => f.ObjectId == selectedObjectId);
                    var deleteResponse = await _orderService.Delete(selectedObjectId);

                    //await LoadData();

                    //UriHelper.NavigateTo("/orders", true);//Todo:Rakib Zakir See Aminul Vai

                    if (!deleteResponse.IsSuccess)
                    {
                        ModalNotification.ShowMessage("Error", deleteResponse.Message);
                        return;
                    }

                    await OrderUploadActivityLogo(selectedOrder);
                    await SendMailToAllOperation("Delete", selectedOrder);
                    await grid.Reload();
                    spinShow = false;
                    await js.DisplayMessage("Successfully Deleted");

                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "ConfirmDelete_Click",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task OrderUploadActivityLogo(ClientOrderListModel deletedOrder)
        {
            try
            {
                ActivityLogModel activityLog = new ActivityLogModel();
                activityLog.ActivityLogFor = ActivityLogForConstants.Order;
                activityLog.PrimaryId = (int)deletedOrder.Id;
                activityLog.Description = $"Deleted Order '{deletedOrder.OrderNumber}' by '{loginUser.FullName}' on {DateTime.Now}";
                activityLog.CreatedDate = DateTime.Now;
                activityLog.CreatedByContactId = loginUser.ContactId;
                activityLog.ObjectId = Guid.NewGuid().ToString();

                await _activityLogService.Insert(activityLog);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OrderUploadActivityLogo",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        void OnGridRender(DataGridRenderEventArgs<ClientOrderListModel> args)
        {
            try
            {
                if (args.FirstRender)
                {
                    args.Grid.Groups.Add(new GroupDescriptor() { Title = "Order Place Date", Property = "OrderPlaceDateOnly", SortOrder = SortOrder.Descending });

                    args.Grid.Groups.Add(new GroupDescriptor() { Title = "Status", Property = "InternalOrderStatusEnumName" });

                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnGridRender",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                _activityLogCommonMethodService.InsertErrorActivityLog(activity).GetAwaiter().GetResult();

                js.DisplayMessage($"{ex.Message}").GetAwaiter().GetResult();
            }
        }

        private async Task AssignToTeam()
        {
            try
            {
                spinShow = true;
                assignOrderToTeam = true;
                team = new TeamModel();

                //isAssignOrderSubmitting = false;
                //ShowAssignPopup();
                await InsertAssingOrderToTeam();

                spinShow = false;
                assignOrderToTeam = false;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AssignToTeam",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        void ShowAssignPopup()
        {
            isAssignPopupVisible = true;
        }
        void ShowOrderAssignToTeamPopup()
        {
            isOrderAssignToTeamVisible = true;
        }
        void CloseOrderAssignToTeamPopup()
        {
            isOrderAssignToTeamVisible = false;
        }
        void CloseAssignPopup()
        {
            isAssignPopupVisible = false;
        }

        //Insert Assign Order To Team


        //Todo:Rakib in this method have some unnecessary work
        private async Task InsertAssingOrderToTeam()
        {
            try
            {
                await Task.Yield();
                isAssignOrderSubmitting = true;

                List<OrderTeamModel> teamOrders = new List<OrderTeamModel>();
                if (selectedOrders.Count() <= 0)
                {
                    await js.DisplayMessage("Dear User  Please select a Order");
                    return;
                }

                //Check Any Selected have no dedicated team
                foreach (var order in selectedOrders)
                {
                    var teamForCompany = new List<CompanyTeamModel>();

                    if (orderAssignToTeam.TeamId <= 0 && !isOrderAssignToTeamVisible)
                    {
                        teamForCompany = await _companyTeamService.GetByCompanyId(order.CompanyId);
                    }
                    
                    if (teamForCompany.Count() <= 0 && orderAssignToTeam.TeamId <= 0)
                    {
                        await js.DisplayMessage("Dear User no select team or haven't any dedicated team to work.");
                        //selectedOrders = new List<ClientOrderListModel>();
                        //isAssignOrderSubmitting = false;
                        //CloseAssignPopup();
                        return;
                    }
                }

                foreach (var order in selectedOrders)
                {
                    if(orderAssignToTeam != null && orderAssignToTeam.TeamId > 0)
                    {
                        OrderTeamModel orderTeam = new OrderTeamModel
                        {
                            OrderId = order.Id,
                            TeamId = orderAssignToTeam.TeamId,
                            CreatedBy = loginUser.ContactId,
                            IsPrimary = true,
                            IsItemAssignToTeam = true
                        };
                            teamOrders.Add(orderTeam);
                            TeamModel teamObject = await _teamService.GetById(orderAssignToTeam.TeamId);
                            await SetInternalMessageObject("OrderAssignByOps", order.OrderNumber, teamObject);
                            await SetInternalMessageObject("OrderAssignByOpsToTeam", order.OrderNumber, teamObject);
                    }
                    else
                    {
                        companyTeams = new List<CompanyTeamModel>();
                        companyTeams = await _companyTeamService.GetByCompanyId(order.CompanyId);

                        if (companyTeams.Any())
                        {
                            foreach (var companyteam in companyTeams)
                            {
                                OrderTeamModel orderTeam = new OrderTeamModel
                                {
                                    OrderId = order.Id,
                                    TeamId = companyteam.TeamId,
                                    CreatedBy = loginUser.ContactId,
                                    IsPrimary = true,
                                    IsItemAssignToTeam = true
                                };
                                teamOrders.Add(orderTeam);
                                TeamModel team = await _teamService.GetById(companyteam.TeamId);
                                await SetInternalMessageObject("OrderAssignByOps", order.OrderNumber, team);
                                await SetInternalMessageObject("OrderAssignByOpsToTeam", order.OrderNumber, team);
                            }
                        }
                    }
                    
                }

                var addResponse = await _orderTeamService.Insert(teamOrders, null);

                if (!addResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", addResponse.Message);
                    isAssignOrderSubmitting = false;
                    return;
                }

                //Update Order Status when Order Assign To Team

                foreach (var order in selectedOrders)
                {

                    if(orderAssignToTeam != null && orderAssignToTeam.TeamId > 0)
                    {
                        order.AssignedTeamId = orderAssignToTeam.TeamId;
                    }

                    else
                    {
                        CompanyTeamModel companyTeam = await _companyTeamService.GetTeamByCompanyId(order.CompanyId);

                        if (companyTeam != null)
                        {
                            order.AssignedTeamId = companyTeam.TeamId;
                        }
                    }

                    await OrderAssingToTeamAndStatusUpdate(order);
                }
                //Update Order Status when Order Assign To Team

                selectedOrders = new List<ClientOrderListModel>();

                await SendInternalMessage();
                teamOrders = new List<OrderTeamModel>();
                isAssignOrderSubmitting = false;
                isOrderAssignToTeamVisible = false;
                orderAssignToTeam = new OrderAssignToTeam();
                CloseAssignPopup();
                spinShow = false;
                StateHasChanged();
                await js.DisplayMessage("Successfully Assigned");
                orders = await _orderService.GetOrderByFilterWithPaging(orderFilter);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertAssingOrderToTeam",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);

                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task OrderAssingToTeamAndStatusUpdate(ClientOrderListModel order)
        {
            
            await _clientOrderService.UpdateClientOrderListModel(order); // One Order Service Call

            var clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);

            List<string> clientOrderItemIds = new List<string>();
            foreach (var orderItem in clientOrderItems)
            {
                clientOrderItemIds.Add(orderItem.Id.ToString());
            }

            await _clientOrderItemService.UpdateClientOrderItemTeamId((int)order.Id, (int)order.AssignedTeamId, clientOrderItemIds);

            await UpdateOrderStatus(order);


            order.AssignedByOpsContactId = loginUser.ContactId;
            order.UpdatedByContactId = loginUser.ContactId;

            await _clientOrderService.UpdateClientOrderOpsAndUpdateByListModel(order); // One Order Service Call

            var externalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.Assigned));

            await _clientOrderItemService.UpdateClientOrderItemStatusByClientOrderId((int)order.Id, (byte)InternalOrderItemStatus.Assigned, externalStatus);
        }

        private async Task UpdateOrderStatus(ClientOrderListModel clientOrder, InternalOrderStatus status = InternalOrderStatus.Assigned)
        {
            try
            {
                clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(status));
                clientOrder.InternalOrderStatus = (byte)status;
                
                await _clientOrderService.UpdateClientOrderListModel(clientOrder);
               
                ClientOrderModel newClientOrder = new ClientOrderModel
                {
                    Id = clientOrder.Id
                };

                await AddOrderStatusChangeLog(newClientOrder, status); // Order Status Change log

                var orderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)clientOrder.Id);
                foreach (var orderItem in orderItems)
                {
                    if (orderItem.Status == (int)InternalOrderItemStatus.OrderPlaced)
                    {
                        orderItem.Status = (byte)status;
                        orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange((InternalOrderItemStatus)status));
                        await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);

                        await AddOrderItemStatusChangeLog(orderItem, (InternalOrderItemStatus)status); // Order Item Status Change log
                    }
					if (orderItem.Status == (int)InternalOrderItemStatus.ReadyToDeliver)
					{
						orderItem.Status = (byte)status;
						orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange((InternalOrderItemStatus)status));
						await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);

						await AddOrderItemStatusChangeLog(orderItem, (InternalOrderItemStatus)status); // Order Item Status Change log
					}

				}
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderStatus",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);

                await js.DisplayMessage($"{ex.Message}");
            }

            //TODO:Need to update this code
			if (status == InternalOrderStatus.Delivered)
			{
				await js.DisplayMessage("Successfully Delivered");
			}
		}


        private async Task MakeOrderComplete(ClientOrderListModel clientOrder, InternalOrderStatus status = InternalOrderStatus.Assigned)
        {
            await _updateOrderItemBLLService.UpdateOrderItemStatusByOrderId(clientOrder.Id, (InternalOrderItemStatus)status,loginUser.ContactId);
            await _orderStatusService.UpdateOrderStatusByOrderId(clientOrder.Id, loginUser.ContactId);
        }
        public async Task OnAssignedTeam(List<int> teamIds)
        {
            await Task.Yield();
            selectedTeams = teamIds;
        }

        #region Operation Mail for Order

        private async Task SendMailToAllOperation(string callerType, ClientOrderListModel deletedOrder)
        {
            try
            {
                var userList = await _operationEmailService.GetUserListByCompanyIdAndPermissionName(Convert.ToInt32(_configuration["CompanyId"]), PermissionConstants.OrderDeleteOrderEmailNotifyForOPeration);
                foreach (var user in userList)
                {
                    var detailUrl = $"Your Order Deleted";

                    var ordervm = new ClientOrderViewModel
                    {
                        Contact = user,
                        DetailUrl = detailUrl,
                        CreatedByContactId = loginUser.ContactId,
                        OrderNumber = deletedOrder.OrderNumber,
                    };
                    ordervm.MailType = callerType;
                    await _workflowEmailService.SendOrderAddUpdateDeleteNotificationForCompanyOperationsTeam(ordervm);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendMailToAllOperation",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        #endregion

        #region Internal Message

        private async Task SetInternalMessageObject(string callerType, string OrderNumber, TeamModel team)
        {
            try
            {
                if ((callerType == "OrderAssignByOps"))
                {
                    var contact = await _contactManager.GetById(loginUser.ContactId);
                    InternalMessageNotification internalMessageNotification = new InternalMessageNotification
                    {
                        Contact = contact,
                        SenderContactId = loginUser.ContactId,
                        OrderNumber = OrderNumber,
                        MessageType = callerType,

                    };
                    if (team != null)
                    {
                        internalMessageNotification.TeamName = team.Name;
                    }
                    internalMessagesNotificationsForOps.Add(internalMessageNotification);
                }
                else
                {
                    List<TeamMemberListModel> teamMembers = await _teamMemberService.GetTeamMemberListWithDetailsByTeamId(team.Id);
                    var contactList = new List<ContactModel>();
                    foreach (var teamMember in teamMembers)
                    {
                        contactList.Add(await _contactManager.GetById(teamMember.ContactId));
                    }

                    InternalMessageNotification internalMessageNotification = new InternalMessageNotification
                    {
                        Contacts = contactList,
                        SenderContactId = loginUser.ContactId,
                        OrderNumber = OrderNumber,
                        MessageType = callerType,
                        TeamName = team.Name
                    };
                    internalMessagesNotificationsForTeam.Add(internalMessageNotification);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SetInternalMessageObject",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

        #endregion

        private async Task SendInternalMessage()
        {
            try
            {
                foreach (var internalMessageNotificationForOps in internalMessagesNotificationsForOps)
                {
                    await _internalMessageService.Insert(internalMessageNotificationForOps);
                }
                foreach (var internalMessageNotificationForTeam in internalMessagesNotificationsForTeam)
                {
                    await _internalMessageService.Insert(internalMessageNotificationForTeam);
                }
                internalMessagesNotificationsForOps = new List<InternalMessageNotification>();
                internalMessagesNotificationsForTeam = new List<InternalMessageNotification>();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendInternalMessage",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        #region For Dropdown List for Rework and Amendment
        // For Dropwond Rework,Amendment
        bool disabled = true;
        OrderType objectOrder = new OrderType();
        IEnumerable<OrderType> reworks = new List<OrderType>{
        new OrderType{ OrderTypeName="Amendment",OrderTypeValue="1"}
    };

        // For Dropdown Rework and Amendment
        void OnChange(object value, string name)
        {
            try
            {
                var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)0,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "OnChange",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                js.DisplayMessage($"{ex.Message}");
            }

            //console.Log($"{name} value changed to {str}");
        }
        void ChangeBound(object value, string name)
        {
            //console.Log($"{name} value changed to {myModel.MyValue}");
        }
        public class OrderType
        {
            public string OrderTypeName { get; set; }

            public string OrderTypeValue { get; set; }
        }

        public async Task AddOrderStatusChangeLog(ClientOrderListModel clientOrder, InternalOrderStatus internalOrderStatus)
        {
            try
            {
                if (clientOrder.InternalOrderStatus != (byte)internalOrderStatus)
                {
                    OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
                    {
                        OrderId = (int)clientOrder.Id,
                        OldInternalStatus = clientOrder.InternalOrderStatus,
                        NewInternalStatus = (byte)internalOrderStatus,
                        OldExternalStatus = clientOrder.ExternalOrderStatus,
                        NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
                        ChangeByContactId = loginUser.ContactId,
                        ChangeDate = DateTime.Now
                    };

                    await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)0,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddOrderStatusChangeLog",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        #endregion
        #region Download and File go to InProgress Folder of 

        private async Task ClientDownload(int OrderId, bool canStatusChange = true)
        {
            var completedFiles = new List<ClientOrderItemModel>();
            try
            {
                spinShow = true;
                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                // Delete Previous Folder
                var downloadpath = await _ftpFilePathService.ExistsFolderDelete($"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}");

                var status = (int)ExternalOrderStatus.ReadyToDownload;

                var selectedFiles = await _clientOrderItemService.GetAllOrderItemByOrderIdForClientDownLoad(OrderId);
                spinShow = false;
                isProgressBar = true;
                if (isProgressBar)
                {
                    CurrentValue = 0.1;
                }
                if (selectedFiles.Count() > 0)
                {
                    isSubmitting = true;
                    var result = false;
                    var orderFind = selectedFiles.FirstOrDefault();
                    var orderInfo = await _clientOrderService.GetById((int)orderFind.ClientOrderId);
                    var serverInfo = await _fileServerService.GetById((int)orderInfo.FileServerId);
                    var count = 0;
                    maxValue = selectedFiles.Count;

                    FileUploadModel Filemodel = new FileUploadModel();
                    Filemodel.FtpUrl = serverInfo.Host;
                    Filemodel.userName = serverInfo.UserName;
                    Filemodel.password = serverInfo.Password;
                    Filemodel.SubFolder = serverInfo.SubFolder;
                    var host = Filemodel.FtpUrl.Split(':');

                    var client = new AsyncFtpClient();

                    if (host.Length == 3)
                    {
                        client = new AsyncFtpClient($"{host[0]}:{host[1]}", Filemodel.userName, Filemodel.password, Convert.ToInt32(host[2]));
                    }
                    else
                    {
                        client = new AsyncFtpClient(Filemodel.FtpUrl, Filemodel.userName, Filemodel.password);
                    }
                    client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    client.Config.ValidateAnyCertificate = true;
                    await client.AutoConnect();

                    var downloadAsZip = true;

                    foreach (var file in selectedFiles)
                    {
                        if (file.InternalFileOutputPath == null)
                        {
                            count++;
                            CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                            this.StateHasChanged();
                            continue;
                        }
                        Filemodel.fileName = Path.GetFileName(file.InternalFileOutputPath);

                        Filemodel.OrderNumber = orderInfo.OrderNumber;
                        if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                        {
                            Filemodel.UploadDirectory = Path.GetDirectoryName($"{serverInfo.SubFolder}/{file.InternalFileOutputPath}");
                        }
                        else
                        {
                            Filemodel.UploadDirectory = Path.GetDirectoryName(file.InternalFileOutputPath);
                        }
                        var modifiedPath = Path.GetDirectoryName(file.InternalFileOutputPath).Split("Completed");
                        Filemodel.DownloadFolderName = modifiedPath[1];

                        Filemodel.Date = orderInfo.CreatedDate;
                        var dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                        Filemodel.ContactName = contactInfo.FirstName + " " + contactInfo.Id;
                        // Download Code for Client 

                        //result = await _ftpService.SingleDownload(js, Filemodel, dlpath,true);
                        await _ftpService.CreateFolderDownloadTime(Filemodel);

                        var dataSavePath = "";
                        //var dataSavePath = dlPath + $"\\Download\\{model.OrderNumber}{model.DownloadFolderName}";
                        if (downloadAsZip)
                        {
                            dataSavePath = dlpath + $"\\{Filemodel.ContactName}\\{Filemodel.DownloadFolderName}";
                        }
                        else
                        {
                            dataSavePath = dlpath + $"\\{Filemodel.DownloadFolderName}";
                        }
                        //var dataSavePath = dlPath + $"\\{model.ContactName}\\{model.DownloadFolderName}";

                        if (!Directory.Exists(dataSavePath))
                        {
                            Directory.CreateDirectory(dataSavePath);
                        }
                        var localPath = $"{dataSavePath}/{Filemodel.fileName}";
                        var remotePath = $"{Filemodel.UploadDirectory}/{Filemodel.fileName}";
                        await client.DownloadFile(localPath, remotePath);
                        completedFiles.Add(file);
                        //await _ftpService.CopyFile(model);
                        count++;
                        CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                        this.StateHasChanged();
                        result = true;
                    }
                    await client.Disconnect();
                    if (CurrentValue == 100)
                    {
                        CurrentValue = 0;
                        isProgressBar = false;
                        StateHasChanged();
                    }
                    spinShow = true;
                    StateHasChanged();
                    if (result)
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        //var model = await DownloadOrderItemZipFile(contactInfo, order, fileinfo);
                        await _downloadService.CreateZipAndDownload(contactInfo, orderInfo, webHost, null, null, null);
                    }
                    else
                    {
                        await js.DisplayMessage("Download Failed");
                    }

                    //	//Update Status after editor download file
                    if (canStatusChange)
                    {
                        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

                        //if (authState.User.IsInRole(PermissionContants.Order_UpdateOrderForClientDownload)) //Todo:Rakib See Aminul vai
                        if (loginUser.CompanyType == (int)CompanyType.Client)
                        {

                            await UpdateOrderItemStatus(InternalOrderItemStatus.Completed, completedFiles);

                            completedFiles = new List<ClientOrderItemModel>();

                            completedFiles = await _clientOrderItemService.GetAllOrderAssignedItemByOrderId((int)orderInfo.Id, loginUser.ContactId);

                            var orderAllItem = await _clientOrderItemService.GetAllOrderItemByOrderId((int)orderInfo.Id);

                            UpdateOrderStatus(orderInfo, await GetInternalOrderStatus(OrderId)); //ToDo:RakibStatus
                                                                                                 //Order Status Update
                        }
                    }
                    spinShow = false;
                    StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");
                    orders = await _orderService.GetOrderByFilterWithPaging(orderFilter);
                    isSubmitting = false;
                    StateHasChanged();
                }
                else
                {
                    await js.DisplayMessage("You are not Eligible To Download ! Contact Admin");
                    spinShow = false;
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)0,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "ClientDownload",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            #endregion
        }

        private async Task UpdateOrderItemStatus(InternalOrderItemStatus status, List<ClientOrderItemModel> selectedFiles)
        {
            try
            {
                if (selectedFiles != null && selectedFiles.Any())
                {
                    foreach (var selectedFile in selectedFiles)
                    {
                        if (selectedFile.Status == (byte)InternalOrderItemStatus.Completed)
                        {
                            selectedFile.Status = (byte)InternalOrderItemStatus.Completed;
                            selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.Completed));
                            await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                        }

                        else
                        {
                            selectedFile.Status = (byte)status;
                            selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
                            await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                        }
                        await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log

                    }
                }
                //else
                //{
                //    foreach (var selectedFile in selectedFiles)
                //    {
                //        if (selectedFile.Status == (byte)InternalOrderItemStatus.Completed)
                //        {
                //            selectedFile.Status = (byte)InternalOrderItemStatus.Completed;
                //            selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.Completed));
                //            await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                //        }
                //        else
                //        {
                //            selectedFile.Status = (byte)status;
                //            selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
                //            await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                //        }
                //        await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log

                //    }
                //}
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderItemStatus",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task UpdateOrderStatus(ClientOrderModel clientOrder, InternalOrderStatus status)
        {
            try
            {
                clientOrder.InternalOrderStatus = (byte)status;
                clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange((InternalOrderStatus)status));
                await _clientOrderService.UpdateClientOrderStatus(clientOrder);
                await AddOrderStatusChangeLog(clientOrder, status); // Order Status Change log
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderStatus",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task<InternalOrderStatus> GetInternalOrderStatus(int orderId)
        {
            InternalOrderStatus internalOrderItemStatus = new InternalOrderStatus();
            try
            {
                await Task.Yield();
                ClientOrderItemStatus clientOrderItemMinStatus = await _clientOrderItemService.GetOrderItemMinStatusByOrderId(orderId);
                internalOrderItemStatus = (InternalOrderStatus)clientOrderItemMinStatus.Status;

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetInternalOrderStatus",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return internalOrderItemStatus;
        }

        public async Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus)
        {
            try
            {
                var previousLog = await _orderStatusChangeLogService.OrderStatusLastChangeLogByOrderId((int)clientOrder.Id);
                
                if (previousLog != null && previousLog.NewInternalStatus != (byte)internalOrderStatus)
                {
                    OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
                    {
                        OrderId = (int)clientOrder.Id,
                        NewInternalStatus = (byte)internalOrderStatus,
                        NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
                        ChangeByContactId = loginUser.ContactId,
                        ChangeDate = DateTime.Now
                    };
                    
                    orderStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                    orderStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                    orderStatusChangeLog.TimeDurationInMinutes = (int)(orderStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
                    
                    await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddOrderStatusChangeLog",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };

                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        public async Task AddOrderItemStatusChangeLog(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus)
        {
            int tryCount = 0;
            while (tryCount <= 3)
            {
                try
                {

                    var previousLog = await _orderItemStatusChangeLogService.OrderItemStatusLastChangeLogByOrderFileId((int)clientOrderItem.Id);
                    if (previousLog != null && previousLog.NewInternalStatus != (byte)internalOrderItemStatus)
                    {
                        OrderItemStatusChangeLogModel orderItemStatusChangeLog = new OrderItemStatusChangeLogModel
                        {
                            OrderFileId = (int)clientOrderItem.Id,
                            NewInternalStatus = (byte)internalOrderItemStatus,
                            NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
                            ChangeByContactId = loginUser.ContactId,
                            ChangeDate = DateTime.Now
                        };

                        
                        orderItemStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                        orderItemStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                        orderItemStatusChangeLog.TimeDurationInMinutes = (int)(orderItemStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
                        
                        await _orderItemStatusChangeLogService.Insert(orderItemStatusChangeLog);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    await js.DisplayMessage(ex.InnerException.ToString());
                    tryCount++;
                }

            }


        }

        private async Task ViewOrderStatusLog(int orderId)
        {
            try
            {
                orderStatusChangeLogs.Clear();
                var orderChangeLogs = await _orderStatusChangeLogService.GetByOrderId(orderId);
                orderStatusChangeLogs.AddRange(orderChangeLogs);
                isOrderChangeLogPopupVisible = true;
                TotalMinutesForOrder = orderStatusChangeLogs.Sum(o => o.TimeDurationInMinutes);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "ViewOrderStatusLog",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        void CloseOrderChangeLogPopup()
        {
            isOrderChangeLogPopupVisible = false;
            orderStatusChangeLogs.Clear();
            TotalMinutesForOrder = 0;
        }

        public async Task UpdateOrderAllowExtraOutputFileUploadField(ClientOrderListModel order)
        {
            try
            {
                ClientOrderModel clientOrder = new ClientOrderModel
                {
                    Id = order.Id,
                    AllowExtraOutputFileUpload = true,
                };
                await Task.Yield();
                var isAllowed = await _clientOrderService.UpdateOrderAllowExtraOutputFileUploadField(clientOrder);
                orders = await _orderService.GetOrderByFilterWithPaging(orderFilter);
                if (isAllowed.IsSuccess)
                {
                    await js.DisplayMessage("Successfully Allowed Extra File");
                }
                else
                {
                    await js.DisplayMessage("Unable To Allow ExtraFile");
                }
            }
            catch (Exception ex)
            {
                await js.DisplayMessage("Unable To Allow ExtraFile");
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderAllowExtraOutputFileUploadField",
                    RazorPage = "OrderList.razor.cs",
                    Category = (int)ActivityLogCategory.OrderListError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

       
        private async Task ViewCategorySetUpPopup(ClientOrderListModel clientOrder)
        {
            try
            {
                orderWiseCategory.OrderId = clientOrder.Id;
                commonCategories = await _clientCategoryService.GetByCompanyId(clientOrder.CompanyId);
                isOrderCategorySetUpPopupVisible = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = 0,
					ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "ViewCategorySetUpPopup",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};

				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
			}
        }

        private async Task CancelCategorySetUpPopup()
        {            
            isOrderCategorySetUpPopupVisible = false;
            StateHasChanged();           
        }

        private async Task SetOrderCategory()
        {
            // add order Item category
            var orderItemList = await _clientOrderItemService.GetAllByOrderId(orderWiseCategory.OrderId);
            
            //It need to be client category
            var category = commonCategories.FirstOrDefault(cc => cc.CommonCategoryId == orderWiseCategory.CategoryId);

            if (category != null)
            {
                orderWiseCategory.CategoryPrice = category.ClientCategoryPrice;
                orderWiseCategory.TimeInMinute = category.TimeInMinutes;
                orderWiseCategory.CategorySetByContactId = _workContext.LoginUserInfo.ContactId;
                orderWiseCategory.CategorySetDate = DateTime.Now;
                orderWiseCategory.CategorySetStatus = (byte)ItemCategorySetStatus.Manual_set;

                if (orderItemList != null)
                {
                    await _clientOrderItemService.UpdateOrderItemListCategory(orderItemList, orderWiseCategory);
                }
            }

            var clientOrder = await _orderService.GetById(orderWiseCategory.OrderId);
            await _orderStatusService.UpdateOrderCategorySetStatus(clientOrder);
            await CancelCategorySetUpPopup();

            await js.DisplayMessage("Category Updated");
        }

        /// <summary>
        /// Here Approved A order All Item Category Set Status. 
        /// </summary>
        /// <returns></returns>
        private async Task ApprovedOrderCategory(ClientOrderListModel clientOrder)
        {
            // add order Item category
            var orderItemList = await _clientOrderItemService.GetAllByOrderId(clientOrder.Id);

            if (orderItemList != null)
            {
                var orderwiseCategoryForApproved = new OrderWiseCategoryModel();
                orderwiseCategoryForApproved.CategorySetStatus = (byte)(ItemCategorySetStatus.Approved);
                orderwiseCategoryForApproved.CategoryApprovedByContactId = _workContext.LoginUserInfo.ContactId;

                await UpdateCategorySetStatus(clientOrder);
                await _clientOrderItemService.ApprovedOrderItemListCategory(orderItemList, orderwiseCategoryForApproved);
            }
            await js.DisplayMessage("Successfully Category Approved");
        }

        private async Task UpdateCategorySetStatus(ClientOrderListModel clientOrder)
        {
            ClientOrderModel order = new ClientOrderModel
            {
                Id = clientOrder.Id,
                CategorySetStatus = (byte)(ItemCategorySetStatus.Approved)
            };
            await _orderStatusService.UpdateOrderCategorySetStatus(order);
        }

        #region 
        private async Task ApplyFilterClicked()
        {
            //isFilterLoadingComplete = true;
            await grid.FirstPage(true);
        }

        private async Task ShowHideTopFilter_Click()
        {
            isShowTopFilter = !isShowTopFilter;

            await js.InvokeVoidAsync("showHideTopFilter", isShowTopFilter);
        }
        #endregion

        #region Export Excel
        public async Task DownloadExcelDocument()
        {
            isExporting = true;
            var allOrders = new List<ClientOrderListModel>();

            if (selectedOrders != null && selectedOrders.Any())
            {
                allOrders = selectedOrders.ToList();
            }
            else
            {
                allOrders = await _orderService.GetOrderByFilterWithoutPaging(orderFilter);
                //allOrders = await _mainProductService.GetOr(mainProductFilter);
            }

            //var columns = grid.ColumnsCollection;

            //columns[0].GetVisible();
            //columns[2].SetWidth("500px");

            //columns[3].SetParametersAsync(ParameterViewExtensions.)

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("Orders");
                int row = 1;
                int column = 1;

                var selectedCols = new List<CustomTableColumnModel>();

                if (loginUser.CompanyType == (int)CompanyType.Admin)
                {
                    selectedCols = tableColumns.Where(f => f.IsAdminCompanyColumn == true).OrderBy(f => f.DisplayOrder).ToList();
                }
                else
                {
                    selectedCols = tableColumns.Where(f => f.IsClientCompanyColumn == true).OrderBy(f => f.DisplayOrder).ToList();
                }

                foreach (var col in selectedCols)
                {
                    worksheet.Cell(row, column++).Value = col.DisplayName;
                }

                for (int i = 1; i < column; i++)
                {
                    worksheet.Cell(1, i).Style.Font.Bold = true;
                }

                row++;

                foreach (var pd in allOrders)
                {
                    column = 1;

                    foreach (var col in selectedCols)
                    {
                        var value = pd.GetType().GetProperty(col.FieldName).GetValue(pd, null);

                        if (col.Id == (int)OrderColumns.InternalOrderStatus)
                        {
                            worksheet.Cell(row, column++).Value = (InternalOrderStatus)Convert.ToInt32(value);
                        }
                        else if (col.Id == (int)OrderColumns.ExternalOrderStatus)
                        {
                            worksheet.Cell(row, column++).Value = (ExternalOrderStatus)Convert.ToInt32(value);
                        }
                        else
                        {
                            worksheet.Cell(row, column++).Value = value;
                        }
                    }

                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    var fileName = $"Orders_{DateTime.Now.ToString("MM_dd_yyyy h_mm_tt")}.xlsx";
                    await js.InvokeAsync<object>("saveAsFile", fileName, Convert.ToBase64String(content));
                }
            }

            isExporting = false;
        }
		#endregion

		#region Update Order Deadline 
		private async Task UpdateOrderDeadLine(ClientOrderListModel order)
		{
			try
			{
				var clientOrder = new ClientOrderModel
				{
					Id = order.Id,
					DeliveryDeadlineInMinute=order.DeliveryDeadlineInMinute
				};
                var update = await _clientOrderService.UpdateOrderDeadline(clientOrder);
                if (update.IsSuccess)
                {
                    await grid.Reload();
                    isOrderDeadlinePopupVisible= false;
                    this.StateHasChanged();
                }
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = 0,
					ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "UpdateOrderDeadLine",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};

				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
			}

		}

        private async Task UpdateExpectedDeliveryDate(ClientOrderListModel order)
        {
            try
            {
                if(OrderExpectedDeliveryDate != null)
                {
                    await _clientOrderItemService.UpdateOrderItemExpectedDeliveryDateByClientOrderId(order.Id,OrderExpectedDeliveryDate);
                    var update = await _clientOrderService.UpdateOrderDeadlineDate(order.Id);
                    if (update.IsSuccess)
                    {
                        await grid.Reload();
                    }
                }
              
            }
            catch (Exception ex)
            {
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = 0,
					ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "UpdateExpectedDeliveryDate",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};

				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
			}

            isOrderExpectedDeliveryDatePopupVisible = false;
            this.StateHasChanged();

        }
        async Task OrderDeadlinePopupVisible(ClientOrderListModel clientOrder)
		{
			spinShow = true;
			ClientOrder = new ClientOrderListModel();
            ClientOrder = orders.FirstOrDefault(x=>x.Id==clientOrder.Id);
			isOrderDeadlinePopupVisible = true;
			this.StateHasChanged();
			spinShow = false;
			StateHasChanged();
		}

        async Task OrderExpectedDeliveryDatePopupVisible(ClientOrderListModel clientOrder)
        {
            spinShow = true;
            ClientOrder = new ClientOrderListModel();
            ClientOrder = orders.FirstOrDefault(x => x.Id == clientOrder.Id);
            isOrderExpectedDeliveryDatePopupVisible = true;
            this.StateHasChanged();
            spinShow = false;
            StateHasChanged();
        }
        async Task CloseDeadlinePopup()
		{
			spinShow = true;
			isOrderDeadlinePopupVisible = false;
			this.StateHasChanged();
			spinShow = false;
			StateHasChanged();
		}

        async Task CloseExpectedDeliveryDatePopup()
        {
            spinShow = true;
            isOrderExpectedDeliveryDatePopupVisible = false;
            this.StateHasChanged();
            spinShow = false;
            StateHasChanged();
        }
        #endregion

    }
}
