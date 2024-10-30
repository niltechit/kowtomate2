using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClientOrders
{
	public interface IOrderTemplateService
	{
        Task<Response<bool>> Delete(int Id);
        Task<List<ClientOrderSOPTemplateModel>> GetAllByOrderId(int orderId);
        Task<List<ClientOrderSOPTemplateModel>> GetAll();
        Task<ClientOrderSOPTemplateModel> GetById(int OrderId);
        Task<ClientOrderSOPTemplateModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(ClientOrderSOPTemplateModel orderTemplate);
        Task<Response<bool>> Update(ClientOrderSOPTemplateModel orderTemplate);
        Task InsertList(List<ClientOrderSOPTemplateModel> orderTemplates, int orderId);
    }
}
