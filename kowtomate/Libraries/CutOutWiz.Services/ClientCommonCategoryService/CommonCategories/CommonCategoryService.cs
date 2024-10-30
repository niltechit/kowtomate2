using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.DbAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.ClientCommonCategoryService.CommonCategories
{
    public class CommonCategoryService : ICommonCategoryService
    {
        private readonly ISqlDataAccess _db;

        public CommonCategoryService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<bool>> Delete(int Id)
        {
            var response = new Core.Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Category_DeleteById", new
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
        public async Task<Response<bool>> DeleteCategoryService(int ServiceId,int CategoryId)
        {
            var response = new Core.Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_CategoryService_DeleteByServiceIdAndCategoryId", new
                {
                    ServiceId = ServiceId,
                    CategoryId = CategoryId,
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
        public async Task<Response<CutOutWiz.Services.Models.ClientCategoryServices.CommonCategoryServiceModel>> GetCommonCategoryServiceByServiceIdAndCategoryId(int ServiceId,int CategoryId)
        {
            var response = new Core.Response<CutOutWiz.Services.Models.ClientCategoryServices.CommonCategoryServiceModel>();
            try
            {
                var result = await _db.LoadDataUsingProcedure<CutOutWiz.Services.Models.ClientCategoryServices.CommonCategoryServiceModel, dynamic>(storedProcedure: "dbo.sp_common_categoryService_getServiceIdAndCategoryId", new
                {
                    ServiceId = ServiceId,
                    CategoryId = CategoryId,
                });
                response.Result = result.FirstOrDefault();
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<List<CommonCategoryModel>> GetAll()
        {
           return await _db.LoadDataUsingProcedure<CommonCategoryModel, dynamic>(storedProcedure: "dbo.SP_Common_Category_GetAll", new { });

        }


        public async Task<CommonCategoryModel> GetById(int Id)
        {
            var result = await _db.LoadDataUsingProcedure<CommonCategoryModel, dynamic>(storedProcedure: "dbo.SP_Common_Category_GetById", new { Id = Id });
            return result.FirstOrDefault();
        }

        public async Task<List<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel>> GetCommonServices(int categoryId)
        {
            var result = await _db.LoadDataUsingProcedure<CutOutWiz.Services.Models.ClientCategoryServices.CommonServiceModel, dynamic>(storedProcedure: "dbo.sp_common_category_getServices_by_CategoryId", new { categoryId = categoryId });
            return result.ToList();
        }
        public async Task<List<CutOutWiz.Services.Models.ClientCategoryServices.CommonCategoryServiceModel>> GetCommonCategoryServices(int categoryId)
        {
            var result = await _db.LoadDataUsingProcedure<CutOutWiz.Services.Models.ClientCategoryServices.CommonCategoryServiceModel, dynamic>(storedProcedure: "dbo.sp_common_category_getServices_by_CategoryId", new { categoryId = categoryId });
            return result.Where(x => x.IsActive == true).ToList();
        }

        public async Task<Response<int>> Insert(CommonCategoryModel commonCategory)
        {

            var response = new Core.Response<int>();

            commonCategory.CreatedDate = DateTime.Now;
            commonCategory.UpdatedDate = DateTime.Now;

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_Category_Insert", new
                {
                    commonCategory.Name,
                    commonCategory.TimeInMinutes,
                    commonCategory.PriceInUSD,
                    commonCategory.IsActive,
                    commonCategory.CreatedDate,
                    commonCategory.CreatedByUsername,
                    commonCategory.UpdatedDate,
                    commonCategory.UpdatedByUsername,
                });

                commonCategory.Id = newId;
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

        public async Task<Response<bool>> Update(CommonCategoryModel commonCategory)
        {
            var response = new Core.Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Category_Update", new
                {
                    commonCategory.Id,
                    commonCategory.Name,
                    commonCategory.TimeInMinutes,
                    commonCategory.PriceInUSD,
                    commonCategory.IsActive,
                    commonCategory.UpdatedDate,
                    commonCategory.UpdatedByUsername,
                    commonCategory.IsDeleted,
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
    }
}
