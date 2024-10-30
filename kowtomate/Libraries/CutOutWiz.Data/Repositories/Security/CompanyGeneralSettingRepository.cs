using CutOutWiz.Core;
using CutOutWiz.Data;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.Entities.Security;

namespace CutOutWiz.Data.Repositories.Security
{
    public class CompanyGeneralSettingRepository : ICompanyGeneralSettingRepository
	{
        private readonly ISqlDataAccess _db;

        public CompanyGeneralSettingRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<bool>> Delete(int Id)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_CompanyGeneralSettings_DeleteCompanyGeneralSettingsById", new { Id = Id });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        
        public async Task<CompanyGeneralSettingEntity> GetGeneralSettingById(int Id)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyGeneralSettingEntity, dynamic>(storedProcedure: "dbo.SP_CompanyGeneralSettings_GetCompanyGeneralSettingsById", new { Id = Id });
            return result.FirstOrDefault();
        }

		public async Task<CompanyGeneralSettingEntity> GetGeneralSettingByCompanyId(int companyId)
		{
			var result = await _db.LoadDataUsingProcedure<CompanyGeneralSettingEntity, dynamic>(storedProcedure: "dbo.SP_CompanyGeneralSettings_GetCompanyGeneralSettingsByCompanyId", new { companyId = companyId });
			return result.FirstOrDefault();
		}

        public async Task<CompanyGeneralSettingEntity> GetAllGeneralSettingsByCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyGeneralSettingEntity, dynamic>(storedProcedure: "dbo.SP_CompanyGeneralSettings_GetCompanyGeneralSettingsByCompanyId", new { CompanyId = companyId });
            return result.FirstOrDefault();
        }

        public async Task<Response<int>> Insert(CompanyGeneralSettingEntity generalSetting)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_CompanyGeneralSettings_Insert", new
                {
                    generalSetting.AutoAssignOrderToTeam,
                    generalSetting.AutoDistributeToEditor,
                    generalSetting.AutoQcPass,
                    generalSetting.AutoOperationPass,
                    generalSetting.AllowPartialDelivery,
                    generalSetting.EnableFtpOrderPlacement,
                    generalSetting.EnableOrderDeliveryToFtp,
                    generalSetting.CompanyId,
                    generalSetting.AllowSingleOrderFromFTP,
					generalSetting.EnableSingleOrderPlacement,
					generalSetting.AllowMaxNumOfItemsPerOrder,
                    generalSetting.IsIbrProcessedEnabled,
                    generalSetting.AllowAutoUploadFromEditorPc,
                    generalSetting.AllowAutoUploadFromQcPc,
                    generalSetting.AllowAutoDownloadToEditorPc,
                    generalSetting.AllowClientWiseImageProcessing,
                    generalSetting.AllowNotifyOpsOnImageArrivalFTP,
                    generalSetting.CheckEmailForUploadCompletedConfirmation,
                    generalSetting.CheckUploadCompletedFlagOnFile,
                    generalSetting.FtpOrderPlacedAppNo,
                    generalSetting.IsBatchRootFolderNameAddWithOrder,
                    generalSetting.isFtpFolderPreviousStructureWiseStayInFtp,
                    generalSetting.IsSameNameImageExistOnSameFolder,
                    generalSetting.AllowExtraFile,
                    generalSetting.DeliveryType,
                    generalSetting.OrderPlaceBatchMoveType,
                    generalSetting.RemoveFacilityNameFromOutputRootFolderPath,
                    generalSetting.CheckUploadCompletedFlagOnBatchName,
                    generalSetting.CompletedFlagKeyNameOnBatch,
                    generalSetting.IsFtpIdleTimeChecking,
                    generalSetting.FtpIdleTime,
                    generalSetting.FtpFileMovedPathAfterOrderCreated,
                    generalSetting.IsOrderCreatedThenFileMove,
                    generalSetting.IsSendClientHotkey,
                    generalSetting.HotkeyFlagFileName,
                    generalSetting.AllowSingleOrderForRootAllFolderAndFiles,
                    generalSetting.IsOrderPlacedEmailSentToCompany,
                    generalSetting.IsOrderPlacedEmailSentToCompanyAllUsers,
                });

                generalSetting.Id = newId;
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

        public async Task<Response<bool>> Update(CompanyGeneralSettingEntity generalSetting)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_CompanyGeneralSettings_Update", new
                {
                    generalSetting.Id,
                    generalSetting.AutoAssignOrderToTeam,
                    generalSetting.AutoDistributeToEditor,
                    generalSetting.AutoQcPass,
                    generalSetting.AutoOperationPass,
                    generalSetting.AllowPartialDelivery,
                    generalSetting.EnableFtpOrderPlacement,
                    generalSetting.EnableOrderDeliveryToFtp,
                    generalSetting.CompanyId,
                    generalSetting.AllowSingleOrderFromFTP,
                    generalSetting.EnableSingleOrderPlacement,
                    generalSetting.AllowMaxNumOfItemsPerOrder,
                    generalSetting.IsIbrProcessedEnabled,
                    generalSetting.AllowAutoUploadFromEditorPc,
                    generalSetting.AllowAutoUploadFromQcPc,
                    generalSetting.AllowAutoDownloadToEditorPc,
                    generalSetting.AllowClientWiseImageProcessing,
                    generalSetting.AllowNotifyOpsOnImageArrivalFTP,
                    generalSetting.CheckEmailForUploadCompletedConfirmation,
                    generalSetting.CheckUploadCompletedFlagOnFile,
                    generalSetting.FtpOrderPlacedAppNo,
					generalSetting.IsBatchRootFolderNameAddWithOrder,
                    generalSetting.isFtpFolderPreviousStructureWiseStayInFtp,
                    generalSetting.IsSameNameImageExistOnSameFolder,
                    generalSetting.AllowExtraFile,
                    generalSetting.DeliveryType,
                    generalSetting.OrderPlaceBatchMoveType,
                    generalSetting.RemoveFacilityNameFromOutputRootFolderPath,
					generalSetting.CheckUploadCompletedFlagOnBatchName,
					generalSetting.CompletedFlagKeyNameOnBatch,
                    generalSetting.IsFtpIdleTimeChecking,
                    generalSetting.FtpIdleTime,
                    generalSetting.FtpFileMovedPathAfterOrderCreated,
                    generalSetting.IsOrderCreatedThenFileMove,
                    generalSetting.IsSendClientHotkey,
                    generalSetting.HotkeyFlagFileName,
                    generalSetting.AllowSingleOrderForRootAllFolderAndFiles,
					generalSetting.IsOrderPlacedEmailSentToCompany,
					generalSetting.IsOrderPlacedEmailSentToCompanyAllUsers,
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

        public async Task<List<CompanyGeneralSettingEntity>> GetAllCompanyGeneralSettingsByQuery(string query)
        {
            var filteredList = await _db.LoadDataUsingQuery<CompanyGeneralSettingEntity, dynamic>(query,
                    new
                    {
                    });
            return filteredList;
        }
    }
}
