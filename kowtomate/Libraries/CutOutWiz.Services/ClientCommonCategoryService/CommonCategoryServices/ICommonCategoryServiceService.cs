using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;

namespace CutOutWiz.Services.ClientCommonCategoryService.CommonCategoryServices
{
    public interface ICommonCategoryServiceService
    {
        Task<Response<int>> Insert(CommonCategoryServiceModel categoryService);
        Task<Response<bool>> Update(CommonCategoryServiceModel categoryService);
        Task<Response<bool>> Delete(int Id);
        Task<CommonCategoryServiceModel> GetById(int Id);
        Task<List<CommonCategoryServiceModel>> GetAll();
        Task<CommonCategoryServiceModel> GetByServiceIdAndCategoryId(int ServiceId, int CategoryId);
        /// <summary>
        /// Get All Common Category Service By Category Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<List<CommonCategoryServiceModel>> GetCommonCategoryServicesByCategoryId(int categoryId);
    }
}
