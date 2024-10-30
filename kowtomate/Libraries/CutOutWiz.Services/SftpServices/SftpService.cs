using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.AutomationAppServices.OrderItemCategorySetByAutomation;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.BLL.OrderAttachment;
using CutOutWiz.Services.BLL.OrderStatusAndOrderItemStatus;
using CutOutWiz.Services.BLL.StatusChangeLog;
using CutOutWiz.Services.ClientCommonCategoryService.ClientCategorys;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.EmailMessage;
using CutOutWiz.Services.ErrorLogServices;
using CutOutWiz.Services.PathReplacementServices;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.StorageService;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO.Compression;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.SftpServices
{
    public class SftpService : ISftpService
	{
		private readonly ISshNetService _sshNetService;
		private readonly IConfiguration _configuration;
		private readonly IClientOrderService _clientOrderService;
		private readonly IClientOrderItemService _clientOrderItemService;
		private readonly IErrorLogService _errorLogService;
		private readonly IOrderStatusService _orderStatusService;
		private readonly IActivityAppLogService _activityAppLogService;
		private readonly ICompanyTeamManager _companyTeamService;
		private readonly IStatusChangeLogBLLService _statusChangeLogBLLService;
		private readonly IOrderAttachmentBLLService _orderAttachmentBLLService;
		private readonly IFluentFtpService _fluentFtpService;
		private readonly IFtpFilePathService _ftpFilePathService;
		private readonly IPathReplacementService _pathReplacementService;
		private readonly ICategorySetService _categorySetService;
		private readonly IClientCategoryService _clientCategoryService;
		private readonly ICompanyGeneralSettingManager _companyGeneralSettingService;
		private readonly IWorkflowEmailService _workflowEmailService;

        public SftpService(ISshNetService sshNetService, IConfiguration configuration,
			IClientOrderService clientOrderService, IClientOrderItemService clientOrderItemService,
			IErrorLogService errorLogService, IOrderStatusService orderStatusService,
			IActivityAppLogService activityAppLogService,
			ICompanyTeamManager companyTeamService, IStatusChangeLogBLLService statusChangeLogBLLService,
			IOrderAttachmentBLLService orderAttachmentBLLService, IFluentFtpService fluentFtpService,
			IFtpFilePathService ftpFilePathService, IPathReplacementService pathReplacementService,
			ICategorySetService categorySetService, IClientCategoryService clientCategoryService,
			ICompanyGeneralSettingManager companyGeneralSettingService,
            IWorkflowEmailService workflowEmailService)
		{
			_sshNetService = sshNetService;
			_configuration = configuration;
			_clientOrderService = clientOrderService;
			_clientOrderItemService = clientOrderItemService;
			_errorLogService = errorLogService;
			_orderStatusService = orderStatusService;
			_activityAppLogService = activityAppLogService;
			_companyTeamService = companyTeamService;
			_statusChangeLogBLLService = statusChangeLogBLLService;
			_orderAttachmentBLLService = orderAttachmentBLLService;
			_fluentFtpService = fluentFtpService;
			_ftpFilePathService = ftpFilePathService;
			_pathReplacementService = pathReplacementService;
			_categorySetService = categorySetService;
			_clientCategoryService = clientCategoryService;
			_companyGeneralSettingService = companyGeneralSettingService;
            _workflowEmailService = workflowEmailService;


        }
		/// <summary>
		/// SFTP Client Connect
		/// </summary>
		/// <param name="sourceFtp"></param>
		/// <returns></returns>
		public async Task<SftpClient> InitializeSftpClient(ClientExternalOrderFTPSetupModel sourceFtp)
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
		public async Task<(List<string> Files, List<(string FilePath, DateTime LastWriteTime, long FileSize)> ZipFiles)> GetFilesOrZipFiles(SftpClient sftpClient,
			ClientExternalOrderFTPSetupModel sourceFtp, CompanyGeneralSettingModel companyGeneralSetting)
		{
			var files = new List<string>();
			var zipFiles = new List<(string FilePath, DateTime LastWriteTime, long FileSize)>();
			var listing = await GetSftpFileList(sourceFtp, companyGeneralSetting);

			if (companyGeneralSetting.CheckUploadCompletedFlagOnFile)
			{
				// Check get listed hot key.
				bool isExistsHotkey = listing.Any(x => x.Name.Contains(companyGeneralSetting.HotKeyFileName));
				if (isExistsHotkey)
				{
					files = listing
						.Where(entry => entry.IsRegularFile && !entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
						.Select(x => x.FullName)
						.ToList();

					zipFiles = listing
						.Where(entry => entry.IsRegularFile && entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
						.Select(entry => (FilePath: entry.FullName, entry.LastWriteTime, FileSize: entry.Length))
						.ToList();
				}
			}
			else if (companyGeneralSetting.CheckUploadCompletedFlagOnBatchName && !string.IsNullOrEmpty(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
			{
				if (listing.Any(x => x.Name.Contains(companyGeneralSetting.CompletedFlagKeyNameOnBatch)))
				{
					files = listing
						.Where(entry => entry.IsRegularFile && !entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && entry.Name.Contains(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
						.Select(x => x.FullName)
						.ToList();

					zipFiles = listing
						.Where(entry => entry.IsRegularFile && entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && entry.Name.Contains(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
						.Select(entry => (FilePath: entry.FullName, entry.LastWriteTime, FileSize: entry.Length))
						.ToList();
				}
			}
			else
			{
				var lastTime = DateTime.Now.AddMinutes(-Convert.ToInt64(companyGeneralSetting.FtpIdleTime));
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
		public async Task<List<SftpFile>> GetDirectoriesToProcess(SftpClient sftpClient, ClientExternalOrderFTPSetupModel sourceSFtp, CompanyModel company,
			CompanyGeneralSettingModel companyGeneralSetting)
		{
			return await GetSftpDirectories(sourceSFtp, companyGeneralSetting);
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
		public async Task OrderProcessWithDirectories(List<SftpFile> directories, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company,
			CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel destinationFtp, FileServerModel fileServer)
		{
			var semaphoreSlim = new SemaphoreSlim(int.Parse(_configuration.GetSection("GeneralSettings")["OrderPlaceSFtpBatchThread"]));
			var orderProcessTasks = new List<Task>();

			foreach (var directory in directories)
			{
				await semaphoreSlim.WaitAsync();
				orderProcessTasks.Add(ProvideSingleDirectoryForOrderProcess(directory, sourceFtp, company, companyGeneralSetting, destinationFtp, fileServer, semaphoreSlim));
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
		public async Task OrderProcessWithRegularFiles(SftpClient sftpClient, List<string> files, ClientExternalOrderFTPSetupModel sourceSftp, CompanyModel company,
			FtpCredentailsModel destinationFtp, FileServerModel fileServer, CompanyGeneralSettingModel companyGeneralSetting)
		{
			if (files.Count > 0)
			{
				files = await FilterExistingFiles(company, files, sourceSftp);

				// Create chunk for order create.
				var chunks = await GetChunksOfPaths(files, companyGeneralSetting.AllowMaxNumOfItemsPerOrder);
				var semaphore = new SemaphoreSlim(10);
				var singleFileTasks = new List<Task>();

				foreach (var chunk in chunks)
				{
					await semaphore.WaitAsync();
					singleFileTasks.Add(SourceSftpToDestinationFtpFileCopyAndOrderProcess(destinationFtp, files, company, fileServer, sourceSftp, sourceSftp.InputRootFolder, companyGeneralSetting));
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
		public async Task OrderProcessWithZipFiles(SftpClient sftpClient, List<(string FilePath, DateTime LastWriteTime, long FileSize)> zipFiles,
			ClientExternalOrderFTPSetupModel sourceSftp, FtpCredentailsModel destinationFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FileServerModel fileServer)
		{
			foreach (var zipFile in zipFiles)
			{
				if (await _clientOrderService.CheckExistenceOfBatchBySourceFullPath(zipFile.FilePath))
				{
					continue;
				}

				string tempStorePath = await CreateTemporaryFolderForDownload();

				try
				{
					await DownloadAndExtractZipFile(sftpClient, zipFile, tempStorePath);
					var pathList = Directory.GetFiles(tempStorePath + "ExtractPath/", "*", SearchOption.AllDirectories);

					if (pathList.Any())
					{
						var response = await CopyFilesFromLocalToFTP(sourceSftp, destinationFtp, pathList.ToList(), company, fileServer, sourceSftp.InputRootFolder, companyGeneralSetting, zipFile, false, true);

						if (response.IsSuccess)
						{
							await _sshNetService.SingleFileMove(sourceSftp, zipFile.FilePath);
						}

						await CleanupTemporaryStore(tempStorePath);
					}
				}
				catch (Exception ex)
				{
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
		public async Task<string> CreateTemporaryFolderForDownload()
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
		public async Task DownloadAndExtractZipFile(SftpClient sftpClient, (string FilePath, DateTime LastWriteTime, long FileSize) zipFile, string tempStorePath)
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
		public async Task CleanupTemporaryStore(string tempStorePath)
		{
			if (Directory.Exists(tempStorePath))
			{
				Directory.Delete(tempStorePath, true);
			}
		}
		/// <summary>
		/// Check file exist or not
		/// </summary>
		/// <param name="company"></param>
		/// <param name="files"></param>
		/// <param name="sourceFtp"></param>
		/// <returns></returns>
		public async Task<List<string>> FilterExistingFiles(CompanyModel company, List<string> files, ClientExternalOrderFTPSetupModel sourceSftp)
		{
			var filteredFiles = new List<string>();
			foreach (var filePath in files)
			{
				var withoutRootPath = await RemoveInputRootFolder(sourceSftp.InputRootFolder, filePath);
				var exists = await _clientOrderItemService.CheckClientOrderItemFile(company.Id, withoutRootPath, Path.GetFileName(filePath), DateTime.Now.Date.ToString());
				if (!exists)
				{
					filteredFiles.Add(filePath);
				}
			}
			return filteredFiles;
		}
		/// <summary>
		/// Get files from Sftp
		/// </summary>
		/// <param name="sourceFtp"></param>
		/// <param name="remoteDirectory"></param>
		/// <param name="companyGeneralSetting"></param>
		/// <param name="getLastDayFiles"></param>
		/// <returns></returns>
		public async Task<List<SftpFile>> GetSftpFileList(ClientExternalOrderFTPSetupModel sourceSftp,
			CompanyGeneralSettingModel companyGeneralSetting, int? getLastDayFiles = 0)
		{
			SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceSftp);
			sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
			sftpClient.Connect();

			// If getLastDayFiles is null or 0, default to the last 24 hours
			int daysToSubtract = getLastDayFiles ?? 1; // Default to 1 day if null
			if (daysToSubtract == 0)
			{
				daysToSubtract = 1; // Default to 1 day if 0 is provided
			}

			// Convert days to hours
			int hoursToSubtract = daysToSubtract * 24;

			var lastModifiedTime = DateTime.Now.AddHours(-hoursToSubtract);

			var directories = sftpClient.ListDirectory(sourceSftp.InputRootFolder);

			var directoryList = directories.Where(d => d.LastWriteTime > lastModifiedTime).ToList();

			sftpClient.Disconnect();

			return directoryList;
		}
		/// <summary>
		/// Get directories from sftp.
		/// </summary>
		/// <param name="sourceFtp"></param>
		/// <param name="companyGeneralSetting"></param>
		/// <param name="getLastDayFiles"></param>
		/// <returns></returns>
		public async Task<List<SftpFile>> GetSftpDirectories(ClientExternalOrderFTPSetupModel sourceFtp,
			CompanyGeneralSettingModel companyGeneralSetting, int? getLastDayFiles = null)
		{
			SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
			sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
			sftpClient.Connect();

			// If getLastDayFiles is null or 0, default to the last 24 hours
			int daysToSubtract = getLastDayFiles ?? 1; // Default to 1 day if null
			if (daysToSubtract == 0)
			{
				daysToSubtract = 1; // Default to 1 day if 0 is provided
			}

			// Convert days to hours
			int hoursToSubtract = daysToSubtract * 24;

			var lastModifiedTime = DateTime.Now.AddHours(-hoursToSubtract);

			var directories = sftpClient.ListDirectory(sourceFtp.InputRootFolder);

			// here remove ftp file move path.
			if (!string.IsNullOrWhiteSpace(companyGeneralSetting.FtpFileMovedPathAfterOrderCreated))
			{
				directories = directories.Where(x => x.Name != companyGeneralSetting.FtpFileMovedPathAfterOrderCreated).ToList();
			}

			 directories = directories
				.Where(item => !string.IsNullOrEmpty(item.Name) && item.IsDirectory && item.Name != "." && item.Name != ".." && item.Name != companyGeneralSetting.FtpFileMovedPathAfterOrderCreated)
				.ToList();

			var directoryList = directories.Where(d => d.IsDirectory && d.LastWriteTime > lastModifiedTime).ToList();

			sftpClient.Disconnect();

			return directoryList;

		}
		/// <summary>
		/// Remove ftp inputRootPath from path.
		/// </summary>
		/// <param name="RootFolderPath"></param>
		/// <param name="FullPath"></param>
		/// <returns></returns>
		public async Task<string> RemoveInputRootFolder(string RootFolderPath, string FullPath)
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

		public async Task ProvideSingleDirectoryForOrderProcess(SftpFile item, ClientExternalOrderFTPSetupModel sourceSftp, CompanyModel company,
			CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel destinationFtp, FileServerModel fileServer, SemaphoreSlim semaphoreSlim)
		{
			SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceSftp);
			sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
			sftpClient.Connect();

			await Task.Run(async () =>
			{
				try
				{
					Response<string> copyResponse = new Response<string>();
					var allFilePath = new List<string>();

					if (!string.IsNullOrEmpty(item.Name) && item.IsDirectory && item.Name != "." && item.Name != ".." && item.Name != companyGeneralSetting.FtpFileMovedPathAfterOrderCreated)
					{
						var orderDirectory = $"{sourceSftp.InputRootFolder}/{item.Name}";

						// Input directory get all file paths.
						await _sshNetService.RecursiveListFiles(sftpClient, orderDirectory, allFilePath);

						if (companyGeneralSetting.CheckUploadCompletedFlagOnFile)
						{

							if (allFilePath.Exists(fp => fp.ToLower().Contains(companyGeneralSetting.HotKeyFileName.ToLower())))
							{
								allFilePath.RemoveAll(fp => fp.ToLower().Contains(companyGeneralSetting.HotKeyFileName.ToLower()));

								copyResponse = await SourceSftpToDestinationFtpFileCopyAndOrderProcess(destinationFtp, allFilePath, company, fileServer, sourceSftp, orderDirectory, companyGeneralSetting);
							}
						}
						if (companyGeneralSetting.IsFtpIdleTimeChecking)
						{
							// Filter ftp idle time when get all file path.
							copyResponse = await SourceSftpToDestinationFtpFileCopyAndOrderProcess(destinationFtp, allFilePath, company, fileServer, sourceSftp, orderDirectory, companyGeneralSetting);
						}

						if (companyGeneralSetting.CheckUploadCompletedFlagOnBatchName && !string.IsNullOrEmpty(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
						{

							if (item.Name.Contains(companyGeneralSetting.CompletedFlagKeyNameOnBatch))
							{
								// var copyResponse = await CopyFilesFromOneFtpToAnotherFTP(sourceFtpCredential, kowToMateFtpCredentails, allFilePath, company, fileServer, orderDirectory, companyGeneralSetting);
								copyResponse = await SourceSftpToDestinationFtpFileCopyAndOrderProcess(destinationFtp, allFilePath, company, fileServer, sourceSftp, orderDirectory, companyGeneralSetting);
							}
						}

						if (copyResponse.IsSuccess)
						{
							await _workflowEmailService.SendEmailToOpsToNotifyOrderUpload(item.Name, copyResponse.Result, company, orderDirectory, sourceSftp.Username, allFilePath.Count());
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = "ProvideStorageFolderForOrderProcess";
					byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;
					await _errorLogService.LogFtpProcessingError(ex, methodName, errorCategory);
				}
				finally
				{
					semaphoreSlim.Release();
				}
			});

		}
		public async Task<Response<string>> SourceSftpToDestinationFtpFileCopyAndOrderProcess(FtpCredentailsModel destinationFtp, List<string> allFilePath, CompanyModel company,
			FileServerModel fileServer, ClientExternalOrderFTPSetupModel sourceSftp, string orderBatchDirectoryPath, CompanyGeneralSettingModel? companyGeneralSetting = null)
		{
			var response = new Response<string>();

			try
			{

				SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceSftp);
				sftpClient.OperationTimeout = TimeSpan.FromMinutes(4);
				sftpClient.Connect();

				var orderSaveResponse = new Response<ClientOrderModel>();

				// Remove Thumbs File or bat files
				if (allFilePath != null && allFilePath.Any())
				{
					allFilePath.RemoveAll(path => string.Equals(Path.GetExtension(path), ".db", StringComparison.OrdinalIgnoreCase));
                }

				// here check the batchpath already exist or not 
				var checkBatchIsExists = await _clientOrderService.CheckBatchNameExistOnClientOrder(company.Id, orderBatchDirectoryPath);

				if (checkBatchIsExists != null && checkBatchIsExists.Result.Id > 0 && checkBatchIsExists.Result.InternalOrderStatus == (int)InternalOrderStatus.OrderPlacing)
				{
					// here remove path which file already exists this order.
					foreach (var filePath in allFilePath)
					{
						var withoutRootPath = await RemoveInputRootFolder(sourceSftp.InputRootFolder, filePath);
						// Query need update : Where clause add new field for order id.
						var existingFilePath = await _clientOrderItemService.CheckClientOrderItemFile(company.Id, withoutRootPath, Path.GetFileName(filePath), DateTime.Now.Date.ToString());
						if (existingFilePath)
						{
							allFilePath.Remove(filePath);
						}
					}
					orderSaveResponse = checkBatchIsExists;
				}
				else
				{
					//Create Order 
					if (allFilePath != null && allFilePath.Any())
					{
						orderSaveResponse = await AddOrderInfo(company, fileServer, sourceSftp.Id, sourceSftp.InputRootFolder);
					}
				}

				if (!orderSaveResponse.IsSuccess)
				{
					response.Message = orderSaveResponse.Message;
					return response;
				}

				if (companyGeneralSetting.AllowExtraFile)
				{
					ClientOrderModel clientOrder = new ClientOrderModel
					{
						Id = orderSaveResponse.Result.Id,
						AllowExtraOutputFileUpload = true,
					};

					var isAllowed = await _clientOrderService.UpdateOrderAllowExtraOutputFileUploadField(clientOrder);
				}
				var successUploadFileCount = 0;
				try
				{
					// Image Path wise thread
					string orderPlaceSFtpFilesThread = _configuration.GetSection("GeneralSettings")["OrderPlaceSFtpFilesThread"];
					SemaphoreSlim semaphoreSlim = new SemaphoreSlim(int.Parse(orderPlaceSFtpFilesThread));
					var uploadTasks = new List<Task<Response<int>>>();

					// Chunk Create for files upload.
					var chunkSize = await CalculateChunkSize(allFilePath.Count);
					var filesChunks = await GetFilesChunksWithPaths(allFilePath, chunkSize);
					try
					{
						foreach (var chunk in filesChunks)
						{
							await semaphoreSlim.WaitAsync();
							uploadTasks.Add(FileUploadFromSftpToFtpForOrderItemProcess(chunk, sourceSftp, destinationFtp,
								company, orderSaveResponse, semaphoreSlim, companyGeneralSetting));

						}

						var FileUploadResponses = await Task.WhenAll(uploadTasks);

						for (int i = 0; i < FileUploadResponses.Length; i++)
						{
							successUploadFileCount = FileUploadResponses[i].Result + successUploadFileCount;
						}
					}
					catch (Exception ex)
					{
						// Handle or log the exception
						response.Message = ex.Message;
					}
					finally
					{
						semaphoreSlim.Release();
					}

					if (successUploadFileCount == allFilePath.Count)
					{
						await PerformOrderUpdates(orderSaveResponse);
						response.IsSuccess = true;
						response.Result = orderSaveResponse.Result.OrderNumber;
					}

					//Mail Send to ops after order place notification.
					if (successUploadFileCount == allFilePath.Count && companyGeneralSetting != null && companyGeneralSetting.CompanyId != 1181)
					{
						await _workflowEmailService.SendEmailToOpsToNotifyOrderUpload(sourceSftp.InputRootFolder, orderSaveResponse.Result.OrderNumber, company, sourceSftp.InputRootFolder, sourceSftp.Username, allFilePath.Count());
					}

				}
				catch (Exception ex)
				{
					string methodName = "CopyFilesFromOneFtpToAnotherSFTP";
					byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

					await _errorLogService.LogFtpProcessingError(ex, methodName, category);

					response.IsSuccess = false;
					response.Message = ex.Message;
				}

				return response;


			}
			catch (Exception ex)
			{
				string methodName = "CopyFilesAndNotify";
				byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

				await _errorLogService.LogFtpProcessingError(ex, methodName, category);
				response.IsSuccess = false;
			}

			return response;
		}
		public async Task<Response<ClientOrderModel>> AddOrderInfo(CompanyModel company, FileServerModel fileServer, long sourceFtpId, string orderDirectory = "")
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

				var addResponse = await _clientOrderService.Insert(order);

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

			return response;
		}
		public async Task<List<List<string>>> GetChunksOfPaths(List<string> allFilePath, int chunkSize)
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
		public async Task<Response<string>> CopyFilesFromLocalToFTP(ClientExternalOrderFTPSetupModel sourceFtpCredential,
		   FtpCredentailsModel destinationFtpCredentails, List<string> allFilesFromNewOrder, CompanyModel company,
		   FileServerModel fileServer, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, (string filePath, DateTime LastWriteTime, long FileSize) fileInfo, bool isMoveSingleFile = false, bool isLocalFile = false)
		{
			var response = new Response<string>();
			var orderSaveResponse = new Response<ClientOrderModel>();
			List<string> orderDirectoryPaths = new List<string>();
			//Bath name send to ops mail . 
			string batchName = "";
			FtpCredentailsModel sourceFtpCredentials = await CreateExternalOrderFTPSetupCredentials(sourceFtpCredential);
			try
			{

				allFilesFromNewOrder.RemoveAll(path => Path.GetExtension(path) == ".db");
				//Create Order 

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

						var isAllowed = await _clientOrderService.UpdateOrderAllowExtraOutputFileUploadField(clientOrder);
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
						//foreach (var path in allFilesFromNewOrder)
						//{

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


											var addItemResponse = await AddOrderItem(clientOrderItem, company.Id, path, orderSaveResponse.Result.Id, InternalOrderItemStatus.OrderPlaced);

											uploadSuccessful = addItemResponse.IsSuccess;


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
															Thread.Sleep(1000);
															fileDownload++;
															if (fileDownload > 3)
															{
																string methodName = $"File Transfer Error {ex.Message.ToString()}";
																byte errorCategory = (byte)ActivityLogCategory.FtpOrderPlaceApp;
																await _errorLogService.LogGeneralError(ex, methodName, errorCategory);

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

											}
											else
											{
												await _clientOrderItemService.Delete(addItemResponse.Result.ToString());
											}
											// sourceClient.Disconnect();
											await destinationClient.Disconnect();
											// Here compare bytes source to destination.
											if (!isLocalFile)
											{
												var compareBytes = await createBytesAndCompareFromPaths(sourceFtpCredentials, path, destinationFtpCredentails, fullFilePath);
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
						//}
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
					MethodName = "CopyFilesFromOneFtpToAnotherFTP",
					RazorPage = "FtpOrderProcessService",
					Category = (int)ActivityLogCategory.FtpOrderPlaceApp,
				};
				await _activityAppLogService.InsertAppErrorActivityLog(activity);
			}

			//Delete Order Place if order does not contain any images 
			var newOrder = await _clientOrderService.GetByOrderNumber(orderSaveResponse.Result.OrderNumber);
			if (newOrder.NumberOfImage == 0)
			{
				await _clientOrderService.Delete(newOrder.ObjectId);

				response.IsSuccess = false;
				response.Result = "";
				return response;
			}
			await _workflowEmailService.SendEmailToOpsToNotifyOrderUpload(batchName, orderSaveResponse.Result.OrderNumber, company);

			response.IsSuccess = true;
			response.Result = orderSaveResponse.Result.OrderNumber;

			return response;
		}
		public async Task<FtpCredentailsModel> CreateExternalOrderFTPSetupCredentials(ClientExternalOrderFTPSetupModel ftp)
		{
			return new FtpCredentailsModel
			{
				Id = ftp.Id,
				Host = ftp.Host,
				UserName = ftp.Username,
				Password = ftp.Password,
				RootFolder = ftp.InputRootFolder,
				Port = ftp.Port
			};
		}
		public async Task<int> CalculateChunkSize(int totalFiles)
		{
			return totalFiles < 200 ? 3 : 2; // Adjust the values as needed
		}
		public async Task<List<List<string>>> GetFilesChunksWithPaths(List<string> allFiles, int chunkSize)
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
		public async Task<Response<int>> FileUploadFromSftpToFtpForOrderItemProcess(List<string> chunk, ClientExternalOrderFTPSetupModel sourceSftp,
		   FtpCredentailsModel destinationFtpCredentails, CompanyModel company,
			Response<ClientOrderModel> orderSaveResponse, SemaphoreSlim semaphoreSlim, CompanyGeneralSettingModel? companyGeneralSetting = null)
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

							using (var destinationClient = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
							{
								await destinationClient.AutoConnect();
								foreach (var path in chunk)
								{
									//File download on this method
									var FileUploadResponse = await FileUploadFromSFTPToFtpAndInsertOrderItem(sourceSftp, destinationFtpCredentails, company, orderSaveResponse, destinationClient, path);

									// Here check company general settings Order placed after file mode or not.
									if (FileUploadResponse.IsSuccess && companyGeneralSetting.OrderPlaceBatchMoveType == (int)OrderPlaceBatchMoveType.FileandFolderMoveAfterOrderPlace)
									{
										response.Result = response.Result + 1;
										await _sshNetService.SingleFileMove(sourceSftp, path);
									}
									response.IsSuccess = FileUploadResponse.IsSuccess;

								}
							}
							break;
						}
						catch (Exception ex)
						{
							string methodName = "ProcessChunkAsync 1";
							byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

							await _errorLogService.LogFtpProcessingError(ex, methodName, category);



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
					string methodName = "ProcessChunkAsync 1";
					byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

					await _errorLogService.LogFtpProcessingError(ex, methodName, category);
				}
				finally
				{
					semaphoreSlim.Release();
				}

			}
			);
			return response;
		}
		public async Task<Response<bool>> FileUploadFromSFTPToFtpAndInsertOrderItem(ClientExternalOrderFTPSetupModel sourceSftp, FtpCredentailsModel destinationFtpCredentails,
			CompanyModel company, Response<ClientOrderModel> orderSaveResponse, AsyncFtpClient destinationClient, string path)
		{
			var orderItemAddToDbandUploadToStorage = 0;
			var response = new Response<bool>();

			try
			{
				var readStartTime = DateTime.Now;

				var companyGeneralSetting = await _companyGeneralSettingService.GetGeneralSettingByCompanyId(company.Id);

				// Prepare the destination path
				var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, orderSaveResponse.Result.CreatedDate, orderSaveResponse.Result.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);
				var pathArray = path.Split(sourceSftp.InputRootFolder);

				if (companyGeneralSetting.IsBatchRootFolderNameAddWithOrder)
				{
					pathArray[1] = Path.GetFileName(sourceSftp.InputRootFolder) + "/" + pathArray[1];// Path array one means order place folder structure from source sftp
				}

				var fullFilePathForFtp = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, pathArray[1]);

				if (!string.IsNullOrWhiteSpace(destinationFtpCredentails.SubFolder))
				{
					fullFilePathForFtp = $"{destinationFtpCredentails.SubFolder}/{fullFilePathForFtp}";
				}

				Console.WriteLine($"Upload to Ftp : {Path.GetFileName(path)}");
				// Insert order item or file
				var orderItemInsertResponse = await InsertOrderItem(sourceSftp, company, orderSaveResponse, path, uploadDirectory, pathArray);
				response.IsSuccess = orderItemInsertResponse.IsSuccess;

				SftpClient sftpClient = await _sshNetService.CreateSshNetConnector(true, sourceSftp);
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
								fileDownload++;

								if (fileDownload >= maxDownloadRetries)
								{
									string methodName = $"File Transfer Error On SFtp Order Place {ex.Message}";
									byte errorCategory = (byte)ActivityLogCategory.SFtpOrderPlaceApp;
									response.IsSuccess = false;
									await _errorLogService.LogGeneralError(ex, methodName, errorCategory);

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

						if (companyGeneralSetting.CompanyId != 1188) //company code
						{
							var fileBytesArray = sftpClient.ReadAllBytes(path);

							var result = await VerifyDownloadedFile(destinationFtpCredentails, fileBytesArray, fullFilePathForFtp);

							if (!result)
							{
								//If verification fails, increment the retry counter
								retryOrderItemUpload++;
								response.IsSuccess = false;
								response.IsSuccess = false;
							}
							else
							{
								//If verification is successful, break out of the retry loop
								response.IsSuccess = true;
								response.IsSuccess = true;
								break;
							}
						}
						else
						{
							break;
						}

						#endregion
					}
					catch (Exception ex)
					{
						// If any other exception occurs, wait for a moment before retrying
						Thread.Sleep(3000);
						retryOrderItemUpload++;

						if (retryOrderItemUpload >= maxRetries)
						{
							string methodName = "AddOrderItemAsync 1";
							byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

							await _errorLogService.LogFtpProcessingError(ex, methodName, category);
						}
					}
				}

			}
			catch (Exception ex)
			{
				string methodName = "AddOrderItemAsync 3" + "Path Name: " + path;
				byte category = (byte)ActivityLogCategory.FtpOrderPlaceApp;

				await _errorLogService.LogFtpProcessingError(ex, methodName, category);
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
		public async Task<Response<bool>> InsertOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, Response<ClientOrderModel> orderSaveResponse, string path, string uploadDirectory, string[] pathArray)
		{
			var response = new Response<bool>();
			ClientOrderItemModel clientOrderItem = await PrepareClientOrderItem(sourceFtp, company, orderSaveResponse, path, uploadDirectory, pathArray);

			SftpClient sourceClient = await _sshNetService.CreateSshNetConnector(true, sourceFtp);
			sourceClient.OperationTimeout = TimeSpan.FromMinutes(50);
			sourceClient.Connect();

			Console.WriteLine($"Add Item call {clientOrderItem.FileName} ");
			var addItemResponse = await AddOrderItem(clientOrderItem, company.Id, path, orderSaveResponse.Result.Id, InternalOrderItemStatus.OrderPlaced);

			response.IsSuccess = addItemResponse.IsSuccess;

			return response;
		}
		public async Task<ClientOrderItemModel> PrepareClientOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, Response<ClientOrderModel> orderSaveResponse, string path, string uploadDirectory, string[] pathArray)
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

			var replaceString = Path.GetDirectoryName(pathArray[1]).Replace($"\\", @"/");
			if (replaceString == "/") { replaceString = ""; }

			if (company.Id == 1182) // this is for mnm 
			{
				clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}/{replaceString}";
			}
			else
			{
				clientOrderItem.PartialPath = @"/" + $"{orderSaveResponse.Result.OrderNumber}{replaceString}";
			}

			var fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, pathArray[1]);
			var fullFilePathReplace = fullFilePath.Replace($"\\", @"/");
			fullFilePathReplace = fullFilePathReplace.Replace($"//", @"/");
			clientOrderItem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePathReplace);
			return clientOrderItem;
		}
		public async Task<Response<long>> AddOrderItem(ClientOrderItemModel clientOrderItem, int companyId, string clientStorageFilePath, long orderId, InternalOrderItemStatus status = 0)
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
		public async Task PerformOrderUpdates(Response<ClientOrderModel> orderSaveResponse)
		{
			await _orderStatusService.UpdateOrderStatus(orderSaveResponse.Result, AutomatedAppConstant.ContactId);
			await _orderStatusService.UpdateOrderArrivalTime(orderSaveResponse.Result);

		}
		public async Task<bool> createBytesAndCompareFromPaths(FtpCredentailsModel sourceFtpCredential, string sourcePath, FtpCredentailsModel destinationFtpCredentails, string destinationPath)
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
					// Handle the exception if needed
					retryCount++;
				}
				retryCount++;
			}

			return result;
		}
		/// <summary>
		/// Compare File Size Local to remote.
		/// </summary>
		/// <param name="destinationFtpCredentails"></param>
		/// <param name="remoteFileBytes"></param>
		/// <param name="destinationFilePath"></param>
		/// <returns></returns>
		public async Task<bool> VerifyDownloadedFile(FtpCredentailsModel destinationFtpCredentails, byte[] remoteFileBytes, string destinationFilePath)
		{
			try
			{
				FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
				using (var destinationClients = new AsyncFtpClient(destinationFtpCredentails.Host, destinationFtpCredentails.UserName, destinationFtpCredentails.Password, destinationFtpCredentails.Port ?? 0, ftpConfig))
				{

					destinationClients.Config.EncryptionMode = FtpEncryptionMode.Auto;
					destinationClients.Config.ValidateAnyCertificate = true;
					await destinationClients.Connect();

					// Here compare file bytes 
					CancellationToken cancellationToken = CancellationToken.None;
					var path = destinationFilePath;

					var destinationBytes = await destinationClients.DownloadBytes(path, cancellationToken);
					await destinationClients.Disconnect();
					var checkByteEqual = await CompareBytes(destinationBytes, remoteFileBytes);
					return checkByteEqual;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException.ToString());
				return false;
			}
		}
		/// <summary>
		/// Compare localFile to RemoteFile Bytes.
		/// </summary>
		/// <param name="localFileBytesArray"></param>
		/// <param name="remoteFileBytesArray"></param>
		/// <returns></returns>
		///
		public async Task<bool> CompareBytes(byte[] localFileBytesArray, byte[] remoteFileBytesArray)
		{
			await Task.Yield();

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
		public async Task<bool> CompareByteArraysAsync(byte[] sourceBytesArray, byte[] destinationBytesArray)
		{
			// Check if the arrays have different lengths, and return false if they do.
			if (sourceBytesArray.Length != destinationBytesArray.Length)
			{
				return false;
			}

			// Use Task.Run to perform the byte-by-byte comparison in a separate thread
			// while not blocking the calling thread.
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

		public async Task PrepareOrderItemCategoryInformation(ClientOrderItemModel clientOrderItem, int categoryId)
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
		public async Task OrderItemCategoryLogAdd(ClientOrderItemModel clientOrderItem)
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

		public async Task<Response<bool>> StartProcessForOrderPlacing(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FtpCredentailsModel destinationFtp, FileServerModel fileServer)
		{
			var response = new Response<bool>();
			using (SftpClient sftpClient = await InitializeSftpClient(sourceFtp))
			{
				// Get files or zipfiles from user sftp.
				var (files, zipFiles) = await GetFilesOrZipFiles(sftpClient, sourceFtp, companyGeneralSetting);
				// Get directories from user sftp.
				var getDirectoriesForOrderProcess = await GetDirectoriesToProcess(sftpClient, sourceFtp, company, companyGeneralSetting);

				if (files.Any())
				{
					await OrderProcessWithRegularFiles(sftpClient, files, sourceFtp, company, destinationFtp, fileServer, companyGeneralSetting);
				}

				if (getDirectoriesForOrderProcess.Any())
				{
					await OrderProcessWithDirectories(getDirectoriesForOrderProcess, sourceFtp, company, companyGeneralSetting, destinationFtp, fileServer);
				}

				if (zipFiles.Any())
				{
					await OrderProcessWithZipFiles(sftpClient, zipFiles, sourceFtp, destinationFtp, company, companyGeneralSetting, fileServer);
				}
				response.IsSuccess = true;
			}
			return response;
		}
	}
}
