using CutOutWiz.Data.EmailModels;

namespace CutOutWiz.Services.EmailMessage
{
    public interface IWorkflowEmailService
    {
        Task<bool> SendAccountVerifyNotificationForUser(AccountVerifyNofitication accountVerifyNofitication);
        Task<bool> SendPasswordResetNotificationForUser(PasswordResetNofitication passwordResetNofitication);
    }
}