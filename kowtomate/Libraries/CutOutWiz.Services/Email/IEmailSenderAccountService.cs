using CutOutWiz.Core;
using CutOutWiz.Services.Models.Email;

namespace CutOutWiz.Services.Email
{
    public interface IEmailSenderAccountService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<EmailSenderAccountModel>> GetAll();
        Task<EmailSenderAccountModel> GetById(int senderAccountId);
        Task<EmailSenderAccountModel> GetByObjectId(string objectId);
        /// <summary>
        /// Get Default Account
        /// </summary>
        /// <param name="SenderAccountId"></param>
        /// <returns></returns>
        Task<EmailSenderAccountModel> GetDefaultAccount();

        Task<Response<int>> Insert(EmailSenderAccountModel senderAccount);
        Task<Response<bool>> Update(EmailSenderAccountModel senderAccount);
    }
}
