using CutOutWiz.Data;
using CutOutWiz.Data.Models.Message;
using System.Data;

namespace CutOutWiz.Services.MessageService
{
    public interface IEmailSenderService
    {
        Task<Response<bool>> SendEmail(EmailMessageModel emailMessage);
        EmailMessageModel GetEmailMessage(DataTable fileTracking);
    }
}