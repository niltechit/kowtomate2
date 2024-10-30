using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.ErrorLogServices;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.Models.Security;
using FluentFTP;
using System.IO.Compression;
using CutOutWiz.Services.AutomationAppServices.FtpOrderProcess;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.PathReplacementServices;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.OrderDeliveryAutomation
{
    public class OrderDeliveryService : IOrderDeliveryService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly IErrorLogService _errorLogService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IAutoOrderDeliveryService _autoOrderDeliveryService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        private readonly ICompanyManager _companyService;
        private readonly IPathReplacementService _pathReplacementService;
        public OrderDeliveryService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            ICompanyGeneralSettingManager companyGeneralSettingService,
            IErrorLogService errorLogService,
            IClientOrderItemService clientOrderItemService,
            IAutoOrderDeliveryService autoOrderDeliveryService,
            IUpdateOrderItemBLLService updateOrderItemBLLService,
            ICompanyManager companyService,
            IPathReplacementService pathReplacementService
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
            _companyGeneralSettingService = companyGeneralSettingService;
            _errorLogService = errorLogService;
            _clientOrderItemService = clientOrderItemService;
            _autoOrderDeliveryService = autoOrderDeliveryService;
            _updateOrderItemBLLService = updateOrderItemBLLService;
            _companyService = companyService;
            _pathReplacementService = pathReplacementService;
        }

        public async Task<Response<bool>> DeliveryOrderToClientStorage(int consoleAppId)
        {
            try
            {
                string query = $"SELECT cgs.* from  dbo.CompanyGeneralSettings cgs where cgs.EnableOrderDeliveryToFtp = 1 and cgs.FtpOrderPlacedAppNo={consoleAppId}";//need to chagne
                
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);

                if (companyGeneralSettings == null || !companyGeneralSettings.Any())
                {
                    return new Response<bool>();
                }

                foreach (var companyGeneralSetting in companyGeneralSettings)
                {
                    if (companyGeneralSetting != null)
                    {
                        List<ClientOrderModel> orders = new List<ClientOrderModel>();

                        if (companyGeneralSetting.AllowPartialDelivery)
                        {
                            orders = await _orderService.GetOrdersByOrderItemStatus(companyGeneralSetting.CompanyId, "21");
                        }
                        else
                        {
                            orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.Delivered, companyGeneralSetting.CompanyId);
                        }

                        if (orders != null && orders.Any())
                        {
                            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
                            var orderDeliveryTasks = new List<Task>();

                            orders = orders.OrderBy(order => order.NumberOfImage).ToList();

                            foreach (var order in orders)
                            {
                                await semaphoreSlim.WaitAsync();

                                orderDeliveryTasks.Add(Task.Run(async () =>
                                {

                                    try
                                    {
                                        if (order.SourceServerId != null)
                                        {
                                            var orderFtpQuery = $"select * from Client_ClientOrderFtp where Id ={order.SourceServerId}";
                                            var clientOrderFtp = await _fileServerService.GetClientFtpByQuery(orderFtpQuery);

                                            if (clientOrderFtp != null)
                                            {
                                                try
                                                {
                                                    string queryForOrderItem = $"SELECT * From Order_ClientOrderItem AS ci where ci.ClientOrderId=${order.Id} and ci.Status in (21) and FileGroup={(int)OrderItemFileGroup.Work}";
                                                    var orderItems = await _clientOrderItemService.GetOrderItemByStatus(queryForOrderItem);
                                                    Console.WriteLine(order.OrderNumber);

                                                    Response<List<ClientOrderItemModel>> response = new Response<List<ClientOrderItemModel>>();


                                                    bool isSuccessfullyUploaded = false;
                                                    var certainFtpClientGivenBatchFileButReturnZipFile = false;
                                                    if (clientOrderFtp.InputRootFolder.ToUpper().Contains("LL_BEAN")) //Todo: Zakir
                                                    {
                                                        certainFtpClientGivenBatchFileButReturnZipFile = true;
                                                    }

                                                    //Zip Delivery Check for client
                                                    if (certainFtpClientGivenBatchFileButReturnZipFile || order.BatchPath != null && order.BatchPath.Contains(".zip") && companyGeneralSetting.DeliveryType == (short)DeliveryType.ZipToZip)
                                                    {
                                                        // Check For KLM Company 
                                                        //if (companyGeneralSetting.CompanyId == 1201)
                                                        //{

                                                        //    var totalFileSizeLength = orderItems.Sum(x => x.FileSize);
                                                        //    if (totalFileSizeLength > 1500000000)
                                                        //    {
                                                        //        long? totalFileLength = 0;
                                                        //        var orderItemList = new List<ClientOrderItem>();

                                                        //        foreach (var item in orderItems)
                                                        //        {
                                                        //            if (totalFileLength > 1500000000)
                                                        //            {
                                                        //                break;
                                                        //            }
                                                        //            totalFileLength = totalFileLength + item.FileSize;
                                                        //            orderItemList.Add(item);
                                                        //        }

                                                        //        isSuccessfullyUploaded = await MoveOrderFromTemporaryStorageToClientFtp(orderItemList, order, clientOrderFtp, companyGeneralSetting);
                                                        //        if (isSuccessfullyUploaded)
                                                        //        {
                                                        //            response.Result = orderItems;
                                                        //        }
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        isSuccessfullyUploaded = await MoveOrderFromTemporaryStorageToClientFtp(orderItems, order, clientOrderFtp, companyGeneralSetting);
                                                        //        if (isSuccessfullyUploaded)
                                                        //        {
                                                        //            response.Result = orderItems;
                                                        //        }
                                                        //    }
                                                        //}
                                                        //else
                                                        //{

                                                        string appRoot1 = AppDomain.CurrentDomain.BaseDirectory;
                                                        string temporaryStorePath1 = Path.Combine(appRoot1, "TemporaryStoreForDelivery/");
                                                        if (Directory.Exists(temporaryStorePath1))
                                                        {
                                                            DeleteFolder(Path.Combine(temporaryStorePath1));
                                                        }

                                                        isSuccessfullyUploaded = await MoveOrderFromTemporaryStorageToClientFtp(orderItems, order, clientOrderFtp, companyGeneralSetting);

                                                        if (isSuccessfullyUploaded)
                                                        {
                                                            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
                                                            string temporaryStorePath = Path.Combine(appRoot, "TemporaryStoreForDelivery/");
                                                            DeleteFolder(Path.Combine(temporaryStorePath));
                                                            response.Result = orderItems;
                                                        }
                                                        //}

                                                    }

                                                    else
                                                    {
                                                        // checking sent output to separate storage or FTP or SFTP
                                                        var protocolType = clientOrderFtp.InputProtocolType;
                                                        if (clientOrderFtp.SentOutputToSeparateFTP)
                                                        {
                                                            protocolType = clientOrderFtp.OutputProtocolType;
                                                        }

                                                        if (protocolType == (int)InputProtocolTypeEnum.FTP)
                                                        {
                                                            response = await _autoOrderDeliveryService.MoveOrderToClientFtp(orderItems, order, clientOrderFtp, companyGeneralSetting);
                                                        }
                                                        else
                                                        {
                                                            response = await _autoOrderDeliveryService.MoveOrderToClientSFtp(orderItems, order, clientOrderFtp);
                                                        }
                                                    }

                                                    if (response.Result != null && response.Result.Count() > 0)
                                                    {
                                                        await _updateOrderItemBLLService.UpdateOrderItemsStatus(response.Result, InternalOrderItemStatus.Completed);
                                                        await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
                                                        var company = await _companyService.GetById(companyGeneralSetting.CompanyId);
                                                        //await SendEmailToOpsToNotifyOrderDeliveryToClient(order.OrderNumber, company, response.Result.Count.ToString());
                                                        // Here send completed flag file for client ftp
                                                        if (companyGeneralSetting.IsSendClientHotkey)
                                                        {
                                                            var FindOrder = await _orderService.GetById(order.Id);
                                                            if (FindOrder.InternalOrderStatus == (int)InternalOrderStatus.Completed)
                                                            {
                                                                string appRoot = AppDomain.CurrentDomain.BaseDirectory;
                                                                var hotKeyPath = "";
                                                                if (!string.IsNullOrWhiteSpace(companyGeneralSetting.HotkeyFlagFileName))
                                                                {
                                                                    hotKeyPath = Path.Combine(appRoot, "Keys", companyGeneralSetting.HotkeyFlagFileName);
                                                                }
                                                                // Get batch name from Path
                                                                var batchName = await _pathReplacementService.TakeBatchNameFromPath(order.BatchPath, 2);
                                                                // Send Hotkey 
                                                                await _autoOrderDeliveryService.SendHotkeyFileToFtp(hotKeyPath, batchName, clientOrderFtp);
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    var errorMessage = $"Console App ID: {consoleAppId}, CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. {clientOrderFtp.GetInputLogDescription()} Exception: {ex.Message}";
                                                    await AddExceptionForOrderLogs((int)order.Id, errorMessage);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorMessage = $"Console App ID: {consoleAppId}, CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. Exception: {ex.Message}";
                                        await AddExceptionForOrderLogs((int)order.Id, errorMessage);
                                    }
                                    finally
                                    {
                                        semaphoreSlim.Release();
                                    }

                                }));
                            }
                            await Task.WhenAll(orderDeliveryTasks);
                        }
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
                    ErrorMessage = $"Console App ID: {consoleAppId}, Exception: {ex.Message}",
                    MethodName = "DeliveryOrderToClientStorage",
                    RazorPage = "OrderDeliveryService",
                    Category = (int)ActivityLogCategory.OrderDeliveryToClient,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return new Response<bool>();
        }
        #region Same method exist on another service

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

        public async Task<bool> MoveOrderFromTemporaryStorageToClientFtp(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order, ClientExternalOrderFTPSetupModel tempClientOrderFtp, CompanyGeneralSettingModel companyGeneralSetting = null)
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            string temporaryStorePath = Path.Combine(appRoot, "TemporaryStoreForDelivery/");
            var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
            var isSucessfullyMoved = false;
            var host = serverInfo.Host.Split(':');

            if (host.Length == 3)
            {
                serverInfo.Host = $"{host[0]}:{host[1]}";
            }

            var sourceFtpCredential = new FileServerViewModel()
            {
                Host = serverInfo.Host,
                UserName = serverInfo.UserName,
                Password = serverInfo.Password,
                Port = serverInfo.Port
            };

            FtpConfig ftpConfig = new FtpConfig { ConnectTimeout = 900000 };
            var orderItemsFileCount = clientOrderItems.Count;
            var downloadedFilesCount = 0;

            foreach (var orderItem in clientOrderItems)
            {
                var sourcePath = orderItem.InternalFileOutputPath;

                if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                {
                    sourcePath = $"{serverInfo.SubFolder}/{sourcePath}";
                }
                string[] pathArray = new string[400];

                if (orderItem.InternalFileOutputPath != null)
                {
                    pathArray = orderItem.InternalFileOutputPath.Split("/Completed");
                }

                var finalPathArray = pathArray[1].Split(order.OrderNumber.ToString());

                using (var sourceClientFtp = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                {
                    //sourceClientFtp.Encoding = Encoding.GetEncoding("ISO-8859-1");
                    sourceClientFtp.Encoding = System.Text.Encoding.UTF8;
                    sourceClientFtp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    sourceClientFtp.Config.ValidateAnyCertificate = true;
                    await sourceClientFtp.Connect();

                    temporaryStorePath = temporaryStorePath.Replace("\\", "/");

                    try
                    {
                        if (!Directory.Exists(temporaryStorePath))
                        {
                            Directory.CreateDirectory(temporaryStorePath);
                        }

                        // Here Substract path and remove facility name.

                        var downloadPath = "";

                        if (companyGeneralSetting.RemoveFacilityNameFromOutputRootFolderPath)
                        {
                            // Get Path Replacement List
                            var pathReplacements = await _pathReplacementService.GetPathReplacements(companyGeneralSetting.CompanyId);
                            var replaceFacilityIndex = pathReplacements.Where(x => x.Type == (int)PathReplacementType.SubstractDuplicateFacilityNameFromPath).FirstOrDefault();

                            var splitedPathArray = finalPathArray[1].Split("/");
                            var ftpOutputRootPathSplitedPath = tempClientOrderFtp.OutputRootFolder.Split("/");
                            // here changes logic To Do:
                            if (ftpOutputRootPathSplitedPath[3] == splitedPathArray[1])
                            {
                                for (int i = 2; i < splitedPathArray.Length; i++)
                                {
                                    downloadPath = downloadPath + "/" + splitedPathArray[i];
                                }
                            }
                        }
                        else
                        {
                            downloadPath = finalPathArray[1];
                        }

                        await sourceClientFtp.DownloadFile(temporaryStorePath + "/" + downloadPath, sourcePath);
                        await sourceClientFtp.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()}, InternalFileOutputPath: {orderItem.InternalFileOutputPath} Exception: {ex.Message}";

                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.Order,
                            PrimaryId = (int)order.Id,
                            ErrorMessage = errorMessage,
                            MethodName = "MoveOrderFromTemporaryStorageToClientFtp->orderItemLoop",
                            RazorPage = "OrderDeliveryService",
                            Category = (int)ActivityLogCategory.OrderDeliveryToClient,
                        };
                        await _activityAppLogService.InsertAppErrorActivityLog(activity);

                        // Log the exception or handle it appropriately
                        Console.WriteLine($"Error creating directory: {ex.Message}");
                        return false;
                    }
                }
            }

            string destinationFolder = temporaryStorePath + "ZipPath/";
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            string zipSourcePath = temporaryStorePath + Path.GetFileNameWithoutExtension(order.BatchPath);
            var finalPath = destinationFolder + Path.GetFileName(order.BatchPath);

            if (Directory.Exists(finalPath))
            {
                // DeleteFolder(finalPath);
            }

            var localDeliveryPath = temporaryStorePath + "/" + Path.GetFileNameWithoutExtension(order.BatchPath);
            downloadedFilesCount = Directory.GetFiles(localDeliveryPath, "*", SearchOption.AllDirectories).Count();
            var zippedFileCount = 0;
            
            if (orderItemsFileCount == downloadedFilesCount)
            {
                if (companyGeneralSetting.IsZipParentFolderSave)
                {
                    try
                    {
                        await Task.Delay(2000);

                        if (tempClientOrderFtp.InputRootFolder.Contains("LL_BEAN"))
                        {

                            if (!finalPath.Contains(".zip"))
                            {
                                finalPath = $"{finalPath}.zip";
                            }

                            ZipFile.CreateFromDirectory(zipSourcePath, finalPath, CompressionLevel.Optimal, true);

                            // Now count the files in the zip archive
                            using (ZipArchive archive = ZipFile.OpenRead(finalPath))
                            {
                                zippedFileCount = archive.Entries.Count;
                                Console.WriteLine($"Number of files in the zip archive: {zippedFileCount}");
                            }
                        }
                        else
                        {
                            ZipFile.CreateFromDirectory(zipSourcePath, finalPath, CompressionLevel.Optimal, true);

                            // Now count the files in the zip archive
                            using (ZipArchive archive = ZipFile.OpenRead(finalPath))
                            {
                                zippedFileCount = archive.Entries.Count;
                                Console.WriteLine($"Number of files in the zip archive: {zippedFileCount}");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()} Final Path:{finalPath}. Exception: {ex.Message}";

                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.Order,
                            PrimaryId = (int)order.Id,
                            ErrorMessage = errorMessage,
                            MethodName = "MoveOrderFromTemporaryStorageToClientFtp->CreateFromDirectory1",
                            RazorPage = "OrderDeliveryService",
                            Category = (int)ActivityLogCategory.OrderDeliveryToClient,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                }
                else
                {
                    try
                    {
                        await Task.Delay(2000);

                        ZipFile.CreateFromDirectory(zipSourcePath, finalPath);

                        // Now count the files in the zip archive
                        using (ZipArchive archive = ZipFile.OpenRead(finalPath))
                        {
                            zippedFileCount = archive.Entries.Count;
                            Console.WriteLine($"Number of files in the zip archive: {zippedFileCount}");
                        }

                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()} Final Path: {finalPath}, Exception: {ex.Message}";

                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.Order,
                            PrimaryId = (int)order.Id,
                            ErrorMessage = errorMessage,
                            MethodName = "MoveOrderFromTemporaryStorageToClientFtp->CreateFromDirectory2",
                            RazorPage = "OrderDeliveryService",
                            Category = (int)ActivityLogCategory.OrderDeliveryToClient,
                        };

                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                }
            }

            if (zippedFileCount == orderItemsFileCount)
            {
                isSucessfullyMoved = await _autoOrderDeliveryService.MoveOrderAsZipToClientFtp(finalPath, tempClientOrderFtp);
            }

            if (Directory.Exists(temporaryStorePath))
            {
                //DeleteFolder(temporaryStorePath);
            }
            return isSucessfullyMoved;
        }

        #endregion

        #region Add Logs
        private async Task AddExceptionForOrderLogs(int orderId, string errorMessage)
        {
            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
            {
                CreatedByContactId = AutomatedAppConstant.ContactId,
                ActivityLogFor = (int)ActivityLogForConstants.Order,
                PrimaryId = orderId,
                ErrorMessage = errorMessage,
                MethodName = "DeliveryOrderToClientStorage->OrderLoop",
                RazorPage = "OrderDeliveryService",
                Category = (int)ActivityLogCategory.OrderDeliveryToClient,
            };
            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }
        #endregion

    }
}
