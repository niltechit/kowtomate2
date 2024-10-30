using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.IBRModels;
using CutOutWiz.Services.Models.Security;
using FluentFTP;
using System.Diagnostics;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.IbrApiServices;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using DocumentFormat.OpenXml.Drawing.Charts;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.DownloadToEditorAutomation
{
    public class DownloadToEditorService : IDownloadToEditorService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly IIbrApiService _ibrApiService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IFtpService _ftpService;
        private readonly ICompanyManager _companyService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        public DownloadToEditorService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            ICompanyGeneralSettingManager companyGeneralSettingService,
            IIbrApiService ibrApiService,
            IClientOrderItemService clientOrderItemService,
            IFtpService ftpService,
            ICompanyManager companyService,
            IUpdateOrderItemBLLService updateOrderItemBLLService
            
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
            _companyGeneralSettingService = companyGeneralSettingService;
            _ibrApiService = ibrApiService;
            _clientOrderItemService = clientOrderItemService;
            _ftpService = ftpService;
            _companyService = companyService;
            _updateOrderItemBLLService = updateOrderItemBLLService;
        }

        public async Task<Response<bool>> RetouchedAiProcessingAndSaveFilePath(int consoleAppId)
        {
            try
            {
                string query = $"SELECT cgs.* from\r\ndbo.Common_Company c with(nolock)\r\ninner join dbo.CompanyGeneralSettings  cgs with(nolock) on c.Id = cgs.CompanyId \r\nwhere c.Status = 1 and cgs.AutoDistributeToEditor = 1 AND cgs.FtpOrderPlacedAppNo={consoleAppId}";
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);

                if (companyGeneralSettings != null && !companyGeneralSettings.Any())
                {
                    return new Response<bool>();
                }
                //login
                string ibrToken = "";
                var ibrDefaultSettings = new IbrDefaultSettingsApiResponse();


                if (companyGeneralSettings.Any(c => c.IsIbrProcessedEnabled == true))
                {

                    Response<IbrLoginResponse> ibrWebApiResponse = new Response<IbrLoginResponse>();

                    IbrLoginRequest ibrLoginRequest = new IbrLoginRequest()
                    {
                        email = "vcteamkow@gmail.com",
                        password = "default@cv/#25kowai@"
                    };

                    ibrWebApiResponse = await _ibrApiService.Login(ibrLoginRequest);

                    if (ibrWebApiResponse.IsSuccess && ibrWebApiResponse.Result != null && !string.IsNullOrEmpty(ibrWebApiResponse.Result.token))
                    {
                        ibrToken = ibrWebApiResponse.Result.token;


                        //Get Default setting
                        var ibrDefaultSettingResponse = await _ibrApiService.GetIbrGeneralSetting(ibrToken);

                        if (ibrDefaultSettingResponse.IsSuccess && ibrDefaultSettingResponse.Result != null)
                        {
                            ibrDefaultSettings = ibrDefaultSettingResponse.Result;
                        }
                    }
                }

                foreach (var companyGeneralSetting in companyGeneralSettings)
                {
                    int i = 0;
                    while (i < 3)
                    {
                        var items = await _clientOrderItemService.GetOrderItemsForSendingToEditorPc(companyGeneralSetting.CompanyId);

                        if (items != null && items.Any())
                        {
                            //Create Ibr Order
                            var orderMasterInfoResponse = await _ibrApiService.GetOrderMasterId(ibrDefaultSettings, ibrToken);
                            var ibrOrderId = "";
                            if (orderMasterInfoResponse != null && orderMasterInfoResponse.Result != null && !string.IsNullOrEmpty(orderMasterInfoResponse.Result.order_id))
                            {
                                ibrOrderId = orderMasterInfoResponse.Result.order_id;
                            }

                            var orders = from p in items
                                         group p by p.ClientOrderId into g
                                         select new { ClientOrderId = g.Key, Items = g.ToList() };

                            foreach (var itemGroup in orders)
                            {
                                var itemsOfOrders = itemGroup.Items;
                                var order = await _orderService.GetById(itemGroup.ClientOrderId);

                                await RetouchedAiApiCallForFileProcessing(order, itemsOfOrders, companyGeneralSetting, ibrToken, ibrOrderId, ibrDefaultSettings.model_base_url);
                            }
                            i++;
                        }
                        else
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
                    ActivityLogFor = (int)ActivityLogForConstants.ConsoleAppId,
                    PrimaryId = consoleAppId,
                    ErrorMessage = $"Console App ID: {consoleAppId}, Exception: {ex.Message}",
                    MethodName = "RetouchedAiProcessingAndSaveFilePath",
                    RazorPage = "DownloadToEditorService",
                    Category = (int)ActivityLogCategory.SingleDownloadEditorError,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return new Response<bool>();
        }

        public async Task<Response<bool>> AutoDownloadDistributedItemToEditorPc(int consoleAppId)
        {
            try
            {
                string query = $"SELECT cgs.* from\r\ndbo.Common_Company c with(nolock)\r\ninner join dbo.CompanyGeneralSettings  cgs with(nolock) on c.Id = cgs.CompanyId \r\nwhere c.Status = 1  AND cgs.FtpOrderPlacedAppNo={consoleAppId}";
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);

                if (companyGeneralSettings != null && !companyGeneralSettings.Any())
                {
                    return new Response<bool>();
                }
                //login
                string ibrToken = "";
                var ibrDefaultSettings = new IbrDefaultSettingsApiResponse();


                if (companyGeneralSettings.Any(c => c.IsIbrProcessedEnabled == true))
                {

                    Response<IbrLoginResponse> ibrWebApiResponse = new Response<IbrLoginResponse>();

                    IbrLoginRequest ibrLoginRequest = new IbrLoginRequest()
                    {
                        email = "vcteamkow@gmail.com",
                        password = "default@cv/#25kowai@"
                    };

                    ibrWebApiResponse = await _ibrApiService.Login(ibrLoginRequest);

                    if (ibrWebApiResponse.IsSuccess && ibrWebApiResponse.Result != null && !string.IsNullOrEmpty(ibrWebApiResponse.Result.token))
                    {
                        ibrToken = ibrWebApiResponse.Result.token;


                        //Get Default setting
                        var ibrDefaultSettingResponse = await _ibrApiService.GetIbrGeneralSetting(ibrToken);

                        if (ibrDefaultSettingResponse.IsSuccess && ibrDefaultSettingResponse.Result != null)
                        {
                            ibrDefaultSettings = ibrDefaultSettingResponse.Result;
                        }
                    }
                }


                foreach (var companyGeneralSetting in companyGeneralSettings)
                {


                    int i = 0;
                    while (i < 3)
                    {
                        var items = await _clientOrderItemService.GetOrderItemsForSendingToEditorPc(companyGeneralSetting.CompanyId);

                        if (items != null && items.Any())
                        {
                            //Create Ibr Order
                            var orderMasterInfoResponse = await _ibrApiService.GetOrderMasterId(ibrDefaultSettings, ibrToken);
                            var ibrOrderId = "";
                            if (orderMasterInfoResponse != null && orderMasterInfoResponse.Result != null && !string.IsNullOrEmpty(orderMasterInfoResponse.Result.order_id))
                            {
                                ibrOrderId = orderMasterInfoResponse.Result.order_id;
                            }

                            var orders = from p in items
                                         group p by p.ClientOrderId into g
                                         select new { ClientOrderId = g.Key, Items = g.ToList() };

                            foreach (var itemGroup in orders)
                            {
                                var itemsOfOrders = itemGroup.Items;
                                var order = await _orderService.GetById(itemGroup.ClientOrderId);

                                await DownloadOrderItemInEditorsPc(order, itemsOfOrders, companyGeneralSetting, ibrToken, ibrOrderId, ibrDefaultSettings.model_base_url);
                            }
                            i++;
                        }
                        else
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
                    ActivityLogFor = (int)ActivityLogForConstants.ConsoleAppId,
                    PrimaryId = consoleAppId,
                    ErrorMessage = $"Console App ID: {consoleAppId}. Exception: {ex.Message}",
                    MethodName = "AutoDownloadDistributedItemToEditorPc",
                    RazorPage = "DownloadToEditorService",
                    Category = (int)ActivityLogCategory.SingleDownloadEditorError
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return new Response<bool>();
        }

        #region Private Method
        private async Task RetouchedAiApiCallForFileProcessing(ClientOrderModel order, List<ClientOrderItemModel> clientOrderItems,
                    CompanyGeneralSettingModel companyGeneralSetting, string ibrToken, string ibrOrderId, string model_base_url)
        {           
            try
            {
                var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

                var semaphore = new SemaphoreSlim(1);

                var tasks = new List<Task>();

                Console.WriteLine("File Write Done");

                var contactImages = from p in clientOrderItems
                                    group p by p.EditorContactId into g
                                    select new { EditorContactId = g.Key, Items = g.ToList() };

                var successItemList = new List<ClientOrderItemModel>();
                //Start of Item Loops

                foreach (var contactImageGroup in contactImages)
                {
                    // Wait for a slot to become available in the semaphore
                    await semaphore.WaitAsync();

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var contactFromImages = contactImageGroup.Items.FirstOrDefault();
                            var orderItemList = contactImageGroup.Items;
                            
                            if (orderItemList.Any())
                            {
                                DateTime currentDateTime = DateTime.Now;

                                FileUploadModel model = new FileUploadModel();
                                model.FtpUrl = serverInfo.Host;
                                model.userName = serverInfo.UserName;
                                model.password = serverInfo.Password;
                                model.OrderNumber = order.OrderNumber;
                                model.Date = order.CreatedDate;
                                model.ContactName = contactFromImages.EditorFirstName.Trim() + " " + contactFromImages.EditorContactId;

                                using (var client = _ftpService.CreateAsyncFtpClient(model))
                                {
                                    client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                    client.Config.ValidateAnyCertificate = true;
                                    await client.AutoConnect();

                                    foreach (var orderItem in orderItemList)
                                    {
                                        try
                                        {
                                            model.fileName = orderItem.FileName;
                                            model.UploadDirectory = Path.GetDirectoryName(orderItem.InternalFileInputPath);
                                            model.DownloadFolderName = orderItem.PartialPath;

                                            var remotePath = $"{model.UploadDirectory}/{model.fileName}";

                                            if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                            {
                                                remotePath = $"{serverInfo.SubFolder}/{model.UploadDirectory}/{model.fileName}";
                                            }

                                            //Call Retouched AI Api Call

                                            if (companyGeneralSetting.IsIbrProcessedEnabled && !string.IsNullOrEmpty(ibrOrderId))
                                            {

                                                var tempStream = await client.OpenRead(remotePath);
                                                byte[] downloadImagebytes;
                                                using (MemoryStream ms = new MemoryStream())
                                                {
                                                    tempStream.CopyTo(ms);
                                                    downloadImagebytes = ms.ToArray();
                                                    tempStream.Close();
                                                }


                                                DateTime ibrCallingTime = DateTime.Now;
                                                Stopwatch stopwatch = Stopwatch.StartNew();

                                                var response = await _ibrApiService.RequestForIbrProcess(ibrOrderId, downloadImagebytes, ibrToken, model.fileName, model_base_url);


                                                if (!string.IsNullOrWhiteSpace(response.Result) && response.IsSuccess)
                                                {
                                                    // Store File Path 

                                                    var clientOrderItem = new ClientOrderItemModel
                                                    {
                                                        Id = orderItem.Id,
                                                        IbrProcessedImageUrl = response.Result,
                                                        IbrStatus = (int)RetouchedAiProcessStatus.Success,
                                                    };

                                                    await _clientOrderItemService.UpdateAfterRetouchedAiProcessed(clientOrderItem);
                                                }
                                                else
                                                {
                                                    var clientOrderItem = new ClientOrderItemModel
                                                    {
                                                        Id = orderItem.Id,
                                                        IbrStatus = (int)RetouchedAiProcessStatus.Failed,
                                                    };

                                                    await _clientOrderItemService.UpdateAfterRetouchedAiProcessed(clientOrderItem);
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
                                                ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. ItemId: {orderItem?.Id}, InternalFileInputPath: {orderItem?.InternalFileInputPath}. Exception: {ex.Message}",
                                                MethodName = "RetouchedAiProcessingAndSaveFilePath->ItemLoop",
                                                RazorPage = "DownloadToEditorService",
                                                Category = (int)ActivityLogCategory.SingleDownloadEditorError,
                                            };

                                            await _activityAppLogService.InsertAppErrorActivityLog(activity);
                                        }
                                    }

                                    await client.Disconnect();

                                }
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }
                //End of all Item loops

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    PrimaryId = (int)order.Id,
                    ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. Exception: {ex.Message}",
                    MethodName = "RetouchedAiProcessingAndSaveFilePath",
                    RazorPage = "DownloadToEditorService",
                    Category = (int)ActivityLogCategory.SingleDownloadEditorError,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
        }

        private async Task DownloadOrderItemInEditorsPc(ClientOrderModel order, List<ClientOrderItemModel> itemsToSendForEditors,
                   CompanyGeneralSettingModel companyGeneralSetting, string ibrToken, string ibrOrderId, string model_base_url)
        {
            var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

            try
            {
                var semaphore = new SemaphoreSlim(1);

                var tasks = new List<Task>();
                //string folderPath = @$"{AutomatedAppConstant.timeTrackerUrl}\{DateTime.Now.ToShortDateString().Replace("/", "_")}";
                //string fileName = "processing_time.txt";
                //string filePath = Path.Combine(folderPath, fileName);
                double totalProcessingTime = 0;
                double totalPsdDownloadTime = 0;
                double numberOfImages = 0;

                // Create the directory if it doesn't exist
                //if (!Directory.Exists(folderPath))
                //{
                //    Directory.CreateDirectory(folderPath);
                //}

                //string text = "DateTime: " + DateTime.Now + ", Order Number: " + order.OrderNumber + Environment.NewLine;
                //using (StreamWriter sw = new StreamWriter(filePath, true))
                //{
                //    sw.WriteLine(text);
                //}

                //System.IO.File.WriteAllText(filePath,"DateTime: "+ DateTime.Now +", Order Number: "+order.OrderNumber + Environment.NewLine);
                Console.WriteLine("File Write Done");

                var contactImages = from p in itemsToSendForEditors
                                    group p by p.EditorContactId into g
                                    select new { EditorContactId = g.Key, Items = g.ToList() };

                var successItemList = new List<ClientOrderItemModel>();
                //Start of Item Loops

                foreach (var contactImageGroup in contactImages)
                {
                    // Wait for a slot to become available in the semaphore
                    await semaphore.WaitAsync();

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var contactFromImages = contactImageGroup.Items.FirstOrDefault();

                            bool isAllDownloadSuccessfully = true;
                            var orderItemList = contactImageGroup.Items; //await _clientOrderItemService.GetDistributedClientOrderItemByEditorContactIdAndOrderId(contactImageGroup.EditorContactId, order.Id);

                            if (orderItemList.Any())
                            {
                                List<ClientOrderItemModel> successfulleDownloadedItems = new List<ClientOrderItemModel>();
                                DateTime currentDateTime = DateTime.Now;
                                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");
                                //CutOutWiz.Data.Security.Contact contact = await _contactManager.GetById(contactImageGroup.EditorContactId);
                                var sharedFolderDownloadPath = contactFromImages.EditorDownloadFolderPath;

                                //if (contact != null)
                                //{
                                //	sharedFolderDownloadPath = contact.DownloadFolderPath;
                                //}

                                if (!string.IsNullOrWhiteSpace(sharedFolderDownloadPath) && Directory.Exists(sharedFolderDownloadPath))
                                {
                                    FileUploadModel model = new FileUploadModel();
                                    model.FtpUrl = serverInfo.Host;
                                    model.userName = serverInfo.UserName;
                                    model.password = serverInfo.Password;
                                    model.OrderNumber = order.OrderNumber;
                                    model.Date = order.CreatedDate;
                                    model.ContactName = contactFromImages.EditorFirstName.Trim() + " " + contactFromImages.EditorContactId;

                                    using (var client = _ftpService.CreateAsyncFtpClient(model))
                                    {
                                        client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                        client.Config.ValidateAnyCertificate = true;
                                        await client.AutoConnect();

                                        var semaphoreItems = new SemaphoreSlim(4);

                                        var taskItems = new List<Task>();

                                        foreach (var orderItem in orderItemList)
                                        {
                                            var dataSavePath = "";
                                            string downloadingFolderPath = ""; //1
                                            try
                                            {
                                                model.fileName = orderItem.FileName;
                                                model.UploadDirectory = System.IO.Path.GetDirectoryName(orderItem.InternalFileInputPath);
                                                model.DownloadFolderName = orderItem.PartialPath;

                                                if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
                                                {
                                                    if (string.IsNullOrEmpty(model.DownloadFolderName))
                                                    {
                                                        dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{order.OrderNumber}";
                                                        downloadingFolderPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "_Downloading\\" + $"{formattedDateTimeForDownload}" + "\\" + $"{order.OrderNumber}";//2
                                                    }
                                                    else
                                                    {
                                                        dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{model.DownloadFolderName}";
                                                        downloadingFolderPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "_Downloading\\" + $"{formattedDateTimeForDownload}" + "\\" + $"{order.OrderNumber}";//3

                                                    }

                                                    var localPath = $"{downloadingFolderPath}/{model.fileName}";
                                                    var remotePath = $"{model.UploadDirectory}/{model.fileName}";

                                                    if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                                                    {
                                                        remotePath = $"{serverInfo.SubFolder}/{model.UploadDirectory}/{model.fileName}";
                                                    }


                                                    //IBr Connect 

                                                    if (companyGeneralSetting.IsIbrProcessedEnabled && !string.IsNullOrEmpty(ibrOrderId))
                                                    {
                                                        var saveFileAs = Path.ChangeExtension(model.fileName, ".psd");
                                                        var localPathForIbrDownload = $"{downloadingFolderPath}/{saveFileAs}";

                                                        var tempStream = await client.OpenRead(remotePath);
                                                        byte[] downloadImagebytes;
                                                        using (MemoryStream ms = new MemoryStream())
                                                        {
                                                            tempStream.CopyTo(ms);
                                                            downloadImagebytes = ms.ToArray();
                                                            tempStream.Close();
                                                        }


                                                        DateTime ibrCallingTime = DateTime.Now;
                                                        Stopwatch stopwatch = Stopwatch.StartNew();
                                                        //api call
                                                        var response = await _ibrApiService.RequestForIbrProcess(ibrOrderId, downloadImagebytes, ibrToken, model.fileName, model_base_url);
                                                        stopwatch.Stop();
                                                        DateTime ibrResponsedTime = DateTime.Now;
                                                        double processingTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
                                                        totalProcessingTime += processingTimeInSeconds;
                                                        string processingTimeStr = $"{processingTimeInSeconds:F2} seconds";


                                                        string newRow = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} ,Ibr Calling Time: {ibrCallingTime} , Ibr Responsed Time: {ibrResponsedTime} ,Processing Time: {processingTimeInSeconds:F2} Second";


                                                        if (!Directory.Exists(downloadingFolderPath))
                                                        {
                                                            Directory.CreateDirectory(downloadingFolderPath);
                                                        }
                                                        if (!string.IsNullOrWhiteSpace(response.Result))
                                                        {
                                                            using (HttpClient httpClient = new HttpClient())
                                                            {
                                                                int i = 0;
                                                                while (true)
                                                                {
                                                                    try
                                                                    {
                                                                        Stopwatch stopwatchforDownload = Stopwatch.StartNew();
                                                                        //webClient.DownloadFile(response.Result, localPathForIbrDownload);
                                                                        using var httpDlResponse = await httpClient.GetAsync(response.Result);
                                                                        using var stream = await httpDlResponse.Content.ReadAsStreamAsync();
                                                                        using var fileStream = new FileStream(localPathForIbrDownload, FileMode.Create);
                                                                        await stream.CopyToAsync(fileStream);
                                                                        stopwatchforDownload.Stop();
                                                                        fileStream.Close();
                                                                        stream.Close();
                                                                        string message = $"File DownloadPath:{localPathForIbrDownload} on {DateTime.Now}";
                                                                        await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, message);

                                                                        double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;
                                                                        totalPsdDownloadTime += downloadTimeInSeconds;
                                                                        newRow = newRow + $", PSD Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

                                                                        //using (StreamWriter sw = new StreamWriter(filePath, true)) // Use "true" to append to existing file
                                                                        //{
                                                                        //    sw.WriteLine(newRow);
                                                                        //}

                                                                        break;
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                                                        {
                                                                            CreatedByContactId = AutomatedAppConstant.ContactId,
                                                                            ActivityLogFor = (int)ActivityLogForConstants.Order,
                                                                            PrimaryId = (int)order.Id,
                                                                            ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. LocalPathForIbrDownload: {localPathForIbrDownload}. Error: Try download from Ibr. Message: {newRow}. Exception: {ex.Message}",
                                                                            MethodName = "DownloadToEditorService->1",
                                                                            RazorPage = "DownloadToEditorPcService",
                                                                            Category = (int)ActivityLogCategory.IbrProcessingApi,
                                                                        };

                                                                        await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                                                        Console.WriteLine(ex.ToString());
                                                                        i++;
                                                                        await Task.Delay(500);

                                                                        if (i >= 3)
                                                                        {
                                                                            Stopwatch stopwatchforDownload = Stopwatch.StartNew();
                                                                            await client.DownloadFile(localPath, remotePath);
                                                                            stopwatchforDownload.Stop();
                                                                            double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;

                                                                            string newRowForRawDownload = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} , Date:{DateTime.Now},Processing Time: {processingTimeInSeconds:F2} Second (Failed !)" + $" Raw Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

                                                                            //using (StreamWriter sw = new StreamWriter(filePath, true))
                                                                            //{
                                                                            //    sw.WriteLine(newRowForRawDownload);
                                                                            //}

                                                                            //await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, localPath + "Note: Trying Three times download from Ibr ");

                                                                            break;
                                                                        }


                                                                        
                                                                    }
                                                                }

                                                            }
                                                        }
                                                        else
                                                        {
                                                            Stopwatch stopwatchforDownload = Stopwatch.StartNew();
                                                            await client.DownloadFile(localPath, remotePath);
                                                            stopwatchforDownload.Stop();
                                                            double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;

                                                            string newRowForRawDownload = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} , Date:{DateTime.Now},Processing Time: {processingTimeInSeconds:F2} Second (Failed !)" + $" Raw Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

                                                            //using (StreamWriter sw = new StreamWriter(filePath, true))
                                                            //{
                                                            //    sw.WriteLine(newRowForRawDownload);
                                                            //}
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int i = 0;
                                                        while (true)
                                                        {
                                                            try
                                                            {
                                                                Stopwatch stopwatchforDownload = Stopwatch.StartNew();
                                                                await client.DownloadFile(localPath, remotePath);
                                                                stopwatchforDownload.Stop();
                                                                double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;

                                                                string newRowForRawDownload = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} , Date:{DateTime.Now}" + $", Raw Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

                                                                //using (StreamWriter sw = new StreamWriter(filePath, true))
                                                                //{
                                                                //    sw.WriteLine(newRowForRawDownload);
                                                                //}

                                                                break;
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                                                {
                                                                    CreatedByContactId = AutomatedAppConstant.ContactId,
                                                                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                                                                    PrimaryId = (int)order.Id,
                                                                    ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. FileName: {orderItem.FileName}. Local Path: {localPath}, Remote Path: {remotePath}. Exception: {ex.Message}",
                                                                    MethodName = "DownloadToEditorService->2",
                                                                    RazorPage = "DownloadToEditorPcService",
                                                                    Category = (int)ActivityLogCategory.IbrProcessingApi,
                                                                };

                                                                await _activityAppLogService.InsertAppErrorActivityLog(activity);


                                                                Console.WriteLine(ex.ToString());
                                                                i++;
                                                                await Task.Delay(500);

                                                                if (i >= 3)
                                                                {
                                                                    orderItemList.RemoveAll(ot => ot.Id == orderItem.Id);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }//Shared Found End

                                            } //end of item try
                                            catch (Exception ex)
                                            {
                                                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                                {
                                                    CreatedByContactId = AutomatedAppConstant.ContactId,
                                                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                                                    PrimaryId = (int)order.Id,
                                                    ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. FileName: {orderItem.FileName}. InternalFileInputPath: {orderItem.InternalFileInputPath}. Exception: {ex.Message}",
                                                    MethodName = "DownloadToEditorService->3",
                                                    RazorPage = "DownloadToEditorPcService",
                                                    Category = (int)ActivityLogCategory.IbrProcessingApi,
                                                };

                                                await _activityAppLogService.InsertAppErrorActivityLog(activity);


                                                isAllDownloadSuccessfully = false;
                                            }
                                            finally
                                            {
                                                semaphoreItems.Release();
                                            }
                                        }

                                        //End of item loop
                                        await Task.WhenAll(taskItems);
                                        await client.Disconnect();

                                        if (isAllDownloadSuccessfully)
                                        {
                                            int j = 0;
                                            while (true)
                                            {
                                                string sourcePath = sharedFolderDownloadPath + $"/{model.ContactName}/_Downloading/{formattedDateTimeForDownload}/{order.OrderNumber}";
                                                string destinationPath = sharedFolderDownloadPath + $"/{model.ContactName}/Raw/{formattedDateTimeForDownload}";

                                                try
                                                {                                                   
                                                    if (companyGeneralSetting.AllowClientWiseImageProcessing)
                                                    {
                                                        DateTime downloadDateTime = DateTime.Now;
                                                        TimeSpan targetTime = new TimeSpan(23, 0, 0);

                                                        if (downloadDateTime.TimeOfDay >= targetTime)
                                                        {
                                                            downloadDateTime = downloadDateTime.AddDays(1);
                                                        }
                                                        string formattedDateForDownload = downloadDateTime.ToString("dd-MM-yyyy");
                                                        CompanyModel company = await _companyService.GetById(companyGeneralSetting.CompanyId);
                                                        string[] files = Directory.GetFiles(sourcePath);
                                                        string destinationPathForClientWiseImageProcessing = sharedFolderDownloadPath + $"/{model.ContactName}/Raw/{formattedDateForDownload}/{company.Code}";
                                                        if (!Directory.Exists(destinationPathForClientWiseImageProcessing))
                                                        {
                                                            Directory.CreateDirectory(destinationPathForClientWiseImageProcessing);
                                                        }
                                                        foreach (string file in files)
                                                        {
                                                            string imageName = Path.GetFileName(file);
                                                            string tempPath = Path.Combine(destinationPathForClientWiseImageProcessing, imageName);
                                                            if (System.IO.File.Exists(tempPath))
                                                            {
                                                                System.IO.File.Delete(tempPath);
                                                            }
                                                            System.IO.File.Move(file, tempPath);

                                                        }
                                                        var editorUploadPath = sharedFolderDownloadPath + $"/{model.ContactName}/Completed/{company.Code}/";
                                                        if (!Directory.Exists(editorUploadPath))
                                                        {
                                                            Directory.CreateDirectory(editorUploadPath);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!Directory.Exists(sourcePath))
                                                        {
                                                            return;
                                                        }

                                                        string rawDirectory = sharedFolderDownloadPath + $"/{model.ContactName}/Raw/";

                                                        if (!Directory.Exists(rawDirectory))
                                                        {
                                                            Directory.CreateDirectory(rawDirectory);
                                                        }

                                                        Directory.Move(sourcePath, destinationPath);
                                                    }

                                                    var uploadedPath = sharedFolderDownloadPath + $"/{model.ContactName}/Completed/_uploaded/";
                                                    if (!Directory.Exists(uploadedPath))
                                                    {
                                                        Directory.CreateDirectory(uploadedPath);
                                                    }

                                                    break;
                                                }
                                                catch(Exception ex)
                                                {
                                                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                                    {
                                                        CreatedByContactId = AutomatedAppConstant.ContactId,
                                                        ActivityLogFor = (int)ActivityLogForConstants.Order,
                                                        PrimaryId = (int)order.Id,
                                                        ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. FileName: {sourcePath}. DestinationPath: {destinationPath}. Exception: {ex.Message}",
                                                        MethodName = "DownloadToEditorService->4",
                                                        RazorPage = "DownloadToEditorPcService",
                                                        Category = (int)ActivityLogCategory.IbrProcessingApi,
                                                    };

                                                    await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                                    j++;
                                                    await Task.Delay(1000);
                                                    if (j > 3)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }

                                            if (orderItemList.Count > 0 && j <= 3)
                                            {
                                                var ReworkInProductionImages = orderItemList.Where(i => i.Status == (int)InternalOrderItemStatus.ReworkDistributed).ToList();
                                                var InproductionImages = orderItemList.Where(i => i.Status == (int)InternalOrderItemStatus.Distributed).ToList();
                                                if (ReworkInProductionImages.Any())
                                                {
                                                    await _updateOrderItemBLLService.UpdateOrderItemsStatus(ReworkInProductionImages, InternalOrderItemStatus.ReworkInProduction);
                                                }
                                                else if (InproductionImages.Any())
                                                {
                                                    await _updateOrderItemBLLService.UpdateOrderItemsStatus(InproductionImages, InternalOrderItemStatus.InProduction);
                                                }
                                                await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
                                            }
                                        }

                                    }
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
                                ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. Exception: {ex.Message}",
                                MethodName = "DownloadOrderItemInEditorsPc->ContactImages Loop",
                                RazorPage = "DownloadToEditorPcService",
                                Category = (int)ActivityLogCategory.IbrProcessingApi,
                            };

                            await _activityAppLogService.InsertAppErrorActivityLog(activity);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));

                }
                //End of all Item loops


                await Task.WhenAll(tasks);
                string row = "TotalProcessing Time: " + totalProcessingTime + "TotalPsdDownloadTime: " + totalPsdDownloadTime + Environment.NewLine;

                //using (StreamWriter sw = new StreamWriter(filePath, true)) // Use "true" to append to existing file
                //{
                //    sw.WriteLine(row);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    PrimaryId = (int)order.Id,
                    ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}. Order Number: {order.OrderNumber}. Exception: {ex.Message}",
                    MethodName = "DownloadOrderItemInEditorsPc->5",
                    RazorPage = "DownloadToEditorPcService",
                    Category = (int)ActivityLogCategory.IbrProcessingApi,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }


        }

        #endregion
    }
}
