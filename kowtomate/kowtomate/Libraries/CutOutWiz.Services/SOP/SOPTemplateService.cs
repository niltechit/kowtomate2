using CutOutWiz.Data;
using CutOutWiz.Data.SOP;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.SOP
{
    public class SOPTemplateService : ISOPTemplateService
    {
        private readonly ISqlDataAccess _db;

        public SOPTemplateService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Templates
        /// </summary>
        /// <returns></returns>
        public async Task<List<SOPTemplate>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<SOPTemplate, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetAll", new { });
        }

        /// <summary>
        /// Get template by template Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<SOPTemplate> GetById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplate, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetById", new { TemplateId = templateId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<SOPTemplate> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplate, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(SOPTemplate template)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_Template_Insert", new
                {
                    template.Name,
                    template.CompanyId,
                    template.FileServerId,
                    template.Version,
                    template.ParentTemplateId,
                    template.Instruction,
                    template.UnitPrice,
                    template.InstructionModifiedByContactId,
                    template.Status,
                    template.IsDeleted,
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
        public async Task<Response<bool>> Update(SOPTemplate template)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_Template_Update", new
                {
                    template.Id,
                    template.Name,
                    template.CompanyId,
                    template.FileServerId,
                    template.Version,
                    template.ParentTemplateId,
                    template.Instruction,
                    template.UnitPrice,
                    template.InstructionModifiedByContactId,
                    template.Status,
                    template.IsDeleted,
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
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_Template_Delete", new { ObjectId = objectId });
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
