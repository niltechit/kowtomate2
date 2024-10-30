using CutOutWiz.Data;
using CutOutWiz.Data.SOP;

namespace CutOutWiz.Services.SOP
{
    public interface ISOPTemplateService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<SOPTemplate>> GetAll();
        Task<SOPTemplate> GetById(int templateId);
        Task<SOPTemplate> GetByObjectId(string objectId);
        Task<Response<int>> Insert(SOPTemplate template);
        Task<Response<bool>> Update(SOPTemplate template);
    }
}
