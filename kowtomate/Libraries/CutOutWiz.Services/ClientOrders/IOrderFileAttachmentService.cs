using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClientOrders
{
	public interface IOrderFileAttachmentService
    {
        Task<Response<int>> Insert(List<OrderFileAttachment> orderAttachments, int orderId);
        Task<Response<bool>> Delete(int attachmentId);
        Task<List<OrderFileAttachment>> GetOrdersAttachementByOrderId(int orderId);
        Task<List<OrderFileAttachment>> GetOrdersAttachementById(int Id);
    }
}
