using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.OrderSOP
{
    public interface IOrderSOPTemplateOrderSOPStandardService
    {
        Task<Response<int>> Insert(OrderSOPTemplateServiceModel orderSOPtemplate);
		Task<OrderSOPStandardServiceModel> GetByOrderSOPName(string orderSOPName);
	}
}
