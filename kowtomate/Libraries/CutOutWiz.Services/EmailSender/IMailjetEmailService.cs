using CutOutWiz.Core;
using CutOutWiz.Services.Models.EmailSender;

namespace CutOutWiz.Services.EmailSender
{
    public interface IMailjetEmailService
    {
        Task<Response<bool>> SendEmail(EmailSendRequestModel emailSendRequest);
    }
}