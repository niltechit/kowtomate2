using CutOutWiz.Core;
using CutOutWiz.Core.Message;
using CutOutWiz.Data;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.InternalMessage;

namespace CutOutWiz.Services.InternalMessage
{
    public class InternalMessageTemplateService : IInternalMessageTemplateService
    {
        private readonly ISqlDataAccess _db;

        public InternalMessageTemplateService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Templates
        /// </summary>
        /// <returns></returns>
        public async Task<List<InternalMessageTemplate>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<InternalMessageTemplate, dynamic>(storedProcedure: "dbo.SP_Message_Template_GetAll", new { });
        }

        /// <summary>
        /// Get template by template Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<InternalMessageTemplate> GetById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<InternalMessageTemplate, dynamic>(storedProcedure: "dbo.SP_Message_Template_GetById", new { TemplateId = templateId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<InternalMessageTemplate> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<InternalMessageTemplate, dynamic>(storedProcedure: "dbo.SP_Message_Template_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get template by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<InternalMessageTemplate> GetByAccessCode(string accessCode)
        {
            var result = await _db.LoadDataUsingProcedure<InternalMessageTemplate, dynamic>(storedProcedure: "dbo.SP_Message_Template_GetAccessCode", new { AccessCode = accessCode });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(InternalMessageTemplate template)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Message_Template_Insert", new
                {
                    template.SenderAccountId,
                    template.Name,
                    template.AccessCode,
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
        public async Task<Response<bool>> Update(InternalMessageTemplate template)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Message_Template_Update", new
                {
                    template.Id,
                    template.SenderAccountId,
                    template.Name,
                    template.AccessCode,
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
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Message_Template_Delete", new { ObjectId = objectId });
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
