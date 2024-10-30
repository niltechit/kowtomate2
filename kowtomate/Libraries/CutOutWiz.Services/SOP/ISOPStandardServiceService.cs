using CutOutWiz.Core;
using CutOutWiz.Services.Models.SOP;

namespace CutOutWiz.Services.SOP
{
    public interface IOrderSOPStandardServiceService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<SOPStandardServiceModel>> GetAll();
        Task<List<SOPStandardServiceModel>> GetListByTemplateId(int templateId);
        Task<SOPStandardServiceModel> GetById(int standardServiceId);
        Task<SOPStandardServiceModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(SOPStandardServiceModel standardService);
        Task<Response<bool>> Update(SOPStandardServiceModel standardService);
        Task<List<SOPTemplateServiceModel>> GetTemplateServiceByTemplateId(int templateId);
    }
}
