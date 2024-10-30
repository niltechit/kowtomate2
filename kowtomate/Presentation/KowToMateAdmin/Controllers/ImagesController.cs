using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.Model;
using DocumentFormat.OpenXml.Drawing.Charts;
using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.StorageService;
using Mailjet.Client.Resources;
using CutOutWiz.Services.FeedbackReworkServices;
using CutOutWiz.Services.Models.Feedback;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.Managers.Common;

namespace KowToMateAdmin.Controllers
{


    public class ImagesController : Controller
    {
        private readonly IClientOrderService _orderService;
        private readonly IFileServerManager _fileServerService;
        private readonly ICompanyManager _companyService;
        DateTimeConfiguration _dateTime = new DateTimeConfiguration();
        private readonly IFluentFtpService _fluentFtpService;
        private readonly IWorkContext _workContext;
        private readonly IOrderFileAttachmentService _orderFileAttachmentService;
        private readonly IFeedbackReworkService _feedbackReworkService;
        private readonly IClientOrderItemService _clientOrderItemService;
        private readonly IUpdateOrderItemBLLService _updateOrderItemBLLService;
        public ImagesController(IClientOrderService orderService, IFileServerManager fileServerService, ICompanyManager companyService,
            IFluentFtpService fluentFtpService, IWorkContext workContext, IOrderFileAttachmentService orderFileAttachmentService,
            IFeedbackReworkService feedbackReworkService, IClientOrderItemService clientOrderItemService, IUpdateOrderItemBLLService updateOrderItemBLLService)
        {
            _orderService= orderService;
            _fileServerService= fileServerService;
            _companyService=companyService;
            _fluentFtpService= fluentFtpService;
            _workContext= workContext;
            _orderFileAttachmentService=orderFileAttachmentService;
            _feedbackReworkService= feedbackReworkService;
            _clientOrderItemService=clientOrderItemService;
            _updateOrderItemBLLService= updateOrderItemBLLService;
        }
        [HttpPost]
        public async Task<IActionResult> SaveMarkup([FromBody] RejectFileRequestModel model)
        {

            var order = await _orderService.GetById(model.OrderId);
            var fileServer = await _fileServerService.GetById((int)order.FileServerId);
            var company = await _companyService.GetById(order.CompanyId);
            var clientOrderItem = await _clientOrderItemService.GetById(model.OrderItemId);
            var base64Split = model.Base64.Split(",");

            var fileServerViewModel = new FileServerViewModel
            {
                Host = fileServer.Host,
                UserName = fileServer.UserName,
                Password = fileServer.Password,
            };

            FileUploadModel fileUploadVM = new FileUploadModel();
            await _dateTime.DateTimeConvert(order.CreatedDate);
            fileUploadVM.UploadDirectory = $"{company.Code}\\{_dateTime.year}\\{_dateTime.month}\\{_dateTime.date}\\Raw\\{order.OrderNumber}\\FeebackFiles\\";
            byte[] imageBytes = null;
            try
            {
                // Convert the base64 string to bytes
                imageBytes = Convert.FromBase64String(base64Split[1]);

            }
            catch (Exception ex)
            {

            }
            // Convert the base64 string to bytes

            using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
            {
                ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
                ftp.Config.ValidateAnyCertificate = true;
                await ftp.AutoConnect();
                try
                {
                    if (!string.IsNullOrWhiteSpace(fileServer.SubFolder))
                    {
                        await ftp.UploadBytes(imageBytes, $"{fileServer.SubFolder}/{fileUploadVM.UploadDirectory + clientOrderItem.FileName}", FtpRemoteExists.Overwrite, true);
                    }
                    else
                    {
                        await ftp.UploadBytes(imageBytes, fileUploadVM.UploadDirectory + clientOrderItem.FileName, FtpRemoteExists.Overwrite, true);
                    }


                    var feedbackOrderItem = new FeedbackOrderItemModel
                    {
                        ClientOrderId = model.OrderId,
                        ClientOrderItemId=model.OrderItemId,
                        FeedBackImagePath = $"{fileUploadVM.UploadDirectory}/{clientOrderItem.FileName}",
                        CreatedById= model.CreatedContactId,
                        CreatedDate= DateTime.Now,
                    };
                    await _feedbackReworkService.Insert(feedbackOrderItem);

                    

                    await _updateOrderItemBLLService.UpdateOrderItemStatus(clientOrderItem, InternalOrderItemStatus.Rejected);

                    await ftp.Disconnect();

                    return Json(new { type = "success", message="Successfully rejected."});

                }
                catch (Exception ex)
                {
                    return Json(new { type = "warning", message = "Problem on rejected. " + ex.Message });
                }
               
            }
        }
    }
}
