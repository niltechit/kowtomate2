using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;


namespace CutOutWiz.Services.OrderAndOrderItemStatusChangeLogServices
{
    public class OrderStatusChangeLogService : IOrderStatusChangeLogService
    {
        private readonly ISqlDataAccess _db;

        public OrderStatusChangeLogService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All OrderStatusChangeLogs
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderStatusChangeLogModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<OrderStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_Common_OrderStatusChangeLog_GetAll", new { });
        }

        /// <summary>
        /// Get company by company Id
        /// </summary>
        /// <param name="OrderStatusChangeLogId"></param>
        /// <returns></returns>
        public async Task<OrderStatusChangeLogModel> GetById(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_Common_OrderStatusChangeLog_GetById", new { OrderStatusChangeLogId = companyId });
            return result.FirstOrDefault();
        }
        public async Task<OrderStatusChangeLogModel> GetTeamByOrderStatusChangeLogId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_Common_OrderStatusChangeLog_GetTeamByOrderStatusChangeLogId", new { OrderStatusChangeLogId = companyId });
            return result.FirstOrDefault();
        }
        public async Task<OrderStatusChangeLogModel> OrderStatusLastChangeLogByOrderId(int orderId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_Order_OrderStatusChangeLog_GetLastLogByOrderId", new { OrderId = orderId });
            return result.FirstOrDefault();
        }
        /// <summary>
        /// Insert OrderStatusChangeLog
        /// </summary>
        /// <param name="orderstatuschangelog"></param>
        /// <returns></returns>
        public async Task<Core.Response<int>> Insert(OrderStatusChangeLogModel orderItemStatusChangeLog)
        {
            var response = new Core.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Order_OrderStatusChangeLog_Insert", new
                    {
                        orderItemStatusChangeLog.OrderId,
                        orderItemStatusChangeLog.OldInternalStatus,
                        orderItemStatusChangeLog.NewInternalStatus,
                        orderItemStatusChangeLog.OldExternalStatus,
                        orderItemStatusChangeLog.NewExternalStatus,
                        orderItemStatusChangeLog.Note,
                        orderItemStatusChangeLog.ChangeByContactId,
                        orderItemStatusChangeLog.TimeDurationInMinutes,
                        orderItemStatusChangeLog.ChangeDate
                   });

                orderItemStatusChangeLog.Id = newId;
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

        public async Task<List<OrderStatusChangeLogModel>> GetByOrderId(int orderId)
        {
            return await _db.LoadDataUsingProcedure<OrderStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_OrderStatusChangeLog_GetByOrderId", new {OrderId = orderId});
        }

    }


}
