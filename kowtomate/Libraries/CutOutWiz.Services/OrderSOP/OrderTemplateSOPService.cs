using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Services.DbAccess;
using Mailjet.Client.Resources;
using CutOutWiz.Data;

namespace CutOutWiz.Services.OrderSOP
{
    public class OrderTemplateSOPService:IOrderTemplateSOPService
    {
        private readonly ISqlDataAccess _db;
        public OrderTemplateSOPService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Templates
        /// </summary>
        /// <returns></returns>

        public async Task<List<OrderSOPTemplateModel>> GetAllById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderSOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_Template_GetById", new { TemplateId = templateId });
            return result;
        }
        /// <summary>
        /// Get template by template Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<OrderSOPTemplateModel> GetById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderSOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_Template_GetById", new { TemplateId = templateId });
            return result.FirstOrDefault();
        }
        public async Task<OrderSOPTemplateModel> GetByIdAndIsDeletedFalse(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderSOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_Template_GetByIdAndIsDeletedFalse", new { TemplateId = templateId });
            return result.FirstOrDefault();
        }


        /// <summary>
        /// Insert template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(OrderSOPTemplateModel template)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_OrderSOP_Template_Insert", new
                {
                    template.BaseTemplateId,
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

                if (newId == 0)
                {
                    response.Message = StandardDataAccessMessages.ErrorOnAddMessaage;
                    return response;
                }
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
        /// Delete Template by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(OrderSOPTemplateModel orderSOPTemplate)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_OrderSOP_DeleteById", new { Id = orderSOPTemplate.Id,IsDeleted= orderSOPTemplate.IsDeleted });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> UpdateSOPTemplateName(OrderSOPTemplateModel orderSOPtemplate)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_OrderSOP_Template_UpdateForName", new
                {
                    orderSOPtemplate.Id,
                    orderSOPtemplate.Name,
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

		public async Task<Response<bool>> UpdateOrderSOPTemplateInstruction(OrderSOPTemplateModel orderSOPtemplate)
		{
			var response = new Response<bool>();

			try
			{
				await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_OrderSOP_Template_UpdateForInstruction", new
				{
					orderSOPtemplate.Id,
					orderSOPtemplate.Name,
                    orderSOPtemplate.Instruction,
                    orderSOPtemplate.UpdatedByContactId
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
	}
}
