using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.ClientOrders
{
    public interface IClientOrderService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<Response<bool>> DeleteOrderSopTemplateByOrderIdAndSopTemplateId(long orderId, int sopTemplateId);
        Task<List<ClientOrderModel>> GetAll();
        Task<List<ClientOrderModel>> GetAllByCompanyId(int companyId);
        //Task<List<ClientOrderListModel>> GetAllOrderByFilterWithPaging(ClientOrderFilter filter);
        //Task<List<ClientOrderListModel>> GetAllOrderByFilterWithPagingForTeam(ClientOrderFilter filter);
        Task<ClientOrderModel> GetById(long OrderId);
        Task<ClientOrderModel> GetByObjectId(string objectId);
        Task<List<ClientOrderListModel>> GetOrderByFilterWithPaging(ClientOrderFilter filter);
        Task<List<ClientOrderListModel>> GetOrderByFilterWithoutPaging(ClientOrderFilter filter);
        Task<Response<int>> Insert(ClientOrderModel Order);
        Task<Response<bool>> Update(ClientOrderModel Order);
        Task<Response<bool>> UpdateClientOrderListModel(ClientOrderListModel Order);
        Task<Response<bool>> UpdateClientOrderOpsAndUpdateByListModel(ClientOrderListModel Order);
        Task<Response<bool>> UpdateClientOrderStatus(ClientOrderModel Order);
        Task<Response<bool>> UpdateSingleField(long orderId, string fieldName, string fieldValue, int updatedByContactId);
        Task<Response<bool>> UpdateOrderAllowExtraOutputFileUploadField(ClientOrderModel clientOrder);
        Task<Response<bool>> UpdateClientOrder(ClientOrderModel order);
        Task<List<ClientOrderModel>> GetOrdersByOrderItemStatus(int companyId, string status);
        Task<List<ClientOrderModel>> GetAllByStatus(int status, int companyId);
        Task<ClientOrderModel> GetByOrderNumber (string orderNumber);
        List<CustomTableColumnModel> GetAllTableColumns();
        Task<Response<bool>> UpdateClientOrderArrivalTime(ClientOrderModel Order);
        Task UpdateOrder(ClientOrderModel clientOrder, InternalOrderStatus status);
        Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus, LoginUserInfoViewModel loginUser);
        Task<Response<bool>> UpdateOrderDeadline(ClientOrderModel Order);
        Task<bool> CheckExistenceOfBatchBySourceFullPath(string sourceFullPath);
        Task<Response<bool>> UpdateOrderDeadlineDate(long clientOrderId);
        Task<List<ClientOrderModel>> GetAllByCompanyIdAndDates(int companyId, DateTime startDate, DateTime endDate);
        Task<bool> CheckBatchNameExistenceOnOrderPlacingStatus(string batchName);
        Task<Response<bool>> UpdateClientOrderCatgegorySetStatus(ClientOrderModel Order);

        Task<Response<ClientOrderModel>> CheckBatchNameExistOnClientOrder(int companyId, string batchPath);
    }
}