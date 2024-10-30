using CutOutWiz.Data;
using CutOutWiz.Data.Email;

namespace CutOutWiz.Services.Email
{
    public interface IEmailSenderAccountService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<EmailSenderAccount>> GetAll();
        Task<EmailSenderAccount> GetById(int senderAccountId);
        Task<EmailSenderAccount> GetByObjectId(string objectId);
        /// <summary>
        /// Get Default Account
        /// </summary>
        /// <param name="SenderAccountId"></param>
        /// <returns></returns>
        Task<EmailSenderAccount> GetDefaultAccount();

        Task<Response<int>> Insert(EmailSenderAccount senderAccount);
        Task<Response<bool>> Update(EmailSenderAccount senderAccount);
    }
}
