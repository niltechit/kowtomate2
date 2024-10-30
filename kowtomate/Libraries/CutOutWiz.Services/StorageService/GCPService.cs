using CutOutWiz.Core;
using CutOutWiz.Services.Models.GcpModel;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.StorageService
{
    public class GCPService : IGCPService
    {
        private readonly IFileServerManager _fileServerService;
        public GCPService(IFileServerManager fileServerService)
        {
            _fileServerService = fileServerService;
        }

        #region Public Methods
        public async Task<bool> InsertFileIntoGcpBucket(string filePath,int serverId, Stream stream, string fileType)
        {
            try
            {
                var gcpCredentials = await GetGcpCredentials(serverId);
                bool isSaved = false;

                //filePath=$"{gcpCredentials.BucketName}/{filePath}";
                using (var storageClient = StorageClient.Create(GoogleCredential.FromFile(gcpCredentials.CredentialFile)))
                {

                    var result = storageClient.UploadObject(gcpCredentials.BucketName, filePath, fileType, stream);
                    isSaved = result.Id != null;
                }

                return isSaved;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }
        public async Task<MemoryStream> DownloadAttachment(int serverId,string filePath)
		{
			try
			{
                var gcpCredentials = await GetGcpCredentials(serverId);
                using (var storageClient = StorageClient.Create(GoogleCredential.FromFile(gcpCredentials.CredentialFile)))
                {   
                    string fileDownloadPath = @"\\192.168.1.2\alljobs2\kowtomatefiles\" +"abc12.png";
                    string objectBlobName = "KTMDCL/SOP/Template-8bf61bc8-2b3c-47ae-aeb4-9c70bf8a33dc/image_2608865_73d8dd8c.png";
                    using var outputFile = File.OpenWrite(fileDownloadPath);
                    storageClient.DownloadObject(gcpCredentials.BucketName, objectBlobName, outputFile);
                    return null;
                
                }
            }
            catch(Exception ex) { 
                var message = ex.Message; 
                return null;
            }
        }

        #endregion

        #region Private Methods
        private async Task<GcpCredentialsModel> GetGcpCredentials(int fileServerId)
        {
            try
            {
                var fileServer = await _fileServerService.GetById(fileServerId);
                return new GcpCredentialsModel
                {
                    BucketName = fileServer.RootFolder,
                    CredentialFile = fileServer.SshKeyPath
                };

            }
            catch(Exception ex)
            {
                return null;
            }
        }
       #endregion
    }
}



