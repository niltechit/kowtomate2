using CutOutWiz.Core;
using CutOutWiz.Services.Models.SOP;

namespace CutOutWiz.Services.SOP
{
    public interface IOrderSOPTemplateService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<SOPTemplateModel>> GetAll();
        Task<List<SOPTemplateModel>> GetAllById(int templateId);
        Task<List<SOPTemplateModel>> GetAllByCompany(int companyId);
        Task<List<SOPTemplateModel>> GetAllPendingSopByCompany(int companyId);
        Task<SOPTemplateModel> GetById(int templateId);
        Task<SOPTemplateModel> GetByObjectId(string objectId);
        Task<SOPTemplateViewModel> GetByObjectID(string objectId);
        Task<Response<int>> Insert(SOPTemplateModel template);
        Task<Response<bool>> Update(SOPTemplateModel template);
        Task<List<SOPTemplateFile>> GetSopTemplateFilesBySopTemplateId(int SOPTemplateId);
        Task<List<SOPTemplateFile>> GetSopTemplateFilesByTemplateId(int SOPTemplateId);
        Task<SOPTemplateFile> GetSopTemplateFilesById(int fileId);
        Task<Response<bool>> UpdateTemplateFile(string objectId);
    }
}
