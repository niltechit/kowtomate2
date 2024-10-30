using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.AutomationAppServices.FtpOrderProcess
{
    public class ClientOrderFtpService : IClientOrderFtpService
    {
        private readonly ISqlDataAccess _db;

        public ClientOrderFtpService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<bool>> Delete(int Id)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_ClientOrderFtp_Delete_byId", new { Id });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<List<ClientOrderFtpModel>> GetAllEnabledClientOrderFtps()
        {
            return await _db.LoadDataUsingProcedure<ClientOrderFtpModel, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetAllEnable_ClientOrderFtps", new { });
        }
        public async Task<List<ClientOrderFtpModel>> GetAllClientOrderFtps()
        {
            return await _db.LoadDataUsingProcedure<ClientOrderFtpModel, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetAllEnable_ClientOrderFtps", new { });
        }

        public async Task<ClientOrderFtpModel> GetByClientOrderFtpsCompanyId(int copanyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderFtpModel, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId", new { ClientCompanyId = copanyId });
            return result.FirstOrDefault();
        }
        public async Task<List<ClientOrderFtpModel>> GetClientOrderFtpsListByCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderFtpModel, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId", new { ClientCompanyId = companyId });
            return result.ToList();
        }
        public async Task<List<ClientOrderFtpModel>> GetClientDestinationFtpsCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderFtpModel, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyIdAndSentOutputistrue", new { ClientCompanyId = companyId });
            return result.ToList();
        }

        public async Task<Response<int>> Insert(ClientOrderFtpModel fileServer)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_Insert", new
                {
                    fileServer.ClientCompanyId,
                    fileServer.Host,
                    fileServer.Port,
                    fileServer.Username,
                    fileServer.Password,
                    fileServer.IsEnable,
                    fileServer.OutputRootFolder,
                    fileServer.InputRootFolder,
                    fileServer.SentOutputToSeparateFTP,
                    fileServer.OutputHost,
                    fileServer.OutputUsername,
                    fileServer.OutputPassword,
                    fileServer.OutputPort,
                    fileServer.OutputFolderName,
                    fileServer.IsTemporaryDeliveryUploading,
                    fileServer.TemporaryDeliveryUploadFolder,
                    fileServer.IsDefault,
                    fileServer.OutputProtocolTypePuttyKeyPath,
                    fileServer.InputProtocolTypePuttyKeyPath,
                    fileServer.InputProtocolType,
                    fileServer.OutputProtocolType,
                    fileServer.InputPassPhrase,
                    fileServer.OutputPassPhrase,
                    fileServer.DeliveryDeadlineInMinute,
					fileServer.IsInternalFtp,
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

        public async Task<Response<bool>> Update(ClientOrderFtpModel fileServer)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_ClientOrderFtp_Update", new
                {
                    fileServer.Id,
                    fileServer.ClientCompanyId,
                    fileServer.Host,
                    fileServer.Port,
                    fileServer.Username,
                    fileServer.Password,
                    fileServer.IsEnable,
                    fileServer.OutputRootFolder,
                    fileServer.InputRootFolder,
                    fileServer.SentOutputToSeparateFTP,
                    fileServer.OutputHost,
                    fileServer.OutputUsername,
                    fileServer.OutputPassword,
                    fileServer.OutputPort,
                    fileServer.OutputFolderName,
                    fileServer.IsTemporaryDeliveryUploading,
                    fileServer.TemporaryDeliveryUploadFolder,
                    fileServer.IsDefault,
                    fileServer.OutputProtocolTypePuttyKeyPath,
                    fileServer.InputProtocolTypePuttyKeyPath,
                    fileServer.InputProtocolType,
                    fileServer.OutputProtocolType,
                    fileServer.InputPassPhrase,
                    fileServer.OutputPassPhrase,
                    fileServer.DeliveryDeadlineInMinute,
					fileServer.IsInternalFtp,
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

        public async Task<Response<bool>> UpdateIsEnableTrueForLocalFtp(int ClientCompanyId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_Client_OrderFtps_Enable_LocalFtps", new
                {
					ClientCompanyId = ClientCompanyId,
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
         public async Task<Response<bool>> UpdateIsEnableFalseForLocalFtp(int ClientCompanyId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_Client_OrderFtps_Enable_LocalFtpsFalse", new
                {
					ClientCompanyId = ClientCompanyId,
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

        public async Task<Response<List<ClientOrderFtpModel>>> GetAllInternalFtp(int? companyId = null)
        {

            var response = new Response<List<ClientOrderFtpModel>>();

            try
            {
                var result = await _db.LoadDataUsingProcedure<ClientOrderFtpModel, dynamic>(storedProcedure: "dbo.SP_Client_ClientOrderFtp_GetAll_Internal_Ftps", new 
                { 
                    companyId = companyId 
                });

                response.Result = result.ToList();
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
                response.IsSuccess = false;
                response.Result = new List<ClientOrderFtpModel>();
            }

            return response;
        }
    }
}
