using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Security;
using FluentFTP;
using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using CutOutWiz.Services.ErrorLogServices;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.Models.ClientOrders;
using System.Transactions;
using Renci.SshNet.Sftp;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.BLL.OrderAttachment;
using CutOutWiz.Services.PathReplacementServices;
using CutOutWiz.Services.AutomationAppServices.FtpOrderProcess;
using CutOutWiz.Services.EmailMessage;
using CutOutWiz.Services.BLL.StatusChangeLog;

using CutOutWiz.Services.AutomationAppServices.OrderItemCategorySetByAutomation;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.SftpServices;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.Models.PathReplacements;
using System.Diagnostics;
using CutOutWiz.Services.ClientCommonCategoryService.ClientCategorys;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.OrderPlaceAutomation
{
    public class OrderPlaceService : IOrderPlaceService
    {

        #region Constructor
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly IErrorLogService _errorLogService;
        private readonly IFluentFtpService _fluentFtpService;
        private readonly ICompanyManager _companyService;
        private readonly ISshNetService _sshNetService;
        private readonly IConfiguration _configuration;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IFtpFilePathService _ftpFilePathService;
        private readonly IOrderAttachmentBLLService _orderAttachmentBLLService;
        private readonly IPathReplacementService _pathReplacementService;
        private readonly IClientExternalOrderFTPSetupService _clientExternalOrderFTPSetupService;
        private readonly IWorkflowEmailService _workflowEmailService;
        private readonly ICompanyTeamManager _companyTeamService;
        private readonly IStatusChangeLogBLLService _statusChangeLogBLLService;
        private readonly ICategorySetService _categorySetService;
        private readonly IClientCategoryService _clientCategoryService;
        private readonly ISftpService _sftpService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        private readonly IContactManager _contactManager;

        private const string GeneralSettingsKey = "GeneralSettings";
        public OrderPlaceService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            ICompanyGeneralSettingManager companyGeneralSettingService,
            IErrorLogService errorLogService,
            IFluentFtpService fluentFtpService,
            ICompanyManager companyService,
            ISshNetService sshNetService,
            IConfiguration configuration,
            IClientOrderItemService clientOrderItemService,
            IFtpFilePathService ftpFilePathService,
            IOrderAttachmentBLLService orderAttachmentBLLService,
            IPathReplacementService pathReplacementService,
            IClientExternalOrderFTPSetupService clientExternalOrderFTPSetupService,
            IWorkflowEmailService workflowEmailService,
            ICompanyTeamManager companyTeamService,
            IStatusChangeLogBLLService statusChangeLogBLLService,
            ICategorySetService categorySetService,
            IClientCategoryService clientCategoryService,
            ISftpService sftpService,
            IUpdateOrderItemBLLService updateOrderItemBLLService,
            IContactManager contactService
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
            _companyGeneralSettingService = companyGeneralSettingService;
            _errorLogService = errorLogService;
            _fluentFtpService = fluentFtpService;
            _companyService = companyService;
            _sshNetService = sshNetService;
            _ftpFilePathService = ftpFilePathService;
            _orderAttachmentBLLService = orderAttachmentBLLService;
            _pathReplacementService = pathReplacementService;
            _clientExternalOrderFTPSetupService = clientExternalOrderFTPSetupService;
            _workflowEmailService = workflowEmailService;
            _companyService = companyService;
            _companyTeamService = companyTeamService;
            _statusChangeLogBLLService = statusChangeLogBLLService;
            _clientOrderItemService = clientOrderItemService;

            _categorySetService = categorySetService;
            _clientCategoryService = clientCategoryService;

            _configuration = configuration;
            _sftpService = sftpService;
            _updateOrderItemBLLService = updateOrderItemBLLService;
            _contactManager = contactService;

        }
        #endregion
        #region Public Method
        public async Task<Response<bool>> PlaceNewOrderFromClientStorage(int consoleAppId, bool isInternalStorageOrderPlace)
        {
            var response = new Response<bool>();

            try
            {
                List<CompanyGeneralSettingModel> companyGeneralSettings = await GetAutoOrderPlaceEnableCompanyGeneralSettings(consoleAppId);
                companyGeneralSettings = companyGeneralSettings.Where(generalSettings => generalSettings.CompanyId > 0).ToList();
                if (!companyGeneralSettings.Any()) return response;

                SemaphoreSlim clientStorageScanThread = GetOrderPlaceFtpsThread();
                var clientStorageScanTasks = new List<Task>();

                foreach (CompanyGeneralSettingModel companyGeneralSetting in companyGeneralSettings)
                {
                    Console.WriteLine(companyGeneralSetting.CompanyId);
                    List<ClientExternalOrderFTPSetupModel> clientOrderPlaceStorages = await GetAutoOrderPlaceStorages(isInternalStorageOrderPlace, companyGeneralSetting);

                    if (!clientOrderPlaceStorages.Any())
                    {
                        return response;
                    }

                    await ScanStorageAndOrderPlace(clientStorageScanThread, clientStorageScanTasks, companyGeneralSetting, clientOrderPlaceStorages);
                    await Task.WhenAll(clientStorageScanTasks);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.ConsoleAppId,
                    PrimaryId = consoleAppId,
                    ErrorMessage = $"Console App ID: {consoleAppId}. Exception: {ex.Message}",
                    MethodName = "ProcessFileChunkAsync",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            return response;
        }

        private async Task ScanStorageAndOrderPlace(SemaphoreSlim clientStorageScanThread, List<Task> clientStorageScanTasks, CompanyGeneralSettingModel companyGeneralSetting, List<ClientExternalOrderFTPSetupModel> clientOrderPlaceStorages)
        {
            foreach (var storage in clientOrderPlaceStorages)
            {
                storage.InputRootFolder = storage.InputRootFolder.Replace("\\", "/");
                await clientStorageScanThread.WaitAsync();
                Console.WriteLine(storage.Username + " " + storage.InputRootFolder);

                clientStorageScanTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        FtpCredentailsModel sourceFtpCredential = CreateExternalOrderFTPSetupCredentials(storage);
                        CompanyModel company = await GetCompanyById(storage.ClientCompanyId);
                        FileServerModel fileServer = await GetFileServerById(company.FileServerId);
                        FtpCredentailsModel kowToMateFtpCredentials = CreateFileServerCredentials(fileServer);

                        Console.WriteLine($"Order process start for {company.Name}");
                        await StorageTypeWiseOrderPlace(companyGeneralSetting, storage, sourceFtpCredential, company, fileServer, kowToMateFtpCredentials);
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = companyGeneralSetting.CompanyId,
                            ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. {storage.GetInputLogDescription()} Exception: {ex.Message}",
                            MethodName = "ScanStorageAndOrderPlace",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };
                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }

                    finally
                    {
                        clientStorageScanThread.Release();
                    }
                }));
            }
        }

        private async Task StorageTypeWiseOrderPlace(CompanyGeneralSettingModel companyGeneralSetting, ClientExternalOrderFTPSetupModel storage, FtpCredentailsModel sourceFtpCredential, CompanyModel company, FileServerModel fileServer, FtpCredentailsModel kowToMateFtpCredentials)
        {
            if (storage.InputProtocolType == (int)InputProtocolTypeEnum.FTP)
            {
                await OrderProcessFromFtp(storage, sourceFtpCredential, company, companyGeneralSetting, kowToMateFtpCredentials, fileServer);
            }
            if (storage.InputProtocolType == (int)InputProtocolTypeEnum.SFTP)
            {
                await OrderProcessFromSftp(storage, company, companyGeneralSetting, kowToMateFtpCredentials, fileServer);
            }
        }

        private async Task<List<ClientExternalOrderFTPSetupModel>> GetAutoOrderPlaceStorages(bool isInternalFtpOrderPlace, CompanyGeneralSettingModel companyGeneralSetting)
        {
            List<ClientExternalOrderFTPSetupModel> clientOrderFtps;

            if (isInternalFtpOrderPlace)
            {
                clientOrderFtps = await _fileServerService.GetEnabledInternalStorage(companyGeneralSetting.CompanyId);
            }
            else
            {
                clientOrderFtps = await _fileServerService.GetEnabledClientStorage(companyGeneralSetting.CompanyId);
            }

            return clientOrderFtps;
        }

        private SemaphoreSlim GetOrderPlaceFtpsThread()
        {
            string defaultOrderPlacedFtpsThread = "4";
            string orderPlacedFtpsThread = _configuration.GetSection(GeneralSettingsKey)["OrderPlacedFtpsThread"] ?? defaultOrderPlacedFtpsThread;
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(int.Parse(orderPlacedFtpsThread));
            return semaphoreSlim;
        }

        private async Task<List<CompanyGeneralSettingModel>> GetAutoOrderPlaceEnableCompanyGeneralSettings(int consoleAppId)
        {
            string autoOrderPlaceEnableQuery = $"SELECT cgs.* FROM dbo.CompanyGeneralSettings cgs WHERE cgs.EnableFtpOrderPlacement = 1";
            List<CompanyGeneralSettingModel> companyGeneralSettings = await GetEnabledCompanyGeneralSettings(consoleAppId, autoOrderPlaceEnableQuery);
            return companyGeneralSettings;
        }
        #endregion

        #region Private Method
        private FtpCredentailsModel CreateExternalOrderFTPSetupCredentials(ClientExternalOrderFTPSetupModel ftp)
        {
            return new FtpCredentailsModel
            {
                Id = ftp.Id,
                Host = ftp.Host,
                UserName = ftp.Username,
                Password = ftp.Password,
                RootFolder = ftp.InputRootFolder,
                Port = ftp.Port,
                FtpEncryptionMode = ftp.FtpEncryptionMode
            };
        }

        private async Task<CompanyModel> GetCompanyById(long companyId)
        {
            return await _companyService.GetById((int)companyId);
        }

        private async Task<FileServerModel> GetFileServerById(int fileServerId)
        {
            var fileServer = await _fileServerService.GetById(fileServerId);
            var host = fileServer.Host.Split(':');

            if (host.Length == 3)
            {
                fileServer.Host = $"{host[0]}:{host[1]}";
            }

            return fileServer;
        }

        private static FtpCredentailsModel CreateFileServerCredentials(FileServerModel ftp)
        {
            return new FtpCredentailsModel
            {
                Id = ftp.Id,
                Host = ftp.Host,
                UserName = ftp.UserName,
                Password = ftp.Password,
                //RootFolder = ftp.,
                Port = ftp.Port,
                SubFolder = ftp.SubFolder
            };
        }

        private async Task<List<CompanyGeneralSettingModel>> GetEnabledCompanyGeneralSettings(int consoleAppId, string query)
        {
            query = $"{query} and cgs.FtpOrderPlacedAppNo = {consoleAppId}";
            return await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);
        }

        private async Task OrderProcessFromFtp(ClientExternalOrderFTPSetupModel ftp, FtpCredentailsModel sourceFtpCredential, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel kowToMateFtpCredentails, FileServerModel fileServer)
        {
            FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();

            try
            {
                // Process the image file
                int FtpSingleFileCount = 0;

                using (var sourceClient = new AsyncFtpClient(ftp.Host,
                       ftp.Username, ftp.Password, ftp.Port ?? 0, ftpConfig))
                {
                    //sourceClient.Encoding = Encoding.GetEncoding("ISO-8859-1");
                    sourceClient.Encoding = System.Text.Encoding.UTF8;
                    sourceClient.Config.EncryptionMode = FluentFtpService.GetFtpEncryptionModeEnumKey(ftp.FtpEncryptionMode);
                    sourceClient.Config.ValidateAnyCertificate = true;

                    if (FtpEncryptionMode.None == sourceClient.Config.EncryptionMode)
                    {
                        await sourceClient.Connect();
                    }
                    else
                    {
                        await sourceClient.AutoConnect();
                    }
                    string getInMethod = "Login Successfully:........................................................ " + " " + ftp.Username + " " + ftp.InputRootFolder;
                    Console.WriteLine(getInMethod);
                    // Get the list of files and folders in the root folder
                    FtpListItem[] ftpFolderAndFileList = await sourceClient.GetListing(ftp.InputRootFolder);
                    if (ftpFolderAndFileList == null || !ftpFolderAndFileList.Any())
                    {
                        return;
                    }
                    // here remove ftp file move path.
                    if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
                    {
                        ftpFolderAndFileList = ftpFolderAndFileList.Where(x => x.Name != companyGeneralSetting.FtpFileMovedPathAfterOrderCreated).ToArray();
                    }
                    if (companyGeneralSetting.AllowSingleOrderForRootAllFolderAndFiles)
                    {
                        List<string> orderItemFilePaths = new List<string>();
                        string orderDirectory = ftp.InputRootFolder;
                        await GetRootAllFilePath(sourceFtpCredential, companyGeneralSetting, ftpFolderAndFileList, orderItemFilePaths, orderDirectory);
                        if (!orderItemFilePaths.Any()) return;
                        if (companyGeneralSetting.CheckUploadCompletedFlagOnFile)
                        {
                            await CheckHotKeyAndOrderPlace(sourceFtpCredential, company, companyGeneralSetting, kowToMateFtpCredentails, fileServer, orderItemFilePaths, orderDirectory);
                        }
                        if (companyGeneralSetting.IsFtpIdleTimeChecking)
                        {
                            DateTime maxModifiedDate = await GetMaxModifiedTime(sourceClient, orderDirectory, company, ftp);
                            var lastModifiedTimeInMin = DateTime.Now.Subtract(maxModifiedDate.AddHours(6)).TotalMinutes;
                            if (lastModifiedTimeInMin > companyGeneralSetting.FtpIdleTime)
                            {
                                await OrderPlaceFromFtpAndSendMail(sourceFtpCredential, kowToMateFtpCredentails, orderItemFilePaths, company, fileServer, orderDirectory, companyGeneralSetting);
                            }
                        }
                        //When Order Place For Root All Folder and files,Create Root directory After order place
                        await CreateDirectoryOnFtp(sourceClient, orderDirectory);
                    }
                    else
                    {
                        string orderPlaceFtpBatchThread = _configuration.GetSection(GeneralSettingsKey)["OrderPlaceFtpBatchThread"];
                        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(int.Parse(orderPlaceFtpBatchThread));
                        var orderProcessTasks = new List<Task>();
                        //Need to add thread
                        foreach (FtpListItem item in ftpFolderAndFileList)
                        {
                            Console.WriteLine("Start Batch Tracking: " + " " + ftp.Username + " " + ftp.InputRootFolder + " " + item.Name);
                            var cutoffDate = DateTime.Now.AddDays(-10);
                            if (item.Modified <= cutoffDate)
                            {
                                continue;
                            }
                            if (item.Name == AutomatedAppConstant.DefaultOrderPlacedFileContainer || item.Name == "Downloaded")
                            {
                                //semaphoreSlim.Release();
                                continue;
                            }
                            if (item.Type == FtpObjectType.File && item.Name != "Thumbs.db" && item.Name != ".BridgeSort")
                            {
                                FtpSingleFileCount++;
                            }
                            // Here changes logic : TO DO. This block of code ignore for PSP client.
                            // Why check Batch Existance When user provide completed hotkey but why need check ? 
                            if (company.Id != 1185 && company.Id != 1177 && company.Id != 1183) //companycode
                            {
                                bool isExistSameBatch = await _orderService.CheckExistenceOfBatchBySourceFullPath(item.FullName);

                                if (isExistSameBatch)
                                {
                                    continue;
                                }
                            }
                            if (item.Type == FtpObjectType.Directory)
                            {
                                await semaphoreSlim.WaitAsync();

                                orderProcessTasks.Add(Task.Run(async () =>
                                {
                                    try
                                    {
                                        string orderDirectory = ftp.InputRootFolder + "/" + item.Name;

                                        bool isNoPathAllowOnGetFileFromClientFtp = false;
                                        if (company.Id == 1196) //Company code 
                                        {
                                            isNoPathAllowOnGetFileFromClientFtp = true;
                                        }
                                        var fileReadResponse = await ReadAllOrderFilesFromFoldersAsync(sourceFtpCredential, orderDirectory, true, isNoPathAllowOnGetFileFromClientFtp, companyGeneralSetting, companyGeneralSetting.FtpFileMovedPathAfterOrderCreated);
                                        if (fileReadResponse.IsSuccess && fileReadResponse.Result != null && fileReadResponse.Result.Any())
                                        {
                                            //check here is "uploadcomplete.txt" file contain or not
                                            // Here previous check If Statement but here should if elseif statement use - suppose a batch only one configuaration uploadcompleted.txt or FtpIdleTime or Batchname flag
                                            List<string> allFilePath = fileReadResponse.Result;

                                            if (companyGeneralSetting.CheckUploadCompletedFlagOnFile)
                                            {
                                                await CheckHotKeyAndOrderPlace(sourceFtpCredential, company, companyGeneralSetting, kowToMateFtpCredentails, fileServer, fileReadResponse.Result, orderDirectory);
                                            }
                                            else if (companyGeneralSetting.IsFtpIdleTimeChecking)
                                            {
                                                DateTime maxModifiedDate = await GetMaxModifiedTime(sourceClient, orderDirectory, company, ftp);
                                                var lastModifiedTimeInMin = DateTime.Now.Subtract(maxModifiedDate.AddHours(6)).TotalMinutes;
                                                if (lastModifiedTimeInMin > companyGeneralSetting.FtpIdleTime)
                                                {
                                                    await OrderPlaceFromFtpAndSendMail(sourceFtpCredential, kowToMateFtpCredentails, allFilePath, company, fileServer, orderDirectory, companyGeneralSetting);
                                                }
                                            }
                                            else if (companyGeneralSetting.CheckUploadCompletedFlagOnBatchName && !string.IsNullOrEmpty(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
                                            {
                                                if (item.Name.Contains(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
                                                {
                                                    await OrderPlaceFromFtpAndSendMail(sourceFtpCredential, kowToMateFtpCredentails, allFilePath, company, fileServer, orderDirectory, companyGeneralSetting);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                        {
                                            CreatedByContactId = AutomatedAppConstant.ContactId,
                                            CompanyObjectId = company.ObjectId,
                                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                                            PrimaryId = company.Id,
                                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Path: {item.FullName}. Exception: {ex.Message}",
                                            MethodName = "OrderProcessFromFtp=>FtpFolderAndFileListLoop",
                                            RazorPage = "OrderPlaceService",
                                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                        };

                                        await _activityAppLogService.InsertAppErrorActivityLog(activity);


                                        //string methodName = "GetOrderFilesUsingFluentFtp";
                                        //byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;

                                        //await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
                                    }
                                    finally
                                    {
                                        semaphoreSlim.Release();
                                    }
                                }));
                            }
                        }
                        await Task.WhenAll(orderProcessTasks);
                        Console.WriteLine("Files : " + FtpSingleFileCount.ToString());
                    }
                }
                //Preocess here
                await OrderPlaceForFtpSingleFile(ftp, sourceFtpCredential, company, companyGeneralSetting, kowToMateFtpCredentails, fileServer, ftpConfig, FtpSingleFileCount);
                //await sourceClient.Disconnect();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()} Exception: {ex.Message}",
                    MethodName = "OrderProcessFromFtp",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
                Console.WriteLine(ex.Message);
            }

            //}
        }

        private async Task OrderPlaceForFtpSingleFile(ClientExternalOrderFTPSetupModel ftp, FtpCredentailsModel sourceFtpCredential, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel kowToMateFtpCredentails, FileServerModel fileServer, FtpConfig ftpConfig, int FtpSingleFileCount)
        {
            if (FtpSingleFileCount > 0)
            {
                var orderFolder = ftp.InputRootFolder;
                var allFilesOnOrderFolder = await ReadAllOrderFilesFromFoldersAsync(sourceFtpCredential, orderFolder, false, true, companyGeneralSetting, companyGeneralSetting.FtpFileMovedPathAfterOrderCreated);

                if (!allFilesOnOrderFolder.IsSuccess || allFilesOnOrderFolder.Result == null || !allFilesOnOrderFolder.Result.Any())
                {
                    return;
                }
                // New Logic : Here check hot key. 
                List<string> zipImagePaths = new List<string>();

                zipImagePaths = allFilesOnOrderFolder.Result.FindAll(path => Path.GetExtension(path) == ".zip" || Path.GetExtension(path) == ".rar");
                allFilesOnOrderFolder.Result.RemoveAll(path => Path.GetExtension(path) == ".zip" || Path.GetExtension(path) == ".rar");

                await OrderPlaceForFtpZipFile(ftp, sourceFtpCredential, company, companyGeneralSetting, kowToMateFtpCredentails, fileServer, ftpConfig, zipImagePaths);
                await OrderPlaceForFtpFiles(sourceFtpCredential, company, companyGeneralSetting, kowToMateFtpCredentails, fileServer, allFilesOnOrderFolder);
            }
        }

        private async Task OrderPlaceForFtpFiles(FtpCredentailsModel sourceFtpCredential, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel kowToMateFtpCredentails, FileServerModel fileServer, Response<List<string>> allFilesOnOrderFolder)
        {
            List<string> allFilePath = allFilesOnOrderFolder.Result;

            int chunkSize = companyGeneralSetting.AllowMaxNumOfItemsPerOrder;

            var chunks = GetChunksOfPaths(allFilePath, chunkSize);

            // Create a SemaphoreSlim object with a maximum count of 3
            var semaphore = new SemaphoreSlim(5);

            var tasks = new List<Task>();

            foreach (var chunk in chunks)
            {
                // Wait for a slot to become available in the semaphore
                await semaphore.WaitAsync();
                Thread.Sleep(2000);
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {

                        bool isSinglefileMove = true;

                        if (companyGeneralSetting.OrderPlaceBatchMoveType == (short)OrderPlaceBatchMoveType.FileandFolderNotMoveAfterOrderPlace)
                        {
                            isSinglefileMove = false;

                        }

                        var copyResponse = await OrderPlaceFromFtpAndSendMail(sourceFtpCredential, kowToMateFtpCredentails, chunk, company, fileServer, "", companyGeneralSetting, isSinglefileMove);
                        //if (copyResponse.IsSuccess)
                        //{
                        //	await SendEmailToOpsToNotifyOrderUpload("---Single File On Root Folder---", copyResponse.Result, company);
                        //}
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            CompanyObjectId = company.ObjectId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = company.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Exception: {ex.Message}",
                            MethodName = "OrderPlaceForFtpFiles",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));

                Thread.Sleep(1005);
            }

            await Task.WhenAll(tasks);
        }

        private async Task OrderPlaceForFtpZipFile(ClientExternalOrderFTPSetupModel ftp, FtpCredentailsModel sourceFtpCredential, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel kowToMateFtpCredentails, FileServerModel fileServer, FtpConfig ftpConfig, List<string> zipImagePaths)
        {
            if (zipImagePaths != null && zipImagePaths.Any())
            {
                Console.WriteLine("Zip processing start");
                await Task.Delay(9000);

                foreach (var zipSourceFullPath in zipImagePaths)
                {
                    Console.WriteLine("PATH processing start");

                    bool isExistSameBatch = await _orderService.CheckExistenceOfBatchBySourceFullPath(zipSourceFullPath);
                    if (isExistSameBatch)
                    {
                        Console.WriteLine("File Exist");
                        continue;
                    }
                    Console.WriteLine("File Not Exist");

                    string appRoot = AppDomain.CurrentDomain.BaseDirectory;
                    string temporaryStorePath = Path.Combine(appRoot, "TemporaryStore/");
                    temporaryStorePath = temporaryStorePath + company.Code + ftp.InputRootFolder + "/";
                    temporaryStorePath = temporaryStorePath.Replace("\\", "/");

                    try
                    {
                        if (!Directory.Exists(temporaryStorePath))
                        {
                            Directory.CreateDirectory(temporaryStorePath);
                        }
                        using (var sourceClientFtp = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                        {
                            sourceClientFtp.Config.EncryptionMode = FluentFtpService.GetFtpEncryptionModeEnumKey(ftp.FtpEncryptionMode);
                            await sourceClientFtp.Connect();

                            if (!await sourceClientFtp.FileExists(zipSourceFullPath))
                            {
                                await sourceClientFtp.Disconnect();
                                continue;
                            }
                            if (await sourceClientFtp.GetFileSize(zipSourceFullPath) <= 0)
                            {
                                await sourceClientFtp.Disconnect();
                                continue;
                            }

                            FtpStatus fileDownlaodStatus = await ZipFileDownloadToAppDomainFolder(zipSourceFullPath, temporaryStorePath, sourceClientFtp, sourceFtpCredential, company);

                            if (fileDownlaodStatus.Equals(FtpStatus.Failed))
                            {
                                Console.WriteLine(fileDownlaodStatus);

                                Directory.Delete(temporaryStorePath, true);
                                await sourceClientFtp.Disconnect();
                                continue;
                            }

                            await sourceClientFtp.Disconnect();

                            if (!Directory.Exists(temporaryStorePath + "ExtractPath/"))
                            {
                                Directory.CreateDirectory(temporaryStorePath + "ExtractPath/");
                            }

                            string sourcePath = temporaryStorePath + "/" + Path.GetFileName(zipSourceFullPath);
                            string destination = temporaryStorePath + "ExtractPath/" + Path.GetFileNameWithoutExtension(zipSourceFullPath);

                            //Extract Zip File
                            ZipFileExtractOnAppPublishedLocationAndSanitize(sourcePath, destination);

                            //Delete Zip File
                            if (System.IO.File.Exists(sourcePath))
                            {
                                System.IO.File.Delete(sourcePath);
                            }
                            Console.WriteLine("Path Create done");
                            await Task.Delay(4000);

                            var pathList = Directory.GetFiles(destination, "*", SearchOption.AllDirectories);

                            if (pathList != null && pathList.Any())
                            {
                                Console.WriteLine("Order Placing Start");
                                await Task.Delay(4000);

                                await OrderPlaceFromFtpAndSendMail(sourceFtpCredential, kowToMateFtpCredentails, pathList.ToList(), company, fileServer, zipSourceFullPath, companyGeneralSetting, false, true);

                                // Delete Project server Unzip Folder After Order Place
                                if (Directory.Exists(destination))
                                {
                                    DeleteFolder(destination);
                                }
                                Console.WriteLine("cONNECT Start");
                                //await Task.Delay(9000);
                                await sourceClientFtp.Connect();
                                Console.WriteLine("cONNECT WITH SOURCE");
                                //await Task.Delay(9000);
                            }
                            //await sourceClientFtp.Disconnect();
                        }
                        await FileMoveToDownloadedFolderAfterOrderPlace(sourceFtpCredential, companyGeneralSetting, ftpConfig, zipSourceFullPath, company);
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            CompanyObjectId = company.ObjectId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = company.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Zip Source Path: {zipSourceFullPath}. Exception: {ex.Message}",
                            MethodName = "ProcessFileChunkAsync",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);

                        // Log the exception or handle it appropriately
                        Console.WriteLine($"Error creating directory: {ex.Message}");
                    }

                }
            }
        }

        private async Task<FtpStatus> ZipFileDownloadToAppDomainFolder(string zipSourceFullPath, string temporaryStorePath,
            AsyncFtpClient sourceClientFtp, FtpCredentailsModel sourceFtpCredential, CompanyModel company)
        {
            int maxRetries = 3;
            int retryCount = 0;
            bool success = false;
            FtpStatus fileDownlaodStatus = FtpStatus.Failed;

            while (retryCount < maxRetries && !success)
            {
                try
                {

                    var stopwatch = new Stopwatch();
                    DateTime lastTimestamp = DateTime.Now;
                    long lastBytes = 0;

                    var progress = new Progress<FtpProgress>(p =>
                    {
                        if (p.TransferredBytes > 0)
                        {
                            var currentTime = DateTime.Now;
                            double elapsedSeconds = (currentTime - lastTimestamp).TotalSeconds;

                            if (elapsedSeconds > 0)
                            {
                                long bytesSinceLast = p.TransferredBytes - lastBytes;
                                double transferRate = bytesSinceLast / elapsedSeconds;

                                Console.WriteLine($"Progress: {p.Progress}% | Speed: {transferRate / 1024 / 1024:F2} MB/s");

                                lastTimestamp = currentTime;
                                lastBytes = p.TransferredBytes;
                            }
                        }
                    });

                    stopwatch.Start();


                    using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(60)))
                    {
                        fileDownlaodStatus = await sourceClientFtp.DownloadFile(temporaryStorePath + "/" + Path.GetFileName(zipSourceFullPath), zipSourceFullPath, FtpLocalExists.Resume, FtpVerify.None, progress, cts.Token);

                        stopwatch.Stop();
                        Console.WriteLine($"Download completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");

                        if (fileDownlaodStatus == FtpStatus.Success || fileDownlaodStatus == FtpStatus.Skipped)
                        {
                            success = true;
                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    retryCount++;
                    Console.WriteLine($"Timeout occurred, retrying... ({retryCount}/{maxRetries})");

                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Zip Source Path: {zipSourceFullPath}. Exception: {ex.Message}",
                        MethodName = "ZipFileDownloadToAppDomainFolder",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading file: {ex.Message}");
                    retryCount++;

                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Zip Source Path: {zipSourceFullPath}. Exception: {ex.Message}",
                        MethodName = "ZipFileDownloadToAppDomainFolder",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);
                }
            }

            if (!success)
            {
                Console.WriteLine("File download failed after multiple attempts.");
            }

            return fileDownlaodStatus;
        }

        private static async Task CreateDirectoryOnFtp(AsyncFtpClient sourceClient, string orderDirectory)
        {
            if (!await sourceClient.DirectoryExists(orderDirectory))
            {
                await sourceClient.CreateDirectory(orderDirectory);
            }
        }

        private async Task CheckHotKeyAndOrderPlace(FtpCredentailsModel sourceFtpCredential, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel kowToMateFtpCredentails, FileServerModel fileServer, List<string> orderItemFilePaths, string orderDirectory)
        {
            if (orderItemFilePaths.Exists(fp => fp.Contains(AutomatedAppConstant.ClientUploadCompletedIndicator)
                                            || fp.Contains(AutomatedAppConstant.GLS_AMSClientOrderPlaceIndicator)))
            {
                orderItemFilePaths.RemoveAll(fp => fp.Contains(AutomatedAppConstant.ClientUploadCompletedIndicator)
                || fp.Contains(AutomatedAppConstant.GLS_AMSClientOrderPlaceIndicator));

                if (orderItemFilePaths.Any())
                {
                    await OrderPlaceFromFtpAndSendMail(sourceFtpCredential, kowToMateFtpCredentails, orderItemFilePaths, company, fileServer, orderDirectory, companyGeneralSetting);
                }
            }
        }

        private async Task GetRootAllFilePath(FtpCredentailsModel sourceFtpCredential, CompanyGeneralSettingModel companyGeneralSetting, FtpListItem[] ftpFolderAndFileList, List<string> orderItemFilePaths, string orderDirectory)
        {
            bool isNoPathAllowOnGetFileFromClientFtp = false;

            var filteredOrderBatches = ftpFolderAndFileList.
                Where(orderBatch => orderBatch.Name != AutomatedAppConstant.DefaultOrderPlacedFileContainer
                      && orderBatch.Name != "Downloaded")
                .Select(orderBatch => orderDirectory + "/" + orderBatch.Name);

            foreach (var orderBatchPath in filteredOrderBatches)
            {
                var response = await ReadAllOrderFilesFromFoldersAsync(sourceFtpCredential, orderBatchPath, true, isNoPathAllowOnGetFileFromClientFtp, companyGeneralSetting, companyGeneralSetting.FtpFileMovedPathAfterOrderCreated);
                orderItemFilePaths.AddRange(response.Result);
            }
        }

        private async Task FileMoveToDownloadedFolderAfterOrderPlace(FtpCredentailsModel sourceFtpCredential, CompanyGeneralSettingModel companyGeneralSetting,
            FtpConfig ftpConfig, string zipSourceFullPath, CompanyModel company)
        {
            if (companyGeneralSetting.OrderPlaceBatchMoveType != (short)OrderPlaceBatchMoveType.FileandFolderNotMoveAfterOrderPlace)
            {
                using (var sourceClientFtp = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                {
                    try
                    {
                        sourceClientFtp.Config.EncryptionMode = FluentFtpService.GetFtpEncryptionModeEnumKey(sourceFtpCredential.FtpEncryptionMode);
                        await sourceClientFtp.AutoConnect();
                        string zipFileName = Path.GetFileName(zipSourceFullPath);
                        //var zipMovePathAfterOrderPlace = sourceFtpCredential.RootFolder + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/" + zipFileName;
                        var zipMovePathAfterOrderPlace = sourceFtpCredential.RootFolder + "/" + companyGeneralSetting.FtpFileMovedPathAfterOrderCreated + "/" + zipFileName;
                        zipMovePathAfterOrderPlace = zipMovePathAfterOrderPlace.Replace("\\", "/");

                        bool isZipMovePathAfterOrderPlaceExist = await sourceClientFtp.DirectoryExists(zipMovePathAfterOrderPlace);

                        if (isZipMovePathAfterOrderPlaceExist)
                        {
                            await sourceClientFtp.DeleteDirectory(zipMovePathAfterOrderPlace);
                        }

                        await sourceClientFtp.MoveFile(zipSourceFullPath, zipMovePathAfterOrderPlace, FtpRemoteExists.Overwrite);
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            CompanyObjectId = company.ObjectId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = company.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Zip Source Path: {zipSourceFullPath}. Exception: {ex.Message}",
                            MethodName = "FileMoveToDownloadedFolderAfterOrderPlace",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);

                        Console.WriteLine($"Error zip moving directory: {ex.Message}");
                    }
                }
            }
        }
        private void ZipFileExtractOnAppPublishedLocationAndSanitize(string sourcePath, string destination)
        {
            if (!Directory.Exists(destination))
            {
                using (ZipArchive archive = ZipFile.OpenRead(sourcePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {

                        string directoryPath = Path.GetDirectoryName(entry.FullName);
                        string sanitizedFileName = SanitizeFileName(Path.GetFileName(entry.FullName));

                        string destinationPath = Path.Combine(destination, directoryPath ?? string.Empty, sanitizedFileName);

                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                            entry.ExtractToFile(destinationPath, true);
                        }
                    }
                }
            }
        }
        private static string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        #region Order upload for SFTP Protocol
        private async Task OrderProcessFromSftp(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel destinationFtp, FileServerModel fileServer)
        {
            using (SftpClient sftpClient = await InitializeSftpClient(sourceFtp))
            {
                var (files, zipFiles) = await GetFilesAndZipFiles(sftpClient, sourceFtp, companyGeneralSetting);
                var directoriesToProcess = await GetDirectoriesToProcess(sftpClient, sourceFtp, company, companyGeneralSetting);

                await OrderProcessDirectories(directoriesToProcess, sourceFtp, company, companyGeneralSetting, destinationFtp, fileServer);

                await OrderProcessRegularFiles(sftpClient, files, sourceFtp, company, destinationFtp, fileServer, companyGeneralSetting);

                await OrderProcessZipFiles(sftpClient, zipFiles, sourceFtp, destinationFtp, company, companyGeneralSetting, fileServer);
            }


        }
        /// <summary>
        /// SFTP Client Connect
        /// </summary>
        /// <param name="sourceFtp"></param>
        /// <returns></returns>
        private async Task<SftpClient> InitializeSftpClient(ClientExternalOrderFTPSetupModel sourceFtp)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();
            return sftpClient;
        }
        /// <summary>
        /// Get Regular files and Zip Files input root path
        /// </summary>
        /// <param name="sftpClient"></param>
        /// <param name="sourceFtp"></param>
        /// <param name="companyGeneralSetting"></param>
        /// <returns></returns>
        private async Task<(List<string> Files, List<(string FilePath, DateTime LastWriteTime, long FileSize)> ZipFiles)> GetFilesAndZipFiles(SftpClient sftpClient, ClientExternalOrderFTPSetupModel sourceFtp, CompanyGeneralSettingModel companyGeneralSetting)
        {
            List<string> files;
            var zipFiles = new List<(string FilePath, DateTime LastWriteTime, long FileSize)>();

            if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
            {
                var lastTime = DateTime.Now.AddMinutes(-Convert.ToInt64(companyGeneralSetting.FtpIdleTime));
                var listing = await SFTPLast24HoursFiles(sourceFtp, sourceFtp.InputRootFolder);
                listing = listing.Where(x => x.LastWriteTime < lastTime).ToList();

                files = listing
                    .Where(entry => entry.IsRegularFile && !entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.FullName)
                    .ToList();

                zipFiles = listing
                    .Where(entry => entry.IsRegularFile && entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .Select(entry => (FilePath: entry.FullName, entry.LastWriteTime, FileSize: entry.Length))
                    .ToList();
            }
            else
            {
                files = sftpClient.ListDirectory(sourceFtp.InputRootFolder)
                    .Where(entry => entry.IsRegularFile && !entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.FullName)
                    .ToList();

                zipFiles = sftpClient.ListDirectory(sourceFtp.InputRootFolder)
                    .Where(entry => entry.IsRegularFile && entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .Select(entry => (FilePath: entry.FullName, entry.LastWriteTime, FileSize: entry.Length))
                    .ToList();
            }

            return (files, zipFiles);
        }
        /// <summary>
        /// Get All directories in input root path.
        /// </summary>
        /// <param name="sftpClient"></param>
        /// <param name="sourceFtp"></param>
        /// <param name="company"></param>
        /// <param name="companyGeneralSetting"></param>
        /// <returns></returns>
        private async Task<List<SftpFile>> GetDirectoriesToProcess(SftpClient sftpClient, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting)
        {
            var listing = sftpClient.ListDirectory(sourceFtp.InputRootFolder);
            if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
            {
                listing = await SFTPLast24HoursDirectories(sourceFtp, sourceFtp.InputRootFolder);
            }

            var directories = listing
                .Where(item => !string.IsNullOrEmpty(item.Name) && item.IsDirectory && item.Name != "." && item.Name != ".." && item.Name != AutomatedAppConstant.DefaultOrderPlacedFileContainer)
                .ToList();

            return directories;
        }
        /// <summary>
        /// Create order with directories.
        /// </summary>
        /// <param name="directories"></param>
        /// <param name="sourceFtp"></param>
        /// <param name="company"></param>
        /// <param name="companyGeneralSetting"></param>
        /// <param name="destinationFtp"></param>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        private async Task OrderProcessDirectories(List<SftpFile> directories, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel destinationFtp, FileServerModel fileServer)
        {
            var semaphoreSlim = new SemaphoreSlim(int.Parse(_configuration.GetSection(GeneralSettingsKey)["OrderPlaceSFtpBatchThread"]));
            var orderProcessTasks = new List<Task>();

            foreach (var directory in directories)
            {
                bool isExistSameBatch = await _orderService.CheckBatchNameExistenceOnOrderPlacingStatus(directory.FullName);

                if (isExistSameBatch && (company.Id == 1176 || company.Id == 1201))//CompanyId using
                {
                    continue;
                }

                await semaphoreSlim.WaitAsync();
                orderProcessTasks.Add(ProvideStorageFolderForOrderProcess(directory, sourceFtp, company, companyGeneralSetting, destinationFtp, fileServer, semaphoreSlim));
            }

            await Task.WhenAll(orderProcessTasks);
            semaphoreSlim.Release();
        }
        /// <summary>
        /// Order create with Regular Files.
        /// </summary>
        /// <param name="sftpClient"></param>
        /// <param name="files"></param>
        /// <param name="sourceFtp"></param>
        /// <param name="company"></param>
        /// <param name="destinationFtp"></param>
        /// <param name="fileServer"></param>
        /// <param name="companyGeneralSetting"></param>
        /// <returns></returns>
        private async Task OrderProcessRegularFiles(SftpClient sftpClient, List<string> files, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, FtpCredentailsModel destinationFtp, FileServerModel fileServer, CompanyGeneralSettingModel companyGeneralSetting)
        {
            if (files.Count > 0)
            {
                var allFilePath = files;

                if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
                {
                    allFilePath = await FilterExistingFiles(company, files, sourceFtp);
                }
                // Create chunk for order create.
                var chunks = GetChunksOfPaths(allFilePath, companyGeneralSetting.AllowMaxNumOfItemsPerOrder);
                var semaphore = new SemaphoreSlim(10);
                var singleFileTasks = new List<Task>();

                foreach (var chunk in chunks)
                {
                    await semaphore.WaitAsync();
                    singleFileTasks.Add(CopyFilesAndNotify(destinationFtp, allFilePath, company, fileServer, sourceFtp, sourceFtp.InputRootFolder, companyGeneralSetting, true));
                }

                await Task.WhenAll(singleFileTasks);
                semaphore.Release();
            }
        }
        /// <summary>
        /// Order create with Zip Files.
        /// </summary>
        /// <param name="sftpClient"></param>
        /// <param name="zipFiles"></param>
        /// <param name="sourceFtp"></param>
        /// <param name="destinationFtp"></param>
        /// <param name="company"></param>
        /// <param name="companyGeneralSetting"></param>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        private async Task OrderProcessZipFiles(SftpClient sftpClient, List<(string FilePath, DateTime LastWriteTime, long FileSize)> zipFiles,
            ClientExternalOrderFTPSetupModel sourceFtp, FtpCredentailsModel destinationFtp, CompanyModel company,
            CompanyGeneralSettingModel companyGeneralSetting, FileServerModel fileServer)
        {
            foreach (var zipFile in zipFiles)
            {
                if (await _orderService.CheckExistenceOfBatchBySourceFullPath(zipFile.FilePath))
                {
                    continue;
                }

                string tempStorePath = CreateTemporaryFolderForDownload();

                try
                {
                    await DownloadAndExtractZipFile(sftpClient, zipFile, tempStorePath);
                    var pathList = Directory.GetFiles(tempStorePath + "ExtractPath/", "*", SearchOption.AllDirectories);

                    if (pathList.Any())
                    {
                        var response = await CopyFilesFromLocalToFTP(sourceFtp, destinationFtp, pathList.ToList(), company, fileServer, sourceFtp.InputRootFolder, companyGeneralSetting, zipFile, false, true);

                        if (response.IsSuccess)
                        {
                            await _sshNetService.SingleFileMove(sourceFtp, zipFile.FilePath);
                        }

                        CleanupTemporaryStore(tempStorePath);
                    }
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. File Path: {zipFile.FilePath}. Exception: {ex.Message}",
                        MethodName = "OrderProcessZipFiles",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                    Console.WriteLine($"Error processing zip file: {ex.Message}");
                }
                finally
                {
                    sftpClient.Disconnect();
                }
            }
        }
        /// <summary>
        /// Create temporary folder for download file.
        /// </summary>
        /// <returns></returns>
        private string CreateTemporaryFolderForDownload()
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            string temporaryStorePath = Path.Combine(appRoot, "TemporaryStore/").Replace("\\", "/");

            if (!Directory.Exists(temporaryStorePath))
            {
                Directory.CreateDirectory(temporaryStorePath);
            }

            return temporaryStorePath;
        }
        /// <summary>
        /// Downloaded and zip file extract
        /// </summary>
        /// <param name="sftpClient"></param>
        /// <param name="zipFile"></param>
        /// <param name="tempStorePath"></param>
        /// <returns></returns>
        private async Task DownloadAndExtractZipFile(SftpClient sftpClient, (string FilePath, DateTime LastWriteTime, long FileSize) zipFile, string tempStorePath)
        {
            using (var streamToWrite = System.IO.File.OpenWrite(tempStorePath + "/" + Path.GetFileName(zipFile.FilePath)))
            {
                sftpClient.DownloadFile(zipFile.FilePath, streamToWrite);
            }

            string destination = tempStorePath + "ExtractPath/";
            if (!Directory.Exists(destination))
            {
                ZipFile.ExtractToDirectory(tempStorePath + "/" + Path.GetFileName(zipFile.FilePath), destination);
            }
        }
        /// <summary>
        /// When order created then clean tem directory.
        /// </summary>
        /// <param name="tempStorePath"></param>
        private void CleanupTemporaryStore(string tempStorePath)
        {
            if (Directory.Exists(tempStorePath))
            {
                Directory.Delete(tempStorePath, true);
            }
        }
        /// <summary>
        /// Check Exist File
        /// </summary>
        /// <param name="company"></param>
        /// <param name="files"></param>
        /// <param name="sourceFtp"></param>
        /// <returns></returns>
        private async Task<List<string>> FilterExistingFiles(CompanyModel company, List<string> files, ClientExternalOrderFTPSetupModel sourceFtp)
        {
            var filteredFiles = new List<string>();
            foreach (var filePath in files)
            {
                var withoutRootPath = await RemoveInputRootFolder(sourceFtp.InputRootFolder, filePath);
                var exists = await _clientOrderItemService.CheckClientOrderItemFile(company.Id, withoutRootPath, Path.GetFileName(filePath), DateTime.Now.Date.ToString());
                if (!exists)
                {
                    filteredFiles.Add(filePath);
                }
            }
            return filteredFiles;
        }


        #endregion Order upload for SFTP Protocol
        private async Task<Response<List<string>>> ReadAllOrderFilesFromFoldersAsync(FtpCredentailsModel sourceFtpCredential, string orderBaseFolderPath, bool isRecursive, bool isNoPath, CompanyGeneralSettingModel companyGeneralSetting, string skipFolderPath = null)
        {
            var response = new Response<List<string>>();
            try
            {
                #region Read Ftp File Path desired input path
                var config = new FtpConfig
                {
                    EncryptionMode = FluentFtpService.GetFtpEncryptionModeEnumKey(sourceFtpCredential.FtpEncryptionMode),
                };
                var ftpClient = new FTPClient(sourceFtpCredential, companyGeneralSetting, config);
                await ftpClient.Connect();
                var folders = new List<string>();
                if (isRecursive)
                {
                    folders = await ftpClient.ReadallFtpFilePathWithRecursively(orderBaseFolderPath, skipFolderPath);
                }
                else
                {
                    folders = await ftpClient.ReadallFtpFilePathWithoutRecursively(orderBaseFolderPath);

                }

                await ftpClient.Disconnect();
                response.Result = folders;
                response.IsSuccess = true;
                #endregion Read Ftp File Path desired input path
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = companyGeneralSetting.CompanyId,
                    ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId} {sourceFtpCredential.GetLogDescription()} Exception: {ex.Message}",
                    MethodName = "ReadAllOrderFilesFromFoldersAsync",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            return response;
        }

        private async Task ProcessFileChunkAsync(List<string> chunk, ClientExternalOrderFTPSetupModel sourceFtp, FtpCredentailsModel destinationFtp, CompanyModel company, FileServerModel fileServer, SemaphoreSlim semaphore)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        FtpCredentailsModel sourceFtpCredentials = ConvertExternalClientFtpToFtpCreadentials(sourceFtp);

                        var copyResponse = await OrderPlaceFromFtpAndSendMail(sourceFtpCredentials, destinationFtp, chunk, company, fileServer, "", new CompanyGeneralSettingModel(), true);

                        if (copyResponse.IsSuccess)
                        {
                            await SendEmailToOpsToNotifyOrderUpload("---Single File On Root Folder---", copyResponse.Result, company);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            CompanyObjectId = company.ObjectId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = company.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()} Exception: {ex.Message}",
                            MethodName = "ProcessFileChunkAsync",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };
                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()} Exception: {ex.Message}",
                    MethodName = "ProcessFileChunkAsync",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task<Response<string>> OrderPlaceFromFtpAndSendMail(FtpCredentailsModel sourceFtpCredential,
            FtpCredentailsModel destinationFtpCredentails, List<string> allFilesFromNewOrder, CompanyModel company,
            FileServerModel fileServer, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, bool isMoveSingleFile = false, bool isLocalFile = false)
        {
            var response = new Response<string>();
            var orderSaveResponse = new Response<ClientOrderModel>();
            List<string> orderDirectoryPaths = new List<string>();
            //Bath name send to ops mail . 
            string batchName = "";
            try
            {
                RemoveNonOrderableFiles(allFilesFromNewOrder);

                if (allFilesFromNewOrder.Any())
                {
                    orderSaveResponse = await AddOrderInfo(company, fileServer, sourceFtpCredential.Id, orderDirectory);
                }

                if (orderSaveResponse.IsSuccess)
                {
                    await UpdateOrderIsExtrafileAllowOnCompanySettings(companyGeneralSetting, orderSaveResponse);

                    await AddOrderAttachmentFilesAndRemoveFromOrderableFiles(sourceFtpCredential, destinationFtpCredentails, allFilesFromNewOrder, company, isLocalFile, orderSaveResponse);

                    string orderPlaceFtpFilesThread = _configuration.GetSection(GeneralSettingsKey)["OrderPlaceFtpFilesThread"];
                    var semaphore = new SemaphoreSlim(int.Parse(orderPlaceFtpFilesThread));


                    var tasks = allFilesFromNewOrder.Select(async path =>
                    {
                        await semaphore.WaitAsync();

                        try
                        {
                            int retryCount = 0;
                            bool uploadSuccessful = false;

                            while (!uploadSuccessful && retryCount < 3)
                            {
                                try
                                {
                                    FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
                                    using (var sourceClient = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                                    using (var destinationClient = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
                                    {
                                        //sourceClient.Encoding = Encoding.GetEncoding("ISO-8859-1");
                                        await ConnectSourceFtpClient(sourceClient);
                                        await ConnnectDestinationClient(destinationClient);

                                        var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, orderSaveResponse.Result.CreatedDate, orderSaveResponse.Result.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);
                                        string[] pathArray = new string[1000];
                                        string destinationFilePath = "";

                                        // zehetu path divided ftp root folder , so akhane root folder null execption dhora holo.
                                        if (!string.IsNullOrWhiteSpace(sourceFtpCredential.RootFolder) || uploadDirectory != null && isLocalFile)
                                        {
                                            if (uploadDirectory != null && isLocalFile)
                                            {
                                                pathArray = path.Split(AutomatedAppConstant.extractParentFolder);
                                            }
                                            else
                                            {
                                                pathArray = path.Split(sourceFtpCredential.RootFolder);
                                            }
                                        }
                                        var pathReplacementList = await _pathReplacementService.GetPathReplacements(company.Id);
                                        //When a company order need Batch Parent Folder 
                                        if (companyGeneralSetting.IsBatchRootFolderNameAddWithOrder)
                                        {
                                            destinationFilePath = await CombineSourceRootPathWithOrderItemPath(sourceFtpCredential, pathArray, destinationFilePath, pathReplacementList);
                                        }
                                        // If ftp root is null then assign path.
                                        else if (string.IsNullOrWhiteSpace(sourceFtpCredential.RootFolder) || sourceFtpCredential.RootFolder == "/")
                                        {
                                            destinationFilePath = path;
                                        }
                                        else
                                        {
                                            destinationFilePath = pathArray[1];
                                        }

                                        var fullFilePathForFtp = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, destinationFilePath);

                                        if (!string.IsNullOrWhiteSpace(destinationFtpCredentails.SubFolder))
                                        {
                                            fullFilePathForFtp = $"{destinationFtpCredentails.SubFolder}/{fullFilePathForFtp}";
                                        }

                                        Console.WriteLine($"Upload to Ftp : {Path.GetFileName(path)}");

                                        // Set OrderDirectory Path 
                                        //Why we use this?? Todo: Zakir
                                        if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                                        {
                                            orderDirectory = Path.GetDirectoryName(path);
                                            if (!orderDirectoryPaths.Contains(orderDirectory))
                                            {
                                                orderDirectoryPaths.Add(orderDirectory);
                                            }
                                        }
                                        var startUpload = DateTime.Now;
                                        Console.WriteLine(startUpload);

                                        if (!await destinationClient.DirectoryExists(Path.GetDirectoryName(fullFilePathForFtp)))
                                        {
                                            await destinationClient.CreateDirectory(Path.GetDirectoryName(fullFilePathForFtp));
                                        }
                                        string fullFilePath = PrepareOrderFileDestinationPath(uploadDirectory, destinationFilePath);
                                        ClientOrderItemModel clientOrderItem = await PrepareOrderItem(company, path, orderSaveResponse, sourceClient, uploadDirectory, destinationFilePath, fullFilePath);
                                        var addItemResponse = await AddOrderItem(clientOrderItem, company, path, orderSaveResponse.Result.Id, sourceFtpCredential.GetLogDescription(), InternalOrderItemStatus.OrderPlacing);

                                        uploadSuccessful = addItemResponse.IsSuccess;
                                        clientOrderItem.Id = addItemResponse.Result;

                                        //Add Order Item End 
                                        bool isDownloaded = false;
                                        if (isLocalFile)
                                        {
                                            isDownloaded = await UploadOrderItemFromAppDomainPath(path, destinationClient, fullFilePathForFtp, clientOrderItem, isDownloaded, destinationFtpCredentails, company);
                                        }
                                        else
                                        {
                                            isDownloaded = await UploadOrderItemFromFtpSourcePath(path, sourceClient, destinationClient, fullFilePathForFtp, clientOrderItem, isDownloaded, sourceFtpCredential, company);
                                        }

                                        var uploadFinish = DateTime.Now;
                                        Console.WriteLine(uploadFinish.Subtract(startUpload).TotalSeconds);

                                        if (isDownloaded && isMoveSingleFile)
                                        {
                                            await MoveSourceFile(sourceFtpCredential, path, sourceClient, pathArray, addItemResponse, company);
                                        }
                                        else
                                        {
                                            await _clientOrderItemService.Delete(addItemResponse.Result.ToString());
                                        }
                                        await sourceClient.Disconnect();
                                        await destinationClient.Disconnect();
                                        // Here compare bytes source to destination.
                                        if (!isLocalFile)
                                        {
                                            var compareBytes = await CreateBytesAndCompareFromPaths(sourceFtpCredential, path, destinationFtpCredentails, fullFilePath, company);
                                        }
                                    }
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                    {
                                        CreatedByContactId = AutomatedAppConstant.ContactId,
                                        CompanyObjectId = company.ObjectId,
                                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                                        PrimaryId = company.Id,
                                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Path: {path}. Exception: {ex.Message}",
                                        MethodName = "OrderPlaceFromFtpAndSendMail->PathLoop",
                                        RazorPage = "OrderPlaceService",
                                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                    };

                                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                    retryCount++;
                                    Thread.Sleep(1000);

                                    if (retryCount >= 3)
                                    {
                                        Console.WriteLine(ex.ToString());
                                        break;
                                    }
                                }


                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }

                    }).ToArray();

                    await Task.WhenAll(tasks);

                    //Delete Order Place if order does not contain any images 
                    var newOrder = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
                    if (newOrder.NumberOfImage == 0)
                    {
                        await _orderService.Delete(newOrder.ObjectId);

                        response.IsSuccess = false;
                        response.Result = "";
                        return response;
                    }

                    bool isOrderUpdated = await _orderStatusService.UpdateOrderStatus(orderSaveResponse.Result, AutomatedAppConstant.ContactId);
                    bool isArrivalTimeUpdate = await _orderStatusService.UpdateOrderArrivalTime(orderSaveResponse.Result);

                    //Update Order DeadLine
                    await UpdateNewOrderDeadLine(company, orderSaveResponse);


                    if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                    {
                        batchName = orderDirectory;
                    }
                    else
                    {
                        batchName = Path.GetFileName(orderDirectory);

                    }

                    await RetryMoveBatchAfterOrderPlace(sourceFtpCredential, orderDirectory, companyGeneralSetting, isMoveSingleFile, orderDirectoryPaths);
                }

                else
                {
                    response.Message = orderSaveResponse.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Line No 630: " + ex.Message);
                response.Message = ex.Message;
                response.IsSuccess = false;

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Exception: {ex.Message}",
                    MethodName = "OrderPlaceFromFtpAndSendMail",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            //Mail Send to ops after order place
            if (companyGeneralSetting != null && companyGeneralSetting.CompanyId != 1181)
            {
                await SendEmailToOpsToNotifyOrderUpload(batchName, orderSaveResponse.Result.OrderNumber, company, sourceFtpCredential.RootFolder, sourceFtpCredential.UserName, allFilesFromNewOrder.Count());
            } 
            //company code
            // When order placed send email to client company
            if (companyGeneralSetting != null && companyGeneralSetting.IsOrderPlacedEmailSentToCompany)
            {
                var contacts = new List<ContactModel>
                {
                    new ContactModel { 
                        Email = company.Email ,
                        //Email = "zakir@thekowcompany.com",
                        FirstName = company.FirstName,
                        LastName = company.LastName,
                    }
                };
                var order = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
                await SendOrderPlacementEmailToClientCompany(order, company, contacts);
            }
            if (companyGeneralSetting != null && companyGeneralSetting.IsOrderPlacedEmailSentToCompanyAllUsers)
            {
                // get all company contact 
                var getAllContacts = await _contactManager.GetAll();
                var contacts = getAllContacts.Where(x=>x.CompanyId == company.Id).ToList();
                var order = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
                await SendOrderPlacementEmailToClientCompany(order, company, contacts);
            }
            response.IsSuccess = true;
            response.Result = orderSaveResponse.Result.OrderNumber;

            return response;
        }

        private async Task RetryMoveBatchAfterOrderPlace(FtpCredentailsModel sourceFtpCredential, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, bool isMoveSingleFile, List<string> orderDirectoryPaths)
        {
            if (companyGeneralSetting.OrderPlaceBatchMoveType != (short)OrderPlaceBatchMoveType.FileandFolderNotMoveAfterOrderPlace && !isMoveSingleFile)
            {
                int i = 0;
                while (true)
                {
                    try
                    {
                        await MoveBatchAfterOrderplaceOnFtp(sourceFtpCredential, orderDirectory, companyGeneralSetting, orderDirectoryPaths);
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
        }

        private async Task MoveBatchAfterOrderplaceOnFtp(FtpCredentailsModel sourceFtpCredential, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, List<string> orderDirectoryPaths)
        {
            FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();

            if (orderDirectoryPaths.Count > 0)
            {
                foreach (var orderDirectori in orderDirectoryPaths)
                {
                    using (var sourceClient = new AsyncFtpClient(sourceFtpCredential.Host,
                            sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))

                    {
                        sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        sourceClient.Config.ValidateAnyCertificate = true;
                        sourceClient.Encoding = System.Text.Encoding.UTF8;
                        await sourceClient.Connect();
                        var temp = "";

                        if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
                        {
                            temp = Path.GetDirectoryName(orderDirectori) + "/" + companyGeneralSetting.FtpFileMovedPathAfterOrderCreated + "/" + Path.GetFileName(orderDirectori);
                        }
                        else
                        {
                            temp = Path.GetDirectoryName(orderDirectori) + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/" + Path.GetFileName(orderDirectori);
                        }
                        temp = temp.Replace("\\", "/");
                        if (!await sourceClient.DirectoryExists(temp))
                        {
                            await sourceClient.CreateDirectory(temp);
                        }
                        await sourceClient.MoveDirectory(orderDirectori, temp, FtpRemoteExists.Overwrite);

                        // After file moved then create base directory for formatting 
                        if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                        {
                            await sourceClient.CreateDirectory($"{orderDirectori}");

                        }
                        await sourceClient.Disconnect();
                    }
                }
            }
            else
            {
                using (var sourceClient = new AsyncFtpClient(sourceFtpCredential.Host,
                      sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))

                {
                    sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    sourceClient.Config.ValidateAnyCertificate = true;
                    sourceClient.Encoding = System.Text.Encoding.UTF8;
                    await sourceClient.Connect();
                    var moveBatchDestinationPath = "";

                    if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
                    {
                        moveBatchDestinationPath = Path.GetDirectoryName(orderDirectory) + "/" + companyGeneralSetting.FtpFileMovedPathAfterOrderCreated + "/" + Path.GetFileName(orderDirectory);
                    }
                    else
                    {
                        moveBatchDestinationPath = Path.GetDirectoryName(orderDirectory) + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/" + Path.GetFileName(orderDirectory);
                    }

                    moveBatchDestinationPath = moveBatchDestinationPath.Replace("\\", "/");

                    //Create move able directory version 
                    if (await sourceClient.DirectoryExists(moveBatchDestinationPath))
                    {
                        var moveAbleDirectoryNewDestination = $"{moveBatchDestinationPath}_{DateTime.Now:yyyyMMdd_HHmmss}";
                        moveBatchDestinationPath = Path.Combine(moveBatchDestinationPath, moveAbleDirectoryNewDestination);
                    }

                    if (!await sourceClient.DirectoryExists(moveBatchDestinationPath))
                    {
                        await sourceClient.CreateDirectory(moveBatchDestinationPath);
                    }


                    await sourceClient.MoveDirectory(orderDirectory, moveBatchDestinationPath, FtpRemoteExists.Overwrite);

                    // After file moved then create base directory for formatting 
                    if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                    {
                        await sourceClient.CreateDirectory($"{orderDirectory}");

                    }

                    await sourceClient.Disconnect();


                }
            }

        }

        private string PrepareOrderFileDestinationPath(string uploadDirectory, string destinationFilePath)
        {
            string fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, destinationFilePath);
            fullFilePath = fullFilePath.Replace($"\\", @"/");
            fullFilePath = fullFilePath.Replace($"//", @"/");
            return fullFilePath;
        }

        private async Task MoveSourceFile(FtpCredentailsModel sourceFtpCredential, string path, AsyncFtpClient sourceClient,
            string[] pathArray, Response<long> addItemResponse, CompanyModel company)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string moveAblePath = sourceFtpCredential.RootFolder + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + pathArray[1];

                try
                {
                    if (!await sourceClient.DirectoryExists(Path.GetDirectoryName(moveAblePath)))
                    {
                        await sourceClient.CreateDirectory(Path.GetDirectoryName(moveAblePath));
                    }

                    bool responses = await sourceClient.MoveFile(path, moveAblePath, FtpRemoteExists.Overwrite);

                    if (!responses)
                    {
                        await _clientOrderItemService.Delete(addItemResponse.Result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    transactionScope.Dispose();
                    await _clientOrderItemService.Delete(addItemResponse.Result.ToString());

                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Path: {path}. Exception: {ex.Message}",
                        MethodName = "MoveSourceFile",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                }
            }
        }

        private async Task<bool> UploadOrderItemFromFtpSourcePath(string path, AsyncFtpClient sourceClient, AsyncFtpClient destinationClient,
            string fullFilePathForFtp, ClientOrderItemModel clientOrderItem, bool isDownloaded, FtpCredentailsModel sourceFtpCredential, CompanyModel company)
        {
            using (var writeStream = await destinationClient.OpenWrite(fullFilePathForFtp))
            {
                int maxRetries = 3;
                int retryCount = 0;
                bool success = false;
                //FtpStatus fileDownlaodStatus = FtpStatus.Failed;

                while (retryCount < maxRetries && !success)
                {
                    try
                    {
                        // Set up the progress tracking and speed calculation
                        var stopwatch = new Stopwatch();
                        DateTime lastTimestamp = DateTime.Now;
                        long lastBytes = 0;

                        var progress = new Progress<FtpProgress>(p =>
                        {
                            if (p.TransferredBytes > 0)
                            {
                                var currentTime = DateTime.Now;
                                double elapsedSeconds = (currentTime - lastTimestamp).TotalSeconds;

                                if (elapsedSeconds > 0)
                                {
                                    long bytesSinceLast = p.TransferredBytes - lastBytes;
                                    double transferRate = bytesSinceLast / elapsedSeconds;

                                    Console.WriteLine($"Progress: {p.Progress}% | Speed: {transferRate / 1024 / 1024:F2} MB/s");

                                    // Update for the next iteration
                                    lastTimestamp = currentTime;
                                    lastBytes = p.TransferredBytes;
                                }
                            }
                        });

                        stopwatch.Start();

                        // Define a cancellation token with a timeout (e.g., 10 minutes)
                        using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(120)))
                        {
                            // Asynchronous file download with progress tracking
                            //fileDownlaodStatus = await sourceClientFtp.DownloadFile(temporaryStorePath + "/" + Path.GetFileName(zipSourceFullPath),
                            //zipSourceFullPath, FtpLocalExists.Resume, FtpVerify.None, progress, cts.Token);

                            isDownloaded = await sourceClient.DownloadStream(writeStream, path, 0, progress, cts.Token);

                            stopwatch.Stop();
                            Console.WriteLine($"Download completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");

                            //if (fileDownlaodStatus == FtpStatus.Success || fileDownlaodStatus == FtpStatus.Skipped)
                            //{
                            //    success = true; // Mark download as successful

                            //    //isDownloaded = true;
                            //}

                            if (isDownloaded)
                            {
                                success = true; // Mark download as successful

                                //isDownloaded = true;
                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        retryCount++;
                        Console.WriteLine($"Timeout occurred, retrying... ({retryCount}/{maxRetries})");

                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CompanyObjectId = company.ObjectId,
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.OrderItem,
                            PrimaryId = (int)clientOrderItem.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Full Path: {fullFilePathForFtp}. Cancel Exception: {ex.Message}",
                            MethodName = "UploadOrderItemFromFtpSourcePath",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error downloading file: {ex.Message}");
                        retryCount++;

                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CompanyObjectId = company.ObjectId,
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.OrderItem,
                            PrimaryId = (int)clientOrderItem.Id,
                            ErrorMessage = $"CompanyId: {clientOrderItem.CompanyId}, Company Code: {company}. {sourceFtpCredential.GetLogDescription()}. Full Path: {fullFilePathForFtp}. Exception: {ex.Message}",
                            MethodName = "UploadOrderItemFromFtpSourcePath",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                }

                if (!success)
                {
                    Console.WriteLine("File download failed after multiple attempts.");
                }


            }

            if (isDownloaded)
            {
                await _updateOrderItemBLLService.UpdateOrderItemStatus(clientOrderItem, InternalOrderItemStatus.OrderPlaced);
            }


            return isDownloaded;
        }
        private async Task<bool> UploadOrderItemFromAppDomainPath(string path, AsyncFtpClient destinationClient, string fullFilePathForFtp,
            ClientOrderItemModel clientOrderItem, bool isDownloaded, FtpCredentailsModel destinationFtpCredentails, CompanyModel company)
        {
            int fileDownload = 0;
            while (true)
            {
                try
                {
                    FtpStatus status = FtpStatus.Failed;

                    //FtpStatus status = await destinationClient.UploadFile(path, fullFilePathForFtp, FtpRemoteExists.Overwrite);
                    using (FileStream fileStream = System.IO.File.OpenRead(path))
                    {
                        DateTime startTime = DateTime.Now;
                        Console.WriteLine("Donwload Start:{0}", fullFilePathForFtp);
                        status = await destinationClient.UploadStream(fileStream, fullFilePathForFtp);
                        //System.IO.File.Copy(path,fullFilePathForFtp,true);
                        var uploadingTime = (DateTime.Now - startTime).TotalSeconds;
                        Console.WriteLine("Donwload End:{0}", fullFilePathForFtp);
                        Console.WriteLine("Total Seconds:{0}", uploadingTime);
                        Console.WriteLine("File Size:{0}", clientOrderItem.FileSize);
                    }
                    if (status.Equals(FtpStatus.Success))
                    {
                        await _updateOrderItemBLLService.UpdateOrderItemStatus(clientOrderItem, InternalOrderItemStatus.OrderPlaced);
                        isDownloaded = true;
                        break;
                    }
                    else
                    {
                        fileDownload++;
                        if (fileDownload > 3)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {destinationFtpCredentails.GetLogDescription()}. Path: {path}. Exception: {ex.Message}",
                        MethodName = "UploadOrderItemFromAppDomainPath",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);


                    fileDownload++;
                    if (fileDownload > 3)
                    {
                        string methodName = $"File Transfer Error On Ftp Order Place {ex.Message.ToString()}";
                        byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;
                        await _errorLogService.LogGeneralError(ex, methodName, errorCategory);

                        break;
                    }
                }
            }
            return isDownloaded;
        }

        private async Task<ClientOrderItemModel> PrepareOrderItem(CompanyModel company, string path, Response<ClientOrderModel> orderSaveResponse, AsyncFtpClient sourceClient, string uploadDirectory, string destinationFilePath, string fullFilePath)
        {
            var clientOrderItem = new ClientOrderItemModel();
            DateTime arrivalTime = await sourceClient.GetModifiedTime(path);
            clientOrderItem.ArrivalTime = arrivalTime.AddHours(6); //Todo: Rakib , Hard coded time
            clientOrderItem.FileName = Path.GetFileName(path);
            clientOrderItem.FileType = Path.GetExtension(path);
            clientOrderItem.FileSize = await sourceClient.GetFileSize(path);
            clientOrderItem.ClientOrderId = orderSaveResponse.Result.Id;
            clientOrderItem.CompanyId = company.Id;
            var replaceString = Path.GetDirectoryName(destinationFilePath).Replace($"\\", @"/");
            if (replaceString == "/") { replaceString = ""; }
            else { replaceString = "/" + replaceString; }
            clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}{replaceString}";
            clientOrderItem.PartialPath = clientOrderItem.PartialPath.Replace("//", "/");
            clientOrderItem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePath);
            return clientOrderItem;
        }
        private async Task<string> CombineSourceRootPathWithOrderItemPath(FtpCredentailsModel sourceFtpCredential, string[] pathArray, string destinationFilePath, List<PathReplacementModel> pathReplacementList)
        {
            var facilityNameFromReplacePath = pathReplacementList.Where(x => x.Type == (int)PathReplacementType.TakeFacilityNameFromPath).FirstOrDefault();
            if (facilityNameFromReplacePath != null)
            {
                var takeFacilityName = await _pathReplacementService.Replace(sourceFtpCredential.RootFolder, pathReplacementList);
                destinationFilePath = takeFacilityName + "/" + pathArray[1];
            }
            else
            {
                destinationFilePath = Path.GetFileName(sourceFtpCredential.RootFolder) + "/" + pathArray[1];
            }
            return destinationFilePath;
        }
        private static async Task ConnnectDestinationClient(AsyncFtpClient destinationClient)
        {
            destinationClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
            destinationClient.Config.ValidateAnyCertificate = true;
            destinationClient.Encoding = System.Text.Encoding.UTF8;
            await destinationClient.Connect();
        }
        private static async Task ConnectSourceFtpClient(AsyncFtpClient sourceClient)
        {
            sourceClient.Encoding = System.Text.Encoding.UTF8;
            sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
            sourceClient.Config.ValidateAnyCertificate = true;
            await sourceClient.Connect();
        }

        private async Task AddOrderAttachmentFilesAndRemoveFromOrderableFiles(FtpCredentailsModel sourceFtpCredential, FtpCredentailsModel destinationFtpCredentails, List<string> allFilesFromNewOrder, CompanyModel company, bool isLocalFile, Response<ClientOrderModel> orderSaveResponse)
        {
            List<string> orderAttachmenttxtAndPdfFiles = allFilesFromNewOrder
            .Where(filePath => _orderAttachmentBLLService.IsTxtOrPdfFile(filePath))
            .ToList();

            allFilesFromNewOrder.RemoveAll(filePath => _orderAttachmentBLLService.IsTxtOrPdfFile(filePath));

            if (orderAttachmenttxtAndPdfFiles != null && orderAttachmenttxtAndPdfFiles.Any())
            {
                await _orderAttachmentBLLService.AddOrderAttachment(orderAttachmenttxtAndPdfFiles, orderSaveResponse.Result, company, sourceFtpCredential, destinationFtpCredentails, isLocalFile);
            }
        }
        private async Task UpdateOrderIsExtrafileAllowOnCompanySettings(CompanyGeneralSettingModel companyGeneralSetting, Response<ClientOrderModel> orderSaveResponse)
        {
            if (companyGeneralSetting.AllowExtraFile)
            {
                ClientOrderModel clientOrder = new ClientOrderModel
                {
                    Id = orderSaveResponse.Result.Id,
                    AllowExtraOutputFileUpload = true,
                };
                var isAllowed = await _orderService.UpdateOrderAllowExtraOutputFileUploadField(clientOrder);
            }
        }
        private static void RemoveNonOrderableFiles(List<string> allFilesFromNewOrder)
        {
            allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".db");
            allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".atn");
            allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".BridgeSort");
            allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".DS_Store");
            allFilesFromNewOrder.RemoveAll(path => Path.GetFileName(path).StartsWith("._")); //Add for klm . later we add it from db
        }
        private async Task<Response<string>> CopyFilesFromLocalToFTP(ClientExternalOrderFTPSetupModel sourceFtpCredential,
            FtpCredentailsModel destinationFtpCredentails, List<string> allFilesFromNewOrder, CompanyModel company,
            FileServerModel fileServer, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, (string filePath, DateTime LastWriteTime, long FileSize) fileInfo, bool isMoveSingleFile = false, bool isLocalFile = false)
        {
            var response = new Response<string>();
            var orderSaveResponse = new Response<ClientOrderModel>();
            List<string> orderDirectoryPaths = new List<string>();
            //Bath name send to ops mail . 
            string batchName = "";
            FtpCredentailsModel sourceFtpCredentials = CreateExternalOrderFTPSetupCredentials(sourceFtpCredential);
            try
            {
                allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".db");
                if (allFilesFromNewOrder.Any())
                {
                    orderSaveResponse = await AddOrderInfo(company, fileServer, sourceFtpCredential.Id, orderDirectory);
                }
                if (orderSaveResponse.IsSuccess)
                {
                    if (companyGeneralSetting.AllowExtraFile)
                    {
                        ClientOrderModel clientOrder = new ClientOrderModel
                        {
                            Id = orderSaveResponse.Result.Id,
                            AllowExtraOutputFileUpload = true,
                        };

                        var isAllowed = await _orderService.UpdateOrderAllowExtraOutputFileUploadField(clientOrder);
                    }
                    List<string> orderAttachmenttxtAndPdfFiles = allFilesFromNewOrder
                    .Where(filePath => _orderAttachmentBLLService.IsTxtOrPdfFile(filePath))
                    .ToList();

                    allFilesFromNewOrder.RemoveAll(filePath => _orderAttachmentBLLService.IsTxtOrPdfFile(filePath));

                    if (orderAttachmenttxtAndPdfFiles != null && orderAttachmenttxtAndPdfFiles.Any())
                    {
                        await _orderAttachmentBLLService.AddOrderAttachment(orderAttachmenttxtAndPdfFiles, orderSaveResponse.Result, company, sourceFtpCredentials, destinationFtpCredentails);
                    }

                    var semaphore = new SemaphoreSlim(15);

                    var tasks = allFilesFromNewOrder.Select(async path =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            int retryCount = 0;
                            bool uploadSuccessful = false;

                            while (!uploadSuccessful && retryCount < 3)
                            {
                                try
                                {
                                    FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
                                    bool isImageAlreadyExist = false;

                                    //Check if this file exist or not , if client file not move to donwloaded
                                    if (companyGeneralSetting.OrderPlaceBatchMoveType == (short)OrderPlaceBatchMoveType.FileandFolderNotMoveAfterOrderPlace)
                                    {
                                        var orderItem = await _clientOrderItemService.GetItemByImageNameAndCompanyId(Path.GetFileName(path), company.Id);
                                        if (orderItem != null && orderItem.Id > 0)
                                        {
                                            isImageAlreadyExist = true;

                                        }

                                    }

                                    if (!isImageAlreadyExist)
                                    {
                                        //SftpClient sourceClient = await _sshNetService.CreateSshNetConnector(true, sourceFtpCredential);


                                        //sourceClient.OperationTimeout = TimeSpan.FromMinutes(50);
                                        //using (var sourceClient = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                                        using (var destinationClient = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
                                        {
                                            //sourceClient.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                                            //sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                            //sourceClient.Config.ValidateAnyCertificate = true;
                                            //await sourceClient.Connect();

                                            destinationClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                            destinationClient.Encoding = System.Text.Encoding.UTF8;
                                            destinationClient.Config.ValidateAnyCertificate = true;
                                            await destinationClient.Connect();

                                            // using (var tempStream = await sourceClient.OpenRead(path))
                                            //{
                                            // Prepare the destination path



                                            var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, orderSaveResponse.Result.CreatedDate, orderSaveResponse.Result.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);
                                            string[] pathArray = new string[1000];
                                            string destinationFilePath = "";

                                            // zehetu path divided ftp root folder , so akhane root folder null execption dhora holo.
                                            if (!string.IsNullOrWhiteSpace(sourceFtpCredential.InputRootFolder) || uploadDirectory != null && isLocalFile)
                                            {
                                                if (uploadDirectory != null && isLocalFile)
                                                {
                                                    pathArray = path.Split(AutomatedAppConstant.extractParentFolder);
                                                }
                                                else
                                                {
                                                    pathArray = path.Split(sourceFtpCredential.InputRootFolder);

                                                }
                                            }
                                            var pathReplacementList = await _pathReplacementService.GetPathReplacements(company.Id);
                                            //When a company order need Batch Parent Folder 
                                            if (companyGeneralSetting.IsBatchRootFolderNameAddWithOrder)
                                            {
                                                var facilityNameFromReplacePath = pathReplacementList.Where(x => x.Type == (int)PathReplacementType.TakeFacilityNameFromPath).FirstOrDefault();
                                                if (facilityNameFromReplacePath != null)
                                                {

                                                    var takeFacilityName = await _pathReplacementService.Replace(sourceFtpCredential.InputRootFolder, pathReplacementList);

                                                    destinationFilePath = takeFacilityName + "/" + pathArray[1];
                                                }
                                                else
                                                {
                                                    destinationFilePath = Path.GetFileName(sourceFtpCredential.InputRootFolder) + "/" + pathArray[1];
                                                }
                                            }
                                            // If ftp root is null then assign path.
                                            else if (string.IsNullOrWhiteSpace(sourceFtpCredential.InputRootFolder) || sourceFtpCredential.InputRootFolder == "/")
                                            {
                                                destinationFilePath = path;

                                            }
                                            else
                                            {
                                                destinationFilePath = pathArray[1];
                                            }

                                            var fullFilePathForFtp = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, destinationFilePath);

                                            if (!string.IsNullOrWhiteSpace(destinationFtpCredentails.SubFolder))
                                            {
                                                fullFilePathForFtp = $"{destinationFtpCredentails.SubFolder}/{fullFilePathForFtp}";
                                            }
                                            else
                                            {
                                                fullFilePathForFtp = fullFilePathForFtp;
                                            }
                                            Console.WriteLine($"Upload to Ftp : {Path.GetFileName(path)}");

                                            // Set OrderDirectory Path 
                                            if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                                            {
                                                orderDirectory = Path.GetDirectoryName(path);
                                                if (!orderDirectoryPaths.Contains(orderDirectory))
                                                {
                                                    orderDirectoryPaths.Add(orderDirectory);
                                                }

                                            }

                                            var startUpload = DateTime.Now;
                                            Console.WriteLine(startUpload);

                                            if (!await destinationClient.DirectoryExists(Path.GetDirectoryName(fullFilePathForFtp)))
                                            {
                                                await destinationClient.CreateDirectory(Path.GetDirectoryName(fullFilePathForFtp));
                                            }


                                            // AddOrder Item
                                            ClientOrderItemModel clientOrderItem = new ClientOrderItemModel();


                                            // Arrival Time
                                            DateTime arrivalTime = fileInfo.LastWriteTime;
                                            clientOrderItem.ArrivalTime = arrivalTime.AddHours(6);

                                            clientOrderItem.FileName = Path.GetFileName(path);
                                            clientOrderItem.FileType = Path.GetExtension(path);
                                            clientOrderItem.FileSize = fileInfo.FileSize;
                                            clientOrderItem.ClientOrderId = orderSaveResponse.Result.Id;
                                            clientOrderItem.CompanyId = company.Id;

                                            var replaceString = Path.GetDirectoryName(destinationFilePath).Replace($"\\", @"/");
                                            if (replaceString == "/") { replaceString = ""; }
                                            else { replaceString = "/" + replaceString; }
                                            clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}{replaceString}";
                                            clientOrderItem.PartialPath = clientOrderItem.PartialPath.Replace("//", "/");
                                            var fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, destinationFilePath);
                                            var fullFilePathReplace = fullFilePath.Replace($"\\", @"/");
                                            fullFilePathReplace = fullFilePathReplace.Replace($"//", @"/");
                                            clientOrderItem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePathReplace);


                                            var addItemResponse = await AddOrderItem(clientOrderItem, company, path, orderSaveResponse.Result.Id, sourceFtpCredential.GetInputLogDescription(), InternalOrderItemStatus.OrderPlaced);

                                            uploadSuccessful = addItemResponse.IsSuccess;
                                            clientOrderItem.Id = addItemResponse.Result;

                                            //Add Order Item End 

                                            bool isDownloaded = false;
                                            if (isLocalFile)
                                            {

                                                int fileDownload = 0;
                                                while (true)
                                                {
                                                    try
                                                    {
                                                        FtpStatus status = FtpStatus.Failed;
                                                        //FtpStatus status = await destinationClient.UploadFile(path, fullFilePathForFtp, FtpRemoteExists.Overwrite);
                                                        using (FileStream fileStream = System.IO.File.OpenRead(path))
                                                        {
                                                            Console.WriteLine("Donwload Start:{0}", fullFilePathForFtp);
                                                            status = await destinationClient.UploadStream(fileStream, fullFilePathForFtp);
                                                            //System.IO.File.Copy(path,fullFilePathForFtp,true);
                                                            Console.WriteLine("Donwload End:{0}", fullFilePathForFtp);

                                                        }
                                                        if (status.Equals(FtpStatus.Success))
                                                        {
                                                            await _updateOrderItemBLLService.UpdateOrderItemStatus(clientOrderItem, InternalOrderItemStatus.OrderPlaced);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            fileDownload++;
                                                            if (fileDownload > 3)
                                                            {
                                                                break;
                                                            }
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                                        {
                                                            CreatedByContactId = AutomatedAppConstant.ContactId,
                                                            CompanyObjectId = company.ObjectId,
                                                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                                                            PrimaryId = company.Id,
                                                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetInputLogDescription()}. Path: {path}. File Transfer Error On Ftp Order Place. Exception: {ex.Message}",
                                                            MethodName = "CopyFilesFromLocalToFTP->TaskLoop",
                                                            RazorPage = "OrderPlaceService",
                                                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                                        };

                                                        await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                                        fileDownload++;
                                                        if (fileDownload > 3)
                                                        {
                                                            break;
                                                        }

                                                    }

                                                }

                                            }
                                            else
                                            {
                                                using (var writeStream = await destinationClient.OpenWrite(fullFilePathForFtp))
                                                {
                                                    int fileDownload = 0;
                                                    while (true)
                                                    {
                                                        try
                                                        {
                                                            isDownloaded = await destinationClient.DownloadStream(writeStream, path);


                                                            if (isDownloaded)
                                                            {
                                                                break;
                                                            }

                                                            else
                                                            {
                                                                Thread.Sleep(1000);
                                                                fileDownload++;
                                                                if (fileDownload > 3)
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                                            {
                                                                CreatedByContactId = AutomatedAppConstant.ContactId,
                                                                CompanyObjectId = company.ObjectId,
                                                                ActivityLogFor = (int)ActivityLogForConstants.Company,
                                                                PrimaryId = company.Id,
                                                                ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {destinationFtpCredentails.GetLogDescription()}. Path: {path}. File Transfer Error On Ftp Order Place. Exception: {ex.Message}",
                                                                MethodName = "CopyFilesFromLocalToFTP->DownloadStream",
                                                                RazorPage = "OrderPlaceService",
                                                                Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                                            };

                                                            await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                                            Thread.Sleep(1000);
                                                            fileDownload++;

                                                            if (fileDownload > 3)
                                                            {
                                                                break;
                                                            }

                                                        }

                                                    }

                                                }
                                            }

                                            var uploadFinish = DateTime.Now;
                                            Console.WriteLine(uploadFinish.Subtract(startUpload).TotalSeconds);

                                            if (isDownloaded)
                                            {
                                                //if (isMoveSingleFile)
                                                //{
                                                //    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                                                //    {
                                                //        string moveAblePath = sourceFtpCredential.RootFolder + "/_downloaded" + pathArray[1];

                                                //        try
                                                //        {


                                                //            if (!await sourceClient.DirectoryExists(Path.GetDirectoryName(moveAblePath)))
                                                //            {
                                                //                await sourceClient.CreateDirectory(Path.GetDirectoryName(moveAblePath));
                                                //            }

                                                //            bool responses = await sourceClient.MoveFile(path, moveAblePath, FtpRemoteExists.Overwrite);

                                                //            if (!responses)
                                                //            {
                                                //                await _clientOrderItemService.Delete(addItemResponse.Result.ToString());

                                                //            }
                                                //        }
                                                //        catch (Exception ex)
                                                //        {
                                                //            transactionScope.Dispose();
                                                //            await _clientOrderItemService.Delete(addItemResponse.Result.ToString());

                                                //        }
                                                //    }
                                                //}
                                            }
                                            else
                                            {
                                                await _clientOrderItemService.Delete(addItemResponse.Result.ToString());
                                            }
                                            //}



                                            // sourceClient.Disconnect();
                                            await destinationClient.Disconnect();
                                            // Here compare bytes source to destination.
                                            if (!isLocalFile)
                                            {
                                                var compareBytes = await CreateBytesAndCompareFromPaths(sourceFtpCredentials, path, destinationFtpCredentails, fullFilePath, company);
                                            }
                                        }
                                        break;

                                    }
                                    else
                                    {
                                        break;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                    {
                                        CreatedByContactId = AutomatedAppConstant.ContactId,
                                        CompanyObjectId = company.ObjectId,
                                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                                        PrimaryId = company.Id,
                                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetInputLogDescription()}. Path: {path}. Exception: {ex.Message}",
                                        MethodName = "CopyFilesFromLocalToFTP->TaskLoop",
                                        RazorPage = "OrderPlaceService",
                                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                    };

                                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                    retryCount++;
                                    Thread.Sleep(1000);

                                    if (retryCount >= 3)
                                    {
                                        Console.WriteLine(ex.ToString());
                                        break;
                                    }
                                }


                            }
                        }
                        finally
                        {
                            //semaphore.Release();
                        }

                    }).ToArray();
                    await Task.WhenAll(tasks);
                    //If Previous any order in Order Placing

                    bool isOrderUpdated = await _orderStatusService.UpdateOrderStatus(orderSaveResponse.Result, AutomatedAppConstant.ContactId);
                    bool isArrivalTimeUpdate = await _orderStatusService.UpdateOrderArrivalTime(orderSaveResponse.Result);
                    if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                    {
                        batchName = orderDirectory;
                    }
                    else
                    {
                        batchName = Path.GetFileName(orderDirectory);
                    }
                }
                else
                {
                    response.Message = orderSaveResponse.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Line No 630: " + ex.Message);
                response.Message = ex.Message;
                response.IsSuccess = false;

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetInputLogDescription()}. Exception: {ex.Message}",
                    MethodName = "CopyFilesFromLocalToFTP",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            //Delete Order Place if order does not contain any images 
            var newOrder = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
            if (newOrder.NumberOfImage == 0)
            {
                await _orderService.Delete(newOrder.ObjectId);

                response.IsSuccess = false;
                response.Result = "";
                return response;
            }
            await SendEmailToOpsToNotifyOrderUpload(batchName, orderSaveResponse.Result.OrderNumber, company);
            response.IsSuccess = true;
            response.Result = orderSaveResponse.Result.OrderNumber;

            return response;
        }
        private List<List<string>> GetChunksOfPaths(List<string> allFilePath, int chunkSize)
        {
            var chunks = new List<List<string>>();
            int count = 0;
            var chunk = new List<string>();

            foreach (var tempPath in allFilePath)
            {
                count++;
                chunk.Add(tempPath);
                if (count == chunkSize)
                {
                    chunks.Add(chunk);
                    chunk = new List<string>();
                    count = 0;
                }
            }

            if (count > 0)
            {
                chunks.Add(chunk);
            }
            return chunks;
        }

        private async Task<List<SftpFile>> SFTPLast24HoursDirectories(ClientExternalOrderFTPSetupModel sourceFtp, string remoteDirectory)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();
            var directories = sftpClient.ListDirectory(remoteDirectory);
            var last24Hours = DateTime.Now.AddHours(-24);
            var directoryList = directories.Where(d => d.IsDirectory && d.LastWriteTime > last24Hours).ToList();
            sftpClient.Disconnect();

            return directoryList;

        }
        private async Task<List<SftpFile>> SFTPLast24HoursFiles(ClientExternalOrderFTPSetupModel sourceFtp, string remoteDirectory)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();
            var directories = sftpClient.ListDirectory(remoteDirectory);
            var last24Hours = DateTime.Now.AddHours(-24);
            var directoryList = directories.Where(d => d.LastWriteTime > last24Hours).ToList();
            sftpClient.Disconnect();

            return directoryList;

        }

        private async Task ProvideStorageFolderForOrderProcess(SftpFile item, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel destinationFtp, FileServerModel fileServer, SemaphoreSlim semaphoreSlim)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();
            await Task.Run(async () =>
            {
                try
                {
                    Response<string> copyResponse = new Response<string>();

                    if (!string.IsNullOrEmpty(item.Name) && item.IsDirectory && item.Name != "." && item.Name != ".." && item.Name != AutomatedAppConstant.DefaultOrderPlacedFileContainer)
                    {
                        var orderDirectory = $"{sourceFtp.InputRootFolder}/{item.Name}";

                        var allFilePath = await GetAllFilePaths(sftpClient, orderDirectory, sourceFtp, company);

                        // here remove path which file already exists in order.
                        if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
                        {
                            foreach (var filePath in allFilePath)
                            {
                                var withoutRootPath = await RemoveInputRootFolder(sourceFtp.InputRootFolder, filePath);
                                var existingFilePath = await _clientOrderItemService.CheckClientOrderItemFile(company.Id, withoutRootPath, Path.GetFileName(filePath), DateTime.Now.Date.ToString());
                                if (existingFilePath)
                                {
                                    allFilePath.Remove(filePath);
                                }
                            }
                        }
                        if (companyGeneralSetting.CheckUploadCompletedFlagOnFile)
                        {
                            if (allFilePath.Exists(fp => fp.ToLower().Contains(companyGeneralSetting.HotKeyFileName.ToLower())))
                            {
                                allFilePath.RemoveAll(fp => fp.ToLower().Contains(companyGeneralSetting.HotKeyFileName.ToLower()));

                                copyResponse = await CopyFilesAndNotify(destinationFtp, allFilePath, company, fileServer, sourceFtp, orderDirectory, companyGeneralSetting, false);
                            }
                        }
                        if (companyGeneralSetting.IsFtpIdleTimeChecking)
                        {
                            // Filter ftp idle time when get all file path.
                            copyResponse = await CopyFilesAndNotify(destinationFtp, allFilePath, company, fileServer, sourceFtp, orderDirectory, companyGeneralSetting, false);
                        }

                        if (companyGeneralSetting.CheckUploadCompletedFlagOnBatchName && !string.IsNullOrEmpty(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
                        {

                            if (item.Name.Contains(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
                            {
                                // var copyResponse = await CopyFilesFromOneFtpToAnotherFTP(sourceFtpCredential, kowToMateFtpCredentails, allFilePath, company, fileServer, orderDirectory, companyGeneralSetting);
                                copyResponse = await CopyFilesAndNotify(destinationFtp, allFilePath, company, fileServer, sourceFtp, orderDirectory, companyGeneralSetting, false);
                            }
                        }
                        if (copyResponse.IsSuccess)
                        {
                            await SendEmailToOpsToNotifyOrderUpload(item.Name, copyResponse.Result, company, orderDirectory, sourceFtp.Username, allFilePath.Count());
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Exception: {ex.Message}",
                        MethodName = "ProvideStorageFolderForOrderProcess",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });
        }
        private static FtpCredentailsModel ConvertExternalClientFtpToFtpCreadentials(ClientExternalOrderFTPSetupModel sourceFtp)
        {
            FtpCredentailsModel sourceFtpCredential = new FtpCredentailsModel();
            sourceFtpCredential.Id = sourceFtp.Id;
            sourceFtpCredential.Host = sourceFtp.Host;
            sourceFtpCredential.UserName = sourceFtp.Username;
            sourceFtpCredential.Password = sourceFtp.Password;
            sourceFtpCredential.RootFolder = sourceFtp.InputRootFolder;
            sourceFtpCredential.Port = sourceFtp.Port;
            return sourceFtpCredential;
        }

        private async Task<string> RemoveInputRootFolder(string RootFolderPath, string FullPath)
        {
            if (FullPath.Contains(RootFolderPath))
            {
                string output = FullPath.Replace(RootFolderPath, "");
                return output;
            }
            else
            {
                Console.WriteLine("Substring not found in the input.");
                return FullPath;
            }
        }
        private async Task<Response<string>> CopyFilesAndNotify(FtpCredentailsModel destinationFtp, List<string> allFilePath, CompanyModel company,
            FileServerModel fileServer, ClientExternalOrderFTPSetupModel sourceFtp, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, bool isMoveSingleFile)
        {
            var response = new Response<string>();

            try
            {
                var copyResponse = await CopyFilesFromOneFtpToAnotherSFTP(sourceFtp, destinationFtp, allFilePath, company, fileServer, orderDirectory, sourceFtp.Id, sourceFtp.InputRootFolder, companyGeneralSetting, isMoveSingleFile);

                if (copyResponse.IsSuccess)
                {
                    response.IsSuccess = true;
                    response.Result = copyResponse.Result;
                }
            }
            catch (Exception ex)
            {
                string methodName = "CopyFilesAndNotify";
                byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

                await _errorLogService.LogFtpProcessingError(ex, methodName, category);

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {destinationFtp.GetLogDescription()}. Exception: {ex.Message}",
                    MethodName = "CopyFilesAndNotify",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);


                response.IsSuccess = false;
            }

            return response;
        }

        private async Task<Response<ClientOrderModel>> AddOrderInfo(CompanyModel company, FileServerModel fileServer, long sourceFtpId, string orderDirectory = "")
        {
            var response = new Response<ClientOrderModel>();
            var order = new ClientOrderModel();
            try
            {
                if (order.Id > 0)
                {
                    response.Message = "Order already have an id.";
                    return response;
                }

                order.CreatedByContactId = AutomatedAppConstant.ContactId;
                order.UpdatedByContactId = AutomatedAppConstant.ContactId;
                order.SourceServerId = sourceFtpId;
                Thread.Sleep(2000);
                var dateTime = DateTime.Now;
                order.OrderNumber = $"{company.Code}-{company.Id}-{dateTime.ToString("ddMMyyyyHHmmss")}";
                Console.WriteLine(order.OrderNumber);
                order.ObjectId = Guid.NewGuid().ToString();
                order.CreatedDate = DateTime.Now;
                order.UpdatedDate = DateTime.Now;
                order.OrderPlaceDate = DateTime.Now;
                order.CompanyId = company.Id;
                order.ExternalOrderStatus = (byte)EnumHelper.ExternalOrderStatusChange(InternalOrderStatus.OrderPlacing);
                order.InternalOrderStatus = (byte)InternalOrderStatus.OrderPlacing;
                order.FileServerId = fileServer.Id;
                order.OrderType = (int)OrderType.NewWork;
                order.BatchPath = orderDirectory;
                var companyTeam = await _companyTeamService.GetByCompanyId(company.Id);

                if (companyTeam != null && companyTeam.Count > 0)
                {
                    var getFirstOrDefaultCompany = companyTeam.FirstOrDefault();
                    order.AssignedTeamId = getFirstOrDefaultCompany.TeamId;
                }
                else
                {
                    order.AssignedTeamId = null;
                }

                var addResponse = await _orderService.Insert(order);

                if (!addResponse.IsSuccess)
                {
                    response.Message = addResponse.Message;
                    response.IsSuccess = false;
                    return response;
                }

                order.Id = addResponse.Result;
                response.IsSuccess = true;
                response.Result = order;

                await _statusChangeLogBLLService.AddOrderStatusChangeLog(order, InternalOrderStatus.OrderPlacing, AutomatedAppConstant.ContactId);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. File Server: {fileServer.Name}. Exception: {ex.Message}",
                    MethodName = "AddOrderInfo",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return response;
        }

        private async Task SendOrderPlacementEmailToClientCompany(ClientOrderModel order, CompanyModel company, List<ContactModel> contacts)
        {
            try
            {
                var orderDetailURL = _configuration["GeneralSettings:KTMBaseURL"];
                OrderPlacementNotificationEmailModel fTPOrderNotifyOpsOnImageArrivalFTP = new OrderPlacementNotificationEmailModel
                {
                    OrderDetailURL = orderDetailURL + "/order/Details/" + order.ObjectId,
                    Contacts = contacts,
                    CompanyName = $"{company.Name}",
                    OrderNumber = order.OrderNumber,
                };
                await _workflowEmailService.SendOrderPlacementEmailsToClientCompany(fTPOrderNotifyOpsOnImageArrivalFTP);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    MethodName = "SendOrderPlacementEmailToClientCompany",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
        }



        private async Task SendEmailToOpsToNotifyOrderUpload(string batchName, string orderNumber, CompanyModel company, string folderName = " ",
            string userName = " ", int numberOfImages = 0)
        {
            try
            {
                //ToDo: Rakib need to add from db
                List<string> opsEmailList = new List<string>()
                            {
                                "rakibul@thekowcompany.com",
								//"anik@thekowcompany.com",
								//"zico@thekowcompany.com",
								//"raihan@thekowcompany.com",
								"ops@thekowcompany.com",
                                "mashfiq@thekowcompany.com",
								//"ak@thekowcompany.com",
								"zakir@thekowcompany.com"
                            };
                FTPOrderNotifyOpsOnImageArrivalFTP fTPOrderNotifyOpsOnImageArrivalFTP = new FTPOrderNotifyOpsOnImageArrivalFTP
                {
                    EmailAddresses = opsEmailList,
                    MailType = "OrderPlaceOnKTM",
                    //ImageCount = $"{imageCount}  ,  UserName:{clientFtp.Username}",

                    //OrderType = $"NAN",
                    CompanyName = $"{company.Name}",
                    BatchName = $"{batchName} ,Folder Name:{folderName}   , Username:{userName}",
                    OrderNumber = $"{orderNumber} , Image Count: {(numberOfImages > 0 ? numberOfImages.ToString() : "N/A")}",
                };
                await _workflowEmailService.SendEmailToOpsToNotifyOrderUpload(fTPOrderNotifyOpsOnImageArrivalFTP);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. Batch Name: {batchName}. Order Number: {orderNumber}. Exception: {ex.Message}",
                    MethodName = "SendEmailToOpsToNotifyOrderUpload",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
        }

        private async Task<bool> CreateBytesAndCompareFromPaths(FtpCredentailsModel sourceFtpCredential, string sourcePath, FtpCredentailsModel destinationFtpCredentails,
            string destinationPath, CompanyModel company)
        {
            var result = false;
            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
                    using (var sourceClient = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                    using (var destinationClient = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                    {
                        sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        sourceClient.Config.ValidateAnyCertificate = true;
                        await sourceClient.Connect();

                        destinationClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        destinationClient.Config.ValidateAnyCertificate = true;
                        await destinationClient.Connect();

                        // Here compare file bytes 
                        CancellationToken cancellationToken = CancellationToken.None;
                        byte[] sourceBytes = await sourceClient.DownloadBytes(sourcePath, cancellationToken);
                        var destinationBytes = await destinationClient.DownloadBytes(destinationPath, cancellationToken);

                        var byteCompare = await CompareByteArraysAsync(sourceBytes, destinationBytes);
                        if (!byteCompare)
                        {
                            using (var writeStream = await destinationClient.OpenWrite(sourcePath))
                            {
                                var isDownloaded = false;
                                try
                                {
                                    isDownloaded = await sourceClient.DownloadStream(writeStream, destinationPath);

                                    if (isDownloaded)
                                    {
                                        result = true;
                                        break; // Break out of the retry loop on success
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                    {
                                        CreatedByContactId = AutomatedAppConstant.ContactId,
                                        CompanyObjectId = company.ObjectId,
                                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                                        PrimaryId = company.Id,
                                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Source Path: {sourcePath}. Exception: {ex.Message}",
                                        MethodName = "CreateBytesAndCompareFromPaths",
                                        RazorPage = "OrderPlaceService",
                                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                    };

                                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                    // Handle the exception if needed
                                }
                            }
                        }
                        else
                        {
                            result = true;
                            break; // Break out of the retry loop on success
                        }
                        await sourceClient.Disconnect();
                        await destinationClient.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Source Path: {sourcePath}. Exception: {ex.Message}",
                        MethodName = "CreateBytesAndCompareFromPaths",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                    // Handle the exception if needed
                    retryCount++;
                }
                retryCount++;
            }
            return result;
        }


        private async Task UpdateNewOrderDeadLine(CompanyModel company, Response<ClientOrderModel> orderSaveResponse)
        {
            var clientFtp = await _clientExternalOrderFTPSetupService.GetById((int)orderSaveResponse.Result.SourceServerId);

            if (clientFtp != null && clientFtp.DeliveryDeadlineInMinute >= 0)
            {
                orderSaveResponse.Result.DeliveryDeadlineInMinute = (int)clientFtp.DeliveryDeadlineInMinute;
            }
            else if (company != null && company.DeliveryDeadlineInMinute >= 0)
            {
                orderSaveResponse.Result.DeliveryDeadlineInMinute = company.DeliveryDeadlineInMinute;
            }

            if (orderSaveResponse.Result.DeliveryDeadlineInMinute > 0)
            {
                DateTime arrivalDateTime = orderSaveResponse.Result.ArrivalTime ?? orderSaveResponse.Result.CreatedDate;
                orderSaveResponse.Result.ExpectedDeliveryDate = arrivalDateTime.AddMinutes((int)orderSaveResponse.Result.DeliveryDeadlineInMinute);

                if (orderSaveResponse.Result.ExpectedDeliveryDate != null)
                {
                    await _clientOrderItemService.UpdateOrderItemExpectedDeliveryDate(orderSaveResponse.Result.Id, orderSaveResponse.Result.ExpectedDeliveryDate);
                }
                await _orderService.UpdateOrderDeadline(orderSaveResponse.Result);
                await _orderService.UpdateOrderDeadlineDate(orderSaveResponse.Result.Id);
            }
        }
        private async Task<Response<string>> CopyFilesFromOneFtpToAnotherSFTP(ClientExternalOrderFTPSetupModel sourceFtp,
        FtpCredentailsModel destinationFtpCredentails, List<string> allFilesFromNewOrder,
        CompanyModel company, FileServerModel fileServer, string orderDirectory,
        long sourceFtpServiceId, string rootFolder, CompanyGeneralSettingModel? companyGeneralSetting = null, bool isMoveSingleFile = true)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(4);
            sftpClient.Connect();
            var response = new Response<string>();
            var orderSaveResponse = new Response<ClientOrderModel>();
            // Remove Thumbs File or bat files
            if (allFilesFromNewOrder != null && allFilesFromNewOrder.Any())
            {
                allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".db");
            }
            //Create Order 
            if (allFilesFromNewOrder != null && allFilesFromNewOrder.Any())
            {
                orderSaveResponse = await AddOrderInfo(company, fileServer, sourceFtpServiceId, orderDirectory);
            }
            if (!orderSaveResponse.IsSuccess)
            {
                response.Message = orderSaveResponse.Message;
                return response;
            }
            //If company all order allow extra file then update it 
            //companyGeneralSetting = await _companyGeneralSettingService.GetAllGeneralSettingsByCompanyId(company.Id);

            if (companyGeneralSetting.AllowExtraFile)
            {
                ClientOrderModel clientOrder = new ClientOrderModel
                {
                    Id = orderSaveResponse.Result.Id,
                    AllowExtraOutputFileUpload = true,
                };

                var isAllowed = await _orderService.UpdateOrderAllowExtraOutputFileUploadField(clientOrder);
            }
            var successUploadFileCount = 0;
            try
            {
                // Image Path wise thread
                string orderPlaceSFtpFilesThread = _configuration.GetSection(GeneralSettingsKey)["OrderPlaceSFtpFilesThread"];
                SemaphoreSlim semaphoreSlim = new SemaphoreSlim(int.Parse(orderPlaceSFtpFilesThread));
                var uploadTasks = new List<Task<Response<int>>>();

                // Chunk Create for files upload.
                var chunkSize = CalculateChunkSize(allFilesFromNewOrder.Count);
                var filesChunks = GetFilesChunksWithPaths(allFilesFromNewOrder, chunkSize);
                try
                {
                    foreach (var chunk in filesChunks)
                    {
                        await semaphoreSlim.WaitAsync();
                        //Todo:Rakib sftpClient unessary send here
                        uploadTasks.Add(FileUploadAndFileMoveForOrderItemInSFTP(chunk, sourceFtp, destinationFtpCredentails,
                            company, orderSaveResponse.Result, rootFolder, orderSaveResponse, semaphoreSlim, isMoveSingleFile, companyGeneralSetting));

                    }
                    var FileUploadResponses = await Task.WhenAll(uploadTasks);
                    for (int i = 0; i < FileUploadResponses.Length; i++)
                    {
                        successUploadFileCount = FileUploadResponses[i].Result + successUploadFileCount;
                    }
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Root Folder: {rootFolder}. Exception: {ex.Message}",
                        MethodName = "CopyFilesFromOneFtpToAnotherSFTP",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                    response.Message = ex.Message;
                }
                finally
                {
                    semaphoreSlim.Release();
                }
                if (!isMoveSingleFile)
                {
                    if (companyGeneralSetting.OrderPlaceBatchMoveType != (short)OrderPlaceBatchMoveType.FileandFolderNotMoveAfterOrderPlace)
                    {
                        //if (!companyGeneralSetting.IsOrderCreatedThenFileMove)
                        //{
                        ClientOrderModel clientOrder = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
                        var newOrderItems = await _clientOrderItemService.GetAllOrderItemByOrderId(clientOrder.Id);
                        if (clientOrder.NumberOfImage == allFilesFromNewOrder.Count && !newOrderItems.Any(item => item.Status == (int)InternalOrderItemStatus.OrderPlacing))
                        {
                            await PerformOrderUpdatesAsync(orderSaveResponse);
                            await StorageFolderMoving(orderDirectory, sourceFtp, company, isMoveSingleFile);
                            // Previous folder root folder create 
                            if (companyGeneralSetting.isFtpFolderPreviousStructureWiseStayInFtp)
                            {
                                sftpClient.CreateDirectory(orderDirectory);
                            }
                            response.IsSuccess = true;
                            response.Result = orderSaveResponse.Result.OrderNumber;
                        }
                    }
                    else
                    {
                        ClientOrderModel clientOrder = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
                        if (clientOrder.NumberOfImage == allFilesFromNewOrder.Count)
                        {
                            await PerformOrderUpdatesAsync(orderSaveResponse);
                            response.IsSuccess = true;
                            response.Result = orderSaveResponse.Result.OrderNumber;
                        }
                    }
                }
                else
                {
                    ClientOrderModel clientOrder = await _orderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
                    await PerformOrderUpdatesAsync(orderSaveResponse);
                    response.IsSuccess = true;
                    response.Result = orderSaveResponse.Result.OrderNumber;
                }
                //Mail Send to ops after order place
                if (successUploadFileCount == allFilesFromNewOrder.Count && companyGeneralSetting != null && companyGeneralSetting.CompanyId != 1181)
                {
                    await SendEmailToOpsToNotifyOrderUpload(rootFolder, orderSaveResponse.Result.OrderNumber, company, sourceFtp.InputRootFolder, sourceFtp.Username, allFilesFromNewOrder.Count());
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Root Folder: {rootFolder}. Exception: {ex.Message}",
                    MethodName = "CopyFilesFromOneFtpToAnotherSFTP",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        private async Task PerformOrderUpdatesAsync(Response<ClientOrderModel> orderSaveResponse)
        {
            await _orderStatusService.UpdateOrderStatus(orderSaveResponse.Result, AutomatedAppConstant.ContactId);
            await _orderStatusService.UpdateOrderArrivalTime(orderSaveResponse.Result);
        }
        private async Task<List<string>> GetAllFilePaths(SftpClient sftpClient, string orderDirectory, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company)
        {
            var allFilePath = new List<string>();
            try
            {
                var files = sftpClient.ListDirectory(orderDirectory);
                await _sshNetService.RecursiveListFiles(sftpClient, orderDirectory, allFilePath);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Order Directory: {orderDirectory}. Exception: {ex.Message}",
                    MethodName = "GetAllFilePaths",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);

            }
            return allFilePath;
        }
        private async Task<bool> CompareByteArraysAsync(byte[] sourceBytesArray, byte[] destinationBytesArray)
        {

            if (sourceBytesArray.Length != destinationBytesArray.Length)
            {
                return false;
            }
            return await Task.Run(() =>
            {
                for (int i = 0; i < sourceBytesArray.Length; i++)
                {
                    if (sourceBytesArray[i] != destinationBytesArray[i])
                    {
                        return false;
                    }
                }
                return true;
            });
        }
        private int CalculateChunkSize(int totalFiles)
        {
            return totalFiles < 200 ? 3 : 2; // Adjust the values as needed
        }

        private async Task<Response<int>> FileUploadAndFileMoveForOrderItemInSFTP(List<string> chunk, ClientExternalOrderFTPSetupModel sourceFtp,
            FtpCredentailsModel destinationFtpCredentails, CompanyModel company, ClientOrderModel order,
            string rootFolder, Response<ClientOrderModel> orderSaveResponse, SemaphoreSlim semaphoreSlim, bool isMoveSingleFile, CompanyGeneralSettingModel? companyGeneralSetting = null
            )
        {
            Response<int> response = new Response<int>();
            await Task.Run(async () =>
            {
                try
                {
                    int retryCount = 0;
                    bool uploadSuccessful = false;

                    while (!uploadSuccessful && retryCount < 3)
                    {
                        try
                        {
                            FtpConfig ftpConfig = new FtpConfig
                            {
                                ConnectTimeout = 1000 * 60 * 10,
                                EncryptionMode = FtpEncryptionMode.Auto,
                                ValidateAnyCertificate = true,
                                TransferChunkSize = 1024 * 1024 * 10,
                                ReadTimeout = 1000 * 60 * 10,
                                DataConnectionConnectTimeout = 1000 * 60 * 10,
                                LocalFileBufferSize = 1024 * 1024 * 10,  //Change this and try again
                                RetryAttempts = 5,
                                UploadRateLimit = 0
                            };
                            //using ()
                            using (var destinationClient = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
                            {
                                await destinationClient.AutoConnect();
                                foreach (var path in chunk)
                                {

                                    var FileUploadResponse = await FileUploadFromSFTPToFtpAndInsertOrderItem(sourceFtp, destinationFtpCredentails, company, rootFolder, orderSaveResponse, uploadSuccessful, destinationClient, path);

                                    if (FileUploadResponse.IsSuccess && companyGeneralSetting.OrderPlaceBatchMoveType == (int)OrderPlaceBatchMoveType.FileandFolderMoveAfterOrderPlace && isMoveSingleFile)
                                    {
                                        response.Result = response.Result + 1;
                                        await _sshNetService.SingleFileMove(sourceFtp, path);
                                    }
                                    response.IsSuccess = FileUploadResponse.IsSuccess;
                                }
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                            {
                                CreatedByContactId = AutomatedAppConstant.ContactId,
                                CompanyObjectId = company.ObjectId,
                                ActivityLogFor = (int)ActivityLogForConstants.Company,
                                PrimaryId = company.Id,
                                ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Root Folder: {rootFolder}. Exception: {ex.Message}",
                                MethodName = "FileUploadAndFileMoveForOrderItemInSFTP",
                                RazorPage = "OrderPlaceService",
                                Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                            };

                            await _activityAppLogService.InsertAppErrorActivityLog(activity);

                            retryCount++;
                            Thread.Sleep(1000);
                            if (retryCount >= 3)
                            {
                                Console.WriteLine(ex.ToString());
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
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Root Folder: {rootFolder}. Exception: {ex.Message}",
                        MethodName = "FileUploadAndFileMoveForOrderItemInSFTP",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
            );
            return response;
        }
        private List<List<string>> GetFilesChunksWithPaths(List<string> allFiles, int chunkSize)
        {
            var chunks = new List<List<string>>();

            int count = 0;
            var chunk = new List<string>();
            int i = 0;
            foreach (var filePath in allFiles)
            {

                count++;
                chunk.Add(filePath);

                if (count == chunkSize)
                {
                    chunks.Add(chunk);
                    chunk = new List<string>();
                    count = 0;
                }

                i++;
            }
            if (count > 0)
            {
                chunks.Add(chunk);
            }

            return chunks;
        }

        private async Task StorageFolderMoving(string orderDirectory, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, bool isMovingSingleFile = false)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();
            int i = 0;

            while (true)
            {
                try
                {
                    var moveableFolder = "";

                    if (!isMovingSingleFile)
                    {
                        moveableFolder = Path.GetDirectoryName(orderDirectory) + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/"; // The new directory path to create
                    }
                    else
                    {
                        moveableFolder = orderDirectory + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/"; // The new directory path to create
                    }
                    moveableFolder = moveableFolder.Replace("\\", "/");
                    if (!sftpClient.Exists(moveableFolder))
                    {
                        //sourceClient.CreateDirectory(moveableFolder);
                        await _sshNetService.RecursiveCreateDirectories(sftpClient, moveableFolder);
                    }
                    var temp = "";

                    if (!isMovingSingleFile)
                    {
                        temp = Path.GetDirectoryName(orderDirectory) + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/" + Path.GetFileName(orderDirectory);
                    }

                    else
                    {
                        temp = orderDirectory + "/" + AutomatedAppConstant.DefaultOrderPlacedFileContainer + "/" + Path.GetFileName(orderDirectory);
                    }

                    temp = temp.Replace("\\", "/");

                    if (!sftpClient.Exists(temp))
                    {
                        await _sshNetService.RecursiveCreateDirectories(sftpClient, moveableFolder);
                    }
                    await _sshNetService.RecursiveListFilesMove(sftpClient, orderDirectory, temp);

                    if (!isMovingSingleFile)
                    {
                        try
                        {
                            await _sshNetService.RecursiveDeleteDiretories(sftpClient, orderDirectory);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                    break;

                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                    {
                        CreatedByContactId = AutomatedAppConstant.ContactId,
                        CompanyObjectId = company.ObjectId,
                        ActivityLogFor = (int)ActivityLogForConstants.Company,
                        PrimaryId = company.Id,
                        ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Order Directory: {orderDirectory}. Exception: {ex.Message}",
                        MethodName = "StorageFolderMoving",
                        RazorPage = "OrderPlaceService",
                        Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                    };

                    await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    //End log

                    i++;
                    await Task.Delay(1000);
                    if (i > 3)
                    {
                        break;
                    }
                }
            }
        }
        private async Task<Response<bool>> FileUploadFromSFTPToFtpAndInsertOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, FtpCredentailsModel destinationFtpCredentails, CompanyModel company, string rootFolder, Response<ClientOrderModel> orderSaveResponse, bool uploadSuccessful, AsyncFtpClient destinationClient, string path)
        {
            var orderItemAddToDbandUploadToStorage = 0;
            var response = new Response<bool>();
            uploadSuccessful = false;
            try
            {
                Console.WriteLine("Start Read" + DateTime.Now);
                var readStartTime = DateTime.Now;

                var companyGeneralSetting = await _companyGeneralSettingService.GetGeneralSettingByCompanyId(company.Id);

                // Prepare the destination path
                var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, orderSaveResponse.Result.CreatedDate, orderSaveResponse.Result.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);
                var pathArray = path.Split(rootFolder);

                if (companyGeneralSetting.IsBatchRootFolderNameAddWithOrder)
                {
                    pathArray[1] = Path.GetFileName(sourceFtp.InputRootFolder) + pathArray[1];// Path array one means order place folder structure from source ftp
                }

                var fullFilePathForFtp = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, pathArray[1]);

                if (!string.IsNullOrWhiteSpace(destinationFtpCredentails.SubFolder))
                {
                    fullFilePathForFtp = $"{destinationFtpCredentails.SubFolder}/{fullFilePathForFtp}";
                }



                Console.WriteLine($"Upload to Ftp : {Path.GetFileName(path)}");
                // Insert order item or file
                var addOrderItemResponse = await InsertOrderItem(sourceFtp, company, rootFolder, orderSaveResponse, uploadSuccessful, destinationClient, path, uploadDirectory, pathArray, fullFilePathForFtp);

                uploadSuccessful = addOrderItemResponse.IsSuccess;

                SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
                sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
                sftpClient.Connect();

                int maxRetries = 3;
                int retryOrderItemUpload = 0;

                while (retryOrderItemUpload < maxRetries)
                {
                    try
                    {
                        if (!await destinationClient.DirectoryExists(Path.GetDirectoryName(fullFilePathForFtp)))
                        {
                            await destinationClient.CreateDirectory(Path.GetDirectoryName(fullFilePathForFtp));
                        }

                        using (var streamToWrite = await destinationClient.OpenWrite(fullFilePathForFtp))
                        {
                            int maxDownloadRetries = 3;
                            int fileDownload = 0;

                            try
                            {
                                Console.WriteLine("Start Read" + DateTime.Now);
                                var readStartTimeTemp = DateTime.Now;

                                //File download on this method
                                sftpClient.DownloadFile(path, streamToWrite);
                                var uploadDoneTimeTemp = DateTime.Now;
                                Console.WriteLine("Upload Finish" + uploadDoneTimeTemp.Subtract(readStartTimeTemp).TotalMinutes);
                            }
                            catch (Exception ex)
                            {
                                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                {
                                    CreatedByContactId = AutomatedAppConstant.ContactId,
                                    CompanyObjectId = company.ObjectId,
                                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                                    PrimaryId = company.Id,
                                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Path: {path}. Exception: {ex.Message}",
                                    MethodName = "FileUploadFromSFTPToFtpAndInsertOrderItem -> DownloadFile",
                                    RazorPage = "OrderPlaceService",
                                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                                };

                                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                fileDownload++;

                                if (fileDownload >= maxDownloadRetries)
                                {
                                    uploadSuccessful = false;

                                    // If download retries are exhausted, break out of the outer retry loop
                                    break;
                                }
                                else
                                {
                                    // If download fails, wait for a moment before retrying
                                    Thread.Sleep(3000);
                                }
                            }

                        }

                        #region File compare to destination to local

                        //var fileBytesArray = sftpClient.ReadAllBytes(path);
                        long fileSize = 0;
                        var fileAttributes = sftpClient.GetAttributes(path);
                        if (fileAttributes != null)
                        {
                            fileSize = fileAttributes.Size;
                        }


                        //var result = await VerifyDownloadedFile(destinationFtpCredentails, fileBytesArray, fullFilePathForFtp);
                        var result = fileSize == (long)await destinationClient.GetFileSize(fullFilePathForFtp) ? true : false;

                        if (!result)
                        {
                            //If verification fails, increment the retry counter
                            retryOrderItemUpload++;
                            uploadSuccessful = false;
                            response.IsSuccess = false;
                        }
                        else
                        {
                            //If verification is successful, break out of the retry loop
                            await _updateOrderItemBLLService.UpdateOrderItemStatus(addOrderItemResponse.Result, InternalOrderItemStatus.OrderPlaced);
                            uploadSuccessful = true;
                            response.IsSuccess = true;
                            break;
                        }


                        #endregion
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            CompanyObjectId = company.ObjectId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = company.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Path: {path}. Exception: {ex.Message}",
                            MethodName = "FileUploadFromSFTPToFtpAndInsertOrderItem",
                            RazorPage = "OrderPlaceService",
                            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);

                        // If any other exception occurs, wait for a moment before retrying
                        Thread.Sleep(3000);
                        retryOrderItemUpload++;

                        //if (retryOrderItemUpload >= maxRetries)
                        //{
                        //    string methodName = "AddOrderItemAsync 1";
                        //    byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

                        //    await _errorLogService.LogFtpProcessingError(ex, methodName, category);
                        //}
                    }
                }

                //break;

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtp.GetInputLogDescription()}. Path: {path}. Exception: {ex.Message}",
                    MethodName = "FileUploadFromSFTPToFtpAndInsertOrderItem",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                Thread.Sleep(3000);
                orderItemAddToDbandUploadToStorage++;
                if (orderItemAddToDbandUploadToStorage >= 3)
                {
                    response.IsSuccess = false;
                    return response;
                }
            }
            return response;
        }
        private async Task<Response<ClientOrderItemModel>> InsertOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, string rootFolder, Response<ClientOrderModel> orderSaveResponse, bool uploadSuccessful, AsyncFtpClient destinationClient, string path, string uploadDirectory, string[] pathArray, string fullFilePathForFtp)
        {
            var response = new Response<ClientOrderItemModel>();

            ClientOrderItemModel clientOrderItem = await PrepareClientOrderItem(sourceFtp, company, orderSaveResponse, path, uploadDirectory, pathArray);
            SftpClient sourceClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sourceClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sourceClient.Connect();
            Console.WriteLine($"Add Item call {clientOrderItem.FileName} ");
            var addItemResponse = await AddOrderItem(clientOrderItem, company, path, orderSaveResponse.Result.Id, sourceFtp.GetInputLogDescription(), InternalOrderItemStatus.OrderPlacing);
            response.IsSuccess = addItemResponse.IsSuccess;
            response.Result = clientOrderItem;

            return response;
        }

        //private async Task<bool> VerifyDownloadedFile(FtpCredentails destinationFtpCredentails, byte[] remoteFileBytes, string localFtpPath)
        //{
        //    try
        //    {
        //        FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
        //        using (var destinationClients = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
        //        {

        //            destinationClients.Config.EncryptionMode = FtpEncryptionMode.Auto;
        //            destinationClients.Config.ValidateAnyCertificate = true;
        //            await destinationClients.Connect();
        //            // Here compare file bytes 
        //            CancellationToken cancellationToken = CancellationToken.None;
        //            var path = localFtpPath;
        //            var destinationBytes = await destinationClients.DownloadBytes(path, cancellationToken);
        //            await destinationClients.Disconnect();
        //            var checkByteEqual = ByteArrayCompare(destinationBytes, remoteFileBytes);
        //            return checkByteEqual;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
        //        {
        //            CreatedByContactId = AutomatedAppConstant.ContactId,
        //            CompanyObjectId = company.ObjectId,
        //            ActivityLogFor = (int)ActivityLogForConstants.Company,
        //            PrimaryId = company.Id,
        //            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {sourceFtpCredential.GetLogDescription()}. Zip Source Path: {zipSourceFullPath}. Exception: {ex.Message}",
        //            MethodName = "VerifyDownloadedFile",
        //            RazorPage = "OrderPlaceService",
        //            Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //        };

        //        await _activityAppLogService.InsertAppErrorActivityLog(activity);

        //        Console.WriteLine(ex.InnerException.ToString());
        //        return false;
        //    }
        //}

        /// <summary>
        /// Compare localFile to RemoteFile
        /// </summary>
        /// <param name="localFileBytesArray"></param>
        /// <param name="remoteFileBytesArray"></param>
        /// <returns></returns>
        bool ByteArrayCompare(byte[] localFileBytesArray, byte[] remoteFileBytesArray)
        {
            if (localFileBytesArray.Length != remoteFileBytesArray.Length)
                return false;

            for (int i = 0; i < localFileBytesArray.Length; i++)
            {
                if (localFileBytesArray[i] != remoteFileBytesArray[i])
                {
                    //Console.WriteLine($"Difference found at index {i}: array1[{i}] = {localFileBytesArray[i]}, array2[{i}] = {remoteFileBytesArray[i]}");
                    //return false;
                }
            }
            return true;
        }
        private async Task<ClientOrderItemModel> PrepareClientOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, Response<ClientOrderModel> orderSaveResponse, string path, string uploadDirectory, string[] pathArray)
        {
            SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();
            // AddOrder Item
            ClientOrderItemModel clientOrderItem = new ClientOrderItemModel();
            // Arrival Time
            if (company.Id == AutomatedAppConstant.VcCompanyId)
            {
                DateTime arrivalTime = sftpClient.GetLastWriteTime(path);
                clientOrderItem.ArrivalTime = arrivalTime.AddHours(6);
            }
            clientOrderItem.FileName = Path.GetFileName(path);
            clientOrderItem.FileType = Path.GetExtension(path);
            SftpFileAttributes attributes = sftpClient.GetAttributes(path);
            if (attributes.IsRegularFile)
            {
                long fileSize = attributes.Size;
                clientOrderItem.FileSize = fileSize;
            }
            clientOrderItem.ClientOrderId = orderSaveResponse.Result.Id;
            clientOrderItem.CompanyId = company.Id;

            string orderItemFileSourceDiretoryWithFileName = pathArray[1];
            string orderItemFileSourceDirectory = Path.GetDirectoryName(orderItemFileSourceDiretoryWithFileName);
            orderItemFileSourceDirectory = orderItemFileSourceDirectory.Replace($"\\", @"/");

            if (orderItemFileSourceDirectory == "/") { orderItemFileSourceDirectory = ""; }

            if (company.Id == 1182) // this is for mnm 
            {
                clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}/{orderItemFileSourceDirectory}";
            }
            else
            {
                clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}{orderItemFileSourceDirectory}";
            }

            var fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, pathArray[1]);
            var fullFilePathReplace = fullFilePath.Replace($"\\", @"/");
            fullFilePathReplace = fullFilePathReplace.Replace($"//", @"/");
            clientOrderItem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePathReplace);
            return clientOrderItem;
        }

        #endregion

        #region Method those are still in FtpOrderProcessService
        private async Task<DateTime> GetMaxModifiedTime(AsyncFtpClient ftp, string directoryPath, CompanyModel company, ClientExternalOrderFTPSetupModel ftpCredentials)
        {
            DateTime maxArrivalTime = new DateTime();
            try
            {
                FtpListItem[] ftpListItems = await ftp.GetListing(directoryPath);
                if (ftpListItems.Length > 0)
                {
                    var imageFiles = ftpListItems.Where(entry => entry.Type == FtpObjectType.File && entry.Name != "Thumbs.db");
                    maxArrivalTime = imageFiles.Max(file => file.Modified);
                }

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {ftpCredentials.GetInputLogDescription()}. Directory Path: {directoryPath}. Exception: {ex.Message}",
                    MethodName = "GetMaxModifiedTime",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return maxArrivalTime;
        }

        private static void DeleteFolder(string temporaryStorePath)
        {
            string[] files = Directory.GetFiles(temporaryStorePath);
            foreach (string file in files)
            {
                System.IO.File.Delete(file);
            }

            string[] subdirectories = Directory.GetDirectories(temporaryStorePath);
            foreach (string subdirectory in subdirectories)
            {
                Directory.Delete(subdirectory, true);
            }


            Directory.Delete(temporaryStorePath);
        }

        private async Task<Response<long>> AddOrderItem(ClientOrderItemModel clientOrderItem, CompanyModel company, string clientStorageFilePath,
            long orderId, string ftpLogDescription, InternalOrderItemStatus status = 0)
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

                var companyTeam = await _companyTeamService.GetByCompanyId(company.Id);
                if (companyTeam != null)
                {
                    var getFirstOrDefaultCompany = companyTeam.FirstOrDefault();

                    if (clientOrderItem.TeamId != null && clientOrderItem.TeamId > 0)
                    {
                        clientOrderItem.TeamId = getFirstOrDefaultCompany.TeamId;
                    }
                }
                //Detect and Set Category 
                int categoryId = await _categorySetService.DetectOrderItemCategory(clientStorageFilePath, clientOrderItem.CompanyId);
                await PrepareOrderItemCategoryInformation(clientOrderItem, categoryId);
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

                        if (clientOrderItem.CategoryId != null && clientOrderItem.CategoryId > 0)
                        {
                            await OrderItemCategoryLogAdd(clientOrderItem);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. {ftpLogDescription}. Client Storage FilePath: {clientStorageFilePath}. Exception: {ex.Message}",
                    MethodName = "AddOrderItem",
                    RazorPage = "OrderPlaceService",
                    Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return addItemResponse;
        }

        private async Task OrderItemCategoryLogAdd(ClientOrderItemModel clientOrderItem)
        {
            ClientCategoryChangeLogModel clientCategoryChangeLog = new ClientCategoryChangeLogModel
            {
                ClientCategoryId = (int)clientOrderItem.CategoryId,
                ClientOrderItemId = clientOrderItem.Id,
                CategorySetByContactId = AutomatedAppConstant.ContactId,
                CategorySetDate = DateTime.Now,
            };

            await _activityAppLogService.ClientCategoryChangeLogInsert(clientCategoryChangeLog);
        }

        private async Task PrepareOrderItemCategoryInformation(ClientOrderItemModel clientOrderItem, int categoryId)
        {
            if (categoryId != 0)
            {
                var clientCategory = await _clientCategoryService.GetById(categoryId);

                clientOrderItem.CategoryId = clientCategory.Id;
                clientOrderItem.CategorySetByContactId = AutomatedAppConstant.ContactId;
                clientOrderItem.CategorySetDate = DateTime.Now;
                clientOrderItem.CategoryPrice = clientCategory.PriceInUSD;
                clientOrderItem.CategorySetStatus = (byte)ItemCategorySetStatus.Auto_set;
                clientOrderItem.CategoryApprovedByContactId = AutomatedAppConstant.ContactId;

            }

            else
            {
                clientOrderItem.CategorySetStatus = (byte)ItemCategorySetStatus.Not_set;
                clientOrderItem.CategoryApprovedByContactId = AutomatedAppConstant.ContactId;
            }
        }
        #endregion
    }
}
