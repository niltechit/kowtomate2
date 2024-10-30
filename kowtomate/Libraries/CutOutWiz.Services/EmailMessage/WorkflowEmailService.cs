using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.Email;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Services.Models.EmailSender;
using CutOutWiz.Services.Models.EmailTokenModels;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Email;
using CutOutWiz.Services.EmailSender;
using CutOutWiz.Services.LogServices;
using CutOutWiz.Services.Security;
using Microsoft.Extensions.Configuration;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Models.ClientOrders;

namespace CutOutWiz.Services.EmailMessage
{
    public class WorkflowEmailService : IWorkflowEmailService
    {
        #region Private Members
        //private readonly IEmailQueueService _queuedEmailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogService _logService;
        private readonly IEmailTokenizer _emailTokenizer;
        private readonly IEmailSenderAccountService _emailSenderAccountService;
        private readonly IEmailTokenProvider _emailTokenProvider;
        private readonly IMailjetEmailService _mailjetEmailService;
        private readonly IConfiguration _configuration;
        private readonly IArchiveQueueEmailService _archiveQueueEmailService;
        private readonly IContactManager _contactManager;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedEmailService"></param>
        /// <param name="emailTemplateService"></param>
        /// <param name="logService"></param>
        /// <param name="emailTokenizer"></param>
        /// <param name="emailAccountService"></param>
        /// <param name="emailTokenProvider"></param>
        public WorkflowEmailService(//IEmailQueueService queuedEmailService,
                                    IEmailTemplateService emailTemplateService,
                                    ILogService logService,
                                    IEmailTokenizer emailTokenizer,
                                    IEmailSenderAccountService emailSenderAccountService,
                                    IEmailTokenProvider emailTokenProvider,
                                    IMailjetEmailService mailjetEmailService,
                                    IConfiguration configuration,
                                    IArchiveQueueEmailService archiveQueueEmailService,
                                    IContactManager contactService
            )
        {
            //_queuedEmailService = queuedEmailService;
            _emailTemplateService = emailTemplateService;
            _logService = logService;
            _emailTokenizer = emailTokenizer;
            _emailSenderAccountService = emailSenderAccountService;
            _emailTokenProvider = emailTokenProvider;
            _mailjetEmailService = mailjetEmailService;
            _configuration = configuration;
            _archiveQueueEmailService = archiveQueueEmailService;
            _contactManager = contactService;
        }
        #endregion

        #region Account Notification
        /// <summary>
        /// Sends an email about a  account reset
        /// </summary>
        /// <param name="passwordResetNofitication"></param>
        /// <returns></returns>
        public async Task<bool> SendPasswordResetNotificationForUser(PasswordResetNofitication passwordResetNofitication)
        {
            var emailTemplate = await GetActiveEmailTemplate("Account.PasswordReset");

            if (emailTemplate == null)
                return false;

            // Add tokens
            var tokens = new List<EmailToken>();
            _emailTokenProvider.AddAccountResetNotification(tokens, passwordResetNofitication);

            var toEmail = passwordResetNofitication.ToEmail;
            var toName = passwordResetNofitication.ToEmailName;

            return await SendNotification(emailTemplate, tokens, passwordResetNofitication.CreatedByContactId, toEmail, toName);
        }

        /// <summary>
        /// Sends an email about a  account veriry
        /// </summary>
        /// <param name="accountVerifyNofitication"></param>
        /// <returns></returns>
        public async Task<bool> SendAccountVerifyNotificationForUser(AccountVerifyNofitication accountVerifyNofitication)
        {
            var emailTemplate = await GetActiveEmailTemplate("Account.VerifyAccount");

            if (emailTemplate == null)
                return false;

            // Add tokens
            var tokens = new List<EmailToken>();
            _emailTokenProvider.AddAccountVerifyNotification(tokens, accountVerifyNofitication);

            var toEmail = accountVerifyNofitication.ToEmail;
            var toName = accountVerifyNofitication.ToEmailName;

            return await SendNotification(emailTemplate, tokens, accountVerifyNofitication.CreatedByContactId, toEmail, toName);
        }
        #endregion

        #region SOP notificitaon

        public async Task<bool> SendSopAddUpdateDeleteNotificationForCompanyOperationsTeam(SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (sOPAddUpdateNotification.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("SOPNOTIOperation");
                }
                else if (sOPAddUpdateNotification.MailType == "Update")
                {
                    emailTemplate = await GetActiveEmailTemplate("SOP.UpdateSOPNotificationForOperation");
                }
                else if (sOPAddUpdateNotification.MailType == "Delete")
                {
                    emailTemplate = await GetActiveEmailTemplate("SOP.DeleteSOPNotificationForOperation");
                }
                else if (sOPAddUpdateNotification.MailType == "PriceUpdateByClient")
                {
                    emailTemplate = await GetActiveEmailTemplate("Sop.PriceConfirmationNotificationForOperation");
                }
                else if (sOPAddUpdateNotification.MailType == "PriceApprovedByClient")
                {
                    emailTemplate = await GetActiveEmailTemplate("Sop.PriceApprovedNotificationForOperation");
                }
            

                if (emailTemplate == null)
                {
                    return false;
                }
                var loginContact = await _contactManager.GetById(sOPAddUpdateNotification.CreatedByContactId);
                var toEmail = sOPAddUpdateNotification.Contact.Email;
                var toName = sOPAddUpdateNotification.Contact.FirstName + sOPAddUpdateNotification.Contact.LastName;
                var tokens = new List<EmailToken>();
                SOPAddUpdateNotification sopAddNotification = new SOPAddUpdateNotification
                {
                    Contact = sOPAddUpdateNotification.Contact,
                    DetailUrl = sOPAddUpdateNotification.DetailUrl,
                    LoginContactName=loginContact.FirstName,
                    TemplateName = sOPAddUpdateNotification.TemplateName,
                };
                if(sOPAddUpdateNotification.MailType == "PriceUpdateByClient" || sOPAddUpdateNotification.MailType == "PriceApprovedByClient")
                {
                    _emailTokenProvider.SopPriceNotificationForOperationAndClientAllUsers(tokens, sopAddNotification);
                }
                else
                {
                    _emailTokenProvider.AddSopAddUpdateNotificationForOperationAllUsers(tokens, sopAddNotification);
                }
                await SendNotification(emailTemplate, tokens, sOPAddUpdateNotification.CreatedByContactId, toEmail, toName);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
       
        public async Task<bool> SendSopAddUpdateDeleteNotificationForCompanyAllUsers(SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (sOPAddUpdateNotification.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("SOP.NewSOPNotificationForClient");
                }
                else if (sOPAddUpdateNotification.MailType == "Update")
                {
                    emailTemplate = await GetActiveEmailTemplate("SOP.UpdateSOPNotificationForClient");
                }
                else if (sOPAddUpdateNotification.MailType == "Delete")
                {
                    emailTemplate = await GetActiveEmailTemplate("SOP.DeleteSOPNotificationForClient");
                }
                else if(sOPAddUpdateNotification.MailType == "PriceUpdateByOperation")
                {
                    emailTemplate = await GetActiveEmailTemplate("Sop.PriceConfirmationNotificationForClient");
                }
                else if(sOPAddUpdateNotification.MailType == "PriceApprovedByOperation")
                {
                    emailTemplate = await GetActiveEmailTemplate("Sop.PriceApprovedNotificationForClient");
                }

                if (emailTemplate == null)
                {
                    return false;
                }

                foreach (var contact in sOPAddUpdateNotification.Contacts)
                {
                    var toEmail = contact.Email;
                    var toName = contact.FirstName + contact.LastName;
                    var tokens = new List<EmailToken>();
                    SOPAddUpdateNotification sopAddNotification = new SOPAddUpdateNotification
                    {
                        Contact = contact,
                        DetailUrl = sOPAddUpdateNotification.DetailUrl,
                        TemplateName = sOPAddUpdateNotification.TemplateName
                    };
                    if(sOPAddUpdateNotification.MailType == "PriceUpdateByOperation" )
                    {
                        _emailTokenProvider.SopPriceNotificationForOperationAndClientAllUsers(tokens, sopAddNotification);
                    }
                    else if(sOPAddUpdateNotification.MailType == "PriceApprovedByOperation")
                    {
                        _emailTokenProvider.SopPriceApprovedNotificationForOperationAndClientAllUsers(tokens, sopAddNotification);
                    }
                    else                                                     
                    {
                        _emailTokenProvider.AddSopAddUpdateNotificationForCompanyAllUsers(tokens, sopAddNotification);
                        
                    }
                    await SendNotification(emailTemplate, tokens, sOPAddUpdateNotification.CreatedByContactId, toEmail, toName);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendSopUpdateNotificationForCompanyAllUsers(SOPAddUpdateNotification sOPAddUpdateNotification)
        {
            try
            {
                var emailTemplate = await GetActiveEmailTemplate("SOP.UpdateSOPNotificationForClient");
                if (emailTemplate == null)
                    return false;
                foreach (var contact in sOPAddUpdateNotification.Contacts)
                {
                    var toEmail = contact.Email;
                    var toName = contact.FirstName + contact.LastName;
                    var tokens = new List<EmailToken>();
                    SOPAddUpdateNotification sopUpdateNotification = new SOPAddUpdateNotification
                    {
                        Contact = contact,
                        DetailUrl = sOPAddUpdateNotification.DetailUrl
                    };
                    _emailTokenProvider.AddSopAddUpdateNotificationForCompanyAllUsers(tokens, sopUpdateNotification);
                    await SendNotification(emailTemplate, tokens, sOPAddUpdateNotification.CreatedByContactId, toEmail, toName);
                }
            }

            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
        #endregion

        #region Order notification
        public async Task<bool> SendOrderAddNotificationForCompanyAllUsers(OrderAddUpdateNotification orderAddUpdateNotification)
        {
            try
            {
                var emailTemplate = await GetActiveEmailTemplate("Order.NewOrderNotificationForClient");
                if (emailTemplate == null)
                    return false;
                foreach (var contact in orderAddUpdateNotification.Contacts)
                {
                    var toEmail = contact.Email;
                    var toName = contact.FirstName + contact.LastName;
                    var tokens = new List<EmailToken>();
                    OrderAddUpdateNotification orderAddNotification = new OrderAddUpdateNotification
                    {
                        Contact = contact,
                        DetailUrl = orderAddUpdateNotification.DetailUrl,
                        OrderNumber = orderAddUpdateNotification.OrderNumber,
                    };
                    _emailTokenProvider.AddOrderAddUpdateNotificationForCompanyAllUsers(tokens, orderAddNotification);
                    await SendNotification(emailTemplate, tokens, orderAddUpdateNotification.CreatedByContactId, toEmail, toName);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendOrderUpdateNotificationForCompanyAllUsers(OrderAddUpdateNotification orderAddUpdateNotification)
        {
            try
            {
                var emailTemplate = await GetActiveEmailTemplate("Order.UpdateOrderNotificationForClient");
                if (emailTemplate == null)
                    return false;
                foreach (var contact in orderAddUpdateNotification.Contacts)
                {
                    var toEmail = contact.Email;
                    var toName = contact.FirstName + contact.LastName;
                    var tokens = new List<EmailToken>();
                    OrderAddUpdateNotification orderUpdateNotification = new OrderAddUpdateNotification
                    {
                        Contact = contact,
                        DetailUrl = orderAddUpdateNotification.DetailUrl,
                        OrderNumber = orderAddUpdateNotification.OrderNumber,
                    };
                    _emailTokenProvider.AddOrderAddUpdateNotificationForCompanyAllUsers(tokens, orderUpdateNotification);
                    await SendNotification(emailTemplate, tokens, orderAddUpdateNotification.CreatedByContactId, toEmail, toName);
                }
            }

            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> SendOrderAddUpdateDeleteNotificationForCompanyOperationsTeam(ClientOrderViewModel orderVM)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (orderVM.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("Order.CreateNotificationForOPerations");
                }
                else if (orderVM.MailType == "Update")
                {
                    emailTemplate = await GetActiveEmailTemplate("Order.UpdateNotificationForOPerations");
                }
                else if (orderVM.MailType == "Delete")
                {
                    emailTemplate = await GetActiveEmailTemplate("Order.DeleteNotificationForOPerations");
                }
                if (emailTemplate == null)
                {
                    return false;
                }
                var loginContact = await _contactManager.GetById(orderVM.CreatedByContactId);
                var toEmail = orderVM.Contact.Email;
                var toName = orderVM.Contact.FirstName + orderVM.Contact.LastName;
                var tokens = new List<EmailToken>();
                SOPAddUpdateNotification sopAddNotification = new SOPAddUpdateNotification
                {
                    Contact = orderVM.Contact,
                    DetailUrl = orderVM.DetailUrl,
                    LoginContactName = loginContact.FirstName,
                    TemplateName = orderVM.OrderNumber,
                };
                if (orderVM.MailType == "PriceUpdateByClient" || orderVM.MailType == "PriceApprovedByClient")
                {
                    _emailTokenProvider.SopPriceNotificationForOperationAndClientAllUsers(tokens, sopAddNotification);
                }
                else
                {
                    _emailTokenProvider.AddSopAddUpdateNotificationForOperationAllUsers(tokens, sopAddNotification);
                }
                await SendNotification(emailTemplate, tokens, orderVM.CreatedByContactId, toEmail, toName);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

		public async Task<bool> SendEmailToOpsToNotifyImageArrivalOnFtp(FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP)
		{
			try
			{
				var emailTemplate = await GetActiveEmailTemplate(EmailTemplateConstants.NotifyOpsOnImageArrivalFTP);
				if (emailTemplate == null)
					return false;
				foreach (var email in notifyOpsOnImageArrivalFTP.EmailAddresses)
				{
					var toEmail = email;
					//var toName = contact.FirstName + contact.LastName;
					var tokens = new List<EmailToken>();
                    //OrderAddUpdateNotification orderUpdateNotification = new OrderAddUpdateNotification
                    //{
                    //    Contact = contact,
                    //    DetailUrl = orderAddUpdateNotification.DetailUrl,
                    //    OrderNumber = orderAddUpdateNotification.OrderNumber,
                    //};
                    _emailTokenProvider.SendEmailToOpsToNotifyImageArrivalOnFtp(tokens, notifyOpsOnImageArrivalFTP);
					await SendNotification(emailTemplate, tokens, AutomatedAppConstant.ContactId, toEmail, "");
				}
			}

			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> SendEmailToOpsToNotifyOrderUpload(FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP)
		{
			try
			{
				var emailTemplate = await GetActiveEmailTemplate(EmailTemplateConstants.NotifyOpsOnOrderPlaced);
				if (emailTemplate == null)
					return false;
				foreach (var email in notifyOpsOnImageArrivalFTP.EmailAddresses)
				{
					var toEmail = email;
					//var toName = contact.FirstName + contact.LastName;
					var tokens = new List<EmailToken>();
					//OrderAddUpdateNotification orderUpdateNotification = new OrderAddUpdateNotification
					//{
					//    Contact = contact,
					//    DetailUrl = orderAddUpdateNotification.DetailUrl,
					//    OrderNumber = orderAddUpdateNotification.OrderNumber,
					//};
					_emailTokenProvider.SendEmailToOpsToNotifyOrderUpload(tokens, notifyOpsOnImageArrivalFTP);
					await SendNotification(emailTemplate, tokens, AutomatedAppConstant.ContactId, toEmail, "");
				}
			}

			catch (Exception ex)
			{
				return false;
			}

			return true;
		}
        public async Task<bool> SendEmailNotificationForCloudStorageLimitation(CloudStorageUsesLimitationNotifyOps notifyOpsOnImageArrivalFTP)
		{
			try
			{
				var emailTemplate = await GetActiveEmailTemplate(EmailTemplateConstants.NotifyOpsForCloudStorageUsesLimitation);
				if (emailTemplate == null)
					return false;
				foreach (var email in notifyOpsOnImageArrivalFTP.EmailAddresses)
				{
					var toEmail = email;
					var tokens = new List<EmailToken>();
					_emailTokenProvider.SendEmailToOpsToNotifyCloudStorageUsesLimitation(tokens, notifyOpsOnImageArrivalFTP);
					await SendNotification(emailTemplate, tokens, AutomatedAppConstant.ContactId, toEmail, "");
				}
			}

			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> SendEmailToOpsToNotifyOrderDeliveryToClient(FTPOrderNotifyOpsOnImageArrivalFTP notifyOpsOnImageArrivalFTP)
		{
			try
			{
				var emailTemplate = await GetActiveEmailTemplate(EmailTemplateConstants.NotifyOpsOnOrderDeliveryToClient);
				if (emailTemplate == null)
					return false;
				foreach (var email in notifyOpsOnImageArrivalFTP.EmailAddresses)
				{
					var toEmail = email;
					//var toName = contact.FirstName + contact.LastName;
					var tokens = new List<EmailToken>();
					//OrderAddUpdateNotification orderUpdateNotification = new OrderAddUpdateNotification
					//{
					//    Contact = contact,
					//    DetailUrl = orderAddUpdateNotification.DetailUrl,
					//    OrderNumber = orderAddUpdateNotification.OrderNumber,
					//};
					_emailTokenProvider.SendEmailToOpsToNotifyOrderDeliveryToClient(tokens, notifyOpsOnImageArrivalFTP);
					await SendNotification(emailTemplate, tokens, AutomatedAppConstant.ContactId, toEmail, "");
				}
			}

			catch (Exception ex)
			{
				return false;
			}

			return true;
		}
		#endregion

		#region Utilities

		/// <summary>
		/// Adds the email message to the email queue
		/// </summary>
		/// <param name="emailTemplate"></param>
		/// <param name="tokens"></param>
		/// <param name="toEmailAddress"></param>
		/// <param name="toName"></param>
		/// <param name="attachmentFilePath"></param>
		/// <param name="attachmentFileName"></param>
		/// <param name="replyToEmailAddress"></param>
		/// <param name="replyToName"></param>
		/// <returns></returns>
		protected virtual async Task<bool> SendNotification(
                    EmailTemplateModel emailTemplate,
                    IEnumerable<EmailToken> tokens,
                    int CreatedByContactId,
                    string toEmailAddress, string toName,
                    string attachmentFilePath = null, string attachmentFileName = null,
                    string replyToEmailAddress = null, string replyToName = null

            )
        {
            try
            {
                // Replace subject and body tokens 
                var emailTokens = tokens as IList<EmailToken> ?? tokens.ToList();

                var subjectReplaced = _emailTokenizer.Replace(emailTemplate.Subject, emailTokens, false);
                var bodyReplaced = _emailTokenizer.Replace(emailTemplate.Body, emailTokens, true);

                var emailAccount = await _emailSenderAccountService.GetById((int)emailTemplate.SenderAccountId);

                if (emailAccount == null)
                    return false;

                var fromEmail = emailAccount.Email;
                var fromEmailName = emailAccount.EmailDisplayName;

                var emailSendRequestModel = new EmailSendRequestModel
                {
                    ApiKey = emailAccount.ApiKey,
                    ApiSecret = emailAccount.SecretKey,

                    FromEmail = fromEmail,
                    ToEmail = toEmailAddress,

                    Subject = subjectReplaced,
                    Body = bodyReplaced
                };

                var emailSendResponse = await _mailjetEmailService.SendEmail(emailSendRequestModel);

                var archiveQueueEmail = new ArchiveQueueEmailModel
                {
                    EmailAccountId = emailAccount.Id,
                    FromEmail = fromEmail,
                    FromEmailName = fromEmailName,
                    ToEmail = toEmailAddress,
                    ToEmailName = toName,
                    CcEmail = string.Empty,
                    BccEmail = emailTemplate.BCCEmailAddresses,
                    Subject = subjectReplaced,
                    Body = bodyReplaced,
                    AttachmentFilePath = attachmentFilePath,
                    AttachedFileName = attachmentFileName,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByContactId = CreatedByContactId,
                    SentTries = 0,
                    ObjectId = Guid.NewGuid().ToString()
                };

                if (emailSendResponse.IsSuccess)
                {
                    archiveQueueEmail.Status = (int)EmailQueueStatus.Sent;
                }
                else
                {
                    archiveQueueEmail.SentTries = 1;
                    archiveQueueEmail.Status = (int)EmailQueueStatus.NotSent;
                }

                await _archiveQueueEmailService.AddAchiveEmail(archiveQueueEmail);

                return emailSendResponse.IsSuccess;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        /// <summary>
        /// Gets the email template and make sure it's active
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        protected async Task<EmailTemplateModel> GetActiveEmailTemplate(string templateName)
        {
            var emailTemplate = await _emailTemplateService.GetByAccessCode(templateName);
            return emailTemplate;
        }

        #endregion

        #region Logging

        ///// <summary>
        ///// Log service exceptions
        ///// </summary>
        ///// <param name="ex"></param>
        ///// <param name="functionName"></param>
        //private void LogWorkflowEmailServiceExceptions(Exception ex, string functionName)
        //{
        //    var newLogModel = new AppLogModel
        //    {
        //        LogLevel = AppLogLevel.High.ToString(),
        //        Logger = AppLogLoggerConstants.Admin,
        //        ShortMessage = $"WorkflowEmailService::{functionName}",
        //        StackTrace = ex.Message
        //    };
        //    _logService.Add(newLogModel);
        //}

        #endregion
        #region New Company Registration Email Send
        public async Task<bool> SendCompanyAddUpdateDeleteNotificationForCompanyOperationsTeam(NewCompanySignUpNotification newCompanySignUpNotification)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (newCompanySignUpNotification.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("company.CreateNotificationForOPerations");
                }
                if (newCompanySignUpNotification.MailType == "Update")
                {
                    emailTemplate = await GetActiveEmailTemplate("company.SendEmailUpdateForOperation");
                }

                if (emailTemplate == null)
                {
                    return false;
                }

                var tokens = new List<EmailToken>();
                _emailTokenProvider.CompanyUpdateNotificationForOperationAllUsers(tokens, newCompanySignUpNotification);
                await SendNotification(emailTemplate, tokens, newCompanySignUpNotification.ContactId, newCompanySignUpNotification.ToEmail, newCompanySignUpNotification.ToEmailName);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> NewUserCreateEmailSendNofication(NewUserNotificatonViewModel newUserNotificatonViewModel)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (newUserNotificatonViewModel.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("user.CreateNotificationForOPerations");
                }
                if (newUserNotificatonViewModel.MailType == "Update")
                {
                    emailTemplate = await GetActiveEmailTemplate("user.UpdateNotificationForOperation");
                }

                if (emailTemplate == null)
                {
                    return false;
                }

                var tokens = new List<EmailToken>();
                _emailTokenProvider.NewUserCreateUpdateNotificationForOperationAllUsers(tokens, newUserNotificatonViewModel);
                await SendNotification(emailTemplate, tokens, newUserNotificatonViewModel.ContactId, newUserNotificatonViewModel.ToEmail, newUserNotificatonViewModel.ToEmailName);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> SendCompanyUsernameAndPassword(NewCompanySignUpNotification newCompanySignUpNotification)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (newCompanySignUpNotification.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("company.SendUsernameAndPasswordForRegisterCompany");
                }
                if (newCompanySignUpNotification.MailType == "Update")
                {
                    emailTemplate = await GetActiveEmailTemplate("company.SendEmailForUpdateCompanyInfo");
                }

                if (emailTemplate == null)
                {
                    return false;
                }
             
                var tokens = new List<EmailToken>();
                
                _emailTokenProvider.SendEmailForCompanyEdit(tokens, newCompanySignUpNotification);
                await SendNotification(emailTemplate, tokens, newCompanySignUpNotification.ContactId, newCompanySignUpNotification.ToEmail, newCompanySignUpNotification.ToEmailName);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> PasswordReset(EmailUserViewModel model)
        {
            try
            {
                EmailTemplateModel emailTemplate = new EmailTemplateModel();

                if (model.MailType == "Add")
                {
                    emailTemplate = await GetActiveEmailTemplate("password.ResetPassword");
                }

                if (emailTemplate == null)
                {
                    return false;
                }
                var loginContact = await _contactManager.GetById(model.ContactId);
                var toEmail = loginContact.Email;
                var toName = loginContact.FirstName + loginContact.LastName;
                var tokens = new List<EmailToken>();
                var userViewModel = new EmailUserViewModel
                {
                    User = model.User,
                    DetailUrl = model.DetailUrl,
                    LoginContactName = loginContact.FirstName,
                    CompanyName = model.CompanyName,
                    Email= toEmail,
                };
                _emailTokenProvider.SendEmailForPasswordReset(tokens, userViewModel);
                await SendNotification(emailTemplate, tokens, model.ContactId, toEmail, toName);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        public async Task SendEmailToOpsToNotifyOrderUpload(string batchName, string orderNumber, CompanyModel company, string folderName = " ", string userName = " ", int numberOfImages = 0)
        {

            try
            {

                //ToDo: Rakib need to add from db
                List<string> opsEmailList = new List<string>()
                            {
                                "rakibul@thekowcompany.com",
								//"anik@thekowcompany.com",
								//"zico@thekowcompany.com",
								//"raihan@thekowcompany.com",
								"ops@thekowcompany.com",
                                "mashfiq@thekowcompany.com",
								//"ak@thekowcompany.com",
								"zakir@thekowcompany.com"
                            };
                FTPOrderNotifyOpsOnImageArrivalFTP fTPOrderNotifyOpsOnImageArrivalFTP = new FTPOrderNotifyOpsOnImageArrivalFTP
                {
                    EmailAddresses = opsEmailList,
                    MailType = "OrderPlaceOnKTM",
                    //ImageCount = $"{imageCount}  ,  UserName:{clientFtp.Username}",

                    //OrderType = $"NAN",
                    CompanyName = $"{company.Name}",
                    BatchName = $"{batchName} ,Folder Name:{folderName}   , Username:{userName}",
                    OrderNumber = $"{orderNumber} , Image Count: {(numberOfImages > 0 ? numberOfImages.ToString() : "N/A")}",
                };
                await SendEmailToOpsToNotifyOrderUpload(fTPOrderNotifyOpsOnImageArrivalFTP);
            }
            catch (Exception ex)
            {
                var loginUser = new LoginUserInfoViewModel
                {
                    ContactId = AutomatedAppConstant.ContactId
                };
                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    //PrimaryId = (int)order.Id,
                    ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message,
                    MethodName = "SendEmailToOpsToNotifyOrderUpload",
                    RazorPage = "FtpOrderProcessService",
                    Category = (int)ActivityLogCategory.NotifyOpsOnImageArrivalFTP,
                };
                //await _activityAppLogService.InsertAppErrorActivityLog(activity);
            }


        }
        public async Task<bool> SendOrderPlacementEmailsToClientCompany(OrderPlacementNotificationEmailModel orderPlacementNotificationEmailModel)
        {
            try
            {
                var emailTemplate = await GetActiveEmailTemplate(EmailTemplateConstants.NotificationOrderPlacementEmailSendToClientCompany);
                if (emailTemplate == null)
                    return false;
                if (orderPlacementNotificationEmailModel == null)
                    return false;
                foreach (var contact in orderPlacementNotificationEmailModel.Contacts)
                {
                    var toEmail = contact.Email;
                    var toName = contact.FirstName + contact.LastName;
                    var tokens = new List<EmailToken>();
                    OrderPlacementNotificationTokens orderPlacementNotificationTokens = new OrderPlacementNotificationTokens
                    {
                        OrderNumber = orderPlacementNotificationEmailModel.OrderNumber,
                        ImageCount = orderPlacementNotificationEmailModel.ImageCount,
                        OrderDetailURL = orderPlacementNotificationEmailModel?.OrderDetailURL,
                    };
                    _emailTokenProvider.InitializeOrderPlacementEmailTokens(tokens, orderPlacementNotificationTokens);
                    await SendNotification(emailTemplate, tokens, orderPlacementNotificationEmailModel.CreatedByContactId, toEmail, toName);
                }
            }

            catch (Exception ex)
            {
                return false;
            }
            return true;

        }


    }
}
