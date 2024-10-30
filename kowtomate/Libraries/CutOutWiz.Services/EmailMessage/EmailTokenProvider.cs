using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.Models.EmailTokenModels;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.EmailMessage
{
    public class EmailTokenProvider : IEmailTokenProvider
    {
        #region Account Notification 

        /// <summary>
        /// Adds password reset notification tokens used in template
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="passwordResetNofitication"></param>

        public void AddAccountResetNotification(List<EmailToken> tokens,
                PasswordResetNofitication passwordResetNofitication)
        {
            tokens.Add(new EmailToken("AccountReset.ContactFirstName", passwordResetNofitication.ContactFirstName));
            tokens.Add(new EmailToken("AccountReset.PasswordResetUrl", passwordResetNofitication.PasswordResetUrl));
        }

        /// <summary>
        /// Adds account verify notification tokens used in template
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="accountVerifyNofitication"></param>

        public void AddAccountVerifyNotification(List<EmailToken> tokens,
                    AccountVerifyNofitication accountVerifyNofitication)
        {
            tokens.Add(new EmailToken("Account.AccountVerifyUrl", accountVerifyNofitication.AccountVerifyUrl));
            tokens.Add(new EmailToken("Account.ToEmailName", accountVerifyNofitication.ToEmailName));
        }

        public void AddSopAddUpdateNotificationForCompanyAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            tokens.Add(new EmailToken("Contact.FirstName", sOPAddUpdateNotification.Contact.FirstName));
            tokens.Add(new EmailToken("SOP.ViewUrl", sOPAddUpdateNotification.DetailUrl));
        }
        public void AddSopAddUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            tokens.Add(new EmailToken("Contact.FirstName", sOPAddUpdateNotification.LoginContactName));
            tokens.Add(new EmailToken("SOP.ViewUrl", sOPAddUpdateNotification.DetailUrl));
        }
        public void CompanyAddUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel)
        {
            tokens.Add(new EmailToken("AdminUser.ReceipentName", newCompanySignUpNotificationViewModel.ToEmailName));
            tokens.Add(new EmailToken("Company.DetailsUrl", newCompanySignUpNotificationViewModel.DetailUrl));
            tokens.Add(new EmailToken("Contact.CreateByUserName", newCompanySignUpNotificationViewModel.CreateByUserName));
            tokens.Add(new EmailToken("Company.Name", newCompanySignUpNotificationViewModel.NewCompanyName));
        }
        public void CompanyUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel)
        {
            tokens.Add(new EmailToken("AdminUser.ReceipentName", newCompanySignUpNotificationViewModel.ToEmailName));
            tokens.Add(new EmailToken("Company.DetailsUrl", newCompanySignUpNotificationViewModel.DetailUrl));
            tokens.Add(new EmailToken("Contact.CreateByUserName", newCompanySignUpNotificationViewModel.CreateByUserName));
            tokens.Add(new EmailToken("Company.Name", newCompanySignUpNotificationViewModel.NewCompanyName));
        }
        public void NewUserCreateUpdateNotificationForOperationAllUsers(List<EmailToken> tokens, NewUserNotificatonViewModel newUserNotificatonViewModel)
        {
            tokens.Add(new EmailToken("AdminUser.ReceipentName", newUserNotificatonViewModel.ToEmailName));
            tokens.Add(new EmailToken("Company.DetailsUrl", newUserNotificatonViewModel.DetailUrl));
            tokens.Add(new EmailToken("Contact.CreateByUserName", newUserNotificatonViewModel.CreateByUserName));
            tokens.Add(new EmailToken("Company.Name", newUserNotificatonViewModel.CompanyName));
        }
        public void SendEmailForRegisterCompanyUsernameAndPassword(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel)
        {
            tokens.Add(new EmailToken("Client.FirstName", newCompanySignUpNotificationViewModel.ClientFirstName));
            tokens.Add(new EmailToken("User.Username", newCompanySignUpNotificationViewModel.UserName));
            tokens.Add(new EmailToken("User.Password", newCompanySignUpNotificationViewModel.Password));
            tokens.Add(new EmailToken("Website.LoginUrl", newCompanySignUpNotificationViewModel.LoginUrl));
        }
        public void SendEmailForCompanyEdit(List<EmailToken> tokens, NewCompanySignUpNotification newCompanySignUpNotificationViewModel)
        {
            tokens.Add(new EmailToken("Client.FirstName", newCompanySignUpNotificationViewModel.ClientFirstName));
            if(newCompanySignUpNotificationViewModel.UserName != null)
            {
                tokens.Add(new EmailToken("User.Username", newCompanySignUpNotificationViewModel.UserName));
            }
            if (newCompanySignUpNotificationViewModel.Password != null)
            {
                tokens.Add(new EmailToken("User.Password", newCompanySignUpNotificationViewModel.Password));
            }
            if (newCompanySignUpNotificationViewModel.CompanyEmail != null)
            {
				tokens.Add(new EmailToken("User.Password", newCompanySignUpNotificationViewModel.CompanyEmail));
			}
           
           tokens.Add(new EmailToken("Website.LoginUrl", newCompanySignUpNotificationViewModel.LoginUrl));
        }
        public void SendEmailForPasswordReset(List<EmailToken> tokens, EmailUserViewModel userViewModel)
        {
            tokens.Add(new EmailToken("User.FirstName", userViewModel.LoginContactName));
            tokens.Add(new EmailToken("User.CompanyName", userViewModel.CompanyName));
            tokens.Add(new EmailToken("User.DetailUrl", userViewModel.DetailUrl));
            tokens.Add(new EmailToken("User.Email", userViewModel.Email));
        }
        public void AddOrderAddUpdateNotificationForCompanyAllUsers(List<EmailToken> tokens, OrderAddUpdateNotification orderAddUpdateNotification)
        {
            tokens.Add(new EmailToken("Contact.FirstName", orderAddUpdateNotification.Contact.FirstName));
            tokens.Add(new EmailToken("Order.ViewUrl", orderAddUpdateNotification.DetailUrl));
            tokens.Add(new EmailToken("Order.OrderNumber", orderAddUpdateNotification.OrderNumber));
        }
        public void SopPriceNotificationForOperationAndClientAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            tokens.Add(new EmailToken("Contact.FirstName", sOPAddUpdateNotification.Contact.FirstName));
            tokens.Add(new EmailToken("Template.Name", sOPAddUpdateNotification.TemplateName));
        }
        public void SopPriceApprovedNotificationForOperationAndClientAllUsers(List<EmailToken> tokens, SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            tokens.Add(new EmailToken("Client.FirstName", sOPAddUpdateNotification.Contact.FirstName));
            tokens.Add(new EmailToken("SOP.ViewUrl", sOPAddUpdateNotification.DetailUrl));
            tokens.Add(new EmailToken("Template.Name", sOPAddUpdateNotification.TemplateName));
        }

		public void SendEmailToOpsToNotifyImageArrivalOnFtp(List<EmailToken> tokens, FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP)
		{
			tokens.Add(new EmailToken("FtpOrder.FolderName", notifyOpsOnImageArrivalFTP?.FolderName));
			tokens.Add(new EmailToken("FtpOrder.ArrivalTime", notifyOpsOnImageArrivalFTP?.ArrivalDateTime));
			tokens.Add(new EmailToken("FtpOrder.DeliveryTime", notifyOpsOnImageArrivalFTP?.DeliveryTime));
			tokens.Add(new EmailToken("FtpOrder.CompanyName", notifyOpsOnImageArrivalFTP?.CompanyName));
			//tokens.Add(new EmailToken("FtpOrder.ImageCount", notifyOpsOnImageArrivalFTP?.ImageCount));
			//tokens.Add(new EmailToken("FtpOrder.OrderType", notifyOpsOnImageArrivalFTP?.OrderType));
			tokens.Add(new EmailToken("FtpOrder.FtpUserName", notifyOpsOnImageArrivalFTP?.FtpUserName));
		}

		public void SendEmailToOpsToNotifyOrderUpload(List<EmailToken> tokens, FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP)
		{
			tokens.Add(new EmailToken("Order.BatchName", notifyOpsOnImageArrivalFTP?.BatchName));
			tokens.Add(new EmailToken("Order.OrderNumber", notifyOpsOnImageArrivalFTP?.OrderNumber));
		    tokens.Add(new EmailToken("Order.CompanyName", notifyOpsOnImageArrivalFTP?.CompanyName));
		}
		public void SendEmailToOpsToNotifyCloudStorageUsesLimitation(List<EmailToken> tokens, CloudStorageUsesLimitationNotifyOps notifyOpsOnImageArrivalFTP)
		{
			tokens.Add(new EmailToken("memory_uses", notifyOpsOnImageArrivalFTP?.MemoryUses));
		}

		public void SendEmailToOpsToNotifyOrderDeliveryToClient(List<EmailToken> tokens, FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP)
		{
			tokens.Add(new EmailToken("Order.OrderNumber", notifyOpsOnImageArrivalFTP?.OrderNumber));
			tokens.Add(new EmailToken("Order.CompanyName", notifyOpsOnImageArrivalFTP?.CompanyName));
			tokens.Add(new EmailToken("Order.ImageCount", notifyOpsOnImageArrivalFTP?.ImageCount));
		}
        public void InitializeOrderPlacementEmailTokens(List<EmailToken> tokens, OrderPlacementNotificationTokens orderPlacementNotificationTokens)
		{
			tokens.Add(new EmailToken("Order.OrderNumber", orderPlacementNotificationTokens?.OrderNumber ?? "NULL"));
			tokens.Add(new EmailToken("Order.ImageCount", orderPlacementNotificationTokens?.ImageCount ?? "NULL"));
			tokens.Add(new EmailToken("SOP.ViewUrl", orderPlacementNotificationTokens?.OrderDetailURL ?? "NULL"));
		}

		#endregion
	}
}
