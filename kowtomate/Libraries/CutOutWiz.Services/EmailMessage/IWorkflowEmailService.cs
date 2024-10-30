using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.ClientOrders;

namespace CutOutWiz.Services.EmailMessage
{
    public interface IWorkflowEmailService
    {
        Task<bool> SendAccountVerifyNotificationForUser(AccountVerifyNofitication accountVerifyNofitication);
        Task<bool> SendPasswordResetNotificationForUser(PasswordResetNofitication passwordResetNofitication);
        Task<bool> SendSopAddUpdateDeleteNotificationForCompanyAllUsers(SOPAddUpdateNotification sOPAddUpdateNotification);
        Task<bool> SendSopAddUpdateDeleteNotificationForCompanyOperationsTeam(SOPAddUpdateNotification sOPAddUpdateNotification);
        Task<bool> SendOrderAddUpdateDeleteNotificationForCompanyOperationsTeam(ClientOrderViewModel orderVM);
        Task<bool> SendSopUpdateNotificationForCompanyAllUsers(SOPAddUpdateNotification sOPAddUpdateNotification);
        Task<bool> SendOrderAddNotificationForCompanyAllUsers(OrderAddUpdateNotification orderAddUpdateNotification);
        Task<bool> SendOrderUpdateNotificationForCompanyAllUsers(OrderAddUpdateNotification orderAddUpdateNotification);
        Task<bool> NewUserCreateEmailSendNofication(NewUserNotificatonViewModel newUserNotificatonViewModel);
        Task<bool> SendCompanyAddUpdateDeleteNotificationForCompanyOperationsTeam(NewCompanySignUpNotification model);
        Task<bool> SendCompanyUsernameAndPassword(NewCompanySignUpNotification newCompanySignUpNotification);
        Task<bool> PasswordReset(EmailUserViewModel model);
        Task<bool> SendEmailToOpsToNotifyImageArrivalOnFtp(FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP);
        Task<bool> SendEmailToOpsToNotifyOrderUpload(FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP);
        Task<bool> SendEmailToOpsToNotifyOrderDeliveryToClient(FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP);

        Task SendEmailToOpsToNotifyOrderUpload(string batchName, string orderNumber, CompanyModel company, string folderName = " ", string userName = " ", int numberOfImages = 0);
        Task<bool> SendEmailNotificationForCloudStorageLimitation(CloudStorageUsesLimitationNotifyOps notifyOpsOnImageArrivalFTP);
        Task<bool> SendOrderPlacementEmailsToClientCompany(OrderPlacementNotificationEmailModel orderPlacementNotificationEmailModel);
    }
}