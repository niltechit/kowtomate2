using CutOutWiz.Core;
using CutOutWiz.Services.Models.FtpModels;
using FluentFTP;
using CutOutWiz.Core.Utilities;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.OrderAndOrderItemStatusChangeLogServices;
using CutOutWiz.Services.Logs;
using CutOutWiz.Services.OrderItemStatusChangeLogService;
using CutOutWiz.Services.OrderTeamServices;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.Management;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.BLL.AssignOrderAndItem;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.Models.Security;
using System.IO;
using static System.Net.WebRequestMethods;
using Amazon.S3.Model;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.BLL;
using static Google.Apis.Requests.BatchRequest;
using CutOutWiz.Services.BLL.StatusChangeLog;
using FluentFTP.Helpers;
using Mailjet.Client.Resources.SMS;
using System.Threading.Tasks;
using CutOutWiz.Services.IbrApiServices;
using CutOutWiz.Services.Models.IBRModels;
using Mailjet.Client.Resources;
using System.Net;
using Amazon;
using System.Buffers.Text;
using System;
using Google.Api.Gax.ResourceNames;
using System.Diagnostics;
using CutOutWiz.Core.OrderTeams;
using System.Net.Http;
using System.Transactions;
using System.Security.Authentication;
using CutOutWiz.Services.EmailMessage;
using CutOutWiz.Services.Models.EmailModels;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Microsoft.AspNetCore.Components.Forms;
using Org.BouncyCastle.Asn1.X9;
using Renci.SshNet.Messages;
using System.Reflection;
using static Org.BouncyCastle.Math.EC.ECCurve;
using RestSharp;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.IO.Compression;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.BLL.OrderAttachment;
using System.Text;
using CutOutWiz.Services.PathReplacementServices;
using Microsoft.Extensions.Configuration;
using System.Collections;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public class FtpOrderProcessService : IFtpOrderProcessService
    {



        // private readonly IClientExternalOrderFTPSetupService _clientExternalOrderFTPSetupService;
        // private readonly ICompanyTeamService _companyTeamService;
        // private readonly IClientOrderService _orderService;
        // private readonly IClientOrderItemService _clientOrderItemService;
        // private readonly IOrderStatusChangeLogService _orderStatusChangeLogService;
        // private readonly ILogServices _activityLogService;
        // private readonly IOrderItemStatusChangeLogService _orderItemStatusChangeLogService;
        // private readonly IFileServerService _fileServerService;
        // private readonly ICompanyService _companyService;
        // private readonly IFtpFilePathService _ftpFilePathService;
        // private readonly IOrderTeamService _orderTeamService;
        // private readonly IOrderStatusService _orderStatusService;
        // private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        // private readonly ITeamMemberService _teamMemberService;
        // private readonly IAssingOrderItemService _assingOrderItemService;
        // private readonly ICompanyGeneralSettingService _companyGeneralSettingService;
        // private readonly IFluentFtpService _fluentFtpService;
        // private readonly IAutoOrderDeliveryService _autoOrderDeliveryService;
        // private readonly IFtpService _ftpService;
        // private readonly IContactService _contactManager;
        // private readonly IOrderAssignedImageEditorService _orderAssignedImageEditorService;
        // private readonly IActivityAppLogService _activityAppLogService;
        // private readonly IStatusChangeLogBLLService _statusChangeLogBLLService;
        // private readonly IWorkflowEmailService _workflowEmailService;
        // private readonly IIbrApiService _ibrApiService;
        // private readonly ISshNetService _sshNetService;
        // private readonly IOrderAttachmentBLLService _orderAttachmentBLLService;
        // private readonly IPathReplacementService _pathReplacementService;
        // private readonly IConfiguration _configuration;

        // public FtpOrderProcessService(IClientExternalOrderFTPSetupService clientExternalOrderFTPSetupService,
        //     ICompanyTeamService companyTeamService, IClientOrderService orderService,
        //     IClientOrderItemService clientOrderItemService,
        //     IOrderStatusChangeLogService orderStatusChangeLogService,
        //     ILogServices activityLogService,
        //     IOrderItemStatusChangeLogService orderItemStatusChangeLogService,
        //     IFileServerService fileServerService,
        //     IFtpFilePathService ftpFilePathService,
        //     ICompanyService companyService,
        //     IOrderTeamService orderTeamService,
        //     IOrderStatusService orderStatusService,
        //     IUpdateOrderItemBLLService updateOrderItemBLLService,
        //     ITeamMemberService teamMemberService,
        //     IAssingOrderItemService assingOrderItemService,
        //     IFtpService ftpService,
        //     IContactService contactService,
        //     ICompanyGeneralSettingService companyGeneralSettingService,
        //     IFluentFtpService fluentFtpService,
        //     IAutoOrderDeliveryService autoOrderDeliveryService,
        //     IOrderAssignedImageEditorService orderAssignedImageEditorService,
        //     IActivityAppLogService activityAppLogService,
        //     IStatusChangeLogBLLService statusChangeLogBLLService,

        //     IIbrApiService ibrApiService,
        //     IWorkflowEmailService workflowEmailService,
        //     ISshNetService sshNetService,
        //     IOrderAttachmentBLLService orderAttachmentBLLService,
        //     IPathReplacementService pathReplacementService,
        //     IConfiguration configuration
        //     )
        // {
        //     _clientExternalOrderFTPSetupService = clientExternalOrderFTPSetupService;
        //     _companyTeamService = companyTeamService;
        //     _orderService = orderService;
        //     _clientOrderItemService = clientOrderItemService;
        //     _orderStatusChangeLogService = orderStatusChangeLogService;
        //     _activityLogService = activityLogService;
        //     _orderItemStatusChangeLogService = orderItemStatusChangeLogService;
        //     _fileServerService = fileServerService;
        //     _companyService = companyService;
        //     _ftpFilePathService = ftpFilePathService;
        //     _orderTeamService = orderTeamService;
        //     _orderStatusService = orderStatusService;
        //     _updateOrderItemBLLService = updateOrderItemBLLService;
        //     _teamMemberService = teamMemberService;
        //     _assingOrderItemService = assingOrderItemService;
        //     _ftpService = ftpService;
        //     _contactManager = contactService;
        //     _companyGeneralSettingService = companyGeneralSettingService;
        //     _fluentFtpService = fluentFtpService;
        //     _autoOrderDeliveryService = autoOrderDeliveryService;
        //     _orderAssignedImageEditorService = orderAssignedImageEditorService;
        //     _activityAppLogService = activityAppLogService;
        //     _statusChangeLogBLLService = statusChangeLogBLLService;
        //     _ibrApiService = ibrApiService;
        //     _workflowEmailService = workflowEmailService;
        //     _sshNetService = sshNetService;
        //     _orderAttachmentBLLService = orderAttachmentBLLService;
        //     _pathReplacementService = pathReplacementService;
        //     _configuration = configuration;
        // }




        // public async Task<Response<bool>> MakeOrderStatusOrderPlacingToOrderPlaced(int consoleAppId)
        // {

        //     var response = new Response<bool>();
        //     try
        //     {
        //         string query = $"SELECT cc.Id,cc.Host,cc.ClientCompanyId,cc.Port,cc.Username,cc.Password,cc.IsEnable,cc.OutputRootFolder,cc.InputRootFolder FROM [dbo].[Client_ClientOrderFtp] as cc inner join dbo.CompanyGeneralSettings as cgs on cc.ClientCompanyId = cgs.CompanyId where cc.IsEnable = 1 and cgs.EnableFtpOrderPlacement = 1 AND cgs.FtpOrderPlacedAppNo={consoleAppId}";
        //         var tempClientOrderFtps = await _fileServerService.GetAllClientFtpByQuery(query);
        //         foreach (var clientFtp in tempClientOrderFtps)
        //         {
        //             if (AutomatedAppConstant.SixCompanyId == clientFtp.ClientCompanyId)
        //             {
        //                 continue;
        //             }

        //             var orders = await _orderService.GetAllByStatus((int)InternalOrderStatus.OrderPlacing, (int)clientFtp.ClientCompanyId);
        //             foreach (var order in orders)
        //             {
        //                 await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
        //                 await _orderStatusService.UpdateOrderArrivalTime(order);
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error line no 375: {ex.Message}");

        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "MakeOrderStatusOrderPlacingToOrderPlaced",
        //             RazorPage = "FtpOrderProcessService - VC - Console Application",
        //             Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }
        //     return response;

        // }

        // private async Task<Response<List<string>>> ReadNewOrderFoldersAsync(FtpCredentails sourceFtpCredential)
        // {
        //     var response = new Response<List<string>>();

        //     try
        //     {
        //         FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
        //         using (var ftp = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
        //         {
        //             //Read only the root folders 
        //             FtpListItem[] ftpListItems = await ftp.GetListing(sourceFtpCredential.RootFolder);

        //             List<string> folders = new List<string>();

        //             //foreach (var ftpListItem in ftpListItems)
        //             //{
        //             //	if (ftpListItem.Type == FtpObjectType.Directory)
        //             //	{
        //             //		if (!ftpListItem.FullName.Contains("_downloaded"))
        //             //		{
        //             //			folders.Add(ftpListItem.FullName);
        //             //		}

        //             //	}
        //             //}

        //             Parallel.ForEach(ftpListItems, ftpListItem =>
        //             {
        //                 if (ftpListItem.Type == FtpObjectType.Directory && !ftpListItem.FullName.Contains("_downloaded"))
        //                 {
        //                     lock (folders)
        //                     {
        //                         folders.Add(ftpListItem.FullName);
        //                     }
        //                 }
        //             });

        //             response.Result = folders;
        //             response.IsSuccess = true;

        //             await ftp.Disconnect();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         response.Message = ex.Message;
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "ReadNewOrderFoldersAsync",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);

        //     }

        //     return response;
        // }



        // /// <summary>
        // /// Copy file from source FTP to Destination FTP
        // /// </summary>
        // /// <param name="ftpFileCopyRequest"></param>
        // /// <returns></returns>



        // private async Task<bool> CompareByteArraysAsync(byte[] sourceBytesArray, byte[] destinationBytesArray)
        // {
        //     // Check if the arrays have different lengths, and return false if they do.
        //     if (sourceBytesArray.Length != destinationBytesArray.Length)
        //     {
        //         return false;
        //     }

        //     // Use Task.Run to perform the byte-by-byte comparison in a separate thread
        //     // while not blocking the calling thread.
        //     return await Task.Run(() =>
        //     {
        //         for (int i = 0; i < sourceBytesArray.Length; i++)
        //         {
        //             if (sourceBytesArray[i] != destinationBytesArray[i])
        //             {
        //                 return false;
        //             }
        //         }
        //         return true;
        //     });
        // }




        // private async Task DownloadOrderItemInEditorsPc(ClientOrder order, List<ClientOrderItem> itemsToSendForEditors,
        //             CompanyGeneralSetting companyGeneralSetting, string ibrToken, string ibrOrderId, string model_base_url)
        // {
        //     var serverInfo = await _fileServerService.GetById((int)order.FileServerId);

        //     try
        //     {
        //         var semaphore = new SemaphoreSlim(4);

        //         var tasks = new List<Task>();

        //         //string folderPath = @$"{AutomatedAppConstant.timeTrackerUrl}\{DateTime.Now.ToShortDateString().Replace("/", "_")}";
        //         //string fileName = "processing_time.txt";
        //         //string filePath = Path.Combine(folderPath, fileName);
        //         double totalProcessingTime = 0;
        //         double totalPsdDownloadTime = 0;
        //         double numberOfImages = 0;

        //         // Create the directory if it doesn't exist
        //         //if (!Directory.Exists(folderPath))
        //         //{
        //         //    Directory.CreateDirectory(folderPath);
        //         //}

        //         //string text = "DateTime: " + DateTime.Now + ", Order Number: " + order.OrderNumber + Environment.NewLine;
        //         //using (StreamWriter sw = new StreamWriter(filePath, true))
        //         //{
        //         //    sw.WriteLine(text);
        //         //}

        //         //System.IO.File.WriteAllText(filePath,"DateTime: "+ DateTime.Now +", Order Number: "+order.OrderNumber + Environment.NewLine);
        //         Console.WriteLine("File Write Done");

        //         var contactImages = from p in itemsToSendForEditors
        //                             group p by p.EditorContactId into g
        //                             select new { EditorContactId = g.Key, Items = g.ToList() };

        //         var successItemList = new List<ClientOrderItem>();
        //         //Start of Item Loops

        //         foreach (var contactImageGroup in contactImages)
        //         {
        //             // Wait for a slot to become available in the semaphore
        //             await semaphore.WaitAsync();

        //             tasks.Add(Task.Run(async () =>
        //             {
        //                 try
        //                 {
        //                     var contactFromImages = contactImageGroup.Items.FirstOrDefault();

        //                     bool isAllDownloadSuccessfully = true;
        //                     var orderItemList = contactImageGroup.Items; //await _clientOrderItemService.GetDistributedClientOrderItemByEditorContactIdAndOrderId(contactImageGroup.EditorContactId, order.Id);



        //                     if (orderItemList.Any())
        //                     {
        //                         List<ClientOrderItem> successfulleDownloadedItems = new List<ClientOrderItem>();
        //                         DateTime currentDateTime = DateTime.Now;
        //                         string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");
        //                         //CutOutWiz.Data.Security.Contact contact = await _contactManager.GetById(contactImageGroup.EditorContactId);
        //                         var sharedFolderDownloadPath = contactFromImages.EditorDownloadFolderPath;

        //                         //if (contact != null)
        //                         //{
        //                         //	sharedFolderDownloadPath = contact.DownloadFolderPath;
        //                         //}

        //                         if (!string.IsNullOrWhiteSpace(sharedFolderDownloadPath) && Directory.Exists(sharedFolderDownloadPath))
        //                         {
        //                             FileUploadVM model = new FileUploadVM();
        //                             model.FtpUrl = serverInfo.Host;
        //                             model.userName = serverInfo.UserName;
        //                             model.password = serverInfo.Password;
        //                             model.OrderNumber = order.OrderNumber;
        //                             model.Date = order.CreatedDate;
        //                             model.ContactName = contactFromImages.EditorFirstName.Trim() + " " + contactFromImages.EditorContactId;

        //                             using (var client = _ftpService.CreateAsyncFtpClient(model))
        //                             {
        //                                 client.Config.EncryptionMode = FtpEncryptionMode.Auto;
        //                                 client.Config.ValidateAnyCertificate = true;
        //                                 await client.AutoConnect();

        //                                 var semaphoreItems = new SemaphoreSlim(4);

        //                                 var taskItems = new List<Task>();

        //                                 foreach (var orderItem in orderItemList)
        //                                 {
        //                                     var dataSavePath = "";
        //                                     string downloadingFolderPath = ""; //1
        //                                     try
        //                                     {
        //                                         model.fileName = orderItem.FileName;
        //                                         model.UploadDirectory = Path.GetDirectoryName(orderItem.InternalFileInputPath);
        //                                         model.DownloadFolderName = orderItem.PartialPath;

        //                                         if (!string.IsNullOrEmpty(sharedFolderDownloadPath))
        //                                         {
        //                                             if (string.IsNullOrEmpty(model.DownloadFolderName))
        //                                             {
        //                                                 dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{order.OrderNumber}";
        //                                                 downloadingFolderPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "_Downloading\\" + $"{formattedDateTimeForDownload}" + "\\" + $"{order.OrderNumber}";//2
        //                                             }
        //                                             else
        //                                             {
        //                                                 dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + $"{model.DownloadFolderName}";
        //                                                 downloadingFolderPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "_Downloading\\" + $"{formattedDateTimeForDownload}" + "\\" + $"{order.OrderNumber}";//3

        //                                             }

        //                                             var localPath = $"{downloadingFolderPath}/{model.fileName}";
        //                                             var remotePath = $"{model.UploadDirectory}/{model.fileName}";

        //                                             if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
        //                                             {
        //                                                 remotePath = $"{serverInfo.SubFolder}/{model.UploadDirectory}/{model.fileName}";
        //                                             }


        //                                             //IBr Connect 

        //                                             if (companyGeneralSetting.IsIbrProcessedEnabled && !string.IsNullOrEmpty(ibrOrderId))
        //                                             {
        //                                                 var saveFileAs = Path.ChangeExtension(model.fileName, ".psd");
        //                                                 var localPathForIbrDownload = $"{downloadingFolderPath}/{saveFileAs}";

        //                                                 var tempStream = await client.OpenRead(remotePath);
        //                                                 byte[] downloadImagebytes;
        //                                                 using (MemoryStream ms = new MemoryStream())
        //                                                 {
        //                                                     tempStream.CopyTo(ms);
        //                                                     downloadImagebytes = ms.ToArray();
        //                                                     tempStream.Close();
        //                                                 }


        //                                                 DateTime ibrCallingTime = DateTime.Now;
        //                                                 Stopwatch stopwatch = Stopwatch.StartNew();
        //                                                 //api call
        //                                                 var response = await _ibrApiService.RequestForIbrProcess(ibrOrderId, downloadImagebytes, ibrToken, model.fileName, model_base_url);
        //                                                 stopwatch.Stop();
        //                                                 DateTime ibrResponsedTime = DateTime.Now;
        //                                                 double processingTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
        //                                                 totalProcessingTime += processingTimeInSeconds;
        //                                                 string processingTimeStr = $"{processingTimeInSeconds:F2} seconds";


        //                                                 string newRow = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} ,Ibr Calling Time: {ibrCallingTime} , Ibr Responsed Time: {ibrResponsedTime} ,Processing Time: {processingTimeInSeconds:F2} Second";


        //                                                 if (!Directory.Exists(downloadingFolderPath))
        //                                                 {
        //                                                     Directory.CreateDirectory(downloadingFolderPath);
        //                                                 }
        //                                                 if (!string.IsNullOrWhiteSpace(response.Result))
        //                                                 {
        //                                                     using (HttpClient httpClient = new HttpClient())
        //                                                     {
        //                                                         int i = 0;
        //                                                         while (true)
        //                                                         {
        //                                                             try
        //                                                             {
        //                                                                 Stopwatch stopwatchforDownload = Stopwatch.StartNew();
        //                                                                 //webClient.DownloadFile(response.Result, localPathForIbrDownload);
        //                                                                 using var httpDlResponse = await httpClient.GetAsync(response.Result);
        //                                                                 using var stream = await httpDlResponse.Content.ReadAsStreamAsync();
        //                                                                 using var fileStream = new FileStream(localPathForIbrDownload, FileMode.Create);
        //                                                                 await stream.CopyToAsync(fileStream);
        //                                                                 stopwatchforDownload.Stop();
        //                                                                 fileStream.Close();
        //                                                                 stream.Close();
        //                                                                 string message = $"File DownloadPath:{localPathForIbrDownload} on {DateTime.Now}";
        //                                                                 await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, message);

        //                                                                 double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;
        //                                                                 totalPsdDownloadTime += downloadTimeInSeconds;
        //                                                                 newRow = newRow + $", PSD Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

        //                                                                 //using (StreamWriter sw = new StreamWriter(filePath, true)) // Use "true" to append to existing file
        //                                                                 //{
        //                                                                 //    sw.WriteLine(newRow);
        //                                                                 //}

        //                                                                 break;
        //                                                             }
        //                                                             catch (Exception ex)
        //                                                             {
        //                                                                 Console.WriteLine(ex.ToString());
        //                                                                 i++;
        //                                                                 await Task.Delay(500);

        //                                                                 if (i >= 3)
        //                                                                 {
        //                                                                     Stopwatch stopwatchforDownload = Stopwatch.StartNew();
        //                                                                     await client.DownloadFile(localPath, remotePath);
        //                                                                     stopwatchforDownload.Stop();
        //                                                                     double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;

        //                                                                     string newRowForRawDownload = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} , Date:{DateTime.Now},Processing Time: {processingTimeInSeconds:F2} Second (Failed !)" + $" Raw Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

        //                                                                     //using (StreamWriter sw = new StreamWriter(filePath, true))
        //                                                                     //{
        //                                                                     //    sw.WriteLine(newRowForRawDownload);
        //                                                                     //}

        //                                                                     await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)orderItem.Id, localPath + "Note: Trying Three times download from Ibr ");

        //                                                                     break;
        //                                                                 }
        //                                                             }
        //                                                         }

        //                                                     }
        //                                                 }
        //                                                 else
        //                                                 {
        //                                                     Stopwatch stopwatchforDownload = Stopwatch.StartNew();
        //                                                     await client.DownloadFile(localPath, remotePath);
        //                                                     stopwatchforDownload.Stop();
        //                                                     double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;

        //                                                     string newRowForRawDownload = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} , Date:{DateTime.Now},Processing Time: {processingTimeInSeconds:F2} Second (Failed !)" + $" Raw Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

        //                                                     //using (StreamWriter sw = new StreamWriter(filePath, true))
        //                                                     //{
        //                                                     //    sw.WriteLine(newRowForRawDownload);
        //                                                     //}
        //                                                 }
        //                                             }
        //                                             else
        //                                             {
        //                                                 int i = 0;
        //                                                 while (true)
        //                                                 {
        //                                                     try
        //                                                     {
        //                                                         Stopwatch stopwatchforDownload = Stopwatch.StartNew();
        //                                                         await client.DownloadFile(localPath, remotePath);
        //                                                         stopwatchforDownload.Stop();
        //                                                         double downloadTimeInSeconds = stopwatchforDownload.Elapsed.TotalSeconds;

        //                                                         string newRowForRawDownload = $"	Image Name:{orderItem.FileName},file Size: {orderItem.FileSize} , Date:{DateTime.Now}" + $", Raw Download Time: {downloadTimeInSeconds:F2} Second" + Environment.NewLine;

        //                                                         //using (StreamWriter sw = new StreamWriter(filePath, true))
        //                                                         //{
        //                                                         //    sw.WriteLine(newRowForRawDownload);
        //                                                         //}

        //                                                         break;
        //                                                     }
        //                                                     catch (Exception ex)
        //                                                     {
        //                                                         Console.WriteLine(ex.ToString());
        //                                                         i++;
        //                                                         await Task.Delay(500);

        //                                                         if (i >= 3)
        //                                                         {
        //                                                             orderItemList.RemoveAll(ot => ot.Id == orderItem.Id);
        //                                                             break;
        //                                                         }
        //                                                     }
        //                                                 }
        //                                             }
        //                                         }//Shared Found End

        //                                     } //end of item try
        //                                     catch (Exception ex)
        //                                     {
        //                                         isAllDownloadSuccessfully = false;
        //                                     }
        //                                     finally
        //                                     {
        //                                         semaphoreItems.Release();
        //                                     }



        //                                 }

        //                                 //End of item loop
        //                                 await Task.WhenAll(taskItems);
        //                                 await client.Disconnect();

        //                                 if (isAllDownloadSuccessfully)
        //                                 {
        //                                     int j = 0;
        //                                     while (true)
        //                                     {
        //                                         try
        //                                         {
        //                                             string sourcePath = sharedFolderDownloadPath + $"/{model.ContactName}/_Downloading/{formattedDateTimeForDownload}/{order.OrderNumber}";
        //                                             string destinationPath = sharedFolderDownloadPath + $"/{model.ContactName}/Raw/{formattedDateTimeForDownload}";

        //                                             if (companyGeneralSetting.AllowClientWiseImageProcessing)
        //                                             {
        //                                                 DateTime downloadDateTime = DateTime.Now;
        //                                                 TimeSpan targetTime = new TimeSpan(23, 0, 0);

        //                                                 if (downloadDateTime.TimeOfDay >= targetTime)
        //                                                 {
        //                                                     downloadDateTime = downloadDateTime.AddDays(1);
        //                                                 }
        //                                                 string formattedDateForDownload = downloadDateTime.ToString("dd-MM-yyyy");
        //                                                 Company company = await _companyService.GetById(companyGeneralSetting.CompanyId);
        //                                                 string[] files = Directory.GetFiles(sourcePath);
        //                                                 string destinationPathForClientWiseImageProcessing = sharedFolderDownloadPath + $"/{model.ContactName}/Raw/{formattedDateForDownload}/{company.Code}";
        //                                                 if (!Directory.Exists(destinationPathForClientWiseImageProcessing))
        //                                                 {
        //                                                     Directory.CreateDirectory(destinationPathForClientWiseImageProcessing);
        //                                                 }
        //                                                 foreach (string file in files)
        //                                                 {
        //                                                     string imageName = Path.GetFileName(file);
        //                                                     string tempPath = Path.Combine(destinationPathForClientWiseImageProcessing, imageName);
        //                                                     if (System.IO.File.Exists(tempPath))
        //                                                     {
        //                                                         System.IO.File.Delete(tempPath);
        //                                                     }
        //                                                     System.IO.File.Move(file, tempPath);

        //                                                 }
        //                                                 var editorUploadPath = sharedFolderDownloadPath + $"/{model.ContactName}/Completed/{company.Code}/";
        //                                                 if (!Directory.Exists(editorUploadPath))
        //                                                 {
        //                                                     Directory.CreateDirectory(editorUploadPath);
        //                                                 }
        //                                             }
        //                                             else
        //                                             {
        //                                                 if (!Directory.Exists(sourcePath))
        //                                                 {
        //                                                     return;
        //                                                 }

        //                                                 string rawDirectory = sharedFolderDownloadPath + $"/{model.ContactName}/Raw/";

        //                                                 if (!Directory.Exists(rawDirectory))
        //                                                 {
        //                                                     Directory.CreateDirectory(rawDirectory);
        //                                                 }

        //                                                 Directory.Move(sourcePath, destinationPath);
        //                                             }

        //                                             var uploadedPath = sharedFolderDownloadPath + $"/{model.ContactName}/Completed/_uploaded/";
        //                                             if (!Directory.Exists(uploadedPath))
        //                                             {
        //                                                 Directory.CreateDirectory(uploadedPath);
        //                                             }



        //                                             break;
        //                                         }
        //                                         catch
        //                                         {

        //                                             j++;
        //                                             await Task.Delay(1000);
        //                                             if (j > 3)
        //                                             {
        //                                                 break;
        //                                             }
        //                                         }
        //                                     }

        //                                     if (orderItemList.Count > 0 && j <= 3)
        //                                     {
        //                                         var ReworkInProductionImages = orderItemList.Where(i => i.Status == (int)InternalOrderItemStatus.ReworkDistributed).ToList();
        //                                         var InproductionImages = orderItemList.Where(i => i.Status == (int)InternalOrderItemStatus.Distributed).ToList();
        //                                         if (ReworkInProductionImages.Any())
        //                                         {
        //                                             await _updateOrderItemBLLService.UpdateOrderItemsStatus(ReworkInProductionImages, InternalOrderItemStatus.ReworkInProduction);
        //                                         }
        //                                         else if (InproductionImages.Any())
        //                                         {
        //                                             await _updateOrderItemBLLService.UpdateOrderItemsStatus(InproductionImages, InternalOrderItemStatus.InProduction);
        //                                         }
        //                                         await _orderStatusService.UpdateOrderStatus(order, AutomatedAppConstant.ContactId);
        //                                     }
        //                                 }

        //                             }
        //                         }

        //                     }
        //                 }
        //                 finally
        //                 {
        //                     semaphore.Release();
        //                 }
        //             }));

        //         }
        //         //End of all Item loops


        //         await Task.WhenAll(tasks);
        //         string row = "TotalProcessing Time: " + totalProcessingTime + "TotalPsdDownloadTime: " + totalPsdDownloadTime + Environment.NewLine;

        //         //using (StreamWriter sw = new StreamWriter(filePath, true)) // Use "true" to append to existing file
        //         //{
        //         //    sw.WriteLine(row);
        //         //}
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.ToString());
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "DownloadOrderItemInEditorsPc",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }


        // }

        // //Create Order
        // private async Task<Response<ClientOrder>> AddOrderInfo(Company company, FileServer fileServer, long sourceFtpId, string orderDirectory = "")
        // {
        //     var response = new Response<ClientOrder>();
        //     var order = new ClientOrder();

        //     try
        //     {
        //         if (order.Id > 0)
        //         {
        //             response.Message = "Order already have an id.";
        //             return response;
        //         }

        //         order.CreatedByContactId = AutomatedAppConstant.ContactId;
        //         order.UpdatedByContactId = AutomatedAppConstant.ContactId;
        //         order.SourceServerId = sourceFtpId;
        //         Thread.Sleep(2000);
        //         var dateTime = DateTime.Now;



        //         order.OrderNumber = $"{company.Code}-{company.Id}-{dateTime.ToString("ddMMyyyyHHmmss")}";
        //         Console.WriteLine(order.OrderNumber);
        //         order.ObjectId = Guid.NewGuid().ToString();
        //         order.CreatedDate = DateTime.Now;
        //         order.UpdatedDate = DateTime.Now;
        //         order.OrderPlaceDate = DateTime.Now;
        //         order.CompanyId = company.Id;
        //         order.ExternalOrderStatus = (byte)EnumHelper.ExternalOrderStatusChange(InternalOrderStatus.OrderPlacing);
        //         order.InternalOrderStatus = (byte)InternalOrderStatus.OrderPlacing;
        //         order.FileServerId = fileServer.Id;
        //         order.OrderType = (int)OrderType.NewWork;
        //         order.BatchPath = orderDirectory;
        //         var companyTeam = await _companyTeamService.GetByCompanyId(company.Id);

        //         if (companyTeam != null && companyTeam.Count > 0)
        //         {
        //             var getFirstOrDefaultCompany = companyTeam.FirstOrDefault();
        //             order.AssignedTeamId = getFirstOrDefaultCompany.TeamId;
        //         }
        //         else
        //         {
        //             order.AssignedTeamId = null;
        //         }

        //         var addResponse = await
        //         (order);

        //         if (!addResponse.IsSuccess)
        //         {
        //             response.Message = addResponse.Message;
        //             response.IsSuccess = false;
        //             return response;
        //         }

        //         order.Id = addResponse.Result;
        //         response.IsSuccess = true;
        //         response.Result = order;

        //         await _statusChangeLogBLLService.AddOrderStatusChangeLog(order, InternalOrderStatus.OrderPlacing, AutomatedAppConstant.ContactId);
        //     }
        //     catch (Exception ex)
        //     {
        //         response.Message = ex.Message;
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };

        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "AddOrderInfo",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //         };

        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }

        //     return response;
        // }

        // //Add Order Item
        // private async Task<Response<long>> AddOrderItem(ClientOrderItem clientOrderItem, int companyId, long orderId, InternalOrderItemStatus status = 0)
        // {
        //     Response<long> addItemResponse = null;
        //     try
        //     {
        //         clientOrderItem.IsDeleted = false;
        //         clientOrderItem.ObjectId = Guid.NewGuid().ToString();
        //         clientOrderItem.CreatedByContactId = AutomatedAppConstant.ContactId; //Dummy
        //         clientOrderItem.FileGroup = (int)OrderItemFileGroup.Work;

        //         //Set status
        //         if (status > 0)
        //         {
        //             clientOrderItem.Status = (byte)status;
        //             clientOrderItem.ExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(status);
        //         }

        //         var companyTeam = await _companyTeamService.GetByCompanyId(companyId);
        //         if (companyTeam != null)
        //         {
        //             var getFirstOrDefaultCompany = companyTeam.FirstOrDefault();

        //             if (clientOrderItem.TeamId != null && clientOrderItem.TeamId > 0)
        //             {
        //                 clientOrderItem.TeamId = getFirstOrDefaultCompany.TeamId;
        //             }

        //         }

        //         //Add Order Item / Files in database 
        //         addItemResponse = await _clientOrderItemService.Insert(clientOrderItem, orderId);
        //         Console.WriteLine(clientOrderItem.FileName + " " + addItemResponse.Message.ToString());
        //         if (addItemResponse.IsSuccess)
        //         {
        //             clientOrderItem.Id = addItemResponse.Result;
        //             //order.orderItems.Add(clientOrderItem);
        //             if (status > 0)
        //             {
        //                 await _statusChangeLogBLLService.AddOrderItemStatusChangeLog(clientOrderItem, status, AutomatedAppConstant.ContactId);
        //             }
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "AddOrderInfo",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }
        //     return addItemResponse;
        // }

        // private async Task UpdateOrder(ClientOrder clientOrder, InternalOrderStatus status)
        // {
        //     try
        //     {
        //         clientOrder.InternalOrderStatus = (byte)status;
        //         clientOrder.ExternalOrderStatus = (byte)EnumHelper.ExternalOrderStatusChange(status);
        //         await _orderService.UpdateClientOrderStatus(clientOrder);
        //     }
        //     catch (Exception ex)
        //     {
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "UpdateOrder",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }
        // }



        // private async Task<bool> ExtraImageFileAddWithAssignOwnUploader(ClientOrder clientOrder, long clientOrderItemId, Data.Security.Contact contact)
        // {
        //     OrderAssignedImageEditor assignedImage = new OrderAssignedImageEditor
        //     {
        //         OrderId = clientOrder.Id,
        //         AssignByContactId = AutomatedAppConstant.ContactId,
        //         AssignContactId = contact.Id,
        //         Order_ImageId = clientOrderItemId,
        //         ObjectId = Guid.NewGuid().ToString(),
        //         UpdatedByContactId = AutomatedAppConstant.ContactId
        //     };
        //     List<OrderAssignedImageEditor> assignedImages = new List<OrderAssignedImageEditor>();
        //     assignedImages.Add(assignedImage);//Todo Rakib
        //     var addResponse = await _orderAssignedImageEditorService.Insert(assignedImages);
        //     return addResponse.IsSuccess;
        // }




        // public async Task<int> GetEditorCapacity(int contactId)
        // {
        //     DateTime now = DateTime.Now;
        //     DateTime currentShiftEndTime = new DateTime();
        //     if (now.Hour < 15 && now.Hour >= 7)
        //     {
        //         currentShiftEndTime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0); // 3 pm
        //     }
        //     else if (now.Hour < 23 && now.Hour >= 15)
        //     {
        //         currentShiftEndTime = new DateTime(now.Year, now.Month, now.Day, 23, 0, 0); // 11 pm
        //     }
        //     else if (now.Hour < 7 || now.Hour >= 23)
        //     {

        //         currentShiftEndTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0); // 7 am

        //         currentShiftEndTime = currentShiftEndTime.AddDays(1);
        //     }

        //     TimeSpan remainingTime = currentShiftEndTime - now;
        //     int remainingMinutes = (int)remainingTime.TotalMinutes;

        //     if (remainingMinutes > 60)
        //     {
        //         remainingMinutes = 60;
        //     }
        //     double editorCapability = remainingMinutes / AutomatedAppConstant.minPerImage;

        //     ClientOrderItemCount editorClientOrderItemCount = await _clientOrderItemService.GetTotalPrductionOngoingItem(contactId);

        //     int editorActualCapability = (int)editorCapability - editorClientOrderItemCount.TotalPrductionOngoingItem;

        //     return editorActualCapability;

        // }

        // #region Email Notification Methods




        //private List<List<string>> GetFilesChunksWithPaths(List<string> allFiles, int chunkSize)
        //{
        //    var chunks = new List<List<string>>();

        //    int count = 0;
        //    var chunk = new List<string>();
        //    int i = 0;
        //    foreach (var filePath in allFiles)
        //    {

        //        count++;
        //        chunk.Add(filePath);

        //        if (count == chunkSize)
        //        {
        //            chunks.Add(chunk);
        //            chunk = new List<string>();
        //            count = 0;
        //        }

        //        i++;
        //    }
        //    if (count > 0)
        //    {
        //        chunks.Add(chunk);
        //    }

        //    return chunks;
        //}
        // private async Task SendEmailToOpsToNotifyOrderUpload(string batchName, string orderNumber, Company company, string folderName = " ", string userName = " ", int numberOfImages = 0)
        // {

        //     try
        //     {

        //         //ToDo: Rakib need to add from db
        //         List<string> opsEmailList = new List<string>()
        //                     {
        //                         "rakibul@thekowcompany.com",
        //	//"anik@thekowcompany.com",
        //	//"zico@thekowcompany.com",
        //	//"raihan@thekowcompany.com",
        //	"ops@thekowcompany.com",
        //                         "mashfiq@thekowcompany.com",
        //	//"ak@thekowcompany.com",
        //	"zakir@thekowcompany.com"
        //                     };
        //         FTPOrderNotifyOpsOnImageArrivalFTP fTPOrderNotifyOpsOnImageArrivalFTP = new FTPOrderNotifyOpsOnImageArrivalFTP
        //         {
        //             EmailAddresses = opsEmailList,
        //             MailType = "OrderPlaceOnKTM",
        //             //ImageCount = $"{imageCount}  ,  UserName:{clientFtp.Username}",

        //             //OrderType = $"NAN",
        //             CompanyName = $"{company.Name}",
        //             BatchName = $"{batchName} ,Folder Name:{folderName}   , Username:{userName}",
        //             OrderNumber = $"{orderNumber} , Image Count: {(numberOfImages > 0 ? numberOfImages.ToString() : "N/A")}",
        //         };
        //         await _workflowEmailService.SendEmailToOpsToNotifyOrderUpload(fTPOrderNotifyOpsOnImageArrivalFTP);
        //     }
        //     catch (Exception ex)
        //     {
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "SendEmailToOpsToNotifyOrderUpload",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }


        // }

        // private async Task SendEmailToOpsToNotifyOrderDeliveryToClient(string orderNumber, Company company, string imageCount)
        // {

        //     try
        //     {

        //         //ToDo: Rakib need to add from db
        //         List<string> opsEmailList = new List<string>()
        //                     {
        //                         "rakibul@thekowcompany.com",
        //	//"anik@thekowcompany.com",
        //	//"zico@thekowcompany.com",
        //	//"raihan@thekowcompany.com",
        //	"ops@thekowcompany.com",
        //                         "mashfiq@thekowcompany.com"
        //	//"ak@thekowcompany.com",
        //	//"zakir@thekowcompany.com"
        //};
        //         FTPOrderNotifyOpsOnImageArrivalFTP fTPOrderNotifyOpsOnImageArrivalFTP = new FTPOrderNotifyOpsOnImageArrivalFTP
        //         {
        //             EmailAddresses = opsEmailList,
        //             MailType = "OrderDeliveryToClientByKTM",
        //             //ImageCount = $"{imageCount}  ,  UserName:{clientFtp.Username}",

        //             //OrderType = $"NAN",
        //             CompanyName = $"{company.Name}",
        //             ImageCount = imageCount,
        //             OrderNumber = $"{orderNumber}",
        //         };
        //         await _workflowEmailService.SendEmailToOpsToNotifyOrderDeliveryToClient(fTPOrderNotifyOpsOnImageArrivalFTP);
        //     }
        //     catch (Exception ex)
        //     {
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "SendEmailToOpsToNotifyOrderUpload",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //     }


        // }

        // private async Task<int> GetImageCount(AsyncFtpClient ftp, string directoryPath)
        // {
        //     try
        //     {
        //         FtpListItem[] ftpListItems = await ftp.GetListing(directoryPath, FtpListOption.Recursive);
        //         if (ftpListItems == null || !ftpListItems.Any())
        //         {
        //             return 0;
        //         }
        //         return ftpListItems.Count(entry => entry.Type == FtpObjectType.File);
        //     }

        //     catch (Exception ex)
        //     {
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "GetImageCount",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //         return 0;
        //     }

        // }

        // private async Task MoveToNotifiedFiles(AsyncFtpClient ftp, string sourceFilePath, string targetFolderPath)
        // {
        //     try
        //     {
        //         if (!await ftp.DirectoryExists(targetFolderPath))
        //             await ftp.CreateDirectory(targetFolderPath);
        //         FtpListItem[] ftpListItems = await ftp.GetListing(sourceFilePath, FtpListOption.Recursive);
        //         if (ftpListItems == null || !ftpListItems.Any())
        //         {
        //             return;
        //         }
        //         foreach (var ftpListItem in ftpListItems)
        //         {
        //             if (ftpListItem.Type == FtpObjectType.File)
        //             {
        //                 await ftp.MoveFile(sourceFilePath + "/" + Path.GetFileName(ftpListItem.FullName), targetFolderPath + "/" + Path.GetFileName(ftpListItem.FullName));
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         var loginUser = new LoginUserInfoViewModel
        //         {
        //             ContactId = AutomatedAppConstant.ContactId
        //         };
        //         CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //         {
        //             //PrimaryId = (int)order.Id,
        //             ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
        //             loginUser = loginUser,
        //             ErrorMessage = ex.Message,
        //             MethodName = "MoveToNotifiedFiles",
        //             RazorPage = "FtpOrderProcessService",
        //             Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
        //         };
        //         await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //         return;
        //     }

        // }



        // #endregion

        // #region Order Place For Six  Private Method



        // #endregion

        // #region Read File From sftp








        //private async Task<bool> AddOrderItemAsync(ClientExternalOrderFTPSetup sourceFtp, FtpCredentails destinationFtpCredentails, Company company, string rootFolder, bool isMoveSingleFile, Response<ClientOrder> orderSaveResponse, bool uploadSuccessful, AsyncFtpClient destinationClient, string path)
        //{
        //    var orderItemAddToDbandUploadToStorage = 0;
        //    //while (true)
        //    //{
        //    try
        //    {
        //        Console.WriteLine("Start Read" + DateTime.Now);
        //        var readStartTime = DateTime.Now;

        //        var companyGeneralSetting = await _companyGeneralSettingService.GetGeneralSettingByCompanyId(company.Id);

        //        // Prepare the destination path
        //        var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, orderSaveResponse.Result.CreatedDate, orderSaveResponse.Result.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);
        //        var pathArray = path.Split(rootFolder);

        //        if (companyGeneralSetting.IsBatchRootFolderNameAddWithOrder)
        //        {
        //            pathArray[1] = Path.GetFileName(sourceFtp.InputRootFolder) + "/" + pathArray[1];// Path array one means order place folder structure from source ftp
        //        }

        //        var fullFilePathForFtp = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, pathArray[1]);

        //        if (!string.IsNullOrWhiteSpace(destinationFtpCredentails.SubFolder))
        //        {
        //            fullFilePathForFtp = $"{destinationFtpCredentails.SubFolder}/{fullFilePathForFtp}";
        //        }



        //        Console.WriteLine($"Upload to Ftp : {Path.GetFileName(path)}");

        //        uploadSuccessful = await SaveDownloadFileAsync(sourceFtp, company, rootFolder, isMoveSingleFile, orderSaveResponse, uploadSuccessful, destinationClient, path, uploadDirectory, pathArray, fullFilePathForFtp);

        //        SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
        //        sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
        //        sftpClient.Connect();

        //        int maxRetries = 3;
        //        int retryOrderItemUpload = 0;

        //        while (retryOrderItemUpload < maxRetries)
        //        {
        //            try
        //            {
        //                if (!await destinationClient.DirectoryExists(Path.GetDirectoryName(fullFilePathForFtp)))
        //                {
        //                    await destinationClient.CreateDirectory(Path.GetDirectoryName(fullFilePathForFtp));
        //                }

        //                using (var streamToWrite = await destinationClient.OpenWrite(fullFilePathForFtp))
        //                {
        //                    int maxDownloadRetries = 3;
        //                    int fileDownload = 0;

        //                    try
        //                    {
        //                        Console.WriteLine("Start Read" + DateTime.Now);
        //                        var readStartTimeTemp = DateTime.Now;

        //                        //File download on this method
        //                        sftpClient.DownloadFile(path, streamToWrite);

        //                        var uploadDoneTimeTemp = DateTime.Now;
        //                        Console.WriteLine("Upload Finish" + uploadDoneTimeTemp.Subtract(readStartTimeTemp).TotalMinutes);

        //                        // If download is successful, break out of the download retry loop
        //                        //break;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        fileDownload++;

        //                        if (fileDownload >= maxDownloadRetries)
        //                        {
        //                            string methodName = $"File Transfer Error On Ftp Order Place {ex.Message}";
        //                            byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;
        //                            uploadSuccessful = false;
        //                            await LogGeneralError(ex, methodName, errorCategory);

        //                            // If download retries are exhausted, break out of the outer retry loop
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            // If download fails, wait for a moment before retrying
        //                            Thread.Sleep(3000);
        //                        }
        //                    }

        //                }

        //                // File compare to destination to local
        //                var fileBytesArray = sftpClient.ReadAllBytes(path);

        //                var result = await VerifyDownloadedFile(destinationFtpCredentails, fileBytesArray, fullFilePathForFtp);

        //                if (!result)
        //                {
        //                    //If verification fails, increment the retry counter
        //                    retryOrderItemUpload++;
        //                    uploadSuccessful = false;
        //                }
        //                else
        //                {
        //                    //If verification is successful, break out of the retry loop
        //                    uploadSuccessful = true;
        //                    break;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // If any other exception occurs, wait for a moment before retrying
        //                Thread.Sleep(3000);
        //                retryOrderItemUpload++;

        //                if (retryOrderItemUpload >= maxRetries)
        //                {
        //                    string methodName = "AddOrderItemAsync 1";
        //                    byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

        //                    await LogFtpProcessingError(ex, methodName, category);
        //                }
        //            }
        //        }

        //        //break;

        //    }
        //    catch (Exception ex)
        //    {
        //        string methodName = "AddOrderItemAsync 3" + "Path Name: " + path;
        //        byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

        //        await LogFtpProcessingError(ex, methodName, category);
        //        Thread.Sleep(3000);
        //        orderItemAddToDbandUploadToStorage++;
        //        if (orderItemAddToDbandUploadToStorage >= 3)
        //        {
        //            return uploadSuccessful = false;
        //        }
        //    }
        //    //}


        //    return uploadSuccessful;
        //}

        // #region Corrunpted file check methods 

        // //Function to verify the downloaded file
        // private async Task<bool> VerifyDownloadedFile(FtpCredentails destinationFtpCredentails, byte[] remoteFileBytes, string localFtpPath)
        // {
        //     try
        //     {
        //         FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
        //         using (var destinationClients = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
        //         {

        //             destinationClients.Config.EncryptionMode = FtpEncryptionMode.Auto;
        //             destinationClients.Config.ValidateAnyCertificate = true;
        //             await destinationClients.Connect();

        //             // Here compare file bytes 
        //             CancellationToken cancellationToken = CancellationToken.None;
        //             var path = localFtpPath;

        //             var destinationBytes = await destinationClients.DownloadBytes(path, cancellationToken);
        //             await destinationClients.Disconnect();
        //             return ByteArrayCompare(destinationBytes, remoteFileBytes);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.InnerException.ToString());
        //         return false;
        //     }
        // }

        // // Function to compare two byte arrays
        // private bool ByteArrayCompare(byte[] array1, byte[] array2)
        // {
        //     return StructuralComparisons.StructuralEqualityComparer.Equals(array1, array2);
        // }

        // #endregion Corrunpted file check methods

        //private async Task<bool> SaveDownloadFileAsync(ClientExternalOrderFTPSetup sourceFtp, Company company, string rootFolder, bool isMoveSingleFile, Response<ClientOrder> orderSaveResponse, bool uploadSuccessful, AsyncFtpClient destinationClient, string path, string uploadDirectory, string[] pathArray, string fullFilePathForFtp)
        //{
        //    ClientOrderItem clientOrderItem = await PrepareClientOrderItem(sourceFtp, company, orderSaveResponse, path, uploadDirectory, pathArray);

        //    SftpClient sourceClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
        //    sourceClient.OperationTimeout = TimeSpan.FromMinutes(50);
        //    sourceClient.Connect();

        //    if (isMoveSingleFile)
        //    {
        //        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            try
        //            {
        //                if (sourceClient.Exists(rootFolder + pathArray[1]))
        //                {
        //                    sourceClient.RenameFile(rootFolder + pathArray[1], rootFolder + "/_downloaded" + pathArray[1]);
        //                }
        //                //bool response = true;
        //                if (true)
        //                {
        //                    var addItemResponse = await AddOrderItem(clientOrderItem, company.Id, orderSaveResponse.Result.Id, InternalOrderItemStatus.OrderPlaced);

        //                    if (addItemResponse.Result <= 0)
        //                    {
        //                        sourceClient.RenameFile(rootFolder + "/_downloaded" + pathArray[1], rootFolder + pathArray[1]);
        //                        await destinationClient.DeleteFile(fullFilePathForFtp);
        //                        transactionScope.Dispose();
        //                    }
        //                    else
        //                    {
        //                        transactionScope.Complete();
        //                        uploadSuccessful = true;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                transactionScope.Dispose();
        //                sourceClient.RenameFile(rootFolder + "/_downloaded" + pathArray[1], rootFolder + pathArray[1]);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Add Item call {clientOrderItem.FileName} ");
        //        var addItemResponse = await AddOrderItem(clientOrderItem, company.Id, orderSaveResponse.Result.Id, InternalOrderItemStatus.OrderPlaced);

        //        uploadSuccessful = addItemResponse.IsSuccess;
        //    }

        //    return uploadSuccessful;
        //}

        //private async Task<ClientOrderItem> PrepareClientOrderItem(ClientExternalOrderFTPSetup sourceFtp, Company company, Response<ClientOrder> orderSaveResponse, string path, string uploadDirectory, string[] pathArray)
        //{
        //    SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
        //    sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
        //    sftpClient.Connect();

        //    // AddOrder Item
        //    ClientOrderItem clientOrderItem = new ClientOrderItem();


        //    // Arrival Time
        //    if (company.Id == AutomatedAppConstant.VcCompanyId)
        //    {
        //        DateTime arrivalTime = sftpClient.GetLastWriteTime(path);
        //        clientOrderItem.ArrivalTime = arrivalTime.AddHours(6);
        //    }

        //    clientOrderItem.FileName = Path.GetFileName(path);
        //    clientOrderItem.FileType = Path.GetExtension(path);



        //    SftpFileAttributes attributes = sftpClient.GetAttributes(path);


        //    if (attributes.IsRegularFile)
        //    {
        //        long fileSize = attributes.Size;
        //        clientOrderItem.FileSize = fileSize;
        //    }

        //    clientOrderItem.ClientOrderId = orderSaveResponse.Result.Id;
        //    clientOrderItem.CompanyId = company.Id;

        //    var replaceString = Path.GetDirectoryName(pathArray[1]).Replace($"\\", @"/");
        //    if (replaceString == "/") { replaceString = ""; }

        //    if (company.Id == 1182) // this is for mnm 
        //    {
        //        clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}/{replaceString}";
        //    }
        //    else
        //    {
        //        clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}{replaceString}";
        //    }

        //    var fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, pathArray[1]);
        //    var fullFilePathReplace = fullFilePath.Replace($"\\", @"/");
        //    fullFilePathReplace = fullFilePathReplace.Replace($"//", @"/");
        //    clientOrderItem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePathReplace);
        //    return clientOrderItem;
        //}


        //private async Task StorageFolderMoving(string orderDirectory, ClientExternalOrderFTPSetup sourceFtp, bool isMovingSingleFile = false)
        //{
        //    SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
        //    sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
        //    sftpClient.Connect();
        //    int i = 0;
        //    while (true)
        //    {
        //        try
        //        {
        //            var moveableFolder = "";


        //            if (!isMovingSingleFile)
        //            {
        //                moveableFolder = Path.GetDirectoryName(orderDirectory) + AutomatedAppConstant.FileMovingFolderNameForSix; // The new directory path to create
        //            }

        //            else
        //            {
        //                moveableFolder = orderDirectory + AutomatedAppConstant.FileMovingFolderNameForSix; // The new directory path to create
        //            }



        //            moveableFolder = moveableFolder.Replace("\\", "/");
        //            if (!sftpClient.Exists(moveableFolder))
        //            {
        //                //sourceClient.CreateDirectory(moveableFolder);
        //                await _sshNetService.RecursiveCreateDirectories(sftpClient, moveableFolder);
        //            }

        //            var temp = "";

        //            if (!isMovingSingleFile)
        //            {
        //                temp = Path.GetDirectoryName(orderDirectory) + AutomatedAppConstant.FileMovingFolderNameForSix + Path.GetFileName(orderDirectory);
        //            }

        //            else
        //            {
        //                temp = orderDirectory + AutomatedAppConstant.FileMovingFolderNameForSix + Path.GetFileName(orderDirectory);
        //            }

        //            temp = temp.Replace("\\", "/");

        //            if (!sftpClient.Exists(temp))
        //            {
        //                await _sshNetService.RecursiveCreateDirectories(sftpClient, moveableFolder);
        //            }

        //            await _sshNetService.RecursiveListFilesMove(sftpClient, orderDirectory, temp);


        //            //var sourceDiretoryFiles = sourceClient.ListDirectory(orderDirectory);


        //            if (!isMovingSingleFile)
        //            {
        //                try
        //                {
        //                    await _sshNetService.RecursiveDeleteDiretories(sftpClient, orderDirectory);
        //                }
        //                catch (Exception ex)
        //                {

        //                    throw;
        //                }
        //            }

        //            break;
        //            //}

        //        }
        //        catch (Exception ex)
        //        {

        //            i++;
        //            await Task.Delay(1000);
        //            if (i > 3)
        //            {
        //                break;
        //            }
        //        }
        //    }

        //}


        // #region Retouched Ai Batch Wise Processing 


        // #endregion

        // #endregion

    }
}
