using CutOutWiz.Core;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Core.Message;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.SOP;
using FastReport.Utils;
using FluentFTP;
using KowToMateAdmin.Helper;
using KowToMateAdmin.Models;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using Mailjet.Client.Resources;
using Mailjet.Client.Resources.SMS;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Net;
using System.Timers;
using Uno.Extensions;
using static CutOutWiz.Core.Utilities.Enums;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using CutOutWiz.Services.Models.Message;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.Models.ClientOrders;

namespace KowToMateAdmin.Pages.OrderUpload
{
    public partial class NewOrder
    {
        #region Properties
        protected ModalNotification ModalNotification { get; set; }
        [Parameter]
        public string objectId { get; set; }
        bool isSubmitting;
        private ClientOrderModel order = new ClientOrderModel();
        private LoginUserInfoViewModel loginUser = null;
        private List<SOPTemplateModel> templateService = new List<SOPTemplateModel>();
        public List<int> selectedTemplateList = new List<int>();
        public List<int> newSelectedSOPTemplateList = new List<int>();
        // SOP Attachment and Instruction
        bool isInsertUpdateTemplatePopupVisible = false;
        bool isSOPNameChange = false;
        private List<SOPStandardServiceModel> sopService = new List<SOPStandardServiceModel>();
        private List<int> selectedSeviceIdList = new List<int>();
        public List<int> SOPStandardServiceList = new List<int>();
        public List<ClientOrderFtpModel> ClientOrderFtps = new List<ClientOrderFtpModel>();
        bool isSOPTemplateView = false;
        //Progress Bar
        private double maxValue;
        private double CurrentValue;
        bool isProgressBar = false;
        // SOP Id
        public int sopTemplateId = 0;
        public SOPTemplateModel sopTemplate = new SOPTemplateModel();
        //Order Object Id
        public string orderobjectid = "";
        // For Hide Control
        private bool AllowFolderUpload = true;
        int radioValue = 0;
        bool spinShow = false;
        bool isCreatingSop = false;
        bool uploadCancel = false;
        bool uploadItemCancel = false;
        private bool isUploadInputDisabled = false;
        #region new Fields
        private CompanyModel company = new CompanyModel();
        private FileServerModel fileServer = new FileServerModel();
        #endregion
        DateTimeConfiguration _dateTime = new DateTimeConfiguration();
        CompanyTeamModel companyTeam = null;
        private int CurrentValueForSopAttachmentProgressbar = 0;
        DateTime showingTime = new DateTime();
        TimerService timer = new TimerService();
        #endregion

        #region Inilizlized page
        /// <summary>
        /// Initialized
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            loginUser = _workContext.LoginUserInfo;
            ClientOrderFtps = await _clientOrderFtpService.GetClientOrderFtpsListByCompanyId(loginUser.CompanyId);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                company = await _companyService.GetById(loginUser.CompanyId);
                companyTeam = await _companyTeamService.GetTeamByCompanyId(loginUser.CompanyId);
                //await LoadOrder();
                await LoadSOPTemplates();
                //Set file Server
                if (company.FileServerId > 0)
                {
                    fileServer = await _fileServerService.GetById(company.FileServerId);
                }
                else
                {
                    var fileServers = await _fileServerService.GetAll();

                    if (fileServers != null && fileServers.Count() > 0)
                    {
                        var defaultTemplate = fileServers.FirstOrDefault(f => f.IsDefault == true);
                    }
                }

                SOPStandardServiceList = selectedSeviceIdList;
                sopService = await _sopStandardService.GetAll();
                
                //await js.InvokeVoidAsync("attachFileUploadHandler");
                //await js.InvokeVoidAsync("browserReload.initEditor");
                //await js.InvokeVoidAsync("attachFileUploadHandler");

                StateHasChanged();
            }

            await js.InvokeVoidAsync("browserReload.initEditor");
            
        }

        protected override void OnAfterRender(bool firstRender)
        {
            js.InvokeVoidAsync("attachFileUploadHandler");
        }
        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    //await js.InvokeVoidAsync("browserReload.initEditor");
        //}
        //protected override async Task OnAfterRender(bool firstRender)
        //{

        //}
        #endregion

        #region Upload Order Files only
        //Upload file
        private async void LoadOrderFile(InputFileChangeEventArgs args)
        {
            timer.StartTimer();
            showingTime = System.DateTime.Now;
            isProgressBar = true;
            isSubmitting = true;
            isCreatingSop = true;
            isUploadInputDisabled = true;
            
            if (isProgressBar)
            {
                CurrentValue = 0.1;
            }

            maxValue = args.GetMultipleFiles(maximumFileCount: 3000).Count;
            var count = 0;
            
            try
            {
                // Add Order
                await AddOrderInfo();

                FileServerViewModel fileServerViewModel = new FileServerViewModel();
                fileServerViewModel.Host = fileServer.Host;
                fileServerViewModel.UserName = fileServer.UserName;
                fileServerViewModel.Password = fileServer.Password;
                fileServerViewModel.SubFolder = fileServer.SubFolder;


                await _dateTime.DateTimeConvert(DateTime.Now);
                //fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\";
                var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, order.CreatedDate, order.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);


                List<ClientOrderItemModel> DuplicateClientOrderItem = new List<ClientOrderItemModel>();
                var tempClientOrderItems = new List<ClientOrderItemModel>();
                using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                {
					ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
					ftp.Config.ValidateAnyCertificate = true;
					await ftp.AutoConnect();

                    ClientOrderItemModel clientOrderITem = new ClientOrderItemModel();
                    var CheckingClientOrderItem = new ClientOrderItemModel();

                    foreach (var file in args.GetMultipleFiles(maximumFileCount: 3000))
                    {
                        var ftpFullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, file.Name);

                        clientOrderITem = new ClientOrderItemModel();
                        clientOrderITem.CompanyId = order.CompanyId;
                        clientOrderITem.FileName = file.Name;
                        clientOrderITem.FileType = file.ContentType;
                        clientOrderITem.FileSize = file.Size;
                        clientOrderITem.ClientOrderId = order.Id;

                        // Thumb File Uplaod Prevent
                        if (Path.GetExtension(clientOrderITem.FileName) == ".db")
                        {
                            count++;
                            CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
							await InvokeAsync(StateHasChanged);
							continue;
                        }

                        CheckingClientOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileName(clientOrderITem);

                        //No duplicate found
                        if (CheckingClientOrderItem == null)
                        {
                            try
                            {
                                //var fileStam = model.file.OpenReadStream(maxAllowedSize: model.file.Size * 1024);

                                if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                                {
									await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), $"{fileServer.SubFolder}/{ftpFullFilePath}", FtpRemoteExists.Overwrite, true);
								}
                                else
                                {
                                    await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), ftpFullFilePath, FtpRemoteExists.Overwrite, true);
                                }
                                //clientOrderITem.ObjectId = Guid.NewGuid().ToString();

                            }
                            catch (Exception ex)
                            {
                                string mdd = ex.Message;
                            }

                            clientOrderITem.InternalFileInputPath = ftpFullFilePath;
                            clientOrderITem.PartialPath = $"/{order.OrderNumber}";

                            //Cancel Order
                            if (uploadCancel)
                            {
                                //TODO: Delete all items from ftp and database for this order.
                                await _orderService.Delete(order.ObjectId);
                                //UriHelper.NavigateTo("/order/upload", true);
                                break;
                            }

                            await AddOrderItem(clientOrderITem);
                        }
                        else
                        {
                            CheckingClientOrderItem.File = file;
                            DuplicateClientOrderItem.Add(CheckingClientOrderItem);
                        }

                        tempClientOrderItems.Add(clientOrderITem);

                        if (uploadItemCancel)
                        {
                            var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                            CurrentValue = 0;
                            uploadItemCancel = false;
                            spinShow = false;
							await InvokeAsync(StateHasChanged);
							break;
                        }

                        count++;
                        CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
						await InvokeAsync(StateHasChanged);
					}

                    await ftp.Disconnect();
                }



                if (DuplicateClientOrderItem.Count > 0)
                {
                    var text = await CreateTextFileName(DuplicateClientOrderItem);
                    //await js.DisplayMessage($"The File have already this Order {order.OrderNumber}\n{text}");

                    if (await js.ReplaceConfirmation($"The destination has {DuplicateClientOrderItem.Count} files with the same names. Do you want to replace the files ?", $"\n{text}", SweetAlertTypeMessagee.question))
                    {
                        //clientOrderITem = new ClientOrderItem();
                        maxValue = DuplicateClientOrderItem.Count();
                        count = 0;
                        tempClientOrderItems = new List<ClientOrderItemModel>();
                        isProgressBar = true;
                        if (isProgressBar)
                        {
                            CurrentValue = 0.1;
                        }

                        foreach (var duplicateOrderItem in DuplicateClientOrderItem)
                        {
                            //var ftpFullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, duplicateOrderItem.File.Name);
                            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                            {
								ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
								ftp.Config.ValidateAnyCertificate = true;
								await ftp.AutoConnect();

                                if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                                {
									await ftp.UploadStream(duplicateOrderItem.File.OpenReadStream(maxAllowedSize: 107374182400), $"{fileServer.SubFolder}/{duplicateOrderItem.InternalFileInputPath}" , FtpRemoteExists.Overwrite, true);
								}
                                else
                                {
                                    await ftp.UploadStream(duplicateOrderItem.File.OpenReadStream(maxAllowedSize: 107374182400), duplicateOrderItem.InternalFileInputPath, FtpRemoteExists.Overwrite, true);
                                }
                                await ftp.Disconnect();
                            }

                            duplicateOrderItem.FileType = duplicateOrderItem.File.ContentType;
                            duplicateOrderItem.FileSize = (byte)duplicateOrderItem.File.Size;
                            duplicateOrderItem.FileName = duplicateOrderItem.File.Name;
                            await _clientOrderItemService.UpdateItemFile(duplicateOrderItem);
                            tempClientOrderItems.Add(duplicateOrderItem);

                            if (uploadItemCancel)
                            {
                                var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                                CurrentValue = 0;
                                uploadItemCancel = false;
                                spinShow = false;
								await InvokeAsync(StateHasChanged);
								break;
                            }

                            count++;
                            CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
							await InvokeAsync(StateHasChanged);
						}

                    }
                }

                if (CurrentValue == 100)
                {
                    CurrentValue = 0;
                    isProgressBar = false;
					await InvokeAsync(StateHasChanged);
				}
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "LoadOrderFile",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

            isSubmitting = false;
            isCreatingSop = false;
            isUploadInputDisabled = false;
            timer.StopTimer();
			await InvokeAsync(StateHasChanged);
		}
        #endregion End of Upload Order Files only

        #region Upload Order Folders and Files
        private async Task LoadOrderFolder1(InputFileChangeEventArgs e)
        {
            maxValue = e.GetMultipleFiles(maximumFileCount: 100000).Count;
            var loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000).ToList();

            if (loadedFiless == null || loadedFiless.Count <= 0)
            {
                await js.DisplayMessage("Folder is Empty");

                return;
            }

            isProgressBar = true;
            isSubmitting = true;
            isCreatingSop = true;
            isUploadInputDisabled = true;
            var CheckingClientOrderItem = new ClientOrderItemModel();
            List<ClientOrderItemModel> DuplicateClientOrderItem = new List<ClientOrderItemModel>();
            
            if (isProgressBar)
            {
                CurrentValue = 0.1;
            }

            var count = 0;

            try
            {

                // Add Order Base Path
                order.BatchPath = _selectedFileFromJs[0].Path.Split("/")[0];


				// Add Order as OrderPlacing 
				await AddOrderInfo();

                //Populate file server
                FileServerViewModel fileServerViewModel = PopulateFielServerViewModel();

                var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, order.CreatedDate, order.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);

                List<ClientOrderItemModel> tempClientOrderItems = new List<ClientOrderItemModel>();

                int totalNoOfFiles = loadedFiless.Count();

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


                var filesChunks = GetFilesChunksWithPaths(loadedFiless, chunkSize);

                //ClientOrderItem clientOrderITem = new ClientOrderItem();
                //var processingTasks = new List<Task>();
                //var semaphore = new SemaphoreSlim(10);

                foreach (var fileChunk in filesChunks)
                {
                    if (uploadItemCancel)
                    {
                        break;
                    }

                    //await semaphore.WaitAsync();

                    //processingTasks.Add(Task.Run(async () =>
                    //{
                        using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                        {
                            //var i = loadedFiless.IndexOf(file);
                           
                            ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                            ftp.Config.ValidateAnyCertificate = true;
							await ftp.AutoConnect();

							//int i = 0;
							foreach (var fileWithPath in fileChunk)
                            {
                                var completefilePath = fileWithPath.filePath;

                                // var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel);
                                var file = fileWithPath.Item2;

                                var clientOrderITem = new ClientOrderItemModel();

                                var fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, completefilePath);

                                // Thumb File Uplaod Prevent
                                if (Path.GetExtension(clientOrderITem.FileName) == ".db")
                                {
                                    count++;
                                    CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

                                   StateHasChanged();
                                    continue;
                                }

                                var filepath = Path.GetDirectoryName(completefilePath);

                                if (!string.IsNullOrEmpty(filepath))
                                {
                                    var replaceString = filepath.Replace($"\\", @"/");
                                    clientOrderITem.PartialPath = @"/" + $"{order.OrderNumber}/{replaceString}" /*+ @"/"*/;
                                }

                                if (!string.IsNullOrEmpty(clientOrderITem.FileName) && !string.IsNullOrEmpty(clientOrderITem.PartialPath))
                                {
                                    CheckingClientOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPath(clientOrderITem);
                                }

                                if (CheckingClientOrderItem == null || CheckingClientOrderItem.Id <= 0)
                                {
                                    try
                                    {
                                        FtpStatus ftpStatus = FtpStatus.Failed;
                                        if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                                        {

                                            ftpStatus = await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: file.Size * 1024), $"{fileServer.SubFolder}/{fullFilePath}", FtpRemoteExists.Overwrite, true);

                                        }
                                        else
                                        {
                                            ftpStatus = await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: file.Size * 1024), fullFilePath, FtpRemoteExists.Overwrite, true);
                                        }

                                        if (ftpStatus.Equals(FtpStatus.Success))
                                        {
                                            count++;
                                            CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

                                            //Add File To Db
                                            clientOrderITem.ObjectId = Guid.NewGuid().ToString();
                                            clientOrderITem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePath);
                                            clientOrderITem.FileName = file.Name;
                                            clientOrderITem.FileType = file.ContentType;
                                            clientOrderITem.FileSize = (byte)file.Size;

                                            clientOrderITem.ClientOrderId = order.Id;
                                            clientOrderITem.CompanyId = company.Id;
                                            var response = await AddOrderItem(clientOrderITem);


                                            clientOrderITem.Id = response.Result;
                                            await InsertClientUploadActivityLog(clientOrderITem, completefilePath);
                                            tempClientOrderItems.Add(clientOrderITem);
                                           StateHasChanged();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string mdd = ex.InnerException.ToString();
                                        await js.DisplayMessage($"Something wrong ! Please Try Again and Error Message : {mdd}");
                                        //StateHasChanged();

                                        //break;
                                    }
                                    /*+_selectedFileFromJs[i].Path*/
                                }

                                if (uploadItemCancel)
                                {
                                    //semaphore.Release();
                                    break;
                                }

                                if (CheckingClientOrderItem != null && CheckingClientOrderItem.Id > 0)
                                {
                                    CheckingClientOrderItem.File = file;
                                    DuplicateClientOrderItem.Add(CheckingClientOrderItem);
                                }
                            } //End of single chunk loop

                            await ftp.Disconnect();
                        }  //End of FTP

                        //semaphore.Release();

                    //})); //End of task 
                } //End of chunk loop

                //await Task.WhenAll(processingTasks);

                if (uploadItemCancel)
                {
                    var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                    CurrentValue = 0;
                    uploadItemCancel = false;
                    spinShow = false;

                    isSubmitting = false;
                    isCreatingSop = false;
                    isUploadInputDisabled = false;
                    timer.StopTimer();
                   StateHasChanged();
                    return;
                }

                if (DuplicateClientOrderItem.Count > 0)
                {
                    var text = await CreateTextFileName(DuplicateClientOrderItem);

                    if (await js.ReplaceConfirmation($"The destination has {DuplicateClientOrderItem.Count} files with the same names. Do you want to replace the files?", $" \n{text}", SweetAlertTypeMessagee.question))
                    {
                        maxValue = DuplicateClientOrderItem.Count();
                        count = 0;
                        tempClientOrderItems = new List<ClientOrderItemModel>();
                        isProgressBar = true;
                       StateHasChanged();

                        if (isProgressBar)
                        {
                            CurrentValue = 0.1;
                        }

                        foreach (var clientOrderITem in DuplicateClientOrderItem)
                        {
                            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                            {
                                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                                ftp.Config.ValidateAnyCertificate = true;
                                await ftp.AutoConnect();
                                if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                                {
                                    await ftp.UploadStream(clientOrderITem.File.OpenReadStream(maxAllowedSize: 107374182400), $"{fileServer.SubFolder}/{clientOrderITem.InternalFileInputPath}", FtpRemoteExists.Overwrite, true);
                                }
                                else
                                {
                                    await ftp.UploadStream(clientOrderITem.File.OpenReadStream(maxAllowedSize: 107374182400), clientOrderITem.InternalFileInputPath, FtpRemoteExists.Overwrite, true);
                                }
                                await ftp.Disconnect();
                            }

                            clientOrderITem.FileType = clientOrderITem.File.ContentType;
                            clientOrderITem.FileSize = (byte)clientOrderITem.File.Size;
                            clientOrderITem.FileName = clientOrderITem.File.Name;

                            await _clientOrderItemService.UpdateItemFile(clientOrderITem);
                            tempClientOrderItems.Add(clientOrderITem);

                            if (uploadItemCancel)
                            {
                                var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
                                CurrentValue = 0;
                                uploadItemCancel = false;
                                spinShow = false;
                                await InvokeAsync(StateHasChanged);
                                break;
                            }

                            count++;
                            CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
                            await InvokeAsync(StateHasChanged);

                        }

                    }
                }

                if (CurrentValue == 100)
                {
                    CurrentValue = 0;
                    isProgressBar = false;
                   StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "LoadOrderFolder",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

            isSubmitting = false;
            isCreatingSop = false;
            isUploadInputDisabled = false;
            timer.StopTimer();
           StateHasChanged();
        }
		private async Task LoadOrderFolder(InputFileChangeEventArgs e)
		{
			maxValue = e.GetMultipleFiles(maximumFileCount: 100000).Count;
			var loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000).ToList();

			if (loadedFiless == null || loadedFiless.Count <= 0)
			{
				await js.DisplayMessage("Folder is Empty");

				return;
			}

			isProgressBar = true;
			isSubmitting = true;
			isCreatingSop = true;
			isUploadInputDisabled = true;
			var CheckingClientOrderItem = new ClientOrderItemModel();
			List<ClientOrderItemModel> DuplicateClientOrderItem = new List<ClientOrderItemModel>();

			if (isProgressBar)
			{
				CurrentValue = 0.1;
			}

			var count = 0;

			try
			{
				// Add Order Base Path
				//order.BatchPath = _selectedFileFromJs[0].Path.Split("/")[0];

				// Add Order as OrderPlacing 
				await AddOrderInfo();

				//Populate file server
				FileServerViewModel fileServerViewModel = PopulateFielServerViewModel();

				var uploadDirectory = _ftpFilePathService.GetFtpRootFolderPathUptoOrderNumber(company.Code, order.CreatedDate, order.OrderNumber, FileStatusWiseLocationOnFtpConstants.Raw);

				List<ClientOrderItemModel> tempClientOrderItems = new List<ClientOrderItemModel>();


				using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
				{

					ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
					ftp.Config.ValidateAnyCertificate = true;
					await ftp.AutoConnect();

					for (int i = 0; i < loadedFiless.Count; i++)
					{
						var completefilePath = _selectedFileFromJs[i].Path;

						var file = loadedFiless[i];

						var clientOrderITem = new ClientOrderItemModel();

						var fullFilePath = _ftpFilePathService.GetFtpFullFilePath(uploadDirectory, completefilePath);

						// Thumb File Uplaod Prevent
						if (Path.GetExtension(clientOrderITem.FileName) == ".db")
						{
							count++;
							CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

							StateHasChanged();
							continue;
						}

						var filepath = Path.GetDirectoryName(completefilePath);

						if (!string.IsNullOrEmpty(filepath))
						{
							var replaceString = filepath.Replace($"\\", @"/");
							clientOrderITem.PartialPath = @"/" + $"{order.OrderNumber}/{replaceString}" /*+ @"/"*/;
						}

						if (!string.IsNullOrEmpty(clientOrderITem.FileName) && !string.IsNullOrEmpty(clientOrderITem.PartialPath))
						{
							CheckingClientOrderItem = await _clientOrderItemService.GetByFileByOrderIdAndFileNameAndPath(clientOrderITem);
						}

						if (CheckingClientOrderItem == null || CheckingClientOrderItem.Id <= 0)
						{
							try
							{
								FtpStatus ftpStatus = FtpStatus.Failed;
								if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
								{

									ftpStatus = await ftp.UploadStream(file.OpenReadStream( maxAllowedSize:  file.Size * 1024), $"{fileServer.SubFolder}/{fullFilePath}", FtpRemoteExists.Overwrite, true);

								}
								else
								{
									ftpStatus = await ftp.UploadStream(file.OpenReadStream(maxAllowedSize:  file.Size * 1024), fullFilePath, FtpRemoteExists.Overwrite, true);
								}

								if (ftpStatus.Equals(FtpStatus.Success))
								{
									count++;
									CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);

									//Add File To Db
									clientOrderITem.ObjectId = Guid.NewGuid().ToString();
									clientOrderITem.InternalFileInputPath = _ftpFilePathService.GetFtpFileDisplayInUIPath(fullFilePath);
									clientOrderITem.FileName = file.Name;
									clientOrderITem.FileType = file.ContentType;
									clientOrderITem.FileSize = (byte)file.Size;

									clientOrderITem.ClientOrderId = order.Id;
									clientOrderITem.CompanyId = company.Id;
									var response = await AddOrderItem(clientOrderITem);


									clientOrderITem.Id = response.Result;
									await InsertClientUploadActivityLog(clientOrderITem, completefilePath);
									tempClientOrderItems.Add(clientOrderITem);
									StateHasChanged();

								}
							}
							catch (Exception ex)
							{
								string mdd = ex.InnerException.ToString();
								await js.DisplayMessage($"Something wrong ! Please Try Again and Error Message : {mdd}");
								//StateHasChanged();

								//break;
							}
						}

						if (uploadItemCancel)
						{
							break;
						}

						if (CheckingClientOrderItem != null && CheckingClientOrderItem.Id > 0)
						{
							CheckingClientOrderItem.File = file;
							DuplicateClientOrderItem.Add(CheckingClientOrderItem);
						}
					}

					await ftp.Disconnect();
				}  //End of FTP


				if (uploadItemCancel)
				{
					var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
					CurrentValue = 0;
					uploadItemCancel = false;
					spinShow = false;

					isSubmitting = false;
					isCreatingSop = false;
					isUploadInputDisabled = false;
					timer.StopTimer();
					StateHasChanged();
					return;
				}

				if (DuplicateClientOrderItem.Count > 0)
				{
					var text = await CreateTextFileName(DuplicateClientOrderItem);

					if (await js.ReplaceConfirmation($"The destination has {DuplicateClientOrderItem.Count} files with the same names. Do you want to replace the files?", $" \n{text}", SweetAlertTypeMessagee.question))
					{
						maxValue = DuplicateClientOrderItem.Count();
						count = 0;
						tempClientOrderItems = new List<ClientOrderItemModel>();
						isProgressBar = true;
						StateHasChanged();

						if (isProgressBar)
						{
							CurrentValue = 0.1;
						}

						foreach (var clientOrderITem in DuplicateClientOrderItem)
						{
							using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
							{
								ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
								ftp.Config.ValidateAnyCertificate = true;
								await ftp.AutoConnect();
								if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
								{
									await ftp.UploadStream(clientOrderITem.File.OpenReadStream(maxAllowedSize: 107374182400), $"{fileServer.SubFolder}/{clientOrderITem.InternalFileInputPath}", FtpRemoteExists.Overwrite, true);
								}
								else
								{
									await ftp.UploadStream(clientOrderITem.File.OpenReadStream(maxAllowedSize: 107374182400), clientOrderITem.InternalFileInputPath, FtpRemoteExists.Overwrite, true);
								}
								await ftp.Disconnect();
							}

							clientOrderITem.FileType = clientOrderITem.File.ContentType;
							clientOrderITem.FileSize = (byte)clientOrderITem.File.Size;
							clientOrderITem.FileName = clientOrderITem.File.Name;

							await _clientOrderItemService.UpdateItemFile(clientOrderITem);
							tempClientOrderItems.Add(clientOrderITem);

							if (uploadItemCancel)
							{
								var result = await _clientOrderItemService.DeleteList(tempClientOrderItems, fileServerViewModel, order);
								CurrentValue = 0;
								uploadItemCancel = false;
								spinShow = false;
								await InvokeAsync(StateHasChanged);
								break;
							}

							count++;
							CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
							await InvokeAsync(StateHasChanged);

						}

					}
				}

				if (CurrentValue == 100)
				{
					CurrentValue = 0;
					isProgressBar = false;
					StateHasChanged();
				}
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)order.Id,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "LoadOrderFolder",
					RazorPage = "NewOrder.razor.cs",
					Category = (int)ActivityLogCategory.OrderUploadError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");
			}

			isSubmitting = false;
			isCreatingSop = false;
			isUploadInputDisabled = false;
			timer.StopTimer();
			StateHasChanged();
		}

		private List<List<(string filePath, IBrowserFile)>> GetFilesChunksWithPaths(List<IBrowserFile> allFiles, int chunkSize)
        {
            var chunks = new List<List<(string filePath, IBrowserFile)>>();

            int count = 0;
            var chunk = new List<(string filePath, IBrowserFile)>();
            int i = 0;
            foreach (var file in allFiles)
            {
                var path = _selectedFileFromJs[i].Path;
                count++;
                chunk.Add((path, file));

                if (count == chunkSize)
                {
                    chunks.Add(chunk);
                    chunk = new List<(string filePath, IBrowserFile)>();
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

        //private List<List<IBrowserFile>> GetChunksOfPaths(List<IBrowserFile> allFilePath, int chunkSize)
        //{
        //    var chunks = new List<List<IBrowserFile>>();

        //    int count = 0;
        //    var chunk = new List<IBrowserFile>();

        //    foreach (var tempPath in allFilePath)
        //    {
        //        count++;
        //        chunk.Add(tempPath);
        //        if (count == chunkSize)
        //        {
        //            chunks.Add(chunk);
        //            chunk = new List<IBrowserFile>();
        //            count = 0;
        //        }
        //    }
        //    if (count > 0)
        //    {
        //        chunks.Add(chunk);
        //    }

        //    return chunks;
        //}

        #endregion End of Upload Order Folder and Files

        #region Insert or Update Order to Database 
        /// <summary>
        /// Insert Or Update Order
        /// </summary>
        /// <returns></returns>
        private async Task InsertUpdateOrder()
        {
            spinShow = true;
            isSubmitting = true;
            isCreatingSop = true;
            order.SOPTemplateList = await GetSOPTemplate();
            try
            {
                if (order.Id > 0)
                {
                    // SOP Template INformation
                    SOPTemplateModel SOPmodel = new SOPTemplateModel();
                    SOPmodel.Instruction = order.Instructions;
                    SOPmodel.CompanyId = loginUser.CompanyId;
                    SOPmodel.CreatedByContactId = loginUser.ContactId;
                    SOPmodel.ObjectId = Guid.NewGuid().ToString();
                    SOPmodel.SopTemplateServiceList = await GetSopServices();
                    SOPmodel.FileServerId = company.FileServerId; //Todo:Zakir need to check
                                                                  // New Order Status Change
                    SOPmodel.Status = (int)SopStatus.New;
                    // New Order Status Change
                    SOPmodel.selectedTemplateList = new List<int>();
                    DateTimeConfiguration currentDate = new DateTimeConfiguration();
                    SOPmodel.Name = @"Default - " + currentDate.currenDateTime;
                    var sopResult = new SOPTemplateModel();
                    List<int> orderSOPStandardServiceListIds = new List<int>();
                    //---Order Upload Time SOP Create-------
                    if (order.SOPTemplateList == null || !order.SOPTemplateList.Any())
                    {
                        if (order.Instructions == null)
                        {
                            await js.DisplayMessage("Select a SOP or Write instruction.");
                        }
                        else
                        {
                            spinShow = false;
							await InvokeAsync(StateHasChanged);
							// Create SOP Template
							if (await js.Confirmation("Yes", $"Are you want to save this SOP '{SOPmodel.Name}' next time.", SweetAlertTypeMessagee.success))
                            {
                                spinShow = true;
								await InvokeAsync(StateHasChanged);
								sopResult = await InsertUpdateTemplate(SOPmodel);
                            }

                            //If company have a dedicated Team TODO: aminul review
                            spinShow = true;
							await InvokeAsync(StateHasChanged);
							if (companyTeam != null)
                            {
                                order.AssignedTeamId = companyTeam.TeamId;
                            }
                            // var addResponse = await
                            // (order);
                            ClientOrderSOPTemplateModel clientOrderSOPTemplate = new ClientOrderSOPTemplateModel();
                            clientOrderSOPTemplate.SOP_Template_Id = sopResult.Id; //TODO: Aminul : make it null able
                            clientOrderSOPTemplate.Order_ClientOrder_Id = (int)order.Id;
                            clientOrderSOPTemplate.SOP_Template_Name = @"Default - " + DateTime.Now.ToString("yyyymmddHHmmss");

                            await _orderTemplateService.Insert(clientOrderSOPTemplate);

                            spinShow = false;
                            //Order Attachment Upload
                            await AddNewOrderAttachment();
                            if (spinShow == false)
                            {
                                spinShow = true;
                            }
                            //Update order instusctions here
                            await AddOrderInstructions("Instructions", order.Instructions);

                            UpdateOrderStatusInDb(order, InternalOrderStatus.OrderPlaced);
                            OrderUploadActivityLog((int)order.Id, company);
                            await SendMailToAllClient(order.CompanyId, (int)order.Id, "add");
                            await SendMailToAllOperation("Add");
                            await SendInternalMessage("OrderAdd");
                            spinShow = false;
							await InvokeAsync(StateHasChanged);
							await js.DisplayMessage("Your Order Place Succesfully");
                            spinShow = true;
							await InvokeAsync(StateHasChanged);

							if (sopResult != null && sopResult.Id > 0)
                            {
                                spinShow = false;
								await InvokeAsync(StateHasChanged);
								if (await js.Confirmation("Yes", $"Are You Want To Rename OF {SOPmodel.Name} SOP ?", SweetAlertTypeMessagee.question))
                                {
                                    NameChangesShowAddEditPopup();
                                    spinShow = true;
									await InvokeAsync(StateHasChanged);
									sopTemplate = await _sopTemplateService.GetById((int)sopResult.Id);
                                    sopTemplate.OrderTemplateId = sopResult.OrderTemplateId;
                                }

                                else
                                {
                                    spinShow = true;
									await InvokeAsync(StateHasChanged);
									orderobjectid = order.ObjectId;
                                    UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
                                }

                            }
                            else
                            {
                                orderobjectid = order.ObjectId;
                                UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
                            }
                            orderobjectid = order.ObjectId;
                            order = new ClientOrderModel();
                        }
                    }
                    else
                    {
                        //If company have a dedicated team 

                        if (companyTeam != null)
                        {
                            order.AssignedTeamId = companyTeam.TeamId;
                        }

                        // Insert Order SOP Template 
                        #region Order SOP Insert From Original SOP Template
                        if (order.SOPTemplateList != null && order.SOPTemplateList.Any())
                        {
                            // Find SOP or Fetch the selected SOP and Bind the SOP for Order SOP Template
                            foreach (var sopTemplate in order.SOPTemplateList)
                            {
                                // Fetch Original SOP Standard Service
                                var standardServiceList = await _sopStandardService.GetListByTemplateId(sopTemplate.SOP_Template_Id);
                                if (standardServiceList != null && standardServiceList.Any())
                                {
                                    foreach (var standard in standardServiceList)
                                    {
                                        var orderStandardService = new OrderSOPStandardServiceModel()
                                        {
                                            Name = standard.Name,
                                            SortOrder = standard.SortOrder,
                                            Status = standard.Status,
                                            IsDeleted = standard.IsDeleted,
                                            CreatedByContactId = standard.CreatedByContactId,
                                            ObjectId = Guid.NewGuid().ToString(),
                                            ParentSopServiceId = standard.ParentSopServiceId,
                                            BaseSOPServiceId = standard.Id,
                                        };
                                        var orderSOPInsertResult = await _orderSOPStandardService.Insert(orderStandardService);

                                        orderSOPStandardServiceListIds.Add(orderSOPInsertResult.Result);
                                    }

                                }

                                // Insert Order SOP Template
                                var fetchOriginalSOPTemplate = await _sopTemplateService.GetById(sopTemplate.SOP_Template_Id);
                                var orderSOPTemplateId = 0;
                                if (fetchOriginalSOPTemplate != null)
                                {
                                    var orderSOPTemplate = new OrderSOPTemplateModel()
                                    {
                                        BaseTemplateId = fetchOriginalSOPTemplate.Id,
                                        Name = fetchOriginalSOPTemplate.Name,
                                        CompanyId = fetchOriginalSOPTemplate.CompanyId,
                                        FileServerId = fetchOriginalSOPTemplate.FileServerId,
                                        Version = fetchOriginalSOPTemplate.Version,
                                        ParentTemplateId = fetchOriginalSOPTemplate.ParentTemplateId,
                                        Instruction = fetchOriginalSOPTemplate.Instruction,
                                        UnitPrice = fetchOriginalSOPTemplate.UnitPrice,
                                        InstructionModifiedByContactId = fetchOriginalSOPTemplate.InstructionModifiedByContactId,
                                        Status = fetchOriginalSOPTemplate.Status,
                                        IsDeleted = fetchOriginalSOPTemplate.IsDeleted,
                                        CreatedByContactId = fetchOriginalSOPTemplate.CreatedByContactId,
                                        ObjectId = Guid.NewGuid().ToString(),
                                    };
                                    var orderSOPTemplateSaveResult = await _orderTemplateSOPService.Insert(orderSOPTemplate);
                                    orderSOPTemplateId = orderSOPTemplateSaveResult.Result;

                                    if (orderSOPTemplateSaveResult.IsSuccess)
                                    {
                                        var clientOrderSOPTemplate = new ClientOrderSOPTemplateModel()
                                        {
                                            Order_ClientOrder_Id = (int)order.Id,
                                            SOP_Template_Id = fetchOriginalSOPTemplate.Id,
                                            OrderSOP_Template_Id = orderSOPTemplateId,
                                        };
                                        await _orderTemplateService.Insert(clientOrderSOPTemplate);
                                    }
                                }


                                // Insert Order SOPService and Order SOPTemplate
                                if (orderSOPStandardServiceListIds != null && orderSOPStandardServiceListIds.Count > 0)
                                {
                                    foreach (var orderStandardService in orderSOPStandardServiceListIds)
                                    {

                                        var orderSOPStandardServiceTemplate = new OrderSOPTemplateServiceModel()
                                        {
                                            OrderSOPTemplateId = orderSOPTemplateId,
                                            OrderSOPStandardServiceId = orderStandardService,

                                            ObjectId = Guid.NewGuid().ToString(),
                                            BaseTemplateId = sopTemplate.SOP_Template_Id,

                                        };
                                        var orderSopServiceAndSOPTemplateSaveResult = await _orderSOPTemplateOrderSOPStandardService.Insert(orderSOPStandardServiceTemplate);
                                    }
                                }

                                // Insert Order SOP Attachment File from Original SOP Attachement File
                                var orderSOPTemplateAttachmentFiles = await _sopTemplateService.GetSopTemplateFilesBySopTemplateId(sopTemplate.SOP_Template_Id);
                                if (orderSOPTemplateAttachmentFiles != null && orderSOPTemplateAttachmentFiles.Any())
                                {
                                    foreach (var orderSOPTemplateAttachmentFile in orderSOPTemplateAttachmentFiles)
                                    {
                                        var fileSavePath = $"{_webHostEnvironment.WebRootPath}\\Upload\\Order Attachments\\{company.Name}\\{order.OrderNumber}\\{fetchOriginalSOPTemplate.Name + " " + orderSOPTemplateId}\\";
                                        if (!Directory.Exists(fileSavePath))
                                        {
                                            Directory.CreateDirectory(fileSavePath);
                                        }
                                        if (File.Exists(orderSOPTemplateAttachmentFile.ActualPath))
                                        {
                                            File.Copy(orderSOPTemplateAttachmentFile.ActualPath, $"{fileSavePath}\\{orderSOPTemplateAttachmentFile.FileName}");
                                        }
                                        var orderSOPTemplateFile = new OrderSOPTemplateFile()
                                        {
                                            OrderSOPTemplateId = orderSOPTemplateId,
                                            FileName = orderSOPTemplateAttachmentFile.FileName,
                                            FileType = orderSOPTemplateAttachmentFile.FileType,
                                            ActualPath = $"{fileSavePath}\\{orderSOPTemplateAttachmentFile.FileName}".Replace("\\", "/"),
                                            ModifiedPath = orderSOPTemplateAttachmentFile.ModifiedPath,
                                            IsDeleted = orderSOPTemplateAttachmentFile.IsDeleted,
                                            CreatedByContactId = orderSOPTemplateAttachmentFile.CreatedByContactId,
                                            ObjectId = Guid.NewGuid().ToString(),
                                            RootFolderPath = $"wwwroot\\Upload\\Order Attachments\\{company.Name}\\{order.OrderNumber}\\{fetchOriginalSOPTemplate.Name + " " + orderSOPTemplateId}".Replace("\\", "/"),
                                            ViewPath = $"{company.Name}\\{order.OrderNumber}\\{fetchOriginalSOPTemplate.Name + " " + orderSOPTemplateId}".Replace("\\", "/"),
                                            FileByteString = orderSOPTemplateAttachmentFile.FileByteString,
                                            BaseSOPTemplateFileId = orderSOPTemplateAttachmentFile.Id,
                                            BaseTemplateId = orderSOPTemplateAttachmentFile.SOPTemplateId,
                                        };
                                        var orderSOPTemplateFileSaveResult = await _orderTemplateSOPFileService.Insert(orderSOPTemplateFile);
                                    }
                                }

                            }
                        }
                        #endregion End Order SOP Insert

                        // Insert Order Template
                        // await _orderTemplateService.InsertList(order.SOPTemplateList, (int)order.Id);


                        var fileServer = await _fileServerService.GetById(company.FileServerId);

                        //Upload File For Order // Folder Upload
                        //Order Attachment Upload
                        await AddNewOrderAttachment();

                        //Update order instusctions here
                        await AddOrderInstructions("Instructions", order.Instructions);


                        UpdateOrderStatusInDb(order, InternalOrderStatus.OrderPlaced);
                        await AddOrderStatusChangeLog(order, InternalOrderStatus.OrderPlaced); // Order Status Change log
                        OrderUploadActivityLog((int)order.Id, company);
                        await SendMailToAllClient(order.CompanyId, (int)order.Id, "add");
                        await SendMailToAllOperation("Add");
                        await SendInternalMessage("OrderAdd");
                        spinShow = false;
						await InvokeAsync(StateHasChanged);
						await js.DisplayMessage("Your Order Place Succesfully");
                        spinShow = true;
						await InvokeAsync(StateHasChanged);
						orderobjectid = order.ObjectId;
                        UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
                        order = new ClientOrderModel();

                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertUpdateOrder",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            isSubmitting = false;
            spinShow = false;
            isCreatingSop = false;
        }
        #endregion

        #region Delete Order
        protected void Delete(string objectId, string name)
        {
            var msg = $"Are you sure you want to delete the order \"{name}\"?";
            ModalNotification.ShowConfirmation("Confirm Delete", msg);
        }       

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            try
            {
                if (deleteConfirmed)
                {
                    var deleteResponse = await _orderService.Delete(objectId);

                    if (deleteResponse.IsSuccess)
                    {
                        order = new ClientOrderModel();
                        CompanyModel company = await _companyService.GetById(order.CompanyId);
                        ActivityLogModel activityLog = new ActivityLogModel();
                        activityLog.ActivityLogFor = ActivityLogForConstants.Order;
                        activityLog.PrimaryId = (int)order.Id;
                        activityLog.Description = $"Deleted Order ‘{order.OrderNumber}' by user {company.Name} on {DateTime.Now}";
                        activityLog.CreatedDate = DateTime.Now;
                        activityLog.CreatedByContactId = company.Id;
                        activityLog.ContactObjectId = loginUser.UserObjectId;
                        activityLog.CompanyObjectId = loginUser.CompanyObjectId;
                        activityLog.ObjectId = Guid.NewGuid().ToString();

                        await _activityLogService.Insert(activityLog);
                    }
                    else
                    {
                        ModalNotification.ShowMessage("Error", deleteResponse.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "ConfirmDelete_Click",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

            isSubmitting = false;
            UriHelper.NavigateTo("/orders");
        }
        #endregion

        #region Operation Mail for Order
        private async Task SendMailToAllClient(int companyId, int orderId, string callerType)
        {
            try
            {
                var contactList = await _contactManager.GetByCompanyId(companyId);
                var detailUrl = $"{_configuration["AppMainUrl"]}/order/Details/{order.ObjectId}";
                var orderAddUpdateNotification = new OrderAddUpdateNotification
                {
                    Contacts = contactList,
                    DetailUrl = detailUrl,
                    CreatedByContactId = loginUser.ContactId,
                    OrderNumber = order.OrderNumber
                };
                if (callerType == "update")
                {
                    await _workflowEmailService.SendOrderUpdateNotificationForCompanyAllUsers(orderAddUpdateNotification);
                }
                else
                {
                    await _workflowEmailService.SendOrderAddNotificationForCompanyAllUsers(orderAddUpdateNotification);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendMailToAllClient",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }

        private async Task SendMailToAllOperation(string callerType)
        {
            try
            {
                var userList = await _operationEmailService.GetUserListByCompanyIdAndPermissionName(Convert.ToInt32(_configuration["CompanyId"]), PermissionConstants.OrderNewOrderEmailNotifyForOPeration);
                foreach (var user in userList)
                {
                    var detailUrl = $"{_configuration["AppMainUrl"]}/order/details/{order.ObjectId}";

                    var ordervm = new ClientOrderViewModel
                    {
                        Contact = user,
                        DetailUrl = detailUrl,
                        CreatedByContactId = loginUser.ContactId,
                        OrderNumber = order.OrderNumber,
                    };

                    ordervm.MailType = callerType;
                    await _workflowEmailService.SendOrderAddUpdateDeleteNotificationForCompanyOperationsTeam(ordervm);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendMailToAllOperation",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        #endregion

        #region Attachment upload for SOP
        private async void AttachmentUpload(InputFileChangeEventArgs e)
        {

            if (order.Id <= 0)
            {
                await js.DisplayMessage("At First Select at least one order file");
                return;
            }

            spinShow = true;
            isSubmitting = true;
            isCreatingSop = true;
            FileUploadModel fileUploadVM = new FileUploadModel();
            await _dateTime.DateTimeConvert(DateTime.Now);
            fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\OrderAttachment\\";

            using (var ftp = await _fluentFtpService.CreateFtpClient(await GetFileServerViewModel()))
            {
                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                ftp.Config.ValidateAnyCertificate = true;
                foreach (var file in e.GetMultipleFiles(maximumFileCount: 3000))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                        {
                            string orderAttachmentUploadFilePath = $"{fileServer.SubFolder}/{fileUploadVM.UploadDirectory + file.Name}";


                            if (!await ftp.DirectoryExists(Path.GetDirectoryName(orderAttachmentUploadFilePath)))
                            {
                                await ftp.CreateDirectory(Path.GetDirectoryName(orderAttachmentUploadFilePath));
                            }

                            await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400),orderAttachmentUploadFilePath, FtpRemoteExists.Overwrite, true);
                        }
                        else
                        {
                            string orderAttachmentUploadFilePath = $"{fileUploadVM.UploadDirectory} + {file.Name}";


                            if (!await ftp.DirectoryExists(Path.GetDirectoryName(orderAttachmentUploadFilePath)))
                            {
                                await ftp.CreateDirectory(Path.GetDirectoryName(orderAttachmentUploadFilePath));
                            }

                            await ftp.UploadStream(file.OpenReadStream(maxAllowedSize: 107374182400), orderAttachmentUploadFilePath, FtpRemoteExists.Overwrite, true);
                        }
                        var orderFileAttachment = new OrderFileAttachment();
                        var name = file.Name.ToString();
                        orderFileAttachment.FileName = name;
                        orderFileAttachment.File = file;
                        orderFileAttachment.ObjectId = Guid.NewGuid().ToString();
                        orderFileAttachment.CreatedByContactId = loginUser.ContactId;
                        orderFileAttachment.FileName = file.Name;
                        orderFileAttachment.FileType = file.ContentType;
                        orderFileAttachment.FileSize = (byte)file.Size;
                        orderFileAttachment.PartialPath = fileUploadVM.UploadDirectory;
                        order.orderAttachments.Add(orderFileAttachment);
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                        {
                            PrimaryId = (int)order.Id,
                            ActivityLogFor = (int)ActivityLogForConstants.Order,
                            loginUser = loginUser,
                            ErrorMessage = ex.Message,
                            MethodName = "AttachmentUpload",
                            RazorPage = "NewOrder.razor.cs",
                            Category = (int)ActivityLogCategory.OrderUploadError,
                        };
                        await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                        await js.DisplayMessage($"{ex.Message}");
                    }
                }

                isSubmitting = false;
                spinShow = false;
                isCreatingSop = false;
            }
			await InvokeAsync(StateHasChanged);

		}
        #endregion

        #region SOP Create
        protected async Task SOPServiceChanged(int id)
        {
            try
            {
                if (newSelectedSOPTemplateList.Contains(id))
                {
                    newSelectedSOPTemplateList.Remove(id);
                }
                else
                {
                    newSelectedSOPTemplateList.Add(id);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SOPServiceChanged",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }


        // For SOP Details
        protected async Task ClickedView(SOPTemplateModel sTemplate)
        {
            sopTemplate = await _sopTemplateService.GetById(sTemplate.Id);
            ShowTemplatePopup();
        }
        void ShowTemplatePopup()
        {
            isSOPTemplateView = true;
        }
        void HideTemplatePopup()
        {
            isSOPTemplateView = false;
        }
        void AddNewSOP()
        {
            sopTemplate = new SOPTemplateModel { Status = (int)GeneralStatus.Active };

            isSubmitting = false;
            ShowAddEditPopup();
        }
        void ShowAddEditPopup()
        {
            isInsertUpdateTemplatePopupVisible = true;
        }
        void CloseAddEditPopup()
        {
            isInsertUpdateTemplatePopupVisible = false;
        }

        void NameChangesShowAddEditPopup()
        {
            isSOPNameChange = true;
        }

        void NameChangesCloseAddEditPopup()
        {
            UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
        }

        private async Task<List<ClientOrderSOPTemplateModel>> GetSOPTemplate()
        {
            var orderTemplates = new List<ClientOrderSOPTemplateModel>();
            try
            {
                orderTemplates = new List<ClientOrderSOPTemplateModel>();
                foreach (var serviceId in newSelectedSOPTemplateList)
                {
                    var orderTemplate = new ClientOrderSOPTemplateModel();
                    orderTemplate.SOP_Template_Id = serviceId;
                    orderTemplates.Add(orderTemplate);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetSOPTemplate",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return orderTemplates;
        }

        private async Task LoadStandardServices()
        {
            try
            {
                if (sopTemplate.Id != 0)
                {
                    List<SOPTemplateServiceModel> templateServiceList = await _sopStandardService.GetTemplateServiceByTemplateId(sopTemplate.Id);
                    var serviceList = await _sopStandardService.GetAll();
                    templateServiceList.ForEach((tService) =>
                    {
                        var service = serviceList.Find(service => service.Id == tService.SOPStandardServiceId);
                        if (service != null) selectedSeviceIdList.Add(service.Id);
                    });

                }
                sopService = await _sopStandardService.GetAll();
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "LoadStandardServices",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task<List<SOPTemplateServiceModel>> GetSopServices()
        {
            List<SOPTemplateServiceModel> sopTemplateServices = new List<SOPTemplateServiceModel>();
            try
            {
                foreach (var serviceId in SOPStandardServiceList)
                {
                    SOPTemplateServiceModel sopTemplateServiceModel = new SOPTemplateServiceModel();
                    sopTemplateServiceModel.SOPStandardServiceId = serviceId;
                    sopTemplateServiceModel.ObjectId = Guid.NewGuid().ToString();
                    sopTemplateServiceModel.Status = (int)GeneralStatus.Active;
                    sopTemplateServiceModel.CreatedByContactId = loginUser.ContactId;

                    sopTemplateServices.Add(sopTemplateServiceModel);

                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetSopServices",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return sopTemplateServices;
        }

        private async void OnSopTemplateAttachmentInputFileChange(InputFileChangeEventArgs e)
        {
            spinShow = true;
            isSubmitting = true;
            if (sopTemplate.SopTemplateFileList == null)
            {
                sopTemplate.SopTemplateFileList = new List<SOPTemplateFile>();
            }
            // For Progress Bar 
            maxValue = e.GetMultipleFiles(maximumFileCount: 2000).Count();
            string filepath = "";
            string viewPath = "";
            string folder = "";
            string rootfolder = "";
            CompanyModel company = await _companyService.GetById(loginUser.CompanyId);
            CurrentValueForSopAttachmentProgressbar = 0;
            int count = 0;
            DateTimeConfiguration _dateTime = new DateTimeConfiguration();
            foreach (var file in e.GetMultipleFiles(maximumFileCount: 3000))
            {
                try
                {
                    var sopTemplateFile = new SOPTemplateFile();

                    using (var memoryStream = new MemoryStream())
                    {
                        if (string.IsNullOrWhiteSpace(rootfolder))
                        {
                            folder = $"{this._webHostEnvironment.WebRootPath}\\Upload\\{company.Name}\\SOP-{_dateTime.currenDateTime}";
                            rootfolder = $"wwwroot\\Upload\\{company.Name}\\SOP-{_dateTime.currenDateTime}";
                            await _folderService.CreateFolder(folder);
                            viewPath = $"{company.Name}/SOP-{_dateTime.currenDateTime}";
                        }
                        await file.OpenReadStream(maxAllowedSize: 102400000000000).CopyToAsync(memoryStream);
                        filepath = $"{folder}\\{file.Name}";

                        await _localFileService.UploadAsync(filepath, memoryStream);
                        count++;
                        CurrentValueForSopAttachmentProgressbar = (int)((100 / maxValue) * count);

                        sopTemplateFile.ActualPath = filepath;
                        sopTemplateFile.ViewPath = viewPath;
                        sopTemplateFile.RootFolderPath = rootfolder;
                        sopTemplateFile.File = file;
                        sopTemplateFile.FileName = file.Name;

                        sopTemplate.SopTemplateFileList.Add(sopTemplateFile);
						await InvokeAsync(StateHasChanged);
					}
                }
                catch (Exception ex)
                {
                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                    {
                        PrimaryId = (int)order.Id,
                        ActivityLogFor = (int)ActivityLogForConstants.Order,
                        loginUser = loginUser,
                        ErrorMessage = ex.Message,
                        MethodName = "OnSopTemplateAttachmentInputFileChange",
                        RazorPage = "NewOrder.razor.cs",
                        Category = (int)ActivityLogCategory.OrderUploadError,
                    };
                    await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                    await js.DisplayMessage($"{ex.Message}");
                }
                if (CurrentValueForSopAttachmentProgressbar == 100)
                {
                    CurrentValueForSopAttachmentProgressbar = 0;
					await InvokeAsync(StateHasChanged);
				}
            }
            spinShow = false;
            isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}

        protected void ServiceChanged(int id)
        {
            if (SOPStandardServiceList.Contains(id))
            {
                SOPStandardServiceList.Remove(id);
            }
            else
            {
                SOPStandardServiceList.Add(id);
            }
        }

        private async void DeleteAttachImagesOnPreview(OrderFileAttachment orderAttachment)
        {
            try
            {
                await Task.Yield();
                order.orderAttachments.RemoveAll(f => f.FileName == orderAttachment.FileName);
				await InvokeAsync(StateHasChanged);
				var fileServer = await GetFileServerViewModel();
                FileUploadModel fileUploadVM = new FileUploadModel()
                {
                    FtpUrl = fileServer.Host,
                    userName = fileServer.UserName,
                    password = fileServer.Password,
                };
                if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                {
                    fileUploadVM.ReturnPath = $"{fileServer.SubFolder}/{orderAttachment.PartialPath}\\{orderAttachment.FileName}";
                }
                else
                {
                    fileUploadVM.ReturnPath = $"{orderAttachment.PartialPath}\\{orderAttachment.FileName}";
                }
                await _fluentFtpService.DeleteFile(fileUploadVM);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "DeleteAttachImagesOnPreview",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        #endregion

        #region Internal Message
        private async Task SendMailToAllClientForSOP(string callerType, SOPTemplateModel sop)
        {
            try
            {
                var contactList = await _contactManager.GetByCompanyId(loginUser.CompanyId);
                var detailUrl = $"{_configuration["AppMainUrl"]}/sop/templates/Details/{sop.ObjectId}";
                SOPAddUpdateNotification sOPAddUpdateNotification = new SOPAddUpdateNotification
                {
                    Contacts = contactList,
                    DetailUrl = detailUrl,
                    CreatedByContactId = loginUser.ContactId,
                    TemplateName = sop.Name
                };
                sOPAddUpdateNotification.MailType = callerType;
                await _workflowEmailService.SendSopAddUpdateDeleteNotificationForCompanyAllUsers(sOPAddUpdateNotification);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendMailToAllClientForSOP",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task SendInternalMessageForSOP(string callerType, SOPTemplateModel sop)
        {
            try
            {
                var contactList = await _contactManager.GetByCompanyId(loginUser.CompanyId);
                InternalMessageNotification internalMessageNotification = new InternalMessageNotification
                {
                    Contacts = contactList,
                    SenderContactId = loginUser.ContactId,
                    TemplateName = sop.Name
                };

                internalMessageNotification.MessageType = callerType;
                await _internalMessageService.Insert(internalMessageNotification);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendInternalMessageForSOP",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task SendMailToAllOperationForSOP(string callerType, SOPTemplateModel sop)
        {
            try
            {
                var company = await _companyService.GetByObjectId(_configuration["CompanyObjectId"]);
                var role = await _roleManager.GetAll(company.ObjectId);
                //var singleRole = role.Where(x => x.Name.Contains("Operation")).FirstOrDefault();
                var singleRole = role.Where(x => x.Name.Contains("Operation")).ToList();
                foreach (var singlerole in singleRole)
                {
                    var userList = await _roleManager.GetAllUserRole(singlerole.ObjectId);
                    foreach (var user in userList)
                    {
                        var userInfo = await _userService.GetByObjectId(user.UserObjectId);
                        var contactInfo = await _contactManager.GetById(userInfo.ContactId);
                        var detailUrl = $"{_configuration["AppMainUrl"]}/sop/templates/Details/{sop.ObjectId}";
                        SOPAddUpdateNotification sOPAddUpdateNotification = new SOPAddUpdateNotification
                        {
                            Contact = contactInfo,
                            DetailUrl = detailUrl,
                            CreatedByContactId = loginUser.ContactId,
                            TemplateName = sop.Name
                        };
                        sOPAddUpdateNotification.MailType = callerType;
                        await _workflowEmailService.SendSopAddUpdateDeleteNotificationForCompanyOperationsTeam(sOPAddUpdateNotification);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendMailToAllOperationForSOP",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task SendInternalMessage(string callerType)
        {
            try
            {
                var contactList = await _contactManager.GetByCompanyId(loginUser.CompanyId);
                InternalMessageNotification internalMessageNotification = new InternalMessageNotification
                {
                    Contacts = contactList,
                    SenderContactId = loginUser.ContactId,
                    OrderNumber = order.OrderNumber,
                    MessageType = callerType
                };
                await _internalMessageService.Insert(internalMessageNotification);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendInternalMessage",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        #endregion

        #region Update Template
        private async Task InsertUpdateTemplate()
        {
            try
            {
                isSubmitting = true;
                spinShow = true;
                CompanyModel company = await _companyService.GetByObjectId(loginUser.CompanyObjectId);

                if (sopTemplate.Id == 0)
                {
                    if (!string.IsNullOrWhiteSpace(sopTemplate.Instruction) && !string.IsNullOrWhiteSpace(sopTemplate.Name))
                    {
                        sopTemplate.CreatedByContactId = loginUser.ContactId;
                        sopTemplate.ObjectId = Guid.NewGuid().ToString();
                        sopTemplate.SopTemplateServiceList = await GetSopServices();
                        sopTemplate.FileServerId = 1;
                        sopTemplate.CompanyId = company.Id;
                        sopTemplate.Status = (int)SopStatus.New;

                        var addResponse = await _sopTemplateService.Insert(sopTemplate);

                        if (!addResponse.IsSuccess)
                        {
                            ModalNotification.ShowMessage("Error", addResponse.Message);
                            isSubmitting = false;
                            return;
                        }

                        //Add File

                        await InsertSopActivityLogForSOP(sopTemplate, company);
                        //await SendMailToAllClientForSOP("Add", sopTemplate);
                        //await SendInternalMessageForSOP("Add", sopTemplate);
                        //await SendMailToAllOperation("Add");
                        ServiceChanged(addResponse.Result);
                        var objectId = sopTemplate.ObjectId;
                        sopTemplate = new SOPTemplateModel();
                        isSubmitting = false;
                        spinShow = false;
                        StateHasChanged();
                        UriHelper.NavigateTo($"/order/upload", true);
                        await js.DisplayMessage("Your SOP Created Successfully");

                    }

                    else
                    {
                        await js.DisplayMessage("Please Enter SOP Instraction");
                        UriHelper.NavigateTo($"/order/upload", true);
                    }
                }
                else
                {
                    await js.DisplayMessage("Your SOP Already Exists");
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertUpdateTemplate",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task<List<SOPTemplateFile>> GetTemplateFile(string filePath, string RootFolder, string viewPath)
        {
            List<SOPTemplateFile> sopTemplateFileList = new List<SOPTemplateFile>();
            try
            {
                foreach (var file in order.orderAttachments)
                {
                    SOPTemplateFile sopTemplateFile = new SOPTemplateFile();
                    sopTemplateFile.ObjectId = Guid.NewGuid().ToString();
                    sopTemplateFile.CreatedByContactId = loginUser.ContactId;
                    sopTemplateFile.FileName = file.FileName;
                    sopTemplateFile.ActualPath = filePath;
                    sopTemplateFile.FileType = file.FileType;
                    sopTemplateFile.RootFolderPath = RootFolder;
                    sopTemplateFile.ViewPath = viewPath;
                    sopTemplateFileList.Add(sopTemplateFile);
                }

            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "GetTemplateFile",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return sopTemplateFileList;
        }
        private async Task<SOPTemplateModel> InsertUpdateTemplate(SOPTemplateModel sopTemplate)
        {
            SOPTemplateModel sop = new SOPTemplateModel();
            isSubmitting = true;
            CompanyModel company = await _companyService.GetByObjectId(loginUser.CompanyObjectId);
            sop.Name = @"Default - " + DateTime.Now.ToString("yyyyMMddHHmmss");
            sop.CreatedByContactId = loginUser.ContactId;
            sop.ObjectId = Guid.NewGuid().ToString();
            sop.SopAttachment = sopTemplate.SopAttachment;
            sop.Instruction = sopTemplate.Instruction;
            sop.SopTemplateServiceList = await GetSopServices();
            sop.FileServerId = 1;
            sop.CompanyId = company.Id;
            sop.Status = (int)SopStatus.New;
            string RootFolderPath = "";
            string filepath = "";
            string viewPath = "";
            if (sop.SopAttachment != null)
            {
                foreach (var file in sop.SopAttachment)
                {
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var datetime = DateTime.Now.ToString("yyyymmddHHMMSS");
                            await file.OpenReadStream().CopyToAsync(memoryStream);
                            var folder = $"{this._webHostEnvironment.WebRootPath}\\Upload\\{company.Code}\\SOP-{datetime}";
                            var rootfolder = $"wwwroot\\Upload\\{company.Code}\\SOP-{datetime}";
                            await _folderService.CreateFolder(folder);
                            RootFolderPath = rootfolder;
                            filepath = $"{folder}\\{file.Name}";
                            viewPath = $"{company.Code}/SOP-{datetime}";
                            await _localFileService.UploadAsync(filepath, memoryStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                        {
                            PrimaryId = (int)order.Id,
                            ActivityLogFor = (int)ActivityLogForConstants.Order,
                            loginUser = loginUser,
                            ErrorMessage = ex.Message,
                            MethodName = "InsertUpdateTemplate",
                            RazorPage = "NewOrder.razor.cs",
                            Category = (int)ActivityLogCategory.OrderUploadError,
                        };
                        await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                        await js.DisplayMessage($"{ex.Message}");
                    }
                }
                sop.SopTemplateFileList = await GetTemplateFile(filepath, RootFolderPath, viewPath);
            }
            var addResponse = await _sopTemplateService.Insert(sop);

            // Cloning SOP For Order Page
            #region Cloning SOP For Order SOP
            try
            {
                if (addResponse.IsSuccess)
                {
                    // Insert SOP Order
                    var fetchOriginalSOPTemplate = await _sopTemplateService.GetById(addResponse.Result);
                    var orderSOPTemplateId = 0;
                    if (fetchOriginalSOPTemplate != null)
                    {
                        var orderSOPTemplate = new OrderSOPTemplateModel()
                        {
                            BaseTemplateId = fetchOriginalSOPTemplate.Id,
                            Name = fetchOriginalSOPTemplate.Name,
                            CompanyId = fetchOriginalSOPTemplate.CompanyId,
                            FileServerId = fetchOriginalSOPTemplate.FileServerId,
                            Version = fetchOriginalSOPTemplate.Version,
                            ParentTemplateId = fetchOriginalSOPTemplate.ParentTemplateId,
                            Instruction = fetchOriginalSOPTemplate.Instruction,
                            UnitPrice = fetchOriginalSOPTemplate.UnitPrice,
                            InstructionModifiedByContactId = fetchOriginalSOPTemplate.InstructionModifiedByContactId,
                            Status = fetchOriginalSOPTemplate.Status,
                            IsDeleted = fetchOriginalSOPTemplate.IsDeleted,
                            CreatedByContactId = fetchOriginalSOPTemplate.CreatedByContactId,
                            ObjectId = Guid.NewGuid().ToString(),
                        };
                        var orderSOPTemplateSaveResult = await _orderTemplateSOPService.Insert(orderSOPTemplate);
                        sopTemplate.OrderTemplateId = orderSOPTemplateSaveResult.Result;

                        if (orderSOPTemplateSaveResult.IsSuccess)
                        {
                            if (orderSOPTemplateSaveResult.IsSuccess)
                            {
                                var clientOrderSOPTemplate = new ClientOrderSOPTemplateModel()
                                {
                                    Order_ClientOrder_Id = (int)order.Id,
                                    SOP_Template_Id = fetchOriginalSOPTemplate.Id,
                                    OrderSOP_Template_Id = sopTemplate.OrderTemplateId,
                                };
                                await _orderTemplateService.Insert(clientOrderSOPTemplate);
                            }
                        }
                    }
                    // Insert Order SOP Attachment File from Original SOP Attachement File
                    var orderSOPTemplateAttachmentFiles = await _sopTemplateService.GetSopTemplateFilesBySopTemplateId(addResponse.Result);
                    if (orderSOPTemplateAttachmentFiles != null && orderSOPTemplateAttachmentFiles.Any())
                    {
                        foreach (var orderSOPTemplateAttachmentFile in orderSOPTemplateAttachmentFiles)
                        {
                            var fileSavePath = $"{_webHostEnvironment.WebRootPath}\\Upload\\Order Attachments\\{company.Name}\\{order.OrderNumber}\\{fetchOriginalSOPTemplate.Name + " " + orderSOPTemplateId}\\";
                            if (!Directory.Exists(fileSavePath))
                            {
                                Directory.CreateDirectory(fileSavePath);
                            }
                            if (File.Exists(orderSOPTemplateAttachmentFile.ActualPath))
                            {
                                File.Copy(orderSOPTemplateAttachmentFile.ActualPath, $"{fileSavePath}\\{orderSOPTemplateAttachmentFile.FileName}");
                            }
                            var orderSOPTemplateFile = new OrderSOPTemplateFile()
                            {
                                OrderSOPTemplateId = orderSOPTemplateId,
                                FileName = orderSOPTemplateAttachmentFile.FileName,
                                FileType = orderSOPTemplateAttachmentFile.FileType,
                                ActualPath = $"{fileSavePath}\\{orderSOPTemplateAttachmentFile.FileName}".Replace("\\", "/"),
                                ModifiedPath = orderSOPTemplateAttachmentFile.ModifiedPath,
                                IsDeleted = orderSOPTemplateAttachmentFile.IsDeleted,
                                CreatedByContactId = orderSOPTemplateAttachmentFile.CreatedByContactId,
                                ObjectId = Guid.NewGuid().ToString(),
                                RootFolderPath = $"wwwroot\\Upload\\Order Attachments\\{company.Name}\\{order.OrderNumber}\\{fetchOriginalSOPTemplate.Name + " " + orderSOPTemplateId}".Replace("\\", "/"),
                                ViewPath = $"{company.Name}\\{order.OrderNumber}\\{fetchOriginalSOPTemplate.Name + " " + orderSOPTemplateId}".Replace("\\", "/"),
                                FileByteString = orderSOPTemplateAttachmentFile.FileByteString,
                                BaseSOPTemplateFileId = orderSOPTemplateAttachmentFile.Id,
                                BaseTemplateId = orderSOPTemplateAttachmentFile.SOPTemplateId,
                            };
                            var orderSOPTemplateFileSaveResult = await _orderTemplateSOPFileService.Insert(orderSOPTemplateFile);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertUpdateTemplate - sop cloning section",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            #endregion

            sopTemplate.Id = addResponse.Result;
            if (!addResponse.IsSuccess)
            {
                ModalNotification.ShowMessage("Error", addResponse.Message);
                isSubmitting = false;
            }
            await InsertSopActivityLogForSOP(sopTemplate, company);
            await SendMailToAllClientForSOP("Add", sop);
            await SendInternalMessageForSOP("Add", sop);
            await SendMailToAllOperationForSOP("Add", sop);
            //await js.Confirmation("Yes", $"You want to use Next Time this {sop.Name} SOP", SweetAlertTypeMessagee.success);

            var objectId = sop.ObjectId;
            isSubmitting = false;
            return sopTemplate;
        }

        private async Task UpdateTemplateName()
        {
            try
            {
                sopTemplate.UpdatedByContactId = loginUser.ContactId;
                var result = await _sopTemplateService.Update(sopTemplate);
                OrderSOPTemplateModel orderSOPTemplate = new OrderSOPTemplateModel()
                {
                    Id = sopTemplate.OrderTemplateId,
                    Name = sopTemplate.Name,

                };
                var orderSOPModified = await _orderTemplateSOPService.UpdateSOPTemplateName(orderSOPTemplate);
                UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateTemplateName",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        #endregion

        #region Folder Upload Model
        private static List<FileForUploadDetails> _selectedFileFromJs = new();

        [JSInvokable]
        public static Task GetSelectedFileDetails(List<FileForUploadDetails> files)
        {
            _selectedFileFromJs = files;
            return Task.CompletedTask;
        }

        #endregion

        #region Show/Hide UI Controls
        private void ShowFolder()
        {
            AllowFolderUpload = true;
            radioValue = 1;
			StateHasChanged();
		}
        private void ShowFile()
        {
            AllowFolderUpload = false;
            radioValue = 0;
            StateHasChanged();
		}
        //private async Task UpdateUIAsync(Action updateAction)
        //{
        //    await InvokeAsync(() =>
        //    {
        //        updateAction?.Invoke();
        //        StateHasChanged();
        //    });
        //}
        #endregion

        #region Update Order Status
        private async void UpdateOrderStatusInDb(ClientOrderModel clientOrder, InternalOrderStatus status)
        {
            try
            {
                clientOrder.InternalOrderStatus = (byte)status;
                clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(status));
                await _orderService.UpdateClientOrderStatus(clientOrder);




                //await _orderStatusService.UpdateOrderArrivalTime(clientOrder);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UpdateOrderStatusInDb",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        #endregion

        #region Order Attachment And Instruction Upload
        private async Task AddNewOrderAttachment()
        {
            try
            {
                CompanyModel company = await _companyService.GetById(loginUser.CompanyId);
                var serverInfo = await _fileServerService.GetById(company.FileServerId);
                List<OrderFileAttachment> orderAttachments = new List<OrderFileAttachment>();
                if (order.orderAttachments != null)
                {
                    var tempAttachmentList = order.orderAttachments;
                    // Order Instruction and Attachment Add
                    foreach (var file in order.orderAttachments)
                    {
                        var orderFileAttachment = new OrderFileAttachment
                        {
                            Order_ClientOrder_Id = order.Id,
                            CompanyId = loginUser.CompanyId,
                            FileName = file.FileName,
                            PartialPath = file.PartialPath,
                            Status = 1,//Todo:Rakib See aminul vai
                            IsDeleted = false,
                            CreatedByContactId = loginUser.ContactId,
                            CreateDated = System.DateTime.Now,
                            ObjectId = file.ObjectId,
                            FileSize = file.FileSize
                        };
                        orderAttachments.Add(orderFileAttachment);
                    }
                    await _orderFileAttachmentService.Insert(orderAttachments, (int)order.Id);
                }
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddNewOrderAttachment",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task AddOrderInstructions(string fieldName, string value)
        {
            order.Instructions = value;
            await _orderService.UpdateSingleField((int)order.Id, fieldName, value, loginUser.ContactId);

        }
        #endregion

        #region Cancel Uploading

        private async Task UploadCancel()
        {
            try
            {
                uploadCancel = true;
                await _orderService.Delete(order.ObjectId);
                await _clientOrderItemService.Delete(order.ObjectId);
                UriHelper.NavigateTo("/order/upload", true);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "UploadCancel",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        private async Task UploadItemCancel()
        {
            await Task.Yield();
            spinShow = true;
            uploadItemCancel = true;
			await InvokeAsync(StateHasChanged);
		}
        #endregion

        #region SOP Db Actions
        private async void DeleteSopTemplateFileOnPreview(string fileName)
        {
            try
            {
                await Task.Yield();

                sopTemplate.SopTemplateFileList = sopTemplate.SopTemplateFileList.Where(f => f.FileName != fileName).ToList();
				await InvokeAsync(StateHasChanged);
			}
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "DeleteSopTemplateFileOnPreview",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        #endregion

        #region Logs
        private async void OrderUploadActivityLog(int orderId, CompanyModel company)
        {
            try
            {
                company = await _companyService.GetById(order.CompanyId);
                ActivityLogModel activityLog = new ActivityLogModel();
                activityLog.ActivityLogFor = ActivityLogForConstants.Order;
                activityLog.PrimaryId = orderId;
                if (orderId == 0)
                {
                    activityLog.Description = $"Created a new Order Upload ‘{order.Id}' Created By User {company.Name} on {DateTime.Now}";
                }
                else
                {
                    activityLog.Description = $"Update Order ‘{order.OrderNumber}' Updated By User {company.Name} on {DateTime.Now}";
                }
                activityLog.CreatedDate = DateTime.Now;
                activityLog.CreatedByContactId = company.Id;
                activityLog.ObjectId = Guid.NewGuid().ToString();
                activityLog.ContactObjectId = loginUser.UserObjectId;
                activityLog.CompanyObjectId = loginUser.CompanyObjectId;
                await _activityLogService.Insert(activityLog);
            }
            catch
            {
            }
        }

        private async Task InsertSopActivityLogForSOP(SOPTemplateModel sop, CompanyModel company)
        {
            try
            {
                ActivityLogModel activityLog = new ActivityLogModel();
                activityLog.ActivityLogFor = ActivityLogForConstants.SOPTemplate;
                activityLog.PrimaryId = sop.Id;

                if (sop.Id > 0)
                {
                    activityLog.Description = $"Update SOP ‘{sop.Name}' by {loginUser.FirstName} on {DateTime.Now}";
                }
                else
                {
                    activityLog.Description = $"Created a new SOP ‘{sop.Name}' by {loginUser.FirstName} on {DateTime.Now}";
                }

                activityLog.CreatedDate = DateTime.Now;
                activityLog.CreatedByContactId = company.Id;
                activityLog.ObjectId = Guid.NewGuid().ToString();
                activityLog.CompanyObjectId = loginUser.CompanyObjectId;
                activityLog.ContactObjectId = loginUser.UserObjectId;

                await _activityLogService.Insert(activityLog);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertSopActivityLogForSOP",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        //TODO: Move to Service
        public async Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus)
        {
            try
            {
                OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
                {
                    OrderId = (int)clientOrder.Id,
                    NewInternalStatus = (byte)internalOrderStatus,
                    NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
                    ChangeByContactId = loginUser.ContactId,
                    ChangeDate = DateTime.Now
                };
                var previousLog = await _orderStatusChangeLogService.OrderStatusLastChangeLogByOrderId((int)clientOrder.Id);

                if (previousLog != null)
                {
                    orderStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                    orderStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                    orderStatusChangeLog.TimeDurationInMinutes = orderStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).Minutes;
                }
                await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddOrderStatusChangeLog",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }

        public async Task AddOrderItemStatusChangeLog(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus)
        {
            try
            {
                OrderItemStatusChangeLogModel orderItemStatusChangeLog = new OrderItemStatusChangeLogModel
                {
                    OrderFileId = (int)clientOrderItem.Id,
                    NewInternalStatus = (byte)internalOrderItemStatus,
                    NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
                    ChangeByContactId = loginUser.ContactId,
                    ChangeDate = DateTime.Now
                };
                var previousLog = await _orderItemStatusChangeLogService.OrderItemStatusLastChangeLogByOrderFileId((int)clientOrderItem.Id);

                if (previousLog != null)
                {
                    orderItemStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                    orderItemStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                    orderItemStatusChangeLog.TimeDurationInMinutes = orderItemStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).Minutes;
                }
                await _orderItemStatusChangeLogService.Insert(orderItemStatusChangeLog);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddOrderItemStatusChangeLog",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        private async Task InsertClientUploadActivityLog(ClientOrderItemModel clientOrderItem, string editorFilePath)
        {
            try
            {
                ActivityLogModel activityLog = new ActivityLogModel();
                activityLog.ActivityLogFor = ActivityLogForConstants.OrderItem;
                activityLog.PrimaryId = (int)clientOrderItem.Id;
                activityLog.Description = $"File ‘{clientOrderItem.FileName}' Uploaded Path '{editorFilePath}' by {loginUser.FirstName} on {DateTime.Now}";
                activityLog.CreatedDate = DateTime.Now;
                activityLog.CreatedByContactId = loginUser.ContactId;
                activityLog.ObjectId = Guid.NewGuid().ToString();
                activityLog.CompanyObjectId = loginUser.CompanyObjectId;
                activityLog.ContactObjectId = loginUser.UserObjectId;

                await _activityLogService.Insert(activityLog);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "InsertClientUploadActivityLog",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
        }
        #endregion

        #region Private Methods

        #region Initial Load
        //private async Task LoadOrder()
        //{
        //    if (string.IsNullOrWhiteSpace(objectId))
        //    {
        //        order = new ClientOrder { CreatedByContactId = loginUser.ContactId, ExternalOrderStatus = (int)GeneralStatus.Active };
        //    }
        //    else
        //    {
        //        order = await _orderService.GetByObjectId(objectId);
        //    }
        //}
        private async Task LoadSOPTemplates()
        {
            if (order.Id > 0)
            {
                List<ClientOrderSOPTemplateModel> templateList = await _orderTemplateService.GetAllByOrderId((int)order.Id);

                foreach (var item in templateList)
                {
                    selectedTemplateList.Add(item.SOP_Template_Id);
                }
            }

            templateService = await _sopTemplateService.GetAllByCompany(loginUser.CompanyId);
        }
        #endregion

        #region Create Order
        //public static List<byte[]> SplitArray(byte[] array, int chunkSize)
        //{
        //	List<byte[]> splitArrays = new List<byte[]>();
        //	int offset = 0;

        //	while (offset < array.Length)
        //	{
        //		int remaining = array.Length - offset;
        //		int currentChunkSize = Math.Min(chunkSize, remaining);
        //		byte[] chunk = new byte[currentChunkSize];
        //		Array.Copy(array, offset, chunk, 0, currentChunkSize);
        //		splitArrays.Add(chunk);
        //		offset += currentChunkSize;
        //	}

        //	return splitArrays;
        //}
        private FileServerViewModel PopulateFielServerViewModel()
        {
            FileServerViewModel fileServerViewModel = new FileServerViewModel();
            fileServerViewModel.Host = fileServer.Host;
            fileServerViewModel.UserName = fileServer.UserName;
            fileServerViewModel.Password = fileServer.Password;
            fileServerViewModel.SubFolder = fileServer.SubFolder;
            return fileServerViewModel;
        }
        
        private async Task AddOrderInfo()
        {
            try
            {
                if (order.Id > 0)
                {
                    return;
                }

                order.CreatedByContactId = loginUser.ContactId;
                order.UpdatedByContactId = loginUser.ContactId;
                var dateTime = DateTime.Now;
                order.OrderNumber = $"{company.Code}-{company.Id}-{dateTime.ToString("ddMMyyyyHHmmss")}";
                order.ObjectId = Guid.NewGuid().ToString();
                order.CreatedDate = System.DateTime.Now;
                order.UpdatedDate = DateTime.Now;
                order.OrderPlaceDate = DateTime.Now;
                order.CompanyId = loginUser.CompanyId;
                order.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(InternalOrderStatus.OrderPlacing));
                order.InternalOrderStatus = (byte)InternalOrderStatus.OrderPlacing;
                order.FileServerId = company.FileServerId;
                order.OrderType = (int)OrderType.NewWork;

                //default client ftp server
                if (order?.SourceServerId == 0 || order?.SourceServerId == null)
                {
                    if (ClientOrderFtps != null && ClientOrderFtps.Any())
                    {
                        var destinationServer = ClientOrderFtps.FirstOrDefault(x => x.IsDefault == true);
                        if (destinationServer != null)
                        {
                            order.SourceServerId = (int)destinationServer.Id;
                        }
                        else
                        {
                            // Set Default File Server Id- Here set File Server Id not Client Ftp Server
                            order.SourceServerId = company.FileServerId;
                        }
                    }
                }

                // Add Order Base Path 

                var companyTeam = await _companyTeamService.GetByCompanyId(loginUser.CompanyId);
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
                    await js.DisplayMessage(addResponse.Message);
                }
                order.Id = addResponse.Result;
                await AddOrderStatusChangeLog(order, InternalOrderStatus.OrderPlacing);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = 0,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddOrderInfo",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");

            }
        }
        
        private async Task<string> CreateTextFileName(List<ClientOrderItemModel> files)
        {
            await Task.Yield();
            var text = "";
            var fileName = "";
            var filesArray = files.ToArray();
            for (int i = 0; i < filesArray.ToList().Count; i++)
            {
                fileName += $"{i}) {filesArray[i].FileName}\n";
            }
            text = $"File Names : \n {fileName}\n";
            return text;
        }

        private async Task<Response<long>> AddOrderItem(ClientOrderItemModel clientOrderItem)
        {
            Response<long> addItemResponse = null;
            try
            {
                clientOrderItem.IsDeleted = false;
                clientOrderItem.ObjectId = Guid.NewGuid().ToString();
                clientOrderItem.CreatedByContactId = loginUser.ContactId;
                clientOrderItem.FileGroup = (int)OrderItemFileGroup.Work;
                //clientOrderItem.ArrivalTime = DateTime.Now;
                //Set status
                clientOrderItem.Status = (byte)InternalOrderItemStatus.OrderPlaced;
                clientOrderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.OrderPlaced));


                if (companyTeam != null)
                {
                    clientOrderItem.TeamId = companyTeam.TeamId;
                }

                //Add Order Item / Files in database 
                addItemResponse = await _clientOrderItemService.Insert(clientOrderItem, (long)order.Id);

                if (addItemResponse.IsSuccess)
                {
                    clientOrderItem.Id = addItemResponse.Result;
                    order.orderItems.Add(clientOrderItem);
                    await InvokeAsync(StateHasChanged);
                    await AddOrderItemStatusChangeLog(clientOrderItem, InternalOrderItemStatus.OrderPlaced);
                }
                return addItemResponse;
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "AddOrderItem",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }
            return addItemResponse;
        }

        private async void DeleteClientOrderItemOnPreview(int id, string objectId)
        {
            try
            {
                await Task.Yield();
                // Delete from view list
                order.orderItems.RemoveAll(f => f.ObjectId == objectId);
                await InvokeAsync(StateHasChanged);
                //Delete from ftp
                await _clientOrderItemService.DeleteFileFromFtp(id);
                //Delete FROM Database
                await _clientOrderItemService.Delete(objectId);
            }
            catch (Exception ex)
            {
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogForConstants.Order,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "DeleteClientOrderItemOnPreview",
                    RazorPage = "NewOrder.razor.cs",
                    Category = (int)ActivityLogCategory.OrderUploadError,
                };
                await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
                await js.DisplayMessage($"{ex.Message}");
            }

        }
        #endregion

        #region Populate Objects
        private async Task<FileServerViewModel> GetFileServerViewModel()
        {
            await Task.Yield();
            FileServerViewModel fileServerViewModel = new FileServerViewModel();
            fileServerViewModel.Host = fileServer.Host;
            fileServerViewModel.UserName = fileServer.UserName;
            fileServerViewModel.Password = fileServer.Password;

            return fileServerViewModel;

        }

        #endregion

        #endregion
    }
}

