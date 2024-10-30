using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using FluentFTP;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.StorageService;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.AutomationAppServices.ConvertOrderAttachmentFiles;
using CutOutWiz.Services.Models.FileUpload;

namespace CutOutWiz.Services.BLL.OrderAttachment
{
	public class OrderAttachmentBLLService:IOrderAttachmentBLLService
	{
		public readonly IFluentFtpService _fluentFtpService;
		public readonly IOrderFileAttachmentService _orderFileAttachmentService;
        public readonly IConvertOrderAttachmentFile _convertOrderAttachmentFile;
		public readonly IActivityAppLogService _activityAppLogService;

        public OrderAttachmentBLLService(IFluentFtpService fluentFtpService, 
			IOrderFileAttachmentService orderFileAttachmentService,
            IConvertOrderAttachmentFile convertOrderAttachmentFile,
            IActivityAppLogService activityAppLogService)
		{
			_fluentFtpService = fluentFtpService;
			_orderFileAttachmentService = orderFileAttachmentService;
			_convertOrderAttachmentFile = convertOrderAttachmentFile;
			_activityAppLogService = activityAppLogService;
        }

		public async Task AddOrderAttachment(List<string> attachmentSourchPath, ClientOrderModel order, CompanyModel company, FtpCredentailsModel sourceFtpCredential, FtpCredentailsModel destinationFtpCredentail, bool isLocalFile = false)
		{
			try
			{
				FileUploadModel fileUploadVM = new FileUploadModel();
				DateTimeConfiguration _dateTime = new DateTimeConfiguration();

				fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.Year}\\{_dateTime.Month}\\{_dateTime.Date}\\Raw\\{order.OrderNumber}\\OrderAttachment\\";

				FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();
				List<OrderFileAttachment> orderAttachments = new List<OrderFileAttachment>();
				
				using (var sourceClient = new AsyncFtpClient(sourceFtpCredential.Host, sourceFtpCredential.UserName, sourceFtpCredential.Password, sourceFtpCredential.Port ?? 0, ftpConfig))
				using (var destinationClient = new AsyncFtpClient(destinationFtpCredentail.Host, destinationFtpCredentail.UserName, destinationFtpCredentail.Password, destinationFtpCredentail.Port ?? 0, ftpConfig))
				{
					sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
					sourceClient.Config.ValidateAnyCertificate = true;
					await sourceClient.Connect();

					destinationClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
					destinationClient.Config.ValidateAnyCertificate = true;
					await destinationClient.Connect();
					
					foreach (var sourcePath in attachmentSourchPath)
					{
						try
						{
							string orderAttachmentUploadFilePath = fileUploadVM.UploadDirectory;

							string fileName = Path.GetFileName(sourcePath);

							if (!string.IsNullOrWhiteSpace(destinationFtpCredentail.SubFolder))
							{
								orderAttachmentUploadFilePath = $"{destinationFtpCredentail.SubFolder}/{fileUploadVM.UploadDirectory}{fileName}";

							}

							if (!await destinationClient.DirectoryExists(Path.GetDirectoryName(orderAttachmentUploadFilePath)))
							{
								await destinationClient.CreateDirectory(Path.GetDirectoryName(orderAttachmentUploadFilePath));
							}

							//TODO:CompanyId ..add this to check for KLM eml file to remove client address.
							//Need to add general settings.Hot fixing.
							if (company.Id == 1192 && orderAttachmentUploadFilePath.Contains(".eml"))
							{
								if (isLocalFile)
								{
									using (FileStream fileStream = System.IO.File.OpenRead(sourcePath))
									{
										MemoryStream memoryStream = new MemoryStream();
										fileStream.CopyTo(memoryStream);

										var convertedStream = await _convertOrderAttachmentFile.ConvertEMLFile(memoryStream);
										await destinationClient.UploadStream(convertedStream, orderAttachmentUploadFilePath);
									}
								}
								else
								{

									MemoryStream attachmentStream = new MemoryStream();
									await sourceClient.DownloadStream(attachmentStream, sourcePath);
									var convertedStream = await _convertOrderAttachmentFile.ConvertEMLFile(attachmentStream);

									await destinationClient.UploadStream(convertedStream, orderAttachmentUploadFilePath);


								}
							}
							else
							{
								bool isDownloaded = false;

								if (isLocalFile)
								{
									using (FileStream fileStream = System.IO.File.OpenRead(sourcePath))
									{
										await destinationClient.UploadStream(fileStream, orderAttachmentUploadFilePath);
										//System.IO.File.Copy(path,fullFilePathForFtp,true);
									}
								}
								else
								{
									using (var writeStream = await destinationClient.OpenWrite(orderAttachmentUploadFilePath))
									{
										isDownloaded = await sourceClient.DownloadStream(writeStream, sourcePath);

										if (!isDownloaded)
										{
											await sourceClient.DownloadStream(writeStream, sourcePath);
										}
									}
								}
							}

							var orderFileAttachment = new OrderFileAttachment
							{
								Order_ClientOrder_Id = order.Id,
								CompanyId = company.Id,
								FileName = fileName,
								PartialPath = fileUploadVM.UploadDirectory,
								Status = 1,//Todo:Rakib See aminul vai
								IsDeleted = false,
								CreatedByContactId = AutomatedAppConstant.ContactId,
								CreateDated = System.DateTime.Now,
								ObjectId = Guid.NewGuid().ToString(),
								FileSize = (byte)await sourceClient.GetFileSize(sourcePath)
							};

							orderAttachments.Add(orderFileAttachment);

						}
						catch (Exception ex)
						{
							CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
							{
                                CompanyObjectId = company.ObjectId,
                                ActivityLogFor = (int)ActivityLogForConstants.Order,
                                PrimaryId = (int)order.Id,
                                CreatedByContactId = AutomatedAppConstant.ContactId,
                                //loginUser = (int)AutomatedAppConstant.ContactId,
                                ErrorMessage = $"CompanyId: {company.Id}. {sourceFtpCredential.GetLogDescription()} Exception: {ex.Message}",
                                MethodName = "AddOrderAttachment",
								RazorPage = "OrderAttachmentBLLService",
								Category = (int)ActivityLogCategory.OrderUploadError,
							};

							await _activityAppLogService.InsertAppErrorActivityLog(activity);
						}
					}

				}
				if (orderAttachments != null && orderAttachments.Any())
				{
					await _orderFileAttachmentService.Insert(orderAttachments, (int)order.Id);
				}
			}
			catch (Exception ex)
			{
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    CompanyObjectId = company.ObjectId,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    PrimaryId = (int)order.Id,
                    CreatedByContactId = AutomatedAppConstant.ContactId,
                    //loginUser = (int)AutomatedAppConstant.ContactId,
                    ErrorMessage = $"CompanyId: {company.Id}. {sourceFtpCredential.GetLogDescription()} Exception: {ex.Message}",
                    MethodName = "AddOrderAttachment",
                    RazorPage = "OrderAttachmentBLLService",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
                //Console.WriteLine(ex.Message.ToString());
            }
		}

		public bool IsTxtOrPdfFile(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase) ||
				   string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase)||
					string.Equals(extension, ".log", StringComparison.OrdinalIgnoreCase)||
                    string.Equals(extension, ".docx", StringComparison.OrdinalIgnoreCase)||
                     string.Equals(extension, ".eml", StringComparison.OrdinalIgnoreCase)||
                      string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase)||
                      string.Equals(extension, ".tmp", StringComparison.OrdinalIgnoreCase)
                   ;
		}
	}
}
