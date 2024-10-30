using CutOutWiz.Data;
using CutOutWiz.Data.Common;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Common
{
    public class FileServerService : IFileServerService
    {
        private readonly ISqlDataAccess _db;

        public FileServerService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All FileServers
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileServer>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<FileServer, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetAll", new { });
        }

        /// <summary>
        /// Get fileServer by fileServer Id
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServer> GetById(int fileServerId)
        {
            var result = await _db.LoadDataUsingProcedure<FileServer, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetById", new { FileServerId = fileServerId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<FileServer> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<FileServer, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert fileServer
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(FileServer fileServer)
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
                    fileServer.IsDefault
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
        public async Task<Response<bool>> Update(FileServer fileServer)
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
                    fileServer.IsDefault
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
