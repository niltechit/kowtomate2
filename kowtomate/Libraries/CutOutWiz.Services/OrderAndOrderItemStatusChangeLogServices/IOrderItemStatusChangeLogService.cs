using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;

namespace CutOutWiz.Services.OrderItemStatusChangeLogService
{
    public interface IOrderItemStatusChangeLogService
    {
        Task<List<OrderItemStatusChangeLogModel>> GetAll();
        Task<OrderItemStatusChangeLogModel> GetById(int companyId);
        Task<OrderItemStatusChangeLogModel> OrderItemStatusLastChangeLogByOrderFileId(int orderFileId);
        Task<OrderItemStatusChangeLogModel> GetTeamByOrderItemStatusChangeLogId(int companyId);
        Task<Response<int>> Insert(OrderItemStatusChangeLogModel orderItemStatusChangeLog);
        Task<List<OrderItemStatusChangeLogModel>> GetByOrderItemId(int orderItemId);
    }
}