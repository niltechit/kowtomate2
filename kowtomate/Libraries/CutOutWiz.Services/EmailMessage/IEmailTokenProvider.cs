using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.Models.EmailTokenModels;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.EmailMessage
{
    public interface IEmailTokenProvider
    {
        void AddAccountResetNotification(List<EmailToken> tokens, PasswordResetNofitication passwordResetNofitication);
        void AddAccountVerifyNotification(List<EmailToken> tokens, AccountVerifyNofitication accountVerifyNofitication);
        void AddSopAddUpdateNotificationForCompanyAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification);
        void AddSopAddUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification);
        void AddOrderAddUpdateNotificationForCompanyAllUsers(List<EmailToken> tokens, OrderAddUpdateNotification orderAddUpdateNotification);
        void SopPriceNotificationForOperationAndClientAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification);
        void SopPriceApprovedNotificationForOperationAndClientAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification);
        void CompanyAddUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel);
        void NewUserCreateUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, NewUserNotificatonViewModel newUserNotificatonViewModel);
        void CompanyUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel);
        void SendEmailForRegisterCompanyUsernameAndPassword(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel);
        void SendEmailForCompanyEdit(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel);
        void SendEmailForPasswordReset(List<EmailToken> tokens, EmailUserViewModel userViewModel);
        void SendEmailToOpsToNotifyImageArrivalOnFtp(List<EmailToken> tokens, FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP);
        void SendEmailToOpsToNotifyOrderUpload(List<EmailToken> tokens, FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP);
        void SendEmailToOpsToNotifyOrderDeliveryToClient(List<EmailToken> tokens, FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP);
        void SendEmailToOpsToNotifyCloudStorageUsesLimitation(List<EmailToken> tokens, CloudStorageUsesLimitationNotifyOps notifyOpsOnImageArrivalFTP);
        void InitializeOrderPlacementEmailTokens(List<EmailToken> tokens, OrderPlacementNotificationTokens notifyOpsOnImageArrivalFTP);
	}
}