using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;

namespace CutOutWiz.Services.ClientCommonCategoryService.ClientCategorys
{
    public interface IClientCategoryService
    {
        Task<Response<bool>> Delete(int Id);
        Task<List<ClientCategoryModel>> GetAll();
        Task<List<ClientCategoryServiceModel>> GetAllCategoryService(int categoryId);
        Task<List<ClientCategoryModel>> GetByCompanyId(int companyId);
        Task<ClientCategoryModel> GetById(int Id);
        Task<Response<int>> Insert(ClientCategoryModel common);
        Task<Response<bool>> Update(ClientCategoryModel common);
    }
}