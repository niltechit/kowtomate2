using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.ClientCommonCategoryService.ClientCategorys
{
    public class ClientCategoryService : IClientCategoryService
    {
        private readonly ISqlDataAccess _db;

        public ClientCategoryService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<bool>> Delete(int Id)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_Category_DeleteById", new
                {
                    Id = Id,
                    IsActive = 0,
                    IsDeleted = 1,
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

        public async Task<List<ClientCategoryModel>> GetAll()
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryModel, dynamic>(storedProcedure: "dbo.SP_Client_Category_GetAll", new { });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
        }

        public async Task<ClientCategoryModel> GetById(int Id)
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryModel, dynamic>(storedProcedure: "dbo.SP_Client_Category_GetById", new { Id = Id });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
        }

        public async Task<List<ClientCategoryModel>> GetByCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryModel, dynamic>(storedProcedure: "dbo.SP_Client_Category_GetByCompanyId", new { companyId = companyId });

            if (result == null)
            {
                result = new List<ClientCategoryModel>();
            }

            return result;
        }

        public async Task<Response<int>> Insert(ClientCategoryModel common)
        {
            var response = new Response<int>();
            common.CreatedDate = DateTime.Now;
            common.UpdatedDate = DateTime.Now;
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Client_Category_Insert", new
                {
                    common.ClientCompanyId,
                    common.CommonCategoryId,
                    common.TimeInMinutes,
                    common.PriceInUSD,
                    common.IsActive,
                    common.CreatedDate,
                    common.CreatedByUsername,
                    common.IsDeleted,
                });

                common.Id = newId;
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

        public async Task<Response<bool>> Update(ClientCategoryModel common)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_Category_Update", new
                {
                    common.Id,
                    common.ClientCompanyId,
                    common.CommonCategoryId,
                    common.TimeInMinutes,
                    common.PriceInUSD,
                    common.IsActive,
                    common.IsDeleted,
                    common.UpdatedDate,
                    common.UpdatedByUsername,
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

        public async Task<List<ClientCategoryServiceModel>> GetAllCategoryService(int categoryId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryServiceModel, dynamic>(storedProcedure: "dbo.sp_client_category_getCategoryServicesByCategoryId", new { categoryId = categoryId });
            return result.ToList();
        }
    }
}

