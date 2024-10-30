using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.Models.FtpModels;
using FluentFTP;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.EmailMessage;
using DocumentFormat.OpenXml.Drawing.Charts;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.EmailSendAutomation
{
    public class AutoEmailSendService: IAutoEmailSendService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IActivityAppLogService _activityAppLogService;
        private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
        private readonly ICompanyManager _companyService;
        private readonly IFluentFtpService _fluentFtpService;
        private readonly IWorkflowEmailService _workflowEmailService;

        public AutoEmailSendService(
            IFileServerManager fileServerService,
            IClientOrderService orderService,
            IOrderStatusService orderStatusService,
            IActivityAppLogService activityAppLogService,
            ICompanyGeneralSettingManager companyGeneralSettingService,
            ICompanyManager companyService,
            IFluentFtpService fluentFtpService,
            IWorkflowEmailService workflowEmailService
            )
        {
            _fileServerService = fileServerService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _activityAppLogService = activityAppLogService;
            _companyGeneralSettingService = companyGeneralSettingService;
            _companyService = companyService;
            _fluentFtpService = fluentFtpService;
            _workflowEmailService = workflowEmailService;
        }

        public async Task<Response<bool>> NotifyOpsOnImageArrivalFTP()
        {
            try
            {
                var semaphore = new SemaphoreSlim(8);
                var tasks = new List<Task>();

                string query = $"SELECT cgs.* from  dbo.CompanyGeneralSettings cgs where cgs.AllowNotifyOpsOnImageArrivalFTP = 1";//need to chagne
                var companyGeneralSettings = await _companyGeneralSettingService.GetAllCompanyGeneralSettingsByQuery(query);
                if (companyGeneralSettings != null && !companyGeneralSettings.Any())
                {
                    return new Response<bool>();
                }

                foreach (var companyGeneralSetting in companyGeneralSettings)
                {
                    string clentFtpQuery = $"SELECT * FROM [dbo].[Client_ClientOrderFtp] as cc where cc.IsEnable = 1 and cc.ClientCompanyId= {companyGeneralSetting.CompanyId}";
                    CompanyModel company = await _companyService.GetById(companyGeneralSetting.CompanyId);
                    List<ClientExternalOrderFTPSetupModel> tempClientOrderFtps = await _fileServerService.GetAllClientFtpByQuery(clentFtpQuery);
                    
                    foreach (var tempClientOrderFtp in tempClientOrderFtps)
                    {
                        await semaphore.WaitAsync();
                        Console.WriteLine("UserName :" + tempClientOrderFtp.Username + "Folder Name:" + tempClientOrderFtp.InputRootFolder);

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                if (tempClientOrderFtp != null)
                                {
                                    FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();

                                    using (var sourceClient = new AsyncFtpClient(tempClientOrderFtp.Host,
                                       tempClientOrderFtp.Username, tempClientOrderFtp.Password, tempClientOrderFtp.Port ?? 0, ftpConfig))
                                    {
                                        sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                        sourceClient.Config.ValidateAnyCertificate = true;
                                        await sourceClient.Connect();

                                        // Specify the path to the D drive
                                        string dDrivePath = @"D:\";

                                        // Create a folder named 'txt' in the D drive
                                        string folderName = "txt";
                                        string folderPath = Path.Combine(dDrivePath, folderName);

                                        // Check if the folder exists, if not, create it
                                        if (!Directory.Exists(folderPath))
                                        {
                                            Directory.CreateDirectory(folderPath);
                                        }

                                        // Specify the file name and path
                                        string fileName = "sample.txt";
                                        string filePath = Path.Combine(folderPath, fileName);

                                        // Write a new line to the text file
                                        string newLine = "UserName :" + tempClientOrderFtp.Username + "Folder Name:" + tempClientOrderFtp.InputRootFolder;

                                        // Open the file in 'append' mode to add the new line
                                        using (StreamWriter writer = new StreamWriter(filePath, true))
                                        {
                                            writer.WriteLine(newLine);
                                        }

                                        // Process the image file


                                        int fileCount = 0;


                                        // Get the list of files and folders in the root folder
                                        FtpListItem[] ftpListItems = await sourceClient.GetListing(tempClientOrderFtp.InputRootFolder);



                                        foreach (FtpListItem item in ftpListItems)
                                        {
                                            if (item.Type == FtpObjectType.File)
                                            {
                                                fileCount++;
                                            }
                                            else if (item.Type == FtpObjectType.Directory)
                                            {
                                                if (item.Name == "_downloaded")
                                                {
                                                    continue;
                                                }
                                                await SendEmailToOpsToNotifyImageArrivalOnFtp(sourceClient, tempClientOrderFtp.InputRootFolder + "/" + item.Name, company, tempClientOrderFtp);
                                            }
                                        }

                                        if (fileCount > 0)
                                        {
                                            await SendEmailToOpsToNotifyImageArrivalOnFtp(sourceClient, tempClientOrderFtp.InputRootFolder, company, tempClientOrderFtp, true);
                                        }
                                        await sourceClient.Disconnect();
                                    }


                                }
                            }
                            catch (Exception ex)
                            {
                                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                                {
                                    CreatedByContactId = AutomatedAppConstant.ContactId,
                                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                                    PrimaryId = company.Id,
                                    ErrorMessage = $"CompanyId: {companyGeneralSetting.CompanyId}, CompanyCode: {company.Code}. {tempClientOrderFtp.GetInputLogDescription()}. Exception: {ex.Message}",
                                    MethodName = "NotifyOpsOnImageArrivalFTP->TempClientOrderFtps Loop",
                                    RazorPage = "AutoEmailSendService",
                                    Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP
                                };

                                await _activityAppLogService.InsertAppErrorActivityLog(activity);

                                //var loginUser = new LoginUserInfoViewModel
                                //{
                                //	ContactId = AutomatedAppConstant.ContactId
                                //};
                                //CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                                //{
                                //	//PrimaryId = (int)order.Id,
                                //	ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
                                //	loginUser = loginUser,
                                //	ErrorMessage = ex.InnerException.ToString(),
                                //	MethodName = $"NotifyOpsOnImageArrivalFTP,'  ', Username:{tempClientOrderFtp.Username}",
                                //	RazorPage = "FtpOrderProcessService",
                                //	Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
                                //};
                                //await _activityAppLogService.InsertAppErrorActivityLog(activity);

                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));

                    }

                }
                await Task.WhenAll(tasks);

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                    PrimaryId = 0,
                    ErrorMessage = $"Exception: {ex.Message}",
                    MethodName = "NotifyOpsOnImageArrivalFTP",
                    RazorPage = "AutoEmailSendService",
                    Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);

            }
            return new Response<bool>();
        }

        #region Private Method
        private async Task SendEmailToOpsToNotifyImageArrivalOnFtp(AsyncFtpClient sourceClient, string fileRootPath, CompanyModel company, ClientExternalOrderFTPSetupModel clientFtp, bool isFile = false)
        {
            try
            {
                //ToDo: Rakib need to add from db
                List<string> opsEmailList = new List<string>()
                            {
                                "rakibul@thekowcompany.com",
                                "anik@thekowcompany.com",
                                "zico@thekowcompany.com",
                                "raihan@thekowcompany.com",
                                "ops@thekowcompany.com",
                                "ak@thekowcompany.com",
                                "zakir@thekowcompany.com",
                                "mashfiq@thekowcompany.com"
                            };

                if (isFile)
                {
                    DateTime minModifiedTime = await GetMinModifiedTime(sourceClient, fileRootPath, company, clientFtp);
                    DateTime maxModifiedTime = await GetMaxModifiedTime(sourceClient, fileRootPath, company, clientFtp);
                    minModifiedTime = minModifiedTime.AddHours(6);
                    maxModifiedTime = maxModifiedTime.AddHours(6);

                    //If min image coming time less than or equal 15 then email send else return 
                    if (company.Id == 1181)
                    {
                        if (DateTime.Now.Subtract(minModifiedTime).TotalMinutes < 30)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Subtract(maxModifiedTime).TotalMinutes > 30)
                        {
                            return;
                        }
                    }

                    FTPOrderNotifyOpsOnImageArrivalFTP fTPOrderNotifyOpsOnImageArrivalFTP = new FTPOrderNotifyOpsOnImageArrivalFTP
                    {
                        EmailAddresses = opsEmailList,
                        MailType = "ArriveImageOnFtp",
                        //ImageCount = $"{imageCount}  ,  UserName:{clientFtp.Username}",
                        FolderName = $"{fileRootPath}",
                        ArrivalDateTime = $"{minModifiedTime}",
                        DeliveryTime = $"{minModifiedTime.AddHours(1.5)}",
                        //OrderType = $"NAN",
                        CompanyName = $"{company.Name}",
                        FtpUserName = $"{clientFtp.Username}"
                    };
                    await _workflowEmailService.SendEmailToOpsToNotifyImageArrivalOnFtp(fTPOrderNotifyOpsOnImageArrivalFTP);
                }
                else
                {
                    FtpListItem[] ftpListItems = await sourceClient.GetListing(fileRootPath, FtpListOption.Recursive);
                    if (ftpListItems.Length <= 0)
                    {
                        return;
                    }

                    var imageFiles = ftpListItems.Where(entry => entry.Type == FtpObjectType.File);

                    DateTime minArrivalTime = imageFiles.Min(file => file.Modified);
                    minArrivalTime = minArrivalTime.AddHours(6);

                    DateTime maxModifiedTime = imageFiles.Max(file => file.Modified);

                    maxModifiedTime = maxModifiedTime.AddHours(6);

                    //If min image coming time less than or equal 15 then email send else return 
                    if (DateTime.Now.Subtract(maxModifiedTime).TotalMinutes > 15)
                    {
                        return;
                    }

                    FTPOrderNotifyOpsOnImageArrivalFTP fTPOrderNotifyOpsOnImageArrivalFTP = new FTPOrderNotifyOpsOnImageArrivalFTP
                    {
                        EmailAddresses = opsEmailList,
                        MailType = "ArriveImageOnFtp",
                        //ImageCount = $"{imageFiles.Count()} ,  UserName:{clientFtp.Username} ",
                        FolderName = $"{fileRootPath}",
                        ArrivalDateTime = $"{minArrivalTime}",
                        DeliveryTime = $"{minArrivalTime.AddHours(1.5)}",
                        //OrderType = $"NAN",
                        CompanyName = $"{company.Name}",
                        FtpUserName = $"{clientFtp.Username}"
                    };

                    await _workflowEmailService.SendEmailToOpsToNotifyImageArrivalOnFtp(fTPOrderNotifyOpsOnImageArrivalFTP);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}. CompanyCode: {company.Code}. {clientFtp.GetInputLogDescription()}. FileRootPath: {fileRootPath}. Exception: {ex.Message}",
                    MethodName = "SendEmailToOpsToNotifyImageArrivalOnFtp",
                    RazorPage = "AutoEmailSendService",
                    Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            //await MoveToNotifiedFiles(sourceClient, tempClientOrderFtps.InputRootFolder, tempClientOrderFtps.InputRootFolder+"/"+AutomatedAppConstant.AfterSendImageComingNotifationFileMoveFolderName);
        }

        private async Task<DateTime> GetMinModifiedTime(AsyncFtpClient ftp, string directoryPath, CompanyModel company,  ClientExternalOrderFTPSetupModel clientFtp)
        {
            DateTime minArrivalTime = new DateTime();
            try
            {
                FtpListItem[] ftpListItems = await ftp.GetListing(directoryPath);

                if (ftpListItems.Length > 0)
                {
                    var imageFiles = ftpListItems.Where(entry => entry.Type == FtpObjectType.File && entry.Name != "Thumbs.db");
                    minArrivalTime = imageFiles.Min(file => file.Modified);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                {
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}. CompanyCode: {company.Code}. {clientFtp.GetInputLogDescription()}. DirectoryPath: {directoryPath}. Exception: {ex.Message}",
                    MethodName = "GetMinModifiedTime",
                    RazorPage = "AutoEmailSendService",
                    Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }

            return minArrivalTime;
        }

        private async Task<DateTime> GetMaxModifiedTime(AsyncFtpClient ftp, string directoryPath, CompanyModel company, ClientExternalOrderFTPSetupModel clientFtp)
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
                    ActivityLogFor = (int)ActivityLogForConstants.Company,
                    PrimaryId = company.Id,
                    ErrorMessage = $"CompanyId: {company.Id}. CompanyCode: {company.Code}. {clientFtp.GetInputLogDescription()}. DirectoryPath: {directoryPath}. Exception: {ex.Message}",
                    MethodName = "GetMaxModifiedTime",
                    RazorPage = "AutoEmailSendService",
                    Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
                };
                await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }
            return maxArrivalTime;
        }
        #endregion

    }
}
