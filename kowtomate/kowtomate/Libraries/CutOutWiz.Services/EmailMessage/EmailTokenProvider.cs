using CutOutWiz.Data.EmailModels;

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

        #endregion
    }
}
