using CutOutWiz.Core;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.SOP;
using FluentFTP;
using KowToMateAdmin.Helper;
using KowToMateAdmin.Models;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using KowToMateAdmin.Reports;
using Mailjet.Client.Resources.SMS;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using static CutOutWiz.Core.Utilities.Enums;
using static iTextSharp.text.pdf.AcroFields;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.Media.Protection.PlayReady;
using Windows.ApplicationModel.Chat;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Microsoft.AspNetCore;
using System.Drawing;
using System.Drawing.Imaging;
using ImageMagick;
using CutOutWiz.Services.Models.ClientCategoryServices;
using DocumentFormat.OpenXml.Drawing.Charts;
using FtpSharpLib.Core.Utility.Enum;
using FtpEncryptionMode = FluentFTP.FtpEncryptionMode;
using System.Linq;
using System.Text;
using CutOutWiz.Services.Models.FileUpload;

namespace KowToMateAdmin.Pages.OrderUpload
{
    public partial class OrderDetails
    {
        #region Paramiters
        [Parameter]
        public string objectId { get; set; }
        #endregion

        #region Global Properties
        private ClientOrderModel order = new ClientOrderModel();

        private List<ClientOrderSOPTemplateModel> orderSOPTemplates = new List<ClientOrderSOPTemplateModel>();

        private List<OrderFileAttachment> orderAttachementFiles = new List<OrderFileAttachment>();

        private List<int> templateIds = new List<int>();
        private List<SOPTemplateModel> sopTemplates = new List<SOPTemplateModel>();
        // Order SOP Template for View
        private List<OrderSOPTemplateModel> orderSOPTemplateList = new List<OrderSOPTemplateModel>();
        private List<ClientOrderItemModel> clientOrderItems = new List<ClientOrderItemModel>();
        IEnumerable<int> pageSizeOptions = new int[] { 5, 10, 7, 12, 15, 18, 20, 50, 100, 500, 600, 700, 1000 };
        bool isLoading = false;
        bool isSubmitting = false;
        List<CustomEnumTypes> internalOrderItemStatusList = new List<CustomEnumTypes>();
        List<CustomEnumTypes> fileGroupCustomEnumList = new List<CustomEnumTypes>();
        private byte? filterStatus;
        private byte? filterGroup;
        private string filterFileName;
        bool allowRowSelectOnRowClick = false;
        IList<ClientOrderItemModel> selectedFiles = new List<ClientOrderItemModel>();
        IList<ClientOrderItemModel> selectedFeedbackFiles = new List<ClientOrderItemModel>();

        RadzenDataGrid<ClientOrderItemModel> grid;
        RadzenDataGrid<ClientOrderItemModel> gridColorRefGroupList;
        RadzenDataGrid<ClientOrderItemModel> feedbacksGrid;
        bool isOrderItemsAssignToEditorPopupVisible = false;
        bool isNewTeamAssignToOrderVisible = false;
        bool isAssignItemToTeamPopupVisible = false;
        bool isReplaceInQcFilesPopupVisible = false;
        public ClientOrderItemModel clientOrderItem = new ClientOrderItemModel();
        bool isAssignOrderSubmitting = false;
        List<TeamMemberListModel> teamMembers = new List<TeamMemberListModel>();
        List<TeamMemberListModel> loginUserTeamMembers = new List<TeamMemberListModel>();
        private LoginUserInfoViewModel loginUser = new LoginUserInfoViewModel();
        protected ModalNotification ModalNotification { get; set; }
        private TeamModel team = new TeamModel();
        List<TeamModel> teams = new List<TeamModel>();
        List<int> selectedCompanyDedicatedTeams = new List<int>();
        int newTeamId = 0;
        //Progress Bar
        private double maxValue;
        private double progressBarCurrentValue = 0;
        // Image Preview
        bool isShowImagePopup = false;
        bool spinShow = false;
        AuthenticationState authState;
        bool isAbleToUpload = false;
        //bool isUploadCompleteFile = false;
        bool isInstructionEditable = false;
        bool isOrderAttachmentEditPopupVisible = false;
        bool isOrderItemAddPopupVisible = false;
        int selectedTeamMemberIdForAssignFiles = 0;
        bool isOrderItemChangeLogPopupVisible = false;
        bool isOrderItemAcitivityLogPopupVisible = false;
        bool isOrderItemExpectedDeliveryDatePopupVisible = false;
        //public List<OrderFileAttachment> newOrderAttachments = new List<OrderFileAttachment>();
        bool isProgressBar = false;
        bool isShowProductionDoneImagePopup = false;
        bool isShowCompletedImagePopup = false;
        private List<ClientOrderItemModel> orderItems = new List<ClientOrderItemModel>();
        private static List<FileForUploadDetails> _orderNewItemsUploadFileFromJs = new();
        private static List<FileForUploadDetails> _orderSOPExistingItemsUploadFileFromJs = new();
        private List<OrderItemStatusChangeLogModel> orderItemStatusChangeLogs = new List<OrderItemStatusChangeLogModel>();
        bool showActionColumn = false;
        private FileServerModel fileServer = new FileServerModel();
        private CompanyModel orderCompany = new CompanyModel();
        DateTimeConfiguration _dateTime = new DateTimeConfiguration();
        private double CurrentValueForEditOrderItemPregressbar = 0;
        private double CurrentValueForEditOrderAttachementPregressbar = 0;
        private bool isUploadInputDisabled = false;
        private List<OrderFileAttachment> orderAttachmentListForView = new List<OrderFileAttachment>();
        int fileViewMode = 1;
        string topDirectoryPath = "";
        private List<FolderNodeModel> folderNodes = new List<FolderNodeModel>();
        List<ClientOrderItemModel> checkListFile = new List<ClientOrderItemModel>();
        TimerService timer = new TimerService();
        bool allowFolderUploadForOrder = false;
        private IReadOnlyList<IBrowserFile> loadedFiles { get; set; }
        private IReadOnlyList<IBrowserFile> loadReplaceFile { get; set; }
        public List<IBrowserFile> loadedFiless = new();
        private static List<FileForUploadDetails> _ReplaceQCselectedFileFromJs = new();
        CompanyModel company = new CompanyModel();
        bool uploadCancelItem = false;
        bool assignOrderToTeam = false;
        bool orderSOPTemplateUploadUpdatePopup = false;
        bool isOrderSOPUploadInputDisabled = false;

        GenericServices _genericService = new GenericServices();
        bool isOrderFileAddPopupVisible = false;
        bool allowFolderUploadForOrderAdditional = false;
        private List<FolderNodeModel> selectedOrderItemFromFolderStructure = new List<FolderNodeModel>();
        private List<ClientOrderItemModel> clientItemColorRefGroupList = new();
        private List<ClientOrderItemModel> feedBackImages = new();
        private bool showImageOnList = false;
        private bool isSetOrderItemTypePopupVisible = false;
        private bool isBackSetOrderItemTypePopupVisible = false;
        private int selectedOrderItemGroup = 0;
        private ClientOrderItemModel clientOrderItemForReplace = new ClientOrderItemModel();
        private List<ActivityLogModel> activityLogs = new List<ActivityLogModel>();
        int orderItemStatusWiseViewMode = 1;
        public CssHelper cssHelper = new CssHelper();
        public ClientOrderItemModel deadLineUpdateOrderItem = new ClientOrderItemModel();

        #region Color Ref Images
        private byte? filterColorRefStatus;
        private byte? filterColorRefGroup;
        private string filterColorRefFileName;
        IList<ClientOrderItemModel> selectedColorRefFiles = new List<ClientOrderItemModel>();

        private bool isCompletedBtnClicked = false;
        private bool isRawBtnClicked = false;

        private List<CutOutWiz.Services.Models.ClientCategoryServices.ClientCategoryModel> commonCategories = new List<CutOutWiz.Services.Models.ClientCategoryServices.ClientCategoryModel>();
        private int commonCatogoryId = 0;
        private bool isOrderItemCategorySetUpPopupVisible = false;
        private OrderItemWiseCategoryModel orderItemWiseCategory = new OrderItemWiseCategoryModel();

        private int rawImageCount = 0;
        private int readyToDeliveryImageCount = 0;
        int productionDoneImageCount = 0;

        #endregion

        #endregion Global Properties
        #region Initialize Page
        protected override async Task OnInitializedAsync()
        {
            loginUser = _workContext.LoginUserInfo;
            authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            foreach (InternalOrderItemStatus item in Enum.GetValues(typeof(InternalOrderItemStatus)))
            {
                internalOrderItemStatusList.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }

            //on paramiter set
            spinShow = true;



            if (authState.User.IsInRole(PermissionConstants.Order_ViewAllProductionDoneOrder))
            {
                filterStatus = (byte)InternalOrderItemStatus.ProductionDone;
            }
            //end of paramiter set

            if (authState.User.IsInRole(PermissionConstants.Order_CanViewOrderItemStatusLog))
            {
                showActionColumn = true;
            }

            if (authState.User.IsInRole(PermissionConstants.AssignNewOrderItemToTeam))
            {
                await LoadTeamsForAssignOrderToSupportTeam(); //Load it here for button view when login as a team lead and want to assign order to support team .
            }


            //StateHasChanged();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // string ip = HttpContext.Connection?.RemoteIpAddress.ToString();

            if (firstRender)
            {
                order = await _orderService.GetByObjectId(objectId);
                fileServer = await _fileServerService.GetById((int)order.FileServerId);
                orderCompany = await _companyService.GetById(order.CompanyId);
                // Login User Company Information 
                company = await _companyService.GetById(loginUser.CompanyId);
                //Templates
                orderSOPTemplates = await _orderTemplateService.GetAllByOrderId((int)order.Id);
                foreach (var item in orderSOPTemplates)
                {
                    templateIds.Add(item.OrderSOP_Template_Id);
                }

                sopTemplates = await GetSOPTemplate(templateIds);
                orderSOPTemplateList = await GetOrderSOPTemplate(templateIds);
                //End of template

                //TODO: Move to component for assing team


                orderAttachementFiles = await _orderFileAttachmentService.GetOrdersAttachementByOrderId((int)order.Id);

                //Permisison wise order item load
                await LoadOrderItemForLoginUser();

                //Todo:Rakib
                var assignTeamsForOrders = await _orderTeamService.GetByOrderId((int)order.Id);

                foreach (var orderTeam in assignTeamsForOrders)
                {
                    selectedCompanyDedicatedTeams.Add((int)orderTeam.TeamId);
                }

                progressBarCurrentValue = 0;

                isLoading = false;
                spinShow = false;
                await LoadOrderItemColorRefGroupData();
                await LoadFeedBackImages();

                StateHasChanged();
            }
        }


        private void OnRowRender(RowRenderEventArgs<ClientOrderItemModel> args)
        {

            if (args.Data.ExpectedDeliveryDate != null)
            {
                //var arrivalTimePlus1_4Hours = args.Data.ArrivalTime.Value.AddHours(AutomatedAppConstant.VcDeadLineInHour);
                var expectedDeliveryDate = args.Data.ExpectedDeliveryDate ?? DateTime.Now;
                var timeLeft = expectedDeliveryDate - DateTime.Now;
                var minLeft = (int)timeLeft.TotalMinutes;

                var dangerWarningTime = (order.DeliveryDeadlineInMinute * AutomatedAppConstant.dangerWarningPercent) / 100;
                var dangerTime = (order.DeliveryDeadlineInMinute * AutomatedAppConstant.dangerPercent) / 100;

                if ((args.Data.Status < (int)InternalOrderStatus.Completed || args.Data.Status == (int)InternalOrderStatus.OrderPlacing) && args.Data.Status != 0)
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
        private async Task LoadOrderItemForLoginUser(bool isReloadOrder = true)
        {
            if (isReloadOrder)
            {
                order = await _orderService.GetByObjectId(objectId);
            }

            var loginUserTeams = await _teamMemberService.GetTeamIdsByContactIdAndOrderId(loginUser.ContactId, order.Id);
            if (loginUser.CompanyType == (int)CompanyType.Client)
            {

                var clientOrderItemsAll = await _clientOrderItemService.GetAllOrderItemByOrderIdForClient((int)order.Id);

                if (clientOrderItemsAll != null)
                {
                    clientOrderItemsAll = clientOrderItemsAll.Where(x => x.ExternalStatus != (int)ExternalOrderItemStatus.Rejected).ToList();
                }
                clientOrderItems = clientOrderItemsAll;
            }
            else
            {
                if (authState.User.IsInRole(PermissionConstants.Order_ViewAllAssignedOrderItemByAnotherTeamLead))
                {
                    if (loginUserTeams.Any() && loginUserTeams.FirstOrDefault().IsPrimaryTeam)
                    {
                        clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
                    }
                    else
                    {
                        if (loginUserTeams.Any())
                        {

                            clientOrderItems = await _clientOrderItemService.GetAllOrderAssignedItemByOrderIdContactIdTeamId((int)order.Id, loginUser.ContactId, loginUserTeams.FirstOrDefault().TeamId);
                        }
                    }


                }
                else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllAssignedOrderItem))
                {
                    clientOrderItems = await _clientOrderItemService.GetAllOrderAssignedItemByOrderId((int)order.Id, loginUser.ContactId);

                    CanUploadProductionDoneImages();
                }
                else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllProductionDoneOrder))
                {
                    var orderTeam = await _orderTeamService.GetByOrderIdAndTeamId((int)order.Id, loginUserTeams.FirstOrDefault().TeamId);

                    if (orderTeam.IsPrimary)
                    {
                        clientOrderItems = await _clientOrderItemService.GetEqualAndGreaterItemByStatus((int)order.Id, (byte)InternalOrderItemStatus.ProductionDone);
                    }
                }
                else  //TODO: add new permisison: Order.CanViewAllOrderItems
                {
                    clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
                }
            }

            SetFileCountStatusWiseForFolderView();

            if (loginUser.CompanyType == (int)CompanyType.Client)
            {
                await PopulateMissingRawFiles();
            }

        }

        private void SetFileCountStatusWiseForFolderView()
        {
            rawImageCount = clientOrderItems.Count(item => item.InternalFileInputPath != null);
            readyToDeliveryImageCount = clientOrderItems.Where(item => item.InternalFileOutputPath != null)
                                                       .GroupBy(item => item.InternalFileOutputPath)
                                                       .Select(group => group.First())
                                                       .Count();

            productionDoneImageCount = clientOrderItems.Where(item => item.ProductionDoneFilePath != null)
                                                       .GroupBy(item => item.ProductionDoneFilePath)
                                                       .Select(group => group.First())
                                                       .Count();
        }

        private async Task LoadOrderItemColorRefGroupData()
        {
            clientItemColorRefGroupList = await _clientOrderItemService.GetByClientOrderIdAndFileGroup((int)order.Id, (int)OrderItemFileGroup.ColorRef);
        }

        private async Task LoadFeedBackImages()
        {
            feedBackImages = await _feedBackReworkService.GetAllByClientOrderId(order.Id);
            //StateHasChanged();
        }
        private void PopulateQueryFilePath()
        {
            foreach (var tempItem in clientOrderItems)
            {
                if (!string.IsNullOrWhiteSpace(tempItem.InternalFileInputPath))
                {
                    tempItem.QueryFilePath = tempItem.InternalFileInputPath;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(tempItem.InternalFileOutputPath))
                {
                    var splitSpring = new string[] { order.OrderNumber };

                    var donePath = tempItem.InternalFileOutputPath.Replace("//", "/");

                    var pathArray = donePath.Split(splitSpring, StringSplitOptions.None);
                    var folderNameWithoutCompletedName = donePath.Split("Completed", StringSplitOptions.None);
                    tempItem.QueryFilePath = $"{pathArray[0]}/{order.OrderNumber}/{folderNameWithoutCompletedName[1]}";
                    tempItem.QueryFilePath = tempItem.QueryFilePath.Replace("//", "/");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(tempItem.ProductionDoneFilePath))
                {
                    //Remove Editor Names

                    var splitSpring = new string[] { order.OrderNumber };

                    var donePath = tempItem.ProductionDoneFilePath.Replace("//", "/");

                    var pathArray = donePath.Split(splitSpring, StringSplitOptions.None);
                    var folderNameWithoutEditor = donePath.Split("Production Done", StringSplitOptions.None);
                    var arrayCount = folderNameWithoutEditor.Count();
                    if (arrayCount > 1)
                    {

                        tempItem.QueryFilePath = $"{pathArray[0]}/{folderNameWithoutEditor[1]}";
                        tempItem.QueryFilePath = tempItem.QueryFilePath.Replace("//", "/");
                    }
                    else
                    {
                        var folderNameWithoutEditorRejected = donePath.Split("Rejected", StringSplitOptions.None);
                        tempItem.QueryFilePath = $"{pathArray[0]}/{folderNameWithoutEditorRejected[1]}";
                        tempItem.QueryFilePath = tempItem.QueryFilePath.Replace("//", "/");
                    }
                }
            }
        }
        #endregion end of inlialize page
        #region SOP Templates of order
        private async Task<List<SOPTemplateModel>> GetSOPTemplate(List<int> templateIds)
        {
            List<SOPTemplateModel> sopTemplates = new List<SOPTemplateModel>();

            foreach (var templateId in templateIds)
            {
                SOPTemplateModel sopTemplate = new SOPTemplateModel();
                sopTemplate = await _sopTemplateService.GetById(templateId);
                sopTemplates.Add(sopTemplate);
            }

            return sopTemplates;
        }
        #endregion
        #region Team Assing to Order
        async Task ShowAssignNewTeamAssignForOrderPopup()
        {

            team = new TeamModel();

            if (authState.User.IsInRole(PermissionConstants.AssignNewOrderToTeam))
            {
                teams = await _teamService.GetAll();
            }

            isAssignOrderSubmitting = false;
            isNewTeamAssignToOrderVisible = true;
        }

        //public async Task OnAssignedTeam(List<int> teamIds)
        //{
        //    await Task.Yield();
        //    selectedCompanyDedicatedTeams = teamIds;
        //}

        //TODO: Need to update code and UI for team assign.
        private async Task GiveAnotherTeamSupportPermissionForAOrder()
        {
            var dedicatedTeamsForOrder = await _companyTeamService.GetCompanyTeamByCompanyId(order.CompanyId);
            var assignTeamsForOrders = await _orderTeamService.GetByOrderId((int)order.Id);

            List<OrderTeamModel> teamOrders = new List<OrderTeamModel>();
            foreach (var teamId in selectedCompanyDedicatedTeams)
            {

                if (dedicatedTeamsForOrder.Exists(dt => dt.TeamId == teamId))
                {
                    OrderTeamModel orderTeam = new OrderTeamModel
                    {
                        OrderId = order.Id,
                        TeamId = teamId,
                        CreatedBy = loginUser.ContactId,
                        IsPrimary = true,
                        IsItemAssignToTeam = true
                    };
                    teamOrders.Add(orderTeam);
                }
                else
                {
                    OrderTeamModel orderTeam = new OrderTeamModel
                    {
                        OrderId = order.Id,
                        TeamId = teamId,
                        CreatedBy = loginUser.ContactId,
                        IsPrimary = false,
                        IsItemAssignToTeam = false
                    };
                    teamOrders.Add(orderTeam);
                }

            }

            var addResponse = await _orderTeamService.Insert(teamOrders, (int)order.Id);
            //Display message if success
            isNewTeamAssignToOrderVisible = false;
        }
        private async Task InsertAssingOrderItemToTeam()
        {
            try
            {
                spinShow = true;
                List<string> clientOrderItemIds = new List<string>();

                foreach (var orderItem in selectedFiles)
                {
                    if (orderItem.Status >= (int)InternalOrderItemStatus.Distributed || orderItem.Status == (int)InternalOrderItemStatus.AssignedForSupport)
                    {
                        await js.DisplayMessage("One Of your selected image is already assigned");
                        CloseAssignItemToTeamPopup();
                        return;
                    }
                    clientOrderItemIds.Add(orderItem.Id.ToString());
                }

                var result = await _clientOrderItemService.UpdateClientOrderItemTeamId((int)order.Id, newTeamId, clientOrderItemIds);

                //Update Assign Order
                OrderTeamModel orderTeam = new OrderTeamModel
                {
                    TeamId = newTeamId,
                    IsItemAssignToTeam = true,
                    OrderId = order.Id
                };
                await _orderTeamService.Update(orderTeam);

                if (result.IsSuccess)
                {
                    await UpdateOrderItem(InternalOrderItemStatus.AssignedForSupport);
                    //_orderTeamService.Insert
                    spinShow = false;
                    StateHasChanged();
                    await js.DisplayMessage("Assigned Succesfully");
                    spinShow = true;
                    StateHasChanged();
                    CloseAssignItemToTeamPopup();
                    MakeOrderItemUnselected();
                    //Todo:Rakib need to optimize
                    clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
                }
                else
                {
                    await js.DisplayMessage("Assigned Failed");
                }
                spinShow = false;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertAssingOrderItemToTeam",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.InsertAssingOrderItemToTeamError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        #endregion
        #region Order Item Assing
        private async Task AssignToEditor()
        {
            try
            {
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }

                clientOrderItem = new ClientOrderItemModel();

                isAssignOrderSubmitting = false;
                await ShowAssignOrderItemsToEditor();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AssignToEditor",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.AssignToEditorError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        async Task ShowAssignOrderItemsToEditor()
        {
            try
            {
                if (authState.User.IsInRole(PermissionConstants.AssignNewOrderToEditor))
                {
                    ContactListModel contact = await _teamService.GetByContactId(loginUser.ContactId);

                    if (contact != null)
                    {
                        var teamIdList = await GetTeamIdForOrderToLoadTeamMembers();
                        foreach (var teamId in teamIdList)
                        {
                            loginUserTeamMembers.AddRange(await _teamMemberService.GetTeamMemberListWithDetailsByTeamId(teamId));
                        }

                        //loginUserTeamMembers = await _teamMemberService.GetTeamMemberListWithDetailsByTeamId((int)order.AssignedTeamId);
                    }
                }
                isOrderItemsAssignToEditorPopupVisible = true;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "ShowAssignOrderItemsToEditor",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.ShowAssignOrderItemsToEditorError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        private async Task<List<int>> GetTeamIdForOrderToLoadTeamMembers()
        {
            var tempTeamList = new List<int>();
            try
            {

                loginUserTeamMembers = new List<TeamMemberListModel>();
                tempTeamList = new List<int>();
                var loginUserTeamIds = await _teamMemberService.GetTeamIdsByContactId(loginUser.ContactId);
                var assignTeamsForOrders = await _orderTeamService.GetByOrderId((int)order.Id);
                if (assignTeamsForOrders.Count() > 1)
                {

                    foreach (var id in loginUserTeamIds)
                    {
                        assignTeamsForOrders.Exists(ot => ot.TeamId == id);
                        tempTeamList.Add(id);
                    }

                }
                else
                {
                    tempTeamList.Add((int)assignTeamsForOrders.FirstOrDefault().TeamId);
                }

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetTeamIdForOrderToLoadTeamMembers",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.GetTeamIdForOrderToLoadTeamMembersError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return tempTeamList;
        }
        void CloseAssignOrderItemToEditorPopup()
        {
            isOrderItemsAssignToEditorPopupVisible = false;
        }
        async Task ShowAssignItemToTeamPopup()
        {
            try
            {
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }
                assignOrderToTeam = true;
                spinShow = true;
                await Task.Yield();
                clientOrderItem = new ClientOrderItemModel();

                isAssignItemToTeamPopupVisible = true;
                spinShow = false;
                assignOrderToTeam = false;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "ShowAssignItemToTeamPopup",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.ShowAssignItemToTeamPopupError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        void CloseAssignItemToTeamPopup()
        {
            isAssignItemToTeamPopupVisible = false;
        }
        void CloseAssignNewTeamForOrderPopup()
        {
            isNewTeamAssignToOrderVisible = false;

        }
        async Task LoadTeamsForAssignOrderToSupportTeam()
        {
            try
            {
                var loginUserTeams = await _teamMemberService.GetTeamIdsByContactIdAndOrderId(loginUser.ContactId, order.Id);

                OrderTeamModel teamOrder = null;
                if (loginUserTeams.FirstOrDefault() != null)
                {
                    teamOrder = await _orderTeamService.GetByOrderIdAndTeamId((int)order.Id, loginUserTeams.FirstOrDefault().TeamId);
                    if (teamOrder != null)
                    {
                        if (teamOrder.IsPrimary)
                        {
                            teams = await _teamService.GetByOrderId(order.Id);
                            if (teams != null && loginUserTeams.FirstOrDefault() != null)
                            {
                                teams.RemoveAll(team => team.Id == loginUserTeams.FirstOrDefault().TeamId);
                            }
                        }
                    }
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "LoadTeamsForAssignOrderToSupportTeam",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.LoadTeamsForAssignOrderToSupportTeamError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        private async Task DistributeOrderItemToEditor()
        {
            try
            {
                isAssignOrderSubmitting = true;
                spinShow = true;
                if (selectedTeamMemberIdForAssignFiles <= 0)
                {
                    spinShow = false;
                    StateHasChanged();
                    isAssignOrderSubmitting = false;
                    await js.DisplayMessage("Please Select At Least One Editor");
                    return;
                }
                await Task.Yield();


                List<OrderAssignedImageEditorModel> assignedImages = new List<OrderAssignedImageEditorModel>();
                var clientOrderItemIds = new List<string>();
                foreach (var selectedFile in selectedFiles)
                {
                    var orderItem = await _clientOrderItemService.GetById((int)selectedFile.Id);
                    if (orderItem.Status >= (int)InternalOrderItemStatus.Distributed || orderItem.Status == (int)InternalOrderItemStatus.AssignedForSupport)
                    {
                        if (orderItem.Status == (int)InternalOrderItemStatus.AssignedForSupport) //&& contact.TeamId == orderItem.TeamId)
                        {
                            OrderAssignedImageEditorModel supportOrderImage = new OrderAssignedImageEditorModel
                            {
                                OrderId = order.Id,
                                AssignByContactId = loginUser.ContactId,
                                AssignContactId = selectedTeamMemberIdForAssignFiles,
                                Order_ImageId = selectedFile.Id,
                                ObjectId = Guid.NewGuid().ToString(),
                                UpdatedByContactId = loginUser.ContactId
                            };
                            assignedImages.Add(supportOrderImage);
                            continue;
                        }
                        else
                        {
                            spinShow = false;
                            StateHasChanged();
                            await js.DisplayMessage("One Of your selected image is already assigned");
                            CloseAssignOrderItemToEditorPopup();
                            isAssignOrderSubmitting = false;
                            return;
                        }
                    }

                    else
                    {
                        OrderAssignedImageEditorModel assignedImage = new OrderAssignedImageEditorModel
                        {
                            OrderId = order.Id,
                            AssignByContactId = loginUser.ContactId,
                            AssignContactId = selectedTeamMemberIdForAssignFiles,
                            Order_ImageId = selectedFile.Id,
                            ObjectId = Guid.NewGuid().ToString(),
                            UpdatedByContactId = loginUser.ContactId

                        };
                        assignedImages.Add(assignedImage);
                    }
                    clientOrderItemIds.Add(selectedFile.Id.ToString());

                }

                var addResponse = await _orderAssignedImageEditorService.Insert(assignedImages);

                if (!addResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", addResponse.Message);
                    //Folder Structure Data Update
                    isAssignOrderSubmitting = false;
                    return;
                }

                //Update Status after assign to editor
                var loginUserTeamIds = await _teamMemberService.GetTeamIdsByContactId(selectedTeamMemberIdForAssignFiles);
                await _clientOrderItemService.UpdateClientOrderItemTeamId((int)order.Id, loginUserTeamIds.FirstOrDefault(), clientOrderItemIds);
                await UpdateOrderItem(InternalOrderItemStatus.Distributed);

                MakeOrderItemUnselected();

                //Update Status after assign to editor
                await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                await LoadOrderItemForLoginUser();
                await LoadSubFoldersAndFiles(currentFolderPath);
                folderNodeFilesList = folderNodes.Where(f => f.IsFolder == false).ToList().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                //Update Status after assign to editor
                isAssignOrderSubmitting = false;
                CloseAssignOrderItemToEditorPopup();
                spinShow = false;
                StateHasChanged();
                await js.DisplayMessage("Successfully Assigned");
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertAssingOrderToEditor",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.InsertAssingOrderToEditorError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        #endregion
        #region Image Preview

        string imageByteString = "";
        /// <summary>
        ///  Check File Extension Check
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool IsValidFileExtensionForConvert(string filePath, bool isForAttachment)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var attachmentExtensions = new List<string> { ".yml", ".yaml", ".txt", ".json", ".pdf", ".eml" };
                var orderItemExtensions = new List<string> { ".psd", ".tiff", ".tif" };
                var fileExtension = Path.GetExtension(filePath).ToLower();

                return isForAttachment
                    ? attachmentExtensions.Contains(fileExtension)
                    : orderItemExtensions.Contains(fileExtension);
            }
            return false;
        }


        async Task ShowImagePopup(int imageId)
        {

            //clientOrderItem = new ClientOrderItem();
            //clientOrderItem.Id = imageId;
            //clientOrderItem = await _clientOrderItemService.GetById(imageId);
            clientOrderItem = clientOrderItems.Where(x => x.Id == imageId).FirstOrDefault();


            if (IsValidFileExtensionForConvert(clientOrderItem.InternalFileInputPath, true))
            {
                var webHost = $"{this._webHostEnvironment.WebRootPath}";
                clientOrderItem.AttachmentFileByteStrings = await _convertion.ConvertFileToPNG(fileServer, clientOrderItem.InternalFileInputPath, webHost);
            }

            isRawBtnClicked = true;
            isShowImagePopup = true;
            StateHasChanged();
            isShowProductionDoneImagePopup = false;
            isShowCompletedImagePopup = false;

            isCompletedBtnClicked = false;

            StateHasChanged();

        }
        async Task ShowProductionDoneImagePopup(ClientOrderItemModel clientOrderItemVM, FileServerModel filerServerInfo = null)
        {

            //clientOrderItem = new ClientOrderItem();
            //clientOrderItem.Id = imageId;
            clientOrderItem = await _clientOrderItemService.GetById((int)clientOrderItemVM.Id);
            isShowProductionDoneImagePopup = true;
            isShowImagePopup = false;
            spinShow = true;
            this.StateHasChanged();


            isCompletedBtnClicked = true;
            if (isCompletedBtnClicked)
            {
                isRawBtnClicked = false;
            }
            if (isRawBtnClicked)
            {
                isCompletedBtnClicked = false;

            }
            spinShow = false;
            StateHasChanged();

            await js.InvokeVoidAsync("initDriveManager");
            this.StateHasChanged();
            // Here create base64string 

            var fileServervm = new FileServerViewModel()
            {
                Host = fileServer.Host,
                UserName = fileServer.UserName,
                Password = fileServer.Password
            };


            try
            {
                // here changes logic : TODO
                var filePath = "";
                if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                {
                    filePath = $"{fileServer.SubFolder}/{clientOrderItem.InternalFileOutputPath}";
                }
                else
                {
                    filePath = $"{clientOrderItem.InternalFileOutputPath}";
                }

                //var filePath = $"/KOW_NAS_STORAGE/KOW_ROOT_FOLDER/KowToMate_Software/{clientOrderItem.InternalFileOutputPath}";

                //var filePath = "/KOW_NAS_STORAGE/KOW_ROOT_FOLDER/KowToMate_Software/vc-20/2023/October/03.10.2023/Raw/vc-20-1181-03102023171603/37196508-1.jpg";



                using (var connector = await _fluentFtpService.CreateFtpClient(fileServervm))
                {
                    connector.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    connector.Config.ValidateAnyCertificate = true;
                    await connector.Connect();

                    CancellationToken cancellationToken = CancellationToken.None; // You can pass an appropriate CancellationToken if needed

                    byte[] downloadedBytes = await connector.DownloadBytes(filePath, cancellationToken);

                    downloadedBytes = ConvertTiffToJpegBytes(downloadedBytes);
                    // Convert the file content to Base64
                    imageByteString = Convert.ToBase64String(downloadedBytes);




                    var getExtention = Path.GetExtension(filePath).Split(".");

                    var imageUri = $"data:image/{getExtention[1]};base64,{imageByteString}";


                    await js.InvokeVoidAsync("init", imageUri, filePath);


                    Console.WriteLine("File uploaded to FTP and converted to Base64:");
                    //Console.WriteLine(base64String);
                    await connector.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }


        }


        private async Task PopulateMissingRawFiles()
        {
            await Task.Yield();
            if (clientOrderItems == null)
            {
                return;
            }
            var missingListOrderItems = clientOrderItems.Where(x => x.InternalFileInputPath == null).ToList();
            if (missingListOrderItems == null)
            {
                return;
            }
            foreach (var item in missingListOrderItems)
            {
                var matchedItem = clientOrderItems.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.InternalFileInputPath) == Path.GetFileNameWithoutExtension(item.InternalFileOutputPath));
                if (matchedItem != null)
                {
                    item.InternalFileInputPath = matchedItem.InternalFileInputPath;
                }
            }
        }

        private byte[] ConvertTiffToJpegBytes(byte[] tiffBytes)
        {
            using (MagickImage image = new MagickImage(tiffBytes))
            {
                // Set the output format to JPEG
                image.Format = MagickFormat.Jpg;

                // Convert the image to a memory stream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Save the image to the memory stream
                    image.Write(memoryStream);

                    // Convert the memory stream to a byte array
                    byte[] byteArray = memoryStream.ToArray();
                    return byteArray;

                    // Convert the byte array to a base64 string
                    //string base64String = Convert.ToBase64String(byteArray);

                    //return base64String;
                }
            }
        }

        private ImageCodecInfo GetJpegEncoder()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    return codec;
                }
            }
            return null;
        }





        async Task ShowCompletedImagePopup(ClientOrderItemModel clientOrderItemVM)
        {
            clientOrderItem = new ClientOrderItemModel();
            //clientOrderItem.Id = imageId;
            clientOrderItem = await _clientOrderItemService.GetById((int)clientOrderItemVM.Id);
            isShowCompletedImagePopup = true;
        }
        void CloseImagePopup()
        {
            isShowImagePopup = false;
        }
        void CloseProductionDoneImagePopup()
        {
            isShowProductionDoneImagePopup = false;
            isShowCompletedImagePopup = false;
            this.StateHasChanged();
        }
        void CloseCompletedImagePopup()
        {
            isShowCompletedImagePopup = false;
            this.StateHasChanged();
        }
        #endregion Image preview end
        #region Replace QC and production done files
        void ShowReplacePopup(ClientOrderItemModel clientOrderItemVM)
        {

            clientOrderItemForReplace = clientOrderItemVM;
            isReplaceInQcFilesPopupVisible = true;
        }
        void CloseReplacePopup()
        {
            isReplaceInQcFilesPopupVisible = false;
            clientOrderItemForReplace = new ClientOrderItemModel();
            loadReplaceFile = null;
            StateHasChanged();
        }
        #endregion
        #region Order Status Update
        private async Task UpdateOrder(ClientOrderModel clientOrder, InternalOrderStatus status)
        {
            if (status == InternalOrderStatus.AssignedForSupport)
            {
                status = InternalOrderStatus.Assigned;
            }
            clientOrder.InternalOrderStatus = (byte)status;
            clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(status));
            await _clientOrderService.UpdateClientOrderStatus(clientOrder);

            await AddOrderStatusChangeLog(clientOrder, status);
        }
        #endregion
        #region Update Order Item
        private async Task UpdateOrderItem(InternalOrderItemStatus status)
        {

            if (selectedFiles.Count > 0)
            {
                foreach (var selectedFile in selectedFiles)
                {
                    var statusHasChanged = false;
                    if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDistributed)
                    {
                        selectedFile.Status = (byte)InternalOrderItemStatus.ReworkInProduction;
                        selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkInProduction));
                        await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                        statusHasChanged = true;
                    }
                    else if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDone)
                    {
                        selectedFile.Status = (byte)InternalOrderItemStatus.ReworkQc;
                        selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkQc));
                        await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                        statusHasChanged = true;

                    }
                    else if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkQc && status == InternalOrderItemStatus.ReworkDistributed)
                    {
                        selectedFile.Status = (byte)InternalOrderItemStatus.ReworkDistributed;
                        selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkDistributed));
                        await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                        statusHasChanged = true;

                    }
                    else
                    {
                        if (selectedFile.Status < (byte)status)
                        {
                            selectedFile.Status = (byte)status;
                            selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
                            await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                            statusHasChanged = true;

                        }

                    }
                    if (statusHasChanged)
                    {
                        await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log
                    }


                }
            }
            else
            {
                foreach (var selectedFile in clientOrderItems)
                {
                    if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDistributed)
                    {
                        selectedFile.Status = (byte)InternalOrderItemStatus.ReworkInProduction;
                        selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkInProduction));
                        await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                    }
                    else
                    {
                        if (selectedFile.Status < (byte)status)
                        {
                            selectedFile.Status = (byte)status;
                            selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
                            await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                        }
                    }
                    await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log

                }
            }


            CanUploadProductionDoneImages();
        }

        #endregion

        #region Download and File go to InProgress Folder of FTP Server

        private async Task UpdateUIAsync(Action updateAction)
        {
            await InvokeAsync(() =>
            {
                updateAction?.Invoke();
                StateHasChanged();
            });
        }

        private async Task SingleDownload(bool canStatusChange = true)
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");
                ContactModel contactInfo = await _contactManager.GetById(loginUser.ContactId);

                var sharedFolderDownloadPath = "";
                if (contactInfo != null)
                {
                    sharedFolderDownloadPath = contactInfo.DownloadFolderPath;
                }

                timer.StartTimer();
                isSubmitting = true;
                spinShow = true;
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }
                if (selectedColorRefFiles.Count > 0)
                {
                    selectedFiles = selectedColorRefFiles;
                }
                var downloadPath = "";

                if (selectedFiles != null && selectedFiles?.Count > 0)
                {

                    if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
                    {
                        downloadPath = sharedFolderDownloadPath + $"\\{loginUser.FullName}\\" + $"{formattedDateTimeForDownload}";
                    }
                    else
                    {
                        downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}";
                    }
                    // delete previous folder
                    await _ftpFilePathService.ExistsFolderDelete(downloadPath);

                    var result = false;

                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    ClientOrderItemModel fileinfo = new ClientOrderItemModel();
                    var count = 0;
                    maxValue = selectedFiles.Count;
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        progressBarCurrentValue = 0.1;
                    }

                    foreach (var file in selectedFiles)
                    {
                        FileUploadModel model = new FileUploadModel();
                        model.fileName = file.FileName;
                        model.FtpUrl = serverInfo.Host;
                        model.userName = serverInfo.UserName;
                        model.password = serverInfo.Password;
                        model.OrderNumber = order.OrderNumber;
                        model.SubFolder = serverInfo.SubFolder;
                        model.UploadDirectory = Path.GetDirectoryName(file.InternalFileInputPath);//$"{fileInfo.ExteranlFileInputPath}";

                        model.DownloadFolderName = file.PartialPath;
                        model.Date = order.CreatedDate;
                        var dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                        model.ContactName = contactInfo.FirstName + " " + contactInfo.Id;

                        result = await _ftpService.SingleDownload(js, model, downloadPath);
                        await _ftpService.CreateFolderDownloadTime(model);
                        //await _ftpService.CopyFile(model);
                        count++;
                        progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

                        this.StateHasChanged();
                    }
                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        this.StateHasChanged();
                    }
                    if (result)
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        if (string.IsNullOrEmpty(sharedFolderDownloadPath))
                        {
                            await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, null);
                            this.StateHasChanged();
                        }
                    }
                    else
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Download Failed");
                        isSubmitting = false;
                        return;
                    }

                    //Update Status after editor download file
                    if (canStatusChange)
                    {
                        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

                        if (authState.User.IsInRole(PermissionConstants.Order_UpdateOrderAllItemStatusInProduction))
                        {
                            await UpdateOrderItem(InternalOrderItemStatus.InProduction);
                            MakeOrderItemUnselected();
                            await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus
                                                                                                   //Order Status Update
                        }

                        //After Download By Qc Order Status Change
                        if (authState.User.IsInRole(PermissionConstants.Order_UpdateOrderAllItemStatusInQc))
                        {
                            await UpdateOrderItemByQc(InternalOrderItemStatus.InQc);
                        }

                        await LoadOrderItemForLoginUser();
                    }

                    isSubmitting = false;
                    MakeOrderItemUnselected();
                    spinShow = false;
                    this.StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");
                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                    isSubmitting = false;
                    spinShow = false;
                    return;
                }
                this.StateHasChanged();

            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownload",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task SingleDownloadCompletedFile(bool canStatusChange = true)
        {
            try
            {
                timer.StartTimer();
                isSubmitting = true;
                spinShow = true;
                StateHasChanged();
                List<ClientOrderItemModel> statusCheckingFiles = new List<ClientOrderItemModel>();
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }
                if (selectedFiles != null && selectedFiles?.Count > 0)
                {
                    DateTime currentDateTime = DateTime.Now;
                    string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");
                    ContactModel contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    var sharedFolderDownloadPath = "";
                    if (contactInfo != null)
                    {
                        sharedFolderDownloadPath = contactInfo.DownloadFolderPath;
                        //sharedFolderDownloadPath = "//192.168.1.216/KD";
                    }
                    if (!Directory.Exists(sharedFolderDownloadPath))
                    {
                        sharedFolderDownloadPath = "";
                    }
                    var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}";

                    // delete previous folder
                    await _ftpFilePathService.ExistsFolderDelete(downloadPath);


                    var result = false;

                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);


                    var count = 0;
                    maxValue = selectedFiles.Count;
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        progressBarCurrentValue = 0.1;
                    }
                    FileUploadModel model = new FileUploadModel();

                    model.FtpUrl = serverInfo.Host;
                    model.userName = serverInfo.UserName;
                    model.password = serverInfo.Password;
                    model.SubFolder = serverInfo.SubFolder;
                    var dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                    using (var client = _ftpService.CreateFtpClient(model))
                    {
                        client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        client.Config.ValidateAnyCertificate = true;
                        client.Connect();
                        foreach (var file in selectedFiles)
                        {

                            if (file.Status >= (int)InternalOrderItemStatus.ReadyToDeliver)
                            {
                                model.UploadDirectory = Path.GetDirectoryName(file.InternalFileOutputPath);
                                int lastIndexOfOrderNumberFileOutputFilePath = file.InternalFileOutputPath.LastIndexOf(order.OrderNumber);
                                model.DownloadFolderName = "/" + file.InternalFileOutputPath.Substring(lastIndexOfOrderNumberFileOutputFilePath);
                                int lastIndexOfSalash = model.DownloadFolderName.LastIndexOf("/");
                                model.DownloadFolderName = model.DownloadFolderName.Substring(0, lastIndexOfSalash);
                                model.fileName = Path.GetFileName(file.InternalFileOutputPath);
                            }

                            if (file.Status < (byte)InternalOrderItemStatus.ReadyToDeliver)
                            {
                                statusCheckingFiles.Add(file);
                                continue;
                            }
                            if (!string.IsNullOrEmpty(file.InternalFileOutputPath))
                            {

                                model.OrderNumber = order.OrderNumber;
                                model.fileName = Path.GetFileName(file.InternalFileOutputPath);
                                model.UploadDirectory = Path.GetDirectoryName(file.InternalFileOutputPath);
                                model.Date = order.CreatedDate;
                                var dataSavePath = "";
                                model.ContactName = contactInfo.FirstName + " " + contactInfo.Id;
                                if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
                                {
                                    if (string.IsNullOrEmpty(model.DownloadFolderName))
                                    {
                                        dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + $"{formattedDateTimeForDownload}\\" + $"{order.OrderNumber}";
                                    }
                                    else
                                    {
                                        dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + $"{formattedDateTimeForDownload}\\" + $"{model.DownloadFolderName}";
                                    }

                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(model.DownloadFolderName))
                                    {
                                        dataSavePath = dlpath + $"\\{model.ContactName}\\{order.OrderNumber}";
                                    }
                                    else
                                    {
                                        dataSavePath = dlpath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
                                    }
                                }

                                var localPath = $"{dataSavePath}/{model.fileName}";
                                var remotePath = "";
                                if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                {
                                    remotePath = $"{serverInfo.SubFolder}/{model.UploadDirectory}/{model.fileName}";
                                }
                                else
                                {
                                    remotePath = $"{model.UploadDirectory}/{model.fileName}";
                                }

                                var downloadResult = client.DownloadFile(localPath, remotePath);

                                await _ftpService.CreateFolderDownloadTime(model);
                            }

                            //await _ftpService.CopyFile(model);
                            count++;
                            progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

                            this.StateHasChanged();
                        }
                        client.Disconnect();
                    }


                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        StateHasChanged();
                    }

                    if (string.IsNullOrEmpty(sharedFolderDownloadPath))
                    {
                        spinShow = true;
                        StateHasChanged();
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, null);
                        spinShow = true;
                        StateHasChanged();
                    }


                    //Update Status after editor download file
                    if (canStatusChange)
                    {
                        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

                        if (authState.User.IsInRole(PermissionConstants.Order_UpdateOrderAllItemStatusInProduction))
                        {
                            await UpdateOrderItem(InternalOrderItemStatus.InProduction);
                            MakeOrderItemUnselected();
                            await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus
                                                                                                   //Order Status Update
                        }

                        //After Download By Qc Order Status Change
                        if (authState.User.IsInRole(PermissionConstants.Order_UpdateOrderAllItemStatusInQc))
                        {
                            await UpdateOrderItemByQc(InternalOrderItemStatus.InQc);
                        }

                        await LoadOrderItemForLoginUser();
                    }

                    if (statusCheckingFiles.Count > 0)
                    {
                        var text = await CreateTextFileName(statusCheckingFiles);
                        isSubmitting = false;
                        MakeOrderItemUnselected();
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage($"This files not for ready to deliver. \n{text}");
                        return;
                    }

                    isSubmitting = false;
                    MakeOrderItemUnselected();
                    spinShow = false;
                    StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");
                }
                else
                {
                    spinShow = false;
                    isSubmitting = false;
                    StateHasChanged();
                    await js.DisplayMessage("Select at least one file !");
                    return;
                }
                StateHasChanged();
            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownloadCompletedFile",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadCompletedFileError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task EditorRejectedFileDownload(bool canStatusChange = true)
        {
            try
            {
                var result = false;
                spinShow = true;
                StateHasChanged();
                timer.StartTimer();
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");

                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                var sharedFolderDownloadPath = "";

                if (contactInfo != null)
                {
                    sharedFolderDownloadPath = contactInfo.DownloadFolderPath;
                }
                if (!Directory.Exists(sharedFolderDownloadPath))
                {
                    sharedFolderDownloadPath = "";
                }

                if (selectedFiles != null)
                    isSubmitting = true;
                List<ClientOrderItemModel> statusCheckingFiles = new List<ClientOrderItemModel>();
                List<ClientOrderItemModel> ErrorDownloadFiles = new List<ClientOrderItemModel>();

                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }

                if (selectedFiles != null && selectedFiles?.Count > 0)
                {

                    // Delete Previous Folder 
                    var downloadpath = await _ftpFilePathService.ExistsFolderDelete($"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}");
                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    var count = 0;
                    maxValue = selectedFiles.Count;
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        progressBarCurrentValue = 0.1;
                    }

                    FileUploadModel model = new FileUploadModel
                    {
                        FtpUrl = serverInfo.Host,
                        userName = serverInfo.UserName,
                        password = serverInfo.Password,
                        SubFolder = serverInfo.SubFolder,
                        OrderNumber = order.OrderNumber,
                    };

                    model.Date = order.CreatedDate;
                    var dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                    var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    model.ContactName = contactInfo.FirstName.Trim() + " " + contactInfo.Id;

                    using (var client = _ftpService.CreateFtpClient(model))
                    {
                        client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        client.Config.ValidateAnyCertificate = true;
                        client.AutoConnect();
                        foreach (var orderItem in selectedFiles)
                        {
                            try
                            {
                                if (orderItem.Status == (byte)InternalOrderItemStatus.ReworkDistributed || orderItem.Status == (byte)InternalOrderItemStatus.ReworkInProduction)
                                {
                                    if (!string.IsNullOrEmpty(orderItem.ProductionDoneFilePath))
                                    {
                                        model.fileName = Path.GetFileName(orderItem.ProductionDoneFilePath);
                                        model.UploadDirectory = Path.GetDirectoryName(orderItem.ProductionDoneFilePath);//$"{fileInfo.ExteranlFileInputPath}";

                                        var dataSavePath = "";
                                        if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
                                        {
                                            if (string.IsNullOrEmpty(orderItem.PartialPath))
                                            {
                                                dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Rejected\\" + $"{formattedDateTimeForDownload}" + $"{order.OrderNumber}";
                                            }
                                            else
                                            {
                                                dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Rejected\\" + $"{formattedDateTimeForDownload}" + $"{orderItem.PartialPath}";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(orderItem.PartialPath))
                                            {
                                                dataSavePath = dlpath + $"\\{model.ContactName}\\{order.OrderNumber}";
                                            }
                                            else
                                            {
                                                dataSavePath = dlpath + $"\\{model.ContactName}\\{orderItem.PartialPath}";
                                            }
                                        }

                                        var localPath = $"{dataSavePath}/{model.fileName}";
                                        var remotePath = $"{model.UploadDirectory}/{model.fileName}";

                                        if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                        {
                                            remotePath = $"{serverInfo.SubFolder}/{remotePath}";
                                        }

                                        var downloadResponse = client.DownloadFile(localPath, remotePath);

                                        if (downloadResponse.Equals(FtpStatus.Success))
                                        {
                                            var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(orderItem, InternalOrderItemStatus.ReworkDistributed);
                                            // Update Order Item Status
                                            if (orderItemStatusUpdate)
                                            {
                                                await AddOrderItemStatusChangeLog(orderItem, InternalOrderItemStatus.ReworkDistributed);
                                            }
                                            await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, localPath, loginUser.ContactId);

                                        }
                                    }
                                }
                                else
                                {
                                    statusCheckingFiles.Add(orderItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrorDownloadFiles.Add(orderItem);
                                result = false;
                            }

                            //await _ftpService.CopyFile(model);
                            count++;
                            progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

                            this.StateHasChanged();
                        }
                        client.Disconnect();
                    }
                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        this.StateHasChanged();
                    }
                    if (string.IsNullOrWhiteSpace(sharedFolderDownloadPath))
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, null);
                        this.StateHasChanged();
                    }

                    // Order Status Update
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));
                    await LoadOrderItemForLoginUser();

                    if (ErrorDownloadFiles != null && ErrorDownloadFiles.Any())
                    {
                        var text = await CreateTextFileName(ErrorDownloadFiles);
                        isSubmitting = false;
                        MakeOrderItemUnselected();
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage($"This files are not downloaded for inner Exception. \n{text}");
                        return;
                    }

                    if (statusCheckingFiles.Count > 0)
                    {
                        var text = await CreateTextFileName(statusCheckingFiles);
                        isSubmitting = false;
                        MakeOrderItemUnselected();
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage($"This File List Not Download Because of some files are not rework distributed. \n{text}");
                        return;
                    }
                    if (result)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Download Failed");
                        isSubmitting = false;
                        return;
                    }
                    isSubmitting = false;
                    MakeOrderItemUnselected();
                    spinShow = false;
                    this.StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");
                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                    isSubmitting = false;
                    spinShow = false;
                    return;
                }
                this.StateHasChanged();
            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "EditorRejectedFileDownload",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.EditorRejectedFileDownloadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        private async Task SingleDownloadForClient(bool canStatusChange = true)
        {
            try
            {
                spinShow = true;
                if (selectedFiles != null)
                {
                    if (selectedFiles.Count == 0)
                    {
                        await js.DisplayMessage("Select at least one file !");
                        return;
                    }
                    isSubmitting = true;
                    spinShow = true;
                    var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}";
                    // delete previous folder
                    await _ftpFilePathService.ExistsFolderDelete(downloadPath);

                    var result = false;

                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    ClientOrderItemModel fileinfo = new ClientOrderItemModel();
                    var count = 0;
                    maxValue = selectedFiles.Count;
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        progressBarCurrentValue = 0.1;
                    }
                    foreach (var file in selectedFiles)
                    {
                        FileUploadModel model = new FileUploadModel();
                        model.fileName = file.FileName;
                        model.FtpUrl = serverInfo.Host;
                        model.userName = serverInfo.UserName;
                        model.password = serverInfo.Password;
                        model.SubFolder = serverInfo.SubFolder;
                        model.OrderNumber = order.OrderNumber;
                        model.UploadDirectory = System.IO.Path.GetDirectoryName(file.InternalFileOutputPath);//$"{fileInfo.ExteranlFileInputPath}";

                        model.DownloadFolderName = file.PartialPath;
                        model.Date = order.CreatedDate;
                        var dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                        model.ContactName = contactInfo.FirstName + " " + contactInfo.Id;

                        result = await _ftpService.SingleDownload(js, model, dlpath);
                        await _ftpService.CreateFolderDownloadTime(model);
                        //await _ftpService.CopyFile(model);
                        count++;
                        progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                        this.StateHasChanged();
                    }
                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        this.StateHasChanged();
                    }
                    if (result)
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, null);
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Download Succesfully");
                    }

                    else
                    {
                        await js.DisplayMessage("Download Failed");
                    }

                    isSubmitting = false;
                    MakeOrderItemUnselected();
                    spinShow = false;
                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                }
                isSubmitting = false;
                MakeOrderItemUnselected();
                spinShow = false;
                this.StateHasChanged();

            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownloadForClient",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadForClientError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

        /// <summary>
        /// Editor Download new implementation
        /// </summary>
        /// <param name="canStatusChange"></param>
        /// <returns></returns>
        /// 

        private async Task SingleDownloadEditor(bool canStatusChange = true)
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");

                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                var sharedFolderDownloadPath = "";

                if (contactInfo != null)
                {
                    sharedFolderDownloadPath = contactInfo.DownloadFolderPath;
                }

                if (!Directory.Exists(sharedFolderDownloadPath))
                {
                    sharedFolderDownloadPath = "";
                }

                spinShow = true;
                isProgressBar = true;
                StateHasChanged();
                if (isProgressBar)
                {
                    spinShow = false;
                    progressBarCurrentValue = 0.7;
                }
                isSubmitting = true;
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }
                if (selectedFiles != null && selectedFiles.Any())
                {
                    //var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}";
                    // delete previous folder
                    await _ftpFilePathService.ExistsFolderDelete(downloadPath);


                    var dlpath = "";

                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    var count = 0;
                    maxValue = selectedFiles.Count;

                    FileUploadModel model = new FileUploadModel();

                    model.FtpUrl = serverInfo.Host;
                    model.userName = serverInfo.UserName;
                    model.password = serverInfo.Password;
                    model.SubFolder = serverInfo.SubFolder;
                    model.OrderNumber = order.OrderNumber;

                    model.Date = order.CreatedDate;
                    dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                    var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    model.ContactName = contactInfo.FirstName.Trim() + " " + contactInfo.Id;

                    const int MaxConcurrentDownloads = 10;
                    var semaphore = new SemaphoreSlim(MaxConcurrentDownloads);
                    // Initialize the FTP client once and reuse it
                    var downloadTasks = selectedFiles.Select(async orderItem =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            using (var client = _ftpSharpLibraryService.CreateAsyncFtpClient(model))
                            {

                                client.Encoding = System.Text.Encoding.UTF8;
                                await client.Connect();

                                model.fileName = orderItem.FileName;
                                // if file life start with production done status, so cant file download and status not changes.
                                if (!string.IsNullOrWhiteSpace(orderItem.InternalFileInputPath))
                                {
                                    model.UploadDirectory = System.IO.Path.GetDirectoryName(orderItem.InternalFileInputPath);//$"{fileInfo.ExteranlFileInputPath}";
                                }
                                else
                                {
                                    model.UploadDirectory = System.IO.Path.GetDirectoryName(orderItem.ProductionDoneFilePath);

                                }//model.DownloadFolderName = orderItem.PartialPath;
                                var dataSavePath = "";
                                if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
                                {
                                    if (string.IsNullOrEmpty(orderItem.PartialPath))
                                    {
                                        dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{order.OrderNumber}";
                                    }
                                    else
                                    {
                                        dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{orderItem.PartialPath}";
                                    }

                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(orderItem.PartialPath))
                                    {
                                        dataSavePath = dlpath + $"\\{model.ContactName}\\{order.OrderNumber}";
                                    }
                                    else
                                    {
                                        dataSavePath = dlpath + $"\\{model.ContactName}\\{orderItem.PartialPath}";
                                    }
                                }


                                var localPath = $"{dataSavePath}/{model.fileName}";
                                var remotePath = $"{model.UploadDirectory}/{model.fileName}";

                                if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                {
                                    remotePath = $"{serverInfo.SubFolder}/{remotePath}";
                                }
                                remotePath = remotePath.Replace("\\", "/");
                                var downloadResponse = await client.DownloadFile(remotePath, localPath);

                                if (downloadResponse.IsSuccess && downloadResponse.StatusCode == 200)
                                {
                                    var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(orderItem, InternalOrderItemStatus.InProduction);
                                    // Update Order Item Status
                                    if (orderItemStatusUpdate)
                                    {
                                        await AddOrderItemStatusChangeLog(orderItem, InternalOrderItemStatus.InProduction);
                                    }
                                    await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, localPath, loginUser.ContactId);
                                }

                                //Add This Folder in pn due to upload completed file from editor pc
                                var uploadedPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Completed\\" + "_uploaded\\";
                                if (!Directory.Exists(uploadedPath))
                                {
                                    Directory.CreateDirectory(uploadedPath);
                                }

                                //Add This Folder in pn due to upload completed file from editor pc

                                count++;

                                progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                                await client.Disconnect();
                                StateHasChanged();

                            }
                        }
                        catch (Exception ex)
                        {
                            await js.DisplayMessage(ex.InnerException.ToString());
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    await Task.WhenAll(downloadTasks);
                    //spinShow = true;
                    //StateHasChanged();

                    await _ftpService.CreateFolderDownloadTime(model);
                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        StateHasChanged();
                    }

                    if (string.IsNullOrEmpty(sharedFolderDownloadPath))
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, null);
                    }

                    StateHasChanged();

                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                    await LoadOrderItemForLoginUser();
                    if ((fileViewMode == 2 || fileViewMode == 3) && folderNodeFilesList != null && folderNodeFilesList.Count > 0)
                    {
                        foreach (var selectedFile in selectedFiles)
                        {
                            var orderFile = folderNodeFilesList.Find(f => f.OrderItemId == selectedFile.Id);
                            folderNodeFilesList.RemoveAll(f => f.OrderItemId == selectedFile.Id);
                            var clientOrderItem = clientOrderItems.Find(oi => oi.Id == selectedFile.Id);

                            if (orderFile.Status == (byte)(InternalOrderItemStatus.Distributed))
                            {
                                orderFile.Status = (byte)(InternalOrderItemStatus.InProduction);
                            }
                            else if (orderFile.Status == (byte)(InternalOrderItemStatus.ReworkDistributed))
                            {
                                orderFile.Status = (byte)(InternalOrderItemStatus.ReworkInProduction);
                            }
                            orderFile.EditorName = clientOrderItem.EditorName;
                            folderNodeFilesList.Add(orderFile);
                        }
                    }
                    isSubmitting = false;
                    spinShow = false;
                    MakeOrderItemUnselected();
                    StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");


                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                    isSubmitting = false;
                    spinShow = false;
                    StateHasChanged();
                    return;
                }
                //Update Status after editor download file
            }
            catch (Exception ex)
            {
                isProgressBar = false;
                spinShow = false;
                StateHasChanged();
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownloadEditor",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadEditorError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        #region Folder view download features
        private async Task DownloadSelectedFilesWithoutListView(int statusWiseFile, InternalOrderItemStatus? status = null, string? message = null, string? whichRoleDownloadFile = null)
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");

                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                if (contactInfo == null) return;
                // Get Shared Folder 
                string sharedFolderDownloadPath = Directory.Exists(contactInfo.DownloadFolderPath) ? contactInfo.DownloadFolderPath : contactInfo.DownloadFolderPath;

                spinShow = true;
                isProgressBar = true;
                StateHasChanged();

                if (isProgressBar)
                {
                    spinShow = false;
                    progressBarCurrentValue = 0.7;
                }

                isSubmitting = true;
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }

                if (selectedFiles == null || !selectedFiles.Any())
                {
                    await js.DisplayMessage("Select at least one file!");
                    ResetProgress();
                    return;
                }

                string temporaryDownloadPath = Path.Combine(this._webHostEnvironment.WebRootPath, "TempDownload", $"{contactInfo.FirstName} {contactInfo.Id}");

                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                if (serverInfo == null) return;

                var model = new FileUploadModel
                {
                    FtpUrl = serverInfo.Host,
                    userName = serverInfo.UserName,
                    password = serverInfo.Password,
                    SubFolder = serverInfo.SubFolder,
                    OrderNumber = order.OrderNumber,
                    Date = order.CreatedDate,
                    ContactName = $"{contactInfo.FirstName.Trim()} {contactInfo.Id}"
                };

                const int MaxConcurrentDownloads = 10;
                var semaphore = new SemaphoreSlim(MaxConcurrentDownloads);

                int count = 0;
                maxValue = selectedFiles.Count;
                var downloadTasks = selectedFiles.Select(async orderItem =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        using (var client = _ftpSharpLibraryService.CreateAsyncFtpClient(model))
                        {
                            client.Encoding = System.Text.Encoding.UTF8;
                            await client.Connect();

                            model.fileName = orderItem.FileName;
                            model.UploadDirectory = await GetDownloadFilePathFromClientOrderItem(orderItem, statusWiseFile);

                            string dataSavePath = await CombineDataSavePath(sharedFolderDownloadPath, model, orderItem.PartialPath, formattedDateTimeForDownload,whichRoleDownloadFile);
                            string localPath = Path.Combine(dataSavePath, model.fileName);
                            string remotePath = Path.Combine(serverInfo.SubFolder ?? "", model.UploadDirectory, model.fileName).Replace("\\", "/");

                            var downloadResponse = await client.DownloadFile(remotePath, localPath);

                            if (downloadResponse.IsSuccess && downloadResponse.StatusCode == 200)
                            {
                                await ItemStatusUpdateAndInsertLogAndCompletedFolderCreate(orderItem, localPath, status, model.ContactName, sharedFolderDownloadPath, whichRoleDownloadFile);
                            }

                            UpdateProgress(ref count);
                            await client.Disconnect();
                            StateHasChanged();
                        }
                    }
                    catch (Exception ex)
                    {
                        await js.DisplayMessage(ex.InnerException?.ToString() ?? ex.Message);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(downloadTasks);

                await _ftpService.CreateFolderDownloadTime(model);
                await ZipDownload(sharedFolderDownloadPath, contactInfo, order, temporaryDownloadPath);
                // Update Order Status
                await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));
                await LoadOrderItemForLoginUser();
                UpdateFolderNodeFileList(selectedFiles.ToList());
                ResetProgress();

                await js.DisplayMessage(message ?? "Download Successfully");
            }
            catch (Exception ex)
            {
                await HandleError(ex, order);
            }
        }
        private async Task<string> GetDownloadFilePathFromClientOrderItem(ClientOrderItemModel orderItem, int statusWiseFile)
        {
            return statusWiseFile switch
            {
                1 => Path.GetDirectoryName(orderItem.InternalFileInputPath),
                2 => Path.GetDirectoryName(orderItem.ProductionDoneFilePath),
                3 => Path.GetDirectoryName(orderItem.InternalFileOutputPath),
                _ => ""
            };
        }

        private async Task<string> CombineDataSavePath(string sharedFolderDownloadPath, FileUploadModel model, string? partialPath, string formattedDateTimeForDownload, string ? whichRoleDownloadFile = null)
        {
            if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
            {
                if (!string.IsNullOrWhiteSpace(whichRoleDownloadFile))
                {
                    return sharedFolderDownloadPath + $"/{model.ContactName} ({whichRoleDownloadFile})" + "/Raw/" + formattedDateTimeForDownload + $"/{partialPath ?? model.OrderNumber}/";
                }
                return sharedFolderDownloadPath + $"/{model.ContactName}" + "/Raw/" + formattedDateTimeForDownload + $"/{partialPath ?? model.OrderNumber}/";
            }
            return Path.Combine(this._webHostEnvironment.WebRootPath, "TempDownload", model.ContactName, partialPath ?? model.OrderNumber);
        }

        private async Task ItemStatusUpdateAndInsertLogAndCompletedFolderCreate(ClientOrderItemModel orderItem, string localPath, InternalOrderItemStatus? status, string contactName,string sharedFolderDownloadPath, string? whichRoleDownloadFile)
        {
            if (status != null)
            {
                var orderItemChangeStatus = status ?? InternalOrderItemStatus.Distributed;
                bool orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(orderItem, orderItemChangeStatus);
                if (orderItemStatusUpdate)
                {
                    await AddOrderItemStatusChangeLog(orderItem, orderItemChangeStatus);
                }
            }

            await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, localPath, loginUser.ContactId);
            var uploadedPath = "";
            if (!string.IsNullOrWhiteSpace(whichRoleDownloadFile))
            {
                uploadedPath = sharedFolderDownloadPath + $"/{contactName} ({whichRoleDownloadFile})"+ "/Completed" + "/_uploaded/";
            }
            else
            {
                uploadedPath = sharedFolderDownloadPath + $"/{contactName}" + "/Completed" + "/_uploaded/";
            }
            if (!Directory.Exists(uploadedPath))
            {
                Directory.CreateDirectory(uploadedPath);
            }
        }

        private void UpdateProgress(ref int count)
        {
            count++;
            progressBarCurrentValue = Math.Round((float)(100 / maxValue) * count, 2);
        }

        private async Task ZipDownload(string sharedFolderDownloadPath, ContactModel contactInfo, ClientOrderModel order, string tempDownloadPath)
        {
            if (progressBarCurrentValue == 100)
            {
                progressBarCurrentValue = 0;
                spinShow = true;
                StateHasChanged();
            }

            if (string.IsNullOrEmpty(sharedFolderDownloadPath))
            {
                await _downloadService.CreateZipAndDownload(contactInfo, order, tempDownloadPath, null, null, null);
            }
        }

        private void UpdateFolderNodeFileList(List<ClientOrderItemModel> selectedFiles)
        {
            if ((fileViewMode == 2 || fileViewMode == 3) && folderNodeFilesList?.Count > 0)
            {
                foreach (var selectedFile in selectedFiles)
                {
                    var orderFile = folderNodeFilesList.Find(f => f.OrderItemId == selectedFile.Id);
                    if (orderFile != null)
                    {
                        folderNodeFilesList.RemoveAll(f => f.OrderItemId == selectedFile.Id);

                        var clientOrderItem = clientOrderItems.Find(oi => oi.Id == selectedFile.Id);
                        orderFile.Status = GetUpdatedOrderFileStatus(orderFile.Status);
                        orderFile.EditorName = clientOrderItem?.EditorName;

                        folderNodeFilesList.Add(orderFile);
                    }
                }
            }
        }

        private byte GetUpdatedOrderFileStatus(byte? currentStatus = null)
        {
            return currentStatus switch
            {
                (byte)InternalOrderItemStatus.Distributed => (byte)InternalOrderItemStatus.InProduction,
                (byte)InternalOrderItemStatus.ReworkDistributed => (byte)InternalOrderItemStatus.ReworkInProduction,
                _ => currentStatus ?? default(byte) // Handle null with a default value
            };
        }

        private void ResetProgress()
        {
            isSubmitting = false;
            spinShow = false;
            StateHasChanged();
        }

        private async Task HandleError(Exception ex, ClientOrderModel order)
        {
            isProgressBar = false;
            spinShow = false;
            StateHasChanged();

            var activity = new CommonActivityLogViewModel
            {
                PrimaryId = (int)order.Id,
                ActivityLogFor = (int)ActivityLogForConstants.Order,
                loginUser = loginUser,
                ErrorMessage = ex.Message,
                MethodName = "DownloadSelectedFilesWithoutListView",
                RazorPage = "OrderDetails.razor.cs",
                Category = (int)ActivityLogCategory.SingleDownloadEditorError,
            };
            await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
            await js.DisplayMessage(ex.Message);
        }

        #endregion Folder view download features

        private async Task SingleDownloadQC(bool canStatusChange = true)
        {
            try
            {

                var downloadMethodStartTime = DateTime.Now;
                var totalDownloadOperationTime = 0;


                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");
                ContactModel contact = await _contactManager.GetById(loginUser.ContactId);
                var sharedFolderDownloadPath = "";
                if (contact != null)
                {
                    sharedFolderDownloadPath = contact.DownloadFolderPath;
                }

                if (!Directory.Exists(sharedFolderDownloadPath))
                {
                    sharedFolderDownloadPath = "";
                }

                spinShow = true;
                isProgressBar = true;
                StateHasChanged();
                if (isProgressBar)
                {
                    spinShow = false;
                    progressBarCurrentValue = 0.7;
                }
                isSubmitting = true;
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }
                if (selectedFiles != null && selectedFiles.Any())
                {
                    var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contact.FirstName + " " + contact.Id}";

                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    var count = 0;
                    maxValue = selectedFiles.Count;
                    FileUploadModel model = new FileUploadModel();

                    model.FtpUrl = serverInfo.Host;
                    model.userName = serverInfo.UserName;
                    model.password = serverInfo.Password;
                    model.SubFolder = serverInfo.SubFolder;
                    model.OrderNumber = order.OrderNumber;


                    // Define a semaphore to limit the number of concurrent downloads
                    const int MaxConcurrentDownloads = 10;
                    var semaphore = new SemaphoreSlim(MaxConcurrentDownloads);

                    // Initialize the FTP client once and reuse it
                    var downloadTasks = selectedFiles.Select(async file =>
                    {

                        await semaphore.WaitAsync();


                        //foreach (var file in selectedFiles)
                        //{
                        try
                        {
                            if (string.IsNullOrWhiteSpace(file.ProductionDoneFilePath))
                            {
                                count++;
                                progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                                StateHasChanged();
                                //return;
                                semaphore.Release();
                            }

                            using (var client = _ftpSharpLibraryService.CreateAsyncFtpClient(model))
                            {
                                client.Encoding = System.Text.Encoding.UTF8;

                                await client.Connect();
                                if (file.Status >= (int)InternalOrderItemStatus.ReadyToDeliver)
                                {
                                    model.UploadDirectory = Path.GetDirectoryName(file.InternalFileOutputPath);
                                    int lastIndexOfOrderNumberFileOutputFilePath = file.InternalFileOutputPath.LastIndexOf(order.OrderNumber);
                                    model.DownloadFolderName = "/" + file.InternalFileOutputPath.Substring(lastIndexOfOrderNumberFileOutputFilePath);
                                    int lastIndexOfSalash = model.DownloadFolderName.LastIndexOf("/");
                                    model.DownloadFolderName = model.DownloadFolderName.Substring(0, lastIndexOfSalash);
                                    model.fileName = Path.GetFileName(file.InternalFileOutputPath);
                                }
                                else
                                {
                                    //Add BY Rakib
                                    if (string.IsNullOrEmpty(file.InternalFileInputPath))
                                    {
                                        int lastIndexOfOrderNumberInProductionDoneFilePath = file.ProductionDoneFilePath.LastIndexOf(order.OrderNumber);
                                        model.DownloadFolderName = "/" + file.ProductionDoneFilePath.Substring(lastIndexOfOrderNumberInProductionDoneFilePath);
                                        int lastIndexOfSalash = model.DownloadFolderName.LastIndexOf("/");
                                        model.DownloadFolderName = model.DownloadFolderName.Substring(0, lastIndexOfSalash);
                                    }
                                    else
                                    {
                                        model.DownloadFolderName = file.PartialPath;
                                    }
                                    model.UploadDirectory = Path.GetDirectoryName(file.ProductionDoneFilePath);

                                    model.fileName = Path.GetFileName(file.ProductionDoneFilePath);
                                }

                                model.Date = order.CreatedDate;
                                var dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                                model.ContactName = contact.FirstName.Trim() + " " + contact.Id;

                                if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
                                {

                                    var dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName} (QC)\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{model.DownloadFolderName}/{model.fileName}";
                                    var dataSourcePath = $"{model.UploadDirectory}/{model.fileName}";


                                    if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                    {
                                        dataSourcePath = $"{serverInfo.SubFolder}/{dataSourcePath}";
                                    }

                                    var startTime = DateTime.Now;

                                    //var downloadResponse = await client.DownloadFile(dataSavePath, dataSourcePath,FtpLocalExists.Overwrite);

                                    dataSourcePath = dataSourcePath.Replace("\\", "/");

                                    var downloadResponse = await client.DownloadFile(dataSourcePath, dataSavePath);

                                    var timeDifference = DateTime.Now.Subtract(startTime).TotalSeconds;

                                    Console.WriteLine(timeDifference);

                                    totalDownloadOperationTime += (int)timeDifference;

                                    //Add This Folder in pn due to upload completed file from qc pc
                                    var uploadedPath = sharedFolderDownloadPath + $"\\{model.ContactName} (QC)\\" + "Completed\\" + "_uploaded\\";
                                    if (!Directory.Exists(uploadedPath))
                                    {
                                        Directory.CreateDirectory(uploadedPath);
                                    }
                                    if (downloadResponse.IsSuccess && downloadResponse.StatusCode == 200)
                                    {
                                        var status = InternalOrderItemStatus.InQc;

                                        if (file.Status == (byte)InternalOrderItemStatus.ReworkDone)
                                        {
                                            status = InternalOrderItemStatus.ReworkQc;
                                        }

                                        var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);
                                        // Update Order Item Status
                                        if (orderItemStatusUpdate)
                                        {
                                            await AddOrderItemStatusChangeLog(file, status);
                                        }
                                        await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)file.Id, dataSavePath, loginUser.ContactId);
                                    }

                                }
                                else
                                {

                                    var dataSavePath = dlpath + $"\\{model.ContactName}\\{model.DownloadFolderName}\\{model.fileName}";

                                    var dataSourcePath = $"{model.UploadDirectory}/{model.fileName}";

                                    if (!string.IsNullOrWhiteSpace(model.SubFolder))
                                    {
                                        dataSourcePath = $"{model.SubFolder}/{model.UploadDirectory}/{model.fileName}";
                                    }

                                    var downloadResponse = await client.DownloadFile(dataSourcePath, dataSavePath);

                                    if (downloadResponse.IsSuccess && downloadResponse.StatusCode == 200)
                                    {
                                        var status = InternalOrderItemStatus.InQc;

                                        if (file.Status == (byte)InternalOrderItemStatus.ReworkDone)
                                        {
                                            status = InternalOrderItemStatus.ReworkQc;
                                        }

                                        var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);
                                        // Update Order Item Status
                                        if (orderItemStatusUpdate)
                                        {
                                            await AddOrderItemStatusChangeLog(file, status);
                                        }
                                        await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)file.Id, dataSavePath, loginUser.ContactId);
                                    }
                                }

                                count++;
                                progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

                                //await client.Disconnect();
                                await client.Disconnect();

                                StateHasChanged();
                            }

                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;
                            // Handle exceptions if necessary
                        }
                        finally
                        {
                            semaphore.Release();
                        }

                        //}

                    });

                    await Task.WhenAll(downloadTasks);

                    spinShow = true;
                    StateHasChanged();
                    if (string.IsNullOrEmpty(sharedFolderDownloadPath))
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        await _downloadService.CreateZipAndDownload(contact, order, webHost, null, null, null);
                    }

                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        StateHasChanged();
                    }
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));

                    isSubmitting = false;
                    Console.WriteLine(totalDownloadOperationTime);
                    var endTotalDownloadTime = DateTime.Now;
                    var differenctDownloadMethodExecutionTime = endTotalDownloadTime.Subtract(downloadMethodStartTime).TotalSeconds;

                    Console.WriteLine(differenctDownloadMethodExecutionTime);
                    spinShow = false;
                    StateHasChanged();
                    MakeOrderItemUnselected();
                    StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");
                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                    isSubmitting = false;
                    spinShow = false;
                    return;
                }

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownloadQC",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadQCError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }


        #region Threading code for qc download
        private async Task SingleDownloadQC1(bool canStatusChange = true)
        {
            try
            {
                var downloadMethodStartTime = DateTime.Now;
                var totalDownloadOperationTime = 0;

                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");
                ContactModel contact = await _contactManager.GetById(loginUser.ContactId);
                var sharedFolderDownloadPath = "";

                if (contact != null)
                {
                    sharedFolderDownloadPath = contact.DownloadFolderPath;
                }

                sharedFolderDownloadPath = "//192.168.1.216/KD";

                if (!Directory.Exists(sharedFolderDownloadPath))
                {
                    sharedFolderDownloadPath = "";
                }

                spinShow = true;
                isProgressBar = true;

                if (isProgressBar)
                {
                    spinShow = false;
                    progressBarCurrentValue = 0.1;
                }

                isSubmitting = true;
                StateHasChanged();

                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }

                if (selectedFiles != null && selectedFiles.Any())
                {
                    var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}";

                    // Delete previous folder
                    await _ftpFilePathService.ExistsFolderDelete(downloadPath);

                    var selectImages = selectedFiles;
                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    var count = 0;
                    maxValue = selectImages.Count;

                    var clientOrder = await _orderService.GetById((int)order.Id);
                    var isAllowExtraOutputFileUpload = clientOrder.AllowExtraOutputFileUpload;

                    FileUploadModel fileUploadVM = new FileUploadModel
                    {
                        FtpUrl = serverInfo.Host,
                        userName = serverInfo.UserName,
                        password = serverInfo.Password,
                        SubFolder = serverInfo.SubFolder,
                        OrderNumber = order.OrderNumber
                    };

                    // Define a semaphore to limit the number of concurrent downloads
                    const int MaxConcurrentDownloads = 10;
                    var semaphore = new SemaphoreSlim(MaxConcurrentDownloads);

                    // Initialize the FTP client once and reuse it


                    var downloadTasks = selectImages.Select(async file =>
                    {

                        await semaphore.WaitAsync();

                        try
                        {
                            using (var client = _ftpService.CreateAsyncFtpClient(fileUploadVM))
                            {
                                client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                client.Config.ValidateAnyCertificate = true;
                                client.Config.TransferChunkSize = 1048576;
                                //client.Config.

                                await client.AutoConnect();

                                var model = new FileUploadModel
                                {
                                    FtpUrl = serverInfo.Host,
                                    userName = serverInfo.UserName,
                                    password = serverInfo.Password,
                                    SubFolder = serverInfo.SubFolder,
                                    OrderNumber = order.OrderNumber
                                };

                                if (string.IsNullOrWhiteSpace(file.ProductionDoneFilePath))
                                {
                                    count++;
                                    progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                                    StateHasChanged();
                                    //return;
                                    semaphore.Release();
                                }

                                if (file.Status >= (int)InternalOrderItemStatus.ReadyToDeliver)
                                {
                                    model.UploadDirectory = Path.GetDirectoryName(file.InternalFileOutputPath);
                                    int lastIndexOfOrderNumberFileOutputFilePath = file.InternalFileOutputPath.LastIndexOf(order.OrderNumber);
                                    model.DownloadFolderName = "/" + file.InternalFileOutputPath.Substring(lastIndexOfOrderNumberFileOutputFilePath);
                                    int lastIndexOfSlash = model.DownloadFolderName.LastIndexOf("/");
                                    model.DownloadFolderName = model.DownloadFolderName.Substring(0, lastIndexOfSlash);
                                    model.fileName = Path.GetFileName(file.InternalFileOutputPath);
                                }
                                else
                                {
                                    if (isAllowExtraOutputFileUpload)
                                    {
                                        int lastIndexOfOrderNumberInProductionDoneFilePath = file.ProductionDoneFilePath.LastIndexOf(order.OrderNumber);
                                        model.DownloadFolderName = "/" + file.ProductionDoneFilePath.Substring(lastIndexOfOrderNumberInProductionDoneFilePath);
                                        int lastIndexOfSlash = model.DownloadFolderName.LastIndexOf("/");
                                        model.DownloadFolderName = model.DownloadFolderName.Substring(0, lastIndexOfSlash);
                                    }
                                    if (!isAllowExtraOutputFileUpload)
                                    {
                                        model.DownloadFolderName = file.PartialPath;
                                    }
                                    model.UploadDirectory = Path.GetDirectoryName(file.ProductionDoneFilePath);
                                    model.fileName = Path.GetFileName(file.ProductionDoneFilePath);
                                }

                                model.Date = order.CreatedDate;
                                model.ContactName = contactInfo.FirstName.Trim() + " " + contactInfo.Id;

                                var dataSavePath = !string.IsNullOrEmpty(sharedFolderDownloadPath)
                                    ? $"{sharedFolderDownloadPath}\\{model.ContactName} (QC)\\Raw\\{formattedDateTimeForDownload}{model.DownloadFolderName}/{model.fileName}"
                                    : $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{model.ContactName}\\{model.DownloadFolderName}\\{model.fileName}";

                                var dataSourcePath = $"{model.UploadDirectory}/{model.fileName}";
                                if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                {
                                    dataSourcePath = $"{serverInfo.SubFolder}/{dataSourcePath}";
                                }

                                var startTime = DateTime.Now;
                                var downloadResponse = await client.DownloadFile(dataSavePath, dataSourcePath, FtpLocalExists.Overwrite);

                                var timeDifference = DateTime.Now.Subtract(startTime).TotalSeconds;
                                totalDownloadOperationTime += (int)timeDifference;

                                var uploadedPath = !string.IsNullOrEmpty(sharedFolderDownloadPath)
                                    ? $"{sharedFolderDownloadPath}\\{model.ContactName} (QC)\\Completed\\_uploaded\\"
                                    : null;

                                if (uploadedPath != null && !Directory.Exists(uploadedPath))
                                {
                                    Directory.CreateDirectory(uploadedPath);
                                }

                                if (downloadResponse.Equals(FtpStatus.Success))
                                {
                                    var status = file.Status == (byte)InternalOrderItemStatus.ReworkDone
                                        ? InternalOrderItemStatus.ReworkQc
                                        : InternalOrderItemStatus.InQc;

                                    var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);

                                    if (orderItemStatusUpdate)
                                    {
                                        await AddOrderItemStatusChangeLog(file, status);
                                    }

                                    if (uploadedPath != null)
                                    {
                                        await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)file.Id, dataSavePath, loginUser.ContactId);
                                    }
                                }

                                count++;
                                progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                                StateHasChanged();
                                await client.Disconnect();
                            }

                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;
                            // Handle exceptions if necessary
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    await Task.WhenAll(downloadTasks);



                    spinShow = true;
                    StateHasChanged();

                    if (string.IsNullOrEmpty(sharedFolderDownloadPath))
                    {
                        await _downloadService.CreateZipAndDownload(contactInfo, order, $"{this._webHostEnvironment.WebRootPath}", null, null, null);
                    }

                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        StateHasChanged();
                    }

                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));

                    isSubmitting = false;
                    var endTotalDownloadTime = DateTime.Now;
                    var differenctDownloadMethodExecutionTime = endTotalDownloadTime.Subtract(downloadMethodStartTime).TotalSeconds;

                    await js.DisplayMessage("Download Successfully");
                    spinShow = false;
                    MakeOrderItemUnselected();
                    StateHasChanged();

                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                    isSubmitting = false;
                    spinShow = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownloadQC",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadQCError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        #endregion Threading code for qc download
        #endregion
        #region QC 

        private async Task UpdateOrderItemByQc(InternalOrderItemStatus status, int imageId = 0, bool FileMove = false)
        {
            var statusNotMatchingFiles = new List<ClientOrderItemModel>();
            try
            {
                spinShow = true;
                if (selectedFiles != null)
                {
                    if (selectedFiles.Count <= 0)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Select at least One File");
                        return;
                    }
                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                    var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    //foreach (var file in selectedFiles)
                    //{
                    //	//if (file.Status == (byte)InternalOrderItemStatus.ProductionDone && status != InternalOrderItemStatus.InQc)
                    //	if (status == InternalOrderItemStatus.ReadyToDeliver || status == InternalOrderItemStatus.Rejected)
                    //	{
                    //		if (file.Status == (byte)InternalOrderItemStatus.ProductionDone || file.Status == (byte)InternalOrderItemStatus.ReworkDone)
                    //		{
                    //			spinShow = false;
                    //			this.StateHasChanged();
                    //			await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Approve or Reject First Take them in QC");
                    //			return;
                    //		}
                    //	}
                    //}

                    foreach (var file in selectedFiles)
                    {
                        if (file.Status == (byte)InternalOrderItemStatus.ProductionDone || file.Status == (byte)InternalOrderItemStatus.ReworkDone)
                        {
                            if (FileMove)
                            {
                                var fileInfo = await _clientOrderItemService.GetById((int)file.Id);
                                var imageInfo = await _assignService.GetById((int)file.Id);
                                var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);

                                FileUploadModel model = new FileUploadModel();
                                model.fileName = Path.GetFileName(fileInfo.ProductionDoneFilePath);
                                model.FtpUrl = serverInfo.Host;
                                model.userName = serverInfo.UserName;
                                model.password = serverInfo.Password;
                                model.SubFolder = serverInfo.SubFolder;
                                model.OrderNumber = order.OrderNumber;

                                var baseDirectory = "";
                                var productionDoneFilePathAfterInProgress = "";
                                if (!string.IsNullOrWhiteSpace(fileInfo.InternalFileInputPath))
                                {
                                    var dividePathFor = fileInfo.InternalFileInputPath.Split("Raw");
                                    baseDirectory = dividePathFor[0];
                                }
                                else
                                {
                                    var dividePathFor = fileInfo.ProductionDoneFilePath.Split("In Progress");
                                    baseDirectory = dividePathFor[0];

                                }
                                productionDoneFilePathAfterInProgress = fileInfo.ProductionDoneFilePath.Split("/In Progress")[1];
                                productionDoneFilePathAfterInProgress = productionDoneFilePathAfterInProgress.Replace("\\", "/");
                                model.Date = order.CreatedDate;
                                model.ContactName = editorInfo.FirstName + " " + editorInfo.Id;
                                //Source Path
                                model.UploadDirectory = $"{baseDirectory}/In Progress{Path.GetDirectoryName(productionDoneFilePathAfterInProgress)}";
                                //Destination Path
                                var destinationPath = "";
                                if (fileInfo.Status == (byte)InternalOrderItemStatus.ReworkDone)
                                {
                                    destinationPath = productionDoneFilePathAfterInProgress.Split("/Rejected")[1];
                                }
                                else
                                {
                                    destinationPath = productionDoneFilePathAfterInProgress.Split("/Production Done")[1];
                                }

                                destinationPath = destinationPath.Replace("\\", "/");
                                model.ReturnPath = $"{baseDirectory}/Completed{Path.GetDirectoryName(destinationPath)}";
                                await _fluentFtpService.FolderCreateAtApprovedTime(model);
                                destinationPath = destinationPath.Replace("//", "/");
                                var tempPath = Path.GetDirectoryName(destinationPath).Replace("\\", "/");

                                FileUploadModel fileUploadVM = new FileUploadModel()
                                {
                                    FtpUrl = serverInfo.Host,
                                    userName = serverInfo.UserName,
                                    password = serverInfo.Password,
                                    SubFolder = serverInfo.SubFolder,
                                    ReturnPath = $"{baseDirectory}/Completed{tempPath}/{model.fileName}",
                                    UploadDirectory = $"{model.UploadDirectory}/{model.fileName}"
                                };

                                var fileMoveResult = await _fluentFtpService.MoveFile(fileUploadVM);

                                if (fileMoveResult.IsSuccess)
                                {

                                    ClientOrderItemModel clientOrderItem = new ClientOrderItemModel()
                                    {
                                        CompanyId = fileInfo.CompanyId,
                                        FileName = file.FileName,
                                        InternalFileOutputPath = fileUploadVM.ReturnPath,
                                        PartialPath = fileInfo.PartialPath,
                                        ClientOrderId = order.Id,
                                        Id = file.Id,
                                    };

                                    var updateItemInDBResponse = await _clientOrderItemService.UpdateItemByQC(clientOrderItem);

                                    //TODO: Need to update items
                                    if (updateItemInDBResponse.IsSuccess)
                                    {
                                        var orderItemForUpdatePath = clientOrderItems.FirstOrDefault(f => f.Id == file.Id);

                                        if (orderItemForUpdatePath.ProductionDoneFilePath != null)
                                        {
                                            //Update current path and status
                                            clientOrderItems.FirstOrDefault(f => f.Id == file.Id);
                                        }
                                    }
                                    //TODO: or

                                    var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);
                                    // Update Order Item Status
                                    if (orderItemStatusUpdate)
                                    {
                                        await AddOrderItemStatusChangeLog(file, InternalOrderItemStatus.ReworkDistributed);
                                    }
                                    //await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)file.Id, localPath, loginUser.ContactId);
                                }
                            }
                        }
                        else
                        {
                            statusNotMatchingFiles.Add(file);
                            //await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Approve or Reject First Take them in QC");
                            //return;
                        }
                    }
                    // Rakib Vai
                    //await UpdateOrderItem(status);

                    MakeOrderItemUnselected();
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                    await LoadOrderItemForLoginUser();
                    //Order Status Update
                    isShowImagePopup = false;
                }
                else
                {
                    spinShow = false;
                    this.StateHasChanged();
                    await js.DisplayMessage("Select at least one Item !");
                    return;
                }

                if (statusNotMatchingFiles.Count > 0)
                {
                    var text = await CreateTextFileName(statusNotMatchingFiles);
                    await js.DisplayMessage($"This Image File is not valid Status -  {order.OrderNumber}\n{text}");
                    isUploadInputDisabled = false;
                    return;
                }

                spinShow = false;
                this.StateHasChanged();
            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderItemByQc",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.UpdateOrderItemByQcError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task UpdateOrderItemByQcForApproved(InternalOrderItemStatus status, int imageId = 0, bool FileMove = false)
        {
            var statusNotMatchingFiles = new List<ClientOrderItemModel>();
            try
            {
                spinShow = true;
                if (selectedFiles != null)
                {
                    if (selectedFiles.Count <= 0)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Select at least One File");
                        return;
                    }
                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                    var contactInfo = await _contactManager.GetById(loginUser.ContactId);

                    foreach (var file in selectedFiles)
                    {
                        if (file.Status == (byte)InternalOrderItemStatus.InQc || file.Status == (byte)InternalOrderItemStatus.ReworkQc)
                        {
                            if (FileMove)
                            {
                                var fileInfo = await _clientOrderItemService.GetById((int)file.Id);
                                var imageInfo = await _assignService.GetById((int)file.Id);
                                var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);

                                FileUploadModel model = new FileUploadModel();
                                model.fileName = Path.GetFileName(fileInfo.ProductionDoneFilePath);
                                model.FtpUrl = serverInfo.Host;
                                model.userName = serverInfo.UserName;
                                model.password = serverInfo.Password;
                                model.SubFolder = serverInfo.SubFolder;
                                model.OrderNumber = order.OrderNumber;

                                var baseDirectory = "";
                                var productionDoneFilePathAfterInProgress = "";
                                if (!string.IsNullOrWhiteSpace(fileInfo.InternalFileInputPath))
                                {
                                    var dividePathFor = fileInfo.InternalFileInputPath.Split("Raw");
                                    baseDirectory = dividePathFor[0];
                                }
                                else
                                {
                                    var dividePathFor = fileInfo.ProductionDoneFilePath.Split("In Progress");
                                    baseDirectory = dividePathFor[0];

                                }
                                productionDoneFilePathAfterInProgress = fileInfo.ProductionDoneFilePath.Split("/In Progress")[1];
                                productionDoneFilePathAfterInProgress = productionDoneFilePathAfterInProgress.Replace("\\", "/");
                                model.Date = order.CreatedDate;
                                model.ContactName = editorInfo.FirstName + " " + editorInfo.Id;
                                //Source Path
                                model.UploadDirectory = $"{baseDirectory}/In Progress{Path.GetDirectoryName(productionDoneFilePathAfterInProgress)}";
                                //Destination Path
                                var destinationPath = "";
                                if (fileInfo.Status == (byte)InternalOrderItemStatus.ReworkDone)
                                {
                                    destinationPath = productionDoneFilePathAfterInProgress.Split("/Rejected")[1];
                                }
                                else
                                {
                                    destinationPath = productionDoneFilePathAfterInProgress.Split("/Production Done")[1];
                                }

                                destinationPath = destinationPath.Replace("\\", "/");
                                model.ReturnPath = $"{baseDirectory}/Completed{Path.GetDirectoryName(destinationPath)}";
                                await _fluentFtpService.FolderCreateAtApprovedTime(model);
                                destinationPath = destinationPath.Replace("//", "/");
                                var tempPath = Path.GetDirectoryName(destinationPath).Replace("\\", "/");

                                FileUploadModel fileUploadVM = new FileUploadModel()
                                {
                                    FtpUrl = serverInfo.Host,
                                    userName = serverInfo.UserName,
                                    password = serverInfo.Password,
                                    SubFolder = serverInfo.SubFolder,
                                    ReturnPath = $"{baseDirectory}/Completed{tempPath}/{model.fileName}",
                                    UploadDirectory = $"{model.UploadDirectory}/{model.fileName}"
                                };

                                var fileMoveResult = await _fluentFtpService.MoveFile(fileUploadVM);

                                if (fileMoveResult.IsSuccess)
                                {

                                    ClientOrderItemModel clientOrderItem = new ClientOrderItemModel()
                                    {
                                        CompanyId = fileInfo.CompanyId,
                                        FileName = file.FileName,
                                        InternalFileOutputPath = fileUploadVM.ReturnPath,
                                        PartialPath = fileInfo.PartialPath,
                                        ClientOrderId = order.Id,
                                        Id = file.Id,
                                    };

                                    var updateItemInDBResponse = await _clientOrderItemService.UpdateItemByQC(clientOrderItem);

                                    //TODO: Need to update items
                                    if (updateItemInDBResponse.IsSuccess)
                                    {
                                        var orderItemForUpdatePath = clientOrderItems.FirstOrDefault(f => f.Id == file.Id);

                                        if (orderItemForUpdatePath.ProductionDoneFilePath != null)
                                        {
                                            //Update current path and status
                                            clientOrderItems.FirstOrDefault(f => f.Id == file.Id);
                                        }
                                    }

                                    var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);
                                    // Update Order Item Status
                                    if (orderItemStatusUpdate)
                                    {
                                        await AddOrderItemStatusChangeLog(file, status);
                                    }
                                    await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)file.Id, $" FTP Destionation Path : {destinationPath}", loginUser.ContactId);
                                }
                            }
                        }
                        else
                        {
                            statusNotMatchingFiles.Add(file);

                        }
                    }

                    MakeOrderItemUnselected();
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                    await LoadOrderItemForLoginUser();
                    //Order Status Update
                    isShowImagePopup = false;
                }
                else
                {
                    spinShow = false;
                    this.StateHasChanged();
                    await js.DisplayMessage("Select at least one Item !");
                    return;
                }

                if (statusNotMatchingFiles.Count > 0)
                {
                    var text = await CreateTextFileName(statusNotMatchingFiles);
                    await js.DisplayMessage($"This Image File is not valid Status -  {order.OrderNumber}\n{text}");
                    isUploadInputDisabled = false;
                    spinShow = false;
                    this.StateHasChanged();
                    return;
                }

                spinShow = false;
                this.StateHasChanged();
            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderItemByQc",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.UpdateOrderItemByQcError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            spinShow = false;
            this.StateHasChanged();
        }

        private async Task UpdateOrderItemByQcForAFQ(InternalOrderItemStatus status)
        {
            var statusNotMatchingFiles = new List<ClientOrderItemModel>();
            try
            {
                spinShow = true;
                if (selectedFiles != null)
                {
                    if (selectedFiles.Count <= 0)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Select at least One File");
                        return;
                    }
                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                    var contactInfo = await _contactManager.GetById(loginUser.ContactId);

                    foreach (var file in selectedFiles)
                    {
                        if (file.Status == (byte)InternalOrderItemStatus.ProductionDone || file.Status == (byte)InternalOrderItemStatus.ReworkDone)
                        {
                            var orderItemStatusUpdate = false;

                            orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);

                            // Update Order Item Status
                            if (orderItemStatusUpdate)
                            {
                                await AddOrderItemStatusChangeLog(file, status);
                            }
                        }
                        else
                        {
                            statusNotMatchingFiles.Add(file);
                            //await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Approve or Reject First Take them in QC");
                            //return;
                        }
                    }
                    // Rakib Vai
                    //await UpdateOrderItem(status);

                    MakeOrderItemUnselected();
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                    await LoadOrderItemForLoginUser();
                    //Order Status Update
                    isShowImagePopup = false;
                }
                else
                {
                    spinShow = false;
                    this.StateHasChanged();
                    await js.DisplayMessage("Select at least one Item !");
                    return;
                }

                if (statusNotMatchingFiles.Count > 0)
                {
                    var text = await CreateTextFileName(statusNotMatchingFiles);
                    await js.DisplayMessage($"This Image File is not valid Status -  {order.OrderNumber}\n{text}");
                    isUploadInputDisabled = false;
                    spinShow = false;
                    this.StateHasChanged();
                    return;
                }

                spinShow = false;
                this.StateHasChanged();
            }

            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderItemByQc",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.UpdateOrderItemByQcError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task Reject(InternalOrderItemStatus status = InternalOrderItemStatus.InQc, int imageId = 0)
        {
            var statusNotMatchingFiles = new List<ClientOrderItemModel>();
            try
            {
                spinShow = true;
                if (selectedFiles != null)
                {
                    if (selectedFiles.Count <= 0)
                    {
                        spinShow = false;
                        this.StateHasChanged();
                        await js.DisplayMessage("Select at least One File");
                        return;
                    }
                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                    var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    foreach (var file in selectedFiles)
                    {
                        if (file.Status == (byte)InternalOrderItemStatus.InQc || file.Status == (byte)InternalOrderItemStatus.ReworkQc)
                        {
                            var fileInfo = await _clientOrderItemService.GetById((int)file.Id);
                            var imageInfo = await _assignService.GetById((int)file.Id);
                            var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);
                            FileUploadModel model = new FileUploadModel();
                            model.fileName = Path.GetFileName(fileInfo.ProductionDoneFilePath);
                            model.FtpUrl = serverInfo.Host;
                            model.userName = serverInfo.UserName;
                            model.password = serverInfo.Password;
                            model.OrderNumber = order.OrderNumber;
                            var path = $"{fileInfo.InternalFileInputPath}";
                            var dividePath = path.Split("/");
                            model.RootDirectory = dividePath[0];
                            model.Date = order.CreatedDate;
                            model.ContactName = editorInfo.FirstName + " " + editorInfo.Id;
                            var baseDirectory = "";
                            var productionDoneFilePathAfterInProgress = "";

                            var dividePathFor = fileInfo.ProductionDoneFilePath.Split("In Progress");
                            baseDirectory = dividePathFor[0];

                            productionDoneFilePathAfterInProgress = fileInfo.ProductionDoneFilePath.Split("/In Progress")[1];
                            productionDoneFilePathAfterInProgress = productionDoneFilePathAfterInProgress.Replace("\\", "/");
                            //Source Path
                            model.UploadDirectory = $"{baseDirectory}/In Progress{Path.GetDirectoryName(productionDoneFilePathAfterInProgress)}";
                            //Destination Path
                            var destinationPath = "";

                            destinationPath = fileInfo.ProductionDoneFilePath.Replace("/Production Done", "/Rejected");

                            model.ReturnPath = Path.GetDirectoryName(destinationPath);

                            await _fluentFtpService.FolderCreateAtApprovedTime(model);
                            FileUploadModel modell = new FileUploadModel()
                            {
                                FtpUrl = serverInfo.Host,
                                userName = serverInfo.UserName,
                                password = serverInfo.Password,
                                SubFolder = serverInfo.SubFolder,
                                ReturnPath = $"{model.ReturnPath}/{model.fileName}",
                                UploadDirectory = $"{model.UploadDirectory}/{model.fileName}"
                            };

                            var fileMoveResult = await _fluentFtpService.MoveFile(modell);

                            if (fileMoveResult.IsSuccess)
                            {
                                ClientOrderItemModel clientOrderItem = new ClientOrderItemModel()
                                {
                                    CompanyId = fileInfo.CompanyId,
                                    FileName = file.FileName,
                                    ProductionDoneFilePath = destinationPath,
                                    PartialPath = fileInfo.PartialPath,
                                    ClientOrderId = order.Id,
                                };

                                var updateItemInDBResponse = await _clientOrderItemService.UpdateItemByQCForReject(clientOrderItem);

                                //TODO: Need to update items
                                if (updateItemInDBResponse.IsSuccess)
                                {
                                    var orderItemForUpdatePath = clientOrderItems.FirstOrDefault(f => f.Id == file.Id);

                                    if (orderItemForUpdatePath.ProductionDoneFilePath != null)
                                    {
                                        //Update currnt path and status
                                        clientOrderItems.FirstOrDefault(f => f.Id == file.Id);
                                    }

                                    var orderItemStatusUpdate = await _clientOrderItemService.UpdateOrderItemStatus(file, status);
                                    // Update Order Item Status
                                    if (orderItemStatusUpdate)
                                    {
                                        await AddOrderItemStatusChangeLog(file, status);
                                    }
                                    await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)file.Id, $" FTP Destionation Path : {destinationPath}", loginUser.ContactId);
                                }
                            }
                        }
                        else
                        {
                            statusNotMatchingFiles.Add(file);
                        }
                    }
                    // Rakib Vai
                    //await UpdateOrderItem(status);
                    MakeOrderItemUnselected();

                    //Order Status Update
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                    await LoadOrderItemForLoginUser();
                    //Order Status Update
                    isShowImagePopup = false;
                    if (statusNotMatchingFiles.Count > 0)
                    {
                        var text = await CreateTextFileName(statusNotMatchingFiles);
                        await js.DisplayMessage($"Dear User One of your selected File is not in Qc . To Reject First Take them in QC -  {order.OrderNumber}\n{text}");
                        isUploadInputDisabled = false;
                        spinShow = false;
                        this.StateHasChanged();
                        return;
                    }
                }
                else
                {
                    spinShow = false;
                    this.StateHasChanged();
                    await js.DisplayMessage("Select at least one file or item !");
                }

                spinShow = false;
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "Reject",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.RejectError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        #endregion
        #region Replace
        private async Task Replace(int orderItemId)
        {
            try
            {
                var selectImages = await _clientOrderItemService.GetItemById(orderItemId);
                var selectFiles = selectImages.FirstOrDefault();
                var orderInfo = await _orderService.GetById((int)selectFiles.ClientOrderId);
                var serverInfo = await _fileServerService.GetById((int)orderInfo.FileServerId);
                var contactInfo = await _contactManager.GetById(loginUser.ContactId);

                ClientOrderItemModel fileinfo = new ClientOrderItemModel();
                if (selectImages != null)
                {
                    foreach (var file in selectedFiles)
                    {
                        if (file.Status <= (byte)InternalOrderItemStatus.ProductionDone)
                        {
                            await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Approve or Reject First Take them in QC");
                            return;
                        }
                        var fileInfo = await _clientOrderItemService.GetById((int)file.Id);
                        var imageInfo = await _assignService.GetById((int)file.Id);
                        var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);
                        fileinfo = fileInfo;
                        FileUploadModel model = new FileUploadModel();
                        model.fileName = fileInfo.FileName;
                        model.FtpUrl = serverInfo.Host;
                        model.userName = serverInfo.UserName;
                        model.password = serverInfo.Password;
                        model.SubFolder = serverInfo.SubFolder;
                        model.OrderNumber = order.OrderNumber;
                        var path = $"{fileInfo.InternalFileInputPath}";
                        var dividePath = path.Split("/");
                        model.RootDirectory = dividePath[0];
                        var dividePathFor = path.Split("Raw");
                        model.DownloadFolderName = dividePathFor[1];
                        var baseDirectory = dividePathFor[0];
                        model.Date = orderInfo.CreatedDate;
                        model.ContactName = editorInfo.FirstName + " " + editorInfo.Id;
                        model.UploadDirectory = $"{baseDirectory}/In Progress/{model.OrderNumber}/{model.ContactName}/Production Done";

                        await _ftpService.MoveFile(model);
                    }
                    // Rakib Vai
                    InternalOrderItemStatus status = InternalOrderItemStatus.ReadyToDeliver;
                    await UpdateOrderItem(status);
                    selectImages = new List<ClientOrderItemModel>();
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

                    //Order Status Update
                    await LoadOrderItemForLoginUser();
                }
                else
                {
                    await js.DisplayMessage("You are not select any image!");
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "Replace",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.ReplaceError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }


        }
        private void ShowFolderUploadControlForOrderItems()
        {
            allowFolderUploadForOrder = true;
            StateHasChanged();
        }
        private void ShowFileUploadControlForOrderItems()
        {
            allowFolderUploadForOrder = false;
            StateHasChanged();
        }
        private async void LoadMultipleFielsForReplaceByQC(InputFileChangeEventArgs args)
        {
            await Task.Yield();
            loadReplaceFile = args.GetMultipleFiles(maximumFileCount: 100000000);
            maxValue = args.GetMultipleFiles(maximumFileCount: 100000000).Count();
        }
        [JSInvokable]
        public static Task GetReplaceQCSelectedFileDetails(List<FileForUploadDetails> files)
        {
            _ReplaceQCselectedFileFromJs = files;
            return Task.CompletedTask;
        }
        //Todo:Rakib
        [JSInvokable]
        public static Task GetOrderNewItems(List<FileForUploadDetails> files)
        {
            _orderNewItemsUploadFileFromJs = files;
            return Task.CompletedTask;
        }
        protected override void OnAfterRender(bool firstRender)
        {
            js.InvokeVoidAsync("replaceattachQCFileUploadHandler");
            js.InvokeVoidAsync("addOrderNewItemsHandler");
            js.InvokeVoidAsync("OrderSOPUploadUpdatedFile");


        }

        private async Task UploadReplaceFiles()
        {
            try
            {
                if (clientOrderItemForReplace.Status == (byte)InternalOrderStatus.ReadyToDeliver)
                {
                    if (loadReplaceFile == null)
                    {
                        await js.DisplayMessage("Please Select A Image Which You want to Replace !");
                        return;
                    }
                    if (loadReplaceFile.Count == 1)
                    {
                        var fileServerViewModel = new FileServerViewModel()
                        {
                            Host = fileServer.Host,
                            UserName = fileServer.UserName,
                            Password = fileServer.Password,
                        };
                        using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                        {
                            foreach (var file in loadReplaceFile)
                            {
                                if (file.Name.Trim() != clientOrderItemForReplace.FileName.Trim())
                                {
                                    await js.DisplayMessage("Replace Failed ! Please Select Same Name image Which you want to Replace");
                                    return;
                                }
                                spinShow = true;
                                StateHasChanged();
                                await ftp.AutoConnect();
                                await ftp.DeleteFile(clientOrderItemForReplace.InternalFileOutputPath);
                                await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: file.Size * 1024), clientOrderItemForReplace.InternalFileOutputPath, FtpRemoteExists.Overwrite, true);
                                await ftp.AutoConnect();
                                spinShow = false;
                                isReplaceInQcFilesPopupVisible = false;
                                StateHasChanged();
                                await js.DisplayMessage("Replace Successfully");
                            }



                        }
                    }
                    else
                    {
                        await js.DisplayMessage("You Can Replace One Image At a Time");
                        return;
                    }
                }
                else
                {
                    await js.DisplayMessage("Only Ready To Deliver Image can be replaced");
                    return;
                }
            }


            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UploadReplaceFiles",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.UploadReplaceFilesError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task UpdateOrderItemStatus(ClientOrderModel order, ClientOrderItemModel clientOrderItem)
        {
            try
            {
                var orderItem = await _orderFileService.GetByIdAndFileName((int)order.Id, Path.GetFileNameWithoutExtension(clientOrderItem.FileName), clientOrderItem.PartialPath);
                if (orderItem != null)
                {
                    if (orderItem.Status >= (int)InternalOrderItemStatus.ReadyToDeliver)
                    {
                        return;
                    }
                    orderItem.Status = (byte)InternalOrderItemStatus.ReadyToDeliver;
                    await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);

                    var orderStatus = await GetInternalOrderStatus((int)order.Id);
                    await UpdateOrder(order, orderStatus); //ToDo:RakibStatus

                    //Order Status Update

                    await AddOrderStatusChangeLog(order, orderStatus); // Order Status Change log
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderItemStatus",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.UpdateOrderItemStatusError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

        private async Task<InternalOrderStatus> GetInternalOrderStatus(int orderId)
        {
            InternalOrderStatus internalOrderItemStatus = 0; //Todo:Rakib
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
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetInternalOrderStatus",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.GetInternalOrderStatusError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");

            }

            return internalOrderItemStatus;

        }

        private async Task UpdateOrderItemStatusFromImagePreviewPopUp(ClientOrderModel order, ClientOrderItemModel clientOrderItem, InternalOrderItemStatus status)
        {
            try
            {
                var orderItem = await _orderFileService.GetByIdAndFileName((int)order.Id, Path.GetFileNameWithoutExtension(clientOrderItem.FileName), clientOrderItem.PartialPath);
                if (orderItem != null)
                {
                    if (orderItem.Status == (int)InternalOrderItemStatus.InQc || orderItem.Status == (int)InternalOrderItemStatus.ReworkQc)
                    {
                        orderItem.Status = (byte)status;
                        await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);

                        var orderStatus = await GetInternalOrderStatus((int)order.Id);
                        await UpdateOrder(order, orderStatus); //ToDo:RakibStatus

                        //Order Status Update

                        await AddOrderStatusChangeLog(order, orderStatus); // Order Status Change log
                    }
                    else
                    {
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderItemStatusFromImagePreviewPopUp",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.UpdateOrderItemStatusFromImagePreviewPopUpError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

        #endregion
        #region Private Method 1
        private async Task PreviousNextFilePreview(string type, int currentFileId)
        {
            //ClientOrderItem selectedFile = null;
            //if (type = "next")
            //{
            //var selectedFile = clientOrderItems.where(f=>f.Status == itemStatus.InQc && f.Id > currentFileId)
            // .OrderBY(f=>f.Date.Take(1).FirstOfDefault()
            //}
        }

        private void CanUploadProductionDoneImages()
        {
            if (clientOrderItems.All(orderItem => orderItem.Status >= (byte)InternalOrderItemStatus.InProduction))
            {
                isAbleToUpload = true;
            }
        }

        public async Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus)
        {
            var previousLog = await _orderStatusChangeLogService.OrderStatusLastChangeLogByOrderId((int)clientOrder.Id);

            if (previousLog != null)
            {
                if (previousLog.NewInternalStatus != (byte)internalOrderStatus)
                {
                    OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
                    {
                        OrderId = (int)clientOrder.Id,
                        NewInternalStatus = (byte)internalOrderStatus,
                        NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
                        ChangeByContactId = loginUser.ContactId,
                        ChangeDate = DateTime.Now
                    };

                    if (previousLog != null)
                    {
                        orderStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                        orderStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                        orderStatusChangeLog.TimeDurationInMinutes = (int)(orderStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
                    }
                    await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
                }
            }



        }
        // This method Bug, Rakib Vai Check this method- Case : When order placing , I am going order detail and add item then find bug.
        public async Task AddOrderItemStatusChangeLog(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus)
        {
            var previousLog = await _orderItemStatusChangeLogService.OrderItemStatusLastChangeLogByOrderFileId((int)clientOrderItem.Id);

            if (previousLog != null)
            {
                if (previousLog != null || previousLog.NewInternalStatus != (byte)internalOrderItemStatus)
                {
                    OrderItemStatusChangeLogModel orderItemStatusChangeLog = new OrderItemStatusChangeLogModel
                    {
                        OrderFileId = (int)clientOrderItem.Id,
                        NewInternalStatus = (byte)internalOrderItemStatus,
                        NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
                        ChangeByContactId = loginUser.ContactId,
                        ChangeDate = DateTime.Now
                    };


                    if (previousLog != null)
                    {
                        orderItemStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                        orderItemStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                        orderItemStatusChangeLog.TimeDurationInMinutes = (int)(orderItemStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
                    }
                    await _orderItemStatusChangeLogService.Insert(orderItemStatusChangeLog);
                }
            }
        }

        public async Task DownloadOrderFileAttachment(int Id, int OrderId)
        {
            isSubmitting = true;
            spinShow = true;
            var contactInfo = await _contactManager.GetById(loginUser.ContactId);
            var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + contactInfo.Id}";
            if (Directory.Exists(downloadPath))
            {
                DirectoryInfo directory = new DirectoryInfo(downloadPath);
                directory.Delete(true);
            }

            if (OrderId > 0)
            {
                var result = false;
                var dlpath = "";
                var selectedFile = await _orderFileAttachmentService.GetOrdersAttachementByOrderId(OrderId);
                var orderInfo = await _orderService.GetById((int)OrderId);
                var serverInfo = await _fileServerService.GetById((int)orderInfo.FileServerId);
                FileUploadModel model = new FileUploadModel();
                model.ContactName = contactInfo.FirstName + " " + contactInfo.Id;
                dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload" + $"\\{model.ContactName}\\{model.OrderNumber}";
                OrderFileAttachment fileinfo = new OrderFileAttachment();
                var count = 0;
                // maxValue = selectImages.Count;
                if (selectedFile.Count() > 0)
                {
                    foreach (var file in selectedFile)
                    {
                        model.fileName = file.FileName;
                        model.FtpUrl = serverInfo.Host;
                        model.userName = serverInfo.UserName;
                        model.password = serverInfo.Password;
                        model.SubFolder = serverInfo.SubFolder;
                        model.OrderNumber = order.OrderNumber;
                        model.UploadDirectory = $"{file.PartialPath}";
                        var path = $"{file.PartialPath}";
                        var dividePath = path.Split("/");
                        model.RootDirectory = dividePath[0];
                        var dividePathFor = path.Split("Raw");
                        model.DownloadFolderName = dividePathFor[1];
                        model.Date = orderInfo.CreatedDate;
                        dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload" + $"\\{model.ContactName}\\{model.OrderNumber}";
                        result = await _ftpService.SingleDownload(js, model, dlpath);
                        await _ftpService.CreateFolderDownloadTime(model);
                    }

                }
                // Attachment
                else
                {
                    Directory.CreateDirectory(downloadPath);
                }
                if (!string.IsNullOrEmpty(orderInfo.Instructions))
                {
                    var attachSavePath = "";

                    if (!string.IsNullOrEmpty(model.DownloadFolderName))
                    {
                        attachSavePath = dlpath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
                    }
                    else
                    {
                        attachSavePath = dlpath + $"\\{model.ContactName}\\{orderInfo.OrderNumber}\\Order Attachment\\";
                        Directory.CreateDirectory(attachSavePath);
                    }
                    if (Directory.Exists(attachSavePath))
                    {
                        // Create Text File 
                        var fullText = $" Order Instructions : \n {orderInfo.Instructions}";
                        var datapath = attachSavePath + "attachment.txt";
                        if (!File.Exists(datapath))
                        {
                            await File.WriteAllTextAsync(datapath, fullText);
                            result = true;
                        }
                    }
                }

                if (orderSOPTemplates != null)
                {
                    foreach (var OrderTemplate in orderSOPTemplates)
                    {
                        var SopTemplate = await _orderTemplateSOPService.GetById((int)OrderTemplate.OrderSOP_Template_Id);
                        await DownloadOrderSOPAllAttachement(SopTemplate);
                        result = true;
                    }
                }
                if (result)
                {
                    var webHost = $"{this._webHostEnvironment.WebRootPath}";
                    //var model = await DownloadOrderItemZipFile(contactInfo, order, fileinfo);
                    await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, "Order All Attachment", null, null);
                    await js.DisplayMessage("Download Succesfully");

                    //UriHelper.NavigateTo("/order/Details" + "/" + orderInfo.ObjectId, true);
                    this.StateHasChanged();
                }
                else
                {
                    await js.DisplayMessage("Download Failed");
                }
            }
            else
            {
                bool result = false;
                var dlpath = "";
                var selectedFile = await _orderFileAttachmentService.GetOrdersAttachementById(Id);
                var FileAttach = selectedFile.FirstOrDefault();
                var orderInfo = await _orderService.GetById((int)FileAttach.Order_ClientOrder_Id);
                var serverInfo = await _fileServerService.GetById((int)orderInfo.FileServerId);

                OrderFileAttachment fileinfo = new OrderFileAttachment();
                FileUploadModel model = new FileUploadModel();
                foreach (var file in selectedFile)
                {
                    fileinfo = file;
                    model.fileName = fileinfo.FileName;
                    model.FtpUrl = serverInfo.Host;
                    model.userName = serverInfo.UserName;
                    model.password = serverInfo.Password;
                    model.SubFolder = serverInfo.SubFolder;
                    model.OrderNumber = order.OrderNumber;
                    model.UploadDirectory = $"{fileinfo.PartialPath}";
                    var path = $"{fileinfo.PartialPath}";
                    var dividePath = path.Split("/");
                    model.RootDirectory = dividePath[0];
                    var dividePathFor = path.Split("Raw");
                    model.DownloadFolderName = dividePathFor[1];
                    model.Date = orderInfo.CreatedDate;
                    //dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                    model.ContactName = contactInfo.FirstName + " " + contactInfo.Id;
                    dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload" + $"\\{model.ContactName}\\";
                    result = await _ftpService.SingleDownload(js, model, dlpath);
                    await _ftpService.CreateFolderDownloadTime(model);
                    this.StateHasChanged();
                }
                if (result)
                {
                    var webHost = $"{this._webHostEnvironment.WebRootPath}";
                    //var model = await DownloadOrderItemZipFile(contactInfo, order, fileinfo);
                    await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, "Order Attachment", null, null);
                    await js.DisplayMessage("Download Succesfully");

                    //UriHelper.NavigateTo("/order/Details" + "/" + orderInfo.ObjectId, true);
                    this.StateHasChanged();
                }
                else
                {
                    await js.DisplayMessage("Download Failed");
                }
                isSubmitting = false;

            }
            spinShow = false;
            this.StateHasChanged();
        }
        public async Task DownloadSOPAllAttachement(SOPTemplateModel sOPTemplate)
        {
            spinShow = true;
            isSubmitting = true;
            var contactInfo = await _contactManager.GetById(loginUser.ContactId);
            var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + contactInfo.Id}";
            if (!Directory.Exists(downloadPath))
            {
                DirectoryInfo directory = new DirectoryInfo(downloadPath);
                directory.Delete(true);
            }
            if (sOPTemplate.Id > 0)
            {
                var result = false;
                isProgressBar = true;
                var selectedFile = await _sopTemplateService.GetSopTemplateFilesBySopTemplateId(sOPTemplate.Id);
                var sopTemplateInfo = await _sopTemplateService.GetById((int)sOPTemplate.Id);
                SOPTemplateFile fileinfo = new SOPTemplateFile();
                // maxValue = selectImages.Count;
                var destFile = "";
                FileUploadModel modell = new FileUploadModel();
                modell.ContactName = contactInfo.FirstName + contactInfo.Id;

                destFile = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{modell.ContactName}\\{order.OrderNumber}\\SOP\\{sopTemplateInfo.Name}\\";

                foreach (var file in selectedFile)
                {
                    fileinfo = await _sopTemplateService.GetSopTemplateFilesById(file.Id);
                    //FileUploadVM modell = new FileUploadVM();
                    var sourcePath = $"{this._webHostEnvironment.WebRootPath}\\Upload\\{fileinfo.ViewPath}\\{fileinfo.FileName}";
                    //modell.ContactName = contactInfo.FirstName + contactInfo.Id;
                    //destFile = $"{this._webHostEnvironment.WebRootPath}\\{modell.ContactName}\\{sopTemplateInfo.Name}\\";

                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }
                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, $"{destFile}\\{fileinfo.FileName}", true);
                    }
                    result = true;
                    this.StateHasChanged();
                }
                var text = await CreateTextSOPInstruction(sopTemplateInfo);

                if (!string.IsNullOrEmpty(text))
                {
                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }

                    // Directory Check
                    if (Directory.Exists(destFile))
                    {
                        // Create Text File 
                        var datapath = destFile + "instruction.txt";
                        if (!File.Exists(datapath))
                        {
                            await File.WriteAllTextAsync(datapath, text);
                            result = true;
                        }
                    }

                }
            }
        }
        public async Task DownloadOrderSOPAllAttachement(OrderSOPTemplateModel sOPTemplate)
        {
            spinShow = true;
            isSubmitting = true;
            if (sOPTemplate != null)
            {
                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + contactInfo.Id}";
                if (!Directory.Exists(downloadPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(downloadPath);
                    directory.Delete(true);
                }
                if (sOPTemplate.Id > 0)
                {
                    var selectedFile = await _orderTemplateSOPFileService.GetOrderSopTemplateFilesByOrderSopTemplateId(sOPTemplate.Id);
                    FileUploadModel fileUploadVM = new FileUploadModel();
                    fileUploadVM.ContactName = contactInfo.FirstName + contactInfo.Id;

                    var destFile = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{fileUploadVM.ContactName}\\{order.OrderNumber}\\SOP\\{sOPTemplate.Name}\\";

                    foreach (var file in selectedFile)
                    {
                        var fileinfo = await _orderTemplateSOPFileService.GetById(file.Id);

                        var sourcePath = $"{fileinfo.ActualPath}".Replace("//", "/");

                        if (!Directory.Exists(destFile))
                        {
                            Directory.CreateDirectory(destFile);
                        }
                        if (File.Exists(sourcePath))
                        {
                            File.Copy(sourcePath, $"{destFile}\\{fileinfo.FileName}", true);
                        }
                        this.StateHasChanged();
                    }
                    var text = await CreateTextOrderSOPInstruction(sOPTemplate);

                    if (!string.IsNullOrEmpty(text))
                    {
                        if (!Directory.Exists(destFile))
                        {
                            Directory.CreateDirectory(destFile);
                        }
                        // Directory Check
                        if (Directory.Exists(destFile))
                        {
                            // Create Text File 
                            var datapath = destFile + "instruction.txt";
                            if (!File.Exists(datapath))
                            {
                                await File.WriteAllTextAsync(datapath, text);
                            }
                        }

                    }
                }
            }
        }
        public async Task DownloadSOPFileAttachment(SOPTemplateModel sOPTemplate)
        {
            spinShow = true;
            isSubmitting = true;
            var contactInfo = await _contactManager.GetById(loginUser.ContactId);
            var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + contactInfo.Id}";

            if (Directory.Exists(downloadPath))
            {
                DirectoryInfo directory = new DirectoryInfo(downloadPath);
                directory.Delete(true);
            }

            if (sOPTemplate.Id > 0)
            {
                var result = false;
                isProgressBar = true;
                var selectedFile = await _sopTemplateService.GetSopTemplateFilesBySopTemplateId(sOPTemplate.Id);
                var sopTemplateInfo = await _sopTemplateService.GetById((int)sOPTemplate.Id);
                SOPTemplateFile fileinfo = new SOPTemplateFile();
                var count = 0;
                var maxValue = selectedFile.Count();
                // maxValue = selectImages.Count;
                var destFile = "";
                FileUploadModel modell = new FileUploadModel();
                modell.ContactName = contactInfo.FirstName + contactInfo.Id;

                destFile = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{modell.ContactName}\\{sopTemplateInfo.Name}\\";

                foreach (var file in selectedFile)
                {
                    fileinfo = await _sopTemplateService.GetSopTemplateFilesById(file.Id);
                    //FileUploadVM modell = new FileUploadVM();
                    var sourcePath = $"{this._webHostEnvironment.WebRootPath}\\Upload\\{fileinfo.ViewPath}\\{fileinfo.FileName}";
                    //modell.ContactName = contactInfo.FirstName + contactInfo.Id;
                    //destFile = $"{this._webHostEnvironment.WebRootPath}\\{modell.ContactName}\\{sopTemplateInfo.Name}\\";

                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }
                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, $"{destFile}\\{fileinfo.FileName}", true);
                    }
                    this.StateHasChanged();
                }
                var text = await CreateTextSOPInstruction(sopTemplateInfo);

                if (!string.IsNullOrEmpty(text))
                {
                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }

                    // Directory Check
                    if (Directory.Exists(destFile))
                    {
                        // Create Text File 
                        var datapath = destFile + "instruction.txt";
                        if (!File.Exists(datapath))
                        {
                            await File.WriteAllTextAsync(datapath, text);
                            result = true;
                        }
                    }
                }
                if (result)
                {
                    var webHost = $"{this._webHostEnvironment.WebRootPath}";
                    //var model = await DownloadOrderItemZipFile(contactInfo, order, fileinfo);
                    await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, sopTemplateInfo, null);
                    await js.DisplayMessage("Download Succesfully");

                    //UriHelper.NavigateTo("/order/Details" + "/" + orderInfo.ObjectId, true);
                    this.StateHasChanged();
                    spinShow = false;
                }
                else
                {
                    await js.DisplayMessage("Download Failed");
                }

            }
            isSubmitting = false;
        }
        public async Task DeleteOrderAttachmentFile(int Id)
        {
            await _orderFileAttachmentService.Delete(Id);
            orderAttachementFiles = await _orderFileAttachmentService.GetOrdersAttachementByOrderId((int)order.Id);
        }
        async Task OnValueChange(string fieldName, string value)
        {
            order.Instructions = value;
            await _orderService.UpdateSingleField((int)order.Id, fieldName, value, loginUser.ContactId);
            isInstructionEditable = false;
        }
        private void ShowAttachmentEditPopUp()
        {
            isOrderAttachmentEditPopupVisible = true;
            //newOrderAttachments = new List<OrderFileAttachment>();
        }
        private void CloseAttachmentEditPopUp()
        {
            isOrderAttachmentEditPopupVisible = false;
            //newOrderAttachments = new List<OrderFileAttachment>();
        }

        private async Task AddNewOrderAttachment(OrderFileAttachment file)
        {
            List<OrderFileAttachment> orderAttachments = new List<OrderFileAttachment>();

            var orderFileAttachment = new OrderFileAttachment
            {
                Order_ClientOrder_Id = order.Id,
                CompanyId = loginUser.CompanyId,
                FileName = file.FileName,
                PartialPath = file.PartialPath,
                Status = StatusConstants.Active,
                IsDeleted = false,
                CreatedByContactId = loginUser.ContactId,
                CreateDated = System.DateTime.Now,
                ObjectId = file.ObjectId,
                FileSize = file.FileSize
            };

            orderAttachments.Add(orderFileAttachment);

            if (!isOrderAttachmentEditPopupVisible)
            {
                return;
            }

            var addResponse = await _orderFileAttachmentService.Insert(orderAttachments, (int)order.Id);
            orderFileAttachment.Id = addResponse.Result;
            orderAttachmentListForView.Add(orderFileAttachment);

            orderAttachementFiles = await _orderFileAttachmentService.GetOrdersAttachementByOrderId((int)order.Id);
        }

        private void CloseItemAddPopUp()
        {
            isOrderItemAddPopupVisible = false;

        }
        private async Task LoadOrderFile(InputFileChangeEventArgs e)
        {
            //loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
            //maxValue = e.GetMultipleFiles(maximumFileCount: 100000000).Count();

            await Task.Yield();
            var loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
            //maxValue = e.GetMultipleFiles(maximumFileCount: 100000000).Count();


            for (int i = 0; i < loadedFiless.Count; i++)
            {
                var filePath = _orderNewItemsUploadFileFromJs[i].Path;
                var file = loadedFiless[i];
                try
                {
                    var orderFileItem = new ClientOrderItemModel();
                    var name = file.Name.ToString();
                    var extensionName = Path.GetExtension(file.Name);
                    var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg", ".avif", ".tif" };
                    var format = file.ContentType;

                    if (imageFileTypes.Contains(extensionName))
                    {
                        var resizeImageFile = await file.RequestImageFileAsync(file.ContentType, 100, 100);
                        var buffer = new byte[resizeImageFile.Size];
                        await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                        var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                        //fileInfo.URL.Add(imageUrl);
                        orderFileItem.FileByteString = imageUrl;
                    }
                    orderFileItem.FileName = name;
                    orderFileItem.File = file;
                    //
                    orderFileItem.ObjectId = Guid.NewGuid().ToString();
                    orderFileItem.CreatedByContactId = loginUser.ContactId;
                    orderFileItem.FileName = file.Name;
                    orderFileItem.FileType = file.ContentType;
                    orderFileItem.FileSize = (byte)file.Size;
                    orderFileItem.PartialPath = filePath;

                    orderItems.Add(orderFileItem);
                }
                catch { }
            }

        }

        private async void LoadOrderFiles(InputFileChangeEventArgs args)
        {
            await Task.Yield();
            foreach (var file in args.GetMultipleFiles(maximumFileCount: 3000))
            {
                try
                {
                    var orderItemFile = new ClientOrderItemModel();
                    var name = file.Name.ToString();
                    var extensionName = Path.GetExtension(file.Name);
                    var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg", ".avif", ".tif" };
                    var format = file.ContentType;

                    if (imageFileTypes.Contains(extensionName))
                    {
                        var resizeImageFile = await file.RequestImageFileAsync(file.ContentType, 100, 100);
                        var buffer = new byte[resizeImageFile.Size];
                        await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                        var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                        //fileInfo.URL.Add(imageUrl);
                        orderItemFile.FileByteString = imageUrl;
                    }

                    orderItemFile.FileName = name;
                    //SopAttachment.Add(file);
                    orderItemFile.File = file;

                    //
                    orderItemFile.ObjectId = Guid.NewGuid().ToString();
                    orderItemFile.CreatedByContactId = loginUser.ContactId;
                    orderItemFile.FileName = file.Name;
                    orderItemFile.FileType = file.ContentType;
                    orderItemFile.FileSize = (byte)file.Size;

                    orderItems.Add(orderItemFile);
                }
                catch { }
            }
            //this.StateHasChanged();

        }

        private async void AddOrderPlaced()
        {
            if (!string.IsNullOrEmpty(order.Instructions) || order.orderAttachments.Count() > 0 || orderSOPTemplates.Count() > 0)
            {
                await UpdateOrder(order, InternalOrderStatus.OrderPlaced);
                await js.DisplayMessage("Order Placed");
            }
            else
            {
                await js.DisplayMessage("Add Instruction Or Attachment for this Order");
            }
        }

        private async void DownloadSopTemplate(SOPTemplateModel template)
        {
            string fileName = template.Name + ".pdf";
            template.SopTemplateFileList = await _templateService.GetSopTemplateFilesBySopTemplateId(template.Id);
            template.SopStandardServiceList = await _standardServiceService.GetListByTemplateId(template.Id);
            SopPdfGeneratorModel pdfGeneratorModel = new SopPdfGeneratorModel
            {
                Name = template.Name,
                HeaderForInstruction = "Instructions",
                Instruction = template.Instruction,
                HeaderForService = "Service",
                SopStandardServiceList = template.SopStandardServiceList,
                SopTemplateFileList = template.SopTemplateFileList
            };
            var pdf = new Report();
            //pdf.Generate(js, pdfGeneratorModel, fileName);
        }


        private async Task ViewOrderItemStatusLog(int orderItemId)
        {
            orderItemStatusChangeLogs.Clear();
            var orderItemChangeLogs = await _orderItemStatusChangeLogService.GetByOrderItemId(orderItemId);
            orderItemStatusChangeLogs.AddRange(orderItemChangeLogs);
            isOrderItemChangeLogPopupVisible = true;
        }

        private async Task ViewOrderItemActivityLog(int orderItemId)
        {
            activityLogs = await _activityLogService.GetByActivityLogFor(ActivityLogForConstants.OrderItem, orderItemId);
            var downloadLogs = await _activityLogService.GetByActivityLogFor(ActivityLogForConstants.OrderItemDownloadToEditorPcByAutomatedUser, orderItemId);
            if (downloadLogs.Any())
            {
                activityLogs.AddRange(downloadLogs);
            }

            isOrderItemAcitivityLogPopupVisible = true;
        }


        void CloseOrderItemChangeLogPopup()
        {
            isOrderItemChangeLogPopupVisible = false;
            orderItemStatusChangeLogs.Clear();
        }
        void CloseOrderItemActivityLogPopup()
        {
            isOrderItemAcitivityLogPopupVisible = false;
            activityLogs.Clear();
            StateHasChanged();
        }

        #endregion

        #region Create A Text with Order Instructiona and SOP Instructions 
        private async Task<string> CreateTextInstructions(ClientOrderModel order, List<ClientOrderSOPTemplateModel> orderSOPTemplates)
        {
            var text = "";
            var orderInstruction = order.Instructions;
            text = $"{orderInstruction}";

            return text;
        }
        private async Task<string> CreateTextSOPInstruction(SOPTemplateModel sOPTemplate)
        {
            var ServiceName = "";
            var sopserviceList = await _standardServiceService.GetListByTemplateId(sOPTemplate.Id);
            var service = sopserviceList.ToArray();
            for (int i = 0; i < service.ToList().Count; i++)
            {
                ServiceName += $"{i}) {service[i].Name}\n ";

            }
            string sopConvertedInstruction = HtmlToStringConverter.HTMLToText(sOPTemplate.Instruction);
            var instructions = $"SOP Instruction :\n{sopConvertedInstruction}\n SOP Services :\n {ServiceName}";
            return instructions;
        }
        #endregion
        #region Folder View
        private async Task ChangePreview(int viewType)
        {
            await Task.Yield();
            fileViewMode = viewType;
            folderNodes = PrepareFolderPreview("");
            MakeOrderItemUnselected();
            StateHasChanged();
        }
        private int numberOfPage = 0;
        private int pageSize = 10;
        private int pageNumber = 1;
        private List<FolderNodeModel> folderNodeFilesList = new List<FolderNodeModel>();

        private string currentFolderPath = "";
        private async Task LoadSubFoldersAndFiles(string prefixPath)
        {
            await Task.Yield();
            currentFolderPath = prefixPath;
            selectedOrderItemFromFolderStructure = new List<FolderNodeModel>();

            folderNodes = PrepareFolderPreview(prefixPath);
            if (folderNodes != null)
            {
                folderNodeFilesList = folderNodes.Where(f => f.IsFolder == false).ToList();
                numberOfPage = Convert.ToInt32(Math.Ceiling(folderNodeFilesList.Count() / Convert.ToDecimal(pageSize)));
                folderNodeFilesList = folderNodes.Where(f => f.IsFolder == false).ToList().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            }

            StateHasChanged();
        }
        private async Task LoadSelectedFiles(MouseEventArgs args, FolderNodeModel folderNode, string folderPrefix, string folderName)
        {
            await Task.Yield();



            folderNode.IsSelected = true;



            if (args.CtrlKey == true)
            {
                folderNode.IsSelected = true;
            }
            else
            {
                folderNodes.ForEach(f => f.IsSelected = false);
                folderNode.IsSelected = true;
            }

            var prefixPath = folderPrefix + "/" + folderName;
            var items = clientOrderItems.Where(f => f.InternalFileInputPath.Contains(prefixPath)).ToList();

            foreach (var item in items)
            {
                if (!selectedFiles.ToList().Exists(f => f.FileName == item.FileName))
                {
                    selectedFiles.Add(item);
                }

            }
            StateHasChanged();
        }
        private async Task<List<ClientOrderItemModel>> GetAssignNodeItems()
        {
            await Task.Yield();

            var itemsForAction = new List<ClientOrderItemModel>();
            //fileViewMode  , 1 = Grid view, 2 = Folder View
            var selectedItems = new List<FolderNodeModel>();

            var selectedNodes = folderNodes.Where(f => f.IsSelected == true).ToList();

            if (selectedNodes == null || !selectedNodes.Any())
            {
                //Show Alteast one 
                return null;
            }

            //Select files only
            selectedItems = selectedNodes.Where(f => f.IsFolder == false).ToList();

            if (selectedItems != null)
            {
                itemsForAction = clientOrderItems.Where(f => selectedItems.Any(i => i.OrderItemId == f.ClientOrderId)).ToList();
            }

            if (itemsForAction == null)
            {
                itemsForAction = new List<ClientOrderItemModel>();
            }

            //Select fields of folders
            var selectedFolders = selectedNodes.Where(f => f.IsFolder == true).ToList();

            if (selectedFolders != null)
            {
                foreach (var folder in selectedFolders)
                {

                    // here that clientOrderitems file remove from the list where internalInputPaht is null. 
                    clientOrderItems = clientOrderItems.Where(x => x.InternalFileInputPath is not null).ToList();

                    var seacrchingPrefix = folder.Prefix + "/" + folder.FolderName + "/";
                    var folderItems = new List<ClientOrderItemModel>();
                    folderItems = clientOrderItems.Where(f => f.InternalFileInputPath.Contains(seacrchingPrefix)).ToList();


                    #region TODO: Rakib , we feel this code is unnecessary 7_10_24 , delete soon

                    //if (orderItemStatusWiseViewMode == 1)
                    //{
                    //folderItems = clientOrderItems.Where(f => f.InternalFileInputPath.Contains(seacrchingPrefix)).ToList();
                    //}
                    //if (orderItemStatusWiseViewMode == 2)
                    //{
                    //	folderItems = clientOrderItems.Where(f => f.ProductionDoneFilePath.Contains(seacrchingPrefix)).ToList();
                    //}
                    //if (orderItemStatusWiseViewMode == 3)
                    //{
                    //	folderItems = clientOrderItems.Where(f => f.InternalFileOutputPath.Contains(seacrchingPrefix)).ToList();
                    //}
                    #endregion

                    if (folderItems != null)
                    {
                        itemsForAction.AddRange(folderItems);
                    }
                }
            }

            return itemsForAction;
        }
        private List<FolderNodeModel> PrepareFolderPreview(string prefix)
        {
            var folders = new List<FolderNodeModel>();

            if (clientOrderItems == null)
            {
                return folders;
            }

            var items = new List<ClientOrderItemModel>();
            if (orderItemStatusWiseViewMode == 1) //Raw Image
            {
                items = clientOrderItems.Where(f => !string.IsNullOrWhiteSpace(f.InternalFileInputPath)).ToList();
            }
            else if (orderItemStatusWiseViewMode == 2) //Production Done Image
            {
                items = clientOrderItems.Where(f => !string.IsNullOrWhiteSpace(f.ProductionDoneFilePath)).ToList();
            }
            else if (orderItemStatusWiseViewMode == 3) //Completed Done Image
            {
                items = clientOrderItems.Where(f => !string.IsNullOrWhiteSpace(f.InternalFileOutputPath)).ToList();
            }

            string[] splitSpring;

            ///filter 
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var addSlashBeforeAndAfterPrefix = '/' + prefix + '/';
                //splitSpring = new string[] { $"{order.OrderNumber}/{prefix}"};
                splitSpring = new string[] { prefix };
                if (orderItemStatusWiseViewMode == 1) //Raw Image
                {
                    items = items.Where(f => f.InternalFileInputPath.Contains(addSlashBeforeAndAfterPrefix)).ToList();
                }
                else if (orderItemStatusWiseViewMode == 2) //Production Done Image
                {
                    items = items.Where(f => f.ProductionDoneFilePath.Contains(addSlashBeforeAndAfterPrefix)).ToList();
                }
                else if (orderItemStatusWiseViewMode == 3) //Completed Done Image
                {
                    items = items.Where(f => f.InternalFileOutputPath.Contains(addSlashBeforeAndAfterPrefix)).ToList();
                }

                //items = clientOrderItems.Where(f => string.IsNullOrWhiteSpace(f.InternalFileInputPath) ? f.InternalFileOutputPath.Contains(prefix) : f.InternalFileInputPath.Contains(prefix)).ToList();
            }
            else
            {
                //items = clientOrderItems;
                splitSpring = new string[] { order.OrderNumber };
            }

            topDirectoryPath = splitSpring[0];

            foreach (var item in items)
            {
                string[] subPath = new string[1000];
                //if (string.IsNullOrEmpty(item.InternalFileInputPath))
                //{
                //	if (string.IsNullOrEmpty(item.InternalFileOutputPath))
                //	{
                //		subPath = item.ProductionDoneFilePath.Split(splitSpring, StringSplitOptions.None);
                //	}
                //	else
                //	{
                //		subPath = item.InternalFileOutputPath.Split(splitSpring, StringSplitOptions.None);
                //	}

                //}
                //else
                //{
                var splitStringWithSlash = splitSpring[0];

                //}
                if (orderItemStatusWiseViewMode == 1)
                {
                    subPath = item.InternalFileInputPath.Split(splitStringWithSlash, StringSplitOptions.None);
                }
                if (orderItemStatusWiseViewMode == 2)
                {
                    var splitProductionDoneFilePath = new string[] { order.OrderNumber };

                    var donePath = item.ProductionDoneFilePath.Replace("//", "/");

                    var pathArray = donePath.Split(splitSpring, StringSplitOptions.None);
                    var folderNameWithoutEditor = donePath.Split("Production Done", StringSplitOptions.None);
                    var arrayCount = folderNameWithoutEditor.Count();
                    if (arrayCount > 1)
                    {

                        item.QueryFilePath = $"{pathArray[0]}/{folderNameWithoutEditor[1]}";
                        item.QueryFilePath = item.QueryFilePath.Replace("//", "/");
                    }
                    else
                    {
                        var folderNameWithoutEditorRejected = donePath.Split("Rejected", StringSplitOptions.None);
                        item.QueryFilePath = $"{pathArray[0]}/{folderNameWithoutEditorRejected[1]}";
                        item.QueryFilePath = item.QueryFilePath.Replace("//", "/");
                    }
                    subPath = item.QueryFilePath.Split(splitStringWithSlash, StringSplitOptions.None);
                }
                if (orderItemStatusWiseViewMode == 3)
                {
                    item.InternalFileOutputPath = item.InternalFileOutputPath.Replace("\\", "/");
                    subPath = item.InternalFileOutputPath.Split(splitStringWithSlash, StringSplitOptions.None);
                }
                var endPath = "";
                if (orderItemStatusWiseViewMode == 3)
                {
                    endPath = subPath[1].TrimStart('/').TrimStart('/');

                }
                else
                {
                    endPath = subPath[1].TrimStart('/').TrimStart('/');
                }


                var fodlername = endPath.Split("/")[0];

                var folderNode = new FolderNodeModel
                {
                    IsFolder = false,
                    FolderName = fodlername,
                    FullPath = item.ExternalFileOutputPath,
                    PartialPath = subPath[1],
                    Prefix = splitSpring[0],
                    Status = item.Status,
                    OrderItemId = item.Id,
                    InternalFileInputPath = item.InternalFileInputPath,
                    EditorName = item.EditorName,
                    ExternalStatus = item.ExternalStatus,
                    FileGroup = (int)item.FileGroup,
                    ProductionDoneFilePath = item.ProductionDoneFilePath,
                    InternalFileOutputPath = item.InternalFileOutputPath,
                };

                if (endPath.Contains("/"))
                {
                    folderNode.IsFolder = true;

                    var imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".svg"
                    };

                    if (endPath.Split('/').Length == 2 && imageExtensions.Contains(Path.GetExtension(endPath.Split('/')[1])))
                    {
                        folderNode.IsFileParentFolder = true;
                    }

                    var endPathWithoutImage = topDirectoryPath + "/" + folderNode.FolderName;
                    var tempItemList = new List<ClientOrderItemModel>();
                    if (orderItemStatusWiseViewMode == 1)
                    {
                        tempItemList = items.Where(i => i.InternalFileInputPath.Contains(endPathWithoutImage)).ToList();

                        folderNode.ParentFolderContainFilePath = items.Where(i => i.InternalFileInputPath
                                                                .Contains(endPath)).First().InternalFileInputPath;
                    }
                    if (orderItemStatusWiseViewMode == 2)
                    {
                        tempItemList = items.Where(i => i.ProductionDoneFilePath.Contains(endPathWithoutImage)).ToList();
                        folderNode.ParentFolderContainFilePath = items.Where(i => i.ProductionDoneFilePath
                                                                .Contains(endPath)).First().ProductionDoneFilePath;
                    }
                    if (orderItemStatusWiseViewMode == 3)
                    {
                        tempItemList = items.Where(i => i.InternalFileOutputPath.Contains(endPathWithoutImage)).ToList();
                        folderNode.ParentFolderContainFilePath = items.Where(i => i.InternalFileOutputPath
                                                                .Contains(endPath)).First().InternalFileOutputPath;
                    }

                    GetFodlerNodeWiseStatus(folderNode, tempItemList);
                }
                else
                {
                    folderNode.OrderItemId = (long)item.Id;
                }

                if (!folders.Any(f => f.FolderName == fodlername))
                {
                    folders.Add(folderNode);
                }
            }

            return folders.OrderByDescending(f => f.IsFolder).ThenBy(f => f.FolderName).ToList();

        }

        private static void GetFodlerNodeWiseStatus(FolderNodeModel folderNode, List<ClientOrderItemModel> tempItemList)
        {
            try
            {
                var workAbleFileList = tempItemList.Where(i => i.FileGroup == (int)OrderItemFileGroup.Work);
                if (workAbleFileList.Any()) folderNode.FolderMinStatus = (byte)tempItemList.Min(i => i.Status);
                else folderNode.FolderMinStatus = 0;
            }
            catch (Exception ex)
            {

            }
        }

        async Task OnClickTitle(MouseEventArgs args, FolderNodeModel folderNode)
        {
            await Task.Yield();

            folderNode.IsSelected = true;

            if (args.CtrlKey == true)
            {
                folderNode.IsSelected = true;
                //List<ClientOrderItem> selectedItemOnFolderClick = await GetAssignNodeItems();
                //            ((List<ClientOrderItem>)selectedFiles).AddRange(selectedItemOnFolderClick);

            }
            else
            {
                folderNodes.ForEach(f => f.IsSelected = false);
                folderNode.IsSelected = true;

            }
            StateHasChanged();
            //Console.WriteLine($"Title was clicked! - CtrlKey:{args.CtrlKey}");
        }

        #endregion
        #region Private Methods 2
        private async Task DownloadOrderInstruction(ClientOrderModel order)
        {
            var fileName = $"order Attchment{order.Id}.txt";
            var content = HtmlToStringConverter.HTMLToText(order.Instructions);
            await js.InvokeAsync<object>("downloadTextFile", fileName, content);
        }
        private async Task LeaveAssignImage()
        {
            spinShow = true;
            if (fileViewMode == 2 || fileViewMode == 3)
            {
                await LoadOrderItemFromFolderStructureView();
            }

            if (selectedFiles == null)
            {
                spinShow = false;
                StateHasChanged();
                await js.DisplayMessage("Select at least One File");
                return;
            }
            if (selectedFiles.Count <= 0)
            {
                spinShow = false;
                StateHasChanged();
                await js.DisplayMessage("Select at least One File");
                return;
            }

            else
            {
                if (authState.User.IsInRole(PermissionConstants.Order_CanLeaveOrderItem))
                {
                    foreach (var orderItem in selectedFiles)
                    {
                        if (orderItem.Status != (int)InternalOrderItemStatus.InProduction)
                        {
                            spinShow = false;
                            StateHasChanged();
                            await js.DisplayMessage("One or more of your selected image unable to leave  Due To Permission of Status Protocol");
                            return;
                        }
                    }
                }
                if (authState.User.IsInRole(PermissionConstants.Order_CanSeizeOrderItem))
                {
                    foreach (var orderItem in selectedFiles)
                    {
                        if (orderItem.Status != (int)InternalOrderItemStatus.InProduction && orderItem.Status != (int)InternalOrderItemStatus.Distributed)
                        {
                            spinShow = false;
                            StateHasChanged();
                            await js.DisplayMessage("One or more of your selected image unable to leave  Due To Permission of Status Protocol");
                            return;
                        }
                    }
                }
                foreach (var orderItem in selectedFiles)
                {

                    orderItem.Status = (byte)InternalOrderItemStatus.Assigned;
                    orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.Assigned));
                    await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);
                    await AddOrderItemStatusChangeLog(orderItem, InternalOrderItemStatus.Assigned);
                    await _orderAssignedImageEditorService.Delete((int)orderItem.Id);
                }

                await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));

                MakeOrderItemUnselected();

                await LoadOrderItemForLoginUser();
                await LoadSubFoldersAndFiles(currentFolderPath);
                folderNodeFilesList = folderNodes.Where(f => f.IsFolder == false).ToList().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                spinShow = false;
                StateHasChanged();
                await js.DisplayMessage("Successfully Seized !");
                return;
            }
        }

        private async void DeleteImagesOnPreview(int id, string objectId)
        {
            await Task.Yield();
            orderItems.RemoveAll(f => f.ObjectId == objectId);
            StateHasChanged();
            //Delete FROM Database
            await _clientOrderItemService.Delete(objectId);
            await _clientOrderItemService.DeleteFileFromFtp(id);

        }
        //Upload folder
        private async void LoadOrderItemFile(InputFileChangeEventArgs args)
        {
            timer.StartTimer();
            isProgressBar = true;
            isSubmitting = true;
            isUploadInputDisabled = true;
            if (isProgressBar)
            {
                CurrentValueForEditOrderItemPregressbar = 0.1;
            }
            maxValue = args.GetMultipleFiles(maximumFileCount: 3000).Count;
            var count = 0;

            FileServerViewModel fileServerViewModel = new FileServerViewModel();
            fileServerViewModel.Host = fileServer.Host;
            fileServerViewModel.UserName = fileServer.UserName;
            fileServerViewModel.Password = fileServer.Password;

            FileUploadModel fileUploadVM = new FileUploadModel();
            await _dateTime.DateTimeConvert(DateTime.Now);
            fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\";

            ClientOrderItemModel clientOrderITem = new ClientOrderItemModel();
            var CheckingClientOrderItem = new ClientOrderItemModel();
            List<ClientOrderItemModel> DuplicateClientOrderItem = new List<ClientOrderItemModel>();
            var tempClientOrderItems = new List<ClientOrderItemModel>();
            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
            {
                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                ftp.Config.ValidateAnyCertificate = true;
                await ftp.AutoConnect();

                foreach (var file in args.GetMultipleFiles(maximumFileCount: 3000))
                {
                    clientOrderITem = new ClientOrderItemModel();
                    fileUploadVM.FileName = file.Name;

                    var orderItemFile = new ClientOrderItemModel();
                    orderItemFile.FileName = file.Name;
                    orderItemFile.ClientOrderId = order.Id;
                    if (orderItemFile != null)
                    {
                        CheckingClientOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileName(orderItemFile);
                    }
                    if (CheckingClientOrderItem == null)
                    {
                        try
                        {
                            //var fileStam = model.file.OpenReadStream(maxAllowedSize: model.file.Size * 1024);

                            await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), fileUploadVM.UploadDirectory + fileUploadVM.FileName, FtpRemoteExists.Overwrite, true);

                            clientOrderITem.FileType = file.ContentType;
                            clientOrderITem.FileSize = (byte)file.Size;
                            clientOrderITem.FileName = file.Name;
                            clientOrderITem.ObjectId = Guid.NewGuid().ToString();

                        }
                        catch (Exception ex)
                        {
                            string mdd = ex.Message;
                        }
                        clientOrderITem.InternalFileInputPath = $"{fileUploadVM.UploadDirectory}{fileUploadVM.FileName}";
                        clientOrderItem.PartialPath = $"/{order.OrderNumber}";
                        await AddOrderItem(clientOrderITem);
                        order.orderItems.Add(clientOrderITem);
                    }
                    tempClientOrderItems.Add(clientOrderITem);
                    if (uploadCancelItem)
                    {
                        var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                        CurrentValueForEditOrderItemPregressbar = 0;
                        uploadCancelItem = false;
                        spinShow = false;
                        StateHasChanged();
                        break;
                    }
                    if (CheckingClientOrderItem != null)
                    {
                        CheckingClientOrderItem.File = file;
                        DuplicateClientOrderItem.Add(CheckingClientOrderItem);
                    }
                    count++;
                    CurrentValueForEditOrderItemPregressbar = Math.Round((float)((100 / maxValue) * count), 2);
                    StateHasChanged();
                }

                await ftp.Disconnect();
            }
            if (CurrentValueForEditOrderItemPregressbar == 100)
            {
                CurrentValueForEditOrderItemPregressbar = 0;
                isProgressBar = false;
                isSubmitting = false;
                isUploadInputDisabled = false;
                StateHasChanged();
            }
            if (DuplicateClientOrderItem.Count > 0)
            {
                var text = await CreateTextFileName(DuplicateClientOrderItem);
                //await js.DisplayMessage($"The File have already this Order {order.OrderNumber}\n{text}");

                if (await js.ReplaceConfirmation($"The destination has {DuplicateClientOrderItem.Count} files with the same names. Do you want to replace the files ?", $"\n{text}", SweetAlertTypeMessagee.question))
                {
                    clientOrderITem = new ClientOrderItemModel();
                    maxValue = DuplicateClientOrderItem.Count();
                    count = 0;
                    tempClientOrderItems = new List<ClientOrderItemModel>();
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        CurrentValueForEditOrderItemPregressbar = 0.1;
                    }
                    foreach (var file in DuplicateClientOrderItem)
                    {

                        using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                        {
                            await ftp.AutoConnect();

                            await ftp.UploadStream(file.File.OpenReadStream(maxAllowedSize: 107374182400), fileUploadVM.UploadDirectory + fileUploadVM.FileName, FtpRemoteExists.Overwrite, true);

                            await ftp.Disconnect();
                        }

                        clientOrderITem.FileType = file.File.ContentType;
                        clientOrderITem.FileSize = (byte)file.File.Size;
                        clientOrderITem.FileName = file.File.Name;
                        clientOrderITem.ObjectId = file.ObjectId;
                        clientOrderITem.InternalFileInputPath = $"{fileUploadVM.UploadDirectory}{fileUploadVM.FileName}";
                        clientOrderITem.PartialPath = file.PartialPath;
                        clientOrderITem.ClientOrderId = file.ClientOrderId;
                        clientOrderITem.CompanyId = file.CompanyId;
                        clientOrderITem.Id = file.Id;

                        //await AddOrderItem(clientOrderITem);
                        order.orderItems.Add(clientOrderITem);
                        await _clientOrderItemService.UpdateItemFile(clientOrderITem);
                        tempClientOrderItems.Add(clientOrderITem);

                        if (uploadCancelItem)
                        {
                            var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                            CurrentValueForEditOrderItemPregressbar = 0;
                            uploadCancelItem = false;
                            spinShow = false;
                            StateHasChanged();
                            break;
                        }

                        count++;
                        CurrentValueForEditOrderItemPregressbar = Math.Round((float)((100 / maxValue) * count), 2);
                        StateHasChanged();
                    }

                }

                if (CurrentValueForEditOrderItemPregressbar == 100)
                {
                    CurrentValueForEditOrderItemPregressbar = 0;
                    isProgressBar = false;
                    StateHasChanged();
                }
                isSubmitting = false;
                isUploadInputDisabled = false;
                this.StateHasChanged();
                return;
            }
            isSubmitting = false;
            isProgressBar = false;
            isUploadInputDisabled = false;
            clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
            await js.DisplayMessage("Successfully Updated");
            CloseItemAddPopUp();
            this.StateHasChanged();
        }
        //Upload Folder
        private async Task LoadOrderItemFolder(InputFileChangeEventArgs e)
        {
            timer.StartTimer();
            isProgressBar = true;
            isSubmitting = true;
            isUploadInputDisabled = true;
            if (isProgressBar)
            {
                CurrentValueForEditOrderItemPregressbar = 0.1;
            }
            maxValue = e.GetMultipleFiles(maximumFileCount: 3000).Count;
            var loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
            var count = 0;

            FileServerViewModel fileServerViewModel = new FileServerViewModel();
            fileServerViewModel.Host = fileServer.Host;
            fileServerViewModel.UserName = fileServer.UserName;
            fileServerViewModel.Password = fileServer.Password;

            FileUploadModel fileUploadVM = new FileUploadModel();
            await _dateTime.DateTimeConvert(DateTime.Now);

            fileUploadVM.UploadDirectory = $"{orderCompany.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\";

            ClientOrderItemModel clientOrderITem = new ClientOrderItemModel();
            var CheckingClientOrderItem = new ClientOrderItemModel();
            List<ClientOrderItemModel> DuplicateClientOrderItem = new List<ClientOrderItemModel>();
            var tempClientOrderItems = new List<ClientOrderItemModel>();
            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
            {
                await ftp.AutoConnect();

                for (int i = 0; i < loadedFiless.Count; i++)
                {
                    {
                        try
                        {
                            clientOrderITem = new ClientOrderItemModel();
                            fileUploadVM.FileName = loadedFiless[i].Name;
                            var file = loadedFiless[i];
                            var completefilePath = _orderNewItemsUploadFileFromJs[i].Path;

                            clientOrderITem.FileName = fileUploadVM.FileName;
                            clientOrderITem.ClientOrderId = order.Id;
                            clientOrderITem.CompanyId = company.Id;
                            var filepath = Path.GetDirectoryName(completefilePath);
                            if (!string.IsNullOrEmpty(filepath))
                            {
                                var replaceString = filepath.Replace($"\\", @"/");
                                clientOrderITem.PartialPath = @"/" + $"{order.OrderNumber}/{replaceString}" /*+ @"/"*/;
                            }
                            if (!string.IsNullOrEmpty(clientOrderITem.FileName) && !string.IsNullOrEmpty(clientOrderITem.PartialPath))
                            {
                                CheckingClientOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPath(clientOrderITem);
                            }

                            if (CheckingClientOrderItem == null)
                            {
                                try
                                {
                                    var getFolder = Path.GetDirectoryName(_orderNewItemsUploadFileFromJs[i].Path);
                                    if (!string.IsNullOrEmpty(getFolder))
                                    {
                                        fileUploadVM.FileName = Path.GetFileName(_orderNewItemsUploadFileFromJs[i].Path);
                                        fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\{getFolder}";
                                    }

                                    string orderItemPath = " ";

                                    if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                                    {
                                        orderItemPath = $"{fileServer.SubFolder}/{fileUploadVM.UploadDirectory}\\{fileUploadVM.FileName}";
                                    }
                                    else
                                    {
                                        orderItemPath = $"{fileUploadVM.UploadDirectory}\\{fileUploadVM.FileName}";
                                    }

                                    var ftpResponse = await ftp.UploadStream(loadedFiless[i].OpenReadStream(maxAllowedSize: loadedFiless[i].Size * 1024), orderItemPath, FtpRemoteExists.Overwrite, true);

                                    clientOrderITem.FileType = loadedFiless[i].ContentType;
                                    clientOrderITem.FileSize = (byte)loadedFiless[i].Size;
                                    clientOrderITem.FileName = loadedFiless[i].Name;
                                    clientOrderITem.ObjectId = Guid.NewGuid().ToString();
                                }
                                catch (Exception ex)
                                {
                                    string mdd = ex.Message;
                                }
                                clientOrderITem.InternalFileInputPath = $"{fileUploadVM.UploadDirectory}\\{clientOrderITem.FileName}" /*+_selectedFileFromJs[i].Path*/;

                                await AddOrderItem(clientOrderITem);
                                tempClientOrderItems.Add(clientOrderITem);
                                order.orderItems.Add(clientOrderITem);
                            }

                            if (uploadCancelItem)
                            {
                                var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                                CurrentValueForEditOrderItemPregressbar = 0;
                                uploadCancelItem = false;
                                spinShow = false;
                                StateHasChanged();
                                break;
                            }
                            if (CheckingClientOrderItem != null)
                            {
                                CheckingClientOrderItem.File = file;
                                DuplicateClientOrderItem.Add(CheckingClientOrderItem);
                            }
                            count++;
                            CurrentValueForEditOrderItemPregressbar = Math.Round((float)((100 / maxValue) * count), 2);
                            StateHasChanged();
                        }
                        catch
                        {

                        }
                    }
                }

                await ftp.Disconnect();
            }

            if (CurrentValueForEditOrderItemPregressbar == 100)
            {
                CurrentValueForEditOrderItemPregressbar = 0;
                isProgressBar = false;
                StateHasChanged();
            }

            if (DuplicateClientOrderItem.Count > 0)
            {
                var text = await CreateTextFileName(DuplicateClientOrderItem);

                if (await js.ReplaceConfirmation($"The destination has {DuplicateClientOrderItem.Count} files with the same names. Do you want to replace the files?", $" \n{text}", SweetAlertTypeMessagee.question))
                {
                    clientOrderITem = new ClientOrderItemModel();
                    maxValue = DuplicateClientOrderItem.Count();
                    count = 0;
                    tempClientOrderItems = new List<ClientOrderItemModel>();
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        CurrentValueForEditOrderItemPregressbar = 0.1;
                    }
                    using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                    {
                        await ftp.AutoConnect();
                        foreach (var file in DuplicateClientOrderItem)
                        {

                            var ftpResult = await ftp.UploadStream(file.File.OpenReadStream(maxAllowedSize: 107374182400), fileUploadVM.UploadDirectory + fileUploadVM.FileName, FtpRemoteExists.Overwrite, true);


                            clientOrderITem.FileType = file.File.ContentType;
                            clientOrderITem.FileSize = (byte)file.File.Size;
                            clientOrderITem.FileName = file.File.Name;
                            clientOrderITem.ObjectId = file.ObjectId;
                            clientOrderITem.InternalFileInputPath = $"{fileUploadVM.UploadDirectory}{fileUploadVM.FileName}";
                            clientOrderITem.PartialPath = file.PartialPath;
                            clientOrderITem.ClientOrderId = file.ClientOrderId;
                            clientOrderITem.CompanyId = file.CompanyId;
                            clientOrderITem.Id = file.Id;

                            //await AddOrderItem(clientOrderITem);
                            await _clientOrderItemService.UpdateItemFile(clientOrderITem);
                            tempClientOrderItems.Add(clientOrderITem);
                            order.orderItems.Add(clientOrderITem);

                            if (uploadCancelItem)
                            {
                                var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                                CurrentValueForEditOrderItemPregressbar = 0;
                                uploadCancelItem = false;
                                spinShow = false;
                                StateHasChanged();
                                break;
                            }

                            count++;
                            CurrentValueForEditOrderItemPregressbar = Math.Round((float)((100 / maxValue) * count), 2);
                            StateHasChanged();
                        }
                        await ftp.Disconnect();
                    }
                    if (CurrentValueForEditOrderItemPregressbar == 100)
                    {
                        CurrentValueForEditOrderItemPregressbar = 0;
                        isProgressBar = false;
                        StateHasChanged();
                    }
                    isSubmitting = false;
                    isUploadInputDisabled = false;
                    this.StateHasChanged();
                    return;
                }
            }

            isSubmitting = false;
            isProgressBar = false;
            isUploadInputDisabled = false;
            clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
            await js.DisplayMessage("Successfully Updated");
            CloseItemAddPopUp();

            this.StateHasChanged();
        }
        private async Task AddOrderItem(ClientOrderItemModel model)
        {
            var clientOrderItem = new ClientOrderItemModel();
            clientOrderItem.ClientOrderId = order.Id;
            clientOrderItem.CompanyId = model.CompanyId;
            clientOrderItem.IsDeleted = false;

            clientOrderItem.ObjectId = Guid.NewGuid().ToString();
            clientOrderItem.CreatedByContactId = loginUser.ContactId;
            clientOrderItem.FileName = model.FileName;
            clientOrderItem.FileType = model.FileType;
            //clientOrderItem.FileSize = (byte)model.FileSize;
            clientOrderItem.FileByteString = model.FileByteString;
            clientOrderItem.FileGroup = (int)OrderItemFileGroup.Work;
            var UploadPath = model.InternalFileInputPath.Split("Raw");
            var PartialPath = UploadPath[1];
            var GetPartialPath = Path.GetDirectoryName(PartialPath);

            if (!string.IsNullOrWhiteSpace(GetPartialPath))
            {
                var ReplacePartialPath = GetPartialPath.Replace("\\", "/");
                clientOrderItem.PartialPath = ReplacePartialPath;
            }
            else
            {
                clientOrderItem.InternalFileInputPath = model.FileName;
            }

            //Set status
            clientOrderItem.Status = (byte)InternalOrderItemStatus.OrderPlaced;
            clientOrderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.OrderPlaced));
            clientOrderItem.CompanyId = model.CompanyId;
            var ReplaceExternalPath = model.InternalFileInputPath.Replace("\\", "/");
            clientOrderItem.InternalFileInputPath = ReplaceExternalPath;

            if (order.AssignedTeamId > 0)
            {
                clientOrderItem.TeamId = order.AssignedTeamId;
            }

            if (clientOrderItem != null)
            {
                clientOrderItem.TeamId = model.TeamId;
            }

            var addItemResponse = await _clientOrderItemService.Insert(clientOrderItem, (int)order.Id);

            if (addItemResponse.IsSuccess)
            {
                clientOrderItem.Id = addItemResponse.Result;
                orderItems.Add(clientOrderItem);
                StateHasChanged();
                await AddOrderItemStatusChangeLog(clientOrderItem, InternalOrderItemStatus.OrderPlaced);
            }

            //Add Order Item / Files in database 
            if (!isOrderItemAddPopupVisible)
            {
                return;
            }
            if (!isOrderFileAddPopupVisible)
            {
                return;
            }
        }
        private async Task<FileServerViewModel> GetFileServerViewModel()
        {
            await Task.Yield();

            FileServerViewModel fileServerViewModel = new FileServerViewModel();
            fileServerViewModel.Host = fileServer.Host;
            fileServerViewModel.UserName = fileServer.UserName;
            fileServerViewModel.Password = fileServer.Password;

            return fileServerViewModel;
        }
        private async void OrderAttachmentsUpload(InputFileChangeEventArgs args)
        {
            isProgressBar = true;
            //isSubmitting = true;

            if (isProgressBar)
            {
                CurrentValueForEditOrderAttachementPregressbar = 1;
            }
            maxValue = args.GetMultipleFiles(maximumFileCount: 3000).Count;
            var count = 0;

            FileServerViewModel fileServerViewModel = new FileServerViewModel();
            fileServerViewModel.Host = fileServer.Host;
            fileServerViewModel.UserName = fileServer.UserName;
            fileServerViewModel.Password = fileServer.Password;

            FileUploadModel fileUploadVM = new FileUploadModel();
            await _dateTime.DateTimeConvert(DateTime.Now);
            fileUploadVM.UploadDirectory = $"{orderCompany.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\OrderAttachment\\";

            OrderFileAttachment orderFileAttachment = new OrderFileAttachment();
            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
            {
                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                ftp.Config.ValidateAnyCertificate = true;
                await ftp.AutoConnect();

                foreach (var file in args.GetMultipleFiles(maximumFileCount: 3000))
                {
                    orderFileAttachment.FileName = file.Name;
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                            {
                                await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), $"{fileServer.SubFolder}/{fileUploadVM.UploadDirectory + file.Name}", FtpRemoteExists.Overwrite, true);
                                //await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), fileUploadVM.UploadDirectory + orderFileAttachment.FileName, FtpRemoteExists.Overwrite, true);
                            }
                            else
                            {
                                await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), fileUploadVM.UploadDirectory + file.Name, FtpRemoteExists.Overwrite, true);
                            }
                            //add fiel in ftp


                            orderFileAttachment.FileName = file.Name.ToString();
                            orderFileAttachment.File = file;
                            orderFileAttachment.ObjectId = Guid.NewGuid().ToString();
                            orderFileAttachment.CreatedByContactId = loginUser.ContactId;
                            orderFileAttachment.FileName = file.Name;
                            orderFileAttachment.FileType = file.ContentType;
                            orderFileAttachment.FileSize = (byte)file.Size;
                            orderFileAttachment.PartialPath = fileUploadVM.UploadDirectory;

                            //Add record in database
                            await AddNewOrderAttachment(orderFileAttachment);  //TODO: need to delete file from ftp 
                        }
                        catch (Exception ex)
                        {
                            string mdd = ex.Message;
                        }
                    }

                    count++;
                    CurrentValueForEditOrderAttachementPregressbar = (int)((100 / maxValue) * count);
                    StateHasChanged();
                }

                await ftp.Disconnect();
            }

            if (CurrentValueForEditOrderAttachementPregressbar == 100)
            {
                CurrentValueForEditOrderAttachementPregressbar = 0;
                isProgressBar = false;
                StateHasChanged();
            }
            orderAttachmentListForView = new List<OrderFileAttachment>();

            await js.DisplayMessage("Successfully add attachtment(s)");
            CloseAttachmentEditPopUp();
            this.StateHasChanged();
        }
        private async void DeleteAttachImagesOnPreview(OrderFileAttachment orderAttachment)
        {
            await Task.Yield();
            await _orderFileAttachmentService.Delete(orderAttachment.Id);



            orderAttachmentListForView = orderAttachmentListForView.Where(f => f.FileName != orderAttachment.FileName).ToList();
            var fileServer = await GetFileServerViewModel();



            FileUploadModel fileUploadVM = new FileUploadModel()
            {
                FtpUrl = fileServer.Host,
                userName = fileServer.UserName,
                password = fileServer.Password,
                SubFolder = fileServer.SubFolder,
                ReturnPath = $"{orderAttachment.PartialPath}\\{orderAttachment.FileName}"



            };



            await _fluentFtpService.DeleteFile(fileUploadVM);
            StateHasChanged();
        }
        private async void DeleteSOPTemplate(SOPTemplateModel template)
        {
            spinShow = true;
            await _clientOrderService.DeleteOrderSopTemplateByOrderIdAndSopTemplateId((int)order.Id, (int)template.Id);
            orderSOPTemplates = await _orderTemplateService.GetAllByOrderId((int)order.Id);
            if (orderSOPTemplates == null || orderSOPTemplates.Count() <= 0)
            {
                sopTemplates = new List<SOPTemplateModel>();
                spinShow = false;
                StateHasChanged();
                return;
            }
            templateIds = new List<int>();
            foreach (var item in orderSOPTemplates)
            {
                templateIds.Add(item.SOP_Template_Id);
            }
            sopTemplates = await GetSOPTemplate(templateIds);
            spinShow = false;
            StateHasChanged();
        }
        private void UploadSop(bool isPopupp)
        {
            //Not Yet Implemented
        }
        private async Task<string> CreateTextFileName(List<ClientOrderItemModel> files)
        {
            await Task.Yield();

            var text = "";
            var fileName = "";
            var filesArray = files.ToArray();
            for (int i = 0; i < filesArray.ToList().Count; i++)
            {
                fileName += $"{i}) {filesArray[i].FileName}\n";
            }
            text = $"File Names : \n {fileName}\n";
            return text;
        }
        private async Task DownloadOrderItemFromFolderStructure()
        {
            selectedFiles = await GetAssignNodeItems();
            await SingleDownloadEditor();
        }
        #endregion
        #region Preview Order SOP From Order SOP Template 
        //      private void OrderUploadSop(OrderSOPTemplate orderSOPTemplate)
        //      {
        //          var template = orderSOPTemplate;
        //          orderSOPTemplateUploadUpdatePopup = true;
        //	//Not Yet Implemented
        //}
        private void OrderUploadSop()
        {
            //var template = orderSOPTemplate;
            orderSOPTemplateUploadUpdatePopup = true;
            //Not Yet Implemented
        }
        public async Task DownloadOrderSOPFileAttachment(OrderSOPTemplateModel sOPTemplate)
        {
            spinShow = true;
            isSubmitting = true;
            var contactInfo = await _contactManager.GetById(loginUser.ContactId);
            var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + contactInfo.Id}";

            if (Directory.Exists(downloadPath))
            {
                DirectoryInfo directory = new DirectoryInfo(downloadPath);
                directory.Delete(true);
            }

            if (sOPTemplate.Id > 0)
            {
                var result = false;
                isProgressBar = true;
                var selectedFile = await _orderTemplateSOPFileService.GetOrderSopTemplateFilesByOrderSopTemplateId(sOPTemplate.Id);
                var sopTemplateInfo = await _orderTemplateSOPService.GetById((int)sOPTemplate.Id);

                var count = 0;
                var maxValue = selectedFile.Count();
                // maxValue = selectImages.Count;
                var destFile = "";
                FileUploadModel modell = new FileUploadModel();
                modell.ContactName = contactInfo.FirstName + contactInfo.Id;

                destFile = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{modell.ContactName}\\{sopTemplateInfo.Name}\\";

                foreach (var file in selectedFile)
                {
                    //FileUploadVM modell = new FileUploadVM();
                    var sourcePath = $"{this._webHostEnvironment.WebRootPath}\\Upload\\Order Attachments\\{file.ViewPath}\\{file.FileName}";
                    //modell.ContactName = contactInfo.FirstName + contactInfo.Id;
                    //destFile = $"{this._webHostEnvironment.WebRootPath}\\{modell.ContactName}\\{sopTemplateInfo.Name}\\";

                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }
                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, $"{destFile}\\{file.FileName}", true);
                    }
                    this.StateHasChanged();
                }
                var instructionText = await CreateOrderSOPInstructions(sopTemplateInfo);

                if (!string.IsNullOrEmpty(instructionText))
                {
                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }

                    // Directory Check
                    if (Directory.Exists(destFile))
                    {
                        // Create Text File 
                        var datapath = destFile + "instruction.txt";
                        if (!File.Exists(datapath))
                        {
                            await File.WriteAllTextAsync(datapath, instructionText);
                            result = true;
                        }
                    }
                }
                var serviceListNameText = await CreateTextOrderSOPService(sopTemplateInfo);

                if (!string.IsNullOrEmpty(serviceListNameText))
                {
                    if (!Directory.Exists(destFile))
                    {
                        Directory.CreateDirectory(destFile);
                    }

                    // Directory Check
                    if (Directory.Exists(destFile))
                    {
                        // Create Text File 
                        var datapath = destFile + "services.txt";
                        if (!File.Exists(datapath))
                        {
                            await File.WriteAllTextAsync(datapath, serviceListNameText);
                            result = true;
                        }
                    }
                }
                if (result)
                {
                    var webHost = $"{this._webHostEnvironment.WebRootPath}";
                    //var model = await DownloadOrderItemZipFile(contactInfo, order, fileinfo);
                    await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, sopTemplateInfo);
                    await js.DisplayMessage("Download Succesfully");

                    //UriHelper.NavigateTo("/order/Details" + "/" + orderInfo.ObjectId, true);
                    this.StateHasChanged();
                    spinShow = false;
                }
                else
                {
                    await js.DisplayMessage("Download Failed");
                }

            }
            isSubmitting = false;
        }
        private async Task<List<OrderSOPTemplateModel>> GetOrderSOPTemplate(List<int> templateIds)
        {
            List<OrderSOPTemplateModel> orderSOPTemplates = new List<OrderSOPTemplateModel>();

            foreach (var templateId in templateIds)
            {
                OrderSOPTemplateModel sopTemplate = new OrderSOPTemplateModel();
                sopTemplate = await _orderTemplateSOPService.GetByIdAndIsDeletedFalse(templateId);
                orderSOPTemplates.Add(sopTemplate);
            }

            return orderSOPTemplates;
        }
        private async Task<string> CreateTextOrderSOPInstruction(OrderSOPTemplateModel sOPTemplate)
        {
            var ServiceName = "";
            var sopserviceList = await _orderSOPStandardService.GetListByOrderTemplateId(sOPTemplate.Id);
            var service = sopserviceList.ToArray();
            for (int i = 0; i < service.ToList().Count; i++)
            {
                ServiceName += $"{i}) {service[i].Name}\n ";

            }
            string sopConvertedInstruction = HtmlToStringConverter.HTMLToText(sOPTemplate.Instruction);
            var instructions = $"SOP Instruction :\n{sopConvertedInstruction}\n SOP Services :\n {ServiceName}";
            return instructions;
        }
        private async Task<string> CreateOrderSOPInstructions(OrderSOPTemplateModel sOPTemplate)
        {
            string sopConvertedInstruction = HtmlToStringConverter.HTMLToText(sOPTemplate.Instruction);
            var instructions = $"SOP Instruction :\n{sopConvertedInstruction}\n";
            return instructions;
        }
        private async Task<string> CreateTextOrderSOPService(OrderSOPTemplateModel sOPTemplate)
        {
            var ServiceName = "";
            var sopserviceList = await _orderSOPStandardService.GetListByOrderTemplateId(sOPTemplate.Id);
            var service = sopserviceList.ToArray();
            for (int i = 0; i < service.ToList().Count; i++)
            {
                ServiceName += $"{i}) {service[i].Name}\n ";

            }
            var instructions = $" SOP Services :\n {ServiceName}";
            return instructions;
        }
        #region Order SOP Edit and Upload For OPeration
        [JSInvokable]
        public static Task GetOrderSOPExistingNewItems(List<FileForUploadDetails> files)
        {
            _orderSOPExistingItemsUploadFileFromJs = files;
            return Task.CompletedTask;
        }
        private async Task LoadOrderSOPFolder(InputFileChangeEventArgs e)
        {
            isSubmitting = true;
            spinShow = true;
            isOrderSOPUploadInputDisabled = true;
            bool isDeletedResult = false;
            maxValue = e.GetMultipleFiles(maximumFileCount: 3000).Count;
            var loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
            List<IBrowserFile> textFiles = new List<IBrowserFile>();
            List<IBrowserFile> orderSOPAttachmentFiles = new List<IBrowserFile>();
            var sopName = "";
            for (int i = 0; i < loadedFiless.Count; i++)
            {
                sopName = Path.GetDirectoryName(_orderSOPExistingItemsUploadFileFromJs[i].Path);
                var file = loadedFiless[i];
                var extensionName = Path.GetExtension(file.Name);
                var fileExtension = ".txt";
                if (fileExtension.Contains(extensionName))
                {
                    textFiles.Add(file);
                }
                else
                {
                    orderSOPAttachmentFiles.Add(file);
                }
            }

            var ClientOrderSOPTemplate = await _orderSOPTemplateJoiningService.GetByClientOrderId((int)order.Id);

            foreach (var orderSOPTemplate in ClientOrderSOPTemplate)
            {
                var findOrderSOPTemplate = await _orderTemplateSOPService.GetById(orderSOPTemplate.OrderSOP_Template_Id);
                if (findOrderSOPTemplate.Name == sopName)
                {
                    string viewPath = "";
                    string folder = "";
                    string rootfolder = "";
                    if (string.IsNullOrWhiteSpace(rootfolder))
                    {
                        folder = $"{this._webHostEnvironment.WebRootPath}\\EditedTextFile\\{company.Name}\\SOP-{_dateTime.currenDateTime}";
                        rootfolder = $"wwwroot\\Upload\\{company.Name}\\SOP-{_dateTime.currenDateTime}";
                        await _folderService.CreateFolder(folder);
                        viewPath = $"{company.Name}/SOP-{_dateTime.currenDateTime}";
                    }
                    var instructions = textFiles.Where(x => x.Name == "instruction.txt").FirstOrDefault();
                    if (instructions != null)
                    {

                        // Save the file in another directory for reading the text file
                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await instructions.OpenReadStream(maxAllowedSize: 102400000000000).CopyToAsync(memoryStream);
                                var filepath = $"{folder}\\{instructions.Name}";

                                var fileUploadResult = await _localFileService.UploadAsync(filepath, memoryStream);
                                if (fileUploadResult)
                                {
                                    string text = File.ReadAllText(filepath);
                                    string removeText = text.Remove(0, 17);
                                    string removeNewLines = removeText.Replace("\n", "<br>").Replace("\r", "<br>");
                                    var orderSopTemplate = new OrderSOPTemplateModel()
                                    {
                                        Id = findOrderSOPTemplate.Id,
                                        Name = findOrderSOPTemplate.Name,
                                        Instruction = removeNewLines,
                                        UpdatedByContactId = loginUser.ContactId,
                                    };
                                    var result = await _orderTemplateSOPService.UpdateOrderSOPTemplateInstruction(orderSopTemplate);
                                    if (result.IsSuccess)
                                    {
                                        isDeletedResult = true;
                                    }
                                }

                                this.StateHasChanged();
                            }
                        }
                        catch
                        {
                        }
                    }
                    var services = textFiles.Where(x => x.Name == "services.txt").FirstOrDefault();
                    if (services != null)
                    {
                        // Save the file in another directory for reading the text file
                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await services.OpenReadStream(maxAllowedSize: 102400000000000).CopyToAsync(memoryStream);
                                var filepath = $"{folder}\\{services.Name}";

                                var fileUploadResult = await _localFileService.UploadAsync(filepath, memoryStream);
                                if (fileUploadResult)
                                {
                                    string text = File.ReadAllText(filepath);
                                    string removeText = text.Remove(0, 17);
                                    string[] replaceText = removeText.Split("\n").ToArray();

                                    for (int i = 0; i < replaceText.ToList().Count; i++)
                                    {
                                        var sopServiceName = replaceText[i];
                                        var removeExtraSOPServiceName = sopServiceName.Remove(0, 4);
                                        var findSOPServiceByName = await _orderSOPStandardService.GetByOrderSOPName(removeExtraSOPServiceName);
                                        if (findSOPServiceByName != null && removeExtraSOPServiceName != findSOPServiceByName.Name)
                                        {
                                            var orderStandardService = new OrderSOPStandardServiceModel()
                                            {
                                                Name = removeExtraSOPServiceName,
                                                Status = findSOPServiceByName.Status,
                                                IsDeleted = findSOPServiceByName.IsDeleted,
                                                CreatedByContactId = loginUser.ContactId,
                                                ObjectId = Guid.NewGuid().ToString(),
                                                ParentSopServiceId = findSOPServiceByName.ParentSopServiceId,
                                                BaseSOPServiceId = findSOPServiceByName.BaseSOPServiceId
                                            };
                                            var orderSOPInsertResult = await _orderSOPStandardService.Insert(orderStandardService);
                                            var orderSOPStandardServiceId = orderSOPInsertResult.Result;

                                            var orderSOPStandardServiceTemplate = new OrderSOPTemplateServiceModel()
                                            {
                                                OrderSOPTemplateId = findOrderSOPTemplate.Id,
                                                OrderSOPStandardServiceId = orderSOPStandardServiceId,

                                                ObjectId = Guid.NewGuid().ToString(),
                                                //BaseTemplateId = findOrderSOPTemplate.BaseTemplateId,

                                            };
                                            var orderSopServiceAndSOPTemplateSaveResult = await _orderSOPTemplateOrderSOPStandardService.Insert(orderSOPStandardServiceTemplate);
                                        }
                                    }
                                }

                                this.StateHasChanged();
                            }
                        }
                        catch
                        {
                        }
                    }

                    foreach (var file in orderSOPAttachmentFiles)
                    {
                        var orderSOPTemplateFile = new OrderSOPTemplateFile()
                        {
                            OrderSOPTemplateId = findOrderSOPTemplate.Id,
                            FileName = file.Name,
                        };
                        var findOrderSOPFile = await _orderTemplateSOPFileService.GetByOrderSOPTemplateIdAndFileName(orderSOPTemplateFile);
                        if (!string.IsNullOrEmpty(findOrderSOPFile.ActualPath))
                        {
                            if (File.Exists(findOrderSOPFile.ActualPath))
                            {
                                File.Delete(findOrderSOPFile.ActualPath);
                            }
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.OpenReadStream(maxAllowedSize: 102400000000000).CopyToAsync(memoryStream);
                                var filepath = $"{findOrderSOPFile.RootFolderPath}\\{file.Name}";

                                var fileUploadResult = await _localFileService.UploadAsync(filepath, memoryStream);
                                this.StateHasChanged();
                            }
                        }
                    }
                }
            }

            if (isDeletedResult)
            {
                spinShow = false;
                await js.DisplayMessage("Order SOP Upload Successfully");
                _uriHelper.NavigateTo($"/order/Details/{objectId}", true);
            }
            else
            {
                await js.DisplayMessage("Please Try Again Or Something Wrong");
                isOrderSOPUploadInputDisabled = false;
            }
            spinShow = false;
            orderSOPTemplateUploadUpdatePopup = false;
        }

        private async Task OrderSOPDelete(OrderSOPTemplateModel orderSOPTemplate)
        {
            if (await js.Confirmation("Yes", "Are you sure want to delete ?", SweetAlertTypeMessagee.question))
            {
                var orderSOPTemplateModel = new OrderSOPTemplateModel()
                {
                    Id = orderSOPTemplate.Id,
                    IsDeleted = true
                };

                var deleteResult = await _orderTemplateSOPService.Delete(orderSOPTemplateModel);
                if (deleteResult.IsSuccess)
                {
                    await js.DisplayMessage("Order SOP Delete Successfully");

                    // This method for Reload
                    //await LoadOrderItemForLoginUser();
                    // Need to reload method for delete 
                    _uriHelper.NavigateTo($"/order/Details/{objectId}", true);
                }
                else
                {
                    await js.DisplayMessage("Order SOP not Successfully Deleted !!");
                }
            }
        }

        #endregion

        #endregion
        public DateTime? OrderImageExpectedDeliveryDate = null;

        private async Task UploadCancelOrderItem()
        {
            spinShow = true;
            uploadCancelItem = true;
            StateHasChanged();
        }
        #region Popum view Approved or Reject
        private async Task RejectFromPopupView(InternalOrderItemStatus status = InternalOrderItemStatus.InQc, int imageId = 0)
        {
            spinShow = true;
            if (imageId > 0)
            {
                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                var fileInfo = await _clientOrderItemService.GetById((int)imageId);

                if (fileInfo.Status == (byte)InternalOrderItemStatus.ProductionDone || fileInfo.Status == (byte)InternalOrderItemStatus.ReworkDone)
                {
                    await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Reject First Take them in QC");
                    return;
                }

                var imageInfo = await _assignService.GetById((int)fileInfo.Id);
                var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);
                FileUploadModel model = new FileUploadModel();
                model.fileName = Path.GetFileName(fileInfo.ProductionDoneFilePath);
                model.FtpUrl = serverInfo.Host;
                model.userName = serverInfo.UserName;
                model.password = serverInfo.Password;
                model.OrderNumber = order.OrderNumber;
                model.SubFolder = serverInfo.SubFolder;
                var path = $"{fileInfo.InternalFileInputPath}";
                var dividePath = path.Split("/");
                model.RootDirectory = dividePath[0];
                var dividePathFor = path.Split("Raw");
                model.DownloadFolderName = dividePathFor[0];
                var baseDirectory = dividePathFor[0];
                model.Date = order.CreatedDate;
                model.ContactName = editorInfo.FirstName.Trim() + " " + editorInfo.Id;
                model.UploadDirectory = $"{baseDirectory}/In Progress/{model.OrderNumber}/{model.ContactName}/Production Done/";
                model.ReturnPath = $"{baseDirectory}/In Progress/{order.OrderNumber}/{editorInfo.FirstName + editorInfo.Id}/Rejected/{fileInfo.PartialPath}/";
                await _fluentFtpService.FolderCreateAtApprovedTime(model);
                FileUploadModel modell = new FileUploadModel()
                {
                    FtpUrl = serverInfo.Host,
                    userName = serverInfo.UserName,
                    password = serverInfo.Password,
                    ReturnPath = $"{model.ReturnPath}/{model.fileName}",
                    UploadDirectory = $"{model.UploadDirectory}/{fileInfo.PartialPath}/{model.fileName}",
                    SubFolder = serverInfo.SubFolder
                };

                var fileMoveResult = await _fluentFtpService.MoveFile(modell);

                if (fileMoveResult.IsSuccess)
                {
                    ClientOrderItemModel clientOrderItem = new ClientOrderItemModel()
                    {
                        CompanyId = fileInfo.CompanyId,
                        FileName = fileInfo.FileName,
                        ProductionDoneFilePath = $"{model.ReturnPath}/{model.fileName}".Replace("//", "/"),
                        PartialPath = fileInfo.PartialPath,
                        ClientOrderId = order.Id,
                        Id = fileInfo.Id,
                    };

                    var updateItemInDBResponse = await _clientOrderItemService.UpdateItemByQCForReject(clientOrderItem);

                    await UpdateOrderItemStatusFromImagePreviewPopUp(order, clientOrderItem, status);
                    MakeOrderItemUnselected();

                    //Order Status Update
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus
                }



                await LoadOrderItemForLoginUser();
                //Order Status Update
                isShowImagePopup = false;
            }
            else
            {
                await js.DisplayMessage("Select at least one file or item !");
            }
            spinShow = true;
            int clientItemIndex = clientOrderItems.FindIndex(x => x.Id.Equals(clientOrderItem.Id));
            clientOrderItem = new ClientOrderItemModel();
            clientOrderItem = clientOrderItems[clientItemIndex + 1];
            this.StateHasChanged();
            spinShow = false;
        }
        private async Task ApprovedQCForPopupView(InternalOrderItemStatus status, int imageId = 0, bool FileMove = true)
        {
            if (imageId > 0)
            {
                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
                var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                var fileInfo = await _clientOrderItemService.GetById((int)imageId);
                if (fileInfo.Status == (byte)InternalOrderItemStatus.ProductionDone || fileInfo.Status == (byte)InternalOrderItemStatus.ReworkDone)
                {
                    await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Approve or Reject First Take them in QC");
                    return;
                }
                var imageInfo = await _assignService.GetById((int)fileInfo.Id);
                var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);

                FileUploadModel model = new FileUploadModel();
                model.fileName = Path.GetFileName(fileInfo.ProductionDoneFilePath);
                model.FtpUrl = serverInfo.Host;
                model.userName = serverInfo.UserName;
                model.password = serverInfo.Password;
                model.SubFolder = serverInfo.SubFolder;
                model.OrderNumber = order.OrderNumber;
                var path = $"{fileInfo.InternalFileInputPath}";
                var dividePath = path.Split("\\");
                model.RootDirectory = dividePath[0];
                var dividePathFor = path.Split("Raw");
                model.DownloadFolderName = dividePathFor[1];
                var baseDirectory = dividePathFor[0];
                model.Date = order.CreatedDate;
                model.ContactName = editorInfo.FirstName.Trim() + " " + editorInfo.Id;
                model.UploadDirectory = $"{baseDirectory}/In Progress/{model.OrderNumber}/{model.ContactName}/Production Done/{fileInfo.PartialPath}";
                model.ReturnPath = $"{baseDirectory}/Completed/{fileInfo.PartialPath}";

                if (FileMove)
                {
                    await _fluentFtpService.FolderCreateAtApprovedTime(model);

                    FileUploadModel fileUploadVM = new FileUploadModel()
                    {
                        FtpUrl = serverInfo.Host,
                        userName = serverInfo.UserName,
                        password = serverInfo.Password,
                        SubFolder = serverInfo.SubFolder,
                        ReturnPath = $"{baseDirectory}/Completed/{fileInfo.PartialPath}/{model.fileName}",
                        UploadDirectory = $"{model.UploadDirectory}/{model.fileName}"
                    };

                    var fileMoveResult = await _fluentFtpService.MoveFile(fileUploadVM);

                    if (fileMoveResult.IsSuccess)
                    {
                        ClientOrderItemModel clientOrderItem = new ClientOrderItemModel()
                        {
                            CompanyId = fileInfo.CompanyId,
                            FileName = fileInfo.FileName,
                            InternalFileOutputPath = $"{model.ReturnPath}/{model.fileName}".Replace("//", "/"),
                            PartialPath = fileInfo.PartialPath,
                            ClientOrderId = order.Id,
                            Id = fileInfo.Id,
                        };

                        var updateItemInDBResponse = await _clientOrderItemService.UpdateItemByQC(clientOrderItem);

                        await UpdateOrderItemStatusFromImagePreviewPopUp(order, clientOrderItem, status);
                        MakeOrderItemUnselected();
                        await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus
                    }
                }

                if (loginUser.CompanyType == (int)CompanyType.Client)
                {
                    var clientOrderItem = await _clientOrderItemService.GetById(imageId);
                    await _updateOrderItemBLLService.UpdateOrderItemStatus(clientOrderItem, InternalOrderItemStatus.Completed);
                }

                await LoadOrderItemForLoginUser();
                //Order Status Update
                isShowImagePopup = false;
            }
            else
            {
                await js.DisplayMessage("Select at least one Item !");
                return;
            }
            spinShow = true;
            int clientItemIndex = clientOrderItems.FindIndex(x => x.Id.Equals(clientOrderItem.Id));
            int LastIndex = clientOrderItems.Count() - 1;
            if ((clientItemIndex + 1) > LastIndex)
            {
                CloseProductionDoneImagePopup();
            }
            else
            {
                clientOrderItem = new ClientOrderItemModel();
                clientOrderItem = clientOrderItems[clientItemIndex + 1];
            }

            this.StateHasChanged();
            spinShow = false;
        }
        private async Task PreviousImageShow(ClientOrderItemModel clientOrderItemVM)
        {
            int clientItemIndex = clientOrderItems.FindIndex(x => x.Id.Equals(clientOrderItemVM.Id));

            if (clientItemIndex == 0)
            {
                return;
            }

            if ((clientItemIndex - 1) == -1)
            {
                CloseProductionDoneImagePopup();
            }
            else
            {
                clientOrderItem = new ClientOrderItemModel();
                clientOrderItem = clientOrderItems[clientItemIndex - 1];
                await ShowProductionDoneImagePopup(clientOrderItem);
            }

        }
        private async Task NextImageShow(ClientOrderItemModel clientOrderItemVM)
        {
            int clientItemIndex = clientOrderItems.FindIndex(x => x.Id.Equals(clientOrderItemVM.Id));
            int LastIndex = clientOrderItems.Count() - 1;
            if ((clientItemIndex + 1) > LastIndex)
            {
                CloseProductionDoneImagePopup();
            }
            else
            {
                clientOrderItem = new ClientOrderItemModel();
                clientOrderItem = clientOrderItems[clientItemIndex + 1];
                await ShowProductionDoneImagePopup(clientOrderItem);
            }
            this.StateHasChanged();
        }
        private async Task ClientPreviousImageShow(ClientOrderItemModel clientOrderItemVM)
        {
            int clientItemIndex = clientOrderItems.FindIndex(x => x.Id.Equals(clientOrderItemVM.Id));

            if (clientItemIndex == 0)
            {
                return;
            }

            if ((clientItemIndex - 1) == -1)
            {
                CloseProductionDoneImagePopup();
            }
            else
            {
                //clientOrderItem = new ClientOrderItem();
                clientOrderItem = clientOrderItems[clientItemIndex - 1];
            }
            await ShowProductionDoneImagePopup(clientOrderItem);
            this.StateHasChanged();
        }
        private async Task ClientNextImageShow(ClientOrderItemModel clientOrderItemVM)
        {
            int clientItemIndex = clientOrderItems.FindIndex(x => x.Id.Equals(clientOrderItemVM.Id));
            int LastIndex = clientOrderItems.Count() - 1;
            if ((clientItemIndex + 1) > LastIndex)
            {
                CloseProductionDoneImagePopup();
            }
            else
            {
                //clientOrderItem = new ClientOrderItem();
                clientOrderItem = clientOrderItems[clientItemIndex + 1];
            }
            await ShowProductionDoneImagePopup(clientOrderItem);
            this.StateHasChanged();
        }

        #endregion

        #region Adding new File For Existing Order and Same Path
        private async Task LoadOrderFileFolder(InputFileChangeEventArgs e)
        {
            try
            {
                timer.StartTimer();
                isProgressBar = true;
                isSubmitting = true;
                isUploadInputDisabled = true;
                if (isProgressBar)
                {
                    CurrentValueForEditOrderItemPregressbar = 0.1;
                }
                maxValue = e.GetMultipleFiles(maximumFileCount: 3000).Count;
                var loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
                var count = 0;

                FileServerViewModel fileServerViewModel = new FileServerViewModel();
                fileServerViewModel.Host = fileServer.Host;
                fileServerViewModel.UserName = fileServer.UserName;
                fileServerViewModel.Password = fileServer.Password;


                FileUploadModel fileUploadVM = new FileUploadModel();
                await _dateTime.DateTimeConvert(DateTime.Now);

                fileUploadVM.UploadDirectory = $"{orderCompany.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\";


                var DuplicateClientOrderItem = new List<ClientOrderItemModel>();
                var tempClientOrderItems = new List<ClientOrderItemModel>();
                var returnPath = "";
                using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                {
                    ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    ftp.Config.ValidateAnyCertificate = true;
                    await ftp.AutoConnect();

                    for (int i = 0; i < loadedFiless.Count; i++)
                    {
                        var CheckingClientOrderItem = new ClientOrderItemModel();
                        {
                            try
                            {
                                var clientOrderITem = new ClientOrderItemModel();
                                fileUploadVM.FileName = loadedFiless[i].Name;
                                var file = loadedFiless[i];
                                var completefilePath = _orderNewItemsUploadFileFromJs[i].Path;
                                if (i == 0)
                                {
                                    returnPath = await PathSplit(completefilePath);
                                }
                                if (string.IsNullOrEmpty(returnPath))
                                {
                                    await js.DisplayMessage("This order file no longer in Ftp !!");
                                    isProgressBar = false;
                                    isSubmitting = false;
                                    isUploadInputDisabled = false;
                                    StateHasChanged();
                                    return;
                                }

                                clientOrderITem.FileName = fileUploadVM.FileName;
                                clientOrderITem.ClientOrderId = order.Id;
                                clientOrderITem.CompanyId = company.Id;
                                var filepath = Path.GetDirectoryName(completefilePath);
                                if (!string.IsNullOrEmpty(filepath))
                                {
                                    var replaceString = filepath.Replace($"\\", @"/");
                                    var splitFilePath = replaceString.Split(returnPath);
                                    clientOrderITem.PartialPath = @"/" + $"{order.OrderNumber}/{returnPath}{splitFilePath[1]}" /*+ @"/"*/;
                                }
                                if (!string.IsNullOrEmpty(clientOrderITem.FileName) && !string.IsNullOrEmpty(clientOrderITem.PartialPath))
                                {
                                    CheckingClientOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPath(clientOrderITem);
                                }

                                if (CheckingClientOrderItem == null)
                                {
                                    try
                                    {
                                        var getFolder = Path.GetDirectoryName(_orderNewItemsUploadFileFromJs[i].Path);
                                        if (!string.IsNullOrEmpty(getFolder))
                                        {
                                            var splitFilePath = getFolder.Split(returnPath);
                                            fileUploadVM.FileName = Path.GetFileName(_orderNewItemsUploadFileFromJs[i].Path);
                                            fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\{returnPath}{splitFilePath[1]}";
                                        }
                                        var destination = $"{fileUploadVM.UploadDirectory}\\{fileUploadVM.FileName}";


                                        if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))

                                        {

                                            destination = $"{fileServer.SubFolder}/{destination}";

                                        }
                                        var ftpResponse = await ftp.UploadStream(loadedFiless[i].OpenReadStream(maxAllowedSize: loadedFiless[i].Size * 1024), destination, FtpRemoteExists.Overwrite, true);

                                        clientOrderITem.FileType = loadedFiless[i].ContentType;
                                        clientOrderITem.FileSize = (byte)loadedFiless[i].Size;
                                        clientOrderITem.FileName = loadedFiless[i].Name;
                                        clientOrderITem.ObjectId = Guid.NewGuid().ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        string mdd = ex.Message;
                                    }
                                    clientOrderITem.InternalFileInputPath = $"{fileUploadVM.UploadDirectory}\\{clientOrderITem.FileName}" /*+_selectedFileFromJs[i].Path*/;

                                    await AddOrderItem(clientOrderITem);
                                    tempClientOrderItems.Add(clientOrderITem);
                                    order.orderItems.Add(clientOrderITem);
                                }

                                if (uploadCancelItem)
                                {
                                    var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                                    CurrentValueForEditOrderItemPregressbar = 0;
                                    uploadCancelItem = false;
                                    spinShow = false;
                                    cloaseAdditionalFileUploadModal();
                                    StateHasChanged();
                                    break;
                                }
                                if (CheckingClientOrderItem != null)
                                {
                                    CheckingClientOrderItem.File = file;
                                    DuplicateClientOrderItem.Add(CheckingClientOrderItem);
                                }
                                count++;
                                CurrentValueForEditOrderItemPregressbar = Math.Round((float)((100 / maxValue) * count), 2);
                                StateHasChanged();
                            }
                            catch
                            {

                            }
                        }
                    }

                    await ftp.Disconnect();
                }

                if (CurrentValueForEditOrderItemPregressbar == 100)
                {
                    CurrentValueForEditOrderItemPregressbar = 0;
                    isProgressBar = false;
                    StateHasChanged();
                }

                if (DuplicateClientOrderItem.Count > 0)
                {
                    var text = await CreateTextFileName(DuplicateClientOrderItem);

                    if (await js.ReplaceConfirmation($"The destination has {DuplicateClientOrderItem.Count} files with the same names. Do you want to replace the files?", $" \n{text}", SweetAlertTypeMessagee.question))
                    {

                        maxValue = DuplicateClientOrderItem.Count();
                        count = 0;
                        tempClientOrderItems = new List<ClientOrderItemModel>();
                        isProgressBar = true;
                        if (isProgressBar)
                        {
                            CurrentValueForEditOrderItemPregressbar = 0.1;
                        }
                        using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                        {
                            await ftp.AutoConnect();
                            foreach (var file in DuplicateClientOrderItem)
                            {
                                var clientOrderITem = new ClientOrderItemModel();

                                var ftpResult = await ftp.UploadStream(file.File.OpenReadStream(maxAllowedSize: 107374182400), $"{file.InternalFileInputPath}", FtpRemoteExists.Overwrite, true);

                                clientOrderITem.FileType = file.File.ContentType;
                                clientOrderITem.FileSize = (byte)file.File.Size;
                                clientOrderITem.FileName = file.File.Name;
                                clientOrderITem.ObjectId = file.ObjectId;
                                clientOrderITem.InternalFileInputPath = $"{file.InternalFileInputPath}".Replace("\\", "/");
                                clientOrderITem.PartialPath = file.PartialPath;
                                clientOrderITem.ClientOrderId = file.ClientOrderId;
                                clientOrderITem.CompanyId = file.CompanyId;
                                clientOrderITem.Id = file.Id;

                                //await AddOrderItem(clientOrderITem);
                                await _clientOrderItemService.UpdateItemFile(clientOrderITem);
                                tempClientOrderItems.Add(clientOrderITem);
                                order.orderItems.Add(clientOrderITem);

                                if (uploadCancelItem)
                                {
                                    var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                                    CurrentValueForEditOrderItemPregressbar = 0;
                                    uploadCancelItem = false;
                                    spinShow = false;
                                    cloaseAdditionalFileUploadModal();
                                    StateHasChanged();
                                    break;
                                }

                                count++;
                                CurrentValueForEditOrderItemPregressbar = Math.Round((float)((100 / maxValue) * count), 2);
                                StateHasChanged();
                            }
                            await ftp.Disconnect();
                        }
                        if (CurrentValueForEditOrderItemPregressbar == 100)
                        {
                            CurrentValueForEditOrderItemPregressbar = 0;
                            isProgressBar = false;
                            StateHasChanged();
                        }
                        isSubmitting = false;
                        isUploadInputDisabled = false;
                        this.StateHasChanged();
                    }
                }

                isSubmitting = false;
                isProgressBar = false;
                isUploadInputDisabled = false;
                clientOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId((int)order.Id);
                await js.DisplayMessage("Successfully Updated");
                cloaseAdditionalFileUploadModal();
                order.orderItems = new List<ClientOrderItemModel>();
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "LoadOrderFileFolder",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.LoadOrderFileFolderError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        private async Task<string> PathSplit(string path)
        {
            var returnPath = "";
            try
            {

                var spiltPath = path.Split("/");
                var takePath = spiltPath[0];
                var combinePath = $"/{order.OrderNumber}/{takePath}";
                await _dateTime.DateTimeConvert(order.CreatedDate);
                var fullpath = $"/{orderCompany.Code}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/Raw{combinePath}/";
                FileServerViewModel model = new FileServerViewModel()
                {
                    Host = fileServer.Host,
                    UserName = fileServer.UserName,
                    Password = fileServer.Password,
                    SubFolder = fileServer.SubFolder
                };
                var ftpResponse = await _fluentFtpService.FolderExists(model, fullpath);
                if (ftpResponse)
                {
                    returnPath = takePath;
                }

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "PathSplit",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.PathSplitError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return returnPath;
        }

        private void cloaseAdditionalFileUploadModal()
        {
            isOrderFileAddPopupVisible = false;
            StateHasChanged();
        }
        #endregion

        private async void SelectOrderItemNodesFromFolderStructure(FolderNodeModel folderNode, object checkedValue)
        {
            try
            {
                if ((bool)checkedValue)
                {
                    selectedOrderItemFromFolderStructure.Add(folderNode);
                }
                else
                {
                    selectedOrderItemFromFolderStructure.RemoveAll(f => f.OrderItemId == folderNode.OrderItemId);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SelectOrderItemNodesFromFolderStructure",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SelectOrderItemNodesFromFolderStructureError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

        private void SelectAllOrderItemNodesFromFolderStructure(object checkedValue)
        {
            if ((bool)checkedValue)
            {
                selectedOrderItemFromFolderStructure = new List<FolderNodeModel>();
                selectedOrderItemFromFolderStructure.AddRange(folderNodeFilesList.Where(f => f.IsFolder == false).ToList());
            }
            else
            {
                selectedOrderItemFromFolderStructure = new List<FolderNodeModel>();
            }
            StateHasChanged();
        }

        private void HandleChangePage(int p)
        {
            pageNumber = p;
            folderNodeFilesList = folderNodes.Where(f => f.IsFolder == false).ToList().Skip((p - 1) * pageSize).Take(pageSize).ToList();
            StateHasChanged();
        }
        private void PageSizeChange(ChangeEventArgs e)
        {

            pageSize = Int32.Parse(e.Value.ToString());
            if (pageSize > 200)
            {
                pageSize = folderNodes.Where(f => f.IsFolder == false).ToList().Count();
            }
            if (pageSize > 0)
            {
                numberOfPage = Convert.ToInt32(Math.Ceiling(folderNodes.Where(f => f.IsFolder == false).ToList().Count() / Convert.ToDecimal(pageSize)));
                folderNodeFilesList = folderNodes.Where(f => f.IsFolder == false).ToList().Skip(0).Take(pageSize).ToList();
                StateHasChanged();
            }
             //Todo:Rakib
        }

        private async Task ShowHideImgaeOnList()
        {
            await Task.Yield();
            showImageOnList = !showImageOnList;
            StateHasChanged();
        }

        private async Task LoadOrderItemFromFolderStructureView()
        {
            //selectedFiles = new List<ClientOrderItem>();
            if (!selectedFiles.Any())
            {
                if (selectedOrderItemFromFolderStructure != null && selectedOrderItemFromFolderStructure.Count > 0)
                {
                    //var selectedOrderItems = folderNodes.Where(f => f.IsSelected == true).ToList();//Node itedm
                    foreach (var item in selectedOrderItemFromFolderStructure)
                    {
                        var orderItem = clientOrderItems.FirstOrDefault(f => f.Id == item.OrderItemId);
                        if (orderItem != null)
                        {
                            selectedFiles.Add(orderItem);
                        }
                    }
                }
                else
                {
                    selectedFiles = await GetAssignNodeItems();
                }
            }
        }

        private async Task ShowSetOrderItemTypePopup()
        {
            await Task.Yield();
            isSetOrderItemTypePopupVisible = true;
            foreach (OrderItemFileGroup item in Enum.GetValues(typeof(OrderItemFileGroup)))
            {
                fileGroupCustomEnumList.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }
            if (fileViewMode == 2 || fileViewMode == 3)
            {
                await LoadOrderItemFromFolderStructureView();
            }
        }
        private async Task BackShowSetOrderItemTypePopup()
        {
            await Task.Yield();
            isBackSetOrderItemTypePopupVisible = true;
            foreach (OrderItemFileGroup item in Enum.GetValues(typeof(OrderItemFileGroup)))
            {
                fileGroupCustomEnumList.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }
            if (fileViewMode == 2 || fileViewMode == 3)
            {
                await LoadOrderItemFromFolderStructureView();
            }
        }

        private async Task MakeReadyToDeliverImageDelivered()
        {
            try
            {
                await Task.Yield();
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }

                if (selectedFiles == null || !selectedFiles.Any())
                {
                    await js.DisplayMessage("Please select as least one image");
                    return;
                }

                int totalselectedFileCount = selectedFiles.Count;

                List<ClientOrderItemModel> deliverAbleItems = new List<ClientOrderItemModel>();

                //update only workable file .
                deliverAbleItems = selectedFiles.ToList();

                deliverAbleItems.RemoveAll(orderItem => orderItem.FileGroup != (int)OrderItemFileGroup.Work ||
                                                 orderItem.Status != (int)InternalOrderItemStatus.ReadyToDeliver ||
                                                 orderItem.Status == null);

                if (selectedFiles == null || !selectedFiles.Any())
                {
                    await js.DisplayMessage("Those image you selected no one in capable to Delivered");
                    return;
                }

                foreach (var selectedFile in deliverAbleItems)
                {
                    if (selectedFile.Status != null)
                    {
                        await _updateOrderItemBLLService.UpdateOrderItemsStatus(deliverAbleItems.ToList(), InternalOrderItemStatus.Delivered, loginUser.ContactId);
                        await _orderStatusService.UpdateOrderStatus(order, loginUser.ContactId);
                    }
                }
                await js.DisplayMessage($"Sucessfully Delivered {deliverAbleItems.Count} out of {totalselectedFileCount}");
            }
            catch (Exception ex)
            {


            }

        }
        private async Task CloseSetOrderItemTypePopup()
        {
            fileGroupCustomEnumList = new List<CustomEnumTypes>();
            selectedFiles = null;
            await Task.Yield();
            isSetOrderItemTypePopupVisible = false;

        }
        private async Task CloseBackSetOrderItemTypePopup()
        {
            fileGroupCustomEnumList = new List<CustomEnumTypes>();
            selectedFiles = null;
            await Task.Yield();
            isBackSetOrderItemTypePopupVisible = false;

        }

        private async Task UpdateOrderItemFileType()
        {
            foreach (var selectedFile in selectedFiles)
            {
                selectedFile.FileGroup = selectedOrderItemGroup;
                await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));
            }

            await LoadOrderItemColorRefGroupData();
            await LoadOrderItemForLoginUser();
            await CloseSetOrderItemTypePopup();
        }
        private async Task BackUpdateOrderItemFileType()
        {
            foreach (var selectedFile in selectedColorRefFiles)
            {
                selectedFile.FileGroup = selectedOrderItemGroup;
                await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
                await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));
            }

            await LoadOrderItemColorRefGroupData();
            await LoadOrderItemForLoginUser();
            await CloseBackSetOrderItemTypePopup();
        }

        private async Task ShowStatusWiseOrderItems(int statusWiseViewMode)
        {
            orderItemStatusWiseViewMode = statusWiseViewMode;
            StateHasChanged();
            await ChangePreview(fileViewMode);
        }



        public string Text { get; set; }
        private async Task UpdateText(ChangeEventArgs e)
        {
            await LoadOrderItemForLoginUser();
            Text = e.Value.ToString();
            ClientOrderItemModel clientOrderItem = new ClientOrderItemModel();
            List<ClientOrderItemModel> clientOrderItemsList = new List<ClientOrderItemModel>();
            foreach (var item in clientOrderItems)
            {
                if (!string.IsNullOrEmpty(item.InternalFileInputPath))
                {
                    clientOrderItemsList.Add(item);
                }
            }
            if (!string.IsNullOrEmpty(Text))
            {
                Text = Text + "/";
                clientOrderItems = clientOrderItemsList.Where(x => x.InternalFileInputPath.Contains(Text)).ToList();
                Text = null;
                clientOrderItemsList = new List<ClientOrderItemModel>();
            }
            else
            {
                await LoadOrderItemForLoginUser();
            }

            StateHasChanged();
        }

        private async Task DownloadRejectedFile(bool canStatusChange)
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");

                ContactModel contactInfo = await _contactManager.GetById(loginUser.ContactId);

                spinShow = true;
                StateHasChanged();

                if (selectedFeedbackFiles != null)
                    isSubmitting = true;

                if (selectedFeedbackFiles != null && selectedFeedbackFiles.Any())
                {
                    //var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    var downloadPath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\{contactInfo.FirstName + " " + contactInfo.Id}";
                    // delete previous folder
                    await _ftpFilePathService.ExistsFolderDelete(downloadPath);


                    var dlpath = "";

                    var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                    var count = 0;
                    maxValue = selectedFeedbackFiles.Count;
                    isProgressBar = true;
                    if (isProgressBar)
                    {
                        spinShow = false;
                        progressBarCurrentValue = 0.1;
                        StateHasChanged();
                    }

                    FileUploadModel model = new FileUploadModel();

                    model.FtpUrl = serverInfo.Host;
                    model.userName = serverInfo.UserName;
                    model.password = serverInfo.Password;
                    model.SubFolder = serverInfo.SubFolder;
                    model.OrderNumber = order.OrderNumber;

                    model.Date = order.CreatedDate;
                    dlpath = $"{this._webHostEnvironment.WebRootPath}\\TempDownload\\";
                    var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    model.ContactName = contactInfo.FirstName.Trim() + " " + contactInfo.Id;


                    foreach (var orderItem in selectedFeedbackFiles)
                    {
                        try
                        {
                            using (var client = _ftpService.CreateAsyncFtpClient(model))
                            {
                                client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                client.Config.ValidateAnyCertificate = true;
                                await client.AutoConnect();

                                model.fileName = orderItem.FileName;
                                model.UploadDirectory = System.IO.Path.GetDirectoryName(orderItem.FeedBackImagePath);//$"{fileInfo.ExteranlFileInputPath}";
                                                                                                                     //model.DownloadFolderName = orderItem.PartialPath;
                                var dataSavePath = "";

                                if (string.IsNullOrEmpty(orderItem.PartialPath))
                                {
                                    dataSavePath = dlpath + $"\\{model.ContactName}\\{order.OrderNumber}";
                                }
                                else
                                {
                                    dataSavePath = dlpath + $"\\{model.ContactName}\\{orderItem.PartialPath}";
                                }

                                var localPath = $"{dataSavePath}/{model.fileName}";
                                var remotePath = $"{model.UploadDirectory}/{model.fileName}";

                                if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                {
                                    remotePath = $"{serverInfo.SubFolder}/{remotePath}";
                                }

                                var downloaderInfo = await _contactManager.GetById(loginUser.ContactId);

                                var downloadResponse = await client.DownloadFile(localPath, remotePath, FtpLocalExists.Overwrite);
                                spinShow = false;

                                count++;

                                progressBarCurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                                StateHasChanged();
                            }
                        }
                        catch (Exception ex)
                        {
                            await js.DisplayMessage(ex.InnerException.ToString());
                        }
                        finally
                        {
                            //semaphoreSlim.Release();
                        }

                    }

                    await _ftpService.CreateFolderDownloadTime(model);
                    if (progressBarCurrentValue == 100)
                    {
                        progressBarCurrentValue = 0;
                        spinShow = true;
                        StateHasChanged();
                    }

                    if (selectedFeedbackFiles.Count > 0)
                    {
                        var webHost = $"{this._webHostEnvironment.WebRootPath}";
                        await _downloadService.CreateZipAndDownload(contactInfo, order, webHost, null, null, null);
                        spinShow = true;
                    }


                    StateHasChanged();

                    isSubmitting = false;
                    spinShow = false;
                    selectedFeedbackFiles = new List<ClientOrderItemModel>();
                    StateHasChanged();
                    await js.DisplayMessage("Download Succesfully");


                }
                else
                {
                    await js.DisplayMessage("Select at least one file !");
                    isSubmitting = false;
                    spinShow = false;
                    StateHasChanged();
                    return;
                }
                //Update Status after editor download file
            }
            catch (Exception ex)
            {
                isProgressBar = false;
                spinShow = false;
                StateHasChanged();
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SingleDownloadEditor",
                    RazorPage = "OrderDetails.razor.cs",
                    Category = (int)ActivityLogCategory.SingleDownloadEditorError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        async Task CloseExpectedDeliveryDatePopup()
        {
            await Task.Yield();
            spinShow = true;
            isOrderItemExpectedDeliveryDatePopupVisible = false;
            this.StateHasChanged();
            spinShow = false;
            StateHasChanged();
        }

        async Task OrderItemExpectedDeliveryDatePopupVisible(ClientOrderItemModel item)
        {
            await Task.Yield();
            spinShow = true;
            deadLineUpdateOrderItem = new ClientOrderItemModel();
            deadLineUpdateOrderItem = clientOrderItems.Where(x => x.Id == item.Id).FirstOrDefault();
            isOrderItemExpectedDeliveryDatePopupVisible = true;
            this.StateHasChanged();
            spinShow = false;
            StateHasChanged();
        }

        private async Task UpdateExpectedDeliveryDate(ClientOrderItemModel clientOrderItem)
        {
            try
            {
                if (OrderImageExpectedDeliveryDate != null)
                {
                    var update = await _clientOrderItemService.UpdateOrderItemExpectedDeliveryDate(clientOrderItem.Id, OrderImageExpectedDeliveryDate);
                    await _clientOrderService.UpdateOrderDeadlineDate(clientOrderItem.ClientOrderId);

                    if (update.IsSuccess)
                    {
                        await LoadOrderItemForLoginUser();
                        StateHasChanged();
                        // await grid.Reload();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            //await LoadOrderItemForLoginUser();
            isOrderItemExpectedDeliveryDatePopupVisible = false;
            this.StateHasChanged();
        }



        #region Re-assign For QC and Files status changes
        private async Task ReAssignImage()
        {
            if (selectedFiles == null)
            {
                await js.DisplayMessage("Select at least One File");
                return;
            }
            if (selectedFiles.Count <= 0)
            {
                await js.DisplayMessage("Select at least One File");
                return;
            }

            else
            {
                var tempNotReAssignFiles = new List<ClientOrderItemModel>();
                if (authState.User.IsInRole(PermissionConstants.Order_item_reassign_for_qc))
                {
                    foreach (var orderItem in selectedFiles)
                    {
                        if (orderItem.Status == (byte)InternalOrderItemStatus.ReadyToDeliver || orderItem.Status == (byte)InternalOrderItemStatus.Delivered || orderItem.Status == (byte)InternalOrderItemStatus.Completed)
                        {
                            tempNotReAssignFiles.Add(orderItem);
                        }
                        else
                        {
                            orderItem.Status = (byte)InternalOrderItemStatus.Assigned;
                            orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.Assigned));
                            await _clientOrderItemService.UpdateClientOrderItemStatus(orderItem);
                            await AddOrderItemStatusChangeLog(orderItem, InternalOrderItemStatus.Assigned);
                            await _orderAssignedImageEditorService.Delete((int)orderItem.Id);
                        }
                    }
                    await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));
                }
                if (tempNotReAssignFiles.Count > 0 && tempNotReAssignFiles.Any())
                {
                    await js.DisplayMessage("Some files are selected Ready To Deliver,Delivered and Completed Statuses Files. So thats file are not reassign");
                    return;
                }
                MakeOrderItemUnselected();

                await LoadOrderItemForLoginUser();
            }
        }
        #endregion


        #region Added Auto Search

        private string employeeIdValue;

        private string EmployeeIdValue
        {
            get => loginUserTeamMembers.FirstOrDefault()?.EmployeeId ?? ""; // Return an empty string if teamMember or EmployeeId is null
            set
            {
                if (loginUserTeamMembers != null)
                {
                    loginUserTeamMembers.FirstOrDefault().EmployeeId = value;
                }
            }
        }

        #endregion

        #region Order Item Category Set
        private async Task ViewCategorySetUpPopup()
        {
            try
            {
                if (fileViewMode == 2 || fileViewMode == 3)
                {
                    await LoadOrderItemFromFolderStructureView();
                }

                clientOrderItem = new ClientOrderItemModel();


                //orderItemWiseCategory.OrderItemId = clientOrderItem.Id;
                commonCategories = await _clientCategoryService.GetByCompanyId(order.CompanyId);

                if (!commonCategories.Any())
                {
                    await js.DisplayMessage("No Category Available");
                }
                else
                {
                    isOrderItemCategorySetUpPopupVisible = true;
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {

            }
        }

        private async Task SetOrderItemCategory()
        {
            if (orderItemWiseCategory == null || orderItemWiseCategory.CategoryId <= 0)
            {
                await js.DisplayMessage("Select a valid category");
            }

            else
            {
                var category = commonCategories.Where(cc => cc.Id == orderItemWiseCategory.CategoryId).ToList().FirstOrDefault();


                foreach (var orderItem in selectedFiles)
                {
                    orderItemWiseCategory.CategoryPrice = category.ClientCategoryPrice;
                    orderItemWiseCategory.TimeInMinute = category.TimeInMinutes;
                    orderItemWiseCategory.CategorySetByContactId = _workContext.LoginUserInfo.ContactId;
                    orderItemWiseCategory.CategorySetDate = DateTime.Now;
                    orderItem.CategorySetStatus = (byte)ItemCategorySetStatus.Manual_set;
                    orderItem.CategoryApprovedByContactId = _workContext.LoginUserInfo.ContactId;
                    await _clientOrderItemService.UpdateOrderItemCategory(orderItem, orderItemWiseCategory);

                }
                await _orderStatusService.UpdateOrderCategorySetStatus(order);
                await LoadOrderItemForLoginUser();
                await CancelCategorySetUpPopup();
            }
        }

        private async Task CancelCategorySetUpPopup()
        {
            try
            {
                isOrderItemCategorySetUpPopupVisible = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}


