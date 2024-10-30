using CutOutWiz.Data.EmailModels;

namespace CutOutWiz.Services.EmailMessage
{
    public interface IEmailTokenProvider
    {
        void AddAccountResetNotification(List<EmailToken> tokens, PasswordResetNofitication passwordResetNofitication);
        void AddAccountVerifyNotification(List<EmailToken> tokens, AccountVerifyNofitication accountVerifyNofitication);
    }
}