using CutOutWiz.Data;
using CutOutWiz.Data.Email;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.Email
{
    public class EmailSenderAccountService : IEmailSenderAccountService
    {
        private readonly ISqlDataAccess _db;

        public EmailSenderAccountService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All SenderAccounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmailSenderAccount>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<EmailSenderAccount, dynamic>(storedProcedure: "dbo.SP_Email_SenderAccount_GetAll", new { });
        }

        /// <summary>
        /// Get senderAccount by senderAccount Id
        /// </summary>
        /// <param name="SenderAccountId"></param>
        /// <returns></returns>
        public async Task<EmailSenderAccount> GetById(int senderAccountId)
        {
            var result = await _db.LoadDataUsingProcedure<EmailSenderAccount, dynamic>(storedProcedure: "dbo.SP_Email_SenderAccount_GetById", new { SenderAccountId = senderAccountId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="SenderAccountId"></param>
        /// <returns></returns>
        public async Task<EmailSenderAccount> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<EmailSenderAccount, dynamic>(storedProcedure: "dbo.SP_Email_SenderAccount_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get Default Account
        /// </summary>
        /// <param name="SenderAccountId"></param>
        /// <returns></returns>
        public async Task<EmailSenderAccount> GetDefaultAccount()
        {
            var result = await _db.LoadDataUsingProcedure<EmailSenderAccount, dynamic>(storedProcedure: "dbo.SP_Email_SenderAccount_GetDefaultAccount", new { });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert senderAccount
        /// </summary>
        /// <param name="senderAccount"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(EmailSenderAccount senderAccount)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Email_SenderAccount_Insert", new
                {
                    senderAccount.Name,
                    senderAccount.Email,
                    senderAccount.EmailDisplayName,
                    senderAccount.MailServer,
                    senderAccount.Port,
                    senderAccount.UserName,
                    senderAccount.Password,
                    senderAccount.ApiKey,
                    senderAccount.SecretKey,
                    senderAccount.Domain,
                    senderAccount.EnableSSL,
                    senderAccount.UseDefaultCredentials,
                    senderAccount.IsDefault,
                    senderAccount.Status,
                    senderAccount.CreatedByContactId,
                    senderAccount.ObjectId
                });

                senderAccount.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update SenderAccount
        /// </summary>
        /// <param name="senderAccount"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(EmailSenderAccount senderAccount)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Email_SenderAccount_Update", new
                {
                    senderAccount.Id,
                    senderAccount.Name,

                    senderAccount.Email,
                    senderAccount.EmailDisplayName,
                    senderAccount.MailServer,
                    senderAccount.Port,
                    senderAccount.UserName,
                    senderAccount.Password,
                    senderAccount.ApiKey,
                    senderAccount.SecretKey,
                    senderAccount.Domain,
                    senderAccount.EnableSSL,
                    senderAccount.UseDefaultCredentials,
                    senderAccount.IsDefault,
                    senderAccount.Status,
                    senderAccount.UpdatedByContactId
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete SenderAccount by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Email_SenderAccount_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

    }
}
