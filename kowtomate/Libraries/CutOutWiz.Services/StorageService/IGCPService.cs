
namespace CutOutWiz.Services.StorageService
{
    public interface IGCPService
    {
        Task<bool> InsertFileIntoGcpBucket(string filePath, int serverId, Stream stream, string fileType);
        Task<MemoryStream> DownloadAttachment(int fileServerId,string filePath);
       

    }
}