using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.OrderSOP
{
    public class OrderSOPTemplateOrderSOPStandardService : IOrderSOPTemplateOrderSOPStandardService
    {
        private readonly ISqlDataAccess _db;

        public OrderSOPTemplateOrderSOPStandardService(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<Response<int>> Insert(OrderSOPTemplateServiceModel orderSOPtemplate)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_OrderSOP_TemplateService_Insert", new
                {
                    orderSOPtemplate.OrderSOPTemplateId,
                    orderSOPtemplate.OrderSOPStandardServiceId,
                    orderSOPtemplate.Status,
                    orderSOPtemplate.IsDeleted,
                    orderSOPtemplate.CreatedByContactId,
                    orderSOPtemplate.ObjectId,
                    orderSOPtemplate.BaseTemplateId,
                    orderSOPtemplate.BaseSOPServiceId,
                });

                orderSOPtemplate.Id = newId;
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
		public async Task<OrderSOPStandardServiceModel> GetByOrderSOPName(string orderSOPName)
		{
			var result = await _db.LoadDataUsingProcedure<OrderSOPStandardServiceModel, dynamic>(storedProcedure: "dbo.SP_OrderSOP_StandardService_GetByOrderSOPName", new { Name = orderSOPName });
			return result.FirstOrDefault();
		}
	}
}
