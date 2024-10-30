using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.OrderTeamServices
{
    public class OrderAssignedImageEditorService : IOrderAssignedImageEditorService
    {
        private readonly ISqlDataAccess _db;

        public OrderAssignedImageEditorService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<int>> Insert(List<OrderAssignedImageEditorModel> orderAssignedImageEditors)
        {
            var response = new Response<int>();
            try
            {
                foreach (var assignImage in orderAssignedImageEditors)
                {
                    var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Order_AssignedImageEditor_Insert", new
                    {
                        assignImage.OrderId,
                        assignImage.AssignContactId,
                        assignImage.AssignByContactId,
                        assignImage.ObjectId,
                        assignImage.Order_ImageId,
                        assignImage.UpdatedByContactId
                    });

                }
                //orderTeam.Id = newId;
                //response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Core.Response<bool>> Delete(int orderItemId)
        {
            var response = new Core.Response<bool>();

            try
            {
                int rows= await _db.SaveDataUsingProcedureAndReturnNumberOfEffectedRow(storedProcedure: "dbo.SP_Order_Order_AssignedImageEditor_DeleteByAssignContactIdAndOrderImageId", new {OrderItemId = orderItemId });
                
                if(rows > 0)
                {
					response.IsSuccess = true;
					response.Message = StandardDataAccessMessages.SuccessMessaage;
				}
                
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }
            return response;
        }

		public async Task<Response<OrderAssignedImageEditorModel>> CheckClientOrderItemFileAssignEditor(OrderAssignedImageEditorModel orderAssignedImageEditor)
        {
           var response = new Response<OrderAssignedImageEditorModel>();
           var orderAssignImageEditorList = await _db.LoadDataUsingProcedure<OrderAssignedImageEditorModel, dynamic>("dbo.SP_Order_AssignedImageEditor_GetByOrder_ImageId", new
            {
                OrderId = orderAssignedImageEditor.OrderId,
                AssignContactId = orderAssignedImageEditor.AssignContactId,
                Order_ImageId = orderAssignedImageEditor.Order_ImageId
            });

			response.Result = orderAssignImageEditorList.FirstOrDefault();
            if(response.Result != null)
            {
                response.IsSuccess = true;
            }

            return response;

		}

	}
}
