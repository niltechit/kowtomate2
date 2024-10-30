using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.StorageService;
using FluentFTP;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Core.Models.ViewModel;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.BLL;
using Renci.SshNet;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.PathReplacementServices;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Core.OrderTeams;
using DocumentFormat.OpenXml.Drawing.Charts;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public class AutoOrderDeliveryService : IAutoOrderDeliveryService
    {
        private readonly IFluentFtpService _fluentFtpService;
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IActivityAppLogService _activityLogCommonMethodService;
        private readonly ISshNetService _sshNetService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly IPathReplacementService _pathReplacementService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBllService;

        public AutoOrderDeliveryService(IFluentFtpService fluentFtpService, IFileServerManager fileServerService, IClientOrderItemService clientOrderItemService,
            IActivityAppLogService activityLogCommonMethodService, ISshNetService sshNetService, IActivityAppLogService activityAppLogService,
            IPathReplacementService pathReplacementService, IUpdateOrderItemBLLService updateOrderItemBllService)
        {
            _fluentFtpService = fluentFtpService;
            _fileServerService = fileServerService;
            _clientOrderItemService = clientOrderItemService;
            _activityLogCommonMethodService = activityLogCommonMethodService;
            _sshNetService = sshNetService;
            _activityAppLogService = activityAppLogService;
            _pathReplacementService = pathReplacementService;
            _updateOrderItemBllService = updateOrderItemBllService;
        }

        public async Task<Response<List<ClientOrderItemModel>>> MoveAsCompletedOrder(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order)
        {
            Response<List<ClientOrderItemModel>> response = new Response<List<ClientOrderItemModel>>();
            List<ClientOrderItemModel> completeOrderItems = new List<ClientOrderItemModel>();

            try
            {
                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                var fileServerViewModel = new FileServerViewModel()
                {
                    Host = serverInfo.Host,
                    UserName = serverInfo.UserName,
                    Password = serverInfo.Password,
                };

                using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                {
                    ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    ftp.Config.ValidateAnyCertificate = true;
                    await ftp.AutoConnect();

                    foreach (var orderItem in clientOrderItems)
                    {
                        try
                        {
                            orderItem.InternalFileOutputPath = await GetInternalFileOutputPathFromProductionPath(order, orderItem);

                            var completedDirectory = Path.GetDirectoryName(orderItem.InternalFileOutputPath);
                            completedDirectory = completedDirectory.Replace("\\", "/");
                            completedDirectory = completedDirectory + "/";

                            string tempOrderItemProductionPath = "";
                            string tempOrderItemInternalOutputPath = "";
                            bool isMoved = false;

                            if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                            {
                                completedDirectory = serverInfo.SubFolder + completedDirectory;
                                if (!await ftp.DirectoryExists(completedDirectory))
                                {
                                    await ftp.CreateDirectory(completedDirectory);
                                }
                                tempOrderItemProductionPath = $"{serverInfo.SubFolder}/{orderItem.ProductionDoneFilePath}";
                                tempOrderItemInternalOutputPath = $"{serverInfo.SubFolder}/{orderItem.InternalFileOutputPath}";
                                isMoved = await ftp.MoveFile(tempOrderItemProductionPath, tempOrderItemInternalOutputPath, FtpRemoteExists.Overwrite);
                            }

                            else
                            {
                                if (!await ftp.DirectoryExists(completedDirectory))
                                {
                                    await ftp.CreateDirectory(completedDirectory);
                                }
                                isMoved = await ftp.MoveFile(orderItem.ProductionDoneFilePath, orderItem.InternalFileOutputPath, FtpRemoteExists.Overwrite);
                            }

                            //update order item path

                            if (isMoved)
                            {
                                var updateResponse = await _clientOrderItemService.UpdateClientOrderItemCompletedPathById(orderItem);

                                if (updateResponse.IsSuccess)
                                {
                                    completeOrderItems.Add(orderItem);
                                }
                                else
                                {
                                    await ftp.DeleteFile(orderItem.InternalFileOutputPath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                            {
                                CreatedByContactId = AutomatedAppConstant.ContactId,
                                ActivityLogFor = (int)ActivityLogForConstants.Order,
                                PrimaryId = (int)order.Id,
                                ErrorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. InternalFileOutputPath: {orderItem.InternalFileOutputPath}. File Exception: {ex.Message}",
                                MethodName = "MoveAsCompletedOrder",
                                RazorPage = "AutoOrderDeliveryService",
                                Category = (int)ActivityLogCategory.OrderDeliveryToClient
                            };
                            await _activityAppLogService.InsertAppErrorActivityLog(activity);
                        }
                    }

                    await ftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    PrimaryId = (int)order.Id,
                    ErrorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. Exception: {ex.Message}",
                    MethodName = "MoveAsCompletedOrder",
                    RazorPage = "AutoOrderDeliveryService",
                    Category = (int)ActivityLogCategory.OrderDeliveryToClient
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            response.Result = completeOrderItems;
            return response;

        }

        public async Task<Response<List<ClientOrderItemModel>>> MoveOrderItemIdsAsCompletedOrder(List<long> clientOrderItems, ClientOrderModel order)
        {
            Response<List<ClientOrderItemModel>> response = new Response<List<ClientOrderItemModel>>();
            List<ClientOrderItemModel> completeOrderItems = new List<ClientOrderItemModel>();

            try
            {
                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                var fileServerViewModel = new FileServerViewModel()
                {
                    Host = serverInfo.Host,
                    UserName = serverInfo.UserName,
                    Password = serverInfo.Password,
                };

                using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                {
                    ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    ftp.Config.ValidateAnyCertificate = true;
                    await ftp.AutoConnect();

                    foreach (var orderItemId in clientOrderItems)
                    {
                        var orderItem = await _clientOrderItemService.GetById(orderItemId);

                        try
                        {
                            
                            orderItem.InternalFileOutputPath = await GetInternalFileOutputPathFromProductionPath(order, orderItem);

                            var completedDirectory = Path.GetDirectoryName(orderItem.InternalFileOutputPath);
                            completedDirectory = completedDirectory.Replace("\\", "/");
                            completedDirectory = completedDirectory + "/";

                            string tempOrderItemProductionPath = "";
                            string tempOrderItemInternalOutputPath = "";
                            bool isMoved = false;

                            if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                            {
                                completedDirectory = serverInfo.SubFolder + completedDirectory;
                                if (!await ftp.DirectoryExists(completedDirectory))
                                {
                                    await ftp.CreateDirectory(completedDirectory);
                                }
                                tempOrderItemProductionPath = $"{serverInfo.SubFolder}/{orderItem.ProductionDoneFilePath}";
                                tempOrderItemInternalOutputPath = $"{serverInfo.SubFolder}/{orderItem.InternalFileOutputPath}";
                                isMoved = await ftp.MoveFile(tempOrderItemProductionPath, tempOrderItemInternalOutputPath, FtpRemoteExists.Overwrite);
                            }

                            else
                            {
                                if (!await ftp.DirectoryExists(completedDirectory))
                                {
                                    await ftp.CreateDirectory(completedDirectory);
                                }
                                isMoved = await ftp.MoveFile(orderItem.ProductionDoneFilePath, orderItem.InternalFileOutputPath, FtpRemoteExists.Overwrite);
                            }

                            //update order item path

                            if (isMoved)
                            {
                                var updateResponse = await _clientOrderItemService.UpdateClientOrderItemCompletedPathById(orderItem);

                                if (updateResponse.IsSuccess)
                                {
                                    completeOrderItems.Add(orderItem);
                                }
                                else
                                {
                                    await ftp.DeleteFile(orderItem.InternalFileOutputPath);
                                }

                            }
                        }
                        catch(Exception ex) 
                        {
                            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                            {
                                CreatedByContactId = AutomatedAppConstant.ContactId,
                                ActivityLogFor = (int)ActivityLogForConstants.Order,
                                PrimaryId = (int)order.Id,
                                ErrorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. FileName: {orderItem.InternalFileOutputPath}. InternalFileOutputPath: {orderItem.InternalFileOutputPath} Exception: {ex.Message}",
                                MethodName = "MoveOrderItemIdsAsCompletedOrder->ItemLoop",
                                RazorPage = "AutoOrderDeliveryService",
                                Category = (int)ActivityLogCategory.OrderDeliveryToClient
                            };
                            await _activityAppLogService.InsertAppErrorActivityLog(activity);
                        }

                    }
                    await ftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    PrimaryId = (int)order.Id,
                    ErrorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. Exception: {ex.Message}",
                    MethodName = "MoveOrderItemIdsAsCompletedOrder",
                    RazorPage = "AutoOrderDeliveryService",
                    Category = (int)ActivityLogCategory.OrderDeliveryToClient
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            response.Result = completeOrderItems;
            return response;

        }
        private async Task<string> GetInternalFileOutputPathFromProductionPath(ClientOrderModel order, ClientOrderItemModel orderItem)
        {
            var finalPath = "";
            try
            {
                await Task.Yield();
                var splitSpring = new string[] { order.OrderNumber };
                var pathArray = orderItem.ProductionDoneFilePath.Split("In Progress", StringSplitOptions.None);
                var pathArraytwo = orderItem.ProductionDoneFilePath.Split(splitSpring, StringSplitOptions.None);
                finalPath = pathArray[0] + "Completed" + "/" + order.OrderNumber + pathArraytwo[pathArraytwo.Length - 1];
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    PrimaryId = (int)order.Id,
                    ErrorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. FileName: {orderItem.FileName}, ProductionDoneFilePath: {orderItem.ProductionDoneFilePath}. Exception: {ex.Message}",
                    MethodName = "GetInternalFileOutputPathFromProductionPath",
                    RazorPage = "AutoOrderDeliveryService",
                    Category = (int)ActivityLogCategory.OrderDeliveryToClient,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return finalPath;
        }

        public async Task<Response<List<ClientOrderItemModel>>> MoveOrderToClientFtp(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order, ClientExternalOrderFTPSetupModel tempClientOrderFtp, CompanyGeneralSettingModel companyGeneralSetting = null)
        {
            Response<List<ClientOrderItemModel>> response = new Response<List<ClientOrderItemModel>>();
            List<ClientOrderItemModel> completeOrderItems = new List<ClientOrderItemModel>();

            try
            {
                FtpCredentailsModel destinationFtpCredentails = new FtpCredentailsModel();

                if (tempClientOrderFtp.SentOutputToSeparateFTP)
                {
                    destinationFtpCredentails = GetClientDifferentOutPutFtpConnection(tempClientOrderFtp);
                }
                else
                {
                    destinationFtpCredentails = GetClientSameOutPutFtpConnection(tempClientOrderFtp);
                }

                if (tempClientOrderFtp.IsTemporaryDeliveryUploading)
                {
                    destinationFtpCredentails.RootFolder = destinationFtpCredentails.RootFolder + "/" + tempClientOrderFtp.TemporaryDeliveryUploadFolder;

                }


                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

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
                    Port = serverInfo.Port,
                };

                FtpConfig ftpConfig = new FtpConfig { ConnectTimeout = 900000 };

                //make chunk on orderitems -- for multithread 


                int totalNoOfFiles = clientOrderItems.Count();

                int chunkSize = 20;

                if (totalNoOfFiles < 20)
                {
                    chunkSize = 5;
                }
                else if (totalNoOfFiles < 50)
                {
                    chunkSize = 10;
                }
                else if (totalNoOfFiles < 100)
                {
                    chunkSize = 20;
                }


                var filesChunks = GetFilesChunksWithPaths(clientOrderItems, chunkSize);


                var processingTasks = new List<Task>();
                var semaphore = new SemaphoreSlim(10);

                foreach (var orderItems in filesChunks)
                {
                    await semaphore.WaitAsync();
                    processingTasks.Add(Task.Run(async () =>
                    {
                        try
                        {

                            using (var sourceftp = new AsyncFtpClient(sourceFtpCredential.Host,
                               sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                            {
                                //sourceftp.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                                sourceftp.Encoding = System.Text.Encoding.UTF8;
                                sourceftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                sourceftp.Config.ValidateAnyCertificate = true;
                                await sourceftp.Connect();

                                using (var destinationFtpConnnector = new AsyncFtpClient(destinationFtpCredentails.Host,
                                   destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
                                {
                                    //destinationFtpConnnector.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                                    destinationFtpConnnector.Encoding = System.Text.Encoding.UTF8;
                                    destinationFtpConnnector.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                    destinationFtpConnnector.Config.ValidateAnyCertificate = true;
                                    await destinationFtpConnnector.Connect();

                                    foreach (var orderItem in orderItems)
                                    {
                                        try
                                        {
                                            var sourcePath = orderItem.InternalFileOutputPath;
                                            if (string.IsNullOrWhiteSpace(sourcePath))
                                            {
                                                continue;
                                            }

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


                                            //TODO:Rakib This is temporary solution for gnr .when add PathReplacement table add we well change it .
                                            if (order.CompanyId == AutomatedAppConstant.GNRCompanyId)
                                            {
                                                string input = finalPathArray[1];

                                                // Split the string by slash
                                                string[] parts = input.Split('/');

                                                // Make sure there are at least two parts before joining
                                                if (parts.Length > 2)
                                                {
                                                    // Join the parts starting from the second element
                                                    finalPathArray[1] = string.Join("/", parts, 2, parts.Length - 2);

                                                }
                                            }

                                            if (finalPathArray.Length < 2)
                                            {
                                                continue;
                                            }

                                            var destinationPath = destinationFtpCredentails.RootFolder + "/" + finalPathArray[1];

                                            // Remove Date from the path
                                            var pathReplaceList = await _pathReplacementService.GetPathReplacements(orderItem.CompanyId);

                                            var pathReplaces = pathReplaceList.Where(x => x.Type == (int)PathReplacementType.SubstractDuplicateFacilityNameFromPath).ToList();

                                            if (pathReplaces != null && companyGeneralSetting.IsBatchRootFolderNameAddWithOrder)
                                            {
                                                // Here substract facility name.
                                                destinationPath = await _pathReplacementService.Replace(destinationPath, pathReplaces);
                                            }
                                            else if (pathReplaceList != null && pathReplaceList.Any())
                                            {
                                                destinationPath = await _pathReplacementService.Replace(destinationPath, pathReplaceList);
                                            }

                                            var readStream = await sourceftp.OpenRead(sourcePath);

                                            if (!await destinationFtpConnnector.DirectoryExists(Path.GetDirectoryName(destinationPath)))
                                            {
                                                await destinationFtpConnnector.CreateDirectory(Path.GetDirectoryName(destinationPath));
                                            }

                                            try
                                            {
                                                FtpStatus ftpStatus = await destinationFtpConnnector.UploadStream(readStream, destinationPath, FtpRemoteExists.Overwrite);
                                                if (ftpStatus == FtpStatus.Success)
                                                {
                                                    Console.WriteLine(orderItem.FileName);
                                                    completeOrderItems.Add(orderItem);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. FileName: {orderItem.FileName}, InternalFileOutputPath:{orderItem.InternalFileOutputPath}. ProductionDoneFilePath: {orderItem.ProductionDoneFilePath}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}";
                                                await AddMoveOrderToClientFtpLog(order.Id, "MoveOrderToClientFtp->ItemChunkLoop1", errorMessage);

                                                readStream.Close();
                                                continue;
                                            }

                                            readStream.Close();
                                        }
                                        catch(Exception ex)
                                        {
                                            var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. FileName: {orderItem.FileName}, InternalFileOutputPath:{orderItem.InternalFileOutputPath}. ProductionDoneFilePath: {orderItem.ProductionDoneFilePath}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}";
                                            await AddMoveOrderToClientFtpLog(order.Id, "MoveOrderToClientFtp->ItemChunkLoop2", errorMessage);
                                            continue;
                                        }

                                    }
                                    await sourceftp.Disconnect();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}";                            
                            await AddMoveOrderToClientFtpLog(order.Id, "MoveOrderToClientFtp->ItemChunkLoop", errorMessage);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                await Task.WhenAll(processingTasks);

                using (var destinationFtpConnnector = new AsyncFtpClient(destinationFtpCredentails.Host,
                                   destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
                {
                    destinationFtpConnnector.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    destinationFtpConnnector.Encoding = System.Text.Encoding.UTF8;
                    destinationFtpConnnector.Config.ValidateAnyCertificate = true;
                    await destinationFtpConnnector.Connect();

                    if (tempClientOrderFtp.IsTemporaryDeliveryUploading)
                    {
                        if (await destinationFtpConnnector.DirectoryExists(destinationFtpCredentails.RootFolder))
                        {
                            FtpListItem[] ftpListItems = await destinationFtpConnnector.GetListing(destinationFtpCredentails.RootFolder);
                            foreach (FtpListItem ftpListItem in ftpListItems)
                            {
                                var source = destinationFtpCredentails.RootFolder + "/" + ftpListItem.Name;
                                if (tempClientOrderFtp.SentOutputToSeparateFTP)
                                {
                                    var destination = tempClientOrderFtp.OutputFolderName + "/" + ftpListItem.Name;

                                    if (!await destinationFtpConnnector.DirectoryExists(destination))
                                    {
                                        await destinationFtpConnnector.CreateDirectory(destination);
                                    }
                                    if (await destinationFtpConnnector.DirectoryExists(source))
                                    {
                                        await destinationFtpConnnector.MoveDirectory(source, destination, FtpRemoteExists.Overwrite); //Here Outputfoldername meaning is if image get from one ftp and delivery to another ftp
                                    }
                                }
                                else
                                {
                                    var destination = tempClientOrderFtp.OutputRootFolder + "/" + ftpListItem.Name;

                                    if (!await destinationFtpConnnector.DirectoryExists(destination))
                                    {
                                        await destinationFtpConnnector.CreateDirectory(destination);
                                    }

                                    if (await destinationFtpConnnector.DirectoryExists(source))
                                    {
                                        await destinationFtpConnnector.MoveDirectory(source, destination, FtpRemoteExists.Overwrite); // if same source and destination ftp

                                    }

                                }
                            }
                        }
                    }
                }

                //if (completeOrderItems.Count == totalNoOfFiles)
                //{ 
                response.Result = completeOrderItems;
                response.IsSuccess = true;
                //}
            }
            catch (Exception ex)
            {
                var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}";                   
                await AddMoveOrderToClientFtpLog(order.Id, "MoveOrderToClientFtp", errorMessage);

                response.Result = completeOrderItems;

            }
            return response;
        }

        private async Task AddMoveOrderToClientFtpLog(long orderId, string methodName, string errorMessage)
        {
            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
            {
                CreatedByContactId = AutomatedAppConstant.ContactId,
                ActivityLogFor = (int)ActivityLogForConstants.Order,
                PrimaryId = (int)orderId,
                ErrorMessage = errorMessage,
                MethodName = methodName,
                RazorPage = "AutoOrderDeliveryService",
                Category = (int)ActivityLogCategory.OrderDeliveryToClient,
            };

            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }

        public async Task<bool> MoveOrderAsZipToClientFtp(string sourcePath, ClientExternalOrderFTPSetupModel tempClientOrderFtp)
        {
            var successResult = false;
            
            if (tempClientOrderFtp.InputProtocolType == (int)InputProtocolTypeEnum.SFTP)
            {
                SftpClient destinationFtpConnnector = await _sshNetService.CreateSshNetConnector(false, tempClientOrderFtp);

                using (await _sshNetService.CreateSshNetConnector(false, tempClientOrderFtp))
                {
                    string destinationPath = "";
                    try
                    {
                        destinationFtpConnnector.Connect();
                        destinationPath = tempClientOrderFtp.OutputRootFolder + "/" + Path.GetFileName(sourcePath);                       
                                                    
                        using (var streamToWrite = System.IO.File.OpenRead(sourcePath))
                        {                               
                            await Task.Run(() =>
                            {                                        
                                // Upload the file
                                destinationFtpConnnector.UploadFile(streamToWrite, destinationPath, null);
                                Console.WriteLine($"File uploaded successfully to {destinationPath}");
                                successResult = true;                                        
                            });
                        }

                        destinationFtpConnnector.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        string message = $"{tempClientOrderFtp.GetOutputLogDescription()}. SourcePath: {sourcePath}, DestinationPath: {destinationPath} {ex.Message}";
                        await AddMoveOrderAsZipToClientFtpLog(message);

                        successResult = false;
                    }
                }
            }
            else
            {
                if (tempClientOrderFtp.SentOutputToSeparateFTP)
                {

                    FtpConfig ftpConfig = new FtpConfig { ConnectTimeout = 900000 };
                    using (var destinationFtpConnnector = new AsyncFtpClient(tempClientOrderFtp.OutputHost,
                                           tempClientOrderFtp.OutputUsername, tempClientOrderFtp.OutputPassword, tempClientOrderFtp.OutputPort, ftpConfig))
                    {
                        string destinationPath = "";

                        try
                        {                           
                            destinationFtpConnnector.Encoding = System.Text.Encoding.UTF8;
                            await destinationFtpConnnector.Connect();

                            destinationPath = tempClientOrderFtp.OutputFolderName + "/" + Path.GetFileName(sourcePath);

                            var result = await destinationFtpConnnector.UploadFile(sourcePath, destinationPath);
                            successResult = true;
                            await destinationFtpConnnector.Disconnect();
                            if (!result.Equals(FtpStatus.Success))
                            {
                                successResult = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = $"{tempClientOrderFtp.GetOutputLogDescription()}. SourcePath: {sourcePath}, DestinationPath: {destinationPath} {ex.Message}";
                            await AddMoveOrderAsZipToClientFtpLog(message);

                            successResult = false;
                        }
                    }
                }
                else
                {
                    var destinationFtpCredentails = GetClientSameOutPutFtpConnection(tempClientOrderFtp);
                    FtpConfig ftpConfig = new FtpConfig { ConnectTimeout = 900000 };
                    using (var destinationFtpConnnector = new AsyncFtpClient(destinationFtpCredentails.Host,
                                           destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
                    {
                        string destinationPath = "";

                        try
                        {                            
                            destinationFtpConnnector.Encoding = System.Text.Encoding.UTF8;
                            await destinationFtpConnnector.Connect();

                            if (tempClientOrderFtp.SentOutputToSeparateFTP)
                            {

                                destinationPath = tempClientOrderFtp.OutputFolderName + "/" + Path.GetFileName(sourcePath);
                            }
                            else
                            {
                                destinationPath = tempClientOrderFtp.OutputRootFolder + "/" + Path.GetFileName(sourcePath);
                            }
                            var result = await destinationFtpConnnector.UploadFile(sourcePath, destinationPath);
                            successResult = true;
                            await destinationFtpConnnector.Disconnect();
                            if (!result.Equals(FtpStatus.Success))
                            {
                                successResult = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            string message = $"{tempClientOrderFtp.GetOutputLogDescription()}. SourcePath: {sourcePath}, DestinationPath: {destinationPath} {ex.Message}";
                            await AddMoveOrderAsZipToClientFtpLog(message);

                            successResult = false;
                        }
                    }
                }
            }

            return successResult;
        }

        #region Private Methods
        private async Task AddMoveOrderAsZipToClientFtpLog(string message)
        {
            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
            {
                CreatedByContactId = AutomatedAppConstant.ContactId,
                ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                PrimaryId = 0,
                ErrorMessage = message,
                MethodName = "MoveOrderAsZipToClientFtp",
                RazorPage = "AutoOrderDeliveryService",
                Category = (int)ActivityLogCategory.OrderDeliveryToClient,
            };

            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }

        private static FtpCredentailsModel GetClientDifferentOutPutFtpConnection(ClientExternalOrderFTPSetupModel tempClientOrderFtp)
        {
            FtpCredentailsModel destinationFtpCredential = new FtpCredentailsModel();
            destinationFtpCredential.Host = tempClientOrderFtp.OutputHost;
            destinationFtpCredential.UserName = tempClientOrderFtp.OutputUsername;
            destinationFtpCredential.Password = tempClientOrderFtp.OutputPassword;
            destinationFtpCredential.RootFolder = tempClientOrderFtp.OutputFolderName;
            destinationFtpCredential.Port = tempClientOrderFtp.OutputPort;
            return destinationFtpCredential;
        }

        private static FtpCredentailsModel GetClientSameOutPutFtpConnection(ClientExternalOrderFTPSetupModel tempClientOrderFtp)
        {
            FtpCredentailsModel destinationFtpCredential = new FtpCredentailsModel();
            destinationFtpCredential.Host = tempClientOrderFtp.Host;
            destinationFtpCredential.UserName = tempClientOrderFtp.Username;
            destinationFtpCredential.Password = tempClientOrderFtp.Password;
            destinationFtpCredential.RootFolder = tempClientOrderFtp.OutputRootFolder;
            destinationFtpCredential.Port = tempClientOrderFtp.Port;
            return destinationFtpCredential;
        }

        private List<List<ClientOrderItemModel>> GetFilesChunksWithPaths(List<ClientOrderItemModel> clientOrderItems, int chunkSize)
        {
            var chunks = new List<List<ClientOrderItemModel>>();

            int count = 0;
            var chunk = new List<ClientOrderItemModel>();
            int i = 0;
            foreach (var clientOrderItem in clientOrderItems)
            {

                count++;
                chunk.Add(clientOrderItem);

                if (count == chunkSize)
                {
                    chunks.Add(chunk);
                    chunk = new List<ClientOrderItemModel>();
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
        private async Task<bool> DiretoryFileMove(List<ClientOrderItemModel> clientOrderItems, SftpClient destinationConnector, string source, string destination)
        {
            await Task.Yield();
            int moveFileCount = await _sshNetService.RecursiveListFilesMove(destinationConnector, source, destination);

            if (moveFileCount == clientOrderItems.Count)
            {

                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion End of Private methods

        public async Task<Response<List<ClientOrderItemModel>>> MoveOrderToClientSFtp(List<ClientOrderItemModel> clientOrderItems, ClientOrderModel order, ClientExternalOrderFTPSetupModel tempClientOrderFtp)
        {
            Response<List<ClientOrderItemModel>> response = new Response<List<ClientOrderItemModel>>();

            var k = 0;
            string clientFolderPath = "";
            bool isSuccessfullyDeliver = false;
            List<ClientOrderItemModel> completeOrderItems = new List<ClientOrderItemModel>();
            try
            {
                FtpCredentailsModel destinationFtpCredentails = new FtpCredentailsModel();

                if (tempClientOrderFtp.SentOutputToSeparateFTP)
                {
                    //if (tempClientOrderFtp.OutputProtocolType == (int)InputProtocolTypeEnum.SFTP)
                    //{
                    //	await _sshNetService.CreateSshNetConnector(tempClientOrderFtp.ClientCompanyId.ToString());
                    //}
                    //else
                    //{
                    destinationFtpCredentails = GetClientDifferentOutPutFtpConnection(tempClientOrderFtp);
                    //}
                }
                else
                {
                    destinationFtpCredentails = GetClientSameOutPutFtpConnection(tempClientOrderFtp);
                }

                if (tempClientOrderFtp.IsTemporaryDeliveryUploading)
                {
                    destinationFtpCredentails.RootFolder = destinationFtpCredentails.RootFolder + "/" + tempClientOrderFtp.TemporaryDeliveryUploadFolder;
                }

                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

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
                };

                //make chunk on orderitems -- for multithread 
                int totalNoOfFiles = clientOrderItems.Count();

                int chunkSize = 1;

                //if (totalNoOfFiles <= 50)
                //{
                //	chunkSize = 1;
                //}
                //else
                //{
                //	chunkSize = 5;
                //}

                var filesChunks = GetFilesChunksWithPaths(clientOrderItems, chunkSize);

                var processingTasks = new List<Task>();
                var semaphore = new SemaphoreSlim(20);

                Console.WriteLine($"Delivery Start:{DateTime.Now}");

                FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();

                foreach (var orderItems in filesChunks)
                {
                    await semaphore.WaitAsync();

                    processingTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            using (var sourceftp = new AsyncFtpClient(sourceFtpCredential.Host,
                               sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
                            {
                                //sourceftp.Config.
                                sourceftp.Config.DownloadRateLimit = 0;
                                sourceftp.Encoding = System.Text.Encoding.UTF8;

                                await sourceftp.AutoConnect();


                                //destination connector create 
                                using (SftpClient destinationConnector = await _sshNetService.CreateSshNetConnector(false, tempClientOrderFtp))
                                {
                                    destinationConnector.BufferSize = 1024 * 1024 * 10;
                                    destinationConnector.Connect();


                                    foreach (var orderItem in orderItems)
                                    {


                                        try
                                        {
                                            var sourcePath = orderItem.InternalFileOutputPath;

                                            if (string.IsNullOrWhiteSpace(sourcePath))
                                            {
                                                continue;
                                            }

                                            if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                            {
                                                sourcePath = $"{serverInfo.SubFolder}/{sourcePath}";
                                            }

                                            string[] pathArray = new string[500];

                                            if (orderItem.InternalFileOutputPath != null)
                                            {
                                                pathArray = orderItem.InternalFileOutputPath.Split("/Completed");
                                            }

                                            var finalPathArray = pathArray[1].Split(order.OrderNumber.ToString());

                                            if (finalPathArray.Length >= 2)
                                            {
                                                var destinationPath = destinationFtpCredentails.RootFolder + "/" + finalPathArray[1];
                                                // NO uses this code and comment
                                                //var destinationFilePath_1 = destinationFtpCredentails.RootFolder;
                                                //destinationFilePath_1 = destinationFilePath_1.Replace("\\", "/");

                                                destinationPath = Path.GetDirectoryName(destinationPath);
                                                destinationPath = destinationPath.Replace("\\", "/");
                                                destinationPath = destinationPath.Replace("//", "/");

                                                clientFolderPath = destinationPath;

                                                var finalOutPutFileName = Path.GetFileName(finalPathArray[1]);
                                                string destinationFullFilePath = destinationPath + "/" + finalOutPutFileName;

                                                // Remove Date from the path Or 
                                                var pathReplaceList = await _pathReplacementService.GetPathReplacements(orderItem.CompanyId);

                                                if (pathReplaceList != null && pathReplaceList.Any())
                                                {
                                                    destinationFullFilePath = await _pathReplacementService.Replace(destinationFullFilePath, pathReplaceList);
                                                    destinationPath = Path.GetDirectoryName(destinationFullFilePath);
                                                }

                                                if (!destinationConnector.Exists(destinationFullFilePath))
                                                {
                                                    await _sshNetService.RecursiveCreateDirectories(destinationConnector, destinationPath);

                                                    //upload folder
                                                    int j = 0;

                                                    while (j < 3)
                                                    {
                                                        try
                                                        {

                                                            Console.WriteLine($", Start On: {DateTime.Now}");
                                                            k++;
                                                            Console.WriteLine(k.ToString());

                                                            //var fileBytes = await sourceftp.DownloadBytes(sourcePath, CancellationToken.None);
                                                            var fileBytes = new byte[220];

                                                            //if (fileBytes == null)
                                                            //{
                                                            //                                                 j++;
                                                            //                                                 Thread.Sleep(2000);

                                                            //                                                 if (j >= 5)
                                                            //                                                 {
                                                            //                                                     break;
                                                            //                                                 }

                                                            //	continue;
                                                            //                                             }



                                                            // Create a Stopwatch instance
                                                            //Stopwatch stopwatch = new Stopwatch();

                                                            // Start the stopwatch
                                                            //stopwatch.Start();

                                                            //destinationConnector.WriteAllBytes(destinationFullFilePath, fileBytes);

                                                            //using (var streamToWrite = destinationConnector.Create(destinationFullFilePath))
                                                            //{

                                                            //	Console.WriteLine("Start Read" + DateTime.Now);
                                                            //	var readStartTimeTemp = DateTime.Now;
                                                            //	//sourceftp.DownloadFile(sourcePath, streamToWrite);
                                                            //	await sourceftp.DownloadStream(streamToWrite, sourcePath);

                                                            //	var uploadDoneTimeTemp = DateTime.Now;
                                                            //	Console.WriteLine("Upload Finish" + uploadDoneTimeTemp.Subtract(readStartTimeTemp).TotalMinutes);

                                                            //}


                                                            int retryOrderItemDelivery = 0;
                                                            while (retryOrderItemDelivery < 3)
                                                            {
                                                                try
                                                                {
                                                                    Console.WriteLine("Start Upload" + DateTime.Now);
                                                                    var readStartTimeTemp = DateTime.Now;
                                                                    using (var sourceStream = await sourceftp.OpenRead(sourcePath))
                                                                    {
                                                                        destinationConnector.UploadFile(sourceStream, destinationFullFilePath);
                                                                    }
                                                                    var uploadDoneTimeTemp = DateTime.Now;
                                                                    Console.WriteLine("Upload Finish" + uploadDoneTimeTemp.Subtract(readStartTimeTemp).TotalMinutes);
                                                                    break;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()}, InternalFileOutputPath: {orderItem.InternalFileOutputPath}, DestinationFullFilePath: {destinationFullFilePath} Exception: {ex.Message}";
                                                                    await AddMoveOrderToClientSFtpLog(order.Id, errorMessage, "->OrderItemsLoop->RetryOrderItemDelivery");

                                                                    Thread.Sleep(1000);
                                                                    retryOrderItemDelivery++;
                                                                    if (retryOrderItemDelivery >= 3)
                                                                    {
                                                                        try
                                                                        {
                                                                            destinationConnector.DeleteFile(destinationFullFilePath);
                                                                        }
                                                                        catch (Exception deleteException)
                                                                        {
                                                                            //Console.WriteLine(ex.ToString());
                                                                            var errorMessage2 = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()}, InternalFileOutputPath: {orderItem.InternalFileOutputPath}, DestinationFullFilePath: {destinationFullFilePath} Exception: {deleteException.Message}";
                                                                            await AddMoveOrderToClientSFtpLog(order.Id, errorMessage2, "->OrderItemsLoop->DeleteFile");
                                                                        }                                                                       
                                                                    }
                                                                }
                                                            }

                                                            //destinationConnector.UploadFile(readStream, , canOverride: true);
                                                            Console.WriteLine($"End On: {DateTime.Now}");

                                                            // Stop the stopwatch
                                                            //stopwatch.Stop();
                                                            // Get the elapsed time
                                                            //TimeSpan elapsedTime = stopwatch.Elapsed;
                                                            //Console.WriteLine($"Total time taken: {elapsedTime.TotalMinutes:F2} minutes");

                                                            Console.WriteLine($"Write :{sourcePath}");
                                                            string message = $"File Uploaded To Client Ftp:{destinationPath + "/" + orderItem.FileName} on {DateTime.Now}";
                                                            await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, message);
                                                            break;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()}, InternalFileOutputPath: {orderItem.InternalFileOutputPath} Exception: {ex.Message}";
                                                            await AddMoveOrderToClientSFtpLog(order.Id, errorMessage, "->OrderItemsLoop->WhileLoop");
                                                            
                                                            j++;
                                                            Thread.Sleep(500);

                                                            if (j >= 3)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                //FtpStatus ftpStatus = await destinationFtpConnnector.UploadStream(readStream, destinationPath);


                                                //if (ftpStatus == FtpStatus.Success)
                                                //{
                                                completeOrderItems.Add(orderItem);
                                                //}
                                                //readStream.Close();
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()}, InternalFileOutputPath: {orderItem.InternalFileOutputPath} Exception: {ex.Message}";
                                            await AddMoveOrderToClientSFtpLog(order.Id, errorMessage, "->OrderItemsLoop");                                            
                                        }
                                    }

                                    destinationConnector.Disconnect();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}";
                            await AddMoveOrderToClientSFtpLog(order.Id, errorMessage, "->OrderItemsChunkLoop");
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                await Task.WhenAll(processingTasks);

                Console.WriteLine($"Delivery End:{DateTime.Now}");
                //if any images misssing to deliver
                if (completeOrderItems.Count != clientOrderItems.Count)
                {
                    //Need To Add Log
                    //Console.WriteLine($"Count Not Equal :{clientOrderItems.Count}");

                    //using (SftpClient destinationConnector = await _sshNetService.CreateSshNetConnector())
                    //{
                    //    destinationConnector.Connect();
                    //    destinationConnector.DeleteDirectory(clientFolderPath);
                    //}
                    response.Result = completeOrderItems;
                    return response;
                }

                Console.WriteLine($"-----------------------Move Start:{DateTime.Now}");
                if (tempClientOrderFtp.IsTemporaryDeliveryUploading)
                {
                    //create detination connector 
                    using (SftpClient destinationConnector = await _sshNetService.CreateSshNetConnector(false, tempClientOrderFtp))
                    {
                        //destinationFtpConnnector.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        //destinationFtpConnnector.Config.ValidateAnyCertificate = true;
                        destinationConnector.Connect();


                        if (destinationConnector.Exists(destinationFtpCredentails.RootFolder))
                        {
                            var ftpListItems = destinationConnector.ListDirectory(destinationFtpCredentails.RootFolder);
                            foreach (var ftpListItem in ftpListItems)
                            {
                                if (ftpListItem.Name == Path.GetFileName(clientFolderPath))
                                {
                                    //if (!string.IsNullOrEmpty(ftpListItem.Name) && ftpListItem.IsDirectory && ftpListItem.Name != "." && ftpListItem.Name != "..")
                                    //{
                                    var source = destinationFtpCredentails.RootFolder + "/" + ftpListItem.Name;
                                    string destination = "";
                                    if (tempClientOrderFtp.SentOutputToSeparateFTP)
                                    {
                                        destination = tempClientOrderFtp.OutputFolderName + "/" + ftpListItem.Name;

                                        isSuccessfullyDeliver = await DiretoryFileMove(clientOrderItems, destinationConnector, source, destination);
                                    }
                                    else
                                    {
                                        destination = tempClientOrderFtp.OutputRootFolder + "/" + ftpListItem.Name;
                                        isSuccessfullyDeliver = await DiretoryFileMove(clientOrderItems, destinationConnector, source, destination);
                                        //}
                                    }

                                    if (isSuccessfullyDeliver)
                                    {
                                        destinationConnector.DeleteDirectory(source);
                                    }
                                    else
                                    {
                                        destinationConnector.DeleteDirectory(destination);
                                    }
                                    //}

                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"-----------------------Move End:{DateTime.Now}");

                if (isSuccessfullyDeliver) // If all file successfully send client ftp folder
                {
                    response.Result = completeOrderItems;
                    response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"CompanyId: {order.CompanyId}. Order Number: {order.OrderNumber}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}";
                await AddMoveOrderToClientSFtpLog(order.Id, errorMessage, "Root");
                //if (completeOrderItems.Count != clientOrderItems.Count)
                //{
                //    using (SftpClient destinationConnector = await _sshNetService.CreateSshNetConnector())
                //    {
                //        destinationConnector.Connect();
                //        destinationConnector.DeleteDirectory(clientFolderPath);
                //    }
                //    return response;
                //}
                response.Result = completeOrderItems;
            }

            if (tempClientOrderFtp.OutputProtocolType == (int)OutputProtocolTypeEnum.SFTP)
            {
                response.Result = completeOrderItems;
            }
            response.Result = completeOrderItems;

            return response;
        }

        private async Task AddMoveOrderToClientSFtpLog(long orderId, string message, string methodDetail="")
        {
            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
            {
                CreatedByContactId = AutomatedAppConstant.ContactId,
                ActivityLogFor = (int)ActivityLogForConstants.Order,
                PrimaryId = (int)orderId,
                ErrorMessage = message,
                MethodName = $"MoveOrderToClientSFtp{methodDetail}",
                RazorPage = "AutoOrderDeliveryService",
                Category = (int)ActivityLogCategory.OrderDeliveryToClient,
            };

            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }

        #region Send Hotkey for Client Confirmation to Ftp 

        public async Task<Response<bool>> SendHotkeyFileToFtp(string localPath, string batchName, ClientExternalOrderFTPSetupModel tempClientOrderFtp)
        {
            await Task.Yield();

            var response = new Response<bool>();

            try
            {
                FtpConfig ftpConfig = new FtpConfig { ConnectTimeout = 900000 };

                using (var destinationFtpClient = new FtpClient(tempClientOrderFtp.Host, tempClientOrderFtp.Username, tempClientOrderFtp.Password, tempClientOrderFtp.Port ?? 0, ftpConfig))
                {                    
                    destinationFtpClient.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    destinationFtpClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                    destinationFtpClient.Config.ValidateAnyCertificate = true;

                    destinationFtpClient.AutoConnect();

                    var destinationPath = $"{tempClientOrderFtp.OutputRootFolder}/{batchName}/{Path.GetFileName(localPath)}";

                    using (var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes(localPath)))
                    {
                        FtpStatus ftpStatus = destinationFtpClient.UploadStream(memoryStream, destinationPath, FtpRemoteExists.Overwrite, createRemoteDir: true);

                        if (ftpStatus != FtpStatus.Success)
                        {
                            response.Result = false;
                            response.IsSuccess = false;
                            return response;
                        }
                    }

                    response.Result = true;
                    response.IsSuccess = true;
                    
                    destinationFtpClient.Disconnect();
                    
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    PrimaryId = 0,
                    ErrorMessage = $"Batch Name: {batchName}. LocalPath: {localPath}. {tempClientOrderFtp.GetInputLogDescription()}, {tempClientOrderFtp.GetOutputLogDescription()} Exception: {ex.Message}",              
                    MethodName = $"SendHotkeyFileToFtp",
                    RazorPage = "AutoOrderDeliveryService",
                    Category = (int)ActivityLogCategory.OrderDeliveryToClient,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                // Handle outer exceptions
                response.Result = false;
                response.IsSuccess = false;
            }

            return response;
        }

        #endregion
    }
}
