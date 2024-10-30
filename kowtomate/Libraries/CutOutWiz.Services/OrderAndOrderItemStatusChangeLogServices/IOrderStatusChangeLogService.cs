using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;

namespace CutOutWiz.Services.OrderAndOrderItemStatusChangeLogServices
{
    public interface IOrderStatusChangeLogService
    {
        Task<List<OrderStatusChangeLogModel>> GetAll();
        Task<OrderStatusChangeLogModel> GetById(int companyId);
        Task<OrderStatusChangeLogModel> OrderStatusLastChangeLogByOrderId(int orderId);
        Task<OrderStatusChangeLogModel> GetTeamByOrderStatusChangeLogId(int companyId);
        Task<Response<int>> Insert(OrderStatusChangeLogModel orderItemStatusChangeLog);
        Task<List<OrderStatusChangeLogModel>> GetByOrderId(int orderId);
    }
}