using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Managers.Common
{
    public interface IFileServerManager
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<FileServerModel>> GetAll();
        /// <summary>
        /// Get Default Page
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        Task<FileServerModel> GetDefaultFileServer();
        Task<FileServerModel> GetById(int fileServerId);
        Task<FileServerModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(FileServerModel fileServer);
        Task<Response<bool>> Update(FileServerModel fileServer);
        Task<List<ClientExternalOrderFTPSetupModel>> GetAllClientFtp();
        Task<List<ClientExternalOrderFTPSetupModel>> GetAllClientFtpByQuery(string query);
        Task<ClientExternalOrderFTPSetupModel> GetClientFtpByQuery(string query);

        Task<List<ClientExternalOrderFTPSetupModel>> GetEnabledClientStorage(int companyId);
        Task<List<ClientExternalOrderFTPSetupModel>> GetEnabledInternalStorage(int companyId);

    }
}
