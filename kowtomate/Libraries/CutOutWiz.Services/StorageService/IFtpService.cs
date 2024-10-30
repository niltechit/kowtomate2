using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FileUpload;
using FluentFTP;
using Microsoft.JSInterop;


namespace CutOutWiz.Services.StorageService
{
    public interface IFtpService
    {
        public Response<List<NodeModel>> ReadDirectoriesAsync(string path);
        public Response<List<DriveImageModel>> ReadFilesAsync(string path);
        FtpClient CreateFtpClient(FileUploadModel model);
        AsyncFtpClient CreateAsyncFtpClient(FileUploadModel model);
        Task<string> UploadFile(FileUploadModel model);
        Task<bool> DownloadFile(IJSRuntime js,FileUploadModel model);
        Task<bool> MultipleDownload(IJSRuntime js,FileUploadModel model,string dlPath);
        Task<bool> SingleDownload(IJSRuntime js,FileUploadModel model,string dlPath,bool downloadAsZip=false);
        Task<bool> DownloadWithNewFolder(IJSRuntime js,FileUploadModel model,string dlPath);
        Task<bool> SingleDownloadForAttachment(IJSRuntime js,FileUploadModel model,string dlPath);
        Task<string> MoveFile(FileUploadModel model);
        Task MoveFileForReject(FileUploadModel model);

        Task<string> CreateFtpFolder(FileUploadModel model);
        Task<string> CheckingCreateFolderForOrderAttachment(FileUploadModel model);
        Task<string> CheckingFtpFolder(FileUploadModel model);
        Task<bool> CheckingFtpFolderForEditor(FileUploadModel model);
        Task<string> CheckingFtpFolderForQC(FileUploadModel model);
        Task<string> CreateFtpFolderForEditor(FileUploadModel model);
        Task<FileServerModel> GetById(int Id);
        Task<string> UploadFileAndFolder(FileUploadModel model);
        Task CreateFolderDownloadTime(FileUploadModel model);
        Task FolderCreateAtApprovedTime(FileUploadModel model);
        Task<bool> DeleteDirectory(string directoryPath);
        Task<bool> MultipleFileDownload(List<ClientOrderItemModel> clientOrderItems, FileUploadModel model, string dlPath);
        Task<bool> SingleDownloadByQc(IJSRuntime js, FileUploadModel model, string dlPath);

	}
}
