using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using FluentFTP;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.BLL.StatusChangeLog;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.OrderTeamServices;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.UploadFromQcPcAutomation
{
    public class UploadFromQcPcService : IUploadFromQcPcService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly IContactManager _contactManager;
        private readonly ICompanyManager _companyService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IFluentFtpService _fluentFtpService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        private readonly IOrderAssignedImageEditorService _orderAssignedImageEditorService;
        private readonly ICompanyTeamManager _companyTeamService;
        private readonly IStatusChangeLogBLLService _statusChangeLogBLLService;
       

        public static readonly string uploadedFilesContainerOnProductionPc = "_uploaded";
        public UploadFromQcPcService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            IContactManager contactService,
            ICompanyManager companyService,
            ICompanyGeneralSettingManager companyGeneralSettingService,
            IClientOrderItemService clientOrderItemService,
            IFluentFtpService fluentFtpService,
            IUpdateOrderItemBLLService updateOrderItemBLLService,
            IOrderAssignedImageEditorService orderAssignedImageEditorService,
            ICompanyTeamManager companyTeamService,
            IStatusChangeLogBLLService statusChangeLogBLLService
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
            _contactManager = contactService;
            _companyService = companyService;
            _companyGeneralSettingService = companyGeneralSettingService;
            _clientOrderItemService = clientOrderItemService;
            _fluentFtpService = fluentFtpService;
            _updateOrderItemBLLService = updateOrderItemBLLService;
            _orderAssignedImageEditorService = orderAssignedImageEditorService;
            _companyTeamService = companyTeamService;
            _statusChangeLogBLLService = statusChangeLogBLLService;
        }
        public async Task<Response<bool>> UploadImageFromQcPc(int consoleAppId)
        {
            try
            {
                var qcList = await _contactManager.GetAllIsSharedFolderQcContact(consoleAppId);
                Console.WriteLine("Scan Qc Pc For Getting QC Completers...........");

                List<ContactModel> qcOrderCompleters = await GetQcOrderCompleters(qcList);

                if (qcOrderCompleters.Any())
                {
                    Console.WriteLine("Start Process Completed Files...........");
                    await IterateQcOrderCompleters(qcOrderCompleters);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)consoleAppId,
                    PrimaryId = consoleAppId,
                    ErrorMessage = $"QC App No: {consoleAppId}.Exception: {ex.Message}",
                    MethodName = "UploadImageFromQcPc",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            return new Response<bool>();
        }
        
        #region private method
        private static CommonActivityLogViewModel CommonActivityLogViewModelPrepare(Exception ex)
        {
            CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
            {
                //PrimaryId = (int)order.Id,
                ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
                loginUser = new LoginUserInfoViewModel{ ContactId = AutomatedAppConstant.ContactId},
                ErrorMessage = ex.Message,
                MethodName = "UploadImageFromQcPcToFtp",
                RazorPage = "FtpOrderProcessService - VC - Console Application",
                Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
            };
            return activity;
        }

        private async Task IterateQcOrderCompleters(List<ContactModel> qcBatchCompleters)
        {
            var semaphore = new SemaphoreSlim(1);
            var qcPcScanTasks = new List<Task>();

            foreach (var qc in qcBatchCompleters)
            {
                Console.WriteLine(qc.DownloadFolderPath);
                Console.WriteLine(qc.FirstName + qc.LastName);
                await semaphore.WaitAsync();
                qcPcScanTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        List<string> qcCompletedOrdersPath = await GetQcCompletedOrdersPath(qc);

                        if(qcCompletedOrdersPath.Any())
                        {
                            await IterateQcCompletedOrdersPath(qc, qcCompletedOrdersPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (byte)qc.Id,
                            PrimaryId = (byte)qc.Id,
                            ErrorMessage = $"QC Completed File Path: {qc.DownloadFolderPath}.Exception: {ex.Message}",
                            MethodName = "IterateQcOrderCompleters",
                            RazorPage = "UploadFromQcPcService",
                            Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                        };
                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
            // Wait for all tasks to complete
            await Task.WhenAll(qcPcScanTasks);
        }

        private async Task IterateQcCompletedOrdersPath(ContactModel qc, List<string> qcCompletedOrders)
        {
            foreach (string orderPath in qcCompletedOrders)
            {
                try
                {
                    ClientOrderModel orderInfo = await GetOrderInfoFromOrderPath(orderPath);
                    if (string.IsNullOrEmpty(orderInfo.OrderNumber) || (orderInfo.Id <= 0)) continue;

                    List<string> uploadedFilesPath = await GetUploadedOrderAllFilePath(orderPath);
                    uploadedFilesPath = FilterFilesByUploadTimeWindow(uploadedFilesPath);

                    if (uploadedFilesPath.Any())
                    {
                        await ProcessQcUploadOrderFiles(qc, uploadedFilesPath, orderInfo);

                        await _orderStatusService.UpdateOrderStatus(orderInfo, AutomatedAppConstant.ContactId);
                    }
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        ActivityLogFor = (byte)qc.Id,
                        PrimaryId = (byte)qc.Id,
                        ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath}.Exception: {ex.Message}",
                        MethodName = "GetQcCompletedOrdersPath",
                        RazorPage = "UploadFromQcPcService",
                        Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                    };
                    await _activityAppLogService.InsertAppErrorActivityLog(activity);
                }
            }
        }

        /// <summary>
        /// Filters the uploaded files by checking if all files were uploaded between the last 2 and 600 minutes.
        /// Files outside this range or with the name "Thumbs.db" are excluded from the list.
        /// </summary>
        /// <param name="uploadedFilesPath">List of file paths for uploaded files.</param>
        /// <returns>A filtered list of file paths.</returns>
        private static List<string> FilterFilesByUploadTimeWindow(List<string> uploadedFilesPath)
        {
            var unwantedOrderItem = new List<string> { "Thumbs.db", ".BridgeSort", ".BridgeLabelsAndRatings" };

            uploadedFilesPath = uploadedFilesPath
                .Where(file => !unwantedOrderItem.Exists(unwantedItem => file.Contains(unwantedItem)))
                .ToList();
            return uploadedFilesPath;
        }


        private async Task<ClientOrderModel> GetOrderInfoFromOrderPath(string orderPath)
        {
            string orderNumber = Path.GetFileName(orderPath);
            if (string.IsNullOrEmpty(orderNumber)) return null;

            return await GetCompletedOrderInfo(orderNumber).ConfigureAwait(false);
        }
        #region Changes Zakir

        private async Task ProcessQcUploadOrderFiles(ContactModel qc, List<string> uploadedFilesPath, ClientOrderModel orderInfo)
        {
            try
            {
                var orderCompanyInfo = await _companyService.GetById(orderInfo.CompanyId).ConfigureAwait(false);
                var serverInfo = await _fileServerService.GetById((int)orderInfo.FileServerId).ConfigureAwait(false);
                var isAllowExtraOutputFileUpload = orderInfo.AllowExtraOutputFileUpload;
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllGeneralSettingsByCompanyId(orderCompanyInfo.Id).ConfigureAwait(false);

                DateTimeConfiguration _dateTime = new DateTimeConfiguration();
                await _dateTime.DateTimeConvert(orderInfo.CreatedDate).ConfigureAwait(false);

                var preparedOrderItems = await PrepareUploadedFileModels(qc, orderInfo, orderCompanyInfo, uploadedFilesPath, _dateTime).ConfigureAwait(false);

                if (preparedOrderItems != null && preparedOrderItems.Any()) 
                {
                    var determinedOrderItems = await DetermineNewUploadsOrFileExists(qc, orderInfo, isAllowExtraOutputFileUpload, companyGeneralSettings, preparedOrderItems).ConfigureAwait(false);

                    if (determinedOrderItems != null && determinedOrderItems.Any())
                    {
                        determinedOrderItems = determinedOrderItems.Where(item => item != null).ToArray();
                        await UploadFiles(qc, orderInfo, serverInfo, determinedOrderItems, _dateTime).ConfigureAwait(false);
                        await DeleteQcCompletedOrderFolder(qc, orderInfo).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath},Order Number: {orderInfo.OrderNumber}.Exception: {ex.Message}",
                    MethodName = "ProcessQcUploadOrderFiles",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity).ConfigureAwait(false);
            }
        }

        private async Task<ClientOrderItemModel[]> PrepareUploadedFileModels(ContactModel qc, ClientOrderModel orderInfo, CompanyModel orderCompanyInfo, List<string> uploadedFilesPath, DateTimeConfiguration _dateTime)
        {
            try
            {
                var prepareTasks = new List<Task<ClientOrderItemModel>>();
                var semaphore = new SemaphoreSlim(1);

                foreach (string filePath in uploadedFilesPath)
                {
                    await semaphore.WaitAsync();

                    prepareTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            return await PrepareUploadedFileModel(qc, orderInfo, orderCompanyInfo, filePath, _dateTime);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                return await Task.WhenAll(prepareTasks);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath},Order Number: {orderInfo.OrderNumber}.Exception: {ex.Message}",
                    MethodName = "PrepareUploadedFileModels",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                return new ClientOrderItemModel[1];
            }
        }

        private async Task<ClientOrderItemModel[]> DetermineNewUploadsOrFileExists(ContactModel qc, ClientOrderModel orderInfo, bool isAllowExtraOutputFileUpload, CompanyGeneralSettingModel companyGeneralSettings, ClientOrderItemModel[] preparedOrderItems)
        {
            try
            {
                var determineTasks = new List<Task<ClientOrderItemModel>>();
                var semaphoreSlim = new SemaphoreSlim(1);
                foreach (var qcUploadedOrderItem in preparedOrderItems)
                {
                    await semaphoreSlim.WaitAsync();
                    determineTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            return await DetermineNewUploadOrFileExist(qc, orderInfo, isAllowExtraOutputFileUpload, companyGeneralSettings, qcUploadedOrderItem);
                        }
                        finally
                        {
                            semaphoreSlim.Release();
                        }
                    }));
                }
                return await Task.WhenAll(determineTasks);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath},Order Number: {orderInfo.OrderNumber}.Exception: {ex.Message}",
                    MethodName = "DetermineNewUploadsOrFileExists",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity).ConfigureAwait(false);

                return new ClientOrderItemModel[1];
            }

            
        }

        private async Task UploadFiles(ContactModel qc, ClientOrderModel orderInfo, FileServerModel serverInfo, ClientOrderItemModel[] determinedOrderItems, DateTimeConfiguration _dateTime)
        {
            try
            { 
                SemaphoreSlim qcImagePathSemaphoreSlim = new SemaphoreSlim(15);
                var uploadTasks = new List<Task>();
                foreach (var qcUploadedOrderItem in determinedOrderItems)
                {
                    await qcImagePathSemaphoreSlim.WaitAsync();
                    uploadTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await PreapareAndQcFileUpload(qc, orderInfo, serverInfo, qcUploadedOrderItem.QcCompletedFilePath, qcUploadedOrderItem, _dateTime);
                        }
                        finally
                        {
                            qcImagePathSemaphoreSlim.Release();
                        }
                    }));
                }
                await Task.WhenAll(uploadTasks);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath},Order Number: {orderInfo.OrderNumber}.Exception: {ex.Message}",
                    MethodName = "UploadFiles",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcFileUploadingOnKTMStorageTimeError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity).ConfigureAwait(false);
            }
        }
        private async Task DeleteQcCompletedOrderFolder(ContactModel qc, ClientOrderModel orderInfo)
        {
            try
            {
                int i = 0;
                while (true)
                {
                    try
                    {
                        string sourcePath = qc.QcPcCompletedFilePath + orderInfo.OrderNumber;

                        if (Directory.Exists(sourcePath) && !Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories).Any())
                        {
                            Directory.Delete(sourcePath, true);
                        }
                        break;
                    }
                    catch
                    {

                        i++;
                        await Task.Delay(1000);
                        if (i > 3)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath},Order Number: {orderInfo.OrderNumber}.Exception: {ex.Message}",
                    MethodName = "DeleteQcCompletedOrderFolder",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity).ConfigureAwait(false);
            }
        }

        private async Task<bool> PreapareAndQcFileUpload1(ContactModel qc, ClientOrderModel orderInfo, 
            FileServerModel serverInfo, SemaphoreSlim qcImagePathSemaphoreSlim, List<Task> qcImagePathTasks, 
            string filePath,ClientOrderItemModel qcUploadedOrderItem, DateTimeConfiguration _dateTime)
        {
            await qcImagePathSemaphoreSlim.WaitAsync();
            qcImagePathTasks.Add(Task.Run(async () =>
            {
                bool isSuccessfullyUpload = false;
                try
                {
                    if (qcUploadedOrderItem != null  && (qcUploadedOrderItem.Status == (byte)InternalOrderItemStatus.InQc ||
                                                         qcUploadedOrderItem.Status == (byte)InternalOrderItemStatus.ReworkQc || 
                                                         qcUploadedOrderItem.Status == (byte)InternalOrderItemStatus.ReadyToDeliver
                                                         )
                       )
                    {
                        isSuccessfullyUpload = await QcFileUploadAndMove(qc, orderInfo, serverInfo,filePath, _dateTime,qcUploadedOrderItem);
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    qcImagePathSemaphoreSlim.Release();
                }

                return isSuccessfullyUpload;
            }));
            return false;
        }

        private async Task<bool> PreapareAndQcFileUpload(ContactModel qc, ClientOrderModel orderInfo,FileServerModel serverInfo,
            string filePath, ClientOrderItemModel qcUploadedOrderItem, DateTimeConfiguration _dateTime)
        {
             bool isSuccessfullyUpload = false;
             try
                {
                    // Proceed with file upload if it's not already uploaded
                    if (qcUploadedOrderItem != null && (qcUploadedOrderItem.Status == (byte)InternalOrderItemStatus.InQc ||
                                                        qcUploadedOrderItem.Status == (byte)InternalOrderItemStatus.ReworkQc ||
                                                        qcUploadedOrderItem.Status == (byte)InternalOrderItemStatus.ReadyToDeliver))
                    {
                        isSuccessfullyUpload = await QcFileUploadAndMove(qc, orderInfo, serverInfo, filePath, _dateTime, qcUploadedOrderItem);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during upload: {ex.Message}");
                }
                

             return isSuccessfullyUpload;
           
        }

        #endregion Changes Zakir

        private async Task<ClientOrderItemModel> DetermineNewUploadOrFileExist(ContactModel qc, ClientOrderModel orderInfo, bool isAllowExtraOutputFileUpload, CompanyGeneralSettingModel companyGeneralSettings, ClientOrderItemModel qcUploadedOrderItem)
        {
            try
            {
                var qcFileUploadPath = qcUploadedOrderItem.InternalFileOutputPath;
                var qcFilePartialPath = qcUploadedOrderItem.PartialPath;
                var qcPCUploadCompletedPath = qcUploadedOrderItem.QcCompletedFilePath;

                if (isAllowExtraOutputFileUpload)
                {
                    var checkedOrderItem = await GetQcUploadedOrderItemByIsSameNameExistOnSameFolder(qcUploadedOrderItem, orderInfo, companyGeneralSettings);

                    if (checkedOrderItem == null || checkedOrderItem.Id == 0)
                    {
                        qcUploadedOrderItem = await HandleNewOrderItem(qcUploadedOrderItem, orderInfo, qc);
                    }
                    else
                    {
                        qcUploadedOrderItem = checkedOrderItem;
                    }

                    qcUploadedOrderItem.InternalFileOutputPath = qcFileUploadPath;
                    qcUploadedOrderItem.PartialPath = qcFilePartialPath;
                    qcUploadedOrderItem.QcCompletedFilePath = qcPCUploadCompletedPath;
                }
                else
                {
                    qcUploadedOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPathWithWorkFileGroup(qcUploadedOrderItem);

                    if (qcUploadedOrderItem != null)
                    {
                        qcUploadedOrderItem.InternalFileOutputPath = qcFileUploadPath;
                        qcUploadedOrderItem.PartialPath = qcFilePartialPath;
                        qcUploadedOrderItem.QcCompletedFilePath = qcPCUploadCompletedPath;
                    }
                }
                return qcUploadedOrderItem;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath},Order Number: {orderInfo.OrderNumber}.Exception: {ex.Message}",
                    MethodName = "DetermineNewUploadOrFileExist",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity).ConfigureAwait(false);

                return new ClientOrderItemModel();
            }

          
        }

        private async Task<ClientOrderItemModel> PrepareUploadedFileModel(ContactModel qc, ClientOrderModel orderInfo, CompanyModel orderCompanyInfo, string filePath,DateTimeConfiguration _dateTime)
        {
            var uploadFolderPath = "/" + Path.GetDirectoryName(filePath.Split(qc.QcPcCompletedFilePath)[1]).Replace("\\", "/");
            string qcCompletedFileUploadRootPathOnKTMStorage = $"{orderCompanyInfo.Code}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/Completed";
            string qcFileUploadFullPath = qcCompletedFileUploadRootPathOnKTMStorage + uploadFolderPath+"/"+ Path.GetFileName(filePath);
            
            var qcUploadedOrderItem = new ClientOrderItemModel()
            {
                PartialPath = uploadFolderPath,
                ClientOrderId = orderInfo.Id,
                FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath),
                CompanyId = orderInfo.CompanyId,
                FileName = Path.GetFileName(filePath),
                FileType = Path.GetExtension(filePath),
                FileSize = System.IO.File.ReadAllBytes(filePath).Length, //Dummy
                IsExtraOutPutFile = true,
                InternalFileOutputPath = qcFileUploadFullPath.Replace("//", "/"),
                QcCompletedFilePath = filePath
            };

            return qcUploadedOrderItem;
        }

       

        private async Task<ClientOrderItemModel> GetQcUploadedOrderItemByIsSameNameExistOnSameFolder(ClientOrderItemModel orderItemFile, ClientOrderModel orderInfo, CompanyGeneralSettingModel companyGeneralSettings)
        {
            if (!companyGeneralSettings.IsSameNameImageExistOnSameFolder)
            {
                return await _clientOrderItemService.GetByFileByOrderIdAndFullFileNameAndPath(orderItemFile);
            }
            return null;
        }

        private async Task<ClientOrderItemModel> HandleNewOrderItem(ClientOrderItemModel orderItemFile, ClientOrderModel orderInfo,ContactModel qc)
        {
            var addItemResponse = await AddOrderItem(orderItemFile, orderInfo.CompanyId, orderInfo.Id, InternalOrderItemStatus.ReadyToDeliver);
            if (addItemResponse.Result > 0)
            {
                await ExtraImageFileAddWithAssignOwnUploader(orderInfo, addItemResponse.Result, qc);
                return await _clientOrderItemService.GetById(addItemResponse.Result);
            }
            return null;
        }
        private async Task<ClientOrderItemModel> HandleExistingOrderItem(List<ClientOrderItemModel> filesExistList, ClientOrderItemModel qcUploadOrderItem, ClientOrderModel orderInfo)
        {
            if (filesExistList.Count == 1)
            {
                var filesExistListOrderItem = filesExistList.First();

                if (IsReadyToDeliverInDifferentFolder(filesExistListOrderItem,qcUploadOrderItem,orderInfo))
                {
                    var addItemResponse = await AddOrderItem(qcUploadOrderItem, orderInfo.CompanyId, orderInfo.Id, InternalOrderItemStatus.ReadyToDeliver);
                    if (addItemResponse.Result > 0)
                    {
                        return await _clientOrderItemService.GetById(addItemResponse.Result);
                    }
                }
                return filesExistListOrderItem;
            }
            else
            {
                return await HandleMultipleExistingItems(filesExistList, qcUploadOrderItem, orderInfo);
            }
        }

        private bool IsReadyToDeliverInDifferentFolder(ClientOrderItemModel filesExistListOrderItem, ClientOrderItemModel qcUploadedOrderItem,ClientOrderModel orderInfo)
        {
            if (filesExistListOrderItem.InternalFileOutputPath != null)
            {
                //var existingFileFolderPath = GetExistingFileFolderPath(filesExistListOrderItem.InternalFileOutputPath, orderInfo.OrderNumber);
                return filesExistListOrderItem.InternalFileOutputPath != qcUploadedOrderItem.InternalFileOutputPath;
            }
            return false;
        }
        private async Task<ClientOrderItemModel> HandleMultipleExistingItems(List<ClientOrderItemModel> filesExistList, ClientOrderItemModel orderItemFile, ClientOrderModel orderInfo)
        {
            //var temporyOutPutFolderPathList = filesExistList
            //.Where(item => item.InternalFileOutputPath != null)
            //    .Select(item => GetExistingFileFolderPath(item.InternalFileOutputPath, orderInfo.OrderNumber))
            //    .ToList();

            var allExistingOrderItemPartialPaths = filesExistList
                .Where(item => item.PartialPath != null)
                .Select(item =>item.PartialPath)
                .ToList();

            if (allExistingOrderItemPartialPaths == null || !allExistingOrderItemPartialPaths.Exists(path => path == orderItemFile.PartialPath))
            {
                var addItemResponse = await AddOrderItem(orderItemFile, orderInfo.CompanyId, orderInfo.Id, InternalOrderItemStatus.ReadyToDeliver);
                if (addItemResponse.Result > 0)
                {
                    return await _clientOrderItemService.GetById(addItemResponse.Result);
                }
            }
            else
            {
                var indexOfOutPutFolderMatchingPath = allExistingOrderItemPartialPaths.IndexOf(orderItemFile.PartialPath);
                return filesExistList[indexOfOutPutFolderMatchingPath];
            }

            return orderItemFile;
        }

        private string GetExistingFileFolderPath(string internalFileOutputPath, string orderNumber)
        {
            var lastIndexOfOrderNumber = internalFileOutputPath.LastIndexOf(orderNumber);
            var existingFileFolderPath = Path.GetDirectoryName("/" + internalFileOutputPath.Substring(lastIndexOfOrderNumber));
            return existingFileFolderPath.Replace("\\", "/");
        }

        private async Task<bool> QcFileUploadAndMove(ContactModel qc, ClientOrderModel orderInfo, FileServerModel serverInfo,
            string filePath, DateTimeConfiguration _dateTime,ClientOrderItemModel orderItemFile)
        {
            var fileServerViewModel = new FileServerViewModel()
            {
                Host = serverInfo.Host,
                UserName = serverInfo.UserName,
                Password = serverInfo.Password,
            };

            bool isSuccessfullyUpload = false;

            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
            {
                ftp.Config.EncryptionMode = FluentFTP.FtpEncryptionMode.Auto;
                ftp.Config.ValidateAnyCertificate = true;

                await ftp.AutoConnect();
                await _dateTime.DateTimeConvert(orderInfo.CreatedDate);


                FtpStatus response = await UploadFileFromQcPcToKTMStorage(serverInfo,filePath,ftp, orderItemFile);

                await ftp.Disconnect();
                if (response.Equals(FtpStatus.Success))
                {
                    Console.WriteLine("Response: " + response + Path.GetFileName(filePath));

                    await UpdateQcUploadItemStatusAndOutputFilePath(orderItemFile);

                    isSuccessfullyUpload = true;

                    Console.WriteLine("File Name : " + orderItemFile.FileName);
                    int i = 0;
                    while (true)
                    {
                        try
                        {
                            var splitedFilePath = filePath.Split(orderInfo.OrderNumber)[1].Replace("\\\\", "\\");
                            string sourcePath = qc.QcPcCompletedFilePath + orderInfo.OrderNumber + splitedFilePath.Replace("\\\\", "\\");
                            string destinationPath = qc.QcPcCompletedFilePath + $"{uploadedFilesContainerOnProductionPc}/" + orderInfo.OrderNumber + Path.GetDirectoryName(splitedFilePath);

                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);
                            }
                            if (!System.IO.File.Exists(destinationPath + orderItemFile.FileName))
                            {
                                var destinationFilePath = $"{destinationPath}//{orderItemFile.FileName}";
                                try
                                {
                                    Console.WriteLine("File Move : " + orderItemFile.FileName);

                                    if (File.Exists(destinationFilePath))
                                    {
                                        File.Delete(destinationFilePath);
                                    }

                                    System.IO.File.Move(sourcePath, destinationFilePath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                            break;
                        }
                        catch
                        {

                            i++;
                            await Task.Delay(1000);
                            if (i > 3)
                            {
                                break;
                            }
                        }
                    }

                }
                else if (response.Equals(FtpStatus.Failed))
                {

                }
            }
            return isSuccessfullyUpload;
        }

        private async Task UpdateQcUploadItemStatusAndOutputFilePath(ClientOrderItemModel orderItemFile)
        {
            await _updateOrderItemBLLService.UpdateOrderItemStatus(orderItemFile, InternalOrderItemStatus.ReadyToDeliver);
            await _clientOrderItemService.UpdateItemByQC(orderItemFile);
        }

        private async Task<ClientOrderModel> GetCompletedOrderInfo(string orderNumber)
        {
           return await _orderService.GetByOrderNumber(orderNumber);
        }
        private async Task<List<string>> GetUploadedOrderAllFilePath(string dir)
        {
            await Task.Yield();
            List<string> filePaths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories).ToList();

            return filePaths;
        }

        private async Task<List<string>> GetQcCompletedOrdersPath(ContactModel qc)
        {
            try
            {
                qc.QcPcCompletedFilePath = qc.DownloadFolderPath + "\\" + qc.FirstName.Trim() + " " + qc.Id + " " + "(QC)" + "\\" + "Completed\\";
                List<string> completedOrdersPath = Directory.GetDirectories(qc.QcPcCompletedFilePath).ToList();

                if (completedOrdersPath.Any(dir => dir.Contains(uploadedFilesContainerOnProductionPc)))
                {
                    completedOrdersPath.RemoveAll(dir => dir.Contains(uploadedFilesContainerOnProductionPc));
                }

                return completedOrdersPath;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (byte)qc.Id,
                    PrimaryId = (byte)qc.Id,
                    ErrorMessage = $"Completed Order Path: {qc.QcPcCompletedFilePath}.Exception: {ex.Message}",
                    MethodName = "GetQcCompletedOrdersPath",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                return new List<string>();
            }
           
        }

        private async Task<List<ContactModel>> GetQcOrderCompleters(List<ContactModel> qcList)
        {
            try
            {

                return qcList.Where(qc =>
                     qc.IsSharedFolderEnable &&
                     Directory.Exists(Path.Combine(qc.DownloadFolderPath, $"{qc.FirstName.Trim()} {qc.Id} (QC)", "Completed")) &&
                     Directory.GetDirectories(Path.Combine(qc.DownloadFolderPath, $"{qc.FirstName.Trim()} {qc.Id} (QC)", "Completed"))
                         .Any(dir => !dir.Contains(uploadedFilesContainerOnProductionPc)))
                         
                     .ToList();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = 0,
                    PrimaryId = 0,
                    ErrorMessage = $"QC App.Exception: {ex.Message}",
                    MethodName = "GetQcOrderCompleters",
                    RazorPage = "UploadFromQcPcService",
                    Category = (int)ActivityLogCategory.QcUploadCompletedFileError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                return new List<ContactModel>();
            }
        }

        private static double GetUploadedFileLatModifiedTime(List<string> filePaths)
        {
            double uploadableFileLastModifiedTime = 0;

            if (filePaths.Count > 0)
            {
                // Use LINQ to find the file with the minimum last modified time
                var minModifiedFile = filePaths
                   .Select(f => new FileInfo(f))
                   .OrderBy(f => f.LastAccessTime)
                   .First()
                   .LastAccessTime;
                uploadableFileLastModifiedTime = DateTime.Now.Subtract(minModifiedFile).TotalMinutes;
            }
            return uploadableFileLastModifiedTime;
        }

        private static async Task<FtpStatus> UploadFileFromQcPcToKTMStorage(FileServerModel serverInfo,
            string filePath, AsyncFtpClient ftp,ClientOrderItemModel qcUploadOrderItem)
        {
            FtpStatus response = FtpStatus.Failed;
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                int retryCount = 0;
                const int maxRetries = 3;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                        {
                            response = await ftp.UploadStream(fileStream, $"{serverInfo.SubFolder}\\{qcUploadOrderItem.InternalFileOutputPath}", FtpRemoteExists.Overwrite, true);
                            Console.WriteLine("File Path By QC : " + filePath);
                        }
                        else
                        {
                            response = await ftp.UploadStream(fileStream, $"{qcUploadOrderItem.InternalFileOutputPath}", FtpRemoteExists.Overwrite, true);
                        }
                       
                        if (response == FtpStatus.Success)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Attempt {retryCount + 1} failed with exception: {ex.Message}");
                    }
                    retryCount++;
                }
            }
            return response;
        }

#endregion

        #region Method which exist on FtpOrderProcessService
        private async Task<Response<long>> AddOrderItem(ClientOrderItemModel clientOrderItem, int companyId, long orderId, InternalOrderItemStatus status = 0)
        {
            Response<long> addItemResponse = null;
            try
            {
                clientOrderItem.IsDeleted = false;
                clientOrderItem.ObjectId = Guid.NewGuid().ToString();
                clientOrderItem.CreatedByContactId = AutomatedAppConstant.ContactId; //Dummy
                clientOrderItem.FileGroup = (int)OrderItemFileGroup.Work;

                //Set status
                if (status > 0)
                {
                    clientOrderItem.Status = (byte)status;
                    clientOrderItem.ExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(status);
                }

                var companyTeam = await _companyTeamService.GetByCompanyId(companyId);
                if (companyTeam != null)
                {
                    var getFirstOrDefaultCompany = companyTeam.FirstOrDefault();

                    if (clientOrderItem.TeamId != null && clientOrderItem.TeamId > 0)
                    {
                        clientOrderItem.TeamId = getFirstOrDefaultCompany.TeamId;
                    }

                }

                //Add Order Item / Files in database 
                addItemResponse = await _clientOrderItemService.Insert(clientOrderItem, orderId);
                Console.WriteLine(clientOrderItem.FileName + " " + addItemResponse.Message.ToString());
                if (addItemResponse.IsSuccess)
                {
                    clientOrderItem.Id = addItemResponse.Result;
                    //order.orderItems.Add(clientOrderItem);
                    if (status > 0)
                    {
                        await _statusChangeLogBLLService.AddOrderItemStatusChangeLog(clientOrderItem, status, AutomatedAppConstant.ContactId);
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
                    MethodName = "AddOrderInfo",
                    RazorPage = "FtpOrderProcessService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return addItemResponse;
        }

        private async Task<bool> ExtraImageFileAddWithAssignOwnUploader(ClientOrderModel clientOrder, long clientOrderItemId, ContactModel contact)
        {
            OrderAssignedImageEditorModel assignedImage = new OrderAssignedImageEditorModel
            {
                OrderId = clientOrder.Id,
                AssignByContactId = AutomatedAppConstant.ContactId,
                AssignContactId = contact.Id,
                Order_ImageId = clientOrderItemId,
                ObjectId = Guid.NewGuid().ToString(),
                UpdatedByContactId = AutomatedAppConstant.ContactId
            };
            List<OrderAssignedImageEditorModel> assignedImages = new List<OrderAssignedImageEditorModel>();
            assignedImages.Add(assignedImage);//Todo Rakib
            var addResponse = await _orderAssignedImageEditorService.Insert(assignedImages);
            return addResponse.IsSuccess;
        }

        #endregion
    }
}
