using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FileUpload;
using FluentFTP;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace CutOutWiz.Services.StorageService
{
	public interface IFluentFtpService
	{
        Task<AsyncFtpClient> CreateFtpClient(FileServerViewModel model);
        Task<string> UplodFile(FileUploadModel model);
        Task<bool> DeleteFile(FileUploadModel model);
        Task<Response<bool>> MoveFile(FileUploadModel model);
        Task FolderCreateAtApprovedTime(FileUploadModel model);
        Task<bool> FileExists(FileUploadModel model);
        Task<bool> SingleDownload(IJSRuntime js, FileUploadModel model,string WebHostEnvironmentPath);
		Task<bool> FolderExists(FileServerViewModel fileServerViewModel, string path);
        Task<FtpConfig> GetFluentFtpConfig();
    }
}
