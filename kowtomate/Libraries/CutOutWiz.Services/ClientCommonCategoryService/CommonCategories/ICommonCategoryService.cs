using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;

namespace CutOutWiz.Services.ClientCommonCategoryService.CommonCategories
{
    public interface ICommonCategoryService
    {
        Task<Response<int>> Insert(CommonCategoryModel commonCategory);
        Task<Response<bool>> Update(CommonCategoryModel commonCategory);
        Task<Response<bool>> Delete(int Id);
        Task<CommonCategoryModel> GetById(int Id);
        Task<List<CommonCategoryModel>> GetAll();
        Task<List<CommonServiceModel>> GetCommonServices(int categoryId);
        Task<List<CommonCategoryServiceModel>> GetCommonCategoryServices(int categoryId);
        Task<Response<bool>> DeleteCategoryService(int ServiceId, int CategoryId);
        Task<Response<CommonCategoryServiceModel>> GetCommonCategoryServiceByServiceIdAndCategoryId(int ServiceId, int CategoryId);
    }
}
