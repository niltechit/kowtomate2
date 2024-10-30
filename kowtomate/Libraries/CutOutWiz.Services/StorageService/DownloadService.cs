using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Core;
using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using CutOutWiz.Services.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.StorageService
{
    public class DownloadService :IDownloadService
    {
        private readonly IConfiguration _configuration;
        private readonly IJSRuntime js;
        public DownloadService(IConfiguration configuration, IJSRuntime _js)
        {
            _configuration = configuration;
            js = _js;
        }
        /// <summary>
        /// Create Zip File Then Download.
        /// </summary>
        /// <param name="contactInfo"></param>
        /// <param name="order"></param>
        /// <param name="fileinfo"></param>
        /// <param name="_webHostEnvironment"></param>
        /// <returns></returns>
        public async Task<FileUploadModel> CreateZipAndDownload(ContactModel contactInfo, ClientOrderModel order, string _webHostEnvironment, string ExtraName = "", SOPTemplateModel sopTemplate = null, OrderSOPTemplateModel orderSOPTemplate = null)
        {
            FileUploadModel model = new FileUploadModel();
            try
            {
                var dataSavePath = "";
                var filestayPath = "";
                var zipfilePath = "";
                var dlUri = "";
                var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                model.ContactName = contactInfo.FirstName + " " + contactInfo.Id;
                if (order != null)
                {
                    model.OrderNumber = order.OrderNumber;
                }
                else
                {
                    model.OrderNumber = sopTemplate.Name;
                }
                if (sopTemplate != null)
                {
                    dataSavePath = $"{_webHostEnvironment}\\TempDownload" + $"\\{model.ContactName}\\{sopTemplate.Name}";
                    filestayPath = $"{_webHostEnvironment}\\TempDownload" + $"\\{model.ContactName}\\{sopTemplate.Name}";
                    zipfilePath = $"{_webHostEnvironment}\\TempDownload\\{model.ContactName}\\{sopTemplate.Name}.zip";
                }
                else if (orderSOPTemplate != null)
                {
                    dataSavePath = $"{_webHostEnvironment}\\TempDownload" + $"\\{model.ContactName}\\{orderSOPTemplate.Name}";
                    filestayPath = $"{_webHostEnvironment}\\TempDownload" + $"\\{model.ContactName}\\{orderSOPTemplate.Name}";
                    zipfilePath = $"{_webHostEnvironment}\\TempDownload\\{model.ContactName}\\{orderSOPTemplate.Name}.zip";
                }
                else
                {
                    dataSavePath = $"{_webHostEnvironment}\\TempDownload" + $"\\{model.ContactName}\\{model.OrderNumber}";
                    filestayPath = $"{_webHostEnvironment}\\TempDownload" + $"\\{model.ContactName}\\{model.OrderNumber}";
                    zipfilePath = $"{_webHostEnvironment}\\TempDownload\\{model.ContactName}\\{model.OrderNumber}.zip";
                }
                if (File.Exists(zipfilePath))
                {
                    try
                    {
                        File.Delete(zipfilePath);
                    }
                    catch (Exception ex)
                    {
                        //Do something
                    }
                }

                ZipFile.CreateFromDirectory(filestayPath, $"{dataSavePath}.zip");

                string baseUrl = $"{_configuration["AppMainUrl"]}/";
                if (sopTemplate != null)
                {
                    dlUri = baseUrl + $"\\TempDownload\\{model.ContactName}\\{sopTemplate.Name}.zip";
                }
                else if (orderSOPTemplate != null)
                {
                    dlUri = baseUrl + $"\\TempDownload\\{model.ContactName}\\{orderSOPTemplate.Name}.zip";
                }
                else
                {
                    dlUri = baseUrl + $"\\TempDownload\\{model.ContactName}\\{model.OrderNumber}.zip";
                }
                if (!string.IsNullOrEmpty(ExtraName))
                {
                    await js.InvokeVoidAsync("triggerFileDownload", model.OrderNumber + " " + ExtraName + ".zip", dlUri);
                }
                else if (sopTemplate != null)
                {
                    await js.InvokeVoidAsync("triggerFileDownload", sopTemplate.Name + ".zip", dlUri);
                }
                else if (orderSOPTemplate != null)
                {
                    await js.InvokeVoidAsync("triggerFileDownload", orderSOPTemplate.Name + ".zip", dlUri);
                }
                else
                {
                    await js.InvokeVoidAsync("triggerFileDownload", model.OrderNumber + ".zip", dlUri);
                }
            }

            catch (Exception ex)
            {
                return null;
            }
            return model;
        }

    }
}
