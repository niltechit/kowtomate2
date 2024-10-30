using CutOutWiz.Core;
using CutOutWiz.Services.Models.Email;

namespace CutOutWiz.Services.Email
{
    public interface IEmailTemplateService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<EmailTemplateModel>> GetAll();
        Task<EmailTemplateModel> GetById(int templateId);
        Task<EmailTemplateModel> GetByObjectId(string objectId);
        // <summary>
        /// Get template by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        Task<EmailTemplateModel> GetByAccessCode(string accessCode);
        Task<Response<int>> Insert(EmailTemplateModel template);
        Task<Response<bool>> Update(EmailTemplateModel template);
    }
}
