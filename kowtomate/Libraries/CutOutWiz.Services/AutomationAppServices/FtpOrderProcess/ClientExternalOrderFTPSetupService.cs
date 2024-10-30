using CutOutWiz.Core;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public class ClientExternalOrderFTPSetupService : IClientExternalOrderFTPSetupService
    {
        private readonly ISqlDataAccess _db;

        public ClientExternalOrderFTPSetupService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Enabled FTps
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClientExternalOrderFTPSetupModel>> GetAllEnabledFtps()
        {
            return await _db.LoadDataUsingProcedure<ClientExternalOrderFTPSetupModel, dynamic>(storedProcedure: "dbo.SP_Client_ExternalOrderFTPSetup_GetEnabledFtps", new { });
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="ClientExternalOrderFTPSetupId"></param>
        /// <returns></returns>
        public async Task<ClientExternalOrderFTPSetupModel> GetByClientCompanyId(int copanyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientExternalOrderFTPSetupModel, dynamic>(storedProcedure: "dbo.SP_Client_ExternalOrderFTPSetup_GetByObjectId", new { ClientCompanyId = copanyId });
            return result.FirstOrDefault();
        }

        public async Task<ClientExternalOrderFTPSetupModel> GetById(int id)
        {
            var query = "Select * FROM Client_ClientOrderFtp WHERE Id = @Id";
            var result = await _db.LoadFirstOrDefaultDataUsingQuery<ClientExternalOrderFTPSetupModel, dynamic>(query, new { Id = id });
            return result;
        }

        /// <summary>
        /// Insert fileServer
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(ClientExternalOrderFTPSetupModel fileServer)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Client_ExternalOrderFTPSetup_Insert", new
                {
                    //fileServer.ClientExternalOrderFTPSetupType,
                    //fileServer.Name,
                    //fileServer.UserName,
                    //fileServer.Password,
                    //fileServer.AccessKey,
                    //fileServer.SecretKey,
                    //fileServer.RootFolder,
                    //fileServer.SshKeyPath,
                    //fileServer.Host,
                    //fileServer.Protocol,
                    //fileServer.Status,
                    //fileServer.CreatedByContactId,
                    //fileServer.ObjectId,
                    //fileServer.IsDefault
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
        /// Update ClientExternalOrderFTPSetup
        /// </summary>
        /// <param name="fileServer"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(ClientExternalOrderFTPSetupModel fileServer)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_ExternalOrderFTPSetup_Update", new
                {
                    fileServer.Id,
                    //fileServer.ClientExternalOrderFTPSetupType,
                    //fileServer.Name,
                    //fileServer.UserName,
                    //fileServer.Password,
                    //fileServer.AccessKey,
                    //fileServer.SecretKey,
                    //fileServer.RootFolder,
                    //fileServer.SshKeyPath,
                    //fileServer.Host,
                    //fileServer.Protocol,
                    //fileServer.Status,
                    //fileServer.UpdatedByContactId,
                    //fileServer.IsDefault
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
        /// Delete ClientExternalOrderFTPSetup by id
        /// </summary>
        /// <param name="fileServerId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(int companyId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_ExternalOrderFTPSetup_Delete", new { ClientCompanyId = companyId });
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
