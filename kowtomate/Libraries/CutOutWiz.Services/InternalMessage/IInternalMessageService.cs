using CutOutWiz.Core;
using CutOutWiz.Core.Message;
using CutOutWiz.Services.Models.Message;

namespace CutOutWiz.Services.InternalMessage
{
    public interface IInternalMessageService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<InternalMessageModel>> GetAll();
        Task<List<InternalMessageModel>> GetByContactId(int contactId);
        Task<InternalMessageModel> GetByObjectId(string objectId);
        // <summary>
        /// Get template by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        Task<InternalMessageModel> GetByAccessCode(string accessCode);
        Task<Response<int>> Insert(InternalMessageNotification template);
    }
}
