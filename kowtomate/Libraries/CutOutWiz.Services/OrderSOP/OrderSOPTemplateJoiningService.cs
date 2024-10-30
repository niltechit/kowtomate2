using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.OrderSOP
{
	public class OrderSOPTemplateJoiningService : IOrderSOPTemplateJoiningService
	{
		private readonly ISqlDataAccess _db;

		public OrderSOPTemplateJoiningService(ISqlDataAccess db)
		{
			_db = db;
		}
		public async Task<List<Order_ClientOrder_SOP_TemplateModel>> GetByClientOrderId(int clientOrderId)
		{
			return await _db.LoadDataUsingProcedure<Order_ClientOrder_SOP_TemplateModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_OrderSOP_Template_GetByClientOrderId", new { Order_ClientOrder_Id = clientOrderId });
		}
	}
}
