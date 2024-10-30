using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Repositories.Common;
using CutOutWiz.Data.Entities.Common;
using CutOutWiz.Data.DTOs.Ftp;

namespace CutOutWiz.Services.Managers.Common
{
    public class FileServerManager : IFileServerManager
    {
        private readonly IMapperHelperService _mapperHelperService;
        private readonly IFileServerRepository _fileServerRepository;

        public FileServerManager(IFileServerRepository fileServerRepository,
            IMapperHelperService mapperHelperService)
        {
            _fileServerRepository = fileServerRepository;
            _mapperHelperService = mapperHelperService;
        }

        /// <summary>
        /// Get All FileServers
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileServerModel>> GetAll()
        {
            var entities = await _fileServerRepository.GetAll();
            return await _mapperHelperService.MapToListAsync<FileServerEntity, FileServerModel>(entities);
        }

        public async Task<List<ClientExternalOrderFTPSetupModel>> GetAllClientFtp()
        {
            var dtos = await _fileServerRepository.GetAllClientFtp();
            return await _mapperHelperService.MapToListAsync<ClientExternalOrderFTPSetupDto, ClientExternalOrderFTPSetupModel>(dtos);
        }


        public async Task<List<ClientExternalOrderFTPSetupModel>> GetEnabledClientStorage(int companyId)
        {
            var dtos = await _fileServerRepository.GetEnabledClientStorage(companyId);
            return await _mapperHelperService.MapToListAsync<ClientExternalOrderFTPSetupDto, ClientExternalOrderFTPSetupModel>(dtos);
        }

        public async Task<List<ClientExternalOrderFTPSetupModel>> GetEnabledInternalStorage(int companyId)
        {
            var dtos = await _fileServerRepository.GetEnabledInternalStorage(companyId);
            return await _mapperHelperService.MapToListAsync<ClientExternalOrderFTPSetupDto, ClientExternalOrderFTPSetupModel>(dtos);
        }

        public async Task<List<ClientExternalOrderFTPSetupModel>> GetAllClientFtpByQuery(string query)
        {
            var dtos = await _fileServerRepository.GetAllClientFtpByQuery(query);
            return await _mapperHelperService.MapToListAsync<ClientExternalOrderFTPSetupDto, ClientExternalOrderFTPSetupModel>(dtos);
        }

        public async Task<ClientExternalOrderFTPSetupModel> GetClientFtpByQuery(string query)
        {
            var dto = await _fileServerRepository.GetClientFtpByQuery(query);
            return await _mapperHelperService.MapToSingleAsync<ClientExternalOrderFTPSetupDto, ClientExternalOrderFTPSetupModel>(dto);
        }

        //public async Task<List<ClientExternalOrderFTPSetup>> GetClientFtpByQuery(string query)
        //{
        //	var filteredList = await _db.LoadDataUsingQuery<ClientExternalOrderFTPSetup, dynamic>(query,
        //			new
        //			{
        //			});


        //	return filteredList;
        //}

        /// <summary>
        /// Get fileServer by fileServer Id
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServerModel> GetById(int fileServerId)
        {
            var entity = await _fileServerRepository.GetById(fileServerId);
            return await _mapperHelperService.MapToSingleAsync<FileServerEntity, FileServerModel>(entity);
        }

        /// <summary>
        /// Get Default Page
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServerModel> GetDefaultFileServer()
        {
            var entity = await _fileServerRepository.GetDefaultFileServer();
            return await _mapperHelperService.MapToSingleAsync<FileServerEntity, FileServerModel>(entity);
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServerModel> GetByObjectId(string objectId)
        {
            var entity = await _fileServerRepository.GetByObjectId(objectId);
            return await _mapperHelperService.MapToSingleAsync<FileServerEntity, FileServerModel>(entity);
        }

        /// <summary>
        /// Insert fileServer
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(FileServerModel fileServerModel)
        {
            //Add validation logic here
            var entity = await _mapperHelperService.MapToSingleAsync<FileServerModel, FileServerEntity>(fileServerModel);
            return await _fileServerRepository.Insert(entity);
        }

        /// <summary>
        /// Update FileServer
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(FileServerModel fileServerModel)
        {
            //Add validation logic here
            var entity = await _mapperHelperService.MapToSingleAsync<FileServerModel, FileServerEntity>(fileServerModel);
            return await _fileServerRepository.Update(entity);
        }

        /// <summary>
        /// Delete FileServer by id
        /// </summary>
        /// <param name="fileServerId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            return await _fileServerRepository.Delete(objectId);
        }
    }
}
