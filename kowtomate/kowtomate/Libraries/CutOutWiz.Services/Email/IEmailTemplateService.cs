using CutOutWiz.Data;
using CutOutWiz.Data.Email;

namespace CutOutWiz.Services.Email
{
    public interface IEmailTemplateService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<EmailTemplate>> GetAll();
        Task<EmailTemplate> GetById(int templateId);
        Task<EmailTemplate> GetByObjectId(string objectId);
        // <summary>
        /// Get template by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        Task<EmailTemplate> GetByAccessCode(string accessCode);
        Task<Response<int>> Insert(EmailTemplate template);
        Task<Response<bool>> Update(EmailTemplate template);
    }
}
