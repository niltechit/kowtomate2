using Amazon.S3.Model;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.BLL.UpdateOrderItem;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.OrderItemStatusChangeLogService;
using CutOutWiz.Services.OrderAndOrderItemStatusChangeLogServices;
using Microsoft.AspNetCore.Http;
using static CutOutWiz.Core.Utilities.Enums;
using static System.Net.WebRequestMethods;
using CutOutWiz.Data;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.ClientOrders
{
    public class ClientOrderService : IClientOrderService
    {
        private readonly ISqlDataAccess _db;
        private readonly IOrderTemplateService _templateService;
        private readonly ICompanyTeamManager _companyTeamService;
        private readonly IOrderStatusChangeLogService _orderStatusChangeLogService;

        public ClientOrderService(ISqlDataAccess db, IOrderTemplateService templateService, ICompanyTeamManager companyTeamService,
            IOrderStatusChangeLogService orderStatusChangeLogService)
        {
            _db = db;
            _templateService = templateService;
            _companyTeamService = companyTeamService;
            _orderStatusChangeLogService = orderStatusChangeLogService;

        }

        #region Client Order

        /// <summary>
        /// Get All Order_ClientOrder
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClientOrderModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_GetAll", new { });
        }

        public async Task<ClientOrderModel> GetByOrderNumber(string orderNumber)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_GetByOrderNumber", new { orderNumber = orderNumber });
            return result.FirstOrDefault();

        }

        /// <summary>
        /// Get Orders by filter
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClientOrderListModel>> GetOrderByFilterWithPaging(ClientOrderFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.SortColumn) || filter.SortColumn == "o.[OrderPlaceDateOnly]")
                filter.SortColumn = "o.[OrderPlaceDate]";

            if (string.IsNullOrWhiteSpace(filter.SortDirection))
                filter.SortDirection = "DESC";

            try
            {
                var queryString = String.Format("dbo.SP_Order_ClientOrder_GetListByFilter '{0}','{1}',{2},{3},'{4}','{5}'",
              filter.Where,
              filter.IsCalculateTotal.ToString().ToLower(),
              filter.Skip,
              filter.Top,
              filter.SortColumn,
              filter.SortDirection
              );

                var filteredList = await _db.LoadDataUsingQuery<ClientOrderListModel, dynamic>(queryString,
                    new
                    {
                    });

                if (filteredList.Any() && filter.IsCalculateTotal)
                {
                    filter.TotalCount = filteredList[0].TotalCount;
                    filter.TotalImageCount = filteredList[0].TotalImageCount;
                }
                else if (!filteredList.Any() && filter.IsCalculateTotal)
                {
                    filter.TotalCount = 0;
                    filter.TotalImageCount = 0;
                }

                return filteredList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return null;
        }


        /// <summary>
		/// Get Orders by filter
		/// </summary>
		/// <returns></returns>
		public async Task<List<ClientOrderListModel>> GetOrderByFilterWithoutPaging(ClientOrderFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.SortColumn) || filter.SortColumn == "o.[OrderPlaceDateOnly]")
                filter.SortColumn = "o.[OrderPlaceDate]";

            if (string.IsNullOrWhiteSpace(filter.SortDirection))
                filter.SortDirection = "DESC";

            try
            {
                var queryString = String.Format("dbo.SP_Order_ClientOrder_GetListByFilterWithoutPaging '{0}','{1}','{2}'",
              filter.Where,
              filter.SortColumn,
              filter.SortDirection
              );

                return await _db.LoadDataUsingQuery<ClientOrderListModel, dynamic>(queryString,
                    new
                    {
                    });
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return null;
        }



        //public async Task<List<ClientOrderListModel>> GetAllOrderByFilterWithPaging(ClientOrderFilter filter)
        //{
        //    if (string.IsNullOrWhiteSpace(filter.SortColumn))
        //        filter.SortColumn = "o.[OrderPlaceDate]";

        //    if (string.IsNullOrWhiteSpace(filter.SortDirection))
        //        filter.SortDirection = "DESC";

        //    try
        //    {
        //        var queryString = String.Format("dbo.SP_Order_ClientOrder_GetAllListByFilter '{0}','{1}',{2},{3},'{4}','{5}'",
        //      filter.Where,
        //      filter.IsCalculateTotal.ToString().ToLower(),
        //      filter.Skip,
        //      filter.Top,
        //      filter.SortColumn,
        //      filter.SortDirection
        //      );

        //        var filteredList = await _db.LoadDataUsingQuery<ClientOrderListModel, dynamic>(queryString,
        //            new
        //            {
        //            });


        //        if (filteredList.Any() && filter.IsCalculateTotal)
        //        {
        //            filter.TotalCount = filteredList[0].TotalCount;
        //        }
        //        else if (!filteredList.Any() && filter.IsCalculateTotal)
        //        {
        //            filter.TotalCount = 0;
        //        }

        //        return filteredList;
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //    }

        //    return null;
        //}

        //public async Task<List<ClientOrderListModel>> GetAllOrderByFilterWithPagingForTeam(ClientOrderFilter filter)
        //{
        //    if (string.IsNullOrWhiteSpace(filter.SortColumn))
        //        filter.SortColumn = "o.[OrderPlaceDate]";

        //    if (string.IsNullOrWhiteSpace(filter.SortDirection))
        //        filter.SortDirection = "DESC";

        //    try
        //    {
        //        var queryString = String.Format("dbo.SP_Order_ClientOrder_GetAllListByFilterForTeam '{0}','{1}',{2},{3},'{4}','{5}'",
        //      filter.Where,
        //      filter.IsCalculateTotal.ToString().ToLower(),
        //      filter.Skip,
        //      filter.Top,
        //      filter.SortColumn,
        //      filter.SortDirection
        //      );

        //        var filteredList = await _db.LoadDataUsingQuery<ClientOrderListModel, dynamic>(queryString,
        //            new
        //            {
        //            });


        //        if (filteredList.Any() && filter.IsCalculateTotal)
        //        {
        //            filter.TotalCount = filteredList[0].TotalCount;
        //        }
        //        else if (!filteredList.Any() && filter.IsCalculateTotal)
        //        {
        //            filter.TotalCount = 0;
        //        }

        //        return filteredList;
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //    }

        //    return null;
        //}
        public async Task<List<ClientOrderModel>> GetAllByCompanyId(int companyId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_GetAllByCompanyId", new { companyId = companyId });
        }

        public async Task<List<ClientOrderModel>> GetAllByCompanyIdAndDates(int companyId,DateTime startDate, DateTime endDate)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_GetAllByCompanyIdAndDates", 
                new { 
                    companyId = companyId,
                    startDate = startDate,
                    endDate = endDate
                });
        }
        /// <summary>
        /// Get Order by Order Id
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<ClientOrderModel> GetById(long OrderId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_GetById", new { OrderId = OrderId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<ClientOrderModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert Order
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(ClientOrderModel Order)
        {
            var response = new Response<int>();
            try
            {
                // Add Order_ClientOrder
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_Insert", new
                {
                    Order.CompanyId,
                    Order.FileServerId,
                    Order.OrderNumber,
                    Order.OrderPlaceDate,
                    Order.CreatedDate,
                    Order.UpdatedDate,
                    Order.UpdatedByContactId,
                    Order.ObjectId,
                    Order.IsDeleted,
                    Order.ExternalOrderStatus,
                    Order.InternalOrderStatus,
                    Order.Instructions,
                    Order.AssignedTeamId,
                    Order.SourceServerId,
                    Order.BatchPath,
                });

                //Add Order_OrderClient_SOP_Template
                if (Order.SOPTemplateList != null && Order.SOPTemplateList.Any())
                {
                    foreach (var soptemplate in Order.SOPTemplateList)
                    {
                        soptemplate.Order_ClientOrder_Id = newId;
                        await _db.SaveDataUsingProcedureAndReturnId<long, dynamic>(storedProcedure: "[dbo].[SP_Order_ClientOrder_SOP_Template_Insert]", new
                        {
                            soptemplate.Order_ClientOrder_Id,
                            soptemplate.SOP_Template_Id,
                        });
                    }
                }

                Order.Id = newId;
                response.Result = newId;
                response.FileServerId = (short)Order.FileServerId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update Order
        /// </summary>
        /// <param name="Order"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(ClientOrderModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_Update", new
                {
                    Order.Id,
                    Order.CompanyId,
                    Order.FileServerId,
                    Order.OrderNumber,
                    Order.OrderPlaceDate,
                    Order.CreatedDate,
                    Order.UpdatedDate,
                    Order.ObjectId,
                    Order.IsDeleted,
                    Order.UpdatedByContactId,
                    Order.Instructions,
                    Order.DeliveryDeadlineInMinute,
                    //Order.AssignedTeamId // Changes The Logic Rakib Vai

                });
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_DeleteByOrderId", new { OrderId = Order.Id });
                if (Order.SOPTemplateList != null && Order.SOPTemplateList.Any())
                {
                    foreach (var soptemplate in Order.SOPTemplateList)
                    {
                        soptemplate.Order_ClientOrder_Id = (int)Order.Id;
                        await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_Insert", new
                        {
                            soptemplate.Order_ClientOrder_Id,
                            soptemplate.SOP_Template_Id,
                        });
                    }
                }
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> UpdateClientOrderListModel(ClientOrderListModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_StatusUpdate", new
                {
                    Order.Id,
                    Order.CompanyId,
                    Order.UpdatedDate,
                    Order.UpdatedByContactId,
                    Order.ExternalOrderStatus,
                    Order.InternalOrderStatus,
                    Order.AssignedTeamId

                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        //
        public async Task<Response<bool>> UpdateClientOrderOpsAndUpdateByListModel(ClientOrderListModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_OpsUpdate", new
                {
                    Order.Id,
                    Order.UpdatedByContactId,
                    Order.AssignedByOpsContactId,


                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        public async Task<Response<bool>> UpdateClientOrderStatus(ClientOrderModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_StatusUpdate", new
                {
                    Order.Id,
                    Order.CompanyId,
                    Order.UpdatedDate,
                    Order.UpdatedByContactId,
                    Order.ExternalOrderStatus,
                    Order.InternalOrderStatus,
                    Order.AssignedTeamId,
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> UpdateClientOrderCatgegorySetStatus(ClientOrderModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_CategorySetStatusUpdate", new
                {
                    Order.Id,
                   Order.CategorySetStatus
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        public async Task<Response<bool>> UpdateClientOrderArrivalTime(ClientOrderModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_UpdateArrivalTime", new
                {
                    Order.Id,
                    Order.ArrivalTime
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        /// <summary>
        /// Delete Order by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_DeleteByObjectId", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #endregion


        public async Task<Response<bool>> UpdateSingleField(long orderId, string fieldName, string fieldValue, int updatedByContactId)
        {
            string queryValue;

            if (fieldValue == null)
            {
                queryValue = "";
            }
            else
            {
                queryValue = $"{fieldValue.Replace("'", "''")}";
            }

            return await UpdateSingleFieldInDB(orderId, fieldName, queryValue, updatedByContactId);

        }
        private async Task<Response<bool>> UpdateSingleFieldInDB(long orderId, string fieldName, string fieldValue, int updatedByContactId)
        {
            var response = new Response<bool>();

            try
            {
                var updatedDate = DateTime.Now;
                var query = $"UPDATE [dbo].[Order_ClientOrder] SET [{fieldName}] = @fieldValue, UpdatedByContactId = @updatedByContactId, UpdatedDate = @updatedate  WHERE [Id] = @orderId";

                await _db.SaveDataUsingQuery(query,
                     new
                     {
                         orderId = orderId,
                         fieldValue = fieldValue,
                         updatedByContactId = updatedByContactId,
                         updatedate = DateTime.Now
                     });



                response.IsSuccess = true;
                response.Message = $"Successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }



            return response;
        }

        public async Task<Response<bool>> DeleteOrderSopTemplateByOrderIdAndSopTemplateId(long orderId, int sopTemplateId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_SOP_Template_Delete", new { OrderId = orderId, SopTemplatId = sopTemplateId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> UpdateOrderAllowExtraOutputFileUploadField(ClientOrderModel clientOrder)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_AllowExtraOutputFileUploadFieldUpdate", new
                {
                    clientOrder.Id,
                    clientOrder.AllowExtraOutputFileUpload,

                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        //Update Client Order Status

        public async Task<Response<bool>> UpdateClientOrder(ClientOrderModel order)
        {
            var response = new Response<bool>();
            CompanyTeamModel companyTeam = await _companyTeamService.GetTeamByCompanyId(order.CompanyId);
            if (companyTeam != null)
            {
                order.AssignedTeamId = companyTeam.TeamId;
            }
            await UpdateClientOrderUsingEntityModel(order);

            order.AssignedByOpsContactId = AutomatedAppConstant.ContactId;

            order.UpdatedByContactId = AutomatedAppConstant.ContactId; //Dummy

            await UpdateClientOrderOpsAndUpdateEntityModel(order);

            return response;
        }

        //Private Method
        private async Task<Response<bool>> UpdateClientOrderUsingEntityModel(ClientOrderModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_StatusUpdate", new
                {
                    Order.Id,
                    Order.CompanyId,
                    Order.UpdatedDate,
                    Order.UpdatedByContactId,
                    Order.ExternalOrderStatus,
                    Order.InternalOrderStatus,
                    Order.AssignedTeamId

                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        private async Task<Response<bool>> UpdateClientOrderOpsAndUpdateEntityModel(ClientOrderModel Order)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_OpsUpdate", new
                {
                    Order.Id,
                    Order.UpdatedByContactId,
                    Order.AssignedByOpsContactId,


                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<List<ClientOrderModel>> GetOrdersByOrderItemStatus(int companyId, string status)
        {
            string query = $"SELECT * FROM Order_ClientOrder WHERE ID IN (SELECT Distinct ci.ClientOrderId From Order_ClientOrderItem AS ci where ci.CompanyId=@CompanyId and ci.Status in ({status}))";

            var filteredList = await _db.LoadDataUsingQuery<ClientOrderModel, dynamic>(query,
                    new
                    {
                        CompanyId = companyId
                    });


            return filteredList;
        }

        public async Task<List<ClientOrderModel>> GetAllByStatus(int status, int companyId)
        {
            string query = $"SELECT * FROM Order_ClientOrder WHERE InternalOrderStatus = @InternalOrderStatus and CompanyId=@CompanyId ";
            var filteredList = await _db.LoadDataUsingQuery<ClientOrderModel, dynamic>(query,
                    new
                    {
                        CompanyId = companyId,
                        InternalOrderStatus = status
                    });


            return filteredList;
        }

		public async Task<bool> CheckExistenceOfBatchBySourceFullPath(string sourceFullPath)
		{
            int isFound = await _db.GetDataUsingProcedure<int, dynamic>(storedProcedure: "SP_Order_ClientOrder_CheckBatchNameExistence",new {BatchName = sourceFullPath });
            return isFound>0;
		}


        public async Task<bool> CheckBatchNameExistenceOnOrderPlacingStatus(string batchName)
        {

            ClientOrderSearchModel clientOrderSearch = new ClientOrderSearchModel()
            {
                SourceFullPath = batchName
            };

            int isFound = await _db.GetDataUsingProcedure<int, dynamic>(storedProcedure: "SP_Order_ClientOrder_CheckBatchNameExistenceOnOrderPlacingStatus", clientOrderSearch);


            return isFound > 0;
        }


        /// <summary>
        /// Get All table columns
        /// </summary>
        /// <returns></returns>
        public List<CustomTableColumnModel> GetAllTableColumns()
        {
            List<CustomTableColumnModel> columns = new List<CustomTableColumnModel>();
            int displayOrder = 1;

            columns.Add(new CustomTableColumnModel { Id = 1, DisplayName = "Company", FieldName = "CompanyName", IsAdminCompanyColumn = true, IsClientCompanyColumn = true, Width = 30, IsVisible = true, IsEditable = false, FieldTypeEnum = TableFieldType.Boolean, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 2, DisplayName = "Order Number", FieldName = "OrderNumber", IsAdminCompanyColumn = true, IsClientCompanyColumn = true, Width = 30, IsVisible = true, IsEditable = false, FieldTypeEnum = TableFieldType.Boolean, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 3, DisplayName = "Status", FieldName = "InternalOrderStatus", IsAdminCompanyColumn = true, IsClientCompanyColumn = false, Width = 130, IsVisible = true, IsEditable = false, FieldTypeEnum = TableFieldType.ShortText, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 4, DisplayName = "Status", FieldName = "ExternalOrderStatus", IsAdminCompanyColumn = false, IsClientCompanyColumn = true, Width = 300, IsVisible = true, IsEditable = true, FieldTypeEnum = TableFieldType.ShortText, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 5, DisplayName = "Images", FieldName = "NumberOfImage", IsAdminCompanyColumn = true, IsClientCompanyColumn = true, Width = 50, IsVisible = true, IsEditable = false, FieldTypeEnum = TableFieldType.ShortText, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 6, DisplayName = "Order Place Date", FieldName = "OrderPlaceDateOnly", IsAdminCompanyColumn = true, IsClientCompanyColumn = true, Width = 120, IsVisible = true, IsEditable = false, FieldTypeEnum = TableFieldType.Boolean, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 7, DisplayName = "Allow Extra Files", FieldName = "AllowExtraOutputFileUpload", IsAdminCompanyColumn = true, IsClientCompanyColumn = true, Width = 120, IsVisible = false, IsEditable = true, FieldTypeEnum = TableFieldType.Boolean, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 8, DisplayName = "Assigned By (OPS)", FieldName = "ContactName", IsAdminCompanyColumn = true, IsClientCompanyColumn = true, Width = 160, IsVisible = true, IsEditable = false, FieldTypeEnum = TableFieldType.Dropdown, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 9, DisplayName = "Ass. Team", FieldName = "TeamName", IsAdminCompanyColumn = true, IsClientCompanyColumn = false, Width = 160, IsVisible = true, IsEditable = true, FieldTypeEnum = TableFieldType.Dropdown, DisplayOrder = displayOrder++ });
            columns.Add(new CustomTableColumnModel { Id = 10, DisplayName = "Team Ass. Date", FieldName = "TeamAssignedDate", IsAdminCompanyColumn = true, IsClientCompanyColumn = false, Width = 160, IsVisible = true, IsEditable = true, FieldTypeEnum = TableFieldType.Multiselect, DisplayOrder = displayOrder++ });

            //Last Id 95
            return columns.OrderBy(f => f.Id).ToList();
        }

        public async Task UpdateOrder(ClientOrderModel clientOrder, InternalOrderStatus status)
        {
            if (status == InternalOrderStatus.AssignedForSupport)
            {
                status = InternalOrderStatus.Assigned;
            }
            clientOrder.InternalOrderStatus = (byte)status;
            clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(status));
            await UpdateClientOrderStatus(clientOrder);

            //await AddOrderStatusChangeLog(clientOrder, status);
        }

        public async Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus, LoginUserInfoViewModel loginUser)
        {
            var previousLog = await _orderStatusChangeLogService.OrderStatusLastChangeLogByOrderId((int)clientOrder.Id);
            if (previousLog.NewInternalStatus != (byte)internalOrderStatus)
            {
                OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
                {
                    OrderId = (int)clientOrder.Id,
                    NewInternalStatus = (byte)internalOrderStatus,
                    NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
                    ChangeByContactId = loginUser.ContactId,
                    ChangeDate = DateTime.Now
                };

                if (previousLog != null)
                {
                    orderStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
                    orderStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
                    orderStatusChangeLog.TimeDurationInMinutes = (int)(orderStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
                }
                await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
            }

        }

        public async Task<Response<bool>> UpdateOrderDeadline(ClientOrderModel clientOrder)
        {
			var response = new Response<bool>();
			try
            {
                //Todo: Rakib 
                //DateTime arrivalDateTime = clientOrder.ArrivalTime ?? clientOrder.CreatedDate;
                //clientOrder.ExpectedDeliveryDate = arrivalDateTime.AddMinutes((int)clientOrder.DeliveryDeadlineInMinute);

                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_UpdateOrderDeadLine", new
                {
                    clientOrder.Id,
					clientOrder.DeliveryDeadlineInMinute,
                    //clientOrder.ExpectedDeliveryDate
                });
				response.IsSuccess = true;
				response.Message = StandardDataAccessMessages.SuccessMessaage;
			}
            catch (Exception ex) 
            {

            }
			return response;
		}

        public async Task<Response<bool>> UpdateOrderDeadlineDate(long clientOrderId)
        {
            var response = new Response<bool>();

            var itemDeliveryDate = await GetClientOrderItemMinMaxDeliveryDate(clientOrderId);

            try
            {
                //Todo: Rakib 

                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrder_UpdateOrderDeadLineDate", new
                {
                    clientOrderId,
                    itemDeliveryDate.MinDeliveryDate
                });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {

            }
            return response;
        }


        private async Task<ClientOrderItemDeliveryDate> GetClientOrderItemMinMaxDeliveryDate(long clientOrderId)
        {
            var items = new List<ClientOrderItemDeliveryDate>();
            try
            {
                //Todo: Rakib 

                items = await _db.LoadDataUsingProcedure<ClientOrderItemDeliveryDate,dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItemsMinDeliveryDateByOrderId", new
                {
                    clientOrderId,
                });
               
            }
            catch (Exception ex)
            {

            }
            return items.FirstOrDefault();
        }

        public async Task<Response<ClientOrderModel>> CheckBatchNameExistOnClientOrder(int companyId, string batchPath)
        {
            var response = new Response<ClientOrderModel>();
            try
            {
                var result = await _db.LoadDataUsingProcedure<ClientOrderModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_CheckBatchExistance", new 
                { 
                    companyId,
                    @batchPath,
                });
                
                response.Result = result.FirstOrDefault();
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                throw;
            }
            return response;
        }
    }
}