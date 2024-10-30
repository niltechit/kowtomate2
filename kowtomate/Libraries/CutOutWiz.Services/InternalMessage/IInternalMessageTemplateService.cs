using CutOutWiz.Core;
using CutOutWiz.Core.Message;

namespace CutOutWiz.Services.InternalMessage
{
    public interface IInternalMessageTemplateService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<InternalMessageTemplate>> GetAll();
        Task<InternalMessageTemplate> GetById(int templateId);
        Task<InternalMessageTemplate> GetByObjectId(string objectId);
        // <summary>
        /// Get template by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        Task<InternalMessageTemplate> GetByAccessCode(string accessCode);
        Task<Response<int>> Insert(InternalMessageTemplate template);
        Task<Response<bool>> Update(InternalMessageTemplate template);
    }
}
