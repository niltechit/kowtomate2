using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.ErrorLogServices;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.OrderTeamServices;
using CutOutWiz.Services.BLL.StatusChangeLog;
using FluentFTP;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.UploadFromEditorPcAutomation
{
    public class UploadFromEditorPcService: IUploadFromEditorPcService
    {

        public static readonly string uploadedFilesContainerOnProductionPc = "_uploaded";

        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly IContactManager _contactManager;
        private readonly IErrorLogService _errorLogService;
        private readonly ICompanyManager _companyService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IFluentFtpService _fluentFtpService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        private readonly IOrderAssignedImageEditorService _orderAssignedImageEditorService;
        private readonly ICompanyTeamManager _companyTeamService;
        private readonly IStatusChangeLogBLLService _statusChangeLogBLLService;

        public UploadFromEditorPcService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            IContactManager contactService,
            IErrorLogService errorLogService,
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
            _errorLogService = errorLogService;
            _companyService = companyService;
            _companyGeneralSettingService = companyGeneralSettingService;
            _clientOrderItemService = clientOrderItemService;
            _fluentFtpService = fluentFtpService;
            _updateOrderItemBLLService = updateOrderItemBLLService;
            _orderAssignedImageEditorService = orderAssignedImageEditorService;
            _companyTeamService = companyTeamService;
            _statusChangeLogBLLService = statusChangeLogBLLService;
        }

        #region Public Method
        public async Task<Response<bool>> UploadImageFromEditorPc(int consoleAppId)
        {
            var editorList = await _contactManager.GetAllIsSharedFolderEditorContact(consoleAppId);

            Console.WriteLine("Scan Editor Pc For Getting editing Completers...........");
            List<ContactModel> editingOrderCompleters = GetProductionDoneOrderCompleters(editorList);
            Console.WriteLine($"Get {editingOrderCompleters.Count} Editors....");

            if (editingOrderCompleters == null || !editingOrderCompleters.Any())
            {
                return new Response<bool>();
            }

            try
            {
                var semaphore = new SemaphoreSlim(1);

                var tasks = new List<Task>();

                    foreach (var editor in editingOrderCompleters)
                    {
                        await semaphore.WaitAsync();

                        tasks.Add(Task.Run(async () =>
                        {
                            await TakeFileFromEditorPc(semaphore, editor);
                        }));
                    }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                string methodName = "UploadImageFromEditorPc";
                byte errorCategory = (byte)ActivityLogCategory.UploadFromEditorPc;

                await _errorLogService.LogGeneralError(ex, methodName, errorCategory);

            }
            return new Response<bool>();
        }

        #endregion

        #region Private Method
        private async Task TakeFileFromEditorPc(SemaphoreSlim semaphore, ContactModel contact)
        {
            if (contact.IsSharedFolderEnable)
            {
                try
                {
                    FileServerModel serverInfo = new FileServerModel();
                    DateTimeConfiguration _dateTime = new DateTimeConfiguration();
                    var uploadDiretory = contact.DownloadFolderPath + "\\" + contact.FirstName.Trim() + " " + contact.Id + "\\" + "Completed\\";
                    Console.WriteLine(uploadDiretory);
                    if (Directory.Exists(uploadDiretory))
                    {
                        Console.WriteLine(uploadDiretory + "StillExist");

                        try
                        {
                            //strat code
                            string[] directories = Directory.GetDirectories(uploadDiretory);

                            if (directories != null && directories.Any())
                            {
                                serverInfo = await UploadSingleImageAndOrderFolderImageFromEdtiorPc(contact, serverInfo, _dateTime, uploadDiretory, directories);
                            }
                        }
                        catch (Exception ex)
                        {
                            string methodName = "TakeFileFromEditorPc- 1";
                            byte errorCategory = (byte)ActivityLogCategory.UploadFromEditorPc;

                            await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string methodName = "TakeFileFromEditorPc- 2";
                    byte errorCategory = (byte)ActivityLogCategory.UploadFromEditorPc;

                    await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        private async Task<FileServerModel> UploadSingleImageAndOrderFolderImageFromEdtiorPc(ContactModel contact, FileServerModel serverInfo, DateTimeConfiguration _dateTime, string uploadDiretory, string[] directories)
        {
            try
            {
                foreach (string dir in directories)
                {
                    Console.WriteLine(contact.FirstName + dir);

                    if (dir.Contains("_uploaded"))
                    {
                        continue;
                    }
                    
                    if (dir.Split(uploadDiretory)[1].Length <= 5) //If folder name less 5 we assume its a company code // VC Company for this logic
                    {
                        var companyCode = dir.Split(uploadDiretory)[1];

                        var uploadFolderCompany = await _companyService.GetByCompanyCode(companyCode);

                        if (uploadFolderCompany == null || uploadFolderCompany.Id < 0)
                        {
                            continue;
                        }

                        var companyGeneralSetting = await _companyGeneralSettingService.GetGeneralSettingByCompanyId(uploadFolderCompany.Id);

                        if (companyGeneralSetting == null || !companyGeneralSetting.AllowClientWiseImageProcessing)
                        {
                            continue;
                        }

                        if (Directory.Exists(dir))
                        {
                            await TakeSingleImageFromEditorPc(contact, _dateTime, uploadDiretory, dir, uploadFolderCompany, companyGeneralSetting);
                        }

                    }
                    else
                    {
                        var uploaDirectories = dir.Split(uploadDiretory);

                        if (uploaDirectories == null || uploaDirectories.Length < 2)
                        {
                            continue;
                        }

                        var orderNumber = uploaDirectories[1];
                        var orderInfo = await _orderService.GetByOrderNumber(orderNumber);

                        if (orderInfo == null || orderInfo.Id <= 0)
                        {
                            continue;
                        }


                        serverInfo = await _fileServerService.GetById((int)orderInfo.FileServerId);
                        var isAllowExtraOutputFileUpload = orderInfo.AllowExtraOutputFileUpload;

                        string[] filePaths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                        var fileExists = new ClientOrderItemModel();
                        bool isAllOrderItemUploaded = false;

                        if (filePaths == null || !filePaths.Any())
                        {
                            continue;
                        }

                        foreach (string filePath in filePaths)
                        {

                            if (Path.GetFileName(filePath) == "Thumbs.db")
                            {
                                continue;
                            }


                            fileExists = new ClientOrderItemModel();
                            var uploadFolderPath = "/" + Path.GetDirectoryName(filePath.Split(uploadDiretory)[1]).Replace("\\", "/");
                            FileUploadModel fileUploadVM = new FileUploadModel();

                            ClientOrderItemModel orderItemFile = await PrepareOrderItemForEditorOrderItemUpload(orderInfo, filePath, uploadFolderPath);

                            if (isAllowExtraOutputFileUpload)
                            {
                                fileExists = await TakeExtraFileAllowOrderImageFromEditorPC(orderInfo, fileExists, orderItemFile, contact);
                            }
                            else
                            {
                                //fileExists = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPath(orderItemFile);
                                fileExists = await _clientOrderItemService.GetByFileByOrderIdAndFullFileNameAndPath(orderItemFile);
                            }

                            if (fileExists != null && fileExists.Id > 0)
                            {
                                orderItemFile.Id = (int)fileExists.Id;
                                if (fileExists.Status == (byte)InternalOrderItemStatus.InProduction || fileExists.Status == (byte)InternalOrderItemStatus.ReworkInProduction || fileExists.Status == (byte)InternalOrderItemStatus.ProductionDone || fileExists.Status == (byte)InternalOrderItemStatus.ReworkDone)
                                {
                                    FileServerViewModel fileServerViewModel = PrepareFileServerViewModel(serverInfo);
                                    if (fileServerViewModel.Host != null && fileServerViewModel.UserName != null && fileServerViewModel.Password != null)
                                    {
                                        using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                                        {
                                            isAllOrderItemUploaded = await UploadProductionDoneImageToFtp(contact, serverInfo, _dateTime, orderInfo, fileExists, isAllOrderItemUploaded, filePath, uploadFolderPath, fileUploadVM, orderItemFile, ftp);
                                        }
                                    }

                                }
                            }
                        }

                        if (isAllOrderItemUploaded)
                        {
                            int i = 0;
                            while (true)
                            {
                                try
                                {
                                    string sourcePath = uploadDiretory + orderNumber;
                                    string destinationPath = uploadDiretory + "_uploaded/" + orderNumber;

                                    if (Directory.Exists(destinationPath))
                                    {
                                        Directory.Delete(destinationPath, true);
                                    }
                                    string rootUploadedDirectoroy = uploadDiretory + "_uploaded/";

                                    if (!Directory.Exists(rootUploadedDirectoroy))
                                    {
                                        Directory.CreateDirectory(rootUploadedDirectoroy);
                                    }

                                    if (Directory.Exists(sourcePath))
                                    {
                                        Directory.Move(sourcePath, destinationPath);
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
                    }
                }


            }
            catch (Exception ex)
            {
                string methodName = "UploadSingleImageAndOrderFolderImageFromEdtiorPc";
                byte errorCategory = (byte)ActivityLogCategory.UploadFromEditorPc;

                await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
            }

            return serverInfo;
        }


        private async Task TakeSingleImageFromEditorPc(ContactModel contact, DateTimeConfiguration _dateTime, string uploadDiretory, string dir, CompanyModel uploadFolderCompany, CompanyGeneralSettingModel companyGeneralSetting)
        {
            try
            {
                string[] files = Directory.GetFiles(dir);


                if (files == null || !files.Any())
                {
                    return;
                }

                foreach (string file in files)
                {
                    var uploadItem = await _clientOrderItemService.GetItemByImageNameAndCompanyId(Path.GetFileName(file), companyGeneralSetting.CompanyId);

                    if (uploadItem != null && (uploadItem.Status == (byte)InternalOrderItemStatus.InProduction || uploadItem.Status == (byte)InternalOrderItemStatus.ReworkInProduction || uploadItem.Status == (byte)InternalOrderItemStatus.ProductionDone || uploadItem.Status == (byte)InternalOrderItemStatus.ReworkDone))
                    {
                        if (uploadItem != null && uploadItem.Id > 0)
                        {
                            if (uploadItem.CompanyId != uploadFolderCompany.Id)
                            {
                                continue;
                            }
                            var orderInfo = await _orderService.GetById(uploadItem.ClientOrderId);

                            var uploadItemServerInfo = await _fileServerService.GetById((int)orderInfo.FileServerId);
                            var fileServerViewModel = new FileServerViewModel()
                            {
                                Host = uploadItemServerInfo.Host,
                                UserName = uploadItemServerInfo.UserName,
                                Password = uploadItemServerInfo.Password,
                                SubFolder = uploadItemServerInfo.SubFolder,
                            };

                            await _dateTime.DateTimeConvert(orderInfo.CreatedDate);
                            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                            {
                                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                ftp.Config.ValidateAnyCertificate = true;
                                await ftp.AutoConnect();
                                string uploadPath = $"{uploadFolderCompany.Code}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/In Progress/{orderInfo.OrderNumber}/{contact.FirstName.Trim() + contact.Id}/Production Done/{orderInfo.OrderNumber}/{Path.GetFileName(file)}";
                                FtpStatus response = FtpStatus.Failed;
                                using (FileStream fileStream = System.IO.File.OpenRead(file))
                                {
                                    if (!string.IsNullOrWhiteSpace(fileServerViewModel.SubFolder))
                                    {

                                        response = await ftp.UploadStream(fileStream, $"{fileServerViewModel.SubFolder}/{uploadPath}", FtpRemoteExists.Overwrite, true);

                                    }
                                    else
                                    {
                                        response = await ftp.UploadStream(fileStream, $"{uploadPath}", FtpRemoteExists.Overwrite, true);
                                    }
                                }

                                await ftp.Disconnect();

                                if (response.Equals(FtpStatus.Success))
                                {
                                    uploadItem.ProductionDoneFilePath = uploadPath;

                                    var isOrderItemUpdated = await _clientOrderItemService.UpdateEitorItemInfo(uploadItem);
                                    if (isOrderItemUpdated.IsSuccess)
                                    {
                                        uploadItem.Status = (byte)InternalOrderItemStatus.ProductionDone;
                                        uploadItem.ExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ProductionDone);

                                        await _updateOrderItemBLLService.UpdateOrderItemStatus(uploadItem, InternalOrderItemStatus.ProductionDone, contact.Id);


                                        string rootUploadedDirectoroy = uploadDiretory + "_uploaded/";

                                        if (!Directory.Exists(rootUploadedDirectoroy))
                                        {
                                            Directory.CreateDirectory(rootUploadedDirectoroy);

                                        }

                                        var moveFileSource = file;
                                        var moveFileDestination = rootUploadedDirectoroy + uploadItem.FileName;
                                        if (System.IO.File.Exists(moveFileDestination))
                                        {
                                            System.IO.File.Delete(moveFileDestination);
                                        }
                                        System.IO.File.Move(moveFileSource, moveFileDestination);

                                        await _orderStatusService.UpdateOrderStatus(orderInfo, AutomatedAppConstant.ContactId);
                                    }
                                }

                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                string methodName = "TakeSingleImageFromEditorPc";
                byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;

                await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
            }
        }


        private async Task<ClientOrderItemModel> PrepareOrderItemForEditorOrderItemUpload(ClientOrderModel orderInfo, string filePath, string uploadFolderPath)
        {
            var orderItemFile = new ClientOrderItemModel();
            try
            {
                orderItemFile = new ClientOrderItemModel()
                {
                    PartialPath = uploadFolderPath,
                    ClientOrderId = orderInfo.Id,
                    FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath),
                    CompanyId = orderInfo.CompanyId,
                    FileName = Path.GetFileName(filePath),
                    FileType = Path.GetExtension(filePath),
                    FileSize = 50, //Dummy
                    IsExtraOutPutFile = true
                };
            }
            catch (Exception ex)
            {
                string methodName = "PrepareOrderItemForEditorOrderItemUpload";
                byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;

                await _errorLogService.LogGeneralError(ex, methodName, errorCategory);

            }

            return orderItemFile;
        }

        private async Task<ClientOrderItemModel> TakeExtraFileAllowOrderImageFromEditorPC(ClientOrderModel orderInfo, ClientOrderItemModel fileExists, ClientOrderItemModel orderItemFile, ContactModel contact = null)
        {
            try
            {
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllGeneralSettingsByCompanyId(orderInfo.CompanyId);
                companyGeneralSettings.IsSameNameImageExistOnSameFolder = true;

                if (!companyGeneralSettings.IsSameNameImageExistOnSameFolder)
                {
                    fileExists = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPath(orderItemFile);
                }

                if (fileExists == null || fileExists.Id == 0)
                {
                    try
                    {
                        var filesExistList = await _clientOrderItemService.GetFileListByOrderIdAndFileName(orderItemFile);
                        if (filesExistList == null || !filesExistList.Any())
                        {
                            var addItemResponse = await AddOrderItem(orderItemFile, orderInfo.CompanyId, orderInfo.Id, InternalOrderItemStatus.ProductionDone);
                            if (addItemResponse.Result > 0)
                            {
                                await ExtraImageFileAddWithAssignOwnUploader(orderInfo, addItemResponse.Result, contact);

                                fileExists = await _clientOrderItemService.GetById(addItemResponse.Result);
                            }
                        }
                        else
                        {
                            if (filesExistList.Count == 1)
                            {
                                fileExists = filesExistList.FirstOrDefault();
                                if (fileExists.Status == (byte)InternalOrderItemStatus.ProductionDone || fileExists.Status == (byte)InternalOrderItemStatus.ReworkDone)
                                {
                                    var lastIndexOfOrderNumber = fileExists.ProductionDoneFilePath.LastIndexOf(orderInfo.OrderNumber);
                                    var existingFileFolderPath = Path.GetDirectoryName("/" + fileExists.ProductionDoneFilePath.Substring(lastIndexOfOrderNumber));
                                    existingFileFolderPath = existingFileFolderPath.Replace("\\", "/");
                                    if (existingFileFolderPath != orderItemFile.PartialPath)
                                    {
                                        var addItemResponse = await AddOrderItem(orderItemFile, orderInfo.CompanyId, orderInfo.Id, InternalOrderItemStatus.ProductionDone);
                                        if (addItemResponse.Result > 0)
                                        {
                                            await ExtraImageFileAddWithAssignOwnUploader(orderInfo, addItemResponse.Result, contact);

                                            fileExists = await _clientOrderItemService.GetById(addItemResponse.Result);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<string> temporyProductionFolderPathList = new();
                                foreach (var item in filesExistList)
                                {
                                    try
                                    {
                                        var lastIndexOfOrderNumber = item.ProductionDoneFilePath.LastIndexOf(orderInfo.OrderNumber);
                                        var existingFileFolderPath = Path.GetDirectoryName("/" + item.ProductionDoneFilePath.Substring(lastIndexOfOrderNumber));
                                        existingFileFolderPath = existingFileFolderPath.Replace("\\", "/");
                                        temporyProductionFolderPathList.Add(existingFileFolderPath);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    
                                }
                                if (!temporyProductionFolderPathList.Contains(orderItemFile.PartialPath))
                                {
                                    var addItemResponse = await AddOrderItem(orderItemFile, orderInfo.CompanyId, orderInfo.Id, InternalOrderItemStatus.ProductionDone);
                                    if (addItemResponse.Result > 0)
                                    {
                                        await ExtraImageFileAddWithAssignOwnUploader(orderInfo, addItemResponse.Result, contact);

                                        fileExists = await _clientOrderItemService.GetById(addItemResponse.Result);
                                    }
                                }
                                else
                                {
                                    var indexOfProductionDoneFolderMatchingPath = temporyProductionFolderPathList.IndexOf(orderItemFile.PartialPath);
                                    fileExists = filesExistList[indexOfProductionDoneFolderMatchingPath];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string methodName = "TakeExtraFileAllowOrderImageFromEditorPC 1";
                        byte errorCategory = (byte)ActivityLogCategory.UploadFromEditorPc;

                        await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                string methodName = "TakeExtraFileAllowOrderImageFromEditorPC 2";
                byte errorCategory = (byte)ActivityLogCategory.UploadFromEditorPc;

                await _errorLogService.LogGeneralError(ex, methodName, errorCategory);
            }



            return fileExists;
        }

        private static FileServerViewModel PrepareFileServerViewModel(FileServerModel serverInfo)
        {
            return new FileServerViewModel()
            {
                Host = serverInfo.Host,
                UserName = serverInfo.UserName,
                Password = serverInfo.Password,
            };
        }

        private async Task<bool> UploadProductionDoneImageToFtp(ContactModel contact, FileServerModel serverInfo, DateTimeConfiguration _dateTime, ClientOrderModel orderInfo, ClientOrderItemModel fileExists, bool isAllOrderItemUploaded, string filePath, string uploadFolderPath, FileUploadModel fileUploadVM, ClientOrderItemModel orderItemFile, AsyncFtpClient ftp)
        {
            try
            {
                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                ftp.Config.ValidateAnyCertificate = true;
                await ftp.AutoConnect();
                await _dateTime.DateTimeConvert(orderInfo.CreatedDate);
                var orderCompanyInfo = await _companyService.GetById(orderInfo.CompanyId);

                fileUploadVM.ReturnPath = $"{orderCompanyInfo.Code}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/In Progress/{orderInfo.OrderNumber}/{contact.FirstName.Trim() + contact.Id}/Production Done/";

                var ftpUploadFullPath = fileUploadVM.ReturnPath + uploadFolderPath + "/" + orderItemFile.FileName;

                if (await ftp.FileExists(ftpUploadFullPath))
                {
                    await ftp.DeleteFile(ftpUploadFullPath);
                }


                FtpStatus response = FtpStatus.Failed;

                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    if (!string.IsNullOrEmpty(serverInfo.SubFolder))
                    {
                        response = await ftp.UploadStream(fileStream, $"{serverInfo.SubFolder}/{ftpUploadFullPath}", FtpRemoteExists.Overwrite, true);
                        Console.WriteLine(contact.FirstName + ftpUploadFullPath);
                    }
                    else
                    {
                        response = await ftp.UploadStream(fileStream, $"{ftpUploadFullPath}", FtpRemoteExists.Overwrite, true);
                    }

                }

                await ftp.Disconnect();

                if (response.Equals(FtpStatus.Success))
                {
                    isAllOrderItemUploaded = true;
                    orderItemFile.ProductionDoneFilePath = ftpUploadFullPath.Replace("//", "/");

                    var isOrderItemUpdated = await _clientOrderItemService.UpdateEitorItemInfo(orderItemFile);
                    if (isOrderItemUpdated.IsSuccess)
                    {
                        orderItemFile.Status = (byte)InternalOrderItemStatus.ProductionDone;
                        orderItemFile.ExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ProductionDone);
                        orderItemFile.FileGroup = fileExists.FileGroup;
                        await _updateOrderItemBLLService.UpdateOrderItemStatus(orderItemFile, InternalOrderItemStatus.ProductionDone, contact.Id);
                        await _orderStatusService.UpdateOrderStatus(orderInfo, AutomatedAppConstant.ContactId);
                    }

                }
                else if (response.Equals(FtpStatus.Failed))
                {
                    isAllOrderItemUploaded = false;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return isAllOrderItemUploaded;
        }

        private static List<ContactModel> GetProductionDoneOrderCompleters(List<ContactModel> editorList)
        {
            //return editorList
            //.Where(editor =>
            //    editor.IsSharedFolderEnable &&
            //    Directory.Exists(Path.Combine(editor.DownloadFolderPath, $"{editor.FirstName.Trim()} {editor.Id}", "Completed")) &&
            //    Directory.GetDirectories(Path.Combine(editor.DownloadFolderPath, $"{editor.FirstName.Trim()} {editor.Id}", "Completed"))
            //        .Any(dir => !dir.Contains(uploadedFilesContainerOnProductionPc)))
            //.ToList();

            var result = new List<ContactModel>();

            Parallel.ForEach(editorList, editor =>
            {
                bool success = false;
                int attempts = 0;

                while (!success && attempts < 3)
                {
                    try
                    {
                        attempts++;

                        // Build the completed path.
                        var completedPath = Path.Combine(editor.DownloadFolderPath, $"{editor.FirstName.Trim()} {editor.Id}", "Completed");

                        // Check if the folder exists and contains directories not matching the uploadedFilesContainer.
                        if (editor.IsSharedFolderEnable &&
                            Directory.Exists(completedPath) &&
                            Directory.GetDirectories(completedPath)
                                .Any(dir => !dir.Contains(uploadedFilesContainerOnProductionPc)))
                        {
                            result.Add(editor);
                            success = true; // Mark as successful.
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Attempt {attempts} failed for editor {editor.Id}. Error: {ex.Message}");

                        // If max attempts reached, log error.
                        if (attempts == 3)
                        {
                            Console.WriteLine($"Failed to process editor {editor.Id} after 3 attempts.");
                        }

                        // Optional: Small delay before retrying (e.g., 100ms).
                        Task.Delay(100).Wait();
                    }
                }
            });

            return result.ToList();
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
