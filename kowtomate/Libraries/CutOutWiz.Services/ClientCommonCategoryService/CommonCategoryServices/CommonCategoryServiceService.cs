using CutOutWiz.Core;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Data;

namespace CutOutWiz.Services.ClientCommonCategoryService.CommonCategoryServices
{
    public class CommonCategoryServiceService : ICommonCategoryServiceService
    {
        private readonly ISqlDataAccess _db;

        public CommonCategoryServiceService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<bool>> Delete(int Id)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_CategoryService_DeleteById", new
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

        public async Task<List<CommonCategoryServiceModel>> GetAll()
        {
            var result = await _db.LoadDataUsingProcedure<CommonCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Common_CategoryService_GetAll", new { });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
        }

        public async Task<CommonCategoryServiceModel> GetById(int Id)
        {
            var result = await _db.LoadDataUsingProcedure<CommonCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Common_CategoryService_GetById", new { Id = Id });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
        }

        public async Task<CommonCategoryServiceModel> GetByServiceIdAndCategoryId(int ServiceId, int CategoryId)
        {
            var result = await _db.LoadDataUsingProcedure<CommonCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Common_CategoryService_GetByServiceIdAndCategoryId", new
            {
                ServiceId = ServiceId,
                CategoryId = CategoryId
            });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
        }

        public async Task<Response<int>> Insert(CommonCategoryServiceModel categoryService)
        {

            var response = new Response<int>();

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_CategoryService_Insert", new
                {
                    categoryService.TimeInMinutes,
                    categoryService.PriceInUSD,
                    categoryService.CommonCategoryId,
                    categoryService.CommonServiceId,
                    categoryService.IsDeleted,
                    categoryService.IsActive,
                });

                categoryService.Id = newId;
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

        public async Task<Response<bool>> Update(CommonCategoryServiceModel categoryService)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_CategoryService_Update", new
                {
                    categoryService.Id,
                    categoryService.TimeInMinutes,
                    categoryService.PriceInUSD,
                    categoryService.CommonCategoryId,
                    categoryService.CommonServiceId,
                    categoryService.IsDeleted,
                    categoryService.IsActive,
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
        public async Task<List<CommonCategoryServiceModel>> GetCommonCategoryServicesByCategoryId(int categoryId)
        {
            return await _db.LoadDataUsingProcedure<CommonCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Common_Category_Service_GetAll_by_CategoryId", new { categoryId = categoryId });

        }
    }
}
