using Amazon.S3.Model;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.ClientOrders
{
	public class OrderTemplateService : IOrderTemplateService
	{
		private readonly ISqlDataAccess _db;

		public OrderTemplateService(ISqlDataAccess db)
		{
			_db = db;
		}
        public async Task<Response<bool>> Delete(int objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_DeleteByOrderId", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<List<ClientOrderSOPTemplateModel>> GetAllByOrderId(int orderId)
		{
			return await _db.LoadDataUsingProcedure<ClientOrderSOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_GetByOrderId", new { Orderid=orderId});
		}
        public async Task<List<ClientOrderSOPTemplateModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<ClientOrderSOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_GetAll", new { });
        }

        public Task<ClientOrderSOPTemplateModel> GetById(int OrderId)
		{
			throw new NotImplementedException();
		}

		public Task<ClientOrderSOPTemplateModel> GetByObjectId(string objectId)
		{
			throw new NotImplementedException();
		}
        public async Task<Response<int>> Insert(ClientOrderSOPTemplateModel orderTemplate)
        {
            var response = new Response<int>();
            try
            {
                // Add Order_ClientOrder
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_Insert", new
                {
                    orderTemplate.Order_ClientOrder_Id,
                    orderTemplate.SOP_Template_Id,
                    orderTemplate.OrderSOP_Template_Id
                });

                orderTemplate.Id = newId;
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
        public Task<Response<bool>> Update(ClientOrderSOPTemplateModel orderTemplate)
		{
			throw new NotImplementedException();
		}
        public async Task InsertList(List<ClientOrderSOPTemplateModel> orderTemplate,int orderId)
		{
            if (orderTemplate != null && orderTemplate.Any())
            {
                foreach (var soptemplate in orderTemplate)
                {
                    soptemplate.Order_ClientOrder_Id = orderId;
                    await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "[dbo].[SP_Order_ClientOrder_SOP_Template_Insert]", new
                    {
                        soptemplate.Order_ClientOrder_Id,
                        soptemplate.SOP_Template_Id,
                    });
                }
            }
        }
	}
}
