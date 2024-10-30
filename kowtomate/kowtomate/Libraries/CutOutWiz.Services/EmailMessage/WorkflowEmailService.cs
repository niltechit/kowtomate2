using CutOutWiz.Data.Email;
using CutOutWiz.Data.EmailModels;
using CutOutWiz.Data.EmailSender;
using CutOutWiz.Services.Email;
using CutOutWiz.Services.EmailSender;
using CutOutWiz.Services.LogService;

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
                                    IMailjetEmailService mailjetEmailService)
        {
            //_queuedEmailService = queuedEmailService;
            _emailTemplateService = emailTemplateService;
            _logService = logService;
            _emailTokenizer = emailTokenizer;
            _emailSenderAccountService = emailSenderAccountService;
            _emailTokenProvider = emailTokenProvider;
            _mailjetEmailService = mailjetEmailService;
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

            return await SendNotification(emailTemplate, tokens, toEmail, toName);
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

            return await SendNotification(emailTemplate, tokens, toEmail, toName);
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
                    EmailTemplate emailTemplate,
                    IEnumerable<EmailToken> tokens,
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

                // Get this email info from template if available
                if (!string.IsNullOrEmpty(emailTemplate.FromEmailAddress))
                    fromEmail = emailTemplate.FromEmailAddress;

                //if (!string.IsNullOrEmpty(emailTemplate.FromEmailName))
                //    fromEmailName = emailTemplate.FromEmailName;

                //var queuedEmail = new EmailQueueModel
                //{
                //    EmailAccountId = emailAccount.Id,
                //    SmtpHostName = emailAccount.Name,
                //    Priority = EmailQueuePriority.High,
                //    FromEmail = fromEmail,
                //    FromEmailName = fromEmailName,
                //    ToEmail = toEmailAddress,
                //    ToEmailName = toName,
                //    ReplyToEmail = replyToEmailAddress,
                //    ReplyToEmailName = replyToName,
                //    CcEmail = string.Empty,
                //    BccEmail = emailTemplate.BccEmailAddresses,
                //    Subject = subjectReplaced,
                //    Body = bodyReplaced,
                //    AttachmentFilePath = attachmentFilePath,
                //    AttachmentFileName = attachmentFileName,
                //    CreatedDateUtc = DateTime.UtcNow,
                //    EmailAccount = emailAccount,
                //    SentTries = 0,
                //    Status = (int)EmailQueueStatus.NotSent
                //};

                //_queuedEmailService.Add(queuedEmail);
                var emailSendRequestModel = new EmailSendRequestModel
                {
                    ApiKey = emailAccount.ApiKey,
                    ApiSecret = emailAccount.SecretKey,

                    FromEmail = fromEmail,
                    ToEmail = toEmailAddress,

                    Subject = subjectReplaced,
                    Body = bodyReplaced
                };

                await _mailjetEmailService.SendEmail(emailSendRequestModel);

                // if succes add a record in Email_Archive Table
                // if failed add a record in email queue table with status not sent

            }
            catch (Exception ex)
            {
                //LogWorkflowEmailServiceExceptions(ex, "SendNotification()");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the email template and make sure it's active
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        protected async Task<EmailTemplate> GetActiveEmailTemplate(string templateName)
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
    }
}
