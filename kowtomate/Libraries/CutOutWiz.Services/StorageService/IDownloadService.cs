using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.FileUpload;

namespace CutOutWiz.Services.StorageService
{
    public interface IDownloadService
    {
        Task<FileUploadModel> CreateZipAndDownload(ContactModel contactInfo, ClientOrderModel order, string _webHostEnvironment, string ExtraName,SOPTemplateModel sopTemplate,OrderSOPTemplateModel orderSOPTemplate);
    }
}
