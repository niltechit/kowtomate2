using CutOutWiz.Data;
using CutOutWiz.Data.Email;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.Email
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly ISqlDataAccess _db;

        public EmailTemplateService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Templates
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmailTemplate>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<EmailTemplate, dynamic>(storedProcedure: "dbo.SP_Email_Template_GetAll", new { });
        }

        /// <summary>
        /// Get template by template Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<EmailTemplate> GetById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<EmailTemplate, dynamic>(storedProcedure: "dbo.SP_Email_Template_GetById", new { TemplateId = templateId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<EmailTemplate> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<EmailTemplate, dynamic>(storedProcedure: "dbo.SP_Email_Template_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get template by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<EmailTemplate> GetByAccessCode(string accessCode)
        {
            var result = await _db.LoadDataUsingProcedure<EmailTemplate, dynamic>(storedProcedure: "dbo.SP_Email_Template_GetAccessCode", new { AccessCode = accessCode });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(EmailTemplate template)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Email_Template_Insert", new
                {
                    template.SenderAccountId,
                    template.Name,
                    template.AccessCode,
                    template.FromEmailAddress,
                    template.BCCEmailAddresses,
                    template.Subject,
                    template.Body,
                    template.Status,
                    template.CreatedByContactId,
                    template.ObjectId,
                });

                template.Id = newId;
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
        /// Update Template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(EmailTemplate template)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Email_Template_Update", new
                {
                    template.Id,
                    template.SenderAccountId,
                    template.Name,
                    template.AccessCode,
                    template.FromEmailAddress,
                    template.BCCEmailAddresses,
                    template.Subject,
                    template.Body,
                    template.Status,
                    template.UpdatedByContactId
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
        /// Delete Template by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Email_Template_Delete", new { ObjectId = objectId });
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
