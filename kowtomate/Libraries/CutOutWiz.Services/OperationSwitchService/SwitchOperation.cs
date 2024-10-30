using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.OperationSwitchService
{
	public class SwitchOperation : ISwitchOperation
	{
		private readonly ISqlDataAccess _db;

		public SwitchOperation(ISqlDataAccess db)
		{
			_db = db;
		}
		public Task<Response<bool>> Delete(string objectId)
		{
			throw new NotImplementedException();
		}

		public async Task<List<SecuritySwhiechOperation>> GetAll<SecuritySwhiechOperation>() where SecuritySwhiechOperation : class
		{
            return await _db.LoadDataUsingProcedure<SecuritySwhiechOperation, dynamic>(storedProcedure: "dbo.sp_Security_SwitchOperation_GetAll", new { });
        }

		public async Task<SecuritySwhiechOperationModel> GetById(int companyId)
		{

			var result = await _db.LoadDataUsingProcedure<SecuritySwhiechOperationModel, dynamic>(storedProcedure: "dbo.sp_Security_SwitchOperation_getById", new { ClientCompanyId = companyId });
			return result.FirstOrDefault();
		}

		public Task<SecuritySwhiechOperationModel> GetByObjectId(string objectId)
		{
			throw new NotImplementedException();
		}

		public async Task<Response<int>> Insert(SecuritySwhiechOperationModel entity)
		{
			var response = new Core.Response<int>();
			try
			{
				var newId = await _db.SaveDataUsingProcedureWithGeneric<SecuritySwhiechOperationModel>(storedProcedure: "dbo.sp_Security_SwitchOperation_Insert", entity, "default");

				entity.Id = newId;
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

		public async Task<Response<bool>> Update(SecuritySwhiechOperationModel entity)
		{
			var response = new Core.Response<bool>();
			try
			{
				var newId = await _db.UpdateDataUsingProcedureWithGeneric<SecuritySwhiechOperationModel>(storedProcedure: "dbo.sp_Security_SwitchOperation_Insert", entity, "default");

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
	}
}
