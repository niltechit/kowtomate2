using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.ClientCommonCategoryService.ClientCategoryServices
{
    public class ClientCategoryServiceService : IClientCategoryServiceService
    {
        private readonly ISqlDataAccess _db;

        public ClientCategoryServiceService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<bool>> Delete(int Id)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_CategoryService_DeleteById", new
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
        public async Task<Response<bool>> Delete(int Id, int clientCategoryId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_CategoryService_DeleteByServiceIdAndClientCategoryId", new
                {
                    Id = Id,
                    clientCategoryId = clientCategoryId,
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
        public async Task<List<ClientCategoryServiceModel>> GetAll()
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Client_CategoryService_GetAll", new { });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
        }

        public async Task<ClientCategoryServiceModel> GetById(int Id)
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Client_CategoryService_GetById", new { Id = Id });
            return result.Where(x => x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
        }
        public async Task<ClientCategoryServiceModel> GetByClientCategoryIdAndServiceId(int ServiceId, int CategoryId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientCategoryServiceModel, dynamic>(storedProcedure: "dbo.SP_Client_CategoryService_GetByServiceIdAndCategoryId",
                new
                {
                    ServiceId = ServiceId,
                    CategoryId = CategoryId
                });
            return result.FirstOrDefault();
        }

        public async Task<Response<int>> Insert(ClientCategoryServiceModel categoryService)
        {
            var response = new Response<int>();

            categoryService.CreatedDate = DateTime.Now;

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Client_CategoryService_Insert", new
                {
                    categoryService.TimeInMinutes,
                    categoryService.PriceInUSD,
                    categoryService.ClientCategoryId,
                    categoryService.CommonServiceId,
                    categoryService.CreatedDate,
                    categoryService.CreatedByUsername,
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

        public async Task<Response<bool>> Update(ClientCategoryServiceModel categoryService)
        {
            var response = new Response<bool>();
            categoryService.UpdatedDate = DateTime.Now;
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Client_CategoryService_Update", new
                {
                    categoryService.Id,
                    categoryService.TimeInMinutes,
                    categoryService.PriceInUSD,
                    categoryService.ClientCategoryId,
                    categoryService.CommonServiceId,
                    categoryService.UpdatedByUsername,
                    categoryService.UpdatedDate,
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

        public async Task<List<CommonServiceModel>> GetCommonServiceByClientCategoryId(int CategoryId)
        {
            var result = await _db.LoadDataUsingProcedure<CommonServiceModel, dynamic>(storedProcedure: "dbo.sp_clientCategoryService_getByCategoryId", new { CategoryId = CategoryId });
            return result.ToList();
        }
    }
}
