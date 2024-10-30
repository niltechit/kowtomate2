using CutOutWiz.Data;
using CutOutWiz.Data.EmailSender;

namespace CutOutWiz.Services.EmailSender
{
    public interface IMailjetEmailService
    {
        Task<Response<bool>> SendEmail(EmailSendRequestModel emailSendRequest);
    }
}