using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.ClientOrders
{
    public class OrderFileAttachmentService : IOrderFileAttachmentService
    {
        private readonly ISqlDataAccess _db;

        public OrderFileAttachmentService(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<Response<int>> Insert(List<OrderFileAttachment> orderAttachments, int orderId)
        {
            var newFileId = 0;
            var response = new Response<int>();
            foreach (var file in orderAttachments)
            {
                try
                {
                    file.Order_ClientOrder_Id = orderId;
                  newFileId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Order_OrderFileAttachment_insert", new
                    {
                        file.CompanyId,
                        file.FileName,
                        file.Order_ClientOrder_Id,
                        file.PartialPath,
                        file.Status,
                        file.IsDeleted,
                        file.CreateDated,
                        file.ObjectId,
                        file.FileSize

                    });
                    
                }
                catch (Exception ex)
                {
                }
            }
            response.Result = newFileId;
            return response;
        }
        public async Task<List<OrderFileAttachment>> GetOrdersAttachementByOrderId(int orderId)
        {
            return await _db.LoadDataUsingProcedure<OrderFileAttachment, dynamic>(storedProcedure: "dbo.SP_Order_OrderFile_GetByOrderId", new { OrderId = orderId });
        }
        public async Task<List<OrderFileAttachment>> GetOrdersAttachementById(int Id)
        {
            return await _db.LoadDataUsingProcedure<OrderFileAttachment, dynamic>(storedProcedure: "dbo.SP_Order_OrderFile_GetById", new { Id = Id });
        }
        public async Task<Response<bool>> Delete(int attachmentId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_OrderFileAttachment_DeleteByAttachmentId", new { AttachmentId = attachmentId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }


    }
}
