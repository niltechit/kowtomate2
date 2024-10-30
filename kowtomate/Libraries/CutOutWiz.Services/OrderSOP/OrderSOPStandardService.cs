using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.OrderSOP
{
    public class OrderSOPStandardService : IOrderSOPStandardService
    {
        private readonly ISqlDataAccess _db;

        public OrderSOPStandardService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All StandardServices
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderSOPStandardServiceModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<OrderSOPStandardServiceModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_StandardService_GetAll", new { });
        }

        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public async Task<List<OrderSOPStandardServiceModel>> GetListByOrderTemplateId(int templateId)
        {
            return await _db.LoadDataUsingProcedure<OrderSOPStandardServiceModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_StandardService_GetListByOrderTemplateId", new { orderTemplateId = templateId });
        }

        /// <summary>
        /// Get standardService by standardService Id
        /// </summary>
        /// <param name="StandardServiceId"></param>
        /// <returns></returns>
        public async Task<OrderSOPStandardService> GetById(int standardServiceId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderSOPStandardService, dynamic>(storedProcedure: "dbo.SP_SOP_StandardService_GetById", new { StandardServiceId = standardServiceId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="StandardServiceId"></param>
        /// <returns></returns>
        public async Task<SOPStandardServiceModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPStandardServiceModel, dynamic>(storedProcedure: "dbo.SP_SOP_StandardService_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert standardService
        /// </summary>
        /// <param name="standardService"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(OrderSOPStandardServiceModel orderStandardService)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_OrderSOP_StandardService_Insert", new
                {
                    orderStandardService.Name,
                    orderStandardService.SortOrder,
                    orderStandardService.Status,
                    orderStandardService.IsDeleted,
                    orderStandardService.CreatedByContactId,
                    orderStandardService.ObjectId,
                    orderStandardService.ParentSopServiceId,
                    orderStandardService.BaseSOPServiceId,
                });

                orderStandardService.Id = newId;
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
        /// Update StandardService
        /// </summary>
        /// <param name="standardService"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(SOPStandardServiceModel standardService)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_StandardService_Update", new
                {
                    standardService.Id,
                    standardService.Name,
                    standardService.SortOrder,
                    standardService.Status,
                    standardService.IsDeleted,
                    standardService.UpdatedByContactId
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
        /// Delete StandardService by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_StandardService_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<List<SOPTemplateServiceModel>> GetTemplateServiceByTemplateId(int SopTemplateId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplateServiceModel, dynamic>(storedProcedure: "dbo.SP_SOP_TemplateService_TemplateId", new { SopTemplateId = SopTemplateId.ToString() });
            return result.ToList();
        }
		public async Task<OrderSOPStandardServiceModel> GetByOrderSOPName(string orderSOPName)
		{
			var result = await _db.LoadDataUsingProcedure<OrderSOPStandardServiceModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_StandardService_GetByOrderSOPName", new { Name = orderSOPName });
			return result.FirstOrDefault();
		}

	}
}
