using CutOutWiz.Core.Utilities;
using CutOutWiz.Data;
using CutOutWiz.Data.ClientOrder;
using CutOutWiz.Data.Common;
using CutOutWiz.Data.EmailModels;
using CutOutWiz.Data.Message;
using CutOutWiz.Data.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Data.SOP;
using KowToMateAdmin.Helper;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static CutOutWiz.Core.Utilities.Enums;

namespace KowToMateAdmin.Pages.OrderUpload
{
    public partial class OrderAddOrEdit
    {
        protected ModalNotification ModalNotification { get; set; }
        [Parameter]
        public string objectId { get; set; }
        bool isSubmitting;
        private ClientOrder order = new ClientOrder();
        private List<FileServer> fileServers = new List<FileServer>();
        private LoginUserInfoViewModel loginUser = null;
        private string selectedObjectId;
        private List<SOPTemplate> templateService = new List<SOPTemplate>();
        private List<string> templateList = new List<string>();
        public List<IBrowserFile> OrderAttachment = new();
        public List<int> selectedTemplateList = new List<int>();
        public List<int> previousSOPTemplateList = new List<int>();
        public List<int> newSelectedSOPTemplateList = new List<int>();
        List<ClientOrderItem> orderItems = new List<ClientOrderItem>();

        List<OrderFileAttachment> orderAttachments = new List<OrderFileAttachment>();
        private List<SOPTemplate> standardService = new List<SOPTemplate>();

        // SOP Attachment and Instruction
        public List<IBrowserFile> SopAttachment = new();

        private string extensionName = "default";
        bool isPopupVisible = false;
        bool isSOPNameChange = false;
        private SOPTemplate sopTempalte = new SOPTemplate();
        private int CountSOP { get; set; }
        private List<SOPTemplate> sopTempaltes = new List<SOPTemplate>();
        private List<SOPStandardService> sopService = new List<SOPStandardService>();
        private List<int> selectedSeviceIdList = new List<int>();
        public List<int> SOPStandardServiceList = new List<int>();
        public SOPTemplate sTemplateForView = null;
        bool isTemplateView = false;

        IList<FileInfoViewModel> fileInfoSOP;

        //Progress Bar
        private double maxValue;
        private double CurrentValue = 0;
        // SOP Id
        public int sopId = 0;
        public SOPTemplate sopTemplate = new SOPTemplate();
        //Order Object Id
        public string orderobjectid = "";
        // For Hide Control
        private bool AllowFolderUpload = true;
        int radioValue = 0;
        bool spinShow = false;
        protected override async Task OnInitializedAsync()
        {
            loginUser = _workContext.LoginUserInfo;
            await LoadOrder();
            await LoadSOPTemplates();
            //newSelectedSOPTemplateList = selectedTemplateList;

            fileServers = await _fileServerService.GetAll();

            if (fileServers != null && fileServers.Count() > 0)
            {
                var defaultTemplate = fileServers.FirstOrDefault(f => f.IsDefault == true);
                order.FileServerId = defaultTemplate?.Id;
            }
            // For SOP Template
            sopTempaltes = await _sopTemplateService.GetAllByCompany(loginUser.CompanyId);
            CountSOP = sopTempaltes.Count();
            SOPStandardServiceList = selectedSeviceIdList;
            sopService = await _sopStandardService.GetAll();
        }

        private async Task LoadSOPTemplates()
        {
            if (order.Id != 0)
            {
                List<ClientOrderSOPTemplate> templateList = await _orderTemplateService.GetAllByOrderId((int)order.Id);
                var templatesList = await _sopTemplateService.GetAllByCompany(loginUser.CompanyId);
                foreach (var item in templateList)
                {
                    selectedTemplateList.Add(item.SOP_Template_Id);
                    newSelectedSOPTemplateList.Add(item.SOP_Template_Id);
                }
            }

            templateService = await _sopTemplateService.GetAllByCompany(loginUser.CompanyId);
        }

        private async Task LoadOrder()
        {
            if (string.IsNullOrWhiteSpace(objectId))
            {
                order = new ClientOrder { CreatedByContactId = loginUser.ContactId, ExternalOrderStatus = (int)GeneralStatus.Active };
            }
            else
            {
                order = await _orderService.GetByObjectId(objectId);
            }
        }

        private async Task InsertUpdateOrder() 
        {

            isSubmitting = true;
            
            Company company = await _companyService.GetById(loginUser.CompanyId);
            CompanyTeam team = await _companyTeamService.GetTeamByCompanyId(company.Id);
            if (order.Id == 0)
            {
                order.CreatedByContactId = loginUser.ContactId;
                order.UpdatedByContactId = loginUser.ContactId;
                // $"{Greeting}, {Name}!";
                //var dateTime = DateTime.Now;
                DateTimeConfiguration dateTimeConfiguration = new DateTimeConfiguration();
                order.OrderNumber = $"{company.Code}-{company.Id}-{dateTimeConfiguration.currenDateTime}";
                order.ObjectId = Guid.NewGuid().ToString();
                order.SOPTemplateList = GetSOPTemplate();
                order.CreatedDate = System.DateTime.Now;
                order.UpdatedDate = DateTime.Now;
                order.OrderPlaceDate = DateTime.Now;
                order.CompanyId = loginUser.CompanyId;
                order.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(InternalOrderStatus.OrderPlacing));
                order.InternalOrderStatus = (byte)InternalOrderStatus.OrderPlacing;
                order.FileServerId = company.FileServerId;
                order.OrderType = (int)OrderType.NewWork;

                // SOP Template INformation

                SOPTemplate SOPmodel = new SOPTemplate();
                SOPmodel.Instruction = order.Instructions;
                SOPmodel.SopAttachment = SopAttachment;
                SOPmodel.CompanyId = loginUser.CompanyId;
                SOPmodel.CreatedByContactId = loginUser.ContactId;
                SOPmodel.ObjectId = Guid.NewGuid().ToString();
                SOPmodel.SopTemplateServiceList = GetSopServices();
                SOPmodel.FileServerId = 1;
                // New Order Status Change
                SOPmodel.Status = (int)SopStatus.New;
                // New Order Status Change
                SOPmodel.selectedTemplateList = new List<int>();
                SOPmodel.Name = @"Default - " + DateTime.Now.ToString("yyyymmddHHmmss");
                bool sopResult = false;

                if ((order.orderItems == null || !order.orderItems.Any()) && (loadedFiless == null || !loadedFiless.Any()))
                {
                    await js.DisplayMessage("Please Attach At Least an One Order File!");
                }
                else
                {
                    //---Order Upload Time SOP Create-------
                    if (order.SOPTemplateList == null || !order.SOPTemplateList.Any())
                    {
                        if (order.Instructions == null)
                        {
                            await js.DisplayMessage("Please Select a SOP or Write Instruction for Order Upload");
                        }
                        else
                        {
                            // Create SOP Template
                            if (await js.Confirmation("Yes", $"Are You want to use Next Time this {SOPmodel.Name} SOP", SweetAlertTypeMessagee.success))
                            {
                                sopResult = await InsertUpdateTemplate(SOPmodel);
                            }
                            spinShow = true;

                            //If company have a dedicated Team 
                            CompanyTeam companyTeam = await _companyTeamService.GetTeamByCompanyId(loginUser.CompanyId);
                            if(companyTeam != null)
                            {
                                order.AssignedTeamId = companyTeam.TeamId;
                            }
                         
                            var addResponse = await _orderService.Insert(order);

                            ClientOrderSOPTemplate orderSOPTemplate = new ClientOrderSOPTemplate();
                            orderSOPTemplate.SOP_Template_Id = sopId;
                            orderSOPTemplate.Order_ClientOrder_Id = addResponse.Result;
                            orderSOPTemplate.SOP_Template_Name = @"Default - " + DateTime.Now.ToString("yyyymmddHHmmss");

                            await _orderTemplateService.Insert(orderSOPTemplate);

                            var serverInfo = await _fileServerService.GetById(company.FileServerId);
                            spinShow=false;
                            //Upload File For Order // Folder Upload
                            if (order.orderItems != null)
                            {
                                int count = 0;
                                maxValue = order.orderItems.Count;
                                foreach (var file in order.orderItems)
                                {
                                    FileUploadVM model = new FileUploadVM();
                                    model.FtpUrl = serverInfo.Host;
                                    model.userName = serverInfo.UserName;
                                    model.password = serverInfo.Password;
                                    model.RootDirectory = $"{company.Code}";
                                    model.fileName = file.File.Name;
                                    model.file = file.File;
                                    var folderPath = file.ExteranlFileInputPath;
                                    model.FolderName = Path.GetDirectoryName(folderPath);
                                    model.BaseFolder = order.OrderNumber;
                                    model.UploadDirectory = await _ftpService.CreateFtpFolder(model);
                                    var remotePath = await _ftpService.UploadFile(model);
                                    var splitRemotePath = remotePath.Split("Raw");
                                    var ExternalRemotePath = splitRemotePath[1];
                                    var externalPathReplace = ExternalRemotePath.Replace("//", "/");
                                    var order_ClientOrderItem = new ClientOrderItem
                                    {
                                        CompanyId = loginUser.CompanyId,
                                        FileName = file.FileName,
                                        ExteranlFileInputPath = remotePath + file.FileName,
                                        InternalFileInputPath = externalPathReplace,
                                        ObjectId = file.ObjectId,
                                        ClientOrderId = addResponse.Result,
                                        IsDeleted = false,
                                        FileSize = (byte)file.FileSize,
                                        FileByteString = file.FileByteString,
                                        Status = (byte)InternalOrderItemStatus.OrderPlaced,
                                        ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.OrderPlaced))
                                    };
                                    if (companyTeam != null)
                                    {
                                        order_ClientOrderItem.TeamId = companyTeam.TeamId;
                                    }

                                    orderItems.Add(order_ClientOrderItem);
                                    await _clientOrderItemService.Insert(order_ClientOrderItem, addResponse.Result);
                                    count++;
                                    CurrentValue = (int)((100 / maxValue) * count);
                                    this.StateHasChanged();
                                }
                            }
                            //Order Attachment Upload
                            if (order.orderAttachments != null)
                            {
                                // Order Instruction and Attachment Add
                                foreach (var file in order.orderAttachments)
                                {
                                    FileUploadVM model = new FileUploadVM();
                                    model.FtpUrl = serverInfo.Host;
                                    model.userName = serverInfo.UserName;
                                    model.password = serverInfo.Password;
                                    model.UploadDirectory = $"{company.Code}";
                                    model.RootDirectory = $"{company.Code}";
                                    model.fileName = file.FileName;
                                    model.file = file.File;
                                    model.BaseFolder = $"OrderAttachment/";
                                    model.OrderNumber = order.OrderNumber;
                                    model.UploadDirectory = await _ftpService.CheckingCreateFolderForOrderAttachment(model);
                                    var remotePath = await _ftpService.UploadFile(model);

                                    var orderFileAttachment = new OrderFileAttachment
                                    {
                                        Order_ClientOrder_Id = addResponse.Result,
                                        CompanyId = loginUser.CompanyId,
                                        FileName = file.FileName,
                                        InternalFileInputPath = remotePath,
                                        Status = 1,
                                        IsDeleted = false,
                                        CreatedByContactId = loginUser.ContactId,
                                        CreateDated = System.DateTime.Now,
                                        ObjectId = file.ObjectId
                                    };
                                    orderAttachments.Add(orderFileAttachment);
                                }

                                await _orderFileAttachmentService.Insert(orderAttachments, addResponse.Result);
                            }
                            if (!addResponse.IsSuccess)
                            {
                                OrderUploadActivityLog(addResponse.Result, company);
                                ModalNotification.ShowMessage("Error", addResponse.Message);
                                isSubmitting = false;
                                return;
                            }
                            spinShow = true;
                            
                            UpdateOrder(order, InternalOrderStatus.OrderPlaced);
                            OrderUploadActivityLog(addResponse.Result, company);
                            await SendMailToAllClient(order.CompanyId, addResponse.Result, "add");
                            await SendMailToAllOperation("Add");
                            await SendInternalMessage("OrderAdd");
                            spinShow = false;
                            await js.DisplayMessage("Your Order Place Succesfully");

                            if (sopResult)
                            {
                                if (await js.Confirmation("Yes", $"Are You Want To Rename OF {SOPmodel.Name} SOP ?", SweetAlertTypeMessagee.question))
                                {
                                    NameChangesShowAddEditPopup();
                                    sopTemplate = await _sopTemplateService.GetById(sopId);
                                }
                                else
                                {
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
                            order = new ClientOrder();
                            orderItems = new List<ClientOrderItem>();
                        }
                    }
                    else
                    {
                        //If company have a dedicated team 
                        CompanyTeam companyTeam = await _companyTeamService.GetTeamByCompanyId(loginUser.CompanyId);
                        if (companyTeam != null)
                        {
                            order.AssignedTeamId = companyTeam.TeamId;
                        }
                        var addResponse = await _orderService.Insert(order);
                        
                        var serverInfo = await _fileServerService.GetById(company.FileServerId);

                        //Upload File For Order
                        if (order.orderItems != null)
                        {
                            int count = 0;
                            maxValue = order.orderItems.Count;
                            foreach (var file in order.orderItems)
                            {
                                FileUploadVM model = new FileUploadVM();
                                model.FtpUrl = serverInfo.Host;
                                model.userName = serverInfo.UserName;
                                model.password = serverInfo.Password;
                                model.RootDirectory = $"{company.Code}";
                                model.fileName = file.File.Name;
                                model.file = file.File;
                                var folderPath = file.ExteranlFileInputPath;
                                model.FolderName = Path.GetDirectoryName(folderPath);
                                model.BaseFolder= order.OrderNumber;
                                model.UploadDirectory = await _ftpService.CreateFtpFolder(model);
                                var remotePath = await _ftpService.UploadFile(model);
                                var splitRemotePath = remotePath.Split("Raw");
                                var ExternalRemotePath = splitRemotePath[1];
                                var externalPathReplace = ExternalRemotePath.Replace("//", "/");
                                //var replaceRemotePath = remotePath.Replace("//", "/");
                                var FullremotePath = remotePath.Replace("//", "/") + file.FileName;
                                var order_ClientOrderItem = new ClientOrderItem
                                {
                                    CompanyId = loginUser.CompanyId,
                                    FileName = file.FileName,
                                    ExteranlFileInputPath = FullremotePath,
                                    InternalFileInputPath = externalPathReplace,
                                    ObjectId = file.ObjectId,
                                    ClientOrderId = addResponse.Result,
                                    IsDeleted = false,
                                    FileSize = (byte)file.FileSize,
                                    FileByteString = file.FileByteString,
                                    Status = (byte)InternalOrderItemStatus.OrderPlaced,
                                    ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.OrderPlaced))
                                };
                                if (companyTeam != null)
                                {
                                    order_ClientOrderItem.TeamId = companyTeam.TeamId;
                                }

                                orderItems.Add(order_ClientOrderItem);
                                await _clientOrderItemService.Insert(order_ClientOrderItem, addResponse.Result);
                                count++;
                                CurrentValue = (int)((100 / maxValue) * count);
                                this.StateHasChanged();
                            }
                        }
                        //Order Attachment Upload
                        if (order.orderAttachments != null)
                        {
                            // Order Instruction and Attachment Add
                            foreach (var file in order.orderAttachments)
                            {
                                FileUploadVM model = new FileUploadVM();
                                model.FtpUrl = serverInfo.Host;
                                model.userName = serverInfo.UserName;
                                model.password = serverInfo.Password;
                                model.UploadDirectory = $"{company.Code}";
                                model.RootDirectory = $"{company.Code}";
                                model.fileName = file.FileName;
                                model.file = file.File;
                                model.BaseFolder = $"OrderAttachment/";
                                model.OrderNumber = order.OrderNumber;
                                model.UploadDirectory = await _ftpService.CheckingCreateFolderForOrderAttachment(model);
                                var remotePath = await _ftpService.UploadFile(model);

                                var orderFileAttachment = new OrderFileAttachment
                                {
                                    Order_ClientOrder_Id = addResponse.Result,
                                    CompanyId = loginUser.CompanyId,
                                    FileName = file.FileName,
                                    InternalFileInputPath = remotePath,
                                    Status = 1,
                                    IsDeleted = false,
                                    CreatedByContactId = loginUser.ContactId,
                                    CreateDated = System.DateTime.Now,
                                    ObjectId = file.ObjectId,
                                    FileSize = file.FileSize
                                };
                                orderAttachments.Add(orderFileAttachment);
                            }

                            await _orderFileAttachmentService.Insert(orderAttachments, addResponse.Result);
                        }
                        if (!addResponse.IsSuccess)
                        {
                            OrderUploadActivityLog(addResponse.Result, company);
                            ModalNotification.ShowMessage("Error", addResponse.Message);
                            isSubmitting = false;
                            return;
                        }
                        await js.DisplayMessage("Your Order Place Succesfully");
                        UpdateOrder(order, InternalOrderStatus.OrderPlaced);
                        //await AddOrderStatusChangeLog(order, InternalOrderStatus.OrderPlaced); // Order Status Change log
                        orderobjectid = order.ObjectId;
                        UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
                        OrderUploadActivityLog(addResponse.Result, company);
                        await SendMailToAllClient(order.CompanyId, addResponse.Result, "add");
                        await SendMailToAllOperation("Add");
                        await SendInternalMessage("OrderAdd");

                        order = new ClientOrder();
                        orderItems = new List<ClientOrderItem>();

                    }
                }
            }
            // Edit
            //else
            //{
            //    order.UpdatedDate = DateTime.Now;
            //    order.UpdatedByContactId = loginUser.ContactId;
            //    //order.ExternalOrderStatus = (byte)ExternalorderStatus.Update;
            //    //order.InternalOrderStatus = (byte)InternalorderStatus.Update;
            //    order.SOPTemplateList = GetSOPTemplate();
            //    order.CompanyId = loginUser.CompanyId;
            //    order.FileServerId = company.FileServerId;
            //    //order.AssignedTeamId = team.TeamId;

            //    SOPTemplate SOPmodel = new SOPTemplate();
            //    SOPmodel.Instruction = order.Instructions;
            //    SOPmodel.SopAttachment = SopAttachment;
            //    SOPmodel.CompanyId = loginUser.CompanyId;
            //    SOPmodel.CreatedByContactId = loginUser.ContactId;
            //    SOPmodel.ObjectId = Guid.NewGuid().ToString();
            //    SOPmodel.SopTemplateServiceList = GetSopServices();
            //    SOPmodel.FileServerId = 1;
            //    SOPmodel.Status = (int)SopStatus.New;
            //    SOPmodel.selectedTemplateList = new List<int>();
            //    bool sopResult = false;
            //    //---Order Upload Time SOP Create-------
            //    if (order.SOPTemplateList == null || !order.SOPTemplateList.Any())
            //    {
                    
            //        if (order.Instructions == null)
            //        {
            //            await js.DisplayMessage("Please Select a SOP or Write Instruction for Order Edit");
            //        }
            //        else
            //        {
            //            if (await js.Confirmation("Yes", $"Are You want to use Next Time this {SOPmodel.Name} SOP", SweetAlertTypeMessagee.success))
            //            {
            //                sopResult = await InsertUpdateTemplate(SOPmodel);
            //            }
            //            //If company have a dedicated Team 
            //            CompanyTeam companyTeam = await _companyTeamService.GetTeamByCompanyId(loginUser.CompanyId);
            //            if (companyTeam != null)
            //            {
            //                order.AssignedTeamId = companyTeam.TeamId;
            //            }
            //        }
            //    }
            //    var updateResponse = await _orderService.Update(order);
            //    var serverInfo = await _fileServerService.GetById(company.FileServerId);

            //    if (loadedFiles == null && loadedFiless == null)
            //    {
            //        await js.DisplayMessage("Please Attach At Least an One Order File!");
            //    }
            //    else
            //    {
            //        // Uploada Folder With Files
            //        if (loadedFiless != null && loadedFiless.Any())
            //        {
            //            int count = 0;
            //            for (var i = 0; i < loadedFiless.Count; i++)
            //            {
            //                FileUploadVM modell = new FileUploadVM();
            //                modell.FtpUrl = serverInfo.Host;
            //                modell.userName = serverInfo.UserName;
            //                modell.password = serverInfo.Password;
            //                modell.RootDirectory = $"{company.Code}";
            //                //modell.BaseFolder = baseFolder;
            //                //model.CompanyName=company.Name
            //                var filepath = Path.GetDirectoryName(_selectedFileFromJs[i].Path);
            //                var file = loadedFiless[i];
            //                modell.FolderName = $"{updateResponse.Result}_{DateTime.Now.ToString("yyyy-MM-dd")}_{company.Id}_{OrderAttachment.Count}_Images";
            //                modell.BaseFolder = filepath;
            //                modell.file = file;
            //                FileInfoViewModel fileInfoVM = new FileInfoViewModel();
            //                fileInfoVM.browserFile = file;
            //                await ImageByteCreate(fileInfoVM);
            //                modell.fileName = file.Name;
            //                modell.UploadDirectory = await _ftpService.CreateFtpFolder(modell);
            //                var remotePath = await _ftpService.UploadFile(modell);
            //                var splitRemotePath = remotePath.Split("Raw");
            //                var ExternalRemotePath = splitRemotePath[1];
            //                var order_ClientOrderItem = new ClientOrderItem
            //                {
            //                    CompanyId = loginUser.CompanyId,
            //                    FileName = file.Name,
            //                    InternalFileInputPath = remotePath,
            //                    InternalFileOutputPath = ExternalRemotePath,
            //                    ObjectId = Guid.NewGuid().ToString(),
            //                    ClientOrderId = order.Id,
            //                    IsDeleted = false,
            //                    FileSize = (byte)file.Size,
            //                    FileByteString = imageByte,
            //                    Status = (byte)InternalOrderItemStatus.OrderPlaced,
            //                    ExternalStatus =(byte) (EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.OrderPlaced))
            //                };
            //                orderItems.Add(order_ClientOrderItem);
            //                await _clientOrderItemService.Insert(order_ClientOrderItem, (int)order.Id);
            //                count++;
            //                CurrentValue = (int)((100 / maxValue) * count);
            //                this.StateHasChanged();
            //            }
            //        }

            //        //Upload File For Order
            //        if (loadedFiles != null)
            //        {
            //            var count = 0;
            //            foreach (var file in loadedFiles)
            //            {
            //                FileUploadVM model = new FileUploadVM();
            //                model.FtpUrl = serverInfo.Host;
            //                model.userName = serverInfo.UserName;
            //                model.password = serverInfo.Password;
            //                model.RootDirectory = $"{company.Code}";
            //                model.fileName = file.Name;
            //                model.file = file;
            //                FileInfoViewModel fileInfoVM = new FileInfoViewModel();
            //                fileInfoVM.browserFile = file;
            //                await ImageByteCreate(fileInfoVM);
            //                //model.CompanyName=company.Name
            //                model.FolderName = $"{order.Id}_{DateTime.Now.ToString("yyyy-MM-dd")}_{company.Id}_{OrderAttachment.Count}_Images";
            //                model.UploadDirectory = await _ftpService.CreateFtpFolder(model);
            //                var remotePath = await _ftpService.UploadFile(model);
            //                var splitRemotePath = remotePath.Split("Raw");
            //                var ExternalRemotePath = splitRemotePath[1];
            //                var order_ClientOrderItem = new ClientOrderItem
            //                {
            //                    CompanyId = loginUser.CompanyId,
            //                    FileName = file.Name,
            //                    InternalFileInputPath = remotePath,
            //                    InternalFileOutputPath = ExternalRemotePath,
            //                    ObjectId = Guid.NewGuid().ToString(),
            //                    ClientOrderId = order.Id,
            //                    IsDeleted = false,
            //                    FileByteString = imageByte,
            //                    Status = (byte)InternalOrderItemStatus.OrderPlaced,
            //                    ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.OrderPlaced))
            //                };
            //                orderItems.Add(order_ClientOrderItem);
            //                await _clientOrderItemService.Insert(order_ClientOrderItem, (int)order.Id);
            //                count++;
            //                CurrentValue = (int)((100 / maxValue) * count);
            //                this.StateHasChanged();
            //            }
            //        }

            //        // Order Instruction and Attachment Add
            //        foreach (var file in orderAttachmeant)
            //        {
            //            FileUploadVM model = new FileUploadVM();
            //            model.FtpUrl = serverInfo.Host;
            //            model.userName = serverInfo.UserName;
            //            model.password = serverInfo.Password;
            //            model.UploadDirectory = $"{company.Code}";
            //            model.RootDirectory = $"{company.Code}";
            //            model.fileName = file.Name;
            //            model.file = file;
            //            FileInfoViewModel fileInfoVM = new FileInfoViewModel();
            //            fileInfoVM.browserFile = file;
            //            await ImageByteCreate(fileInfoVM);
            //            model.BaseFolder = $"OrderAttachment/";
            //            model.OrderNumber = order.OrderNumber;
            //            model.UploadDirectory = await _ftpService.CheckingCreateFolderForOrderAttachment(model);
            //            var remotePath = await _ftpService.UploadFile(model);

            //            var orderFileAttachment = new OrderFileAttachment
            //            {
            //                Order_ClientOrder_Id = order.Id,
            //                CompanyId = loginUser.CompanyId,
            //                FileName = file.Name,
            //                InternalFileInputPath = remotePath,
            //                Status = 1,
            //                IsDeleted = false,
            //                CreatedByContactId = loginUser.ContactId,
            //                CreateDated = System.DateTime.Now,
            //                ObjectId = Guid.NewGuid().ToString()
            //            };
            //            orderAttachments.Add(orderFileAttachment);
            //        }

            //        await _orderFileAttachmentService.Insert(orderAttachments, (int)order.Id);
            //        if (!updateResponse.IsSuccess)
            //        {
            //            OrderUploadActivityLog((int)order.Id, company);
            //            ModalNotification.ShowMessage("Error", updateResponse.Message);
            //            isSubmitting = false;
            //            return;
            //        }
            //        //await AddOrderStatusChangeLog(order, InternalOrderStatus.OrderPlaced);
            //        await js.DisplayMessage($"Your Order Updated Successfully");
            //        var objectid = order.ObjectId;
            //        UriHelper.NavigateTo("/order/Details" + "/" + objectid);
            //        OrderUploadActivityLog((int)order.Id, company);
            //        await SendMailToAllClient(order.CompanyId, (int)order.Id, "update");
            //        await SendInternalMessage("OrderUpdate");
            //        await SendMailToAllOperation("Update");

            //        //Send an mail to use

            //        order = new ClientOrder();
            //        orderItems = new List<ClientOrderItem>();

            //    }
            //}
            isSubmitting = false;
        }

        protected void Delete(string objectId, string name)
        {
            selectedObjectId = objectId;
            var msg = $"Are you sure you want to delete the order \"{name}\"?";
            ModalNotification.ShowConfirmation("Confirm Delete", msg);
        }
        // For SOP Details
        protected async Task ClickedView(SOPTemplate sTemplate)
        {
            sTemplateForView = await _sopTemplateService.GetById(sTemplate.Id);
            ShowTemplatePopup();
        }
        void ShowTemplatePopup()
        {
            isTemplateView = true;
        }
        void HideTemplatePopup()
        {
            isTemplateView = false;
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                var deleteResponse = await _orderService.Delete(selectedObjectId);

                if (deleteResponse.IsSuccess)
                {
                    order = new ClientOrder();
                    Company company = await _companyService.GetById(order.CompanyId);
                    ActivityLog activityLog = new ActivityLog();
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

            isSubmitting = false;
            UriHelper.NavigateTo("/orders");
        }

        protected void SOPServiceChanged(int id)
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

        private async void LoadFiles(InputFileChangeEventArgs args)
        {
            await Task.Yield();
            foreach (var file in args.GetMultipleFiles(maximumFileCount: 3000))
            {
                try
                {
                    var orderItemFile = new ClientOrderItem();
                    var name = file.Name.ToString();
                    extensionName = Path.GetExtension(file.Name);
                    var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg", ".avif", ".tif" };
                    var format = "image/png";

                    if (imageFileTypes.Contains(extensionName))
                    {
                        var resizeImageFile = await file.RequestImageFileAsync(file.ContentType, 100, 100);
                        var buffer = new byte[resizeImageFile.Size];
                        await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                        var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                        orderItemFile.FileByteString = imageUrl;
                    }

                    orderItemFile.FileName = name;
                    orderItemFile.File = file;

                    //
                    orderItemFile.ObjectId = Guid.NewGuid().ToString();
                    orderItemFile.CreatedByContactId = loginUser.ContactId;
                    orderItemFile.FileName = file.Name;
                    orderItemFile.FileType = file.ContentType;
                    orderItemFile.FileSize = (byte)file.Size;

                    order.orderItems.Add(orderItemFile);
                }
                catch { }
                this.StateHasChanged();
            }
            this.StateHasChanged();
        }
        private List<ClientOrderSOPTemplate> GetSOPTemplate()
        {
            var orderTemplates = new List<ClientOrderSOPTemplate>();
            foreach (var serviceId in newSelectedSOPTemplateList)
            {
                var orderTemplate = new ClientOrderSOPTemplate();
                orderTemplate.SOP_Template_Id = serviceId;
                orderTemplates.Add(orderTemplate);
            }
            return orderTemplates;
        }

        private async void OrderUploadActivityLog(int orderId, Company company)
        {
            try
            {
                company = await _companyService.GetById(order.CompanyId);
                ActivityLog activityLog = new ActivityLog();
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

        private async Task SendMailToAllClient(int companyId, int orderId, string callerType)
        {
            var contactList = await _contactService.GetByCompanyId(companyId);
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

        private async Task GetOrderItems(string path, string fileName)
        {
            await Task.Yield();

            var orderItem = new ClientOrderItem
            {
                CompanyId = order.CompanyId,
                ClientOrderId = order.Id,
                FileName = fileName,
                InternalFileInputPath = path,
                CreatedByContactId = order.CreatedByContactId,
                ObjectId = new Guid().ToString(),
            };

            orderItems.Add(orderItem);
        }

        #region Operation Mail for Order
        private async Task SendMailToAllOperation(string callerType)
        {
            var userList = await _operationEmailService.GetUserListByCompanyIdAndPermissionName(Convert.ToInt32(_configuration["CompanyId"]), PermissionContants.OrderNewOrderEmailNotifyForOPeration);
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
        #endregion

        #region Attachment upload for SOP
        private async void AttachmentUpload(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles(maximumFileCount: 3000))
            {
                try
                {
                    var orderFileAttachment = new OrderFileAttachment();
                    var name = file.Name.ToString();
                    extensionName = Path.GetExtension(file.Name);
                    var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg", ".avif", ".tif" };
                    var format = file.ContentType;

                    if (imageFileTypes.Contains(extensionName))
                    {
                        var resizeImageFile = await file.RequestImageFileAsync(file.ContentType, 100, 100);
                        var buffer = new byte[resizeImageFile.Size];
                        await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                        var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                        //fileInfo.URL.Add(imageUrl);
                        orderFileAttachment.FileByteString = imageUrl;
                    }

                    orderFileAttachment.FileName = name;
                    //SopAttachment.Add(file);
                    orderFileAttachment.File = file;

                    //
                    orderFileAttachment.ObjectId = Guid.NewGuid().ToString();
                    orderFileAttachment.CreatedByContactId = loginUser.ContactId;
                    orderFileAttachment.FileName = file.Name;
                    orderFileAttachment.FileType = file.ContentType;
                    orderFileAttachment.FileSize = (byte)file.Size;

                    order.orderAttachments.Add(orderFileAttachment);
                }
                catch { }
            }

            this.StateHasChanged();
        }
        #endregion

        #region SOP Create
        void AddNewSOP()
        {
            sopTempalte = new SOPTemplate { Status = (int)GeneralStatus.Active };

            isSubmitting = false;
            ShowAddEditPopup();
        }
        void ShowAddEditPopup()
        {
            isPopupVisible = true;
        }
        void CloseAddEditPopup()
        {
            isPopupVisible = false;
            sopTempalte = new SOPTemplate();
            fileInfoSOP = null;
        }

        void NameChangesSOP()
        {
            sopTempalte = new SOPTemplate { Status = (int)GeneralStatus.Active };

            isSubmitting = false;
            NameChangesShowAddEditPopup();
        }
        void NameChangesShowAddEditPopup()
        {
            isSOPNameChange = true;
        }

        void NameChangesCloseAddEditPopup()
        {
            UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
        }

        private async Task InsertUpdateTemplate()
        {
            isSubmitting = true;
            Company company = await _companyService.GetByObjectId(loginUser.CompanyObjectId);

            if (sopTempalte.Id == 0)
            {
                if (!string.IsNullOrWhiteSpace(sopTempalte.Instruction) && !string.IsNullOrWhiteSpace(sopTempalte.Name))
                {
                    sopTempalte.CreatedByContactId = loginUser.ContactId;
                    sopTempalte.ObjectId = Guid.NewGuid().ToString();
                    sopTempalte.SopAttachment = SopAttachment;
                    sopTempalte.SopTemplateServiceList = GetSopServices();
                    sopTempalte.FileServerId = 1;
                    sopTempalte.CompanyId = company.Id;
                    sopTempalte.Status = (int)SopStatus.New;
                    string RootFolderPath = "";
                    string filepath = "";
                    string viewPath = "";

                    if (sopTempalte.SopAttachment != null)
                    {
                        foreach (var file in sopTempalte.SopAttachment)
                        {
                            try
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await file.OpenReadStream().CopyToAsync(memoryStream);
                                    var folder = $"{this._webHostEnvironment.WebRootPath}\\Upload\\{company.Code}\\SOP-{CountSOP}";
                                    await _folderService.CreateFolder(folder);
                                    RootFolderPath = folder;
                                    filepath = $"{folder}\\{file.Name}";
                                    viewPath = $"{company.Code}/SOP-{CountSOP}";
                                    await _localFileService.UploadAsync(filepath, memoryStream);
                                }
                            }
                            catch
                            {
                            }
                        }
                        sopTempalte.SopTemplateFileList = GetTemplateFile(filepath, RootFolderPath, viewPath);
                    }

                    var addResponse = await _sopTemplateService.Insert(sopTempalte);

                    if (!addResponse.IsSuccess)
                    {
                        ModalNotification.ShowMessage("Error", addResponse.Message);
                        isSubmitting = false;
                        return;
                    }

                    //Add File

                    await InsertSopActivityLogForSOP(sopTempalte, company);
                    await SendMailToAllClientForSOP("Add", sopTempalte);
                    await SendInternalMessageForSOP("Add", sopTempalte);
                    await SendMailToAllOperation("Add");
                    ServiceChanged(addResponse.Result);
                    var objectId = sopTempalte.ObjectId;
                    sopTempalte = new SOPTemplate();
                    isSubmitting = false;
                    await js.DisplayMessage("Your SOP Created Successfully");
                    UriHelper.NavigateTo($"/order/upload", true);
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
        private List<SOPTemplateFile> GetTemplateFile(string filePath, string RootFolder, string viewPath)
        {
            List<SOPTemplateFile> templateFileList = new List<SOPTemplateFile>();
            foreach (var file in SopAttachment)
            {
                SOPTemplateFile templateFile = new SOPTemplateFile();
                templateFile.ObjectId = Guid.NewGuid().ToString();
                templateFile.CreatedByContactId = loginUser.ContactId;
                templateFile.FileName = file.Name;
                templateFile.ActualPath = filePath;
                templateFile.FileType = file.ContentType;
                templateFile.RootFolderPath = RootFolder;
                templateFile.ViewPath = viewPath;
                templateFileList.Add(templateFile);
            }
            return templateFileList;
        }
        private async Task LoadStandardServices()
        {
            if (sopTempalte.Id != 0)
            {
                List<SOPTemplateServiceModel> templateServiceList = await _sopStandardService.GetTemplateServiceByTemplateId(sopTempalte.Id);
                var serviceList = await _sopStandardService.GetAll();
                templateServiceList.ForEach((tService) =>
                {
                    var service = serviceList.Find(service => service.Id == tService.SOPStandardServiceId);
                    if (service != null) selectedSeviceIdList.Add(service.Id);
                });

            }
            sopService = await _sopStandardService.GetAll();
        }
        private List<SOPTemplateServiceModel> GetSopServices()
        {
            List<SOPTemplateServiceModel> sopTemplateServices = new List<SOPTemplateServiceModel>();

            foreach (var serviceId in SOPStandardServiceList)
            {
                SOPTemplateServiceModel sopTemplateServiceModel = new SOPTemplateServiceModel();
                sopTemplateServiceModel.SOPStandardServiceId = serviceId;
                sopTemplateServiceModel.ObjectId = Guid.NewGuid().ToString();
                sopTemplateServiceModel.Status = (int)GeneralStatus.Active;
                sopTemplateServiceModel.CreatedByContactId = loginUser.ContactId;

                sopTemplateServices.Add(sopTemplateServiceModel);

            }
            return sopTemplateServices;
        }

        private async Task<bool> InsertUpdateTemplate(SOPTemplate sopTemplate)
        {
            SOPTemplate sop = new SOPTemplate();
            isSubmitting = true;
            Company company = await _companyService.GetByObjectId(loginUser.CompanyObjectId);
            sop.Name = @"Default - " + DateTime.Now.ToString("yyyymmddHHmmss");
            //ddmmyyyyHHmmss
            sop.CreatedByContactId = loginUser.ContactId;
            sop.ObjectId = Guid.NewGuid().ToString();
            sop.SopAttachment = sopTemplate.SopAttachment;
            sop.Instruction = sopTemplate.Instruction;
            sop.SopTemplateServiceList = GetSopServices();
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
                    catch
                    {
                    }
                }
                sop.SopTemplateFileList = GetTemplateFile(filepath, RootFolderPath, viewPath);
            }
            var addResponse = await _sopTemplateService.Insert(sop);
            sopId = addResponse.Result;
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
            sopTempalte = new SOPTemplate();
            isSubmitting = false;
            return true;
            //UriHelper.NavigateTo($"/order/upload", true);
        }
        private async void OnInputFileChange(InputFileChangeEventArgs e)
        {
            // Preview upload time for attachment
            SopAttachment.Clear();

            fileInfoSOP = new List<FileInfoViewModel>();
            foreach (var file in e.GetMultipleFiles(maximumFileCount: 3000))
            {
                try
                {
                    FileInfoViewModel fileINfoVM = new FileInfoViewModel();
                    SopAttachment.Add(file);
                    var name = file.Name.ToString();
                    extensionName = Path.GetExtension(file.Name);
                    var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg" };
                    var format = "image/png";
                    if (imageFileTypes.Contains(extensionName))
                    {
                        var resizeImageFile = await file.RequestImageFileAsync(file.ContentType, 100, 100);
                        var buffer = new byte[resizeImageFile.Size];
                        await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                        var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                        fileINfoVM.URL = imageUrl;
                    }
                    fileINfoVM.FileName = name;
                    //SopAttachment.Add(file);
                    fileINfoVM.browserFile = file;
                    fileInfoSOP.Add(fileINfoVM);
                    //SopAttachment.Add(file);
                }
                catch { }
            }
            this.StateHasChanged();
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
        //private async void DeleteImagesOnPreview(FileInfoViewModel fileInfoViewModel)
        //{
        //    await Task.Yield();
        //    SopAttachment.Remove(fileInfoViewModel.browserFile);
        //    fileInfo.Remove(fileInfoViewModel);
        //}
        private async void DeleteImagesOnPreview(string fileName)
        {
            await Task.Yield();

            order.orderItems = order.orderItems.Where(f => f.FileName != fileName).ToList();
            StateHasChanged();
            //SopAttachment.Remove(fileInfoViewModel.browserFile);
            //fileInfo.Remove(fileInfoViewModel);
        } 
        private async void DeleteAttachImagesOnPreview(string fileName)
        {
            await Task.Yield();

            order.orderAttachments = order.orderAttachments.Where(f => f.FileName != fileName).ToList();
            StateHasChanged();
            //SopAttachment.Remove(fileInfoViewModel.browserFile);
            //fileInfo.Remove(fileInfoViewModel);
        }
        private async void DeleteImagesOnPreviewSOP(FileInfoViewModel fileInfoViewModel)
        {
            await Task.Yield();
            SopAttachment.Remove(fileInfoViewModel.browserFile);
            fileInfoSOP.Remove(fileInfoViewModel);
            this.StateHasChanged();
        }
        private async Task InsertSopActivityLogForSOP(SOPTemplate sop, Company company)
        {
            try
            {
                ActivityLog activityLog = new ActivityLog();
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
            catch
            {
            }
        }
        private async Task SendMailToAllClientForSOP(string callerType, SOPTemplate sop)
        {
            var contactList = await _contactService.GetByCompanyId(loginUser.CompanyId);
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

        private async Task SendInternalMessageForSOP(string callerType, SOPTemplate sop)
        {
            var contactList = await _contactService.GetByCompanyId(loginUser.CompanyId);
            InternalMessageNotification internalMessageNotification = new InternalMessageNotification
            {
                Contacts = contactList,
                SenderContactId = loginUser.ContactId,
                TemplateName = sop.Name
            };

            internalMessageNotification.MessageType = callerType;
            await _internalMessageService.Insert(internalMessageNotification);
        }

        private async Task SendMailToAllOperationForSOP(string callerType, SOPTemplate sop)
        {
            var company = await _companyService.GetByObjectId(_configuration["CompanyObjectId"]);
            var role = await _roleService.GetAll(company.ObjectId);
            //var singleRole = role.Where(x => x.Name.Contains("Operation")).FirstOrDefault();
            var singleRole = role.Where(x => x.Name.Contains("Operation")).ToList();
            foreach (var singlerole in singleRole)
            {
                var userList = await _roleService.GetAllUserRole(singlerole.ObjectId);
                foreach (var user in userList)
                {
                    var userInfo = await _userService.GetByObjectId(user.UserObjectId);
                    var contactInfo = await _contactService.GetById(userInfo.ContactId);
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
        #endregion

        #region Internal Message
        private async Task SendInternalMessage(string callerType)
        {
            var contactList = await _contactService.GetByCompanyId(loginUser.CompanyId);
            InternalMessageNotification internalMessageNotification = new InternalMessageNotification
            {
                Contacts = contactList,
                SenderContactId = loginUser.ContactId,
                OrderNumber = order.OrderNumber,
                MessageType = callerType
            };
            await _internalMessageService.Insert(internalMessageNotification);
        }
        #endregion

        private async Task UpdateTemplateName()
        {
            sopTemplate.UpdatedByContactId = loginUser.ContactId;
            var result = await _sopTemplateService.Update(sopTemplate);
            UriHelper.NavigateTo("/order/Details" + "/" + orderobjectid);
        }
        #region Folder Upload Model
        private static List<FileForUploadDetails> _selectedFileFromJs = new();
        private List<IBrowserFile> loadedFiless = new();
        private string baseFolder = "";
        public class FileForUploadDetails
        {
            public string Name { get; set; }

            public string Path { get; set; }
        }

        [JSInvokable]
        public static Task GetSelectedFileDetails(List<FileForUploadDetails> files)
        {
            _selectedFileFromJs = files;
            return Task.CompletedTask;
        }
        protected override void OnAfterRender(bool firstRender)
        {
            js.InvokeVoidAsync("attachFileUploadHandler");
        }
        private async Task LoadFile(InputFileChangeEventArgs e)
        {
            //loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
            //maxValue = e.GetMultipleFiles(maximumFileCount: 100000000).Count();

            await Task.Yield();
            loadedFiless = e.GetMultipleFiles(maximumFileCount: 100000000).ToList();
            //maxValue = e.GetMultipleFiles(maximumFileCount: 100000000).Count();

            for (int i = 0; i < loadedFiless.Count; i++)
            {
                var filePath = _selectedFileFromJs[i].Path;
                var file = loadedFiless[i];
                try
                {
                    var orderFileItem = new ClientOrderItem();
                    var name = file.Name.ToString();
                    extensionName = Path.GetExtension(file.Name);
                    var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg", ".avif", ".tif" };
                    var format = "image/png";

                    if (imageFileTypes.Contains(extensionName))
                    {
                        var resizeImageFile = await file.RequestImageFileAsync(file.ContentType, 100, 100);
                        var buffer = new byte[resizeImageFile.Size];
                        await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                        var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                        orderFileItem.FileByteString = imageUrl;
                    }
                    orderFileItem.FileName = name;
                    orderFileItem.File = file;
                    //
                    orderFileItem.ObjectId = Guid.NewGuid().ToString();
                    orderFileItem.CreatedByContactId = loginUser.ContactId;
                    orderFileItem.FileName = file.Name;
                    orderFileItem.FileType = file.ContentType;
                    orderFileItem.FileSize = (byte)file.Size;
                    orderFileItem.ExteranlFileInputPath = filePath;

                    order.orderItems.Add(orderFileItem);
                }
                catch { }
            }

        }
        #endregion

        private void ShowFolder()
        {
            AllowFolderUpload = true;
            radioValue = 1;
        }
        private void ShowFile()
        {
            AllowFolderUpload = false;
            radioValue = 0;
        }

        #region Image Byte Create
        string imageByte = "";
        public async Task ImageByteCreate(FileInfoViewModel fileInfo)
        {
            try
            {
                var name = fileInfo.browserFile.Name.ToString();
                extensionName = Path.GetExtension(fileInfo.browserFile.Name);
                var imageFileTypes = new List<string> { ".png", ".PNG", ".jpg", ".jpeg", ".avif" };
                var format = "image/png";
                if (imageFileTypes.Contains(extensionName))
                {

                    var resizeImageFile = await fileInfo.browserFile.RequestImageFileAsync(fileInfo.browserFile.ContentType, 200, 200);
                    var buffer = new byte[resizeImageFile.Size];
                    await resizeImageFile.OpenReadStream(maxAllowedSize: 1024000000000).ReadAsync(buffer);
                    var imageUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                    //fileInfo.URL.Add(imageUrl);
                    imageByte = imageUrl;
                }
            }
            catch { }
        }

        #endregion


        public async Task AddOrderStatusChangeLog(ClientOrder clientOrder, InternalOrderStatus internalOrderStatus)
        {
            OrderStatusChangeLog orderStatusChangeLog = new OrderStatusChangeLog
            {
                OrderId = (int)clientOrder.Id,
                NewInternalStatus = (byte)internalOrderStatus,
                NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
                ChangeByContactId = loginUser.ContactId
            };
            await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
        }

        public async Task AddOrderItemStatusChangeLog(ClientOrderItem clientOrderItem, InternalOrderItemStatus internalOrderItemStatus)
        {
            OrderItemStatusChangeLog orderStatusChangeLog = new OrderItemStatusChangeLog
            {
                OrderFileId = (int)clientOrderItem.Id,
                NewInternalStatus = (byte)internalOrderItemStatus,
                NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
                ChangeByContactId = loginUser.ContactId
            };
            await _orderItemStatusChangeLogService.Insert(orderStatusChangeLog);
        }

        private async void UpdateOrder(ClientOrder clientOrder, InternalOrderStatus status)
        {
            clientOrder.InternalOrderStatus = (byte)status;
            clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(status));
            await _orderService.UpdateClientOrderStatus(clientOrder);
        }
    }
}

