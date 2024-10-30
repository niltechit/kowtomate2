using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.Models.Security;
using FluentFTP;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.SftpServices
{
    public interface ISftpService
    {
        Task<SftpClient> InitializeSftpClient(ClientExternalOrderFTPSetupModel sourceFtp);
        Task<(List<string> Files, List<(string FilePath, DateTime LastWriteTime, long FileSize)> ZipFiles)> GetFilesOrZipFiles(SftpClient sftpClient, 
            ClientExternalOrderFTPSetupModel sourceFtp, CompanyGeneralSettingModel companyGeneralSetting);
        Task<List<SftpFile>> GetDirectoriesToProcess(SftpClient sftpClient, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting);
        Task OrderProcessWithDirectories(List<SftpFile> directories, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, 
            FtpCredentailsModel destinationFtp, FileServerModel fileServer);
        Task OrderProcessWithRegularFiles(SftpClient sftpClient, List<string> files, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, FtpCredentailsModel destinationFtp, 
            FileServerModel fileServer, CompanyGeneralSettingModel companyGeneralSetting);
        Task OrderProcessWithZipFiles(SftpClient sftpClient, List<(string FilePath, DateTime LastWriteTime, long FileSize)> zipFiles, ClientExternalOrderFTPSetupModel sourceFtp, 
            FtpCredentailsModel destinationFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, FileServerModel fileServer);
        Task<string> CreateTemporaryFolderForDownload();
        Task DownloadAndExtractZipFile(SftpClient sftpClient, (string FilePath, DateTime LastWriteTime, long FileSize) zipFile, string tempStorePath);
        Task CleanupTemporaryStore(string tempStorePath);
        Task<List<string>> FilterExistingFiles(CompanyModel company, List<string> files, ClientExternalOrderFTPSetupModel sourceFtp);
        Task<List<SftpFile>> GetSftpFileList(ClientExternalOrderFTPSetupModel sourceFtp,CompanyGeneralSettingModel companyGeneralSetting, int? getLastDayFiles = 0);
        Task<List<SftpFile>> GetSftpDirectories(ClientExternalOrderFTPSetupModel sourceFtp,CompanyGeneralSettingModel companyGeneralSetting, int? getLastDayFiles = 0);
        Task<string> RemoveInputRootFolder(string RootFolderPath, string FullPath);
        Task ProvideSingleDirectoryForOrderProcess(SftpFile item, ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, 
            FtpCredentailsModel destinationFtp, FileServerModel fileServer, SemaphoreSlim semaphoreSlim);
        Task<List<List<string>>> GetChunksOfPaths(List<string> allFilePath, int chunkSize);
        Task<Response<string>> CopyFilesFromLocalToFTP(ClientExternalOrderFTPSetupModel sourceFtpCredential,
          FtpCredentailsModel destinationFtpCredentails, List<string> allFilesFromNewOrder, CompanyModel company,
          FileServerModel fileServer, string orderDirectory, CompanyGeneralSettingModel companyGeneralSetting, (string filePath, DateTime LastWriteTime, long FileSize) fileInfo, 
          bool isMoveSingleFile = false, bool isLocalFile = false);
        Task<FtpCredentailsModel> CreateExternalOrderFTPSetupCredentials(ClientExternalOrderFTPSetupModel ftp);
        Task<int> CalculateChunkSize(int totalFiles);
        Task<List<List<string>>> GetFilesChunksWithPaths(List<string> allFiles, int chunkSize);
        Task<Response<int>> FileUploadFromSftpToFtpForOrderItemProcess(List<string> chunk, ClientExternalOrderFTPSetupModel sourceFtp,FtpCredentailsModel destinationFtpCredentails, CompanyModel company,
            Response<ClientOrderModel> orderSaveResponse, SemaphoreSlim semaphoreSlim, CompanyGeneralSettingModel? companyGeneralSetting = null);
        Task<Response<bool>> FileUploadFromSFTPToFtpAndInsertOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, FtpCredentailsModel destinationFtpCredentails, 
            CompanyModel company, Response<ClientOrderModel> orderSaveResponse, AsyncFtpClient destinationClient, string path);
        Task<bool> VerifyDownloadedFile(FtpCredentailsModel destinationFtpCredentails, byte[] remoteFileBytes, string localFtpPath);
        Task<bool> CompareBytes(byte[] localFileBytesArray, byte[] remoteFileBytesArray);

        Task<Response<string>> SourceSftpToDestinationFtpFileCopyAndOrderProcess(FtpCredentailsModel destinationFtp, List<string> allFilePath, CompanyModel company,
            FileServerModel fileServer, ClientExternalOrderFTPSetupModel sourceSftp,string orderBatchDirectoryPath, CompanyGeneralSettingModel? companyGeneralSetting = null);

        Task<Response<ClientOrderModel>> AddOrderInfo(CompanyModel company, FileServerModel fileServer, long sourceFtpId, string orderDirectory = "");

        Task<Response<bool>> InsertOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, Response<ClientOrderModel> orderSaveResponse, 
            string path, string uploadDirectory, string[] pathArray);

        Task<ClientOrderItemModel> PrepareClientOrderItem(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, Response<ClientOrderModel> orderSaveResponse,
            string path, string uploadDirectory, string[] pathArray);
        Task<Response<long>> AddOrderItem(ClientOrderItemModel clientOrderItem, int companyId, string clientStorageFilePath, long orderId, InternalOrderItemStatus status = 0);
        Task PerformOrderUpdates(Response<ClientOrderModel> orderSaveResponse);

        Task<bool> createBytesAndCompareFromPaths(FtpCredentailsModel sourceFtpCredential, string sourcePath, FtpCredentailsModel destinationFtpCredentails, string destinationPath);

        Task<bool> CompareByteArraysAsync(byte[] sourceBytesArray, byte[] destinationBytesArray);

        Task PrepareOrderItemCategoryInformation(ClientOrderItemModel clientOrderItem, int categoryId);
        Task OrderItemCategoryLogAdd(ClientOrderItemModel clientOrderItem);
        Task<Response<bool>> StartProcessForOrderPlacing(ClientExternalOrderFTPSetupModel sourceFtp, CompanyModel company, CompanyGeneralSettingModel companyGeneralSetting, 
            FtpCredentailsModel destinationFtp, FileServerModel fileServer);
    }
}
