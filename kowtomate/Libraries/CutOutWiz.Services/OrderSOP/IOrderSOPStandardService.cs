using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.SOP;

namespace CutOutWiz.Services.OrderSOP
{
    public interface IOrderSOPStandardService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<OrderSOPStandardServiceModel>> GetAll();
        Task<List<OrderSOPStandardServiceModel>> GetListByOrderTemplateId(int templateId);
        Task<OrderSOPStandardService> GetById(int standardServiceId);
        Task<SOPStandardServiceModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(OrderSOPStandardServiceModel orderStandardService);
        Task<Response<bool>> Update(SOPStandardServiceModel standardService);
        Task<List<SOPTemplateServiceModel>> GetTemplateServiceByTemplateId(int templateId);
		Task<OrderSOPStandardServiceModel> GetByOrderSOPName(string orderSOPName);
	}
}
