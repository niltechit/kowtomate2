using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.OrderItemStatusChangeLogService;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Data;

namespace CutOutWiz.Services.MessageService
{
    public class OrderItemStatusChangeLogService : IOrderItemStatusChangeLogService
    {
        private readonly ISqlDataAccess _db;

        public OrderItemStatusChangeLogService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All OrderItemStatusChangeLogs
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderItemStatusChangeLogModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<OrderItemStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_OrderItemStatusChangeLog_GetAll", new { });
        }

        /// <summary>
        /// Get company by company Id
        /// </summary>
        /// <param name="OrderItemStatusChangeLogId"></param>
        /// <returns></returns>
        public async Task<OrderItemStatusChangeLogModel> GetById(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderItemStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_OrderItemStatusChangeLog_GetById", new { OrderItemStatusChangeLogId = companyId });
            return result.FirstOrDefault();
        }
        public async Task<OrderItemStatusChangeLogModel> GetTeamByOrderItemStatusChangeLogId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderItemStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_OrderItemStatusChangeLog_GetTeamByOrderItemStatusChangeLogId", new { OrderItemStatusChangeLogId = companyId });
            return result.FirstOrDefault();
        }
        public async Task<OrderItemStatusChangeLogModel> OrderItemStatusLastChangeLogByOrderFileId(int orderFileId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderItemStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_Order_OrderItemStatusChangeLog_GetLastLogByOrderItemId", new { orderFileId = orderFileId });
            return result.FirstOrDefault();
        }
        /// <summary>
        /// Insert OrderItemStatusChangeLog
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Core.Response<int>> Insert(OrderItemStatusChangeLogModel orderItemStatusChangeLog)
        {
            var response = new Core.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Order_OrderItemStatusChangeLog_Insert", new
                {
                    orderItemStatusChangeLog.OrderFileId,
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

			await InsertOrderItemHistory(orderItemStatusChangeLog.OrderFileId, (byte)orderItemStatusChangeLog.NewInternalStatus);

			return response;
        }

        private async Task InsertOrderItemHistory(long OrderItemId,byte status)
        {
            try
            {
                string query = "";

				if ((byte)InternalOrderItemStatus.OrderPlaced == status)
                {
					query = $"INSERT INTO dbo.Order_ClientOrderItemHistory (ClientOrderItemId, {(InternalOrderItemStatus)status}Date) VALUES (@OrderItemId, @ChangeDate)";
					
				}
                else
                {
					query = $"Update dbo.Order_ClientOrderItemHistory set {(InternalOrderItemStatus)status}Date = @ChangeDate where ClientOrderItemId=@OrderItemId";
				}
				
				await _db.SaveDataUsingQuery<dynamic>(query, new { ChangeDate = DateTime.Now, OrderItemId = OrderItemId });
			}
            catch (Exception ex)
            {
                // Time will Create it;
            }
		}

		public async Task<List<OrderItemStatusChangeLogModel>> GetByOrderItemId(int orderItemId)
        {
            return await _db.LoadDataUsingProcedure<OrderItemStatusChangeLogModel, dynamic>(storedProcedure: "dbo.SP_OrderItemStatusChangeLog_GetByOrderId", new { OrderItemId = orderItemId });
        }

    }


}
