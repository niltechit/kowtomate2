using CutOutWiz.Core;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.DTOs.Ftp;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public class FileServerRepository : IFileServerRepository
	{
        private readonly ISqlDataAccess _db;

        public FileServerRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All FileServers
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileServerEntity>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<FileServerEntity, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetAll", new { });
        }

		public async Task<List<ClientExternalOrderFTPSetupDto>> GetAllClientFtp()
		{
			return await _db.LoadDataUsingProcedure<ClientExternalOrderFTPSetupDto, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetAll", new { });
		}


        public async Task<List<ClientExternalOrderFTPSetupDto>> GetEnabledClientStorage(int companyId)
        {
            var query = $"SELECT * FROM [dbo].[Client_ClientOrderFtp] AS cc WHERE cc.IsEnable = 1 " +
                $"AND cc.ClientCompanyId = {companyId} AND (cc.IsInternalFtp IS NULL OR cc.IsInternalFtp = 0)";
            return await GetAllClientFtpByQuery(query);
        }
        public async Task<List<ClientExternalOrderFTPSetupDto>> GetEnabledInternalStorage(int companyId)
        {
            var query = $"SELECT * FROM [dbo].[Client_ClientOrderFtp] AS cc WHERE cc.IsEnable = 1 " +
                $"AND cc.ClientCompanyId = {companyId} AND cc.IsInternalFtp=1";
            return await GetAllClientFtpByQuery(query);
        }


        public async Task<List<ClientExternalOrderFTPSetupDto>> GetAllClientFtpByQuery(string query)
		{
			var filteredList = await _db.LoadDataUsingQuery<ClientExternalOrderFTPSetupDto, dynamic>(query,
					new
					{
						
						
					});


			return filteredList;
		}

		public async Task<ClientExternalOrderFTPSetupDto> GetClientFtpByQuery(string query)
		{
			var filteredList = await _db.LoadDataUsingQuery<ClientExternalOrderFTPSetupDto, dynamic>(query,
					new
					{


					});


			return filteredList.FirstOrDefault();
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
		public async Task<FileServerEntity> GetById(int fileServerId)
        {
            var result = await _db.LoadDataUsingProcedure<FileServerEntity, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetById", new { FileServerId = fileServerId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get Default Page
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServerEntity> GetDefaultFileServer()
        {
            var query = "SELECT * FROM Common_FileServer WHERE IsDefault = 1 AND Status = 1";
            var result = await _db.LoadDataUsingQuery<FileServerEntity, dynamic>(query, new {});
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServerEntity> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<FileServerEntity, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert fileServer
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(FileServerEntity fileServer)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_Insert", new
                {
                    fileServer.FileServerType,
                    fileServer.Name,
                    fileServer.UserName,
                    fileServer.Password,
                    fileServer.AccessKey,
                    fileServer.SecretKey,
                    fileServer.RootFolder,
                    fileServer.SshKeyPath,
                    fileServer.Host,
                    fileServer.Protocol,
                    fileServer.Status,
                    fileServer.CreatedByContactId,
                    fileServer.ObjectId,
                    fileServer.IsDefault,
                    fileServer.SubFolder
                    
                });

                fileServer.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update FileServer
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(FileServerEntity fileServer)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_FileServer_Update", new
                {
                    fileServer.Id,
                    fileServer.FileServerType,
                    fileServer.Name,
                    fileServer.UserName,
                    fileServer.Password,
                    fileServer.AccessKey,
                    fileServer.SecretKey,
                    fileServer.RootFolder,
                    fileServer.SshKeyPath,
                    fileServer.Host,
                    fileServer.Protocol,
                    fileServer.Status,
                    fileServer.UpdatedByContactId,
                    fileServer.IsDefault,
                    fileServer.SubFolder
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete FileServer by id
        /// </summary>
        /// <param name="fileServerId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_FileServer_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

    }
}
