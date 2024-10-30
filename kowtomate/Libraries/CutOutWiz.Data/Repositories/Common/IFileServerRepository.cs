using CutOutWiz.Core;
using CutOutWiz.Data.DTOs.Ftp;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public interface IFileServerRepository
	{
        Task<Response<bool>> Delete(string objectId);
        Task<List<FileServerEntity>> GetAll();
        /// <summary>
        /// Get Default Page
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        Task<FileServerEntity> GetDefaultFileServer();
        Task<FileServerEntity> GetById(int fileServerId);
        Task<FileServerEntity> GetByObjectId(string objectId);
        Task<Response<int>> Insert(FileServerEntity fileServer);
        Task<Response<bool>> Update(FileServerEntity fileServer);
        Task<List<ClientExternalOrderFTPSetupDto>> GetAllClientFtp();
        Task<List<ClientExternalOrderFTPSetupDto>> GetAllClientFtpByQuery(string query);
        Task<ClientExternalOrderFTPSetupDto> GetClientFtpByQuery(string query);

        Task<List<ClientExternalOrderFTPSetupDto>> GetEnabledClientStorage(int companyId);
        Task<List<ClientExternalOrderFTPSetupDto>> GetEnabledInternalStorage(int companyId);

    }
}
