using CutOutWiz.Services.Models.OrderSOP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.OrderSOP
{
	public interface IOrderSOPTemplateJoiningService
	{
		Task<List<Order_ClientOrder_SOP_TemplateModel>> GetByClientOrderId(int clientOrderId);
	}
}
