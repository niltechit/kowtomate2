using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;

namespace CutOutWiz.Services.ClientCommonCategoryService.ClientCategoryServices
{
    public interface IClientCategoryServiceService
    {
        Task<Response<int>> Insert(ClientCategoryServiceModel categoryService);
        Task<Response<bool>> Update(ClientCategoryServiceModel categoryService);
        Task<Response<bool>> Delete(int Id, int clientCategoryId);
        Task<Response<bool>> Delete(int Id);
        Task<CutOutWiz.Services.Models.ClientCategoryServices.ClientCategoryServiceModel> GetById(int Id);
        Task<List<ClientCategoryServiceModel>> GetAll();
        Task<ClientCategoryServiceModel> GetByClientCategoryIdAndServiceId(int ServiceId, int CategoryId);
        Task<List<CommonServiceModel>> GetCommonServiceByClientCategoryId(int CategoryId);
    }
}

